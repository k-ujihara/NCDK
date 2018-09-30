/* Copyright (C) 2008 Rajarshi Guha
 *               2009,2011 Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 * Contact: rajarshi@users.sourceforge.net
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
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System.Collections;

namespace NCDK.Fingerprints
{
    // @cdk.module test-fingerprint
    [TestClass()]
    public class MACCSFingerprinterTest : AbstractFixedLengthFingerprinterTest
    {
        public override IFingerprinter GetBitFingerprinter()
        {
            return new MACCSFingerprinter();
        }

        [TestMethod()]
        public void Getsize()
        {
            IFingerprinter printer = new MACCSFingerprinter(Silent.ChemObjectBuilder.Instance);
            Assert.AreEqual(166, printer.Length);
        }

        [TestMethod()]
        public void TestFingerprint()
        {
            SmilesParser parser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IFingerprinter printer = new MACCSFingerprinter();

            IAtomContainer mol1 = parser.ParseSmiles("c1ccccc1CCc1ccccc1");
            IAtomContainer mol2 = parser.ParseSmiles("c1ccccc1CC");
            IAtomContainer mol3 = parser.ParseSmiles("CCC.CCC");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol3);
            Aromaticity.CDKLegacy.Apply(mol1);
            Aromaticity.CDKLegacy.Apply(mol2);
            Aromaticity.CDKLegacy.Apply(mol3);

            BitArray bs1 = printer.GetBitFingerprint(mol1).AsBitSet();
            BitArray bs2 = printer.GetBitFingerprint(mol2).AsBitSet();
            BitArray bs3 = printer.GetBitFingerprint(mol3).AsBitSet();

            Assert.AreEqual(166, printer.Length);

            Assert.IsFalse(bs1[165]);
            Assert.IsTrue(bs1[124]);

            Assert.IsFalse(bs2[124]);

            Assert.IsTrue(bs3[165]);

            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs2));
        }

        [TestMethod()]
        public void Testfp2()
        {
            SmilesParser parser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IFingerprinter printer = new MACCSFingerprinter();

            IAtomContainer mol1 = parser.ParseSmiles("CC(N)CCCN");
            IAtomContainer mol2 = parser.ParseSmiles("CC(N)CCC");
            IAtomContainer mol3 = parser.ParseSmiles("CCCC");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol3);

            Aromaticity.CDKLegacy.Apply(mol1);
            Aromaticity.CDKLegacy.Apply(mol2);
            Aromaticity.CDKLegacy.Apply(mol3);

            BitArray bs1 = printer.GetBitFingerprint(mol1).AsBitSet();
            BitArray bs2 = printer.GetBitFingerprint(mol2).AsBitSet();
            BitArray bs3 = printer.GetBitFingerprint(mol3).AsBitSet();

            Assert.IsFalse(bs1[124]);
            Assert.IsFalse(bs2[124]);
            Assert.IsFalse(bs3[124]);

            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs2));
            Assert.IsTrue(FingerprinterTool.IsSubset(bs2, bs3));
        }

        /// <summary>
        /// Using MACCS keys, these molecules are not considered substructures
        /// and should only be used for similarity. This is because the MACCS
        /// fragments match hydrogen counts.
        /// </summary>
        [TestMethod()]
        public override void TestBug706786()
        {
            IAtomContainer superStructure = Bug706786_1();
            IAtomContainer subStructure = Bug706786_2();

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(superStructure);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(subStructure);
            AddImplicitHydrogens(superStructure);
            AddImplicitHydrogens(subStructure);

            IFingerprinter fpr = new MACCSFingerprinter();
            IBitFingerprint superBits = fpr.GetBitFingerprint(superStructure);
            IBitFingerprint subBits = fpr.GetBitFingerprint(subStructure);

            Assert.IsTrue(BitArrays.Equals(
                AsBitSet(53, 56, 65, 71, 73, 88, 97, 104, 111, 112, 126, 130, 136, 138, 139, 140, 142, 143,
                        144, 145, 148, 149, 151, 153, 156, 158, 159, 161, 162, 163, 164),
                superBits.AsBitSet()));
            Assert.IsTrue(BitArrays.Equals(
                AsBitSet(56, 97, 104, 108, 112, 117, 131, 136, 143, 144, 146, 151, 152, 156, 161, 162, 163, 164),
                subBits.AsBitSet()));
        }
    }
}
