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

using System;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    /// Match an atom in a specific size ring. The ring size is specified by "r&lt;NUMBER&gt;"
    /// in a SMARTS pattern. This term is non-portable, depending on the
    /// set of rings chosen and which ring sizes are used. The default implementation
    /// (Daylight) only stores the smallest ring each atom belongs to whilst other
    /// implementations may store multiple values. A more portable term is the
    /// ring connectivity which is specified as "x&lt;NUMBER&gt;".
    /// </summary>
    // @cdk.module smarts
    // @cdk.keyword SMARTS
    // @cdk.githash
    [Obsolete]
    public sealed class SmallestRingAtom : SMARTSAtom
    {
        /// <summary>Ring size to check.</summary>
        private readonly int ringSize;

        /// <summary>
        /// Creates a matcher for specified ring size.
        /// </summary>
        /// <param name="ringSize">size of the ring to check.</param>
        /// <param name="builder"></param>
        public SmallestRingAtom(int ringSize, IChemObjectBuilder builder)
            : base(builder)
        {
            this.ringSize = ringSize;
        }

        /// <inheritdoc/>
        public override bool Matches(IAtom atom)
        {
            return ringSize < 0 ? Invariants(atom).RingConnectivity > 0
                                : Invariants(atom).RingSize.Contains(ringSize);
        }
    }
}
