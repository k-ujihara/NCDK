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

namespace NCDK.Smiles.SMARTS
{
    // @author John May
    [TestClass()]
    public class SmartsPatternTest
    {
        IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void RingSizeOrNumber_membership()
        {
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[R]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_ringConnectivity()
        {
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[X2]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_elements()
        {
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Br]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Cr]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Fr]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Sr]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Ra]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Re]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Rf]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_negatedMembership()
        {
            Assert.IsTrue(SmartsPattern.RingSizeOrNumber("[!R]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_membershipZero()
        {
            Assert.IsTrue(SmartsPattern.RingSizeOrNumber("[R0]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_membershipTwo()
        {
            Assert.IsTrue(SmartsPattern.RingSizeOrNumber("[R2]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_ringSize()
        {
            Assert.IsTrue(SmartsPattern.RingSizeOrNumber("[r5]"));
        }

        [TestMethod()]
        public void Components()
        {
            Assert.IsTrue(SmartsPattern.Create("(O).(O)", bldr).Matches(Smi("O.O")));
            Assert.IsFalse(SmartsPattern.Create("(O).(O)", bldr).Matches(Smi("OO")));
        }

        [TestMethod()]
        public void Stereochemistry()
        {
            Assert.IsTrue(SmartsPattern.Create("C[C@H](O)CC", bldr).Matches(Smi("C[C@H](O)CC")));
            Assert.IsFalse(SmartsPattern.Create("C[C@H](O)CC", bldr).Matches(Smi("C[C@@H](O)CC")));
            Assert.IsFalse(SmartsPattern.Create("C[C@H](O)CC", bldr).Matches(Smi("CC(O)CC")));
        }

        [TestMethod()]
        public void SmartsMatchingReaction()
        {
            Assert.IsTrue(SmartsPattern.Create("CC", bldr).Matches(Rsmi("CC>>")));
            Assert.IsTrue(SmartsPattern.Create("CC", bldr).Matches(Rsmi(">>CC")));
            Assert.IsTrue(SmartsPattern.Create("CC", bldr).Matches(Rsmi(">CC>")));
            Assert.IsFalse(SmartsPattern.Create("CO", bldr).Matches(Rsmi(">>CC")));
        }

        [TestMethod()]
        public void ReactionSmartsMatchingReaction()
        {
            Assert.IsTrue(SmartsPattern.Create("CC>>", bldr).Matches(Rsmi("CC>>")));
            Assert.IsFalse(SmartsPattern.Create("CC>>", bldr).Matches(Rsmi(">>CC")));
            Assert.IsFalse(SmartsPattern.Create("CC>>", bldr).Matches(Rsmi(">CC>")));
        }

        [TestMethod()]
        public void ReactionGrouping()
        {
            Assert.IsTrue(SmartsPattern.Create("[Na+].[OH-]>>", bldr).Matches(Rsmi("[Na+].[OH-]>>")));
            Assert.IsTrue(SmartsPattern.Create("[Na+].[OH-]>>", bldr).Matches(Rsmi("[Na+].[OH-]>> |f:0.1|")));
            Assert.IsTrue(SmartsPattern.Create("([Na+].[OH-])>>", bldr).Matches(Rsmi("[Na+].[OH-]>> |f:0.1|")));
            // this one can't match because we don't know if NaOH is one component from the input smiles
            Assert.IsFalse(SmartsPattern.Create("([Na+].[OH-])>>", bldr).Matches(Rsmi("[Na+].[OH-]>>")));
        }

        [TestMethod()]
        public void NoMaps()
        {
            Assert.AreEqual(4, SmartsPattern.Create("C>>C", null).MatchAll(Rsmi("CC>>CC")).Count());
        }

        [TestMethod()]
        public void NoMapsInQueryMapsInTargetIgnored()
        {
            Assert.AreEqual(4, SmartsPattern.Create("C>>C", null).MatchAll(Rsmi("[C:7][C:8]>>[C:7][C:8]")).Count());
        }

        [TestMethod()]
        public void UnpairedMapIsQueryIsIgnored()
        {
            Assert.AreEqual(4, SmartsPattern.Create("[C:1]>>C", null).MatchAll(Rsmi("[CH3:7][CH3:8]>>[CH3:7][CH3:8]")).Count());
            Assert.AreEqual(4, SmartsPattern.Create("C>>[C:1]", null).MatchAll(Rsmi("[CH3:7][CH3:8]>>[CH3:7][CH3:8]")).Count());
        }

        [TestMethod()]
        public void NoMapsInTarget()
        {
            Assert.AreEqual(0, SmartsPattern.Create("[C:1]>>[C:1]", null).MatchAll(Rsmi("C>>C")).Count());
        }

        //@Ignore("Not supported yet")
        public void OptionalMapping()
        {
            Assert.AreEqual(2, SmartsPattern.Create("[C:?1]>>[C:?1]", null).MatchAll(Rsmi("[CH3:7][CH3:8]>>[CH3:7][CH3:8]")).Count());
            Assert.AreEqual(4, SmartsPattern.Create("[C:?1]>>[C:?1]", null).MatchAll(Rsmi("CC>>CC")).Count());
        }

        [TestMethod()]
        public void MappedMatch()
        {
            Assert.AreEqual(2, SmartsPattern.Create("[C:1]>>[C:1]", null).MatchAll(Rsmi("[CH3:7][CH3:8]>>[CH3:7][CH3:8]")).Count());
        }

        [TestMethod()]
        public void MismatchedQueryMapsIgnored()
        {
            Assert.AreEqual(4, SmartsPattern.Create("[C:1]>>[C:2]", null).MatchAll(Rsmi("[CH3:7][CH3:8]>>[CH3:7][CH3:8]")).Count());
        }

        // map :1 in query binds only to :7 in target
        [TestMethod()]
        public void AtomMapsWithOrLogic1()
        {
            Assert.AreEqual(4, SmartsPattern.Create("[C:1][C:1]>>[C:1]", null).MatchAll(Rsmi("[CH3:7][CH3:7]>>[CH3:7][CH3:7]")).Count());
        }

        // map :1 in query binds to :7 or :8 in target
        [TestMethod()]
        public void AtomMapsWithOrLogic2()
        {
            Assert.AreEqual(4, SmartsPattern.Create("[C:1][C:1]>>[C:1]", null).MatchAll(Rsmi("[CH3:7][CH3:8]>>[CH3:7][CH3:8]")).Count());
        }

        // map :1 in query binds only to :7 in target
        [TestMethod()]
        public void AtomMapsWithOrLogic3()
        {
            Assert.AreEqual(2, SmartsPattern.Create("[C:1][C:1]>>[C:1]", null).MatchAll(Rsmi("[CH3:7][CH3:7]>>[CH3:7][CH3:8]")).Count());
        }

        [TestMethod()]
        public void CCBondForming()
        {
            Assert.AreEqual(2, SmartsPattern.Create("([C:1]).([C:2])>>[C:1][C:2]", null)
                                    .MatchAll(Rsmi("[C-:13]#[N:14].[K+].[CH:3]1=[CH:4][C:5](=[CH:11][CH:12]=[C:2]1[CH2:1]Br)[C:6](=[O:10])[CH:7]2[CH2:8][CH2:9]2>>[CH:3]1=[CH:4][C:5](=[CH:11][CH:12]=[C:2]1[CH2:1][C:13]#[N:14])[C:6](=[O:10])[CH:7]2[CH2:8][CH2:9]2 |f:0.1|")).Count());
        }

        [TestMethod()]
        public void Stereo_ring_closures()
        {
            var ptrn = SmartsPattern.Create("[C@@]1(O[C@@]([C@@]([C@]([C@]1(C)O)(C)O)(O)C)(O)C)(O)C");
            Assert.IsTrue(ptrn.Matches(Smi("[C@@]1(O[C@@]([C@@]([C@]([C@]1(C)O)(C)O)(O)C)(O)C)(O)C")));
        }

        IAtomContainer Smi(string smi)
        {
            return new SmilesParser(bldr).ParseSmiles(smi);
        }

        IReaction Rsmi(string smi)
        {
            return new SmilesParser(bldr).ParseReactionSmiles(smi);
        }
    }
}
