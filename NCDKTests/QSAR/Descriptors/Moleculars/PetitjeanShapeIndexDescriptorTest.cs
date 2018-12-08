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
    public class PetitjeanShapeIndexDescriptorTest : MolecularDescriptorTest<PetitjeanShapeIndexDescriptor>
    {
        [TestMethod()]
        public void TestPetitjeanShapeIndexDescriptor()
        {
            // first molecule is nbutane, second is naphthalene
            var filename = "NCDK.Data.MDL.petitejean.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            var result = CreateDescriptor(ac).Calculate();
            Assert.AreEqual(0.5, result.TopologicalShapeIndex, 0.00001);
            Assert.AreEqual(0.606477, result.GeometricShapeIndex, 0.000001);

            ac = cList[1];
            result = CreateDescriptor(ac).Calculate();
            Assert.AreEqual(0.666666, result.TopologicalShapeIndex, 0.000001);
            Assert.AreEqual(0.845452, result.GeometricShapeIndex, 0.000001);

        }

        [TestMethod()]
        public void TestPetiteJeanShapeNo3D()
        {
            var sp = CDK.SmilesParser;
            var atomContainer = sp.ParseSmiles("CCCOCCC(O)=O");
            var result = CreateDescriptor(atomContainer).Calculate();
            Assert.IsTrue(double.IsNaN(result.GeometricShapeIndex));
        }
    }
}
