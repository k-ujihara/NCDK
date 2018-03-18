/* Copyright (C) 2005-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System.Collections.Generic;

namespace NCDK.Templates
{
    // @cdk.module test-pdb
    [TestClass()]
    public class AminoAcidsTest : CDKTestCase
    {
        [TestMethod()]
        public void TestCreateBondMatrix()
        {
            int[][] bonds = AminoAcids.CreateAABondInfo();
            Assert.IsNotNull(bonds);
        }

        [TestMethod()]
        public void TestCreateAAs()
        {
            IAminoAcid[] aas = AminoAcids.CreateAAs();
            Assert.IsNotNull(aas);
            Assert.AreEqual(20, aas.Length);
            for (int i = 0; i < 20; i++)
            {
                Assert.IsNotNull(aas[i]);
                Assert.IsFalse(0 == aas[i].Atoms.Count);
                Assert.IsFalse(0 == aas[i].Bonds.Count);
                Assert.IsNotNull(aas[i].MonomerName);
                Assert.IsNotNull(aas[i].GetProperty<string>(AminoAcids.RESIDUE_NAME_SHORT));
                Assert.IsNotNull(aas[i].GetProperty<string>(AminoAcids.RESIDUE_NAME));
            }
        }

        [TestMethod()]
        public void TestGetHashMapBySingleCharCode()
        {
            IDictionary<string, IAminoAcid> map = AminoAcids.GetHashMapBySingleCharCode();
            Assert.IsNotNull(map);
            Assert.AreEqual(20, map.Count);

            string[] aas = { "G", "A", "V", "L" };
            foreach (var aa1 in aas)
            {
                AminoAcid aa = (AminoAcid)map[aa1];
                Assert.IsNotNull(aa, "Did not find AA for: " + aa1);
            }
        }

        [TestMethod()]
        public void TestGetHashMapByThreeLetterCode()
        {
            IDictionary<string, IAminoAcid> map = AminoAcids.GetHashMapByThreeLetterCode();
            Assert.IsNotNull(map);
            Assert.AreEqual(20, map.Count);

            string[] aas = { "GLY", "ALA" };
            foreach (var aa1 in aas)
            {
                AminoAcid aa = (AminoAcid)map[aa1];
                Assert.IsNotNull(aa, "Did not find AA for: " + aa1);
            }
        }
    }
}
