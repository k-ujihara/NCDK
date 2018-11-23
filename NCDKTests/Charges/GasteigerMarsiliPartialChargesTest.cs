/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.IO;
using NCDK.Silent;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System.IO;
using System.Linq;

namespace NCDK.Charges
{
    // @cdk.module test-charges
    // @author     chhoppe
    // @cdk.created    2004-11-04
    [TestClass()]
    public class GasteigerMarsiliPartialChargesTest : CDKTestCase
    {
        private IChemObjectBuilder builder = CDK.Builder;

        /// <summary>
        /// A unit test with methylenfluoride
        /// </summary>
        // @cdk.inchi InChI=1/CH3F/c1-2/h1H3
        [TestMethod()]
        public void TestCalculateCharges_IAtomContainer()
        {
            double[] testResult = { 0.07915, -0.25264, 0.05783, 0.05783, 0.05783 };

            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("F"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            peoe.CalculateCharges(molecule);
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                //Debug.WriteLine("Charge for atom:"+i+" S:"+mol.GetAtomAt(i).Symbol+" Charge:"+mol.GetAtomAt(i).Charge);
                Assert.AreEqual(testResult[i], molecule.Atoms[i].Charge.Value, 0.01);
            }
        }

        [TestMethod()]
        public void TestAssignGasteigerMarsiliSigmaPartialCharges_IAtomContainer_Boolean()
        {
            double[] testResult = { 0.07915, -0.25264, 0.05783, 0.05783, 0.05783 };

            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("F"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            peoe.AssignGasteigerMarsiliSigmaPartialCharges(molecule, true);
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                //Debug.WriteLine("Charge for atom:"+i+" S:"+mol.GetAtomAt(i).Symbol+" Charge:"+mol.GetAtomAt(i).Charge);
                Assert.AreEqual(testResult[i], molecule.Atoms[i].Charge.Value, 0.01);
            }
        }

        [TestMethod()]
        public void TestAssignGasteigerSigmaMarsiliFactors_IAtomContainer()
        {
            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();

            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms[0].Charge = 0.0;
            molecule.Atoms.Add(new Atom("F"));
            molecule.Atoms[1].Charge = 0.0;
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);
            foreach (var atom in molecule.Atoms)
                atom.Charge = 0.0;

            Assert.IsNotNull(peoe.AssignGasteigerSigmaMarsiliFactors(molecule).Length);
        }

        [TestMethod()]
        public void TestGetMaxGasteigerIters()
        {
            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
            Assert.AreEqual(20, peoe.MaxGasteigerIterations, 0.01);
        }

        [TestMethod()]
        public void TestGetMaxGasteigerDamp()
        {
            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
            Assert.AreEqual(20, peoe.MaxGasteigerIterations, 0.01);
        }

        [TestMethod()]
        public void TestGetChiCatHydrogen()
        {
            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
            Assert.AreEqual(20, peoe.MaxGasteigerIterations, 0.01);
        }

        [TestMethod()]
        public void TestGetStepSize()
        {
            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
            Assert.AreEqual(5, peoe.StepSize);
        }

        [TestMethod()]
        public void TestSetMaxGasteigerIters_Double()
        {
            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
            double MX_ITERATIONS = 10;
            peoe.MaxGasteigerIterations = MX_ITERATIONS;
            Assert.AreEqual(MX_ITERATIONS, peoe.MaxGasteigerIterations, 0.01);
        }

        [TestMethod()]
        public void TestSetMaxGasteigerDamp_Double()
        {
            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
            double MX_DAMP = 1;
            peoe.MaxGasteigerDamp = MX_DAMP;
            Assert.AreEqual(MX_DAMP, peoe.MaxGasteigerDamp, 0.01);
        }

        [TestMethod()]
        public void TestSetChiCatHydrogen_Double()
        {
            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
            double DEOC_HYDROGEN = 22;
            peoe.ChiCatHydrogen = DEOC_HYDROGEN;
            Assert.AreEqual(DEOC_HYDROGEN, peoe.ChiCatHydrogen, 0.01);
        }
  
        [TestMethod()]
        public void TestSetStepSize()
        {
            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
            int StepSize = 22;
            peoe.StepSize = StepSize;
            Assert.AreEqual(StepSize, peoe.StepSize);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestUndefinedPartialCharge()
        {
            string filename = "NCDK.Data.MDL.burden_undefined.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = reader.Read(new ChemFile());
            reader.Close();
            var cList = ChemFileManipulator.GetAllAtomContainers(content);
            IAtomContainer ac = cList.First();

            Assert.IsNotNull(ac);
            AddExplicitHydrogens(ac);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
            Aromaticity.CDKLegacy.Apply(ac);

            AddExplicitHydrogens(ac);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
            CDK.LonePairElectronChecker.Saturate(ac);

            GasteigerMarsiliPartialCharges peoe = new GasteigerMarsiliPartialCharges();
            peoe.CalculateCharges(ac);
        }
    }
}
