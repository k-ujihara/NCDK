using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace NCDK.FaulonSignatures.Simple
{
    [TestClass()]
    public class SimpleQuotientGraphTest
    {
        //public void Draw(SimpleQuotientGraph quotientGraph)
        //{
        //    string directoryPath = "tmp4";
        //    List<string> signatureStrings =
        //        quotientGraph.GetVertexSignatureStrings();
        //    int w = 1200;
        //    int h = 400;
        //    TreeDrawer.MakeTreeImages(signatureStrings, directoryPath, w, h);
        //}

        public void Check3Regularity(SimpleGraph graph)
        {
            Console.Out.WriteLine(graph);
            for (int i = 0; i < graph.GetVertexCount(); i++)
            {
                Assert.AreEqual(3, graph.Degree(i), "failed for " + i);
            }
        }

        public void Check4Regularity(SimpleGraph graph)
        {
            Console.Out.WriteLine(graph);
            for (int i = 0; i < graph.GetVertexCount(); i++)
            {
                Assert.AreEqual(4, graph.Degree(i), "failed for " + i);
            }
        }

        public void CheckParameters(SimpleQuotientGraph qGraph,
                                    int expectedVertexCount,
                                    int expectedEdgeCount,
                                    int expectedLoopEdgeCount)
        {
            Console.Out.WriteLine(qGraph);
            Assert.AreEqual(expectedVertexCount, qGraph.GetVertexCount());
            Assert.AreEqual(expectedEdgeCount, qGraph.GetEdgeCount());
            Assert.AreEqual(expectedLoopEdgeCount, qGraph.NumberOfLoopEdges());

        }

        [TestMethod()]
        public void FourCubeTest()
        {
            SimpleGraph fourCube = SimpleGraphFactory.Make4Cube();
            Check4Regularity(fourCube);
            SimpleQuotientGraph qgraph = new SimpleQuotientGraph(fourCube);
            CheckParameters(qgraph, 1, 1, 1);
        }

        [TestMethod()]
        public void FourRegularExampleTest()
        {
            SimpleGraph fourRegular = SimpleGraphFactory.MakeFourRegularExample();
            Check4Regularity(fourRegular);
            SimpleQuotientGraph qgraph = new SimpleQuotientGraph(fourRegular);
            CheckParameters(qgraph, 1, 1, 1);
        }

        [TestMethod()]
        public void PetersensGraphTest()
        {
            SimpleGraph petersens = SimpleGraphFactory.MakePetersensGraph();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(petersens);
            CheckParameters(quotientGraph, 1, 1, 1);
        }

        [TestMethod()]
        public void PappusGraphTest()
        {
            SimpleGraph pappus = SimpleGraphFactory.MakePappusGraph();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(pappus);
            CheckParameters(quotientGraph, 1, 1, 1);
        }

        [TestMethod()]
        public void TriangularPrismTest()
        {
            SimpleGraph pentaprism = SimpleGraphFactory.MakePrism(3);
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(pentaprism);
            CheckParameters(quotientGraph, 1, 1, 1);
        }

        [TestMethod()]
        public void CubeTest()
        {
            SimpleGraph cube = SimpleGraphFactory.MakePrism(4);
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(cube);
            CheckParameters(quotientGraph, 1, 1, 1);
        }

        [TestMethod()]
        public void PentagonalPrismTest()
        {
            SimpleGraph pentaprism = SimpleGraphFactory.MakePrism(5);
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(pentaprism);
            CheckParameters(quotientGraph, 1, 1, 1);
        }

        [TestMethod()]
        public void TruncatedTetrahedronTest()
        {
            SimpleGraph truncatedTetrahedron =
                SimpleGraphFactory.MakeTruncatedTetrahedron();
            SimpleQuotientGraph quotientGraph =
                new SimpleQuotientGraph(truncatedTetrahedron);
            CheckParameters(quotientGraph, 1, 1, 1);
        }

        [TestMethod()]
        public void AdamantaneTest()
        {
            SimpleGraph adamantane = SimpleGraphFactory.MakeAdamantane();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(adamantane);
            CheckParameters(quotientGraph, 2, 1, 0);
        }

        [TestMethod()]
        public void TriangleSandwichTest()
        {
            SimpleGraph triangle = SimpleGraphFactory.MakeSandwich(3);
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(triangle);
            CheckParameters(quotientGraph, 2, 2, 1);
        }

        [TestMethod()]
        public void SquareSandwichTest()
        {
            SimpleGraph square = SimpleGraphFactory.MakeSandwich(4);
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(square);
            CheckParameters(quotientGraph, 2, 2, 1);
        }

        [TestMethod()]
        public void PentagonalSandwichTest()
        {
            SimpleGraph pentagon = SimpleGraphFactory.MakeSandwich(5);
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(pentagon);
            CheckParameters(quotientGraph, 2, 2, 1);
        }

        [TestMethod()]
        public void HexagonalSandwichTest()
        {
            SimpleGraph hexagon = SimpleGraphFactory.MakeSandwich(6);
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(hexagon);
            CheckParameters(quotientGraph, 2, 2, 1);
        }

        [TestMethod()]
        public void Symmetric1TwistaneTest()
        {
            SimpleGraph symmetric1Twistane = SimpleGraphFactory.MakeSymmetric1Twistane();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(symmetric1Twistane);
            CheckParameters(quotientGraph, 2, 2, 1);
        }

        [TestMethod()]
        public void Symmetric2TwistaneTest()
        {
            SimpleGraph symmetric2Twistane = SimpleGraphFactory.MakeSymmetric2Twistane();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(symmetric2Twistane);
            CheckParameters(quotientGraph, 2, 3, 2);
        }

        [TestMethod()]
        public void HerschelGraphTest()
        {
            SimpleGraph herschel = SimpleGraphFactory.MakeHerschelGraph();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(herschel);
            CheckParameters(quotientGraph, 3, 2, 0);
        }

        [TestMethod()]
        public void DiamantaneTest()
        {
            SimpleGraph diamantane = SimpleGraphFactory.MakeDiamantane();
            SimpleQuotientGraph qGraph = new SimpleQuotientGraph(diamantane);
            CheckParameters(qGraph, 3, 3, 1);
        }

        [TestMethod()]
        public void GrotschGraphTest()
        {
            SimpleGraph grotschGraph = SimpleGraphFactory.MakeGrotschGraph();
            SimpleQuotientGraph qgraph = new SimpleQuotientGraph(grotschGraph);
            CheckParameters(qgraph, 3, 3, 1);
        }

        [TestMethod()]
        public void QuadricyclaneTest()
        {
            SimpleGraph quadricyclane = SimpleGraphFactory.MakeQuadricyclane();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(quadricyclane);
            CheckParameters(quotientGraph, 3, 3, 1);
        }

        [TestMethod()]
        public void SpiroPentagonTest()
        {
            SimpleGraph spiroPentagons = SimpleGraphFactory.MakeSpiroPentagons();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(spiroPentagons);
            CheckParameters(quotientGraph, 3, 3, 1);
        }

        [TestMethod()]
        public void ThreeFourFiveTwistedGraphTest()
        {
            SimpleGraph threeFourFive =
                SimpleGraphFactory.MakeThreeFourFiveTwisted();
            SimpleQuotientGraph quotientGraph =
                new SimpleQuotientGraph(threeFourFive);
            CheckParameters(quotientGraph, 3, 3, 1);
        }

        [TestMethod()]
        public void TwistaneTest()
        {
            SimpleGraph twistane = SimpleGraphFactory.MakeTwistane();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(twistane);
            CheckParameters(quotientGraph, 3, 4, 2);
        }

        [TestMethod()]
        public void NapthaleneTest()
        {
            SimpleGraph napthalene = SimpleGraphFactory.MakeNapthalene();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(napthalene);
            CheckParameters(quotientGraph, 3, 4, 2);
        }

        [TestMethod()]
        public void TietzesGraphTest()
        {
            SimpleGraph tietzes = SimpleGraphFactory.MakeTietzesGraph();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(tietzes);
            CheckParameters(quotientGraph, 3, 4, 2);
        }

        [TestMethod()]
        public void CuneaneTest()
        {
            SimpleGraph cuneane = SimpleGraphFactory.MakeCuneane();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(cuneane);
            CheckParameters(quotientGraph, 3, 5, 3);
        }

        [TestMethod()]
        public void SquareQuotientGraphTest()
        {
            SimpleGraph squareQuotientGraph =
                SimpleGraphFactory.MakeSquareQuotientGraph();
            SimpleQuotientGraph quotientGraph =
                new SimpleQuotientGraph(squareQuotientGraph);
            CheckParameters(quotientGraph, 4, 4, 0);
        }

        [TestMethod()]
        public void DoubleBridgedPentagonTest()
        {
            SimpleGraph g = SimpleGraphFactory.MakeDoubleBridgedPentagon();
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(g);
            CheckParameters(quotientGraph, 4, 5, 1);
        }

        [TestMethod()]
        public void BowtieaneQuotientGraphTest()
        {
            SimpleGraph bowtieane = SimpleGraphFactory.MakeBowtieane();
            //        Check3Regularity(bowtieane);
            SimpleQuotientGraph quotientGraph = new SimpleQuotientGraph(bowtieane);
            //        Draw(quotientGraph);
            // TODO : FIXME
            CheckParameters(quotientGraph, 4, 5, 2);
        }

        [TestMethod()]
        public void Fullerene26Test()
        {
            SimpleGraph fullerene26 = SimpleGraphFactory.Make26Fullerene();
            Check3Regularity(fullerene26);
            SimpleQuotientGraph quotientGraph =
                new SimpleQuotientGraph(fullerene26);
            CheckParameters(quotientGraph, 4, 5, 2);
        }

        [TestMethod()]
        public void DiSpiroOctaneQuotientGraphTest()
        {
            SimpleGraph diSpiroOctane = SimpleGraphFactory.MakeDiSpiroOctane();
            SimpleQuotientGraph quotientGraph =
                new SimpleQuotientGraph(diSpiroOctane);
            CheckParameters(quotientGraph, 5, 6, 1);
        }

        [TestMethod()]
        public void TricycloPropaIndeneQuotientGraphTest()
        {
            SimpleGraph tricycloPropaIndene =
                SimpleGraphFactory.MakeTricycloPropaIndene();
            SimpleQuotientGraph quotientGraph =
                new SimpleQuotientGraph(tricycloPropaIndene);
            CheckParameters(quotientGraph, 6, 8, 2);
        }
    }
}
