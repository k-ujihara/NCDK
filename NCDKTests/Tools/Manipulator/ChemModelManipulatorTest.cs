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
using NCDK.IO;
using NCDK.Silent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
    [TestClass()]
    public class ChemModelManipulatorTest : CDKTestCase
    {
        IAtomContainer molecule1 = null;
        IAtomContainer molecule2 = null;
        IAtom atomInMol1 = null;
        IBond bondInMol1 = null;
        IAtom atomInMol2 = null;
        IChemObjectSet<IAtomContainer> moleculeSet = null;
        IReaction reaction = null;
        IReactionSet reactionSet = null;
        IChemModel chemModel = null;

        public ChemModelManipulatorTest()
            : base()
        { }

        [TestInitialize()]
        public void SetUp()
        {
            molecule1 = new AtomContainer();
            atomInMol1 = new Atom("Cl")
            {
                Charge = -1.0,
                FormalCharge = -1,
                ImplicitHydrogenCount = 1
            };
            molecule1.Atoms.Add(atomInMol1);
            molecule1.Atoms.Add(new Atom("Cl"));
            bondInMol1 = new Bond(atomInMol1, molecule1.Atoms[1]);
            molecule1.Bonds.Add(bondInMol1);
            molecule2 = new AtomContainer();
            atomInMol2 = new Atom("O")
            {
                ImplicitHydrogenCount = 2
            };
            molecule2.Atoms.Add(atomInMol2);
            moleculeSet = ChemObjectBuilder.Instance.NewAtomContainerSet();
            moleculeSet.Add(molecule1);
            moleculeSet.Add(molecule2);
            reaction = new Reaction();
            reaction.Reactants.Add(molecule1);
            reaction.Products.Add(molecule2);
            reactionSet = new ReactionSet
            {
                reaction
            };
            chemModel = new ChemModel
            {
                MoleculeSet = moleculeSet,
                ReactionSet = reactionSet
            };
        }

        [TestMethod()]
        public void TestGetAllAtomContainers_IChemModel()
        {
            var filename = "NCDK.Data.MDL.a-pinene.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);

            var reader = new MDLV2000Reader(ins);
            ChemModel chemFile = (ChemModel)reader.Read((ChemObject)new ChemModel());
            Assert.IsNotNull(chemFile);
            var containersList = ChemModelManipulator.GetAllAtomContainers(chemFile);
            Assert.AreEqual(1, containersList.Count());
        }

        [TestMethod()]
        public void TestGetAllAtomContainers_IChemModel_WithReactions()
        {
            var filename = "NCDK.Data.MDL.0024.stg02.rxn";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);

            MDLRXNV2000Reader reader = new MDLRXNV2000Reader(ins, ChemObjectReaderMode.Strict);
            ChemModel chemFile = (ChemModel)reader.Read((ChemObject)new ChemModel());
            Assert.IsNotNull(chemFile);
            var containersList = ChemModelManipulator.GetAllAtomContainers(chemFile);

            Assert.AreEqual(2, containersList.Count());
        }

        [TestMethod()]
        public void TestNewChemModel_IAtomContainer()
        {
            var ac = new AtomContainer();
            ac.Atoms.Add(new Atom("C"));
            var model = ChemModelManipulator.NewChemModel(ac);
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(ac.Atoms.Count, mol.Atoms.Count);
        }

        [TestMethod()]
        public void TestGetAtomCount_IChemModel()
        {
            int count = ChemModelManipulator.GetAtomCount(chemModel);
            Assert.AreEqual(6, count);
        }

        [TestMethod()]
        public void TestGetBondCount_IChemModel()
        {
            int count = ChemModelManipulator.GetBondCount(chemModel);
            Assert.AreEqual(2, count);
        }

        [TestMethod()]
        public void TestRemoveElectronContainer_IChemModel_IElectronContainer()
        {
            IAtomContainer mol1 = new AtomContainer();
            mol1.Atoms.Add(new Atom("Cl"));
            mol1.Atoms.Add(new Atom("Cl"));
            IBond bond1 = new Bond(mol1.Atoms[0], mol1.Atoms[1]);
            mol1.Bonds.Add(bond1);
            IAtomContainer mol2 = new AtomContainer();
            mol2.Atoms.Add(new Atom("I"));
            mol2.Atoms.Add(new Atom("I"));
            IBond bond2 = new Bond(mol2.Atoms[0], mol2.Atoms[1]);
            mol2.Bonds.Add(bond2);
            var molSet = ChemObjectBuilder.Instance.NewAtomContainerSet();
            molSet.Add(mol1);
            IReaction r = new Reaction();
            r.Products.Add(mol2);
            IReactionSet rSet = new ReactionSet
            {
                r
            };
            var model = new ChemModel
            {
                MoleculeSet = molSet,
                ReactionSet = rSet
            };
            IBond otherBond = new Bond();
            Assert.AreEqual(2, ChemModelManipulator.GetBondCount(model));
            ChemModelManipulator.RemoveElectronContainer(model, otherBond);
            Assert.AreEqual(2, ChemModelManipulator.GetBondCount(model));
            ChemModelManipulator.RemoveElectronContainer(model, bond1);
            Assert.AreEqual(1, ChemModelManipulator.GetBondCount(model));
            ChemModelManipulator.RemoveElectronContainer(model, bond2);
            Assert.AreEqual(0, ChemModelManipulator.GetBondCount(model));
        }

        [TestMethod()]
        public void TestRemoveAtomAndConnectedElectronContainers_IChemModel_IAtom()
        {
            IAtomContainer mol1 = new AtomContainer();
            IAtom atom1 = new Atom("Cl");
            mol1.Atoms.Add(atom1);
            mol1.Atoms.Add(new Atom("Cl"));
            IBond bond1 = new Bond(mol1.Atoms[0], mol1.Atoms[1]);
            mol1.Bonds.Add(bond1);
            IAtomContainer mol2 = new AtomContainer();
            IAtom atom2 = new Atom("I");
            mol2.Atoms.Add(atom2);
            mol2.Atoms.Add(new Atom("I"));
            IBond bond2 = new Bond(mol2.Atoms[0], mol2.Atoms[1]);
            mol2.Bonds.Add(bond2);
            var molSet = ChemObjectBuilder.Instance.NewAtomContainerSet();
            molSet.Add(mol1);
            IReaction r = new Reaction();
            r.Products.Add(mol2);
            IReactionSet rSet = new ReactionSet
            {
                r
            };
            var model = new ChemModel
            {
                MoleculeSet = molSet,
                ReactionSet = rSet
            };
            IAtom otherAtom = new Atom("Cl");
            Assert.AreEqual(2, ChemModelManipulator.GetBondCount(model));
            Assert.AreEqual(4, ChemModelManipulator.GetAtomCount(model));
            ChemModelManipulator.RemoveAtomAndConnectedElectronContainers(model, otherAtom);
            Assert.AreEqual(2, ChemModelManipulator.GetBondCount(model));
            Assert.AreEqual(4, ChemModelManipulator.GetAtomCount(model));
            ChemModelManipulator.RemoveAtomAndConnectedElectronContainers(model, atom1);
            Assert.AreEqual(1, ChemModelManipulator.GetBondCount(model));
            Assert.AreEqual(3, ChemModelManipulator.GetAtomCount(model));
            ChemModelManipulator.RemoveAtomAndConnectedElectronContainers(model, atom2);
            Assert.AreEqual(0, ChemModelManipulator.GetBondCount(model));
            Assert.AreEqual(2, ChemModelManipulator.GetAtomCount(model));
        }

        [TestMethod()]
        public void TestSetAtomProperties_IChemModel_Object_Object()
        {
            string key = "key";
            string value = "value";
            ChemModelManipulator.SetAtomProperties(chemModel, key, value);
            Assert.AreEqual(value, atomInMol1.GetProperty<string>(key));
            Assert.AreEqual(value, atomInMol2.GetProperty<string>(key));
        }

        [TestMethod()]
        public void TestGetRelevantAtomContainer_IChemModel_IAtom()
        {
            IAtomContainer ac1 = ChemModelManipulator.GetRelevantAtomContainer(chemModel, atomInMol1);
            Assert.AreEqual(molecule1, ac1);
            IAtomContainer ac2 = ChemModelManipulator.GetRelevantAtomContainer(chemModel, atomInMol2);
            Assert.AreEqual(molecule2, ac2);
        }

        [TestMethod()]
        public void TestGetRelevantAtomContainer_IChemModel_IBond()
        {
            IAtomContainer ac1 = ChemModelManipulator.GetRelevantAtomContainer(chemModel, bondInMol1);
            Assert.AreEqual(molecule1, ac1);
        }

        [TestMethod()]
        public void TestGetAllChemObjects_IChemModel()
        {
            var list = ChemModelManipulator.GetAllChemObjects(chemModel);
            Assert.AreEqual(5, list.Count);
            //int atomCount = 0; // not traversed
            //int bondCount = 0; // not traversed
            int molCount = 0;
            int molSetCount = 0;
            int reactionCount = 0;
            int reactionSetCount = 0;
            foreach (var o in list)
            {
                //if (o is IAtom) ++atomCount;
                //if (o is IBond) ++bondCount;
                if (o is IAtomContainer)
                    ++molCount;
                else if (o is IChemObjectSet<IAtomContainer>)
                    ++molSetCount;
                else if (o is IReaction)
                    ++reactionCount;
                else if (o is IReactionSet)
                    ++reactionSetCount;
                else
                    Assert.Fail("Unexpected Object of type " + o.GetType());
            }
            //Assert.AreEqual(3, atomCount);
            //Assert.AreEqual(1, bondCount);
            Assert.AreEqual(2, molCount);
            Assert.AreEqual(1, molSetCount);
            Assert.AreEqual(1, reactionCount);
            Assert.AreEqual(1, reactionSetCount);
        }

        [TestMethod()]
        public void TestCreateNewMolecule_IChemModel()
        {
            var model = new ChemModel();
            IAtomContainer ac = ChemModelManipulator.CreateNewMolecule(model);
            Assert.AreEqual(1, model.MoleculeSet.Count);
            Assert.AreEqual(ac, model.MoleculeSet[0]);
        }

        [TestMethod()]
        public void TestGetRelevantReaction_IChemModel_IAtom()
        {
            IReaction r = ChemModelManipulator.GetRelevantReaction(chemModel, atomInMol1);
            Assert.IsNotNull(r);
            Assert.AreEqual(reaction, r);
        }

        [TestMethod()]
        public void TestGetAllIDs_IChemModel()
        {
            Assert.AreEqual(0, ChemModelManipulator.GetAllIDs(chemModel).Count());
            IDCreator.CreateIDs(chemModel);
            var allIDs = ChemModelManipulator.GetAllIDs(chemModel);
            Assert.AreEqual(16, ChemModelManipulator.GetAllIDs(chemModel).Count());
            var uniq = new HashSet<string>(allIDs);
            Assert.AreEqual(10, uniq.Count);
        }

        // @cdk.bug 3530861
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetRelevantAtomContainer_NonExistentAtom()
        {
            var model = CDK.Builder.NewChemModel();
            ChemModelManipulator.GetRelevantAtomContainer(model, CDK.Builder.NewAtom());
        }
    }
}

