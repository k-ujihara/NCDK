/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.Tools
{
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class IDCreatorTest : CDKTestCase
    {

        public IDCreatorTest()
            : base()
        { }

        [TestMethod()]
        public void TestCreateIDs_IChemObject()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom1 = new Atom("C");
            Atom atom2 = new Atom("C");
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            Bond bond = new Bond(atom1, atom2);
            mol.Bonds.Add(bond);

            IDCreator.CreateIDs(mol);
            Assert.AreEqual("a1", atom1.Id);
            Assert.AreEqual("b1", bond.Id);
            var ids = AtomContainerManipulator.GetAllIDs(mol);
            Assert.AreEqual(4, ids.Count);
        }

        [TestMethod()]
        public void TestKeepingIDs()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom = new Atom("C");
            atom.Id = "atom1";
            mol.Atoms.Add(atom);

            IDCreator.CreateIDs(mol);

            Assert.AreEqual("atom1", atom.Id);
            Assert.IsNotNull(mol.Id);
            var ids = AtomContainerManipulator.GetAllIDs(mol);
            Assert.AreEqual(2, ids.Count);
        }

        [TestMethod()]
        public void TestNoDuplicateCreation()
        {
            IAtomContainer mol = new AtomContainer();
            Atom atom1 = new Atom("C");
            Atom atom2 = new Atom("C");
            atom1.Id = "a1";
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom1);

            IDCreator.CreateIDs(mol);
            Assert.AreEqual("a2", atom2.Id);
            var ids = AtomContainerManipulator.GetAllIDs(mol);
            Assert.AreEqual(3, ids.Count);
        }

        /// <summary>
        // @cdk.bug 1455341
        /// </summary>
        [TestMethod()]
        public void TestCallingTwice()
        {
            var molSet = new AtomContainerSet<IAtomContainer>();
            IAtomContainer mol = new AtomContainer();
            Atom atom0 = new Atom("C");
            Atom atom2 = new Atom("C");
            atom0.Id = "a1";
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom0);
            molSet.Add(mol);

            IDCreator.CreateIDs(molSet);
            var ids = MoleculeSetManipulator.GetAllIDs(molSet);
            Assert.AreEqual(4, ids.Count());

            mol = new AtomContainer();
            Atom atom1 = new Atom("C");
            atom2 = new Atom("C");
            atom1.Id = "a2";
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom1);
            molSet.Add(mol);

            IDCreator.CreateIDs(molSet);
            ids = MoleculeSetManipulator.GetAllIDs(molSet);
            Assert.AreEqual(7, ids.Count());

            mol = new AtomContainer();
            atom1 = new Atom("C");
            atom2 = new Atom("C");
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom1);
            molSet.Add(mol);

            atom0.Id = "atomX";
            ids = MoleculeSetManipulator.GetAllIDs(molSet);
            Assert.IsFalse(ids.Contains("a1"));

            IDCreator.CreateIDs(molSet);
            var idsAfter = MoleculeSetManipulator.GetAllIDs(molSet);
            Assert.IsTrue(idsAfter.Contains("a1"));
            Assert.AreEqual(10, idsAfter.Count());
        }
    }
}
