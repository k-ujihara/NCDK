/*
 * Copyright (C) 2010  Rajarshi Guha <rajarshi.guha@gmail.com>
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
using NCDK.Smiles;
using System.Collections.Generic;

namespace NCDK.Fingerprint
{
    /// <summary>
    // @cdk.module test-smiles
    /// </summary>
    [TestClass()]
    public class LingoFingerprinterTest : AbstractFingerprinterTest
    {
        public override IFingerprinter GetBitFingerprinter()
        {
            return new LingoFingerprinter();
        }

        [TestMethod()]
        public void TestGetSize()
        {
            IFingerprinter fingerprinter = new LingoFingerprinter();
            Assert.IsNotNull(fingerprinter);
            Assert.AreEqual(-1, fingerprinter.Count);
        }


        [TestMethod()]
        public override void TestGetCountFingerprint()
        {
            LingoFingerprinter fpr = new LingoFingerprinter(4);
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("Oc1ccccc1");
            ICountFingerprint fp = fpr.GetCountFingerprint(mol);
            Assert.AreEqual(2, fp.GetCountForHash("cccc".GetHashCode()));
            Assert.AreEqual(1, fp.GetCountForHash("Oc0c".GetHashCode()));
            Assert.AreEqual(1, fp.GetCountForHash("c0cc".GetHashCode()));
            Assert.AreEqual(1, fp.GetCountForHash("0ccc".GetHashCode()));
            Assert.AreEqual(1, fp.GetCountForHash("ccc0".GetHashCode()));
        }

        [TestMethod()]
        public override void TestGetRawFingerprint()
        {
            LingoFingerprinter lfp = new LingoFingerprinter(3);
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("SPONC");
            IDictionary<string, int> map = lfp.GetRawFingerprint(mol);
            Assert.AreEqual(3, map.Count);
            // depend on canonical ordering of the SMILES since lingos uses Unique SMILES
            string[] subs = { "PON", "ONC", "SPO" };
            foreach (var s in subs)
                Assert.IsTrue(map.ContainsKey(s));
        }
    }
}
