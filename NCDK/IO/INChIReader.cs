/* Copyright (C) 2002-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.IO.Formats;
using System.IO;
using System;
using System.Xml;
using System.Xml.Schema;
using NCDK.IO.InChI;
using System.Diagnostics;
using System.Xml.Linq;
using NCDK.Utils.Xml;

namespace NCDK.IO
{
    /// <summary>
    /// Reads the content of a IUPAC/NIST Chemical Identifier (INChI) document. See
    /// <token>cdk-cite-HEL01</token>. Recently a new INChI format was introduced an files generated
    /// with the latest INChI generator cannot be parsed with this class. This class
    /// needs to be updated.
    /// <para>The elements that are read are given in the INChIHandler class.</para>
    /// </summary>
    /// <seealso cref="InChIHandler"/>
    // @cdk.module extra
    // @cdk.githash
    // @cdk.iooptions
    // @author      Egon Willighagen <egonw@sci.kun.nl>
    // @cdk.created 2004-05-17
    // @cdk.keyword file format, INChI
    // @cdk.keyword chemical identifier
    // @cdk.require java1.4+
    public class InChIReader : DefaultChemObjectReader
    {
        private Stream input;

        /// <summary>
        /// Construct a INChI reader from a Stream object.
        /// </summary>
        /// <param name="input">the Stream with the content</param>
        public InChIReader(Stream input)
        {
            this.input = input;
        }

        public InChIReader()
            : this(new MemoryStream(new byte[0]))
        { }

        public override IResourceFormat Format => InChIFormat.Instance;

        /// <summary>
        /// This method must not be used; XML reading requires the use of an Stream.
        /// Use SetReader(Stream) instead.
        /// </summary>
        public override void SetReader(TextReader reader)
        {
            throw new CDKException("Invalid method call; use SetReader(Stream) instead.");
        }

        public override void SetReader(Stream input)
        {
            this.input = input;
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Reads a IChemObject of type object from input.
        /// Supported types are: ChemFile.
        /// </summary>
        /// <param name="obj">type of requested IChemObject</param>
        /// <returns>the content in a ChemFile object</returns>
        public override T Read<T>(T obj)
        {
            if (obj is IChemFile)
            {
                return (T)ReadChemFile();
            }
            else
            {
                throw new CDKException("Only supported is reading of ChemFile objects.");
            }
        }

        // private functions

        /// <summary>
        /// Reads a ChemFile object from input.
        /// </summary>
        /// <returns>ChemFile with the content read from the input</returns>
        private IChemFile ReadChemFile()
        {
            IChemFile cf = null;
            XmlReaderSettings setting = new XmlReaderSettings();
            setting.ValidationFlags = XmlSchemaValidationFlags.None;

            InChIHandler handler = new InChIHandler();

            try
            {
                var r = new XReader();
                r.Handler = handler;
                XDocument doc = XDocument.Load(input);
                r.Read(doc);
                cf = handler.ChemFile;
            }
            catch (IOException e)
            {
                Trace.TraceError("IOException: ", e.Message);
                Debug.WriteLine(e);
            }
            catch (XmlException saxe)
            {
                Trace.TraceError("SAXException: ", saxe.Message);
                Debug.WriteLine(saxe);
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
