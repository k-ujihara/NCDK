/*
 * Copyright (C) 2010 Rajarshi Guha <rajarshi.guha@gmail.com>
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

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs a test for the <see cref="HybridizationRatioDescriptor"/>.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class HybridizationRatioDescriptorTest : MolecularDescriptorTest
    {
        public HybridizationRatioDescriptorTest()
        {
            SetDescriptor(typeof(HybridizationRatioDescriptor));
        }

        [TestMethod()]
        public void TestHybRatioDescriptor1()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCC");
            Assert.AreEqual(1.0, ((DoubleResult)Descriptor.Calculate(mol).Value).Value, 0.1);
        }

        [TestMethod()]
        public void TestHybRatioDescriptor2()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("c1ccccc1");
            Assert.AreEqual(0.0, ((DoubleResult)Descriptor.Calculate(mol).Value).Value, 0.1);
        }

        [TestMethod()]
        public void TestHybRatioDescriptor3()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[H]C#N");
            Assert.AreEqual(double.NaN, ((DoubleResult)Descriptor.Calculate(mol).Value).Value);
        }
    }
}
