/* Copyright (C) 2014  Tomas Pluskal <plusik@gmail.com>
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
using NCDK.Config;
using NCDK.Tools.Manipulator;
using System;
using System.Linq;
using System.Threading;

namespace NCDK.Formula
{
    /**
	 * Checks the functionality of the MolecularFormulaGenerator.
	 *
	 * @cdk.module test-formula
	 */
    [TestClass()]
    public class MolecularFormulaGeneratorTest : CDKTestCase
    {

        private readonly IChemObjectBuilder builder = Silent.ChemObjectBuilder
                .Instance;

        /**
		 * Test the GetNextFormula() method
		 */
        [TestMethod()]
        public void TestGetNextFormula()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 10);
            mfRange.Add(h, 0, 10);
            mfRange.Add(o, 0, 10);
            mfRange.Add(n, 0, 10);

            double minMass = 100.0;
            double maxMass = 100.05;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormula f = gen.GetNextFormula();
            Assert.IsNotNull(f);

        }

        /**
		 * Test the GetAllFormulas() method
		 */
        [TestMethod()]
        public void TestGetAllFormulas()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 10);
            mfRange.Add(h, 0, 10);
            mfRange.Add(o, 0, 10);
            mfRange.Add(n, 0, 10);

            double minMass = 100.0;
            double maxMass = 100.05;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreNotEqual(0, mfSet.Count);
        }

        /**
		 * Test the GetFinishedPercentage() method
		 */
        [TestMethod()]
        public void TestGetFinishedPercentage()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 10);
            mfRange.Add(h, 0, 10);
            mfRange.Add(o, 0, 10);
            mfRange.Add(n, 0, 10);

            double minMass = 100.0;
            double maxMass = 100.05;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);

            double finishedPerc, lastFinishedPerc = 0d;

            // The initial value must be 0
            finishedPerc = gen.GetFinishedPercentage();
            Assert.AreEqual(0d, finishedPerc, 0.0001);

            // The value must increase after each generated formula
            while (gen.GetNextFormula() != null)
            {
                finishedPerc = gen.GetFinishedPercentage();
                Assert.IsTrue(finishedPerc > lastFinishedPerc);
                lastFinishedPerc = finishedPerc;
            }

            // The final value must be 1
            finishedPerc = gen.GetFinishedPercentage();
            Assert.AreEqual(1d, finishedPerc, 0.0001);

        }


        /**
		 * Test the Cancel() method called from another thread. This test must
		 * finish in 100 ms.
		 */
        [TestMethod()]
        [Timeout(100)]
        public void TestCancel()
        {
            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");
            IIsotope p = ifac.GetMajorIsotope("P");
            IIsotope s = ifac.GetMajorIsotope("S");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 1000);
            mfRange.Add(h, 0, 1000);
            mfRange.Add(o, 0, 1000);
            mfRange.Add(n, 0, 1000);
            mfRange.Add(p, 0, 1000);
            mfRange.Add(s, 0, 1000);

            double minMass = 100000.0;
            double maxMass = 100000.001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(
                   builder, minMass, maxMass, mfRange);

            var cancelThread = new ThreadStart(() => {
				Thread.Sleep(5);
				gen.Cancel();
            });
            new Thread(cancelThread).Start();

			// We will get stuck in the next method call until the cancel thread
			// calls the Cancel() method
			gen.GetAllFormulas();

            // Next GetNextFormula() call should return null
            IMolecularFormula f = gen.GetNextFormula();
			Assert.IsNull(f);
        }

        /**
		 * Test empty molecular formula range
		 *
		 */
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEmptyMFRange()
        {
            new MolecularFormulaGenerator(builder, 0, 100,
                    new MolecularFormulaRange());
        }

        /**
		 * Test negative mass
		 */
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestNegativeMass()
        {
            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 100);
            new MolecularFormulaGenerator(builder, -20, -10,
                    new MolecularFormulaRange());
        }

        /**
		 * Test if the generator respects minimal element counts
		 *
		 */
        [TestMethod()]
        public void TestMinCounts()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 5, 20);
            mfRange.Add(h, 5, 20);
            mfRange.Add(o, 5, 20);
            mfRange.Add(n, 5, 20);

            // The minimal formula MF=C5H5O5N5 MW=215.0290682825
            double minMass = 100;
            double maxMass = 250;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            // Check that all element counts in the formula are >= 5
            foreach (var f in mfSet)
            {
                foreach (var i in f.Isotopes)
                {
                    int count = f.GetCount(i);
                    Assert.IsTrue(count >= 5);
                }
            }

        }

        /**
		 * Test if the generator respects maximal element counts
		 *
		 */
        [TestMethod()]
        public void TestMaxCounts()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 3, 7);
            mfRange.Add(h, 3, 7);
            mfRange.Add(o, 3, 7);
            mfRange.Add(n, 3, 7);

            // The maximal formula MF=C7H7O7N7 MW=301.0406955954
            double minMass = 250;
            double maxMass = 400;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            // Check that all element counts in the formula are <= 7
            foreach (var f in mfSet)
            {
                foreach (var i in f.Isotopes)
                {
                    int count = f.GetCount(i);
                    Assert.IsTrue(count <= 7);
                }
            }
        }

        /**
		 * Test to find a single carbon.
		 */
        [TestMethod()]
        public void TestSingleCarbon()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 100);

            double minMass = 5;
            double maxMass = 15;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C", MolecularFormulaManipulator.GetString(mfSet[0]));
        }

        /**
		 * Test to find MF=C10000, MW=120000.0 using only carbons.
		 */
        [TestMethod()]
        public void TestCarbons()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 100000);

            double minMass = 120000.0 - 1;
            double maxMass = 120000.0 + 1;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C10000", MolecularFormulaManipulator
                    .GetString(mfSet[0]));
        }

        /**
		 * Test to find H2O in a range of 1-20.
		 */
        [TestMethod()]
        public void TestWater()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");
            IIsotope p = ifac.GetMajorIsotope("P");
            IIsotope s = ifac.GetMajorIsotope("S");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 10);
            mfRange.Add(h, 0, 10);
            mfRange.Add(o, 0, 10);
            mfRange.Add(n, 0, 10);
            mfRange.Add(p, 0, 10);
            mfRange.Add(s, 0, 10);

            double minMass = 1;
            double maxMass = 20;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);

            bool found = false;
            foreach (IMolecularFormula formula in mfSet)
            {
                string mf = MolecularFormulaManipulator.GetString(formula);
                if (mf.Equals("H2O"))
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "The molecular formula H2O should be found");
        }

        /**
		 * Test to find MF=C5H11N2O, MW=115.08714
		 */
        [TestMethod()]
        public void TestSmallMass()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 20);
            mfRange.Add(h, 0, 30);
            mfRange.Add(o, 0, 15);
            mfRange.Add(n, 0, 10);

            double minMass = 115.08714 - 0.0001;
            double maxMass = 115.08714 + 0.0001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C5H11N2O", MolecularFormulaManipulator
                    .GetString(mfSet[0]));
        }

        /**
		 * Test to find pentacarboxyporphyrin, MF=C37H38N4O10 MW=698.25879
		 * 
		 */
        [TestMethod()]
        public void TestMiddleMass()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 50);
            mfRange.Add(h, 0, 100);
            mfRange.Add(o, 0, 30);
            mfRange.Add(n, 0, 10);

            double minMass = 698.25879 - 0.0001;
            double maxMass = 698.25879 + 0.0001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C37H38N4O10", MolecularFormulaManipulator
                    .GetString(mfSet[0]));
        }

        /**
		 * Test to find ubiquitin: MF=C374H623N103O116S MW=8445.573784
		 *
		 */
        [TestMethod()]
        public void TestHighMass()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");
            IIsotope s = ifac.GetMajorIsotope("S");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 350, 400);
            mfRange.Add(h, 620, 650);
            mfRange.Add(o, 100, 150);
            mfRange.Add(n, 100, 150);
            mfRange.Add(s, 0, 10);

            double minMass = 8445.573784 - 0.00001;
            double maxMass = 8445.573784 + 0.00001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C374H623N103O116S", MolecularFormulaManipulator
                    .GetString(mfSet[0]));
        }

        /**
		 * 
		 *
		 * Test if formula MF=C4H11NO4 MW=137.06881 is found in mass range
		 * 137-137.2.
		 *
		 */
        [TestMethod()]
        public void TestFormulaFoundInRange()
        {
            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 1, 50);
            mfRange.Add(h, 1, 100);
            mfRange.Add(o, 1, 50);
            mfRange.Add(n, 1, 50);

            double minMass = 137.0;
            double maxMass = 137.2;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.AreEqual(48, mfSet.Count);
            bool found = false;
            foreach (var formula in mfSet)
            {
                string mf = MolecularFormulaManipulator.GetString(formula);
                if (mf.Equals("C4H11NO4"))
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "The molecular formula C4H11NO4 should be found");
        }

        /**
		 * Test if formula MF=C11H10NO2 MW=188.07115 is found in mass range 187-189.
		 *
		 */
        [TestMethod()]
        public void TestFormulaFoundInRange2()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 1, 50);
            mfRange.Add(h, 1, 100);
            mfRange.Add(o, 1, 50);
            mfRange.Add(n, 1, 50);

            double minMass = 187;
            double maxMass = 189;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.AreEqual(528, mfSet.Count);
            bool found = false;
            foreach (var formula in mfSet)
            {
                string mf = MolecularFormulaManipulator.GetString(formula);
                if (mf.Equals("C11H10NO2"))
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "The molecular formula C11H10NO2 should be found");
        }

        /**
		 * Test if formula with 7 different elements is found in a narrow mass
		 * range. MF=C8H9Cl3NO2PS MW=318.915719
		 *
		 */
        [TestMethod()]
        public void TestCompoundWith7Elements()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");
            IIsotope s = ifac.GetMajorIsotope("S");
            IIsotope p = ifac.GetMajorIsotope("P");
            IIsotope cl = ifac.GetMajorIsotope("Cl");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 7, 9);
            mfRange.Add(h, 8, 10);
            mfRange.Add(o, 1, 3);
            mfRange.Add(n, 0, 2);
            mfRange.Add(s, 0, 2);
            mfRange.Add(p, 0, 2);
            mfRange.Add(cl, 2, 4);

            double minMass = 318.915719 - 0.0001;
            double maxMass = 318.915719 + 0.0001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C8H9Cl3NO2PS", MolecularFormulaManipulator
                    .GetString(mfSet[0]));

        }

        /**
		 * Test if C13 isotope-containing formula is found. MF=C(^12)3C(^13)H5
		 */
        [TestMethod()]
        public void TestDifferentIsotopes()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            var carbons = ifac.GetIsotopes("C");
            IIsotope c13 = carbons.ElementAt(5); // 13
            IIsotope h = ifac.GetMajorIsotope("H");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 11);
            mfRange.Add(c13, 0, 10);
            mfRange.Add(h, 0, 10);

            double minMass = 54.04193 - 0.001;
            double maxMass = 54.04193 + 0.001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);

            IMolecularFormula trueFormula = new MolecularFormula(); // C3CH5
            trueFormula.Add(c, 3);
            trueFormula.Add(c13, 1);
            trueFormula.Add(h, 5);

            Assert.AreEqual(trueFormula.Count, mfSet
                    [0].Count);
            Assert.AreEqual(trueFormula.GetCount(c), mfSet
                    [0].GetCount(c));
            Assert.AreEqual(trueFormula.GetCount(c13), mfSet
                    [0].GetCount(c13));

        }

        /**
		 * Test if formula MF=C7H15N2O4 MW=191.10318 is found properly if we fix the
		 * element counts
		 */
        [TestMethod()]
        public void TestFixedElementCounts()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 7, 7);
            mfRange.Add(h, 15, 15);
            mfRange.Add(o, 4, 4);
            mfRange.Add(n, 2, 2);

            double massMin = 10d;
            double massMax = 1000d;
            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    massMin, massMax, mfRange);

            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C7H15N2O4", MolecularFormulaManipulator
                    .GetString(mfSet[0]));

        }

        /**
		 * Test if zero results are returned in case the target mass range is too
		 * high
		 */
        [TestMethod()]
        public void TestMassRangeTooHigh()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 0, 10);
            mfRange.Add(h, 0, 10);
            mfRange.Add(o, 0, 10);
            mfRange.Add(n, 0, 10);

            double massMin = 1000d;
            double massMax = 2000d;
            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    massMin, massMax, mfRange);

            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(0, mfSet.Count);

        }

        /**
         * Test if zero results are returned in case the target mass range is too
         * low
         */
        [TestMethod()]
        public void TestMassRangeTooLow()
        {

            IsotopeFactory ifac = Isotopes.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.Add(c, 100, 200);
            mfRange.Add(h, 100, 200);
            mfRange.Add(o, 100, 200);
            mfRange.Add(n, 100, 200);

            double massMin = 50d;
            double massMax = 100d;
            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder,
                    massMin, massMax, mfRange);

            IMolecularFormulaSet mfSet = gen.GetAllFormulas();
            Assert.IsNotNull(mfSet);
            Assert.AreEqual(0, mfSet.Count);
        }
    }
}
