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
using System.Collections.Generic;
using System.Linq;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IRingSet"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractRingSetTest : AbstractAtomContainerSetTest<IRing>
    {
        [TestMethod()]
        public virtual void TestAdd_IRingSet()
        {
            IRingSet rs = (IRingSet)NewChemObject();
            IRing r1 = rs.Builder.NewRing(5, "C");
            IRing r2 = rs.Builder.NewRing(3, "C");
            rs.Add(r1);

            IRingSet rs2 = (IRingSet)NewChemObject();
            rs2.Add(r2);
            rs2.Add(rs);

            Assert.AreEqual(1, rs.Count);
            Assert.AreEqual(2, rs2.Count);
        }

        [TestMethod()]
        public override void TestToString()
        {
            IRingSet ringset = (IRingSet)NewChemObject();
            string description = ringset.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]
        public override void TestClone()
        {
            IRingSet ringset = (IRingSet)NewChemObject();
            IRing ring = ringset.Builder.NewRing();
            ringset.Add(ring);

            IRingSet clone = (IRingSet)ringset.Clone();
            Assert.IsNotNull(clone);
            Assert.IsTrue(clone is IRingSet);
            Assert.AreEqual(1, clone.Count);
            Assert.AreNotSame(ring, clone[0]);
        }

        [TestMethod()]
        public virtual void TestContains_IAtom()
        {
            IRingSet ringset = (IRingSet)NewChemObject();

            IAtom ring1Atom1 = ringset.Builder.NewAtom("C"); // rather artificial molecule
            IAtom ring1Atom2 = ringset.Builder.NewAtom("C");
            IAtom sharedAtom1 = ringset.Builder.NewAtom("C");
            IAtom sharedAtom2 = ringset.Builder.NewAtom("C");
            IAtom ring2Atom1 = ringset.Builder.NewAtom("C");
            IAtom ring2Atom2 = ringset.Builder.NewAtom("C");
            IBond ring1Bond1 = ringset.Builder.NewBond(ring1Atom1, ring1Atom2);
            IBond ring1Bond2 = ringset.Builder.NewBond(sharedAtom1, ring1Atom1);
            IBond ring1Bond3 = ringset.Builder.NewBond(sharedAtom2, ring1Atom2);
            IBond sharedBond = ringset.Builder.NewBond(sharedAtom1, sharedAtom2);
            IBond ring2Bond1 = ringset.Builder.NewBond(ring2Atom1, ring2Atom2);
            IBond ring2Bond2 = ringset.Builder.NewBond(sharedAtom1, ring2Atom1);
            IBond ring2Bond3 = ringset.Builder.NewBond(sharedAtom2, ring2Atom2);

            IRing ring1 = ringset.Builder.NewRing();
            ring1.Atoms.Add(ring1Atom1);
            ring1.Atoms.Add(ring1Atom2);
            ring1.Atoms.Add(sharedAtom1);
            ring1.Atoms.Add(sharedAtom2);
            ring1.Bonds.Add(ring1Bond1);
            ring1.Bonds.Add(ring1Bond2);
            ring1.Bonds.Add(ring1Bond3);
            ring1.Bonds.Add(sharedBond);
            IRing ring2 = ringset.Builder.NewRing();
            ring2.Atoms.Add(ring2Atom1);
            ring2.Atoms.Add(ring2Atom2);
            ring2.Atoms.Add(sharedAtom1);
            ring2.Atoms.Add(sharedAtom2);
            ring2.Bonds.Add(ring2Bond1);
            ring2.Bonds.Add(ring2Bond2);
            ring2.Bonds.Add(ring2Bond3);
            ring2.Bonds.Add(sharedBond);

            ringset.Add(ring1);
            ringset.Add(ring2);

            Assert.IsTrue(ringset.Contains(ring1Atom1));
            Assert.IsTrue(ringset.Contains(ring1Atom2));
            Assert.IsTrue(ringset.Contains(sharedAtom1));
            Assert.IsTrue(ringset.Contains(sharedAtom2));
            Assert.IsTrue(ringset.Contains(ring2Atom1));
            Assert.IsTrue(ringset.Contains(ring2Atom2));
        }

        [TestMethod()]
        public virtual void TestContains_IAtomContainer()
        {
            IRingSet ringset = (IRingSet)NewChemObject();

            IAtom ring1Atom1 = ringset.Builder.NewAtom("C"); // rather artificial molecule
            IAtom ring1Atom2 = ringset.Builder.NewAtom("C");
            IAtom sharedAtom1 = ringset.Builder.NewAtom("C");
            IAtom sharedAtom2 = ringset.Builder.NewAtom("C");
            IAtom ring2Atom1 = ringset.Builder.NewAtom("C");
            IAtom ring2Atom2 = ringset.Builder.NewAtom("C");
            IBond ring1Bond1 = ringset.Builder.NewBond(ring1Atom1, ring1Atom2);
            IBond ring1Bond2 = ringset.Builder.NewBond(sharedAtom1, ring1Atom1);
            IBond ring1Bond3 = ringset.Builder.NewBond(sharedAtom2, ring1Atom2);
            IBond sharedBond = ringset.Builder.NewBond(sharedAtom1, sharedAtom2);
            IBond ring2Bond1 = ringset.Builder.NewBond(ring2Atom1, ring2Atom2);
            IBond ring2Bond2 = ringset.Builder.NewBond(sharedAtom1, ring2Atom1);
            IBond ring2Bond3 = ringset.Builder.NewBond(sharedAtom2, ring2Atom2);

            IRing ring1 = ringset.Builder.NewRing();
            ring1.Atoms.Add(ring1Atom1);
            ring1.Atoms.Add(ring1Atom2);
            ring1.Atoms.Add(sharedAtom1);
            ring1.Atoms.Add(sharedAtom2);
            ring1.Bonds.Add(ring1Bond1);
            ring1.Bonds.Add(ring1Bond2);
            ring1.Bonds.Add(ring1Bond3);
            ring1.Bonds.Add(sharedBond);
            IRing ring2 = ringset.Builder.NewRing();
            ring2.Atoms.Add(ring2Atom1);
            ring2.Atoms.Add(ring2Atom2);
            ring2.Atoms.Add(sharedAtom1);
            ring2.Atoms.Add(sharedAtom2);
            ring2.Bonds.Add(ring2Bond1);
            ring2.Bonds.Add(ring2Bond2);
            ring2.Bonds.Add(ring2Bond3);
            ring2.Bonds.Add(sharedBond);

            ringset.Add(ring1);
            ringset.Add(ring2);

            Assert.IsTrue(ringset.Contains(ring1));
            Assert.IsTrue(ringset.Contains(ring2));
        }

        [TestMethod()]
        public virtual void TestGetRings_IBond()
        {
            IRingSet ringset = (IRingSet)NewChemObject();

            IAtom ring1Atom1 = ringset.Builder.NewAtom("C"); // rather artificial molecule
            IAtom ring1Atom2 = ringset.Builder.NewAtom("C");
            IAtom sharedAtom1 = ringset.Builder.NewAtom("C");
            IAtom sharedAtom2 = ringset.Builder.NewAtom("C");
            IAtom ring2Atom1 = ringset.Builder.NewAtom("C");
            IAtom ring2Atom2 = ringset.Builder.NewAtom("C");
            IBond ring1Bond1 = ringset.Builder.NewBond(ring1Atom1, ring1Atom2);
            IBond ring1Bond2 = ringset.Builder.NewBond(sharedAtom1, ring1Atom1);
            IBond ring1Bond3 = ringset.Builder.NewBond(sharedAtom2, ring1Atom2);
            IBond sharedBond = ringset.Builder.NewBond(sharedAtom1, sharedAtom2);
            IBond ring2Bond1 = ringset.Builder.NewBond(ring2Atom1, ring2Atom2);
            IBond ring2Bond2 = ringset.Builder.NewBond(sharedAtom1, ring2Atom1);
            IBond ring2Bond3 = ringset.Builder.NewBond(sharedAtom2, ring2Atom2);

            IRing ring1 = ringset.Builder.NewRing();
            ring1.Atoms.Add(ring1Atom1);
            ring1.Atoms.Add(ring1Atom2);
            ring1.Atoms.Add(sharedAtom1);
            ring1.Atoms.Add(sharedAtom2);
            ring1.Bonds.Add(ring1Bond1);
            ring1.Bonds.Add(ring1Bond2);
            ring1.Bonds.Add(ring1Bond3);
            ring1.Bonds.Add(sharedBond);
            IRing ring2 = ringset.Builder.NewRing();
            ring2.Atoms.Add(ring2Atom1);
            ring2.Atoms.Add(ring2Atom2);
            ring2.Atoms.Add(sharedAtom1);
            ring2.Atoms.Add(sharedAtom2);
            ring2.Bonds.Add(ring2Bond1);
            ring2.Bonds.Add(ring2Bond2);
            ring2.Bonds.Add(ring2Bond3);
            ring2.Bonds.Add(sharedBond);

            ringset.Add(ring1);
            ringset.Add(ring2);

            Assert.AreEqual(1, ringset.GetRings(ring1Bond1).Count());
            Assert.AreEqual(1, ringset.GetRings(ring1Bond2).Count());
            Assert.AreEqual(1, ringset.GetRings(ring1Bond3).Count());
            Assert.AreEqual(2, ringset.GetRings(sharedBond).Count());
            Assert.AreEqual(1, ringset.GetRings(ring2Bond1).Count());
            Assert.AreEqual(1, ringset.GetRings(ring2Bond2).Count());
            Assert.AreEqual(1, ringset.GetRings(ring2Bond3).Count());
        }

        [TestMethod()]
        public virtual void TestGetRings_IAtom()
        {
            IRingSet ringset = (IRingSet)NewChemObject();

            IAtom ring1Atom1 = ringset.Builder.NewAtom("C"); // rather artificial molecule
            IAtom ring1Atom2 = ringset.Builder.NewAtom("C");
            IAtom sharedAtom1 = ringset.Builder.NewAtom("C");
            IAtom sharedAtom2 = ringset.Builder.NewAtom("C");
            IAtom ring2Atom1 = ringset.Builder.NewAtom("C");
            IAtom ring2Atom2 = ringset.Builder.NewAtom("C");
            IBond ring1Bond1 = ringset.Builder.NewBond(ring1Atom1, ring1Atom2);
            IBond ring1Bond2 = ringset.Builder.NewBond(sharedAtom1, ring1Atom1);
            IBond ring1Bond3 = ringset.Builder.NewBond(sharedAtom2, ring1Atom2);
            IBond sharedBond = ringset.Builder.NewBond(sharedAtom1, sharedAtom2);
            IBond ring2Bond1 = ringset.Builder.NewBond(ring2Atom1, ring2Atom2);
            IBond ring2Bond2 = ringset.Builder.NewBond(sharedAtom1, ring2Atom1);
            IBond ring2Bond3 = ringset.Builder.NewBond(sharedAtom2, ring2Atom2);

            IRing ring1 = ringset.Builder.NewRing();
            ring1.Atoms.Add(ring1Atom1);
            ring1.Atoms.Add(ring1Atom2);
            ring1.Atoms.Add(sharedAtom1);
            ring1.Atoms.Add(sharedAtom2);
            ring1.Bonds.Add(ring1Bond1);
            ring1.Bonds.Add(ring1Bond2);
            ring1.Bonds.Add(ring1Bond3);
            ring1.Bonds.Add(sharedBond);
            IRing ring2 = ringset.Builder.NewRing();
            ring2.Atoms.Add(ring2Atom1);
            ring2.Atoms.Add(ring2Atom2);
            ring2.Atoms.Add(sharedAtom1);
            ring2.Atoms.Add(sharedAtom2);
            ring2.Bonds.Add(ring2Bond1);
            ring2.Bonds.Add(ring2Bond2);
            ring2.Bonds.Add(ring2Bond3);
            ring2.Bonds.Add(sharedBond);

            ringset.Add(ring1);
            ringset.Add(ring2);

            Assert.AreEqual(1, ringset.GetRings(ring1Atom1).Count());
            Assert.AreEqual(1, ringset.GetRings(ring1Atom1).Count());
            Assert.AreEqual(2, ringset.GetRings(sharedAtom1).Count());
            Assert.AreEqual(2, ringset.GetRings(sharedAtom2).Count());
            Assert.AreEqual(1, ringset.GetRings(ring2Atom1).Count());
            Assert.AreEqual(1, ringset.GetRings(ring2Atom2).Count());
        }

        [TestMethod()]
        public virtual void TestGetConnectedRings_IRing()
        {
            IRingSet ringset = (IRingSet)NewChemObject();

            IAtom ring1Atom1 = ringset.Builder.NewAtom("C"); // rather artificial molecule
            IAtom ring1Atom2 = ringset.Builder.NewAtom("C");
            IAtom sharedAtom1 = ringset.Builder.NewAtom("C");
            IAtom sharedAtom2 = ringset.Builder.NewAtom("C");
            IAtom ring2Atom1 = ringset.Builder.NewAtom("C");
            IAtom ring2Atom2 = ringset.Builder.NewAtom("C");
            IBond ring1Bond1 = ringset.Builder.NewBond(ring1Atom1, ring1Atom2);
            IBond ring1Bond2 = ringset.Builder.NewBond(sharedAtom1, ring1Atom1);
            IBond ring1Bond3 = ringset.Builder.NewBond(sharedAtom2, ring1Atom2);
            IBond sharedBond = ringset.Builder.NewBond(sharedAtom1, sharedAtom2);
            IBond ring2Bond1 = ringset.Builder.NewBond(ring2Atom1, ring2Atom2);
            IBond ring2Bond2 = ringset.Builder.NewBond(sharedAtom1, ring2Atom1);
            IBond ring2Bond3 = ringset.Builder.NewBond(sharedAtom2, ring2Atom2);

            IRing ring1 = ringset.Builder.NewRing();
            ring1.Atoms.Add(ring1Atom1);
            ring1.Atoms.Add(ring1Atom2);
            ring1.Atoms.Add(sharedAtom1);
            ring1.Atoms.Add(sharedAtom2);
            ring1.Bonds.Add(ring1Bond1);
            ring1.Bonds.Add(ring1Bond2);
            ring1.Bonds.Add(ring1Bond3);
            ring1.Bonds.Add(sharedBond);
            IRing ring2 = ringset.Builder.NewRing();
            ring2.Atoms.Add(ring2Atom1);
            ring2.Atoms.Add(ring2Atom2);
            ring2.Atoms.Add(sharedAtom1);
            ring2.Atoms.Add(sharedAtom2);
            ring2.Bonds.Add(ring2Bond1);
            ring2.Bonds.Add(ring2Bond2);
            ring2.Bonds.Add(ring2Bond3);
            ring2.Bonds.Add(sharedBond);

            ringset.Add(ring1);
            ringset.Add(ring2);

            Assert.AreEqual(1, ringset.GetConnectedRings(ring2).Count());
            Assert.AreEqual(1, ringset.GetConnectedRings(ring1).Count());
        }

        /// <summary>
        /// Test for RingSetTest bug #1772613.
        /// When using method GetConnectedRings(...) of RingSet.java fused or bridged rings
        /// returned a list of connected rings that contained duplicates.
        /// Bug fix by Andreas Schueller <a.schueller@chemie.uni-frankfurt.de>
        /// </summary>
        // @cdk.bug 1772613
        [TestMethod()]
        public virtual void TestGetConnectedRingsBug1772613()
        {
            // Build a bridged and fused norbomane like ring system
            // C1CCC2C(C1)C4CC2C3CCCCC34
            IRingSet ringSet = (IRingSet)NewChemObject();
            IRing leftCyclohexane = ringSet.Builder.NewRing(6, "C");
            IRing rightCyclopentane = ringSet.Builder.NewRing(5, "C");

            IRing leftCyclopentane = ringSet.Builder.NewRing();
            IBond leftCyclohexane0RightCyclopentane4 = ringSet.Builder.NewBond(leftCyclohexane.Atoms[0], rightCyclopentane.Atoms[4]);
            IBond leftCyclohexane1RightCyclopentane2 = ringSet.Builder.NewBond(leftCyclohexane.Atoms[1], rightCyclopentane.Atoms[2]);
            leftCyclopentane.Atoms.Add(leftCyclohexane.Atoms[0]);
            leftCyclopentane.Atoms.Add(leftCyclohexane.Atoms[1]);
            leftCyclopentane.Atoms.Add(rightCyclopentane.Atoms[2]);
            leftCyclopentane.Atoms.Add(rightCyclopentane.Atoms[3]);
            leftCyclopentane.Atoms.Add(rightCyclopentane.Atoms[4]);
            leftCyclopentane.Bonds.Add(leftCyclohexane.GetBond(leftCyclohexane.Atoms[0], leftCyclohexane.Atoms[1]));
            leftCyclopentane.Bonds.Add(leftCyclohexane1RightCyclopentane2);
            leftCyclopentane.Bonds.Add(rightCyclopentane.GetBond(rightCyclopentane.Atoms[2], rightCyclopentane.Atoms[3]));
            leftCyclopentane.Bonds.Add(rightCyclopentane.GetBond(rightCyclopentane.Atoms[3], rightCyclopentane.Atoms[4]));
            leftCyclopentane.Bonds.Add(leftCyclohexane0RightCyclopentane4);

            IRing rightCyclohexane = ringSet.Builder.NewRing();
            IAtom rightCyclohexaneAtom0 = ringSet.Builder.NewAtom("C");
            IAtom rightCyclohexaneAtom1 = ringSet.Builder.NewAtom("C");
            IAtom rightCyclohexaneAtom2 = ringSet.Builder.NewAtom("C");
            IAtom rightCyclohexaneAtom5 = ringSet.Builder.NewAtom("C");
            IBond rightCyclohexaneAtom0Atom1 = ringSet.Builder.NewBond(rightCyclohexaneAtom0,
                    rightCyclohexaneAtom1);
            IBond rightCyclohexaneAtom1Atom2 = ringSet.Builder.NewBond(rightCyclohexaneAtom1,
                    rightCyclohexaneAtom2);
            IBond rightCyclohexane2rightCyclopentane1 = ringSet.Builder.NewBond(rightCyclohexaneAtom2, rightCyclopentane.Atoms[1]);
            IBond rightCyclohexane5rightCyclopentane0 = ringSet.Builder.NewBond(rightCyclohexaneAtom5, rightCyclopentane.Atoms[0]);
            IBond rightCyclohexaneAtom0Atom5 = ringSet.Builder.NewBond(rightCyclohexaneAtom0,
                    rightCyclohexaneAtom5);
            rightCyclohexane.Atoms.Add(rightCyclohexaneAtom0);
            rightCyclohexane.Atoms.Add(rightCyclohexaneAtom1);
            rightCyclohexane.Atoms.Add(rightCyclohexaneAtom2);
            rightCyclohexane.Atoms.Add(rightCyclopentane.Atoms[1]);
            rightCyclohexane.Atoms.Add(rightCyclopentane.Atoms[0]);
            rightCyclohexane.Atoms.Add(rightCyclohexaneAtom5);
            rightCyclohexane.Bonds.Add(rightCyclohexaneAtom0Atom1);
            rightCyclohexane.Bonds.Add(rightCyclohexaneAtom1Atom2);
            rightCyclohexane.Bonds.Add(rightCyclohexane2rightCyclopentane1);
            rightCyclohexane.Bonds.Add(rightCyclopentane.GetBond(rightCyclopentane.Atoms[0], rightCyclopentane.Atoms[1]));
            rightCyclohexane.Bonds.Add(rightCyclohexane5rightCyclopentane0);
            rightCyclohexane.Bonds.Add(rightCyclohexaneAtom0Atom5);

            ringSet.Add(leftCyclohexane);
            ringSet.Add(leftCyclopentane);
            ringSet.Add(rightCyclopentane);
            ringSet.Add(rightCyclohexane);

            // Get connected rings
            var connectedRings = ringSet.GetConnectedRings(leftCyclohexane);

            // Iterate over the connectedRings and fail if any duplicate is found
            IList<IRing> foundRings = new List<IRing>();
            foreach (var container in connectedRings)
            {
                IRing connectedRing = (IRing)container;
                if (foundRings.Contains(connectedRing))
                    Assert.Fail("The list of connected rings contains duplicates.");
                foundRings.Add(connectedRing);
            }
        }

        [TestMethod()]
        public override void TestIsEmpty()
        {
            IRingSet ringSet = (IRingSet)NewChemObject();
            Assert.IsTrue(ringSet.IsEmpty(), "new ringset should be empty");
            ringSet.Add(ringSet.Builder.NewRing());    // NCDK does not suppor to add AtomContainer object to RingSet object
            Assert.IsFalse(ringSet.IsEmpty(), "ringset with an atom container should not be empty");
            ringSet.Clear();            
            Assert.IsTrue(ringSet.IsEmpty(), "ringset with removed atom containers should be empty");
        }
    }
}
