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
using NCDK.Tools.Manipulator;
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

            var peoe = new GasteigerMarsiliPartialCharges();

            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            peoe.CalculateCharges(molecule);
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                Assert.AreEqual(testResult[i], molecule.Atoms[i].Charge.Value, 0.01);
            }
        }

        [TestMethod()]
        public void TestAssignGasteigerMarsiliSigmaPartialCharges_IAtomContainer_Boolean()
        {
            double[] testResult = { 0.07915, -0.25264, 0.05783, 0.05783, 0.05783 };

            var peoe = new GasteigerMarsiliPartialCharges();

            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("F"));
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
            var peoe = new GasteigerMarsiliPartialCharges();

            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[0].Charge = 0.0;
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms[1].Charge = 0.0;
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);
            foreach (var atom in molecule.Atoms)
                atom.Charge = 0.0;

            Assert.AreNotEqual(0, peoe.AssignGasteigerSigmaMarsiliFactors(molecule).Length);
        }

        [TestMethod()]
        public void TestGetMaxGasteigerIters()
        {
            var peoe = new GasteigerMarsiliPartialCharges();
            Assert.AreEqual(20, peoe.MaxGasteigerIterations, 0.01);
        }

        [TestMethod()]
        public void TestGetMaxGasteigerDamp()
        {
            var peoe = new GasteigerMarsiliPartialCharges();
            Assert.AreEqual(20, peoe.MaxGasteigerIterations, 0.01);
        }

        [TestMethod()]
        public void TestGetChiCatHydrogen()
        {
            var peoe = new GasteigerMarsiliPartialCharges();
            Assert.AreEqual(20, peoe.MaxGasteigerIterations, 0.01);
        }

        [TestMethod()]
        public void TestGetStepSize()
        {
            var peoe = new GasteigerMarsiliPartialCharges();
            Assert.AreEqual(5, peoe.StepSize);
        }

        [TestMethod()]
        public void TestSetMaxGasteigerIters_Double()
        {
            var peoe = new GasteigerMarsiliPartialCharges();
            double MX_ITERATIONS = 10;
            peoe.MaxGasteigerIterations = MX_ITERATIONS;
            Assert.AreEqual(MX_ITERATIONS, peoe.MaxGasteigerIterations, 0.01);
        }

        [TestMethod()]
        public void TestSetMaxGasteigerDamp_Double()
        {
            var peoe = new GasteigerMarsiliPartialCharges();
            double MX_DAMP = 1;
            peoe.MaxGasteigerDamp = MX_DAMP;
            Assert.AreEqual(MX_DAMP, peoe.MaxGasteigerDamp, 0.01);
        }

        [TestMethod()]
        public void TestSetChiCatHydrogen_Double()
        {
            var peoe = new GasteigerMarsiliPartialCharges();
            double DEOC_HYDROGEN = 22;
            peoe.ChiCatHydrogen = DEOC_HYDROGEN;
            Assert.AreEqual(DEOC_HYDROGEN, peoe.ChiCatHydrogen, 0.01);
        }
  
        [TestMethod()]
        public void TestSetStepSize()
        {
            var peoe = new GasteigerMarsiliPartialCharges();
            int StepSize = 22;
            peoe.StepSize = StepSize;
            Assert.AreEqual(StepSize, peoe.StepSize);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestUndefinedPartialCharge()
        {
            var filename = "NCDK.Data.MDL.burden_undefined.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            var content = reader.Read(builder.NewChemFile());
            reader.Close();
            var cList = ChemFileManipulator.GetAllAtomContainers(content);
            var ac = cList.First();

            Assert.IsNotNull(ac);
            AddExplicitHydrogens(ac);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
            Aromaticity.CDKLegacy.Apply(ac);

            AddExplicitHydrogens(ac);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
            CDK.LonePairElectronChecker.Saturate(ac);

            var peoe = new GasteigerMarsiliPartialCharges();
            peoe.CalculateCharges(ac);
        }
    }
}
