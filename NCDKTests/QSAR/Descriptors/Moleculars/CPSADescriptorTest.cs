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
using NCDK.Tools.Manipulator;

using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    public class CPSADescriptorTest : MolecularDescriptorTest
    {
        public CPSADescriptorTest()
        {
            SetDescriptor(typeof(CPSADescriptor));
        }

        [TestMethod()]
        public void TestCPSA()
        {
            string filename = "NCDK.Data.HIN.benzene.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            ArrayResult<double> retval = (ArrayResult<double>)Descriptor.Calculate(ac).Value;
            // Console.Out.WriteLine("Num ret = "+retval.Count); for (int i = 0; i <
            // retval.Count; i++) { Console.Out.WriteLine( retval[i] ); }

            Assert.AreEqual(0, retval[28], 0.0001);
            Assert.AreEqual(1, retval[27], 0.0001);
            Assert.AreEqual(0, retval[26], 0.0001);
            Assert.AreEqual(356.8849, retval[25], 0.0001);
        }

        [TestMethod()]
        public void TestChargedMolecule()
        {
            string filename = "NCDK.Data.MDL.cpsa-charged.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            ArrayResult<double> retval = (ArrayResult<double>)Descriptor.Calculate(ac).Value;
            int ndesc = retval.Length;
            for (int i = 0; i < ndesc; i++)
                Assert.IsTrue(retval[i] != double.NaN);
        }

        [TestMethod()]
        public void TestUnChargedMolecule()
        {
            string filename = "NCDK.Data.MDL.cpsa-uncharged.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            ArrayResult<double> retval = (ArrayResult<double>)Descriptor.Calculate(ac).Value;
            int ndesc = retval.Length;
            for (int i = 0; i < ndesc; i++)
                Assert.IsTrue(retval[i] != double.NaN);
        }
    }
}
