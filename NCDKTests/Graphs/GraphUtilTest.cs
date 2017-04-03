/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System;

namespace NCDK.Graphs
{
    [TestClass()]
    public class GraphUtilTest
    {
        [TestMethod()]
        public virtual void SequentialSubgraph()
        {
            int[][] graph = new int[][] { new int[] { 1, 2 }, new int[] { 0, 2 }, new int[] { 0, 1 } };
            int[][] subgraph = GraphUtil.Subgraph(graph, new int[] { 0, 1 });
            int[][] expected = new int[][] { new int[] { 1 }, new int[] { 0 } };
            Assert.IsTrue(Compares.AreDeepEqual(expected, subgraph));
        }

        [TestMethod()]
        public virtual void IntermittentSubgraph()
        {
            int[][]
            graph = new int[][] { new int[] { 1, 2 }, new int[] { 0, 2, 3 }, new int[] { 0, 1 }, new int[] { 1 } };
            int[][] subgraph = GraphUtil.Subgraph(graph, new int[] { 0, 2, 3 });
            int[][] expected = new int[][] { new int[] { 1 }, new int[] { 0 }, new int[] { } };
            Assert.IsTrue(Compares.AreDeepEqual(expected, subgraph));
        }

        [TestMethod()]
        public virtual void ResizeSubgraph()
        {
            int[][] graph = new int[][] {
                new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14},
                new int[] {0}, new int[] {0}, new int[] {0}, new int[] {0},
                new int[] {0}, new int[] {0}, new int[] {0}, new int[] {0},
                new int[] {0}, new int[] {0}, new int[] {0}, new int[]{0},
                new int[] {0}, new int[] {0} };
            int[][] subgraph = GraphUtil.Subgraph(graph, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            int[][] expected = new int[][] {
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                new int[] { 0 }, new int[] { 0 }, new int[] { 0 }, new int[] { 0 },
                new int[] { 0 }, new int[] { 0 }, new int[] { 0 },new int[] { 0 },
                new int[] { 0 }, };
            Assert.IsTrue(Compares.AreDeepEqual(expected, subgraph));
        }

        [TestMethod()]
        public virtual void TestCycle()
        {
            // 0-1-2-3-4-5-
            int[][] g = new int[][] { new int[] { 1, 5 }, new int[] { 0, 2 }, new int[] { 1, 3 }, new int[] { 2, 4 }, new int[] { 3, 5 }, new int[] { 4, 0 } };
            int[] path = GraphUtil.Cycle(g, new int[] { 0, 3, 4, 1, 5, 2 });
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5, 0 }, path));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void TestAcyclic()
        {
            // 0-1-2-3-4-5 (5 and 0 not connected)
            int[][] g = new int[][] { new int[] { 1 }, new int[] { 0, 2 }, new int[] { 1, 3 }, new int[] { 2, 4 }, new int[] { 3, 5 }, new int[] { 4 } };
            GraphUtil.Cycle(g, new int[] { 0, 3, 4, 1, 5, 2 });
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void TestAcyclic2()
        {
            // 0-1-2 3-4-5- (2 and 3) not connected
            int[][] g = new int[][] { new int[] { 1 }, new int[] { 0, 2 }, new int[] { 1 }, new int[] { 4 }, new int[] { 3, 5 }, new int[] { 4 } };
            GraphUtil.Cycle(g, new int[] { 0, 3, 4, 1, 5, 2 });
        }

        [TestMethod()]
        public virtual void FirstMarked()
        {
            var type = new PrivateType(typeof(GraphUtil));
            int actual;
            actual = (int)type.InvokeStatic("FirstMarked", new int[] { 0, 1, 2 }, new bool[] { false, true, false });
            Assert.AreEqual(1, actual);
            actual = (int)type.InvokeStatic("FirstMarked", new int[] { 0, 1, 2 }, new bool[] { true, false, false });
            Assert.AreEqual(0, actual);
            actual = (int)type.InvokeStatic("FirstMarked", new int[] { 0, 1, 2 }, new bool[] { false, false, false });
            Assert.AreEqual(-1, actual);
        }

        [TestMethod()]
        public virtual void TestToAdjList()
        {
            IAtomContainer container = Simple;

            int[][] adjacent = GraphUtil.ToAdjList(container);
            Assert.AreEqual(5, adjacent.Length, "adjacency list should have 5 vertices");

            Assert.AreEqual(1, adjacent[0].Length, "vertex 'a' should have degree 1");
            Assert.AreEqual(3, adjacent[1].Length, "vertex 'b' should have degree 3");
            Assert.AreEqual(2, adjacent[2].Length, "vertex 'c' should have degree 2");
            Assert.AreEqual(1, adjacent[3].Length, "vertex 'd' should have degree 1");
            Assert.AreEqual(1, adjacent[4].Length, "vertex 'e' should have degree 1");

            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1 }, adjacent[0]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 2, 4 }, adjacent[1]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 3 }, adjacent[2]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2 }, adjacent[3]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1 }, adjacent[4]));
        }

        [TestMethod()]
        public virtual void TestToAdjList_withMap()
        {
            IAtomContainer container = Simple;

            GraphUtil.EdgeToBondMap map = new GraphUtil.EdgeToBondMap();
            int[][] adjacent = GraphUtil.ToAdjList(container, map);
            Assert.AreEqual(5, adjacent.Length, "adjacency list should have 5 vertices");

            Assert.AreEqual(1, adjacent[0].Length, "vertex 'a' should have degree 1");
            Assert.AreEqual(3, adjacent[1].Length, "vertex 'b' should have degree 3");
            Assert.AreEqual(2, adjacent[2].Length, "vertex 'c' should have degree 2");
            Assert.AreEqual(1, adjacent[3].Length, "vertex 'd' should have degree 1");
            Assert.AreEqual(1, adjacent[4].Length, "vertex 'e' should have degree 1");

            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1 }, adjacent[0]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 2, 4 }, adjacent[1]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 3 }, adjacent[2]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2 }, adjacent[3]));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1 }, adjacent[4]));

            Assert.IsNotNull(map[0, 1]);
            Assert.IsNotNull(map[1, 2]);

            Assert.AreSame(map[0, 1], map[1, 0]);
            Assert.AreSame(map[1, 2], map[2, 1]);
        }

        [TestMethod()]
        public virtual void TestToAdjList_resize()
        {
            IAtomContainer container = new AtomContainer();

            IAtom a = new Atom("C");
            container.Atoms.Add(a);

            // add 50 neighbour to 'a'
            for (int i = 0; i < 50; i++)
            {
                IAtom neighbour = new Atom("C");
                IBond bond = new Bond(a, neighbour);

                container.Atoms.Add(neighbour);
                container.Bonds.Add(bond);
            }

            int[][] adjacent = GraphUtil.ToAdjList(container);

            Assert.AreEqual(50, adjacent[0].Length, "vertex 'a' should have degree 50");

            for (int i = 1; i < 51; i++)
                Assert.AreEqual(1, adjacent[i].Length, "connected vertex should have degree of 1");

            // check adjacent neighbours are not empty
            for (int i = 0; i < adjacent[0].Length; i++)
                Assert.AreEqual(i + 1, adjacent[0][i]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void TestToAdjList_missingAtom()
        {
            IAtomContainer container = Simple;
            container.Atoms.Remove(container.Atoms[4]); // remove 'e'
            GraphUtil.ToAdjList(container);
        }

        [TestMethod()]
        public virtual void TestToAdjList_Empty()
        {
            int[][]adjacent = GraphUtil.ToAdjList(new AtomContainer());
            Assert.AreEqual(0, adjacent.Length);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void TestToAdjList_Null()
        {
            GraphUtil.ToAdjList(null);
        }

        /// <summary>
        /// 2,2-dimethylpropane
        // @cdk.inchi InChI=1S/C5H12/c1-5(2,3)4/h1-4H3
        /// </summary>
        private static IAtomContainer Simple
        {
            get
            {
                IAtomContainer container = new AtomContainer();

                IAtom a = new Atom("C");
                IAtom b = new Atom("C");
                IAtom c = new Atom("C");
                IAtom d = new Atom("C");
                IAtom e = new Atom("C");

                IBond ab = new Bond(a, b);
                IBond bc = new Bond(b, c);
                IBond cd = new Bond(c, d);
                IBond be = new Bond(b, e);

                container.Atoms.Add(a);
                container.Atoms.Add(b);
                container.Atoms.Add(c);
                container.Atoms.Add(d);
                container.Atoms.Add(e);

                container.Bonds.Add(ab);
                container.Bonds.Add(bc);
                container.Bonds.Add(cd);
                container.Bonds.Add(be);

                return container;
            }
        }
    }
}
