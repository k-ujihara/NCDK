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

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IChemModel"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractChemModelTest
        : AbstractChemObjectTest
    {
        [TestMethod()]
        public void TestSetMoleculeSet_IAtomContainerSet()
        {
            IChemModel chemModel = (IChemModel)NewChemObject();
            IAtomContainerSet<IAtomContainer> crystal = chemModel.Builder.NewAtomContainerSet();
            chemModel.MoleculeSet = crystal;
            Assert.AreEqual(crystal, chemModel.MoleculeSet);
        }

        [TestMethod()]
        public void TestGetMoleculeSet()
        {
            TestSetMoleculeSet_IAtomContainerSet();
        }

        [TestMethod()]
        public void TestSetReactionSet_IReactionSet()
        {
            IChemModel chemModel = (IChemModel)NewChemObject();
            IReactionSet crystal = chemModel.Builder.NewReactionSet();
            chemModel.ReactionSet = crystal;
            Assert.AreEqual(crystal, chemModel.ReactionSet);
        }

        [TestMethod()]
        public void TestGetReactionSet()
        {
            TestSetReactionSet_IReactionSet();
        }

        [TestMethod()]
        public void TestSetRingSet_IRingSet()
        {
            IChemModel chemModel = (IChemModel)NewChemObject();
            IRingSet crystal = chemModel.Builder.NewRingSet();
            chemModel.RingSet = crystal;
            Assert.AreEqual(crystal, chemModel.RingSet);
        }

        [TestMethod()]
        public void TestGetRingSet()
        {
            TestSetRingSet_IRingSet();
        }

        [TestMethod()]
        public void TestSetCrystal_ICrystal()
        {
            IChemModel chemModel = (IChemModel)NewChemObject();
            ICrystal crystal = chemModel.Builder.NewCrystal();
            chemModel.Crystal = crystal;
            Assert.AreEqual(crystal, chemModel.Crystal);
        }

        [TestMethod()]
        public void TestGetCrystal()
        {
            TestSetCrystal_ICrystal();
        }

        [TestMethod()]
        public void TestToString()
        {
            IChemModel model = (IChemModel)NewChemObject();
            string description = model.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]

        public override void TestClone()
        {
            IChemModel model = (IChemModel)NewChemObject();
            object clone = model.Clone();
            Assert.IsNotNull(clone);
            Assert.IsTrue(clone is IChemModel);
        }

        [TestMethod()]
        public void TestClone_IAtomContainerSet()
        {
            IChemModel model = (IChemModel)NewChemObject();
            IChemModel clone = (IChemModel)model.Clone();
            Assert.IsNull(clone.MoleculeSet);

            model.MoleculeSet = model.Builder.NewAtomContainerSet();
            clone = (IChemModel)model.Clone();
            Assert.IsNotNull(clone.MoleculeSet);
            Assert.AreNotSame(model.MoleculeSet, clone.MoleculeSet);
        }

        [TestMethod()]
        public void TestClone_IReactionSet()
        {
            IChemModel model = (IChemModel)NewChemObject();
            IChemModel clone = (IChemModel)model.Clone();
            Assert.IsNull(clone.ReactionSet);

            model.ReactionSet = model.Builder.NewReactionSet();
            clone = (IChemModel)model.Clone();
            Assert.IsNotNull(clone.ReactionSet);
            Assert.AreNotSame(model.ReactionSet, clone.ReactionSet);
        }

        [TestMethod()]
        public void TestClone_Crystal()
        {
            IChemModel model = (IChemModel)NewChemObject();
            IChemModel clone = (IChemModel)model.Clone();
            Assert.IsNull(clone.Crystal);

            model.Crystal = model.Builder.NewCrystal();
            clone = (IChemModel)model.Clone();
            Assert.IsNotNull(clone.Crystal);
            Assert.AreNotSame(model.Crystal, clone.Crystal);
        }

        [TestMethod()]
        public void TestClone_RingSet()
        {
            IChemModel model = (IChemModel)NewChemObject();
            IChemModel clone = (IChemModel)model.Clone();
            Assert.IsNull(clone.RingSet);

            model.RingSet = model.Builder.NewRingSet();
            clone = (IChemModel)model.Clone();
            Assert.IsNotNull(clone.RingSet);
            Assert.AreNotSame(model.RingSet, clone.RingSet);
        }

        [TestMethod()]
        public override void TestStateChanged_IChemObjectChangeEvent()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            chemObject.MoleculeSet = chemObject.Builder.NewAtomContainerSet();
            Assert.IsTrue(listener.Changed);

            listener.Reset();
            Assert.IsFalse(listener.Changed);
            chemObject.ReactionSet = chemObject.Builder.NewReactionSet();
            Assert.IsTrue(listener.Changed);

            listener.Reset();
            Assert.IsFalse(listener.Changed);
            chemObject.Crystal = chemObject.Builder.NewCrystal();
            Assert.IsTrue(listener.Changed);

            listener.Reset();
            Assert.IsFalse(listener.Changed);
            chemObject.RingSet = chemObject.Builder.NewRingSet();
            Assert.IsTrue(listener.Changed);
        }

        [TestMethod()]
        public virtual void TestStateChanged_EventPropagation_Crystal()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            ICrystal crystal = chemObject.Builder.NewCrystal();
            chemObject.Crystal = crystal;
            Assert.IsTrue(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            crystal.Add(chemObject.Builder.NewAtomContainer());
            Assert.IsTrue(listener.Changed);
        }

        [TestMethod()]
        public virtual void TestStateChanged_EventPropagation_AtomContainerSet()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IAtomContainerSet<IAtomContainer> molSet = chemObject.Builder.NewAtomContainerSet();
            chemObject.MoleculeSet = molSet;
            Assert.IsTrue(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            molSet.Add(chemObject.Builder.NewAtomContainer());
            Assert.IsTrue(listener.Changed);
        }

        [TestMethod()]
        public virtual void TestStateChanged_EventPropagation_ReactionSet()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IReactionSet reactionSet = chemObject.Builder.NewReactionSet();
            chemObject.ReactionSet = reactionSet;
            Assert.IsTrue(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            reactionSet.Add(chemObject.Builder.NewReaction());
            Assert.IsTrue(listener.Changed);
        }

        [TestMethod()]
        public virtual void TestStateChanged_EventPropagation_RingSet()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IRingSet ringSet = chemObject.Builder.NewRingSet();
            chemObject.RingSet = ringSet;
            Assert.IsTrue(listener.Changed);
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set should trigger a change event in the IChemModel
            ringSet.Add(chemObject.Builder.NewRing());
            Assert.IsTrue(listener.Changed);
        }

        [TestMethod()]
        public virtual void TestStateChanged_ButNotAfterRemoval_Crystal()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            ICrystal crystal = chemObject.Builder.NewCrystal();
            chemObject.Crystal = crystal;
            Assert.IsTrue(listener.Changed);
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
        public virtual void TestStateChanged_ButNotAfterRemoval_AtomContainerSet()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IAtomContainerSet<IAtomContainer> molSet = chemObject.Builder.NewAtomContainerSet();
            chemObject.MoleculeSet = molSet;
            Assert.IsTrue(listener.Changed);
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
        public virtual void TestStateChanged_ButNotAfterRemoval_ReactionSet()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IReactionSet reactionSet = chemObject.Builder.NewReactionSet();
            chemObject.ReactionSet = reactionSet;
            Assert.IsTrue(listener.Changed);
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
        public virtual void TestStateChanged_ButNotAfterRemoval_RingSet()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemModel chemObject = (IChemModel)NewChemObject();
            chemObject.Listeners.Add(listener);

            IRingSet ringSet = chemObject.Builder.NewRingSet();
            chemObject.RingSet = ringSet;
            Assert.IsTrue(listener.Changed);
            // remove the set from the IChemModel
            chemObject.RingSet = null;
            // reset the listener
            listener.Reset();
            Assert.IsFalse(listener.Changed);
            // changing the set must *not* trigger a change event in the IChemModel
            ringSet.Add(chemObject.Builder.NewRing());
            Assert.IsFalse(listener.Changed);
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
        public void TestIsEmpty()
        {
            IChemModel chemModel = (IChemModel)NewChemObject();
            Assert.IsTrue(chemModel.IsEmpty(), "new chem model is empty");
        }

        [TestMethod()]
        public void TestIsEmpty_MoleculeSet()
        {

            IChemModel chemModel = (IChemModel)NewChemObject();
            IChemObjectBuilder builder = chemModel.Builder;

            Assert.IsNotNull(chemModel);
            Assert.IsTrue(chemModel.IsEmpty());

            IAtom atom = builder.NewAtom();
            IAtomContainer mol = builder.NewAtomContainer();
            IAtomContainerSet<IAtomContainer> mset = builder.NewAtomContainerSet();

            mol.Atoms.Add(atom);
            mset.Add(mol);
            chemModel.MoleculeSet = mset;
            Assert.IsFalse(chemModel.IsEmpty(), "chem model with a molecule set should not be empty");
            mol.Atoms.Remove(atom);
            Assert.IsFalse(chemModel.IsEmpty(), "chem model with a (empty) molecule set should not be empty");
            chemModel.MoleculeSet = null;
            Assert.IsTrue(chemModel.IsEmpty(), "chemo model with no molecule set should be empty");
        }

        [TestMethod()]
        public void TestIsEmpty_ReactionSet()
        {

            IChemModel model = (IChemModel)NewChemObject();
            IChemObjectBuilder builder = model.Builder;

            IAtomContainer molecule = builder.NewAtomContainer();
            IReaction reaction = builder.NewReaction();

            reaction.Reactants.Add(molecule);

            IReactionSet set = builder.NewReactionSet();
            model.ReactionSet = set;
            Assert.IsTrue(model.IsEmpty(), "model has an empty reaction set and should be empty");
            set.Add(reaction);
            Assert.IsFalse(model.IsEmpty(), "model has a reaction set and should not be empty");
            model.ReactionSet = null;
            Assert.IsTrue(model.IsEmpty(), "model has no reaction set");

        }

        [TestMethod()]
        public void TestIsEmpty_RingSet()
        {

            IChemModel model = (IChemModel)NewChemObject();
            IChemObjectBuilder builder = model.Builder;

            IRing container = builder.NewRing();    // NCDK does not allow to add Ring to RingSet
            IRingSet ringset = builder.NewRingSet();

            Assert.IsTrue(model.IsEmpty());
            model.RingSet = ringset;
            Assert.IsTrue(model.IsEmpty());
            ringset.Add(container);
            Assert.IsFalse(model.IsEmpty());
            model.RingSet = null;
            Assert.IsTrue(model.IsEmpty());

        }

        [TestMethod()]
        public void TestIsEmpty_Crystal()
        {

            IChemModel model = (IChemModel)NewChemObject();
            IChemObjectBuilder builder = model.Builder;

            ICrystal crystal = builder.NewCrystal();
            model.Crystal = crystal;
            Assert.IsTrue(model.IsEmpty());
            crystal.Atoms.Add(builder.NewAtom("C"));
            Assert.IsFalse(model.IsEmpty());
            model.Crystal = null;
            Assert.IsTrue(model.IsEmpty());

        }
    }
}
