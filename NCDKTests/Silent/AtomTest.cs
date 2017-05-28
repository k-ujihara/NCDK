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
 *
 */
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="Atom"/>.
    /// </summary>
    // @cdk.module test-silent
    [TestClass()]
    public class AtomTest : AbstractAtomTest
    {
        public override IChemObject NewChemObject()
        {
            return new Atom();
        }

        [TestMethod()]
        public void TestAtom()
        {
            IAtom a = new Atom();
            Assert.IsNotNull(a);
        }

        [TestMethod()]
        public void TestAtom_IElement()
        {
            IElement element = NewChemObject().Builder.CreateElement();
            IAtom a = new Atom(element);
            Assert.IsNotNull(a);
        }

        [TestMethod()]
        public void TestAtom_String()
        {
            IAtom a = new Atom("C");
            Assert.AreEqual("C", a.Symbol);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestAtom_NH4plus_direct()
        {
            IAtom a = new Atom(7, 4, +1);
            Assert.AreEqual("N", a.Symbol);
            Assert.AreEqual(7, a.AtomicNumber);
            Assert.AreEqual(4, a.ImplicitHydrogenCount);
            Assert.AreEqual(1, a.FormalCharge);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestAtom_CH3_direct()
        {
            IAtom a = new Atom(6, 3);
            Assert.AreEqual("C", a.Symbol);
            Assert.AreEqual(6, a.AtomicNumber);
            Assert.AreEqual(3, a.ImplicitHydrogenCount);
            Assert.AreEqual(0, a.FormalCharge);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestAtom_Cl_direct()
        {
            IAtom a = new Atom(17);
            Assert.AreEqual("Cl", a.Symbol);
            Assert.AreEqual(17, a.AtomicNumber);
            Assert.AreEqual(0, a.ImplicitHydrogenCount);
            Assert.AreEqual(0, a.FormalCharge);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestAtom_NH4plus()
        {
            IAtom a = new Atom("NH4+");
            Assert.AreEqual("N", a.Symbol);
            Assert.AreEqual(7, a.AtomicNumber);
            Assert.AreEqual(4, a.ImplicitHydrogenCount);
            Assert.AreEqual(1, a.FormalCharge);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestAtom_Ominus()
        {
            IAtom a = new Atom("O-");
            Assert.AreEqual("O", a.Symbol);
            Assert.AreEqual(8, a.AtomicNumber);
            Assert.AreEqual(0, a.ImplicitHydrogenCount);
            Assert.AreEqual(-1, a.FormalCharge);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestAtom_Ca2plus()
        {
            IAtom a = new Atom("Ca+2");
            Assert.AreEqual("Ca", a.Symbol);
            Assert.AreEqual(20, a.AtomicNumber);
            Assert.AreEqual(0, a.ImplicitHydrogenCount);
            Assert.AreEqual(+2, a.FormalCharge);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestAtom_13CH3()
        {
            IAtom a = new Atom("13CH3");
            Assert.AreEqual("C", a.Symbol);
            Assert.AreEqual(13, a.MassNumber);
            Assert.AreEqual(6, a.AtomicNumber);
            Assert.AreEqual(3, a.ImplicitHydrogenCount);
            Assert.AreEqual(0, a.FormalCharge);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestAtom_String_Point3d()
        {
            Vector3 point3d = new Vector3(1.0, 2.0, 3.0);

            IAtom a = new Atom("C", point3d);
            Assert.AreEqual("C", a.Symbol);
            Assert.AreEqual(point3d, a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestAtom_String_Point2d()
        {
            Vector2 point2d = new Vector2(1.0, 2.0);

            IAtom a = new Atom("C", point2d);
            Assert.AreEqual("C", a.Symbol);
            Assert.AreEqual(point2d, a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
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

