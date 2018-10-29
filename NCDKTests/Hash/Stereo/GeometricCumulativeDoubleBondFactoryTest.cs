/*
 * Copyright (c) 2013 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Silent;

namespace NCDK.Hash.Stereo
{
    /// <summary>
    /// Some isolated test of the cumulative encoder factory, check out <see cref="HashCodeScenariosTest"/>
    /// for actual examples encoding allene and cumulene.
    /// </summary>
    // @author John May
    // @cdk.module test-hash
    [TestClass()]
    public class GeometricCumulativeDoubleBondFactoryTest
    {
        private static IAtom CarbonAt(double x, double y)
        {
            IAtom atom = new Atom("C");
            atom.Point2D = new Vector2(x, y);
            return atom;
        }

        [TestMethod()]
        public void TestCreate()
        {
            var m = new AtomContainer();
            m.Atoms.Add(CarbonAt(-0.2994, 3.2084));
            m.Atoms.Add(CarbonAt(-1.1244, 3.2084));
            m.Atoms.Add(CarbonAt(-1.9494, 3.2084));
            m.Atoms.Add(CarbonAt(-2.3619, 2.4939));
            m.Atoms.Add(CarbonAt(0.1131, 3.9228));
            m.Bonds.Add(new Bond(m.Atoms[0], m.Atoms[1], BondOrder.Double));
            m.Bonds.Add(new Bond(m.Atoms[1], m.Atoms[2], BondOrder.Double));
            m.Bonds.Add(new Bond(m.Atoms[2], m.Atoms[3]));
            m.Bonds.Add(new Bond(m.Atoms[0], m.Atoms[4]));

            IStereoEncoderFactory factory = new GeometricCumulativeDoubleBondFactory();
            // graph not used
            IStereoEncoder encoder = factory.Create(m, null);
            Assert.IsInstanceOfType(encoder, typeof(MultiStereoEncoder));
        }

        [TestMethod()]
        public void TestAxialEncoder_Empty()
        {
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            var m_start = new Mock<IAtom>(); var start = m_start.Object;
            var m_end = new Mock<IAtom>(); var end = m_end.Object;
            m_container.Setup(n => n.GetConnectedBonds(start)).Returns(new IBond[0]);
            m_container.Setup(n => n.GetConnectedBonds(end)).Returns(new IBond[0]);
            Assert.IsNull(GeometricCumulativeDoubleBondFactory.AxialEncoder(container, start, end));
        }

        [TestMethod()]
        public void TestElevation_Atom_Up()
        {
            var m_a1 = new Mock<IAtom>(); var a1 = m_a1.Object;
            var m_a2 = new Mock<IAtom>(); var a2 = m_a2.Object;
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.Stereo).Returns(BondStereo.Up);
            m_bond.SetupGet(n => n.Begin).Returns(a1);
            m_bond.SetupGet(n => n.End).Returns(a2);
            Assert.AreEqual(+1, GeometricCumulativeDoubleBondFactory.Elevation(bond, a1));
            Assert.AreEqual(-1, GeometricCumulativeDoubleBondFactory.Elevation(bond, a2));
        }

        [TestMethod()]
        public void TestElevation_Atom_Down()
        {
            var m_a1 = new Mock<IAtom>(); var a1 = m_a1.Object;
            var m_a2 = new Mock<IAtom>(); var a2 = m_a2.Object;
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.Stereo).Returns(BondStereo.Down);
            m_bond.SetupGet(n => n.Begin).Returns(a1);
            m_bond.SetupGet(n => n.End).Returns(a2);
            Assert.AreEqual(-1, GeometricCumulativeDoubleBondFactory.Elevation(bond, a1));
            Assert.AreEqual(+1, GeometricCumulativeDoubleBondFactory.Elevation(bond, a2));
        }

        [TestMethod()]
        public void TestElevation_null()
        {
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.AreEqual(0, GeometricCumulativeDoubleBondFactory.Elevation(bond));
        }

        [TestMethod()]
        public void TestElevation_Up()
        {
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.Stereo).Returns(BondStereo.Up);
            Assert.AreEqual(+1, GeometricCumulativeDoubleBondFactory.Elevation(bond));
        }

        [TestMethod()]
        public void TestElevation_Down()
        {
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.Stereo).Returns(BondStereo.Down);
            Assert.AreEqual(-1, GeometricCumulativeDoubleBondFactory.Elevation(bond));
        }
    }
}
