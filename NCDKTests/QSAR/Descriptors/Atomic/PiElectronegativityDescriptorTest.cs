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
using NCDK.QSAR.Results;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class PiElectronegativityDescriptorTest : AtomicDescriptorTest
    {
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /// <summary>
        ///  Constructor for the PiElectronegativityDescriptorTest object
        ///
        /// </summary>
        public PiElectronegativityDescriptorTest()
        {
            SetDescriptor(typeof(PiElectronegativityDescriptor));
        }

        /// <summary>
        ///  A unit test for JUnit with Methyl Fluoride
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methyl_Fluoride()
        {
            double[] testResult = { 3.9608, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("FC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                descriptor.Parameters = new object[] { 10 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
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
        ///  A unit test for JUnit with Methyl Chloride
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methyl_Chloride()
        {
            double[] testResult = { 4.7054, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();

            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("ClC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                descriptor.Parameters = new object[] { 10 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            Debug.WriteLine("result: "+result);
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
        ///  A unit test for JUnit with Methyl iodide
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methyl_Iodide()
        {
            double[] testResult = { 4.1951, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("IC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                descriptor.Parameters = new object[] { 10 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            Debug.WriteLine("result: "+result);
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
        ///  A unit test for JUnit with Methyl Bromide
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methyl_Bromide()
        {
            double[] testResult = { 3.8922, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("BrC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                descriptor.Parameters = new object[] { 10 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
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
        ///  A unit test for JUnit with Methyl Alcohol
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methyl_Alcohol()
        {
            double[] testResult = { 3.1138, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("OC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < 4; i++)
            {
                descriptor.Parameters = new object[] { 10 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            Debug.WriteLine("result: "+result);
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
        ///  A unit test for JUnit with Formaldehyde
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Formaldehyde()
        {
            double[] testResult = { 6.3012, 8.0791, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("C=O");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                descriptor.Parameters = new object[] { 10 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            Debug.WriteLine("result: "+result);
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
        ///  A unit test for JUnit with Ethylene
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Ethylene()
        {
            double[] testResult = { 5.1519, 5.1519, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();

            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("C=C");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);
            for (int i = 0; i < 3; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            Debug.WriteLine("result: "+result);
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
        ///  A unit test for JUnit with Fluoroethylene
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Fluoroethylene()
        {
            double[] testResult = { 4.7796, 5.9414, 5.0507, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();

            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("F-C=C");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < 3; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            Debug.WriteLine("result: "+result);
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
        ///  A unit test for JUnit with Formic Acid
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_FormicAcid()
        {
            double[] testResult = { 6.8954, 7.301, 4.8022, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();

            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("C(=O)O");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            Debug.WriteLine("result: "+result);
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
        ///  A unit test for JUnit with Methoxyethylene
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativityDescriptor_Methoxyethylene()
        {
            double[] testResult = { 4.916, 5.7345, 3.971, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();

            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("C=C-O-C");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            Debug.WriteLine("result: "+result);
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
        ///  A unit test for JUnit with F[C+][C-]
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativity1()
        {
            double[] testResult = { 5.1788, 5.465, 5.2475, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();

            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("F[C+][C-]");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            Debug.WriteLine(mol.GetAtomAt(i).Symbol+"-result: "+result);
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
        ///  A unit test for JUnit with CCOCCCO
        /// </summary>
        [TestMethod()]
        public void TestPiElectronegativity2()
        {
            double[] testResult = { 0.0, 0.0, 3.2849, 0.0, 0.0, 0.0, 3.2849, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            /// from Petra online:
            /// http://www2.chemie.uni-erlangen
            /// .de/services/petra/smiles.phtml
            /// </summary>
            IAtomicDescriptor descriptor = new PiElectronegativityDescriptor();

            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("CCOCCCO");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
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
        ///  A unit test for JUnit with CCCCl # CCC[Cl+*]
        ///
        ///  @cdk.inchi InChI=1/C3H7Cl/c1-2-3-4/h2-3H2,1H3
        /// </summary>
        [TestMethod()]
        public void TestCompareIonized()
        {
            IAtomContainer molA = builder.NewAtomContainer();
            molA.Atoms.Add(builder.NewAtom("C"));
            molA.Atoms.Add(builder.NewAtom("C"));
            molA.AddBond(molA.Atoms[0], molA.Atoms[1], BondOrder.Single);
            molA.Atoms.Add(builder.NewAtom("C"));
            molA.AddBond(molA.Atoms[1], molA.Atoms[2], BondOrder.Single);
            molA.Atoms.Add(builder.NewAtom("Cl"));
            molA.AddBond(molA.Atoms[2], molA.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molA);
            AddExplicitHydrogens(molA);
            CDK.LonePairElectronChecker.Saturate(molA);

            double resultA = ((Result<double>)descriptor.Calculate(molA.Atoms[3], molA).Value).Value;

            IAtomContainer molB = builder.NewAtomContainer();
            molB.Atoms.Add(builder.NewAtom("C"));
            molB.Atoms.Add(builder.NewAtom("C"));
            molB.AddBond(molB.Atoms[0], molB.Atoms[1], BondOrder.Single);
            molB.Atoms.Add(builder.NewAtom("C"));
            molB.AddBond(molB.Atoms[1], molB.Atoms[2], BondOrder.Single);
            molB.Atoms.Add(builder.NewAtom("Cl"));
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

            IAtomicDescriptor descriptor_ = new PiElectronegativityDescriptor();
            double resultB = ((Result<double>)descriptor_.Calculate(molB.Atoms[3], molB).Value).Value;

            Assert.AreEqual(resultA, resultB, 0.00001);
        }
    }
}
