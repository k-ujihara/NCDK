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
    /**
     * @author John May
     * @cdk.module test-hash
     */
    [TestClass()]
    public class MinimumEquivalentCyclicSetTest
    {

        /**
         * @cdk.inchi InChI=1S/C8H16/c1-7-3-5-8(2)6-4-7/h7-8H,3-6H2,1-2H3
         */
        [TestMethod()]
        public void TestFind_OneChoice()
        {

            IAtomContainer dummy = new Mock<IAtomContainer>().Object;
            int[][] g = new int[][] { new[] { 1, 5, 6 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4, 7 }, new[] { 3, 5 }, new[] { 0, 4 }, new[] { 0 }, new[] { 3 } };

            // this mock the invariants
            long[] values = new long[] { 1, 0, 0, 1, 0, 0, 2, 2 };

            EquivalentSetFinder finder = new MinimumEquivalentCyclicSet();
            var set = finder.Find(values, dummy, g);

            Assert.AreEqual(2, set.Count);
            Assert.IsTrue(set.Contains(0));
            Assert.IsTrue(set.Contains(3));

        }

        /**
         * @cdk.inchi InChI=1S/C24H36/c1-2-13(1)19-20(14-3-4-14)22(16-7-8-16)24(18-11-12-18)23(17-9-10-17)21(19)15-5-6-15/h13-24H,1-12H2
         */
        [TestMethod()]
        public void TestFind_TwoChoices()
        {

            IAtomContainer dummy = new Mock<IAtomContainer>().Object;
            int[][] g = new int[][]{ new[] {1, 5, 6}, new[] {0, 2, 8}, new[] {1, 3, 9}, new[] {2, 4, 7}, new[] {3, 5, 10}, new[] {0, 4, 11}, new[] {0, 14, 15},
                new[] {3, 20, 21}, new[] {1, 12, 13}, new[] {2, 22, 23}, new[] {4, 18, 19}, new[] {5, 16, 17}, new[] {8, 13}, new[] {8, 12}, new[] {6, 15}, new[] {14, 6},
                new[] {11, 17}, new[] {16, 11}, new[] {10, 19}, new[] {10, 18}, new[] {7, 21}, new[] {7, 20}, new[] {9, 23}, new[] {22, 9}};

            // this mock the invariants
            long[] values = new long[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };

            EquivalentSetFinder finder = new MinimumEquivalentCyclicSet();
            var set = finder.Find(values, dummy, g);

            Assert.AreEqual(6, set.Count);
            Assert.IsTrue(set.Contains(0));
            Assert.IsTrue(set.Contains(1));
            Assert.IsTrue(set.Contains(2));
            Assert.IsTrue(set.Contains(3));
            Assert.IsTrue(set.Contains(4));
            Assert.IsTrue(set.Contains(5));

            // invert values, we should now get the other set
            values = new long[] { 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };

            set = finder.Find(values, dummy, g);

            Assert.AreEqual(6, set.Count);
            Assert.IsTrue(set.Contains(6));
            Assert.IsTrue(set.Contains(7));
            Assert.IsTrue(set.Contains(8));
            Assert.IsTrue(set.Contains(9));
            Assert.IsTrue(set.Contains(10));
            Assert.IsTrue(set.Contains(11));
        }

        /**
         * @cdk.inchi InChI=1S/C10H22/c1-7(2)10(8(3)4)9(5)6/h7-10H,1-6H3
         */
        [TestMethod()]
        public void TestFind_NoChoice()
        {

            IAtomContainer dummy = new Mock<IAtomContainer>().Object;
            int[][] g = new int[][] { new[] { 1, 2, 3 }, new[] { 0, 4, 9 }, new[] { 0, 5, 6 }, new[] { 0, 7, 8 }, new[] { 1 }, new[] { 2 }, new[] { 2 }, new[] { 3 }, new[] { 3 }, new[] { 1 } };

            // this mock the invariants
            long[] values = new long[] { 1, 2, 2, 2, 3, 3, 3, 3, 3, 3 };

            EquivalentSetFinder finder = new MinimumEquivalentCyclicSet();
            var set = finder.Find(values, dummy, g);

            Assert.AreEqual(0, set.Count);
        }
    }
}
