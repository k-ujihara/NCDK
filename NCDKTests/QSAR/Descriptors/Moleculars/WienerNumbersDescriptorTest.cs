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
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class WienerNumbersDescriptorTest : MolecularDescriptorTest
    {
        public WienerNumbersDescriptorTest()
        {
            SetDescriptor(typeof(WienerNumbersDescriptor));
        }

        [TestMethod()]
        public void TestWienerNumbersDescriptor()
        {
            double[] testResult = { 18, 2 };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[H]C([H])([H])C([H])([H])C(=O)O");
            AtomContainerManipulator.RemoveHydrogens(mol);
            DoubleArrayResult retval = (DoubleArrayResult)Descriptor.Calculate(mol).GetValue();
            Assert.AreEqual(testResult[0], retval[0], 0.0001);
            Assert.AreEqual(testResult[1], retval[1], 0.0001);
        }

        /// <summary>
        /// Test if the Descriptor returns the same results with and without explicit hydrogens.
        /// </summary>
        [TestMethod()]
        public void TestWithExplicitHydrogens()
        {
            double[] testResult = { 18, 2 };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[H]C([H])([H])C([H])([H])C(=O)O");
            DoubleArrayResult retval = (DoubleArrayResult)Descriptor.Calculate(mol).GetValue();
            Assert.AreEqual(testResult[0], retval[0], 0.0001);
            Assert.AreEqual(testResult[1], retval[1], 0.0001);
        }

        /// <summary>
        /// Numbers extracted from {@cdk.cite Wiener1947}.
        /// </summary>
        [TestMethod()]
        public void TestOriginalWienerPaperCompounds()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            double[] testResult = { 10, 20, 35, 56, 84, 120, 165, 220, 286 };
            string smiles = "CCC";
            for (int i = 0; i < testResult.Length; i++)
            {
                smiles += "C"; // create the matching paraffin
                IAtomContainer mol = sp.ParseSmiles(smiles);
                DoubleArrayResult retval = (DoubleArrayResult)Descriptor.Calculate(mol).GetValue();
                Assert.AreEqual(testResult[i], retval[0], 0.0001);
            }
        }
    }
}
