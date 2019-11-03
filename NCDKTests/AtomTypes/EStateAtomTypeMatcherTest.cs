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
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.AtomTypes
{
    // @cdk.module test-standard
    [TestClass()]
    public class EStateAtomTypeMatcherTest : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;

        public EStateAtomTypeMatcherTest()
            : base()
        { }

        private static IRingSet GetRings(IAtomContainer mol)
        {
            try
            {
                var arf = new AllRingsFinder();
                var rs = arf.FindAllRings(mol);
                return rs;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine($"Could not find all rings: {e.Message}");
            }
            return null;
        }

        private bool TestAtom(IAtomTypeMatcher matcher, IAtomContainer mol, string expectedAtType, IAtom atom)
        {
            return expectedAtType.Equals(matcher.FindMatchingAtomType(mol, atom).AtomTypeName);
        }

        [TestMethod()]
        public void TestFindMatchingAtomType_IAtomContainer()
        {
            var matcher = new EStateAtomTypeMatcher();
            var mol = builder.NewAtomContainer();
            var atom = builder.NewAtom("C");
            var thisHybridization = Hybridization.SP3;
            atom.Hybridization = thisHybridization;
            mol.Atoms.Add(atom);

            // just check consistency; other methods do perception testing
            var types = matcher.FindMatchingAtomTypes(mol).ToReadOnlyList();
            for (int i = 0; i < types.Count; i++)
            {
                var type = matcher.FindMatchingAtomType(mol, mol.Atoms[i]);
                Assert.AreEqual(type.AtomTypeName, types[i].AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestSP3Atoms()
        {
            //Testing with CC(C)(C)CC
            var mol = builder.NewAtomContainer();
            var a1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a1);
            var a2 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a2);
            var a3 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a3);
            var a4 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a4);
            var a5 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a5);
            var a6 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a6);
            var a7 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a7);
            var a8 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a8);
            var a9 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a9);
            var a10 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a10);
            var a11 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a11);
            var a12 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a12);
            var a13 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a13);
            var a14 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a14);
            var a15 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a15);
            var a16 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a16);
            var a17 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a17);
            var a18 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a18);
            var a19 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a19);
            var a20 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a20);
            var b1 = mol.Builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            var b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            var b3 = mol.Builder.NewBond(a4, a2, BondOrder.Single);
            mol.Bonds.Add(b3);
            var b4 = mol.Builder.NewBond(a5, a2, BondOrder.Single);
            mol.Bonds.Add(b4);
            var b5 = mol.Builder.NewBond(a6, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            var b6 = mol.Builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            var b7 = mol.Builder.NewBond(a1, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            var b8 = mol.Builder.NewBond(a1, a9, BondOrder.Single);
            mol.Bonds.Add(b8);
            var b9 = mol.Builder.NewBond(a3, a10, BondOrder.Single);
            mol.Bonds.Add(b9);
            var b10 = mol.Builder.NewBond(a3, a11, BondOrder.Single);
            mol.Bonds.Add(b10);
            var b11 = mol.Builder.NewBond(a3, a12, BondOrder.Single);
            mol.Bonds.Add(b11);
            var b12 = mol.Builder.NewBond(a4, a13, BondOrder.Single);
            mol.Bonds.Add(b12);
            var b13 = mol.Builder.NewBond(a4, a14, BondOrder.Single);
            mol.Bonds.Add(b13);
            var b14 = mol.Builder.NewBond(a4, a15, BondOrder.Single);
            mol.Bonds.Add(b14);
            var b15 = mol.Builder.NewBond(a5, a16, BondOrder.Single);
            mol.Bonds.Add(b15);
            var b16 = mol.Builder.NewBond(a5, a17, BondOrder.Single);
            mol.Bonds.Add(b16);
            var b17 = mol.Builder.NewBond(a6, a18, BondOrder.Single);
            mol.Bonds.Add(b17);
            var b18 = mol.Builder.NewBond(a6, a19, BondOrder.Single);
            mol.Bonds.Add(b18);
            var b19 = mol.Builder.NewBond(a6, a20, BondOrder.Single);
            mol.Bonds.Add(b19);

            var matcher = new EStateAtomTypeMatcher(GetRings(mol));
            Assert.IsTrue(TestAtom(matcher, mol, "SsCH3", a1));
            Assert.IsTrue(TestAtom(matcher, mol, "SssssC", a2));
            Assert.IsTrue(TestAtom(matcher, mol, "SsCH3", a3));
            Assert.IsTrue(TestAtom(matcher, mol, "SsCH3", a4));
            Assert.IsTrue(TestAtom(matcher, mol, "SssCH2", a5));
            Assert.IsTrue(TestAtom(matcher, mol, "SsCH3", a6));
            Assert.IsTrue(TestAtom(matcher, mol, "SsH", a7));
            Assert.IsTrue(TestAtom(matcher, mol, "SsH", a8));
        }

        [TestMethod()]
        public void TestSP2Atoms()
        {
            //Test with C=CC=N
            var mol = builder.NewAtomContainer();
            var a1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a1);
            var a2 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a2);
            var a3 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a3);
            var a4 = mol.Builder.NewAtom("N");
            mol.Atoms.Add(a4);
            var a5 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a5);
            var a6 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a6);
            var a7 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a7);
            var a8 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a8);
            var a9 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a9);
            var b1 = mol.Builder.NewBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            var b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            var b3 = mol.Builder.NewBond(a4, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            var b4 = mol.Builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            var b5 = mol.Builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            var b6 = mol.Builder.NewBond(a2, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            var b7 = mol.Builder.NewBond(a3, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            var b8 = mol.Builder.NewBond(a4, a9, BondOrder.Single);
            mol.Bonds.Add(b8);

            var matcher = new EStateAtomTypeMatcher(GetRings(mol));
            Assert.IsTrue(TestAtom(matcher, mol, "SdCH2", a1));
            Assert.IsTrue(TestAtom(matcher, mol, "SdsCH", a2));
            Assert.IsTrue(TestAtom(matcher, mol, "SdsCH", a3));
            Assert.IsTrue(TestAtom(matcher, mol, "SdNH", a4));
            Assert.IsTrue(TestAtom(matcher, mol, "SsH", a9));
        }

        [TestMethod()]
        public void TestSPAtoms()
        {
            //Testing with  C#CCC#N
            var mol = builder.NewAtomContainer();
            var a1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a1);
            var a2 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a2);
            var a3 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a3);
            var a4 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a4);
            var a5 = mol.Builder.NewAtom("N");
            mol.Atoms.Add(a5);
            var a6 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a6);
            var a7 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a7);
            var a8 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a8);
            var b1 = mol.Builder.NewBond(a2, a1, BondOrder.Triple);
            mol.Bonds.Add(b1);
            var b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            var b3 = mol.Builder.NewBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            var b4 = mol.Builder.NewBond(a5, a4, BondOrder.Triple);
            mol.Bonds.Add(b4);
            var b5 = mol.Builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            var b6 = mol.Builder.NewBond(a3, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            var b7 = mol.Builder.NewBond(a3, a8, BondOrder.Single);
            mol.Bonds.Add(b7);

            var matcher = new EStateAtomTypeMatcher(GetRings(mol));
            Assert.IsTrue(TestAtom(matcher, mol, "StCH", a1));
            Assert.IsTrue(TestAtom(matcher, mol, "StsC", a2));
            Assert.IsTrue(TestAtom(matcher, mol, "SssCH2", a3));
            Assert.IsTrue(TestAtom(matcher, mol, "StsC", a4));
            Assert.IsTrue(TestAtom(matcher, mol, "StN", a5));
        }

        [TestMethod()]
        public void TestAromaticAtoms()
        {
            //Testing with C1=CN=CC=C1C
            var mol = builder.NewAtomContainer();
            var a1 = mol.Builder.NewAtom("C");
            a1.IsAromatic = true;
            mol.Atoms.Add(a1);
            var a2 = mol.Builder.NewAtom("C");
            a2.IsAromatic = true;
            mol.Atoms.Add(a2);
            var a3 = mol.Builder.NewAtom("N");
            a3.IsAromatic = true;
            mol.Atoms.Add(a3);
            var a4 = mol.Builder.NewAtom("C");
            a4.IsAromatic = true;
            mol.Atoms.Add(a4);
            var a5 = mol.Builder.NewAtom("C");
            a5.IsAromatic = true;
            mol.Atoms.Add(a5);
            var a6 = mol.Builder.NewAtom("C");
            a6.IsAromatic = true;
            mol.Atoms.Add(a6);
            var a7 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a7);
            var a8 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a8);
            var a9 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a9);
            var a10 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a10);
            var a11 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a11);
            var a12 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a12);
            var a13 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a13);
            var a14 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a14);
            var b1 = mol.Builder.NewBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            var b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            var b3 = mol.Builder.NewBond(a4, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            var b4 = mol.Builder.NewBond(a5, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            var b5 = mol.Builder.NewBond(a6, a5, BondOrder.Double);
            mol.Bonds.Add(b5);
            var b6 = mol.Builder.NewBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b6);
            var b7 = mol.Builder.NewBond(a7, a6, BondOrder.Single);
            mol.Bonds.Add(b7);
            var b8 = mol.Builder.NewBond(a1, a8, BondOrder.Single);
            mol.Bonds.Add(b8);
            var b9 = mol.Builder.NewBond(a2, a9, BondOrder.Single);
            mol.Bonds.Add(b9);
            var b10 = mol.Builder.NewBond(a5, a10, BondOrder.Single);
            mol.Bonds.Add(b10);
            var b11 = mol.Builder.NewBond(a7, a11, BondOrder.Single);
            mol.Bonds.Add(b11);
            var b12 = mol.Builder.NewBond(a7, a12, BondOrder.Single);
            mol.Bonds.Add(b12);
            var b13 = mol.Builder.NewBond(a7, a13, BondOrder.Single);
            mol.Bonds.Add(b13);
            var b14 = mol.Builder.NewBond(a4, a14, BondOrder.Single);
            mol.Bonds.Add(b14);

            var matcher = new EStateAtomTypeMatcher(GetRings(mol));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a1));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a2));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaN", a3));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a4));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a5));
            Assert.IsTrue(TestAtom(matcher, mol, "SsaaC", a6));
            Assert.IsTrue(TestAtom(matcher, mol, "SsCH3", a7));
        }

        [TestMethod()]
        public void TestBenzeneFromSmiles()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C1=CC=CC=C1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);

            var matcher = new EStateAtomTypeMatcher(GetRings(mol));
            foreach (var atom in mol.Atoms)
            {
                if (atom.Symbol.Equals("C"))
                {
                    Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", atom));
                }
            }
        }

        [TestMethod()]
        public void TestNaphthalene()
        {
            //Testing with C1=CC2C=CC=CC=2C=C1
            var mol = builder.NewAtomContainer();
            var a1 = mol.Builder.NewAtom("C");
            a1.IsAromatic = true;
            mol.Atoms.Add(a1);
            var a2 = mol.Builder.NewAtom("C");
            a2.IsAromatic = true;
            mol.Atoms.Add(a2);
            var a3 = mol.Builder.NewAtom("C");
            a3.IsAromatic = true;
            mol.Atoms.Add(a3);
            var a4 = mol.Builder.NewAtom("C");
            a4.IsAromatic = true;
            mol.Atoms.Add(a4);
            var a5 = mol.Builder.NewAtom("C");
            a5.IsAromatic = true;
            mol.Atoms.Add(a5);
            var a6 = mol.Builder.NewAtom("C");
            a6.IsAromatic = true;
            mol.Atoms.Add(a6);
            var a7 = mol.Builder.NewAtom("C");
            a7.IsAromatic = true;
            mol.Atoms.Add(a7);
            var a8 = mol.Builder.NewAtom("C");
            a8.IsAromatic = true;
            mol.Atoms.Add(a8);
            var a9 = mol.Builder.NewAtom("C");
            a9.IsAromatic = true;
            mol.Atoms.Add(a9);
            var a10 = mol.Builder.NewAtom("C");
            a10.IsAromatic = true;
            mol.Atoms.Add(a10);
            var a11 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a11);
            var a12 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a12);
            var a13 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a13);
            var a14 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a14);
            var a15 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a15);
            var a16 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a16);
            var a17 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a17);
            var a18 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a18);
            var b1 = mol.Builder.NewBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            var b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            var b3 = mol.Builder.NewBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            var b4 = mol.Builder.NewBond(a5, a4, BondOrder.Double);
            mol.Bonds.Add(b4);
            var b5 = mol.Builder.NewBond(a6, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            var b6 = mol.Builder.NewBond(a7, a6, BondOrder.Double);
            mol.Bonds.Add(b6);
            var b7 = mol.Builder.NewBond(a8, a7, BondOrder.Single);
            mol.Bonds.Add(b7);
            var b8 = mol.Builder.NewBond(a8, a3, BondOrder.Double);
            mol.Bonds.Add(b8);
            var b9 = mol.Builder.NewBond(a9, a8, BondOrder.Single);
            mol.Bonds.Add(b9);
            var b10 = mol.Builder.NewBond(a10, a9, BondOrder.Double);
            mol.Bonds.Add(b10);
            var b11 = mol.Builder.NewBond(a10, a1, BondOrder.Single);
            mol.Bonds.Add(b11);
            var b12 = mol.Builder.NewBond(a1, a11, BondOrder.Single);
            mol.Bonds.Add(b12);
            var b13 = mol.Builder.NewBond(a2, a12, BondOrder.Single);
            mol.Bonds.Add(b13);
            var b14 = mol.Builder.NewBond(a10, a13, BondOrder.Single);
            mol.Bonds.Add(b14);
            var b15 = mol.Builder.NewBond(a9, a14, BondOrder.Single);
            mol.Bonds.Add(b15);
            var b16 = mol.Builder.NewBond(a4, a15, BondOrder.Single);
            mol.Bonds.Add(b16);
            var b17 = mol.Builder.NewBond(a5, a16, BondOrder.Single);
            mol.Bonds.Add(b17);
            var b18 = mol.Builder.NewBond(a7, a17, BondOrder.Single);
            mol.Bonds.Add(b18);
            var b19 = mol.Builder.NewBond(a6, a18, BondOrder.Single);
            mol.Bonds.Add(b19);

            var matcher = new EStateAtomTypeMatcher(GetRings(mol));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a1));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a2));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaaC", a3));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a4));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a5));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a6));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a7));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaaC", a8));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a9));
            Assert.IsTrue(TestAtom(matcher, mol, "SaaCH", a10));
        }

        [TestMethod()]
        public void TestChargedAtoms()
        {
            //Testing with C[N+]
            var mol = builder.NewAtomContainer();
            var a1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a1);
            var a2 = mol.Builder.NewAtom("N");
            a2.FormalCharge = +1;
            mol.Atoms.Add(a2);
            var a3 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a3);
            var a4 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a4);
            var a5 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a5);
            var a6 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a6);
            var a7 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a7);
            var a8 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a8);
            var b1 = mol.Builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            var b2 = mol.Builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            var b3 = mol.Builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            var b4 = mol.Builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            var b5 = mol.Builder.NewBond(a2, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            var b6 = mol.Builder.NewBond(a2, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            var b7 = mol.Builder.NewBond(a2, a8, BondOrder.Single);
            mol.Bonds.Add(b7);

            var matcher = new EStateAtomTypeMatcher(GetRings(mol));
            Assert.IsTrue(TestAtom(matcher, mol, "SsCH3", a1));
            Assert.IsTrue(TestAtom(matcher, mol, "SsNpH3", a2));
        }

        [TestMethod()]
        public void TestNaCl()
        {
            //Testing with [Na+].[Cl-]
            var mol = builder.NewAtomContainer();
            var a1 = mol.Builder.NewAtom("Na");
            a1.FormalCharge = +1;
            mol.Atoms.Add(a1);
            var a2 = mol.Builder.NewAtom("Cl");
            a2.FormalCharge = -1;
            mol.Atoms.Add(a2);

            {
                var matcher = new EStateAtomTypeMatcher(GetRings(mol));
                Assert.IsTrue(TestAtom(matcher, mol, "SNap", a1));
                Assert.IsTrue(TestAtom(matcher, mol, "SClm", a2));

                //Testing with different presentation - [Na]Cl
                mol = builder.NewAtomContainer();
                a1 = mol.Builder.NewAtom("Na");
                mol.Atoms.Add(a1);
                a2 = mol.Builder.NewAtom("Cl");
                mol.Atoms.Add(a2);
                var b1 = mol.Builder.NewBond(a2, a1, BondOrder.Single);
                mol.Bonds.Add(b1);
            }

            {
                var matcher = new EStateAtomTypeMatcher(GetRings(mol));
                Assert.IsTrue(TestAtom(matcher, mol, "SsNa", a1));
                Assert.IsTrue(TestAtom(matcher, mol, "SsCl", a2));
            }
        }
    }
}
