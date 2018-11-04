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
using NCDK.Silent;
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
    public class PetitjeanShapeIndexDescriptorTest : MolecularDescriptorTest
    {
        public PetitjeanShapeIndexDescriptorTest()
        {
            SetDescriptor(typeof(PetitjeanShapeIndexDescriptor));
        }

        [TestMethod()]
        public void TestPetitjeanShapeIndexDescriptor()
        {
            // first molecule is nbutane, second is naphthalene
            string filename = "NCDK.Data.MDL.petitejean.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            IAtomContainer ac = (IAtomContainer)cList[0];

            var result = Descriptor.Calculate(ac);
            ArrayResult<double> dar = (ArrayResult<double>)result.Value;
            Assert.AreEqual(0.5, dar[0], 0.00001);
            Assert.AreEqual(0.606477, dar[1], 0.000001);

            ac = (IAtomContainer)cList[1];
            result = Descriptor.Calculate(ac);
            dar = (ArrayResult<double>)result.Value;
            Assert.AreEqual(0.666666, dar[0], 0.000001);
            Assert.AreEqual(0.845452, dar[1], 0.000001);

        }

        [TestMethod()]
        public void TestPetiteJeanShapeNo3D()
        {
            var sp = CDK.SmilesParser;
            var atomContainer = sp.ParseSmiles("CCCOCCC(O)=O");
            var result = Descriptor.Calculate(atomContainer);
            ArrayResult<double> dar = (ArrayResult<double>)result.Value;
            Assert.IsTrue(double.IsNaN(dar[1]));
        }
    }
}
