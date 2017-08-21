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
using NCDK.QSAR.Result;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class StabilizationPlusChargeDescriptorTest : AtomicDescriptorTest
    {
        private readonly static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        LonePairElectronChecker lpcheck = new LonePairElectronChecker();

        public StabilizationPlusChargeDescriptorTest()
        {
            descriptor = new StabilizationPlusChargeDescriptor();
            SetDescriptor(typeof(StabilizationPlusChargeDescriptor));
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestStabilizationPlusChargeDescriptor()
        {
            IAtomContainer mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[1].FormalCharge = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            lpcheck.Saturate(mol);

            DoubleResult result = ((DoubleResult)descriptor.Calculate(mol.Atoms[1], mol).Value);

            Assert.AreNotSame(0.0, result.Value);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestNotCharged()
        {
            IAtomContainer mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            lpcheck.Saturate(mol);

            DoubleResult result = ((DoubleResult)descriptor.Calculate(mol.Atoms[0], mol).Value);

            Assert.AreEqual(0.0, result.Value, 0.00001);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestStabilizationPlusChargeDescriptor2()
        {
            IAtomContainer mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[1].FormalCharge = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            lpcheck.Saturate(mol);

            DoubleResult result = ((DoubleResult)descriptor.Calculate(mol.Atoms[1], mol).Value);

            Assert.AreNotSame(0.0, result.Value);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestStabilizationComparative()
        {
            IAtomContainer mol1 = builder.NewAtomContainer();
            mol1.Atoms.Add(builder.NewAtom("C"));
            mol1.Atoms.Add(builder.NewAtom("C"));
            mol1.Atoms[1].FormalCharge = 1;
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[1], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("C"));
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[2], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("O"));
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[3], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AddExplicitHydrogens(mol1);
            lpcheck.Saturate(mol1);

            DoubleResult result1 = ((DoubleResult)descriptor.Calculate(mol1.Atoms[1], mol1).Value);

            IAtomContainer mol2 = builder.NewAtomContainer();
            mol2.Atoms.Add(builder.NewAtom("C"));
            mol2.Atoms.Add(builder.NewAtom("C"));
            mol2.Atoms[1].FormalCharge = 1;
            mol2.AddBond(mol2.Atoms[0], mol2.Atoms[1], BondOrder.Single);
            mol2.Atoms.Add(builder.NewAtom("O"));
            mol2.AddBond(mol2.Atoms[1], mol2.Atoms[2], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            AddExplicitHydrogens(mol2);
            lpcheck.Saturate(mol2);

            DoubleResult result2 = ((DoubleResult)descriptor.Calculate(mol2.Atoms[1], mol2).Value);

            IAtomContainer mol3 = builder.NewAtomContainer();
            mol3.Atoms.Add(builder.NewAtom("C"));
            mol3.Atoms.Add(builder.NewAtom("C"));
            mol3.Atoms[1].FormalCharge = 1;
            mol3.AddBond(mol3.Atoms[0], mol3.Atoms[1], BondOrder.Single);
            mol3.Atoms.Add(builder.NewAtom("C"));
            mol3.AddBond(mol3.Atoms[1], mol3.Atoms[2], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol3);
            AddExplicitHydrogens(mol3);
            lpcheck.Saturate(mol3);

            DoubleResult result3 = ((DoubleResult)descriptor.Calculate(mol3.Atoms[1], mol3).Value);

            Assert.IsTrue(result3.Value < result2.Value);
            Assert.IsTrue(result2.Value < result1.Value);
        }

        /// <summary>
        /// A unit test for JUnit with C=CCCl # C=CC[Cl+*]
        /// </summary>
        // @cdk.inchi InChI=1/C3H7Cl/c1-2-3-4/h2-3H2,1H3
        [TestMethod()]
        [TestCategory("SlowTest")]
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
            lpcheck.Saturate(molA);

            double resultA = ((DoubleResult)descriptor.Calculate(molA.Atoms[3], molA).Value).Value;

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

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molB);
            AddExplicitHydrogens(molB);
            lpcheck.Saturate(molB);

            Assert.AreEqual(1, molB.Atoms[3].FormalCharge.Value, 0.00001);
            Assert.AreEqual(1, molB.SingleElectrons.Count, 0.00001);
            Assert.AreEqual(2, molB.LonePairs.Count, 0.00001);

            double resultB = ((DoubleResult)descriptor.Calculate(molB.Atoms[3], molB).Value).Value;

            Assert.AreNotSame(resultA, resultB);
        }
    }
}
