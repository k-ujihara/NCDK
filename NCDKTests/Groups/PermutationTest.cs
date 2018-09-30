/* Copyright (C) 2012  Gilleain Torrance <gilleain.torrance@gmail.com>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using System;
using System.Linq;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module test-group
    [TestClass()]
    public class PermutationTest : CDKTestCase
    {
        [TestMethod()]
        public void SizeNConstructor()
        {
            int size = 4;
            var p = new Permutation(size);
            for (int index = 0; index < size; index++)
            {
                Assert.AreEqual(index, p[index]);
            }
        }

        [TestMethod()]
        public void ValuesConstructor()
        {
            int[] values = new int[] { 1, 0, 3, 2 };
            var p = new Permutation(values);
            for (int index = 0; index < p.Count; index++)
            {
                Assert.AreEqual(values[index], p[index]);
            }
        }

        [TestMethod()]
        public void CloneConstructor()
        {
            int[] values = new int[] { 1, 0, 3, 2 };
            var a = new Permutation(values);
            var b = new Permutation(a);
            Assert.AreEqual(a, b);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            var a = new Permutation(1, 2, 0, 3);
            var b = new Permutation(1, 2, 0, 3);
            Assert.AreEqual(a, b);
        }

        [TestMethod()]
        public void EqualsTest_null()
        {
            var a = new Permutation(1, 2, 0, 3);
            Assert.AreNotSame(a, null);
        }

        [TestMethod()]
        public void EqualsTest_difference()
        {
            var a = new Permutation(1, 2, 0, 3);
            var b = new Permutation(1, 0, 2, 3);
            Assert.AreNotSame(a, b);
        }

        [TestMethod()]
        public void IsIdentityTest()
        {
            int size = 4;
            var p = new Permutation(size);
            Assert.IsTrue(p.IsIdentity());
        }

        [TestMethod()]
        public void SizeTest()
        {
            int size = 4;
            var p = new Permutation(size);
            Assert.AreEqual(size, p.Count);
        }

        [TestMethod()]
        public void GetTest()
        {
            var p = new Permutation(1, 0);
            Assert.AreEqual(1, p[0]);
        }

        [TestMethod()]
        public void GetValuesTest()
        {
            int[] values = new int[] { 1, 0, 3, 2 };
            var p = new Permutation(values);
            Assert.IsTrue(Compares.AreDeepEqual(values, p.Values));
        }

        [TestMethod()]
        public void FirstIndexDiffTest()
        {
            int[] valuesA = new int[] { 1, 0, 3, 2 };
            int[] valuesB = new int[] { 1, 0, 2, 3 };
            var a = new Permutation(valuesA);
            var b = new Permutation(valuesB);
            Assert.AreEqual(2, a.FirstIndexOfDifference(b));
        }

        [TestMethod()]
        public void GetOrbitTest()
        {
            var p = new Permutation(4, 6, 1, 3, 2, 5, 0);
            var orbit = p.GetOrbit(1);
            Assert.AreEqual(5, orbit.Count);
            Assert.IsTrue(orbit.Contains(1));
        }

        [TestMethod()]
        public void SetTest()
        {
            var p = new Permutation(1, 0);
            p[0] = 0;
            p[1] = 1;
            Assert.AreEqual(0, p[0]);
            Assert.AreEqual(1, p[1]);
        }

        [TestMethod()]
        public void SetToTest()
        {
            int[] values = new int[] { 1, 0, 3, 2 };
            var a = new Permutation(values);
            var b = new Permutation(values.Length);
            a.SetTo(b);
            Assert.IsTrue(a.IsIdentity());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void SetToTest_differentLength()
        {
            var a = new Permutation(1, 0, 2);
            var b = new Permutation(0, 1);
            a.SetTo(b);
        }

        [TestMethod()]
        public void MultiplyTest()
        {
            int[] valuesA = new int[] { 1, 0, 2, 3 };
            int[] valuesB = new int[] { 0, 1, 3, 2 };
            int[] expectC = new int[] { 1, 0, 3, 2 };
            var a = new Permutation(valuesA);
            var b = new Permutation(valuesB);
            var c = new Permutation(expectC);
            Assert.AreEqual(c, a.Multiply(b));
        }

        [TestMethod()]
        public void InvertTest()
        {
            int[] values = new int[] { 3, 1, 0, 2 };
            int[] invert = new int[] { 2, 1, 3, 0 };
            var p = new Permutation(values);
            var invP = new Permutation(invert);
            Assert.AreEqual(invP, p.Invert());
        }

        [TestMethod()]
        public void ToCycleStringTest()
        {
            int[] values = new int[] { 0, 2, 1, 4, 5, 3, 7, 8, 9, 6 };
            string expected = "(0)(1, 2)(3, 4, 5)(6, 7, 8, 9)";
            var p = new Permutation(values);
            Assert.AreEqual(expected, p.ToCycleString());
        }
    }
}
