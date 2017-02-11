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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Common.Primitives;
using NCDK.IO.Formats;
using NCDK.IO.InChI;
using System;
using System.IO;

namespace NCDK.IO
{
    /**
     * Reads the content of a IUPAC/NIST Chemical Identifier (INChI) plain text
     * document. This reader parses output generated with INChI 1.12beta like:
     * <pre>
     *
     * Input_File: "E:\Program Files\INChI\inchi-samples\Figure04.mol"
     *
     * Structure: 1
     * INChI=1.12Beta/C6H6/c1-2-4-6-5-3-1/h1-6H
     * AuxInfo=1.12Beta/0/N:1,2,3,4,5,6/E:(1,2,3,4,5,6)/rA:6CCCCCC/rB:s1;d1;d2;s3;s4d5;/rC:5.6378,-4.0013,0;5.6378,-5.3313,0;4.4859,-3.3363,0;4.4859,-5.9963,0;3.3341,-4.0013,0;3.3341,-5.3313,0;
     * </pre>
     *
     * @cdk.module extra
     * @cdk.githash
     * @cdk.iooptions
     *
     * @author      Egon Willighagen <egonw@sci.kun.nl>
     * @cdk.created 2004-08-01
     *
     * @cdk.keyword file format, INChI
     * @cdk.keyword chemical identifier
     * @cdk.require java1.4+
     *
     * @see     org.openscience.cdk.io.INChIReader
     */
    public class INChIPlainTextReader : DefaultChemObjectReader
    {
        private TextReader input;
        private INChIContentProcessorTool inchiTool;

        /**
         * Construct a INChI reader from a Reader object.
         *
         * @param input the Reader with the content
         */
        public INChIPlainTextReader(TextReader input)
        {
            this.Init();
            SetReader(input);
            inchiTool = new INChIContentProcessorTool();
        }

        public INChIPlainTextReader(Stream input)
            : this(new StreamReader(input))
        { }

        public INChIPlainTextReader()
            : this(new StringReader(""))
        { }

        public override IResourceFormat Format => INChIPlainTextFormat.Instance;

        public override void SetReader(TextReader input)
        {
            this.input = input;
        }

        public override void SetReader(Stream input)
        {
            SetReader(new StreamReader(input));
        }

        /**
         * Initializes this reader.
         */
        private void Init() { }

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            return false;
        }

        /**
         * Reads a IChemObject of type object from input.
         * Supported types are: ChemFile.
         *
         * @param  object type of requested IChemObject
         * @return the content in a ChemFile object
         */
        public override T Read<T>(T obj)
        {
            if (obj is IChemFile)
            {
                return (T)ReadChemFile((IChemFile)obj);
            }
            else
            {
                throw new CDKException("Only supported is reading of ChemFile objects.");
            }
        }

        // private functions

        /**
         * Reads a ChemFile object from input.
         *
         * @return ChemFile with the content read from the input
         */
        private IChemFile ReadChemFile(IChemFile cf)
        {
            // have to do stuff here
            try
            {
                string line = null;
                while ((line = input.ReadLine()) != null)
                {
                    if (line.StartsWith("INChI=") || line.StartsWith("InChI="))
                    {
                        // ok, the fun starts
                        cf = cf.Builder.CreateChemFile();
                        // ok, we need to parse things like:
                        // INChI=1.12Beta/C6H6/c1-2-4-6-5-3-1/h1-6H
                        string INChI = line.Substring(6);
                        var tok = Strings.Tokenize(INChI, '/');
                        // ok, we expect 4 tokens
                        // tok[0]; // 1.12Beta not stored since never used
                        string formula = tok[1]; // C6H6
                        string connections = null;
                        if (tok.Count > 2)
                            connections = tok[2].Substring(1); // 1-2-4-6-5-3-1
                                                               //final string hydrogens = tokenizer.NextToken().Substring(1); // 1-6H

                        IAtomContainer parsedContent = inchiTool.ProcessFormula(
                                cf.Builder.CreateAtomContainer(), formula);
                        if (connections != null)
                            inchiTool.ProcessConnections(connections, parsedContent, -1);

                        var moleculeSet = cf.Builder.CreateAtomContainerSet();
                        moleculeSet.Add(cf.Builder.CreateAtomContainer(parsedContent));
                        IChemModel model = cf.Builder.CreateChemModel();
                        model.MoleculeSet = moleculeSet;
                        IChemSequence sequence = cf.Builder.CreateChemSequence();
                        sequence.Add(model);
                        cf.Add(sequence);
                    }
                }
            }
            catch (Exception exception)
            {
                if (exception is IOException || exception is ArgumentException)
                {
                    Console.Error.WriteLine(exception.StackTrace);
                    throw new CDKException("Error while reading INChI file: " + exception.Message, exception);
                }
                else
                    throw;
            }
            return cf;
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
