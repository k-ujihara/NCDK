/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Default;
using NCDK.IO;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
    [TestClass()]
    public class ReactionSetManipulatorTest : CDKTestCase
    {
        private IChemObjectBuilder builder;
        private ReactionSet set;

        public ReactionSetManipulatorTest()
                : base()
        { }

        [TestInitialize()]
        public void SetUp()
        {
            builder = Default.ChemObjectBuilder.Instance;
            string filename1 = "NCDK.Data.MDL.reaction-1.rxn";
            var ins1 = ResourceLoader.GetAsStream(filename1);
            MDLRXNReader reader1 = new MDLRXNReader(ins1);
            set = (ReactionSet)reader1.Read(new ReactionSet());
            reader1.Close();
        }

        [TestMethod()]
        public void TestGetAllMolecules_IReactionSet()
        {
            IReactionSet reactionSet = builder.NewReactionSet();
            reactionSet.Add(builder.NewReaction()); // 1
            reactionSet.Add(builder.NewReaction()); // 2

            Assert.AreEqual(0, ReactionSetManipulator.GetAllMolecules(reactionSet).Count);

        }

        [TestMethod()]
        public void TestGetAllMolecules_IReactionSet2()
        {
            IReactionSet reactionSet = builder.NewReactionSet();
            IReaction reaction1 = builder.NewReaction();
            IAtomContainer molecule = builder.NewAtomContainer();
            reaction1.Products.Add(molecule);
            reaction1.Reactants.Add(builder.NewAtomContainer());
            reactionSet.Add(reaction1);
            IReaction reaction2 = builder.NewReaction();
            reaction2.Products.Add(builder.NewAtomContainer());
            reaction2.Reactants.Add(molecule);
            reactionSet.Add(reaction2);

            Assert.AreEqual(3, ReactionSetManipulator.GetAllMolecules(reactionSet).Count);
        }

        [TestMethod()]
        public void TestGetAtomCount_IReactionSet()
        {
            Assert.AreEqual(19, ReactionSetManipulator.GetAtomCount(set));
        }

        [TestMethod()]
        public void TestGetBondCount_IReactionSet()
        {
            Assert.AreEqual(18, ReactionSetManipulator.GetBondCount(set));
        }

        [TestMethod()]
        public void TestGetAllAtomContainers_IReactionSet()
        {
            Assert.AreEqual(3, ReactionSetManipulator.GetAllAtomContainers(set).Count());
        }

        [TestMethod()]
        public void TestGetRelevantReaction_IReactionSet_IAtom()
        {
            foreach (var container in ReactionSetManipulator.GetAllAtomContainers(set))
            {
                IAtom anAtom = container.Atoms[0];
                Assert.AreEqual(set[0], ReactionSetManipulator.GetRelevantReaction(set, anAtom));
            }
        }

        [TestMethod()]
        public void TestGetRelevantReaction_IReactionSet_IBond()
        {
            foreach (var container in ReactionSetManipulator.GetAllAtomContainers(set))
            {
                IBond aBond = container.Bonds[0];
                Assert.AreEqual(set[0], ReactionSetManipulator.GetRelevantReaction(set, aBond));
            }
        }

        [TestMethod()]
        public void TestGetRelevantAtomContainer_IReactionSet_IAtom()
        {
            foreach (var container in ReactionSetManipulator.GetAllAtomContainers(set))
            {
                IAtom anAtom = container.Atoms[0];
                Assert.AreEqual(container, ReactionSetManipulator.GetRelevantAtomContainer(set, anAtom));
            }
        }

        [TestMethod()]
        public void TestGetRelevantAtomContainer_IReactionSet_IBond()
        {
            foreach (var container in ReactionSetManipulator.GetAllAtomContainers(set))
            {
                IBond aBond = container.Bonds[0];
                Assert.AreEqual(container, ReactionSetManipulator.GetRelevantAtomContainer(set, aBond));
            }
        }

        [TestMethod()]
        public void TestSetAtomProperties_IReactionSet_Object_Object()
        {
            ReactionSetManipulator.SetAtomProperties(set, "test", "ok");
            foreach (var container in ReactionSetManipulator.GetAllAtomContainers(set))
            {
                foreach (var atom in container.Atoms)
                {
                    Assert.IsNotNull(atom.GetProperty<string>("test"));
                    Assert.AreEqual("ok", atom.GetProperty<string>("test"));
                }
            }
            // reset things
            SetUp();
        }

        [TestMethod()]
        public void TestGetAllChemObjects_IReactionSet()
        {
            var allObjects = ReactionSetManipulator.GetAllChemObjects(set);
            // does not recurse beyond the IAtomContainer, so:
            // set, reaction, 2xreactant, 1xproduct
            Assert.AreEqual(5, allObjects.Count);
        }

        [TestMethod()]
        public void TestRemoveElectronContainer_IReactionSet_IElectronContainer()
        {
            IReactionSet set = builder.NewReactionSet();
            IReaction reaction = builder.NewReaction();
            set.Add(reaction);
            IAtomContainer mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            reaction.Reactants.Add(mol);
            reaction.Reactants.Add(builder.NewAtomContainer());
            reaction.Reactants.Add(builder.NewAtomContainer());
            reaction.Products.Add(builder.NewAtomContainer());
            reaction.Products.Add(builder.NewAtomContainer());
            ReactionSetManipulator.RemoveElectronContainer(set, mol.Bonds[0]);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(0, mol.Bonds.Count);
        }

        [TestMethod()]
        public void TestRemoveAtomAndConnectedElectronContainers_IReactionSet_IAtom()
        {
            IReactionSet set = builder.NewReactionSet();
            IReaction reaction = builder.NewReaction();
            set.Add(reaction);
            IAtomContainer mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            reaction.Reactants.Add(mol);
            reaction.Reactants.Add(builder.NewAtomContainer());
            reaction.Reactants.Add(builder.NewAtomContainer());
            reaction.Products.Add(builder.NewAtomContainer());
            reaction.Products.Add(builder.NewAtomContainer());
            ReactionSetManipulator.RemoveAtomAndConnectedElectronContainers(set, mol.Atoms[0]);

            Assert.AreEqual(1, mol.Atoms.Count);
            Assert.AreEqual(0, mol.Bonds.Count);
        }

        [TestMethod()]
        public void TestGetAllIDs_IReactionSet()
        {
            IReactionSet set = builder.NewReactionSet();
            IReaction reaction1 = builder.NewReaction();
            set.Add(reaction1);
            reaction1.Id = "r1";
            IAtomContainer water = new AtomContainer { Id = "m1" };
            Atom oxygen = new Atom("O") { Id = "a1" };
            water.Atoms.Add(oxygen);
            reaction1.Reactants.Add(water);
            reaction1.Products.Add(water);
            IReaction reaction2 = builder.NewReaction();
            reaction2.Id = "r2";
            set.Add(reaction2);

            var ids = ReactionSetManipulator.GetAllIDs(set);
            Assert.IsNotNull(ids);
            Assert.AreEqual(6, ids.Count);
        }

        [TestMethod()]
        public void TestGetRelevantReactions_IReactionSet_IAtomContainer()
        {
            IReactionSet set = builder.NewReactionSet();
            IReaction reaction1 = builder.NewReaction();
            set.Add(reaction1);
            IAtomContainer mol1a = builder.NewAtomContainer();
            IAtomContainer mol1b = builder.NewAtomContainer();
            reaction1.Reactants.Add(mol1a);
            reaction1.Reactants.Add(mol1b);
            reaction1.Products.Add(builder.NewAtomContainer());
            reaction1.Products.Add(builder.NewAtomContainer());

            IReaction reaction2 = builder.NewReaction();
            reaction2.Reactants.Add(mol1b);
            reaction2.Products.Add(builder.NewAtomContainer());
            set.Add(reaction2);

            IReaction reaction3 = builder.NewReaction();
            reaction3.Reactants.Add(builder.NewAtomContainer());
            reaction3.Products.Add(builder.NewAtomContainer());
            set.Add(reaction3);

            Assert.AreEqual(3, set.Count);
            IReactionSet reactionSet2 = ReactionSetManipulator.GetRelevantReactions(set, mol1b);
            Assert.AreEqual(2, reactionSet2.Count);
            Assert.AreEqual(reaction1, reactionSet2[0]);
            Assert.AreEqual(reaction2, reactionSet2[1]);
            IReactionSet reactionSet1 = ReactionSetManipulator.GetRelevantReactions(set, mol1a);
            Assert.AreEqual(1, reactionSet1.Count);
            Assert.AreEqual(reaction1, reactionSet1[0]);
        }

        [TestMethod()]
        public void TestGetRelevantReactionsAsReactant_IReactionSet_IAtomContainer()
        {
            IReactionSet set = builder.NewReactionSet();
            IReaction reaction1 = builder.NewReaction();
            set.Add(reaction1);
            IAtomContainer mol1a = builder.NewAtomContainer();
            IAtomContainer mol1b = builder.NewAtomContainer();
            reaction1.Reactants.Add(mol1a);
            reaction1.Reactants.Add(mol1b);
            reaction1.Products.Add(builder.NewAtomContainer());
            reaction1.Products.Add(builder.NewAtomContainer());

            IReaction reaction2 = builder.NewReaction();
            reaction2.Reactants.Add(mol1b);
            reaction2.Products.Add(builder.NewAtomContainer());
            set.Add(reaction2);

            IReaction reaction3 = builder.NewReaction();
            reaction3.Reactants.Add(builder.NewAtomContainer());
            reaction3.Products.Add(builder.NewAtomContainer());
            set.Add(reaction3);

            Assert.AreEqual(3, set.Count);
            IReactionSet reactionSet2 = ReactionSetManipulator.GetRelevantReactionsAsReactant(set, mol1b);
            Assert.AreEqual(2, reactionSet2.Count);
            Assert.AreEqual(reaction1, reactionSet2[0]);
            Assert.AreEqual(reaction2, reactionSet2[1]);
            IReactionSet reactionSet1 = ReactionSetManipulator.GetRelevantReactionsAsReactant(set, mol1a);
            Assert.AreEqual(1, reactionSet1.Count);
            Assert.AreEqual(reaction1, reactionSet1[0]);
        }

        [TestMethod()]
        public void TestGetRelevantReactionsAsProduct_IReactionSet_IAtomContainer()
        {
            IReactionSet set = builder.NewReactionSet();
            IReaction reaction1 = builder.NewReaction();
            set.Add(reaction1);
            IAtomContainer mol1a = builder.NewAtomContainer();
            IAtomContainer mol1b = builder.NewAtomContainer();
            reaction1.Reactants.Add(mol1a);
            reaction1.Reactants.Add(mol1b);
            reaction1.Products.Add(builder.NewAtomContainer());
            reaction1.Products.Add(builder.NewAtomContainer());

            IReaction reaction2 = builder.NewReaction();
            reaction2.Reactants.Add(mol1b);
            reaction2.Products.Add(builder.NewAtomContainer());
            set.Add(reaction2);

            IReaction reaction3 = builder.NewReaction();
            reaction3.Reactants.Add(builder.NewAtomContainer());
            reaction3.Products.Add(mol1a);
            set.Add(reaction3);

            Assert.AreEqual(3, set.Count);
            IReactionSet reactionSet2 = ReactionSetManipulator.GetRelevantReactionsAsProduct(set, mol1b);
            Assert.AreEqual(0, reactionSet2.Count);
            IReactionSet reactionSet1 = ReactionSetManipulator.GetRelevantReactionsAsProduct(set, mol1a);
            Assert.AreEqual(1, reactionSet1.Count);
            Assert.AreEqual(reaction3, reactionSet1[0]);
        }

        [TestMethod()]
        public void TestGetReactionByReactionID_IReactionSet_String()
        {
            IReactionSet set = builder.NewReactionSet();
            IReaction reaction1 = builder.NewReaction();
            reaction1.Id = "1";
            set.Add(reaction1);
            IAtomContainer mol1a = builder.NewAtomContainer();
            IAtomContainer mol1b = builder.NewAtomContainer();
            reaction1.Reactants.Add(mol1a);
            reaction1.Reactants.Add(mol1b);
            reaction1.Products.Add(builder.NewAtomContainer());
            reaction1.Products.Add(builder.NewAtomContainer());

            IReaction reaction2 = builder.NewReaction();
            reaction2.Id = "2";
            reaction2.Reactants.Add(mol1b);
            reaction2.Products.Add(builder.NewAtomContainer());
            set.Add(reaction2);

            IReaction reaction3 = builder.NewReaction();
            reaction3.Id = "3";
            reaction3.Reactants.Add(builder.NewAtomContainer());
            reaction3.Products.Add(mol1a);
            set.Add(reaction3);
            Assert.AreEqual(reaction1, ReactionSetManipulator.GetReactionByReactionID(set, "1"));
            Assert.IsNull(ReactionSetManipulator.GetReactionByAtomContainerID(set, "4"));
        }

        [TestMethod()]
        public void TestGetReactionByAtomContainerID_IReactionSet_String()
        {
            IReactionSet set = builder.NewReactionSet();
            IReaction reaction1 = builder.NewReaction();
            set.Add(reaction1);
            IAtomContainer mol1a = builder.NewAtomContainer();
            mol1a.Id = "1";
            IAtomContainer mol1b = builder.NewAtomContainer();
            mol1b.Id = "2";
            reaction1.Reactants.Add(mol1a);
            reaction1.Reactants.Add(builder.NewAtomContainer());
            reaction1.Products.Add(builder.NewAtomContainer());
            reaction1.Products.Add(builder.NewAtomContainer());

            IReaction reaction2 = builder.NewReaction();
            reaction2.Reactants.Add(builder.NewAtomContainer());
            reaction2.Products.Add(mol1b);
            set.Add(reaction2);

            IReaction reaction3 = builder.NewReaction();
            reaction3.Reactants.Add(builder.NewAtomContainer());
            reaction3.Products.Add(builder.NewAtomContainer());
            set.Add(reaction3);
            Assert.AreEqual(reaction1, ReactionSetManipulator.GetReactionByAtomContainerID(set, "1"));
            Assert.AreEqual(reaction2, ReactionSetManipulator.GetReactionByAtomContainerID(set, "2"));
            Assert.IsNull(ReactionSetManipulator.GetReactionByAtomContainerID(set, "3"));
        }
    }
}
