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
namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /**
     * This matcher checks the valence of the Atom. The valence is the number of
     * bonds formed by an atom (including bonds to implicit hydrogens).
     *
     * @cdk.module  smarts
     * @cdk.keyword SMARTS
     * @cdk.githash
     */
    public sealed class TotalValencyAtom : SMARTSAtom
    {

        /**
         * The valence to match.
         */
        private readonly int valence;

        /**
         * Match the valence of atom.
         * @param valence valence value
         * @param builder chem object builder (required for ChemObject.getBuilder)
         */
        public TotalValencyAtom(int valence, IChemObjectBuilder builder)
            : base(builder)
        {
            this.valence = valence;
        }

        /// <inheritdoc/>
        public override bool Matches(IAtom atom)
        {
            return Invariants(atom).Valence == valence;
        }
    }
}
