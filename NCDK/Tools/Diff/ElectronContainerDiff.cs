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
     * Compares two {@link IChemObject} classes.
     *
     * @author     egonw
     * @cdk.module diff
     * @cdk.githash
     */
    public class ElectronContainerDiff
    {

        /**
         * Overwrite the default public constructor because this class is not
         * supposed to be instantiated.
         */
        private ElectronContainerDiff() { }

        /**
         * Compare two {@link IChemObject} classes and return the difference as a {@link string}.
         *
         * @param first  the first of the two classes to compare
         * @param second the second of the two classes to compare
         * @return a {@link string} representation of the difference between the first and second {@link IChemObject}.
         */
        public static string Diff(IChemObject first, IChemObject second)
        {
            IDifference diff = Difference(first, second);
            if (diff == null)
            {
                return "";
            }
            else
            {
                return diff.ToString();
            }
        }

        /**
         * Compare two {@link IChemObject} classes and return the difference as an {@link IDifference}.
         *
         * @param first  the first of the two classes to compare
         * @param second the second of the two classes to compare
         * @return an {@link IDifference} representation of the difference between the first and second {@link IChemObject}.
         */
        public static IDifference Difference(IChemObject first, IChemObject second)
        {
            if (!(first is IElectronContainer && second is IElectronContainer))
            {
                return null;
            }
            IElectronContainer firstEC = (IElectronContainer)first;
            IElectronContainer secondEC = (IElectronContainer)second;
            IDifferenceList totalDiff = new ChemObjectDifference("ElectronContainerDiff");
            totalDiff.AddChild(IntegerDifference.Construct("eCount", firstEC.ElectronCount,
                    secondEC.ElectronCount));
            totalDiff.AddChild(ChemObjectDiff.Difference(first, second));
            if (totalDiff.ChildCount() > 0)
            {
                return totalDiff;
            }
            else
            {
                return null;
            }
        }

    }
}
