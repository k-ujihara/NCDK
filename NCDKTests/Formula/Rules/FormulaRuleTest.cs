/* Copyright (C) 2007-2008  Egon Willighagen <egonw@users.sf.net>
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
using System;

namespace NCDK.Formula.Rules
{
    /// <summary>
    /// Tests for formula restriction rules.
    /// </summary>
    // @cdk.module test-formula
    [TestClass()]
    public abstract class FormulaRuleTest : CDKTestCase
    {
        protected abstract Type RuleClass { get; }

        protected virtual IRule GetRule()
        {
            object rule = (object)RuleClass.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            if (!(rule is IRule))
            {
                throw new CDKException("The passed rule class must be a IRule");
            }
            return (IRule)rule;
        }

        /// <summary>
        /// Makes sure that the extending class has set the super.rule.
        /// </summary>
        /// <example>
        /// Each extending class should have this bit of code (JUnit4 formalism):
        /// <code>
        /// public static void SetUp() {
        ///   // Pass a Class, not an Object!
        ///   SetRule(typeof(SomeDescriptor));
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// The unit tests in the extending class may use this instance, but
        /// are not required.
        /// </remarks>
        [TestMethod()]
        public void TestHasSetSuperDotRule()
        {
            Assert.IsNotNull(GetRule());
        }

        [TestMethod()]
        public virtual void TestValidate_IMolecularFormula()
        {
            var rule = GetRule();
            IMolecularFormula mf = new MolecularFormula();
            mf.Add(new Isotope("C", 13));
            mf.Add(new Isotope("H", 2), 4);
            rule.Validate(mf);

            // can it handle an empty MF?
            rule.Validate(new MolecularFormula());
        }
    }
}
