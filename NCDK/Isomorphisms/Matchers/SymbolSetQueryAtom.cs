/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    ///  A QueryAtom that matches all symbols in this container. You may add symbols
    ///  to this container. This QueryAtom will only give a match if it contains the
    ///  symbol of the Atom to match (example: add "F", "Cl", "Br", "I" to get a
    ///  match for the most common halogens).
    /// </summary>
    /// <seealso cref="InverseSymbolSetQueryAtom"/>
    // @author        kha
    // @cdk.created   2004-09-16
    // @cdk.module    isomorphism
    [Obsolete("Use new Expr(Element, 6).And(new Expr(Element, 8)) etc")]
    public class SymbolSetQueryAtom : QueryAtom, IQueryAtom
    {
        /// <summary>
        /// The symbol Set
        /// </summary>
        public ICollection<string> Symbols { get; } = new HashSet<string>();

        /// <summary>
        ///  Constructor for the <see cref="SymbolSetQueryAtom"/> object
        /// </summary>
        public SymbolSetQueryAtom(IChemObjectBuilder builder)
            : base(builder)
        { }

        /// <summary>
        /// The matches implementation of the <see cref="QueryAtom"/> interface.
        /// </summary>
        /// <param name="atom">The atom to be matched by this <see cref="QueryAtom"/></param>
        /// <returns>true if Atom matched</returns>
        public override bool Matches(IAtom atom)
        {
            return Symbols.Contains(atom.Symbol);
        }

        /// <summary>
        ///  The ToString method
        /// </summary>
        /// <returns>The string representation of this object.</returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("SymbolSetQueryAtom(");
            s.Append(this.GetHashCode() + ", ");
            s.Append(Strings.ToJavaString(Symbols));
            s.Append(')');
            return s.ToString();
        }

        public override object Clone()
        {
            throw new InvalidOperationException();
        }
    }
}

