/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Config;
using NCDK.Graphs;
using NCDK.IO;
using NCDK.Isomorphisms;
using NCDK.Numerics;
using NCDK.Stereo;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NCDK.Smiles
{
    // @author         steinbeck
    // @cdk.created    2004-02-09
    // @cdk.module     test-smiles
    [TestClass()]
    public class SmilesGeneratorTest 
        : CDKTestCase
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestSmilesGenerator()
        {
            var mol2 = TestMoleculeFactory.MakeAlphaPinene();
            var sg = new SmilesGenerator();
            AddImplicitHydrogens(mol2);
            string smiles2 = sg.Create(mol2);
            Assert.IsNotNull(smiles2);
            Assert.AreEqual("C1(=CCC2CC1C2(C)C)C", smiles2);
        }

        [TestMethod()]
        public void TestEthylPropylPhenantren()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeEthylPropylPhenantren();
            var sg = new SmilesGenerator();
            FixCarbonHCount(mol1);
            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("C=1C=CC(=C2C=CC3=C(C12)C=CC(=C3)CCC)CC", smiles1);
        }

        [TestMethod()]
        public void TestPropylCycloPropane()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakePropylCycloPropane();
            var sg = new SmilesGenerator();
            FixCarbonHCount(mol1);
            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("C1CC1CCC", smiles1);
        }

        [TestMethod()]
        public void TestAlanin()
        {
            IAtomContainer mol1 = builder.NewAtomContainer();
            var sg = SmilesGenerator.Isomeric;
            mol1.Atoms.Add(builder.NewAtom("N", new Vector2(1, 0)));
            // 1
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(1, 2)));
            // 2
            mol1.Atoms.Add(builder.NewAtom("F", new Vector2(1, 2)));
            // 3
            mol1.Atoms.Add(builder.NewAtom("C", Vector2.Zero));
            // 4
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(1, 4)));
            // 5
            mol1.Atoms.Add(builder.NewAtom("O", new Vector2(1, 5)));
            // 6
            mol1.Atoms.Add(builder.NewAtom("O", new Vector2(1, 6)));
            // 7
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[1], BondOrder.Single);
            // 1
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[2], BondOrder.Single, BondStereo.Up);
            // 2
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[3], BondOrder.Single, BondStereo.Down);
            // 3
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[4], BondOrder.Single);
            // 4
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[5], BondOrder.Single);
            // 5
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[6], BondOrder.Double);
            // 6
            // hydrogens in-lined from hydrogen adder/placer
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0.13, -0.50)));
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[7], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(1.87, -0.50)));
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[8], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-0.89, 0.45)));
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[9], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-0.45, -0.89)));
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[10], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0.89, -0.45)));
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[11], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(1.00, 6.00)));
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[12], BondOrder.Single);
            AddImplicitHydrogens(mol1);

            var ifac = BODRIsotopeFactory.Instance;
            ifac.ConfigureAtoms(mol1);

            Define(mol1, Anticlockwise(mol1, 1, 0, 2, 3, 4));

            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);

            Assert.AreEqual("N([C@](F)(C([H])([H])[H])C(O[H])=O)([H])[H]", smiles1);

            Define(mol1, Clockwise(mol1, 1, 0, 2, 3, 4));

            smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("N([C@@](F)(C([H])([H])[H])C(O[H])=O)([H])[H]", smiles1);
        }

        [TestMethod()]
        public void TestCisResorcinol()
        {
            IAtomContainer mol1 = builder.NewAtomContainer();
            var sg = SmilesGenerator.Isomeric;
            mol1.Atoms.Add(builder.NewAtom("O", new Vector2(3, 1)));
            // 1
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(2, 0)));
            // 2
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(2, 1)));
            // 3
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(1, 1)));
            // 4
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(1, 4)));
            // 5
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(1, 5)));
            // 6
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(1, 2)));
            // 7
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(2, 2)));
            // 1
            mol1.Atoms.Add(builder.NewAtom("O", new Vector2(3, 2)));
            // 2
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(2, 3)));
            // 3
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[2], BondOrder.Single, BondStereo.Down);
            // 1
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[2], BondOrder.Single, BondStereo.Up);
            // 2
            mol1.AddBond(mol1.Atoms[2], mol1.Atoms[3], BondOrder.Single);
            // 3
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[4], BondOrder.Single);
            // 4
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[5], BondOrder.Single);
            // 5
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[6], BondOrder.Single);
            // 6
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[7], BondOrder.Single);
            // 3
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[8], BondOrder.Single, BondStereo.Up);
            // 4
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[9], BondOrder.Single, BondStereo.Down);
            // 5
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[2], BondOrder.Single);
            // 6

            // hydrogens in-lined from hydrogen adder/placer
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(4.00, 1.00)));
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[10], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0.00, 1.00)));
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[11], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(1.00, 0.00)));
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[12], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0.13, 4.50)));
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[13], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0.13, 3.50)));
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[14], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(1.87, 5.50)));
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[15], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0.13, 5.50)));
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[16], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0.00, 2.00)));
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[17], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(1.00, 1.00)));
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[18], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(4.00, 2.00)));
            mol1.AddBond(mol1.Atoms[8], mol1.Atoms[19], BondOrder.Single);

            AddImplicitHydrogens(mol1);
            var ifac = BODRIsotopeFactory.Instance;

            ifac.ConfigureAtoms(mol1);
            Define(mol1, Clockwise(mol1, 2, 0, 1, 3, 7), Clockwise(mol1, 7, 2, 6, 8, 9));
            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("O([C@@]1([H])C(C(C(C([C@]1(O[H])[H])([H])[H])([H])[H])([H])[H])([H])[H])[H]", smiles1);
            mol1 = AtomContainerManipulator.RemoveHydrogens(mol1);
            smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("O[C@H]1CCCC[C@H]1O", smiles1);
        }

        [TestMethod()]
        public void TestCisTransDecalin()
        {
            IAtomContainer mol1 = builder.NewAtomContainer();
            var sg = SmilesGenerator.Isomeric;

            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0, 3))); // 0
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(0, 1))); // 1
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(0, -1))); // 2
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0, -3))); // 3

            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(1.5, 2))); // 4
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(3, 1))); // 5
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(3, -1))); // 6
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(1.5, -2))); // 7

            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(-1.5, 2))); // 8
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(-3, 1))); // 9
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(-3, -1))); // 10
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(-1.5, -2))); // 11

            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[0], BondOrder.Single, BondStereo.Down);
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[2], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[2], mol1.Atoms[3], BondOrder.Single, BondStereo.Down);

            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[4], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[5], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[6], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[7], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[2], BondOrder.Single);

            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[8], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[8], mol1.Atoms[9], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[9], mol1.Atoms[10], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[10], mol1.Atoms[11], BondOrder.Single);
            mol1.AddBond(mol1.Atoms[11], mol1.Atoms[2], BondOrder.Single);

            // hydrogens in-lined from hydrogen adder/placer
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(2.16, 2.75)));
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[12], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0.84, 2.75)));
            mol1.AddBond(mol1.Atoms[4], mol1.Atoms[13], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(3.98, 0.81)));
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[14], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(3.38, 1.92)));
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[15], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(3.38, -1.92)));
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[16], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(3.98, -0.81)));
            mol1.AddBond(mol1.Atoms[6], mol1.Atoms[17], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(0.84, -2.75)));
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[18], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(2.16, -2.75)));
            mol1.AddBond(mol1.Atoms[7], mol1.Atoms[19], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-0.84, 2.75)));
            mol1.AddBond(mol1.Atoms[8], mol1.Atoms[20], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-2.16, 2.75)));
            mol1.AddBond(mol1.Atoms[8], mol1.Atoms[21], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-3.38, 1.92)));
            mol1.AddBond(mol1.Atoms[9], mol1.Atoms[22], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-3.98, 0.81)));
            mol1.AddBond(mol1.Atoms[9], mol1.Atoms[23], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-3.98, -0.81)));
            mol1.AddBond(mol1.Atoms[10], mol1.Atoms[24], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-3.38, -1.92)));
            mol1.AddBond(mol1.Atoms[10], mol1.Atoms[25], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-2.16, -2.75)));
            mol1.AddBond(mol1.Atoms[11], mol1.Atoms[26], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-0.84, -2.75)));
            mol1.AddBond(mol1.Atoms[11], mol1.Atoms[27], BondOrder.Single);
            AddImplicitHydrogens(mol1);
            var ifac = BODRIsotopeFactory.Instance;
            ifac.ConfigureAtoms(mol1);
            Define(mol1, Clockwise(mol1, 1, 0, 2, 4, 8), Clockwise(mol1, 2, 1, 3, 7, 1));
            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual(
                    "[H][C@@]12[C@@]([H])(C(C(C(C1([H])[H])([H])[H])([H])[H])([H])[H])C(C(C(C2([H])[H])([H])[H])([H])[H])([H])[H]",
                    smiles1);
            Define(mol1, Clockwise(mol1, 1, 0, 2, 4, 8), Anticlockwise(mol1, 2, 1, 3, 7, 1));
            string smiles3 = sg.Create(mol1);
            Assert.AreNotEqual(smiles3, smiles1);
        }

        [TestMethod()]
        public void TestDoubleBondConfiguration()
        {
            IAtomContainer mol1 = builder.NewAtomContainer();
            var sg = SmilesGenerator.Isomeric;
            mol1.Atoms.Add(builder.NewAtom("S", Vector2.Zero));
            // 1
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(1, 1)));
            // 2
            mol1.Atoms.Add(builder.NewAtom("F", new Vector2(2, 0)));
            // 3
            mol1.Atoms.Add(builder.NewAtom("C", new Vector2(1, 2)));
            // 4
            mol1.Atoms.Add(builder.NewAtom("F", new Vector2(2, 3)));
            // 5
            mol1.Atoms.Add(builder.NewAtom("S", new Vector2(0, 3)));
            // 1

            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[1], BondOrder.Single);
            // 1
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[2], BondOrder.Single);
            // 2
            mol1.AddBond(mol1.Atoms[1], mol1.Atoms[3], BondOrder.Double);
            // 3
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[4], BondOrder.Single);
            // 4
            mol1.AddBond(mol1.Atoms[3], mol1.Atoms[5], BondOrder.Single);
            // 4
            AddImplicitHydrogens(mol1);
            var ifac = BODRIsotopeFactory.Instance;
            ifac.ConfigureAtoms(mol1);

            mol1.StereoElements.Clear(); // clear existing
            mol1.StereoElements.Add(new DoubleBondStereochemistry(mol1.Bonds[2], new IBond[]{mol1.Bonds[1],
                mol1.Bonds[3]}, DoubleBondConformation.Opposite));
            string smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("S\\C(\\F)=C(/F)\\S", smiles1);

            mol1.StereoElements.Clear(); // clear existing
            mol1.StereoElements.Add(new DoubleBondStereochemistry(mol1.Bonds[2], new IBond[]{mol1.Bonds[1],
                mol1.Bonds[3]}, DoubleBondConformation.Together));

            smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("S\\C(\\F)=C(\\F)/S", smiles1);

            // hydrogens in-lined from hydrogen adder/placer
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(-0.71, -0.71)));
            mol1.Atoms[0].ImplicitHydrogenCount = 0;
            mol1.Atoms[mol1.Atoms.Count - 1].ImplicitHydrogenCount = 0;
            mol1.AddBond(mol1.Atoms[0], mol1.Atoms[6], BondOrder.Single);
            mol1.Atoms.Add(builder.NewAtom("H", new Vector2(2.71, 3.71)));
            mol1.Atoms[5].ImplicitHydrogenCount = 0;
            mol1.Atoms[mol1.Atoms.Count - 1].ImplicitHydrogenCount = 0;
            mol1.AddBond(mol1.Atoms[5], mol1.Atoms[7], BondOrder.Single);

            mol1.StereoElements.Clear(); // clear existing
            mol1.StereoElements.Add(new DoubleBondStereochemistry(mol1.Bonds[2], new IBond[]{mol1.Bonds[0],
                mol1.Bonds[3]}, DoubleBondConformation.Opposite));

            smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("S(/C(/F)=C(/F)\\S[H])[H]", smiles1);

            mol1.StereoElements.Clear(); // clear existing
            mol1.StereoElements.Add(new DoubleBondStereochemistry(mol1.Bonds[2], new IBond[]{mol1.Bonds[0],
                mol1.Bonds[3]}, DoubleBondConformation.Together));

            smiles1 = sg.Create(mol1);
            Assert.IsNotNull(smiles1);
            Assert.AreEqual("S(/C(/F)=C(\\F)/S[H])[H]", smiles1);
        }

        [TestMethod()]
        public void TestPartitioning()
        {
            var smiles = "";
            var molecule = builder.NewAtomContainer();
            var sg = new SmilesGenerator();
            var sodium = builder.NewAtom("Na");
            sodium.FormalCharge = +1;
            var hydroxyl = builder.NewAtom("O");
            hydroxyl.ImplicitHydrogenCount = 1;
            hydroxyl.FormalCharge = -1;
            molecule.Atoms.Add(sodium);
            molecule.Atoms.Add(hydroxyl);
            AddImplicitHydrogens(molecule);
            smiles = sg.Create(molecule);
            Assert.IsTrue(smiles.IndexOf(".", StringComparison.Ordinal) != -1);
        }

        // @cdk.bug 791091
        [TestMethod()]
        public void TestBug791091()
        {
            var smiles = "";
            var molecule = builder.NewAtomContainer();
            var sg = new SmilesGenerator();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("N"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[0], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[3], BondOrder.Single);
            FixCarbonHCount(molecule);
            smiles = sg.Create(molecule);
            Assert.AreEqual("C1CCN1C", smiles);
        }

        // @cdk.bug 590236
        [TestMethod()]
        public void TestBug590236()
        {
            var smiles = "";
            var molecule = builder.NewAtomContainer();
            var sg = SmilesGenerator.Isomeric;
            molecule.Atoms.Add(builder.NewAtom("C"));
            var carbon2 = builder.NewAtom("C");
            carbon2.MassNumber = 13;
            molecule.Atoms.Add(carbon2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            FixCarbonHCount(molecule);
            smiles = sg.Create(molecule);
            Assert.AreEqual("C[13CH3]", smiles);
        }

        /// <summary>
        /// A bug reported for JChemPaint.
        /// </summary>
        // @cdk.bug 956923
        [TestMethod()]
        public void TestSFBug956923_aromatic()
        {
            var smiles = "";
            var molecule = builder.NewAtomContainer();
            var sg = new SmilesGenerator().Aromatic();
            var sp2CarbonWithOneHydrogen = builder.NewAtom("C");
            sp2CarbonWithOneHydrogen.Hybridization = Hybridization.SP2;
            sp2CarbonWithOneHydrogen.ImplicitHydrogenCount = 1;
            molecule.Atoms.Add(sp2CarbonWithOneHydrogen);
            molecule.Atoms.Add((IAtom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((IAtom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((IAtom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((IAtom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((IAtom)sp2CarbonWithOneHydrogen.Clone());
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[0], BondOrder.Single);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            smiles = sg.Create(molecule);
            Assert.AreEqual("c1ccccc1", smiles);
        }

        [TestMethod()]
        public void TestSFBug956923_nonAromatic()
        {
            var smiles = "";
            var molecule = builder.NewAtomContainer();
            var sg = new SmilesGenerator();
            var sp2CarbonWithOneHydrogen = builder.NewAtom("C");
            sp2CarbonWithOneHydrogen.Hybridization = Hybridization.SP2;
            sp2CarbonWithOneHydrogen.ImplicitHydrogenCount = 1;
            molecule.Atoms.Add(sp2CarbonWithOneHydrogen);
            molecule.Atoms.Add((IAtom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((IAtom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((IAtom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((IAtom)sp2CarbonWithOneHydrogen.Clone());
            molecule.Atoms.Add((IAtom)sp2CarbonWithOneHydrogen.Clone());
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[0], BondOrder.Single);
            smiles = sg.Create(molecule);
            Assert.AreEqual("[CH]1[CH][CH][CH][CH][CH]1", smiles);
        }

        [TestMethod()]
        public void TestAtomPermutation()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("S"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            mol.Atoms[3].ImplicitHydrogenCount = 1;
            mol.Atoms[4].ImplicitHydrogenCount = 1;
            AddImplicitHydrogens(mol);
            var acap = new AtomContainerAtomPermutor(mol);
            var sg = SmilesGenerator.Unique;
            var smiles = "";
            string oldSmiles = sg.Create(mol);
            while (acap.MoveNext())
            {
                smiles = sg.Create(builder.NewAtomContainer(acap.Current));
                //Debug.WriteLine(smiles);
                Assert.AreEqual(oldSmiles, smiles);
            }
        }

        [TestMethod()]
        public void TestBondPermutation()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("S"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            mol.Atoms[3].ImplicitHydrogenCount = 1;
            mol.Atoms[4].ImplicitHydrogenCount = 1;
            AddImplicitHydrogens(mol);
            AtomContainerBondPermutor acbp = new AtomContainerBondPermutor(mol);
            var sg = SmilesGenerator.Unique;
            var smiles = "";
            string oldSmiles = sg.Create(mol);
            while (acbp.MoveNext())
            {
                smiles = sg.Create(builder.NewAtomContainer(acbp.Current));
                //Debug.WriteLine(smiles);
                Assert.AreEqual(oldSmiles, smiles);
            }
        }

        private static void FixCarbonHCount(IAtomContainer mol)
        {
            // the following line are just a quick fix for this particluar
            // carbon-only molecule until we have a proper hydrogen count
            // configurator
            double bondCount = 0;
            IAtom atom;
            for (int f = 0; f < mol.Atoms.Count; f++)
            {
                atom = mol.Atoms[f];
                bondCount = mol.GetBondOrderSum(atom);
                int correction = (int)(bondCount - (atom.Charge ?? 0));
                switch (atom.AtomicNumber)
                {
                    case AtomicNumbers.C:
                        atom.ImplicitHydrogenCount = 4 - correction;
                        break;
                    case AtomicNumbers.N:
                        atom.ImplicitHydrogenCount = 3 - correction;
                        break;
                }
            }
        }

        [TestMethod()]
        public void TestPseudoAtom()
        {
            var atom = builder.NewPseudoAtom("Star");
            var sg = new SmilesGenerator(SmiFlavors.Generic);
            var smiles = "";
            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(atom);
            AddImplicitHydrogens(molecule);
            smiles = sg.Create(molecule);
            Assert.AreEqual("*", smiles);
        }

        /// <summary>
        ///  Test generation of a reaction SMILES. I know, it's a stupid alchemic
        ///  reaction, but it serves its purpose.
        /// </summary>
        [TestMethod()]
        public void TestReactionSMILES()
        {
            var reaction = builder.NewReaction();
            var methane = builder.NewAtomContainer();
            methane.Atoms.Add(builder.NewAtom("C"));
            reaction.Reactants.Add(methane);
            IAtomContainer magic = builder.NewAtomContainer();
            magic.Atoms.Add(builder.NewPseudoAtom("magic"));
            reaction.Agents.Add(magic);
            IAtomContainer gold = builder.NewAtomContainer();
            gold.Atoms.Add(builder.NewAtom("Au"));
            reaction.Products.Add(gold);

            methane.Atoms[0].ImplicitHydrogenCount = 4;
            gold.Atoms[0].ImplicitHydrogenCount = 0;

            var sg = new SmilesGenerator(SmiFlavors.Generic);
            var smiles = sg.Create(reaction);
            //Debug.WriteLine($"Generated SMILES: {smiles}");
            Assert.AreEqual("C>*>[Au]", smiles);
        }

        /// <summary>
        ///  Test generation of a D and L alanin.
        /// </summary>
        [TestMethod()]
        public void TestAlaSMILES()
        {
            var filename = "NCDK.Data.MDL.l-ala.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer mol1 = reader.Read(builder.NewAtomContainer());
            filename = "NCDK.Data.MDL.d-ala.mol";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            var mol2 = reader.Read(builder.NewAtomContainer());
            var sg = SmilesGenerator.Isomeric;

            Define(mol1, Anticlockwise(mol1, 1, 0, 2, 3, 6));
            Define(mol2, Clockwise(mol2, 1, 0, 2, 3, 6));

            string smiles1 = sg.Create(mol1);
            string smiles2 = sg.Create(mol2);
            Assert.AreNotEqual(smiles2, smiles1);
        }

        /// <summary>
        ///  Test some sugars
        /// </summary>
        [TestMethod()]
        public void TestSugarSMILES()
        {
            var filename = "NCDK.Data.MDL.D-mannose.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer mol1 = reader.Read(builder.NewAtomContainer());
            filename = "NCDK.Data.MDL.D+-glucose.mol";
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            var mol2 = reader.Read(builder.NewAtomContainer());
            var sg = SmilesGenerator.Isomeric;

            Define(mol1, Anticlockwise(mol1, 0, 0, 1, 5, 9), Anticlockwise(mol1, 1, 1, 0, 2, 8),
                    Clockwise(mol1, 2, 2, 1, 3, 6), Anticlockwise(mol1, 5, 5, 0, 4, 10));
            Define(mol2, Anticlockwise(mol2, 0, 0, 1, 5, 9), Anticlockwise(mol2, 1, 1, 0, 2, 8),
                    Clockwise(mol2, 2, 2, 1, 3, 6), Clockwise(mol2, 5, 5, 0, 4, 10), Anticlockwise(mol2, 4, 4, 3, 5, 11));

            string smiles1 = sg.Create(mol1);
            string smiles2 = sg.Create(mol2);
            Assert.AreNotEqual(smiles2, smiles1);
        }

        /// <summary>
        ///  Test for some rings where the double bond is broken
        /// </summary>
        [TestMethod()]
        public void TestCycloOctan()
        {
            var filename = "NCDK.Data.MDL.cyclooctan.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer mol1 = reader.Read(builder.NewAtomContainer());
            var sg = new SmilesGenerator();
            string moleculeSmile = sg.Create(mol1);
            Assert.AreEqual("C\\1=C\\CCCCCC1", moleculeSmile);
        }

        [TestMethod()]
        public void TestCycloOcten()
        {
            var filename = "NCDK.Data.MDL.cycloocten.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer mol1 = reader.Read(builder.NewAtomContainer());
            var sg = new SmilesGenerator();
            string moleculeSmile = sg.Create(mol1);
            Assert.AreEqual("C1/C=C\\CCCCC1", moleculeSmile);
        }

        /// <summary>
        ///  A unit test
        /// </summary>
        [TestMethod()]
        public void TestCycloOctadien()
        {
            var filename = "NCDK.Data.MDL.cyclooctadien.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer mol1 = reader.Read(builder.NewAtomContainer());
            var sg = new SmilesGenerator();
            string moleculeSmile = sg.Create(mol1);
            Assert.AreEqual("C=1\\CC/C=C\\CC/C1", moleculeSmile);
        }

        // @cdk.bug 1089770
        [TestMethod()]
        public void TestSFBug1089770_1()
        {
            var filename = "NCDK.Data.MDL.bug1089770-1.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer mol1 = reader.Read(builder.NewAtomContainer());
            var sg = new SmilesGenerator();
            string moleculeSmile = sg.Create(mol1);
            //Debug.WriteLine(filename + " -> " + moleculeSmile);
            Assert.AreEqual("C1CCC2=C(C1)CCC2", moleculeSmile);
        }

        /// <see href="https://sourceforge.net/p/cdk/bugs/242/">CDK Bug 1089770</see>
        // @cdk.bug 1089770
        [TestMethod()]
        public void TestSFBug1089770_2()
        {
            var filename = "NCDK.Data.MDL.bug1089770-2.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer mol1 = reader.Read(builder.NewAtomContainer());
            var sg = new SmilesGenerator();
            string moleculeSmile = sg.Create(mol1);
            //Debug.WriteLine(filename + " -> " + moleculeSmile);
            Assert.AreEqual("C=1\\CC/C=C\\CC/C1", moleculeSmile);
        }

        // @cdk.bug 1014344
        // MDL -> CML (slow) -> SMILES round tripping
        [TestMethod()]
        public void TestSFBug1014344()
        {
            var filename = "NCDK.Data.MDL.bug1014344-1.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLReader reader = new MDLReader(ins, ChemObjectReaderMode.Strict);
            IAtomContainer mol1 = reader.Read(builder.NewAtomContainer());
            AddImplicitHydrogens(mol1);
            var sg = new SmilesGenerator();
            string molSmiles = sg.Create(mol1);
            StringWriter output = new StringWriter();
            CMLWriter cmlWriter = new CMLWriter(output);
            cmlWriter.Write(mol1);

            var aa = output.ToString();
            var bb = Encoding.UTF8.GetBytes(aa);

            CMLReader cmlreader = new CMLReader(new MemoryStream(bb));
            var mol2 = ((IChemFile)cmlreader.Read(builder.NewChemFile()))[0][0].MoleculeSet[0];
            AddImplicitHydrogens(mol2);
            string cmlSmiles = sg.Create(builder.NewAtomContainer(mol2));
            Assert.AreEqual(molSmiles, cmlSmiles);
        }

        // @cdk.bug 1014344
        [TestMethod()]
        public void TestTest()
        {
            string filename_cml = "NCDK.Data.MDL.9554-with-exp-hyd.mol";
            string filename_mol = "NCDK.Data.MDL.9553-with-exp-hyd.mol";
            var ins1 = ResourceLoader.GetAsStream(filename_cml);
            var ins2 = ResourceLoader.GetAsStream(filename_mol);
            MDLV2000Reader reader1 = new MDLV2000Reader(ins1, ChemObjectReaderMode.Strict);
            IAtomContainer mol1 = reader1.Read(builder.NewAtomContainer());

            MDLV2000Reader reader2 = new MDLV2000Reader(ins2, ChemObjectReaderMode.Strict);
            var mol2 = reader2.Read(builder.NewAtomContainer());

            var sg = SmilesGenerator.Isomeric;

            Define(mol1, Clockwise(mol1, 0, 1, 5, 12, 13), Clockwise(mol1, 1, 0, 2, 6, 12),
                    Clockwise(mol1, 2, 1, 3, 9, 10), Clockwise(mol1, 5, 0, 4, 11, 18));
            Define(mol2, Clockwise(mol2, 0, 1, 5, 12, 13), Clockwise(mol2, 1, 0, 2, 6, 12),
                    Anticlockwise(mol2, 2, 1, 3, 9, 10), Clockwise(mol2, 5, 0, 4, 11, 18));

            string moleculeSmile1 = sg.Create(mol1);
            string moleculeSmile2 = sg.Create(mol2);
            Assert.AreNotEqual(moleculeSmile2, moleculeSmile1);
        }

        // @cdk.bug 1535055
        [TestMethod()]
        public void TestSFBug1535055()
        {
            string filename_cml = "NCDK.Data.CML.test1.cml";
            var ins1 = ResourceLoader.GetAsStream(filename_cml);
            CMLReader reader1 = new CMLReader(ins1);
            var chemFile = (IChemFile)reader1.Read(builder.NewChemFile());
            Assert.IsNotNull(chemFile);
            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            var model = seq[0];
            Assert.IsNotNull(model);
            IAtomContainer mol1 = model.MoleculeSet[0];
            Assert.IsNotNull(mol1);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AddImplicitHydrogens(mol1);
            Assert.IsTrue(Aromaticity.CDKLegacy.Apply(mol1));

            var sg = new SmilesGenerator().Aromatic();

            string mol1SMILES = sg.Create(mol1);
            Assert.IsTrue(mol1SMILES.Contains("nH"));
        }

        // @cdk.bug 1014344
        [TestMethod()]
        public void TestSFBug1014344_1()
        {
            string filename_cml = "NCDK.Data.CML.bug1014344-1.cml";
            string filename_mol = "NCDK.Data.MDL.bug1014344-1.mol";
            var ins1 = ResourceLoader.GetAsStream(filename_cml);
            var ins2 = ResourceLoader.GetAsStream(filename_mol);
            CMLReader reader1 = new CMLReader(ins1);
            var chemFile = (IChemFile)reader1.Read(builder.NewChemFile());
            var seq = chemFile[0];
            var model = seq[0];
            IAtomContainer mol1 = model.MoleculeSet[0];

            MDLReader reader2 = new MDLReader(ins2);
            var mol2 = reader2.Read(builder.NewAtomContainer());

            AddImplicitHydrogens(mol1);
            AddImplicitHydrogens(mol2);

            var sg = new SmilesGenerator();

            string moleculeSmile1 = sg.Create(mol1);
            //        Debug.WriteLine(filename_cml + " -> " + moleculeSmile1);
            string moleculeSmile2 = sg.Create(mol2);
            //        Debug.WriteLine(filename_mol + " -> " + moleculeSmile2);
            Assert.AreEqual(moleculeSmile1, moleculeSmile2);
        }

        // @cdk.bug 1875946
        [TestMethod()]
        public void TestPreservingFormalCharge()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom(ChemicalElement.O));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(builder.NewAtom(ChemicalElement.C));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            AddImplicitHydrogens(mol);
            var generator = new SmilesGenerator();
            generator.Create(builder.NewAtomContainer(mol));
            Assert.AreEqual(-1, mol.Atoms[0].FormalCharge.Value);
            // mmm, that does not reproduce the bug findings yet :(
        }

        [TestMethod()]
        public void TestIndole()
        {
            var mol = TestMoleculeFactory.MakeIndole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var smilesGenerator = new SmilesGenerator().Aromatic();
            var smiles = smilesGenerator.Create(mol);
            Assert.IsTrue(smiles.Contains("[nH]"));
        }

        [TestMethod()]
        public void TestPyrrole()
        {
            var mol = TestMoleculeFactory.MakePyrrole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            var smilesGenerator = new SmilesGenerator().Aromatic();
            var smiles = smilesGenerator.Create(mol);
            Assert.IsTrue(smiles.Contains("[nH]"));
        }

        // @cdk.bug 1300
        [TestMethod()]
        public void TestDoubleBracketProblem()
        {
            var mol = TestMoleculeFactory.MakePyrrole();
            mol.Atoms[1].FormalCharge = -1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);
            AddImplicitHydrogens(mol);
            var smilesGenerator = new SmilesGenerator().Aromatic();
            var smiles = smilesGenerator.Create(mol);
            Assert.IsFalse(smiles.Contains("[[nH]-]"));
        }

        // @cdk.bug 1300
        [TestMethod()]
        public void TestHydrogenOnChargedNitrogen()
        {
            var mol = TestMoleculeFactory.MakePyrrole();
            mol.Atoms[1].FormalCharge = -1;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AddImplicitHydrogens(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            var smilesGenerator = new SmilesGenerator().Aromatic();
            var smiles = smilesGenerator.Create(mol);
            Assert.IsTrue(smiles.Contains("[n-]"));
        }

        // @cdk.bug 545
        [TestMethod()]
        public void TestTimeOut()
        {
            var filename = "NCDK.Data.MDL.24763.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            var chemFile = reader.Read(builder.NewChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToReadOnlyList();
            Assert.AreEqual(1, containersList.Count);
            var container = containersList[0];
            var smilesGenerator = new SmilesGenerator();
            Assert.IsNotNull(smilesGenerator.Create(container));
        }

        // @cdk.bug 2051597
        [TestMethod()]
        public void TestSFBug2051597()
        {
            var smiles = "c1(c2ccc(c8ccccc8)cc2)" + "c(c3ccc(c9ccccc9)cc3)" + "c(c4ccc(c%10ccccc%10)cc4)"
                + "c(c5ccc(c%11ccccc%11)cc5)" + "c(c6ccc(c%12ccccc%12)cc6)" + "c1(c7ccc(c%13ccccc%13)cc7)";
            var smilesParser = CDK.SmilesParser;
            var cdkMol = smilesParser.ParseSmiles(smiles);
            var smilesGenerator = new SmilesGenerator();
            string genSmiles = smilesGenerator.Create(cdkMol);

            // check that we have the appropriate ring closure symbols
            Assert.IsTrue(genSmiles.Contains("%"), "There were'nt any % ring closures in the output");
            Assert.IsTrue(genSmiles.Contains("%10"));
            Assert.IsTrue(genSmiles.Contains("%11"));
            Assert.IsTrue(genSmiles.Contains("%12"));
            Assert.IsTrue(genSmiles.Contains("%13"));

            // check that we can read in the SMILES we got
            var cdkRoundTripMol = smilesParser.ParseSmiles(genSmiles);
            Assert.IsNotNull(cdkRoundTripMol);
        }

        // @cdk.bug 2596061
        [TestMethod()]
        public void TestRoundTripPseudoAtom()
        {
            var sp = CDK.SmilesParser;
            var smiles = "[12*H2-]";
            var mol = sp.ParseSmiles(smiles);
            var smilesGenerator = SmilesGenerator.Isomeric;
            string genSmiles = smilesGenerator.Create(mol);
            Assert.AreEqual(smiles, genSmiles);
        }

        // @cdk.bug 2781199
        [TestMethod()]
        public void TestBug2781199()
        {
            var sp = CDK.SmilesParser;
            var smiles = "n1ncn(c1)CC";
            var mol = sp.ParseSmiles(smiles);
            var smilesGenerator = new SmilesGenerator().Aromatic();
            string genSmiles = smilesGenerator.Create(mol);
            Assert.IsFalse(genSmiles.Contains("H"), "Generated SMILES should not have explicit H: " + genSmiles);
        }

        // @cdk.bug 2898032
        [TestMethod()]
        public void TestCanSmiWithoutConfiguredAtoms()
        {
            var sp = CDK.SmilesParser;
            string s1 = "OC(=O)C(Br)(Cl)N";
            string s2 = "ClC(Br)(N)C(=O)O";

            var m1 = sp.ParseSmiles(s1);
            var m2 = sp.ParseSmiles(s2);

            var sg = SmilesGenerator.Unique;
            string o1 = sg.Create(m1);
            string o2 = sg.Create(m2);

            Assert.AreEqual(o1, o2, "The two canonical SMILES should match");
        }

        // @cdk.bug 2898032
        [TestMethod()]
        public void TestCanSmiWithConfiguredAtoms()
        {
            var sp = CDK.SmilesParser;
            string s1 = "OC(=O)C(Br)(Cl)N";
            string s2 = "ClC(Br)(N)C(=O)O";

            var m1 = sp.ParseSmiles(s1);
            var m2 = sp.ParseSmiles(s2);

            IsotopeFactory fact = BODRIsotopeFactory.Instance;
            fact.ConfigureAtoms(m1);
            fact.ConfigureAtoms(m2);

            var sg = SmilesGenerator.Unique;
            string o1 = sg.Create(m1);
            string o2 = sg.Create(m2);

            Assert.AreEqual(o1, o2, "The two canonical SMILES should match");
        }

        // @cdk.bug 3040273
        [TestMethod()]
        public void TestBug3040273()
        {
            var sp = CDK.SmilesParser;
            var testSmiles = "C1(C(C(C(C(C1Br)Br)Br)Br)Br)Br";
            var mol = sp.ParseSmiles(testSmiles);
            var fact = BODRIsotopeFactory.Instance;
            fact.ConfigureAtoms(mol);
            var sg = new SmilesGenerator();
            var smiles = sg.Create((IAtomContainer)mol);
            var mol2 = sp.ParseSmiles(smiles);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(mol, mol2));
        }

        [TestMethod()]
        public void TestCreateSMILESWithoutCheckForMultipleMolecules_withDetectAromaticity()
        {
            var benzene = TestMoleculeFactory.MakeBenzene();
            AddImplicitHydrogens(benzene);
            var sg = new SmilesGenerator();
            var smileswithoutaromaticity = sg.Create(benzene);
            Assert.AreEqual("C=1C=CC=CC1", smileswithoutaromaticity);
        }

        [TestMethod()]
        public void TestCreateSMILESWithoutCheckForMultipleMolecules_withoutDetectAromaticity()
        {
            var benzene = TestMoleculeFactory.MakeBenzene();
            AddImplicitHydrogens(benzene);
            var sg = new SmilesGenerator().Aromatic();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(benzene);
            Aromaticity.CDKLegacy.Apply(benzene);
            var smileswitharomaticity = sg.Create(benzene);
            Assert.AreEqual("c1ccccc1", smileswitharomaticity);
        }

        [TestMethod()]
        public void OutputOrder()
        {
            var adenine = TestMoleculeFactory.MakeAdenine();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(adenine);
            CDK.HydrogenAdder.AddImplicitHydrogens(adenine);

            var sg = SmilesGenerator.Generic;
            var order = new int[adenine.Atoms.Count];

            var smi = sg.Create(adenine, order);
            var at = new string[adenine.Atoms.Count];

            for (int i = 0; i < at.Length; i++)
            {
                at[order[i]] = adenine.Atoms[i].AtomTypeName;
            }

            // read in the SMILES
            var sp = CDK.SmilesParser;
            var adenine2 = sp.ParseSmiles(smi);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(adenine2);
            CDK.HydrogenAdder.AddImplicitHydrogens(adenine2);

            // check atom types
            for (int i = 0; i < adenine2.Atoms.Count; i++)
            {
                Assert.AreEqual(adenine2.Atoms[i].AtomTypeName, at[i]);
            }
        }

        [TestMethod()]
        public void OutputCanOrder()
        {
            var adenine = TestMoleculeFactory.MakeAdenine();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(adenine);
            CDK.HydrogenAdder.AddImplicitHydrogens(adenine);

            var sg = SmilesGenerator.Unique;
            var order = new int[adenine.Atoms.Count];

            var smi = sg.Create(adenine, order);
            var at = new string[adenine.Atoms.Count];

            for (int i = 0; i < at.Length; i++)
            {
                at[order[i]] = adenine.Atoms[i].AtomTypeName;
            }

            // read in the SMILES
            var sp = CDK.SmilesParser;
            var adenine2 = sp.ParseSmiles(smi);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(adenine2);
            CDK.HydrogenAdder.AddImplicitHydrogens(adenine2);

            // check atom types
            for (int i = 0; i < adenine2.Atoms.Count; i++)
            {
                Assert.AreEqual(adenine2.Atoms[i].AtomTypeName, at[i]);
            }
        }

        [TestMethod()]
        public void AtomClasses()
        {
            var ethanol = CDK.SmilesParser.ParseSmiles("C[CH2:6]O");
            Assert.AreEqual("CCO", SmilesGenerator.Generic.Create(ethanol));
            Assert.AreEqual("C[CH2:6]O", SmilesGenerator.Generic.WithAtomClasses().Create(ethanol));
        }

        // @cdk.bug 328
        [TestMethod()]
        public void Bug328()
        {
            Assert.AreEqual(
                Canon("Clc1ccc(Cl)c2[nH]c([nH0]c21)C(F)(F)F"),
                Canon("[H]c2c([H])c(c1c(nc(n1([H]))C(F)(F)F)c2Cl)Cl"));
        }

        [TestMethod()]
        public void WarnOnBadInput()
        {
            try
            {
                var smipar = new SmilesParser(CDK.Builder, false);
                var mol = smipar.ParseSmiles("c1ccccc1");
                System.Console.Error.WriteLine(SmilesGenerator.Isomeric.Create(mol));
                Assert.Fail();
            }
            catch (CDKException)
            { }
        }

        /// <summary>
        /// <see href="https://tech.knime.org/forum/cdk/buggy-behavior-of-molecule-to-cdk-node"/>
        /// </summary>
        [TestMethod()]
        public void AssignDbStereo()
        {
            string ins = "C(/N)=C\\C=C\\1/N=C1";
            var smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles(ins);
            Assert.AreEqual("C(\\N)=C/C=C/1N=C1", SmilesGenerator.Isomeric.Create(mol));
        }

        [TestMethod()]
        public void CanonicalReactions()
        {
            var smipar = CDK.SmilesParser;
            var r1 = smipar.ParseReactionSmiles("CC(C)C1=CC=CC=C1.C(CC(=O)Cl)CCl>[Al+3].[Cl-].[Cl-].[Cl-].C(Cl)Cl>CC(C)C1=CC=C(C=C1)C(=O)CCCCl");
            var r2 = smipar.ParseReactionSmiles("C(CC(=O)Cl)CCl.CC(C)C1=CC=CC=C1>[Al+3].[Cl-].[Cl-].[Cl-].C(Cl)Cl>CC(C)C1=CC=C(C=C1)C(=O)CCCCl");
            var r3 = smipar.ParseReactionSmiles("CC(C)C1=CC=CC=C1.C(CC(=O)Cl)CCl>C(Cl)Cl.[Al+3].[Cl-].[Cl-].[Cl-]>CC(C)C1=CC=C(C=C1)C(=O)CCCCl");
            var smigen = new SmilesGenerator(SmiFlavors.Canonical);
            Assert.AreEqual(smigen.Create(r2), smigen.Create(r1));
            Assert.AreEqual(smigen.Create(r3), smigen.Create(r2));
        }

        [TestMethod()]
        public void InconsistentAromaticState()
        {
            try
            {
                var smipar = CDK.SmilesParser;
                var mol = smipar.ParseSmiles("c1ccccc1");
                foreach (IAtom atom in mol.Atoms)
                    atom.IsAromatic = false;
                var smigen = new SmilesGenerator(SmiFlavors.UseAromaticSymbols);
                smigen.Create(mol);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            { }
        }

        [TestMethod()]
        public void StrictIsotopes()
        {
            var smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("[12CH3]C");
            Assert.AreEqual("[12CH3]C", new SmilesGenerator(SmiFlavors.AtomicMassStrict).Create(mol));
        }

        [TestMethod()]
        public void TestIsotopes()
        {
            var smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("[12CH3]C");
            Assert.AreEqual("[12CH3]C", new SmilesGenerator(SmiFlavors.AtomicMass).Create(mol));
        }

        [TestMethod()]
        public void Cyclobutene()
        {
            var smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("C1(C)=C(c2ccccc2)C=C1.C1(C)C(c2ccccc2)=CC=1");
            // by default we generate SMILES that allows all double bonds to move
            // and remove differences just because of kekule assignment. This matches
            // InChI behavior
            Assert.AreEqual("C=1C=CC(=CC1)C=2C=CC2C.C=1C=CC(=CC1)C=2C=CC2C", new SmilesGenerator(SmiFlavors.Canonical).Create(mol));
            // this might not be desirable in some cases, so if UseAromaticSymbols
            // is set, double bonds are in rings are not allowed to move
            Assert.AreEqual("c1ccc(cc1)C=2C=CC2C.c1ccc(cc1)C2=CC=C2C", new SmilesGenerator(SmiFlavors.Canonical | SmiFlavors.UseAromaticSymbols).Create(mol));
        }

        [TestMethod()]
        public void RoundTripExtendedCisTrans()
        {
            var smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("C/C=C=C=C/C");
            Assert.AreEqual("C/C=C=C=C/C", new SmilesGenerator(SmiFlavors.Stereo).Create(mol));
            foreach (var se in mol.StereoElements)
                se.Configure = se.Configure.Flip();
            Assert.AreEqual("C/C=C=C=C\\C", new SmilesGenerator(SmiFlavors.Stereo).Create(mol));
        }

        [TestMethod()]
        public void CanonAtomMaps()
        {
            var smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("[*:2]C(CC[*:3])[*:1]");
            Assert.AreEqual("[*:1]C([*:2])CC[*:3]", new SmilesGenerator(SmiFlavors.Canonical | SmiFlavors.AtomAtomMap).Create(mol));
            var mol2 = smipar.ParseSmiles("[*:2]C(CC[*:1])[*:2]");
            Assert.AreEqual("[*:1]CCC([*:2])[*:2]", new SmilesGenerator(SmiFlavors.Canonical | SmiFlavors.AtomAtomMap).Create(mol2));
        }

        [TestMethod()]
        public void CanonAtomMapsRenumber()
        {
            var smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles("[*:2]C(CC[*:3])[*:1]");
            Assert.AreEqual("[*:1]CCC([*:2])[*:3]", new SmilesGenerator(SmiFlavors.Canonical | SmiFlavors.AtomAtomMapRenumber).Create(mol));
            var mol2 = smipar.ParseSmiles("[*:3]C(CC[*:1])[*:2]");
            Assert.AreEqual("[*:1]CCC([*:2])[*:3]", new SmilesGenerator(SmiFlavors.Canonical | SmiFlavors.AtomAtomMapRenumber).Create(mol2));
        }

        static ITetrahedralChirality Anticlockwise(IAtomContainer container, int central, int a1, int a2, int a3, int a4)
        {
            return new TetrahedralChirality(container.Atoms[central],
                new IAtom[] {
                    container.Atoms[a1], container.Atoms[a2], container.Atoms[a3], container.Atoms[a4] },
                TetrahedralStereo.AntiClockwise);
        }

        static ITetrahedralChirality Clockwise(IAtomContainer container, int central, int a1, int a2, int a3, int a4)
        {
            return new TetrahedralChirality(container.Atoms[central],
                new IAtom[] {
                    container.Atoms[a1], container.Atoms[a2], container.Atoms[a3], container.Atoms[a4] },
                TetrahedralStereo.Clockwise);
        }

        static void Define(IAtomContainer container, params IStereoElement<IChemObject, IChemObject>[] elements)
        {
            container.SetStereoElements(new List<IStereoElement<IChemObject, IChemObject>>(elements));
        }

        static string Canon(string smi)
        {
            var smipar = CDK.SmilesParser;
            var container = smipar.ParseSmiles(smi);
            AtomContainerManipulator.SuppressHydrogens(container);
            var arom = new Aromaticity(ElectronDonation.DaylightModel, Cycles.AllSimpleFinder);
            arom.Apply(container);
            return SmilesGenerator.Unique.Create(container);
        }
    }
}
