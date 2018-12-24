/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.IO;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.Graphs
{
    /// <summary>
    ///  Checks the functionality of the ConnectivityChecker
    /// </summary>
    // @cdk.module test-standard
    // @author     steinbeck
    // @cdk.created    2001-07-24
    [TestClass()]
    public class ConnectivityCheckerTest : CDKTestCase
    {
        IChemObjectBuilder builder = CDK.Builder;

        public ConnectivityCheckerTest()
            : base()
        { }

        /// <summary>
        /// This test tests the function of the PartitionIntoMolecule() method.
        /// </summary>
        [TestMethod()]
        public void TestPartitionIntoMolecules_IAtomContainer()
        {
            //Debug.WriteLine(atomCon);
            var atomCon = builder.NewAtomContainer();
            atomCon.Add(TestMoleculeFactory.Make4x3CondensedRings());
            atomCon.Add(TestMoleculeFactory.MakeAlphaPinene());
            atomCon.Add(TestMoleculeFactory.MakeSpiroRings());
            var moleculeSet = ConnectivityChecker.PartitionIntoMolecules(atomCon).ToReadOnlyList();
            Assert.IsNotNull(moleculeSet);
            Assert.AreEqual(3, moleculeSet.Count);
        }

        /// <summary>
        /// Test for SF bug #903551
        /// </summary>
        [TestMethod()]
        public void TestPartitionIntoMoleculesKeepsAtomIDs()
        {
            var atomCon = builder.NewAtomContainer();
            var atom1 = builder.NewAtom("C");
            atom1.Id = "atom1";
            var atom2 = builder.NewAtom("C");
            atom2.Id = "atom2";
            atomCon.Atoms.Add(atom1);
            atomCon.Atoms.Add(atom2);
            var moleculeSet = ConnectivityChecker.PartitionIntoMolecules(atomCon).ToReadOnlyList();
            Assert.IsNotNull(moleculeSet);
            Assert.AreEqual(2, moleculeSet.Count);
            IAtom copy1 = moleculeSet[0].Atoms[0];
            IAtom copy2 = moleculeSet[1].Atoms[0];

            Assert.AreEqual(atom1.Id, copy1.Id);
            Assert.AreEqual(atom2.Id, copy2.Id);
        }

        /// <summary>
        /// This test tests the consistency between IsConnected() and
        /// PartitionIntoMolecules().
        /// </summary>
        [TestMethod()]
        public void TestPartitionIntoMolecules_IsConnected_Consistency()
        {
            //Debug.WriteLine(atomCon);
            var atomCon = builder.NewAtomContainer();
            atomCon.Add(TestMoleculeFactory.Make4x3CondensedRings());
            atomCon.Add(TestMoleculeFactory.MakeAlphaPinene());
            atomCon.Add(TestMoleculeFactory.MakeSpiroRings());
            var moleculeSet = ConnectivityChecker.PartitionIntoMolecules(atomCon).ToReadOnlyList();
            Assert.IsNotNull(moleculeSet);
            Assert.AreEqual(3, moleculeSet.Count);

            Assert.IsTrue(ConnectivityChecker.IsConnected(moleculeSet[0]));
            Assert.IsTrue(ConnectivityChecker.IsConnected(moleculeSet[1]));
            Assert.IsTrue(ConnectivityChecker.IsConnected(moleculeSet[2]));
        }

        /// <summary>
        /// This test makes sure that it is checked that the PartitionIntoMolecules()
        /// method keeps LonePairs and SingleElectrons with its associated atoms.
        /// </summary>
        [TestMethod()]
        public void TestDontDeleteSingleElectrons()
        {
            var atomCon = builder.NewAtomContainer();
            // make two molecules; one with an LonePair, the other with a SingleElectron
            var mol1 = builder.NewAtomContainer();
            var atom1 = builder.NewAtom("C");
            mol1.Atoms.Add(atom1);
            var lp1 = builder.NewLonePair(atom1);
            mol1.LonePairs.Add(lp1);
            // mol2
            var mol2 = builder.NewAtomContainer();
            var atom2 = builder.NewAtom("C");
            mol2.Atoms.Add(atom2);
            var se2 = builder.NewSingleElectron(atom2);
            mol2.SingleElectrons.Add(se2);

            atomCon.Add(mol1);
            atomCon.Add(mol2);

            // now partition
            var moleculeSet = ConnectivityChecker.PartitionIntoMolecules(atomCon).ToReadOnlyList();
            Assert.IsNotNull(moleculeSet);
            Assert.AreEqual(2, moleculeSet.Count);

            Assert.IsTrue(ConnectivityChecker.IsConnected(moleculeSet[0]));
            Assert.IsTrue(ConnectivityChecker.IsConnected(moleculeSet[1]));

            // make sure
            Assert.AreEqual(1, moleculeSet[0].Atoms.Count);
            Assert.AreEqual(1, moleculeSet[0].GetElectronContainers().Count());
            Assert.AreEqual(1, moleculeSet[1].Atoms.Count);
            Assert.AreEqual(1, moleculeSet[1].GetElectronContainers().Count());
            // we don't know which partition contains the LP and which the electron
            Assert.IsTrue(moleculeSet[0].GetConnectedSingleElectrons(
                    moleculeSet[0].Atoms[0]).Count() == 0
                    || moleculeSet[1].GetConnectedSingleElectrons(
                            moleculeSet[1].Atoms[0]).Count() == 0);
            Assert.IsTrue(moleculeSet[0].GetConnectedLonePairs(
                    moleculeSet[0].Atoms[0]).Count() == 0
                    || moleculeSet[1].GetConnectedLonePairs(
                            moleculeSet[1].Atoms[0]).Count() == 0);
        }

        /// <summary>
        /// This test tests the algorithm behind IsConnected().
        /// </summary>
        [TestMethod()]
        public void TestIsConnected_IAtomContainer()
        {
            IAtomContainer spiro = TestMoleculeFactory.MakeSpiroRings();
            Assert.IsTrue(ConnectivityChecker.IsConnected(spiro));
        }

        [TestMethod()]
        public void TestIsConnectedArtemisinin1()
        {
            var sp = CDK.SmilesParser;
            var container = sp.ParseSmiles("C1CN2CCN(CCCN(CCN(C1)Cc1ccccn1)CC2)C");
            Assert.IsTrue(ConnectivityChecker.IsConnected(container));
        }

        /// <summary>
        // @cdk.bug 2126904
        /// </summary>
        [TestMethod()]
        public void TestIsConnectedFromHINFile()
        {
            var filename = "NCDK.Data.HIN.connectivity1.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new HINReader(ins);
            var content = reader.Read(builder.NewChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content);
            IAtomContainer ac = cList.First();

            Assert.IsTrue(ConnectivityChecker.IsConnected(ac), "Molecule appears not to be connected");
        }

        /// <summary>
       // @cdk.bug 2126904
       /// </summary>
        [TestMethod()]
        public void TestIsConnectedFromSDFile()
        {
            var filename = "NCDK.Data.MDL.mdeotest.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            var content = reader.Read(builder.NewChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content);
            IAtomContainer ac = cList.First();

            Assert.IsTrue(ConnectivityChecker.IsConnected(ac), "Molecule appears not to be connected");
        }

        [TestMethod()]
        public void TestPartitionExtendedTetrahedral()
        {
            var smipar = CDK.SmilesParser;
            var container = smipar.ParseSmiles("CC=[C@]=CC.C");
            var containerSet = ConnectivityChecker.PartitionIntoMolecules(container).ToReadOnlyList();
            Assert.AreEqual(2, containerSet.Count);
            Assert.IsTrue(containerSet[0].StereoElements.GetEnumerator().MoveNext());
        }

        /// <summary>
        // @cdk.bug 2784209
        /// </summary>
        [TestMethod()]
        public void TestNoAtomsIsConnected()
        {
            var container = builder.NewAtomContainer();
            Assert.IsTrue(ConnectivityChecker.IsConnected(container), "Molecule appears not to be connected");
        }
    }
}
