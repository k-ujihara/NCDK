/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using NCDK.Default;
using NCDK.IO;
using NCDK.Numerics;
using NCDK.Smiles;
using NCDK.Stereo;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Geometries.CIP
{
    // @cdk.module test-cip
    [TestClass()]
    public class CIPToolTest : CDKTestCase
    {
        static SmilesParser smiles = new SmilesParser(Silent.ChemObjectBuilder.Instance);
        static IAtomContainer molecule;
        static ILigand[] ligands;

        static CIPToolTest()
        {
            molecule = smiles.ParseSmiles("ClC(Br)(I)[H]");
            VisitedAtoms visitedAtoms = new VisitedAtoms();
            ILigand ligand1 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[4]);
            ILigand ligand2 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[3]);
            ILigand ligand3 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[2]);
            ILigand ligand4 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[0]);
            ligands = new ILigand[] { ligand1, ligand2, ligand3, ligand4 };
        }

        [TestMethod()]
        public void TestCheckIfAllLigandsAreDifferent()
        {
            Assert.IsTrue(CIPTool.CheckIfAllLigandsAreDifferent(ligands));
        }

        [TestMethod()]
        public void TestCheckIfAllLigandsAreDifferent_False()
        {
            ILigand[] sameLigands = new ILigand[] { ligands[0], ligands[0], ligands[1], ligands[2] };
            Assert.IsFalse(CIPTool.CheckIfAllLigandsAreDifferent(sameLigands));
        }

        [TestMethod()]
        public void TestOrder()
        {
            ILigand[] ligandCopy = CIPTool.Order(ligands);
            Assert.AreEqual("H", ligandCopy[0].GetLigandAtom().Symbol);
            Assert.AreEqual("Cl", ligandCopy[1].GetLigandAtom().Symbol);
            Assert.AreEqual("Br", ligandCopy[2].GetLigandAtom().Symbol);
            Assert.AreEqual("I", ligandCopy[3].GetLigandAtom().Symbol);
        }

        [TestMethod()]
        public void TestGetCIPChirality()
        {
            LigancyFourChirality chirality = new LigancyFourChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            CIPTool.CIPChirality rsChirality = CIPTool.GetCIPChirality(chirality);
            Assert.AreEqual(CIPTool.CIPChirality.S, rsChirality);
        }

        [TestMethod()]
        public void TestGetCIPChirality_Anti()
        {
            ILigand[] antiLigands = new ILigand[] { ligands[0], ligands[1], ligands[3], ligands[2] };

            LigancyFourChirality chirality = new LigancyFourChirality(molecule.Atoms[1], antiLigands,
                    TetrahedralStereo.AntiClockwise);
            CIPTool.CIPChirality rsChirality = CIPTool.GetCIPChirality(chirality);
            Assert.AreEqual(CIPTool.CIPChirality.S, rsChirality);
        }

        [TestMethod()]
        public void TestGetCIPChirality_ILigancyFourChirality()
        {
            List<IAtom> ligandAtoms = new List<IAtom>();
            foreach (var ligand in ligands)
                ligandAtoms.Add(ligand.GetLigandAtom());
            ITetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1],
                ligandAtoms, TetrahedralStereo.Clockwise);
            CIPTool.CIPChirality rsChirality = CIPTool.GetCIPChirality(molecule, chirality);
            Assert.AreEqual(CIPTool.CIPChirality.S, rsChirality);
        }

        [TestMethod()]
        public void TestGetCIPChirality_Anti_ILigancyFourChirality()
        {
            ILigand[] antiLigands = new ILigand[] { ligands[0], ligands[1], ligands[3], ligands[2] };
            List<IAtom> ligandAtoms = new List<IAtom>();
            foreach (var ligand in antiLigands)
                ligandAtoms.Add(ligand.GetLigandAtom());

            ITetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1],
                ligandAtoms, TetrahedralStereo.AntiClockwise);
            CIPTool.CIPChirality rsChirality = CIPTool.GetCIPChirality(molecule, chirality);
            Assert.AreEqual(CIPTool.CIPChirality.S, rsChirality);
        }

        [TestMethod()]
        public void TestGetCIPChirality_DoubleBond_Together()
        {
            IAtomContainer container = new SmilesParser(Silent.ChemObjectBuilder.Instance).ParseSmiles("CCC(C)=C(C)CC");
            CIPTool.CIPChirality label = CIPTool.GetCIPChirality(
                    container,
                    new DoubleBondStereochemistry(container.GetBond(container.Atoms[2], container.Atoms[4]),
                            new IBond[]{container.GetBond(container.Atoms[2], container.Atoms[3]),
                                container.GetBond(container.Atoms[4], container.Atoms[5])},
                                    DoubleBondConformation.Together));
            Assert.AreEqual(CIPTool.CIPChirality.Z, label);
        }

        [TestMethod()]
        public void TestGetCIPChirality_DoubleBond_Opposite()
        {
            IAtomContainer container = new SmilesParser(Silent.ChemObjectBuilder.Instance).ParseSmiles("CCC(C)=C(C)CC");
            CIPTool.CIPChirality label = CIPTool.GetCIPChirality(
                    container,
                    new DoubleBondStereochemistry(container.GetBond(container.Atoms[2], container.Atoms[4]),
                            new IBond[]{container.GetBond(container.Atoms[2], container.Atoms[3]),
                                container.GetBond(container.Atoms[4], container.Atoms[6])},
                            DoubleBondConformation.Opposite));
            Assert.AreEqual(CIPTool.CIPChirality.Z, label);
        }

        [TestMethod()]
        public void Label()
        {
            IAtomContainer container = new SmilesParser(Silent.ChemObjectBuilder.Instance)
                    .ParseSmiles("C/C=C/[C@@H](C)C(/C)=C(/C)C[C@H](C)O");
            CIPTool.Label(container);
            Assert.AreEqual("R", container.Atoms[3].GetProperty<string>(CDKPropertyName.CIP_Descriptor));
            Assert.AreEqual("S", container.Atoms[10].GetProperty<string>(CDKPropertyName.CIP_Descriptor));
            Assert.AreEqual("E", container.GetBond(container.Atoms[1], container.Atoms[2]).GetProperty<string>(CDKPropertyName.CIP_Descriptor));
            Assert.AreEqual("Z", container.GetBond(container.Atoms[5], container.Atoms[7]).GetProperty<string>(CDKPropertyName.CIP_Descriptor));
        }

        [TestMethod()]
        public void TestDefineLigancyFourChirality()
        {
            LigancyFourChirality chirality = CIPTool.DefineLigancyFourChirality(molecule, 1, 0, 2, 3, 4,
                    TetrahedralStereo.AntiClockwise);
            Assert.AreEqual(molecule.Atoms[1], chirality.ChiralAtom);
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, chirality.Stereo);
            ILigand[] ligands = chirality.Ligands;
            Assert.AreEqual(molecule, ligands[0].GetAtomContainer());
            Assert.AreEqual(molecule.Atoms[0], ligands[0].GetLigandAtom());
            Assert.AreEqual(molecule.Atoms[1], ligands[0].GetCentralAtom());
            Assert.AreEqual(molecule, ligands[1].GetAtomContainer());
            Assert.AreEqual(molecule.Atoms[2], ligands[1].GetLigandAtom());
            Assert.AreEqual(molecule.Atoms[1], ligands[1].GetCentralAtom());
            Assert.AreEqual(molecule, ligands[2].GetAtomContainer());
            Assert.AreEqual(molecule.Atoms[3], ligands[2].GetLigandAtom());
            Assert.AreEqual(molecule.Atoms[1], ligands[2].GetCentralAtom());
            Assert.AreEqual(molecule, ligands[3].GetAtomContainer());
            Assert.AreEqual(molecule.Atoms[4], ligands[3].GetLigandAtom());
            Assert.AreEqual(molecule.Atoms[1], ligands[3].GetCentralAtom());
        }

        [TestMethod()]
        public void TestDefineLigand()
        {
            ILigand ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 1, 2);
            Assert.AreEqual(molecule, ligand.GetAtomContainer());
            Assert.AreEqual(molecule.Atoms[1], ligand.GetCentralAtom());
            Assert.AreEqual(molecule.Atoms[2], ligand.GetLigandAtom());
        }

        /**
         * Tests if it returns the right number of ligands, for single bonds only.
         */
        [TestMethod()]
        public void TestGetLigandLigands()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC(C)C(CC)(C(C)(C)C)[H]");
            ILigand ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 1);
            ILigand[] sideChains = CIPTool.GetLigandLigands(ligand);
            Assert.AreEqual(2, sideChains.Length);
            ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 4);
            sideChains = CIPTool.GetLigandLigands(ligand);
            Assert.AreEqual(1, sideChains.Length);
            ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 6);
            sideChains = CIPTool.GetLigandLigands(ligand);
            Assert.AreEqual(3, sideChains.Length);
            ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 10);
            sideChains = CIPTool.GetLigandLigands(ligand);
            Assert.AreEqual(0, sideChains.Length);
        }

        /**
         * Tests if it returns the right number of ligands, for single bonds only.
         */
        [TestMethod()]
        public void TestGetLigandLigands_VisitedTracking()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC(C)C(CC)(C(C)(C)C)[H]");
            ILigand ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 1);
            ILigand[] sideChains = CIPTool.GetLigandLigands(ligand);
            foreach (var ligand2 in sideChains)
            {
                Assert.AreNotSame(ligand.GetVisitedAtoms(), ligand2.GetVisitedAtoms());
            }
        }

        /// <summary>
        /// Tests if it returns the right number of ligands, for double bonds.
        /// </summary>
        [TestMethod()]
        public void TestGetLigandLigands_DoubleTriple()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC(C)C(C#N)(C(=C)C)[H]");
            ILigand ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 1);
            ILigand[] sideChains = CIPTool.GetLigandLigands(ligand);
            Assert.AreEqual(2, sideChains.Length);
            ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 4);
            sideChains = CIPTool.GetLigandLigands(ligand);
            Assert.AreEqual(3, sideChains.Length);
            ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 6);
            sideChains = CIPTool.GetLigandLigands(ligand);
            Assert.AreEqual(3, sideChains.Length);
            ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 9);
            sideChains = CIPTool.GetLigandLigands(ligand);
            Assert.AreEqual(0, sideChains.Length);
        }

        [TestMethod()]
        public void TestDefineLigand_ImplicitHydrogen()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC(C)C(C#N)(C(=C)C)");
            ILigand ligand = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, CIPTool.Hydrogen);
            Assert.IsTrue(ligand is ImplicitHydrogenLigand);
        }

        [TestMethod()]
        //(timeout=5000)
        public void TestTermination()
        {
            int ringSize = 7;
            IAtomContainer ring = new AtomContainer();
            for (int i = 0; i < ringSize; i++)
            {
                ring.Atoms.Add(new Atom("C"));
            }
            for (int j = 0; j < ringSize - 1; j++)
            {
                ring.AddBond(ring.Atoms[j], ring.Atoms[j + 1], BondOrder.Single);
            }
            ring.AddBond(ring.Atoms[ringSize - 1], ring.Atoms[0], BondOrder.Single);

            ring.Atoms.Add(new Atom("Cl"));
            ring.Atoms.Add(new Atom("F"));
            ring.AddBond(ring.Atoms[0], ring.Atoms[ringSize], BondOrder.Single);
            ring.AddBond(ring.Atoms[0], ring.Atoms[ringSize + 1], BondOrder.Single);
            ring.Atoms.Add(new Atom("O"));
            ring.AddBond(ring.Atoms[1], ring.Atoms[ringSize + 2], BondOrder.Single);
            IAtom[] atoms = new IAtom[]{ring.Atoms[ringSize], ring.Atoms[ringSize + 1], ring.Atoms[ringSize - 1],
                ring.Atoms[1]};
            ITetrahedralChirality stereoCenter = new TetrahedralChirality(ring.Atoms[0], atoms, TetrahedralStereo.AntiClockwise);
            ring.StereoElements.Add(stereoCenter);
            SmilesGenerator generator = new SmilesGenerator();
            CIPTool.GetCIPChirality(ring, stereoCenter);
        }

        [TestMethod()]
        public void TestOla28()
        {
            string filename = "NCDK.Data.CML.mol28.cml";

            IChemFile file;
            IAtomContainer mol;
            using (CMLReader reader = new CMLReader(ResourceLoader.GetAsStream(filename)))
            {
                file = reader.Read(new ChemFile());
                mol = ChemFileManipulator.GetAllAtomContainers(file).First();
            }

            foreach (var atom in mol.Atoms)
            {
                var neighbors = mol.GetConnectedAtoms(atom).ToList();
                if (neighbors.Count == 4)
                {
                    var stereo = StereoTool.GetStereo(neighbors[0], neighbors[1], neighbors[2], neighbors[3]);
                    ITetrahedralChirality stereoCenter = new TetrahedralChirality(mol.Atoms[0], neighbors, stereo);
                    CIPTool.CIPChirality chirality = CIPTool.GetCIPChirality(mol, stereoCenter);
                }
            }
        }

        // @cdk.inchi InChI=1S/C27H43FO6/c1-23(2,28)9-8-22(32)26(5,33)21-7-11-27(34)16-12-18(29)17-13-19(30)20(31)14-24(17,3)15(16)6-10-25(21,27)4/h12,15,17,19-22,30-34H,6-11,13-14H2,1-5H3/t15-,17-,19+,20-,21-,22+,24+,25+,26+,27+/m0/s1
        [TestMethod()]
        public void TestSteroid()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("F");
            a1.FormalCharge = 0;
            a1.Point3D = new Vector3(7.0124, 2.5853, -0.9016);
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("O");
            a2.FormalCharge = 0;
            a2.Point3D = new Vector3(-0.5682, -0.2861, 2.1733);
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("O");
            a3.FormalCharge = 0;
            a3.Point3D = new Vector3(2.2826, -2.9598, -0.5754);
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("O");
            a4.FormalCharge = 0;
            a4.Point3D = new Vector3(-6.6808, -1.9515, 0.4596);
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("O");
            a5.FormalCharge = 0;
            a5.Point3D = new Vector3(4.2201, -1.7701, -1.7827);
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("O");
            a6.FormalCharge = 0;
            a6.Point3D = new Vector3(-7.0886, 0.761, 0.0885);
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("O");
            a7.FormalCharge = 0;
            a7.Point3D = new Vector3(-3.3025, 3.5973, -0.657);
            mol.Atoms.Add(a7);
            IAtom a8 = builder.NewAtom("C");
            a8.FormalCharge = 0;
            a8.Point3D = new Vector3(0.4862, -0.9146, 0.0574);
            mol.Atoms.Add(a8);
            IAtom a9 = builder.NewAtom("C");
            a9.FormalCharge = 0;
            a9.Point3D = new Vector3(-0.1943, 0.2177, 0.8706);
            mol.Atoms.Add(a9);
            IAtom a10 = builder.NewAtom("C");
            a10.FormalCharge = 0;
            a10.Point3D = new Vector3(1.7596, -1.1559, 0.9089);
            mol.Atoms.Add(a10);
            IAtom a11 = builder.NewAtom("C");
            a11.FormalCharge = 0;
            a11.Point3D = new Vector3(-2.4826, -0.4593, -0.073);
            mol.Atoms.Add(a11);
            IAtom a12 = builder.NewAtom("C");
            a12.FormalCharge = 0;
            a12.Point3D = new Vector3(-3.7166, 0.0102, -0.941);
            mol.Atoms.Add(a12);
            IAtom a13 = builder.NewAtom("C");
            a13.FormalCharge = 0;
            a13.Point3D = new Vector3(-0.4659, -2.1213, 0.0044);
            mol.Atoms.Add(a13);
            IAtom a14 = builder.NewAtom("C");
            a14.FormalCharge = 0;
            a14.Point3D = new Vector3(-1.485, 0.6715, 0.2231);
            mol.Atoms.Add(a14);
            IAtom a15 = builder.NewAtom("C");
            a15.FormalCharge = 0;
            a15.Point3D = new Vector3(0.9729, 1.1842, 1.122);
            mol.Atoms.Add(a15);
            IAtom a16 = builder.NewAtom("C");
            a16.FormalCharge = 0;
            a16.Point3D = new Vector3(2.1976, 0.2666, 1.3272);
            mol.Atoms.Add(a16);
            IAtom a17 = builder.NewAtom("C");
            a17.FormalCharge = 0;
            a17.Point3D = new Vector3(-1.8034, -1.7401, -0.6501);
            mol.Atoms.Add(a17);
            IAtom a18 = builder.NewAtom("C");
            a18.FormalCharge = 0;
            a18.Point3D = new Vector3(-4.2265, 1.3894, -0.4395);
            mol.Atoms.Add(a18);
            IAtom a19 = builder.NewAtom("C");
            a19.FormalCharge = 0;
            a19.Point3D = new Vector3(2.8802, -1.9484, 0.2485);
            mol.Atoms.Add(a19);
            IAtom a20 = builder.NewAtom("C");
            a20.FormalCharge = 0;
            a20.Point3D = new Vector3(0.862, -0.4809, -1.3862);
            mol.Atoms.Add(a20);
            IAtom a21 = builder.NewAtom("C");
            a21.FormalCharge = 0;
            a21.Point3D = new Vector3(-4.8907, -1.0078, -0.8633);
            mol.Atoms.Add(a21);
            IAtom a22 = builder.NewAtom("C");
            a22.FormalCharge = 0;
            a22.Point3D = new Vector3(-1.7576, 1.9697, 0.0241);
            mol.Atoms.Add(a22);
            IAtom a23 = builder.NewAtom("C");
            a23.FormalCharge = 0;
            a23.Point3D = new Vector3(-4.9064, 1.3293, 0.9405);
            mol.Atoms.Add(a23);
            IAtom a24 = builder.NewAtom("C");
            a24.FormalCharge = 0;
            a24.Point3D = new Vector3(-3.339, 0.1527, -2.4408);
            mol.Atoms.Add(a24);
            IAtom a25 = builder.NewAtom("C");
            a25.FormalCharge = 0;
            a25.Point3D = new Vector3(-3.1038, 2.4096, -0.4054);
            mol.Atoms.Add(a25);
            IAtom a26 = builder.NewAtom("C");
            a26.FormalCharge = 0;
            a26.Point3D = new Vector3(-5.5668, -1.0627, 0.5104);
            mol.Atoms.Add(a26);
            IAtom a27 = builder.NewAtom("C");
            a27.FormalCharge = 0;
            a27.Point3D = new Vector3(3.7564, -1.0338, -0.652);
            mol.Atoms.Add(a27);
            IAtom a28 = builder.NewAtom("C");
            a28.FormalCharge = 0;
            a28.Point3D = new Vector3(-6.0498, 0.3154, 0.9627);
            mol.Atoms.Add(a28);
            IAtom a29 = builder.NewAtom("C");
            a29.FormalCharge = 0;
            a29.Point3D = new Vector3(3.6914, -2.6828, 1.3258);
            mol.Atoms.Add(a29);
            IAtom a30 = builder.NewAtom("C");
            a30.FormalCharge = 0;
            a30.Point3D = new Vector3(4.9535, -0.3812, 0.0661);
            mol.Atoms.Add(a30);
            IAtom a31 = builder.NewAtom("C");
            a31.FormalCharge = 0;
            a31.Point3D = new Vector3(5.4727, 0.8461, -0.696);
            mol.Atoms.Add(a31);
            IAtom a32 = builder.NewAtom("C");
            a32.FormalCharge = 0;
            a32.Point3D = new Vector3(6.7079, 1.5265, -0.0844);
            mol.Atoms.Add(a32);
            IAtom a33 = builder.NewAtom("C");
            a33.FormalCharge = 0;
            a33.Point3D = new Vector3(6.4387, 2.104, 1.3013);
            mol.Atoms.Add(a33);
            IAtom a34 = builder.NewAtom("C");
            a34.FormalCharge = 0;
            a34.Point3D = new Vector3(7.9342, 0.6197, -0.0661);
            mol.Atoms.Add(a34);
            IAtom a35 = builder.NewAtom("H");
            a35.FormalCharge = 0;
            a35.Point3D = new Vector3(1.4474, -1.6941, 1.8161);
            mol.Atoms.Add(a35);
            IAtom a36 = builder.NewAtom("H");
            a36.FormalCharge = 0;
            a36.Point3D = new Vector3(-2.8575, -0.7521, 0.9166);
            mol.Atoms.Add(a36);
            IAtom a37 = builder.NewAtom("H");
            a37.FormalCharge = 0;
            a37.Point3D = new Vector3(-0.0529, -2.952, -0.5733);
            mol.Atoms.Add(a37);
            IAtom a38 = builder.NewAtom("H");
            a38.FormalCharge = 0;
            a38.Point3D = new Vector3(-0.6583, -2.5149, 1.01);
            mol.Atoms.Add(a38);
            IAtom a39 = builder.NewAtom("H");
            a39.FormalCharge = 0;
            a39.Point3D = new Vector3(1.1462, 1.8516, 0.2703);
            mol.Atoms.Add(a39);
            IAtom a40 = builder.NewAtom("H");
            a40.FormalCharge = 0;
            a40.Point3D = new Vector3(0.8186, 1.8095, 2.0087);
            mol.Atoms.Add(a40);
            IAtom a41 = builder.NewAtom("H");
            a41.FormalCharge = 0;
            a41.Point3D = new Vector3(2.5044, 0.2648, 2.3797);
            mol.Atoms.Add(a41);
            IAtom a42 = builder.NewAtom("H");
            a42.FormalCharge = 0;
            a42.Point3D = new Vector3(2.9822, 0.7582, 0.7671);
            mol.Atoms.Add(a42);
            IAtom a43 = builder.NewAtom("H");
            a43.FormalCharge = 0;
            a43.Point3D = new Vector3(-2.4854, -2.5906, -0.5319);
            mol.Atoms.Add(a43);
            IAtom a44 = builder.NewAtom("H");
            a44.FormalCharge = 0;
            a44.Point3D = new Vector3(-1.6353, -1.6475, -1.7261);
            mol.Atoms.Add(a44);
            IAtom a45 = builder.NewAtom("H");
            a45.FormalCharge = 0;
            a45.Point3D = new Vector3(-4.9616, 1.7691, -1.1638);
            mol.Atoms.Add(a45);
            IAtom a46 = builder.NewAtom("H");
            a46.FormalCharge = 0;
            a46.Point3D = new Vector3(-0.0354, -0.2446, -1.9684);
            mol.Atoms.Add(a46);
            IAtom a47 = builder.NewAtom("H");
            a47.FormalCharge = 0;
            a47.Point3D = new Vector3(1.3691, -1.2574, -1.9625);
            mol.Atoms.Add(a47);
            IAtom a48 = builder.NewAtom("H");
            a48.FormalCharge = 0;
            a48.Point3D = new Vector3(1.4296, 0.4511, -1.4252);
            mol.Atoms.Add(a48);
            IAtom a49 = builder.NewAtom("H");
            a49.FormalCharge = 0;
            a49.Point3D = new Vector3(-4.5596, -2.0138, -1.147);
            mol.Atoms.Add(a49);
            IAtom a50 = builder.NewAtom("H");
            a50.FormalCharge = 0;
            a50.Point3D = new Vector3(-5.6512, -0.7511, -1.6149);
            mol.Atoms.Add(a50);
            IAtom a51 = builder.NewAtom("H");
            a51.FormalCharge = 0;
            a51.Point3D = new Vector3(-1.0464, 2.7559, 0.2488);
            mol.Atoms.Add(a51);
            IAtom a52 = builder.NewAtom("H");
            a52.FormalCharge = 0;
            a52.Point3D = new Vector3(-4.1786, 1.0807, 1.7222);
            mol.Atoms.Add(a52);
            IAtom a53 = builder.NewAtom("H");
            a53.FormalCharge = 0;
            a53.Point3D = new Vector3(-5.2947, 2.3265, 1.1848);
            mol.Atoms.Add(a53);
            IAtom a54 = builder.NewAtom("H");
            a54.FormalCharge = 0;
            a54.Point3D = new Vector3(-2.421, 0.7311, -2.5838);
            mol.Atoms.Add(a54);
            IAtom a55 = builder.NewAtom("H");
            a55.FormalCharge = 0;
            a55.Point3D = new Vector3(-3.2008, -0.8224, -2.9194);
            mol.Atoms.Add(a55);
            IAtom a56 = builder.NewAtom("H");
            a56.FormalCharge = 0;
            a56.Point3D = new Vector3(-4.1353, 0.658, -3.0004);
            mol.Atoms.Add(a56);
            IAtom a57 = builder.NewAtom("H");
            a57.FormalCharge = 0;
            a57.Point3D = new Vector3(-4.8758, -1.4669, 1.2574);
            mol.Atoms.Add(a57);
            IAtom a58 = builder.NewAtom("H");
            a58.FormalCharge = 0;
            a58.Point3D = new Vector3(-0.9312, 0.4562, 2.6867);
            mol.Atoms.Add(a58);
            IAtom a59 = builder.NewAtom("H");
            a59.FormalCharge = 0;
            a59.Point3D = new Vector3(3.1882, -0.2287, -1.0977);
            mol.Atoms.Add(a59);
            IAtom a60 = builder.NewAtom("H");
            a60.FormalCharge = 0;
            a60.Point3D = new Vector3(-6.4869, 0.2469, 1.965);
            mol.Atoms.Add(a60);
            IAtom a61 = builder.NewAtom("H");
            a61.FormalCharge = 0;
            a61.Point3D = new Vector3(4.102, -2.0082, 2.0826);
            mol.Atoms.Add(a61);
            IAtom a62 = builder.NewAtom("H");
            a62.FormalCharge = 0;
            a62.Point3D = new Vector3(4.5162, -3.2434, 0.8708);
            mol.Atoms.Add(a62);
            IAtom a63 = builder.NewAtom("H");
            a63.FormalCharge = 0;
            a63.Point3D = new Vector3(3.0747, -3.4251, 1.8469);
            mol.Atoms.Add(a63);
            IAtom a64 = builder.NewAtom("H");
            a64.FormalCharge = 0;
            a64.Point3D = new Vector3(1.8961, -3.6368, 0.0058);
            mol.Atoms.Add(a64);
            IAtom a65 = builder.NewAtom("H");
            a65.FormalCharge = 0;
            a65.Point3D = new Vector3(5.7631, -1.1204, 0.1084);
            mol.Atoms.Add(a65);
            IAtom a66 = builder.NewAtom("H");
            a66.FormalCharge = 0;
            a66.Point3D = new Vector3(4.743, -0.1036, 1.1001);
            mol.Atoms.Add(a66);
            IAtom a67 = builder.NewAtom("H");
            a67.FormalCharge = 0;
            a67.Point3D = new Vector3(-6.3482, -2.8223, 0.1828);
            mol.Atoms.Add(a67);
            IAtom a68 = builder.NewAtom("H");
            a68.FormalCharge = 0;
            a68.Point3D = new Vector3(4.6594, -1.153, -2.3908);
            mol.Atoms.Add(a68);
            IAtom a69 = builder.NewAtom("H");
            a69.FormalCharge = 0;
            a69.Point3D = new Vector3(-7.3836, 1.6319, 0.4047);
            mol.Atoms.Add(a69);
            IAtom a70 = builder.NewAtom("H");
            a70.FormalCharge = 0;
            a70.Point3D = new Vector3(5.716, 0.5715, -1.7297);
            mol.Atoms.Add(a70);
            IAtom a71 = builder.NewAtom("H");
            a71.FormalCharge = 0;
            a71.Point3D = new Vector3(4.6721, 1.5926, -0.7787);
            mol.Atoms.Add(a71);
            IBond b1 = builder.NewBond(a1, a32, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a2, a9, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a2, a58, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a3, a19, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a3, a64, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a4, a26, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = builder.NewBond(a4, a67, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = builder.NewBond(a5, a27, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = builder.NewBond(a5, a68, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = builder.NewBond(a6, a28, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = builder.NewBond(a6, a69, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = builder.NewBond(a7, a25, BondOrder.Double);
            mol.Bonds.Add(b12);
            IBond b13 = builder.NewBond(a8, a9, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = builder.NewBond(a8, a10, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = builder.NewBond(a8, a13, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = builder.NewBond(a8, a20, BondOrder.Single);
            mol.Bonds.Add(b16);
            IBond b17 = builder.NewBond(a9, a14, BondOrder.Single);
            mol.Bonds.Add(b17);
            IBond b18 = builder.NewBond(a9, a15, BondOrder.Single);
            mol.Bonds.Add(b18);
            IBond b19 = builder.NewBond(a10, a16, BondOrder.Single);
            mol.Bonds.Add(b19);
            IBond b20 = builder.NewBond(a10, a19, BondOrder.Single);
            mol.Bonds.Add(b20);
            IBond b21 = builder.NewBond(a10, a35, BondOrder.Single);
            mol.Bonds.Add(b21);
            IBond b22 = builder.NewBond(a11, a12, BondOrder.Single);
            mol.Bonds.Add(b22);
            IBond b23 = builder.NewBond(a11, a14, BondOrder.Single);
            mol.Bonds.Add(b23);
            IBond b24 = builder.NewBond(a11, a17, BondOrder.Single);
            mol.Bonds.Add(b24);
            IBond b25 = builder.NewBond(a11, a36, BondOrder.Single);
            mol.Bonds.Add(b25);
            IBond b26 = builder.NewBond(a12, a18, BondOrder.Single);
            mol.Bonds.Add(b26);
            IBond b27 = builder.NewBond(a12, a21, BondOrder.Single);
            mol.Bonds.Add(b27);
            IBond b28 = builder.NewBond(a12, a24, BondOrder.Single);
            mol.Bonds.Add(b28);
            IBond b29 = builder.NewBond(a13, a17, BondOrder.Single);
            mol.Bonds.Add(b29);
            IBond b30 = builder.NewBond(a13, a37, BondOrder.Single);
            mol.Bonds.Add(b30);
            IBond b31 = builder.NewBond(a13, a38, BondOrder.Single);
            mol.Bonds.Add(b31);
            IBond b32 = builder.NewBond(a14, a22, BondOrder.Double);
            mol.Bonds.Add(b32);
            IBond b33 = builder.NewBond(a15, a16, BondOrder.Single);
            mol.Bonds.Add(b33);
            IBond b34 = builder.NewBond(a15, a39, BondOrder.Single);
            mol.Bonds.Add(b34);
            IBond b35 = builder.NewBond(a15, a40, BondOrder.Single);
            mol.Bonds.Add(b35);
            IBond b36 = builder.NewBond(a16, a41, BondOrder.Single);
            mol.Bonds.Add(b36);
            IBond b37 = builder.NewBond(a16, a42, BondOrder.Single);
            mol.Bonds.Add(b37);
            IBond b38 = builder.NewBond(a17, a43, BondOrder.Single);
            mol.Bonds.Add(b38);
            IBond b39 = builder.NewBond(a17, a44, BondOrder.Single);
            mol.Bonds.Add(b39);
            IBond b40 = builder.NewBond(a18, a23, BondOrder.Single);
            mol.Bonds.Add(b40);
            IBond b41 = builder.NewBond(a18, a25, BondOrder.Single);
            mol.Bonds.Add(b41);
            IBond b42 = builder.NewBond(a18, a45, BondOrder.Single);
            mol.Bonds.Add(b42);
            IBond b43 = builder.NewBond(a19, a27, BondOrder.Single);
            mol.Bonds.Add(b43);
            IBond b44 = builder.NewBond(a19, a29, BondOrder.Single);
            mol.Bonds.Add(b44);
            IBond b45 = builder.NewBond(a20, a46, BondOrder.Single);
            mol.Bonds.Add(b45);
            IBond b46 = builder.NewBond(a20, a47, BondOrder.Single);
            mol.Bonds.Add(b46);
            IBond b47 = builder.NewBond(a20, a48, BondOrder.Single);
            mol.Bonds.Add(b47);
            IBond b48 = builder.NewBond(a21, a26, BondOrder.Single);
            mol.Bonds.Add(b48);
            IBond b49 = builder.NewBond(a21, a49, BondOrder.Single);
            mol.Bonds.Add(b49);
            IBond b50 = builder.NewBond(a21, a50, BondOrder.Single);
            mol.Bonds.Add(b50);
            IBond b51 = builder.NewBond(a22, a25, BondOrder.Single);
            mol.Bonds.Add(b51);
            IBond b52 = builder.NewBond(a22, a51, BondOrder.Single);
            mol.Bonds.Add(b52);
            IBond b53 = builder.NewBond(a23, a28, BondOrder.Single);
            mol.Bonds.Add(b53);
            IBond b54 = builder.NewBond(a23, a52, BondOrder.Single);
            mol.Bonds.Add(b54);
            IBond b55 = builder.NewBond(a23, a53, BondOrder.Single);
            mol.Bonds.Add(b55);
            IBond b56 = builder.NewBond(a24, a54, BondOrder.Single);
            mol.Bonds.Add(b56);
            IBond b57 = builder.NewBond(a24, a55, BondOrder.Single);
            mol.Bonds.Add(b57);
            IBond b58 = builder.NewBond(a24, a56, BondOrder.Single);
            mol.Bonds.Add(b58);
            IBond b59 = builder.NewBond(a26, a28, BondOrder.Single);
            mol.Bonds.Add(b59);
            IBond b60 = builder.NewBond(a26, a57, BondOrder.Single);
            mol.Bonds.Add(b60);
            IBond b61 = builder.NewBond(a27, a30, BondOrder.Single);
            mol.Bonds.Add(b61);
            IBond b62 = builder.NewBond(a27, a59, BondOrder.Single);
            mol.Bonds.Add(b62);
            IBond b63 = builder.NewBond(a28, a60, BondOrder.Single);
            mol.Bonds.Add(b63);
            IBond b64 = builder.NewBond(a29, a61, BondOrder.Single);
            mol.Bonds.Add(b64);
            IBond b65 = builder.NewBond(a29, a62, BondOrder.Single);
            mol.Bonds.Add(b65);
            IBond b66 = builder.NewBond(a29, a63, BondOrder.Single);
            mol.Bonds.Add(b66);
            IBond b67 = builder.NewBond(a30, a31, BondOrder.Single);
            mol.Bonds.Add(b67);
            IBond b68 = builder.NewBond(a30, a65, BondOrder.Single);
            mol.Bonds.Add(b68);
            IBond b69 = builder.NewBond(a30, a66, BondOrder.Single);
            mol.Bonds.Add(b69);
            IBond b70 = builder.NewBond(a31, a32, BondOrder.Single);
            mol.Bonds.Add(b70);
            IBond b71 = builder.NewBond(a31, a70, BondOrder.Single);
            mol.Bonds.Add(b71);
            IBond b72 = builder.NewBond(a31, a71, BondOrder.Single);
            mol.Bonds.Add(b72);
            IBond b73 = builder.NewBond(a32, a33, BondOrder.Single);
            mol.Bonds.Add(b73);
            IBond b74 = builder.NewBond(a32, a34, BondOrder.Single);
            mol.Bonds.Add(b74);

            IAtom[] ligandAtoms = new IAtom[4];
            ligandAtoms[0] = a1; // F
            ligandAtoms[1] = a33; // Me
            ligandAtoms[2] = a34; // Me
            ligandAtoms[3] = a31; // rest of molecule
            var stereo = StereoTool.GetStereo(ligandAtoms[0], ligandAtoms[1], ligandAtoms[2], ligandAtoms[3]);
            ITetrahedralChirality tetraStereo = new TetrahedralChirality(a32, ligandAtoms, stereo);

            Assert.AreEqual(CIPTool.CIPChirality.None, CIPTool.GetCIPChirality(mol, tetraStereo));
        }
    }
}
