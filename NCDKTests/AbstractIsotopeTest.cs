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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Diff.Tree;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of the Isotope class.
    ///
    // @cdk.module test-interfaces
    ///
    // @see org.openscience.cdk.Isotope
    /// </summary>
    [TestClass()]
    public abstract class AbstractIsotopeTest : AbstractElementTest
    {
        [TestMethod()]
        public virtual void TestSetNaturalAbundance_Double()
        {
            IIsotope i = (IIsotope)NewChemObject();
            i.NaturalAbundance = 80.0;
            Assert.AreEqual(80.0, i.NaturalAbundance.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestGetNaturalAbundance()
        {
            TestSetNaturalAbundance_Double();
        }

        [TestMethod()]
        public virtual void TestSetExactMass_Double()
        {
            IIsotope i = (IIsotope)NewChemObject();
            i.ExactMass = 12.03;
            Assert.AreEqual(12.03, i.ExactMass.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestGetExactMass()
        {
            TestSetExactMass_Double();
        }

        [TestMethod()]
        public virtual void TestSetMassNumber_Integer()
        {
            IIsotope i = (IIsotope)NewChemObject();
            i.MassNumber = 2;
            Assert.AreEqual(2, i.MassNumber.Value);
        }

        [TestMethod()]
        public virtual void TestGetMassNumber()
        {
            TestSetMassNumber_Integer();
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]

        public override void TestClone()
        {
            IIsotope iso = (IIsotope)NewChemObject();
            object clone = iso.Clone();
            Assert.IsTrue(clone is IIsotope);

            // test that everything has been cloned properly
            string diff = IsotopeDiff.Diff(iso, (IIsotope)clone);
            Assert.IsNotNull(diff);
            Assert.AreEqual(0, diff.Length);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public virtual void TestClone_ExactMass()
        {
            IIsotope iso = (IIsotope)NewChemObject();
            iso.ExactMass = 1.0;
            IIsotope clone = (IIsotope)iso.Clone();

            // test cloning of exact mass
            iso.ExactMass = 2.0;
            Assert.AreEqual(1.0, clone.ExactMass.Value, 0.001);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public virtual void TestClone_NaturalAbundance()
        {
            IIsotope iso = (IIsotope)NewChemObject();
            iso.NaturalAbundance = 1.0;
            IIsotope clone = (IIsotope)iso.Clone();

            // test cloning of exact mass
            iso.NaturalAbundance = 2.0;
            Assert.AreEqual(1.0, clone.NaturalAbundance.Value, 0.001);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public virtual void TestClone_MassNumber()
        {
            IIsotope iso = (IIsotope)NewChemObject();
            iso.MassNumber = 12;
            IIsotope clone = (IIsotope)iso.Clone();

            // test cloning of exact mass
            iso.MassNumber = 13;
            Assert.AreEqual(12, clone.MassNumber.Value);
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]
        public override void TestToString()
        {
            IIsotope iso = (IIsotope)NewChemObject();
            string description = iso.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

    }
}