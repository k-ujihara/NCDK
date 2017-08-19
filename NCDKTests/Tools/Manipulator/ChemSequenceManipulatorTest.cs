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
using System.Collections.Generic;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
    [TestClass()]
    public class ChemSequenceManipulatorTest : CDKTestCase
    {
        IAtomContainer molecule1 = null;
        IAtomContainer molecule2 = null;
        IAtom atomInMol1 = null;
        IBond bondInMol1 = null;
        IAtom atomInMol2 = null;
        IChemObjectSet<IAtomContainer> moleculeSet = null;
        IReaction reaction = null;
        IReactionSet reactionSet = null;
        IChemModel chemModel1 = null;
        IChemModel chemModel2 = null;
        IChemSequence chemSequence = null;

        public ChemSequenceManipulatorTest()
            : base()
        { }

        [TestInitialize()]
        public void SetUp()
        {
            molecule1 = new AtomContainer();
            atomInMol1 = new Atom("Cl");
            molecule1.Atoms.Add(atomInMol1);
            molecule1.Atoms.Add(new Atom("Cl"));
            bondInMol1 = new Bond(atomInMol1, molecule1.Atoms[1]);
            molecule1.Bonds.Add(bondInMol1);
            molecule2 = new AtomContainer();
            atomInMol2 = new Atom("O");
            atomInMol2.ImplicitHydrogenCount = 2;
            molecule2.Atoms.Add(atomInMol2);
            moleculeSet = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
            moleculeSet.Add(molecule1);
            moleculeSet.Add(molecule2);
            reaction = new Reaction();
            reaction.Reactants.Add(molecule1);
            reaction.Products.Add(molecule2);
            reactionSet = new ReactionSet();
            reactionSet.Add(reaction);
            chemModel1 = new ChemModel();
            chemModel1.MoleculeSet = moleculeSet;
            chemModel2 = new ChemModel();
            chemModel2.ReactionSet = reactionSet;
            chemSequence = new ChemSequence();
            chemSequence.Add(chemModel1);
            chemSequence.Add(chemModel2);
        }

        [TestMethod()]
        public void TestGetAtomCount_IChemSequence()
        {
            int count = ChemSequenceManipulator.GetAtomCount(chemSequence);
            Assert.AreEqual(6, count);
        }

        [TestMethod()]
        public void TestGetBondCount_IChemSequence()
        {
            int count = ChemSequenceManipulator.GetBondCount(chemSequence);
            Assert.AreEqual(2, count);
        }

        [TestMethod()]
        public void TestGetAllAtomContainers_IChemSequence()
        {
            var list = ChemSequenceManipulator.GetAllAtomContainers(chemSequence);
            Assert.AreEqual(4, list.Count);
        }

        [TestMethod()]
        public void TestGetAllChemObjects_IChemSequence()
        {
            var list = ChemSequenceManipulator.GetAllChemObjects(chemSequence);
            int molCount = 0;
            int molSetCount = 0;
            int reactionCount = 0;
            int reactionSetCount = 0;
            int chemModelCount = 0;
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
                else if (o is IChemModel)
                    ++chemModelCount;
                else
                    Assert.Fail("Unexpected Object of type " + o.GetType());
            }
            //Assert.AreEqual(3, atomCount);
            //Assert.AreEqual(1, bondCount);
            Assert.AreEqual(2, molCount);
            Assert.AreEqual(1, molSetCount);
            Assert.AreEqual(1, reactionCount);
            Assert.AreEqual(1, reactionSetCount);
            Assert.AreEqual(2, chemModelCount);
        }

        [TestMethod()]
        public void TestGetAllIDs_IChemSequence()
        {
            Assert.AreEqual(0, ChemSequenceManipulator.GetAllIDs(chemSequence).Count);
            IDCreator.CreateIDs(chemSequence);
            var allIDs = ChemSequenceManipulator.GetAllIDs(chemSequence);
            Assert.AreEqual(18, ChemSequenceManipulator.GetAllIDs(chemSequence).Count);
            var uniq = new HashSet<string>(allIDs);
            Assert.AreEqual(12, uniq.Count);
        }
    }
}
