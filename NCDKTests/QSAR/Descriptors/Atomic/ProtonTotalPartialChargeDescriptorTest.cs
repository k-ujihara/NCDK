/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.QSAR.Results;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class ProtonTotalPartialChargeDescriptorTest : AtomicDescriptorTest
    {
        public ProtonTotalPartialChargeDescriptorTest()
        {
            SetDescriptor(typeof(ProtonTotalPartialChargeDescriptor));
        }

        [TestMethod()]
        public void TestProtonTotalPartialChargeDescriptorTest()
        {
            double[] testResult = { 0.07915, 0.05783, 0.05783, 0.05783 };
            IAtomicDescriptor descriptor = new ProtonTotalPartialChargeDescriptor();

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CF");
            AddExplicitHydrogens(mol);
            ArrayResult<double> retval = (ArrayResult<double>)descriptor.Calculate(mol.Atoms[0], mol).Value;
            for (int i = 0; i < testResult.Length; ++i)
            {
                Assert.AreEqual(testResult[i], retval[i], 0.00001);
            }
        }

        // @cdk.bug 2039739
        [TestMethod()]
        public void TestNaNs()
        {
            IAtomicDescriptor descriptor = new ProtonTotalPartialChargeDescriptor();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C(F)(F)");
            AddExplicitHydrogens(mol);
            ArrayResult<double> retval = (ArrayResult<double>)descriptor.Calculate(mol.Atoms[0], mol).Value;
            Assert.AreEqual(5, retval.Length);
            Assert.IsTrue(double.IsNaN(retval[3]));
            Assert.IsTrue(double.IsNaN(retval[4]));
        }
    }
}
