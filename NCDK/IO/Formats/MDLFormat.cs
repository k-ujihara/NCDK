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
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NCDK.Tools;
using NCDK.Common.Primitives;

namespace NCDK.IO.Formats
{
    /// </summary>
    /// <summary>
    /// See <a href="http://www.mdl.com/downloads/public/ctfile/ctfile.jsp">here</a>.
    /// </summary>
    // @cdk.module ioformats
    // @cdk.githash
    // @cdk.set    io-formats
    public class MDLFormat : SimpleChemFormatMatcher, IChemFormatMatcher
    {
        private static IResourceFormat myself = null;

        public MDLFormat() { }

        public static IResourceFormat Instance
        {
            get
            {
                if (myself == null) myself = new MDLFormat();
                return myself;
            }
        }

        /// <inheritdoc/>
        public override string FormatName => "MDL Molfile";

        /// <inheritdoc/>
        public override string MIMEType => "chemical/x-mdl-molfile";

        /// <inheritdoc/>
        public override string PreferredNameExtension => NameExtensions[0];

        /// <inheritdoc/>
        public override string[] NameExtensions { get; } = new string[] { "mol" };

        /// <inheritdoc/>
        public override string ReaderClassName => typeof(NCDK.IO.MDLReader).ToString();

        /// <inheritdoc/>
        public override string WriterClassName => null;

        /// <inheritdoc/>
        public override bool Matches(int lineNumber, string line)
        {
            if (lineNumber == 4 && line.Length > 7 && (line.IndexOf("2000") == -1) && // MDL Mol V2000 format
                    (line.IndexOf("3000") == -1)) // MDL Mol V3000 format
            {
                // possibly a MDL mol file
                try
                {
                    string atomCountString = Strings.Substring(line, 0, 3).Trim();
                    string bondCountString = Strings.Substring(line, 3, 3).Trim();

                    int.Parse(atomCountString);
                    int.Parse(bondCountString);
                    if (line.Length > 6)
                    {
                        string remainder = Strings.Substring(line, 6).Trim();
                        for (int i = 0; i < remainder.Length; ++i)
                        {
                            char c = remainder[i];
                            if (!(char.IsDigit(c) || char.IsWhiteSpace(c)))
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (FormatException)
                {
                    // Integers not found on fourth line; therefore not a MDL file
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public override bool IsXmlBased => false;

        /// <inheritdoc/>
        public override int SupportedDataFeatures
                => RequiredDataFeatures | DataFeatures.HAS_2D_COORDINATES | DataFeatures.HAS_3D_COORDINATES
                    | DataFeatures.HAS_GRAPH_REPRESENTATION;

        /// <inheritdoc/>
        public override int RequiredDataFeatures
            => DataFeatures.HAS_ATOM_ELEMENT_SYMBOL;
    }
}
