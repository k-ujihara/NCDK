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

namespace NCDK.RingSearches
{
    /// <summary>
    /// Unit tests for ring search. These unit tests ensure bicyclo rings (a bridged
    /// system) is found correctly.
    ///
    // @author John May
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public sealed class RingSearchTest_Bicyclo
    {

        private static readonly IAtomContainer bicyclo = TestMoleculeFactory.MakeBicycloRings();

        [TestMethod()]
        public void TestCyclic()
        {
            int n = bicyclo.Atoms.Count;
            Assert.AreEqual(n, new RingSearch(bicyclo).Cyclic().Length, "cyclic vertices should be invariant for any ordering");
        }

        [TestMethod()]
        public void TestCyclic_Int()
        {
            int n = bicyclo.Atoms.Count;

            RingSearch ringSearch = new RingSearch(bicyclo);
            for (int i = 0; i < n; i++)
                Assert.IsTrue(ringSearch.Cyclic(i), "all atoms should be cyclic");

        }

        [TestMethod()]
        public void TestIsolated()
        {
            Assert.AreEqual(0, new RingSearch(bicyclo).Isolated().Length, "no isolated cycle should be found");

        }

        [TestMethod()]
        public void TestFUsed()
        {
            Assert.AreEqual(1, new RingSearch(bicyclo).Fused().Length, "one fused cycle should be found");

        }

        [TestMethod()]
        public void TestRingFragments()
        {
            int n = bicyclo.Atoms.Count;

            IAtomContainer fragment = new RingSearch(bicyclo).RingFragments();
            Assert.AreEqual(bicyclo.Atoms.Count, fragment.Atoms.Count);
            Assert.AreEqual(bicyclo.Bonds.Count, fragment.Bonds.Count);

        }

        [TestMethod()]
        public void TestIsolatedRingFragments()
        {
            int n = bicyclo.Atoms.Count;

            IList<IAtomContainer> fragments = new RingSearch(bicyclo).IsolatedRingFragments();
            Assert.IsTrue(fragments.Count == 0);

        }

        [TestMethod()]
        public void TestFUsedRingFragments()
        {

            IList<IAtomContainer> fragments = new RingSearch(bicyclo).FusedRingFragments();
            Assert.AreEqual(1, fragments.Count);
            IAtomContainer fragment = fragments[0];
            Assert.AreEqual(bicyclo.Atoms.Count, fragment.Atoms.Count);
            Assert.AreEqual(bicyclo.Bonds.Count, fragment.Bonds.Count);

        }
    }
}
