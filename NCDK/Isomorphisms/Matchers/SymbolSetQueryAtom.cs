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
using System;
using System.Collections.Generic;
using System.Text;

namespace NCDK.Isomorphisms.Matchers
{
    /**
     *  A QueryAtom that matches all symbols in this container. You may add symbols
     *  to this container. This QueryAtom will only give a match if it contains the
     *  symbol of the Atom to match (example: add "F", "Cl", "Br", "I" to get a
     *  match for the most common halogens).
     *
     *@author        kha
     * @cdk.githash
     *@cdk.created   2004-09-16
     *@see           InverseSymbolSetQueryAtom
     *@cdk.module    isomorphism
     */
    public class SymbolSetQueryAtom : QueryAtom, IQueryAtom
    {
        private ICollection<string> symbols = new HashSet<string>();

        /**
         *  Constructor for the SymbolSetQueryAtom object
         */
        public SymbolSetQueryAtom(IChemObjectBuilder builder)
            : base(builder)
        { }

        public void SetOperator(string str) { }

        /**
         *  The matches implementation of the QueryAtom interface.
         *
         *@param  atom  The atom to be matched by this QueryAtom
         *@return       true if Atom matched
         */

        public override bool Matches(IAtom atom)
        {
            return symbols.Contains(atom.Symbol);
        }

        /**
         *  Add a symbol to this QueryAtom
         *
         *@param  symbol  The symbol to add
         */
        public void AddSymbol(string symbol)
        {
            symbols.Add(symbol);
        }

        /**
         *  Remove a symbol from this QueryAtom
         *
         *@param  symbol  The symbol to remove
         */
        public void RemoveSymbol(string symbol)
        {
            symbols.Remove(symbol);
        }

        /**
         *  Check whether a symbol is already registered
         *
         *@param  symbol  The symbol to check for
         *@return         true if symbol already registered
         */
        public bool HasSymbol(string symbol)
        {
            return symbols.Contains(symbol);
        }

        /**
         *  Retrieve the Set of symbols
         *
         *@return    The symbol Set
         */
        public ICollection<string> GetSymbolSet()
        {
            return symbols;
        }

        /**
         *  The ToString method
         *
         *@return    The string representation of this object.
         */

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("SymbolSetQueryAtom(");
            s.Append(this.GetHashCode() + ", ");
            s.Append(symbols.ToString());
            s.Append(')');
            return s.ToString();
        }

        public override object Clone()
        {
            throw new InvalidOperationException();
        }
    }
}

