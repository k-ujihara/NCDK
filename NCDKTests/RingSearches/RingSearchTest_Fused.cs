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
    /// ring search unit tests for a fused system
    ///
    // @author John May
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class RingSearchTest_Fused
    {

        private readonly IAtomContainer fusedRings = TestMoleculeFactory.MakeFUsedRings();

        [TestMethod()]
        public void TestCyclic_Int()
        {
            RingSearch ringSearch = new RingSearch(fusedRings);
            for (int i = 0; i < fusedRings.Atoms.Count; i++)
                Assert.IsTrue(ringSearch.Cyclic(i));
        }

        [TestMethod()]
        public void TestCyclic()
        {
            RingSearch ringSearch = new RingSearch(fusedRings);
            Assert.AreEqual(fusedRings.Atoms.Count, ringSearch.Cyclic().Length);
        }

        [TestMethod()]
        public void TestFUsed()
        {
            RingSearch ringSearch = new RingSearch(fusedRings);
            Assert.AreEqual(1, ringSearch.Fused().Length);
        }

        [TestMethod()]
        public void TestIsolated()
        {
            RingSearch ringSearch = new RingSearch(fusedRings);
            Assert.AreEqual(0, ringSearch.Isolated().Length);
        }

        [TestMethod()]
        public void TestRingFragments()
        {
            RingSearch ringSearch = new RingSearch(fusedRings);
            IAtomContainer fragment = ringSearch.RingFragments();
            foreach (var atom in fusedRings.Atoms)
            {
                Assert.IsTrue(fragment.Contains(atom));
            }
            foreach (var bond in fusedRings.Bonds)
            {
                Assert.IsTrue(fragment.Contains(bond));
            }
        }

        [TestMethod()]
        public void TestFUsedRingFragments()
        {
            RingSearch ringSearch = new RingSearch(fusedRings);
            IList<IAtomContainer> fragments = ringSearch.FusedRingFragments();
            Assert.AreEqual(1, fragments.Count);
            IAtomContainer fragment = fragments[0];
            foreach (var atom in fusedRings.Atoms)
            {
                Assert.IsTrue(fragment.Contains(atom));
            }
            foreach (var bond in fusedRings.Bonds)
            {
                Assert.IsTrue(fragment.Contains(bond));
            }
        }

        [TestMethod()]
        public void TestIsolatedRingFragments()
        {
            RingSearch ringSearch = new RingSearch(fusedRings);
            IList<IAtomContainer> fragments = ringSearch.IsolatedRingFragments();
            Assert.AreEqual(0, fragments.Count);
        }
    }
}
