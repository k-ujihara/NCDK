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
using System.Linq;
using System.Collections.Generic;
using NCDK.Tools.Diff.Tree;

namespace NCDK.Tools.Diff
{
    /**
     * Compares two {@link IAtom} classes.
     *
     * @author     egonw
     * @cdk.module diff
     * @cdk.githash
     */
    public class AtomDiff
    {

        /**
         * Overwrite the default public constructor because this class is not
         * supposed to be instantiated.
         */
        private AtomDiff() { }

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
            if (!(first is IAtom && second is IAtom))
            {
                return null;
            }
            IAtom firstElem = (IAtom)first;
            IAtom secondElem = (IAtom)second;
            ChemObjectDifference totalDiff = new ChemObjectDifference("AtomDiff");
            totalDiff.AddChild(IntegerDifference.Construct("H", firstElem.ImplicitHydrogenCount,
                    secondElem.ImplicitHydrogenCount));
            totalDiff
                    .AddChild(IntegerDifference.Construct("SP", firstElem.StereoParity, secondElem.StereoParity));
            totalDiff.AddChild(Point2dDifference.Construct("2D", firstElem.Point2D, secondElem.Point2D));
            totalDiff.AddChild(Point3dDifference.Construct("3D", firstElem.Point3D, secondElem.Point3D));
            totalDiff.AddChild(Point3dDifference.Construct("F3D", firstElem.FractionalPoint3D,
                    secondElem.FractionalPoint3D));
            totalDiff.AddChild(DoubleDifference.Construct("C", firstElem.Charge, secondElem.Charge));
            totalDiff.AddChild(AtomTypeDiff.Difference(first, second));
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
