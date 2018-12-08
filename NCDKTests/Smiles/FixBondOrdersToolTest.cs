/*
 * Copyright (C) 2006-2007  Rajarshi Guha <rajarshi@users.sf.net>
 * Copyright (C) 2012 Kevin Lawson <kevin.lawson@syngenta.com>
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
using NCDK.Tools.Manipulator;


namespace NCDK.Smiles
{
    // @author         Rajarshi Guha
    // @cdk.created    2006-09-18
    // @cdk.module     test-smiles
    [TestClass()]
    public class FixBondOrdersToolTest : CDKTestCase
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        [Timeout(1000)]
        public void TestPyrrole()
        {
            var smiles = "c2ccc3n([H])c1ccccc1c3(c2)";
            var sp = new SmilesParser(builder, false);
            var molecule = sp.ParseSmiles(smiles);
            AtomContainerManipulator.SetSingleOrDoubleFlags(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            molecule = FixBondOrdersTool.KekuliseAromaticRings(molecule);
            Assert.IsNotNull(molecule);

            molecule = (IAtomContainer)AtomContainerManipulator.RemoveHydrogens(molecule);
            int doubleBondCount = 0;
            for (int i = 0; i < molecule.Bonds.Count; i++)
            {
                IBond bond = molecule.Bonds[i];
                Assert.IsTrue(bond.IsAromatic);
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(6, doubleBondCount);
        }

        [TestMethod()]
        [Timeout(1000)]
        public void TestPyrrole_Silent()
        {
            var smiles = "c2ccc3n([H])c1ccccc1c3(c2)";
            var sp = new SmilesParser(builder, false);
            var molecule = sp.ParseSmiles(smiles);
            AtomContainerManipulator.SetSingleOrDoubleFlags(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            molecule = FixBondOrdersTool.KekuliseAromaticRings(molecule);
            Assert.IsNotNull(molecule);

            molecule = (IAtomContainer)AtomContainerManipulator.RemoveHydrogens(molecule);
            int doubleBondCount = 0;
            for (int i = 0; i < molecule.Bonds.Count; i++)
            {
                IBond bond = molecule.Bonds[i];
                Assert.IsTrue(bond.IsAromatic);
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(6, doubleBondCount);
        }

        [TestMethod()]
        public void TestLargeRingSystem()
        {
            var smiles = "O=C1Oc6ccccc6(C(O)C1C5c2ccccc2CC(c3ccc(cc3)c4ccccc4)C5)";
            var smilesParser = CDK.SmilesParser;
            var molecule = smilesParser.ParseSmiles(smiles);

            molecule = FixBondOrdersTool.KekuliseAromaticRings(molecule);
            Assert.IsNotNull(molecule);

            molecule = (IAtomContainer)AtomContainerManipulator.RemoveHydrogens(molecule);
            Assert.AreEqual(34, molecule.Atoms.Count);

            // we should have 14 double bonds
            int doubleBondCount = 0;
            for (int i = 0; i < molecule.Bonds.Count; i++)
            {
                IBond bond = molecule.Bonds[i];
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(13, doubleBondCount);
        }

        // @cdk.bug 3506770
        [TestMethod()]
        public void TestLargeBioclipseUseCase()
        {
            var smiles = "COc1ccc2[C@@H]3[C@H](COc2c1)C(C)(C)OC4=C3C(=O)C(=O)C5=C4OC(C)(C)[C@@H]6COc7cc(OC)ccc7[C@H]56";
            var smilesParser = CDK.SmilesParser;
            var molecule = smilesParser.ParseSmiles(smiles);

            molecule = FixBondOrdersTool.KekuliseAromaticRings(molecule);
            Assert.IsNotNull(molecule);

            molecule = (IAtomContainer)AtomContainerManipulator.RemoveHydrogens(molecule);
            Assert.AreEqual(40, molecule.Atoms.Count);

            // we should have 14 double bonds
            int doubleBondCount = 0;
            for (int i = 0; i < molecule.Bonds.Count; i++)
            {
                IBond bond = molecule.Bonds[i];
                if (bond.Order == BondOrder.Double) doubleBondCount++;
            }
            Assert.AreEqual(10, doubleBondCount);
        }

        // @cdk.inchi InChI=1/C4H5N/c1-2-4-5-3-1/h1-5H
        [TestMethod()]
        public void XtestPyrrole()
        {
            var enol = builder.NewAtomContainer();

            // atom block
            var atom1 = builder.NewAtom(ChemicalElement.C);
            atom1.Hybridization = Hybridization.SP2;
            var atom2 = builder.NewAtom(ChemicalElement.C);
            atom2.Hybridization = Hybridization.SP2;
            var atom3 = builder.NewAtom(ChemicalElement.C);
            atom3.Hybridization = Hybridization.SP2;
            var atom4 = builder.NewAtom(ChemicalElement.C);
            atom4.Hybridization = Hybridization.SP2;
            var atom5 = builder.NewAtom(ChemicalElement.N);
            atom5.Hybridization = Hybridization.SP2;
            atom5.ImplicitHydrogenCount = 1;

            // bond block
            var bond1 = builder.NewBond(atom1, atom2);
            var bond2 = builder.NewBond(atom2, atom3);
            var bond3 = builder.NewBond(atom3, atom4);
            var bond4 = builder.NewBond(atom4, atom5);
            var bond5 = builder.NewBond(atom5, atom1);

            enol.Atoms.Add(atom1);
            enol.Atoms.Add(atom2);
            enol.Atoms.Add(atom3);
            enol.Atoms.Add(atom4);
            enol.Atoms.Add(atom5);
            enol.Bonds.Add(bond1);
            enol.Bonds.Add(bond2);
            enol.Bonds.Add(bond3);
            enol.Bonds.Add(bond4);
            enol.Bonds.Add(bond5);

            // perceive atom types
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(enol);

            // now have the algorithm have a go at it
            enol = FixBondOrdersTool.KekuliseAromaticRings(enol);
            Assert.IsNotNull(enol);
            //Assert.IsTrue(FixBondOrdersTool.IsOK(enol));

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Double, enol.Bonds[0].Order); ;
            Assert.AreEqual(BondOrder.Single, enol.Bonds[1].Order); ;
            Assert.AreEqual(BondOrder.Double, enol.Bonds[2].Order); ;
            Assert.AreEqual(BondOrder.Single, enol.Bonds[3].Order); ;
            Assert.AreEqual(BondOrder.Single, enol.Bonds[4].Order); ;
        }

        [TestMethod()]
        public void XtestPyridine()
        {
            var enol = builder.NewAtomContainer();

            // atom block
            var atom1 = builder.NewAtom(ChemicalElement.C);
            atom1.Hybridization = Hybridization.SP2;
            var atom2 = builder.NewAtom(ChemicalElement.C);
            atom2.Hybridization = Hybridization.SP2;
            var atom3 = builder.NewAtom(ChemicalElement.C);
            atom3.Hybridization = Hybridization.SP2;
            var atom4 = builder.NewAtom(ChemicalElement.C);
            atom4.Hybridization = Hybridization.SP2;
            var atom5 = builder.NewAtom(ChemicalElement.C);
            atom5.Hybridization = Hybridization.SP2;
            var atom6 = builder.NewAtom(ChemicalElement.N);
            atom6.Hybridization = Hybridization.SP2;

            // bond block
            var bond1 = builder.NewBond(atom1, atom2);
            var bond2 = builder.NewBond(atom2, atom3);
            var bond3 = builder.NewBond(atom3, atom4);
            var bond4 = builder.NewBond(atom4, atom5);
            var bond5 = builder.NewBond(atom5, atom6);
            var bond6 = builder.NewBond(atom6, atom1);

            enol.Atoms.Add(atom1);
            enol.Atoms.Add(atom2);
            enol.Atoms.Add(atom3);
            enol.Atoms.Add(atom4);
            enol.Atoms.Add(atom5);
            enol.Atoms.Add(atom6);
            enol.Bonds.Add(bond1);
            enol.Bonds.Add(bond2);
            enol.Bonds.Add(bond3);
            enol.Bonds.Add(bond4);
            enol.Bonds.Add(bond5);
            enol.Bonds.Add(bond6);

            // perceive atom types
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(enol);

            // now have the algorithm have a go at it
            enol = FixBondOrdersTool.KekuliseAromaticRings(enol);
            Assert.IsNotNull(enol);
            // Assert.IsTrue(dbst.IsOK(enol));

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[0].Order.Numeric()
                    + enol.Bonds[5].Order.Numeric()); // around atom1
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[0].Order.Numeric()
                    + enol.Bonds[1].Order.Numeric()); // around atom2
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[1].Order.Numeric()
                    + enol.Bonds[2].Order.Numeric()); // around atom3
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[2].Order.Numeric()
                    + enol.Bonds[3].Order.Numeric()); // around atom4
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[3].Order.Numeric()
                    + enol.Bonds[4].Order.Numeric()); // around atom5
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[4].Order.Numeric()
                    + enol.Bonds[5].Order.Numeric()); // around atom6
        }

        // @cdk.inchi InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H
        // @cdk.bug   1931262
        [TestMethod()]
        public void XtestBenzene()
        {
            var enol = builder.NewAtomContainer();

            // atom block
            var atom1 = builder.NewAtom(ChemicalElement.C);
            atom1.Hybridization = Hybridization.SP2;
            var atom2 = builder.NewAtom(ChemicalElement.C);
            atom2.Hybridization = Hybridization.SP2;
            var atom3 = builder.NewAtom(ChemicalElement.C);
            atom3.Hybridization = Hybridization.SP2;
            var atom4 = builder.NewAtom(ChemicalElement.C);
            atom4.Hybridization = Hybridization.SP2;
            var atom5 = builder.NewAtom(ChemicalElement.C);
            atom5.Hybridization = Hybridization.SP2;
            var atom6 = builder.NewAtom(ChemicalElement.C);
            atom6.Hybridization = Hybridization.SP2;

            // bond block
            var bond1 = builder.NewBond(atom1, atom2);
            var bond2 = builder.NewBond(atom2, atom3);
            var bond3 = builder.NewBond(atom3, atom4);
            var bond4 = builder.NewBond(atom4, atom5);
            var bond5 = builder.NewBond(atom5, atom6);
            var bond6 = builder.NewBond(atom6, atom1);

            enol.Atoms.Add(atom1);
            enol.Atoms.Add(atom2);
            enol.Atoms.Add(atom3);
            enol.Atoms.Add(atom4);
            enol.Atoms.Add(atom5);
            enol.Atoms.Add(atom6);
            enol.Bonds.Add(bond1);
            enol.Bonds.Add(bond2);
            enol.Bonds.Add(bond3);
            enol.Bonds.Add(bond4);
            enol.Bonds.Add(bond5);
            enol.Bonds.Add(bond6);

            // perceive atom types
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(enol);

            // now have the algorithm have a go at it
            enol = FixBondOrdersTool.KekuliseAromaticRings(enol);
            Assert.IsNotNull(enol);
            //Assert.IsTrue(dbst.IsOK(enol));

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[0].Order.Numeric()
                    + enol.Bonds[5].Order.Numeric()); // around atom1
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[0].Order.Numeric()
                    + enol.Bonds[1].Order.Numeric()); // around atom2
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[1].Order.Numeric()
                    + enol.Bonds[2].Order.Numeric()); // around atom3
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[2].Order.Numeric()
                    + enol.Bonds[3].Order.Numeric()); // around atom4
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[3].Order.Numeric()
                    + enol.Bonds[4].Order.Numeric()); // around atom5
            Assert.AreEqual(BondOrder.Single.Numeric() + BondOrder.Double.Numeric(), enol
                    .Bonds[4].Order.Numeric()
                    + enol.Bonds[5].Order.Numeric()); // around atom6
        }

        /// <summary>
        /// Just to ensure it doesn't throw exceptions
        /// </summary>
        [TestMethod()]
        [Timeout(1000)]
        public void TestAcyclic()
        {
            var smiles = "CCCCCCC";
            var smilesParser = CDK.SmilesParser;
            var molecule = smilesParser.ParseSmiles(smiles);

            molecule = FixBondOrdersTool.KekuliseAromaticRings(molecule);
            Assert.IsNotNull(molecule);
        }
    }
}
