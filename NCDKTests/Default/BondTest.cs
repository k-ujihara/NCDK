/* Copyright (C) 2002-2007  The Chemistry Development Kit (CDK) project
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
 *
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Default
{
    /**
     * Checks the functionality of the Bond class.
     *
     * @cdk.module test-data
     * @see org.openscience.cdk.Bond
     */
    [TestClass()]
    public class BondTest : AbstractBondTest
    {
        public override IChemObject NewChemObject()
        {
            return new Bond();
        }

        [TestMethod()]
        public virtual void TestBond()
        {
            IBond bond = new Bond();
            Assert.AreEqual(0, bond.Atoms.Count);
            //Assert.IsNull(bond.Atoms[0]);
            //Assert.IsNull(bond.Atoms[1]);
            Assert.AreEqual(default(BondOrder), bond.Order);
            Assert.AreEqual(default(BondStereo), bond.Stereo);
            Assert.AreEqual(BondOrder.Unset, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        [TestMethod()]
        public virtual void TestBond_arrayIAtom()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IAtom atom3 = obj.Builder.CreateAtom("C");
            IAtom atom4 = obj.Builder.CreateAtom("C");
            IAtom atom5 = obj.Builder.CreateAtom("C");

            IBond bond1 = new Bond(new IAtom[] { atom1, atom2, atom3, atom4, atom5 });
            Assert.AreEqual(5, bond1.Atoms.Count);
            Assert.AreEqual(atom1, bond1.Atoms[0]);
            Assert.AreEqual(atom2, bond1.Atoms[1]);
        }

        [TestMethod()]
        public virtual void TestBond_arrayIAtom_BondOrder()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IAtom atom3 = obj.Builder.CreateAtom("C");
            IAtom atom4 = obj.Builder.CreateAtom("C");
            IAtom atom5 = obj.Builder.CreateAtom("C");

            IBond bond1 = new Bond(new IAtom[] { atom1, atom2, atom3, atom4, atom5 }, BondOrder.Single);
            Assert.AreEqual(5, bond1.Atoms.Count);
            Assert.AreEqual(atom1, bond1.Atoms[0]);
            Assert.AreEqual(atom2, bond1.Atoms[1]);
            Assert.AreEqual(BondOrder.Single, bond1.Order);
        }

        [TestMethod()]
        public virtual void TestBond_IAtom_IAtom()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.CreateAtom("C");
            IAtom o = obj.Builder.CreateAtom("O");
            IBond bond = new Bond(c, o);

            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(c, bond.Atoms[0]);
            Assert.AreEqual(o, bond.Atoms[1]);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        [TestMethod()]
        public virtual void TestBond_IAtom_IAtom_BondOrder()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.CreateAtom("C");
            IAtom o = obj.Builder.CreateAtom("O");
            IBond bond = new Bond(c, o, BondOrder.Double);

            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(c, bond.Atoms[0]);
            Assert.AreEqual(o, bond.Atoms[1]);
            Assert.IsTrue(bond.Order == BondOrder.Double);
            Assert.AreEqual(BondStereo.None, bond.Stereo);
        }

        [TestMethod()]
        public virtual void TestBond_IAtom_IAtom_BondOrder_IBond_Stereo()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.CreateAtom("C");
            IAtom o = obj.Builder.CreateAtom("O");
            IBond bond = new Bond(c, o, BondOrder.Single, BondStereo.Up);

            Assert.AreEqual(2, bond.Atoms.Count);
            Assert.AreEqual(c, bond.Atoms[0]);
            Assert.AreEqual(o, bond.Atoms[1]);
            Assert.IsTrue(bond.Order == BondOrder.Single);
            Assert.AreEqual(BondStereo.Up, bond.Stereo);
        }

        [TestMethod()]
        public override void TestCompare_Object()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.CreateAtom("C");
            IAtom o = obj.Builder.CreateAtom("O");

            IBond b = new Bond(c, o, BondOrder.Double); // C=O bond
            IBond b2 = new Bond(c, o, BondOrder.Double); // same C=O bond

            Assert.IsTrue(b.Compare(b2));
        }
    }
}
