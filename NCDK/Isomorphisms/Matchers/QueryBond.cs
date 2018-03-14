/* Copyright (C) 2010  M.Rijnbeek <markr@ebi.ac.uk>
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

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    /// Implements the concept of a "query bond" between two or more atoms.
    /// Query bonds can be used to capture types such as "Single or Double" or "Any".
    /// </summary>
    // @cdk.module isomorphism
    // @cdk.githash
    // @cdk.created 2010-12-16
    public abstract class QueryBond : Silent.Bond, IQueryBond
    {
        private IChemObjectBuilder builder;
        /// <inheritdoc/>
        public override IChemObjectBuilder Builder => builder;

        public QueryBond(IChemObjectBuilder builder)
        {
            this.builder = builder;
        }

        public QueryBond(IAtom atom1, IAtom atom2, BondOrder order, IChemObjectBuilder builder)
            : base(atom1, atom2, order)
        {
            this.builder = builder;
        }

        public abstract bool Matches(IBond bond);
    }
}
