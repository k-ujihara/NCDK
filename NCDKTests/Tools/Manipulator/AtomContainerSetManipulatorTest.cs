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
using NCDK.Silent;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
    // @author     Kai Hartmann
    // @cdk.created    2004-02-20
    [TestClass()]
    public class AtomContainerSetManipulatorTest : CDKTestCase
    {
        IAtomContainer mol1 = null;
        IAtomContainer mol2 = null;
        IAtom atomInMol1 = null;
        IBond bondInMol1 = null;
        IAtom atomInMol2 = null;
        IChemObjectSet<IAtomContainer> som = new ChemObjectSet<IAtomContainer>();

        public AtomContainerSetManipulatorTest()
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
            int count = AtomContainerSetManipulator.GetAtomCount(som);
            Assert.AreEqual(3, count);
        }

        [TestMethod()]
        public void TestGetBondCount_IAtomContainerSet()
        {
            int count = AtomContainerSetManipulator.GetBondCount(som);
            Assert.AreEqual(1, count);
        }

        [TestMethod()]
        public void TestRemoveElectronContainer_IAtomContainerSet_IElectronContainer()
        {
            var ms = new ChemObjectSet<IAtomContainer>();
            var mol = new AtomContainer();
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            IBond bond = mol.Bonds[0];
            ms.Add(mol);
            IBond otherBond = new Bond(new Atom(), new Atom());
            AtomContainerSetManipulator.RemoveElectronContainer(ms, otherBond);
            Assert.AreEqual(1, AtomContainerSetManipulator.GetBondCount(ms));
            AtomContainerSetManipulator.RemoveElectronContainer(ms, bond);
            Assert.AreEqual(0, AtomContainerSetManipulator.GetBondCount(ms));
        }

        [TestMethod()]
        public void TestRemoveAtomAndConnectedElectronContainers_IAtomContainerSet_IAtom()
        {
            var ms = new ChemObjectSet<IAtomContainer>();
            var mol = new AtomContainer();
            mol.Atoms.Add(new Atom("O"));
            mol.Atoms.Add(new Atom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            IAtom atom = mol.Atoms[0];
            ms.Add(mol);
            IAtom otherAtom = new Atom("O");
            AtomContainerSetManipulator.RemoveAtomAndConnectedElectronContainers(ms, otherAtom);
            Assert.AreEqual(1, AtomContainerSetManipulator.GetBondCount(ms));
            Assert.AreEqual(2, AtomContainerSetManipulator.GetAtomCount(ms));
            AtomContainerSetManipulator.RemoveAtomAndConnectedElectronContainers(ms, atom);
            Assert.AreEqual(0, AtomContainerSetManipulator.GetBondCount(ms));
            Assert.AreEqual(1, AtomContainerSetManipulator.GetAtomCount(ms));
        }

        [TestMethod()]
        public void TestGetTotalCharge_IAtomContainerSet()
        {
            double charge = AtomContainerSetManipulator.GetTotalCharge(som);
            Assert.AreEqual(-1.0, charge, 0.000001);
        }

        [TestMethod()]
        public void TestGetTotalFormalCharge_IAtomContainerSet()
        {
            double charge = AtomContainerSetManipulator.GetTotalFormalCharge(som);
            Assert.AreEqual(-1.0, charge, 0.000001);
        }

        [TestMethod()]
        public void TestGetTotalHydrogenCount_IAtomContainerSet()
        {
            int hCount = AtomContainerSetManipulator.GetTotalHydrogenCount(som);
            Assert.AreEqual(3, hCount);
        }

        [TestMethod()]
        public void TestGetAllIDs_IAtomContainerSet()
        {
            som.Id = "som";
            mol2.Id = "mol";
            atomInMol2.Id = "atom";
            bondInMol1.Id = "bond";
            var list = AtomContainerSetManipulator.GetAllIDs(som);
            Assert.AreEqual(4, list.Count());
        }

        [TestMethod()]
        public void TestGetAllAtomContainers_IAtomContainerSet()
        {
            var list = AtomContainerSetManipulator.GetAllAtomContainers(som);
            Assert.AreEqual(2, list.Count());
        }

        [TestMethod()]
        public void TestSetAtomProperties_IAtomContainerSet_Object_Object()
        {
            string key = "key";
            string value = "value";
            AtomContainerSetManipulator.SetAtomProperties(som, key, value);
            Assert.AreEqual(value, atomInMol1.GetProperty<string>(key));
            Assert.AreEqual(value, atomInMol2.GetProperty<string>(key));
        }

        [TestMethod()]
        public void TestGetRelevantAtomContainer_IAtomContainerSet_IAtom()
        {
            IAtomContainer ac1 = AtomContainerSetManipulator.GetRelevantAtomContainer(som, atomInMol1);
            Assert.AreEqual(mol1, ac1);
            IAtomContainer ac2 = AtomContainerSetManipulator.GetRelevantAtomContainer(som, atomInMol2);
            Assert.AreEqual(mol2, ac2);
        }

        [TestMethod()]
        public void TestGetRelevantAtomContainer_IAtomContainerSet_IBond()
        {
            IAtomContainer ac1 = AtomContainerSetManipulator.GetRelevantAtomContainer(som, bondInMol1);
            Assert.AreEqual(mol1, ac1);
        }

        [TestMethod()]
        public void TestGetAllChemObjects_IAtomContainerSet()
        {
            var list = AtomContainerSetManipulator.GetAllChemObjects(som);
            Assert.AreEqual(3, list.Count()); // only AtomContainerSets and AtomContainers at the moment (see source code comment)
        }

        [TestMethod()]
        public void TestSort_IAtomContainerSet()
        {
            // Create some IAtomContainers
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            IRing cycloPentane = builder.NewRing(5, "C");
            IRing cycloHexane = builder.NewRing(6, "C");
            IAtomContainer hexaneNitrogen = builder.NewRing(6, "N");
            hexaneNitrogen.Bonds.RemoveAt(0);
            IRing cycloHexaneNitrogen = builder.NewRing(6, "N");
            IRing cycloHexeneNitrogen = builder.NewRing(6, "N");
            cycloHexeneNitrogen.Bonds[0].Order = BondOrder.Double;

            // Add them to a IAtomContainerSet
            var atomContainerSet = builder.NewAtomContainerSet();
            atomContainerSet.Add(cycloHexane);
            atomContainerSet.Add(cycloHexeneNitrogen);
            atomContainerSet.Add(cycloPentane);
            atomContainerSet.Add(hexaneNitrogen);
            atomContainerSet.Add(cycloHexaneNitrogen);

            // Sort the IAtomContainerSet
            AtomContainerSetManipulator.Sort(atomContainerSet);

            // Assert.assert the correct order
            Assert.AreSame(cycloPentane, atomContainerSet[0], "first order: cycloPentane");
            Assert.AreSame(cycloHexane, atomContainerSet[1], "second order: cycloHexane");
            Assert.AreSame(hexaneNitrogen, atomContainerSet[2], "third order: hexaneNitrogen");
            Assert.AreSame(cycloHexaneNitrogen, atomContainerSet[3], "forth order: cycloHexaneNitrogen");
            Assert.AreSame(cycloHexeneNitrogen, atomContainerSet[4], "firth order: cycloHexeneNitrogen");
        }

        [TestMethod()]
        public void TestContainsByID_IAtomContainerSet_IAtomContainer()
        {
            IAtomContainer relevantAtomContainer = ChemObjectBuilder.Instance.NewAtomContainer();
            var atomContainerSet = ChemObjectBuilder.Instance.NewAtomContainerSet();
            atomContainerSet.Add(relevantAtomContainer);
            Assert.IsFalse(AtomContainerSetManipulator.ContainsByID(atomContainerSet, relevantAtomContainer.Id));
            relevantAtomContainer.Id = "1";
            Assert.IsTrue(AtomContainerSetManipulator.ContainsByID(atomContainerSet, relevantAtomContainer.Id));
        }
    }
}
