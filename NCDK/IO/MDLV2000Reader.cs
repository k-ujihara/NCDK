/* Copyright (C) 1997-2007  Christoph Steinbeck <steinbeck@users.sourceforge.net>
 *                    2010  Egon Willighagen <egonw@users.sourceforge.net>
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Primitives;
using NCDK.Config;
using NCDK.IO.Formats;
using NCDK.IO.Setting;
using NCDK.Isomorphisms.Matchers;
using NCDK.SGroups;
using NCDK.Stereo;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NCDK.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using static NCDK.IO.MDLV2000Writer;

namespace NCDK.IO
{
    /// <summary>
    /// Reads content from MDL molfiles and SD files. It can read a <see cref="IAtomContainer"/>
    /// or <see cref="IChemModel"/> from an MDL molfile, and a <see cref="IChemFile"/>
    ///  from a SD file, with a <see cref="IChemSequence"/> of <see cref="IChemModel"/>'s, 
    ///  where each IChemModel will contain one <see cref="IAtomContainer"/>.
    /// <para>
    /// From the Atom block it reads atomic coordinates, element types and formal
    /// charges. From the Bond block it reads the bonds and the orders. Additionally,
    /// it reads 'M  CHG', 'G  ', 'M  RAD' and 'M  ISO' lines from the property
    /// block.
    /// </para>
    /// <para>
    /// If all z coordinates are 0.0, then the xy coordinates are taken as 2D,
    /// otherwise the coordinates are read as 3D.
    /// </para>
    /// <para>The title of the MOL file is read and can be retrieved with:
    /// <code>
    ///   molecule.GetProperty&lt;string&gt;(CDKPropertyName.TITLE);
    /// </code>
    /// </para>
    /// <para>RGroups which are saved in the MDL molfile as R#, are renamed according to
    /// their appearance, e.g. the first R# is named R1. With PseudAtom.Label
    /// "R1" is returned (instead of R#). This is introduced due to the SAR table
    /// generation procedure of Scitegics PipelinePilot.
    /// </para>
    /// </summary>
    // @author steinbeck
    // @author Egon Willighagen
    // @cdk.module io
    // @cdk.githash
    // @cdk.iooptions
    // @cdk.created 2000-10-02
    // @cdk.keyword file format, MDL molfile
    // @cdk.keyword file format, SDF
    // @cdk.bug 1587283
    public class MDLV2000Reader : DefaultChemObjectReader
    {
        TextReader input = null;

        private BooleanIOSetting forceReadAs3DCoords;
        private BooleanIOSetting interpretHydrogenIsotopes;
        private BooleanIOSetting addStereoElements;

        // Pattern to remove trailing space (string.Trim() will remove leading space, which we don't want)
        private static readonly Regex TRAILING_SPACE = new Regex("\\s+$", RegexOptions.Compiled);

        /// <summary>Delimits Structure-Data (SD) Files.</summary>
        private const string RECORD_DELIMITER = "$$$$";

        /// <summary>Valid pseudo labels.</summary>
        private static readonly ICollection<string> PSEUDO_LABELS =
                new ReadOnlyCollection<string>(new[] {
                    "*", "A", "Q",
                    "L", "LP", "R", // XXX: not in spec
                	"R#"});

        public MDLV2000Reader()
            : this(new StringReader(""))
        { }

        /// <summary>
        /// Constructs a new <see cref="MDLReader"/> that can read Molecule from a given <see cref="Stream"/>.
        /// </summary>
        /// <param name="input">The Stream to read from</param>
        public MDLV2000Reader(Stream input)
            : this(new StreamReader(input))
        { }

        public MDLV2000Reader(Stream input, ChemObjectReaderModes mode)
            : this(new StreamReader(input), mode)
        { }


        /// <summary>
        /// Constructs a new <see cref="MDLReader"/> that can read Molecule from a given <see cref="TextReader"/>.
        /// </summary>
        /// <param name="input">The Reader to read from</param>
        public MDLV2000Reader(TextReader input)
            : this(input, ChemObjectReaderModes.Relaxed)
        { }

        public MDLV2000Reader(TextReader input, ChemObjectReaderModes mode)
        {
            this.input = input;
            InitIOSettings();
            base.mode = mode;
        }

        public override IResourceFormat Format
        {
            get
            {
                return MDLV2000Format.Instance;
            }
        }

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
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            if (typeof(IChemModel).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Takes an object which subclasses IChemObject, e.g. Molecule, and will
        /// read this (from file, database, internet etc). If the specific
        /// implementation does not support a specific IChemObject it will throw an
        /// Exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object that subclasses IChemObject</param>
        /// <returns>The IChemObject read</returns>
        public override T Read<T>(T obj)
        {
            if (obj is IAtomContainer)
            {
                return (T)ReadAtomContainer((IAtomContainer)obj);
            }
            else if (obj is IChemFile)
            {
                return (T)ReadChemFile((IChemFile)obj);
            }
            else if (obj is IChemModel)
            {
                return (T)ReadChemModel((IChemModel)obj);
            }
            else
            {
                throw new CDKException("Only supported are ChemFile and Molecule.");
            }
        }

        private IChemModel ReadChemModel(IChemModel chemModel)
        {
            IAtomContainerSet<IAtomContainer> setOfMolecules = chemModel.MoleculeSet;
            if (setOfMolecules == null)
            {
                setOfMolecules = chemModel.Builder.CreateAtomContainerSet();
            }
            IAtomContainer m = ReadAtomContainer(chemModel.Builder.CreateAtomContainer());
            if (m != null)
            {
                setOfMolecules.Add(m);
            }
            chemModel.MoleculeSet = setOfMolecules;
            return chemModel;
        }

        /// <summary>
        /// Read a ChemFile from a file in MDL SDF format.
        /// </summary>
        /// <param name="chemFile"></param>
        /// <returns>The <see cref="IChemFile"/> that was read from the MDL file.</returns>
        private IChemFile ReadChemFile(IChemFile chemFile)
        {
            IChemObjectBuilder builder = chemFile.Builder;
            IChemSequence sequence = builder.CreateChemSequence();

            try
            {
                IAtomContainer m;
                while ((m = ReadAtomContainer(builder.CreateAtomContainer())) != null)
                {
                    sequence.Add(NewModel(m));
                }
            }
            catch (CDKException e)
            {
                throw e;
            }
            catch (ArgumentException exception)
            {
                string error = "Error while parsing SDF";
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

            chemFile.Add(sequence);
            return chemFile;
        }

        /// <summary>
        /// Create a new chem model for a single <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="container">the container to create the model for</param>
        /// <returns>a new <see cref="IChemModel"/></returns>
        private static IChemModel NewModel(IAtomContainer container)
        {
            if (container == null) throw new NullReferenceException("cannot create chem model for a null container");

            IChemObjectBuilder builder = container.Builder;
            IChemModel model = builder.CreateChemModel();
            IAtomContainerSet<IAtomContainer> containers = builder.CreateAtomContainerSet();

            containers.Add(container);
            model.MoleculeSet = containers;

            return model;
        }

        /// <summary>
        /// Read an IAtomContainer from a file in MDL sd format
        /// </summary>
        /// <param name="molecule"></param>
        /// <returns>The Molecule that was read from the MDL file.</returns>
        private IAtomContainer ReadAtomContainer(IAtomContainer molecule)
        {
            IAtomContainer outputContainer = null;
            IDictionary<IAtom, int> parities = new Dictionary<IAtom, int>();

            int linecount = 0;
            string title = null;
            string remark = null;
            string line = "";

            try
            {
                line = input.ReadLine();
                linecount++;
                if (line == null)
                {
                    return null;
                }

                if (line.StartsWith("$$$$"))
                {
                    return molecule;
                }
                if (line.Length > 0)
                {
                    title = line;
                }
                line = input.ReadLine();
                linecount++;
                line = input.ReadLine();
                linecount++;
                if (line.Length > 0)
                {
                    remark = line;
                }

                line = input.ReadLine();
                linecount++;

                // if the line is empty we hav a problem - either a malformed
                // molecule entry or just extra new lines at the end of the file
                if (line.Length == 0)
                {
                    HandleError("Unexpected empty line", linecount, 0, 0);
                    // read till the next $$$$ or EOF
                    while (true)
                    {
                        line = input.ReadLine();
                        linecount++;
                        if (line == null)
                        {
                            return null;
                        }
                        if (line.StartsWith("$$$$"))
                        {
                            return molecule; // an empty molecule
                        }
                    }
                }

                CTabVersion version = CTabVersion.OfHeader(line);

                // check the CT block version
                if (version == CTabVersion.V3000)
                {
                    HandleError("This file must be read with the MDLV3000Reader.");
                    // even if relaxed we can't read V3000 using the V2000 parser
                    throw new CDKException("This file must be read with the MDLV3000Reader.");
                }
                else if (version == CTabVersion.Unspecified)
                {
                    HandleError("This file must be read with the MDLReader.");
                    // okay to read in relaxed mode
                }

                int nAtoms = ReadMolfileInt(line, 0);
                int nBonds = ReadMolfileInt(line, 3);

                IAtom[] atoms = new IAtom[nAtoms];
                IBond[] bonds = new IBond[nBonds];

                // used for applying the MDL valence model
                int[] explicitValence = new int[nAtoms];

                bool hasX = false, hasY = false, hasZ = false;

                for (int i = 0; i < nAtoms; i++)
                {
                    line = input.ReadLine();
                    linecount++;

                    IAtom atom = ReadAtomFast(line, molecule.Builder, parities, linecount);

                    atoms[i] = atom;

                    Vector3 p = atom.Point3D.Value;
                    hasX = hasX || p.X != 0d;
                    hasY = hasY || p.Y != 0d;
                    hasZ = hasZ || p.Z != 0d;
                }

                // convert to 2D, if totalZ == 0
                if (!hasX && !hasY && !hasZ)
                {
                    if (nAtoms == 1)
                    {
                        atoms[0].Point2D = Vector2.Zero;
                    }
                    else
                    {
                        foreach (var atomToUpdate in atoms)
                        {
                            atomToUpdate.Point3D = null;
                        }
                    }
                }
                else if (!hasZ)
                {

                    if (!forceReadAs3DCoords.IsSet)
                    {
                        foreach (var atomToUpdate in atoms)
                        {
                            Vector3 p3d = atomToUpdate.Point3D.Value;
                            if (p3d != null)
                            {
                                atomToUpdate.Point2D = new Vector2(p3d.X, p3d.Y);
                                atomToUpdate.Point3D = null;
                            }
                        }
                    }
                }

                bool hasQueryBonds = false;
                for (int i = 0; i < nBonds; i++)
                {
                    line = input.ReadLine();
                    linecount++;

                    bonds[i] = ReadBondFast(line, molecule.Builder, atoms, explicitValence, linecount);
                    hasQueryBonds = hasQueryBonds
                            || (bonds[i].Order == BondOrder.Unset && !bonds[i].IsAromatic);
                }

                if (!hasQueryBonds)
                    outputContainer = molecule;
                else
                    outputContainer = new QueryAtomContainer(molecule.Builder);

                outputContainer.SetProperty(CDKPropertyName.TITLE, title);
                outputContainer.SetProperty(CDKPropertyName.REMARK, remark);

                if (outputContainer.IsEmpty)
                {
                    outputContainer.SetAtoms(atoms);
                    outputContainer.SetBonds(bonds);
                }
                else
                {
                    foreach (var atom in atoms)
                        outputContainer.Atoms.Add(atom);
                    foreach (var bond in bonds)
                        outputContainer.Bonds.Add(bond);
                }

                // create 0D stereochemistry

                foreach (var e in parities)
                {
                    int parity = e.Value;
                    if (parity != 1 && parity != 2)
                        continue; // 3=unspec
                    int idx = 0;
                    IAtom focus = e.Key;
                    IAtom[] carriers = new IAtom[4];
                    int hidx = -1;
                    foreach (var nbr in molecule.GetConnectedAtoms(focus))
                    {
                        if (idx == 4)
                            goto Next_Parities; // too many neighbors
                        if (nbr.AtomicNumber == 1)
                        {
                            if (hidx >= 0)
                                goto Next_Parities;
                            hidx = idx;
                        }
                        carriers[idx++] = nbr;
                    }
                    // to few neighbors, or already have a hydrogen defined
                    if (idx < 3 || idx < 4 && hidx >= 0)
                        continue;
                    if (idx == 3)
                        carriers[idx++] = focus;

                    if (idx == 4)
                    {
                        TetrahedralStereo winding = parity == 1 ? TetrahedralStereo.Clockwise : TetrahedralStereo.AntiClockwise;
                        // H is always at back, even if explicit! At least this seems to be the case.
                        // we adjust the winding as needed
                        if (hidx == 0 || hidx == 2)
                            winding = winding.Invert();
                        molecule.AddStereoElement(new TetrahedralChirality(focus, carriers, winding));
                    }
                    Next_Parities:
                    ;

                }

                // read PROPERTY block
                ReadPropertiesFast(input, outputContainer, nAtoms);

                // read potential SD file data between M  END and $$$$
                ReadNonStructuralData(input, outputContainer);

                if (interpretHydrogenIsotopes.IsSet)
                {
                    FixHydrogenIsotopes(molecule, Isotopes.Instance);
                }

                // note: apply the valence model last so that all fixes (i.e. hydrogen
                // isotopes) are in place we need to use a offset as this atoms
                // could be added to a molecule which already had atoms present
                int offset = outputContainer.Atoms.Count - nAtoms;
                for (int i = offset; i < outputContainer.Atoms.Count; i++)
                {
                    int valence = explicitValence[i - offset];
                    if (valence < 0)
                    {
                        hasQueryBonds = true; // also counts aromatic bond as query
                    }
                    else
                    {
                        int unpaired = outputContainer.GetConnectedSingleElectrons(outputContainer.Atoms[i]).Count();
                        ApplyMDLValenceModel(outputContainer.Atoms[i], valence + unpaired);
                    }
                }

                // sanity check that we have a decent molecule, query bonds mean we
                // don't have a hydrogen count for atoms and stereo perception isn't
                // currently possible
                if (!hasQueryBonds && addStereoElements.IsSet && hasX && hasY)
                {
                    if (hasZ)
                    { // has 3D coordinates
                        outputContainer.SetStereoElements(StereoElementFactory.Using3DCoordinates(outputContainer)
                                .CreateAll());
                    }
                    else if (!forceReadAs3DCoords.IsSet)
                    { // has 2D coordinates (set as 2D coordinates)
                        outputContainer.SetStereoElements(StereoElementFactory.Using2DCoordinates(outputContainer)
                                .CreateAll());
                    }
                }

            }
            catch (CDKException exception)
            {
                string error = "Error while parsing line " + linecount + ": " + line + " -> " + exception.Message;
                Trace.TraceError(error);
                throw exception;
            }
            catch (IOException exception)
            {
                Console.Error.WriteLine(exception.StackTrace);
                string error = "Error while parsing line " + linecount + ": " + line + " -> " + exception.Message;
                Trace.TraceError(error);
                HandleError("Error while parsing line: " + line, linecount, 0, 0, exception);
            }

            return outputContainer;
        }

        /// <summary>
        /// Applies the MDL valence model to atoms using the explicit valence (bond
        /// order sum) and charge to determine the correct number of implicit
        /// hydrogens. The model is not applied if the explicit valence is less than
        /// 0 - this is the case when a query bond was read for an atom.
        /// </summary>
        /// <param name="atom">the atom to apply the model to</param>
        /// <param name="explicitValence">the explicit valence (bond order sum)</param>
        private void ApplyMDLValenceModel(IAtom atom, int explicitValence)
        {

            if (atom.Valency != null)
            {
                if (atom.Valency >= explicitValence)
                    atom.ImplicitHydrogenCount = atom.Valency - explicitValence;
                else
                    atom.ImplicitHydrogenCount = 0;
            }
            else
            {
                int element = atom.AtomicNumber ?? 0;

                int charge = atom.FormalCharge ?? 0;

                int implicitValence = MDLValence.ImplicitValence(element, charge, explicitValence);
                if (implicitValence < explicitValence)
                {
                    atom.Valency = explicitValence;
                    atom.ImplicitHydrogenCount = 0;
                }
                else
                {
                    atom.Valency = implicitValence;
                    atom.ImplicitHydrogenCount = implicitValence - explicitValence;
                }
            }
        }

        private void FixHydrogenIsotopes(IAtomContainer molecule, IsotopeFactory isotopeFactory)
        {
            foreach (var atom in AtomContainerManipulator.GetAtomArray(molecule))
            {
                if (atom is IPseudoAtom)
                {
                    IPseudoAtom pseudo = (IPseudoAtom)atom;
                    if ("D".Equals(pseudo.Label))
                    {
                        IAtom newAtom = molecule.Builder.CreateAtom(atom);
                        newAtom.Symbol = "H";
                        newAtom.AtomicNumber = 1;
                        isotopeFactory.Configure(newAtom, isotopeFactory.GetIsotope("H", 2));
                        AtomContainerManipulator.RePlaceAtomByAtom(molecule, atom, newAtom);
                    }
                    else if ("T".Equals(pseudo.Label))
                    {
                        IAtom newAtom = molecule.Builder.CreateAtom(atom);
                        newAtom.Symbol = "H";
                        newAtom.AtomicNumber = 1;
                        isotopeFactory.Configure(newAtom, isotopeFactory.GetIsotope("H", 3));
                        AtomContainerManipulator.RePlaceAtomByAtom(molecule, atom, newAtom);
                    }
                }
            }
        }

        public override void Dispose()
        {
            Close();
        }

        public override void Close()
        {
            input.Close();
        }

        private void InitIOSettings()
        {
            forceReadAs3DCoords = IOSettings.Add(new BooleanIOSetting("ForceReadAs3DCoordinates", IOSetting.Importance.Low,
                    "Should coordinates always be read as 3D?", "false"));
            interpretHydrogenIsotopes = IOSettings.Add((new BooleanIOSetting("InterpretHydrogenIsotopes",
                    IOSetting.Importance.Low, "Should D and T be interpreted as hydrogen isotopes?", "true")));
            addStereoElements = IOSettings.Add((new BooleanIOSetting("AddStereoElements", IOSetting.Importance.Low,
                    "Assign stereo configurations to stereocenters utilising 2D/3D coordinates.", "true")));
        }

        public void CustomizeJob()
        {
            foreach (var setting in IOSettings.Settings)
            {
                FireIOSettingQuestion(setting);
            }
        }

        private string RemoveNonDigits(string input)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char inputChar = input[i];
                if (char.IsDigit(inputChar)) sb.Append(inputChar);
            }
            return sb.ToString();
        }

        private static readonly IDictionary<IAtom, int> EmptyMap = new ReadOnlyDictionary<IAtom, int>(new Dictionary<IAtom, int>());

#if TEST
        public
#endif
            IAtom ReadAtomFast(string line, IChemObjectBuilder builder, int lineNum)
        {
            return ReadAtomFast(line, builder, EmptyMap, lineNum);
        }

        /// <summary>
        /// Parse an atom line from the atom block using the format: {@code
        /// xxxxx.xxxxyyyyy.yyyyzzzzz.zzzz aaaddcccssshhhbbbvvvHHHrrriiimmmnnneee}
        /// where: <ul> <li>x: x coordinate</li> <li>y: y coordinate</li> <li>z: z
        /// coordinate</li> <li>a: atom symbol</li> <li>d: mass difference</li>
        /// <li>c: charge</li> <li>s: stereo parity</li> <li>h: hydrogen count + 1
        /// (not read - query)</li> <li>b: stereo care (not read - query)</li> <li>v:
        /// valence</li> <li>H: H0 designator (not read - query)</li> <li>r: not
        /// used</li> <li>i: not used</li> <li>m: atom reaction mapping</li> <li>n:
        /// inversion/retention flag</li> <li>e: exact change flag</li> </ul>
        /// 
        /// The parsing is strict and does not allow extra columns (i.e. NMR shifts)
        /// malformed input.
        /// </summary>
        /// <param name="line">input line</param>
        /// <param name="builder">chem object builder to create the atom</param>
        /// <param name="parities">map of atom parities for creation 0D stereochemistry</param>
        /// <param name="lineNum">the line number - for printing error messages</param>
        /// <returns>a new atom instance</returns>
#if TEST
        public
#endif
        IAtom ReadAtomFast(string line, IChemObjectBuilder builder, IDictionary<IAtom, int> parities, int lineNum)
        {
            // The line may be truncated and it's checked in reverse at the specified
            // lengths:
            //          1         2         3         4         5         6
            // 123456789012345678901234567890123456789012345678901234567890123456789
            //                                  | |  |  |  |  |  |  |  |  |  |  |  |
            // xxxxx.xxxxyyyyy.yyyyzzzzz.zzzz aaaddcccssshhhbbbvvvHHHrrriiimmmnnneee

            string symbol;
            double x = 0, y = 0, z = 0;
            int massDiff = 0, charge = 0, parity = 0, valence = 0, mapping = 0;

            int length = GetLength(line);
            if (length > 69) // excess data we should check all fields
                length = 69;

            // given the length we jump to the position and parse all fields
            // that could be present (note - fall through switch)
            switch (length)
            {
                case 69: // eee: exact charge flag [reaction, query]
                case 66: // nnn: inversion / retention [reaction]
                case 63: // mmm: atom-atom mapping [reaction]
                    mapping = ReadMolfileInt(line, 60);
                    goto case 60;
                case 60: // iii: not used
                case 57: // rrr: not used
                case 54: // HHH: H0 designation [redundant]
                case 51: // vvv: valence
                    valence = ReadMolfileInt(line, 48);
                    goto case 48;
                case 48: // bbb: stereo care [query]
                case 45: // hhh: hydrogen count + 1 [query]
                case 42: // sss: stereo parity
                    parity = ToInt(line[41]);
                    goto case 39;
                case 39: // ccc: charge
                    charge = ToCharge(line[38]);
                    goto case 36;
                case 36: // dd: mass difference
                    massDiff = Sign(line[34]) * ToInt(line[35]);
                    goto case 34;
                case 34: // x y z and aaa: atom coordinates and symbol
                case 33: // symbol is left aligned
                case 32:
                    x = ReadMDLCoordinate(line, 0);
                    y = ReadMDLCoordinate(line, 10);
                    z = ReadMDLCoordinate(line, 20);
                    symbol = string.Intern(Strings.Substring(line, 31, 3).Trim());
                    break;
                default:
                    HandleError("invalid line length", lineNum, 0, 0);
                    throw new CDKException("invalid line length, " + length + ": " + line);
            }

            IAtom atom = CreateAtom(symbol, builder, lineNum);

            atom.Point3D = new Vector3(x, y, z);
            atom.FormalCharge = charge;
            atom.StereoParity = parity;
            if (parity != 0)
                parities[atom] = parity;

            // if there was a mass difference, set the mass number
            if (massDiff != 0 && atom.AtomicNumber > 0)
                atom.MassNumber = Isotopes.Instance.GetMajorIsotope(atom.AtomicNumber.Value).MassNumber
                        + massDiff;

            if (valence > 0 && valence < 16) atom.Valency = valence == 15 ? 0 : valence;

            if (mapping != 0) atom.SetProperty(CDKPropertyName.ATOM_ATOM_MAPPING, mapping);

            return atom;
        }

        /// <summary>
        /// Read a bond from a line in the MDL bond block. The bond block is
        /// formatted as follows, {@code 111222tttsssxxxrrrccc}, where:
        /// <ul>
        ///     <li>111: first atom number</li>
        ///     <li>222: second atom number</li>
        ///     <li>ttt: bond type</li>
        ///     <li>xxx: bond stereo</li>
        ///     <li>rrr: bond topology</li>
        ///     <li>ccc: reaction center</li>
        /// </ul>
        /// </summary>
        /// <param name="line">the input line</param>
        /// <param name="builder">builder to create objects with</param>
        /// <param name="atoms">atoms read from the atom block</param>
        /// <param name="explicitValence">array to fill with explicit valence</param>
        /// <param name="lineNum">the input line number</param>
        /// <returns>a new bond</returns>
        /// <exception cref="CDKException">if the input was malformed or didn't make sense</exception>
#if TEST
        public
#endif
        IBond ReadBondFast(string line, IChemObjectBuilder builder, IAtom[] atoms, int[] explicitValence, int lineNum)
        {
            // The line may be truncated and it's checked in reverse at the specified
            // lengths. Absolutely required is atom indices, bond type and stereo.
            //          1         2
            // 123456789012345678901
            //            |  |  |  |
            // 111222tttsssxxxrrrccc

            int length = GetLength(line);
            if (length > 21) length = 21;

            int u, v, type, stereo = 0;

            switch (length)
            {
                case 21: // ccc: reaction centre status
                case 18: // rrr: bond topology
                case 15: // xxx: not used
                case 12: // sss: stereo
                    stereo = ReadUInt(line, 9, 3);
                    goto case 9;
                case 9: // 111222ttt: atoms, type and stereo
                    u = ReadMolfileInt(line, 0) - 1;
                    v = ReadMolfileInt(line, 3) - 1;
                    type = ReadMolfileInt(line, 6);
                    break;
                default:
                    throw new CDKException("invalid line length: " + length + " " + line);
            }

            IBond bond = builder.CreateBond(atoms[u], atoms[v]);

            switch (type)
            {
                case 1: // single
                    bond.Order = BondOrder.Single;
                    bond.Stereo = ToStereo(stereo, type);
                    break;
                case 2: // double
                    bond.Order = BondOrder.Double;
                    bond.Stereo = ToStereo(stereo, type);
                    break;
                case 3: // triple
                    bond.Order = BondOrder.Triple;
                    break;
                case 4: // aromatic
                    bond.Order = BondOrder.Unset;
                    bond.IsAromatic = true;
                    bond.IsSingleOrDouble = true;
                    atoms[u].IsAromatic = true;
                    atoms[v].IsAromatic = true;
                    break;
                case 5: // single or double
                case 6: // single or aromatic
                case 7: // double or aromatic
                case 8: // any
                    bond = CTFileQueryBond.OfType(bond, type);
                    break;
                default:
                    throw new CDKException("unrecognised bond type: " + type + ", " + line);
            }

            if (type < 4)
            {
                explicitValence[u] += type;
                explicitValence[v] += type;
            }
            else
            {
                explicitValence[u] = explicitValence[v] = int.MinValue;
            }

            return bond;
        }

        /// <summary>
        /// Reads the property block from the {@code input} setting the values in the
        /// container.
        /// </summary>
        /// <param name="input">input resource</param>
        /// <param name="container">the structure with atoms / bonds present</param>
        /// <param name="nAtoms">the number of atoms in the atoms block</param>
        /// <exception cref="">low-level IO error</exception>
#if TEST
        public
#endif
            void ReadPropertiesFast(TextReader input, IAtomContainer container, int nAtoms)
        {
            string line;

            // first atom index in this Molfile, the container may have
            // already had atoms present before reading the file
            int offset = container.Atoms.Count - nAtoms;

            IDictionary<int, Sgroup> sgroups = new SortedDictionary<int, Sgroup>();

            while ((line = input.ReadLine()) != null)
            {
                int index, count, lnOffset;
                Sgroup sgroup;
                int length = line.Length;
                PropertyKey key = PropertyKey.Of(line);

                if (key == PropertyKey.ATOM_ALIAS)
                {
                    // A  aaa
                    // x...
                    //
                    // atom alias is stored as label on a pseudo atom
                    index = ReadMolfileInt(line, 3) - 1;
                    string label = input.ReadLine();
                    if (label == null) return;
                    Label(container, offset + index, label);
                }
                else if (key == PropertyKey.ATOM_VALUE)
                {
                    // V  aaa v...
                    //
                    // an atom value is stored as comment on an atom
                    index = ReadMolfileInt(line, 3) - 1;
                    string comment = Strings.Substring(line, 7);
                    container.Atoms[offset + index].SetProperty(CDKPropertyName.COMMENT, comment);
                }
                else if (key == PropertyKey.GROUP_ABBREVIATION)
                {
                    // G  aaappp
                    // x...
                    //
                    // Abbreviation is required for compatibility with previous versions of MDL ISIS/Desktop which
                    // allowed abbreviations with only one attachment. The attachment is denoted by two atom
                    // numbers, aaa and ppp. All of the atoms on the aaa side of the bond formed by aaa-ppp are
                    // abbreviated. The coordinates of the abbreviation are the coordinates of aaa. The text of the
                    // abbreviation is on the following line (x...). In current versions of ISIS, abbreviations can have any
                    // number of attachments and are written out using the Sgroup appendixes. However, any ISIS
                    // abbreviations that do have one attachment are also written out in the old style, again for
                    // compatibility with older ISIS versions, but this behavior might not be supported in future
                    // versions.
                    // not supported, existing parsing doesn't do what is
                    // mentioned in the specification above
                    // final int    from  = ReadMolfileInt(line, 3) - 1;
                    // final int    to    = ReadMolfileInt(line, 6) - 1;
                    string group = input.ReadLine();
                    if (group == null) return;
                }
                else if (key == PropertyKey.M_CHG)
                {
                    // M  CHGnn8 aaa vvv ...
                    //
                    // vvv: -15 to +15. Default of 0 = uncharged atom. When present, this property supersedes
                    //      all charge and radical values in the atom block, forcing a 0 charge on all atoms not
                    //      listed in an M CHG or M RAD line.
                    count = ReadUInt(line, 6, 3);
                    for (int i = 0, st = 10; i < count && st + 7 <= length; i++, st += 8) //
                    {
                        index = ReadMolfileInt(line, st) - 1;
                        int charge = ReadMolfileInt(line, st + 4);
                        container.Atoms[offset + index].FormalCharge = charge;
                    }
                }
                else if (key == PropertyKey.M_ISO)
                {
                    // M  ISOnn8 aaa vvv ...
                    //
                    // vvv: Absolute mass of the atom isotope as a positive integer. When present, this property
                    //      supersedes all isotope values in the atom block. Default (no entry) means natural
                    //      abundance. The difference between this absolute mass value and the natural
                    //      abundance value specified in the PTABLE.DAT file must be within the range of -18
                    //      to +12.
                    count = ReadUInt(line, 6, 3);
                    for (int i = 0, st = 10; i < count && st + 7 <= length; i++, st += 8)
                    {
                        index = ReadMolfileInt(line, st) - 1;
                        int mass = ReadMolfileInt(line, st + 4);
                        container.Atoms[offset + index].MassNumber = mass;
                    }
                }
                else if (key == PropertyKey.M_RAD)
                {
                    // M  RADnn8 aaa vvv ...
                    //
                    // vvv: Default of 0 = no radical, 1 = singlet (:), 2 = doublet ( . or ^), 3 = triplet (^^). When
                    //      present, this property supersedes all charge and radical values in the atom block,
                    //      forcing a 0 (zero) charge and radical on all atoms not listed in an M CHG or
                    //      M RAD line.
                    count = ReadUInt(line, 6, 3);
                    for (int i = 0, st = 10; i < count && st + 7 <= length; i++, st += 8)
                    {
                        index = ReadMolfileInt(line, st) - 1;
                        int value = ReadMolfileInt(line, st + 4);
                        SpinMultiplicity multiplicity = SpinMultiplicity.OfValue(value);

                        for (int e = 0; e < multiplicity.SingleElectrons; e++)
                            container.AddSingleElectron(container.Atoms[offset + index]);
                    }
                }
                else if (key == PropertyKey.M_RGP)
                {
                    // M  RGPnn8 aaa rrr ...
                    //
                    // rrr: Rgroup number, value from 1 to 32 *, labels position of Rgroup on root.
                    //
                    // see also, RGroupQueryReader
                    count = ReadUInt(line, 6, 3);
                    for (int i = 0, st = 10; i < count && st + 7 <= length; i++, st += 8)
                    {
                        index = ReadMolfileInt(line, st) - 1;
                        int number = ReadMolfileInt(line, st + 4);
                        Label(container, offset + index, "R" + number);
                    }
                }
                else if (key == PropertyKey.M_ZZC)
                {
                    // M  ZZC aaa c...
                    // 
                    // c: first character of the label, : to EOL.
                    //
                    // Proprietary atom labels created by ACD/Labs ChemSketch using the Manual Numbering Tool.
                    // This atom property appears to be undocumented, but experimentation leads to the following
                    // specification (tested with ACD/ChemSketch version 12.00 Build 29305, 25 Nov 2008)
                    //
                    // It's not necessary to label any/all atoms but if a label is present, the following applies:
                    //
                    // The atom Label(s) consist of an optional prefix, a required numeric label, and optional suffix.
                    //                         
                    // The numeric label is an integer in the range 0 - 999 inclusive.
                    // 
                    // If present, the prefix and suffix can each contain 1 - 50 characters, from the set of printable 
                    // ASCII characters shown here
                    //                            
                    //    !"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~"
                    //                    
                    // In addition, both the prefix and suffix may contain leading and/or trailing and/or embedded 
                    // whitespace, included within the limit of 50 characters. These should be preserved when read.
                    //                    
                    // Long labels in the mol/sdfile are not truncated or wrapped onto multiple lines. As a result, the
                    // line could be 114 characters in length (excluding the newline).
                    //
                    // By stopping and restarting the Manual Numbering Tool, it's possible to create non-sequential
                    // or even duplicate numbers or labels. This is reasonable for the intended purpose of the tool - 
                    // labelling the structure as you wish. If unique labels are required, downstream processing will be
                    // necessary to enforce this.
                    //
                    if (mode == ChemObjectReaderModes.Strict)
                    {
                        throw new CDKException("Atom property ZZC is illegal in Strict mode");
                    }
                    index = ReadMolfileInt(line, 7) - 1;
                    string atomLabel = Strings.Substring(line, 11);  // DO NOT TRIM
                    container.Atoms[offset + index].SetProperty(CDKPropertyName.ACDLABS_LABEL, atomLabel);
                }
                else if (key == PropertyKey.M_STY)
                {
                    // M STYnn8 sss ttt ...
                    //  sss: Sgroup number
                    //  ttt: Sgroup type: SUP = abbreviation Sgroup (formerly called superatom), MUL = multiple group,
                    //                    SRU = SRU type, MON = monomer, MER = Mer type, COP = copolymer, CRO = crosslink,
                    //                    MOD = modification, GRA = graft, COM = component, MIX = mixture,
                    //                    FOR = formulation, DAT = data Sgroup, Any = any polymer, GEN = generic.
                    //
                    // Note: For a given Sgroup, an STY line giving its type must appear before any other line that
                    //       supplies information about it. For a data Sgroup, an SDT line must describe the data
                    //       field before the SCD and SED lines that contain the data (see Data Sgroup Data below).
                    //       When a data Sgroup is linked to another Sgroup, the Sgroup must already have been defined.
                    //
                    // Sgroups can be in any order on the Sgroup Type line. Brackets are drawn around Sgroups with the
                    // M SDI lines defining the coordinates.
                    count = ReadMolfileInt(line, 6);
                    for (int i = 0; i < count; i++)
                    {
                        lnOffset = 10 + (i * 8);
                        index = ReadMolfileInt(line, lnOffset);

                        if (mode == ChemObjectReaderModes.Strict && sgroups.ContainsKey(index))
                            HandleError("STY line must appear before any other line that supplies Sgroup information");

                        sgroup = new Sgroup();
                        sgroups[index] = sgroup;

                        SgroupType type = SgroupType.ParseCtabKey(Strings.Substring(line, lnOffset + 4, 3));
                        if (type != null)
                            sgroup.Type = type;
                    }
                }
                else if (key == PropertyKey.M_SST)
                {
                    // Sgroup Subtype [Sgroup]
                    // M  SSTnn8 sss ttt ...
                    // ttt: Polymer Sgroup subtypes: ALT = alternating, RAN = random, BLO = block
                    count = ReadMolfileInt(line, 6);
                    for (int i = 0, st = 10; i < count && st + 7 <= length; i++, st += 8)
                    {
                        sgroup = EnsureSgroup(sgroups,
                                              ReadMolfileInt(line, st));
                        if (mode == ChemObjectReaderModes.Strict && sgroup.Type != SgroupType.CtabCopolymer)
                            HandleError("SST (Sgroup Subtype) specified for a non co-polymer group");

                        string sst = Strings.Substring(line, st + 4, 3);

                        if (mode == ChemObjectReaderModes.Strict && !("ALT".Equals(sst) || "RAN".Equals(sst) || "BLO".Equals(sst)))
                            HandleError("Invalid sgroup subtype: " + sst + " expected (ALT, RAN, or BLO)");

                        sgroup.PutValue(SgroupKey.CtabSubType, sst);
                    }
                }
                else if (key == PropertyKey.M_SAL)
                {
                    // Sgroup Atom List [Sgroup]
                    // M   SAL sssn15 aaa ...
                    // aaa: Atoms in Sgroup sss
                    sgroup = EnsureSgroup(sgroups, ReadMolfileInt(line, 7));
                    count = ReadMolfileInt(line, 10);
                    for (int i = 0, st = 14; i < count && st + 3 <= length; i++, st += 4)
                    {
                        index = ReadMolfileInt(line, st) - 1;
                        sgroup.Add(container.Atoms[offset + index]);
                    }
                }
                else if (key == PropertyKey.M_SBL)
                {
                    // Sgroup Bond List [Sgroup]
                    // M  SBL sssn15 bbb ...
                    // bbb: Bonds in Sgroup sss.
                    // (For data Sgroups, bbb’s are the containment bonds, for all other
                    //  Sgroup types, bbb’s are crossing bonds.)
                    sgroup = EnsureSgroup(sgroups, ReadMolfileInt(line, 7));
                    count = ReadMolfileInt(line, 10);
                    for (int i = 0, st = 14; i < count && st + 3 <= length; i++, st += 4)
                    {
                        index = ReadMolfileInt(line, st) - 1;
                        sgroup.Add(container.Bonds[offset + index]);
                    }
                }
                else if (key == PropertyKey.M_SPL)
                {
                    // Sgroup Hierarchy Information [Sgroup]
                    // M  SPLnn8 ccc ppp ...
                    //   ccc: Sgroup index of the child Sgroup
                    //   ppp: Sgroup index of the parent Sgroup (ccc and ppp must already be defined via an
                    //        STY line prior to encountering this line)
                    count = ReadMolfileInt(line, 6);
                    for (int i = 0, st = 10; i < count && st + 6 <= length; i++, st += 8)
                    {
                        sgroup = EnsureSgroup(sgroups, ReadMolfileInt(line, st));
                        sgroup.AddParent(EnsureSgroup(sgroups, ReadMolfileInt(line, st + 4)));
                    }
                }
                else if (key == PropertyKey.M_SCN)
                {
                    // Sgroup Connectivity [Sgroup]
                    // M  SCNnn8 sss ttt ...
                    // ttt: HH = head-to-head, HT = head-to-tail, EU = either unknown.
                    // Left justified.
                    count = ReadMolfileInt(line, 6);
                    for (int i = 0, st = 10; i < count && st + 6 <= length; i++, st += 8)
                    {
                        sgroup = EnsureSgroup(sgroups,
                                              ReadMolfileInt(line, st));
                        string con = Strings.Substring(line, st + 4, 3).Trim();
                        if (mode == ChemObjectReaderModes.Strict && !("HH".Equals(con) || "HT".Equals(con) || "EU".Equals(con)))
                            HandleError("Unknown SCN type (expected: HH, HT, or EU) was " + con);
                        sgroup.PutValue(SgroupKey.CtabConnectivity,
                                        con);
                    }
                }
                else if (key == PropertyKey.M_SDI)
                {
                    // Sgroup Display Information
                    // M SDI sssnn4 x1 y1 x2 y2
                    // x1,y1, Coordinates of bracket endpoints
                    // x2,y2:
                    sgroup = EnsureSgroup(sgroups, ReadMolfileInt(line, 7));
                    count = ReadMolfileInt(line, 10);
                    Trace.Assert(count == 4); // fixed?
                    sgroup.AddBracket(new SgroupBracket(ReadMDLCoordinate(line, 13),
                                                        ReadMDLCoordinate(line, 23),
                                                        ReadMDLCoordinate(line, 33),
                                                        ReadMDLCoordinate(line, 43)));
                }
                else if (key == PropertyKey.M_SMT)
                {
                    // Sgroup subscript
                    // M SMT sss m...
                    // m...: Text of subscript Sgroup sss.
                    // (For multiple groups, m... is the text representation of the multiple group multiplier.
                    //  For abbreviation Sgroups, m... is the text of the abbreviation Sgroup label.)
                    sgroup = EnsureSgroup(sgroups, ReadMolfileInt(line, 7));
                    sgroup.PutValue(SgroupKey.CtabSubScript,
                                    Strings.Substring(line, 11).Trim());
                }
                else if (key == PropertyKey.M_SBT)
                {
                    // Sgroup Bracket Style
                    // The format for the Sgroup bracket style is as follows:
                    // M  SBTnn8 sss ttt ...
                    // where:
                    //   sss: Index of Sgroup
                    //   ttt: Bracket display style: 0 = default, 1 = curved (parenthetic) brackets
                    // This appendix supports altering the display style of the Sgroup brackets.
                    count = ReadMolfileInt(line, 6);
                    for (int i = 0, st = 10; i < count && st + 7 <= length; i++, st += 8)
                    {
                        sgroup = EnsureSgroup(sgroups,
                                              ReadMolfileInt(line, st));
                        sgroup.PutValue(SgroupKey.CtabBracketStyle,
                                        ReadMolfileInt(line, st + 4));
                    }
                }
                else if (key == PropertyKey.M_SDS)
                {
                    // Sgroup Expansion
                    // M  SDS EXPn15 sss ...
                    // sss: Sgroup index of expanded abbreviation Sgroups
                    if ("EXP".Equals(Strings.Substring(line, 7, 3)))
                    {
                        count = ReadMolfileInt(line, 10);
                        for (int i = 0, st = 14; i < count && st + 3 <= length; i++, st += 4)
                        {
                            sgroup = EnsureSgroup(sgroups, ReadMolfileInt(line, st));
                            sgroup.PutValue(SgroupKey.CtabExpansion, true);
                        }
                    }
                    else if (mode == ChemObjectReaderModes.Strict)
                    {
                        HandleError("Expected EXP to follow SDS tag");
                    }
                }
                else if (key == PropertyKey.M_SPA)
                {
                    // Multiple Group Parent Atom List [Sgroup]
                    // M SPA sssn15 aaa ...
                    // aaa: Atoms in paradigmatic repeating unit of multiple group sss
                    // Note: To ensure that all current molfile readers consistently
                    //       interpret chemical structures, multiple groups are written
                    //       in their fully expanded state to the molfile. The M SPA atom
                    //       list is a subset of the full atom list that is defined by the
                    //       Sgroup Atom List M SAL entry.
                    sgroup = EnsureSgroup(sgroups, ReadMolfileInt(line, 7));
                    count = ReadMolfileInt(line, 10);
                    ICollection<IAtom> parentAtomList = (ICollection<IAtom>)sgroup.GetValue(SgroupKey.CtabParentAtomList);
                    if (parentAtomList == null)
                    {
                        sgroup.PutValue(SgroupKey.CtabParentAtomList, parentAtomList = new HashSet<IAtom>());
                    }
                    for (int i = 0, st = 14; i < count && st + 3 <= length; i++, st += 4)
                    {
                        index = ReadMolfileInt(line, st) - 1;
                        parentAtomList.Add(container.Atoms[offset + index]);
                    }
                }
                else if (key == PropertyKey.M_SNC)
                {
                    // Sgroup Component Numbers [Sgroup]
                    // M  SNCnn8 sss ooo ...
                    // sss: Index of component Sgroup
                    // ooo: Integer component order (1...256). This limit applies only to MACCS-II
                    count = ReadMolfileInt(line, 6);
                    for (int i = 0, st = 10; i < count && st + 7 <= length; i++, st += 8)
                    {
                        sgroup = EnsureSgroup(sgroups,
                                              ReadMolfileInt(line, st));
                        sgroup.PutValue(SgroupKey.CtabComponentNumber,
                                        ReadMolfileInt(line, st + 4));
                    }
                }
                else if (key == PropertyKey.M_END)
                {
                    // M  END
                    //
                    // This entry goes at the end of the properties block and is required for molfiles which contain a
                    // version stamp in the counts line.
                    goto GoNext_LINES;
                }
            }
            GoNext_LINES:

            if (sgroups.Count != 0)
            {
                // load Sgroups into molecule, first we downcast
                List<Sgroup> sgroupOrgList = new List<Sgroup>(sgroups.Values);
                List<Sgroup> sgroupCpyList = new List<Sgroup>(sgroupOrgList.Count);
                for (int i = 0; i < sgroupOrgList.Count; i++)
                {
                    Sgroup cpy = sgroupOrgList[i].Downcast<Sgroup>();
                    sgroupCpyList.Add(cpy);
                }
                // update replaced parents
                for (int i = 0; i < sgroupOrgList.Count; i++)
                {
                    Sgroup newSgroup = sgroupCpyList[i];
                    ICollection<Sgroup> oldParents = new HashSet<Sgroup>(newSgroup.Parents);
                    newSgroup.RemoveParents(oldParents);
                    foreach (var parent in oldParents)
                    {
                        newSgroup.AddParent(sgroupCpyList[sgroupOrgList.IndexOf(parent)]);
                    }
                }
                container.SetProperty(CDKPropertyName.CTAB_SGROUPS, sgroupCpyList);
            }
        }

        private Sgroup EnsureSgroup(IDictionary<int, Sgroup> map, int idx)
        {
            Sgroup sgroup;
            if (!map.TryGetValue(idx, out sgroup))
            {
                if (mode == ChemObjectReaderModes.Strict)
                    HandleError("Sgroups must first be defined by a STY property");
                map[idx] = (sgroup = new Sgroup());
            }
            return sgroup;
        }

        /// <summary>
        /// Convert an MDL V2000 stereo value to the CDK {@link BondStereo}. The
        /// method should only be invoked for single/double bonds. If strict mode is
        /// enabled irrational bond stereo/types cause errors (e.g. up double bond).
        /// </summary>
        /// <param name="stereo">stereo value</param>
        /// <param name="type">bond type</param>
        /// <returns>bond stereo</returns>
        /// <exception cref="CDKException">the stereo value was invalid (strict mode).</exception>
        private BondStereo ToStereo(int stereo, int type)
        {
            switch (stereo)
            {
                case 0:
                    return type == 2 ? BondStereo.EZByCoordinates : BondStereo.None;
                case 1:
                    if (mode == ChemObjectReaderModes.Strict && type == 2)
                        throw new CDKException("stereo flag was 'up' but bond order was 2");
                    return BondStereo.Up;
                case 3:
                    if (mode == ChemObjectReaderModes.Strict && type == 1)
                        throw new CDKException("stereo flag was 'cis/trans' but bond order was 1");
                    return BondStereo.EOrZ;
                case 4:
                    if (mode == ChemObjectReaderModes.Strict && type == 2)
                        throw new CDKException("stereo flag was 'up/down' but bond order was 2");
                    return BondStereo.UpOrDown;
                case 6:
                    if (mode == ChemObjectReaderModes.Strict && type == 2)
                        throw new CDKException("stereo flag was 'down' but bond order was 2");
                    return BondStereo.Down;
            }
            if (mode == ChemObjectReaderModes.Strict) throw new CDKException("unknown bond stereo type: " + stereo);
            return BondStereo.None;
        }

        /// <summary>
        /// Determine the length of the line excluding trailing whitespace.
        /// </summary>
        /// <param name="str">a string</param>
        /// <returns> the length when trailing white space is removed</returns>
#if TEST
        public
#endif
        static int GetLength(string str)
        {
            int i = str.Length - 1;
            while (i >= 0 && str[i] == ' ')
            {
                i--;
            }
            return i + 1;
        }

        /// <summary>
        /// Create an atom for the provided symbol. If the atom symbol is a periodic
        /// element a new 'Atom' is created otherwise if the symbol is an allowed
        /// query atom ('R', 'Q', 'A', '*', 'L', 'LP') a new 'PseudoAtom' is created.
        /// If the symbol is invalid an exception is thrown.
        /// </summary>
        /// <param name="symbol">input symbol</param>
        /// <param name="builder">chem object builder</param>
        /// <param name="lineNum"></param>
        /// <returns>a new atom</returns>
        /// <exception cref="CDKException">the symbol is not allowed</exception>
        private IAtom CreateAtom(string symbol, IChemObjectBuilder builder, int lineNum)
        {
            if (IsPeriodicElement(symbol)) return builder.CreateAtom(symbol);

            if (!IsPseudoElement(symbol))
            {
                HandleError("invalid symbol: " + symbol, lineNum, 31, 34);
                // when strict only accept labels from the specification
                if (mode == ChemObjectReaderModes.Strict) throw new CDKException("invalid symbol: " + symbol);
            }

            // will be renumbered later by RGP if R1, R2 etc. if not renumbered then
            // 'R' is a better label than 'R#' if now RGP is specified
            if (symbol.Equals("R#")) symbol = "R";

            IAtom atom = builder.CreatePseudoAtom(symbol);
            atom.Symbol = symbol;
            atom.AtomicNumber = 0; // avoid NPE downstream

            return atom;
        }

        /// <summary>
        /// Is the symbol a periodic element.
        /// </summary>
        /// <param name="symbol">a symbol from the input</param>
        /// <returns>the symbol is a pseudo atom</returns>
        private static bool IsPeriodicElement(string symbol)
        {
            // XXX: PeriodicTable is slow - switch without file IO would be optimal
            int elem = PeriodicTable.GetAtomicNumber(symbol);
            return elem > 0;
        }

        /// <summary>
        /// Is the atom symbol a non-periodic element (i.e. pseudo). Valid pseudo
        /// atoms are 'R#', 'A', 'Q', '*', 'L' and 'LP'. We also accept 'R' but this
        /// is not listed in the specification.
        /// </summary>
        /// <param name="symbol">a symbol from the input</param>
        /// <returns>the symbol is a valid pseudo element</returns>
#if TEST
        public
#endif
            static bool IsPseudoElement(string symbol)
        {
            return PSEUDO_LABELS.Contains(symbol);
        }

        /// <summary>
        /// Read a coordinate from an MDL input. The MDL V2000 input coordinate has
        /// 10 characters, 4 significant figures and is prefixed with whitespace for
        /// padding: 'xxxxx.xxxx'. Knowing the format allows us to use an optimised
        /// parser which does not consider exponents etc.
        /// </summary>
        /// <param name="line">input line</param>
        /// <param name="offset">first character of the coordinate</param>
        /// <returns>the specified value</returns>
        /// <exception cref="CDKException">the coordinates specification was not valid</exception>
#if TEST
        public
#endif
        static double ReadMDLCoordinate(string line, int offset)
        {
            // to be valid the decimal should be at the fifth index (4 sig fig)
            if (line[offset + 5] != '.') throw new CDKException("invalid coordinate specification");

            int start = offset;
            while (line[start] == ' ')
                start++;

            int sign = Sign(line[start]);
            if (sign < 0) start++;

            int integral = ReadUInt(line, start, (offset + 5) - start);
            int fraction = ReadUInt(line, offset + 6, 4);

            return sign * (integral * 10000L + fraction) / 10000d;
        }

        /// <summary>
        /// Convert the a character (from an MDL V2000 input) to a charge value:
        /// 1 = +1, 2 = +2, 3 = +3, 4 = doublet radical, 5 = -1, 6 = -2, 7 = -3.
        /// </summary>
        /// <param name="c">a character</param>
        /// <returns>formal charge</returns>
        private static int ToCharge(char c)
        {
            switch (c)
            {
                case '1':
                    return +3;
                case '2':
                    return +2;
                case '3':
                    return +1;
                case '4':
                    return 0; // doublet radical - superseded by M  RAD
                case '5':
                    return -1;
                case '6':
                    return -2;
                case '7':
                    return -3;
            }
            return 0;
        }

        /// <summary>
        /// Obtain the sign of the character, -1 if the character is '-', +1
        /// otherwise.
        /// </summary>
        /// <param name="c">a character</param>
        /// <returns>the sign</returns>
        private static int Sign(char c)
        {
            return c == '-' ? -1 : +1;
        }

        /// <summary>
        /// Convert a character (ASCII code points) to an integer. If the character
        /// was not a digit (i.e. space) the value defaults to 0.
        /// </summary>
        /// <param name="c">a character</param>
        /// <returns>the numerical value</returns>
        private static int ToInt(char c)
        {
            // Character.getNumericalValue allows all of unicode which we don't want
            // or need it - imagine an MDL file with roman numerals!
            return c >= '0' && c <= '9' ? c - '0' : 0;
        }

        /// <summary>
        /// Read an unsigned int value from the given index with the expected number
        /// of digits.
        /// </summary>
        /// <param name="line">input line</param>
        /// <param name="index">start index</param>
        /// <param name="digits">number of digits (max)</param>
        /// <returns>an unsigned int</returns>
        private static int ReadUInt(string line, int index, int digits)
        {
            int result = 0;
            while (digits-- > 0)
                result = (result * 10) + ToInt(line[index++]);
            return result;
        }

        /// <summary>
        /// Optimised method for reading a integer from 3 characters in a string at a
        /// specified index. MDL V2000 Molfile make heavy use of the 3 character ints
        /// in the atom/bond and property blocks. The integer may be signed and
        /// pre/post padded with white space.
        /// </summary>
        /// <param name="line">input</param>
        /// <param name="index">start index</param>
        /// <returns>the value specified in the string</returns>
        private static int ReadMolfileInt(string line, int index)
        {
            int sign = 1;
            int result = 0;
            char c;
            switch ((c = line[index]))
            {
                case ' ':
                    break;
                case '-':
                    sign = -1;
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    result = (c - '0');
                    break;
                default:
                    return 0;
            }
            switch ((c = line[index + 1]))
            {
                case ' ':
                    if (result > 0) return sign * result;
                    break;
                case '-':
                    if (result > 0) return sign * result;
                    sign = -1;
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    result = (result * 10) + (c - '0');
                    break;
                default:
                    return sign * result;
            }
            switch ((c = line[index + 2]))
            {
                case ' ':
                    if (result > 0) return sign * result;
                    break;
                case '-':
                    if (result > 0) return sign * result;
                    sign = -1;
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    result = (result * 10) + (c - '0');
                    break;
                default:
                    return sign * result;
            }
            return sign * result;
        }

        /// <summary>
        /// Labels the atom at the specified index with the provide label. If the
        /// atom was not already a pseudo atom then the original atom is replaced.
        /// </summary>
        /// <param name="container">structure</param>
        /// <param name="index">atom index to replace</param>
        /// <param name="label">the label for the atom</param>
        /// <seealso cref="IPseudoAtom.Label"/>
#if TEST
        public
#endif
            static void Label(IAtomContainer container, int index, string label)
        {
            IAtom atom = container.Atoms[index];
            IPseudoAtom pseudoAtom = atom is IPseudoAtom ? (IPseudoAtom)atom : container.Builder
                   .CreatePseudoAtom();
            if (atom == pseudoAtom)
            {
                pseudoAtom.Label = label;
            }
            else
            {
                pseudoAtom.Symbol = label;
                pseudoAtom.AtomicNumber = 0;
                pseudoAtom.Point2D = atom.Point2D;
                pseudoAtom.Point3D = atom.Point3D;
                pseudoAtom.MassNumber = atom.MassNumber;
                pseudoAtom.FormalCharge = atom.FormalCharge;
                pseudoAtom.Valency = atom.Valency;
                pseudoAtom.Label = label;
                // XXX: would be faster to track all replacements and do it all in one
                AtomContainerManipulator.RePlaceAtomByAtom(container, atom, pseudoAtom);
            }
        }

        /// <summary>
        /// Reads an atom from the input allowing for non-standard formatting (i.e
        /// truncated lines) and chemical shifts.
        /// </summary>
        /// <param name="line">input line</param>
        /// <param name="builder">chem object builder</param>
        /// <param name="linecount">the current line count</param>
        /// <returns>an atom to add to a container</returns>
        /// <exception cref="CDKException">a CDK error occurred</exception>
        /// <exception cref="">the isotopes file could not be read</exception>
        private IAtom ReadAtomSlow(string line, IChemObjectBuilder builder, int linecount)
        {
            IAtom atom;
            line = TRAILING_SPACE.Replace(line, match =>
                {
                    HandleError("Trailing space found", linecount, match.Index, match.Index + match.Length);
                    return "";
                });
            double x = double.Parse(Strings.Substring(line, 0, 10).Trim());
            double y = double.Parse(Strings.Substring(line, 10, 10).Trim());
            double z = double.Parse(Strings.Substring(line, 20, 10).Trim());

            string element = Strings.Substring(line, 31, 3).Trim();
            if (line.Length < 34)
            {
                HandleError("Element atom type does not follow V2000 format type should of length three"
                        + " and padded with space if required", linecount, 31, 34);
            }

            Debug.WriteLine("Atom type: ", element);
            IsotopeFactory isotopeFactory = Isotopes.Instance;
            if (isotopeFactory.IsElement(element))
            {
                atom = isotopeFactory.Configure(builder.CreateAtom(element));
            }
            else if ("A".Equals(element))
            {
                atom = builder.CreatePseudoAtom(element);
            }
            else if ("Q".Equals(element))
            {
                atom = builder.CreatePseudoAtom(element);
            }
            else if ("*".Equals(element))
            {
                atom = builder.CreatePseudoAtom(element);
            }
            else if ("LP".Equals(element))
            {
                atom = builder.CreatePseudoAtom(element);
            }
            else if ("L".Equals(element))
            {
                atom = builder.CreatePseudoAtom(element);
            }
            else if (element.Equals("R") || (element.Length > 0 && element[0] == 'R'))
            {
                Debug.WriteLine("Atom ", element, " is not an regular element. Creating a PseudoAtom.");
                //check if the element is R
                if (element.Length > 1 && element[0] == 'R')
                {
                    try
                    {
                        element = "R" + int.Parse(element.Substring(1));
                        atom = builder.CreatePseudoAtom(element);
                    }
                    catch (Exception)
                    {
                        // This happens for atoms labeled "R#".
                        // The Rnumber may be set later on, using RGP line
                        atom = builder.CreatePseudoAtom("R");
                    }
                }
                else
                {
                    atom = builder.CreatePseudoAtom(element);
                }
            }
            else
            {
                HandleError("Invalid element type. Must be an existing " + "element, or one in: A, Q, L, LP, *.",
                        linecount, 32, 35);
                atom = builder.CreatePseudoAtom(element);
                atom.Symbol = element;
            }

            // store as 3D for now, convert to 2D (if totalZ == 0.0) later
            atom.Point3D = new Vector3(x, y, z);

            // parse further fields
            if (line.Length >= 36)
            {
                string massDiffString = Strings.Substring(line, 34, 2).Trim();
                Debug.WriteLine("Mass difference: ", massDiffString);
                if (!(atom is IPseudoAtom))
                {
                    try
                    {
                        int massDiff = int.Parse(massDiffString);
                        if (massDiff != 0)
                        {
                            IIsotope major = Isotopes.Instance.GetMajorIsotope(element);
                            atom.MassNumber = major.MassNumber + massDiff;
                        }
                    }
                    catch (FormatException exception)
                    {
                        HandleError("Could not parse mass difference field.", linecount, 35, 37, exception);
                    }
                    catch (IOException exception)
                    {
                        HandleError("Could not parse mass difference field.", linecount, 35, 37, exception);
                    }
                    catch (Exception)
                    {
                        Trace.TraceError("Cannot set mass difference for a non-element!");
                    }
                }
                else
                {
                    HandleError("Mass difference is missing", linecount, 34, 36);
                }
            }

            // set the stereo partiy
            int? parity = line.Length > 41 ? int.Parse(line[41].ToString()) : 0;
            atom.StereoParity = parity;

            if (line.Length >= 51)
            {
                string valenceString = RemoveNonDigits(Strings.Substring(line, 48, 3));
                Debug.WriteLine("Valence: ", valenceString);
                if (!(atom is IPseudoAtom))
                {
                    try
                    {
                        int valence = int.Parse(valenceString);
                        if (valence != 0)
                        {
                            //15 is defined as 0 in mol files
                            if (valence == 15)
                                atom.Valency = 0;
                            else
                                atom.Valency = valence;
                        }
                    }
                    catch (Exception exception)
                    {
                        HandleError("Could not parse valence information field", linecount, 49, 52, exception);
                    }
                }
                else
                {
                    Trace.TraceError("Cannot set valence information for a non-element!");
                }
            }

            if (line.Length >= 39)
            {
                string chargeCodeString = Strings.Substring(line, 36, 3).Trim();
                Debug.WriteLine("Atom charge code: ", chargeCodeString);
                int chargeCode = int.Parse(chargeCodeString);
                if (chargeCode == 0)
                {
                    // uncharged species
                }
                else if (chargeCode == 1)
                {
                    atom.FormalCharge = +3;
                }
                else if (chargeCode == 2)
                {
                    atom.FormalCharge = +2;
                }
                else if (chargeCode == 3)
                {
                    atom.FormalCharge = +1;
                }
                else if (chargeCode == 4)
                {
                }
                else if (chargeCode == 5)
                {
                    atom.FormalCharge = -1;
                }
                else if (chargeCode == 6)
                {
                    atom.FormalCharge = -2;
                }
                else if (chargeCode == 7)
                {
                    atom.FormalCharge = -3;
                }
            }
            else
            {
                HandleError("Atom charge is missing", linecount, 36, 39);
            }

            try
            {
                // read the mmm field as position 61-63
                string reactionAtomIDString = Strings.Substring(line, 60, 3).Trim();
                Debug.WriteLine("Parsing mapping id: ", reactionAtomIDString);
                try
                {
                    int reactionAtomID = int.Parse(reactionAtomIDString);
                    if (reactionAtomID != 0)
                    {
                        atom.SetProperty(CDKPropertyName.ATOM_ATOM_MAPPING, reactionAtomID);
                    }
                }
                catch (Exception exception)
                {
                    Trace.TraceError("Mapping number ", reactionAtomIDString, " is not an integer.");
                    Debug.WriteLine(exception);
                }
            }
            catch (Exception)
            {
                // older mol files don't have all these fields...
                Trace.TraceWarning("A few fields are missing. Older MDL MOL file?");
            }

            //shk3: This reads shifts from after the molecule. I don't think this is an official format, but I saw it frequently 80=>78 for alk
            if (line.Length >= 78)
            {
                double shift = double.Parse(Strings.Substring(line, 69, 11).Trim());
                atom.SetProperty("first shift", shift);
            }
            if (line.Length >= 87)
            {
                double shift = double.Parse(Strings.Substring(line, 79, 8).Trim());
                atom.SetProperty("second shift", shift);
            }

            return atom;
        }

        /// <summary>
        /// Read a bond line from an MDL V2000 molfile bond block (slow). The
        /// explicit valence is also modified.
        /// </summary>
        /// <param name="line">the input from the bond block</param>
        /// <param name="builder">chem object builder</param>
        /// <param name="atoms">array of atoms</param>
        /// <param name="explicitValence">stores the explicit valence of each atom (bond order sum)</param>
        /// <param name="linecount">the current line count</param>
        /// <returns>a new bond</returns>
        /// <exception cref="">the bond line could not be parsed</exception>
        private IBond ReadBondSlow(string line, IChemObjectBuilder builder, IAtom[] atoms, int[] explicitValence,
                int linecount)
        {
            int atom1 = int.Parse(Strings.Substring(line, 0, 3).Trim());
            int atom2 = int.Parse(Strings.Substring(line, 3, 3).Trim());
            int order = int.Parse(Strings.Substring(line, 6, 3).Trim());
            BondStereo stereo = BondStereo.None;
            if (line.Length >= 12)
            {
                int mdlStereo = line.Length > 12 ? int.Parse(Strings.Substring(line, 9, 3).Trim()) : int.Parse(line
                        .Substring(9).Trim());
                if (mdlStereo == 1)
                {
                    // MDL up bond
                    stereo = BondStereo.Up;
                }
                else if (mdlStereo == 6)
                {
                    // MDL down bond
                    stereo = BondStereo.Down;
                }
                else if (mdlStereo == 0)
                {
                    if (order == 2)
                    {
                        // double bond stereo defined by coordinates
                        stereo = BondStereo.EZByCoordinates;
                    }
                    else
                    {
                        // bond has no stereochemistry
                        stereo = BondStereo.None;
                    }
                }
                else if (mdlStereo == 3 && order == 2)
                {
                    // unknown E/Z stereochemistry
                    stereo = BondStereo.EOrZ;
                }
                else if (mdlStereo == 4)
                {
                    //MDL bond undefined
                    stereo = BondStereo.UpOrDown;
                }
            }
            else
            {
                HandleError("Missing expected stereo field at line: ", linecount, 10, 12);
            }
#if DEBUG
            Debug.WriteLine("Bond: " + atom1 + " - " + atom2 + "; order " + order);
#endif
            // interpret CTfile's special bond orders
            IAtom a1 = atoms[atom1 - 1];
            IAtom a2 = atoms[atom2 - 1];
            IBond newBond;
            if (order >= 1 && order <= 3)
            {
                BondOrder cdkOrder = BondOrder.Single;
                if (order == 2) cdkOrder = BondOrder.Double;
                if (order == 3) cdkOrder = BondOrder.Triple;
                if (stereo != BondStereo.None)
                {
                    newBond = builder.CreateBond(a1, a2, cdkOrder, stereo);
                }
                else
                {
                    newBond = builder.CreateBond(a1, a2, cdkOrder);
                }
                explicitValence[atom1 - 1] += cdkOrder.Numeric;
                explicitValence[atom2 - 1] += cdkOrder.Numeric;
            }
            else if (order == 4)
            {
                // aromatic bond
                if (stereo != BondStereo.None)
                {
                    newBond = builder.CreateBond(a1, a2, BondOrder.Unset, stereo);
                }
                else
                {
                    newBond = builder.CreateBond(a1, a2, BondOrder.Unset);
                }
                // mark both atoms and the bond as aromatic and raise the SingleOrDouble-flag
                newBond.IsSingleOrDouble = true;
                newBond.IsAromatic = true;
                a1.IsAromatic = true;
                a2.IsAromatic = true;
                explicitValence[atom1 - 1] = explicitValence[atom2 - 1] = int.MinValue;
            }
            else
            {
                newBond = new CTFileQueryBond(builder);
                newBond.SetAtoms(new[] { a1, a2 });
                newBond.Order = BondOrder.Unset;
                CTFileQueryBond.BondType queryBondType = CTFileQueryBond.BondType.Unset;
                switch (order)
                {
                    case 5:
                        queryBondType = CTFileQueryBond.BondType.SingleOrDouble;
                        break;
                    case 6:
                        queryBondType = CTFileQueryBond.BondType.SingleOrAromatic;
                        break;
                    case 7:
                        queryBondType = CTFileQueryBond.BondType.DoubleOrAromatic;
                        break;
                    case 8:
                        queryBondType = CTFileQueryBond.BondType.Any;
                        break;
                }
                ((CTFileQueryBond)newBond).Type = queryBondType;
                newBond.Stereo = stereo;
                explicitValence[atom1 - 1] = explicitValence[atom2 - 1] = int.MinValue;
            }
            return newBond;
        }

        private static readonly Regex Regex_A_d = new Regex("A\\s{1,4}\\d+", RegexOptions.Compiled);
        private static readonly Regex Regex_A_ = new Regex("A\\s{1,4}", RegexOptions.Compiled);

        /// <summary>
        /// Read the properties from the V2000 block (slow).
        /// </summary>
        /// <param name="input">input source</param>
        /// <param name="container">the container with the atoms / bonds loaded</param>
        /// <param name="nAtoms">the number of atoms in the atom block</param>
        /// <param name="linecount">the line count</param>
        /// <exception cref="">internal low-level error</exception>
        /// <exception cref="">the properties block could not be parsed</exception>
        private void ReadPropertiesSlow(TextReader input, IAtomContainer container, int nAtoms, int linecount)
        {
            Trace.TraceInformation("Reading property block");
            string line;
            while (true)
            {
                line = input.ReadLine();
                linecount++;
                if (line == null)
                {
                    HandleError("The expected property block is missing!", linecount, 0, 0);
                }
                if (line.StartsWith("M  END")) break;

                bool lineRead = false;
                if (line.StartsWith("M  CHG"))
                {
                    // FIXME: if this is encountered for the first time, all
                    // atom charges should be set to zero first!
                    int infoCount = int.Parse(Strings.Substring(line, 6, 3).Trim());

                    var st = ((IEnumerable<string>)Strings.Substring(line, 9).Split(' ', '\t')).GetEnumerator();
                    for (int i = 1; i <= infoCount; i++)
                    {
                        st.MoveNext();
                        string token = st.Current;
                        int atomNumber = int.Parse(token.Trim());
                        st.MoveNext();
                        token = st.Current;
                        int charge = int.Parse(token.Trim());
                        container.Atoms[atomNumber - 1].FormalCharge = charge;
                    }
                }
                else if (Regex_A_d.IsMatch(line))
                {
                    // Reads the pseudo atom property from the mol file

                    // The atom number of the to replaced atom
                    int aliasAtomNumber = int.Parse(Regex_A_d.Replace(line, "", 1));
                    string alias = input.ReadLine();
                    linecount++;
                    IAtom aliasAtom = container.Atoms[aliasAtomNumber - 1];

                    // skip if already a pseudoatom
                    if (aliasAtom is IPseudoAtom)
                    {
                        ((IPseudoAtom)aliasAtom).Label = alias;
                        continue;
                    }

                    IAtom newPseudoAtom = container.Builder.CreatePseudoAtom(alias);
                    if (aliasAtom.Point2D != null) newPseudoAtom.Point2D = aliasAtom.Point2D;
                    if (aliasAtom.Point3D != null) newPseudoAtom.Point3D = aliasAtom.Point3D;
                    AtomContainerManipulator.RePlaceAtomByAtom(container, aliasAtom, newPseudoAtom);
                }
                else if (line.StartsWith("M  ISO"))
                {
                    try
                    {
                        string countString = Strings.Substring(line, 6, 4).Trim();
                        int infoCount = int.Parse(countString);
                        var st = ((IEnumerable<string>)Strings.Substring(line, 10).Split(' ', '\t')).GetEnumerator();
                        for (int i = 1; i <= infoCount; i++)
                        {
                            st.MoveNext();
                            int atomNumber = int.Parse(st.Current.Trim());
                            st.MoveNext();
                            int absMass = int.Parse(st.Current.Trim());
                            if (absMass != 0)
                            {
                                IAtom isotope = container.Atoms[atomNumber - 1];
                                isotope.MassNumber = absMass;
                            }
                        }
                    }
                    catch (FormatException exception)
                    {
                        string error = "Error (" + exception.Message + ") while parsing line " + linecount + ": "
                                + line + " in property block.";
                        Trace.TraceError(error);
                        HandleError("FormatException in isotope information.", linecount, 7, 11, exception);
                    }
                }
                else if (line.StartsWith("M  RAD"))
                {
                    try
                    {
                        string countString = Strings.Substring(line, 6, 3).Trim();
                        int infoCount = int.Parse(countString);
                        var st = ((IEnumerable<string>)Strings.Substring(line, 9).Split(' ', '\t')).GetEnumerator();
                        for (int i = 1; i <= infoCount; i++)
                        {
                            st.MoveNext();
                            int atomNumber = int.Parse(st.Current.Trim());
                            st.MoveNext();
                            int spinMultiplicity = int.Parse(st.Current.Trim());
                            MDLV2000Writer.SpinMultiplicity spin = MDLV2000Writer.SpinMultiplicity.None;
                            if (spinMultiplicity > 0)
                            {
                                IAtom radical = container.Atoms[atomNumber - 1];
                                switch (spinMultiplicity)
                                {
                                    case 1:
                                        spin = MDLV2000Writer.SpinMultiplicity.Doublet;
                                        break;
                                    case 2:
                                        spin = MDLV2000Writer.SpinMultiplicity.Singlet;
                                        break;
                                    case 3:
                                        spin = MDLV2000Writer.SpinMultiplicity.Triplet;
                                        break;
                                    default:
                                        Debug.WriteLine("Invalid spin multiplicity found: " + spinMultiplicity);
                                        break;
                                }
                                for (int j = 0; j < spin.SingleElectrons; j++)
                                {
                                    container.Add(container.Builder.CreateSingleElectron(radical));
                                }
                            }
                        }
                    }
                    catch (FormatException exception)
                    {
                        string error = "Error (" + exception.Message + ") while parsing line " + linecount + ": "
                                + line + " in property block.";
                        Trace.TraceError(error);
                        HandleError("FormatException in radical information", linecount, 7, 10, exception);
                    }
                }
                else if (line.StartsWith("G  "))
                {
                    try
                    {
                        string atomNumberString = Strings.Substring(line, 3, 3).Trim();
                        int atomNumber = int.Parse(atomNumberString);
                        //string whatIsThisString = line.Substring(6,9).Trim();

                        string atomName = input.ReadLine();

                        // convert Atom into a PseudoAtom
                        IAtom prevAtom = container.Atoms[atomNumber - 1];
                        IPseudoAtom pseudoAtom = container.Builder.CreatePseudoAtom(atomName);
                        if (prevAtom.Point2D != null)
                        {
                            pseudoAtom.Point2D = prevAtom.Point2D;
                        }
                        if (prevAtom.Point3D != null)
                        {
                            pseudoAtom.Point3D = prevAtom.Point3D;
                        }
                        AtomContainerManipulator.RePlaceAtomByAtom(container, prevAtom, pseudoAtom);
                    }
                    catch (FormatException exception)
                    {
                        string error = "Error (" + exception.ToString() + ") while parsing line " + linecount + ": " + line
                                + " in property block.";
                        Trace.TraceError(error);
                        HandleError("FormatException in group information", linecount, 4, 7, exception);
                    }
                }
                else if (line.StartsWith("M  RGP"))
                {
                    var st = ((IEnumerable<string>)line.Split(' ', '\t')).GetEnumerator();
                    //Ignore first 3 tokens (overhead).
                    st.MoveNext();
                    st.MoveNext();
                    st.MoveNext();
                    //Process the R group numbers as defined in RGP line.
                    while (st.MoveNext())
                    {
                        int position = int.Parse(st.Current);
                        st.MoveNext();
                        int rNumber = int.Parse(st.Current);
                        // the container may have already had atoms before the new atoms were read
                        int index = container.Atoms.Count - nAtoms + position - 1;
                        IPseudoAtom pseudoAtom = (IPseudoAtom)container.Atoms[index];
                        if (pseudoAtom != null)
                        {
                            pseudoAtom.Label = "R" + rNumber;
                        }
                    }
                }
                if (line.StartsWith("V  "))
                {
                    int atomNumber = int.Parse(Strings.Substring(line, 3, 3).Trim());
                    IAtom atomWithComment = container.Atoms[atomNumber - 1];
                    atomWithComment.SetProperty(CDKPropertyName.COMMENT, Strings.Substring(line, 7));
                }

                if (!lineRead)
                {
                    Trace.TraceWarning("Skipping line in property block: ", line);
                }
            }
        }

        /// <summary>
        /// Read non-structural data from input and store as properties the provided
        /// 'container'. Non-structural data appears in a structure data file (SDF)
        /// after an Molfile and before the record deliminator ('$$$$'). The data
        /// consists of one or more Data Header and Data blocks, an example is seen
        /// below.
        /// <![CDATA[
        /// > 29 <DENSITY>
        /// 0.9132 - 20.0
        /// 
        /// > 29 <BOILING.POINT>
        /// 63.0 (737 MM)
        /// 79.0 (42 MM)
        /// 
        /// > 29 <ALTERNATE.NAMES>
        /// SYLVAN
        /// 
        /// > 29 <DATE>
        /// 09-23-1980
        /// 
        /// > 29 <CRC.NUMBER>
        /// F-0213
        /// 
        /// ]]>
        /// </summary>
        /// <param name="input">input source</param>
        /// <param name="container">the container</param>
        /// <exception cref="">an error occur whilst reading the input</exception>
#if TEST
        public
#endif
            static void ReadNonStructuralData(TextReader input, IAtomContainer container)
        {

            string line, header = null;
            bool wrap = false;

            StringBuilder data = new StringBuilder(80);

            while (!EndOfRecord(line = input.ReadLine()))
            {

                string newHeader = DataHeader(line);

                if (newHeader != null)
                {

                    if (header != null) container.SetProperty(header, data.ToString());

                    header = newHeader;
                    wrap = false;
                    data.Length = 0;

                }
                else
                {

                    if (data.Length > 0 || !line.Equals(" ")) line = line.Trim();

                    if (string.IsNullOrEmpty(line)) continue;

                    if (!wrap && data.Length > 0) data.Append('\n');
                    data.Append(line);

                    wrap = line.Length == 80;
                }
            }

            if (header != null) container.SetProperty(header, data.ToString());
        }

        /// <summary>
        /// Obtain the field name from a potential SD data header. If the header
        /// does not contain a field name, then null is returned. The method does
        /// not currently return field numbers (e.g. DT&lt;n&gt;).
        /// </summary>
        /// <param name="line">an input line</param>
        /// <returns>the field name</returns>
#if TEST
        public
#endif
        static string DataHeader(string line)
        {
            if (line.Length > 2 && line[0] != '>' && line[1] != ' ') return null;
            if (line.Length < 2)
                return null;
            int i = line.IndexOf('<', 2);
            if (i < 0) return null;
            int j = line.IndexOf('>', i);
            if (j < 0) return null;
            return Strings.Substring(line, i + 1, j - (i + 1));
        }

        /// <summary>
        /// Is the line the end of a record. A line is the end of a record if it
        /// is 'null' or is the SDF deliminator, '$$$$'.
        /// </summary>
        /// <param name="line">a line from the input</param>
        /// <returns>the line indicates the end of a record was reached</returns>
        private static bool EndOfRecord(string line)
        {
            return line == null || line.Equals(RECORD_DELIMITER);
        }

        /// <summary>
        /// Enumeration of property keys that can be specified in the V2000 property block.
        /// </summary>
#if TEST
        public
#endif        
        sealed class PropertyKey
        {
            /// <summary>Atom Alias.</summary>
            public static readonly PropertyKey ATOM_ALIAS = new PropertyKey("ATOM_ALIAS");

            /// <summary>Atom Value.</summary>
            public static readonly PropertyKey ATOM_VALUE = new PropertyKey("ATOM_VALUE");

            /// <summary>Group Abbreviation.</summary>
            public static readonly PropertyKey GROUP_ABBREVIATION = new PropertyKey("GROUP_ABBREVIATION");

            /// <summary>Skip lines.</summary>
            public static readonly PropertyKey SKIP = new PropertyKey("SKIP");

            /// <summary>Charge [Generic].</summary>
            public static readonly PropertyKey M_CHG = new PropertyKey("M_CHG");

            /// <summary>Radical [Generic].</summary>
            public static readonly PropertyKey M_RAD = new PropertyKey("M_RAD");

            /// <summary>Isotope [Generic].</summary>
            public static readonly PropertyKey M_ISO = new PropertyKey("M_ISO");

            /// <summary>Ring Bond Count [Query].</summary>
            public static readonly PropertyKey M_RBC = new PropertyKey("M_RBC");

            /// <summary>Substitution Count [Query].</summary>
            public static readonly PropertyKey M_SUB = new PropertyKey("M_SUB");

            /// <summary>Unsaturated Atom [Query].</summary>
            public static readonly PropertyKey M_UNS = new PropertyKey("M_UNS");

            /// <summary>Link Atom [Query].</summary>
            public static readonly PropertyKey M_LIN = new PropertyKey("M_LIN");

            /// <summary>Atom List [Query].</summary>
            public static readonly PropertyKey M_ALS = new PropertyKey("M_ALS");

            /// <summary>Attachment Point [Rgroup].</summary>
            public static readonly PropertyKey M_APO = new PropertyKey("M_APO");

            /// <summary>Atom Attachment Order [Rgroup].</summary>
            public static readonly PropertyKey M_AAL = new PropertyKey("M_AAL");

            /// <summary>Rgroup Label Location [Rgroup].</summary>
            public static readonly PropertyKey M_RGP = new PropertyKey("M_RGP");

            /// <summary>Rgroup Logic, Unsatisfied Sites, Range of Occurrence [Rgroup].</summary>
            public static readonly PropertyKey M_LOG = new PropertyKey("M_LOG");

            /// <summary>Sgroup Type [Sgroup].</summary>
            public static readonly PropertyKey M_STY = new PropertyKey("M_STY");

            /// <summary>Sgroup Subtype [Sgroup].</summary>
            public static readonly PropertyKey M_SST = new PropertyKey("M_SST");

            /// <summary>Sgroup Labels [Sgroup].</summary>
            public static readonly PropertyKey M_SLB = new PropertyKey("M_SLB");

            /// <summary>Sgroup Connectivity [Sgroup].</summary>
            public static readonly PropertyKey M_SCN = new PropertyKey("M_SCN");

            /// <summary>Sgroup Expansion [Sgroup].</summary>
            public static readonly PropertyKey M_SDS = new PropertyKey("M_SDS");

            /// <summary>Sgroup Atom List [Sgroup].</summary>
            public static readonly PropertyKey M_SAL = new PropertyKey("M_SAL");

            /// <summary>Sgroup Bond List [Sgroup].</summary>
            public static readonly PropertyKey M_SBL = new PropertyKey("M_SBL");

            /// <summary>Multiple Group Parent Atom List [Sgroup].</summary>
            public static readonly PropertyKey M_SPA = new PropertyKey("M_SPA");

            /// <summary>Sgroup Subscript [Sgroup].</summary>
            public static readonly PropertyKey M_SMT = new PropertyKey("M_SMT");

            /// <summary>Sgroup Correspondence [Sgroup].</summary>
            public static readonly PropertyKey M_CRS = new PropertyKey("M_CRS");

            /// <summary>Sgroup Display Information [Sgroup].</summary>
            public static readonly PropertyKey M_SDI = new PropertyKey("M_SDI");

            /// <summary>Superatom Bond and Vector Information [Sgroup].</summary>
            public static readonly PropertyKey M_SBV = new PropertyKey("M_SBV");

            /// <summary>Data Sgroup Field Description [Sgroup].</summary>
            public static readonly PropertyKey M_SDT = new PropertyKey("M_SDT");

            /// <summary>Data Sgroup Display Information [Sgroup].</summary>
            public static readonly PropertyKey M_SDD = new PropertyKey("M_SDD");

            /// <summary>Data Sgroup Data.</summary>
            public static readonly PropertyKey M_SCD = new PropertyKey("M_SCD");

            /// <summary>Data Sgroup Data.</summary>
            public static readonly PropertyKey M_SED = new PropertyKey("M_SED");

            /// <summary>Sgroup Hierarchy Information.</summary>
            public static readonly PropertyKey M_SPL = new PropertyKey("M_SPL");

            /// <summary>Sgroup Component Numbers.</summary>
            public static readonly PropertyKey M_SNC = new PropertyKey("M_SNC");

            /// <summary>Sgroup Bracket Style.</summary>
            public static readonly PropertyKey M_SBT = new PropertyKey("M_SBT");

            /// <summary>3D Feature Properties.</summary>
            public static readonly PropertyKey M_S3D = new PropertyKey("M_$3D");

            /// <summary>ACDLabs Atom Label</summary>
            public static readonly PropertyKey M_ZZC = new PropertyKey("M_ZZC");

            /// <summary>End of Block.</summary>
            public static readonly PropertyKey M_END = new PropertyKey("M_END");

            /// <summary>Non-property header.</summary>
            public static readonly PropertyKey Unknown = new PropertyKey("Unknown");

            public static readonly PropertyKey[] Values = new[] {
                ATOM_ALIAS,
                ATOM_VALUE,
                GROUP_ABBREVIATION,
                SKIP,
                M_CHG,
                M_RAD,
                M_ISO,
                M_RBC,
                M_SUB,
                M_UNS,
                M_LIN,
                M_ALS,
                M_APO,
                M_AAL,
                M_RGP,
                M_LOG,
                M_STY,
                M_SST,
                M_SLB,
                M_SCN,
                M_SDS,
                M_SAL,
                M_SBL,
                M_SPA,
                M_SMT,
                M_CRS,
                M_SDI,
                M_SDT,
                M_SDD,
                M_SCD,
                M_SED,
                M_SPL,
                M_SNC,
                M_SBT,
                M_S3D,
                M_ZZC,
                M_END,
                Unknown,
            };

            /// <summary>Index of 'M XXX' properties for quick lookup.</summary>
            private static IDictionary<string, PropertyKey> mSuffix = new Dictionary<string, PropertyKey>(60);

            static PropertyKey()
            {
                foreach (var p in Values)
                {
                    if (p.Name[0] == 'M') mSuffix[p.Name.Substring(2, 3)] = p;
                }
            }

            public string Name { get; private set; }

            public PropertyKey(string name)
            {
                Name = name;
            }

            /// <summary>
	        /// Determine the property key of the provided line.
        	/// </summary>
	        /// <param name="line">an property line</param>
	        /// <returns>the key (defaults to <see cref="Unknown"/>)</returns>
            public static PropertyKey Of(string line)
            {
                if (line.Length < 5) return Unknown;
                switch (line[0])
                {
                    case 'A':
                        if (line[1] == ' ' && line[2] == ' ') return ATOM_ALIAS;
                        return Unknown;
                    case 'G':
                        if (line[1] == ' ' && line[2] == ' ') return GROUP_ABBREVIATION;
                        return Unknown;
                    case 'S':
                        if (line[1] == ' ' && line[2] == ' ') return SKIP;
                        return Unknown;
                    case 'V':
                        if (line[1] == ' ' && line[2] == ' ') return ATOM_VALUE;
                        return Unknown;
                    case 'M':
                        if (line[1] != ' ' || line[2] != ' ') return Unknown;
                        PropertyKey property;
                        if (mSuffix.TryGetValue(Strings.Substring(line, 3, 3), out property))
                        {
                            return property;
                        }
                        return Unknown;
                }
                return Unknown;
            }
        }

        /// <summary>
        /// Defines the version of the CTab.
        /// </summary>
#if TEST
        public
#endif
        class CTabVersion
        {
            public static readonly CTabVersion V2000 = new CTabVersion("V2000");
            public static readonly CTabVersion V3000 = new CTabVersion("V3000");
            public static readonly CTabVersion Unspecified = new CTabVersion("Unspecified");

            public string Name { get; private set; }

            public CTabVersion(string name)
            {
                Name = name;
            }

            /// <summary>
	        /// Given a CTab header, what version was specified. The version
	        /// is identifier in the by the presence of 'V[2|3]000'. If not
	        /// version tag is present the version is unspecified.
        	/// <![CDATA[
	        ///   5  5  0  0  0  0            999 V2000
	        ///   0  0  0  0  0  0            999 V3000
        	/// ]]>
            /// </summary>
	        /// <param name="header">input line (non-null)</param>
	        /// <returns>the CTab version</returns>
            public static CTabVersion OfHeader(string header)
            {
                if (header.Length < 39) return Unspecified;
                char c = header[34];
                if (c != 'v' && c != 'V') return Unspecified;
                if (header[35] == '2') // could check for '000'
                    return V2000;
                if (header[35] == '3') // could check for '000'
                    return V3000;
                return Unspecified;
            }
        }
    }
}
