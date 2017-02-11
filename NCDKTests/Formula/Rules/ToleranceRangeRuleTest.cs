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
    public class ToleranceRangeRuleTest : FormulaRuleTest
    {

        private static IChemObjectBuilder builder;

        [TestInitialize()]
        public void SetUp()
        {
            builder = Default.ChemObjectBuilder.Instance;
            SetRule(typeof(ToleranceRangeRule));
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestToleranceRangeRule()
        {

            IRule rule = new ToleranceRangeRule();
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

            IRule rule = new ToleranceRangeRule();
            var objects = rule.Parameters;
            Assert.AreEqual(2, objects.Length);

            double mass = (double)objects[0];
            Assert.AreEqual(0.0, mass, 0.00001);
            double tolerance = (double)objects[1];
            Assert.AreEqual(0.05, tolerance, 0.00001);

        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestSetParameters()
        {

            IRule rule = new ToleranceRangeRule();

            object[] parameters = new object[2];
            parameters[0] = 133.0;
            parameters[1] = 0.00005;
            rule.Parameters = parameters;

            var objects = rule.Parameters;

            Assert.AreEqual(2, objects.Length);

            double mass = (double)objects[0];
            Assert.AreEqual(133.0, mass, 0.00001);
            double tolerance = (double)objects[1];
            Assert.AreEqual(0.00005, tolerance, 0.00001);

        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestDefaultValidFalse()
        {

            IRule rule = new ToleranceRangeRule();

            IMolecularFormula formula = new MolecularFormula();
            IIsotope carb = builder.CreateIsotope("C");
            carb.ExactMass = 12.00;
            IIsotope cl = builder.CreateIsotope("Cl");
            cl.ExactMass = 34.96885268;
            formula.Add(carb);
            formula.Add(cl);

            Assert.AreEqual(0.0, rule.Validate(formula), 0.0001);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestDefaultValidFalse_SetParam()
        {

            IRule rule = new ToleranceRangeRule();

            IMolecularFormula formula = new MolecularFormula();
            IIsotope carb = builder.CreateIsotope("C");
            carb.ExactMass = 12.00;
            IIsotope cl = builder.CreateIsotope("Cl");
            cl.ExactMass = 34.96885268;
            formula.Add(carb);
            formula.Add(cl);

            object[] parameters = new object[2];
            parameters[0] = 46.0; // real -> 46.96885268
            parameters[1] = 0.00005;
            rule.Parameters = parameters;

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

            IRule rule = new ToleranceRangeRule();

            IMolecularFormula formula = new MolecularFormula();
            IIsotope carb = builder.CreateIsotope("C");
            carb.ExactMass = 12.00;
            IIsotope cl = builder.CreateIsotope("Cl");
            cl.ExactMass = 34.96885268;
            formula.Add(carb);
            formula.Add(cl);

            object[] parameters = new object[2];
            parameters[0] = 46.96885268;
            parameters[1] = 0.00005;
            rule.Parameters = parameters;

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }
    }
}
