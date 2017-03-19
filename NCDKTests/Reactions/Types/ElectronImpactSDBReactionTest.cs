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
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// TestSuite that runs a test for the ElectronImpactSDBReactionTest.
    ///
    // @cdk.module test-reaction
    /// </summary>
    [TestClass()]
    public class ElectronImpactSDBReactionTest : ReactionProcessTest
    {

        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        public ElectronImpactSDBReactionTest()
        {
            SetReaction(typeof(ElectronImpactSDBReaction));
        }

        /// <summary>
        ///  The JUnit setup method
        /// </summary>
        [TestMethod()]
        public void TestElectronImpactSDBReaction()
        {
            IReactionProcess type = new ElectronImpactSDBReaction();
            Assert.IsNotNull(type);
        }

        /// <summary>
        ///  A unit test for JUnit.
        ///
        ///  FIXME REAC: not recognized IAtomType =C*
        ///
        /// <returns>Description of the Return Value</returns>
        /// </summary>
        [TestMethod()]
        public override void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            /* Ionize(>C-C<): C=CCC -> C=C* + C+ , set the reactive center */

            var setOfReactants = GetExampleReactants();
            IAtomContainer reactant = setOfReactants[0];

            foreach (var bond in reactant.Bonds)
            {
                IAtom atom1 = bond.Atoms[0];
                IAtom atom2 = bond.Atoms[1];
                if (bond.Order == BondOrder.Single && atom1.Symbol.Equals("C") && atom2.Symbol.Equals("C"))
                {
                    bond.IsReactiveCenter = true;
                    atom1.IsReactiveCenter = true;
                    atom2.IsReactiveCenter = true;
                }
            }

            Assert.AreEqual(0, reactant.SingleElectrons.Count);

            /* initiate */
            IReactionProcess type = new ElectronImpactSDBReaction();
            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;
            var setOfReactions = type.Initiate(setOfReactants, null);

            Assert.AreEqual(2, setOfReactions.Count);
            Assert.AreEqual(2, setOfReactions[0].Products.Count);

            IAtomContainer molecule1 = setOfReactions[0].Products[0];//[H][C+]=C([H])[H]

            Assert.AreEqual(1, molecule1.Atoms[1].FormalCharge.Value);
            Assert.AreEqual(0, molecule1.SingleElectrons.Count);

            IAtomContainer molecule2 = setOfReactions[0].Products[1];//[H][C*]([H])[H]

            Assert.AreEqual(1, molecule2.SingleElectrons.Count);
            Assert.AreEqual(1, molecule2.GetConnectedSingleElectrons(molecule2.Atoms[0]).Count());

            Assert.IsTrue(setOfReactions[0].Mappings.Any());

            Assert.AreEqual(2, setOfReactions[1].Products.Count);

            molecule1 = setOfReactions[1].Products[0];//[H]C=[C*]([H])[H]
            Assert.AreEqual(1, molecule1.GetConnectedSingleElectrons(molecule1.Atoms[1]).Count());

            molecule2 = setOfReactions[1].Products[1];//[H][C+]([H])[H]

            Assert.AreEqual(0, molecule2.SingleElectrons.Count);
            Assert.AreEqual(1, molecule2.Atoms[0].FormalCharge.Value);

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
        /// Get the example set of molecules.
        ///
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        private IAtomContainerSet<IAtomContainer> GetExampleReactants()
        {
            var setOfReactants = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();

            IAtomContainer reactant = builder.CreateAtomContainer();//CreateFromSmiles("C=CC")
            reactant.Atoms.Add(builder.CreateAtom("C"));
            reactant.Atoms.Add(builder.CreateAtom("C"));
            reactant.Atoms.Add(builder.CreateAtom("C"));
            reactant.AddBond(reactant.Atoms[0], reactant.Atoms[1], BondOrder.Double);
            reactant.AddBond(reactant.Atoms[1], reactant.Atoms[2], BondOrder.Single);
            try
            {
                AddExplicitHydrogens(reactant);
                MakeSureAtomTypesAreRecognized(reactant);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            setOfReactants.Add(reactant);
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
