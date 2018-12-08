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
using NCDK.IO;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class ZagrebIndexDescriptorTest : MolecularDescriptorTest<ZagrebIndexDescriptor>
    {
        [TestMethod()]
        public void TestZagrebIndexDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=C(O)CC");
            Assert.AreEqual(16, CreateDescriptor(mol).Calculate().Value, 0.0001);
        }

        [TestMethod()]
        public void Test2Dvs3D()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O1C2C34C(C(C1O)CCCc1cc(cc(c1)C(F)(F)F)C(F)(F)F)CCC(C3CCC(O2)(OO4)C)C");

            AddExplicitHydrogens(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            double value2D = CreateDescriptor(mol).Calculate().Value;

            var filename = "NCDK.Data.MDL.cpsa-uncharged.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            mol = cList[0];
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var value3D = CreateDescriptor(mol).Calculate().Value;

            Assert.AreEqual(value2D, value3D, 0.001);
        }
    }
}
