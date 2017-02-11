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

namespace NCDK.Formula.Rules
{
    /**
	 * @cdk.module test-formula
	 */
    [TestClass()]
    public class MMElementRuleTest : FormulaRuleTest
    {
        private static IChemObjectBuilder builder;

        [TestInitialize()]
        public void SetUp()
        {
            builder = Default.ChemObjectBuilder.Instance;
            SetRule(typeof(MMElementRule));
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public void TestMMElementRule()
        {

            IRule rule = new MMElementRule();
            Assert.IsNotNull(rule);

        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public void TestDefault()
        {

            IRule rule = new MMElementRule();
            object[] objects = rule.Parameters;

            Assert.AreSame(MMElementRule.Database.WILEY, objects[0]);
            Assert.AreSame(MMElementRule.RangeMass.Minus500, objects[1]);

        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public void TestSetParameters()
        {

            IRule rule = new MMElementRule();

            object[] params_ = new object[2];

            params_[0] = MMElementRule.Database.DNP;
            params_[1] = MMElementRule.RangeMass.Minus1000;

            rule.Parameters = params_;
            object[] objects = rule.Parameters;

            Assert.AreSame(MMElementRule.Database.DNP, objects[0]);
            Assert.AreSame(MMElementRule.RangeMass.Minus1000, objects[1]);
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public void TestDefaultValidFalse()
        {

            IRule rule = new MMElementRule();

            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 2);
            formula.Add(builder.CreateIsotope("H"), 200);

            Assert.AreEqual(0.0, rule.Validate(formula), 0.0001);
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public void TestDefaultValidTrue()
        {

            IRule rule = new MMElementRule();

            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 2);
            formula.Add(builder.CreateIsotope("H"), 6);

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }
    }
}
