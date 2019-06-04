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
    /// TestSuite that runs a test for the RadicalSiteHrDeltaReactionTest.
    /// Generalized Reaction: [A*]-(C)_4-C5[H] => A([H])-(C_4)-[C5*].
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class RadicalSiteHrDeltaReactionTest : ReactionProcessTest
    {
        private IChemObjectBuilder builder = CDK.Builder;

        public RadicalSiteHrDeltaReactionTest()
        {
            SetReaction(typeof(RadicalSiteHrDeltaReaction));
        }

        [TestMethod()]
        public void TestRadicalSiteHrDeltaReaction()
        {
            var type = new RadicalSiteHrDeltaReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite. Reaction: C([H])([H])([H])C([H])([H])C(=O)C([H])([H])C([H])C([H])[H]
        /// Automatic search of the center active. hexan-3-one
        /// </summary>
        // @cdk.inchi InChI=1/C6H12O/c1-3-5-6(7)4-2/h3-5H2,1-2H3
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            var type = new RadicalSiteHrDeltaReaction();
            var setOfReactants = GetExampleReactants();
            /* initiate */

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
        /// A unit test suite. Reaction: C([H])([H])([H])C([H])([H])C(=O)C([H])([H])C([H])C([H])[H]
        /// Automatic search of the center active. hexan-3-one
        /// </summary>
        // @cdk.inchi InChI=1/C6H12O/c1-3-5-6(7)4-2/h3-5H2,1-2H3
        [TestMethod()]
        public void TestManuallyCentreActive()
        {
            var type = new RadicalSiteHrDeltaReaction();
            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            molecule.Atoms[6].IsReactiveCenter = true;
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[7].IsReactiveCenter = true;
            molecule.Bonds[6].IsReactiveCenter = true;

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            var product = setOfReactions[0].Products[0];
            var molecule2 = GetExpectedProducts()[0];

            var queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        /// <summary>
        /// create the compound Hexan-3-one.
        /// </summary>
        // @cdk.inchi InChI=1/C6H12O/c1-3-5-6(7)4-2/h3-5H2,1-2H3
        private IChemObjectSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = CDK.Builder.NewAtomContainerSet();

            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[6].FormalCharge = 1;
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Single);
            try
            {
                AddExplicitHydrogens(molecule);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }
            molecule.Atoms[6].FormalCharge = 0;
            molecule.SingleElectrons.Add(CDK.Builder.NewSingleElectron(molecule.Atoms[6]));
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
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Single);
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
            var type = new RadicalSiteHrDeltaReaction();

            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            /* manually put the reactive center */
            molecule.Atoms[6].IsReactiveCenter = true;
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[7].IsReactiveCenter = true;
            molecule.Bonds[6].IsReactiveCenter = true;

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
            Assert.IsTrue(molecule.Atoms[7].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[7].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[6].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[6].IsReactiveCenter);
        }

        [TestMethod()]
        public void TestMapping()
        {
            var type = new RadicalSiteHrDeltaReaction();

            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            /* automatic search of the center active */
            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            var product = setOfReactions[0].Products[0];

            Assert.AreEqual(18, setOfReactions[0].Mappings.Count);
            var mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA1, product.Atoms[0]);
            var mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[6]);
            Assert.AreEqual(mappedProductA2, product.Atoms[6]);
            var mappedProductA3 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[7]);
            Assert.AreEqual(mappedProductA3, product.Atoms[7]);
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
