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
    /// TestSuite that runs a test for the ElectronImpactNBEReactionTest.
    /// Generalized Reaction: [A+]-B| => A=[B+].
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class SharingLonePairReactionTest : ReactionProcessTest
    {
        private IChemObjectBuilder builder = CDK.Builder;

        public SharingLonePairReactionTest()
        {
            SetReaction(typeof(SharingLonePairReaction));
        }

        [TestMethod()]
        public void TestSharingLonePairReaction()
        {
            var type = new SharingLonePairReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite. Reaction: [C+]-O => C=[O+]
        /// Automatic search of the center active.
        /// </summary>
        [TestMethod()]

        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            var type = new SharingLonePairReaction();

            var setOfReactants = GetExampleReactants();

            /* initiate */

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            var product = setOfReactions[0].Products[0];
            /* C=[O+] */
            var molecule2 = GetExpectedProducts()[0];

            var queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        /// <summary>
        /// A unit test suite. Reaction: [C+]-O => C=[O+]
        /// Manually put of the center active.
        /// </summary>
        [TestMethod()]
        public void TestManuallyCentreActive()
        {
            var type = new SharingLonePairReaction();

            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            /* manually put the center active */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */

            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            var product = setOfReactions[0].Products[0];

            /* C=[O+] */
            var molecule2 = GetExpectedProducts()[0];

            var queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            var type = new SharingLonePairReaction();

            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            /* manually put the reactive center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            var reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[0].IsReactiveCenter);
        }

        [TestMethod()]
        public void TestMapping()
        {
            var type = new SharingLonePairReaction();

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

            Assert.AreEqual(5, setOfReactions[0].Mappings.Count);

            var mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA1, product.Atoms[0]);
            var mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA2, product.Atoms[1]);
        }

        /// <summary>
        /// Test to recognize if this IAtomContainer_1 matches correctly into the CDKAtomTypes.
        /// </summary>
        [TestMethod()]
        public void TestAtomTypesAtomContainer1()
        {
            var moleculeTest = GetExampleReactants()[0];
            MakeSureAtomTypesAreRecognized(moleculeTest);
        }

        /// <summary>
        /// Test to recognize if this IAtomContainer_2 matches correctly into the CDKAtomTypes.
        /// </summary>
        [TestMethod()]
        public void TestAtomTypesAtomContainer2()
        {
            var moleculeTest = GetExpectedProducts()[0];
            MakeSureAtomTypesAreRecognized(moleculeTest);
        }

        /// <summary>
        /// get the molecule 1: [C+]-O-H
        /// </summary>
        private IChemObjectSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = CDK.Builder.NewAtomContainerSet();

            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[0].FormalCharge = 1;
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            try
            {
                AddExplicitHydrogens(molecule);
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

                CDK.LonePairElectronChecker.Saturate(molecule);
            }
            catch (Exception e)
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
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.Atoms[1].FormalCharge = 1;
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);

            try
            {
                AddExplicitHydrogens(molecule);
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

                CDK.LonePairElectronChecker.Saturate(molecule);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            setOfProducts.Add(molecule);
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
