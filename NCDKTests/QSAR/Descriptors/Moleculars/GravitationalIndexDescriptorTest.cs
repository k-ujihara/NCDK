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
using NCDK.QSAR.Result;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class GravitationalIndexDescriptorTest : MolecularDescriptorTest
    {
        public GravitationalIndexDescriptorTest()
        {
            SetDescriptor(typeof(GravitationalIndexDescriptor));
        }

        [TestMethod()]
        public void TestGravitationalIndex()
        {
            string filename = "NCDK.Data.HIN.gravindex.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            DoubleArrayResult retval = (DoubleArrayResult)Descriptor.Calculate(ac).Value;

            Assert.AreEqual(1756.5060703860984, retval[0], 0.00000001);
            Assert.AreEqual(41.91069159994975, retval[1], 0.00000001);
            Assert.AreEqual(12.06562671430088, retval[2], 0.00000001);
            Assert.AreEqual(1976.6432599699767, retval[3], 0.00000001);
            Assert.AreEqual(44.45945636161082, retval[4], 0.00000001);
            Assert.AreEqual(12.549972243701887, retval[5], 0.00000001);
            Assert.AreEqual(4333.097373073368, retval[6], 0.00000001);
            Assert.AreEqual(65.82626658920714, retval[7], 0.00000001);
            Assert.AreEqual(16.302948232909483, retval[8], 0.00000001);
        }
    }
}
