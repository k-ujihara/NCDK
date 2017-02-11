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

using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK;
using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NCDK
{
    /**
     * Checks the functionality of {@link IBond} implementations.
     *
     * @cdk.module test-interfaces
     */
    [TestClass()]
    public abstract class AbstractBondTest
        : AbstractElectronContainerTest
    {
        [TestMethod()]
        public override void TestCompare_Object()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            IBond b2 = (IBond)NewChemObject();
            b2.SetAtoms(new[] { c, o });
            b2.Order = BondOrder.Single;

            Assert.IsTrue(b.Compare(b2));
        }

        [TestMethod()]
        public virtual void TestContains_IAtom()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            Assert.IsTrue(b.Contains(c));
            Assert.IsTrue(b.Contains(o));
        }

        [TestMethod()]
        public virtual void TestGetAtomCount()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            Assert.AreEqual(2.0, b.Atoms.Count, 0.001);
        }

        [TestMethod()]
        public virtual void TestSetAtoms_arrayIAtom()
        {
            IBond b = (IBond)NewChemObject();

            IAtom[] atomsToAdd = new IAtom[2];
            atomsToAdd[0] = b.Builder.CreateAtom("C");
            atomsToAdd[1] = b.Builder.CreateAtom("O");
            b.SetAtoms(atomsToAdd);

            Assert.AreEqual(2, b.Atoms.Count);
            Assert.AreEqual(atomsToAdd[0], b.Atoms[0]);
            Assert.AreEqual(atomsToAdd[1], b.Atoms[1]);
        }

        [TestMethod()]
        public virtual void TestSetAtom_SomeNull()
        {
            IBond b = (IBond)NewChemObject();
            b.Atoms.Add(b.Builder.CreateAtom("C"));
            Assert.AreEqual(1, b.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestUnSetAtom()
        {
            IBond b = (IBond)NewChemObject();
            b.Atoms.Add(b.Builder.CreateAtom("C"));
            Assert.AreEqual(1, b.Atoms.Count);
            b.Atoms[0] = b.Builder.CreateAtom("C");
            Assert.AreEqual(1, b.Atoms.Count);
            //b.Atoms[0] = null;
            //Assert.AreEqual(0, b.Atoms.Count);
            //b.Atoms[0] = null;
            //Assert.AreEqual(0, b.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestOverwriteAtom()
        {
            IBond b = (IBond)NewChemObject();
            b.Atoms.Add(b.Builder.CreateAtom("C"));
            Assert.AreEqual(1, b.Atoms.Count);
            b.Atoms[0] = b.Builder.CreateAtom("C");
            Assert.AreEqual(1, b.Atoms.Count);

            //// test overwrite with null
            //b.Atoms[0] = null;
            //Assert.AreEqual(0, b.Atoms.Count);
            //b.Atoms[0] = null;
            //Assert.AreEqual(0, b.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestAtoms()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            IEnumerator<IAtom> atoms = b.Atoms.GetEnumerator();
            Assert.AreEqual(2, b.Atoms.Count);
            Assert.IsTrue(atoms.MoveNext());
            Assert.AreEqual(c, atoms.Current);
            Assert.IsTrue(atoms.MoveNext());
            Assert.AreEqual(o, atoms.Current);
            Assert.IsFalse(atoms.MoveNext());
        }

        [TestMethod()]
        public virtual void TestGetAtom_int()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            Assert.AreEqual(c, b.Atoms[0]);
            Assert.AreEqual(o, b.Atoms[1]);
        }

        [TestMethod()]
        public virtual void TestSetAtom_IAtom_int()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");

            b.SetAtoms(new[] { c, o });

            Assert.AreEqual(c, b.Atoms[0]);
            Assert.AreEqual(o, b.Atoms[1]);
        }

        [TestMethod()]
        public virtual void TestGetConnectedAtom_IAtom()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            Assert.AreEqual(c, b.GetConnectedAtom(o));
            Assert.AreEqual(o, b.GetConnectedAtom(c));

            // test default return value
            Assert.IsNull(b.GetConnectedAtom(b.Builder.CreateAtom()));
        }

        [TestMethod()]
        public virtual void TestGetConnectedAtoms_IAtom()
        {
            IBond b = (IBond)NewChemObject();
            IAtom[] atoms = new IAtom[3];
            atoms[0] = b.Builder.CreateAtom("B");
            atoms[1] = b.Builder.CreateAtom("H");
            atoms[2] = b.Builder.CreateAtom("B");
            b.SetAtoms(atoms);
            b.Order = BondOrder.Single; // C=O bond

            IEnumerable<IAtom> connectedAtoms = b.GetConnectedAtoms(atoms[1]);
            Assert.IsNotNull(connectedAtoms);
            Assert.AreEqual(2, connectedAtoms.Count());
            Assert.IsNotNull(connectedAtoms.ElementAt(0));
            Assert.IsNotNull(connectedAtoms.ElementAt(1));

            // test default return value
            connectedAtoms = b.GetConnectedAtoms(b.Builder.CreateAtom());
            Assert.IsNull(connectedAtoms);
        }

        [TestMethod()]
        public virtual void TestIsConnectedTo_IBond()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c1 = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");
            IAtom c2 = b.Builder.CreateAtom("C");
            IAtom c3 = b.Builder.CreateAtom("C");

            IBond b1 = b.Builder.CreateBond(c1, o);
            IBond b2 = b.Builder.CreateBond(o, c2);
            IBond b3 = b.Builder.CreateBond(c2, c3);

            Assert.IsTrue(b1.IsConnectedTo(b2));
            Assert.IsTrue(b2.IsConnectedTo(b1));
            Assert.IsTrue(b2.IsConnectedTo(b3));
            Assert.IsTrue(b3.IsConnectedTo(b2));
            Assert.IsFalse(b1.IsConnectedTo(b3));
            Assert.IsFalse(b3.IsConnectedTo(b1));
        }

        [TestMethod()]
        public virtual void TestGetOrder()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Double;

            Assert.AreEqual(BondOrder.Double, b.Order);
        }

        [TestMethod()]
        public virtual void TestSetOrder_BondOrder()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Double;

            Assert.AreEqual(BondOrder.Double, b.Order);

            b.Order = BondOrder.Single;
            Assert.AreEqual(BondOrder.Single, b.Order);
        }

        [TestMethod()]
        public virtual void TestSetOrder_electronCounts()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("C");

            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;
            Assert.IsNotNull(b.ElectronCount);
            Assert.AreEqual(2, b.ElectronCount.Value);

            b.Atoms[0] = c;
            b.Atoms[1] = o;
            b.Order = BondOrder.Double;
            Assert.IsNotNull(b.ElectronCount);
            Assert.AreEqual(4, b.ElectronCount.Value);

            b.Atoms[0] = c;
            b.Atoms[1] = o;
            b.Order = BondOrder.Triple;
            Assert.IsNotNull(b.ElectronCount);
            Assert.AreEqual(6, b.ElectronCount.Value);

            // OK, a bit hypothetical
            b.Atoms[0] = c;
            b.Atoms[1] = o;
            b.Order = BondOrder.Quadruple;
            Assert.IsNotNull(b.ElectronCount);
            Assert.AreEqual(8, b.ElectronCount.Value);

            // OK, a bit hypothetical
            b.Atoms[0] = c;
            b.Atoms[1] = o;
            b.Order = BondOrder.Quintuple;
            Assert.IsNotNull(b.ElectronCount);
            Assert.AreEqual(10, b.ElectronCount.Value);

            // OK, a bit hypothetical
            b.Atoms[0] = c;
            b.Atoms[1] = o;
            b.Order = BondOrder.Sextuple;
            Assert.IsNotNull(b.ElectronCount);
            Assert.AreEqual(12, b.ElectronCount.Value);
        }

        [TestMethod()]
        public virtual void TestSetStereo_IBond_Stereo()
        {
            IBond b = (IBond)NewChemObject();
            IAtom c = b.Builder.CreateAtom("C");
            IAtom o = b.Builder.CreateAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Double;
            b.Stereo = BondStereo.Down;
            Assert.AreEqual(BondStereo.Down, b.Stereo);
            b.Stereo = BondStereo.Up;
            Assert.AreEqual(BondStereo.Up, b.Stereo);
        }

        [TestMethod()]
        public virtual void TestGetStereo()
        {
            IChemObject obj = NewChemObject();
            IAtom c = obj.Builder.CreateAtom("C");
            IAtom o = obj.Builder.CreateAtom("O");

            IBond b = obj.Builder.CreateBond(c, o, BondOrder.Double, BondStereo.Up);
            Assert.AreEqual(BondStereo.Up, b.Stereo);
        }

        [TestMethod()]
        public virtual void TestGet2DCenter()
        {
            IChemObject obj = NewChemObject();
            IAtom o = obj.Builder.CreateAtom("O", Vector2.Zero);
            IAtom c = obj.Builder.CreateAtom("C", new Vector2(1, 1));
            IBond b = obj.Builder.CreateBond(c, o);

            Assert.AreEqual(0.5, b.Geometric2DCenter.X, 0.001);
            Assert.AreEqual(0.5, b.Geometric2DCenter.Y, 0.001);
        }

        [TestMethod()]
        public virtual void TestGet3DCenter()
        {
            IChemObject obj = NewChemObject();
            IAtom o = obj.Builder.CreateAtom("O", Vector3.Zero);
            IAtom c = obj.Builder.CreateAtom("C", new Vector3(1, 1, 1));
            IBond b = obj.Builder.CreateBond(c, o);

            Assert.AreEqual(0.5, b.Geometric3DCenter.X, 0.001);
            Assert.AreEqual(0.5, b.Geometric3DCenter.Y, 0.001);
            Assert.AreEqual(0.5, b.Geometric3DCenter.Z, 0.001);
        }

        [TestMethod()]

        public override void TestClone()
        {
            IBond bond = (IBond)NewChemObject();
            object clone = bond.Clone();
            Assert.IsNotNull(clone);
            Assert.IsTrue(clone is IBond);
        }

        [TestMethod()]
        public virtual void TestClone_IAtom()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IBond bond = obj.Builder.CreateBond(atom1, atom2);
            IBond clone = (IBond)bond.Clone();

            // test cloning of atoms
            Assert.AreNotSame(atom1, clone.Atoms[0]);
            Assert.AreNotSame(atom2, clone.Atoms[1]);
        }

        [TestMethod()]
        public virtual void TestClone_Order()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IBond bond = obj.Builder.CreateBond(atom1, atom2, BondOrder.Single);
            IBond clone = (IBond)bond.Clone();

            // test cloning of bond order
            bond.Order = BondOrder.Double;
            Assert.AreEqual(BondOrder.Single, clone.Order);
        }

        [TestMethod()]
        public virtual void TestClone_Stereo()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IBond bond = obj.Builder.CreateBond(atom1, atom2, BondOrder.Single, BondStereo.Up);
            IBond clone = (IBond)bond.Clone();

            // test cloning of bond order
            bond.Stereo = BondStereo.UpInverted;
            Assert.AreEqual(BondStereo.Up, clone.Stereo);
        }

        /**
         * Test for RFC #9
         */
        [TestMethod()]

        public override void TestToString()
        {
            IBond bond = (IBond)NewChemObject();
            string description = bond.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]
        public virtual void TestMultiCenter1()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IAtom atom3 = obj.Builder.CreateAtom("C");

            IBond bond = obj.Builder.CreateBond(new IAtom[] { atom1, atom2, atom3 });
            Assert.AreEqual(3, bond.Atoms.Count);
            Assert.AreEqual(atom1, bond.Atoms[0]);
            Assert.AreEqual(atom2, bond.Atoms[1]);
            Assert.AreEqual(atom3, bond.Atoms[2]);

            Assert.AreEqual(BondOrder.Unset, bond.Order);
        }

        [TestMethod()]
        public virtual void TestMultiCenterCompare()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IAtom atom3 = obj.Builder.CreateAtom("C");

            IBond bond1 = obj.Builder.CreateBond(new IAtom[] { atom1, atom2, atom3 });
            IBond bond2 = obj.Builder.CreateBond(new IAtom[] { atom1, atom2, atom3 });

            Assert.IsTrue(bond1.Compare(bond2));

            IAtom atom4 = obj.Builder.CreateAtom("C");
            IBond bond3 = obj.Builder.CreateBond(new IAtom[] { atom1, atom2, atom4 });
            Assert.IsFalse(bond1.Compare(bond3));
        }

        [TestMethod()]
        public virtual void TestMultiCenterContains()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IAtom atom3 = obj.Builder.CreateAtom("C");
            IAtom atom4 = obj.Builder.CreateAtom("C");

            IBond bond1 = obj.Builder.CreateBond(new IAtom[] { atom1, atom2, atom3 });
            Assert.IsTrue(bond1.Contains(atom1));
            Assert.IsTrue(bond1.Contains(atom2));
            Assert.IsTrue(bond1.Contains(atom3));
            Assert.IsFalse(bond1.Contains(atom4));
        }

        [TestMethod()]
        public virtual void TestMultiCenterIterator()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IAtom atom3 = obj.Builder.CreateAtom("C");
            IAtom atom4 = obj.Builder.CreateAtom("C");

            IBond bond1 = obj.Builder.CreateBond(new IAtom[] { atom1, atom2, atom3, atom4 });
            IEnumerable<IAtom> atoms = bond1.Atoms;
            int natom = 0;
            foreach (var atom in atoms)
            {
                Assert.IsNotNull(atom);
                natom++;
            }
            Assert.AreEqual(4, natom);
        }

        [TestMethod()]
        public virtual void TestMultiCenterConnectedAtoms()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IAtom atom3 = obj.Builder.CreateAtom("C");
            IAtom atom4 = obj.Builder.CreateAtom("C");

            IBond bond1 = obj.Builder.CreateBond(new IAtom[] { atom1, atom2, atom3, atom4 });
            Assert.AreEqual(atom2, bond1.GetConnectedAtom(atom1));
            Assert.IsNull(bond1.GetConnectedAtom(obj.Builder.CreateAtom()));

            IEnumerable<IAtom> conAtoms = bond1.GetConnectedAtoms(atom1);
            bool correct = true;
            foreach (var atom in conAtoms)
            {
                if (atom == atom1)
                {
                    correct = false;
                    break;
                }
            }
            Assert.IsTrue(correct);

            conAtoms = bond1.GetConnectedAtoms(atom3);
            correct = true;
            foreach (var atom in conAtoms)
            {
                if (atom == atom3)
                {
                    correct = false;
                    break;
                }
            }
            Assert.IsTrue(correct);
        }

        [TestMethod()]
        public virtual void TestMultiCenterIsConnectedTo()
        {
            IChemObject obj = NewChemObject();
            IAtom atom1 = obj.Builder.CreateAtom("C");
            IAtom atom2 = obj.Builder.CreateAtom("O");
            IAtom atom3 = obj.Builder.CreateAtom("C");
            IAtom atom4 = obj.Builder.CreateAtom("C");
            IAtom atom5 = obj.Builder.CreateAtom("C");

            IBond bond1 = obj.Builder.CreateBond(new IAtom[] { atom1, atom2, atom3 });
            IBond bond2 = obj.Builder.CreateBond(new IAtom[] { atom2, atom3, atom4 });
            IBond bond3 = obj.Builder.CreateBond(new IAtom[] { atom2, atom4 });
            IBond bond4 = obj.Builder.CreateBond(new IAtom[] { atom5, atom4 });

            Assert.IsTrue(bond1.IsConnectedTo(bond2));
            Assert.IsTrue(bond2.IsConnectedTo(bond1));
            Assert.IsTrue(bond1.IsConnectedTo(bond3));
            Assert.IsFalse(bond4.IsConnectedTo(bond1));
        }
    }
}
