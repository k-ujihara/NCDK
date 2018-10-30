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
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System.Diagnostics;

namespace NCDK.Graphs.Invariant
{
    /// <summary>
    /// Checks the functionality of the ConjugatedPiSystemsCalculator.
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class ConjugatedPiSystemsDetectorTest : CDKTestCase
    {
        [TestMethod()]
        public void TestDetectButadiene()
        {
            Trace.TraceInformation("Entering testDetectButadiene.");
            IAtomContainer mol = null;
            string filename = "NCDK.Data.CML.butadiene.cml";
            mol = ReadCMLMolecule(filename);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac = acSet[0];
            Assert.AreEqual(4, ac.Atoms.Count);
            Assert.AreEqual(3, ac.Bonds.Count);

            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac.Atoms[i]));
            }

            for (int i = 0; i < ac.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac.Bonds[i]));
            }
        }

        [TestMethod()]
        public void TestDetectNaphtalene()
        {
            Trace.TraceInformation("Entering testDetectNaphtalene.");
            IAtomContainer mol = null;
            string filename = "NCDK.Data.CML.naphtalene.cml";
            mol = ReadCMLMolecule(filename);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac = acSet[0];
            Assert.AreEqual(10, ac.Atoms.Count);
            Assert.AreEqual(11, ac.Bonds.Count);

            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac.Atoms[i]));
            }

            for (int i = 0; i < ac.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac.Bonds[i]));
            }
        }

        [TestMethod()]
        public void TestDetectToluene()
        {
            Trace.TraceInformation("Entering testDetectToluene.");
            IAtomContainer mol = null;
            string filename = "NCDK.Data.CML.toluene.cml";
            mol = ReadCMLMolecule(filename);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac = acSet[0];
            Assert.AreEqual(6, ac.Atoms.Count);
            Assert.AreEqual(6, ac.Bonds.Count);

            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac.Atoms[i]));
            }

            for (int i = 0; i < ac.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac.Bonds[i]));
            }
        }

        [TestMethod()]
        public void TestNonConnectedPiSystems()
        {
            Trace.TraceInformation("Entering testNonConnectedPiSystems.");
            IAtomContainer mol = null;
            string filename = "NCDK.Data.MDL.nonConnectedPiSystems.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLReader reader = new MDLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            mol = chemFile[0][0].MoleculeSet[0];
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(2, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(4, ac1.Atoms.Count);
            Assert.AreEqual(3, ac1.Bonds.Count);

            for (int i = 0; i < ac1.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Atoms[i]));
            }

            for (int i = 0; i < ac1.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Bonds[i]));
            }

            IAtomContainer ac2 = acSet[1];
            Assert.AreEqual(4, ac2.Atoms.Count);
            Assert.AreEqual(3, ac2.Bonds.Count);

            for (int i = 0; i < ac2.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac2.Atoms[i]));
            }

            for (int i = 0; i < ac2.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac2.Bonds[i]));
            }
        }

        [TestMethod()]
        public void TestPiSystemWithCarbokation()
        {
            Trace.TraceInformation("Entering testPiSystemWithCarbokation.");
            IAtomContainer mol = null;
            string filename = "NCDK.Data.MDL.piSystemWithCarbokation.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLReader reader = new MDLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read((ChemObject)new ChemFile());
            mol = chemFile[0][0].MoleculeSet[0];

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(2, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(4, ac1.Atoms.Count);
            Assert.AreEqual(3, ac1.Bonds.Count);

            for (int i = 0; i < ac1.Atoms.Count; i++)
                Assert.IsTrue(mol.Contains(ac1.Atoms[i]));

            for (int i = 0; i < ac1.Bonds.Count; i++)
                Assert.IsTrue(mol.Contains(ac1.Bonds[i]));

            IAtomContainer ac2 = acSet[0];
            Assert.AreEqual(4, ac2.Atoms.Count);
            Assert.AreEqual(3, ac2.Bonds.Count);

            for (int i = 0; i < ac2.Atoms.Count; i++)
                Assert.IsTrue(mol.Contains(ac2.Atoms[i]));

            for (int i = 0; i < ac2.Bonds.Count; i++)
                Assert.IsTrue(mol.Contains(ac2.Bonds[i]));

        }

        [TestMethod()]
        public void TestPiSystemWithCumulativeDB()
        {
            Trace.TraceInformation("Entering testPiSystemWithCumulativeDB.");
            IAtomContainer mol = null;
            string filename = "NCDK.Data.MDL.piSystemCumulative.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLReader reader = new MDLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read((ChemObject)new ChemFile());

            mol = chemFile[0][0].MoleculeSet[0];
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(2, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(4, ac1.Atoms.Count);
            Assert.AreEqual(3, ac1.Bonds.Count);

            for (int i = 0; i < ac1.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Atoms[i]));
            }

            for (int i = 0; i < ac1.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Bonds[i]));
            }

            IAtomContainer ac2 = acSet[0];
            Assert.AreEqual(4, ac2.Atoms.Count);
            Assert.AreEqual(3, ac2.Bonds.Count);

            for (int i = 0; i < ac2.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Atoms[i]));
            }

            for (int i = 0; i < ac2.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Bonds[i]));
            }

        }

        // @cdk.inchi InChI=1/C2H4O2/c1-2(3)4/h1H3,(H,3,4)/f/h3
        [TestMethod()]
        public void TestAceticAcid()
        {
            IAtomContainer mol = null;
            mol = CDK.SilentSmilesParser.ParseSmiles("CC(=O)O");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(3, ac1.Atoms.Count);
            Assert.AreEqual(2, ac1.Bonds.Count);

            for (int i = 0; i < ac1.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Atoms[i]));
            }

            for (int i = 0; i < ac1.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Bonds[i]));
            }

        }

        [TestMethod()]
        public void TestNN_dimethylaniline_cation()
        {
            IAtomContainer mol = null;
            string filename = "NCDK.Data.MDL.NN_dimethylaniline.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile chemFile = (IChemFile)reader.Read((ChemObject)new ChemFile());
            mol = chemFile[0][0].MoleculeSet[0];

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(6, ac1.Atoms.Count);
            Assert.AreEqual(5, ac1.Bonds.Count);

        }

        [TestMethod()]
        public void Test1_fluorobutadienene()
        {
            IAtomContainer mol = CDK.SilentSmilesParser.ParseSmiles("FC=CC=C");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(5, ac1.Atoms.Count);
            Assert.AreEqual(4, ac1.Bonds.Count);

        }

        // @cdk.inchi  InChI=1/C2F2/c3-1-2-4
        [TestMethod()]
        public void TestEthyne_difluoro()
        {
            IAtomContainer mol = null;
            mol = CDK.SilentSmilesParser.ParseSmiles("FC#CF");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(4, ac1.Atoms.Count);
            Assert.AreEqual(3, ac1.Bonds.Count);

        }

        // @cdk.inchi  InChI=1/C7H19N3/c1-8(2)7(9(3)4)10(5)6/h7H,1-6H3
        [TestMethod()]
        public void Test3Aminomethane_cation()
        {
            var builder = Silent.ChemObjectBuilder.Instance;
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms[3].FormalCharge = +1;
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[4], mol.Atoms[6], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[7], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(4, ac1.Atoms.Count);
            Assert.AreEqual(3, ac1.Bonds.Count);

        }

        private IAtomContainer ReadCMLMolecule(string filename)
        {
            IAtomContainer mol = null;
            Debug.WriteLine($"Filename: {filename}");
            var ins = ResourceLoader.GetAsStream(filename);
            CMLReader reader = new CMLReader(ins);

            IChemFile file = (IChemFile)reader.Read(new ChemFile());
            Assert.IsNotNull(file);
            Assert.AreEqual(1, file.Count);
            IChemSequence sequence = file[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(1, sequence.Count);
            IChemModel chemModel = sequence[0];
            Assert.IsNotNull(chemModel);
            var moleculeSet = chemModel.MoleculeSet;
            Assert.IsNotNull(moleculeSet);
            Assert.AreEqual(1, moleculeSet.Count);
            mol = moleculeSet[0];
            Assert.IsNotNull(mol);

            return mol;

        }

        // @cdk.inchi  InChI=1/C4H3N/c1-2-3-4-5/h3H,1H2
        [TestMethod()]
        public void TestCyanoallene()
        {
            IAtomContainer mol = null;
            mol = CDK.SilentSmilesParser.ParseSmiles("C=C=CC#N");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(4, ac1.Atoms.Count);
            Assert.AreEqual(3, ac1.Bonds.Count);

            for (int i = 0; i < ac1.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Atoms[i]));
            }

            for (int i = 0; i < ac1.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Bonds[i]));
            }

        }

        // with [H]C([H])=C([H])[C+]([H])[H]
        [TestMethod()]
        public void TestChargeWithProtonExplicit()
        {
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("[H]C([H])=C([H])[C+]([H])[H]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(3, ac1.Atoms.Count);
            Assert.AreEqual(2, ac1.Bonds.Count);

            for (int i = 0; i < ac1.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Atoms[i]));
            }

            for (int i = 0; i < ac1.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Bonds[i]));
            }
        }

        // with  [H]C([H])=C([H])[C+]([H])[H]
        [TestMethod()]
        public void TestChargeWithProtonImplicit()
        {
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("C=C[C+]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var acSet = ConjugatedPiSystemsDetector.Detect(mol);

            Assert.AreEqual(1, acSet.Count);
            IAtomContainer ac1 = acSet[0];
            Assert.AreEqual(3, ac1.Atoms.Count);
            Assert.AreEqual(2, ac1.Bonds.Count);

            for (int i = 0; i < ac1.Atoms.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Atoms[i]));
            }

            for (int i = 0; i < ac1.Bonds.Count; i++)
            {
                Assert.IsTrue(mol.Contains(ac1.Bonds[i]));
            }
        }
    }
}
