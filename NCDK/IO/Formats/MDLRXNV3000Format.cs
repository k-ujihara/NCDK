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
using System.Collections.Generic;

namespace NCDK.IO.Formats
{
    /// <summary>
    /// See <see href="http://www.mdl.com/downloads/public/ctfile/ctfile.jsp">here</see>.
    /// </summary>
    // @cdk.module ioformats
    // @cdk.githash
    public class MDLRXNV3000Format : AbstractResourceFormat, IChemFormatMatcher
    {
        private static IResourceFormat myself = null;

        public MDLRXNV3000Format() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new MDLRXNV3000Format();
                return myself;
            }
        }

        /// <inheritdoc/>
        public override string FormatName => "MDL RXN V3000";

        /// <inheritdoc/>
        public override string MIMEType => "chemical/x-mdl-rxnfile";

        /// <inheritdoc/>
        public override string PreferredNameExtension => NameExtensions[0];

        /// <inheritdoc/>
        public override string[] NameExtensions { get; } = new string[] { "rxn" };

        /// <inheritdoc/>
        public string ReaderClassName { get; } = typeof(MDLRXNV3000Reader).FullName;

        /// <inheritdoc/>
        public string WriterClassName => null;

        /// <inheritdoc/>
        public MatchResult Matches(IList<string> lines)
        {
            // if the first line doesn't have '$RXN' then it can't match
            if (lines.Count < 1 || !lines[0].StartsWith("$RXN V3000"))
                return MatchResult.NO_MATCH;

            // check the header (fifth line)
            string header = lines.Count > 4 ? lines[4] : "";

            if (!header.StartsWith("M  V30 "))
                return MatchResult.NO_MATCH;

            return new MatchResult(true, this, 0);
        }

        /// <inheritdoc/>
        public override bool IsXmlBased => false;

        /// <inheritdoc/>
        public DataFeatures SupportedDataFeatures => DataFeatures.None;

        /// <inheritdoc/>
        public DataFeatures RequiredDataFeatures => DataFeatures.None;
    }
}
