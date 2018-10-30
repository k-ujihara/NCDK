/* Copyright (C) 2011 Mark Rijnbeek <markr@ebi.ac.uk>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using NCDK.IO;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.IO;

namespace NCDK.Tautomers
{
    /// <summary>
    /// Tests generation of tautomers.
    /// </summary>
    // @author Mark Rijnbeek
    // @cdk.module test-tautomer
    [TestClass()]
    public class InChITautomerGeneratorTest : CDKTestCase
    {
        private SmilesParser smilesParser = CDK.SilentSmilesParser;
        private InChITautomerGenerator tautomerGenerator = new InChITautomerGenerator();

        public InChITautomerGeneratorTest()
                : base()
        { }

        private ICollection<IAtomContainer> UnitTestWithInChIProvided(string smiles, string inchi, int tautCountExpected)
        {
            var container = smilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            var tautomers = tautomerGenerator.GetTautomers(container, inchi);
            Assert.AreEqual(tautCountExpected, tautomers.Count);
            return tautomers;
        }

        private ICollection<IAtomContainer> UnitTestWithoutInChIProvided(string smiles, InChITautomerGenerator.Options flags, int tautCountExpected)
        {
            var container = smilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            InChITautomerGenerator tautegen = new InChITautomerGenerator(flags);
            var tautomers = tautegen.GetTautomers(container);
            Assert.AreEqual(tautCountExpected, tautomers.Count);
            return tautomers;
        }

        [TestMethod()]
        public void Test1()
        {
            UnitTestWithInChIProvided("NC1=CC(N)=NC(O)=N1", "InChI=1S/C4H6N4O/c5-2-1-3(6)8-4(9)7-2/h1H,(H5,5,6,7,8,9)", 5);
        }

        [TestMethod()]
        public void Test2()
        {
            UnitTestWithInChIProvided("CCCN1C2=C(NC=N2)C(=O)NC1=O", "InChI=1S/C8H10N4O2/c1-2-3-12-6-5(9-4-10-6)7(13)11-8(12)14/h4H,2-3H2,1H3,(H,9,10)(H,11,13,14)", 8);
        }

        [TestMethod()]
        public void Test3()
        {
            UnitTestWithInChIProvided("CCNC(=N)NC", "InChI=1S/C4H11N3/c1-3-7-4(5)6-2/h3H2,1-2H3,(H3,5,6,7)", 3);
        }

        [TestMethod()]
        public void Test4()
        {
            UnitTestWithInChIProvided("O=C1NC=CC(=O)N1", "InChI=1S/C4H4N2O2/c7-3-1-2-5-4(8)6-3/h1-2H,(H2,5,6,7,8)", 6);
        }

        [TestMethod()]
        public void Test5()
        {
            UnitTestWithInChIProvided("CCN1CCOC2=CC(NC3=NCCN3)=CC=C12", "InChI=1S/C13H18N4O/c1-2-17-7-8-18-12-9-10(3-4-11(12)17)16-13-14-5-6-15-13/h3-4,9H,2,5-8H2,1H3,(H2,14,15,16)", 2);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void Test6()
        {
            //Warfarin: not you need to create the InChI with option KET to get the ketone/hydroxyl tautomerism
            UnitTestWithInChIProvided("CC(=O)CC(C1=CC=CC=C1)C1=C(O)C2=C(OC1=O)C=CC=C2", "InChI=1/C19H16O4/c1-12(20)11-15(13-7-3-2-4-8-13)17-18(21)14-9-5-6-10-16(14)23-19(17)22/h2-10,15H,1H3,(H2,11,20)(H,17,21,22)", 6);
        }

        [TestMethod()]
        public void Test1_fast()
        {
            UnitTestWithoutInChIProvided("NC1=CC(N)=NC(O)=N1", 0, 5);
        }

        [TestMethod()]
        public void Test2_fast()
        {
            UnitTestWithoutInChIProvided("CCCN1C2=C(NC=N2)C(=O)NC1=O", 0, 8);
        }

        [TestMethod()]
        public void Test3_fast()
        {
            UnitTestWithoutInChIProvided("CCNC(=N)NC", 0, 3);
        }

        [TestMethod()]
        public void Test4_fast()
        {
            UnitTestWithoutInChIProvided("O=C1NC=CC(=O)N1", 0, 6);
        }

        [TestMethod()]
        public void Test5_fast()
        {
            UnitTestWithoutInChIProvided("CCN1CCOC2=CC(NC3=NCCN3)=CC=C12", 0, 2);
        }

        [TestMethod()]
        public void Test6_fast()
        {
            //Warfarin: not you need to create the InChI with option KET to get the ketone/hydroxyl tautomerism
            UnitTestWithoutInChIProvided("CC(=O)CC(C1=CC=CC=C1)C1=C(O)C2=C(OC1=O)C=CC=C2", InChITautomerGenerator.Options.KetoEnol, 6);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        // bail out on dots in formula
        public void TestFail1()
        {
            UnitTestWithInChIProvided("[I-].CCN1CCOC2=CC(NC3=NCCN3)=CC=C12", "InChI=1S/C13H18N4O.HI/c1-2-17-7-8-18-12-9-10(3-4-11(12)17)16-13-14-5-6-15-13;/h3-4,9H,2,5-8H2,1H3,(H2,14,15,16);1H/p-1", 2);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        // bail out on dots in formula
        public void TestFail2()
        {
            UnitTestWithInChIProvided("CN1C=C(C)C(=O)N2C1O[Pt]([NH3+])([NH3+])OC3N(C)C=C(C)C(=O)N3[Pt]2([NH3+])[NH3+]", "InChI=1S/2C6H9N2O2.4H3N.2Pt/c2*1-4-3-8(2)6(10)7-5(4)9;;;;;;/h2*3,6H,1-2H3,(H,7,9);4*1H3;;/q2*-1;;;;;2*+4/p-2", 10);
        }

        [TestMethod()]
        public void Test_WithNInChI()
        {
            string mdlInput = // same as NC1=CC(N)=NC(O)=N1
            "\n" + "  Mrv0541 02151109592D\n" + "\n" + "  9  9  0  0  0  0            999 V2000\n"
                    + "    2.1434   -0.4125    0.0000 N   0  0  0  0  0  0  0  0  0  0  0  0\n"
                    + "    1.4289   -0.0000    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n"
                    + "    0.7145   -0.4125    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n"
                    + "    0.0000   -0.0000    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n"
                    + "   -0.7145   -0.4125    0.0000 N   0  0  0  0  0  0  0  0  0  0  0  0\n"
                    + "    0.0000    0.8250    0.0000 N   0  0  0  0  0  0  0  0  0  0  0  0\n"
                    + "    0.7145    1.2375    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n"
                    + "    0.7145    2.0625    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0\n"
                    + "    1.4289    0.8250    0.0000 N   0  0  0  0  0  0  0  0  0  0  0  0\n" + "  1  2  1  0  0  0  0\n"
                    + "  2  3  2  0  0  0  0\n" + "  3  4  1  0  0  0  0\n" + "  4  5  1  0  0  0  0\n"
                    + "  4  6  2  0  0  0  0\n" + "  6  7  1  0  0  0  0\n" + "  7  8  1  0  0  0  0\n"
                    + "  7  9  2  0  0  0  0\n" + "  2  9  1  0  0  0  0\n" + "M  END\n";

            MDLV2000Reader reader = new MDLV2000Reader(new StringReader(mdlInput));
            IAtomContainer molecule = reader.Read(new AtomContainer());
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            var hAdder = CDK.HydrogenAdder;
            hAdder.AddImplicitHydrogens(molecule);

            var tautomers = tautomerGenerator.GetTautomers(molecule); // InChI will be calculated
            Assert.AreEqual(5, tautomers.Count);
        }

        [TestMethod()]
        public void TestAdenine()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("N");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("N");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("N");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("N");
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("N");
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            mol.Atoms.Add(a7);
            IAtom a8 = builder.NewAtom("C");
            mol.Atoms.Add(a8);
            IAtom a9 = builder.NewAtom("C");
            mol.Atoms.Add(a9);
            IAtom a10 = builder.NewAtom("C");
            mol.Atoms.Add(a10);
            IAtom a11 = builder.NewAtom("H");
            mol.Atoms.Add(a11);
            IAtom a12 = builder.NewAtom("H");
            mol.Atoms.Add(a12);
            IAtom a13 = builder.NewAtom("H");
            mol.Atoms.Add(a13);
            IAtom a14 = builder.NewAtom("H");
            mol.Atoms.Add(a14);
            IAtom a15 = builder.NewAtom("H");
            mol.Atoms.Add(a15);
            IBond b1 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a9, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a11, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a2, a7, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a2, a9, BondOrder.Double);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a3, a7, BondOrder.Double);
            mol.Bonds.Add(b6);
            IBond b7 = builder.NewBond(a3, a10, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = builder.NewBond(a4, a8, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = builder.NewBond(a4, a10, BondOrder.Double);
            mol.Bonds.Add(b9);
            IBond b10 = builder.NewBond(a5, a8, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = builder.NewBond(a5, a14, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = builder.NewBond(a5, a15, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = builder.NewBond(a6, a7, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = builder.NewBond(a6, a8, BondOrder.Double);
            mol.Bonds.Add(b14);
            IBond b15 = builder.NewBond(a9, a12, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = builder.NewBond(a10, a13, BondOrder.Single);
            mol.Bonds.Add(b16);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            var tautomers = tautomerGenerator.GetTautomers(mol);
            Assert.AreEqual(8, tautomers.Count);
        }
    }
}
