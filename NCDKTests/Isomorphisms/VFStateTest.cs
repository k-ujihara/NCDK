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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Graphs;
using NCDK.Templates;

namespace NCDK.Isomorphisms
{
    /// <summary>
    /// These are isolated tests - really difficult to isolate the behaviour here but
    /// will add.
    /// </summary>
    // @author John May
    // @cdk.module test-isomorphism
    [TestClass()]
    public class VFStateTest
    {
        // 0-look-ahead
        [TestMethod()]
        public void InfeasibleAtoms()
        {
            var m = new Mock<AtomMatcher>(); AtomMatcher mock = m.Object;
            m.Setup(n => n.Matches(It.IsAny<IAtom>(), It.IsAny<IAtom>())).Returns(false);
            VFState state = CreateBenzeneToNaphthalene(mock, new Mock<BondMatcher>().Object);
            for (int i = 0; i < state.NMax(); i++)
            {
                for (int j = 0; j < state.MMax(); j++)
                {
                    Assert.IsFalse(state.Feasible(i, j));
                }
            }
        }

        // 0-look-ahead
        [TestMethod()]
        public void InfeasibleBonds()
        {
            var m = new Mock<BondMatcher>(); BondMatcher mock = m.Object;
            m.Setup(n => n.Matches(It.IsAny<IBond>(), It.IsAny<IBond>())).Returns(false);
            VFState state = CreateBenzeneToNaphthalene(AtomMatcher.CreateAnyMatcher(), mock);
            state.m1[0] = 0;
            state.m1[1] = 1;
            state.m1[2] = 2;
            state.m1[3] = 3;
            state.m1[4] = 4;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Assert.IsFalse(state.Feasible(i, j));
                }
            }
        }

        // 1-look-ahead
        [TestMethod()]
        public void InfeasibleTerminalCount()
        {
            VFState state = CreateBenzeneToNaphthalene(AtomMatcher.CreateAnyMatcher(), BondMatcher.CreateAnyMatcher());
            Assert.IsTrue(state.Feasible(4, 4)); // 4, 4 is feasible
            state.Add(0, 0);
            state.Add(1, 1);
            state.Add(2, 2);
            state.Add(3, 3);
            Assert.IsFalse(state.Feasible(4, 4)); // 4, 4 is infeasible
        }

        /// <summary>
        /// Create a sub state for matching benzene to naphthalene Benzene:
        /// InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H Naphthalene: InChI=1/C10H8/c1-2-6-10-8-4-3-7-9(10)5-1/h1-8H
        /// </summary>
        VFState CreateBenzeneToNaphthalene(AtomMatcher atomMatcher, BondMatcher bondMatcher)
        {
            IAtomContainer container1 = TestMoleculeFactory.MakeBenzene();
            IAtomContainer container2 = TestMoleculeFactory.MakeNaphthalene();
            GraphUtil.EdgeToBondMap bonds1 = GraphUtil.EdgeToBondMap.WithSpaceFor(container1);
            GraphUtil.EdgeToBondMap bonds2 = GraphUtil.EdgeToBondMap.WithSpaceFor(container2);
            int[][] g1 = GraphUtil.ToAdjList(container1, bonds1);
            int[][] g2 = GraphUtil.ToAdjList(container2, bonds2);
            return new VFState(container1, container2, g1, g2, bonds1, bonds2, atomMatcher, bondMatcher);
        }
    }
}
