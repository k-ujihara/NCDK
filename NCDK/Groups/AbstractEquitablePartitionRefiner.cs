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
using System.Collections.Generic;

namespace NCDK.Groups
{
    /**
     * Refines a 'coarse' partition (with more blocks) to a 'finer' partition that
     * is equitable.
     *
     * Closely follows algorithm 7.5 in CAGES {@cdk.cite Kreher98}. The basic idea is that the refiner
     * maintains a queue of blocks to refine, starting with all the initial blocks
     * in the partition to refine. These blocks are popped off the queue, and
     *
     * @author maclean
     * @cdk.module group
     */
    public abstract class AbstractEquitablePartitionRefiner
    {

        /**
         * A forward split order tends to favor partitions where the cells are
         * refined from lowest to highest. A reverse split order is, of course, the
         * opposite.
         *
         */
        public enum SplitOrder
        {
            FORWARD, REVERSE
        };

        /**
         * The bias in splitting cells when refining
         */
        private SplitOrder splitOrder = SplitOrder.FORWARD;

        /**
         * The block of the partition that is being refined
         */
        private int currentBlockIndex;

        /**
         * The blocks to be refined, or at least considered for refinement
         */
        private Queue<ISet<int>> blocksToRefine;

        /**
         * Gets from the graph the number of vertices. Abstract to allow different
         * graph classes to be used (eg: Graph or IAtomContainer, etc).
         *
         * @return the number of vertices
         */
        public abstract int GetVertexCount();

        /**
         * Find |a &cap; b| - that is, the size of the intersection between a and b.
         *
         * @param block a set of numbers
         * @param vertexIndex the element to compare
         * @return the size of the intersection
         */
        public abstract int NeighboursInBlock(ISet<int> block, int vertexIndex);

        /**
         * Set the preference for splitting cells.
         *
         * @param splitOrder either FORWARD or REVERSE
         */
        public void SetSplitOrder(SplitOrder splitOrder)
        {
            this.splitOrder = splitOrder;
        }

        /**
         * Refines the coarse partition <code>a</code> into a finer one.
         *
         * @param coarser the partition to refine
         * @return a finer partition
         */
        public Partition Refine(Partition coarser)
        {
            Partition finer = new Partition(coarser);

            // start the queue with the blocks of a in reverse order
            blocksToRefine = new Queue<ISet<int>>();
            for (int i = 0; i < finer.Count; i++)
            {
                blocksToRefine.Enqueue(finer.CopyBlock(i));
            }

            int numberOfVertices = GetVertexCount();
            while (blocksToRefine.Count != 0)
            {
                var t = blocksToRefine.Dequeue();
                currentBlockIndex = 0;
                while (currentBlockIndex < finer.Count && finer.Count < numberOfVertices)
                {
                    if (!finer.IsDiscreteCell(currentBlockIndex))
                    {

                        // get the neighbor invariants for this block
                        IDictionary<int, SortedSet<int>> invariants = GetInvariants(finer, t);

                        // split the block on the basis of these invariants
                        Split(invariants, finer);
                    }
                    currentBlockIndex++;
                }

                // the partition is discrete
                if (finer.Count == numberOfVertices)
                {
                    return finer;
                }
            }
            return finer;
        }

        /**
         * Gets the neighbor invariants for the block j as a map of
         * |N<sub>g</sub>(v) &cap; T| to elements of the block j. That is, the
         * size of the intersection between the set of neighbors of element v in
         * the graph and the target block T.
         *
         * @param partition the current partition
         * @param targetBlock the current target block of the partition
         * @return a map of set intersection sizes to elements
         */
        private IDictionary<int, SortedSet<int>> GetInvariants(Partition partition, ISet<int> targetBlock)
        {
            IDictionary<int, SortedSet<int>> setList = new Dictionary<int, SortedSet<int>>();
            foreach (var u in partition.GetCell(currentBlockIndex))
            {
                int h = NeighboursInBlock(targetBlock, u);
                if (setList.ContainsKey(h))
                {
                    setList[h].Add(u);
                }
                else
                {
                    SortedSet<int> set = new SortedSet<int>();
                    set.Add(u);
                    setList[h] = set;
                }
            }
            return setList;
        }

        /**
         * Split the current block using the invariants calculated in getInvariants.
         *
         * @param invariants a map of neighbor counts to elements
         * @param partition the partition that is being refined
         */
        private void Split(IDictionary<int, SortedSet<int>> invariants, Partition partition)
        {
            int nonEmptyInvariants = invariants.Keys.Count;
            if (nonEmptyInvariants > 1)
            {
                List<int> invariantKeys = new List<int>();
                invariantKeys.AddRange(invariants.Keys);
                partition.RemoveCell(currentBlockIndex);
                int k = currentBlockIndex;
                if (splitOrder == SplitOrder.REVERSE)
                {
                    invariantKeys.Sort();
                }
                else
                {
                    invariantKeys.Sort();
                    invariantKeys.Reverse();
                }
                foreach (var h in invariantKeys)
                {
                    SortedSet<int> setH = invariants[h];
                    partition.InsertCell(k, setH);
                    blocksToRefine.Enqueue(setH);
                    k++;

                }
                // skip over the newly added blocks
                currentBlockIndex += nonEmptyInvariants - 1;
            }
        }
    }
}
