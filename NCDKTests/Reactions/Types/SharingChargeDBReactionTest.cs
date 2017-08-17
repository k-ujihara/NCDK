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
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the SharingChargeDBReactionTest.
    /// Generalized Reaction: [A+]=B => A|-[B+].
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class SharingChargeDBReactionTest : ReactionProcessTest
    {
        private readonly LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        public SharingChargeDBReactionTest()
        {
            SetReaction(typeof(SharingChargeDBReaction));
        }

        [TestMethod()]
        public void TestSharingChargeDBReaction()
        {
            IReactionProcess type = new SharingChargeDBReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C-C=[O+] => C-[C+]O|
        /// Automatic search of the center active.
        /// </summary>
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            IReactionProcess type = new SharingChargeDBReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* initiate */

            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[0].Products[0];
            Assert.AreEqual(1, product.Atoms[1].FormalCharge.Value);
            Assert.AreEqual(0, product.GetConnectedLonePairs(product.Atoms[1]).Count());

            /* C[C+]O| */
            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C-C=[O+] => C-[C+]O|
        /// Manually put of the center active.
        /// </summary>
        [TestMethod()]
        public void TestManuallyCentreActive()
        {
            IReactionProcess type = new SharingChargeDBReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually put the center active */
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */

            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[0].Products[0];

            /* C-[C+]O| */
            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new SharingChargeDBReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually put the reactive center */
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[1].IsReactiveCenter);
        }
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new SharingChargeDBReaction();

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

            IAtomContainer product = setOfReactions[0].Products[0];

            Assert.AreEqual(8, setOfReactions[0].Mappings.Count);

            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA1, product.Atoms[1]);
            mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[2]);
            Assert.AreEqual(mappedProductA1, product.Atoms[2]);
        }

        /// <summary>
        /// Test to recognize if this IAtomContainer_1 matches correctly into the CDKAtomTypes.
        /// </summary>
        [TestMethod()]
        public void TestAtomTypesAtomContainer1()
        {
            IAtomContainer moleculeTest = GetExampleReactants()[0];
            MakeSureAtomTypesAreRecognized(moleculeTest);
        }

        /// <summary>
        /// Test to recognize if this IAtomContainer_2 matches correctly into the CDKAtomTypes.
        /// </summary>
        [TestMethod()]
        public void TestAtomTypesAtomContainer2()
        {
            IAtomContainer moleculeTest = GetExpectedProducts()[0];
            MakeSureAtomTypesAreRecognized(moleculeTest);
        }

        /// <summary>
        /// get the molecule 1: C-C=[O+]
        ///
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.Atoms[2].FormalCharge = 1;
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            try
            {
                AddExplicitHydrogens(molecule);

                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                lpcheck.Saturate(molecule);
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
        /// <returns>The IAtomContainerSet</returns>
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.NewAtomContainerSet();
            //C[C+]O|
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[1].FormalCharge = 1;
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);

            try
            {
                AddExplicitHydrogens(molecule);

                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                lpcheck.Saturate(molecule);
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
        /// <exception cref="CDKException"></exception>
        private void MakeSureAtomTypesAreRecognized(IAtomContainer molecule)
        {
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(molecule.Builder);

            foreach (var nextAtom in molecule.Atoms)
            {
                Assert.IsNotNull(matcher.FindMatchingAtomType(molecule, nextAtom), "Missing atom type for: " + nextAtom);
            }
        }
    }
}
