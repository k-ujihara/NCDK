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
using NCDK.QSAR.Result;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Atomic
{
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class CovalentRadiusDescriptorTest : AtomicDescriptorTest
    {
        public CovalentRadiusDescriptorTest()
        {
            SetDescriptor(typeof(CovalentRadiusDescriptor));
        }

        [TestMethod()]
        public void TestVdWRadiusDescriptor()
        {
            double[] testResult = { 0.77 };
            IAtomicDescriptor descriptor = new CovalentRadiusDescriptor();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("NCCN(C)(C)");
            double retval = ((DoubleResult)descriptor.Calculate(mol.Atoms[1], mol).GetValue()).Value;

            Assert.AreEqual(testResult[0], retval, 0.01);
        }
    }
}
