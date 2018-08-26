



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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the <see cref="ChemModel"/>.
    /// </summary>
    [TestClass()]
    public class ChemModelTest : AbstractChemModelTest
    {
        public override IChemObject NewChemObject()
        {
            return new ChemModel();
        }

        [TestMethod()]
        public virtual void TestChemModel()
        {
            IChemModel chemModel = new ChemModel();
            Assert.IsNotNull(chemModel);
            Assert.IsTrue(chemModel.IsEmpty());

            IAtom atom = new Atom("N");
            IRing mol = new Ring();   // NCDK does not allow to add AtomContainer to RingSet
            IChemObjectSet<IAtomContainer> mset = new ChemObjectSet<IAtomContainer>();
            mol.Atoms.Add(atom);
            mset.Add(mol);    
            chemModel.MoleculeSet = mset;
            Assert.IsFalse(chemModel.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(chemModel.IsEmpty());
            chemModel.MoleculeSet = null;
            Assert.IsTrue(chemModel.IsEmpty());

            IChemModel model1 = new ChemModel();
            mol.Atoms.Add(atom);
            IReaction react = new Reaction();
            react.Reactants.Add(mol);
            IReactionSet rset = new ReactionSet { react };
            model1.ReactionSet = rset;
            Assert.IsFalse(model1.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(model1.IsEmpty());
            model1.ReactionSet = null;
            Assert.IsTrue(model1.IsEmpty());

            IChemModel model2 = new ChemModel();
            mol.Atoms.Add(atom);
            IRingSet ringset = new RingSet();
            ringset.AddRange(mset.Cast<IRing>());  // NCDK does not allow to add AtomContainer to RingSet directly
            model2.RingSet = ringset;
            Assert.IsFalse(model2.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(model2.IsEmpty());
            model2.RingSet = null;
            Assert.IsTrue(model2.IsEmpty());

            IChemModel model3 = new ChemModel();
            mol.Atoms.Add(atom);
            ICrystal cry = new Crystal(mol);
            model3.Crystal = cry;
            Assert.IsFalse(model3.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(model3.IsEmpty());
            model3.Crystal = null;
            Assert.IsTrue(model3.IsEmpty());
        }

    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="ChemModel"/>.
    /// </summary>
    [TestClass()]
    public class ChemModelTest : AbstractChemModelTest
    {
        public override IChemObject NewChemObject()
        {
            return new ChemModel();
        }

        [TestMethod()]
        public virtual void TestChemModel()
        {
            IChemModel chemModel = new ChemModel();
            Assert.IsNotNull(chemModel);
            Assert.IsTrue(chemModel.IsEmpty());

            IAtom atom = new Atom("N");
            IRing mol = new Ring();   // NCDK does not allow to add AtomContainer to RingSet
            IChemObjectSet<IAtomContainer> mset = new ChemObjectSet<IAtomContainer>();
            mol.Atoms.Add(atom);
            mset.Add(mol);    
            chemModel.MoleculeSet = mset;
            Assert.IsFalse(chemModel.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(chemModel.IsEmpty());
            chemModel.MoleculeSet = null;
            Assert.IsTrue(chemModel.IsEmpty());

            IChemModel model1 = new ChemModel();
            mol.Atoms.Add(atom);
            IReaction react = new Reaction();
            react.Reactants.Add(mol);
            IReactionSet rset = new ReactionSet { react };
            model1.ReactionSet = rset;
            Assert.IsFalse(model1.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(model1.IsEmpty());
            model1.ReactionSet = null;
            Assert.IsTrue(model1.IsEmpty());

            IChemModel model2 = new ChemModel();
            mol.Atoms.Add(atom);
            IRingSet ringset = new RingSet();
            ringset.AddRange(mset.Cast<IRing>());  // NCDK does not allow to add AtomContainer to RingSet directly
            model2.RingSet = ringset;
            Assert.IsFalse(model2.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(model2.IsEmpty());
            model2.RingSet = null;
            Assert.IsTrue(model2.IsEmpty());

            IChemModel model3 = new ChemModel();
            mol.Atoms.Add(atom);
            ICrystal cry = new Crystal(mol);
            model3.Crystal = cry;
            Assert.IsFalse(model3.IsEmpty());
            mol.Atoms.Remove(atom);
            Assert.IsFalse(model3.IsEmpty());
            model3.Crystal = null;
            Assert.IsTrue(model3.IsEmpty());
        }

 
        [TestMethod()]
        public override void TestStateChanged_EventPropagation_Crystal()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            ICrystal crystal = chemObject.Builder.NewCrystal();
            chemObject.Crystal = crystal;
            Assert.IsFalse(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            crystal.Add(chemObject.Builder.NewAtomContainer());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_EventPropagation_AtomContainerSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            var molSet = chemObject.Builder.NewAtomContainerSet();
            chemObject.MoleculeSet = molSet;
            Assert.IsFalse(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            molSet.Add(chemObject.Builder.NewAtomContainer());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_EventPropagation_ReactionSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IReactionSet reactionSet = chemObject.Builder.NewReactionSet();
            chemObject.ReactionSet = reactionSet;
            Assert.IsFalse(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            reactionSet.Add(chemObject.Builder.NewReaction());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_EventPropagation_RingSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IRingSet ringSet = chemObject.Builder.NewRingSet();
            chemObject.RingSet = ringSet;
            Assert.IsFalse(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            ringSet.Add(chemObject.Builder.NewRing());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_ButNotAfterRemoval_Crystal()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            ICrystal crystal = chemObject.Builder.NewCrystal();
            chemObject.Crystal = crystal;
            Assert.IsFalse(listener.Changed);
            // remove the set from the IChemModel
            chemObject.Crystal = null;
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set must *not* trigger a change event in the IChemModel
            crystal.Add(chemObject.Builder.NewAtomContainer());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_ButNotAfterRemoval_AtomContainerSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            var molSet = chemObject.Builder.NewAtomContainerSet();
            chemObject.MoleculeSet = molSet;
            Assert.IsFalse(listener.Changed);
            // remove the set from the IChemModel
            chemObject.MoleculeSet = null;
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set must *not* trigger a change event in the IChemModel
            molSet.Add(chemObject.Builder.NewAtomContainer());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_ButNotAfterRemoval_ReactionSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IReactionSet reactionSet = chemObject.Builder.NewReactionSet();
            chemObject.ReactionSet = reactionSet;
            Assert.IsFalse(listener.Changed);
            // remove the set from the IChemModel
            chemObject.ReactionSet = null;
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set must *not* trigger a change event in the IChemModel
            reactionSet.Add(chemObject.Builder.NewReaction());
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public override void TestStateChanged_ButNotAfterRemoval_RingSet()
        {
            ChemObjectListener listener = new ChemObjectListener();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IRingSet ringSet = chemObject.Builder.NewRingSet();
            chemObject.RingSet = ringSet;
            Assert.IsFalse(listener.Changed);
            // remove the set from the IChemModel
            chemObject.RingSet = null;
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set must *not* trigger a change event in the IChemModel
            ringSet.Add(chemObject.Builder.NewRing());
            Assert.IsFalse(listener.Changed);
        }

                // Overwrite default methods: no notifications are expected!

        [TestMethod()]
        public override void TestNotifyChanged()
        {
            ChemObjectTestHelper.TestNotifyChanged(NewChemObject());
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
        public override void TestNotifyChanged_SetProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetProperty(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_RemoveProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_RemoveProperty(NewChemObject());
        }

    }
}
