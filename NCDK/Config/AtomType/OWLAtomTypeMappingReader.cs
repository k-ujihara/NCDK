/* Copyright (C) 2002-2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Util.Xml;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace NCDK.Config.AtomType
{
    /// <summary>
    /// XML Reader for the <see cref="OWLBasedAtomTypeConfigurator"/>.
    /// </summary>
    // @cdk.module  atomtype
    // @cdk.githash
    public class OWLAtomTypeMappingReader
    {
        private TextReader input;

        /// <summary>
        /// Instantiates the XML based AtomTypeReader.
        /// </summary>
        /// <param name="input">The Reader to read the IAtomType's from.</param>
        public OWLAtomTypeMappingReader(TextReader input)
        {
            this.input = input;
        }

        /// <summary>
        /// Reads the atom type mappings from the data file.
        /// </summary>
        /// <returns>a <see cref="IDictionary{string, string}"/> with atom type mappings. <see langword="null"/>, if some reading error occurred.</returns>
        public IDictionary<string, string> ReadAtomTypeMappings()
        {
            IDictionary<string, string> mappings = null;

            XmlReaderSettings setting = new XmlReaderSettings();
            setting.DtdProcessing = DtdProcessing.Parse;
            setting.ValidationFlags = XmlSchemaValidationFlags.None;
            OWLAtomTypeMappingHandler handler = new OWLAtomTypeMappingHandler();
            XmlReader parser = XmlReader.Create(input, setting);
            var reader = new XReader();
            reader.Handler = handler;
            try
            {
                XDocument doc = XDocument.Load(parser);
                reader.Read(doc);
                mappings = handler.GetAtomTypeMappings();
            }
            catch (IOException exception)
            {
                Trace.TraceError(nameof(IOException) + ": " + exception.Message);
                Debug.WriteLine(exception);
            }
            catch (XmlException saxe)
            {
                Trace.TraceError(nameof(XmlException) + ": " + saxe.Message);
                Debug.WriteLine(saxe);
            }
            return mappings == null ? new Dictionary<string, string>() : mappings;
        }
    }
}
