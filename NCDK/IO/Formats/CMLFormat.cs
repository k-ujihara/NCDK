/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Tools;

namespace NCDK.IO.Formats
{
    /// <summary>
    /// See <see href="http://en.wikipedia.org/wiki/Chemical_Markup_Language">here</see>.
    /// </summary>
    // @cdk.module ioformats
    // @cdk.githash
    public class CMLFormat : SimpleChemFormatMatcher, IChemFormatMatcher
    {
        private static IResourceFormat myself = null;

        public CMLFormat() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new CMLFormat();
                return myself;
            }
        }

        /// <inheritdoc/>
        public override string FormatName => "Chemical Markup Language";

        /// <inheritdoc/>
        public override string MIMEType => "chemical/x-cml";

        /// <inheritdoc/>
        public override string PreferredNameExtension => NameExtensions[0];

        /// <inheritdoc/>
        public override string[] NameExtensions { get; } = new string[] { "cml", "xml" };

        /// <inheritdoc/>
        public override string ReaderClassName => "NCDK.IO.CMLReader";

        /// <inheritdoc/>
        public override string WriterClassName => "NCDK.IO.CMLWriter";

        /// <inheritdoc/>
        public override bool Matches(int lineNumber, string line)
        {
            if ((line.IndexOf("http://www.xml-cml.org/schema") != -1) || (line.IndexOf("<atom") != -1)
                    || (line.IndexOf("<molecule") != -1) || (line.IndexOf("<reaction") != -1)
                    || (line.IndexOf("<cml") != -1) || (line.IndexOf("<bond") != -1))
            {
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public override bool IsXmlBased => true;

        /// <inheritdoc/>
        public override int SupportedDataFeatures => DataFeatures.HAS_2D_COORDINATES | DataFeatures.HAS_3D_COORDINATES
                    | DataFeatures.HAS_ATOM_PARTIAL_CHARGES | DataFeatures.HAS_ATOM_FORMAL_CHARGES
                    | DataFeatures.HAS_ATOM_MASS_NUMBERS | DataFeatures.HAS_ATOM_ISOTOPE_NUMBERS
                    | DataFeatures.HAS_GRAPH_REPRESENTATION | DataFeatures.HAS_ATOM_ELEMENT_SYMBOL;

        /// <inheritdoc/>
        public override int RequiredDataFeatures => DataFeatures.None;
    }
}
