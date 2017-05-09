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
using NCDK.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;

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
            SubstructureFingerprinter fp = new SubstructureFingerprinter();
            Assert.AreEqual(307, fp.Count);
        }

        [TestMethod()]
        public override void TestBug706786()
        {
            IAtomContainer superStructure = Bug706786_1();
            IAtomContainer subStructure = Bug706786_2();

            AddImplicitHydrogens(superStructure);
            AddImplicitHydrogens(subStructure);

            IFingerprinter fpr = GetBitFingerprinter();
            IBitFingerprint superBits = fpr.GetBitFingerprint(superStructure);
            IBitFingerprint subBits = fpr.GetBitFingerprint(subStructure);

            Assert.IsTrue(BitArrays.AreEqual(
                AsBitSet(0, 11, 13, 17, 40, 48, 136, 273, 274, 278, 286, 294, 299, 301, 304, 306),
                superBits.AsBitSet()));
            Assert.IsTrue(BitArrays.AreEqual(
                AsBitSet(1, 17, 273, 274, 278, 294, 306),
                subBits.AsBitSet()));
        }

        [TestMethod()]
        public void TestUserFunctionalGroups()
        {
            string[] smarts = { "c1ccccc1", "[CX4H3][#6]", "[CX2]#[CX2]" };
            IFingerprinter printer = new SubstructureFingerprinter(smarts);
            Assert.AreEqual(3, printer.Count);

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = sp.ParseSmiles("c1ccccc1CCC");
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
        public void TestFingerprint()
        {
            IFingerprinter printer = new SubstructureFingerprinter();
            Assert.AreEqual(307, printer.Count);

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = sp.ParseSmiles("c1ccccc1CCC");
            IBitFingerprint fp = printer.GetBitFingerprint(mol1);
            Assert.IsNotNull(fp);
            Assert.IsTrue(fp[273]);
            Assert.IsTrue(fp[0]);
            Assert.IsTrue(fp[1]);
            Assert.IsFalse(fp[100]);
        }

        /// <summary>
        // @cdk.bug 2871303
        ///
        /// While this test fails, Daylight says that the
        /// SMARTS pattern used for vinylogous ester should
        /// match benzaldehyde twice. So according to the
        /// supplied definition this answer is actually correct.
        /// </summary>
        //("the SMARTS pattern vinylogous ester is not strict enough - we can not fix this")
        public void TestVinylogousEster()
        {
            string benzaldehyde = "c1ccccc1C=O";
            IFingerprinter fprinter = new SubstructureFingerprinter();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
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
