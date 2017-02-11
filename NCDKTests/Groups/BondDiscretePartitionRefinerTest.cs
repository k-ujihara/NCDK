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
using NCDK.Common.Base;
using NCDK.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Templates;
using NCDK.Tools.Manipulator;

namespace NCDK.Groups
{
    // @author maclean
    // @cdk.module test-group
    [TestClass()]
    public class BondDiscretePartitionRefinerTest : CDKTestCase
    {
        public static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void DefaultConstructorTest()
        {
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            Assert.IsNotNull(refiner);
        }

        [TestMethod()]
        public void AdvancedConstructorTest()
        {
            bool ignoreBondOrder = true;
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner(ignoreBondOrder);
            Assert.IsNotNull(refiner);
        }

        [TestMethod()]
        public void ResetTest()
        {
            string acpString1 = "C0C1C2 0:1(1),1:2(1)";
            IAtomContainer ac1 = AtomContainerPrinter.FromString(acpString1, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            refiner.Refine(ac1);
            Assert.AreEqual(refiner.GetConnectivity(0, 1), 1);
            Assert.AreEqual(refiner.GetVertexCount(), 2);

            refiner.Reset();

            string acpString2 = "C0C1C2 0:1(1),0:2(1),1:2(1)";
            IAtomContainer ac2 = AtomContainerPrinter.FromString(acpString2, builder);
            refiner.Refine(ac2);
            Assert.AreEqual(refiner.GetConnectivity(0, 2), 1);
            Assert.AreEqual(refiner.GetVertexCount(), 3);
        }

        [TestMethod()]
        public void GetBondPartitionTest()
        {
            string acpString = "C0C1C2C3O4 0:1(2),0:4(1),1:2(1),2:3(2),3:4(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            Partition bondPartition = refiner.GetBondPartition(ac);
            Partition expected = Partition.FromString("0,3|1,4|2");
            Assert.AreEqual(expected, bondPartition);
        }

        [TestMethod()]
        public void Refine_StartingPartitionTest()
        {
            Partition partition = Partition.FromString("0,1|2,3");
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            refiner.Refine(ac, partition);
            PermutationGroup autG = refiner.GetAutomorphismGroup();
            Assert.AreEqual(2, autG.Order());
        }

        [TestMethod()]
        public void Refine_IgnoreBondOrderTest()
        {
            string acpString = "C0C1C2C3 0:1(2),0:3(1),1:2(1),2:3(2)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            bool ignoreBondOrder = true;
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner(ignoreBondOrder);
            refiner.Refine(ac);
            PermutationGroup autG = refiner.GetAutomorphismGroup();
            Assert.AreEqual(8, autG.Order());
        }

        [TestMethod()]
        public void RefineTest()
        {
            string acpString = "C0C1O2O3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            refiner.Refine(ac);
            PermutationGroup autG = refiner.GetAutomorphismGroup();
            Assert.AreEqual(2, autG.Order());
        }

        [TestMethod()]
        public void IsCanonical_TrueTest()
        {
            string acpString = "C0C1C2O3 0:1(2),0:2(1),1:3(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            Assert.IsTrue(refiner.IsCanonical(ac));
        }

        [TestMethod()]
        public void IsCanonical_FalseTest()
        {
            string acpString = "C0C1C2O3 0:1(2),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            Assert.IsFalse(refiner.IsCanonical(ac));
        }

        [TestMethod()]
        public void GetAutomorphismGroupTest()
        {
            string acpString = "C0C1C2O3 0:1(2),0:2(1),1:3(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
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
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
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
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            PermutationGroup autG = refiner.GetAutomorphismGroup(ac, partition);
            Assert.AreEqual(2, autG.Order());
        }

        [TestMethod()]
        public void GetVertexCountTest()
        {
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            refiner.Refine(ac);
            Assert.AreEqual(ac.Atoms.Count, refiner.GetVertexCount());
        }

        [TestMethod()]
        public void GetConnectivityTest()
        {
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            refiner.Refine(ac);
            Assert.AreEqual(1, refiner.GetConnectivity(0, 1));
        }

        [TestMethod()]
        public void GetConnectedIndicesTest()
        {
            string acpString = "C0C1C2C3 0:1(1),0:3(1),1:2(1),2:3(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            refiner.Refine(ac);
            int[] expected = new int[] { 0, 3 };
            int[] observed = refiner.GetConnectedIndices(1);
            Assert.IsTrue(Compares.AreDeepEqual(expected, observed),
                $"Expected : {Arrays.ToJavaString(expected)} but was {Arrays.ToJavaString(observed)}");
        }

        [TestMethod()]
        public void GetAutomorphismPartitionTest()
        {
            string acpString = "C0C1C2C3C4C5C6C7C8C9 0:1(2),1:2(1),2:3(2),3:4(1),"
                    + "4:5(2),5:6(1),6:7(2),7:8(1),8:9(2),5:9(1),0:9(1)";
            IAtomContainer ac = AtomContainerPrinter.FromString(acpString, builder);
            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            Partition autP = refiner.GetAutomorphismPartition(ac);
            Partition expected = Partition.FromString("0|1|2|3|4|5|6|7|8|9|10");
            Assert.AreEqual(expected, autP);
        }

        // NOTE : the following tests are from bug 1250 by Luis F. de Figueiredo
        // and mostly test for aromatic bonds

        [TestMethod()]
        public void TestAzulene()
        {

            IAtomContainer mol = TestMoleculeFactory.MakeAzulene();
            Assert.IsNotNull(mol, "Created molecule was null");
            AtomContainerPrinter.Print(mol);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            refiner.Refine(mol);
            Partition autP = refiner.GetAutomorphismPartition();

            Assert.AreEqual(6, autP.Count, "Wrong number of equivalent classes");
            Partition expected = Partition.FromString("0,3|1,2|4,10|5,8|6,7|9");
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

            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            refiner.Refine(mol);
            Partition autP = refiner.GetAutomorphismPartition();

            Assert.AreEqual(3, autP.Count, "Wrong number of equivalent classes");
            Partition expected = Partition.FromString("0,3|1,2|4,5");
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

            BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
            refiner.Refine(mol);
            Partition autP = refiner.GetAutomorphismPartition();

            Assert.AreEqual(4, autP.Count, "Wrong number of equivalent classes");
            Partition expected = Partition.FromString("0,5,7,12|1,4,8,11|2,3,9,10|6");
            Assert.AreEqual(expected, autP, "Wrong class assignment");
        }
    }
}
