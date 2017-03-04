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
    /// <summary>
    /// This matcher checks the number of implicit hydrogens of the Atom.
    ///
    // @cdk.module  smarts
    // @cdk.githash
    // @cdk.keyword SMARTS
    /// </summary>
    public class ImplicitHCountAtom : SMARTSAtom
    {
        private readonly int hcount;

        /// <summary>
        /// Creates a new instance
        ///
        /// <param name="hcount">/// </summary></param>
        public ImplicitHCountAtom(int hcount, IChemObjectBuilder builder)
            : base(builder)
        {
            this.hcount = hcount;
            this.ImplicitHydrogenCount = hcount;
        }

        public override bool Matches(IAtom atom)
        {
            // h counts should be set before match throw runtime exception?
            if (atom.ImplicitHydrogenCount == null)
                return false;
            return atom.ImplicitHydrogenCount == hcount;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("ImplicitHCountAtom(");
            s.Append(this.GetHashCode()).Append(", ");
            s.Append("IH:" + hcount);
            s.Append(')');
            return s.ToString();
        }
    }
}
