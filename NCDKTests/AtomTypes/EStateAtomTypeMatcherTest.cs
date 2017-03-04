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
using NCDK.Aromaticities;
using NCDK.Default;
using NCDK.RingSearches;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.AtomTypes
{
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class EStateAtomTypeMatcherTest : CDKTestCase
    {
        static EStateAtomTypeMatcher matcher = new EStateAtomTypeMatcher();
        IAtomContainer mol = null;

        public EStateAtomTypeMatcherTest()
            : base()
        { }

        [TestMethod()]
        public void TestEStateAtomTypeMatcher()
        {
            Assert.IsNotNull(matcher);
        }

        IRingSet GetRings()
        {
            IRingSet rs = null;
            try
            {
                AllRingsFinder arf = new AllRingsFinder();
                rs = arf.FindAllRings(mol);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Could not find all rings: " + e.Message);
            }
            return (rs);
        }

        private bool TestAtom(string expectedAtType, IAtom atom)
        {
            return expectedAtType.Equals(matcher.FindMatchingAtomType(mol, atom).AtomTypeName);
        }

        [TestMethod()]
        public void TestFindMatchingAtomType_IAtomContainer()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom atom = new Atom("C");
            Hybridization thisHybridization = Hybridization.SP3;
            atom.Hybridization = thisHybridization;
            mol.Atoms.Add(atom);

            // just check consistency; other methods do perception testing
            IAtomType[] types = matcher.FindMatchingAtomTypes(mol);
            for (int i = 0; i < types.Length; i++)
            {
                IAtomType type = matcher.FindMatchingAtomType(mol, mol.Atoms[i]);
                Assert.AreEqual(type.AtomTypeName, types[i].AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestSP3Atoms()
        {
            //Testing with CC(C)(C)CC
            mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a14);
            IAtom a15 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a15);
            IAtom a16 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a16);
            IAtom a17 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a17);
            IAtom a18 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a18);
            IAtom a19 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a19);
            IAtom a20 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a20);
            IBond b1 = mol.Builder.CreateBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a4, a2, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a5, a2, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a6, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a1, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.CreateBond(a1, a9, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.CreateBond(a3, a10, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.CreateBond(a3, a11, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.CreateBond(a3, a12, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.CreateBond(a4, a13, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.CreateBond(a4, a14, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = mol.Builder.CreateBond(a4, a15, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = mol.Builder.CreateBond(a5, a16, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = mol.Builder.CreateBond(a5, a17, BondOrder.Single);
            mol.Bonds.Add(b16);
            IBond b17 = mol.Builder.CreateBond(a6, a18, BondOrder.Single);
            mol.Bonds.Add(b17);
            IBond b18 = mol.Builder.CreateBond(a6, a19, BondOrder.Single);
            mol.Bonds.Add(b18);
            IBond b19 = mol.Builder.CreateBond(a6, a20, BondOrder.Single);
            mol.Bonds.Add(b19);

            matcher.RingSet = GetRings();
            Assert.IsTrue(TestAtom("SsCH3", a1));
            Assert.IsTrue(TestAtom("SssssC", a2));
            Assert.IsTrue(TestAtom("SsCH3", a3));
            Assert.IsTrue(TestAtom("SsCH3", a4));
            Assert.IsTrue(TestAtom("SssCH2", a5));
            Assert.IsTrue(TestAtom("SsCH3", a6));
            Assert.IsTrue(TestAtom("SsH", a7));
            Assert.IsTrue(TestAtom("SsH", a8));
        }

        [TestMethod()]
        public void TestSP2Atoms()
        {
            //Test with C=CC=N
            mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("N");
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a9);
            IBond b1 = mol.Builder.CreateBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a4, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a2, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a3, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.CreateBond(a4, a9, BondOrder.Single);
            mol.Bonds.Add(b8);

            matcher.RingSet = GetRings();
            Assert.IsTrue(TestAtom("SdCH2", a1));
            Assert.IsTrue(TestAtom("SdsCH", a2));
            Assert.IsTrue(TestAtom("SdsCH", a3));
            Assert.IsTrue(TestAtom("SdNH", a4));
            Assert.IsTrue(TestAtom("SsH", a9));
        }

        [TestMethod()]
        public void TestSPAtoms()
        {
            //Testing with  C#CCC#N
            mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("N");
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a8);
            IBond b1 = mol.Builder.CreateBond(a2, a1, BondOrder.Triple);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a5, a4, BondOrder.Triple);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a3, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a3, a8, BondOrder.Single);
            mol.Bonds.Add(b7);

            matcher.RingSet = GetRings();
            Assert.IsTrue(TestAtom("StCH", a1));
            Assert.IsTrue(TestAtom("StsC", a2));
            Assert.IsTrue(TestAtom("SssCH2", a3));
            Assert.IsTrue(TestAtom("StsC", a4));
            Assert.IsTrue(TestAtom("StN", a5));
        }

        [TestMethod()]
        public void TestAromaticAtoms()
        {
            //Testing with C1=CN=CC=C1C
            mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("C");
            a1.IsAromatic = true;
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("C");
            a2.IsAromatic = true;
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("N");
            a3.IsAromatic = true;
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("C");
            a4.IsAromatic = true;
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("C");
            a5.IsAromatic = true;
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("C");
            a6.IsAromatic = true;
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a14);
            IBond b1 = mol.Builder.CreateBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a4, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a5, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a6, a5, BondOrder.Double);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a7, a6, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.CreateBond(a1, a8, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.CreateBond(a2, a9, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.CreateBond(a5, a10, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.CreateBond(a7, a11, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.CreateBond(a7, a12, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.CreateBond(a7, a13, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = mol.Builder.CreateBond(a4, a14, BondOrder.Single);
            mol.Bonds.Add(b14);

            matcher.RingSet = GetRings();
            Assert.IsTrue(TestAtom("SaaCH", a1));
            Assert.IsTrue(TestAtom("SaaCH", a2));
            Assert.IsTrue(TestAtom("SaaN", a3));
            Assert.IsTrue(TestAtom("SaaCH", a4));
            Assert.IsTrue(TestAtom("SaaCH", a5));
            Assert.IsTrue(TestAtom("SsaaC", a6));
            Assert.IsTrue(TestAtom("SsCH3", a7));
        }

        [TestMethod()]
        public void TestBenzeneFromSmiles()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            mol = sp.ParseSmiles("C1=CC=CC=C1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);

            matcher.RingSet = GetRings();
            foreach (var atom in mol.Atoms)
            {
                if (atom.Symbol.Equals("C"))
                {
                    Assert.IsTrue(TestAtom("SaaCH", atom));
                }
            }
        }

        [TestMethod()]
        public void TestNaphthalene()
        {
            //Testing with C1=CC2C=CC=CC=2C=C1
            mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("C");
            a1.IsAromatic = true;
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("C");
            a2.IsAromatic = true;
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            a3.IsAromatic = true;
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("C");
            a4.IsAromatic = true;
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("C");
            a5.IsAromatic = true;
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("C");
            a6.IsAromatic = true;
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("C");
            a7.IsAromatic = true;
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("C");
            a8.IsAromatic = true;
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.CreateAtom("C");
            a9.IsAromatic = true;
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.CreateAtom("C");
            a10.IsAromatic = true;
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a14);
            IAtom a15 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a15);
            IAtom a16 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a16);
            IAtom a17 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a17);
            IAtom a18 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a18);
            IBond b1 = mol.Builder.CreateBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a5, a4, BondOrder.Double);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a6, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a7, a6, BondOrder.Double);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a8, a7, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.CreateBond(a8, a3, BondOrder.Double);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.CreateBond(a9, a8, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.CreateBond(a10, a9, BondOrder.Double);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.CreateBond(a10, a1, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.CreateBond(a1, a11, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.CreateBond(a2, a12, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = mol.Builder.CreateBond(a10, a13, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = mol.Builder.CreateBond(a9, a14, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = mol.Builder.CreateBond(a4, a15, BondOrder.Single);
            mol.Bonds.Add(b16);
            IBond b17 = mol.Builder.CreateBond(a5, a16, BondOrder.Single);
            mol.Bonds.Add(b17);
            IBond b18 = mol.Builder.CreateBond(a7, a17, BondOrder.Single);
            mol.Bonds.Add(b18);
            IBond b19 = mol.Builder.CreateBond(a6, a18, BondOrder.Single);
            mol.Bonds.Add(b19);

            matcher.RingSet = GetRings();
            Assert.IsTrue(TestAtom("SaaCH", a1));
            Assert.IsTrue(TestAtom("SaaCH", a2));
            Assert.IsTrue(TestAtom("SaaaC", a3));
            Assert.IsTrue(TestAtom("SaaCH", a4));
            Assert.IsTrue(TestAtom("SaaCH", a5));
            Assert.IsTrue(TestAtom("SaaCH", a6));
            Assert.IsTrue(TestAtom("SaaCH", a7));
            Assert.IsTrue(TestAtom("SaaaC", a8));
            Assert.IsTrue(TestAtom("SaaCH", a9));
            Assert.IsTrue(TestAtom("SaaCH", a10));
        }

        [TestMethod()]
        public void TestChargedAtoms()
        {
            //Testing with C[N+]
            mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("N");
            a2.FormalCharge = +1;
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("H");
            mol.Atoms.Add(a8);
            IBond b1 = mol.Builder.CreateBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a2, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a2, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a2, a8, BondOrder.Single);
            mol.Bonds.Add(b7);

            matcher.RingSet = GetRings();
            Assert.IsTrue(TestAtom("SsCH3", a1));
            Assert.IsTrue(TestAtom("SsNpH3", a2));
        }

        [TestMethod()]
        public void TestNaCl()
        {
            //Testing with [Na+].[Cl-]
            mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("Na");
            a1.FormalCharge = +1;
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("Cl");
            a2.FormalCharge = -1;
            mol.Atoms.Add(a2);

            matcher.RingSet = GetRings();
            Assert.IsTrue(TestAtom("SNap", a1));
            Assert.IsTrue(TestAtom("SClm", a2));

            //Testing with different presentation - [Na]Cl
            mol = new AtomContainer();
            a1 = mol.Builder.CreateAtom("Na");
            mol.Atoms.Add(a1);
            a2 = mol.Builder.CreateAtom("Cl");
            mol.Atoms.Add(a2);
            IBond b1 = mol.Builder.CreateBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);

            matcher.RingSet = GetRings();
            Assert.IsTrue(TestAtom("SsNa", a1));
            Assert.IsTrue(TestAtom("SsCl", a2));
        }
    }
}
