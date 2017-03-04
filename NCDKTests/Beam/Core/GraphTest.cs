/*
 * Copyright (c) 2013, European Bioinformatics Institute (EMBL-EBI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies,
 * either expressed or implied, of the FreeBSD Project.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;
using NCDK.Common.Base;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author>*/
    [TestClass()]
    public class GraphTest
    {
        [TestMethod()]
        public void AddAtoms()
        {
            Graph g = new Graph(5);
            Assert.AreEqual(g.AddAtom(new Mock<Atom>().Object), 0);
            Assert.AreEqual(g.AddAtom(new Mock<Atom>().Object), 1);
            Assert.AreEqual(g.AddAtom(new Mock<Atom>().Object), 2);
            Assert.AreEqual(g.AddAtom(new Mock<Atom>().Object), 3);
            Assert.AreEqual(g.AddAtom(new Mock<Atom>().Object), 4);
        }

        [TestMethod()]
        public void AddAtomsResize()
        {
            Graph g = new Graph(2);
            Assert.AreEqual(g.AddAtom(new Mock<Atom>().Object), 0);
            Assert.AreEqual(g.AddAtom(new Mock<Atom>().Object), 1);
            Assert.AreEqual(g.AddAtom(new Mock<Atom>().Object), 2);
            Assert.AreEqual(g.AddAtom(new Mock<Atom>().Object), 3);
            Assert.AreEqual(g.AddAtom(new Mock<Atom>().Object), 4);
        }

        [TestMethod()]
        public void AtomAccess()
        {
            Atom[] atoms = new Atom[]{
                new Mock<Atom>().Object,
                new Mock<Atom>().Object,
                new Mock<Atom>().Object,
                new Mock<Atom>().Object,
            };
            Graph g = new Graph(5);
            foreach (var a in atoms)
                g.AddAtom(a);
            Assert.AreEqual(g.GetAtom(0), atoms[0]);
            Assert.AreEqual(g.GetAtom(1), atoms[1]);
            Assert.AreEqual(g.GetAtom(2), atoms[2]);
            Assert.AreEqual(g.GetAtom(3), atoms[3]);
        }

        [TestMethod()]
        public void TestOrder()
        {
            Graph g = new Graph(5);
            Assert.AreEqual(g.Order, 0);
            g.AddAtom(new Mock<Atom>().Object);
            Assert.AreEqual(g.Order, 1);
            g.AddAtom(new Mock<Atom>().Object);
            Assert.AreEqual(g.Order, 2);
            g.AddAtom(new Mock<Atom>().Object);
            Assert.AreEqual(g.Order, 3);
            g.AddAtom(new Mock<Atom>().Object);
            Assert.AreEqual(g.Order, 4);
            g.AddAtom(new Mock<Atom>().Object);
            Assert.AreEqual(g.Order, 5);
        }

        [TestMethod()]
        public void TestSize()
        {
            Graph g = new Graph(5);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);

            Edge e1 = new Edge(0, 1, Bond.Implicit);
            Edge e2 = new Edge(0, 1, Bond.Implicit);

            Assert.AreEqual(g.Size, 0);
            g.AddEdge(e1);
            Assert.AreEqual(g.Size, 1);
            g.AddEdge(e2);
            Assert.AreEqual(g.Size, 2);
        }

        [TestMethod()]
        public void TestEdges()
        {
            Graph g = new Graph(5);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            Assert.AreEqual(g.GetEdges(0).Count, 1);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { new Edge(0, 1, Bond.Implicit) },
                g.GetEdges(0)));
            Assert.AreEqual(g.GetEdges(1).Count, 2);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                    new Edge(0, 1, Bond.Implicit),
                    new Edge(1, 2, Bond.Implicit) },
                g.GetEdges(1)));    // fixed Beam's bug
        }

        [TestMethod()]
        public void TestEdgesResize()
        {
            Graph g = new Graph(2);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            Assert.AreEqual(g.GetEdges(0).Count, 1);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { new Edge(0, 1, Bond.Implicit) },
                g.GetEdges(0)));
            Assert.AreEqual(g.GetEdges(1).Count, 2);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                    new Edge(0, 1, Bond.Implicit),
                    new Edge(1, 2, Bond.Implicit) },
                g.GetEdges(1)));    // fixed Beam's bug
        }

        [TestMethod()]
        public void TestEdgesIterable()
        {
            Graph g = new Graph(2);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));

            IEnumerable<Edge> es = g.Edges;
            IEnumerator<Edge> it = es.GetEnumerator();
            Assert.IsTrue(it.MoveNext());
            Assert.AreEqual(it.Current, new Edge(0, 1, Bond.Implicit));
            Assert.IsTrue(it.MoveNext());
            Assert.AreEqual(it.Current, new Edge(1, 2, Bond.Implicit));
            Assert.IsFalse(it.MoveNext());
        }

        [TestMethod()]
        public void TestDegree()
        {
            Graph g = new Graph(5);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            Assert.AreEqual(g.Degree(0), 1);
            Assert.AreEqual(g.Degree(1), 2);
        }

        [TestMethod()]
        public void Adjacent()
        {
            Graph g = new Graph(5);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            Assert.IsTrue(g.Adjacent(0, 1));
            Assert.IsTrue(g.Adjacent(1, 2));
            Assert.IsFalse(g.Adjacent(0, 2));
        }

        [TestMethod()]
        public void CreateEdge()
        {
            Graph g = new Graph(5);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            Assert.AreEqual(g.CreateEdge(0, 1), new Edge(0, 1, Bond.Implicit));
            Assert.AreEqual(g.CreateEdge(1, 2), new Edge(1, 2, Bond.Implicit));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void EdgeNone()
        {
            Graph g = new Graph(5);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.CreateEdge(0, 2);
        }

        [TestMethod()]
        public void AddTopology()
        {
            var mock_t = new Mock<Topology>();
            mock_t.SetupGet(t => t.Atom).Returns(5);
            Graph g = new Graph(6);
            g.AddTopology(mock_t.Object);
            Assert.AreEqual(g.TopologyOf(5), mock_t.Object);
        }

        [TestMethod()]
        public void AddUnknownTopology()
        {
            Topology t = Topology.Unknown;
            Graph g = new Graph(6);
            g.AddTopology(t);
        }

        [TestMethod()]
        public void DefaultTopology()
        {
            Graph g = new Graph(5);
            Assert.AreEqual(g.TopologyOf(4), Topology.Unknown);
        }

        [TestMethod()]
        public void ClearTest()
        {
            Graph g = new Graph(2);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            Assert.AreEqual(g.Order, 3);
            Assert.AreEqual(g.Size, 2);
            Assert.AreEqual(g.GetEdges(0).Count, 1);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { new Edge(0, 1, Bond.Implicit) },
                g.GetEdges(0)));
            Assert.AreEqual(g.GetEdges(1).Count, 2);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                    new Edge(0, 1, Bond.Implicit),
                    new Edge(1, 2, Bond.Implicit) },
                g.GetEdges(1)));    // fixed Beam's bug

            g.Clear();
            Assert.AreEqual(g.Order, 0);
            Assert.AreEqual(g.Size, 0);
        }

        [TestMethod()]
        public void Permute()
        {
            Graph g = new Graph(2);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(2, 3, Bond.Implicit));

            Assert.AreEqual(g.Degree(0), 1);
            Assert.AreEqual(g.Degree(1), 2);
            Assert.AreEqual(g.Degree(2), 2);
            Assert.AreEqual(g.Degree(3), 1);

            Graph h = g.Permute(new int[] { 1, 0, 3, 2 });
            Assert.AreEqual(h.Degree(0), 2);
            Assert.AreEqual(h.Degree(1), 1);
            Assert.AreEqual(h.Degree(2), 1);
            Assert.AreEqual(h.Degree(3), 2);
            Assert.AreEqual(g.GetAtom(0), h.GetAtom(1));
            Assert.AreEqual(g.GetAtom(1), h.GetAtom(0));
            Assert.AreEqual(g.GetAtom(2), h.GetAtom(3));
            Assert.AreEqual(g.GetAtom(3), h.GetAtom(2));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidPermutation()
        {
            Graph g = new Graph(2);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(2, 3, Bond.Implicit));
            g.Permute(new int[2]);
        }

        [TestMethod()]
        public void SortTest()
        {
            Graph g = new Graph(2);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddEdge(new Edge(3, 2, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(0, 3, Bond.Implicit));
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { new Edge(0, 3, Bond.Implicit), new Edge(0, 1, Bond.Implicit) },
                g.GetEdges(0)));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { new Edge(1, 2, Bond.Implicit), new Edge(1, 0, Bond.Implicit) },
                g.GetEdges(1)));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { new Edge(2, 3, Bond.Implicit), new Edge(2, 1, Bond.Implicit) },
                g.GetEdges(2)));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { new Edge(3, 2, Bond.Implicit), new Edge(3, 0, Bond.Implicit) },
                g.GetEdges(3)));

            g.Sort(new Graph.CanOrderFirst());
            Assert.IsTrue(Compares.AreEqual(
               new[] { new Edge(0, 1, Bond.Implicit), new Edge(0, 3, Bond.Implicit) },
               g.GetEdges(0)));
            Assert.IsTrue(Compares.AreEqual(
                new[] { new Edge(1, 0, Bond.Implicit), new Edge(1, 2, Bond.Implicit) },
                g.GetEdges(1)));
            Assert.IsTrue(Compares.AreEqual(
                new[] { new Edge(2, 1, Bond.Implicit), new Edge(2, 3, Bond.Implicit) },
                g.GetEdges(2)));
            Assert.IsTrue(Compares.AreEqual(
                new[] { new Edge(3, 0, Bond.Implicit), new Edge(3, 2, Bond.Implicit) },
                g.GetEdges(3)));
        }

        [TestMethod()]
        public void Atoms()
        {
            Graph g = new Graph(20);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            g.AddAtom(new Mock<Atom>().Object);
            IEnumerable<Atom> atoms = g.GetAtoms_();
            IEnumerator<Atom> it = atoms.GetEnumerator();
            Assert.IsTrue(it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.IsFalse(it.MoveNext());
        }

        [TestMethod()]
        public void ConfigurationOf()
        {
            Graph g = Graph.FromSmiles("O[C@]12CCCC[C@@]1(O)CCCC2");

            Assert.AreEqual(g.ConfigurationOf(1), Configuration.TH1);
            Assert.AreEqual(g.ConfigurationOf(6), Configuration.TH1);
        }

        [TestMethod()]
        public void ConfigurationOf_myoInositol()
        {
            Graph g = Graph
                    .FromSmiles("O[C@@H]1[C@H](O)[C@H](O)[C@H](O)[C@H](O)[C@@H]1O");

            Assert.AreEqual(g.ConfigurationOf(1), Configuration.TH1);
            Assert.AreEqual(g.ConfigurationOf(2), Configuration.TH1);
            Assert.AreEqual(g.ConfigurationOf(4), Configuration.TH1);
            Assert.AreEqual(g.ConfigurationOf(6), Configuration.TH1);
            Assert.AreEqual(g.ConfigurationOf(8), Configuration.TH1);
            Assert.AreEqual(g.ConfigurationOf(10), Configuration.TH2);
        }

        [TestMethod()]
        public void MyoInositol_neighbors()
        {
            Graph g = Graph
                    .FromSmiles("O[C@@H]1[C@H](O)[C@H](O)[C@H](O)[C@H](O)[C@@H]1O");

            Assert.IsTrue(Compares.AreEqual(g.Neighbors(1), new int[] { 0, 2, 10 }));
            Assert.IsTrue(Compares.AreEqual(g.Neighbors(2), new int[] { 1, 3, 4 }));
            Assert.IsTrue(Compares.AreEqual(g.Neighbors(4), new int[] { 2, 5, 6 }));
            Assert.IsTrue(Compares.AreEqual(g.Neighbors(6), new int[] { 4, 7, 8 }));
            Assert.IsTrue(Compares.AreEqual(g.Neighbors(8), new int[] { 6, 9, 10 }));
            Assert.IsTrue(Compares.AreEqual(g.Neighbors(10), new int[] { 1, 8, 11 }));
        }

        [TestMethod()]
        public void ImplHCount()
        {
            Graph g = Graph.FromSmiles("C1NC=C[C]=C1");
            Assert.AreEqual(g.ImplHCount(0), 2);
            Assert.AreEqual(g.ImplHCount(1), 1);
            Assert.AreEqual(g.ImplHCount(2), 1);
            Assert.AreEqual(g.ImplHCount(3), 1);
            Assert.AreEqual(g.ImplHCount(4), 0);
            Assert.AreEqual(g.ImplHCount(5), 1);
        }

        [TestMethod()]
        public void ImplHCount_nonExpH()
        {
            Graph g = Graph.FromSmiles("C([H])([H])1NC=C[C]=C1");
            Assert.AreEqual(g.ImplHCount(0), 0); // 2 exp hs
            Assert.AreEqual(g.ImplHCount(1), 0); // [H]
            Assert.AreEqual(g.ImplHCount(2), 0); // [H]
            Assert.AreEqual(g.ImplHCount(3), 1);
            Assert.AreEqual(g.ImplHCount(4), 1);
            Assert.AreEqual(g.ImplHCount(5), 1);
            Assert.AreEqual(g.ImplHCount(6), 0);
            Assert.AreEqual(g.ImplHCount(7), 1);
        }

        [TestMethod()]
        public void OutputOrder()
        {
            Graph g = GraphBuilder.Create(5)
                                  .Add(Element.Carbon, 3)
                                  .Add(Element.Carbon, 1)
                                  .Add(Element.Carbon, 2)
                                  .Add(Element.Carbon, 3)
                                  .Add(Element.Carbon, 2)
                                  .Add(Element.Carbon, 3)
                                  .Add(0, 1)
                                  .Add(1, 2)
                                  .Add(1, 3)
                                  .Add(2, 4)
                                  .Add(4, 5)
                                  .Build();
            g.Sort(new Graph.CanOrderFirst());
            int[] visited = new int[g.Order];
            Assert.AreEqual(g.ToSmiles(visited), "CC(CCC)C");
            Assert.IsTrue(Compares.AreEqual(new int[] { 0, 1, 2, 5, 3, 4 }, visited));
        }

        [TestMethod()]
        public void Resonate()
        {
            // two different resonance forms with the same
            // Ordering
            Graph g = Graph.FromSmiles("C1=CC2=CC=CC2=C1");
            Graph h = Graph.FromSmiles("C=1C=C2C=CC=C2C=1");
            // produce different SMILES
            Assert.AreEqual(g.ToSmiles(), "C1=CC2=CC=CC2=C1");
            Assert.AreEqual(h.ToSmiles(), "C=1C=C2C=CC=C2C1");
            // but once resonate we get the same SMILES 
            Assert.AreEqual(g.Resonate().ToSmiles(), h.Resonate().ToSmiles());
        }

        // ensures we don't loose the carbonyl
        [TestMethod()]
        public void Nitrogen_5v()
        {
            Graph g = Graph.FromSmiles("O=N1=CC=CC=C1");
            Graph h = Graph.FromSmiles("O=N=1C=CC=CC1");
            // produce different SMILES
            Assert.AreEqual(g.ToSmiles(), "O=N1=CC=CC=C1");
            Assert.AreEqual(h.ToSmiles(), "O=N=1C=CC=CC1");
            // but once resonate we get the same SMILES 
            Assert.AreEqual(g.Resonate().ToSmiles(), h.Resonate().ToSmiles());
        }

        // ensures we don't loose the allene
        [TestMethod()]
        public void Allene()
        {
            Graph g = Graph.FromSmiles("C1=CC=C=CC=C1");
            Assert.AreEqual(g.ToSmiles(), "C1=CC=C=CC=C1");
            Assert.AreEqual(g.Resonate().ToSmiles(), "C1=CC=C=CC=C1");
        }

        [TestMethod()]
        public void SortH()
        {
            Graph g = Graph.FromSmiles("C(C(C)[H])[H]");
            g.Sort(new Graph.VisitHydrogenFirst());
            Assert.AreEqual(g.ToSmiles(), "C([H])C([H])C");
        }

        [TestMethod()]
        public void SortHIsotopes()
        {
            Graph g = Graph.FromSmiles("C([3H])([2H])[H]");
            g.Sort(new Graph.VisitHydrogenFirst());
            Assert.AreEqual(g.ToSmiles(), "C([H])([2H])[3H]");
        }

        [TestMethod()]
        public void HiBondOrderFirst()
        {
            Graph g = Graph.FromSmiles("C=1C=CC=CC1");
            g.Sort(new Graph.VisitHighOrderFirst());
            Assert.AreEqual(g.ToSmiles(), "C1=CC=CC=C1");
        }

        [TestMethod()]
        public void HiBondOrderFirst2()
        {
            Graph g = Graph.FromSmiles("P(=C)#N");
            g.Sort(new Graph.VisitHighOrderFirst());
            Assert.AreEqual(g.ToSmiles(), "P(#N)=C");
        }

        [TestMethod()]
        public void StableSort()
        {
            Graph g = Graph.FromSmiles("C=1(C(=C(C(=C(C1[H])[H])[H])[H])[H])[H]");
            g.Sort(new Graph.VisitHighOrderFirst());
            g.Sort(new Graph.VisitHydrogenFirst());
            Assert.AreEqual(g.ToSmiles(), "C1([H])=C([H])C([H])=C([H])C([H])=C1[H]");
        }

        [TestMethod()]
        public void CHEMBL1215012()
        {
            Graph g = Graph.FromSmiles("[Na+].[Na+].CC(C)c1c(O)c(O)c(\\C=N\\[C@H]2[C@H]3SC(C)(C)[C@@H](N3C2=O)C(=O)[O-])c4C(=O)C(=C(C)C(=O)c14)C5=C(C)C(=O)c6c(C(C)C)c(O)c(O)c(\\C=N\\[C@H]7[C@H]8SC(C)(C)[C@@H](N8C7=O)C(=O)[O-])c6C5=O CHEMBL1215012");
        }
    }
}
