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
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Formula;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    /// Checks the functionality of the MolecularFormulaSetManipulator.
    ///
    // @cdk.module test-formula
    /// </summary>
    [TestClass()]
    public class MolecularFormulaSetManipulatorTest : CDKTestCase
    {

        private readonly static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  Constructor for the MolecularFormulaSetManipulatorTest object.
        ///
        /// </summary>
        public MolecularFormulaSetManipulatorTest()
            : base()
        { }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestGetMaxOccurrenceElements_IMolecularFormulaSet()
        {
            IMolecularFormula mf1 = new MolecularFormula(); /* C4H12NO4 */
            mf1.Add(builder.CreateIsotope("C"), 4);
            mf1.Add(builder.CreateIsotope("H"), 12);
            mf1.Add(builder.CreateIsotope("N"), 1);
            mf1.Add(builder.CreateIsotope("O"), 4);

            IMolecularFormula mf2 = new MolecularFormula(); /* C7H20N4O2 */
            mf2.Add(builder.CreateIsotope("C"), 7);
            mf2.Add(builder.CreateIsotope("H"), 20);
            mf2.Add(builder.CreateIsotope("N"), 4);
            mf2.Add(builder.CreateIsotope("O"), 2);

            IMolecularFormula mf3 = new MolecularFormula(); /* C9H5O7 */
            mf3.Add(builder.CreateIsotope("C"), 9);
            mf3.Add(builder.CreateIsotope("H"), 5);
            mf3.Add(builder.CreateIsotope("O"), 7);

            IMolecularFormulaSet mfSet = new MolecularFormulaSet();
            mfSet.Add(mf1);
            mfSet.Add(mf2);
            mfSet.Add(mf3);

            IMolecularFormula molecularFormula = MolecularFormulaSetManipulator.GetMaxOccurrenceElements(mfSet);

            /* Result: C9H20N4O7 */

            Assert.AreEqual(40, MolecularFormulaManipulator.GetAtomCount(molecularFormula));
            Assert.AreEqual(4, molecularFormula.Isotopes.Count());
            Assert.AreEqual(9, molecularFormula.GetCount(builder.CreateIsotope("C")));
            Assert.AreEqual(20, molecularFormula.GetCount(builder.CreateIsotope("H")));
            Assert.AreEqual(4, molecularFormula.GetCount(builder.CreateIsotope("N")));
            Assert.AreEqual(7, molecularFormula.GetCount(builder.CreateIsotope("O")));

        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestGetMinOccurrenceElements_IMolecularFormulaSet()
        {
            IMolecularFormula mf1 = new MolecularFormula(); /* C4H12NO4 */
            mf1.Add(builder.CreateIsotope("C"), 4);
            mf1.Add(builder.CreateIsotope("H"), 12);
            mf1.Add(builder.CreateIsotope("N"), 1);
            mf1.Add(builder.CreateIsotope("O"), 4);

            IMolecularFormula mf2 = new MolecularFormula(); /* C7H20N4O2 */
            mf2.Add(builder.CreateIsotope("C"), 7);
            mf2.Add(builder.CreateIsotope("H"), 20);
            mf2.Add(builder.CreateIsotope("N"), 4);
            mf2.Add(builder.CreateIsotope("O"), 2);

            IMolecularFormula mf3 = new MolecularFormula(); /* C9H5O7 */
            mf3.Add(builder.CreateIsotope("C"), 9);
            mf3.Add(builder.CreateIsotope("H"), 5);
            mf3.Add(builder.CreateIsotope("O"), 7);

            IMolecularFormulaSet mfSet = new MolecularFormulaSet();
            mfSet.Add(mf1);
            mfSet.Add(mf2);
            mfSet.Add(mf3);

            IMolecularFormula molecularFormula = MolecularFormulaSetManipulator.GetMinOccurrenceElements(mfSet);

            /* Result: C4H5NO2 */

            Assert.AreEqual(12, MolecularFormulaManipulator.GetAtomCount(molecularFormula));
            Assert.AreEqual(4, molecularFormula.Isotopes.Count());
            Assert.AreEqual(4, molecularFormula.GetCount(builder.CreateIsotope("C")));
            Assert.AreEqual(5, molecularFormula.GetCount(builder.CreateIsotope("H")));
            Assert.AreEqual(1, molecularFormula.GetCount(builder.CreateIsotope("N")));
            Assert.AreEqual(2, molecularFormula.GetCount(builder.CreateIsotope("O")));

        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestRemove_IMolecularFormulaSet_IMolecularFormula_IMolecularFormula()
        {

            IMolecularFormula formulaMin = new MolecularFormula();
            formulaMin.Add(builder.CreateIsotope("C"), 1);
            formulaMin.Add(builder.CreateIsotope("H"), 1);
            formulaMin.Add(builder.CreateIsotope("O"), 1);

            IMolecularFormula formulaMax = new MolecularFormula();
            formulaMax.Add(builder.CreateIsotope("C"), 4);
            formulaMax.Add(builder.CreateIsotope("H"), 12);
            formulaMax.Add(builder.CreateIsotope("N"), 2);

            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C"), 3);
            mf1.Add(builder.CreateIsotope("H"), 10);
            mf1.Add(builder.CreateIsotope("N"), 1);

            IMolecularFormulaSet formulaSet = new MolecularFormulaSet();
            formulaSet.Add(mf1);

            IMolecularFormulaSet newMFSet = MolecularFormulaSetManipulator.Remove(formulaSet, formulaMin, formulaMax);

            Assert.IsNull(newMFSet);

        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestRemove_1()
        {

            IMolecularFormula formulaMin = new MolecularFormula();
            formulaMin.Add(builder.CreateIsotope("C"), 1);
            formulaMin.Add(builder.CreateIsotope("H"), 1);
            formulaMin.Add(builder.CreateIsotope("N"), 1);

            IMolecularFormula formulaMax = new MolecularFormula();
            formulaMax.Add(builder.CreateIsotope("C"), 4);
            formulaMax.Add(builder.CreateIsotope("H"), 12);
            formulaMax.Add(builder.CreateIsotope("N"), 2);

            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C"), 3);
            mf1.Add(builder.CreateIsotope("H"), 10);
            mf1.Add(builder.CreateIsotope("N"), 1);

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 1);
            mf2.Add(builder.CreateIsotope("H"), 1);
            mf2.Add(builder.CreateIsotope("N"), 1);

            IMolecularFormula mf3 = new MolecularFormula();
            mf3.Add(builder.CreateIsotope("C"), 4);
            mf3.Add(builder.CreateIsotope("H"), 12);
            mf3.Add(builder.CreateIsotope("N"), 2);

            IMolecularFormula mf4 = new MolecularFormula();
            mf4.Add(builder.CreateIsotope("C"), 7);
            mf4.Add(builder.CreateIsotope("H"), 10);
            mf4.Add(builder.CreateIsotope("N"), 1);

            IMolecularFormulaSet formulaSet = new MolecularFormulaSet();
            formulaSet.Add(mf1);
            formulaSet.Add(mf2);
            formulaSet.Add(mf3);
            formulaSet.Add(mf4);

            IMolecularFormulaSet newMFSet = MolecularFormulaSetManipulator.Remove(formulaSet, formulaMin, formulaMax);
            /* the mf4 is excluded from the limits */

            Assert.AreEqual(3, newMFSet.Count());
            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf1),
                    MolecularFormulaManipulator.GetString(newMFSet[0]));
            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf2),
                    MolecularFormulaManipulator.GetString(newMFSet[1]));
            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf3),
                    MolecularFormulaManipulator.GetString(newMFSet[2]));

        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestRemove_2()
        {

            IMolecularFormula formulaMin = new MolecularFormula();
            formulaMin.Add(builder.CreateIsotope("C"), 1);
            formulaMin.Add(builder.CreateIsotope("H"), 1);
            formulaMin.Add(builder.CreateIsotope("N"), 1);

            IMolecularFormula formulaMax = new MolecularFormula();
            formulaMax.Add(builder.CreateIsotope("C"), 4);
            formulaMax.Add(builder.CreateIsotope("H"), 12);
            formulaMax.Add(builder.CreateIsotope("N"), 2);

            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C"), 3);
            mf1.Add(builder.CreateIsotope("H"), 10);
            mf1.Add(builder.CreateIsotope("N"), 1);

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 1);
            mf2.Add(builder.CreateIsotope("H"), 1);

            IMolecularFormulaSet formulaSet = new MolecularFormulaSet();
            formulaSet.Add(mf1);
            formulaSet.Add(mf2);

            IMolecularFormulaSet newMFSet = MolecularFormulaSetManipulator.Remove(formulaSet, formulaMin, formulaMax);
            /* the mf2 is excluded from the limits. It doesn't contain N */

            Assert.AreEqual(1, newMFSet.Count());
            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf1),
                    MolecularFormulaManipulator.GetString(newMFSet[0]));

        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestRemove_3()
        {

            IMolecularFormula formulaMin = new MolecularFormula();
            formulaMin.Add(builder.CreateIsotope("C"), 1);
            formulaMin.Add(builder.CreateIsotope("H"), 1);
            formulaMin.Add(builder.CreateIsotope("N"), 1);

            IMolecularFormula formulaMax = new MolecularFormula();
            formulaMax.Add(builder.CreateIsotope("C"), 4);
            formulaMax.Add(builder.CreateIsotope("H"), 12);
            formulaMax.Add(builder.CreateIsotope("N"), 2);

            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C"), 3);
            mf1.Add(builder.CreateIsotope("H"), 10);
            mf1.Add(builder.CreateIsotope("N"), 1);

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 1);
            mf2.Add(builder.CreateIsotope("H"), 1);
            mf2.Add(builder.CreateIsotope("O"), 1);

            IMolecularFormulaSet formulaSet = new MolecularFormulaSet();
            formulaSet.Add(mf1);
            formulaSet.Add(mf2);

            IMolecularFormulaSet newMFSet = MolecularFormulaSetManipulator.Remove(formulaSet, formulaMin, formulaMax);
            /* the mf2 is excluded from the limits. It doesn't contain N */

            Assert.AreEqual(1, newMFSet.Count());
            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf1),
                    MolecularFormulaManipulator.GetString(newMFSet[0]));

        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestRemove_IMolecularFormulaSet_MolecularFormulaRange()
        {

            MolecularFormulaRange formulaRange = new MolecularFormulaRange();
            formulaRange.AddIsotope(builder.CreateIsotope("C"), 0, 4);
            formulaRange.AddIsotope(builder.CreateIsotope("H"), 0, 12);
            formulaRange.AddIsotope(builder.CreateIsotope("N"), 0, 2);

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 1);
            mf2.Add(builder.CreateIsotope("H"), 11);
            mf2.Add(builder.CreateIsotope("N"), 1);

            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C"), 3);
            mf1.Add(builder.CreateIsotope("H"), 10);

            IMolecularFormulaSet formulaSet = new MolecularFormulaSet();
            formulaSet.Add(mf1);
            formulaSet.Add(mf2);

            IMolecularFormulaSet newMFSet = MolecularFormulaSetManipulator.Remove(formulaSet, formulaRange);
            /* the mf2 is excluded from the limits. It doesn't contain N */

            Assert.AreEqual(2, newMFSet.Count());

        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestContains_IMolecularFormulaSet_IMolecularFormula()
        {
            IMolecularFormula mf1 = new MolecularFormula();
            mf1.Add(builder.CreateIsotope("C"), 4);
            mf1.Add(builder.CreateIsotope("H"), 12);
            mf1.Add(builder.CreateIsotope("N"), 1);
            mf1.Add(builder.CreateIsotope("O"), 4);

            IMolecularFormula mf3 = new MolecularFormula();
            mf3.Add(builder.CreateIsotope("C"), 9);
            mf3.Add(builder.CreateIsotope("H"), 5);
            mf3.Add(builder.CreateIsotope("O"), 7);

            IMolecularFormulaSet formulaSet = new MolecularFormulaSet();
            formulaSet.Add(mf1);
            formulaSet.Add(mf3);

            IMolecularFormula mf2 = new MolecularFormula();
            mf2.Add(builder.CreateIsotope("C"), 4);
            mf2.Add(builder.CreateIsotope("H"), 12);
            mf2.Add(builder.CreateIsotope("N"), 1);
            mf2.Add(builder.CreateIsotope("O"), 4);

            IMolecularFormula mf4 = new MolecularFormula();
            mf4.Add(builder.CreateIsotope("C"), 4);
            IIsotope hyd = builder.CreateIsotope("H");
            hyd.ExactMass = 2.0032342;
            mf4.Add(hyd, 12);
            mf4.Add(builder.CreateIsotope("N"), 1);
            mf4.Add(builder.CreateIsotope("O"), 4);

            Assert.IsTrue(MolecularFormulaSetManipulator.Contains(formulaSet, mf2));
            Assert.IsFalse(MolecularFormulaSetManipulator.Contains(formulaSet, mf4));
        }
    }
}
