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
using NCDK.Config;
using NCDK.Formula.Rules;
using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK.Formula
{
    /// <summary>
    /// Checks the functionality of the MolecularFormulaChecker.
    /// </summary>
    // @cdk.module test-formula
    [TestClass()]
    public class MolecularFormulaCheckerTest : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;
        private IsotopeFactory ifac;

        public MolecularFormulaCheckerTest()
            : base()
        {
            try
            {
                ifac = BODRIsotopeFactory.Instance;
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
        }

        [TestMethod()]
        public void TestMolecularFormulaChecker_List()
        {
            Assert.IsNotNull(new MolecularFormulaChecker(new List<IRule>()));
        }

        [TestMethod()]
        public void TestGetRules()
        {
            MolecularFormulaChecker MFChecker = new MolecularFormulaChecker(new List<IRule>());

            Assert.IsNotNull(MFChecker.Rules);
        }

        [TestMethod()]
        public void TestIsValidSum_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(ifac.GetMajorIsotope("C"), 1);
            formula.Add(ifac.GetMajorIsotope("H"), 100);

            List<IRule> rules = new List<IRule>
            {
                new MMElementRule()
            };

            MolecularFormulaChecker MFChecker = new MolecularFormulaChecker(rules);

            Assert.AreEqual(0.0, MFChecker.IsValidSum(formula), 0.001);
        }

        [TestMethod()]
        public void TestIsValid_NOT()
        {
            var formula = new MolecularFormula();
            formula.Add(ifac.GetMajorIsotope("C"), 1);
            formula.Add(ifac.GetMajorIsotope("H"), 100);

            List<IRule> rules = new List<IRule>
            {
                new MMElementRule()
            };

            MolecularFormulaChecker MFChecker = new MolecularFormulaChecker(rules);

            IMolecularFormula formulaWith = MFChecker.IsValid(formula);

            Assert.AreEqual(0.0, formulaWith.GetProperty<double>((new MMElementRule()).GetType().ToString()));
        }

        [TestMethod()]
        public void TestIsValid_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(ifac.GetMajorIsotope("C"), 1);
            formula.Add(ifac.GetMajorIsotope("H"), 100);
            formula.Charge = 0;

            List<IRule> rules = new List<IRule>
            {
                new MMElementRule(),
                new ChargeRule()
            };

            MolecularFormulaChecker MFChecker = new MolecularFormulaChecker(rules);

            Assert.AreEqual(0.0, MFChecker.IsValidSum(formula), 0.001);
        }

        [TestMethod()]
        public void TestIsValid_NOT_2Rules()
        {
            var formula = new MolecularFormula();
            formula.Add(ifac.GetMajorIsotope("C"), 1);
            formula.Add(ifac.GetMajorIsotope("H"), 100);
            formula.Charge = 0;

            List<IRule> rules = new List<IRule>
            {
                new MMElementRule(),
                new ChargeRule()
            };

            MolecularFormulaChecker MFChecker = new MolecularFormulaChecker(rules);

            IMolecularFormula formulaWith = MFChecker.IsValid(formula);

            Assert.AreEqual(0.0, formulaWith.GetProperty<double>((new MMElementRule()).GetType().ToString()));
            Assert.AreEqual(1.0, formulaWith.GetProperty<double>((new ChargeRule()).GetType().ToString()));
        }

        [TestMethod()]
        public void TestIsValidSum_True_2Rules()
        {
            var formula = new MolecularFormula();
            formula.Add(ifac.GetMajorIsotope("C"), 1);
            formula.Add(ifac.GetMajorIsotope("H"), 4);
            formula.Charge = 0;

            List<IRule> rules = new List<IRule>
            {
                new MMElementRule(),
                new ChargeRule()
            };

            MolecularFormulaChecker MFChecker = new MolecularFormulaChecker(rules);

            Assert.AreEqual(1.0, MFChecker.IsValidSum(formula), 0.001);
        }

        [TestMethod()]
        public void TestIsValid_True_2Rules()
        {
            var formula = new MolecularFormula();
            formula.Add(ifac.GetMajorIsotope("C"), 1);
            formula.Add(ifac.GetMajorIsotope("H"), 4);
            formula.Charge = 0;

            List<IRule> rules = new List<IRule>
            {
                new MMElementRule(),
                new ChargeRule()
            };

            MolecularFormulaChecker MFChecker = new MolecularFormulaChecker(rules);

            IMolecularFormula formulaWith = MFChecker.IsValid(formula);

            Assert.AreEqual(1.0, formulaWith.GetProperty<double>((new MMElementRule()).GetType().ToString()));
            Assert.AreEqual(1.0, formulaWith.GetProperty<double>((new ChargeRule()).GetType().ToString()));
        }
    }
}

