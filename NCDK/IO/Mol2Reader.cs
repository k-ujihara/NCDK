/* Copyright (C) 2003-2007  Egon Willighagen <egonw@users.sf.net>
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
 */
using NCDK.Common.Primitives;
using NCDK.Config;
using NCDK.IO.Formats;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using NCDK.Numerics;

namespace NCDK.IO
{
    /// <summary>
    /// Reads a molecule from an Mol2 file, such as written by Sybyl.
    /// See the specs <see href="http://www.tripos.com/data/support/mol2.pdf">here</see>.
    /// </summary>
    // @author Egon Willighagen
    // @cdk.module io
    // @cdk.githash
    // @cdk.iooptions
    // @cdk.created 2003-08-21
    // @cdk.keyword file format, Mol2
    public class Mol2Reader : DefaultChemObjectReader
    {
        bool firstLineisMolecule = false;

        TextReader input = null;

        /// <summary>
        /// Dictionary of known atom type aliases. If the key is seen on input, it
        /// is repleaced with the specified value. Bugs /openbabel/bug/214 and /cdk/bug/1346
        /// </summary>
        private static readonly IDictionary<string, string> ATOM_TYPE_ALIASES =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() {
                // previously produced by Open Babel
                { "S.o2", "S.O2" },
                { "S.o", "S.O" },
                // seen in MMFF94 validation suite
                { "CL", "Cl" }, { "CU", "Cu" },
                { "FE", "Fe" }, { "BR", "Br" },
                { "NA", "Na" }, { "SI", "Si" },
                { "CA", "Ca" }, { "ZN", "Zn" },
                { "LI", "Li" }, { "MG", "Mg" },
            });

        /// <summary>
        /// Constructs a new MDLReader that can read Molecule from a given Reader.
        /// </summary>
        /// <param name="ins">The Reader to read from</param>
        public Mol2Reader(TextReader ins)
        {
            input = ins;
        }

        public Mol2Reader(Stream input)
            : this(new StreamReader(input))
        { }

        public Mol2Reader()
            : this(new StringReader(""))
        { }

        public override IResourceFormat Format => Mol2Format.Instance;

        public override void SetReader(TextReader input)
        {
            this.input = input;
        }

        public override void SetReader(Stream input)
        {
            SetReader(new StreamReader(input));
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            if (typeof(IChemModel).IsAssignableFrom(type)) return true;
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            return false;
        }

        public override T Read<T>(T obj)
        {
            if (obj is IChemFile)
            {
                return (T)ReadChemFile((IChemFile)obj);
            }
            else if (obj is IChemModel)
            {
                return (T)ReadChemModel((IChemModel)obj);
            }
            else if (obj is IAtomContainer)
            {
                return (T)ReadMolecule((IAtomContainer)obj);
            }
            else
            {
                throw new CDKException("Only supported are ChemFile and Molecule.");
            }
        }

        private IChemModel ReadChemModel(IChemModel chemModel)
        {
            var setOfMolecules = chemModel.MoleculeSet;
            if (setOfMolecules == null)
            {
                setOfMolecules = chemModel.Builder.CreateAtomContainerSet();
            }
            IAtomContainer m = ReadMolecule(chemModel.Builder.CreateAtomContainer());
            if (m != null)
            {
                setOfMolecules.Add(m);
            }
            chemModel.MoleculeSet = setOfMolecules;
            return chemModel;
        }

        private IChemFile ReadChemFile(IChemFile chemFile)
        {
            IChemSequence chemSequence = chemFile.Builder.CreateChemSequence();

            IChemModel chemModel = chemFile.Builder.CreateChemModel();
            var setOfMolecules = chemFile.Builder.CreateAtomContainerSet();
            IAtomContainer m = ReadMolecule(chemFile.Builder.CreateAtomContainer());
            if (m != null) setOfMolecules.Add(m);
            chemModel.MoleculeSet = setOfMolecules;
            chemSequence.Add(chemModel);
            setOfMolecules = chemFile.Builder.CreateAtomContainerSet();
            chemModel = chemFile.Builder.CreateChemModel();
            try
            {
                firstLineisMolecule = true;
                while (m != null)
                {
                    m = ReadMolecule(chemFile.Builder.CreateAtomContainer());
                    if (m != null)
                    {
                        setOfMolecules.Add(m);
                        chemModel.MoleculeSet = setOfMolecules;
                        chemSequence.Add(chemModel);
                        setOfMolecules = chemFile.Builder.CreateAtomContainerSet();
                        chemModel = chemFile.Builder.CreateChemModel();
                    }
                }
            }
            catch (CDKException cdkexc)
            {
                throw cdkexc;
            }
            catch (ArgumentException exception)
            {
                string error = "Error while parsing MOL2";
                Trace.TraceError(error);
                Debug.WriteLine(exception);
                throw new CDKException(error, exception);
            }
            try
            {
                input.Close();
            }
            catch (Exception exc)
            {
                string error = "Error while closing file: " + exc.Message;
                Trace.TraceError(error);
                throw new CDKException(error, exc);
            }

            chemFile.Add(chemSequence);

            // reset it to false so that other read methods called later do not get confused
            firstLineisMolecule = false;

            return chemFile;
        }

        public bool Accepts(IChemObject obj)
        {
            if (obj is IChemFile) return true;
            if (obj is IChemModel) return true;
            if (obj is IAtomContainer) return true;
            return false;
        }

        /// <summary>
        /// Read a Reaction from a file in MDL RXN format
        /// </summary>
        /// <returns>The Reaction that was read from the MDL file.</returns>
        private IAtomContainer ReadMolecule(IAtomContainer molecule)
        {
            AtomTypeFactory atFactory = null;
            try
            {
                atFactory = AtomTypeFactory.GetInstance("NCDK.Config.Data.mol2_atomtypes.xml",
                        molecule.Builder);
            }
            catch (Exception exception)
            {
                string error = "Could not instantiate an AtomTypeFactory";
                Trace.TraceError(error);
                Debug.WriteLine(exception);
                throw new CDKException(error, exception);
            }
            try
            {
                int atomCount = 0;
                int bondCount = 0;

                string line;
                while (true)
                {
                    line = input.ReadLine();
                    if (line == null) return null;
                    if (line.StartsWith("@<TRIPOS>MOLECULE")) break;
                    if (!line.StartsWith("#") && line.Trim().Length > 0) break;
                }

                // ok, if we're coming from the chemfile functoion, we've alreay read the molecule RTI
                if (firstLineisMolecule)
                    molecule.SetProperty(CDKPropertyName.Title, line);
                else
                {
                    line = input.ReadLine();
                    molecule.SetProperty(CDKPropertyName.Title, line);
                }

                // get atom and bond counts
                string counts = input.ReadLine();
                var tokenizer = Strings.Tokenize(counts);
                try
                {
                    atomCount = int.Parse(tokenizer[0]);
                }
                catch (FormatException nfExc)
                {
                    string error = "Error while reading atom count from MOLECULE block";
                    Trace.TraceError(error);
                    Debug.WriteLine(nfExc);
                    throw new CDKException(error, nfExc);
                }
                if (tokenizer.Count > 1)
                {
                    try
                    {
                        bondCount = int.Parse(tokenizer[1]);
                    }
                    catch (FormatException nfExc)
                    {
                        string error = "Error while reading atom and bond counts";
                        Trace.TraceError(error);
                        Debug.WriteLine(nfExc);
                        throw new CDKException(error, nfExc);
                    }
                }
                else
                {
                    bondCount = 0;
                }
                Trace.TraceInformation("Reading #atoms: ", atomCount);
                Trace.TraceInformation("Reading #bonds: ", bondCount);

                // we skip mol type, charge type and status bit lines
                Trace.TraceWarning("Not reading molecule qualifiers");

                line = input.ReadLine();
                bool molend = false;
                while (line != null)
                {
                    if (line.StartsWith("@<TRIPOS>MOLECULE"))
                    {
                        molend = true;
                        break;
                    }
                    else if (line.StartsWith("@<TRIPOS>ATOM"))
                    {
                        Trace.TraceInformation("Reading atom block");
                        for (int i = 0; i < atomCount; i++)
                        {
                            line = input.ReadLine().Trim();
                            if (line.StartsWith("@<TRIPOS>MOLECULE"))
                            {
                                molend = true;
                                break;
                            }
                            tokenizer = Strings.Tokenize(line);
                            // disregard the id token
                            string nameStr = tokenizer[1];
                            string xStr = tokenizer[2];
                            string yStr = tokenizer[3];
                            string zStr = tokenizer[4];
                            string atomTypeStr = tokenizer[5];

                            // replace unrecognised atom type
                            if (ATOM_TYPE_ALIASES.ContainsKey(atomTypeStr))
                                atomTypeStr = ATOM_TYPE_ALIASES[atomTypeStr];

                            IAtom atom = molecule.Builder.CreateAtom("X");
                            IAtomType atomType;
                            try
                            {
                                atomType = atFactory.GetAtomType(atomTypeStr);
                            }
                            catch (Exception)
                            {
                                // ok, *not* an mol2 atom type
                                atomType = null;
                            }
                            // Maybe it is just an element
                            if (atomType == null && IsElementSymbol(atomTypeStr))
                            {
                                atom.Symbol = atomTypeStr;
                            }
                            else
                            {
                                if (atomType == null)
                                {
                                    atomType = atFactory.GetAtomType("X");
                                    Trace.TraceError("Could not find specified atom type: ", atomTypeStr);
                                }
                                AtomTypeManipulator.Configure(atom, atomType);
                            }

                            atom.AtomicNumber = Elements.OfString(atom.Symbol).AtomicNumber;
                            atom.Id = nameStr;
                            atom.AtomTypeName = atomTypeStr;
                            try
                            {
                                double x = double.Parse(xStr);
                                double y = double.Parse(yStr);
                                double z = double.Parse(zStr);
                                atom.Point3D = new Vector3(x, y, z);
                            }
                            catch (FormatException nfExc)
                            {
                                string error = "Error while reading atom coordinates";
                                Trace.TraceError(error);
                                Debug.WriteLine(nfExc);
                                throw new CDKException(error, nfExc);
                            }
                            molecule.Atoms.Add(atom);
                        }
                    }
                    else if (line.StartsWith("@<TRIPOS>BOND"))
                    {
                        Trace.TraceInformation("Reading bond block");
                        for (int i = 0; i < bondCount; i++)
                        {
                            line = input.ReadLine();
                            if (line.StartsWith("@<TRIPOS>MOLECULE"))
                            {
                                molend = true;
                                break;
                            }
                            tokenizer = Strings.Tokenize(line);
                            // disregard the id token
                            string atom1Str = tokenizer[1];
                            string atom2Str = tokenizer[2];
                            string orderStr = tokenizer[3];
                            try
                            {
                                int atom1 = int.Parse(atom1Str);
                                int atom2 = int.Parse(atom2Str);
                                if ("nc".Equals(orderStr))
                                {
                                    // do not connect the atoms
                                }
                                else
                                {
                                    IBond bond = molecule.Builder.CreateBond(
                                        molecule.Atoms[atom1 - 1], molecule.Atoms[atom2 - 1]);
                                    if ("1".Equals(orderStr))
                                    {
                                        bond.Order = BondOrder.Single;
                                    }
                                    else if ("2".Equals(orderStr))
                                    {
                                        bond.Order = BondOrder.Double;
                                    }
                                    else if ("3".Equals(orderStr))
                                    {
                                        bond.Order = BondOrder.Triple;
                                    }
                                    else if ("am".Equals(orderStr) || "ar".Equals(orderStr))
                                    {
                                        bond.Order = BondOrder.Single;
                                        bond.IsAromatic = true;
                                        bond.Atoms[0].IsAromatic = true;
                                        bond.Atoms[1].IsAromatic = true;
                                    }
                                    else if ("du".Equals(orderStr))
                                    {
                                        bond.Order = BondOrder.Single;
                                    }
                                    else if ("un".Equals(orderStr))
                                    {
                                        bond.Order = BondOrder.Single;
                                    }
                                    molecule.Bonds.Add(bond);
                                }
                            }
                            catch (FormatException nfExc)
                            {
                                string error = "Error while reading bond information";
                                Trace.TraceError(error);
                                Debug.WriteLine(nfExc);
                                throw new CDKException(error, nfExc);
                            }
                        }
                    }
                    if (molend) return molecule;
                    line = input.ReadLine();
                }
            }
            catch (IOException exception)
            {
                string error = "Error while reading general structure";
                Trace.TraceError(error);
                Debug.WriteLine(exception);
                throw new CDKException(error, exception);
            }
            return molecule;
        }

        private bool IsElementSymbol(string atomTypeStr)
        {
            for (int i = 1; i < PeriodicTable.ElementCount; i++)
            {
                if (PeriodicTable.GetSymbol(i).Equals(atomTypeStr)) return true;
            }
            return false;
        }

        public override void Close()
        {
            input.Close();
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
