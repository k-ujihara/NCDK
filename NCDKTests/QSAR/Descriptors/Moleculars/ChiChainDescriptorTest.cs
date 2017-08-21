using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.QSAR.Results;
using NCDK.Smiles;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars {
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class ChiChainDescriptorTest : MolecularDescriptorTest {
        public ChiChainDescriptorTest() {
            SetDescriptor(typeof(ChiChainDescriptor));
        }

        [TestMethod()]
        public void TestDan64() {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.NewAtom("C");
            a1.Point2D = new Vector2(0.7500000000000004, 2.799038105676658);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("C");
            a2.Point2D = new Vector2(0.0, 1.5);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            a3.Point2D = new Vector2(0.0, 0.0);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("O");
            a4.Point2D = new Vector2(-1.2990381056766582, 0.7500000000000001);
            mol.Atoms.Add(a4);
            IBond b1 = mol.Builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a4, a2, BondOrder.Single);
            mol.Bonds.Add(b4);

            DoubleArrayResult ret = (DoubleArrayResult)Descriptor.Calculate(mol).Value;

            Assert.AreEqual(0.2887, ret[0], 0.0001);
            Assert.AreEqual(0.2887, ret[1], 0.0001);
            Assert.AreEqual(0.0000, ret[2], 0.0001);
            Assert.AreEqual(0.0000, ret[3], 0.0001);
            Assert.AreEqual(0.1667, ret[5], 0.0001);
            Assert.AreEqual(0.1667, ret[6], 0.0001);
            Assert.AreEqual(0.0000, ret[7], 0.0001);
            Assert.AreEqual(0.0000, ret[8], 0.0001);
        }

        [TestMethod()]
        public void TestDan80() {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.NewAtom("C");
            a1.Point2D = new Vector2(0.0, 1.5);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("C");
            a2.Point2D = new Vector2(0.0, 0.0);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            a3.Point2D = new Vector2(-1.4265847744427305, -0.46352549156242084);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("C");
            a4.Point2D = new Vector2(-2.3082626528814396, 0.7500000000000002);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("O");
            a5.Point2D = new Vector2(-1.42658477444273, 1.9635254915624212);
            mol.Atoms.Add(a5);
            IBond b1 = mol.Builder.NewBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a4, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a5, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a5, a1, BondOrder.Single);
            mol.Bonds.Add(b5);

            DoubleArrayResult ret = (DoubleArrayResult)Descriptor.Calculate(mol).Value;

            Assert.AreEqual(0.0000, ret[0], 0.0001);
            Assert.AreEqual(0.0000, ret[1], 0.0001);
            Assert.AreEqual(0.1768, ret[2], 0.0001);
            Assert.AreEqual(0.0000, ret[3], 0.0001);
            Assert.AreEqual(0.0000, ret[5], 0.0001);
            Assert.AreEqual(0.0000, ret[6], 0.0001);
            Assert.AreEqual(0.04536, ret[7], 0.00001);
            Assert.AreEqual(0.0000, ret[8], 0.0001);

        }

        [TestMethod()]
        public void TestDan81() {
            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.NewAtom("C");
            a1.Point2D = new Vector2(0.0, 1.5);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("C");
            a2.Point2D = new Vector2(0.0, 0.0);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            a3.Point2D = new Vector2(-1.4265847744427305, -0.46352549156242084);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("C");
            a4.Point2D = new Vector2(-2.3082626528814396, 0.7500000000000002);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("S");
            a5.Point2D = new Vector2(-1.42658477444273, 1.9635254915624212);
            mol.Atoms.Add(a5);
            IBond b1 = mol.Builder.NewBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a4, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a5, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a5, a1, BondOrder.Single);
            mol.Bonds.Add(b5);

            DoubleArrayResult ret = (DoubleArrayResult)Descriptor.Calculate(mol).Value;

            Assert.AreEqual(0.0000, ret[0], 0.0001);
            Assert.AreEqual(0.0000, ret[1], 0.0001);
            Assert.AreEqual(0.1768, ret[2], 0.0001);
            Assert.AreEqual(0.0000, ret[3], 0.0001);
            Assert.AreEqual(0.0000, ret[5], 0.0001);
            Assert.AreEqual(0.0000, ret[6], 0.0001);
            Assert.AreEqual(0.1361, ret[7], 0.0001);
            Assert.AreEqual(0.0000, ret[8], 0.0001);
        }

        [TestMethod()]
        public void TestDan82() {

            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.NewAtom("C");
            a1.Point2D = new Vector2(0.0, 1.5);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("C");
            a2.Point2D = new Vector2(0.0, 0.0);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            a3.Point2D = new Vector2(-1.4265847744427305, -0.46352549156242084);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("C");
            a4.Point2D = new Vector2(-2.3082626528814396, 0.7500000000000002);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("O");
            a5.Point2D = new Vector2(-1.42658477444273, 1.9635254915624212);
            mol.Atoms.Add(a5);
            IBond b1 = mol.Builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a3, a2, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a5, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a5, a1, BondOrder.Single);
            mol.Bonds.Add(b5);

            DoubleArrayResult ret = (DoubleArrayResult)Descriptor.Calculate(mol).Value;

            Assert.AreEqual(0.0000, ret[0], 0.0001);
            Assert.AreEqual(0.0000, ret[1], 0.0001);
            Assert.AreEqual(0.1768, ret[2], 0.0001);
            Assert.AreEqual(0.0000, ret[3], 0.0001);
            Assert.AreEqual(0.0000, ret[5], 0.0001);
            Assert.AreEqual(0.0000, ret[6], 0.0001);
            Assert.AreEqual(0.06804, ret[7], 0.00001);
            Assert.AreEqual(0.0000, ret[8], 0.0001);
        }

        [TestMethod()]
        public void TestDan154() {

            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.NewAtom("C");
            a1.Point2D = new Vector2(0.0, 1.5);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("C");
            a2.Point2D = new Vector2(0.0, 0.0);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            a3.Point2D = new Vector2(-1.2990381056766584, -0.7500000000000001);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("C");
            a4.Point2D = new Vector2(-2.598076211353316, -2.220446049250313E-16);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("C");
            a5.Point2D = new Vector2(-2.5980762113533165, 1.5);
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.NewAtom("C");
            a6.Point2D = new Vector2(-1.2990381056766582, 2.2500000000000004);
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.NewAtom("Cl");
            a7.Point2D = new Vector2(-1.2990381056766582, 3.7500000000000004);
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.NewAtom("Cl");
            a8.Point2D = new Vector2(1.2990381056766576, -0.7500000000000007);
            mol.Atoms.Add(a8);
            IBond b1 = mol.Builder.NewBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a4, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a5, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a6, a5, BondOrder.Double);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.NewBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.NewBond(a7, a6, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.NewBond(a8, a2, BondOrder.Single);
            mol.Bonds.Add(b8);

            DoubleArrayResult ret = (DoubleArrayResult)Descriptor.Calculate(mol).Value;

            Assert.AreEqual(0.0000, ret[0], 0.0001);
            Assert.AreEqual(0.0000, ret[1], 0.0001);
            Assert.AreEqual(0.0000, ret[2], 0.0001);
            Assert.AreEqual(0.08333, ret[3], 0.00001);
            Assert.AreEqual(0.0000, ret[5], 0.0001);
            Assert.AreEqual(0.0000, ret[6], 0.0001);
            Assert.AreEqual(0.0000, ret[7], 0.0001);
            Assert.AreEqual(0.02778, ret[8], 0.00001);
        }

        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentMetal() {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCCC[Sn](CCCC)(CCCC)c1cc(Cl)c(Nc2nc(C)nc(N(CCC)CC3CC3)c2Cl)c(Cl)c1");
            DoubleArrayResult ret = (DoubleArrayResult)Descriptor.Calculate(mol).Value;
            Assert.IsNotNull(ret);
        }

        // @cdk.bug 3023326
        [TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCovalentPlatinum()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CC1CN[Pt]2(N1)OC(=O)C(C)P(=O)(O)O2");
            var dummy = Descriptor.Calculate(mol).Value;
        }

        //    [TestMethod()] public void TestDan277() {
        //
        //        IAtomContainer mol = null;
        //
        //        ChiChainDescriptor desc = new ChiChainDescriptor();
        //        DoubleArrayResult ret = (DoubleArrayResult) desc.Calculate(mol).GetValue();
        //
        //        Assert.AreEqual(0.0000, ret[0], 0.0001);
        //        Assert.AreEqual(0.0000, ret[1], 0.0001);
        //        Assert.AreEqual(0.0000, ret[2], 0.0001);
        //        Assert.AreEqual(0.08333, ret[3], 0.00001);
        //        Assert.AreEqual(0.0000, ret[4], 0.0001);
        //        Assert.AreEqual(0.0000, ret[5], 0.0001);
        //        Assert.AreEqual(0.0000, ret[6], 0.0001);
        //        Assert.AreEqual(0.02778, ret[7], 0.00001);
        //    }
    }
}
