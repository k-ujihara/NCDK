



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
    /// Checks the functionality of the <see cref="Bond"/>.
    /// </summary>
    [TestClass()]
    public class BondTest : AbstractBondTest
    {
        public override IChemObject NewChemObject()
        {
            return new Bond();
        }

        [TestMethod()]
        public void TestBond()
        {
            IBond bond = new Bond();
            Assert.AreEqual(0, bond.Atoms.Count);
            //Assert.IsNull(bond.Begin);
            //Assert.IsNull(bond.End);
            Assert.AreEqual(default(BondOrder), bond.Order);
            Assert.AreEqual(default(BondStereo), bond.Stereo);
            Assert.AreEqual(BondOrder.Unset, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        [TestMethod()]
        public void TestBond_arrayIAtom()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.NewAtom("C");
            IAtom atom2 = obj.Builder.NewAtom("O");
            IAtom atom3 = obj.Builder.NewAtom("C");
            IAtom atom4 = obj.Builder.NewAtom("C");
            IAtom atom5 = obj.Builder.NewAtom("C");

            IBond bond1 = new Bond(new IAtom[] { atom1, atom2, atom3, atom4, atom5 });
            Assert.AreEqual(5, bond1.Atoms.Count);
            Assert.AreEqual(atom1, bond1.Begin);
            Assert.AreEqual(atom2, bond1.End);
        }

        [TestMethod()]
        public void TestBond_arrayIAtom_BondOrder()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.NewAtom("C");
            IAtom atom2 = obj.Builder.NewAtom("O");
            IAtom atom3 = obj.Builder.NewAtom("C");
            IAtom atom4 = obj.Builder.NewAtom("C");
            IAtom atom5 = obj.Builder.NewAtom("C");

            IBond bond1 = new Bond(new IAtom[] { atom1, atom2, atom3, atom4, atom5 }, BondOrder.Single);
            Assert.AreEqual(5, bond1.Atoms.Count);
            Assert.AreEqual(atom1, bond1.Begin);
            Assert.AreEqual(atom2, bond1.End);
            Assert.AreEqual(BondOrder.Single, bond1.Order);
        }

        [TestMethod()]
        public void TestBond_IAtom_IAtom()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.NewAtom("C");
            IAtom o = obj.Builder.NewAtom("O");
            IBond bond = new Bond(c, o);

            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(c, bond.Begin);
            Assert.AreEqual(o, bond.End);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        [TestMethod()]
        public void TestBond_IAtom_IAtom_BondOrder()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.NewAtom("C");
            IAtom o = obj.Builder.NewAtom("O");
            IBond bond = new Bond(c, o, BondOrder.Double);

            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(c, bond.Begin);
            Assert.AreEqual(o, bond.End);
            Assert.IsTrue(bond.Order == BondOrder.Double);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        [TestMethod()]
        public void TestBond_IAtom_IAtom_BondOrder_IBond_Stereo()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.NewAtom("C");
            IAtom o = obj.Builder.NewAtom("O");
            IBond bond = new Bond(c, o, BondOrder.Single, BondStereo.Up);

            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(c, bond.Begin);
            Assert.AreEqual(o, bond.End);
            Assert.IsTrue(bond.Order == BondOrder.Single);
            Assert.AreEqual(BondStereo.Up, bond.Stereo);
        }

    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="Bond"/>.
    /// </summary>
    [TestClass()]
    public class BondTest : AbstractBondTest
    {
        public override IChemObject NewChemObject()
        {
            return new Bond();
        }

        [TestMethod()]
        public void TestBond()
        {
            IBond bond = new Bond();
            Assert.AreEqual(0, bond.Atoms.Count);
            //Assert.IsNull(bond.Begin);
            //Assert.IsNull(bond.End);
            Assert.AreEqual(default(BondOrder), bond.Order);
            Assert.AreEqual(default(BondStereo), bond.Stereo);
            Assert.AreEqual(BondOrder.Unset, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        [TestMethod()]
        public void TestBond_arrayIAtom()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.NewAtom("C");
            IAtom atom2 = obj.Builder.NewAtom("O");
            IAtom atom3 = obj.Builder.NewAtom("C");
            IAtom atom4 = obj.Builder.NewAtom("C");
            IAtom atom5 = obj.Builder.NewAtom("C");

            IBond bond1 = new Bond(new IAtom[] { atom1, atom2, atom3, atom4, atom5 });
            Assert.AreEqual(5, bond1.Atoms.Count);
            Assert.AreEqual(atom1, bond1.Begin);
            Assert.AreEqual(atom2, bond1.End);
        }

        [TestMethod()]
        public void TestBond_arrayIAtom_BondOrder()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.NewAtom("C");
            IAtom atom2 = obj.Builder.NewAtom("O");
            IAtom atom3 = obj.Builder.NewAtom("C");
            IAtom atom4 = obj.Builder.NewAtom("C");
            IAtom atom5 = obj.Builder.NewAtom("C");

            IBond bond1 = new Bond(new IAtom[] { atom1, atom2, atom3, atom4, atom5 }, BondOrder.Single);
            Assert.AreEqual(5, bond1.Atoms.Count);
            Assert.AreEqual(atom1, bond1.Begin);
            Assert.AreEqual(atom2, bond1.End);
            Assert.AreEqual(BondOrder.Single, bond1.Order);
        }

        [TestMethod()]
        public void TestBond_IAtom_IAtom()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.NewAtom("C");
            IAtom o = obj.Builder.NewAtom("O");
            IBond bond = new Bond(c, o);

            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(c, bond.Begin);
            Assert.AreEqual(o, bond.End);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        [TestMethod()]
        public void TestBond_IAtom_IAtom_BondOrder()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.NewAtom("C");
            IAtom o = obj.Builder.NewAtom("O");
            IBond bond = new Bond(c, o, BondOrder.Double);

            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(c, bond.Begin);
            Assert.AreEqual(o, bond.End);
            Assert.IsTrue(bond.Order == BondOrder.Double);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        [TestMethod()]
        public void TestBond_IAtom_IAtom_BondOrder_IBond_Stereo()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.NewAtom("C");
            IAtom o = obj.Builder.NewAtom("O");
            IBond bond = new Bond(c, o, BondOrder.Single, BondStereo.Up);

            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(c, bond.Begin);
            Assert.AreEqual(o, bond.End);
            Assert.IsTrue(bond.Order == BondOrder.Single);
            Assert.AreEqual(BondStereo.Up, bond.Stereo);
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
