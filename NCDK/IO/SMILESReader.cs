/* Copyright (C) 2001-2007  The Chemistry Development Kit (CDK) project
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
 *
 */
using NCDK.IO.Formats;
using NCDK.IO.Iterator;
using NCDK.Smiles;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// This Reader reads files which has one SMILES string on each
    /// line, where the format is given as below:
    /// <code>
    /// COC ethoxy ethane
    /// </code>
    /// Thus first the SMILES, and then after the first space (or tab) on the line a title
    /// that is stored as {@link CDKConstants#TITLE}. For legacy comparability the
    /// title is also placed in a "SMIdbNAME" property. If a line is invalid an empty
    /// molecule is inserted into the container set. The molecule with have the prop
    /// {@link IteratingSMILESReader#BAD_SMILES_INPUT} set to the input line that
    /// could not be read. 
    ///
    /// <p>For each line a molecule is generated, and multiple Molecules are
    /// read as MoleculeSet.
    ///
    // @cdk.module  smiles
    // @cdk.githash
    // @cdk.iooptions
    // @cdk.keyword file format, SMILES
    ///
    // @see org.openscience.cdk.io.iterator.IteratingSMILESReader
    /// </summary>
    public class SMILESReader : DefaultChemObjectReader
    {

        private TextReader input = null;
        private SmilesParser sp = null;

        /// <summary>
        /// Construct a new reader from a Reader and a specified builder object.
        ///
        /// <param name="input">The Reader object from which to read structures</param>
        /// </summary>
        public SMILESReader(TextReader input)
        {
            this.input = input;
        }

        public SMILESReader(Stream input)
                : this(new StreamReader(input))
        { }

        public SMILESReader()
                : this(new StringReader(""))
        { }

        public override IResourceFormat Format => SMILESFormat.Instance;

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
            if (typeof(IAtomContainerSet<IAtomContainer>).IsAssignableFrom(type)) return true;
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// reads the content from a XYZ input. It can only return a
        /// IChemObject of type ChemFile
        ///
        /// <param name="object">class must be of type ChemFile</param>
        ///
        /// <seealso cref="IChemFile"/>
        /// </summary>
        public override T Read<T>(T obj)
        {
            sp = new SmilesParser(obj.Builder);

            if (obj is IAtomContainerSet<IAtomContainer>)
            {
                return (T)ReadAtomContainerSet((IAtomContainerSet<IAtomContainer>)obj);
            }
            else if (obj is IChemFile)
            {
                IChemFile file = (IChemFile)obj;
                IChemSequence sequence = file.Builder.CreateChemSequence();
                IChemModel chemModel = file.Builder.CreateChemModel();
                chemModel.MoleculeSet = ReadAtomContainerSet(file.Builder.CreateAtomContainerSet());
                sequence.Add(chemModel);
                file.Add(sequence);
                return (T)file;
            }
            else
            {
                throw new CDKException("Only supported is reading of MoleculeSet objects.");
            }
        }

        // private procedures

        /// <summary>
        ///  Private method that actually parses the input to read a ChemFile
        ///  object.
        ///
        /// <param name="som">The set of molecules that came fron the file</param>
        /// <returns>A ChemFile containing the data parsed from input.</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> ReadAtomContainerSet(IAtomContainerSet<IAtomContainer> som)
        {
            try
            {
                string line;
                while ((line = input.ReadLine()) != null)
                {
                    line = line.Trim();
                    Debug.WriteLine("Line: ", line);

                    string name = Suffix(line);

                    try
                    {
                        IAtomContainer molecule = sp.ParseSmiles(line);
                        molecule.SetProperty("SMIdbNAME", name);
                        som.Add(molecule);
                    }
                    catch (CDKException exception)
                    {
                        Trace.TraceWarning("This SMILES could not be parsed: ", line);
                        Trace.TraceWarning("Because of: ", exception.Message);
                        Debug.WriteLine(exception);
                        IAtomContainer empty = som.Builder.CreateAtomContainer();
                        empty.SetProperty(IteratingSMILESReader.BAD_SMILES_INPUT, line);
                        som.Add(empty);
                    }
                }
            }
            catch (Exception exception)
            {
                Trace.TraceError("Error while reading SMILES line: ", exception.Message);
                Debug.WriteLine(exception);
            }
            return som;
        }


        public override void Close()
        {
            input.Close();
        }

        /// <summary>
        /// Obtain the suffix after a line containing SMILES. The suffix follows
        /// any ' ' or '\t' termination characters.
        ///
        /// <param name="line">input line</param>
        /// <returns>the suffix - or an empty line</returns>
        /// </summary>
        private string Suffix(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == ' ' || c == '\t') return line.Substring(i + 1);
            }
            return null;
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
