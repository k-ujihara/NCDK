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
    /// </summary>
    // @cdk.module test-formula
    [TestClass()]
    public class MolecularFormulaRangeManipulatorTest : CDKTestCase
    {
        private readonly static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  Constructor for the MolecularFormulaRangeManipulatorTest object.
        /// </summary>
        public MolecularFormulaRangeManipulatorTest()
            : base()
        { }

        [TestMethod()]
        public void TestGetRange_IMolecularFormulaSet()
        {
            IMolecularFormula mf1 = new MolecularFormula(); /* C4H12NO4 */
            mf1.Add(builder.NewIsotope("C"), 4);
            mf1.Add(builder.NewIsotope("H"), 12);
            mf1.Add(builder.NewIsotope("N"), 1);
            mf1.Add(builder.NewIsotope("O"), 4);

            IMolecularFormula mf2 = new MolecularFormula(); /* C7H20N4O2 */
            mf2.Add(builder.NewIsotope("C"), 7);
            mf2.Add(builder.NewIsotope("H"), 20);
            mf2.Add(builder.NewIsotope("N"), 4);
            mf2.Add(builder.NewIsotope("O"), 2);

            IMolecularFormula mf3 = new MolecularFormula(); /* C9H5O7 */
            mf3.Add(builder.NewIsotope("C"), 9);
            mf3.Add(builder.NewIsotope("H"), 5);
            mf3.Add(builder.NewIsotope("O"), 7);

            IMolecularFormulaSet mfSet = new MolecularFormulaSet();
            mfSet.Add(mf1);
            mfSet.Add(mf2);
            mfSet.Add(mf3);

            MolecularFormulaRange mfRange = MolecularFormulaRangeManipulator.GetRange(mfSet);

            /* Result: C4-9H5-20N0-4O2-7 */

            Assert.AreEqual(4, mfRange.GetIsotopes().Count());
            Assert.AreEqual(4, mfRange.GetIsotopeCountMin(builder.NewIsotope("C")));
            Assert.AreEqual(9, mfRange.GetIsotopeCountMax(builder.NewIsotope("C")));
            Assert.AreEqual(5, mfRange.GetIsotopeCountMin(builder.NewIsotope("H")));
            Assert.AreEqual(20, mfRange.GetIsotopeCountMax(builder.NewIsotope("H")));
            Assert.AreEqual(0, mfRange.GetIsotopeCountMin(builder.NewIsotope("N")));
            Assert.AreEqual(4, mfRange.GetIsotopeCountMax(builder.NewIsotope("N")));
            Assert.AreEqual(2, mfRange.GetIsotopeCountMin(builder.NewIsotope("O")));
            Assert.AreEqual(7, mfRange.GetIsotopeCountMax(builder.NewIsotope("O")));
        }

        [TestMethod()]
        public void TestGetMaximalFormula_MolecularFormulaRange_IChemObjectBuilder()
        {
            IMolecularFormula mf1 = new MolecularFormula(); /* C4H12NO4 */
            mf1.Add(builder.NewIsotope("C"), 4);
            mf1.Add(builder.NewIsotope("H"), 12);
            mf1.Add(builder.NewIsotope("N"), 1);
            mf1.Add(builder.NewIsotope("O"), 4);

            IMolecularFormula mf2 = new MolecularFormula(); /* C7H20N4O2 */
            mf2.Add(builder.NewIsotope("C"), 7);
            mf2.Add(builder.NewIsotope("H"), 20);
            mf2.Add(builder.NewIsotope("N"), 4);
            mf2.Add(builder.NewIsotope("O"), 2);

            IMolecularFormula mf3 = new MolecularFormula(); /* C9H5O7 */
            mf3.Add(builder.NewIsotope("C"), 9);
            mf3.Add(builder.NewIsotope("H"), 5);
            mf3.Add(builder.NewIsotope("O"), 7);

            IMolecularFormulaSet mfSet = new MolecularFormulaSet();
            mfSet.Add(mf1);
            mfSet.Add(mf2);
            mfSet.Add(mf3);

            MolecularFormulaRange mfRange = MolecularFormulaRangeManipulator.GetRange(mfSet);
            IMolecularFormula formula = MolecularFormulaRangeManipulator.GetMaximalFormula(mfRange, builder);

            /* Result: C4-9H5-20N0-4O2-7 */

            Assert.AreEqual(4, mfRange.GetIsotopes().Count());
            Assert.AreEqual(formula.GetCount(builder.NewIsotope("C")),
                    mfRange.GetIsotopeCountMax(builder.NewIsotope("C")));
            Assert.AreEqual(formula.GetCount(builder.NewIsotope("H")),
                    mfRange.GetIsotopeCountMax(builder.NewIsotope("H")));
            Assert.AreEqual(formula.GetCount(builder.NewIsotope("N")),
                    mfRange.GetIsotopeCountMax(builder.NewIsotope("N")));
            Assert.AreEqual(formula.GetCount(builder.NewIsotope("O")),
                    mfRange.GetIsotopeCountMax(builder.NewIsotope("O")));
        }

        [TestMethod()]
        public void TestGetMinimalFormula_MolecularFormulaRange_IChemObjectBuilder()
        {
            IMolecularFormula mf1 = new MolecularFormula(); /* C4H12NO4 */
            mf1.Add(builder.NewIsotope("C"), 4);
            mf1.Add(builder.NewIsotope("H"), 12);
            mf1.Add(builder.NewIsotope("N"), 1);
            mf1.Add(builder.NewIsotope("O"), 4);

            IMolecularFormula mf2 = new MolecularFormula(); /* C7H20N4O2 */
            mf2.Add(builder.NewIsotope("C"), 7);
            mf2.Add(builder.NewIsotope("H"), 20);
            mf2.Add(builder.NewIsotope("N"), 4);
            mf2.Add(builder.NewIsotope("O"), 2);

            IMolecularFormula mf3 = new MolecularFormula(); /* C9H5O7 */
            mf3.Add(builder.NewIsotope("C"), 9);
            mf3.Add(builder.NewIsotope("H"), 5);
            mf3.Add(builder.NewIsotope("O"), 7);

            IMolecularFormulaSet mfSet = new MolecularFormulaSet();
            mfSet.Add(mf1);
            mfSet.Add(mf2);
            mfSet.Add(mf3);

            MolecularFormulaRange mfRange = MolecularFormulaRangeManipulator.GetRange(mfSet);
            IMolecularFormula formula = MolecularFormulaRangeManipulator.GetMinimalFormula(mfRange, builder);

            /* Result: C4-9H5-20N0-4O2-7 */

            Assert.AreEqual(4, mfRange.GetIsotopes().Count());
            Assert.AreEqual(formula.GetCount(builder.NewIsotope("C")),
                    mfRange.GetIsotopeCountMin(builder.NewIsotope("C")));
            Assert.AreEqual(formula.GetCount(builder.NewIsotope("H")),
                    mfRange.GetIsotopeCountMin(builder.NewIsotope("H")));
            Assert.AreEqual(formula.GetCount(builder.NewIsotope("N")),
                    mfRange.GetIsotopeCountMin(builder.NewIsotope("N")));
            Assert.AreEqual(formula.GetCount(builder.NewIsotope("O")),
                    mfRange.GetIsotopeCountMin(builder.NewIsotope("O")));
        }
    }
}
