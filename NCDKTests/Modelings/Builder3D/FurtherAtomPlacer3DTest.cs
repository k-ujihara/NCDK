/* Copyright (C) 2012 Daniel Szisz
 *
 * Contact: orlando@caesar.elte.hu
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
using NCDK.Numerics;
using NCDK.Templates;

namespace NCDK.Modelings.Builder3D
{
    /// <summary>
    /// Tests not-yet-tested functionalities of <see cref="AtomPlacer3D"/>.
    /// </summary>
    // @author danielszisz
    // @cdk.module test-builder3d
    // @created 04/10/2012
    // @version 04/22/2012
    [TestClass()]
    public class FurtherAtomPlacer3DTest : AtomPlacer3DTest
    {
        [TestMethod()]
        public void TestAllHeavyAtomsPlaced_benzene()
        {
            AtomPlacer3D atmplacer = new AtomPlacer3D();
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            foreach (var atom in benzene.Atoms)
            {
                atom.IsPlaced = true;
            }
            Assert.IsTrue(atmplacer.AllHeavyAtomsPlaced(benzene));
        }

        [TestMethod()]
        public override void TestNumberOfUnplacedHeavyAtoms_IAtomContainer()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeAlkane(5);
            for (int i = 0; i < 3; i++)
            {
                (molecule.Atoms[i]).IsPlaced = true;
            }
            int placedAtoms = new AtomPlacer3D().NumberOfUnplacedHeavyAtoms(molecule);
            Assert.AreEqual(2, placedAtoms);
        }

        [TestMethod()]
        public override void TestGetPlacedHeavyAtoms_IAtomContainer_IAtom()
        {
            AtomPlacer3D atmplacer = new AtomPlacer3D();
            IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();
            for (int j = 0; j < 3; j++)
            {
                (molecule.Atoms[j]).IsPlaced = true;
            }
            IAtomContainer placedAndConnectedTo1 = atmplacer.GetPlacedHeavyAtoms(molecule, molecule.Atoms[1]);
            IAtomContainer placedAndConnectedTo2 = atmplacer.GetPlacedHeavyAtoms(molecule, molecule.Atoms[2]);
            IAtomContainer placedAndConnectedTo4 = atmplacer.GetPlacedHeavyAtoms(molecule, molecule.Atoms[4]);

            Assert.AreEqual(2, placedAndConnectedTo1.Atoms.Count);
            Assert.AreEqual(1, placedAndConnectedTo2.Atoms.Count);
            Assert.AreEqual(0, placedAndConnectedTo4.Atoms.Count);

        }

        [TestMethod()]
        public override void TestGetPlacedHeavyAtom_IAtomContainer_IAtom_IAtom()
        {
            AtomPlacer3D atmplacer = new AtomPlacer3D();
            IAtomContainer molecule = TestMoleculeFactory.MakeAlkane(7);
            for (int j = 0; j < 5; j++)
            {
                molecule.Atoms[j].IsPlaced = true;
            }
            IAtom atom2 = atmplacer.GetPlacedHeavyAtom(molecule, molecule.Atoms[1], molecule.Atoms[0]);
            IAtom atom3 = atmplacer.GetPlacedHeavyAtom(molecule, molecule.Atoms[2], molecule.Atoms[1]);
            IAtom nullAtom = atmplacer.GetPlacedHeavyAtom(molecule, molecule.Atoms[0], molecule.Atoms[1]);

            Assert.AreEqual(atom2, molecule.Atoms[2]);
            Assert.AreEqual(atom3, molecule.Atoms[3]);
            Assert.IsNull(nullAtom);
        }

        [TestMethod()]
        public override void TestGetPlacedHeavyAtom_IAtomContainer_IAtom()
        {
            AtomPlacer3D atmplacer = new AtomPlacer3D();
            IAtomContainer molecule = TestMoleculeFactory.MakeCyclohexane();
            //        For(IAtom a : m.Atoms) a.IsPlaced = true;
            for (int i = 0; i < 3; i++)
            {
                molecule.Atoms[i].IsPlaced = true;
            }

            IAtom atom1 = atmplacer.GetPlacedHeavyAtom(molecule, molecule.Atoms[0]);
            Assert.AreEqual(atom1, molecule.Atoms[1]);
            IAtom atom2 = atmplacer.GetPlacedHeavyAtom(molecule, molecule.Atoms[2]);
            Assert.AreEqual(atom2, molecule.Atoms[1]);
            IAtom atom3 = atmplacer.GetPlacedHeavyAtom(molecule, molecule.Atoms[4]);
            Assert.IsNull(atom3);
        }

        [TestMethod()]
        public override void TestGeometricCenterAllPlacedAtoms_IAtomContainer()
        {
            AtomPlacer3D atmplacer = new AtomPlacer3D();
            IAtomContainer molecule = TestMoleculeFactory.MakeAlkane(2);
            foreach (var atom in molecule.Atoms)
            {
                atom.IsPlaced = true;
            }
            molecule.Atoms[0].Point3D = new Vector3(-1.0, 0.0, 0.0);
            molecule.Atoms[1].Point3D = new Vector3(1.0, 0.0, 0.0);

            Vector3 center = atmplacer.GeometricCenterAllPlacedAtoms(molecule);
            Assert.AreEqual(0.0, center.X, 0.01);
            Assert.AreEqual(0.0, center.Y, 0.01);
            Assert.AreEqual(0.0, center.Z, 0.01);

        }

        [TestMethod()]
        public void TestGetUnplacedRingHeavyAtom_IAtomContainer_IAtom()
        {
            AtomPlacer3D atmplacer = new AtomPlacer3D();
            IAtomContainer molecule = TestMoleculeFactory.MakeCyclopentane();

            foreach (var atom in molecule.Atoms)
                atom.IsInRing = true;
            for (int j = 0; j < 2; j++)
            {
                molecule.Atoms[j].IsPlaced = true;
            }
            IAtom atom0 = molecule.Atoms[0];
            IAtom atom1 = molecule.Atoms[1];
            IAtom natom = molecule.Atoms[4];

            IAtom atom0pair = atmplacer.GetUnplacedRingHeavyAtom(molecule, atom0);
            IAtom atom1pair = atmplacer.GetUnplacedRingHeavyAtom(molecule, atom1);
            IAtom natompair = atmplacer.GetUnplacedRingHeavyAtom(molecule, natom);

            Assert.AreEqual(atom0pair, molecule.Atoms[4]);
            Assert.AreEqual(atom1pair, molecule.Atoms[2]);
            Assert.AreEqual(atom0.IsPlaced, true);

            foreach (var bond in molecule.Bonds)
            {
                if (bond.GetOther(molecule.Atoms[4]) != null
                        && !bond.GetOther(molecule.Atoms[4]).IsPlaced)
                {
                    natompair = bond.GetOther(molecule.Atoms[4]);
                }
            }
            Assert.AreEqual(natompair, molecule.Atoms[3]);
        }

        [TestMethod()]
        public void TestGetFarthestAtom_Point3d_IAtomContainer()
        {
            AtomPlacer3D atmplacer = new AtomPlacer3D();
            IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();

            molecule.Atoms[0].Point3D = new Vector3(0.0, 0.0, 0.0);
            molecule.Atoms[1].Point3D = new Vector3(1.0, 1.0, 1.0);
            molecule.Atoms[4].Point3D = new Vector3(3.0, 2.0, 1.0);
            molecule.Atoms[5].Point3D = new Vector3(4.0, 4.0, 4.0);

            IAtom farthestFromAtoma = atmplacer.GetFarthestAtom(molecule.Atoms[0].Point3D.Value, molecule);
            IAtom farthestFromAtomb = atmplacer.GetFarthestAtom(molecule.Atoms[4].Point3D.Value, molecule);

            Assert.AreEqual(molecule.Atoms[5], farthestFromAtoma);
            Assert.AreEqual(molecule.Atoms[0], farthestFromAtomb);

        }

        [TestMethod()]
        public void TestGetNextPlacedHeavyAtomWithUnplacedRingNeighbour_IAtomContainer()
        {
            AtomPlacer3D atmplacer = new AtomPlacer3D();
            IAtomContainer acyclicAlkane = TestMoleculeFactory.MakeAlkane(3);
            IAtomContainer cycloPentane = TestMoleculeFactory.MakeCyclopentane();

            //TestMoleculeFactory does not set ISINRING flags for cyclic molecules
            Assert.AreEqual(false, cycloPentane.Atoms[0].IsInRing);
            foreach (var atom in cycloPentane.Atoms)
            {
                atom.IsInRing = true;
            }

            //acyclic molecule so null is expected
            foreach (var atom in acyclicAlkane.Atoms)
            {
                atom.IsPlaced = true;
            }
            Assert.IsNull(atmplacer.GetNextPlacedHeavyAtomWithUnplacedRingNeighbour(acyclicAlkane));

            for (int j = 0; j < 3; j++)
            {
                cycloPentane.Atoms[j].IsPlaced = true;
            }
            Assert.AreEqual(cycloPentane.Atoms[2],
                    atmplacer.GetNextPlacedHeavyAtomWithUnplacedRingNeighbour(cycloPentane));

        }

        [TestMethod()]
        public void TestGetNextPlacedHeavyAtomWithUnplacedAliphaticNeighbour_IAtomContainer()
        {
            AtomPlacer3D atmplacer = new AtomPlacer3D();
            IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
            IAtomContainer acyclicAlkane = TestMoleculeFactory.MakeAlkane(5);

            foreach (var atom in benzene.Atoms)
                atom.IsInRing = true;
            foreach (var atom in acyclicAlkane.Atoms)
                atom.IsAliphatic = true;

            for (int j = 0; j < 3; j++)
                benzene.Atoms[j].IsPlaced = true;
            IAtom searchedatom1 = atmplacer.GetNextPlacedHeavyAtomWithUnplacedAliphaticNeighbour(benzene);
            Assert.IsNull(searchedatom1);

            foreach (var atom in benzene.Atoms)
            {
                if (!atom.IsPlaced)
                {
                    atom.IsPlaced = true;
                }
            }
            IAtom searchedatom2 = atmplacer.GetNextPlacedHeavyAtomWithUnplacedAliphaticNeighbour(benzene);
            Assert.IsNull(searchedatom2);

            for (int k = 0; k < 3; k++)
            {
                acyclicAlkane.Atoms[k].IsPlaced = true;
            }
            IAtom nextAtom = atmplacer.GetNextPlacedHeavyAtomWithUnplacedAliphaticNeighbour(acyclicAlkane);
            Assert.AreEqual(acyclicAlkane.Atoms[2], nextAtom);

        }

        [TestMethod()]
        public void TestGetNextUnplacedHeavyAtomWithAliphaticPlacedNeighbour_IAtomContainer()
        {
            AtomPlacer3D atmplacer = new AtomPlacer3D();
            IAtomContainer cyclobutane = TestMoleculeFactory.MakeCyclobutane();
            IAtomContainer acyclicAlkane = TestMoleculeFactory.MakeAlkane(6);

            foreach (var atom in cyclobutane.Atoms)
            {
                atom.IsInRing = true;
            }
            foreach (var atom in acyclicAlkane.Atoms)
            {
                atom.IsAliphatic = true;
            }
            for (int j = 0; j < 3; j++)
            {
                cyclobutane.Atoms[j].IsPlaced = true;
            }
            IAtom nextHeavyAtom = atmplacer.GetNextUnplacedHeavyAtomWithAliphaticPlacedNeighbour(cyclobutane);
            Assert.IsNull(nextHeavyAtom);

            foreach (var atom in cyclobutane.Atoms)
            {
                if (!atom.IsPlaced)
                {
                    atom.IsPlaced = true;
                }
            }
            IAtom nextHeavyAtom2 = atmplacer.GetNextUnplacedHeavyAtomWithAliphaticPlacedNeighbour(cyclobutane);
            Assert.IsNull(nextHeavyAtom2);

            for (int k = 0; k < 3; k++)
            {
                acyclicAlkane.Atoms[k].IsPlaced = true;
            }
            IAtom nextSuchUnplacedHeavyAtom = atmplacer.GetNextUnplacedHeavyAtomWithAliphaticPlacedNeighbour(acyclicAlkane);
            Assert.AreEqual(acyclicAlkane.Atoms[3], nextSuchUnplacedHeavyAtom);

            foreach (var atom in acyclicAlkane.Atoms)
            {
                atom.IsPlaced = true;
            }
            nextSuchUnplacedHeavyAtom = atmplacer.GetNextUnplacedHeavyAtomWithAliphaticPlacedNeighbour(acyclicAlkane);
            Assert.IsNull(nextSuchUnplacedHeavyAtom);
        }

        // @cdk.bug #3224093
        [TestMethod()]
        public void TestGetAngleValue_String_String_String()
        {
            var parser = CDK.SmilesParser;
            var smiles = "CCCCCC";
            var molecule = parser.ParseSmiles(smiles);
            Assert.IsNotNull(molecule);
            ForceFieldConfigurator ffc = new ForceFieldConfigurator();
            ffc.SetForceFieldConfigurator("mmff94");
            AtomPlacer3D atomPlacer3d = new AtomPlacer3D(ffc.GetParameterSet());
            ffc.AssignAtomTyps(molecule);

            string id1 = molecule.Atoms[1].AtomTypeName;
            string id2 = molecule.Atoms[2].AtomTypeName;
            string id3 = molecule.Atoms[3].AtomTypeName;

            double anglev = atomPlacer3d.GetAngleValue(id1, id2, id3);
            Assert.AreEqual(109.608, anglev, 0.001);

        }

        // @cdk.bug #3524092
        [TestMethod()]
        public void TestGetBondLengthValue_String_String()
        {
            var parser = CDK.SmilesParser;
            var smiles = "CCCCCC";
            var molecule = parser.ParseSmiles(smiles);
            Assert.IsNotNull(molecule);
            ForceFieldConfigurator ffc = new ForceFieldConfigurator();
            ffc.SetForceFieldConfigurator("mmff94");
            AtomPlacer3D atomPlacer3d = new AtomPlacer3D(ffc.GetParameterSet());
            ffc.AssignAtomTyps(molecule);

            string id1 = molecule.Atoms[1].AtomTypeName;
            string id2 = molecule.Atoms[2].AtomTypeName;
            string mmff94id1 = "C";
            string mmff94id2 = "C";
            Assert.AreNotSame(mmff94id1, id1);
            Assert.AreNotSame(mmff94id2, id2);

            double bondlength = atomPlacer3d.GetBondLengthValue(id1, id2);
            Assert.AreEqual(1.508, bondlength, 0.001);
        }

        // @cdk.bug #3523247
        [TestMethod()]
        public void TestGetBondLengthValue_bug_CNBond()
        {
            var parser = CDK.SmilesParser;
            var smiles = "CCCN";
            var molecule = parser.ParseSmiles(smiles);
            Assert.IsNotNull(molecule);
            ForceFieldConfigurator ffc = new ForceFieldConfigurator();
            ffc.SetForceFieldConfigurator("mmff94");
            AtomPlacer3D atomPlacer3d = new AtomPlacer3D(ffc.GetParameterSet());
            ffc.AssignAtomTyps(molecule);

            string id1 = molecule.Atoms[2].AtomTypeName;
            string id2 = molecule.Atoms[3].AtomTypeName;
            double bondlength = atomPlacer3d.GetBondLengthValue(id1, id2);
            Assert.AreEqual(1.451, bondlength, 0.001);

        }

        /// <summary>
        /// This class only places 'chains' - i.e. no branching. Check an exception is thrown.
        /// </summary>
        // @cdk.inchi InChI=1/C14H30/c1-4-7-10-13-14(11-8-5-2)12-9-6-3/h14H,4-13H2,1-3H3
        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void InvalidChain()
        {
            string input = "CCCCCC(CCCC)CCCC";
            var sp = CDK.SmilesParser;
            var m = sp.ParseSmiles(input);

            ForceFieldConfigurator ffc = new ForceFieldConfigurator();
            ffc.SetForceFieldConfigurator("mmff92");
            ffc.AssignAtomTyps(m);

            AtomPlacer3D ap3d = new AtomPlacer3D(ffc.GetParameterSet());
            ap3d.PlaceAliphaticHeavyChain(m, m);
        }
    }
}
