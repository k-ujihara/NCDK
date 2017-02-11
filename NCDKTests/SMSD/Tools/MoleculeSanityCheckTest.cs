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

namespace NCDK.SMSD.Tools
{
    /**
     * @cdk.module test-smsd
     * @author Asad
     */
    [TestClass()]
    public class MoleculeSanityCheckTest
    {

        public MoleculeSanityCheckTest() { }

        /**
         * Test of checkAndCleanMolecule method, of class MoleculeSanityCheck.
         * @throws InvalidSmilesException
         */
        [TestMethod()]
        public void TestCheckAndCleanMolecule()
        {
            string fragmentMolSmiles = "C1=CC=CC=C1.C1=CC2=C(C=C1)C=CC=C2";
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = sp.ParseSmiles(fragmentMolSmiles);
            IAtomContainer expResult = sp.ParseSmiles("C1=CC2=C(C=C1)C=CC=C2");
            IAtomContainer result = MoleculeSanityCheck.CheckAndCleanMolecule(molecule);
            Assert.AreEqual(expResult.Bonds.Count, result.Bonds.Count);
        }

        /**
         * Test of fixAromaticity method, of class MoleculeSanityCheck.
         * @throws InvalidSmilesException
         */
        [TestMethod()]
        public void TestFixAromaticity()
        {
            string rawMolSmiles = "C1=CC2=C(C=C1)C=CC=C2";
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles(rawMolSmiles);
            MoleculeSanityCheck.CheckAndCleanMolecule(mol);
            int count = 0;
            foreach (var b in mol.Bonds)
            {
                if (b.IsAromatic && b.Order.Equals(BondOrder.Double))
                {
                    count++;
                }
            }
            Assert.AreEqual(5, count);
        }
    }
}
