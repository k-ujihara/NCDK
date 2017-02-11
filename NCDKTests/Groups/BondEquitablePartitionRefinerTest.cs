using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module test-group
    public class BondEquitablePartitionRefinerTest : CDKTestCase
    {
        public MockBondRefiner MakeExampleTable()
        {
            int[][] table = new int[4][];
            table[0] = new int[] { 1, 2 };
            table[1] = new int[] { 0, 3 };
            table[2] = new int[] { 0, 3 };
            table[3] = new int[] { 1, 2 };
            return new MockBondRefiner(table);
        }

        public class MockBondRefiner : BondDiscretePartitionRefiner
        {
            public int[][] connections;

            public MockBondRefiner(int[][] connections)
            {
                this.connections = connections;
            }

            public override int GetVertexCount()
            {
                return connections.Length;
            }

            public override int[] GetConnectedIndices(int vertexI)
            {
                return connections[vertexI];
            }
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            BondEquitablePartitionRefiner refiner = new BondEquitablePartitionRefiner(MakeExampleTable());
            Assert.IsNotNull(refiner);
        }

        [TestMethod()]
        public void GetVertexCountTest()
        {
            BondEquitablePartitionRefiner refiner = new BondEquitablePartitionRefiner(MakeExampleTable());
            Assert.AreEqual(4, refiner.GetVertexCount());
        }

        [TestMethod()]
        public void NeighboursInBlockTest()
        {
            BondEquitablePartitionRefiner refiner = new BondEquitablePartitionRefiner(MakeExampleTable());
            var block = new HashSet<int>();
            block.Add(1);
            block.Add(2);
            block.Add(3);
            Assert.AreEqual(2, refiner.NeighboursInBlock(block, 0));
        }

        [TestMethod()]
        public void RefineTest()
        {
            BondEquitablePartitionRefiner refiner = new BondEquitablePartitionRefiner(MakeExampleTable());
            Partition coarser = Partition.FromString("[0|1,2,3]");
            Partition finer = refiner.Refine(coarser);
            Partition expected = Partition.FromString("[0|1,2|3]");
            Assert.AreEqual(expected, finer);
        }
    }
}
