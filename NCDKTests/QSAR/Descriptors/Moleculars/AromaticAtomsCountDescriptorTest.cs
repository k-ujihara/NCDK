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
using NCDK.QSAR.Result;
using NCDK.Smiles;
using NCDK.Templates;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class AromaticAtomsCountDescriptorTest : MolecularDescriptorTest
    {
        public AromaticAtomsCountDescriptorTest()
        {
            SetDescriptor(typeof(AromaticAtomsCountDescriptor));
        }

        [TestMethod()]
        public void TestAromaticAtomsCountDescriptor()
        {
            object[] parameters = new object[] { true };
            Descriptor.Parameters = parameters;
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCOc1ccccc1"); // ethanol
            Assert.AreEqual(6, ((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
        }

        [TestMethod()]
        public void TestViaFlags()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();
            foreach (var atom in molecule.Atoms)
            {
                atom.IsAromatic = true;
            }
            Assert.AreEqual(6, ((IntegerResult)Descriptor.Calculate(molecule).GetValue()).Value);
        }
    }
}
