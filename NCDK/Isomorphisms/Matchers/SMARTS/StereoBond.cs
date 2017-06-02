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
using System;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    /// This query bond indicates a particular geometric stereo configuration.
    /// </summary>
    // @cdk.module  smarts
    // @cdk.githash
    // @cdk.keyword SMARTS
    public class StereoBond : SMARTSBond
    {
        private readonly bool unspecified;
        private readonly Direction direction;

        public enum Direction
        {
            Up, Down
        }

        public StereoBond(IChemObjectBuilder builder, Direction direction, bool unspecified)
            : base(builder)
        {
            this.unspecified = unspecified;
            this.direction = direction;
        }

        public override bool Matches(IBond bond)
        {
            return BondOrder.Single.Equals(bond.Order);
        }

        public bool IsUnspecified => unspecified;

        public Direction GetDirection(IAtom atom)
        {
            if (Begin == atom)
                return direction;
            else if (End == atom)
                return Inv(direction);
            throw new ArgumentException("atom is not a memeber of this bond");
        }

        private Direction Inv(Direction direction)
        {
            return direction == Direction.Up ? Direction.Down : Direction.Up;
        }
    }
}
