/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.IO.Formats;
using NCDK.Smiles;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// Reader for MoSS output files {@cdk.cite BOR2002} which present the results
    /// of a substructure mining study. These files look like:
    /// <code>
    /// id,description,nodes,edges,s_abs,s_rel,c_abs,c_rel
    /// 1,S-c:c:c:c:c:c,7,6,491,5.055081,5,1.7421603
    /// 2,S-c:c:c:c:c,6,5,493,5.0756717,5,1.7421603
    /// </code>
    ///
    /// <p><b>Caution</b>: the output contains substructures, not full molecules,
    /// even though they are read as such right now.
    ///
    // @cdk.module  smiles
    // @cdk.githash
    // @cdk.iooptions
    ///
    // @cdk.keyword MoSS
    /// </summary>
    public class MoSSOutputReader : DefaultChemObjectReader
    {
        private TextReader input;

        /// <summary>
        /// Create a reader for MoSS output files from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="input">source of CIF data</param>
        public MoSSOutputReader(TextReader input)
        {
            this.input = input;
        }

        /// <summary>
        /// Create a reader for MoSS output files from an <see cref="Stream"/>.
        /// </summary>
        /// <param name="input">source of CIF data</param>
        public MoSSOutputReader(Stream input)
                : this(new StreamReader(input))
        { }

        /// <summary>
        /// Create a reader for MoSS output files from an empty string.
        /// </summary>
        public MoSSOutputReader()
                : this(new StringReader(""))
        { }

        /// <inheritdoc/>    
        public override IResourceFormat Format => MoSSOutputFormat.Instance;

        /// <inheritdoc/>
        public override void SetReader(TextReader reader)
        {
            this.input = reader; // fixed CDK's bug
        }

        /// <inheritdoc/>

        public override void SetReader(Stream input)
        {
            SetReader(new StreamReader(input));
        }

        /// <inheritdoc/>
        public override bool Accepts(Type type)
        {
            if (typeof(IAtomContainerSet<IAtomContainer>).IsAssignableFrom(type)) return true;
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Read a <see cref="IAtomContainerSet{T}"/> from the input source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">object an <see cref="IAtomContainerSet{T}"/> into which the data is stored.</param>
        /// <returns>the content in a <see cref="IAtomContainerSet{T}"/> object</returns>
        public override T Read<T>(T obj)
        {
            if (obj is IAtomContainerSet<IAtomContainer>)
            {
                var cf = (IAtomContainerSet<IAtomContainer>)obj;
                try
                {
                    cf = ReadAtomContainerSet(cf);
                }
                catch (IOException)
                {
                    Trace.TraceError("Input/Output error while reading from input.");
                }
                return (T)cf;
            }
            else if (obj is IChemFile)
            {
                IChemFile chemFile = (IChemFile)obj;
                IChemSequence chemSeq = obj.Builder.CreateChemSequence();
                IChemModel chemModel = obj.Builder.CreateChemModel();
                var molSet = obj.Builder.CreateAtomContainerSet();
                try
                {
                    molSet = ReadAtomContainerSet(molSet);
                }
                catch (IOException)
                {
                    Trace.TraceError("Input/Output error while reading from input.");
                }
                chemModel.MoleculeSet = molSet;
                chemSeq.Add(chemModel);
                chemFile.Add(chemSeq);
                return (T)chemFile;
            }
            else
            {
                throw new CDKException("Only supported is reading of IAtomContainerSet.");
            }
        }

        /// <summary>
        /// Read the file content into a <see cref="IAtomContainerSet"/>.
        /// <param name="molSet">an <see cref="IAtomContainerSet"/> to store the structures</param>
        /// <returns>the <see cref="IAtomContainerSet"/> containing the molecules read in</returns>
        /// <exception cref="IOException">if there is an error during reading</exception>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> ReadAtomContainerSet(IAtomContainerSet<IAtomContainer> molSet)
        {
            SmilesParser parser = new SmilesParser(molSet.Builder);
            parser.Kekulise(false);
            string line = input.ReadLine();
            line = input.ReadLine(); // skip the first line
            while (line != null)
            {
                string[] cols = line.Split(',');
                try
                {
                    IAtomContainer mol = parser.ParseSmiles(cols[1]);
                    mol.SetProperty("focusSupport", cols[5]);
                    mol.SetProperty("complementSupport", cols[7]);
                    mol.SetProperty("atomCount", cols[2]);
                    mol.SetProperty("bondCount", cols[3]);
                    molSet.Add(mol);
                }
                catch (InvalidSmilesException exception)
                {
                    Trace.TraceError("Skipping invalid SMILES: " + cols[1]);
                    Debug.WriteLine(exception);
                }
                line = input.ReadLine();
            }
            return molSet;
        }

        /// <inheritdoc/>
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
