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
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.Reactions.Types
{
    /**
     * TestSuite that runs a test for the AdductionProtonLPReactionTest.
     * Generalized Reaction: [X-] + [H+] => X -H.
     *
     * @cdk.module test-reaction
     */
    [TestClass()]
    public class AdductionProtonLPReactionTest : ReactionProcessTest
    {

        private readonly LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        private UniversalIsomorphismTester uiTester = new UniversalIsomorphismTester();

        /**
         *  The JUnit setup method
         */
        public AdductionProtonLPReactionTest()
        {
            SetReaction(typeof(AdductionProtonLPReaction));
        }

        /**
         *  The JUnit setup method
         */
        [TestMethod()]
        public void TestAdductionProtonLPReaction()
        {
            IReactionProcess type = new AdductionProtonLPReaction();
            Assert.IsNotNull(type);
        }

        /**
         * A unit test suite for JUnit for acetaldehyde.
         * Reaction: O=C-C-H => O(H)-C=C.
         * Automatically looks for the active centre.
         *
         * @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
         *
         * @return    The test suite
         */
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {

            IReactionProcess type = new AdductionProtonLPReaction();

            /* initiate */
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(GetExampleReactants(), null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[0].Products[0];

            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(uiTester.IsIsomorph(molecule2, queryAtom));

        }

        /**
         * A unit test suite for JUnit for acetaldehyde.
         * Reaction: O=C-C-H => O(H)-C=C.
         * Manually tests for active centre.
         *
         * @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestManuallyCentreActive()
        {
            IReactionProcess type = new AdductionProtonLPReaction();
            var setOfReactants = GetExampleReactants();

            /* manually putting the active center */
            setOfReactants[0].Atoms[0].IsReactiveCenter = true;

            /* initiate */
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[0].Products[0];

            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(uiTester.IsIsomorph(molecule2, queryAtom));

        }

        /**
         * A unit test suite for JUnit.
         *
         * @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new AdductionProtonLPReaction();
            var setOfReactants = GetExampleReactants();

            /* manually putting the active center */
            IAtomContainer molecule = setOfReactants[0];
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Atoms[4].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;
            molecule.Bonds[3].IsReactiveCenter = true;

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;
            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[4].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[4].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[0].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[1].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[3].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[3].IsReactiveCenter);
        }

        /**
         * A unit test suite for JUnit.
         *
         * @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new AdductionProtonLPReaction();

            /* automatic looking for active center */
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];
            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer product = setOfReactions[0].Products[0];

            Assert.AreEqual(8, setOfReactions[0].Mappings.Count);

            IAtom mappedProductA0 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA0, product.Atoms[0]);

        }

        /**
         * Get the Acetaldehyde structure.
         *
         * @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
         *
         * @return The IAtomContainerSet
         */
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("O"));
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);

            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

                lpcheck.Saturate(molecule);
            }
            catch (CDKException e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            setOfReactants.Add(molecule);
            return setOfReactants;
        }

        /**
         * Get the expected set of molecules.
         *
         * @return The IAtomContainerSet
         */
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.CreateAtomContainerSet();
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("O"));
            molecule.Atoms[0].FormalCharge = 1;
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[7], BondOrder.Single);
            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

                lpcheck.Saturate(molecule);
            }
            catch (CDKException e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            setOfProducts.Add(molecule);
            return setOfProducts;
        }
    }
}
