/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by the
 * Free Software Foundation; either version 2.1 of the License, or (at your
 * option) any later version. All we ask is that proper credit is given for our
 * work, which includes - but is not limited to - adding the above copyright
 * notice to the beginning of your source code files, and to any copyright
 * notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using System.Collections.Generic;

namespace NCDK.RingSearches
{
    /// <summary>
    /// Mocking for <see cref="RingSearch"/>. Please refer to RingSearchTest_* for
    /// situation unit tests.
    /// </summary>
    // @author John May
    // @cdk.module test-core
    [TestClass()]
    public class RingSearchTest
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull()
        {
            new RingSearch(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullContainer()
        {
            new RingSearch(null, new Mock<CyclicVertexSearch>().Object);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullCyclicSearch()
        {
            new RingSearch(new Mock<IAtomContainer>().Object, (CyclicVertexSearch)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullGraph()
        {
            new RingSearch(new Mock<IAtomContainer>().Object, (int[][])null);
        }

        [TestMethod()]
        public void TestMatch()
        {
            Assert.IsTrue(RingSearch.Match(0, 0));
            Assert.IsTrue(RingSearch.Match(0, 1));
            Assert.IsTrue(RingSearch.Match(1, 0));
            Assert.IsTrue(RingSearch.Match(5, 0));
            Assert.IsTrue(RingSearch.Match(0, 5));
            Assert.IsTrue(RingSearch.Match(5, 5));

            Assert.IsFalse(RingSearch.Match(-1, -1));
            Assert.IsFalse(RingSearch.Match(6, 5));
            Assert.IsFalse(RingSearch.Match(5, 6));
            Assert.IsFalse(RingSearch.Match(-1, 5));
            Assert.IsFalse(RingSearch.Match(5, -1));
            Assert.IsFalse(RingSearch.Match(-1, 0));
            Assert.IsFalse(RingSearch.Match(0, -1));
        }

        [TestMethod()]
        public void TestCyclic()
        {
            var mock_cyclicSearch = new Mock<CyclicVertexSearch>();
            CyclicVertexSearch cyclicSearch = mock_cyclicSearch.Object;
            IAtomContainer container = new Mock<IAtomContainer>().Object;

            RingSearch ringSearch = new RingSearch(container, cyclicSearch);
            ringSearch.Cyclic();

            mock_cyclicSearch.Verify(n => n.Cyclic(), Times.Once());
        }

        [TestMethod()]
        public void TestCyclic_Int()
        {
            var mock_cyclicSearch = new Mock<CyclicVertexSearch>();
            CyclicVertexSearch cyclicSearch = mock_cyclicSearch.Object;
            IAtomContainer container = new Mock<IAtomContainer>().Object;

            RingSearch ringSearch = new RingSearch(container, cyclicSearch);
            ringSearch.Cyclic(1);

            mock_cyclicSearch.Verify(n => n.Cyclic(1), Times.Once());
        }

        [TestMethod()]
        public void TestCyclic_IntInt()
        {
            var mock_cyclicSearch = new Mock<CyclicVertexSearch>();
            CyclicVertexSearch cyclicSearch = mock_cyclicSearch.Object;
            IAtomContainer container = new Mock<IAtomContainer>().Object;

            RingSearch ringSearch = new RingSearch(container, cyclicSearch);
            ringSearch.Cyclic(2, 4);

            mock_cyclicSearch.Verify(n => n.Cyclic(2, 4), Times.Once());
        }

        [TestMethod()]
        public void TestCyclic_Atom()
        {
            var mock_cyclicSearch = new Mock<CyclicVertexSearch>();
            CyclicVertexSearch cyclicSearch = mock_cyclicSearch.Object;
            var mock_container = new Mock<IAtomContainer>();
            IAtomContainer container = mock_container.Object;
            IAtom atom = new Mock<IAtom>().Object;

            mock_container.Setup(n => n.Atoms.IndexOf(It.IsAny<IAtom>())).Returns(42);

            RingSearch ringSearch = new RingSearch(container, cyclicSearch);
            ringSearch.Cyclic(atom);

            // verify the number returned from getAtomNumber is passed on
            mock_container.Verify(n => n.Atoms.IndexOf(atom), Times.Once());
            mock_cyclicSearch.Verify(n => n.Cyclic(42), Times.Once());
        }

        [TestMethod()]
        public void TestCyclic_Bond()
        {
            var mock_cyclicSearch = new Mock<CyclicVertexSearch>();
            CyclicVertexSearch cyclicSearch = mock_cyclicSearch.Object;
            var mock_container = new Mock<IAtomContainer>();
            IAtomContainer container = mock_container.Object;
            IAtom a1 = new Mock<IAtom>().Object;
            IAtom a2 = new Mock<IAtom>().Object;
            var mock_bond = new Mock<IBond>();
            IBond bond = mock_bond.Object;

            mock_container.Setup(n => n.Atoms.IndexOf(a1)).Returns(42);
            mock_container.Setup(n => n.Atoms.IndexOf(a2)).Returns(43);
            mock_bond.Setup(n => n.Atoms[0]).Returns(a1);
            mock_bond.Setup(n => n.Atoms[1]).Returns(a2);

            RingSearch ringSearch = new RingSearch(container, cyclicSearch);
            ringSearch.Cyclic(bond);

            // verify the number returned from getAtomNumber is passed on
            mock_container.Verify(n => n.Atoms.IndexOf(a1), Times.Once());
            mock_container.Verify(n => n.Atoms.IndexOf(a2), Times.Once());
            mock_cyclicSearch.Verify(n => n.Cyclic(42, 43), Times.Once());
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestCyclic_Atom_NotFound()
        {

            CyclicVertexSearch cyclicSearch = new Mock<CyclicVertexSearch>().Object;
            var mock_container = new Mock<IAtomContainer>();
            IAtomContainer container = mock_container.Object;
            IAtom atom = new Mock<IAtom>().Object;

            mock_container.Setup(n => n.Atoms.IndexOf(It.IsAny<IAtom>())).Returns(-1);

            RingSearch ringSearch = new RingSearch(container, cyclicSearch);

            ringSearch.Cyclic(atom);
        }

        [TestMethod()]
        public void TestIsolated()
        {
            var mock_cyclicSearch = new Mock<CyclicVertexSearch>();
            CyclicVertexSearch cyclicSearch = mock_cyclicSearch.Object;
            IAtomContainer container = new Mock<IAtomContainer>().Object;
            IAtom atom = new Mock<IAtom>().Object;

            RingSearch ringSearch = new RingSearch(container, cyclicSearch);

            ringSearch.Isolated();

            mock_cyclicSearch.Verify(n => n.Isolated(), Times.Once());
        }

        [TestMethod()]
        public void TestFused()
        {
            var mock_cyclicSearch = new Mock<CyclicVertexSearch>();
            CyclicVertexSearch cyclicSearch = mock_cyclicSearch.Object;
            IAtomContainer container = new Mock<IAtomContainer>().Object;
            IAtom atom = new Mock<IAtom>().Object;

            RingSearch ringSearch = new RingSearch(container, cyclicSearch);

            ringSearch.Fused();

            mock_cyclicSearch.Verify(n => n.Fused(), Times.Once());
        }

        [TestMethod()]
        public void TestRingFragments()
        {
            var mock_cyclicSearch = new Mock<CyclicVertexSearch>();
            var mock_container = new Mock<IAtomContainer>();
            var mock_builder = new Mock<IChemObjectBuilder>();
            var mock_atom = new Mock<IAtom>();

            RingSearch ringSearch = new RingSearch(mock_container.Object, mock_cyclicSearch.Object);

            mock_cyclicSearch.Setup(n => n.Cyclic()).Returns(new int[] { 0, 1, 2 });
            mock_cyclicSearch.Setup(n => n.Isolated()).Returns(new int[][] { new[] { 0, 1, 2 } });
            mock_cyclicSearch.Setup(n => n.Fused()).Returns(new int[0][]);
            mock_container.Setup(n => n.Atoms.Count).Returns(3);
            mock_builder.Setup(n => n.CreateAtomContainer(It.IsAny<IEnumerable<IAtom>>(), It.IsAny<IEnumerable<IBond>>())).Returns(new Mock<IAtomContainer>().Object);
            mock_container.Setup(n => n.Builder).Returns(mock_builder.Object);
            mock_container.Setup(n => n.Bonds).Returns(new List<IBond>());

            ringSearch.RingFragments();

            mock_cyclicSearch.Verify(n => n.Cyclic(), Times.Once());

            // atoms were accessed
            mock_container.Verify(n => n.Atoms[0], Times.Once());
            mock_container.Verify(n => n.Atoms[1], Times.Once());
            mock_container.Verify(n => n.Atoms[2], Times.Once());

            // builder was invoked
            mock_builder.Verify(n => n.CreateAtomContainer(It.IsAny<IEnumerable<IAtom>>(), It.IsAny<IEnumerable<IBond>>()), Times.Once());
        }

        [TestMethod()]
        public void TestIsolatedRingFragments()
        {
            var mock_cyclicSearch = new Mock<CyclicVertexSearch>();
            var mock_container = new Mock<IAtomContainer>();
            var mock_builder = new Mock<IChemObjectBuilder>();
            var mock_atom = new Mock<IAtom>();

            RingSearch ringSearch = new RingSearch(mock_container.Object, mock_cyclicSearch.Object);

            mock_cyclicSearch.Setup(n => n.Isolated()).Returns(new int[][] { new[] { 0, 1 }, new[] { 2 } });
            mock_container.Setup(n => n.Builder).Returns(mock_builder.Object);
            mock_container.Setup(n => n.Bonds).Returns(new List<IBond>());
            mock_container.Setup(n => n.Atoms[It.IsAny<int>()]).Returns(new Mock<IAtom>().Object);
            mock_builder.Setup(n => n.CreateAtomContainer(It.IsAny<IEnumerable<IAtom>>(), It.IsAny<IEnumerable<IBond>>())).Returns(new Mock<IAtomContainer>().Object);

            ringSearch.IsolatedRingFragments();

            mock_cyclicSearch.Verify(n => n.Isolated(), Times.Once());

            // atoms were accessed
            mock_container.Verify(n => n.Atoms[0], Times.Once());
            mock_container.Verify(n => n.Atoms[1], Times.Once());
            mock_container.Verify(n => n.Atoms[2], Times.Once());

            // builder was invoked
            mock_builder.Verify(n => n.CreateAtomContainer(It.IsAny<IEnumerable<IAtom>>(), It.IsAny<IEnumerable<IBond>>()), Times.Exactly(2));
        }

        [TestMethod()]
        public void TestFusedRingFragments()
        {
            var mock_cyclicSearch = new Mock<CyclicVertexSearch>();
            var mock_container = new Mock<IAtomContainer>();
            var mock_builder = new Mock<IChemObjectBuilder>();
            var mock_atom = new Mock<IAtom>();

            RingSearch ringSearch = new RingSearch(mock_container.Object, mock_cyclicSearch.Object);

            mock_cyclicSearch.Setup(n => n.Fused()).Returns(new int[][] { new[] { 0, 1 }, new[] { 2 } });
            mock_container.Setup(n => n.Builder).Returns(mock_builder.Object);
            mock_builder.Setup(n => n.CreateAtomContainer(It.IsAny<IEnumerable<IAtom>>(), It.IsAny<IEnumerable<IBond>>())).Returns(new Mock<IAtomContainer>().Object);
            mock_container.Setup(n => n.Bonds).Returns(new List<IBond>());
            mock_container.Setup(n => n.Atoms[It.IsAny<int>()]).Returns(new Mock<IAtom>().Object);

            ringSearch.FusedRingFragments();

            mock_cyclicSearch.Verify(n => n.Fused(), Times.Once());

            // atoms were accessed
            mock_container.Verify(n => n.Atoms[0], Times.Once());
            mock_container.Verify(n => n.Atoms[1], Times.Once());
            mock_container.Verify(n => n.Atoms[2], Times.Once());

            // builder was invoked
            mock_builder.Verify(n => n.CreateAtomContainer(It.IsAny<IEnumerable<IAtom>>(), It.IsAny<IEnumerable<IBond>>()), Times.Exactly(2));
        }

        [TestMethod()]
        public void ConnectingEdge1()
        {
            IAtomContainer mol = DiSpiroPentane();
            RingSearch rs = new RingSearch(mol);
            IAtomContainer frag = rs.RingFragments();
            Assert.AreEqual(frag.Bonds.Count + 1, mol.Bonds.Count);
        }

        [TestMethod()]
        public void ConnectingEdge2()
        {
            IAtomContainer mol = TriSpiroPentane();
            RingSearch rs = new RingSearch(mol);
            IAtomContainer frag = rs.RingFragments();
            Assert.AreEqual(frag.Bonds.Count, mol.Bonds.Count);
        }

        /// <summary>
        /// Hypothetial molecule - C1C[C]11(CC1)[C]123CC1.C2C3
        /// </summary>
        // @cdk.inchi InChI=1/C10H16/c1-2-9(1,3-4-9)10(5-6-10)7-8-10/h1-8H2
        public static IAtomContainer DiSpiroPentane()
        {

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IAtom a8 = builder.CreateAtom("C");
            a8.FormalCharge = 0;
            mol.Atoms.Add(a8);
            IAtom a9 = builder.CreateAtom("C");
            a9.FormalCharge = 0;
            mol.Atoms.Add(a9);
            IAtom a10 = builder.CreateAtom("C");
            a10.FormalCharge = 0;
            mol.Atoms.Add(a10);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a4, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = builder.CreateBond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = builder.CreateBond(a6, a7, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = builder.CreateBond(a7, a8, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = builder.CreateBond(a6, a8, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = builder.CreateBond(a6, a9, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = builder.CreateBond(a9, a10, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = builder.CreateBond(a6, a10, BondOrder.Single);
            mol.Bonds.Add(b13);
            return mol;
        }

        /// <summary>
        /// Hypothetial molecule - C1C[C]1123CC1.C1C[C]211(CC1)C3
        /// </summary>
        // @cdk.inchi InChI=1/C11H18/c1-2-10(1,3-4-10)9-11(10,5-6-11)7-8-11/h1-9H2
        public static IAtomContainer TriSpiroPentane()
        {

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IAtom a8 = builder.CreateAtom("C");
            a8.FormalCharge = 0;
            mol.Atoms.Add(a8);
            IAtom a9 = builder.CreateAtom("C");
            a9.FormalCharge = 0;
            mol.Atoms.Add(a9);
            IAtom a10 = builder.CreateAtom("C");
            a10.FormalCharge = 0;
            mol.Atoms.Add(a10);
            IAtom a11 = builder.CreateAtom("C");
            a11.FormalCharge = 0;
            mol.Atoms.Add(a11);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a4, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = builder.CreateBond(a6, a7, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = builder.CreateBond(a7, a8, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = builder.CreateBond(a3, a8, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = builder.CreateBond(a6, a8, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = builder.CreateBond(a8, a9, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = builder.CreateBond(a9, a10, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = builder.CreateBond(a8, a10, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = builder.CreateBond(a8, a11, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = builder.CreateBond(a3, a11, BondOrder.Single);
            mol.Bonds.Add(b15);
            return mol;
        }
    }
}
