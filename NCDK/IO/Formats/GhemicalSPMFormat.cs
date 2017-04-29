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
using System;

namespace NCDK.IO.Formats
{
    // @cdk.module ioformats
    // @cdk.githash
    public class GhemicalSPMFormat : SimpleChemFormatMatcher, IChemFormatMatcher
    {
        private static IResourceFormat myself = null;

        public GhemicalSPMFormat() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new GhemicalSPMFormat();
                return myself;
            }
        }

        public override string FormatName => "Ghemical Simplified Protein Model";
        public override string MIMEType => null;
        public override string PreferredNameExtension => null;
        public override string[] NameExtensions => Array.Empty<string>();
        public override string ReaderClassName => "NCDK.IO.GhemicalMMReader";
        public override string WriterClassName => null;

        public override bool Matches(int lineNumber, string line)
        {
            if (line.StartsWith("!Header mm1gp"))
            {
                return true;
            }
            return false;
        }

        public override bool IsXmlBased => false;
        public override int SupportedDataFeatures => DataFeatures.None;
        public override int RequiredDataFeatures => DataFeatures.None;
    }
}
