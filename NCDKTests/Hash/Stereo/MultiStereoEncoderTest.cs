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
using System;

namespace NCDK.Hash.Stereo
{
    /**
     * @author John May
     * @cdk.module test-hash
     */
    [TestClass()]
    public class MultiStereoEncoderTest
    {

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_Empty()
        {
            new MultiStereoEncoder(new IStereoEncoder[0]);
        }

        [TestMethod()]
        public void TestEncode()
        {

            var m_a = new Mock<IStereoEncoder>(); var a = m_a.Object;
            var m_b = new Mock<IStereoEncoder>(); var b = m_b.Object;

            IStereoEncoder encoder = new MultiStereoEncoder(new[] { a, b });

            long[] current = new long[5];
            long[] next = new long[5];

            m_a.Setup(n => n.Encode(current, next)).Returns(true);
            m_b.Setup(n => n.Encode(current, next)).Returns(true);

            // configured once
            Assert.IsTrue(encoder.Encode(current, next));

            // not configured again
            Assert.IsFalse(encoder.Encode(current, next));

            m_a.Verify(n => n.Encode(current, next), Times.Exactly(1));
            m_b.Verify(n => n.Encode(current, next), Times.Exactly(1));

        }

        [TestMethod()]
        public void TestReset()
        {
            var m_a = new Mock<IStereoEncoder>(); var a = m_a.Object;
            var m_b = new Mock<IStereoEncoder>(); var b = m_b.Object;

            IStereoEncoder encoder = new MultiStereoEncoder(new[] { a, b });

            long[] current = new long[0];
            long[] next = new long[0];

            m_a.Setup(n => n.Encode(current, next)).Returns(true);
            m_b.Setup(n => n.Encode(current, next)).Returns(true);

            // configured once
            Assert.IsTrue(encoder.Encode(current, next));

            // not configured again
            Assert.IsFalse(encoder.Encode(current, next));

            m_a.Verify(n => n.Encode(current, next), Times.Exactly(1));
            m_b.Verify(n => n.Encode(current, next), Times.Exactly(1));

            encoder.Reset();

            Assert.IsTrue(encoder.Encode(current, next));

            m_a.Verify(n => n.Encode(current, next), Times.Exactly(2));
            m_b.Verify(n => n.Encode(current, next), Times.Exactly(2));
        }
    }
}
