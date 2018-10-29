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
using NCDK.Silent;
using System;
using System.Collections.Generic;

namespace NCDK.Formula.Rules
{
    // @cdk.module test-formula
    [TestClass()]
    public class IsotopePatternRuleTest : FormulaRuleTest
    {
        private static readonly IChemObjectBuilder builder = ChemObjectBuilder.Instance;
        private static readonly IsotopeFactory ifac = BODRIsotopeFactory.Instance;
        protected override Type RuleClass => typeof(IsotopePatternRule);

        protected override IRule GetRule()
        {
            var spectrum = new List<double[]>
            {
                new double[] { 133.0977, 100.00 },
                new double[] { 134.09475, 0.6 },
                new double[] { 134.1010, 5.4 }
            };
            var parameters = new object[2];
            parameters[0] = spectrum;
            parameters[1] = 0.001;
            var rule = new IsotopePatternRule { Parameters = parameters };
            return rule;
        }

        [TestMethod()]
        public void TestIsotopePatternRule()
        {
            var rule = new IsotopePatternRule();
            Assert.IsNotNull(rule);
        }

        [TestMethod()]
        public void TestDefault()
        {
            var rule = new IsotopePatternRule();
            var objects = rule.Parameters;

            Assert.IsNull(objects[0]);
        }

        [TestMethod()]
        public void TestSetParameters()
        {
            var rule = new IsotopePatternRule();

            var parameters = new object[2];

            parameters[0] = new List<double[]>();
            parameters[1] = 0.0001;
            rule.Parameters = parameters;

            var objects = rule.Parameters;

            Assert.IsNotNull(objects[0]);
            Assert.AreEqual(2, objects.Count);
        }

        [TestMethod()]
        public void TestValidBromine()
        {
            var spectrum = new List<double[]>
            {
                new double[] { 157.8367, 51.399 },
                new double[] { 159.8346, 100.00 },
                new double[] { 161.8326, 48.639 }
            };

            var rule = new IsotopePatternRule();
            var parameters = new object[2];
            parameters[0] = spectrum;
            parameters[1] = 0.001;
            rule.Parameters = parameters;

            var formula = new MolecularFormula();
            formula.Add(ifac.GetMajorIsotope("C"), 2);
            formula.Add(ifac.GetMajorIsotope("Br"), 2);
            formula.Charge = 0;

            Assert.AreEqual(0.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public override void TestValidate_IMolecularFormula()
        {
            var formula = new MolecularFormula();
            formula.Add(ifac.GetMajorIsotope("C"), 5);
            formula.Add(ifac.GetMajorIsotope("H"), 13);
            formula.Add(ifac.GetMajorIsotope("N"), 2);
            formula.Add(ifac.GetMajorIsotope("O"), 2);
            formula.Charge = 0;

            // experimental results

            var spectrum = new List<double[]>
            {
                new double[] { 133.0977, 100.00 },
                new double[] { 134.09475, 0.6 },
                new double[] { 134.1010, 5.4 }
            };

            var rule = new IsotopePatternRule();
            var parameters = new object[2];
            parameters[0] = spectrum;
            parameters[1] = 0.001;
            rule.Parameters = parameters;

            Assert.AreEqual(0.9433, rule.Validate(formula), 0.001);
        }
    }
}
