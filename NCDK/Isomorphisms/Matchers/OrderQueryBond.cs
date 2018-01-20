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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;

namespace NCDK.Isomorphisms.Matchers
{
    // @cdk.module isomorphism
    // @cdk.githash
    public class OrderQueryBond : QueryBond, IQueryBond
    {
        public OrderQueryBond(IChemObjectBuilder builder)
            : base(builder)
        { }

        public OrderQueryBond(IQueryAtom atom1, IQueryAtom atom2, BondOrder order, IChemObjectBuilder builder)
            : base(atom1, atom2, order, builder)
        { }

        public override bool Matches(IBond bond)
        {
            if (this.Order == bond.Order)
            {
                // bond orders match
                return true;
            }
            else if (this.IsAromatic && bond.IsAromatic)
            {
                return true;
            } // else
            return false;
        }

        public void SetAtoms(IAtom[] atoms)
        {
            if (atoms.Length > 0 && atoms[0] is IQueryAtom)
            {
                base.SetAtoms(atoms);
            }
            else
            {
                throw new ArgumentException("Array is not of type QueryAtom[]");
            }
        }

        public void SetAtomAt(IAtom atom, int position)
        {
            if (atom is IQueryAtom)
            {
                base.Atoms[position] = atom;
            }
            else
            {
                throw new ArgumentException("Atom is not of type QueryAtom");
            }
        }
    }
}
