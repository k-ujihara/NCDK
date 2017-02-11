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
using NCDK.Default;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.Charges
{
    /**
    * TestSuite that runs all tests.
    *
    * @cdk.module test-charges
*/
    [TestClass()]
    public class StabilizationChargesTest : CDKTestCase
    {
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        private LonePairElectronChecker lpcheck = new LonePairElectronChecker();

        /**
         * Constructor of the StabilizationChargesTest.
         */
        public StabilizationChargesTest()
                : base()
        { }

        /**
         * A unit test suite for JUnit.
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestStabilizationCharges()
        {

            Assert.IsNotNull(new StabilizationCharges());
        }

        /**
         * A unit test suite for JUnit.
         *
         *  @cdk.inchi InChI=1/C4H8/c1-3-4-2/h3H,1,4H2,2H3
         *
         * @return    The test suite
         * @throws Exception
         */
        [TestMethod()]
        public void TestCalculatePositive_IAtomContainer_IAtom()
        {

            StabilizationCharges sc = new StabilizationCharges();

            IAtomContainer molecule = builder.CreateAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms[1].FormalCharge = +1;
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Double);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            lpcheck.Saturate(molecule);

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                if (i == 1)
                    Assert.AreNotSame(0.0, sc.CalculatePositive(molecule, molecule.Atoms[i]));
                else
                    Assert.AreEqual(0.0, sc.CalculatePositive(molecule, molecule.Atoms[i]), 0.001);

            }
        }
    }
}
