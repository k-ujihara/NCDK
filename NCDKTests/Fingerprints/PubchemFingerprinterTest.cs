/* Copyright (C) 2009 Rajarshi Guha
 *               2009,2011 Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 * Contact: Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.Common.Base;
using NCDK.Common.Collections;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCDK.Fingerprints
{
    // @cdk.module test-fingerprint
    [TestClass()]
    public class PubchemFingerprinterTest : AbstractFixedLengthFingerprinterTest
    {
        SmilesParser parser;

        public override IFingerprinter GetBitFingerprinter()
        {
            return new PubchemFingerprinter(ChemObjectBuilder.Instance);
        }

        [TestInitialize()]
        public void Setup()
        {
            parser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
        }

        [TestMethod()]
        public void TestGetSize()
        {
            IFingerprinter printer = new PubchemFingerprinter(ChemObjectBuilder.Instance);
            Assert.AreEqual(881, printer.Length);
        }

        [TestMethod()]
        public void TestFingerprint()
        {
            IFingerprinter printer = new PubchemFingerprinter(ChemObjectBuilder.Instance);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(ChemObjectBuilder.Instance);

            IAtomContainer mol1 = parser.ParseSmiles("c1ccccc1CCc1ccccc1");
            IAtomContainer mol2 = parser.ParseSmiles("c1ccccc1CC");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);

            adder.AddImplicitHydrogens(mol1);
            adder.AddImplicitHydrogens(mol2);

            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol1);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol2);

            Aromaticity.CDKLegacy.Apply(mol1);
            Aromaticity.CDKLegacy.Apply(mol2);

            BitArray bs1 = printer.GetBitFingerprint(mol1).AsBitSet();
            BitArray bs2 = printer.GetBitFingerprint(mol2).AsBitSet();

            Assert.AreEqual(881, printer.Length);

            Assert.IsFalse(FingerprinterTool.IsSubset(bs1, bs2),
                "c1ccccc1CC was detected as a subset of c1ccccc1CCc1ccccc1");
        }

        [TestMethod()]
        public void Testfp2()
        {
            IFingerprinter printer = new PubchemFingerprinter(ChemObjectBuilder.Instance);

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

            Assert.IsTrue(FingerprinterTool.IsSubset(bs1, bs2));
            Assert.IsTrue(FingerprinterTool.IsSubset(bs2, bs3));
        }

        /// <summary>
        /// Test case for Pubchem CID 25181308.
        /// </summary>
        /// <exception cref="InvalidSmilesException"></exception>
        // @cdk.inchi InChI=1S/C13H24O10S/c1-20-12-8(18)6(16)10(4(2-14)21-12)23-13-9(19)7(17)11(24)5(3-15)22-13/h4-19,24H,2-3H2,1H3/t4-,5-,6-,7-,8-,9-,10-,11-,12-,13+/m1/s1
        [TestMethod()]
        public void TestCID2518130()
        {
            IAtomContainer mol = parser.ParseSmiles("COC1C(C(C(C(O1)CO)OC2C(C(C(C(O2)CO)S)O)O)O)O");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol.Builder);
            adder.AddImplicitHydrogens(mol);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            IFingerprinter printer = new PubchemFingerprinter(ChemObjectBuilder.Instance);
            BitArray fp = printer.GetBitFingerprint(mol).AsBitSet();
            BitArray ref_ = PubchemFingerprinter
                    .Decode("AAADceBwPABAAAAAAAAAAAAAAAAAAAAAAAAkSAAAAAAAAAAAAAAAGgQACAAACBS0wAOCCAAABgQAAAAAAAAAAAAAAAAAAAAAAAAREAIAAAAiQAAFAAAHAAHAYAwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==");

            Assert.IsTrue(BitArrays.Equals(ref_, fp));
        }

        /// <summary>
        /// Test case for Pubchem CID 5934166.
        /// </summary>
        /// <exception cref="InvalidSmilesException"></exception>
        // @cdk.inchi InChI=1S/C32H26N/c1-5-13-26(14-6-1)21-22-31-23-30(28-17-9-3-10-18-28)24-32(29-19-11-4-12-20-29)33(31)25-27-15-7-2-8-16-27/h1-24H,25H2/q+1/b22-21+
        [TestMethod()]
        public void TestCID5934166()
        {
            IAtomContainer mol = parser.ParseSmiles("C1=CC=C(C=C1)C[N+]2=C(C=C(C=C2C=CC3=CC=CC=C3)C4=CC=CC=C4)C5=CC=CC=C5");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol.Builder);
            adder.AddImplicitHydrogens(mol);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            IFingerprinter printer = new PubchemFingerprinter(ChemObjectBuilder.Instance);
            BitArray fp = printer.GetBitFingerprint(mol).AsBitSet();
            BitArray ref_ = PubchemFingerprinter
                    .Decode("AAADceB+AAAAAAAAAAAAAAAAAAAAAAAAAAA8YMGCAAAAAAAB1AAAHAAAAAAADAjBHgQwgJMMEACgAyRiRACCgCAhAiAI2CA4ZJgIIOLAkZGEIAhggADIyAcQgMAOgAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==");

            Assert.IsTrue(BitArrays.Equals(ref_, fp));
        }

        /// <summary>
        /// Test case for Pubchem CID 25181289.
        /// </summary>
        // @cdk.inchi  InChI=1S/C14H10Cl3N3O3/c1-6(7-2-4-8(21)5-3-7)19-20-11-9(15)12(14(22)23)18-13(17)10(11)16/h2-5,19,21H,1H2,(H,18,20)(H,22,23)
        [TestMethod()]
        public void TestCID25181289()
        {
            IAtomContainer mol = parser.ParseSmiles("C=C(C1=CC=C(C=C1)O)NNC2=C(C(=NC(=C2Cl)Cl)C(=O)O)Cl");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol.Builder);
            adder.AddImplicitHydrogens(mol);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            IFingerprinter printer = new PubchemFingerprinter(ChemObjectBuilder.Instance);
            BitArray fp = printer.GetBitFingerprint(mol).AsBitSet();
            BitArray ref_ = PubchemFingerprinter
                    .Decode("AAADccBzMAAGAAAAAAAAAAAAAAAAAAAAAAA8QAAAAAAAAAABwAAAHgIYCAAADA6BniAwzpJqEgCoAyTyTASChCAnJiIYumGmTtgKJnLD1/PEdQhkwBHY3Qe82AAOIAAAAAAAAABAAAAAAAAAAAAAAAAAAA==");

            Assert.IsTrue(BitArrays.Equals(ref_, fp));
        }

        [TestMethod()]
        public void TestGetFingerprintAsBytes()
        {

            IAtomContainer mol = parser.ParseSmiles("C=C(C1=CC=C(C=C1)O)NNC2=C(C(=NC(=C2Cl)Cl)C(=O)O)Cl");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol.Builder);
            adder.AddImplicitHydrogens(mol);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            PubchemFingerprinter printer = new PubchemFingerprinter(mol.Builder);
            BitArray fp = printer.GetBitFingerprint(mol).AsBitSet();

            byte[] actual = printer.GetFingerprintAsBytes();
            byte[] expected = Arrays.CopyOf(ToByteArray(fp), actual.Length);

            Assert.IsTrue(Compares.AreEqual(expected, actual));
        }

        // adapted from: http://stackoverflow.com/questions/6197411/converting-from-bitset-to-byte-array
        public static byte[] ToByteArray(BitArray bits)
        {
            byte[] bytes = new byte[bits.Length / 8 + 1];
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                {
                    bytes[i / 8] |= (byte)(1 << (7 - i % 8));
                }
            }
            return bytes;
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDecode_invalid()
        {
            PubchemFingerprinter.Decode("a");
        }

        [TestMethod()]
        public void TestDecode()
        {
            BitArray bitSet = PubchemFingerprinter
                    .Decode("AAADcYBgAAAAAAAAAAAAAAAAAAAAAAAAAAAwAAAAAAAAAAABAAAAGAAAAAAACACAEAAwAIAAAACAACBCAAACAAAgAAAIiAAAAIgIICKAERCAIAAggAAIiAcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==");
            int[] setBits = new int[]{0, 9, 10, 178, 179, 255, 283, 284, 332, 344, 355, 370, 371, 384, 416, 434, 441, 446,
                470, 490, 516, 520, 524, 552, 556, 564, 570, 578, 582, 584, 595, 599, 603, 608, 618, 634, 640, 660,
                664, 668, 677, 678, 679};
            foreach (var set in setBits)
            {
                Assert.IsTrue(bitSet[set], "bit " + set + " was not set");
            }
        }

        [TestMethod()]
        public void TestBenzene()
        {
            IAtomContainer mol = parser.ParseSmiles("c1ccccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol.Builder);
            adder.AddImplicitHydrogens(mol);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);

            Aromaticity.CDKLegacy.Apply(mol);
            IFingerprinter printer = new PubchemFingerprinter(ChemObjectBuilder.Instance);
            BitArray fp = printer.GetBitFingerprint(mol).AsBitSet();
            BitArray ref_ = PubchemFingerprinter
                    .Decode("AAADcYBgAAAAAAAAAAAAAAAAAAAAAAAAAAAwAAAAAAAAAAABAAAAGAAAAAAACACAEAAwAIAAAACAACBCAAACAAAgAAAIiAAAAIgIICKAERCAIAAggAAIiAcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==");

            Assert.IsTrue(BitArrays.Equals(ref_, fp));
        }

        class FpRunner
        {
            IAtomContainer mol;

            public BitArray Result { get; private set; }

            public FpRunner(IAtomContainer mol)
            {
                this.mol = mol;
            }

            public void Call()
            {
                BitArray fp = null;
                IFingerprinter fpr = new PubchemFingerprinter(ChemObjectBuilder.Instance);
                try
                {
                    fp = fpr.GetBitFingerprint(mol).AsBitSet();
                }
                catch (CDKException e)
                {
                    Console.Error.WriteLine(e.StackTrace); //To change body of catch statement use File | Settings | File Templates.
                }
                Result = fp;
            }
        }

        // @cdk.bug 3510588
        [TestMethod()]
        public void TestMultithReadedUsage()
        {
            IAtomContainer mol1 = parser.ParseSmiles("C=C(C1=CC=C(C=C1)O)NNC2=C(C(=NC(=C2Cl)Cl)C(=O)O)Cl");
            IAtomContainer mol2 = parser
                    .ParseSmiles("C1=CC=C(C=C1)C[N+]2=C(C=C(C=C2C=CC3=CC=CC=C3)C4=CC=CC=C4)C5=CC=CC=C5");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);

            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol1.Builder);
            adder.AddImplicitHydrogens(mol1);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol1);
            Aromaticity.CDKLegacy.Apply(mol1);

            adder.AddImplicitHydrogens(mol2);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol2);
            Aromaticity.CDKLegacy.Apply(mol2);

            IFingerprinter fp = new PubchemFingerprinter(ChemObjectBuilder.Instance);
            BitArray bs1 = fp.GetBitFingerprint(mol1).AsBitSet();
            BitArray bs2 = fp.GetBitFingerprint(mol2).AsBitSet();

            // now lets run some threads
            var objs = new List<FpRunner>
            {
                new FpRunner(mol1),
                new FpRunner(mol2)
            };
            var ret = Parallel.ForEach(objs, o => o.Call());
            Assert.IsTrue(ret.IsCompleted);
            BitArray fb1 = objs[0].Result;
            Assert.IsNotNull(fb1);

            BitArray fb2 = objs[1].Result;
            Assert.IsNotNull(fb2);

            Assert.IsTrue(BitArrays.Equals(bs1, fb1));
            Assert.IsTrue(BitArrays.Equals(bs2, fb2));
        }

        /// <summary>
        /// Using PubChem/CACTVS Substr keys, these molecules are not considered
        /// substructures and should only be used for similarity. This is because the
        /// PubChem fragments match hydrogen counts. In this case the "599"
        /// bit ("[#1]-C-C=C-[#1]") is found in the substructure but not the
        /// superstructure.
        /// </summary>
        [TestMethod()]
        public override void TestBug934819()
        {
            IAtomContainer subStructure = Bug934819_1();
            IAtomContainer superStructure = Bug934819_2();

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(superStructure);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(subStructure);
            AddImplicitHydrogens(superStructure);
            AddImplicitHydrogens(subStructure);

            IFingerprinter fpr = new PubchemFingerprinter(Silent.ChemObjectBuilder.Instance);
            IBitFingerprint superBits = fpr.GetBitFingerprint(superStructure);
            IBitFingerprint subBits = fpr.GetBitFingerprint(subStructure);

            Assert.IsTrue(BitArrays.Equals(
                AsBitSet(9, 10, 14, 18, 19, 33, 143, 146, 255, 256, 283, 284, 285, 293, 301, 332, 344, 349, 351,
                            353, 355, 368, 370, 371, 376, 383, 384, 395, 401, 412, 416, 421, 423, 434, 441, 446, 449, 454,
                            455, 464, 470, 471, 480, 489, 490, 500, 502, 507, 513, 514, 516, 520, 524, 531, 532, 545, 546,
                            549, 552, 556, 558, 564, 570, 586, 592, 599, 600, 607, 633, 658, 665),
               subBits.AsBitSet()));
            Assert.IsTrue(BitArrays.Equals(
                AsBitSet(9, 10, 11, 14, 18, 19, 33, 34, 143, 146, 150, 153, 255, 256, 257, 258, 283, 284, 285, 293,
                        301, 332, 344, 349, 351, 353, 355, 368, 370, 371, 374, 376, 383, 384, 395, 401, 412, 416, 417,
                        421, 423, 427, 434, 441, 446, 449, 454, 455, 460, 464, 470, 471, 479, 480, 489, 490, 500, 502,
                        507, 513, 514, 516, 520, 524, 531, 532, 545, 546, 549, 552, 556, 558, 564, 570, 578, 582, 584,
                        586, 592, 595, 600, 603, 607, 608, 633, 634, 640, 658, 660, 664, 665, 668, 677, 678, 683),
                superBits.AsBitSet()));
        }
    }
}
