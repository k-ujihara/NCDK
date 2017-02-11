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
    /// See <a href="http://www.tripos.com/data/support/mol2.pdf">here</a>.
    /// </summary>
    // @cdk.module ioformats
    // @cdk.githash
    // @cdk.set    io-formats
    public class Mol2Format : SimpleChemFormatMatcher, IChemFormatMatcher
    {
        private static IResourceFormat myself = null;

        public Mol2Format() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new Mol2Format();
                return myself;
            }
        }

        /// <inheritdoc/>
        public override string FormatName => "Mol2 (Sybyl)";

        /// <inheritdoc/>
        public override string MIMEType => "chemical/x-mol2";

        /// <inheritdoc/>
        public override string PreferredNameExtension => NameExtensions[0];

        /// <inheritdoc/>
        public override string[] NameExtensions { get; } = new string[] { "mol2" };

        /// <inheritdoc/>
        public override string ReaderClassName => "NCDK.IO.Mol2Reader";

        /// <inheritdoc/>
        public override string WriterClassName => "NCDK.IO.Mol2Writer";

        /// <inheritdoc/>
        public override bool Matches(int lineNumber, string line)
        {
            if (line.IndexOf("<TRIPOS>") >= 0)
            {
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public override bool IsXmlBased => false;

        /// <inheritdoc/>
        public override int SupportedDataFeatures => RequiredDataFeatures | DataFeatures.HAS_2D_COORDINATES | DataFeatures.HAS_3D_COORDINATES
                    | DataFeatures.HAS_GRAPH_REPRESENTATION;

        /// <inheritdoc/>
        public override int RequiredDataFeatures => DataFeatures.HAS_ATOM_ELEMENT_SYMBOL;
    }
}
