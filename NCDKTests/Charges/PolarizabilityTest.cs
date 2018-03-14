/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@list.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;

namespace NCDK.Charges
{
    /// <summary>
    ///  Description of the Class
    /// </summary>
    // @cdk.module test-charges
    // @author     chhoppe
    // @cdk.created    2004-11-04
    [TestClass()]
    public class PolarizabilityTest : CDKTestCase
    {
        /// <summary>
        ///  A unit test for JUnit
        /// </summary>
        [TestMethod()]
        public void TestGetPolarizabilitiyFactorForAtom_IAtomContainer_IAtom()
        {
            Polarizability pol = new Polarizability();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("NCCN(C)(C)");
            double result = pol.GetPolarizabilitiyFactorForAtom(mol, mol.Atoms[0]);
            Assert.IsNotNull(result);
            result = pol.GetPolarizabilitiyFactorForAtom(mol, mol.Atoms[3]);
            Assert.IsNotNull(result);
        }

        /// <summary>
        ///  A unit test for JUnit with n,n-dimethyl ethylendiamine
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_IAtomContainer_IAtom_Int_Boolean()
        {
            double[] testResult = { 4.73, 6.92 };
            Polarizability pol = new Polarizability();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("NCCN(C)(C)");
            double result = pol.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[0], 100, true);
            Assert.AreEqual(testResult[0], result, 0.01);
            result = pol.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[3], 100, true);
            Assert.AreEqual(testResult[1], result, 0.01);
        }

        /// <summary>
        ///  A unit test for JUnit
        /// </summary>
        [TestMethod(), Ignore()]
        //[TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_IAtomContainer_IAtom_Boolean_IntInt()
        {
            Assert.Fail("Not tested yet");
        }

        /// <summary>
        ///  A unit test for JUnit with n,n-dimethyl ethylendiamine
        /// </summary>
        [TestMethod()]
        public void TestCalculateBondPolarizability_IAtomContainer_IBond()
        {
            Polarizability pol = new Polarizability();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("NCCN(C)(C)");
            double result = pol.CalculateBondPolarizability((IAtomContainer)mol, mol.Bonds[0]);
            Assert.IsNotNull(result);
        }

        /// <summary>
        ///  A unit test for JUnit with methane
        /// </summary>
        [TestMethod()]
        public void TestCalculateKJMeanMolecularPolarizability()
        {
            double testResult = 2.61;
            Polarizability pol = new Polarizability();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C");
            double result = pol.CalculateKJMeanMolecularPolarizability(mol);
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test for JUnit with Ethyl chloride
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Ethyl_chloride()
        {
            double testResult = 4.62; /* from thesis Wolfgang Hanebeck, TUM */
            Polarizability pol = new Polarizability();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCCl");
            double result = pol.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[2], 100, true);
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test for JUnit with Allyl bromide
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Allyl_bromide()
        {
            double testResult = 6.17; /* from thesis Wolfgang Hanebeck, TUM */
            Polarizability pol = new Polarizability();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CCBr");
            double result = pol.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[3], 100, true);
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test for JUnit with Isopentyl iodide
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Isopentyl_iodide()
        {
            double testResult = 8.69; /* from thesis Wolfgang Hanebeck, TUM */
            Polarizability pol = new Polarizability();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C(C)(C)CCI");
            double result = pol.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[5], 100, true);
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test for JUnit with Ethoxy ethane
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Ethoxy_ethane()
        {
            double testResult = 5.21; /* from thesis Wolfgang Hanebeck, TUM */
            Polarizability pol = new Polarizability();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCOCC");
            double result = pol.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[2], 100, true);
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test for JUnit with Ethanolamine
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Ethanolamine()
        {
            double[] testResult = { 4.26, 3.60 }; // from thesis Wolfgang Hanebeck, TUM
            Polarizability pol = new Polarizability();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("NCCO");
            double result = pol.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[3], 100, true);
            Assert.AreEqual(testResult[1], result, 0.01);
            result = pol.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[0], 100, true);
            Assert.AreEqual(testResult[0], result, 0.01);
        }

        /// <summary>
        ///  A unit test for JUnit with Allyl mercaptan
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Allyl_mercaptan()
        {
            double testResult = 6.25; /* from thesis Wolfgang Hanebeck, TUM */
            Polarizability pol = new Polarizability();
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CCS");
            double result = pol.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[3], 100, true);
            Assert.AreEqual(testResult, result, 0.01);
        }
    }
}
