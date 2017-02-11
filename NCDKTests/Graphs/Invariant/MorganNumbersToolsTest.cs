/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Graphs.Canon;
using NCDK.IO;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.Graphs.Invariant
{
    /**
     * Checks the functionality of the MorganNumberTools.
     *
     * @cdk.module test-standard
     */
	[TestClass()]
    public class MorganNumbersToolsTest : CDKTestCase
    {

        public MorganNumbersToolsTest()
                : base()
        {
        }

        [TestMethod()]
        public void TestGetMorganNumbers_IAtomContainer()
        {
            // This is an array with the expected Morgan Numbers for a-pinene
            long[] reference = { 28776, 17899, 23549, 34598, 31846, 36393, 9847, 45904, 15669, 15669 };

            IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
            long[] morganNumbers = MorganNumbersTools.GetMorganNumbers((AtomContainer)mol);
            Assert.AreEqual(reference.Length, morganNumbers.Length);
            for (int f = 0; f < morganNumbers.Length; f++)
            {
                //Debug.WriteLine(morganNumbers[f]);
                Assert.AreEqual(reference[f], morganNumbers[f]);
            }
        }

        [TestMethod()]
        public void TestPhenylamine()
        {
            // This is an array with the expected Morgan Numbers for a-pinene
            string[] reference = { "C-457", "C-428", "C-325", "C-354", "C-325", "C-428", "N-251" };

            IAtomContainer mol = TestMoleculeFactory.MakePhenylAmine();
            string[] morganNumbers = MorganNumbersTools.GetMorganNumbersWithElementSymbol((AtomContainer)mol);
            Assert.AreEqual(reference.Length, morganNumbers.Length);
            for (int f = 0; f < morganNumbers.Length; f++)
            {
                //Debug.WriteLine(morganNumbers[f]);
                Assert.AreEqual(reference[f], morganNumbers[f]);
            }
        }

        /**
         * @cdk.bug 2846213
         */
        [TestMethod()]
        public void TestBug2846213()
        {
            string filename = "NCDK.Data.MDL.bug2846213.mol";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            ChemFile chemFile = (ChemFile) reader.Read((ChemObject) new ChemFile());
            IAtomContainer ac = ChemFileManipulator.GetAllAtomContainers(chemFile).First();
            long[] morganNumbers = MorganNumbersTools.GetMorganNumbers(ac);
            Assert.IsFalse(morganNumbers[7] == morganNumbers[8]);
        }
    }
}
