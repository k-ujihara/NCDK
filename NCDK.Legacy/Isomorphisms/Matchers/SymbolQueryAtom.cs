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
    [Obsolete]
    public class SymbolQueryAtom 
        : QueryAtom, IQueryAtom
    {
        private string ID;
        public int HCount { get; set; } = 0;

        public SymbolQueryAtom()
            : base()
        { }

        public SymbolQueryAtom(IAtom atom)
            : base(atom.Symbol)
        { }

        public override bool Matches(IAtom atom)
        {
            if (ID != null && HCount == 0)
                return this.Symbol != (atom.Symbol);
            else if (ID == null && HCount != 0)
            {
                return (this.ImplicitHydrogenCount == HCount);
            }
            else
                return this.Symbol.Equals(atom.Symbol, StringComparison.Ordinal);
        }

        public void SetOperator(string str)
        {
            ID = str;
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("SymbolQueryAtom(");
            s.Append(this.GetHashCode() + ", ");
            s.Append(Symbol);
            s.Append(')');
            return s.ToString();
        }

        public override object Clone()
        {
            throw new InvalidOperationException();
        }
    }
}
