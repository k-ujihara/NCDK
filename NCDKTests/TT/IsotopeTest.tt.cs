



/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *                    2012  Egon Willighagen <egonw@users.sf.net>
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
    /// Checks the functionality of the <see cref="Isotope"/>.
    /// </summary>
    [TestClass()]
    public class IsotopeTest : AbstractIsotopeTest
    {
        public override IChemObject NewChemObject()
        {
            return new Isotope(new Element());
        }

        [TestMethod()]
        public void TestIsotope_String()
        {
            IIsotope i = new Isotope("C");
            Assert.AreEqual("C", i.Symbol);
        }

        [TestMethod()]
        public void TestIsotope_IElement()
        {
            IElement element = NewChemObject().Builder.NewElement("C");
            IIsotope i = new Isotope(element);
            Assert.AreEqual("C", i.Symbol);
        }

        [TestMethod()]
        public void TestIsotope_int_String_int_double_double()
        {
            IIsotope i = new Isotope(6, "C", 12, 12.001, 80.0);
            Assert.AreEqual(12, i.MassNumber);
            Assert.AreEqual("C", i.Symbol);
            Assert.AreEqual(6, i.AtomicNumber.Value);
            Assert.AreEqual(12.001, i.ExactMass.Value, 0.001);
            Assert.AreEqual(80.0, i.NaturalAbundance.Value, 0.001);
        }

        [TestMethod()]
        public void TestIsotope_String_int()
        {
            IIsotope i = new Isotope("C", 12);
            Assert.AreEqual(12, i.MassNumber.Value);
            Assert.AreEqual("C", i.Symbol);
        }

        [TestMethod()]
        public void TestIsotope_int_String_double_double()
        {
            IIsotope i = new Isotope(6, "C", 12.001, 80.0);
            Assert.AreEqual("C", i.Symbol);
            Assert.AreEqual(6, i.AtomicNumber.Value);
            Assert.AreEqual(12.001, i.ExactMass.Value, 0.001);
            Assert.AreEqual(80.0, i.NaturalAbundance.Value, 0.001);
        }

        [TestMethod()]
        public void TestCompare_MassNumber()
        {
            Isotope iso = new Isotope("C") { MassNumber = 12 };
            Isotope iso2 = new Isotope("C") { MassNumber = (int)12.0 };
            Assert.IsTrue(iso.Compare(iso2));
        }

        [TestMethod()]
        public void TestCompare_ExactMass()
        {
            Isotope iso = new Isotope("C") { ExactMass = 12.000000 };
            Isotope iso2 = new Isotope("C") { ExactMass = 12.0 };
            Assert.IsTrue(iso.Compare(iso2));
        }

        [TestMethod()]
        public void TestCompare_NaturalAbundance()
        {
            Isotope iso = new Isotope("C") { NaturalAbundance = 12.000000 };
            Isotope iso2 = new Isotope("C") { NaturalAbundance = 12.0 };
            Assert.IsTrue(iso.Compare(iso2));
        }

    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="Isotope"/>.
    /// </summary>
    [TestClass()]
    public class IsotopeTest : AbstractIsotopeTest
    {
        public override IChemObject NewChemObject()
        {
            return new Isotope(new Element());
        }

        [TestMethod()]
        public void TestIsotope_String()
        {
            IIsotope i = new Isotope("C");
            Assert.AreEqual("C", i.Symbol);
        }

        [TestMethod()]
        public void TestIsotope_IElement()
        {
            IElement element = NewChemObject().Builder.NewElement("C");
            IIsotope i = new Isotope(element);
            Assert.AreEqual("C", i.Symbol);
        }

        [TestMethod()]
        public void TestIsotope_int_String_int_double_double()
        {
            IIsotope i = new Isotope(6, "C", 12, 12.001, 80.0);
            Assert.AreEqual(12, i.MassNumber);
            Assert.AreEqual("C", i.Symbol);
            Assert.AreEqual(6, i.AtomicNumber.Value);
            Assert.AreEqual(12.001, i.ExactMass.Value, 0.001);
            Assert.AreEqual(80.0, i.NaturalAbundance.Value, 0.001);
        }

        [TestMethod()]
        public void TestIsotope_String_int()
        {
            IIsotope i = new Isotope("C", 12);
            Assert.AreEqual(12, i.MassNumber.Value);
            Assert.AreEqual("C", i.Symbol);
        }

        [TestMethod()]
        public void TestIsotope_int_String_double_double()
        {
            IIsotope i = new Isotope(6, "C", 12.001, 80.0);
            Assert.AreEqual("C", i.Symbol);
            Assert.AreEqual(6, i.AtomicNumber.Value);
            Assert.AreEqual(12.001, i.ExactMass.Value, 0.001);
            Assert.AreEqual(80.0, i.NaturalAbundance.Value, 0.001);
        }

        [TestMethod()]
        public void TestCompare_MassNumber()
        {
            Isotope iso = new Isotope("C") { MassNumber = 12 };
            Isotope iso2 = new Isotope("C") { MassNumber = (int)12.0 };
            Assert.IsTrue(iso.Compare(iso2));
        }

        [TestMethod()]
        public void TestCompare_ExactMass()
        {
            Isotope iso = new Isotope("C") { ExactMass = 12.000000 };
            Isotope iso2 = new Isotope("C") { ExactMass = 12.0 };
            Assert.IsTrue(iso.Compare(iso2));
        }

        [TestMethod()]
        public void TestCompare_NaturalAbundance()
        {
            Isotope iso = new Isotope("C") { NaturalAbundance = 12.000000 };
            Isotope iso2 = new Isotope("C") { NaturalAbundance = 12.0 };
            Assert.IsTrue(iso.Compare(iso2));
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

