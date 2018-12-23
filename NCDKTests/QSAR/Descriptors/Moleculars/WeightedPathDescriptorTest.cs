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
using NCDK.IO;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class WeightedPathDescriptorTest : MolecularDescriptorTest<WeightedPathDescriptor>
    {
        [TestMethod()]
        public void TestWeightedPathDescriptor()
        {
            var sp = CDK.SmilesParser;
            {
                var mol = sp.ParseSmiles("CCCC");
                var result = CreateDescriptor().Calculate(mol);
                var values = result.Values;
                Assert.AreEqual(6.871320, values[0], 0.000001);
                Assert.AreEqual(1.717830, values[1], 0.000001);
                Assert.AreEqual(0.0, values[2], 0.000001);
                Assert.AreEqual(0.0, values[3], 0.000001);
                Assert.AreEqual(0.0, values[4], 0.000001);
            }

            {
                var filename = "NCDK.Data.MDL.wpo.sdf";
                IChemFile content;
                using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
                {
                    content = reader.Read(CDK.Builder.NewChemFile());
                }
                var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
                var mol = cList[0];
                mol = AtomContainerManipulator.RemoveHydrogens(mol);
                var result = CreateDescriptor().Calculate(mol);
                var values = result.Values;
                Assert.AreEqual(18.42026, values[0], 0.00001);
                Assert.AreEqual(1.842026, values[1], 0.00001);
                Assert.AreEqual(13.45733, values[2], 0.00001);
                Assert.AreEqual(13.45733, values[3], 0.00001);
                Assert.AreEqual(0, values[4], 0.00001);
            }

            {
                var filename = "NCDK.Data.MDL.wpn.sdf";
                IChemFile content;
                using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
                {
                    content = reader.Read(CDK.Builder.NewChemFile());
                }
                var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
                var mol = cList[0];
                mol = AtomContainerManipulator.RemoveHydrogens(mol);
                var result = CreateDescriptor().Calculate(mol);
                var values = result.Values;
                Assert.AreEqual(26.14844, values[0], 0.00001);
                Assert.AreEqual(1.867746, values[1], 0.00001);
                Assert.AreEqual(19.02049, values[2], 0.00001);
                Assert.AreEqual(0, values[3], 0.000001);
                Assert.AreEqual(19.02049, values[4], 0.00001);
            }
        }
    }
}
