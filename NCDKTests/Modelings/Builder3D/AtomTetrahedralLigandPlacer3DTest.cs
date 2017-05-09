/* Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
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
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Numerics;

namespace NCDK.Modelings.Builder3D
{
    /// <summary>
    /// Tests for AtomPlacer3D
    /// </summary>
    // @cdk.module test-builder3d
    // @cdk.githash
    [TestClass()]
    public class AtomTetrahedralLigandPlacer3DTest : CDKTestCase
    {
        [TestMethod()]
        public void TestAdd3DCoordinatesForSinglyBondedLigands_IAtomContainer()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            IAtom atom2 = new Atom("H");
            IAtom atom3 = new Atom("H");
            IAtom atom4 = new Atom("H");
            IAtom atom5 = new Atom("H");
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom1, atom3);
            IBond bond3 = new Bond(atom1, atom4);
            IBond bond4 = new Bond(atom1, atom5);
            IAtomContainer ac = atom1.Builder.CreateAtomContainer();
            atom1.FormalNeighbourCount = 4;
            atom2.FormalNeighbourCount = 1;
            atom3.FormalNeighbourCount = 1;
            atom4.FormalNeighbourCount = 1;
            atom5.FormalNeighbourCount = 1;
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            ac.Atoms.Add(atom3);
            ac.Atoms.Add(atom4);
            ac.Atoms.Add(atom5);
            ac.Bonds.Add(bond1);
            ac.Bonds.Add(bond2);
            ac.Bonds.Add(bond3);
            ac.Bonds.Add(bond4);
            new AtomTetrahedralLigandPlacer3D().Add3DCoordinatesForSinglyBondedLigands(ac);
            ModelBuilder3DTest.CheckAverageBondLength(ac);
        }

        [TestMethod()]
        public void RescaleBondLength_IAtom_IAtom_Point3d()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            atom1.CovalentRadius = 0.2;
            IAtom atom2 = new Atom("C");
            atom2.Point3D = new Vector3(2, 2, 2);
            atom2.CovalentRadius = 0.2;
            Vector3 newpoint = new AtomTetrahedralLigandPlacer3D().RescaleBondLength(atom1, atom2, atom2.Point3D.Value);
            Assert.AreEqual(0.4, Vector3.Distance(newpoint, atom1.Point3D.Value), 0.001);
        }

        [TestMethod()]
        public void TestGet3DCoordinatesForLigands_IAtom_IAtomContainer_IAtomContainer_IAtom_int_Double_double()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            IAtom atom2 = new Atom("H");
            IAtom atom3 = new Atom("H");
            IAtom atom4 = new Atom("H");
            IAtom atom5 = new Atom("H");
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom1, atom3);
            IBond bond3 = new Bond(atom1, atom4);
            IBond bond4 = new Bond(atom1, atom5);
            IAtomContainer ac = atom1.Builder.CreateAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            ac.Atoms.Add(atom3);
            ac.Atoms.Add(atom4);
            ac.Atoms.Add(atom5);
            atom1.FormalNeighbourCount = 4;
            atom2.FormalNeighbourCount = 1;
            atom3.FormalNeighbourCount = 1;
            atom4.FormalNeighbourCount = 1;
            atom5.FormalNeighbourCount = 1;
            ac.Bonds.Add(bond1);
            ac.Bonds.Add(bond2);
            ac.Bonds.Add(bond3);
            ac.Bonds.Add(bond4);
            IAtomContainer noCoords = new AtomTetrahedralLigandPlacer3D().GetUnsetAtomsInAtomContainer(atom1, ac);
            IAtomContainer withCoords = new AtomTetrahedralLigandPlacer3D().GetPlacedAtomsInAtomContainer(atom1, ac);
            Vector3[] newPoints = new AtomTetrahedralLigandPlacer3D().Get3DCoordinatesForLigands(atom1, noCoords,
                    withCoords, null, 4, AtomTetrahedralLigandPlacer3D.DEFAULT_BOND_LENGTH_H, -1);
            for (int j = 0; j < noCoords.Atoms.Count; j++)
            {
                if (newPoints[j] == null) Assert.Fail("No coordinates generated for atom " + j);
                IAtom ligand = noCoords.Atoms[j];
                ligand.Point3D = newPoints[j];
            }
            ModelBuilder3DTest.CheckAverageBondLength(ac);
        }
    }
}
