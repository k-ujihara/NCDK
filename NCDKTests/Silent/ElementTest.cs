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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Silent
{
    /**
     * Checks the functionality of {@link Element}.
     *
     * @cdk.module test-silent
     */
    [TestClass()]
    public class ElementTest : AbstractElementTest
    {
        public override IChemObject NewChemObject()
        {
            return new Element();
        }

        [TestMethod()]
        public void TestElement()
        {
            IElement e = new Element();
            Assert.IsTrue(e is IChemObject);
        }

        [TestMethod()]
        public void TestElement_IElement()
        {
            IElement element = new Element();
            IElement e = element.Builder.CreateElement(element);
            Assert.IsTrue(e is IChemObject);
        }

        [TestMethod()]
        public void TestElement_String()
        {
            IElement e = new Element("C");
            Assert.AreEqual("C", e.Symbol);
        }

        [TestMethod()]
        public void TestElement_X()
        {
            IElement e = new Element("X");
            Assert.AreEqual("X", e.Symbol);
            // and it should not throw exceptions
            Assert.IsNotNull(e.AtomicNumber);
            Assert.AreEqual(0, e.AtomicNumber);
        }

        [TestMethod()]
        public void TestElement_String_Integer()
        {
            IElement e = new Element("H", 1);
            Assert.AreEqual("H", e.Symbol);
            Assert.AreEqual(1, e.AtomicNumber);
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
        public void CompareSymbol()
        {
            Element e1 = new Element("H", 1);
            Element e2 = new Element("H", 1);
            Assert.IsTrue(e1.Compare(e2));
        }

        [TestMethod()]
        public void CompareAtomicNumber()
        {
            Element e1 = new Element("H", 1);
            Element e2 = new Element("H", 1);
            Assert.IsTrue(e1.Compare(e2));
        }

        [TestMethod()]
        public void CompareDiffSymbol()
        {
            Element e1 = new Element("H", 1);
            Element e2 = new Element("C", 12);
            Assert.IsFalse(e1.Compare(e2));
        }

        [TestMethod()]
        public void CompareDiffAtomicNumber()
        {
            Element e1 = new Element("H", 1);
            Element e2 = new Element("H", null);
            Assert.IsFalse(e1.Compare(e2));
        }
    }
}
