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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Groups
{
    /// <summary>
    // @author maclean
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class DisjointSetForestTest
    {
        [TestMethod()]
        public void ConstructorTest()
        {
            int n = 10;
            DisjointSetForest forest = new DisjointSetForest(n);
            Assert.IsNotNull(forest);
        }

        [TestMethod()]
        public void GetTest()
        {
            int n = 10;
            DisjointSetForest forest = new DisjointSetForest(n);
            for (int i = 0; i < n; i++)
            {
                Assert.AreEqual(-1, forest[i]);
            }
        }

        [TestMethod()]
        public void GetRootTest()
        {
            int n = 2;
            DisjointSetForest forest = new DisjointSetForest(n);
            forest.MakeUnion(0, 1);
            Assert.AreEqual(0, forest.GetRoot(1));
        }

        [TestMethod()]
        public void MakeUnionTest()
        {
            int n = 2;
            DisjointSetForest forest = new DisjointSetForest(n);
            forest.MakeUnion(0, 1);
            Assert.AreEqual(0, forest[1]);
        }

        [TestMethod()]
        public void GetSetsTest()
        {
            int n = 6;
            DisjointSetForest forest = new DisjointSetForest(n);
            forest.MakeUnion(0, 1);
            forest.MakeUnion(2, 3);
            forest.MakeUnion(4, 5);
            int[][] sets = forest.GetSets();
            int[][] expected = new int[][] { new[] { 0, 1 }, new[] { 2, 3 }, new[] { 4, 5 } };
            string failMessage = "Expected " + Arrays.DeepToString(expected) + " but was " + Arrays.DeepToString(sets);
            Assert.IsTrue(Arrays.DeepEquals(expected, sets), failMessage);
        }
    }
}
