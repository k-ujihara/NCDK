/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by the
 * Free Software Foundation; either version 2.1 of the License, or (at your
 * option) any later version. All we ask is that proper credit is given for our
 * work, which includes - but is not limited to - adding the above copyright
 * notice to the beginning of your source code files, and to any copyright
 * notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using NCDK.Common.Collections;

namespace NCDK.RingSearches
{
    /**
     * CyclicVertexSearch for graphs with more then 64 vertices.
     *
     * @author John May
     * @cdk.module core
     */
#if TEST
	public 
#endif
        class JumboCyclicVertexSearch 
		: CyclicVertexSearch
    {
        /* graph representation */
        private readonly int[][] g;

        /* set of known cyclic vertices */
        private readonly BitArray  cyclic;

		/* cycle systems as they are discovered */
		private List<BitArray> cycles = new List<BitArray>(1);

        /* indicates if the 'cycle' at 'i' in 'cycles' is fused */
        private List<bool> fused = new List<bool>(1);

        /* set of visited vertices */
        private BitArray visited;

        /* the vertices in our path at a given vertex index */
        private BitArray[] state;

        /// <summary>vertex colored by each component.</summary>
        private int[] colors;

        /**
		 * Create a new cyclic vertex search for the provided graph.
		 *
		 * @param graph adjacency list representation of a graph
		 */
        public JumboCyclicVertexSearch(int[][] graph) {
            this.g = graph;
            int n = graph.Length;

            cyclic = new BitArray(n);

            if (n == 0) return;

            state = new BitArray[n];
            visited = new BitArray(n);

            BitArray empty = new BitArray(n);

            // start from vertex 0
            Search(0, Copy(empty), Copy(empty));

            // if g is a fragment we will not have visited everything
            int v = 0;
            while (BitArrays.Cardinality(visited) != n) {
                v++;
                // each search expands to the whole fragment, as we
                // may have fragments we need to visit 0 and then
                // check every other vertex
                if (!visited[v]) {
                    Search(v, Copy(empty), Copy(empty));
                }
            }

            // allow the states to be collected
            state = null;
            visited = null;

        }

        /**
		 * Perform a depth first search from the vertex <i>v</i>.
		 *
		 * @param v    vertex to search from
		 * @param prev the state before we vistaed our parent (previous state)
		 * @param curr the current state (including our parent)
		 */
        private void Search(int v, BitArray prev, BitArray curr) {

            state[v] = curr; // set the state before we visit v
            curr = Copy(curr); // include v in our current state (state[v] is unmodified)
            curr.Set(v, true);
            visited.Or(curr); // mark v as visited (or being visited)

            // for each neighbor w of v
            foreach (var w in g[v]) {

                // if w is in our prev state we have a cycle of size >3.
                // we don't check out current state as this will always
                // include w - they are adjacent
                if (prev[w]) {
                    // we have a cycle, xor the state when we last visited 'w'
                    // with our current state. this set is all the vertices
                    // we visited since then
                    Add(Xor(state[w], curr));
                }

                // check w hasn't been visited or isn't being visited further up the stack.
                // this mainly stops us re-visiting the vertex we came from
                else if (!visited[w]) {
                    // recursively call for the neighbor 'w'
                    Search(w, state[v], curr);
                }
            }

        }

        /// <summary>Synchronisation lock.</summary>
        private readonly object syncLock = new object();

        /**
		 * Lazily build an indexed lookup of vertex color. The vertex color
		 * indicates which cycle a given vertex belongs. If a vertex belongs to more
		 * then one cycle it is colored '0'. If a vertex belongs to no cycle it is
		 * colored '-1'.
		 *
		 * @return vertex colors
		 */
        public int[] VertexColor() {
            int[] result = colors;
            if (result == null) {
                lock (syncLock) {
                    result = colors;
                    if (result == null) {
                        colors = result = BuildVertexColor();
                    }
                }
            }
            return result;
        }

        /**
		 * Build an indexed lookup of vertex color. The vertex color indicates which
		 * cycle a given vertex belongs. If a vertex belongs to more then one cycle
		 * it is colored '0'. If a vertex belongs to no cycle it is colored '-1'.
		 *
		 * @return vertex colors
		 */
        private int[] BuildVertexColor() {
            int[] color = new int[g.Length];

            int n = 1;
            Arrays.Fill(color, -1);
            foreach (var cycle in cycles) {
                for (int i = BitArrays.NextSetBit(cycle, 0); i >= 0; i = BitArrays.NextSetBit(cycle, i + 1)) {
                    color[i] = color[i] < 0 ? n : 0;
                }
                n++;
            }

            return color;
        }

        /**
		 * @inheritDoc
		 */
        public bool Cyclic(int v) {
            return cyclic[v];
        }

        /**
		 * @inheritDoc
		 */
        public bool Cyclic(int u, int v) {

             int[] colors = VertexColor();

            // if either vertex has no color then the edge can not
            // be cyclic
            if (colors[u] < 0 || colors[v] < 0) return false;

            // if the vertex color is 0 it is shared between
            // two components (i.e. spiro-rings) we need to
            // check each component
            if (colors[u] == 0 || colors[v] == 0) {
                // either vertices are shared - need to do the expensive check
                foreach (var cycle in cycles) {
                    if (cycle[u] && cycle[v]) return true;
                }
                return false;
            }

            // vertex is not shared between components
            return colors[u] == colors[v];
        }

        /**
		 * @inheritDoc
		 */
       public int[] Cyclic() {
            return ToArray(cyclic);
        }

        /**
		 * @inheritDoc
		 */
        public int[][] Isolated() {
            List<int[]> isolated = new List<int[]>(cycles.Count());
            for (int i = 0; i < cycles.Count(); i++) {
                if (!fused[i]) isolated.Add(ToArray(cycles[i]));
            }
            return isolated.ToArray();
        }

        /**
		 * @inheritDoc
		 */
        public int[][] FUsed() {
            List<int[]> fused = new List<int[]>(cycles.Count());
            for (int i = 0; i < cycles.Count(); i++) {
                if (this.fused[i]) fused.Add(ToArray(cycles[i]));
            }
            return fused.ToArray();
        }

        /**
		 * Add the cycle vertices to our discovered cycles. The cycle is first
		 * checked to see if it is isolated (shares at most one vertex) or
		 * <i>potentially</i> fused.
		 *
		 * @param cycle newly discovered cyclic vertex set
		 */
        private void Add(BitArray cycle) {

            BitArray intersect = And(cycle, cyclic);

            if (BitArrays.Cardinality(intersect) > 1) {
                AddFUsed(cycle);
            } else {
                AddIsolated(cycle);
            }

            cyclic.Or(cycle);

        }

        /**
		 * Add an a new isolated cycle which is currently edge disjoint with all
		 * other cycles.
		 *
		 * @param cycle newly discovered cyclic vertices
		 */
        private void AddIsolated(BitArray cycle) {
            cycles.Add(cycle);
            fused.Add(false);
        }

        /**
		 * Adds a <i>potentially</i> fused cycle. If the cycle is discovered not be
		 * fused it will still be added as isolated.
		 *
		 * @param cycle vertex set of a potentially fused cycle, indicated by the
		 *              set bits
		 */
        private void AddFUsed(BitArray cycle) {

            int i = IndexOfFUsed(0, cycle);

            if (i != -1) {
                // add new cycle and mark as fused
                cycles[i].Or(cycle);
                fused[i] = true;
                int j = i;

                // merge other cycles we could be fused with into 'i'
                while ((j = IndexOfFUsed(j + 1, cycle)) != -1) {
                    cycles[i].Or(cycles[j]);
                    cycles.RemoveAt(j);
                    fused.RemoveAt(j);
                    j--;
                }
            } else {
                AddIsolated(cycle);
            }

        }

        /**
		 * Find the next index that the <i>cycle</i> intersects with by at least two
		 * vertices. If the intersect of a vertex set with another contains more
		 * then two vertices it cannot be edge disjoint.
		 *
		 * @param start start searching from here
		 * @param cycle test whether any current cycles are fused with this one
		 * @return the index of the first fused after 'start', -1 if none
		 */
        private int IndexOfFUsed(int start, BitArray cycle) {
            for (int i = start; i < cycles.Count(); i++) {
                if (BitArrays.Cardinality(And(cycles[i], cycle)) > 1) {
                    return i;
                }
            }
            return -1;
        }

        /**
		 * Convert the set bits of a BitArray to an int[].
		 *
		 * @param set input with 0 or more set bits
		 * @return the bits which are set in the input
		 */
        public static int[] ToArray(BitArray set) {
            int[] vertices = new int[BitArrays.Cardinality(set)];
            int i = 0;

            // fill the cyclic vertices with the bits that have been set
            for (int v = 0; i < vertices.Length; v++) {
                if (set[v]) {
                    vertices[i++] = v;
                }
            }

            return vertices;
        }

        /**
		 * XOR the to bit sets together and return the result. Neither input is
		 * modified.
		 *
		 * @param x first bit set
		 * @param y second bit set
		 * @return the XOR of the two bit sets
		 */
        public static BitArray Xor(BitArray x, BitArray y) {
            BitArray z = Copy(x);
            z.Xor(y);
            return z;
        }

        /**
		 * AND the to bit sets together and return the result. Neither input is
		 * modified.
		 *
		 * @param x first bit set
		 * @param y second bit set
		 * @return the AND of the two bit sets
		 */
        public static BitArray And(BitArray x, BitArray y) {
            BitArray z = Copy(x);
            z.And(y);
            return z;
        }

        /**
		 * Copy the original bit set.
		 *
		 * @param org input bit set
		 * @return copy of the input
		 */
        public static BitArray Copy(BitArray org) {
            BitArray cpy = (BitArray)org.Clone();
            return cpy;
        }

    }
}
