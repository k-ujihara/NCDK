/* Copyright (C) 2017  Gilleain Torrance <gilleain.torrance@gmail.com>
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
using System.Collections.Generic;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module group 
    public class BondRefinableTest
    {
        public static IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void GetVertexCount()
        {
            string acpString = "C0C1C2C3 0:1(1),1:2(1),2:3(1)";
            BondRefinable bondRefinable = Refinable(acpString);
            Assert.AreEqual(3, bondRefinable.GetVertexCount());
        }

        [TestMethod()]
        public void GetConnectivity()
        {
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            BondRefinable bondRefinable = Refinable(acpString);
            Assert.AreEqual(1, bondRefinable.GetConnectivity(0, 1));
            Assert.AreEqual(1, bondRefinable.GetConnectivity(0, 2));
            Assert.AreEqual(1, bondRefinable.GetConnectivity(1, 3));
            Assert.AreEqual(1, bondRefinable.GetConnectivity(2, 3));
        }

        [TestMethod()]
        public void NeighboursInBlock()
        {
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            BondRefinable bondRefinable = Refinable(acpString);
            ISet<int> block = new HashSet<int>();
            block.Add(1);
            block.Add(3);
            Assert.AreEqual(new IntegerInvariant(1), bondRefinable.NeighboursInBlock(block, 0));
            Assert.AreEqual(new IntegerInvariant(1), bondRefinable.NeighboursInBlock(block, 2));
        }

        [TestMethod()]
        public void GetBondPartitionTest()
        {
            string acpString = "C0C1C2C3O4 0:1(2),0:4(1),1:2(1),2:3(2),3:4(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondRefinable refinable = new BondRefinable(ac);
            Partition bondPartition = refinable.GetInitialPartition();
            Partition expected = Partition.FromString("0,3|1,4|2");
            Assert.AreEqual(expected, bondPartition);
        }

        private BondRefinable Refinable(string acpString)
        {
            return new BondRefinable(AtomContainerPrinter.FromString(acpString, builder));
        }
    }
}
