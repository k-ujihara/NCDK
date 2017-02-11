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
using NCDK.Tools;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summaTestSuite that runs a test for the AtomCountDescriptor.ry>
    /// TestSuite that runs a test for the AtomCountDescriptor.
    /// </summary>
    // @cdk.module test-qsarprotein
    [TestClass()]
    public class AminoAcidCountDescriptorTest : MolecularDescriptorTest
    {
        protected override IMolecularDescriptor Descriptor { get; set; }

        public AminoAcidCountDescriptorTest()
        {
            Descriptor = new AminoAcidCountDescriptor();
            SetDescriptor(typeof(AminoAcidCountDescriptor));
        }

        [TestMethod()]
        public void TestAACount()
        {
            IBioPolymer protein = ProteinBuilderTool.CreateProtein("ARNDCFQEGHIPLKMSTYVW", Silent.ChemObjectBuilder.Instance);
            IDescriptorResult result = Descriptor.Calculate(protein).GetValue();
            Assert.IsTrue(result is IntegerArrayResult);
            IntegerArrayResult iaResult = (IntegerArrayResult)result;
            for (int i = 0; i < iaResult.Length; i++)
            {
                Assert.IsTrue(iaResult[i] >= 1); // all AAs are found at least once
            }
            Assert.AreEqual(20, iaResult[8]); // glycine is in all of them, so 20 times
        }

        [TestMethod()]
        public void TestFCount()
        {
            IBioPolymer protein = ProteinBuilderTool.CreateProtein("FF", Silent.ChemObjectBuilder.Instance);
            IDescriptorResult result = Descriptor.Calculate(protein).GetValue();
            Assert.IsTrue(result is IntegerArrayResult);
            IntegerArrayResult iaResult = (IntegerArrayResult)result;
            Assert.AreEqual(2, iaResult[8]);
            Assert.AreEqual(4, iaResult[5]); // thingy is symmetrical, so two mappings at each AA position possible
        }

        [TestMethod()]
        public void TestTCount()
        {
            IBioPolymer protein = ProteinBuilderTool.CreateProtein("TT", Silent.ChemObjectBuilder.Instance);
            IDescriptorResult result = Descriptor.Calculate(protein).GetValue();
            Assert.IsTrue(result is IntegerArrayResult);
            IntegerArrayResult iaResult = (IntegerArrayResult)result;
            Assert.AreEqual(2, iaResult[8]);
            Assert.AreEqual(2, iaResult[16]);
        }
    }
}
