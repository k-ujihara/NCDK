/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 * 			  John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; Either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - Adding the above
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
using System.Collections;
using NCDK.Common.Collections;

namespace NCDK.Graphs
{
    /**
	 * A Path graph (<b>P-Graph</b>) for graphs with more than 64 vertices - the
	 * P-Graph provides efficient generation of all simple cycles in a graph
	 * {@cdk.cite HAN96}. Vertices are sequentially Removed from the graph by
	 * reducing incident edges and forming new 'Path edges'. The order in which the
	 * vertices are to be Removed should be pre-defined in the constructor as the
	 * {@code rank[]} parameter.
	 *
	 * @author John May
	 * @author Till Schäfer (predefined vertex ordering)
	 * @cdk.module core
	 * @cdk.githash
	 * @see org.openscience.cdk.ringsearch.RingSearch
	 * @see org.openscience.cdk.graph.GraphUtil
	 * @see <a href="http://en.wikipedia.org/wiki/Biconnected_component">Wikipedia:
	 *      Biconnected Component</a>
	 */
    sealed class JumboPathGraph
        : PathGraph
    {
        private static readonly List<PathEdge> EmptyPathEdgeList = new List<PathEdge>();

        /// <summary>Path edges, indexed by their end points (incidence list).</summary>
        private readonly List<PathEdge>[] graph;

        /// <summary>Limit on the maximum Length of cycle to be found.</summary>
        private readonly int limit;

        /// <summary>Indicates when each vertex will be Removed, '0' = first, '|V|' = last.</summary>
        private readonly int[] rank;

        /**
		 * Create a regular Path graph (<b>P-Graph</b>) for the given molecule graph
		 * (<b>M-Graph</b>).
		 *
		 * @param mGraph The molecule graph (M-Graph) in adjacency list
		 *               representation.
		 * @param rank   Unique rank of each vertex - indicates when it will be
		 *               Removed.
		 * @param limit  Limit for size of cycles found, to find all cycles specify
		 *               the limit as the number of vertices in the graph.
		 * @ limit was invalid or the graph was too
		 *                                  large
		 * @     the molecule graph was not provided
		 */
        public JumboPathGraph(int[][] mGraph, int[] rank, int limit)
        {
            if (mGraph == null)
                throw new ArgumentNullException(nameof(mGraph), "no molecule graph");
            if (rank == null)
                throw new ArgumentNullException(nameof(rank), "no rank provided");

            this.graph = new List<PathEdge>[mGraph.Length];
            this.rank = rank;
            this.limit = limit + 1; // first/last vertex repeats
            int ord = graph.Length;

            // check configuration
            if (!(ord > 2))
                throw new ArgumentOutOfRangeException(nameof(graph), "graph was acyclic");
            if (!(limit >= 3 && limit <= ord))
                throw new ArgumentOutOfRangeException(nameof(graph), "limit should be from 3 to |V|");

            for (int v = 0; v < ord; v++)
                graph[v] = new List<JumboPathGraph.PathEdge>();

            // construct the Path-graph
            for (int v = 0; v < ord; v++)
            {
                foreach (int w in mGraph[v])
                {
                    if (w > v)
                        Add(new SimpleEdge(v, w, ord));
                }
            }
        }

        /**
		 * Add a Path-edge to the Path-graph. Edges are only Added to the vertex of
		 * lowest rank (see. constructor).
		 *
		 * @param edge Path edge
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
		 * Access edges which are incident to <i>x</i> and Remove them from the
		 * graph.
		 *
		 * @param x a vertex
		 * @return vertices incident to x
		 */
        private List<PathEdge> Remove(int x)
        {
            List<PathEdge> edges = graph[x];
            graph[x] = EmptyPathEdgeList;
            return edges;
        }

        /**
		 * Pairwise combination of all Disjoint <i>edges</i> incident to a vertex
		 * <i>x</i>.
		 *
		 * @param edges edges which are currently incident to <i>x</i>
		 * @param x     a vertex in the graph
		 * @return reduced edges
		 */
        private List<PathEdge> Combine(List<PathEdge> edges, int x)
        {
            int n = edges.Count;
            List<PathEdge> reduced = new List<PathEdge>(n);

            for (int i = 0; i < n; i++)
            {
                PathEdge e = edges[i];
                for (int j = i + 1; j < n; j++)
                {
                    PathEdge f = edges[j];
                    if (e.Disjoint(f)) reduced.Add(new ReducedEdge(e, f, x));
                }
            }

            return reduced;
        }

        /// <inheritdoc/>
        public override void Remove(int x, List<int[]> cycles)
        {
            List<PathEdge> edges = Remove(x);
            List<PathEdge> reduced = Combine(edges, x);

            foreach (PathEdge e in reduced)
            {
                if (e.Len() <= limit)
                {
                    if (e.IsLoop)
                        cycles.Add(e.Path());
                    else
                        Add(e);
                }
            }
        }

        /**
         * An abstract Path edge. A Path edge has two end points and 0 or more
         * reduced vertices which represent a Path between those endpoints.
         */
        public abstract class PathEdge
        {
            /// <summary>Endpoints of the edge.</summary>
            public int u, v;

            /// <summary>Bits indicate reduced vertices between endpoints (exclusive).</summary>
            public BitArray xs;

            /**
             * A new edge specified by two endpoints and a bit set indicating which
             * vertices have been reduced.
             *
             * @param u  an endpoint
             * @param v  the Other endpoint
             * @param xs reduced vertices between endpoints
             */
            public PathEdge(int u, int v, BitArray xs)
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
                return !BitArrays.Intersects(this.xs, other.xs);
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
             * @return Either endpoint.
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
			 * Total Length of the Path formed by this edge. The value includes
			 * endpoints and reduced vertices.
			 *
			 * @return Length of Path
			 */
            public abstract int Len();

            /**
			 * Reconstruct the Path through the edge by appending vertices to a
			 * mutable {@link ArrayBuilder}.
			 *
			 * @param ab array builder to append vertices to
			 * @return the array builder parameter for convenience
			 */
            public abstract ArrayBuilder Reconstruct(ArrayBuilder ab);

            /**
             * The Path stored by the edge as a fixed size array of vertices.
             *
             * @return fixed size array of vertices which are in the Path.
             */
            public int[] Path()
            {
                return Reconstruct(new ArrayBuilder(Len()).Append(Either())).xs;
            }
        }

        /// <summary>A simple non-reduced edge, just the two end points.</summary>
        public class SimpleEdge
            : PathEdge
        {

            /**
			 * A new simple edge, with two endpoints.
			 *
			 * @param u an endpoint
			 * @param v anOther endpoint
			 */
            public SimpleEdge(int u, int v, int size)
                : base(u, v, new BitArray(size)) // fixed CDK's bug. EMPTY_SET is not value type.
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
		 * A reduced edge, made from two existing Path edges and an endpoint they
		 * have in common.
		 */
        public sealed class ReducedEdge
            : PathEdge
        {
            /// <summary>Reduced edges.</summary>
            private readonly PathEdge e, f;

            /**
			 * Create a new reduced edge from two existing edges and vertex they
			 * have in common.
			 *
			 * @param e an edge
			 * @param f anOther edge
			 * @param x a common vertex
			 */
            public ReducedEdge(PathEdge e, PathEdge f, int x)
                : base(e.Other(x), f.Other(x), Union(e.xs, f.xs, x))
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
                int count = 0;
                foreach (bool b in xs)
                    if (b)
                        count++;
                return count + 2;
            }

            static BitArray Union(BitArray s, BitArray t, int x)
            {
                BitArray u = (BitArray)s.Clone();
                u.Or(t);
                u.Set(x, true);
                return u;
            }
        }

        /**
		 * A simple helper class for constructing a fixed size int[] array and
		 * sequentially appending vertices.
		 */
        public class ArrayBuilder
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
