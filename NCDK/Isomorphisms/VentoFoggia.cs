/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Default;
using NCDK.Graphs;
using NCDK.Isomorphisms.Matchers;
using static NCDK.Graphs.GraphUtil;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;

namespace NCDK.Isomorphisms
{
    /// <summary>
    /// A structure pattern which utilises the Vento-Foggia (VF) algorithm {@cdk.cite Cordella04}.
    /// </summary>
    /// <example>
    /// Find and count the number molecules which contain the query substructure.
    ///
    /// <code>
    /// IAtomContainer query   = ...;
    /// Pattern        pattern = VentoFoggia.FindSubstructure(query);
    ///
    /// int hits = 0;
    /// foreach (var m in ms)
    ///     if (pattern.Matches(m))
    ///         hits++;
    /// </code>
    ///
    /// Finding the matching to molecules which contain the query substructure. It is
    /// more efficient to obtain the <see cref="Match(IAtomContainer)"/> and check it's size rather than
    /// test if it <see cref="MatchAll(IAtomContainer)"/>. These methods automatically verify
    /// stereochemistry.
    ///
    /// <code>
    /// IAtomContainer query   = ...;
    /// Pattern        pattern = VentoFoggia.FindSubstructure(query);
    ///
    /// int hits = 0;
    /// foreach (var m in ms) {
    ///     int[] match = pattern.Match(m);
    ///     if (match.Length > 0)
    ///         hits++;
    /// }
    /// </code>
    /// </example>
    // @author John May
    // @cdk.module isomorphism
    public sealed class VentoFoggia : Pattern
    {
        /// <summary>The query structure.</summary>
        private readonly IAtomContainer query;

        /// <summary>The query structure adjacency list.</summary>
        private readonly int[][] g1;

        /// <summary>The bonds of the query structure.</summary>
        private readonly EdgeToBondMap bonds1;

        /// <summary>The atom matcher to determine atom feasibility.</summary>
        private readonly AtomMatcher atomMatcher;

        /// <summary>The bond matcher to determine atom feasibility.</summary>
        private readonly BondMatcher bondMatcher;

        /// <summary>Search for a subgraph.</summary>
        private readonly bool subgraph;

        /// <summary>Is the query matching query atoms/bonds etc?</summary>
        private readonly bool queryMatching;

        /// <summary>
        /// Non-public constructor for-now the atom/bond semantics are fixed.
        /// </summary>
        /// <param name="query">the query structure</param>
        /// <param name="atomMatcher">how atoms should be matched</param>
        /// <param name="bondMatcher">how bonds should be matched</param>
        /// <param name="substructure">substructure search</param>
        private VentoFoggia(IAtomContainer query, AtomMatcher atomMatcher, BondMatcher bondMatcher, bool substructure)
        {
            this.query = query;
            this.atomMatcher = atomMatcher;
            this.bondMatcher = bondMatcher;
            this.bonds1 = EdgeToBondMap.WithSpaceFor(query);
            this.g1 = GraphUtil.ToAdjList(query, bonds1);
            this.subgraph = substructure;
            this.queryMatching = query is IQueryAtomContainer;
        }

        /// <inheritdoc/>
        public override int[] Match(IAtomContainer target)
        {
            return MatchAll(target).GetStereochemistry().First();
        }

        /// <inheritdoc/>
        public override Mappings MatchAll(IAtomContainer target)
        {
            EdgeToBondMap bonds2 = EdgeToBondMap.WithSpaceFor(target);
            int[][] g2 = GraphUtil.ToAdjList(target, bonds2);
            IEnumerable<int[]> iterable = new VFIterable(query, target, g1, g2, bonds1, bonds2, atomMatcher, bondMatcher,
                    subgraph);
            return new Mappings(query, target, iterable);
        }

        /// <summary>
        /// Create a pattern which can be used to find molecules which contain the
        /// <paramref name="query"/> structure.
        /// </summary>
        /// <param name="query">the substructure to find</param>
        /// <returns>a pattern for finding the <paramref name="query"/></returns>
        public static new Pattern FindSubstructure(IAtomContainer query)
        {
            bool isQuery = query is IQueryAtomContainer;
            return FindSubstructure(query,
                                    isQuery ? AtomMatcher.CreateQueryMatcher() : AtomMatcher.CreateElementMatcher(),
                                    isQuery ? BondMatcher.CreateQueryMatcher() : BondMatcher.CreateOrderMatcher());
        }

        /// <summary>
        /// Create a pattern which can be used to find molecules which are the same
        /// as the <paramref name="query"/> structure.
        /// </summary>
        /// <param name="query">the substructure to find</param>
        /// <returns>a pattern for finding the <paramref name="query"/></returns>
        public static new Pattern FindIdentical(IAtomContainer query)
        {
            bool isQuery = query is IQueryAtomContainer;
            return FindIdentical(query,
                                 isQuery ? AtomMatcher.CreateQueryMatcher() : AtomMatcher.CreateElementMatcher(),
                                 isQuery ? BondMatcher.CreateQueryMatcher() : BondMatcher.CreateOrderMatcher());
        }

        /// <summary>
        /// Create a pattern which can be used to find molecules which contain the
        /// <paramref name="query"/> structure.
        /// </summary>
        /// <param name="query">the substructure to find</param>
        /// <param name="atomMatcher">how atoms are matched</param>
        /// <param name="bondMatcher">how bonds are matched</param>
        /// <returns>a pattern for finding the <paramref name="query"/></returns>
        public static Pattern FindSubstructure(IAtomContainer query, AtomMatcher atomMatcher, BondMatcher bondMatcher)
        {
            return new VentoFoggia(query, atomMatcher, bondMatcher, true);
        }

        /// <summary>
        /// Create a pattern which can be used to find molecules which are the same
        /// as the <paramref name="query"/> structure.
        /// </summary>
        /// <param name="query">the substructure to find</param>
        /// <param name="atomMatcher">how atoms are matched</param>
        /// <param name="bondMatcher">how bonds are matched</param>
        /// <returns>a pattern for finding the <paramref name="query"/></returns>
        public static Pattern FindIdentical(IAtomContainer query, AtomMatcher atomMatcher, BondMatcher bondMatcher)
        {
            return new VentoFoggia(query, atomMatcher, bondMatcher, false);
        }

        private sealed class VFIterable : IEnumerable<int[]>
        {
            /// <summary>Query and target containers.</summary>
            private readonly IAtomContainer container1, container2;

            /// <summary>Query and target adjacency lists.</summary>
            private readonly int[][] g1, g2;

            /// <summary>Query and target bond lookup.</summary>
            private readonly EdgeToBondMap bonds1, bonds2;

            /// <summary>How are atoms are matched.</summary>
            private readonly AtomMatcher atomMatcher;

            /// <summary>How are bonds are match.</summary>
            private readonly BondMatcher bondMatcher;

            /// <summary>The query is a subgraph.</summary>
            private readonly bool subgraph;

            /// <summary>
            /// Create a match for the following parameters.
            /// </summary>
            /// <param name="container1">query structure</param>
            /// <param name="container2">target structure</param>
            /// <param name="g1">query adjacency list</param>
            /// <param name="g2">target adjacency list</param>
            /// <param name="bonds1">query bond map</param>
            /// <param name="bonds2">target bond map</param>
            /// <param name="atomMatcher">how atoms are matched</param>
            /// <param name="bondMatcher">how bonds are matched</param>
            /// <param name="subgraph">perform subgraph search</param>
            public VFIterable(IAtomContainer container1, IAtomContainer container2, int[][] g1, int[][] g2,
                    EdgeToBondMap bonds1, EdgeToBondMap bonds2, AtomMatcher atomMatcher, BondMatcher bondMatcher,
                    bool subgraph)
            {
                this.container1 = container1;
                this.container2 = container2;
                this.g1 = g1;
                this.g2 = g2;
                this.bonds1 = bonds1;
                this.bonds2 = bonds2;
                this.atomMatcher = atomMatcher;
                this.bondMatcher = bondMatcher;
                this.subgraph = subgraph;
            }

            /// <inheritdoc/>
            public IEnumerator<int[]> GetEnumerator()
            {
                StateStream ss;
                if (subgraph)
                {
                    ss = new StateStream(new VFSubState(container1, container2, g1, g2, bonds1, bonds2, atomMatcher,
                            bondMatcher));
                }
                else
                {
                    ss = new StateStream(
                        new VFState(container1, container2, g1, g2, bonds1, bonds2, atomMatcher, bondMatcher));
                }
                return ss.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
