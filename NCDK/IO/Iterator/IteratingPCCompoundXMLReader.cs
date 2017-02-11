/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.IO.PubChemXml;
using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.IO.Iterator
{
    /// <summary>
    /// Iterating PubChem PCCompound ASN.1 XML reader.
    /// </summary>
    // @cdk.module io
    // @cdk.githash
    // @cdk.iooptions
    ///
    // @see org.openscience.cdk.io.PCCompoundASNReader
    ///
    // @author       Egon Willighagen <egonw@users.sf.net>
    // @cdk.created  2008-05-05
    ///
    // @cdk.keyword  file format, ASN
    // @cdk.keyword  PubChem
    public class IteratingPCCompoundXMLReader : DefaultIteratingChemObjectReader<IAtomContainer>
    {
        private TextReader primarySource;
        private XElement parser;
        private PubChemXMLHelper parserHelper;
        private IChemObjectBuilder builder;

        /// <summary>
        /// Constructs a new IteratingPCCompoundXMLReader that can read Molecule from a given Reader and IChemObjectBuilder.
        /// </summary>
        /// <param name="in_">The input stream</param>
        /// <param name="builder">The builder</param>
        /// <exception cref="Exception">if there is an error isn setting up the XML parser</exception>
        public IteratingPCCompoundXMLReader(TextReader in_, IChemObjectBuilder builder)
        {
            this.builder = builder;
            parserHelper = new PubChemXMLHelper(builder);

            primarySource = in_;
            parser = XDocument.Load(in_).Root;
        }

        /// <summary>
        /// Constructs a new IteratingPCCompoundXLReader that can read Molecule from a given Stream and IChemObjectBuilder.
        /// </summary>
        /// <param name="in_">The input stream</param>
        /// <param name="builder">The builder. In general, use <see cref="Default.ChemObjectBuilder"/></param>
        /// <exception cref="Exception">if there is a problem creating an InputStreamReader</exception>
        public IteratingPCCompoundXMLReader(Stream in_, IChemObjectBuilder builder)
            : this(new StreamReader(in_), builder)
        { }

        public override IResourceFormat Format => PubChemCompoundsXMLFormat.Instance;

        public override IEnumerator<IAtomContainer> GetEnumerator()
        {
            Debug.WriteLine($"start: '{parser.Name}'");
            foreach (var elm in parser.Elements(PubChemXMLHelper.Name_EL_PCCOMPOUND))
            {
                var molecule = parserHelper.ParseMolecule(elm, builder);
                yield return molecule;
            }
            yield break;
        }

        public override void Close()
        {
            primarySource.Close();
        }

        public override void Dispose()
        {
            Close();
        }

        public override void SetReader(TextReader reader)
        {
            primarySource = reader;
            try
            {
                parser = XDocument.Load(primarySource).Root;
            }
            catch (Exception e)
            {
                throw new CDKException("Error while opening the input:" + e.Message, e);
            }
        }

        public override void SetReader(Stream reader)
        {
            SetReader(new StreamReader(reader));
        }
    }
}
