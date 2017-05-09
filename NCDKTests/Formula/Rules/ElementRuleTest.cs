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
using NCDK.Default;

namespace NCDK.Formula.Rules
{
    // @cdk.module test-formula
    [TestClass()]
    public class ElementRuleTest : FormulaRuleTest
    {
        private static IChemObjectBuilder builder;

        [TestInitialize()]
        public void SetUp()
        {
            builder = Default.ChemObjectBuilder.Instance;
            SetRule(typeof(ElementRule));
        }

        [TestMethod()]
        public void TestElementRule()
        {
            IRule rule = new ElementRule();
            Assert.IsNotNull(rule);
        }

        [TestMethod()]
        public void TestDefault()
        {
            IRule rule = new ElementRule();
            var objects = rule.Parameters;

            // MolecularFormulaRange needs a build to create isotopes
            Assert.AreEqual(1, objects.Length);
            Assert.IsNull(objects[0]);

            // when we do a validation...
            rule.Validate(new MolecularFormula());

            // a default option is created
            objects = rule.Parameters;
            Assert.AreEqual(1, objects.Length);
            Assert.IsNotNull(objects[0]);

            MolecularFormulaRange mfRange = (MolecularFormulaRange)objects[0];
            Assert.AreEqual(93, mfRange.Count);
            Assert.AreEqual(0, mfRange.GetIsotopeCountMin(new Isotope("C")));
            Assert.AreEqual(50, mfRange.GetIsotopeCountMax(new Isotope("C")));
        }

        [TestMethod()]
        public void TestSetParameters()
        {
            IRule rule = new ElementRule();

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(builder.CreateIsotope("C"), 1, 10);
            mfRange.AddIsotope(builder.CreateIsotope("H"), 1, 10);

            rule.Parameters = new object[] { mfRange };

            var objects = rule.Parameters;
            Assert.AreEqual(1, objects.Length);

            MolecularFormulaRange mfRange2 = (MolecularFormulaRange)objects[0];
            Assert.AreEqual(mfRange.Count, mfRange2.Count);
            Assert.AreEqual(mfRange.GetIsotopeCountMin(new Isotope("C")), mfRange2.GetIsotopeCountMin(new Isotope("C")));
            Assert.AreEqual(mfRange.GetIsotopeCountMax(new Isotope("C")), mfRange2.GetIsotopeCountMax(new Isotope("C")));
        }

        [TestMethod()]
        public void TestDefaultValidFalse()
        {
            IRule rule = new ElementRule();

            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 2);
            formula.Add(builder.CreateIsotope("H"), 200);

            Assert.AreEqual(0.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestDefaultValidFalse_SetParam()
        {
            IRule rule = new ElementRule();

            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 2);
            formula.Add(builder.CreateIsotope("H"), 6);

            MolecularFormulaRange mfRange = new MolecularFormulaRange();
            mfRange.AddIsotope(builder.CreateIsotope("C"), 1, 2);
            mfRange.AddIsotope(builder.CreateIsotope("H"), 1, 2);

            rule.Parameters = new object[] { mfRange };

            Assert.AreEqual(0.0, rule.Validate(formula), 0.0001);
        }

        [TestMethod()]
        public void TestDefaultValidTrue()
        {
            IRule rule = new ElementRule();

            IMolecularFormula formula = new MolecularFormula();
            formula.Add(builder.CreateIsotope("C"), 2);
            formula.Add(builder.CreateIsotope("H"), 6);

            Assert.AreEqual(1.0, rule.Validate(formula), 0.0001);
        }
    }
}
