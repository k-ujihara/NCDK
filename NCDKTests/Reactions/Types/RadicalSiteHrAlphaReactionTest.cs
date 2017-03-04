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
using NCDK.Default;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Reactions.Types.Parameters;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the RadicalSiteHrAlphaReaction.
    ///
    // @cdk.module test-reaction
    /// </summary>
    [TestClass()]
    public class RadicalSiteHrAlphaReactionTest : ReactionProcessTest
    {

        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        public RadicalSiteHrAlphaReactionTest()
        {
            SetReaction(typeof(RadicalSiteHrAlphaReaction));
        }

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        [TestMethod()]
        public void TestRadicalSiteHrAlphaReaction()
        {
            IReactionProcess type = new RadicalSiteHrAlphaReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction:
        /// Automatic search of the center active.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]

        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            IReactionProcess type = new RadicalSiteHrAlphaReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* initiate */
            MakeSureAtomTypesAreRecognized(molecule);

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(5, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[2].Products[0];
            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));

        }

        /// <summary>
        /// create the compound
        ///
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();

            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("C"));
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Add(builder.CreateAtom("C"));
            molecule.Atoms[2].FormalCharge = 1;
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            try
            {
                AddExplicitHydrogens(molecule);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            molecule.Atoms[2].FormalCharge = 0;
            molecule.Add(new SingleElectron(molecule.Atoms[2]));

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
        ///
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.CreateAtomContainerSet();
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("C"));
            molecule.Atoms[0].FormalCharge = 1;
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Add(builder.CreateAtom("C"));
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
            molecule.Add(new SingleElectron(molecule.Atoms[0]));

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

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new RadicalSiteHrAlphaReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually put the reactive center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Atoms[6].IsReactiveCenter = true;
            molecule.Bonds[5].IsReactiveCenter = true;

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            IAtomContainer reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[2].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[6].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[6].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[5].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[5].IsReactiveCenter);
        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new RadicalSiteHrAlphaReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* automatic search of the center active */
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer product = setOfReactions[2].Products[0];

            Assert.AreEqual(19, setOfReactions[2].Mappings.Count);
            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[2],
                    molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA1, product.Atoms[0]);
            IAtom mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[2],
                    molecule.Atoms[6]);
            Assert.AreEqual(mappedProductA2, product.Atoms[6]);
            IAtom mappedProductA3 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[2],
                    molecule.Atoms[2]);
            Assert.AreEqual(mappedProductA3, product.Atoms[2]);
        }

        /// <summary>
        /// Test to recognize if a IAtomContainer matcher correctly identifies the CDKAtomTypes.
        ///
        /// <param name="molecule">The IAtomContainer to analyze</param>
        // @throws CDKException
        /// </summary>
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
