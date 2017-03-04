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
using NCDK.Smiles;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.SMSD.Filters
{
    /// <summary>
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    ///
    // @cdk.module test-smsd
    // @cdk.require java1.6+
    /// </summary>
    [TestClass()]
    public class ChemicalFiltersTest
    {
        public ChemicalFiltersTest() { }

        /// <summary>
        /// Test of sortResultsByStereoAndBondMatch method, of class ChemicalFilters.
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestSortResultsByStereoAndBondMatch()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/OCC=C");
            IAtomContainer queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd = new Isomorphism(Algorithm.Default, false);
            smsd.Init(queryac, target, true, true);
            smsd.SetChemFilters(false, false, false);
            Assert.AreEqual(4, smsd.GetAllAtomMapping().Count);

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            Assert.AreEqual(1, smsd1.GetAllAtomMapping().Count);
        }

        /// <summary>
        /// Test of sortResultsByFragments method, of class ChemicalFilters.
        // @throws InvalidSmilesException
        // @throws CDKException
        /// </summary>
        [TestMethod()]
        public void TestSortResultsByFragments()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd = new Isomorphism(Algorithm.CDKMCS, false);
            smsd.Init(queryac, target, true, true);
            smsd.SetChemFilters(false, false, false);
            Assert.AreEqual(4, smsd.GetAllAtomMapping().Count);

            Isomorphism smsd1 = new Isomorphism(Algorithm.CDKMCS, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(false, true, false);
            Assert.AreEqual(2, smsd1.GetAllAtomMapping().Count);
        }

        /// <summary>
        /// Test of sortResultsByEnergies method, of class ChemicalFilters.
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestSortResultsByEnergies()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd = new Isomorphism(Algorithm.Default, true);
            smsd.Init(queryac, target, true, true);
            smsd.SetChemFilters(false, false, false);
            Assert.AreEqual(4, smsd.GetAllAtomMapping().Count);

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(false, false, true);
            Assert.AreEqual(2, smsd1.GetAllAtomMapping().Count);
        }

        /// <summary>
        /// Test of sortMapByValueInAccendingOrder method, of class ChemicalFilters.
        /// </summary>
        [TestMethod()]
        public void TestSortMapByValueInAccendingOrder()
        {
            IDictionary<int, double> map = new Dictionary<int, double>();
            map[1] = 3.0;
            map[2] = 2.0;
            map[3] = 1.0;
            map[4] = 4.0;
            IDictionary<int, double> expResult = new Dictionary<int, double>();
            expResult[3] = 1.0;
            expResult[2] = 2.0;
            expResult[1] = 3.0;
            expResult[4] = 4.0;

            var result = ChemicalFilters.SortMapByValueInAccendingOrder(map);

            Assert.IsTrue(AreOrderedEqual(expResult, result));
        }

        /// <summary>
        /// Test of sortMapByValueInDecendingOrder method, of class ChemicalFilters.
        /// </summary>
        [TestMethod()]
        public void TestSortMapByValueInDecendingOrder()
        {
            IDictionary<int, double> map = new Dictionary<int, double>();
            map[1] = 3.0;
            map[2] = 2.0;
            map[3] = 1.0;
            map[4] = 4.0;
            IDictionary<int, double> expResult = new Dictionary<int, double>();
            expResult[4] = 4.0;
            expResult[1] = 3.0;
            expResult[2] = 2.0;
            expResult[3] = 1.0;

            var result = ChemicalFilters.SortMapByValueInDecendingOrder(map);
            Assert.IsTrue(AreOrderedEqual(expResult, result));
        }

        private static bool AreOrderedEqual<T, V>(IEnumerable<KeyValuePair<T, V>> expected, IEnumerable<KeyValuePair<T, V>> actual)
        {
            if (expected.Count() != actual.Count())
                return false;
            var ee = expected.GetEnumerator();
            var ea = actual.GetEnumerator();
            while (ee.MoveNext())
            {
                ea.MoveNext();
                if (!object.Equals(ee.Current.Key, ea.Current.Key))
                    return false;
                if (!object.Equals(ee.Current.Value, ea.Current.Value))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Test of getSortedEnergy method, of class ChemicalFilters.
        // @throws InvalidSmilesException
        // @throws CDKException
        /// </summary>
        [TestMethod()]
        public void TestGetSortedEnergy()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(false, false, true);
            double score = 610.0;
            Assert.AreEqual(score, smsd1.GetEnergyScore(0));
        }

        /// <summary>
        /// Test of getSortedFragment method, of class ChemicalFilters.
        // @throws InvalidSmilesException
        // @throws CDKException
        /// </summary>
        [TestMethod()]
        public void TestGetSortedFragment()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(false, true, false);
            int score = 2;
            Assert.AreEqual(score, smsd1.GetFragmentSize(0));
        }

        /// <summary>
        /// Test of getStereoMatches method, of class ChemicalFilters.
        // @throws InvalidSmilesException
        // @throws CDKException
        /// </summary>
        [TestMethod()]
        public void TestGetStereoMatches()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/OCC=C");
            IAtomContainer queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            int score = 1048;
            Assert.AreEqual(score, smsd1.GetStereoScore(0));
        }
    }
}
