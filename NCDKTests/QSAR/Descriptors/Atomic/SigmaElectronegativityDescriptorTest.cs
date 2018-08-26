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
using NCDK.QSAR.Results;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class SigmaElectronegativityDescriptorTest : AtomicDescriptorTest
    {
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
    
        public SigmaElectronegativityDescriptorTest()
        {
            SetDescriptor(typeof(SigmaElectronegativityDescriptor));
        }

        [TestMethod()]
        public void TestSigmaElectronegativityDescriptor()
        {
            double[] testResult = { 8.7177, 11.306 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new SigmaElectronegativityDescriptor();
            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer mol = sp.ParseSmiles("CF");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 2; i++)
            {
                descriptor.Parameters = new object[] { 6 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }

        }

        /// <summary>
        /// A unit test for JUnit with Methyl chloride
        /// </summary>
        [TestMethod()]
        public void TestSigmaElectronegativityDescriptor_Methyl_chloride()
        {
            double[] testResult = { 8.3293, 10.491 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new SigmaElectronegativityDescriptor();
            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer mol = sp.ParseSmiles("CCl");
            AddExplicitHydrogens(mol);
            for (int i = 0; i < 2; i++)
            {
                descriptor.Parameters = new object[] { 6 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.05);
            }
        }

        /// <summary>
        /// A unit test for JUnit with Allyl bromide
        /// </summary>
        [TestMethod()]
        public void TestSigmaElectronegativityDescriptor_Allyl_bromide()
        {
            double[] testResult = { 7.8677, 8.1073, 8.4452, 10.154 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new SigmaElectronegativityDescriptor();
            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer mol = sp.ParseSmiles("C=CCBr");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 4; i++)
            {
                descriptor.Parameters = new object[] { 6 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.02);
            }
        }

        /// <summary>
        ///   A unit test for JUnit with Isopentyl iodide
        /// </summary>
        [TestMethod()]
        public void TestSigmaElectronegativityDescriptor_Isopentyl_iodide()
        {
            double testResult = 9.2264;
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new SigmaElectronegativityDescriptor();

            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer mol = sp.ParseSmiles("C(C)(C)CCI");
            AddExplicitHydrogens(mol);

            double result = ((Result<double>)descriptor.Calculate(mol.Atoms[5], mol).Value).Value;
            Assert.AreEqual(testResult, result, 0.08);
        }

        /// <summary>
        ///   A unit test for JUnit with Ethoxy ethane
        /// </summary>
        [TestMethod()]
        public void TestSigmaElectronegativityDescriptor_Ethoxy_ethane()
        {
            double[] testResult = { 7.6009, 8.3948, 9.4663, 8.3948, 7.6009 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new SigmaElectronegativityDescriptor();
            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer mol = sp.ParseSmiles("CCOCC");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 5; i++)
            {
                descriptor.Parameters = new object[] { 6 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.002);
            }
        }

        /// <summary>
        ///   A unit test for JUnit with Ethanolamine
        /// </summary>
        [TestMethod()]
        public void TestSigmaElectronegativityDescriptor_Ethanolamine()
        {
            double[] testResult = { 8.1395, 8.1321, 8.5049, 9.3081 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new SigmaElectronegativityDescriptor();
            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer mol = sp.ParseSmiles("NCCO");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 4; i++)
            {
                descriptor.Parameters = new object[] { 6 };
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.002);
            }
        }

        /// <summary>
        ///   A unit test for JUnit with Allyl mercaptan
        /// </summary>
        [TestMethod()]
        public void TestSigmaElectronegativityDescriptor_Allyl_mercaptan()
        {
            double[] testResult = { 7.8634, 8.0467, 8.061, 8.5917 };
            // from Petra online: http://www2.chemie.uni-erlangen.de/services/petra/smiles.phtml
            IAtomicDescriptor descriptor = new SigmaElectronegativityDescriptor();

            SmilesParser sp = new SmilesParser(builder);
            IAtomContainer mol = sp.ParseSmiles("C=CCS");
            AddExplicitHydrogens(mol);

            for (int i = 0; i < 4; i++)
            {
                double result = ((Result<double>)descriptor.Calculate(mol.Atoms[i], mol).Value).Value;
                Assert.AreEqual(testResult[i], result, 0.01);
            }
        }

        /// <summary>
        ///   A unit test for JUnit with CCCCl # CCC[Cl+*]
        /// </summary>
        /// 
        // @cdk.inchi InChI=1/C3H7Cl/c1-2-3-4/h2-3H2,1H3
        [TestMethod()]
        public void TestCompareIonized()
        {
            IAtomContainer molA = builder.NewAtomContainer();
            molA.Atoms.Add(builder.NewAtom("C"));
            molA.Atoms.Add(builder.NewAtom("C"));
            molA.AddBond(molA.Atoms[0], molA.Atoms[1], BondOrder.Single);
            molA.Atoms.Add(builder.NewAtom("C"));
            molA.AddBond(molA.Atoms[1], molA.Atoms[2], BondOrder.Single);
            molA.Atoms.Add(builder.NewAtom("Cl"));
            molA.AddBond(molA.Atoms[2], molA.Atoms[3], BondOrder.Single);

            AddExplicitHydrogens(molA);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molA);
            LonePairElectronChecker.Saturate(molA);

            double resultA = ((Result<double>)descriptor.Calculate(molA.Atoms[3], molA).Value).Value;

            IAtomContainer molB = builder.NewAtomContainer();
            molB.Atoms.Add(builder.NewAtom("C"));
            molB.Atoms.Add(builder.NewAtom("C"));
            molB.AddBond(molB.Atoms[0], molB.Atoms[1], BondOrder.Single);
            molB.Atoms.Add(builder.NewAtom("C"));
            molB.AddBond(molB.Atoms[1], molB.Atoms[2], BondOrder.Single);
            molB.Atoms.Add(builder.NewAtom("Cl"));
            molB.Atoms[3].FormalCharge = 1;
            molB.AddSingleElectronTo(molB.Atoms[3]);
            molB.AddLonePairTo(molB.Atoms[3]);
            molB.AddLonePairTo(molB.Atoms[3]);
            molB.AddBond(molB.Atoms[2], molB.Atoms[3], BondOrder.Single);

            AddExplicitHydrogens(molB);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molB);
            LonePairElectronChecker.Saturate(molB);

            Assert.AreEqual(1, molB.Atoms[3].FormalCharge.Value, 0.00001);
            Assert.AreEqual(1, molB.SingleElectrons.Count, 0.00001);
            Assert.AreEqual(2, molB.LonePairs.Count, 0.00001);

            double resultB = ((Result<double>)descriptor.Calculate(molB.Atoms[3], molB).Value).Value;

            Assert.AreEqual(resultA, resultB, 0.00001);
        }
    }
}
