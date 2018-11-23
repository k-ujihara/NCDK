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
    [TestClass()]
    public class MomentOfInertiaDescriptorTest : MolecularDescriptorTest<MomentOfInertiaDescriptor>
    {
        // @cdk.bug 1956139
        [TestMethod()]
        public void TestMOIFromSmiles()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCC");
            try
            {
                var value = CreateDescriptor(mol).Calculate();
                Assert.Fail();
            }
            catch (ThreeDRequiredException)
            {
            }
        }

        [TestMethod()]
        public void TestMomentOfInertia1()
        {
            string filename = "NCDK.Data.HIN.gravindex.hin";
            IChemFile content;
            using (var reader = new HINReader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            var retval = CreateDescriptor(ac).Calculate().Values;

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
            IChemFile content;
            using (var reader = new HINReader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            var retval = CreateDescriptor(ac).Calculate().Values;

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
