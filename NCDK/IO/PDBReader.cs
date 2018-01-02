/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *  */
using NCDK.Config;
using NCDK.Default;
using NCDK.Graphs.Rebond;
using NCDK.IO.Formats;
using NCDK.IO.Setting;
using NCDK.Numerics;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NCDK.IO
{
    /// <summary>
    /// Reads the contents of a PDBFile.
    /// <para>A description can be found at <see href="http://www.rcsb.org/pdb/static.do?p=file_formats/pdb/index.html">http://www.rcsb.org/pdb/static.do?p=file_formats/pdb/index.html</see>.</para>
    /// </summary>
    // @cdk.module  pdb
    // @cdk.githash
    // @cdk.iooptions
    // @author      Edgar Luttmann
    // @author      Bradley Smith <bradley@baysmith.com>
    // @author      Martin Eklund <martin.eklund@farmbio.uu.se>
    // @author      Ola Spjuth <ola.spjuth@farmbio.uu.se>
    // @author      Gilleain Torrance <gilleain.torrance@gmail.com>
    // @cdk.created 2001-08-06
    // @cdk.keyword file format, PDB
    // @cdk.bug     1714141
    // @cdk.bug     1794439
    public class PDBReader : DefaultChemObjectReader
    {
        private TextReader oInput;                                                                  // The internal used TextReader
        private BooleanIOSetting useRebondTool;
        private BooleanIOSetting readConnect;
        private BooleanIOSetting useHetDictionary;

        private IDictionary<int, IAtom> atomNumberMap;

        /// <summary>
        /// This is a temporary store for bonds from CONNECT records. As CONNECT is
        /// deliberately fully redundant (a->b and b->a) we need to use this to weed
        /// out the duplicates.
        /// </summary>
        private List<IBond> bondsFromConnectRecords;

        /// <summary>
        /// A mapping between HETATM 3-letter codes + atomNames to CDK atom type
        /// names; for example "RFB.N13" maps to "N.planar3".
        /// </summary>
        private IDictionary<string, string> hetDictionary;

        private AtomTypeFactory cdkAtomTypeFactory;
        private const string hetDictionaryPath = "type_map.txt";

        /// <summary>
        /// Constructs a new PDBReader that can read Molecules from a given Stream.
        /// </summary>
        /// <param name="oIn">The Stream to read from</param>
        public PDBReader(Stream oIn)
            : this(new StreamReader(oIn))
        { }

        /// <summary>
        /// Constructs a new PDBReader that can read Molecules from a given Reader.
        /// </summary>
        /// <param name="oIn">The Reader to read from</param>
        public PDBReader(TextReader oIn)
        {
            oInput = oIn;
            InitIOSettings();
            hetDictionary = null;
            cdkAtomTypeFactory = null;
        }

        public override IResourceFormat Format => PDBFormat.Instance;

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Takes an object which subclasses IChemObject, e.g. Molecule, and will
        /// read this (from file, database, internet etc). If the specific
        /// implementation does not support a specific IChemObject it will throw
        /// an Exception.
        /// </summary>
        /// <param name="oObj">The object that subclasses IChemObject</param>
        /// <returns>The IChemObject read</returns>
        /// <exception cref="CDKException"></exception>
        public override T Read<T>(T oObj)
        {
            if (oObj is IChemFile)
            {
                return (T)ReadChemFile((IChemFile)oObj);
            }
            else
            {
                throw new CDKException("Only supported is reading of ChemFile objects.");
            }
        }

        /// <summary>
        /// Read a <see cref="IChemFile"/> from a file in PDB format. The molecules
        /// in the file are stored as <see cref="IBioPolymer"/>s in the
        /// <see cref="IChemFile"/>. The residues are the monomers of the
        /// <see cref="IBioPolymer"/>, and their names are the concatenation of the
        /// residue, chain id, and the sequence number. Separate chains (denoted by
        /// TER records) are stored as separate <see cref="IBioPolymer"/> molecules.
        /// </summary>
        /// <remarks>
        /// Connectivity information is not currently read.
        /// </remarks>
        /// <returns>The ChemFile that was read from the PDB file.</returns>
        private IChemFile ReadChemFile(IChemFile oFile)
        {
            // initialize all containers
            IChemSequence oSeq = oFile.Builder.NewChemSequence();
            IChemModel oModel = oFile.Builder.NewChemModel();
            var oSet = oFile.Builder.NewAtomContainerSet();

            // some variables needed
            string cCol;
            PDBAtom oAtom;
            PDBPolymer oBP = new PDBPolymer();
            IAtomContainer molecularStructure = oFile.Builder.NewAtomContainer();
            StringBuilder cResidue;
            string oObj;
            IMonomer oMonomer;
            string cRead = "";
            char chain = 'A'; // To ensure stringent name giving of monomers
            IStrand oStrand;
            int lineLength = 0;

            bool isProteinStructure = false;

            atomNumberMap = new Dictionary<int, IAtom>();
            if (readConnect.IsSet)
            {
                bondsFromConnectRecords = new List<IBond>();
            }

            // do the reading of the Input
            try
            {
                do
                {
                    cRead = oInput.ReadLine();
                    Debug.WriteLine("Read line: ", cRead);
                    if (cRead != null)
                    {
                        lineLength = cRead.Length;

                        if (lineLength < 80)
                        {
                            Trace.TraceWarning("Line is not of the expected length 80!");
                        }

                        // make sure the record name is 6 characters long
                        if (lineLength < 6)
                        {
                            cRead = cRead + "      ";
                        }
                        // check the first column to decide what to do
                        cCol = cRead.Substring(0, 6);
                        switch (cCol.ToUpperInvariant())
                        {
                            case "SEQRES":
                                {
                                    isProteinStructure = true;
                                }
                                break;
                            case "ATOM  ":
                                {
                                    #region
                                    // read an atom record
                                    oAtom = ReadAtom(cRead, lineLength);

                                    if (isProteinStructure)
                                    {
                                        // construct a string describing the residue
                                        cResidue = new StringBuilder(8);
                                        oObj = oAtom.ResName;
                                        if (oObj != null)
                                        {
                                            cResidue = cResidue.Append(oObj.Trim());
                                        }
                                        oObj = oAtom.ChainID;
                                        if (oObj != null)
                                        {
                                            // cResidue = cResidue.Append(((string)oObj).Trim());
                                            cResidue = cResidue.Append(chain);
                                        }
                                        oObj = oAtom.ResSeq;
                                        if (oObj != null)
                                        {
                                            cResidue = cResidue.Append(oObj.Trim());
                                        }

                                        // search for an existing strand or create a new one.
                                        string strandName = oAtom.ChainID;
                                        if (strandName == null || strandName.Length == 0)
                                        {
                                            strandName = chain.ToString();
                                        }
                                        oStrand = oBP.GetStrand(strandName);
                                        if (oStrand == null)
                                        {
                                            oStrand = new PDBStrand
                                            {
                                                StrandName = strandName,
                                                Id = chain.ToString()
                                            };
                                        }

                                        // search for an existing monomer or create a new one.
                                        oMonomer = oBP.GetMonomer(cResidue.ToString(), chain.ToString());
                                        if (oMonomer == null)
                                        {
                                            PDBMonomer monomer = new PDBMonomer
                                            {
                                                MonomerName = cResidue.ToString(),
                                                MonomerType = oAtom.ResName,
                                                ChainID = oAtom.ChainID,
                                                ICode = oAtom.ICode,
                                                ResSeq = oAtom.ResSeq
                                            };
                                            oMonomer = monomer;
                                        }

                                        // add the atom
                                        oBP.AddAtom(oAtom, oMonomer, oStrand);
                                    }
                                    else
                                    {
                                        molecularStructure.Atoms.Add(oAtom);
                                    }

                                    if (readConnect.IsSet)
                                    {
                                        bool isDup = atomNumberMap.ContainsKey(oAtom.Serial.Value);
                                        atomNumberMap[oAtom.Serial.Value] = oAtom;
                                        if (isDup)
                                            Trace.TraceWarning("Duplicate serial ID found for atom: ", oAtom);
                                    }
                                    Debug.WriteLine("Added ATOM: ", oAtom);

                                    // As HETATMs cannot be considered to either belong to a certain monomer or strand,
                                    // they are dealt with seperately.
                                    #endregion
                                }
                                break;
                            case "HETATM":
                                {
                                    #region
                                    // read an atom record
                                    oAtom = ReadAtom(cRead, lineLength);
                                    oAtom.HetAtom = true;
                                    if (isProteinStructure)
                                    {
                                        oBP.Atoms.Add(oAtom);
                                    }
                                    else
                                    {
                                        molecularStructure.Atoms.Add(oAtom);
                                    }
                                    var isDup = atomNumberMap.ContainsKey(oAtom.Serial.Value);
                                    atomNumberMap[oAtom.Serial.Value] = oAtom;
                                    if (isDup)
                                        Trace.TraceWarning($"Duplicate serial ID found for atom: {oAtom}");

                                    Debug.WriteLine($"Added HETATM: {oAtom}");
                                    #endregion
                                }
                                break;
                            case "TER   ":
                                {
                                    #region
                                    // start new strand
                                    chain++;
                                    oStrand = new PDBStrand
                                    {
                                        StrandName = chain.ToString()
                                    };
                                    Debug.WriteLine("Added new STRAND");
                                    #endregion
                                }
                                break;
                            case "END   ":
                                {
                                    #region
                                    atomNumberMap.Clear();
                                    if (isProteinStructure)
                                    {
                                        // create bonds and finish the molecule
                                        oSet.Add(oBP);
                                        if (useRebondTool.IsSet)
                                        {
                                            try
                                            {
                                                if (!CreateBondsWithRebondTool(oBP))
                                                {
                                                    // Get rid of all potentially created bonds.
                                                    Trace.TraceInformation("Bonds could not be created using the RebondTool when PDB file was read.");
                                                    oBP.Bonds.Clear();
                                                }
                                            }
                                            catch (Exception exception)
                                            {
                                                Trace.TraceInformation("Bonds could not be created when PDB file was read.");
                                                Debug.WriteLine(exception);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (useRebondTool.IsSet) CreateBondsWithRebondTool(molecularStructure);
                                        oSet.Add(molecularStructure);
                                    }
                                    #endregion
                                }
                                break;
                            case "MODEL ":
                                {
                                    #region
                                    // OK, start a new model and save the current one first *if* it contains atoms
                                    if (isProteinStructure)
                                    {
                                        if (oBP.Atoms.Count > 0)
                                        {
                                            // save the model
                                            oSet.Add(oBP);
                                            oModel.MoleculeSet = oSet;
                                            oSeq.Add(oModel);
                                            // setup a new one
                                            oBP = new PDBPolymer();
                                            oModel = oFile.Builder.NewChemModel();
                                            oSet = oFile.Builder.NewAtomContainerSet();
                                        }
                                    }
                                    else
                                    {
                                        if (molecularStructure.Atoms.Count > 0)
                                        {
                                            //                                 save the model
                                            oSet.Add(molecularStructure);
                                            oModel.MoleculeSet = oSet;
                                            oSeq.Add(oModel);
                                            // setup a new one
                                            molecularStructure = oFile.Builder.NewAtomContainer();
                                            oModel = oFile.Builder.NewChemModel();
                                            oSet = oFile.Builder.NewAtomContainerSet();
                                        }
                                    }
                                    #endregion
                                }
                                break;
                            case "REMARK":
                                {
                                    #region
                                    var comment = oFile.GetProperty<string>(CDKPropertyName.Comment, "");
                                    if (lineLength > 12)
                                    {
                                        comment = comment + cRead.Substring(11).Trim()
                                                + "\n";
                                        oFile.SetProperty(CDKPropertyName.Comment, comment);
                                    }
                                    else
                                    {
                                        Trace.TraceWarning("REMARK line found without any comment!");
                                    }
                                    #endregion
                                }
                                break;
                            case "COMPND":
                                {
                                    #region
                                    string title = cRead.Substring(10).Trim();
                                    oFile.SetProperty(CDKPropertyName.Title, title);
                                    #endregion
                                }
                                break;
                            case "CONECT":
                                {
                                    #region
                                    // ***********************************************************
                                    // Read connectivity information from CONECT records. Only
                                    // covalent bonds are dealt with. Perhaps salt bridges
                                    // should be dealt with in the same way..?
                                    if (!readConnect.IsSet)
                                        break;
                                    cRead.Trim();
                                    if (cRead.Length < 16)
                                    {
                                        Debug.WriteLine("Skipping unexpected empty CONECT line! : ", cRead);
                                    }
                                    else
                                    {
                                        int lineIndex = 6;
                                        int atomFromNumber = -1;
                                        int atomToNumber = -1;
                                        IAtomContainer molecule = (isProteinStructure) ? oBP : molecularStructure;
                                        while (lineIndex + 5 <= cRead.Length)
                                        {
                                            string part = cRead.Substring(lineIndex, 5).Trim();
                                            if (atomFromNumber == -1)
                                            {
                                                try
                                                {
                                                    atomFromNumber = int.Parse(part);
                                                }
                                                catch (FormatException)
                                                {
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    atomToNumber = int.Parse(part);
                                                }
                                                catch (FormatException)
                                                {
                                                    atomToNumber = -1;
                                                }
                                                if (atomFromNumber != -1 && atomToNumber != -1)
                                                {
                                                    AddBond(molecule, atomFromNumber, atomToNumber);
                                                    Trace.TraceWarning("Bonded " + atomFromNumber + " with " + atomToNumber);
                                                }
                                            }
                                            lineIndex += 5;
                                        }
                                    }
                                    // **********************************************************
                                    #endregion
                                }
                                break;
                            case "HELIX ":
                                {
                                    #region
                                    //                        HELIX    1 H1A CYS A   11  LYS A   18  1 RESIDUE 18 HAS POSITIVE PHI    1D66  72
                                    //                                  1         2         3         4         5         6         7
                                    //                        01234567890123456789012345678901234567890123456789012345678901234567890123456789
                                    PDBStructure structure = new PDBStructure
                                    {
                                        StructureType = PDBStructure.Helix,
                                        StartChainID = cRead[19],
                                        StartSequenceNumber = int.Parse(cRead.Substring(21, 4).Trim()),
                                        StartInsertionCode = cRead[25],
                                        EndChainID = cRead[31],
                                        EndSequenceNumber = int.Parse(cRead.Substring(33, 4).Trim()),
                                        EndInsertionCode = cRead[37]
                                    };
                                    oBP.Add(structure);
                                    #endregion
                                }
                                break;
                            case "SHEET ":
                                {
                                    #region
                                    PDBStructure structure = new PDBStructure
                                    {
                                        StructureType = PDBStructure.Sheet,
                                        StartChainID = cRead[21],
                                        StartSequenceNumber = int.Parse(cRead.Substring(22, 4).Trim()),
                                        StartInsertionCode = cRead[26],
                                        EndChainID = cRead[32],
                                        EndSequenceNumber = int.Parse(cRead.Substring(33, 4).Trim()),
                                        EndInsertionCode = cRead[37]
                                    };
                                    oBP.Add(structure);
                                    #endregion
                                }
                                break;
                            case "TURN  ":
                                {
                                    #region
                                    PDBStructure structure = new PDBStructure
                                    {
                                        StructureType = PDBStructure.Turn,
                                        StartChainID = cRead[19],
                                        StartSequenceNumber = int.Parse(cRead.Substring(20, 4).Trim()),
                                        StartInsertionCode = cRead[24],
                                        EndChainID = cRead[30],
                                        EndSequenceNumber = int.Parse(cRead.Substring(31, 4).Trim()),
                                        EndInsertionCode = cRead[35]
                                    };
                                    oBP.Add(structure);
                                    #endregion
                                }
                                break;
                            default:
                                break;  // ignore all other commands
                        }
                    }
                } while (cRead != null);
            }
            catch (Exception e)
            {
                if (e is IOException || e is ArgumentException)
                {
                    Trace.TraceError("Found a problem at line:");
                    Trace.TraceError(cRead);
                    Trace.TraceError("01234567890123456789012345678901234567890123456789012345678901234567890123456789");
                    Trace.TraceError("          1         2         3         4         5         6         7         ");
                    Trace.TraceError("  error: " + e.Message);
                    Debug.WriteLine(e);
                    Console.Error.WriteLine(e.StackTrace);
                }
                else
                    throw;
            }

            // try to close the Input
            try
            {
                oInput.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            // Set all the dependencies
            oModel.MoleculeSet = oSet;
            oSeq.Add(oModel);
            oFile.Add(oSeq);

            return oFile;
        }

        private void AddBond(IAtomContainer molecule, int bondAtomNo, int bondedAtomNo)
        {
            if (!atomNumberMap.TryGetValue(bondAtomNo, out IAtom firstAtom))
                Trace.TraceError("Could not find bond start atom in map with serial id: ", bondAtomNo);
            if (!atomNumberMap.TryGetValue(bondedAtomNo, out IAtom secondAtom))
                Trace.TraceError("Could not find bond target atom in map with serial id: ", bondAtomNo);
            IBond bond = firstAtom.Builder.NewBond(firstAtom, secondAtom, BondOrder.Single);
            for (int i = 0; i < bondsFromConnectRecords.Count; i++)
            {
                IBond existingBond = (IBond)bondsFromConnectRecords[i];
                IAtom a = existingBond.Begin;
                IAtom b = existingBond.End;
                if ((a == firstAtom && b == secondAtom) || (b == firstAtom && a == secondAtom))
                {
                    // already stored
                    return;
                }
            }
            bondsFromConnectRecords.Add(bond);
            molecule.Bonds.Add(bond);
        }

        private bool CreateBondsWithRebondTool(IAtomContainer molecule)
        {
            RebondTool tool = new RebondTool(2.0, 0.5, 0.5);
            try
            {
                //             configure atoms
                AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Config.Data.jmol_atomtypes.txt",
                        molecule.Builder);
                foreach (var atom in molecule.Atoms)
                {
                    try
                    {
                        var types = factory.GetAtomTypes(atom.Symbol);
                        var type = types.FirstOrDefault();
                        if (type != null)
                        {
                            // just pick the first one
                            AtomTypeManipulator.Configure(atom, type);
                        }
                        else
                        {
                            Trace.TraceWarning("Could not configure atom with symbol: " + atom.Symbol);
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceWarning("Could not configure atom (but don't care): " + e.Message);
                        Debug.WriteLine(e);
                    }
                }
                tool.Rebond(molecule);
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not rebond the polymer: " + e.Message);
                Debug.WriteLine(e);
            }
            return true;
        }

        private static bool IsUpper(char c)
        {
            return c >= 'A' && c <= 'Z';
        }
        private static bool IsLower(char c)
        {
            return c >= 'a' && c <= 'z';
        }
        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private string ParseAtomSymbol(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            int len = str.Length;

            StringBuilder sym = new StringBuilder();

            // try grabbing from end of line

            if (len > 76 && IsUpper(str[76]))
            {
                sym.Append(str[76]);
                if (len > 77 && IsUpper(str[77]))
                    sym.Append(str.Substring(77, 1).ToLowerInvariant());
                else if (len > 77 && IsLower(str[77]))
                    sym.Append(str.Substring(77, 1).ToLowerInvariant());
            }
            else if (len > 76 && str[76] == ' ')
            {
                if (len > 77 && IsUpper(str[77]))
                    sym.Append(str[77]);
            }

            if (sym.Length > 0)
                return sym.ToString();

            // try getting from PDB atom name
            if (len > 13 && IsUpper(str[13]))
            {
                if (str[12] == ' ')
                {
                    sym.Append(str[13]);
                    if (IsLower(str[14]))
                        sym.Append(str[14]);
                }
                else if (IsUpper(str[12]))
                {
                    if (str[0] == 'A' && str[12] == 'H')
                    {
                        sym.Append('H'); // ATOM record H is always H
                    }
                    else
                    {
                        sym.Append(str[12]);
                        sym.Append(str.Substring(13, 1).ToLowerInvariant());
                    }
                }
                else if (IsDigit(str[12]))
                {
                    sym.Append(str[13]);
                }
            }

            if (sym.Length > 0)
                return sym.ToString();

            return null;
        }

        /// <summary>
        /// Creates an <see cref="PDBAtom"/> and sets properties to their values from
        /// the ATOM or HETATM record. If the line is shorter than 80 characters, the
        /// information past 59 characters is treated as optional. If the line is
        /// shorter than 59 characters, a <see cref="ApplicationException"/> is thrown.
        /// </summary>
        /// <param name="cLine">the PDB ATOM or HEATATM record.</param>
        /// <param name="lineLength"></param>
        /// <returns>the <see cref="PDBAtom"/>created from the record.</returns>
        /// <exception cref="InvalidDataException">if the line is too short (less than 59 characters).</exception>
        private PDBAtom ReadAtom(string cLine, int lineLength)
        {
            // a line can look like (two in old format, then two in new format):
            //
            //           1         2         3         4         5         6         7
            // 01234567890123456789012345678901234567890123456789012345678901234567890123456789
            // ATOM      1  O5*   C A   1      20.662  36.632  23.475  1.00 10.00      114D  45
            // ATOM   1186 1H   ALA     1      10.105   5.945  -6.630  1.00  0.00      1ALE1288
            // ATOM     31  CA  SER A   3      -0.891  17.523  51.925  1.00 28.64           C
            // HETATM 3486 MG    MG A 302      24.885  14.008  59.194  1.00 29.42          MG+2
            //
            // note: the first two examples have the PDBID in col 72-75
            // note: the last two examples have the element symbol in col 76-77
            // note: the last (Mg hetatm) has a charge in col 78-79

            if (lineLength < 59)
            {
                throw new InvalidDataException("PDBReader error during ReadAtom(): line too short");
            }

            bool isHetatm = cLine.Substring(0, 6).Equals("HETATM");
            string atomName = cLine.Substring(12, 4).Trim();
            string resName = cLine.Substring(17, 3).Trim();
            string symbol = ParseAtomSymbol(cLine);

            if (symbol == null)
                HandleError("Cannot parse symbol from " + atomName);

            PDBAtom oAtom = new PDBAtom(symbol, new Vector3(double.Parse(cLine.Substring(30, 8)),
                        double.Parse(cLine.Substring(38, 8)), double.Parse(cLine.Substring(46, 8))));
            if (useHetDictionary.IsSet && isHetatm)
            {
                string cdkType = TypeHetatm(resName, atomName);
                oAtom.AtomTypeName = cdkType;
                if (cdkType != null)
                {
                    try
                    {
                        cdkAtomTypeFactory.Configure(oAtom);
                    }
                    catch (CDKException)
                    {
                        Trace.TraceWarning("Could not configure", resName, " ", atomName);
                    }
                }
            }

            oAtom.Record = cLine;
            oAtom.Serial = int.Parse(cLine.Substring(6, 5).Trim());
            oAtom.Name = atomName.Trim();
            oAtom.AltLoc = cLine.Substring(16, 1).Trim();
            oAtom.ResName = resName;
            oAtom.ChainID = cLine.Substring(21, 1).Trim();
            oAtom.ResSeq = cLine.Substring(22, 4).Trim();
            oAtom.ICode = cLine.Substring(26, 1).Trim();
            if (useHetDictionary.IsSet && isHetatm)
            {
                oAtom.Id = oAtom.ResName + "." + atomName;
            }
            else
            {
                oAtom.AtomTypeName = oAtom.ResName + "." + atomName;
            }
            if (lineLength >= 59)
            {
                string frag = cLine.Substring(54, Math.Min(lineLength - 54, 6)).Trim();
                if (frag.Length > 0)
                {
                    oAtom.Occupancy = double.Parse(frag);
                }
            }
            if (lineLength >= 65)
            {
                string frag = cLine.Substring(60, Math.Min(lineLength - 60, 6)).Trim();
                if (frag.Length > 0)
                {
                    oAtom.TempFactor = double.Parse(frag);
                }
            }
            if (lineLength >= 75)
            {
                oAtom.SegID = cLine.Substring(72, Math.Min(lineLength - 72, 4)).Trim();
            }
            //        if (lineLength >= 78) {
            //            oAtom.Symbol = (new string(cLine.Substring(76, 78))).Trim();
            //        }
            if (lineLength >= 79)
            {
                string frag;
                if (lineLength >= 80)
                {
                    frag = cLine.Substring(78, 2).Trim();
                }
                else
                {
                    frag = cLine.Substring(78);
                }
                if (frag.Length > 0)
                {
                    // see Format_v33_A4.pdf, p. 178
                    if (frag.EndsWith("-") || frag.EndsWith("+"))
                    {
                        var aa = frag.ToCharArray();
                        Array.Reverse(aa);
                        oAtom.Charge = double.Parse(new string(aa));
                    }
                    else
                    {
                        oAtom.Charge = double.Parse(frag);
                    }
                }
            }

            // ***********************************************************************************
            // It sets a flag in the property content of an atom, which is used when
            // bonds are created to check if the atom is an OXT-record => needs
            // special treatment.
            string oxt = cLine.Substring(13, 3).Trim();

            if (oxt.Equals("OXT"))
            {
                oAtom.Oxt = true;
            }
            else
            {
                oAtom.Oxt = false;
            }
            // ********************************************************************************** 

            return oAtom;
        }

        private string TypeHetatm(string resName, string atomName)
        {
            if (hetDictionary == null)
            {
                ReadHetDictionary();
                cdkAtomTypeFactory = AtomTypeFactory.GetInstance("NCDK.Dict.Data.cdk-atom-types.owl",
                        Default.ChemObjectBuilder.Instance);
            }
            string key = resName + "." + atomName;
            if (hetDictionary.ContainsKey(key))
            {
                return hetDictionary[key];
            }
            return null;
        }

        private void ReadHetDictionary()
        {
            try
            {
                Stream ins = ResourceLoader.GetAsStream(GetType(), hetDictionaryPath);
                TextReader bufferedReader = new StreamReader(ins);
                hetDictionary = new Dictionary<string, string>();
                string line;
                while ((line = bufferedReader.ReadLine()) != null)
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex == -1) continue;
                    string typeKey = line.Substring(0, colonIndex);
                    string typeValue = line.Substring(colonIndex + 1);
                    if (typeValue.Equals("null"))
                    {
                        hetDictionary[typeKey] = null;
                    }
                    else
                    {
                        hetDictionary[typeKey] = typeValue;
                    }
                }
                bufferedReader.Close();
            }
            catch (IOException ioe)
            {
                Trace.TraceError(ioe.Message);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    oInput.Dispose();
                }

                oInput = null;

                disposedValue = true;
                base.Dispose(disposing);
            }
        }
        #endregion

        private void InitIOSettings()
        {
            useRebondTool = IOSettings.Add(new BooleanIOSetting("UseRebondTool", IOSetting.Importance.Low,
                    "Should the PDBReader deduce bonding patterns?", "false"));
            readConnect = IOSettings.Add(new BooleanIOSetting("ReadConnectSection", IOSetting.Importance.Low,
                    "Should the CONECT be read?", "true"));
            useHetDictionary = IOSettings.Add(new BooleanIOSetting("UseHetDictionary", IOSetting.Importance.Low,
                    "Should the PDBReader use the HETATM dictionary for atom types?", "false"));
        }

        public void CustomizeJob()
        {
            foreach (var setting in IOSettings.Settings)
            {
                FireIOSettingQuestion(setting);
            }
        }
    }
}
