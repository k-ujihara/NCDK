/* Copyright (C) 2009-2010  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;

namespace NCDK.SMSD.Helper
{
    /// <summary>
    /// Helper class defining the energy for a bond type. The bond
    /// type is defined as to element symbols and a bond order.
    ///
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    /// </summary>
    public class BondEnergy
    {
        private string symbol1 = "";
        private string symbol2 = "";
        private BondOrder bondOrder = BondOrder.Unset;
        private int energy = -1;

        /// <summary>
        /// Creates a new bond energy for the given elements and
        /// bond order.
        ///
        /// <param name="symbol1">element symbol for the first atom</param>
        /// <param name="symbol2">element symbol for the second atom</param>
        /// <param name="order">bond order</param>
        /// <param name="energy">energy for this bond type</param>
        /// </summary>
        public BondEnergy(string symbol1, string symbol2, BondOrder order, int energy)
        {
            this.symbol1 = symbol1;
            this.symbol2 = symbol2;
            this.bondOrder = order;
            this.energy = energy;
        }

        /// <summary>
        /// Returns the element symbol of the first atom.
        ///
        /// <returns>the element symbol as <see cref="string"/></returns>
        /// </summary>
        public string SymbolFirstAtom => symbol1;

        /// <summary>
        /// Returns the element symbol of the second atom.
        ///
        /// <returns>the element symbol as <see cref="string"/></returns>
        /// </summary>
        public string SymbolSecondAtom => symbol2;

        /// <summary>
        /// Returns the bond order for this bond type energy.
        ///
        /// <returns>the bond order of the bond type as <see cref="Order"/></returns>
        /// </summary>
        public BondOrder BondOrder => bondOrder;

        /// <summary>
        /// Returns the energy for this bond type.
        ///
        /// <returns>the bond energy as integer.</returns>
        /// </summary>
        public int Energy => energy;

        public bool Matches(IBond bond)
        {
            IAtom atom1 = bond.Atoms[0];
            IAtom atom2 = bond.Atoms[1];

            if ((string.Equals(atom1.Symbol, symbol1, StringComparison.OrdinalIgnoreCase) &&
                  string.Equals(atom2.Symbol, symbol2, StringComparison.OrdinalIgnoreCase))
                    || string.Equals(atom1.Symbol, symbol2, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(atom2.Symbol, symbol1, StringComparison.OrdinalIgnoreCase))
            {
                if (bond.Order.CompareTo(bondOrder) == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
