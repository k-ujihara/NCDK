/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class BondCountDescriptorTest : MolecularDescriptorTest
    {
        private static readonly SmilesParser sp = CDK.SilentSmilesParser;
        
        public BondCountDescriptorTest()
        {
            SetDescriptor(typeof(BondCountDescriptor));
        }

        [TestMethod()]
        public void TestBondCountDescriptor()
        {
            Assert.IsNotNull(Descriptor);
        }

        [TestMethod()]
        public void TestSingleBondCount()
        {
            Descriptor.Parameters = new string[] { "s" };
            var mol = sp.ParseSmiles("CCO"); // ethanol
            Assert.AreEqual(2, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
            mol = sp.ParseSmiles("C=C=C");
            Assert.AreEqual(0, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void TestDoubleBondCount()
        {
            Descriptor.Parameters = new string[] { "d" };
            var mol = sp.ParseSmiles("CCO"); // ethanol
            Assert.AreEqual(0, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
            mol = sp.ParseSmiles("C=C=C");
            Assert.AreEqual(2, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        /// <summary>
        /// The default setting should be to count *all* bonds.
        /// </summary>
        // @cdk.bug 1651263
        [TestMethod()]
        public void TestDefaultSetting()
        {
            IMolecularDescriptor descriptor = new BondCountDescriptor();
            var mol = sp.ParseSmiles("CCO"); // ethanol
            Assert.AreEqual(2, ((Result<int>)descriptor.Calculate(mol).Value).Value);
            mol = sp.ParseSmiles("C=C=C");
            Assert.AreEqual(2, ((Result<int>)descriptor.Calculate(mol).Value).Value);
            mol = sp.ParseSmiles("CC=O");
            Assert.AreEqual(2, ((Result<int>)descriptor.Calculate(mol).Value).Value);
            mol = sp.ParseSmiles("CC#N");
            Assert.AreEqual(2, ((Result<int>)descriptor.Calculate(mol).Value).Value);
        }
    }
}
