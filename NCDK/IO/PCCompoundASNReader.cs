/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.IO.Formats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NCDK.IO
{
    /// <summary>
    /// Reads an object from ASN formated input for PubChem Compound entries. The following
    /// bits are supported: atoms.aid, atoms.element, bonds.aid1, bonds.aid2. Additionally,
    /// it extracts the InChI and canonical SMILES properties.
    /// </summary>
    // @cdk.module io
    // @cdk.githash
    // @cdk.iooptions
    // @cdk.keyword file format, PubChem Compound ASN
    public class PCCompoundASNReader : DefaultChemObjectReader
    {
        private TextReader input;
        IAtomContainer molecule = null;
        IDictionary<string, IAtom> atomIDs = null;

        /// <summary>
        /// Construct a new reader from a Reader type object.
        /// </summary>
        /// <param name="input">reader from which input is read</param>
        public PCCompoundASNReader(TextReader input)
        {
            this.input = input;
        }

        public PCCompoundASNReader(Stream input)
            : this(new StreamReader(input))
        { }

        public PCCompoundASNReader()
                : this(new StringReader(""))
        { }

        public override IResourceFormat Format => PubChemASNFormat.Instance;

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
            return false;
        }

        public override T Read<T>(T obj)
        {
            if (obj is IChemFile)
            {
                try
                {
                    return (T)ReadChemFile((IChemFile)obj);
                }
                catch (IOException e)
                {
                    throw new CDKException("An IO Exception occurred while reading the file.", e);
                }
                catch (CDKException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new CDKException("An error occurred.", e);
                }
            }
            else
            {
                throw new CDKException("Only supported is reading of ChemFile objects.");
            }
        }

        public override void Close()
        {
            input.Close();
        }

        public override void Dispose()
        {
            Close();
        }

        // private procedures

        private IChemFile ReadChemFile(IChemFile file)
        {
            IChemSequence chemSequence = file.Builder.CreateChemSequence();
            IChemModel chemModel = file.Builder.CreateChemModel();
            var moleculeSet = file.Builder.CreateAtomContainerSet();
            molecule = file.Builder.CreateAtomContainer();
            atomIDs = new Dictionary<string, IAtom>();

            string line = input.ReadLine();
            while (line != null)
            {
                if (line.IndexOf('{') != -1)
                {
                    ProcessBlock(line);
                }
                else
                {
                    Trace.TraceWarning("Skipping non-block: " + line);
                }
                line = input.ReadLine();
            }
            moleculeSet.Add(molecule);
            chemModel.MoleculeSet = moleculeSet;
            chemSequence.Add(chemModel);
            file.Add(chemSequence);
            return file;
        }

        private void ProcessBlock(string line)
        {
            string command = GetCommand(line);
            if (command.Equals("atoms"))
            {
                // parse frame by frame
                Debug.WriteLine("ASN atoms found");
                ProcessAtomBlock();
            }
            else if (command.Equals("bonds"))
            {
                // ok, that fine
                Debug.WriteLine("ASN bonds found");
                ProcessBondBlock();
            }
            else if (command.Equals("props"))
            {
                // ok, that fine
                Debug.WriteLine("ASN props found");
                ProcessPropsBlock();
            }
            else if (command.Equals("PC-Compound ::="))
            {
                // ok, that fine
                Debug.WriteLine("ASN PC-Compound found");
            }
            else
            {
                Trace.TraceWarning("Skipping block: " + command);
                SkipBlock();
            }
        }

        private void ProcessPropsBlock()
        {
            string line = input.ReadLine();
            while (line != null)
            {
                if (line.IndexOf('{') != -1)
                {
                    ProcessPropsBlockBlock();
                }
                else if (line.IndexOf('}') != -1)
                {
                    return;
                }
                else
                {
                    Trace.TraceWarning("Skipping non-block: " + line);
                }
                line = input.ReadLine();
            }
        }

        private void ProcessPropsBlockBlock()
        {
            string line = input.ReadLine();
            URN urn = new URN();
            while (line != null)
            {
                if (line.IndexOf("urn") != -1)
                {
                    urn = ExtractURN();
                }
                else if (line.IndexOf("value") != -1)
                {
                    Debug.WriteLine("Found a prop value line: " + line);
                    if (line.IndexOf(" sval") != -1)
                    {
                        Debug.WriteLine("Label: " + urn.Label);
                        Debug.WriteLine("Name: " + urn.Name);
                        if ("InChI".Equals(urn.Label))
                        {
                            string value = GetQuotedValue(line.Substring(line.IndexOf("value sval") + 10));
                            molecule.SetProperty(CDKPropertyName.InChI, value);
                        }
                        else if ("SMILES".Equals(urn.Label) && "Canonical".Equals(urn.Name))
                        {
                            string value = GetQuotedValue(line.Substring(line.IndexOf("value sval") + 10));
                            molecule.SetProperty(CDKPropertyName.SMILES, value);
                        }
                    }
                }
                else if (line.IndexOf('}') != -1)
                {
                    return;
                }
                else
                {
                    Trace.TraceWarning("Skipping non-block: " + line);
                }
                line = input.ReadLine();
            }
        }

        private URN ExtractURN()
        {
            URN urn = new URN();
            string line = input.ReadLine();
            while (line != null)
            {
                if (line.IndexOf("name") != -1)
                {
                    urn.Name = GetQuotedValue(line.Substring(line.IndexOf("name") + 4));
                }
                else if (line.IndexOf("label") != -1)
                {
                    urn.Label = GetQuotedValue(line.Substring(line.IndexOf("label") + 4));
                }
                else if (line.IndexOf('}') != -1 && line.IndexOf('\"') == -1)
                {
                    // ok, don't return if it also has a "
                    return urn;
                }
                else
                {
                    Trace.TraceWarning("Ignoring URN statement: " + line);
                }
                line = input.ReadLine();
            }
            return urn;
        }

        private void ProcessAtomBlock()
        {
            string line = input.ReadLine();
            while (line != null)
            {
                if (line.IndexOf('{') != -1)
                {
                    ProcessAtomBlockBlock(line);
                }
                else if (line.IndexOf('}') != -1)
                {
                    return;
                }
                else
                {
                    Trace.TraceWarning("Skipping non-block: " + line);
                }
                line = input.ReadLine();
            }
        }

        private void ProcessBondBlock()
        {
            string line = input.ReadLine();
            while (line != null)
            {
                if (line.IndexOf('{') != -1)
                {
                    ProcessBondBlockBlock(line);
                }
                else if (line.IndexOf('}') != -1)
                {
                    return;
                }
                else
                {
                    Trace.TraceWarning("Skipping non-block: " + line);
                }
                line = input.ReadLine();
            }
        }

        private IAtom GetAtom(int i)
        {
            if (molecule.Atoms.Count <= i)
            {
                molecule.Atoms.Add(molecule.Builder.CreateAtom());
            }
            return molecule.Atoms[i];
        }

        private IBond GetBond(int i)
        {
            if (molecule.Bonds.Count <= i)
            {
                molecule.Bonds.Add(molecule.Builder.CreateBond(null, null));
            }
            return molecule.Bonds[i];
        }

        private void ProcessAtomBlockBlock(string line)
        {
            string command = GetCommand(line);
            if (command.Equals("aid"))
            {
                // assume this is the first block in the atom block
                Debug.WriteLine("ASN atoms aid found");
                ProcessAtomAIDs();
            }
            else if (command.Equals("element"))
            {
                // assume this is the first block in the atom block
                Debug.WriteLine("ASN atoms element found");
                ProcessAtomElements();
            }
            else
            {
                Trace.TraceWarning("Skipping atom block block: " + command);
                SkipBlock();
            }
        }

        private void ProcessBondBlockBlock(string line)
        {
            string command = GetCommand(line);
            if (command.Equals("aid1"))
            {
                // assume this is the first block in the atom block
                Debug.WriteLine("ASN bonds aid1 found");
                ProcessBondAtomIDs(0);
            }
            else if (command.Equals("aid2"))
            {
                // assume this is the first block in the atom block
                Debug.WriteLine("ASN bonds aid2 found");
                ProcessBondAtomIDs(1);
            }
            else
            {
                Trace.TraceWarning("Skipping atom block block: " + command);
                SkipBlock();
            }
        }

        private void ProcessAtomAIDs()
        {
            string line = input.ReadLine();
            int atomIndex = 0;
            while (line != null)
            {
                if (line.IndexOf('}') != -1)
                {
                    // done
                    return;
                }
                else
                {
                    //                Debug.WriteLine("Found an atom ID: " + line);
                    //                Debug.WriteLine("  index: " + atomIndex);
                    IAtom atom = GetAtom(atomIndex);
                    string id = GetValue(line);
                    atom.Id = id;
                    atomIDs[id] = atom;
                    atomIndex++;
                }
                line = input.ReadLine();
            }
        }

        private void ProcessBondAtomIDs(int pos)
        {
            string line = input.ReadLine();
            int bondIndex = 0;
            while (line != null)
            {
                if (line.IndexOf('}') != -1)
                {
                    // done
                    return;
                }
                else
                {
                    //                Debug.WriteLine("Found an atom ID: " + line);
                    //                Debug.WriteLine("  index: " + atomIndex);
                    IBond bond = GetBond(bondIndex);
                    string id = GetValue(line);
                    IAtom atom = (IAtom)atomIDs[id];
                    if (atom == null)
                    {
                        throw new CDKException("File is corrupt: atom ID does not exist " + id);
                    }
                    bond.Atoms[pos] = atom;
                    bondIndex++;
                }
                line = input.ReadLine();
            }
        }

        private void ProcessAtomElements()
        {
            string line = input.ReadLine();
            int atomIndex = 0;
            while (line != null)
            {
                if (line.IndexOf('}') != -1)
                {
                    // done
                    return;
                }
                else
                {
                    //                Debug.WriteLine("Found symbol: " + ToSymbol(GetValue(line)));
                    //                Debug.WriteLine("  index: " + atomIndex);
                    IAtom atom = GetAtom(atomIndex);
                    atom.Symbol = ToSymbol(GetValue(line));
                    atomIndex++;
                }
                line = input.ReadLine();
            }
        }

        private string ToSymbol(string value)
        {
            if (value.Length == 1) return value.ToUpperInvariant();
            return value.Substring(0, 1).ToUpperInvariant() + value.Substring(1);
        }

        private void SkipBlock()
        {
            string line = input.ReadLine();
            int openBrackets = 0;
            while (line != null)
            {
                //            Debug.WriteLine("SkipBlock: line=" + line);
                if (line.IndexOf('{') != -1)
                {
                    openBrackets++;
                }
                //            Debug.WriteLine(" #open brackets: " + openBrackets);
                if (line.IndexOf('}') != -1)
                {
                    if (openBrackets == 0) return;
                    openBrackets--;
                }
                line = input.ReadLine();
            }
        }

        private string GetCommand(string line)
        {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            bool foundBracket = false;
            while (i < line.Length && !foundBracket)
            {
                char currentChar = line[i];
                if (currentChar == '{')
                {
                    foundBracket = true;
                }
                else
                {
                    buffer.Append(currentChar);
                }
                i++;
            }
            return foundBracket ? buffer.ToString().Trim() : null;
        }

        private string GetValue(string line)
        {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            bool foundComma = false;
            bool preWS = true;
            while (i < line.Length && !foundComma)
            {
                char currentChar = line[i];
                if (char.IsWhiteSpace(currentChar))
                {
                    if (!preWS) buffer.Append(currentChar);
                }
                else if (currentChar == ',')
                {
                    foundComma = true;
                }
                else
                {
                    buffer.Append(currentChar);
                    preWS = false;
                }
                i++;
            }
            return buffer.ToString();
        }

        private string GetQuotedValue(string line)
        {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            //        Debug.WriteLine("QV line: " + line);
            bool startQuoteFound = false;
            while (line != null)
            {
                while (i < line.Length)
                {
                    char currentChar = line[i];
                    if (currentChar == '"')
                    {
                        if (startQuoteFound)
                        {
                            return buffer.ToString();
                        }
                        else
                        {
                            startQuoteFound = true;
                        }
                    }
                    else if (startQuoteFound)
                    {
                        buffer.Append(currentChar);
                    }
                    i++;
                }
                line = input.ReadLine();
                i = 0;
            }
            return null;
        }

        struct URN
        {
            public string Name { get; set; }
            public string Label { get; set; }
        }
    }
}
