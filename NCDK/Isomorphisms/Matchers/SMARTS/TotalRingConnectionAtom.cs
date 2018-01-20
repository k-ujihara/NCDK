/*  Copyright (C) 2002-2006  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All I ask is that proper credit is given for my work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    /// This matcher checks the number of ring connections of the checked Atom with
    /// other Atom's. This cannot be matched without prepossessing Atom - <see cref="SMARTSAtomInvariants"/>.
    /// The ring connectivity is encoded in smarts using <c>x&lt;NUMBER&gt;</c>.
    /// </summary>
    // @cdk.module smarts
    // @cdk.githash
    // @cdk.keyword SMARTS
    public sealed class TotalRingConnectionAtom : SMARTSAtom
    {
        /// <summary>Number of rings.</summary>
        private readonly int ringConnectivity;

        /// <summary>
        /// Create a matcher for the number of rings an atom belongs to.
        /// </summary>
        /// <param name="ringConnectivity">number of ring bonds this atom is adjacent to</param>
        /// <param name="builder"></param>
        public TotalRingConnectionAtom(int ringConnectivity, IChemObjectBuilder builder)
            : base(builder)
        {
            this.ringConnectivity = ringConnectivity;
        }

        /// <inheritdoc/>
        public override bool Matches(IAtom atom)
        {
            return Invariants(atom).RingConnectivity == ringConnectivity;
        }
    }
}
