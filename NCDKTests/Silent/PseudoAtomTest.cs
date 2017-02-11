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
    /**
     * Checks the functionality of the {@link PseudoAtom}.
     *
     * @cdk.module test-silent
     */
    [TestClass()]
    public class PseudoAtomTest : AbstractPseudoAtomTest
    {
        public override IChemObject NewChemObject()
        {
            return new PseudoAtom();
        }

        [TestMethod()]
        public void TestPseudoAtom()
        {
            IPseudoAtom a = new PseudoAtom();
            Assert.AreEqual("R", a.Symbol);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestPseudoAtom_IElement()
        {
            IElement element = NewChemObject().Builder.CreateElement();
            IPseudoAtom a = new PseudoAtom(element);
            Assert.AreEqual("R", a.Symbol);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public override void TestPseudoAtom_IAtom()
        {
            IAtom element = NewChemObject().Builder.CreateAtom("C");
            IPseudoAtom a = new PseudoAtom(element);
            Assert.AreEqual("R", a.Symbol);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestPseudoAtom_String()
        {
            string label = "Arg255";
            IPseudoAtom a = new PseudoAtom(label);
            Assert.AreEqual("R", a.Symbol);
            Assert.AreEqual(label, a.Label);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.Point2D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestPseudoAtom_String_Point2d()
        {
            Vector2 point = new Vector2(1.0, 2.0);
            string label = "Arg255";
            IPseudoAtom a = new PseudoAtom(label, point);
            Assert.AreEqual("R", a.Symbol);
            Assert.AreEqual(label, a.Label);
            Assert.AreEqual(point, a.Point2D);
            Assert.IsNull(a.Point3D);
            Assert.IsNull(a.FractionalPoint3D);
        }

        [TestMethod()]
        public void TestPseudoAtom_String_Point3d()
        {
            Vector3 point = new Vector3(1.0, 2.0, 3.0);
            string label = "Arg255";
            IPseudoAtom a = new PseudoAtom(label, point);
            Assert.AreEqual("R", a.Symbol);
            Assert.AreEqual(label, a.Label);
            Assert.AreEqual(point, a.Point3D);
            Assert.IsNull(a.Point2D);
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
