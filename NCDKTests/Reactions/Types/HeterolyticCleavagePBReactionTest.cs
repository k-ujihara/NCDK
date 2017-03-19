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

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the HeterolyticCleavagePBReactionTest.
    /// Generalized Reaction: A=B => |[A-]-[B+] + [A+]-|[B-]. Depending of the bond order
    /// the bond will be removed or simply the order decreased.
    ///
    // @cdk.module test-reaction
    /// </summary>
    [TestClass()]
    public class HeterolyticCleavagePBReactionTest : ReactionProcessTest
    {

        private readonly LonePairElectronChecker lpcheck = new LonePairElectronChecker();
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        private UniversalIsomorphismTester uiTester = new UniversalIsomorphismTester();

        public HeterolyticCleavagePBReactionTest()
        {
            SetReaction(typeof(HeterolyticCleavagePBReaction));
        }

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        [TestMethod()]
        public void TestHeterolyticCleavagePBReaction()
        {
            IReactionProcess type = new HeterolyticCleavagePBReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: Propene.
        /// CC!=!C => C[C+][C-]
        ///           C[C-][C+]
        ///
        // @cdk.inchi InChI=1/C3H6/c1-3-2/h3H,1H2,2H3
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            //CreateFromSmiles("CC=C")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[7], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[8], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavagePBReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C[C+][C-]")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms[1].FormalCharge = +1;
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms[2].FormalCharge = -1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[6], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[7], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[8], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("C[C-][C+]")
            expected1.Atoms[1].FormalCharge = -1;
            expected1.Atoms[2].FormalCharge = +1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            product1 = setOfReactions[1].Products[0];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction:.
        /// C[C+]!=!C => C[C+][C-]
        ///           C[C-][C+]
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestCspChargeDoubleB()
        {

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: Allene.
        /// C=C!=!C => C=[C+][C-]
        ///            C=[C-][C+]
        ///
        // @cdk.inchi InChI=1/C3H4/c1-3-2/h1-2H2
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestCspDoubleB()
        {
            //CreateFromSmiles("C=C=C")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavagePBReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C=[C+][C-]")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms[1].FormalCharge = +1;
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms[2].FormalCharge = -1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[6], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("C=[C-][C+]")
            expected1.Atoms[1].FormalCharge = -1;
            expected1.Atoms[2].FormalCharge = +1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            product1 = setOfReactions[1].Products[0];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, expected1));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: Propyne.
        /// CC#C => C[C+]=[C-]
        ///         C[C-]=[C+]
        ///
        // @cdk.inchi InChI=1/C3H4/c1-3-2/h1H,2H3
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestCspTripleB()
        {
            //CreateFromSmiles("CC#C")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Triple);
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavagePBReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C[C+]=[C-]")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms[1].FormalCharge = +1;
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms[2].FormalCharge = -1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Double);
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[6], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));

            //CreateFromSmiles("C[C-]=[C+]")
            expected1.Atoms[1].FormalCharge = -1;
            expected1.Atoms[2].FormalCharge = +1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            product1 = setOfReactions[1].Products[0];
            queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: N-methylmethanimine.
        /// CN!=!C => C[N-]-[C+]
        ///
        // @cdk.inchi InChI=1/C2H5N/c1-3-2/h1H2,2H3
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestNsp2DoubleB()
        {
            //CreateFromSmiles("CN=C")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.Atoms.Add(builder.CreateAtom("N"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[7], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavagePBReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("C[N-]-[C+]")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms.Add(builder.CreateAtom("N"));
            expected1.Atoms[1].FormalCharge = -1;
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms[2].FormalCharge = +1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[3], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[4], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[5], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[6], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[2], expected1.Atoms[7], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: formonitrile.
        /// N!#!C => [N-]=[C+]
        ///
        // @cdk.inchi InChI=1/CHN/c1-2/h1H
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestNspTripleB()
        {
            //CreateFromSmiles("N#C")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(builder.CreateAtom("N"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Triple);
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavagePBReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("[N-]=[C+]")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Atoms.Add(builder.CreateAtom("N"));
            expected1.Atoms[0].FormalCharge = -1;
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms[1].FormalCharge = +1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Double);
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: formaldehyde.
        /// O!=!C => [O-][C+]
        ///
        // @cdk.inchi  InChI=1/CH2O/c1-2/h1H2
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestOspDoubleB()
        {
            //CreateFromSmiles("O=C")
            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(builder.CreateAtom("O"));
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.Atoms.Add(builder.CreateAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;

            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            setOfReactants.Add(molecule);

            IReactionProcess type = new HeterolyticCleavagePBReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);

            // expected products

            //CreateFromSmiles("[O-][C+]")
            IAtomContainer expected1 = builder.CreateAtomContainer();
            expected1.Atoms.Add(builder.CreateAtom("O"));
            expected1.Atoms[0].FormalCharge = -1;
            expected1.Atoms.Add(builder.CreateAtom("C"));
            expected1.Atoms[1].FormalCharge = +1;
            expected1.AddBond(expected1.Atoms[0], expected1.Atoms[1], BondOrder.Single);
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.Atoms.Add(builder.CreateAtom("H"));
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[2], BondOrder.Single);
            expected1.AddBond(expected1.Atoms[1], expected1.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(expected1);
            lpcheck.Saturate(expected1);
            IAtomContainer product1 = setOfReactions[0].Products[0];
            QueryAtomContainer queryAtom = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(expected1);
            Assert.IsTrue(uiTester.IsIsomorph(product1, queryAtom));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C=O => [C+]-[O-]
        /// Manually put of the reactive center.
        ///
        // @cdk.inchi InChI=1/CH2O/c1-2/h1H2
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new HeterolyticCleavagePBReaction();
            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();

            /* C=O */
            IAtomContainer molecule = builder.CreateAtomContainer();//CreateFromSmiles("C=O")
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.Atoms.Add(builder.CreateAtom("O"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            AddExplicitHydrogens(molecule);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);
            setOfReactants.Add(molecule);

            /* manually put the reactive center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Bonds[0].IsReactiveCenter = true;

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            MakeSureAtomTypesAreRecognized(molecule);

            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer reactant = setOfReactions[0].Reactants[0];
            Assert.IsTrue(molecule.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[0].IsReactiveCenter);
            Assert.IsTrue(molecule.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(reactant.Atoms[1].IsReactiveCenter);
            Assert.IsTrue(molecule.Bonds[0].IsReactiveCenter);
            Assert.IsTrue(reactant.Bonds[0].IsReactiveCenter);
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C=O => [C+]-[O-] + [C-]-[O+]
        /// Test of mapped between the reactant and product. Only is mapped the reactive center.
        ///
        // @cdk.inchi InChI=1/CH2O/c1-2/h1H2
        ///
        /// <returns>The test suite</returns>
        /// </summary>
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new HeterolyticCleavagePBReaction();
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

            IAtomContainer product = setOfReactions[0].Products[0];

            Assert.AreEqual(4, setOfReactions[0].Mappings.Count);
            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA1, product.Atoms[0]);
            IAtom mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA2, product.Atoms[1]);
        }

        /// <summary>
        /// Test to recognize if a IAtomContainer matcher correctly identifies the CDKAtomTypes.
        ///
        /// <param name="molecule">The IAtomContainer to analyze</param>
        /// <exception cref="CDKException"></exception>
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
       /// A unit test suite for JUnit. Reaction:
       /// C(H)(H)=O => [C+](H)(H)-[O-] + [C+](H)=O +
       /// Automatic search of the reactive atoms and bonds.
       ///
       // @cdk.inchi InChI=1/CH2O/c1-2/h1H2
       ///
       /// <returns>The test suite</returns>
       /// </summary>
        [TestMethod()]
        public void TestBB_AutomaticSearchCentreActiveFormaldehyde()
        {
            IReactionProcess type = new HeterolyticCleavagePBReaction();
            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* automatic search of the reactive atoms and bonds */
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;

            /* initiate */
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            MakeSureAtomTypesAreRecognized(molecule);

            var setOfReactions = type.Initiate(setOfReactants, null);
            Assert.AreEqual(1, setOfReactions.Count);

            Assert.AreEqual(1, setOfReactions[0].Products.Count);

            IAtomContainer product = setOfReactions[0].Products[0];

            Assert.IsTrue(uiTester.IsIsomorph(GetExpectedProducts()[0], product));

        }

        /// <summary>
        /// Get the example set of molecules.
        ///
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();

            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(builder.CreateAtom("C"));
            molecule.Atoms.Add(builder.CreateAtom("O"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
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
        ///
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.CreateAtomContainerSet();
            //CreateFromSmiles("[C+](H)(H)-[O-]");
            IAtomContainer molecule = builder.CreateAtomContainer();
            IAtom carbon = builder.CreateAtom("C");
            carbon.FormalCharge = 1;
            molecule.Atoms.Add(carbon);
            IAtom oxyg = builder.CreateAtom("O");
            oxyg.FormalCharge = -1;
            molecule.Atoms.Add(oxyg);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(new Atom("H"));
            molecule.Atoms.Add(new Atom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);

            setOfProducts.Add(molecule);
            return setOfProducts;
        }
    }
}
