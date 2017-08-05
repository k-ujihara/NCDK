using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.FaulonSignatures.Simple;
using System;

namespace NCDK.FaulonSignatures
{
    [TestClass()]
    public class HeightTest
    {

        public SimpleGraph MakeTorus(int width, int height)
        {
            SimpleGraph graph = new SimpleGraph();
            // make a grid of width * height
            for (int i = 0; i < width - 1; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int x = (width * j) + i;
                    int y = x + 1;
                    graph.MakeEdge(x, y);
                    if (j < height - 1)
                    {
                        int z = width * (j + 1) + i;
                        graph.MakeEdge(x, z);
                    }
                }
            }
            // finish off the last column
            for (int i = 0; i < height - 1; i++)
            {
                int x = ((i + 1) * width) - 1;
                int y = ((i + 2) * width) - 1;
                graph.MakeEdge(x, y);
            }

            int size = width * height;

            // connect the top edge to the bottom
            for (int i = 0; i < width; i++)
            {
                graph.MakeEdge(i, size - width + i);
            }

            // connect the left edge to the right
            for (int j = 0; j < height; j++)
            {
                int x = width * j;
                int y = (x + width) - 1;
                graph.MakeEdge(x, y);
            }

            return graph;
        }

        public void RegularGraphTest(SimpleGraph graph, int diameter)
        {
            for (int h = 1; h <= diameter; h++)
            {
                SimpleVertexSignature sig0 =
                    new SimpleVertexSignature(0, h, graph);
                string zeroCanonical = sig0.ToCanonicalString();
                Console.Out.WriteLine(h + "\t" + zeroCanonical);
                for (int i = 1; i < graph.GetVertexCount(); i++)
                {
                    SimpleVertexSignature sig =
                        new SimpleVertexSignature(i, h, graph);
                    //                Assert.AreEqual(zeroCanonical, sig.ToCanonicalString());
                    string canString = sig.ToCanonicalString();
                    if (zeroCanonical.Equals(canString))
                    {
                        //                    Console.Out.WriteLine("EQU");
                    }
                    else
                    {
                        Console.Out.WriteLine("NEQ "
                                + h + "\t" + i + "\t"
                                + zeroCanonical + " " + canString);
                    }
                    Assert.AreEqual(zeroCanonical, canString);
                }
            }
        }

        [TestMethod()]
        public void TorusTest()
        {
            int width = 6;
            int height = 6;

            SimpleGraph torus = MakeTorus(width, height);
            Console.Out.WriteLine(torus);
            int diameter = Math.Min(width, height);
            RegularGraphTest(torus, diameter);
        }

        public SimpleGraph MakeCompleteGraph(int n)
        {
            SimpleGraph g = new SimpleGraph();
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    g.MakeEdge(i, j);
                }
            }
            return g;
        }

        [TestMethod()]
        public void CompleteGraphTest()
        {
            int n = 7;
            SimpleGraph kN = MakeCompleteGraph(n);
            int expectedEdgeCount = (n * (n - 1)) / 2;
            Assert.AreEqual(expectedEdgeCount, kN.edges.Count);
            RegularGraphTest(kN, n - 1);
        }

        public static void Main(string[] args)
        {
            new HeightTest().TorusTest();
        }
    }
}
