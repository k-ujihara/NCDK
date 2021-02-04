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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IMolecularFormulaSet"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    public abstract class AbstractMolecularFormulaSetTest 
        : CDKTestCase
    {
        protected abstract IChemObjectBuilder Builder { get; }

        [TestMethod()]
        public virtual void TestSize()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            mfS.Add(Builder.NewMolecularFormula());
            Assert.AreEqual(1, mfS.Count);
        }

        [TestMethod()]
        public virtual void TestAdd_IMolecularFormula()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            Assert.AreEqual(3, mfS.Count);
        }

        [TestMethod()]
        public virtual void TestIterator()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            Assert.AreEqual(3, mfS.Count);
#if false // C# does not support remove item by enumertor
            IEnumerator<IMolecularFormula> iter = mfS.GetEnumerator();
            int count = 0;
            while (iter.MoveNext()) {
                iter.Current;
                ++count;
                iter.Remove();
            }
            Assert.AreEqual(0, mfS.Count);
            Assert.AreEqual(3, count);
            Assert.IsFalse(iter.MoveNext());
#endif
        }

        [TestMethod()]
        public virtual void TestMolecularFormulas()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            Assert.AreEqual(3, mfS.Count);
            int count = 0;
            foreach (var formula in mfS)
            {
                ++count;
                Assert.IsNotNull(formula);
            }
            Assert.AreEqual(3, count);
        }

        [TestMethod()]
        public virtual void TestAdd_IMolecularFormulaSet()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            var tested = Builder.NewMolecularFormulaSet();
            Assert.AreEqual(0, tested.Count);
            tested.AddRange(mfS);
            Assert.AreEqual(3, tested.Count);
        }

        [TestMethod()]
        public virtual void TestGetMolecularFormula_int()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            Assert.IsNotNull(mfS[2]); // third molecule should exist
            // Assert.IsNull(mfS[3]); // fourth molecule must not exist
        }

        [TestMethod()]
        public virtual void TestAddMolecularFormula_IMolecularFormula()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            Assert.AreEqual(5, mfS.Count);

            // now test it to make sure it properly grows the array
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            Assert.AreEqual(7, mfS.Count);
        }

        [TestMethod()]
        public virtual void TestGetMolecularFormulas()
        {
            var mfS = Builder.NewMolecularFormulaSet();

            Assert.AreEqual(0, mfS.Count);

            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            Assert.AreEqual(3, mfS.Count);
            Assert.IsNotNull(mfS[0]);
            Assert.IsNotNull(mfS[1]);
            Assert.IsNotNull(mfS[2]);
        }

        [TestMethod()]
        public virtual void TestContains_IMolecularFormula()
        {
            var mfS = Builder.NewMolecularFormulaSet();

            var mf = Builder.NewMolecularFormula();
            var carb = Builder.NewIsotope("C");
            var h1 = Builder.NewIsotope("H");
            var h2 = Builder.NewIsotope("H");
            h2.ExactMass = 2.00055;

            mf.Add(carb);
            mf.Add(h1);

            mfS.Add(mf);

            Assert.IsTrue(mfS.Contains(mf));
        }

        [TestMethod()]
        public virtual void TestClone()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            object clone = mfS.Clone();
            Assert.IsTrue(clone is IMolecularFormulaSet);
            Assert.AreNotSame(mfS, clone);
        }

        [TestMethod()]
        public virtual void TestClone_IMolecualrFormula()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            var mf1 = Builder.NewMolecularFormula();
            var carb = Builder.NewIsotope("C");
            var flu = Builder.NewIsotope("F");
            var h1 = Builder.NewIsotope("H");
            mf1.Add(carb);
            mf1.Add(flu);
            mf1.Add(h1, 3);
            mfS.Add(mf1);

            var mf2 = Builder.NewMolecularFormula();
            var carb2 = Builder.NewIsotope("C");
            var iode = Builder.NewIsotope("I");
            var h2 = Builder.NewIsotope("H");
            mf2.Add(carb2);
            mf2.Add(iode, 2);
            mf2.Add(h2, 2);
            mfS.Add(mf2);

            object clone = mfS.Clone();
            Assert.IsTrue(clone is IMolecularFormulaSet);
            Assert.AreNotSame(mfS, clone);
            Assert.AreEqual(mfS.Count, ((IMolecularFormulaSet)clone).Count);
            Assert.AreEqual(mfS[0].IsotopesCount, ((IMolecularFormulaSet)clone)[0].IsotopesCount);
            Assert.AreEqual(mfS[1].IsotopesCount, ((IMolecularFormulaSet)clone)[1].IsotopesCount);
        }

        [TestMethod()]
        public virtual void TestRemoveMolecularFormula_IMolecularFormula()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            var mf1 = Builder.NewMolecularFormula();
            var mf2 = Builder.NewMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);
            mfS.Remove(mf1);
            Assert.AreEqual(1, mfS.Count);
            Assert.AreEqual(mf2, mfS[0]);
        }

        [TestMethod()]
        public virtual void TestRemoveAllMolecularFormulas()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            var mf1 = Builder.NewMolecularFormula();
            var mf2 = Builder.NewMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);

            Assert.AreEqual(2, mfS.Count);
            mfS.Clear();
            Assert.AreEqual(0, mfS.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveMolecularFormula_int()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            var mf1 = Builder.NewMolecularFormula();
            var mf2 = Builder.NewMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);
            mfS.RemoveAt(0);
            Assert.AreEqual(1, mfS.Count);
            Assert.AreEqual(mf2, mfS[0]);
        }

        [TestMethod()]
        public virtual void TestReplaceMolecularFormula_int_IMolecularFormula()
        {
            var mfS = Builder.NewMolecularFormulaSet();
            var mf1 = Builder.NewMolecularFormula();
            var mf2 = Builder.NewMolecularFormula();
            var mf3 = Builder.NewMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);
            Assert.AreEqual(mf2, mfS[1]);
            mfS.RemoveAt(1);
            mfS.Add(mf3);
            Assert.AreEqual(mf3, mfS[1]);
        }

        [TestMethod()]
        public virtual void TestGetBuilder()
        {
            var add = Builder.NewMolecularFormulaSet();
            var builder = add.Builder;
            Assert.IsNotNull(builder);
            Assert.AreEqual(Builder.GetType().Name, builder.GetType().Name);
        }
    }
}
