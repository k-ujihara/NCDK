/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@users.sf.net>
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
using NCDK;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    /**
     * @cdk.module test-standard
     */
    [TestClass()]
    public class ReactionSchemeManipulatorTest : CDKTestCase
    {
        private IChemObjectBuilder builder;

        public ReactionSchemeManipulatorTest()
            : base()
        { }

        [TestInitialize()]
        public void SetUp()
        {
            builder = Default.ChemObjectBuilder.Instance;
        }

        [TestMethod()]
        public void TestGetAllMolecules_IReactionScheme()
        {
            IReactionScheme reactionScheme = builder.CreateReactionScheme();
            IReaction reaction1 = builder.CreateReaction();
            reaction1.Products.Add(builder.CreateAtomContainer());
            IReaction reaction2 = builder.CreateReaction();
            reaction2.Products.Add(builder.CreateAtomContainer());
            reactionScheme.Add(reaction1); // 1
            reactionScheme.Add(reaction2); // 2

            Assert.AreEqual(2, ReactionSchemeManipulator.GetAllAtomContainers(reactionScheme).Count);
        }

        [TestMethod()]
        public void TestGetAllMolecules_IReactionScheme_IAtomContainerSet()
        {
            IReactionScheme reactionScheme = builder.CreateReactionScheme();
            IReaction reaction1 = builder.CreateReaction();
            reaction1.Products.Add(builder.CreateAtomContainer());
            IReaction reaction2 = builder.CreateReaction();
            reaction2.Products.Add(builder.CreateAtomContainer());
            reactionScheme.Add(reaction1); // 1
            reactionScheme.Add(reaction2); // 2

            Assert.AreEqual(
                    2,
                    ReactionSchemeManipulator.GetAllAtomContainers(reactionScheme,
                            builder.CreateAtomContainerSet()).Count);

        }

        [TestMethod()]
        public void TestGetAllMolecules_IReactionScheme2()
        {
            IReactionScheme reactionScheme = builder.CreateReactionScheme();
            IReaction reaction1 = builder.CreateReaction();
            IAtomContainer molecule = builder.CreateAtomContainer();
            reaction1.Products.Add(molecule);
            reaction1.Reactants.Add(builder.CreateAtomContainer());
            reactionScheme.Add(reaction1);
            IReaction reaction2 = builder.CreateReaction();
            reaction2.Products.Add(builder.CreateAtomContainer());
            reaction2.Reactants.Add(molecule);
            reactionScheme.Add(reaction2);

            Assert.AreEqual(3, ReactionSchemeManipulator.GetAllAtomContainers(reactionScheme).Count);

        }

        [TestMethod()]
        public void TestGetAllMolecules_IReactionScheme3()
        {
            IReactionScheme scheme1 = builder.CreateReactionScheme();

            IReactionScheme scheme11 = builder.CreateReactionScheme();
            IReaction reaction1 = builder.CreateReaction();
            IAtomContainer molecule = builder.CreateAtomContainer();
            reaction1.Products.Add(molecule);
            reaction1.Reactants.Add(builder.CreateAtomContainer());
            scheme11.Add(reaction1);
            IReaction reaction2 = builder.CreateReaction();
            reaction2.Products.Add(builder.CreateAtomContainer());
            reaction2.Reactants.Add(molecule);
            scheme11.Add(reaction2);
            scheme1.Add(scheme11);

            IReactionScheme scheme12 = builder.CreateReactionScheme();
            IReaction reaction3 = builder.CreateReaction();
            reaction3.Products.Add(builder.CreateAtomContainer());
            reaction3.Reactants.Add(molecule);
            scheme12.Add(reaction3);
            scheme1.Add(scheme12);

            IReaction reaction11 = builder.CreateReaction();
            reaction11.Products.Add(builder.CreateAtomContainer());
            scheme1.Add(reaction11);

            Assert.AreEqual(5, ReactionSchemeManipulator.GetAllAtomContainers(scheme1).Count);

        }

        [TestMethod()]
        public void TestGetAllIDs_IReactionScheme()
        {
            IReactionScheme scheme1 = builder.CreateReactionScheme();
            scheme1.Id = "scheme1";

            IReactionScheme scheme11 = builder.CreateReactionScheme();
            scheme11.Id = "scheme11";
            IReaction reaction1 = builder.CreateReaction();
            reaction1.Id = "reaction1";
            IAtomContainer molecule = builder.CreateAtomContainer();
            reaction1.Id = "molecule";
            reaction1.Products.Add(molecule);
            reaction1.Reactants.Add(builder.CreateAtomContainer());
            scheme11.Add(reaction1);
            IReaction reaction2 = builder.CreateReaction();
            reaction1.Id = "reaction2";
            reaction2.Products.Add(builder.CreateAtomContainer());
            reaction2.Reactants.Add(molecule);
            scheme11.Add(reaction2);
            scheme1.Add(scheme11);

            IReactionScheme scheme12 = builder.CreateReactionScheme();
            scheme12.Id = "scheme12";
            IReaction reaction3 = builder.CreateReaction();
            reaction3.Id = "reaction3";
            reaction3.Products.Add(builder.CreateAtomContainer());
            reaction3.Reactants.Add(molecule);
            scheme12.Add(reaction3);
            scheme1.Add(scheme12);

            IReaction reaction11 = builder.CreateReaction();
            reaction11.Id = "reaction11";
            reaction11.Products.Add(builder.CreateAtomContainer());
            scheme1.Add(reaction11);

            Assert.AreEqual(6, ReactionSchemeManipulator.GetAllIDs(scheme1).Count());

        }

        [TestMethod()]
        public void TestCreateReactionScheme_IReactionSet()
        {
            IAtomContainer molA = builder.CreateAtomContainer();
            molA.Id = "A";
            IAtomContainer molB = builder.CreateAtomContainer();
            molB.Id = "B";
            IAtomContainer molC = builder.CreateAtomContainer();
            molC.Id = "C";
            IAtomContainer molD = builder.CreateAtomContainer();
            molD.Id = "D";
            IAtomContainer molE = builder.CreateAtomContainer();
            molE.Id = "E";

            IReactionSet reactionSet = builder.CreateReactionSet();
            IReaction reaction1 = builder.CreateReaction();
            reaction1.Id = "r1";
            reaction1.Reactants.Add(molA);
            reaction1.Products.Add(molB);
            reactionSet.Add(reaction1);

            IReaction reaction2 = builder.CreateReaction();
            reaction2.Id = "r2";
            reaction2.Reactants.Add(molB);
            reaction2.Products.Add(molC);
            reactionSet.Add(reaction2);

            IReaction reaction3 = builder.CreateReaction();
            reaction3.Id = "r3";
            reaction3.Reactants.Add(molB);
            reaction3.Products.Add(molD);
            reactionSet.Add(reaction3);

            IReaction reaction4 = builder.CreateReaction();
            reaction4.Id = "r4";
            reaction4.Reactants.Add(molC);
            reaction4.Products.Add(molE);
            reactionSet.Add(reaction4);

            IReactionScheme scheme1 = ReactionSchemeManipulator.CreateReactionScheme(reactionSet);
            Assert.AreEqual(1, scheme1.Reactions.Count());
            Assert.AreEqual("r1", scheme1.Reactions.ElementAt(0).Id);
            Assert.AreEqual(1, scheme1.Schemes.Count);

            IReactionScheme scheme2 = scheme1.Schemes.First();
            Assert.AreEqual(2, scheme2.Reactions.Count());
            Assert.AreEqual("r2", scheme2.Reactions.ElementAt(0).Id);
            Assert.AreEqual("r3", scheme2.Reactions.ElementAt(1).Id);
            Assert.AreEqual(1, scheme2.Schemes.Count);

            IReactionScheme scheme3 = scheme2.Schemes.First();
            Assert.AreEqual(1, scheme3.Reactions.Count());
            Assert.AreEqual("r4", scheme3.Reactions.ElementAt(0).Id);
            Assert.AreEqual(0, scheme3.Schemes.Count);
        }

        [TestMethod()]
        public void TestGetMoleculeSet_IAtomContainer_IAtomContainer_IReactionScheme()
        {
            IReactionScheme scheme1 = builder.CreateReactionScheme();

            IReactionScheme scheme11 = builder.CreateReactionScheme();
            IReaction reaction1 = builder.CreateReaction();
            reaction1.Id = "reaction1";
            IAtomContainer startMol = builder.CreateAtomContainer();
            startMol.Id = "startMol";
            reaction1.Reactants.Add(startMol);
            IAtomContainer mitMol = builder.CreateAtomContainer();
            mitMol.Id = "mitMol";
            reaction1.Products.Add(mitMol);
            scheme11.Add(reaction1);
            IReaction reaction2 = builder.CreateReaction();
            reaction2.Products.Add(builder.CreateAtomContainer());
            reaction2.Reactants.Add(builder.CreateAtomContainer());
            reaction2.Id = "reaction2";
            scheme11.Add(reaction2);
            scheme1.Add(scheme11);

            IReactionScheme scheme12 = builder.CreateReactionScheme();
            IReaction reaction3 = builder.CreateReaction();
            IAtomContainer finalMol = builder.CreateAtomContainer();
            finalMol.Id = "finalMol";
            reaction3.Products.Add(finalMol);
            reaction3.Reactants.Add(mitMol);
            reaction3.Id = "reaction3";
            scheme12.Add(reaction3);
            scheme1.Add(scheme12);

            IReaction reaction11 = builder.CreateReaction();
            reaction11.Products.Add(builder.CreateAtomContainer());
            reaction11.Id = "reaction11";
            scheme1.Add(reaction11);

            var listSet = ReactionSchemeManipulator.GetAtomContainerSet(startMol, finalMol,
                    scheme1);
            Assert.AreEqual(1, listSet.Count);
            var moleculeSet = listSet[0];
            Assert.AreEqual("startMol", moleculeSet[0].Id);
            Assert.AreEqual("mitMol", moleculeSet[1].Id);
            Assert.AreEqual("finalMol", moleculeSet[2].Id);
        }

        [TestMethod()]
        public void TestGetAllReactions_IReactionScheme()
        {
            IReactionScheme scheme1 = builder.CreateReactionScheme();

            IReactionScheme scheme11 = builder.CreateReactionScheme();
            IReaction reaction1 = builder.CreateReaction();
            IAtomContainer startMol = builder.CreateAtomContainer();
            startMol.Id = "startMol";
            reaction1.Reactants.Add(startMol);
            IAtomContainer mitMol = builder.CreateAtomContainer();
            mitMol.Id = "mitMol";
            reaction1.Products.Add(mitMol);
            scheme11.Add(reaction1);
            IReaction reaction2 = builder.CreateReaction();
            reaction2.Products.Add(builder.CreateAtomContainer());
            reaction2.Reactants.Add(builder.CreateAtomContainer());
            scheme11.Add(reaction2);
            scheme1.Add(scheme11);

            IReactionScheme scheme12 = builder.CreateReactionScheme();
            IReaction reaction3 = builder.CreateReaction();
            IAtomContainer finalMol = builder.CreateAtomContainer();
            finalMol.Id = "finalMol";
            reaction3.Products.Add(finalMol);
            reaction3.Reactants.Add(startMol);
            scheme12.Add(reaction3);
            scheme1.Add(scheme12);

            IReaction reaction11 = builder.CreateReaction();
            reaction11.Products.Add(builder.CreateAtomContainer());
            scheme1.Add(reaction11);

            IReactionSet reactionSet = ReactionSchemeManipulator.GetAllReactions(scheme1);
            Assert.AreEqual(4, reactionSet.Count);
            Assert.AreEqual(reaction1, reactionSet[0]);
            Assert.AreEqual(reaction2, reactionSet[1]);
            Assert.AreEqual(reaction3, reactionSet[2]);
            Assert.AreEqual(reaction11, reactionSet[3]);
        }

        [TestMethod()]
        public void TestExtractTopReactions_IReactionScheme()
        {
            IReactionScheme scheme1 = builder.CreateReactionScheme();
            IReaction reaction1 = builder.CreateReaction();
            IAtomContainer molA = builder.CreateAtomContainer();
            reaction1.Reactants.Add(molA);
            IAtomContainer molB = builder.CreateAtomContainer();
            reaction1.Products.Add(molB);
            scheme1.Add(reaction1);

            IReactionScheme scheme2 = builder.CreateReactionScheme();
            IReaction reaction2 = builder.CreateReaction();
            reaction2.Reactants.Add(molB);
            IAtomContainer molC = builder.CreateAtomContainer();
            reaction2.Products.Add(molC);
            scheme2.Add(reaction2);

            IReaction reaction3 = builder.CreateReaction();
            reaction3.Reactants.Add(molB);
            IAtomContainer molD = builder.CreateAtomContainer();
            reaction3.Products.Add(molD);
            scheme2.Add(reaction3);

            IReaction reaction4 = builder.CreateReaction();
            IAtomContainer molE = builder.CreateAtomContainer();
            reaction4.Reactants.Add(molE);
            IAtomContainer molF = builder.CreateAtomContainer();
            reaction4.Products.Add(molF);
            scheme1.Add(reaction4);

            IReactionSet reactionSet = ReactionSchemeManipulator.ExtractTopReactions(scheme1);
            Assert.AreEqual(2, reactionSet.Count);
            Assert.AreEqual(reaction1, reactionSet[0]);
            Assert.AreEqual(reaction4, reactionSet[1]);
        }
    }
}

