/* Copyright (C) 1997-2009,2011  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Default;
using NCDK.IO;
using NCDK.Isomorphisms;
using NCDK.Smiles;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace NCDK.Fingerprint
{
    /**
     * @cdk.module test-standard
     */
    [TestClass()]
    public class GraphOnlyFingerprinterTest : AbstractFixedLengthFingerprinterTest
    {
        public override IFingerprinter GetBitFingerprinter()
        {
            return new GraphOnlyFingerprinter();
        }

        [TestMethod()]
        public void TestFingerprint()
        {
            SmilesParser parser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IFingerprinter printer = new GraphOnlyFingerprinter();

            IBitFingerprint bs1 = printer.GetBitFingerprint(parser.ParseSmiles("C=C-C#N"));
            Console.Out.WriteLine("----");
            IBitFingerprint bs2 = printer.GetBitFingerprint(parser.ParseSmiles("CCCN"));

            Assert.AreEqual(bs1, bs2);
        }

        /* ethanolamine */
        private const string ethanolamine = "\n\n\n  4  3  0     0  0  0  0  0  0  1 V2000\n    2.5187   -0.3500    0.0000 N   0  0  0  0  0  0  0  0  0  0\n    0.0938   -0.3500    0.0000 C   0  0  0  0  0  0  0  0  0  0\n    1.3062    0.3500    0.0000 C   0  0  0  0  0  0  0  0  0  0\n   -1.1187    0.3500    0.0000 O   0  0  0  0  0  0  0  0  0  0\n  2  3  1  0  0  0  0\n  2  4  1  0  0  0  0\n  1  3  1  0  0  0  0\nM  END\n";

        /* 2,4-diamino-5-hydroxypyrimidin-dihydrochlorid */
        private const string molecule_test_2 = "\n\n\n 13 11  0     0  0  0  0  0  0  1 V2000\n   -0.5145   -1.0500    0.0000 C   0  0  0  0  0  0  0  0  0  0\n   -1.7269   -1.7500    0.0000 N   0  0  0  0  0  0  0  0  0  0\n   -2.9393   -1.0500    0.0000 C   0  0  0  0  0  0  0  0  0  0\n   -2.9393    0.3500    0.0000 C   0  0  0  0  0  0  0  0  0  0\n   -1.7269    1.0500    0.0000 C   0  0  0  0  0  0  0  0  0  0\n   -0.5145    0.3500    0.0000 N   0  0  0  0  0  0  0  0  0  0\n   -4.1518    1.0500    0.0000 O   0  0  0  0  0  0  0  0  0  0\n   -4.1518   -1.7500    0.0000 N   0  0  0  0  0  0  0  0  0  0\n    0.6980   -1.7500    0.0000 N   0  0  0  0  0  0  0  0  0  0\n   -4.1518    2.4500    0.0000 H   0  0  0  0  0  1  0  0  0  0\n   -5.3642    3.1500    0.0000 Cl  0  0  0  0  0  0  0  0  0  0\n   -4.1518   -3.1500    0.0000 H   0  0  0  0  0  1  0  0  0  0\n   -5.3642   -3.8500    0.0000 Cl  0  0  0  0  0  0  0  0  0  0\n  1  2  1  0  0  0  0\n  2  3  2  0  0  0  0\n  3  4  1  0  0  0  0\n  4  5  2  0  0  0  0\n  5  6  1  0  0  0  0\n  1  6  2  0  0  0  0\n  4  7  1  0  0  0  0\n  3  8  1  0  0  0  0\n  1  9  1  0  0  0  0\n 10 11  1  0  0  0  0\n 12 13  1  0  0  0  0\nM  END\n";

        /**
         * This basic test case shows that some molecules will not be considered
         * as a subset of each other by Fingerprint.IsSubset(), for the GetBitFingerprint(),
         * despite the fact that they are a sub graph of each other according to the
         * UniversalIsomorphismTester.IsSubgraph().
         *
         * @author  Hugo Lafayette <hugo.lafayette@laposte.net>
         *
         * @throws  CloneNotSupportedException
         * @throws  Exception
         *
         * @cdk.bug 1626894
         *
         */
        [TestMethod()]
        public void TestFingerPrint()
        {
            IFingerprinter printer = new GraphOnlyFingerprinter();

            IAtomContainer mol1 = CreateMolecule(molecule_test_2);
            IAtomContainer mol2 = CreateMolecule(ethanolamine);
            Assert.IsTrue(new UniversalIsomorphismTester().IsSubgraph(mol1, mol2), "SubGraph does NOT match");

            BitArray bs1 = printer.GetBitFingerprint((IAtomContainer)mol1.Clone()).AsBitSet();
            BitArray bs2 = printer.GetBitFingerprint((IAtomContainer)mol2.Clone()).AsBitSet();

            Assert.IsTrue(FingerprinterTool.IsSubset(bs1, bs2), "Subset (with fingerprint) does NOT match");

            // Match OK
            Debug.WriteLine("Subset (with fingerprint) does match");
        }

        private static IAtomContainer CreateMolecule(string molecule)
        {
            IAtomContainer structure = null;
            if (molecule != null)
            {
                ISimpleChemObjectReader reader = new MDLV2000Reader(new StringReader(molecule));
                Assert.IsNotNull(reader, "Could not create reader");
                if (reader.Accepts(typeof(AtomContainer)))
                {
                    structure = reader.Read(Default.ChemObjectBuilder.Instance.CreateAtomContainer());
                }
            }
            return structure;
        }
    }
}
