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
using NCDK.Aromaticities;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class ProtonAffinityHOSEDescriptorTest : AtomicDescriptorTest<ProtonAffinityHOSEDescriptor>
    {
        // @cdk.inchi InChI=1/C6H5Cl/c7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        public void TestAffinityDescriptor1()
        {
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Double);
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[6]).Value;
            var resultAccordingNIST = 753.1;

            Assert.AreEqual(resultAccordingNIST, result, 0.00001);
        }

        // @cdk.inchi InChI=1/C2H5Cl/c1-2-3/h2H2,1H3
        [TestMethod()]
        public void TestAffinityDescriptor2()
        {
            var mol = CDK.Builder.NewAtomContainer();
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.Atoms.Add(CDK.Builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(CDK.Builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            AddExplicitHydrogens(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Atoms[2]).Value;
            var resultAccordingNIST = 693.4;

            Assert.AreEqual(resultAccordingNIST, result, 0.00001);
        }
    }
}
