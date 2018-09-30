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
using NCDK.Silent;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the PiBondingMovementReactionTest.
    /// Generalized Reaction: C1=C(C)-C(C)=C-C=C1 -> C1(C)=C(C)-C=C-C=C1.
    /// FIXME: REACT: The tests fail if I don't put the smiles, strange
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class PiBondingMovementReactionTest : ReactionProcessTest
    {
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        public PiBondingMovementReactionTest()
        {
            SetReaction(typeof(PiBondingMovementReaction));
        }

        [TestMethod()]
        public void TestPiBondingMovementReaction()
        {
            IReactionProcess type = new PiBondingMovementReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite for JUnit with benzene.
        /// Reaction:  C1=CC=CC=C1 -> C1(C)=C(C)-C=C-C=C1
        /// Automatic search of the center active.
        ///
        /// InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H
        /// </summary>
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            IReactionProcess type = new PiBondingMovementReaction();
            // C1=C(C)-C(C)=C-C=C1
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[0], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            MakeSureAtomTypesAreRecognized(molecule);

            var setOfReactants = ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(molecule);

            /* initiate */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product2 = setOfReactions[0].Products[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule, queryAtom));

        }

        /// <summary>
        /// A unit test suite for JUnit with 1,2-dimethylbenzene.
        /// Reaction: C1=C(C)-C(C)=C-C=C1 -> C1(C)=C(C)-C=C-C=C1
        /// Automatic search of the center active.
        ///
        /// InChI=1/C8H10/c1-7-5-3-4-6-8(7)2/h3-6H,1-2H3
        /// </summary>
        [TestMethod()]
        public void TestAutomaticSearchCentreActiveExample1()
        {
            IReactionProcess type = new PiBondingMovementReaction();
            // C1=C(C)-C(C)=C-C=C1
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[5], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[7], molecule.Atoms[0], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            MakeSureAtomTypesAreRecognized(molecule);

            var setOfReactants = ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(molecule);

            /* initiate */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product2 = setOfReactions[0].Products[0];

            //C1(C)=C(C)-C=C-C=C1
            IAtomContainer molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[2], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[2], molecule2.Atoms[3], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[2], molecule2.Atoms[4], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[4], molecule2.Atoms[5], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[5], molecule2.Atoms[6], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[6], molecule2.Atoms[7], BondOrder.Double);
            molecule2.AddBond(molecule2.Atoms[7], molecule2.Atoms[0], BondOrder.Single);

            AddExplicitHydrogens(molecule2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule2);
            MakeSureAtomTypesAreRecognized(molecule2);

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        static bool Matches(IAtomContainer a, IAtomContainer b)
        {
            IQueryAtomContainer query = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(b);
            return new UniversalIsomorphismTester().IsIsomorph(a, query);
        }

        /// <summary>
        /// A unit test suite for JUnit with 2-methylnaphthalene.
        /// Reaction: C1=CC(=CC2=C1C=CC=C2)C
        /// -> C1=CC(=CC2=CC=CC=C12)C + C1=C2C(=CC(=C1)C)C=CC=C2
        /// Automatic search of the center active.
        ///
        /// InChI=1/C11H10/c1-9-6-7-10-4-2-3-5-11(10)8-9/h2-8H,1H3
        /// </summary>
        [TestMethod()]
        public void TestDoubleRingConjugated()
        {
            IReactionProcess type = new PiBondingMovementReaction();
            // C1=CC(=CC2=C1C=CC=C2)C
            var setOfReactants = GetExampleReactants();

            /* initiate */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product1 = setOfReactions[0].Products[0];
            IAtomContainer molecule1 = GetExpectedProducts()[0];

            Assert.AreEqual(1, setOfReactions[1].Products.Count);

            IAtomContainer product2 = setOfReactions[1].Products[0];
            //C1=CC(=CC2=CC=CC=C12)C
            IAtomContainer molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[1], molecule2.Atoms[2], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[2], molecule2.Atoms[3], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[3], molecule2.Atoms[4], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[4], molecule2.Atoms[5], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[5], molecule2.Atoms[6], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[6], molecule2.Atoms[7], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[7], molecule2.Atoms[8], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[8], molecule2.Atoms[9], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[9], molecule2.Atoms[10], BondOrder.Single);
            molecule2.AddBond(molecule2.Atoms[10], molecule2.Atoms[1], BondOrder.Double);
            molecule2.AddBond(molecule2.Atoms[9], molecule2.Atoms[4], BondOrder.Single);

            AddExplicitHydrogens(molecule2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule2);
            MakeSureAtomTypesAreRecognized(molecule2);

            // order depends on ring perception order (may change between versions) we check both
            // combinations just in case
            Assert.IsTrue((Matches(molecule2, product1) && Matches(molecule1, product2))
                    || (Matches(molecule1, product1) && Matches(molecule2, product2)));

        }

        /// <summary>
        /// A unit test suite for JUnit with 2-methylnaphthalene.
        /// Reaction: C1=CC(=CC2=C1C=CC=C2)C
        /// -> C1=CC(=CC2=CC=CC=C12)C + {NO => C1=C2C(=CC(=C1)C)C=CC=C2}
        ///
        /// restricted the reaction center.
        ///
        /// InChI=1/C11H10/c1-9-6-7-10-4-2-3-5-11(10)8-9/h2-8H,1H3
        /// </summary>
        [TestMethod()]
        public void TestDoubleRingConjugated2()
        {
            IReactionProcess type = new PiBondingMovementReaction();
            // C1=CC(=CC2=C1C=CC=C2)C

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually putting the reaction center */
            molecule.Bonds[1].IsReactiveCenter = true;
            molecule.Bonds[2].IsReactiveCenter = true;
            molecule.Bonds[3].IsReactiveCenter = true;
            molecule.Bonds[9].IsReactiveCenter = true;
            molecule.Bonds[10].IsReactiveCenter = true;
            molecule.Bonds[11].IsReactiveCenter = true;

            /* initiate */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product2 = setOfReactions[0].Products[0];

            IAtomContainer molecule2 = GetExpectedProducts()[0];

            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(product2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, queryAtom));
        }

        /// <summary>
        /// Create one of the resonance for 2-methylnaphthalene.
        /// C1=CC(=CC2=C1C=CC=C2)C
        /// </summary>
        private IChemObjectSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = ChemObjectBuilder.Instance.NewAtomContainerSet();
            // C{0}1=C{1}C{2}(=C{3}C{4}2=C{5}1C{6}=C{7}C{8}=C{9}2)C{10}
            // C1=CC(=CC2=C1C=CC=C2)C
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[7], molecule.Atoms[8], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[8], molecule.Atoms[9], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[9], molecule.Atoms[10], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[10], molecule.Atoms[1], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[9], molecule.Atoms[4], BondOrder.Double);

            try
            {
                AddExplicitHydrogens(molecule);
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                MakeSureAtomTypesAreRecognized(molecule);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            setOfReactants.Add(molecule);
            return setOfReactants;
        }

        /// <summary>
        /// Get the expected set of molecules. 2-methylnaphthalene.
        /// C=1C=CC2=CC(=CC=C2(C=1))C
        /// </summary>
        private IChemObjectSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.NewAtomContainerSet();

            //C=1C=CC2=CC(=CC=C2(C=1))C
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[7], molecule.Atoms[8], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[8], molecule.Atoms[9], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[9], molecule.Atoms[10], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[10], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[9], molecule.Atoms[4], BondOrder.Single);

            try
            {
                AddExplicitHydrogens(molecule);
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                MakeSureAtomTypesAreRecognized(molecule);
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
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(molecule.Builder);
            foreach (var nextAtom in molecule.Atoms)
            {
                Assert.IsNotNull(matcher.FindMatchingAtomType(molecule, nextAtom), "Missing atom type for: " + nextAtom);
            }
        }
    }
}
