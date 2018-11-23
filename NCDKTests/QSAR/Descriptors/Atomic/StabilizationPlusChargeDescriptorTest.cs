/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@yahoo.es>
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
    public class StabilizationPlusChargeDescriptorTest : AtomicDescriptorTest<StabilizationPlusChargeDescriptor>
    {
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestStabilizationPlusChargeDescriptor()
        {
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms[1].FormalCharge = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[1]);
            Assert.AreNotEqual(0.0, result.Value);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestNotCharged()
        {
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[0]);
            Assert.AreEqual(0.0, result.Value, 0.00001);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestStabilizationPlusChargeDescriptor2()
        {
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms[1].FormalCharge = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[1]);
            Assert.AreNotEqual(0.0, result.Value);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestStabilizationComparative()
        {
            var mol1 = CDK.Builder.NewAtomContainer();
            mol1.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol1.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol1.Atoms[1].FormalCharge = 1;
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[1], BondOrder.Single);
            mol1.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[2], BondOrder.Single);
            mol1.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AddExplicitHydrogens(mol1);
            CDK.LonePairElectronChecker.Saturate(mol1);

            var result1 = CreateDescriptor(mol1).Calculate(mol1.Atoms[1]).Value;

            var mol2 = CDK.Builder.NewAtomContainer();
            mol2.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol2.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol2.Atoms[1].FormalCharge = 1;
            mol2.AddBond(mol2.Atoms[0], mol2.Atoms[1], BondOrder.Single);
            mol2.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol2.AddBond(mol2.Atoms[1], mol2.Atoms[2], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            AddExplicitHydrogens(mol2);
            CDK.LonePairElectronChecker.Saturate(mol2);

            var result2 = CreateDescriptor(mol2).Calculate(mol2.Atoms[1]).Value;

            var mol3 = CDK.Builder.NewAtomContainer();
            mol3.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol3.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol3.Atoms[1].FormalCharge = 1;
            mol3.AddBond(mol3.Atoms[0], mol3.Atoms[1], BondOrder.Single);
            mol3.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol3.AddBond(mol3.Atoms[1], mol3.Atoms[2], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol3);
            AddExplicitHydrogens(mol3);
            CDK.LonePairElectronChecker.Saturate(mol3);

            var result3 = CreateDescriptor(mol3).Calculate(mol3.Atoms[1]).Value;

            Assert.IsTrue(result3 < result2);
            Assert.IsTrue(result2 < result1);
        }

        /// <summary>
        /// C=CCCl # C=CC[Cl+*]
        /// </summary>
        // @cdk.inchi InChI=1/C3H7Cl/c1-2-3-4/h2-3H2,1H3
        [TestMethod()]
        [TestCategory("SlowTest")]
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

            var resultA = CreateDescriptor(molA).Calculate(molA.Atoms[3]).Value;

            var molB = CDK.Builder.NewAtomContainer();
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

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molB);
            AddExplicitHydrogens(molB);
            CDK.LonePairElectronChecker.Saturate(molB);

            Assert.AreEqual(1, molB.Atoms[3].FormalCharge.Value, 0.00001);
            Assert.AreEqual(1, molB.SingleElectrons.Count, 0.00001);
            Assert.AreEqual(2, molB.LonePairs.Count, 0.00001);

            var resultB = CreateDescriptor(molB).Calculate(molB.Atoms[3]).Value;

            Assert.AreNotEqual(resultA, resultB);
        }
    }
}
