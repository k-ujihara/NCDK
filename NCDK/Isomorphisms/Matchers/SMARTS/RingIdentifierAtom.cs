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
    /**
     * This encapsulates an atom with a ring identifier, with an optional ring
     * bond specified. For example, <code>C=1CCCCC1</code>.
     *
     * @cdk.module  smarts
     * @cdk.githash
     * @cdk.keyword SMARTS
     */
    public class RingIdentifierAtom : SMARTSAtom
    {
        public RingIdentifierAtom(IChemObjectBuilder builder)
            : base(builder)
        { }

        /*
         * (non-Javadoc)
         * @see
         * org.openscience.cdk.isomorphism.matchers.smarts.SMARTSAtom#Matches(org
         * .openscience.cdk.interfaces.IAtom)
         */
        public override bool Matches(IAtom atom)
        {
            return this.Atom.Matches(atom);
        }

        public IQueryAtom Atom { get; set; }

        public IQueryBond RingBond { get; set; }
    }
}
