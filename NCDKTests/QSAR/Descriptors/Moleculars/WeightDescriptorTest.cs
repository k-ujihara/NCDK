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
using NCDK.QSAR.Results;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs a test for the WeightDescriptor.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class WeightDescriptorTest : MolecularDescriptorTest
    {
        public WeightDescriptorTest()
        {
            SetDescriptor(typeof(WeightDescriptor));
        }

        [TestMethod()]
        public void TestWeightDescriptor()
        {
            Descriptor.Parameters = new object[] { "*" };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCC");
            Assert.AreEqual(44.06, ((DoubleResult)Descriptor.Calculate(mol).Value).Value, 0.1);
        }

        // @cdk.bug 2185475
        [TestMethod()]
        public void TestNoHydrogens()
        {
            Descriptor.Parameters = new object[] { "*" };
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            Assert.AreEqual(12.00, ((DoubleResult)Descriptor.Calculate(mol).Value).Value, 0.1);
        }

        // @cdk.bug 2185475
        [TestMethod()]
        public void TestExplicitHydrogens()
        {
            Descriptor.Parameters = new object[] { "*" };
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            Assert.AreEqual(16.01, ((DoubleResult)Descriptor.Calculate(mol).Value).Value, 0.1);
        }

        // @cdk.bug 2185475
        [TestMethod()]
        public void TestImplicitHydrogens()
        {
            Descriptor.Parameters = new object[] { "*" };
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].ImplicitHydrogenCount = 4;
            Assert.AreEqual(16.01, ((DoubleResult)Descriptor.Calculate(mol).Value).Value, 0.1);
        }
    }
}
