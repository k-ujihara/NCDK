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

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the HeterolyticCleavageSBReactionTest.
    /// Generalized Reaction: A-B => |[A-] +[B+] // [A+] + |[B-]. Depending on the bond order
    /// the bond will be removed or simply the order decreased.
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class HeterolyticCleavageSBReactionTest : ReactionProcessTest
    {
        private readonly LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        private UniversalIsomorphismTester uiTester = new UniversalIsomorphismTester();

        public HeterolyticCleavageSBReactionTest()
        {
            SetReaction(typeof(HeterolyticCleavageSBReaction));
        }

        [TestMethod()]
        public void TestHeterolyticCleavageSBReaction()
        {
            IReactionProcess type = new HeterolyticCleavageSBReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: propane.
        /// CC!-!C => C[C+] + [C-]
        ///           C[C-] + [C+]
        /// </summary>
        // @cdk.inchi InChI=1/C3H8/c1-3-2/h3H2,1-2H3
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            //CreateFromSmiles("CCC")
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[7], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[8], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[9], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[10], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavageSBReaction();
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C[C+]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms[1].FormalCharge = +1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[6], BondOrder.Single); ;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C-]")
            IAtomContainer expected2 = builder.NewAtomContainer();
            expected2.Atoms.Add(builder.NewAtom("C"));
            expected2.Atoms[0].FormalCharge = -1;
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            lpcheck.Saturate(expected2);
            IAtomContainer product2 = setOfReactions[0].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(uiTester.IsIsomorph(product2, queryAtom));

            //CreateFromSmiles("C[C-]")
            expected1.Atoms[1].FormalCharge = -1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            product1 = setOfReactions[1].Products[0];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C+]")
            expected2.Atoms[0].FormalCharge = +1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            product2 = setOfReactions[1].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(uiTester.IsIsomorph(product2, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: .
        /// C[C+]!-!C => CC + [C+]
        /// </summary>
        [TestMethod()]
        public void TestCsp2ChargeSingleB()
        {
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: Propene.
        /// C=C!-!C => C=[C+] + [C-]
        ///            C=[C-] + [C+]
        /// </summary>
        // @cdk.inchi  InChI=1/C3H6/c1-3-2/h3H,1H2,2H3
        [TestMethod()]
        public void TestCsp2SingleB()
        {
            //CreateFromSmiles("C=CC")
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[7], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[8], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavageSBReaction();
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C=[C+]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms[1].FormalCharge = +1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[4], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            IQueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C-]")
            IAtomContainer expected2 = builder.NewAtomContainer();
            expected2.Atoms.Add(builder.NewAtom("C"));
            expected2.Atoms[0].FormalCharge = -1;
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            lpcheck.Saturate(expected2);

            IAtomContainer product2 = setOfReactions[0].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(uiTester.IsIsomorph(product2, queryAtom));

            //CreateFromSmiles("C=[C-]")
            expected1.Atoms[1].FormalCharge = -1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            product1 = setOfReactions[1].Products[0];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C+]")
            expected2.Atoms[0].FormalCharge = +1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);

            product2 = setOfReactions[1].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(uiTester.IsIsomorph(product2, queryAtom));

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: .
        /// C=[C+]!-!C => C=C + [C+]
        /// </summary>
        [TestMethod()]
        public void TestCspChargeSingleB()
        {

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: Propyne.
        /// C#C!-!C => C#[C+] + [C-]
        ///            C#[C-] + [C+]
        /// </summary>
        // @cdk.inchi InChI=1/C3H4/c1-3-2/h1H,2H3
        [TestMethod()]
        public void TestCspSingleB()
        {
            //CreateFromSmiles("C#CC")
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Triple);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavageSBReaction();
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C#[C-]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms[1].FormalCharge = -1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Triple);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            IAtomContainer product1 = setOfReactions[1].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C+]")
            IAtomContainer expected2 = builder.NewAtomContainer();
            expected2.Atoms.Add(builder.NewAtom("C"));
            expected2.Atoms[0].FormalCharge = +1;
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            lpcheck.Saturate(expected2);
            IAtomContainer product2 = setOfReactions[1].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(uiTester.IsIsomorph(product2, queryAtom));

            //CreateFromSmiles("C#[C+]")
            expected1.Atoms[1].FormalCharge = +1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            product1 = setOfReactions[0].Products[0];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C-]")
            expected2.Atoms[0].FormalCharge = -1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            product2 = setOfReactions[0].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(uiTester.IsIsomorph(product2, queryAtom));

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: dimethylamine.
        /// CN!-!C => C[N-] + [C+]
        /// </summary>
        [TestMethod()]
        public void TestNsp3SingleB()
        {
            //CreateFromSmiles("CNC")
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("N"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[7], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[8], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[9], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavageSBReaction();
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C[N-]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("N"));
            expected1.Atoms[1].FormalCharge = -1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[5], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C+]")
            IAtomContainer expected2 = builder.NewAtomContainer();
            expected2.Atoms.Add(builder.NewAtom("C"));
            expected2.Atoms[0].FormalCharge = +1;
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            IAtomContainer product2 = setOfReactions[0].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(uiTester.IsIsomorph(product2, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: N-methylmethanimine.
        /// C=N!-!C =>[C+] +  C=[N-]
        /// </summary>
        // @cdk.inchi InChI=1/C2H5N/c1-3-2/h1H2,2H3
        [TestMethod()]
        public void TestNsp2SingleB()
        {
            //CreateFromSmiles("C=NC")
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("N"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[7], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavageSBReaction();
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("[C+]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms[0].FormalCharge = +1;
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[1];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("C=[N-]")
            IAtomContainer expected2 = builder.NewAtomContainer();
            expected2.Atoms.Add(builder.NewAtom("C"));
            expected2.Atoms.Add(builder.NewAtom("N"));
            expected2.Atoms[1].FormalCharge = -1;
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Double);
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            lpcheck.Saturate(expected2);
            IAtomContainer product2 = setOfReactions[0].Products[0];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(uiTester.IsIsomorph(product2, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction:  methoxymethane.
        /// CO!-!C =>  C[O-] + [C+]
        /// </summary>
        // @cdk.inchi InChI=1/C2H6O/c1-3-2/h1-2H3
        [TestMethod()]
        public void TestOsp2SingleB()
        {
            //CreateFromSmiles("COC")
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[7], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[8], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavageSBReaction();
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C[O-]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("C"));
            expected1.Atoms.Add(builder.NewAtom("O"));
            expected1.Atoms[1].FormalCharge = -1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.Atoms.Add(builder.NewAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C+]")
            IAtomContainer expected2 = builder.NewAtomContainer();
            expected2.Atoms.Add(builder.NewAtom("C"));
            expected2.Atoms[0].FormalCharge = +1;
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            IAtomContainer product2 = setOfReactions[0].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(uiTester.IsIsomorph(product2, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: fluoromethane.
        /// F!-!C => [F-] + [C+]
        /// </summary>
        // @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        public void TestFspSingleB()
        {
            //CreateFromSmiles("FC")
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavageSBReaction();
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            //CreateFromSmiles("[F-]")
            IAtomContainer expected1 = builder.NewAtomContainer();
            expected1.Atoms.Add(builder.NewAtom("F"));
            expected1.Atoms[0].FormalCharge = -1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("[C+]")
            IAtomContainer expected2 = builder.NewAtomContainer();
            expected2.Atoms.Add(builder.NewAtom("C"));
            expected2.Atoms[0].FormalCharge = +1;
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.Atoms.Add(builder.NewAtom("H"));
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[1], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[2], BondOrder.Single);
            expected2.AddBond(expected2.Atoms[0], expected2.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected2);
            IAtomContainer product2 = setOfReactions[0].Products[1];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected2);
            Assert.IsTrue(uiTester.IsIsomorph(product2, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C-O => [C+] + [O-]
        /// Manually put of the reactive center.
        /// </summary>
        // @cdk.inchi  InChI=1/CH4O/c1-2/h2H,1H3
        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new HeterolyticCleavageSBReaction();
            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* manually put the reactive center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;

            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            IAtomContainer reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[0].IsReactiveCenter);
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C-O => [C+] + [O-]
        /// Test of mapped between the reactant and product. Only is mapped the reactive center.
        /// </summary>
        // @cdk.inchi  InChI=1/CH4O/c1-2/h2H,1H3
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new HeterolyticCleavageSBReaction();
            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* automatic search of the center active */
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            MakeSureAtomTypesAreRecognized(molecule);

            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer product1 = setOfReactions[0].Products[0];
            IAtomContainer product2 = setOfReactions[0].Products[1];

            Assert.AreEqual(6, setOfReactions[0].Mappings.Count);
            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA1, product1.Atoms[0]);
            IAtom mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA2, product2.Atoms[0]);
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

        /// <summary>
        /// Get the example set of molecules.
        /// </summary>
        /// <returns>The IAtomContainerSet</returns>
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
            IAtomContainer molecule = builder.NewAtomContainer();//CreateFromSmiles("CO")
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            try
            {
                AddExplicitHydrogens(molecule);
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                lpcheck.Saturate(molecule);
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
        /// Get the expected set of molecules.
        /// TODO:reaction. Set the products
        /// </summary>
        /// <returns>The IAtomContainerSet</returns>
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.NewAtomContainerSet();

            setOfProducts.Add(null);
            return setOfProducts;
        }
    }
}
