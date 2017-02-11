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
using NCDK.QSAR.Result;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class IsProtonInAromaticSystemDescriptorTest : AtomicDescriptorTest
    {
        public IsProtonInAromaticSystemDescriptorTest()
        {
            SetDescriptor(typeof(IsProtonInAromaticSystemDescriptor));
        }

        [TestMethod()]
        public void TestIsProtonInAromaticSystemDescriptor()
        {
            IAtomicDescriptor descriptor = new IsProtonInAromaticSystemDescriptor();
            descriptor.Parameters = new object[] { true };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("Oc1cc(OC)c(cc1Br)Br");
            AddExplicitHydrogens(mol);
            Assert.AreEqual("H", mol.Atoms[11].Symbol);
            Assert.AreEqual("H", mol.Atoms[12].Symbol);
            Assert.AreEqual("H", mol.Atoms[13].Symbol);
            Assert.AreEqual("H", mol.Atoms[14].Symbol);
            Assert.AreEqual("H", mol.Atoms[15].Symbol);
            Assert.AreEqual("H", mol.Atoms[16].Symbol);
            Assert.AreEqual(0, ((IntegerResult)descriptor.Calculate(mol.Atoms[11], mol).GetValue()).Value);
            Assert.AreEqual(1, ((IntegerResult)descriptor.Calculate(mol.Atoms[12], mol).GetValue()).Value);
            Assert.AreEqual(0, ((IntegerResult)descriptor.Calculate(mol.Atoms[13], mol).GetValue()).Value);
            Assert.AreEqual(0, ((IntegerResult)descriptor.Calculate(mol.Atoms[14], mol).GetValue()).Value);
            Assert.AreEqual(0, ((IntegerResult)descriptor.Calculate(mol.Atoms[15], mol).GetValue()).Value);
            Assert.AreEqual(1, ((IntegerResult)descriptor.Calculate(mol.Atoms[16], mol).GetValue()).Value);
        }
    }
}
