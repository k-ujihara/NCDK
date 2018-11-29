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

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IChemFile"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    public abstract class AbstractChemFileTest
        : AbstractChemObjectTest
    {
        [TestMethod()]
        public virtual void TestAddChemSequence_IChemSequence()
        {
            IChemFile cs = (IChemFile)NewChemObject();
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());
            Assert.AreEqual(3, cs.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveChemSequence_int()
        {
            IChemFile cs = (IChemFile)NewChemObject();
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());
            Assert.AreEqual(3, cs.Count);
            cs.RemoveAt(1);
            Assert.AreEqual(2, cs.Count);
        }

        [TestMethod()]
        public virtual void TestGetChemSequence_int()
        {
            IChemFile cs = (IChemFile)NewChemObject();
            cs.Add(cs.Builder.NewChemSequence());
            IChemSequence second = cs.Builder.NewChemSequence();
            cs.Add(second);
            cs.Add(cs.Builder.NewChemSequence());
            Assert.AreEqual(second, cs[1]);
        }

        [TestMethod()]
        public virtual void TestGrowChemSequenceArray()
        {
            IChemFile cs = (IChemFile)NewChemObject();
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());
            Assert.AreEqual(3, cs.Count);
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence()); // this one should enfore array grow
            Assert.AreEqual(6, cs.Count);
        }

        [TestMethod()]
        public virtual void TestChemSequences()
        {
            IChemFile cs = (IChemFile)NewChemObject();
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());

            Assert.IsNotNull(cs);
            Assert.AreEqual(3, cs.Count);
        }

        [TestMethod()]
        public virtual void TestGetChemSequenceCount()
        {
            IChemFile cs = (IChemFile)NewChemObject();
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());
            cs.Add(cs.Builder.NewChemSequence());

            Assert.AreEqual(3, cs.Count);
        }

        /// <summary>Test for RFC #9</summary>
        [TestMethod()]
        public virtual void TestToString()
        {
            IChemFile cs = (IChemFile)NewChemObject();
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
            IChemFile chemObject = (IChemFile)NewChemObject();
            chemObject.Listeners.Add(listener);

            chemObject.Add(chemObject.Builder.NewChemSequence());
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
            public void OnStateChanged(ChemObjectChangeEventArgs e)
            {
                Changed = true;
            }

            [TestMethod()]
            public void Reset()
            {
                Changed = false;
            }
        }

        [TestMethod()]
        public override void TestClone()
        {
            IChemFile file = (IChemFile)NewChemObject();
            object clone = file.Clone();
            Assert.IsTrue(clone is IChemFile);
        }

        [TestMethod()]
        public virtual void TestClone_ChemSequence()
        {
            IChemFile file = (IChemFile)NewChemObject();
            file.Add(file.Builder.NewChemSequence()); // 1
            file.Add(file.Builder.NewChemSequence()); // 2
            file.Add(file.Builder.NewChemSequence()); // 3
            file.Add(file.Builder.NewChemSequence()); // 4

            IChemFile clone = (IChemFile)file.Clone();
            Assert.AreEqual(file.Count, clone.Count);
            for (int f = 0; f < file.Count; f++)
            {
                for (int g = 0; g < clone.Count; g++)
                {
                    Assert.IsNotNull(file[f]);
                    Assert.IsNotNull(clone[g]);
                    Assert.AreNotSame(file[f], clone[g]);
                }
            }
        }
    }
}
