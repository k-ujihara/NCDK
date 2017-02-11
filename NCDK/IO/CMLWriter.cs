/* Copyright (C) 2001-2007  Egon Willighagen <egonw@users.sf.net>
 *                          Stefan Kuhn <shk3@users.sf.net>
 *                          Miguel Rojas-Cherto <miguelrojasch@users.sf.net>
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
using NCDK.IO.Setting;
using NCDK.LibIO.CML;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using NCDK.IO.Formats;
using System.Xml.Linq;
using System.Xml;
using System.Text;

namespace NCDK.IO
{
    /**
     * Serializes a {@link IAtomContainerSet} or a <see cref="IAtomContainer"/> object to CML 2 code.
     * Chemical Markup Language is an XML-based file format {@cdk.cite PMR99}.
     * Output can be redirected to other Writer objects like {@link StringWriter}
     * and {@link FileWriter}. An example:
     *
     * <pre>
     *   StringWriter output = new StringWriter();
     *   bool makeFragment = true;
     *   CMLWriter cmlwriter = new CMLWriter(output, makeFragment);
     *   cmlwriter.Write(molecule);
     *   cmlwriter.Close();
     *   string cmlcode = output.ToString();
     * </pre>
     *
     * <p>Output to a file called "molecule.cml" can done with:
     *
     * <pre>
     *   FileWriter output = new FileWriter("molecule.cml");
     *   CMLWriter cmlwriter = new CMLWriter(output);
     *   cmlwriter.Write(molecule);
     *   cmlwriter.Close();
     * </pre>
     *
     * <p>For atoms it outputs: coordinates, element type and formal charge.
     * For bonds it outputs: order, atoms (2, or more) and wedges.
     *
     * @cdk.module       libiocml
     * @cdk.githash
     * @cdk.require      java1.5+
     * @cdk.bug          1565563
     * @cdk.iooptions
     *
     * @see java.io.FileWriter
     * @see java.io.StringWriter
     *
     * @author Egon Willighagen
     *
     * @cdk.keyword file format, CML
     */
    public class CMLWriter : DefaultChemObjectWriter
    {
        private Stream output;

        private BooleanIOSetting cmlIds;
        private BooleanIOSetting namespacedOutput;
        private StringIOSetting namespacePrefix;
        private BooleanIOSetting schemaInstanceOutput;
        private StringIOSetting instanceLocation;
        private BooleanIOSetting indent;
        private BooleanIOSetting xmlDeclaration;

        private IList<ICMLCustomizer> customizers = null;

        class SteamByWriter : Stream
        {
            TextWriter writer;

            public SteamByWriter(TextWriter writer)
            {
                this.writer = writer;
            }

            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length { get { throw new NotSupportedException(); } }
            public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }
            public override void Flush() { writer.Flush(); }
            public override int Read(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
            public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
            public override void SetLength(long value) { throw new NotSupportedException(); }
            public override void Write(byte[] buffer, int offset, int count)
            {
                for (int i = 0; i < count; i++)
                    WriteByte(buffer[offset + i]);
            }

            public override void WriteByte(byte value)
            {
                writer.Write((char)value);
            }

            public override void Close()
            {
                writer.Close();
            }
        }

        /**
         * Constructs a new CMLWriter class. Output will be stored in the Writer
         * class given as parameter. The CML code will be valid CML code with a
         * XML header. Only one object can be stored.
         *
         * @param writer Writer to redirect the output to.
         */
        public CMLWriter(TextWriter writer)
        {
            // Stream doesn't handle encoding - the serializers read/write in the same format we're okay
            Trace.TraceWarning("possible loss of encoding when using a Writer with CMLWriter");
            this.output = new SteamByWriter(writer);
            InitIOSettings();
        }

        public CMLWriter(Stream output)
        {
            this.output = output;
            InitIOSettings();
        }

        public CMLWriter()
            : this(new MemoryStream())
        { }

        public void RegisterCustomizer(ICMLCustomizer customizer)
        {
            if (customizers == null) customizers = new List<ICMLCustomizer>();

            customizers.Add(customizer);
            Trace.TraceInformation("Loaded Customizer: ", customizer.GetType().Name);
        }

        public override IResourceFormat Format => CMLFormat.Instance;

        public override void SetWriter(TextWriter writer)
        {
            // Stream doesn't handle encoding - the serializers read/write in the same format we're okay
            Trace.TraceWarning("possible loss of encoding when using a Writer with CMLWriter");
            this.output = new SteamByWriter(writer);
        }

        public override void SetWriter(Stream output)
        {
            this.output = output;
        }

        /**
         * Flushes the output and closes this object.
         */
        public override void Close()
        {
            if (output != null) output.Close();
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IAtom).IsAssignableFrom(type)) return true;
            if (typeof(IBond).IsAssignableFrom(type)) return true;
            if (typeof(ICrystal).IsAssignableFrom(type)) return true;
            if (typeof(IChemModel).IsAssignableFrom(type)) return true;
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            if (typeof(IChemSequence).IsAssignableFrom(type)) return true;
            if (typeof(IAtomContainerSet).IsAssignableFrom(type)) return true;
            if (typeof(IReactionSet).IsAssignableFrom(type)) return true;
            if (typeof(IReaction).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Serializes the IChemObject to CML and redirects it to the output Writer.
        /// </summary>
        /// <param name="obj">A Molecule of AtomContaineSet object</param>
        public override void Write(IChemObject obj)
        {
            if (!(obj is IAtomContainer) && !(obj is IAtomContainerSet)
                    && !(obj is IReaction) && !(obj is IReactionSet)
                    && !(obj is IChemSequence) && !(obj is IChemModel)
                    && !(obj is IChemFile) && !(obj is ICrystal) && !(obj is IAtom)
                    && !(obj is IBond))
            {
                throw new CDKException("Cannot write this unsupported IChemObject: " + obj.GetType().Name);
            }

            Debug.WriteLine("Writing obj in CML of type: ", obj.GetType().Name);

            CustomizeJob();

            Convertor convertor = new Convertor(cmlIds.IsSet,
                    (namespacePrefix.Setting.Length > 0) ? namespacePrefix.Setting : null);
            // adding the customizer
            if (customizers != null)
            {
                foreach (var customizer in customizers)
                {
                    convertor.RegisterCustomizer(customizer);
                }
            }

            // now convert the obj
            XElement root = null;
            if (obj is IPDBPolymer)
            {
                root = convertor.CDKPDBPolymerToCMLMolecule((IPDBPolymer)obj);
            }
            else if (obj is ICrystal)
            {
                root = convertor.CDKCrystalToCMLMolecule((ICrystal)obj);
            }
            else if (obj is IAtom)
            {
                root = convertor.CDKAtomToCMLAtom(null, (IAtom)obj);
            }
            else if (obj is IBond)
            {
                root = convertor.CDKJBondToCMLBond((IBond)obj);
            }
            else if (obj is IReaction)
            {
                root = convertor.CDKReactionToCMLReaction((IReaction)obj);
            }
            else if (obj is IReactionScheme)
            {
                root = convertor.CDKReactionSchemeToCMLReactionSchemeAndMoleculeList((IReactionScheme)obj);
            }
            else if (obj is IReactionSet)
            {
                root = convertor.CDKReactionSetToCMLReactionList((IReactionSet)obj);
            }
            else if (obj is IAtomContainerSet<IAtomContainer>)
            {
                root = convertor.CDKAtomContainerSetToCMLList((IAtomContainerSet<IAtomContainer>)obj);
            }
            else if (obj is IChemSequence)
            {
                root = convertor.CDKChemSequenceToCMLList((IChemSequence)obj);
            }
            else if (obj is IChemModel)
            {
                root = convertor.CDKChemModelToCMLList((IChemModel)obj);
            }
            else if (obj is IAtomContainer)
            {
                root = convertor.CDKAtomContainerToCMLMolecule((IAtomContainer)obj);
            }
            else if (obj is IChemFile)
            {
                root = convertor.CDKChemFileToCMLList((IChemFile)obj);
            }

            var encoding = "ISO-8859-1";
            var se = new XmlWriterSettings();
            se.Encoding = Encoding.GetEncoding(encoding);
            if (indent.IsSet)
            {
                Trace.TraceInformation("Indenting XML output");
                se.Indent = true;
                se.IndentChars = new string(' ', 2);
            }

            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            if (schemaInstanceOutput.IsSet)
            {
                root.SetAttributeValue(XNamespace.Xmlns + "xsi", xsi.NamespaceName);
                root.SetAttributeValue(xsi + "schemaLocation", "http://www.xml-cml.org/schema/cml2/core " + instanceLocation.Setting);
            }

            var de = new XDeclaration(null, encoding, null);
            XDocument doc = new XDocument(de, root);
            doc.Save(output);
        }

        private void InitIOSettings()
        {
            cmlIds = Add(new BooleanIOSetting("CMLIDs", IOSetting.Importance.Low,
                    "Should the output use CML identifiers?", "true"));

            namespacedOutput = Add(new BooleanIOSetting("NamespacedOutput", IOSetting.Importance.Low,
                    "Should the output use namespaced output?", "true"));

            namespacePrefix = Add(new StringIOSetting("NamespacePrefix", IOSetting.Importance.Low,
                    "What should the namespace prefix be? [empty is no prefix]", ""));

            schemaInstanceOutput = Add(new BooleanIOSetting("SchemaInstance", IOSetting.Importance.Low,
                    "Should the output use the Schema-Instance attribute?", "false"));

            instanceLocation = Add(new StringIOSetting("InstanceLocation", IOSetting.Importance.Low,
                    "Where is the schema found?", ""));

            indent = Add(new BooleanIOSetting("Indenting", IOSetting.Importance.Low,
                    "Should the output be indented?", "true"));

            xmlDeclaration = Add(new BooleanIOSetting("XMLDeclaration", IOSetting.Importance.Low,
                    "Should the output contain an XML declaration?", "true"));
        }

        private void CustomizeJob()
        {
            FireIOSettingQuestion(cmlIds);
            FireIOSettingQuestion(namespacedOutput);
            if (namespacedOutput.IsSet)
            {
                FireIOSettingQuestion(namespacePrefix);
            }
            FireIOSettingQuestion(schemaInstanceOutput);
            if (schemaInstanceOutput.IsSet)
            {
                FireIOSettingQuestion(instanceLocation);
            }
            FireIOSettingQuestion(indent);
            FireIOSettingQuestion(xmlDeclaration);
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
