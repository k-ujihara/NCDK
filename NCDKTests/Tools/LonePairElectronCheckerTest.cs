/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
 *                     2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Default;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.Tools
{
    /// <summary>
    /// Tests CDK's Lone Pair Electron checking capabilities in terms of
    /// example molecules.
    /// </summary>
    // @cdk.module     test-standard
    // @author         Miguel Rojas
    // @cdk.created    2006-04-01
    [TestClass()]
    public class LonePairElectronCheckerTest : CDKTestCase
    {
        private static LonePairElectronChecker lpcheck = new LonePairElectronChecker();

        [TestMethod()]
        public void TestAllSaturated_Formaldehyde()
        {
            // test Formaldehyde, CH2=O with explicit hydrogen
            IAtomContainer m = new AtomContainer();
            Atom c = new Atom("C");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            Atom O = new Atom("O");
            m.Atoms.Add(c);
            m.Atoms.Add(h1);
            m.Atoms.Add(h2);
            m.Atoms.Add(O);
            for (int i = 0; i < 2; i++)
            {
                LonePair lp = new LonePair(O);
                m.LonePairs.Add(lp);
            }
            m.Bonds.Add(new Bond(c, h1));
            m.Bonds.Add(new Bond(c, h2));
            m.Bonds.Add(new Bond(c, O, BondOrder.Double));
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);

            Assert.IsTrue(lpcheck.AllSaturated(m));
        }

        [TestMethod()]
        public void TestAllSaturated_Methanethiol()
        {
            // test Methanethiol, CH4S
            Atom c = new Atom("C") { ImplicitHydrogenCount = 3 };
            Atom s = new Atom("S") { ImplicitHydrogenCount = 1 };

            Bond b1 = new Bond(c, s, BondOrder.Single);

            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(c);
            m.Atoms.Add(s);
            m.Bonds.Add(b1);
            for (int i = 0; i < 1; i++)
            {
                LonePair lp = new LonePair(s);
                m.LonePairs.Add(lp);
            }
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);

            Assert.IsFalse(lpcheck.AllSaturated(m));
        }

        [TestMethod()]
        public void TestNewSaturate_Methyl_chloride()
        {
            // test Methyl chloride, CH3Cl
            Atom c1 = new Atom("C") { ImplicitHydrogenCount = 3 };
            Atom cl = new Atom("Cl");
            Bond b1 = new Bond(c1, cl, BondOrder.Single);

            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(c1);
            m.Atoms.Add(cl);
            m.Bonds.Add(b1);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);
            lpcheck.Saturate(m);
            Assert.AreEqual(3, m.GetConnectedLonePairs(cl).Count());
            Assert.AreEqual(0, m.GetConnectedLonePairs(c1).Count());
        }

        [TestMethod()]
        public void TestNewSaturate_Methyl_alcohol()
        {
            // test Methyl chloride, CH3OH
            Atom c1 = new Atom("C") { ImplicitHydrogenCount = 3 };
            Atom o = new Atom("O") { ImplicitHydrogenCount = 1 };
            Bond b1 = new Bond(c1, o, BondOrder.Single);

            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(c1);
            m.Atoms.Add(o);
            m.Bonds.Add(b1);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);
            lpcheck.Saturate(m);
            Assert.AreEqual(2, m.GetConnectedLonePairs(o).Count());
            Assert.AreEqual(0, m.GetConnectedLonePairs(c1).Count());
        }

        [TestMethod()]
        public void TestNewSaturate_Methyl_alcohol_AddH()
        {
            // test Methyl alcohol, CH3OH
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("O"));
            for (int i = 0; i < 4; i++)
                m.Atoms.Add(new Atom("H"));

            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[5], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);
            lpcheck.Saturate(m);

            Assert.AreEqual(2, m.GetConnectedLonePairs(m.Atoms[1]).Count());
            Assert.AreEqual(0, m.GetConnectedLonePairs(m.Atoms[0]).Count());
        }

        [TestMethod()]
        public void TestNewSaturate_Methyl_alcohol_protonated()
        {
            // test Methyl alcohol protonated, CH3OH2+
            Atom c1 = new Atom("C") { ImplicitHydrogenCount = 3 };
            Atom o = new Atom("O")
            {
                FormalCharge = +1,
                ImplicitHydrogenCount = 2
            };
            Bond b1 = new Bond(c1, o, BondOrder.Single);

            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(c1);
            m.Atoms.Add(o);
            m.Bonds.Add(b1);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);
            lpcheck.Saturate(m);

            Assert.AreEqual(1, m.GetConnectedLonePairs(o).Count());
        }

        [TestMethod()]
        public void TestNewSaturate_methoxide_anion()
        {
            // test methoxide anion, CH3O-
            Atom c1 = new Atom("C") { ImplicitHydrogenCount = 3 };
            Atom o = new Atom("O") { FormalCharge = -1 };
            Bond b1 = new Bond(c1, o, BondOrder.Single);

            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(c1);
            m.Atoms.Add(o);
            m.Bonds.Add(b1);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);
            lpcheck.Saturate(m);

            Assert.AreEqual(3, m.GetConnectedLonePairs(o).Count());
        }

        [TestMethod()]
        public void TestNewSaturate_Ammonia()
        {
            // test Ammonia, H3N
            Atom n = new Atom("N") { ImplicitHydrogenCount = 3 };

            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(n);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);
            lpcheck.Saturate(m);

            Assert.AreEqual(1, m.GetConnectedLonePairs(n).Count());
        }

        [TestMethod()]
        public void TestNewSaturate_methylamine_radical_cation()
        {
            // test Ammonia, CH3NH3+
            Atom c = new Atom("C") { ImplicitHydrogenCount = 3 };
            Atom n = new Atom("N")
            {
                ImplicitHydrogenCount = 3,
                FormalCharge = +1
            };
            Bond b1 = new Bond(c, n, BondOrder.Single);

            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(c);
            m.Atoms.Add(n);
            m.Bonds.Add(b1);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);
            lpcheck.Saturate(m);

            Assert.AreEqual(0, m.GetConnectedLonePairs(n).Count());
        }

        /// <summary>
        /// A unit test O=C([H])[C+]([H])[C-]([H])[H]
        /// </summary>
        [TestMethod()]
        public void TestNewSaturate_withHAdded()
        {
            // O=C([H])[C+]([H])[C-]([H])[H]
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("O=C([H])[C+]([H])[C-]([H])[H]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            lpcheck.Saturate(mol);

            Assert.AreEqual(2, mol.GetConnectedLonePairs(mol.Atoms[0]).Count());
            Assert.AreEqual(0, mol.GetConnectedLonePairs(mol.Atoms[3]).Count());
            Assert.AreEqual(1, mol.GetConnectedLonePairs(mol.Atoms[5]).Count());
        }
    }
}
