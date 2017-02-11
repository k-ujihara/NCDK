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
using NCDK.Graphs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static NCDK.Graphs.InitialCycles;
using static NCDK.Graphs.InitialCyclesTest;

namespace NCDK.Graphs
{
    /**
     * @author John May
     * @cdk.module test-core
     */
	[TestClass()]
    public class GreedyBasisTest
    {

        [TestMethod()]
        public virtual void Add()
        {
            var c1 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c2 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            c1.SetupGet(c => c.EdgeVector).Returns(new BitArray(0));
            c2.SetupGet(c => c.EdgeVector).Returns(new BitArray(0));
            GreedyBasis basis = new GreedyBasis(2, 0);
            Assert.IsTrue(basis.Members.Count == 0);
            basis.Add(c1.Object);
            Assert.IsTrue(basis.Members.Contains(c1.Object));
            basis.Add(c2.Object);
            Assert.IsTrue(basis.Members.Contains(c1.Object));
            Assert.IsTrue(basis.Members.Contains(c2.Object));
        }

        [TestMethod()]
        public virtual void AddAll()
        {
            var c1 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c2 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            c1.SetupGet(c => c.EdgeVector).Returns(new BitArray(0));
            c1.SetupGet(c => c.EdgeVector).Returns(new BitArray(0));
            c2.SetupGet(c => c.EdgeVector).Returns(new BitArray(0));
            GreedyBasis basis = new GreedyBasis(2, 0);
            Assert.IsTrue(basis.Members.Count == 0);
            basis.AddAll(new[] { c1.Object, c2.Object });
			Assert.IsTrue(basis.Members.Contains(c1.Object));
            Assert.IsTrue(basis.Members.Contains(c2.Object));
        }

        [TestMethod()]
		[ExpectedException(typeof(NotSupportedException))]
        public virtual void UnmodifiableMembers()
        {
            var c1 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            c1.SetupGet(c => c.EdgeVector).Returns(new BitArray(0));
            GreedyBasis basis = new GreedyBasis(2, 0);
            basis.Members.Add(c1.Object);
        }

        [TestMethod()]
        public virtual void SubSetOfBasis()
        {
            var c1 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c2 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c3 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            c1.SetupGet(c => c.EdgeVector).Returns(BitArrays.FromString("111000000000"));
            c2.SetupGet(c => c.EdgeVector).Returns(BitArrays.FromString("000111000000"));	// fixed CDK's bug
            c3.SetupGet(c => c.EdgeVector).Returns(BitArrays.FromString("011110000000"));
            c1.SetupGet(c => c.Length).Returns(3);
            c2.SetupGet(c => c.Length).Returns(3);
            c3.SetupGet(c => c.Length).Returns(4);
            GreedyBasis basis = new GreedyBasis(3, 12);
            Assert.IsFalse(basis.IsSubsetOfBasis(c3.Object));
            basis.Add(c1.Object);
            Assert.IsFalse(basis.IsSubsetOfBasis(c3.Object));
            basis.Add(c2.Object);
            Assert.IsTrue(basis.IsSubsetOfBasis(c3.Object));

        }

        [TestMethod()]
        public virtual void Independence()
        {
            var c1 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c2 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c3 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            c1.SetupGet(c => c.EdgeVector).Returns(BitArrays.FromString("111000000000"));
            c2.SetupGet(c => c.EdgeVector).Returns(BitArrays.FromString("000111000000"));
            c3.SetupGet(c => c.EdgeVector).Returns(BitArrays.FromString("111111000000"));
            c1.SetupGet(c => c.Length).Returns(3);
            c2.SetupGet(c => c.Length).Returns(3);
            c3.SetupGet(c => c.Length).Returns(6);
            GreedyBasis basis = new GreedyBasis(3, 12);
            Assert.IsTrue(basis.IsIndependent(c1.Object));
            Assert.IsTrue(basis.IsIndependent(c2.Object));
            Assert.IsTrue(basis.IsIndependent(c3.Object));
            basis.Add(c1.Object);
            Assert.IsTrue(basis.IsIndependent(c2.Object));
            Assert.IsTrue(basis.IsIndependent(c2.Object));
            basis.Add(c2.Object);
            Assert.IsFalse(basis.IsIndependent(c3.Object));
        }

        [TestMethod()]
        public virtual void Size()
        {
            GreedyBasis basis = new GreedyBasis(3, 12);
            Assert.AreEqual(0, basis.Count);
            var c1 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c2 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            var c3 = new Mock<Cycle>((InitialCycles)null, (ShortestPaths)null, (int[])null);
            c1.SetupGet(c => c.EdgeVector).Returns(BitArrays.FromString("111000000000"));
            c2.SetupGet(c => c.EdgeVector).Returns(BitArrays.FromString("000111000000"));
            c3.SetupGet(c => c.EdgeVector).Returns(BitArrays.FromString("111111000000"));
            basis.Add(c1.Object);
            Assert.AreEqual(1, basis.Count);
            basis.Add(c2.Object);
            Assert.AreEqual(2, basis.Count);
            basis.Add(c3.Object);
            Assert.AreEqual(3, basis.Count);
        }
    }
}