/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.IO.Setting;
using NCDK.Smiles;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /**
     * Writes the SMILES strings to a plain text file.
     *
     * @cdk.module  smiles
     * @cdk.githash
     * @cdk.iooptions
     *
     * @cdk.keyword file format, SMILES
     */
    public class SMILESWriter : DefaultChemObjectWriter
    {
        private TextWriter writer;

        private BooleanIOSetting useAromaticityFlag;

        /**
         * Constructs a new SMILESWriter that can write a list of SMILES to a Writer
         *
         * @param   out  The Writer to write to
         */
        public SMILESWriter(TextWriter out_)
        {
            this.writer = out_;
            InitIOSettings();
        }

        public SMILESWriter(Stream output)
                : this(new StreamWriter(output))
        { }

        public SMILESWriter()
            : this(new StringWriter())
        { }

        public override IResourceFormat Format => SMILESFormat.Instance;

        public override void SetWriter(TextWriter out_)
        {
            this.writer = out_;
        }

        public override void SetWriter(Stream output)
        {
            SetWriter(new StreamWriter(output));
        }

        /**
         * Flushes the output and closes this object.
         */

        public override void Close()
        {
            writer.Flush();
            writer.Close();
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            if (typeof(IAtomContainerSet<IAtomContainer>).IsAssignableFrom(type)) return true;
            return false;
        }

        /**
         * Writes the content from object to output.
         *
         * @param   object  IChemObject of which the data is outputted.
         */

        public override void Write(IChemObject obj)
        {
            if (obj is IAtomContainerSet<IAtomContainer>)
            {
                WriteAtomContainerSet((IAtomContainerSet<IAtomContainer>)obj);
            }
            else if (obj is IAtomContainer)
            {
                WriteAtomContainer((IAtomContainer)obj);
            }
            else
            {
                throw new CDKException("Only supported is writing of ChemFile and Molecule objects.");
            }
        }

        /**
         * Writes a list of molecules to an Stream.
         *
         * @param   som  MoleculeSet that is written to an Stream
         */
        public void WriteAtomContainerSet(IAtomContainerSet<IAtomContainer> som)
        {
            WriteAtomContainer(som[0]);
            for (int i = 1; i <= som.Count - 1; i++)
            {
                try
                {
                    WriteAtomContainer(som[i]);
                }
                catch (Exception)
                {
                }
            }
        }

        /**
         * Writes the content from molecule to output.
         *
         * @param   molecule  Molecule of which the data is outputted.
         */
        public void WriteAtomContainer(IAtomContainer molecule)
        {
            SmilesGenerator sg = new SmilesGenerator();
            if (useAromaticityFlag.IsSet) sg = sg.Aromatic();
            string smiles = "";
            try
            {
                smiles = sg.Create(molecule);
                Debug.WriteLine("Generated SMILES: " + smiles);
                writer.Write(smiles);
                writer.WriteLine();
                writer.Flush();
                Debug.WriteLine("file flushed...");
            }
            catch (Exception exc)
            {
                if (exc is CDKException | exc is IOException)
                {
                    Trace.TraceError("Error while writing Molecule: ", exc.Message);
                    Debug.WriteLine(exc);
                }
                else
                    throw;
            }
        }

        private void InitIOSettings()
        {
            useAromaticityFlag = Add(new BooleanIOSetting("UseAromaticity", IOSetting.Importance.Low,
                    "Should aromaticity information be stored in the SMILES?", "false"));
        }

        public void CustomizeJob()
        {
            FireIOSettingQuestion(useAromaticityFlag);
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
