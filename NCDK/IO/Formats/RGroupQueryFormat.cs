/*
 * Copyright (C) 2010  Mark Rijnbeek <mark_rynbeek@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may
 * distribute with programs based on this work.
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
using NCDK.Tools;

namespace NCDK.IO.Formats
{
    /// <summary>
    /// Format for Symyx RGfiles (Rgroup query files).
    /// </summary>
    // @cdk.module ioformats
    // @cdk.githash
    // @cdk.set    io-formats
    public class RGroupQueryFormat : SimpleChemFormatMatcher, IChemFormatMatcher
    {
        private static IResourceFormat myself = null;

        public RGroupQueryFormat() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new RGroupQueryFormat();
                return myself;
            }
        }

        public override string FormatName => "Symyx Rgroup query files";
        public override string MIMEType => null;
        public override string PreferredNameExtension => NameExtensions[0];
        public override string[] NameExtensions { get; } = new string[] { "mol", "rgp" };
        public override string ReaderClassName => "NCDK.IO.RGroupQueryReader";
        public override string WriterClassName => "NCDK.IO.RGroupQueryWriter";

        public override bool Matches(int lineNumber, string line)
        {
            if (line.IndexOf("$RGP") >= 0)
                return true;
            else
                return false;
        }

        public override bool IsXmlBased => false;
        public override int SupportedDataFeatures => RequiredDataFeatures | DataFeatures.HAS_2D_COORDINATES;
        public override int RequiredDataFeatures => DataFeatures.HAS_ATOM_ELEMENT_SYMBOL;
    }
}
