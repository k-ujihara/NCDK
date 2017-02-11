/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCDK.Numerics;
using NCDK;

namespace NCDK
{

    /**
     * Checks the functionality of {@link IReactionSet} implementations.
     *
     * @cdk.module test-interfaces
     */
    [TestClass()]
    public abstract class AbstractReactionSetTest : AbstractChemObjectTest
    {

        [TestMethod()]

        public override void TestClone()
        {
            IReactionSet reactionSet = (IReactionSet)NewChemObject();
            object clone = reactionSet.Clone();
            Assert.IsTrue(clone is IReactionSet);
        }

        [TestMethod()]
        public virtual void TestGetReactionCount()
        {
            IReactionSet reactionSet = (IReactionSet)NewChemObject();
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 1
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 2
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 3
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 4
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 5
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 6 (force growing)
            Assert.AreEqual(6, reactionSet.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveAllReactions()
        {
            IReactionSet reactionSet = (IReactionSet)NewChemObject();
            reactionSet.Add(reactionSet.Builder.CreateReaction());
            reactionSet.Clear();
			Assert.AreEqual(0, reactionSet.Count);
        }

        [TestMethod()]
        public virtual void TestReactions()
        {
            IReactionSet reactionSet = (IReactionSet)NewChemObject();
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 1
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 2
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 3
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 4

            IEnumerator<IReaction> reactionIter = reactionSet.GetEnumerator();
            Assert.IsNotNull(reactionIter);
            int count = 0;

            while (reactionIter.MoveNext())
            {
                Assert.IsNotNull(reactionIter.Current);
                ++count;
            }
            Assert.AreEqual(4, count);
        }

        [TestMethod()]
        public virtual void TestGetReaction_int()
        {
            IReactionSet reactionSet = (IReactionSet)NewChemObject();
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 1
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 2
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 3
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 4

            for (int i = 0; i < reactionSet.Count; i++)
            {
                Assert.IsNotNull(reactionSet[i]);
            }
        }

        [TestMethod()]
        public virtual void TestAddReaction_IReaction()
        {
            IReactionSet reactionSet = (IReactionSet)NewChemObject();
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 1
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 2
            IReaction third = reactionSet.Builder.CreateReaction();
            reactionSet.Add(third); // 3
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 4

            Assert.AreEqual(4, reactionSet.Count);
            Assert.AreEqual(third, reactionSet[2]);
        }

        [TestMethod()]
        public virtual void TestRemoveReaction_int()
        {
            IReactionSet reactionSet = (IReactionSet)NewChemObject();
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 1
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 2
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 3
            Assert.AreEqual(3, reactionSet.Count);
            reactionSet.RemoveAt(1);
            Assert.AreEqual(2, reactionSet.Count);
            Assert.IsNotNull(reactionSet[0]);
            Assert.IsNotNull(reactionSet[1]);
        }

        [TestMethod()]
        public virtual void TestClone_Reaction()
        {
            IReactionSet reactionSet = (IReactionSet)NewChemObject();
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 1
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 2
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 3
            reactionSet.Add(reactionSet.Builder.CreateReaction()); // 4

            IReactionSet clone = (IReactionSet)reactionSet.Clone();
            Assert.AreEqual(reactionSet.Count, clone.Count);
            for (int f = 0; f < reactionSet.Count; f++)
            {
                for (int g = 0; g < clone.Count; g++)
                {
                    Assert.IsNotNull(reactionSet[f]);
                    Assert.IsNotNull(clone[g]);
                    Assert.AreNotSame(reactionSet[f], clone[g]);
                }
            }
        }

        /**
         * Method to test whether the class complies with RFC #9.
         */
        [TestMethod()]
        public virtual void TestToString()
        {
            IReactionSet reactionSet = (IReactionSet)NewChemObject();
            string description = reactionSet.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }

            IReaction reaction = reactionSet.Builder.CreateReaction();
            reactionSet.Add(reaction);
            description = reactionSet.ToString();
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
            IReactionSet chemObject = (IReactionSet)NewChemObject();
            chemObject.Listeners.Add(listener);

            chemObject.Add(chemObject.Builder.CreateReaction());
            Assert.IsTrue(listener.Changed);

            listener.Reset();
            Assert.IsFalse(listener.Changed);

        }

        [TestMethod()]
        public virtual void TestRemoveReaction_IReaction()
        {
            IReactionSet reactionSet = (IReactionSet)NewChemObject();
            IReaction reaction = reactionSet.Builder.CreateReaction();
            reaction.Id = "1";
            reactionSet.Add(reaction);
            IReaction relevantReaction = reactionSet.Builder.CreateReaction();
            relevantReaction.Id = "2";
            reactionSet.Add(relevantReaction);
            Assert.AreEqual(2, reactionSet.Count);
            reactionSet.Remove(relevantReaction);
            Assert.AreEqual(1, reactionSet.Count);
            Assert.AreEqual("1", reactionSet[0].Id);
            reactionSet.Add(relevantReaction);
            reactionSet.Add(relevantReaction);
            Assert.AreEqual(3, reactionSet.Count);
            reactionSet.Remove(relevantReaction);
            Assert.AreEqual(1, reactionSet.Count);
            Assert.AreEqual("1", reactionSet[0].Id);
        }

        private class ChemObjectListenerImpl : IChemObjectListener
        {
            public bool Changed { get; set; }

            public ChemObjectListenerImpl()
            {
                Changed = false;
            }

            public void OnStateChanged(ChemObjectChangeEventArgs e)
            {
                Changed = true;
            }

            public void Reset()
            {
                Changed = false;
            }
        }

        [TestMethod()]
        public virtual void TestIsEmpty()
        {

            IReactionSet set = (IReactionSet)NewChemObject();

            Assert.IsTrue(set.IsEmpty, "new reaction set should be empty");

            set.Add(set.Builder.CreateReaction());

            Assert.IsFalse(set.IsEmpty, "reaction set with a single reaction should not be empty");

            set.Clear();

            Assert.IsTrue(set.IsEmpty, "reaction set with all reactions removed should be empty");

        }

    }
}