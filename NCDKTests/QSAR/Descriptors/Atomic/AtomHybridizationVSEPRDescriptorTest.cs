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
    public class AtomHybridizationVSEPRDescriptorTest : AtomicDescriptorTest<AtomHybridizationVSEPRDescriptor>
    {
        /// <summary>
        ///  O-C
        /// </summary>
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_1()
        {
            //O=CC
            var molecule = CDK.Builder.NewAtomContainer();
            var O1 = CDK.Builder.NewAtom("O");
            var c2 = CDK.Builder.NewAtom("C");
            c2.ImplicitHydrogenCount = 1;
            var c3 = CDK.Builder.NewAtom("C");
            c3.ImplicitHydrogenCount = 3;
            molecule.Atoms.Add(O1);
            molecule.Atoms.Add(c2);
            molecule.Atoms.Add(c3);
            var b1 = CDK.Builder.NewBond(c2, O1, BondOrder.Double);
            var b2 = CDK.Builder.NewBond(c2, c3, BondOrder.Single);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);
            AssertAtomTypesPerceived(molecule);

            var descriptor = CreateDescriptor(molecule);
            Assert.AreEqual(Hybridization.SP2, descriptor.Calculate(molecule.Atoms[0]).Value);
            Assert.AreEqual(Hybridization.SP2, descriptor.Calculate(molecule.Atoms[1]).Value);
            Assert.AreEqual(Hybridization.SP3, descriptor.Calculate(molecule.Atoms[2]).Value);
        }

         /// <summary>
         /// A unit test for JUnit with [O+]=C-C
         /// </summary>
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_2()
        {
            //[O+]#CC
            var molecule = CDK.Builder.NewAtomContainer();
            var O1 = CDK.Builder.NewAtom("O");
            O1.FormalCharge = 1;
            var c2 = CDK.Builder.NewAtom("C");
            var c3 = CDK.Builder.NewAtom("C");
            c3.ImplicitHydrogenCount = 3;
            molecule.Atoms.Add(O1);
            molecule.Atoms.Add(c2);
            molecule.Atoms.Add(c3);
            var b1 = CDK.Builder.NewBond(c2, O1, BondOrder.Triple);
            var b2 = CDK.Builder.NewBond(c2, c3, BondOrder.Single);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);
            AssertAtomTypesPerceived(molecule);

            var descriptor = CreateDescriptor(molecule);
            Assert.AreEqual(Hybridization.SP1, descriptor.Calculate(molecule.Atoms[0]).Value);
            Assert.AreEqual(Hybridization.SP1, descriptor.Calculate(molecule.Atoms[1]).Value);
            Assert.AreEqual(Hybridization.SP3, descriptor.Calculate(molecule.Atoms[2]).Value);
        }

        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_3()
        {
            //[C+]CC
            var molecule = CDK.Builder.NewAtomContainer();
            var c1 = CDK.Builder.NewAtom("C");
            c1.FormalCharge = 1;
            c1.ImplicitHydrogenCount = 2;
            var c2 = CDK.Builder.NewAtom("C");
            c2.ImplicitHydrogenCount = 2;
            var c3 = CDK.Builder.NewAtom("C");
            c3.ImplicitHydrogenCount = 3;
            molecule.Atoms.Add(c1);
            molecule.Atoms.Add(c2);
            molecule.Atoms.Add(c3);
            var b1 = CDK.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = CDK.Builder.NewBond(c2, c3, BondOrder.Single);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);
            AssertAtomTypesPerceived(molecule);

            var descriptor = CreateDescriptor(molecule);
            Assert.AreEqual(Hybridization.Planar3, descriptor.Calculate(molecule.Atoms[0]).Value);
            Assert.AreEqual(Hybridization.SP3, descriptor.Calculate(molecule.Atoms[1]).Value);
            Assert.AreEqual(Hybridization.SP3, descriptor.Calculate(molecule.Atoms[2]).Value);
        }

        // @cdk.bug 2323124
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_4()
        {
            //SO3
            var molecule = CDK.Builder.NewAtomContainer();
            var S1 = CDK.Builder.NewAtom("S");
            var O2 = CDK.Builder.NewAtom("O");
            var O3 = CDK.Builder.NewAtom("O");
            var O4 = CDK.Builder.NewAtom("O");
            molecule.Atoms.Add(S1);
            molecule.Atoms.Add(O2);
            molecule.Atoms.Add(O3);
            molecule.Atoms.Add(O4);
            var b1 = CDK.Builder.NewBond(S1, O2, BondOrder.Double);
            var b2 = CDK.Builder.NewBond(S1, O3, BondOrder.Double);
            var b3 = CDK.Builder.NewBond(S1, O4, BondOrder.Double);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);
            molecule.Bonds.Add(b3);
            AssertAtomTypesPerceived(molecule);

            var descriptor = CreateDescriptor(molecule);
            Assert.AreEqual(Hybridization.SP2, descriptor.Calculate(molecule.Atoms[0]).Value);
        }

        // @cdk.bug 2323133
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_5()
        {
            //XeF4
            var molecule = CDK.Builder.NewAtomContainer();
            var Xe1 = CDK.Builder.NewAtom("Xe");
            var F2 = CDK.Builder.NewAtom("F");
            var F3 = CDK.Builder.NewAtom("F");
            var F4 = CDK.Builder.NewAtom("F");
            var F5 = CDK.Builder.NewAtom("F");
            molecule.Atoms.Add(Xe1);
            molecule.Atoms.Add(F2);
            molecule.Atoms.Add(F3);
            molecule.Atoms.Add(F4);
            molecule.Atoms.Add(F5);
            var b1 = CDK.Builder.NewBond(Xe1, F2, BondOrder.Single);
            var b2 = CDK.Builder.NewBond(Xe1, F3, BondOrder.Single);
            var b3 = CDK.Builder.NewBond(Xe1, F4, BondOrder.Single);
            var b4 = CDK.Builder.NewBond(Xe1, F5, BondOrder.Single);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);
            molecule.Bonds.Add(b3);
            molecule.Bonds.Add(b4);
            AssertAtomTypesPerceived(molecule);

            var descriptor = CreateDescriptor(molecule);
            Assert.AreEqual(Hybridization.SP3D2, descriptor.Calculate(molecule.Atoms[0]).Value);
        }

        /// <summary>
        /// A unit test for JUnit with F-[I-]-F.
        /// </summary>
        // @cdk.bug 2323126
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_6()
        {
            //IF2-
            var molecule = CDK.Builder.NewAtomContainer();
            var I1 = CDK.Builder.NewAtom("I");
            I1.FormalCharge = -1;
            var F2 = CDK.Builder.NewAtom("F");
            var F3 = CDK.Builder.NewAtom("F");
            molecule.Atoms.Add(I1);
            molecule.Atoms.Add(F2);
            molecule.Atoms.Add(F3);
            var b1 = CDK.Builder.NewBond(I1, F2, BondOrder.Single);
            var b2 = CDK.Builder.NewBond(I1, F3, BondOrder.Single);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);
            AssertAtomTypesPerceived(molecule);

            var descriptor = CreateDescriptor(molecule);
            Assert.AreEqual(Hybridization.SP3D1, descriptor.Calculate(molecule.Atoms[0]).Value);
        }

        /// <summary>
        /// F-C=C
        /// </summary>
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_7()
        {
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            var testResult = new[] { Hybridization.SP3, Hybridization.SP2, Hybridization.SP2 };

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("F-C=C");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            AssertAtomTypesPerceived(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 3; i++)
                Assert.AreEqual(testResult[i], descriptor.Calculate(mol.Atoms[i]).Value);
        }

        /// <summary>
        /// [F+]=C-[C-]
        /// </summary>
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_8()
        {
            var testResult = new[]
            {
                Hybridization.SP2,
                Hybridization.SP2,
                Hybridization.SP3
            };

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[F+]=C-[C-]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AssertAtomTypesPerceived(mol);
            AddImplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            AssertAtomTypesPerceived(mol);

            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 3; i++)
                Assert.AreEqual(testResult[i], descriptor.Calculate(mol.Atoms[i]).Value);
        }
    }
}
