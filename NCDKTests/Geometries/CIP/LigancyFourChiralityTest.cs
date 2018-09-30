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
using NCDK.Silent;
using NCDK.Stereo;
using System.Collections.Generic;

namespace NCDK.Geometries.CIP
{
    // @cdk.module test-cip
    [TestClass()]
    public class LigancyFourChiralityTest : CDKTestCase
    {
        private static IAtomContainer molecule = MoleculeMaker();
        private static ILigand[] ligands = LigandsMaker();

        private static IAtomContainer MoleculeMaker()
        {
            var molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("Cl"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("Br"));
            molecule.Atoms.Add(new Atom("I"));
            molecule.Atoms.Add(new Atom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            return molecule;
        }

        private static ILigand[] LigandsMaker()
        {           
            var ligand1 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[4]);
            var ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[3]);
            var ligand3 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[2]);
            var ligand4 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            ligands = new ILigand[] { ligand1, ligand2, ligand3, ligand4 };
            return ligands;
        }

        [TestMethod()]
        public void TestConstructor()
        {
            var chirality = new LigancyFourChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            Assert.IsNotNull(chirality);
            Assert.AreEqual(molecule.Atoms[1], chirality.ChiralAtom);
            for (int i = 0; i < ligands.Length; i++)
            {
                Assert.AreEqual(ligands[i], chirality.Ligands[i]);
            }
            Assert.AreEqual(TetrahedralStereo.Clockwise, chirality.Stereo);
        }

        [TestMethod()]
        public void TestConstructorILigancyFourChirality()
        {
            var ligandAtoms = new List<IAtom>();
            foreach (var ligand in ligands)
                ligandAtoms.Add(ligand.LigandAtom);
            var cdkChiral = new TetrahedralChirality(molecule.Atoms[1],
                 ligandAtoms, TetrahedralStereo.Clockwise);
            var chirality = new LigancyFourChirality(molecule, cdkChiral);
            Assert.IsNotNull(chirality);
            Assert.AreEqual(molecule.Atoms[1], chirality.ChiralAtom);
            for (int i = 0; i < ligands.Length; i++)
            {
                Assert.AreEqual(ligands[i].LigandAtom, chirality.Ligands[i].LigandAtom);
                Assert.AreEqual(ligands[i].CentralAtom, chirality.Ligands[i].CentralAtom);
                Assert.AreEqual(ligands[i].AtomContainer, chirality.Ligands[i].AtomContainer);
            }
            Assert.AreEqual(TetrahedralStereo.Clockwise, chirality.Stereo);
        }

        /// <summary>
        /// Checks if projecting onto itself does not change the stereochemistry.
        /// </summary>
        [TestMethod()]
        public void TestProject()
        {
            var chirality = new LigancyFourChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            chirality.Project(ligands);
            Assert.AreEqual(TetrahedralStereo.Clockwise, chirality.Stereo);
        }

        [TestMethod()]
        public void TestProjectOneChange()
        {
            var chirality = new LigancyFourChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            var newLigands = new ILigand[] { ligands[0], ligands[1], ligands[3], ligands[2] };
            chirality = chirality.Project(newLigands);
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, chirality.Stereo);
        }

        [TestMethod()]
        public void TestProjectTwoChanges()
        {
            var chirality = new LigancyFourChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            var newLigands = new ILigand[] { ligands[1], ligands[0], ligands[3], ligands[2] };
            chirality = chirality.Project(newLigands);
            Assert.AreEqual(TetrahedralStereo.Clockwise, chirality.Stereo);
        }
    }
}
