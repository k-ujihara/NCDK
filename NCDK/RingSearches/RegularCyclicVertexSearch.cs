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
using NCDK.Common.Primitives;
using NCDK.Common.Collections;

namespace NCDK.RingSearches
{
    /**
     * CyclicVertexSearch for graphs with 64 vertices or less. This search is
     * optimised using primitive {@literal long} values to represent vertex sets.
     *
     * @author John May
     * @cdk.module core
     */
#if TEST
    public
#endif
    class RegularCyclicVertexSearch
        : CyclicVertexSearch
    {
        /* graph representation */
        private readonly int[][] g;

        /* set of known cyclic vertices */
        private long cyclic;

        /* cycle systems as they are discovered */
        private List<long> cycles = new List<long>(1);

        /* indicates if the 'cycle' at 'i' in 'cycles' is fused */
        private List<bool> fused = new List<bool>(1);

        /* set of visited vertices */
        private long visited;

        /* the vertices in our path at a given vertex index */
        private long[] state;

        /// <summary>Vertex colors - which component does each vertex belong.</summary>
        private volatile int[] colors;

        /**
         * Create a new cyclic vertex search for the provided graph.
         *
         * @param graph adjacency list representation of a graph
         */
#if TEST
        public
#else
            internal
#endif
        RegularCyclicVertexSearch(int[][] graph)
        {

            this.g = graph;
            int n = graph.Length;

            // skip search if empty graph
            if (n == 0) return;

            state = new long[n];

            // start from vertex 0
            Search(0, 0L, 0L);

            // if disconnected we have not visited all vertices
            int v = 1;
            while (Longs.BitCount(visited) != n)
            {

                // haven't visited v, start a new search from there
                if (!Visited(v))
                {
                    Search(v, 0L, 0L);
                }
                v++;
            }

            // no longer needed for the lifetime of the object
            state = null;

        }

        /**
         * Perform a depth first search from the vertex <i>v</i>.
         *
         * @param v    vertex to search from
         * @param prev the state before we vistaed our parent (previous state)
         * @param curr the current state (including our parent)
         */
        private void Search(int v, long prev, long curr)
        {

            state[v] = curr; // store the state before we visited v
            curr = SetBit(curr, v); // include v in our current state (state[v] is unmodified)
            visited |= curr; // mark v as visited (or being visited)

            // neighbors of v
            foreach (var w in g[v])
            {

                // w has been visited or is partially visited further up stack
                if (Visited(w))
                {

                    // if w is in our prev state we have a cycle of size >2.
                    // we don't check out current state as this will always
                    // include w - they are adjacent
                    if (IsBitSet(prev, w))
                    {

                        // xor the state when we last visited 'w' with our current
                        // state. this set is all the vertices we visited since then
                        // and are all in a cycle
                        Add(state[w] ^ curr);
                    }
                }
                else
                {
                    // recursively call for the unvisited neighbor w
                    Search(w, state[v], curr);
                }
            }
        }

        /**
         * Returns whether the vertex 'v' has been visited.
         *
         * @param v a vertex
         * @return whether the vertex has been visited
         */
        private bool Visited(int v)
        {
            return IsBitSet(visited, v);
        }

        /**
         * Add the cycle vertices to our discovered cycles. The cycle is first
         * checked to see if it is isolated (shares at most one vertex) or
         * <i>potentially</i> fused.
         *
         * @param cycle newly discovered cyclic vertex set
         */
        private void Add(long cycle)
        {

            long intersect = cyclic & cycle;

            // intersect by more then 1 vertex, we 'may' have a fused cycle
            if (intersect != 0 && Longs.BitCount(intersect) > 1)
            {
                AddFUsed(cycle);
            }
            else
            {
                AddIsolated(cycle);
            }

            cyclic |= cycle;

        }

        /**
         * Add an a new isolated cycle which is currently edge disjoint with all
         * other cycles.
         *
         * @param cycle newly discovered cyclic vertices
         */
        private void AddIsolated(long cycle)
        {
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
        private void AddFUsed(long cycle)
        {

            // find index of first fused cycle
            int i = IndexOfFUsed(0, cycle);

            if (i != -1)
            {
                // include the new cycle vertices and mark as fused
                cycles[i] = cycle | cycles[i];
                fused[i] = true;
                
                // merge other cycles we are share an edge with
                int j = i;
                while ((j = IndexOfFUsed(j + 1, cycles[i])) != -1)
                {
                    var newval = cycles[j] | cycles[i];
                    cycles[i] = newval;
                    cycles.RemoveAt(j);
                    fused.RemoveAt(j);
                    j--; // removed a vertex, need to move back one
                }
            }
            else
            {
                // edge disjoint
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
        private int IndexOfFUsed(int start, long cycle)
        {
            for (int i = start; i < cycles.Count(); i++)
            {
                long intersect = cycles[i] & cycle;
                if (intersect != 0 && Longs.BitCount(intersect) > 1)
                {
                    return i;
                }
            }
            return -1;
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
        public int[] VertexColor()
        {
            int[] result = colors;
            if (result == null)
            {
                lock (syncLock)
                {
                    result = colors;
                    if (result == null)
                    {
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
        private int[] BuildVertexColor()
        {
            int[] color = new int[g.Length];
            int n = 1;
            Arrays.Fill(color, -1);
            foreach (var l_cycle in cycles)
            {
                var cycle = l_cycle;
                for (int i = 0; i < g.Length; i++)
                {
                    if ((cycle & 0x1) == 0x1)
                        color[i] = color[i] < 0 ? n : 0;
                    cycle >>= 1;
                }
                n++;
            }

            return color;
        }

        /**
         * @inheritDoc
         */
        public bool Cyclic(int v)
        {
            return IsBitSet(cyclic, v);
        }

        /**
         * @inheritDoc
         */
        public bool Cyclic(int u, int v)
        {

            int[] colors = VertexColor();

            // if either vertex has no color then the edge can not
            // be cyclic
            if (colors[u] < 0 || colors[v] < 0) return false;

            // if the vertex color is 0 it is shared between
            // two components (i.e. spiro-rings) we need to
            // check each component
            if (colors[u] == 0 || colors[v] == 0)
            {
                // either vertices are shared - need to do the expensive check
                foreach (var cycle in cycles)
                {
                    if (IsBitSet(cycle, u) && IsBitSet(cycle, v))
                    {
                        return true;
                    }
                }
                return false;
            }

            // vertex is not shared between components check the colors match (i.e.
            // in same component)
            return colors[u] == colors[v];
        }

        /**
         * @inheritDoc
         */
        public int[] Cyclic()
        {
            return ToArray(cyclic);
        }

        /**
         * @inheritDoc
         */
        public int[][] Isolated()
        {
            List<int[]> isolated = new List<int[]>(cycles.Count());
            for (int i = 0; i < cycles.Count(); i++)
            {
                if (!fused[i]) isolated.Add(ToArray(cycles[i]));
            }
            return isolated.ToArray();
        }

        /**
         * @inheritDoc
         */
        public int[][] FUsed()
        {
            List<int[]> fused = new List<int[]>(cycles.Count());
            for (int i = 0; i < cycles.Count(); i++)
            {
                if (this.fused[i]) fused.Add(ToArray(cycles[i]));
            }
            return fused.ToArray();
        }

        /**
         * Convert the bits of a {@code long} to an array of integers. The size of
         * the output array is the number of bits set in the value.
         *
         * @param set value to convert
         * @return array of the set bits in the long value
         */
#if TEST
        public
#endif
        static int[] ToArray(long set)
        {

            int[] vertices = new int[Longs.BitCount(set)];
            int i = 0;

            // fill the cyclic vertices with the bits that have been set
            for (int v = 0; i < vertices.Length; v++)
            {
                if (IsBitSet(set, v)) vertices[i++] = v;
            }

            return vertices;
        }

        /**
         * Determine if the specified bit on the value is set.
         *
         * @param value bits indicate that vertex is in the set
         * @param bit   bit to test
         * @return whether the specified bit is set
         */
#if TEST
        public
#endif
        static bool IsBitSet(long value, int bit)
        {
            return (value & 1L << bit) != 0;
        }

        /**
         * Set the specified bit on the value and return the modified value.
         *
         * @param value the value to set the bit on
         * @param bit   the bit to set
         * @return modified value
         */
#if TEST
        public
#endif
            static long SetBit(long value, int bit)
        {
            return value | 1L << bit;
        }

    }
}
