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
using System.Collections;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NCDK.Graphs
{
    /// <summary>
    /// Maximum matching is not specific to kekulisation but it serves as a good
    /// demonstration. The provission of a subset to the matching inicates the atom
    /// indicies we know must be adjacent to a pi bond.
    ///
    // @author John May
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public sealed class EdmondsMaximumMatchingTest
    {
        private IChemObjectBuilder bldr;
        private SmilesParser smipar;

        [TestInitialize()]
        public void SetUp()
        {
            bldr = Silent.ChemObjectBuilder.Instance;
            smipar = new SmilesParser(bldr);
        }

        [TestMethod()]
        public void Benzene()
        {
            Matching m = CreateMatching("c1ccccc1");
            AssertMatch(m, 0, 1);
            AssertMatch(m, 2, 3);
            AssertMatch(m, 4, 5);
        }

        [TestMethod()]
        public void Fulvelene()
        {
            Matching m = CreateMatching("c1cccc1c1cccc1");
            AssertMatch(m, 0, 1);
            AssertMatch(m, 2, 3);
            AssertMatch(m, 4, 5);
            AssertMatch(m, 6, 7);
            AssertMatch(m, 8, 9);
        }

        [TestMethod()]
        public void Quinone()
        {
            Matching m = CreateMatching("oc1ccc(o)cc1");
            AssertMatch(m, 0, 1);
            AssertMatch(m, 2, 3);
            AssertMatch(m, 4, 5);
            AssertMatch(m, 6, 7);
        }

        [TestMethod()]
        public void Azulene()
        {
            Matching m = CreateMatching("c1cc2cccccc2c1");
            AssertMatch(m, 0, 1);
            AssertMatch(m, 2, 3);
            AssertMatch(m, 4, 5);
            AssertMatch(m, 6, 7);
            AssertMatch(m, 8, 9);
        }

        [TestMethod()]
        public void Pyrrole()
        {
            // the nitrogen (index=0) does not need any double bonds
            Matching m = CreateMatching("[nH]1cccc1", 1, 2, 3, 4);
            AssertMatch(m, 1, 2);
            AssertMatch(m, 3, 4);
        }

        [TestMethod()]
        public void Furane()
        {
            // the oxygen (index=0) does not need any double bonds
            Matching m = CreateMatching("o1cccc1", 1, 2, 3, 4);
            AssertMatch(m, 1, 2);
            AssertMatch(m, 3, 4);
        }

        [TestMethod()]
        public void Acyclic()
        {
            Matching m = CreateMatching("cccccc");
            AssertMatch(m, 0, 1);
            AssertMatch(m, 2, 3);
            AssertMatch(m, 4, 5);
        }

        [TestMethod()]
        public void Adenine()
        {
            // the nitroges (index 0 and 6) do need any double bonds
            Matching m = CreateMatching("Nc1ncnc2[nH]cnc12", 1, 2, 3, 4, 5, 7, 8, 9);
            AssertMatch(m, 1, 2);
            AssertMatch(m, 3, 4);
            AssertMatch(m, 5, 9);
            AssertMatch(m, 7, 8);
        }

        [TestMethod()]
        public void Caffeine()
        {
            // 0, 1, 5, 9, 10 do not need any double bonds
            Matching m = CreateMatching("Cn1cnc2n(C)c(=O)n(C)c(=O)c12", 2, 3, 4, 7, 8, 11, 12, 13);
            AssertMatch(m, 2, 3);
            AssertMatch(m, 4, 13);
            AssertMatch(m, 7, 8); // C=O was refound
            AssertMatch(m, 11, 12); // C=O was refound
        }

        /// <summary>
        /// These two large cases show why it's benifical to seed the matching with
        /// and arbitary matching first before maximising it. All matched edges are
        /// succesive.
        /// </summary>
        [TestMethod()]
        public void Fullerene_C60()
        {
            Matching m = CreateMatching("c12c3c4c5c1c1c6c7c2c2c8c3c3c9c4c4c%10c5c5c1c1c6c6c%11c7c2c2c7c8c3c3c8c9c4c4c9c%10c5c5c1c1c6c6c%11c2c2c7c3c3c8c4c4c9c5c1c1c6c2c3c41");
            for (int i = 0; i < 60; i += 2)
                AssertMatch(m, i, i + 1);
        }

        [TestMethod()]
        public void Graphene()
        {
            Matching m = CreateMatching("c1cc2cc3cc4cc5cc6cc7cc8cc9cc%10cc%11cc%12cc%13cc%14cc%15cc%16ccc%17ccc%18c%19ccc%20c%21ccc%22c%23ccc%24c%25ccc%26c%27ccc%28c%29ccc%30c%31cccc%32cc%33cc%34cc%35cc%36cc%37cc%38cc%39cc%40cc%41cc%42cc%43cc%44cc%45cc%46ccc%47ccc%48c%49ccc%50c%51ccc%52c%53ccc%54c%55ccc%56c%57ccc%58c%59ccc%60c(c1)c2c1c3c2c4c3c5c4c6c5c7c6c8c7c9c8c%10c9c%11c%10c%12c%11c%13c%12c%14c%13c%15c%14c%16c%17c%18c%15c%16c%19c%20c%17c%18c%21c%22c%19c%20c%23c%24c%21c%22c%25c%26c%23c%24c%27c%28c%25c%26c%29c%30c%27c(c%31%32)c%33c%28c%34c%29c%35c%30c%36c%31c%37c%32c%38c%33c%39c%34c%40c%35c%41c%36c%42c%37c%43c%38c%44c%39c%45c%40c%46c%47c%48c%41c%42c%49c%50c%43c%44c%51c%52c%45c%46c%53c%54c%47c%48c%55c%56c%49c%50c%57c%58c%51c%52c%59c%60c1c1c2c2c3c3c4c4c5c5c6c6c7c7c8c8c9c9c%10c%10c%11c%11c%12c%12c%13c(c%14%15)c%13c%16c%17c%14c%15c%18c%19c%16c%17c%20c%21c%18c%19c%22c%23c%20c%21c%24c%25c%22c%23c%26c%27c%28c%24c%29c%25c%30c%26c%31c%27c%32c%28c%33c%29c%34c%30c%35c%31c%36c%32c%37c%33c%38c%34c%39c(c%40%41)c%35c%42c%43c%36c%37c%44c%45c%38c%39c%46c%47c%40c%41c%48c%49c%42c%43c%50c%51c%44c(c%521)c2c1c3c2c4c3c5c4c6c5c7c6c8c7c9c8c%10c9c%11c%10c%12c%13c%14c%11c%12c%15c%16c%13c%14c%17c%18c%15c%16c%19c%20c%17c%18c%21c%22c%19c(c%23%24)c%25c%20c%26c%21c%27c%22c%28c%23c%29c%24c%30c%25c%31c%26c%32c%27c%33c%28c%34c%35c%36c%29c%30c%37c%38c%31c%32c%39c%40c%33c%34c%41c%42c%35c%36c%43c%44c1c1c2c2c3c3c4c4c5c5c6c6c7c7c8c8c9c(c%10%11)c9c%12c%13c%10c%11c%14c%15c%12c%13c%16c%17c%14c%15c%18c%19c%20c%16c%21c%17c%22c%18c%23c%19c%24c%20c%25c%21c%26c%22c%27c(c%28%29)c%23c%30c%31c%24c%25c%32c%33c%26c%27c%34c%35c%28c(c%361)c2c1c3c2c4c3c5c4c6c5c7c6c8c9c%10c7c8c%11c%12c9c%10c%13c%14c%11c(c%15%16)c%17c%12c%18c%13c%19c%14c%20c%15c%21c%16c%22c%23c%24c%17c%18c%25c%26c%19c%20c%27c%28c1c1c2c2c3c3c4c4c5c(c67)c5c8c9c6c7c%10c%11c%12c8c%13c9c%14c%10c%15c(c%16%17)c%11c%18c%19c%12c(c%201)c2c1c3c2c4c5c6c3c(c78)c9c4c%10c%11c%12c1c4c23");
            for (int i = 0; i < 576; i += 2)
                AssertMatch(m, i, i + 1);
        }

        // tougher than C60 due to odd cycles
        [TestMethod()]
        public void Fullerene_C70()
        {
            Matching m = CreateMatching("c12c3c4c5c1c1c6c7c2c2c8c3c3c9c4c4c%10c5c5c1c1c6c6c%11c%12c%13c%14c%15c%16c%17c%14c%14c%18c%13c%11c1c1c5c%10c5c(c%14c%10c%17c%11c%13c%16c%14c%16c%15c%12c%12c%16c(c2c7c6%12)c2c8c3c(c%13c%142)c2c9c4c5c%10c%112)c%181");
            AssertMatch(m, 0, 1);
            AssertMatch(m, 2, 3);
            AssertMatch(m, 4, 5);
            AssertMatch(m, 6, 7);
            AssertMatch(m, 8, 9);
            AssertMatch(m, 10, 11);
            AssertMatch(m, 12, 13);
            AssertMatch(m, 14, 15);
            AssertMatch(m, 16, 17);
            AssertMatch(m, 18, 19);
            AssertMatch(m, 20, 21);
            AssertMatch(m, 22, 56);
            AssertMatch(m, 23, 24);
            AssertMatch(m, 25, 33);
            AssertMatch(m, 26, 27);
            AssertMatch(m, 28, 29);
            AssertMatch(m, 30, 31);
            AssertMatch(m, 32, 69);
            AssertMatch(m, 34, 35);
            AssertMatch(m, 36, 37);
            AssertMatch(m, 38, 39);
            AssertMatch(m, 40, 41);
            AssertMatch(m, 42, 43);
            AssertMatch(m, 44, 45);
            AssertMatch(m, 46, 47);
            AssertMatch(m, 48, 49);
            AssertMatch(m, 50, 51);
            AssertMatch(m, 52, 53);
            AssertMatch(m, 54, 55);
            AssertMatch(m, 57, 58);
            AssertMatch(m, 59, 60);
            AssertMatch(m, 61, 62);
            AssertMatch(m, 63, 64);
            AssertMatch(m, 65, 66);
            AssertMatch(m, 67, 68);
        }

        void AssertMatch(Matching m, int u, int v)
        {
            Assert.IsTrue(m.Matched(u));
            Assert.IsTrue(m.Matched(v));
            Assert.AreEqual(v, m.Other(u));
        }

        private Matching CreateMatching(string smi, params int[] xs)
        {
            return CreateMatching(smipar.ParseSmiles(smi), xs);
        }

        private Matching CreateMatching(IAtomContainer container, params int[] xs)
        {
            BitArray subset = new BitArray(container.Atoms.Count);
            if (xs.Length == 0)
            {
                BitArrays.Flip(subset, 0, container.Atoms.Count);
            }
            else
            {
                foreach (var x in xs)
                    subset.Set(x, true);
            }
            Matching m = Matching.WithCapacity(container.Atoms.Count);
            return EdmondsMaximumMatching.Maxamise(m, GraphUtil.ToAdjList(container), subset);
        }
    }
}
