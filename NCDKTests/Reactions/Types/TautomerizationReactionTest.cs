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
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the TautomerizationReactionTest.
    /// Generalized Reaction: X=Y-Z-H => X(H)-Y=Z.
    ///
    // @cdk.module test-reaction
    /// </summary>
    [TestClass()]
    public class TautomerizationReactionTest : ReactionProcessTest
    {

        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        public TautomerizationReactionTest()
        {
            SetReaction(typeof(TautomerizationReaction));
        }

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        [TestMethod()]
        public void TestTautomerizationReaction()
        {
            IReactionProcess type = new TautomerizationReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite for JUnit for acetaldehyde.
        /// Reaction: O=C-C-H => O(H)-C=C.
        /// Automatic looking for active center.
        ///
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]

        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {

            IReactionProcess type = new TautomerizationReaction();

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

            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));

            // reverse process
            var setOfReactants2 = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants2.Add(molecule2);

            IReactionSet setOfReactions2 = type.Initiate(setOfReactants2, null);

            Assert.AreEqual(1, setOfReactions2.Count);
            Assert.AreEqual(1, setOfReactions2[0].Products.Count);

            IAtomContainer product2 = setOfReactions2[0].Products[0];

            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit for acetaldehyde.
        /// Reaction: O=C-C-H => O(H)-C=C.
        /// Manually putting for active center.
        ///
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestManuallyCentreActive()
        {
            IReactionProcess type = new TautomerizationReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually putting the active center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Atoms[4].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;
            molecule.Bonds[3].IsReactiveCenter = true;

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
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));

            // reverse process
            /* manually putting the active center */
            molecule2.Atoms[0].IsReactiveCenter = true;
            molecule2.Atoms[1].IsReactiveCenter = true;
            molecule2.Atoms[2].IsReactiveCenter = true;
            molecule2.Atoms[6].IsReactiveCenter = true;
            molecule2.Bonds[0].IsReactiveCenter = true;
            molecule2.Bonds[1].IsReactiveCenter = true;
            molecule2.Bonds[5].IsReactiveCenter = true;
            var setOfReactants2 = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants2.Add(molecule2);

            var setOfReactions2 = type.Initiate(setOfReactants2, null);

            Assert.AreEqual(1, setOfReactions2.Count);
            Assert.AreEqual(1, setOfReactions2[0].Products.Count);

            IAtomContainer product2 = setOfReactions2[0].Products[0];

            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit.
        ///
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new TautomerizationReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually putting the active center */
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
        ///
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new TautomerizationReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* automatic looking for active center */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            /* initiate */

            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer product = setOfReactions[0].Products[0];

            Assert.AreEqual(7, setOfReactions[0].Mappings.Count);

            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA1, product.Atoms[0]);
            mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA1, product.Atoms[1]);
            mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[2]);
            Assert.AreEqual(mappedProductA1, product.Atoms[2]);
            mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[4]);
            Assert.AreEqual(mappedProductA1, product.Atoms[4]);

        }

        /// <summary>
        /// Get the Acetaldehyde structure.
        ///
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        ///
        /// <returns>The IAtomContainer</returns>
        /// <exception cref="CDKException"></exception>
        /// </summary>
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
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2-3H,1H2
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.CreateAtomContainerSet();

            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(builder.CreateAtom("O"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[6], BondOrder.Single);
            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
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
