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
using NCDK.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Graphs
{
    [TestClass()]
    public class JumboPathGraphTests
    {
        // RegularPathGraph
        static PrivateObject NewPathGraph(int[][] mGraph, int[] rank, int limit)
            => new PrivateObject("NCDK", "NCDK.Graphs.JumboPathGraph", mGraph, rank, limit);
        // RegularPathGraph.SimpleEdge
        static PrivateObject NewSimpleEdge(int u, int v)
            => new PrivateObject("NCDK", "NCDK.Graphs.JumboPathGraph+SimpleEdge", u, v, 8);
        static PrivateObject NewSimpleEdge(int u, int v, int size)
            => new PrivateObject("NCDK", "NCDK.Graphs.JumboPathGraph+SimpleEdge", u, v, size);
        // RegularPathGraph.ReducedEdge(RegularPathGraph.PathEdge, RegularPathGraph.PathEdge, int)
        static PrivateObject NewReducedEdge(PrivateObject e, PrivateObject f, int x)
            => new PrivateObject("NCDK", "NCDK.Graphs.JumboPathGraph+ReducedEdge", e.Target, f.Target, x);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void NullMGraph()
        {
            try
            {
                NewPathGraph(null, new int[0], 0);
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
                NewPathGraph(new int[4][], new int[0], -1);
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
                NewPathGraph(new int[4][], new int[0], 5);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        /* re-invoking the Remove on the same vertex should not do anything */
        [TestMethod()]
        public virtual void repeatRemoval()
        {
            int ord = 3;
            int[][] k3 = CompleteGraphOfSize(ord);
            var pg = NewPathGraph(k3, Identity(3), ord);
            List<int[]> cycles = new List<int[]>();
            pg.Invoke("Remove", new object[] { 0, cycles });
            Assert.AreEqual(0, cycles.Count);
            pg.Invoke("Remove", new object[] { 0, cycles });
            Assert.AreEqual(0, cycles.Count);
            pg.Invoke("Remove", new object[] { 1, cycles });
            Assert.AreEqual(1, cycles.Count);
            pg.Invoke("Remove", new object[] { 1, cycles });
            Assert.AreEqual(1, cycles.Count);
            pg.Invoke("Remove", new object[] { 2, cycles });
            Assert.AreEqual(1, cycles.Count);
            pg.Invoke("Remove", new object[] { 2, cycles });
            Assert.AreEqual(1, cycles.Count);
        }

        [TestMethod()]
        public virtual void K3Degree()
        {
            int ord = 3;
            int[][] k3 = CompleteGraphOfSize(ord);
            var pg = NewPathGraph(k3, Identity(3), ord);
            // note - vertices are only added to either vertex (lowest rank)
            Assert.AreEqual(2, pg.Invoke("Degree", new object[] { 0 }));
            Assert.AreEqual(1, pg.Invoke("Degree", new object[] { 1 }));
            Assert.AreEqual(0, pg.Invoke("Degree", new object[] { 2 }));
            pg.Invoke("Remove", new object[] { 0, new List<int[]>(0) });
            Assert.AreEqual(0, pg.Invoke("Degree", new object[] { 0 }));
            Assert.AreEqual(2, pg.Invoke("Degree", new object[] { 1 }));
            Assert.AreEqual(0, pg.Invoke("Degree", new object[] { 2 }));
            pg.Invoke("Remove", new object[] { 1, new List<int[]>(0) });
            Assert.AreEqual(0, pg.Invoke("Degree", new object[] { 0 }));
            Assert.AreEqual(0, pg.Invoke("Degree", new object[] { 1 }));
            Assert.AreEqual(0, pg.Invoke("Degree", new object[] { 2 }));
        }

        /* graph with 3 vertices where each vertex is connected to every vertex. */
        [TestMethod()]
        [Timeout(200)]
        public virtual void K3()
        {
            int ord = 3;
            int[][] k3 = CompleteGraphOfSize(ord);
            var pg = NewPathGraph(k3, Identity(3), ord);
            List<int[]> cycles = new List<int[]>();
            for (int v = 0; v < ord; v++)
                pg.Invoke("Remove", v, cycles);
            Assert.AreEqual(1, cycles.Count);
        }

        /* graph with 8 vertices where each vertex is connected to every vertex. */
        [TestMethod()]
        [Timeout(2000)]
        public virtual void K8()
        {
            int ord = 8;
            int[][] k8 = CompleteGraphOfSize(ord);
            var pg = NewPathGraph(k8, Identity(8), ord);
            List<int[]> cycles = new List<int[]>();
            for (int v = 0; v < ord; v++)
                pg.Invoke("Remove", v, cycles);
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
        static int[][] CompleteGraphOfSize(int n)
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
            Assert.IsFalse((bool)NewSimpleEdge(0, 1).GetProperty("IsLoop"));
            Assert.IsTrue((bool)NewSimpleEdge(0, 0).GetProperty("IsLoop"));
            Assert.IsFalse((bool)NewReducedEdge(NewSimpleEdge(0, 1), NewSimpleEdge(2, 1), 1).GetProperty("IsLoop"));
            Assert.IsTrue((bool)NewReducedEdge(NewSimpleEdge(0, 1), NewSimpleEdge(0, 1), 0).GetProperty("IsLoop"));
            Assert.IsTrue((bool)NewReducedEdge(NewSimpleEdge(0, 1), NewSimpleEdge(0, 1), 1).GetProperty("IsLoop"));
        }

        [TestMethod()]
        public virtual void Path()
        {
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 1 },
                NewSimpleEdge(0, 1).Invoke("Path")));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 1, 2 },
                NewReducedEdge(NewSimpleEdge(0, 1), NewSimpleEdge(2, 1), 1).Invoke("Path")));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 1, 2, 3, 4 },
                NewReducedEdge(NewReducedEdge(NewSimpleEdge(0, 1), NewSimpleEdge(2, 1), 1), NewReducedEdge(
                    NewSimpleEdge(2, 3), NewSimpleEdge(3, 4), 3), 2).Invoke("Path")));
        }

        [TestMethod()]
        public virtual void Disjoint()
        {
            var e = NewReducedEdge(NewSimpleEdge(0, 1), NewSimpleEdge(2, 1), 1);
            var f = NewReducedEdge(NewSimpleEdge(2, 3), NewSimpleEdge(3, 4), 3);
            Assert.IsTrue((bool)e.Invoke("Disjoint", new object[] { f.Target }));
            Assert.IsTrue((bool)f.Invoke("Disjoint", new object[] { e.Target }));
            Assert.IsFalse((bool)e.Invoke("Disjoint", new object[] { e.Target }));
            Assert.IsFalse((bool)f.Invoke("Disjoint", new object[] { f.Target }));  // fixed CDK's bug
        }

        [TestMethod()]
        public virtual void Len()
        {
            Assert.AreEqual(2, NewSimpleEdge(0, 1).Invoke("Len"));
            var e = NewReducedEdge(NewSimpleEdge(0, 1), NewSimpleEdge(2, 1), 1);
            var f = NewReducedEdge(NewSimpleEdge(2, 3), NewSimpleEdge(3, 4), 3);
            Assert.AreEqual(3, e.Invoke("Len"));
            Assert.AreEqual(3, f.Invoke("Len"));
            Assert.AreEqual(5, NewReducedEdge(e, f, 2).Invoke("Len"));
        }
    }
}
