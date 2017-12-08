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
    [TestClass()]
    public abstract class AbstractAtomContainerTest
        : AbstractChemObjectTest
    {
        private static IAtom[] MakeTestAtomsArray(IAtomContainer ac)
        {
            IAtom[] atoms = new IAtom[4];
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
            IAtomContainer ac = (IAtomContainer)NewChemObject();
            IAtom[] atoms = MakeTestAtomsArray(ac);
            Assert.AreEqual(4, ac.Atoms.Count);
        }

        // @cdk.bug 2993609
        [TestMethod()]
        public virtual void TestSetAtoms_removeListener()
        {
            IAtomContainer ac = (IAtomContainer)NewChemObject();
            IAtom[] atoms = MakeTestAtomsArray(ac);

            // if an atom changes, the atomcontainer will throw a change event too
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
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
            IAtomContainer ac = (IAtomContainer)NewChemObject();
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            object clone = molecule.Clone();
            Assert.IsInstanceOfType(clone, typeof(IAtomContainer));
        }

        [TestMethod()]
        public virtual void TestClone_IAtom()
        {
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            for (int i = 0; i < 4; i++)
                molecule.Atoms.Add(molecule.Builder.NewAtom("C"));

            IAtomContainer clonedMol = (IAtomContainer)molecule.Clone();
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
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            IAtom atom = molecule.Builder.NewAtom();
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
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            molecule.Atoms.Add(molecule.Builder.NewAtom());
            molecule.Atoms.Add(molecule.Builder.NewAtom());
            IBond bond = molecule.Builder.NewBond(molecule.Atoms[0], molecule.Atoms[1],
                    BondOrder.Single);
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
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            molecule.Atoms.Add(molecule.Builder.NewAtom());
            ILonePair lonePair = molecule.Builder.NewLonePair(molecule.Atoms[0]);
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
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            molecule.Atoms.Add(molecule.Builder.NewAtom());
            ISingleElectron singleElectron = molecule.Builder.NewSingleElectron(molecule.Atoms[0]);
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
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            IAtom carbon = molecule.Builder.NewAtom("C");
            carbon.Point2D = new Vector2(2, 4);
            molecule.Atoms.Add(carbon); // 1

            // test cloning of Atoms
            IAtomContainer clonedMol = (IAtomContainer)molecule.Clone();
            carbon.Point2D = new Vector2(3, 1);
            Assert.AreEqual(clonedMol.Atoms[0].Point2D.Value.X, 2.0, 0.001);
        }

        [TestMethod()]
        public virtual void TestClone_IBond()
        {
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            molecule.Atoms.Add(molecule.Builder.NewAtom("C")); // 1
            molecule.Atoms.Add(molecule.Builder.NewAtom("C")); // 2
            molecule.Atoms.Add(molecule.Builder.NewAtom("C")); // 3
            molecule.Atoms.Add(molecule.Builder.NewAtom("C")); // 4

            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double); // 1
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single); // 2
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single); // 3
            IAtomContainer clonedMol = (IAtomContainer)molecule.Clone();
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
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            IAtom atom1 = molecule.Builder.NewAtom("C");
            IAtom atom2 = molecule.Builder.NewAtom("C");
            molecule.Atoms.Add(atom1); // 1
            molecule.Atoms.Add(atom2); // 2
            molecule.Bonds.Add(molecule.Builder.NewBond(atom1, atom2, BondOrder.Double)); // 1

            // test cloning of atoms in bonds
            IAtomContainer clonedMol = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clonedMol);
            Assert.AreNotSame(atom1, clonedMol.Bonds[0].Begin);
            Assert.AreNotSame(atom2, clonedMol.Bonds[0].End);
        }

        [TestMethod()]
        public virtual void TestClone_IBond3()
        {
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            IAtom atom1 = molecule.Builder.NewAtom("C");
            IAtom atom2 = molecule.Builder.NewAtom("C");
            molecule.Atoms.Add(atom1); // 1
            molecule.Atoms.Add(atom2); // 2
            molecule.Bonds.Add(molecule.Builder.NewBond(atom1, atom2, BondOrder.Double)); // 1

            // test that cloned bonds contain atoms from cloned atomcontainer
            IAtomContainer clonedMol = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clonedMol);
            Assert.IsTrue(clonedMol.Contains(clonedMol.Bonds[0].Begin));
            Assert.IsTrue(clonedMol.Contains(clonedMol.Bonds[0].End));
        }

        [TestMethod()]
        public virtual void TestClone_AtomlessIBond()
        {
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            IBond bond = molecule.Builder.NewBond();
            molecule.Bonds.Add(bond);
            Assert.AreEqual(bond, molecule.Bonds[0]);
            IAtomContainer clone = (IAtomContainer)molecule.Clone();
            Assert.AreEqual(0, clone.Bonds[0].Atoms.Where(atom => atom != null).Count());
        }

        [TestMethod()]
        public virtual void TestClone_AtomlessILonePair()
        {
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            ILonePair lonePair = molecule.Builder.NewLonePair();
            molecule.LonePairs.Add(lonePair);
            Assert.AreEqual(lonePair, molecule.LonePairs[0]);
            IAtomContainer clone = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clone.LonePairs[0]);
        }

        [TestMethod()]
        public virtual void TestClone_AtomlessISingleElectron()
        {
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            ISingleElectron singleElectron = molecule.Builder.NewSingleElectron();
            molecule.SingleElectrons.Add(singleElectron);
            Assert.AreEqual(singleElectron, molecule.SingleElectrons[0]);
            IAtomContainer clone = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clone.SingleElectrons[0]);
        }

        [TestMethod()]
        public virtual void TestClone_ILonePair()
        {
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            IAtom atom1 = molecule.Builder.NewAtom("C");
            IAtom atom2 = molecule.Builder.NewAtom("C");
            molecule.Atoms.Add(atom1); // 1
            molecule.Atoms.Add(atom2); // 2
            molecule.AddLonePairTo(atom1);

            // test that cloned bonds contain atoms from cloned atomcontainer
            IAtomContainer clonedMol = (IAtomContainer)molecule.Clone();
            Assert.IsNotNull(clonedMol);
            Assert.AreEqual(1, clonedMol.GetConnectedLonePairs(clonedMol.Atoms[0]).Count());
        }

        [TestMethod()]
        public virtual void TestGetConnectedElectronContainersList_IAtom()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();

            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(1, acetone.GetConnectedElectronContainers(o).Count());
            Assert.AreEqual(3, acetone.GetConnectedElectronContainers(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedElectronContainers(c2).Count());
            Assert.AreEqual(1, acetone.GetConnectedElectronContainers(c3).Count());

            // Add lone pairs on oxygen
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();

            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(1, acetone.GetConnectedBonds(o).Count());
            Assert.AreEqual(3, acetone.GetConnectedBonds(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c2).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c3).Count());

            // Add lone pairs on oxygen
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
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

            IAtomContainer container = (IAtomContainer)NewChemObject();

            IChemObjectBuilder builder = container.Builder;

            IAtom c1 = builder.NewAtom("C");
            IAtom o2 = builder.NewAtom("O");
            IAtom n3 = builder.NewAtom("N");
            IAtom c4 = builder.NewAtom("C");
            IAtom h5 = builder.NewAtom("H");

            container.Atoms.Add(c1);
            container.Atoms.Add(o2);
            container.Atoms.Add(n3);
            container.Atoms.Add(c4);
            container.Atoms.Add(h5);

            IBond c1o2 = builder.NewBond(c1, o2);
            IBond c1n3 = builder.NewBond(c1, n3);
            IBond c1c4 = builder.NewBond(c1, c4);
            IBond c1h5 = builder.NewBond(c1, h5);

            c1o2.Stereo = BondStereo.Up;

            container.Bonds.Add(c1o2);
            container.Bonds.Add(c1n3);
            container.Bonds.Add(c1c4);
            container.Bonds.Add(c1h5);

            ITetrahedralChirality chirality = builder.NewTetrahedralChirality(c1, new IAtom[]{o2, n3, c4,
                h5}, TetrahedralStereo.Clockwise);

            container.StereoElements.Add(chirality);

            // clone the container
            IAtomContainer clone = (IAtomContainer)container.Clone();

            var elements = clone.StereoElements;

            Assert.IsTrue(elements.Count() > 0, "no stereo elements cloned");

            var element = elements.First();

            Assert.IsInstanceOfType(chirality, element.GetType(), "cloned element was incorrect class");
            Assert.IsTrue(elements.Count() == 1, "too many stereo elements");

            // we've tested the class already  - cast is okay
            ITetrahedralChirality cloneChirality = (ITetrahedralChirality)element;
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
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;

            IAtom c1 = builder.NewAtom("C");
            IAtom c2 = builder.NewAtom("C");
            IAtom c3 = builder.NewAtom("C");
            IAtom c4 = builder.NewAtom("C");

            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            container.Atoms.Add(c3);
            container.Atoms.Add(c4);

            IBond c1c2 = builder.NewBond(c1, c2, BondOrder.Double);
            IBond c2c3 = builder.NewBond(c2, c3);
            IBond c1c4 = builder.NewBond(c1, c4);

            container.Bonds.Add(c1c2);
            container.Bonds.Add(c2c3);
            container.Bonds.Add(c1c4);

            IDoubleBondStereochemistry dbStereo = new DoubleBondStereochemistry(
                c1c2,
                new IBond[] { c2c3, c1c4 },
                DoubleBondConformation.Opposite);

            container.StereoElements.Add(dbStereo);

            // clone the container
            IAtomContainer clone = (IAtomContainer)container.Clone();

            var elements = clone.StereoElements;

            Assert.IsTrue(elements.Count() > 0, "no stereo elements cloned");

            var element = elements.First();

            Assert.IsInstanceOfType(dbStereo, element.GetType(), "cloned element was incorrect class");
            Assert.IsTrue(elements.Count() == 1, "too many stereo elements");

            // we've tested the class already - cast is okay
            IDoubleBondStereochemistry clonedDBStereo = (IDoubleBondStereochemistry)element;
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

            IAtomContainer container = (IAtomContainer)NewChemObject();

            IChemObjectBuilder builder = container.Builder;

            IAtom c1 = builder.NewAtom("C");
            IAtom o2 = builder.NewAtom("O");
            IAtom n3 = builder.NewAtom("N");
            IAtom c4 = builder.NewAtom("C");
            IAtom h5 = builder.NewAtom("H");

            container.Atoms.Add(c1);
            container.Atoms.Add(o2);
            container.Atoms.Add(n3);
            container.Atoms.Add(c4);
            container.Atoms.Add(h5);

            IBond c1o2 = builder.NewBond(c1, o2);
            IBond c1n3 = builder.NewBond(c1, n3);
            IBond c1c4 = builder.NewBond(c1, c4);
            IBond c1h5 = builder.NewBond(c1, h5);

            c1o2.Stereo = BondStereo.Up;

            container.Bonds.Add(c1o2);
            container.Bonds.Add(c1n3);
            container.Bonds.Add(c1c4);
            container.Bonds.Add(c1h5);

            ITetrahedralChirality chirality = builder.NewTetrahedralChirality(c1, new IAtom[]{o2, n3, c4,
                h5}, TetrahedralStereo.Clockwise);

            container.StereoElements.Add(chirality);

            // clone the container
            IAtomContainer clone = (IAtomContainer)container.Clone();

            var elements = clone.StereoElements;

            Assert.IsTrue(elements.Count() > 0, "no stereo elements cloned");

            var element = elements.First();

            Assert.IsInstanceOfType(chirality, element.GetType(), "cloned element was incorrect class");
            Assert.IsTrue(elements.Count() == 1, "too many stereo elements");

            // we've tested the class already  - cast is okay
            ITetrahedralChirality cloneChirality = (ITetrahedralChirality)element;
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
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IAtom atom = container.Builder.NewAtom();
            IBond bond = container.Builder.NewBond();
            IAtom a1 = container.Builder.NewAtom();
            IAtom a2 = container.Builder.NewAtom();
            IAtom a3 = container.Builder.NewAtom();
            IAtom a4 = container.Builder.NewAtom();
            IBond b1 = container.Builder.NewBond();
            IBond b2 = container.Builder.NewBond();

            Assert.IsFalse(container.StereoElements.Count > 0);

            var dbElements = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>();
            dbElements.Add(new DoubleBondStereochemistry(bond, new IBond[] { b1, b2 },
                        DoubleBondConformation.Together));
            container.SetStereoElements(dbElements);
            var first = container.StereoElements.GetEnumerator();
            Assert.IsTrue(first.MoveNext(), "container did not have stereo elements");
            Assert.AreEqual(dbElements[0], first.Current, "expected element to equal set element (double bond)");
            Assert.IsFalse(first.MoveNext(), "container had more then one stereo element");

            var tetrahedralElements = new List<IReadOnlyStereoElement<IChemObject, IChemObject>>();
            tetrahedralElements.Add(new TetrahedralChirality(atom, new IAtom[] { a1, a2, a3, a4 }, TetrahedralStereo.Clockwise));
            container.SetStereoElements(tetrahedralElements);
            var second = container.StereoElements.GetEnumerator();
            Assert.IsTrue(second.MoveNext(), "container did not have stereo elements");
            Assert.AreEqual(tetrahedralElements[0], second.Current, "expected element to equal set element (tetrahedral)");
            Assert.IsFalse(second.MoveNext(), "container had more then one stereo element");
        }

        //    [TestMethod()] public virtual void TestGetConnectedBonds_IAtom() {
        //        // acetone molecule
        //        IAtomContainer acetone = GetNewBuilder().NewAtomContainer();
        //
        //        IAtom c1 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom c2 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom o = GetNewBuilder().NewInstance(typeof(IAtom),"O");
        //        IAtom c3 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        acetone.Add(c1);
        //        acetone.Add(c2);
        //        acetone.Add(c3);
        //        acetone.Add(o);
        //        IBond b1 = GetNewBuilder().NewBond(c1, c2, BondOrder.Single);
        //        IBond b2 = GetNewBuilder().NewBond(c1, o, BondOrder.Double);
        //        IBond b3 = GetNewBuilder().NewBond(c1, c3, BondOrder.Single);
        //        acetone.Add(b1);
        //        acetone.Add(b2);
        //        acetone.Add(b3);
        //
        //        Assert.AreEqual(1, acetone.GetConnectedBondsVector(o).Count());
        //        Assert.AreEqual(3, acetone.GetConnectedBondsVector(c1).Count());
        //        Assert.AreEqual(1, acetone.GetConnectedBondsVector(c2).Count());
        //        Assert.AreEqual(1, acetone.GetConnectedBondsVector(c3).Count());
        //
        //        // Add lone pairs on oxygen
        //        ILonePair lp1 = GetNewBuilder().NewInstance(typeof(ILonePair),o);
        //        ILonePair lp2 = GetNewBuilder().NewInstance(typeof(ILonePair),o);
        //        acetone.Add(lp1);
        //        acetone.Add(lp2);
        //
        //        Assert.AreEqual(1, acetone.GetConnectedBondsVector(o).Count());
        //        Assert.AreEqual(3, acetone.GetConnectedBondsVector(c1).Count());
        //        Assert.AreEqual(1, acetone.GetConnectedBondsVector(c2).Count());
        //        Assert.AreEqual(1, acetone.GetConnectedBondsVector(c3).Count());
        //    }

        [TestMethod()]
        public virtual void TestGetConnectedLonePairsList_IAtom()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();

            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(0, acetone.GetConnectedLonePairs(o).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c1).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c2).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c3).Count());

            // Add lone pairs on oxygen
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();

            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();

            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            acetone.StereoElements.Add(new TetrahedralChirality(c1, new IAtom[] { c2, o, c3, c1 },
                    TetrahedralStereo.Clockwise));

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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            Assert.AreEqual(0, acetone.Atoms.Count);

            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            Assert.AreEqual(0, acetone.Bonds.Count);

            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestAdd_IAtomContainer()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            IAtomContainer container = (IAtomContainer)NewChemObject();
            container.Add(acetone);
            Assert.AreEqual(4, container.Atoms.Count);
            Assert.AreEqual(3, container.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestAdd_IAtomContainer_LonePairs()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c1);
            mol.AddLonePairTo(mol.Atoms[0]);

            IAtomContainer container = (IAtomContainer)NewChemObject();
            container.Add(mol);
            Assert.AreEqual(1, container.Atoms.Count);
            Assert.AreEqual(1, container.LonePairs.Count);
        }

        [TestMethod()]
        public virtual void TestAdd_IAtomContainer_SingleElectrons()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            IAtomContainer container = (IAtomContainer)NewChemObject();
            container.Add(mol);
            Assert.AreEqual(1, container.Atoms.Count);
            Assert.AreEqual(1, container.SingleElectrons.Count);
        }

        [TestMethod()]
        public virtual void TestRemove_IAtomContainer()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            IAtomContainer container = (IAtomContainer)NewChemObject();
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            IAtomContainer container = (IAtomContainer)NewChemObject();
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
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom focus = builder.NewAtom();
            IAtom a1 = builder.NewAtom();
            IAtom a2 = builder.NewAtom();
            IAtom a3 = builder.NewAtom();
            IAtom a4 = builder.NewAtom();
            container.StereoElements.Add(new TetrahedralChirality(focus,
                                                                new IAtom[] { a1, a2, a3, a4 },
                                                                TetrahedralStereo.Clockwise));

            int count = 0;
            foreach (var element in container.StereoElements)
                count++;

            Assert.AreEqual(1, count, "no stereo elements were added");

            count = 0;
            Assert.AreEqual(0, count, "count did not reset");

            container.RemoveAllElements();

            foreach (IReadOnlyStereoElement<IChemObject, IChemObject> element in container.StereoElements)
            {
                count++;
            }
            Assert.AreEqual(0, count, "stereo elements were not removed");
        }

        [TestMethod()]
        public virtual void TestRemoveAtom_int()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
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
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestSetAtomOutOfRange()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IAtom c = container.Builder.NewAtom("C");
            container.Atoms[0] = c;
        }

        [TestMethod()]
        public void TestSetAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IAtom c1 = container.Builder.NewAtom("C");
            IAtom c2 = container.Builder.NewAtom("C");
            container.Atoms.Add(c1);
            container.Atoms[0] = c2;
            Assert.AreEqual(c2, container.Atoms[0]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void TestSetAtomSameMolecule()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IAtom c1 = container.Builder.NewAtom("C");
            IAtom c2 = container.Builder.NewAtom("C");
            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            container.Atoms[0] = c2;
        }

        [TestMethod()]
        public void TestSetAtomUpdatesBonds()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IAtom a1 = container.Builder.NewAtom();
            IAtom a2 = container.Builder.NewAtom();
            IAtom a3 = container.Builder.NewAtom();
            IBond b1 = container.Builder.NewBond();
            IBond b2 = container.Builder.NewBond();
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

            IAtom a4 = container.Builder.NewAtom();
            container.Atoms[2] = a4;
            Assert.AreEqual(a4, b2.End);
        }

        [TestMethod()]
        public void TestSetAtomUpdatesSingleElectron()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder bldr = container.Builder;
            IAtom a1 = bldr.NewAtom();
            IAtom a2 = bldr.NewAtom();
            IAtom a3 = bldr.NewAtom();
            IBond b1 = bldr.NewBond();
            IBond b2 = bldr.NewBond();
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
            ISingleElectron se = bldr.NewSingleElectron();
            se.Atom = a3;
            container.SingleElectrons.Add(se);

            IAtom a4 = bldr.NewAtom();
            container.Atoms[2] = a4;

            Assert.AreEqual(a4, se.Atom);
        }

        [TestMethod()]
        public void TestSetAtomUpdatesAtomStereo()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder bldr = container.Builder;
            IAtom a1 = bldr.NewAtom();
            IAtom a2 = bldr.NewAtom();
            IAtom a3 = bldr.NewAtom();
            IAtom a4 = bldr.NewAtom();
            IAtom a5 = bldr.NewAtom();
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
            container.StereoElements.Add(new TetrahedralChirality(container.Atoms[0],
                                                                new IAtom[]{
                                                                container.Atoms[1],
                                                                container.Atoms[2],
                                                                container.Atoms[3],
                                                                container.Atoms[4]},
                                                                TetrahedralStereo.Clockwise));

            IAtom aNew = bldr.NewAtom();
            container.Atoms[2] = aNew;

            var siter = container.StereoElements.GetEnumerator();
            Assert.IsTrue(siter.MoveNext());
            var se = siter.Current;
            Assert.IsInstanceOfType(se, typeof(ITetrahedralChirality));
            ITetrahedralChirality tc = (ITetrahedralChirality)se;
            Assert.AreEqual(a1, tc.ChiralAtom);
            Assert.IsTrue(Compares.AreDeepEqual(new IAtom[] { a2, aNew, a4, a5 }, tc.Ligands));
            Assert.IsFalse(siter.MoveNext());
        }

        [TestMethod()]
        public void TestSetAtomUpdatesBondStereo()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder bldr = container.Builder;
            IAtom a1 = bldr.NewAtom();
            IAtom a2 = bldr.NewAtom();
            IAtom a3 = bldr.NewAtom();
            IAtom a4 = bldr.NewAtom();
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
            IBond b1 = container.Bonds[0];
            IBond b2 = container.Bonds[1];
            IBond b3 = container.Bonds[2];

            container.StereoElements.Add(new DoubleBondStereochemistry(b2,
                                                                     new IBond[] { b1, b3 },
                                                                     DoubleBondConformation.Together));

            IAtom aNew = bldr.NewAtom();
            container.Atoms[2] = aNew;

            Assert.AreEqual(aNew, b2.End);
            Assert.AreEqual(aNew, b3.Begin);

            var siter = container.StereoElements.GetEnumerator();
            Assert.IsTrue(siter.MoveNext());
            var se = siter.Current;
            Assert.IsInstanceOfType(se, typeof(IDoubleBondStereochemistry));
            IDoubleBondStereochemistry tc = (IDoubleBondStereochemistry)se;
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
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = mol.Builder;
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
            IAtomContainer mol = (IAtomContainer)NewChemObject();
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
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = mol.Builder;
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
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            var dummy = mol.Atoms[99999];
        }

        [TestMethod()]
        public virtual void TestGetAtom_int()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();

            IAtom c = acetone.Builder.NewAtom("C");
            IAtom n = acetone.Builder.NewAtom("N");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom s = acetone.Builder.NewAtom("S");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(n);
            acetone.Atoms.Add(o);
            acetone.Atoms.Add(s);

            IAtom a1 = acetone.Atoms[0];
            Assert.IsNotNull(a1);
            Assert.AreEqual("C", a1.Symbol);
            IAtom a2 = acetone.Atoms[1];
            Assert.IsNotNull(a2);
            Assert.AreEqual("N", a2.Symbol);
            IAtom a3 = acetone.Atoms[2];
            Assert.IsNotNull(a3);
            Assert.AreEqual("O", a3.Symbol);
            IAtom a4 = acetone.Atoms[3];
            Assert.IsNotNull(a4);
            Assert.AreEqual("S", a4.Symbol);
        }

        [TestMethod()]
        public virtual void TestGetBond_int()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            Assert.AreEqual(0, acetone.Bonds.Count);

            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Triple);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(BondOrder.Triple, acetone.Bonds[0].Order);
            Assert.AreEqual(BondOrder.Double, acetone.Bonds[1].Order);
            Assert.AreEqual(BondOrder.Single, acetone.Bonds[2].Order);
        }

        //    [TestMethod()] public virtual void TestSetElectronContainer_int_IElectronContainer() {
        //        IAtomContainer container = (IAtomContainer)NewChemObject();
        //        IAtom c1 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom c2 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        container.Add(c1);
        //        container.Add(c2);
        //        IBond b = GetNewBuilder().NewBond(c1, c2, 3);
        //        container.ElectronContainer = 3, b;
        //
        //        Assert.IsTrue(container.GetElectronContainers().ElementAt(3) is IBond);
        //        IBond bond = (IBond)container.GetElectronContainers().ElementAt(3);
        //        Assert.AreEqual(3.0, bond.Order);;
        //    }

        [TestMethod()]
        public virtual void TestGetElectronContainerCount()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.Bonds.Count);

            acetone.RemoveAllBonds();
            Assert.AreEqual(0, acetone.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveAllElectronContainers()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.GetElectronContainers().Count());

            acetone.RemoveAllElectronContainers();
            Assert.AreEqual(0, acetone.GetElectronContainers().Count());
        }

        //    [TestMethod()] public virtual void TestSetElectronContainerCount_int() {
        //        IAtomContainer container = (IAtomContainer)NewChemObject();
        //        container.GetElectronContainers().Count() = 2;
        //
        //        Assert.AreEqual(2, container.GetElectronContainers().Count());
        //    }

        //    [TestMethod()] public virtual void TestSetAtomCount_int() {
        //        IAtomContainer container = (IAtomContainer)NewChemObject();
        //        container.AtomCount = 2;
        //
        //        Assert.AreEqual(2, container.Atoms.Count);
        //    }

        //    [TestMethod()] public virtual void TestGetAtoms() {
        //        // acetone molecule
        //        IAtomContainer acetone = GetNewBuilder().NewAtomContainer();
        //        IAtom c1 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom c2 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom o = GetNewBuilder().NewInstance(typeof(IAtom),"O");
        //        IAtom c3 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        acetone.Add(c1);
        //        acetone.Add(c2);
        //        acetone.Add(c3);
        //        acetone.Add(o);
        //
        //        Assert.AreEqual(4, acetone.GetAtoms().Length);
        //    }

        [TestMethod()]
        public virtual void TestAddAtom_IAtom()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            IEnumerator<IAtom> atomIter = acetone.Atoms.GetEnumerator();
            Assert.IsNotNull(atomIter);
            Assert.IsTrue(atomIter.MoveNext());
            IAtom next = atomIter.Current;
            Assert.IsTrue(next is IAtom);
            Assert.AreEqual(c1, next);
            Assert.IsTrue(atomIter.MoveNext());
            next = (IAtom)atomIter.Current;
            Assert.IsTrue(next is IAtom);
            Assert.AreEqual(c2, next);
            Assert.IsTrue(atomIter.MoveNext());
            next = (IAtom)atomIter.Current;
            Assert.IsTrue(next is IAtom);
            Assert.AreEqual(c3, next);
            Assert.IsTrue(atomIter.MoveNext());
            next = (IAtom)atomIter.Current;
            Assert.IsTrue(next is IAtom);
            Assert.AreEqual(o, next);

            Assert.IsFalse(atomIter.MoveNext());
        }

        [TestMethod()]
        public virtual void TestBonds()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            IBond bond1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond bond2 = acetone.Builder.NewBond(c2, o, BondOrder.Double);
            IBond bond3 = acetone.Builder.NewBond(c2, c3, BondOrder.Single);
            acetone.Bonds.Add(bond1);
            acetone.Bonds.Add(bond2);
            acetone.Bonds.Add(bond3);

            IEnumerator<IBond> bonds = acetone.Bonds.GetEnumerator();
            Assert.IsNotNull(bonds);
            Assert.IsTrue(bonds.MoveNext());

            IBond next = (IBond)bonds.Current;
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            IBond bond1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond bond2 = acetone.Builder.NewBond(c2, o, BondOrder.Double);
            IBond bond3 = acetone.Builder.NewBond(c2, c3, BondOrder.Single);
            acetone.Bonds.Add(bond1);
            acetone.Bonds.Add(bond2);
            acetone.Bonds.Add(bond3);
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            IEnumerator<ILonePair> lonePairs = acetone.LonePairs.GetEnumerator();
            Assert.IsNotNull(lonePairs);
            Assert.IsTrue(lonePairs.MoveNext());

            ILonePair next = (ILonePair)lonePairs.Current;
            Assert.IsTrue(next is ILonePair);
            Assert.AreEqual(lp1, next);
            lonePairs.MoveNext();
            next = (ILonePair)lonePairs.Current;
            Assert.IsTrue(next is ILonePair);
            Assert.AreEqual(lp2, next);

            Assert.IsFalse(lonePairs.MoveNext());
        }

        [TestMethod()]
        public virtual void TestSingleElectrons()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            IBond bond1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond bond2 = acetone.Builder.NewBond(c2, o, BondOrder.Double);
            IBond bond3 = acetone.Builder.NewBond(c2, c3, BondOrder.Single);
            acetone.Bonds.Add(bond1);
            acetone.Bonds.Add(bond2);
            acetone.Bonds.Add(bond3);
            ISingleElectron se1 = acetone.Builder.NewSingleElectron(o);
            ISingleElectron se2 = acetone.Builder.NewSingleElectron(c1);
            acetone.SingleElectrons.Add(se1);
            acetone.SingleElectrons.Add(se2);

            var singleElectrons = acetone.SingleElectrons;
            Assert.IsNotNull(singleElectrons);
            Assert.IsTrue(singleElectrons.Count > 0);

            ISingleElectron next = (ISingleElectron)singleElectrons[0];
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);

            IBond bond1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond bond2 = acetone.Builder.NewBond(c2, o, BondOrder.Double);
            IBond bond3 = acetone.Builder.NewBond(c2, c3, BondOrder.Single);
            acetone.Bonds.Add(bond1);
            acetone.Bonds.Add(bond2);
            acetone.Bonds.Add(bond3);
            ISingleElectron se1 = acetone.Builder.NewSingleElectron(c1);
            ISingleElectron se2 = acetone.Builder.NewSingleElectron(c2);
            acetone.SingleElectrons.Add(se1);
            acetone.SingleElectrons.Add(se2);
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            var electronContainers = acetone.GetElectronContainers()?.ToList();
            Assert.IsNotNull(electronContainers);
            Assert.IsTrue(electronContainers.Count > 0);
            IElectronContainer ec = (IElectronContainer)electronContainers[2];
            Assert.IsTrue(ec is IBond);
            Assert.AreEqual(bond3, ec);
            ILonePair lp = (ILonePair)electronContainers[4];
            Assert.IsTrue(lp is ILonePair);
            Assert.AreEqual(lp2, lp);
            ISingleElectron se = (ISingleElectron)electronContainers[5];
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
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
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            container.Atoms.Add(builder.NewAtom());
            container.Atoms.Add(builder.NewAtom());
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Sextuple);
            Assert.AreEqual(BondOrder.Sextuple, container.GetMinimumBondOrder(container.Atoms[0]));
        }

        [TestMethod()]
        public void TestGetMinBondOrderNoBonds()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom atom = builder.NewAtom();
            container.Atoms.Add(atom);
            Assert.AreEqual(BondOrder.Unset, container.GetMinimumBondOrder(atom));
        }

        [TestMethod()]
        public void TestGetMinBondOrderImplH()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom a = builder.NewAtom();
            a.ImplicitHydrogenCount = 1;
            container.Atoms.Add(a);
            Assert.AreEqual(BondOrder.Single, container.GetMinimumBondOrder(a));
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetMinBondOrderNoSuchAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom a1 = builder.NewAtom();
            IAtom a2 = builder.NewAtom();
            container.Atoms.Add(a1);
            Assert.AreEqual(BondOrder.Unset, container.GetMinimumBondOrder(a2));
        }

        [TestMethod()]
        public void TestGetMaxBondOrderHighBondOrder()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            container.Atoms.Add(builder.NewAtom());
            container.Atoms.Add(builder.NewAtom());
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Sextuple);
            Assert.AreEqual(BondOrder.Sextuple, container.GetMaximumBondOrder(container.Atoms[0]));
        }

        [TestMethod()]
        public void TestGetMaxBondOrderNoBonds()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom atom = builder.NewAtom();
            container.Atoms.Add(atom);
            Assert.AreEqual(BondOrder.Unset, container.GetMaximumBondOrder(atom));
        }

        [TestMethod()]
        public void TestGetMaxBondOrderImplH()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom a = builder.NewAtom();
            a.ImplicitHydrogenCount = 1;
            container.Atoms.Add(a);
            Assert.AreEqual(BondOrder.Single, container.GetMaximumBondOrder(a));
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetMaxBondOrderNoSuchAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom a1 = builder.NewAtom();
            IAtom a2 = builder.NewAtom();
            container.Atoms.Add(a1);
            Assert.AreEqual(BondOrder.Unset, container.GetMaximumBondOrder(a2));
        }

        [TestMethod()]
        public virtual void TestRemoveElectronContainer_int()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            acetone.AddLonePairTo(acetone.Atoms[2]);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(3, acetone.Bonds.Count);
            Assert.AreEqual(5, acetone.GetElectronContainers().Count());
            acetone.RemoveElectronContainer(acetone.GetElectronContainers().Skip(3).First());

            Assert.AreEqual(3, acetone.Bonds.Count);
            Assert.AreEqual(4, acetone.GetElectronContainers().Count());
            acetone.RemoveElectronContainer(acetone.GetElectronContainers().First()); // first bond now
            Assert.AreEqual(2, acetone.Bonds.Count);
            Assert.AreEqual(3, acetone.GetElectronContainers().Count());
        }

        [TestMethod()]
        public virtual void TestRemoveElectronContainer_IElectronContainer()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            ILonePair firstLP = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(firstLP);
            acetone.LonePairs.Add(acetone.Builder.NewLonePair(o));
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
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

        //    [TestMethod()] public virtual void TestSetElectronContainers_arrayIElectronContainer() {
        //        // acetone molecule
        //        IAtomContainer acetone = GetNewBuilder().NewAtomContainer();
        //        IAtom c1 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom c2 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom o = GetNewBuilder().NewInstance(typeof(IAtom),"O");
        //        IAtom c3 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        acetone.Add(c1);
        //        acetone.Add(c2);
        //        acetone.Add(c3);
        //        acetone.Add(o);
        //        IElectronContainer[] electronContainers = new IElectronContainer[3];
        //        electronContainers[0] = GetNewBuilder().NewBond(c1, c2, BondOrder.Single);
        //        electronContainers[1] = GetNewBuilder().NewBond(c1, o, BondOrder.Double);
        //        electronContainers[2] = GetNewBuilder().NewBond(c1, c3, BondOrder.Single);
        //        acetone.ElectronContainers = electronContainers;
        //
        //        Assert.AreEqual(3, acetone.Bonds.Count);
        //        IBond[] bonds = acetone.Bonds;
        //        for (int i=0; i<bonds.Length; i++) {
        //            Assert.IsNotNull(bonds[i]);
        //        }
        //        Assert.AreEqual(electronContainers[0], bonds[0]);
        //        Assert.AreEqual(electronContainers[1], bonds[1]);
        //        Assert.AreEqual(electronContainers[2], bonds[2]);
        //    }

        //    [TestMethod()] public virtual void TestAddElectronContainers_IAtomContainer() {
        //        // acetone molecule
        //        IAtomContainer acetone = GetNewBuilder().NewAtomContainer();
        //        IAtom c1 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom c2 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom o = GetNewBuilder().NewInstance(typeof(IAtom),"O");
        //        IAtom c3 = GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        acetone.Add(c1);
        //        acetone.Add(c2);
        //        acetone.Add(c3);
        //        acetone.Add(o);
        //        IElectronContainer[] electronContainers = new IElectronContainer[3];
        //        electronContainers[0] = GetNewBuilder().NewBond(c1, c2, BondOrder.Single);
        //        electronContainers[1] = GetNewBuilder().NewBond(c1, o, BondOrder.Double);
        //        electronContainers[2] = GetNewBuilder().NewBond(c1, c3, BondOrder.Single);
        //        acetone.ElectronContainers = electronContainers;
        //
        //        IAtomContainer tested = (IAtomContainer)NewChemObject();
        //        tested.Add(GetNewBuilder().NewBond(c2, c3));
        //        tested.Adds(acetone);
        //
        //        Assert.AreEqual(0, tested.Atoms.Count);
        //        Assert.AreEqual(4, tested.Bonds.Count);
        //        IBond[] bonds = tested.Bonds;
        //        for (int i=0; i<bonds.Length; i++) {
        //            Assert.IsNotNull(bonds[i]);
        //        }
        //        Assert.AreEqual(electronContainers[0], bonds[1]);
        //        Assert.AreEqual(electronContainers[1], bonds[2]);
        //        Assert.AreEqual(electronContainers[2], bonds[3]);
        //    }

        [TestMethod()]
        public virtual void TestAddElectronContainer_IElectronContainer()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            acetone.LonePairs.Add(acetone.Builder.NewLonePair(o));
            ISingleElectron single = acetone.Builder.NewSingleElectron(c);
            acetone.SingleElectrons.Add(single);

            Assert.AreEqual(1, acetone.GetConnectedSingleElectrons(c).Count());
            Assert.AreEqual(single, (ISingleElectron)acetone.GetConnectedSingleElectrons(c).First());
        }

        [TestMethod()]
        public virtual void TestRemoveBond_IAtom_IAtom()
        {
            // acetone molecule
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
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
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IAtom c1 = container.Builder.NewAtom("C");
            IAtom c2 = container.Builder.NewAtom("O");
            IAtom o = container.Builder.NewAtom("H");
            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            container.Atoms.Add(o);

            Assert.IsNotNull(container.Atoms[0]);
            Assert.AreEqual("C", container.Atoms[0].Symbol);
        }

        [TestMethod()]
        public virtual void TestGetLastAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IAtom c1 = container.Builder.NewAtom("C");
            IAtom c2 = container.Builder.NewAtom("O");
            IAtom o = container.Builder.NewAtom("H");
            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            container.Atoms.Add(o);

            Assert.IsNotNull(container.Atoms[container.Atoms.Count - 1]);
            Assert.AreEqual("H", container.Atoms[container.Atoms.Count - 1].Symbol);
        }

        [TestMethod()]
        public virtual void TestGetAtomNumber_IAtom()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            Assert.AreEqual(acetone.GetBond(c1, c2), b1);
            Assert.AreEqual(acetone.GetBond(c1, o), b2);
            Assert.AreEqual(acetone.GetBond(c1, c3), b3);

            // test the default return value
            Assert.IsNull(acetone.GetBond(acetone.Builder.NewAtom(), acetone.Builder
                    .NewAtom()));
        }

        //    [TestMethod()] public virtual void TestGetConnectedAtoms_IAtom() {
        //        IAtomContainer acetone = acetone.GetNewBuilder().NewAtomContainer();
        //        IAtom c1 = acetone.GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom c2 = acetone.GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        IAtom o = acetone.GetNewBuilder().NewInstance(typeof(IAtom),"O");
        //        IAtom c3 = acetone.GetNewBuilder().NewInstance(typeof(IAtom),"C");
        //        acetone.Add(c1);
        //        acetone.Add(c2);
        //        acetone.Add(c3);
        //        acetone.Add(o);
        //        IBond b1 = acetone.GetNewBuilder().NewBond(c1, c2, BondOrder.Single);
        //        IBond b2 = acetone.GetNewBuilder().NewBond(c1, o, BondOrder.Double);
        //        IBond b3 = acetone.GetNewBuilder().NewBond(c1, c3, BondOrder.Single);
        //        acetone.Add(b1);
        //        acetone.Add(b2);
        //        acetone.Add(b3);
        //
        //        Assert.AreEqual(3, acetone.GetConnectedAtoms(c1).Length);
        //        Assert.AreEqual(1, acetone.GetConnectedAtoms(c2).Length);
        //        Assert.AreEqual(1, acetone.GetConnectedAtoms(c3).Length);
        //        Assert.AreEqual(1, acetone.GetConnectedAtoms(o).Length);
        //    }

        [TestMethod()]
        public virtual void TestGetConnectedAtomsList_IAtom()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(2, acetone.LonePairs.Count);
        }

        [TestMethod()]
        public virtual void TestGetConnectedLonePairsCount_IAtom()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            IAtom c3 = acetone.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // Add lone pairs on oxygen
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
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
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IAtom carbon = container.Builder.NewAtom("C");
            carbon.Id = "central";
            IAtom carbon1 = container.Builder.NewAtom("C");
            carbon1.Id = "c1";
            IAtom carbon2 = container.Builder.NewAtom("C");
            carbon2.Id = "c2";
            IAtom carbon3 = container.Builder.NewAtom("C");
            carbon3.Id = "c3";
            IAtom carbon4 = container.Builder.NewAtom("C");
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
            IAtomContainer container = (IAtomContainer)NewChemObject();
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
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IAtomContainer chemObject = (IAtomContainer)NewChemObject();
            chemObject.Listeners.Add(listener);

            IChemObjectBuilder builder = chemObject.Builder;
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            ISingleElectron single1 = acetone.Builder.NewSingleElectron(c);
            ISingleElectron single2 = acetone.Builder.NewSingleElectron(c);
            ISingleElectron single3 = acetone.Builder.NewSingleElectron(o);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            ILonePair lp1 = acetone.Builder.NewLonePair(o);
            ILonePair lp2 = acetone.Builder.NewLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);
            Assert.AreEqual(2, acetone.GetConnectedLonePairs(o).Count());
            Assert.AreEqual(0, acetone.GetConnectedLonePairs(c).Count());
        }

        [TestMethod()]
        public virtual void TestAddSingleElectron_ISingleElectron()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            ISingleElectron single1 = acetone.Builder.NewSingleElectron(c);
            ISingleElectron single2 = acetone.Builder.NewSingleElectron(c);
            ISingleElectron single3 = acetone.Builder.NewSingleElectron(o);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c = acetone.Builder.NewAtom("C");
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            IBond b = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b);
            acetone.Atoms.Add(c1);
            IBond b1 = acetone.Builder.NewBond(c, c1, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Atoms.Add(c2);
            IBond b2 = acetone.Builder.NewBond(c, c2, BondOrder.Single);
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
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b1);
            IBond falseBond = acetone.Builder.NewBond();
            Assert.IsTrue(acetone.Contains(b1));
            Assert.IsFalse(acetone.Contains(falseBond));
        }

        [TestMethod()]
        public virtual void TestAddSingleElectron_int()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            Assert.AreEqual(2, mol.SingleElectrons.Count);
            Assert.IsNotNull(mol.SingleElectrons[1]);
            var singles = mol.SingleElectrons.ToList();
            ISingleElectron singleElectron = singles[0];
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
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
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
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            Assert.AreEqual(1, mol.Bonds.Count);
            IBond bond = mol.Bonds[0];
            mol.Bonds.Remove(bond);
            Assert.AreEqual(0, mol.Bonds.Count);
        }

        [TestMethod()]
        public virtual void TestGetConnectedBondsCount_IAtom()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c = acetone.Builder.NewAtom("C");
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            IBond b = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b);
            acetone.Atoms.Add(c1);
            IBond b1 = acetone.Builder.NewBond(c, c1, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Atoms.Add(c2);
            IBond b2 = acetone.Builder.NewBond(c, c2, BondOrder.Single);
            acetone.Bonds.Add(b2);
            Assert.AreEqual(1, acetone.GetConnectedBonds(o).Count());
            Assert.AreEqual(3, acetone.GetConnectedBonds(c).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c1).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(c2).Count());
        }

        [TestMethod()]
        public virtual void TestGetConnectedBondsCount_int()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c = acetone.Builder.NewAtom("C");
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            IBond b = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b);
            acetone.Atoms.Add(c1);
            IBond b1 = acetone.Builder.NewBond(c, c1, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Atoms.Add(c2);
            IBond b2 = acetone.Builder.NewBond(c, c2, BondOrder.Single);
            acetone.Bonds.Add(b2);
            Assert.AreEqual(1, acetone.GetConnectedBonds(acetone.Atoms[1]).Count());
            Assert.AreEqual(3, acetone.GetConnectedBonds(acetone.Atoms[0]).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(acetone.Atoms[2]).Count());
            Assert.AreEqual(1, acetone.GetConnectedBonds(acetone.Atoms[3]).Count());
        }

        // AtomContainer.SetBonds is not supported
        //[TestMethod()]
        //public virtual void TestSetBonds_arrayIBond()
        //{
        //    IAtomContainer acetone = (IAtomContainer)NewChemObject();
        //    IAtom c = acetone.Builder.NewAtom("C");
        //    IAtom c1 = acetone.Builder.NewAtom("C");
        //    IAtom c2 = acetone.Builder.NewAtom("C");
        //    IAtom o = acetone.Builder.NewAtom("O");
        //    acetone.Add(c);
        //    acetone.Add(o);
        //    IBond b = acetone.Builder.NewBond(c, o, BondOrder.Double);
        //    //acetone.Add(b);
        //    acetone.Add(c1);
        //    IBond b1 = acetone.Builder.NewBond(c, c1, BondOrder.Single);
        //    //acetone.Add(b1);
        //    acetone.Add(c2);
        //    IBond b2 = acetone.Builder.NewBond(c, c2, BondOrder.Single);
        //    //acetone.Add(b2);
        //    IBond[] bonds = new IBond[3];
        //    bonds[0] = b;
        //    bonds[1] = b1;
        //    bonds[2] = b2;
        //    acetone.Bonds = bonds;
        //    Assert.AreEqual(3, acetone.Bonds.Count);
        //    Assert.AreEqual(acetone.Bonds[2], b2);
        //}

        [TestMethod()]
        public virtual void TestGetLonePair_int()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddLonePairTo(mol.Atoms[1]);
            ILonePair lp = mol.Builder.NewLonePair(c);
            mol.LonePairs.Add(lp);
            Assert.AreEqual(lp, mol.LonePairs[1]);
        }

        [TestMethod()]
        public virtual void TestGetSingleElectron_int()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            ISingleElectron se = mol.Builder.NewSingleElectron(c);
            mol.SingleElectrons.Add(se);
            Assert.AreEqual(se, mol.SingleElectrons[1]);
        }

        [TestMethod()]
        public virtual void TestGetLonePairNumber_ILonePair()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddLonePairTo(mol.Atoms[1]);
            ILonePair lp = mol.Builder.NewLonePair(c);
            mol.LonePairs.Add(lp);
            Assert.AreEqual(1, mol.LonePairs.IndexOf(lp));
        }

        [TestMethod()]
        public virtual void TestGetSingleElectronNumber_ISingleElectron()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            ISingleElectron se = mol.Builder.NewSingleElectron(c);
            mol.SingleElectrons.Add(se);
            Assert.AreEqual(1, mol.SingleElectrons.IndexOf(se));
        }

        [TestMethod()]
        public virtual void TestGetElectronContainer_int()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c = acetone.Builder.NewAtom("C");
            IAtom c1 = acetone.Builder.NewAtom("C");
            IAtom c2 = acetone.Builder.NewAtom("C");
            IAtom o = acetone.Builder.NewAtom("O");
            acetone.Atoms.Add(c);
            acetone.Atoms.Add(o);
            IBond b = acetone.Builder.NewBond(c, o, BondOrder.Double);
            acetone.Bonds.Add(b);
            acetone.Atoms.Add(c1);
            IBond b1 = acetone.Builder.NewBond(c, c1, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Atoms.Add(c2);
            IBond b2 = acetone.Builder.NewBond(c, c2, BondOrder.Single);
            acetone.Bonds.Add(b2);
            acetone.AddLonePairTo(acetone.Atoms[1]);
            acetone.AddLonePairTo(acetone.Atoms[1]);
            Assert.IsTrue(acetone.GetElectronContainers().ElementAt(2) is IBond);
            Assert.IsTrue(acetone.GetElectronContainers().ElementAt(4) is ILonePair);
        }

        [TestMethod()]
        public virtual void TestGetSingleElectronCount()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            Assert.AreEqual(2, mol.SingleElectrons.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveLonePair_int()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddLonePairTo(mol.Atoms[1]);
            ILonePair lp = mol.Builder.NewLonePair(c);
            mol.LonePairs.Add(lp);
            mol.LonePairs.Remove(mol.LonePairs[0]);
            Assert.AreEqual(1, mol.LonePairs.Count);
            Assert.AreEqual(lp, mol.LonePairs[0]);
        }

        [TestMethod()]
        public virtual void TestRemoveLonePair_ILonePair()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            ILonePair lp = mol.Builder.NewLonePair(c1);
            mol.LonePairs.Add(lp);
            ILonePair lp1 = mol.Builder.NewLonePair(c);
            mol.LonePairs.Add(lp1);
            mol.LonePairs.Remove(lp);
            Assert.AreEqual(1, mol.LonePairs.Count);
            Assert.AreEqual(lp1, mol.LonePairs[0]);
        }

        [TestMethod()]
        public virtual void TestRemoveSingleElectron_int()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            ISingleElectron se = mol.Builder.NewSingleElectron(c);
            mol.SingleElectrons.Add(se);
            mol.SingleElectrons.Remove(mol.SingleElectrons[0]);
            Assert.AreEqual(1, mol.SingleElectrons.Count);
            Assert.AreEqual(se, mol.SingleElectrons[0]);
        }

        [TestMethod()]
        public virtual void TestRemoveSingleElectron_ISingleElectron()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            ISingleElectron se1 = mol.Builder.NewSingleElectron(c1);
            mol.SingleElectrons.Add(se1);
            ISingleElectron se = mol.Builder.NewSingleElectron(c);
            mol.SingleElectrons.Add(se);
            Assert.AreEqual(2, mol.SingleElectrons.Count);
            mol.SingleElectrons.Remove(se);
            Assert.AreEqual(1, mol.SingleElectrons.Count);
            Assert.AreEqual(se1, mol.SingleElectrons[0]);
        }

        [TestMethod()]
        public virtual void TestContains_ILonePair()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            ILonePair lp = mol.Builder.NewLonePair(c1);
            mol.LonePairs.Add(lp);
            ILonePair lp1 = mol.Builder.NewLonePair(c);
            Assert.IsTrue(mol.Contains(lp));
            Assert.IsFalse(mol.Contains(lp1));
        }

        [TestMethod()]
        public virtual void TestContains_ISingleElectron()
        {
            IAtomContainer mol = (IAtomContainer)NewChemObject();
            IAtom c = mol.Builder.NewAtom("C");
            IAtom c1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(c);
            mol.Atoms.Add(c1);
            ISingleElectron se = mol.Builder.NewSingleElectron(c1);
            mol.SingleElectrons.Add(se);
            ISingleElectron se1 = mol.Builder.NewSingleElectron(c1);
            Assert.IsTrue(mol.Contains(se));
            Assert.IsFalse(mol.Contains(se1));
        }

        [TestMethod()]
        public virtual void TestIsEmpty()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            Assert.IsTrue(container.IsEmpty(), "new atom container was not empty");
            IAtom c1 = container.Builder.NewAtom("C");
            IAtom c2 = container.Builder.NewAtom("C");
            container.Atoms.Add(c1);
            container.Atoms.Add(c2);
            Assert.IsFalse(container.IsEmpty(), "atom container Contains 2 atoms but was empty");
            container.Bonds.Add(container.Builder.NewBond(c1, c2));
            Assert.IsFalse(container.IsEmpty(), "atom container Contains 2 atoms and 1 bond but was empty");
            container.Atoms.Remove(c1);
            container.Atoms.Remove(c2);
            Assert.AreEqual(1, container.Bonds.Count, "atom Contains Contains no bonds");
            Assert.IsTrue(container.IsEmpty(), "atom Contains Contains no atoms but was not empty");
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedBondsMissingAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom atom = builder.NewAtom();
            container.GetConnectedBonds(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedAtomsMissingAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom atom = builder.NewAtom();
            container.GetConnectedBonds(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedAtomCountMissingAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom atom = builder.NewAtom();
            container.GetConnectedAtoms(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedBondCountMissingAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom atom = builder.NewAtom();
            container.GetConnectedBonds(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetConnectedBondCountMissingIdx()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            container.GetConnectedBonds(container.Atoms[0]);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedLongPairsMissingAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom atom = builder.NewAtom();
            container.GetConnectedLonePairs(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedSingleElecsMissingAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom atom = builder.NewAtom();
            container.GetConnectedSingleElectrons(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedLongPairCountMissingAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom atom = builder.NewAtom();
            container.GetConnectedLonePairs(atom);
        }

        [TestMethod()]
        [ExpectedException(typeof(NoSuchAtomException))]
        public void TestGetConnectedSingleElecCountMissingAtom()
        {
            IAtomContainer container = (IAtomContainer)NewChemObject();
            IChemObjectBuilder builder = container.Builder;
            IAtom atom = builder.NewAtom();
            container.GetConnectedSingleElectrons(atom);
        }
    }
}
