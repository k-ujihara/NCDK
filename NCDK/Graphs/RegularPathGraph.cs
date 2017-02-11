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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NCDK.Common.Primitives;

namespace NCDK.Graphs
{
    /**
     * A path graph (<b>P-Graph</b>) for graphs with less than 64 vertices - the
     * P-Graph provides efficient generation of all simple cycles in a graph
     * {@cdk.cite HAN96}. Vertices are sequentially removed from the graph by
     * reducing incident edges and forming new 'path edges'. The order in which the
     * vertices are to be removed should be pre-defined in the constructor as the
     * {@code rank[]} parameter.
     *
     * @author John May
     * @author Till Schäfer (predefined vertex ordering)
     * @cdk.module core
     * @cdk.githash
     * @see org.openscience.cdk.ringsearch.RingSearch
     * @see GraphUtil
     * @see <a href="http://en.wikipedia.org/wiki/Biconnected_component">Wikipedia:
     *      Biconnected Component</a>
     */
    sealed class RegularPathGraph
        : PathGraph
    {
        private static readonly List<PathEdge> EmptyPathEdgeList = new List<PathEdge>();

        /// <summary>Path edges, indexed by their end points (incidence list).</summary>
        private IList<PathEdge>[] graph;

        /// <summary>Limit on the maximum Length of cycle to be found.</summary>
        private readonly int limit;

        /// <summary>Indicates when each vertex will be removed, '0' = first, '|V|' = last.</summary>
        private readonly int[] rank;

        /**
		 * Create a regular path graph (<b>P-Graph</b>) for the given molecule graph
		 * (<b>M-Graph</b>).
		 *
		 * @param mGraph The molecule graph (M-Graph) in adjacency list
		 *               representation.
		 * @param rank   Unique rank of each vertex - indicates when it will be
		 *               removed.
		 * @param limit  Limit for size of cycles found, to find all cycles specify
		 *               the limit as the number of vertices in the graph.
		 * @ limit was invalid or the graph was too
		 *                                  large
		 * @     the molecule graph was not provided
		 */
        public RegularPathGraph(int[][] mGraph, int[] rank, int limit)
        {
            if (mGraph == null)
                throw new ArgumentNullException(nameof(mGraph));
            if (rank == null)
                throw new ArgumentNullException(nameof(rank));

            this.graph = new List<PathEdge>[mGraph.Length];
            this.rank = rank;
            this.limit = limit + 1; // first/last vertex repeats
            int ord = graph.Length;

            // check configuration
			if (!(ord > 2)) 
				throw new ArgumentOutOfRangeException(nameof(graph), "graph was acyclic");
            if (!(limit >= 3 && limit <= ord))
                throw new ArgumentOutOfRangeException(nameof(limit), "limit should be from 3 to |V|");
			if (!(ord < 64))
                throw new ArgumentOutOfRangeException(nameof(graph), "graph has 64 or more atoms, use JumboPathGraph");

            for (int v = 0; v < ord; v++)
                graph[v] = new List<PathEdge>();

            // construct the path-graph
            for (int v = 0; v < ord; v++)
            {
                foreach (int w in mGraph[v])
                    if (w > v)
                        Add(new SimpleEdge(v, w));
            }
        }

        /**
		 * Add a path-edge to the path-graph. Edges are only added to the vertex of
		 * lowest rank (see. constructor).
		 *
		 * @param edge path edge
		 */
        private void Add(PathEdge edge)
        {
            int u = edge.Either();
            int v = edge.Other(u);
            if (rank[u] < rank[v])
                graph[u].Add(edge);
            else
                graph[v].Add(edge);
        }

        /// <inheritdoc/>
        public override int Degree(int x)
        {
            return graph[x].Count;
        }

        /**
		 * Access edges which are incident to <i>x</i> and remove them from the
		 * graph.
		 *
		 * @param x a vertex
		 * @return vertices incident to x
		 */
        private IList<PathEdge> Remove(int x)
        {
            IList<PathEdge> edges = graph[x];
            graph[x] = EmptyPathEdgeList; 
            return edges;
        }

        /**
		 * Pairwise combination of all disjoint <i>edges</i> incident to a vertex
		 * <i>x</i>.
		 *
		 * @param edges edges which are currently incident to <i>x</i>
		 * @param x     a vertex in the graph
		 * @return reduced edges
		 */
        private IList<PathEdge> Combine(IList<PathEdge> edges, int x)
        {
            int n = edges.Count;
            IList<PathEdge> reduced = new List<PathEdge>();

            for (int i = 0; i < n; i++) {
                PathEdge e = edges[i];
                for (int j = i + 1; j < n; j++) {
                    PathEdge f = edges[j];
                    if (e.Disjoint(f)) reduced.Add(new ReducedEdge(e, f, x));
                }
            }

            return reduced;
        }

        /// <inheritdoc/>
        public override void Remove(int x, List<int[]> cycles)
        {
            IList<PathEdge> edges = Remove(x);
            IList<PathEdge> reduced = Combine(edges, x);

            foreach (PathEdge e in reduced)
            {
                if (e.Len() <= limit) {
                    if (e.IsLoop)
                        cycles.Add(e.Path());
                    else
                        Add(e);
                }
            }
        }

        /// <summary>Empty bit set.</summary>
        private const long EMPTY_SET = 0;

        /**
		 * An abstract path edge. A path edge has two end points and 0 or more
		 * reduced vertices which represent a path between those endpoints.
		 */
        abstract class PathEdge
        {
            /// <summary>Endpoints of the edge.</summary>
            public readonly int u, v;

            /// <summary>Bits indicate reduced vertices between endpoints (exclusive).</summary>
            public readonly long xs;

            /**
			 * A new edge specified by two endpoints and a bit set indicating which
			 * vertices have been reduced.
			 *
			 * @param u  an endpoint
			 * @param v  the other endpoint
			 * @param xs reduced vertices between endpoints
			 */
            public PathEdge(int u, int v, long xs)
            {
                this.u = u;
                this.v = v;
                this.xs = xs;
            }

            /**
			 * Check if the edges are disjoint with respect to their reduced
			 * vertices. That is, excluding the endpoints, no reduced vertices are
			 * shared.
			 *
			 * @param other another edge
			 * @return the edges reduced vertices are disjoint.
			 */
            public bool Disjoint(PathEdge other)
            {
                return (this.xs & other.xs) == EMPTY_SET;
            }

            /**
			 * Is the edge a loop and connects a vertex to its self.
			 *
			 * @return whether the edge is a loop
			 */
            public bool IsLoop => u == v;

            /**
			 * Access either endpoint of the edge.
			 *
			 * @return either endpoint.
			 */
            public int Either()
            {
                return u;
            }

            /**
             * Given one endpoint, retrieve the other endpoint.
             *
             * @param x an endpoint
             * @return the other endpoint.
             */
            public int Other(int x)
            {
                return u == x ? v : u;
            }

            /**
             * Total Length of the path formed by this edge. The value includes
             * endpoints and reduced vertices.
             *
             * @return Length of path
             */
            public abstract int Len();

            /**
             * Reconstruct the path through the edge by appending vertices to a
             * mutable {@link ArrayBuilder}.
             *
             * @param ab array builder to append vertices to
             * @return the array builder parameter for convenience
             */
            public abstract ArrayBuilder Reconstruct(ArrayBuilder ab);

            /**
			 * The path stored by the edge as a fixed size array of vertices.
			 *
			 * @return fixed size array of vertices which are in the path.
			 */
            public int[] Path()
            {
                return Reconstruct(new ArrayBuilder(Len()).Append(Either())).xs;
            }
        }

        /// <summary>A simple non-reduced edge, just the two end points.</summary>
        sealed class SimpleEdge
            : PathEdge
        {
            /**
			 * A new simple edge, with two endpoints.
			 *
			 * @param u an endpoint
			 * @param v another endpoint
			 */
            public SimpleEdge(int u, int v) 
				: base(u, v, EMPTY_SET)
            { 
            }

            /// <inheritdoc/>
            public override ArrayBuilder Reconstruct(ArrayBuilder ab)
            {
                return ab.Append(Other(ab.Prev()));
            }

            /// <inheritdoc/>
            public override int Len()
            {
                return 2;
            }
        }

        /**
		 * A reduced edge, made from two existing path edges and an endpoint they
		 * have in common.
		 */
        sealed class ReducedEdge
            : PathEdge
        {

            /// <summary>Reduced edges.</summary>
            private readonly PathEdge e, f;

            /**
			 * Create a new reduced edge from two existing edges and vertex they
			 * have in common.
			 *
			 * @param e an edge
			 * @param f another edge
			 * @param x a common vertex
			 */
            public ReducedEdge(PathEdge e, PathEdge f, int x)
				: base(e.Other(x), f.Other(x), e.xs | f.xs | 1L << x)
            {
                this.e = e;
                this.f = f;
            }

            /// <inheritdoc/>
            public override ArrayBuilder Reconstruct(ArrayBuilder ab)
            {
                return u == ab.Prev() ? f.Reconstruct(e.Reconstruct(ab)) : e.Reconstruct(f.Reconstruct(ab));
            }

            /// <inheritdoc/>
            public override int Len()
            {
                return Longs.BitCount(xs) + 2;
            }
        }

		/**
		 * A simple helper class for constructing a fixed size int[] array and
		 * sequentially appending vertices.
		 */
        sealed class ArrayBuilder
        {
            private int i = 0;
            public readonly int[] xs;

            /**
             * A new array builder of fixed size.
             *
             * @param n size of the array
             */
            public ArrayBuilder(int n)
            {
                xs = new int[n];
            }

            /**
             * Append a value to the end of the sequence.
             *
             * @param x a new value
             * @return self-reference for chaining
             */
            public ArrayBuilder Append(int x)
            {
                xs[i++] = x;
                return this;
            }

            /**
             * Previously value in the sequence.
             *
             * @return previous value
             */
            public int Prev()
            {
                return xs[i - 1];
            }
        }
    }
}
