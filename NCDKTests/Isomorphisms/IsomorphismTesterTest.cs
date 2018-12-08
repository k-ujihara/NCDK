/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.Isomorphisms
{
    /// <summary>
    /// Checks the functionality of the IsomorphismTester
    /// </summary>
    // @cdk.module test-standard
    [TestClass()]
    public class IsomorphismTesterTest : CDKTestCase
    {
        IAtomContainer pinene_1 = null, pinene_2 = null, pinene_non = null;

        public IsomorphismTesterTest()
                : base()
        { }

        [TestInitialize()]
        public void SetUp()
        {
            pinene_1 = new AtomContainer();
            pinene_1.Atoms.Add(new Atom("C")); // 1
            pinene_1.Atoms.Add(new Atom("C")); // 2
            pinene_1.Atoms.Add(new Atom("C")); // 3
            pinene_1.Atoms.Add(new Atom("C")); // 4
            pinene_1.Atoms.Add(new Atom("C")); // 5
            pinene_1.Atoms.Add(new Atom("C")); // 6
            pinene_1.Atoms.Add(new Atom("C")); // 7
            pinene_1.Atoms.Add(new Atom("C")); // 8
            pinene_1.Atoms.Add(new Atom("C")); // 9
            pinene_1.Atoms.Add(new Atom("C")); // 10

            pinene_1.AddBond(pinene_1.Atoms[0], pinene_1.Atoms[1], BondOrder.Double); // 1
            pinene_1.AddBond(pinene_1.Atoms[1], pinene_1.Atoms[2], BondOrder.Single); // 2
            pinene_1.AddBond(pinene_1.Atoms[2], pinene_1.Atoms[3], BondOrder.Single); // 3
            pinene_1.AddBond(pinene_1.Atoms[3], pinene_1.Atoms[4], BondOrder.Single); // 4
            pinene_1.AddBond(pinene_1.Atoms[4], pinene_1.Atoms[5], BondOrder.Single); // 5
            pinene_1.AddBond(pinene_1.Atoms[5], pinene_1.Atoms[0], BondOrder.Single); // 6
            pinene_1.AddBond(pinene_1.Atoms[0], pinene_1.Atoms[6], BondOrder.Single); // 7
            pinene_1.AddBond(pinene_1.Atoms[3], pinene_1.Atoms[7], BondOrder.Single); // 8
            pinene_1.AddBond(pinene_1.Atoms[5], pinene_1.Atoms[7], BondOrder.Single); // 9
            pinene_1.AddBond(pinene_1.Atoms[7], pinene_1.Atoms[8], BondOrder.Single); // 10
            pinene_1.AddBond(pinene_1.Atoms[7], pinene_1.Atoms[9], BondOrder.Single); // 11

            pinene_2 = new AtomContainer();
            pinene_2.Atoms.Add(new Atom("C")); // 1
            pinene_2.Atoms.Add(new Atom("C")); // 2
            pinene_2.Atoms.Add(new Atom("C")); // 3
            pinene_2.Atoms.Add(new Atom("C")); // 4
            pinene_2.Atoms.Add(new Atom("C")); // 5
            pinene_2.Atoms.Add(new Atom("C")); // 6
            pinene_2.Atoms.Add(new Atom("C")); // 7
            pinene_2.Atoms.Add(new Atom("C")); // 8
            pinene_2.Atoms.Add(new Atom("C")); // 9
            pinene_2.Atoms.Add(new Atom("C")); // 10

            pinene_2.AddBond(pinene_2.Atoms[0], pinene_2.Atoms[4], BondOrder.Single); // 1
            pinene_2.AddBond(pinene_2.Atoms[0], pinene_2.Atoms[5], BondOrder.Single); // 2
            pinene_2.AddBond(pinene_2.Atoms[0], pinene_2.Atoms[8], BondOrder.Single); // 3
            pinene_2.AddBond(pinene_2.Atoms[1], pinene_2.Atoms[2], BondOrder.Single); // 4
            pinene_2.AddBond(pinene_2.Atoms[1], pinene_2.Atoms[9], BondOrder.Single); // 5
            pinene_2.AddBond(pinene_2.Atoms[2], pinene_2.Atoms[3], BondOrder.Single); // 6
            pinene_2.AddBond(pinene_2.Atoms[2], pinene_2.Atoms[0], BondOrder.Single); // 7
            pinene_2.AddBond(pinene_2.Atoms[3], pinene_2.Atoms[8], BondOrder.Single); // 8
            pinene_2.AddBond(pinene_2.Atoms[8], pinene_2.Atoms[7], BondOrder.Single); // 9
            pinene_2.AddBond(pinene_2.Atoms[7], pinene_2.Atoms[9], BondOrder.Double); // 10
            pinene_2.AddBond(pinene_2.Atoms[7], pinene_2.Atoms[6], BondOrder.Single); // 11

            pinene_non = new AtomContainer();
            pinene_non.Atoms.Add(new Atom("C")); // 1
            pinene_non.Atoms.Add(new Atom("C")); // 2
            pinene_non.Atoms.Add(new Atom("C")); // 3
            pinene_non.Atoms.Add(new Atom("C")); // 4
            pinene_non.Atoms.Add(new Atom("C")); // 5
            pinene_non.Atoms.Add(new Atom("C")); // 6
            pinene_non.Atoms.Add(new Atom("C")); // 7
            pinene_non.Atoms.Add(new Atom("C")); // 8
            pinene_non.Atoms.Add(new Atom("C")); // 9
            pinene_non.Atoms.Add(new Atom("C")); // 10

            pinene_non.AddBond(pinene_non.Atoms[0], pinene_non.Atoms[5], BondOrder.Single); // 1
            pinene_non.AddBond(pinene_non.Atoms[0], pinene_non.Atoms[7], BondOrder.Single); // 2
            pinene_non.AddBond(pinene_non.Atoms[0], pinene_non.Atoms[8], BondOrder.Single); // 3
            pinene_non.AddBond(pinene_non.Atoms[1], pinene_non.Atoms[9], BondOrder.Single); // 4
            pinene_non.AddBond(pinene_non.Atoms[1], pinene_non.Atoms[4], BondOrder.Single); // 5
            pinene_non.AddBond(pinene_non.Atoms[2], pinene_non.Atoms[3], BondOrder.Single); // 6
            pinene_non.AddBond(pinene_non.Atoms[2], pinene_non.Atoms[4], BondOrder.Single); // 7
            pinene_non.AddBond(pinene_non.Atoms[2], pinene_non.Atoms[6], BondOrder.Single); // 8
            pinene_non.AddBond(pinene_non.Atoms[2], pinene_non.Atoms[7], BondOrder.Single); // 9
            pinene_non.AddBond(pinene_non.Atoms[4], pinene_non.Atoms[5], BondOrder.Double); // 10
            pinene_non.AddBond(pinene_non.Atoms[7], pinene_non.Atoms[9], BondOrder.Single); // 11
        }

        [TestMethod()]
        public void TestIsomorphismTester_IAtomContainer()
        {
            IsomorphismTester it = new IsomorphismTester(pinene_1);
            Assert.IsNotNull(it);
        }

        [TestMethod()]
        public void TestIsomorphismTester()
        {
            IsomorphismTester it = new IsomorphismTester();
            Assert.IsNotNull(it);
        }

        [TestMethod()]
        public void TestIsIsomorphic_IAtomContainer()
        {
            IsomorphismTester it = new IsomorphismTester(pinene_1);
            Assert.IsTrue(it.IsIsomorphic(pinene_2));
            Assert.IsFalse(it.IsIsomorphic(pinene_non));
        }

        [TestMethod()]
        public void TestIsIsomorphic_IAtomContainer_IAtomContainer()
        {
            IsomorphismTester it = new IsomorphismTester();
            Assert.IsTrue(it.IsIsomorphic(pinene_2, pinene_1));
            Assert.IsFalse(it.IsIsomorphic(pinene_2, pinene_non));
        }

        [TestMethod()]
        public void TestBiphenyl()
        {
            //get the biphenyl as aromatic smiles
            var parser = CDK.SmilesParser;
            var biphenyl_aromaticsmiles = parser.ParseSmiles("c1ccccc1-c2ccccc2");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(biphenyl_aromaticsmiles);
            var hAdder = CDK.HydrogenAdder;
            hAdder.AddImplicitHydrogens(biphenyl_aromaticsmiles);
            Aromaticity.CDKLegacy.Apply(biphenyl_aromaticsmiles);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(biphenyl_aromaticsmiles);

            //get the biphenyl as Kekule smiles
            var biphenyl_kekulesmiles = parser.ParseSmiles("C1=C(C=CC=C1)C2=CC=CC=C2");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(biphenyl_kekulesmiles);
            hAdder = CDK.HydrogenAdder;
            hAdder.AddImplicitHydrogens(biphenyl_kekulesmiles);
            Aromaticity.CDKLegacy.Apply(biphenyl_kekulesmiles);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(biphenyl_kekulesmiles);

            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(biphenyl_aromaticsmiles, biphenyl_kekulesmiles));
        }
    }
}
