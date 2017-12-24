/* Copyright (C) 2006-2007  Rajarshi Guha <rajarshi@users.sf.net>
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
using NCDK.SGroups;
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;
using NCDK.Default;
using NCDK.Config;

namespace NCDK.Smiles
{
    // @author         Rajarshi Guha
    // @cdk.created    2006-09-18
    // @cdk.module     test-smiles
    [TestClass()]
    public class DeduceBondSystemToolTest : CDKTestCase
    {
        private static DeduceBondSystemTool dbst = new DeduceBondSystemTool();

        [TestMethod()]
        public void TestConstructors()
        {
            // basically: just test that no exception is thrown
            Assert.IsNotNull(new DeduceBondSystemTool());
            Assert.IsNotNull(new DeduceBondSystemTool(new AllRingsFinder()));
        }

        [TestMethod()]
        public void TestInterruption()
        {
            dbst.Interrupted = false;
            Assert.IsFalse(dbst.Interrupted);
            dbst.Interrupted = true;
            Assert.IsTrue(dbst.Interrupted);
            dbst.Interrupted = false;
        }

        [TestMethod()]
        [Timeout(1000)]
        public void TestPyrrole()
        {
            string smiles = "c2ccc3n([H])c1ccccc1c3(c2)";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            smilesParser.Kekulise(false);
            IAtomContainer molecule = smilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.SetSingleOrDoubleFlags(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            molecule = dbst.FixAromaticBondOrders(molecule);
            Assert.IsNotNull(molecule);

            molecule = AtomContainerManipulator.RemoveHydrogens(molecule);
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
            string smiles = "c2ccc3n([H])c1ccccc1c3(c2)";
            SmilesParser smilesParser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            smilesParser.Kekulise(false);
            IAtomContainer molecule = smilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.SetSingleOrDoubleFlags(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            molecule = dbst.FixAromaticBondOrders(molecule);
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
            string smiles = "O=C1Oc6ccccc6(C(O)C1C5c2ccccc2CC(c3ccc(cc3)c4ccccc4)C5)";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = smilesParser.ParseSmiles(smiles);

            DeduceBondSystemTool dbst = new DeduceBondSystemTool(new AllRingsFinder());
            molecule = dbst.FixAromaticBondOrders(molecule);
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
        [TestMethod(), Ignore()] // This is an example structure where this class fails
        public void TestLargeBioclipseUseCase()
        {
            string smiles = "COc1ccc2[C@@H]3[C@H](COc2c1)C(C)(C)OC4=C3C(=O)C(=O)C5=C4OC(C)(C)[C@@H]6COc7cc(OC)ccc7[C@H]56";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = smilesParser.ParseSmiles(smiles);

            DeduceBondSystemTool dbst = new DeduceBondSystemTool(new AllRingsFinder());
            molecule = dbst.FixAromaticBondOrders(molecule);
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

        [TestMethod()]
        [Timeout(1000)]
        public void TestPyrrole_CustomRingFinder()
        {
            string smiles = "c2ccc3n([H])c1ccccc1c3(c2)";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            smilesParser.Kekulise(false);
            IAtomContainer molecule = smilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.SetSingleOrDoubleFlags(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            DeduceBondSystemTool dbst = new DeduceBondSystemTool(new AllRingsFinder());
            molecule = dbst.FixAromaticBondOrders(molecule);
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

        // @cdk.inchi InChI=1/C6H4O2/c7-5-1-2-6(8)4-3-5/h1-4H
        [TestMethod(), Ignore()] // previouls disabled 'xtest'
        public void XtestQuinone()
        {
            IAtomContainer enol = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            atom1.Hybridization = Hybridization.SP2;
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            atom2.Hybridization = Hybridization.SP2;
            IAtom atom3 = new Atom(Elements.Carbon.ToIElement());
            atom3.Hybridization = Hybridization.SP2;
            IAtom atom4 = new Atom(Elements.Carbon.ToIElement());
            atom4.Hybridization = Hybridization.SP2;
            IAtom atom5 = new Atom(Elements.Carbon.ToIElement());
            atom5.Hybridization = Hybridization.SP2;
            IAtom atom6 = new Atom(Elements.Carbon.ToIElement());
            atom6.Hybridization = Hybridization.SP2;
            IAtom atom7 = new Atom(Elements.Oxygen.ToIElement());
            atom7.Hybridization = Hybridization.SP2;
            IAtom atom8 = new Atom(Elements.Oxygen.ToIElement());
            atom8.Hybridization = Hybridization.SP2;

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);
            IBond bond3 = new Bond(atom3, atom4);
            IBond bond4 = new Bond(atom4, atom5);
            IBond bond5 = new Bond(atom5, atom6);
            IBond bond6 = new Bond(atom6, atom1);
            IBond bond7 = new Bond(atom7, atom1);
            IBond bond8 = new Bond(atom8, atom4);

            enol.Atoms.Add(atom1);
            enol.Atoms.Add(atom2);
            enol.Atoms.Add(atom3);
            enol.Atoms.Add(atom4);
            enol.Atoms.Add(atom5);
            enol.Atoms.Add(atom6);
            enol.Atoms.Add(atom7);
            enol.Atoms.Add(atom8);
            enol.Bonds.Add(bond1);
            enol.Bonds.Add(bond2);
            enol.Bonds.Add(bond3);
            enol.Bonds.Add(bond4);
            enol.Bonds.Add(bond5);
            enol.Bonds.Add(bond6);
            enol.Bonds.Add(bond7);
            enol.Bonds.Add(bond8);

            // perceive atom types
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(enol);

            // now have the algorithm have a go at it
            enol = dbst.FixAromaticBondOrders(enol);
            Assert.IsNotNull(enol);
            Assert.IsTrue(dbst.IsOK(enol));

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Single, enol.Bonds[0].Order);
            Assert.AreEqual(BondOrder.Double, enol.Bonds[1].Order);
            Assert.AreEqual(BondOrder.Single, enol.Bonds[2].Order);
            Assert.AreEqual(BondOrder.Single, enol.Bonds[3].Order);
            Assert.AreEqual(BondOrder.Double, enol.Bonds[4].Order);
            Assert.AreEqual(BondOrder.Single, enol.Bonds[5].Order);
            Assert.AreEqual(BondOrder.Double, enol.Bonds[6].Order);
            Assert.AreEqual(BondOrder.Double, enol.Bonds[7].Order);
        }

        // @cdk.inchi InChI=1/C4H5N/c1-2-4-5-3-1/h1-5H
        [TestMethod()]
        public void XtestPyrrole()
        {
            IAtomContainer enol = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            atom1.Hybridization = Hybridization.SP2;
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            atom2.Hybridization = Hybridization.SP2;
            IAtom atom3 = new Atom(Elements.Carbon.ToIElement());
            atom3.Hybridization = Hybridization.SP2;
            IAtom atom4 = new Atom(Elements.Carbon.ToIElement());
            atom4.Hybridization = Hybridization.SP2;
            IAtom atom5 = new Atom(Elements.Nitrogen.ToIElement());
            atom5.Hybridization = Hybridization.SP2;
            atom5.ImplicitHydrogenCount = 1;

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);
            IBond bond3 = new Bond(atom3, atom4);
            IBond bond4 = new Bond(atom4, atom5);
            IBond bond5 = new Bond(atom5, atom1);

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
            enol = dbst.FixAromaticBondOrders(enol);
            Assert.IsNotNull(enol);
            Assert.IsTrue(dbst.IsOK(enol));

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
            IAtomContainer enol = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            atom1.Hybridization = Hybridization.SP2;
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            atom2.Hybridization = Hybridization.SP2;
            IAtom atom3 = new Atom(Elements.Carbon.ToIElement());
            atom3.Hybridization = Hybridization.SP2;
            IAtom atom4 = new Atom(Elements.Carbon.ToIElement());
            atom4.Hybridization = Hybridization.SP2;
            IAtom atom5 = new Atom(Elements.Carbon.ToIElement());
            atom5.Hybridization = Hybridization.SP2;
            IAtom atom6 = new Atom(Elements.Nitrogen.ToIElement());
            atom6.Hybridization = Hybridization.SP2;

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);
            IBond bond3 = new Bond(atom3, atom4);
            IBond bond4 = new Bond(atom4, atom5);
            IBond bond5 = new Bond(atom5, atom6);
            IBond bond6 = new Bond(atom6, atom1);

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
            enol = dbst.FixAromaticBondOrders(enol);
            Assert.IsNotNull(enol);
            Assert.IsTrue(dbst.IsOK(enol));

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[0].Order.Numeric
                    + enol.Bonds[5].Order.Numeric); // around atom1
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[0].Order.Numeric
                    + enol.Bonds[1].Order.Numeric); // around atom2
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[1].Order.Numeric
                    + enol.Bonds[2].Order.Numeric); // around atom3
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[2].Order.Numeric
                    + enol.Bonds[3].Order.Numeric); // around atom4
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[3].Order.Numeric
                    + enol.Bonds[4].Order.Numeric); // around atom5
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[4].Order.Numeric
                    + enol.Bonds[5].Order.Numeric); // around atom6
        }

        // @cdk.inchi InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H
        // @cdk.bug   1931262
            [TestMethod()]
        public void XtestBenzene()
        {
            IAtomContainer enol = new AtomContainer();

            // atom block
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            atom1.Hybridization = Hybridization.SP2;
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            atom2.Hybridization = Hybridization.SP2;
            IAtom atom3 = new Atom(Elements.Carbon.ToIElement());
            atom3.Hybridization = Hybridization.SP2;
            IAtom atom4 = new Atom(Elements.Carbon.ToIElement());
            atom4.Hybridization = Hybridization.SP2;
            IAtom atom5 = new Atom(Elements.Carbon.ToIElement());
            atom5.Hybridization = Hybridization.SP2;
            IAtom atom6 = new Atom(Elements.Carbon.ToIElement());
            atom6.Hybridization = Hybridization.SP2;

            // bond block
            IBond bond1 = new Bond(atom1, atom2);
            IBond bond2 = new Bond(atom2, atom3);
            IBond bond3 = new Bond(atom3, atom4);
            IBond bond4 = new Bond(atom4, atom5);
            IBond bond5 = new Bond(atom5, atom6);
            IBond bond6 = new Bond(atom6, atom1);

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
            enol = dbst.FixAromaticBondOrders(enol);
            Assert.IsNotNull(enol);
            Assert.IsTrue(dbst.IsOK(enol));

            // now check whether it did the right thing
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[0].Order.Numeric
                    + enol.Bonds[5].Order.Numeric); // around atom1
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[0].Order.Numeric
                    + enol.Bonds[1].Order.Numeric); // around atom2
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[1].Order.Numeric
                    + enol.Bonds[2].Order.Numeric); // around atom3
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[2].Order.Numeric
                    + enol.Bonds[3].Order.Numeric); // around atom4
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[3].Order.Numeric
                    + enol.Bonds[4].Order.Numeric); // around atom5
            Assert.AreEqual(BondOrder.Single.Numeric + BondOrder.Double.Numeric, enol
                    .Bonds[4].Order.Numeric
                    + enol.Bonds[5].Order.Numeric); // around atom6
        }
    }
}
