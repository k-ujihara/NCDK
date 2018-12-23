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

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class RuleOfFiveDescriptorTest : MolecularDescriptorTest<RuleOfFiveDescriptor>
    {
        protected override RuleOfFiveDescriptor CreateDescriptor() => new RuleOfFiveDescriptor(checkAromaticity: true);

        [TestMethod()]
        public void TestRuleOfFiveDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCC(OCC)OCC(c1cccc2ccccc12)C4CCC(CCCO)C(CC3CNCNC3)C4"); //
            AddExplicitHydrogens(mol);
            Assert.AreEqual(3, CreateDescriptor().Calculate(mol).Value);
        }

        [TestMethod()]
        public void TestRuleOfFiveRotatableBonds()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCC1=CC(NC(=O)CC)=CC(CCC)=C1"); // nRot = 10 (excl. amide C-N bond)
            AddExplicitHydrogens(mol);
            Assert.AreEqual(0, CreateDescriptor().Calculate(mol).Value);
        }

        [TestMethod()]
        public void TestRuleOfFiveRotatableBondsViolated()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCCC1=CC(CCC)=CC(NC(=O)CC)=C1"); // nRot = 11 (excl. amide C-N bond)
            AddExplicitHydrogens(mol);
            Assert.AreEqual(1, CreateDescriptor().Calculate(mol).Value);
        }
    }
}
