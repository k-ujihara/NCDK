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
 *
 */

using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;
using NCDK.Smiles;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;


namespace NCDK.Graphs
{
    /// <summary>
    ///  Checks the functionality of the ConnectivityChecker
    ///
    // @cdk.module test-standard
    ///
    // @author     steinbeck
    // @cdk.created    2001-07-24
    /// </summary>
    [TestClass()]
    public class ConnectivityCheckerTest : CDKTestCase
    {

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
            AtomContainer atomCon = new AtomContainer();
            atomCon.Add(TestMoleculeFactory.Make4x3CondensedRings());
            atomCon.Add(TestMoleculeFactory.MakeAlphaPinene());
            atomCon.Add(TestMoleculeFactory.MakeSpiroRings());
            IAtomContainerSet<IAtomContainer> moleculeSet = ConnectivityChecker.PartitionIntoMolecules(atomCon);
            Assert.IsNotNull(moleculeSet);
            Assert.AreEqual(3, moleculeSet.Count);
        }

        /// <summary>
        /// Test for SF bug #903551
        /// </summary>
        [TestMethod()]
        public void TestPartitionIntoMoleculesKeepsAtomIDs()
        {
            AtomContainer atomCon = new AtomContainer();
            Atom atom1 = new Atom("C");
            atom1.Id = "atom1";
            Atom atom2 = new Atom("C");
            atom2.Id = "atom2";
            atomCon.Atoms.Add(atom1);
            atomCon.Atoms.Add(atom2);
            IAtomContainerSet<IAtomContainer> moleculeSet = ConnectivityChecker.PartitionIntoMolecules(atomCon);
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
            AtomContainer atomCon = new AtomContainer();
            atomCon.Add(TestMoleculeFactory.Make4x3CondensedRings());
            atomCon.Add(TestMoleculeFactory.MakeAlphaPinene());
            atomCon.Add(TestMoleculeFactory.MakeSpiroRings());
            IAtomContainerSet<IAtomContainer> moleculeSet = ConnectivityChecker.PartitionIntoMolecules(atomCon);
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
            AtomContainer atomCon = new AtomContainer();
            // make two molecules; one with an LonePair, the other with a SingleElectron
            IAtomContainer mol1 = new AtomContainer();
            Atom atom1 = new Atom("C");
            mol1.Atoms.Add(atom1);
            LonePair lp1 = new LonePair(atom1);
            mol1.LonePairs.Add(lp1);
            // mol2
            IAtomContainer mol2 = new AtomContainer();
            Atom atom2 = new Atom("C");
            mol2.Atoms.Add(atom2);
            SingleElectron se2 = new SingleElectron(atom2);
            mol2.SingleElectrons.Add(se2);

            atomCon.Add(mol1);
            atomCon.Add(mol2);

            // now partition
            IAtomContainerSet<IAtomContainer> moleculeSet = ConnectivityChecker.PartitionIntoMolecules(atomCon);
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
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer container = sp.ParseSmiles("C1CN2CCN(CCCN(CCN(C1)Cc1ccccn1)CC2)C");
            Assert.IsTrue(ConnectivityChecker.IsConnected(container));
        }

        /// <summary>
        // @cdk.bug 2126904
        /// </summary>
        [TestMethod()]
        public void TestIsConnectedFromHINFile()
        {
            string filename = "NCDK.Data.HIN.connectivity1.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
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
            string filename = "NCDK.Data.MDL.mdeotest.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content);
            IAtomContainer ac = cList.First();

            Assert.IsTrue(ConnectivityChecker.IsConnected(ac), "Molecule appears not to be connected");
        }

        [TestMethod()]
        public void TestPartitionExtendedTetrahedral()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer container = smipar.ParseSmiles("CC=[C@]=CC.C");
            IAtomContainerSet<IAtomContainer> containerSet = ConnectivityChecker.PartitionIntoMolecules(container);
            Assert.AreEqual(2, containerSet.Count);
            Assert.IsTrue(containerSet[0].StereoElements.GetEnumerator().MoveNext());
        }

        /// <summary>
        // @cdk.bug 2784209
        /// </summary>
        [TestMethod()]
        public void TestNoAtomsIsConnected()
        {
            IAtomContainer container = new AtomContainer();
            Assert.IsTrue(ConnectivityChecker.IsConnected(container), "Molecule appears not to be connected");
        }

    }
}
