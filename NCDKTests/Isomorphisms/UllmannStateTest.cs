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

namespace NCDK.Isomorphisms
{
    /// <summary>
    // @author John May
    // @cdk.module test-isomorphism
    /// </summary>
    [TestClass()]
    public class UllmannStateTest
    {
        [TestMethod()]
        public void TestNextN()
        {
            UllmannState state = CreateBenzeneToNaphthalene(AtomMatcher.CreateAnyMatcher(), BondMatcher.CreateAnyMatcher());
            Assert.AreEqual(state.NextN(0), 0);
            state.size = 1;
            Assert.AreEqual(state.NextN(0), 1);
            state.size = 2;
            Assert.AreEqual(state.NextN(0), 2);
        }

        [TestMethod()]
        public void TestNextM()
        {
            UllmannState state = CreateBenzeneToNaphthalene(AtomMatcher.CreateAnyMatcher(), BondMatcher.CreateAnyMatcher());
            Assert.AreEqual(state.NextM(0, -1), 0);
            Assert.AreEqual(state.NextM(0, 0), 1);
            Assert.AreEqual(state.NextM(0, 1), 2);
            state.m2[1] = 0; // 1 has been mapped and should be skipped over
            Assert.AreEqual(state.NextM(0, 0), 2);
        }

        [TestMethod()]
        public void Add()
        {
            UllmannState state = CreateBenzeneToNaphthalene(AtomMatcher.CreateAnyMatcher(), BondMatcher.CreateAnyMatcher());
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}},
                state.matrix.Fix()
                ));
            Assert.IsTrue(state.Add(0, 0));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {1, -1, 1, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, 1, -1, -1, -1, 1, -1, 1},
                    new[] {1, -1, 1, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1}},
                state.matrix.Fix()));
            Assert.IsTrue(state.Add(1, 9));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, -2, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {1, -1, -2, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, -2, -1, -1, -1, 1, -1, 1},
                    new[] {1, -1, 1, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1}},
                state.matrix.Fix()));
            Assert.IsTrue(state.Add(2, 8));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, -2, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {-3, -1, -2, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, -3, -1, -2, -1, -1, -1, 1, -1, 1},
                    new[] {1, -1, 1, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1}},
                state.matrix.Fix()));
            Assert.IsTrue(state.Add(3, 7));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, -2, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {-3, -1, -2, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, -3, -1, -2, -1, -1, -1, 1, -1, -4},
                    new[] {-4, -1, 1, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1}},
                state.matrix.Fix()));
            Assert.IsTrue(state.Add(4, 2));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[]  {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, -2, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {-3, -1, -2, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, -3, -1, -2, -1, -1, -1, 1, -1, -4},
                    new[] {-4, -1, 1, -1, -1, -1, -1, -1, -5, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, -5}},
                state.matrix.Fix()));
            Assert.IsTrue(state.Add(5, 1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, -2, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {-3, -1, -2, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, -3, -1, -2, -1, -1, -1, 1, -1, -4},
                    new[] {-4, -1, 1, -1, -1, -1, -1, -1, -5, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, -5}},
                state.matrix.Fix()));
        }

        [TestMethod()]
        public void Remove()
        {
            UllmannState state = CreateBenzeneToNaphthalene(AtomMatcher.CreateAnyMatcher(), BondMatcher.CreateAnyMatcher());
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}},
                state.matrix.Fix()));
            Assert.IsTrue(state.Add(0, 0));
            Assert.IsTrue(state.Add(1, 9));
            Assert.IsTrue(state.Add(2, 8));
            Assert.IsTrue(state.Add(3, 7));
            Assert.IsTrue(state.Add(4, 2));
            Assert.IsTrue(state.Add(5, 1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, -2, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {-3, -1, -2, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, -3, -1, -2, -1, -1, -1, 1, -1, -4},
                    new[] {-4, -1, 1, -1, -1, -1, -1, -1, -5, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, -5}},
                state.matrix.Fix()));
            state.Remove(5, 1);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, -2, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {-3, -1, -2, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, -3, -1, -2, -1, -1, -1, 1, -1, -4},
                    new[] {-4, -1, 1, -1, -1, -1, -1, -1, -5, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, -5}},
                state.matrix.Fix()));
            state.Remove(4, 2);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, -2, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {-3, -1, -2, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, -3, -1, -2, -1, -1, -1, 1, -1, -4},
                    new[] {-4, -1, 1, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1}},
                state.matrix.Fix()));
            state.Remove(3, 7);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, -2, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {-3, -1, -2, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, -3, -1, -2, -1, -1, -1, 1, -1, 1},
                    new[] {1, -1, 1, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1}},
                state.matrix.Fix()));
            state.Remove(2, 8);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, -2, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {1, -1, -2, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, -2, -1, -1, -1, 1, -1, 1},
                    new[] {1, -1, 1, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1}},
                state.matrix.Fix()));
            state.Remove(1, 9);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1},
                    new[] {1, -1, 1, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, 1, -1, -1, -1, 1, -1, 1},
                    new[] {1, -1, 1, -1, -1, -1, -1, -1, 1, -1},
                    new[] {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1}},
                state.matrix.Fix()));
            state.Remove(0, 0);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}},
                state.matrix.Fix()));
        }

        [TestMethod()]
        public void Mapping()
        {
            UllmannState state = CreateBenzeneToNaphthalene(AtomMatcher.CreateAnyMatcher(), BondMatcher.CreateAnyMatcher());
            state.m1[0] = 1;
            state.m1[1] = 2;
            Assert.IsTrue(Compares.AreDeepEqual(state.m1, state.Mapping()));
            Assert.AreNotSame(state.m1, state.Mapping());
        }

        [TestMethod()]
        public void Accessors()
        {
            UllmannState state = CreateBenzeneToNaphthalene(AtomMatcher.CreateAnyMatcher(), BondMatcher.CreateAnyMatcher());
            state.size = 1;
            Assert.AreEqual(state.Count, 1);
            Assert.AreEqual(state.NMax(), state.g1.Length);
            Assert.AreEqual(state.MMax(), state.g2.Length);
        }

        /// <summary>
        /// Create a state for matching benzene to naphthalene Benzene:
        /// InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H Naphthalene: InChI=1/C10H8/c1-2-6-10-8-4-3-7-9(10)5-1/h1-8H
        /// </summary>
        UllmannState CreateBenzeneToNaphthalene(AtomMatcher atomMatcher, BondMatcher bondMatcher)
        {
            IAtomContainer container1 = TestMoleculeFactory.MakeBenzene();
            IAtomContainer container2 = TestMoleculeFactory.MakeNaphthalene();
            GraphUtil.EdgeToBondMap bonds1 = GraphUtil.EdgeToBondMap.WithSpaceFor(container1);
            GraphUtil.EdgeToBondMap bonds2 = GraphUtil.EdgeToBondMap.WithSpaceFor(container2);
            int[][] g1 = GraphUtil.ToAdjList(container1, bonds1);
            int[][] g2 = GraphUtil.ToAdjList(container2, bonds2);
            return new UllmannState(container1, container2, g1, g2, bonds1, bonds2, atomMatcher, bondMatcher);
        }
    }
}
