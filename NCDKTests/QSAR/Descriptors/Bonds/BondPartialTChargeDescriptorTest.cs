/* Copyright (C) 2004-2008  Miguel Rojas <miguel.rojas@uni-koeln.de>
 *                          Egon Willighagen <egonw@users.sf.net>
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
using NCDK.QSAR.Results;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Bonds
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarbond
    [TestClass()]
    public class BondPartialTChargeDescriptorTest : BondDescriptorTest
    {
        public BondPartialTChargeDescriptorTest()
        {
            descriptor = new BondPartialTChargeDescriptor();
            SetDescriptor(typeof(BondPartialTChargeDescriptor));
        }

        [TestMethod()]
        public void TestBondTElectronegativityDescriptor()
        {
            double[] testResult = { 0.3323, 0.0218 }; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CF");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < 2; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Bonds[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Allyl bromide
        /// </summary>
        [TestMethod()]
        public void TestBondTElectronegativityDescriptor_Allyl_bromide()
        {
            double[] testResult = { 0.0243, 0.1279, 0.1872, 0.1553, 0.1553, 0.1358, 0.0013, 0.0013 };  // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CCBr");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);

            for (int i = 0; i < 8; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Bonds[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.035);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Isopentyl iodide
        /// </summary>
        [TestMethod()]
        public void TestBondTElectronegativityDescriptor_Isopentyl_iodide()
        {
            double testResult = 0.0165; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C(C)(C)CCI");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            double result = ((Result<double>)descriptor.Calculate(mol.Bonds[0], mol).Value).Value;
            Assert.AreEqual(testResult, result, 0.001);
        }

        /// <summary>
        /// A unit test for JUnit with Allyl mercaptan
        /// </summary>
        [TestMethod()]
        public void TestBondTElectronegativityDescriptor_Allyl_mercaptan()
        {
            double[] testResult = { 0.0197, 0.0924, 0.1835, 0.1566, 0.1566, 0.1412, 0.0323, 0.0323, 0.2761 }; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CCS");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 9; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Bonds[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.03);
            }
        }
    }
}
