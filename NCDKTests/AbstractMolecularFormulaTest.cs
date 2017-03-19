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
    /// Checks the functionality of <see cref="IMolecularFormula"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractMolecularFormulaTest : CDKTestCase
    {
        protected abstract IChemObjectBuilder Builder { get; }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public virtual void TestGetIsotopeCount0()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();

            Assert.AreEqual(0, mf.Count);
        }

        [TestMethod()]
        public virtual void TestGetIsotopeCount()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            mf.Add(Builder.CreateIsotope("C"));
            mf.Add(Builder.CreateIsotope("H"));
            mf.Add(Builder.CreateIsotope("H"));
            mf.Add(Builder.CreateIsotope("H"));
            mf.Add(Builder.CreateIsotope("H"));

            Assert.AreEqual(2, mf.Count);
        }

        [TestMethod()]
        public virtual void TestAddIsotope_IIsotope()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            mf.Add(Builder.CreateIsotope("C"));
            mf.Add(Builder.CreateIsotope("H"));

            IIsotope hy = Builder.CreateIsotope("C");
            hy.NaturalAbundance = 2.00342342;
            mf.Add(hy);

            Assert.AreEqual(3, mf.Count);
        }

        [TestMethod()]
        public virtual void TestGetIsotopeCount_IIsotope()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();

            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope flu = Builder.CreateIsotope("F");
            IIsotope h1 = Builder.CreateIsotope("H");
            IIsotope h2 = Builder.CreateIsotope("H");
            IIsotope h3 = Builder.CreateIsotope("H");
            mf.Add(carb);
            mf.Add(flu);
            mf.Add(h1);
            mf.Add(h2);
            mf.Add(h3);

            Assert.AreEqual(3, mf.Count);
            Assert.AreEqual(1, mf.GetCount(carb));
            Assert.AreEqual(1, mf.GetCount(flu));
            Assert.AreEqual(3, mf.GetCount(h1));
        }

        [TestMethod()]
        public virtual void TestGetIsotopeCount_IIsotope2()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();

            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope flu = Builder.CreateIsotope("F");
            IIsotope h1 = Builder.CreateIsotope("H");
            mf.Add(carb);
            mf.Add(flu);
            mf.Add(h1);
            mf.Add(h1);
            mf.Add(h1);

            Assert.AreEqual(3, mf.Count);
            Assert.AreEqual(1, mf.GetCount(carb));
            Assert.AreEqual(1, mf.GetCount(flu));
            Assert.AreEqual(3, mf.GetCount(h1));
        }

        [TestMethod()]
        public virtual void TestAddIsotope_IIsotope_int()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();

            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope flu = Builder.CreateIsotope("F");
            IIsotope h1 = Builder.CreateIsotope("H");
            mf.Add(carb);
            mf.Add(flu);
            mf.Add(h1, 3);

            Assert.AreEqual(3, mf.Count);
            Assert.AreEqual(1, mf.GetCount(carb));
            Assert.AreEqual(1, mf.GetCount(flu));
            Assert.AreEqual(3, mf.GetCount(h1));
            // In a List the objects are not stored in the same order than called
            //        Assert.AreEqual("C", mf.Isotopes[0].Symbol);
            //        Assert.AreEqual("F", mf.Isotopes[1].Symbol);
            //        Assert.AreEqual("H", mf.Isotopes[2].Symbol);
        }

        [TestMethod()]
        public virtual void TestGetIsotope_Number_Clone()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();

            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope flu = Builder.CreateIsotope("F");
            IIsotope h1 = Builder.CreateIsotope("H");
            mf.Add(carb);
            mf.Add(flu);
            mf.Add(h1, 3);

            object clone = mf.Clone();
            Assert.IsTrue(clone is IMolecularFormula);

            IMolecularFormula cloneFormula = (IMolecularFormula)clone;

            Assert.AreEqual(1, cloneFormula.GetCount(carb));
            Assert.AreEqual(1, cloneFormula.GetCount(flu));
            Assert.AreEqual(3, cloneFormula.GetCount(h1));
            // In a List the objects are not stored in the same order than called
            //        Assert.AreEqual("C", cloneFormula.Isotopes[0].Symbol);
            //        Assert.AreEqual("F", cloneFormula.Isotopes[1].Symbol);
            //        Assert.AreEqual("H", cloneFormula.Isotopes[2].Symbol);
        }

        [TestMethod()]
        public virtual void TestGetIsotopeCount_IIsotope_Occurr()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();

            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope flu = Builder.CreateIsotope("F");
            IIsotope h1 = Builder.CreateIsotope("H");
            mf.Add(carb);
            mf.Add(flu);
            mf.Add(h1, 3);

            Assert.AreEqual(3, mf.Count);
            Assert.AreEqual(1, mf.GetCount(carb));
            Assert.AreEqual(1, mf.GetCount(flu));
            Assert.AreEqual(3, mf.GetCount(h1));
        }

        [TestMethod()]
        public virtual void TestAdd_IMolecularFormula()
        {
            IMolecularFormula acetone = Builder.CreateMolecularFormula();
            acetone.Add(Builder.CreateIsotope("C"), 3);
            IIsotope oxig = Builder.CreateIsotope("O");
            acetone.Add(oxig);

            Assert.AreEqual(2, acetone.Count);

            IMolecularFormula water = Builder.CreateMolecularFormula();
            water.Add(Builder.CreateIsotope("H"), 2);
            water.Add(oxig);
            acetone.Add(water);

            Assert.AreEqual(3, acetone.Count);
        }

        [TestMethod()]
        public virtual void TestMolecularFormula_NullCharge()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            IMolecularFormula mf2 = Builder.CreateMolecularFormula();
            mf2.Charge = 0;
            mf.Add(mf2);
        }

        [TestMethod()]
        public virtual void TestIsotopes()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            mf.Add(Builder.CreateIsotope("C"));
            mf.Add(Builder.CreateIsotope("F"));
            mf.Add(Builder.CreateIsotope("H"), 3);

            IEnumerator<IIsotope> istoIter = mf.Isotopes.GetEnumerator();
            int counter = 0;
            while (istoIter.MoveNext())
            {
                counter++;
            }
            Assert.AreEqual(3, counter);
        }

        [TestMethod()]
        public virtual void TestContains_IIsotope()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();

            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope h1 = Builder.CreateIsotope("H");
            IIsotope h2 = Builder.CreateIsotope("H");
            h2.ExactMass = 2.0004;

            mf.Add(carb);
            mf.Add(h1);

            Assert.IsTrue(mf.Contains(carb));
            Assert.IsTrue(mf.Contains(h1));
            Assert.IsFalse(mf.Contains(h2));
        }

        [TestMethod()]
        public virtual void TestInstance_IIsotope()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();

            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope flu = Builder.CreateIsotope("F");
            IIsotope h1 = Builder.CreateIsotope("H");
            mf.Add(carb);
            mf.Add(flu);
            mf.Add(h1, 3);

            IEnumerator<IIsotope> istoIter = mf.Isotopes.GetEnumerator();
            Assert.IsNotNull(istoIter);
            Assert.IsTrue(istoIter.MoveNext());
            IIsotope next = istoIter.Current;
            Assert.IsTrue(next is IIsotope);
            //        Assert.AreEqual(carb, next);

            Assert.IsTrue(istoIter.MoveNext());
            next = istoIter.Current;
            Assert.IsTrue(next is IIsotope);
            //        Assert.AreEqual(flu, next);

            Assert.IsTrue(istoIter.MoveNext());
            next = istoIter.Current;
            Assert.IsTrue(next is IIsotope);
            //        Assert.AreEqual(h1, next);

            Assert.IsFalse(istoIter.MoveNext());
        }

        [TestMethod()]
        public virtual void TestGetCharge()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            mf.Charge = 1;
            mf.Add(Builder.CreateAtom("C"));
            mf.Add(Builder.CreateAtom("F"));
            mf.Add(Builder.CreateAtom("H"), 3);

            Assert.AreEqual(3, mf.Count);
            Assert.AreEqual(1.0, mf.Charge.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestSetCharge_Double()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            Assert.AreEqual(null, mf.Charge);

            mf.Charge = 1;
            Assert.AreEqual(1.0, mf.Charge.Value, 0.001);

            mf.Add(mf);
            Assert.AreEqual(2.0, mf.Charge.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestSetCharge_Integer()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            mf.Charge = 1;
            mf.Add(Builder.CreateAtom("C"));
            mf.Add(Builder.CreateAtom("F"));
            mf.Add(Builder.CreateAtom("H"), 3);

            Assert.AreEqual(1.0, mf.Charge.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestCharge_rest()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            Assert.AreEqual(null, mf.Charge);

            mf.Charge = 1;
            Assert.AreEqual(1.0, mf.Charge.Value, 0.001);

            IMolecularFormula mf2 = Builder.CreateMolecularFormula();
            mf2.Charge = -1;
            mf.Add(mf2);
            Assert.AreEqual(0.0, mf.Charge.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestRemoveIsotope_IIsotope()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope flu = Builder.CreateIsotope("F");
            IIsotope h1 = Builder.CreateIsotope("H");
            mf.Add(carb);
            mf.Add(flu);
            mf.Add(h1, 3);

            // remove the Fluorine
            mf.Remove(flu);

            Assert.AreEqual(2, mf.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveAllIsotopes()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope flu = Builder.CreateIsotope("F");
            IIsotope h1 = Builder.CreateIsotope("H");
            mf.Add(carb);
            mf.Add(flu);
            mf.Add(h1, 3);

            // remove the Fluorine
            mf.Clear();

            Assert.AreEqual(0, mf.Count);
        }

        /// <summary>
        /// Only test whether the MolecularFormula are correctly cloned.
       /// </summary>
        [TestMethod()]
        public virtual void TestClone()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            mf.Charge = 1;
            object clone = mf.Clone();
            Assert.IsTrue(clone is IMolecularFormula);
            Assert.AreEqual(mf.Count, ((IMolecularFormula)clone).Count);
            Assert.AreEqual(mf.Charge, ((IMolecularFormula)clone).Charge);
        }

        /// <summary>
        /// Only test whether the MolecularFormula are correctly cloned.
       /// </summary>
        [TestMethod()]
        public virtual void TestClone_Isotopes()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            IIsotope carb = Builder.CreateIsotope("C");
            IIsotope flu = Builder.CreateIsotope("F");
            IIsotope h1 = Builder.CreateIsotope("H");
            mf.Add(carb);
            mf.Add(flu);
            mf.Add(h1, 3);

            Assert.AreEqual(3, mf.Count);
            Assert.AreEqual(1, mf.GetCount(carb));
            Assert.AreEqual(1, mf.GetCount(flu));
            Assert.AreEqual(3, mf.GetCount(h1));

            object clone = mf.Clone();
            Assert.IsTrue(clone is IMolecularFormula);
            Assert.AreEqual(mf.Count, ((IMolecularFormula)clone).Count);

            Assert.AreEqual(3, ((IMolecularFormula)clone).Count);
        }

        [TestMethod()]
        public virtual void TestSetProperty_Object_Object()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            mf.SetProperty("blabla", 2);
            Assert.IsNotNull(mf.GetProperty<object>("blabla"));
        }

        [TestMethod()]
        public virtual void TestRemoveProperty_Object()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            string blabla = "blabla";
            double number = 2;
            mf.SetProperty(blabla, number);
            Assert.IsNotNull(mf.GetProperty<object>(blabla));

            mf.RemoveProperty("blabla");
            Assert.IsNull(mf.GetProperty<object>(blabla));
        }

        [TestMethod()]
        public virtual void TestGetProperty_Object()
        {
            TestSetProperty_Object_Object();
        }

        [TestMethod()]
        public virtual void TestGetProperties()
        {
            IMolecularFormula mf = Builder.CreateMolecularFormula();
            mf.SetProperty("blabla", 2);
            mf.SetProperty("blabla3", 3);
            Assert.AreEqual(2, mf.GetProperties().Count());
        }
        
        [TestMethod()]
        public virtual void TestSetProperties_Map()
        {
            TestGetProperties();
        }

        [TestMethod()]
        public virtual void TestGetBuilder()
        {
            IMolecularFormula add = Builder.CreateMolecularFormula();
            IChemObjectBuilder builder = add.Builder;
            Assert.IsNotNull(Builder);
            Assert.AreEqual(Builder.GetType().Name, builder.GetType().Name);
        }
    }
}