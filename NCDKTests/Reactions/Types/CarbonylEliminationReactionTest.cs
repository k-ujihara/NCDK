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
using NCDK.Silent;
using NCDK.Isomorphisms;
using NCDK.Reactions.Types.Parameters;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the CarbonylEliminationReactionTest.
    /// Generalized Reaction: RC-C#[O+] => R[C] + |C#[O+]
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class CarbonylEliminationReactionTest : ReactionProcessTest
    {
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        public CarbonylEliminationReactionTest()
        {
            SetReaction(typeof(CarbonylEliminationReaction));
        }

        [TestMethod()]
        public void TestCarbonylEliminationReaction()
        {
            IReactionProcess type = new CarbonylEliminationReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C-C#[O+] => [C+] + [|C-]#[O+]
        /// Automatically looks for active centre.
        /// </summary>
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            IReactionProcess type = new CarbonylEliminationReaction();
            /* [C*]-C-C */
            var setOfReactants = GetExampleReactants();

            /* initiate */
            var paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter
            {
                IsSetParameter = false
            };
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            IAtomContainer product1 = setOfReactions[0].Products[0];
            IAtomContainer molecule1 = GetExpectedProducts()[0];//CreateFromSmiles("[C+]");
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule1, product1));

            IAtomContainer product2 = setOfReactions[0].Products[1];
            IAtomContainer molecule2 = GetExpectedProducts()[1];//CreateFromSmiles("[C-]#[O+]");
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, product2));
        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C-C#[O+] => [C+] + [|C-]#[O+]
        /// Automatically looks for active centre.
        /// </summary>
        [TestMethod()]
        public void TestManuallyPCentreActiveExample1()
        {

            IReactionProcess type = new CarbonylEliminationReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* initiate */
            /* manually put the reactive center */
            molecule.Atoms[0].IsReactiveCenter = true;
            molecule.Atoms[4].IsReactiveCenter = true;
            molecule.Atoms[5].IsReactiveCenter = true;
            molecule.Bonds[3].IsReactiveCenter = true;
            molecule.Bonds[4].IsReactiveCenter = true;

            var paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter
            {
                IsSetParameter = true
            };
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(1, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            IAtomContainer product1 = setOfReactions[0].Products[0];
            IAtomContainer molecule1 = GetExpectedProducts()[0];//CreateFromSmiles("[C+]");
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule1, product1));

            IAtomContainer product2 = setOfReactions[0].Products[1];
            IAtomContainer molecule2 = GetExpectedProducts()[1];//CreateFromSmiles("[C-]#[O+]");
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(molecule2, product2));

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction: C-C#[O+] => [C+] + [|C-]#[O+]
        /// Automatically looks for active centre.
        /// </summary>
        [TestMethod()]
        public void TestMappingExample1()
        {

            IReactionProcess type = new CarbonylEliminationReaction();

            var setOfReactants = GetExampleReactants();
            IAtomContainer molecule = setOfReactants[0];

            /* initiate */
            var paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter
            {
                IsSetParameter = false
            };
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            IAtomContainer product1 = setOfReactions[0].Products[0];
            IAtomContainer product2 = setOfReactions[0].Products[1];

            Assert.AreEqual(6, setOfReactions[0].Mappings.Count);
            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA1, product1.Atoms[0]);
            IAtom mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[4]);
            Assert.AreEqual(mappedProductA2, product2.Atoms[0]);
        }

        /// <summary>
        /// Test to recognize if a IAtomContainer matcher correctly the CDKAtomTypes.
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

        /// <summary>
        /// Get the example set of molecules.
        /// </summary>
        private IChemObjectSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = ChemObjectBuilder.Instance.NewAtomContainerSet();
            IAtomContainer molecule = builder.NewAtomContainer();//CreateFromSmiles("C-C#[O+]")
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[4], BondOrder.Single);
            IAtom oxy = builder.NewAtom("O");
            oxy.FormalCharge = 1;
            molecule.Atoms.Add(oxy);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Triple);

            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

                CDK.LonePairElectronChecker.Saturate(molecule);
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

            IAtomContainer molecule1 = builder.NewAtomContainer();//CreateFromSmiles("[C+]");
            IAtom carb = builder.NewAtom("C");
            carb.FormalCharge = 1;
            molecule1.Atoms.Add(carb);
            molecule1.Atoms.Add(builder.NewAtom("H"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[1], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("H"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[2], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("H"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[3], BondOrder.Single);

            IAtomContainer molecule2 = builder.NewAtomContainer();//CreateFromSmiles("[C-]#[O+]");
            carb = builder.NewAtom("C");
            carb.FormalCharge = -1;
            molecule2.LonePairs.Add(new LonePair(carb));
            molecule2.Atoms.Add(carb);
            IAtom oxy = builder.NewAtom("O");
            oxy.FormalCharge = 1;
            molecule2.Atoms.Add(oxy);
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Triple);

            setOfProducts.Add(molecule1);
            setOfProducts.Add(molecule2);
            return setOfProducts;
        }
    }
}
