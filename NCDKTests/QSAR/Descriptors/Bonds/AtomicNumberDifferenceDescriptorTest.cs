/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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

namespace NCDK.QSAR.Descriptors.Bonds
{
    // @cdk.module test-qsarbond
    [TestClass()]
    public class AtomicNumberDifferenceDescriptorTest : BondDescriptorTest
    {
        public AtomicNumberDifferenceDescriptorTest()
        {
            SetDescriptor(typeof(AtomicNumberDifferenceDescriptor));
        }

        [TestMethod()]
        public void TestDescriptor1()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = sp.ParseSmiles("CC");
            double value = ((DoubleResult)descriptor.Calculate(mol1.Bonds[0], mol1).GetValue()).Value;
            Assert.AreEqual(0, value, 0.0000);
        }

        [TestMethod()]
        public void TestDescriptor2()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = sp.ParseSmiles("CO");
            double value = ((DoubleResult)descriptor.Calculate(mol1.Bonds[0], mol1).GetValue()).Value;
            Assert.AreEqual(2, value, 0.0000);
        }
    }
}
