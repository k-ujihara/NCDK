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

namespace NCDK.QSAR.Results
{
    // @cdk.module test-standard
    [TestClass()]
    public class IntegerArrayResultTest : CDKTestCase
    {
        public IntegerArrayResultTest()
            : base()
        { }

        [TestMethod()]
        public void TestIntegerArrayResultType()
        {
            IDescriptorResult type = new ArrayResult<int>(6);
            Assert.IsNotNull(type);
        }

        [TestMethod()]
        public void TestLength()
        {
            Assert.AreEqual(7, new ArrayResult<int>(7).Length);
        }

        [TestMethod()]
        public void IntegerArrayResult_int()
        {
            ArrayResult<int> result = new ArrayResult<int>(5);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
        }

        [TestMethod()]
        public void TestIntegerArrayResult()
        {
            ArrayResult<int> result = new ArrayResult<int>();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod()]
        public void TestAdd_int()
        {
            ArrayResult<int> result = new ArrayResult<int>();
            Assert.IsNotNull(result);
            result.Add(5);
            result.Add(5);
            result.Add(5);
            result.Add(5);
            result.Add(5);
            Assert.AreEqual(5, result.Length);
        }

        [TestMethod()]
        public void TestSize()
        {
            ArrayResult<int> result = new ArrayResult<int>();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
            result.Add(5);
            Assert.AreEqual(1, result.Length);
        }

        [TestMethod()]
        public void TestToString()
        {
            ArrayResult<int> result = new ArrayResult<int>();
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.ToString());
            result.Add(5);
            Assert.AreEqual("5", result.ToString());
            result.Add(2);
            Assert.AreEqual("5,2", result.ToString());
            result.Add(-3);
            Assert.AreEqual("5,2,-3", result.ToString());
        }

        [TestMethod()]
        public void TestGet_int()
        {
            ArrayResult<int> result = new ArrayResult<int>();
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.ToString());
            result.Add(5);
            Assert.AreEqual(5, result[0]);
            result.Add(2);
            Assert.AreEqual(5, result[0]);
            Assert.AreEqual(2, result[1]);
            result.Add(-1);
            Assert.AreEqual(5, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(-1, result[2]);
        }
    }
}
