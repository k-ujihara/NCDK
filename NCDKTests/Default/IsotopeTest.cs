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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
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
    /// Checks the functionality of the Isotope class.
    ///
    // @cdk.module test-data
    ///
    // @see org.openscience.cdk.Isotope
    /// </summary>
    [TestClass()]
    public class IsotopeTest
        : AbstractIsotopeTest
    {
        public override IChemObject NewChemObject()
        {
            return new Isotope("C");
        }
        [TestMethod()]
        public virtual void TestIsotope_String()
        {
            IIsotope i = new Isotope("C");
            Assert.AreEqual("C", i.Symbol);
        }

        [TestMethod()]
        public virtual void TestIsotope_IElement()
        {
            IElement element = new Element("C");
            IIsotope i = new Isotope(element);
            Assert.AreEqual("C", i.Symbol);
        }

        [TestMethod()]
        public virtual void TestIsotope_int_String_int_Double_double()
        {
            IIsotope i = new Isotope(6, "C", 12, 12.001, 80.0);
            Assert.AreEqual(12, i.MassNumber.Value);
            Assert.AreEqual("C", i.Symbol);
            Assert.AreEqual(6, i.AtomicNumber.Value);
            Assert.AreEqual(12.001, i.ExactMass.Value, 0.001);
            Assert.AreEqual(80.0, i.NaturalAbundance.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestIsotope_String_int()
        {
            IIsotope i = new Isotope("C", 12);
            Assert.AreEqual(12, i.MassNumber.Value);
            Assert.AreEqual("C", i.Symbol);
        }

        [TestMethod()]
        public virtual void TestIsotope_int_String_Double_double()
        {
            IIsotope i = new Isotope(6, "C", 12.001, 80.0);
            Assert.AreEqual("C", i.Symbol);
            Assert.AreEqual(6, i.AtomicNumber.Value);
            Assert.AreEqual(12.001, i.ExactMass.Value, 0.001);
            Assert.AreEqual(80.0, i.NaturalAbundance.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestCompare_MassNumber()
        {
            Isotope iso = new Isotope("C");
            iso.MassNumber = 12;
            Isotope iso2 = new Isotope("C");
            iso2.MassNumber = (int)12.0;
            Assert.IsTrue(iso.Compare(iso2));
        }

        [TestMethod()]
        public virtual void TestCompare_MassNumberIntegers()
        {
            Isotope iso = new Isotope("C");
            iso.MassNumber = new int?(12);
            Isotope iso2 = new Isotope("C");
            iso2.MassNumber = new int?(12);
            Assert.IsTrue(iso.Compare(iso2));
        }

        [TestMethod()]
        public virtual void TestCompare_MassNumberIntegers_ValueOf()
        {
            Isotope iso = new Isotope("C");
            iso.MassNumber = 12;
            Isotope iso2 = new Isotope("C");
            iso2.MassNumber = 12;
            Assert.IsTrue(iso.Compare(iso2));
        }

        [TestMethod()]
        public virtual void TestCompare_ExactMass()
        {
            Isotope iso = new Isotope("C");
            iso.ExactMass = 12.000000;
            Isotope iso2 = new Isotope("C");
            iso2.ExactMass = 12.0;
            Assert.IsTrue(iso.Compare(iso2));
        }

        [TestMethod()]
        public virtual void TestCompare_NaturalAbundance()
        {
            Isotope iso = new Isotope("C");
            iso.NaturalAbundance = 12.000000;
            Isotope iso2 = new Isotope("C");
            iso2.NaturalAbundance = 12.0;
            Assert.IsTrue(iso.Compare(iso2));
        }
    }

}