/*
 * Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.IO;
using NCDK.Templates;
using NCDK.Tools.Manipulator;

namespace NCDK.Graphs.Invariant
{
    /// <summary>
    /// Checks the functionality of the TopologicalEquivalentClass.
    /// </summary>
    // @author      Junfeng Hao
    // @author      Luis F. de Figueiredo
    // @cdk.created 2003-09-26
    // @cdk.module test-extra
    [TestClass()]
    public class EquivalentClassPartitionerTest
        : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestEquivalent()
        {
            var C40C3V = builder.NewAtomContainer();
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 1
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 2
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 3
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 4
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 5
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 6
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 7
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 8
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 9
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 10
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 11
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 12
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 13
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 14
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 15
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 16
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 17
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 18
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 19
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 20
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 21
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 22
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 23
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 24
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 25
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 26
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 27
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 28
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 29
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 30
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 31
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 32
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 33
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 34
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 35
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 36
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 37
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 38
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 39
            C40C3V.Atoms.Add(builder.NewAtom("C")); // 40

            C40C3V.AddBond(C40C3V.Atoms[0], C40C3V.Atoms[1], BondOrder.Single); // 1
            C40C3V.AddBond(C40C3V.Atoms[0], C40C3V.Atoms[5], BondOrder.Single); // 2
            C40C3V.AddBond(C40C3V.Atoms[0], C40C3V.Atoms[8], BondOrder.Single); // 3
            C40C3V.AddBond(C40C3V.Atoms[1], C40C3V.Atoms[2], BondOrder.Single); // 4
            C40C3V.AddBond(C40C3V.Atoms[1], C40C3V.Atoms[25], BondOrder.Single); // 5
            C40C3V.AddBond(C40C3V.Atoms[2], C40C3V.Atoms[3], BondOrder.Single); // 6
            C40C3V.AddBond(C40C3V.Atoms[2], C40C3V.Atoms[6], BondOrder.Single); // 7
            C40C3V.AddBond(C40C3V.Atoms[3], C40C3V.Atoms[4], BondOrder.Single); // 8
            C40C3V.AddBond(C40C3V.Atoms[3], C40C3V.Atoms[24], BondOrder.Single); // 9
            C40C3V.AddBond(C40C3V.Atoms[4], C40C3V.Atoms[7], BondOrder.Single); // 10
            C40C3V.AddBond(C40C3V.Atoms[4], C40C3V.Atoms[8], BondOrder.Single); // 11
            C40C3V.AddBond(C40C3V.Atoms[5], C40C3V.Atoms[21], BondOrder.Single); // 12
            C40C3V.AddBond(C40C3V.Atoms[5], C40C3V.Atoms[28], BondOrder.Single); // 13
            C40C3V.AddBond(C40C3V.Atoms[6], C40C3V.Atoms[22], BondOrder.Single); // 14
            C40C3V.AddBond(C40C3V.Atoms[6], C40C3V.Atoms[27], BondOrder.Single); // 15
            C40C3V.AddBond(C40C3V.Atoms[7], C40C3V.Atoms[20], BondOrder.Single); // 16
            C40C3V.AddBond(C40C3V.Atoms[7], C40C3V.Atoms[23], BondOrder.Single); // 17
            C40C3V.AddBond(C40C3V.Atoms[8], C40C3V.Atoms[26], BondOrder.Single); // 18
            C40C3V.AddBond(C40C3V.Atoms[9], C40C3V.Atoms[12], BondOrder.Single); // 19
            C40C3V.AddBond(C40C3V.Atoms[9], C40C3V.Atoms[37], BondOrder.Single); // 20
            C40C3V.AddBond(C40C3V.Atoms[9], C40C3V.Atoms[39], BondOrder.Single); // 21
            C40C3V.AddBond(C40C3V.Atoms[10], C40C3V.Atoms[14], BondOrder.Single); // 22
            C40C3V.AddBond(C40C3V.Atoms[10], C40C3V.Atoms[38], BondOrder.Single); // 23
            C40C3V.AddBond(C40C3V.Atoms[10], C40C3V.Atoms[39], BondOrder.Single); // 24
            C40C3V.AddBond(C40C3V.Atoms[11], C40C3V.Atoms[13], BondOrder.Single); // 25
            C40C3V.AddBond(C40C3V.Atoms[11], C40C3V.Atoms[36], BondOrder.Single); // 26
            C40C3V.AddBond(C40C3V.Atoms[11], C40C3V.Atoms[39], BondOrder.Single); // 27
            C40C3V.AddBond(C40C3V.Atoms[12], C40C3V.Atoms[35], BondOrder.Single); // 28
            C40C3V.AddBond(C40C3V.Atoms[12], C40C3V.Atoms[38], BondOrder.Single); // 29
            C40C3V.AddBond(C40C3V.Atoms[13], C40C3V.Atoms[34], BondOrder.Single); // 30
            C40C3V.AddBond(C40C3V.Atoms[13], C40C3V.Atoms[37], BondOrder.Single); // 31
            C40C3V.AddBond(C40C3V.Atoms[14], C40C3V.Atoms[33], BondOrder.Single); // 32
            C40C3V.AddBond(C40C3V.Atoms[14], C40C3V.Atoms[36], BondOrder.Single); // 33
            C40C3V.AddBond(C40C3V.Atoms[15], C40C3V.Atoms[29], BondOrder.Single); // 34
            C40C3V.AddBond(C40C3V.Atoms[15], C40C3V.Atoms[17], BondOrder.Single); // 35
            C40C3V.AddBond(C40C3V.Atoms[15], C40C3V.Atoms[37], BondOrder.Single); // 36
            C40C3V.AddBond(C40C3V.Atoms[16], C40C3V.Atoms[19], BondOrder.Single); // 37
            C40C3V.AddBond(C40C3V.Atoms[16], C40C3V.Atoms[30], BondOrder.Single); // 38
            C40C3V.AddBond(C40C3V.Atoms[16], C40C3V.Atoms[36], BondOrder.Single); // 39
            C40C3V.AddBond(C40C3V.Atoms[17], C40C3V.Atoms[20], BondOrder.Single); // 40
            C40C3V.AddBond(C40C3V.Atoms[17], C40C3V.Atoms[35], BondOrder.Single); // 41
            C40C3V.AddBond(C40C3V.Atoms[18], C40C3V.Atoms[22], BondOrder.Single); // 42
            C40C3V.AddBond(C40C3V.Atoms[18], C40C3V.Atoms[32], BondOrder.Single); // 43
            C40C3V.AddBond(C40C3V.Atoms[18], C40C3V.Atoms[33], BondOrder.Single); // 44
            C40C3V.AddBond(C40C3V.Atoms[19], C40C3V.Atoms[28], BondOrder.Single); // 45
            C40C3V.AddBond(C40C3V.Atoms[19], C40C3V.Atoms[34], BondOrder.Single); // 46
            C40C3V.AddBond(C40C3V.Atoms[20], C40C3V.Atoms[26], BondOrder.Single); // 47
            C40C3V.AddBond(C40C3V.Atoms[21], C40C3V.Atoms[26], BondOrder.Single); // 48
            C40C3V.AddBond(C40C3V.Atoms[21], C40C3V.Atoms[29], BondOrder.Single); // 49
            C40C3V.AddBond(C40C3V.Atoms[22], C40C3V.Atoms[24], BondOrder.Single); // 50
            C40C3V.AddBond(C40C3V.Atoms[23], C40C3V.Atoms[24], BondOrder.Single); // 51
            C40C3V.AddBond(C40C3V.Atoms[23], C40C3V.Atoms[31], BondOrder.Single); // 52
            C40C3V.AddBond(C40C3V.Atoms[25], C40C3V.Atoms[27], BondOrder.Single); // 53
            C40C3V.AddBond(C40C3V.Atoms[25], C40C3V.Atoms[28], BondOrder.Single); // 54
            C40C3V.AddBond(C40C3V.Atoms[27], C40C3V.Atoms[30], BondOrder.Single); // 55
            C40C3V.AddBond(C40C3V.Atoms[29], C40C3V.Atoms[34], BondOrder.Single); // 56
            C40C3V.AddBond(C40C3V.Atoms[30], C40C3V.Atoms[33], BondOrder.Single); // 57
            C40C3V.AddBond(C40C3V.Atoms[31], C40C3V.Atoms[32], BondOrder.Single); // 58
            C40C3V.AddBond(C40C3V.Atoms[31], C40C3V.Atoms[35], BondOrder.Single); // 59
            C40C3V.AddBond(C40C3V.Atoms[32], C40C3V.Atoms[38], BondOrder.Single); // 60
            var it = new EquivalentClassPartitioner(C40C3V);
            var equivalentClass = it.GetTopoEquivClassbyHuXu(C40C3V);
            var arrEquivalent = new char[39];
            for (int i = 1; i < equivalentClass.Length - 1; i++)
                arrEquivalent[i - 1] = equivalentClass[i].ToString()[0];
            string strEquivalent = new string(arrEquivalent);
            Assert.IsNotNull(equivalentClass);
            Assert.IsTrue(equivalentClass[0] == 10);//number of Class
            Assert.IsTrue(equivalentClass[40] == 10);
            Assert.AreEqual("111112221333444556667878222879995555444", strEquivalent);
        }

        [TestMethod()]
        public void TestFullereneC24D6D()
        {
            var C24D6D = builder.NewAtomContainer();
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 1
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 2
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 3
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 4
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 5
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 6
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 7
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 8
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 9
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 10
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 11
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 12
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 13
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 14
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 15
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 16
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 17
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 18
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 19
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 20
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 21
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 22
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 23
            C24D6D.Atoms.Add(builder.NewAtom("C")); // 24

            C24D6D.AddBond(C24D6D.Atoms[0], C24D6D.Atoms[1], BondOrder.Single); // 1
            C24D6D.AddBond(C24D6D.Atoms[0], C24D6D.Atoms[5], BondOrder.Single); // 2
            C24D6D.AddBond(C24D6D.Atoms[0], C24D6D.Atoms[11], BondOrder.Single); // 3
            C24D6D.AddBond(C24D6D.Atoms[1], C24D6D.Atoms[2], BondOrder.Single); // 4
            C24D6D.AddBond(C24D6D.Atoms[1], C24D6D.Atoms[10], BondOrder.Single); // 5
            C24D6D.AddBond(C24D6D.Atoms[2], C24D6D.Atoms[3], BondOrder.Single); // 6
            C24D6D.AddBond(C24D6D.Atoms[2], C24D6D.Atoms[9], BondOrder.Single); // 7
            C24D6D.AddBond(C24D6D.Atoms[3], C24D6D.Atoms[4], BondOrder.Single); // 8
            C24D6D.AddBond(C24D6D.Atoms[3], C24D6D.Atoms[8], BondOrder.Single); // 9
            C24D6D.AddBond(C24D6D.Atoms[4], C24D6D.Atoms[5], BondOrder.Single); // 10
            C24D6D.AddBond(C24D6D.Atoms[4], C24D6D.Atoms[7], BondOrder.Single); // 11
            C24D6D.AddBond(C24D6D.Atoms[5], C24D6D.Atoms[6], BondOrder.Single); // 12
            C24D6D.AddBond(C24D6D.Atoms[6], C24D6D.Atoms[16], BondOrder.Single); // 13
            C24D6D.AddBond(C24D6D.Atoms[6], C24D6D.Atoms[17], BondOrder.Single); // 14
            C24D6D.AddBond(C24D6D.Atoms[7], C24D6D.Atoms[15], BondOrder.Single); // 15
            C24D6D.AddBond(C24D6D.Atoms[7], C24D6D.Atoms[16], BondOrder.Single); // 16
            C24D6D.AddBond(C24D6D.Atoms[8], C24D6D.Atoms[14], BondOrder.Single); // 17
            C24D6D.AddBond(C24D6D.Atoms[8], C24D6D.Atoms[15], BondOrder.Single); // 18
            C24D6D.AddBond(C24D6D.Atoms[9], C24D6D.Atoms[13], BondOrder.Single); // 19
            C24D6D.AddBond(C24D6D.Atoms[9], C24D6D.Atoms[14], BondOrder.Single); // 20
            C24D6D.AddBond(C24D6D.Atoms[10], C24D6D.Atoms[12], BondOrder.Single); // 21
            C24D6D.AddBond(C24D6D.Atoms[10], C24D6D.Atoms[13], BondOrder.Single); // 22
            C24D6D.AddBond(C24D6D.Atoms[11], C24D6D.Atoms[12], BondOrder.Single); // 23
            C24D6D.AddBond(C24D6D.Atoms[11], C24D6D.Atoms[17], BondOrder.Single); // 24
            C24D6D.AddBond(C24D6D.Atoms[12], C24D6D.Atoms[19], BondOrder.Single); // 25
            C24D6D.AddBond(C24D6D.Atoms[13], C24D6D.Atoms[20], BondOrder.Single); // 26
            C24D6D.AddBond(C24D6D.Atoms[14], C24D6D.Atoms[21], BondOrder.Single); // 27
            C24D6D.AddBond(C24D6D.Atoms[15], C24D6D.Atoms[22], BondOrder.Single); // 28
            C24D6D.AddBond(C24D6D.Atoms[16], C24D6D.Atoms[23], BondOrder.Single); // 29
            C24D6D.AddBond(C24D6D.Atoms[17], C24D6D.Atoms[18], BondOrder.Single); // 30
            C24D6D.AddBond(C24D6D.Atoms[18], C24D6D.Atoms[19], BondOrder.Single); // 31
            C24D6D.AddBond(C24D6D.Atoms[18], C24D6D.Atoms[23], BondOrder.Single); // 32
            C24D6D.AddBond(C24D6D.Atoms[19], C24D6D.Atoms[20], BondOrder.Single); // 33
            C24D6D.AddBond(C24D6D.Atoms[20], C24D6D.Atoms[21], BondOrder.Single); // 34
            C24D6D.AddBond(C24D6D.Atoms[21], C24D6D.Atoms[22], BondOrder.Single); // 35
            C24D6D.AddBond(C24D6D.Atoms[22], C24D6D.Atoms[23], BondOrder.Single); // 36

            var it = new EquivalentClassPartitioner(C24D6D);
            var equivalentClass = it.GetTopoEquivClassbyHuXu(C24D6D);
            var arrEquivalent = new char[24];
            for (int i = 1; i < equivalentClass.Length; i++)
                arrEquivalent[i - 1] = equivalentClass[i].ToString()[0];
            string strEquivalent = new string(arrEquivalent);
            Assert.IsNotNull(equivalentClass);
            Assert.IsTrue(equivalentClass[0] == 2);//number of Class
            Assert.AreEqual("111111222222222222111111", strEquivalent);
        }

        // @cdk.bug 3513954
        [TestMethod()]
        public void TestPseudoAtoms()
        {
            var filename = "NCDK.Data.MDL.pseudoatoms.sdf";

            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            var mol = builder.NewAtomContainer();
            mol = reader.Read(mol);
            Assert.IsNotNull(mol);

            // check that there are some pseudo-atoms
            bool hasPseudo = false;
            foreach (var atom in mol.Atoms)
            {
                if (atom is IPseudoAtom) hasPseudo = true;
            }
            Assert.IsTrue(hasPseudo, "The molecule should have one or more pseudo atoms");

            EquivalentClassPartitioner partitioner = new EquivalentClassPartitioner(mol);
            Assert.IsNotNull(partitioner);

            int[] classes = partitioner.GetTopoEquivClassbyHuXu(mol);
        }

        /// <summary>
        /// Test if aromatic bonds are being considered as such.
        /// Azulene has an aromatic outer ring and if bonds are considered only as a sequence of single and double bonds
        /// then the atoms closing the rings will be assigned to different classes (and all other atoms as well) because
        /// there will be a different number of single and double bonds on opposite sides of the symmetry axis.
        /// </summary>
        // @cdk.bug 3562476
        [TestMethod()]
        public void TestAromaticSystem()
        {
            var mol = TestMoleculeFactory.MakeAzulene();
            Assert.IsNotNull(mol, "Created molecule was null");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            var it = new EquivalentClassPartitioner(mol);
            var equivalentClass = it.GetTopoEquivClassbyHuXu(mol);
            var arrEquivalent = new char[mol.Atoms.Count];
            for (int i = 1; i < equivalentClass.Length; i++)
                arrEquivalent[i - 1] = equivalentClass[i].ToString()[0];
            string strEquivalent = new string(arrEquivalent);

            Assert.IsNotNull(equivalentClass, "Equivalent class was null");
            Assert.AreEqual(mol.Atoms.Count + 1, equivalentClass.Length, "Unexpected equivalent class length");
            Assert.AreEqual(6, equivalentClass[0], "Wrong number of equivalent classes");//number of Class
            Assert.AreEqual("1232145654", strEquivalent, "Wrong class assignment");
        }

        /// <summary>
        /// Test the equivalent classes method in alpha-pinene
        /// </summary>
        [TestMethod()]
        public void TestAlphaPinene()
        {
            var mol = TestMoleculeFactory.MakeAlphaPinene();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            Assert.IsNotNull(mol, "Created molecule was null");
            var it = new EquivalentClassPartitioner(mol);
            var equivalentClass = it.GetTopoEquivClassbyHuXu(mol);
            var arrEquivalent = new char[mol.Atoms.Count];
            for (int i = 1; i < equivalentClass.Length; i++)
                arrEquivalent[i - 1] = equivalentClass[i].ToString()[0];
            string strEquivalent = new string(arrEquivalent);
            Assert.IsNotNull(equivalentClass, "Equivalent class was null");
            Assert.AreEqual(9, equivalentClass[0], "Wrong number of equivalent classes");
            Assert.AreEqual("1234567899", strEquivalent, "Wrong class assignment");
        }

        /// <summary>
        /// Test the equivalent classes method in pyrimidine
        /// Tests if the position of the single and double bonds in an aromatic ring matter
        /// to assign a class.
        /// </summary>
        [TestMethod()]
        public void TestPyrimidine()
        {
            var mol = TestMoleculeFactory.MakePyrimidine();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            Assert.IsNotNull(mol, "Created molecule was null");
            var it = new EquivalentClassPartitioner(mol);
            var equivalentClass = it.GetTopoEquivClassbyHuXu(mol);
            var arrEquivalent = new char[mol.Atoms.Count];
            for (int i = 1; i < equivalentClass.Length; i++)
                arrEquivalent[i - 1] = equivalentClass[i].ToString()[0];
            string strEquivalent = new string(arrEquivalent);
            Assert.IsNotNull(equivalentClass, "Equivalent class was null");
            Assert.AreEqual(4, equivalentClass[0], "Wrong number of equivalent classes");
            Assert.AreEqual("123214", strEquivalent, "Wrong class assignment");
        }

        /// <summary>
        /// Test the equivalent classes method in biphenyl,
        /// a molecule with two aromatic systems. It has 2 symmetry axis.
        /// </summary>
        [TestMethod()]
        public void TestBiphenyl()
        {
            var mol = TestMoleculeFactory.MakeBiphenyl();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            Assert.IsNotNull(mol, "Created molecule was null");
            var it = new EquivalentClassPartitioner(mol);
            var equivalentClass = it.GetTopoEquivClassbyHuXu(mol);
            var arrEquivalent = new char[mol.Atoms.Count];
            for (int i = 1; i < equivalentClass.Length; i++)
                arrEquivalent[i - 1] = equivalentClass[i].ToString()[0];
            string strEquivalent = new string(arrEquivalent);
            Assert.IsNotNull(equivalentClass, "Equivalent class was null");
            Assert.AreEqual(4, equivalentClass[0], "Wrong number of equivalent classes");
            Assert.AreEqual("123432123432", strEquivalent, "Wrong class assignment");
        }

        /// <summary>
        /// Test the equivalent classes method in imidazole,
        /// an aromatic molecule with a proton that can be exchanged between two aromatic nitrogens.
        /// The method should have failed because only one tautomer is considered,
        /// but there is no priority class for nodes of type ArNH to distinguish the nitrogens.
        /// </summary>
        [TestMethod()]
        public void TestImidazole()
        {
            var mol = TestMoleculeFactory.MakeImidazole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            Assert.IsNotNull(mol, "Created molecule was null");
            var it = new EquivalentClassPartitioner(mol);
            var equivalentClass = it.GetTopoEquivClassbyHuXu(mol);
            var arrEquivalent = new char[mol.Atoms.Count];
            for (int i = 1; i < equivalentClass.Length; i++)
                arrEquivalent[i - 1] = equivalentClass[i].ToString()[0];
            string strEquivalent = new string(arrEquivalent);
            Assert.IsNotNull(equivalentClass, "Equivalent class was null");
            Assert.AreEqual(3, equivalentClass[0], "Wrong number of equivalent classes");
            Assert.AreEqual("12321", strEquivalent, "Wrong class assignment");
        }
    }
}
