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
using NCDK.Silent;
using NCDK.Smiles;

namespace NCDK.Charges
{
    // @cdk.module test-charges
    // @author     chhoppe
    // @cdk.created    2004-11-04
    [TestClass()]
    public class PolarizabilityTest : CDKTestCase
    {
        [TestMethod()]
        public void TestGetPolarizabilitiyFactorForAtom_IAtomContainer_IAtom()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("NCCN(C)(C)");
            double result = Polarizability.GetPolarizabilitiyFactorForAtom(mol, mol.Atoms[0]);
            Assert.IsFalse(double.IsNaN(result));
            result = Polarizability.GetPolarizabilitiyFactorForAtom(mol, mol.Atoms[3]);
            Assert.IsFalse(double.IsNaN(result));
        }

        /// <summary>
        /// A unit test with n,n-dimethyl ethylendiamine
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_IAtomContainer_IAtom_Int_Boolean()
        {
            double[] testResult = { 4.73, 6.92 };
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("NCCN(C)(C)");
            double result = Polarizability.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[0], 100, true);
            Assert.AreEqual(testResult[0], result, 0.01);
            result = Polarizability.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[3], 100, true);
            Assert.AreEqual(testResult[1], result, 0.01);
        }

        [TestMethod(), Ignore()]
        public void TestCalculateGHEffectiveAtomPolarizability_IAtomContainer_IAtom_Boolean_IntInt()
        {
            Assert.Fail("Not tested yet");
        }

        /// <summary>
        /// A unit test with n,n-dimethyl ethylendiamine
        /// </summary>
        [TestMethod()]
        public void TestCalculateBondPolarizability_IAtomContainer_IBond()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("NCCN(C)(C)");
            double result = Polarizability.CalculateBondPolarizability((IAtomContainer)mol, mol.Bonds[0]);
            Assert.IsFalse(double.IsNaN(result));
        }

        /// <summary>
        /// A unit test with methane
        /// </summary>
        [TestMethod()]
        public void TestCalculateKJMeanMolecularPolarizability()
        {
            double testResult = 2.61;
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C");
            double result = Polarizability.CalculateKJMeanMolecularPolarizability(mol);
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test with Ethyl chloride
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Ethyl_chloride()
        {
            double testResult = 4.62; /* from thesis Wolfgang Hanebeck, TUM */
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCl");
            double result = Polarizability.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[2], 100, true);
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test with Allyl bromide
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Allyl_bromide()
        {
            double testResult = 6.17; /* from thesis Wolfgang Hanebeck, TUM */
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CCBr");
            double result = Polarizability.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[3], 100, true);
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test with Isopentyl iodide
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Isopentyl_iodide()
        {
            double testResult = 8.69; /* from thesis Wolfgang Hanebeck, TUM */
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C(C)(C)CCI");
            double result = Polarizability.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[5], 100, true);
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test with Ethoxy ethane
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Ethoxy_ethane()
        {
            double testResult = 5.21; /* from thesis Wolfgang Hanebeck, TUM */
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCOCC");
            double result = Polarizability.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[2], 100, true);
            Assert.AreEqual(testResult, result, 0.01);
        }

        /// <summary>
        /// A unit test with Ethanolamine
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Ethanolamine()
        {
            double[] testResult = { 4.26, 3.60 }; // from thesis Wolfgang Hanebeck, TUM
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("NCCO");
            double result = Polarizability.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[3], 100, true);
            Assert.AreEqual(testResult[1], result, 0.01);
            result = Polarizability.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[0], 100, true);
            Assert.AreEqual(testResult[0], result, 0.01);
        }

        /// <summary>
        /// A unit test with Allyl mercaptan
        /// </summary>
        [TestMethod()]
        public void TestCalculateGHEffectiveAtomPolarizability_Allyl_mercaptan()
        {
            double testResult = 6.25; /* from thesis Wolfgang Hanebeck, TUM */
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CCS");
            double result = Polarizability.CalculateGHEffectiveAtomPolarizability(mol, mol.Atoms[3], 100, true);
            Assert.AreEqual(testResult, result, 0.01);
        }
    }
}
