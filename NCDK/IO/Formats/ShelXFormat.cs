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
    /// See <see href="http://shelx.uni-ac.gwdg.de/SHELX/">here</see>.
    /// </summary>
    // @cdk.module ioformats
    // @cdk.githash
    public class ShelXFormat : SimpleChemFormatMatcher, IChemFormatMatcher
    {
        private static IResourceFormat myself = null;

        public ShelXFormat() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new ShelXFormat();
                return myself;
            }
        }

        public override string FormatName => "ShelXL";
        public override string MIMEType => "chemical/x-shelx";
        public override string PreferredNameExtension => NameExtensions[0];
        public override string[] NameExtensions { get; } = new string[] { "ins", "res" };
        public override string ReaderClassName => "NCDK.IO.ShelXReader";
        public override string WriterClassName => "NCDK.IO.ShelXWriter";

        public override bool Matches(int lineNumber, string line)
        {
            if (line.StartsWith("ZERR ") || line.StartsWith("TITL "))
            {
                return true;
            }
            return false;
        }

        public override bool IsXmlBased => false;
        public override int SupportedDataFeatures => RequiredDataFeatures;
        public override int RequiredDataFeatures =>
                DataFeatures.HAS_3D_COORDINATES | DataFeatures.HAS_UNITCELL_PARAMETERS
                    | DataFeatures.HAS_ATOM_ELEMENT_SYMBOL;
    }
}
