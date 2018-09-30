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
using NCDK.Silent;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.Formula.Rules
{
    // @cdk.module test-formula
    [TestClass()]
    public class NitrogenRuleTest : FormulaRuleTest
    {
        private static readonly IChemObjectBuilder builder = ChemObjectBuilder.Instance;
        protected override Type RuleClass => typeof(NitrogenRule);

        [TestMethod()]
        public void TestNitrogenRule()
        {
            IRule rule = new NitrogenRule();
            Assert.IsNotNull(rule);
        }

        [TestMethod()]
        public void TestDefault()
        {
            IRule rule = new NitrogenRule();
            var objects = rule.Parameters;
            Assert.IsNull(objects);
        }

        [TestMethod()]
        public void TestSetParameters()
        {
            IRule rule = new NitrogenRule { Parameters = null };
            var objects = rule.Parameters;
            Assert.IsNull(objects);
        }

        [TestMethod()]
        public void TestDefaultValidFalse()
        {
            IRule rule = new NitrogenRule();
            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C2H4", builder);
            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        /// <summary>
        /// C2H11N4O4
        /// </summary>
        [TestMethod()]
        public void TestDefaultValidFalse_SetParam()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C2H11N4O4", builder);
            formula.Charge = 1;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestDefaultValidTrue()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C4H13N1O5", builder);
            formula.Charge = 0;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestC45H75NO15()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C45H75NO15", builder);
            formula.Charge = 0;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestC45H71N7O10()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C45H71N7O10", builder);
            formula.Charge = 0;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestC49H75NO12()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C49H75NO12", builder);
            formula.Charge = 0;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestC50H95NO10()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C50H95NO10", builder);
            formula.Charge = 0;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestC47H75N5O10()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C47H75N5O10", builder);
            formula.Charge = 0;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestC36H42N2O23()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("C36H42N2O23", builder);
            formula.Charge = 0;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestN()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("NH3", builder);
            formula.Charge = 0;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestNPlus()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMajorIsotopeMolecularFormula("NH4", builder);
            formula.Charge = 1;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestNominalMass()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula("C25H53NO7P", builder);
            formula.Charge = 1;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestDoubleCharge()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula("C22H34N2S2", builder);
            formula.Charge = 2;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        /// <summary>
        /// Compounds like Fe, Co, Hg, Pt, As.C40H46FeN6O8S2
        /// </summary>
        [TestMethod()]
        public void TestWithFe()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula("C40H46FeN6O8S2", builder);
            formula.Charge = 2;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }

        /// <summary>
        /// Compounds like Fe, Co, Hg, Pt, As.C40H46FeN6O8S2
        /// </summary>
        [TestMethod()]
        public void TestWithCo()
        {
            IRule rule = new NitrogenRule();

            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula("C43H50CoN4O16", builder);
            formula.Charge = 0;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }
    }
}
