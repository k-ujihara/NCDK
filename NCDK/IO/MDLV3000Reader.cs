/* Copyright (C) 2006-2008  Egon Willighagen <egonw@sci.kun.nl>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Common.Primitives;
using NCDK.IO.Formats;
using NCDK.Isomorphisms.Matchers;
using NCDK.Numerics;
using NCDK.Sgroups;
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
    /// Class that implements the MDL mol V3000 format. This reader reads the
    /// element symbol and 2D or 3D coordinates from the ATOM block.
    /// </summary>
    // @cdk.module io
    // @cdk.iooptions
    // @author      Egon Willighagen <egonw@users.sf.net>
    // @cdk.created 2006
    // @cdk.keyword MDL molfile V3000
    public class MDLV3000Reader : DefaultChemObjectReader
    {
        TextReader input = null;

        private readonly Regex keyValueTuple;
        private readonly Regex keyValueTuple2;

        private int lineNumber;

        public MDLV3000Reader(TextReader ins)
                : this(ins, ChemObjectReaderMode.Relaxed)
        { }

        public MDLV3000Reader(TextReader ins, ChemObjectReaderMode mode)
        {
            input = ins;
            InitIOSettings();
            base.ReaderMode = mode;
            /* compile patterns */
            keyValueTuple = new Regex("\\s*(\\w+)=([^\\s]*)(.*)", RegexOptions.Compiled); // e.g. CHG=-1
            keyValueTuple2 = new Regex("\\s*(\\w+)=\\(([^\\)]*)\\)(.*)", RegexOptions.Compiled); // e.g. ATOMS=(1 31)
            lineNumber = 0;
        }

        public MDLV3000Reader(Stream input)
            : this(input, ChemObjectReaderMode.Relaxed)
        { }

        public MDLV3000Reader(Stream input, ChemObjectReaderMode mode)
            : this(new StreamReader(input), mode)
        { }

        public override IResourceFormat Format => MDLV3000Format.Instance;

        public override bool Accepts(Type type)
        {
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            return false;
        }

        public override T Read<T>(T obj) 
        {
            if (obj is IAtomContainer)
            {
                return (T)ReadMolecule(obj.Builder);
            }
            throw new CDKException("Only supports AtomContainer objects.");
        }

        public IAtomContainer ReadMolecule(IChemObjectBuilder builder)
        {
            return ReadConnectionTable(builder);
        }

        public IAtomContainer ReadConnectionTable(IChemObjectBuilder builder)
        {
            Trace.TraceInformation("Reading CTAB block");
            var readData = builder.NewAtomContainer();
            bool foundEND = false;
            var lastLine = ReadHeader(readData);
            while (!foundEND)
            {
                string command = ReadCommand(lastLine);
                Debug.WriteLine($"command found: {command}");
                switch (command)
                {
                    case "END CTAB":
                        foundEND = true;
                        break;
                    case "BEGIN CTAB":
                        // that's fine
                        break;
                    case "COUNTS":
                        // don't think I need to parse this
                        break;
                    case "BEGIN ATOM":
                        ReadAtomBlock(readData);
                        break;
                    case "BEGIN BOND":
                        ReadBondBlock(readData);
                        break;
                    case "BEGIN SGROUP":
                        ReadSgroup(readData);
                        break;
                    default:
                        Trace.TraceWarning("Unrecognized command: " + command);
                        break;
                }
                lastLine = ReadLine();
            }

            foreach (var atom in readData.Atoms)
            {
                // XXX: slow method is slow
                int valence = 0;
                foreach (var bond in readData.GetConnectedBonds(atom))
                {
                    if (bond is IQueryBond || bond.Order == BondOrder.Unset)
                    {
                        valence = -1;
                        break;
                    }
                    else
                    {
                        valence += bond.Order.Numeric();
                    }
                }
                if (valence < 0)
                {
                    Trace.TraceWarning("Cannot set valence for atom with query bonds"); // also counts aromatic bond as query
                }
                else
                {
                    int unpaired = readData.GetConnectedSingleElectrons(atom).Count();
                    ApplyMDLValenceModel(atom, valence + unpaired, unpaired);
                }
            }

            return readData;
        }

        /// <summary>
        /// </summary>
        /// <returns>Last line read</returns>
        /// <exception cref="CDKException">when no file content is detected</exception>
        public string ReadHeader(IAtomContainer readData)
        {
            // read four lines
            var line1 = ReadLine();
            if (line1 == null)
            {
                throw new CDKException("Expected a header line, but found nothing.");
            }
            if (line1.Length > 0)
            {
                if (line1.StartsWith("M  V30", StringComparison.Ordinal))
                {
                    // no header
                    return line1;
                }
                readData.Title = line1;
            }
            ReadLine();
            var line3 = ReadLine();
            if (line3.Length > 0)
                readData.SetProperty(CDKPropertyName.Comment, line3);
            var line4 = ReadLine();
            if (!line4.ContainsOrdinal("3000"))
            {
                throw new CDKException("This file is not a MDL V3000 molfile.");
            }
            return ReadLine();
        }

        /// <summary>
        /// Reads the atoms, coordinates and charges.
        /// <para>IMPORTANT: it does not support the atom list and its negation!</para>
        /// </summary>
        public void ReadAtomBlock(IAtomContainer readData)
        {
            Trace.TraceInformation("Reading ATOM block");
            var isotopeFactory = CDK.IsotopeFactory;

            int RGroupCounter = 1;
            int Rnumber;
            string[] rGroup = null;

            bool foundEND = false;
            while (!foundEND)
            {
                string command = ReadCommand(ReadLine());
                if (string.Equals("END ATOM", command, StringComparison.Ordinal))
                {
                    // FIXME: should check whether 3D is really 2D
                    foundEND = true;
                }
                else
                {
                    Debug.WriteLine($"Parsing atom from: {command}");
                    var atom = readData.Builder.NewAtom();
                    var tokenizer = Strings.Tokenize(command).GetEnumerator();
                    // parse the index
                    try
                    {
                        tokenizer.MoveNext();
                        atom.Id = tokenizer.Current;
                    }
                    catch (Exception exception)
                    {
                        var error = "Error while parsing atom index";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }
                    // parse the element
                    tokenizer.MoveNext();
                    string element = tokenizer.Current;
                    if (isotopeFactory.IsElement(element))
                    {
                        atom.Symbol = element;
                        isotopeFactory.Configure(atom); // ?
                    }
                    else if (string.Equals("A", element, StringComparison.Ordinal))
                    {
                        atom = readData.Builder.NewPseudoAtom(element);
                    }
                    else if (string.Equals("Q", element, StringComparison.Ordinal))
                    {
                        atom = readData.Builder.NewPseudoAtom(element);
                    }
                    else if (string.Equals("*", element, StringComparison.Ordinal))
                    {
                        atom = readData.Builder.NewPseudoAtom(element);
                    }
                    else if (string.Equals("LP", element, StringComparison.Ordinal))
                    {
                        atom = readData.Builder.NewPseudoAtom(element);
                    }
                    else if (string.Equals("L", element, StringComparison.Ordinal))
                    {
                        atom = readData.Builder.NewPseudoAtom(element);
                    }
                    else if (element.Length > 0 && element[0] == 'R')
                    {
                        Debug.WriteLine($"Atom {element} is not an regular element. Creating a PseudoAtom.");
                        //check if the element is R
                        try
                        {
                            Rnumber = int.Parse(rGroup[(rGroup.Length - 1)], NumberFormatInfo.InvariantInfo);
                            RGroupCounter = Rnumber;
                        }
                        catch (Exception)
                        {
                            Rnumber = RGroupCounter;
                            RGroupCounter++;
                        }
                        element = "R" + Rnumber;
                        atom = readData.Builder.NewPseudoAtom(element);
                    }
                    else
                    {
                        if (ReaderMode == ChemObjectReaderMode.Strict)
                        {
                            throw new CDKException("Invalid element type. Must be an existing element, or one in: A, Q, L, LP, *.");
                        }
                        atom = readData.Builder.NewPseudoAtom(element);
                        atom.Symbol = element;
                    }

                    // parse atom coordinates (in Angstrom)
                    try
                    {
                        tokenizer.MoveNext();
                        var xString = tokenizer.Current;
                        tokenizer.MoveNext();
                        var yString = tokenizer.Current;
                        tokenizer.MoveNext();
                        var zString = tokenizer.Current;
                        var x = double.Parse(xString, NumberFormatInfo.InvariantInfo);
                        var y = double.Parse(yString, NumberFormatInfo.InvariantInfo);
                        var z = double.Parse(zString, NumberFormatInfo.InvariantInfo);
                        atom.Point3D = new Vector3(x, y, z);
                        atom.Point2D = new Vector2(x, y); // FIXME: dirty!
                    }
                    catch (Exception exception)
                    {
                        string error = "Error while parsing atom coordinates";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }
                    // atom-atom mapping
                    tokenizer.MoveNext();
                    var mapping = tokenizer.Current;
                    if (!string.Equals(mapping, "0", StringComparison.Ordinal))
                    {
                        Trace.TraceWarning("Skipping atom-atom mapping: " + mapping);
                    } // else: default 0 is no mapping defined

                    // the rest are key value things
                    if (command.IndexOf('=') != -1)
                    {
                        var options = ParseOptions(ExhaustStringTokenizer(tokenizer));
                        var keys = options.Keys;
                        foreach (var key in keys)
                        {
                            string value = options[key];
                            try
                            {
                                switch (key)
                                {
                                    case "CHG":
                                        int charge = int.Parse(value, NumberFormatInfo.InvariantInfo);
                                        if (charge != 0)
                                        { // zero is no charge specified
                                            atom.FormalCharge = charge;
                                        }
                                        break;
                                    case "RAD":
                                        MDLV2000Writer.SpinMultiplicity spinMultiplicity = MDLV2000Writer.SpinMultiplicity.OfValue(int.Parse(value, NumberFormatInfo.InvariantInfo));
                                        int numElectons = spinMultiplicity.SingleElectrons;
                                        atom.SetProperty(CDKPropertyName.SpinMultiplicity, spinMultiplicity);
                                        while (numElectons-- > 0)
                                        {
                                            readData.SingleElectrons.Add(readData.Builder.NewSingleElectron(atom));
                                        }
                                        break;
                                    case "VAL":
                                        if (!(atom is IPseudoAtom))
                                        {
                                            try
                                            {
                                                int valence = int.Parse(value, NumberFormatInfo.InvariantInfo);
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
                                                HandleError("Could not parse valence information field", lineNumber, 0, 0, exception);
                                            }
                                        }
                                        else
                                        {
                                            Trace.TraceError("Cannot set valence information for a non-element!");
                                        }
                                        break;
                                    default:
                                        Trace.TraceWarning("Not parsing key: " + key);
                                        break;
                                }
                            }
                            catch (Exception exception)
                            {
                                string error = "Error while parsing key/value " + key + "=" + value + ": "
                                        + exception.Message;
                                Trace.TraceError(error);
                                Debug.WriteLine(exception);
                                throw new CDKException(error, exception);
                            }
                        }
                    }

                    // store atom
                    readData.Atoms.Add(atom);
                    Debug.WriteLine($"Added atom: {atom}");
                }
            }
        }

        /// <summary>
        /// Reads the bond atoms, order and stereo configuration.
        /// </summary>
        public void ReadBondBlock(IAtomContainer readData)
        {
            Trace.TraceInformation("Reading BOND block");
            bool foundEND = false;
            while (!foundEND)
            {
                var command = ReadCommand(ReadLine());
                if (string.Equals("END BOND", command, StringComparison.Ordinal))
                {
                    foundEND = true;
                }
                else
                {
                    Debug.WriteLine($"Parsing bond from: {command}");
                    var tokenizer = Strings.Tokenize(command).GetEnumerator();
                    var bond = readData.Builder.NewBond();
                    // parse the index
                    try
                    {
                        tokenizer.MoveNext();
                        var indexString = tokenizer.Current;
                        bond.Id = indexString;
                    }
                    catch (Exception exception)
                    {
                        var error = "Error while parsing bond index";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }
                    // parse the order
                    try
                    {
                        tokenizer.MoveNext();
                        var orderString = tokenizer.Current;
                        var order = int.Parse(orderString, NumberFormatInfo.InvariantInfo);
                        if (order >= 4)
                        {
                            bond.Order = BondOrder.Unset;
                            Trace.TraceWarning("Query order types are not supported (yet). File a bug if you need it");
                        }
                        else
                        {
                            bond.Order = BondManipulator.CreateBondOrder((double)order);
                        }
                    }
                    catch (Exception exception)
                    {
                        var error = "Error while parsing bond index";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }
                    // parse index atom 1
                    try
                    {
                        tokenizer.MoveNext();
                        var indexAtom1String = tokenizer.Current;
                        var indexAtom1 = int.Parse(indexAtom1String, NumberFormatInfo.InvariantInfo);
                        var atom1 = readData.Atoms[indexAtom1 - 1];
                        bond.Atoms.Add(atom1);  // bond.Atoms[0]
                    }
                    catch (Exception exception)
                    {
                        var error = "Error while parsing index atom 1 in bond";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }
                    // parse index atom 2
                    try
                    {
                        tokenizer.MoveNext();
                        var indexAtom2String = tokenizer.Current;
                        var indexAtom2 = int.Parse(indexAtom2String, NumberFormatInfo.InvariantInfo);
                        var atom2 = readData.Atoms[indexAtom2 - 1];
                        bond.Atoms.Add(atom2); // bond.Atoms[1]
                    }
                    catch (Exception exception)
                    {
                        var error = "Error while parsing index atom 2 in bond";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }

                    var endpts = new List<IAtom>();
                    string attach = null;

                    // the rest are key=value fields
                    if (command.IndexOf('=') != -1)
                    {
                        var options = ParseOptions(ExhaustStringTokenizer(tokenizer));
                        foreach (var key in options.Keys)
                        {
                            var value = options[key];
                            try
                            {
                                switch (key)
                                {
                                    case "CFG":
                                        var configuration = int.Parse(value, NumberFormatInfo.InvariantInfo);
                                        switch (configuration)
                                        {
                                            case 0:
                                                bond.Stereo = BondStereo.None;
                                                break;
                                            case 1:
                                                bond.Stereo = BondStereo.Up;
                                                break;
                                            case 2:
                                                bond.Stereo = BondStereo.UpOrDown;
                                                break;
                                            case 3:
                                                bond.Stereo = BondStereo.Down;
                                                break;
                                        }
                                        break;
                                    case "ENDPTS":
                                        var endptStr = value.Split(' ');
                                        // skip first value that is count
                                        for (int i = 1; i < endptStr.Length; i++)
                                        {
                                            endpts.Add(readData.Atoms[int.Parse(endptStr[i], NumberFormatInfo.InvariantInfo) - 1]);
                                        }
                                        break;
                                    case "ATTACH":
                                        attach = value;
                                        break;
                                    default:
                                        Trace.TraceWarning("Not parsing key: " + key);
                                        break;
                                }
                            }
                            catch (Exception exception)
                            {
                                var error = $"Error while parsing key/value {key}={value}: {exception.Message}";
                                Trace.TraceError(error);
                                Debug.WriteLine(exception);
                                throw new CDKException(error, exception);
                            }
                        }
                    }

                    // storing bond
                    readData.Bonds.Add(bond);

                    // storing positional variation
                    if (string.Equals("ANY", attach, StringComparison.Ordinal))
                    {
                        var sgroup = new Sgroup { Type = SgroupType.ExtMulticenter };
                        sgroup.Atoms.Add(bond.Begin); // could be other end?
                        sgroup.Bonds.Add(bond);
                        foreach (var endpt in endpts)
                            sgroup.Atoms.Add(endpt);

                        var sgroups = readData.GetCtabSgroups();
                        if (sgroups == null)
                            readData.SetCtabSgroups(sgroups = new List<Sgroup>(4));
                        sgroups.Add(sgroup);
                    }

                    Debug.WriteLine($"Added bond: {bond}");
                }
            }
        }

        /// <summary>
        /// Reads labels.
        /// </summary>
        public void ReadSgroup(IAtomContainer readData)
        {
            bool foundEND = false;
            while (!foundEND)
            {
                var command = ReadCommand(ReadLine());
                if (string.Equals("END SGROUP", command, StringComparison.Ordinal))
                {
                    foundEND = true;
                }
                else
                {
                    Debug.WriteLine($"Parsing Sgroup line: {command}");
                    var tokenizer = Strings.Tokenize(command).GetEnumerator();
                    // parse the index
                    tokenizer.MoveNext();
                    var indexString = tokenizer.Current;
                    Trace.TraceWarning("Skipping external index: " + indexString);
                    // parse command type
                    tokenizer.MoveNext();
                    var type = tokenizer.Current;
                    // parse the external index
                    tokenizer.MoveNext();
                    var externalIndexString = tokenizer.Current;
                    Trace.TraceWarning("Skipping external index: " + externalIndexString);

                    // the rest are key=value fields
                    var options = new Dictionary<string, string>();
                    if (command.IndexOf('=') != -1)
                    {
                        options = ParseOptions(ExhaustStringTokenizer(tokenizer));
                    }

                    var sgroup = new Sgroup();
                    // now interpret line
                    if (type.StartsWith("SUP", StringComparison.Ordinal))
                    {
                        sgroup.Type = SgroupType.CtabAbbreviation;
                        var keys = options.Keys;
                        var label = "";
                        foreach (var key in keys)
                        {
                            var value = options[key];
                            try
                            {
                                switch (key)
                                {
                                    case "ATOMS":
                                        {
                                            var atomsTokenizer = Strings.Tokenize(value).GetEnumerator();
                                            atomsTokenizer.MoveNext();
                                            var nExpected = int.Parse(atomsTokenizer.Current, NumberFormatInfo.InvariantInfo);
                                            while (atomsTokenizer.MoveNext())
                                            {
                                                sgroup.Atoms.Add(readData.Atoms[int.Parse(atomsTokenizer.Current, NumberFormatInfo.InvariantInfo) - 1]);
                                            }
                                        }
                                        break;
                                    case "XBONDS":
                                        {
                                            var xbonds = Strings.Tokenize(value).GetEnumerator();
                                            xbonds.MoveNext();
                                            var nExpected = int.Parse(xbonds.Current, NumberFormatInfo.InvariantInfo);
                                            while (xbonds.MoveNext())
                                            {
                                                sgroup.Bonds.Add(readData.Bonds[int.Parse(xbonds.Current, NumberFormatInfo.InvariantInfo) - 1]);
                                            }
                                        }
                                        break;
                                    case "LABEL":
                                        {
                                            label = value;
                                        }
                                        break;
                                    default:
                                        Trace.TraceWarning("Not parsing key: " + key);
                                        break;
                                }
                            }
                            catch (Exception exception)
                            {
                                var error = $"Error while parsing key/value {key}={value}: {exception.Message}";
                                Trace.TraceError(error);
                                Debug.WriteLine(exception);
                                throw new CDKException(error, exception);
                            }
                            if (sgroup.Atoms.Any() && label.Length > 0)
                            {
                                sgroup.Subscript = label;
                            }
                        }
                        var sgroups = readData.GetCtabSgroups();
                        if (sgroups == null)
                            sgroups = new List<Sgroup>();
                        sgroups.Add(sgroup);
                        readData.SetCtabSgroups(sgroups);
                    }
                    else
                    {
                        Trace.TraceWarning("Skipping unrecognized SGROUP type: " + type);
                    }
                }
            }
        }

        /// <summary>
        /// Reads the command on this line. If the line is continued on the next, that
        /// part is added.
        /// </summary>
        /// <returns>Returns the command on this line.</returns>
        private string ReadCommand(string line)
        {
            if (line.StartsWith("M  V30 ", StringComparison.Ordinal))
            {
                var command = line.Substring(7);
                if (command.EndsWithChar('-'))
                {
                    command = command.Substring(0, command.Length - 1);
                    command += ReadCommand(ReadLine());
                }
                return command;
            }
            else
            {
                throw new CDKException($"Could not read MDL file: unexpected line: {line}");
            }
        }

        private Dictionary<string, string> ParseOptions(string str)
        {
            var keyValueTuples = new Dictionary<string, string>();
            while (str.Length >= 3)
            {
                Debug.WriteLine($"Matching remaining option string: {str}");
                var tuple1Matcher = keyValueTuple2.Match(str);
                if (tuple1Matcher.Success)
                {
                    var key = tuple1Matcher.Groups[1].Value;
                    var value = tuple1Matcher.Groups[2].Value;
                    str = tuple1Matcher.Groups[3].Value;
                    Debug.WriteLine($"Found key: {key}");
                    Debug.WriteLine($"Found value: {value}");
                    keyValueTuples[key] = value;
                }
                else
                {
                    var tuple2Matcher = keyValueTuple.Match(str);
                    if (tuple2Matcher.Success)
                    {
                        var key = tuple2Matcher.Groups[1].Value;
                        var value = tuple2Matcher.Groups[2].Value;
                        str = tuple2Matcher.Groups[3].Value;
                        Debug.WriteLine($"Found key: {key}");
                        Debug.WriteLine($"Found value: {value}");
                        keyValueTuples[key] = value;
                    }
                    else
                    {
                        Trace.TraceWarning("Quiting; could not parse: " + str + ".");
                        str = "";
                    }
                }
            }
            return keyValueTuples;
        }

        public static string ExhaustStringTokenizer(IEnumerator<string> tokenizer)
        {
            var buffer = new StringBuilder();
            buffer.Append(' ');
            while (tokenizer.MoveNext())
            {
                buffer.Append(tokenizer.Current);
                buffer.Append(' ');
            }
            return buffer.ToString();
        }

        public string ReadLine()
        {
            string line;
            try
            {
                line = input.ReadLine();
                lineNumber++;
                Debug.WriteLine("read line " + lineNumber + ":", line);
            }
            catch (Exception exception)
            {
                var error = $"Unexpected error while reading file: {exception.Message}";
                Trace.TraceError(error);
                Debug.WriteLine(exception);
                throw new CDKException(error, exception);
            }
            return line;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    input.Dispose();
                }

                input = null;

                disposedValue = true;
                base.Dispose(disposing);
            }
        }
        #endregion

        private static void InitIOSettings() { }

        /// <summary>
        /// Applies the MDL valence model to atoms using the explicit valence (bond
        /// order sum) and charge to determine the correct number of implicit
        /// hydrogens. The model is not applied if the explicit valence is less than
        /// 0 - this is the case when a query bond was read for an atom.
        /// </summary>
        /// <param name="atom">the atom to apply the model to</param>
        /// <param name="explicitValence">the explicit valence (bond order sum)</param>
        private static void ApplyMDLValenceModel(IAtom atom, int explicitValence, int unpaired)
        {

            if (atom.Valency != null)
            {
                if (atom.Valency >= explicitValence)
                    atom.ImplicitHydrogenCount = atom.Valency - (explicitValence - unpaired);
                else
                    atom.ImplicitHydrogenCount = 0;
            }
            else
            {
                var element = atom.AtomicNumber;
                var charge = atom.FormalCharge ?? 0;

                var implicitValence = MDLValence.ImplicitValence(element, charge, explicitValence);
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
    }
}
