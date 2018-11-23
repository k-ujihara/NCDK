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
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles;
using System.IO;
using System.Linq;

namespace NCDK.SMARTS
{
    [TestClass()]
    public abstract class SubstructureTest
    {
        public abstract Pattern Create(IAtomContainer container);

        // ensure edges can be absent in the target
        [TestMethod()]
        public void Monomorphism1()
        {
            AssertMatch(Smi("CCC"), Smi("C1CC1"), 6);
        }

        [TestMethod()]
        public void Monomorphism2()
        {
            AssertMatch(Smi("C1CCCCCCCCC1"), Smi("C1CCC2CCCCC2C1"), 20);
        }

        [TestMethod()]
        public void Cyclopropane()
        {
            AssertMismatch(Smi("C1CC1"), Smi("CC(C)C"));
        }

        [TestMethod()]
        public void Symmetric()
        {
            AssertMatch(Sma("C**C"), Smi("CSSC"));
            AssertMismatch(Sma("C**C"), Smi("SCCS"));
        }

        [TestMethod()]
        public void DisconnectedQuery()
        {
            AssertMatch(Smi("C.C"), Smi("CC"), 2);
        }

        [TestMethod()]
        public void DisconnectedTarget()
        {
            AssertMatch(Smi("C1CC1"), Smi("C1CC1.C1CC1"), 12);
        }

        [TestMethod()]
        public void Disconnected()
        {
            AssertMatch(Smi("C1CC1.C1CC1"), Smi("C1CC1.C1CC1"), 72);
        }

        // original VF algorithm can't find both of these
        [TestMethod()]
        public void Disconnected2()
        {
            AssertMatch(Smi("O.O"), Smi("OO"), 2);
            AssertMatch(Smi("O.O"), Smi("OCO"), 2);
            AssertMatch(Smi("O.O"), Smi("OCCO"), 2);
            AssertMatch(Smi("O.O"), Smi("OCCCO"), 2);
        }

        [TestMethod()]
        public void Tetrahedral_match()
        {
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](C)(N)(O)CC"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](C)(O)(CC)N"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](C)(CC)(N)(O)"));

            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](C)(O)(N)CC"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](C)(CC)(O)N"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](C)(N)(CC)(O)"));

            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](N)(O)(C)CC"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](N)(CC)(O)C"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](N)(C)(CC)O"));

            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](N)(C)(O)CC"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](N)(O)(CC)C"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](N)(CC)(C)O"));

            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](O)(CC)(C)N"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](O)(N)(CC)C"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](O)(C)(N)(CC)"));

            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](O)(C)(CC)N"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](O)(CC)(N)C"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](O)(N)(C)(CC)"));

            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](CC)(C)(O)N"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](CC)(N)(C)O"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@](CC)(O)(N)C"));

            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](CC)(O)(C)N"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](CC)(C)(N)O"));
            AssertMatch(Smi("[C@](C)(N)(O)CC"), Smi("[C@@](CC)(N)(O)C"));
        }

        [TestMethod()]
        public void Tetrahedral_mismatch()
        {
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](C)(N)(O)CC"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](C)(O)(CC)N"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](C)(CC)(N)(O)"));

            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](C)(O)(N)CC"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](C)(CC)(O)N"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](C)(N)(CC)(O)"));

            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](N)(O)(C)CC"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](N)(CC)(O)C"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](N)(C)(CC)O"));

            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](N)(C)(O)CC"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](N)(O)(CC)C"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](N)(CC)(C)O"));

            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](O)(CC)(C)N"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](O)(N)(CC)C"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](O)(C)(N)(CC)"));

            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](O)(C)(CC)N"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](O)(CC)(N)C"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](O)(N)(C)(CC)"));

            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](CC)(C)(O)N"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](CC)(N)(C)O"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@](CC)(O)(N)C"));

            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](CC)(O)(C)N"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](CC)(C)(N)O"));
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("[C@@](CC)(N)(O)C"));
        }

        [TestMethod()]
        public void Tetrahedral_match_implicit_h()
        {
            AssertMatch(Smi("[C@H](C)(N)(O)"), Smi("[C@H](C)(N)(O)"));
            AssertMatch(Smi("[C@H](C)(N)(O)"), Smi("[C@]([H])(C)(N)(O)"));
            AssertMatch(Smi("[C@H](C)(N)(O)"), Smi("[C@@](C)([H])(N)(O)"));
        }

        [TestMethod()]
        public void Tetrahedral_mismatch_implicit_h()
        {
            AssertMismatch(Smi("[C@H](C)(N)(O)"), Smi("[C@@H](C)(N)(O)"));
            AssertMismatch(Smi("[C@H](C)(N)(O)"), Smi("[C@@]([H])(C)(N)(O)"));
            AssertMismatch(Smi("[C@H](C)(N)(O)"), Smi("[C@](C)([H])(N)(O)"));
        }

        [TestMethod()]
        public void Tetrahedral_match_sulfoxide()
        {
            AssertMatch(Smi("[S@](=O)(C)CC"), Smi("[S@](=O)(C)CC"));
            AssertMatch(Smi("[S@](=O)(C)CC"), Smi("[S@](C)(CC)(=O)"));
            AssertMatch(Smi("[S@](=O)(C)CC"), Smi("[S@](CC)(=O)C"));
            AssertMatch(Smi("[S@](=O)(C)CC"), Smi("[S@@](C)(=O)CC"));
            AssertMatch(Smi("[S@](=O)(C)CC"), Smi("[S@@](=O)(CC)C"));
            AssertMatch(Smi("[S@](=O)(C)CC"), Smi("[S@@](CC)(C)=O"));
        }

        [TestMethod()]
        public void Tetrahedral_mismatch_sulfoxide()
        {
            AssertMismatch(Smi("[S@@](=O)(C)CC"), Smi("[S@](=O)(C)CC"));
            AssertMismatch(Smi("[S@@](=O)(C)CC"), Smi("[S@](C)(CC)(=O)"));
            AssertMismatch(Smi("[S@@](=O)(C)CC"), Smi("[S@](CC)(=O)C"));
            AssertMismatch(Smi("[S@@](=O)(C)CC"), Smi("[S@@](C)(=O)CC"));
            AssertMismatch(Smi("[S@@](=O)(C)CC"), Smi("[S@@](=O)(CC)C"));
            AssertMismatch(Smi("[S@@](=O)(C)CC"), Smi("[S@@](CC)(C)=O"));
        }

        [TestMethod()]
        public void Tetrahedral_missing_in_query()
        {
            AssertMatch(Smi("C(C)(N)(O)CC"), Smi("[C@@](C)(N)(O)CC"));
        }

        [TestMethod()]
        public void Tetrahedral_missing_in_target()
        {
            AssertMismatch(Smi("[C@@](C)(N)(O)CC"), Smi("C(C)(N)(O)CC"));
        }

        [TestMethod()]
        public void Tetrahedral_count()
        {
            // we can map any witch way 4 neighbours but 2 configuration so (4!/2) = 12
            AssertMatch(Smi("[C@](C)(C)(C)C"), Smi("[C@](C)(CC)(CCC)CCCC"), 12);
            AssertMatch(Smi("[C@@](C)(C)(C)C"), Smi("[C@](C)(CC)(CCC)CCCC"), 12);
            AssertMatch(Smi("[C@](C)(C)(C)C"), Smi("[C@@](C)(CC)(CCC)CCCC"), 12);
            AssertMatch(Smi("[C@@](C)(C)(C)C"), Smi("[C@@](C)(CC)(CCC)CCCC"), 12);
        }

        [TestMethod()]
        public void Geometric_trans_match()
        {
            AssertMatch(Smi("F/C=C/F"), Smi("F/C=C/F"));
            AssertMatch(Smi("F/C=C/F"), Smi("F\\C=C\\F"));
            // shouldn't mater which substituents are used
            AssertMatch(Smi("F/C=C/F"), Smi("F/C(/[H])=C/F"));
            AssertMatch(Smi("F/C=C/F"), Smi("FC(/[H])=C/F"));
            AssertMatch(Smi("F/C=C/F"), Smi("F/C=C([H])/F"));
            AssertMatch(Smi("F/C=C/F"), Smi("F/C=C(\\[H])F"));
            AssertMatch(Smi("F/C=C/F"), Smi("FC(/[H])=C(\\[H])F"));
            // or the order is different
            AssertMatch(Smi("F/C=C/F"), Smi("C(\\F)=C/F"));
            AssertMatch(Smi("F/C=C/F"), Smi("C(/F)=C\\F"));
        }

        [TestMethod()]
        public void Geometric_cis_match()
        {
            AssertMatch(Smi("F/C=C\\F"), Smi("F/C=C\\F"));
            AssertMatch(Smi("F/C=C\\F"), Smi("F\\C=C/F"));
            AssertMatch(Smi("F\\C=C/F"), Smi("F/C=C\\F"));
            AssertMatch(Smi("F\\C=C/F"), Smi("F\\C=C/F"));
            // shouldn't mater which substituents are used
            AssertMatch(Smi("F/C=C\\F"), Smi("F/C(/[H])=C\\F"));
            AssertMatch(Smi("F/C=C\\F"), Smi("FC(/[H])=C\\F"));
            AssertMatch(Smi("F/C=C\\F"), Smi("F/C=C([H])\\F"));
            AssertMatch(Smi("F/C=C\\F"), Smi("F/C=C(/[H])F"));
            AssertMatch(Smi("F/C=C\\F"), Smi("FC(/[H])=C(/[H])F"));
            // or the order is different
            AssertMatch(Smi("F/C=C\\F"), Smi("C(\\F)=C\\F"));
            AssertMatch(Smi("F/C=C\\F"), Smi("C(/F)=C/F"));
        }

        [TestMethod()]
        public void Geometric_trans_mismatch()
        {
            AssertMismatch(Smi("F/C=C/F"), Smi("F/C=C\\F"));
            AssertMismatch(Smi("F/C=C/F"), Smi("F\\C=C/F"));
            AssertMismatch(Smi("F\\C=C\\F"), Smi("F/C=C\\F"));
            AssertMismatch(Smi("F\\C=C\\F"), Smi("F\\C=C/F"));
        }

        [TestMethod()]
        public void Geometric_cis_mismatch()
        {
            AssertMismatch(Smi("F/C=C\\F"), Smi("F/C=C/F"));
            AssertMismatch(Smi("F/C=C\\F"), Smi("F\\C=C\\F"));
            AssertMismatch(Smi("F\\C=C/F"), Smi("F/C=C/F"));
            AssertMismatch(Smi("F\\C=C/F"), Smi("F\\C=C\\F"));
        }

        [TestMethod()]
        public void Geometric_missing_in_query()
        {
            AssertMatch(Smi("FC=CF"), Smi("F/C=C/F"));
            AssertMatch(Smi("FC=CF"), Smi("F\\C=C\\F"));
            AssertMatch(Smi("FC=CF"), Smi("F\\C=C/F"));
            AssertMatch(Smi("FC=CF"), Smi("F/C=C\\F"));
        }

        [TestMethod()]
        public void Geometric_missing_in_target()
        {
            AssertMismatch(Smi("F/C=C/F"), Smi("FC=CF"));
            AssertMismatch(Smi("F/C=C\\F"), Smi("FC=CF"));
            AssertMismatch(Smi("F\\C=C/F"), Smi("FC=CF"));
            AssertMismatch(Smi("F\\C=C\\F"), Smi("FC=CF"));
        }

        [TestMethod()]
        public void Geometric_count()
        {
            AssertMatch(Smi("C/C=C/C"), Smi("CC(/CC)=C(/CC)C"), 4);
            AssertMatch(Smi("C/C=C\\C"), Smi("CC(/CC)=C(/CC)C"), 4);
            AssertMatch(Smi("C\\C=C\\C"), Smi("CC(/CC)=C(/CC)C"), 4);
            AssertMatch(Smi("C\\C=C/C"), Smi("CC(/CC)=C(/CC)C"), 4);
        }

        [TestMethod()]
        public void Cubane_automorphisms()
        {
            AssertMatch(Smi("C12C3C4C1C1C2C3C41"), Smi("C12C3C4C1C1C2C3C41"), 48);
        }

        [TestMethod()]
        public void Fullerene_c60()
        {
            AssertMatch(
                    Smi("C1CCCCC1"),
                    Smi("C12C3C4C5C1C1C6C7C2C2C8C3C3C9C4C4C%10C5C5C1C1C6C6C%11C7C2C2C7C8C3C3C8C9C4C4C9C%10C5C5C1C1C6C6C%11C2C2C7C3C3C8C4C4C9C5C1C1C6C2C3C41"),
                    240);
        }

        [TestMethod()]
        public void Fullerene_c70()
        {
            AssertMatch(
                    Smi("C1CCCCC1"),
                    Smi("C12C3C4C5C1C1C6C7C5C5C8C4C4C9C3C3C%10C2C2C1C1C%11C%12C%13C%14C%15C%16C%17C%18C%19C%20C%16C%16C%14C%12C%12C%14C%21C%22C(C%20C%16%14)C%14C%19C%16C(C4C8C(C%18%16)C4C%17C%15C(C7C54)C%13C61)C1C%14C%22C(C3C91)C1C%21C%12C%11C2C%101"),
                    300);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void Fullerene_c70_automorphisms()
        {
            AssertMatch(
                    Smi("C12C3C4C5C1C1C6C7C5C5C8C4C4C9C3C3C%10C2C2C1C1C%11C%12C%13C%14C%15C%16C%17C%18C%19C%20C%16C%16C%14C%12C%12C%14C%21C%22C(C%20C%16%14)C%14C%19C%16C(C4C8C(C%18%16)C4C%17C%15C(C7C54)C%13C61)C1C%14C%22C(C3C91)C1C%21C%12C%11C2C%101"),
                    Smi("C12C3C4C5C1C1C6C7C5C5C8C4C4C9C3C3C%10C2C2C1C1C%11C%12C%13C%14C%15C%16C%17C%18C%19C%20C%16C%16C%14C%12C%12C%14C%21C%22C(C%20C%16%14)C%14C%19C%16C(C4C8C(C%18%16)C4C%17C%15C(C7C54)C%13C61)C1C%14C%22C(C3C91)C1C%21C%12C%11C2C%101"),
                    20);
        }

        [TestMethod()]
        public void Ferrocene_automorphisms_disconnected()
        {
            AssertMatch(Smi("[Fe].C1CCCC1.C1CCCC1"), Smi("[Fe].C1CCCC1.C1CCCC1"), 200);
        }

        [TestMethod()]
        public void Ferrocene_automorphisms()
        {
            AssertMatch(Smi("[Fe]123456789C%10C1C2C3C4%10.C51C6C7C8C91"), Smi("[Fe]123456789C%10C1C2C3C4%10.C51C6C7C8C91"), 200);
        }

        [TestMethod()]
        public void Butanoylurea()
        {
            AssertMatch(Smi("CCCC(=O)NC(N)=O"), Smi("CCC(Br)(CC)C(=O)NC(=O)NC(C)=O"), 2);
        }

        [TestMethod()]
        public void UpgradeHydrogen()
        {
            AssertMatch(Smi("CC[C@@H](C)O"), Smi("CC[C@](C)([H])O"), 1);
        }

        public void ShouldMatchAsSmilesButNotSmarts()
        {
            // H > 0
            AssertMismatch(Sma("CC[C@@H](C)O"), Smi("CC[C@](C)(N)O"));
        }

        [TestMethod()]
        public void Sulfoxide()
        {
            AssertMatch(Sma("[S@](=O)(C)CC"), Smi("O=[S@@](C)CC"), 1);
            AssertMatch(Smi("O1.[S@]=1(C)CC"), Smi("O=[S@@](C)CC"), 1);
            AssertMatch(Sma("O1.[S@]=1(C)CC"), Smi("O=[S@@](C)CC"), 1);
        }

        // doesn't matter if the match takes place but it should not cause and error
        // if the query is larger than the target
        [TestMethod()]
        public void LargerQuery()
        {
            AssertMismatch(Smi("CCCC"), Smi("CC"));
        }

        [TestMethod()]
        public void EmptyQuery()
        {
            AssertMismatch(Smi(""), Smi("[H][H]"));
        }

        [TestMethod()]
        public void EmptyTarget()
        {
            AssertMismatch(Smi("[H][H]"), Smi(""));
        }

        void AssertMatch(IAtomContainer query, IAtomContainer target, int count)
        {
            Assert.AreEqual(count, Create(query).MatchAll(target).Count(), $"{query.Title} should match {target.Title} {count} times");
        }

        void AssertMatch(IAtomContainer query, IAtomContainer target)
        {
            Assert.IsTrue(Create(query).Matches(target), $"{query.Title} should match {target.Title}");
        }

        void AssertMismatch(IAtomContainer query, IAtomContainer target)
        {
            Assert.IsFalse(Create(query).Matches(target), $"{query.Title} should not matched {target.Title}");
        }

        private static readonly SmilesParser sp = CDK.SmilesParser;

        // create a container from a smiles string
        protected static IAtomContainer Smi(string smi)
        {
            var container = sp.ParseSmiles(smi);
            container.Title = smi;
            return container;
        }

        // create a query container from a smarts pattern
        // Note: only use simple constructs! the target properties will not
        // currently be initialised. avoid aromaticity, rings etc.
        protected static IAtomContainer Sma(string sma)
        {
            var query = new QueryAtomContainer(CDK.Builder);
            if (!Smarts.Parse(query, sma))
            {
                throw new IOException(Smarts.GetLastErrorMessage());
            }
            query.Title = sma;
            return query;
        }
    }
}
