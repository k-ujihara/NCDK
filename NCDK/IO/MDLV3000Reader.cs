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
using NCDK.Common.Mathematics;
using NCDK.Common.Primitives;
using NCDK.Config;
using NCDK.IO.Formats;
using NCDK.SGroups;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NCDK.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace NCDK.IO
{
    /**
     * Class that implements the MDL mol V3000 format. This reader reads the
     * element symbol and 2D or 3D coordinates from the ATOM block.
     *
     * @cdk.module io
     * @cdk.githash
     * @cdk.iooptions
     *
     * @author      Egon Willighagen <egonw@users.sf.net>
     * @cdk.created 2006
     *
     * @cdk.keyword MDL molfile V3000
     * @cdk.require java1.4+
     */
    public class MDLV3000Reader : DefaultChemObjectReader
    {

        TextReader input = null;

        private Regex keyValueTuple;
        private Regex keyValueTuple2;

        private int lineNumber;

        public MDLV3000Reader(TextReader ins)
                : this(ins, ChemObjectReaderModes.Relaxed)
        { }

        public MDLV3000Reader(TextReader ins, ChemObjectReaderModes mode)
        {
            input = ins;
            InitIOSettings();
            base.mode = mode;
            /* compile patterns */
            keyValueTuple = new Regex("\\s*(\\w+)=([^\\s]*)(.*)", RegexOptions.Compiled); // e.g. CHG=-1
            keyValueTuple2 = new Regex("\\s*(\\w+)=\\(([^\\)]*)\\)(.*)", RegexOptions.Compiled); // e.g. ATOMS=(1 31)
            lineNumber = 0;
        }

        public MDLV3000Reader(Stream input)
            : this(input, ChemObjectReaderModes.Relaxed)
        { }

        public MDLV3000Reader(Stream input, ChemObjectReaderModes mode)
            : this(new StreamReader(input), mode)
        { }

        public MDLV3000Reader()
            : this(new StringReader(""))
        { }

        public override IResourceFormat Format => MDLV3000Format.Instance;

        public override void SetReader(TextReader input)
        {
            this.input = input;
            lineNumber = 0;
        }

        public override void SetReader(Stream input)
        {
            SetReader(new StreamReader(input));
        }

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
            return default(T);
        }

        public IAtomContainer ReadMolecule(IChemObjectBuilder builder)
        {
            return ReadConnectionTable(builder);
        }

        public IAtomContainer ReadConnectionTable(IChemObjectBuilder builder)
        {
            Trace.TraceInformation("Reading CTAB block");
            IAtomContainer readData = builder.CreateAtomContainer();
            bool foundEND = false;
            string lastLine = ReadHeader(readData);
            while (!foundEND)
            {
                string command = ReadCommand(lastLine);
                Debug.WriteLine("command found: " + command);
                if ("END CTAB".Equals(command))
                {
                    foundEND = true;
                }
                else if ("BEGIN CTAB".Equals(command))
                {
                    // that's fine
                }
                else if ("COUNTS".Equals(command))
                {
                    // don't think I need to parse this
                }
                else if ("BEGIN ATOM".Equals(command))
                {
                    ReadAtomBlock(readData);
                }
                else if ("BEGIN BOND".Equals(command))
                {
                    ReadBondBlock(readData);
                }
                else if ("BEGIN SGROUP".Equals(command))
                {
                    ReadSGroup(readData);
                }
                else
                {
                    Trace.TraceWarning("Unrecognized command: " + command);
                }
                lastLine = ReadLine();
            }
            return readData;
        }

        /**
         * @ when no file content is detected
         * @return Last line read
         */
        public string ReadHeader(IAtomContainer readData)
        {
            // read four lines
            string line1 = ReadLine();
            if (line1 == null)
            {
                throw new CDKException("Expected a header line, but found nothing.");
            }
            if (line1.Length > 0)
            {
                if (line1.StartsWith("M  V30"))
                {
                    // no header
                    return line1;
                }
                readData.SetProperty(CDKPropertyName.TITLE, line1);
            }
            ReadLine();
            string line3 = ReadLine();
            if (line3.Length > 0) readData.SetProperty(CDKPropertyName.COMMENT, line3);
            string line4 = ReadLine();
            if (!line4.Contains("3000"))
            {
                throw new CDKException("This file is not a MDL V3000 molfile.");
            }
            return ReadLine();
        }

        /**
         * Reads the atoms, coordinates and charges.
         *
         * <p>IMPORTANT: it does not support the atom list and its negation!
         */
        public void ReadAtomBlock(IAtomContainer readData)
        {
            Trace.TraceInformation("Reading ATOM block");
            IsotopeFactory isotopeFactory;
            try
            {
                isotopeFactory = Isotopes.Instance;
            }
            catch (IOException exception)
            {
                throw new CDKException("Could not initiate the IsotopeFactory.", exception);
            }

            int RGroupCounter = 1;
            int Rnumber = 0;
            string[] rGroup = null;

            bool foundEND = false;
            while (!foundEND)
            {
                string command = ReadCommand(ReadLine());
                if ("END ATOM".Equals(command))
                {
                    // FIXME: should check whether 3D is really 2D
                    foundEND = true;
                }
                else
                {
                    Debug.WriteLine("Parsing atom from: " + command);
                    IAtom atom = readData.Builder.CreateAtom();
                    var tokenizer = Strings.Tokenize(command).GetEnumerator();
                    // parse the index
                    try
                    {
                        tokenizer.MoveNext();
                        atom.Id = tokenizer.Current;
                    }
                    catch (Exception exception)
                    {
                        string error = "Error while parsing atom index";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }
                    // parse the element
                    tokenizer.MoveNext();
                    string element = tokenizer.Current;
                    if (isotopeFactory.IsElement(element))
                    {
                        atom = isotopeFactory.Configure(readData.Builder.CreateAtom(element));
                    }
                    else if ("A".Equals(element))
                    {
                        atom = readData.Builder.CreatePseudoAtom(element);
                    }
                    else if ("Q".Equals(element))
                    {
                        atom = readData.Builder.CreatePseudoAtom(element);
                    }
                    else if ("*".Equals(element))
                    {
                        atom = readData.Builder.CreatePseudoAtom(element);
                    }
                    else if ("LP".Equals(element))
                    {
                        atom = readData.Builder.CreatePseudoAtom(element);
                    }
                    else if ("L".Equals(element))
                    {
                        atom = readData.Builder.CreatePseudoAtom(element);
                    }
                    else if (element.Length > 0 && element[0] == 'R')
                    {
                        Debug.WriteLine("Atom ", element, " is not an regular element. Creating a PseudoAtom.");
                        //check if the element is R
                        var rGroupNumStr = element.Substring(1);
                        {
                            try
                            {
                                Rnumber = int.Parse(rGroup[(rGroup.Length - 1)]);
                                RGroupCounter = Rnumber;
                            }
                            catch (Exception)
                            {
                                Rnumber = RGroupCounter;
                                RGroupCounter++;
                            }
                            element = "R" + Rnumber;
                        }
                        atom = readData.Builder.CreatePseudoAtom(element);
                    }
                    else
                    {
                        if (mode == ChemObjectReaderModes.Strict)
                        {
                            throw new CDKException(
                                    "Invalid element type. Must be an existing element, or one in: A, Q, L, LP, *.");
                        }
                        atom = readData.Builder.CreatePseudoAtom(element);
                        atom.Symbol = element;
                    }

                    // parse atom coordinates (in Angstrom)
                    try
                    {
                        tokenizer.MoveNext();
                        string xString = tokenizer.Current;
                        tokenizer.MoveNext();
                        string yString = tokenizer.Current;
                        tokenizer.MoveNext();
                        string zString = tokenizer.Current;
                        double x = double.Parse(xString);
                        double y = double.Parse(yString);
                        double z = double.Parse(zString);
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
                    string mapping = tokenizer.Current;
                    if (!mapping.Equals("0"))
                    {
                        Trace.TraceWarning("Skipping atom-atom mapping: " + mapping);
                    } // else: default 0 is no mapping defined

                    // the rest are key value things
                    if (command.IndexOf('=') != -1)
                    {
                        IDictionary<string, string> options = ParseOptions(ExhaustStringTokenizer(tokenizer));
                        var keys = options.Keys;
                        foreach (var key in keys)
                        {
                            string value = options[key];
                            try
                            {
                                if (key.Equals("CHG"))
                                {
                                    int charge = int.Parse(value);
                                    if (charge != 0)
                                    { // zero is no charge specified
                                        atom.FormalCharge = charge;
                                    }
                                }
                                else
                                {
                                    Trace.TraceWarning("Not parsing key: " + key);
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
                    Debug.WriteLine("Added atom: " + atom);
                }
            }
        }

        /**
         * Reads the bond atoms, order and stereo configuration.
         */
        public void ReadBondBlock(IAtomContainer readData)
        {
            Trace.TraceInformation("Reading BOND block");
            bool foundEND = false;
            while (!foundEND)
            {
                string command = ReadCommand(ReadLine());
                if ("END BOND".Equals(command))
                {
                    foundEND = true;
                }
                else
                {
                    Debug.WriteLine("Parsing bond from: " + command);
                    var tokenizer = Strings.Tokenize(command).GetEnumerator();
                    IBond bond = readData.Builder.CreateBond();
                    // parse the index
                    try
                    {
                        tokenizer.MoveNext();
                        string indexString = tokenizer.Current;
                        bond.Id = indexString;
                    }
                    catch (Exception exception)
                    {
                        string error = "Error while parsing bond index";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }
                    // parse the order
                    try
                    {
                        tokenizer.MoveNext();
                        string orderString = tokenizer.Current;
                        int order = int.Parse(orderString);
                        if (order >= 4)
                        {
                            Trace.TraceWarning("Query order types are not supported (yet). File a bug if you need it");
                        }
                        else
                        {
                            bond.Order = BondManipulator.CreateBondOrder((double)order);
                        }
                    }
                    catch (Exception exception)
                    {
                        string error = "Error while parsing bond index";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }
                    // parse index atom 1
                    try
                    {
                        tokenizer.MoveNext();
                        string indexAtom1String = tokenizer.Current;
                        int indexAtom1 = int.Parse(indexAtom1String);
                        IAtom atom1 = readData.Atoms[indexAtom1 - 1];
                        bond.Atoms.Add(atom1);  // bond.Atoms[0]
                    }
                    catch (Exception exception)
                    {
                        string error = "Error while parsing index atom 1 in bond";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }
                    // parse index atom 2
                    try
                    {
                        tokenizer.MoveNext();
                        string indexAtom2String = tokenizer.Current;
                        int indexAtom2 = int.Parse(indexAtom2String);
                        IAtom atom2 = readData.Atoms[indexAtom2 - 1];
                        bond.Atoms.Add(atom2); // bond.Atoms[1]
                    }
                    catch (Exception exception)
                    {
                        string error = "Error while parsing index atom 2 in bond";
                        Trace.TraceError(error);
                        Debug.WriteLine(exception);
                        throw new CDKException(error, exception);
                    }

                    List<IAtom> endpts = new List<IAtom>();
                    string attach = null;

                    // the rest are key=value fields
                    if (command.IndexOf('=') != -1)
                    {
                        IDictionary<string, string> options = ParseOptions(ExhaustStringTokenizer(tokenizer));
                        foreach (var key in options.Keys)
                        {
                            string value = options[key];
                            try
                            {
                                if (key.Equals("CFG"))
                                {
                                    int configuration = int.Parse(value);
                                    if (configuration == 0)
                                    {
                                        bond.Stereo = BondStereo.None;
                                    }
                                    else if (configuration == 1)
                                    {
                                        bond.Stereo = BondStereo.Up;
                                    }
                                    else if (configuration == 2)
                                    {
                                        bond.Stereo = BondStereo.None;
                                    }
                                    else if (configuration == 3)
                                    {
                                        bond.Stereo = BondStereo.Down;
                                    }
                                }
                                else if (key.Equals("ENDPTS"))
                                {
                                    string[] endptStr = value.Split(' ');
                                    // skip first value that is count
                                    for (int i = 1; i < endptStr.Length; i++)
                                    {
                                        endpts.Add(readData.Atoms[int.Parse(endptStr[i]) - 1]);
                                    }
                                }
                                else if (key.Equals("ATTACH"))
                                {
                                    attach = value;
                                }
                                else
                                {
                                    Trace.TraceWarning("Not parsing key: " + key);
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

                    // storing bond
                    readData.Bonds.Add(bond);

                    // storing positional variation
                    if ("ANY".Equals(attach))
                    {
                        Sgroup sgroup = new Sgroup();
                        sgroup.Type = SgroupType.ExtMulticenter;
                        sgroup.Atoms.Add(bond.Atoms[0]); // could be other end?
                        sgroup.Bonds.Add(bond);
                        foreach (var endpt in endpts)
                            sgroup.Atoms.Add(endpt);

                        IList<Sgroup> sgroups = readData.GetProperty<IList<Sgroup>>(CDKPropertyName.CTAB_SGROUPS);
                        if (sgroups == null)
                            readData.SetProperty(CDKPropertyName.CTAB_SGROUPS, sgroups = new List<Sgroup>(4));
                        sgroups.Add(sgroup);
                    }

                    Debug.WriteLine("Added bond: " + bond);
                }
            }
        }

        /**
         * Reads labels.
         */
        public void ReadSGroup(IAtomContainer readData)
        {
            bool foundEND = false;
            while (!foundEND)
            {
                string command = ReadCommand(ReadLine());
                if ("END SGROUP".Equals(command))
                {
                    foundEND = true;
                }
                else
                {
                    Debug.WriteLine("Parsing Sgroup line: " + command);
                    var tokenizer = Strings.Tokenize(command).GetEnumerator();
                    // parse the index
                    tokenizer.MoveNext();
                    string indexString = tokenizer.Current;
                    Trace.TraceWarning("Skipping external index: " + indexString);
                    // parse command type
                    tokenizer.MoveNext();
                    string type = tokenizer.Current;
                    // parse the external index
                    tokenizer.MoveNext();
                    string externalIndexString = tokenizer.Current;
                    Trace.TraceWarning("Skipping external index: " + externalIndexString);

                    // the rest are key=value fields
                    IDictionary<string, string> options = new Dictionary<string, string>();
                    if (command.IndexOf('=') != -1)
                    {
                        options = ParseOptions(ExhaustStringTokenizer(tokenizer));
                    }

                    // now interpret line
                    if (type.StartsWith("SUP"))
                    {
                        var keys = options.Keys;
                        int atomID = -1;
                        string label = "";
                        foreach (var key in keys)
                        {
                            string value = options[key];
                            try
                            {
                                if (key.Equals("ATOMS"))
                                {
                                    var atomsTokenizer = Strings.Tokenize(value).GetEnumerator();
                                    atomsTokenizer.MoveNext();
                                    int.Parse(atomsTokenizer.Current); // should be 1, int atomCount =
                                    atomsTokenizer.MoveNext();
                                    atomID = int.Parse(atomsTokenizer.Current);
                                }
                                else if (key.Equals("LABEL"))
                                {
                                    label = value;
                                }
                                else
                                {
                                    Trace.TraceWarning("Not parsing key: " + key);
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
                            if (atomID != -1 && label.Length > 0)
                            {
                                IAtom original = readData.Atoms[atomID - 1];
                                IAtom replacement = original;
                                if (!(original is IPseudoAtom))
                                {
                                    replacement = readData.Builder.CreatePseudoAtom(original);
                                }
                                ((IPseudoAtom)replacement).Label = label;
                                if (replacement != original)
                                    AtomContainerManipulator.RePlaceAtomByAtom(readData, original, replacement);
                            }
                        }
                    }
                    else
                    {
                        Trace.TraceWarning("Skipping unrecognized SGROUP type: " + type);
                    }
                }
            }
        }

        /**
         * Reads the command on this line. If the line is continued on the next, that
         * part is added.
         *
         * @return Returns the command on this line.
         */
        private string ReadCommand(string line)
        {
            if (line.StartsWith("M  V30 "))
            {
                string command = line.Substring(7);
                if (command.EndsWith("-"))
                {
                    command = command.Substring(0, command.Length - 1);
                    command += ReadCommand(ReadLine());
                }
                return command;
            }
            else
            {
                throw new CDKException("Could not read MDL file: unexpected line: " + line);
            }
        }

        private IDictionary<string, string> ParseOptions(string str)
        {
            IDictionary<string, string> keyValueTuples = new Dictionary<string, string>();
            while (str.Length >= 3)
            {
                Debug.WriteLine("Matching remaining option string: " + str);
                var tuple1Matcher = keyValueTuple2.Match(str);
                if (tuple1Matcher.Success)
                {
                    string key = tuple1Matcher.Groups[1].Value;
                    string value = tuple1Matcher.Groups[2].Value;
                    str = tuple1Matcher.Groups[3].Value;
                    Debug.WriteLine("Found key: " + key);
                    Debug.WriteLine("Found value: " + value);
                    keyValueTuples[key] = value;
                }
                else
                {
                    var tuple2Matcher = keyValueTuple.Match(str);
                    if (tuple2Matcher.Success)
                    {
                        string key = tuple2Matcher.Groups[1].Value;
                        string value = tuple2Matcher.Groups[2].Value;
                        str = tuple2Matcher.Groups[3].Value;
                        Debug.WriteLine("Found key: " + key);
                        Debug.WriteLine("Found value: " + value);
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

        public string ExhaustStringTokenizer(IEnumerator<string> tokenizer)
        {
            StringBuilder buffer = new StringBuilder();
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
            string line = null;
            try
            {
                line = input.ReadLine();
                lineNumber++;
                Debug.WriteLine("read line " + lineNumber + ":", line);
            }
            catch (Exception exception)
            {
                string error = "Unexpected error while reading file: " + exception.Message;
                Trace.TraceError(error);
                Debug.WriteLine(exception);
                throw new CDKException(error, exception);
            }
            return line;
        }

        public override void Close()
        {
            input.Close();
        }

        private void InitIOSettings() { }

        public override void Dispose()
        {
            Close();
        }
    }
}

