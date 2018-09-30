/* Copyright (C) 2012  Gilleain Torrance <gilleain.torrance@gmail.com>
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
using NCDK.Silent;
using NCDK.Smiles;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module test-group
    [TestClass()]
    public class AtomGroupTests : CDKTestCase
    {
        public IAtomContainer GetMol(string smiles)
        {
            SmilesParser parser = new SmilesParser(ChemObjectBuilder.Instance);
            return parser.ParseSmiles(smiles);
        }

        public void Test(IAtomContainer mol, int expected)
        {
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            PermutationGroup group = refiner.GetAutomorphismGroup(mol);
            Assert.AreEqual(expected, group.Order());
        }

        [TestMethod()]
        public void CarbonSingleTree()
        {
            Test(GetMol("CC(C)C(C)C"), 8);
        }

        [TestMethod()]
        public void HetatmSingleTree()
        {
            Test(GetMol("CC(O)C(C)C"), 2);
        }

        [TestMethod()]
        public void CarbonMultipleTree()
        {
            Test(GetMol("CC(=C)C(C)C"), 2);
        }

        [TestMethod()]
        public void CarbonSingleCycle()
        {
            Test(GetMol("C1CCC1"), 8);
        }

        [TestMethod()]
        public void HetatmMultipleTree()
        {
            Test(GetMol("CC(=O)C(C)C"), 2);
        }

        [TestMethod()]
        public void HetatmSingleCycle()
        {
            Test(GetMol("C1COC1"), 2);
        }

        [TestMethod()]
        public void CarbonMultipleCycle()
        {
            Test(GetMol("C1=CC=C1"), 4);
        }

        [TestMethod()]
        public void HetatmMultipleCycle()
        {
            Test(GetMol("C1=OC=C1"), 1);
        }
    }
}
