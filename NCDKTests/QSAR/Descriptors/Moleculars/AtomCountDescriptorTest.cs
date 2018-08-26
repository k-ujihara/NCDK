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
using NCDK.Silent;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs a test for the AtomCountDescriptor.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class AtomCountDescriptorTest : MolecularDescriptorTest
    {
        public AtomCountDescriptorTest()
        {
            SetDescriptor(typeof(AtomCountDescriptor));
        }

        [TestMethod()]
        public void TestCarbonCount()
        {
            object[] parameters = new object[] { "C" };
            Descriptor.Parameters = parameters;
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCO"); // ethanol
            var value = Descriptor.Calculate(mol);
            Assert.AreEqual(2, ((Result<int>)value.Value).Value);
            Assert.AreEqual(1, value.Names.Count);
            Assert.AreEqual("nC", value.Names[0]);
            Assert.AreEqual(Descriptor.DescriptorNames[0], value.Names[0]);
        }

        [TestMethod()]
        public void TestImplicitExplicitH()
        {
            object[] parameters = new object[] { "*" };
            Descriptor.Parameters = parameters;
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C"); // ethanol
            var value = Descriptor.Calculate(mol);
            Assert.AreEqual(5, ((Result<int>)value.Value).Value);

            mol = sp.ParseSmiles("[C]"); // ethanol
            value = Descriptor.Calculate(mol);
            Assert.AreEqual(1, ((Result<int>)value.Value).Value);
        }
    }
}

