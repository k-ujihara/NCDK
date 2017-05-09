/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Maths
{
    // @cdk.module test-standard
    [TestClass()]
    public class MathToolsTest : CDKTestCase
    {

        public MathToolsTest() :
                base()
        { }

        public void TestMax_arrayDouble()
        {
            double[] doubles = { 2.0, 1.0, 3.0, 5.0, 4.0 };
            Assert.AreEqual(5.0, MathTools.Max(doubles), 0.001);
        }

        [TestMethod()]
        public void TestMin_arrayDouble()
        {
            double[] doubles = { 2.0, 1.0, 3.0, 5.0, 4.0 };
            Assert.AreEqual(1.0, MathTools.Min(doubles), 0.001);
        }

        [TestMethod()]
        public void TestMax_arrayint()
        {
            int[] ints = { 1, 2, 3, 4, 5 };
            Assert.AreEqual(5, MathTools.Max(ints));
        }

        [TestMethod()]
        public void TestMin_arrayint()
        {
            int[] ints = { 1, 2, 3, 4, 5 };
            Assert.AreEqual(1, MathTools.Min(ints));
        }

        [TestMethod()]
        public void TestIsEven_int()
        {
            Assert.IsTrue(MathTools.IsEven(2));
            Assert.IsTrue(MathTools.IsEven(208));
        }

        [TestMethod()]
        public void TestIsOdd_int()
        {
            Assert.IsTrue(MathTools.IsOdd(1));
            Assert.IsTrue(MathTools.IsOdd(3));
            Assert.IsTrue(MathTools.IsOdd(209));
        }
    }
}
