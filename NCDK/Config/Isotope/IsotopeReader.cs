/* Copyright (C) 2003-2007  Egon Willighagen <egonw@users.sf.net>
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
 */
using NCDK.Util.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace NCDK.Config.Isotope
{
    /// <summary>
    /// Reader that instantiates an XML parser and customized handler to process
    /// the isotope information in the CML2 isotope data file. The Reader first
    /// tries to instantiate a JAXP XML parser available from Sun JVM 1.4.0 and
    /// later. If not found it tries the Aelfred2 parser, and as last try the
    /// Xerces parser.
    /// </summary>
    // @cdk.module  extra
    // @cdk.githash
    // @author     Egon Willighagen
    public class IsotopeReader
    {
        private Stream input;

        private IChemObjectBuilder builder;

        /// <summary>
        /// Instantiates a new reader that parses the XML from the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input"><see cref="Stream"/> with the XML source</param>
        /// <param name="builder">The <see cref="IChemObjectBuilder"/> used to create new <see cref="IIsotope"/>'s.</param>
        public IsotopeReader(Stream input, IChemObjectBuilder builder)
        {
            this.input = input;
            this.builder = builder;
        }

        /// <summary>
        /// Triggers the XML parsing of the data file and returns the read Isotopes.
        /// It turns of XML validation before parsing.
        /// </summary>
        /// <returns>a List of Isotope's. Returns an empty list is some reading error occurred.</returns>
        public IList<IIsotope> ReadIsotopes()
        {
            IList<IIsotope> isotopes = new List<IIsotope>();

            var reader = new XReader();
            IsotopeHandler handler = new IsotopeHandler(builder);
            reader.Handler = handler;
            try
            {
                var settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.None;
                settings.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
                XmlReader xreader = XmlReader.Create(input, settings);
                var doc = XDocument.Load(xreader);
                reader.Read(doc);
                isotopes = handler.Isotopes;
            }
            catch (Exception exception)
            {
                Trace.TraceError($"{exception.GetType().Name}: {exception.Message}");
                Debug.WriteLine(exception);
            }
            return isotopes;
        }
    }
}
