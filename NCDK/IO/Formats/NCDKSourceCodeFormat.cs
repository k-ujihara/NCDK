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
    // @cdk.module ioformats
    // @cdk.githash
    // @cdk.set    io-formats
    public class CDKSourceCodeFormat : AbstractResourceFormat, IChemFormat
    {
        private static IResourceFormat myself = null;

        public CDKSourceCodeFormat() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new CDKSourceCodeFormat();
                return myself;
            }
        }

        public override string FormatName => "NCDK Source Code";
        public override string MIMEType => null;
        public override string PreferredNameExtension => NameExtensions[0];
        public override string[] NameExtensions { get; } = new string[] { "cs" };
        public string ReaderClassName => null;
        public string WriterClassName => "NCDK.IO.NCDKSourceCodeWriter";
        public override bool IsXmlBased => false;
        public int SupportedDataFeatures => RequiredDataFeatures | DataFeatures.HAS_GRAPH_REPRESENTATION;
        public int RequiredDataFeatures => DataFeatures.HAS_ATOM_ELEMENT_SYMBOL;
    }
}

