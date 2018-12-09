/* Copyright (C) 2007  Egon Willighagen <egonw@sci.kun.nl>
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
using NCDK.AtomTypes;
using NCDK.IO;
using NCDK.Numerics;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools
{
    /// <summary>
    /// Tests CDK's hydrogen adding capabilities in terms of
    /// example molecules.
    /// </summary>
    // @cdk.module  test-valencycheck
    // @author      Egon Willighagen <egonw@users.sf.net>
    // @cdk.created 2007-07-28
    [TestClass()]
    public class CDKHydrogenAdderTest
        : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;
        private readonly static IHydrogenAdder adder = CDK.HydrogenAdder;
        private readonly static IAtomTypeMatcher matcher = CDK.AtomTypeMatcher;

        [TestMethod()]
        public void TestInstance()
        {
            Assert.IsNotNull(adder);
        }

        [TestMethod()]
        public void TestMethane()
        {
            var molecule = builder.NewAtomContainer();
            var newAtom = builder.NewAtom(ChemicalElement.C);
            molecule.Atoms.Add(newAtom);
            var type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.AreEqual(4, newAtom.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestFormaldehyde()
        {
            var molecule = builder.NewAtomContainer();
            var newAtom = builder.NewAtom(ChemicalElement.C);
            var newAtom2 = builder.NewAtom(ChemicalElement.O);
            molecule.Atoms.Add(newAtom);
            molecule.Atoms.Add(newAtom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            var type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);
            type = matcher.FindMatchingAtomType(molecule, newAtom2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom2, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.IsNotNull(newAtom2.ImplicitHydrogenCount);
            Assert.AreEqual(2, newAtom.ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, newAtom2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestMethanol()
        {
            var molecule = builder.NewAtomContainer();
            var newAtom = builder.NewAtom(ChemicalElement.C);
            var newAtom2 = builder.NewAtom(ChemicalElement.O);
            molecule.Atoms.Add(newAtom);
            molecule.Atoms.Add(newAtom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            var type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);
            type = matcher.FindMatchingAtomType(molecule, newAtom2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom2, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.IsNotNull(newAtom2.ImplicitHydrogenCount);
            Assert.AreEqual(3, newAtom.ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, newAtom2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestHCN()
        {
            var molecule = builder.NewAtomContainer();
            var newAtom = builder.NewAtom(ChemicalElement.C);
            var newAtom2 = builder.NewAtom(ChemicalElement.N);
            molecule.Atoms.Add(newAtom);
            molecule.Atoms.Add(newAtom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Triple);
            var type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);
            type = matcher.FindMatchingAtomType(molecule, newAtom2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom2, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.IsNotNull(newAtom2.ImplicitHydrogenCount);
            Assert.AreEqual(1, newAtom.ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, newAtom2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestMethylAmine()
        {
            var molecule = builder.NewAtomContainer();
            var newAtom = builder.NewAtom(ChemicalElement.C);
            var newAtom2 = builder.NewAtom(ChemicalElement.N);
            molecule.Atoms.Add(newAtom);
            molecule.Atoms.Add(newAtom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            var type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);
            type = matcher.FindMatchingAtomType(molecule, newAtom2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom2, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.IsNotNull(newAtom2.ImplicitHydrogenCount);
            Assert.AreEqual(3, newAtom.ImplicitHydrogenCount.Value);
            Assert.AreEqual(2, newAtom2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestMethyleneImine()
        {
            var molecule = builder.NewAtomContainer();
            var newAtom = builder.NewAtom(ChemicalElement.C);
            var newAtom2 = builder.NewAtom(ChemicalElement.N);
            molecule.Atoms.Add(newAtom);
            molecule.Atoms.Add(newAtom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            var type = matcher.FindMatchingAtomType(molecule, newAtom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom, type);
            type = matcher.FindMatchingAtomType(molecule, newAtom2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(newAtom2, type);

            Assert.IsNull(newAtom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(molecule);
            Assert.IsNotNull(newAtom.ImplicitHydrogenCount);
            Assert.IsNotNull(newAtom2.ImplicitHydrogenCount);
            Assert.AreEqual(2, newAtom.ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, newAtom2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestSulphur()
        {
            var mol = builder.NewAtomContainer();
            var atom = builder.NewAtom("S");
            mol.Atoms.Add(atom);
            var type = matcher.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(atom, type);

            Assert.AreNotEqual(2, atom.ImplicitHydrogenCount);
            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(1, mol.Atoms.Count);
            Assert.IsNotNull(atom.ImplicitHydrogenCount);
            Assert.AreEqual(2, atom.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestProton()
        {
            var mol = builder.NewAtomContainer();
            var proton = builder.NewAtom("H");
            proton.FormalCharge = +1;
            mol.Atoms.Add(proton);
            var type = matcher.FindMatchingAtomType(mol, proton);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(proton, type);

            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(1, mol.Atoms.Count);
            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula(mol);
            Assert.AreEqual(1,
                    MolecularFormulaManipulator.GetElementCount(formula, ChemicalElement.H));
            Assert.AreEqual(0, mol.GetConnectedBonds(proton).Count());
            Assert.IsNotNull(proton.ImplicitHydrogenCount);
            Assert.AreEqual(0, proton.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestHydrogen()
        {
            var mol = builder.NewAtomContainer();
            var proton = builder.NewAtom("H");
            mol.Atoms.Add(proton);
            var type = matcher.FindMatchingAtomType(mol, proton);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(proton, type);

            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(1, mol.Atoms.Count);
            IMolecularFormula formula = MolecularFormulaManipulator.GetMolecularFormula(mol);
            Assert.AreEqual(2, MolecularFormulaManipulator.GetElementCount(formula, ChemicalElement.H));
            Assert.AreEqual(0, mol.GetConnectedBonds(proton).Count());
            Assert.IsNotNull(proton.ImplicitHydrogenCount);
            Assert.AreEqual(1, proton.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestAmmonia()
        {
            var mol = builder.NewAtomContainer();
            var nitrogen = builder.NewAtom("N");
            mol.Atoms.Add(nitrogen);
            var type = matcher.FindMatchingAtomType(mol, nitrogen);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(nitrogen, type);

            adder.AddImplicitHydrogens(mol);
            Assert.IsNotNull(nitrogen.ImplicitHydrogenCount);
            Assert.AreEqual(3, nitrogen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestAmmonium()
        {
            var mol = builder.NewAtomContainer();
            var nitrogen = builder.NewAtom("N");
            nitrogen.FormalCharge = +1;
            mol.Atoms.Add(nitrogen);
            var type = matcher.FindMatchingAtomType(mol, nitrogen);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(nitrogen, type);

            adder.AddImplicitHydrogens(mol);
            Assert.IsNotNull(nitrogen.ImplicitHydrogenCount);
            Assert.AreEqual(4, nitrogen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestWater()
        {
            var mol = builder.NewAtomContainer();
            var oxygen = builder.NewAtom("O");
            mol.Atoms.Add(oxygen);
            var type = matcher.FindMatchingAtomType(mol, oxygen);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(oxygen, type);

            adder.AddImplicitHydrogens(mol);
            Assert.IsNotNull(oxygen.ImplicitHydrogenCount);
            Assert.AreEqual(2, oxygen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestHydroxonium()
        {
            var mol = builder.NewAtomContainer();
            var oxygen = builder.NewAtom("O");
            oxygen.FormalCharge = +1;
            mol.Atoms.Add(oxygen);
            var type = matcher.FindMatchingAtomType(mol, oxygen);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(oxygen, type);

            adder.AddImplicitHydrogens(mol);
            Assert.IsNotNull(oxygen.ImplicitHydrogenCount);
            Assert.AreEqual(3, oxygen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestHydroxyl()
        {
            var mol = builder.NewAtomContainer();
            var oxygen = builder.NewAtom("O");
            oxygen.FormalCharge = -1;
            mol.Atoms.Add(oxygen);
            var type = matcher.FindMatchingAtomType(mol, oxygen);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(oxygen, type);

            adder.AddImplicitHydrogens(mol);
            Assert.IsNotNull(oxygen.ImplicitHydrogenCount);
            Assert.AreEqual(1, oxygen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestHalogens()
        {
            HalogenTest("I");
            HalogenTest("F");
            HalogenTest("Cl");
            HalogenTest("Br");
        }

        [TestMethod()]
        public void TestHalogenAnions()
        {
            NegativeHalogenTest("I");
            NegativeHalogenTest("F");
            NegativeHalogenTest("Cl");
            NegativeHalogenTest("Br");
        }

        private void HalogenTest(string halogen)
        {
            var mol = builder.NewAtomContainer();
            var atom = builder.NewAtom(halogen);
            mol.Atoms.Add(atom);
            var type = matcher.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(atom, type);

            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(1, mol.Atoms.Count);
            Assert.IsNotNull(atom.ImplicitHydrogenCount);
            Assert.AreEqual(1, atom.ImplicitHydrogenCount.Value);
        }

        private void NegativeHalogenTest(string halogen)
        {
            var mol = builder.NewAtomContainer();
            var atom = builder.NewAtom(halogen);
            atom.FormalCharge = -1;
            mol.Atoms.Add(atom);
            var type = matcher.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(atom, type);

            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(1, mol.Atoms.Count);
            Assert.IsNotNull(atom.ImplicitHydrogenCount);
            Assert.AreEqual(0, atom.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestSulfite()
        {
            var mol = builder.NewAtomContainer();
            var s = builder.NewAtom("S");
            var o1 = builder.NewAtom("O");
            var o2 = builder.NewAtom("O");
            var o3 = builder.NewAtom("O");
            mol.Atoms.Add(s);
            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);
            mol.Atoms.Add(o3);
            var b1 = builder.NewBond(s, o1, BondOrder.Single);
            var b2 = builder.NewBond(s, o2, BondOrder.Single);
            var b3 = builder.NewBond(s, o3, BondOrder.Double);
            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);
            mol.Bonds.Add(b3);
            var type = matcher.FindMatchingAtomType(mol, s);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(s, type);
            type = matcher.FindMatchingAtomType(mol, o1);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(o1, type);
            type = matcher.FindMatchingAtomType(mol, o2);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(o2, type);
            type = matcher.FindMatchingAtomType(mol, o3);
            Assert.IsNotNull(type);
            AtomTypeManipulator.Configure(o3, type);

            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(4, mol.Atoms.Count);
            Assert.AreEqual(3, mol.Bonds.Count);
            Assert.IsNotNull(s.ImplicitHydrogenCount);
            Assert.AreEqual(0, s.ImplicitHydrogenCount.Value);
            Assert.IsNotNull(o1.ImplicitHydrogenCount);
            Assert.AreEqual(1, o1.ImplicitHydrogenCount.Value);
            Assert.IsNotNull(o2.ImplicitHydrogenCount);
            Assert.AreEqual(1, o2.ImplicitHydrogenCount.Value);
            Assert.IsNotNull(o3.ImplicitHydrogenCount);
            Assert.AreEqual(0, o3.ImplicitHydrogenCount.Value);

        }

        [TestMethod()]
        public void TestAceticAcid()
        {
            var mol = builder.NewAtomContainer();
            var carbonylOxygen = builder.NewAtom("O");
            var hydroxylOxygen = builder.NewAtom("O");
            var methylCarbon = builder.NewAtom("C");
            var carbonylCarbon = builder.NewAtom("C");
            mol.Atoms.Add(carbonylOxygen);
            mol.Atoms.Add(hydroxylOxygen);
            mol.Atoms.Add(methylCarbon);
            mol.Atoms.Add(carbonylCarbon);
            var b1 = builder.NewBond(methylCarbon, carbonylCarbon, BondOrder.Single);
            var b2 = builder.NewBond(carbonylOxygen, carbonylCarbon, BondOrder.Double);
            var b3 = builder.NewBond(hydroxylOxygen, carbonylCarbon, BondOrder.Single);
            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);
            mol.Bonds.Add(b3);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(4, mol.Atoms.Count);
            Assert.AreEqual(3, mol.Bonds.Count);
            Assert.AreEqual(0, carbonylOxygen.ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, hydroxylOxygen.ImplicitHydrogenCount.Value);
            Assert.AreEqual(3, methylCarbon.ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, carbonylCarbon.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestEthane()
        {
            var mol = builder.NewAtomContainer();
            var carbon1 = builder.NewAtom("C");
            var carbon2 = builder.NewAtom("C");
            var b = builder.NewBond(carbon1, carbon2, BondOrder.Single);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.Bonds.Add(b);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            Assert.AreEqual(3, carbon1.ImplicitHydrogenCount.Value);
            Assert.AreEqual(3, carbon2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestEthaneWithPReSetImplicitHCount()
        {
            var mol = builder.NewAtomContainer();
            var carbon1 = builder.NewAtom("C");
            var carbon2 = builder.NewAtom("C");
            var b = builder.NewBond(carbon1, carbon2, BondOrder.Single);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.Bonds.Add(b);
            carbon1.ImplicitHydrogenCount = 3;
            carbon2.ImplicitHydrogenCount = 3;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);

            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            Assert.AreEqual(3, carbon1.ImplicitHydrogenCount.Value);
            Assert.AreEqual(3, carbon2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestEthene()
        {
            var mol = builder.NewAtomContainer();
            var carbon1 = builder.NewAtom("C");
            var carbon2 = builder.NewAtom("C");
            var b = builder.NewBond(carbon1, carbon2, BondOrder.Double);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.Bonds.Add(b);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            Assert.AreEqual(2, carbon1.ImplicitHydrogenCount.Value);
            Assert.AreEqual(2, carbon2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestEthyne()
        {
            var mol = builder.NewAtomContainer();
            var carbon1 = builder.NewAtom("C");
            var carbon2 = builder.NewAtom("C");
            var b = builder.NewBond(carbon1, carbon2, BondOrder.Triple);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.Bonds.Add(b);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
            Assert.AreEqual(1, carbon1.ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, carbon2.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestAromaticSaturation()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C")); // 0
            mol.Atoms.Add(builder.NewAtom("C")); // 1
            mol.Atoms.Add(builder.NewAtom("C")); // 2
            mol.Atoms.Add(builder.NewAtom("C")); // 3
            mol.Atoms.Add(builder.NewAtom("C")); // 4
            mol.Atoms.Add(builder.NewAtom("C")); // 5
            mol.Atoms.Add(builder.NewAtom("C")); // 6
            mol.Atoms.Add(builder.NewAtom("C")); // 7

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Single); // 6
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single); // 7
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Triple); // 8

            for (int f = 0; f < 6; f++)
            {
                mol.Atoms[f].IsAromatic = true;
                mol.Atoms[f].Hybridization = Hybridization.SP2;
                mol.Bonds[f].IsAromatic = true;
            }
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(6, AtomContainerManipulator.GetTotalHydrogenCount(mol));
        }

        [TestMethod()]
        public void TestaddImplicitHydrogensToSatisfyValency_OldValue()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            var oxygen = builder.NewAtom("O");
            mol.Atoms.Add(oxygen);
            mol.Atoms.Add(builder.NewAtom("C"));

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.IsNotNull(oxygen.ImplicitHydrogenCount);
            Assert.AreEqual(0, oxygen.ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestAdenine()
        {
            var mol = builder.NewAtomContainer(); // Adenine
            IAtom a1 = mol.Builder.NewAtom("C");
            a1.Point2D = new Vector2(21.0223, -17.2946);
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("C");
            a2.Point2D = new Vector2(21.0223, -18.8093);
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            a3.Point2D = new Vector2(22.1861, -16.6103);
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("N");
            a4.Point2D = new Vector2(19.8294, -16.8677);
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("N");
            a5.Point2D = new Vector2(22.2212, -19.5285);
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.NewAtom("N");
            a6.Point2D = new Vector2(19.8177, -19.2187);
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.NewAtom("N");
            a7.Point2D = new Vector2(23.4669, -17.3531);
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.NewAtom("N");
            a8.Point2D = new Vector2(22.1861, -15.2769);
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.NewAtom("C");
            a9.Point2D = new Vector2(18.9871, -18.0139);
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.NewAtom("C");
            a10.Point2D = new Vector2(23.4609, -18.8267);
            mol.Atoms.Add(a10);
            IBond b1 = mol.Builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a2, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a2, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.NewBond(a3, a7, BondOrder.Double);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.NewBond(a3, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.NewBond(a4, a9, BondOrder.Double);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.NewBond(a5, a10, BondOrder.Double);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.NewBond(a6, a9, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.NewBond(a7, a10, BondOrder.Single);
            mol.Bonds.Add(b11);

            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);
            Assert.AreEqual(5, AtomContainerManipulator.GetTotalHydrogenCount(mol));
        }

        // @cdk.bug 1727373
        [TestMethod()]
        public void TestBug1727373()
        {
            IAtomContainer molecule = null;
            var filename = "NCDK.Data.MDL.carbocations.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            molecule = reader.Read(builder.NewAtomContainer());
            FindAndConfigureAtomTypesForAllAtoms(molecule);
            adder.AddImplicitHydrogens(molecule);
            Assert.AreEqual(2, molecule.Atoms[0].ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, molecule.Atoms[1].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[2].ImplicitHydrogenCount.Value);
            Assert.AreEqual(2, molecule.Atoms[3].ImplicitHydrogenCount.Value);
        }

        // @cdk.bug 1575269
        [TestMethod()]
        public void TestBug1575269()
        {
            var filename = "NCDK.Data.MDL.furan.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            var molecule = reader.Read(builder.NewAtomContainer());
            FindAndConfigureAtomTypesForAllAtoms(molecule);
            adder.AddImplicitHydrogens(molecule);
            Assert.AreEqual(1, molecule.Atoms[0].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[1].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[2].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[3].ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestImpHByAtom()
        {
            var filename = "NCDK.Data.MDL.furan.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            var molecule = reader.Read(builder.NewAtomContainer());
            FindAndConfigureAtomTypesForAllAtoms(molecule);
            foreach (var atom in molecule.Atoms)
            {
                adder.AddImplicitHydrogens(molecule, atom);
            }
            Assert.AreEqual(1, molecule.Atoms[0].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[1].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[2].ImplicitHydrogenCount.Value);
            Assert.AreEqual(1, molecule.Atoms[3].ImplicitHydrogenCount.Value);
        }

        [TestMethod()]
        public void TestPseudoAtom()
        {
            var molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewPseudoAtom("Waterium"));
            FindAndConfigureAtomTypesForAllAtoms(molecule);
            Assert.IsNull(molecule.Atoms[0].ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void TestNaCl()
        {
            var mol = builder.NewAtomContainer();
            var cl = builder.NewAtom("Cl");
            cl.FormalCharge = -1;
            mol.Atoms.Add(cl);
            var na = builder.NewAtom("Na");
            na.FormalCharge = +1;
            mol.Atoms.Add(na);
            FindAndConfigureAtomTypesForAllAtoms(mol);
            adder.AddImplicitHydrogens(mol);

            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(0, AtomContainerManipulator.GetTotalHydrogenCount(mol));
            Assert.AreEqual(0, mol.GetConnectedBonds(cl).Count());
            Assert.AreEqual(0, cl.ImplicitHydrogenCount.Value);
            Assert.AreEqual(0, mol.GetConnectedBonds(na).Count());
            Assert.AreEqual(0, na.ImplicitHydrogenCount.Value);
        }

        // @cdk.bug 1244612
        [TestMethod()]
        public void TestSulfurCompound_ImplicitHydrogens()
        {
            var filename = "NCDK.Data.MDL.sulfurCompound.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            var chemFile = reader.Read(builder.NewChemFile());
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToReadOnlyList();
            Assert.AreEqual(1, containersList.Count);

            var atomContainer_0 = (IAtomContainer)containersList[0];
            Assert.AreEqual(10, atomContainer_0.Atoms.Count);
            var sulfur = atomContainer_0.Atoms[1];
            FindAndConfigureAtomTypesForAllAtoms(atomContainer_0);
            adder.AddImplicitHydrogens(atomContainer_0);
            Assert.AreEqual("S", sulfur.Symbol);
            Assert.IsNotNull(sulfur.ImplicitHydrogenCount);
            Assert.AreEqual(0, sulfur.ImplicitHydrogenCount.Value);
            Assert.AreEqual(3, atomContainer_0.GetConnectedBonds(sulfur).Count());
            Assert.AreEqual(10, atomContainer_0.Atoms.Count);
            Assert.IsNotNull(sulfur.ImplicitHydrogenCount);
            Assert.AreEqual(0, sulfur.ImplicitHydrogenCount.Value);
            Assert.AreEqual(3, atomContainer_0.GetConnectedBonds(sulfur).Count());
        }

        // @cdk.bug 1627763
        [TestMethod()]
        public void TestBug1627763()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("O"));
            mol.Bonds.Add(mol.Builder.NewBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single));
            AddExplicitHydrogens(mol);
            int hCount = 0;
            IEnumerator<IAtom> neighbors = mol.GetConnectedAtoms(mol.Atoms[0]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(3, hCount);
            hCount = 0;
            neighbors = mol.GetConnectedAtoms(mol.Atoms[1]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(1, hCount);
        }

        [TestMethod()]
        public void TestMercaptan()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("S"));
            mol.Bonds.Add(mol.Builder.NewBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double));
            mol.Bonds.Add(mol.Builder.NewBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single));
            mol.Bonds.Add(mol.Builder.NewBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single));
            AddExplicitHydrogens(mol);
            int hCount = 0;
            IEnumerator<IAtom> neighbors = mol.GetConnectedAtoms(mol.Atoms[0]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(2, hCount);
            hCount = 0;
            neighbors = mol.GetConnectedAtoms(mol.Atoms[1]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(1, hCount);
            hCount = 0;
            neighbors = mol.GetConnectedAtoms(mol.Atoms[2]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(2, hCount);
            hCount = 0;
            neighbors = mol.GetConnectedAtoms(mol.Atoms[3]).GetEnumerator();
            while (neighbors.MoveNext())
            {
                if (neighbors.Current.Symbol.Equals("H")) hCount++;
            }
            Assert.AreEqual(1, hCount);
        }

        [TestMethod()]
        public void UnknownAtomTypeLeavesHydrogenCountAlone()
        {
            var bldr = CDK.Builder;
            var hydrogenAdder = CDK.HydrogenAdder;
            var container = bldr.NewAtomContainer();
            var atom = bldr.NewAtom("C");
            atom.ImplicitHydrogenCount = 3;
            atom.AtomTypeName = "X";
            container.Atoms.Add(atom);
            hydrogenAdder.AddImplicitHydrogens(container);
            Assert.AreEqual(3, atom.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void UnknownAtomTypeLeavesHydrogenCountAloneUnlessNull()
        {
            var bldr = CDK.Builder;
            var hydrogenAdder = CDK.HydrogenAdder;
            var container = bldr.NewAtomContainer();
            var atom = bldr.NewAtom("C");
            atom.ImplicitHydrogenCount = null;
            atom.AtomTypeName = "X";
            container.Atoms.Add(atom);
            hydrogenAdder.AddImplicitHydrogens(container);
            Assert.AreEqual(0, atom.ImplicitHydrogenCount);
        }

        private void FindAndConfigureAtomTypesForAllAtoms(IAtomContainer container)
        {
            var atoms = container.Atoms.GetEnumerator();
            while (atoms.MoveNext())
            {
                var atom = atoms.Current;
                var type = matcher.FindMatchingAtomType(container, atom);
                Assert.IsNotNull(type);
                AtomTypeManipulator.Configure(atom, type);
            }
        }
    }
}
