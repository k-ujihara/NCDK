/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.Aromaticities;
using NCDK.SMSD.Algorithms.VFLib.Builder;
using NCDK.SMSD.Algorithms.VFLib.Map;
using NCDK.SMSD.Algorithms.VFLib.Query;
using NCDK.SMSD.Tools;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.SMSD.Algorithms.VFLib
{
    /// <summary>
    /// Unit testing for the <see cref="VFMapper"/>, <see cref="VFState"/>, <see cref="Match"/> class.
    /// </summary>
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public class VFLibTest 
        : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;
        private static IAtomContainer hexane;
        private static IQuery hexaneQuery;
        private static IAtomContainer benzene;
        private static IQuery benzeneQuery;

        static VFLibTest()
        {
            hexane = CreateHexane();
            Assert.AreEqual(6, hexane.Atoms.Count);
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(hexane);
            hexane = ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(hexane);
            Aromaticity.CDKLegacy.Apply(hexane);
            hexaneQuery = new QueryCompiler(hexane, true).Compile();
            Assert.AreEqual(6, hexaneQuery.CountNodes());
            benzene = CreateBenzene();
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(benzene);
            hexane = ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(benzene);
            Aromaticity.CDKLegacy.Apply(benzene);
            benzeneQuery = new QueryCompiler(benzene, true).Compile();
        }

        [TestMethod()]
        public void TestItShouldFindAllMatchCandidatesInTheRootState()
        {

            IState state = new VFState(benzeneQuery, new TargetProperties(benzene));
            int count = 0;

            while (state.HasNextCandidate())
            {
                state.NextCandidate();
                count++;
            }
            Assert.AreEqual(benzene.Atoms.Count * benzene.Atoms.Count, count);
        }

        [TestMethod()]
        public void TestItShoudFindAllMatchCandidatesInThePrimaryState()
        {
            IState state = new VFState(benzeneQuery, new TargetProperties(benzene));
            Match match = new Match(benzeneQuery.GetNode(0), benzene.Atoms[0]);
            IState newState = state.NextState(match);
            var candidates = new List<Match>();

            while (newState.HasNextCandidate())
            {
                candidates.Add(newState.NextCandidate());
            }

            Assert.AreEqual(4, candidates.Count);
        }

        [TestMethod()]
        public void TestItShouldFindAllMatchCandidatesInTheSecondaryState()
        {
            IState state0 = new VFState(benzeneQuery, new TargetProperties(benzene));
            Match match0 = new Match(benzeneQuery.GetNode(0), benzene.Atoms[0]);
            IState state1 = state0.NextState(match0);
            Match match1 = new Match(benzeneQuery.GetNode(1), benzene.Atoms[1]);
            IState state2 = state1.NextState(match1);
            List<Match> candidates = new List<Match>();

            while (state2.HasNextCandidate())
            {
                candidates.Add(state2.NextCandidate());
            }

            Assert.AreEqual(1, candidates.Count);
        }

        [TestMethod()]
        public void TestItShouldMapAllAtomsInTheSecondaryState()
        {
            IState state0 = new VFState(benzeneQuery, new TargetProperties(benzene));
            Match match0 = new Match(benzeneQuery.GetNode(0), benzene.Atoms[0]);
            IState state1 = state0.NextState(match0);
            Match match1 = new Match(benzeneQuery.GetNode(1), benzene.Atoms[1]);
            IState state2 = state1.NextState(match1);

            var map = state2.GetMap();

            Assert.AreEqual(2, map.Count);
            Assert.AreEqual(benzene.Atoms[0], map[benzeneQuery.GetNode(0)]);
            Assert.AreEqual(benzene.Atoms[1], map[benzeneQuery.GetNode(1)]);
        }

        [TestMethod()]
        public void TestItShouldFindAllMatchCandidatesFromTheTeriaryState()
        {
            IState state0 = new VFState(benzeneQuery, new TargetProperties(benzene));
            Match match0 = new Match(benzeneQuery.GetNode(0), benzene.Atoms[0]);
            IState state1 = state0.NextState(match0);
            Match match1 = new Match(benzeneQuery.GetNode(1), benzene.Atoms[1]);
            IState state2 = state1.NextState(match1);
            Match match2 = new Match(benzeneQuery.GetNode(2), benzene.Atoms[2]);
            IState state3 = state2.NextState(match2);
            List<Match> candidates = new List<Match>();

            while (state3.HasNextCandidate())
            {
                candidates.Add(state3.NextCandidate());
            }

            Assert.AreEqual(1, candidates.Count);
        }

        [TestMethod()]
        public void TestItShouldMapAllAtomsInTheTertiaryState()
        {
            IState state0 = new VFState(benzeneQuery, new TargetProperties(benzene));
            Match match0 = new Match(benzeneQuery.GetNode(0), benzene.Atoms[0]);
            IState state1 = state0.NextState(match0);
            Match match1 = new Match(benzeneQuery.GetNode(1), benzene.Atoms[1]);
            IState state2 = state1.NextState(match1);
            Match match2 = new Match(benzeneQuery.GetNode(2), benzene.Atoms[2]);
            IState state3 = state2.NextState(match2);
            var map = state3.GetMap();

            Assert.AreEqual(3, map.Count);
            Assert.AreEqual(benzene.Atoms[0], map[benzeneQuery.GetNode(0)]);
            Assert.AreEqual(benzene.Atoms[1], map[benzeneQuery.GetNode(1)]);
            Assert.AreEqual(benzene.Atoms[2], map[benzeneQuery.GetNode(2)]);
        }

        [TestMethod()]
        public void TestItShouldReachGoalWhenAllAtomsAreMapped()
        {
            IState state0 = new VFState(benzeneQuery, new TargetProperties(benzene));
            Match match0 = new Match(benzeneQuery.GetNode(0), benzene.Atoms[0]);
            IState state1 = state0.NextState(match0);
            Match match1 = new Match(benzeneQuery.GetNode(1), benzene.Atoms[1]);
            IState state2 = state1.NextState(match1);
            Match match2 = new Match(benzeneQuery.GetNode(2), benzene.Atoms[2]);
            IState state3 = state2.NextState(match2);
            Match match3 = new Match(benzeneQuery.GetNode(3), benzene.Atoms[3]);
            IState state4 = state3.NextState(match3);
            Match match4 = new Match(benzeneQuery.GetNode(4), benzene.Atoms[4]);
            IState state5 = state4.NextState(match4);

            Assert.IsFalse(state5.IsGoal);

            Match match5 = new Match(benzeneQuery.GetNode(5), benzene.Atoms[5]);
            IState state6 = state5.NextState(match5);

            Assert.IsTrue(state6.IsGoal);
        }

        [TestMethod()]
        public void TestItShouldHaveANextCandidateInTheSecondaryState()
        {
            IState state = new VFState(benzeneQuery, new TargetProperties(benzene));
            Match match = new Match(benzeneQuery.GetNode(0), benzene.Atoms[0]);
            IState nextState = state.NextState(match);
            Assert.IsTrue(nextState.HasNextCandidate());
        }

        [TestMethod()]
        public void TestItShouldMatchHexaneToHexaneWhenUsingMolecule()
        {
            IMapper mapper = new VFMapper(hexane, true);
            Assert.IsTrue(mapper.HasMap(hexane));
        }

        public static IAtomContainer CreateHexane()
        {
            IAtomContainer result = builder.NewAtomContainer();
            IAtom c1 = builder.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = builder.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = builder.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = builder.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = builder.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = builder.NewAtom("C");
            c6.Id = "6";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);

            IBond bond1 = builder.NewBond(c1, c2, BondOrder.Single);
            IBond bond2 = builder.NewBond(c2, c3, BondOrder.Single);
            IBond bond3 = builder.NewBond(c3, c4, BondOrder.Single);
            IBond bond4 = builder.NewBond(c4, c5, BondOrder.Single);
            IBond bond5 = builder.NewBond(c5, c6, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);

            return result;
        }

        public static IAtomContainer CreateBenzene()
        {
            IAtomContainer result = builder.NewAtomContainer();

            IAtom c1 = builder.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = builder.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = builder.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = builder.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = builder.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = builder.NewAtom("C");
            c6.Id = "6";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);

            IBond bond1 = builder.NewBond(c1, c2, BondOrder.Single);
            IBond bond2 = builder.NewBond(c2, c3, BondOrder.Double);
            IBond bond3 = builder.NewBond(c3, c4, BondOrder.Single);
            IBond bond4 = builder.NewBond(c4, c5, BondOrder.Double);
            IBond bond5 = builder.NewBond(c5, c6, BondOrder.Single);
            IBond bond6 = builder.NewBond(c6, c1, BondOrder.Double);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            return result;
        }
    }
}
