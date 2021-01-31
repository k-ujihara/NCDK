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
 */

using NCDK.IO.Formats;
using NCDK.IO.Setting;
using NCDK.Smiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NCDK.IO
{
    /// <summary>
    /// Writes the SMILES strings to a plain text file.
    /// </summary>
    // @cdk.module  smiles
    // @cdk.iooptions
    // @cdk.keyword file format, SMILES
    public class SMILESWriter : DefaultChemObjectWriter
    {
        private TextWriter writer;
        private BooleanIOSetting aromSetting;
        private BooleanIOSetting titleSetting;
        private IntegerIOSetting flavorSetting;
        private SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Default);
        private readonly StringBuilder buffer = new StringBuilder();

        /// <summary>
        /// Constructs a new SMILESWriter that can write a list of SMILES to a Writer
        /// </summary>
        /// <param name="output">The Writer to write to</param>
        public SMILESWriter(TextWriter output)
        {
            this.writer = output;
            InitIOSettings();
        }

        public SMILESWriter(Stream output)
                : this(new StreamWriter(output))
        { }

        public void SetFlavor(SmiFlavors flav)
        {
            try
            {
                flavorSetting.Setting = Convert.ToString((int)flav, 2);
            }
            catch (CDKException)
            {
                // ignored
            }
            CustomizeJob();
        }

        public void SetWriteTitle(bool val)
        {
            try
            {
                titleSetting.Setting = val.ToString();
            }
            catch (CDKException)
            {
                // ignored
            }
            CustomizeJob();
        }

        public override IResourceFormat Format => SMILESFormat.Instance;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    writer.Flush();
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
            if (typeof(IEnumerableChemObject<IAtomContainer>).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Writes the content from object to output.
        /// </summary>
        /// <param name="obj">IChemObject of which the data is given as output.</param>
        public override void Write(IChemObject obj)
        {
            switch (obj)
            {
                case IEnumerableChemObject<IAtomContainer> o:
                    WriteAtomContainerSet(o);
                    break;
                case IAtomContainer mol:
                    WriteAtomContainer(mol);
                    break;
                default:
                    throw new CDKException($"Only supported is writing of {nameof(IAtomContainer)}.");
            }
        }

        /// <summary>
        /// Writes a list of molecules to an Stream.
        /// </summary>
        /// <param name="som">MoleculeSet that is written to an Stream</param>
        public void WriteAtomContainerSet(IEnumerable<IAtomContainer> som)
        {
            var listSom = som.ToReadOnlyList();
            WriteAtomContainer(listSom[0]);
            for (int i = 1; i <= listSom.Count - 1; i++)
            {
                try
                {
                    WriteAtomContainer(listSom[i]);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Writes the content from molecule to output.
        /// </summary>
        /// <param name="molecule">Molecule of which the data is given as output.</param>
        public void WriteAtomContainer(IAtomContainer molecule)
        {
            try
            {
                buffer.Length = 0;
                buffer.Append(smigen.Create(molecule));
                if (titleSetting.IsSet && molecule.Title != null)
                    buffer.Append('\t').Append(molecule.Title);
                buffer.Append('\n');
                writer.Write(buffer.ToString());
                writer.Flush();
            }
            catch (Exception e)
            {
                if (e is CDKException || e is IOException)
                {
                    Trace.TraceError($"Error while writing Molecule: {e.Message}");
                }
                else
                    throw e;
            }
        }

        private void InitIOSettings()
        {
            flavorSetting = Add(new IntegerIOSetting("SmilesFlavor", Importance.High,
                    "Output SMILES flavor, binary option", ((int)SmiFlavors.Default).ToString()));
            titleSetting = Add(new BooleanIOSetting("WriteTitle", Importance.High,
                    "Write the molecule title after the SMILES", "true"));
            aromSetting = Add(new BooleanIOSetting("UseAromaticity", Importance.Low,
                    "Should aromaticity information be stored in the SMILES?", "false"));
        }

        public void CustomizeJob()
        {
            ProcessIOSettingQuestion(flavorSetting);
            ProcessIOSettingQuestion(titleSetting);
            ProcessIOSettingQuestion(aromSetting);
            var flav = (SmiFlavors)flavorSetting.GetSettingValue();
            if (aromSetting.IsSet)
                flav |= SmiFlavors.UseAromaticSymbols;
            smigen = new SmilesGenerator(flav);
        }
    }
}
