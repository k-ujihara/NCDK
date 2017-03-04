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
using NCDK.Common.Collections;

using NCDK.Isomorphisms.Matchers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{ 
    /// <summary>
    /// This matches recursive smarts atoms.
    ///
    // @cdk.module smarts
    // @cdk.githash
    // @cdk.keyword SMARTS
    /// </summary>
    public sealed class RecursiveSmartsAtom : SMARTSAtom
    {
        /// <summary>The IQueryAtomContainer created by parsing the recursive smarts</summary>
        private readonly IQueryAtomContainer                  query;

        /// <summary>Query cache.</summary>
        private readonly Dictionary<IAtomContainer, BitArray> cache;

        /// <summary>
        /// Creates a new instance
        ///
        /// <param name="query">/// </summary></param>
        public RecursiveSmartsAtom(IQueryAtomContainer query)
            : base(query.Builder)
        {
            this.query = query;
            this.cache = new Dictionary<IAtomContainer, BitArray>();
        }

        public override bool Matches(IAtom atom)
        {
            if (!((IQueryAtom) query.Atoms[0]).Matches(atom)) return false;

            if (query.Atoms.Count == 1) return true;

            IAtomContainer target = Invariants(atom).Target;

            BitArray v;
            if (!cache.TryGetValue(target, out v))
            {
                BitArray hits = new BitArray(0);
                foreach (var mapping in Ullmann.FindSubstructure(query).MatchAll(target)
                    .Where(n => new SmartsStereoMatch(query, target).Apply(n))
                    .Where(n => new ComponentGrouping(query, target).Apply(n)))
                {
                    BitArrays.SetValue(hits, mapping[0], true);
                }
                v = hits;
                cache[target] = v;
            }

            return BitArrays.GetValue(v, target.Atoms.IndexOf(atom));
        }
    }
}
