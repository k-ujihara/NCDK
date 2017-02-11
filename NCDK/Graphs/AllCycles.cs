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
     * Compute all simple cycles (rings) in a graph. Generally speaking one does not
     * need all the cycles and tractable subsets offer good alternatives.
     *
     * <ul> <li>EdgeShortCycles - the smallest cycle through each edge</li>
     * <li>{@link RelevantCycles} - union of all minimum cycle bases - unique but
     * may be exponential</li> <li>{@link EssentialCycles} - intersection of all
     * minimum cycle bases </li> <li>{@link MinimumCycleBasis} - a minimum cycles
     * basis, may not be unique. Often used interchangeable with the term SSSR.</li>
     * </ul>
     *
     *
     * For maximum performance the algorithm should be run only on ring systems (a
     * biconnected component with at least one tri-connected vertex). An example of
     * this is shown below:
     *
     * <blockquote><pre>
     * // convert the molecule to adjacency list - may be redundant in future
     * IAtomContainer m =...;
     * int[][] g = GraphUtil.ToAdjList(m);
     *
     * // efficient computation/partitioning of the ring systems
     * RingSearch rs = new RingSearch(m, g);
     *
     * // isolated cycles don't need to be run
     * rs.Isolated();
     *
     * // process fused systems separately
     * foreach (var fused : rs.FUsed()) {
     *     // given the fused subgraph, max cycle size is
     *     // the number of vertices
     *     AllCycles ac = new AllCycles(Subgraph(g, fused),
     *                                  fused.Length,
     *                                  maxDegree);
     *    // cyclic walks
     *    int[][] paths = ac.GetPaths();
     * }
     * </pre></blockquote>
     *
     * @author John May
     * @cdk.module core
     * @cdk.githash
     * @see RegularPathGraph
     * @see JumboPathGraph
     * @see org.openscience.cdk.ringsearch.RingSearch
     * @see GraphUtil
     * @see <a href="http://efficientbits.blogspot.co.uk/2013/06/allringsfinder-sport-edition.html">Performance
     *      Analysis (Blog Post)</a>
     */
    public sealed class AllCycles
    {
        /// <summary>All simple cycles.</summary>
        private readonly List<int[]> cycles = new List<int[]>();

        /// <summary>Indicates whether the perception completed.</summary>
        private readonly bool completed;

        /**
         * Compute all simple cycles up to given <i>maxCycleSize</i> in the provided
         * <i>graph</i>. In some graphs the topology makes it impracticable to
         * compute all the simple. To avoid running forever on these molecules the
         * <i>maxDegree</i> provides an escape clause. The value doesn't quantify
         * how many cycles we get. The percentage of molecules in PubChem Compound
         * (Dec '12) which would successfully complete for a given Degree are listed
         * below.
         *
         * <table> <tr><th>Percent</th><th>Max Degree</th></tr>
         * <tr><td>99%</td><td>9</td></tr> <tr><td>99.95%</td><td>72</td></tr>
         * <tr><td>99.96%</td><td>84</td></tr> <tr><td>99.97%</td><td>126</td></tr>
         * <tr><td>99.98%</td><td>216</td></tr> <tr><td>99.99%</td><td>684</td></tr>
         * </table>
         *
         * @param graph        adjacency list representation of a graph
         * @param maxCycleSize the maximum cycle size to perceive
         * @param maxDegree    escape clause to stop the algorithm running forever
         */
        public AllCycles(int[][] graph, int maxCycleSize, int maxDegree)
        {
            // get the order in which we remove vertices, the rank tells us
            // the index in the ordered array of each vertex
            int[] rank = GetRank(graph);
            int[] vertices = GetVerticesInOrder(rank);

            PathGraph pGraph;
            if (graph.Length < 64)
                pGraph = new RegularPathGraph(graph, rank, maxCycleSize);
            else
                pGraph = new JumboPathGraph(graph, rank, maxCycleSize);

            // perceive the cycles by removing the vertices in order
            int removed = 0;
            foreach (int v in vertices)
            {

                if (pGraph.Degree(v) > maxDegree) break; // or could throw exception...

                pGraph.Remove(v, cycles);
                removed++;
            }

            completed = removed == graph.Length;
        }

        /**
         * Using the pre-computed rank, get the vertices in order.
         *
         * @param rank see {@link #rank(int[][])}
         * @return vertices in order
         */
        static int[] GetVerticesInOrder(int[] rank)
        {
            int[] vs = new int[rank.Length];
            for (int v = 0; v < rank.Length; v++)
                vs[rank[v]] = v;
            return vs;
        }

        /**
         * Compute a rank for each vertex. This rank is based on the Degree and
         * indicates the position each vertex would be in a sorted array.
         *
         * @param g a graph in adjacent list representation
         * @return array indicating the rank of each vertex.
         */
        static int[] GetRank(int[][] g)
        {
            int ord = g.Length;

            int[] count = new int[ord + 1];
            int[] rank = new int[ord];

            // frequency of each Degree
            for (int v = 0; v < ord; v++)
                count[g[v].Length + 1]++;
            // cumulated counts
            for (int i = 0; count[i] < ord; i++)
                count[i + 1] += count[i];
            // store sorted position of each vertex
            for (int v = 0; v < ord; v++)
                rank[v] = count[g[v].Length]++;

            return rank;
        }

        /**
         * The paths describing all simple cycles in the given graph. The path stats
         * and ends vertex.
         *
         * @return 2d array of paths
         */
        public int[][] GetPaths()
        {
            int[][] paths = new int[cycles.Count][];
            for (int i = 0; i < cycles.Count; i++)
                paths[i] = (int[])cycles[i].Clone();
            return paths;
        }

        /**
         * Cardinality of the set.
         *
         * @return number of cycles
         */
        public int Count => cycles.Count;

        /**
         * Did the cycle perception complete - if not the molecule was considered
         * impractical and computation was aborted.
         *
         * @return algorithm completed
         */
        public bool Completed => completed;
    }
}
