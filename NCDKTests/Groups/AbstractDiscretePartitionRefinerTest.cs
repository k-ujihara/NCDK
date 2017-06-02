using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module test-group
    [TestClass()]
    public class AbstractDiscretePartitionRefinerTest : CDKTestCase
    {
        public class Graph
        {
            public int vertexCount;
            public int[][] connectionTable;

            public Graph(int vertexCount)
            {
                this.vertexCount = vertexCount;
            }
        }

        public class MockRefiner : AbstractDiscretePartitionRefiner
        {
            public Graph graph;

            public MockRefiner(Graph graph)
                : base()
            {
                this.graph = graph;
            }

            protected internal override int GetVertexCount()
            {
                return graph.vertexCount;
            }

            protected internal override int GetConnectivity(int vertexI, int vertexJ)
            {
                return graph.connectionTable[vertexI][vertexJ];
            }
        }

        public class MockEqRefiner : AbstractEquitablePartitionRefiner
        {
            public Graph graph;

            public MockEqRefiner(Graph graph)
            {
                this.graph = graph;
            }

            public override int GetVertexCount()
            {
                return graph.vertexCount;
            }

            public override int NeighboursInBlock(ISet<int> block, int vertexIndex)
            {
                //            Console.Out.WriteLine("calling NIB for " + block + " " + vertexIndex);
                int neighbours = 0;
                int n = GetVertexCount();
                for (int i = 0; i < n; i++)
                {
                    if (graph.connectionTable[vertexIndex][i] == 1 && block.Contains(i))
                    {
                        neighbours++;
                    }
                }
                //            Console.Out.WriteLine(neighbours + " neighbours");
                return neighbours;
            }
        }

        private class GraphRefinable : Refinable
        {
            private readonly Graph graph;

            public GraphRefinable(Graph graph)
            {
                this.graph = graph;
            }

            public int GetVertexCount()
            {
                return graph.vertexCount;
            }

            public int GetConnectivity(int vertexI, int vertexJ)
            {
                return graph.connectionTable[vertexI][vertexJ];
            }

            private int[] GetConnectedIndices(int vertexIndex)
            {
                ISet<int> connectedSet = new HashSet<int>();
                for (int index = 0; index < graph.connectionTable.Length; index++)
                {
                    if (graph.connectionTable[vertexIndex][index] == 1)
                    {
                        connectedSet.Add(index);
                    }
                }
                int[] connections = new int[connectedSet.Count];
                {
                    int index = 0;
                    foreach (int connected in connectedSet)
                    {
                        connections[index] = connected;
                        index++;
                    }
                }
                return connections;
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
        public void EmptyConstructor()
        {
            MockRefiner refiner = new MockRefiner(null);
            Assert.IsNotNull(refiner);
        }

        [TestMethod()]
        public void GetVertexCountTest()
        {
            int n = 10;
            Graph g = new Graph(n);
            MockRefiner refiner = new MockRefiner(g);
            Assert.AreEqual(g.vertexCount, refiner.GetVertexCount());
        }

        [TestMethod()]
        public void GetConnectivityTest()
        {
            int n = 3;
            Graph g = new Graph(n);
            g.connectionTable = new int[][] { new[] { 0, 1, 0 }, new[] { 1, 0, 1 }, new[] { 0, 1, 0 } };
            MockRefiner refiner = new MockRefiner(g);
            Assert.AreEqual(1, refiner.GetConnectivity(0, 1));
        }

        private void Setup(MockRefiner refiner, PermutationGroup group, Graph g)
        {
            refiner.Setup(group, new EquitablePartitionRefiner(new GraphRefinable(g)));
        }

        [TestMethod()]
        public void SetupTest()
        {
            int n = 5;
            PermutationGroup group = new PermutationGroup(n);
            Graph g = new Graph(n);
            MockRefiner refiner = new MockRefiner(g);
            Setup(refiner, group, g);
            Assert.AreEqual(group, refiner.GetAutomorphismGroup());
        }

        [TestMethod()]
        public void FirstIsIdentityTest()
        {
            int n = 3;
            Graph g = new Graph(n);
            g.connectionTable = new int[][] { new[] { 0, 1, 1 }, new[] { 1, 0, 0 }, new[] { 1, 0, 0 } };
            PermutationGroup group = new PermutationGroup(n);
            MockRefiner refiner = new MockRefiner(g);
            Setup(refiner, group, g);
            refiner.Refine(Partition.Unit(n));
            Assert.IsTrue(refiner.FirstIsIdentity());
        }

        [TestMethod()]
        public void GetAutomorphismPartitionTest()
        {
            int n = 3;
            Graph g = new Graph(n);
            g.connectionTable = new int[][] { new[] { 0, 1, 1 }, new[] { 1, 0, 0 }, new[] { 1, 0, 0 } };
            PermutationGroup group = new PermutationGroup(n);
            MockRefiner refiner = new MockRefiner(g);
            Setup(refiner, group, g);
            refiner.Refine(Partition.Unit(n));
            Partition autPartition = refiner.GetAutomorphismPartition();
            Partition expected = Partition.FromString("0|1,2");
            Assert.AreEqual(expected, autPartition);
        }

        [TestMethod()]
        public void GetFirstHalfMatrixStringTest()
        {
            int n = 3;
            Graph g = new Graph(n);
            g.connectionTable = new int[][] { new[] { 0, 0, 1 }, new[] { 0, 0, 1 }, new[] { 1, 1, 0 } };
            PermutationGroup group = new PermutationGroup(n);
            MockRefiner refiner = new MockRefiner(g);
            Setup(refiner, group, g);
            refiner.Refine(Partition.Unit(n));
            string hms = refiner.GetFirstHalfMatrixString();
            string expected = "110";
            Assert.AreEqual(expected, hms);
        }

        [TestMethod()]
        public void GetGroupTest()
        {
            int n = 3;
            Graph g = new Graph(n);
            g.connectionTable = new int[][] { new[] { 0, 1, 0 }, new[] { 1, 0, 1 }, new[] { 0, 1, 0 } };
            PermutationGroup group = new PermutationGroup(n);
            MockRefiner refiner = new MockRefiner(g);
            Setup(refiner, group, g);
            Assert.IsNotNull(refiner.GetAutomorphismGroup());
        }

        [TestMethod()]
        public void GetBestTest()
        {
            int n = 3;
            Graph g = new Graph(n);
            g.connectionTable = new int[][] { new[] { 0, 1, 0 }, new[] { 1, 0, 1 }, new[] { 0, 1, 0 } };
            PermutationGroup group = new PermutationGroup(n);
            MockRefiner refiner = new MockRefiner(g);
            Setup(refiner, group, g);
            refiner.Refine(Partition.Unit(n));
            Permutation best = refiner.GetBest();
            Permutation expected = new Permutation(1, 0, 2);
            Assert.AreEqual(expected, best);
        }

        [TestMethod()]
        public void GetFirstTest()
        {
            int n = 3;
            Graph g = new Graph(n);
            g.connectionTable = new int[][] { new[] { 0, 1, 0 }, new[] { 1, 0, 1 }, new[] { 0, 1, 0 } };
            PermutationGroup group = new PermutationGroup(n);
            MockRefiner refiner = new MockRefiner(g);
            Setup(refiner, group, g);
            refiner.Refine(Partition.Unit(n));
            Permutation first = refiner.GetFirst();
            Permutation expected = new Permutation(1, 0, 2);
            Assert.AreEqual(expected, first);
        }

        [TestMethod()]
        public void IsCanonical_TrueTest()
        {
            int n = 3;
            Graph g = new Graph(n);
            g.connectionTable = new int[][] { new[] { 0, 1, 1 }, new[] { 1, 0, 0 }, new[] { 1, 0, 0 } };
            PermutationGroup group = new PermutationGroup(n);
            MockRefiner refiner = new MockRefiner(g);
            Setup(refiner, group, g);
            refiner.Refine(Partition.Unit(n));
            Assert.IsTrue(refiner.IsCanonical());
        }

        [TestMethod()]
        public void IsCanonical_FalseTest()
        {
            int n = 3;
            Graph g = new Graph(n);
            g.connectionTable = new int[][] { new[] { 0, 1, 0 }, new[] { 1, 0, 1 }, new[] { 0, 1, 0 } };
            PermutationGroup group = new PermutationGroup(n);
            MockRefiner refiner = new MockRefiner(g);
            Setup(refiner, group, g);
            refiner.Refine(Partition.Unit(n));
            Assert.IsFalse(refiner.IsCanonical());
        }

        [TestMethod()]
        public void RefineTest()
        {
            int n = 3;
            Graph g = new Graph(n);
            g.connectionTable = new int[][] { new[] { 0, 1, 1 }, new[] { 1, 0, 0 }, new[] { 1, 0, 0 } };
            PermutationGroup group = new PermutationGroup(n);
            MockRefiner refiner = new MockRefiner(g);
            Setup(refiner, group, g);
            refiner.Refine(Partition.Unit(n));
            Assert.IsNotNull(refiner);
        }
    }
}

