/*
 * Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Silent;
using NCDK.IO;
using NCDK.QSAR.Results;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

using System;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class BCUTDescriptorTest : MolecularDescriptorTest
    {
        public BCUTDescriptorTest()
                : base()
        {
            SetDescriptor(typeof(BCUTDescriptor));
        }

        [TestMethod()]
        public void TestBCUT()
        {
            string filename = "NCDK.Data.HIN.gravindex.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            object[] parameters = new object[] { 2, 2, true };
            Descriptor.Parameters = parameters;
            var descriptorValue = Descriptor.Calculate(ac);

            ArrayResult<double> retval = (ArrayResult<double>)descriptorValue.Value;
            Assert.IsNotNull(retval);
            /* Console.Out.WriteLine("Num ret = "+retval.Count); */
            for (int i = 0; i < retval.Length; i++)
            {
                Assert.IsTrue(Math.Abs(0.0 - retval[i]) > 0.0000001, "The returned value must be non-zero");
            }

            var names = descriptorValue.Names;
            foreach (var name in names)
                Assert.IsNotNull(name);

            // Assert.AreEqual(1756.5060703860984,
            // ((Double)retval[0]).Value, 0.00000001);
            // Assert.AreEqual(41.91069159994975,
            // ((Double)retval[1]).Value, 0.00000001);
            // Assert.AreEqual(12.06562671430088,
            // ((Double)retval[2]).Value, 0.00000001);
            // Assert.AreEqual(1976.6432599699767,
            // ((Double)retval[3]).Value, 0.00000001);
            // Assert.AreEqual(44.45945636161082,
            // ((Double)retval[4]).Value, 0.00000001);
            // Assert.AreEqual(12.549972243701887,
            // ((Double)retval[5]).Value, 0.00000001);
            // Assert.AreEqual(4333.097373073368,
            // ((Double)retval[6]).Value, 0.00000001);
            // Assert.AreEqual(65.82626658920714,
            // ((Double)retval[7]).Value, 0.00000001);
            // Assert.AreEqual(16.302948232909483,
            // ((Double)retval[8]).Value, 0.00000001);
        }

        [TestMethod()]
        public void TestExtraEigenvalues()
        {
            string filename = "NCDK.Data.HIN.gravindex.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            object[] parameters = new object[] { 0, 25, true };
            Descriptor.Parameters = parameters;
            var descriptorValue = Descriptor.Calculate(ac);

            ArrayResult<double> retval = (ArrayResult<double>)descriptorValue.Value;
            int nheavy = 20;

            Assert.AreEqual(75, retval.Length);
            for (int i = 0; i < nheavy; i++)
                Assert.IsTrue(retval[i] != double.NaN);
            for (int i = nheavy; i < nheavy + 5; i++)
            {
                Assert.IsTrue(double.IsNaN(retval[i]), "Extra eigenvalue should have been NaN");
            }
        }

        [TestMethod()]
        public void TestAromaticity()
        {
            SetDescriptor(typeof(BCUTDescriptor));

            string smiles1 = "c1ccccc1";
            string smiles2 = "C1=CC=CC=C1";

            var sp = CDK.SilentSmilesParser;
            var mol1 = sp.ParseSmiles(smiles1);
            var mol2 = sp.ParseSmiles(smiles2);

            AddExplicitHydrogens(mol1);
            AddExplicitHydrogens(mol2);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            Aromaticity.CDKLegacy.Apply(mol1);
            Aromaticity.CDKLegacy.Apply(mol2);

            ArrayResult<double> result1 = (ArrayResult<double>)Descriptor.Calculate(mol1).Value;
            ArrayResult<double> result2 = (ArrayResult<double>)Descriptor.Calculate(mol2).Value;

            Assert.AreEqual(result1.Length, result2.Length);
            for (int i = 0; i < result1.Length; i++)
            {
                Assert.AreEqual(result1[i], result2[i], 0.01, $"element {i} does not match");
            }
        }

        [TestMethod()]
        public void TestHAddition()
        {
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("C=1C=CC(=CC1)CNC2=CC=C(C=C2N(=O)=O)S(=O)(=O)C(Cl)(Cl)Br");
            ArrayResult<double> result1 = (ArrayResult<double>)Descriptor.Calculate(mol).Value;
            for (int i = 0; i < result1.Length; i++)
                Assert.IsTrue(result1[i] != double.NaN);
        }

        // @cdk.bug 3489559
        [TestMethod()]
        public void TestUndefinedValues()
        {
            string filename = "NCDK.Data.MDL.burden_undefined.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = reader.Read(new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            Assert.IsNotNull(ac);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
            AddExplicitHydrogens(ac);
            Aromaticity.CDKLegacy.Apply(ac);

            Exception e = Descriptor.Calculate(ac).Exception;
            Assert.IsNotNull(e);
            // make sure exception was a NPE etc.
            Assert.AreEqual("Could not calculate partial charges: Partial charge not-supported for element: 'As'.", e.Message);
        }
    }
}
