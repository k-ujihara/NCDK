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
using NCDK.QSAR.Results;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all test for the KierHallSmartsDescriptor
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class KierHallSmartsDescriptorTest : MolecularDescriptorTest
    {
        private readonly IReadOnlyList<string> names;

        public KierHallSmartsDescriptorTest()
        {
            SetDescriptor(typeof(KierHallSmartsDescriptor));
            names = Descriptor.DescriptorNames;
        }

        private int GetIndex(string name)
        {
            for (int i = 0; i < names.Count; i++)
            {
                if (names[i].Equals(name)) return i;
            }
            return -1;
        }

        [TestMethod()]
        public void Test1()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCO");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var value = Descriptor.Calculate(mol);
            ArrayResult<int> result = (ArrayResult<int>)value.Value;

            Assert.AreEqual(79, result.Length);
            Assert.AreEqual(1, result[GetIndex("khs.sOH")]);
        }

        [TestMethod()]
        public void Test2()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("c1c(CN)cc(CCNC)cc1C(CO)CC(=O)CCOCCCO");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var value = Descriptor.Calculate(mol);
            ArrayResult<int> result = (ArrayResult<int>)value.Value;

            Assert.AreEqual(79, result.Length);
            Assert.AreEqual(2, result[GetIndex("khs.sOH")]);
            Assert.AreEqual(1, result[GetIndex("khs.dO")]);
            Assert.AreEqual(1, result[GetIndex("khs.ssO")]);
            Assert.AreEqual(1, result[GetIndex("khs.sNH2")]);
            Assert.AreEqual(1, result[GetIndex("khs.ssNH")]);
        }

        [TestMethod()]
        public void Test3()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C#CC(C)(C)C(C)(C)C#C");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var value = Descriptor.Calculate(mol);
            ArrayResult<int> result = (ArrayResult<int>)value.Value;

            Assert.AreEqual(79, result.Length);
            Assert.AreEqual(2, result[GetIndex("khs.tsC")]);
            Assert.AreEqual(2, result[GetIndex("khs.ssssC")]);
        }
    }
}
