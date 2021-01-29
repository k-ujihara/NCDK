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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
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
            var atom1 = new Atom("C")
            {
                Point3D = new Vector3(1, 1, 1)
            };
            var atom2 = new Atom("H");
            var atom3 = new Atom("H");
            var atom4 = new Atom("H");
            var atom5 = new Atom("H");
            var bond1 = new Bond(atom1, atom2);
            var bond2 = new Bond(atom1, atom3);
            var bond3 = new Bond(atom1, atom4);
            var bond4 = new Bond(atom1, atom5);
            var ac = atom1.Builder.NewAtomContainer();
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
            var atom1 = new Atom("C")
            {
                Point3D = new Vector3(1, 1, 1),
                CovalentRadius = 0.2
            };
            var atom2 = new Atom("C")
            {
                Point3D = new Vector3(2, 2, 2),
                CovalentRadius = 0.2
            };
            var newpoint = new AtomTetrahedralLigandPlacer3D().RescaleBondLength(atom1, atom2, atom2.Point3D.Value);
            Assert.AreEqual(0.4, Vector3.Distance(newpoint, atom1.Point3D.Value), 0.001);
        }

        [TestMethod()]
        public void TestGet3DCoordinatesForLigands_IAtom_IAtomContainer_IAtomContainer_IAtom_int_Double_double()
        {
            IAtom atom1 = new Atom("C")
            {
                Point3D = new Vector3(1, 1, 1)
            };
            var atom2 = new Atom("H");
            var atom3 = new Atom("H");
            var atom4 = new Atom("H");
            var atom5 = new Atom("H");
            var bond1 = new Bond(atom1, atom2);
            var bond2 = new Bond(atom1, atom3);
            var bond3 = new Bond(atom1, atom4);
            var bond4 = new Bond(atom1, atom5);
            var ac = atom1.Builder.NewAtomContainer();
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
            var noCoords = AtomTetrahedralLigandPlacer3D.GetUnsetAtomsInAtomContainer(atom1, ac);
            var withCoords = AtomTetrahedralLigandPlacer3D.GetPlacedAtomsInAtomContainer(atom1, ac);
            var placer = new AtomTetrahedralLigandPlacer3D();
            var newPoints = placer.Get3DCoordinatesForLigands(atom1, noCoords, withCoords, null, 4, placer.DefaultBondLengthH, -1);
            for (int j = 0; j < noCoords.Atoms.Count; j++)
            {
                var ligand = noCoords.Atoms[j];
                ligand.Point3D = newPoints[j];
            }
            ModelBuilder3DTest.CheckAverageBondLength(ac);
        }
    }
}
