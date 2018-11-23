/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class PiElectronegativityDescriptorTest : AtomicDescriptorTest<PiElectronegativityDescriptor>
    {
        public PiElectronegativityDescriptor CreateDescriptor(IAtomContainer mol, int maxIterations) => new PiElectronegativityDescriptor(mol, maxIterations);

        /// <summary>
        ///  Methyl Fluoride
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methyl_Fluoride()
        {
            double[] testResult = { 3.9608, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("FC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 10);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 0.03);
                }
            }
        }

        /// <summary>
        /// Methyl Chloride
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methyl_Chloride()
        {
            double[] testResult = { 4.7054, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("ClC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 10);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 0.01);
                }
            }
        }

        /// <summary>
        /// Methyl iodide
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methyl_Iodide()
        {
            double[] testResult = { 4.1951, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("IC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 10);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 0.01);
                }
            }
        }

        /// <summary>
        ///  Methyl Bromide
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methyl_Bromide()
        {
            double[] testResult = { 3.8922, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("BrC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 10);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                //            Debug.WriteLine("result: "+result);
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 0.03);
                }
            }
        }

        /// <summary>
        /// Methyl Alcohol
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methyl_Alcohol()
        {
            double[] testResult = { 3.1138, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("OC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 10);
            for (int i = 0; i < 4; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 0.01);
                }
            }
        }

        /// <summary>
        /// Formaldehyde
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Formaldehyde()
        {
            double[] testResult = { 6.3012, 8.0791, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=O");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 10);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 0.55);
                }
            }
        }

        /// <summary>
        ///  Ethylene
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Ethylene()
        {
            double[] testResult = { 5.1519, 5.1519, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=C");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 3; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 0.02);
                }
            }
        }

        /// <summary>
        ///  Fluoroethylene
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Fluoroethylene()
        {
            double[] testResult = { 4.7796, 5.9414, 5.0507, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("F-C=C");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 3; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 0.7);
                }
            }
        }

        /// <summary>
        /// Formic Acid
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_FormicAcid()
        {
            double[] testResult = { 6.8954, 7.301, 4.8022, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C(=O)O");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 2);
                }
            }
        }

        /// <summary>
        ///  Methoxyethylene
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methoxyethylene()
        {
            double[] testResult = { 4.916, 5.7345, 3.971, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=C-O-C");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 0.5);
                }
            }
        }

        /// <summary>
        /// F[C+][C-]
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativity1()
        {
            double[] testResult = { 5.1788, 5.465, 5.2475, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("F[C+][C-]");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 2.0);
                }
            }
        }

        /// <summary>
        ///  CCOCCCO
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativity2()
        {
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            double[] testResult = { 0.0, 0.0, 3.2849, 0.0, 0.0, 0.0, 3.2849, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCOCCCO");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                if (result == 0.0)
                    Assert.AreEqual(testResult[i], result, 0.0001);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(testResult[i], result, 0.2);
                }
            }
        }

        /// <summary>
        ///  CCCCl # CCC[Cl+*]
        ///
        ///  @cdk.inchi InChI=1/C3H7Cl/c1-2-3-4/h2-3H2,1H3
        /// </summary>
        [TestMethod()]
        public void TestCompareIonized()
        {
            var molA = CDK.Builder.NewAtomContainer();
            molA.Atoms.Add(CDK.Builder.NewAtom("C"));
            molA.Atoms.Add(CDK.Builder.NewAtom("C"));
            molA.AddBond(molA.Atoms[0], molA.Atoms[1], BondOrder.Single);
            molA.Atoms.Add(CDK.Builder.NewAtom("C"));
            molA.AddBond(molA.Atoms[1], molA.Atoms[2], BondOrder.Single);
            molA.Atoms.Add(CDK.Builder.NewAtom("Cl"));
            molA.AddBond(molA.Atoms[2], molA.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molA);
            AddExplicitHydrogens(molA);
            CDK.LonePairElectronChecker.Saturate(molA);

            PiElectronegativityDescriptor descriptor;

            descriptor = CreateDescriptor(molA);
            var resultA = descriptor.Calculate(molA.Atoms[3]).Value;

            IAtomContainer molB = CDK.Builder.NewAtomContainer();
            molB.Atoms.Add(CDK.Builder.NewAtom("C"));
            molB.Atoms.Add(CDK.Builder.NewAtom("C"));
            molB.AddBond(molB.Atoms[0], molB.Atoms[1], BondOrder.Single);
            molB.Atoms.Add(CDK.Builder.NewAtom("C"));
            molB.AddBond(molB.Atoms[1], molB.Atoms[2], BondOrder.Single);
            molB.Atoms.Add(CDK.Builder.NewAtom("Cl"));
            molB.Atoms[3].FormalCharge = 1;
            molB.AddSingleElectronTo(molB.Atoms[3]);
            molB.AddLonePairTo(molB.Atoms[3]);
            molB.AddLonePairTo(molB.Atoms[3]);
            molB.AddBond(molB.Atoms[2], molB.Atoms[3], BondOrder.Single);

            AddExplicitHydrogens(molB);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molB);
            CDK.LonePairElectronChecker.Saturate(molB);

            Assert.AreEqual(1, molB.Atoms[3].FormalCharge.Value, 0.00001);
            Assert.AreEqual(1, molB.SingleElectrons.Count, 0.00001);
            Assert.AreEqual(2, molB.LonePairs.Count, 0.00001);

            descriptor = CreateDescriptor(molB);
            var resultB = descriptor.Calculate(molB.Atoms[3]).Value;

            Assert.AreEqual(resultA, resultB, 0.00001);
        }
    }
}
