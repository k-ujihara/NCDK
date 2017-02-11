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
using NCDK.Aromaticities;
using NCDK.Templates;
using NCDK.Tools.Manipulator;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module test-group
    [TestClass()]
    public class AtomDiscretePartitionRefinerTest : CDKTestCase
    {
        public static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void DefaultConstructorTest()
        {
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            Assert.IsNotNull(refiner);
        }

        [TestMethod()]
        public void AdvancedConstructorTest()
        {
            bool ignoreElements = true;
            bool ignoreBondOrder = true;
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner(ignoreElements, ignoreBondOrder);
            Assert.IsNotNull(refiner);
        }

        [TestMethod()]
        public void ReSetTest()
        {
            string acpString1 = "C0C1 0:1(1)";
            IAtomContainer ac1 = AtomContainerPrinter.FromString(acpString1, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            refiner.Refine(ac1);
            Assert.AreEqual(refiner.GetConnectivity(0, 1), 1);
            Assert.AreEqual(refiner.GetVertexCount(), 2);

            refiner.Reset();

            string acpString2 = "C0C1C2 0:1(2),1:2(1)";
            IAtomContainer ac2 = AtomContainerPrinter.FromString(acpString2, builder);
            refiner.Refine(ac2);
            Assert.AreEqual(refiner.GetConnectivity(0, 1), 2);
            Assert.AreEqual(refiner.GetVertexCount(), 3);
        }

        [TestMethod()]
        public void GetElementPartitionTest()
        {
            string acpString = "C0N1C2P3C4N5";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            Partition elPartition = refiner.GetElementPartition(ac);
            Partition expected = Partition.FromString("0,2,4|1,5|3");
            Assert.AreEqual(expected, elPartition);
        }

        [TestMethod()]
        public void Refine_StartingPartitionTest()
        {
            Partition partition = Partition.FromString("0,1|2,3");
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            refiner.Refine(ac, partition);
            PermutationGroup autG = refiner.GetAutomorphismGroup();
            Assert.AreEqual(2, autG.Order());
        }

        [TestMethod()]
        public void Refine_IgnoreElementsTest()
        {
            string acpString = "C0C1O2O3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            bool ignoreElements = true;
            bool ignoreBondOrder = false;
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner(ignoreElements, ignoreBondOrder);
            refiner.Refine(ac);
            PermutationGroup autG = refiner.GetAutomorphismGroup();
            Assert.AreEqual(8, autG.Order());
        }

        [TestMethod()]
        public void RefineTest()
        {
            string acpString = "C0C1O2O3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            refiner.Refine(ac);
            PermutationGroup autG = refiner.GetAutomorphismGroup();
            Assert.AreEqual(2, autG.Order());
        }

        [TestMethod()]
        public void IsCanonical_TrueTest()
        {
            string acpString = "C0C1C2O3 0:1(2),0:2(1),1:3(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            Assert.IsTrue(refiner.IsCanonical(ac));
        }

        [TestMethod()]
        public void IsCanonical_FalseTest()
        {
            string acpString = "C0C1C2O3 0:1(2),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            Assert.IsFalse(refiner.IsCanonical(ac));
        }

        [TestMethod()]
        public void GetAutomorphismGroupTest()
        {
            string acpString = "C0C1C2O3 0:1(2),0:2(1),1:3(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            PermutationGroup autG = refiner.GetAutomorphismGroup(ac);
            Assert.IsNotNull(autG);
            Assert.AreEqual(1, autG.Order());
        }

        [TestMethod()]
        public void GetAutomorphismGroup_StartingGroupTest()
        {
            string acpString = "C0C1C2C3 0:1(1),0:2(1),1:3(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            Permutation flip = new Permutation(1, 0, 3, 2);
            PermutationGroup autG = new PermutationGroup(4, new[] { flip });
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            refiner.GetAutomorphismGroup(ac, autG);
            Assert.IsNotNull(autG);
            Assert.AreEqual(8, autG.Order());
        }

        [TestMethod()]
        public void GetAutomorphismGroup_StartingPartitionTest()
        {
            Partition partition = Partition.FromString("0,1|2,3");
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            PermutationGroup autG = refiner.GetAutomorphismGroup(ac, partition);
            Assert.AreEqual(2, autG.Order());
        }

        [TestMethod()]
        public void GetVertexCountTest()
        {
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            refiner.Refine(ac);
            Assert.AreEqual(ac.Atoms.Count, refiner.GetVertexCount());
        }

        [TestMethod()]
        public void GetConnectivityTest()
        {
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(2),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            refiner.Refine(ac);
            IBond bond = ac.GetBond(ac.Atoms[1], ac.Atoms[2]);
            int orderN = bond.Order.Numeric;
            Assert.AreEqual(orderN, refiner.GetConnectivity(1, 2));
        }

        [TestMethod()]
        public void GetAutomorphismPartitionTest()
        {
            string acpString = "C0C1C2C3C4C5C6C7C8C9 0:1(2),1:2(1),2:3(2),3:4(1),"
                    + "4:5(2),5:6(1),6:7(2),7:8(1),8:9(2),5:9(1),0:9(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            Partition autP = refiner.GetAutomorphismPartition(ac);
            Partition expected = Partition.FromString("0|1|2|3|4|5|6|7|8|9");
            Assert.AreEqual(expected, autP);
        }

        // NOTE : the following tests are from bug 1250 by Luis F. de Figueiredo
        // and mostly test for aromatic bonds

        [TestMethod()]
        public void TestAzulene()
        {

            IAtomContainer mol = TestMoleculeFactory.MakeAzulene();
            Assert.IsNotNull(mol, "Created molecule was null");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            refiner.Refine(mol);
            Partition autP = refiner.GetAutomorphismPartition();

            Assert.AreEqual(6, autP.Count, "Wrong number of equivalent classes");
            Partition expected = Partition.FromString("0,4|1,3|2|5,9|6,8|7");
            Assert.AreEqual(expected, autP, "Wrong class assignment");
        }

        /**
         * Test the equivalent classes method in pyrimidine
         * Tests if the position of the single and double bonds in an aromatic ring matter
         * to assign a class.
         *
         * @throws Exception
         */
        [TestMethod()]
        public void TestPyrimidine()
        {
            IAtomContainer mol = TestMoleculeFactory.MakePyrimidine();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            Assert.IsNotNull(mol, "Created molecule was null");

            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            refiner.Refine(mol);
            Partition autP = refiner.GetAutomorphismPartition();

            Assert.AreEqual(4, autP.Count, "Wrong number of equivalent classes");
            Partition expected = Partition.FromString("0,4|1,3|2|5");
            Assert.AreEqual(expected, autP, "Wrong class assignment");
        }

        /**
         * Test the equivalent classes method in biphenyl,
         * a molecule with two aromatic systems. It has 2 symmetry axis.
         *
         * @throws Exception
         */
        [TestMethod()]
        public void TestBiphenyl()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeBiphenyl();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            Assert.IsNotNull(mol, "Created molecule was null");

            AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
            refiner.Refine(mol);
            Partition autP = refiner.GetAutomorphismPartition();

            Assert.AreEqual(4, autP.Count, "Wrong number of equivalent classes");
            Partition expected = Partition.FromString("0,6|1,5,7,11|2,4,8,10|3,9");
            Assert.AreEqual(expected, autP, "Wrong class assignment");
        }
    }
}
