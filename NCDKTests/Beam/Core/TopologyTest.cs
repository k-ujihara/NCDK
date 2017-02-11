using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using System;
using System.Collections.Generic;
using static NCDK.Beam.Configuration;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author>*/
    [TestClass()]
    public class TopologyTest
    {
        [TestMethod()]
        public void UnknownTest()
        {
            Assert.AreEqual(Configuration.Unknown, Topology.Unknown.Configuration);
        }

        [TestMethod()]
        [ExpectedException(typeof(NotSupportedException))]
        public void UnknownAtom()
        {
            int dummy = Topology.Unknown.Atom;
        }

        [TestMethod()]
        public void UnknownTransform()
        {
            Assert.AreSame(Topology.Unknown, Topology.Unknown.Transform(new int[0]));
        }

        [TestMethod()]
        public void UnknownOrderBy()
        {
            Assert.AreSame(Topology.Unknown, Topology.Unknown.OrderBy(new int[0]));
        }

        [TestMethod()]
        public void PermutationParity()
        {
            Assert.AreEqual(1,
                Topology.Parity(
                    new int[] { 0, 1, 2, 3 },
                    new int[] { 0, 1, 2, 3 }));   // even
            Assert.AreEqual(-1,
                Topology.Parity(
                    new int[] { 0, 1, 2, 3 },
                    new int[] { 0, 1, 3, 2 }));  // swap 2,3 = odd
            Assert.AreEqual(1,
                Topology.Parity(
                    new int[] { 0, 1, 2, 3 },
                    new int[] { 1, 0, 3, 2 }));   // swap 0,1 = even
            Assert.AreEqual(-1,
                Topology.Parity(
                    new int[] { 0, 1, 2, 3 },
                    new int[] { 2, 0, 3, 1 }));  // swap 0,3 = odd
        }

        [TestMethod()]
        public void SortTest()
        {
            int[] org = new int[] { 1, 2, 3, 4 };
            Assert.AreNotSame(org, Topology.Sort(org, new int[] { 0, 1, 2, 3, 4 }));
            Assert.IsTrue(Compares.AreEqual(
                new int[] { 1, 2, 3, 4 },
                Topology.Sort(org, new int[] { 0, 1, 2, 3, 4 })));
            Assert.IsTrue(Compares.AreEqual(
                new int[] { 2, 1, 3, 4 },
                Topology.Sort(org, new int[] { 0, 2, 1, 3, 4 })));
            // non-sequential
            Assert.IsTrue(Compares.AreEqual(
                new int[] { 2, 1, 4, 3 },
                Topology.Sort(org, new int[] { 0, 2, 1, 7, 4 })));
        }

        [TestMethod()]
        public void TetrahedralAtom()
        {
            Topology t1 = Topology.CreateTetrahedral(1, new int[] { 0, 2, 3, 4 }, TH1);
            Assert.AreEqual(1, t1.Atom);
        }

        [TestMethod()]
        public void TetrahedralOrderBy()
        {
            // test shows the first example of tetrahedral configuration from the
            // OpenSMILES specification

            // N=1, Br=2, O=3, C=4
            Topology t1 = Topology.CreateTetrahedral(0, new int[] { 1, 2, 3, 4 }, TH1);

            // N, Br, O, C
            Assert.AreEqual(TH2, t1.OrderBy(new int[] { 0, 1, 2, 4, 3 }).Configuration);
            // O, Br, C, N
            Assert.AreEqual(TH1, t1.OrderBy(new int[] { 0, 4, 2, 1, 3 }).Configuration);
            // C, Br, N, O
            Assert.AreEqual(TH1, t1.OrderBy(new int[] { 0, 3, 2, 4, 1 }).Configuration);
            // C, Br, O, N
            Assert.AreEqual(TH2, t1.OrderBy(new int[] { 0, 4, 2, 3, 1 }).Configuration);
            // Br, O, N, C
            Assert.AreEqual(TH1, t1.OrderBy(new int[] { 0, 3, 1, 2, 4 }).Configuration);
            // Br, C, O, N
            Assert.AreEqual(TH1, t1.OrderBy(new int[] { 0, 4, 1, 3, 2 }).Configuration);
            // Br, N, C, O
            Assert.AreEqual(TH1, t1.OrderBy(new int[] { 0, 2, 1, 4, 3 }).Configuration);
            // Br, N, O, C
            Assert.AreEqual(TH2, t1.OrderBy(new int[] { 0, 2, 1, 3, 4 }).Configuration);
        }

        [TestMethod()]
        public void implicitToExplicit_tetrahedral()
        {
            // N[C@]([H])(C)C(=O)O
            Graph g = new Graph(7);
            g.AddAtom(AtomImpl.AliphaticSubset.Nitrogen);
            g.AddAtom(new AtomImpl.BracketAtom(Element.Carbon, 0, 0));
            g.AddAtom(AtomImpl.EXPLICIT_HYDROGEN);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Oxygen);
            g.AddAtom(AtomImpl.AliphaticSubset.Oxygen);

            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(1, 3, Bond.Implicit));
            g.AddEdge(new Edge(1, 4, Bond.Implicit));
            g.AddEdge(new Edge(4, 5, Bond.Double));
            g.AddEdge(new Edge(4, 6, Bond.Implicit));

            Assert.AreEqual(Topology.ToExplicit(g, 1, AntiClockwise), TH1);
            Assert.AreEqual(Topology.ToExplicit(g, 1, Clockwise), TH2);
        }

        [TestMethod()]
        public void implicitToExplicit_tetrahedralImplicitH()
        {

            // N[C@]([H])(C)C(=O)O
            Graph g = new Graph(7);
            g.AddAtom(AtomImpl.AliphaticSubset.Nitrogen);
            g.AddAtom(new AtomImpl.BracketAtom(Element.Carbon, 1, 0));
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Oxygen);
            g.AddAtom(AtomImpl.AliphaticSubset.Oxygen);

            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(1, 3, Bond.Implicit));
            g.AddEdge(new Edge(3, 4, Bond.Double));
            g.AddEdge(new Edge(3, 5, Bond.Implicit));

            Assert.AreEqual(Topology.ToExplicit(g, 1, AntiClockwise), TH1);
            Assert.AreEqual(Topology.ToExplicit(g, 1, Clockwise), TH2);
        }

        [TestMethod()]
        public void implicitToExplicit_sulfoxide()
        {
            // C[S@](CC)=O
            Graph g = new Graph(5);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(new AtomImpl.BracketAtom(Element.Sulfur, 0, 0));
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Oxygen);

            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(1, 4, Bond.Double));
            g.AddEdge(new Edge(2, 3, Bond.Implicit));

            Assert.AreEqual(Topology.ToExplicit(g, 1, AntiClockwise), TH1);
            Assert.AreEqual(Topology.ToExplicit(g, 1, Clockwise), TH2);
        }

        // CCCCC[P@@]1CCC[C@H]1[C@@H]2CCCP2CCCCC CID 59836513
        [TestMethod()]
        public void implicitToExplicit_phosphorus()
        {
            // C[P@@](CC)O
            Graph g = new Graph(5);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(new AtomImpl.BracketAtom(Element.Phosphorus, 0, 0));
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Oxygen);

            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(1, 4, Bond.Implicit));
            g.AddEdge(new Edge(2, 3, Bond.Implicit));

            Assert.AreEqual(TH1, Topology.ToExplicit(g, 1, AntiClockwise));
            Assert.AreEqual(TH2, Topology.ToExplicit(g, 1, Clockwise));
        }

    [TestMethod()]
        public void implicitToExplicit_allene()
        {

            // OC(Cl)=[C@]=C(C)F
            Graph g = new Graph(7);
            g.AddAtom(AtomImpl.AliphaticSubset.Oxygen);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Chlorine);
            g.AddAtom(new AtomImpl.BracketAtom(Element.Carbon, 0, 0));
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Fluorine);

            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(1, 3, Bond.Double));
            g.AddEdge(new Edge(3, 4, Bond.Double));
            g.AddEdge(new Edge(4, 5, Bond.Implicit));
            g.AddEdge(new Edge(5, 6, Bond.Implicit));

            Assert.AreEqual(Topology.ToExplicit(g, 3, AntiClockwise), AL1);
            Assert.AreEqual(Topology.ToExplicit(g, 3, Clockwise), AL2);
        }

        [TestMethod()]
        public void implicitToExplicit_trigonalBipyramidal()
        {
            // O=C[As@](F)(Cl)(Br)S
            Graph g = new Graph(7);
            g.AddAtom(AtomImpl.AliphaticSubset.Oxygen);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(new AtomImpl.BracketAtom(Element.Arsenic, 0, 0));
            g.AddAtom(AtomImpl.AliphaticSubset.Fluorine);
            g.AddAtom(AtomImpl.AliphaticSubset.Chlorine);
            g.AddAtom(AtomImpl.AliphaticSubset.Bromine);
            g.AddAtom(AtomImpl.AliphaticSubset.Sulfur);

            g.AddEdge(new Edge(0, 1, Bond.Double));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(2, 3, Bond.Implicit));
            g.AddEdge(new Edge(2, 4, Bond.Implicit));
            g.AddEdge(new Edge(2, 5, Bond.Implicit));
            g.AddEdge(new Edge(2, 6, Bond.Implicit));

            Assert.AreEqual(Configuration.TB1, Topology.ToExplicit(g, 2, AntiClockwise));
            Assert.AreEqual(Configuration.TB2, Topology.ToExplicit(g, 2, Clockwise));
        }

        [TestMethod()]
        public void implicitToExplicit_octahedral()
        {
            // S[Co@@](F)(Cl)(Br)(I)C=O
            Graph g = new Graph(8);
            g.AddAtom(AtomImpl.AliphaticSubset.Sulfur);
            g.AddAtom(new AtomImpl.BracketAtom(Element.Cobalt, 0, 0));
            g.AddAtom(AtomImpl.AliphaticSubset.Fluorine);
            g.AddAtom(AtomImpl.AliphaticSubset.Chlorine);
            g.AddAtom(AtomImpl.AliphaticSubset.Bromine);
            g.AddAtom(AtomImpl.AliphaticSubset.Iodine);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Oxygen);

            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(1, 3, Bond.Implicit));
            g.AddEdge(new Edge(1, 4, Bond.Implicit));
            g.AddEdge(new Edge(1, 5, Bond.Implicit));
            g.AddEdge(new Edge(1, 6, Bond.Implicit));
            g.AddEdge(new Edge(6, 7, Bond.Double));

            Assert.AreEqual(Configuration.OH1, Topology.ToExplicit(g, 1, AntiClockwise));
            Assert.AreEqual(Configuration.OH2, Topology.ToExplicit(g, 1, Clockwise));
        }

        [TestMethod()]
        public void implicitToExplicit_Unknown()
        {
            Assert.AreEqual(Configuration.Unknown, Topology.ToExplicit(new Graph(0), 0, Configuration.Unknown));
        }

        [TestMethod()]
        public void implicitToExplicit_th1_th2()
        {
            Assert.AreEqual(Configuration.TH1, Topology.ToExplicit(new Graph(0), 0, Configuration.TH1));
            Assert.AreEqual(Configuration.TH2, Topology.ToExplicit(new Graph(0), 0, Configuration.TH2));
        }

        [TestMethod()]
        public void implicitToExplicit_al1_al2()
        {
            Assert.AreEqual(Configuration.AL1, Topology.ToExplicit(new Graph(0), 0, Configuration.AL1));
            Assert.AreEqual(Configuration.AL2, Topology.ToExplicit(new Graph(0), 0, Configuration.AL2));
        }

        [TestMethod()]
        public void implicitToExplicit_tbs()
        {
            foreach (var c in Configuration.Values)
            {
                if (c.Type == Configuration.Types.TrigonalBipyramidal)
                    Assert.AreEqual(c, Topology.ToExplicit(new Graph(0), 0, c));
            }
        }

        [TestMethod()]
        public void implicitToExplicit_ohs()
        {
            foreach (var c in Configuration.Values)
            {
                if (c.Type == Configuration.Types.Octahedral)
                    Assert.AreEqual(c, Topology.ToExplicit(new Graph(0), 0, c));
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Create_AntiClockwise()
        {
            Topology.Create(0, new int[0], new List<Edge>(), Configuration.AntiClockwise);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Create_Clockwise()
        {
            Topology.Create(0, new int[0], new List<Edge>(), Configuration.Clockwise);
        }

        [TestMethod()]
        public void Create_tb()
        {
            Assert.AreEqual(Topology.Unknown, Topology.Create(0, new int[0], new List<Edge>(), Configuration.TB1));
        }

        [TestMethod()]
        public void Create_sp()
        {
            Assert.AreEqual(Topology.Unknown, Topology.Create(0, new int[0], new List<Edge>(), Configuration.SP1));
        }

        [TestMethod()]
        public void Create_oh()
        {
            Assert.AreEqual(Topology.Unknown, Topology.Create(0, new int[0], new List<Edge>(), Configuration.OH1));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Create_al()
        {
            Assert.AreEqual(Topology.Unknown, Topology.Create(0, new int[0], new List<Edge>(), Configuration.AL1));
        }

        [TestMethod()]
        public void Create_th()
        {
            int[] vs = new int[] { 1, 2, 3, 4 };
            List<Edge> es = new List<Edge>(new[] {
                new Edge(0, 1, Bond.Implicit),
                new Edge(0, 2, Bond.Implicit),
                new Edge(0, 3, Bond.Implicit),
                new Edge(0, 4, Bond.Implicit), });
            Topology t = Topology.Create(0, vs, es, TH1);
            Assert.AreEqual(t.Configuration, TH1);
            Assert.AreEqual(t.Atom, 0);
        }
    }
}

