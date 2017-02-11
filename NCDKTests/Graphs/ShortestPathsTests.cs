/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
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

using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Graphs;
using NCDK.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Graphs
{
    [TestClass()]
    public class ShortestPathsTests
    {
        [TestMethod()]
        public virtual void TestConstructor_Container_Empty()
        {
            ShortestPaths sp = new ShortestPaths(new AtomContainer(), new Atom());
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], sp.GetPathTo(1)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], sp.GetPathsTo(1)));
            Assert.AreEqual(0, sp.GetNPathsTo(1));
            Assert.AreEqual(int.MaxValue, sp.GetDistanceTo(1));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void TestConstructor_Container_Null()
        {
            new ShortestPaths(null, new Atom());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void TestConstructor_Container_MissingAtom()
        {
            new ShortestPaths(TestMoleculeFactory.MakeBenzene(), new Atom());
        }

        [TestMethod()]
        public virtual void TestPathTo_Atom_Simple()
        {
            IAtomContainer simple = Simple();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1 }, paths.GetPathTo(simple.Atoms[1])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2 }, paths.GetPathTo(simple.Atoms[2])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3 }, paths.GetPathTo(simple.Atoms[3])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 4 }, paths.GetPathTo(simple.Atoms[4])));

        }

        [TestMethod()]
        public virtual void TestPathTo_Int_Simple()
        {

            IAtomContainer simple = Simple();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1 }, paths.GetPathTo(1)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2 }, paths.GetPathTo(2)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3 }, paths.GetPathTo(3)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 4 }, paths.GetPathTo(4)));

        }

        /**
         * ensures that when multiple paths are available, one path is still
         * returned via {@link ShortestPaths#PathTo(IAtom)}
         */
        [TestMethod()]
        public virtual void TestPathTo_Atom_Benzene()
        {

            IAtomContainer simple = TestMoleculeFactory.MakeBenzene();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3 }, paths.GetPathTo(simple.Atoms[3])));

        }

        /**
         * ensures that when multiple paths are available, one path is still
         * returned via {@link ShortestPaths#PathTo(IAtom)}
         */
        [TestMethod()]
        public virtual void TestPathTo_Int_Benzene()
        {

            IAtomContainer simple = TestMoleculeFactory.MakeBenzene();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3 }, paths.GetPathTo(3)));

        }

        [TestMethod()]
        public virtual void TestIsPrecedingPathTo()
        {
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            int[][] graph = GraphUtil.ToAdjList(benzene);
            int[] order = new int[] { 0, 1, 2, 3, 4, 5 };
            ShortestPaths paths = new ShortestPaths(graph, benzene, 0, order);
            Assert.IsFalse(paths.IsPrecedingPathTo(1));
            Assert.IsFalse(paths.IsPrecedingPathTo(2));
            Assert.IsFalse(paths.IsPrecedingPathTo(3));
            Assert.IsFalse(paths.IsPrecedingPathTo(4));
            Assert.IsFalse(paths.IsPrecedingPathTo(5));

            paths = new ShortestPaths(graph, benzene, 5, order);
            Assert.IsTrue(paths.IsPrecedingPathTo(4));
            Assert.IsTrue(paths.IsPrecedingPathTo(3));
            Assert.IsTrue(paths.IsPrecedingPathTo(2));
            Assert.IsTrue(paths.IsPrecedingPathTo(1));
            Assert.IsTrue(paths.IsPrecedingPathTo(0));

            paths = new ShortestPaths(graph, benzene, 4, order);
            Assert.IsFalse(paths.IsPrecedingPathTo(5));
            Assert.IsTrue(paths.IsPrecedingPathTo(3));
            Assert.IsTrue(paths.IsPrecedingPathTo(2));
            Assert.IsTrue(paths.IsPrecedingPathTo(1));

            // shortest path to 0 is 4,5,0...
            Assert.IsFalse(paths.IsPrecedingPathTo(0));
            //   1 - 2
            //  /     \
            // 0       3
            //  \     /
            //   5 - 4
        }

        [TestMethod()]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public virtual void TestIsPrecedingPathTo_OutOfBounds()
        {
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            ShortestPaths paths = new ShortestPaths(benzene, benzene.Atoms[0]);
            Assert.IsFalse(paths.IsPrecedingPathTo(-1));
        }

        /**
         * ensures that when multiple paths are available, one path is still
         * returned via {@link ShortestPaths#PathTo(IAtom)}
         */
        [TestMethod()]
        public virtual void TestPathTo_Atom_Norbornane()
        {
            IAtomContainer norbornane = Norbornane;
            ShortestPaths paths = new ShortestPaths(norbornane, norbornane.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3 }, paths.GetPathTo(norbornane.Atoms[3])));
        }

        /**
         * ensures that when multiple paths are available, one path is still
         * returned via {@link ShortestPaths#PathTo(IAtom)}
         */
        [TestMethod()]
        public virtual void TestPathTo_Int_Norbornane()
        {
            IAtomContainer norbornane = Norbornane;
            ShortestPaths paths = new ShortestPaths(norbornane, norbornane.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3 }, paths.GetPathTo(3)));
        }

        /**
         * ensures that when multiple paths are available, one path is still
         * returned via {@link ShortestPaths#PathTo(IAtom)}
         */
        [TestMethod()]
        public virtual void TestPathTo_Atom_Spiroundecane()
        {
            IAtomContainer spiroundecane = Spiroundecane;
            ShortestPaths paths = new ShortestPaths(spiroundecane, spiroundecane.Atoms[1]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 0, 5, 4, 6, 10, 9 }, paths.GetPathTo(spiroundecane.Atoms[9])));
        }

        /**
         * ensures that when multiple paths are available, one path is still
         * returned via {@link ShortestPaths#PathTo(IAtom)}
         */
        [TestMethod()]
        public virtual void TestPathTo_Int_Spiroundecane()
        {
            IAtomContainer spiroundecane = Spiroundecane;
            ShortestPaths paths = new ShortestPaths(spiroundecane, spiroundecane.Atoms[1]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 0, 5, 4, 6, 10, 9 }, paths.GetPathTo(9)));
        }

        /**
         * ensures that when multiple paths are available, one path is still
         * returned via {@link ShortestPaths#PathTo(IAtom)}
         */
        [TestMethod()]
        public virtual void TestPathTo_Atom_Pentadecaspiro()
        {

            //   3 - // ... //  - 4
            //  /        \ /         \
            // 0          x           1
            //  \        / \         /
            //   2 - // ...  // - 5

            // bridgehead atoms 0, 66, 68, 70, 72, 74, 76, 78, 80, 79, 77, 75, 73, 71, 69, 67, 1

            IAtomContainer pentadecaspiro = Pentadecaspiro;
            ShortestPaths paths = new ShortestPaths(pentadecaspiro, pentadecaspiro.Atoms[0]);

            // first path is determined by storage order and will always be the same
            int[] expected = new int[]{0, 2, 6, 66, 10, 14, 68, 18, 22, 70, 26, 30, 72, 34, 38, 74, 42, 46, 76, 50, 54, 78,
                58, 62, 80, 64, 60, 79, 56, 52, 77, 48, 44, 75, 40, 36, 73, 32, 28, 71, 24, 20, 69, 16, 12, 67, 8, 4, 1};

            int[] path = paths.GetPathTo(pentadecaspiro.Atoms[1]);
            Assert.IsTrue(Compares.AreDeepEqual(expected, path));

        }

        /**
         * ensures that when multiple paths are available, one path is still
         * returned via {@link ShortestPaths#PathTo(IAtom)}
         */
        [TestMethod()]
        public virtual void TestPathTo_Int_Pentadecaspiro()
        {

            //   3 - // ... //  - 4
            //  /        \ /         \
            // 0          x           1
            //  \        / \         /
            //   2 - // ...  // - 5

            // bridgehead atoms 0, 66, 68, 70, 72, 74, 76, 78, 80, 79, 77, 75, 73, 71, 69, 67, 1

            IAtomContainer pentadecaspiro = Pentadecaspiro;
            ShortestPaths paths = new ShortestPaths(pentadecaspiro, pentadecaspiro.Atoms[0]);

            // first path is determined by storage order and will always be the same
            int[] expected = new int[]{0, 2, 6, 66, 10, 14, 68, 18, 22, 70, 26, 30, 72, 34, 38, 74, 42, 46, 76, 50, 54, 78,
                58, 62, 80, 64, 60, 79, 56, 52, 77, 48, 44, 75, 40, 36, 73, 32, 28, 71, 24, 20, 69, 16, 12, 67, 8, 4, 1};

            int[] path = paths.GetPathTo(1);
            Assert.IsTrue(Compares.AreDeepEqual(expected, path));

        }

        [TestMethod()]
        public virtual void TestPathTo_Int_OutOfBoundsIndex()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(20)));
        }

        [TestMethod()]
        public virtual void TestPathTo_Int_NegativeIndex()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(-1)));
        }

        [TestMethod()]
        public virtual void TestPathTo_Atom_MissingAtom()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(new Atom("C"))));
        }

        [TestMethod()]
        public virtual void TestPathTo_Atom_Null()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(null)));
        }

        [TestMethod()]
        public virtual void TestPathTo_Atom_Disconnected()
        {

            IAtomContainer simple = Disconnected();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1 }, paths.GetPathTo(simple.Atoms[1])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2 }, paths.GetPathTo(simple.Atoms[2])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3 }, paths.GetPathTo(simple.Atoms[3])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 4 }, paths.GetPathTo(simple.Atoms[4])));

            // disconnect fragment should return 0 .Length path
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(simple.Atoms[5])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(simple.Atoms[6])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(simple.Atoms[7])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(simple.Atoms[8])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(simple.Atoms[9])));

        }

        [TestMethod()]
        public virtual void TestPathTo_Int_Disconnected()
        {

            IAtomContainer simple = Disconnected();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1 }, paths.GetPathTo(1)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2 }, paths.GetPathTo(2)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3 }, paths.GetPathTo(3)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 4 }, paths.GetPathTo(4)));

            // disconnect fragment should return 0 .Length path
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(5)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(6)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(7)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(8)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], paths.GetPathTo(9)));

        }

        [TestMethod()]
        public virtual void TestPathsTo_Atom_Simple()
        {

            IAtomContainer simple = Simple();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1 } }, paths.GetPathsTo(simple.Atoms[1])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 2 } }, paths.GetPathsTo(simple.Atoms[2])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 2, 3 } }, paths.GetPathsTo(simple.Atoms[3])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 4 } }, paths.GetPathsTo(simple.Atoms[4])));

        }

        [TestMethod()]
        public virtual void TestPathsTo_Int_Simple()
        {

            IAtomContainer simple = Simple();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1 } }, paths.GetPathsTo(1)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 2 } }, paths.GetPathsTo(2)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 2, 3 } }, paths.GetPathsTo(3)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 4 } }, paths.GetPathsTo(4)));

        }

        [TestMethod()]
        public virtual void TestPathsTo_Atom_Benzene()
        {

            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

            ShortestPaths paths = new ShortestPaths(benzene, benzene.Atoms[0]);

            int[][] expected = new int[][] { new int[] { 0, 1, 2, 3 }, new int[] { 0, 5, 4, 3 } };
            Assert.IsTrue(Compares.AreDeepEqual(expected, paths.GetPathsTo(benzene.Atoms[3])));

        }

        [TestMethod()]
        public virtual void TestPathsTo_Atom_Spiroundecane()
        {

            IAtomContainer spiroundecane = Spiroundecane;

            ShortestPaths paths = new ShortestPaths(spiroundecane, spiroundecane.Atoms[1]);

            //   2 -- 3   7 -- 8
            //  /      \ /      \
            // 1        4        9
            //  \      / \      /
            //   0 -- 5   6 - 10

            // path order is determined by storage order, given the same input,
            // the output order will never change
            int[][] expected = new int[][]{ new int[] {1, 0, 5, 4, 6, 10, 9}, new int[] {1, 2, 3, 4, 6, 10, 9}, new int[] {1, 0, 5, 4, 7, 8, 9},
                new int[] {1, 2, 3, 4, 7, 8, 9}};

            Assert.IsTrue(Compares.AreDeepEqual(expected, paths.GetPathsTo(spiroundecane.Atoms[9])));

        }

        [TestMethod()]
        public virtual void TestPathsTo_Int_Spiroundecane()
        {

            IAtomContainer spiroundecane = Spiroundecane;

            ShortestPaths paths = new ShortestPaths(spiroundecane, spiroundecane.Atoms[1]);

            //   2 -- 3   7 -- 8
            //  /      \ /      \
            // 1        4        9
            //  \      / \      /
            //   0 -- 5   6 - 10

            // path order is determined by storage order, given the same input,
            // the output order will never change
            int[][] expected = new int[][]{ new int[] {1, 0, 5, 4, 6, 10, 9}, new int[] {1, 2, 3, 4, 6, 10, 9}, new int[] {1, 0, 5, 4, 7, 8, 9},
                new int[] {1, 2, 3, 4, 7, 8, 9}};

            Assert.IsTrue(Compares.AreDeepEqual(expected, paths.GetPathsTo(9)));

        }

        [TestMethod()]
        public virtual void TestPathsTo_Int_Benzene()
        {

            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

            ShortestPaths paths = new ShortestPaths(benzene, benzene.Atoms[0]);

            int[][] expected = new int[][] { new int[] { 0, 1, 2, 3 }, new int[] { 0, 5, 4, 3 } };
            Assert.IsTrue(Compares.AreDeepEqual(expected, paths.GetPathsTo(3)));

        }

        [TestMethod()]
        public virtual void TestPathsTo_Atom_Norbornane()
        {
            IAtomContainer norbornane = Norbornane;
            ShortestPaths paths = new ShortestPaths(norbornane, norbornane.Atoms[0]);
            int[][] expected = new int[][] { new int[] { 0, 1, 2, 3 }, new int[] { 0, 5, 4, 3 }, new int[] { 0, 6, 7, 3 } };
            Assert.IsTrue(Compares.AreDeepEqual(expected, paths.GetPathsTo(norbornane.Atoms[3])));
        }

        [TestMethod()]
        public virtual void TestPathsTo_Int_Norbornane()
        {
            IAtomContainer norbornane = Norbornane;
            ShortestPaths paths = new ShortestPaths(norbornane, norbornane.Atoms[0]);
            int[][] expected = new int[][] { new int[] { 0, 1, 2, 3 }, new int[] { 0, 5, 4, 3 }, new int[] { 0, 6, 7, 3 } };
            Assert.IsTrue(Compares.AreDeepEqual(expected, paths.GetPathsTo(3)));
        }

        [TestMethod()]
        public virtual void TestPathsTo_Atom_Pentadecaspiro()
        {

            //   3 - // ... //  - 4
            //  /        \ /         \
            // 0          x           1
            //  \        / \         /
            //   2 - // ...  // - 5

            // bridgehead atoms 0, 66, 68, 70, 72, 74, 76, 78, 80, 79, 77, 75, 73, 71, 69, 67, 1

            IAtomContainer pentadecaspiro = Pentadecaspiro;
            ShortestPaths paths = new ShortestPaths(pentadecaspiro, pentadecaspiro.Atoms[0]);

            int[] bridgeheads = new int[] { 66, 68, 70, 72, 74, 76, 78, 80, 79, 77, 75, 73, 71, 69, 67, 1 };

            // demonstrates that all paths up and beyond 65,000+ can be retrieved
            for (int i = 0; i < bridgeheads.Length; i++)
            {

                int bridgehead = bridgeheads[i];

                int[][] path = paths.GetPathsTo(pentadecaspiro.Atoms[bridgehead]);

                int n = (int)Math.Pow(2, (i + 1));
                Assert.AreEqual(n, path.Length);

                // test is too long when more then 500 different paths
                if (n < 500)
                {
                    for (int j = 0; j < n; j++)
                    {

                        // check the first atom is '0' and the last atom is the 'bridgehead'
                        Assert.AreEqual(0, path[j][0]);
                        Assert.AreEqual(bridgehead, path[j][paths.GetDistanceTo(bridgehead)]);

                        // check all paths are unique
                        for (int k = j + 1; k < n; k++)
                        {
                            // hamcrest matcher does array comparison
                            Assert.IsTrue(Compares.AreDeepEqual(path[j], path[j]));
                        }
                    }
                }

            }

        }

        [TestMethod()]
        public virtual void TestPathsTo_Int_OutOfBoundsIndex()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(20)));
        }

        [TestMethod()]
        public virtual void TestPathsTo_Int_NegativeIndex()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(-1)));
        }

        [TestMethod()]
        public virtual void TestPathsTo_Atom_MissingAtom()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(new Atom("C"))));
        }

        [TestMethod()]
        public virtual void TestPathsTo_Atom_Null()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(null)));
        }

        [TestMethod()]
        public virtual void TestPathsTo_Atom_Disconnected()
        {

            IAtomContainer simple = Disconnected();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1 } }, paths.GetPathsTo(simple.Atoms[1])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 2 } }, paths.GetPathsTo(simple.Atoms[2])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 2, 3 } }, paths.GetPathsTo(simple.Atoms[3])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 4 } }, paths.GetPathsTo(simple.Atoms[4])));

            // disconnect fragment should return 0 .Length path
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(simple.Atoms[5])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(simple.Atoms[6])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(simple.Atoms[7])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(simple.Atoms[8])));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(simple.Atoms[9])));

        }

        [TestMethod()]
        public virtual void TestPathsTo_Int_Disconnected()
        {

            IAtomContainer simple = Disconnected();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1 } }, paths.GetPathsTo(1)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 2 } }, paths.GetPathsTo(2)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 2, 3 } }, paths.GetPathsTo(3)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { new int[] { 0, 1, 4 } }, paths.GetPathsTo(4)));

            // disconnect fragment should return 0 .Length path
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(5)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(6)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(7)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(8)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[0][], paths.GetPathsTo(9)));

        }

        [TestMethod()]
        public virtual void TestAtomsTo_Atom_Simple()
        {

            IAtomContainer simple = Simple();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            IAtom a = simple.Atoms[0];
            IAtom b = simple.Atoms[1];
            IAtom c = simple.Atoms[2];
            IAtom d = simple.Atoms[3];
            IAtom e = simple.Atoms[4];

            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b }, paths.GetAtomsTo(b)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, c }, paths.GetAtomsTo(c)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, c, d }, paths.GetAtomsTo(d)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, e }, paths.GetAtomsTo(e)));

        }

        [TestMethod()]
        public virtual void TestAtomsTo_Int_Simple()
        {

            IAtomContainer simple = Simple();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            IAtom a = simple.Atoms[0];
            IAtom b = simple.Atoms[1];
            IAtom c = simple.Atoms[2];
            IAtom d = simple.Atoms[3];
            IAtom e = simple.Atoms[4];

            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b }, paths.GetAtomsTo(1)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, c }, paths.GetAtomsTo(2)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, c, d }, paths.GetAtomsTo(3)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, e }, paths.GetAtomsTo(4)));

        }

        /**
         * ensures that when multiple paths are available, one path is still
         * returned via {@link ShortestPaths#PathTo(IAtom)}
         */
        [TestMethod()]
        public virtual void TestAtomsTo_Atom_Benzene()
        {

            IAtomContainer simple = TestMoleculeFactory.MakeBenzene();

            IAtom c1 = simple.Atoms[0];
            IAtom c2 = simple.Atoms[1];
            IAtom c3 = simple.Atoms[2];
            IAtom c4 = simple.Atoms[3];

            ShortestPaths paths = new ShortestPaths(simple, c1);

            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { c1, c2, c3, c4 }, paths.GetAtomsTo(c4)));

        }

        /**
         * ensures that when multiple paths are available, one path is still
         * returned via {@link ShortestPaths#PathTo(IAtom)}
         */
        [TestMethod()]
        public virtual void TestAtomsTo_Int_Benzene()
        {

            IAtomContainer simple = TestMoleculeFactory.MakeBenzene();

            IAtom c1 = simple.Atoms[0];
            IAtom c2 = simple.Atoms[1];
            IAtom c3 = simple.Atoms[2];
            IAtom c4 = simple.Atoms[3];

            ShortestPaths paths = new ShortestPaths(simple, c1);

            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { c1, c2, c3, c4 }, paths.GetAtomsTo(3)));

        }

        [TestMethod()]
        public virtual void TestAtomsTo_Atom_Disconnected()
        {

            IAtomContainer simple = Disconnected();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            IAtom a = simple.Atoms[0];
            IAtom b = simple.Atoms[1];
            IAtom c = simple.Atoms[2];
            IAtom d = simple.Atoms[3];
            IAtom e = simple.Atoms[4];
            IAtom f = simple.Atoms[5];
            IAtom g = simple.Atoms[6];
            IAtom h = simple.Atoms[7];
            IAtom i = simple.Atoms[8];
            IAtom j = simple.Atoms[9];

            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b }, paths.GetAtomsTo(b)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, c }, paths.GetAtomsTo(c)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, c, d }, paths.GetAtomsTo(d)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, e }, paths.GetAtomsTo(e)));

            // disconnect fragment should return 0 .Length path
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(f)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(g)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(h)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(i)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(j)));

        }

        [TestMethod()]
        public virtual void TestAtomsTo_Int_Disconnected()
        {

            IAtomContainer simple = Disconnected();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            IAtom a = simple.Atoms[0];
            IAtom b = simple.Atoms[1];
            IAtom c = simple.Atoms[2];
            IAtom d = simple.Atoms[3];
            IAtom e = simple.Atoms[4];

            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b }, paths.GetAtomsTo(1)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, c }, paths.GetAtomsTo(2)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, c, d }, paths.GetAtomsTo(3)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a, b, e }, paths.GetAtomsTo(4)));

            // disconnect fragment should return 0 .Length path
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(5)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(6)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(7)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(8)));
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(9)));

        }

        [TestMethod()]
        public virtual void TestAtomsTo_Int_OutOfBoundsIndex()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(20)));
        }

        [TestMethod()]
        public virtual void TestAtomsTo_Int_NegativeIndex()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(-1)));
        }

        [TestMethod()]
        public virtual void TestAtomsTo_Atom_MissingAtom()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(new Atom("C"))));
        }

        [TestMethod()]
        public virtual void TestAtomsTo_Atom_Null()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[0], paths.GetAtomsTo(null)));
        }

        [TestMethod()]
        public virtual void TestNPathsTo_Atom_Simple()
        {

            IAtomContainer simple = Simple();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.AreEqual(1, paths.GetNPathsTo(simple.Atoms[0]));
            Assert.AreEqual(1, paths.GetNPathsTo(simple.Atoms[1]));
            Assert.AreEqual(1, paths.GetNPathsTo(simple.Atoms[2]));
            Assert.AreEqual(1, paths.GetNPathsTo(simple.Atoms[3]));
            Assert.AreEqual(1, paths.GetNPathsTo(simple.Atoms[4]));

        }

        [TestMethod()]
        public virtual void TestNPathsTo_Int_Simple()
        {

            IAtomContainer simple = Simple();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.AreEqual(1, paths.GetNPathsTo(1));
            Assert.AreEqual(1, paths.GetNPathsTo(2));
            Assert.AreEqual(1, paths.GetNPathsTo(3));
            Assert.AreEqual(1, paths.GetNPathsTo(4));

        }

        [TestMethod()]
        public virtual void TestNPathsTo_Atom_MissingAtom()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.AreEqual(0, paths.GetNPathsTo(new Atom("C")));
        }

        [TestMethod()]
        public virtual void TestNPathsTo_Atom_Null()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.AreEqual(0, paths.GetNPathsTo(null));
        }

        [TestMethod()]
        public virtual void TestNPathsTo_Int_OutOfBoundIndex()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.AreEqual(0, paths.GetNPathsTo(20));
        }

        [TestMethod()]
        public virtual void TestNPathsTo_Int_NegativeIndex()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.AreEqual(0, paths.GetNPathsTo(-1));
        }

        [TestMethod()]
        public virtual void TestNPathsTo_Atom_Disconnected()
        {

            IAtomContainer container = Disconnected();

            ShortestPaths paths = new ShortestPaths(container, container.Atoms[0]);

            Assert.AreEqual(1, paths.GetNPathsTo(container.Atoms[0]));
            Assert.AreEqual(1, paths.GetNPathsTo(container.Atoms[1]));
            Assert.AreEqual(1, paths.GetNPathsTo(container.Atoms[2]));
            Assert.AreEqual(1, paths.GetNPathsTo(container.Atoms[3]));
            Assert.AreEqual(1, paths.GetNPathsTo(container.Atoms[4]));

            Assert.AreEqual(0, paths.GetNPathsTo(container.Atoms[5]));
            Assert.AreEqual(0, paths.GetNPathsTo(container.Atoms[6]));
            Assert.AreEqual(0, paths.GetNPathsTo(container.Atoms[7]));
            Assert.AreEqual(0, paths.GetNPathsTo(container.Atoms[8]));
            Assert.AreEqual(0, paths.GetNPathsTo(container.Atoms[9]));

        }

        [TestMethod()]
        public virtual void TestNPathsTo_Int_Disconnected()
        {

            IAtomContainer container = Disconnected();

            ShortestPaths paths = new ShortestPaths(container, container.Atoms[0]);

            Assert.AreEqual(1, paths.GetNPathsTo(0));
            Assert.AreEqual(1, paths.GetNPathsTo(1));
            Assert.AreEqual(1, paths.GetNPathsTo(2));
            Assert.AreEqual(1, paths.GetNPathsTo(3));
            Assert.AreEqual(1, paths.GetNPathsTo(4));

            Assert.AreEqual(0, paths.GetNPathsTo(5));
            Assert.AreEqual(0, paths.GetNPathsTo(6));
            Assert.AreEqual(0, paths.GetNPathsTo(7));
            Assert.AreEqual(0, paths.GetNPathsTo(8));
            Assert.AreEqual(0, paths.GetNPathsTo(9));

        }

        [TestMethod()]
        public virtual void TestNPathsTo_Atom_Benzene()
        {

            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

            ShortestPaths paths = new ShortestPaths(benzene, benzene.Atoms[0]);

            Assert.AreEqual(1, paths.GetNPathsTo(benzene.Atoms[0]));
            Assert.AreEqual(1, paths.GetNPathsTo(benzene.Atoms[1]));
            Assert.AreEqual(1, paths.GetNPathsTo(benzene.Atoms[2]));
            Assert.AreEqual(2, paths.GetNPathsTo(benzene.Atoms[3]));
            Assert.AreEqual(1, paths.GetNPathsTo(benzene.Atoms[4]));
            Assert.AreEqual(1, paths.GetNPathsTo(benzene.Atoms[5]));

        }

        [TestMethod()]
        public virtual void TestNPathsTo_Int_Benzene()
        {

            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

            ShortestPaths paths = new ShortestPaths(benzene, benzene.Atoms[0]);

            Assert.AreEqual(1, paths.GetNPathsTo(0));
            Assert.AreEqual(1, paths.GetNPathsTo(1));
            Assert.AreEqual(1, paths.GetNPathsTo(2));
            Assert.AreEqual(2, paths.GetNPathsTo(3));
            Assert.AreEqual(1, paths.GetNPathsTo(4));
            Assert.AreEqual(1, paths.GetNPathsTo(5));

        }

        [TestMethod()]
        public virtual void TestNPathsTo_Atom_Norbornane()
        {

            IAtomContainer norbornane = Norbornane;

            ShortestPaths paths = new ShortestPaths(norbornane, norbornane.Atoms[0]);

            Assert.AreEqual(1, paths.GetNPathsTo(norbornane.Atoms[0]));
            Assert.AreEqual(1, paths.GetNPathsTo(norbornane.Atoms[1]));
            Assert.AreEqual(1, paths.GetNPathsTo(norbornane.Atoms[2]));
            Assert.AreEqual(3, paths.GetNPathsTo(norbornane.Atoms[3]));
            Assert.AreEqual(1, paths.GetNPathsTo(norbornane.Atoms[4]));
            Assert.AreEqual(1, paths.GetNPathsTo(norbornane.Atoms[5]));
            Assert.AreEqual(1, paths.GetNPathsTo(norbornane.Atoms[6]));
            Assert.AreEqual(1, paths.GetNPathsTo(norbornane.Atoms[7]));

        }

        [TestMethod()]
        public virtual void TestNPathsTo_Int_Norbornane()
        {

            IAtomContainer norbornane = Norbornane;

            ShortestPaths paths = new ShortestPaths(norbornane, norbornane.Atoms[0]);

            Assert.AreEqual(1, paths.GetNPathsTo(0));
            Assert.AreEqual(1, paths.GetNPathsTo(1));
            Assert.AreEqual(1, paths.GetNPathsTo(2));
            Assert.AreEqual(3, paths.GetNPathsTo(3));
            Assert.AreEqual(1, paths.GetNPathsTo(4));
            Assert.AreEqual(1, paths.GetNPathsTo(5));
            Assert.AreEqual(1, paths.GetNPathsTo(6));
            Assert.AreEqual(1, paths.GetNPathsTo(7));

        }

        [TestMethod()]
        public virtual void TestNPathsTo_Atom_Spiroundecane()
        {
            IAtomContainer spiroundecane = Spiroundecane;
            ShortestPaths paths = new ShortestPaths(spiroundecane, spiroundecane.Atoms[1]);
            Assert.AreEqual(4, paths.GetNPathsTo(spiroundecane.Atoms[9]));
        }

        [TestMethod()]
        public virtual void TestNPathsTo_Int_Spiroundecane()
        {
            IAtomContainer spiroundecane = Spiroundecane;
            ShortestPaths paths = new ShortestPaths(spiroundecane, spiroundecane.Atoms[1]);
            Assert.AreEqual(4, paths.GetNPathsTo(9));
        }

        [TestMethod()]
        public virtual void TestNPathsTo_Atom_Pentadecaspiro()
        {

            //   3 - // ... //  - 4
            //  /        \ /         \
            // 0         x            1
            //  \        / \         /
            //   2 - // ...  // - 5

            // bridgehead atoms 0, 66, 68, 70, 72, 74, 76, 78, 80, 79, 77, 75, 73, 71, 69, 67, 1

            IAtomContainer pentadecaspiro = Pentadecaspiro;
            ShortestPaths paths = new ShortestPaths(pentadecaspiro, pentadecaspiro.Atoms[0]);

            Assert.AreEqual(2, paths.GetNPathsTo(pentadecaspiro.Atoms[66]));
            Assert.AreEqual(4, paths.GetNPathsTo(pentadecaspiro.Atoms[68]));
            Assert.AreEqual(8, paths.GetNPathsTo(pentadecaspiro.Atoms[70]));
            Assert.AreEqual(16, paths.GetNPathsTo(pentadecaspiro.Atoms[72]));
            Assert.AreEqual(32, paths.GetNPathsTo(pentadecaspiro.Atoms[74]));
            Assert.AreEqual(64, paths.GetNPathsTo(pentadecaspiro.Atoms[76]));
            Assert.AreEqual(128, paths.GetNPathsTo(pentadecaspiro.Atoms[78]));
            Assert.AreEqual(256, paths.GetNPathsTo(pentadecaspiro.Atoms[80]));
            Assert.AreEqual(512, paths.GetNPathsTo(pentadecaspiro.Atoms[79]));
            Assert.AreEqual(1024, paths.GetNPathsTo(pentadecaspiro.Atoms[77]));
            Assert.AreEqual(2048, paths.GetNPathsTo(pentadecaspiro.Atoms[75]));
            Assert.AreEqual(4096, paths.GetNPathsTo(pentadecaspiro.Atoms[73]));
            Assert.AreEqual(8192, paths.GetNPathsTo(pentadecaspiro.Atoms[71]));
            Assert.AreEqual(16384, paths.GetNPathsTo(pentadecaspiro.Atoms[69]));
            Assert.AreEqual(32768, paths.GetNPathsTo(pentadecaspiro.Atoms[67]));
            Assert.AreEqual(65536, paths.GetNPathsTo(pentadecaspiro.Atoms[1]));
        }

        [TestMethod()]
        public virtual void TestNPathsTo_Int_Pentadecaspiro()
        {
            IAtomContainer pentadecaspiro = Pentadecaspiro;
            ShortestPaths paths = new ShortestPaths(pentadecaspiro, pentadecaspiro.Atoms[0]);

            // bridgehead atoms 0, 66, 68, 70, 72, 74, 76, 78, 80, 79, 77, 75, 73, 71, 69, 67, 1

            Assert.AreEqual(2, paths.GetNPathsTo(66));
            Assert.AreEqual(4, paths.GetNPathsTo(68));
            Assert.AreEqual(8, paths.GetNPathsTo(70));
            Assert.AreEqual(16, paths.GetNPathsTo(72));
            Assert.AreEqual(32, paths.GetNPathsTo(74));
            Assert.AreEqual(64, paths.GetNPathsTo(76));
            Assert.AreEqual(128, paths.GetNPathsTo(78));
            Assert.AreEqual(256, paths.GetNPathsTo(80));
            Assert.AreEqual(512, paths.GetNPathsTo(79));
            Assert.AreEqual(1024, paths.GetNPathsTo(77));
            Assert.AreEqual(2048, paths.GetNPathsTo(75));
            Assert.AreEqual(4096, paths.GetNPathsTo(73));
            Assert.AreEqual(8192, paths.GetNPathsTo(71));
            Assert.AreEqual(16384, paths.GetNPathsTo(69));
            Assert.AreEqual(32768, paths.GetNPathsTo(67));
            Assert.AreEqual(65536, paths.GetNPathsTo(1));
        }

        [TestMethod()]
        public virtual void TestDistanceTo_Atom_Simple()
        {

            IAtomContainer simple = Simple();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.AreEqual(0, paths.GetDistanceTo(simple.Atoms[0]));
            Assert.AreEqual(1, paths.GetDistanceTo(simple.Atoms[1]));
            Assert.AreEqual(2, paths.GetDistanceTo(simple.Atoms[2]));
            Assert.AreEqual(3, paths.GetDistanceTo(simple.Atoms[3]));
            Assert.AreEqual(2, paths.GetDistanceTo(simple.Atoms[4]));

        }

        [TestMethod()]
        public virtual void TestDistanceTo_Int_Simple()
        {

            IAtomContainer simple = Simple();

            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);

            Assert.AreEqual(0, paths.GetDistanceTo(0));
            Assert.AreEqual(1, paths.GetDistanceTo(1));
            Assert.AreEqual(2, paths.GetDistanceTo(2));
            Assert.AreEqual(3, paths.GetDistanceTo(3));
            Assert.AreEqual(2, paths.GetDistanceTo(4));

        }

        [TestMethod()]
        public virtual void TestDistanceTo_Atom_MissingAtom()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(new Atom("C")));
        }

        [TestMethod()]
        public virtual void TestDistanceTo_Atom_Null()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(null));
        }

        [TestMethod()]
        public virtual void TestDistanceTo_Int_OutOfBoundIndex()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(20));
        }

        [TestMethod()]
        public virtual void TestDistanceTo_Int_NegativeIndex()
        {
            IAtomContainer simple = Simple();
            ShortestPaths paths = new ShortestPaths(simple, simple.Atoms[0]);
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(-1));
        }

        [TestMethod()]
        public virtual void TestDistanceTo_Atom_Disconnected()
        {

            IAtomContainer container = Disconnected();

            ShortestPaths paths = new ShortestPaths(container, container.Atoms[0]);

            Assert.AreEqual(0, paths.GetDistanceTo(container.Atoms[0]));
            Assert.AreEqual(1, paths.GetDistanceTo(container.Atoms[1]));
            Assert.AreEqual(2, paths.GetDistanceTo(container.Atoms[2]));
            Assert.AreEqual(3, paths.GetDistanceTo(container.Atoms[3]));
            Assert.AreEqual(2, paths.GetDistanceTo(container.Atoms[4]));

            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(container.Atoms[5]));
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(container.Atoms[6]));
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(container.Atoms[7]));
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(container.Atoms[8]));
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(container.Atoms[9]));

        }

        [TestMethod()]
        public virtual void TestDistanceTo_Int_Disconnected()
        {

            IAtomContainer container = Disconnected();

            ShortestPaths paths = new ShortestPaths(container, container.Atoms[0]);

            Assert.AreEqual(0, paths.GetDistanceTo(0));
            Assert.AreEqual(1, paths.GetDistanceTo(1));
            Assert.AreEqual(2, paths.GetDistanceTo(2));
            Assert.AreEqual(3, paths.GetDistanceTo(3));
            Assert.AreEqual(2, paths.GetDistanceTo(4));

            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(5));
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(6));
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(7));
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(8));
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(9));

        }

        [TestMethod()]
        public virtual void TestDistanceTo_Atom_Benzene()
        {

            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

            ShortestPaths paths = new ShortestPaths(benzene, benzene.Atoms[0]);

            Assert.AreEqual(0, paths.GetDistanceTo(benzene.Atoms[0]));
            Assert.AreEqual(1, paths.GetDistanceTo(benzene.Atoms[1]));
            Assert.AreEqual(2, paths.GetDistanceTo(benzene.Atoms[2]));
            Assert.AreEqual(3, paths.GetDistanceTo(benzene.Atoms[3]));
            Assert.AreEqual(2, paths.GetDistanceTo(benzene.Atoms[4]));
            Assert.AreEqual(1, paths.GetDistanceTo(benzene.Atoms[5]));

        }

        [TestMethod()]
        public virtual void TestDistanceTo_Int_Benzene()
        {

            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

            ShortestPaths paths = new ShortestPaths(benzene, benzene.Atoms[0]);

            Assert.AreEqual(0, paths.GetDistanceTo(0));
            Assert.AreEqual(1, paths.GetDistanceTo(1));
            Assert.AreEqual(2, paths.GetDistanceTo(2));
            Assert.AreEqual(3, paths.GetDistanceTo(3));
            Assert.AreEqual(2, paths.GetDistanceTo(4));
            Assert.AreEqual(1, paths.GetDistanceTo(5));

        }

        [TestMethod()]
        public virtual void TestDistanceTo_Int_Benzene_limited()
        {
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

            ShortestPaths paths = new ShortestPaths(GraphUtil.ToAdjList(benzene), benzene, 0, 2, null);

            Assert.AreEqual(0, paths.GetDistanceTo(0));
            Assert.AreEqual(1, paths.GetDistanceTo(1));
            Assert.AreEqual(2, paths.GetDistanceTo(2));
            Assert.AreEqual(int.MaxValue, paths.GetDistanceTo(3)); // dist > 2 (our limit)
            Assert.AreEqual(2, paths.GetDistanceTo(4));
            Assert.AreEqual(1, paths.GetDistanceTo(5));
        }

        [TestMethod()]
        public virtual void TestDistanceTo_Atom_Spiroundecane()
        {
            IAtomContainer spiroundecane = Spiroundecane;
            ShortestPaths paths = new ShortestPaths(spiroundecane, spiroundecane.Atoms[1]);

            //   2 -- 3   7 -- 8
            //  /      \ /      \
            // 1        4        9
            //  \      / \      /
            //   0 -- 5   6 - 10

            Assert.AreEqual(1, paths.GetDistanceTo(spiroundecane.Atoms[0]));
            Assert.AreEqual(1, paths.GetDistanceTo(spiroundecane.Atoms[2]));

            Assert.AreEqual(2, paths.GetDistanceTo(spiroundecane.Atoms[3]));
            Assert.AreEqual(2, paths.GetDistanceTo(spiroundecane.Atoms[5]));

            Assert.AreEqual(3, paths.GetDistanceTo(spiroundecane.Atoms[4]));

            Assert.AreEqual(4, paths.GetDistanceTo(spiroundecane.Atoms[6]));
            Assert.AreEqual(4, paths.GetDistanceTo(spiroundecane.Atoms[7]));

            Assert.AreEqual(5, paths.GetDistanceTo(spiroundecane.Atoms[8]));
            Assert.AreEqual(5, paths.GetDistanceTo(spiroundecane.Atoms[10]));

            Assert.AreEqual(6, paths.GetDistanceTo(spiroundecane.Atoms[9]));
        }

        [TestMethod()]
        public virtual void TestDistanceTo_Int_Spiroundecane()
        {
            IAtomContainer spiroundecane = Spiroundecane;
            ShortestPaths paths = new ShortestPaths(spiroundecane, spiroundecane.Atoms[1]);

            //   2 -- 3   7 -- 8
            //  /      \ /      \
            // 1        4        9
            //  \      / \      /
            //   0 -- 5   6 - 10

            Assert.AreEqual(1, paths.GetDistanceTo(0));
            Assert.AreEqual(1, paths.GetDistanceTo(2));

            Assert.AreEqual(2, paths.GetDistanceTo(3));
            Assert.AreEqual(2, paths.GetDistanceTo(5));

            Assert.AreEqual(3, paths.GetDistanceTo(4));

            Assert.AreEqual(4, paths.GetDistanceTo(6));
            Assert.AreEqual(4, paths.GetDistanceTo(7));

            Assert.AreEqual(5, paths.GetDistanceTo(8));
            Assert.AreEqual(5, paths.GetDistanceTo(10));

            Assert.AreEqual(6, paths.GetDistanceTo(9));
        }

        [TestMethod()]
        public virtual void TestDistanceTo_Atom_Pentadecaspiro()
        {

            //   3 - // ... //  - 4
            //  /        \ /         \
            // 0          x           1
            //  \        / \         /
            //   2 - // ...  // - 5

            // bridgehead atoms 0, 66, 68, 70, 72, 74, 76, 78, 80, 79, 77, 75, 73, 71, 69, 67, 1

            IAtomContainer pentadecaspiro = Pentadecaspiro;
            ShortestPaths paths = new ShortestPaths(pentadecaspiro, pentadecaspiro.Atoms[0]);

            Assert.AreEqual(3, paths.GetDistanceTo(pentadecaspiro.Atoms[66]));
            Assert.AreEqual(6, paths.GetDistanceTo(pentadecaspiro.Atoms[68]));
            Assert.AreEqual(9, paths.GetDistanceTo(pentadecaspiro.Atoms[70]));
            Assert.AreEqual(12, paths.GetDistanceTo(pentadecaspiro.Atoms[72]));
            Assert.AreEqual(15, paths.GetDistanceTo(pentadecaspiro.Atoms[74]));
            Assert.AreEqual(18, paths.GetDistanceTo(pentadecaspiro.Atoms[76]));
            Assert.AreEqual(21, paths.GetDistanceTo(pentadecaspiro.Atoms[78]));
            Assert.AreEqual(24, paths.GetDistanceTo(pentadecaspiro.Atoms[80]));
            Assert.AreEqual(27, paths.GetDistanceTo(pentadecaspiro.Atoms[79]));
            Assert.AreEqual(30, paths.GetDistanceTo(pentadecaspiro.Atoms[77]));
            Assert.AreEqual(33, paths.GetDistanceTo(pentadecaspiro.Atoms[75]));
            Assert.AreEqual(36, paths.GetDistanceTo(pentadecaspiro.Atoms[73]));
            Assert.AreEqual(39, paths.GetDistanceTo(pentadecaspiro.Atoms[71]));
            Assert.AreEqual(42, paths.GetDistanceTo(pentadecaspiro.Atoms[69]));
            Assert.AreEqual(45, paths.GetDistanceTo(pentadecaspiro.Atoms[67]));
            Assert.AreEqual(48, paths.GetDistanceTo(pentadecaspiro.Atoms[1]));
        }

        [TestMethod()]
        public virtual void TestDistanceTo_Int_Pentadecaspiro()
        {
            IAtomContainer pentadecaspiro = Pentadecaspiro;
            ShortestPaths paths = new ShortestPaths(pentadecaspiro, pentadecaspiro.Atoms[0]);

            // bridgehead atoms 0, 66, 68, 70, 72, 74, 76, 78, 80, 79, 77, 75, 73, 71, 69, 67, 1

            Assert.AreEqual(3, paths.GetDistanceTo(66));
            Assert.AreEqual(6, paths.GetDistanceTo(68));
            Assert.AreEqual(9, paths.GetDistanceTo(70));
            Assert.AreEqual(12, paths.GetDistanceTo(72));
            Assert.AreEqual(15, paths.GetDistanceTo(74));
            Assert.AreEqual(18, paths.GetDistanceTo(76));
            Assert.AreEqual(21, paths.GetDistanceTo(78));
            Assert.AreEqual(24, paths.GetDistanceTo(80));
            Assert.AreEqual(27, paths.GetDistanceTo(79));
            Assert.AreEqual(30, paths.GetDistanceTo(77));
            Assert.AreEqual(33, paths.GetDistanceTo(75));
            Assert.AreEqual(36, paths.GetDistanceTo(73));
            Assert.AreEqual(39, paths.GetDistanceTo(71));
            Assert.AreEqual(42, paths.GetDistanceTo(69));
            Assert.AreEqual(45, paths.GetDistanceTo(67));
            Assert.AreEqual(48, paths.GetDistanceTo(1));
        }

        /**
         * two disconnected 2,2-dimethylpropanes
         */
        private static IAtomContainer Disconnected()
        {
            IAtomContainer container = Simple();
            container.Add(Simple());
            return container;
        }

        /**
         * 2,2-dimethylpropane
         */
        private static IAtomContainer Simple()
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

            container.Add(a);
            container.Add(b);
            container.Add(c);
            container.Add(d);
            container.Add(e);

            container.Add(ab);
            container.Add(bc);
            container.Add(cd);
            container.Add(be);

            return container;

        }

        /**
         * norbornane generated with CDKSourceCodeWriter
         */
        private static IAtomContainer Norbornane
        {
            get
            {
                // TODO: ChemObjectBuilder  to Silent.ChemObjectBuilder 
                IChemObjectBuilder builder = ChemObjectBuilder.Instance;
                IAtomContainer mol = builder.CreateAtomContainer();
                IAtom a1 = builder.CreateAtom("C");
                mol.Add(a1);
                IAtom a2 = builder.CreateAtom("C");
                mol.Add(a2);
                IAtom a3 = builder.CreateAtom("C");
                mol.Add(a3);
                IAtom a4 = builder.CreateAtom("C");
                mol.Add(a4);
                IAtom a5 = builder.CreateAtom("C");
                mol.Add(a5);
                IAtom a6 = builder.CreateAtom("C");
                mol.Add(a6);
                IAtom a7 = builder.CreateAtom("C");
                mol.Add(a7);
                IAtom a8 = builder.CreateAtom("C");
                mol.Add(a8);
                IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
                mol.Add(b1);
                IBond b2 = builder.CreateBond(a1, a6, BondOrder.Single);
                mol.Add(b2);
                IBond b3 = builder.CreateBond(a2, a3, BondOrder.Single);
                mol.Add(b3);
                IBond b4 = builder.CreateBond(a3, a4, BondOrder.Single);
                mol.Add(b4);
                IBond b5 = builder.CreateBond(a4, a5, BondOrder.Single);
                mol.Add(b5);
                IBond b6 = builder.CreateBond(a5, a6, BondOrder.Single);
                mol.Add(b6);
                IBond b7 = builder.CreateBond(a1, a7, BondOrder.Single);
                mol.Add(b7);
                IBond b8 = builder.CreateBond(a7, a8, BondOrder.Single);
                mol.Add(b8);
                IBond b9 = builder.CreateBond(a8, a4, BondOrder.Single);
                mol.Add(b9);

                return mol;
            }
        }

        /**
         * pentadecaspiro[5.2.2.2.2.2.2.2.2.2.2.2.2.2.2.5^{48}.2^{45}.2^{42}.2^{39}.2^{36}.2^{33}.2^{30}.2^{27}.2^{24}.2^{21}.2^{18}.2^{15}.2^{12}.2^{9}.2^{6}]henoctacontane
         *
         * @cdk.inchi InChI=1S/C81H132/c1-3-7-67(8-4-1)11-15-69(16-12-67)19-23-71(24-20-69)27-31-73(32-28-71)35-39-75(40-36-73)43-47-77(48-44-75)51-55-79(56-52-77)59-63-81(64-60-79)65-61-80(62-66-81)57-53-78(54-58-80)49-45-76(46-50-78)41-37-74(38-42-76)33-29-72(30-34-74)25-21-70(22-26-72)17-13-68(14-18-70)9-5-2-6-10-68/h1-66H2
         */
        public IAtomContainer Pentadecaspiro
        {
            get
            {
                IAtomContainer mol = new AtomContainer();
                IAtom a1 = new Atom("C");
                mol.Add(a1);
                IAtom a2 = new Atom("C");
                mol.Add(a2);
                IAtom a3 = new Atom("C");
                mol.Add(a3);
                IAtom a4 = new Atom("C");
                mol.Add(a4);
                IAtom a5 = new Atom("C");
                mol.Add(a5);
                IAtom a6 = new Atom("C");
                mol.Add(a6);
                IAtom a7 = new Atom("C");
                mol.Add(a7);
                IAtom a8 = new Atom("C");
                mol.Add(a8);
                IAtom a9 = new Atom("C");
                mol.Add(a9);
                IAtom a10 = new Atom("C");
                mol.Add(a10);
                IAtom a11 = new Atom("C");
                mol.Add(a11);
                IAtom a12 = new Atom("C");
                mol.Add(a12);
                IAtom a13 = new Atom("C");
                mol.Add(a13);
                IAtom a14 = new Atom("C");
                mol.Add(a14);
                IAtom a15 = new Atom("C");
                mol.Add(a15);
                IAtom a16 = new Atom("C");
                mol.Add(a16);
                IAtom a17 = new Atom("C");
                mol.Add(a17);
                IAtom a18 = new Atom("C");
                mol.Add(a18);
                IAtom a19 = new Atom("C");
                mol.Add(a19);
                IAtom a20 = new Atom("C");
                mol.Add(a20);
                IAtom a21 = new Atom("C");
                mol.Add(a21);
                IAtom a22 = new Atom("C");
                mol.Add(a22);
                IAtom a23 = new Atom("C");
                mol.Add(a23);
                IAtom a24 = new Atom("C");
                mol.Add(a24);
                IAtom a25 = new Atom("C");
                mol.Add(a25);
                IAtom a26 = new Atom("C");
                mol.Add(a26);
                IAtom a27 = new Atom("C");
                mol.Add(a27);
                IAtom a28 = new Atom("C");
                mol.Add(a28);
                IAtom a29 = new Atom("C");
                mol.Add(a29);
                IAtom a30 = new Atom("C");
                mol.Add(a30);
                IAtom a31 = new Atom("C");
                mol.Add(a31);
                IAtom a32 = new Atom("C");
                mol.Add(a32);
                IAtom a33 = new Atom("C");
                mol.Add(a33);
                IAtom a34 = new Atom("C");
                mol.Add(a34);
                IAtom a35 = new Atom("C");
                mol.Add(a35);
                IAtom a36 = new Atom("C");
                mol.Add(a36);
                IAtom a37 = new Atom("C");
                mol.Add(a37);
                IAtom a38 = new Atom("C");
                mol.Add(a38);
                IAtom a39 = new Atom("C");
                mol.Add(a39);
                IAtom a40 = new Atom("C");
                mol.Add(a40);
                IAtom a41 = new Atom("C");
                mol.Add(a41);
                IAtom a42 = new Atom("C");
                mol.Add(a42);
                IAtom a43 = new Atom("C");
                mol.Add(a43);
                IAtom a44 = new Atom("C");
                mol.Add(a44);
                IAtom a45 = new Atom("C");
                mol.Add(a45);
                IAtom a46 = new Atom("C");
                mol.Add(a46);
                IAtom a47 = new Atom("C");
                mol.Add(a47);
                IAtom a48 = new Atom("C");
                mol.Add(a48);
                IAtom a49 = new Atom("C");
                mol.Add(a49);
                IAtom a50 = new Atom("C");
                mol.Add(a50);
                IAtom a51 = new Atom("C");
                mol.Add(a51);
                IAtom a52 = new Atom("C");
                mol.Add(a52);
                IAtom a53 = new Atom("C");
                mol.Add(a53);
                IAtom a54 = new Atom("C");
                mol.Add(a54);
                IAtom a55 = new Atom("C");
                mol.Add(a55);
                IAtom a56 = new Atom("C");
                mol.Add(a56);
                IAtom a57 = new Atom("C");
                mol.Add(a57);
                IAtom a58 = new Atom("C");
                mol.Add(a58);
                IAtom a59 = new Atom("C");
                mol.Add(a59);
                IAtom a60 = new Atom("C");
                mol.Add(a60);
                IAtom a61 = new Atom("C");
                mol.Add(a61);
                IAtom a62 = new Atom("C");
                mol.Add(a62);
                IAtom a63 = new Atom("C");
                mol.Add(a63);
                IAtom a64 = new Atom("C");
                mol.Add(a64);
                IAtom a65 = new Atom("C");
                mol.Add(a65);
                IAtom a66 = new Atom("C");
                mol.Add(a66);
                IAtom a67 = new Atom("C");
                mol.Add(a67);
                IAtom a68 = new Atom("C");
                mol.Add(a68);
                IAtom a69 = new Atom("C");
                mol.Add(a69);
                IAtom a70 = new Atom("C");
                mol.Add(a70);
                IAtom a71 = new Atom("C");
                mol.Add(a71);
                IAtom a72 = new Atom("C");
                mol.Add(a72);
                IAtom a73 = new Atom("C");
                mol.Add(a73);
                IAtom a74 = new Atom("C");
                mol.Add(a74);
                IAtom a75 = new Atom("C");
                mol.Add(a75);
                IAtom a76 = new Atom("C");
                mol.Add(a76);
                IAtom a77 = new Atom("C");
                mol.Add(a77);
                IAtom a78 = new Atom("C");
                mol.Add(a78);
                IAtom a79 = new Atom("C");
                mol.Add(a79);
                IAtom a80 = new Atom("C");
                mol.Add(a80);
                IAtom a81 = new Atom("C");
                mol.Add(a81);
                IBond b1 = new Bond(a3, a1);
                mol.Add(b1);
                IBond b2 = new Bond(a4, a1);
                mol.Add(b2);
                IBond b3 = new Bond(a5, a2);
                mol.Add(b3);
                IBond b4 = new Bond(a6, a2);
                mol.Add(b4);
                IBond b5 = new Bond(a7, a3);
                mol.Add(b5);
                IBond b6 = new Bond(a8, a4);
                mol.Add(b6);
                IBond b7 = new Bond(a9, a5);
                mol.Add(b7);
                IBond b8 = new Bond(a10, a6);
                mol.Add(b8);
                IBond b9 = new Bond(a15, a11);
                mol.Add(b9);
                IBond b10 = new Bond(a16, a12);
                mol.Add(b10);
                IBond b11 = new Bond(a17, a13);
                mol.Add(b11);
                IBond b12 = new Bond(a18, a14);
                mol.Add(b12);
                IBond b13 = new Bond(a23, a19);
                mol.Add(b13);
                IBond b14 = new Bond(a24, a20);
                mol.Add(b14);
                IBond b15 = new Bond(a25, a21);
                mol.Add(b15);
                IBond b16 = new Bond(a26, a22);
                mol.Add(b16);
                IBond b17 = new Bond(a31, a27);
                mol.Add(b17);
                IBond b18 = new Bond(a32, a28);
                mol.Add(b18);
                IBond b19 = new Bond(a33, a29);
                mol.Add(b19);
                IBond b20 = new Bond(a34, a30);
                mol.Add(b20);
                IBond b21 = new Bond(a39, a35);
                mol.Add(b21);
                IBond b22 = new Bond(a40, a36);
                mol.Add(b22);
                IBond b23 = new Bond(a41, a37);
                mol.Add(b23);
                IBond b24 = new Bond(a42, a38);
                mol.Add(b24);
                IBond b25 = new Bond(a47, a43);
                mol.Add(b25);
                IBond b26 = new Bond(a48, a44);
                mol.Add(b26);
                IBond b27 = new Bond(a49, a45);
                mol.Add(b27);
                IBond b28 = new Bond(a50, a46);
                mol.Add(b28);
                IBond b29 = new Bond(a55, a51);
                mol.Add(b29);
                IBond b30 = new Bond(a56, a52);
                mol.Add(b30);
                IBond b31 = new Bond(a57, a53);
                mol.Add(b31);
                IBond b32 = new Bond(a58, a54);
                mol.Add(b32);
                IBond b33 = new Bond(a63, a59);
                mol.Add(b33);
                IBond b34 = new Bond(a64, a60);
                mol.Add(b34);
                IBond b35 = new Bond(a65, a61);
                mol.Add(b35);
                IBond b36 = new Bond(a66, a62);
                mol.Add(b36);
                IBond b37 = new Bond(a67, a7);
                mol.Add(b37);
                IBond b38 = new Bond(a67, a8);
                mol.Add(b38);
                IBond b39 = new Bond(a67, a11);
                mol.Add(b39);
                IBond b40 = new Bond(a67, a12);
                mol.Add(b40);
                IBond b41 = new Bond(a68, a9);
                mol.Add(b41);
                IBond b42 = new Bond(a68, a10);
                mol.Add(b42);
                IBond b43 = new Bond(a68, a13);
                mol.Add(b43);
                IBond b44 = new Bond(a68, a14);
                mol.Add(b44);
                IBond b45 = new Bond(a69, a15);
                mol.Add(b45);
                IBond b46 = new Bond(a69, a16);
                mol.Add(b46);
                IBond b47 = new Bond(a69, a19);
                mol.Add(b47);
                IBond b48 = new Bond(a69, a20);
                mol.Add(b48);
                IBond b49 = new Bond(a70, a17);
                mol.Add(b49);
                IBond b50 = new Bond(a70, a18);
                mol.Add(b50);
                IBond b51 = new Bond(a70, a21);
                mol.Add(b51);
                IBond b52 = new Bond(a70, a22);
                mol.Add(b52);
                IBond b53 = new Bond(a71, a23);
                mol.Add(b53);
                IBond b54 = new Bond(a71, a24);
                mol.Add(b54);
                IBond b55 = new Bond(a71, a27);
                mol.Add(b55);
                IBond b56 = new Bond(a71, a28);
                mol.Add(b56);
                IBond b57 = new Bond(a72, a25);
                mol.Add(b57);
                IBond b58 = new Bond(a72, a26);
                mol.Add(b58);
                IBond b59 = new Bond(a72, a29);
                mol.Add(b59);
                IBond b60 = new Bond(a72, a30);
                mol.Add(b60);
                IBond b61 = new Bond(a73, a31);
                mol.Add(b61);
                IBond b62 = new Bond(a73, a32);
                mol.Add(b62);
                IBond b63 = new Bond(a73, a35);
                mol.Add(b63);
                IBond b64 = new Bond(a73, a36);
                mol.Add(b64);
                IBond b65 = new Bond(a74, a33);
                mol.Add(b65);
                IBond b66 = new Bond(a74, a34);
                mol.Add(b66);
                IBond b67 = new Bond(a74, a37);
                mol.Add(b67);
                IBond b68 = new Bond(a74, a38);
                mol.Add(b68);
                IBond b69 = new Bond(a75, a39);
                mol.Add(b69);
                IBond b70 = new Bond(a75, a40);
                mol.Add(b70);
                IBond b71 = new Bond(a75, a43);
                mol.Add(b71);
                IBond b72 = new Bond(a75, a44);
                mol.Add(b72);
                IBond b73 = new Bond(a76, a41);
                mol.Add(b73);
                IBond b74 = new Bond(a76, a42);
                mol.Add(b74);
                IBond b75 = new Bond(a76, a45);
                mol.Add(b75);
                IBond b76 = new Bond(a76, a46);
                mol.Add(b76);
                IBond b77 = new Bond(a77, a47);
                mol.Add(b77);
                IBond b78 = new Bond(a77, a48);
                mol.Add(b78);
                IBond b79 = new Bond(a77, a51);
                mol.Add(b79);
                IBond b80 = new Bond(a77, a52);
                mol.Add(b80);
                IBond b81 = new Bond(a78, a49);
                mol.Add(b81);
                IBond b82 = new Bond(a78, a50);
                mol.Add(b82);
                IBond b83 = new Bond(a78, a53);
                mol.Add(b83);
                IBond b84 = new Bond(a78, a54);
                mol.Add(b84);
                IBond b85 = new Bond(a79, a55);
                mol.Add(b85);
                IBond b86 = new Bond(a79, a56);
                mol.Add(b86);
                IBond b87 = new Bond(a79, a59);
                mol.Add(b87);
                IBond b88 = new Bond(a79, a60);
                mol.Add(b88);
                IBond b89 = new Bond(a80, a57);
                mol.Add(b89);
                IBond b90 = new Bond(a80, a58);
                mol.Add(b90);
                IBond b91 = new Bond(a80, a61);
                mol.Add(b91);
                IBond b92 = new Bond(a80, a62);
                mol.Add(b92);
                IBond b93 = new Bond(a81, a63);
                mol.Add(b93);
                IBond b94 = new Bond(a81, a64);
                mol.Add(b94);
                IBond b95 = new Bond(a81, a65);
                mol.Add(b95);
                IBond b96 = new Bond(a81, a66);
                mol.Add(b96);
                return mol;
            }
        }

        /**
         * spiro[5.5]undecane
         *
         * @cdk.inchi InChI=1S/C11H20/c1-3-7-11(8-4-1)9-5-2-6-10-11/h1-10H2
         */
        private static IAtomContainer Spiroundecane
        {
            get
            {
                IAtomContainer mol = new AtomContainer();
                IAtom a1 = new Atom("C");
                mol.Add(a1);
                IAtom a2 = new Atom("C");
                mol.Add(a2);
                IAtom a3 = new Atom("C");
                mol.Add(a3);
                IAtom a4 = new Atom("C");
                mol.Add(a4);
                IAtom a5 = new Atom("C");
                mol.Add(a5);
                IAtom a6 = new Atom("C");
                mol.Add(a6);
                IAtom a7 = new Atom("C");
                mol.Add(a7);
                IAtom a8 = new Atom("C");
                mol.Add(a8);
                IAtom a9 = new Atom("C");
                mol.Add(a9);
                IAtom a10 = new Atom("C");
                mol.Add(a10);
                IAtom a11 = new Atom("C");
                mol.Add(a11);
                IBond b1 = new Bond(a1, a2);
                mol.Add(b1);
                IBond b2 = new Bond(a1, a6);
                mol.Add(b2);
                IBond b3 = new Bond(a2, a3);
                mol.Add(b3);
                IBond b4 = new Bond(a3, a4);
                mol.Add(b4);
                IBond b5 = new Bond(a4, a5);
                mol.Add(b5);
                IBond b6 = new Bond(a5, a6);
                mol.Add(b6);
                IBond b7 = new Bond(a7, a5);
                mol.Add(b7);
                IBond b8 = new Bond(a5, a8);
                mol.Add(b8);
                IBond b9 = new Bond(a7, a11);
                mol.Add(b9);
                IBond b10 = new Bond(a8, a9);
                mol.Add(b10);
                IBond b11 = new Bond(a9, a10);
                mol.Add(b11);
                IBond b12 = new Bond(a10, a11);
                mol.Add(b12);
                return mol;
            }
        }
    }
}
