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

namespace NCDK.Graphs
{
    /**
	 * Collection of static utilities for manipulating adjacency list
	 * representations stored as a {@literal int[][]}. May well be replaced in
	 * future with a <i>Graph</i> data type.
	 *
	 * @author John May
	 * @cdk.module core
	 * @cdk.githash
	 * @see ShortestPaths
	 * @see org.openscience.cdk.ringsearch.RingSearch
	 */
    public static class GraphUtil
    {
        /**
		 * Create an adjacent list representation of the {@literal container}.
		 *
		 * @param container the molecule
		 * @return adjacency list representation stored as an {@literal int[][]}.
		 * @     the container was null
		 * @ a bond was found which contained atoms
		 *                                  not in the molecule
		 */
        public static int[][] ToAdjList(IAtomContainer container)
        {
            return ToAdjList(container, null);
        }

        /**
		 * Create an adjacent list representation of the {@code container} and
		 * fill in the {@code bondMap} for quick lookup.
		 *
		 * @param container the molecule
		 * @param bondMap a map to index the bonds into
		 * @return adjacency list representation stored as an {@literal int[][]}.
		 * @     the container was null
		 * @ a bond was found which contained atoms
		 *                                  not in the molecule
		 */
        public static int[][] ToAdjList(IAtomContainer container, EdgeToBondMap bondMap)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            int n = container.Atoms.Count;

            List<int>[] graph = new List<int>[n];
            for (var i = 0; i < n; i++)
                graph[i] = new List<int>();

            foreach (var bond in container.Bonds)
            {
                int v = container.Atoms.IndexOf(bond.Atoms[0]);
                int w = container.Atoms.IndexOf(bond.Atoms[1]);

                if (v < 0 || w < 0)
                    throw new ArgumentException($"bond at index {container.Bonds.IndexOf(bond)} contained an atom not pressent in molecule");


                graph[v].Add(w);
                graph[w].Add(v);

                bondMap?.Add(v, w, bond);
            }

            int[][] agraph = new int[n][];
            for (int v = 0; v < n; v++)
            {
                agraph[v] = graph[v].ToArray();
            }

            return agraph;
        }

        /**
		 * Create a subgraph by specifying the vertices from the original {@literal
		 * graph} to {@literal include} in the subgraph. The provided vertices also
		 * provide the mapping between vertices in the subgraph and the original.
		 *
		 * <blockquote><pre>
		 * int[][] g  = ToAdjList(naphthalene);
		 * int[]   vs = new int[]{0, 1, 2, 3, 4, 5};
		 *
		 * int[][] h = Subgraph(g, vs);
		 * // for the vertices in h, the provided 'vs' gives the original index
		 * For(int v = 0; v < h.Length; v++) {
		 *     // vs[v] is 'v' in 'g'
		 * }
		 * </pre></blockquote>
		 *
		 * @param graph   adjacency list graph
		 * @param include the vertices of he graph to include in the subgraph
		 * @return the subgraph
		 */
        public static int[][] Subgraph(int[][] graph, int[] include)
        {
            // number of vertices in the graph and the subgraph
            int n = graph.Length;
            int m = include.Length;

            // mapping from vertex in 'graph' to 'subgraph'
            int[] mapping = new int[n];
            for (int i = 0; i < m; i++)
                mapping[include[i]] = i + 1;

            // initialise the subgraph
            List<int>[] subgraph = new List<int>[m];
            for (var i = 0; i < m; i++)
                subgraph[i] = new List<int>();

            // build the subgraph, in the subgraph we denote to adjacent
            // vertices p and q. If p or q is less then 0 then it is not
            // in the subgraph
            for (int v = 0; v < n; v++)
            {
                int p = mapping[v] - 1;
                if (p < 0)
                    continue;

                foreach (int w in graph[v])
                {
                    int q = mapping[w] - 1;
                    if (q < 0)
                        continue;
                    subgraph[p].Add(q);
                }
            }

            int[][] asubgraph = new int[m][];
            // truncate excess storage
            for (int p = 0; p < m; p++)
                asubgraph[p] = subgraph[p].ToArray();

            return asubgraph;
        }

        /**
         * Arrange the {@literal vertices} in a simple cyclic path. If the vertices
         * do not form such a path an {@link ArgumentException} is thrown.
         *
         * @param graph    a graph
         * @param vertices set of vertices
         * @return vertices in a walk which makes a cycle (first and last are the
         *         same)
         * @ thrown if the vertices do not form a
         *                                  cycle
         * @see org.openscience.cdk.ringsearch.RingSearch#Isolated()
         */
        public static int[] Cycle(int[][] graph, int[] vertices)
        {
            int n = graph.Length;
            int m = vertices.Length;

            // mark vertices
            bool[] marked = new bool[n];
            foreach (int v in vertices)
                marked[v] = true;

            int[] path = new int[m + 1];

            path[0] = path[m] = vertices[0];
            marked[vertices[0]] = false;

            for (int i = 1; i < m; i++)
            {
                int w = FirstMarked(graph[path[i - 1]], marked);
                if (w < 0)
                    throw new ArgumentException("broken path");
                path[i] = w;
                marked[w] = false;
            }

            // the path is a cycle if the start and end are adjacent, if this is
            // the case return the path
            foreach (int w in graph[path[m - 1]])
                if (w == path[0])
                    return path;

            throw new ArgumentException("path does not make a cycle");
        }

        /**
         * Find the first value in {@literal ws} which is {@literal marked}.
         *
         * @param xs     array of values
         * @param marked marked values
         * @return first marked value, -1 if none found
         */
        static int FirstMarked(int[] xs, bool[] marked)
        {
            foreach (int x in xs)
                if (marked[x])
                    return x;
            return -1;
        }

        /// <summary>Utility for storing {@link IBond}s indexed by vertex end points.</summary>
        public class EdgeToBondMap
        {
            Dictionary<Tuple, IBond> lookup = new Dictionary<Tuple, IBond>();

            public EdgeToBondMap()
            { }

            /**
             * Index a bond by the endpoints.
             *
             * @param v    an endpoint
             * @param w    another endpoint
             * @param bond the bond value
             */
            public void Add(int v, int w, IBond bond)
            {
                lookup[new Tuple(v, w)] = bond;
            }

            /**
             * Access the bond store at the end points v and w. If no bond is
             * store, null is returned.
             *
             * @param v an endpoint
             * @param w another endpoint
             * @return the bond stored for the endpoints
             */
            public IBond this[int v, int w]
            {
                get
                {
                    IBond bond;
                    if (lookup.TryGetValue(new Tuple(v, w), out bond))
                        return bond;
                    return null;
                }
            }

            /**
             * Create a map with enough space for all the bonds in the molecule,
             * {@code container}. Note - the map is not filled by this method.
             *
             * @param container the container
             * @return a map with enough space for the container
             */
            public static EdgeToBondMap WithSpaceFor(IAtomContainer container)
            {
                return new EdgeToBondMap();
            }

            /**
             * Unordered storage of two int values. Mainly useful to index bonds by
             * it's vertex end points.
             */
            struct Tuple
            {
                private readonly int u, v;

                /// <summary>
                /// Create a new tuple with the specified values.
                /// </summary>
                /// <param name="u">a value</param>
                /// <param name="v">another value</param>
                public Tuple(int u, int v)
                {
                    this.u = u;
                    this.v = v;
                }

                public override bool Equals(object o)
                {
                    if (o is Tuple)
                    {
                        Tuple that = (Tuple)o;
                        return this.u == that.u && this.v == that.v || this.u == that.v && this.v == that.u;
                    }
                    return false;
                }

                public override int GetHashCode()
                {
                    return u ^ v;
                }
            }
        }
    }
}
