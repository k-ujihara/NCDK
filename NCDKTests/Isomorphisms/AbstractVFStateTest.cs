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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NCDK.Isomorphisms.AbstractVFState;
using NCDK.Common.Base;
using NCDK.Common.Collections;

namespace NCDK.Isomorphisms
{
    /// <summary>
    // @author John May
    // @cdk.module test-isomorphism
    /// </summary>
    [TestClass()]
    public class AbstractVFStateTest
    {

        // size = 0, always the first vertex
        [TestMethod()]
        public void NextNAt0()
        {
            AbstractVFState state = Create(5, 10);
            Assert.AreEqual(0, state.NextN(-1));
        }

        // size > 0, select the first unmapped terminal vertex
        [TestMethod()]
        public void NextNTerminal()
        {
            AbstractVFState state = Create(5, 10);
            state.size = 2;
            state.m1[0] = 1;
            state.m1[1] = 0;
            state.t1[4] = 1; // <- first terminal
            Assert.AreEqual(4, state.NextN(-1));
        }

        // no terminal mappings, select the first unmapped
        [TestMethod()]
        public void NextNNonTerminal()
        {
            AbstractVFState state = Create(5, 10);
            state.size = 2;
            state.m1[0] = 1;
            state.m1[1] = 0;
            Assert.AreEqual(2, state.NextN(-1));
        }

        // size = 0, always the next vertex
        [TestMethod()]
        public void NextMAt0()
        {
            AbstractVFState state = Create(5, 10);
            Assert.AreEqual(0, state.NextM(0, -1));
            Assert.AreEqual(1, state.NextM(0, 0));
            Assert.AreEqual(2, state.NextM(0, 1));
            Assert.AreEqual(3, state.NextM(0, 2));
        }

        // size > 0, select the first unmapped terminal vertex
        [TestMethod()]
        public void NextMTerminal()
        {
            AbstractVFState state = Create(5, 10);
            state.size = 2;
            state.m2[0] = 1;
            state.m2[1] = 0;
            state.t1[1] = 1; // query vertex is in terminal set
            state.t2[4] = 1; // <- first terminal (not kept returned for now - allow disconnected)
            Assert.AreEqual(4, state.NextM(1, -1));
        }

        // no terminal mappings, select the first unmapped
        [TestMethod()]
        public void NextMNonTerminal()
        {
            AbstractVFState state = Create(5, 10);
            state.size = 2;
            state.m2[0] = 1;
            state.m2[1] = 0;
            Assert.AreEqual(2, state.NextM(0, -1));
        }

        [TestMethod()]
        public void AddNonFeasible()
        {
            AbstractVFState state = new VFState(new int[4][], new int[6][], false);
            Assert.IsFalse(state.Add(0, 1));
            Assert.AreEqual(0, state.size);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { UNMAPPED, UNMAPPED, UNMAPPED, UNMAPPED },
                state.m1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { UNMAPPED, UNMAPPED, UNMAPPED, UNMAPPED, UNMAPPED, UNMAPPED },
                state.m2));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 0, 0, 0 },
                state.t1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 0, 0, 0, 0, 0 },
                state.t2));
        }

        [TestMethod()]
        public void Add()
        {
            int[][] g1 = new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2 } };
            int[][] g2 = new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 4 } };
            AbstractVFState state = Create(g1, g2);
            Assert.IsTrue(state.Add(0, 1));
            Assert.AreEqual(1, state.size);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 1, UNMAPPED, UNMAPPED, UNMAPPED },
                state.m1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { UNMAPPED, 0, UNMAPPED, UNMAPPED, UNMAPPED, UNMAPPED },
                state.m2));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 1, 0, 0 },
                state.t1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 1, 0, 1, 0, 0, 0 },
                state.t2));
            Assert.IsTrue(state.Add(1, 2));
            Assert.AreEqual(2, state.size);
            Assert.IsTrue(Compares.AreDeepEqual(
               new int[] { 1, 2, UNMAPPED, UNMAPPED },
               state.m1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { UNMAPPED, 0, 1, UNMAPPED, UNMAPPED, UNMAPPED },
                state.m2));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 2, 1, 2, 0 },
                state.t1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 1, 2, 1, 2, 0, 0 },
                state.t2));
        }

        [TestMethod()]
        public void Remove()
        {
            int[][] g1 = new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2 } };
            int[][] g2 = new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 4 } };
            AbstractVFState state = Create(g1, g2);
            state.size = 2;
            // see Add()
            state.m1[0] = 1;
            state.m1[1] = 2;
            state.m2[1] = 0;
            state.m2[2] = 1;
            state.t1[0] = 2;
            state.t1[1] = 1;
            state.t1[2] = 2;
            state.t2[0] = 1;
            state.t2[1] = 2;
            state.t2[2] = 1;
            state.t2[3] = 2;
            Assert.AreEqual(2, state.size);
            Assert.IsTrue(Compares.AreDeepEqual(
                          new int[] { 1, 2, UNMAPPED, UNMAPPED },
                          state.m1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { UNMAPPED, 0, 1, UNMAPPED, UNMAPPED, UNMAPPED },
                state.m2));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 2, 1, 2, 0 },
                state.t1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 1, 2, 1, 2, 0, 0 },
                state.t2));
            state.Remove(1, 2);
            Assert.AreEqual(1, state.size);
            Assert.IsTrue(Compares.AreDeepEqual(
               new int[] { 1, UNMAPPED, UNMAPPED, UNMAPPED },
               state.m1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { UNMAPPED, 0, UNMAPPED, UNMAPPED, UNMAPPED, UNMAPPED },
                state.m2));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 1, 0, 0 },
                state.t1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 1, 0, 1, 0, 0, 0 },
                state.t2));
            state.Remove(0, 1);
            Assert.AreEqual(0, state.size);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { UNMAPPED, UNMAPPED, UNMAPPED, UNMAPPED },
                state.m1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { UNMAPPED, UNMAPPED, UNMAPPED, UNMAPPED, UNMAPPED, UNMAPPED },
                state.m2));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 0, 0, 0 },
                state.t1));
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[] { 0, 0, 0, 0, 0, 0 },
                state.t2));
        }

        [TestMethod()]
        public void CopyMapping()
        {
            int[][] g1 = new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2 } };
            int[][] g2 = new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 4 } };
            AbstractVFState state = Create(g1, g2);
            state.m1[0] = 1;
            state.m1[1] = 2;
            state.m1[2] = 5;
            state.m1[3] = 6;
            Assert.IsTrue(Compares.AreDeepEqual(state.m1, state.Mapping()));
            Assert.AreNotSame(state.m1, state.Mapping());
        }

        [TestMethod()]
        public void Accessors()
        {
            int[][] g1 = new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2 } };
            int[][] g2 = new int[][] { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2, 4 }, new[] { 3, 5 }, new[] { 4 } };
            AbstractVFState state = Create(g1, g2);
            Assert.AreEqual(g1.Length, state.NMax());
            Assert.AreEqual(g2.Length, state.MMax());
            Assert.AreEqual(0, state.Count);
            state.size = 2;
            Assert.AreEqual(2, state.Count);
        }

        internal AbstractVFState Create(int g1Size, int g2Size)
        {
            return Create(Arrays.CreateJagged<int>(g1Size, 0), Arrays.CreateJagged<int>(g2Size, 0));
        }

        internal AbstractVFState Create(int[][] g1, int[][] g2)
        {
            return new VFState(g1, g2, true);
        }

        class VFState : AbstractVFState
        {
            bool feasible;
            public VFState(int[][] g1, int[][] g2, bool feasible)
                : base(g1, g2)
            {
                this.feasible = feasible;
            }

            public override bool Feasible(int n, int m) => feasible;
        }
    }
}
