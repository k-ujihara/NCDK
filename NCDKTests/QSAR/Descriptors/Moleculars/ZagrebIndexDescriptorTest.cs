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
using NCDK.Aromaticities;
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
    public class ZagrebIndexDescriptorTest : MolecularDescriptorTest
    {
        public ZagrebIndexDescriptorTest()
        {
            SetDescriptor(typeof(ZagrebIndexDescriptor));
        }

        [TestMethod()]
        public void TestZagrebIndexDescriptor()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("O=C(O)CC");
            Assert.AreEqual(16, ((DoubleResult)Descriptor.Calculate(mol).GetValue()).Value, 0.0001);
        }

        [TestMethod()]
        public void Test2Dvs3D()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("O1C2C34C(C(C1O)CCCc1cc(cc(c1)C(F)(F)F)C(F)(F)F)CCC(C3CCC(O2)(OO4)C)C");

            AddExplicitHydrogens(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            double value2D = ((DoubleResult)Descriptor.Calculate(mol).GetValue()).Value;

            string filename = "NCDK.Data.MDL.cpsa-uncharged.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            mol = (IAtomContainer)cList[0];
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            double value3D = ((DoubleResult)Descriptor.Calculate(mol).GetValue()).Value;

            Assert.AreEqual(value2D, value3D, 0.001);
        }
    }
}
