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
    /**
     * {@link IDifference} between two {@link string}.
     *
     * @author     egonw
     * @cdk.module diff
     * @cdk.githash
     */
    public class StringDifference : IDifference
    {

        private string name;
        private string first;
        private string second;

        private StringDifference(string name, string first, string second)
        {
            this.name = name;
            this.first = first;
            this.second = second;
        }

        /**
         * Constructs a new {@link IDifference} object.
         *
         * @param name   a name reflecting the nature of the created {@link IDifference}
         * @param first  the first object to compare
         * @param second the second object to compare
         * @return       an {@link IDifference} reflecting the differences between the first and second object
         */
        public static IDifference Construct(string name, string first, string second)
        {
            if (first == null && second == null)
            {
                return null; // no difference
            }
            if (first == null || second == null)
            {
                return new StringDifference(name, first, second);
            }
            if (first.Equals(second))
            {
                return null; // no difference
            }
            return new StringDifference(name, first, second);
        }

        /**
         * Returns a {@link string} representation for this {@link IDifference}.
         *
         * @return a {@link string}
         */

        public override string ToString()
        {
            return name + ":" + (first == null ? "NA" : first) + "/" + (second == null ? "NA" : second);
        }
    }
}