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
using NCDK.Default;
using NCDK.QSAR.Result;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class RotatableBondsCountDescriptorTest : MolecularDescriptorTest
    {
        public RotatableBondsCountDescriptorTest()
        {
            SetDescriptor(typeof(RotatableBondsCountDescriptor));
        }

        [TestMethod()]
        public void TestRotatableBondsCount()
        {
            Descriptor.Parameters = new object[] { true, false };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CC2CCC(C1CCCCC1)CC2"); // molecule with 2 bridged cicloexane and 1 methyl
            Assert.AreEqual(2, ((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
        }

        private IAtomContainer MakeEthane()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.NewAtom(Elements.Carbon.ToIElement()));
            container.Atoms.Add(container.Builder.NewAtom(Elements.Carbon.ToIElement()));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            return container;
        }

        private IAtomContainer MakeButane()
        {
            IAtomContainer container = MakeEthane();
            container.Atoms.Add(container.Builder.NewAtom(Elements.Carbon.ToIElement()));
            container.Atoms.Add(container.Builder.NewAtom(Elements.Carbon.ToIElement()));
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[2], container.Atoms[3], BondOrder.Single);
            return container;
        }

        [TestMethod()]
        public void TestEthaneIncludeTerminals()
        {
            IAtomContainer container = MakeEthane();
            IMolecularDescriptor Descriptor = new RotatableBondsCountDescriptor();
            Descriptor.Parameters = new object[] { true, false };
            DescriptorValue result = Descriptor.Calculate(container);
            Assert.AreEqual(1, ((IntegerResult)result.GetValue()).Value);
        }

        [TestMethod()]
        public void TestEthane()
        {
            IAtomContainer container = MakeEthane();
            IMolecularDescriptor Descriptor = new RotatableBondsCountDescriptor();
            Descriptor.Parameters = new object[] { false, false };
            DescriptorValue result = Descriptor.Calculate(container);
            Assert.AreEqual(0, ((IntegerResult)result.GetValue()).Value);
        }

        [TestMethod()]
        public void TestButaneIncludeTerminals()
        {
            IAtomContainer container = MakeButane();
            IMolecularDescriptor Descriptor = new RotatableBondsCountDescriptor();
            Descriptor.Parameters = new object[] { true, false };
            DescriptorValue result = Descriptor.Calculate(container);
            Assert.AreEqual(3, ((IntegerResult)result.GetValue()).Value);
        }

        [TestMethod()]
        public void TestButane()
        {
            IAtomContainer container = MakeButane();
            IMolecularDescriptor Descriptor = new RotatableBondsCountDescriptor();
            Descriptor.Parameters = new object[] { false, false };
            DescriptorValue result = Descriptor.Calculate(container);
            Assert.AreEqual(1, ((IntegerResult)result.GetValue()).Value);
        }

        // @cdk.bug 2449257
        [TestMethod()]
        public void TestEthaneIncludeTerminalsExplicitH()
        {
            IAtomContainer container = MakeEthane();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(container.Builder);
            adder.AddImplicitHydrogens(container);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
            IMolecularDescriptor Descriptor = new RotatableBondsCountDescriptor();
            Descriptor.Parameters = new object[] { true, false };
            DescriptorValue result = Descriptor.Calculate(container);
            Assert.AreEqual(1, ((IntegerResult)result.GetValue()).Value);
        }

        // @cdk.bug 2449257
        [TestMethod()]
        public void TestEthaneExplicitH()
        {
            IAtomContainer container = MakeEthane();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(container.Builder);
            adder.AddImplicitHydrogens(container);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
            IMolecularDescriptor Descriptor = new RotatableBondsCountDescriptor();
            Descriptor.Parameters = new object[] { false, false };
            DescriptorValue result = Descriptor.Calculate(container);
            Assert.AreEqual(0, ((IntegerResult)result.GetValue()).Value);
        }

        // @cdk.bug 2449257
        [TestMethod()]
        public void TestButaneIncludeTerminalsExplicitH()
        {
            IAtomContainer container = MakeButane();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(container.Builder);
            adder.AddImplicitHydrogens(container);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
            IMolecularDescriptor Descriptor = new RotatableBondsCountDescriptor();
            Descriptor.Parameters = new object[] { true, false };
            DescriptorValue result = Descriptor.Calculate(container);
            Assert.AreEqual(3, ((IntegerResult)result.GetValue()).Value);
        }

        // @cdk.bug 2449257
        [TestMethod()]
        public void TestButaneExplicitH()
        {
            IAtomContainer container = MakeButane();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(container.Builder);
            adder.AddImplicitHydrogens(container);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
            IMolecularDescriptor Descriptor = new RotatableBondsCountDescriptor();
            Descriptor.Parameters = new object[] { false, false };
            DescriptorValue result = Descriptor.Calculate(container);
            Assert.AreEqual(1, ((IntegerResult)result.GetValue()).Value);
        }

        [TestMethod()]
        public void TestAmideIncluded()
        {
            string amide = "CCNC(=O)CC(C)C"; // N-ethyl-3-methylbutanamide
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles(amide);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            IMolecularDescriptor Descriptor = new RotatableBondsCountDescriptor();
            Descriptor.Parameters = new object[] { false, false };
            DescriptorValue result = Descriptor.Calculate(mol);
            Assert.AreEqual(4, ((IntegerResult)result.GetValue()).Value);
        }

        [TestMethod()]
        public void TestAmideExcluded()
        {
            string amide = "CCNC(=O)CC(C)C"; // N-ethyl-3-methylbutanamide
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles(amide);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            IMolecularDescriptor Descriptor = new RotatableBondsCountDescriptor();
            Descriptor.Parameters = new object[] { false, true };
            DescriptorValue result = Descriptor.Calculate(mol);
            Assert.AreEqual(3, ((IntegerResult)result.GetValue()).Value);
        }
    }
}
