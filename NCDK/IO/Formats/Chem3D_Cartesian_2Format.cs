/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
    // @author Miguel Rojas
    // @cdk.module ioformats
    // @cdk.githash
    // @cdk.set    io-formats
    public class Chem3D_Cartesian_2Format : AbstractResourceFormat, IChemFormat
    {
        private static IResourceFormat myself = null;

        public Chem3D_Cartesian_2Format() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new Chem3D_Cartesian_2Format();
                return myself;
            }
        }

        public override string FormatName => "Chem3D Cartesian 2";
        public override string MIMEType => null;
        public override string PreferredNameExtension => NameExtensions[0];
        public override string[] NameExtensions { get; } = new string[] { "c3d2" };
        public string ReaderClassName => null;
        public string WriterClassName => null;
        public override bool IsXmlBased => false;
        public int SupportedDataFeatures => DataFeatures.None;
        public int RequiredDataFeatures => DataFeatures.None;
    }
}
