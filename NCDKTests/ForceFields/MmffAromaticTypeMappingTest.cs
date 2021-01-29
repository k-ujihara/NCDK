/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
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
using NCDK.Common.Base;
using NCDK.Common.Collections;
using System.Collections.Generic;

namespace NCDK.ForceFields
{
    [TestClass()]
    public class MmffAromaticTypeMappingTest
    {
        [TestMethod()]
        public void IndexOfHetroAt0()
        {
            int[] cycle = new int[] { 3, 2, 1, 4, 5, 3 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            contr[cycle[0]] = 2;
            Assert.AreEqual(0, MmffAromaticTypeMapping.IndexOfHetro(cycle, contr));
        }

        [TestMethod()]
        public void IndexOfHetroAt1()
        {
            int[] cycle = new int[] { 3, 2, 1, 4, 5, 3 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            contr[cycle[1]] = 2;
            Assert.AreEqual(1, MmffAromaticTypeMapping.IndexOfHetro(cycle, contr));
        }

        [TestMethod()]
        public void IndexOfHetroAt2()
        {
            int[] cycle = new int[] { 3, 2, 1, 4, 5, 3 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            contr[cycle[2]] = 2;
            Assert.AreEqual(2, MmffAromaticTypeMapping.IndexOfHetro(cycle, contr));
        }

        [TestMethod()]
        public void IndexOfHetroAt3()
        {
            int[] cycle = new int[] { 3, 2, 1, 4, 5, 3 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            contr[cycle[3]] = 2;
            Assert.AreEqual(3, MmffAromaticTypeMapping.IndexOfHetro(cycle, contr));
        }

        [TestMethod()]
        public void IndexOfHetroAt4()
        {
            int[] cycle = new int[] { 3, 2, 1, 4, 5, 3 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            contr[cycle[4]] = 2;
            Assert.AreEqual(4, MmffAromaticTypeMapping.IndexOfHetro(cycle, contr));
        }

        [TestMethod()]
        public void IndexOfNoHetroAtom()
        {
            int[] cycle = new int[] { 3, 2, 1, 4, 5, 3 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            Assert.AreEqual(-1, MmffAromaticTypeMapping.IndexOfHetro(cycle, contr));
        }

        [TestMethod()]
        public void IndexOfTwoHetroAtoms()
        {
            int[] cycle = new int[] { 3, 2, 1, 4, 5, 3 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            contr[cycle[0]] = 2;
            contr[cycle[4]] = 2;
            Assert.AreEqual(-2, MmffAromaticTypeMapping.IndexOfHetro(cycle, contr));
        }

        [TestMethod()]
        public void NormaliseNoHetro()
        {
            int[] cycle = new int[] { 3, 2, 1, 4, 5, 3 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            Assert.IsFalse(MmffAromaticTypeMapping.NormaliseCycle(cycle, contr));
        }

        [TestMethod()]
        public void NormaliseHetroAt3()
        {
            int[] cycle = new int[] { 3, 2, 1, 4, 5, 3 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            contr[cycle[3]] = 2;
            Assert.IsTrue(MmffAromaticTypeMapping.NormaliseCycle(cycle, contr));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 4, 5, 3, 2, 1, 4 }, cycle));
        }

        [TestMethod()]
        public void NormaliseHetroAt2()
        {
            int[] cycle = new int[] { 3, 2, 1, 4, 5, 3 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            contr[cycle[2]] = 2;
            Assert.IsTrue(MmffAromaticTypeMapping.NormaliseCycle(cycle, contr));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 4, 5, 3, 2, 1 }, cycle));
        }

        [TestMethod()]
        public void TetravalentCarbonContributesOneElectron()
        {
            Assert.AreEqual(1, MmffAromaticTypeMapping.Contribution(6, 3, 4));
        }

        [TestMethod()]
        public void TetravalentTricoordinateNitrogenContributesOneElectron()
        {
            Assert.AreEqual(1, MmffAromaticTypeMapping.Contribution(7, 3, 4));
        }

        [TestMethod()]
        public void TrivalentBicoordinateNitrogenContributesOneElectron()
        {
            Assert.AreEqual(1, MmffAromaticTypeMapping.Contribution(7, 2, 3));
        }

        [TestMethod()]
        public void TrivalentTricoordinateNitrogenContributesTwoElectrons()
        {
            Assert.AreEqual(2, MmffAromaticTypeMapping.Contribution(7, 3, 3));
        }

        [TestMethod()]
        public void BivalentBicoordinateNitrogenContributesTwoElectrons()
        {
            Assert.AreEqual(2, MmffAromaticTypeMapping.Contribution(7, 2, 2));
        }

        [TestMethod()]
        public void DivalentSulphurContributesTwoElectrons()
        {
            Assert.AreEqual(2, MmffAromaticTypeMapping.Contribution(16, 2, 2));
        }

        [TestMethod()]
        public void DivalentOxygenContributesTwoElectrons()
        {
            Assert.AreEqual(2, MmffAromaticTypeMapping.Contribution(8, 2, 2));
        }

        [TestMethod()]
        public void BenzeneIsAromatic()
        {
            int[] cycle = new int[] { 0, 1, 2, 3, 4, 5, 0 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            int[] dbs = new int[] { 1, 0, 3, 2, 5, 4 };
            bool[] arom = new bool[contr.Length];
            Assert.IsTrue(MmffAromaticTypeMapping.IsAromaticRing(cycle, contr, dbs, arom));
        }

        [TestMethod()]
        public void PyrroleIsAromatic()
        {
            int[] cycle = new int[] { 0, 1, 2, 3, 4, 0 };
            int[] contr = new int[] { 2, 1, 1, 1, 1 };
            int[] dbs = new int[] { -1, 2, 1, 4, 3 };
            bool[] arom = new bool[contr.Length];
            Assert.IsTrue(MmffAromaticTypeMapping.IsAromaticRing(cycle, contr, dbs, arom));
        }

        [TestMethod()]
        public void ExocyclicDoubleBondsBreakAromaticity()
        {
            int[] cycle = new int[] { 0, 1, 2, 3, 4, 5, 0 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1, 1, 1 };
            int[] dbs = new int[] { 1, 0, 6, 7, 5, 4 };
            bool[] arom = new bool[contr.Length];
            Assert.IsFalse(MmffAromaticTypeMapping.IsAromaticRing(cycle, contr, dbs, arom));
        }

        [TestMethod()]
        public void DelocaiIsedExocyclicDoubleBondsMaintainAromaticity()
        {
            int[] cycle = new int[] { 0, 1, 2, 3, 4, 5, 0 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1, 1, 1 };
            int[] dbs = new int[] { 1, 0, 6, 7, 5, 4 };
            bool[] arom = new bool[contr.Length];
            arom[2] = arom[3] = arom[6] = arom[7] = true; // adjacent ring is aromatic
            Assert.IsTrue(MmffAromaticTypeMapping.IsAromaticRing(cycle, contr, dbs, arom));
        }

        [TestMethod()]
        public void UpdateN2OXtoNPOX()
        {
            int[] cycle = new int[] { 2, 4, 3, 1, 0, 5, 2 };
            string[] symbs = new string[10];
            Arrays.Fill(symbs, "");
            symbs[cycle[1]] = "N2OX";
            MmffAromaticTypeMapping.UpdateAromaticTypesInSixMemberRing(cycle, symbs);
            Assert.AreEqual("NPOX", symbs[cycle[1]]);
        }

        // NCN+,N+=C,N=+C -> NPD+
        [TestMethod()]
        public void UpdateToNPDPlus()
        {
            int[] cycle = new int[] { 2, 4, 3, 1, 0, 5, 2 };
            string[] symbs = new string[10];
            Arrays.Fill(symbs, "");
            symbs[cycle[1]] = "NCN+";
            symbs[cycle[2]] = "N+=C";
            symbs[cycle[3]] = "N=+C";
            MmffAromaticTypeMapping.UpdateAromaticTypesInSixMemberRing(cycle, symbs);
            Assert.AreEqual("NPD+", symbs[cycle[1]]);
            Assert.AreEqual("NPD+", symbs[cycle[2]]);
            Assert.AreEqual("NPD+", symbs[cycle[3]]);
        }

        // N* -> NPYD
        [TestMethod()]
        public void UpdateNStarToNPYD()
        {
            int[] cycle = new int[] { 2, 4, 3, 1, 0, 5, 2 };
            string[] symbs = new string[10];
            Arrays.Fill(symbs, "");
            symbs[cycle[1]] = "N=C";
            symbs[cycle[2]] = "N=N";
            MmffAromaticTypeMapping.UpdateAromaticTypesInSixMemberRing(cycle, symbs);
            Assert.AreEqual("NPYD", symbs[cycle[1]]);
            Assert.AreEqual("NPYD", symbs[cycle[2]]);
        }

        // C* -> CB
        [TestMethod()]
        public void UpdateCStarToCB()
        {
            int[] cycle = new int[] { 2, 4, 3, 1, 0, 5, 2 };
            string[] symbs = new string[10];
            Arrays.Fill(symbs, "");
            symbs[cycle[1]] = "C=C";
            symbs[cycle[2]] = "C=N";
            MmffAromaticTypeMapping.UpdateAromaticTypesInSixMemberRing(cycle, symbs);
            Assert.AreEqual("CB", symbs[cycle[1]]);
            Assert.AreEqual("CB", symbs[cycle[2]]);
        }

        [TestMethod()]
        public void ImidazoleCarbonTypesAreNeitherAlphaOrBeta()
        {
            var map = new Dictionary<string, string>() { { "CB", "C5A" } };
            Assert.AreEqual("C5", MmffAromaticTypeMapping.GetAromaticType(map, 'A', "CB", true, false));
        }

        [TestMethod()]
        public void ImidazoleNitrogenTypesAreNeitherAlphaOrBeta()
        {
            var map = new Dictionary<string, string>() { { "N=C", "N5A" } };
            Assert.AreEqual("N5", MmffAromaticTypeMapping.GetAromaticType(map, 'A', "N=C", true, false));
        }

        [TestMethod()]
        public void AnionCarbonTypesAreNeitherAlphaOrBeta()
        {
            var map = new Dictionary<string, string>() { { "CB", "C5A" } };
            Assert.AreEqual("C5", MmffAromaticTypeMapping.GetAromaticType(map, 'A', "CB", false, true));
        }

        [TestMethod()]
        public void AnionNitrogensAreAlwaysN5M()
        {
            var map = new Dictionary<string, string>() { { "N=C", "N5A" } };
            Assert.AreEqual("N5M", MmffAromaticTypeMapping.GetAromaticType(map, 'A', "N=C", false, true));
        }

        // IM = false + AN = false
        [TestMethod()]
        public void UseMappingWhenNeitherFlagIsRaised()
        {
            var map = new Dictionary<string, string>() { { "N=C", "N5A" } };
            Assert.AreEqual("N5A", MmffAromaticTypeMapping.GetAromaticType(map, 'A', "N=C", false, false));
        }

        [TestMethod()]
        public void ElementContributingOneElectronRejectWhenNoDoubleBond()
        {
            int[] cycle = new int[] { 0, 1, 2, 3, 4, 5, 0 };
            int[] contr = new int[] { 1, 1, 1, 1, 1, 1 };
            int[] dbs = new int[] { 1, 0, 3, -1, 5, 4 };
            Assert.IsFalse(MmffAromaticTypeMapping.IsAromaticRing(cycle, contr, dbs, new bool[contr.Length]));
        }

        [TestMethod()]
        public void IntractableNumberOfCycles()
        {
            // to ensure intractable cycles are handled we create a complete graph
            // where every vertex is attached to every other vertex. K9 is sufficient
            // to trigger an abort when finding cycles for setting PubChem_994
            IAtomContainer container = new Mock<IAtomContainer>().Object;
            int[][] graphK9 = Arrays.CreateJagged<int>(9, 8);

            for (int i = 0; i < graphK9.Length; i++)
            {
                int n = 0;
                for (int j = 0; j < graphK9.Length; j++)
                {
                    if (i == j) continue;
                    graphK9[i][n++] = j;
                }
            }

            Assert.AreEqual(0, MmffAromaticTypeMapping.CyclesOfSizeFiveOrSix(container, graphK9).Length);
        }

        [TestMethod()]
        public void ContributionOfThreeValentCarbon()
        {
            Assert.AreEqual(-1, MmffAromaticTypeMapping.Contribution(6, 3, 3));
        }

        [TestMethod()]
        public void ContributionOfFiveValentNitrogen()
        {
            Assert.AreEqual(-1, MmffAromaticTypeMapping.Contribution(7, 3, 5));
        }
    }
}
