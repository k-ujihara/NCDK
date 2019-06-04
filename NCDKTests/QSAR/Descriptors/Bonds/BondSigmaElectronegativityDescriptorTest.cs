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
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Bonds
{
    // @cdk.module test-qsarbond
    [TestClass()]
    public class BondSigmaElectronegativityDescriptorTest : BondDescriptorTest<BondSigmaElectronegativityDescriptor>
    {
        public BondSigmaElectronegativityDescriptor CreateDescriptor(IAtomContainer mol, int maxIterations) => new BondSigmaElectronegativityDescriptor(mol, maxIterations);

        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor()
        {
            double[] testResult = { 2.5882, 1.1894 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CF");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            CDK.LonePairElectronChecker.Saturate(mol);
            var descriptor = CreateDescriptor(mol, 6);
            for (int i = 0; i < 2; i++)
            {
                var result = descriptor.Calculate(mol.Bonds[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// A unit test with Methyl chloride
        /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Methyl_chloride()
        {
            double[] testResult = { 2.1612, 0.8751 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCl");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 2; i++)
            {
                var result = descriptor.Calculate(mol.Bonds[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.05);
            }
        }

        /// <summary>
        /// A unit test with Allyl bromide 
        /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Allyl_bromide()
        {
            double[] testResult = { 0.2396, 0.3635, 1.7086, 0.3635, 0.338, 0.574, 0.969, 0.969 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CCBr");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 8; i++)
            {
                var result = descriptor.Calculate(mol.Bonds[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.03);
            }
        }

        /// <summary>
        /// A unit test with Isopentyl iodide
        /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Isopentyl_iodide()
        {
            double testResult = 0.1482;
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C(C)(C)CCI");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            var descriptor = CreateDescriptor(mol);
            var result = descriptor.Calculate(mol.Bonds[0]).Value;
            Assert.AreEqual(testResult, result, 0.001);
        }

        /// <summary>
        /// A unit test with Ethoxy ethane
        /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Ethoxy_ethane()
        {
            double[] testResult = { 0.7939, 1.0715, 1.0715, 0.7939, 0.2749, 0.2749, 0.2749, 0.8796, 0.8796 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCOCC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 8; i++)
            {
                var result = descriptor.Calculate(mol.Bonds[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.002);
            }
        }

        /// <summary>
        /// A unit test with Ethanolamine 
        /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Ethanolamine()
        {
            double[] testResult = { 0.0074, 0.3728, 0.8547, 0.2367, 0.2367 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("NCCO");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 5; i++)
            {
                var result = descriptor.Calculate(mol.Bonds[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.06);
            }
        }

        /// <summary>
        /// A unit test with Allyl mercaptan
        /// </summary>
        [TestMethod()]
        public void TestBondSigmaElectronegativityDescriptor_Allyl_mercaptan()
        {
            double[] testResult = { 0.1832, 0.0143, 0.5307, 0.3593, 0.3593, 8.5917 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CCS");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddExplicitHydrogens(mol);
            var descriptor = CreateDescriptor(mol);
            for (int i = 0; i < 4; i++)
            {
                var result = descriptor.Calculate(mol.Bonds[i]).Value;
                Assert.AreEqual(testResult[i], result, 0.005);
            }
        }
    }
}
