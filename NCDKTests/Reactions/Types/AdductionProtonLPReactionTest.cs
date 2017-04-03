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
    /// <summary>
    /// TestSuite that runs a test for the AdductionProtonLPReactionTest.
    /// Generalized Reaction: [X-] + [H+] => X -H.
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class AdductionProtonLPReactionTest : ReactionProcessTest
    {
        private readonly LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        private UniversalIsomorphismTester uiTester = new UniversalIsomorphismTester();

        public AdductionProtonLPReactionTest()
        {
            SetReaction(typeof(AdductionProtonLPReaction));
        }

        [TestMethod()]
        public void TestAdductionProtonLPReaction()
        {
            IReactionProcess type = new AdductionProtonLPReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite for acetaldehyde.
        /// Reaction: O=C-C-H => O(H)-C=C.
        /// Automatically looks for the active centre.
        /// </summary>
        /// <returns>The test suite</returns>
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            IReactionProcess type = new AdductionProtonLPReaction();

            /* initiate */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
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

        /// <summary>
        /// A unit test suite for JUnit for acetaldehyde.
        /// Reaction: O=C-C-H => O(H)-C=C.
        /// Manually tests for active centre.
        /// </summary>
        /// <returns>The test suite</returns>
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        [TestMethod()]
        public void TestManuallyCentreActive()
        {
            IReactionProcess type = new AdductionProtonLPReaction();
            var setOfReactants = GetExampleReactants();

            /* manually putting the active center */
            setOfReactants[0].Atoms[0].IsReactiveCenter = true;

            /* initiate */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
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

        /// <summary>
        /// A unit test suite for JUnit.
        /// </summary>
        /// <returns>The test suite</returns>
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
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

            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
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

        /// <summary>
        /// A unit test suite for JUnit.
        /// </summary>
        /// <returns>The test suite</returns>
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new AdductionProtonLPReaction();

            /* automatic looking for active center */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];
            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer product = setOfReactions[0].Products[0];
            Assert.AreEqual(8, setOfReactions[0].Mappings.Count);
            IAtom mappedProductA0 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0], molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA0, product.Atoms[0]);
        }

        /// <summary>
        /// Get the Acetaldehyde structure.
        /// </summary>
        /// <returns>The IAtomContainerSet</returns>
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(builder.CreateAtom("O"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
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

        /// <summary>
        /// Get the expected set of molecules.
        /// </summary>
        /// <returns>The IAtomContainerSet</returns>
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.CreateAtomContainerSet();
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(builder.CreateAtom("O"));
            molecule.Atoms[0].FormalCharge = 1;
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
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
