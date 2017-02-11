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
    /**
     * This matcher checks the formal charge of the Atom.
     *
     * @cdk.module  smarts
     * @cdk.githash
     * @cdk.keyword SMARTS
     */
    public class FormalChargeAtom : SMARTSAtom
    {
        /**
         * Creates a new instance
         *
         * @param charge
         */
        public FormalChargeAtom(int charge, IChemObjectBuilder builder)
            : base(builder)
        {
            this.formalCharge = charge;
        }

        /*
         * (non-Javadoc)
         * @see
         * org.openscience.cdk.isomorphism.matchers.smarts.SMARTSAtom#Matches(org
         * .openscience.cdk.interfaces.IAtom)
         */
        public override bool Matches(IAtom atom)
        {
            return atom.FormalCharge == this.formalCharge;
        }

        /*
         * (non-Javadoc)
         * @see org.openscience.cdk.PseudoAtom#ToString()
         */
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("FormalChargeAtom(");
            s.Append(this.GetHashCode()).Append(", ");
            s.Append("C:").Append(this.formalCharge);
            s.Append(')');
            return s.ToString();
        }
    }
}
