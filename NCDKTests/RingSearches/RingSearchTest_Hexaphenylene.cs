/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by the
 * Free Software Foundation; either version 2.1 of the License, or (at your
 * option) any later version. All we ask is that proper credit is given for our
 * work, which includes - but is not limited to - adding the above copyright
 * notice to the beginning of your source code files, and to any copyright
 * notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System.Collections.Generic;

namespace NCDK.RingSearches
{
    /// <summary>
    /// ring search unit tests for a hexaphenylene (ChEBI:33157)
    /// </summary>
    // @author John May
    // @cdk.module test-standard
    [TestClass()]
    public sealed class RingSearchTest_Hexaphenylene
    {
        private readonly IAtomContainer hexaphenylene = Hexaphenylene();

        [TestMethod()]
        public void TestCyclic()
        {
            Assert.AreEqual(hexaphenylene.Atoms.Count, new RingSearch(hexaphenylene).Cyclic().Length);
        }

        [TestMethod()]
        public void TestCyclic_Int()
        {
            int n = hexaphenylene.Atoms.Count;
            RingSearch ringSearch = new RingSearch(hexaphenylene);
            for (int i = 0; i < n; i++)
            {
                Assert.IsTrue(ringSearch.Cyclic(i));
            }
        }

        [TestMethod()]
        public void TestIsolated()
        {
            RingSearch search = new RingSearch(hexaphenylene);
            int[][] isolated = search.Isolated();
            Assert.AreEqual(0, isolated.Length);
        }

        [TestMethod()]
        public void TestFused()
        {
            int[][] fused = new RingSearch(hexaphenylene).Fused();
            Assert.AreEqual(1, fused.Length);
            Assert.AreEqual(hexaphenylene.Atoms.Count, fused[0].Length);
        }

        [TestMethod()]
        public void TestRingFragments()
        {
            IAtomContainer fragment = new RingSearch(hexaphenylene).RingFragments();
            Assert.AreEqual(hexaphenylene.Atoms.Count, fragment.Atoms.Count);
            Assert.AreEqual(hexaphenylene.Bonds.Count, fragment.Bonds.Count);
        }

        [TestMethod()]
        public void TestIsolatedRingFragments()
        {
            RingSearch search = new RingSearch(hexaphenylene);
            IList<IAtomContainer> isolated = search.IsolatedRingFragments();
            Assert.AreEqual(0, isolated.Count);
        }

        [TestMethod()]
        public void TestFusedRingFragments()
        {
            RingSearch search = new RingSearch(hexaphenylene);
            IList<IAtomContainer> fused = search.FusedRingFragments();
            Assert.AreEqual(1, fused.Count);
            Assert.AreEqual(hexaphenylene.Atoms.Count, fused[0].Atoms.Count);
            Assert.AreEqual(hexaphenylene.Bonds.Count, fused[0].Bonds.Count);
        }

        // @cdk.inchi InChI=1S/C36H24/c1-2-14-26-25(13-1)27-15-3-4-17-29(27)31-19-7-8-21-33(31)35-23-11-12-24-36(35)34-22-10-9-20-32(34)30-18-6-5-16-28(26)30/h1-24H/b27-25-,28-26-,31-29-,32-30-,35-33-,36-34-
        public static IAtomContainer Hexaphenylene()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = new Atom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = new Atom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = new Atom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = new Atom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = new Atom("C");
            mol.Atoms.Add(a5);
            IAtom a6 = new Atom("C");
            mol.Atoms.Add(a6);
            IAtom a7 = new Atom("C");
            mol.Atoms.Add(a7);
            IAtom a8 = new Atom("C");
            mol.Atoms.Add(a8);
            IAtom a9 = new Atom("C");
            mol.Atoms.Add(a9);
            IAtom a10 = new Atom("C");
            mol.Atoms.Add(a10);
            IAtom a11 = new Atom("C");
            mol.Atoms.Add(a11);
            IAtom a12 = new Atom("C");
            mol.Atoms.Add(a12);
            IAtom a13 = new Atom("C");
            mol.Atoms.Add(a13);
            IAtom a14 = new Atom("C");
            mol.Atoms.Add(a14);
            IAtom a15 = new Atom("C");
            mol.Atoms.Add(a15);
            IAtom a16 = new Atom("C");
            mol.Atoms.Add(a16);
            IAtom a17 = new Atom("C");
            mol.Atoms.Add(a17);
            IAtom a18 = new Atom("C");
            mol.Atoms.Add(a18);
            IAtom a19 = new Atom("C");
            mol.Atoms.Add(a19);
            IAtom a20 = new Atom("C");
            mol.Atoms.Add(a20);
            IAtom a21 = new Atom("C");
            mol.Atoms.Add(a21);
            IAtom a22 = new Atom("C");
            mol.Atoms.Add(a22);
            IAtom a23 = new Atom("C");
            mol.Atoms.Add(a23);
            IAtom a24 = new Atom("C");
            mol.Atoms.Add(a24);
            IAtom a25 = new Atom("C");
            mol.Atoms.Add(a25);
            IAtom a26 = new Atom("C");
            mol.Atoms.Add(a26);
            IAtom a27 = new Atom("C");
            mol.Atoms.Add(a27);
            IAtom a28 = new Atom("C");
            mol.Atoms.Add(a28);
            IAtom a29 = new Atom("C");
            mol.Atoms.Add(a29);
            IAtom a30 = new Atom("C");
            mol.Atoms.Add(a30);
            IAtom a31 = new Atom("C");
            mol.Atoms.Add(a31);
            IAtom a32 = new Atom("C");
            mol.Atoms.Add(a32);
            IAtom a33 = new Atom("C");
            mol.Atoms.Add(a33);
            IAtom a34 = new Atom("C");
            mol.Atoms.Add(a34);
            IAtom a35 = new Atom("C");
            mol.Atoms.Add(a35);
            IAtom a36 = new Atom("C");
            mol.Atoms.Add(a36);
            IBond b1 = new Bond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = new Bond(a1, a6, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = new Bond(a2, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = new Bond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = new Bond(a4, a5, BondOrder.Double);
            mol.Bonds.Add(b5);
            IBond b6 = new Bond(a4, a36, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = new Bond(a5, a6, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = new Bond(a5, a7, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = new Bond(a7, a8, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = new Bond(a7, a12, BondOrder.Double);
            mol.Bonds.Add(b10);
            IBond b11 = new Bond(a8, a9, BondOrder.Double);
            mol.Bonds.Add(b11);
            IBond b12 = new Bond(a9, a10, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = new Bond(a10, a11, BondOrder.Double);
            mol.Bonds.Add(b13);
            IBond b14 = new Bond(a11, a12, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = new Bond(a12, a13, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = new Bond(a13, a14, BondOrder.Single);
            mol.Bonds.Add(b16);
            IBond b17 = new Bond(a13, a18, BondOrder.Double);
            mol.Bonds.Add(b17);
            IBond b18 = new Bond(a14, a15, BondOrder.Double);
            mol.Bonds.Add(b18);
            IBond b19 = new Bond(a15, a16, BondOrder.Single);
            mol.Bonds.Add(b19);
            IBond b20 = new Bond(a16, a17, BondOrder.Double);
            mol.Bonds.Add(b20);
            IBond b21 = new Bond(a17, a18, BondOrder.Single);
            mol.Bonds.Add(b21);
            IBond b22 = new Bond(a18, a19, BondOrder.Single);
            mol.Bonds.Add(b22);
            IBond b23 = new Bond(a19, a20, BondOrder.Single);
            mol.Bonds.Add(b23);
            IBond b24 = new Bond(a19, a24, BondOrder.Double);
            mol.Bonds.Add(b24);
            IBond b25 = new Bond(a20, a21, BondOrder.Double);
            mol.Bonds.Add(b25);
            IBond b26 = new Bond(a21, a22, BondOrder.Single);
            mol.Bonds.Add(b26);
            IBond b27 = new Bond(a22, a23, BondOrder.Double);
            mol.Bonds.Add(b27);
            IBond b28 = new Bond(a23, a24, BondOrder.Single);
            mol.Bonds.Add(b28);
            IBond b29 = new Bond(a24, a25, BondOrder.Single);
            mol.Bonds.Add(b29);
            IBond b30 = new Bond(a25, a26, BondOrder.Single);
            mol.Bonds.Add(b30);
            IBond b31 = new Bond(a25, a30, BondOrder.Double);
            mol.Bonds.Add(b31);
            IBond b32 = new Bond(a26, a27, BondOrder.Double);
            mol.Bonds.Add(b32);
            IBond b33 = new Bond(a27, a28, BondOrder.Single);
            mol.Bonds.Add(b33);
            IBond b34 = new Bond(a28, a29, BondOrder.Double);
            mol.Bonds.Add(b34);
            IBond b35 = new Bond(a29, a30, BondOrder.Single);
            mol.Bonds.Add(b35);
            IBond b36 = new Bond(a30, a31, BondOrder.Single);
            mol.Bonds.Add(b36);
            IBond b37 = new Bond(a31, a32, BondOrder.Single);
            mol.Bonds.Add(b37);
            IBond b38 = new Bond(a31, a36, BondOrder.Double);
            mol.Bonds.Add(b38);
            IBond b39 = new Bond(a32, a33, BondOrder.Double);
            mol.Bonds.Add(b39);
            IBond b40 = new Bond(a33, a34, BondOrder.Single);
            mol.Bonds.Add(b40);
            IBond b41 = new Bond(a34, a35, BondOrder.Double);
            mol.Bonds.Add(b41);
            IBond b42 = new Bond(a35, a36, BondOrder.Single);
            mol.Bonds.Add(b42);
            return mol;
        }
    }
}
