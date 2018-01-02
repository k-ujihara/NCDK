/*
 * Copyright (c) 2013. John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NCDK.Hash
{
    // @author John May
    // @cdk.module test-hash
    [TestClass()]
    public class BasicAtomEncoderTest
    {
        [TestMethod()]
        public void TestAtomicNumber()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.AtomicNumber;

            m_atom.SetupGet(n => n.AtomicNumber).Returns(6);
            Assert.AreEqual(6, encoder.Encode(atom, container));

            m_atom.Verify(n => n.AtomicNumber, Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }

        [TestMethod()]
        public void TestAtomicNumber_Null()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.AtomicNumber;

            m_atom.SetupGet(n => n.AtomicNumber).Returns((int?)null);
            Assert.AreEqual(32451169, encoder.Encode(atom, container));
            m_atom.Verify(n => n.AtomicNumber, Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }

        [TestMethod()]
        public void TestMassNumber()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.MassNumber;

            m_atom.Setup(n => n.MassNumber).Returns(12);
            Assert.AreEqual(12, encoder.Encode(atom, container));

            m_atom.Verify(n => n.MassNumber, Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }

        [TestMethod()]
        public void TestMassNumber_Null()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.MassNumber;

            m_atom.SetupGet(n => n.MassNumber).Returns((int?)null);
            Assert.AreEqual(32451179, encoder.Encode(atom, container));
            m_atom.Verify(n => n.MassNumber, Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }

        [TestMethod()]
        public void TestFormalNumber()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.FormalCharge;

            m_atom.SetupGet(n => n.FormalCharge).Returns(-2);
            Assert.AreEqual(-2, encoder.Encode(atom, container));

            m_atom.Verify(n => n.FormalCharge, Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }

        [TestMethod()]
        public void TestFormalNumber_Null()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.FormalCharge;

            m_atom.SetupGet(n => n.FormalCharge).Returns((int?)null);
            Assert.AreEqual(32451193, encoder.Encode(atom, container));
            m_atom.Verify(n => n.FormalCharge, Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }

        [TestMethod()]
        public void TestNConnectedAtoms()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.NConnectedAtoms;

            m_container.Setup(n => n.GetConnectedBonds(atom)).Returns(new IBond[2]);
            Assert.AreEqual(2, encoder.Encode(atom, container));
            m_container.Verify(n => n.GetConnectedBonds(atom), Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }

        [TestMethod()]
        public void TestBondOrderSum()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.BondOrderSum;

            m_container.Setup(n => n.GetBondOrderSum(atom)).Returns(3D);
            Assert.AreEqual(3D.GetHashCode(), encoder.Encode(atom, container));
            m_container.Verify(n => n.GetBondOrderSum(atom), Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }

        [TestMethod()]
        public void TestOrbitalHybridization()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.OrbitalHybridization;

            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);
            Assert.AreEqual(Hybridization.SP2, encoder.Encode(atom, container));

            m_atom.Verify(n => n.Hybridization, Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }

        [TestMethod()]
        public void TestOrbitalHybridization_Null()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.OrbitalHybridization;

            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.Unset);
            Assert.AreEqual(32451301, encoder.Encode(atom, container));
            m_atom.Verify(n => n.Hybridization, Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }

        [TestMethod()]
        public void TestFreeRadicals()
        {
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            IAtomEncoder encoder = BasicAtomEncoder.FreeRadicals;

            m_container.Setup(n => n.GetConnectedSingleElectrons(atom)).Returns(new ISingleElectron[1]);
            Assert.AreEqual(1, encoder.Encode(atom, container));
            m_container.Verify(n => n.GetConnectedSingleElectrons(atom), Times.Exactly(1));
            //VerifyNoMoreInteractions(atom, container);
        }
    }
}
