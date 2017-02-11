/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using System;
using System.Linq;
using System.Collections.Generic;
using NCDK.Common.Primitives;

namespace NCDK.IO.Formats
{
    /// <summary>
    /// This interface is used for classes that are able to match a certain
    /// chemical file format. For example: Chemical Markup Language, PDB etc.
    /// </summary>
    // @cdk.module ioformats
    // @cdk.githash
    // @author      Egon Willighagen <egonw@sci.kun.nl>
    // @cdk.created 2004-10-25
    public interface IChemFormatMatcher : IChemFormat
    {
        /// <summary>
        /// Method that checks whether the given lines are part of the format read by
        /// this reader.
        /// </summary>
        /// <param name="lines">lines of the input to be checked</param>
        /// <returns>whether the format matched and when it matched</returns>
        MatchResult Matches(IList<string> lines);
    }

    /// <summary>
    /// Simple class holds whether a format matcher matched, when it matched and
    /// what the format was. The result is comparable to be prioritised (lower
    /// match position being favoured).
   /// </summary>
    public sealed class MatchResult : IComparable<MatchResult>
    {
        /// <summary>Convenience method for indicating a format did not match.</summary>
        public static MatchResult NO_MATCH = new MatchResult(false, null, int.MaxValue);

        /// <summary>Did the format match.</summary>
        private readonly bool matched;

        /// <summary>When did the format match.</summary>
        private readonly int position;

        /// <summary>Which format matched.</summary>
        private readonly IChemFormat format;

        public MatchResult(bool matched, IChemFormat format, int position)
        {
            this.matched = matched;
            this.format = format;
            this.position = position;
        }

        /// <summary>
        /// Did the chem format match.
        /// </summary>
        /// <returns>whether the format matched</returns>
        public bool IsMatched => matched;

        /// <summary>
        /// What was the format which matched if there was a match <see cref="IsMatched"/>.
        /// </summary>
        /// <returns>the format which matched</returns>
        /// <exception cref="ArgumentException">there was no match</exception>
        public IChemFormat Format
        {
            get
            {
                if (!matched) throw new ArgumentException("result did not match");
                return format;
            }
        }

        /// <summary>
        /// Compares the match result with another, results with lower position
        /// are ordered before those with higher position.
       /// </summary>
        public int CompareTo(MatchResult that)
        {
            return Ints.Compare(this.position, that.position);
        }
    }
}
