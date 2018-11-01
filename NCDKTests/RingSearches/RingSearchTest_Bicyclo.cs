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
using NCDK.Templates;
using System.Linq;

namespace NCDK.RingSearches
{
    /// <summary>
    /// Unit tests for ring search. These unit tests ensure bicyclo rings (a bridged
    /// system) is found correctly.
    /// </summary>
    // @author John May
    // @cdk.module test-standard
    [TestClass()]
    public sealed class RingSearchTest_Bicyclo
    {
        private static readonly IAtomContainer bicyclo = TestMoleculeFactory.MakeBicycloRings();

        [TestMethod()]
        public void TestCyclic()
        {
            var n = bicyclo.Atoms.Count;
            Assert.AreEqual(n, new RingSearch(bicyclo).Cyclic().Length, "cyclic vertices should be invariant for any ordering");
        }

        [TestMethod()]
        public void TestCyclic_Int()
        {
            var n = bicyclo.Atoms.Count;
            var ringSearch = new RingSearch(bicyclo);
            for (int i = 0; i < n; i++)
                Assert.IsTrue(ringSearch.Cyclic(i), "all atoms should be cyclic");
        }

        [TestMethod()]
        public void TestIsolated()
        {
            Assert.AreEqual(0, new RingSearch(bicyclo).Isolated().Length, "no isolated cycle should be found");
        }

        [TestMethod()]
        public void TestFused()
        {
            Assert.AreEqual(1, new RingSearch(bicyclo).Fused().Length, "one fused cycle should be found");
        }

        [TestMethod()]
        public void TestRingFragments()
        {
            var n = bicyclo.Atoms.Count;
            var fragment = new RingSearch(bicyclo).RingFragments();
            Assert.AreEqual(bicyclo.Atoms.Count, fragment.Atoms.Count);
            Assert.AreEqual(bicyclo.Bonds.Count, fragment.Bonds.Count);
        }

        [TestMethod()]
        public void TestIsolatedRingFragments()
        {
            var n = bicyclo.Atoms.Count;
            var fragments = new RingSearch(bicyclo).IsolatedRingFragments();
            Assert.IsTrue(fragments.Count() == 0);
        }

        [TestMethod()]
        public void TestFusedRingFragments()
        {
            var fragments = new RingSearch(bicyclo).FusedRingFragments().ToReadOnlyList();
            Assert.AreEqual(1, fragments.Count);
            IAtomContainer fragment = fragments[0];
            Assert.AreEqual(bicyclo.Atoms.Count, fragment.Atoms.Count);
            Assert.AreEqual(bicyclo.Bonds.Count, fragment.Bonds.Count);
        }
    }
}
