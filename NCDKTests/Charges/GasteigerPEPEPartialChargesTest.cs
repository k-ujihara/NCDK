/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@yahoo.es>
 *
 *  Contact: cdk-devel@list.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.Charges
{
    // @author Miguel Rojas
    // @cdk.module test-charges
    // @cdk.created 2008-18-05
    [TestClass()]
    public class GasteigerPEPEPartialChargesTest : CDKTestCase
    {
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        
        /// <summary>
        /// A unit test for JUnit with methylenfluoride
        /// </summary>
        // @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        public void TestCalculateCharges_IAtomContainer()
        {
            double[] testResult = { 0.0, 0.0, 0.0, 0.0, 0.0 };

            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("F"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            LonePairElectronChecker.Saturate(molecule);

            peoe.CalculateCharges(molecule);
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                //Debug.WriteLine("Charge for atom:"+i+" S:"+mol.GetAtomAt(i).Symbol+" Charge:"+mol.GetAtomAt(i).Charge);
                Assert.AreEqual(testResult[i], molecule.Atoms[i].Charge.Value, 0.01);
            }
        }

        // @cdk.bug 2013689
        [TestMethod()]
        public void TestAromaticBondOrders()
        {
            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();

            string smiles1 = "c1ccccc1";
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol1 = sp.ParseSmiles(smiles1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            Aromaticity.CDKLegacy.Apply(mol1);
            AddExplicitHydrogens(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            LonePairElectronChecker.Saturate(mol1);

            List<bool> oldBondOrders = new List<bool>();
            for (int i = 0; i < mol1.Bonds.Count; i++)
                oldBondOrders.Add(mol1.Bonds[i].IsAromatic);

            peoe.CalculateCharges(mol1);

            List<bool> newBondOrders = new List<bool>();
            for (int i = 0; i < mol1.Bonds.Count; i++)
                newBondOrders.Add(mol1.Bonds[i].IsAromatic);

            for (int i = 0; i < oldBondOrders.Count; i++)
            {
                Assert.AreEqual(oldBondOrders[i], newBondOrders[i], "bond " + i + " does not match");
            }
        }

        [TestMethod()]
        public void TestAromaticAndNonAromatic()
        {
            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();

            string smiles1 = "c1ccccc1";
            string smiles2 = "C1=CC=CC=C1";

            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol1 = sp.ParseSmiles(smiles1);
            IAtomContainer mol2 = sp.ParseSmiles(smiles2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            Aromaticity.CDKLegacy.Apply(mol1);
            Aromaticity.CDKLegacy.Apply(mol2);

            AddExplicitHydrogens(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            LonePairElectronChecker.Saturate(mol1);

            AddExplicitHydrogens(mol2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            LonePairElectronChecker.Saturate(mol2);

            peoe.CalculateCharges(mol1);
            peoe.CalculateCharges(mol2);
            for (int i = 0; i < mol1.Atoms.Count; i++)
            {
                Assert.AreEqual(mol1.Atoms[i].Charge.Value, mol2.Atoms[i].Charge.Value, 0.01, "charge on atom " + i + " does not match");
            }
        }

        [TestMethod()]
        public void TestAssignGasteigerPiPartialCharges_IAtomContainer_Boolean()
        {
            double[] testResult = { 0.0, 0.0, 0.0, 0.0, 0.0 };

            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("F"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            LonePairElectronChecker.Saturate(molecule);

            peoe.AssignGasteigerPiPartialCharges(molecule, true);
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                //Debug.WriteLine("Charge for atom:"+i+" S:"+mol.GetAtomAt(i).Symbol+" Charge:"+mol.GetAtomAt(i).Charge);
                Assert.AreEqual(testResult[i], molecule.Atoms[i].Charge.Value, 0.01);
            }
        }

        [TestMethod()]
        public void TestGetMaxGasteigerIters()
        {
            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();

            Assert.AreEqual(8, peoe.MaxGasteigerIterations);
        }

        [TestMethod()]
        public void TestGetMaxResoStruc()
        {
            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();

            Assert.AreEqual(50, peoe.MaxResonanceStructures);
        }

        [TestMethod()]
        public void TestGetStepSize()
        {
            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();
            Assert.AreEqual(5, peoe.StepSize);
        }

        [TestMethod()]
        public void TestSetMaxGasteigerIters_Double()
        {
            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();
            int MX_ITERATIONS = 10;
            peoe.MaxGasteigerIterations = MX_ITERATIONS;
            Assert.AreEqual(MX_ITERATIONS, peoe.MaxGasteigerIterations);
        }

        [TestMethod()]
        public void TestSetMaxResoStruc_Int()
        {
            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();
            int MX_RESON = 1;
            peoe.MaxResonanceStructures = MX_RESON;
            Assert.AreEqual(MX_RESON, peoe.MaxResonanceStructures);
        }

        [TestMethod()]
        public void TestSetStepSize()
        {
            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();
            int StepSize = 22;
            peoe.StepSize = StepSize;
            Assert.AreEqual(StepSize, peoe.StepSize);
        }

        [TestMethod()]
        public void TestAssignrPiMarsilliFactors_IAtomContainerSet()
        {
            GasteigerPEPEPartialCharges peoe = new GasteigerPEPEPartialCharges();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("F"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            LonePairElectronChecker.Saturate(molecule);
            foreach (var atom in molecule.Atoms)
                atom.Charge = 0;

            var set = builder.NewAtomContainerSet();
            set.Add(molecule);
            set.Add(molecule);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            LonePairElectronChecker.Saturate(molecule);

            Assert.IsNotNull(peoe.AssignrPiMarsilliFactors(set));
        }
    }
}
