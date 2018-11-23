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
using NCDK.Aromaticities;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class PartialSigmaChargeDescriptorTest : AtomicDescriptorTest<PartialSigmaChargeDescriptor>
    {
        protected override PartialSigmaChargeDescriptor CreateDescriptor(IAtomContainer mol)
            => new PartialSigmaChargeDescriptor(mol, 6);

        private readonly static IChemObjectBuilder builder = CDK.Builder;

        /// <summary>
        /// Fluoroethylene
        /// </summary>
        /// @cdk.inchi InChI=1/C2H3F/c1-2-3/h2H,1H2
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Fluoroethylene()
        {
            double[] testResult = { -0.2138, 0.079, 0.0942, -0.072, 0.0563, 0.0563 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[5], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.003);
            }
        }

        /// <summary>
        /// Ethyl Fluoride
        /// </summary>
        /// @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Methyl_Floride()
        {
            double[] testResult = { 0.07915, -0.25264, 0.05783, 0.05783, 0.05783 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            AddExplicitHydrogens(mol);
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.001);
            }
        }

        /// <summary>
        /// Methyl chloride
        /// </summary>
        // @cdk.inchi  InChI=1/CH3Cl/c1-2/h1H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Methyl_chloride()
        {
            double[] testResult = { 0.0382, -0.1755, 0.0457, 0.0457, 0.0457 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.001);
            }
        }

        /// <summary>
        /// Methyl chloride
        /// </summary>
        // @cdk.inchi  InChI=1/CH3Br/c1-2/h1H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Methyl_bromide()
        {
            double[] testResult = { 0.021, -0.1448, 0.0413, 0.0413, 0.0413 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("Br"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// Methyl iodide
        /// </summary>
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Methyl_iodide()
        {
            double[] testResult = { -0.0116, -0.0892, 0.0336, 0.0336, 0.0336 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("I"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.001);
            }
        }

        /// <summary>
        /// Allyl bromide
        /// </summary>
        // @cdk.inchi  InChI=1/C3H5Br/c1-2-3-4/h2H,1,3H2
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Allyl_bromide()
        {
            var testResult = -0.1366;
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("Br"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[3]).Value;
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// Isopentyl iodide
        /// </summary>
        // @cdk.inchi  InChI=1/C5H11I/c1-5(2)3-4-6/h5H,3-4H2,1-2H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Isopentyl_iodide()
        {
            double[] testResult = { -0.0458, -0.0623, -0.0623, -0.0415, 0.0003, -0.0855 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C(C)(C)CCI");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 6; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.001);
            }
        }

        /// <summary>
        /// Ethoxy ethane
        /// </summary>
        //  @cdk.inchi  InChI=1/C4H10O/c1-3-5-4-2/h3-4H2,1-2H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Ethoxy_ethane()
        {
            var testResult = -0.3809;
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[2]).Value;
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// Ethanolamine
        /// </summary>
        // @cdk.inchi  InChI=1/C2H7NO/c3-1-2-4/h4H,1-3H2
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Ethanolamine()
        {
            double[] testResult = { -0.3293, 0.017, 0.057, -0.3943 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml 
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("N"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 4; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// Allyl mercaptan
        /// </summary>
        // @cdk.inchi  InChI=1/C3H6S/c1-2-3-4/h2,4H,1,3H2
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Allyl_mercaptan()
        {
            double[] testResult = { -0.1031, -0.0828, 0.0093, -0.1742 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("S"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 4; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor1()
        {
            double[] testResult = { -0.2138, 0.079, 0.0942, -0.072, 0.0563, 0.0563 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[F+]=C([H])[C-]([H])[H]");

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.003);
            }
        }

        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor2()
        {
            double[] testResult = { -0.3855, -0.0454, 0.0634, -0.0544, -0.0391, -0.0391 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=C([H])[C-]([H])[H]");
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.2);
            }
        }

        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor3()
        {
            double[] testResult = { -0.3855, -0.0454, 0.0634, -0.0544, -0.0391, -0.0391 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[O-]C([H])=C([H])[H]");
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.2);
            }
        }

        // @cdk.inchi  InChI=1/CH2O/c1-2/h1H2
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor4()
        {
            double[] testResult = { -0.3041, 0.1055, 0.0993, 0.0993 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.003);
            }
        }

        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor5()
        {
            double[] testResult = { -0.3291, 0.144, 0.1028, -0.0084, 0.0303, 0.0303, 0.0303 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[5], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.03);
            }
        }

        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor6()
        {
            double[] testResult = { -0.4331, -0.1067, 0.0133, 0.0133, 0.0133 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[O-]C([H])([H])[H]");
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                //            Debug.WriteLine(mol.Atoms[i].Symbol+",result: "+result);
                Assert.AreEqual(testResult[i], result, 0.3);
            }
        }

        /// <summary>
        /// [H]c1[n-][c+]([H])c([H])c([H])c1([H])
        /// </summary>
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor7()
        {
            // from Petra online: http://www2.chemi.uni.erlange.de/services/petra/smiles.pthml
            double[] testResult = { 0.0835, 0.0265, -0.2622, 0.0265, 0.0835, -0.0444, 0.064, -0.0596, 0.0626, -0.0444, 0.064 };
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[H]c1[n-][c+]([H])c([H])c([H])c1([H])");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.05);
            }
        }
    }
}
