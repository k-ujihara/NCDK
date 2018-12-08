/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Manipulator;

namespace NCDK.Tools
{
    /// <summary>
    /// Tests CDK's valency checker capabilities in terms of example molecules.
    /// </summary>
    // @cdk.module  test-valencycheck
    // @author      Egon Willighagen <egonw@users.sf.net>
    // @cdk.created 2007-07-28
    [TestClass()]
    public class CDKValencyCheckerTest
        : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestInstance()
        {
            CDKValencyChecker checker = CDKValencyChecker.Instance;
            Assert.IsNotNull(checker);
        }

        [TestMethod()]
        public void TestIsSaturated_IAtomContainer()
        {
            // test methane with explicit hydrogen
            var mol = builder.NewAtomContainer();
            var checker = CDKValencyChecker.Instance;
            var c = builder.NewAtom("C");
            var h1 = builder.NewAtom("H");
            var h2 = builder.NewAtom("H");
            var h3 = builder.NewAtom("H");
            var h4 = builder.NewAtom("H");
            mol.Atoms.Add(c);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.Atoms.Add(h3);
            mol.Atoms.Add(h4);
            mol.Bonds.Add(builder.NewBond(c, h1));
            mol.Bonds.Add(builder.NewBond(c, h2));
            mol.Bonds.Add(builder.NewBond(c, h3));
            mol.Bonds.Add(builder.NewBond(c, h4));
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));

            // test methane with implicit hydrogen
            mol = builder.NewAtomContainer();
            c = builder.NewAtom("C");
            c.ImplicitHydrogenCount = 4;
            mol.Atoms.Add(c);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));
        }

        [TestMethod()]
        public void TestIsSaturatedPerAtom()
        {
            // test methane with explicit hydrogen
            var mol = builder.NewAtomContainer();
            var checker = CDKValencyChecker.Instance;
            var c = builder.NewAtom("C");
            var h1 = builder.NewAtom("H");
            var h2 = builder.NewAtom("H");
            var h3 = builder.NewAtom("H");
            var h4 = builder.NewAtom("H");
            mol.Atoms.Add(c);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.Atoms.Add(h3);
            mol.Atoms.Add(h4);
            mol.Bonds.Add(builder.NewBond(c, h1));
            mol.Bonds.Add(builder.NewBond(c, h2));
            mol.Bonds.Add(builder.NewBond(c, h3));
            mol.Bonds.Add(builder.NewBond(c, h4));
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));

            // test methane with implicit hydrogen
            mol = builder.NewAtomContainer();
            c = builder.NewAtom("C");
            c.ImplicitHydrogenCount = 4;
            mol.Atoms.Add(c);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            foreach (var atom in mol.Atoms)
            {
                Assert.IsTrue(checker.IsSaturated(atom, mol));
            }
        }

        [TestMethod()]
        public void TestIsSaturated_MissingHydrogens_Methane()
        {
            // test methane with explicit hydrogen
            var mol = builder.NewAtomContainer();
            var checker = CDKValencyChecker.Instance;
            var c = builder.NewAtom("C");
            mol.Atoms.Add(c);
            c.ImplicitHydrogenCount = 3;
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsFalse(checker.IsSaturated(mol));
        }

        /// <summary>
        /// Tests if the saturation checker considers negative charges.
        /// </summary>
        [TestMethod()]
        public void TestIsSaturated_NegativelyChargedOxygen()
        {
            // test methane with explicit hydrogen
            var mol = builder.NewAtomContainer();
            var checker = CDKValencyChecker.Instance;
            var c = builder.NewAtom("C");
            var h1 = builder.NewAtom("H");
            var h2 = builder.NewAtom("H");
            var h3 = builder.NewAtom("H");
            var o = builder.NewAtom("O");
            o.FormalCharge = -1;
            mol.Atoms.Add(c);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.Atoms.Add(h3);
            mol.Atoms.Add(o);
            mol.Bonds.Add(builder.NewBond(c, h1));
            mol.Bonds.Add(builder.NewBond(c, h2));
            mol.Bonds.Add(builder.NewBond(c, h3));
            mol.Bonds.Add(builder.NewBond(c, o));
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));
        }

        /// <summary>
        /// Tests if the saturation checker considers positive
        /// charges.
        /// </summary>
        [TestMethod()]
        public void TestIsSaturated_PositivelyChargedNitrogen()
        {
            // test methane with explicit hydrogen
            var mol = builder.NewAtomContainer();
            var checker = CDKValencyChecker.Instance;
            var n = builder.NewAtom("N");
            var h1 = builder.NewAtom("H");
            var h2 = builder.NewAtom("H");
            var h3 = builder.NewAtom("H");
            var h4 = builder.NewAtom("H");
            n.FormalCharge = +1;
            mol.Atoms.Add(n);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.Atoms.Add(h3);
            mol.Atoms.Add(h4);
            mol.Bonds.Add(builder.NewBond(n, h1));
            mol.Bonds.Add(builder.NewBond(n, h2));
            mol.Bonds.Add(builder.NewBond(n, h3));
            mol.Bonds.Add(builder.NewBond(n, h4));
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));
        }

        /// <summary>
        /// Test sulfuric acid.
        /// </summary>
        [TestMethod()]
        public void TestBug772316()
        {
            // test methane with explicit hydrogen
            var mol = builder.NewAtomContainer();
            var checker = CDKValencyChecker.Instance;
            var sulphur = builder.NewAtom("S");
            var o1 = builder.NewAtom("O");
            var o2 = builder.NewAtom("O");
            var o3 = builder.NewAtom("O");
            var o4 = builder.NewAtom("O");
            var h1 = builder.NewAtom("H");
            var h2 = builder.NewAtom("H");
            mol.Atoms.Add(sulphur);
            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);
            mol.Atoms.Add(o3);
            mol.Atoms.Add(o4);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.Bonds.Add(builder.NewBond(sulphur, o1, BondOrder.Double));
            mol.Bonds.Add(builder.NewBond(sulphur, o2, BondOrder.Double));
            mol.Bonds.Add(builder.NewBond(sulphur, o3, BondOrder.Single));
            mol.Bonds.Add(builder.NewBond(sulphur, o4, BondOrder.Single));
            mol.Bonds.Add(builder.NewBond(h1, o3, BondOrder.Single));
            mol.Bonds.Add(builder.NewBond(h2, o4, BondOrder.Single));
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));
        }

        /// <summary>
        /// Tests if the saturation checker gets a proton right.
        /// </summary>
        [TestMethod()]
        public void TestIsSaturated_Proton()
        {
            // test H+
            var mol = builder.NewAtomContainer();
            var checker = CDKValencyChecker.Instance;
            var hydrogen = builder.NewAtom("H");
            hydrogen.FormalCharge = +1;
            mol.Atoms.Add(hydrogen);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));
        }

        /// <summary> TODO: check who added this test. I think Miguel; it seems to be a
        ///  resonance structure.
        /// </summary>
        [TestMethod()]
        public void Test1()
        {
            var mol = builder.NewAtomContainer();
            var f1 = builder.NewAtom("F");
            var c2 = builder.NewAtom("C");
            var c3 = builder.NewAtom("C");
            f1.FormalCharge = 1;
            mol.Atoms.Add(f1);
            mol.Atoms.Add(c2);
            mol.Atoms.Add(c3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            var checker = CDKValencyChecker.Instance;
            FindAndConfigureAtomTypesForAllAtoms(mol);
            mol.Atoms[2].ImplicitHydrogenCount = 2; // third atom
            Assert.IsTrue(checker.IsSaturated(mol));
        }

        [TestMethod()]
        public void TestIsSaturated_MissingBondOrders_Ethane()
        {
            // test ethane with explicit hydrogen
            var mol = builder.NewAtomContainer();
            var checker = CDKValencyChecker.Instance;
            var c1 = builder.NewAtom("C");
            c1.ImplicitHydrogenCount = 2;
            c1.Hybridization = Hybridization.SP2;
            var c2 = builder.NewAtom("C");
            c2.ImplicitHydrogenCount = 2;
            c2.Hybridization = Hybridization.SP2;
            mol.Atoms.Add(c1);
            mol.Atoms.Add(c2);
            var bond = builder.NewBond(c1, c2, BondOrder.Single);
            mol.Bonds.Add(bond);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsFalse(checker.IsSaturated(mol));

            // sanity check
            bond.Order = BondOrder.Double;
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));
        }

        private void FindAndConfigureAtomTypesForAllAtoms(IAtomContainer container)
        {
            var matcher = CDK.AtomTypeMatcher;
            foreach (var atom in container.Atoms)
            {
                var type = matcher.FindMatchingAtomType(container, atom);
                if (type != null)
                    AtomTypeManipulator.Configure(atom, type);
            }
        }
    }
}
