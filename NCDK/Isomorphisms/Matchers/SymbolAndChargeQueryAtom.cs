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
using System.Text;

namespace NCDK.Isomorphisms.Matchers
{
    // @cdk.module  isomorphism
    // @cdk.githash
    public class SymbolAndChargeQueryAtom : QueryAtom, IQueryAtom
    {
        public SymbolAndChargeQueryAtom(IChemObjectBuilder builder)
           : base(builder)
        { }

        public SymbolAndChargeQueryAtom(IAtom atom)
            : base(atom.Symbol, atom.Builder)
        {
            FormalCharge = atom.FormalCharge;
        }

        public void SetOperator(string str) { }


        public override bool Matches(IAtom atom)
        {
            int requiredCharge = this.FormalCharge ?? 0;
            int foundCharge = atom.FormalCharge ?? 0;
            return this.Symbol.Equals(atom.Symbol) && requiredCharge == foundCharge;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("SymbolAndChargeQueryAtom(");
            s.Append(this.GetHashCode() + ", ");
            s.Append(Symbol + ", ");
            s.Append(FormalCharge);
            s.Append(')');
            return s.ToString();
        }

        public override object Clone()
        {
            throw new InvalidOperationException();
        }
    }
}
