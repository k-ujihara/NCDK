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
using NCDK.Templates;
using NCDK.Tools;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarprotein
    [TestClass()]
    public class AminoAcidCountDescriptorTest : MolecularDescriptorTest<AminoAcidCountDescriptor>
    {
        [TestMethod()]
        public void TestKeyName()
        {
            var values = new int[AminoAcids.Proteinogenics.Count];
            for (int i = 0; i < AminoAcids.Proteinogenics.Count; i++)
                values[i] = i;
            var result = new AminoAcidCountDescriptor.Result(values);

            for (int i = 0; i < AminoAcids.Proteinogenics.Count; i++)
            {
                var aa = AminoAcids.Proteinogenics[i];
                var shortName = aa.GetProperty<string>(AminoAcids.ResidueNameShortKey);
                Assert.AreEqual(values[i], (int)result[$"n{shortName}"]);
            }
        }

        [TestMethod()]
        public void TestAACount()
        {
            var protein = ProteinBuilderTool.CreateProtein("ARNDCFQEGHIPLKMSTYVW");
            var result = CreateDescriptor(protein).Calculate();
            foreach (var n in result.Values)
                Assert.IsTrue(n >= 1); // all AAs are found at least once
            Assert.AreEqual(20, result.NumberOfG); // glycine is in all of them, so 20 times
        }

        [TestMethod()]
        public void TestFCount()
        {
            var protein = ProteinBuilderTool.CreateProtein("FF", Silent.ChemObjectBuilder.Instance);
            var result = CreateDescriptor(protein).Calculate();
            Assert.AreEqual(2, result.NumberOfG);
            Assert.AreEqual(4, result.NumberOfF); // thingy is symmetrical, so two mappings at each AA position possible
        }

        [TestMethod()]
        public void TestTCount()
        {
            var protein = ProteinBuilderTool.CreateProtein("TT");
            var result = CreateDescriptor(protein).Calculate();
            Assert.AreEqual(2, result.NumberOfG);
            Assert.AreEqual(2, result.NumberOfT);
        }
    }
}
