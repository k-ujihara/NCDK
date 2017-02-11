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
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.QSAR.Result;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class HBondDonorCountDescriptorTest : MolecularDescriptorTest
    {
        public HBondDonorCountDescriptorTest()
        {
            SetDescriptor(typeof(HBondDonorCountDescriptor));
        }

        [TestMethod()]
        public void TestHBondDonorCountDescriptor()
        {
            Descriptor.Parameters = new object[] { true };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("Oc1ccccc1"); //
            Assert.AreEqual(1, ((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
        }

        // @cdk.bug   3133610
        // @cdk.inchi InChI=1S/C2H3N3/c1-3-2-5-4-1/h1-2H,(H,3,4,5)
        [TestMethod()]
        public void TestCID9257()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom a1 = builder.CreateAtom("N");
            a1.FormalCharge = 0;
            a1.Point3D = new Vector3(0.5509, 0.9639, 0.0);
            mol.Atoms.Add(a1);
            IAtom a2 = builder.CreateAtom("N");
            a2.FormalCharge = 0;
            a2.Point3D = new Vector3(1.1852, -0.2183, 1.0E-4);
            mol.Atoms.Add(a2);
            IAtom a3 = builder.CreateAtom("N");
            a3.FormalCharge = 0;
            a3.Point3D = new Vector3(-1.087, -0.4827, 2.0E-4);
            mol.Atoms.Add(a3);
            IAtom a4 = builder.CreateAtom("C");
            a4.FormalCharge = 0;
            a4.Point3D = new Vector3(-0.7991, 0.7981, -1.0E-4);
            mol.Atoms.Add(a4);
            IAtom a5 = builder.CreateAtom("C");
            a5.FormalCharge = 0;
            a5.Point3D = new Vector3(0.15, -1.0609, -2.0E-4);
            mol.Atoms.Add(a5);
            IAtom a6 = builder.CreateAtom("H");
            a6.FormalCharge = 0;
            a6.Point3D = new Vector3(1.094, 1.8191, 1.0E-4);
            mol.Atoms.Add(a6);
            IAtom a7 = builder.CreateAtom("H");
            a7.FormalCharge = 0;
            a7.Point3D = new Vector3(-1.4981, 1.6215, -2.0E-4);
            mol.Atoms.Add(a7);
            IAtom a8 = builder.CreateAtom("H");
            a8.FormalCharge = 0;
            a8.Point3D = new Vector3(0.3019, -2.13, -2.0E-4);
            mol.Atoms.Add(a8);
            IBond b1 = builder.CreateBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.CreateBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.CreateBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.CreateBond(a2, a5, BondOrder.Double);
            mol.Bonds.Add(b4);
            IBond b5 = builder.CreateBond(a3, a4, BondOrder.Double);
            mol.Bonds.Add(b5);
            IBond b6 = builder.CreateBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = builder.CreateBond(a4, a7, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = builder.CreateBond(a5, a8, BondOrder.Single);
            mol.Bonds.Add(b8);

            Assert.AreEqual(1, ((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
        }
    }
}
