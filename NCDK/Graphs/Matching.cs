/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
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

using NCDK.Common.Collections;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace NCDK.Graphs
{
    /**
     * A matching is an independent edge set of a graph. This is a set of edges that
     * share no common vertices. A matching is perfect if every vertex in the graph
     * is matched. Each vertex can be matched with exactly one other vertex.<p/>
     *
     * This class provides storage and manipulation of a matching. A new match is
     * added with {@link #Match(int, int)}, any existing match for the newly matched
     * vertices is no-longer available. The status of a vertex can be queried with
     * {@link #Matched(int)} and the matched vertex obtained with {@link
     * #Other(int)}. <p/>
     *
     * @author John May
     * @cdk.module standard
     * @see <a href="http://en.wikipedia.org/wiki/Matching_(graph_theory)">Matching
     * (graph theory), Wikipedia</a>
     */
    public sealed class Matching
    {

        /// <summary>Indicate an unmatched vertex.</summary>
        private const int NIL = -1;

        /// <summary>Match storage.</summary>
        private readonly int[] match;

        /**
         * Create a matching of the given size.
         *
         * @param n number of items
         */
        private Matching(int n)
        {
            this.match = new int[n];
            Arrays.Fill(match, NIL);
        }

        /**
         * Add the edge '{u,v}' to the matched edge set. Any existing matches for
         * 'u' or 'v' are removed from the matched set.
         *
         * @param u a vertex
         * @param v another vertex
         */
        public void Match(int u, int v)
        {
            // set the new match, don't need to update existing - we only provide
            // access to bidirectional mappings
            match[u] = v;
            match[v] = u;
        }

        /**
         * Access the vertex matched with 'v'.
         *
         * @param v vertex
         * @return matched vertex
         * @ the vertex is currently unmatched
         */
        public int Other(int v)
        {
            if (Unmatched(v)) throw new ArgumentException(v + " is not matched");
            return match[v];
        }

        /**
         * Remove a matching for the specified vertex.
         *
         * @param v vertex
         */
        public void Unmatch(int v)
        {
            match[v] = NIL;
        }

        /**
         * Determine if a vertex has a match.
         *
         * @param v vertex
         * @return the vertex is matched
         */
        public bool Matched(int v)
        {
            return !Unmatched(v);
        }

        /**
         * Determine if a vertex is not matched.
         *
         * @param v a vertex
         * @return the vertex has no matching
         */
        public bool Unmatched(int v)
        {
            return match[v] == NIL || match[match[v]] != v;
        }

        /**
         * Attempt to augment the matching such that it is perfect over the subset
         * of vertices in the provided graph.
         *
         * @param graph  adjacency list representation of graph
         * @param subset subset of vertices
         * @return the matching was perfect
         * @ the graph was a different size to the
         *                                  matching capacity
         */
        public bool Perfect(int[][] graph, BitArray subset)
        {

            if (graph.Length != match.Length || BitArrays.Cardinality(subset) > graph.Length)
                throw new ArgumentException("graph and matching had different capacity");

            // and odd set can never provide a perfect matching
            if ((BitArrays.Cardinality(subset) & 0x1) == 0x1) return false;

            // arbitrary matching was perfect
            if (ArbitaryMatching(graph, subset)) return true;

            EdmondsMaximumMatching.Maxamise(this, graph, subset);

            // the matching is imperfect if any vertex was
            for (int v = BitArrays.NextSetBit(subset, 0); v >= 0; v = BitArrays.NextSetBit(subset, v + 1))
                if (Unmatched(v)) return false;

            return true;
        }

        /**
         * Assign an arbitrary matching that covers the subset of vertices.
         *
         * @param graph  adjacency list representation of graph
         * @param subset subset of vertices in the graph
         * @return the matching was perfect
         */
#if TEST
        public
#endif
        bool ArbitaryMatching(int[][] graph, BitArray subset)
        {

            BitArray unmatched = new BitArray(subset.Length);

            // indicates the deg of each vertex in unmatched subset
            int[] deg = new int[graph.Length];

            // queue/stack of vertices with deg1 vertices
            int[] deg1 = new int[graph.Length];
            int nd1 = 0, nMatched = 0;

            for (int v = BitArrays.NextSetBit(subset, 0); v >= 0; v = BitArrays.NextSetBit(subset, v + 1))
            {
                if (Matched(v))
                {
                    Trace.Assert(subset[Other(v)]);
                    nMatched++;
                    continue;
                }
                unmatched.Set(v, true);
                foreach (var w in graph[v])
                    if (subset[w] && Unmatched(w)) deg[v]++;
                if (deg[v] == 1) deg1[nd1++] = v;
            }

            while (!BitArrays.IsEmpty(unmatched))
            {

                int v = -1;

                // attempt to select a vertex with degree = 1 (in matched set)
                while (nd1 > 0)
                {
                    v = deg1[--nd1];
                    if (unmatched[v]) break;
                }

                // no unmatched degree 1 vertex, select the first unmatched
                if (v < 0 || unmatched[v]) v = BitArrays.NextSetBit(unmatched, 0);

                unmatched.Set(v, false);

                // find a unmatched edge and match it, adjacent degrees are updated
                foreach (var w in graph[v])
                {
                    if (unmatched[w])
                    {
                        Match(v, w);
                        nMatched += 2;
                        unmatched.Set(w, false);
                        // update neighbors of w and v (if needed)
                        foreach (var u in graph[w])
                            if (--deg[u] == 1 && unmatched[u]) deg1[nd1++] = u;

                        // if deg == 1, w is the only neighbor
                        if (deg[v] > 1)
                        {
                            foreach (var u in graph[v])
                                if (--deg[u] == 1 && unmatched[u]) deg1[nd1++] = u;
                        }
                        break;
                    }
                }
            }

            return nMatched == BitArrays.Cardinality(subset);
        }

        /**
         * Create an empty matching with the specified capacity.
         *
         * @param capacity maximum number of vertices
         * @return empty matching
         */
        public static Matching WithCapacity(int capacity)
        {
            return new Matching(capacity);
        }

        /// <inheritdoc/>

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(4 * match.Length);
            sb.Append('[');
            for (int u = 0; u < match.Length; u++)
            {
                int v = match[u];
                if (v > u && match[v] == u)
                {
                    if (sb.Length > 1) sb.Append(", ");
                    sb.Append(u).Append('=').Append(v);
                }
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
