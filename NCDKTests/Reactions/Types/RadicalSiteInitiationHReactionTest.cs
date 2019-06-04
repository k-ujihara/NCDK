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
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Reactions.Types.Parameters;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the RadicalSiteInitiationHReactionTest.
    /// Generalized Reaction: [A*]-B-H => A=B + [H*].
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class RadicalSiteInitiationHReactionTest : ReactionProcessTest
    {
        private IChemObjectBuilder builder = CDK.Builder;

        public RadicalSiteInitiationHReactionTest()
        {
            SetReaction(typeof(RadicalSiteInitiationHReaction));
        }

        [TestMethod()]
        public void TestRadicalSiteInitiationHReaction()
        {
            var type = new RadicalSiteInitiationHReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite. Reaction: [C*]([H])([H])C([H])([H])[H] => C=C +[H*]
        /// Automatic search of the center active.
        /// </summary>
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            var type = new RadicalSiteInitiationHReaction();
            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            /* initiate */
            MakeSureAtomTypesAreRecognized(molecule);

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(3, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            var product1 = setOfReactions[0].Products[0];

            /* C=C */
            var molecule1 = GetExpectedProducts()[0];

            var queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule1, queryAtom));

            var product2 = setOfReactions[0].Products[1];

            /* [H*] */
            var molecule2 = GetExpectedProducts()[1];

            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
       }

        /// <summary>
        /// A unit test suite. Reaction: [C*]([H])([H])C([H])([H])[H] => C=C +[H*]
        /// Automatic search of the center active.
        /// </summary>
        [TestMethod()]
        public void TestManuallyCentreActive()
        {
            var type = new RadicalSiteInitiationHReaction();
            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            /* initiate */
            MakeSureAtomTypesAreRecognized(molecule);

            /* manually put the reactive center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[3].IsReactiveCenter = true;
            molecule.Atoms[4].IsReactiveCenter = true;
            molecule.Bonds[2].IsReactiveCenter = true;
            molecule.Bonds[3].IsReactiveCenter = true;

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            var product1 = setOfReactions[0].Products[0];

            /* C=C */
            var molecule1 = GetExpectedProducts()[0];

            var queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule1, queryAtom));

            var product2 = setOfReactions[0].Products[1];

            /* [H*] */
            var molecule2 = GetExpectedProducts()[1];

            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            var type = new RadicalSiteInitiationHReaction();
            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            /* initiate */
            MakeSureAtomTypesAreRecognized(molecule);

            /* manually put the reactive center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[3].IsReactiveCenter = true;
            molecule.Atoms[4].IsReactiveCenter = true;
            molecule.Bonds[2].IsReactiveCenter = true;
            molecule.Bonds[3].IsReactiveCenter = true;

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            var reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[3].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[3].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[4].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[4].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[2].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[2].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[3].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[3].IsReactiveCenter);
        }

        [TestMethod()]
        public void TestMapping()
        {
            var type = new RadicalSiteInitiationHReaction();
            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];
            /* initiate */

            /* manually put the reactive center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[3].IsReactiveCenter = true;
            molecule.Atoms[4].IsReactiveCenter = true;
            molecule.Bonds[2].IsReactiveCenter = true;
            molecule.Bonds[3].IsReactiveCenter = true;

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            var product = setOfReactions[0].Products[0];

            Assert.AreEqual(7, setOfReactions[0].Mappings.Count);

            var mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA1, product.Atoms[0]);
            var mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA2, product.Atoms[0]);
            var mappedProductA3 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[3]);
            Assert.AreEqual(mappedProductA3, product.Atoms[3]);
        }

        private IChemObjectSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = CDK.Builder.NewAtomContainerSet();

            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[6], BondOrder.Single);

            IAtom atom = molecule.Atoms[0];
            molecule.SingleElectrons.Add(CDK.Builder.NewSingleElectron(atom));
            try
            {
                MakeSureAtomTypesAreRecognized(molecule);
            }
            catch (CDKException e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }
            setOfReactants.Add(molecule);
            return setOfReactants;
        }

        /// <summary>
        /// Get the expected set of molecules.
        /// </summary>
        private IChemObjectSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.NewAtomContainerSet();

            var molecule1 = builder.NewAtomContainer();
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.Atoms.Add(builder.NewAtom("H"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[1], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("H"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[2], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[3], BondOrder.Double);
            molecule1.Atoms.Add(builder.NewAtom("H"));
            molecule1.AddBond(molecule1.Atoms[3], molecule1.Atoms[4], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("H"));
            molecule1.AddBond(molecule1.Atoms[3], molecule1.Atoms[5], BondOrder.Single);

            /* [H*] */
            var molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(CDK.Builder.NewAtom("H"));
            molecule2.SingleElectrons.Add(CDK.Builder.NewSingleElectron(molecule2.Atoms[0]));

            setOfProducts.Add(molecule1);
            setOfProducts.Add(molecule2);
            return setOfProducts;
        }

        /// <summary>
        /// Test to recognize if a IAtomContainer matcher correctly identifies the CDKAtomTypes.
        /// </summary>
        /// <param name="molecule">The IAtomContainer to analyze</param>
        private void MakeSureAtomTypesAreRecognized(IAtomContainer molecule)
        {
            var matcher = CDK.AtomTypeMatcher;
            foreach (var nextAtom in molecule.Atoms)
            {
                Assert.IsNotNull(matcher.FindMatchingAtomType(molecule, nextAtom), "Missing atom type for: " + nextAtom);
            }
        }
    }
}
