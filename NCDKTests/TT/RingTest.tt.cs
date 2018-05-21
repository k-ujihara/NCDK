


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

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the <see cref="Ring"/>.
    /// </summary>
    [TestClass()]
    public class RingTest : AbstractRingTest
    {
        public override IChemObject NewChemObject()
        {
            return new Ring();
        }

        [TestMethod()]
        public void TestRing_int_String()
        {
            IRing r = new Ring(5, "C");
            Assert.AreEqual(5, r.Atoms.Count);
            Assert.AreEqual(5, r.Bonds.Count);
        }

        [TestMethod()]
        public void TestRing_int()
        {
            var r = new Ring(); // This does not create a ring!
            Assert.AreEqual(0, r.Atoms.Count);
            Assert.AreEqual(0, r.Bonds.Count);
        }

        [TestMethod()]
        public void TestRing()
        {
            IRing ring = new Ring();
            Assert.IsNotNull(ring);
            Assert.AreEqual(0, ring.Atoms.Count);
            Assert.AreEqual(0, ring.Bonds.Count);
        }

        [TestMethod()]
        public void TestRing_IAtomContainer()
        {
            IAtomContainer container = NewChemObject().Builder.NewAtomContainer();
            container.Atoms.Add(container.Builder.NewAtom("C"));
            container.Atoms.Add(container.Builder.NewAtom("C"));

            IRing ring = new Ring(container);
            Assert.IsNotNull(ring);
            Assert.AreEqual(2, ring.Atoms.Count);
            Assert.AreEqual(0, ring.Bonds.Count);
        }
        
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="Ring"/>.
    /// </summary>
    [TestClass()]
    public class RingTest : AbstractRingTest
    {
        public override IChemObject NewChemObject()
        {
            return new Ring();
        }

        [TestMethod()]
        public void TestRing_int_String()
        {
            IRing r = new Ring(5, "C");
            Assert.AreEqual(5, r.Atoms.Count);
            Assert.AreEqual(5, r.Bonds.Count);
        }

        [TestMethod()]
        public void TestRing_int()
        {
            var r = new Ring(); // This does not create a ring!
            Assert.AreEqual(0, r.Atoms.Count);
            Assert.AreEqual(0, r.Bonds.Count);
        }

        [TestMethod()]
        public void TestRing()
        {
            IRing ring = new Ring();
            Assert.IsNotNull(ring);
            Assert.AreEqual(0, ring.Atoms.Count);
            Assert.AreEqual(0, ring.Bonds.Count);
        }

        [TestMethod()]
        public void TestRing_IAtomContainer()
        {
            IAtomContainer container = NewChemObject().Builder.NewAtomContainer();
            container.Atoms.Add(container.Builder.NewAtom("C"));
            container.Atoms.Add(container.Builder.NewAtom("C"));

            IRing ring = new Ring(container);
            Assert.IsNotNull(ring);
            Assert.AreEqual(2, ring.Atoms.Count);
            Assert.AreEqual(0, ring.Bonds.Count);
        }
        
 
		        [TestMethod()]
        public override void TestSetAtoms_RemoveListener()
        {
            ChemObjectTestHelper.TestSetAtoms_RemoveListener(NewChemObject());
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

    }
}

