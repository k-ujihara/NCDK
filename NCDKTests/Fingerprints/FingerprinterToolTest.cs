/* Copyright (C) 1997-2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Templates;
using System.Collections;
using System.Collections.Generic;

namespace NCDK.Fingerprints
{
    // @cdk.module test-standard
    [TestClass()]
    public class FingerprinterToolTest : CDKTestCase
    {
        public FingerprinterToolTest() : base()
        { }

        [TestMethod()]
        public void TestIsSubSet_BitSet_BitSet()
        {
            Fingerprinter fingerprinter = new Fingerprinter();

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            BitArray bs = fingerprinter.GetBitFingerprint(mol).AsBitSet();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            BitArray bs1 = fingerprinter.GetBitFingerprint(frag1).AsBitSet();
            Assert.IsTrue(FingerprinterTool.IsSubset(bs, bs1));
        }

        [TestMethod()]
        public void TestListDifferences_BitSet_BitSet()
        {
            BitArray bs1 = new BitArray(4);
            BitArray bs2 = new BitArray(5);

            bs1.Set(0, true);
            bs2.Set(0, true);
            bs1.Set(1, true);
            bs1.Set(2, true);
            bs2.Set(2, true);
            bs1.Set(3, true);
            bs2.Set(4, true);

            // 2 bits set in bs1 which are clear in bs2
            Assert.AreEqual(2, FingerprinterTool.ListDifferences(bs2, bs1).Count);
            // 2 bits set in bs2 which are clear in bs1
            Assert.AreEqual(1, FingerprinterTool.ListDifferences(bs1, bs2).Count);
        }

        [TestMethod()]
        public void TestDifferences()
        {
            BitArray bs1 = new BitArray(4);
            BitArray bs2 = new BitArray(5);

            bs1.Set(0, true);
            bs2.Set(0, true);
            bs1.Set(1, true);
            bs1.Set(2, true);
            bs2.Set(2, true);
            bs1.Set(3, true);
            bs2.Set(4, true);

            Assert.AreEqual(3, FingerprinterTool.Differences(bs1, bs2).Count);
        }

        [TestMethod()]
        public void MakeBitFingerprint()
        {
            IDictionary<string, int> features = new Dictionary<string, int>();
            features.Add("CCO", 1);
            features.Add("CC", 1);
            features.Add("C", 1);
            IBitFingerprint fp = FingerprinterTool.MakeBitFingerprint(features, 1024, 1);
            Assert.IsTrue(3 >= fp.Cardinality);
            Assert.IsTrue(fp[(int)((uint)"CCO".GetHashCode() % 1024)]);
            Assert.IsTrue(fp[(int)((uint)"CC".GetHashCode() % 1024)]);
            Assert.IsTrue(fp[(int)((uint)"C".GetHashCode() % 1024)]);
        }

        [TestMethod()]
        public void MakeCountFingerprint()
        {
            IDictionary<string, int> features = new Dictionary<string, int>();
            features.Add("CCO", 1);
            features.Add("CC", 2);
            features.Add("C", 2);
            ICountFingerprint fp = FingerprinterTool.MakeCountFingerprint(features);
            Assert.AreEqual(3, fp.GetNumberOfPopulatedBins());
            Assert.AreEqual(1, fp.GetCountForHash("CCO".GetHashCode()));
            Assert.AreEqual(2, fp.GetCountForHash("CC".GetHashCode()));
            Assert.AreEqual(2, fp.GetCountForHash("C".GetHashCode()));
        }
    }
}
