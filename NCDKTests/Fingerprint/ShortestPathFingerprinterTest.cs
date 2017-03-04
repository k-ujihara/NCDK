/*
 * Copyright (C) 1997-2007, 2011  Egon Willighagen <egonw@users.sf.net>
 * Copyright (C) 2012   Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.Aromaticities;
using NCDK.Default;
using NCDK.Graphs;
using NCDK.Smiles;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Collections;

namespace NCDK.Fingerprint
{
    /// <summary>
    // @author Syed Asad Rahman (2012)
    // @cdk.module test-fingerprint
    /// </summary>
    [TestClass()]
    public class ShortestPathFingerprinterTest : AbstractFixedLengthFingerprinterTest
    {
        public override IFingerprinter GetBitFingerprinter()
        {
            return new ShortestPathFingerprinter();
        }

        [TestMethod()]
        public void TestRegression()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeIndole();
            IAtomContainer mol2 = TestMoleculeFactory.MakePyrrole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            ShortestPathFingerprinter fingerprinter = new ShortestPathFingerprinter();
            IBitFingerprint bs1 = fingerprinter.GetBitFingerprint(mol1);
            Assert.AreEqual(22, bs1.Cardinality,
                    "Seems the fingerprint code has changed. This will cause a number of other tests to fail too!");
            IBitFingerprint bs2 = fingerprinter.GetBitFingerprint(mol2);
            Assert.AreEqual(11, bs2.Cardinality,
                  "Seems the fingerprint code has changed. This will cause a number of other tests to fail too!");
        }

        [TestMethod()]
        public void TestGetSize()
        {
            IFingerprinter fingerprinter = new ShortestPathFingerprinter(512);
            Assert.IsNotNull(fingerprinter);
            Assert.AreEqual(512, fingerprinter.Count);
        }

        /// <summary>
        /// Test of ShortestPathFingerprinter method
        ///
        // @throws InvalidSmilesException
        // @
        /// </summary>
        [TestMethod()]
        public void TestGenerateFingerprint()
        {
            string smiles = "CCCCC1C(=O)N(N(C1=O)C1=CC=CC=C1)C1=CC=CC=C1";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = smilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            ShortestPathFingerprinter fingerprint = new ShortestPathFingerprinter(1024);
            BitArray fingerprint1;
            fingerprint1 = fingerprint.GetBitFingerprint(molecule).AsBitSet();
            Assert.AreEqual(125, BitArrays.Cardinality(fingerprint1));
            Assert.AreEqual(1024, fingerprint1.Count);
        }

        /// <summary>
        /// Test of ShortestPathFingerprinter method
        ///
        // @throws InvalidSmilesException
        // @
        /// </summary>
        [TestMethod()]
        public void TestGenerateFingerprintIsSubset()
        {
            string smilesT = "NC(=O)C1=C2C=CC(Br)=CC2=C(Cl)C=C1";
            string smilesQ = "CC1=C2C=CC(Br)=CC2=C(Cl)C=C1";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer moleculeQ = smilesParser.ParseSmiles(smilesQ);
            IAtomContainer moleculeT = smilesParser.ParseSmiles(smilesT);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(moleculeQ);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(moleculeT);

            ShortestPathFingerprinter fingerprint = new ShortestPathFingerprinter(1024);
            BitArray fingerprintQ;
            BitArray fingerprintT;
            fingerprintQ = fingerprint.GetBitFingerprint(moleculeQ).AsBitSet();
            fingerprintT = fingerprint.GetBitFingerprint(moleculeT).AsBitSet();

            Assert.IsTrue(FingerprinterTool.IsSubset(fingerprintT, fingerprintQ));
        }

        /// <summary>
        /// Test of ShortestPathFingerprinter method
        ///
        // @throws InvalidSmilesException
        // @
        // @throws FileNotFoundException
        /// </summary>
        [TestMethod()]
        public void TestGenerateFingerprintIsNotASubSet1()
        {

            string smilesT = "O[C@H]1[C@H](O)[C@@H](O)[C@H](O)[C@H](O)[C@@H]1O";
            string smilesQ = "OC[C@@H](O)[C@@H](O)[C@H](O)[C@@H](O)C(O)=O";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            smilesParser.Kekulise(false);
            IAtomContainer moleculeQ = smilesParser.ParseSmiles(smilesQ);

            IAtomContainer moleculeT = smilesParser.ParseSmiles(smilesT);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(moleculeQ);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(moleculeT);

            ShortestPathFingerprinter fingerprint = new ShortestPathFingerprinter(1024);
            BitArray fingerprintQ;
            BitArray fingerprintT;
            fingerprintQ = fingerprint.GetBitFingerprint(moleculeQ).AsBitSet();
            fingerprintT = fingerprint.GetBitFingerprint(moleculeT).AsBitSet();
            Assert.IsFalse(FingerprinterTool.IsSubset(fingerprintT, fingerprintQ));
        }

        [TestMethod()]
        public void TestGenerateFingerprintAnthracene()
        {

            string smiles = "C1=CC2=CC3=CC=CC=C3C=C2C=C1";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = smilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            ShortestPathFingerprinter fingerprint = new ShortestPathFingerprinter(1024);
            BitArray fingerprint1;
            fingerprint1 = fingerprint.GetBitFingerprint(molecule).AsBitSet();
            Assert.AreEqual(10, BitArrays.Cardinality(fingerprint1));
        }

        [TestMethod()]
        public void TestGenerateFingerprintNaphthalene()
        {
            string smiles = "C1=CC2=CC=CC=C2C=C1";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = smilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            ShortestPathFingerprinter fingerprint = new ShortestPathFingerprinter(1024);
            BitArray fingerprint1;
            fingerprint1 = fingerprint.GetBitFingerprint(molecule).AsBitSet();
            Assert.AreEqual(8, BitArrays.Cardinality(fingerprint1));
        }

        [TestMethod()]
        public void TestGenerateFingerprintMultiphtalene()
        {

            string smiles = "C1=CC2=CC=C3C4=CC5=CC6=CC=CC=C6C=C5C=C4C=CC3=C2C=C1";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = smilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            ShortestPathFingerprinter fingerprint = new ShortestPathFingerprinter(1024);
            BitArray fingerprint1;
            fingerprint1 = fingerprint.GetBitFingerprint(molecule).AsBitSet();
            Assert.AreEqual(15, BitArrays.Cardinality(fingerprint1));
        }

        [TestMethod()]
        public void TestgetBitFingerprint_IAtomContainer()
        {
            ShortestPathFingerprinter fingerprinter = new ShortestPathFingerprinter();

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            IBitFingerprint bs = fingerprinter.GetBitFingerprint(mol);
            Assert.IsNotNull(bs);
            Assert.AreEqual(fingerprinter.Count, bs.Count);
        }

        [TestMethod()]
        public void TestFingerprinter()
        {
            ShortestPathFingerprinter fingerprinter = new ShortestPathFingerprinter();
            Assert.IsNotNull(fingerprinter);

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(frag1);
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
        }

        [TestMethod()]
        public void TestFingerprinter_int()
        {
            ShortestPathFingerprinter fingerprinter = new ShortestPathFingerprinter(512);
            Assert.IsNotNull(fingerprinter);

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(frag1);
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
        }

        [TestMethod()]
        public void TestFingerprinter_int_int()
        {
            ShortestPathFingerprinter fingerprinter = new ShortestPathFingerprinter(1024);
            Assert.IsNotNull(fingerprinter);

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(frag1);
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
        }

        [TestMethod()]
        public void TestFingerprinterBitSetSize()
        {
            ShortestPathFingerprinter fingerprinter = new ShortestPathFingerprinter(1024);
            Assert.IsNotNull(fingerprinter);
            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            Assert.AreEqual(1024, bs.Length); // highest set bit
            Assert.AreEqual(1024, bs.Count); // actual bit set size
        }

        /// <summary>
        // @cdk.bug 2819557
        ///
        // @throws CDKException
        /// </summary>
        [TestMethod()]
        public void TestBug2819557()
        {
            IAtomContainer butane = MakeButane();
            IAtomContainer propylAmine = MakePropylAmine();

            ShortestPathFingerprinter fp = new ShortestPathFingerprinter();
            BitArray b1 = fp.GetBitFingerprint(butane).AsBitSet();
            BitArray b2 = fp.GetBitFingerprint(propylAmine).AsBitSet();

            //Assert.IsFalse(FingerprinterTool.IsSubset(b2, b1)); // fixed CDK's mistake
            Assert.IsFalse(FingerprinterTool.IsSubset(b2, b1), "butane should not be a substructure of propylamine");
        }

        [TestMethod()]
        public void TestBondPermutation()
        {
            IAtomContainer pamine = MakePropylAmine();
            ShortestPathFingerprinter fp = new ShortestPathFingerprinter();
            IBitFingerprint bs1 = fp.GetBitFingerprint(pamine);

            AtomContainerBondPermutor acp = new AtomContainerBondPermutor(pamine);
            while (acp.MoveNext())
            {
                IAtomContainer container = acp.Current;
                IBitFingerprint bs2 = fp.GetBitFingerprint(container);
                Assert.IsTrue(bs1.Equals(bs2));
            }
        }

        [TestMethod()]
        public void TestAtomPermutation()
        {
            IAtomContainer pamine = MakePropylAmine();
            ShortestPathFingerprinter fp = new ShortestPathFingerprinter();
            IBitFingerprint bs1 = fp.GetBitFingerprint(pamine);

            AtomContainerAtomPermutor acp = new AtomContainerAtomPermutor(pamine);
            while (acp.MoveNext())
            {
                IAtomContainer container = acp.Current;
                IBitFingerprint bs2 = fp.GetBitFingerprint(container);
                Assert.IsTrue(bs1.Equals(bs2));
            }
        }

        [TestMethod()]
        public void TestBondPermutation2()
        {
            IAtomContainer pamine = TestMoleculeFactory.MakeCyclopentane();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(pamine);
            ShortestPathFingerprinter fp = new ShortestPathFingerprinter();
            IBitFingerprint bs1 = fp.GetBitFingerprint(pamine);

            AtomContainerBondPermutor acp = new AtomContainerBondPermutor(pamine);
            while (acp.MoveNext())
            {
                IAtomContainer container = acp.Current;
                IBitFingerprint bs2 = fp.GetBitFingerprint(container);
                Assert.IsTrue(bs1.Equals(bs2));
            }
        }

        [TestMethod()]
        public void TestAtomPermutation2()
        {
            IAtomContainer pamine = TestMoleculeFactory.MakeCyclopentane();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(pamine);
            ShortestPathFingerprinter fp = new ShortestPathFingerprinter();
            IBitFingerprint bs1 = fp.GetBitFingerprint(pamine);

            AtomContainerAtomPermutor acp = new AtomContainerAtomPermutor(pamine);
            while (acp.MoveNext())
            {
                IAtomContainer container = acp.Current;
                IBitFingerprint bs2 = fp.GetBitFingerprint(container);
                Assert.IsTrue(bs1.Equals(bs2));
            }
        }

        public static IAtomContainer MakeFragment1()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            mol.Atoms.Add(new Atom("C")); // 0
            mol.Atoms.Add(new Atom("C")); // 1
            mol.Atoms.Add(new Atom("C")); // 2
            mol.Atoms.Add(new Atom("C")); // 3
            mol.Atoms.Add(new Atom("C")); // 4
            mol.Atoms.Add(new Atom("C")); // 5
            mol.Atoms.Add(new Atom("C")); // 6

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[3], mol.Atoms[5], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Double); // 6
            return mol;
        }

        public static IAtomContainer MakeFragment4()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            mol.Atoms.Add(new Atom("C")); // 0
            mol.Atoms.Add(new Atom("C")); // 1

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            return mol;
        }

        public static IAtomContainer MakeFragment2()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            mol.Atoms.Add(new Atom("C")); // 0
            mol.Atoms.Add(new Atom("C")); // 1
            mol.Atoms.Add(new Atom("C")); // 2
            mol.Atoms.Add(new Atom("S")); // 3
            mol.Atoms.Add(new Atom("O")); // 4
            mol.Atoms.Add(new Atom("C")); // 5
            mol.Atoms.Add(new Atom("C")); // 6

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[3], mol.Atoms[5], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Double); // 6
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Double); // 7
            return mol;
        }

        public static IAtomContainer MakeFragment3()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            mol.Atoms.Add(new Atom("C")); // 0
            mol.Atoms.Add(new Atom("C")); // 1
            mol.Atoms.Add(new Atom("C")); // 2
            mol.Atoms.Add(new Atom("C")); // 3
            mol.Atoms.Add(new Atom("C")); // 4
            mol.Atoms.Add(new Atom("C")); // 5
            mol.Atoms.Add(new Atom("C")); // 6

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[3], mol.Atoms[5], BondOrder.Double); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single); // 6
            return mol;
        }

        public static IAtomContainer MakeButane()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            Atom atom = new Atom("C");
            atom.Id = "0";
            mol.Atoms.Add(atom); // 0

            atom = new Atom("C");
            atom.Id = "1";
            mol.Atoms.Add(atom); // 1

            atom = new Atom("C");
            atom.Id = "2";
            mol.Atoms.Add(atom); // 2

            atom = new Atom("C");
            atom.Id = "3";
            mol.Atoms.Add(atom); // 3

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3

            return mol;
        }

        public static IAtomContainer MakePropylAmine()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            Atom atom = new Atom("C");
            atom.Id = "0";
            mol.Atoms.Add(atom); // 0

            atom = new Atom("C");
            atom.Id = "1";
            mol.Atoms.Add(atom); // 1

            atom = new Atom("C");
            atom.Id = "2";
            mol.Atoms.Add(atom); // 2

            atom = new Atom("N");
            atom.Id = "3";
            mol.Atoms.Add(atom); // 3

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3

            return mol;
        }

        //public static void Main(string[] args) {
        //    BigInteger bi = new BigInteger("0");
        //    bi = bi.Add(BigInteger.ValueOf((long) Math.Pow(2, 63)));
        //    Console.Error.WriteLine(bi.ToString());
        //    bi = bi.Add(BigInteger.ValueOf((long) Math.Pow(2, 0)));
        //    Console.Error.WriteLine(bi.ToString());
        //        ShortestPathFingerprinter fpt = new ShortestPathFingerprinter();
        //        fpt.standAlone = true;
        //        fpt.TestFingerprinter();
        //        fpt.TestFingerprinterArguments();
        //        fpt.TestBug706786();
        //        fpt.TestBug771485();
        //        fpt.TestBug853254();
        //        fpt.TestBug931608();
        //        fpt.TestBug934819();
        //}
    }
}

