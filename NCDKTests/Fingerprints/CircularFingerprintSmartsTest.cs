/*
 * Copyright (c) 2016 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.SMARTS;
using NCDK.Smiles;
using System.Collections.Generic;

namespace NCDK.Fingerprints
{
    // @cdk.module test-standard 
    [TestClass()]
    public class CircularFingerprintSmartsTest : CDKTestCase
    {
        private static SmilesParser parser = CDK.SmilesParser;

        [TestMethod()]
        public void TestMol1()
        {
            var molSmiles = "CC";
            var expectedFPSmarts = new[] { new[] { "C*" }, new[] { "CC" } };

            CheckFPSmartsForMolecule(molSmiles, expectedFPSmarts);
        }

        [TestMethod()]
        public void TestMol2()
        {
            var molSmiles = "CCC";
            var expectedFPSmarts = new[] { new[] { "C*" }, new[] { "C(*)*" },
                new[] { "CC*", "C(*)C" }, new[] { "CCC" }, };

            CheckFPSmartsForMolecule(molSmiles, expectedFPSmarts);
        }

        [TestMethod()]
        public void TestMol3()
        {
            var molSmiles = "CCN";
            var expectedFPSmarts = new[] { new[] { "C*" }, new[] { "C(*)*" }, new[] { "N*" },
                new[] { "CC*", "C(*)C" }, new[] { "C(*)N", "NC*" },
                new[] { "CCN", "NCC", "C(C)N", "C(N)C" }, };

            CheckFPSmartsForMolecule(molSmiles, expectedFPSmarts);
        }

        [TestMethod()]
        public void TestMol4()
        {
            var molSmiles = "C1CC1";
            var expectedFPSmarts = new[]
            { new[] { "C(*)*" }, new[] { "C1CC1", "C(C1)C1" } };

            CheckFPSmartsForMolecule(molSmiles, expectedFPSmarts);
        }

        [TestMethod()]
        public void TestMol5()
        {
            var molSmiles = "C1CCC1";
            var expectedFPSmarts = new[] {
                new[] { "C(*)*" }, new[] { "C(C*)C*", "C(CC*)*", "C(*)CC*" },
                new[] { "C1CCC1", "C(CC1)C1", "C(C1)CC1" } };

            CheckFPSmartsForMolecule(molSmiles, expectedFPSmarts);
        }

        [TestMethod()]
        public void TestMol6()
        {
            var molSmiles = "CC[C-]";
            var expectedFPSmarts = new[] {
                new[] { "C*" }, new[] { "C(*)*" }, new[] { "[C-]*" }, new[] { "CC*", "C(*)C" },
                new[] { "[C-]C*", "C(*)[C-]" },
                new[] { "CC[C-]", "C(C)[C-]", "[C-]CC", "C([C-])C" } };

            CheckFPSmartsForMolecule(molSmiles, expectedFPSmarts);
        }

        [TestMethod()]
        public void TestMol7()
        {
            var molSmiles = "c1ccccc1";
            var expectedFPSmarts = new[] {
                new[] { "c(a)a" },
                new[] { "c(a)cca", "c(ca)ca", "c(cca)a" },
                new[] { "c(a)cccca", "c(ca)ccca", "c(cca)cca", "c(ccca)ca", "c(cccca)a" },
                new[] { "c1ccccc1", "c(c1)cccc1", "c(cc1)ccc1", "c(ccc1)cc1", "c(cccc1)c1" } };

            CheckFPSmartsForMolecule(molSmiles, expectedFPSmarts);
        }

        private static void CheckFPSmartsForMolecule(string moleculeSmiles, string[][] expectedFPSmarts)
        {
            var expected = new HashSet<string>();
            foreach (var strs in expectedFPSmarts)
                foreach (var str in strs)
                    expected.Add(str);

            // expectedFPSmarts[][] is a double array because for each smarts
            // several equivalent variants
            // of the smarts are given e.g. CCC C(C)C
            var mol = parser.ParseSmiles(moleculeSmiles);

            var circ = new CircularFingerprinter();
            circ.Calculate(mol);
            var subsmarts = new SmartsFragmentExtractor(mol);
            subsmarts.SetMode(SubstructureSelectionMode.JCompoundMapper);
            var numFP = circ.FPCount;

            var actual = new HashSet<string>();
            for (int i = 0; i < numFP; i++)
            {
                var fp = circ.GetFP(i);
                actual.Add(subsmarts.Generate(fp.Atoms));
            }

            Assert.IsTrue(expected.IsSupersetOf(actual));
        }
    }
}
