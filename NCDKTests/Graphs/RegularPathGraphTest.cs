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
using System.Collections.Generic;
using System.Reflection;
using static NCDK.Graphs.RegularPathGraph;

namespace NCDK.Graphs
{
    [TestClass()]
    public class RegularPathGraphTest
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void NullMGraph()
        {
            try
            {
                new RegularPathGraph(null, new int[0], 0);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public virtual void LimitTooLow()
        {
            try
            {
                new RegularPathGraph(new int[4][], new int[0], -1);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public virtual void LimitTooHigh()
        {
            try
            {
                new RegularPathGraph(new int[4][], new int[0], 5);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        /* re-invoking the Remove on the same vertex should not do anything */
        [TestMethod()]
        public virtual void RepeatRemoval()
        {
            int ord = 3;
            int[][] k3 = CompleteGraphOfSize(ord);
            var pg = new RegularPathGraph(k3, Identity(3), ord);
            List<int[]> cycles = new List<int[]>();
            pg.Remove(0, cycles);
            Assert.AreEqual(0, cycles.Count);
            pg.Remove(0, cycles);
            Assert.AreEqual(0, cycles.Count);
            pg.Remove(1, cycles);
            Assert.AreEqual(1, cycles.Count);
            pg.Remove(1, cycles);
            Assert.AreEqual(1, cycles.Count);
            pg.Remove(2, cycles);
            Assert.AreEqual(1, cycles.Count);
            pg.Remove(2, cycles);
            Assert.AreEqual(1, cycles.Count);
        }

        [TestMethod()]
        public virtual void K3Degree()
        {
            int ord = 3;
            int[][] k3 = CompleteGraphOfSize(ord);
            var pg = new RegularPathGraph(k3, Identity(3), ord);
            // note - vertices are only added to either vertex (lowest rank)
            Assert.AreEqual(2, pg.Degree(0));
            Assert.AreEqual(1, pg.Degree(1));
            Assert.AreEqual(0, pg.Degree(2));
            pg.Remove(0, new List<int[]>(0));
            Assert.AreEqual(0, pg.Degree(0));
            Assert.AreEqual(2, pg.Degree(1));
            Assert.AreEqual(0, pg.Degree(2));
            pg.Remove(1, new List<int[]>(0));
            Assert.AreEqual(0, pg.Degree(0));
            Assert.AreEqual(0, pg.Degree(1));
            Assert.AreEqual(0, pg.Degree(2));
        }

        /* graph with 3 vertices where each vertex is connected to every vertex. */
        [TestMethod()]
        [Timeout(200)]
        public virtual void K3()
        {
            int ord = 3;
            int[][] k3 = CompleteGraphOfSize(ord);
            var pg = new RegularPathGraph(k3, Identity(3), ord);
            List<int[]> cycles = new List<int[]>();
            for (int v = 0; v < ord; v++)
                pg.Remove(v, cycles);
            Assert.AreEqual(1, cycles.Count);
        }

        /* graph with 8 vertices where each vertex is connected to every vertex. */
        [TestMethod()]
        [Timeout(200)]
        public virtual void K8()
        {
            int ord = 8;
            int[][] k8 = CompleteGraphOfSize(ord);
            var pg = new RegularPathGraph(k8, Identity(8), ord);
            List<int[]> cycles = new List<int[]>();
            for (int v = 0; v < ord; v++)
                pg.Remove(v, cycles);
            Assert.AreEqual(8018, cycles.Count);
        }

        public static int[] Identity(int n)
        {
            int[] identity = new int[n];
            for (int i = 0; i < n; i++)
                identity[i] = i;
            return identity;
        }

        /* make a complete graph */
        internal static int[][] CompleteGraphOfSize(int n)
        {
            int[][] g = new int[n][];
            for (int v = 0; v < n; v++)
            {
                g[v] = new int[n - 1];
                int deg = 0;
                for (int w = 0; w < n; w++)
                {
                    if (v != w)
                    {
                        g[v][deg++] = w;
                    }
                }
            }
            return g;
        }

        [TestMethod()]
        public virtual void Loop()
        {
            var aa = new SimpleEdge(0, 1);
            Assert.IsFalse(new SimpleEdge(0, 1).IsLoop);
            Assert.IsTrue(new SimpleEdge(0, 0).IsLoop);
            Assert.IsFalse(new ReducedEdge(new SimpleEdge(0, 1), new SimpleEdge(2, 1), 1).IsLoop);
            Assert.IsTrue(new ReducedEdge(new SimpleEdge(0, 1), new SimpleEdge(0, 1), 0).IsLoop);
            Assert.IsTrue(new ReducedEdge(new SimpleEdge(0, 1), new SimpleEdge(0, 1), 1).IsLoop);
        }

        [TestMethod()]
        public virtual void Path()
        {
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 1 },
                new SimpleEdge(0, 1).Path()));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 1, 2 },
                new ReducedEdge(new SimpleEdge(0, 1), new SimpleEdge(2, 1), 1).Path()));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 1, 2, 3, 4 },
                new ReducedEdge(new ReducedEdge(new SimpleEdge(0, 1), new SimpleEdge(2, 1), 1), new ReducedEdge(
                    new SimpleEdge(2, 3), new SimpleEdge(3, 4), 3), 2).Path()));
        }

        [TestMethod()]
        public virtual void Disjoint()
        {
            var e = new ReducedEdge(new SimpleEdge(0, 1), new SimpleEdge(2, 1), 1);
            var f = new ReducedEdge(new SimpleEdge(2, 3), new SimpleEdge(3, 4), 3);
            Assert.IsTrue(e.Disjoint(f));
            Assert.IsTrue(f.Disjoint(e));
            Assert.IsFalse(e.Disjoint(e));
            Assert.IsFalse(f.Disjoint(f));
        }

        [TestMethod()]
        public virtual void Len()
        {
            Assert.AreEqual(2, new SimpleEdge(0, 1).Length);
            var e = new ReducedEdge(new SimpleEdge(0, 1), new SimpleEdge(2, 1), 1);
            var f = new ReducedEdge(new SimpleEdge(2, 3), new SimpleEdge(3, 4), 3);
            Assert.AreEqual(3, e.Length);
            Assert.AreEqual(3, f.Length);
            Assert.AreEqual(5, new ReducedEdge(e, f, 2).Length);
        }
    }
}
