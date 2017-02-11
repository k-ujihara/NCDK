/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Config;
using NCDK.Default;
using System;
using System.Collections.Generic;

namespace NCDK.Tools.Manipulator
{
    /**
     * @cdk.module test-core
     */
    [TestClass()]
    public class BondManipulatorTest : CDKTestCase
    {

        [TestMethod()]
        public void TestGetAtomArray_IBond()
        {
            IAtom atom1 = new Atom(Elements.Carbon.ToIElement());
            IAtom atom2 = new Atom(Elements.Carbon.ToIElement());
            IBond bond = new Bond(atom1, atom2, BondOrder.Triple);
            IAtom[] atoms = BondManipulator.GetAtomArray(bond);
            Assert.AreEqual(2, atoms.Length);
            Assert.AreEqual(atom1, atoms[0]);
            Assert.AreEqual(atom2, atoms[1]);
        }

        [TestMethod()]
        public void TestIsHigherOrder_BondOrder_BondOrder()
        {
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Single, BondOrder.Single));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Single, BondOrder.Double));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Single, BondOrder.Triple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Single, BondOrder.Quadruple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Single, BondOrder.Quintuple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Single, BondOrder.Sextuple));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Double, BondOrder.Single));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Double, BondOrder.Double));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Double, BondOrder.Triple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Double, BondOrder.Quadruple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Double, BondOrder.Quintuple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Double, BondOrder.Sextuple));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Triple, BondOrder.Single));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Triple, BondOrder.Double));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Triple, BondOrder.Triple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Triple, BondOrder.Quadruple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Triple, BondOrder.Quintuple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Triple, BondOrder.Sextuple));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Quadruple, BondOrder.Single));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Quadruple, BondOrder.Double));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Quadruple, BondOrder.Triple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Quadruple, BondOrder.Quadruple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Quadruple, BondOrder.Quintuple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Quadruple, BondOrder.Sextuple));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Quintuple, BondOrder.Single));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Quintuple, BondOrder.Double));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Quintuple, BondOrder.Triple));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Quintuple, BondOrder.Quadruple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Quintuple, BondOrder.Quintuple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Quintuple, BondOrder.Sextuple));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Sextuple, BondOrder.Single));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Sextuple, BondOrder.Double));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Sextuple, BondOrder.Triple));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Sextuple, BondOrder.Quadruple));
            Assert.IsTrue(BondManipulator.IsHigherOrder(BondOrder.Sextuple, BondOrder.Quintuple));
            Assert.IsFalse(BondManipulator.IsHigherOrder(BondOrder.Sextuple, BondOrder.Sextuple));
        }

        [TestMethod()]
        public void TestIsLowerOrder_BondOrder_BondOrder()
        {
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Single, BondOrder.Single));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Single, BondOrder.Double));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Single, BondOrder.Triple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Single, BondOrder.Quadruple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Single, BondOrder.Quintuple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Single, BondOrder.Sextuple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Double, BondOrder.Single));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Double, BondOrder.Double));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Double, BondOrder.Triple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Double, BondOrder.Quadruple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Double, BondOrder.Quintuple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Double, BondOrder.Sextuple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Triple, BondOrder.Single));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Triple, BondOrder.Double));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Triple, BondOrder.Triple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Triple, BondOrder.Quadruple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Triple, BondOrder.Quintuple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Triple, BondOrder.Sextuple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Quadruple, BondOrder.Single));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Quadruple, BondOrder.Double));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Quadruple, BondOrder.Triple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Quadruple, BondOrder.Quadruple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Quadruple, BondOrder.Quintuple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Quadruple, BondOrder.Sextuple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Quintuple, BondOrder.Single));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Quintuple, BondOrder.Double));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Quintuple, BondOrder.Triple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Quintuple, BondOrder.Quadruple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Quintuple, BondOrder.Quintuple));
            Assert.IsTrue(BondManipulator.IsLowerOrder(BondOrder.Quintuple, BondOrder.Sextuple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Sextuple, BondOrder.Single));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Sextuple, BondOrder.Double));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Sextuple, BondOrder.Triple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Sextuple, BondOrder.Quadruple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Sextuple, BondOrder.Quintuple));
            Assert.IsFalse(BondManipulator.IsLowerOrder(BondOrder.Sextuple, BondOrder.Sextuple));
        }

        [TestMethod()]
        public void TestIncreaseBondOrder_BondOrder()
        {
            Assert.AreEqual(BondOrder.Double, BondManipulator.IncreaseBondOrder(BondOrder.Single));
            Assert.AreEqual(BondOrder.Triple, BondManipulator.IncreaseBondOrder(BondOrder.Double));
            Assert.AreEqual(BondOrder.Quadruple, BondManipulator.IncreaseBondOrder(BondOrder.Triple));
            Assert.AreEqual(BondOrder.Quintuple, BondManipulator.IncreaseBondOrder(BondOrder.Quadruple));
            Assert.AreEqual(BondOrder.Sextuple, BondManipulator.IncreaseBondOrder(BondOrder.Quintuple));
            Assert.AreEqual(BondOrder.Sextuple, BondManipulator.IncreaseBondOrder(BondOrder.Sextuple));
        }

        [TestMethod()]
        public void TestIncreaseBondOrder_IBond()
        {
            IBond bond = new Bond();
            bond.Order = BondOrder.Single;
            BondManipulator.IncreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Double, bond.Order);
            BondManipulator.IncreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Triple, bond.Order);
            BondManipulator.IncreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Quadruple, bond.Order);
            BondManipulator.IncreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Quintuple, bond.Order);
            BondManipulator.IncreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Sextuple, bond.Order);
            BondManipulator.IncreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Sextuple, bond.Order);
        }

        [TestMethod()]
        public void TestDecreaseBondOrder_BondOrder()
        {
            Assert.AreEqual(BondOrder.Single, BondManipulator.DecreaseBondOrder(BondOrder.Single));
            Assert.AreEqual(BondOrder.Single, BondManipulator.DecreaseBondOrder(BondOrder.Double));
            Assert.AreEqual(BondOrder.Double, BondManipulator.DecreaseBondOrder(BondOrder.Triple));
            Assert.AreEqual(BondOrder.Triple, BondManipulator.DecreaseBondOrder(BondOrder.Quadruple));
            Assert.AreEqual(BondOrder.Quadruple, BondManipulator.DecreaseBondOrder(BondOrder.Quintuple));
            Assert.AreEqual(BondOrder.Quintuple, BondManipulator.DecreaseBondOrder(BondOrder.Sextuple));
        }

        [TestMethod()]
        public void TestDecreaseBondOrder_IBond()
        {
            IBond bond = new Bond();
            bond.Order = BondOrder.Sextuple;
            BondManipulator.DecreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Quintuple, bond.Order);
            BondManipulator.DecreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Quadruple, bond.Order);
            BondManipulator.DecreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Triple, bond.Order);
            BondManipulator.DecreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Double, bond.Order);
            BondManipulator.DecreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Single, bond.Order);
            BondManipulator.DecreaseBondOrder(bond);
            Assert.AreEqual(BondOrder.Single, bond.Order);
        }

        [TestMethod()]
        public void TestDestroyBondOrder_BondOrder()
        {
            Assert.AreEqual(1.0, BondManipulator.DestroyBondOrder(BondOrder.Single), 0.00001);
            Assert.AreEqual(2.0, BondManipulator.DestroyBondOrder(BondOrder.Double), 0.00001);
            Assert.AreEqual(3.0, BondManipulator.DestroyBondOrder(BondOrder.Triple), 0.00001);
            Assert.AreEqual(4.0, BondManipulator.DestroyBondOrder(BondOrder.Quadruple), 0.00001);
            Assert.AreEqual(5.0, BondManipulator.DestroyBondOrder(BondOrder.Quintuple), 0.00001);
            Assert.AreEqual(6.0, BondManipulator.DestroyBondOrder(BondOrder.Sextuple), 0.00001);
        }

        [TestMethod()]
        public void TestGetMaximumBondOrder_List()
        {
            List<IBond> bonds = new List<IBond>();
            IBond bond = new Bond();
            bond.Order = BondOrder.Single;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Quadruple;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Quadruple;
            bonds.Add(bond);
            Assert.AreEqual(BondOrder.Quadruple, BondManipulator.GetMaximumBondOrder(bonds));
        }

        [TestMethod()]
        public void TestGetMaximumBondOrder_Iterator()
        {
            List<IBond> bonds = new List<IBond>();
            IBond bond = new Bond();
            bond.Order = BondOrder.Single;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Quadruple;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Quadruple;
            bonds.Add(bond);
            Assert.AreEqual(BondOrder.Quadruple, BondManipulator.GetMaximumBondOrder(bonds));
        }

        [TestMethod()]
        public void TestGetMaximumBondOrder_IBond_IBond()
        {
            IBond bond1 = new Bond();
            bond1.Order = BondOrder.Single;
            IBond bond2 = new Bond();
            bond2.Order = BondOrder.Quadruple;
            Assert.AreEqual(BondOrder.Quadruple, BondManipulator.GetMaximumBondOrder(bond1, bond2));
        }

        [TestMethod()]
        public void TestGetMaximumBondOrder_IBond_IBond_Unset()
        {
            IBond bond1 = new Bond();
            bond1.Order = BondOrder.Unset;
            IBond bond2 = new Bond();
            bond2.Order = BondOrder.Double;
            Assert.AreEqual(BondOrder.Double, BondManipulator.GetMaximumBondOrder(bond1, bond2));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetMaximumBondOrder_IBond_IBond_null()
        {
            IBond bond1 = new Bond();
            bond1.Order = BondOrder.Unset;
            IBond bond2 = new Bond();
            bond2.Order = BondOrder.Double;
            BondManipulator.GetMaximumBondOrder(null, bond2);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetMaximumBondOrder_UnSet_Unset()
        {
            BondManipulator.GetMaximumBondOrder(BondOrder.Unset, BondOrder.Unset);
        }

        [TestMethod()]
        public void TestGetMaximumBondOrder_Order_Order()
        {
            Assert.AreEqual(BondOrder.Quadruple,
                    BondManipulator.GetMaximumBondOrder(BondOrder.Single, BondOrder.Quadruple));
        }

        [TestMethod()]
        public void TestGetMaximumBondOrder_Order_Order_Single()
        {
            Assert.AreEqual(BondOrder.Single,
                    BondManipulator.GetMaximumBondOrder(BondOrder.Single, BondOrder.Single));
        }

        [TestMethod()]
        public void TestGetMaximumBondOrder_Order_Order_Unset()
        {
            Assert.AreEqual(BondOrder.Single,
                    BondManipulator.GetMaximumBondOrder(BondOrder.Single, BondOrder.Unset));
            Assert.AreEqual(BondOrder.Single,
                    BondManipulator.GetMaximumBondOrder(BondOrder.Unset, BondOrder.Single));
        }

        [TestMethod()]
        public void TestGetMinimumBondOrder_List()
        {
            List<IBond> bonds = new List<IBond>();
            IBond bond = new Bond();
            bond.Order = BondOrder.Double;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Quadruple;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Triple;
            bonds.Add(bond);
            Assert.AreEqual(BondOrder.Double, BondManipulator.GetMinimumBondOrder(bonds));
        }

        [TestMethod()]
        public void TestGetMinimumBondOrder_Iterator()
        {
            List<IBond> bonds = new List<IBond>();
            IBond bond = new Bond();
            bond.Order = BondOrder.Double;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Quadruple;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Triple;
            bonds.Add(bond);
            Assert.AreEqual(BondOrder.Double, BondManipulator.GetMinimumBondOrder(bonds));
        }

        [TestMethod()]
        public void TestGetMinimumBondOrder_HigherOrders()
        {
            List<IBond> bonds = new List<IBond>();
            IBond bond = new Bond();
            bond.Order = BondOrder.Quintuple;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Sextuple;
            bonds.Add(bond);
            Assert.AreEqual(BondOrder.Quintuple, BondManipulator.GetMinimumBondOrder(bonds));
        }

        [TestMethod()]
        public void TestGetSingleBondEquivalentSum_List()
        {
            List<IBond> bonds = new List<IBond>();
            IBond bond = new Bond();
            bond.Order = BondOrder.Single;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Double;
            bonds.Add(bond);
            Assert.AreEqual(3, BondManipulator.GetSingleBondEquivalentSum(bonds));
            bond = new Bond();
            bond.Order = BondOrder.Quadruple;
            bonds.Add(bond);
            Assert.AreEqual(7, BondManipulator.GetSingleBondEquivalentSum(bonds));
        }

        [TestMethod()]
        public void TestGetSingleBondEquivalentSum_Iterator()
        {
            List<IBond> bonds = new List<IBond>();
            IBond bond = new Bond();
            bond.Order = BondOrder.Single;
            bonds.Add(bond);
            bond = new Bond();
            bond.Order = BondOrder.Double;
            bonds.Add(bond);
            Assert.AreEqual(3, BondManipulator.GetSingleBondEquivalentSum(bonds));
            bond = new Bond();
            bond.Order = BondOrder.Quadruple;
            bonds.Add(bond);
            Assert.AreEqual(7, BondManipulator.GetSingleBondEquivalentSum(bonds));
        }

        [TestMethod()]
        public void TestCreateBondOrder_Double()
        {
            Assert.AreEqual(BondOrder.Single, BondManipulator.CreateBondOrder(1.0));
            Assert.AreEqual(BondOrder.Double, BondManipulator.CreateBondOrder(2.0));
            Assert.AreEqual(BondOrder.Triple, BondManipulator.CreateBondOrder(3.0));
            Assert.AreEqual(BondOrder.Quadruple, BondManipulator.CreateBondOrder(4.0));
            Assert.AreEqual(BondOrder.Quintuple, BondManipulator.CreateBondOrder(5.0));
            Assert.AreEqual(BondOrder.Sextuple, BondManipulator.CreateBondOrder(6.0));
        }
    }
}
