/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *                    2013  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Default;
using System;
using System.Linq;

namespace NCDK.Config
{
    /// <summary>
    /// Checks the functionality of the IsotopeFactory
    /// </summary>
    // @cdk.module test-core
    [TestClass()]
    public class IsotopesTest : CDKTestCase
    {
        [TestMethod()]
        public void TestGetInstance_IChemObjectBuilder()
        {
            Isotopes isofac = Isotopes.Instance;
            Assert.IsNotNull(isofac);
        }

        [TestMethod()]
        public void TestGetSize()
        {
            Isotopes isofac = Isotopes.Instance;
            Assert.IsTrue(isofac.Count > 0);
        }

        [TestMethod()]
        public void TestConfigure_IAtom()
        {
            Isotopes isofac = Isotopes.Instance;
            Atom atom = new Atom("H");
            isofac.Configure(atom);
            Assert.AreEqual(1, atom.AtomicNumber.Value);
        }

        [TestMethod()]
        public void TestConfigure_IAtom_IIsotope()
        {
            Isotopes isofac = Isotopes.Instance;
            Atom atom = new Atom("H");
            IIsotope isotope = new Default.Isotope("H", 2);
            isofac.Configure(atom, isotope);
            Assert.AreEqual(2, atom.MassNumber.Value);
        }

        [TestMethod()]
        public void TestGetMajorIsotope_String()
        {
            Isotopes isofac = Isotopes.Instance;
            IIsotope isotope = isofac.GetMajorIsotope("Te");
            Assert.AreEqual(129.9062244, isotope.ExactMass.Value, 0.0001);
        }

        [TestMethod()]
        public void TestGetMajorIsotope_int()
        {
            Isotopes isofac = Isotopes.Instance;
            IIsotope isotope = isofac.GetMajorIsotope(17);
            Assert.AreEqual("Cl", isotope.Symbol);
        }

        [TestMethod()]
        public void TestGetElement_String()
        {
            IsotopeFactory elfac = Isotopes.Instance;
            IElement element = elfac.GetElement("Br");
            Assert.AreEqual(35, element.AtomicNumber.Value);
        }

        [TestMethod()]
        public void TestGetElement_int()
        {
            IsotopeFactory elfac = Isotopes.Instance;
            IElement element = elfac.GetElement(6);
            Assert.AreEqual("C", element.Symbol);
        }

        [TestMethod()]
        public void TestGetElementSymbol_int()
        {
            IsotopeFactory elfac = Isotopes.Instance;
            string symbol = elfac.GetElementSymbol(8);
            Assert.AreEqual("O", symbol);
        }

        [TestMethod()]
        public void TestGetIsotopes_String()
        {
            Isotopes isofac = Isotopes.Instance;
            var list = isofac.GetIsotopes("He");
            Assert.AreEqual(8, list.Count());
        }

        [TestMethod()]
        public void TestGetIsotopes()
        {
            Isotopes isofac = Isotopes.Instance;
            var list = isofac.GetIsotopes();
            Assert.IsTrue(list.Count() > 200);
        }

        [TestMethod()]
        public void TestGetIsotopes_Double_double()
        {
            Isotopes isofac = Isotopes.Instance;
            var list = isofac.GetIsotopes(87.90, 0.01).ToList();
            //        should return:
            //        Isotope match: 88Sr has mass 87.9056121
            //        Isotope match: 88Y has mass 87.9095011
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(88, list[0].MassNumber.Value);
            Assert.AreEqual(88, list[1].MassNumber.Value);
        }

        [TestMethod()]
        public void TestIsElement_String()
        {
            Isotopes isofac = Isotopes.Instance;
            Assert.IsTrue(isofac.IsElement("C"));
        }

        [TestMethod()]
        public void TestConfigureAtoms_IAtomContainer()
        {
            AtomContainer container = new AtomContainer();
            container.Atoms.Add(new Atom("C"));
            container.Atoms.Add(new Atom("H"));
            container.Atoms.Add(new Atom("N"));
            container.Atoms.Add(new Atom("O"));
            container.Atoms.Add(new Atom("F"));
            container.Atoms.Add(new Atom("Cl"));
            Isotopes isofac = Isotopes.Instance;
            isofac.ConfigureAtoms(container);
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                Assert.IsTrue(0 < container.Atoms[i].AtomicNumber);
            }
        }

        [TestMethod()]
        public void TestGetNaturalMass_IElement()
        {
            Isotopes isofac = Isotopes.Instance;
            Assert.AreEqual(1.0079760, isofac.GetNaturalMass(new Default.Element("H")), 0.1);
        }

        [TestMethod()]
        public void TestGetIsotope()
        {
            Isotopes isofac = Isotopes.Instance;
            Assert.AreEqual(13.00335484, isofac.GetIsotope("C", 13).ExactMass.Value, 0.0000001);
        }

        /// <summary>
        /// Elements without a major isotope should return null.
        /// </summary>
        [TestMethod()]
        public void TestMajorUnstableIsotope()
        {
            Isotopes isotopes = Isotopes.Instance;
            Assert.IsNull(isotopes.GetMajorIsotope("Es"));
        }

        [TestMethod()]
        public void TestGetIsotope_NonElement()
        {
            Isotopes isofac = Isotopes.Instance;
            Assert.IsNull(isofac.GetIsotope("R", 13));
        }

        [TestMethod()]
        public void TestGetIsotopeFromExactMass()
        {
            Isotopes isofac = Isotopes.Instance;
            IIsotope carbon13 = isofac.GetIsotope("C", 13);
            IIsotope match = isofac.GetIsotope(carbon13.Symbol, carbon13.ExactMass.Value, 0.0001);
            Assert.IsNotNull(match);
            Assert.AreEqual(13, match.MassNumber.Value);
        }

        [TestMethod()]
        public void TestGetIsotopeFromExactMass_NonElement()
        {
            Isotopes isofac = Isotopes.Instance;
            IIsotope match = isofac.GetIsotope("R", 13.00001, 0.0001);
            Assert.IsNull(match);
        }

        [TestMethod()]
        public void TestYeahSure()
        {
            Isotopes isofac = Isotopes.Instance;
            IIsotope match = isofac.GetIsotope("H", 13.00001, 0.0001);
            Assert.IsNull(match);
        }

        [TestMethod()]
        public void TestGetIsotopeFromExactMass_LargeTolerance()
        {
            Isotopes isofac = Isotopes.Instance;
            IIsotope carbon13 = isofac.GetIsotope("C", 13);
            IIsotope match = isofac.GetIsotope(carbon13.Symbol, carbon13.ExactMass.Value, 2.0);
            Assert.IsNotNull(match);
            Assert.AreEqual(13, match.MassNumber.Value);
        }

        [TestMethod()]
        public void ConfigureDoesNotSetMajorIsotope()
        {
            IAtom atom = new Atom("CH4");
            Isotopes isotopes = Isotopes.Instance;
            IIsotope major = isotopes.GetMajorIsotope(atom.Symbol);
            Assert.IsNotNull(major);
            Assert.AreEqual(12, major.MassNumber);
            isotopes.Configure(atom);
            Assert.IsNull(atom.MassNumber);
        }

        // @cdk.bug 3534288
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNonexistingElement()
        {
            Isotopes isofac = Isotopes.Instance;
            IAtom xxAtom = new Atom("Xx");
            isofac.Configure(xxAtom);
        }

        [TestMethod()]
        public void TestGetIsotopes_Nonelement()
        {
            IsotopeFactory isofac = Isotopes.Instance;
            var list = isofac.GetIsotopes("E");
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count());
        }

        [TestMethod()]
        public void TestGetElement_Nonelement()
        {
            IsotopeFactory isofac = Isotopes.Instance;
            IElement element = isofac.GetElement("E");
            Assert.IsNull(element);
        }

        [TestMethod()]
        public void TestGetMajorIsotope_Nonelement()
        {
            IsotopeFactory isofac = Isotopes.Instance;
            IIsotope isotope = isofac.GetMajorIsotope("E");
            Assert.IsNull(isotope);
        }
    }
}
