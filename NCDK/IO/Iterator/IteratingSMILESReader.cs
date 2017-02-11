/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Smiles;
using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.IO.Iterator
{
    /**
     * Iterating SMILES file reader. It allows to iterate over all molecules
     * in the SMILES file, without being read into memory all. Suitable
     * for very large SMILES files. These SMILES files are expected to have one
     * molecule on each line. If a line could not be parsed and empty molecule is
     * returned and the property {@link #BAD_SMILES_INPUT} is set to the attempted
     * input. The error is also logged.
     *
     * <p>For parsing each SMILES it still uses the normal SMILESReader.
     *
     * @cdk.module smiles
     * @cdk.githash
     * @cdk.iooptions
     *
     * @see org.openscience.cdk.io.SMILESReader
     *
     * @author     Egon Willighagen <egonw@sci.kun.nl>
     * @cdk.created    2004-12-16
     *
     * @cdk.keyword    file format, SMILES
     */
    public class IteratingSMILESReader : DefaultIteratingChemObjectReader<IAtomContainer>
    {
        private TextReader input;
        private SmilesParser sp = null;

        private readonly IChemObjectBuilder builder;

        /// <summary>Store the problem input as a property.</summary>
        public const string BAD_SMILES_INPUT = "bad.smiles.input";

        /**
         * Constructs a new IteratingSMILESReader that can read Molecule from a given Reader.
         *
         * @param  in  The Reader to read from
         * @param builder The builder to use
         * @see org.openscience.cdk.Default.ChemObjectBuilder
         * @see org.openscience.cdk.silent.Silent.ChemObjectBuilder
         */
        public IteratingSMILESReader(TextReader ins, IChemObjectBuilder builder)
        {
            sp = new SmilesParser(builder);
            SetReader(ins);
            this.builder = builder;
        }

        /**
         * Constructs a new IteratingSMILESReader that can read Molecule from a given Stream and IChemObjectBuilder.
         *
         * @param in      The input stream
         * @param builder The builder
         */
        public IteratingSMILESReader(Stream ins, IChemObjectBuilder builder)
           : this(new StreamReader(ins), builder)
        { }

        /**
         * Get the format for this reader.
         *
         * @return An instance of {@link NCDK.IO.Formats.SMILESFormat}
         */
        public override IResourceFormat Format => SMILESFormat.Instance;

        /**
         * Checks whether there is another molecule to read.
         *
         * @return  true if there are molecules to read, false otherwise
         */

        public override IEnumerator<IAtomContainer> GetEnumerator()
        {
            string line;
            // now try to parse the next Molecule
            while ((line = input.ReadLine()) != null)
            {
                IAtomContainer nextMolecule;
                try
                {
                    string suffix = Suffix(line);
                    nextMolecule = ReadSmiles(line);
                    nextMolecule.SetProperty(CDKPropertyName.TITLE, suffix);
                }
                catch (Exception exception)
                {
                    Trace.TraceError("Unexpeced problem: ", exception.Message);
                    Debug.WriteLine(exception);
                    yield break;
                }
                yield return nextMolecule;
            }
            yield break;
        }

        /**
         * Obtain the suffix after a line containing SMILES. The suffix follows
         * any ' ' or '\t' termination characters.
         *
         * @param line input line
         * @return the suffix - or an empty line
         */
        private string Suffix(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == ' ' || c == '\t') return line.Substring(i + 1);
            }
            return "";
        }

        /**
         * Read the SMILES given in the input line - or return an empty container.
         *
         * @param line input line
         * @return the read container (or an empty one)
         */
        private IAtomContainer ReadSmiles(string line)
        {
            try
            {
                return sp.ParseSmiles(line);
            }
            catch (CDKException e)
            {
                Trace.TraceError("Error while reading the SMILES from: " + line + ", ", e);
                IAtomContainer empty = builder.CreateAtomContainer();
                empty.SetProperty(BAD_SMILES_INPUT, line);
                return empty;
            }
        }

        /**
         * Close the reader.
         *
         * @ if there is an error during closing
         */

        public override void Close()
        {
            if (input != null) input.Close();
        }

        public override void Remove()
        {
            throw new NotSupportedException();
        }

        public override void SetReader(TextReader reader)
        {
            this.input = reader;
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
