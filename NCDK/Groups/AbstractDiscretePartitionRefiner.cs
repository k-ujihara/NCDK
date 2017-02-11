/* Copyright (C) 2012  Gilleain Torrance <gilleain.torrance@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System.Text;

namespace NCDK.Groups
{
    /**
     * Refines vertex partitions until they are discrete, and therefore equivalent
     * to permutations. These permutations are automorphisms of the graph that was
     * used during the refinement to guide the splitting of partition blocks.
     *
     * @author maclean
     * @cdk.module group
     */
    public abstract class AbstractDiscretePartitionRefiner
    {

        /**
         * The result of a comparison between the current partition
         * and the best permutation found so far.
         *
         */
        public enum Result
        {
            WORSE, EQUAL, BETTER
        };

        /**
         * If true, then at least one partition has been refined
         * to a permutation (IE extends to a discrete partition).
         */
        private bool bestExist;

        /**
         * The best permutation is the one that gives the maximal
         * half-matrix string (so far) when applied to the graph.
         */
        private Permutation best;

        /**
         * The first permutation seen when refining.
         */
        private Permutation first;

        /**
         * An equitable refiner.
         */
        private IEquitablePartitionRefiner equitableRefiner;

        /**
         * The automorphism group that is used to prune the search.
         */
        private PermutationGroup group;

        /**
         * A refiner - it is necessary to call {@link #setup} before use.
         */
        public AbstractDiscretePartitionRefiner()
        {
            this.bestExist = false;
            this.best = null;
            this.equitableRefiner = null;
        }

        /**
         * Get the number of vertices in the graph to be refined.
         *
         * @return a count of the vertices in the underlying graph
         */
        public abstract int GetVertexCount();

        /**
         * Get the connectivity between two vertices as an integer, to allow
         * for multigraphs : so a single edge is 1, a double edge 2, etc. If
         * there is no edge, then 0 should be returned.
         *
         * @param vertexI a vertex of the graph
         * @param vertexJ a vertex of the graph
         * @return the multiplicity of the edge (0, 1, 2, 3, ...)
         */
        public abstract int GetConnectivity(int vertexI, int vertexJ);

        /**
         * Setup the group and refiner; it is important to call this method before
         * calling {@link #refine} otherwise the refinement process will fail.
         *
         * @param group a group (possibly empty) of automorphisms
         * @param refiner the equitable refiner
         */
        public void Setup(PermutationGroup group, IEquitablePartitionRefiner refiner)
        {
            this.bestExist = false;
            this.best = null;
            this.group = group;
            this.equitableRefiner = refiner;
        }

        /**
         * Check that the first refined partition is the identity.
         *
         * @return true if the first is the identity permutation
         */
        public bool FirstIsIdentity()
        {
            return this.first.IsIdentity();
        }

        /**
         * The automorphism partition is a partition of the elements of the group.
         *
         * @return a partition of the elements of group
         */
        public Partition GetAutomorphismPartition()
        {
            int n = group.Count;
            DisjointSetForest forest = new DisjointSetForest(n);
            group.Apply(new AutomorphismPartitionBacktracker(n, forest));

            // convert to a partition
            Partition partition = new Partition();
            foreach (var set in forest.GetSets())
            {
                partition.AddCell(set);
            }

            // necessary for comparison by string
            partition.Order();
            return partition;
        }

        class AutomorphismPartitionBacktracker
            : PermutationGroup.Backtracker
        {
            int n;
            DisjointSetForest forest;
            bool[] inOrbit;
            private int inOrbitCount = 0;
            private bool isFinished;

            public AutomorphismPartitionBacktracker(int n, DisjointSetForest forest)
            {
                this.n = n;
                this.forest = forest;
                inOrbit = new bool[n];
            }

            public bool IsFinished()
            {
                return isFinished;
            }

            public void ApplyTo(Permutation p)
            {
                for (int elementX = 0; elementX < n; elementX++)
                {
                    if (inOrbit[elementX])
                    {
                        continue;
                    }
                    else
                    {
                        int elementY = p[elementX];
                        while (elementY != elementX)
                        {
                            if (!inOrbit[elementY])
                            {
                                inOrbitCount++;
                                inOrbit[elementY] = true;
                                forest.MakeUnion(elementX, elementY);
                            }
                            elementY = p[elementY];
                        }
                    }
                }
                if (inOrbitCount == n)
                {
                    isFinished = true;
                }
            }
        }

        /**
         * Get the upper-half of the adjacency matrix under the permutation.
         *
         * @param permutation a permutation of the adjacency matrix
         * @return a string containing the permuted values of half the matrix
         */
        public string GetHalfMatrixString(Permutation permutation)
        {
            StringBuilder builder = new StringBuilder(permutation.Count);
            int size = permutation.Count;
            for (int indexI = 0; indexI < size - 1; indexI++)
            {
                for (int indexJ = indexI + 1; indexJ < size; indexJ++)
                {
                    builder.Append(GetConnectivity(permutation[indexI], permutation[indexJ]));
                }
            }
            return builder.ToString();
        }

        /**
         * Get the half-matrix string under the best permutation.
         *
         * @return the upper-half adjacency matrix string permuted by the best
         */
        public string GetBestHalfMatrixString()
        {
            return GetHalfMatrixString(best);
        }

        /**
         * Get the half-matrix string under the first permutation.
         *
         * @return the upper-half adjacency matrix string permuted by the first
         */
        public string GetFirstHalfMatrixString()
        {
            return GetHalfMatrixString(first);
        }

        /**
         * Get the initial (unpermuted) half-matrix string.
         *
         * @return the upper-half adjacency matrix string
         */
        public string GetHalfMatrixString()
        {
            return GetHalfMatrixString(new Permutation(GetVertexCount()));
        }

        /**
         * Get the automorphism group used to prune the search.
         *
         * @return the automorphism group
         */
        public PermutationGroup GetAutomorphismGroup()
        {
            return this.group;
        }

        /**
         * Get the best permutation found.
         *
         * @return the permutation that gives the maximal half-matrix string
         */
        public Permutation GetBest()
        {
            return this.best;
        }

        /**
         * Get the first permutation reached by the search.
         *
         * @return the first permutation reached
         */
        public Permutation GetFirst()
        {
            return this.first;
        }

        /**
         * Check for a canonical graph, without generating the whole
         * automorphism group.
         *
         * @return true if the graph is canonical
         */
        public bool IsCanonical()
        {
            return best.IsIdentity();
        }

        /**
         * Refine the partition. The main entry point for subclasses.
         *
         * @param partition the initial partition of the vertices
         */
        public void Refine(Partition partition)
        {
            Refine(this.group, partition);
        }

        /**
         * Does the work of the class, that refines a coarse partition into a finer
         * one using the supplied automorphism group to prune the search.
         *
         * @param group the automorphism group of the graph
         * @param coarser the partition to refine
         */
        private void Refine(PermutationGroup group, Partition coarser)
        {
            int vertexCount = GetVertexCount();

            Partition finer = equitableRefiner.Refine(coarser);

            int firstNonDiscreteCell = finer.GetIndexOfFirstNonDiscreteCell();
            if (firstNonDiscreteCell == -1)
            {
                firstNonDiscreteCell = vertexCount;
            }

            Permutation pi1 = new Permutation(firstNonDiscreteCell);

            Result result = Result.BETTER;
            if (bestExist)
            {
                pi1 = finer.SetAsPermutation(firstNonDiscreteCell);
                result = CompareRowwise(pi1);
            }

            // partition is discrete
            if (finer.Count == vertexCount)
            {
                if (!bestExist)
                {
                    best = finer.ToPermutation();
                    first = finer.ToPermutation();
                    bestExist = true;
                }
                else
                {
                    if (result == Result.BETTER)
                    {
                        best = new Permutation(pi1);
                    }
                    else if (result == Result.EQUAL)
                    {
                        group.Enter(pi1.Multiply(best.Invert()));
                    }
                }
            }
            else
            {
                if (result != Result.WORSE)
                {
                    var blockCopy = finer.CopyBlock(firstNonDiscreteCell);
                    for (int vertexInBlock = 0; vertexInBlock < vertexCount; vertexInBlock++)
                    {
                        if (blockCopy.Contains(vertexInBlock))
                        {
                            Partition nextPartition = finer.SplitBefore(firstNonDiscreteCell, vertexInBlock);

                            this.Refine(group, nextPartition);

                            int[] permF = new int[vertexCount];
                            int[] invF = new int[vertexCount];
                            for (int i = 0; i < vertexCount; i++)
                            {
                                permF[i] = i;
                                invF[i] = i;
                            }

                            for (int j = 0; j <= firstNonDiscreteCell; j++)
                            {
                                int x = nextPartition.GetFirstInCell(j);
                                int i = invF[x];
                                int h = permF[j];
                                permF[j] = x;
                                permF[i] = h;
                                invF[h] = i;
                                invF[x] = j;
                            }
                            Permutation pPermF = new Permutation(permF);
                            group.ChangeBase(pPermF);
                            for (int j = 0; j < vertexCount; j++)
                            {
                                Permutation g = group[firstNonDiscreteCell, j];
                                if (g != null)
                                {
                                    blockCopy.Remove(g[vertexInBlock]);
                                }
                            }
                        }
                    }
                }
            }
        }

        /**
         * Check a permutation to see if it is better, equal, or worse than the
         * current best.
         *
         * @param perm the permutation to check
         * @return BETTER, EQUAL, or WORSE
         */
        private Result CompareRowwise(Permutation perm)
        {
            int m = perm.Count;
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = i + 1; j < m; j++)
                {
                    int x = GetConnectivity(best[i], best[j]);
                    int y = GetConnectivity(perm[i], perm[j]);
                    if (x > y) return Result.WORSE;
                    if (x < y) return Result.BETTER;
                }
            }
            return Result.EQUAL;
        }
    }
}
