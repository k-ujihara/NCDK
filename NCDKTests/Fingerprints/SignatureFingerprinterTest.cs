/* Copyright (C) 2011  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Smiles;
using System.Collections.Generic;

namespace NCDK.Fingerprints
{
    // @cdk.module test-signature
    [TestClass()]
    public class SignatureFingerprinterTest : AbstractFingerprinterTest
    {
        public override IFingerprinter GetBitFingerprinter()
        {
            return new SignatureFingerprinter();
        }

        [TestMethod()]
        public void TestGetSize()
        {
            IFingerprinter fingerprinter = new SignatureFingerprinter();
            Assert.IsNotNull(fingerprinter);
            Assert.AreEqual(-1, fingerprinter.Count);
        }

        [TestMethod()]
        public override void TestGetRawFingerprint()
        {
            SignatureFingerprinter fingerprinter = new SignatureFingerprinter(0);
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("O(NC)CC");
            IDictionary<string, int> map = fingerprinter.GetRawFingerprint(mol);
            Assert.AreEqual(3, map.Count);
            string[] expectedPrints = { "[O]", "[C]", "[N]" };
            foreach (var print in expectedPrints)
            {
                Assert.IsTrue(map.ContainsKey(print));
            }
        }

        [TestMethod()]
        public void TestBitFingerprint()
        {
            SignatureFingerprinter fingerprinter = new SignatureFingerprinter(0);
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("O(NC)CC");
            IBitFingerprint bitFP = fingerprinter.GetBitFingerprint(mol);
            Assert.IsNotNull(bitFP);
            Assert.AreNotSame(0, bitFP.Count);
        }

        [TestMethod()]
        public override void TestGetCountFingerprint()
        {
            SignatureFingerprinter fingerprinter = new SignatureFingerprinter(0);
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("O(NC)CC");
            ICountFingerprint bitFP = fingerprinter.GetCountFingerprint(mol);
            Assert.IsNotNull(bitFP);
            Assert.AreNotSame(0, bitFP.Count);
        }
    }
}
