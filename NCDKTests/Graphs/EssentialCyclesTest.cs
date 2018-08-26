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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NCDK.Graphs.InitialCyclesTest;
using NCDK.Common.Base;
using System;

namespace NCDK.Graphs
{
    // @author John May
    // @cdk.module test-core
    [TestClass()]
    public class EssentialCyclesTest
    {
        [TestMethod()]
        public virtual void Paths_bicyclo()
        {
            int[][] bicyclo = Bicyclo;
            EssentialCycles essential = new EssentialCycles(bicyclo);
            int[][] paths = essential.GetPaths();
            Assert.IsTrue(Compares.AreDeepEqual(Array.Empty<object[]>(), paths));
        }

        [TestMethod()]
        public virtual void Paths_norbornane()
        {
            int[][] norbornane = Norbornane;
            EssentialCycles essential = new EssentialCycles(norbornane);
            int[][] paths = essential.GetPaths();
            Assert.AreEqual(2, paths.Length);
            int[][] expected = new int[][] { new[] { 5, 6, 2, 1, 0, 5 }, new[] { 5, 6, 2, 3, 4, 5 } };
            Assert.IsTrue(Compares.AreDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_napthalene()
        {
            int[][] napthalene = Naphthalene;
            EssentialCycles essential = new EssentialCycles(napthalene);
            int[][] paths = essential.GetPaths();
            int[][] expected = new int[][] { new[] { 5, 0, 1, 2, 3, 4, 5 }, new[] { 5, 4, 7, 8, 9, 6, 5 } };
            Assert.IsTrue(Compares.AreDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_anthracene()
        {
            int[][] anthracene = Anthracene;
            EssentialCycles essential = new EssentialCycles(anthracene);
            int[][] paths = essential.GetPaths();
            int[][] expected = new int[][] { new[] { 5, 0, 1, 2, 3, 4, 5 }, new[] { 9, 6, 5, 4, 7, 8, 9 }, new[] { 9, 8, 10, 11, 12, 13, 9 } };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_cyclophane_even()
        {
            int[][] cyclophane_even = CyclophaneEven;
            EssentialCycles essential = new EssentialCycles(cyclophane_even);
            int[][] paths = essential.GetPaths();
            int[][] expected = new int[][] { new[] { 3, 2, 1, 0, 5, 4, 3 } };
            Assert.IsTrue(Compares.AreDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_cyclophane_odd()
        {
            int[][] cyclophane_even = CyclophaneEven;
            EssentialCycles essential = new EssentialCycles(cyclophane_even);
            int[][] paths = essential.GetPaths();
            int[][] expected = new int[][] { new[] { 3, 2, 1, 0, 5, 4, 3 } };
            Assert.IsTrue(Compares.AreDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Size_norbornane()
        {
            int[][] norbornane = Norbornane;
            EssentialCycles essential = new EssentialCycles(norbornane);
            Assert.AreEqual(2, essential.Count);
        }

        [TestMethod()]
        public virtual void Size_bicyclo()
        {
            int[][] bicyclo = Bicyclo;
            EssentialCycles essential = new EssentialCycles(bicyclo);
            Assert.AreEqual(0, essential.Count);
        }

        [TestMethod()]
        public virtual void Size_napthalene()
        {
            int[][] napthalene = Naphthalene;
            EssentialCycles essential = new EssentialCycles(napthalene);
            Assert.AreEqual(2, essential.Count);
        }

        [TestMethod()]
        public virtual void Size_anthracene()
        {
            int[][] anthracene = Anthracene;
            EssentialCycles essential = new EssentialCycles(anthracene);
            Assert.AreEqual(3, essential.Count);
        }

        [TestMethod()]
        public virtual void Size_cyclophane_even()
        {
            int[][] cyclophane_even = CyclophaneEven;
            EssentialCycles relevant = new EssentialCycles(cyclophane_even);
            Assert.AreEqual(1, relevant.Count);
        }

        [TestMethod()]
        public virtual void Size_cyclophane_odd()
        {
            int[][] cyclophane_even = CyclophaneEven;
            EssentialCycles essential = new EssentialCycles(cyclophane_even);
            Assert.AreEqual(1, essential.Count);
        }
    }
}
