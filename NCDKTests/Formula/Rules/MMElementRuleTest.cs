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
using System;

namespace NCDK.Formula.Rules
{
    // @cdk.module test-formula
    [TestClass()]
    public class MMElementRuleTest : FormulaRuleTest
    {
        private static readonly IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
        protected override Type RuleClass => typeof(MMElementRule);

        [TestMethod()]
        public void TestMMElementRule()
        {
            IRule rule = new MMElementRule();
            Assert.IsNotNull(rule);
        }

        [TestMethod()]
        public void TestDefault()
        {
            IRule rule = new MMElementRule();
            object[] objects = rule.Parameters;

            Assert.AreSame(MMElementRule.Database.Wiley, objects[0]);
            Assert.AreSame(MMElementRule.RangeMass.Minus500, objects[1]);
        }

        [TestMethod()]
        public void TestSetParameters()
        {
            IRule rule = new MMElementRule();

            object[] params_ = new object[2];

            params_[0] = MMElementRule.Database.DictionaryNaturalProductsOnline;
            params_[1] = MMElementRule.RangeMass.Minus1000;

            rule.Parameters = params_;
            object[] objects = rule.Parameters;

            Assert.AreSame(MMElementRule.Database.DictionaryNaturalProductsOnline, objects[0]);
            Assert.AreSame(MMElementRule.RangeMass.Minus1000, objects[1]);
        }

        [TestMethod()]
        public void TestDefaultValidFalse()
        {
            IRule rule = new MMElementRule();

            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 2);
            formula.Add(builder.NewIsotope("H"), 200);

            Assert.AreEqual(0.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestDefaultValidTrue()
        {
            IRule rule = new MMElementRule();

            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.NewIsotope("C"), 2);
            formula.Add(builder.NewIsotope("H"), 6);

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }
    }
}
