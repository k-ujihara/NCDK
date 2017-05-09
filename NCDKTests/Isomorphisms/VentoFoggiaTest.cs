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

using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;
using System.Linq;

namespace NCDK.Isomorphisms
{
    /// <summary>
    /// Simple tests for exact and non exact matching.
    /// </summary>
    // @author John May
    // @cdk.module test-isomorphism
    [TestClass()]
    public class VentoFoggiaTest
    {

        [TestMethod()]
        public void BenzeneIdentical()
        {
            int[] match = VentoFoggia.FindIdentical(TestMoleculeFactory.MakeBenzene()).Match(TestMoleculeFactory.MakeBenzene());
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2, 3, 4, 5 }, match));
            int count = VentoFoggia.FindIdentical(TestMoleculeFactory.MakeBenzene()).MatchAll(TestMoleculeFactory.MakeBenzene()).ToList().Count;
            Assert.AreEqual(6, count); // note: aromatic one would be 12
        }

        [TestMethod()]
        public void BenzeneNonIdentical()
        {
            int[] match = VentoFoggia.FindIdentical(TestMoleculeFactory.MakeBenzene()).Match(TestMoleculeFactory.MakeNaphthalene());
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], match));
            int count = VentoFoggia.FindIdentical(TestMoleculeFactory.MakeBenzene()).MatchAll(TestMoleculeFactory.MakeNaphthalene()).ToList().Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod()]
        public void BenzeneSubsearch()
        {
            int[] match = VentoFoggia.FindSubstructure(TestMoleculeFactory.MakeBenzene()).Match(TestMoleculeFactory.MakeNaphthalene());
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 7, 6, 5, 4, 3 }, match));
            int count =
                    VentoFoggia.FindSubstructure(TestMoleculeFactory.MakeBenzene()).MatchAll(
                            TestMoleculeFactory.MakeNaphthalene()).ToList().Count;
            Assert.AreEqual(6, count); // note: aromatic one would be 24
        }

        [TestMethod()]
        public void NapthaleneSubsearch()
        {
            int[] match = VentoFoggia.FindSubstructure(TestMoleculeFactory.MakeNaphthalene()).Match(
                    TestMoleculeFactory.MakeBenzene());
            Assert.IsTrue(Compares.AreDeepEqual(new int[0], match));
            int count =
                VentoFoggia.FindSubstructure(TestMoleculeFactory.MakeNaphthalene()).MatchAll(
                        TestMoleculeFactory.MakeBenzene()).ToList().Count;
            Assert.AreEqual(0, count);
        }
    }
}
