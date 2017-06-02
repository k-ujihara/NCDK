/* Copyright (C) 2017  Gilleain Torrance <gilleain.torrance@gmail.com>
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module test-group 
    public class EquitablePartitionRefinerTest : CDKTestCase
    {
        public MockRefinable MakeExampleTable()
        {
            int[][] table = new int[4][];
            table[0] = new int[] { 1, 2 };
            table[1] = new int[] { 0, 3 };
            table[2] = new int[] { 0, 3 };
            table[3] = new int[] { 1, 2 };
            return new MockRefinable(table);
        }

        public class MockRefinable : Refinable
        {
            public int[][] connections;

            public MockRefinable(int[][] connections)
            {
                this.connections = connections;
            }

            public int GetVertexCount()
            {
                return connections.Length;
            }

            public int[] GetConnectedIndices(int vertexI)
            {
                return connections[vertexI];
            }

            public int GetConnectivity(int vertexI, int vertexJ)
            {
                foreach (int connected in connections[vertexI])
                {
                    if (connected == vertexJ)
                    {
                        return 1;
                    }
                }
                return 0;
            }

            public Partition GetInitialPartition()
            {
                return Partition.Unit(GetVertexCount());
            }

            public Invariant NeighboursInBlock(ISet<int> block, int vertexIndex)
            {
                int neighbours = 0;
                foreach (int connected in GetConnectedIndices(vertexIndex))
                {
                    if (block.Contains(connected))
                    {
                        neighbours++;
                    }
                }
                return new IntegerInvariant(neighbours);
            }

        }

        [TestMethod()]
        public void ConstructorTest()
        {
            EquitablePartitionRefiner refiner = new EquitablePartitionRefiner(MakeExampleTable());
            Assert.IsNotNull(refiner);
        }

        [TestMethod()]
        public void RefineTest()
        {
            EquitablePartitionRefiner refiner = new EquitablePartitionRefiner(MakeExampleTable());
            Partition coarser = Partition.FromString("[0|1,2,3]");
            Partition finer = refiner.Refine(coarser);
            Partition expected = Partition.FromString("[0|1,2|3]");
            Assert.AreEqual(expected, finer);
        }
    }
}
