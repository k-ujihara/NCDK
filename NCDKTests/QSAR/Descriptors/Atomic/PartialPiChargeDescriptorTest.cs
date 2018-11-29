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
    public class PartialPiChargeDescriptorTest
        : AtomicDescriptorTest<PartialPiChargeDescriptor>
    {
        public PartialPiChargeDescriptor CreateDescriptor(IAtomContainer mol, int maxIterations) => new PartialPiChargeDescriptor(mol, maxIterations);
        public PartialPiChargeDescriptor CreateDescriptor(IAtomContainer mol, int maxIterations, bool checkLonePairElectron) => new PartialPiChargeDescriptor(mol, maxIterations, checkLonePairElectron);

        /// <summary>
        /// Ethyl Fluoride
        /// </summary>
        // @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialPiChargeDescriptor_Methyl_Fluoride()
        {
            double[] testResult = { 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);

                if (testResult[i] == 0.0)
                    Assert.IsTrue(result == 0.0);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(GetSign(testResult[i]), GetSign(result), 0.00001);
                }
                Assert.AreEqual(testResult[i], result, 0.0001);
            }
        }

        /// <summary>
        /// Fluoroethylene
        /// </summary>
        // @cdk.inchi InChI=1/C2H3F/c1-2-3/h2H,1H2
        // @cdk.bug   1959099
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialPiChargeDescriptor_Fluoroethylene()
        {
            double[] testResult = { 0.0299, 0.0, -0.0299, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol);

            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);
                Assert.AreEqual(testResult[i], result, 0.05);
            }
        }

        /// <summary>
        /// Formic Acid
        /// </summary>
        // @cdk.inchi  InChI=1/CH2O2/c2-1-3/h1H,(H,2,3)/f/h2H
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialPiChargeDescriptor_FormicAcid()
        {
            double[] testResult = { 0.0221, -0.1193, 0.0972, 0.0, 0.0 };
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
                Assert.IsNotNull(result);

                if (testResult[i] == 0.0)
                    Assert.IsTrue(result == 0.0);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(GetSign(testResult[i]), GetSign(result), 0.00001);
                }
                Assert.AreEqual(testResult[i], result, 0.04);
            }
        }

        /// <summary>
        /// Fluorobenzene
        /// </summary>
        // @cdk.inchi InChI=1/C6H5F/c7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialPiChargeDescriptor_Fluorobenzene()
        {
            double[] testResult = { 0.0262, 0.0, -0.0101, 0.0, -0.006, 0.0, -0.0101, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
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

            var descriptor = CreateDescriptor(mol, 6);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);

                if (testResult[i] == 0.0)
                    Assert.IsTrue(result == 0.0);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(GetSign(testResult[i]), GetSign(result), 0.00001);
                }
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// Methoxyethylene
        /// </summary>
        // @cdk.inchi InChI=1/C3H6O/c1-3-4-2/h3H,1H2,2H3
        // @cdk.bug   1959099
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialPiChargeDescriptor_Methoxyethylene()
        {
            double[] testResult = { -0.044, 0.0, 0.044, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
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

            var descriptor = CreateDescriptor(mol, 6);
            for (int i = 0; i < 4/* mol.Atoms.Count */; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);
                Assert.AreEqual(testResult[i], result, 0.05);
            }
        }

        /// <summary>
        /// 1-Methoxybutadiene
        /// </summary>
        // @cdk.inchi InChI=1/C5H8O/c1-3-4-5-6-2/h3-5H,1H2,2H3
        // @cdk.bug   1959099
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialPiChargeDescriptor_1_Methoxybutadiene()
        {
            double[] testResult = { -0.0333, 0.0, -0.0399, 0.0, 0.0733, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
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
                Assert.IsNotNull(result);
                Assert.AreEqual(testResult[i], result, 0.3);
            }
        }

        /// <summary>
        /// get the sign of a value
        /// </summary>
        private double GetSign(double d)
        {
            var sign = 0.0;
            if (d > 0)
                sign = 1;
            else if (d < 0) sign = -1;
            return sign;
        }

        /// <summary>
        /// A unit test for JUnit
        /// </summary>
        // @cdk.bug   1959099
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialPiChargeDescriptoCharge_1()
        {
            double[] testResult = { 0.0613, -0.0554, 0.0, -0.0059, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("F[C+]([H])[C-]([H])[H]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 6; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);
                Assert.AreEqual(testResult[i], result, 0.2);
            }
        }

        /// <summary>
        /// A unit test : n1ccccc1
        /// </summary>
        // @cdk.inchi InChI: InChI=1/C5H5N/c1-2-4-6-5-3-1/h1-5H
        // @cdk.bug   1959099
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialPiChargeDescriptoCharge_2()
        {
            double[] testResult = { -0.0822, 0.02, 0.0, 0.0423, 0.0, 0.02, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("N"));
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
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Double);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);
                Assert.AreEqual(testResult[i], result, 0.08);
            }
        }

        /// <summary>
        /// This mol breaks with PETRA as well.
        /// </summary>
        // @cdk.bug   1959099
        [TestMethod(), Ignore()] // Bug was always present - and is not a regression. The non-charge seperated form of mol produces the correct result.
        public void TestPartialPiChargeDescriptoCharge_3()
        {
            double[] testResult = { -0.0379, -0.0032, 0.0, -0.0078, 0.0, 0.0488, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=C([H])[C+]([H])[C-]([H])[H]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);
                Assert.AreEqual(testResult[i], result, 0.1);
            }
        }

        /// <summary>
        /// This mol breaks with PETRA as well.
        /// </summary>
        /// @cdk.inchi InChI: InChI=1/C5H12O2/c1-2-7-5-3-4-6/h6H,2-5H2,1H3
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestPartialPiChargeDescripto4()
        {
            double[] testResult = { 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCOCCCO");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.AreEqual(testResult[0], result, 0.0001);
            }
        }

        // @cdk.inchi InChI=1/C2H5NO/c1-2(3)4/h1H3,(H2,3,4)/f/h3H2
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestArticle1()
        {
            double[] testResult = { 0.0, 0.0216, -0.1644, 0.1428, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC(=O)N");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);

                if (testResult[i] == 0.0)
                    Assert.IsTrue(result == 0.0);
                else
                {
                    Assert.IsTrue(result != 0.0);
                    Assert.AreEqual(GetSign(testResult[i]), GetSign(result), 0.00001);
                }
                Assert.AreEqual(testResult[i], result, 0.1);
            }
        }

        /// <summary>
        /// [H]C1=C([H])C([H])=C(C(=C1(F))C([H])([H])[H])C([H])([H])C([H])([H])C(F)=O
        /// </summary>
        // @cdk.bug   1959099
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestSousa()
        {
            double[] testResult = { 0.0914, 0.0193, -0.1107, 0.0, 0.0, 0.0, -0.0063, 0.0, -0.0101, 0.0, 0.0262, -0.0098, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));//ring
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            //aromatic
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));//ring
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));//ring
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));//ring
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));//ring
            mol.AddBond(mol.Atoms[8], mol.Atoms[9], BondOrder.Single);
            //Fluor
            mol.Atoms.Add(CDK.Builder.NewAtom("F"));
            mol.AddBond(mol.Atoms[9], mol.Atoms[10], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));//ring
            mol.AddBond(mol.Atoms[9], mol.Atoms[11], BondOrder.Double);
            mol.AddBond(mol.Atoms[5], mol.Atoms[11], BondOrder.Single);

            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[11], mol.Atoms[12], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 6, true);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);
                Assert.AreEqual(testResult[i], result, 0.15);
            }
        }

        /// <summary>
        /// [H]C([H])=C([H])C([H])([H])C([H])=O
        /// </summary>
        // @cdk.inchi  InChI=1/C4H6O/c1-2-3-4-5/h2,4H,1,3H2
        // @cdk.bug   1959099
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestBondNotConjugated()
        {
            double[] testResult = { 0.0, 0.0004, 0.0, -0.0004, 0.0, 0.0, 0.0, 0.0, 0.0277, 0.0, -0.0277 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[H]C([H])=C([H])C([H])([H])C([H])=O");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 6, true);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);
                Assert.AreEqual(testResult[i], result, 0.03);
            }
        }

        /// <summary>
        /// [H]C([H])=C([H])C([H])([H])C([H])=O
        /// </summary>
        // @cdk.inchi InChI=1/C4H6O/c1-2-3-4-5/h2,4H,1,3H2
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestDifferentStarts()
        {
            var sp = CDK.SmilesParser;
            var mol1 = sp.ParseSmiles("C=CCC=O");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AddExplicitHydrogens(mol1);
            CDK.LonePairElectronChecker.Saturate(mol1);

            var mol2 = sp.ParseSmiles("O=CCC=C");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            AddExplicitHydrogens(mol2);
            CDK.LonePairElectronChecker.Saturate(mol2);

            var descriptor1 = CreateDescriptor(mol1);
            var descriptor2 = CreateDescriptor(mol2);

            for (int i = 0; i < 5; i++)
            {
                var result1 = descriptor1.Calculate(mol1.Atoms[i]).Value;
                var result2 = descriptor2.Calculate(mol2.Atoms[i]).Value;
                Assert.IsNotNull(result1);
                Assert.IsNotNull(result2);
                Assert.AreEqual(result1, result2, 0.0001);
            }
        }

        /// <summary>
        /// [H]C([H])=C([H])C([H])([H])[H]
        /// </summary>
        // @cdk.inchi  InChI=1/C3H6/c1-3-2/h3H,1H2,2H3
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestBondNotConjugated1()
        {
            double[] testResult = { 0.0, -0.0009, 0.0, 0.0009, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[H]C([H])=C([H])C([H])([H])[H]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 6, true);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);
                Assert.AreEqual(testResult[i], result, 0.02);
            }
        }

        /// <summary>
        /// [H]C([H])=C([H])[C+]([H])[H]
        /// </summary>
        // @cdk.bug   1959099
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestBondNotConjugated2()
        {
            double[] testResult = { 0.0, 0.25, 0.0, 0.0, 0.0, 0.25, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[H]C([H])=C([H])[C+]([H])[H]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 6, true);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);
                Assert.AreEqual(testResult[i], result, 0.29);
            }
        }

        /// <summary>
        /// c1ccc(cc1)N3c4ccccc4(c2ccccc23)
        /// </summary>
        // @cdk.bug   1959099
        // @cdk.inchi
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestLangCalculation()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("c1ccc(cc1)N3c4ccccc4(c2ccccc23)");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol, 6, true);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var result = descriptor.Calculate(mol.Atoms[i]).Value;
                Assert.IsNotNull(result);
            }
        }
    }
}
