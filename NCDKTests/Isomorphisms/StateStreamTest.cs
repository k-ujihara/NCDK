/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Graphs;
using NCDK.Templates;
using System.Linq;

namespace NCDK.Isomorphisms
{
    // @author John May
    // @cdk.module test-isomorphism
    [TestClass()]
    public class StateStreamTest
    {
        [TestMethod()]
        public void HasNext()
        {
            var state = CreateNaphthaleneToBenzene(AtomMatcher.CreateAnyMatcher(), BondMatcher.CreateAnyMatcher());
            var it = new StateStream(state);
            Assert.IsFalse(it.Any());
        }

        [TestMethod()]
        public void HasNext2()
        {
            var state = CreateBenzeneToNaphthalene(AtomMatcher.CreateAnyMatcher(), BondMatcher.CreateAnyMatcher());
            int cnt = 0;
            var it = new StateStream(state);
            foreach (var i in it)
            {
                Assert.IsNotNull(i);
                cnt++;
            }
            Assert.AreEqual(24, cnt);
        }

        [TestMethod()]
        public void Next()
        {
            var state = CreateBenzeneToNaphthalene(AtomMatcher.CreateAnyMatcher(), BondMatcher.CreateAnyMatcher());
            var it = new StateStream(state).GetEnumerator();
            it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 7, 8, 9 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 9, 8, 7, 2, 1 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 0, 9, 8, 7, 2 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 2, 7, 8, 9, 0 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 1, 0, 9, 8, 7 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 3, 4, 5, 6, 7 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 7, 6, 5, 4, 3 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 7, 8, 9, 0, 1 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 3, 2, 7, 6, 5, 4 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 3, 4, 5, 6, 7, 2 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 4, 3, 2, 7, 6, 5 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 4, 5, 6, 7, 2, 3 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 5, 4, 3, 2, 7, 6 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 5, 6, 7, 2, 3, 4 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 6, 5, 4, 3, 2, 7 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 6, 7, 2, 3, 4, 5 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 7, 2, 1, 0, 9, 8 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 7, 2, 3, 4, 5, 6 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 7, 6, 5, 4, 3, 2 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 7, 8, 9, 0, 1, 2 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 8, 7, 2, 1, 0, 9 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 8, 9, 0, 1, 2, 7 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 9, 0, 1, 2, 7, 8 }, it.Current)); it.MoveNext();
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 9, 8, 7, 2, 1, 0 }, it.Current)); it.MoveNext();
        }

        /// <summary>
        /// Create a sub state for matching benzene to naphthalene
        ///
        ///
        /// Benzene: InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H
        ///
        /// Naphthalene: InChI=1/C10H8/c1-2-6-10-8-4-3-7-9(10)5-1/h1-8H
        /// </summary>
        VFSubState CreateBenzeneToNaphthalene(AtomMatcher atomMatcher, BondMatcher bondMatcher)
        {
            IAtomContainer container1 = TestMoleculeFactory.MakeBenzene();
            IAtomContainer container2 = TestMoleculeFactory.MakeNaphthalene();
            GraphUtil.EdgeToBondMap bonds1 = GraphUtil.EdgeToBondMap.WithSpaceFor(container1);
            GraphUtil.EdgeToBondMap bonds2 = GraphUtil.EdgeToBondMap.WithSpaceFor(container2);
            int[][] g1 = GraphUtil.ToAdjList(container1, bonds1);
            int[][] g2 = GraphUtil.ToAdjList(container2, bonds2);
            return new VFSubState(container1, container2, g1, g2, bonds1, bonds2, atomMatcher, bondMatcher);
        }

        /// <summary>
        /// Create a sub state for matching naphthalene to benzene
        ///
        /// Benzene: InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H
        ///
        /// Naphthalene: InChI=1/C10H8/c1-2-6-10-8-4-3-7-9(10)5-1/h1-8H
        /// </summary>
        VFSubState CreateNaphthaleneToBenzene(AtomMatcher atomMatcher, BondMatcher bondMatcher)
        {
            IAtomContainer container1 = TestMoleculeFactory.MakeNaphthalene();
            IAtomContainer container2 = TestMoleculeFactory.MakeBenzene();
            GraphUtil.EdgeToBondMap bonds1 = GraphUtil.EdgeToBondMap.WithSpaceFor(container1);
            GraphUtil.EdgeToBondMap bonds2 = GraphUtil.EdgeToBondMap.WithSpaceFor(container2);
            int[][] g1 = GraphUtil.ToAdjList(container1, bonds1);
            int[][] g2 = GraphUtil.ToAdjList(container2, bonds2);
            return new VFSubState(container1, container2, g1, g2, bonds1, bonds2, atomMatcher, bondMatcher);
        }
    }
}
