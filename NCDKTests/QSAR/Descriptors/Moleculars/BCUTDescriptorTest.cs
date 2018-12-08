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
using NCDK.IO;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class BCUTDescriptorTest : MolecularDescriptorTest<BCUTDescriptor>
    {
        public BCUTDescriptor CreateDescriptor(IAtomContainer mol, bool checkAromaticity) => new BCUTDescriptor(mol, checkAromaticity);

        [TestMethod()]
        public void TestBCUT()
        {
            var filename = "NCDK.Data.HIN.gravindex.hin";
            IChemFile content;
            using (var reader = new HINReader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            var descriptor = CreateDescriptor(ac, true);
            var descriptorValue = descriptor.Calculate(2, 2);

            var retval = descriptorValue.Values;
            Assert.IsNotNull(retval);
            foreach (var v in retval)
                Assert.IsTrue(Math.Abs(0.0 - v) > 0.0000001, "The returned value must be non-zero");

            var names = descriptorValue.Keys;
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
            var filename = "NCDK.Data.HIN.gravindex.hin";
            IChemFile content;
            using (var reader = new HINReader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            var descriptor = CreateDescriptor(ac, true);
            var descriptorValue = descriptor.Calculate(0, 25);

            var retval = descriptorValue.Values;
            int nheavy = 20;

            Assert.AreEqual(75, retval.Count);
            foreach (var v in retval)
                Assert.IsTrue(v != double.NaN);
            for (int i = nheavy; i < nheavy + 5; i++)
            {
                Assert.IsTrue(double.IsNaN(retval[i]), "Extra eigenvalue should have been NaN");
            }
        }

        [TestMethod()]
        public void TestAromaticity()
        {
            string smiles1 = "c1ccccc1";
            string smiles2 = "C1=CC=CC=C1";

            var sp = CDK.SmilesParser;
            var mol1 = sp.ParseSmiles(smiles1);
            var mol2 = sp.ParseSmiles(smiles2);

            AddExplicitHydrogens(mol1);
            AddExplicitHydrogens(mol2);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            Aromaticity.CDKLegacy.Apply(mol1);
            Aromaticity.CDKLegacy.Apply(mol2);

            var result1 = CreateDescriptor(mol1).Calculate().Values;
            var result2 = CreateDescriptor(mol2).Calculate().Values;

            Assert.AreEqual(result1.Count, result2.Count);
            for (int i = 0; i < result1.Count; i++)
            {
                Assert.AreEqual(result1[i], result2[i], 0.01, $"element {i} does not match");
            }
        }

        [TestMethod()]
        public void TestHAddition()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=1C=CC(=CC1)CNC2=CC=C(C=C2N(=O)=O)S(=O)(=O)C(Cl)(Cl)Br");
            var result1 = CreateDescriptor(mol).Calculate().Values;
            foreach (var v in result1)
                Assert.IsTrue(v != double.NaN);
        }

        // @cdk.bug 3489559
        [TestMethod()]
        public void TestUndefinedValues()
        {
            var filename = "NCDK.Data.MDL.burden_undefined.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            Assert.IsNotNull(ac);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
            AddExplicitHydrogens(ac);
            Aromaticity.CDKLegacy.Apply(ac);

            var e = CreateDescriptor(ac).Calculate().Exception;
            Assert.IsNotNull(e);
            // make sure exception was a NPE etc.
            Assert.AreEqual("Could not calculate partial charges: Partial charge not-supported for element: 'As'.", e.Message);
        }
    }
}
