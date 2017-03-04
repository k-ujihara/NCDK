/*
 * Copyright (c) 2013. John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NCDK.Hash
{
    /// <summary>
    // @author John May
    // @cdk.module test-hash
    /// </summary>
    [TestClass()]
    public class AbstractHashGeneratorTest
    {

        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TestConstruction_Null()
        {
            new AbstractHashGenerator(null);
        }

        [TestMethod()]
        public void TestCopy()
        {
            long[] x = new long[] { 2, 1, 3, 2 };
            long[] y = AbstractHashGenerator.Copy(x);
            Assert.IsTrue(Common.Base.Compares.AreDeepEqual(x, y));
            Assert.AreNotSame(x, y);
        }

        [TestMethod()]
        public void TestCopy_SrcDest()
        {
            long[] x = new long[] { 42, 23, 1, 72 };
            long[] y = new long[4];
            AbstractHashGenerator.Copy(x, y);
            Assert.IsTrue(Common.Base.Compares.AreDeepEqual(x, y));
            Assert.AreNotSame(x, y);
        }

        [TestMethod()]
        public void TestRotate()
        {
            var m_pseudorandom = new Mock<Pseudorandom>();
            AbstractHashGenerator f = new AbstractHashGenerator(m_pseudorandom.Object);
            f.Rotate(5L);
            m_pseudorandom.Verify(n => n.Next(5L), Times.Exactly(1));
        }

        [TestMethod()]
        public void TestRotate_N()
        {
            var m_pseudorandom = new Mock<Pseudorandom>();
            AbstractHashGenerator f = new AbstractHashGenerator(m_pseudorandom.Object);
            f.Rotate(0, 5); // note 0 doesn't rotate..
            m_pseudorandom.Verify(n => n.Next(0), Times.Exactly(5));
        }

        [TestMethod()]
        public void TestLowestThreeBits()
        {
            Assert.AreEqual(0, AbstractHashGenerator.LowestThreeBits(0L));
            Assert.AreEqual(1, AbstractHashGenerator.LowestThreeBits(1L));
            Assert.AreEqual(2, AbstractHashGenerator.LowestThreeBits(2L));
            Assert.AreEqual(3, AbstractHashGenerator.LowestThreeBits(3L));
            Assert.AreEqual(4, AbstractHashGenerator.LowestThreeBits(4L));
            Assert.AreEqual(5, AbstractHashGenerator.LowestThreeBits(5L));
            Assert.AreEqual(6, AbstractHashGenerator.LowestThreeBits(6L));
            Assert.AreEqual(7, AbstractHashGenerator.LowestThreeBits(7L));

            // check we don't exceed 7
            Assert.AreEqual(0, AbstractHashGenerator.LowestThreeBits(8L));
            Assert.AreEqual(1, AbstractHashGenerator.LowestThreeBits(9L));
            Assert.AreEqual(2, AbstractHashGenerator.LowestThreeBits(10L));
            Assert.AreEqual(3, AbstractHashGenerator.LowestThreeBits(11L));
            Assert.AreEqual(4, AbstractHashGenerator.LowestThreeBits(12L));
            Assert.AreEqual(5, AbstractHashGenerator.LowestThreeBits(13L));
            Assert.AreEqual(6, AbstractHashGenerator.LowestThreeBits(14L));
            Assert.AreEqual(7, AbstractHashGenerator.LowestThreeBits(15L));
            Assert.AreEqual(0, AbstractHashGenerator.LowestThreeBits(16L));

            // max/min numbers
            Assert.AreEqual(7, AbstractHashGenerator.LowestThreeBits(long.MaxValue));
            Assert.AreEqual(0, AbstractHashGenerator.LowestThreeBits(long.MinValue));
        }

        [TestMethod()]
        public void TestDistribute_AtLeastOnce()
        {
            var m_pseudorandom = new Mock<Pseudorandom>();
            AbstractHashGenerator f = new AbstractHashGenerator(m_pseudorandom.Object);
            long x = f.Distribute(8L); // lowest 3 bits = 0, make sure we rotate 1
            m_pseudorandom.Verify(n => n.Next(It.IsAny<long>()), Times.Exactly(1));
            Assert.AreNotEqual(8L, x);
        }

        [TestMethod()]
        public void TestDistribute()
        {
            var m_pseudorandom = new Mock<Pseudorandom>();
            AbstractHashGenerator f = new AbstractHashGenerator(m_pseudorandom.Object);
            long x = f.Distribute(5L); // lowest 3 bits = 5, rotate 6 times
            m_pseudorandom.Verify(n => n.Next(It.IsAny<long>()), Times.Exactly(6));
            Assert.AreNotEqual(5L, x);
        }

        [TestMethod()]
        public void TestToAdjList()
        {
            // already tests in ShortestPaths... this method be moved once all
            // pending patches are merged
        }
    }
}
