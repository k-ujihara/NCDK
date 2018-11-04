/* Copyright (C) 1997-2007  The Chemistry Development Kit (CKD) project
 *               2009,2011  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
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
using NCDK.Aromaticities;
using NCDK.Common.Collections;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System.Collections;

namespace NCDK.Fingerprints
{
    // @cdk.module test-fingerprint
    [TestClass()]
    public class SubstructureFingerprinterTest : AbstractFixedLengthFingerprinterTest
    {
        public override IFingerprinter GetBitFingerprinter()
        {
            return new SubstructureFingerprinter();
        }

        [TestMethod()]
        public void TestSize()
        {
            SubstructureFingerprinter fp;

            fp = new SubstructureFingerprinter();
            Assert.AreEqual(307, fp.Length);

            fp = new SubstructureFingerprinter(StandardSubstructureSets.GetFunctionalGroupSMARTS());
            Assert.AreEqual(307, fp.Length);

            fp = new SubstructureFingerprinter(StandardSubstructureSets.GetCountableMACCSSMARTS());
            Assert.AreEqual(142, fp.Length);
        }

        [TestMethod()]
        public override void TestBug706786()
        {
            IAtomContainer superStructure = Bug706786_1();
            IAtomContainer subStructure = Bug706786_2();

            AddImplicitHydrogens(superStructure);
            AddImplicitHydrogens(subStructure);

            // SMARTS is now correct and D will include H atoms, CDK had this wrong
            // for years (had it has non-H count). Whilst you can set the optional
            // SMARTS flavor CDK_LEGACY this is not correct
            AtomContainerManipulator.SuppressHydrogens(superStructure);
            AtomContainerManipulator.SuppressHydrogens(subStructure);

            IFingerprinter fpr = GetBitFingerprinter();
            IBitFingerprint superBits = fpr.GetBitFingerprint(superStructure);
            IBitFingerprint subBits = fpr.GetBitFingerprint(subStructure);

            Assert.IsTrue(BitArrays.Equals(
                AsBitSet(0, 11, 13, 17, 40, 48, 136, 273, 274, 278, 286, 294, 299, 301, 304, 306),
                superBits.AsBitSet()));
            Assert.IsTrue(BitArrays.Equals(
                AsBitSet(1, 17, 273, 274, 278, 294, 306),
                subBits.AsBitSet()));
        }

        [TestMethod()]
        public void TestUserFunctionalGroups()
        {
            string[] smarts = { "c1ccccc1", "[CX4H3][#6]", "[CX2]#[CX2]" };
            var printer = new SubstructureFingerprinter(smarts);
            Assert.AreEqual(3, printer.Length);

            var sp = CDK.SmilesParser;
            var mol1 = sp.ParseSmiles("c1ccccc1CCC");
            IBitFingerprint fp = printer.GetBitFingerprint(mol1);
            Assert.IsNotNull(fp);

            Assert.IsTrue(fp[0]);
            Assert.IsTrue(fp[1]);
            Assert.IsFalse(fp[2]);

            mol1 = sp.ParseSmiles("C=C=C");
            fp = printer.GetBitFingerprint(mol1);
            Assert.IsNotNull(fp);
            Assert.IsFalse(fp[0]);
            Assert.IsFalse(fp[1]);
            Assert.IsFalse(fp[2]);
        }

        [TestMethod()]
        public void TestFunctionalGroupsBinary()
        {
            var printer = new SubstructureFingerprinter();
            Assert.AreEqual(307, printer.Length);

            var sp = CDK.SmilesParser;
            var mol1 = sp.ParseSmiles("c1ccccc1CCC");
            IBitFingerprint fp = printer.GetBitFingerprint(mol1);
            Assert.IsNotNull(fp);
            Assert.IsTrue(fp[273]);
            Assert.IsTrue(fp[0]);
            Assert.IsTrue(fp[1]);
            Assert.IsFalse(fp[100]);
        }

        [TestMethod()]
        public void TestFunctionalGroupsCount()
        {
            // TODO: Implement tests
        }

        [TestMethod()]
        public void TestCountableMACCSBinary()
        {
            // Tests are modified copy of the test included in the MACCS-FPs class
            var parser = CDK.SmilesParser;
            var printer = new SubstructureFingerprinter(StandardSubstructureSets.GetCountableMACCSSMARTS());
            Assert.AreEqual(142, printer.Length);
            var mol0 = parser.ParseSmiles("CC(N)CCCN");
            var mol1 = parser.ParseSmiles("c1ccccc1CCc1ccccc1");
            var mol2 = parser.ParseSmiles("c1ccccc1CC");
            var mol3 = parser.ParseSmiles("CC(N)CCC");
            var mol4 = parser.ParseSmiles("CCCC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol0);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol3);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol4);
            Aromaticity.CDKLegacy.Apply(mol0);
            Aromaticity.CDKLegacy.Apply(mol1);
            Aromaticity.CDKLegacy.Apply(mol2);
            Aromaticity.CDKLegacy.Apply(mol3);
            Aromaticity.CDKLegacy.Apply(mol4);
            var bs0 = printer.GetBitFingerprint(mol0).AsBitSet();
            var bs1 = printer.GetBitFingerprint(mol1).AsBitSet();
            var bs2 = printer.GetBitFingerprint(mol2).AsBitSet();
            var bs3 = printer.GetBitFingerprint(mol3).AsBitSet();
            var bs4 = printer.GetBitFingerprint(mol4).AsBitSet();
            // Check for the aromatic 6M rings
            Assert.IsFalse(bs0[111]);
            Assert.IsTrue(bs1[111]);
            Assert.IsTrue(bs2[111]);
            Assert.IsFalse(bs3[111]);
            Assert.IsFalse(bs4[111]);
            // Check for the fingerprints being subsets
            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs2));
            Assert.IsFalse(FingerprinterTool.IsSubset(bs0, bs3));
            Assert.IsTrue(FingerprinterTool.IsSubset(bs3, bs4));
        }

        [TestMethod()]
        public void TestCountableMACCSBinary2()
        {
            var parser = CDK.SmilesParser;
            var printer = new SubstructureFingerprinter(StandardSubstructureSets.GetCountableMACCSSMARTS());
            IAtomContainer mol;
            BitArray bs;
            // Test molecule 1
            mol = parser.ParseSmiles("C([S](O)(=O)=O)C1=C(C=CC=C1)CCCC[N+](=O)[O-]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            bs = printer.GetBitFingerprint(mol).AsBitSet();
            Assert.IsTrue(bs[46]);
            Assert.IsTrue(bs[27]);
            Assert.IsTrue(bs[59]);
            Assert.IsTrue(bs[49]);
            Assert.IsTrue(bs[111]);
            Assert.IsTrue(bs[129]);
            Assert.IsTrue(bs[115]);
            Assert.IsTrue(bs[120]);
            Assert.IsTrue(bs[41]);
            Assert.IsFalse(bs[93]);
            Assert.IsFalse(bs[91]);
            Assert.IsFalse(bs[24]);
            // Test molecule 2: Diatrizoic acid
            mol = parser.ParseSmiles("CC(=O)NC1=C(C(=C(C(=C1I)C(=O)O)I)NC(=O)C)I");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            bs = printer.GetBitFingerprint(mol).AsBitSet();
            Assert.IsTrue(bs[15]);
            Assert.IsTrue(bs[135]);
            Assert.IsTrue(bs[139]);
            Assert.IsTrue(bs[93]);
            Assert.IsTrue(bs[73]);
            Assert.IsFalse(bs[91]);
        }

        [TestMethod()]
        public override void TestGetCountFingerprint()
        {
            // See other function for specific test cases
        }

        [TestMethod()]
        public void TestCountableMACCSCount2()
        {
            var parser = CDK.SmilesParser;
            var printer = new SubstructureFingerprinter(StandardSubstructureSets.GetCountableMACCSSMARTS());
            IAtomContainer mol;
            ICountFingerprint cfp;
            // Test molecule 1
            mol = parser.ParseSmiles("C([S](O)(=O)=O)C1=C(C=CC=C1)CCCC[N+](=O)[O-]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            cfp = printer.GetCountFingerprint(mol);
            Assert.IsTrue(cfp.GetCountForHash(46) == 2);
            Assert.IsTrue(cfp.GetCountForHash(27) == 1);
            Assert.IsTrue(cfp.GetCountForHash(59) == 2);
            Assert.IsTrue(cfp.GetCountForHash(49) == 1);
            Assert.IsTrue(cfp.GetCountForHash(111) == 1);
            Assert.IsTrue(cfp.GetCountForHash(129) == 3);
            Assert.IsTrue(cfp.GetCountForHash(115) == 2);
            Assert.IsTrue(cfp.GetCountForHash(120) == 3);
            Assert.IsTrue(cfp.GetCountForHash(41) == 3);
            Assert.IsTrue(cfp.GetCountForHash(93) == 0);
            Assert.IsTrue(cfp.GetCountForHash(91) == 0);
            Assert.IsTrue(cfp.GetCountForHash(24) == 0);
            // Test molecule 2: Diatrizoic acid
            mol = parser.ParseSmiles("CC(=O)NC1=C(C(=C(C(=C1I)C(=O)O)I)NC(=O)C)I");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            cfp = printer.GetCountFingerprint(mol);
            Assert.IsTrue(cfp.GetCountForHash(15) == 3);
            Assert.IsTrue(cfp.GetCountForHash(135) == 3);
            Assert.IsTrue(cfp.GetCountForHash(139) == 4);
            Assert.IsTrue(cfp.GetCountForHash(93) == 3);
            Assert.IsTrue(cfp.GetCountForHash(73) == 6);
            Assert.IsTrue(cfp.GetCountForHash(91) == 0);
        }

        [TestMethod()]
        public void TestCountableMACCSCount_Rings()
        {
            var parser = CDK.SmilesParser;
            var printer = new SubstructureFingerprinter(StandardSubstructureSets.GetCountableMACCSSMARTS());
            IAtomContainer mol;
            ICountFingerprint cfp;
            // Aromatic 6-rings
            mol = parser.ParseSmiles("C1=CC=CC(=C1)CCCC2=CC=CC=C2");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            cfp = printer.GetCountFingerprint(mol);
            Assert.IsTrue(cfp.GetCountForHash(128) == 2); // 6-ring
            Assert.IsTrue(cfp.GetCountForHash(111) == 2); // aromaticity
            Assert.IsTrue(cfp.GetCountForHash(7) == 0); // 7-ring
            Assert.IsTrue(cfp.GetCountForHash(82) == 0); // 5-ring
                                                         // Non-aromatic 6-rings
            mol = parser.ParseSmiles("C1CC(CCC1)CCCCC2CCCCC2");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            cfp = printer.GetCountFingerprint(mol);
            Assert.IsTrue(cfp.GetCountForHash(128) == 2); // 6-ring
            Assert.IsTrue(cfp.GetCountForHash(111) == 0); // aromaticity
            Assert.IsTrue(cfp.GetCountForHash(7) == 0); // 7-ring
            Assert.IsTrue(cfp.GetCountForHash(82) == 0); // 5-ring
                                                         // Aromatic 6-ring, 3-ring and 4-ring
            mol = parser.ParseSmiles("C1CC1C(CCC2CCC2)CC3=CC=CC=C3");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            cfp = printer.GetCountFingerprint(mol);
            Assert.IsTrue(cfp.GetCountForHash(128) == 1); // 6-ring
            Assert.IsTrue(cfp.GetCountForHash(111) == 1); // aromaticity
            Assert.IsTrue(cfp.GetCountForHash(10) == 1); // 3-ring
            Assert.IsTrue(cfp.GetCountForHash(1) == 1); // 4-ring
            Assert.IsTrue(cfp.GetCountForHash(7) == 0); // 7-ring
            Assert.IsTrue(cfp.GetCountForHash(82) == 0); // 5-ring
                                                         // Aromatic 6-ring, 3-ring and 4-ring
            mol = parser.ParseSmiles("C1(CC1C(CCC2CCC2)CC3=CC=CC=C3)C(C(C(C4CC4)C5CC5)C6CC6)C7CC7");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            cfp = printer.GetCountFingerprint(mol);
            Assert.IsTrue(cfp.GetCountForHash(128) == 1); // 6-ring
            Assert.IsTrue(cfp.GetCountForHash(111) == 1); // aromaticity
            Assert.IsTrue(cfp.GetCountForHash(10) == 5); // 3-ring
            Assert.IsTrue(cfp.GetCountForHash(1) == 1); // 4-ring
            Assert.IsTrue(cfp.GetCountForHash(7) == 0); // 7-ring
            Assert.IsTrue(cfp.GetCountForHash(82) == 0); // 5-ring
        }

        [TestMethod()]
        public void TestCountableMACCSBinary_Rings()
        {
            var parser = CDK.SmilesParser;
            var printer = new SubstructureFingerprinter(StandardSubstructureSets.GetCountableMACCSSMARTS());
            IAtomContainer mol;
            BitArray bs;
            // Aromatic 6-rings
            mol = parser.ParseSmiles("C1=CC=CC(=C1)CCCC2=CC=CC=C2");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            bs = printer.GetBitFingerprint(mol).AsBitSet();
            Assert.IsTrue(bs[128]); // 6-ring
            Assert.IsTrue(bs[111]); // aromaticity
            Assert.IsFalse(bs[7]); // 7-ring
            Assert.IsFalse(bs[82]); // 5-ring
                                    // Non-aromatic 6-rings
            mol = parser.ParseSmiles("C1CC(CCC1)CCCCC2CCCCC2");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            bs = printer.GetBitFingerprint(mol).AsBitSet();
            Assert.IsTrue(bs[128]); // 6-ring
            Assert.IsFalse(bs[111]); // aromaticity
            Assert.IsFalse(bs[7]); // 7-ring
            Assert.IsFalse(bs[82]); // 5-ring
                                    // Aromatic 6-ring, 3-ring and 4-ring
            mol = parser.ParseSmiles("C1CC1C(CCC2CCC2)CC3=CC=CC=C3");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            bs = printer.GetBitFingerprint(mol).AsBitSet();
            Assert.IsTrue(bs[128]); // 6-ring
            Assert.IsTrue(bs[111]); // aromaticity
            Assert.IsTrue(bs[10]); // 3-ring
            Assert.IsTrue(bs[1]); // 4-ring
            Assert.IsFalse(bs[7]); // 7-ring
            Assert.IsFalse(bs[82]); // 5-ring
                                    // Aromatic 6-ring, 3-ring and 4-ring
            mol = parser.ParseSmiles("C1(CC1C(CCC2CCC2)CC3=CC=CC=C3)C(C(C(C4CC4)C5CC5)C6CC6)C7CC7");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            bs = printer.GetBitFingerprint(mol).AsBitSet();
            Assert.IsTrue(bs[128]); // 6-ring
            Assert.IsTrue(bs[111]); // aromaticity
            Assert.IsTrue(bs[10]); // 3-ring
            Assert.IsTrue(bs[1]); // 4-ring
            Assert.IsFalse(bs[7]); // 7-ring
            Assert.IsFalse(bs[82]); // 5-ring
        }

        /// <summary>
        /// While this test fails, Daylight says that the
        /// SMARTS pattern used for vinylogous ester should
        /// match benzaldehyde twice. So according to the
        /// supplied definition this answer is actually correct.
        /// </summary>
        // @cdk.bug 2871303
        //("the SMARTS pattern vinylogous ester is not strict enough - we can not fix this")
        public static void TestVinylogousEster()
        {
            string benzaldehyde = "c1ccccc1C=O";
            IFingerprinter fprinter = new SubstructureFingerprinter();
            var sp = CDK.SmilesParser;
            IBitFingerprint fp = fprinter.GetBitFingerprint(sp.ParseSmiles(benzaldehyde));
            Assert.IsFalse(fp[136], "Bit 136 (vinylogous ester) is set to true");
        }

        [TestMethod()]
        public void TestGetSubstructure()
        {
            string[] smarts = { "c1ccccc1", "[CX4H3][#6]", "[CX2]#[CX2]" };
            SubstructureFingerprinter printer = new SubstructureFingerprinter(smarts);
            Assert.AreEqual(printer.GetSubstructure(1), smarts[1]);
        }
    }
}

