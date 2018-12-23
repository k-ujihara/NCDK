/* Copyright (C) 2008 Rajarshi Guha
 *
 * Contact: rajarshi@users.sourceforge.net
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
using NCDK.Aromaticities;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class KierHallSmartsDescriptorTest : MolecularDescriptorTest<KierHallSmartsDescriptor>
    {
        [TestMethod()]
        public void Test1()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCO");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var result = CreateDescriptor().Calculate(mol);
            var values = result.Values;

            Assert.AreEqual(79, result.Count);
            Assert.AreEqual(1, result["khs.sOH"]);
            Assert.AreEqual(result["khs.sOH"], result.KHSsOH);
        }

        [TestMethod()]
        public void Test2()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("c1c(CN)cc(CCNC)cc1C(CO)CC(=O)CCOCCCO");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var result = CreateDescriptor().Calculate(mol);
            var values = result.Values;

            Assert.AreEqual(79, result.Count);
            Assert.AreEqual(2, result["khs.sOH"]);
            Assert.AreEqual(1, result["khs.dO"]);
            Assert.AreEqual(1, result["khs.ssO"]);
            Assert.AreEqual(1, result["khs.sNH2"]);
            Assert.AreEqual(1, result["khs.ssNH"]);
        }

        [TestMethod()]
        public void Test3()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C#CC(C)(C)C(C)(C)C#C");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var result = CreateDescriptor().Calculate(mol);
            var values = result.Values;

            Assert.AreEqual(79, result.Count);
            Assert.AreEqual(2, result["khs.tsC"]);
            Assert.AreEqual(2, result["khs.ssssC"]);
        }
    }
}
