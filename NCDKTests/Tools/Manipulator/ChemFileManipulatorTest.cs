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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
    [TestClass()]
    public class ChemFileManipulatorTest : CDKTestCase
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
        IChemSequence chemSequence1 = null;
        IChemSequence chemSequence2 = null;
        IChemFile chemFile = null;

        public ChemFileManipulatorTest()
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
            moleculeSet = new ChemObjectSet<IAtomContainer>();
            moleculeSet.Add(molecule1);
            moleculeSet.Add(molecule2);
            reaction = new Reaction();
            reaction.Reactants.Add(molecule1);
            reaction.Products.Add(molecule2);
            reactionSet = new ReactionSet();
            reactionSet.Add(reaction);
            chemModel = new ChemModel();
            chemModel.MoleculeSet = moleculeSet;
            chemModel.ReactionSet = reactionSet;
            chemSequence1 = new ChemSequence();
            chemSequence1.Add(chemModel);
            chemSequence2 = new ChemSequence();
            chemFile = new ChemFile();
            chemFile.Add(chemSequence1);
            chemFile.Add(chemSequence2);
        }

        [TestMethod()]
        public void TestGetAllAtomContainers_IChemFile()
        {
            string filename = "NCDK.Data.MDL.prev2000.sd";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);

            MDLReader reader = new MDLReader(ins, ChemObjectReaderMode.Strict);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            Assert.AreEqual(2, containersList.Count);
        }

        [TestMethod()]
        public void TestGetAllIDs_IChemFile()
        {
            Assert.AreEqual(0, ChemFileManipulator.GetAllIDs(chemFile).Count());
            IDCreator.CreateIDs(chemFile);
            var allIDs = ChemFileManipulator.GetAllIDs(chemFile);
            Assert.AreEqual(19, ChemFileManipulator.GetAllIDs(chemFile).Count());
            var uniq = new HashSet<string>(allIDs);
            Assert.AreEqual(13, uniq.Count);
        }

        [TestMethod()]
        public void TestGetAtomCount_IChemFile()
        {
            int count = ChemFileManipulator.GetAtomCount(chemFile);
            Assert.AreEqual(6, count);
        }

        [TestMethod()]
        public void TestGetBondCount_IChemFile()
        {
            int count = ChemFileManipulator.GetBondCount(chemFile);
            Assert.AreEqual(2, count);
        }

        [TestMethod()]
        public void TestGetAllChemObjects_IChemFile()
        {
            List<IChemObject> list = ChemFileManipulator.GetAllChemObjects(chemFile).ToList();
            Assert.AreEqual(8, list.Count); // not the file itself
            int atomCount = 0;
            int bondCount = 0;
            int molCount = 0;
            int molSetCount = 0;
            int reactionCount = 0;
            int reactionSetCount = 0;
            int chemModelCount = 0;
            int chemSequenceCount = 0;
            foreach (var o in list)
            {
                if (o is IAtom) ++atomCount;
                if (o is IBond) ++bondCount;
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
                else if (o is IChemSequence)
                    ++chemSequenceCount;
                else
                    Assert.Fail("Unexpected Object of type " + o.GetType());
            }
            Assert.AreEqual(0, atomCount); /// it does not recurse into IAtomContainer
            Assert.AreEqual(0, bondCount);
            Assert.AreEqual(2, molCount);
            Assert.AreEqual(1, molSetCount);
            Assert.AreEqual(1, reactionCount);
            Assert.AreEqual(1, reactionSetCount);
            Assert.AreEqual(1, chemModelCount);
            Assert.AreEqual(2, chemSequenceCount);
        }

        [TestMethod()]
        public void TestGetAllChemModels_IChemFile()
        {
            IList<IChemModel> list = ChemFileManipulator.GetAllChemModels(chemFile).ToList();
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod()]
        public void TestGetAllReactions_IChemFile()
        {
            IList<IReaction> list = ChemFileManipulator.GetAllReactions(chemFile).ToList();
            Assert.AreEqual(1, list.Count);
        }
    }
}
