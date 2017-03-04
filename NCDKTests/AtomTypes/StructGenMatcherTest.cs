/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using System.Collections.Generic;
using NCDK.Default;
using System.Linq;

namespace NCDK.AtomTypes
{
    /// <summary>
    /// This class tests the matching of atom types defined in the
    /// structgen atom type list.
    ///
    // @cdk.module test-structgen
    /// </summary>
    //@FixMethodOrder(MethodSorters.NAME_ASCENDING)
    [TestClass()]
    public class StructGenMatcherTest : AbstractAtomTypeTest
    {
        private const string ATOMTYPE_LIST = "structgen_atomtypes.owl";

        private readonly static AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Config.Data."
                                                                   + ATOMTYPE_LIST, Silent.ChemObjectBuilder.Instance);

        public override string AtomTypeListName => ATOMTYPE_LIST;
        public override AtomTypeFactory GetFactory() => factory;
        public override IAtomTypeMatcher GetAtomTypeMatcher(IChemObjectBuilder builder)
        {
            return new StructGenMatcher();
        }

        private static IDictionary<string, int> testedAtomTypes = new Dictionary<string, int>();

        [TestMethod()]
        public void TestStructGenMatcher()
        {
            StructGenMatcher matcher = new StructGenMatcher();
            Assert.IsNotNull(matcher);
        }

        [TestMethod()]
        public void TestFindMatchingAtomType_IAtomContainer_IAtom()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom atom = Default.ChemObjectBuilder.Instance.CreateAtom("C");
            atom.ImplicitHydrogenCount = 4;
            mol.Atoms.Add(atom);

            StructGenMatcher atm = new StructGenMatcher();
            IAtomType matched = atm.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(matched);

            Assert.AreEqual("C", matched.Symbol);
        }

        [TestMethod()]
        public void TestN3()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("N");
            atom.ImplicitHydrogenCount = 3;
            mol.Atoms.Add(atom);

            StructGenMatcher atm = new StructGenMatcher();
            IAtomType matched = atm.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(matched);

            Assert.AreEqual("N", matched.Symbol);
        }

        [TestMethod()]
        public void TestFlourine()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom atom1 = Default.ChemObjectBuilder.Instance.CreateAtom("C");
            atom1.ImplicitHydrogenCount = 0;
            mol.Atoms.Add(atom1);
            for (int i = 0; i < 4; i++)
            {
                IAtom floruineAtom = Default.ChemObjectBuilder.Instance.CreateAtom("F");
                mol.Atoms.Add(floruineAtom);
                IBond bond = Default.ChemObjectBuilder.Instance.CreateBond(floruineAtom, atom1);
                mol.Bonds.Add(bond);
            }

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "C4", matched);

            for (int i = 1; i < mol.Atoms.Count; i++)
            {
                IAtom atom = mol.Atoms[i];
                matched = matcher.FindMatchingAtomType(mol, atom);
                AssertAtomType(testedAtomTypes, "atom " + i + " failed to match", "F1", matched);
            }
        }

        [TestMethod()]
        public void TestChlorine()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom atom1 = Default.ChemObjectBuilder.Instance.CreateAtom("C");
            atom1.ImplicitHydrogenCount = 0;
            mol.Atoms.Add(atom1);
            for (int i = 0; i < 4; i++)
            {
                IAtom floruineAtom = Default.ChemObjectBuilder.Instance.CreateAtom("Cl");
                mol.Atoms.Add(floruineAtom);
                IBond bond = Default.ChemObjectBuilder.Instance.CreateBond(floruineAtom, atom1);
                mol.Bonds.Add(bond);
            }

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "C4", matched);

            for (int i = 1; i < mol.Atoms.Count; i++)
            {
                IAtom atom = mol.Atoms[i];
                matched = matcher.FindMatchingAtomType(mol, atom);
                AssertAtomType(testedAtomTypes, "atom " + i + " failed to match", "Cl1", matched);
            }
        }

        [TestMethod()]
        public void TestBromine()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom atom1 = Default.ChemObjectBuilder.Instance.CreateAtom("C");
            atom1.ImplicitHydrogenCount = 0;
            mol.Atoms.Add(atom1);
            for (int i = 0; i < 4; i++)
            {
                IAtom floruineAtom = Default.ChemObjectBuilder.Instance.CreateAtom("Br");
                mol.Atoms.Add(floruineAtom);
                IBond bond = Default.ChemObjectBuilder.Instance.CreateBond(floruineAtom, atom1);
                mol.Bonds.Add(bond);
            }

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "C4", matched);

            for (int i = 1; i < mol.Atoms.Count; i++)
            {
                IAtom atom = mol.Atoms[i];
                matched = matcher.FindMatchingAtomType(mol, atom);
                AssertAtomType(testedAtomTypes, "atom " + i + " failed to match", "Br1", matched);
            }
        }

        [TestMethod()]
        public void TestIodine()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom atom1 = Default.ChemObjectBuilder.Instance.CreateAtom("C");
            atom1.ImplicitHydrogenCount = 0;
            mol.Atoms.Add(atom1);
            for (int i = 0; i < 4; i++)
            {
                IAtom floruineAtom = Default.ChemObjectBuilder.Instance.CreateAtom("I");
                mol.Atoms.Add(floruineAtom);
                IBond bond = Default.ChemObjectBuilder.Instance.CreateBond(floruineAtom, atom1);
                mol.Bonds.Add(bond);
            }

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "C4", matched);

            for (int i = 1; i < mol.Atoms.Count; i++)
            {
                IAtom atom = mol.Atoms[i];
                matched = matcher.FindMatchingAtomType(mol, atom);
                AssertAtomType(testedAtomTypes, "atom " + i + " failed to match", "I1", matched);
            }
        }

        [TestMethod()]
        public void TestLithium()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom atom1 = Default.ChemObjectBuilder.Instance.CreateAtom("Li");
            IAtom atom2 = Default.ChemObjectBuilder.Instance.CreateAtom("F");
            IBond bond = Default.ChemObjectBuilder.Instance.CreateBond(atom1, atom2);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Bonds.Add(bond);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "Li1", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "F1", matched);
        }

        /*
        /// Tests As3, Cl1
        /// </summary>
        [TestMethod()]
        public void TestArsenic()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom atom1 = Default.ChemObjectBuilder.Instance.CreateAtom("As");
            atom1.ImplicitHydrogenCount = 0;
            mol.Atoms.Add(atom1);
            for (int i = 0; i < 3; i++)
            {
                IAtom atom = Default.ChemObjectBuilder.Instance.CreateAtom("Cl");
                mol.Atoms.Add(atom);
                IBond bond = Default.ChemObjectBuilder.Instance.CreateBond(atom, atom1,
                        BondOrder.Single);
                mol.Bonds.Add(bond);
            }

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "As3", matched);

            for (int i = 1; i < mol.Atoms.Count; i++)
            {
                IAtom atom = mol.Atoms[i];
                matched = matcher.FindMatchingAtomType(mol, atom);
                AssertAtomType(testedAtomTypes, "atom " + i + " failed to match", "Cl1", matched);
            }
        }

        /*
        /// Tests C4, O2
        /// </summary>
        [TestMethod()]
        public void TestOxygen1()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom carbon = Default.ChemObjectBuilder.Instance.CreateAtom("C");
            IAtom o1 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom o2 = Default.ChemObjectBuilder.Instance.CreateAtom("O");

            carbon.ImplicitHydrogenCount = 1;
            o1.ImplicitHydrogenCount = 1;
            o2.ImplicitHydrogenCount = 0;

            IBond bond1 = Default.ChemObjectBuilder.Instance.CreateBond(carbon, o1, BondOrder.Single);
            IBond bond2 = Default.ChemObjectBuilder.Instance.CreateBond(carbon, o2, BondOrder.Double);

            mol.Atoms.Add(carbon);
            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);
            mol.Bonds.Add(bond1);
            mol.Bonds.Add(bond2);

            StructGenMatcher matcher = new StructGenMatcher();

            // look at the sp2 O first
            IAtomType matched = matcher.FindMatchingAtomType(mol, mol.Atoms[2]);
            AssertAtomType(testedAtomTypes, "O2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "C4", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "O2", matched);
        }

        /*
        /// Tests O2, H1
        /// </summary>
        [TestMethod()]
        public void TestOxygen2()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom o1 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom o2 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom h1 = Default.ChemObjectBuilder.Instance.CreateAtom("H");
            IAtom h2 = Default.ChemObjectBuilder.Instance.CreateAtom("H");

            IBond bond1 = Default.ChemObjectBuilder.Instance.CreateBond(h1, o1, BondOrder.Single);
            IBond bond2 = Default.ChemObjectBuilder.Instance.CreateBond(o1, o2, BondOrder.Single);
            IBond bond3 = Default.ChemObjectBuilder.Instance.CreateBond(o2, h2, BondOrder.Single);

            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);

            mol.Bonds.Add(bond1);
            mol.Bonds.Add(bond2);
            mol.Bonds.Add(bond3);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "O2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "O2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[2]);
            AssertAtomType(testedAtomTypes, "H1", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[3]);
            AssertAtomType(testedAtomTypes, "H1", matched);
        }

        /*
        /// Tests P4, S2, Cl1
        /// </summary>
        [TestMethod()]
        public void TestP4()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom p = Default.ChemObjectBuilder.Instance.CreateAtom("P");
            IAtom cl1 = Default.ChemObjectBuilder.Instance.CreateAtom("Cl");
            IAtom cl2 = Default.ChemObjectBuilder.Instance.CreateAtom("Cl");
            IAtom cl3 = Default.ChemObjectBuilder.Instance.CreateAtom("Cl");
            IAtom s = Default.ChemObjectBuilder.Instance.CreateAtom("S");

            IBond bond1 = Default.ChemObjectBuilder.Instance.CreateBond(p, cl1, BondOrder.Single);
            IBond bond2 = Default.ChemObjectBuilder.Instance.CreateBond(p, cl2, BondOrder.Single);
            IBond bond3 = Default.ChemObjectBuilder.Instance.CreateBond(p, cl3, BondOrder.Single);
            IBond bond4 = Default.ChemObjectBuilder.Instance.CreateBond(p, s, BondOrder.Double);

            mol.Atoms.Add(p);
            mol.Atoms.Add(cl1);
            mol.Atoms.Add(cl2);
            mol.Atoms.Add(cl3);
            mol.Atoms.Add(s);

            mol.Bonds.Add(bond1);
            mol.Bonds.Add(bond2);
            mol.Bonds.Add(bond3);
            mol.Bonds.Add(bond4);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "P4", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[4]);
            AssertAtomType(testedAtomTypes, "S2", matched);

            for (int i = 1; i < 4; i++)
            {
                matched = matcher.FindMatchingAtomType(mol, mol.Atoms[i]);
                AssertAtomType(testedAtomTypes, "atom " + i + " failed to match", "Cl1", matched);
            }
        }

        /*
        /// Tests P3, O2, C4
        /// </summary>
        [TestMethod()]
        public void TestP3()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom p = Default.ChemObjectBuilder.Instance.CreateAtom("P");
            IAtom o1 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom o2 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom o3 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom c1 = Default.ChemObjectBuilder.Instance.CreateAtom("C");
            IAtom c2 = Default.ChemObjectBuilder.Instance.CreateAtom("C");
            IAtom c3 = Default.ChemObjectBuilder.Instance.CreateAtom("C");

            c1.ImplicitHydrogenCount = 3;
            c2.ImplicitHydrogenCount = 3;
            c3.ImplicitHydrogenCount = 3;

            IBond bond1 = Default.ChemObjectBuilder.Instance.CreateBond(p, o1, BondOrder.Single);
            IBond bond2 = Default.ChemObjectBuilder.Instance.CreateBond(p, o2, BondOrder.Single);
            IBond bond3 = Default.ChemObjectBuilder.Instance.CreateBond(p, o3, BondOrder.Single);
            IBond bond4 = Default.ChemObjectBuilder.Instance.CreateBond(c1, o1, BondOrder.Single);
            IBond bond5 = Default.ChemObjectBuilder.Instance.CreateBond(c2, o2, BondOrder.Single);
            IBond bond6 = Default.ChemObjectBuilder.Instance.CreateBond(c3, o3, BondOrder.Single);

            mol.Atoms.Add(p);
            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);
            mol.Atoms.Add(o3);
            mol.Atoms.Add(c1);
            mol.Atoms.Add(c2);
            mol.Atoms.Add(c3);

            mol.Bonds.Add(bond1);
            mol.Bonds.Add(bond2);
            mol.Bonds.Add(bond3);
            mol.Bonds.Add(bond4);
            mol.Bonds.Add(bond5);
            mol.Bonds.Add(bond6);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            string[] atomTypes = { "P3", "O2", "O2", "O2", "C4", "C4", "C4" };
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                matched = matcher.FindMatchingAtomType(mol, mol.Atoms[i]);
                AssertAtomType(testedAtomTypes, atomTypes[i], matched);
            }
        }

        /* Test Na1, Cl1 */
        [TestMethod()]
        public void TestNa1()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom na = Default.ChemObjectBuilder.Instance.CreateAtom("Na");
            IAtom cl = Default.ChemObjectBuilder.Instance.CreateAtom("Cl");
            IBond bond = Default.ChemObjectBuilder.Instance.CreateBond(na, cl, BondOrder.Single);
            mol.Atoms.Add(na);
            mol.Atoms.Add(cl);
            mol.Bonds.Add(bond);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "Na1", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "Cl1", matched);
        }

        /* Test Si4, C4, Cl1 */
        [TestMethod()]
        public void TestSi4()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom si = Default.ChemObjectBuilder.Instance.CreateAtom("Si");
            IAtom c1 = Default.ChemObjectBuilder.Instance.CreateAtom("C");
            IAtom cl1 = Default.ChemObjectBuilder.Instance.CreateAtom("Cl");
            IAtom cl2 = Default.ChemObjectBuilder.Instance.CreateAtom("Cl");
            IAtom cl3 = Default.ChemObjectBuilder.Instance.CreateAtom("Cl");

            c1.ImplicitHydrogenCount = 3;

            IBond bond1 = Default.ChemObjectBuilder.Instance.CreateBond(si, c1, BondOrder.Single);
            IBond bond2 = Default.ChemObjectBuilder.Instance.CreateBond(si, cl1, BondOrder.Single);
            IBond bond3 = Default.ChemObjectBuilder.Instance.CreateBond(si, cl2, BondOrder.Single);
            IBond bond4 = Default.ChemObjectBuilder.Instance.CreateBond(si, cl3, BondOrder.Single);

            mol.Atoms.Add(si);
            mol.Atoms.Add(c1);
            mol.Atoms.Add(cl1);
            mol.Atoms.Add(cl2);
            mol.Atoms.Add(cl3);

            mol.Bonds.Add(bond1);
            mol.Bonds.Add(bond2);
            mol.Bonds.Add(bond3);
            mol.Bonds.Add(bond4);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "Si4", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "C4", matched);

            for (int i = 3; i < mol.Atoms.Count; i++)
            {
                matched = matcher.FindMatchingAtomType(mol, mol.Atoms[i]);
                AssertAtomType(testedAtomTypes, "atom " + i + " failed to match", "Cl1", matched);
            }
        }

        /* Tests S2, H1 */
        [TestMethod()]
        public void TestS2()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom s = Default.ChemObjectBuilder.Instance.CreateAtom("S");
            s.ImplicitHydrogenCount = 2;

            mol.Atoms.Add(s);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "S2", matched);

            mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            s = Default.ChemObjectBuilder.Instance.CreateAtom("S");
            IAtom h1 = Default.ChemObjectBuilder.Instance.CreateAtom("H");
            IAtom h2 = Default.ChemObjectBuilder.Instance.CreateAtom("H");
            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(s, h1, BondOrder.Single);
            IBond b2 = Default.ChemObjectBuilder.Instance.CreateBond(s, h2, BondOrder.Single);

            mol.Atoms.Add(s);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "S2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "H1", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[2]);
            AssertAtomType(testedAtomTypes, "H1", matched);
        }

        /* Tests S3, O2 */
        [TestMethod()]
        public void TestS3()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom s = Default.ChemObjectBuilder.Instance.CreateAtom("S");
            IAtom o1 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom o2 = Default.ChemObjectBuilder.Instance.CreateAtom("O");

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(s, o1, BondOrder.Double);
            IBond b2 = Default.ChemObjectBuilder.Instance.CreateBond(s, o2, BondOrder.Double);

            mol.Atoms.Add(s);
            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "S3", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "O2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[2]);
            AssertAtomType(testedAtomTypes, "O2", matched);
        }

        /* Tests S4, Cl1 */
        [TestMethod()]
        public void TestS4()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom s = Default.ChemObjectBuilder.Instance.CreateAtom("S");
            mol.Atoms.Add(s);
            for (int i = 0; i < 6; i++)
            {
                IAtom f = Default.ChemObjectBuilder.Instance.CreateAtom("F");
                mol.Atoms.Add(f);
                IBond bond = Default.ChemObjectBuilder.Instance.CreateBond(s, f, BondOrder.Single);
                mol.Bonds.Add(bond);
            }

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "S4", matched);

            for (int i = 1; i < mol.Atoms.Count; i++)
            {
                matched = matcher.FindMatchingAtomType(mol, mol.Atoms[i]);
                AssertAtomType(testedAtomTypes, "atom " + i + " failed to match", "F1", matched);
            }
        }

        /* Tests S4, O2 */
        [TestMethod()]
        public void TestS4oxide()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom s = Default.ChemObjectBuilder.Instance.CreateAtom("S");
            IAtom o1 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom o2 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom o3 = Default.ChemObjectBuilder.Instance.CreateAtom("O");

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(s, o1, BondOrder.Double);
            IBond b2 = Default.ChemObjectBuilder.Instance.CreateBond(s, o2, BondOrder.Double);
            IBond b3 = Default.ChemObjectBuilder.Instance.CreateBond(s, o3, BondOrder.Double);

            mol.Atoms.Add(s);
            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);
            mol.Atoms.Add(o3);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);
            mol.Bonds.Add(b3);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "S4", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "O2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[2]);
            AssertAtomType(testedAtomTypes, "O2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[3]);
            AssertAtomType(testedAtomTypes, "O2", matched);
        }

        /* Tests N3, O2 */
        [TestMethod()]
        public void TestN3acid()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom n = Default.ChemObjectBuilder.Instance.CreateAtom("N");
            IAtom o = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom h = Default.ChemObjectBuilder.Instance.CreateAtom("H");

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(n, o, BondOrder.Double);
            IBond b2 = Default.ChemObjectBuilder.Instance.CreateBond(n, h, BondOrder.Single);

            mol.Atoms.Add(n);
            mol.Atoms.Add(o);
            mol.Atoms.Add(h);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "N3", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "O2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[2]);
            AssertAtomType(testedAtomTypes, "H1", matched);
        }

        [TestMethod()]
        public void TestN3cyanide()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom n = Default.ChemObjectBuilder.Instance.CreateAtom("N");
            IAtom c1 = Default.ChemObjectBuilder.Instance.CreateAtom("C");
            IAtom c2 = Default.ChemObjectBuilder.Instance.CreateAtom("C");

            c1.ImplicitHydrogenCount = 0;
            c2.ImplicitHydrogenCount = 3;

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(n, c1, BondOrder.Triple);
            IBond b2 = Default.ChemObjectBuilder.Instance.CreateBond(c1, c2, BondOrder.Single);

            mol.Atoms.Add(n);
            mol.Atoms.Add(c1);
            mol.Atoms.Add(c2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "N3", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "C4", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[2]);
            AssertAtomType(testedAtomTypes, "C4", matched);
        }

        /* Tests N5, O2, C4 */
        [TestMethod()]
        public void TestN5()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom n = Default.ChemObjectBuilder.Instance.CreateAtom("N");
            IAtom o1 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom o2 = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IAtom c = Default.ChemObjectBuilder.Instance.CreateAtom("C");

            c.ImplicitHydrogenCount = 3;

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(n, o1, BondOrder.Double);
            IBond b2 = Default.ChemObjectBuilder.Instance.CreateBond(n, o2, BondOrder.Double);
            IBond b3 = Default.ChemObjectBuilder.Instance.CreateBond(n, c, BondOrder.Single);

            mol.Atoms.Add(n);
            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);
            mol.Atoms.Add(c);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);
            mol.Bonds.Add(b3);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "N5", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "O2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[2]);
            AssertAtomType(testedAtomTypes, "O2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[3]);
            AssertAtomType(testedAtomTypes, "C4", matched);
        }

        /* Test B3, F1 */
        [TestMethod()]
        public void TestB3()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom b = Default.ChemObjectBuilder.Instance.CreateAtom("B");
            IAtom f1 = Default.ChemObjectBuilder.Instance.CreateAtom("F");
            IAtom f2 = Default.ChemObjectBuilder.Instance.CreateAtom("F");
            IAtom f3 = Default.ChemObjectBuilder.Instance.CreateAtom("F");

            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(b, f1, BondOrder.Single);
            IBond b2 = Default.ChemObjectBuilder.Instance.CreateBond(b, f2, BondOrder.Single);
            IBond b3 = Default.ChemObjectBuilder.Instance.CreateBond(b, f3, BondOrder.Single);

            mol.Atoms.Add(b);
            mol.Atoms.Add(f1);
            mol.Atoms.Add(f2);
            mol.Atoms.Add(f3);
            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);
            mol.Bonds.Add(b3);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "B3", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "F1", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[2]);
            AssertAtomType(testedAtomTypes, "F1", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[3]);
            AssertAtomType(testedAtomTypes, "F1", matched);
        }

        [TestMethod()]
        public void TestSe2()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom se = Default.ChemObjectBuilder.Instance.CreateAtom("Se");
            IAtom o = Default.ChemObjectBuilder.Instance.CreateAtom("O");
            IBond b1 = Default.ChemObjectBuilder.Instance.CreateBond(se, o, BondOrder.Double);
            mol.Atoms.Add(se);
            mol.Atoms.Add(o);
            mol.Bonds.Add(b1);

            StructGenMatcher matcher = new StructGenMatcher();
            IAtomType matched;

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[0]);
            AssertAtomType(testedAtomTypes, "Se2", matched);

            matched = matcher.FindMatchingAtomType(mol, mol.Atoms[1]);
            AssertAtomType(testedAtomTypes, "O2", matched);

        }

        /// <summary>
        /// The test seems to be run by JUnit in order in which they found
        /// in the source. Ugly, but @AfterClass does not work because that
        /// methods does cannot assert anything.
        ///
        /// ...not anymore. Bad idea to do have such a test in the first place
        /// but we can hack it by sorting by test name (see fix method order
        /// annotation).
        /// </summary>
        [ClassCleanup()]
        public static void UTestCountTestedAtomTypes()
        {
            AtomTypeFactory factory = AtomTypeFactory.GetInstance(
                    "NCDK.Config.Data.structgen_atomtypes.xml", Silent.ChemObjectBuilder.Instance);

            var expectedTypes = factory.GetAllAtomTypes().ToList();
            if (expectedTypes.Count != testedAtomTypes.Count)
            {
                string errorMessage = "Atom types not tested:";
                foreach (var expectedType in expectedTypes)
                {
                    if (!testedAtomTypes.ContainsKey(expectedType.AtomTypeName))
                        errorMessage += " " + expectedType.AtomTypeName;
                }
                Assert.AreEqual(factory.GetAllAtomTypes().Count(), testedAtomTypes.Count, errorMessage);
            }
        }
    }
}
