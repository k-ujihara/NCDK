using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.FaulonSignatures
{
    [TestClass()]
    public class DAGTester
    {
        public void TestInvariants(
                int[] nodeInv, int[] vertexInv, Invariants invariants)
        {
            // TODO
            //        Assert.AssertArrayEquals(nodeInv, invariants.nodeInvariants);
            //        Assert.AssertArrayEquals(vertexInv, invariants.vertexInvariants);
        }

        //[TestMethod()]
        public void TestColoring()
        {
            // C12CC1C1
            DAG ring = new DAG(0, 4);
            DAG.Node root = ring.GetRoot();

            DAG.Node child1 = ring.MakeNodeInLayer(1, 1);
            ring.AddRelation(child1, root);

            DAG.Node child2 = ring.MakeNodeInLayer(2, 1);
            ring.AddRelation(child2, root);

            DAG.Node child3 = ring.MakeNodeInLayer(3, 1);
            ring.AddRelation(child3, root);

            DAG.Node child4 = ring.MakeNodeInLayer(3, 2);
            ring.AddRelation(child4, child1);
            ring.AddRelation(child4, child2);

            DAG.Node child5 = ring.MakeNodeInLayer(1, 2);
            ring.AddRelation(child5, child3);

            DAG.Node child6 = ring.MakeNodeInLayer(2, 2);
            ring.AddRelation(child6, child3);

            Console.Out.WriteLine(ring);
            string[] labels = new string[] { "C", "C", "C" };
            ring.InitializeWithStringLabels(labels);

            ring.UpdateNodeInvariants(DAG.Direction.UP);
            Console.Out.WriteLine(ring.CopyInvariants());

            ring.ComputeVertexInvariants();
            Console.Out.WriteLine(ring.CopyInvariants());

            ring.UpdateNodeInvariants(DAG.Direction.Down);
            Console.Out.WriteLine(ring.CopyInvariants());

            ring.ComputeVertexInvariants();
            Console.Out.WriteLine(ring.CopyInvariants());

            ring.UpdateVertexInvariants();
            Console.Out.WriteLine(ring.CopyInvariants());

            //        List<Integer> orbit = ring.CreateOrbit();
            //        Console.Out.WriteLine(orbit);
            //        
            //        ring.SetColor(orbit[0], 1);
            //        ring.UpdateVertexInvariants();
            //        Console.Out.WriteLine(ring.CopyInvariants());
            //
            //        orbit = ring.CreateOrbit();
            //        Console.Out.WriteLine(orbit);
            //        
            //        ring.SetColor(orbit[0], 2);
            //        ring.UpdateVertexInvariants();
            //        Console.Out.WriteLine(ring.CopyInvariants());
            //
            //        orbit = ring.CreateOrbit();
            //        Console.Out.WriteLine(orbit);
            //        ring.SetColor(orbit[0], 3);
            //        Console.Out.WriteLine(ring.CopyInvariants());
        }

        //[TestMethod()]
        public void TestColoringForUnlabelledThreeCycle()
        {
            DAG dag = new DAG(0, 3);
            DAG.Node root = dag.GetRoot();

            DAG.Node childA = dag.MakeNodeInLayer(1, 1);
            dag.AddRelation(childA, root);

            DAG.Node childB = dag.MakeNodeInLayer(2, 1);
            dag.AddRelation(childB, root);

            DAG.Node childC = dag.MakeNodeInLayer(2, 2);
            dag.AddRelation(childC, childA);

            DAG.Node childD = dag.MakeNodeInLayer(1, 2);
            dag.AddRelation(childD, childB);

            Console.Out.WriteLine(dag);
            dag.InitializeWithStringLabels(new string[] { "C", "C" });

            dag.UpdateVertexInvariants();
            Console.Out.WriteLine(dag.CopyInvariants());

            dag.SetColor(1, 1);
            dag.UpdateVertexInvariants();
            Console.Out.WriteLine(dag.CopyInvariants());
        }

        //[TestMethod()]
        public void TestSimpleUnlabelledDAG()
        {
            // Sets up a simple test case with a graph that looks like this:
            //             0 - Node (vertexIndex - label)
            //            / \
            //    1 - Node  2 - Node

            DAG simpleDAG = new DAG(0, 3);


            // First do all the initializations related to the nodes of the graph.
            // Create the nodes.
            DAG.Node parentNode = simpleDAG.GetRoot();
            DAG.Node childNode;

            // Add the first child.
            childNode = simpleDAG.MakeNodeInLayer(1, 1);
            simpleDAG.AddRelation(childNode, parentNode);

            // Add the second child.
            childNode = simpleDAG.MakeNodeInLayer(2, 1);
            simpleDAG.AddRelation(childNode, parentNode);

            // Initialize the all invariants.
            simpleDAG.InitializeWithStringLabels(new string[] { "Node", "Node", "Node" });

            // Canonize DAG by a simple Hopcroft-Tarjan sweep.
            int[] nodeInvariants = { 0, 0, 0 };
            int[] vertexInvariants = { 1, 2, 2 };
            TestInvariants(
                    nodeInvariants, vertexInvariants, simpleDAG.CopyInvariants());

            simpleDAG.UpdateNodeInvariants(DAG.Direction.Down);
            int[] nodeInvariantsAfterDown = { 1, 0, 0 };
            int[] vertexInvariantsAfterDown = { 1, 2, 2 };
            TestInvariants(nodeInvariantsAfterDown,
                           vertexInvariantsAfterDown,
                           simpleDAG.CopyInvariants());

            simpleDAG.ComputeVertexInvariants();
            int[] nodeInvariantsAfterComputeVertexInv = { 1, 0, 0 };
            int[] vertexInvariantsAfterComputeVertexInv = { 2, 1, 1 };
            TestInvariants(nodeInvariantsAfterComputeVertexInv,
                           vertexInvariantsAfterComputeVertexInv,
                           simpleDAG.CopyInvariants());

            simpleDAG.UpdateNodeInvariants(DAG.Direction.UP);
            int[] nodeInvariantsAfterUp = { 1, 1, 1 };
            int[] vertexInvariantsAfterUp = { 2, 1, 1 };
            TestInvariants(nodeInvariantsAfterUp,
                           vertexInvariantsAfterUp,
                           simpleDAG.CopyInvariants());

            string simpleDAGString = simpleDAG.ToString();
            string expected = "[0 Node ([], [1,2])]\n[1 Node ([0], []), " +
                              "2 Node ([0], [])]\n";
            Assert.AreEqual(expected, simpleDAGString);
        }

        //[TestMethod()]
        public void TestSimpleLabelledDAG()
        { // {
          // Sets up a simple test case with a graph that looks like this:
          //             0 - Node0 (vertexIndex - label)
          //            / \
          //    1 - Node2  2 - Node1

            DAG simpleDAG = new DAG(0, 3);


            // First do all the initializations related to the nodes of the graph.
            // Create the nodes.
            DAG.Node parentNode = simpleDAG.GetRoot();
            // Add the first child.
            DAG.Node childNode = simpleDAG.MakeNodeInLayer(1, 1);
            simpleDAG.AddRelation(childNode, parentNode);
            // Add the second child.
            childNode = simpleDAG.MakeNodeInLayer(2, 1);
            simpleDAG.AddRelation(childNode, parentNode);

            // Initialize the all invariants.
            simpleDAG.InitializeWithStringLabels(new string[] { "Node0", "Node2", "Node1" });

            //Console.Out.WriteLine(simpleDAG.ToString());

            // Canonize DAG by a simple Hopcroft-Tarjan sweep.
            int[] nodeInvariants = { 0, 0, 0 };
            int[] vertexInvariants = { 1, 3, 2 };
            TestInvariants(
                    nodeInvariants, vertexInvariants, simpleDAG.CopyInvariants());

            simpleDAG.UpdateNodeInvariants(DAG.Direction.Down);
            int[] nodeInvariantsAfterDown = { 1, 0, 0 };
            int[] vertexInvariantsAfterDown = { 1, 3, 2 };
            TestInvariants(nodeInvariantsAfterDown,
                           vertexInvariantsAfterDown,
                           simpleDAG.CopyInvariants());

            simpleDAG.ComputeVertexInvariants();
            int[] nodeInvariantsAfterComputeVertexInv = { 1, 0, 0 };
            int[] vertexInvariantsAfterComputeVertexInv = { 2, 1, 1 };
            TestInvariants(nodeInvariantsAfterComputeVertexInv,
                           vertexInvariantsAfterComputeVertexInv,
                           simpleDAG.CopyInvariants());

            simpleDAG.UpdateNodeInvariants(DAG.Direction.UP);
            int[] nodeInvariantsAfterUp = { 1, 1, 1 };
            int[] vertexInvariantsAfterUp = { 2, 1, 1 };
            TestInvariants(nodeInvariantsAfterUp,
                           vertexInvariantsAfterUp,
                           simpleDAG.CopyInvariants());
        }

        [TestMethod()]
        public void DagWithEdgeLabels()
        {
            DAG dag = new DAG(0, 4);
            DAG.Node root = dag.GetRoot();

            DAG.Node child = dag.MakeNodeInLayer(1, 1);
            dag.AddRelation(root, child);
            child.AddEdgeColor(0, 2);
            root.AddEdgeColor(1, 2);

            child = dag.MakeNodeInLayer(2, 1);
            dag.AddRelation(root, child);
            child.AddEdgeColor(0, 1);
            root.AddEdgeColor(2, 1);

            child = dag.MakeNodeInLayer(3, 1);
            dag.AddRelation(root, child);
            child.AddEdgeColor(0, 1);
            root.AddEdgeColor(3, 1);

            dag.InitializeWithStringLabels(new string[] { "C", "C", "C", "H" });
            dag.UpdateVertexInvariants();
            Console.Out.WriteLine(dag.CopyInvariants());
        }
    }
}
