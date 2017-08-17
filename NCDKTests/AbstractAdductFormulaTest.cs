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
using System.Collections.Generic;
using System.Linq;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IAdductFormula"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractAdductFormulaTest : AbstractMolecularFormulaSetTest
    {
        [TestMethod()]
        public override void TestSize()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
            Assert.AreEqual(0, mfS.Count);
        }

        [TestMethod()]
        public virtual void TestAddIMolecularFormula()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            Assert.AreEqual(3, mfS.Count);
        }

        [TestMethod()]

        public override void TestAdd_IMolecularFormulaSet()
        {
            IAdductFormula adduct = Builder.NewAdductFormula();
            IMolecularFormulaSet mfSet = Builder.NewMolecularFormulaSet();
            mfSet.Add(Builder.NewMolecularFormula());
            mfSet.Add(Builder.NewMolecularFormula());
            mfSet.Add(Builder.NewMolecularFormula());
            adduct.AddRange(mfSet);

            Assert.AreEqual(3, adduct.Count);
        }

        [TestMethod()]

        public override void TestIterator()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
        }

        [TestMethod()]
        public override void TestMolecularFormulas()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
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
        public virtual void TestAdd_IAdductFormula()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            IAdductFormula tested = Builder.NewAdductFormula();
            Assert.AreEqual(0, tested.Count);
            tested.AddRange(mfS);
            Assert.AreEqual(3, tested.Count);
        }

        [TestMethod()]
        public override void TestGetMolecularFormula_int()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());
            mfS.Add(Builder.NewMolecularFormula());

            Assert.IsNotNull(mfS[2]); // third molecule should exist
        }

        [TestMethod()]
        public override void TestAddMolecularFormula_IMolecularFormula()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
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
        public virtual void TestGetMolecularFormulas_int()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();

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
        public virtual void TestContains_IIsotope()
        {
            IAdductFormula add = Builder.NewAdductFormula();

            IMolecularFormula mf = Builder.NewMolecularFormula();
            IIsotope carb = Builder.NewIsotope("C");
            IIsotope h1 = Builder.NewIsotope("H");
            IIsotope h2 = Builder.NewIsotope("H");
            h2.ExactMass = 2.00055;

            mf.Add(carb);
            mf.Add(h1);

            add.Add(mf);

            Assert.IsTrue(mf.Contains(carb));
            Assert.IsTrue(mf.Contains(h1));
            Assert.IsFalse(mf.Contains(h2));
        }

        [TestMethod()]
        public override void TestContains_IMolecularFormula()
        {
            IAdductFormula add = Builder.NewAdductFormula();

            IMolecularFormula mf = Builder.NewMolecularFormula();
            IIsotope carb = Builder.NewIsotope("C");
            IIsotope h1 = Builder.NewIsotope("H");
            IIsotope h2 = Builder.NewIsotope("H");
            h2.ExactMass = 2.00055;

            mf.Add(carb);
            mf.Add(h1);

            add.Add(mf);

            Assert.IsTrue(add.Contains(mf));
        }

        [TestMethod()]
        public virtual void TestGetCharge()
        {
            IAdductFormula add = Builder.NewAdductFormula();
            IMolecularFormula mf1 = Builder.NewMolecularFormula();
            mf1.Charge = 1;
            add.Add(mf1);

            Assert.AreEqual(1.0, add.Charge.Value, 0.01);
        }

        [TestMethod()]
        public virtual void TestSetCharge_Integer()
        {
            TestGetCharge();
        }

        [TestMethod()]
        public override void TestClone()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
            object clone = mfS.Clone();
            Assert.IsTrue(clone is IAdductFormula);
            Assert.AreNotSame(mfS, clone);
        }

        [TestMethod()]
        public override void TestRemoveMolecularFormula_IMolecularFormula()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
            IMolecularFormula mf1 = Builder.NewMolecularFormula();
            IMolecularFormula mf2 = Builder.NewMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);
            mfS.Remove(mf1);
            Assert.AreEqual(1, mfS.Count);
            Assert.AreEqual(mf2, mfS[0]);
        }

        [TestMethod()]
        public override void TestRemoveAllMolecularFormulas()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
            IMolecularFormula mf1 = Builder.NewMolecularFormula();
            IMolecularFormula mf2 = Builder.NewMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);

            Assert.AreEqual(2, mfS.Count);
            mfS.Clear();
            Assert.AreEqual(0, mfS.Count);
        }

        [TestMethod()]
        public override void TestRemoveMolecularFormula_int()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
            IMolecularFormula mf1 = Builder.NewMolecularFormula();
            IMolecularFormula mf2 = Builder.NewMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);
            mfS.RemoveAt(0);
            Assert.AreEqual(1, mfS.Count);
            Assert.AreEqual(mf2, mfS[0]);
        }

        [TestMethod()]
        public override void TestReplaceMolecularFormula_int_IMolecularFormula()
        {
            IAdductFormula mfS = Builder.NewAdductFormula();
            IMolecularFormula mf1 = Builder.NewMolecularFormula();
            IMolecularFormula mf2 = Builder.NewMolecularFormula();
            IMolecularFormula mf3 = Builder.NewMolecularFormula();
            mfS.Add(mf1);
            mfS.Add(mf2);
            Assert.AreEqual(mf2, mfS[1]);
            mfS.RemoveAt(1);
            mfS.Add(mf3);
            Assert.AreEqual(mf3, mfS[1]);
        }

        [TestMethod()]
        public virtual void TestGetIsotopeCount()
        {
            IAdductFormula add = Builder.NewAdductFormula();
            Assert.AreEqual(0, add.GetIsotopes().Count());

            IMolecularFormula formula = Builder.NewMolecularFormula();
            formula.Add(Builder.NewIsotope("C"));
            formula.Add(Builder.NewIsotope("H"), 4);

            add.Add(formula);

            Assert.AreEqual(2, add.GetIsotopes().Count());
        }

        [TestMethod()]
        public virtual void TestIsotopes()
        {
            IAdductFormula add = Builder.NewAdductFormula();

            IMolecularFormula formula1 = Builder.NewMolecularFormula();
            formula1.Add(Builder.NewIsotope("C"));
            formula1.Add(Builder.NewIsotope("H"), 4);

            IMolecularFormula formula2 = Builder.NewMolecularFormula();
            formula2.Add(Builder.NewIsotope("F"));

            add.Add(formula1);
            add.Add(formula2);

            int count = 0;
            IEnumerator<IIsotope> it = add.GetIsotopes().GetEnumerator();
            while (it.MoveNext())
            {
                ++count;
            }
            Assert.AreEqual(3, count);
        }

        [TestMethod()]
        public virtual void TestGetIsotopeCount_Sum()
        {
            IAdductFormula add = Builder.NewAdductFormula();
            Assert.AreEqual(0, add.GetIsotopes().Count());

            IMolecularFormula adduct1 = Builder.NewMolecularFormula();
            adduct1.Add(Builder.NewIsotope("C"));
            IIsotope h = Builder.NewIsotope("H");
            adduct1.Add(h, 4);
            add.Add(adduct1);

            IMolecularFormula formula = Builder.NewMolecularFormula();
            formula.Add(h);
            add.Add(adduct1);

            Assert.AreEqual(2, add.GetIsotopes().Count());
        }

        [TestMethod()]
        public virtual void TestGetIsotopeCount_IIsotope()
        {
            IAdductFormula add = Builder.NewAdductFormula();
            Assert.AreEqual(0, add.GetIsotopes().Count());

            IMolecularFormula formula = Builder.NewMolecularFormula();
            IIsotope C = Builder.NewIsotope("C");
            formula.Add(C);
            IIsotope h = Builder.NewIsotope("H");
            formula.Add(h, 4);

            add.Add(formula);

            Assert.AreEqual(2, formula.Isotopes.Count());
            Assert.AreEqual(2, add.GetIsotopes().Count());
            Assert.AreEqual(1, add.GetCount(C));
            Assert.AreEqual(4, add.GetCount(h));
        }

        [TestMethod()]
        public virtual void TestGetIsotopeCount_Sum_Isotope()
        {

            IAdductFormula add = Builder.NewAdductFormula();
            Assert.AreEqual(0, add.GetIsotopes().Count());

            IMolecularFormula adduct1 = Builder.NewMolecularFormula();
            IIsotope C = Builder.NewIsotope("C");
            adduct1.Add(C);
            IIsotope h = Builder.NewIsotope("H");
            adduct1.Add(h, 4);
            add.Add(adduct1);

            IMolecularFormula adduct2 = Builder.NewMolecularFormula();
            adduct2.Add(h);
            add.Add(adduct2);

            Assert.AreEqual(1, add.GetCount(C));
            Assert.AreEqual(5, add.GetCount(h));
        }

        [TestMethod()]
        public override void TestGetBuilder()
        {
            IAdductFormula add = Builder.NewAdductFormula();
            IChemObjectBuilder builder = add.Builder;
            Assert.IsNotNull(builder);
            Assert.AreEqual(Builder.GetType().Name, builder.GetType().Name);
        }
    }
}
