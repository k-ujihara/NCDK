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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Graphs
{
    [TestClass()]
    public class InitialCyclesTest
    {
        [TestMethod()]
        public virtual void Lengths_empty()
        {
            Assert.IsFalse(new InitialCycles(new int[0][]).Lengths.Count() > 0);
        }

        [TestMethod()]
        public virtual void CyclesOfLength_empty()
        {
            Assert.IsTrue(new InitialCycles(new int[0][]).GetCyclesOfLength(0).Count() == 0);
        }

        [TestMethod()]
        public virtual void Graph()
        {
            int[][] g = new int[0][];
            Assert.AreSame(g, new InitialCycles(g).Graph);
        }

        [TestMethod()]
        public virtual void Lengths_K1()
        {
            Assert.IsFalse(new InitialCycles(K1).Lengths.Count() > 0);
        }

        [TestMethod()]
        public virtual void Cycles_K1()
        {
            InitialCycles initial = new InitialCycles(K1);
            Assert.AreEqual(0, initial.GetCycles().Count());
        }

        [TestMethod()]
        public virtual void Lengths_K4()
        {
            Assert.IsTrue(new InitialCycles(K4).Lengths.Contains(3));
        }

        [TestMethod()]
        public virtual void Cycles_K4()
        {
            InitialCycles initial = new InitialCycles(K4);
            // todo replace with hasSize (hamcrest)
            Assert.AreEqual(4, initial.GetCycles().Count());
            Assert.AreEqual(4, initial.GetCyclesOfLength(3).Count());
        }

        [TestMethod()]
        public virtual void NumberOfCycles_K4()
        {
            Assert.AreEqual(4, new InitialCycles(K4).GetNumberOfCycles());
        }

        [TestMethod()]
        public virtual void NumberOfEdges_K4()
        {
            Assert.AreEqual(6, new InitialCycles(K4).GetNumberOfEdges());
        }

        [TestMethod()]
        public virtual void IndexOfEdge_K4()
        {
            InitialCycles initial = new InitialCycles(K4);
            Assert.AreEqual(0, initial.IndexOfEdge(0, 1));
            Assert.AreEqual(0, initial.IndexOfEdge(1, 0));
            Assert.AreEqual(1, initial.IndexOfEdge(0, 2));
            Assert.AreEqual(1, initial.IndexOfEdge(2, 0)); ;
            Assert.AreEqual(2, initial.IndexOfEdge(0, 3));
            Assert.AreEqual(2, initial.IndexOfEdge(3, 0));
            Assert.AreEqual(3, initial.IndexOfEdge(1, 2));
            Assert.AreEqual(3, initial.IndexOfEdge(1, 2));
            Assert.AreEqual(4, initial.IndexOfEdge(1, 3));
            Assert.AreEqual(4, initial.IndexOfEdge(1, 3));
            Assert.AreEqual(5, initial.IndexOfEdge(2, 3));
            Assert.AreEqual(5, initial.IndexOfEdge(2, 3));
        }

        [TestMethod()]
        public virtual void Edge_K4()
        {
            InitialCycles initial = new InitialCycles(K4);
            Assert.AreEqual(initial.GetEdge(0), new InitialCycles.Cycle.Edge(0, 1));
            Assert.AreEqual(initial.GetEdge(1), new InitialCycles.Cycle.Edge(0, 2));
            Assert.AreEqual(initial.GetEdge(2), new InitialCycles.Cycle.Edge(0, 3));
            Assert.AreEqual(initial.GetEdge(3), new InitialCycles.Cycle.Edge(1, 2));
            Assert.AreEqual(initial.GetEdge(4), new InitialCycles.Cycle.Edge(1, 3));
            Assert.AreEqual(initial.GetEdge(5), new InitialCycles.Cycle.Edge(2, 3));
        }

        [TestMethod()]
        public virtual void ToEdgeVector_K4()
        {
            InitialCycles initial = new InitialCycles(K4);
            BitArray expected, actual;

            expected = new BitArray(new bool[] { true, false, true, true, false, true });
            actual = initial.ToEdgeVector(new int[] { 0, 1, 2, 3, 0 });
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));

            expected = new BitArray(new bool[] { true, true, false, true, false, false });
            actual = initial.ToEdgeVector(new int[] { 0, 1, 2, 0 });
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        private static void AssertInitialCycles(int[][] expecteds, IEnumerable<InitialCycles.Cycle> cycles)
        {
            for (int i = 0; i < expecteds.Length; i++)
            {
                var actual = cycles.ElementAt(i).Path;
                Assert.IsTrue(expecteds.Any(n => Compares.AreDeepEqual(n, actual)));
            }
        }

        [TestMethod()]
        public virtual void Lengths_naphthalene()
        {
            Assert.IsTrue(new InitialCycles(Naphthalene).Lengths.Contains(6));
        }

        [TestMethod()]
        public virtual void Cycles_naphthalene()
        {
            InitialCycles initial = new InitialCycles(Naphthalene);
            var cycles = initial.GetCycles();
            Assert.AreEqual(2, cycles.Count());
            int[][] expecteds = new int[][] {
                new int[] { 5, 0, 1, 2, 3, 4, 5 },
                new int[] { 5, 4, 7, 8, 9, 6, 5 },
            };
            AssertInitialCycles(expecteds, cycles);
        }

        [TestMethod()]
        public virtual void Lengths_anthracene()
        {
            Assert.IsTrue(new InitialCycles(Anthracene).Lengths.Contains(6));
        }

        [TestMethod()]
        public virtual void Cycles_anthracene()
        {
            InitialCycles initial = new InitialCycles(Anthracene);
            var cycles = initial.GetCycles();
            Assert.AreEqual(3, cycles.Count());

            int[][] expecteds = new int[][]
            {
                new int[] { 5, 0, 1, 2, 3, 4, 5 },
                new int[] { 9, 6, 5, 4, 7, 8, 9 },
                new int[] { 9, 8, 10, 11, 12, 13, 9 },
            };
            AssertInitialCycles(expecteds, cycles);
        }

        [TestMethod()]
        public virtual void Lengths_bicyclo()
        {
            Assert.IsTrue(new InitialCycles(Bicyclo).Lengths.Contains(6));
        }

        [TestMethod()]
        public virtual void Cycles_bicyclo()
        {
            InitialCycles initial = new InitialCycles(Bicyclo);
            var cycles = initial.GetCycles();
            Assert.AreEqual(3, cycles.Count());
            int[][] expecteds = new int[][] {
                new int[] { 5, 0, 1, 2, 3, 4, 5 },
                new int[] { 5, 0, 1, 2, 7, 6, 5 },
                new int[] { 5, 4, 3, 2, 7, 6, 5 },
            };
            AssertInitialCycles(expecteds, cycles);
        }

        [TestMethod()]
        public virtual void Lengths_cyclophane()
        {
            InitialCycles initial = new InitialCycles(CyclophaneOdd);
            foreach (var i in new int[] { 6, 9 })
                Assert.IsTrue(initial.Lengths.Contains(i));
            Assert.AreEqual(2, initial.Lengths.Count());
        }

        [TestMethod()]
        public virtual void Cycles_cyclophane()
        {
            InitialCycles initial = new InitialCycles(CyclophaneOdd);
            var cycles = initial.GetCycles();
            Assert.AreEqual(2, cycles.Count());
            int[][] expecteds = new int[][] {
                new int[] { 3, 2, 1, 0, 5, 4, 3 },
                new int[] { 3, 2, 1, 0, 10, 9, 8, 7, 6, 3 },
            };
            AssertInitialCycles(expecteds, cycles);
        }

        [TestMethod()]
        public virtual void Cycles_cyclophane_odd_limit_5()
        {
            InitialCycles initial = new InitialCycles(CyclophaneOdd, 5);
            var cycles = initial.GetCycles();
            Assert.AreEqual(0, cycles.Count());
        }

        [TestMethod()]
        public virtual void Cycles_cyclophane_odd_limit_6()
        {
            InitialCycles initial = new InitialCycles(CyclophaneOdd, 6);
            var cycles = initial.GetCycles();
            Assert.AreEqual(1, cycles.Count());
            int[][] expecteds = new int[][] {
                new int[] { 3, 2, 1, 0, 5, 4, 3 },
            };
            AssertInitialCycles(expecteds, cycles);
        }

        [TestMethod()]
        public virtual void Cycles_cyclophane_odd_limit_7()
        {
            InitialCycles initial = new InitialCycles(CyclophaneOdd, 7);
            var cycles = initial.GetCycles();
            Assert.AreEqual(1, cycles.Count());
            int[][] expecteds = new int[][] {
                new int[] { 3, 2, 1, 0, 5, 4, 3 },
            };
            AssertInitialCycles(expecteds, cycles);
        }

        [TestMethod()]
        public virtual void Cycles_family_odd()
        {
            InitialCycles initial = new InitialCycles(CyclophaneOdd);
            var cycles = initial.GetCycles();
            int[] expected = new int[] { 3, 2, 1, 0, 10, 9, 8, 7, 6, 3 };
            var cycle = cycles.FirstOrDefault(n => Compares.AreDeepEqual(expected, n.Path));
            Assert.IsNotNull(cycle);
            int[][] family = cycle.GetFamily();
            Assert.AreEqual(2, family.Length);
            int[][] expecteds = new int[][] {
                expected,
                new int[] { 3, 4, 5, 0, 10, 9, 8, 7, 6, 3 },
            };
            foreach (var f in family)
                Assert.IsTrue(expecteds.Any(n => Compares.AreDeepEqual(n, f)));
        }

        [TestMethod()]
        public virtual void Cycles_family_even()
        {
            InitialCycles initial = new InitialCycles(CyclophaneEven);
            var cycles = initial.GetCycles();
            int[] expected = new int[] { 3, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3 };
            var cycle = cycles.FirstOrDefault(n => Compares.AreDeepEqual(expected, n.Path));
            Assert.IsNotNull(cycle);
            int[][] family = cycle.GetFamily();
            Assert.AreEqual(2, family.Length);
            int[][] expecteds = new int[][] {
                expected,
                new int[] { 3, 6, 7, 8, 9, 10, 11, 0, 5, 4, 3 },
            };
            foreach (var f in family)
                Assert.IsTrue(expecteds.Any(n => Compares.AreDeepEqual(n, f)));
        }

        // ensure using the biconnected optimisation will still find the cycle in
        // a simple cycle, cylcohexane (there are no vertices with deg 3)
        [TestMethod()]
        public virtual void Bioconnected_simpleCycle()
        {
            InitialCycles ic = InitialCycles.OfBiconnectedComponent(Cyclohexane);
            Assert.AreEqual(1, ic.GetNumberOfCycles());
        }

        [TestMethod()]
        public virtual void Bioconnected_simpleCycle_limit_5()
        {
            InitialCycles ic = InitialCycles.OfBiconnectedComponent(Cyclohexane, 5);
            Assert.AreEqual(0, ic.GetNumberOfCycles());
        }

        [TestMethod()]
        public virtual void Bioconnected_simpleCycle_limit_6()
        {
            InitialCycles ic = InitialCycles.OfBiconnectedComponent(Cyclohexane, 6);
            Assert.AreEqual(1, ic.GetNumberOfCycles());
        }

        [TestMethod()]
        public virtual void Join()
        {
            int[] a = new int[] { 0, 1, 2 };
            int[] b = new int[] { 0, 3, 4 };
            var expected = new int[] { 0, 1, 2, 4, 3, 0 };
            Assert.IsTrue(Compares.AreDeepEqual(expected, InitialCycles.Join(a, b)));
        }

        [TestMethod()]
        public virtual void JoinWith()
        {
            int[] a = new int[] { 0, 1, 2 };
            int[] b = new int[] { 0, 3, 4 };
            var expected = new int[] { 0, 1, 2, 5, 4, 3, 0 };
            Assert.IsTrue(Compares.AreDeepEqual(expected, InitialCycles.Join(a, 5, b)));
        }

        [TestMethod()]
        public virtual void Singleton()
        {
            int[] a = new int[] { 0, 1, 3, 5, 7, 9 };
            int[] b = new int[] { 0, 2, 4, 6, 8, 10 };
            Assert.IsTrue(InitialCycles.GetSingletonIntersect(a, b));
        }

        [TestMethod()]
        public virtual void StartOverlap()
        {
            int[] a = new int[] { 0, 1, 2, 3, 4, 6 };
            int[] b = new int[] { 0, 1, 2, 3, 5, 7 };
            Assert.IsFalse(InitialCycles.GetSingletonIntersect(a, b));
        }

        [TestMethod()]
        public virtual void MiddleOverlap()
        {
            int[] a = new int[] { 0, 1, 3, 5, 6, 7, 9 };
            int[] b = new int[] { 0, 2, 4, 5, 6, 8, 10 };
            Assert.IsFalse(InitialCycles.GetSingletonIntersect(a, b));
        }

        [TestMethod()]
        public virtual void EndOverlap()
        {
            int[] a = new int[] { 0, 1, 3, 5, 7, 9, 10 };
            int[] b = new int[] { 0, 2, 4, 6, 8, 9, 10 };
            Assert.IsFalse(InitialCycles.GetSingletonIntersect(a, b));
        }

        [TestMethod()]
        public virtual void EdgeIsTransitive()
        {
            InitialCycles.Cycle.Edge p = new InitialCycles.Cycle.Edge(0, 1);
            InitialCycles.Cycle.Edge q = new InitialCycles.Cycle.Edge(1, 0);
            Assert.AreEqual(p.GetHashCode(), q.GetHashCode());
            Assert.AreEqual(p, q);
        }

        [TestMethod()]
        public virtual void EdgeToString()
        {
            InitialCycles.Cycle.Edge p = new InitialCycles.Cycle.Edge(0, 1);
            Assert.AreEqual("{0, 1}", p.ToString());
        }

        internal static int[][] K1
            => new int[][] { new int[] { } };

        /// <summary>
        /// Simple undirected graph where every pair of of the four vertices is
        /// connected. The graph is known as a complete graph and is referred to as
        /// K<sub>4</sub>.
        ///
        /// <returns>adjacency list of K<sub>4</sub></returns>
        /// </summary>
        internal static int[][] K4
            => new int[][] { new[] { 1, 2, 3 }, new[] { 0, 2, 3 }, new[] { 0, 1, 3 }, new[] { 0, 1, 2 } };

        /// <summary>benzene/cyclohexane graph</summary>
        internal static int[][] Cyclohexane
            => new int[][] { new[] { 1, 5 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 4, 0 } };

        /// <summary>@cdk.inchi InChI=1S/C10H8/c1-2-6-10-8-4-3-7-9(10)5-1/h1-8H</summary>
        internal static int[][] Naphthalene
            => new int[][] { new int[] { 1, 5 }, new int[] { 0, 2 }, new int[] { 1, 3 }, new int[] { 2, 4 }, new int[] { 3, 5, 7 }, new int[] { 0, 6, 4 }, new int[] { 5, 9 }, new int[] { 4, 8 }, new int[] { 7, 9 }, new int[] { 6, 8 } };

        /// <summary>@cdk.inchi InChI=1S/C14H10/c1-2-6-12-10-14-8-4-3-7-13(14)9-11(12)5-1/h1-10H</summary>
        internal static int[][] Anthracene
            => new int[][]{new[]{1, 5}, new[]{0, 2},new[] {1, 3},new[] {2, 4},new[] {3, 5, 7},new[] {0, 6, 4}, new[]{5, 9},new[] {4, 8},new[] {7, 10, 9},
                       new[] {6, 8, 13},new[] {8, 11}, new[]{10, 12},new[] {11, 13}, new[]{9, 12} };

        /// <summary>@cdk.inchi InChI=1S/C8H14/c1-2-8-5-3-7(1)4-6-8/h7-8H,1-6H2</summary>
        internal static int[][] Bicyclo
            => new int[][] { new[] { 1, 5 }, new[] { 0, 2 }, new[] { 1, 3, 7 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 0, 4, 6 }, new[] { 5, 7 }, new[] { 2, 6 } };

        /// <summary>@cdk.inchi InChI=1S/C7H12/c1-2-7-4-3-6(1)5-7/h6-7H,1-5H2</summary>
        internal static int[][] Norbornane
            => new int[][] { new[] { 1, 5 }, new[] { 0, 2 }, new[] { 1, 3, 6 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 0, 4, 6 }, new[] { 2, 5 }, };

        /// <summary>@cdk.inchi InChI=1S/C11H14/c1-2-4-10-6-8-11(5-3-1)9-7-10/h6-9H,1-5H2</summary>
        internal static int[][] CyclophaneOdd
            => new int[][] { new[] { 1, 5, 10 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4, 6 }, new[] { 3, 5 }, new[] { 0, 4 }, new[] { 3, 7 }, new[] { 6, 8 }, new[] { 7, 9 }, new[] { 8, 10 }, new[] { 0, 9 }, };

        /// <summary>
        /// Same as above but generate an even cycle so we can also test {@link
        /// org.openscience.cdk.graph.InitialCycles.Cycle#Family()} method.
        /// </summary>
        internal static int[][] CyclophaneEven
            => new int[][] { new int[] { 1, 5, 11 }, new int[] { 0, 2 }, new int[] { 1, 3 }, new int[] { 2, 4, 6 }, new int[] { 3, 5 }, new int[] { 0, 4 }, new int[] { 3, 7 }, new int[] { 6, 8 }, new int[] { 7, 9 }, new int[] { 8, 10 }, new int[] { 9, 11 }, new int[] { 0, 10 } };
    }
}
