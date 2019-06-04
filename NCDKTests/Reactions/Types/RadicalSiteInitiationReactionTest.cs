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
    /// TestSuite that runs a test for the RearrangementRadicalReactionTest.
    /// Generalized Reaction: [A*]-B-C => A=B + [c*].
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class RadicalSiteInitiationReactionTest : ReactionProcessTest
    {
        private IChemObjectBuilder builder = CDK.Builder;

        public RadicalSiteInitiationReactionTest()
        {
            SetReaction(typeof(RadicalSiteInitiationReaction));
        }

        [TestMethod()]
        public void TestRadicalSiteInitiationReaction()
        {
            var type = new RadicalSiteInitiationReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite. Reaction: [C*]-C-C => C=C +[C*]
        /// Automatic search of the center active.
        /// </summary>
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            var type = new RadicalSiteInitiationReaction();
            var setOfReactants = GetExampleReactants();

            /* initiate */

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = false;
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

            /* [C*] */
            var molecule2 = GetExpectedProducts()[1];

            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            var type = new RadicalSiteInitiationReaction();
            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            /* manually put the reactive center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

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
            Assert.IsTrue(molecule.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[0].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[1].IsReactiveCenter);
        }

        [TestMethod()]
        public void TestMapping()
        {
            var type = new RadicalSiteInitiationReaction();
            var setOfReactants = GetExampleReactants();
            var molecule = setOfReactants[0];

            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;
            /* initiate */

            var setOfReactions = type.Initiate(setOfReactants, null);

            var product1 = setOfReactions[0].Products[0];
            var product2 = setOfReactions[0].Products[1];

            Assert.AreEqual(10, setOfReactions[0].Mappings.Count);
            var mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA1, product1.Atoms[1]);
            var mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[2]);
            Assert.AreEqual(mappedProductA2, product2.Atoms[0]);
            var mappedProductA3 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA3, product1.Atoms[0]);
        }

        private IChemObjectSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = CDK.Builder.NewAtomContainerSet();

            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[0].FormalCharge = 1;
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);

            try
            {
                AddExplicitHydrogens(molecule);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }
            IAtom atom = molecule.Atoms[0];
            molecule.SingleElectrons.Add(CDK.Builder.NewSingleElectron(atom));
            atom.FormalCharge = 0;
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

            /* C=C */
            var molecule1 = builder.NewAtomContainer();
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[1], BondOrder.Double);
            try
            {
                AddExplicitHydrogens(molecule1);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            /* [C*] */
            var molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.Atoms.Add(builder.NewAtom("H"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("H"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[2], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("H"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[3], BondOrder.Single);
            IAtom atom = molecule2.Atoms[0];
            molecule2.SingleElectrons.Add(CDK.Builder.NewSingleElectron(atom));

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
