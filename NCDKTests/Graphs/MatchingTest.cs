/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;
using System;
using System.Collections;

namespace NCDK.Graphs
{
    /// <summary>
    // @author John May
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class MatchingTest
    {
        private static IChemObjectBuilder Builder { get; } = Silent.ChemObjectBuilder.Instance;
        private SmilesParser smipar = new SmilesParser(Builder);

        //@Ignore("no operation performed")
        public void Nop() { }

        [TestMethod()]
        public void Match()
        {
            Matching matching = Matching.WithCapacity(8);
            matching.Match(2, 5);
            matching.Match(6, 7);
            Assert.IsTrue(matching.Matched(2));
            Assert.IsTrue(matching.Matched(5));
            Assert.IsTrue(matching.Matched(6));
            Assert.IsTrue(matching.Matched(7));
            Assert.AreEqual(5, matching.Other(2));
            Assert.AreEqual(2, matching.Other(5));
            Assert.AreEqual(7, matching.Other(6));
            Assert.AreEqual(6, matching.Other(7));
        }

        [TestMethod()]
        public void Replace()
        {
            Matching matching = Matching.WithCapacity(8);
            matching.Match(2, 5);
            matching.Match(6, 7);
            matching.Match(5, 6);
            Assert.IsFalse(matching.Matched(2));
            Assert.IsTrue(matching.Matched(5));
            Assert.IsTrue(matching.Matched(6));
            Assert.IsFalse(matching.Matched(7));
            Assert.AreEqual(6, matching.Other(5));
            Assert.AreEqual(5, matching.Other(6));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Other()
        {
            Matching matching = Matching.WithCapacity(8);
            matching.Match(2, 5);
            matching.Match(6, 7);
            matching.Match(5, 6);
            matching.Other(2); // 2 is unmatched!
        }

        [TestMethod()]
        public void Unmatch()
        {
            Matching matching = Matching.WithCapacity(5);
            matching.Match(2, 4);
            matching.Unmatch(4); // also unmatches 2
            Assert.IsFalse(matching.Matched(4));
            Assert.IsFalse(matching.Matched(2));
        }

        [TestMethod()]
        public void PerfectArbitaryMatching()
        {
            Matching matching = Matching.WithCapacity(4);
            BitArray subset = new BitArray(4);
            BitArrays.Flip(subset, 0, 4);
            Assert.IsTrue(matching.ArbitaryMatching(new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2 } }, subset));
        }

        [TestMethod()]
        public void ImperfectArbitaryMatching()
        {
            Matching matching = Matching.WithCapacity(5);
            BitArray subset = new BitArray(5);
            BitArrays.Flip(subset, 0, 5);
            Assert.IsFalse(matching.ArbitaryMatching(new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3 } }, subset));
        }

        [TestMethod()]
        public void Fulvelene1()
        {
            int[][] graph = GraphUtil.ToAdjList(smipar.ParseSmiles("c1cccc1c1cccc1"));
            Matching m = Matching.WithCapacity(graph.Length);
            BitArray subset = new BitArray(graph.Length);
            BitArrays.Flip(subset, 0, graph.Length);
            // arbitary matching will assign a perfect matching here
            Assert.IsTrue(m.ArbitaryMatching(graph, subset));
        }

        [TestMethod()]
        public void Fulvelene2()
        {
            int[][] graph = GraphUtil.ToAdjList(smipar.ParseSmiles("c1cccc1c1cccc1"));
            Matching m = Matching.WithCapacity(graph.Length);
            BitArray subset = new BitArray(graph.Length);
            BitArrays.Flip(subset, 0, graph.Length);

            // induced match - can't be perfected without removing this match
            m.Match(1, 2);

            // arbitary matching will not be able assign a perfect matching
            Assert.IsFalse(m.ArbitaryMatching(graph, subset));

            // but Perfect() will
            Assert.IsTrue(m.Perfect(graph, subset));
        }

        [TestMethod()]
        public void StRingTest()
        {
            Matching matching = Matching.WithCapacity(9);
            matching.Match(1, 3);
            matching.Match(4, 8);
            Assert.AreEqual("[1=3, 4=8]", matching.ToString());
        }
    }
}
