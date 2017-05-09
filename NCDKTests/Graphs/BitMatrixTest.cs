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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Common.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using static NCDK.Graphs.InitialCycles;
using NCDK.Common.Base;

namespace NCDK.Graphs
{
    // @author John May
    // @cdk.module test-core
    [TestClass()]
    public class BitMatrixTest
    {
        [TestMethod()]
        public virtual void Swap_basic()
        {
            BitMatrix m = new BitMatrix(0, 4);
            IList<BitArray> rows = new List<BitArray>();
            for (int i = 0; i < 4; i++)
            {
                BitArray row = new BitArray(4);
                rows.Add(row);
                m.Add(row);
            }
            m.Swap(0, 1);
            m.Swap(2, 3);
            m.Swap(0, 2);
            // check when we access by index we get back the exact same instance
            for (int i = 0; i < 4; i++)
            {
                Assert.AreSame(rows[i], m.Row(i));
            }
        }

        // ensure we can access the rows by with original index even after swapping
        [TestMethod()]
        public virtual void Swap()
        {
            BitMatrix m = new BitMatrix(0, 100);
            IList<BitArray> rows = new List<BitArray>();
            for (int i = 0; i < 100; i++)
            {
                BitArray row = new BitArray(100);
                rows.Add(row);
                m.Add(row);
            }

            // randomly swap rows
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                m.Swap(random.Next(100), random.Next(100));
            }

            // check when we access by index we get back the exact same instance
            for (int i = 0; i < 100; i++)
            {
                Assert.AreSame(rows[i], m.Row(i));
            }
        }

        [TestMethod()]
        public virtual void Clear()
        {
            BitMatrix m = new BitMatrix(9, 3);
            BitArray r1 = BitArrays.FromString("110000000");
            BitArray r2 = BitArrays.FromString("100000000");
            BitArray r3 = BitArrays.FromString("010000000");
            m.Add(r1);
            m.Add(r2);
            m.Add(r3);
            Assert.AreSame(r1, m.Row(0));
            Assert.AreSame(r2, m.Row(1));
            Assert.AreSame(r3, m.Row(2));
            m.Clear();
            m.Add(r3);
            m.Add(r2);
            m.Add(r1);
            Assert.AreSame(r3, m.Row(0));
            Assert.AreSame(r2, m.Row(1));
            Assert.AreSame(r1, m.Row(2));
        }

        [TestMethod()]
        public virtual void IndexOf()
        {
            BitMatrix m = new BitMatrix(9, 3);
            BitArray r1 = BitArrays.FromString("010000000");
            BitArray r2 = BitArrays.FromString("100001000");
            BitArray r3 = BitArrays.FromString("010000000");
            m.Add(r1);
            m.Add(r2);
            m.Add(r3);
            Assert.AreEqual(1, m.IndexOf(0, 0));
            Assert.AreEqual(1, m.IndexOf(0, 1));
            Assert.AreEqual(-1, m.IndexOf(0, 2));
            Assert.AreEqual(0, m.IndexOf(1, 0));
            Assert.AreEqual(2, m.IndexOf(1, 1));
            Assert.AreEqual(2, m.IndexOf(1, 2));
            Assert.AreEqual(-1, m.IndexOf(2, 0));
            Assert.AreEqual(-1, m.IndexOf(2, 1));
            Assert.AreEqual(-1, m.IndexOf(2, 2));
        }

        [TestMethod()]
        public virtual void StRingTest()
        {
            BitMatrix m = new BitMatrix(9, 3);
            m.Add(BitArrays.FromString("110000000"));
            m.Add(BitArrays.FromString("110011000"));
            m.Add(BitArrays.FromString("000011000"));
            string str = m.ToString();
            Assert.AreEqual("0: 11-------\n" + "1: 11--11---\n" + "2: ----11---\n", str);
        }

        [TestMethod()]
        public virtual void Eliminate1()
        {
            // vectors[0] = vectors[1] ^ vectors[2] (xor)
            BitMatrix m = new BitMatrix(9, 3);
            m.Add(BitArrays.FromString("110000000"));
            m.Add(BitArrays.FromString("110011000"));
            m.Add(BitArrays.FromString("000011000"));
            Assert.AreEqual(2, m.Eliminate());
            Assert.IsFalse(m.Eliminated(0));
            Assert.IsFalse(m.Eliminated(1));
            Assert.IsTrue(m.Eliminated(2));
        }

        [TestMethod()]
        public virtual void Eliminate2()
        {
            // vectors[2] = vectors[0] ^ vectors[1] (xor)
            BitMatrix m = new BitMatrix(9, 3);
            m.Add(BitArrays.FromString("110011000"));
            m.Add(BitArrays.FromString("001000110"));
            m.Add(BitArrays.FromString("111011110"));
            Assert.AreEqual(2, m.Eliminate());
            Assert.IsFalse(m.Eliminated(0));
            Assert.IsFalse(m.Eliminated(1));
            Assert.IsTrue(m.Eliminated(2));
        }

        [TestMethod()]
        public virtual void Eliminate3()
        {
            // all vectors are independent
            BitMatrix m = new BitMatrix(15, 4);

            // 1-3 can all be made from each other
            m.Add(BitArrays.FromString("111111000000000"));
            m.Add(BitArrays.FromString("000111111000000"));
            m.Add(BitArrays.FromString("111000111000000"));

            // 4 cannot
            m.Add(BitArrays.FromString("111000000111100"));

            // 1,2 or 3 was eliminated
            Assert.AreEqual(3, m.Eliminate());

            // 4 was not
            Assert.IsFalse(m.Eliminated(3));
        }

        [TestMethod()]
        public virtual void Independent1()
        {
            BitMatrix m = new BitMatrix(9, 3);
            m.Add(BitArrays.FromString("010011000"));
            m.Add(BitArrays.FromString("001000110"));
            m.Add(BitArrays.FromString("111011110"));
            Assert.AreEqual(3, m.Eliminate());
            Assert.IsFalse(m.Eliminated(0));
            Assert.IsFalse(m.Eliminated(1));
            Assert.IsFalse(m.Eliminated(2));
        }

        [TestMethod()]
        public virtual void Independent2()
        {
            // all vectors are independent
            BitMatrix m = new BitMatrix(9, 3);
            m.Add(BitArrays.FromString("110011000"));
            m.Add(BitArrays.FromString("110011011"));
            m.Add(BitArrays.FromString("110011010"));
            Assert.AreEqual(3, m.Eliminate());
            Assert.IsFalse(m.Eliminated(0));
            Assert.IsFalse(m.Eliminated(1));
            Assert.IsFalse(m.Eliminated(2));
        }

        [TestMethod()]
        public virtual void Duplicates()
        {
            // ensure duplicates are handled
            BitMatrix m = new BitMatrix(9, 3);
            m.Add(BitArrays.FromString("110000000"));
            m.Add(BitArrays.FromString("110000000"));
            m.Add(BitArrays.FromString("001100000"));
            Assert.AreEqual(2, m.Eliminate());
        }

        [TestMethod()]
        public virtual void Xor()
        {
            BitArray s = BitArrays.FromString("00011");
            BitArray t = BitArrays.FromString("10010");
            BitArray u = BitMatrix.Xor(s, t);
            Assert.AreNotSame(u, s);
            Assert.AreNotSame(u, t);
            Assert.IsTrue(Compares.AreDeepEqual(BitArrays.FromString("10001"), u));
        }

        [TestMethod()]
        public virtual void From_cycles()
        {
            var c1 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c2 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c3 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            BitArray s1 = BitArrays.FromString("010011000");
            BitArray s2 = BitArrays.FromString("110011011");
            BitArray s3 = BitArrays.FromString("110011010");
            c1.SetupGet(c => c.EdgeVector).Returns(s1);
            c2.SetupGet(c => c.EdgeVector).Returns(s2);
            c3.SetupGet(c => c.EdgeVector).Returns(s3);
            BitMatrix m = BitMatrix.From(new[] { c1.Object, c2.Object, c3.Object });
            Assert.AreSame(s1, m.Row(0));
            Assert.AreSame(s2, m.Row(1));
            Assert.AreSame(s3, m.Row(2));
        }


        [TestMethod()]
        public virtual void From_cycles_cycle()
        {
            var c1 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c2 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var last = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            BitArray s1 = BitArrays.FromString("010011000");
            BitArray s2 = BitArrays.FromString("110011011");
            BitArray s3 = BitArrays.FromString("110011010");
            c1.SetupGet(c => c.EdgeVector).Returns(s1);
            c2.SetupGet(c => c.EdgeVector).Returns(s2);
            last.SetupGet(c => c.EdgeVector).Returns(s3);
            BitMatrix m = BitMatrix.From(new[] { c1.Object, c2.Object }, last.Object);
            Assert.AreSame(s1, m.Row(0));
            Assert.AreSame(s2, m.Row(1));
            Assert.AreSame(s3, m.Row(2));
        }
    }
}
