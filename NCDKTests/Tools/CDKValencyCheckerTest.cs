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
using NCDK.AtomTypes;
using NCDK.Silent;
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
    public class CDKValencyCheckerTest : CDKTestCase
    {
        [TestMethod()]
        public void TestInstance()
        {
            CDKValencyChecker checker = CDKValencyChecker.GetInstance(ChemObjectBuilder.Instance);
            Assert.IsNotNull(checker);
        }

        [TestMethod()]
        public void TestIsSaturated_IAtomContainer()
        {
            // test methane with explicit hydrogen
            IAtomContainer mol = new AtomContainer();
            CDKValencyChecker checker = CDKValencyChecker.GetInstance(mol.Builder);
            Atom c = new Atom("C");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            Atom h3 = new Atom("H");
            Atom h4 = new Atom("H");
            mol.Atoms.Add(c);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.Atoms.Add(h3);
            mol.Atoms.Add(h4);
            mol.Bonds.Add(new Bond(c, h1));
            mol.Bonds.Add(new Bond(c, h2));
            mol.Bonds.Add(new Bond(c, h3));
            mol.Bonds.Add(new Bond(c, h4));
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));

            // test methane with implicit hydrogen
            mol = new AtomContainer();
            c = new Atom("C") { ImplicitHydrogenCount = 4 };
            mol.Atoms.Add(c);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));
        }

        [TestMethod()]
        public void TestIsSaturatedPerAtom()
        {
            // test methane with explicit hydrogen
            IAtomContainer mol = new AtomContainer();
            CDKValencyChecker checker = CDKValencyChecker.GetInstance(mol.Builder);
            Atom c = new Atom("C");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            Atom h3 = new Atom("H");
            Atom h4 = new Atom("H");
            mol.Atoms.Add(c);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.Atoms.Add(h3);
            mol.Atoms.Add(h4);
            mol.Bonds.Add(new Bond(c, h1));
            mol.Bonds.Add(new Bond(c, h2));
            mol.Bonds.Add(new Bond(c, h3));
            mol.Bonds.Add(new Bond(c, h4));
            FindAndConfigureAtomTypesForAllAtoms(mol);
            Assert.IsTrue(checker.IsSaturated(mol));

            // test methane with implicit hydrogen
            mol = new AtomContainer();
            c = new Atom("C") { ImplicitHydrogenCount = 4 };
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
            IAtomContainer mol = new AtomContainer();
            CDKValencyChecker checker = CDKValencyChecker.GetInstance(mol.Builder);
            Atom c = new Atom("C");
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
            IAtomContainer mol = new AtomContainer();
            CDKValencyChecker checker = CDKValencyChecker.GetInstance(mol.Builder);
            Atom c = new Atom("C");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            Atom h3 = new Atom("H");
            Atom o = new Atom("O") { FormalCharge = -1 };
            mol.Atoms.Add(c);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.Atoms.Add(h3);
            mol.Atoms.Add(o);
            mol.Bonds.Add(new Bond(c, h1));
            mol.Bonds.Add(new Bond(c, h2));
            mol.Bonds.Add(new Bond(c, h3));
            mol.Bonds.Add(new Bond(c, o));
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
            IAtomContainer mol = new AtomContainer();
            CDKValencyChecker checker = CDKValencyChecker.GetInstance(mol.Builder);
            Atom n = new Atom("N");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            Atom h3 = new Atom("H");
            Atom h4 = new Atom("H");
            n.FormalCharge = +1;
            mol.Atoms.Add(n);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.Atoms.Add(h3);
            mol.Atoms.Add(h4);
            mol.Bonds.Add(new Bond(n, h1));
            mol.Bonds.Add(new Bond(n, h2));
            mol.Bonds.Add(new Bond(n, h3));
            mol.Bonds.Add(new Bond(n, h4));
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
            IAtomContainer mol = new AtomContainer();
            CDKValencyChecker checker = CDKValencyChecker.GetInstance(mol.Builder);
            Atom sulphur = new Atom("S");
            Atom o1 = new Atom("O");
            Atom o2 = new Atom("O");
            Atom o3 = new Atom("O");
            Atom o4 = new Atom("O");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            mol.Atoms.Add(sulphur);
            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);
            mol.Atoms.Add(o3);
            mol.Atoms.Add(o4);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);
            mol.Bonds.Add(new Bond(sulphur, o1, BondOrder.Double));
            mol.Bonds.Add(new Bond(sulphur, o2, BondOrder.Double));
            mol.Bonds.Add(new Bond(sulphur, o3, BondOrder.Single));
            mol.Bonds.Add(new Bond(sulphur, o4, BondOrder.Single));
            mol.Bonds.Add(new Bond(h1, o3, BondOrder.Single));
            mol.Bonds.Add(new Bond(h2, o4, BondOrder.Single));
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
            IAtomContainer mol = new AtomContainer();
            CDKValencyChecker checker = CDKValencyChecker.GetInstance(mol.Builder);
            Atom hydrogen = new Atom("H") { FormalCharge = +1 };
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
            IAtomContainer mol = new AtomContainer();
            Atom f1 = new Atom("F");
            Atom c2 = new Atom("C");
            Atom c3 = new Atom("C");
            f1.FormalCharge = 1;
            mol.Atoms.Add(f1);
            mol.Atoms.Add(c2);
            mol.Atoms.Add(c3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            CDKValencyChecker checker = CDKValencyChecker.GetInstance(mol.Builder);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            mol.Atoms[2].ImplicitHydrogenCount = 2; // third atom
            Assert.IsTrue(checker.IsSaturated(mol));
        }

        [TestMethod()]
        public void TestIsSaturated_MissingBondOrders_Ethane()
        {
            // test ethane with explicit hydrogen
            IAtomContainer mol = new AtomContainer();
            CDKValencyChecker checker = CDKValencyChecker.GetInstance(mol.Builder);
            Atom c1 = new Atom("C")
            {
                ImplicitHydrogenCount = 2,
                Hybridization = Hybridization.SP2
            };
            Atom c2 = new Atom("C")
            {
                Hybridization = Hybridization.SP2,
                ImplicitHydrogenCount = 2
            };
            mol.Atoms.Add(c1);
            mol.Atoms.Add(c2);
            IBond bond = new Bond(c1, c2, BondOrder.Single);
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
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(container.Builder);
            foreach (var atom in container.Atoms)
            {
                IAtomType type = matcher.FindMatchingAtomType(container, atom);
                if (type != null)
                    AtomTypeManipulator.Configure(atom, type);
            }
        }
    }
}
