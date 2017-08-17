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
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class HBondAcceptorCountDescriptorTest : MolecularDescriptorTest
    {
        public HBondAcceptorCountDescriptorTest()
        {
            SetDescriptor(typeof(HBondAcceptorCountDescriptor));
        }

        [TestMethod()]
        public void TestHBondAcceptorCountDescriptor()
        {
            Descriptor.Parameters = new object[] { true };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            // original molecule O=N(=O)c1cccc2cn[nH]c12 - correct kekulisation will give
            // the same result. this test though should depend on kekulisation working
            IAtomContainer mol = sp.ParseSmiles("O=N(=O)C1=C2NN=CC2=CC=C1");
            Assert.AreEqual(1, ((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
        }

        // @cdk.bug   3133610
        // @cdk.inchi InChI=1S/C2H3N3/c1-3-2-5-4-1/h1-2H,(H,3,4,5)
        [TestMethod()]
        public void TestCID9257()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("N");
            a1.FormalCharge = 0;
            a1.Point3D = new Vector3(0.5509, 0.9639, 0.0);
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("N");
            a2.FormalCharge = 0;
            a2.Point3D = new Vector3(1.1852, -0.2183, 1.0E-4);
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("N");
            a3.FormalCharge = 0;
            a3.Point3D = new Vector3(-1.087, -0.4827, 2.0E-4);
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            a4.Point3D = new Vector3(-0.7991, 0.7981, -1.0E-4);
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            a5.Point3D = new Vector3(0.15, -1.0609, -2.0E-4);
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("H");
            a6.FormalCharge = 0;
            a6.Point3D = new Vector3(1.094, 1.8191, 1.0E-4);
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("H");
            a7.FormalCharge = 0;
            a7.Point3D = new Vector3(-1.4981, 1.6215, -2.0E-4);
            mol.Atoms.Add(a7);
            IAtom a8 = builder.NewAtom("H");
            a8.FormalCharge = 0;
            a8.Point3D = new Vector3(0.3019, -2.13, -2.0E-4);
            mol.Atoms.Add(a8);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a2, a5, BondOrder.Double);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a3, a4, BondOrder.Double);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = builder.NewBond(a4, a7, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = builder.NewBond(a5, a8, BondOrder.Single);
            mol.Bonds.Add(b8);

            Descriptor.Parameters = new object[] { true };
            Assert.AreEqual(2, ((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
        }
    }
}
