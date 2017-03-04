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
using System;
using static NCDK.Graphs.InitialCyclesTest;

namespace NCDK.Graphs
{
    /// <summary>
    // @author John May
    // @cdk.module test-core
    /// </summary>
    [TestClass()]
    public class MinimumCycleBasisTest
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void NoGraph()
        {
            new MinimumCycleBasis((int[][])null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void NoInitialCycles()
        {
            new MinimumCycleBasis((InitialCycles)null);
        }

        [TestMethod()]
        public virtual void Paths_norbornane()
        {
            int[][] norbornane = Norbornane;
            MinimumCycleBasis mcb = new MinimumCycleBasis(norbornane);
            int[][] paths = mcb.GetPaths();
            Assert.AreEqual(2, paths.Length);
            int[][] expected = new int[][] { new[] { 5, 6, 2, 1, 0, 5 }, new[] { 5, 6, 2, 3, 4, 5 } };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_bicyclo()
        {
            int[][] bicyclo = Bicyclo;
            MinimumCycleBasis mcb = new MinimumCycleBasis(bicyclo);
            int[][] paths = mcb.GetPaths();
            Assert.AreEqual(2, paths.Length);
            int[][] expected = new int[][] { new[] { 5, 0, 1, 2, 3, 4, 5 }, new[] { 5, 0, 1, 2, 7, 6, 5 } };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_napthalene()
        {
            int[][] napthalene = Naphthalene;
            MinimumCycleBasis mcb = new MinimumCycleBasis(napthalene);
            int[][] paths = mcb.GetPaths();
            Assert.AreEqual(2, paths.Length);
            int[][] expected = new int[][] { new[] { 5, 0, 1, 2, 3, 4, 5 }, new[] { 5, 4, 7, 8, 9, 6, 5 } };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_anthracene()
        {
            int[][] anthracene = Anthracene;
            MinimumCycleBasis mcb = new MinimumCycleBasis(anthracene);
            int[][] paths = mcb.GetPaths();
            Assert.AreEqual(3, paths.Length);
            int[][] expected = new int[][] { new[] { 5, 0, 1, 2, 3, 4, 5 }, new[] { 9, 6, 5, 4, 7, 8, 9 }, new[] { 9, 8, 10, 11, 12, 13, 9 } };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_cyclophane_even()
        {
            int[][] cyclophane_even = CyclophaneEven;
            MinimumCycleBasis mcb = new MinimumCycleBasis(cyclophane_even);
            int[][] paths = mcb.GetPaths();
            Assert.AreEqual(2, paths.Length);
            int[][] expected = new int[][] { new[] { 3, 2, 1, 0, 5, 4, 3 }, new[] { 3, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3 } };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_cyclophane_odd()
        {
            int[][] cyclophane_even = CyclophaneEven;
            MinimumCycleBasis mcb = new MinimumCycleBasis(cyclophane_even);
            int[][] paths = mcb.GetPaths();
            Assert.AreEqual(2, paths.Length);
            int[][] expected = new int[][] { new[] { 3, 2, 1, 0, 5, 4, 3 }, new[] { 3, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3 } };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Size_norbornane()
        {
            int[][] norbornane = Norbornane;
            MinimumCycleBasis mcb = new MinimumCycleBasis(norbornane);
            int[][] paths = mcb.GetPaths();
            Assert.AreEqual(2, paths.Length);
        }

        [TestMethod()]
        public virtual void Size_bicyclo()
        {
            int[][] bicyclo = Bicyclo;
            MinimumCycleBasis mcb = new MinimumCycleBasis(bicyclo);
            Assert.AreEqual(2, mcb.Count);
        }

        [TestMethod()]
        public virtual void Size_napthalene()
        {
            int[][] napthalene = Naphthalene;
            MinimumCycleBasis mcb = new MinimumCycleBasis(napthalene);
            Assert.AreEqual(2, mcb.Count);
        }

        [TestMethod()]
        public virtual void Size_anthracene()
        {
            int[][] anthracene = Anthracene;
            MinimumCycleBasis mcb = new MinimumCycleBasis(anthracene);
            Assert.AreEqual(3, mcb.Count);
        }

        [TestMethod()]
        public virtual void Size_cyclophane_even()
        {
            int[][] cyclophane_even = CyclophaneEven;
            MinimumCycleBasis relevant = new MinimumCycleBasis(cyclophane_even);
            Assert.AreEqual(2, relevant.Count);
        }

        [TestMethod()]
        public virtual void Size_cyclophane_odd()
        {
            int[][] cyclophane_even = CyclophaneEven;
            MinimumCycleBasis mcb = new MinimumCycleBasis(cyclophane_even);
            Assert.AreEqual(2, mcb.Count);
        }
    }
}
