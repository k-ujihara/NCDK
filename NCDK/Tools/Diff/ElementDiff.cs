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
     * Compares two {@link IElement} classes.
     *
     * @author     egonw
     * @cdk.module diff
     * @cdk.githash
     */
    public class ElementDiff
    {

        /**
         * Overwrite the default public constructor because this class is not
         * supposed to be instantiated.
         */
        private ElementDiff() { }

        /**
         * Compare two {@link IChemObject} classes and return the difference as a {@link string}.
         *
         * @param first  the first of the two classes to compare
         * @param second the second of the two classes to compare
         * @return a {@link string} representation of the difference between the first and second {@link IChemObject}.
         */
        public static string Diff(IChemObject first, IChemObject second)
        {
            IDifference difference = Difference(first, second);
            if (difference == null)
            {
                return "";
            }
            else
            {
                return difference.ToString();
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
            if (!(first is IElement && second is IElement))
            {
                return null;
            }
            IElement firstElem = (IElement)first;
            IElement secondElem = (IElement)second;
            ChemObjectDifference coDiff = new ChemObjectDifference("ElementDiff");
            coDiff.AddChild(StringDifference.Construct("S", firstElem.Symbol, secondElem.Symbol));
            coDiff.AddChild(StringDifference.Construct("ID", firstElem.Id, secondElem.Id));
            coDiff.AddChild(IntegerDifference.Construct("AN", firstElem.AtomicNumber, secondElem.AtomicNumber));
            coDiff.AddChild(ChemObjectDiff.Difference(first, second));
            if (coDiff.ChildCount() > 0)
            {
                return coDiff;
            }
            else
            {
                return null;
            }
        }

    }
}