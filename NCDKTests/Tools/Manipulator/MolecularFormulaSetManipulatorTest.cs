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
    /// </summary>
    // @cdk.module test-formula
    [TestClass()]
    public class MolecularFormulaSetManipulatorTest : CDKTestCase
    {
        private readonly static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  Constructor for the MolecularFormulaSetManipulatorTest object.
        /// </summary>
        public MolecularFormulaSetManipulatorTest()
            : base()
        { }

        [TestMethod()]
        public void TestGetMaxOccurrenceElements_IMolecularFormulaSet()
        {
            var mf1 = new MolecularFormula(); /* C4H12NO4 */
            mf1.Add(builder.NewIsotope("C"), 4);
            mf1.Add(builder.NewIsotope("H"), 12);
            mf1.Add(builder.NewIsotope("N"), 1);
            mf1.Add(builder.NewIsotope("O"), 4);

            var mf2 = new MolecularFormula(); /* C7H20N4O2 */
            mf2.Add(builder.NewIsotope("C"), 7);
            mf2.Add(builder.NewIsotope("H"), 20);
            mf2.Add(builder.NewIsotope("N"), 4);
            mf2.Add(builder.NewIsotope("O"), 2);

            var mf3 = new MolecularFormula(); /* C9H5O7 */
            mf3.Add(builder.NewIsotope("C"), 9);
            mf3.Add(builder.NewIsotope("H"), 5);
            mf3.Add(builder.NewIsotope("O"), 7);

            IMolecularFormulaSet mfSet = new MolecularFormulaSet
            {
                mf1,
                mf2,
                mf3
            };

            var molecularFormula = MolecularFormulaSetManipulator.GetMaxOccurrenceElements(mfSet);

            /* Result: C9H20N4O7 */

            Assert.AreEqual(40, MolecularFormulaManipulator.GetAtomCount(molecularFormula));
            Assert.AreEqual(4, molecularFormula.Isotopes.Count());
            Assert.AreEqual(9, molecularFormula.GetCount(builder.NewIsotope("C")));
            Assert.AreEqual(20, molecularFormula.GetCount(builder.NewIsotope("H")));
            Assert.AreEqual(4, molecularFormula.GetCount(builder.NewIsotope("N")));
            Assert.AreEqual(7, molecularFormula.GetCount(builder.NewIsotope("O")));
        }

        [TestMethod()]
        public void TestGetMinOccurrenceElements_IMolecularFormulaSet()
        {
            var mf1 = new MolecularFormula(); /* C4H12NO4 */
            mf1.Add(builder.NewIsotope("C"), 4);
            mf1.Add(builder.NewIsotope("H"), 12);
            mf1.Add(builder.NewIsotope("N"), 1);
            mf1.Add(builder.NewIsotope("O"), 4);

            var mf2 = new MolecularFormula(); /* C7H20N4O2 */
            mf2.Add(builder.NewIsotope("C"), 7);
            mf2.Add(builder.NewIsotope("H"), 20);
            mf2.Add(builder.NewIsotope("N"), 4);
            mf2.Add(builder.NewIsotope("O"), 2);

            var mf3 = new MolecularFormula(); /* C9H5O7 */
            mf3.Add(builder.NewIsotope("C"), 9);
            mf3.Add(builder.NewIsotope("H"), 5);
            mf3.Add(builder.NewIsotope("O"), 7);

            var mfSet = new MolecularFormulaSet
            {
                mf1,
                mf2,
                mf3
            };

            var molecularFormula = MolecularFormulaSetManipulator.GetMinOccurrenceElements(mfSet);

            /* Result: C4H5NO2 */

            Assert.AreEqual(12, MolecularFormulaManipulator.GetAtomCount(molecularFormula));
            Assert.AreEqual(4, molecularFormula.Isotopes.Count());
            Assert.AreEqual(4, molecularFormula.GetCount(builder.NewIsotope("C")));
            Assert.AreEqual(5, molecularFormula.GetCount(builder.NewIsotope("H")));
            Assert.AreEqual(1, molecularFormula.GetCount(builder.NewIsotope("N")));
            Assert.AreEqual(2, molecularFormula.GetCount(builder.NewIsotope("O")));
        }

        [TestMethod()]
        public void TestRemove_IMolecularFormulaSet_IMolecularFormula_IMolecularFormula()
        {
            var formulaMin = new MolecularFormula();
            formulaMin.Add(builder.NewIsotope("C"), 1);
            formulaMin.Add(builder.NewIsotope("H"), 1);
            formulaMin.Add(builder.NewIsotope("O"), 1);

            var formulaMax = new MolecularFormula();
            formulaMax.Add(builder.NewIsotope("C"), 4);
            formulaMax.Add(builder.NewIsotope("H"), 12);
            formulaMax.Add(builder.NewIsotope("N"), 2);

            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C"), 3);
            mf1.Add(builder.NewIsotope("H"), 10);
            mf1.Add(builder.NewIsotope("N"), 1);

            var formulaSet = new MolecularFormulaSet { mf1 };
            var newMFSet = MolecularFormulaSetManipulator.Remove(formulaSet, formulaMin, formulaMax);
            Assert.IsNull(newMFSet);
        }

        [TestMethod()]
        public void TestRemove_1()
        {
            var formulaMin = new MolecularFormula();
            formulaMin.Add(builder.NewIsotope("C"), 1);
            formulaMin.Add(builder.NewIsotope("H"), 1);
            formulaMin.Add(builder.NewIsotope("N"), 1);

            var formulaMax = new MolecularFormula();
            formulaMax.Add(builder.NewIsotope("C"), 4);
            formulaMax.Add(builder.NewIsotope("H"), 12);
            formulaMax.Add(builder.NewIsotope("N"), 2);

            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C"), 3);
            mf1.Add(builder.NewIsotope("H"), 10);
            mf1.Add(builder.NewIsotope("N"), 1);

            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 1);
            mf2.Add(builder.NewIsotope("H"), 1);
            mf2.Add(builder.NewIsotope("N"), 1);

            var mf3 = new MolecularFormula();
            mf3.Add(builder.NewIsotope("C"), 4);
            mf3.Add(builder.NewIsotope("H"), 12);
            mf3.Add(builder.NewIsotope("N"), 2);

            var mf4 = new MolecularFormula();
            mf4.Add(builder.NewIsotope("C"), 7);
            mf4.Add(builder.NewIsotope("H"), 10);
            mf4.Add(builder.NewIsotope("N"), 1);

            var formulaSet = new MolecularFormulaSet
            {
                mf1,
                mf2,
                mf3,
                mf4
            };

            var newMFSet = MolecularFormulaSetManipulator.Remove(formulaSet, formulaMin, formulaMax);
            /* the mf4 is excluded from the limits */

            Assert.AreEqual(3, newMFSet.Count());
            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf1),
                    MolecularFormulaManipulator.GetString(newMFSet[0]));
            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf2),
                    MolecularFormulaManipulator.GetString(newMFSet[1]));
            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf3),
                    MolecularFormulaManipulator.GetString(newMFSet[2]));
        }

        [TestMethod()]
        public void TestRemove_2()
        {
            var formulaMin = new MolecularFormula();
            formulaMin.Add(builder.NewIsotope("C"), 1);
            formulaMin.Add(builder.NewIsotope("H"), 1);
            formulaMin.Add(builder.NewIsotope("N"), 1);

            var formulaMax = new MolecularFormula();
            formulaMax.Add(builder.NewIsotope("C"), 4);
            formulaMax.Add(builder.NewIsotope("H"), 12);
            formulaMax.Add(builder.NewIsotope("N"), 2);

            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C"), 3);
            mf1.Add(builder.NewIsotope("H"), 10);
            mf1.Add(builder.NewIsotope("N"), 1);

            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 1);
            mf2.Add(builder.NewIsotope("H"), 1);

            var formulaSet = new MolecularFormulaSet
            {
                mf1,
                mf2
            };

            var newMFSet = MolecularFormulaSetManipulator.Remove(formulaSet, formulaMin, formulaMax);
            /* the mf2 is excluded from the limits. It doesn't contain N */

            Assert.AreEqual(1, newMFSet.Count());
            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf1),
                    MolecularFormulaManipulator.GetString(newMFSet[0]));
        }

        [TestMethod()]
        public void TestRemove_3()
        {
            var formulaMin = new MolecularFormula();
            formulaMin.Add(builder.NewIsotope("C"), 1);
            formulaMin.Add(builder.NewIsotope("H"), 1);
            formulaMin.Add(builder.NewIsotope("N"), 1);

            var formulaMax = new MolecularFormula();
            formulaMax.Add(builder.NewIsotope("C"), 4);
            formulaMax.Add(builder.NewIsotope("H"), 12);
            formulaMax.Add(builder.NewIsotope("N"), 2);

            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C"), 3);
            mf1.Add(builder.NewIsotope("H"), 10);
            mf1.Add(builder.NewIsotope("N"), 1);

            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 1);
            mf2.Add(builder.NewIsotope("H"), 1);
            mf2.Add(builder.NewIsotope("O"), 1);

            var formulaSet = new MolecularFormulaSet
            {
                mf1,
                mf2
            };

            var newMFSet = MolecularFormulaSetManipulator.Remove(formulaSet, formulaMin, formulaMax);
            /* the mf2 is excluded from the limits. It doesn't contain N */

            Assert.AreEqual(1, newMFSet.Count());
            Assert.AreEqual(MolecularFormulaManipulator.GetString(mf1),
                    MolecularFormulaManipulator.GetString(newMFSet[0]));
        }

        [TestMethod()]
        public void TestRemove_IMolecularFormulaSet_MolecularFormulaRange()
        {
            var formulaRange = new MolecularFormulaRange();
            formulaRange.AddIsotope(builder.NewIsotope("C"), 0, 4);
            formulaRange.AddIsotope(builder.NewIsotope("H"), 0, 12);
            formulaRange.AddIsotope(builder.NewIsotope("N"), 0, 2);

            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 1);
            mf2.Add(builder.NewIsotope("H"), 11);
            mf2.Add(builder.NewIsotope("N"), 1);

            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C"), 3);
            mf1.Add(builder.NewIsotope("H"), 10);

            var formulaSet = new MolecularFormulaSet
            {
                mf1,
                mf2
            };

            var newMFSet = MolecularFormulaSetManipulator.Remove(formulaSet, formulaRange);
            /* the mf2 is excluded from the limits. It doesn't contain N */

            Assert.AreEqual(2, newMFSet.Count());
        }

        [TestMethod()]
        public void TestContains_IMolecularFormulaSet_IMolecularFormula()
        {
            var mf1 = new MolecularFormula();
            mf1.Add(builder.NewIsotope("C"), 4);
            mf1.Add(builder.NewIsotope("H"), 12);
            mf1.Add(builder.NewIsotope("N"), 1);
            mf1.Add(builder.NewIsotope("O"), 4);

            var mf3 = new MolecularFormula();
            mf3.Add(builder.NewIsotope("C"), 9);
            mf3.Add(builder.NewIsotope("H"), 5);
            mf3.Add(builder.NewIsotope("O"), 7);

            var formulaSet = new MolecularFormulaSet
            {
                mf1,
                mf3
            };

            var mf2 = new MolecularFormula();
            mf2.Add(builder.NewIsotope("C"), 4);
            mf2.Add(builder.NewIsotope("H"), 12);
            mf2.Add(builder.NewIsotope("N"), 1);
            mf2.Add(builder.NewIsotope("O"), 4);

            var mf4 = new MolecularFormula();
            mf4.Add(builder.NewIsotope("C"), 4);
            var hyd = builder.NewIsotope("H");
            hyd.ExactMass = 2.0032342;
            mf4.Add(hyd, 12);
            mf4.Add(builder.NewIsotope("N"), 1);
            mf4.Add(builder.NewIsotope("O"), 4);

            Assert.IsTrue(MolecularFormulaSetManipulator.Contains(formulaSet, mf2));
            Assert.IsFalse(MolecularFormulaSetManipulator.Contains(formulaSet, mf4));
        }
    }
}
