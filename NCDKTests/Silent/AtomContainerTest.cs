/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using System.Linq;

namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="AtomContainer"/>.
    /// </summary>
    // @cdk.module test-silent
    [TestClass()]
    public class AtomContainerTest : AbstractAtomContainerTest
    {
        public override IChemObject NewChemObject()
        {
            return new AtomContainer();
        }

        [TestMethod()]
        public void TestAtomContainer_int_int_int_int()
        {
            // create an empty container with predefined
            // array lengths
            IAtomContainer ac = new AtomContainer();

            Assert.AreEqual(0, ac.Atoms.Count);
            Assert.AreEqual(0, ac.GetElectronContainers().Count());

            // test whether the ElectronContainer is correctly initialized
            ac.Bonds.Add(ac.Builder.CreateBond(ac.Builder.CreateAtom("C"),
                    ac.Builder.CreateAtom("C"), BondOrder.Double));
            ac.LonePairs.Add(ac.Builder.CreateLonePair(ac.Builder.CreateAtom("N")));
        }

        [TestMethod()]
        public void TestAtomContainer()
        {
            // create an empty container with in the constructor defined array lengths
            IAtomContainer container = new AtomContainer();

            Assert.AreEqual(0, container.Atoms.Count);
            Assert.AreEqual(0, container.Bonds.Count);

            // test whether the ElectronContainer is correctly initialized
            container.Bonds.Add(container.Builder.CreateBond(container.Builder.CreateAtom("C"),
                    container.Builder.CreateAtom("C"), BondOrder.Double));
            container.LonePairs.Add(container.Builder.CreateLonePair(container.Builder.CreateAtom("N")));
        }

        [TestMethod()]
        public void TestAtomContainer_IAtomContainer()
        {
            IAtomContainer acetone = NewChemObject().Builder.CreateAtomContainer();
            IAtom c1 = acetone.Builder.CreateAtom("C");
            IAtom c2 = acetone.Builder.CreateAtom("C");
            IAtom o = acetone.Builder.CreateAtom("O");
            IAtom c3 = acetone.Builder.CreateAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.CreateBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.CreateBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.CreateBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            IAtomContainer container = new AtomContainer(acetone);
            Assert.AreEqual(4, container.Atoms.Count);
            Assert.AreEqual(3, container.Bonds.Count);
        }

        // Overwrite default methods: no notifications are expected!

        [TestMethod()]
        public override void TestNotifyChanged()
        {
            ChemObjectTestHelper.TestNotifyChanged(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_SetFlag()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetFlag(NewChemObject());
        }

        [TestMethod()]
        public void TestNotifyChanged_SetFlags()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetFlags(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_IChemObjectChangeEvent()
        {
            ChemObjectTestHelper.TestNotifyChanged_IChemObjectChangeEvent(NewChemObject());
        }

        [TestMethod()]
        public override void TestStateChanged_IChemObjectChangeEvent()
        {
            ChemObjectTestHelper.TestStateChanged_IChemObjectChangeEvent(NewChemObject());
        }

        [TestMethod()]
        public override void TestClone_ChemObjectListeners()
        {
            ChemObjectTestHelper.TestClone_ChemObjectListeners(NewChemObject());
        }

        [TestMethod()]
        public override void TestAddListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestAddListener_IChemObjectListener(NewChemObject());
        }

        [TestMethod()]
        public override void TestGetListenerCount()
        {
            ChemObjectTestHelper.TestGetListenerCount(NewChemObject());
        }

        [TestMethod()]
        public override void TestRemoveListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestRemoveListener_IChemObjectListener(NewChemObject());
        }

        [TestMethod()]
        public override void TestSetNotification_true()
        {
            ChemObjectTestHelper.TestSetNotification_true(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_SetProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetProperty(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_RemoveProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_RemoveProperty(NewChemObject());
        }

        [TestMethod()]
        public override void TestSetAtoms_removeListener()
        {
            ChemObjectTestHelper.TestSetAtoms_removeListener(NewChemObject());
        }
    }
}
