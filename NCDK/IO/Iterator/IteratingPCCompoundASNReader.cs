/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NCDK.IO.Iterator
{
    /// <summary>
    /// Iterating PubChem PCCompound ASN reader.
    /// </summary>
    // @cdk.module io
    // @cdk.githash
    // @cdk.iooptions
    // @see org.openscience.cdk.io.PCCompoundASNReader
    // @author       Egon Willighagen <egonw@users.sf.net>
    // @cdk.created  2008-05-05
    // @cdk.keyword  file format, ASN
    // @cdk.keyword  PubChem
    public class IteratingPCCompoundASNReader : DefaultIteratingChemObjectReader<IAtomContainer>
    {
        private TextReader input;
        private IChemObjectBuilder builder;
        private int depth;

        /// <summary>
        /// Constructs a new IteratingPCCompoundASNReader that can read Molecule from a given Reader.
        /// </summary>
        /// <param name="ins"> The Reader to read from</param>
        /// <param name="builder"></param>
        public IteratingPCCompoundASNReader(TextReader ins, IChemObjectBuilder builder)
        {
            this.builder = builder;
            SetReader(ins);
        }

        /// <summary>
        /// Constructs a new <see cref="IteratingPCCompoundASNReader"/> that can read molecule from a given <see cref="Stream"/> and <see cref="IChemObjectBuilder"/>.
        /// </summary>
        /// <param name="ins">The input stream</param>
        /// <param name="builder">The builder</param>
        public IteratingPCCompoundASNReader(Stream ins, IChemObjectBuilder builder)
            : this(new StreamReader(ins), builder)
        { }

        public override IResourceFormat Format => PubChemSubstancesASNFormat.Instance;

        public override IEnumerator<IAtomContainer> GetEnumerator()
        {
            // now try to read the next molecule
            string currentLine = input.ReadLine();
            while (currentLine != null)
            {
                IAtomContainer ac = null;
                try
                {
                    var buffer = new StringBuilder();
                    for (;;)
                    {
                        int depthDiff = CountBrackets(currentLine);
                        depth += depthDiff;
                        if (depthDiff > 0 && depth == 3)
                        {
                            string command = GetCommand(currentLine);
                            if (command.Equals("compound"))
                            {
                                buffer.Append("PC-Compound ::= {\n");
                                currentLine = input.ReadLine();
                                break;
                            }
                        }
                        if ((currentLine = input.ReadLine()) == null)
                            yield break;
                    }
                    for (;;)
                    {
                        int depthDiff = CountBrackets(currentLine);
                        depth += depthDiff;
                        if (depthDiff < 0 && depth == 2)
                        {
                            buffer.Append("}\n");
                            currentLine = input.ReadLine();
                            break;
                        }
                        else
                        {
                            buffer.Append(currentLine).Append('\n');
                        }
                        if ((currentLine = input.ReadLine()) == null)
                            yield break;
                    }
                    using (PCCompoundASNReader asnReader = new PCCompoundASNReader(new StringReader(buffer.ToString())))
                    {
                        IChemFile cFile = (IChemFile)asnReader.Read(builder.CreateChemFile());
                        ac = ChemFileManipulator.GetAllAtomContainers(cFile).First();
                    }
                }
                catch (Exception exception)
                {
                    if (!(exception is IOException || exception is ArgumentException || exception is CDKException))
                            throw;
                    Trace.TraceError("Error while reading next molecule: ", exception.Message);
                    Debug.WriteLine(exception);
                    Console.Error.WriteLine(exception.StackTrace);
                }
                yield return ac;
            }
            yield break;
        }

        private int CountChars(string copy, char character)
        {
            int occurences = 0;
            for (int i = 0; i < copy.Length; i++)
            {
                if (character == copy[i]) occurences++;
            }
            return occurences;
        }

        private int CountBrackets(string currentLine)
        {
            int bracketsOpen = CountChars(currentLine, '{');
            int bracketsClose = CountChars(currentLine, '}');
            return bracketsOpen - bracketsClose;
        }

        public override void Close()
        {
            input.Close();
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

        public override void SetReader(TextReader reader)
        {
            input = (TextReader)reader;
        }

        public override void SetReader(Stream reader)
        {
            SetReader(new StreamReader(reader));
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
