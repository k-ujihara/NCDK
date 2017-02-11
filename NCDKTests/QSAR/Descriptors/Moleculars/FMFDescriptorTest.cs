/* Copyright (C) 2010  Rajarshi Guha <rajarshi.guha@gmail.com>
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
using NCDK.Aromaticities;
using NCDK.QSAR.Result;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class FMFDescriptorTest : MolecularDescriptorTest
    {
        public FMFDescriptorTest()
        {
            SetDescriptor(typeof(FMFDescriptor));
        }

        [TestMethod()]
        public void TestClenbuterol()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("Clc1cc(cc(Cl)c1N)C(O)CNC(C)(C)C");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            DoubleResult result = (DoubleResult)Descriptor.Calculate(mol).GetValue();
            Assert.AreEqual(0.353, result.Value, 0.01);
        }

        [TestMethod()]
        public void TestCarbinoxamine()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CN(C)CCOC(C1=CC=C(Cl)C=C1)C1=CC=CC=N1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            DoubleResult result = (DoubleResult)Descriptor.Calculate(mol).GetValue();
            Assert.AreEqual(0.65, result.Value, 0.01);
        }

        [TestMethod()]
        public void TestIsamoltane()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CC(C)NCC(O)COC1=C(C=CC=C1)N1C=CC=C1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            DoubleResult result = (DoubleResult)Descriptor.Calculate(mol).GetValue();
            Assert.AreEqual(0.55, result.Value, 0.01);
        }

        [TestMethod()]
        public void TestPirenperone()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("Fc1ccc(cc1)C(=O)C4CCN(CCC\\3=C(\\N=C2\\C=C/C=C\\N2C/3=O)C)CC4");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            DoubleResult result = (DoubleResult)Descriptor.Calculate(mol).GetValue();
            Assert.AreEqual(0.862, result.Value, 0.001);
        }
    }
}
