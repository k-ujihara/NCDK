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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the {@link ChemModel}.
    ///
    // @cdk.module test-silent
    /// </summary>
    [TestClass()]
    public class ChemModelTest : AbstractChemModelTest
    {
        public override IChemObject NewChemObject()
        {
            return new ChemModel();
        }

        [TestMethod()]
        public void TestChemModel()
        {
            IChemModel chemModel = new ChemModel();
            Assert.IsNotNull(chemModel);
        }

        // Overwrite default methods: no notifications are expected!

        [TestMethod()]

        public override void TestNotifyChanged()
        {
            ChemObjectTestHelper.TestNotifyChanged(NewChemObject());
        }

        [TestMethod()]

        public override void TestNotifyChanged_IChemObjectChangeEvent()
        {
            ChemObjectTestHelper.TestNotifyChanged_IChemObjectChangeEvent(NewChemObject());
        }

        [TestMethod()]

        public override void TestStateChanged_IChemObjectChangeEvent()
        {
            ChemObjectTestHelper.TestStateChanged_IChemObjectChangeEvent(NewChemObject());
        }

        [TestMethod()]

        public override void TestClone_ChemObjectListeners()
        {
            ChemObjectTestHelper.TestClone_ChemObjectListeners(NewChemObject());
        }

        [TestMethod()]

        public override void TestAddListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestAddListener_IChemObjectListener(NewChemObject());
        }

        [TestMethod()]

        public override void TestGetListenerCount()
        {
            ChemObjectTestHelper.TestGetListenerCount(NewChemObject());
        }

        [TestMethod()]

        public override void TestRemoveListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestRemoveListener_IChemObjectListener(NewChemObject());
        }

        [TestMethod()]

        public override void TestSetNotification_true()
        {
            ChemObjectTestHelper.TestSetNotification_true(NewChemObject());
        }

        [TestMethod()]
        public override void TestStateChanged_EventPropagation_Crystal()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            ICrystal crystal = chemObject.Builder.CreateCrystal();
            chemObject.Crystal = crystal;
            Assert.IsFalse(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            crystal.Add(chemObject.Builder.CreateAtomContainer());
            Assert.IsFalse(listener.Changed);
        }


        [TestMethod()]
        public override void TestStateChanged_EventPropagation_AtomContainerSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            var molSet = chemObject.Builder.CreateAtomContainerSet();
            chemObject.MoleculeSet = molSet;
            Assert.IsFalse(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            molSet.Add(chemObject.Builder.CreateAtomContainer());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_EventPropagation_ReactionSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IReactionSet reactionSet = chemObject.Builder.CreateReactionSet();
            chemObject.ReactionSet = reactionSet;
            Assert.IsFalse(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            reactionSet.Add(chemObject.Builder.CreateReaction());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_EventPropagation_RingSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IRingSet ringSet = chemObject.Builder.CreateRingSet();
            chemObject.RingSet = ringSet;
            Assert.IsFalse(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            ringSet.Add(chemObject.Builder.CreateRing());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_ButNotAfterRemoval_Crystal()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            ICrystal crystal = chemObject.Builder.CreateCrystal();
            chemObject.Crystal = crystal;
            Assert.IsFalse(listener.Changed);
            // remove the set from the IChemModel
            chemObject.Crystal = null;
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set must *not* trigger a change event in the IChemModel
            crystal.Add(chemObject.Builder.CreateAtomContainer());
            Assert.IsFalse(listener.Changed);
        }


        [TestMethod()]
        public override void TestStateChanged_ButNotAfterRemoval_AtomContainerSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            var molSet = chemObject.Builder.CreateAtomContainerSet();
            chemObject.MoleculeSet = molSet;
            Assert.IsFalse(listener.Changed);
            // remove the set from the IChemModel
            chemObject.MoleculeSet = null;
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set must *not* trigger a change event in the IChemModel
            molSet.Add(chemObject.Builder.CreateAtomContainer());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_ButNotAfterRemoval_ReactionSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IReactionSet reactionSet = chemObject.Builder.CreateReactionSet();
            chemObject.ReactionSet = reactionSet;
            Assert.IsFalse(listener.Changed);
            // remove the set from the IChemModel
            chemObject.ReactionSet = null;
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set must *not* trigger a change event in the IChemModel
            reactionSet.Add(chemObject.Builder.CreateReaction());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_ButNotAfterRemoval_RingSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IRingSet ringSet = chemObject.Builder.CreateRingSet();
            chemObject.RingSet = ringSet;
            Assert.IsFalse(listener.Changed);
            // remove the set from the IChemModel
            chemObject.RingSet = null;
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set must *not* trigger a change event in the IChemModel
            ringSet.Add(chemObject.Builder.CreateRing());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestNotifyChanged_SetProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetProperty(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_RemoveProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_RemoveProperty(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_SetFlag()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetFlag(NewChemObject());
        }

        [TestMethod()]
        public void TestNotifyChanged_SetFlags()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetFlags(NewChemObject());
        }
    }
}
