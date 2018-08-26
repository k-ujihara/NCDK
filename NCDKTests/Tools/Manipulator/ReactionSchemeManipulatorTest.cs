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
using NCDK.Silent;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
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
            builder = ChemObjectBuilder.Instance;
        }

        [TestMethod()]
        public void TestGetAllMolecules_IReactionScheme()
        {
            IReactionScheme reactionScheme = builder.NewReactionScheme();
            IReaction reaction1 = builder.NewReaction();
            reaction1.Products.Add(builder.NewAtomContainer());
            IReaction reaction2 = builder.NewReaction();
            reaction2.Products.Add(builder.NewAtomContainer());
            reactionScheme.Add(reaction1); // 1
            reactionScheme.Add(reaction2); // 2

            Assert.AreEqual(2, ReactionSchemeManipulator.GetAllAtomContainers(reactionScheme).Count);
        }

        [TestMethod()]
        public void TestGetAllMolecules_IReactionScheme_IAtomContainerSet()
        {
            IReactionScheme reactionScheme = builder.NewReactionScheme();
            IReaction reaction1 = builder.NewReaction();
            reaction1.Products.Add(builder.NewAtomContainer());
            IReaction reaction2 = builder.NewReaction();
            reaction2.Products.Add(builder.NewAtomContainer());
            reactionScheme.Add(reaction1); // 1
            reactionScheme.Add(reaction2); // 2

            Assert.AreEqual(
                    2,
                    ReactionSchemeManipulator.GetAllAtomContainers(reactionScheme,
                            builder.NewAtomContainerSet()).Count);
        }

        [TestMethod()]
        public void TestGetAllMolecules_IReactionScheme2()
        {
            IReactionScheme reactionScheme = builder.NewReactionScheme();
            IReaction reaction1 = builder.NewReaction();
            IAtomContainer molecule = builder.NewAtomContainer();
            reaction1.Products.Add(molecule);
            reaction1.Reactants.Add(builder.NewAtomContainer());
            reactionScheme.Add(reaction1);
            IReaction reaction2 = builder.NewReaction();
            reaction2.Products.Add(builder.NewAtomContainer());
            reaction2.Reactants.Add(molecule);
            reactionScheme.Add(reaction2);

            Assert.AreEqual(3, ReactionSchemeManipulator.GetAllAtomContainers(reactionScheme).Count);
        }

        [TestMethod()]
        public void TestGetAllMolecules_IReactionScheme3()
        {
            IReactionScheme scheme1 = builder.NewReactionScheme();

            IReactionScheme scheme11 = builder.NewReactionScheme();
            IReaction reaction1 = builder.NewReaction();
            IAtomContainer molecule = builder.NewAtomContainer();
            reaction1.Products.Add(molecule);
            reaction1.Reactants.Add(builder.NewAtomContainer());
            scheme11.Add(reaction1);
            IReaction reaction2 = builder.NewReaction();
            reaction2.Products.Add(builder.NewAtomContainer());
            reaction2.Reactants.Add(molecule);
            scheme11.Add(reaction2);
            scheme1.Add(scheme11);

            IReactionScheme scheme12 = builder.NewReactionScheme();
            IReaction reaction3 = builder.NewReaction();
            reaction3.Products.Add(builder.NewAtomContainer());
            reaction3.Reactants.Add(molecule);
            scheme12.Add(reaction3);
            scheme1.Add(scheme12);

            IReaction reaction11 = builder.NewReaction();
            reaction11.Products.Add(builder.NewAtomContainer());
            scheme1.Add(reaction11);

            Assert.AreEqual(5, ReactionSchemeManipulator.GetAllAtomContainers(scheme1).Count);
        }

        [TestMethod()]
        public void TestGetAllIDs_IReactionScheme()
        {
            IReactionScheme scheme1 = builder.NewReactionScheme();
            scheme1.Id = "scheme1";

            IReactionScheme scheme11 = builder.NewReactionScheme();
            scheme11.Id = "scheme11";
            IReaction reaction1 = builder.NewReaction();
            reaction1.Id = "reaction1";
            IAtomContainer molecule = builder.NewAtomContainer();
            reaction1.Id = "molecule";
            reaction1.Products.Add(molecule);
            reaction1.Reactants.Add(builder.NewAtomContainer());
            scheme11.Add(reaction1);
            IReaction reaction2 = builder.NewReaction();
            reaction1.Id = "reaction2";
            reaction2.Products.Add(builder.NewAtomContainer());
            reaction2.Reactants.Add(molecule);
            scheme11.Add(reaction2);
            scheme1.Add(scheme11);

            IReactionScheme scheme12 = builder.NewReactionScheme();
            scheme12.Id = "scheme12";
            IReaction reaction3 = builder.NewReaction();
            reaction3.Id = "reaction3";
            reaction3.Products.Add(builder.NewAtomContainer());
            reaction3.Reactants.Add(molecule);
            scheme12.Add(reaction3);
            scheme1.Add(scheme12);

            IReaction reaction11 = builder.NewReaction();
            reaction11.Id = "reaction11";
            reaction11.Products.Add(builder.NewAtomContainer());
            scheme1.Add(reaction11);

            Assert.AreEqual(6, ReactionSchemeManipulator.GetAllIDs(scheme1).Count());
        }

        [TestMethod()]
        public void TestCreateReactionScheme_IReactionSet()
        {
            IAtomContainer molA = builder.NewAtomContainer();
            molA.Id = "A";
            IAtomContainer molB = builder.NewAtomContainer();
            molB.Id = "B";
            IAtomContainer molC = builder.NewAtomContainer();
            molC.Id = "C";
            IAtomContainer molD = builder.NewAtomContainer();
            molD.Id = "D";
            IAtomContainer molE = builder.NewAtomContainer();
            molE.Id = "E";

            IReactionSet reactionSet = builder.NewReactionSet();
            IReaction reaction1 = builder.NewReaction();
            reaction1.Id = "r1";
            reaction1.Reactants.Add(molA);
            reaction1.Products.Add(molB);
            reactionSet.Add(reaction1);

            IReaction reaction2 = builder.NewReaction();
            reaction2.Id = "r2";
            reaction2.Reactants.Add(molB);
            reaction2.Products.Add(molC);
            reactionSet.Add(reaction2);

            IReaction reaction3 = builder.NewReaction();
            reaction3.Id = "r3";
            reaction3.Reactants.Add(molB);
            reaction3.Products.Add(molD);
            reactionSet.Add(reaction3);

            IReaction reaction4 = builder.NewReaction();
            reaction4.Id = "r4";
            reaction4.Reactants.Add(molC);
            reaction4.Products.Add(molE);
            reactionSet.Add(reaction4);

            IReactionScheme scheme1 = ReactionSchemeManipulator.NewReactionScheme(reactionSet);
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
            IReactionScheme scheme1 = builder.NewReactionScheme();

            IReactionScheme scheme11 = builder.NewReactionScheme();
            IReaction reaction1 = builder.NewReaction();
            reaction1.Id = "reaction1";
            IAtomContainer startMol = builder.NewAtomContainer();
            startMol.Id = "startMol";
            reaction1.Reactants.Add(startMol);
            IAtomContainer mitMol = builder.NewAtomContainer();
            mitMol.Id = "mitMol";
            reaction1.Products.Add(mitMol);
            scheme11.Add(reaction1);
            IReaction reaction2 = builder.NewReaction();
            reaction2.Products.Add(builder.NewAtomContainer());
            reaction2.Reactants.Add(builder.NewAtomContainer());
            reaction2.Id = "reaction2";
            scheme11.Add(reaction2);
            scheme1.Add(scheme11);

            IReactionScheme scheme12 = builder.NewReactionScheme();
            IReaction reaction3 = builder.NewReaction();
            IAtomContainer finalMol = builder.NewAtomContainer();
            finalMol.Id = "finalMol";
            reaction3.Products.Add(finalMol);
            reaction3.Reactants.Add(mitMol);
            reaction3.Id = "reaction3";
            scheme12.Add(reaction3);
            scheme1.Add(scheme12);

            IReaction reaction11 = builder.NewReaction();
            reaction11.Products.Add(builder.NewAtomContainer());
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
            IReactionScheme scheme1 = builder.NewReactionScheme();

            IReactionScheme scheme11 = builder.NewReactionScheme();
            IReaction reaction1 = builder.NewReaction();
            IAtomContainer startMol = builder.NewAtomContainer();
            startMol.Id = "startMol";
            reaction1.Reactants.Add(startMol);
            IAtomContainer mitMol = builder.NewAtomContainer();
            mitMol.Id = "mitMol";
            reaction1.Products.Add(mitMol);
            scheme11.Add(reaction1);
            IReaction reaction2 = builder.NewReaction();
            reaction2.Products.Add(builder.NewAtomContainer());
            reaction2.Reactants.Add(builder.NewAtomContainer());
            scheme11.Add(reaction2);
            scheme1.Add(scheme11);

            IReactionScheme scheme12 = builder.NewReactionScheme();
            IReaction reaction3 = builder.NewReaction();
            IAtomContainer finalMol = builder.NewAtomContainer();
            finalMol.Id = "finalMol";
            reaction3.Products.Add(finalMol);
            reaction3.Reactants.Add(startMol);
            scheme12.Add(reaction3);
            scheme1.Add(scheme12);

            IReaction reaction11 = builder.NewReaction();
            reaction11.Products.Add(builder.NewAtomContainer());
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
            IReactionScheme scheme1 = builder.NewReactionScheme();
            IReaction reaction1 = builder.NewReaction();
            IAtomContainer molA = builder.NewAtomContainer();
            reaction1.Reactants.Add(molA);
            IAtomContainer molB = builder.NewAtomContainer();
            reaction1.Products.Add(molB);
            scheme1.Add(reaction1);

            IReactionScheme scheme2 = builder.NewReactionScheme();
            IReaction reaction2 = builder.NewReaction();
            reaction2.Reactants.Add(molB);
            IAtomContainer molC = builder.NewAtomContainer();
            reaction2.Products.Add(molC);
            scheme2.Add(reaction2);

            IReaction reaction3 = builder.NewReaction();
            reaction3.Reactants.Add(molB);
            IAtomContainer molD = builder.NewAtomContainer();
            reaction3.Products.Add(molD);
            scheme2.Add(reaction3);

            IReaction reaction4 = builder.NewReaction();
            IAtomContainer molE = builder.NewAtomContainer();
            reaction4.Reactants.Add(molE);
            IAtomContainer molF = builder.NewAtomContainer();
            reaction4.Products.Add(molF);
            scheme1.Add(reaction4);

            IReactionSet reactionSet = ReactionSchemeManipulator.ExtractTopReactions(scheme1);
            Assert.AreEqual(2, reactionSet.Count);
            Assert.AreEqual(reaction1, reactionSet[0]);
            Assert.AreEqual(reaction4, reactionSet[1]);
        }
    }
}

