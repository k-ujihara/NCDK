/*
 * Copyright (C) 2011 Jonathan Alvarsson <jonalv@users.sf.net>
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
using System.Collections;

namespace NCDK.Fingerprints
{
    // @cdk.module test-fingerprint
    [TestClass()]
    public class KlekotaRothFingerprinterTest : AbstractFingerprinterTest
    {
        public override IFingerprinter GetBitFingerprinter()
        {
            return new KlekotaRothFingerprinter();
        }

        [TestMethod()]
        public void TestGetSize()
        {
            IFingerprinter printer = GetBitFingerprinter();
            Assert.AreEqual(4860, printer.Count);
        }

        [TestMethod()]
        public void TestFingerprint()
        {
            SmilesParser parser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IFingerprinter printer = GetBitFingerprinter();

            BitArray bs1 = printer.GetBitFingerprint(parser.ParseSmiles("C=C-C#N")).AsBitSet();
            BitArray bs2 = printer.GetBitFingerprint(parser.ParseSmiles("C=CCC(O)CC#N")).AsBitSet();

            Assert.AreEqual(4860, printer.Count);

            Assert.IsTrue(FingerprinterTool.IsSubset(bs2, bs1));
        }
    }
}
