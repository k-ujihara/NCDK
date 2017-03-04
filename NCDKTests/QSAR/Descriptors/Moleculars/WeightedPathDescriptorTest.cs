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
using NCDK.Default;
using NCDK.IO;
using NCDK.QSAR.Result;
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
    public class WeightedPathDescriptorTest : MolecularDescriptorTest
    {
        public WeightedPathDescriptorTest()
        {
            SetDescriptor(typeof(WeightedPathDescriptor));
        }

        [TestMethod()]
        public void TestWeightedPathDescriptor()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = null;
            DescriptorValue value = null;
            DoubleArrayResult result = null;

            mol = sp.ParseSmiles("CCCC");
            value = Descriptor.Calculate(mol);
            result = (DoubleArrayResult)value.GetValue();
            Assert.AreEqual(6.871320, result[0], 0.000001);
            Assert.AreEqual(1.717830, result[1], 0.000001);
            Assert.AreEqual(0.0, result[2], 0.000001);
            Assert.AreEqual(0.0, result[3], 0.000001);
            Assert.AreEqual(0.0, result[4], 0.000001);

            string filename = "NCDK.Data.MDL.wpo.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            IChemFile content = (IChemFile)reader.Read(new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            mol = (IAtomContainer)cList[0];
            mol = AtomContainerManipulator.RemoveHydrogens(mol);

            value = Descriptor.Calculate(mol);
            result = (DoubleArrayResult)value.GetValue();
            Assert.AreEqual(18.42026, result[0], 0.00001);
            Assert.AreEqual(1.842026, result[1], 0.00001);
            Assert.AreEqual(13.45733, result[2], 0.00001);
            Assert.AreEqual(13.45733, result[3], 0.00001);
            Assert.AreEqual(0, result[4], 0.00001);

            filename = "NCDK.Data.MDL.wpn.sdf";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins);
            content = (IChemFile)reader.Read(new ChemFile());
            cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            mol = (IAtomContainer)cList[0];
            mol = AtomContainerManipulator.RemoveHydrogens(mol);
            value = Descriptor.Calculate(mol);
            result = (DoubleArrayResult)value.GetValue();
            Assert.AreEqual(26.14844, result[0], 0.00001);
            Assert.AreEqual(1.867746, result[1], 0.00001);
            Assert.AreEqual(19.02049, result[2], 0.00001);
            Assert.AreEqual(0, result[3], 0.000001);
            Assert.AreEqual(19.02049, result[4], 0.00001);
        }
    }
}
