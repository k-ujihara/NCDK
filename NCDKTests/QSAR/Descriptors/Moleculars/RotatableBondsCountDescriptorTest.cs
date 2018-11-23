/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Config;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class RotatableBondsCountDescriptorTest : MolecularDescriptorTest<RotatableBondsCountDescriptor>
    {
        [TestMethod()]
        public void TestRotatableBondsCount()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC2CCC(C1CCCCC1)CC2"); // molecule with 2 bridged cicloexane and 1 methyl
            Assert.AreEqual(2, CreateDescriptor(mol).Calculate(true, false).Value);
        }

        private IAtomContainer MakeEthane()
        {
            var container = CDK.Builder.NewAtomContainer();
            container.Atoms.Add(container.Builder.NewAtom(NaturalElements.Carbon.Element));
            container.Atoms.Add(container.Builder.NewAtom(NaturalElements.Carbon.Element));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            return container;
        }

        private IAtomContainer MakeButane()
        {
            var container = MakeEthane();
            container.Atoms.Add(container.Builder.NewAtom(NaturalElements.Carbon.Element));
            container.Atoms.Add(container.Builder.NewAtom(NaturalElements.Carbon.Element));
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[2], container.Atoms[3], BondOrder.Single);
            return container;
        }

        [TestMethod()]
        public void TestEthaneIncludeTerminals()
        {
            var container = MakeEthane();
            var result = CreateDescriptor(container).Calculate(true, false);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod()]
        public void TestEthane()
        {
            var container = MakeEthane();
            var result = CreateDescriptor(container).Calculate(false, false);
            Assert.AreEqual(0, result.Value);
        }

        [TestMethod()]
        public void TestButaneIncludeTerminals()
        {
            var container = MakeButane();
            var result = CreateDescriptor(container).Calculate(true, false);
            Assert.AreEqual(3, result.Value);
        }

        [TestMethod()]
        public void TestButane()
        {
            var container = MakeButane();
            var result = CreateDescriptor(container).Calculate(false, false);
            Assert.AreEqual(1, result.Value);
        }

        // @cdk.bug 2449257
        [TestMethod()]
        public void TestEthaneIncludeTerminalsExplicitH()
        {
            var container = MakeEthane();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            var adder = CDK.HydrogenAdder;
            adder.AddImplicitHydrogens(container);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
            var result = CreateDescriptor(container).Calculate(true, false);
            Assert.AreEqual(1, result.Value);
        }

        // @cdk.bug 2449257
        [TestMethod()]
        public void TestEthaneExplicitH()
        {
            var container = MakeEthane();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            var adder = CDK.HydrogenAdder;
            adder.AddImplicitHydrogens(container);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
            var result = CreateDescriptor(container).Calculate(false, false);
            Assert.AreEqual(0, result.Value);
        }

        // @cdk.bug 2449257
        [TestMethod()]
        public void TestButaneIncludeTerminalsExplicitH()
        {
            var container = MakeButane();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            var adder = CDK.HydrogenAdder;
            adder.AddImplicitHydrogens(container);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
            var result = CreateDescriptor(container).Calculate(true, false);
            Assert.AreEqual(3, result.Value);
        }

        // @cdk.bug 2449257
        [TestMethod()]
        public void TestButaneExplicitH()
        {
            var container = MakeButane();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            var adder = CDK.HydrogenAdder;
            adder.AddImplicitHydrogens(container);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
            var result = CreateDescriptor(container).Calculate(false, false);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod()]
        public void TestAmideIncluded()
        {
            var amide = "CCNC(=O)CC(C)C"; // N-ethyl-3-methylbutanamide
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles(amide);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            var result = CreateDescriptor(mol).Calculate(false, false);
            Assert.AreEqual(4, result.Value);
        }

        [TestMethod()]
        public void TestAmideExcluded()
        {
            var amide = "CCNC(=O)CC(C)C"; // N-ethyl-3-methylbutanamide
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles(amide);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            var result = CreateDescriptor(mol).Calculate(false, true);
            Assert.AreEqual(3, result.Value);
        }
    }
}
