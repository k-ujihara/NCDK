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
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Graphs;
using NCDK.IO;
using NCDK.RingSearches;
using NCDK.Templates;
using NCDK.Tools.Diff;
using System.Collections;
using System.Collections.Generic;

namespace NCDK.Fingerprint
{
    /**
     * @cdk.module test-fingerprint
     */
    [TestClass()]
    public class ExtendedFingerprinterTest : AbstractFixedLengthFingerprinterTest
    {
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
        public void TestgetBitFingerprint_IAtomContainer_IRingSet_List()
        {
            ExtendedFingerprinter fingerprinter = new ExtendedFingerprinter();
            Assert.IsNotNull(fingerprinter);

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            IRingSet rs = Cycles.SSSR(mol).ToRingSet();
            IList<IRingSet> rslist = RingPartitioner.PartitionRings(rs);
            BitArray bs = fingerprinter.GetBitFingerprint(mol, rs, rslist).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs));
        }

        [TestMethod()]
        public void TestGetSize()
        {
            IFingerprinter fingerprinter = new ExtendedFingerprinter(512);
            Assert.IsNotNull(fingerprinter);
            Assert.AreEqual(512, fingerprinter.Count);
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

        /*
         * this test only works with allringsfinder in fingerprinter shk3 2008-8-7:
         * With the change of the extended fingerprinter in r11932, this works by
         * default
         */
        [TestMethod()]
        public void TestDifferentRingFinders()
        {
            IFingerprinter fingerprinter = new ExtendedFingerprinter();
            IAtomContainer ac1 = new AtomContainer();
            Atom atom1 = new Atom("C");
            Atom atom2 = new Atom("C");
            Atom atom3 = new Atom("C");
            Atom atom4 = new Atom("C");
            Atom atom5 = new Atom("C");
            Atom atom6 = new Atom("C");
            ac1.Atoms.Add(atom1);
            ac1.Atoms.Add(atom2);
            ac1.Atoms.Add(atom3);
            ac1.Atoms.Add(atom4);
            ac1.Atoms.Add(atom5);
            ac1.Atoms.Add(atom6);
            Bond bond1 = new Bond(atom1, atom2);
            Bond bond2 = new Bond(atom2, atom3);
            Bond bond3 = new Bond(atom3, atom4);
            Bond bond4 = new Bond(atom4, atom5);
            Bond bond5 = new Bond(atom5, atom6);
            Bond bond6 = new Bond(atom6, atom1);
            ac1.Bonds.Add(bond1);
            ac1.Bonds.Add(bond2);
            ac1.Bonds.Add(bond3);
            ac1.Bonds.Add(bond4);
            ac1.Bonds.Add(bond5);
            ac1.Bonds.Add(bond6);
            IAtomContainer ac2 = new AtomContainer();
            ac2.Atoms.Add(atom1);
            ac2.Atoms.Add(atom2);
            ac2.Atoms.Add(atom3);
            ac2.Atoms.Add(atom4);
            ac2.Atoms.Add(atom5);
            ac2.Atoms.Add(atom6);
            Bond bond7 = new Bond(atom3, atom1);
            ac2.Bonds.Add(bond1);
            ac2.Bonds.Add(bond2);
            ac2.Bonds.Add(bond3);
            ac2.Bonds.Add(bond4);
            ac2.Bonds.Add(bond5);
            ac2.Bonds.Add(bond6);
            ac2.Bonds.Add(bond7);
            BitArray bs = fingerprinter.GetBitFingerprint(ac1).AsBitSet();
            BitArray bs1 = fingerprinter.GetBitFingerprint(ac2).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs1, bs));
            Assert.IsFalse(FingerprinterTool.IsSubset(bs, bs1));
        }

        /*
         * this tests if a system with three single rings is not found (it should
         * not) if looking for a system with three condensed rings using the
         * fingerprint
         */
        [TestMethod()]
        public void TestCondensedSingle()
        {
            IAtomContainer molcondensed = new AtomContainer();
            IAtom a1 = molcondensed.Builder.CreateAtom("C");
            a1.Point2D = new Vector2(421.99999999999994, 860.0);
            molcondensed.Atoms.Add(a1);
            IAtom a2 = molcondensed.Builder.CreateAtom("C");
            a2.Point2D = new Vector2(390.8230854637602, 878.0);
            molcondensed.Atoms.Add(a2);
            IAtom a3 = molcondensed.Builder.CreateAtom("C");
            a3.Point2D = new Vector2(390.8230854637602, 914.0);
            molcondensed.Atoms.Add(a3);
            IAtom a4 = molcondensed.Builder.CreateAtom("C");
            a4.Point2D = new Vector2(422.0, 932.0);
            molcondensed.Atoms.Add(a4);
            IAtom a5 = molcondensed.Builder.CreateAtom("C");
            a5.Point2D = new Vector2(453.1769145362398, 914.0);
            molcondensed.Atoms.Add(a5);
            IAtom a6 = molcondensed.Builder.CreateAtom("C");
            a6.Point2D = new Vector2(453.1769145362398, 878.0);
            molcondensed.Atoms.Add(a6);
            IAtom a7 = molcondensed.Builder.CreateAtom("C");
            a7.Point2D = new Vector2(484.3538290724796, 860.0);
            molcondensed.Atoms.Add(a7);
            IAtom a8 = molcondensed.Builder.CreateAtom("C");
            a8.Point2D = new Vector2(515.5307436087194, 878.0);
            molcondensed.Atoms.Add(a8);
            IAtom a9 = molcondensed.Builder.CreateAtom("C");
            a9.Point2D = new Vector2(515.5307436087194, 914.0);
            molcondensed.Atoms.Add(a9);
            IAtom a10 = molcondensed.Builder.CreateAtom("C");
            a10.Point2D = new Vector2(484.3538290724796, 932.0);
            molcondensed.Atoms.Add(a10);
            IAtom a11 = molcondensed.Builder.CreateAtom("C");
            a11.Point2D = new Vector2(546.7076581449592, 932.0);
            molcondensed.Atoms.Add(a11);
            IAtom a12 = molcondensed.Builder.CreateAtom("C");
            a12.Point2D = new Vector2(577.884572681199, 914.0);
            molcondensed.Atoms.Add(a12);
            IAtom a13 = molcondensed.Builder.CreateAtom("C");
            a13.Point2D = new Vector2(577.884572681199, 878.0);
            molcondensed.Atoms.Add(a13);
            IAtom a14 = molcondensed.Builder.CreateAtom("C");
            a14.Point2D = new Vector2(546.7076581449592, 860.0);
            molcondensed.Atoms.Add(a14);
            IAtom a15 = molcondensed.Builder.CreateAtom("C");
            a15.Point2D = new Vector2(359.6461709275204, 860.0);
            molcondensed.Atoms.Add(a15);
            IAtom a16 = molcondensed.Builder.CreateAtom("C");
            a16.Point2D = new Vector2(609.0614872174388, 860.0);
            molcondensed.Atoms.Add(a16);
            IBond b1 = molcondensed.Builder.CreateBond(a1, a2, BondOrder.Single);
            molcondensed.Bonds.Add(b1);
            IBond b2 = molcondensed.Builder.CreateBond(a2, a3, BondOrder.Single);
            molcondensed.Bonds.Add(b2);
            IBond b3 = molcondensed.Builder.CreateBond(a3, a4, BondOrder.Single);
            molcondensed.Bonds.Add(b3);
            IBond b4 = molcondensed.Builder.CreateBond(a4, a5, BondOrder.Single);
            molcondensed.Bonds.Add(b4);
            IBond b5 = molcondensed.Builder.CreateBond(a5, a6, BondOrder.Single);
            molcondensed.Bonds.Add(b5);
            IBond b6 = molcondensed.Builder.CreateBond(a6, a1, BondOrder.Single);
            molcondensed.Bonds.Add(b6);
            IBond b7 = molcondensed.Builder.CreateBond(a6, a7, BondOrder.Single);
            molcondensed.Bonds.Add(b7);
            IBond b8 = molcondensed.Builder.CreateBond(a7, a8, BondOrder.Single);
            molcondensed.Bonds.Add(b8);
            IBond b9 = molcondensed.Builder.CreateBond(a8, a9, BondOrder.Single);
            molcondensed.Bonds.Add(b9);
            IBond b10 = molcondensed.Builder.CreateBond(a9, a10, BondOrder.Single);
            molcondensed.Bonds.Add(b10);
            IBond b11 = molcondensed.Builder.CreateBond(a10, a5, BondOrder.Single);
            molcondensed.Bonds.Add(b11);
            IBond b12 = molcondensed.Builder.CreateBond(a9, a11, BondOrder.Single);
            molcondensed.Bonds.Add(b12);
            IBond b13 = molcondensed.Builder.CreateBond(a11, a12, BondOrder.Single);
            molcondensed.Bonds.Add(b13);
            IBond b14 = molcondensed.Builder.CreateBond(a12, a13, BondOrder.Single);
            molcondensed.Bonds.Add(b14);
            IBond b15 = molcondensed.Builder.CreateBond(a13, a14, BondOrder.Single);
            molcondensed.Bonds.Add(b15);
            IBond b16 = molcondensed.Builder.CreateBond(a14, a8, BondOrder.Single);
            molcondensed.Bonds.Add(b16);
            IBond b17 = molcondensed.Builder.CreateBond(a2, a15, BondOrder.Single);
            molcondensed.Bonds.Add(b17);
            IBond b18 = molcondensed.Builder.CreateBond(a13, a16, BondOrder.Single);
            molcondensed.Bonds.Add(b18);

            IAtomContainer molsingle = new AtomContainer();
            IAtom a1s = molsingle.Builder.CreateAtom("C");
            a1s.Point2D = new Vector2(421.99999999999994, 860.0);
            molsingle.Atoms.Add(a1s);
            IAtom a2s = molsingle.Builder.CreateAtom("C");
            a2s.Point2D = new Vector2(390.8230854637602, 878.0);
            molsingle.Atoms.Add(a2s);
            IAtom a6s = molsingle.Builder.CreateAtom("C");
            a6s.Point2D = new Vector2(453.1769145362398, 878.0);
            molsingle.Atoms.Add(a6s);
            IAtom a3s = molsingle.Builder.CreateAtom("C");
            a3s.Point2D = new Vector2(390.8230854637602, 914.0);
            molsingle.Atoms.Add(a3s);
            IAtom a15s = molsingle.Builder.CreateAtom("C");
            a15s.Point2D = new Vector2(359.6461709275204, 860.0);
            molsingle.Atoms.Add(a15s);
            IAtom a5s = molsingle.Builder.CreateAtom("C");
            a5s.Point2D = new Vector2(453.1769145362398, 914.0);
            molsingle.Atoms.Add(a5s);
            IAtom a7s = molsingle.Builder.CreateAtom("C");
            a7s.Point2D = new Vector2(492.8230854637602, 881.0);
            molsingle.Atoms.Add(a7s);
            IAtom a4s = molsingle.Builder.CreateAtom("C");
            a4s.Point2D = new Vector2(422.0, 932.0);
            molsingle.Atoms.Add(a4s);
            IAtom a8s = molsingle.Builder.CreateAtom("C");
            a8s.Point2D = new Vector2(524.0, 863.0);
            molsingle.Atoms.Add(a8s);
            IAtom a9s = molsingle.Builder.CreateAtom("C");
            a9s.Point2D = new Vector2(492.8230854637602, 917.0);
            molsingle.Atoms.Add(a9s);
            IAtom a10s = molsingle.Builder.CreateAtom("C");
            a10s.Point2D = new Vector2(555.1769145362398, 881.0);
            molsingle.Atoms.Add(a10s);
            IAtom a11s = molsingle.Builder.CreateAtom("C");
            a11s.Point2D = new Vector2(524.0, 935.0);
            molsingle.Atoms.Add(a11s);
            IAtom a12s = molsingle.Builder.CreateAtom("C");
            a12s.Point2D = new Vector2(555.1769145362398, 917.0);
            molsingle.Atoms.Add(a12s);
            IAtom a13s = molsingle.Builder.CreateAtom("C");
            a13s.Point2D = new Vector2(592.8230854637602, 889.0);
            molsingle.Atoms.Add(a13s);
            IAtom a14s = molsingle.Builder.CreateAtom("C");
            a14s.Point2D = new Vector2(624.0, 871.0);
            molsingle.Atoms.Add(a14s);
            IAtom a16s = molsingle.Builder.CreateAtom("C");
            a16s.Point2D = new Vector2(592.8230854637602, 925.0);
            molsingle.Atoms.Add(a16s);
            IAtom a17s = molsingle.Builder.CreateAtom("C");
            a17s.Point2D = new Vector2(655.1769145362398, 889.0);
            molsingle.Atoms.Add(a17s);
            IAtom a18s = molsingle.Builder.CreateAtom("C");
            a18s.Point2D = new Vector2(624.0, 943.0);
            molsingle.Atoms.Add(a18s);
            IAtom a19s = molsingle.Builder.CreateAtom("C");
            a19s.Point2D = new Vector2(655.1769145362398, 925.0);
            molsingle.Atoms.Add(a19s);
            IAtom a20s = molsingle.Builder.CreateAtom("C");
            a20s.Point2D = new Vector2(686.3538290724796, 871.0);
            molsingle.Atoms.Add(a20s);
            IBond b1s = molsingle.Builder.CreateBond(a1s, a2s, BondOrder.Single);
            molsingle.Bonds.Add(b1s);
            IBond b6s = molsingle.Builder.CreateBond(a6s, a1s, BondOrder.Single);
            molsingle.Bonds.Add(b6s);
            IBond b2s = molsingle.Builder.CreateBond(a2s, a3s, BondOrder.Single);
            molsingle.Bonds.Add(b2s);
            IBond b17s = molsingle.Builder.CreateBond(a2s, a15s, BondOrder.Single);
            molsingle.Bonds.Add(b17s);
            IBond b5s = molsingle.Builder.CreateBond(a5s, a6s, BondOrder.Single);
            molsingle.Bonds.Add(b5s);
            IBond b7s = molsingle.Builder.CreateBond(a6s, a7s, BondOrder.Single);
            molsingle.Bonds.Add(b7s);
            IBond b3s = molsingle.Builder.CreateBond(a3s, a4s, BondOrder.Single);
            molsingle.Bonds.Add(b3s);
            IBond b4s = molsingle.Builder.CreateBond(a4s, a5s, BondOrder.Single);
            molsingle.Bonds.Add(b4s);
            IBond b8s = molsingle.Builder.CreateBond(a8s, a7s, BondOrder.Single);
            molsingle.Bonds.Add(b8s);
            IBond b9s = molsingle.Builder.CreateBond(a7s, a9s, BondOrder.Single);
            molsingle.Bonds.Add(b9s);
            IBond b10s = molsingle.Builder.CreateBond(a10s, a8s, BondOrder.Single);
            molsingle.Bonds.Add(b10s);
            IBond b11s = molsingle.Builder.CreateBond(a9s, a11s, BondOrder.Single);
            molsingle.Bonds.Add(b11s);
            IBond b12s = molsingle.Builder.CreateBond(a12s, a10s, BondOrder.Single);
            molsingle.Bonds.Add(b12s);
            IBond b13s = molsingle.Builder.CreateBond(a10s, a13s, BondOrder.Single);
            molsingle.Bonds.Add(b13s);
            IBond b14s = molsingle.Builder.CreateBond(a11s, a12s, BondOrder.Single);
            molsingle.Bonds.Add(b14s);
            IBond b15s = molsingle.Builder.CreateBond(a14s, a13s, BondOrder.Single);
            molsingle.Bonds.Add(b15s);
            IBond b16s = molsingle.Builder.CreateBond(a13s, a16s, BondOrder.Single);
            molsingle.Bonds.Add(b16s);
            IBond b18s = molsingle.Builder.CreateBond(a17s, a14s, BondOrder.Single);
            molsingle.Bonds.Add(b18s);
            IBond b19s = molsingle.Builder.CreateBond(a16s, a18s, BondOrder.Single);
            molsingle.Bonds.Add(b19s);
            IBond b20s = molsingle.Builder.CreateBond(a19s, a17s, BondOrder.Single);
            molsingle.Bonds.Add(b20s);
            IBond b21s = molsingle.Builder.CreateBond(a18s, a19s, BondOrder.Single);
            molsingle.Bonds.Add(b21s);
            IBond b22s = molsingle.Builder.CreateBond(a17s, a20s, BondOrder.Single);
            molsingle.Bonds.Add(b22s);

            IFingerprinter fingerprinter = new ExtendedFingerprinter();
            BitArray bs1 = fingerprinter.GetBitFingerprint(molsingle).AsBitSet();
            BitArray bs2 = fingerprinter.GetBitFingerprint(molcondensed).AsBitSet();

            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs2));
            Assert.IsTrue(FingerprinterTool.IsSubset(bs2, bs1));

        }

        /*
         * The power of the extended fingerprinter could not distinguish these
         * before the change in r11932
         */
        [TestMethod()]
        public void TestChebi()
        {
            IAtomContainer searchmol = null;
            IAtomContainer findmol = null;
            string filename = "NCDK.Data.MDL.chebisearch.mol";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            searchmol = reader.Read(new AtomContainer());
            reader.Close();
            filename = "NCDK.Data.MDL.chebifind.mol";
            ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            reader = new MDLV2000Reader(ins);
            findmol = reader.Read(new AtomContainer());
            reader.Close();
            IFingerprinter fingerprinter = new ExtendedFingerprinter();
            BitArray superBS = fingerprinter.GetBitFingerprint(findmol).AsBitSet();
            BitArray subBS = fingerprinter.GetBitFingerprint(searchmol).AsBitSet();
            bool isSubset = FingerprinterTool.IsSubset(superBS, subBS);
            bool isSubset2 = FingerprinterTool.IsSubset(subBS, superBS);
            Assert.IsFalse(isSubset);
            Assert.IsFalse(isSubset2);
        }

        /**
         * @cdk.bug 2219597
         * @
         * @throws CloneNotSupportedException
         */
        [TestMethod()]
        public void TestMoleculeInvariance()
        {
            IAtomContainer mol = TestMoleculeFactory.MakePyrrole();
            IAtomContainer clone = (IAtomContainer)mol.Clone();

            // should pass since we have not explicitly detected aromaticity
            foreach (var atom in mol.Atoms)
            {
                Assert.IsFalse(atom.IsAromatic);
            }

            string diff1 = AtomContainerDiff.Diff(mol, clone);
            Assert.AreEqual("", diff1);

            ExtendedFingerprinter fprinter = new ExtendedFingerprinter();
            BitArray fp = fprinter.GetBitFingerprint(mol).AsBitSet();
            Assert.IsNotNull(fp);

            string diff2 = AtomContainerDiff.Diff(mol, clone);
            Assert.IsTrue(diff2.Equals(""), "There was a difference\n" + diff2);
        }
    }
}
