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
    /// Checks the functionality of the MolecularFormulaRangeManipulator.
    ///
    // @cdk.module test-formula
    /// </summary>
    [TestClass()]
    public class MolecularFormulaRangeManipulatorTest : CDKTestCase
    {

        private readonly static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  Constructor for the MolecularFormulaRangeManipulatorTest object.
        ///
        /// </summary>
        public MolecularFormulaRangeManipulatorTest()
            : base()
        { }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestGetRange_IMolecularFormulaSet()
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

            MolecularFormulaRange mfRange = MolecularFormulaRangeManipulator.GetRange(mfSet);

            /* Result: C4-9H5-20N0-4O2-7 */

            Assert.AreEqual(4, mfRange.GetIsotopes().Count());
            Assert.AreEqual(4, mfRange.GetIsotopeCountMin(builder.CreateIsotope("C")));
            Assert.AreEqual(9, mfRange.GetIsotopeCountMax(builder.CreateIsotope("C")));
            Assert.AreEqual(5, mfRange.GetIsotopeCountMin(builder.CreateIsotope("H")));
            Assert.AreEqual(20, mfRange.GetIsotopeCountMax(builder.CreateIsotope("H")));
            Assert.AreEqual(0, mfRange.GetIsotopeCountMin(builder.CreateIsotope("N")));
            Assert.AreEqual(4, mfRange.GetIsotopeCountMax(builder.CreateIsotope("N")));
            Assert.AreEqual(2, mfRange.GetIsotopeCountMin(builder.CreateIsotope("O")));
            Assert.AreEqual(7, mfRange.GetIsotopeCountMax(builder.CreateIsotope("O")));

        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestGetMaximalFormula_MolecularFormulaRange_IChemObjectBuilder()
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

            MolecularFormulaRange mfRange = MolecularFormulaRangeManipulator.GetRange(mfSet);
            IMolecularFormula formula = MolecularFormulaRangeManipulator.GetMaximalFormula(mfRange, builder);

            /* Result: C4-9H5-20N0-4O2-7 */

            Assert.AreEqual(4, mfRange.GetIsotopes().Count());
            Assert.AreEqual(formula.GetCount(builder.CreateIsotope("C")),
                    mfRange.GetIsotopeCountMax(builder.CreateIsotope("C")));
            Assert.AreEqual(formula.GetCount(builder.CreateIsotope("H")),
                    mfRange.GetIsotopeCountMax(builder.CreateIsotope("H")));
            Assert.AreEqual(formula.GetCount(builder.CreateIsotope("N")),
                    mfRange.GetIsotopeCountMax(builder.CreateIsotope("N")));
            Assert.AreEqual(formula.GetCount(builder.CreateIsotope("O")),
                    mfRange.GetIsotopeCountMax(builder.CreateIsotope("O")));

        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestGetMinimalFormula_MolecularFormulaRange_IChemObjectBuilder()
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

            MolecularFormulaRange mfRange = MolecularFormulaRangeManipulator.GetRange(mfSet);
            IMolecularFormula formula = MolecularFormulaRangeManipulator.GetMinimalFormula(mfRange, builder);

            /* Result: C4-9H5-20N0-4O2-7 */

            Assert.AreEqual(4, mfRange.GetIsotopes().Count());
            Assert.AreEqual(formula.GetCount(builder.CreateIsotope("C")),
                    mfRange.GetIsotopeCountMin(builder.CreateIsotope("C")));
            Assert.AreEqual(formula.GetCount(builder.CreateIsotope("H")),
                    mfRange.GetIsotopeCountMin(builder.CreateIsotope("H")));
            Assert.AreEqual(formula.GetCount(builder.CreateIsotope("N")),
                    mfRange.GetIsotopeCountMin(builder.CreateIsotope("N")));
            Assert.AreEqual(formula.GetCount(builder.CreateIsotope("O")),
                    mfRange.GetIsotopeCountMin(builder.CreateIsotope("O")));
        }
    }
}
