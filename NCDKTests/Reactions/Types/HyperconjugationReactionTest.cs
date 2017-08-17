/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.AtomTypes;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Reactions.Types.Parameters;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the HyperconjugationReactionTest.
    /// Generalized Reaction: [C1+]-C2 => C1=C2 + [H+].
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class HyperconjugationReactionTest : ReactionProcessTest
    {
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        public HyperconjugationReactionTest()
        {
            SetReaction(typeof(HyperconjugationReaction));
        }

        [TestMethod()]
        public void TestHyperconjugationReaction()
        {
            IReactionProcess type = new HyperconjugationReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: [C+]-CC => C=CC + [H+]
        /// Automatic search for the active center.
        /// </summary>
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            IReactionProcess type = new HyperconjugationReaction();
            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();

            /* [C+]CC */
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[0].FormalCharge = 1;
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            AddExplicitHydrogens(molecule);

            setOfReactants.Add(molecule);

            /* automatic search of the center active */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            MakeSureAtomTypesAreRecognized(molecule);

            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[0].Products[0];
            /* C=CC */
            IAtomContainer molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[1], molecule2.Atoms[2], BondOrder.Single);
            AddExplicitHydrogens(molecule2);

            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, product));

            product = setOfReactions[0].Products[1];
            /* [H+] */
            molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(builder.NewAtom("H"));
            molecule2.Atoms[0].FormalCharge = 1;

            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, product));
        }

        /// <summary>
        /// Test to recognize if a IAtomContainer matcher correctly identifies the CDKAtomTypes.
        /// </summary>
        /// <param name="molecule">The IAtomContainer to analyze</param>
        private void MakeSureAtomTypesAreRecognized(IAtomContainer molecule)
        {
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(molecule.Builder);
            foreach (var nextAtom in molecule.Atoms)
            {
                Assert.IsNotNull(matcher.FindMatchingAtomType(molecule, nextAtom), "Missing atom type for: " + nextAtom);
            }
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: [C-][C+]-C => [C-]C=C + [H+]
        /// Manually put of the center active.
        /// </summary>
        [TestMethod()]
        public void TestManuallyCentreActive()
        {
            IReactionProcess type = new HyperconjugationReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually put the center active */
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Atoms[6].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;
            molecule.Bonds[5].IsReactiveCenter = true;

            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */

            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[0].Products[0];

            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: [C-][C+]-C => [C-]C=C + [H+]
        /// Automatic search for the active center.
        /// </summary>
        [TestMethod()]
        public void TestWithNegativeCharge()
        {
            IReactionProcess type = new HyperconjugationReaction();

            var setOfReactants = GetExampleReactants();

            /* automatic search of the center active */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(3, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[0].Products[0];

            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product, queryAtom));

            product = setOfReactions[0].Products[1];

            molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(builder.NewAtom("H"));
            molecule2.Atoms[0].FormalCharge = 1;

            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new HyperconjugationReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually put the reactive center */
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Atoms[6].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;
            molecule.Bonds[5].IsReactiveCenter = true;

            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer reactant1 = setOfReactions[0].Reactants[0];

            Assert.IsTrue(molecule.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(reactant1.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(reactant1.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[6].IsReactiveCenter);
            Assert.IsTrue(reactant1.Atoms[6].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[1].IsReactiveCenter);
            Assert.IsTrue(reactant1.Bonds[1].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[5].IsReactiveCenter);
            Assert.IsTrue(reactant1.Bonds[5].IsReactiveCenter);
        }

        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new HyperconjugationReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* automatic search of the center active */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer product1 = setOfReactions[0].Products[0];
            IAtomContainer product2 = setOfReactions[0].Products[1];

            Assert.AreEqual(3, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            // if these assertions fail the atom order may have changed
            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA1, product1.Atoms[1]);
            mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[2]);
            Assert.AreEqual(mappedProductA1, product1.Atoms[2]);
            mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[6]);
            Assert.AreEqual(mappedProductA1, product2.Atoms[0]);
        }

        /// <summary>
        /// get the molecule 1: [C-]-[C+] - C
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[0].FormalCharge = -1;
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[1].FormalCharge = +1;
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            try
            {
                AddExplicitHydrogens(molecule);
                MakeSureAtomTypesAreRecognized(molecule);

            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            setOfReactants.Add(molecule);
            return setOfReactants;
        }

        /// <summary>
        /// Get the expected set of molecules. [C-]-C = C
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.NewAtomContainerSet();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[0].FormalCharge = -1;
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);

            try
            {
                AddExplicitHydrogens(molecule);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            setOfProducts.Add(molecule);
            return setOfProducts;
        }
    }
}
