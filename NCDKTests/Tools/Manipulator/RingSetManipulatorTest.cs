/* Copyright (C) 2006-2007  The Chemistry Development Kit Project
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.RingSearches;
using NCDK.Templates;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    // @cdk.module test-standard
    [TestClass()]
    public class RingSetManipulatorTest : CDKTestCase
    {
        protected IChemObjectBuilder builder;

        private IRingSet ringset = null;
        private IAtom ring1Atom1 = null;
        private IAtom ring1Atom3 = null;
        private IAtom ring2Atom3 = null;
        private IBond bondRing2Ring3 = null;
        private IRing ring2 = null;
        private IRing ring3 = null;

        [TestInitialize()]
        public void SetUp()
        {
            builder = Default.ChemObjectBuilder.Instance;
            ringset = builder.NewRingSet();
            ring1Atom1 = builder.NewAtom("C"); // rather artificial molecule
            IAtom ring1Atom2 = builder.NewAtom("C");
            ring1Atom3 = builder.NewAtom("C");
            IAtom ring2Atom1 = builder.NewAtom("C");
            IAtom ring2Atom2 = builder.NewAtom("C");
            ring2Atom3 = builder.NewAtom("C");
            IAtom ring3Atom3 = builder.NewAtom("C");
            IAtom ring3Atom4 = builder.NewAtom("C");

            IAtom ring4Atom1 = builder.NewAtom("C");
            IAtom ring4Atom2 = builder.NewAtom("C");

            IBond ring1Bond1 = builder.NewBond(ring1Atom1, ring1Atom2);
            IBond ring1Bond2 = builder.NewBond(ring1Atom2, ring1Atom3);
            IBond ring1Bond3 = builder.NewBond(ring1Atom3, ring1Atom1);
            bondRing2Ring3 = builder.NewBond(ring2Atom1, ring2Atom2);
            IBond ring2Bond2 = builder.NewBond(ring2Atom2, ring2Atom3);
            IBond ring2Bond3 = builder.NewBond(ring2Atom3, ring2Atom1, BondOrder.Double);
            IBond ring3Bond2 = builder.NewBond(ring2Atom2, ring3Atom3);
            IBond bondRing3Ring4 = builder.NewBond(ring3Atom3, ring3Atom4);
            IBond ring3Bond4 = builder.NewBond(ring3Atom4, ring2Atom1);
            IBond ring4Bond1 = builder.NewBond(ring4Atom1, ring4Atom2);
            IBond ring4Bond2 = builder.NewBond(ring4Atom2, ring3Atom3);
            IBond ring4Bond3 = builder.NewBond(ring3Atom4, ring4Atom1);

            IRing ring1 = builder.NewRing();
            ring1.Atoms.Add(ring1Atom1);
            ring1.Atoms.Add(ring1Atom2);
            ring1.Atoms.Add(ring1Atom3);
            ring1.Bonds.Add(ring1Bond1);
            ring1.Bonds.Add(ring1Bond2);
            ring1.Bonds.Add(ring1Bond3);

            ring2 = builder.NewRing();
            ring2.Atoms.Add(ring2Atom1);
            ring2.Atoms.Add(ring2Atom2);
            ring2.Atoms.Add(ring2Atom3);
            ring2.Bonds.Add(bondRing2Ring3);
            ring2.Bonds.Add(ring2Bond2);
            ring2.Bonds.Add(ring2Bond3);

            ring3 = builder.NewRing();
            ring3.Atoms.Add(ring2Atom1);
            ring3.Atoms.Add(ring2Atom2);
            ring3.Atoms.Add(ring3Atom3);
            ring3.Atoms.Add(ring3Atom4);
            ring3.Bonds.Add(bondRing2Ring3);
            ring3.Bonds.Add(ring3Bond2);
            ring3.Bonds.Add(bondRing3Ring4);
            ring3.Bonds.Add(ring3Bond4);

            IRing ring4 = builder.NewRing();
            ring4.Atoms.Add(ring4Atom1);
            ring4.Atoms.Add(ring4Atom2);
            ring4.Atoms.Add(ring3Atom3);
            ring4.Atoms.Add(ring3Atom4);
            ring4.Bonds.Add(bondRing3Ring4);
            ring4.Bonds.Add(ring4Bond1);
            ring4.Bonds.Add(ring4Bond2);
            ring4.Bonds.Add(ring4Bond3);

            ringset.Add(ring1);
            ringset.Add(ring2);
            ringset.Add(ring3);
            ringset.Add(ring4);
        }

        [TestMethod()]
        public void TestIsSameRing_IRingSet_IAtom_IAtom()
        {
            Assert.IsTrue(RingSetManipulator.IsSameRing(ringset, ring1Atom1, ring1Atom3));
            Assert.IsFalse(RingSetManipulator.IsSameRing(ringset, ring1Atom1, ring2Atom3));
        }

        [TestMethod()]
        public void TestRingAlreadyInSet_IRing_IRingSet()
        {
            IRing r1 = builder.NewRing(5, "C");
            IRing r2 = builder.NewRing(3, "C");

            IRingSet rs = builder.NewRingSet();
            Assert.IsFalse(RingSetManipulator.RingAlreadyInSet(r1, rs));
            Assert.IsFalse(RingSetManipulator.RingAlreadyInSet(r2, rs));

            rs.Add(r1);
            Assert.IsTrue(RingSetManipulator.RingAlreadyInSet(r1, rs));
            Assert.IsFalse(RingSetManipulator.RingAlreadyInSet(r2, rs));

            rs.Add(r2);
            Assert.IsTrue(RingSetManipulator.RingAlreadyInSet(r1, rs));
            Assert.IsTrue(RingSetManipulator.RingAlreadyInSet(r2, rs));
        }

        [TestMethod()]
        public void TestGetAllAtomContainers_IRingSet()
        {
            IRingSet rs = builder.NewRingSet();
            rs.Add(builder.NewRing());
            rs.Add(builder.NewRing());
            var list = RingSetManipulator.GetAllAtomContainers(rs);
            Assert.AreEqual(2, list.Count());
        }

        [TestMethod()]
        public void TestGetAtomCount_IRingSet()
        {
            IRingSet rs = builder.NewRingSet();
            IRing ac1 = builder.NewRing();
            ac1.Atoms.Add(builder.NewAtom("O"));
            rs.Add(ac1);
            IRing ac2 = builder.NewRing();
            ac2.Atoms.Add(builder.NewAtom("C"));
            ac2.Atoms.Add(builder.NewAtom("C"));
            ac2.AddBond(ac2.Atoms[0], ac2.Atoms[1], BondOrder.Double);
            rs.Add(ac2);
            Assert.AreEqual(3, RingSetManipulator.GetAtomCount(rs));
            Assert.AreEqual(1, RingSetManipulator.GetBondCount(rs));
        }

        [TestMethod()]
        public void TestGetHeaviestRing_IRingSet_IBond()
        {
            IRing ring = RingSetManipulator.GetHeaviestRing(ringset, bondRing2Ring3);
            Assert.AreEqual(ring2, ring);
        }

        [TestMethod()]
        public void TestGetMostComplexRing_IRingSet()
        {
            IRing ring = RingSetManipulator.GetMostComplexRing(ringset);
            Assert.AreEqual(ring3, ring);
        }

        [TestMethod()]
        public void TestSort_IRingSet()
        {
            RingSetManipulator.Sort(ringset);
            Assert.AreEqual(4, ringset.Count);
            int currentSize = ringset[0].Atoms.Count;
            for (int i = 1; i < ringset.Count; ++i)
            {
                Assert.IsTrue(ringset[i].Atoms.Count >= currentSize);
                currentSize = ringset[i].Atoms.Count;
            }
        }

        [TestMethod()]
        public void TestGetBondCount()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeAdenine();
            AllRingsFinder arf = new AllRingsFinder();
            IRingSet ringSet = arf.FindAllRings(mol);
            Assert.AreEqual(3, ringSet.Count);
            Assert.AreEqual(20, RingSetManipulator.GetBondCount(ringSet));

            mol = TestMoleculeFactory.MakeBiphenyl();
            ringSet = arf.FindAllRings(mol);
            Assert.AreEqual(2, ringSet.Count);
            Assert.AreEqual(12, RingSetManipulator.GetBondCount(ringSet));
        }

        [TestMethod()]
        public void MarkAromatic()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeBiphenyl();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            AllRingsFinder arf = new AllRingsFinder();
            IRingSet ringSet = arf.FindAllRings(mol);
            Assert.AreEqual(2, ringSet.Count);

            RingSetManipulator.MarkAromaticRings(ringSet);
            for (int i = 0; i < ringSet.Count; i++)
            {
                IRing ring = (IRing)ringSet[i];
                Assert.IsTrue(ring.IsAromatic);
            }
        }

        [TestMethod()]
        public void TestGetAllInOneContainer_IRingSet()
        {
            IAtomContainer ac = RingSetManipulator.GetAllInOneContainer(ringset);
            Assert.AreEqual(10, ac.Atoms.Count);
        }

        [TestMethod()]
        public void TestGetLargestRingSet_List_IRingSet()
        {
            IList<IRingSet> list = new List<IRingSet> { ringset };
            IAtomContainer mol = TestMoleculeFactory.MakeBiphenyl();

            AllRingsFinder arf = new AllRingsFinder();
            IRingSet ringSet = arf.FindAllRings(mol);
            list.Add(ringSet);
            Assert.AreEqual(2, RingSetManipulator.GetLargestRingSet(list).Count);
        }
    }
}
