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
using NCDK.Smiles;

namespace NCDK.SMARTS
{
    // @author John May
    [TestClass()]
    public class SmartsPatternTest
    {
        readonly IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
        
        [TestMethod()]
        public void Isotopes()
        {
            // FIXME SMARTS Grammar needs fixing/replacing [12] is not considered valid

            Assert.IsFalse(SmartsPattern.Create("[12*]", bldr).Matches(Smi("C")));
            Assert.IsFalse(SmartsPattern.Create("[12*]", bldr).Matches(Smi("[CH4]")));
            Assert.IsTrue(SmartsPattern.Create("[12*]", bldr).Matches(Smi("[12CH4]")));
            Assert.IsFalse(SmartsPattern.Create("[12*]", bldr).Matches(Smi("[13CH4]")));

            Assert.IsFalse(SmartsPattern.Create("[13*]", bldr).Matches(Smi("C")));
            Assert.IsFalse(SmartsPattern.Create("[13*]", bldr).Matches(Smi("[CH4]")));
            Assert.IsFalse(SmartsPattern.Create("[13*]", bldr).Matches(Smi("[12CH4]")));
            Assert.IsTrue(SmartsPattern.Create("[13*]", bldr).Matches(Smi("[13CH4]")));

            Assert.IsTrue(SmartsPattern.Create("[0*]", bldr).Matches(Smi("C")));
            Assert.IsTrue(SmartsPattern.Create("[0*]", bldr).Matches(Smi("[CH4]")));
            Assert.IsFalse(SmartsPattern.Create("[0*]", bldr).Matches(Smi("[12CH4]")));
            Assert.IsFalse(SmartsPattern.Create("[0*]", bldr).Matches(Smi("[13CH4]")));

            //      Not possible with current grammar
            //        assertFalse(SmartsPattern.create("[!0*]", bldr).matches(smi("C")));
            //        assertFalse(SmartsPattern.create("[!0*]", bldr).matches(smi("[CH4]")));
            //        assertTrue(SmartsPattern.create("[!0*]", bldr).matches(smi("[12CH4]")));
            //        assertTrue(SmartsPattern.create("[!0*]", bldr).matches(smi("[13CH4]")));
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

        [TestMethod(), Ignore()] // Not supported yet
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
            Assert.AreEqual(2, SmartsPattern.Create("([C:1]).([C:2])>>[C:1][C:2]", null).MatchAll(Rsmi("[C-:13]#[N:14].[K+].[CH:3]1=[CH:4][C:5](=[CH:11][CH:12]=[C:2]1[CH2:1]Br)[C:6](=[O:10])[CH:7]2[CH2:8][CH2:9]2>>[CH:3]1=[CH:4][C:5](=[CH:11][CH:12]=[C:2]1[CH2:1][C:13]#[N:14])[C:6](=[O:10])[CH:7]2[CH2:8][CH2:9]2 |f:0.1|")).Count());
        }

        [TestMethod()]
        public void MatchProductStereo()
        {
            Assert.AreEqual(1, SmartsPattern.Create(">>C[C@H](CC)[C@H](CC)O")
                                    .MatchAll(Rsmi(">>C[C@H](CC)[C@H](CC)O"))
                                    .CountUnique());
        }

        [TestMethod()]
        public void Stereo_ring_closures()
        {
            var ptrn = SmartsPattern.Create("[C@@]1(O[C@@]([C@@]([C@]([C@]1(C)O)(C)O)(O)C)(O)C)(O)C");
            Assert.IsTrue(ptrn.Matches(Smi("[C@@]1(O[C@@]([C@@]([C@]([C@]1(C)O)(C)O)(O)C)(O)C)(O)C")));
        }

        [TestMethod()]
        public void HasIsotope()
        {
            var ptrn = SmartsPattern.Create("[!0]");
            Assert.IsFalse(ptrn.Matches(Smi("C")));
            Assert.IsTrue(ptrn.Matches(Smi("[12C]")));
            Assert.IsTrue(ptrn.Matches(Smi("[13C]")));
        }

        [TestMethod()]
        public void HIsotope()
        {
            var ptrn = SmartsPattern.Create("[2#1,3#1]");
            Assert.IsFalse(ptrn.Matches(Smi("[H][H]")));
            Assert.IsTrue(ptrn.Matches(Smi("[2H]")));
            Assert.IsTrue(ptrn.Matches(Smi("[3H]")));
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
