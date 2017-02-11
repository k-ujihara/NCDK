/* Copyright (C) 1997-2007  The Chemistry Development Kit (CKD) project
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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Fingerprint;
using NCDK.Smiles;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Collections;
using System.Collections.Generic;

namespace NCDK.Similarity
{
    /**
     * @cdk.module test-fingerprint
     */
    [TestClass()]
    public class TanimotoTest : CDKTestCase
    {

        bool standAlone = false;

        [TestMethod()]
        public void TestTanimoto1()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeIndole();
            IAtomContainer mol2 = TestMoleculeFactory.MakePyrrole();
            Fingerprinter fingerprinter = new Fingerprinter();
            BitArray bs1 = fingerprinter.GetBitFingerprint(mol1).AsBitSet();
            BitArray bs2 = fingerprinter.GetBitFingerprint(mol2).AsBitSet();
            var tanimoto = Tanimoto.Calculate(bs1, bs2);
            if (standAlone) System.Console.Out.WriteLine("Tanimoto: " + tanimoto);
            if (!standAlone) Assert.AreEqual(0.3939, tanimoto, 0.01 * 2);
        }

        [TestMethod()]
        public void TestTanimoto2()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeIndole();
            IAtomContainer mol2 = TestMoleculeFactory.MakeIndole();
            Fingerprinter fingerprinter = new Fingerprinter();
            BitArray bs1 = fingerprinter.GetBitFingerprint(mol1).AsBitSet();
            BitArray bs2 = fingerprinter.GetBitFingerprint(mol2).AsBitSet();
            var tanimoto = Tanimoto.Calculate(bs1, bs2);
            if (standAlone) System.Console.Out.WriteLine("Tanimoto: " + tanimoto);
            if (!standAlone) Assert.AreEqual(1.0, tanimoto, 0.001);
        }

        [TestMethod()]
        public void TestCalculate_BitFingerprint()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeIndole();
            IAtomContainer mol2 = TestMoleculeFactory.MakePyrrole();
            Fingerprinter fp = new Fingerprinter();
            double similarity = Tanimoto.Calculate(fp.GetBitFingerprint(mol1), fp.GetBitFingerprint(mol2));
            Assert.AreEqual(0.3939, similarity, 0.01 * 2);
        }

        [TestMethod()]
        public void TestExactMatch()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeIndole();
            IAtomContainer mol2 = TestMoleculeFactory.MakeIndole();
            AddImplicitHydrogens(mol1);
            AddImplicitHydrogens(mol2);
            LingoFingerprinter fingerprinter = new LingoFingerprinter();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            IDictionary<string, int> feat1 = fingerprinter.GetRawFingerprint(mol1);
            IDictionary<string, int> feat2 = fingerprinter.GetRawFingerprint(mol2);
            var tanimoto = Tanimoto.Calculate(feat1, feat2);
            Assert.AreEqual(1.0, tanimoto, 0.001);

        }

        [TestMethod()]
        public void TestTanimoto3()
        {
            double[] f1 = { 1, 2, 3, 4, 5, 6, 7 };
            double[] f2 = { 1, 2, 3, 4, 5, 6, 7 };
            var tanimoto = Tanimoto.Calculate(f1, f2);
            if (standAlone) System.Console.Out.WriteLine("Tanimoto: " + tanimoto);
            if (!standAlone) Assert.AreEqual(1.0, tanimoto, 0.001);
        }

        [TestMethod()]
        public void KeggR00258()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string smiles1 = "O=C(O)CCC(=O)C(=O)O";
            string smiles2 = "O=C(O)C(N)CCC(=O)O";
            string smiles3 = "O=C(O)C(N)C";
            string smiles4 = "CC(=O)C(=O)O";
            IAtomContainer molecule1 = sp.ParseSmiles(smiles1);
            IAtomContainer molecule2 = sp.ParseSmiles(smiles2);
            IAtomContainer molecule3 = sp.ParseSmiles(smiles3);
            IAtomContainer molecule4 = sp.ParseSmiles(smiles4);
            Fingerprinter fingerprinter = new Fingerprinter(1024, 6);
            BitArray bs1 = fingerprinter.GetBitFingerprint(molecule1).AsBitSet();
            BitArray bs2 = fingerprinter.GetBitFingerprint(molecule2).AsBitSet();
            BitArray bs3 = fingerprinter.GetBitFingerprint(molecule3).AsBitSet();
            BitArray bs4 = fingerprinter.GetBitFingerprint(molecule4).AsBitSet();

            Assert.AreEqual(0.75, (double)Tanimoto.Calculate(bs1, bs2), 0.1);
            Assert.AreEqual(0.46, (double)Tanimoto.Calculate(bs1, bs3), 0.1);
            Assert.AreEqual(0.52, (double)Tanimoto.Calculate(bs1, bs4), 0.1);
            Assert.AreEqual(0.53, (double)Tanimoto.Calculate(bs2, bs3), 0.1);
            Assert.AreEqual(0.42, (double)Tanimoto.Calculate(bs2, bs4), 0.1);
            Assert.AreEqual(0.8, (double)Tanimoto.Calculate(bs3, bs4), 0.1);
        }

        [TestMethod()]
        public void Method1()
        {
            ICountFingerprint fp1 = new IntArrayCountFingerprint(new Dictionary<string, int>() { { "A", 3 } });
            ICountFingerprint fp2 = new IntArrayCountFingerprint(new Dictionary<string, int>() { { "A", 4 } });
            Assert.AreEqual(0.923, Tanimoto.Method1(fp1, fp2), 0.001);
        }

        [TestMethod()]
        public void Method2()
        {
            ICountFingerprint fp1 = new IntArrayCountFingerprint(new Dictionary<string, int>() { { "A", 3 } });
            ICountFingerprint fp2 = new IntArrayCountFingerprint(new Dictionary<string, int>() { { "A", 4 } });
            Assert.AreEqual(0.75, Tanimoto.Method2(fp1, fp2), 0.001);
        }

        [TestMethod()]
        public void TestCompareBitSetandBitFingerprintTanimoto()
        {
            IAtomContainer mol1 = TestMoleculeFactory.Make123Triazole();
            IAtomContainer mol2 = TestMoleculeFactory.MakeImidazole();
            Fingerprinter fingerprinter = new Fingerprinter();
            BitArray bs1 = fingerprinter.GetBitFingerprint(mol1).AsBitSet();
            BitArray bs2 = fingerprinter.GetBitFingerprint(mol2).AsBitSet();
            var tanimoto = Tanimoto.Calculate(bs1, bs2);

            BitSetFingerprint fp1 = new BitSetFingerprint(bs1);
            BitSetFingerprint fp2 = new BitSetFingerprint(bs2);

            double tanimoto2 = Tanimoto.Calculate(fp1, fp2);
            Assert.AreEqual(tanimoto, tanimoto2, 0.01);

            IntArrayFingerprint ifp1 = new IntArrayFingerprint(fp1);
            IntArrayFingerprint ifp2 = new IntArrayFingerprint(fp2);

            tanimoto2 = Tanimoto.Calculate(ifp1, ifp2);
            Assert.AreEqual(tanimoto, tanimoto2, 0.01);
        }
    }
}
