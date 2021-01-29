/*
 * Copyright (c) 2016 John May <jwmay@users.sf.net>
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Smiles
{
    [TestClass()]
    public class CxSmilesParserTest
    {
        [TestMethod()]
        public void AtomLabels()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|$;;;Het;;;;;A$|", state));
            Assert.IsTrue(state.atomLabels.Contains(new KeyValuePair<int, string>(3, "Het")));
            Assert.IsTrue(state.atomLabels.Contains(new KeyValuePair<int, string>(8, "A")));
        }
        
        [TestMethod()]
        public void EscapedAtomLabels()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|$R&#39;;;;;;;$|", state));
            Assert.IsTrue(state.atomLabels[0] == "R'");
        }

        [TestMethod()]
        public void EscapedAtomLabels2()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|$;;;&#40;C&#40;R41&#41;&#40;R41&#41;&#41;n;;R41;R41;R41;;_AP1;R41;R41;;_AP1$|", state));
            Assert.IsTrue(state.atomLabels[3] == "(C(R41)(R41))n");
        }

        [TestMethod()]
        public void AtomValues()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|$_AV:;;;5;;;;;8$|", state));
            Assert.IsTrue(state.atomValues.Contains(new KeyValuePair<int, string>(3, "5")));
            Assert.IsTrue(state.atomValues.Contains(new KeyValuePair<int, string>(8, "8")));
        }

        [TestMethod()]
        public void AtomLabelsEmpty()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|$$|", state));
            Assert.AreEqual(0, state.atomLabels.Count);
        }

        [TestMethod()]
        public void AtomLabelsTruncated1()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreEqual(-1, CxSmilesParser.ProcessCx("|$;;;Het", state));
            Assert.AreEqual(-1, CxSmilesParser.ProcessCx("|$;;;Het;", state));
        }

        [TestMethod()]
        public void AtomLabelsTruncated3()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreEqual(-1, CxSmilesParser.ProcessCx("|$;;;Het;$", state));
        }

        [TestMethod()]
        public void RemoveUnderscore()
        {
            CxSmilesState state = new CxSmilesState();
            CxSmilesParser.ProcessCx("|$;;;_R1;$|", state);
            Assert.AreEqual("R1", state.atomLabels[3]);
        }

        [TestMethod()]
        public void SkipCis()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|c:1,4,5|", state));
        }

        [TestMethod()]
        public void SkipTrans()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|t:1,4,5|", state));
        }

        [TestMethod()]
        public void SkipCisTrans()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|c:2,6,8,t:1,4,5|", state));
        }

        [TestMethod()]
        public void SkipCisTransUnspec()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|c:2,6,8,ctu:10,t:1,4,5|", state));
        }

        [TestMethod()]
        public void SkipLonePairDefinitions()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|c:6,8,t:4,lp:2:2,4:1,11:1,m:1:8.9|", state));
        }

        [TestMethod()]
        public void FragmentGrouping()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|f:0.1.2.3,4.5.6|", state));
            Assert.IsTrue(Compares.AreDeepEqual(
                new[] { new[] { 0, 1, 2, 3 }, new[] { 4, 5, 6 } },
                state.fragGroups));
        }

        [TestMethod()]
        public void FragmentGroupingFollowedByAtomLabels()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|f:0.1.2.3,4.5.6,$;;;R$|", state));
            Assert.IsTrue(Compares.AreDeepEqual(
                new[] { new[] { 0, 1, 2, 3 }, new[] { 4, 5, 6 } },
                state.fragGroups));
            Assert.IsTrue(state.atomLabels.Contains(new KeyValuePair<int, string>(3, "R")));
        }

        [TestMethod()]
        public void Coords()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|(.0,-1.5,;-1.3,-.75,;-2.6,-1.5,;-3.9,-.75,;-3.9,.75,)|", state));
            Assert.IsTrue(new AprxDoubleArray(0, -1.5, 0).Matches(state.atomCoords[0]));
            Assert.IsTrue(new AprxDoubleArray(-1.3, -.75, 0).Matches(state.atomCoords[1]));
            Assert.IsTrue(new AprxDoubleArray(-2.6, -1.5, 0).Matches(state.atomCoords[2]));
            Assert.IsTrue(new AprxDoubleArray(-3.9, -.75, 0).Matches(state.atomCoords[3]));
            Assert.IsTrue(new AprxDoubleArray(-3.9, .75, 0).Matches(state.atomCoords[4]));
        }

        [TestMethod()]
        public void HydrogenBondingSkipped()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|H:0.1,2.3|", state));
        }

        [TestMethod()]
        public void HydrogenBondingTruncated()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreEqual(-1, CxSmilesParser.ProcessCx("|H:0.1,2.3", state));
            Assert.AreEqual(-1, CxSmilesParser.ProcessCx("|H:0.1,2.", state));
        }

        [TestMethod()]
        public void HydrogenAndCoordinationBondingSkipped()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|H:0.1,2.3,C:6.7,3.4|", state));
        }

        [TestMethod()]
        public void PositionalVariation()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|m:2:5.6.7.8.9.10,m:4:5.6.7.8.9|", state));
            Assert.IsTrue(Compares.DeepContains(
                state.positionVar,
                new KeyValuePair<int, IList<int>>(2, new[] { 5, 6, 7, 8, 9, 10 })));
            Assert.IsTrue(Compares.DeepContains(
                state.positionVar,
                new KeyValuePair<int, IList<int>>(4, new[] { 5, 6, 7, 8, 9 })));
        }

        [TestMethod()]
        public void PositionalVariationImpliedLayer()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|m:2:5.6.7.8.9.10,4:5.6.7.8.9|", state));
            Assert.IsTrue(Compares.DeepContains(
                state.positionVar,
                new KeyValuePair<int, IList<int>>(2, new[] { 5, 6, 7, 8, 9, 10 })));
            Assert.IsTrue(Compares.DeepContains(
                state.positionVar,
                new KeyValuePair<int, IList<int>>(4, new[] { 5, 6, 7, 8, 9 })));
        }

        [TestMethod()]
        public void MultiAtomSRU()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|Sg:n:1,2,3:m:ht|", state));
            Assert.IsTrue(Compares.DeepContains(
                state.mysgroups,
                new CxSmilesState.CxPolymerSgroup("n", new[] { 1, 2, 3 }, "m", "ht")));
        }

        [TestMethod()]
        public void DataSgroups()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|SgD::cdk&#58;ReactionConditions:Heat&#10;Hv|", state));
            Assert.IsTrue(Compares.DeepContains(
                state.mysgroups,
                new CxSmilesState.CxDataSgroup(new List<int>(), "cdk:ReactionConditions", "Heat\nHv", "", "", "")));
        }

        [TestMethod()]
        public void Unescape()
        {
            Assert.AreEqual("$", CxSmilesParser.Unescape("&#36;"));
            Assert.AreEqual("\u007F", CxSmilesParser.Unescape("&#127;")); // DEL
            Assert.AreEqual("\t", CxSmilesParser.Unescape("&#9;")); // TAB
        }

        [TestMethod()]
        public void RelativeStereoMolecule()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|r|", state));
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|r,$_R1$|", state));
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|$_R1$,r|", state));
        }

        [TestMethod()]
        public void RelativeStereoReaction()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx("|r:2,4,5|", state));
        }

        [TestMethod()]
        public void SgroupHierarchy()
        {
            const string cxsmilayers = "|Sg:c:0,1,2,3,4,5,6::,Sg:c:7,8::,Sg:c:9::,Sg:mix:0,1,2,3,4,5,6,7,8,9::,Sg:mix:7,8,9::,SgH:3:4.0,4:2.1|";
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx(cxsmilayers, state));
        }

        [TestMethod()]
        public void CxSmiLay()
        {
            const string cxsmilayers = "|Sg:c:0,1,2,3,4,5,6::,Sg:c:7,8::,Sg:c:9::,Sg:mix:0,1,2,3,4,5,6,7,8,9::,Sg:mix:7,8,9::,SgD::RATIO:1/3::,SgD::RATIO:2/3::,SgD::WEIGHT_PERCENT:15::%,SgH:3:4.0,0:7,4:2.1,1:5,2:6|";
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx(cxsmilayers, state));
        }

        [TestMethod()]
        public void CxSmiLay2()
        {
            const string cxsmilayers = "|Sg:c:0,1,2::,Sg:c:3::,Sg:mix:0,1,2,3::,SgH:2:1.0|";
            CxSmilesState state = new CxSmilesState();
            Assert.AreNotEqual(-1, CxSmilesParser.ProcessCx(cxsmilayers, state));
        }

        /// <summary>
        /// Custom matcher for checking an array of doubles closely matches (epsilon=0.01)
        /// an expected value.
        /// </summary>
        private class AprxDoubleArray
        {
            readonly double[] expected;
            const double epsilon = 0.01;

            public AprxDoubleArray(params double[] expected)
            {
                this.expected = expected;
            }

            public bool Matches(object o)
            {
                double[] actual = (double[])o;
                if (expected.Length != actual.Length)
                    return false;
                for (int i = 0; i < expected.Length; i++)
                {
                    if (Math.Abs(expected[i] - actual[i]) > epsilon)
                        return false;
                }
                return true;
            }
        }
    }
}
