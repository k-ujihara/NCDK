using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module test-group
	[TestClass()]
    public class AtomEquitablePartitionRefinerTest : CDKTestCase
    {
        public MockAtomRefiner MakeExampleTable()
        {
            int[][] table = new int[4][];
            table[0] = new int[] { 1, 2 };
            table[1] = new int[] { 0, 3 };
            table[2] = new int[] { 0, 3 };
            table[3] = new int[] { 1, 2 };
            return new MockAtomRefiner(table);
        }

        public class MockAtomRefiner : AtomDiscretePartitionRefiner
        {
            public int[][] connections;

            public MockAtomRefiner(int[][] connections)
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
            AtomEquitablePartitionRefiner refiner = new AtomEquitablePartitionRefiner(MakeExampleTable());
            Assert.IsNotNull(refiner);
        }

        [TestMethod()]
        public void GetVertexCountTest()
        {
            AtomEquitablePartitionRefiner refiner = new AtomEquitablePartitionRefiner(MakeExampleTable());
            Assert.AreEqual(4, refiner.GetVertexCount());
        }

        [TestMethod()]
        public void NeighboursInBlockTest()
        {
            AtomEquitablePartitionRefiner refiner = new AtomEquitablePartitionRefiner(MakeExampleTable());
            var block = new HashSet<int>();
            block.Add(1);
            block.Add(2);
            block.Add(3);
            Assert.AreEqual(2, refiner.NeighboursInBlock(block, 0));
        }

        [TestMethod()]
        public void RefineTest()
        {
            AtomEquitablePartitionRefiner refiner = new AtomEquitablePartitionRefiner(MakeExampleTable());
            Partition coarser = Partition.FromString("[0|1,2,3]");
            Partition finer = refiner.Refine(coarser);
            Partition expected = Partition.FromString("[0|1,2|3]");
            Assert.AreEqual(expected, finer);
        }
    }
}
