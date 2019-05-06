/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using NCDK.IO;
using NCDK.Isomorphisms;
using NCDK.Sgroups;
using NCDK.Smiles;
using NCDK.Stereo;
using NCDK.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
    [TestClass()]
    public class AtomContainerManipulatorTest
        : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;
        readonly IAtomContainer ac = TestMoleculeFactory.MakeAlphaPinene();

        [TestMethod()]
        public void TestExtractSubstructure()
        {
            IAtomContainer source = TestMoleculeFactory.MakeEthylCyclohexane();
            IAtomContainer ringSubstructure = AtomContainerManipulator.ExtractSubstructure(source, 0, 1, 2, 3, 4, 5);
            Assert.AreEqual(6, ringSubstructure.Atoms.Count);
            Assert.AreEqual(6, ringSubstructure.Bonds.Count);
        }

        // @cdk.bug 1254
        [TestMethod()]
        public void TestGetTotalHydrogenCount_IAtomContainer()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            Assert.AreEqual(6, mol.Atoms.Count);
            Assert.AreEqual(5, mol.Bonds.Count);
            // total includes explicit and implicit (we don't have any implicit to 4 is expected)
            Assert.AreEqual(4, AtomContainerManipulator.GetTotalHydrogenCount(mol));
        }

        [TestMethod()]
        public void TestConvertImplicitToExplicitHydrogens_IAtomContainer()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].ImplicitHydrogenCount = 2;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[1].ImplicitHydrogenCount = 2;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);

            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);
            Assert.AreEqual(6, mol.Atoms.Count);
            Assert.AreEqual(5, mol.Bonds.Count);
        }

        [TestMethod()]
        public void TestConvertImplicitToExplicitHydrogens_IAtomContainer2()
        {
            var mol = builder.NewAtomContainer(); // ethane
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms[1].ImplicitHydrogenCount = 3;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);

            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);
            Assert.AreEqual(8, mol.Atoms.Count);
            Assert.AreEqual(7, mol.Bonds.Count);
        }

        [TestMethod()]
        public void TestGetTotalHydrogenCount_IAtomContainer_zeroImplicit()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].ImplicitHydrogenCount = 0;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[1].ImplicitHydrogenCount = 0;
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            Assert.AreEqual(6, mol.Atoms.Count);
            Assert.AreEqual(5, mol.Bonds.Count);

            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);
            Assert.AreEqual(6, mol.Atoms.Count);
            Assert.AreEqual(5, mol.Bonds.Count);
        }

        [TestMethod()]
        public void TestGetTotalHydrogenCount_IAtomContainer_nullImplicit()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].ImplicitHydrogenCount = null;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[1].ImplicitHydrogenCount = null;
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            Assert.AreEqual(6, mol.Atoms.Count);
            Assert.AreEqual(5, mol.Bonds.Count);

            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);
            Assert.AreEqual(6, mol.Atoms.Count);
            Assert.AreEqual(5, mol.Bonds.Count);
        }

        [TestMethod()]
        public void TestGetTotalHydrogenCount_ImplicitHydrogens()
        {
            var mol = builder.NewAtomContainer();
            var carbon = builder.NewAtom("C");
            carbon.ImplicitHydrogenCount = 4;
            mol.Atoms.Add(carbon);
            Assert.AreEqual(4, AtomContainerManipulator.GetTotalHydrogenCount(mol));
        }

        [TestMethod()]
        public void TestRemoveHydrogens_IAtomContainer()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            foreach (var atom in mol.Atoms)
                atom.ImplicitHydrogenCount = 0;
            mol.IsAromatic = true;

            Assert.AreEqual(6, mol.Atoms.Count);
            IAtomContainer ac = AtomContainerManipulator.RemoveHydrogens(mol);
            Assert.AreEqual(2, ac.Atoms.Count);
            Assert.IsTrue(ac.IsAromatic);
        }

        [TestMethod()]
        public void DontSuppressHydrogensOnPseudoAtoms()
        {
            var mol = builder.NewAtomContainer(); // *[H]
            mol.Atoms.Add(builder.NewPseudoAtom("*"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms[0].ImplicitHydrogenCount = 0;
            mol.Atoms[1].ImplicitHydrogenCount = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            Assert.AreEqual(2, mol.Atoms.Count);
            IAtomContainer ac = AtomContainerManipulator.RemoveHydrogens(mol);
            Assert.AreEqual(2, ac.Atoms.Count);
        }

        [TestMethod()]
        public void SuppressHydrogensKeepsRadicals()
        {
            var mol = builder.NewAtomContainer(); // *[H]
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms[0].ImplicitHydrogenCount = 0;
            mol.Atoms[1].ImplicitHydrogenCount = 1;
            mol.Atoms[2].ImplicitHydrogenCount = 1;
            mol.Atoms[3].ImplicitHydrogenCount = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddSingleElectronTo(mol.Atoms[0]);
            Assert.AreEqual(4, mol.Atoms.Count);
            Assert.AreEqual(1, mol.SingleElectrons.Count);
            IAtomContainer ac = AtomContainerManipulator.RemoveHydrogens(mol);
            Assert.AreEqual(1, ac.Atoms.Count);
            Assert.AreEqual(1, ac.SingleElectrons.Count);
        }

        private static IAtomContainer GetChiralMolTemplate()
        {
            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("Cl"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("Br"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("H"));
            molecule.Atoms.Add(builder.NewAtom("Cl"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[7], BondOrder.Single);

            return molecule;
        }

        [TestMethod()]
        public void TestRemoveNonChiralHydrogens_StereoElement()
        {
            IAtomContainer molecule = GetChiralMolTemplate();
            IAtom[] ligands = new IAtom[]{molecule.Atoms[4], molecule.Atoms[3], molecule.Atoms[2],
                molecule.Atoms[0]};

            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands,
                    TetrahedralStereo.Clockwise);
            molecule.StereoElements.Add(chirality);

            Assert.AreEqual(8, molecule.Atoms.Count);
            IAtomContainer ac = AtomContainerManipulator.RemoveNonChiralHydrogens(molecule);
            Assert.AreEqual(6, ac.Atoms.Count);
        }

        [TestMethod()]
        public void TestRemoveNonChiralHydrogens_StereoParity()
        {
            IAtomContainer molecule = GetChiralMolTemplate();
            molecule.Atoms[1].StereoParity = StereoAtomParities.Minus;

            Assert.AreEqual(8, molecule.Atoms.Count);
            IAtomContainer ac = AtomContainerManipulator.RemoveNonChiralHydrogens(molecule);
            Assert.AreEqual(6, ac.Atoms.Count);
        }

        [TestMethod()]
        public void TestRemoveNonChiralHydrogens_StereoBond()
        {
            IAtomContainer molecule = GetChiralMolTemplate();
            molecule.Bonds[2].Stereo = BondStereo.Up;

            Assert.AreEqual(8, molecule.Atoms.Count);
            IAtomContainer ac = AtomContainerManipulator.RemoveNonChiralHydrogens(molecule);
            Assert.AreEqual(6, ac.Atoms.Count);
        }

        [TestMethod()]
        public void TestRemoveNonChiralHydrogens_StereoBondHeteroAtom()
        {
            IAtomContainer molecule = GetChiralMolTemplate();
            molecule.Bonds[3].Stereo = BondStereo.Up;

            Assert.AreEqual(8, molecule.Atoms.Count);
            IAtomContainer ac = AtomContainerManipulator.RemoveNonChiralHydrogens(molecule);
            Assert.AreEqual(6, ac.Atoms.Count);
        }

        [TestMethod()]
        public void TestRemoveNonChiralHydrogens_IAtomContainer()
        {

            IAtomContainer molecule = GetChiralMolTemplate();

            Assert.AreEqual(8, molecule.Atoms.Count);
            IAtomContainer ac = AtomContainerManipulator.RemoveNonChiralHydrogens(molecule);
            Assert.AreEqual(5, ac.Atoms.Count);
        }

        [TestMethod()]
        public void TestRemoveHydrogensZeroHydrogenCounts()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("Br"));
            mol.Atoms.Add(builder.NewAtom("Br"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);

            mol.Atoms[0].ImplicitHydrogenCount = 0;
            mol.Atoms[1].ImplicitHydrogenCount = 0;
            mol.Atoms[2].ImplicitHydrogenCount = 0;
            mol.Atoms[3].ImplicitHydrogenCount = 0;
            mol.Atoms[4].ImplicitHydrogenCount = 0;
            mol.Atoms[5].ImplicitHydrogenCount = 0;

            Assert.AreEqual(6, mol.Atoms.Count);
            IAtomContainer ac = AtomContainerManipulator.RemoveHydrogens(mol);
            Assert.AreEqual(4, ac.Atoms.Count);
            Assert.IsNotNull(ac.Atoms[0].ImplicitHydrogenCount);
            Assert.IsNotNull(ac.Atoms[1].ImplicitHydrogenCount);
            Assert.IsNotNull(ac.Atoms[2].ImplicitHydrogenCount);
            Assert.IsNotNull(ac.Atoms[3].ImplicitHydrogenCount);
            Assert.AreEqual(0, ac.Atoms[0].ImplicitHydrogenCount.Value);
            Assert.AreEqual(2, ac.Atoms[1].ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, ac.Atoms[2].ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, ac.Atoms[3].ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestGetAllIDs_IAtomContainer()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].Id = "a1";
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[1].Id = "a2";
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms[2].Id = "a3";
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms[3].Id = "a4";
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms[4].Id = "a5";
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms[5].Id = "a6";

            var ids = AtomContainerManipulator.GetAllIDs(mol).ToReadOnlyList();
            Assert.AreEqual(6, ids.Count);
            Assert.IsTrue(ids.Contains("a1"));
            Assert.IsTrue(ids.Contains("a2"));
            Assert.IsTrue(ids.Contains("a3"));
            Assert.IsTrue(ids.Contains("a4"));
            Assert.IsTrue(ids.Contains("a5"));
            Assert.IsTrue(ids.Contains("a6"));
        }

        [TestMethod()]
        public void TestGetAtomArray_IAtomContainer()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));

            IAtom[] atoms = mol.Atoms.ToArray();
            Assert.AreEqual(6, atoms.Length);
            Assert.AreEqual(mol.Atoms[0], atoms[0]);
            Assert.AreEqual(mol.Atoms[1], atoms[1]);
            Assert.AreEqual(mol.Atoms[2], atoms[2]);
            Assert.AreEqual(mol.Atoms[3], atoms[3]);
            Assert.AreEqual(mol.Atoms[4], atoms[4]);
            Assert.AreEqual(mol.Atoms[5], atoms[5]);
        }

        [TestMethod()]
        public void TestGetAtomArray_List()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Double);
            mol.IsAromatic = true;

            IAtom[] atoms = mol.GetConnectedAtoms(mol.Atoms[0]).ToArray();
            Assert.AreEqual(3, atoms.Length);
            Assert.AreEqual(mol.Atoms[1], atoms[0]);
            Assert.AreEqual(mol.Atoms[2], atoms[1]);
            Assert.AreEqual(mol.Atoms[3], atoms[2]);
        }

        [TestMethod()]
        public void TestGetBondArray_List()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Double);
            mol.IsAromatic = true;

            IBond[] bonds = mol.GetConnectedBonds(mol.Atoms[0]).ToArray();
            Assert.AreEqual(3, bonds.Length);
            Assert.AreEqual(mol.Bonds[0], bonds[0]);
            Assert.AreEqual(mol.Bonds[1], bonds[1]);
            Assert.AreEqual(mol.Bonds[2], bonds[2]);
        }

        [TestMethod()]
        public void TestGetBondArray_IAtomContainer()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Double);
            mol.IsAromatic = true;

            IBond[] bonds = mol.Bonds.ToArray();
            Assert.AreEqual(5, bonds.Length);
            Assert.AreEqual(mol.Bonds[0], bonds[0]);
            Assert.AreEqual(mol.Bonds[1], bonds[1]);
            Assert.AreEqual(mol.Bonds[2], bonds[2]);
            Assert.AreEqual(mol.Bonds[3], bonds[3]);
            Assert.AreEqual(mol.Bonds[4], bonds[4]);
        }

        [TestMethod()]
        public void TestGetAtomById_IAtomContainer_String()
        {
            var mol = builder.NewAtomContainer(); // ethene
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].Id = "a1";
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[1].Id = "a2";
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms[2].Id = "a3";
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms[3].Id = "a4";
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms[4].Id = "a5";
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms[5].Id = "a6";
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Double);
            mol.IsAromatic = true;

            Assert.AreEqual(mol.Atoms[0], AtomContainerManipulator.GetAtomById(mol, "a1"));
            Assert.AreEqual(mol.Atoms[1], AtomContainerManipulator.GetAtomById(mol, "a2"));
            Assert.AreEqual(mol.Atoms[2], AtomContainerManipulator.GetAtomById(mol, "a3"));
            Assert.AreEqual(mol.Atoms[3], AtomContainerManipulator.GetAtomById(mol, "a4"));
            Assert.AreEqual(mol.Atoms[4], AtomContainerManipulator.GetAtomById(mol, "a5"));
            Assert.AreEqual(mol.Atoms[5], AtomContainerManipulator.GetAtomById(mol, "a6"));
        }

        /// <summary>
        /// Test removeHydrogens for B2H6, which contains two multiply bonded H.
        /// The old behaviour would removed these but now the bridged hydrogens are
        /// kept.
        /// </summary>
        [TestMethod()]
        public void TestRemoveHydrogensBorane()
        {
            IAtomContainer borane = builder.NewAtomContainer();
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.Atoms.Add(borane.Builder.NewAtom("B"));
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.Atoms.Add(borane.Builder.NewAtom("B"));
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.AddBond(borane.Atoms[0], borane.Atoms[2], BondOrder.Single);
            borane.AddBond(borane.Atoms[1], borane.Atoms[2], BondOrder.Single);
            borane.AddBond(borane.Atoms[2], borane.Atoms[3], BondOrder.Single); // REALLY 3-CENTER-2-ELECTRON
            borane.AddBond(borane.Atoms[2], borane.Atoms[4], BondOrder.Single); // REALLY 3-CENTER-2-ELECTRON
            borane.AddBond(borane.Atoms[3], borane.Atoms[5], BondOrder.Single); // REALLY 3-CENTER-2-ELECTRON
            borane.AddBond(borane.Atoms[4], borane.Atoms[5], BondOrder.Single); // REALLY 3-CENTER-2-ELECTRON
            borane.AddBond(borane.Atoms[5], borane.Atoms[6], BondOrder.Single);
            borane.AddBond(borane.Atoms[5], borane.Atoms[7], BondOrder.Single);
            foreach (var atom in borane.Atoms)
                atom.ImplicitHydrogenCount = 0;
            IAtomContainer ac = AtomContainerManipulator.RemoveHydrogens(borane);

            // bridged hydrogens are now kept
            Assert.AreEqual(4, ac.Atoms.Count, "incorrect atom count");
            Assert.AreEqual(4, ac.Bonds.Count, "incorrect bond count");
            foreach (var atom in ac.Atoms)
            {
                if (atom.AtomicNumber == 1) continue;
                Assert.AreEqual(2, atom.ImplicitHydrogenCount.Value, "incorrect hydrogen count");
            }
        }

        /// <summary>
        /// Test total formal charge.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalFormalCharge_IAtomContainer()
        {
            var parser = CDK.SmilesParser;
            var mol = parser.ParseSmiles("[O-]C([N+])C([N+])C");
            int totalCharge = AtomContainerManipulator.GetTotalFormalCharge(mol);

            Assert.AreEqual(1, totalCharge);
        }

        /// <summary>
        /// Test total Exact Mass.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalExactMass_IAtomContainer()
        {
            var parser = CDK.SmilesParser;
            var mol = parser.ParseSmiles("CCl");
            mol.Atoms[0].ExactMass = 12.00;
            mol.Atoms[1].ExactMass = 34.96885268;
            double totalExactMass = AtomContainerManipulator.GetTotalExactMass(mol);

            Assert.AreEqual(49.992327775, totalExactMass, 0.000001);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetNaturalExactMassNeedsHydrogens()
        {
            var mol = builder.NewAtomContainer();
            var atom = builder.NewAtom("C");
            atom.ImplicitHydrogenCount = null;
            mol.Atoms.Add(atom);
            AtomContainerManipulator.GetNaturalExactMass(mol);
        }

        [TestMethod()]
        public void TestGetNaturalExactMass_IAtomContainer()
        {
            IAtomContainer mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("Cl"));

            mol.Atoms[0].ImplicitHydrogenCount = 4;
            mol.Atoms[1].ImplicitHydrogenCount = 1;

            double expectedMass = 0.0;
            expectedMass += BODRIsotopeFactory.Instance.GetNaturalMass(ChemicalElement.C);
            expectedMass += BODRIsotopeFactory.Instance.GetNaturalMass(ChemicalElement.Cl);
            expectedMass += 5 * BODRIsotopeFactory.Instance.GetNaturalMass(ChemicalElement.H);

            double totalExactMass = AtomContainerManipulator.GetNaturalExactMass(mol);

            Assert.AreEqual(expectedMass, totalExactMass, 0.000001);
        }

        /// <summary>
        /// Test total natural abundance.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalNaturalAbundance_IAtomContainer()
        {
            var parser = CDK.SmilesParser;
            var mol = parser.ParseSmiles("CCl");
            mol.Atoms[0].Abundance = 98.93;
            mol.Atoms[1].Abundance = 75.78;
            double totalAbudance = AtomContainerManipulator.GetTotalNaturalAbundance(mol);

            Assert.AreEqual(0.749432, totalAbudance, 0.000001);
        }

        /// <summary>
        /// Test total positive formal charge.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalPositiveFormalCharge_IAtomContainer()
        {
            var parser = CDK.SmilesParser;
            var mol = parser.ParseSmiles("[O-]C([N+])C([N+])C");
            int totalCharge = AtomContainerManipulator.GetTotalPositiveFormalCharge(mol);

            Assert.AreEqual(2, totalCharge);
        }

        /// <summary>
        /// Test total negative formal charge.
        /// </summary>
        [TestMethod()]
        public void TestGetTotalNegativeFormalCharge_IAtomContainer()
        {
            var parser = CDK.SmilesParser;
            var mol = parser.ParseSmiles("[O-]C([N+])C([N+])C");
            int totalCharge = AtomContainerManipulator.GetTotalNegativeFormalCharge(mol);

            Assert.AreEqual(-1, totalCharge);
        }

        [TestMethod()]
        public void TestGetIntersection_IAtomContainer_IAtomContainer()
        {
            IAtom c1 = builder.NewAtom("C");
            IAtom o = builder.NewAtom("O");
            IAtom c2 = builder.NewAtom("C");
            IAtom c3 = builder.NewAtom("C");

            IBond b1 = builder.NewBond(c1, o);
            IBond b2 = builder.NewBond(o, c2);
            IBond b3 = builder.NewBond(c2, c3);

            IAtomContainer container1 = builder.NewAtomContainer();
            container1.Atoms.Add(c1);
            container1.Atoms.Add(o);
            container1.Atoms.Add(c2);
            container1.Bonds.Add(b1);
            container1.Bonds.Add(b2);
            IAtomContainer container2 = builder.NewAtomContainer();
            container2.Atoms.Add(o);
            container2.Atoms.Add(c3);
            container2.Atoms.Add(c2);
            container2.Bonds.Add(b3);
            container2.Bonds.Add(b2);

            IAtomContainer intersection = AtomContainerManipulator.GetIntersection(container1, container2);
            Assert.AreEqual(2, intersection.Atoms.Count);
            Assert.AreEqual(1, intersection.Bonds.Count);
            Assert.IsTrue(intersection.Contains(b2));
            Assert.IsTrue(intersection.Contains(o));
            Assert.IsTrue(intersection.Contains(c2));
        }

        [TestMethod()]
        public void TestPerceiveAtomTypesAndConfigureAtoms()
        {
            IAtomContainer container = builder.NewAtomContainer();
            container.Atoms.Add(builder.NewAtom("R"));

            // the next should not throw an exception
            try
            {
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            }
            catch (CDKException)
            {
                Assert.Fail($"The {nameof(AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms)} must not throw exceptions when no atom type is perceived.");
            }
        }

        [TestMethod()]
        public void TestPerceiveAtomTypesAndConfigureUnsetProperties()
        {
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            atom.ExactMass = 13.0;
            container.Atoms.Add(atom);
            IAtomType type = builder.NewAtomType("C");
            type.AtomTypeName = "C.sp3";
            type.ExactMass = 12.0;

            AtomContainerManipulator.PercieveAtomTypesAndConfigureUnsetProperties(container);
            Assert.IsNotNull(atom.ExactMass);
            Assert.AreEqual(13.0, atom.ExactMass.Value, 0.1);
            Assert.IsNotNull(atom.AtomTypeName);
            Assert.AreEqual("C.sp3", atom.AtomTypeName);
        }

        [TestMethod()]
        public void TestClearConfig()
        {
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("O");
            IAtom atom3 = builder.NewAtom("C");
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);
            container.Bonds.Add(builder.NewBond(atom1, atom2, BondOrder.Single));
            container.Bonds.Add(builder.NewBond(atom2, atom3, BondOrder.Single));

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            foreach (var atom in container.Atoms)
            {
                Assert.IsTrue(atom.AtomTypeName != null);
                Assert.IsTrue(!atom.Hybridization.IsUnset());
            }

            AtomContainerManipulator.ClearAtomConfigurations(container);
            foreach (var atom in container.Atoms)
            {
                Assert.IsTrue(atom.AtomTypeName == null);
                Assert.IsTrue(atom.Hybridization.IsUnset());
            }
        }

        [TestMethod()]
        public void AtomicNumberIsNotCleared()
        {
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("O");
            IAtom atom3 = builder.NewAtom("C");
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);
            container.Bonds.Add(builder.NewBond(atom1, atom2, BondOrder.Single));
            container.Bonds.Add(builder.NewBond(atom2, atom3, BondOrder.Single));

            AtomContainerManipulator.ClearAtomConfigurations(container);
            foreach (var atom in container.Atoms)
            {
                Assert.IsNotNull(atom.AtomicNumber);
            }
        }

        [TestMethod()]
        public void TestGetMaxBondOrder()
        {
            Assert.AreEqual(BondOrder.Double, AtomContainerManipulator.GetMaximumBondOrder(ac));
        }

        [TestMethod()]
        public void TestGetSBE()
        {
            Assert.AreEqual(12, AtomContainerManipulator.GetSingleBondEquivalentSum(ac));
        }

        [TestMethod()]
        public void TestGetTotalCharge()
        {
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            atom1.Charge = 1.0;
            IAtom atom2 = builder.NewAtom("N");

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Bonds.Add(builder.NewBond(atom1, atom2, BondOrder.Single));

            double totalCharge = AtomContainerManipulator.GetTotalCharge(container);

            Assert.AreEqual(1.0, totalCharge, 0.01);
        }

        // @cdk.bug 1254
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCountExplicitH_Null_IAtom()
        {
            AtomContainerManipulator.CountExplicitHydrogens(null,
                    builder.NewAtom());
        }

        // @cdk.bug 1254
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCountExplicitH_IAtomContainer_Null()
        {
            AtomContainerManipulator.CountExplicitHydrogens(
                    builder.NewAtomContainer(), null);
        }

        [TestMethod()]
        public void TestCountExplicitH()
        {
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            atom1.Charge = 1.0;
            IAtom atom2 = builder.NewAtom("N");

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Bonds.Add(builder.NewBond(atom1, atom2, BondOrder.Single));

            Assert.AreEqual(0, AtomContainerManipulator.CountExplicitHydrogens(container, atom1));
            Assert.AreEqual(0, AtomContainerManipulator.CountExplicitHydrogens(container, atom2));

            for (int i = 0; i < 3; i++)
            {
                IAtom h = builder.NewAtom("H");
                container.Atoms.Add(h);
                container.Bonds.Add(builder.NewBond(atom1, h, BondOrder.Single));
            }
            Assert.AreEqual(3, AtomContainerManipulator.CountExplicitHydrogens(container, atom1));
        }

        [TestMethod()]
        public void TestCountH()
        {
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            atom1.Charge = 1.0;
            IAtom atom2 = builder.NewAtom("N");

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Bonds.Add(builder.NewBond(atom1, atom2, BondOrder.Single));

            // no atom type perception, so implicit count is 0
            Assert.AreEqual(0, AtomContainerManipulator.CountHydrogens(container, atom1));
            Assert.AreEqual(0, AtomContainerManipulator.CountHydrogens(container, atom2));

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            var ha = CDK.HydrogenAdder;
            ha.AddImplicitHydrogens(container);

            Assert.AreEqual(3, AtomContainerManipulator.CountHydrogens(container, atom1));
            Assert.AreEqual(2, AtomContainerManipulator.CountHydrogens(container, atom2));

            for (int i = 0; i < 3; i++)
            {
                IAtom h = builder.NewAtom("H");
                container.Atoms.Add(h);
                container.Bonds.Add(builder.NewBond(atom1, h, BondOrder.Single));
            }
            Assert.AreEqual(6, AtomContainerManipulator.CountHydrogens(container, atom1));

        }

        // @cdk.bug 1254
        [TestMethod()]
        public void TestGetImplicitHydrogenCount_unperceived()
        {
            IAtomContainer container = TestMoleculeFactory.MakeAdenine();
            Assert.AreEqual(0,
                    AtomContainerManipulator.GetImplicitHydrogenCount(container),
                    "Container has not been atom-typed - should have 0 implicit hydrogens");
        }

        // @cdk.bug 1254
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetImplicitHydrogenCount_null()
        {
            AtomContainerManipulator.GetImplicitHydrogenCount(null);
        }

        // @cdk.bug 1254
        [TestMethod()]
        public void TestGetImplicitHydrogenCount_adenine()
        {
            var container = TestMoleculeFactory.MakeAdenine();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            CDK.HydrogenAdder.AddImplicitHydrogens(container);
            Assert.AreEqual(5,
                    AtomContainerManipulator.GetImplicitHydrogenCount(container),
                    "Adenine should have 5 implicit hydrogens");
        }

        [TestMethod()]
        public void TestReplaceAtom()
        {
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            atom1.Charge = 1.0;
            IAtom atom2 = builder.NewAtom("N");

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Bonds.Add(builder.NewBond(atom1, atom2, BondOrder.Single));

            IAtom atom3 = builder.NewAtom("Br");

            AtomContainerManipulator.ReplaceAtomByAtom(container, atom2, atom3);
            Assert.AreEqual(atom3, container.Atoms[1]);
        }

        [TestMethod()]
        public void TestReplaceAtom_lonePair()
        {
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            atom1.Charge = 1.0;
            IAtom atom2 = builder.NewAtom("N");

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Bonds.Add(builder.NewBond(atom1, atom2, BondOrder.Single));
            container.AddLonePairTo(container.Atoms[1]);

            IAtom atom3 = builder.NewAtom("Br");

            AtomContainerManipulator.ReplaceAtomByAtom(container, atom2, atom3);
            Assert.AreEqual(atom3, container.LonePairs[0].Atom);
        }

        [TestMethod()]
        public void TestReplaceAtom_singleElectron()
        {
            IAtomContainer container = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            atom1.Charge = 1.0;
            IAtom atom2 = builder.NewAtom("N");

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Bonds.Add(builder.NewBond(atom1, atom2, BondOrder.Single));
            container.AddSingleElectronTo(container.Atoms[1]);

            IAtom atom3 = builder.NewAtom("Br");

            AtomContainerManipulator.ReplaceAtomByAtom(container, atom2, atom3);
            Assert.AreEqual(atom3, container.SingleElectrons[0].Atom);
        }

        [TestMethod()]
        public void TestReplaceAtom_stereochemistry()
        {
            var smipar = new SmilesParser(builder);
            var mol = smipar.ParseSmiles("N[C@H](CC)O");
            var newAtom = builder.NewAtom("Cl");
            newAtom.ImplicitHydrogenCount = 0;
            AtomContainerManipulator.ReplaceAtomByAtom(mol, mol.Atoms[0], newAtom);
            Assert.AreEqual("Cl[C@H](CC)O", SmilesGenerator.Isomeric.Create(mol));
        }

        [TestMethod()]
        public void TestGetHeavyAtoms_IAtomContainer()
        {
            var container = builder.NewAtomContainer();
            container.Atoms.Add(builder.NewAtom("C"));
            for (int i = 0; i < 4; i++)
                container.Atoms.Add(builder.NewAtom("H"));
            container.Atoms.Add(builder.NewAtom("O"));
            Assert.AreEqual(2, AtomContainerManipulator.GetHeavyAtoms(container).Count);
        }

        /// <summary>
        /// Test removeHydrogensPreserveMultiplyBonded for B2H6, which contains two multiply bonded H.
        /// </summary>
        [TestMethod()]
        public void TestRemoveHydrogensPreserveMultiplyBonded()
        {
            var borane = builder.NewAtomContainer();
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.Atoms.Add(borane.Builder.NewAtom("B"));
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.Atoms.Add(borane.Builder.NewAtom("B"));
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.Atoms.Add(borane.Builder.NewAtom("H"));
            borane.AddBond(borane.Atoms[0], borane.Atoms[2], BondOrder.Single);
            borane.AddBond(borane.Atoms[1], borane.Atoms[2], BondOrder.Single);
            borane.AddBond(borane.Atoms[2], borane.Atoms[3], BondOrder.Single); // REALLY 3-CENTER-2-ELECTRON
            borane.AddBond(borane.Atoms[2], borane.Atoms[4], BondOrder.Single); // REALLY 3-CENTER-2-ELECTRON
            borane.AddBond(borane.Atoms[3], borane.Atoms[5], BondOrder.Single); // REALLY 3-CENTER-2-ELECTRON
            borane.AddBond(borane.Atoms[4], borane.Atoms[5], BondOrder.Single); // REALLY 3-CENTER-2-ELECTRON
            borane.AddBond(borane.Atoms[5], borane.Atoms[6], BondOrder.Single);
            borane.AddBond(borane.Atoms[5], borane.Atoms[7], BondOrder.Single);
            foreach (var atom in borane.Atoms)
                atom.ImplicitHydrogenCount = 0;
            var ac = AtomContainerManipulator.RemoveHydrogens(borane);

            // Should be two connected Bs with H-count == 2 and two explicit Hs.
            Assert.AreEqual(4, ac.Atoms.Count, "incorrect atom count");
            Assert.AreEqual(4, ac.Bonds.Count, "incorrect bond count");

            int b = 0;
            int h = 0;
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                var atom = ac.Atoms[i];
                string sym = atom.Symbol;
                switch (sym)
                {
                    case "B":
                        // Each B has two explicit and two implicit H.
                        b++;
                        Assert.AreEqual(2, atom.ImplicitHydrogenCount.Value, "incorrect hydrogen count");
                        var nbs = ac.GetConnectedAtoms(atom).ToReadOnlyList();
                        Assert.AreEqual(2, nbs.Count, "incorrect connected count");
                        Assert.AreEqual("H", (nbs[0]).Symbol, "incorrect bond");
                        Assert.AreEqual("H", (nbs[1]).Symbol, "incorrect bond");
                        break;
                    case "H":
                        h++;
                        break;
                }
            }
            Assert.AreEqual(2, b, "incorrect no. Bs");
            Assert.AreEqual(2, h, "incorrect no. Hs");
        }

        [TestMethod()]
        public void TestCreateAnyAtomAnyBondAtomContainerIAtomContainer()
        {
            var smiles = "c1ccccc1";
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles(smiles);
            mol = AtomContainerManipulator.CreateAllCarbonAllSingleNonAromaticBondAtomContainer(mol);
            string smiles2 = "C1CCCCC1";
            var mol2 = sp.ParseSmiles(smiles2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(mol, mol2));
        }

        [TestMethod()]
        public void TestAnonymise()
        {
            IAtomContainer cyclohexane = TestMoleculeFactory.MakeCyclohexane();

            cyclohexane.Atoms[0].Symbol = "O";
            cyclohexane.Atoms[2].Symbol = "O";
            cyclohexane.Atoms[1].AtomTypeName = "remove me";
            cyclohexane.Atoms[3].IsAromatic = true;
            cyclohexane.Atoms[4].ImplicitHydrogenCount = 2;
            cyclohexane.Bonds[0].IsSingleOrDouble = true;
            cyclohexane.Bonds[1].IsAromatic = true;

            IAtomContainer anonymous = AtomContainerManipulator.Anonymise(cyclohexane);

            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(anonymous, TestMoleculeFactory.MakeCyclohexane()));

            Assert.AreEqual("C", anonymous.Atoms[0].Symbol);
            Assert.AreEqual("C", anonymous.Atoms[2].Symbol);
            Assert.IsNull(anonymous.Atoms[1].AtomTypeName);
            Assert.AreEqual(0, anonymous.Atoms[4].ImplicitHydrogenCount);
            Assert.IsFalse(anonymous.Atoms[3].IsAromatic);

            Assert.IsFalse(anonymous.Bonds[1].IsAromatic);
            Assert.IsFalse(anonymous.Bonds[1].IsSingleOrDouble);
        }

        [TestMethod()]
        public void Skeleton()
        {
            IAtomContainer adenine = TestMoleculeFactory.MakeAdenine();
            IAtomContainer skeleton = AtomContainerManipulator.Skeleton(adenine);

            Assert.AreNotSame(adenine, skeleton);

            foreach (var bond in skeleton.Bonds)
                Assert.AreEqual(BondOrder.Single, bond.Order);

            for (int i = 0; i < skeleton.Atoms.Count; i++)
            {
                Assert.AreEqual(adenine.Atoms[i].Symbol, skeleton.Atoms[i].Symbol);
            }
        }

        /// <summary>
        /// https://sourceforge.net/p/cdk/mailman/message/20639023/
        /// </summary>
        // @cdk.bug  1969156
        [TestMethod()]
        public void TestOverWriteConfig()
        {
            var filename = "NCDK.Data.MDL.lobtest2.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            var content = reader.Read(builder.NewChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            var exactMass = new Dictionary<IAtom, double?>();

            BODRIsotopeFactory.Instance.ConfigureAtoms(ac);

            foreach (var atom in ac.Atoms)
            {
                exactMass[atom] = atom.ExactMass;
            }

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);

            foreach (var atom in ac.Atoms)
            {
                var expected = exactMass[atom];
                var actual = atom.ExactMass;
                if (expected == null)
                    Assert.IsNull(actual);
                else
                    Assert.IsTrue(Math.Abs(expected.Value - actual.Value) < 0.001);
            }
        }

        [TestMethod()]
        public void SetSingleOrDoubleFlags()
        {
            IAtomContainer biphenyl = TestMoleculeFactory.MakeBiphenyl();
            foreach (var bond in biphenyl.Bonds)
            {
                bond.IsAromatic = true;
            }
            AtomContainerManipulator.SetSingleOrDoubleFlags(biphenyl);
            Assert.IsTrue(biphenyl.IsSingleOrDouble);
            foreach (var atom in biphenyl.Atoms)
            {
                Assert.IsTrue(biphenyl.IsSingleOrDouble);
            }
            int n = 0;
            foreach (var bond in biphenyl.Bonds)
            {
                n += bond.IsSingleOrDouble ? 1 : 0;
            }
            // 13 bonds - the one which joins the two rings is now marked as single
            // or double
            Assert.AreEqual(12, n);
        }

        /// <summary>
        /// Molecular hydrogen is found in the first batch of PubChem entries, and
        /// removal of hydrogen should simply return an empty IAtomContainer, not
        /// throw an NullReferenceException.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// now molecular hydrogen is preserved to avoid information loss.
        /// </note>
        /// </remarks>
        // @cdk.bug 2366528
        [TestMethod()]
        public void TestRemoveHydrogensFromMolecularHydrogen()
        {
            var mol = builder.NewAtomContainer(); // molecular hydrogen
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            Assert.AreEqual(2, mol.Atoms.Count);
            IAtomContainer ac = AtomContainerManipulator.RemoveHydrogens(mol);
            Assert.AreEqual(2, ac.Atoms.Count);
        }

        [TestMethod()]
        public void TestBondOrderSum()
        {
            var parser = CDK.SmilesParser;
            var mol = parser.ParseSmiles("C=CC");
            double bosum = AtomContainerManipulator.GetBondOrderSum(mol, mol.Atoms[0]);
            Assert.AreEqual(2.0, bosum, 0.001);
            bosum = AtomContainerManipulator.GetBondOrderSum(mol, mol.Atoms[1]);
            Assert.AreEqual(3.0, bosum, 0.001);
            bosum = AtomContainerManipulator.GetBondOrderSum(mol, mol.Atoms[2]);
            Assert.AreEqual(1.0, bosum, 0.001);
        }

        [TestMethod()]
        public void ConvertExplicitHydrogenChiralCarbon()
        {
            SmilesParser smipar = CDK.SmilesParser;
            var m = smipar.ParseSmiles("C[C@H](CC)O");
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(m);
            Assert.AreEqual("C([C@](C(C([H])([H])[H])([H])[H])(O[H])[H])([H])([H])[H]", SmilesGenerator.Isomeric.Create(m));
        }

        [TestMethod()]
        public void ConvertExplicitHydrogenSulfoxide()
        {
            SmilesParser smipar = CDK.SmilesParser;
            var m = smipar.ParseSmiles("[S@](=O)(C)CC");
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(m);
            Assert.AreEqual("[S@](=O)(C([H])([H])[H])C(C([H])([H])[H])([H])[H]", SmilesGenerator.Isomeric.Create(m));
        }

        [TestMethod()]
        public void RemoveHydrogensChiralCarbon1()
        {
            AssertRemoveH("C[C@@](CC)([H])O", "C[C@H](CC)O");
        }

        [TestMethod()]
        public void RemoveHydrogensChiralCarbon2()
        {
            AssertRemoveH("C[C@@]([H])(CC)O", "C[C@@H](CC)O");
        }

        [TestMethod()]
        public void RemoveHydrogensChiralCarbon3()
        {
            AssertRemoveH("C[C@@](CC)(O)[H]", "C[C@@H](CC)O");
        }

        [TestMethod()]
        public void RemoveHydrogensChiralCarbon4()
        {
            AssertRemoveH("[H][C@@](C)(CC)O", "[C@@H](C)(CC)O");
        }

        [TestMethod()]
        public void RemoveHydrogensDbTrans1()
        {
            AssertRemoveH("C/C([H])=C([H])/C", "C/C=C/C");
            AssertRemoveH("C\\C([H])=C([H])\\C", "C/C=C/C");
        }

        [TestMethod()]
        public void RemoveHydrogensDbCis1()
        {
            AssertRemoveH("C/C([H])=C([H])\\C", "C/C=C\\C");
            AssertRemoveH("C\\C([H])=C([H])/C", "C/C=C\\C");
        }

        [TestMethod()]
        public void RemoveHydrogensDbTrans2()
        {
            AssertRemoveH("CC(/[H])=C([H])/C", "C/C=C/C");
        }

        [TestMethod()]
        public void RemoveHydrogensDbCis2()
        {
            AssertRemoveH("CC(\\[H])=C([H])/C", "C/C=C\\C");
        }

        [TestMethod()]
        public void RemoveHydrogensDbTrans3()
        {
            AssertRemoveH("CC(/[H])=C(\\[H])C", "C/C=C/C");
        }

        [TestMethod()]
        public void RemoveHydrogensDbCis3()
        {
            AssertRemoveH("CC(\\[H])=C(\\[H])C", "C/C=C\\C");
        }

        // hydrogen isotopes should not be removed
        [TestMethod()]
        public void RemoveHydrogensIsotopes()
        {
            AssertRemoveH("C([H])([2H])([3H])[H]", "C([2H])[3H]");
        }

        // hydrogens with charge should not be removed
        [TestMethod()]
        public void RemoveHydrogensIons()
        {
            AssertRemoveH("C([H])([H+])([H-])[H]", "C([H+])[H-]");
        }

        [TestMethod()]
        public void RemoveHydrogensMolecularH()
        {
            AssertRemoveH("[H][H]", "[H][H]");
            AssertRemoveH("[HH]", "[H][H]");
        }

        [TestMethod()]
        public void TestSgroupSuppressionSRU()
        {
            AssertRemoveH("CCC([H])CC |Sg:n:1,2,3,4:n:ht|", "CCCCC |Sg:n:1,2,3:n:ht|");
        }

        [TestMethod()]
        public void TestSgroupSuppressionSRUUpdated()
        {
            var smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("CCC([H])CC |Sg:n:1,2,3,4:n:ht|");
            AtomContainerManipulator.SuppressHydrogens(mol);
            var sgroups = mol.GetCtabSgroups();
            Assert.IsNotNull(sgroups);
            Assert.AreEqual(1, sgroups.Count);
            Sgroup sgroup = sgroups.First();
            Assert.AreEqual(3, sgroup.Atoms.Count);
        }

        [TestMethod()]
        public void TestSgroupSuppressionPositionalVariation()
        {
            AssertRemoveH("*[H].C1=CC=CC=C1 |m:0:2.3.4|", "*[H].C1=CC=CC=C1 |m:0:2.3.4|");
        }

        [TestMethod()]
        public void TestSgroupSuppressionSRUCrossingBond()
        {
            AssertRemoveH("CCC[H] |Sg:n:2:n:ht|", "CCC[H] |Sg:n:2:n:ht|");
        }

        [TestMethod()]
        public void MolecularWeight()
        {
            SmilesParser smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("[13CH4]CO");
            double molecularWeight = AtomContainerManipulator.GetMolecularWeight(mol);
            double naturalExactMass = AtomContainerManipulator.GetNaturalExactMass(mol);
            var isotopes = BODRIsotopeFactory.Instance;
            foreach (IAtom atom in mol.Atoms)
            {
                if (atom.MassNumber == null)
                    atom.ExactMass = isotopes.GetMajorIsotope(atom.AtomicNumber).ExactMass;
                else
                    isotopes.Configure(atom);
            }
            double exactMass = AtomContainerManipulator.GetTotalExactMass(mol);
            Assert.IsTrue(Math.Abs(molecularWeight - 48.069) < 0.001);
            Assert.IsTrue(Math.Abs(naturalExactMass - 47.076) < 0.001);
            Assert.IsTrue(Math.Abs(exactMass - 48.053) < 0.001);
        }

        [TestMethod()]
        public void RemoveBondStereo()
        {
            SmilesParser smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("[2H]/C=C/[H]");
            AtomContainerManipulator.SuppressHydrogens(mol);
            Assert.IsFalse(mol.StereoElements.Any());
        }

        [TestMethod()]
        public void Keep1Hisotopes()
        {
            SmilesParser smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("[2H]/C=C/[1H]");
            AtomContainerManipulator.SuppressHydrogens(mol);
            Assert.AreEqual(4, mol.Atoms.Count);
        }

        // util for testing hydrogen removal using SMILES
        static void AssertRemoveH(string smiIn, string smiExp)
        {
            SmilesParser smipar = CDK.SmilesParser;
            var m = smipar.ParseSmiles(smiIn);

            string smiAct = new SmilesGenerator().Create(AtomContainerManipulator.RemoveHydrogens(m));

            Assert.AreEqual(smiExp, smiAct);
        }

        [TestMethod()]
        public void GetMassC6Br6()
        {
            var mol = CDK.SmilesParser.ParseSmiles("Brc1c(Br)c(Br)c(Br)c(Br)c1Br");
            Assert.AreEqual(551.485, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MolWeight), 0.001);
            Assert.AreEqual(551.485, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MolWeightIgnoreSpecified), 0.001);
            Assert.AreEqual(545.510, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MonoIsotopic), 0.001);
            Assert.AreEqual(551.503, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MostAbundant), 0.001);
        }

        [TestMethod()]
        public void GetMassCranbin()
        {
            var mol = MolecularFormulaManipulator.GetAtomContainer("C202H315N55O64S6");
            Assert.AreEqual(4730.397, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MolWeight), 0.001);
            Assert.AreEqual(4730.397, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MolWeightIgnoreSpecified), 0.001);
            Assert.AreEqual(4727.140, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MonoIsotopic), 0.001);
            Assert.AreEqual(4729.147, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MostAbundant), 0.001);
        }

        [TestMethod()]
        public void GetMassCranbinSpecIsotopes()
        {
            var mol = MolecularFormulaManipulator.GetAtomContainer("[12]C200[13]C2[1]H315[14]N55[16]O64[32]S6");
            Assert.AreEqual(4729.147, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MolWeight), 0.001);
            Assert.AreEqual(4730.397, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MolWeightIgnoreSpecified), 0.001);
            Assert.AreEqual(4729.147, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MonoIsotopic), 0.001);
            Assert.AreEqual(4729.147, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MostAbundant), 0.001);
        }

        [TestMethod()]
        public void GetMassCranbinMixedSpecIsotopes()
        {
            var aa = MolecularFormulaManipulator.GetAtomContainer("[13]C");
            var mol = MolecularFormulaManipulator.GetAtomContainer("C200[13]C2H315N55O64S6");
            Assert.AreEqual(4732.382, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MolWeight), 0.001);
            Assert.AreEqual(4730.397, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MolWeightIgnoreSpecified), 0.001);
            Assert.AreEqual(4729.147, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MonoIsotopic), 0.001);
            Assert.AreEqual(4731.154, AtomContainerManipulator.GetMass(mol, MolecularWeightTypes.MostAbundant), 0.001);
        }
    }
}
