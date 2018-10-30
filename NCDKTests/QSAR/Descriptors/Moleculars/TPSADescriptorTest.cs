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
using NCDK.Silent;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class TPSADescriptorTest : MolecularDescriptorTest
    {
        private SmilesParser sp;

        public TPSADescriptorTest()
        {
            sp = CDK.SilentSmilesParser;
            SetDescriptor(typeof(TPSADescriptor));
            Descriptor.Parameters = new object[] { true };
        }

        [TestMethod()]
        public void TestTPSA1()
        {
            var mol = sp.ParseSmiles("O=C(O)CC");
            AddExplicitHydrogens(mol);
            Assert.AreEqual(37.29, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.01);
        }

        [TestMethod()]
        public void TestTPSA2()
        {
            var mol = sp.ParseSmiles("C=NC(CC#N)N(C)C");
            AddExplicitHydrogens(mol);
            Assert.AreEqual(39.39, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.01);
        }

        [TestMethod()]
        public void TestTPSA3()
        {
            var mol = sp.ParseSmiles("CCCN(=O)=O");
            AddExplicitHydrogens(mol);
            Assert.AreEqual(45.82, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.01);
        }

        [TestMethod()]
        public void TestTPSA4()
        {
            var mol = sp.ParseSmiles("C#N=CC(CNC)N1CC1");
            AddExplicitHydrogens(mol);
            Assert.AreEqual(28.632, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.01);
        }

        [TestMethod()]
        public void TestTPSA5()
        {
            var mol = sp.ParseSmiles("c1ccncc1");
            AddExplicitHydrogens(mol);
            Assert.AreEqual(12.892, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.01);
        }

        [TestMethod()]
        public void TestTPSA6()
        {
            var mol = sp.ParseSmiles("[H][N+]([H])(C)C");//at:  16
            AddExplicitHydrogens(mol);
            Assert.AreEqual(16.61, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void TestTPSA7()
        {
            var mol = sp.ParseSmiles("C(I)I");//at:  16
            AddExplicitHydrogens(mol);
            Assert.AreEqual(0.0, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void TestTPSA8()
        {
            var mol = sp.ParseSmiles("C(O)O");//at:  16
            AddExplicitHydrogens(mol);
            Assert.AreEqual(40.45, ((Result<double>)Descriptor.Calculate(mol).Value).Value, 0.1); //at:  16
        }

        [TestMethod()]
        public void TestRing()
        {
            sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("C1CCCC1CCC2CCCNC2");
            AddExplicitHydrogens(mol);
            var dv = Descriptor.Calculate(mol);
            Assert.IsNotNull(dv);
        }
    }
}
