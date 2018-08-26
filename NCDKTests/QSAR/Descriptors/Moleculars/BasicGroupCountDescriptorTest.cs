/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class BasicGroupCountDescriptorTest : MolecularDescriptorTest
    {
        public BasicGroupCountDescriptorTest()
        {
            SetDescriptor(typeof(BasicGroupCountDescriptor));
        }

        [TestMethod()]
        public void TestConstructor()
        {
            Assert.IsNotNull(new BasicGroupCountDescriptor());
        }

        [TestMethod()]
        public void TestAmine()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("NC");
            Result<int> result = (Result<int>)Descriptor.Calculate(mol).Value;
            Assert.AreEqual(1, result.Value);
        }

        // @cdk.inchi InChI=1S/C2H4N2/c1-4-2-3/h2-3H,1H2
        [TestMethod()]
        public void Test()
        {
            IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("N");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("N");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("H");
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("H");
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("H");
            mol.Atoms.Add(a7);
            IAtom a8 = builder.NewAtom("H");
            mol.Atoms.Add(a8);
            IBond b1 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a2, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a2, a8, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a4, a6, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = builder.NewBond(a4, a7, BondOrder.Single);
            mol.Bonds.Add(b7);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);

            Result<int> result = (Result<int>)Descriptor.Calculate(mol).Value;
            // two SMARTS matches
            Assert.AreEqual(2, result.Value);
        }
    }
}
