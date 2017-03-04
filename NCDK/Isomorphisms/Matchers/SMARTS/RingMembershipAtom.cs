/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 * (or see http://www.gnu.org/copyleft/lesser.html)
 */
namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    /// This query is found in a specified number of ring. The ring membership is
    /// specified with the SMARTS {@code R<NUMBER>}. The membership depends on the
    /// ring set used and as such is not a portable term. If the Smallest Set of
    /// Smallest Rings (SSSR) is used then changing the order of atoms
    /// <i>may</i> change which atoms match in a pattern.
    ///
    // @cdk.module smarts
    // @cdk.keyword SMARTS
    // @cdk.githash
    /// </summary>
    public class RingMembershipAtom : SMARTSAtom
    {

        /// <summary>
        /// Number of rings to which this atom belongs, if < 0 check any ring
        /// membership.
        /// </summary>
        private int ringNumber;

        /// <summary>
        /// Ring membership query atom. Check if the an atom belongs to <i>num</i> of
        /// rings. To specify any ring membership, <i>num</i> should be specified as
        /// < 0. Generally in SMARTS it's better negate ring membership with {@code
        /// [!R]} however for legacy reasons {@code [R0]} was accepted and checks
        /// this atoms belongs to 0 rings.
        ///
        /// <param name="num">number of rings which this atom belongs to, < 0 any ring.</param>
        /// </summary>
        public RingMembershipAtom(int num, IChemObjectBuilder builder)
            : base(builder)
        {
            this.ringNumber = num;
        }

        /// <inheritdoc/>
        public override bool Matches(IAtom atom)
        {
            return ringNumber < 0 ? Invariants(atom).RingConnectivity > 0 : ringNumber == Invariants(atom).RingNumber;
        }
    }
}
