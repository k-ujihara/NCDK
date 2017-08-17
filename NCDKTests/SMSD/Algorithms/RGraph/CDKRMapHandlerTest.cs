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
using NCDK.SMSD.Helper;
using System.Collections.Generic;

namespace NCDK.SMSD.Algorithms.RGraph
{
    // @cdk.module test-smsd
    // @author     Syed Asad Rahman
    [TestClass()]
    public class CDKRMapHandlerTest
    {
        private readonly CDKRMapHandler handler = new CDKRMapHandler();

        public CDKRMapHandlerTest() { }

        /// <summary>
        /// Test of getSource method, of class CDKRMapHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetSource()
        {
            IAtomContainer expResult = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            handler.Source = expResult;
            IAtomContainer result = handler.Source;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of setSource method, of class CDKRMapHandler.
        /// </summary>
        [TestMethod()]
        public void TestSetSource()
        {
            IAtomContainer expResult = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            handler.Source = expResult;
            IAtomContainer result = handler.Source;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of getTarget method, of class CDKRMapHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetTarget()
        {
            IAtomContainer expResult = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            handler.Target = expResult;
            IAtomContainer result = handler.Target;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of setTarget method, of class CDKRMapHandler.
        /// </summary>
        [TestMethod()]
        public void TestSetTarget()
        {
            IAtomContainer expResult = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            handler.Target = expResult;
            IAtomContainer result = handler.Target;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of calculateOverlapsAndReduce method, of class CDKRMapHandler.
        /// </summary>
        [TestMethod()]
        public void TestCalculateOverlapsAndReduce()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer Molecule1 = sp.ParseSmiles("O1C=CC=C1");
            IAtomContainer Molecule2 = sp.ParseSmiles("C1CCCC1");
            CDKRMapHandler instance = new CDKRMapHandler();
            instance.CalculateOverlapsAndReduce(Molecule1, Molecule2, true);
            Assert.IsNotNull(FinalMappings.Instance.Count);
        }

        /// <summary>
        /// Test of calculateOverlapsAndReduceExactMatch method, of class CDKRMapHandler.
        /// </summary>
        [TestMethod()]
        public void TestCalculateOverlapsAndReduceExactMatch()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer Molecule1 = sp.ParseSmiles("O1C=CC=C1");
            IAtomContainer Molecule2 = sp.ParseSmiles("O1C=CC=C1");
            CDKRMapHandler instance = new CDKRMapHandler();
            instance.CalculateOverlapsAndReduceExactMatch(Molecule1, Molecule2, true);
            // TODO review the generated test code and remove the default call to fail.
            Assert.IsNotNull(FinalMappings.Instance.Count);
        }

        /// <summary>
        /// Test of getMappings method, of class CDKRMapHandler.
        /// </summary>
        [TestMethod()]
        public void TestGetMappings()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer Molecule1 = sp.ParseSmiles("O1C=CC=C1");
            IAtomContainer Molecule2 = sp.ParseSmiles("O1C=CC=C1");
            CDKRMapHandler instance = new CDKRMapHandler();
            instance.CalculateOverlapsAndReduceExactMatch(Molecule1, Molecule2, true);
            var result = instance.Mappings;
            Assert.AreEqual(2, result.Count);
        }

        /// <summary>
        /// Test of setMappings method, of class CDKRMapHandler.
        /// </summary>
        [TestMethod()]
        public void TestSetMappings()
        {
            var map = new SortedDictionary<int, int>();
            map[0] = 0;
            map[1] = 1;

            var mappings = new List<IDictionary<int, int>>();
            mappings.Add(map);
            CDKRMapHandler instance = new CDKRMapHandler();
            instance.Mappings = mappings;
            Assert.IsNotNull(instance.Mappings);
        }

        /// <summary>
        /// Test of isTimeoutFlag method, of class CDKRMapHandler.
        /// </summary>
        [TestMethod()]
        public void TestIsTimeoutFlag()
        {
            CDKRMapHandler instance = new CDKRMapHandler();
            bool expResult = true;
            instance.IsTimedOut = true;
            bool result = instance.IsTimedOut;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of setTimeoutFlag method, of class CDKRMapHandler.
        /// </summary>
        [TestMethod()]
        public void TestSetTimeoutFlag()
        {
            bool timeoutFlag = false;
            CDKRMapHandler instance = new CDKRMapHandler();
            instance.IsTimedOut = timeoutFlag;
            Assert.AreNotEqual(true, instance.IsTimedOut);
        }
    }
}
