/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using System.Text;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    /// This matches a bond with a certain bond order.
    ///
    /// Daylight spec indicates that if match a single bond
    /// using '-', it should be an aliphatic single bond
    ///
    // @cdk.module  smarts
    // @cdk.githash
    // @cdk.keyword SMARTS
    /// </summary>
    public class OrderQueryBond : SMARTSBond
    {
        /// <summary>
        /// Creates a new instance
        ///
        /// <param name="order">the order of bond</param>
        /// </summary>
        public OrderQueryBond(BondOrder order, IChemObjectBuilder builder)
            : base(builder)
        {
            this.Order = order;
        }

        public override bool Matches(IBond bond)
        {
            if (bond.IsAromatic ^ IsAromatic) return false;

            // we check for both bonds being aromatic - but the query will
            // never come in as aromatic (since there is a separate aromatic
            // query bond). But no harm in checking
            return bond.IsAromatic || bond.Order == Order;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("OrderQueryBond(");
            s.Append(this.GetHashCode() + ", ");
            s.Append("#O:" + Order);
            s.Append(')');
            return s.ToString();
        }
    }
}
