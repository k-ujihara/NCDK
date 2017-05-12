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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IChemSequence"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractChemSequenceTest
        : AbstractChemObjectTest
    {

        [TestMethod()]
        public virtual void TestAddChemModel_IChemModel()
        {
            IChemSequence cs = (IChemSequence)NewChemObject();
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());
            Assert.AreEqual(3, cs.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveChemModel_int()
        {
            IChemSequence cs = (IChemSequence)NewChemObject();
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());
            Assert.AreEqual(3, cs.Count);
            cs.RemoveAt(1);
            Assert.AreEqual(2, cs.Count);
        }

        [TestMethod()]
        public virtual void TestGrowChemModelArray()
        {
            IChemSequence cs = (IChemSequence)NewChemObject();
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());
            Assert.AreEqual(3, cs.Count);
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel()); // this one should enfore array grow
            Assert.AreEqual(6, cs.Count);
        }

        [TestMethod()]
        public virtual void TestGetChemModelCount()
        {
            IChemSequence cs = (IChemSequence)NewChemObject();
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());
            Assert.AreEqual(3, cs.Count);
        }

        [TestMethod()]
        public virtual void TestGetChemModel_int()
        {
            IChemSequence cs = (IChemSequence)NewChemObject();
            cs.Add(cs.Builder.CreateChemModel());
            IChemModel second = cs.Builder.CreateChemModel();
            cs.Add(second);
            cs.Add(cs.Builder.CreateChemModel());

            Assert.AreEqual(second, cs[1]);
        }

        [TestMethod()]
        public virtual void TestChemModels()
        {
            IChemSequence cs = (IChemSequence)NewChemObject();
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());
            cs.Add(cs.Builder.CreateChemModel());

            Assert.AreEqual(3, cs.Count);
            IEnumerator<IChemModel> models = cs.GetEnumerator();
            int count = 0;
            while (models.MoveNext())
            {
                Assert.IsNotNull(models.Current);
                ++count;
            }
            Assert.AreEqual(3, count);
        }

        /// <summary>Test for RFC #9</summary>
        [TestMethod()]
        public virtual void TestToString()
        {
            IChemSequence cs = (IChemSequence)NewChemObject();
            string description = cs.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]
        public override void TestStateChanged_IChemObjectChangeEvent()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemSequence chemObject = (IChemSequence)NewChemObject();
            chemObject.Listeners.Add(listener);

            chemObject.Add(chemObject.Builder.CreateChemModel());
            Assert.IsTrue(listener.Changed);
        }

        private class ChemObjectListenerImpl : IChemObjectListener
        {
            public bool Changed { get; private set; }

            public ChemObjectListenerImpl()
            {
                Changed = false;
            }

            [TestMethod()]

            public virtual void OnStateChanged(ChemObjectChangeEventArgs e)
            {
                Changed = true;
            }

            [TestMethod()]
            public virtual void Reset()
            {
                Changed = false;
            }
        }

        [TestMethod()]

        public override void TestClone()
        {
            IChemSequence sequence = (IChemSequence)NewChemObject();
            object clone = sequence.Clone();
            Assert.IsTrue(clone is IChemSequence);
        }

        [TestMethod()]
        public virtual void TestClone_IChemModel()
        {
            IChemSequence sequence = (IChemSequence)NewChemObject();
            sequence.Add(sequence.Builder.CreateChemModel()); // 1
            sequence.Add(sequence.Builder.CreateChemModel()); // 2
            sequence.Add(sequence.Builder.CreateChemModel()); // 3
            sequence.Add(sequence.Builder.CreateChemModel()); // 4

            IChemSequence clone = (IChemSequence)sequence.Clone();
            Assert.AreEqual(sequence.Count, clone.Count);
            for (int f = 0; f < sequence.Count; f++)
            {
                for (int g = 0; g < clone.Count; g++)
                {
                    Assert.IsNotNull(sequence[f]);
                    Assert.IsNotNull(clone[g]);
                    Assert.AreNotSame(sequence[f], clone[g]);
                }
            }
        }
    }
}