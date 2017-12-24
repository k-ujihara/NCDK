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
using NCDK.Default;
using NCDK.IO;
using NCDK.Smiles;
using NCDK.Templates;

using System;

namespace NCDK.RingSearches
{
    // @cdk.module test-standard
    [TestClass()]
    public class AllRingsFinderTest : CDKTestCase
    {
        bool standAlone = false;

        public AllRingsFinderTest()
            : base()
        { }

        public void SetStandAlone(bool standAlone)
        {
            // not-used
        }

        [TestMethod()]
        public void TestAllRingsFinder()
        {
            AllRingsFinder arf = new AllRingsFinder();
            Assert.IsNotNull(arf);
        }

        [TestMethod()]
        public void TestFindAllRings_IAtomContainer()
        {
            IRingSet ringSet = null;
            AllRingsFinder arf = new AllRingsFinder();
            IAtomContainer molecule = TestMoleculeFactory.MakeEthylPropylPhenantren();
            //Display(molecule);

            ringSet = arf.FindAllRings(molecule);

            Assert.AreEqual(6, ringSet.Count);
        }

        // @cdk.bug 746067
        [TestMethod()]
        public void TestBondsWithinRing()
        {
            IRingSet ringSet = null;
            AllRingsFinder arf = new AllRingsFinder();
            IAtomContainer molecule = TestMoleculeFactory.MakeEthylPropylPhenantren();
            //Display(molecule);

            ringSet = arf.FindAllRings(molecule);
            for (int i = 0; i < ringSet.Count; i++)
            {
                Ring ring = (Ring)ringSet[i];
                for (int j = 0; j < ring.Bonds.Count; j++)
                {
                    IBond ec = ring.Bonds[j];

                    IAtom atom1 = ec.Begin;
                    IAtom atom2 = ec.End;
                    Assert.IsTrue(ring.Contains(atom1));
                    Assert.IsTrue(ring.Contains(atom2));
                }
            }
        }

        [TestMethod()]
        public void TestFindAllRings_IAtomContainer_bool()
        {
            AllRingsFinder arf = new AllRingsFinder();
            IAtomContainer molecule = TestMoleculeFactory.MakeEthylPropylPhenantren();
            arf.FindAllRings(molecule);
        }

        [TestMethod(), Ignore()] // timeout not longer used
        public void TestSetTimeout_long()
        {
            AllRingsFinder arf = new AllRingsFinder();
            arf.SetTimeout(1);
            IAtomContainer molecule = TestMoleculeFactory.MakeEthylPropylPhenantren();
            arf.FindAllRings(molecule);
        }

        [TestMethod(), Ignore()] // timeout not longer used
        public void TestCheckTimeout()
        {
            AllRingsFinder arf = new AllRingsFinder();
            arf.SetTimeout(3);
            arf.CheckTimeout();
        }

        [TestMethod(), Ignore()] // timeout not longer used
        public void TestGetTimeout()
        {
            AllRingsFinder arf = new AllRingsFinder();
            arf.SetTimeout(3);
            Assert.AreEqual(3, arf.TimeOut, 0.01);
        }

        [TestMethod()]
        public void TestPorphyrine()
        {
            IRingSet ringSet = null;
            AllRingsFinder arf = new AllRingsFinder();

            string filename = "NCDK.Data.MDL.porphyrin.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer molecule = model.MoleculeSet[0];

            ringSet = arf.FindAllRings(molecule);
            Assert.AreEqual(20, ringSet.Count);
        }

        [TestMethod()]
        //[Timeout(500)]
        [TestCategory("SlowTest")]
        public void TestBigRingSystem()
        {
            IRingSet ringSet = null;
            AllRingsFinder arf = AllRingsFinder.UsingThreshold(AllRingsFinder.Threshold.PubChem_994);

            string filename = "NCDK.Data.MDL.ring_03419.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer molecule = model.MoleculeSet[0];

            ringSet = arf.FindAllRings(molecule);
            // the 1976 value was empirically derived, and might not be accurate
            Assert.AreEqual(1976, ringSet.Count);
        }

        [TestMethod()]
        public void TestCholoylCoA()
        {
            IRingSet ringSet = null;
            AllRingsFinder arf = new AllRingsFinder();

            string filename = "NCDK.Data.MDL.choloylcoa.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer molecule = model.MoleculeSet[0];

            ringSet = arf.FindAllRings(molecule);
            Assert.AreEqual(14, ringSet.Count);
        }

        [TestMethod()]
        public void TestAzulene()
        {
            IRingSet ringSet = null;
            AllRingsFinder arf = new AllRingsFinder();

            string filename = "NCDK.Data.MDL.azulene.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer molecule = model.MoleculeSet[0];

            ringSet = arf.FindAllRings(molecule);
            Assert.AreEqual(3, ringSet.Count);
        }

        // @cdk.inchi InChI=1S/C90H74O28/c91-48-18-8-36(28-58(48)101)81-64(107)34-35-4-3-6-43(80(35)114-81)66-44-14-24-55(98)72(87(44)115-83(75(66)108)38-10-20-50(93)60(103)30-38)68-46-16-26-57(100)74(89(46)117-85(77(68)110)40-12-22-52(95)62(105)32-40)70-47-17-27-56(99)73(90(47)118-86(79(70)112)41-13-23-53(96)63(106)33-41)69-45-15-25-54(97)71(88(45)116-84(78(69)111)39-11-21-51(94)61(104)31-39)67-42-5-1-2-7-65(42)113-82(76(67)109)37-9-19-49(92)59(102)29-37/h1-33,64,66-70,75-79,81-86,91-112H,34H2
        [TestMethod()]
        public void TestBigMoleculeWithIsolatedRings()
        {
            IRingSet ringSet = null;
            AllRingsFinder arf = new AllRingsFinder();

            string filename = "NCDK.Data.CML.isolated_ringsystems.cml";
            var ins = ResourceLoader.GetAsStream(filename);

            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer mol = model.MoleculeSet[0];

            //Debug.WriteLine("Constructed Molecule");
            //Debug.WriteLine("Starting AllRingsFinder");
            ringSet = new AllRingsFinder().FindAllRings(mol);
            //Debug.WriteLine("Finished AllRingsFinder");
            Assert.AreEqual(24, ringSet.Count);
            //Display(mol);

            // check sizes of rings
            int[] ringSize = new int[mol.Atoms.Count];
            foreach (var ring in ringSet)
            {
                ringSize[ring.Atoms.Count]++;
            }

            Assert.AreEqual(18, ringSize[6]);
            Assert.AreEqual(6, ringSize[10]);
        }

        /// <summary>
        /// This test takes a very long time. It was to ensure that
        /// AllRingsFinder actually stops for the given examples.
        /// And it does, after a very long time.
        /// So, the test is commented out because of its long runtime.
        /// </summary>
        // @cdk.bug 777488
        [TestCategory("SlowTest")]
        [TestMethod()]
        public void TestBug777488()
        {
            //string filename = "data/Bug646.cml";
            string filename = "NCDK.Data.CML.testBug777488-1-AllRingsFinder.cml";
            //string filename = "data/NCI_diversity_528.mol.cml";
            //string filename = "data/NCI_diversity_978.mol.cml";
            var ins = ResourceLoader.GetAsStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer mol = model.MoleculeSet[0];
            if (standAlone) Console.Out.WriteLine("Constructed Molecule");
            if (standAlone) Console.Out.WriteLine("Starting AllRingsFinder");
            IRingSet ringSet = new AllRingsFinder().FindAllRings(mol);
            if (standAlone) Console.Out.WriteLine("Finished AllRingsFinder");
            if (standAlone) Console.Out.WriteLine("Found " + ringSet.Count + " rings.");

            //Display(mol);
        }

        [TestMethod()]
        public void TestRingFlags1()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = sp.ParseSmiles("c1ccccc1");
            foreach (var a in molecule.Atoms)
                a.IsInRing = false;
            AllRingsFinder arf = new AllRingsFinder();
            arf.FindAllRings(molecule);

            int count = 0;
            foreach (var atom in molecule.Atoms)
            {
                if (atom.IsInRing) count++;
            }
            Assert.AreEqual(6, count, "All atoms in benzene were not marked as being in a ring");
        }

        [TestMethod()]
        public void TestRingFlags2()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = sp.ParseSmiles("C1CCCC1CC");
            foreach (var a in molecule.Atoms)
                a.IsInRing = false;

            AllRingsFinder arf = new AllRingsFinder();
            arf.FindAllRings(molecule);

            int count = 0;
            foreach (var atom in molecule.Atoms)
            {
                if (atom.IsInRing) count++;
            }
            Assert.AreEqual(5, count, "All atoms in 1-ethyl-cyclopentane were not marked as being in a ring");
        }

        [TestMethod()]
        public void TestBigRingSystem_MaxRingSize6_03419()
        {
            IRingSet ringSet = null;
            AllRingsFinder arf = new AllRingsFinder();
            string filename = "NCDK.Data.MDL.ring_03419.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer molecule = model.MoleculeSet[0];
            ringSet = arf.FindAllRings(molecule, 6);
            Assert.AreEqual(12, ringSet.Count);
        }

        [TestMethod()]
        public void TestBigRingSystem_MaxRingSize4_fourRing5x10()
        {
            IRingSet ringSet = null;
            AllRingsFinder arf = new AllRingsFinder();
            string filename = "NCDK.Data.MDL.four-ring-5x10.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer molecule = model.MoleculeSet[0];
            // there are 5x10 squares (four-rings) in the 5x10 molecule
            ringSet = arf.FindAllRings(molecule, 4);
            Assert.AreEqual(50, ringSet.Count);
        }

        [TestMethod()]
        public void TestBigRingSystem_MaxRingSize6_fourRing5x10()
        {
            IRingSet ringSet = null;
            AllRingsFinder arf = new AllRingsFinder();
            string filename = "NCDK.Data.MDL.four-ring-5x10.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer molecule = model.MoleculeSet[0];
            // there are 5x10 four-rings (squares ) = 50
            // there are (9x5) + (4x10) six-rings   = 85
            // combined 135
            ringSet = arf.FindAllRings(molecule, 6);
            Assert.AreEqual(135, ringSet.Count);
        }
    }
}
