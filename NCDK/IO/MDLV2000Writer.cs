/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *                    2009  Egon Willighagen <egonw@users.sf.net>
 *                    2010  Mark Rijnbeek <mark_rynbeek@users.sf.net>
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
using NCDK.Config;
using NCDK.IO.Formats;
using NCDK.IO.Setting;
using NCDK.Isomorphisms.Matchers;
using NCDK.SGroups;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NCDK.IO
{
    /// <summary>
    /// Writes MDL molfiles, which contains a single molecule (see <token>cdk-cite-DAL92</token>).
    /// </summary>
    /// <example>
    /// For writing a MDL molfile you can this code:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.IO.MDLV2000Writer_Example.cs"]/*' />
    /// The writer has two IO settings: one for writing 2D coordinates, even if
    /// 3D coordinates are given for the written data; the second writes aromatic
    /// bonds as bond type 4, which is, strictly speaking, a query bond type, but
    /// my many tools used to reflect aromaticity. The full IO setting API is
    /// explained in CDK News <token>cdk-cite-WILLIGHAGEN2004</token>. One programmatic option
    /// to set the option for writing 2D coordinates looks like:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.IO.MDLV2000Writer_Example.cs+listener"]/*' />
    /// </example>
    // @cdk.module io
    // @cdk.githash
    // @cdk.iooptions
    // @cdk.keyword file format, MDL molfile
    public class MDLV2000Writer : DefaultChemObjectWriter
    {
        // regular expression to capture R groups with attached numbers
        private static readonly Regex NUMERED_R_GROUP = new Regex("R(\\d+)", RegexOptions.Compiled);

        /// <summary>
        /// Enumeration of all valid radical values.
        /// </summary>
        public class SpinMultiplicity
        {
            public static readonly SpinMultiplicity None = new SpinMultiplicity(0, 0);
            public static readonly SpinMultiplicity Monovalent = new SpinMultiplicity(2, 1);
            public static readonly SpinMultiplicity DivalentSinglet = new SpinMultiplicity(1, 2);
            public static readonly SpinMultiplicity DivalentTriplet = new SpinMultiplicity(3, 2);

            /// <summary>
            /// Radical value for the spin multiplicity in the properties block.
            /// </summary>
            public int Value { get; private set; }

            /// <summary>
            /// The number of single electrons that correspond to the spin multiplicity.
            /// </summary>
            public int SingleElectrons { get; private set; }

            private SpinMultiplicity(int value, int singleElectrons)
            {
                Value = value;
                SingleElectrons = singleElectrons;
            }
            /// <summary>
            /// Create a SpinMultiplicity instance for the specified value.
            /// </summary>
            /// <param name="value">input value (in the property block)</param>
            /// <returns>instance</returns>
            // @ unknown spin multiplicity value
            public static SpinMultiplicity OfValue(int value)
            {
                switch (value)
                {
                    case 0:
                        return None;
                    case 1:
                        return DivalentSinglet;
                    case 2:
                        return Monovalent;
                    case 3:
                        return DivalentTriplet;
                    default:
                        throw new CDKException("unknown spin multiplicity: " + value);
                }
            }
        }

        // number of entries on line; value = 1 to 8
        private const int NN8 = 8;
        // spacing between entries on line
        private const int WIDTH = 3;

        private BooleanIOSetting ForceWriteAs2DCoords;

        // The next two options are MDL Query format options, not really
        // belonging to the MDLV2000 format, and will be removed when
        // a MDLV2000QueryWriter is written.

        /// <summary>
        /// Should aromatic bonds be written as bond type 4? If true, this makes the
        /// output a query file.
        /// </summary>
        private BooleanIOSetting WriteAromaticBondTypes;

        /* Should atomic valencies be written in the Query format. */
        [Obsolete]
        private BooleanIOSetting WriteQueryFormatValencies;

        private TextWriter writer;

        /// <summary>
        /// Used only for InitIOSettings
        /// </summary>
        internal MDLV2000Writer()
            : this((TextWriter)null)
        { }

        /// <summary>
        /// Constructs a new MDLWriter that can write an <see cref="IAtomContainer"/>
        /// to the MDL molfile format.
        /// </summary>
        /// <param name="writer">The Writer to write to</param>
        public MDLV2000Writer(TextWriter writer)
        {
            this.writer = writer;
            InitIOSettings();
        }

        /// <summary>
        /// Constructs a new MDLWriter that can write an <see cref="IAtomContainer"/>
        /// to a given Stream.
        /// </summary>
        /// <param name="output">The Stream to write to</param>
        public MDLV2000Writer(Stream output)
            : this(new StreamWriter(output, Encoding.UTF8))
        { }

        public override IResourceFormat Format => MDLFormat.Instance;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (writer != null)
                        writer.Dispose();
                }

                writer = null;

                disposedValue = true;
                base.Dispose(disposing);
            }
        }
        #endregion

        public override bool Accepts(Type type)
        {
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            if (typeof(IChemModel).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Writes a <see cref="IChemObject"/> to the MDL molfile formated output.
        /// It can only output ChemObjects of type <see cref="IChemFile"/>,
        /// <see cref="IChemObject"/> and <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="obj"><see cref="IChemObject"/> to write</param>
        /// <see cref="Accepts(Type)"/>
        public override void Write(IChemObject obj)
        {
            CustomizeJob();
            try
            {
                if (obj is IChemFile)
                {
                    WriteChemFile((IChemFile)obj);
                    return;
                }
                else if (obj is IChemModel)
                {
                    IChemFile file = obj.Builder.NewChemFile();
                    IChemSequence sequence = obj.Builder.NewChemSequence();
                    sequence.Add((IChemModel)obj);
                    file.Add(sequence);
                    WriteChemFile((IChemFile)file);
                    return;
                }
                else if (obj is IAtomContainer)
                {
                    WriteMolecule((IAtomContainer)obj);
                    return;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                Debug.WriteLine(ex);
                throw new CDKException("Exception while writing MDL file: " + ex.Message, ex);
            }
            throw new CDKException("Only supported is writing of IChemFile, " + "IChemModel, and IAtomContainer objects.");
        }

        private void WriteChemFile(IChemFile file)
        {
            IAtomContainer bigPile = file.Builder.NewAtomContainer();
            foreach (var container in ChemFileManipulator.GetAllAtomContainers(file))
            {
                bigPile.Add(container);
                if (container.GetProperty<string>(CDKPropertyName.Title) != null)
                {
                    if (bigPile.GetProperty<string>(CDKPropertyName.Title) != null)
                        bigPile.SetProperty(CDKPropertyName.Title,
                                            bigPile.GetProperty<string>(CDKPropertyName.Title) + "; " + container.GetProperty<string>(CDKPropertyName.Title));
                    else
                        bigPile.SetProperty(CDKPropertyName.Title, container.GetProperty<string>(CDKPropertyName.Title));
                }
                if (container.GetProperty<string>(CDKPropertyName.Remark) != null)
                {
                    if (bigPile.GetProperty<string>(CDKPropertyName.Remark) != null)
                        bigPile.SetProperty(CDKPropertyName.Remark, bigPile.GetProperty<string>(CDKPropertyName.Remark) + "; "
                                                                 + container.GetProperty<string>(CDKPropertyName.Remark));
                    else
                        bigPile.SetProperty(CDKPropertyName.Remark, container.GetProperty<string>(CDKPropertyName.Remark));
                }
            }
            WriteMolecule(bigPile);
        }

        /// <summary>
        /// Writes a Molecule to an Stream in MDL sdf format.
        /// </summary>
        /// <param name="container">Molecule that is written to an Stream</param>
        public void WriteMolecule(IAtomContainer container)
        {
            int dim = GetNumberOfDimensions(container);
            string line = "";
            IDictionary<int, int> rgroups = null;
            IDictionary<int, string> aliases = null;
            // write header block
            // lines get shortened to 80 chars, that's in the spec
            string title = container.GetProperty<string>(CDKPropertyName.Title);
            if (title == null) title = "";
            if (title.Length > 80) title = title.Substring(0, 80);
            writer.Write(title);
            writer.Write('\n');

            // From CTX spec This line has the format:
            // IIPPPPPPPPMMDDYYHHmmddSSssssssssssEEEEEEEEEEEERRRRRR (FORTRAN:
            // A2<--A8--><---A10-->A2I2<--F10.5-><---F12.5--><-I6-> ) User's first
            // and last initials (l), program name (P), date/time (M/D/Y,H:m),
            // dimensional codes (d), scaling factors (S, s), energy (E) if modeling
            // program input, internal registry number (R) if input through MDL
            // form. A blank line can be substituted for line 2.
            writer.Write("  CDK     ");
            writer.Write(DateTime.Now.ToUniversalTime().ToString("MMddyyHHmm"));
            if (dim != 0)
            {
                writer.Write(dim.ToString());
                writer.Write('D');
            }
            writer.Write('\n');

            string comment = container.GetProperty<string>(CDKPropertyName.Remark);
            if (comment == null) comment = "";
            if (comment.Length > 80) comment = comment.Substring(0, 80);
            writer.Write(comment);
            writer.Write('\n');

            // index stereo elements for setting atom parity values
            IDictionary<IAtom, ITetrahedralChirality> atomstereo = new Dictionary<IAtom, ITetrahedralChirality>();
            IDictionary<IAtom, int> atomindex = new Dictionary<IAtom, int>();
            foreach (var element in container.StereoElements)
                if (element is ITetrahedralChirality)
                    atomstereo[((ITetrahedralChirality)element).ChiralAtom] = (ITetrahedralChirality)element;
            foreach (var atom in container.Atoms)
                atomindex[atom] = atomindex.Count;

            // write Counts line
            line += FormatMDLInt(container.Atoms.Count, 3);
            line += FormatMDLInt(container.Bonds.Count, 3);
            line += "  0  0";
            // we mark all stereochemistry to absolute for now
            line += !atomstereo.Any() ? "  0" : "  1";
            line += "  0  0  0  0  0999 V2000";
            writer.Write(line);
            writer.Write('\n');

            // write Atom block
            for (int f = 0; f < container.Atoms.Count; f++)
            {
                IAtom atom = container.Atoms[f];
                line = "";
                switch (dim)
                {
                    case 0:
                        // if no coordinates available, then output a number
                        // of zeros
                        line += "    0.0000    0.0000    0.0000 ";
                        break;
                    case 2:
                        if (atom.Point2D != null)
                        {
                            line += FormatMDLFloat(atom.Point2D.Value.X);
                            line += FormatMDLFloat(atom.Point2D.Value.Y);
                            line += "    0.0000 ";
                        }
                        else
                        {
                            line += "    0.0000    0.0000    0.0000 ";
                        }
                        break;
                    case 3:
                        if (atom.Point3D != null)
                        {
                            line += FormatMDLFloat((float)atom.Point3D.Value.X);
                            line += FormatMDLFloat((float)atom.Point3D.Value.Y);
                            line += FormatMDLFloat((float)atom.Point3D.Value.Z) + " ";
                        }
                        else
                        {
                            line += "    0.0000    0.0000    0.0000 ";
                        }
                        break;
                }
                if (container.Atoms[f] is IPseudoAtom)
                {
                    //according to http://www.google.co.uk/url?sa=t&ct=res&cd=2&url=http%3A%2F%2Fwww.mdl.com%2Fdownloads%2Fpublic%2Fctfile%2Fctfile.pdf&ei=MsJjSMbjAoyq1gbmj7zCDQ&usg=AFQjCNGaJSvH4wYy4FTXIaQ5f7hjoTdBAw&sig2=eSfruNOSsdMFdlrn7nhdAw an R group is written as R#
                    IPseudoAtom pseudoAtom = (IPseudoAtom)container.Atoms[f];
                    string label = pseudoAtom.Label;
                    if (label == null) // set to empty string if null
                        label = "";

                    // firstly check if it's a numbered R group
                    Match matcher = NUMERED_R_GROUP.Match(label);
                    if (pseudoAtom.Symbol.Equals("R") && !string.IsNullOrEmpty(label) && matcher.Success)
                    {

                        line += "R# ";
                        if (rgroups == null)
                        {
                            // we use a tree map to ensure the output order is always the same
                            rgroups = new SortedDictionary<int, int>();
                        }
                        rgroups[f + 1] = int.Parse(matcher.Groups[1].Value);
                    }
                    // not a numbered R group - note the symbol may still be R
                    else
                    {
                        // note: no distinction made between alias and pseudo atoms - normally
                        //       aliases maintain their original symbol while pseudo atoms are
                        //       written with a 'A' in the atom block

                        // if the label is longer then 3 characters we need
                        // to use an alias.
                        if (label.Length > 3)
                        {

                            if (aliases == null) aliases = new SortedDictionary<int, string>();

                            aliases[f + 1] = label; // atom index to alias

                            line += FormatMDLString(atom.Symbol, 3);

                        }
                        else
                        { // label is short enough to fit in the atom block

                            // make sure it's not empty
                            if (!string.IsNullOrEmpty(label))
                                line += FormatMDLString(label, 3);
                            else
                                line += FormatMDLString(atom.Symbol, 3);
                        }
                    }
                }
                else
                {
                    line += FormatMDLString(container.Atoms[f].Symbol, 3);
                }

                ITetrahedralChirality tc;
                if (!atomstereo.TryGetValue(atom, out tc))
                {
                    line += " 0  0  0  0  0";
                }
                else
                {
                    int parity = tc.Stereo == TetrahedralStereo.Clockwise ? 1 : 2;
                    IAtom focus = tc.ChiralAtom;
                    var carriers = tc.Ligands;

                    int hidx = -1;
                    for (int i = 0; i < 4; i++)
                    {
                        // hydrogen position
                        if (carriers[i] == focus || carriers[i].AtomicNumber == 1)
                        {
                            if (hidx >= 0) parity = 0;
                            hidx = i;
                        }
                    }

                    if (parity != 0)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = i + 1; j < 4; j++)
                            {
                                int a = atomindex[carriers[i]];
                                int b = atomindex[carriers[j]];
                                if (i == hidx)
                                    a = container.Atoms.Count;
                                if (j == hidx)
                                    b = container.Atoms.Count;
                                if (a > b)
                                    parity ^= 0x3;
                            }
                        }
                    }
                    line += $" 0  0  {parity}  0  0";
                }

                // write valence - this is a bit of pain as the CDK has both
                // valence and implied hydrogen counts making life a lot more
                // difficult than it needs to be - we also have formal
                // neighbor count but to avoid more verbosity that check has been
                // omitted
                {
                    try
                    {
                        // slow but neat
                        int explicitValence = (int)AtomContainerManipulator.GetBondOrderSum(container, atom);
                        int charge = atom.FormalCharge ?? 0;
                        int? element = atom.AtomicNumber;

                        if (element == null)
                        {
                            line += FormatMDLInt(0, 3);
                        }
                        else
                        {
                            int implied = MDLValence.ImplicitValence(element.Value, charge, explicitValence);

                            if (atom.Valency != null && atom.ImplicitHydrogenCount != null)
                            {
                                int valence = atom.Valency.Value;
                                int actual = explicitValence + atom.ImplicitHydrogenCount.Value;

                                // valence from h count differs from field? we still
                                // set to default - which one has more merit?
                                if (valence != actual || implied == atom.Valency)
                                    line += FormatMDLInt(0, 3);
                                else if (valence == 0)
                                    line += FormatMDLInt(15, 3);
                                else if (valence > 0 && valence < 15)
                                    line += FormatMDLInt(valence, 3);
                                else
                                    line += FormatMDLInt(0, 3);
                            }
                            else if (atom.ImplicitHydrogenCount != null)
                            {
                                int actual = explicitValence + atom.ImplicitHydrogenCount.Value;

                                if (implied == actual)
                                {
                                    line += FormatMDLInt(0, 3);
                                }
                                else
                                {
                                    if (actual == 0)
                                        line += FormatMDLInt(15, 3);
                                    else if (actual > 0 && actual < 15)
                                        line += FormatMDLInt(actual, 3);
                                    else
                                        line += FormatMDLInt(0, 3);
                                }
                            }
                            else
                            {
                                int valence = atom.Valency.Value;

                                // valence from h count differs from field? we still
                                // set to default - which one has more merit?
                                if (implied == valence)
                                    line += FormatMDLInt(0, 3);
                                else if (valence == 0)
                                    line += FormatMDLInt(15, 3);
                                else if (valence > 0 && valence < 15)
                                    line += FormatMDLInt(valence, 3);
                                else
                                    line += FormatMDLInt(0, 3);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // null bond order, query bond order - who knows.. but
                        line += FormatMDLInt(0, 3);
                    }
                }
                line += "  0  0  0";

                if (container.Atoms[f].GetProperty<object>(CDKPropertyName.AtomAtomMapping) != null)
                {
                    object atomAtomMapping = container.Atoms[f].GetProperty<object>(CDKPropertyName.AtomAtomMapping);
                    if (atomAtomMapping is string)
                    {
                        try
                        {
                            int value = int.Parse((string)atomAtomMapping);
                            line += FormatMDLInt(value, 3);
                        }
                        catch (FormatException)
                        {
                            line += FormatMDLInt(0, 3);
                            Trace.TraceWarning("Skipping atom-atom mapping, invalid value: " + atomAtomMapping);
                        }
                    }
                    else if (atomAtomMapping is int)
                    {
                        int value = (int)atomAtomMapping;
                        line += FormatMDLInt(value, 3);
                    }
                    else
                    {
                        line += FormatMDLInt(0, 3);
                    }
                }
                else
                {
                    line += FormatMDLInt(0, 3);
                }
                line += "  0  0";
                writer.Write(line);
                writer.Write('\n');
            }

            // write Bond block
            foreach (var bond in container.Bonds)
            {
                if (bond.Atoms.Count != 2)
                {
                    Trace.TraceWarning("Skipping bond with more/less than two atoms: " + bond);
                }
                else
                {
                    if (bond.Stereo == BondStereo.UpInverted || bond.Stereo == BondStereo.DownInverted
                        || bond.Stereo == BondStereo.UpOrDownInverted)
                    {
                        // turn around atom coding to correct for inv stereo
                        line = FormatMDLInt(atomindex[bond.End] + 1, 3);
                        line += FormatMDLInt(atomindex[bond.Begin] + 1, 3);
                    }
                    else
                    {
                        line = FormatMDLInt(atomindex[bond.Begin] + 1, 3);
                        line += FormatMDLInt(atomindex[bond.End] + 1, 3);
                    }
                    int bondType = 0;

                    if (bond is CTFileQueryBond)
                    {
                        // Could do ordinal()-1 but this is clearer
                        switch (((CTFileQueryBond)bond).Type)
                        {
                            case CTFileQueryBond.BondTypes.Single:
                                bondType = 1;
                                break;
                            case CTFileQueryBond.BondTypes.Double:
                                bondType = 2;
                                break;
                            case CTFileQueryBond.BondTypes.Triple:
                                bondType = 3;
                                break;
                            case CTFileQueryBond.BondTypes.Aromatic:
                                bondType = 4;
                                break;
                            case CTFileQueryBond.BondTypes.SingleOrDouble:
                                bondType = 5;
                                break;
                            case CTFileQueryBond.BondTypes.SingleOrAromatic:
                                bondType = 6;
                                break;
                            case CTFileQueryBond.BondTypes.DoubleOrAromatic:
                                bondType = 7;
                                break;
                            case CTFileQueryBond.BondTypes.Any:
                                bondType = 8;
                                break;
                        }
                    }
                    else
                    {
                        switch (bond.Order.Ordinal)
                        {
                            case BondOrder.O.Single:
                            case BondOrder.O.Double:
                            case BondOrder.O.Triple:
                                if (WriteAromaticBondTypes.IsSet && bond.IsAromatic)
                                    bondType = 4;
                                else
                                    bondType = bond.Order.Numeric;
                                break;
                            case BondOrder.O.Unset:
                                if (bond.IsAromatic)
                                {
                                    if (!WriteAromaticBondTypes.IsSet)
                                        throw new CDKException("Bond at idx " + container.Bonds.IndexOf(bond) + " was an unspecific aromatic bond which should only be used for querie in Molfiles. These can be written if desired by enabling the option 'WriteAromaticBondTypes'.");
                                    bondType = 4;
                                }
                                break;
                        }
                    }

                    if (bondType == 0)
                        throw new CDKException("Bond at idx=" + container.Bonds.IndexOf(bond) + " is not supported by Molfile, bond=" + bond.Order);

                    line += FormatMDLInt(bondType, 3);
                    line += "  ";
                    switch (bond.Stereo.Ordinal)
                    {
                        case BondStereo.O.Up:
                        case BondStereo.O.UpInverted:
                            line += "1";
                            break;
                        case BondStereo.O.Down:
                        case BondStereo.O.DownInverted:
                            line += "6";
                            break;
                        case BondStereo.O.UpOrDown:
                        case BondStereo.O.UpOrDownInverted:
                            line += "4";
                            break;
                        case BondStereo.O.EOrZ:
                            line += "3";
                            break;
                        default:
                            line += "0";
                            break;
                    }
                    line += "  0  0  0 ";
                    writer.Write(line);
                    writer.Write('\n');
                }
            }

            // Write Atom Value
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                IAtom atom = container.Atoms[i];
                if (atom.GetProperty<object>(CDKPropertyName.Comment) != null
                    && atom.GetProperty<object>(CDKPropertyName.Comment) is string
                    && !atom.GetProperty<string>(CDKPropertyName.Comment).Trim().Equals(""))
                {
                    writer.Write("V  ");
                    writer.Write(FormatMDLInt(i + 1, 3));
                    writer.Write(" ");
                    writer.Write(atom.GetProperty<string>(CDKPropertyName.Comment));
                    writer.Write('\n');
                }
            }

            // write formal atomic charges
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                IAtom atom = container.Atoms[i];
                int? charge = atom.FormalCharge;
                if (charge != null && charge != 0)
                {
                    writer.Write("M  CHG  1 ");
                    writer.Write(FormatMDLInt(i + 1, 3));
                    writer.Write(" ");
                    writer.Write(FormatMDLInt(charge.Value, 3));
                    writer.Write('\n');
                }
            }

            // write radical information
            if (container.SingleElectrons.Count > 0)
            {
                IDictionary<int, SpinMultiplicity> atomIndexSpinMap = new SortedDictionary<int, SpinMultiplicity>();
                for (int i = 0; i < container.Atoms.Count; i++)
                {
                    int eCount = container.GetConnectedSingleElectrons(container.Atoms[i]).Count();
                    switch (eCount)
                    {
                        case 0:
                            continue;
                        case 1:
                            atomIndexSpinMap[i] = SpinMultiplicity.Monovalent;
                            break;
                        case 2:
                            // information loss, divalent but singlet or triplet?
                            atomIndexSpinMap[i] = SpinMultiplicity.DivalentSinglet;
                            break;
                        default:
                            Debug.WriteLine("Invalid number of radicals found: " + eCount);
                            break;
                    }
                }
                IEnumerator<KeyValuePair<int, SpinMultiplicity>> iterator = atomIndexSpinMap.GetEnumerator();
                for (int i = 0; i < atomIndexSpinMap.Count; i += NN8)
                {
                    if (atomIndexSpinMap.Count - i <= NN8)
                    {
                        writer.Write("M  RAD" + FormatMDLInt(atomIndexSpinMap.Count - i, WIDTH));
                        iterator.MoveNext();
                        WriteRadicalPattern(iterator, 0);
                    }
                    else
                    {
                        writer.Write("M  RAD" + FormatMDLInt(NN8, WIDTH));
                        iterator.MoveNext();
                        WriteRadicalPattern(iterator, 0);
                    }
                    writer.Write('\n');
                }
            }

            // write formal isotope information
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                IAtom atom = container.Atoms[i];
                if (!(atom is IPseudoAtom))
                {
                    int? atomicMass = atom.MassNumber;
                    if (atomicMass != null)
                    {
                        int majorMass = Isotopes.Instance.GetMajorIsotope(atom.Symbol).MassNumber.Value;
                        if (atomicMass != majorMass)
                        {
                            writer.Write("M  ISO  1 ");
                            writer.Write(FormatMDLInt(i + 1, 3));
                            writer.Write(" ");
                            writer.Write(FormatMDLInt(atomicMass.Value, 3));
                            writer.Write('\n');
                        }
                    }
                }
            }

            //write RGP line (max occurrence is 16 data points per line)
            if (rgroups != null)
            {
                StringBuilder rgpLine = new StringBuilder();
                int cnt = 0;

                // the order isn't guarantied but as we index with the atom
                // number this isn't an issue
                foreach (var e in rgroups)
                {
                    rgpLine.Append(FormatMDLInt(e.Key, 4));
                    rgpLine.Append(FormatMDLInt(e.Value, 4));
                    cnt++;
                    if (cnt == 8)
                    {
                        rgpLine.Insert(0, "M  RGP" + FormatMDLInt(cnt, 3));
                        writer.Write(rgpLine.ToString());
                        writer.Write('\n');
                        rgpLine = new StringBuilder();
                        cnt = 0;
                    }
                }
                if (cnt != 0)
                {
                    rgpLine.Insert(0, "M  RGP" + FormatMDLInt(cnt, 3));
                    writer.Write(rgpLine.ToString());
                    writer.Write('\n');
                }

            }

            // write atom aliases
            if (aliases != null)
            {

                foreach (var e in aliases)
                {
                    writer.Write("A" + FormatMDLInt(e.Key, 5));
                    writer.Write('\n');

                    string label = e.Value;

                    // fixed width file - doubtful someone would have a label > 70 but trim if they do
                    if (label.Length > 70) label = label.Substring(0, 70);

                    writer.Write(label);
                    writer.Write('\n');

                }
            }

            WriteSgroups(container, writer, atomindex);

            // close molecule
            writer.Write("M  END");
            writer.Write('\n');
            writer.Flush();
        }

        private void WriteSgroups(IAtomContainer container, TextWriter writer, IDictionary<IAtom, int> atomidxs)
        {
            IList<Sgroup> sgroups = container.GetProperty<IList<Sgroup>>(CDKPropertyName.CtabSgroups);
            if (sgroups == null)
                return;

            // going to modify
            sgroups = new List<Sgroup>(sgroups);

            // remove non-ctab Sgroups 
            {
                var removes = new List<Sgroup>();
                foreach (var e in sgroups.Where(n => n.Type == SgroupType.ExtMulticenter))
                    removes.Add(e);
                foreach (var e in removes)
                    sgroups.Remove(e);
            }

            foreach (var wrapSgroups in Wrap(sgroups, 8))
            {
                // Declare the SGroup type
                writer.Write("M  STY");
                writer.Write(FormatMDLInt(wrapSgroups.Count, 3));
                foreach (var sgroup in wrapSgroups)
                {
                    writer.Write(' ');
                    writer.Write(FormatMDLInt(1 + sgroups.IndexOf(sgroup), 3));
                    writer.Write(' ');
                    writer.Write(sgroup.Type.Key);
                }
                writer.Write('\n');
            }

            // Sgroup output is non-compact for now - but valid
            for (int id = 1; id <= sgroups.Count; id++)
            {
                Sgroup sgroup = sgroups[id - 1];

                // Sgroup Atom List
                foreach (var atoms in Wrap(sgroup.Atoms, 15))
                {
                    writer.Write("M  SAL ");
                    writer.Write(FormatMDLInt(id, 3));
                    writer.Write(FormatMDLInt(atoms.Count, 3));
                    foreach (var atom in atoms)
                    {
                        writer.Write(' ');
                        writer.Write(FormatMDLInt(1 + atomidxs[atom], 3));
                    }
                    writer.Write('\n');
                }

                // Sgroup Bond List
                foreach (var bonds in Wrap(sgroup.Bonds, 15))
                {
                    writer.Write("M  SBL ");
                    writer.Write(FormatMDLInt(id, 3));
                    writer.Write(FormatMDLInt(bonds.Count, 3));
                    foreach (var bond in bonds)
                    {
                        writer.Write(' ');
                        writer.Write(FormatMDLInt(1 + container.Bonds.IndexOf(bond), 3));
                    }
                    writer.Write('\n');
                }

                // Sgroup Parent List
                foreach (var parents in Wrap(sgroup.Parents.ToList(), 8))
                {
                    writer.Write("M  SPL");
                    writer.Write(FormatMDLInt(parents.Count, 3));
                    foreach (var parent in parents)
                    {
                        writer.Write(' ');
                        writer.Write(FormatMDLInt(id, 3));
                        writer.Write(' ');
                        writer.Write(FormatMDLInt(1 + sgroups.IndexOf(parent), 3));
                    }
                    writer.Write('\n');
                }

                ICollection<SgroupKeys> attributeKeys = sgroup.AttributeKeys;
                // TODO order and aggregate attribute keys
                foreach (var key in attributeKeys)
                {
                    switch (key)
                    {
                        case SgroupKeys.CtabSubScript:
                            writer.Write("M  SMT ");
                            writer.Write(FormatMDLInt(id, 3));
                            writer.Write(' ');
                            writer.Write((string)sgroup.GetValue(key));
                            writer.Write('\n');
                            break;
                        case SgroupKeys.CtabExpansion:
                            bool expanded = (bool)sgroup.GetValue(key);
                            if (expanded)
                            {
                                writer.Write("M  SDS EXP");
                                writer.Write(FormatMDLInt(1, 3));
                                writer.Write(' ');
                                writer.Write(FormatMDLInt(id, 3));
                                writer.Write('\n');
                            }
                            break;
                        case SgroupKeys.CtabBracket:
                            IEnumerable<SgroupBracket> brackets = (IEnumerable<SgroupBracket>)sgroup.GetValue(key);
                            foreach (var bracket in brackets)
                            {
                                writer.Write("M  SDI ");
                                writer.Write(FormatMDLInt(id, 3));
                                writer.Write(FormatMDLInt(4, 3));
                                writer.Write(FormatMDLFloat(bracket.FirstPoint.X));
                                writer.Write(FormatMDLFloat(bracket.FirstPoint.Y));
                                writer.Write(FormatMDLFloat(bracket.SecondPoint.X));
                                writer.Write(FormatMDLFloat(bracket.SecondPoint.Y));
                                writer.Write('\n');
                            }
                            break;
                        case SgroupKeys.CtabBracketStyle:
                            writer.Write("M  SBT");
                            writer.Write(FormatMDLInt(1, 3));
                            writer.Write(' ');
                            writer.Write(FormatMDLInt(id, 3));
                            writer.Write(' ');
                            writer.Write(FormatMDLInt((int)sgroup.GetValue(key), 3));
                            writer.Write('\n');
                            break;
                        case SgroupKeys.CtabConnectivity:
                            writer.Write("M  SCN");
                            writer.Write(FormatMDLInt(1, 3));
                            writer.Write(' ');
                            writer.Write(FormatMDLInt(id, 3));
                            writer.Write(' ');
                            writer.Write(((string)sgroup.GetValue(key)).ToUpperInvariant());
                            writer.Write('\n');
                            break;
                        case SgroupKeys.CtabSubType:
                            writer.Write("M  SST");
                            writer.Write(FormatMDLInt(1, 3));
                            writer.Write(' ');
                            writer.Write(FormatMDLInt(id, 3));
                            writer.Write(' ');
                            writer.Write((string)sgroup.GetValue(key));
                            writer.Write('\n');
                            break;
                        case SgroupKeys.CtabParentAtomList:
                            IEnumerable<IAtom> parentAtomList = (IEnumerable<IAtom>)sgroup.GetValue(key);
                            foreach (var atoms in Wrap(parentAtomList.ToList(), 15))
                            {
                                writer.Write("M  SPA ");
                                writer.Write(FormatMDLInt(id, 3));
                                writer.Write(FormatMDLInt(atoms.Count, 3));
                                foreach (var atom in atoms)
                                {
                                    writer.Write(' ');
                                    writer.Write(FormatMDLInt(1 + atomidxs[atom], 3));
                                }
                                writer.Write('\n');
                            }
                            break;
                        case SgroupKeys.CtabComponentNumber:
                            int compNumber = (int)sgroup.GetValue(key);
                            writer.Write("M  SNC");
                            writer.Write(FormatMDLInt(1, 3));
                            writer.Write(' ');
                            writer.Write(FormatMDLInt(id, 3));
                            writer.Write(' ');
                            writer.Write(FormatMDLInt(compNumber, 3));
                            writer.Write('\n');
                            break;
                    }
                }
            }
        }

        private IList<IList<T>> Wrap<T>(ICollection<T> set, int lim)
        {
            IList<IList<T>> wrapped = new List<IList<T>>();
            List<T> list = new List<T>(set);
            if (set.Count <= lim)
            {
                if (list.Count != 0)
                    wrapped.Add(list);
            }
            else
            {
                int i = 0;
                for (; (i + lim) < set.Count; i += lim)
                {
                    wrapped.Add(list.GetRange(i, lim));
                }
                wrapped.Add(list.GetRange(i, list.Count - i));
            }
            return wrapped;
        }

        private int GetNumberOfDimensions(IAtomContainer mol)
        {
            foreach (IAtom atom in mol.Atoms)
            {
                if (atom.Point3D != null && !ForceWriteAs2DCoords.IsSet)
                    return 3;
                else if (atom.Point2D != null)
                    return 2;
            }
            return 0;
        }

        private void WriteRadicalPattern(IEnumerator<KeyValuePair<int, SpinMultiplicity>> iterator, int i)
        {

            KeyValuePair<int, SpinMultiplicity> entry = iterator.Current;
            writer.Write(" ");
            writer.Write(FormatMDLInt(entry.Key + 1, WIDTH));
            writer.Write(" ");
            writer.Write(FormatMDLInt(entry.Value.Value, WIDTH));

            i = i + 1;
            if (i < NN8 && iterator.MoveNext()) WriteRadicalPattern(iterator, i);
        }

        /// <summary>
        /// Formats an integer to fit into the connection table and changes it
        /// to a string.
        /// </summary>
        /// <param name="i">The int to be formated</param>
        /// <param name="l">Length of the string</param>
        /// <returns>The string to be written into the connectiontable</returns>
        protected internal static string FormatMDLInt(int i, int l)
        {
            string s = i.ToString(CultureInfo.InvariantCulture);
            return new string(' ', l - s.Length) + s;
        }

        /// <summary>
        /// Formats a float to fit into the connectiontable and changes it
        /// to a string.
        /// </summary>
        /// <param name="fl">The float to be formated</param>
        /// <returns>The string to be written into the connectiontable</returns>
        protected static string FormatMDLFloat(double fl)
        {
            string s;
            if (double.IsNaN(fl) || double.IsInfinity(fl))
                s = "0.0000";
            else
                s = fl.ToString("F4", CultureInfo.InvariantCulture);
            return s.PadLeft(10);
        }

        /// <summary>
        /// Formats a string to fit into the connectiontable.
        /// </summary>
        /// <param name="s">The string to be formated</param>
        /// <param name="le">The length of the string</param>
        /// <returns>The string to be written in the connectiontable</returns>
        protected static string FormatMDLString(string s, int le)
        {
            return (s + new string(' ', le)).Substring(0, le);
        }

        /// <summary>
        /// Initializes IO settings.
        /// <para>
        /// Please note with regards to "WriteAromaticBondTypes": bond type values 4 through 8 are for SSS queries only,
        /// so a 'query file' is created if the container has aromatic bonds and this settings is true.
        /// </para>
        /// </summary>
        private void InitIOSettings()
        {
            ForceWriteAs2DCoords = IOSettings.Add(new BooleanIOSetting("ForceWriteAs2DCoordinates", IOSetting.Importance.Low,
                                                                   "Should coordinates always be written as 2D?", "false"));
            WriteAromaticBondTypes = IOSettings.Add(new BooleanIOSetting("WriteAromaticBondTypes", IOSetting.Importance.Low,
                                                                     "Should aromatic bonds be written as bond type 4?", "false"));
            WriteQueryFormatValencies = IOSettings.Add(new BooleanIOSetting("WriteQueryFormatValencies",
                                                                        IOSetting.Importance.Low, "Should valencies be written in the MDL Query format? (deprecated)", "false"));
        }

        /// <summary>
        /// Convenience method to set the option for writing aromatic bond types.
        /// </summary>
        /// <param name="val">the value.</param>
        public void SetWriteAromaticBondTypes(bool val)
        {
            try
            {
                WriteAromaticBondTypes.Setting = val.ToString();
            }
            catch (CDKException)
            {
                // ignored can't happen since we are statically typed here
            }
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
