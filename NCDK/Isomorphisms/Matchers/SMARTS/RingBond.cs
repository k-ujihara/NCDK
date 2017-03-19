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
    /// This smarts bond matches any bond that is in a ring.
    /// </summary>
    // @cdk.module  smarts
    // @cdk.githash
    // @cdk.keyword SMARTS
    public class RingBond : SMARTSBond
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public RingBond(IChemObjectBuilder builder)
            : base(builder)
        {
            this.IsInRing = true;
        }

        public override bool Matches(IBond bond)
        {
            return bond.IsInRing;
        }
    }
}

