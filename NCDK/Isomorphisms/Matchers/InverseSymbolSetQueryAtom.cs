/*  Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *                     2010  Egon Willighagen <egonw@users.sf.net>
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

using System;
using System.Collections.Generic;
using System.Text;

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    /// A QueryAtom that matches all symbols but those in this container. You may
    /// add symbols to this container. This QueryAtom will only give a match if it
    /// does NOT contain the symbol of the Atom to match (example: add "C" to get a
    /// match for all non-"C"-Atoms).
    /// </summary>
    // @author        kha
    // @cdk.created   2004-09-16
    // @see           SymbolSetQueryAtom
    // @cdk.module    isomorphism
    [Obsolete("Use new Expr(Element, 6).And(new Expr(Element, 8)).Negate() etc")]
    public class InverseSymbolSetQueryAtom 
        : QueryAtom, IQueryAtom
    {
        private ICollection<string> symbols = new HashSet<string>();

        /// <summary>
        ///  Constructor for the InverseSymbolSetQueryAtom object
        /// </summary>
        public InverseSymbolSetQueryAtom(IChemObjectBuilder builder)
            : base(builder)
        { }

        /// <summary>
        ///  The matches implementation of the <see cref="QueryAtom"/> interface.
        /// </summary>
        /// <param name="atom">The atom to be matched by this <see cref="QueryAtom"/></param>
        /// <returns>true if Atom matched</returns>
        public override bool Matches(IAtom atom)
        {
            return !symbols.Contains(atom.Symbol);
        }

        /// <summary>
        ///  Add a symbol to this <see cref="QueryAtom"/>
        /// </summary>
        /// <param name="symbol">The symbol to add</param>
        public void AddSymbol(string symbol)
        {
            symbols.Add(symbol);
        }

        /// <summary>
        /// Remove a symbol from this <see cref="QueryAtom"/>
        /// </summary>
        /// <param name="symbol">The symbol to remove</param>
        public void RemoveSymbol(string symbol)
        {
            symbols.Remove(symbol);
        }

        /// <summary>
        /// Check whether a symbol is already registered
        /// </summary>
        /// <param name="symbol">The symbol to check for</param>
        /// <returns>true if symbol already registered</returns>
        public bool HasSymbol(string symbol)
        {
            return symbols.Contains(symbol);
        }

        /// <summary>
        /// Retrieve the Set of symbols
        ///
        /// <returns>The symbol Set</returns>
        /// </summary>
        public ICollection<string> GetSymbolSet()
        {
            return symbols;
        }

        /// <summary>
        ///  The ToString method
        /// </summary>
        /// <returns>The string representation of this object.</returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("InverseSymbolSetQueryAtom(");
            s.Append(this.GetHashCode() + ", ");
            s.Append(symbols.ToString());
            s.Append(')');
            return s.ToString();
        }
    }
}
