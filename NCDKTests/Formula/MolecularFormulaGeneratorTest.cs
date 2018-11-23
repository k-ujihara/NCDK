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
    /// <summary>
    /// Checks the functionality of the MolecularFormulaGenerator.
    /// </summary>
    // @cdk.module test-formula
    [TestClass()]
    public class MolecularFormulaGeneratorTest : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;

        /// <summary>
        /// Test the GetNextFormula() method
        /// </summary>
        [TestMethod()]
        public void TestGetNextFormula()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 10);
            mfRange.AddIsotope(h, 0, 10);
            mfRange.AddIsotope(o, 0, 10);
            mfRange.AddIsotope(n, 0, 10);

            double minMass = 100.0;
            double maxMass = 100.05;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
            IMolecularFormula f = gen.GetNextFormula();
            Assert.IsNotNull(f);
        }

        /// <summary>
        /// Test the GetAllFormulas() method
        /// </summary>
        [TestMethod()]
        public void TestGetAllFormulas()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 10);
            mfRange.AddIsotope(h, 0, 10);
            mfRange.AddIsotope(o, 0, 10);
            mfRange.AddIsotope(n, 0, 10);

            double minMass = 100.0;
            double maxMass = 100.05;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreNotEqual(0, mfSet.Count);
        }

        /// <summary>
        /// Test the GetFinishedPercentage() method
        /// </summary>
        [TestMethod()]
        public void TestGetFinishedPercentage()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 10);
            mfRange.AddIsotope(h, 0, 10);
            mfRange.AddIsotope(o, 0, 10);
            mfRange.AddIsotope(n, 0, 10);

            double minMass = 100.0;
            double maxMass = 100.05;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);

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


        /// <summary>
        /// Test the Cancel() method called from another thread. This test must
        /// finish in 1000 ms.
        /// </summary>
        [TestMethod()]
        [Timeout(1000)]
        public void TestCancel()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");
            IIsotope p = ifac.GetMajorIsotope("P");
            IIsotope s = ifac.GetMajorIsotope("S");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 1000);
            mfRange.AddIsotope(h, 0, 1000);
            mfRange.AddIsotope(o, 0, 1000);
            mfRange.AddIsotope(n, 0, 1000);
            mfRange.AddIsotope(p, 0, 1000);
            mfRange.AddIsotope(s, 0, 1000);

            double minMass = 100000.0;
            double maxMass = 100000.001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);

            var cancelThread = new ThreadStart(() => {
                Thread.Sleep(5);
                gen.Cancel();
            });
            new Thread(cancelThread).Start();

            Thread.Sleep(10);
            // We will get stuck in the next method call until the cancel thread
            // calls the Cancel() method
            gen.GetAllFormulas();

            // Next GetNextFormula() call should return null
            IMolecularFormula f = gen.GetNextFormula();
            Assert.IsNull(f);
        }

        /// <summary>
        /// Test empty molecular formula range
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEmptyMFRange()
        {
            new MolecularFormulaGenerator(builder, 0, 100, new MolecularFormulaRange());
        }

        /// <summary>
        /// Test negative mass
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestNegativeMass()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 100);
            new MolecularFormulaGenerator(builder, -20, -10, new MolecularFormulaRange());
        }

        /// <summary>
        /// Test if the generator respects minimal element counts
        /// </summary>
        [TestMethod()]
        public void TestMinCounts()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 5, 20);
            mfRange.AddIsotope(h, 5, 20);
            mfRange.AddIsotope(o, 5, 20);
            mfRange.AddIsotope(n, 5, 20);

            // The minimal formula MF=C5H5O5N5 MW=215.0290682825
            double minMass = 100;
            double maxMass = 250;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
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

        /// <summary>
        /// Test if the generator respects maximal element counts
        /// </summary>
        [TestMethod()]
        public void TestMaxCounts()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 3, 7);
            mfRange.AddIsotope(h, 3, 7);
            mfRange.AddIsotope(o, 3, 7);
            mfRange.AddIsotope(n, 3, 7);

            // The maximal formula MF=C7H7O7N7 MW=301.0406955954
            double minMass = 250;
            double maxMass = 400;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
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

        /// <summary>
        /// Test to find a single carbon.
        /// </summary>
        [TestMethod()]
        public void TestSingleCarbon()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 100);

            double minMass = 5;
            double maxMass = 15;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C", MolecularFormulaManipulator.GetString(mfSet[0]));
        }

        /// <summary>
        /// Test to find MF=C10000, MW=120000.0 using only carbons.
        /// </summary>
        [TestMethod()]
        public void TestCarbons()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 100000);

            double minMass = 120000.0 - 1;
            double maxMass = 120000.0 + 1;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C10000", MolecularFormulaManipulator.GetString(mfSet[0]));
        }

        /// <summary>
        /// Test to find H2O in a range of 1-20.
        /// </summary>
        [TestMethod()]
        public void TestWater()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");
            IIsotope p = ifac.GetMajorIsotope("P");
            IIsotope s = ifac.GetMajorIsotope("S");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 10);
            mfRange.AddIsotope(h, 0, 10);
            mfRange.AddIsotope(o, 0, 10);
            mfRange.AddIsotope(n, 0, 10);
            mfRange.AddIsotope(p, 0, 10);
            mfRange.AddIsotope(s, 0, 10);

            double minMass = 1;
            double maxMass = 20;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
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

        /// <summary>
        /// MolecularFormulaGenerator should use full enumeration method when smallest element has large weight
        /// </summary>
        [TestMethod()]
        public void TestUseFullEnumerationWhenNoHydrogen()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 50);
            mfRange.AddIsotope(o, 0, 30);
            mfRange.AddIsotope(n, 0, 10);

            MolecularFormulaGenerator generator = new MolecularFormulaGenerator(builder, 1023.000, 1023.002, mfRange);
            Assert.IsTrue(generator.formulaGenerator is FullEnumerationFormulaGenerator, "generator implementation should be instance of FullEnumerationFormulaGenerator");
        }

        /// <summary>
        /// MolecularFormulaGenerator should use full enumeration method when the mass deviation is very large (i.e. as
        /// large as the smallest weight)
        /// </summary>
        [TestMethod()]
        public void TestUseFullEnumerationWhenSuperLargeMassDeviation()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 20);
            mfRange.AddIsotope(h, 0, 30);
            mfRange.AddIsotope(o, 0, 15);
            mfRange.AddIsotope(n, 0, 10);

            MolecularFormulaGenerator generator = new MolecularFormulaGenerator(builder, 13, 14, mfRange);
            Assert.IsTrue(generator.formulaGenerator is FullEnumerationFormulaGenerator, "generator implementation should be instance of FullEnumerationFormulaGenerator");
        }

        /// <summary>
        /// MolecularFormulaGenerator should use full enumeration method when mass to decompose is too large to encode
        /// it as 32 bit integer with default blowup factor
        /// </summary>
        [TestMethod()]
        public void TestUseFullEnumerationWhenExceedIntegerSpace()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 20);
            mfRange.AddIsotope(h, 0, 30);
            mfRange.AddIsotope(o, 0, 15);
            mfRange.AddIsotope(n, 0, 10);

            MolecularFormulaGenerator generator = new MolecularFormulaGenerator(builder, 1300000, 1300000.1, mfRange);
            Assert.IsTrue(generator.formulaGenerator is FullEnumerationFormulaGenerator, "generator implementation should be instance of FullEnumerationFormulaGenerator");
        }

        /// <summary>
        /// MolecularFormulaGenerator should use Round Robin when using proper input
        /// </summary>
        [TestMethod()]
        public void TestUseRoundRobinWheneverPossible()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 20);
            mfRange.AddIsotope(h, 0, 30);
            mfRange.AddIsotope(o, 0, 15);
            mfRange.AddIsotope(n, 0, 10);

            MolecularFormulaGenerator generator = new MolecularFormulaGenerator(builder, 230.002, 230.004, mfRange);
            Assert.IsTrue(generator.formulaGenerator is RoundRobinFormulaGenerator, "generator implementation should be instance of RoundRobinFormulaGenerator");
        }

        /// <summary>
        /// Test to find MF=C5H11N2O, MW=115.08714
        /// </summary>
        [TestMethod()]
        public void TestSmallMass()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 20);
            mfRange.AddIsotope(h, 0, 30);
            mfRange.AddIsotope(o, 0, 15);
            mfRange.AddIsotope(n, 0, 10);

            double minMass = 115.08714 - 0.0001;
            double maxMass = 115.08714 + 0.0001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C5H11N2O", MolecularFormulaManipulator.GetString(mfSet[0]));
        }

        /// <summary>
        /// Test to find pentacarboxyporphyrin, MF=C37H38N4O10 MW=698.25879
        /// </summary>
        [TestMethod()]
        public void TestMiddleMass()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 50);
            mfRange.AddIsotope(h, 0, 100);
            mfRange.AddIsotope(o, 0, 30);
            mfRange.AddIsotope(n, 0, 10);

            double minMass = 698.25879 - 0.0001;
            double maxMass = 698.25879 + 0.0001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C37H38N4O10", MolecularFormulaManipulator.GetString(mfSet[0]));
        }

        /// <summary>
        /// Test to find ubiquitin: MF=C374H623N103O116S MW=8445.573784
        /// </summary>
        [TestMethod()]
        public void TestHighMass()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");
            IIsotope s = ifac.GetMajorIsotope("S");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 350, 400);
            mfRange.AddIsotope(h, 620, 650);
            mfRange.AddIsotope(o, 100, 150);
            mfRange.AddIsotope(n, 100, 150);
            mfRange.AddIsotope(s, 0, 10);

            double minMass = 8445.573784 - 0.00001;
            double maxMass = 8445.573784 + 0.00001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C374H623N103O116S", MolecularFormulaManipulator.GetString(mfSet[0]));
        }

        /// <summary>
        /// Test if formula MF=C4H11NO4 MW=137.06881 is found in mass range
        /// 137-137.2.
        /// </summary>
        [TestMethod()]
        public void TestFormulaFoundInRange()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 1, 50);
            mfRange.AddIsotope(h, 1, 100);
            mfRange.AddIsotope(o, 1, 50);
            mfRange.AddIsotope(n, 1, 50);

            double minMass = 137.0;
            double maxMass = 137.2;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
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

        /// <summary>
        /// Test if formula MF=C11H10NO2 MW=188.07115 is found in mass range 187-189.
        /// </summary>
        [TestMethod()]
        public void TestFormulaFoundInRange2()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 1, 50);
            mfRange.AddIsotope(h, 1, 100);
            mfRange.AddIsotope(o, 1, 50);
            mfRange.AddIsotope(n, 1, 50);

            double minMass = 187;
            double maxMass = 189;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
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

        /// <summary>
        /// Test if formula with 7 different elements is found in a narrow mass
        /// range. MF=C8H9Cl3NO2PS MW=318.915719
        /// </summary>
        [TestMethod()]
        public void TestCompoundWith7Elements()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");
            IIsotope s = ifac.GetMajorIsotope("S");
            IIsotope p = ifac.GetMajorIsotope("P");
            IIsotope cl = ifac.GetMajorIsotope("Cl");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 7, 9);
            mfRange.AddIsotope(h, 8, 10);
            mfRange.AddIsotope(o, 1, 3);
            mfRange.AddIsotope(n, 0, 2);
            mfRange.AddIsotope(s, 0, 2);
            mfRange.AddIsotope(p, 0, 2);
            mfRange.AddIsotope(cl, 2, 4);

            double minMass = 318.915719 - 0.0001;
            double maxMass = 318.915719 + 0.0001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C8H9Cl3NO2PS", MolecularFormulaManipulator.GetString(mfSet[0]));
        }

        /// <summary>
        /// Test if C13 isotope-containing formula is found. MF=C(^12)3C(^13)H5
        /// </summary>
        [TestMethod()]
        public void TestDifferentIsotopes()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            var carbons = ifac.GetIsotopes("C");
            IIsotope c13 = carbons.ElementAt(5); // 13
            IIsotope h = ifac.GetMajorIsotope("H");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 11);
            mfRange.AddIsotope(c13, 0, 10);
            mfRange.AddIsotope(h, 0, 10);

            double minMass = 54.04193 - 0.001;
            double maxMass = 54.04193 + 0.001;

            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, minMass, maxMass, mfRange);
            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);

            IMolecularFormula trueFormula = new MolecularFormula(); // C3CH5
            trueFormula.Add(c, 3);
            trueFormula.Add(c13, 1);
            trueFormula.Add(h, 5);

            Assert.AreEqual(trueFormula.IsotopesCount, mfSet[0].IsotopesCount);
            Assert.AreEqual(trueFormula.GetCount(c), mfSet[0].GetCount(c));
            Assert.AreEqual(trueFormula.GetCount(c13), mfSet[0].GetCount(c13));
        }

        /// <summary>
        /// Test if formula MF=C7H15N2O4 MW=191.10318 is found properly if we fix the
        /// element counts
        /// </summary>
        [TestMethod()]
        public void TestFixedElementCounts()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 7, 7);
            mfRange.AddIsotope(h, 15, 15);
            mfRange.AddIsotope(o, 4, 4);
            mfRange.AddIsotope(n, 2, 2);

            double massMin = 10d;
            double massMax = 1000d;
            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, massMin, massMax, mfRange);

            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(1, mfSet.Count);
            Assert.AreEqual("C7H15N2O4", MolecularFormulaManipulator.GetString(mfSet[0]));
        }

        /// <summary>
        /// Test if zero results are returned in case the target mass range is too
        /// high
        /// </summary>
        [TestMethod()]
        public void TestMassRangeTooHigh()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 0, 10);
            mfRange.AddIsotope(h, 0, 10);
            mfRange.AddIsotope(o, 0, 10);
            mfRange.AddIsotope(n, 0, 10);

            double massMin = 1000d;
            double massMax = 2000d;
            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, massMin, massMax, mfRange);

            IMolecularFormulaSet mfSet = gen.GetAllFormulas();

            Assert.IsNotNull(mfSet);
            Assert.AreEqual(0, mfSet.Count);
        }

        /// <summary>
        /// Test if zero results are returned in case the target mass range is too
        /// low
        /// </summary>
        [TestMethod()]
        public void TestMassRangeTooLow()
        {
            var ifac = BODRIsotopeFactory.Instance;
            IIsotope c = ifac.GetMajorIsotope("C");
            IIsotope h = ifac.GetMajorIsotope("H");
            IIsotope n = ifac.GetMajorIsotope("N");
            IIsotope o = ifac.GetMajorIsotope("O");

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(c, 100, 200);
            mfRange.AddIsotope(h, 100, 200);
            mfRange.AddIsotope(o, 100, 200);
            mfRange.AddIsotope(n, 100, 200);

            double massMin = 50d;
            double massMax = 100d;
            MolecularFormulaGenerator gen = new MolecularFormulaGenerator(builder, massMin, massMax, mfRange);

            IMolecularFormulaSet mfSet = gen.GetAllFormulas();
            Assert.IsNotNull(mfSet);
            Assert.AreEqual(0, mfSet.Count);
        }
    }
}
