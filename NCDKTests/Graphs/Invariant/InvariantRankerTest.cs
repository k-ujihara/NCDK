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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Graphs.Invariant
{
    // @author John May
    // @cdk.module test-standard
    [TestClass()]
    public class InvariantRankerTest
    {
        [TestMethod()]
        public void Rank()
        {
            InvariantRanker ranker = new InvariantRanker(6);
            long[] prev = new long[] { 1, 1, 1, 1, 1, 1 };
            long[] curr = new long[] { 50, 100, 25, 100, 50, 90 };

            // no we leave extra space
            int[] vs = new int[] { 0, 1, 2, 3, 4, 5 };
            int[] ws = new int[6];

            int ranks = ranker.Rank(vs, ws, 6, curr, prev);

            Assert.AreEqual(4, ranks);

            // assigned ranks (note: unique assigned first)
            Assert.IsTrue(Compares.AreEqual(new long[] { 2, 5, 1, 5, 2, 4 }, prev));

            // remaining non-unique vertices
            Assert.IsTrue(Compares.AreEqual(new int[] { 0, 4, 1, 3, -1, 0 }, ws));
        }

        [TestMethod()]
        public void Rank_all_equiv()
        {
            InvariantRanker ranker = new InvariantRanker(6);
            long[] prev = new long[] { 1, 1, 1, 1, 1, 1 };
            long[] curr = new long[] { 42, 42, 42, 42, 42, 42 };

            // no we leave extra space
            int[] vs = new int[] { 0, 1, 2, 3, 4, 5 };
            int[] ws = new int[6];

            int ranks = ranker.Rank(vs, ws, 6, curr, prev);

            Assert.AreEqual(1, ranks);

            // assigned ranks (note: unique assigned first)
            Assert.IsTrue(Compares.AreEqual(new long[] { 1, 1, 1, 1, 1, 1 }, prev));

            // remaining non-unique vertices
            Assert.IsTrue(Compares.AreEqual(new int[] { 0, 1, 2, 3, 4, 5 }, ws));
        }

        [TestMethod()]
        public void Rank_all_unique()
        {
            InvariantRanker ranker = new InvariantRanker(7);
            long[] prev = new long[] { 1, 1, 1, 1, 1, 1, 1 };
            long[] curr = new long[] { 7, 3, 1, 0, 91, 32, 67 };

            // no we leave extra space
            int[] vs = new int[] { 0, 1, 2, 3, 4, 5, 6 };
            int[] ws = new int[7];

            int ranks = ranker.Rank(vs, ws, 7, curr, prev);

            Assert.AreEqual(7, ranks);

            // assigned ranks (note: unique assigned first)
            Assert.IsTrue(Compares.AreEqual(new long[] { 4, 3, 2, 1, 7, 5, 6 }, prev));

            // no non-unique vertices
            Assert.IsTrue(Compares.AreEqual(new int[] { -1, 0, 0, 0, 0, 0, 0 }, ws));
        }

        [TestMethod()]
        public void MergeSort()
        {
            int n = 100;

            // random (unique) values in random order
            Random rnd = new Random();
            ICollection<long> values = new HashSet<long>();
            while (values.Count < n)
                values.Add((long)(((ulong)((uint)rnd.Next()) << 32) + ((uint)rnd.Next())));

            long[] prev = values.ToArray();

            // ident array
            int[] vs = new int[n];
            for (int i = 0; i < n; i++)
                vs[i] = i;

            InvariantRanker invRanker = new InvariantRanker(n);
            invRanker.SortBy(vs, 0, n, prev, prev);

            // check they are sorted
            for (int i = 1; i < n; i++)
                Assert.IsTrue(prev[vs[i]] > prev[vs[i - 1]]);
        }

        [TestMethod()]
        public void MergeSort_range()
        {
            int n = 100;

            // random (unique) values in random order
            Random rnd = new Random();
            ICollection<long> values = new HashSet<long>();
            while (values.Count < n)
                values.Add((long)(((ulong)((uint)rnd.Next()) << 32) + ((uint)rnd.Next())));

            long[] prev = values.ToArray();

            // ident array
            int[] vs = new int[n];
            for (int i = 0; i < n; i++)
                vs[i] = i;

            InvariantRanker invRanker = new InvariantRanker(n);
            invRanker.SortBy(vs, 10, n - 20, prev, prev);

            // check they are sorted
            for (int i = 11; i < (n - 20); i++)
                Assert.IsTrue(prev[vs[i]] > prev[vs[i - 1]]);

            // other values weren't touched
            for (int i = 0; i < 10; i++)
                Assert.AreEqual(i, vs[i]);
            for (int i = n - 10; i < n; i++)
                Assert.AreEqual(i, vs[i]);
        }

        [TestMethod()]
        public void InsertionSort()
        {
            long[] prev = new long[] { 11, 10, 9, 8, 7 };
            long[] curr = new long[] { 11, 10, 9, 8, 7 };
            int[] vs = new int[] { 0, 1, 2, 3, 4 };
            InvariantRanker.InsertionSortBy(vs, 0, 5, curr, prev);
            Assert.IsTrue(Compares.AreEqual(new int[] { 4, 3, 2, 1, 0 }, vs));
        }

        [TestMethod()]
        public void InsertionSort_duplicate()
        {
            long[] prev = new long[] { 11, 10, 10, 9, 8, 7 };
            long[] curr = new long[] { 11, 10, 10, 9, 8, 7 };
            int[] vs = new int[] { 0, 1, 2, 3, 4, 5 };
            InvariantRanker.InsertionSortBy(vs, 0, 6, curr, prev);
            Assert.IsTrue(Compares.AreEqual(new int[] { 5, 4, 3, 1, 2, 0 }, vs));
        }

        [TestMethod()]
        public void InsertionSort_range()
        {
            long[] prev = new long[] { 12, 11, 10, 9, 8, 7 };
            long[] curr = new long[] { 12, 11, 10, 9, 8, 7 };
            int[] vs = new int[] { 0, 1, 2, 3, 4, 5 };
            InvariantRanker.InsertionSortBy(vs, 2, 3, curr, prev);
            Assert.IsTrue(Compares.AreEqual(new int[] { 0, 1, 4, 3, 2, 5 }, vs));
        }

        [TestMethod()]
        public void Less()
        {
            long[] prev = new long[] { 1, 1, 2, 2 };
            long[] curr = new long[] { 1, 1, 2, 2 };
            Assert.IsFalse(InvariantRanker.Less(0, 1, curr, prev));
            Assert.IsFalse(InvariantRanker.Less(2, 3, curr, prev));
            Assert.IsTrue(InvariantRanker.Less(0, 2, curr, prev));
            Assert.IsTrue(InvariantRanker.Less(0, 3, curr, prev));
            Assert.IsTrue(InvariantRanker.Less(1, 2, curr, prev));
            Assert.IsTrue(InvariantRanker.Less(1, 3, curr, prev));
        }

        [TestMethod()]
        public void LessUsingPrev()
        {
            long[] prev = new long[] { 1, 1, 2, 2 };
            long[] curr = new long[] { 1, 2, 1, 2 };
            // 0,1 and 2,3 are only less is we inspect the 'curr' invariants
            Assert.IsTrue(InvariantRanker.Less(0, 1, curr, prev));
            Assert.IsTrue(InvariantRanker.Less(2, 3, curr, prev));
            // these values are only less inspecting the first invariants
            Assert.IsTrue(InvariantRanker.Less(0, 2, curr, prev));
            Assert.IsTrue(InvariantRanker.Less(0, 3, curr, prev));
            Assert.IsTrue(InvariantRanker.Less(1, 2, curr, prev));
            Assert.IsTrue(InvariantRanker.Less(1, 3, curr, prev));
        }
    }
}
