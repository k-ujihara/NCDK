/* Copyright (C) 2004-2008  Rajarshi Guha <rajarshi.guha@gmail.com>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Common.Primitives;
using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles.SMARTS.Parser;
using System;
using System.Text;

namespace NCDK.Pharmacophore
{
    /// <summary>
    /// Represents a query pharmacophore group.
    /// <para>
    /// This class is meant to be used to construct pharmacophore queries in conjunction
    /// </para>
    /// with <see cref="PharmacophoreQueryBond"/> and an <see cref="QueryAtomContainer"/>.
    /// </summary>
    /// <seealso cref="PharmacophoreQueryBond"/>
    /// <seealso cref="QueryAtomContainer"/>
    /// <seealso cref="PharmacophoreMatcher"/>
    // @author Rajarshi Guha
    // @cdk.module pcore
    // @cdk.githash
    // @cdk.keyword pharmacophore
    // @cdk.keyword 3D isomorphism
    public class PharmacophoreQueryAtom : Silent.Atom, IQueryAtom
    {
        /// <summary>
        /// Creat a new query pharmacophore group
        /// </summary>
        /// <param name="symbol">The symbol for the group</param>
        /// <param name="smarts">The SMARTS pattern to be used for matching</param>
        public PharmacophoreQueryAtom(string symbol, string smarts)
        {
            Symbol = symbol;
            Smarts = smarts;
            // Note that we allow a special form of SMARTS where the | operator
            // represents logical or of multi-atom groups (as opposed to ','
            // which is for single atom matches)
            var subSmarts = Strings.Tokenize(smarts, '|');
            CompiledSmarts = new IQueryAtomContainer[subSmarts.Count];
            for (int i = 0; i < CompiledSmarts.Length; i++)
                CompiledSmarts[i] = SMARTSParser.Parse(subSmarts[i], null);
        }

        /// <summary>
        /// The SMARTS pattern for this pharmacophore group.
        /// </summary>
        public string Smarts { get; private set; }

        /// <summary>
        /// Accessed the compiled SMARTS for this pcore query atom.  
        /// </summary>
        internal IQueryAtomContainer[] CompiledSmarts { get; private set; }

        /// <summary>
        /// Checks whether this query atom matches a target atom.
        /// <para>
        /// Currently a query pharmacophore atom will match a target pharmacophore group if the
        /// symbols of the two groups match. This is based on the assumption that
        /// pharmacophore groups with the same symbol will have the same SMARTS
        /// pattern.
        /// </para>
        /// </summary>
        /// <param name="atom">A target pharmacophore group</param>
        /// <returns>true if the current query group has the same symbol as the target group</returns>
        public bool Matches(IAtom atom)
        {
            PharmacophoreAtom patom = PharmacophoreAtom.Get(atom);
            return patom.Symbol.Equals(Symbol, StringComparison.Ordinal);
        }

        /// <summary>
        /// string representation of this pharmacophore group.
        /// </summary>
        /// <returns>string representation of this pharmacophore group</returns>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(Symbol).Append(" [").Append(Smarts).Append(']');
            return s.ToString();
        }
    }
}
