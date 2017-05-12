/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by the
 * Free Software Foundation; either version 2.1 of the License, or (at your
 * option) any later version. All we ask is that proper credit is given for our
 * work, which includes - but is not limited to - adding the above copyright
 * notice to the beginning of your source code files, and to any copyright
 * notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Common.Base;
using NCDK.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.RingSearches
{
    /// <summary>
    /// unit tests for the small to medium graphs
    /// </summary>
    // @author John May
    // @cdk.module test-core    [TestClass()]
    public class RegularCyclicVertexSearchTest
    {
        [TestMethod()]
        public virtual void TestEmpty()
        {
            CyclicVertexSearch search = new RegularCyclicVertexSearch(new int[0][] { });
            Assert.IsNotNull(search);
        }

        [TestMethod()]
        public virtual void TestCyclic()
        {
            // cyclohexane like
            int[][] g = new int[][] { new[] { 5, 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 4, 0 } };
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5 }, search.Cyclic()));
        }

        [TestMethod()]
        public virtual void TestCyclic_Int()
        {
            // cyclohexane like
            int[][] g = new int[][] { new[] { 5, 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 4, 0 } };
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            for (int v = 0; v < g.Length; v++)
                Assert.IsTrue(search.Cyclic(v));
        }

        [TestMethod()]
        public virtual void TestCyclic_IntInt()
        {
            int[][] g = new int[][] { new[] { 5, 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 4, 0, 6 }, new[] { 5 } };
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            Assert.IsTrue(search.Cyclic(0, 1));
            Assert.IsTrue(search.Cyclic(1, 2));
            Assert.IsTrue(search.Cyclic(2, 3));
            Assert.IsTrue(search.Cyclic(3, 4));
            Assert.IsTrue(search.Cyclic(4, 5));
            Assert.IsTrue(search.Cyclic(5, 0));
            Assert.IsFalse(search.Cyclic(5, 6));
        }

        [TestMethod()]
        public virtual void VertexColor()
        {
            // medium size spiro cyclo hexane like
            int[][] g = new int[][]{new[]{1, 5}, new[] {0, 2}, new[] {1, 3}, new[] {2, 4}, new[] {3, 5}, new[] {0, 4, 7, 8}, new[] {7, 10}, new[] {5, 6}, new[] {5, 9}, new[] {8, 10},
             new[]   {6, 9, 12, 13}, new[] {12, 15}, new[] {10, 11}, new[] {10, 14}, new[] {13, 15}, new[] {11, 14, 17, 18}, new[] {17, 20}, new[] {15, 16}, new[] {15, 19},
             new[]   {18, 20}, new[] {16, 19, 22, 23}, new[] {22, 25}, new[] {20, 21}, new[] {20, 24}, new[] {23, 25}, new[] {21, 24, 27, 28}, new[] {27, 30},
             new[]   {25, 26}, new[] {25, 29}, new[] {28, 30}, new[] {26, 29, 32, 33}, new[] {32, 35}, new[] {30, 31}, new[] {30, 34}, new[] {33, 35},
             new[]   {31, 34, 37, 38}, new[] {37, 40}, new[] {35, 36}, new[] {35, 39}, new[] {38, 40}, new[] {36, 39, 42, 43}, new[] {42, 45}, new[] {40, 41},
              new[]  {40, 44}, new[] {43, 45}, new[] {41, 44, 47, 48}, new[] {47, 50}, new[] {45, 46}, new[] {45, 49}, new[] {48, 50}, new[] {46, 49, 52, 53},
             new[]   {52, 55}, new[] {50, 51}, new[] {50, 54}, new[] {53, 55}, new[] {51, 54, 57, 58}, new[] {57, 60}, new[] {55, 56}, new[] {55, 59}, new[] {58, 60},
             new[]   {56, 59}};
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[] colors = search.VertexColor();
        }

        [TestMethod()]
        public virtual void TestIsolated()
        {
            // cyclohexane like
            int[][] g = new int[][] { new[] { 5, 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 4, 0 } };
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new[] { 0, 1, 2, 3, 4, 5 } }, search.Isolated()));
        }

        [TestMethod()]
        public virtual void TestIsolated_NonCyclic()
        {
            int[][] g = new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3 } };
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], search.Cyclic()));
        }

        [TestMethod()]
        public virtual void TestIsolated_Empty()
        {
            CyclicVertexSearch search = new RegularCyclicVertexSearch(Arrays.CreateJagged<int>(0, 0));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], search.Cyclic()));
            Assert.IsTrue(Compares.AreDeepEqual(Arrays.CreateJagged<int>(0, 0), search.Isolated()));
            Assert.IsTrue(Compares.AreDeepEqual(Arrays.CreateJagged<int>(0, 0), search.Fused()));
        }

        /// <summary>
        /// C1CCC2(CC1)CCCCC2
        /// </summary>
        [TestMethod()]
        public virtual void TestIsolated_Spiro()
        {
            // spiro cyclo hexane like
            int[][] g = new int[][] { new[] { 1, 5 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 0, 4, 7, 8 }, new[] { 7, 10 }, new[] { 5, 6 }, new[] { 5, 9 }, new[] { 8, 10 }, new[] { 6, 9 } };
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[][] isolated = search.Isolated();
            Assert.AreEqual(2, isolated.Length);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5 }, isolated[0]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 5, 6, 7, 8, 9, 10 }, isolated[1]));
        }

        /// <summary>
        /// C1CCC2(CC1)CCC1(CC2)CCC2(CC1)CCC1(CC2)CCC2(CC1)CCC1(CC2)CCC2(CCC3(CCC4(CCC5(CCC6(CCCCC6)CC5)CC4)CC3)CC2)CC1
        /// </summary>
        [TestMethod()]
        public virtual void TestIsolated_SpiroMedium()
        {
            // medium size spiro cyclo hexane like
            int[][] g = new int[][]{ new[] {1, 5}, new[] {0, 2}, new[] {1, 3}, new[] {2, 4}, new[] {3, 5}, new[] {0, 4, 7, 8}, new[] {7, 10}, new[] {5, 6}, new[] {5, 9}, new[] {8, 10},
                        new[] {6, 9, 12, 13}, new[] {12, 15}, new[] {10, 11}, new[] {10, 14}, new[] {13, 15}, new[] {11, 14, 17, 18}, new[] {17, 20}, new[] {15, 16}, new[] {15, 19},
                        new[] {18, 20}, new[] {16, 19, 22, 23}, new[] {22, 25}, new[] {20, 21}, new[] {20, 24}, new[] {23, 25}, new[] {21, 24, 27, 28}, new[] {27, 30},
                        new[] {25, 26}, new[] {25, 29}, new[] {28, 30}, new[] {26, 29, 32, 33}, new[] {32, 35}, new[] {30, 31}, new[] {30, 34}, new[] {33, 35},
                        new[] {31, 34, 37, 38}, new[] {37, 40}, new[] {35, 36}, new[] {35, 39}, new[] {38, 40}, new[] {36, 39, 42, 43}, new[] {42, 45}, new[] {40, 41},
                        new[] {40, 44}, new[] {43, 45}, new[] {41, 44, 47, 48}, new[] {47, 50}, new[] {45, 46}, new[] {45, 49}, new[] {48, 50}, new[] {46, 49, 52, 53},
                        new[] {52, 55}, new[] {50, 51}, new[] {50, 54}, new[] {53, 55}, new[] {51, 54, 57, 58}, new[] {57, 60}, new[] {55, 56}, new[] {55, 59}, new[] {58, 60},
                        new[] {56, 59}};
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[][] isolated = search.Isolated();
            Assert.AreEqual(12, isolated.Length);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5 }, isolated[0]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 5, 6, 7, 8, 9, 10 }, isolated[1]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 10, 11, 12, 13, 14, 15 }, isolated[2]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 15, 16, 17, 18, 19, 20 }, isolated[3]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 20, 21, 22, 23, 24, 25 }, isolated[4]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 25, 26, 27, 28, 29, 30 }, isolated[5]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 30, 31, 32, 33, 34, 35 }, isolated[6]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 35, 36, 37, 38, 39, 40 }, isolated[7]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 40, 41, 42, 43, 44, 45 }, isolated[8]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 45, 46, 47, 48, 49, 50 }, isolated[9]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 50, 51, 52, 53, 54, 55 }, isolated[10]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 55, 56, 57, 58, 59, 60 }, isolated[11]));
        }

        [TestMethod()]
        public virtual void TestIsolated_Biphenyl()
        {
            // biphenyl like
            int[][] g = new int[][]{ new[] {5, 1}, new[] {0, 2}, new[] {1, 3}, new[] {2, 4}, new[] {3, 5}, new[] {4, 0, 6}, new[] {11, 7, 5}, new[] {6, 8}, new[] {7, 9}, new[] {8, 10},
                        new[]{9, 11}, new[] {10, 7}};
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, search.Cyclic()));
            int[][] isolated = search.Isolated();
            Assert.AreEqual(2, isolated.Length);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5 }, isolated[0]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 6, 7, 8, 9, 10, 11 }, isolated[1]));
        }

        /// <summary>
        /// C(C1CCCCC1)C1CCCCC1
        /// </summary>
        [TestMethod()]
        public virtual void TestIsolated_BenzylBenzene()
        {
            // benzylbenzene like
            int[][] g = new int[][]{ new[] {1, 5}, new[] {0, 2}, new[] {1, 3}, new[] {2, 4}, new[] {3, 5}, new[] {0, 4, 12}, new[] {7, 11}, new[] {6, 8}, new[] {7, 9, 12},
                        new[] {8, 10}, new[] {9, 11}, new[] {6, 10}, new[] {8, 5}};
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[][] isolated = search.Isolated();
            Assert.AreEqual(2, isolated.Length);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5 }, isolated[0]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 6, 7, 8, 9, 10, 11 }, isolated[1]));
        }

        [TestMethod()]
        public virtual void TestIsolatedFragments()
        {
            // two disconnected cyclohexanes
            int[][] g = new int[][]{ new[] {5, 1}, new[] {0, 2}, new[] {1, 3}, new[] {2, 4}, new[] {3, 5}, new[] {4, 0}, new[] {11, 7}, new[] {6, 8}, new[] {7, 9}, new[] {8, 10},
                        new[] {9, 11}, new[] {10, 7}};
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, search.Cyclic()));
            int[][] isolated = search.Isolated();
            Assert.AreEqual(2, isolated.Length);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5 }, isolated[0]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 6, 7, 8, 9, 10, 11 }, isolated[1]));
        }

        /// <summary>
        /// C1CC2CCC1CC2
        /// </summary>
        [TestMethod()]
        public virtual void TestFused()
        {
            int[][] g = new int[][] { new[] { 1, 5, 6 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4, 7 }, new[] { 3, 5 }, new[] { 0, 4 }, new[] { 0, 7 }, new[] { 6, 3 } };
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[][] isolated = search.Isolated();
            int[][] fused = search.Fused();
            Assert.AreEqual(0, isolated.Length);
            Assert.AreEqual(1, fused.Length);
            Assert.AreEqual(g.Length, fused[0].Length);
        }

        /// <summary>
        /// two fused systems which are edge disjoint with respect to each other but
        /// have a (non cyclic) edge which connects them C1CC2(CCC1CC2)C12CCC(CC1)CC2
        /// </summary>
        [TestMethod()]
        public virtual void TestFused_BiocycloEdgeLinked()
        {
            // biocyclooctanylbiocylooctane like
            int[][] g = new int[][]{new[]{1, 5, 6}, new[] {0, 2}, new[] {1, 3}, new[] {2, 4, 7, 8}, new[] {3, 5}, new[] {0, 4}, new[] {0, 7}, new[] {6, 3},
                        new[]{9, 13, 14, 3}, new[] {8, 10}, new[] {9, 11}, new[] {10, 12, 15}, new[] {11, 13}, new[] {8, 12}, new[] {8, 15}, new[] {11, 14}};
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[][] isolated = search.Isolated();
            int[][] fused = search.Fused();
            Assert.AreEqual(0, isolated.Length);
            Assert.AreEqual(2, fused.Length);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, fused[0]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 8, 9, 10, 11, 12, 13, 14, 15 }, fused[1]));
        }

        /// <summary>
        /// two fused systems which are edge disjoint with respect to each other
        /// however in between the two fused cycle systems there is a single non
        /// cyclic vertex which is adjacent to both C(C12CCC(CC1)CC2)C12CCC(CC1)CC2
        /// </summary>
        [TestMethod()]
        public virtual void TestFused_BiocycloVertexLinked()
        {
            // biocyclooctanylbiocylooctane like
            int[][] g = new int[][]{ new[] {1, 5}, new[] {0, 2}, new[] {1, 3, 6, 16}, new[] {2, 4}, new[] {3, 5}, new[] {4, 0, 7}, new[] {2, 7}, new[] {6, 5}, new[] {9, 13},
                       new[] {8, 10}, new[] {9, 11, 14}, new[] {10, 12}, new[] {11, 13}, new[] {12, 8, 15, 16}, new[] {10, 15}, new[] {14, 13}, new[] {13, 2}};
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[][] isolated = search.Isolated();
            int[][] fused = search.Fused();
            Assert.AreEqual(0, isolated.Length);
            Assert.AreEqual(2, fused.Length);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, fused[0]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 8, 9, 10, 11, 12, 13, 14, 15 }, fused[1]));
        }

        /// <summary>
        /// C1CCC2CCCCC2C1
        /// </summary>
        [TestMethod()]
        public virtual void TestFused_OrthoFused()
        {
            // napthalene like
            int[][] g = new int[][] { new[] { 1, 5 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5, 7 }, new[] { 0, 6, 4 }, new[] { 5, 9 }, new[] { 4, 8 }, new[] { 7, 9 }, new[] { 6, 8 } };
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[][] isolated = search.Isolated();
            int[][] fused = search.Fused();
            Assert.AreEqual(0, isolated.Length);
            Assert.AreEqual(1, fused.Length);
            Assert.AreEqual(g.Length, fused[0].Length);
        }

        /// <summary>
        /// C1CCC2CC3CCCCC3CC2C1
        /// </summary>
        [TestMethod()]
        public virtual void TestFused_BiorthoFused()
        {
            // 3 fused rings
            int[][] g = new int[][]{ new[] {1, 5}, new[] {0, 2, 10}, new[] {3, 13, 1}, new[] {2, 4}, new[] {3, 5, 7}, new[] {0, 6, 4}, new[] {5, 9}, new[] {4, 8}, new[] {7, 9},
                        new[] {6, 8}, new[] {1, 11}, new[] {10, 12}, new[] {11, 13}, new[] {2, 12}};
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[][] isolated = search.Isolated();
            int[][] fused = search.Fused();
            Assert.AreEqual(0, isolated.Length);
            Assert.AreEqual(1, fused.Length);
            Assert.AreEqual(g.Length, fused[0].Length);
        }

        /// <summary>
        /// C1CC23CCC4(CC2)CCC2(CCCC5(CCCC6(CCC7(CCCC8(CCC9(CC8)CCC8(CCCC%10(CCCC%11(CCC(C1)(CC%11)C3)C%10)C8)CC9)C7)CC6)C5)C2)CC4
        /// </summary>
        [TestMethod()]
        public virtual void TestFused_Cylclophane()
        {
            // medium size spiro cyclophane
            int[][] g = new int[][]{ new[] {1, 5}, new[] {0, 2}, new[] {1, 3, 50, 46}, new[] {2, 4}, new[] {3, 5}, new[] {0, 4, 7, 8}, new[] {7, 10}, new[] {5, 6}, new[] {5, 9},
                        new[] {8, 10}, new[] {6, 9, 12, 13}, new[] {12, 15}, new[] {10, 11}, new[] {10, 14}, new[] {13, 15, 16, 17}, new[] {11, 14}, new[] {14, 20}, new[] {14, 18},
                        new[] {17, 19}, new[] {18, 20, 21, 22}, new[] {16, 19}, new[] {19, 25}, new[] {19, 23}, new[] {22, 24, 26, 30}, new[] {23, 25}, new[] {21, 24},
                        new[] {23, 27}, new[] {26, 28, 35, 31}, new[] {27, 29}, new[] {28, 30}, new[] {23, 29}, new[] {27, 32}, new[] {31, 33}, new[] {32, 34, 40, 36},
                        new[] {33, 35}, new[] {27, 34}, new[] {33, 37}, new[] {36, 38}, new[] {37, 39, 45, 41}, new[] {38, 40}, new[] {33, 39}, new[] {38, 42},
                        new[] {41, 43, 58, 59}, new[] {42, 44}, new[] {43, 45}, new[] {38, 44}, new[] {2, 47}, new[] {46, 48}, new[] {47, 49}, new[] {48, 50, 51, 55}, new[] {2, 49},
                        new[] {49, 52}, new[] {51, 53}, new[] {52, 54}, new[] {53, 55, 56, 57}, new[] {49, 54}, new[] {54, 59}, new[] {54, 58}, new[] {42, 57}, new[] {42, 56}};
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[][] isolated = search.Isolated();
            int[][] fused = search.Fused();
            Assert.AreEqual(0, isolated.Length);
            Assert.AreEqual(1, fused.Length);
            Assert.AreEqual(g.Length, fused[0].Length);
        }

        /// <summary>
        /// CHEBI:33128
        /// </summary>
        [TestMethod()]
        public virtual void TestFused_Fullerene()
        {
            int[][] g = new int[][]{ new[] {1, 4, 8}, new[] {0, 2, 11}, new[] {1, 3, 14}, new[] {2, 4, 17}, new[] {3, 0, 5}, new[] {4, 6, 19}, new[] {5, 7, 21},
                        new[] {6, 8, 24}, new[] {7, 0, 9}, new[] {8, 10, 25}, new[] {9, 11, 28}, new[] {10, 1, 12}, new[] {11, 13, 29}, new[] {12, 14, 32}, new[] {13, 2, 15},
                        new[] {14, 16, 33}, new[] {15, 17, 36}, new[] {16, 3, 18}, new[] {17, 19, 37}, new[] {18, 5, 20}, new[] {19, 21, 39}, new[] {20, 6, 22},
                        new[] {21, 23, 41}, new[] {22, 24, 43}, new[] {23, 7, 25}, new[] {24, 9, 26}, new[] {25, 27, 44}, new[] {26, 28, 46}, new[] {27, 10, 29},
                        new[] {28, 12, 30}, new[] {29, 31, 47}, new[] {30, 32, 49}, new[] {31, 13, 33}, new[] {32, 15, 34}, new[] {33, 35, 50}, new[] {34, 36, 52},
                        new[] {35, 16, 37}, new[] {36, 18, 38}, new[] {37, 39, 53}, new[] {38, 20, 40}, new[] {39, 41, 54}, new[] {40, 22, 42}, new[] {41, 43, 56},
                        new[] {42, 23, 44}, new[] {43, 26, 45}, new[] {44, 46, 57}, new[] {45, 27, 47}, new[] {46, 30, 48}, new[] {47, 49, 58}, new[] {48, 31, 50},
                        new[] {49, 34, 51}, new[] {50, 52, 59}, new[] {51, 35, 53}, new[] {52, 38, 54}, new[] {53, 40, 55}, new[] {54, 56, 59}, new[] {55, 42, 57},
                        new[] {56, 45, 58}, new[] {57, 48, 59}, new[] {58, 51, 55}};
            CyclicVertexSearch search = new RegularCyclicVertexSearch(g);
            int[][] isolated = search.Isolated();
            int[][] fused = search.Fused();
            Assert.AreEqual(0, isolated.Length);
            Assert.AreEqual(1, fused.Length);
            Assert.AreEqual(g.Length, fused[0].Length);
        }

        [TestMethod()]
        public virtual void TestToArray_Empty()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], RegularCyclicVertexSearch.ToArray(0L)));
        }

        [TestMethod()]
        public virtual void TestToArray_Singleton()
        {
            for (int i = 0; i < 62; i++)
            {
                Assert.IsTrue(Compares.AreDeepEqual(new int[] { i }, RegularCyclicVertexSearch.ToArray(Pow(2L, i))));
            }
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 63 }, RegularCyclicVertexSearch.ToArray(long.MinValue)));
        }

        [TestMethod()]
        public virtual void TestSetBit()
        {
            for (int i = 0; i < 62; i++)
            {
                Assert.AreEqual(Math.Pow(2L, i), RegularCyclicVertexSearch.SetBit(0L, i));
            }
            Assert.AreEqual(long.MinValue, RegularCyclicVertexSearch.SetBit(0L, 63));
        }

        [TestMethod()]
        public virtual void TestSetBit_Universe()
        {
            long s = 0L;
            long t = ~s;
            for (int i = 0; i < 64; i++)
            {
                s = RegularCyclicVertexSearch.SetBit(s, i);
            }
            Assert.AreEqual(t, s);
        }

        [TestMethod()]
        public virtual void TestIsBitSet_Empty()
        {
            long s = 0L;
            for (int i = 0; i < 64; i++)
            {
                Assert.IsFalse(RegularCyclicVertexSearch.IsBitSet(s, i));
            }
        }

        [TestMethod()]
        public virtual void TestIsBitSet_Universe()
        {
            long s = ~0L;
            for (int i = 0; i < 64; i++)
            {
                Assert.IsTrue(RegularCyclicVertexSearch.IsBitSet(s, i));
            }
        }

        [TestMethod()]
        public virtual void TestIsBitSet_Singleton()
        {
            long s = 1L;
            Assert.IsTrue(RegularCyclicVertexSearch.IsBitSet(s, 0));
            for (int i = 1; i < 64; i++)
            {
                Assert.IsFalse(RegularCyclicVertexSearch.IsBitSet(s, i));
            }
        }

        [TestMethod()]
        public virtual void TestIsBitSet()
        {
            for (int i = 0; i < 62; i++)
            {
                Assert.IsTrue(RegularCyclicVertexSearch.IsBitSet(Pow(2L, i), i));
            }
            Assert.IsTrue(RegularCyclicVertexSearch.IsBitSet(long.MinValue, 63));
        }

        static long Pow(long val, int pow)
        {
            return (long)Math.Pow(val, pow);
        }
    }
}

