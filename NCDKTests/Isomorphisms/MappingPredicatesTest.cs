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
using NCDK.Graphs;
using NCDK.Smiles;
using System.Linq;

namespace NCDK.Isomorphisms
{
    /// <summary>
    // @author John May
    // @cdk.module test-smarts
    /// </summary>
    [TestClass()]
    public class MappingPredicatesTest
    {
        [TestMethod()]
        public void UniqueAtoms()
        {
            UniqueAtomMatches uam = new UniqueAtomMatches();
            Assert.IsTrue(uam.Apply(new int[] { 1, 2, 3, 4 }));
            Assert.IsTrue(uam.Apply(new int[] { 1, 2, 3, 5 }));
            Assert.IsFalse(uam.Apply(new int[] { 4, 3, 2, 1 }));
            Assert.IsFalse(uam.Apply(new int[] { 1, 5, 2, 3 }));
        }

        [TestMethod()]
        public void UniqueBonds()
        {
            IAtomContainer query = Smi("C1CCC1");
            IAtomContainer target = Smi("C12C3C1C23");

            var mappings = VentoFoggia.FindSubstructure(query).MatchAll(target);

            // using unique atoms we may think we only found 1 mapping
            {
                var p = new UniqueAtomMatches();
                Assert.AreEqual(1, mappings.Count(n => p.Apply(n)));
            }

            // when in fact we found 4 different mappings
            {
                var p = new UniqueBondMatches(GraphUtil.ToAdjList(query));
                Assert.AreEqual(3, mappings.Count(n => p.Apply(n)));
            }
        }

        [TestMethod()]
        public void UniqueAtoms_multipleIterations()
        {
            IAtomContainer ethane = Smi("CC");
            IAtomContainer ethanol = Smi("CCO");
            Mappings mappings = Pattern.FindSubstructure(ethane).MatchAll(ethanol);
            Assert.AreEqual(1, mappings.CountUnique());
            Assert.AreEqual(1, mappings.CountUnique()); // re-iteration
        }

        [TestMethod()]
        public void UniqueBonds_multipleIterations()
        {
            IAtomContainer ethane = Smi("CC");
            IAtomContainer ethanol = Smi("CCO");
            Mappings mappings = Pattern.FindSubstructure(ethane).MatchAll(ethanol);
            Assert.AreEqual(1, mappings.GetUniqueBonds().Count());
            Assert.AreEqual(1, mappings.GetUniqueBonds().Count()); // re-iteration
        }

        SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);

        IAtomContainer Smi(string smi)
        {
            return smipar.ParseSmiles(smi);
        }
    }
}
