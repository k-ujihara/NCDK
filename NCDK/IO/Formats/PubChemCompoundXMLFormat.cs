/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
using System.Collections.Generic;

namespace NCDK.IO.Formats
{
    // @cdk.module ioformats
    // @cdk.githash
    // @cdk.set     io-formats
    public class PubChemCompoundXMLFormat : AbstractResourceFormat, IChemFormatMatcher
    {
        private static IResourceFormat myself = null;

        public PubChemCompoundXMLFormat() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new PubChemCompoundXMLFormat();
                return myself;
            }
        }

        public override string FormatName => "PubChem Compound XML";
        public override string MIMEType => null;
        public override string PreferredNameExtension => NameExtensions[0];
        public override string[] NameExtensions { get; } = new string[] { "xml" };
        public string ReaderClassName => "NCDK.IO.PCCompoundXMLReader";
        public string WriterClassName => null;
        public override bool IsXmlBased => true;
        public int SupportedDataFeatures => DataFeatures.None;
        public int RequiredDataFeatures => DataFeatures.None;

        public MatchResult Matches(IList<string> lines)
        {
            MatchResult result = MatchResult.NO_MATCH;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (line.Contains("<PC-Compound") && result == MatchResult.NO_MATCH) result = new MatchResult(true, this, i);
                if (line.Contains("<PC-Compounds")) return MatchResult.NO_MATCH;
            }
            return result;
        }
    }
}
