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
using NCDK.Numerics;
using NCDK.QSAR.Results;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class AcidicGroupCountDescriptorTest : MolecularDescriptorTest
    {
        public AcidicGroupCountDescriptorTest()
        {
            SetDescriptor(typeof(AcidicGroupCountDescriptor));
        }

        [TestMethod()]
        public void TestConstructor()
        {
            Assert.IsNotNull(new AcidicGroupCountDescriptor());
        }

        [TestMethod()]
        public void TestOneAcidGroup()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC(=O)O");
            Result<int> result = (Result<int>)Descriptor.Calculate(mol).Value;
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod()]
        public void TestSulphurAcidGroup()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("OS(=O)(=O)O");
            Result<int> result = (Result<int>)Descriptor.Calculate(mol).Value;
            Assert.AreEqual(2, result.Value);
        }

        [TestMethod()]
        public void TestPhosphorusAcidGroup()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=P(=O)O");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Result<int> result = (Result<int>)Descriptor.Calculate(mol).Value;
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod()]
        public void TestFancyGroup()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[NH](S(=O)=O)C(F)(F)F");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Result<int> result = (Result<int>)Descriptor.Calculate(mol).Value;
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod()]
        public void TestNitroRing()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[nH]1nnnc1");
            Result<int> result = (Result<int>)Descriptor.Calculate(mol).Value;
            Assert.AreEqual(2, result.Value);
        }

        // @cdk.inchi InChI=1S/C2H2N4O2/c7-2(8)1-3-5-6-4-1/h(H,7,8)(H,3,4,5,6)
        [TestMethod()]
        public void TestTwoGroup()
        {
            IAtomContainer mol = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("O");
            a1.FormalCharge = 0;
            a1.Point2D = new Vector2(5.9019, 0.5282);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("O");
            a2.FormalCharge = 0;
            a2.Point2D = new Vector2(5.3667, -1.1191);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("N");
            a3.FormalCharge = 0;
            a3.Point2D = new Vector2(3.3987, -0.4197);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("N");
            a4.FormalCharge = 0;
            a4.Point2D = new Vector2(2.5896, 0.1681);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("N");
            a5.FormalCharge = 0;
            a5.Point2D = new Vector2(3.8987, 1.1191);
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.NewAtom("N");
            a6.FormalCharge = 0;
            a6.Point2D = new Vector2(2.8987, 1.1191);
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.NewAtom("C");
            a7.FormalCharge = 0;
            a7.Point2D = new Vector2(4.2077, 0.1681);
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.NewAtom("C");
            a8.FormalCharge = 0;
            a8.Point2D = new Vector2(5.1588, -0.141);
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.NewAtom("H");
            a9.FormalCharge = 0;
            a9.Point2D = new Vector2(2.0, -0.0235);
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.NewAtom("H");
            a10.FormalCharge = 0;
            a10.Point2D = new Vector2(6.4916, 0.3366);
            mol.Atoms.Add(a10);
            IBond b1 = mol.Builder.NewBond(a1, a8, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a1, a10, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a2, a8, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a3, a7, BondOrder.Double);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.NewBond(a4, a6, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.NewBond(a4, a9, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.NewBond(a5, a6, BondOrder.Double);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.NewBond(a5, a7, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.NewBond(a7, a8, BondOrder.Single);
            mol.Bonds.Add(b10);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);

            Result<int> result = (Result<int>)Descriptor.Calculate(mol).Value;
            Assert.AreEqual(3, result.Value);
        }

        // @cdk.inchi InChI=1S/C6H12O10S/c7-2(1-16-17(13,14)15)3(8)4(9)5(10)6(11)12/h2-5,7-10H,1H2,(H,11,12)(H,13,14,15)/t2-,3-,4+,5-/m1/s1
        [TestMethod()]
        public void TestCID()
        {
            IAtomContainer mol = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("S");
            a1.FormalCharge = 0;
            a1.Point2D = new Vector2(9.4651, 0.25);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("O");
            a2.FormalCharge = 0;
            a2.Point2D = new Vector2(6.001, 1.25);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("O");
            a3.FormalCharge = 0;
            a3.Point2D = new Vector2(5.135, -1.25);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("O");
            a4.FormalCharge = 0;
            a4.Point2D = new Vector2(6.8671, -1.25);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("O");
            a5.FormalCharge = 0;
            a5.Point2D = new Vector2(8.5991, -0.25);
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.NewAtom("O");
            a6.FormalCharge = 0;
            a6.Point2D = new Vector2(4.269, 1.25);
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.NewAtom("O");
            a7.FormalCharge = 0;
            a7.Point2D = new Vector2(2.5369, 0.25);
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.NewAtom("O");
            a8.FormalCharge = 0;
            a8.Point2D = new Vector2(3.403, -1.25);
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.NewAtom("O");
            a9.FormalCharge = 0;
            a9.Point2D = new Vector2(10.3312, 0.75);
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.NewAtom("O");
            a10.FormalCharge = 0;
            a10.Point2D = new Vector2(9.9651, -0.616);
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.NewAtom("O");
            a11.FormalCharge = 0;
            a11.Point2D = new Vector2(8.9651, 1.116);
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.NewAtom("C");
            a12.FormalCharge = 0;
            a12.Point2D = new Vector2(6.001, 0.25);
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.NewAtom("C");
            a13.FormalCharge = 0;
            a13.Point2D = new Vector2(5.135, -0.25);
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.NewAtom("C");
            a14.FormalCharge = 0;
            a14.Point2D = new Vector2(6.8671, -0.25);
            mol.Atoms.Add(a14);
            IAtom a15 = mol.Builder.NewAtom("C");
            a15.FormalCharge = 0;
            a15.Point2D = new Vector2(4.269, 0.25);
            mol.Atoms.Add(a15);
            IAtom a16 = mol.Builder.NewAtom("C");
            a16.FormalCharge = 0;
            a16.Point2D = new Vector2(7.7331, 0.25);
            mol.Atoms.Add(a16);
            IAtom a17 = mol.Builder.NewAtom("C");
            a17.FormalCharge = 0;
            a17.Point2D = new Vector2(3.403, -0.25);
            mol.Atoms.Add(a17);
            IAtom a18 = mol.Builder.NewAtom("H");
            a18.FormalCharge = 0;
            a18.Point2D = new Vector2(6.538, 0.56);
            mol.Atoms.Add(a18);
            IAtom a19 = mol.Builder.NewAtom("H");
            a19.FormalCharge = 0;
            a19.Point2D = new Vector2(5.672, -0.56);
            mol.Atoms.Add(a19);
            IAtom a20 = mol.Builder.NewAtom("H");
            a20.FormalCharge = 0;
            a20.Point2D = new Vector2(6.3301, -0.56);
            mol.Atoms.Add(a20);
            IAtom a21 = mol.Builder.NewAtom("H");
            a21.FormalCharge = 0;
            a21.Point2D = new Vector2(4.8059, 0.56);
            mol.Atoms.Add(a21);
            IAtom a22 = mol.Builder.NewAtom("H");
            a22.FormalCharge = 0;
            a22.Point2D = new Vector2(8.1316, 0.7249);
            mol.Atoms.Add(a22);
            IAtom a23 = mol.Builder.NewAtom("H");
            a23.FormalCharge = 0;
            a23.Point2D = new Vector2(7.3346, 0.7249);
            mol.Atoms.Add(a23);
            IAtom a24 = mol.Builder.NewAtom("H");
            a24.FormalCharge = 0;
            a24.Point2D = new Vector2(6.538, 1.56);
            mol.Atoms.Add(a24);
            IAtom a25 = mol.Builder.NewAtom("H");
            a25.FormalCharge = 0;
            a25.Point2D = new Vector2(4.5981, -1.56);
            mol.Atoms.Add(a25);
            IAtom a26 = mol.Builder.NewAtom("H");
            a26.FormalCharge = 0;
            a26.Point2D = new Vector2(7.404, -1.56);
            mol.Atoms.Add(a26);
            IAtom a27 = mol.Builder.NewAtom("H");
            a27.FormalCharge = 0;
            a27.Point2D = new Vector2(3.732, 1.56);
            mol.Atoms.Add(a27);
            IAtom a28 = mol.Builder.NewAtom("H");
            a28.FormalCharge = 0;
            a28.Point2D = new Vector2(2.0, -0.06);
            mol.Atoms.Add(a28);
            IAtom a29 = mol.Builder.NewAtom("H");
            a29.FormalCharge = 0;
            a29.Point2D = new Vector2(10.8681, 0.44);
            mol.Atoms.Add(a29);
            IBond b1 = mol.Builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a1, a9, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a1, a10, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a1, a11, BondOrder.Double);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a2, a12, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.NewBond(a2, a24, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.NewBond(a3, a13, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.NewBond(a3, a25, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.NewBond(a4, a14, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.NewBond(a4, a26, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.NewBond(a5, a16, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.NewBond(a6, a15, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.NewBond(a6, a27, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = mol.Builder.NewBond(a7, a17, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = mol.Builder.NewBond(a7, a28, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = mol.Builder.NewBond(a8, a17, BondOrder.Double);
            mol.Bonds.Add(b16);
            IBond b17 = mol.Builder.NewBond(a9, a29, BondOrder.Single);
            mol.Bonds.Add(b17);
            IBond b18 = mol.Builder.NewBond(a12, a13, BondOrder.Single);
            mol.Bonds.Add(b18);
            IBond b19 = mol.Builder.NewBond(a12, a14, BondOrder.Single);
            mol.Bonds.Add(b19);
            IBond b20 = mol.Builder.NewBond(a12, a18, BondOrder.Single);
            mol.Bonds.Add(b20);
            IBond b21 = mol.Builder.NewBond(a13, a15, BondOrder.Single);
            mol.Bonds.Add(b21);
            IBond b22 = mol.Builder.NewBond(a13, a19, BondOrder.Single);
            mol.Bonds.Add(b22);
            IBond b23 = mol.Builder.NewBond(a14, a16, BondOrder.Single);
            mol.Bonds.Add(b23);
            IBond b24 = mol.Builder.NewBond(a14, a20, BondOrder.Single);
            mol.Bonds.Add(b24);
            IBond b25 = mol.Builder.NewBond(a15, a17, BondOrder.Single);
            mol.Bonds.Add(b25);
            IBond b26 = mol.Builder.NewBond(a15, a21, BondOrder.Single);
            mol.Bonds.Add(b26);
            IBond b27 = mol.Builder.NewBond(a16, a22, BondOrder.Single);
            mol.Bonds.Add(b27);
            IBond b28 = mol.Builder.NewBond(a16, a23, BondOrder.Single);
            mol.Bonds.Add(b28);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            Result<int> result = (Result<int>)Descriptor.Calculate(mol).Value;
            Assert.AreEqual(2, result.Value);
        }
    }
}
