/* Copyright (C) 2012  Gilleain Torrance <gilleain.torrance@gmail.com>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK.Groups
{
    /**
     * @author maclean
     * @cdk.module test-group
     *
     */
    [TestClass()]
    public class PermutationGroupTest : CDKTestCase
    {

        // the first 7 factorials
        private readonly static int[] lookup = { 1, 1, 2, 6, 24, 120, 720, 5040 };

        private int Factorial(int n)
        {
            if (n < lookup.Length)
            {
                return lookup[n];
            }
            else
            {
                return f(n);
            }
        }

        private int f(int n)
        {
            if (n == 1)
            {
                return 1;
            }
            else if (n < lookup.Length)
            {
                return lookup[n];
            }
            else
            {
                return f(n - 1) * n;
            }
        }

        private PermutationGroup GetCubeGroup()
        {
            int size = 8;

            // the group of the cube
            Permutation p1 = new Permutation(1, 3, 5, 7, 0, 2, 4, 6);
            Permutation p2 = new Permutation(1, 3, 0, 2, 5, 7, 4, 6);
            List<Permutation> generators = new List<Permutation>();
            generators.Add(p1);
            generators.Add(p2);
            return new PermutationGroup(size, generators);
        }

        [TestMethod()]
        public void TestTheFactorialFunction()
        {
            Assert.AreEqual(40320, Factorial(8));
        }

        [TestMethod()]
        public void SizeConstructor()
        {
            int size = 4;
            PermutationGroup group = new PermutationGroup(size);
            Assert.AreEqual(size, group.Count);
        }

        [TestMethod()]
        public void baseConstructor()
        {
            int size = 4;
            Permutation base_ = new Permutation(size);
            PermutationGroup group = new PermutationGroup(base_);
            Assert.AreEqual(size, group.Count);
        }

        [TestMethod()]
        public void GeneratorConstructor()
        {
            int size = 4;
            Permutation p1 = new Permutation(1, 0, 2, 3);
            Permutation p2 = new Permutation(1, 2, 3, 0);
            List<Permutation> generators = new List<Permutation>();
            generators.Add(p1);
            generators.Add(p2);
            PermutationGroup group = new PermutationGroup(size, generators);
            Assert.AreEqual(size, group.Count);
            Assert.AreEqual(Factorial(size), group.Order());
        }

        [TestMethod()]
        public void MakeSymNTest()
        {
            int size = 4;
            PermutationGroup sym = PermutationGroup.MakeSymN(size);
            Assert.AreEqual(size, sym.Count);
            Assert.AreEqual(Factorial(size), sym.Order());
        }

        [TestMethod()]
        public void GetSizeTest()
        {
            int size = 4;
            PermutationGroup group = new PermutationGroup(size);
            Assert.AreEqual(size, group.Count);
        }

        [TestMethod()]
        public void OrderTest()
        {
            int size = 5;
            PermutationGroup sym = PermutationGroup.MakeSymN(size);
            Assert.AreEqual(Factorial(size), sym.Order());
        }

        [TestMethod()]
        public void GetTest()
        {
            int size = 6;
            // group that could represent a hexagon (numbered clockwise from top)
            // p1 = a flip across the vertical, p2 = flip across the horizontal
            Permutation p1 = new Permutation(0, 5, 4, 3, 2, 1);
            Permutation p2 = new Permutation(3, 2, 1, 0, 5, 4);
            List<Permutation> generators = new List<Permutation>();
            generators.Add(p1);
            generators.Add(p2);
            PermutationGroup group = new PermutationGroup(size, generators);

            // the permutations in U0 all have 0 in the orbit of i
            // but fixing 0 cannot fix 1, so there is no such perm
            int uIndex = 0;
            int uSubIndex = 1;
            Permutation u01 = group[uIndex, uSubIndex];
            Assert.IsNull(u01);

            // however, 0 and 3 are in the same orbit by both flips
            uSubIndex = 3;
            Permutation u03 = group[uIndex, uSubIndex];
            var orbit = u03.GetOrbit(0);
            Assert.IsTrue(orbit.Contains(uSubIndex));
        }

        [TestMethod()]
        public void GetLeftTransversalTest()
        {
            PermutationGroup group = GetCubeGroup();
            List<Permutation> transversal = group.GetLeftTransversal(1);
            Assert.AreEqual(3, transversal.Count);
        }

        [TestMethod()]
        public void TestTransversal()
        {
            int size = 4;
            // Sym(n) : make the total symmetry group
            PermutationGroup group = PermutationGroup.MakeSymN(size);

            // Aut(G) : make the automorphism group for a graph
            Permutation p1 = new Permutation(2, 1, 0, 3);
            Permutation p2 = new Permutation(0, 3, 2, 1);
            List<Permutation> generators = new List<Permutation>();
            generators.Add(p1);
            generators.Add(p2);
            PermutationGroup subgroup = new PermutationGroup(size, generators);

            // generate the traversal
            List<Permutation> transversal = group.Transversal(subgroup);

            int subgroupOrder = (int)subgroup.Order();
            int groupOrder = (int)group.Order();
            int transversalSize = transversal.Count;

            // check that |Aut(G)| / |Sym(N)| = |Transversal|
            Assert.AreEqual(Factorial(size), groupOrder);
            Assert.AreEqual(groupOrder / subgroupOrder, transversalSize);
        }

        [TestMethod()]
        public void ApplyTest()
        {
            var all = new List<Permutation>();
            int size = 4;
            PermutationGroup group = PermutationGroup.MakeSymN(size);
            group.Apply(new NBacktracker(all));
            Assert.AreEqual(Factorial(size), all.Count);
        }

        class NBacktracker
            : PermutationGroup.Backtracker
        {
            List<Permutation> all;

            public NBacktracker(List<Permutation> all)
            {
                this.all = all;
            }

            public bool IsFinished()
            {
                return false;
            }

            public void ApplyTo(Permutation p)
            {
                all.Add(p);
            }
        }

        [TestMethod()]
        public void Apply_FinishEarlyTest()
        {
            List<Permutation> all = new List<Permutation>();
            int max = 5; // stop after this many seen
            int size = 4;
            PermutationGroup group = PermutationGroup.MakeSymN(size);
            group.Apply(new FinishEarlyBacktracker(all, max));
            Assert.AreEqual(max, all.Count);
        }

        class FinishEarlyBacktracker
            : PermutationGroup.Backtracker
        {
            List<Permutation> all;
            int max;

            public FinishEarlyBacktracker(List<Permutation> all, int max)
            {
                this.all = all;
                this.max = max;
            }

            public bool IsFinished()
            {
                return all.Count >= max;
            }

            public void ApplyTo(Permutation p)
            {
                all.Add(p);
            }
        }

        [TestMethod()]
        public void AllTest()
        {
            int size = 4;
            PermutationGroup group = PermutationGroup.MakeSymN(size);
            List<Permutation> all = group.All();
            Assert.AreEqual(Factorial(size), all.Count);
        }

        [TestMethod()]
        public void Test_SuccessTest()
        {
            PermutationGroup group = GetCubeGroup();
            Permutation p = new Permutation(6, 7, 4, 5, 2, 3, 0, 1);
            int position = group.Test(p);
            // this means p is a member of G
            Assert.IsTrue(position == group.Count);
        }

        [TestMethod()]
        public void Test_FailureTest()
        {
            PermutationGroup group = GetCubeGroup();
            Permutation p = new Permutation(1, 2, 3, 4, 0, 6, 7, 5);
            int position = group.Test(p);
            // this means p is not in G
            Assert.IsTrue(position < group.Count);
        }

        [TestMethod()]
        public void EnterTest()
        {
            int size = 4;
            PermutationGroup group = new PermutationGroup(size);
            group.Enter(new Permutation(1, 0, 3, 2));
            Assert.AreEqual(2, group.Order());
        }

        [TestMethod()]
        public void ChangeBaseTest()
        {
            int size = 4;
            PermutationGroup group = new PermutationGroup(size);
            group.Enter(new Permutation(1, 0, 3, 2));
            group.ChangeBase(new Permutation(size));
            Assert.AreEqual(2, group.Order());
        }
    }
}
