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

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class EffectiveAtomPolarizabilityDescriptorTest : AtomicDescriptorTest
    {
        public EffectiveAtomPolarizabilityDescriptorTest()
        {
            SetDescriptor(typeof(EffectiveAtomPolarizabilityDescriptor));
        }

        /// <summary>
        /// A unit test for JUnit with 2-(dimethylamino)ethyl)amino
        /// </summary>
        [TestMethod()]
        public void TestEffectivePolarizabilityDescriptor()
        {
            double[] testResult = { 4.7253, 6.1345, 6.763, 6.925, 5.41, 5.41 };
            IAtomicDescriptor descriptor = new EffectiveAtomPolarizabilityDescriptor();

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("NCCN(C)(C)");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 6; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Ethyl chloride
        /// </summary>
        [TestMethod()]
        public void TestPolarizabilityDescriptor_Ethyl_chloride()
        {
            double[] testResult = { 4.8445, 5.824, 4.6165 };                
                // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml

            EffectiveAtomPolarizabilityDescriptor descriptor = new EffectiveAtomPolarizabilityDescriptor();

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCCl");
            AddExplicitHydrogens(mol);
            for (int i = 0; i < 3; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Allyl bromide
        /// </summary>
        [TestMethod()]
        public void TestPolarizabilityDescriptor_Allyl_bromide()
        {
            double testResult = 6.1745; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            EffectiveAtomPolarizabilityDescriptor descriptor = new EffectiveAtomPolarizabilityDescriptor();

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CCBr");
            AddExplicitHydrogens(mol);

            double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[3], mol).GetValue()).Value;
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test for JUnit with Isopentyl iodide
        /// </summary>
        [TestMethod()]
        public void TestPolarizabilityDescriptor_Isopentyl_iodide()
        {
            double[] testResult = { 8.3585, 6.1118, 6.1118, 9.081, 10.526, 8.69 }; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            EffectiveAtomPolarizabilityDescriptor descriptor = new EffectiveAtomPolarizabilityDescriptor();

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C(C)(C)CCI");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 6; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Ethoxy ethane
        /// </summary>
        [TestMethod()]
        public void TestPolarizabilityDescriptor_Ethoxy_ethane()
        {
            double testResult = 5.207;  // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            EffectiveAtomPolarizabilityDescriptor descriptor = new EffectiveAtomPolarizabilityDescriptor();

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCOCC");
            AddExplicitHydrogens(mol);

            double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[2], mol).GetValue()).Value;
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test for JUnit with Ethanolamine
        /// </summary>
        [TestMethod()]
        public void TestPolarizabilityDescriptor_Ethanolamine()
        {
            double[] testResult = { 4.2552, 5.1945, 4.883, 3.595 }; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            EffectiveAtomPolarizabilityDescriptor descriptor = new EffectiveAtomPolarizabilityDescriptor();

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("NCCO");
            AddExplicitHydrogens(mol);
            for (int i = 0; i < 4; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Allyl mercaptan
        /// </summary>
        [TestMethod()]
        public void TestPolarizabilityDescriptor_Allyl_mercaptan()
        {
            double[] testResult = { 5.2995, 6.677, 7.677, 6.2545 }; // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            EffectiveAtomPolarizabilityDescriptor descriptor = new EffectiveAtomPolarizabilityDescriptor();

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CCS");
            AddExplicitHydrogens(mol);
            for (int i = 0; i < 4; i++)
            {
                double result = ((DoubleResult)descriptor.Calculate(mol.Atoms[i], mol).GetValue()).Value;
                Assert.AreEqual(testResult[i], result, 0.02);
            }
        }
    }
}
