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
using NCDK.Reactions.Types.Parameters;
using NCDK.Silent;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the ElectronImpactPDBReactionTest.
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class ElectronImpactPDBReactionTest : ReactionProcessTest
    {
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        public ElectronImpactPDBReactionTest()
        {
            SetReaction(typeof(ElectronImpactPDBReaction));
        }

        [TestMethod()]
        public void TestElectronImpactPDBReaction()
        {
            IReactionProcess type = new ElectronImpactPDBReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        ///  A unit test for JUnit with the compound 2_5_Hexen_3_one.
        /// </summary>
        // @cdk.inchi InChI=1/C6H10O/c1-3-5-6(7)4-2/h3H,1,4-5H2,2H3
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            /* ionize >C=C< , set the reactive center */
            IAtomContainer reactant = builder.NewAtomContainer();//CreateFromSmiles("C=CCC(=O)CC")
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("O"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.AddBond(reactant.Atoms[0], reactant.Atoms[1], BondOrder.Double);
            reactant.AddBond(reactant.Atoms[1], reactant.Atoms[2], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[2], reactant.Atoms[3], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[3], reactant.Atoms[4], BondOrder.Double);
            reactant.AddBond(reactant.Atoms[3], reactant.Atoms[5], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[5], reactant.Atoms[6], BondOrder.Single);
            AddExplicitHydrogens(reactant);

            foreach (var bond in reactant.Bonds)
            {
                IAtom atom1 = bond.Atoms[0];
                IAtom atom2 = bond.Atoms[1];
                if (bond.Order == BondOrder.Double && atom1.Symbol.Equals("C") && atom2.Symbol.Equals("C"))
                {
                    bond.IsReactiveCenter = true;
                    atom1.IsReactiveCenter = true;
                    atom2.IsReactiveCenter = true;
                }
            }

            var setOfReactants = ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(reactant);

            /* initiate */
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactant);
            MakeSureAtomTypesAreRecognized(reactant);

            IReactionProcess type = new ElectronImpactPDBReaction();
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);

            IAtomContainer molecule = setOfReactions[0].Products[0];

            Assert.AreEqual(1, molecule.Atoms[0].FormalCharge.Value);
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[1]).Count());
            Assert.AreEqual(1, molecule.SingleElectrons.Count);

            molecule = setOfReactions[1].Products[0];
            Assert.AreEqual(1, molecule.Atoms[1].FormalCharge.Value);
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[0]).Count());
            Assert.AreEqual(1, molecule.SingleElectrons.Count);

            Assert.AreEqual(17, setOfReactions[0].Mappings.Count);

        }

        /// <summary>
        ///  A unit test for JUnit with the compound propene.
        /// </summary>
        // @cdk.inchi InChI=1/C3H6/c1-3-2/h3H,1H2,2H3
        [TestMethod()]
        public void TestAutomatic_Set_Active_Bond()
        {
            /* ionize all possible double bonds */
            IAtomContainer reactant = builder.NewAtomContainer();//Miles("C=CC")
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.AddBond(reactant.Atoms[0], reactant.Atoms[1], BondOrder.Double);
            reactant.AddBond(reactant.Atoms[1], reactant.Atoms[2], BondOrder.Single);
            AddExplicitHydrogens(reactant);

            var setOfReactants = ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(reactant);

            /* initiate */
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactant);
            MakeSureAtomTypesAreRecognized(reactant);

            IReactionProcess type = new ElectronImpactPDBReaction();
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);

            IAtomContainer molecule = setOfReactions[0].Products[0];
            Assert.AreEqual(1, molecule.Atoms[0].FormalCharge.Value);
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[1]).Count());

            molecule = setOfReactions[1].Products[0];
            Assert.AreEqual(1, molecule.Atoms[1].FormalCharge.Value);
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[0]).Count());

        }

        /// <summary>
        ///  A unit test for JUnit with the compound 2_5_Hexen_3_one.
        /// </summary>
        // @cdk.inchi InChI=1/C6H10O/c1-3-5-6(7)4-2/h3H,1,4-5H2,2H3
        [TestMethod()]
        public void TestAutomatic_Set_Active_Bond2()
        {
            /* ionize >C=C< , set the reactive center */
            IAtomContainer reactant = builder.NewAtomContainer();//CreateFromSmiles("C=CCC(=O)CC")
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("O"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.Atoms.Add(builder.NewAtom("C"));
            reactant.AddBond(reactant.Atoms[0], reactant.Atoms[1], BondOrder.Double);
            reactant.AddBond(reactant.Atoms[1], reactant.Atoms[2], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[2], reactant.Atoms[3], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[3], reactant.Atoms[4], BondOrder.Double);
            reactant.AddBond(reactant.Atoms[3], reactant.Atoms[5], BondOrder.Single);
            reactant.AddBond(reactant.Atoms[5], reactant.Atoms[6], BondOrder.Single);
            AddExplicitHydrogens(reactant);

            var setOfReactants = ChemObjectBuilder.Instance.NewAtomContainerSet();
            setOfReactants.Add(reactant);

            /* initiate */
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactant);
            MakeSureAtomTypesAreRecognized(reactant);

            IReactionProcess type = new ElectronImpactPDBReaction();
            List<IParameterReaction> paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter();
            param.IsSetParameter = false;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(3, setOfReactions.Count);

            IAtomContainer molecule = setOfReactions[0].Products[0];
            Assert.AreEqual(1, molecule.Atoms[0].FormalCharge.Value);
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[1]).Count());

            molecule = setOfReactions[1].Products[0];
            Assert.AreEqual(1, molecule.Atoms[1].FormalCharge.Value);
            Assert.AreEqual(1, molecule.GetConnectedSingleElectrons(molecule.Atoms[0]).Count());

            Assert.AreEqual(17, setOfReactions[0].Mappings.Count);

        }

        /// <summary>
        /// A unit test suite for JUnit. Reaction:propene
        /// Manually put of the reactive center.
        /// </summary>
        // @cdk.inchi InChI=1/C3H6/c1-3-2/h3H,1H2,2H3
        [TestMethod()]
        public void TestCDKConstants_REACTIVE_CENTER()
        {
            IReactionProcess type = new ElectronImpactPDBReaction();
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
            MakeSureAtomTypesAreRecognized(molecule);

            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);
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
        /// A unit test suite for JUnit. Reaction: propene
        /// Manually put of the reactive center.
        /// </summary>
        // @cdk.inchi InChI=1/C3H6/c1-3-2/h3H,1H2,2H3
        [TestMethod()]
        public void TestMapping()
        {
            IReactionProcess type = new ElectronImpactPDBReaction();
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

            Assert.AreEqual(9, setOfReactions[0].Mappings.Count);
            IAtom mappedProductA1 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[0]);
            Assert.AreEqual(mappedProductA1, product.Atoms[0]);
            IAtom mappedProductA2 = (IAtom)ReactionManipulator.GetMappedChemObject(setOfReactions[0],
                    molecule.Atoms[1]);
            Assert.AreEqual(mappedProductA2, product.Atoms[1]);
        }

        /// <summary>
        /// Test to recognize if a IAtomContainer matcher correctly the CDKAtomTypes.
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

        /// <summary>
        /// Get the example set of molecules.
        /// </summary>
        private IChemObjectSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = ChemObjectBuilder.Instance.NewAtomContainerSet();

            IAtomContainer molecule = builder.NewAtomContainer();//Miles("C=CC")
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

            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                LonePairElectronChecker.Saturate(molecule);
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
        /// </summary>
        private IChemObjectSet<IAtomContainer> GetExpectedProducts()
        {
            var setOfProducts = builder.NewAtomContainerSet();

            setOfProducts.Add(null);
            return setOfProducts;
        }
    }
}
