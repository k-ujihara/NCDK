/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK;
using NCDK.Tools.Diff.Tree;

namespace NCDK
{
    /**
     * Checks the functionality of {@link IElement} implementations.
     *
     * @cdk.module test-interfaces
     *
     * @see org.openscience.cdk.Element
     */
    [TestClass()]
    public abstract class AbstractElementTest : AbstractChemObjectTest
    {

        // test methods

        [TestMethod()]
        public virtual void TestSetSymbol_String()
        {
            IElement e = (IElement)NewChemObject();
            e.Symbol = "C";
            Assert.AreEqual("C", e.Symbol);
        }

        [TestMethod()]
        public virtual void TestGetSymbol()
        {
            IElement e = (IElement)NewChemObject();
            e.Symbol = "X";
            Assert.AreEqual("X", e.Symbol);
        }

        [TestMethod()]
        public virtual void TestSetAtomicNumber_Integer()
        {
            IElement e = (IElement)NewChemObject();
            e.AtomicNumber = 1;
            Assert.AreEqual(1, e.AtomicNumber.Value);
        }

        [TestMethod()]
        public virtual void TestGetAtomicNumber()
        {
            IElement e = (IElement)NewChemObject();
            e.AtomicNumber = 1;
            Assert.AreEqual(1, e.AtomicNumber.Value);
        }

        [TestMethod()]

        public override void TestClone()
        {
            IElement elem = (IElement)NewChemObject();
            object clone = elem.Clone();
            Assert.IsTrue(clone is IElement);

            // test that everything has been cloned properly
            string diff = ElementDiff.Diff(elem, (IElement)clone);
            Assert.IsNotNull(diff);
            Assert.AreEqual(0, diff.Length);
        }

        [TestMethod()]
        public virtual void TestCloneDiff()
        {
            IElement elem = (IElement)NewChemObject();
            IElement clone = (IElement)elem.Clone();
            Assert.AreEqual("", ElementDiff.Diff(elem, clone));
        }

        [TestMethod()]
        public virtual void TestClone_Symbol()
        {
            IElement elem = (IElement)NewChemObject();
            elem.Symbol = "C";
            IElement clone = (IElement)elem.Clone();

            // test cloning of symbol
            elem.Symbol = "H";
            Assert.AreEqual("C", clone.Symbol);
        }

        [TestMethod()]
        public virtual void TestClone_IAtomicNumber()
        {
            IElement elem = (IElement)NewChemObject();
            elem.AtomicNumber = 6;
            IElement clone = (IElement)elem.Clone();

            // test cloning of atomic number
            elem.AtomicNumber = 5; // don't care about symbol
            Assert.AreEqual(6, clone.AtomicNumber.Value);
        }

        /// <summary>Test for RFC #9</summary>
        [TestMethod()]
        public virtual void TestToString()
        {
            IElement elem = (IElement)NewChemObject();
            string description = elem.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }
    }
}