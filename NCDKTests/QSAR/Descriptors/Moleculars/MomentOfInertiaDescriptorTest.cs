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
using NCDK.Default;
using NCDK.IO;
using NCDK.QSAR.Results;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class MomentOfInertiaDescriptorTest : MolecularDescriptorTest
    {
        public MomentOfInertiaDescriptorTest()
        {
            SetDescriptor(typeof(MomentOfInertiaDescriptor));
        }

        // @cdk.bug 1956139
        // @throws InvalidSmilesException
        [TestMethod()]
        public void TestMOIFromSmiles()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCCC");
            DescriptorValue value = Descriptor.Calculate(mol);
            Assert.IsNotNull(value.Exception, "The Exception should be non-null since we don't have 3D coords");
        }

        [TestMethod()]
        public void TestMomentOfInertia1()
        {
            string filename = "NCDK.Data.HIN.gravindex.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            DoubleArrayResult retval = (DoubleArrayResult)Descriptor.Calculate(ac).Value;

            Assert.AreEqual(1820.692519, retval[0], 0.00001);
            Assert.AreEqual(1274.532522, retval[1], 0.00001);
            Assert.AreEqual(979.210423, retval[2], 0.00001);
            Assert.AreEqual(1.428517, retval[3], 0.00001);
            Assert.AreEqual(1.859347, retval[4], 0.00001);
            Assert.AreEqual(1.301592, retval[5], 0.00001);
            Assert.AreEqual(5.411195, retval[6], 0.00001);
        }

        [TestMethod()]
        public void TestMomentOfInertia2()
        {
            string filename = "NCDK.Data.HIN.momi2.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            DoubleArrayResult retval = (DoubleArrayResult)Descriptor.Calculate(ac).Value;

            Assert.AreEqual(10068.419360, retval[0], 0.00001);
            Assert.AreEqual(9731.078356, retval[1], 0.00001);
            Assert.AreEqual(773.612799, retval[2], 0.00001);
            Assert.AreEqual(1.034666, retval[3], 0.00001);
            Assert.AreEqual(13.014804, retval[4], 0.00001);
            Assert.AreEqual(12.578745, retval[5], 0.00001);
            Assert.AreEqual(8.2966226, retval[6], 0.00001);
        }
    }
}
