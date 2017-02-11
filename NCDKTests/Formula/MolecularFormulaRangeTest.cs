/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
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
using System.Collections.Generic;

namespace NCDK.Formula
{
    /**
     * Checks the functionality of the MolecularFormulaRange.
     *
     * @cdk.module test-formula
     *
     * @see MolecularFormula
     */
    [TestClass()]
    public class MolecularFormulaRangeTest : CDKTestCase
    {

        private static readonly IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /**
         *  Constructor for the MolecularFormulaRangeTest object.
         *
         */
        public MolecularFormulaRangeTest()
            : base()
        { }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestMolecularFormulaRange()
        {

            MolecularFormulaRange mfRange = new MolecularFormulaRange();

            Assert.IsNotNull(mfRange);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetIsotopeCount()
        {

            MolecularFormulaRange mfRange = new MolecularFormulaRange();

            Assert.AreEqual(0, mfRange.Count);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestAddIsotope_IIsotope_int_int()
        {

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(builder.CreateIsotope("C"), 0, 10);
            mfRange.Add(builder.CreateIsotope("H"), 0, 10);

            Assert.AreEqual(2, mfRange.Count);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestAddIsotope2()
        {

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(builder.CreateIsotope("C"), 0, 10);
            mfRange.Add(builder.CreateIsotope("H"), 0, 10);

            IIsotope hy = builder.CreateIsotope("C");
            hy.NaturalAbundance = 2.00342342;
            mfRange.Add(hy, 0, 10);

            Assert.AreEqual(3, mfRange.Count);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetIsotopeCountMax_IIsotope()
        {
            MolecularFormulaRange mfRange = new MolecularFormulaRange();

            IIsotope carb = builder.CreateIsotope("C");
            IIsotope h1 = builder.CreateIsotope("H");
            mfRange.Add(carb, 0, 10);
            mfRange.Add(h1, 0, 10);

            Assert.AreEqual(2, mfRange.Count);
            Assert.AreEqual(10, mfRange.GetIsotopeCountMax(carb));
            Assert.AreEqual(10, mfRange.GetIsotopeCountMax(h1));
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetIsotopeCountMin_IIsotope()
        {
            MolecularFormulaRange mfRange = new MolecularFormulaRange();

            IIsotope carb = builder.CreateIsotope("C");
            IIsotope h1 = builder.CreateIsotope("H");
            IIsotope flu = builder.CreateIsotope("F");
            mfRange.Add(carb, 0, 10);
            mfRange.Add(h1, 0, 10);

            Assert.AreEqual(2, mfRange.Count);
            Assert.AreEqual(0, mfRange.GetIsotopeCountMin(carb));
            Assert.AreEqual(0, mfRange.GetIsotopeCountMin(h1));
            Assert.AreEqual(-1, mfRange.GetIsotopeCountMin(flu));
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetIsotopeCountMin_IIsotope2()
        {
            MolecularFormulaRange mfRange = new MolecularFormulaRange();

            IIsotope carb = builder.CreateIsotope("C");
            IIsotope h1 = builder.CreateIsotope("H");
            mfRange.Add(carb, 0, 10);
            mfRange.Add(h1, 0, 10);

            mfRange.Add(carb, 5, 10);
            mfRange.Add(h1, 5, 10);

            Assert.AreEqual(2, mfRange.Count);
            Assert.AreEqual(5, mfRange.GetIsotopeCountMin(carb));
            Assert.AreEqual(5, mfRange.GetIsotopeCountMin(h1));

        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetIsotopeCountMin_IIsotope3()
        {
            MolecularFormulaRange mfRange = new MolecularFormulaRange();

            IIsotope carb1 = builder.CreateIsotope("C");
            IIsotope h1 = builder.CreateIsotope("H");

            IIsotope carb2 = builder.CreateIsotope("C");
            IIsotope h2 = builder.CreateIsotope("H");

            mfRange.Add(carb1, 0, 10);
            mfRange.Add(h1, 0, 10);

            mfRange.Add(carb2, 5, 10);
            mfRange.Add(h2, 5, 10);

            Assert.AreEqual(2, mfRange.Count);
            Assert.AreEqual(5, mfRange.GetIsotopeCountMin(carb1));
            Assert.AreEqual(5, mfRange.GetIsotopeCountMin(h1));
            Assert.AreEqual(5, mfRange.GetIsotopeCountMin(carb2));
            Assert.AreEqual(5, mfRange.GetIsotopeCountMin(h2));

        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetIsotopeCountMin_IIsotope4()
        {
            MolecularFormulaRange mfRange = new MolecularFormulaRange();

            IIsotope carb1 = builder.CreateIsotope("C");
            IIsotope h1 = builder.CreateIsotope("H");

            IIsotope carb2 = builder.CreateIsotope("C");
            carb2.NaturalAbundance = 13.0876689;
            IIsotope h2 = builder.CreateIsotope("H");
            h2.NaturalAbundance = 2.0968768;

            mfRange.Add(carb1, 0, 10);
            mfRange.Add(h1, 0, 10);

            mfRange.Add(carb2, 5, 10);
            mfRange.Add(h2, 5, 10);

            Assert.AreEqual(4, mfRange.Count);
            Assert.AreEqual(0, mfRange.GetIsotopeCountMin(carb1));
            Assert.AreEqual(0, mfRange.GetIsotopeCountMin(h1));
            Assert.AreEqual(5, mfRange.GetIsotopeCountMin(carb2));
            Assert.AreEqual(5, mfRange.GetIsotopeCountMin(h2));

        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestIsotopes()
        {

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(builder.CreateIsotope("C"), 0, 10);
            mfRange.Add(builder.CreateIsotope("F"), 0, 10);

            IEnumerator<IIsotope> istoIter = mfRange.GetIsotopes().GetEnumerator();
            int counter = 0;
            while (istoIter.MoveNext())
            {
                counter++;
            }
            Assert.AreEqual(2, counter);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestContains_IIsotope()
        {
            MolecularFormulaRange mfRange = new MolecularFormulaRange();

            IIsotope carb = builder.CreateIsotope("C");
            IIsotope cl = builder.CreateIsotope("Cl");
            IIsotope h1 = builder.CreateIsotope("H");
            IIsotope h2 = builder.CreateIsotope("H");
            h2.ExactMass = 2.0004;

            mfRange.Add(carb, 0, 10);
            mfRange.Add(h1, 0, 10);

            Assert.IsTrue(mfRange.Contains(carb));
            Assert.IsFalse(mfRange.Contains(cl));
            Assert.IsTrue(mfRange.Contains(h1));
            Assert.IsFalse(mfRange.Contains(h2));
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        public void TestRemoveIsotope_IIsotope()
        {

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            IIsotope carb = builder.CreateIsotope("C");
            IIsotope flu = builder.CreateIsotope("F");
            IIsotope h1 = builder.CreateIsotope("H");
            mfRange.Add(carb, 0, 10);
            mfRange.Add(flu, 0, 10);
            mfRange.Add(h1, 0, 10);

            // remove the Fluorine
            mfRange.Remove(flu);

            Assert.AreEqual(2, mfRange.Count);
            Assert.AreEqual(0, mfRange.GetIsotopeCountMin(carb));
            Assert.AreEqual(-1, mfRange.GetIsotopeCountMin(flu));

        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestRemoveAllIsotopes()
        {
            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            IIsotope carb = builder.CreateIsotope("C");
            IIsotope flu = builder.CreateIsotope("F");
            IIsotope h1 = builder.CreateIsotope("H");
            mfRange.Add(carb, 0, 10);
            mfRange.Add(flu, 0, 10);
            mfRange.Add(h1, 0, 10);

            // remove the Fluorine
            mfRange.Clear();

            Assert.AreEqual(0, mfRange.Count);
            Assert.AreEqual(-1, mfRange.GetIsotopeCountMin(carb));
            Assert.AreEqual(-1, mfRange.GetIsotopeCountMin(h1));
            Assert.AreEqual(-1, mfRange.GetIsotopeCountMin(flu));

        }

        /**
         * A unit test suite for JUnit. Only test whether the
         * MolecularFormula are correctly cloned.
         *
         * @return    The test suite
        */
        [TestMethod()]
        public void TestClone()
        {
            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            object clone = mfRange.Clone();
            Assert.IsTrue(clone is MolecularFormulaRange);
            Assert.AreEqual(mfRange.Count, ((MolecularFormulaRange)clone).Count);

        }

        /**
         * A unit test suite for JUnit. Only test whether
         * the MolecularFormula are correctly cloned.
        */
        [TestMethod()]
        public void TestClone_Isotopes()
        {
            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            IIsotope carb = builder.CreateIsotope("C");
            IIsotope flu = builder.CreateIsotope("F");
            IIsotope h1 = builder.CreateIsotope("H");
            mfRange.Add(carb, 0, 5);
            mfRange.Add(flu, 2, 8);
            mfRange.Add(h1, 4, 10);

            Assert.AreEqual(3, mfRange.Count);
            Assert.AreEqual(0, mfRange.GetIsotopeCountMin(carb));
            Assert.AreEqual(2, mfRange.GetIsotopeCountMin(flu));
            Assert.AreEqual(4, mfRange.GetIsotopeCountMin(h1));
            Assert.AreEqual(5, mfRange.GetIsotopeCountMax(carb));
            Assert.AreEqual(8, mfRange.GetIsotopeCountMax(flu));
            Assert.AreEqual(10, mfRange.GetIsotopeCountMax(h1));

            object clone = mfRange.Clone();
            Assert.IsTrue(clone is MolecularFormulaRange);
            Assert.AreEqual(mfRange.Count, ((MolecularFormulaRange)clone).Count);

            Assert.AreEqual(3, ((MolecularFormulaRange)clone).Count);

            Assert.AreEqual(3, ((MolecularFormulaRange)clone).Count);
            Assert.AreEqual(0, ((MolecularFormulaRange)clone).GetIsotopeCountMin(carb));
            Assert.AreEqual(2, ((MolecularFormulaRange)clone).GetIsotopeCountMin(flu));
            Assert.AreEqual(4, ((MolecularFormulaRange)clone).GetIsotopeCountMin(h1));
            Assert.AreEqual(5, ((MolecularFormulaRange)clone).GetIsotopeCountMax(carb));
            Assert.AreEqual(8, ((MolecularFormulaRange)clone).GetIsotopeCountMax(flu));
            Assert.AreEqual(10, ((MolecularFormulaRange)clone).GetIsotopeCountMax(h1));
        }
    }
}
