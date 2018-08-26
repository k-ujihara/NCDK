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
using NCDK.Silent;
using System.Linq;

namespace NCDK.RingSearches
{
    /// <summary>
    /// ring search unit tests for an empty molecule
    /// </summary>
    // @author John May
    // @cdk.module test-standard
    [TestClass()]
    public sealed class RingSearchTest_Empty
    {
        private readonly IAtomContainer empty = new AtomContainer();

        [TestMethod()]
        public void TestCyclic()
        {
            Assert.AreEqual(0, new RingSearch(empty).Cyclic().Length);
        }

        [TestMethod()]
        public void TestCyclic_Int()
        {
            int n = empty.Atoms.Count;
            RingSearch ringSearch = new RingSearch(empty);
            for (int i = 0; i < n; i++)
            {
                Assert.IsFalse(ringSearch.Cyclic(i));
            }
        }

        [TestMethod()]
        public void TestIsolated()
        {
            Assert.AreEqual(0, new RingSearch(empty).Isolated().Length);
        }

        [TestMethod()]
        public void TestFused()
        {
            Assert.AreEqual(0, new RingSearch(empty).Fused().Length);
        }

        [TestMethod()]
        public void TestRingFragments()
        {
            Assert.IsTrue(new RingSearch(empty).RingFragments().IsEmpty());
        }

        [TestMethod()]
        public void TestIsolatedRingFragments()
        {
            Assert.IsTrue(new RingSearch(empty).IsolatedRingFragments().Count() == 0);
        }

        [TestMethod()]
        public void TestFusedRingFragments()
        {
            Assert.IsTrue(new RingSearch(empty).FusedRingFragments().Count() == 0);
        }
    }
}
