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
using NCDK.Silent;
using NCDK.QSAR.Results;
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
    public class PartialSigmaChargeDescriptorTest : AtomicDescriptorTest
    {
        private readonly static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        
        public PartialSigmaChargeDescriptorTest()
        {
            SetDescriptor(typeof(PartialSigmaChargeDescriptor));
            descriptor.Parameters = new object[] { 6 };
        }

        /// <summary>
        /// A unit test with Fluoroethylene
        /// </summary>
        /// @cdk.inchi InChI=1/C2H3F/c1-2-3/h2H,1H2
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Fluoroethylene()
        {
            double[] testResult = { -0.2138, 0.079, 0.0942, -0.072, 0.0563, 0.0563 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[5], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[i], molecule).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.003);
            }
        }

        /// <summary>
        /// A unit test with Ethyl Fluoride
        /// </summary>
        /// @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Methyl_Floride()
        {
            double[] testResult = { 0.07915, -0.25264, 0.05783, 0.05783, 0.05783 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("F"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            AddExplicitHydrogens(molecule);
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[i], molecule).Value)
                        .Value;
                Assert.AreEqual(testResult[i], result, 0.001);
            }
        }

        /// <summary>
        /// A unit test with Methyl chloride
        /// </summary>
        // @cdk.inchi  InChI=1/CH3Cl/c1-2/h1H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Methyl_chloride()
        {
            double[] testResult = { 0.0382, -0.1755, 0.0457, 0.0457, 0.0457 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("Cl"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[i], molecule).Value)
                        .Value;
                Assert.AreEqual(testResult[i], result, 0.001);
            }
        }

        /// <summary>
        /// A unit test with Methyl chloride
        /// </summary>
        // @cdk.inchi  InChI=1/CH3Br/c1-2/h1H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Methyl_bromide()
        {
            double[] testResult = { 0.021, -0.1448, 0.0413, 0.0413, 0.0413 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("Br"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[i], molecule).Value)
                        .Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// A unit test with Methyl iodide
        /// </summary>
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Methyl_iodide()
        {
            double[] testResult = { -0.0116, -0.0892, 0.0336, 0.0336, 0.0336 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("I"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[i], molecule).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.001);
            }
        }

        /// <summary>
        /// A unit test with Allyl bromide
        /// </summary>
        // @cdk.inchi  InChI=1/C3H5Br/c1-2-3-4/h2H,1,3H2
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Allyl_bromide()
        {
            double testResult = -0.1366;
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("Br"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[3], molecule).Value).Value;
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test with Isopentyl iodide
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

            for (int i = 0; i < 6; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.001);
            }
        }

        /// <summary>
        /// A unit test with Ethoxy ethane
        /// </summary>
        //  @cdk.inchi  InChI=1/C4H10O/c1-3-5-4-2/h3-4H2,1-2H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Ethoxy_ethane()
        {
            double testResult = -0.3809;
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[2], molecule).Value).Value;
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test with Ethanolamine
        /// </summary>
        // @cdk.inchi  InChI=1/C2H7NO/c3-1-2-4/h4H,1-3H2
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Ethanolamine()
        {
            double[] testResult = { -0.3293, 0.017, 0.057, -0.3943 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml 
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("N"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            for (int i = 0; i < 4; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[i], molecule).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// A unit test with Allyl mercaptan
        /// </summary>
        // @cdk.inchi  InChI=1/C3H6S/c1-2-3-4/h2,4H,1,3H2
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor_Allyl_mercaptan()
        {
            double[] testResult = { -0.1031, -0.0828, 0.0093, -0.1742 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("S"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            for (int i = 0; i < 4; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[i], molecule).Value)
                        .Value;
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

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
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
            descriptor.Parameters = new object[] { 6 };
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
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

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.2);
            }
        }

        // @cdk.inchi  InChI=1/CH2O/c1-2/h1H2
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor4()
        {
            double[] testResult = { -0.3041, 0.1055, 0.0993, 0.0993 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[i], molecule).Value)
                        .Value;
                Assert.AreEqual(testResult[i], result, 0.003);
            }
        }

        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2H,1H3
        [TestMethod()]
        public void TestPartialSigmaChargeDescriptor5()
        {
            double[] testResult = { -0.3291, 0.144, 0.1028, -0.0084, 0.0303, 0.0303, 0.0303 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[5], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            AddExplicitHydrogens(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            descriptor.Parameters = new object[] { 6 };

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(molecule.Atoms[i], molecule).Value).Value;
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
            descriptor.Parameters = new object[] { 6 };
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            Debug.WriteLine(mol.Atoms[i].Symbol+",result: "+result);
                Assert.AreEqual(testResult[i], result, 0.3);
            }
        }

        /// <summary>
        /// A unit test with [H]c1[n-][c+]([H])c([H])c([H])c1([H])
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
            object[] obj = { 6 };
            descriptor.Parameters = obj;
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                //            logger.debug(mol.getAtom(i).getSymbol()+",result: "+result);
                Assert.AreEqual(testResult[i], result, 0.05);
            }
        }
    }
}
