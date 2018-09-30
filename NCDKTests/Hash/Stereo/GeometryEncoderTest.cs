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

using NCDK.Common.Base;
using NCDK.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace NCDK.Hash.Stereo
{
    // @author John May
    // @cdk.module test-hash
    [TestClass()]
    public class GeometryEncoderTest
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_Empty()
        {
            new GeometryEncoder(Array.Empty<int>(), new Mock<PermutationParity>().Object, new Mock<GeometricParity>().Object);
        }

        [TestMethod()]
        public void TestConstruction_Singleton()
        {
            var m_permutation = new Mock<PermutationParity>(); var permutation = m_permutation.Object;
            var m_geometric = new Mock<GeometricParity>(); var geometric = m_geometric.Object;

            m_permutation.Setup(n => n.Parity(It.IsAny<long[]>())).Returns(-1);
            m_geometric.SetupGet(n => n.Parity).Returns(+1);

            IStereoEncoder encoder = new GeometryEncoder(1, permutation, geometric);
            long[] prev = new long[3];
            long[] result = new long[3];
            Arrays.Fill(prev, 1);
            Arrays.Fill(result, 1);

            // check returned true
            Assert.IsTrue(encoder.Encode(prev, result));

            // check only the value at index '1' was changed
            Assert.IsTrue(Compares.AreDeepEqual(new long[] { 1, 15543053, 1 }, result));
        }

        [TestMethod()]
        public void TestEncode_Clockwise()
        {

            var m_permutation = new Mock<PermutationParity>(); var permutation = m_permutation.Object;
            var m_geometric = new Mock<GeometricParity>(); var geometric = m_geometric.Object;

            m_permutation.Setup(n => n.Parity(It.IsAny<long[]>())).Returns(-1);
            m_geometric.SetupGet(n => n.Parity).Returns(+1);

            IStereoEncoder encoder = new GeometryEncoder(new int[] { 1 }, permutation, geometric);
            long[] prev = new long[3];
            long[] result = new long[3];
            Arrays.Fill(prev, 1);
            Arrays.Fill(result, 1);

            // check returned true
            Assert.IsTrue(encoder.Encode(prev, result));

            // check only the value at index '1' was changed
            Assert.IsTrue(Compares.AreDeepEqual(new long[] { 1, 15543053, 1 }, result));
        }

        [TestMethod()]
        public void TestEncode_Clockwise_Alt()
        {
            var m_permutation = new Mock<PermutationParity>(); var permutation = m_permutation.Object;
            var m_geometric = new Mock<GeometricParity>(); var geometric = m_geometric.Object;

            m_permutation.Setup(n => n.Parity(It.IsAny<long[]>())).Returns(+1);
            m_geometric.SetupGet(n => n.Parity).Returns(-1);

            IStereoEncoder encoder = new GeometryEncoder(new int[] { 1 }, permutation, geometric);
            long[] prev = new long[3];
            long[] result = new long[3];
            Arrays.Fill(prev, 1);
            Arrays.Fill(result, 1);

            // check returned true
            Assert.IsTrue(encoder.Encode(prev, result));

            // check only the value at index '1' was changed
            Assert.IsTrue(Compares.AreDeepEqual(new long[] { 1, 15543053, 1 }, result));
        }

        [TestMethod()]
        public void TestEncode_Clockwise_Two()
        {
            var m_permutation = new Mock<PermutationParity>(); var permutation = m_permutation.Object;
            var m_geometric = new Mock<GeometricParity>(); var geometric = m_geometric.Object;

            m_permutation.Setup(n => n.Parity(It.IsAny<long[]>())).Returns(-1);
            m_geometric.SetupGet(n => n.Parity).Returns(+1);

            IStereoEncoder encoder = new GeometryEncoder(new int[] { 1, 3 }, permutation, geometric);
            long[] prev = new long[6];
            long[] result = new long[6];
            Arrays.Fill(prev, 1);
            Arrays.Fill(result, 1);

            // check returned true
            Assert.IsTrue(encoder.Encode(prev, result));

            // check only the value at index '1' was changed
            Assert.IsTrue(Compares.AreDeepEqual(new long[] { 1, 15543053, 1, 15543053, 1, 1 }, result));
        }

        [TestMethod()]
        public void TestEncode_Anticlockwise()
        {
            var m_permutation = new Mock<PermutationParity>(); var permutation = m_permutation.Object;
            var m_geometric = new Mock<GeometricParity>(); var geometric = m_geometric.Object;

            m_permutation.Setup(n => n.Parity(It.IsAny<long[]>())).Returns(+1);
            m_geometric.SetupGet(n => n.Parity).Returns(+1);

            IStereoEncoder encoder = new GeometryEncoder(new int[] { 1 }, permutation, geometric);
            long[] prev = new long[3];
            long[] result = new long[3];
            Arrays.Fill(prev, 1);
            Arrays.Fill(result, 1);

            // check returned true
            Assert.IsTrue(encoder.Encode(prev, result));

            // check only the value at index '1' was changed
            Assert.IsTrue(Compares.AreDeepEqual(new long[] { 1, 15521419, 1 }, result));
        }

        [TestMethod()]
        public void TestEncode_Anticlockwise_Alt()
        {
            var m_permutation = new Mock<PermutationParity>(); var permutation = m_permutation.Object;
            var m_geometric = new Mock<GeometricParity>(); var geometric = m_geometric.Object;

            m_permutation.Setup(n => n.Parity(It.IsAny<long[]>())).Returns(-1);
            m_geometric.SetupGet(n => n.Parity).Returns(-1);

            IStereoEncoder encoder = new GeometryEncoder(new int[] { 1 }, permutation, geometric);
            long[] prev = new long[3];
            long[] result = new long[3];
            Arrays.Fill(prev, 1);
            Arrays.Fill(result, 1);

            // check returned true
            Assert.IsTrue(encoder.Encode(prev, result));

            // check only the value at index '1' was changed
            Assert.IsTrue(Compares.AreDeepEqual(new long[] { 1, 15521419, 1 }, result));
        }

        [TestMethod()]
        public void TestEncode_Anticlockwise_Two()
        {
            var m_permutation = new Mock<PermutationParity>(); var permutation = m_permutation.Object;
            var m_geometric = new Mock<GeometricParity>(); var geometric = m_geometric.Object;

            m_permutation.Setup(n => n.Parity(It.IsAny<long[]>())).Returns(+1);
            m_geometric.SetupGet(n => n.Parity).Returns(+1);

            IStereoEncoder encoder = new GeometryEncoder(new int[] { 1, 3 }, permutation, geometric);
            long[] prev = new long[6];
            long[] result = new long[6];
            Arrays.Fill(prev, 1);
            Arrays.Fill(result, 1);

            // check returned true
            Assert.IsTrue(encoder.Encode(prev, result));

            // check only the value at index '1' was changed
            Assert.IsTrue(Compares.AreDeepEqual(new long[] { 1, 15521419, 1, 15521419, 1, 1 }, result));
        }

        [TestMethod()]
        public void TestEncode_NoGeometry()
        {
            var m_permutation = new Mock<PermutationParity>(); var permutation = m_permutation.Object;
            var m_geometric = new Mock<GeometricParity>(); var geometric = m_geometric.Object;

            m_permutation.Setup(n => n.Parity(It.IsAny<long[]>())).Returns(+1);
            m_geometric.SetupGet(n => n.Parity).Returns(0);

            IStereoEncoder encoder = new GeometryEncoder(new int[] { 1, 3 }, permutation, geometric);
            long[] prev = new long[6];
            long[] result = new long[6];
            Arrays.Fill(prev, 1);
            Arrays.Fill(result, 1);

            // check returned true. the permutation was okay, but no geometry, this
            // will never change
            Assert.IsTrue(encoder.Encode(prev, result));

            // check no values modified
            Assert.IsTrue(Compares.AreDeepEqual(new long[] { 1, 1, 1, 1, 1, 1 }, result));
        }

        [TestMethod()]
        public void TestEncode_NoPermutation()
        {
            var m_permutation = new Mock<PermutationParity>(); var permutation = m_permutation.Object;
            var m_geometric = new Mock<GeometricParity>(); var geometric = m_geometric.Object;

            m_permutation.Setup(n => n.Parity(It.IsAny<long[]>())).Returns(0);
            m_geometric.SetupGet(n => n.Parity).Returns(+1);

            IStereoEncoder encoder = new GeometryEncoder(new int[] { 1, 3 }, permutation, geometric);
            long[] prev = new long[6];
            long[] result = new long[6];
            Arrays.Fill(prev, 1);
            Arrays.Fill(result, 1);

            // check returned false, the permutation changes for each cycle
            Assert.IsFalse(encoder.Encode(prev, result));

            // check no values modified
            Assert.IsTrue(Compares.AreDeepEqual(new long[] { 1, 1, 1, 1, 1, 1 }, result));

            // geometric parity should not be called
            m_geometric.Verify(n => n.Parity, Times.Never());
        }

        [TestMethod()]
        public void TestReset()
        {
            // no method body to test
        }
    }
}
