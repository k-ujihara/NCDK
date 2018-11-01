/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NCDK.QSAR.Results
{
    // @cdk.module test-standard
    [TestClass()]
    public class DoubleArrayResultTest : CDKTestCase
    {
        public DoubleArrayResultTest()
            : base()
        { }

        [TestMethod()]
        public void TestDoubleArrayResultType()
        {
            IDescriptorResult type = new ArrayResult<double>(6);
            Assert.IsNotNull(type);
        }

        [TestMethod()]
        public void TestLength()
        {
            Assert.AreEqual(7, new ArrayResult<double>(7).Length);
        }

        [TestMethod()]
        public void TestDoubleArrayResult_int()
        {
            ArrayResult<double> result = new ArrayResult<double>(5);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
        }

        [TestMethod()]
        public void TestDoubleArrayResult()
        {
            ArrayResult<double> result = new ArrayResult<double>();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod()]
        public void TestSize()
        {
            ArrayResult<double> result = new ArrayResult<double>();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
            result.Add(5);
            Assert.AreEqual(1, result.Length);
        }

        [TestMethod()]
        public void TestAdd_Double()
        {
            ArrayResult<double> result = new ArrayResult<double>();
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.ToString());
            result.Add(5);
            result.Add(2);
            result.Add(-3);
            Assert.AreEqual(3, result.Length);
        }

        [TestMethod()]
        public void TestToString()
        {
            ArrayResult<double> result = new ArrayResult<double>();
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.ToString());
            result.Add(5);
            Assert.IsTrue(new[] { "5.0", "5" }.ToReadOnlyList().Contains(result.ToString()));
            result.Add(2);
            Assert.IsTrue(new[] { "5.0,2.0", "5,2" }.ToReadOnlyList().Contains(result.ToString()));
            result.Add(-3);
            Assert.IsTrue(new[] { "5.0,2.0,-3.0", "5,2,-3" }.ToReadOnlyList().Contains(result.ToString()));
        }

        [TestMethod()]
        public void TestGet_int()
        {
            ArrayResult<double> result = new ArrayResult<double>();
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.ToString());
            result.Add(5);
            Assert.AreEqual(5, result[0], 0.000001);
            result.Add(2);
            Assert.AreEqual(5, result[0], 0.000001);
            Assert.AreEqual(2, result[1], 0.000001);
            result.Add(-1);
            Assert.AreEqual(5, result[0], 0.000001);
            Assert.AreEqual(2, result[1], 0.000001);
            Assert.AreEqual(-1, result[2], 0.000001);
        }
    }
}
