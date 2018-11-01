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
using NCDK.Common.Base;
using NCDK.Templates;
using System.Linq;

namespace NCDK.RingSearches
{
    /// <summary>
    /// ring search unit tests for spiro rings
    /// </summary>
    // @author John May
    // @cdk.module test-standard
    [TestClass()]
    public sealed class RingSearchTest_SpiroRings
    {
        private readonly IAtomContainer spiro = TestMoleculeFactory.MakeSpiroRings();

        [TestMethod()]
        public void TestCyclic()
        {
            Assert.AreEqual(spiro.Atoms.Count, new RingSearch(spiro).Cyclic().Length);
        }

        [TestMethod()]
        public void TestCyclic_Int()
        {
            var n = spiro.Atoms.Count;
            var ringSearch = new RingSearch(spiro);
            for (int i = 0; i < n; i++)
            {
                Assert.IsTrue(ringSearch.Cyclic(i));
            }
        }

        [TestMethod()]
        public void TestIsolated()
        {
            var search = new RingSearch(spiro);
            var isolated = search.Isolated();
            Assert.AreEqual(2, isolated.Length);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(new[] { 4, 7 }, new int[] { isolated[0].Length, isolated[1].Length }));
        }

        [TestMethod()]
        public void TestFused()
        {
            Assert.AreEqual(0, new RingSearch(spiro).Fused().Length);
        }

        [TestMethod()]
        public void TestRingFragments()
        {
            var fragment = new RingSearch(spiro).RingFragments();
            Assert.AreEqual(spiro.Atoms.Count, fragment.Atoms.Count);
            Assert.AreEqual(spiro.Bonds.Count, fragment.Bonds.Count);
        }

        [TestMethod()]
        public void TestIsolatedRingFragments()
        {
            var search = new RingSearch(spiro);
            var isolated = search.IsolatedRingFragments().ToReadOnlyList();
            Assert.AreEqual(2, isolated.Count);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(new[] { 4, 7 }, new int[] { isolated[0].Atoms.Count, isolated[1].Atoms.Count }));
        }

        [TestMethod()]
        public void TestFusedRingFragments()
        {
            var search = new RingSearch(spiro);
            var fused = search.FusedRingFragments();
            Assert.AreEqual(0, fused.Count());
        }
    }
}
