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
    public class EccentricConnectivityIndexDescriptorTest : MolecularDescriptorTest
    {
        public EccentricConnectivityIndexDescriptorTest()
        {
            SetDescriptor(typeof(EccentricConnectivityIndexDescriptor));
        }

        [TestMethod()]
        public void TestEhccentricConnectivityIndex()
        {
            string filename = "NCDK.Data.HIN.gravindex.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            AddImplicitHydrogens(ac);

            IntegerResult retval = (IntegerResult)Descriptor.Calculate(ac).GetValue();
            //Debug.WriteLine(retval.Value);

            Assert.AreEqual(254, retval.Value, 0);
        }
    }
}
