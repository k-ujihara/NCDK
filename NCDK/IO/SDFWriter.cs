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
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /**
     * Writes MDL SD files ({@cdk.cite DAL92}). A MDL SD file contains one or more molecules,
     * complemented by properties.
     *
     * @cdk.module  io
     * @cdk.githash
     * @cdk.iooptions
     * @cdk.keyword file format, MDL SD file
     */
    public class SDFWriter : DefaultChemObjectWriter
    {
        private TextWriter writer;
        private BooleanIOSetting writerProperties;
        private ICollection<string> propertiesToWrite;

        /**
         * Constructs a new SDFWriter that writes to the given {@link Writer}.
         *
         * @param   out  The {@link Writer} to write to
         */
        public SDFWriter(TextWriter out_)
        {
            this.writer = out_;
            InitIOSettings();
        }

        /**
         * Constructs a new MDLWriter that can write to a given
         * {@link Stream}.
         *
         * @param   output  The {@link Stream} to write to
         */
        public SDFWriter(Stream output)
            : this(new StreamWriter(output))
        { }

        public SDFWriter()
                : this(new StringWriter())
        { }

        /**
         * Constructs a new SDFWriter that writes to the given {@link Writer}.
         *
         * @param out The {@link Writer} to write to
         */
        public SDFWriter(TextWriter out_, ICollection<string> propertiesToWrite)
        {
            writer = out_;
            InitIOSettings();
            this.propertiesToWrite = propertiesToWrite;
        }

        /**
         * Constructs a new SdfWriter that can write to a given
         * {@link Stream}.
         *
         * @param output The {@link Stream} to write to
         */
        public SDFWriter(Stream output, ICollection<string> propertiesToWrite)
                : this(new StreamWriter(output), propertiesToWrite)
        { }

        /**
         * Writes SD-File to a string including the given properties
         */
        public SDFWriter(ICollection<string> propertiesToWrite)
                : this(new StringWriter(), propertiesToWrite)
        { }

        public override IResourceFormat Format => SDFFormat.Instance;

        public override void SetWriter(TextWriter out_)
        {
            writer = out_;
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
            writer.Close();
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            if (typeof(IChemModel).IsAssignableFrom(type)) return true;
            if (typeof(IAtomContainerSet<IAtomContainer>).IsAssignableFrom(type)) return true;
            return false;
        }

        /**
         * Writes a IChemObject to the MDL SD file formated output. It can only
         * output IChemObjects of type {@link IChemFile}, {@link IAtomContainerSet}
         * and {@link IAtomContainerSet}.
         *
         * @param object an acceptable {@link IChemObject}
         *
         * @see #Accepts(Class)
         */
        public override void Write(IChemObject obj)
        {
            try
            {
                if (obj is IAtomContainerSet<IAtomContainer>)
                {
                    WriteMoleculeSet((IAtomContainerSet<IAtomContainer>)obj);
                    return;
                }
                else if (obj is IChemFile)
                {
                    WriteChemFile((IChemFile)obj);
                    return;
                }
                else if (obj is IChemModel)
                {
                    IChemFile file = obj.Builder.CreateChemFile();
                    IChemSequence sequence = obj.Builder.CreateChemSequence();
                    sequence.Add((IChemModel)obj);
                    file.Add(sequence);
                    WriteChemFile((IChemFile)file);
                    return;
                }
                else if (obj is IAtomContainer)
                {
                    WriteMolecule((IAtomContainer)obj);
                    return;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                Debug.WriteLine(ex);
                throw new CDKException("Exception while writing MDL file: " + ex.Message, ex);
            }
            throw new CDKException(
                    "Only supported is writing of ChemFile, MoleculeSet, AtomContainer and Molecule objects.");
        }

        /**
         * Writes an {@link IAtomContainerSet}.
         *
         * @param   som  the {@link IAtomContainerSet} to serialize
         */
        private void WriteMoleculeSet(IAtomContainerSet<IAtomContainer> som)
        {
            foreach (var mol in som)
            {
                WriteMolecule(mol);
            }
        }

        private void WriteChemFile(IChemFile file)
        {
            foreach (var container in ChemFileManipulator.GetAllAtomContainers(file))
            {
                WriteMolecule(container);
            }
        }

        private void WriteMolecule(IAtomContainer container)
        {
            try
            {
                // write the MDL molfile bits
                StringWriter stringWriter = new StringWriter();
                MDLV2000Writer mdlWriter = new MDLV2000Writer(stringWriter);
                mdlWriter.AddSettings(IOSettings.Settings);
                mdlWriter.Write(container);
                mdlWriter.Close();
                writer.Write(stringWriter.ToString());

                // write the properties
                var sdFields = container.GetProperties();
                bool writeAllProperties = propertiesToWrite == null;
                if (sdFields != null)
                {
                    foreach (var propKey in sdFields.Keys)
                    {
                        if (!IsCDKInternalProperty(propKey))
                        {
                            var stringKey = propKey as string;
                            if (writeAllProperties || (propKey != null && propertiesToWrite.Contains(stringKey)))
                            {
                                writer.Write("> <" + propKey + ">");
                                writer.WriteLine();
                                writer.Write("" + sdFields[propKey]);
                                writer.WriteLine();
                                writer.WriteLine();
                            }
                        }
                    }
                }
                writer.Write("$$$$\n");
            }
            catch (IOException exception)
            {
                throw new CDKException("Error while writing a SD file entry: " + exception.Message, exception);
            }
        }

        /**
         * A list of properties used by CDK algorithms which must never be
         * serialized into the SD file format.
         */
        private static List<string> cdkInternalProperties = new List<string>();

        static SDFWriter()
        {
            cdkInternalProperties.Add(InvPair.CANONICAL_LABEL);
            cdkInternalProperties.Add(InvPair.INVARIANCE_PAIR);
            // I think there are a few more, but cannot find them right now
        }

        private bool IsCDKInternalProperty(object propKey)
        {
            var stringKey = propKey as string;
            if (stringKey == null)
                return false;
            return cdkInternalProperties.Contains(stringKey);
        }

        private void InitIOSettings()
        {
            writerProperties = Add(new BooleanIOSetting("writeProperties", IOSetting.Importance.Low,
                    "Should molecular properties be written?", "true"));
            AddSettings(new MDLV2000Writer().IOSettings.Settings);
        }

        public void CustomizeJob()
        {
            foreach (var setting in IOSettings.Settings)
            {
                FireIOSettingQuestion(setting);
            }
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
