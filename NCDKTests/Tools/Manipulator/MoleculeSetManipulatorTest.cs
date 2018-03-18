/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Default;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
    // @author     Kai Hartmann
    // @cdk.created    2004-02-20
    [TestClass()]
    public class MoleculeSetManipulatorTest : CDKTestCase
    {
        IAtomContainer mol1 = null;
        IAtomContainer mol2 = null;
        IAtom atomInMol1 = null;
        IBond bondInMol1 = null;
        IAtom atomInMol2 = null;
        IChemObjectSet<IAtomContainer> som = new ChemObjectSet<IAtomContainer>();

        public MoleculeSetManipulatorTest()
            : base()
        { }

        [TestInitialize()]
        public void SetUp()
        {
            mol1 = new AtomContainer();
            atomInMol1 = new Atom("Cl")
            {
                Charge = -1.0,
                FormalCharge = -1,
                ImplicitHydrogenCount = 1
            };
            mol1.Atoms.Add(atomInMol1);
            mol1.Atoms.Add(new Atom("Cl"));
            bondInMol1 = new Bond(atomInMol1, mol1.Atoms[1]);
            mol1.Bonds.Add(bondInMol1);
            mol2 = new AtomContainer();
            atomInMol2 = new Atom("O")
            {
                ImplicitHydrogenCount = 2
            };
            mol2.Atoms.Add(atomInMol2);
            som.Add(mol1);
            som.Add(mol2);
        }

        [TestMethod()]
        public void TestGetAtomCount_IAtomContainerSet()
        {
            int count = MoleculeSetManipulator.GetAtomCount(som);
            Assert.AreEqual(3, count);
        }

        [TestMethod()]
        public void TestGetBondCount_IAtomContainerSet()
        {
            int count = MoleculeSetManipulator.GetBondCount(som);
            Assert.AreEqual(1, count);
        }

        [TestMethod()]
        public void TestRemoveElectronContainer_IAtomContainerSet_IElectronContainer()
        {
            IChemObjectSet<IAtomContainer> ms = new ChemObjectSet<IAtomContainer>();
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            IBond bond = mol.Bonds[0];
            ms.Add(mol);
            IBond otherBond = new Bond(new Atom(), new Atom());
            MoleculeSetManipulator.RemoveElectronContainer(ms, otherBond);
            Assert.AreEqual(1, MoleculeSetManipulator.GetBondCount(ms));
            MoleculeSetManipulator.RemoveElectronContainer(ms, bond);
            Assert.AreEqual(0, MoleculeSetManipulator.GetBondCount(ms));
        }

        [TestMethod()]
        public void TestRemoveAtomAndConnectedElectronContainers_IAtomContainerSet_IAtom()
        {
            IChemObjectSet<IAtomContainer> ms = new ChemObjectSet<IAtomContainer>();
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            IAtom atom = mol.Atoms[0];
            ms.Add(mol);
            IAtom otherAtom = new Atom("O");
            MoleculeSetManipulator.RemoveAtomAndConnectedElectronContainers(ms, otherAtom);
            Assert.AreEqual(1, MoleculeSetManipulator.GetBondCount(ms));
            Assert.AreEqual(2, MoleculeSetManipulator.GetAtomCount(ms));
            MoleculeSetManipulator.RemoveAtomAndConnectedElectronContainers(ms, atom);
            Assert.AreEqual(0, MoleculeSetManipulator.GetBondCount(ms));
            Assert.AreEqual(1, MoleculeSetManipulator.GetAtomCount(ms));
        }

        [TestMethod()]
        public void TestGetTotalCharge_IAtomContainerSet()
        {
            double charge = MoleculeSetManipulator.GetTotalCharge(som);
            Assert.AreEqual(-1.0, charge, 0.000001);
        }

        [TestMethod()]
        public void TestGetTotalFormalCharge_IAtomContainerSet()
        {
            double charge = MoleculeSetManipulator.GetTotalFormalCharge(som);
            Assert.AreEqual(-1.0, charge, 0.000001);
        }

        [TestMethod()]
        public void TestGetTotalHydrogenCount_IAtomContainerSet()
        {
            int hCount = MoleculeSetManipulator.GetTotalHydrogenCount(som);
            Assert.AreEqual(3, hCount);
        }

        [TestMethod()]
        public void TestGetAllIDs_IAtomContainerSet()
        {
            som.Id = "som";
            mol2.Id = "mol";
            atomInMol2.Id = "atom";
            bondInMol1.Id = "bond";
            var list = MoleculeSetManipulator.GetAllIDs(som);
            Assert.AreEqual(4, list.Count());
        }

        [TestMethod()]
        public void TestGetAllAtomContainers_IAtomContainerSet()
        {
            var list = MoleculeSetManipulator.GetAllAtomContainers(som);
            Assert.AreEqual(2, list.Count());
        }

        [TestMethod()]
        public void TestSetAtomProperties_IAtomContainerSet_Object_Object()
        {
            string key = "key";
            string value = "value";
            MoleculeSetManipulator.SetAtomProperties(som, key, value);
            Assert.AreEqual(value, atomInMol1.GetProperty<string>(key));
            Assert.AreEqual(value, atomInMol2.GetProperty<string>(key));
        }

        [TestMethod()]
        public void TestGetRelevantAtomContainer_IAtomContainerSet_IAtom()
        {
            IAtomContainer ac1 = MoleculeSetManipulator.GetRelevantAtomContainer(som, atomInMol1);
            Assert.AreEqual(mol1, ac1);
            IAtomContainer ac2 = MoleculeSetManipulator.GetRelevantAtomContainer(som, atomInMol2);
            Assert.AreEqual(mol2, ac2);
        }

        [TestMethod()]
        public void TestGetRelevantAtomContainer_IAtomContainerSet_IBond()
        {
            IAtomContainer ac1 = MoleculeSetManipulator.GetRelevantAtomContainer(som, bondInMol1);
            Assert.AreEqual(mol1, ac1);
        }

        [TestMethod()]
        public void TestGetAllChemObjects_IAtomContainerSet()
        {
            var list = MoleculeSetManipulator.GetAllChemObjects(som);
            Assert.AreEqual(3, list.Count()); // only MoleculeSets and AtomContainers at the moment (see source code comment)
        }
    }
}
