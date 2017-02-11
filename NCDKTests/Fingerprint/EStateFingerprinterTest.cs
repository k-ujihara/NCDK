/* Copyright (C) 2008 Rajarshi Guha
 *               2009,2011 Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 * Contact: rajarshi@users.sourceforge.net
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
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.Fingerprint
{
    /**
     * @cdk.module test-fingerprint
     */
    [TestClass()]
    public class EStateFingerprinterTest : AbstractFixedLengthFingerprinterTest
    {
        public override IFingerprinter GetBitFingerprinter()
        {
            return new EStateFingerprinter();
        }

        [TestMethod()]
        public void TestGetSize()
        {
            IFingerprinter printer = new EStateFingerprinter();
            Assert.AreEqual(79, printer.Count);
        }

        [TestMethod()]
        public void TestFingerprint()
        {
            SmilesParser parser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IFingerprinter printer = new EStateFingerprinter();

            IBitFingerprint bs1 = printer.GetBitFingerprint(parser.ParseSmiles("C=C-C#N"));
            IBitFingerprint bs2 = printer.GetBitFingerprint(parser.ParseSmiles("C=CCC(O)CC#N"));

            Assert.AreEqual(79, printer.Count);

            Assert.IsTrue(bs1[7]);
            Assert.IsTrue(bs1[10]);
            Assert.IsTrue(FingerprinterTool.IsSubset(bs2.AsBitSet(), bs1.AsBitSet()));
        }

        /**
         * Using EState keys, these molecules are not considered substructures
         * and should only be used for similarity. This is because the EState
         * fragments match hydrogen counts.
         */
        [TestMethod()]
        public override void TestBug706786()
        {

            IAtomContainer superStructure = Bug706786_1();
            IAtomContainer subStructure = Bug706786_2();

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(superStructure);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(subStructure);
            AddImplicitHydrogens(superStructure);
            AddImplicitHydrogens(subStructure);

            IFingerprinter fpr = new EStateFingerprinter();
            IBitFingerprint superBits = fpr.GetBitFingerprint(superStructure);
            IBitFingerprint subBits = fpr.GetBitFingerprint(subStructure);

            Assert.IsTrue(BitArrays.AreEqual(AsBitSet(6, 11, 12, 15, 16, 18, 33, 34, 35), superBits.AsBitSet()));
            Assert.IsTrue(BitArrays.AreEqual(AsBitSet(8, 11, 16, 35), subBits.AsBitSet()));
        }
    }
}
