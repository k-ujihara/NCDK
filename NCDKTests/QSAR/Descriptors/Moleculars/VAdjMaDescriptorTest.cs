/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Silent;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite for the VAdjMaDescriptor.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class VAdjMaDescriptorTest : MolecularDescriptorTest
    {
        public VAdjMaDescriptorTest()
        {
            SetDescriptor(typeof(VAdjMaDescriptor));
        }

        public void IgnoreCalculate_IAtomContainer()
        {
            Assert.Fail("Not tested");
        }

        [TestMethod()]
        public void TestCyclic()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C1CCC2CCCCC2C1");
            Assert.AreEqual(4.459, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.001);
        }

        [TestMethod()]
        public void TestLinear()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCCCCCCCC");
            Assert.AreEqual(4.17, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.001);
        }

        [TestMethod()]
        public void TestCompound()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCCC1CCCCC1");
            Assert.AreEqual(4.322, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.001);
        }
    }
}
