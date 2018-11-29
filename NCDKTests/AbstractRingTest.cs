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
using NCDK.Tools.Manipulator;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IRing"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    public abstract class AbstractRingTest
        : AbstractAtomContainerTest
    {
        [TestMethod()]
        public virtual void TestGetBondOrderSum()
        {
            IChemObject obj = NewChemObject();
            IRing r = obj.Builder.NewRing(5, "C");
            Assert.AreEqual(5, r.GetBondOrderSum());

            BondManipulator.IncreaseBondOrder(r.Bonds[0]);
            Assert.AreEqual(6, r.GetBondOrderSum());

            BondManipulator.IncreaseBondOrder(r.Bonds[0]);
            Assert.AreEqual(7, r.GetBondOrderSum());

            BondManipulator.IncreaseBondOrder(r.Bonds[4]);
            Assert.AreEqual(8, r.GetBondOrderSum());
        }

        [TestMethod()]
        public virtual void TestGetRingSize()
        {
            IChemObject obj = NewChemObject();
            IRing r = obj.Builder.NewRing(5, "C");
            Assert.AreEqual(5, r.RingSize);
        }

        [TestMethod()]
        public virtual void TestGetNextBond_IBond_IAtom()
        {
            IRing ring = (IRing)NewChemObject();
            IAtom c1 = ring.Builder.NewAtom("C");
            IAtom c2 = ring.Builder.NewAtom("C");
            IAtom c3 = ring.Builder.NewAtom("C");
            IBond b1 = ring.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = ring.Builder.NewBond(c3, c2, BondOrder.Single);
            IBond b3 = ring.Builder.NewBond(c1, c3, BondOrder.Single);
            ring.Atoms.Add(c1);
            ring.Atoms.Add(c2);
            ring.Atoms.Add(c3);
            ring.Bonds.Add(b1);
            ring.Bonds.Add(b2);
            ring.Bonds.Add(b3);

            Assert.AreEqual(b1, ring.GetNextBond(b2, c2));
            Assert.AreEqual(b1, ring.GetNextBond(b3, c1));
            Assert.AreEqual(b2, ring.GetNextBond(b1, c2));
            Assert.AreEqual(b2, ring.GetNextBond(b3, c3));
            Assert.AreEqual(b3, ring.GetNextBond(b1, c1));
            Assert.AreEqual(b3, ring.GetNextBond(b2, c3));
        }

        [TestMethod()]

        public override void TestToString()
        {
            IChemObject obj = NewChemObject();
            IRing r = obj.Builder.NewRing(5, "C");
            string description = r.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }
    }
}
