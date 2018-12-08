/* Copyright (C) 1997-2009,2011  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using NCDK.Graphs;
using NCDK.IO;
using NCDK.Numerics;
using NCDK.RingSearches;
using NCDK.Templates;
using NCDK.Tools.Diff;
using System.Collections;

namespace NCDK.Fingerprints
{
    // @cdk.module test-fingerprint
    [TestClass()]
    public class ExtendedFingerprinterTest 
        : AbstractFixedLengthFingerprinterTest
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;

        public override IFingerprinter GetBitFingerprinter()
        {
            return new ExtendedFingerprinter();
        }

        [TestMethod()]
        public void TestExtendedFingerprinter()
        {
            IFingerprinter fingerprinter = new ExtendedFingerprinter();
            Assert.IsNotNull(fingerprinter);
        }

        [TestMethod()]
        public void TestgetBitFingerprint_IAtomContainer()
        {
            IFingerprinter fingerprinter = new ExtendedFingerprinter();
            Assert.IsNotNull(fingerprinter);

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs));
        }

        [TestMethod()]
        public void TestgetBitFingerprintIAtomContainerIRingSetList()
        {
            var fingerprinter = new ExtendedFingerprinter();
            Assert.IsNotNull(fingerprinter);

            var mol = TestMoleculeFactory.MakeIndole();
            var rs = Cycles.FindSSSR(mol).ToRingSet();
            var rslist = RingPartitioner.PartitionRings(rs);
            var bs = fingerprinter.GetBitFingerprint(mol, rs, rslist).AsBitSet();
            var frag1 = TestMoleculeFactory.MakePyrrole();
            var bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs));
        }

        [TestMethod()]
        public void TestGetSize()
        {
            IFingerprinter fingerprinter = new ExtendedFingerprinter(512);
            Assert.IsNotNull(fingerprinter);
            Assert.AreEqual(512, fingerprinter.Length);
        }

        [TestMethod()]
        public void TestExtendedFingerprinter_int()
        {
            IFingerprinter fingerprinter = new ExtendedFingerprinter(512);
            Assert.IsNotNull(fingerprinter);

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs));
        }

        [TestMethod()]
        public void TestExtendedFingerprinter_int_int()
        {
            IFingerprinter fingerprinter = new ExtendedFingerprinter(512, 7);
            Assert.IsNotNull(fingerprinter);

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs));
        }

        /// <summary>
        /// this test only works with allringsfinder in fingerprinter shk3 2008-8-7:
        /// With the change of the extended fingerprinter in r11932, this works by
        /// default
        /// </summary>
        [TestMethod()]
        public void TestDifferentRingFinders()
        {
            var fingerprinter = new ExtendedFingerprinter();
            var ac1 = builder.NewAtomContainer();
            var atom1 = builder.NewAtom("C");
            var atom2 = builder.NewAtom("C");
            var atom3 = builder.NewAtom("C");
            var atom4 = builder.NewAtom("C");
            var atom5 = builder.NewAtom("C");
            var atom6 = builder.NewAtom("C");
            ac1.Atoms.Add(atom1);
            ac1.Atoms.Add(atom2);
            ac1.Atoms.Add(atom3);
            ac1.Atoms.Add(atom4);
            ac1.Atoms.Add(atom5);
            ac1.Atoms.Add(atom6);
            var bond1 = builder.NewBond(atom1, atom2);
            var bond2 = builder.NewBond(atom2, atom3);
            var bond3 = builder.NewBond(atom3, atom4);
            var bond4 = builder.NewBond(atom4, atom5);
            var bond5 = builder.NewBond(atom5, atom6);
            var bond6 = builder.NewBond(atom6, atom1);
            ac1.Bonds.Add(bond1);
            ac1.Bonds.Add(bond2);
            ac1.Bonds.Add(bond3);
            ac1.Bonds.Add(bond4);
            ac1.Bonds.Add(bond5);
            ac1.Bonds.Add(bond6);
            var ac2 = builder.NewAtomContainer();
            ac2.Atoms.Add(atom1);
            ac2.Atoms.Add(atom2);
            ac2.Atoms.Add(atom3);
            ac2.Atoms.Add(atom4);
            ac2.Atoms.Add(atom5);
            ac2.Atoms.Add(atom6);
            var bond7 = builder.NewBond(atom3, atom1);
            ac2.Bonds.Add(bond1);
            ac2.Bonds.Add(bond2);
            ac2.Bonds.Add(bond3);
            ac2.Bonds.Add(bond4);
            ac2.Bonds.Add(bond5);
            ac2.Bonds.Add(bond6);
            ac2.Bonds.Add(bond7);
            var bs = fingerprinter.GetBitFingerprint(ac1).AsBitSet();
            var bs1 = fingerprinter.GetBitFingerprint(ac2).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs1, bs));
            Assert.IsFalse(FingerprinterTool.IsSubset(bs, bs1));
        }

        /// <summary>
        /// this tests if a system with three single rings is not found (it should
        /// not) if looking for a system with three condensed rings using the
        /// fingerprint
        /// </summary>
        [TestMethod()]
        public void TestCondensedSingle()
        {
            var molcondensed = builder.NewAtomContainer();
            var a1 = molcondensed.Builder.NewAtom("C");
            a1.Point2D = new Vector2(421.99999999999994, 860.0);
            molcondensed.Atoms.Add(a1);
            var a2 = molcondensed.Builder.NewAtom("C");
            a2.Point2D = new Vector2(390.8230854637602, 878.0);
            molcondensed.Atoms.Add(a2);
            var a3 = molcondensed.Builder.NewAtom("C");
            a3.Point2D = new Vector2(390.8230854637602, 914.0);
            molcondensed.Atoms.Add(a3);
            var a4 = molcondensed.Builder.NewAtom("C");
            a4.Point2D = new Vector2(422.0, 932.0);
            molcondensed.Atoms.Add(a4);
            var a5 = molcondensed.Builder.NewAtom("C");
            a5.Point2D = new Vector2(453.1769145362398, 914.0);
            molcondensed.Atoms.Add(a5);
            var a6 = molcondensed.Builder.NewAtom("C");
            a6.Point2D = new Vector2(453.1769145362398, 878.0);
            molcondensed.Atoms.Add(a6);
            var a7 = molcondensed.Builder.NewAtom("C");
            a7.Point2D = new Vector2(484.3538290724796, 860.0);
            molcondensed.Atoms.Add(a7);
            var a8 = molcondensed.Builder.NewAtom("C");
            a8.Point2D = new Vector2(515.5307436087194, 878.0);
            molcondensed.Atoms.Add(a8);
            var a9 = molcondensed.Builder.NewAtom("C");
            a9.Point2D = new Vector2(515.5307436087194, 914.0);
            molcondensed.Atoms.Add(a9);
            var a10 = molcondensed.Builder.NewAtom("C");
            a10.Point2D = new Vector2(484.3538290724796, 932.0);
            molcondensed.Atoms.Add(a10);
            var a11 = molcondensed.Builder.NewAtom("C");
            a11.Point2D = new Vector2(546.7076581449592, 932.0);
            molcondensed.Atoms.Add(a11);
            var a12 = molcondensed.Builder.NewAtom("C");
            a12.Point2D = new Vector2(577.884572681199, 914.0);
            molcondensed.Atoms.Add(a12);
            var a13 = molcondensed.Builder.NewAtom("C");
            a13.Point2D = new Vector2(577.884572681199, 878.0);
            molcondensed.Atoms.Add(a13);
            var a14 = molcondensed.Builder.NewAtom("C");
            a14.Point2D = new Vector2(546.7076581449592, 860.0);
            molcondensed.Atoms.Add(a14);
            var a15 = molcondensed.Builder.NewAtom("C");
            a15.Point2D = new Vector2(359.6461709275204, 860.0);
            molcondensed.Atoms.Add(a15);
            var a16 = molcondensed.Builder.NewAtom("C");
            a16.Point2D = new Vector2(609.0614872174388, 860.0);
            molcondensed.Atoms.Add(a16);
            var b1 = molcondensed.Builder.NewBond(a1, a2, BondOrder.Single);
            molcondensed.Bonds.Add(b1);
            var b2 = molcondensed.Builder.NewBond(a2, a3, BondOrder.Single);
            molcondensed.Bonds.Add(b2);
            var b3 = molcondensed.Builder.NewBond(a3, a4, BondOrder.Single);
            molcondensed.Bonds.Add(b3);
            var b4 = molcondensed.Builder.NewBond(a4, a5, BondOrder.Single);
            molcondensed.Bonds.Add(b4);
            var b5 = molcondensed.Builder.NewBond(a5, a6, BondOrder.Single);
            molcondensed.Bonds.Add(b5);
            var b6 = molcondensed.Builder.NewBond(a6, a1, BondOrder.Single);
            molcondensed.Bonds.Add(b6);
            var b7 = molcondensed.Builder.NewBond(a6, a7, BondOrder.Single);
            molcondensed.Bonds.Add(b7);
            var b8 = molcondensed.Builder.NewBond(a7, a8, BondOrder.Single);
            molcondensed.Bonds.Add(b8);
            var b9 = molcondensed.Builder.NewBond(a8, a9, BondOrder.Single);
            molcondensed.Bonds.Add(b9);
            var b10 = molcondensed.Builder.NewBond(a9, a10, BondOrder.Single);
            molcondensed.Bonds.Add(b10);
            var b11 = molcondensed.Builder.NewBond(a10, a5, BondOrder.Single);
            molcondensed.Bonds.Add(b11);
            var b12 = molcondensed.Builder.NewBond(a9, a11, BondOrder.Single);
            molcondensed.Bonds.Add(b12);
            var b13 = molcondensed.Builder.NewBond(a11, a12, BondOrder.Single);
            molcondensed.Bonds.Add(b13);
            var b14 = molcondensed.Builder.NewBond(a12, a13, BondOrder.Single);
            molcondensed.Bonds.Add(b14);
            var b15 = molcondensed.Builder.NewBond(a13, a14, BondOrder.Single);
            molcondensed.Bonds.Add(b15);
            var b16 = molcondensed.Builder.NewBond(a14, a8, BondOrder.Single);
            molcondensed.Bonds.Add(b16);
            var b17 = molcondensed.Builder.NewBond(a2, a15, BondOrder.Single);
            molcondensed.Bonds.Add(b17);
            var b18 = molcondensed.Builder.NewBond(a13, a16, BondOrder.Single);
            molcondensed.Bonds.Add(b18);

            var molsingle = builder.NewAtomContainer();
            var a1s = molsingle.Builder.NewAtom("C");
            a1s.Point2D = new Vector2(421.99999999999994, 860.0);
            molsingle.Atoms.Add(a1s);
            var a2s = molsingle.Builder.NewAtom("C");
            a2s.Point2D = new Vector2(390.8230854637602, 878.0);
            molsingle.Atoms.Add(a2s);
            var a6s = molsingle.Builder.NewAtom("C");
            a6s.Point2D = new Vector2(453.1769145362398, 878.0);
            molsingle.Atoms.Add(a6s);
            var a3s = molsingle.Builder.NewAtom("C");
            a3s.Point2D = new Vector2(390.8230854637602, 914.0);
            molsingle.Atoms.Add(a3s);
            var a15s = molsingle.Builder.NewAtom("C");
            a15s.Point2D = new Vector2(359.6461709275204, 860.0);
            molsingle.Atoms.Add(a15s);
            var a5s = molsingle.Builder.NewAtom("C");
            a5s.Point2D = new Vector2(453.1769145362398, 914.0);
            molsingle.Atoms.Add(a5s);
            var a7s = molsingle.Builder.NewAtom("C");
            a7s.Point2D = new Vector2(492.8230854637602, 881.0);
            molsingle.Atoms.Add(a7s);
            var a4s = molsingle.Builder.NewAtom("C");
            a4s.Point2D = new Vector2(422.0, 932.0);
            molsingle.Atoms.Add(a4s);
            var a8s = molsingle.Builder.NewAtom("C");
            a8s.Point2D = new Vector2(524.0, 863.0);
            molsingle.Atoms.Add(a8s);
            var a9s = molsingle.Builder.NewAtom("C");
            a9s.Point2D = new Vector2(492.8230854637602, 917.0);
            molsingle.Atoms.Add(a9s);
            var a10s = molsingle.Builder.NewAtom("C");
            a10s.Point2D = new Vector2(555.1769145362398, 881.0);
            molsingle.Atoms.Add(a10s);
            var a11s = molsingle.Builder.NewAtom("C");
            a11s.Point2D = new Vector2(524.0, 935.0);
            molsingle.Atoms.Add(a11s);
            var a12s = molsingle.Builder.NewAtom("C");
            a12s.Point2D = new Vector2(555.1769145362398, 917.0);
            molsingle.Atoms.Add(a12s);
            var a13s = molsingle.Builder.NewAtom("C");
            a13s.Point2D = new Vector2(592.8230854637602, 889.0);
            molsingle.Atoms.Add(a13s);
            var a14s = molsingle.Builder.NewAtom("C");
            a14s.Point2D = new Vector2(624.0, 871.0);
            molsingle.Atoms.Add(a14s);
            var a16s = molsingle.Builder.NewAtom("C");
            a16s.Point2D = new Vector2(592.8230854637602, 925.0);
            molsingle.Atoms.Add(a16s);
            var a17s = molsingle.Builder.NewAtom("C");
            a17s.Point2D = new Vector2(655.1769145362398, 889.0);
            molsingle.Atoms.Add(a17s);
            var a18s = molsingle.Builder.NewAtom("C");
            a18s.Point2D = new Vector2(624.0, 943.0);
            molsingle.Atoms.Add(a18s);
            var a19s = molsingle.Builder.NewAtom("C");
            a19s.Point2D = new Vector2(655.1769145362398, 925.0);
            molsingle.Atoms.Add(a19s);
            var a20s = molsingle.Builder.NewAtom("C");
            a20s.Point2D = new Vector2(686.3538290724796, 871.0);
            molsingle.Atoms.Add(a20s);
            var b1s = molsingle.Builder.NewBond(a1s, a2s, BondOrder.Single);
            molsingle.Bonds.Add(b1s);
            var b6s = molsingle.Builder.NewBond(a6s, a1s, BondOrder.Single);
            molsingle.Bonds.Add(b6s);
            var b2s = molsingle.Builder.NewBond(a2s, a3s, BondOrder.Single);
            molsingle.Bonds.Add(b2s);
            var b17s = molsingle.Builder.NewBond(a2s, a15s, BondOrder.Single);
            molsingle.Bonds.Add(b17s);
            var b5s = molsingle.Builder.NewBond(a5s, a6s, BondOrder.Single);
            molsingle.Bonds.Add(b5s);
            var b7s = molsingle.Builder.NewBond(a6s, a7s, BondOrder.Single);
            molsingle.Bonds.Add(b7s);
            var b3s = molsingle.Builder.NewBond(a3s, a4s, BondOrder.Single);
            molsingle.Bonds.Add(b3s);
            var b4s = molsingle.Builder.NewBond(a4s, a5s, BondOrder.Single);
            molsingle.Bonds.Add(b4s);
            var b8s = molsingle.Builder.NewBond(a8s, a7s, BondOrder.Single);
            molsingle.Bonds.Add(b8s);
            var b9s = molsingle.Builder.NewBond(a7s, a9s, BondOrder.Single);
            molsingle.Bonds.Add(b9s);
            var b10s = molsingle.Builder.NewBond(a10s, a8s, BondOrder.Single);
            molsingle.Bonds.Add(b10s);
            var b11s = molsingle.Builder.NewBond(a9s, a11s, BondOrder.Single);
            molsingle.Bonds.Add(b11s);
            var b12s = molsingle.Builder.NewBond(a12s, a10s, BondOrder.Single);
            molsingle.Bonds.Add(b12s);
            var b13s = molsingle.Builder.NewBond(a10s, a13s, BondOrder.Single);
            molsingle.Bonds.Add(b13s);
            var b14s = molsingle.Builder.NewBond(a11s, a12s, BondOrder.Single);
            molsingle.Bonds.Add(b14s);
            var b15s = molsingle.Builder.NewBond(a14s, a13s, BondOrder.Single);
            molsingle.Bonds.Add(b15s);
            var b16s = molsingle.Builder.NewBond(a13s, a16s, BondOrder.Single);
            molsingle.Bonds.Add(b16s);
            var b18s = molsingle.Builder.NewBond(a17s, a14s, BondOrder.Single);
            molsingle.Bonds.Add(b18s);
            var b19s = molsingle.Builder.NewBond(a16s, a18s, BondOrder.Single);
            molsingle.Bonds.Add(b19s);
            var b20s = molsingle.Builder.NewBond(a19s, a17s, BondOrder.Single);
            molsingle.Bonds.Add(b20s);
            var b21s = molsingle.Builder.NewBond(a18s, a19s, BondOrder.Single);
            molsingle.Bonds.Add(b21s);
            var b22s = molsingle.Builder.NewBond(a17s, a20s, BondOrder.Single);
            molsingle.Bonds.Add(b22s);

            var fingerprinter = new ExtendedFingerprinter();
            var bs1 = fingerprinter.GetBitFingerprint(molsingle).AsBitSet();
            var bs2 = fingerprinter.GetBitFingerprint(molcondensed).AsBitSet();

            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs2));
            Assert.IsTrue(FingerprinterTool.IsSubset(bs2, bs1));
        }

        /// <summary>
        /// The power of the extended fingerprinter could not distinguish these
        /// before the change in r11932
        /// </summary>
        [TestMethod()]
        public void TestChebi()
        {
            IAtomContainer searchmol = null;
            IAtomContainer findmol = null;
            var filename = "NCDK.Data.MDL.chebisearch.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            searchmol = reader.Read(builder.NewAtomContainer());
            reader.Close();
            filename = "NCDK.Data.MDL.chebifind.mol";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins);
            findmol = reader.Read(builder.NewAtomContainer());
            reader.Close();
            var fingerprinter = new ExtendedFingerprinter();
            var superBS = fingerprinter.GetBitFingerprint(findmol).AsBitSet();
            var subBS = fingerprinter.GetBitFingerprint(searchmol).AsBitSet();
            var isSubset = FingerprinterTool.IsSubset(superBS, subBS);
            var isSubset2 = FingerprinterTool.IsSubset(subBS, superBS);
            Assert.IsFalse(isSubset);
            Assert.IsFalse(isSubset2);
        }

        // @cdk.bug 2219597
        [TestMethod()]
        public void TestMoleculeInvariance()
        {
            var mol = TestMoleculeFactory.MakePyrrole();
            var clone = (IAtomContainer)mol.Clone();

            // should pass since we have not explicitly detected aromaticity
            foreach (var atom in mol.Atoms)
            {
                Assert.IsFalse(atom.IsAromatic);
            }

            var diff1 = AtomContainerDiff.Diff(mol, clone);
            Assert.AreEqual("", diff1);

            var fprinter = new ExtendedFingerprinter();
            var fp = fprinter.GetBitFingerprint(mol).AsBitSet();
            Assert.IsNotNull(fp);

            var diff2 = AtomContainerDiff.Diff(mol, clone);
            Assert.IsTrue(diff2.Length == 0, "There was a difference\n" + diff2);
        }
    }
}
