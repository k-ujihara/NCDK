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
    public class PartialTChargePEOEDescriptorTest : AtomicDescriptorTest<PartialTChargePEOEDescriptor>
    {
        /// <summary>
        /// Ethyl Fluoride
        /// </summary>
        // @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialTChargeDescriptor_Methyl_Fluoride()
        {
            double[] testResult = { -0.2527, 0.0795, 0.0577, 0.0577, 0.0577 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            AddExplicitHydrogens(mol);
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;

                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// Fluoroethylene
        /// </summary>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialTChargeDescriptor_Fluoroethylene()
        {
            double[] testResult = { -0.1839, 0.079, -0.1019, 0.0942, 0.0563, 0.0563 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("F-C=C");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;

                Assert.AreEqual(testResult[i], result, 0.04);
            }
        }

        /// <summary>
        /// Formic Acid
        /// </summary>
        // @cdk.inchi  InChI=1/CH2O2/c2-1-3/h1H,(H,2,3)/f/h2H
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialTChargeDescriptor_FormicAcid()
        {
            double[] testResult = { 0.2672, -0.3877, -0.2365, 0.1367, 0.2203 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;

                Assert.AreEqual(testResult[i], result, 0.05);
            }
        }

        /// <summary>
        /// Fluorobenzene
        /// </summary>
        // @cdk.inchi InChI=1/C6H5F/c7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialTChargeDescriptor_Fluorobenzene()
        {
            double[] testResult = { -0.1785, 0.1227, -0.0373, -0.0598, -0.0683 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Double);
            mol.AddBond(mol.Atoms[6], mol.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 5; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;

                Assert.AreEqual(testResult[i], result, 0.012);
            }
        }

        /// <summary>
        /// Methoxyethylene
        /// </summary>
        // @cdk.inchi InChI=1/C3H6O/c1-3-4-2/h3H,1H2,2H3
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialTChargeDescriptor_Methoxyethylene()
        {
            double[] testResult = { -0.1211, 0.0314, -0.3121, 0.0429, 0.056, 0.056, 0.0885, 0.056, 0.056, 0.056 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;

                Assert.AreEqual(testResult[i], result, 0.05);
            }
        }

        /// <summary>
        /// 1-Methoxybutadiene
        /// </summary>
        // @cdk.inchi InChI=1/C5H8O/c1-3-4-5-6-2/h3-5H,1H2,2H3
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialTChargeDescriptor_1_Methoxybutadiene()
        {
            double[] testResult = {-0.1331, -0.0678, -0.0803, 0.0385, -0.2822, 0.0429, 0.0541, 0.0541, 0.0619, 0.0644,
                0.0891, 0.0528, 0.0528, 0.0528, 0.0528};
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;

                Assert.AreEqual(testResult[i], result, 0.3);
            }
        }
    }
}
