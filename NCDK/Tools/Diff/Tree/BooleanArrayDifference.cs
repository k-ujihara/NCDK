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
using System.Text;

namespace NCDK.Tools.Diff.Tree
{
    /**
     * Difference between two bool[]'s.
     *
     * @author     egonw
     * @cdk.module diff
     * @cdk.githash
     */
    public class BooleanArrayDifference
        : AbstractDifferenceList, IDifferenceList
    {
        private string name;

        private BooleanArrayDifference(string name)
        {
            this.name = name;
        }

        /**
         * Constructs a new {@link IDifference} object.
         *
         * @param name   a name reflecting the nature of the created {@link IDifference}
         * @param first  the first object to compare
         * @param second the second object to compare
         * @return       an {@link IDifference} reflecting the differences between the first and second object
         */
        public static IDifference Construct(string name, bool[] first, bool[] second)
        {
            if (first == null && second == null) return null;

            BooleanArrayDifference totalDiff = new BooleanArrayDifference(name);
            int firstLength = first == null ? 0 : first.Length;
            int secondLength = second == null ? 0 : second.Length;
            if (firstLength == secondLength)
            {
                for (int i = 0; i < firstLength; i++)
                {
                    totalDiff.AddChild(BooleanDifference.Construct("" + i, first[i], second[i]));
                }
            }
            else if (firstLength < secondLength)
            {
                for (int i = 0; i < firstLength; i++)
                {
                    totalDiff.AddChild(BooleanDifference.Construct("" + i, first[i], second[i]));
                }
                for (int i = firstLength; i < secondLength; i++)
                {
                    totalDiff.AddChild(BooleanDifference.Construct("" + i, null, second[i]));
                }
            }
            else
            { // secondLength < firstLength
                for (int i = 0; i < secondLength; i++)
                {
                    totalDiff.AddChild(BooleanDifference.Construct("" + i, first[i], second[i]));
                }
                for (int i = secondLength; i < firstLength; i++)
                {
                    totalDiff.AddChild(BooleanDifference.Construct("" + i, first[i], null));
                }
            }
            if (totalDiff.ChildCount() == 0)
            {
                return null;
            }
            return totalDiff;
        }

        /**
         * Returns a {@link string} representation for this {@link IDifference}.
         *
         * @return a {@link string}
         */
        public override string ToString()
        {
            if (differences.Count() == 0) return "";

            var diffBuffer = new StringBuilder();
            diffBuffer.Append(this.name).Append('{');
            IEnumerable<IDifference> children = GetChildren();
            bool isFirst = true;
            foreach (var child in children)
            {
                if (isFirst)
                    isFirst = false;
                else
                    diffBuffer.Append(", ");
                diffBuffer.Append(child.ToString());
            }
            diffBuffer.Append('}');

            return diffBuffer.ToString();
        }
    }
}
