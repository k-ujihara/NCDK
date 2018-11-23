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
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class WienerNumbersDescriptorTest : MolecularDescriptorTest<WienerNumbersDescriptor>
    {
        [TestMethod()]
        public void TestWienerNumbersDescriptor()
        {
            double[] testResult = { 18, 2 };
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[H]C([H])([H])C([H])([H])C(=O)O");
            AtomContainerManipulator.RemoveHydrogens(mol);
            var retval = CreateDescriptor(mol).Calculate();
            Assert.AreEqual(testResult[0], retval.PathNumber, 0.0001);
            Assert.AreEqual(testResult[1], retval.PolarityNumber, 0.0001);
        }

        /// <summary>
        /// Test if the Descriptor returns the same results with and without explicit hydrogens.
        /// </summary>
        [TestMethod()]
        public void TestWithExplicitHydrogens()
        {
            double[] testResult = { 18, 2 };
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("[H]C([H])([H])C([H])([H])C(=O)O");
            var retval = CreateDescriptor(mol).Calculate();
            Assert.AreEqual(testResult[0], retval.PathNumber, 0.0001);
            Assert.AreEqual(testResult[1], retval.PolarityNumber, 0.0001);
        }

        /// <summary>
        /// Numbers extracted from <token>cdk-cite-Wiener1947</token>.
        /// </summary>
        [TestMethod()]
        public void TestOriginalWienerPaperCompounds()
        {
            var sp = CDK.SmilesParser;
            double[] testResult = { 10, 20, 35, 56, 84, 120, 165, 220, 286 };
            string smiles = "CCC";
            for (int i = 0; i < testResult.Length; i++)
            {
                smiles += "C"; // create the matching paraffin
                var mol = sp.ParseSmiles(smiles);
                var retval = CreateDescriptor(mol).Calculate();
                Assert.AreEqual(testResult[i], retval.PathNumber, 0.0001);
            }
        }
    }
}
