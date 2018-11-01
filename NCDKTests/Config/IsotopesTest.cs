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
using NCDK.Silent;
using System;
using System.Linq;

namespace NCDK.Config
{
    /// <summary>
    /// Checks the functionality of the <see cref="IsotopeFactory"/>.
    /// </summary>
    // @cdk.module test-core
    [TestClass()]
    public class IsotopesTest : CDKTestCase
    {
        [TestMethod()]
        public void TestGetInstanceIChemObjectBuilder()
        {
            var isofac = BODRIsotopeFactory.Instance;
            Assert.IsNotNull(isofac);
        }

        [TestMethod()]
        public void TestGetSize()
        {
            var isofac = BODRIsotopeFactory.Instance;
            Assert.IsTrue(isofac.Count > 0);
        }

        [TestMethod()]
        public void TestConfigureIAtom()
        {
            var isofac = BODRIsotopeFactory.Instance;
            Atom atom = new Atom("H");
            isofac.Configure(atom);
            Assert.AreEqual(1, atom.AtomicNumber.Value);
        }

        [TestMethod()]
        public void TestConfigureIAtomIIsotope()
        {
            var isofac = BODRIsotopeFactory.Instance;
            Atom atom = new Atom("H");
            IIsotope isotope = new Default.Isotope("H", 2);
            isofac.Configure(atom, isotope);
            Assert.AreEqual(2, atom.MassNumber.Value);
        }

        [TestMethod()]
        public void TestGetMajorIsotopeString()
        {
            var isofac = BODRIsotopeFactory.Instance;
            IIsotope isotope = isofac.GetMajorIsotope("Te");
            Assert.AreEqual(129.9062244, isotope.ExactMass.Value, 0.0001);
        }

        [TestMethod()]
        public void TestGetMajorIsotopeInt()
        {
            var isofac = BODRIsotopeFactory.Instance;
            IIsotope isotope = isofac.GetMajorIsotope(17);
            Assert.AreEqual("Cl", isotope.Symbol);
        }

        [TestMethod()]
        public void TestGetElementString()
        {
            IsotopeFactory elfac = BODRIsotopeFactory.Instance;
            IElement element = elfac.GetElement("Br");
            Assert.AreEqual(35, element.AtomicNumber.Value);
        }

        [TestMethod()]
        public void TestGetElementInt()
        {
            IsotopeFactory elfac = BODRIsotopeFactory.Instance;
            IElement element = elfac.GetElement(6);
            Assert.AreEqual("C", element.Symbol);
        }

        [TestMethod()]
        public void TestGetElementSymbolInt()
        {
            IsotopeFactory elfac = BODRIsotopeFactory.Instance;
            string symbol = elfac.GetElementSymbol(8);
            Assert.AreEqual("O", symbol);
        }

        [TestMethod()]
        public void TestGetIsotopesString()
        {
            var isofac = BODRIsotopeFactory.Instance;
            var list = isofac.GetIsotopes("He");
            Assert.AreEqual(8, list.Count());
        }

        [TestMethod()]
        public void TestGetIsotopes()
        {
            var isofac = BODRIsotopeFactory.Instance;
            var list = isofac.GetIsotopes();
            Assert.IsTrue(list.Count() > 200);
        }

        [TestMethod()]
        public void TestGetIsotopesDoubleDouble()
        {
            var isofac = BODRIsotopeFactory.Instance;
            var list = isofac.GetIsotopes(87.90, 0.01).ToReadOnlyList();
            //        should return:
            //        Isotope match: 88Sr has mass 87.9056121
            //        Isotope match: 88Y has mass 87.9095011
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(88, list[0].MassNumber.Value);
            Assert.AreEqual(88, list[1].MassNumber.Value);
        }

        [TestMethod()]
        public void TestIsElementString()
        {
            var isofac = BODRIsotopeFactory.Instance;
            Assert.IsTrue(isofac.IsElement("C"));
        }

        [TestMethod()]
        public void TestConfigureAtomsIAtomContainer()
        {
            AtomContainer container = new AtomContainer();
            container.Atoms.Add(new Atom("C"));
            container.Atoms.Add(new Atom("H"));
            container.Atoms.Add(new Atom("N"));
            container.Atoms.Add(new Atom("O"));
            container.Atoms.Add(new Atom("F"));
            container.Atoms.Add(new Atom("Cl"));
            var isofac = BODRIsotopeFactory.Instance;
            isofac.ConfigureAtoms(container);
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                Assert.IsTrue(0 < container.Atoms[i].AtomicNumber);
            }
        }

        [TestMethod()]
        public void TestGetNaturalMassIElement()
        {
            var isofac = BODRIsotopeFactory.Instance;
            Assert.AreEqual(1.0079760, isofac.GetNaturalMass(new Default.Element("H")), 0.1);
        }

        [TestMethod()]
        public void TestGetIsotope()
        {
            var isofac = BODRIsotopeFactory.Instance;
            Assert.AreEqual(13.00335484, isofac.GetIsotope("C", 13).ExactMass.Value, 0.0000001);
        }

        /// <summary>
        /// Elements without a major isotope should return null.
        /// </summary>
        [TestMethod()]
        public void TestMajorUnstableIsotope()
        {
            var isotopes = BODRIsotopeFactory.Instance;
            Assert.IsNull(isotopes.GetMajorIsotope("Es"));
        }

        [TestMethod()]
        public void TestGetIsotopeNonElement()
        {
            var isofac = BODRIsotopeFactory.Instance;
            Assert.IsNull(isofac.GetIsotope("R", 13));
        }

        [TestMethod()]
        public void TestGetIsotopeFromExactMass()
        {
            var isofac = BODRIsotopeFactory.Instance;
            IIsotope carbon13 = isofac.GetIsotope("C", 13);
            IIsotope match = isofac.GetIsotope(carbon13.Symbol, carbon13.ExactMass.Value, 0.0001);
            Assert.IsNotNull(match);
            Assert.AreEqual(13, match.MassNumber.Value);
        }

        [TestMethod()]
        public void TestGetIsotopeFromExactMassNonElement()
        {
            var isofac = BODRIsotopeFactory.Instance;
            IIsotope match = isofac.GetIsotope("R", 13.00001, 0.0001);
            Assert.IsNull(match);
        }

        [TestMethod()]
        public void TestYeahSure()
        {
            var isofac = BODRIsotopeFactory.Instance;
            IIsotope match = isofac.GetIsotope("H", 13.00001, 0.0001);
            Assert.IsNull(match);
        }

        [TestMethod()]
        public void TestGetIsotopeFromExactMassLargeTolerance()
        {
            var isofac = BODRIsotopeFactory.Instance;
            IIsotope carbon13 = isofac.GetIsotope("C", 13);
            IIsotope match = isofac.GetIsotope(carbon13.Symbol, carbon13.ExactMass.Value, 2.0);
            Assert.IsNotNull(match);
            Assert.AreEqual(13, match.MassNumber.Value);
        }

        [TestMethod()]
        public void ConfigureDoesNotSetMajorIsotope()
        {
            IAtom atom = new Atom("CH4");
            var isotopes = BODRIsotopeFactory.Instance;
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
            var isofac = BODRIsotopeFactory.Instance;
            IAtom xxAtom = new Atom("Xx");
            isofac.Configure(xxAtom);
        }

        [TestMethod()]
        public void TestGetIsotopesNonelement()
        {
            IsotopeFactory isofac = BODRIsotopeFactory.Instance;
            var list = isofac.GetIsotopes("E");
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count());
        }

        [TestMethod()]
        public void TestGetElementNonelement()
        {
            IsotopeFactory isofac = BODRIsotopeFactory.Instance;
            IElement element = isofac.GetElement("E");
            Assert.IsNull(element);
        }

        [TestMethod()]
        public void TestGetMajorIsotopeNonelement()
        {
            IsotopeFactory isofac = BODRIsotopeFactory.Instance;
            IIsotope isotope = isofac.GetMajorIsotope("E");
            Assert.IsNull(isotope);
        }
    }
}
