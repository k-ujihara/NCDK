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
    /// TestSuite that runs a test for the RadicalSiteHrGammaReaction.
    /// Generalized Reaction: [A*]-(C)_3-C4[H] => A([H])-(C_3)-[C4*].
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class RadicalSiteHrGammaReactionTest : ReactionProcessTest
    {
        private IChemObjectBuilder builder = CDK.Builder;

        public RadicalSiteHrGammaReactionTest()
        {
            SetReaction(typeof(RadicalSiteHrGammaReaction));
        }

        [TestMethod()]
        public void TestRadicalSiteHrGammaReaction()
        {
            var type = new RadicalSiteHrGammaReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite. Reaction: [A*]-C1-C2-C3[H] => A([H])-C1-C2-[C3*]
        /// Automatic search of the center active.
        /// </summary>
        /// <returns>The test suite</returns>
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            var type = new RadicalSiteHrGammaReaction();
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
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            var product = setOfReactions[0].Products[0];
            var molecule2 = GetExpectedProducts()[0];

            var queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        /// <summary>
        /// create the compound
        /// </summary>
        /// <returns>The IAtomContainer</returns>
        private IChemObjectSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = CDK.Builder.NewAtomContainerSet();

            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[4].FormalCharge = 1;
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            try
            {
                AddExplicitHydrogens(molecule);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            molecule.Atoms[4].FormalCharge = 0;
            molecule.SingleElectrons.Add(CDK.Builder.NewSingleElectron(molecule.Atoms[4]));

            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
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
        /// <returns>The IAtomContainerSet</returns>
        private IChemObjectSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.NewAtomContainerSet();

            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[0].FormalCharge = 1;
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            try
            {
                AddExplicitHydrogens(molecule);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            molecule.Atoms[0].FormalCharge = 0;
            molecule.SingleElectrons.Add(CDK.Builder.NewSingleElectron(molecule.Atoms[0]));

            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                MakeSureAtomTypesAreRecognized(molecule);
            }
            catch (CDKException e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            setOfProducts.Add(molecule);
            return setOfProducts;
        }

        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            var type = new RadicalSiteHrGammaReaction();
            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            /* manually put the reactive center */
            molecule.Atoms[4].IsReactiveCenter = true;
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[6].IsReactiveCenter = true;
            molecule.Bonds[5].IsReactiveCenter = true;

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            var reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[6].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[6].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[4].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[4].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[5].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[5].IsReactiveCenter);
        }

        [TestMethod()]
        public void TestMapping()
        {
            var type = new RadicalSiteHrGammaReaction();
            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            /* automatic search of the center active */
            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            MakeSureAtomTypesAreRecognized(molecule);

            var setOfReactions = type.Initiate(setOfReactants, null);

            var product = setOfReactions[0].Products[0];

            Assert.AreEqual(19, setOfReactions[0].Mappings.Count);
            var mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA1, product.Atoms[0]);
            var mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[6]);
            Assert.AreEqual(mappedProductA2, product.Atoms[6]);
            var mappedProductA3 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[4]);
            Assert.AreEqual(mappedProductA3, product.Atoms[4]);
        }

        /// <summary>
        /// Test to recognize if a IAtomContainer matcher correctly identifies the CDKAtomTypes.
        /// </summary>
        /// <param name="molecule">The IAtomContainer to analyze</param>
        /// <exception cref="CDKException"></exception>
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
