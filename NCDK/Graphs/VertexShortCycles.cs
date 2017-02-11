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
using static NCDK.Graphs.InitialCycles;
using static NCDK.Common.Base.Preconditions;
using NCDK.Common.Collections;

namespace NCDK.Graphs
{
    /**
     * Determine the set of cycles which are the shortest through each vertex.
     * Unlike the Smallest Set of Smallest Rings (SSSR), linear dependence of
     * each cycle does not need to be verified.
     *
     * @author John May
     * @cdk.module core
     */
#if TEST
    public
#endif
    sealed class VertexShortCycles {

        /// <summary>Shortest cycles stored as closed walks.</summary>
        private IList<int[]> paths;

        /** Construct the vertex short cycles for the
         *  given graph. */
        public VertexShortCycles(int[][] graph)
            : this(new InitialCycles(graph))
        { }

        /** Construct the vertex short cycles for the
         *  given initial cycles. */
        public VertexShortCycles(InitialCycles initialCycles) {

            int[][] graph = initialCycles.Graph;
            int[] sizeOf = new int[graph.Length];

            this.paths = new List<int[]>(initialCycles.GetNumberOfCycles());

            // cycles are returned ordered by length
            foreach (var cycle in initialCycles.GetCycles()) {
                 int length = cycle.Length;
                 int[] path = cycle.Path;

                bool found = false;

                // check if any vertex is the shortest through a vertex in the path
                foreach (var v in path) {
                    if (sizeOf[v] < 1 || length <= sizeOf[v]) {
                        found = true;
                        sizeOf[v] = length;
                    }
                }

                if (found) {
                    foreach (var p in cycle.GetFamily()) {
                        paths.Add(p);
                    }
                }
            }
        }

        /**
		 * The paths of the shortest cycles, that paths are closed walks such that
		 * the last and first vertex is the same.
		 *
		 * @return the paths
		 */
        public int[][] GetPaths() {
            int[][] paths = new int[this.paths.Count][];
            for (int i = 0; i < this.paths.Count; i++)
                paths[i] = this.paths[i];
            return paths;
        }

        /**
		 * The size of the shortest cycles set.
		 *
		 * @return number of cycles
		 */
#if TEST
        public
#endif
        int Count => paths.Count;
    }
}
