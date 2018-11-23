/* Copyright (C) 2006-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
    public class IPAtomicHOSEDescriptorTest : AtomicDescriptorTest<IPAtomicHOSEDescriptor>
    {
        /// <summary>
        /// CCCCl
        /// </summary>
        // @cdk.inchi InChI=1/C3H7Cl/c1-2-3-4/h2-3H2,1H3
        [TestMethod()]
        public void TestIPDescriptor1()
        {
            var mol = CDK.SmilesParser.ParseSmiles("CCCCl");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[3]).Value;
            var resultAccordingNIST = 10.8;
            Assert.AreEqual(resultAccordingNIST, result, 0.00001);
        }

        /// <summary>
        /// CC(C)Cl
        /// </summary>
        // @cdk.inchi InChI=1/C3H7Cl/c1-3(2)4/h3H,1-2H3
        [TestMethod()]
        public void TestIPDescriptor2()
        {
            var mol = CDK.SmilesParser.ParseSmiles("CC(CC)Cl"); // not in db
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[4]).Value;
            var resultAccordingNIST = 10.57; //value for CC(C)Cl
            Assert.AreEqual(resultAccordingNIST, result, 0.00001);
        }

        /// <summary>
        /// C=CCCl
        /// </summary>
        // @cdk.inchi InChI=1/C3H5Cl/c1-2-3-4/h2H,1,3H2
        [TestMethod()]
        public void TestNotDB()
        {
            var mol = CDK.SmilesParser.ParseSmiles("C=CCCl"); // not in db
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[3]).Value;
            var resultAccordingNIST = 10.8; //value for CCCCl aprox.
            Assert.AreEqual(resultAccordingNIST, result, 0.00001);
        }

        /// <summary>
        /// C-Cl
        /// </summary>
        // @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        public void TestIPDescriptor_1()
        {
            var mol = CDK.SmilesParser.ParseSmiles("C-Cl");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[1]).Value;
            var resultAccordingNIST = 11.26;

            Assert.AreEqual(resultAccordingNIST, result, 0.42);
        }

        /// <summary>
        /// C-C-Br
        /// </summary>
        [TestMethod()]
        public void TestIPDescriptor_2()
        {
            var mol = CDK.SmilesParser.ParseSmiles("C-C-Br");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[2]).Value;
            var resultAccordingNIST = 11.29;

            Assert.AreEqual(resultAccordingNIST, result, 1.95);
        }

        /// <summary>
        /// C-C-C-I
        /// </summary>
        [TestMethod()]
        public void TestIPDescriptor_3()
        {
            var mol = CDK.SmilesParser.ParseSmiles("C-C-C-I");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[3]).Value;
            var resultAccordingNIST = 9.27;

            Assert.AreEqual(resultAccordingNIST, result, 0.02);
        }
    }
}
