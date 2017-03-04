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
using NCDK.Templates;
using System.Collections.Generic;

namespace NCDK.RingSearches {
    /// <summary>
    /// biphenyl ring search unit tests
    ///
    // @author John May
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public sealed class RingSearchTest_Biphenyl {

        private readonly IAtomContainer biphenyl = TestMoleculeFactory.MakeBiphenyl();

    [TestMethod()]
        public void TestCyclic() {
            Assert.AreEqual(biphenyl.Atoms.Count, new RingSearch(biphenyl).Cyclic().Length);
        }

        [TestMethod()]
        public void TestCyclic_Int() {
            int n = biphenyl.Atoms.Count;
            RingSearch ringSearch = new RingSearch(biphenyl);
            for (int i = 0; i < n; i++) {
                Assert.IsTrue(ringSearch.Cyclic(i));
            }
        }

        [TestMethod()]
        public void TestIsolated() {
            RingSearch search = new RingSearch(biphenyl);
            int[][] isolated = search.Isolated();
            Assert.AreEqual(2, isolated.Length);
            Assert.AreEqual(6, isolated[0].Length);
            Assert.AreEqual(6, isolated[1].Length);
        }

        [TestMethod()]
        public void TestFUsed() {
            Assert.AreEqual(0, new RingSearch(biphenyl).Fused().Length);
        }

        [TestMethod()]
        public void TestRingFragments() {
            IAtomContainer fragment = new RingSearch(biphenyl).RingFragments();
            Assert.AreEqual(biphenyl.Atoms.Count, fragment.Atoms.Count);
            Assert.AreEqual(biphenyl.Bonds.Count - 1, fragment.Bonds.Count);
        }

        [TestMethod()]
        public void TestIsolatedRingFragments() {
            RingSearch search = new RingSearch(biphenyl);
            IList<IAtomContainer> isolated = search.IsolatedRingFragments();
            Assert.AreEqual(2, isolated.Count);
            Assert.AreEqual(6, isolated[0].Atoms.Count);
            Assert.AreEqual(6, isolated[0].Bonds.Count);
            Assert.AreEqual(6, isolated[1].Atoms.Count);
            Assert.AreEqual(6, isolated[1].Bonds.Count);
        }

        [TestMethod()]
        public void TestFUsedRingFragments() {
            RingSearch search = new RingSearch(biphenyl);
            IList<IAtomContainer> fused = search.FusedRingFragments();
            Assert.AreEqual(0, fused.Count);
        }
    }
}
