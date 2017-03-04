/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools.Diff.Tree
{
    /// <summary>
    /// <see cref="IDifference"/> between two {@link int?}.
    ///
    // @author     egonw
    // @cdk.module diff
    // @cdk.githash
    /// </summary>
    public class IntegerDifference : IDifference
    {

        private string name;
        private int? first;
        private int? second;

        private IntegerDifference(string name, int? first, int? second)
        {
            this.name = name;
            this.first = first;
            this.second = second;
        }

        /// <summary>
        /// Constructs a new <see cref="IDifference"/> object.
        ///
        /// <param name="name">a name reflecting the nature of the created <see cref="IDifference"/></param>
        /// <param name="first">the first object to compare</param>
        /// <param name="second">the second object to compare</param>
        /// <returns>an <see cref="IDifference"/> reflecting the differences between the first and second object</returns>
        /// </summary>
        public static IDifference Construct(string name, int? first, int? second)
        {
            if (first == null && second == null)
            {
                return null; // no difference
            }
            if (first == null || second == null)
            {
                return new IntegerDifference(name, first, second);
            }
            if (first.Value == second.Value)
            {
                return null; // no difference
            }
            return new IntegerDifference(name, first, second);
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation for this <see cref="IDifference"/>.
        ///
        /// <returns>a <see cref="string"/></returns>
        /// </summary>

        public override string ToString()
        {
            return name + ":" + (first == null ? "NA" : first.ToString()) + "/" + (second == null ? "NA" : second.ToString());
        }
    }
}