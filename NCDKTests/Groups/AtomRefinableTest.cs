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
    /// <summary>
    /// Test the refinable wrapper around atom containers.
    /// </summary>
    // @author maclean
    // @cdk.module group 
    public class AtomRefinableTest
    {
        public static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void GetVertexCount()
        {
            IAtomContainer ac = MakeAtomContainer("CCCC");
            AtomRefinable refinable = new AtomRefinable(ac);
            Assert.AreEqual(ac.Atoms.Count, refinable.GetVertexCount());
        }

        [TestMethod()]
        public void GetConnectivity()
        {
            string acpString = "C0C1C2C3 0:1(1),1:2(2),2:3(3)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomRefinable refinable = new AtomRefinable(ac);
            Assert.AreEqual(1, refinable.GetConnectivity(0, 1));
            Assert.AreEqual(2, refinable.GetConnectivity(1, 2));
            Assert.AreEqual(3, refinable.GetConnectivity(2, 3));
        }

        [TestMethod()]
        public void NeighboursInBlockForSingleBonds()
        {
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomRefinable refinable = new AtomRefinable(ac);

            Invariant invariant = refinable.NeighboursInBlock(Set(0, 2), 1);
            Assert.IsTrue(invariant is IntegerInvariant);
            Assert.AreEqual(new IntegerInvariant(2), invariant);
        }

        [TestMethod()]
        public void NeighboursInBlockForMultipleBonds()
        {
            string acpString = "C0C1C2C3C4 0:1(1),0:2(2),0:3(1),1:4(1),2:4(1),3:4(2)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomRefinable refinable = new AtomRefinable(ac);

            Invariant invariant = refinable.NeighboursInBlock(Set(1, 2), 0);
            Assert.IsTrue(invariant is IntegerListInvariant);
            Assert.AreEqual(new IntegerListInvariant(new int[] { 1, 1 }), invariant);
        }

        [TestMethod()]
        public void NeighboursInBlockForMultipleBondsIgnoringBondOrders()
        {
            string acpString = "C0C1C2C3C4 0:1(1),0:2(2),0:3(1),1:4(1),2:4(1),3:4(2)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomRefinable refinable = new AtomRefinable(ac, false, true);

            Invariant invariant = refinable.NeighboursInBlock(Set(1, 2), 0);
            Assert.IsTrue(invariant is IntegerInvariant);
            Assert.AreEqual(new IntegerInvariant(2), invariant);
        }

        private ISet<int> Set(params int[] elements)
        {
            ISet<int> block = new HashSet<int>();
            foreach (int element in elements)
            {
                block.Add(element);
            }
            return block;
        }

        [TestMethod()]
        public void GetElementPartitionTest()
        {
            string acpString = "C0N1C2P3C4N5";
            Partition expected = Partition.FromString("0,2,4|1,5|3");

            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomRefinable refinable = new AtomRefinable(ac);

            Partition elPartition = refinable.GetInitialPartition();
            Assert.AreEqual(expected, elPartition);
        }

        [TestMethod()]
        public void OddEvenElementPartitionTest()
        {
            IAtomContainer ac = MakeAtomContainer("CNCNCN");
            Partition expected = Partition.FromString("0,2,4|1,3,5");

            AtomRefinable refinable = new AtomRefinable(ac);

            Partition elPartition = refinable.GetInitialPartition();
            Assert.AreEqual(expected, elPartition);
        }

        [TestMethod()]
        public void OrderedElementPartitionTest()
        {
            IAtomContainer ac = MakeAtomContainer("CCCCNNNNOOOO");
            Partition expected = Partition.FromString("0,1,2,3|4,5,6,7|8,9,10,11");

            AtomRefinable refinable = new AtomRefinable(ac);

            Partition elPartition = refinable.GetInitialPartition();
            Assert.AreEqual(expected, elPartition);
        }

        [TestMethod()]
        public void DisorderedElementPartitionTest()
        {
            IAtomContainer ac = MakeAtomContainer("NNNNCCCCOOOO");
            Partition expected = Partition.FromString("4,5,6,7|0,1,2,3|8,9,10,11");

            AtomRefinable refinable = new AtomRefinable(ac);

            Partition elPartition = refinable.GetInitialPartition();
            Assert.AreEqual(expected, elPartition);
        }

        private IAtomContainer MakeAtomContainer(string elements)
        {
            IAtomContainer ac = builder.CreateAtomContainer();
            for (int i = 0; i < elements.Length; i++)
            {
                string element = elements[i].ToString();
                ac.Atoms.Add(builder.CreateAtom(element));
            }
            return ac;
        }
    }
}
