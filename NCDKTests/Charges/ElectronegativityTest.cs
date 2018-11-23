/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@yahoo.es>
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
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.Charges
{
    // @cdk.module test-charges
    [TestClass()]
    public class ElectronegativityTest : CDKTestCase
    {
        private IChemObjectBuilder builder = CDK.Builder;

        public ElectronegativityTest()
            : base()
        { }

        [TestMethod()]
        public void TestElectronegativity()
        {
            Assert.IsNotNull(new Electronegativity());
        }

        [TestMethod()]
        public void TestElectronegativity_Int_Int()
        {
            Assert.IsNotNull(new Electronegativity(6, 50));
        }

        // @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        public void TestCalculateSigmaElectronegativity_IAtomContainer_IAtom()
        {
            double[] testResult = { 11.308338, 8.7184094, 7.5289848, 7.5289848, 7.5289848 };
            Electronegativity pe = new Electronegativity();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("F"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            for (int i = 0; i < molecule.Atoms.Count; i++)
                Assert.AreEqual(testResult[i], pe.CalculateSigmaElectronegativity(molecule, molecule.Atoms[i]), 0.001);
        }

        // @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        public void TestCalculateSigmaElectronegativity_IAtomContainer_IAtom_Int_Int()
        {
            double[] testResult = { 11.308338, 8.7184094, 7.5289848, 7.5289848, 7.5289848 };
            Electronegativity pe = new Electronegativity();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("F"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                Assert.AreEqual(testResult[i],
                        pe.CalculateSigmaElectronegativity(molecule, molecule.Atoms[i], 6, 50), 0.001);
            }
        }

        [TestMethod()]
        public void TestGetMaxIterations()
        {
            Electronegativity pe = new Electronegativity();
            Assert.AreEqual(6, pe.MaxIterations);
        }

        [TestMethod()]
        public void TestGetMaxResonStruc()
        {
            Electronegativity pe = new Electronegativity();
            Assert.AreEqual(50, pe.MaxResonanceStructures);
        }

        [TestMethod()]
        public void TestSetMaxIterations_Int()
        {
            Electronegativity pe = new Electronegativity();
            int maxIter = 10;
            pe.MaxIterations = maxIter;
            Assert.AreEqual(maxIter, pe.MaxIterations);
        }

        [TestMethod()]
        public void TestSetMaxResonStruc_Int()
        {
            Electronegativity pe = new Electronegativity();
            int maxRes = 10;
            pe.MaxResonanceStructures = maxRes;
            Assert.AreEqual(maxRes, pe.MaxResonanceStructures);
        }
    }
}
