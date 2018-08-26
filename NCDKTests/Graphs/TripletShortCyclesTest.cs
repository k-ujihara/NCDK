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
using static NCDK.Graphs.InitialCyclesTest;
using System;

namespace NCDK.Graphs
{
    // @author John May
    // @cdk.module test-core
    [TestClass()]
    public class TripletShortCyclesTest
    {
        [TestMethod()]
        public virtual void Lexicographic()
        {
            int[] exp = new int[] { 0, 1, 2, 3, 4 };

            Assert.IsTrue(Compares.AreDeepEqual(exp, TripletShortCycles.Lexicographic(new int[] { 0, 1, 2, 3, 4 })));
            Assert.IsTrue(Compares.AreDeepEqual(exp, TripletShortCycles.Lexicographic(new int[] { 4, 0, 1, 2, 3 })));
            Assert.IsTrue(Compares.AreDeepEqual(exp, TripletShortCycles.Lexicographic(new int[] { 3, 4, 0, 1, 2 })));
            Assert.IsTrue(Compares.AreDeepEqual(exp, TripletShortCycles.Lexicographic(new int[] { 2, 3, 4, 0, 1 })));
            Assert.IsTrue(Compares.AreDeepEqual(exp, TripletShortCycles.Lexicographic(new int[] { 1, 2, 3, 4, 0 })));

            Assert.IsTrue(Compares.AreDeepEqual(exp, TripletShortCycles.Lexicographic(new int[] { 4, 3, 2, 1, 0 })));
            Assert.IsTrue(Compares.AreDeepEqual(exp, TripletShortCycles.Lexicographic(new int[] { 3, 2, 1, 0, 4 })));
            Assert.IsTrue(Compares.AreDeepEqual(exp, TripletShortCycles.Lexicographic(new int[] { 2, 1, 0, 4, 3 })));
            Assert.IsTrue(Compares.AreDeepEqual(exp, TripletShortCycles.Lexicographic(new int[] { 1, 0, 4, 3, 2 })));
            Assert.IsTrue(Compares.AreDeepEqual(exp, TripletShortCycles.Lexicographic(new int[] { 0, 4, 3, 2, 1 })));
        }

        [TestMethod()]
        public virtual void IsEmpty()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Array.Empty<int[]>()), false);
            int[][] paths = esssr.GetPaths();
            Assert.IsTrue(Compares.AreDeepEqual(Array.Empty<int[]>(), paths));
        }

        [TestMethod()]
        public virtual void Unmodifiable()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(K4), false);
            int[][] paths = esssr.GetPaths();
            // modify paths
            foreach (var path in paths)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    path[i] = path[i] + 1;
                }
            }
            // the internal paths should not be changed
            Assert.IsFalse(Compares.AreDeepEqual(esssr.GetPaths(), paths));
        }

        [TestMethod()]
        public virtual void NaphthalenePaths()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Naphthalene), false);
            int[][] paths = esssr.GetPaths();
            Assert.AreEqual(6, paths[0].Length - 1);
            Assert.AreEqual(6, paths[1].Length - 1);
            Assert.IsFalse(Compares.AreDeepEqual(paths[0], paths[1]));
            Assert.AreEqual(10, paths[2].Length - 1);
        }

        [TestMethod()]
        public virtual void NaphthaleneSize()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Naphthalene), false);
            Assert.AreEqual(3, esssr.Count);
        }

        [TestMethod()]
        public virtual void AnthracenePaths()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Anthracene), false);
            int[][] paths = esssr.GetPaths();
            Assert.AreEqual(6, paths[0].Length - 1);
            Assert.AreEqual(6, paths[1].Length - 1);
            Assert.AreEqual(6, paths[2].Length - 1);

            Assert.IsFalse(Compares.AreDeepEqual(paths[0], paths[1]));
            Assert.IsFalse(Compares.AreDeepEqual(paths[0], paths[2]));
            Assert.IsFalse(Compares.AreDeepEqual(paths[1], paths[2]));

            Assert.AreEqual(10, paths[3].Length - 1);
            Assert.AreEqual(10, paths[4].Length - 1);

            Assert.IsFalse(Compares.AreDeepEqual(paths[3], paths[4]));
        }

        [TestMethod()]
        public virtual void AnthraceneSize()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Anthracene), false);
            Assert.AreEqual(5, esssr.Count);
        }

        [TestMethod()]
        public virtual void BicycloPaths()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Bicyclo), false);
            int[][] paths = esssr.GetPaths();
            Assert.AreEqual(6, paths[0].Length - 1);
            Assert.AreEqual(6, paths[1].Length - 1);
            Assert.AreEqual(6, paths[2].Length - 1);

            Assert.IsFalse(Compares.AreDeepEqual(paths[0], paths[1]));
            Assert.IsFalse(Compares.AreDeepEqual(paths[0], paths[2]));
            Assert.IsFalse(Compares.AreDeepEqual(paths[1], paths[2]));
        }

        [TestMethod()]
        public virtual void BicycloSize()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Bicyclo), false);
            Assert.AreEqual(3, esssr.Count);
        }

        [TestMethod()]
        public virtual void NorbornanePaths()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Norbornane), false);
            int[][] paths = esssr.GetPaths();
            Assert.AreEqual(5, paths[0].Length - 1);
            Assert.AreEqual(5, paths[1].Length - 1);
            Assert.AreEqual(6, paths[2].Length - 1);

            Assert.IsFalse(Compares.AreDeepEqual(paths[0], paths[1]));
        }

        [TestMethod()]
        public virtual void NorbornaneSize()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Norbornane), false);
            Assert.AreEqual(3, esssr.Count);
        }

        /// <summary>
        /// Ensures non-canonic really does give you all cycles. If we didn't use
        /// multiple shortest paths here we would miss one of the larger cycles
        /// </summary>
        [TestMethod()]
        public virtual void CyclophanePaths()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Cyclophane()), false);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]
                {
                    new[] {0, 1, 2, 3, 4, 5, 0},
                    new[] {6, 7, 8, 9, 10, 11, 6},
                    new[] {0, 1, 2, 3, 17, 18, 19, 16, 9, 8, 7, 6, 15, 14, 13, 12, 0},
                    new[] {0, 1, 2, 3, 17, 18, 19, 16, 9, 10, 11, 6, 15, 14, 13, 12, 0},
                    new[] {0, 5, 4, 3, 17, 18, 19, 16, 9, 8, 7, 6, 15, 14, 13, 12, 0},
                    new[] {0, 5, 4, 3, 17, 18, 19, 16, 9, 10, 11, 6, 15, 14, 13, 12, 0},
                },
                esssr.GetPaths()));
        }

        [TestMethod()]
        public virtual void CyclophaneSize()
        {
            TripletShortCycles esssr = new TripletShortCycles(new MinimumCycleBasis(Cyclophane()), false);
            Assert.AreEqual(6, esssr.Count);
        }

        // @cdk.inchi InChI=1S/C20H36/c1-2-6-18-13-15-20(16-14-18)8-4-3-7-19-11-9-17(5-1)10-12-19/h17-20H,1-16H2
        static int[][] Cyclophane()
        {
            return new int[][]{
                new[] {1, 5, 12}, new[] {0, 2}, new[] {1, 3}, new[] {2, 4, 17}, new[] {3, 5}, new[] {4, 0}, new[] {7, 11, 15}, new[] {6, 8}, new[] {7, 9},
                new[] {8, 10, 16}, new[] {9, 11}, new[] {10, 6}, new[] {0, 13}, new[] {12, 14}, new[] {13, 15}, new[] {14, 6}, new[] {9, 19}, new[] {3, 18}, new[] {17, 19},
                new[] {18, 16}};
        }
    }
}
