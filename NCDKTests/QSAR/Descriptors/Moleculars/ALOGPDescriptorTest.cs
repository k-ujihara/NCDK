/* Copyright (C) 2007  Rajarshi Guha <rajarshi@users.sourceforge.net>
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
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class ALogPDescriptorTest
        : MolecularDescriptorTest<ALogPDescriptor>
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;

        /// <summary>
        /// This test is actually testing 1-cholorpropane.
        /// </summary>
        // @cdk.inchi InChI=1S/C3H7Cl/c1-2-3-4/h2-3H2,1H3
        [TestMethod()]
        public void TestChloroButane()
        {
            var mol = builder.NewAtomContainer();
            var c1 = ChemObjectBuilder.Instance.NewAtom("C");
            var c2 = ChemObjectBuilder.Instance.NewAtom("C");
            var c3 = ChemObjectBuilder.Instance.NewAtom("C");
            var cl = ChemObjectBuilder.Instance.NewAtom("Cl");
            mol.Atoms.Add(c1);
            mol.Atoms.Add(c2);
            mol.Atoms.Add(c3);
            mol.Atoms.Add(cl);

            mol.Bonds.Add(new Bond(c1, c2));
            mol.Bonds.Add(new Bond(c2, c3));
            mol.Bonds.Add(new Bond(c3, cl));

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            var result = CreateDescriptor(mol).Calculate();
            Assert.AreEqual(0.5192, result.ALogP, 0.0001);
            Assert.AreEqual(19.1381, result.AMR, 0.0001);
        }
    }
}
