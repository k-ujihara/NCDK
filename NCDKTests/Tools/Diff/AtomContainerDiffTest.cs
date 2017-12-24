/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace NCDK.Tools.Diff
{
    // @cdk.module test-diff 
    public class AtomContainerDiffTest : CDKTestCase
    {
        [TestMethod()]
        public void TestMatchAgainstItself()
        {
            var m_container = new Mock<IAtomContainer>(); IAtomContainer container = m_container.Object;
            m_container.Setup(n => n.GetElectronContainers().Count()).Returns(1);
            m_container.Setup(n => n.GetElectronContainers().First()).Returns(new Mock<IBond>().Object);
            string result = AtomContainerDiff.Diff(container, container);
            AssertZeroLength(result);
        }

        [TestMethod()]
        public void TestDiff()
        {
            var m_carbon = new Mock<IAtom>(); IAtom carbon = m_carbon.Object;
            var m_oxygen = new Mock<IAtom>(); IAtom oxygen = m_oxygen.Object;

            m_carbon.SetupGet(n => n.Symbol).Returns("C");
            m_carbon.SetupGet(n => n.Symbol).Returns("C");
            m_oxygen.SetupGet(n => n.Symbol).Returns("O");

            var m_b1 = new Mock<IBond>(); IBond b1 = m_b1.Object;
            var m_b2 = new Mock<IBond>(); IBond b2 = m_b2.Object;

            m_b1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_b2.SetupGet(n => n.Order).Returns(BondOrder.Double);

            m_b1.SetupGet(n => n.Atoms.Count).Returns(2);
            m_b2.SetupGet(n => n.Atoms.Count).Returns(2);

            m_b1.SetupGet(n => n.Begin).Returns(carbon);
            m_b1.SetupGet(n => n.End).Returns(carbon);
            m_b2.SetupGet(n => n.Begin).Returns(carbon);
            m_b2.SetupGet(n => n.End).Returns(oxygen);

            var m_container1 = new Mock<IAtomContainer>(); IAtomContainer container1 = m_container1.Object;
            var m_container2 = new Mock<IAtomContainer>(); IAtomContainer container2 = m_container2.Object;
            m_container1.Setup(n => n.GetElectronContainers().Count()).Returns(1);
            m_container2.Setup(n => n.GetElectronContainers().Count()).Returns(1);
            m_container1.Setup(n => n.GetElectronContainers().First()).Returns(b1);
            m_container2.Setup(n => n.GetElectronContainers().First()).Returns(b2);

            string result = AtomContainerDiff.Diff(container1, container2);
            Assert.IsNotNull(result);
            Assert.AreNotSame(0, result.Length);
            AssertContains(result, "AtomContainerDiff");
            AssertContains(result, "BondDiff");
            AssertContains(result, "Single/DOUBLE");
            AssertContains(result, "AtomDiff");
            AssertContains(result, "C/O");
        }

        [TestMethod()]
        public void TestDifference()
        {
            var m_carbon = new Mock<IAtom>(); IAtom carbon = m_carbon.Object;
            var m_oxygen = new Mock<IAtom>(); IAtom oxygen = m_oxygen.Object;

            m_carbon.SetupGet(n => n.Symbol).Returns("C");
            m_oxygen.SetupGet(n => n.Symbol).Returns("O");

            var m_b1 = new Mock<IBond>(); IBond b1 = m_b1.Object;
            var m_b2 = new Mock<IBond>(); IBond b2 = m_b2.Object;

            m_b1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_b2.SetupGet(n => n.Order).Returns(BondOrder.Double);

            m_b1.SetupGet(n => n.Atoms.Count).Returns(2);
            m_b2.SetupGet(n => n.Atoms.Count).Returns(2);

            m_b1.SetupGet(n => n.Begin).Returns(carbon);
            m_b1.SetupGet(n => n.End).Returns(carbon);
            m_b2.SetupGet(n => n.Begin).Returns(carbon);
            m_b2.SetupGet(n => n.End).Returns(oxygen);

            var m_container1 = new Mock<IAtomContainer>(); IAtomContainer container1 = m_container1.Object;
            var m_container2 = new Mock<IAtomContainer>(); IAtomContainer container2 = m_container2.Object;
            m_container1.Setup(n => n.GetElectronContainers().Count()).Returns(1);
            m_container2.Setup(n => n.GetElectronContainers().Count()).Returns(1);
            m_container1.Setup(n => n.GetElectronContainers().First()).Returns(b1);
            m_container2.Setup(n => n.GetElectronContainers().First()).Returns(b2);

            string result = AtomContainerDiff.Diff(container1, container2);
            Assert.IsNotNull(result);
        }

        [TestMethod(), Ignore()] //unit test did not test AtomContainerDiff but rather the ability of AtomContainer to be serialized. This is already tested in each respective domain module
        public void TestDiffFromSerialized()
        {
            //        IAtomContainer atomContainer = new AtomContainer();
            //        IBond bond1 = new Bond(new Atom("C"), new Atom("C"));
            //        bond1.setOrder(BondOrder.Single);
            //        atomContainer.addBond(bond1);
            //
            //        File tmpFile = File.createTempFile("serialized", ".dat");
            //        tmpFile.deleteOnExit();
            //        String objFilename = tmpFile.getAbsolutePath();
            //
            //        FileOutputStream fout = new FileOutputStream(objFilename);
            //        ObjectOutputStream ostream = new ObjectOutputStream(fout);
            //        ostream.writeObject(atomContainer);
            //
            //        ostream.close();
            //        fout.close();
            //
            //        // now read the serialized atomContainer in
            //        FileInputStream fin = new FileInputStream(objFilename);
            //        ObjectInputStream istream = new ObjectInputStream(fin);
            //        Object obj = istream.readObject();
            //
            //        Assert.assertTrue(obj instanceof IAtomContainer);
            //
            //        IAtomContainer newAtomContainer = (IAtomContainer) obj;
            //        String diff = AtomDiff.diff(atomContainer, newAtomContainer);
            //
            //        Assert.assertTrue("There were differences between original and deserialized version!", diff.equals(""));
        }
    }
}
