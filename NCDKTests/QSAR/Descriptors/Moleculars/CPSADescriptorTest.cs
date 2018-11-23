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
using NCDK.IO;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    public class CPSADescriptorTest : MolecularDescriptorTest<CPSADescriptor>
    {
        IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestCPSA()
        {
            string filename = "NCDK.Data.HIN.benzene.hin";
            IChemFile content;
            using (var reader = new HINReader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            var retval = CreateDescriptor(ac).Calculate().Values;

            Assert.AreEqual(0, retval[28], 0.0001);
            Assert.AreEqual(1, retval[27], 0.0001);
            Assert.AreEqual(0, retval[26], 0.0001);
            Assert.AreEqual(356.8849, retval[25], 0.0001);
        }

        [TestMethod()]
        public void TestChargedMolecule()
        {
            string filename = "NCDK.Data.MDL.cpsa-charged.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            var retval = CreateDescriptor(ac).Calculate().Values;
            int ndesc = retval.Count;
            for (int i = 0; i < ndesc; i++)
                Assert.IsTrue(retval[i] != double.NaN);
        }

        [TestMethod()]
        public void TestUnChargedMolecule()
        {
            string filename = "NCDK.Data.MDL.cpsa-uncharged.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            var retval = CreateDescriptor(ac).Calculate().Values;
            int ndesc = retval.Count;
            for (int i = 0; i < ndesc; i++)
                Assert.IsTrue(retval[i] != double.NaN);
        }
    }
}
