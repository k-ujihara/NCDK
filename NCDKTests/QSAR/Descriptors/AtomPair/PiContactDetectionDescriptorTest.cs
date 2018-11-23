/*
 * Copyright (C) 2018 Kazuya Ujihara <ujihara.kazuya@gmail.com>
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
using NCDK.Templates;

namespace NCDK.QSAR.Descriptors.AtomPair
{
    [TestClass()]
    public class PiContactDetectionDescriptorTest : DescriptorTest<PiContactDetectionDescriptor>
    {
        [TestMethod()]
        public void TestHexane()
        {
            // Make hexane
            var mol = TestMoleculeFactory.MakeAlkane(6);
            var descriptor = CreateDescriptor(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsFalse(descriptor.Calculate(mol.Atoms[0], atom).Value);
        }

        [TestMethod()]
        public void Test12diMecyclohexa13diene()
        {
            var mol = Make12diMecyclohexa13diene();
            var descriptor = CreateDescriptor(mol);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[0]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[1]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[2]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[3]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[4]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[5]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[6]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[7]).Value);
        }

        [TestMethod()]
        public void TestHexa13diene()
        {
            var mol = MakeHexa13diene();
            var descriptor = CreateDescriptor(mol);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[0]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[1]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[2]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[3]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[4]).Value);
            Assert.AreEqual(false, descriptor.Calculate(mol.Atoms[0], mol.Atoms[5]).Value);
        }

        [TestMethod()][Ignore()]
        public void TestHexa15diene()
        {
            var mol = MakeHexa15diene();
            var descriptor = CreateDescriptor(mol);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[0]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[1]).Value);
            Assert.AreEqual(true, descriptor.Calculate(mol.Atoms[0], mol.Atoms[2]).Value);
            Assert.AreEqual(false, descriptor.Calculate(mol.Atoms[0], mol.Atoms[3]).Value);
            Assert.AreEqual(false, descriptor.Calculate(mol.Atoms[0], mol.Atoms[4]).Value);
            Assert.AreEqual(false, descriptor.Calculate(mol.Atoms[0], mol.Atoms[5]).Value);
        }

        private static IAtomContainer MakeHexa13diene()
        {
            var molecule = CDK.Builder.NewAtomContainer();
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            return molecule;
        }

        private static IAtomContainer MakeHexa15diene()
        {
            var molecule = CDK.Builder.NewAtomContainer();
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Double);
            return molecule;
        }

        private static IAtomContainer Make12diMecyclohexa13diene()
        {
            var molecule = CDK.Builder.NewAtomContainer();
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.Atoms.Add(CDK.Builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[0], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[7], BondOrder.Single);
            return molecule;
        }
    }
}
