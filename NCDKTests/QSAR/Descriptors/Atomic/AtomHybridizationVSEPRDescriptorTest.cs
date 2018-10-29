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
    public class AtomHybridizationVSEPRDescriptorTest : AtomicDescriptorTest
    {
        public AtomHybridizationVSEPRDescriptorTest()
        {
            SetDescriptor(typeof(AtomHybridizationVSEPRDescriptor));
        }

        /// <summary>
        /// A unit test for JUnit with O-C
        /// </summary>
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_1()
        {
            AtomHybridizationVSEPRDescriptor descriptor = new AtomHybridizationVSEPRDescriptor();

            //O=CC
            IAtomContainer molecule = new AtomContainer();
            Atom O1 = new Atom("O");
            Atom c2 = new Atom("C");
            c2.ImplicitHydrogenCount = 1;
            Atom c3 = new Atom("C");
            c3.ImplicitHydrogenCount = 3;
            molecule.Atoms.Add(O1);
            molecule.Atoms.Add(c2);
            molecule.Atoms.Add(c3);
            Bond b1 = new Bond(c2, O1, BondOrder.Double);
            Bond b2 = new Bond(c2, c3, BondOrder.Single);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);

            AssertAtomTypesPerceived(molecule);
            Assert.AreEqual(Hybridization.SP2,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[0], molecule).Value).Value);
            Assert.AreEqual(Hybridization.SP2,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[1], molecule).Value).Value);
            Assert.AreEqual(Hybridization.SP3,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[2], molecule).Value).Value);
        }

         /// <summary>
         /// A unit test for JUnit with [O+]=C-C
         /// </summary>
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_2()
        {
            AtomHybridizationVSEPRDescriptor descriptor = new AtomHybridizationVSEPRDescriptor();

            //[O+]#CC
            IAtomContainer molecule = new AtomContainer();
            Atom O1 = new Atom("O");
            O1.FormalCharge = 1;
            Atom c2 = new Atom("C");
            Atom c3 = new Atom("C");
            c3.ImplicitHydrogenCount = 3;
            molecule.Atoms.Add(O1);
            molecule.Atoms.Add(c2);
            molecule.Atoms.Add(c3);
            Bond b1 = new Bond(c2, O1, BondOrder.Triple);
            Bond b2 = new Bond(c2, c3, BondOrder.Single);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);

            AssertAtomTypesPerceived(molecule);
            Assert.AreEqual(Hybridization.SP1,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[0], molecule).Value).Value);
            Assert.AreEqual(Hybridization.SP1,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[1], molecule).Value).Value);
            Assert.AreEqual(Hybridization.SP3,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[2], molecule).Value).Value);
        }

        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_3()
        {
            AtomHybridizationVSEPRDescriptor descriptor = new AtomHybridizationVSEPRDescriptor();

            //[C+]CC
            IAtomContainer molecule = new AtomContainer();
            Atom c1 = new Atom("C");
            c1.FormalCharge = 1;
            c1.ImplicitHydrogenCount = 2;
            Atom c2 = new Atom("C");
            c2.ImplicitHydrogenCount = 2;
            Atom c3 = new Atom("C");
            c3.ImplicitHydrogenCount = 3;
            molecule.Atoms.Add(c1);
            molecule.Atoms.Add(c2);
            molecule.Atoms.Add(c3);
            Bond b1 = new Bond(c1, c2, BondOrder.Single);
            Bond b2 = new Bond(c2, c3, BondOrder.Single);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);

            AssertAtomTypesPerceived(molecule);
            Assert.AreEqual(Hybridization.Planar3,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[0], molecule).Value).Value);
            Assert.AreEqual(Hybridization.SP3,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[1], molecule).Value).Value);
            Assert.AreEqual(Hybridization.SP3,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[2], molecule).Value).Value);
        }

        // @cdk.bug 2323124
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_4()
        {
            AtomHybridizationVSEPRDescriptor descriptor = new AtomHybridizationVSEPRDescriptor();

            //SO3
            IAtomContainer molecule = new AtomContainer();
            Atom S1 = new Atom("S");
            Atom O2 = new Atom("O");
            Atom O3 = new Atom("O");
            Atom O4 = new Atom("O");
            molecule.Atoms.Add(S1);
            molecule.Atoms.Add(O2);
            molecule.Atoms.Add(O3);
            molecule.Atoms.Add(O4);
            Bond b1 = new Bond(S1, O2, BondOrder.Double);
            Bond b2 = new Bond(S1, O3, BondOrder.Double);
            Bond b3 = new Bond(S1, O4, BondOrder.Double);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);
            molecule.Bonds.Add(b3);

            AssertAtomTypesPerceived(molecule);
            Assert.AreEqual(Hybridization.SP2,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[0], molecule).Value).Value);

        }

        // @cdk.bug 2323133
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_5()
        {
            AtomHybridizationVSEPRDescriptor descriptor = new AtomHybridizationVSEPRDescriptor();

            //XeF4
            IAtomContainer molecule = new AtomContainer();
            Atom Xe1 = new Atom("Xe");
            Atom F2 = new Atom("F");
            Atom F3 = new Atom("F");
            Atom F4 = new Atom("F");
            Atom F5 = new Atom("F");
            molecule.Atoms.Add(Xe1);
            molecule.Atoms.Add(F2);
            molecule.Atoms.Add(F3);
            molecule.Atoms.Add(F4);
            molecule.Atoms.Add(F5);
            Bond b1 = new Bond(Xe1, F2, BondOrder.Single);
            Bond b2 = new Bond(Xe1, F3, BondOrder.Single);
            Bond b3 = new Bond(Xe1, F4, BondOrder.Single);
            Bond b4 = new Bond(Xe1, F5, BondOrder.Single);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);
            molecule.Bonds.Add(b3);
            molecule.Bonds.Add(b4);

            AssertAtomTypesPerceived(molecule);
            Assert.AreEqual(Hybridization.SP3D2,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[0], molecule).Value).Value);
        }

        /// <summary>
        /// A unit test for JUnit with F-[I-]-F.
        /// </summary>
        // @cdk.bug 2323126
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_6()
        {
            AtomHybridizationVSEPRDescriptor descriptor = new AtomHybridizationVSEPRDescriptor();

            //IF2-
            IAtomContainer molecule = new AtomContainer();
            Atom I1 = new Atom("I");
            I1.FormalCharge = -1;
            Atom F2 = new Atom("F");
            Atom F3 = new Atom("F");
            molecule.Atoms.Add(I1);
            molecule.Atoms.Add(F2);
            molecule.Atoms.Add(F3);
            Bond b1 = new Bond(I1, F2, BondOrder.Single);
            Bond b2 = new Bond(I1, F3, BondOrder.Single);
            molecule.Bonds.Add(b1);
            molecule.Bonds.Add(b2);

            AssertAtomTypesPerceived(molecule);
            Assert.AreEqual(Hybridization.SP3D1,
                    ((Result<Hybridization>)descriptor.Calculate(molecule.Atoms[0], molecule).Value).Value);

        }

        /// <summary>
        /// A unit test with F-C=C
        /// </summary>
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_7()
        {
            Hybridization[] testResult = {Hybridization.SP3, Hybridization.SP2, Hybridization.SP2};
                // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            AtomHybridizationVSEPRDescriptor descriptor = new AtomHybridizationVSEPRDescriptor();

            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("F-C=C");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AssertAtomTypesPerceived(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            AssertAtomTypesPerceived(mol);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(testResult[i],
                        ((Result<Hybridization>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value);
            }
        }

        /// <summary>
        /// A unit test for JUnit with [F+]=C-[C-]
        /// </summary>
        [TestMethod()]
        public void TestAtomHybridizationVSEPRDescriptorTest_8()
        {
            Hybridization[] testResult = 
            {
                Hybridization.SP2,
                Hybridization.SP2,
                Hybridization.SP3
            };

            AtomHybridizationVSEPRDescriptor descriptor = new AtomHybridizationVSEPRDescriptor();

            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("[F+]=C-[C-]");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AssertAtomTypesPerceived(mol);
            AddImplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            AssertAtomTypesPerceived(mol);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(testResult[i],
                        ((Result<Hybridization>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value);
            }
        }
    }
}
