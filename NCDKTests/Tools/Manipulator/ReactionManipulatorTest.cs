/* Copyright (C) 2003-2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Smiles;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
    // @author     Egon Willighagen
    // @cdk.created    2003-07-23
    [TestClass()]
    public class ReactionManipulatorTest : CDKTestCase
    {
        private IReaction reaction;
        private IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

        public ReactionManipulatorTest()
            : base()
        { }

        [TestInitialize()]
        public void SetUp()
        {
            string filename1 = "NCDK.Data.MDL.reaction-1.rxn";
            var ins1 = ResourceLoader.GetAsStream(filename1);
            MDLRXNReader reader1 = new MDLRXNReader(ins1);
            ReactionSet set = (ReactionSet)reader1.Read(new ReactionSet());
            reaction = set[0];
            reader1.Close();
        }

        [TestMethod()]
        public void TestReverse_IReaction()
        {
            Reaction reaction = new Reaction();
            reaction.Direction = ReactionDirection.Backward;
            IAtomContainer water = new AtomContainer();
            reaction.Reactants.Add(water, 3.0);
            reaction.Reactants.Add(new AtomContainer());
            reaction.Products.Add(new AtomContainer());

            Reaction reversedReaction = (Reaction)ReactionManipulator.Reverse(reaction);
            Assert.AreEqual(ReactionDirection.Forward, reversedReaction.Direction);
            Assert.AreEqual(2, reversedReaction.Products.Count);
            Assert.AreEqual(1, reversedReaction.Reactants.Count);
            Assert.AreEqual(3.0, reversedReaction.Products.GetMultiplier(water).Value, 0.00001);
        }

        [TestMethod()]
        public void TestGetAllIDs_IReaction()
        {
            Reaction reaction = new Reaction();
            reaction.Id = "r1";
            IAtomContainer water = new AtomContainer();
            water.Id = "m1";
            Atom oxygen = new Atom("O");
            oxygen.Id = "a1";
            water.Atoms.Add(oxygen);
            reaction.Reactants.Add(water);
            reaction.Products.Add(water);

            var ids = ReactionManipulator.GetAllIDs(reaction);
            Assert.IsNotNull(ids);
            Assert.AreEqual(5, ids.Count);
        }

        /// <summary>
        /// A unit test suite for JUnit. Test of mapped IAtoms
        /// </summary>
        [TestMethod()]
        public void TestGetMappedChemObject_IReaction_IAtom()
        {
            IReaction reaction = builder.CreateReaction();
            IAtomContainer reactant = (new SmilesParser(builder)).ParseSmiles("[C+]-C=C");
            IAtomContainer product = (new SmilesParser(builder)).ParseSmiles("C=C=C");

            IMapping mapping = builder.CreateMapping(reactant.Atoms[0], product.Atoms[0]);
            reaction.Mappings.Add(mapping);
            mapping = builder.CreateMapping(reactant.Atoms[1], product.Atoms[1]);
            reaction.Mappings.Add(mapping);
            mapping = builder.CreateMapping(reactant.Atoms[2], product.Atoms[2]);
            reaction.Mappings.Add(mapping);

            reaction.Reactants.Add(reactant);
            reaction.Products.Add(product);

            IAtom mappedAtom = (IAtom)ReactionManipulator.GetMappedChemObject(reaction, reactant.Atoms[0]);
            Assert.AreEqual(mappedAtom, product.Atoms[0]);

            mappedAtom = (IAtom)ReactionManipulator.GetMappedChemObject(reaction, product.Atoms[1]);
            Assert.AreEqual(mappedAtom, reactant.Atoms[1]);

        }

        /// <summary>
        /// A unit test suite for JUnit. Test of mapped IBond
        /// </summary>
        [TestMethod()]
        public void TestGetMappedChemObject_IReaction_IBond()
        {
            IReaction reaction = builder.CreateReaction();
            IAtomContainer reactant = (new SmilesParser(builder)).ParseSmiles("[C+]-C=C");
            IAtomContainer product = (new SmilesParser(builder)).ParseSmiles("C=C=C");

            IMapping mapping = builder.CreateMapping(reactant.Atoms[0], product.Atoms[0]);
            reaction.Mappings.Add(mapping);
            mapping = builder.CreateMapping(reactant.Bonds[0], product.Bonds[0]);
            reaction.Mappings.Add(mapping);
            mapping = builder.CreateMapping(reactant.Bonds[1], product.Bonds[1]);
            reaction.Mappings.Add(mapping);

            reaction.Reactants.Add(reactant);
            reaction.Products.Add(product);

            IBond mappedBond = (IBond)ReactionManipulator.GetMappedChemObject(reaction, reactant.Bonds[0]);
            Assert.AreEqual(mappedBond, product.Bonds[0]);

            mappedBond = (IBond)ReactionManipulator.GetMappedChemObject(reaction, product.Bonds[1]);
            Assert.AreEqual(mappedBond, reactant.Bonds[1]);
        }

        [TestMethod()]
        public void TestGetAtomCount_IReaction()
        {
            Assert.AreEqual(19, ReactionManipulator.GetAtomCount(reaction));
        }

        [TestMethod()]
        public void TestGetBondCount_IReaction()
        {
            Assert.AreEqual(18, ReactionManipulator.GetBondCount(reaction));
        }

        [TestMethod()]
        public void TestGetAllAtomContainers_IReaction()
        {
            Assert.AreEqual(3, ReactionManipulator.GetAllAtomContainers(reaction).Count());
        }

        [TestMethod()]
        public void TestSetAtomProperties_IReactionSet_Object_Object()
        {
            ReactionManipulator.SetAtomProperties(reaction, "test", "ok");
            foreach (var container in ReactionManipulator.GetAllAtomContainers(reaction))
            {
                foreach (var atom in container.Atoms)
                {
                    Assert.IsNotNull(atom.GetProperty<string>("test"));
                    Assert.AreEqual("ok", atom.GetProperty<string>("test"));
                }
            }
        }

        [TestMethod()]
        public void TestGetAllChemObjects_IReactionSet()
        {
            var allObjects = ReactionManipulator.GetAllChemObjects(reaction);
            // does not recurse beyond the IAtomContainer, so:
            // reaction, 2xreactant, 1xproduct
            Assert.AreEqual(4, allObjects.Count);
        }

        [TestMethod()]
        public void TestGetRelevantAtomContainer_IReaction_IAtom()
        {
            foreach (var container in ReactionManipulator.GetAllAtomContainers(reaction))
            {
                IAtom anAtom = container.Atoms[0];
                Assert.AreEqual(container, ReactionManipulator.GetRelevantAtomContainer(reaction, anAtom));
            }
        }

        [TestMethod()]
        public void TestGetRelevantAtomContainer_IReaction_IBond()
        {
            foreach (var container in ReactionManipulator.GetAllAtomContainers(reaction))
            {
                IBond aBond = container.Bonds[0];
                Assert.AreEqual(container, ReactionManipulator.GetRelevantAtomContainer(reaction, aBond));
            }
        }

        [TestMethod()]
        public void TestRemoveElectronContainer_IReaction_IElectronContainer()
        {
            IReaction reaction = builder.CreateReaction();
            IAtomContainer mol = builder.CreateAtomContainer();
            mol.Atoms.Add(builder.CreateAtom("C"));
            mol.Atoms.Add(builder.CreateAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            reaction.Reactants.Add(mol);
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Products.Add(builder.CreateAtomContainer());
            reaction.Products.Add(builder.CreateAtomContainer());
            ReactionManipulator.RemoveElectronContainer(reaction, mol.Bonds[0]);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(0, mol.Bonds.Count);
        }

        [TestMethod()]
        public void TestRemoveAtomAndConnectedElectronContainers_IReaction_IAtom()
        {
            IReaction reaction = builder.CreateReaction();
            IAtomContainer mol = builder.CreateAtomContainer();
            mol.Atoms.Add(builder.CreateAtom("C"));
            mol.Atoms.Add(builder.CreateAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            reaction.Reactants.Add(mol);
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Products.Add(builder.CreateAtomContainer());
            reaction.Products.Add(builder.CreateAtomContainer());
            ReactionManipulator.RemoveAtomAndConnectedElectronContainers(reaction, mol.Atoms[0]);

            Assert.AreEqual(1, mol.Atoms.Count);
            Assert.AreEqual(0, mol.Bonds.Count);
        }

        [TestMethod()]
        public void TestGetAllMolecules_IReaction()
        {
            IReaction reaction = builder.CreateReaction();
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Products.Add(builder.CreateAtomContainer());
            reaction.Products.Add(builder.CreateAtomContainer());
            Assert.AreEqual(5, ReactionManipulator.GetAllMolecules(reaction).Count);
        }

        [TestMethod()]
        public void TestGetAllProducts_IReaction()
        {
            IReaction reaction = builder.CreateReaction();
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Products.Add(builder.CreateAtomContainer());
            reaction.Products.Add(builder.CreateAtomContainer());
            Assert.AreEqual(3, ReactionManipulator.GetAllReactants(reaction).Count);
        }

        [TestMethod()]
        public void TestGetAllReactants_IReaction()
        {
            IReaction reaction = builder.CreateReaction();
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Reactants.Add(builder.CreateAtomContainer());
            reaction.Products.Add(builder.CreateAtomContainer());
            reaction.Products.Add(builder.CreateAtomContainer());
            Assert.AreEqual(2, ReactionManipulator.GetAllProducts(reaction).Count);
        }

        [TestMethod()]
        public void InliningReactions()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IReaction reaction = smipar.ParseReactionSmiles("CCO.CC(=O)O>[H+]>CCOC(=O)C.O ethyl esterification");
            SmilesGenerator smigen = SmilesGenerator.Isomeric();
            // convert to molecule
            IAtomContainer mol = ReactionManipulator.ToMolecule(reaction);
            Assert.AreEqual("CCO.CC(=O)O.[H+].CCOC(=O)C.O", smigen.Create(mol));
            Assert.AreEqual("CCO.CC(=O)O>[H+]>CCOC(=O)C.O", smigen.CreateReactionSMILES(ReactionManipulator.ToReaction(mol)));
        }
    }
}
