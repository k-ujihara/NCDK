/* Copyright (C) 1997-2007  The Chemistry Development Kit (CKD) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All I ask is that proper credit is given for my work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Default;
using NCDK.IO;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System;
using System.IO;

namespace NCDK.Tools
{
    /// <summary>
    /// Tests the HOSECode generator.
    /// </summary>
    // @cdk.module  test-standard
    // @author      steinbeck
    // @cdk.created 2002-11-16
    [TestClass()]
    public class HOSECodeGeneratorTest : CDKTestCase
    {
        static bool standAlone = false;

        // @cdk.bug 968852
        [TestMethod()]
        public void Test968852()
        {
            string filename = "NCDK.Data.MDL.2,5-dimethyl-furan.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader.Read(Default.ChemObjectBuilder.Instance.CreateAtomContainer());
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            Aromaticity.CDKLegacy.Apply(mol1);
            Assert.AreEqual(new HOSECodeGenerator().GetHOSECode(mol1, mol1.Atoms[2], 6),
                    new HOSECodeGenerator().GetHOSECode(mol1, mol1.Atoms[3], 6));
        }

        [TestMethod()]
        public void TestSecondSphere()
        {
            string filename = "NCDK.Data.MDL.isopropylacetate.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader.Read(Default.ChemObjectBuilder.Instance.CreateAtomContainer());
            string code1 = new HOSECodeGenerator().GetHOSECode(mol1, mol1.Atoms[0], 6);
            filename = "NCDK.Data.MDL.testisopropylacetate.mol";
            Stream ins2 = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader2 = new MDLV2000Reader(ins2, ChemObjectReaderModes.Strict);
            IAtomContainer mol2 = reader2.Read(Default.ChemObjectBuilder.Instance.CreateAtomContainer());
            string code2 = new HOSECodeGenerator().GetHOSECode(mol2, mol2.Atoms[2], 6);
            Assert.AreNotSame(code2, code1);
        }

        [TestMethod()]
        public void Test1Sphere()
        {
            string[] result = {"O-1;=C(//)", "C-3;=OCC(//)", "C-3;=CC(//)", "C-3;=CC(//)", "C-3;*C*CC(//)", "C-3;*C*C(//)",
                "C-3;*C*C(//)", "C-3;*C*CC(//)", "C-3;*C*CC(//)", "C-3;*C*C(//)", "C-3;*C*C(//)", "C-3;*C*C(//)",
                "C-3;*C*C(//)", "C-3;*C*CO(//)", "O-2;CC(//)", "C-3;*C*CO(//)", "C-3;*C*CO(//)", "O-2;CC(//)",
                "C-4;O(//)", "C-3;*C*C(//)", "C-3;*C*CC(//)", "C-3;*C*C*C(//)", "C-3;*C*C*C(//)"};

            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("O");
            a1.Point2D = new Vector2(502.88457268119913, 730.4999999999999);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("C");
            a2.Point2D = new Vector2(502.8845726811991, 694.4999999999999);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            a3.Point2D = new Vector2(534.0614872174388, 676.4999999999999);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("C");
            a4.Point2D = new Vector2(534.0614872174388, 640.4999999999999);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("C");
            a5.Point2D = new Vector2(502.8845726811991, 622.4999999999999);
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("C");
            a6.Point2D = new Vector2(502.8845726811991, 586.4999999999999);
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("C");
            a7.Point2D = new Vector2(471.7076581449593, 568.4999999999999);
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("C");
            a8.Point2D = new Vector2(440.5307436087194, 586.5);
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.CreateAtom("C");
            a9.Point2D = new Vector2(409.35382907247964, 568.5);
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.CreateAtom("C");
            a10.Point2D = new Vector2(409.3538290724796, 532.5);
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.CreateAtom("C");
            a11.Point2D = new Vector2(378.1769145362398, 514.5);
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.CreateAtom("C");
            a12.Point2D = new Vector2(347.0, 532.5);
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.CreateAtom("C");
            a13.Point2D = new Vector2(347.0, 568.5);
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.CreateAtom("C");
            a14.Point2D = new Vector2(378.17691453623985, 586.5);
            mol.Atoms.Add(a14);
            IAtom a15 = mol.Builder.CreateAtom("O");
            a15.Point2D = new Vector2(378.17691453623985, 622.5);
            mol.Atoms.Add(a15);
            IAtom a16 = mol.Builder.CreateAtom("C");
            a16.Point2D = new Vector2(409.3538290724797, 640.5);
            mol.Atoms.Add(a16);
            IAtom a17 = mol.Builder.CreateAtom("C");
            a17.Point2D = new Vector2(409.3538290724797, 676.5);
            mol.Atoms.Add(a17);
            IAtom a18 = mol.Builder.CreateAtom("O");
            a18.Point2D = new Vector2(378.17691453623996, 694.5);
            mol.Atoms.Add(a18);
            IAtom a19 = mol.Builder.CreateAtom("C");
            a19.Point2D = new Vector2(378.17691453624, 730.5);
            mol.Atoms.Add(a19);
            IAtom a20 = mol.Builder.CreateAtom("C");
            a20.Point2D = new Vector2(440.5307436087195, 694.4999999999999);
            mol.Atoms.Add(a20);
            IAtom a21 = mol.Builder.CreateAtom("C");
            a21.Point2D = new Vector2(471.7076581449593, 676.4999999999999);
            mol.Atoms.Add(a21);
            IAtom a22 = mol.Builder.CreateAtom("C");
            a22.Point2D = new Vector2(471.7076581449593, 640.4999999999999);
            mol.Atoms.Add(a22);
            IAtom a23 = mol.Builder.CreateAtom("C");
            a23.Point2D = new Vector2(440.53074360871943, 622.4999999999999);
            mol.Atoms.Add(a23);
            IBond b1 = mol.Builder.CreateBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a4, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a5, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a6, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a7, a6, BondOrder.Double);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a8, a7, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.CreateBond(a9, a8, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.CreateBond(a10, a9, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.CreateBond(a11, a10, BondOrder.Double);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.CreateBond(a12, a11, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.CreateBond(a13, a12, BondOrder.Double);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.CreateBond(a14, a13, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = mol.Builder.CreateBond(a14, a9, BondOrder.Double);
            mol.Bonds.Add(b14);
            IBond b15 = mol.Builder.CreateBond(a15, a14, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = mol.Builder.CreateBond(a16, a15, BondOrder.Single);
            mol.Bonds.Add(b16);
            IBond b17 = mol.Builder.CreateBond(a17, a16, BondOrder.Double);
            mol.Bonds.Add(b17);
            IBond b18 = mol.Builder.CreateBond(a18, a17, BondOrder.Single);
            mol.Bonds.Add(b18);
            IBond b19 = mol.Builder.CreateBond(a19, a18, BondOrder.Single);
            mol.Bonds.Add(b19);
            IBond b20 = mol.Builder.CreateBond(a20, a17, BondOrder.Single);
            mol.Bonds.Add(b20);
            IBond b21 = mol.Builder.CreateBond(a21, a20, BondOrder.Double);
            mol.Bonds.Add(b21);
            IBond b22 = mol.Builder.CreateBond(a21, a2, BondOrder.Single);
            mol.Bonds.Add(b22);
            IBond b23 = mol.Builder.CreateBond(a22, a21, BondOrder.Single);
            mol.Bonds.Add(b23);
            IBond b24 = mol.Builder.CreateBond(a22, a5, BondOrder.Double);
            mol.Bonds.Add(b24);
            IBond b25 = mol.Builder.CreateBond(a23, a22, BondOrder.Single);
            mol.Bonds.Add(b25);
            IBond b26 = mol.Builder.CreateBond(a23, a16, BondOrder.Single);
            mol.Bonds.Add(b26);
            IBond b27 = mol.Builder.CreateBond(a23, a8, BondOrder.Double);
            mol.Bonds.Add(b27);

            AddImplicitHydrogens(mol);

            //MoleculeViewer2D.Display(molecule, true);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            HOSECodeGenerator hcg = new HOSECodeGenerator();
            string s = null;
            for (int f = 0; f < 23; f++)
            {
                s = hcg.GetHOSECode(mol, mol.Atoms[f], 1);
                if (standAlone) Console.Out.Write("|" + s + "| -> " + result[f]);
                Assert.AreEqual(result[f], s);
                if (standAlone) Console.Out.WriteLine("  OK");
            }
        }

        [TestMethod()]
        public void TestMakeBremserCompliant()
        {
            string[] startData = {"O-1;=C(//)", "C-3;=OCC(//)", "C-2;CC(//)", "C-2;CC(//)", "C-3;CCC(//)", "C-2;CC(//)",
                "C-2;CC(//)", "C-3;CCC(//)", "C-3;CCC(//)", "C-2;CC(//)", "C-2;CC(//)", "C-2;CC(//)", "C-2;CC(//)",
                "C-3;CCO(//)", "O-2;CC(//)", "C-3;CCO(//)", "C-3;CCO(//)", "O-2;CC(//)", "C-1;O(//)", "C-2;CC(//)",
                "C-3;CCC(//)", "C-3;CCC(//)", "C-3;CCC(//)"};

            string[] result = {"=C(//)", "=OCC(//)", "CC(//)", "CC(//)", "CCC(//)", "CC(//)", "CC(//)", "CCC(//)",
                "CCC(//)", "CC(//)", "CC(//)", "CC(//)", "CC(//)", "CCO(//)", "CC(//)", "CCO(//)", "CCO(//)", "CC(//)",
                "O(//)", "CC(//)", "CCC(//)", "CCC(//)", "CCC(//)"};

            string s = null;
            HOSECodeGenerator hcg = new HOSECodeGenerator();
            for (int f = 0; f < startData.Length; f++)
            {
                s = hcg.MakeBremserCompliant(startData[f]);
                if (standAlone) Console.Out.Write("|" + s + "| -> " + result[f]);
                Assert.AreEqual(result[f], s);
                if (standAlone) Console.Out.WriteLine("  OK");
            }
        }

        [TestMethod()]
        public void Test4Sphere()
        {
            string[] result = {

        "O-1;=C(CC/*C*C,=C/*C*C,*C,&)", "C-3;=OCC(,*C*C,=C/*C*C,*C,&/*C*C,*C&,*&O)",
                "C-3;=CC(C,=OC/*C*C,,*&*C/*C*&,*C,*C)", "C-3;=CC(C,*C*C/=OC,*C*&,*C/,*&*C,*C*C,*&)",
                "C-3;*C*CC(*C*C,*C,=C/*C*C,*CC,*&,&/*C,*&C,O,*&,=O&)", "C-3;*C*C(*CC,*C/*C*C,=C,*&C/*C*&,*CC,&,*C*C)",
                "C-3;*C*C(*CC,*C/*C*C,*C*C,*&C/*C*&,*CO,*C&,*C,=C)",
                "C-3;*C*CC(*C*C,*C,*C*C/*C*C,*CO,*&,*C&,*C/*CC,*&C,*&O,&,*C,*&)",
                "C-3;*C*CC(*CO,*C,*C*C/*C,C,*&,*C*&,*C/*&,*&*C,*C*C,*&)", "C-3;*C*C(*CC,*C/*CO,*C*C,*&/*&,C,*C*&,*C)",
                "C-3;*C*C(*C,*C/*CC,*&/*&O,*C*C)", "C-3;*C*C(*C,*C/*CO,*&/*&C,C)",
                "C-3;*C*C(*CO,*C/*CC,C,*&/*&,*C*C,*&*C)", "C-3;*C*CO(*CC,*C,C/*C,*C*C,*&,*&*C/*&,*C*&,*C,*CO)",
                "O-2;CC(*C*C,*C*C/*C*C,*CO,*C&,*C/*C*C,*C&,*&,C,*C,*&)",
                "C-3;*C*CO(*C*C,*CO,C/*C*C,*CC,*&,C,*&*C/*&C,*CC,*&,*&*C,,*C)",
                "C-3;*C*CO(*CO,*C,C/*C*C,C,*&C,/*&*C,*CC,*&*C,=OC)", "O-2;CC(*C*C,/*CO,*C/*C*C,C,*&C)",
                "C-4;O(C/*C*C/*CO,*C)", "C-3;*C*C(*CC,*CO/*C*C,=OC,*&O,C/*&*C,*CC,,=&,C,)",
                "C-3;*C*CC(*C*C,*C,=OC/*C*C,*CC,*&O,,=&/*&,*CC,O,*&,=&,C)",
                "C-3;*C*C*C(*C*C,*CC,*CC/*C,*CC,O,*&,=OC,*&,=&/*&O,*&,*C*C,&,,=&)",
                "C-3;*C*C*C(*C*C,*C,*CC,O/*CC,*CC,*&O,*&,*C*C,&/*&,=OC,*&,=&,C,*C&,*C)"};

            IAtomContainer mol = new AtomContainer();
            IAtom a1 = mol.Builder.CreateAtom("O");
            a1.Point2D = new Vector2(502.88457268119913, 730.4999999999999);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.CreateAtom("C");
            a2.Point2D = new Vector2(502.8845726811991, 694.4999999999999);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.CreateAtom("C");
            a3.Point2D = new Vector2(534.0614872174388, 676.4999999999999);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.CreateAtom("C");
            a4.Point2D = new Vector2(534.0614872174388, 640.4999999999999);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.CreateAtom("C");
            a5.Point2D = new Vector2(502.8845726811991, 622.4999999999999);
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.CreateAtom("C");
            a6.Point2D = new Vector2(502.8845726811991, 586.4999999999999);
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.CreateAtom("C");
            a7.Point2D = new Vector2(471.7076581449593, 568.4999999999999);
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.CreateAtom("C");
            a8.Point2D = new Vector2(440.5307436087194, 586.5);
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.CreateAtom("C");
            a9.Point2D = new Vector2(409.35382907247964, 568.5);
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.CreateAtom("C");
            a10.Point2D = new Vector2(409.3538290724796, 532.5);
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.CreateAtom("C");
            a11.Point2D = new Vector2(378.1769145362398, 514.5);
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.CreateAtom("C");
            a12.Point2D = new Vector2(347.0, 532.5);
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.CreateAtom("C");
            a13.Point2D = new Vector2(347.0, 568.5);
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.CreateAtom("C");
            a14.Point2D = new Vector2(378.17691453623985, 586.5);
            mol.Atoms.Add(a14);
            IAtom a15 = mol.Builder.CreateAtom("O");
            a15.Point2D = new Vector2(378.17691453623985, 622.5);
            mol.Atoms.Add(a15);
            IAtom a16 = mol.Builder.CreateAtom("C");
            a16.Point2D = new Vector2(409.3538290724797, 640.5);
            mol.Atoms.Add(a16);
            IAtom a17 = mol.Builder.CreateAtom("C");
            a17.Point2D = new Vector2(409.3538290724797, 676.5);
            mol.Atoms.Add(a17);
            IAtom a18 = mol.Builder.CreateAtom("O");
            a18.Point2D = new Vector2(378.17691453623996, 694.5);
            mol.Atoms.Add(a18);
            IAtom a19 = mol.Builder.CreateAtom("C");
            a19.Point2D = new Vector2(378.17691453624, 730.5);
            mol.Atoms.Add(a19);
            IAtom a20 = mol.Builder.CreateAtom("C");
            a20.Point2D = new Vector2(440.5307436087195, 694.4999999999999);
            mol.Atoms.Add(a20);
            IAtom a21 = mol.Builder.CreateAtom("C");
            a21.Point2D = new Vector2(471.7076581449593, 676.4999999999999);
            mol.Atoms.Add(a21);
            IAtom a22 = mol.Builder.CreateAtom("C");
            a22.Point2D = new Vector2(471.7076581449593, 640.4999999999999);
            mol.Atoms.Add(a22);
            IAtom a23 = mol.Builder.CreateAtom("C");
            a23.Point2D = new Vector2(440.53074360871943, 622.4999999999999);
            mol.Atoms.Add(a23);
            IBond b1 = mol.Builder.CreateBond(a2, a1, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.CreateBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.CreateBond(a4, a3, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.CreateBond(a5, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.CreateBond(a6, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.CreateBond(a7, a6, BondOrder.Double);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.CreateBond(a8, a7, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.CreateBond(a9, a8, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.CreateBond(a10, a9, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.CreateBond(a11, a10, BondOrder.Double);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.CreateBond(a12, a11, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.CreateBond(a13, a12, BondOrder.Double);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.CreateBond(a14, a13, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = mol.Builder.CreateBond(a14, a9, BondOrder.Double);
            mol.Bonds.Add(b14);
            IBond b15 = mol.Builder.CreateBond(a15, a14, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = mol.Builder.CreateBond(a16, a15, BondOrder.Single);
            mol.Bonds.Add(b16);
            IBond b17 = mol.Builder.CreateBond(a17, a16, BondOrder.Double);
            mol.Bonds.Add(b17);
            IBond b18 = mol.Builder.CreateBond(a18, a17, BondOrder.Single);
            mol.Bonds.Add(b18);
            IBond b19 = mol.Builder.CreateBond(a19, a18, BondOrder.Single);
            mol.Bonds.Add(b19);
            IBond b20 = mol.Builder.CreateBond(a20, a17, BondOrder.Single);
            mol.Bonds.Add(b20);
            IBond b21 = mol.Builder.CreateBond(a21, a20, BondOrder.Double);
            mol.Bonds.Add(b21);
            IBond b22 = mol.Builder.CreateBond(a21, a2, BondOrder.Single);
            mol.Bonds.Add(b22);
            IBond b23 = mol.Builder.CreateBond(a22, a21, BondOrder.Single);
            mol.Bonds.Add(b23);
            IBond b24 = mol.Builder.CreateBond(a22, a5, BondOrder.Double);
            mol.Bonds.Add(b24);
            IBond b25 = mol.Builder.CreateBond(a23, a22, BondOrder.Single);
            mol.Bonds.Add(b25);
            IBond b26 = mol.Builder.CreateBond(a23, a16, BondOrder.Single);
            mol.Bonds.Add(b26);
            IBond b27 = mol.Builder.CreateBond(a23, a8, BondOrder.Double);
            mol.Bonds.Add(b27);

            AddImplicitHydrogens(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            HOSECodeGenerator hcg = new HOSECodeGenerator();
            string s = null;
            for (int f = 0; f < mol.Atoms.Count; f++)
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
                Aromaticity.CDKLegacy.Apply(mol);
                s = hcg.GetHOSECode(mol, mol.Atoms[f], 4);
                if (standAlone) Console.Out.WriteLine(f + "|" + s + "| -> " + result[f]);
                Assert.AreEqual(result[f], s);
                if (standAlone) Console.Out.WriteLine("  OK");
            }
        }

        [TestMethod()]
        public void Test4()
        {
            string[] result = {"C-3;*C*C*C(*C*N,*C,*C/*C,*&,*&,*&/*&)", "C-3;*C*C(*C*C,*N/*C*&,*C,*&/*C,*&)",
                "C-3;*C*N(*C,*C/*&*C,*&*C/,*C,*C)", "N-3;*C*C(*C*C,*C/*C*&,*C,*&/*C,*&)",
                "C-3;*C*C*N(*C*C,*C,*C/*C,*&,*&,*&/*&)", "C-3;*C*C(*C*N,*C/*C*C,*C,*&/*&,*&,*&)",
                "C-3;*C*C(*C,*C/*C*N,*&/*&*C,*C)", "C-3;*C*C(*C,*C/*C*C,*&/*&*N,*C)",
                "C-3;*C*C(*C*C,*C/*C*N,*C,*&/*&,*&,*&)"};

            IAtomContainer molecule = (new SmilesParser(Default.ChemObjectBuilder.Instance))
                    .ParseSmiles("C1(C=CN2)=C2C=CC=C1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            //Display(molecule);
            HOSECodeGenerator hcg = new HOSECodeGenerator();
            string s = null;
            for (int f = 0; f < molecule.Atoms.Count; f++)
            {
                s = hcg.GetHOSECode(molecule, molecule.Atoms[f], 4);
                if (standAlone) Console.Out.WriteLine(f + "|" + s + "| -> " + result[f]);
                Assert.AreEqual(result[f], s);
                if (standAlone) Console.Out.WriteLine("  OK");
            }
        }

        // @cdk.bug 655169
        [TestMethod()]
        public void TestBug655169()
        {
            IAtomContainer molecule = null;
            HOSECodeGenerator hcg = null;
            string[] result = { "C-4;C(=C/Y/)", "C-3;=CC(Y,//)", "C-3;=CY(C,//)", "Br-1;C(=C/C/)" };

            molecule = (new SmilesParser(Default.ChemObjectBuilder.Instance)).ParseSmiles("CC=CBr");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            hcg = new HOSECodeGenerator();
            string s = null;
            for (int f = 0; f < molecule.Atoms.Count; f++)
            {
                s = hcg.GetHOSECode(molecule, molecule.Atoms[f], 4);
                if (standAlone) Console.Out.Write("|" + s + "| -> " + result[f]);
                Assert.AreEqual(result[f], s);
                if (standAlone) Console.Out.WriteLine("  OK");
            }
        }

        // @cdk.bug 795480
        [TestMethod()]
        public void TestBug795480()
        {
            IAtomContainer molecule = null;
            HOSECodeGenerator hcg = null;
            string[] result = { "C-4-;C(=C/Y'+4'/)", "C-3;=CC-(Y'+4',//)", "C-3;=CY'+4'(C-,//)", "Br-1'+4';C(=C/C-/)" };

            molecule = (new SmilesParser(Default.ChemObjectBuilder.Instance)).ParseSmiles("CC=CBr");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            bool isAromatic = Aromaticity.CDKLegacy.Apply(molecule);
            Assert.IsFalse(isAromatic);
            molecule.Atoms[0].FormalCharge = -1;
            molecule.Atoms[3].FormalCharge = +4;
            hcg = new HOSECodeGenerator();
            string s = null;
            for (int f = 0; f < molecule.Atoms.Count; f++)
            {
                s = hcg.GetHOSECode(molecule, molecule.Atoms[f], 4);
                if (standAlone) Console.Out.Write("|" + s + "| -> " + result[f]);
                Assert.AreEqual(result[f], s);
                if (standAlone) Console.Out.WriteLine("  OK");
            }
        }

        [TestMethod()]
        public void TestGetAtomsOfSphere()
        {
            IAtomContainer molecule = (new SmilesParser(Default.ChemObjectBuilder.Instance)).ParseSmiles("CC=CBr");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            HOSECodeGenerator hcg = new HOSECodeGenerator();

            hcg.GetSpheres(molecule, molecule.Atoms[0], 4, true);
          var atoms = hcg.GetNodesInSphere(3);

            Assert.AreEqual(1, atoms.Count);
            Assert.AreEqual("Br", atoms[0].Symbol);
        }

        [TestMethod()]
        public void TestGetAtomsOfSphereWithHydr()
        {
            IAtomContainer molecule = (new SmilesParser(Default.ChemObjectBuilder.Instance))
                    .ParseSmiles("C([H])([H])([H])C([H])=C([H])Br");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            HOSECodeGenerator hcg = new HOSECodeGenerator();

            hcg.GetSpheres(molecule, molecule.Atoms[0], 3, true);
            var atoms = hcg.GetNodesInSphere(3);

            Assert.AreEqual(2, atoms.Count);

            Assert.AreEqual("H", atoms[0].Symbol);
            Assert.AreEqual("Br", atoms[1].Symbol);
        }
    }
}
