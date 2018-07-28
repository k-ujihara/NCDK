/* Copyright (c) 2014 Collaborative Drug Discovery, Inc. <alex@collaborativedrug.com>
 *
 * Implemented by Alex M. Clark, produced by Collaborative Drug Discovery, Inc.
 * Made available to the CDK community under the terms of the GNU LGPL.
 *
 *    http://collaborativedrug.com
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
using NCDK.Common.Base;
using NCDK.Common.Collections;
using NCDK.Default;
using NCDK.IO;
using NCDK.Numerics;
using NCDK.Smiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace NCDK.Fingerprints
{
    // @cdk.module test-standard
    [TestClass()]
    public class CircularFingerprinterTest : CDKTestCase
    {
        private static readonly IAtomContainer trivialMol = null;

        static CircularFingerprinterTest()
        {
            SmilesParser parser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            try
            {
                trivialMol = parser.ParseSmiles("CCC(=O)N");
            }
            catch (InvalidSmilesException)
            {
            }
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestFingerprints()
        {
            Trace.TraceInformation("CircularFingerprinter test: loading source materials");

            string fnzip = "NCDK.Data.CDD.circular_validation.zip";
            Trace.TraceInformation("Loading source content: " + fnzip);
            using (Stream ins = ResourceLoader.GetAsStream(fnzip))
            {
                Validate(ins);
            }

            Trace.TraceInformation("CircularFingerprinter test: completed without any problems");
        }

        [TestMethod()]
        public void TestUseStereoElements()
        {
            const string smiles1 = "CC[C@@H](C)O";
            const string smiles2 = "CC[C@H](O)C";
            const string molfile = "\n"
                                 + "  CDK     10121722462D          \n"
                                 + "\n"
                                 + "  5  4  0  0  0  0            999 V2000\n"
                                 + "   -4.1837    2.6984    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n"
                                 + "   -3.4692    3.1109    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n"
                                 + "   -2.7547    2.6984    0.0000 C   0  0  1  0  0  0  0  0  0  0  0  0\n"
                                 + "   -2.0403    3.1109    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\n"
                                 + "   -2.7547    1.8734    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0\n"
                                 + "  1  2  1  0  0  0  0\n"
                                 + "  2  3  1  0  0  0  0\n"
                                 + "  3  4  1  0  0  0  0\n"
                                 + "  3  5  1  1  0  0  0\n"
                                 + "M  END\n";
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            MDLV2000Reader mdlr = new MDLV2000Reader(new StringReader(molfile));
            SmilesParser smipar = new SmilesParser(bldr);

            IAtomContainer mol1 = smipar.ParseSmiles(smiles1);
            IAtomContainer mol2 = smipar.ParseSmiles(smiles2);
            IAtomContainer mol3 = mdlr.Read(bldr.NewAtomContainer());

            CircularFingerprinter fpr = new CircularFingerprinter
            {

                // when stereo-chemistry is perceived we don't have coordinates from the
                // SMILES and so get a different fingerprint
                PerceiveStereo = true
            };
            Assert.IsTrue(Compares.AreEqual(fpr.GetFingerprint(mol1), fpr.GetFingerprint(mol2)));
            Assert.IsFalse(Compares.AreEqual(fpr.GetFingerprint(mol2), fpr.GetFingerprint(mol3)));

            fpr.PerceiveStereo = false;
            Assert.IsTrue(Compares.AreEqual(fpr.GetFingerprint(mol1), fpr.GetFingerprint(mol2)));
            Assert.IsTrue(Compares.AreEqual(fpr.GetFingerprint(mol2), fpr.GetFingerprint(mol3)));
        }

        [TestMethod()]
        public void TestGetBitFingerprint()
        {
            Assert.IsTrue(trivialMol != null);
            CircularFingerprinter circ = new CircularFingerprinter();
            IBitFingerprint result = circ.GetBitFingerprint(trivialMol);

            BitArray wantBits = new BitArray(0), gotBits = result.AsBitSet();
            int[] REQUIRE_BITS = { 19, 152, 293, 340, 439, 480, 507, 726, 762, 947, 993 };
            foreach (var b in REQUIRE_BITS)
                BitArrays.SetValue(wantBits, b, true);
            if (!BitArrays.AreEqual(wantBits, gotBits)) throw new CDKException("Got " + gotBits + ", wanted " + wantBits);
        }

        [TestMethod()]
        public void TestGetCountFingerprint()
        {
            Assert.IsTrue(trivialMol != null);
            CircularFingerprinter circ = new CircularFingerprinter();
            ICountFingerprint result = circ.GetCountFingerprint(trivialMol);

            int[] ANSWER_KEY = {-414937772, 1, -1027418143, 1, 1627608083, 1, -868007456, 1, -1006701866, 1,
                -1059145289, 1, -801752141, 1, 790592664, 1, -289109509, 1, -1650154758, 1, 1286833445, 1};

            int wantBits = ANSWER_KEY.Length >> 1;
            bool fail = result.GetNumberOfPopulatedBins() != wantBits;
            for (int n = 0; !fail && n < result.GetNumberOfPopulatedBins(); n++)
            {
                int gotHash = result.GetHash(n), gotCount = result.GetCount(n);
                bool found = false;
                for (int i = 0; i < wantBits; i++)
                {
                    int wantHash = ANSWER_KEY[i * 2], wantCount = ANSWER_KEY[i * 2 + 1];
                    if (gotHash == wantHash)
                    {
                        found = true;
                        if (gotCount != wantCount)
                            throw new CDKException("For hash " + gotHash + " got count " + gotCount + " but wanted "
                                    + wantCount);
                    }
                }
                if (!found)
                {
                    fail = true;
                    break;
                }
            }
            if (fail) throw new CDKException("Hash values do not match.");
        }

        [TestMethod()]
        public void TestGetRawFingerprint()
        {
            // currently no-op
        }

        private void Validate(Stream ins)
        {
            // stream the contents form the zipfile: these are all short
            Dictionary<string, byte[]> content = new Dictionary<string, byte[]>();
            using (var zip = new ZipArchive(ins))
            {
                foreach (var ze in zip.Entries)
                {
                    string fn = ze.Name;
                    MemoryStream buff = new MemoryStream();
                    using (var zs = ze.Open())
                    {
                        int b;
                        while ((b = zs.ReadByte()) != -1)
                            buff.WriteByte((byte)b);
                        content[fn] = buff.ToArray();
                    }
                }
            }

            for (int idx = 1; ; idx++)
            {
                string basefn = idx.ToString();
                while (basefn.Length < 6)
                    basefn = "0" + basefn;
                if (!content.TryGetValue(basefn + ".mol", out byte[] molBytes))
                    break;

                AtomContainer mol = new AtomContainer();
                using (MDLV2000Reader mdl = new MDLV2000Reader(new MemoryStream(molBytes)))
                {
                    mdl.Read(mol);
                }

                CircularFingerprinter.FP[] validateECFP = ParseValidation(content[basefn + ".ecfp"]);
                CircularFingerprinter.FP[] validateFCFP = ParseValidation(content[basefn + ".fcfp"]);

                Trace.TraceInformation("FN=" + basefn + " MOL=" + mol.Atoms.Count + "," + mol.Bonds.Count + " Requires ECFP="
                        + validateECFP.Length + " FCFP=" + validateFCFP.Length);

                ValidateFingerprints("ECFP6", mol, CircularFingerprinter.CFPClass.ECFP6, validateECFP);
                ValidateFingerprints("FCFP6", mol, CircularFingerprinter.CFPClass.FCFP6, validateFCFP);
            }
        }

        private CircularFingerprinter.FP[] ParseValidation(byte[] raw)
        {
            Stream ins = new MemoryStream(raw);
            TextReader rdr = new StreamReader(ins);
            List<CircularFingerprinter.FP> list = new List<CircularFingerprinter.FP>();

            while (true)
            {
                string line = rdr.ReadLine();
                if (line == null || line.Length == 0) break;
                string[] bits = line.Split(' ');
                int hashCode = int.Parse(bits[0]);
                int iteration = int.Parse(bits[1]);
                int[] atoms = new int[bits.Length - 2];
                for (int n = 0; n < atoms.Length; n++)
                    atoms[n] = int.Parse(bits[n + 2]) - 1; // note: atom#'s are 1-based in reference file
                list.Add(new CircularFingerprinter.FP(hashCode, iteration, atoms));
            }

            rdr.Close();
            return list.ToArray();
        }

        private void ValidateFingerprints(string label, AtomContainer mol, CircularFingerprinter.CFPClass classType,
                CircularFingerprinter.FP[] validate)
        {
            CircularFingerprinter circ = new CircularFingerprinter(classType);
            try
            {
                circ.Calculate(mol);
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine("Fingerprint calculation failed for molecule:");
                MDLV2000Writer molwr = new MDLV2000Writer(System.Console.Out);
                molwr.Write(mol);
                molwr.Close();
                throw ex;
            }

            CircularFingerprinter.FP[] obtained = new CircularFingerprinter.FP[circ.FPCount];
            for (int n = 0; n < circ.FPCount; n++)
                obtained[n] = circ.GetFP(n);

            bool same = obtained.Length == validate.Length;
            for (int i = 0; i < obtained.Length && same; i++)
            {
                bool hit = false;
                for (int j = 0; j < validate.Length; j++)
                    if (EqualFingerprints(obtained[i], validate[j]))
                    {
                        hit = true;
                        break;
                    }
                if (!hit) same = false;
            }
            for (int i = 0; i < validate.Length && same; i++)
            {
                bool hit = false;
                for (int j = 0; j < obtained.Length; j++)
                    if (EqualFingerprints(validate[i], obtained[j]))
                    {
                        hit = true;
                        break;
                    }
                if (!hit) same = false;
            }
            if (same) return;

            {
                System.Console.Out.WriteLine("Fingerprint mismatch, validation failed.\nMolecular structure");
                MDLV2000Writer molwr = new MDLV2000Writer(System.Console.Out);
                molwr.Write(mol);
                molwr.Close();
            }

            System.Console.Out.WriteLine("Obtained fingerprints:");
            for (int n = 0; n < obtained.Length; n++)
                System.Console.Out.WriteLine((n + 1) + "/" + obtained.Length + ": " + FormatFP(obtained[n]));
            System.Console.Out.WriteLine("Validation fingerprints:");
            for (int n = 0; n < validate.Length; n++)
                System.Console.Out.WriteLine((n + 1) + "/" + validate.Length + ": " + FormatFP(validate[n]));

            throw new CDKException("Fingerprint comparison failed.");
        }

        private bool EqualFingerprints(CircularFingerprinter.FP fp1, CircularFingerprinter.FP fp2)
        {
            if (fp1.HashCode != fp2.HashCode || fp1.Iteration != fp2.Iteration || fp1.Atoms.Length != fp2.Atoms.Length)
                return false;
            for (int n = 0; n < fp1.Atoms.Length; n++)
                if (fp1.Atoms[n] != fp2.Atoms[n]) return false;
            return true;
        }

        private string FormatFP(CircularFingerprinter.FP fp)
        {
            string str = "[" + fp.HashCode + "] iter=" + fp.Iteration + " atoms={";
            for (int n = 0; n < fp.Atoms.Length; n++)
                str += (n > 0 ? "," : "") + fp.Atoms[n];
            return str + "}";
        }

        [TestMethod()]
        public void ProtonsDontCauseNPE()
        {
            IAtomContainer proton = new AtomContainer();
            proton.Atoms.Add(Atom("H", +1, 0));
            CircularFingerprinter circ = new CircularFingerprinter(CircularFingerprinter.CFPClass.FCFP2);
            Assert.AreEqual(circ.GetBitFingerprint(proton).Cardinality, 0);
        }

        [TestMethod()]
        public void IminesDetectionDoesntCauseNPE()
        {
            IAtomContainer pyrazole = new AtomContainer();
            pyrazole.Atoms.Add(Atom("H", 0, 0));
            pyrazole.Atoms.Add(Atom("N", 0, 0));
            pyrazole.Atoms.Add(Atom("C", 0, 1));
            pyrazole.Atoms.Add(Atom("C", 0, 1));
            pyrazole.Atoms.Add(Atom("C", 0, 1));
            pyrazole.Atoms.Add(Atom("N", 0, 0));
            pyrazole.AddBond(pyrazole.Atoms[0], pyrazole.Atoms[1], BondOrder.Single);
            pyrazole.AddBond(pyrazole.Atoms[1], pyrazole.Atoms[2], BondOrder.Single);
            pyrazole.AddBond(pyrazole.Atoms[2], pyrazole.Atoms[3], BondOrder.Double);
            pyrazole.AddBond(pyrazole.Atoms[3], pyrazole.Atoms[4], BondOrder.Single);
            pyrazole.AddBond(pyrazole.Atoms[4], pyrazole.Atoms[5], BondOrder.Double);
            pyrazole.AddBond(pyrazole.Atoms[1], pyrazole.Atoms[5], BondOrder.Single);
            CircularFingerprinter circ = new CircularFingerprinter(CircularFingerprinter.CFPClass.FCFP2);
            Assert.IsNotNull(circ.GetBitFingerprint(pyrazole));
        }

        // @cdk.bug 1357
        [TestMethod()]
        public void PartialCoordinatesDontCauseNPE()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(Atom("C", 3, 0.000, 0.000));
            m.Atoms.Add(Atom("C", 0, 1.299, -0.750));
            m.Atoms.Add(Atom("H", 0, 0));
            m.Atoms.Add(Atom("O", 0, 1));
            m.Atoms.Add(Atom("C", 2, 2.598, -0.000));
            m.Atoms.Add(Atom("C", 3, 3.897, -0.750));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[3], BondOrder.Single, BondStereo.Down);
            m.AddBond(m.Atoms[1], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            CircularFingerprinter circ = new CircularFingerprinter(CircularFingerprinter.CFPClass.ECFP6);
            Assert.IsNotNull(circ.GetBitFingerprint(m));
        }

        [TestMethod()]
        public void TestNonZZeroPlaner()
        {
            IAtomContainer mol = new AtomContainer();
            Atom[] atoms = new Atom[] {
                new Atom("C"),
                new Atom("F"),
                new Atom("N"),
                new Atom("O"),
            };
            atoms[0].Point3D = new Vector3(0, 0, -10);
            atoms[1].Point3D = new Vector3(0, 1, -10);
            atoms[2].Point3D = new Vector3(-1, -1, -10);
            atoms[3].Point3D = new Vector3(1, -1, -10);
            mol.SetAtoms(atoms);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Bonds[0].Stereo = BondStereo.Up;

            CircularFingerprinter circ = new CircularFingerprinter(CircularFingerprinter.CFPClass.ECFP6)
            {
                PerceiveStereo = true
            };
            IBitFingerprint fp0 = circ.GetBitFingerprint(mol);

            foreach (var atom in atoms)
            {
                var v = atom.Point3D.Value;
                v.Z += 20;
                atom.Point3D = v;
            }

            IBitFingerprint fp1 = circ.GetBitFingerprint(mol);

            Assert.AreEqual(fp0, fp1);
        }

        static IAtom Atom(string symbol, int q, int h)
        {
            IAtom a = new Atom(symbol)
            {
                FormalCharge = q,
                ImplicitHydrogenCount = h
            };
            return a;
        }

        static IAtom Atom(string symbol, int h, double x, double y)
        {
            IAtom a = new Atom(symbol)
            {
                Point2D = new Vector2(x, y),
                ImplicitHydrogenCount = h
            };
            return a;
        }

        [TestMethod()]
        public void TestVersion()
        {
            var fpr = new CircularFingerprinter(CircularFingerprinter.CFPClass.ECFP4);
            string expected = "CDK-CircularFingerprinter/" + CDK.Version + 
                " classType=ECFP4 perceiveStereochemistry=false";
            Assert.AreEqual(expected, fpr.GetVersionDescription());
        }
    }
}
