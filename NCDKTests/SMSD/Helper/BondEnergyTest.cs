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

namespace NCDK.SMSD.Helper
{
    /// <summary>
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    ///
    // @cdk.module test-smsd
    // @cdk.require java1.6+
    /// </summary>
    [TestClass()]
    public class BondEnergyTest
    {
        public BondEnergyTest() { }

        /// <summary>
        /// Test of getSymbolFirstAtom method, of class BondEnergy.
        /// </summary>
        [TestMethod()]
        public void TestGetSymbolFirstAtom()
        {
            BondEnergy instance = new BondEnergy("H", "I", BondOrder.Single, 295);
            string expResult = "H";
            string result = instance.SymbolFirstAtom;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of getSymbolSecondAtom method, of class BondEnergy.
        /// </summary>
        [TestMethod()]
        public void TestGetSymbolSecondAtom()
        {
            BondEnergy instance = new BondEnergy("H", "I", BondOrder.Single, 295);
            string expResult = "I";
            string result = instance.SymbolSecondAtom;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of getBondOrder method, of class BondEnergy.
        /// </summary>
        [TestMethod()]
        public void TestGetBondOrder()
        {
            BondEnergy instance = new BondEnergy("H", "I", BondOrder.Single, 295);
            BondOrder expResult = BondOrder.Single;
            BondOrder result = instance.BondOrder;
            Assert.AreEqual(expResult, result);
        }

        /// <summary>
        /// Test of getEnergy method, of class BondEnergy.
        /// </summary>
        [TestMethod()]
        public void TestGetEnergy()
        {
            BondEnergy instance = new BondEnergy("H", "I", BondOrder.Single, 295);
            int expResult = 295;
            int result = instance.Energy;
            Assert.AreEqual(expResult, result);
        }
    }
}
