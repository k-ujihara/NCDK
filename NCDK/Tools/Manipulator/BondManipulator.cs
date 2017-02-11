/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 *  */
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    /**
     * Class with convenience methods that provide methods to manipulate
     * AtomContainer's. For example:
     * <pre>
     * AtomContainerManipulator.replaceAtomByAtom(container, atom1, atom2);
     * </pre>
     * will replace the Atom in the AtomContainer, but in all the ElectronContainer's
     * it participates too.
     *
     * @cdk.module  core
     * @cdk.githash
     *
     * @author  Egon Willighagen
     * @cdk.created 2003-08-07
     */
    public class BondManipulator
    {

        /**
         * Constructs an array of Atom objects from Bond.
         * @param  container The Bond object.
         * @return The array of Atom objects.
         */
        public static IAtom[] GetAtomArray(IBond container)
        {
            IAtom[] ret = new IAtom[container.Atoms.Count];
            for (int i = 0; i < ret.Length; ++i)
                ret[i] = container.Atoms[i];
            return ret;
        }

        /**
         * Returns true if the first bond has a lower bond order than the second bond.
         * It returns false if the bond order is equal, and if the order of the first
         * bond is larger than that of the second. Also returns false if either bond
         * order is unset.
         *
         * @param first  The first bond order object
         * @param second The second bond order object
         * @return true if the first bond order is lower than the second one, false otherwise
         * @see #IsHigherOrder(BondOrder, BondOrder)
         */
        public static bool IsLowerOrder(BondOrder first, BondOrder second)
        {
            if (first == BondOrder.Unset || second == BondOrder.Unset) return false;
            return first.CompareTo(second) < 0;
        }

        /**
         * Returns true if the first bond has a higher bond order than the second bond.
         * It returns false if the bond order is equal, and if the order of the first
         * bond is lower than that of the second. Also returns false if either bond
         * order is unset.
         *
         * @param first  The first bond order object
         * @param second  The second bond order object
         * @return true if the first bond order is higher than the second one, false otherwise
         * @see #IsLowerOrder(BondOrder, BondOrder)
         */
        public static bool IsHigherOrder(BondOrder first, BondOrder second)
        {
            if (first == BondOrder.Unset || second == BondOrder.Unset) return false;
            return first.CompareTo(second) > 0;
        }

        /**
         * Returns the BondOrder one higher. Does not increase the bond order
         * beyond the Quadruple bond order.
         * @param oldOrder the old order
         * @return The incremented bond order
         * @see #IncreaseBondOrder(IBond)
         * @see #DecreaseBondOrder(BondOrder)
         * @see #DecreaseBondOrder(IBond)
         */
        public static BondOrder IncreaseBondOrder(BondOrder oldOrder)
        {
            if (oldOrder == BondOrder.Single) return BondOrder.Double;
            if (oldOrder == BondOrder.Double) return BondOrder.Triple;
            if (oldOrder == BondOrder.Triple) return BondOrder.Quadruple;
            if (oldOrder == BondOrder.Quadruple) return BondOrder.Quintuple;
            if (oldOrder == BondOrder.Quintuple) return BondOrder.Sextuple;
            return oldOrder;
        }

        /**
         * Increment the bond order of this bond.
         *
         * @param bond  The bond whose order is to be incremented
         * @see #IncreaseBondOrder(BondOrder)
         * @see #DecreaseBondOrder(BondOrder)
         * @see #DecreaseBondOrder(IBond)
         */
        public static void IncreaseBondOrder(IBond bond)
        {
            bond.Order = IncreaseBondOrder(bond.Order);
        }

        /**
         * Returns the BondOrder one lower. Does not decrease the bond order
         * lower the Quadruple bond order.
         * @param oldOrder the old order
         * @return the decremented order
         * @see #DecreaseBondOrder(IBond)
         * @see #IncreaseBondOrder(BondOrder)
         * @see #IncreaseBondOrder(BondOrder)
         */
        public static BondOrder DecreaseBondOrder(BondOrder oldOrder)
        {
            if (oldOrder == BondOrder.Sextuple) return BondOrder.Quintuple;
            if (oldOrder == BondOrder.Quintuple) return BondOrder.Quadruple;
            if (oldOrder == BondOrder.Quadruple) return BondOrder.Triple;
            if (oldOrder == BondOrder.Triple) return BondOrder.Double;
            if (oldOrder == BondOrder.Double) return BondOrder.Single;
            return oldOrder;
        }

        /**
         * Decrease the order of a bond.
         *
         * @param bond  The bond in question
         * @see #DecreaseBondOrder(BondOrder)
         * @see #IncreaseBondOrder(BondOrder)
         * @see #IncreaseBondOrder(BondOrder)
         */
        public static void DecreaseBondOrder(IBond bond)
        {
            bond.Order = DecreaseBondOrder(bond.Order);
        }

        /**
         * Convenience method to convert a double into an BondOrder.
         * Returns NULL if the bond order is not 1.0, 2.0, 3.0 and 4.0.
         * @param bondOrder The numerical bond order
         * @return An instance of {@link BondOrder}
         * @see #DestroyBondOrder(BondOrder)
         */
        public static BondOrder CreateBondOrder(double bondOrder)
        {
            foreach (var order in BondOrder.Values)
            {
                if (order.Numeric == bondOrder) return order;
            }
            return BondOrder.Unset;
        }

        /**
         * Convert a {@link BondOrder} to a numeric value.
         *
         * Single, double, triple and quadruple bonds are converted to 1.0, 2.0, 3.0
         * and 4.0 respectively.
         *
         * @param bondOrder The bond order object
         * @return  The numeric value
         * @see #CreateBondOrder(double)
         * 
         * @deprecated use <code>BondOrder.Numeric.Value</code> instead
         */
        public static double DestroyBondOrder(BondOrder bondOrder)
        {
            return bondOrder.Numeric;
        }

        /**
         * Returns the maximum bond order for a List of bonds, given an iterator to the list.
         * @param bonds An iterator for the list of bonds
         * @return The maximum bond order found
         * @see #GetMaximumBondOrder(IList)
         */
        public static BondOrder GetMaximumBondOrder(IEnumerable<IBond> bonds)
        {
            BondOrder maxOrder = BondOrder.Single;
            foreach (var bond in bonds)
            {
                if (IsHigherOrder(bond.Order, maxOrder)) maxOrder = bond.Order;
            }
            return maxOrder;
        }

        /**
         * Returns the maximum bond order for the two bonds.
         *
         * @param  firstBond  first bond to compare
         * @param  secondBond second bond to compare
         * @return            The maximum bond order found
         */
        public static BondOrder GetMaximumBondOrder(IBond firstBond, IBond secondBond)
        {
            if (firstBond == null || secondBond == null)
                throw new ArgumentNullException("null instance of IBond provided");
            return GetMaximumBondOrder(firstBond.Order, secondBond.Order);
        }

        /**
         * Returns the maximum bond order for the two bond orders.
         *
         * @param  firstOrder  first bond order to compare
         * @param  secondOrder second bond order to compare
         * @return             The maximum bond order found
         */
        public static BondOrder GetMaximumBondOrder(BondOrder firstOrder, BondOrder secondOrder)
        {
            if (firstOrder == BondOrder.Unset)
            {
                if (secondOrder == BondOrder.Unset) throw new ArgumentException("Both bond orders are unset");
                return secondOrder;
            }
            if (secondOrder == BondOrder.Unset)
            {
                if (firstOrder == BondOrder.Unset) throw new ArgumentException("Both bond orders are unset");
                return firstOrder;
            }

            if (IsHigherOrder(firstOrder, secondOrder))
                return firstOrder;
            else
                return secondOrder;
        }

        /**
         * Returns the minimum bond order for a List of bonds, given an iterator
         * to the list.
         *
         * @param bonds An iterator for the list of bonds
         * @return The minimum bond order found
         * @see #GetMinimumBondOrder(IList)
         */
        public static BondOrder GetMinimumBondOrder(IEnumerable<IBond> bonds)
        {
            BondOrder minOrder = BondOrder.Sextuple;
            foreach (var bond in bonds)
                if (IsLowerOrder(bond.Order, minOrder)) minOrder = bond.Order;
            return minOrder;
        }

        /**
         * Get the single bond equivalent (SBE) of a list of bonds, given an iterator to the list.
         *
         * @param bonds An iterator to the list of bonds
         * @return The SBE sum
         */
        public static int GetSingleBondEquivalentSum(IEnumerable<IBond> bonds)
        {
            int sum = 0;
            foreach (var bond in bonds)
            {
                BondOrder order = bond.Order;
                if (!order.IsUnset)
                {
                    sum += order.Numeric;
                }
            }
            return sum;
        }
    }
}
