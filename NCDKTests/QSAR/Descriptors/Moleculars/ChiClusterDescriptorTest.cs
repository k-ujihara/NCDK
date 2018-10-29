using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.QSAR.Results;
using NCDK.Smiles;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    ///  TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class ChiClusterDescriptorTest : MolecularDescriptorTest
    {
        public ChiClusterDescriptorTest()
        {
            SetDescriptor(typeof(ChiClusterDescriptor));
        }

        [TestMethod()]
        public void TestDan64()
        {
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

            ArrayResult<double> ret = (ArrayResult<double>)Descriptor.Calculate(mol).Value;

            Assert.AreEqual(0.2887, ret[0], 0.0001);
            Assert.AreEqual(0.0000, ret[1], 0.0001);
            Assert.AreEqual(0.0000, ret[2], 0.0001);
            Assert.AreEqual(0.0000, ret[3], 0.0001);
            Assert.AreEqual(0.1667, ret[4], 0.0001);
            Assert.AreEqual(0.0000, ret[5], 0.0001);
            Assert.AreEqual(0.0000, ret[6], 0.0001);
            Assert.AreEqual(0.0000, ret[7], 0.0001);
        }

        [TestMethod()]
        public void TestDan154()
        {
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

            ArrayResult<double> ret = (ArrayResult<double>)Descriptor.Calculate(mol).Value;

            Assert.AreEqual(0.5774, ret[0], 0.0001);
            Assert.AreEqual(0.0000, ret[1], 0.0001);
            Assert.AreEqual(0.0000, ret[2], 0.0001);
            Assert.AreEqual(0.0000, ret[3], 0.0001);
            Assert.AreEqual(0.3780, ret[4], 0.0001);
            Assert.AreEqual(0.0000, ret[5], 0.0001);
            Assert.AreEqual(0.0000, ret[6], 0.0001);
            Assert.AreEqual(0.0000, ret[7], 0.0001);
        }

        [TestMethod()]
        public void TestDan248()
        {
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
            IAtom a7 = mol.Builder.NewAtom("C");
            a7.Point2D = new Vector2(-3.897114317029975, 2.249999999999999);
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.NewAtom("O");
            a8.Point2D = new Vector2(-1.2990381056766587, -2.25);
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.NewAtom("C");
            a9.Point2D = new Vector2(1.477211629518312, 1.2395277334996044);
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.NewAtom("C");
            a10.Point2D = new Vector2(0.5130302149885025, 2.909538931178863);
            mol.Atoms.Add(a10);
            IBond b1 = mol.Builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a5, a4, BondOrder.Double);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a6, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.NewBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.NewBond(a7, a5, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.NewBond(a8, a3, BondOrder.Double);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.NewBond(a9, a1, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.NewBond(a10, a1, BondOrder.Single);
            mol.Bonds.Add(b10);

            ArrayResult<double> ret = (ArrayResult<double>)Descriptor.Calculate(mol).Value;

            Assert.AreEqual(1.7845, ret[0], 0.0001);
            Assert.AreEqual(0.2500, ret[1], 0.0001);
            Assert.AreEqual(0.0000, ret[2], 0.0001);
            Assert.AreEqual(0.0000, ret[3], 0.0001);
            Assert.AreEqual(1.4946, ret[4], 0.0001);
            Assert.AreEqual(0.2500, ret[5], 0.0001);
            Assert.AreEqual(0.0000, ret[6], 0.0001);
            Assert.AreEqual(0.0000, ret[7], 0.0001);
        }

        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentMetal()
        {
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("CCCC[Sn](CCCC)(CCCC)c1cc(Cl)c(Nc2nc(C)nc(N(CCC)CC3CC3)c2Cl)c(Cl)c1");
            ArrayResult<double> ret = (ArrayResult<double>)Descriptor.Calculate(mol).Value;
            Assert.IsNotNull(ret);
        }
     
        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentPlatinum()
        {
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("CC1CN[Pt]2(N1)OC(=O)C(C)P(=O)(O)O2");
            var dummy = Descriptor.Calculate(mol).Value;
            if (dummy is ArrayResult<double> result)
                Assert.IsTrue(double.IsNaN(result[0]));
            else
                Assert.Fail();
        }

        //    [TestMethod()] public void TestDan277() {
        //
        //        IAtomContainer mol = null;
        //
        //        ChiClusterDescriptor desc = new ChiClusterDescriptor();
        //        ArrayResult<double> ret = (ArrayResult<double>) desc.Calculate(mol).Value;
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
