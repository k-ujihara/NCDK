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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NCDK
{
    /**
	 * Checks the functionality of {@link IMolecularFormulaSet} implementations.
	 *
	 * @cdk.module test-interfaces
	 */
	[TestClass()]
    public abstract class AbstractMolecularFormulaSetTest : CDKTestCase
    {
        protected abstract IChemObjectBuilder Builder { get; }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestSize()
        {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            mfS.Add(Builder.CreateMolecularFormula());
            Assert.AreEqual(1, mfS.Count());
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestAdd_IMolecularFormula() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());

            Assert.AreEqual(3, mfS.Count());
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestIterator() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());

            Assert.AreEqual(3, mfS.Count());
#if false // C# does not support remove item by enumertor
            IEnumerator<IMolecularFormula> iter = mfS.GetEnumerator();
            int count = 0;
            while (iter.MoveNext()) {
                iter.Current;
                ++count;
                iter.Remove();
            }
            Assert.AreEqual(0, mfS.Count());
            Assert.AreEqual(3, count);
            Assert.IsFalse(iter.MoveNext());
#endif
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestMolecularFormulas() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());

            Assert.AreEqual(3, mfS.Count());
            int count = 0;
            foreach (var formula in mfS) {
                ++count;
                Assert.IsNotNull(formula);
            }
            Assert.AreEqual(3, count);
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestAdd_IMolecularFormulaSet() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());

            IMolecularFormulaSet tested = Builder.CreateMolecularFormulaSet();
            Assert.AreEqual(0, tested.Count());
            tested.AddRange(mfS);
            Assert.AreEqual(3, tested.Count());
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestGetMolecularFormula_int() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());

            Assert.IsNotNull(mfS[2]); // third molecule should exist
                                                          //        Assert.IsNull(mfS[3]); // fourth molecule must not exist
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestAddMolecularFormula_IMolecularFormula() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());

            Assert.AreEqual(5, mfS.Count());

            // now test it to make sure it properly grows the array
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());

            Assert.AreEqual(7, mfS.Count());
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestGetMolecularFormulas() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();

            Assert.AreEqual(0, mfS.Count());

            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());
            mfS.Add(Builder.CreateMolecularFormula());

            Assert.AreEqual(3, mfS.Count());
            Assert.IsNotNull(mfS[0]);
            Assert.IsNotNull(mfS[1]);
            Assert.IsNotNull(mfS[2]);
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestContains_IMolecularFormula() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();

            IMolecularFormula mf = Builder.CreateMolecularFormula();
            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope h1 = Builder.CreateIsotope("H");
            IIsotope h2 = Builder.CreateIsotope("H");
            h2.ExactMass = 2.00055;

            mf.Add(carb);
            mf.Add(h1);

            mfS.Add(mf);

            Assert.IsTrue(mfS.Contains(mf));
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestClone() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            object clone = mfS.Clone();
            Assert.IsTrue(clone is IMolecularFormulaSet);
            Assert.AreNotSame(mfS, clone);
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestClone_IMolecualrFormula() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            IMolecularFormula mf1 = Builder.CreateMolecularFormula();
            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope flu = Builder.CreateIsotope("F");
            IIsotope h1 = Builder.CreateIsotope("H");
            mf1.Add(carb);
            mf1.Add(flu);
            mf1.Add(h1, 3);
            mfS.Add(mf1);

            IMolecularFormula mf2 = Builder.CreateMolecularFormula();
            IIsotope carb2 = Builder.CreateIsotope("C");
            IIsotope iode = Builder.CreateIsotope("I");
            IIsotope h2 = Builder.CreateIsotope("H");
            mf2.Add(carb2);
            mf2.Add(iode, 2);
            mf2.Add(h2, 2);
            mfS.Add(mf2);

            object clone = mfS.Clone();
            Assert.IsTrue(clone is IMolecularFormulaSet);
            Assert.AreNotSame(mfS, clone);
            Assert.AreEqual(mfS.Count(), ((IMolecularFormulaSet)clone).Count());
            Assert.AreEqual(mfS[0].Count, ((IMolecularFormulaSet)clone)
                    [0].Count);
            Assert.AreEqual(mfS[1].Count, ((IMolecularFormulaSet)clone)
                    [1].Count);
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestRemoveMolecularFormula_IMolecularFormula() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            IMolecularFormula mf1 = Builder.CreateMolecularFormula();
            IMolecularFormula mf2 = Builder.CreateMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);
            mfS.Remove(mf1);
            Assert.AreEqual(1, mfS.Count());
            Assert.AreEqual(mf2, mfS[0]);
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestRemoveAllMolecularFormulas() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            IMolecularFormula mf1 = Builder.CreateMolecularFormula();
            IMolecularFormula mf2 = Builder.CreateMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);

            Assert.AreEqual(2, mfS.Count());
            mfS.Clear();
            Assert.AreEqual(0, mfS.Count());
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestRemoveMolecularFormula_int() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            IMolecularFormula mf1 = Builder.CreateMolecularFormula();
            IMolecularFormula mf2 = Builder.CreateMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);
            mfS.RemoveAt(0);
            Assert.AreEqual(1, mfS.Count());
            Assert.AreEqual(mf2, mfS[0]);
        }

        /**
		 * A unit test suite for JUnit.
		 *
		 * @return    The test suite
		 */
        [TestMethod()]
        public virtual void TestRePlaceMolecularFormula_int_IMolecularFormula() {
            IMolecularFormulaSet mfS = Builder.CreateMolecularFormulaSet();
            IMolecularFormula mf1 = Builder.CreateMolecularFormula();
            IMolecularFormula mf2 = Builder.CreateMolecularFormula();
            IMolecularFormula mf3 = Builder.CreateMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);
            Assert.AreEqual(mf2, mfS[1]);
            mfS.RemoveAt(1);
            mfS.Add(mf3);
            Assert.AreEqual(mf3, mfS[1]);
        }

        [TestMethod()]
        public virtual void TestGetBuilder() {
            IMolecularFormulaSet add = Builder.CreateMolecularFormulaSet();
            IChemObjectBuilder builder = add.Builder;
            Assert.IsNotNull(builder);
            Assert.AreEqual(Builder.GetType().Name, builder.GetType().Name);
        }
    }
}