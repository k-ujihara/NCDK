/* Copyright (C) 1997-2007,2011  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Collections;
using NCDK.Default;
using NCDK.Graphs;
using NCDK.IO;
using NCDK.Smiles;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NCDK.Fingerprints
{
    // @cdk.module test-standard
    [TestClass()]
    public class FingerprinterTest : AbstractFixedLengthFingerprinterTest
    {
        public override IFingerprinter GetBitFingerprinter()
        {
            return new Fingerprinter();
        }

        [TestMethod()]
        public void TestRegression()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeIndole();
            IAtomContainer mol2 = TestMoleculeFactory.MakePyrrole();
            Fingerprinter fingerprinter = new Fingerprinter(1024, 8);
            IBitFingerprint bs1 = fingerprinter.GetBitFingerprint(mol1);
            Assert.AreEqual(
                33, bs1.Cardinality,
                "Seems the fingerprint code has changed. This will cause a number of other tests to fail too!");
            IBitFingerprint bs2 = fingerprinter.GetBitFingerprint(mol2);
            Assert.AreEqual(
                13, bs2.Cardinality,
                "Seems the fingerprint code has changed. This will cause a number of other tests to fail too!");
        }

        [TestMethod()]
        public void TestGetSize()
        {
            IFingerprinter fingerprinter = new Fingerprinter(512);
            Assert.IsNotNull(fingerprinter);
            Assert.AreEqual(512, fingerprinter.Count);
        }

        [TestMethod()]
        public void TestGetSearchDepth()
        {
            Fingerprinter fingerprinter = new Fingerprinter(512, 3);
            Assert.IsNotNull(fingerprinter);
            Assert.AreEqual(3, fingerprinter.SearchDepth);
        }

        [TestMethod()]
        public void TestgetBitFingerprint_IAtomContainer()
        {
            Fingerprinter fingerprinter = new Fingerprinter();

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            IBitFingerprint bs = fingerprinter.GetBitFingerprint(mol);
            Assert.IsNotNull(bs);
            Assert.AreEqual(fingerprinter.Count, bs.Count);
        }

        [TestMethod()]
        public void TestFingerprinter()
        {
            Fingerprinter fingerprinter = new Fingerprinter();
            Assert.IsNotNull(fingerprinter);

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
        }

        [TestMethod()]
        public void TestFingerprinter_int()
        {
            Fingerprinter fingerprinter = new Fingerprinter(512);
            Assert.IsNotNull(fingerprinter);

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
        }

        [TestMethod()]
        public void TestFingerprinter_int_int()
        {
            Fingerprinter fingerprinter = new Fingerprinter(1024, 7);
            Assert.IsNotNull(fingerprinter);

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
        }

        [TestMethod()]
        public void TestFingerprinterBitSetSize()
        {
            Fingerprinter fingerprinter = new Fingerprinter(1024, 7);
            Assert.IsNotNull(fingerprinter);
            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            Assert.AreEqual(994, BitArrays.GetLength(bs)); // highest set bit
            Assert.AreEqual(1024, bs.Count); // actual bit set size
        }

        // @cdk.bug 1851202
        [TestMethod()]
        public void TestBug1851202()
        {
            string filename1 = "NCDK.Data.MDL.0002.stg01.rxn";
            Trace.TraceInformation("Testing: " + filename1);
            Stream ins1 = ResourceLoader.GetAsStream(filename1);
            MDLRXNV2000Reader reader = new MDLRXNV2000Reader(ins1, ChemObjectReaderMode.Strict);
            IReaction reaction = (IReaction)reader.Read(new Reaction());
            Assert.IsNotNull(reaction);

            IAtomContainer reactant = reaction.Reactants[0];
            IAtomContainer product = reaction.Products[0];

            Fingerprinter fingerprinter = new Fingerprinter(64 * 26, 8);
            Assert.IsNotNull(fingerprinter.GetBitFingerprint(reactant));
            Assert.IsNotNull(fingerprinter.GetBitFingerprint(product));
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        [TestCategory("SlowTest")]
        public void Testbug2917084()
        {
            string filename1 = "NCDK.Data.MDL.boronBuckyBall.mol";
            Trace.TraceInformation("Testing: " + filename1);
            Stream ins1 = ResourceLoader.GetAsStream(filename1);
            MDLV2000Reader reader = new MDLV2000Reader(ins1, ChemObjectReaderMode.Strict);
            IChemFile chemFile = reader.Read(new ChemFile());
            Assert.IsNotNull(chemFile);
            IAtomContainer mol = ChemFileManipulator.GetAllAtomContainers(chemFile).First();

            Fingerprinter fingerprinter = new Fingerprinter(1024, 8);
            Assert.IsNotNull(fingerprinter.GetBitFingerprint(mol));
        }

        // @cdk.bug 2819557
        [TestMethod()]
        public void TestBug2819557()
        {
            IAtomContainer butane = MakeButane();
            IAtomContainer propylAmine = MakePropylAmine();

            Fingerprinter fp = new Fingerprinter();
            BitArray b1 = fp.GetBitFingerprint(butane).AsBitSet();
            BitArray b2 = fp.GetBitFingerprint(propylAmine).AsBitSet();

            Assert.IsFalse(FingerprinterTool.IsSubset(b2, b1), "butane should not be a substructure of propylamine");
        }

        [TestMethod()]
        public void TestBondPermutation()
        {
            IAtomContainer pamine = MakePropylAmine();
            Fingerprinter fp = new Fingerprinter();
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
            Fingerprinter fp = new Fingerprinter();
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
            Fingerprinter fp = new Fingerprinter();
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
            Fingerprinter fp = new Fingerprinter();
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
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.NewAtomContainer();
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
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            mol.Atoms.Add(new Atom("C")); // 0
            mol.Atoms.Add(new Atom("C")); // 1

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            return mol;
        }

        public static IAtomContainer MakeFragment2()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.NewAtomContainer();
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
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.NewAtomContainer();
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
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            mol.Atoms.Add(new Atom("C")); // 0
            mol.Atoms.Add(new Atom("C")); // 1
            mol.Atoms.Add(new Atom("C")); // 2
            mol.Atoms.Add(new Atom("C")); // 3

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3

            return mol;
        }

        public static IAtomContainer MakePropylAmine()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            mol.Atoms.Add(new Atom("C")); // 0
            mol.Atoms.Add(new Atom("C")); // 1
            mol.Atoms.Add(new Atom("C")); // 2
            mol.Atoms.Add(new Atom("N")); // 3

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3

            return mol;
        }

        [TestMethod()]
        public void PseudoAtomFingerprint()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            string query = "*1CCCC1";
            string indole = "N1CCCC1";
            IAtomContainer queryMol = smipar.ParseSmiles(query);
            IAtomContainer indoleMol = smipar.ParseSmiles(indole);
            Fingerprinter fpr = new Fingerprinter();
            BitArray fp1 = fpr.GetFingerprint(queryMol);
            BitArray fp2 = fpr.GetFingerprint(indoleMol);
            Assert.IsTrue(FingerprinterTool.IsSubset(fp2, fp1));
            Assert.IsFalse(FingerprinterTool.IsSubset(fp1, fp2));
            fpr.SetHashPseudoAtoms(true);
            BitArray fp3 = fpr.GetFingerprint(queryMol);
            BitArray fp4 = fpr.GetFingerprint(indoleMol);
            Assert.IsFalse(FingerprinterTool.IsSubset(fp4, fp3));
            Assert.IsFalse(FingerprinterTool.IsSubset(fp3, fp4));
        }

        [TestMethod()]
        public void PseudoAtomFingerprintArom()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            string query = "*1cccc1";
            string indole = "o1cccc1";
            IAtomContainer queryMol = smipar.ParseSmiles(query);
            IAtomContainer indoleMol = smipar.ParseSmiles(indole);
            Fingerprinter fpr = new Fingerprinter();
            BitArray fp1 = fpr.GetFingerprint(queryMol);
            BitArray fp2 = fpr.GetFingerprint(indoleMol);
            Assert.IsTrue(FingerprinterTool.IsSubset(fp2, fp1));
            Assert.IsFalse(FingerprinterTool.IsSubset(fp1, fp2));
            fpr.SetHashPseudoAtoms(true);
            BitArray fp3 = fpr.GetFingerprint(queryMol);
            BitArray fp4 = fpr.GetFingerprint(indoleMol);
            Assert.IsFalse(FingerprinterTool.IsSubset(fp4, fp3));
            Assert.IsFalse(FingerprinterTool.IsSubset(fp3, fp4));
        }

        [TestMethod()]
        public void TestVersion()
        {
            Fingerprinter fpr = new Fingerprinter(1024, 7);
            fpr.SetPathLimit(2000);
            fpr.SetHashPseudoAtoms(true);
            string expected = "CDK-Fingerprinter/" + CDK.Version + " searchDepth=7 pathLimit=2000 hashPseudoAtoms=" + true.ToString();
            Assert.AreEqual(expected, fpr.GetVersionDescription());
        }
    }
}
