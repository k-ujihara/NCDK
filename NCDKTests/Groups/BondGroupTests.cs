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
    public class BondGroupTests : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        public static IAtomContainer GetMol(string smiles)
        {
            var parser = new SmilesParser(ChemObjectBuilder.Instance);
            parser.Kekulise(false);
            return parser.ParseSmiles(smiles);
        }

        public static void Test(IAtomContainer mol, int expected)
        {
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            PermutationGroup group = refiner.GetAutomorphismGroup(mol);
            Assert.AreEqual(expected, group.Order());
        }

        [TestMethod()]
        public void CycloButane()
        {
            Test(AtomContainerPrinter.FromString("C0C1C2C3 0:1(1),1:2(1),2:3(1),0:3(1)", builder), 8);
        }

        [TestMethod()]
        public void CycloButadiene()
        {
            Test(AtomContainerPrinter.FromString("C0C1C2C3 0:1(2),1:2(1),2:3(2),0:3(1)", builder), 4);
        }

        [TestMethod()]
        public void CycloPentadiene()
        {
            Test(GetMol("C1=CCC=C1"), 2);
        }

        [TestMethod()]
        public void NapthaleneA()
        {
            Test(AtomContainerPrinter.FromString("C0C1C2C3C4C5C6C7C8C9 0:1(2),1:2(1),2:3(2),3:4(1),4:5(2),"
                    + "5:6(1),6:7(2),7:8(1),3:8(1),8:9(2),0:9(1)", builder), 2);
        }

        [TestMethod()]
        public void NapthaleneB()
        {
            Test(AtomContainerPrinter.FromString("C0C1C2C3C4C5C6C7C8C9 0:1(1),1:2(2),2:3(1),3:4(1),4:5(2),"
                    + "5:6(1),6:7(2),7:8(1),3:8(2),8:9(1),0:9(2)", Silent.ChemObjectBuilder.Instance), 4);
        }
    }
}
