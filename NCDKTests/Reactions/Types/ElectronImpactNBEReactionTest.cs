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
    /// TestSuite that runs a test for the ElectronImpactNBEReactionTest.
    ///
    // @cdk.module test-reaction
    /// </summary>
    [TestClass()]
    public class ElectronImpactNBEReactionTest : ReactionProcessTest
    {

        private readonly LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        public ElectronImpactNBEReactionTest()
        {
            SetReaction(typeof(ElectronImpactNBEReaction));
        }

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        [TestMethod()]
        public void TestElectronImpactNBEReaction()
        {
            IReactionProcess type = new ElectronImpactNBEReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        ///  A unit test for JUnit with the compound 2_5_Hexen_3_one.
        ///
        /// <returns>Description of the Return Value</returns>
        /// </summary>
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            /* Ionize(>C=O): C=CCC(=O)CC -> C=CCC(=O*)CC , set the reactive center */

            IAtomContainer reactant = builder.CreateAtomContainer();//CreateFromSmiles("C=CCC(=O)CC")
            reactant.Add(builder.CreateAtom("C"));
            reactant.Add(builder.CreateAtom("C"));
            reactant.Add(builder.CreateAtom("C"));
            reactant.Add(builder.CreateAtom("C"));
            reactant.Add(builder.CreateAtom("O"));
            reactant.Add(builder.CreateAtom("C"));
            reactant.Add(builder.CreateAtom("C"));
            reactant.AddBond(reactant.Atoms[0], reactant.Atoms[1], BondOrder.Double);
            reactant.AddBond(reactant.Atoms[1], reactant.Atoms[2], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[2], reactant.Atoms[3], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[3], reactant.Atoms[4], BondOrder.Double);
            reactant.AddBond(reactant.Atoms[3], reactant.Atoms[5], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[5], reactant.Atoms[6], BondOrder.Single);
            AddExplicitHydrogens(reactant);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactant);
            lpcheck.Saturate(reactant);

            foreach (var atom in reactant.Atoms)
            {
                if (reactant.GetConnectedLonePairs(atom).Count() > 0)
                {
                    atom.IsReactiveCenter = true;
                }
            }

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(reactant);

            /* initiate */
            MakeSureAtomTypesAreRecognized(reactant);

            IReactionProcess type = new ElectronImpactNBEReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer molecule = setOfReactions[0].Products[0];
            Assert.AreEqual(1, molecule.Atoms[4].FormalCharge.Value);
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[4]).Count());

            Assert.IsTrue(setOfReactions[0].Mappings.Any());

        }

        /// <summary>
        ///  A unit test for JUnit with the compound 2_5_Hexen_3_one.
        ///
        /// <returns>Description of the Return Value</returns>
        /// </summary>
        [TestMethod()]
        public void TestAutomatic_Set_Active_Atom()
        {
            // Ionize(>C=O): C=CCC(=O)CC -> C=CCC(=O*)CC, without setting the
            // reactive center
            IAtomContainer reactant = builder.CreateAtomContainer();//CreateFromSmiles("C=CCC(=O)CC")
            reactant.Add(builder.CreateAtom("C"));
            reactant.Add(builder.CreateAtom("C"));
            reactant.Add(builder.CreateAtom("C"));
            reactant.Add(builder.CreateAtom("C"));
            reactant.Add(builder.CreateAtom("O"));
            reactant.Add(builder.CreateAtom("C"));
            reactant.Add(builder.CreateAtom("C"));
            reactant.AddBond(reactant.Atoms[0], reactant.Atoms[1], BondOrder.Double);
            reactant.AddBond(reactant.Atoms[1], reactant.Atoms[2], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[2], reactant.Atoms[3], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[3], reactant.Atoms[4], BondOrder.Double);
            reactant.AddBond(reactant.Atoms[3], reactant.Atoms[5], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[5], reactant.Atoms[6], BondOrder.Single);
            AddExplicitHydrogens(reactant);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactant);
            lpcheck.Saturate(reactant);

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(reactant);

            /* initiate */
            MakeSureAtomTypesAreRecognized(reactant);

            IReactionProcess type = new ElectronImpactNBEReaction();
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer molecule = setOfReactions[0].Products[0];
            Assert.AreEqual(1, molecule.Atoms[4].FormalCharge.Value);
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[4]).Count());

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: methanamine.
        /// C-!N! => C[N*+]
        ///
        // @cdk.inchi  InChI=1/CH5N/c1-2/h2H2,1H3
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestNsp3SingleB()
        {
            //CreateFromSmiles("CN")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("C"));
            molecule.Add(builder.CreateAtom("N"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[6], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new ElectronImpactNBEReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C[N*+]")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Add(builder.CreateAtom("C"));
            expected1.Add(builder.CreateAtom("N"));
            expected1.Atoms[1].FormalCharge = +1;
            expected1.Add(builder.CreateSingleElectron(expected1.Atoms[1]));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[6], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);

            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product1, queryAtom));

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: Methanimine.
        /// C=!N! => C=[N*+]
        ///
        // @cdk.inchi  InChI=1/CH3N/c1-2/h2H,1H2
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestNsp2SingleB()
        {
            //CreateFromSmiles("C=N")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("N"));
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[0].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new ElectronImpactNBEReaction();

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("[N*+]=C")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Add(builder.CreateAtom("N"));
            expected1.Atoms[0].FormalCharge = 1;
            expected1.Add(builder.CreateSingleElectron(expected1.Atoms[0]));
            expected1.Add(builder.CreateAtom("C"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[4], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);

            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product1, queryAtom));

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: fluoromethane.
        /// F!-!C => [F*+]C
        ///
        // @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        ///
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestFspSingleB()
        {
            //CreateFromSmiles("FC")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Add(builder.CreateAtom("F"));
            molecule.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[0].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new ElectronImpactNBEReaction();

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("[F*+]C")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Add(builder.CreateAtom("F"));
            expected1.Atoms[0].FormalCharge = 1;
            expected1.Add(builder.CreateSingleElectron(expected1.Atoms[0]));
            expected1.Add(builder.CreateAtom("C"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[4], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);

            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product1, queryAtom));

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C=O => C=[O*+]
        /// Manually put of the reactive center.
        ///
        // @cdk.inchi InChI=1/CH2O/c1-2/h1H2
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new ElectronImpactNBEReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];
            /* manually put the reactive center */
            molecule.Atoms[1].IsReactiveCenter = true;

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[1].IsReactiveCenter);
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C=O => C=[O*+]
        /// Manually put of the reactive center.
        ///
        // @cdk.inchi InChI=1/CH2O/c1-2/h1H2
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new ElectronImpactNBEReaction();
            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* automatic search of the center active */
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer product = setOfReactions[0].Products[0];

            Assert.AreEqual(4, setOfReactions[0].Mappings.Count);
            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA1, product.Atoms[1]);
        }

        /// <summary>
        /// Test to recognize if a IAtomContainer matcher correctly the CDKAtomTypes.
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

        /// <summary>
        /// Get the example set of molecules.
        ///
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            IAtomContainer molecule = builder.CreateAtomContainer();//CreateFromSmiles("C=O")
            molecule.Add(builder.CreateAtom("C"));
            molecule.Add(builder.CreateAtom("O"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Add(builder.CreateAtom("H"));
            molecule.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);

            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                lpcheck.Saturate(molecule);
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
        /// TODO:reaction. Set the products
        ///
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.CreateAtomContainerSet();

            setOfProducts.Add(null);
            return setOfProducts;
        }
    }
}
