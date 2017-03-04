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
using System.Linq;

namespace NCDK.Smiles.SMARTS.Parser
{
    /// <summary>
    /// Verifies stereo matching. We check the counts to ensure that
    /// tetrahedral/geometric stereo isn't matching absolute values (i.e. R/S or
    /// odd/even // parity from MDL molfile)
    ///
    // @author John May
    // @cdk.module test-smarts
    /// </summary>
    [TestClass()]
    public class SmartsStereoTest
    {

        [TestMethod()]
        public void NonAbsoluteGeometric_trans()
        {
            AssertMatch("C/C=C/C", "C/C(CC)=C(CC)/C", 4, 2);
        }

        [TestMethod()]
        public void NonAbsoluteGeometric_cis()
        {
            AssertMatch("C(/C)=C/C", "C/C(CC)=C(CC)/C", 4, 2);
        }

        [TestMethod()]
        public void UnspecifiedGeometric()
        {
            AssertMatch("C/C=C/?Cl", "CC=CCl", 1, 1);
            AssertMatch("C/C=C/?Cl", "C/C=C/Cl", 1, 1);
            AssertMatch("C/?C=C/Cl", "CC=CCl", 1, 1);
            AssertMatch("C/?C=C/Cl", "C/C=C/Cl", 1, 1);
            AssertMatch("C/C=C/?Cl", "CC=CCl", 1, 1);
            AssertMatch("C/C=C/?Cl", "C/C=C/Cl", 1, 1);
        }

        [TestMethod()]
        public void NonAbsoluteTetrahedral()
        {
            AssertMatch("C[C@](C)(C)C", "C[C@](CC)(CCC)CCCC", 12, 1);
            AssertMatch("C[C@](C)(C)C", "C[C@@](CC)(CCC)CCCC", 12, 1);
        }

        [TestMethod()]
        public void TetrahedralNegation_anticlockwise()
        {
            AssertMatch("[!@](C)(N)(O)CC", "C(C)(N)(O)CC", 1, 1);
            AssertMatch("[!@](C)(N)(O)CC", "[C@@](C)(N)(O)CC", 1, 1);
            AssertMatch("[!@](C)(N)(O)CC", "[C@](C)(N)(O)CC", 0, 0);
        }

        [TestMethod()]
        public void TetrahedralNegation_clockwise()
        {
            AssertMatch("[!@@](C)(N)(O)CC", "C(C)(N)(O)CC", 1, 1);
            AssertMatch("[!@@](C)(N)(O)CC", "[C@@](C)(N)(O)CC", 0, 0);
            AssertMatch("[!@@](C)(N)(O)CC", "[C@](C)(N)(O)CC", 1, 1);
        }

        [TestMethod()]
        public void TetrahedralUnspecified_clockwise()
        {
            AssertMatch("[@@?](C)(N)(O)CC", "C(C)(N)(O)CC", 1, 1);
            AssertMatch("[@@?](C)(N)(O)CC", "[C@@](C)(N)(O)CC", 1, 1);
            AssertMatch("[@@?](C)(N)(O)CC", "[C@](C)(N)(O)CC", 0, 0);
        }

        [TestMethod()]
        public void TetrahedralUnspecified_anticlockwise()
        {
            AssertMatch("[@?](C)(N)(O)CC", "C(C)(N)(O)CC", 1, 1);
            AssertMatch("[@?](C)(N)(O)CC", "[C@@](C)(N)(O)CC", 0, 0);
            AssertMatch("[@?](C)(N)(O)CC", "[C@](C)(N)(O)CC", 1, 1);
        }

        [TestMethod()]
        public void Tetrahedral_or()
        {
            AssertMatch("C[@,@@](C)(C)C", "CC(CC)(CCC)CCCC", 0, 0);
            AssertMatch("C[@,@@](C)(C)C", "C[C@](CC)(CCC)CCCC", 24, 1);
            AssertMatch("C[@,@@](C)(C)C", "C[C@](CC)(CCC)CCCC", 24, 1);
        }

        [TestMethod()]
        public void Tetrahedral_and()
        {
            AssertMatch("C[@&@@](C)(C)C", "CC(CC)(CCC)CCCC", 0, 0);
            AssertMatch("C[@&@@](C)(C)C", "C[C@](CC)(CCC)CCCC", 0, 0);
            AssertMatch("C[@&@@](C)(C)C", "C[C@@](CC)(CCC)CCCC", 0, 0);
        }

        [TestMethod()]
        public void TetrahedralAndSymbol_or()
        {
            AssertMatch("C[C@,Si@@](CC)(CCC)CCCC", "CC(CC)(CCC)CCCCC", 0, 0);
            AssertMatch("C[C@,Si@@](CC)(CCC)CCCC", "C[Si](CC)(CCC)CCCCC", 0, 0);
            AssertMatch("C[C@,Si@@](CC)(CCC)CCCC", "C[C@](CC)(CCC)CCCC", 1, 1);
            AssertMatch("C[C@,Si@@](CC)(CCC)CCCC", "C[C@@](CC)(CCC)CCCC", 0, 0);
            AssertMatch("C[C@,Si@@](CC)(CCC)CCCC", "C[Si@](CC)(CCC)CCCC", 0, 0);
            AssertMatch("C[C@,Si@@](CC)(CCC)CCCC", "C[Si@@](CC)(CCC)CCCC", 1, 1);
        }

        [TestMethod()]
        public void RecursiveGeometric_trans()
        {
            AssertMatch("[$(*/C=C/*)]", "C/C=C/C", 2, 2);
            AssertMatch("[$(*/C=C/*)]", "F/C=C/Cl", 2, 2);
            AssertMatch("[$(*/C=C/*)]", "CC=CC", 0, 0);
            AssertMatch("[$(*/C=C/*)]", "FC=CCl", 0, 0);
            AssertMatch("[$(*/C=C/*)]", "C/C=C\\C", 0, 0);
            AssertMatch("[$(*/C=C/*)]", "F/C=C\\Cl", 0, 0);
        }

        [TestMethod()]
        public void RecursiveGeometric_cis()
        {
            AssertMatch("[$(C(/*)=C/*)]", "C/C=C/C", 0, 0);
            AssertMatch("[$(C(/*)=C/*)]", "F/C=C/Cl", 0, 0);
            AssertMatch("[$(C(/*)=C/*)]", "CC=CC", 0, 0);
            AssertMatch("[$(C(/*)=C/*)]", "FC=CCl", 0, 0);
            AssertMatch("[$(C(/*)=C/*)]", "C/C=C\\C", 2, 2);
            AssertMatch("[$(C(/*)=C/*)]", "F/C=C\\Cl", 2, 2);
        }

        [TestMethod()]
        public void RecursiveTetrahedral()
        {
            AssertMatch("[$([C@](C)(CC)(N)O)]", "C[C@@](N)(CC)O", 1, 1);
            AssertMatch("[$([C@](C)(CC)(N)O)]", "C[C@](N)(CC)O", 0, 0);
            AssertMatch("[$([C@](C)(CC)(N)O)]", "CC(N)(CC)O", 0, 0);
        }

        [TestMethod()]
        public void TetrahedralImplicitH()
        {
            AssertMatch("[C@H](C)(N)O", "[C@@H](N)(C)O", 1, 1);
            AssertMatch("[C@H](C)(N)O", "[C@H](N)(C)O", 0, 0);
            AssertMatch("[C@H](C)(N)O", "C(N)(C)O", 0, 0);
        }

        [TestMethod()]
        public void TetrahedralImplicitH_unspec()
        {
            AssertMatch("[C@?H](C)(N)O", "[C@@H](N)(C)O", 1, 1);
            AssertMatch("[C@?H](C)(N)O", "[C@H](N)(C)O", 0, 0);
            AssertMatch("[C@?H](C)(N)O", "C(N)(C)O", 1, 1);
        }

        static void AssertMatch(SMARTSQueryTool sqt, IAtomContainer m, int hits, int usaHits)
        {
            sqt.Matches(m);
            Assert.AreEqual(hits, sqt.GetMatchingAtoms().Count());
            Assert.AreEqual(usaHits, sqt.GetUniqueMatchingAtoms().Count());
        }

        static void AssertMatch(string smarts, string smiles, int hits, int usaHits)
        {
            AssertMatch(CreateFromSmarts(smarts), CreateFromSmiles(smiles), hits, usaHits);
        }

        static IAtomContainer CreateFromSmiles(string smiles)
        {
            return sp.ParseSmiles(smiles);
        }

        static SMARTSQueryTool CreateFromSmarts(string smarts)
        {
            return new SMARTSQueryTool(smarts, Default.ChemObjectBuilder.Instance);
        }

        private static readonly SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
    }
}
