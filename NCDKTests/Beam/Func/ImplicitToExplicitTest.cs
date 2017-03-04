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
using Moq;
using NCDK.Common.Base;

namespace NCDK.Beam
{
    // @author John May 
    [TestClass()]
    public class ImplicitToExplicitTest
    {
        [TestMethod()]
        public void CycloHexane()
        {
            Graph g = new Graph(6);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(2, 3, Bond.Implicit));
            g.AddEdge(new Edge(3, 4, Bond.Implicit));
            g.AddEdge(new Edge(4, 5, Bond.Implicit));
            g.AddEdge(new Edge(5, 0, Bond.Implicit));

            Graph h = new ImplicitToExplicit().Apply(g);

            Assert.AreNotSame(g, h);

            for (int u = 0; u < h.Order; u++)
            {
                foreach (var e in h.GetEdges(u))
                {
                    Assert.AreEqual(Bond.Single, e.Bond);
                }
            }
        }

        [TestMethod()]
        public void AromaticBenzene()
        {
            Graph g = new Graph(6);
            g.AddAtom(AtomImpl.AromaticSubset.Carbon);
            g.AddAtom(AtomImpl.AromaticSubset.Carbon);
            g.AddAtom(AtomImpl.AromaticSubset.Carbon);
            g.AddAtom(AtomImpl.AromaticSubset.Carbon);
            g.AddAtom(AtomImpl.AromaticSubset.Carbon);
            g.AddAtom(AtomImpl.AromaticSubset.Carbon);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Implicit));
            g.AddEdge(new Edge(2, 3, Bond.Implicit));
            g.AddEdge(new Edge(3, 4, Bond.Implicit));
            g.AddEdge(new Edge(4, 5, Bond.Implicit));
            g.AddEdge(new Edge(5, 0, Bond.Implicit));

            Graph h = new ImplicitToExplicit().Apply(g);

            Assert.AreNotSame(h, g);

            for (int u = 0; u < h.Order; u++)
            {
                foreach (var e in h.GetEdges(u))
                {
                    Assert.AreEqual(Bond.Aromatic, e.Bond);
                }
            }
        }

        [TestMethod()]
        public void KekuleBenzene()
        {
            Graph g = new Graph(6);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddAtom(AtomImpl.AliphaticSubset.Carbon);
            g.AddEdge(new Edge(0, 1, Bond.Implicit));
            g.AddEdge(new Edge(1, 2, Bond.Double));
            g.AddEdge(new Edge(2, 3, Bond.Implicit));
            g.AddEdge(new Edge(3, 4, Bond.Double));
            g.AddEdge(new Edge(4, 5, Bond.Implicit));
            g.AddEdge(new Edge(5, 0, Bond.Double));

            Graph h = new ImplicitToExplicit().Apply(g);

            Assert.AreNotSame(h, g);

            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                        new Edge(0, 1, Bond.Single),
                        new Edge(0, 5, Bond.Double) },
                h.GetEdges(0)));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                        new Edge(1, 0, Bond.Single),
                        new Edge(1, 2, Bond.Double) },
                h.GetEdges(1)));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                        new Edge(2, 1, Bond.Double),
                        new Edge(2, 3, Bond.Single) },
                h.GetEdges(2)));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                        new Edge(3, 2, Bond.Single),
                        new Edge(3, 4, Bond.Double) },
                h.GetEdges(3)));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                        new Edge(4, 3, Bond.Double),
                        new Edge(4, 5, Bond.Single) },
                h.GetEdges(4)));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                        new Edge(5, 0, Bond.Double),
                        new Edge(5, 4, Bond.Single) },
                h.GetEdges(5)));
        }

        [TestMethod()]
        public void AromaticType()
        {
            var mock_a = new Mock<Atom>();
            var mock_b = new Mock<Atom>();
            mock_a.Setup(n => n.IsAromatic()).Returns(true);
            mock_b.Setup(n => n.IsAromatic()).Returns(true);
            Assert.AreEqual(Bond.Aromatic, ImplicitToExplicit.Type(mock_a.Object, mock_b.Object));
        }

        [TestMethod()]
        public void SingleType()
        {
            var mock_a = new Mock<Atom>();
            var mock_b = new Mock<Atom>();
            mock_a.Setup(n => n.IsAromatic()).Returns(true);
            mock_b.Setup(n => n.IsAromatic()).Returns(false);
            Assert.AreEqual(Bond.Single, ImplicitToExplicit.Type(mock_a.Object, mock_b.Object));

            mock_a.Setup(n => n.IsAromatic()).Returns(false);
            mock_b.Setup(n => n.IsAromatic()).Returns(true);
            Assert.AreEqual(Bond.Single, ImplicitToExplicit.Type(mock_a.Object, mock_b.Object));

            mock_a.Setup(n => n.IsAromatic()).Returns(false);
            mock_b.Setup(n => n.IsAromatic()).Returns(false);
            Assert.AreEqual(Bond.Single, ImplicitToExplicit.Type(mock_a.Object, mock_b.Object));
        }

        [TestMethod()]
        public void ToExplicitEdge_NonImplicitIdentity()
        {
            Graph g = new Graph(0);
            foreach (var b in Bond.Values)
            {
                if (b != Bond.Implicit)
                {
                    Edge e = new Edge(0, 1, Bond.Single);
                    Assert.AreSame(e, ImplicitToExplicit
                                              .ToExplicitEdge(g, e));
                }
            }
        }

        [TestMethod()]
        public void ToExplicitEdge()
        {
            Graph g = new Graph(2);

            var mock_u = new Mock<Atom>();
            var mock_v = new Mock<Atom>();

            mock_u.Setup(n => n.IsAromatic()).Returns(false);
            mock_v.Setup(n => n.IsAromatic()).Returns(false);

            g.AddAtom(mock_u.Object);
            g.AddAtom(mock_v.Object);

            Edge e = new Edge(0, 1, Bond.Implicit);
            Assert.AreNotSame(e, ImplicitToExplicit.ToExplicitEdge(g, e));

            Assert.AreEqual(new Edge(0, 1, Bond.Single), ImplicitToExplicit.ToExplicitEdge(g, e));
        }
    }
}
