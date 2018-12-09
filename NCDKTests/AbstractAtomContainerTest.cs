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
using NCDK.Common.Base;
using NCDK.Numerics;
using NCDK.Sgroups;
using NCDK.Stereo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of the AtomContainer.
    /// </summary>
    // @cdk.module test-interfaces
    public abstract class AbstractAtomContainerTest
        : AbstractChemObjectTest
    {
        private static IAtom[] MakeTestAtomsArray(IAtomContainer ac)
        {
            var atoms = new IAtom[4];
            atoms[0] = ac.Builder.NewAtom("C");
            atoms[1] = ac.Builder.NewAtom("C");
            atoms[2] = ac.Builder.NewAtom("C");
            atoms[3] = ac.Builder.NewAtom("O");
            foreach (var atom in atoms)
                ac.Atoms.Add(atom);
            return atoms;
        }

        [TestMethod()]
        public virtual void TestSetAtoms_arrayIAtom()
        {
            var ac = (IAtomContainer)NewChemObject();
            var atoms = MakeTestAtomsArray(ac);
            Assert.AreEqual(4, ac.Atoms.Count);
        }

        // @cdk.bug 2993609
        [TestMethod()]
        public virtual void TestSetAtoms_RemoveListener()
        {
            var ac = (IAtomContainer)NewChemObject();
            var atoms = MakeTestAtomsArray(ac);

            // if an atom changes, the atomcontainer will throw a change event too
            var listener = new ChemObjectListenerImpl();
            ac.Listeners.Add(listener);
            Assert.IsFalse(listener.Changed);

            // ok, change the atom, and make sure we do get an event
            atoms[0].AtomTypeName = "C.sp2";
            Assert.IsTrue(listener.Changed);

            // reset the listener, overwrite the atoms, and change an old atom.
            // if all is well, we should not get a change event this time
            ac.Atoms.Clear();
            listener.Reset(); // reset here, because the SetAtoms() triggers a change even too
            atoms[1].AtomTypeName = "C.sp2"; // make a change to an old atom
            Assert.IsFalse(listener.Changed); // but no change event should happen
        }

        /// <summary>
        /// Only test whether the atoms are correctly cloned.
        /// </summary>
        [TestMethod()]
        public override void TestClone()
        {
            var ac = (IAtomContainer)NewChemObject();
            var molecule = (IAtomContainer)NewChemObject();
            object clone = molecule.Clone();
            Assert.IsInstanceOfType(clone, typeof(IAtomContainer));
        }

        [TestMethod()]
        public virtual void TestClone_IAtom()
        {
            var molecule = (IAtomContainer)NewChemObject();
            for (int i = 0; i < 4; i++)
                molecule.Atoms.Add(molecule.Builder.NewAtom("C"));

            var clonedMol = (IAtomContainer)molecule.Clone();
            Assert.AreEqual(molecule.Atoms.Count, clonedMol.Atoms.Count);
            for (int f = 0; f < molecule.Atoms.Count; f++)
            {
                for (int g = 0; g < clonedMol.Atoms.Count; g++)
                {
                    Assert.IsNotNull(molecule.Atoms[f]);
                    Assert.IsNotNull(clonedMol.Atoms[g]);
                    Assert.AreNotSame(molecule.Atoms[f], clonedMol.Atoms[g]);
                }
            }
        }

        [TestMethod()]
        public virtual void TestCloneButKeepOriginalsIntact()
        {
            var molecule = (IAtomContainer)NewChemObject();
            var atom = molecule.Builder.NewAtom();
            molecule.Atoms.Add(atom);
            Assert.AreEqual(atom, molecule.Atoms[0]);
            object clone = molecule.Clone();
            Assert.AreNotSame(molecule, clone);
            // after the cloning the IAtom on the original IAtomContainer should be unchanged
            Assert.AreEqual(atom, molecule.Atoms[0]);
        }

        [TestMethod()]
        public virtual void TestCloneButKeepOriginalsIntact_IBond()
        {
            var molecule = (IAtomContainer)NewChemObject();
            molecule.Atoms.Add(molecule.Builder.NewAtom());
            molecule.Atoms.Add(molecule.Builder.NewAtom());
            var bond = molecule.Builder.NewBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Bonds.Add(bond);
            Assert.AreEqual(bond, molecule.Bonds[0]);
            object clone = molecule.Clone();
            Assert.AreNotSame(molecule, clone);
            // after the cloning the IBond on the original IAtomContainer should be unchanged
            Assert.AreEqual(bond, molecule.Bonds[0]);
        }

        [TestMethod()]
        public virtual void TestCloneButKeepOriginalsIntact_ILonePair()
        {
            var molecule = (IAtomContainer)NewChemObject();
            molecule.Atoms.Add(molecule.Builder.NewAtom());
            var lonePair = molecule.Builder.NewLonePair(molecule.Atoms[0]);
            molecule.LonePairs.Add(lonePair);
            Assert.AreEqual(lonePair, molecule.LonePairs[0]);
            object clone = molecule.Clone();
            Assert.AreNotSame(molecule, clone);
            // after the cloning the ILonePair on the original IAtomContainer should be unchanged
            Assert.AreEqual(lonePair, molecule.LonePairs[0]);
        }

        [TestMethod()]
        public virtual void TestCloneButKeepOriginalsIntact_ISingleElectron()
        {
            var molecule = (IAtomContainer)NewChemObject();
            molecule.Atoms.Add(molecule.Builder.NewAtom());
            var singleElectron = molecule.Builder.NewSingleElectron(molecule.Atoms[0]);
            molecule.SingleElectrons.Add(singleElectron);
            Assert.AreEqual(singleElectron, molecule.SingleElectrons[0]);
            object clone = molecule.Clone();
            Assert.AreNotSame(molecule, clone);
            // after the cloning the ISingleElectron on the original IAtomContainer should be unchanged
            Assert.AreEqual(singleElectron, molecule.SingleElectrons[0]);
        }

        [TestMethod()]
        public virtual void TestClone_IAtom2()
        {
            var molecule = (IAtomContainer)NewChemObject();
            var carbon = molecule.Builder.NewAtom("C");
            carbon.Point2D = new Vector2(2, 4);
            molecule.Atoms.Add(carbon); // 1

            // test cloning of Atoms
            var clonedMol = (IAtomContainer)molecule.Clone();
            carbon.Point2D = new Vector2(3, 1);
            Assert.AreEqual(clonedMol.Atoms[0].Point2D.Value.X, 2.0, 0.001);
        }

        [TestMethod()]
        public virtual void TestClone_IBond()
        {
            var molecule = (IAtomContainer)NewChemObject();
            molecule.Atoms.Add(molecule.Builder.NewAtom("C")); // 1
            molecule.Atoms.Add(molecule.Builder.NewAtom("C")); // 2
            molecule.Atoms.Add(molecule.Builder.NewAtom("C")); // 3
            molecule.Atoms.Add(molecule.Builder.NewAtom("C")); // 4

            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double); // 1
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single); // 2
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single); // 3
            var clonedMol = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clonedMol);
            Assert.AreEqual(molecule.Bonds.Count, clonedMol.Bonds.Count);
            for (int f = 0; f < molecule.GetElectronContainers().Count(); f++)
            {
                for (int g = 0; g < clonedMol.GetElectronContainers().Count(); g++)
                {
                    Assert.IsNotNull(molecule.Bonds[f]);
                    Assert.IsNotNull(clonedMol.Bonds[g]);
                    Assert.AreNotSame(molecule.Bonds[f], clonedMol.Bonds[g]);
                }
            }
        }

        [TestMethod()]
        public virtual void TestClone_IBond2()
        {
            var molecule = (IAtomContainer)NewChemObject();
            var atom1 = molecule.Builder.NewAtom("C");
            var atom2 = molecule.Builder.NewAtom("C");
            molecule.Atoms.Add(atom1); // 1
            molecule.Atoms.Add(atom2); // 2
            molecule.Bonds.Add(molecule.Builder.NewBond(atom1, atom2, BondOrder.Double)); // 1

            // test cloning of atoms in bonds
            var clonedMol = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clonedMol);
            Assert.AreNotSame(atom1, clonedMol.Bonds[0].Begin);
            Assert.AreNotSame(atom2, clonedMol.Bonds[0].End);
        }

        [TestMethod()]
        public virtual void TestClone_IBond3()
        {
            var molecule = (IAtomContainer)NewChemObject();
            var atom1 = molecule.Builder.NewAtom("C");
            var atom2 = molecule.Builder.NewAtom("C");
            molecule.Atoms.Add(atom1); // 1
            molecule.Atoms.Add(atom2); // 2
            molecule.Bonds.Add(molecule.Builder.NewBond(atom1, atom2, BondOrder.Double)); // 1

            // test that cloned bonds contain atoms from cloned atomcontainer
            var clonedMol = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clonedMol);
            Assert.IsTrue(clonedMol.Contains(clonedMol.Bonds[0].Begin));
            Assert.IsTrue(clonedMol.Contains(clonedMol.Bonds[0].End));
        }

        [TestMethod()]
        public virtual void TestClone_AtomlessIBond()
        {
            var molecule = (IAtomContainer)NewChemObject();
            var bond = molecule.Builder.NewBond();
            molecule.Bonds.Add(bond);
            Assert.AreEqual(bond, molecule.Bonds[0]);
            var clone = (IAtomContainer)molecule.Clone();
            Assert.AreEqual(0, clone.Bonds[0].Atoms.Where(atom => atom != null).Count());
        }

        [TestMethod()]
        public virtual void TestClone_AtomlessILonePair()
        {
            var molecule = (IAtomContainer)NewChemObject();
            var lonePair = molecule.Builder.NewLonePair();
            molecule.LonePairs.Add(lonePair);
            Assert.AreEqual(lonePair, molecule.LonePairs[0]);
            var clone = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clone.LonePairs[0]);
        }

        [TestMethod()]
        public virtual void TestClone_AtomlessISingleElectron()
        {
            var molecule = (IAtomContainer)NewChemObject();
            var singleElectron = molecule.Builder.NewSingleElectron();
            molecule.SingleElectrons.Add(singleElectron);
            Assert.AreEqual(singleElectron, molecule.SingleElectrons[0]);
            var clone = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clone.SingleElectrons[0]);
        }

        [TestMethod()]
        public virtual void TestClone_ILonePair()
        {
            var molecule = (IAtomContainer)NewChemObject();
            var atom1 = molecule.Builder.NewAtom("C");
            var atom2 = molecule.Builder.NewAtom("C");
            molecule.Atoms.Add(atom1); // 1
            molecule.Atoms.Add(atom2); // 2
            molecule.AddLonePairTo(atom1);

            // test that cloned bonds contain atoms from cloned atomcontainer
            var clonedMol = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clonedMol);
            Assert.AreEqual(1, clonedMol.GetConnectedLonePairs(clonedMol.Atoms[0]).Count());
        }

        [TestMethod()]
        public virtual void TestGetConnectedElectronContainersList_IAtom()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();

            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(1, acetone.GetConnectedElectronContainers(o).Count());
            Assert.AreEqual(3, acetone.GetConnectedElectronContainers(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedElectronContainers(c2).Count());
            Assert.AreEqual(1, acetone.GetConnectedElectronContainers(c3).Count());

            // Add lone pairs on oxygen
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(3, acetone.GetConnectedElectronContainers(o).Count());
            Assert.AreEqual(3, acetone.GetConnectedElectronContainers(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedElectronContainers(c2).Count());
            Assert.AreEqual(1, acetone.GetConnectedElectronContainers(c3).Count());
        }

        [TestMethod()]
        public virtual void TestGetConnectedBondsList_IAtom()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();

            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(1, acetone.GetConnectedBonds(o).Count());
            Assert.AreEqual(3, acetone.GetConnectedBonds(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c2).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c3).Count());

            // Add lone pairs on oxygen
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(1, acetone.GetConnectedBonds(o).Count());
            Assert.AreEqual(3, acetone.GetConnectedBonds(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c2).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c3).Count());
        }

        /// <summary>
        /// Unit test to ensure that the stereo elements remain intact on cloning a
        /// container. This test ensures tetrahedral chirality is preserved
        /// </summary>
        // @cdk.bug 1264
        [TestMethod()]
        public virtual void TestClone_IStereoElement_Tetrahedral()
        {

            var container = (IAtomContainer)NewChemObject();

            var builder = container.Builder;

            var c1 = builder.NewAtom("C");
            var o2 = builder.NewAtom("O");
            var n3 = builder.NewAtom("N");
            var c4 = builder.NewAtom("C");
            var h5 = builder.NewAtom("H");

            container.Atoms.Add(c1);
            container.Atoms.Add(o2);
            container.Atoms.Add(n3);
            container.Atoms.Add(c4);
            container.Atoms.Add(h5);

            var c1o2 = builder.NewBond(c1, o2);
            var c1n3 = builder.NewBond(c1, n3);
            var c1c4 = builder.NewBond(c1, c4);
            var c1h5 = builder.NewBond(c1, h5);

            c1o2.Stereo = BondStereo.Up;

            container.Bonds.Add(c1o2);
            container.Bonds.Add(c1n3);
            container.Bonds.Add(c1c4);
            container.Bonds.Add(c1h5);

            var chirality = builder.NewTetrahedralChirality(c1, new IAtom[]{o2, n3, c4, h5}, TetrahedralStereo.Clockwise);

            container.StereoElements.Add(chirality);

            // clone the container
            var clone = (IAtomContainer)container.Clone();

            var elements = clone.StereoElements;

            Assert.IsTrue(elements.Count() > 0, "no stereo elements cloned");

            var element = elements.First();

            Assert.IsInstanceOfType(chirality, element.GetType(), "cloned element was incorrect class");
            Assert.IsTrue(elements.Count() == 1, "too many stereo elements");

            // we've tested the class already  - cast is okay
            var cloneChirality = (ITetrahedralChirality)element;
            var ligands = cloneChirality.Ligands;

            Assert.AreEqual(4, ligands.Count, "not enough ligands");

            // test same instance - reference equality '=='
            Assert.AreSame(ligands[0], clone.Atoms[1], "expected same oxygen instance");
            Assert.AreSame(ligands[1], clone.Atoms[2], "expected same nitrogen instance");
            Assert.AreSame(ligands[2], clone.Atoms[3], "expected same carbon instance");
            Assert.AreSame(ligands[3], clone.Atoms[4], "expected same hydrogen instance");

            Assert.AreEqual(TetrahedralStereo.Clockwise, cloneChirality.Stereo, "incorrect stereo");
            Assert.AreSame(clone.Atoms[0], cloneChirality.ChiralAtom, "incorrect chiral atom");
        }

        /// <summary>
        /// Unit test to ensure that the stereo elements remain intact on cloning a
        /// container. This test ensures DoubleBondStereochemistry is preserved
        /// </summary>
        // @cdk.bug 1264
        [TestMethod()]
        public virtual void TestClone_IStereoElement_DoubleBond()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;

            var c1 = builder.NewAtom("C");
            var c2 = builder.NewAtom("C");
            var c3 = builder.NewAtom("C");
            var c4 = builder.NewAtom("C");

            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            container.Atoms.Add(c3);
            container.Atoms.Add(c4);

            var c1c2 = builder.NewBond(c1, c2, BondOrder.Double);
            var c2c3 = builder.NewBond(c2, c3);
            var c1c4 = builder.NewBond(c1, c4);

            container.Bonds.Add(c1c2);
            container.Bonds.Add(c2c3);
            container.Bonds.Add(c1c4);

            var dbStereo = new DoubleBondStereochemistry(
                c1c2,
                new IBond[] { c2c3, c1c4 },
                DoubleBondConformation.Opposite);

            container.StereoElements.Add(dbStereo);

            // clone the container
            var clone = (IAtomContainer)container.Clone();

            var elements = clone.StereoElements;

            Assert.IsTrue(elements.Count() > 0, "no stereo elements cloned");

            var element = elements.First();

            Assert.IsInstanceOfType(dbStereo, element.GetType(), "cloned element was incorrect class");
            Assert.IsTrue(elements.Count() == 1, "too many stereo elements");

            // we've tested the class already - cast is okay
            var clonedDBStereo = (IDoubleBondStereochemistry)element;
            var ligands = clonedDBStereo.Bonds;

            Assert.AreEqual(2, ligands.Count, "not enough ligands");

            // test same instance - reference equality '=='
            Assert.AreSame(ligands[0], clone.Bonds[1], "expected same c2-c3 instance");
            Assert.AreSame(ligands[1], clone.Bonds[2], "expected same c1-c4 instance");
            Assert.AreEqual(clonedDBStereo.Stereo, DoubleBondConformation.Opposite, "incorrect stereo");
            Assert.AreEqual(clonedDBStereo.StereoBond, clone.Bonds[0], "incorrect chiral atom");
        }

        /// <summary>
        /// Unit test to ensure that the stereo elements remain intact on cloning a
        /// container. This test ensures AtomParity is preserved
        /// </summary>
        // @cdk.bug 1264
        [TestMethod()]
        public virtual void TestClone_IStereoElement_AtomParity()
        {
            var container = (IAtomContainer)NewChemObject();

            var builder = container.Builder;

            var c1 = builder.NewAtom("C");
            var o2 = builder.NewAtom("O");
            var n3 = builder.NewAtom("N");
            var c4 = builder.NewAtom("C");
            var h5 = builder.NewAtom("H");

            container.Atoms.Add(c1);
            container.Atoms.Add(o2);
            container.Atoms.Add(n3);
            container.Atoms.Add(c4);
            container.Atoms.Add(h5);

            var c1o2 = builder.NewBond(c1, o2);
            var c1n3 = builder.NewBond(c1, n3);
            var c1c4 = builder.NewBond(c1, c4);
            var c1h5 = builder.NewBond(c1, h5);

            c1o2.Stereo = BondStereo.Up;

            container.Bonds.Add(c1o2);
            container.Bonds.Add(c1n3);
            container.Bonds.Add(c1c4);
            container.Bonds.Add(c1h5);

            var chirality = builder.NewTetrahedralChirality(c1, new IAtom[]{o2, n3, c4, h5}, TetrahedralStereo.Clockwise);

            container.StereoElements.Add(chirality);

            // clone the container
            var clone = (IAtomContainer)container.Clone();

            var elements = clone.StereoElements;

            Assert.IsTrue(elements.Count() > 0, "no stereo elements cloned");

            var element = elements.First();

            Assert.IsInstanceOfType(chirality, element.GetType(), "cloned element was incorrect class");
            Assert.IsTrue(elements.Count() == 1, "too many stereo elements");

            // we've tested the class already  - cast is okay
            var cloneChirality = (ITetrahedralChirality)element;
            var ligands = cloneChirality.Ligands;

            Assert.AreEqual(4, ligands.Count, "not enough ligands");

            // test same instance - reference equality
            Assert.AreSame(ligands[0], clone.Atoms[1], "expected same oxygen instance");
            Assert.AreSame(ligands[1], clone.Atoms[2], "expected same nitrogen instance");
            Assert.AreSame(ligands[2], clone.Atoms[3], "expected same carbon instance");
            Assert.AreSame(ligands[3], clone.Atoms[4], "expected same hydrogen instance");

            Assert.AreEqual(cloneChirality.Stereo, TetrahedralStereo.Clockwise, "incorrect stereo");
            Assert.AreEqual(cloneChirality.ChiralAtom, clone.Atoms[0], "incorrect chiral atom");
        }

        [TestMethod()]
        public virtual void TestSetStereoElements_List()
        {
            var container = (IAtomContainer)NewChemObject();
            var atom = container.Builder.NewAtom();
            var bond = container.Builder.NewBond();
            var a1 = container.Builder.NewAtom();
            var a2 = container.Builder.NewAtom();
            var a3 = container.Builder.NewAtom();
            var a4 = container.Builder.NewAtom();
            var b1 = container.Builder.NewBond();
            var b2 = container.Builder.NewBond();

            Assert.IsFalse(container.StereoElements.Count > 0);

            var dbElement = new DoubleBondStereochemistry(bond, new IBond[] { b1, b2 }, DoubleBondConformation.Together);
            container.SetStereoElements(new[] { dbElement });
            var first = container.StereoElements.GetEnumerator();
            Assert.IsTrue(first.MoveNext(), "container did not have stereo elements");
            Assert.AreEqual(dbElement, first.Current, "expected element to equal set element (double bond)");
            Assert.IsFalse(first.MoveNext(), "container had more then one stereo element");

            var tetrahedralElement = new TetrahedralChirality(atom, new IAtom[] { a1, a2, a3, a4 }, TetrahedralStereo.Clockwise);
            container.SetStereoElements(new[] { tetrahedralElement });
            var second = container.StereoElements.GetEnumerator();
            Assert.IsTrue(second.MoveNext(), "container did not have stereo elements");
            Assert.AreEqual(tetrahedralElement, second.Current, "expected element to equal set element (tetrahedral)");
            Assert.IsFalse(second.MoveNext(), "container had more then one stereo element");
        }

        [TestMethod()]
        public virtual void TestGetConnectedLonePairsList_IAtom()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();

            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(0, acetone.GetConnectedLonePairs(o).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c1).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c2).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c3).Count());

            // Add lone pairs on oxygen
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(2, acetone.GetConnectedLonePairs(o).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c1).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c2).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c3).Count());
        }

        [TestMethod()]
        public virtual void TestRemoveAtomAndConnectedElectronContainers_IAtom()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();

            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            // Remove the oxygen
            acetone.RemoveAtom(o);
            Assert.AreEqual(3, acetone.Atoms.Count);
            Assert.AreEqual(2, acetone.Bonds.Count);
            Assert.AreEqual(0, acetone.LonePairs.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveAtomAndConnectedElectronContainers_stereoElement()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();

            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            acetone.StereoElements.Add(new TetrahedralChirality(c1, new IAtom[] { c2, o, c3, c1 }, TetrahedralStereo.Clockwise));

            // Remove the oxygen
            acetone.RemoveAtom(o);
            Assert.AreEqual(3, acetone.Atoms.Count);
            Assert.AreEqual(2, acetone.Bonds.Count);
            Assert.AreEqual(0, acetone.LonePairs.Count);
            Assert.IsFalse(acetone.StereoElements.Count > 0);
        }

        [TestMethod()]
        public virtual void TestGetAtomCount()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            Assert.AreEqual(0, acetone.Atoms.Count);

            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            Assert.AreEqual(4, acetone.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestGetBondCount()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            Assert.AreEqual(0, acetone.Bonds.Count);

            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestAdd_IAtomContainer()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            var container = (IAtomContainer)NewChemObject();
            container.Add(acetone);
            Assert.AreEqual(4, container.Atoms.Count);
            Assert.AreEqual(3, container.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestAdd_IAtomContainer_LonePairs()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c1);
            mol.AddLonePairTo(mol.Atoms[0]);

            var container = (IAtomContainer)NewChemObject();
            container.Add(mol);
            Assert.AreEqual(1, container.Atoms.Count);
            Assert.AreEqual(1, container.LonePairs.Count);
        }

        [TestMethod()]
        public virtual void TestAdd_IAtomContainer_SingleElectrons()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            var container = (IAtomContainer)NewChemObject();
            container.Add(mol);
            Assert.AreEqual(1, container.Atoms.Count);
            Assert.AreEqual(1, container.SingleElectrons.Count);
        }

        [TestMethod()]
        public virtual void TestRemove_IAtomContainer()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            var container = (IAtomContainer)NewChemObject();
            container.Add(acetone);
            Assert.AreEqual(4, container.Atoms.Count);
            Assert.AreEqual(3, container.Bonds.Count);
            container.Remove((IAtomContainer)acetone.Clone());
            Assert.AreEqual(4, container.Atoms.Count);
            Assert.AreEqual(3, container.Bonds.Count);
            container.Remove(acetone);
            Assert.AreEqual(0, container.Atoms.Count);
            Assert.AreEqual(0, container.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveAllElements()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            var container = (IAtomContainer)NewChemObject();
            container.Add(acetone);
            Assert.AreEqual(4, container.Atoms.Count);
            Assert.AreEqual(3, container.Bonds.Count);
            container.RemoveAllElements();
            Assert.AreEqual(0, container.Atoms.Count);
            Assert.AreEqual(0, container.Bonds.Count);
        }

        /// <summary>
        /// Unit test ensures that stereo-elements are removed from a container
        /// when <see cref="IAtomContainer.RemoveAllElements()"/> is invoked.
        /// </summary>
        // @cdk.bug 1270
        [TestMethod()]
        public virtual void TestRemoveAllElements_StereoElements()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var focus = builder.NewAtom();
            var a1 = builder.NewAtom();
            var a2 = builder.NewAtom();
            var a3 = builder.NewAtom();
            var a4 = builder.NewAtom();
            container.StereoElements.Add(new TetrahedralChirality(focus, new IAtom[] { a1, a2, a3, a4 }, TetrahedralStereo.Clockwise));

            int count = 0;
            foreach (var element in container.StereoElements)
                count++;

            Assert.AreEqual(1, count, "no stereo elements were added");

            count = 0;
            Assert.AreEqual(0, count, "count did not reset");

            container.RemoveAllElements();

            foreach (IStereoElement<IChemObject, IChemObject> element in container.StereoElements)
            {
                count++;
            }
            Assert.AreEqual(0, count, "stereo elements were not removed");
        }

        [TestMethod()]
        public virtual void TestRemoveAtom_int()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            Assert.AreEqual(4, acetone.Atoms.Count);
            acetone.Atoms.Remove(acetone.Atoms[1]);
            Assert.AreEqual(3, acetone.Atoms.Count);
            Assert.AreEqual(c1, acetone.Atoms[0]);
            Assert.AreEqual(c3, acetone.Atoms[1]);
            Assert.AreEqual(o, acetone.Atoms[2]);
        }

        [TestMethod()]
        public virtual void TestRemoveAtom_IAtom()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            Assert.AreEqual(4, acetone.Atoms.Count);
            acetone.Atoms.Remove(c3);
            Assert.AreEqual(3, acetone.Atoms.Count);
            Assert.AreEqual(c1, acetone.Atoms[0]);
            Assert.AreEqual(c2, acetone.Atoms[1]);
            Assert.AreEqual(o, acetone.Atoms[2]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSetAtomOutOfRange()
        {
            var container = (IAtomContainer)NewChemObject();
            var c = container.Builder.NewAtom("C");
            container.Atoms[0] = c;
        }

        [TestMethod()]
        public void TestSetAtom()
        {
            var container = (IAtomContainer)NewChemObject();
            var c1 = container.Builder.NewAtom("C");
            var c2 = container.Builder.NewAtom("C");
            container.Atoms.Add(c1);
            container.Atoms[0] = c2;
            Assert.AreEqual(c2, container.Atoms[0]);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = true)]
        public void TestSetAtomSameMolecule()
        {
            var container = (IAtomContainer)NewChemObject();
            var c1 = container.Builder.NewAtom("C");
            var c2 = container.Builder.NewAtom("C");
            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            container.Atoms[0] = c2;
        }

        [TestMethod()]
        public void TestSetAtomUpdatesBonds()
        {
            var container = (IAtomContainer)NewChemObject();
            var a1 = container.Builder.NewAtom();
            var a2 = container.Builder.NewAtom();
            var a3 = container.Builder.NewAtom();
            var b1 = container.Builder.NewBond();
            var b2 = container.Builder.NewBond();
            a1.Symbol = "C";
            a2.Symbol = "C";
            a2.Symbol = "O";
            b1.Order = BondOrder.Single;
            b1.SetAtoms(new IAtom[] { a1, a2 });
            b2.Order = BondOrder.Single;
            b2.SetAtoms(new IAtom[] { a2, a3 });
            container.Atoms.Add(a1);
            container.Atoms.Add(a2);
            container.Atoms.Add(a3);
            container.Bonds.Add(b1);
            container.Bonds.Add(b2);

            var a4 = container.Builder.NewAtom();
            container.Atoms[2] = a4;
            Assert.AreEqual(a4, b2.End);
        }

        [TestMethod()]
        public void TestSetAtomUpdatesSingleElectron()
        {
            var container = (IAtomContainer)NewChemObject();
            var bldr = container.Builder;
            var a1 = bldr.NewAtom();
            var a2 = bldr.NewAtom();
            var a3 = bldr.NewAtom();
            var b1 = bldr.NewBond();
            var b2 = bldr.NewBond();
            a1.Symbol = "C";
            a2.Symbol = "C";
            a2.Symbol = "O";
            b1.Order = BondOrder.Single;
            b1.SetAtoms(new IAtom[] { a1, a2 });
            b2.Order = BondOrder.Single;
            b2.SetAtoms(new IAtom[] { a2, a3 });
            container.Atoms.Add(a1);
            container.Atoms.Add(a2);
            container.Atoms.Add(a3);
            container.Bonds.Add(b1);
            container.Bonds.Add(b2);
            var se = bldr.NewSingleElectron();
            se.Atom = a3;
            container.SingleElectrons.Add(se);

            var a4 = bldr.NewAtom();
            container.Atoms[2] = a4;

            Assert.AreEqual(a4, se.Atom);
        }

        [TestMethod()]
        public void TestSetAtomUpdatesAtomStereo()
        {
            var container = (IAtomContainer)NewChemObject();
            var bldr = container.Builder;
            var a1 = bldr.NewAtom();
            var a2 = bldr.NewAtom();
            var a3 = bldr.NewAtom();
            var a4 = bldr.NewAtom();
            var a5 = bldr.NewAtom();
            a1.Symbol = "C";
            a2.Symbol = "O";
            a3.Symbol = "Cl";
            a4.Symbol = "F";
            a5.Symbol = "C";
            container.Atoms.Add(a1);
            container.Atoms.Add(a2);
            container.Atoms.Add(a3);
            container.Atoms.Add(a4);
            container.Atoms.Add(a5);
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[4], BondOrder.Single);
            container.StereoElements.Add(new TetrahedralChirality(container.Atoms[0], container.Atoms.Skip(1).Take(4).ToReadOnlyList(), TetrahedralStereo.Clockwise));

            var aNew = bldr.NewAtom();
            container.Atoms[2] = aNew;

            var siter = container.StereoElements.GetEnumerator();
            Assert.IsTrue(siter.MoveNext());
            var se = siter.Current;
            Assert.IsInstanceOfType(se, typeof(ITetrahedralChirality));
            var tc = (ITetrahedralChirality)se;
            Assert.AreEqual(a1, tc.ChiralAtom);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a2, aNew, a4, a5 }, tc.Ligands));
            Assert.IsFalse(siter.MoveNext());
        }

        [TestMethod()]
        public void TestSetAtomUpdatesBondStereo()
        {
            var container = (IAtomContainer)NewChemObject();
            var bldr = container.Builder;
            var a1 = bldr.NewAtom();
            var a2 = bldr.NewAtom();
            var a3 = bldr.NewAtom();
            var a4 = bldr.NewAtom();
            a1.Symbol = "C";
            a2.Symbol = "C";
            a3.Symbol = "C";
            a4.Symbol = "C";
            container.Atoms.Add(a1);
            container.Atoms.Add(a2);
            container.Atoms.Add(a3);
            container.Atoms.Add(a4);
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Double);
            container.AddBond(container.Atoms[2], container.Atoms[3], BondOrder.Single);
            var b1 = container.Bonds[0];
            var b2 = container.Bonds[1];
            var b3 = container.Bonds[2];

            container.StereoElements.Add(new DoubleBondStereochemistry(b2, new IBond[] { b1, b3 }, DoubleBondConformation.Together));

            var aNew = bldr.NewAtom();
            container.Atoms[2] = aNew;

            Assert.AreEqual(aNew, b2.End);
            Assert.AreEqual(aNew, b3.Begin);

            var siter = container.StereoElements.GetEnumerator();
            Assert.IsTrue(siter.MoveNext());
            var se = siter.Current;
            Assert.IsInstanceOfType(se, typeof(IDoubleBondStereochemistry));
            var tc = (IDoubleBondStereochemistry)se;
            Assert.AreEqual(b2, tc.StereoBond);
            Assert.IsTrue(Compares.AreDeepEqual(new IBond[] { b1, b3 }, tc.Bonds));
            Assert.IsFalse(siter.MoveNext());
        }

        /// <summary>
        /// This test we ensure there is backing array and then access the index,
        /// we should get an exception rather than null
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetAtomOutOfBackedArray()
        {
            var mol = (IAtomContainer)NewChemObject();
            var builder = mol.Builder;
            for (int i = 0; i < 10; i++)
                mol.Atoms.Add(builder.NewAtom());
            for (int i = 9; i >= 0; i--)
                mol.Atoms.RemoveAt(i);
            var dummy = mol.Atoms[0]; // fail rather than return null
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetAtomOutOfRange()
        {
            var mol = (IAtomContainer)NewChemObject();
            var dummy = mol.Atoms[99999];
        }

        /// <summary>
        /// This test we ensure there is backing array and then access the index,
        /// we should get an exception rather than null
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetBondOutOfRangeBackedArray()
        {
            var mol = (IAtomContainer)NewChemObject();
            var builder = mol.Builder;
            for (int i = 0; i < 10; i++)
                mol.Atoms.Add(builder.NewAtom());
            for (int i = 0; i < 9; i++)
                mol.AddBond(mol.Atoms[i], mol.Atoms[i + 1], BondOrder.Single);
            for (int i = 8; i >= 0; i--)
                mol.Bonds.RemoveAt(i);
            var dummy = mol.Bonds[0]; // fail rather than return null
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetBondOutOfRange()
        {
            var mol = (IAtomContainer)NewChemObject();
            var dummy = mol.Atoms[99999];
        }

        [TestMethod()]
        public virtual void TestGetAtom_int()
        {
            var acetone = (IAtomContainer)NewChemObject();

            var c = acetone.Builder.NewAtom("C");
            var n = acetone.Builder.NewAtom("N");
            var o = acetone.Builder.NewAtom("O");
            var s = acetone.Builder.NewAtom("S");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(n);
            acetone.Atoms.Add(o);
            acetone.Atoms.Add(s);

            var a1 = acetone.Atoms[0];
            Assert.IsNotNull(a1);
            Assert.AreEqual("C", a1.Symbol);
            var a2 = acetone.Atoms[1];
            Assert.IsNotNull(a2);
            Assert.AreEqual("N", a2.Symbol);
            var a3 = acetone.Atoms[2];
            Assert.IsNotNull(a3);
            Assert.AreEqual("O", a3.Symbol);
            var a4 = acetone.Atoms[3];
            Assert.IsNotNull(a4);
            Assert.AreEqual("S", a4.Symbol);
        }

        [TestMethod()]
        public virtual void TestGetBond_int()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            Assert.AreEqual(0, acetone.Bonds.Count);

            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Triple);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(BondOrder.Triple, acetone.Bonds[0].Order);
            Assert.AreEqual(BondOrder.Double, acetone.Bonds[1].Order);
            Assert.AreEqual(BondOrder.Single, acetone.Bonds[2].Order);
        }

        [TestMethod()]
        public virtual void TestGetElectronContainerCount()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(3, acetone.Bonds.Count);
            Assert.AreEqual(2, acetone.LonePairs.Count);
            Assert.AreEqual(5, acetone.GetElectronContainers().Count());
        }

        [TestMethod()]
        public virtual void TestRemoveAllBonds()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.Bonds.Count);

            acetone.Bonds.Clear();
            Assert.AreEqual(0, acetone.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveAllElectronContainers()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.GetElectronContainers().Count());

            acetone.RemoveAllElectronContainers();
            Assert.AreEqual(0, acetone.GetElectronContainers().Count());
        }

        [TestMethod()]
        public virtual void TestAddAtom_IAtom()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            IEnumerator<IAtom> atomIter = acetone.Atoms.GetEnumerator();
            int counter = 0;
            while (atomIter.MoveNext())
            {
                counter++;
            }
            Assert.AreEqual(4, counter);

            // test force growing of default arrays
            for (int i = 0; i < 500; i++)
            {
                acetone.Atoms.Add(acetone.Builder.NewAtom());
                acetone.Bonds.Add(acetone.Builder.NewBond());
            }
        }

        [TestMethod()]
        public virtual void TestAtoms()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            var atomIter = acetone.Atoms.GetEnumerator();
            Assert.IsNotNull(atomIter);
            Assert.IsTrue(atomIter.MoveNext());
            var next = atomIter.Current;
            Assert.IsTrue(next is IAtom);
            Assert.AreEqual(c1, next);
            Assert.IsTrue(atomIter.MoveNext());
            next = atomIter.Current;
            Assert.IsTrue(next is IAtom);
            Assert.AreEqual(c2, next);
            Assert.IsTrue(atomIter.MoveNext());
            next = atomIter.Current;
            Assert.IsTrue(next is IAtom);
            Assert.AreEqual(c3, next);
            Assert.IsTrue(atomIter.MoveNext());
            next = atomIter.Current;
            Assert.IsTrue(next is IAtom);
            Assert.AreEqual(o, next);

            Assert.IsFalse(atomIter.MoveNext());
        }

        [TestMethod()]
        public virtual void TestBonds()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            var bond1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var bond2 = acetone.Builder.NewBond(c2, o, BondOrder.Double);
            var bond3 = acetone.Builder.NewBond(c2, c3, BondOrder.Single);
            acetone.Bonds.Add(bond1);
            acetone.Bonds.Add(bond2);
            acetone.Bonds.Add(bond3);

            var bonds = acetone.Bonds.GetEnumerator();
            Assert.IsNotNull(bonds);
            Assert.IsTrue(bonds.MoveNext());

            var next = (IBond)bonds.Current;
            Assert.IsTrue(next is IBond);
            Assert.AreEqual(bond1, next);
            bonds.MoveNext();
            next = (IBond)bonds.Current;
            Assert.IsTrue(next is IBond);
            Assert.AreEqual(bond2, next);
            bonds.MoveNext();
            next = (IBond)bonds.Current;
            Assert.IsTrue(next is IBond);
            Assert.AreEqual(bond3, next);

            Assert.IsFalse(bonds.MoveNext());
        }

        [TestMethod()]
        public virtual void TestLonePairs()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            var bond1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var bond2 = acetone.Builder.NewBond(c2, o, BondOrder.Double);
            var bond3 = acetone.Builder.NewBond(c2, c3, BondOrder.Single);
            acetone.Bonds.Add(bond1);
            acetone.Bonds.Add(bond2);
            acetone.Bonds.Add(bond3);
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            var lonePairs = acetone.LonePairs.GetEnumerator();
            Assert.IsNotNull(lonePairs);
            Assert.IsTrue(lonePairs.MoveNext());

            var next = lonePairs.Current;
            Assert.IsTrue(next is ILonePair);
            Assert.AreEqual(lp1, next);
            lonePairs.MoveNext();
            next = lonePairs.Current;
            Assert.IsTrue(next is ILonePair);
            Assert.AreEqual(lp2, next);

            Assert.IsFalse(lonePairs.MoveNext());
        }

        [TestMethod()]
        public virtual void TestSingleElectrons()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            var bond1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var bond2 = acetone.Builder.NewBond(c2, o, BondOrder.Double);
            var bond3 = acetone.Builder.NewBond(c2, c3, BondOrder.Single);
            acetone.Bonds.Add(bond1);
            acetone.Bonds.Add(bond2);
            acetone.Bonds.Add(bond3);
            var se1 = acetone.Builder.NewSingleElectron(o);
            var se2 = acetone.Builder.NewSingleElectron(c1);
            acetone.SingleElectrons.Add(se1);
            acetone.SingleElectrons.Add(se2);

            var singleElectrons = acetone.SingleElectrons;
            Assert.IsNotNull(singleElectrons);
            Assert.IsTrue(singleElectrons.Count > 0);

            var next = (ISingleElectron)singleElectrons[0];
            Assert.IsTrue(next is ISingleElectron);
            Assert.AreEqual(se1, next);

            next = (ISingleElectron)singleElectrons[1];
            Assert.IsTrue(next is ISingleElectron);
            Assert.AreEqual(se2, next);

            Assert.IsTrue(singleElectrons.Count == 2);
        }

        [TestMethod()]
        public virtual void TestElectronContainers()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            var bond1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var bond2 = acetone.Builder.NewBond(c2, o, BondOrder.Double);
            var bond3 = acetone.Builder.NewBond(c2, c3, BondOrder.Single);
            acetone.Bonds.Add(bond1);
            acetone.Bonds.Add(bond2);
            acetone.Bonds.Add(bond3);
            var se1 = acetone.Builder.NewSingleElectron(c1);
            var se2 = acetone.Builder.NewSingleElectron(c2);
            acetone.SingleElectrons.Add(se1);
            acetone.SingleElectrons.Add(se2);
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            var electronContainers = acetone.GetElectronContainers()?.ToReadOnlyList();
            Assert.IsNotNull(electronContainers);
            Assert.IsTrue(electronContainers.Count > 0);
            var ec = (IElectronContainer)electronContainers[2];
            Assert.IsTrue(ec is IBond);
            Assert.AreEqual(bond3, ec);
            var lp = (ILonePair)electronContainers[4];
            Assert.IsTrue(lp is ILonePair);
            Assert.AreEqual(lp2, lp);
            var se = (ISingleElectron)electronContainers[5];
            Assert.IsTrue(se is ISingleElectron);
            Assert.AreEqual(se1, se);
            se = (ISingleElectron)electronContainers[6];
            Assert.IsTrue(se is ISingleElectron);
            Assert.AreEqual(se2, se);

            Assert.IsTrue(electronContainers.Count == 7);
        }

        [TestMethod()]
        public virtual void TestContains_IAtom()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            Assert.IsTrue(acetone.Contains(c1));
            Assert.IsTrue(acetone.Contains(c2));
            Assert.IsTrue(acetone.Contains(o));
            Assert.IsTrue(acetone.Contains(c3));
        }

        [TestMethod()]
        public virtual void TestAddLonePair_int()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.Bonds.Count);
            Assert.AreEqual(5, acetone.GetElectronContainers().Count());
        }

        [TestMethod()]
        public virtual void TestGetMaximumBondOrder_IAtom()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(BondOrder.Double, acetone.GetMaximumBondOrder(o));
            Assert.AreEqual(BondOrder.Double, acetone.GetMaximumBondOrder(c1));
            Assert.AreEqual(BondOrder.Single, acetone.GetMaximumBondOrder(c2));
            Assert.AreEqual(BondOrder.Single, acetone.GetMaximumBondOrder(c3));
        }

        [TestMethod()]
        public virtual void TestGetMinimumBondOrder_IAtom()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(BondOrder.Double, acetone.GetMinimumBondOrder(o));
            Assert.AreEqual(BondOrder.Single, acetone.GetMinimumBondOrder(c1));
            Assert.AreEqual(BondOrder.Single, acetone.GetMinimumBondOrder(c2));
            Assert.AreEqual(BondOrder.Single, acetone.GetMinimumBondOrder(c3));
        }

        [TestMethod()]
        public void TestGetMinBondOrderHighBondOrder()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            container.Atoms.Add(builder.NewAtom());
            container.Atoms.Add(builder.NewAtom());
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Sextuple);
            Assert.AreEqual(BondOrder.Sextuple, container.GetMinimumBondOrder(container.Atoms[0]));
        }

        [TestMethod()]
        public void TestGetMinBondOrderNoBonds()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var atom = builder.NewAtom();
            container.Atoms.Add(atom);
            Assert.AreEqual(BondOrder.Unset, container.GetMinimumBondOrder(atom));
        }

        [TestMethod()]
        public void TestGetMinBondOrderImplH()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var a = builder.NewAtom();
            a.ImplicitHydrogenCount = 1;
            container.Atoms.Add(a);
            Assert.AreEqual(BondOrder.Single, container.GetMinimumBondOrder(a));
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetMinBondOrderNoSuchAtom()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var a1 = builder.NewAtom();
            var a2 = builder.NewAtom();
            container.Atoms.Add(a1);
            Assert.AreEqual(BondOrder.Unset, container.GetMinimumBondOrder(a2));
        }

        [TestMethod()]
        public void TestGetMaxBondOrderHighBondOrder()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            container.Atoms.Add(builder.NewAtom());
            container.Atoms.Add(builder.NewAtom());
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Sextuple);
            Assert.AreEqual(BondOrder.Sextuple, container.GetMaximumBondOrder(container.Atoms[0]));
        }

        [TestMethod()]
        public void TestGetMaxBondOrderNoBonds()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var atom = builder.NewAtom();
            container.Atoms.Add(atom);
            Assert.AreEqual(BondOrder.Unset, container.GetMaximumBondOrder(atom));
        }

        [TestMethod()]
        public void TestGetMaxBondOrderImplH()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var a = builder.NewAtom();
            a.ImplicitHydrogenCount = 1;
            container.Atoms.Add(a);
            Assert.AreEqual(BondOrder.Single, container.GetMaximumBondOrder(a));
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetMaxBondOrderNoSuchAtom()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var a1 = builder.NewAtom();
            var a2 = builder.NewAtom();
            container.Atoms.Add(a1);
            Assert.AreEqual(BondOrder.Unset, container.GetMaximumBondOrder(a2));
        }

        [TestMethod()]
        public virtual void TestRemoveElectronContainer_int()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.Bonds.Count);
            Assert.AreEqual(5, acetone.GetElectronContainers().Count());
            acetone.Remove(acetone.GetElectronContainers().Skip(3).First());

            Assert.AreEqual(3, acetone.Bonds.Count);
            Assert.AreEqual(4, acetone.GetElectronContainers().Count());
            acetone.Remove(acetone.GetElectronContainers().First()); // first bond now
            Assert.AreEqual(2, acetone.Bonds.Count);
            Assert.AreEqual(3, acetone.GetElectronContainers().Count());
        }

        [TestMethod()]
        public virtual void TestRemoveElectronContainer_IElectronContainer()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var firstLP = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(firstLP);
            acetone.LonePairs.Add(acetone.Builder.NewLonePair(o));
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.Bonds.Count);
            Assert.AreEqual(5, acetone.GetElectronContainers().Count());
            acetone.LonePairs.Remove(firstLP);
            Assert.AreEqual(3, acetone.Bonds.Count);
            Assert.AreEqual(4, acetone.GetElectronContainers().Count());
            acetone.Bonds.Remove(b1); // first bond now
            Assert.AreEqual(2, acetone.Bonds.Count);
            Assert.AreEqual(3, acetone.GetElectronContainers().Count());
        }

        [TestMethod()]
        public virtual void TestAddBond_IBond()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.Bonds.Count);
            IEnumerable<IBond> bonds = acetone.Bonds;
            foreach (var bond in bonds)
                Assert.IsNotNull(bond);
            Assert.AreEqual(b1, acetone.Bonds[0]);
            Assert.AreEqual(b2, acetone.Bonds[1]);
            Assert.AreEqual(b3, acetone.Bonds[2]);
        }
 
        [TestMethod()]
        public virtual void TestAddElectronContainer_IElectronContainer()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            acetone.LonePairs.Add(acetone.Builder.NewLonePair(o));
            acetone.SingleElectrons.Add(acetone.Builder.NewSingleElectron(c));

            Assert.AreEqual(3, acetone.GetElectronContainers().Count());
            Assert.AreEqual(1, acetone.Bonds.Count);
            Assert.AreEqual(1, acetone.LonePairs.Count);
        }

        [TestMethod()]
        public virtual void TestGetSingleElectron_IAtom()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            acetone.LonePairs.Add(acetone.Builder.NewLonePair(o));
            var single = acetone.Builder.NewSingleElectron(c);
            acetone.SingleElectrons.Add(single);

            Assert.AreEqual(1, acetone.GetConnectedSingleElectrons(c).Count());
            Assert.AreEqual(single, (ISingleElectron)acetone.GetConnectedSingleElectrons(c).First());
        }

        [TestMethod()]
        public virtual void TestRemoveBond_IAtom_IAtom()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.Bonds.Count);
            acetone.Bonds.Remove(acetone.GetBond(c1, o));
            Assert.AreEqual(2, acetone.Bonds.Count);
            Assert.AreEqual(b1, acetone.Bonds[0]);
            Assert.AreEqual(b3, acetone.Bonds[1]);
        }

        [TestMethod()]
        public virtual void TestAddBond_int_int_BondOrder()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            acetone.AddBond(acetone.Atoms[0], acetone.Atoms[1], BondOrder.Single);
            acetone.AddBond(acetone.Atoms[1], acetone.Atoms[3], BondOrder.Double);
            acetone.AddBond(acetone.Atoms[1], acetone.Atoms[2], BondOrder.Single);

            Assert.AreEqual(3, acetone.Bonds.Count);
            foreach (var bond in acetone.Bonds)
                Assert.IsNotNull(bond);

            Assert.AreEqual(c1, acetone.Bonds[0].Begin);
            Assert.AreEqual(c2, acetone.Bonds[0].End);
            Assert.AreEqual(BondOrder.Single, acetone.Bonds[0].Order);
            Assert.AreEqual(c2, acetone.Bonds[1].Begin);
            Assert.AreEqual(o, acetone.Bonds[1].End);
            Assert.AreEqual(BondOrder.Double, acetone.Bonds[1].Order);
            Assert.AreEqual(c2, acetone.Bonds[2].Begin);
            Assert.AreEqual(c3, acetone.Bonds[2].End);
            Assert.AreEqual(BondOrder.Single, acetone.Bonds[2].Order);
        }

        [TestMethod()]
        public virtual void TestAddBond_int_int_BondOrder_IBond_Stereo()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            acetone.AddBond(acetone.Atoms[0], acetone.Atoms[1], BondOrder.Single, BondStereo.Up); // yes this is crap
            acetone.AddBond(acetone.Atoms[1], acetone.Atoms[3], BondOrder.Double, BondStereo.Down);
            acetone.AddBond(acetone.Atoms[1], acetone.Atoms[2], BondOrder.Single, BondStereo.None);

            Assert.AreEqual(3, acetone.Bonds.Count);
            foreach (var bond in acetone.Bonds)
                Assert.IsNotNull(bond);

            Assert.AreEqual(c1, acetone.Bonds[0].Begin);
            Assert.AreEqual(c2, acetone.Bonds[0].End);
            Assert.AreEqual(BondOrder.Single, acetone.Bonds[0].Order);
            Assert.AreEqual(BondStereo.Up, acetone.Bonds[0].Stereo);
            Assert.AreEqual(c2, acetone.Bonds[1].Begin);
            Assert.AreEqual(o, acetone.Bonds[1].End);
            Assert.AreEqual(BondOrder.Double, acetone.Bonds[1].Order);
            Assert.AreEqual(BondStereo.Down, acetone.Bonds[1].Stereo);
            Assert.AreEqual(c2, acetone.Bonds[2].Begin);
            Assert.AreEqual(c3, acetone.Bonds[2].End);
            Assert.AreEqual(BondOrder.Single, acetone.Bonds[2].Order);
            Assert.AreEqual(BondStereo.None, acetone.Bonds[2].Stereo);
        }

        [TestMethod()]
        public virtual void TestContains_IElectronContainer()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.IsTrue(acetone.Contains(b1));
            Assert.IsTrue(acetone.Contains(b2));
            Assert.IsTrue(acetone.Contains(b3));
            Assert.IsTrue(acetone.Contains(lp1));
            Assert.IsTrue(acetone.Contains(lp2));
        }

        [TestMethod()]
        public virtual void TestGetFirstAtom()
        {
            var container = (IAtomContainer)NewChemObject();
            var c1 = container.Builder.NewAtom("C");
            var c2 = container.Builder.NewAtom("O");
            var o = container.Builder.NewAtom("H");
            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            container.Atoms.Add(o);

            Assert.IsNotNull(container.Atoms[0]);
            Assert.AreEqual("C", container.Atoms[0].Symbol);
        }

        [TestMethod()]
        public virtual void TestGetLastAtom()
        {
            var container = (IAtomContainer)NewChemObject();
            var c1 = container.Builder.NewAtom("C");
            var c2 = container.Builder.NewAtom("O");
            var o = container.Builder.NewAtom("H");
            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            container.Atoms.Add(o);

            Assert.IsNotNull(container.Atoms[container.Atoms.Count - 1]);
            Assert.AreEqual("H", container.Atoms[container.Atoms.Count - 1].Symbol);
        }

        [TestMethod()]
        public virtual void TestGetAtomNumber_IAtom()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            Assert.AreEqual(0, acetone.Atoms.IndexOf(c1));
            Assert.AreEqual(1, acetone.Atoms.IndexOf(c2));
            Assert.AreEqual(2, acetone.Atoms.IndexOf(c3));
            Assert.AreEqual(3, acetone.Atoms.IndexOf(o));
        }

        [TestMethod()]
        public virtual void TestGetBondNumber_IBond()
        {
            // acetone molecule
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(0, acetone.Bonds.IndexOf(b1));
            Assert.AreEqual(1, acetone.Bonds.IndexOf(b2));
            Assert.AreEqual(2, acetone.Bonds.IndexOf(b3));

            // test the default return value
            Assert.AreEqual(-1, acetone.Bonds.IndexOf(acetone.Builder.NewBond()));
        }

        [TestMethod()]
        public virtual void TestGetBondNumber_IAtom_IAtom()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(0, acetone.Bonds.IndexOf(acetone.GetBond(c1, c2)));
            Assert.AreEqual(1, acetone.Bonds.IndexOf(acetone.GetBond(c1, o)));
            Assert.AreEqual(2, acetone.Bonds.IndexOf(acetone.GetBond(c1, c3)));
        }

        [TestMethod()]
        public virtual void TestGetBond_IAtom_IAtom()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(acetone.GetBond(c1, c2), b1);
            Assert.AreEqual(acetone.GetBond(c1, o), b2);
            Assert.AreEqual(acetone.GetBond(c1, c3), b3);

            // test the default return value
            Assert.IsNull(acetone.GetBond(acetone.Builder.NewAtom(), acetone.Builder.NewAtom()));
        }

        [TestMethod()]
        public virtual void TestGetConnectedAtomsList_IAtom()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.GetConnectedAtoms(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedAtoms(c2).Count());
            Assert.AreEqual(1, acetone.GetConnectedAtoms(c3).Count());
            Assert.AreEqual(1, acetone.GetConnectedAtoms(o).Count());
        }

        [TestMethod()]
        public virtual void TestGetConnectedAtomsCount_IAtom()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.GetConnectedAtoms(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedAtoms(c2).Count());
            Assert.AreEqual(1, acetone.GetConnectedAtoms(c3).Count());
            Assert.AreEqual(1, acetone.GetConnectedAtoms(o).Count());
        }

        [TestMethod()]
        public virtual void TestGetLonePairCount()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(2, acetone.LonePairs.Count);
        }

        [TestMethod()]
        public virtual void TestGetConnectedLonePairsCount_IAtom()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(2, acetone.GetConnectedLonePairs(o).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c2).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c3).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c1).Count());
        }

        [TestMethod()]
        public virtual void TestGetBondOrderSum_IAtom()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(4.0, acetone.GetBondOrderSum(c1), 0.00001);
            Assert.AreEqual(1.0, acetone.GetBondOrderSum(c2), 0.00001);
            Assert.AreEqual(1.0, acetone.GetBondOrderSum(c3), 0.00001);
            Assert.AreEqual(2.0, acetone.GetBondOrderSum(o), 0.00001);
        }

        [TestMethod()]
        public virtual void TestGetBondCount_IAtom()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(3, acetone.GetConnectedBonds(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c2).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c3).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(o).Count());
        }

        [TestMethod()]
        public virtual void TestGetBondCount_int()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            var c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            var b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            var b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(3, acetone.GetConnectedBonds(acetone.Atoms[0]).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(acetone.Atoms[1]).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(acetone.Atoms[2]).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(acetone.Atoms[3]).Count());
        }

        [TestMethod()]
        public virtual void TestStereoElements()
        {
            var container = (IAtomContainer)NewChemObject();
            var carbon = container.Builder.NewAtom("C");
            carbon.Id = "central";
            var carbon1 = container.Builder.NewAtom("C");
            carbon1.Id = "c1";
            var carbon2 = container.Builder.NewAtom("C");
            carbon2.Id = "c2";
            var carbon3 = container.Builder.NewAtom("C");
            carbon3.Id = "c3";
            var carbon4 = container.Builder.NewAtom("C");
            carbon4.Id = "c4";
            var stereoElement = container.Builder.NewTetrahedralChirality(carbon,
                            new IAtom[] { carbon1, carbon2, carbon3, carbon4 }, TetrahedralStereo.Clockwise);
            container.StereoElements.Add(stereoElement);

            var stereoElements = container.StereoElements.GetEnumerator();
            Assert.IsTrue(stereoElements.MoveNext());
            var element = stereoElements.Current;
            Assert.IsNotNull(element);
            Assert.IsTrue(element is ITetrahedralChirality);
            Assert.AreEqual(carbon, ((ITetrahedralChirality)element).ChiralAtom);
            Assert.IsFalse(stereoElements.MoveNext());
        }

        /// <summary>Test for RFC #9</summary>
        [TestMethod()]
        public virtual void TestToString()
        {
            var container = (IAtomContainer)NewChemObject();
            string description = container.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]
        public override void TestStateChanged_IChemObjectChangeEvent()
        {
            var listener = new ChemObjectListenerImpl();
            var chemObject = (IAtomContainer)NewChemObject();
            chemObject.Listeners.Add(listener);

            var builder = chemObject.Builder;
            chemObject.Atoms.Add(builder.NewAtom());
            Assert.IsTrue(listener.Changed);

            listener.Reset();
            Assert.IsFalse(listener.Changed);
            chemObject.Atoms.Add(builder.NewAtom());
            chemObject.Atoms.Add(builder.NewAtom());
            chemObject.Bonds.Add(builder.NewBond(chemObject.Atoms[0], chemObject.Atoms[1]));
            Assert.IsTrue(listener.Changed);
        }

        class ChemObjectListenerImpl : IChemObjectListener
        {
            public bool Changed { get; private set; }

            public ChemObjectListenerImpl()
            {
                Changed = false;
            }

            public void OnStateChanged(ChemObjectChangeEventArgs evt)
            {
                Changed = true;
            }

            public virtual void Reset()
            {
                Changed = false;
            }
        }

        [TestMethod()]
        public virtual void TestAddStereoElement_IStereoElement()
        {
            TestStereoElements();
        }

        [TestMethod()]
        public virtual void TestGetConnectedSingleElectronsCount_IAtom()
        {
            // another rather artifial example
            var acetone = (IAtomContainer)NewChemObject();
            var c = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            var single1 = acetone.Builder.NewSingleElectron(c);
            var single2 = acetone.Builder.NewSingleElectron(c);
            var single3 = acetone.Builder.NewSingleElectron(o);
            acetone.SingleElectrons.Add(single1);
            acetone.SingleElectrons.Add(single2);
            acetone.SingleElectrons.Add(single3);

            Assert.AreEqual(2, acetone.GetConnectedSingleElectrons(c).Count());
            Assert.AreEqual(1, acetone.GetConnectedSingleElectrons(o).Count());
            Assert.AreEqual(single1, (ISingleElectron)acetone.GetConnectedSingleElectrons(c).ElementAt(0));
            Assert.AreEqual(single2, (ISingleElectron)acetone.GetConnectedSingleElectrons(c).ElementAt(1));
            Assert.AreEqual(single3, (ISingleElectron)acetone.GetConnectedSingleElectrons(o).ElementAt(0));

            Assert.AreEqual(2, acetone.GetConnectedSingleElectrons(c).Count());
            Assert.AreEqual(1, acetone.GetConnectedSingleElectrons(o).Count());
        }

        [TestMethod()]
        public virtual void TestAddLonePair_ILonePair()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            var lp1 = acetone.Builder.NewLonePair(o);
            var lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);
            Assert.AreEqual(2, acetone.GetConnectedLonePairs(o).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c).Count());
        }

        [TestMethod()]
        public virtual void TestAddSingleElectron_ISingleElectron()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            var single1 = acetone.Builder.NewSingleElectron(c);
            var single2 = acetone.Builder.NewSingleElectron(c);
            var single3 = acetone.Builder.NewSingleElectron(o);
            acetone.SingleElectrons.Add(single1);
            acetone.SingleElectrons.Add(single2);
            acetone.SingleElectrons.Add(single3);
            Assert.AreEqual(single1, (ISingleElectron)acetone.GetConnectedSingleElectrons(c).ElementAt(0));
            Assert.AreEqual(single2, (ISingleElectron)acetone.GetConnectedSingleElectrons(c).ElementAt(1));
            Assert.AreEqual(single3, (ISingleElectron)acetone.GetConnectedSingleElectrons(o).ElementAt(0));
        }

        [TestMethod()]
        public virtual void TestRemoveBond_int()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c = acetone.Builder.NewAtom("C");
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            var b = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b);
            acetone.Atoms.Add(c1);
            var b1 = acetone.Builder.NewBond(c, c1, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Atoms.Add(c2);
            var b2 = acetone.Builder.NewBond(c, c2, BondOrder.Single);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Remove(acetone.Bonds[2]);
            Assert.AreEqual(2, acetone.Bonds.Count);
            Assert.AreEqual(b, acetone.Bonds[0]);
            Assert.AreEqual(b1, acetone.Bonds[1]);
            acetone.Bonds.Remove(acetone.Bonds[0]);
            Assert.AreEqual(1, acetone.Bonds.Count);
            Assert.AreEqual(b1, acetone.Bonds[0]);
        }

        [TestMethod()]
        public virtual void TestContains_IBond()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            var b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            var falseBond = acetone.Builder.NewBond();
            Assert.IsTrue(acetone.Contains(b1));
            Assert.IsFalse(acetone.Contains(falseBond));
        }

        [TestMethod()]
        public virtual void TestAddSingleElectron_int()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            Assert.AreEqual(2, mol.SingleElectrons.Count);
            Assert.IsNotNull(mol.SingleElectrons[1]);
            var singles = mol.SingleElectrons.ToReadOnlyList();
            var singleElectron = singles[0];
            Assert.IsNotNull(singleElectron);
            Assert.AreEqual(c1, singleElectron.Atom);
            Assert.IsTrue(singleElectron.Contains(c1));
            singleElectron = singles[1];
            Assert.IsNotNull(singleElectron);
            Assert.AreEqual(c1, singleElectron.Atom);
            Assert.IsTrue(singleElectron.Contains(c1));
        }

        [TestMethod()]
        public virtual void TestGetConnectedSingleElectronsList_IAtom()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            var list = mol.GetConnectedSingleElectrons(c1);
            Assert.AreEqual(2, list.Count());
        }

        [TestMethod()]
        public virtual void TestRemoveBond_IBond()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            Assert.AreEqual(1, mol.Bonds.Count);
            var bond = mol.Bonds[0];
            mol.Bonds.Remove(bond);
            Assert.AreEqual(0, mol.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestGetConnectedBondsCount_IAtom()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c = acetone.Builder.NewAtom("C");
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            var b = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b);
            acetone.Atoms.Add(c1);
            var b1 = acetone.Builder.NewBond(c, c1, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Atoms.Add(c2);
            var b2 = acetone.Builder.NewBond(c, c2, BondOrder.Single);
            acetone.Bonds.Add(b2);
            Assert.AreEqual(1, acetone.GetConnectedBonds(o).Count());
            Assert.AreEqual(3, acetone.GetConnectedBonds(c).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c2).Count());
        }

        [TestMethod()]
        public virtual void TestGetConnectedBondsCount_int()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c = acetone.Builder.NewAtom("C");
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            var b = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b);
            acetone.Atoms.Add(c1);
            var b1 = acetone.Builder.NewBond(c, c1, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Atoms.Add(c2);
            var b2 = acetone.Builder.NewBond(c, c2, BondOrder.Single);
            acetone.Bonds.Add(b2);
            Assert.AreEqual(1, acetone.GetConnectedBonds(acetone.Atoms[1]).Count());
            Assert.AreEqual(3, acetone.GetConnectedBonds(acetone.Atoms[0]).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(acetone.Atoms[2]).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(acetone.Atoms[3]).Count());
        }

        [TestMethod()]
        public virtual void TestGetLonePair_int()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddLonePairTo(mol.Atoms[1]);
            var lp = mol.Builder.NewLonePair(c);
            mol.LonePairs.Add(lp);
            Assert.AreEqual(lp, mol.LonePairs[1]);
        }

        [TestMethod()]
        public virtual void TestGetSingleElectron_int()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            var se = mol.Builder.NewSingleElectron(c);
            mol.SingleElectrons.Add(se);
            Assert.AreEqual(se, mol.SingleElectrons[1]);
        }

        [TestMethod()]
        public virtual void TestGetLonePairNumber_ILonePair()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddLonePairTo(mol.Atoms[1]);
            var lp = mol.Builder.NewLonePair(c);
            mol.LonePairs.Add(lp);
            Assert.AreEqual(1, mol.LonePairs.IndexOf(lp));
        }

        [TestMethod()]
        public virtual void TestGetSingleElectronNumber_ISingleElectron()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            var se = mol.Builder.NewSingleElectron(c);
            mol.SingleElectrons.Add(se);
            Assert.AreEqual(1, mol.SingleElectrons.IndexOf(se));
        }

        [TestMethod()]
        public virtual void TestGetElectronContainer_int()
        {
            var acetone = (IAtomContainer)NewChemObject();
            var c = acetone.Builder.NewAtom("C");
            var c1 = acetone.Builder.NewAtom("C");
            var c2 = acetone.Builder.NewAtom("C");
            var o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            var b = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b);
            acetone.Atoms.Add(c1);
            var b1 = acetone.Builder.NewBond(c, c1, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Atoms.Add(c2);
            var b2 = acetone.Builder.NewBond(c, c2, BondOrder.Single);
            acetone.Bonds.Add(b2);
            acetone.AddLonePairTo(acetone.Atoms[1]);
            acetone.AddLonePairTo(acetone.Atoms[1]);
            Assert.IsTrue(acetone.GetElectronContainers().ElementAt(2) is IBond);
            Assert.IsTrue(acetone.GetElectronContainers().ElementAt(4) is ILonePair);
        }

        [TestMethod()]
        public virtual void TestGetSingleElectronCount()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            Assert.AreEqual(2, mol.SingleElectrons.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveLonePair_int()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddLonePairTo(mol.Atoms[1]);
            var lp = mol.Builder.NewLonePair(c);
            mol.LonePairs.Add(lp);
            mol.LonePairs.Remove(mol.LonePairs[0]);
            Assert.AreEqual(1, mol.LonePairs.Count);
            Assert.AreEqual(lp, mol.LonePairs[0]);
        }

        [TestMethod()]
        public virtual void TestRemoveLonePair_ILonePair()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            var lp = mol.Builder.NewLonePair(c1);
            mol.LonePairs.Add(lp);
            var lp1 = mol.Builder.NewLonePair(c);
            mol.LonePairs.Add(lp1);
            mol.LonePairs.Remove(lp);
            Assert.AreEqual(1, mol.LonePairs.Count);
            Assert.AreEqual(lp1, mol.LonePairs[0]);
        }

        [TestMethod()]
        public virtual void TestRemoveSingleElectron_int()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            var se = mol.Builder.NewSingleElectron(c);
            mol.SingleElectrons.Add(se);
            mol.SingleElectrons.Remove(mol.SingleElectrons[0]);
            Assert.AreEqual(1, mol.SingleElectrons.Count);
            Assert.AreEqual(se, mol.SingleElectrons[0]);
        }

        [TestMethod()]
        public virtual void TestRemoveSingleElectron_ISingleElectron()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            var se1 = mol.Builder.NewSingleElectron(c1);
            mol.SingleElectrons.Add(se1);
            var se = mol.Builder.NewSingleElectron(c);
            mol.SingleElectrons.Add(se);
            Assert.AreEqual(2, mol.SingleElectrons.Count);
            mol.SingleElectrons.Remove(se);
            Assert.AreEqual(1, mol.SingleElectrons.Count);
            Assert.AreEqual(se1, mol.SingleElectrons[0]);
        }

        [TestMethod()]
        public virtual void TestContains_ILonePair()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            var lp = mol.Builder.NewLonePair(c1);
            mol.LonePairs.Add(lp);
            var lp1 = mol.Builder.NewLonePair(c);
            Assert.IsTrue(mol.Contains(lp));
            Assert.IsFalse(mol.Contains(lp1));
        }

        [TestMethod()]
        public virtual void TestContains_ISingleElectron()
        {
            var mol = (IAtomContainer)NewChemObject();
            var c = mol.Builder.NewAtom("C");
            var c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            var se = mol.Builder.NewSingleElectron(c1);
            mol.SingleElectrons.Add(se);
            var se1 = mol.Builder.NewSingleElectron(c1);
            Assert.IsTrue(mol.Contains(se));
            Assert.IsFalse(mol.Contains(se1));
        }

        [TestMethod()]
        public virtual void TestIsEmpty()
        {
            var container = (IAtomContainer)NewChemObject();
            Assert.IsTrue(container.IsEmpty(), "new atom container was not empty");
            var c1 = container.Builder.NewAtom("C");
            var c2 = container.Builder.NewAtom("C");
            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            Assert.IsFalse(container.IsEmpty(), "atom container Contains 2 atoms but was empty");
            container.Bonds.Add(container.Builder.NewBond(c1, c2));
            Assert.IsFalse(container.IsEmpty(), "atom container Contains 2 atoms and 1 bond but was empty");
            container.Atoms.Remove(c1);
            container.Atoms.Remove(c2);
            Assert.AreEqual(1, container.Bonds.Count, "atom contains no bonds");
            Assert.IsTrue(container.IsEmpty(), "atom contains no atoms but was not empty");
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedBondsMissingAtom()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var atom = builder.NewAtom();
            container.GetConnectedBonds(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedAtomsMissingAtom()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var atom = builder.NewAtom();
            container.GetConnectedBonds(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedAtomCountMissingAtom()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var atom = builder.NewAtom();
            container.GetConnectedAtoms(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedBondCountMissingAtom()
        {
            var container = (IAtomContainer)NewChemObject();
            var builder = container.Builder;
            var atom = builder.NewAtom();
            container.GetConnectedBonds(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetConnectedBondCountMissingIdx()
        {
            var container = (IAtomContainer)NewChemObject();
            container.GetConnectedBonds(container.Atoms[0]);
        }

        [TestMethod()]
        public void TestGetConnectedLongPairsMissingAtom()
        {
            try
            {
                var container = (IAtomContainer)NewChemObject();
                var builder = container.Builder;
                var atom = builder.NewAtom();
                container.GetConnectedLonePairs(atom);
                Assert.Fail();
            }
            catch (NoSuchAtomException)
            {
            }
        }

        [TestMethod()]
        public void TestGetConnectedSingleElecsMissingAtom()
        {
            try
            {
                var container = (IAtomContainer)NewChemObject();
                var builder = container.Builder;
                var atom = builder.NewAtom();
                container.GetConnectedSingleElectrons(atom);
            }
            catch (NoSuchAtomException)
            {
            }
        }

        [TestMethod()]
        public void TestGetConnectedLongPairCountMissingAtom()
        {
            try
            {
                var container = (IAtomContainer)NewChemObject();
                var builder = container.Builder;
                var atom = builder.NewAtom();
                container.GetConnectedLonePairs(atom);
            }
            catch (NoSuchAtomException)
            {
            }
        }

        [TestMethod()]
        public void TestGetConnectedSingleElecCountMissingAtom()
        {
            try
            {
                var container = (IAtomContainer)NewChemObject();
                var builder = container.Builder;
                var atom = builder.NewAtom();
                container.GetConnectedSingleElectrons(atom);
            }
            catch (NoSuchAtomException)
            {
            }
        }

        [TestMethod()]
        public void AddSameAtomTwice()
        {
            var mol = (IAtomContainer)NewChemObject();
            var atom = mol.Builder.NewAtom();
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom);
            Assert.AreEqual(1, mol.Atoms.Count);
        }

        [TestMethod()]
        public void PreserveAdjacencyOnSetAtoms()
        {
            var mol = (IAtomContainer)NewChemObject();
            var a1 = mol.Builder.NewAtom();
            var a2 = mol.Builder.NewAtom();
            var a3 = mol.Builder.NewAtom();
            var a4 = mol.Builder.NewAtom();
            var b1 = mol.Builder.NewBond();
            var b2 = mol.Builder.NewBond();
            var b3 = mol.Builder.NewBond();
            b1.SetAtoms(new IAtom[] { a1, a2 });
            b2.SetAtoms(new IAtom[] { a2, a3 });
            b3.SetAtoms(new IAtom[] { a3, a4 });
            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            mol.Atoms.Add(a4);
            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);
            mol.Bonds.Add(b3);
            Assert.AreEqual(1, mol.GetConnectedBonds(a1).Count());
            Assert.AreEqual(2, mol.GetConnectedBonds(a2).Count());
            Assert.AreEqual(2, mol.GetConnectedBonds(a3).Count());
            Assert.AreEqual(1, mol.GetConnectedBonds(a4).Count());
            mol.SetAtoms(new IAtom[] { a3, a4, a2, a1 });
            Assert.AreEqual(1, mol.GetConnectedBonds(a1).Count());
            Assert.AreEqual(2, mol.GetConnectedBonds(a2).Count());
            Assert.AreEqual(2, mol.GetConnectedBonds(a3).Count());
            Assert.AreEqual(1, mol.GetConnectedBonds(a4).Count());
        }

        [TestMethod()]
        public void SetConnectedAtomsAfterAddBond()
        {
            var mol = (IAtomContainer)NewChemObject();
            var a1 = mol.Builder.NewAtom();
            var a2 = mol.Builder.NewAtom();
            var b1 = mol.Builder.NewBond();
            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Bonds.Add(b1);
            // can't call on b1!
            mol.Bonds[0].SetAtoms(new IAtom[] { a1, a2 });
            Assert.AreEqual(1, mol.GetConnectedBonds(a1).Count());
            Assert.AreEqual(1, mol.GetConnectedBonds(a2).Count());
        }

        [TestMethod()]
        public void ChangeConnectedAtomsAfterAddBond()
        {
            var mol = (IAtomContainer)NewChemObject();
            var a1 = mol.Builder.NewAtom();
            var a2 = mol.Builder.NewAtom();
            var a3 = mol.Builder.NewAtom();
            var b1 = mol.Builder.NewBond();
            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            b1.SetAtoms(new IAtom[] { a1, a2 });
            mol.Bonds.Add(b1);
            Assert.AreEqual(1, mol.GetConnectedBonds(a1).Count());
            Assert.AreEqual(1, mol.GetConnectedBonds(a2).Count());
            Assert.AreEqual(0, mol.GetConnectedBonds(a3).Count());
            mol.Bonds[0].Atoms[0] = a3;
            Assert.AreEqual(0, mol.GetConnectedBonds(a1).Count());
            Assert.AreEqual(1, mol.GetConnectedBonds(a2).Count());
            Assert.AreEqual(1, mol.GetConnectedBonds(a3).Count());
            mol.Bonds[0].Atoms[1] = a1;
            Assert.AreEqual(1, mol.GetConnectedBonds(a1).Count());
            Assert.AreEqual(0, mol.GetConnectedBonds(a2).Count());
            Assert.AreEqual(1, mol.GetConnectedBonds(a3).Count());
        }

        [TestMethod()]
        public void CloneSgroups()
        {
            var mol = (IAtomContainer)NewChemObject();
            var a1 = mol.Builder.NewAtom();
            var a2 = mol.Builder.NewAtom();
            var a3 = mol.Builder.NewAtom();
            var b1 = mol.Builder.NewBond(a1, a2);
            var b2 = mol.Builder.NewBond(a2, a3);
            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);
            var sgroup = new Sgroup
            {
                Type = SgroupType.CtabStructureRepeatUnit,
                Subscript = "n"
            };
            sgroup.Atoms.Add(a2);
            sgroup.Bonds.Add(b1);
            sgroup.Bonds.Add(b2);
            mol.SetCtabSgroups(new[] { sgroup });
            var clone = (IAtomContainer)mol.Clone();
            var sgroups = clone.GetCtabSgroups();
            Assert.IsNotNull(sgroups);
            Assert.AreEqual(1, sgroups.Count);
            var clonedSgroup = sgroups.First();
            Assert.AreEqual(SgroupType.CtabStructureRepeatUnit, clonedSgroup.Type);
            Assert.AreEqual("n", clonedSgroup.Subscript);
            Assert.IsFalse(clonedSgroup.Atoms.Contains(a2));
            Assert.IsFalse(clonedSgroup.Bonds.Contains(b1));
            Assert.IsFalse(clonedSgroup.Bonds.Contains(b2));
            Assert.IsTrue(clonedSgroup.Atoms.Contains(clone.Atoms[1]));
            Assert.IsTrue(clonedSgroup.Bonds.Contains(clone.Bonds[0]));
            Assert.IsTrue(clonedSgroup.Bonds.Contains(clone.Bonds[1]));
        }

        [TestMethod()]
        public void CloneSgroupsBrackets()
        {
            var mol = (IAtomContainer)NewChemObject();
            var a1 = mol.Builder.NewAtom();
            var a2 = mol.Builder.NewAtom();
            var a3 = mol.Builder.NewAtom();
            var b1 = mol.Builder.NewBond(a1, a2);
            var b2 = mol.Builder.NewBond(a2, a3);
            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);
            var sgroup = new Sgroup
            {
                Type = SgroupType.CtabStructureRepeatUnit,
                Subscript = "n"
            };
            sgroup.Atoms.Add(a2);
            sgroup.Bonds.Add(b1);
            sgroup.Bonds.Add(b2);
            var bracket1 = new SgroupBracket(0, 1, 2, 3);
            var bracket2 = new SgroupBracket(1, 2, 3, 4);
            sgroup.AddBracket(bracket1);
            sgroup.AddBracket(bracket2);
            mol.SetCtabSgroups(new[] { sgroup });
            var clone = (IAtomContainer)mol.Clone();
            var sgroups = clone.GetCtabSgroups();
            Assert.IsNotNull(sgroups);
            Assert.AreEqual(1, sgroups.Count);
            var clonedSgroup = sgroups.First();
            Assert.AreEqual(SgroupType.CtabStructureRepeatUnit, clonedSgroup.Type);
            Assert.AreEqual("n", clonedSgroup.Subscript);
            Assert.IsFalse(clonedSgroup.Atoms.Contains(a2));
            Assert.IsFalse(clonedSgroup.Bonds.Contains(b1));
            Assert.IsFalse(clonedSgroup.Bonds.Contains(b2));
            Assert.IsTrue(clonedSgroup.Atoms.Contains(clone.Atoms[1]));
            Assert.IsTrue(clonedSgroup.Bonds.Contains(clone.Bonds[0]));
            Assert.IsTrue(clonedSgroup.Bonds.Contains(clone.Bonds[1]));
            var brackets = ((IEnumerable<SgroupBracket>)clonedSgroup.GetValue(SgroupKey.CtabBracket)).Cast<SgroupBracket>().ToList();
            Assert.AreEqual(2, brackets.Count);
            Assert.AreNotSame(bracket1, brackets[0]);
            Assert.AreNotSame(bracket2, brackets[1]);
            AssertAreEqual(brackets[0].FirstPoint, new Vector2(0, 1), 0.01);
            AssertAreEqual(brackets[0].SecondPoint, new Vector2(2, 3), 0.01);
            AssertAreEqual(brackets[1].FirstPoint, new Vector2(1, 2), 0.01);
            AssertAreEqual(brackets[1].SecondPoint, new Vector2(3, 4), 0.01);
        }

        [TestMethod()]
        public void GetSelfBond()
        {
            var mol = (IAtomContainer)NewChemObject();
            var a1 = mol.Builder.NewAtom();
            var a2 = mol.Builder.NewAtom();
            var a3 = mol.Builder.NewAtom();
            var b1 = mol.Builder.NewBond();
            var b2 = mol.Builder.NewBond();
            b1.Atoms.Add(a1);
            b1.Atoms.Add(a2);
            b2.Atoms.Add(a2);
            b2.Atoms.Add(a3);
            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);
            Assert.IsNull(mol.GetBond(a1, a1));
        }
    }
}
