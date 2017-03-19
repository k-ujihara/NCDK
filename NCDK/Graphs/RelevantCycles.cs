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
using System.Collections.Generic;
using static NCDK.Graphs.InitialCycles;
using static NCDK.Common.Base.Preconditions;

namespace NCDK.Graphs
{
    /// <summary>
    /// Compute the relevant cycles (<i>C<sub>R</sub></i>) of a graph. A cycle is
    /// relevant if it cannot be represented as the ⊕-sum (xor) of strictly
    /// shorter cycles {@cdk.cite Berger04}. This is the smallest set of short cycles
    /// which is <i>uniquely</i> defined for a graph. The set can also be thought of
    /// as the union of all minimum cycle bases. The set of cycles may be exponential
    /// in number but can be checked (see <see cref="Count"/>) before construction
    /// {@cdk.cite Vismara97}.
    /// </summary>
    /// <example><code>
    /// // import static org.openscience.cdk.graph.GraphUtil.*;
    /// IAtomContainer m = ...;
    ///
    /// // compute on the whole graph
    /// RelevantCycles relevant = new RelevantCycles(ToAdjList(m));
    ///
    /// // it is much faster to compute on the separate ring systems of the molecule
    /// int[][]    graph      = ToAdjList(m);
    /// RingSearch ringSearch = new RingSearch(m, graph);
    ///
    /// // all isolated cycles are relevant
    /// for (int[] isolated : ringSearch.Isolated()){
    ///     int[] path = Cycle(graph, isolated);
    /// }
    ///
    /// // compute the relevant cycles for each system
    /// for (int[] fused : ringSearch.Fused()){
    ///
    ///     int[][] subgraph = Subgraph(graph, fused);
    ///     RelevantCycles relevant = new RelevantCycles(subgraph);
    ///
    ///     For(int[] path : relevant.GetPaths()){
    ///         // convert the sub graph vertices back to the super graph indices
    ///         For(int i = 0; i &lt; path.Length; i++) {
    ///             path[i] = fused[path[i];
    ///         }
    ///     }
    /// }
    /// </code></example>
    /// <seealso cref="RingSearches.RingSearch"/>
    /// <seealso cref="GreedyBasis"/>
    // @author John May
    // @cdk.module core
    // @cdk.keyword relevant cycles
    // @cdk.keyword relevant rings
    // @cdk.keyword R(G)
    // @cdk.keyword union of all minimum cycles bases
    // @cdk.keyword cycle
    // @cdk.keyword ring
    // @cdk.keyword ring perception
    // @cdk.githash
    public sealed class RelevantCycles
    { 
        /// <summary>The relevant cycle basis.</summary>
        private readonly GreedyBasis basis;

        /// <summary>
        /// Generate the relevant cycle basis for a graph.
        /// </summary>
        /// <param name="graph">undirected adjacency list</param>
        /// <seealso cref="RingSearches.RingSearch.Fused"/>
        /// <seealso cref="GraphUtil.Subgraph(int[][], int[])"/>
        public RelevantCycles(int[][] graph)
            : this(new InitialCycles(graph))
        {
        }

        /// <summary>
        /// Generate the relevant cycle basis from a precomputed set of initial
        /// cycles.
        /// </summary>
        /// <param name="initial">set of initial cycles.</param>
        /// <exception cref="System.ArgumentNullException">null InitialCycles provided</exception>
        internal RelevantCycles(InitialCycles initial)
        {
            CheckNotNull(initial, nameof(initial), "No InitialCycles provided");

            this.basis = new GreedyBasis(initial.GetNumberOfCycles(), initial.GetNumberOfEdges());

            // processing by size add cycles which are independent of smaller cycles
            foreach (var length in initial.Lengths)
            {
                basis.AddAll(Independent(initial.GetCyclesOfLength(length)));
            }
        }

        /// <summary>
        /// Given a list of cycles return those which are independent (⊕-sum)
        /// from the current basis.
        /// </summary>
        /// <param name="cycles">cycles of a given .Length</param>
        /// <returns>cycles which were independent</returns>
        private List<Cycle> Independent(IEnumerable<Cycle> cycles)
        {
            var independent = new List<Cycle>();
            foreach (var cycle in cycles)
            {
                if (basis.IsIndependent(cycle)) independent.Add(cycle);
            }
            return independent;
        }

        /// <summary>
        /// Reconstruct the paths of all relevant cycles.
        /// </summary>
        /// <example><code>
        /// RelevantCycles relevant = ...
        ///
        /// // ensure the number is manageable
        /// if(relevant.Count() &lt; 100) {
        ///   For(int[] path : relevant.GetPaths()){
        ///     // process the path
        ///   }
        /// }
        /// </code></example>
        /// <returns>array of vertex paths</returns>
        public int[][] GetPaths()
        {
            int[][] paths = new int[Count()][];
            int i = 0;
            foreach (var c in basis.Members)
            {
                foreach (var path in c.GetFamily())
                    paths[i++] = path;
            }
            return paths;
        }

        /// <summary>
        /// The number of the relevant cycles.
        /// </summary>
        /// <returns>size of relevant cycle set</returns>
        public int Count()
        {
            int size = 0;
            foreach (var c in basis.Members)
                size += c.SizeOfFamily();
            return size;
        }
    }
}
