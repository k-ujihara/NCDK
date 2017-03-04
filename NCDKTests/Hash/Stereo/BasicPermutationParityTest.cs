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
using NCDK.Hash.Stereo;
using System;

namespace NCDK.Hash.Stereo
{
    /// <summary>
    // @author John May
    // @cdk.module test-hash
    /// </summary>
     [TestClass()]
    public class BasicPermutationParityTest
    {

        BasicPermutationParity permutationParity = new BasicPermutationParity(new int[] { 0, 1, 2, 3 });

        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TestConstruction_Null()
        {
            new BasicPermutationParity(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_Empty()
        {
            new BasicPermutationParity(new int[0]);
        }

        [TestMethod()]
        public void TestParity_Even()
        {
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 4, 3, 2, 1 }));
        }

        [TestMethod()]
        public void TestParity_Odd()
        {
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 4, 2, 3, 1 }));
        }

        [TestMethod()]
        public void TestParity_Even_Negative()
        {
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 4, 3, -1, -2 }));
        }

        [TestMethod()]
        public void TestParity_Odd_Negative()
        {
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 4, -1, 3, -2 }));
        }

        [TestMethod()]
        public void TestParity_Duplicate()
        {
            Assert.AreEqual(0, permutationParity.Parity(new long[] { 4, 3, -1, -1 }));
        }

        [TestMethod()]
        public void TestParity_All()
        {
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 1, 2, 3, 4 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 2, 1, 3, 4 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 1, 3, 2, 4 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 3, 1, 2, 4 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 2, 3, 1, 4 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 3, 2, 1, 4 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 1, 2, 4, 3 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 2, 1, 4, 3 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 1, 4, 2, 3 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 4, 1, 2, 3 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 2, 4, 1, 3 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 4, 2, 1, 3 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 1, 3, 4, 2 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 3, 1, 4, 2 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 1, 4, 3, 2 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 4, 1, 3, 2 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 3, 4, 1, 2 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 4, 3, 1, 2 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 2, 3, 4, 1 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 3, 2, 4, 1 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 2, 4, 3, 1 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 4, 2, 3, 1 }));
            Assert.AreEqual(-1, permutationParity.Parity(new long[] { 3, 4, 2, 1 }));
            Assert.AreEqual(1, permutationParity.Parity(new long[] { 4, 3, 2, 1 }));
        }
    }
}
