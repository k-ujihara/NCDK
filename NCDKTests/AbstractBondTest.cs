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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;
using System.Linq;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IBond"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    public abstract class AbstractBondTest
        : AbstractElectronContainerTest
    {
        [TestMethod()]
        public override void TestCompare_Object()
        {
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            var b2 = (IBond)NewChemObject();
            b2.SetAtoms(new[] { c, o });
            b2.Order = BondOrder.Single;

            Assert.IsTrue(b.Compare(b2));
        }

        [TestMethod()]
        public virtual void TestContains_IAtom()
        {
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            Assert.IsTrue(b.Contains(c));
            Assert.IsTrue(b.Contains(o));
        }

        [TestMethod()]
        public virtual void TestGetAtomCount()
        {
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            Assert.AreEqual(2.0, b.Atoms.Count, 0.001);
        }

        [TestMethod()]
        public virtual void TestSetAtoms_arrayIAtom()
        {
            var b = (IBond)NewChemObject();

            var atomsToAdd = new IAtom[2];
            atomsToAdd[0] = b.Builder.NewAtom("C");
            atomsToAdd[1] = b.Builder.NewAtom("O");
            b.SetAtoms(atomsToAdd);

            Assert.AreEqual(2, b.Atoms.Count);
            Assert.AreEqual(atomsToAdd[0], b.Begin);
            Assert.AreEqual(atomsToAdd[1], b.End);
        }

        [TestMethod()]
        public virtual void TestSetAtom_SomeNull()
        {
            var b = (IBond)NewChemObject();
            b.Atoms.Add(b.Builder.NewAtom("C"));
            Assert.AreEqual(1, b.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestUnSetAtom()
        {
            var b = (IBond)NewChemObject();
            b.Atoms.Add(b.Builder.NewAtom("C"));
            Assert.AreEqual(1, b.Atoms.Count);
            b.Atoms[0] = b.Builder.NewAtom("C");
            Assert.AreEqual(1, b.Atoms.Count);
            //b.Begin = null;
            //Assert.AreEqual(0, b.Atoms.Count);
            //b.Begin = null;
            //Assert.AreEqual(0, b.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestOverwriteAtom()
        {
            var b = (IBond)NewChemObject();
            b.Atoms.Add(b.Builder.NewAtom("C"));
            Assert.AreEqual(1, b.Atoms.Count);
            b.Atoms[0] = b.Builder.NewAtom("C");
            Assert.AreEqual(1, b.Atoms.Count);

            //// test overwrite with null
            //b.Begin = null;
            //Assert.AreEqual(0, b.Atoms.Count);
            //b.Begin = null;
            //Assert.AreEqual(0, b.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestAtoms()
        {
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            var atoms = b.Atoms.GetEnumerator();
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
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            Assert.AreEqual(c, b.Begin);
            Assert.AreEqual(o, b.End);
        }

        [TestMethod()]
        public virtual void TestSetAtom_IAtom_int()
        {
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");

            b.SetAtoms(new[] { c, o });

            Assert.AreEqual(c, b.Begin);
            Assert.AreEqual(o, b.End);
        }

        [TestMethod()]
        public virtual void TestGetConnectedAtom_IAtom()
        {
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Single;

            Assert.AreEqual(c, b.GetOther(o));
            Assert.AreEqual(o, b.GetOther(c));

            // test default return value
            Assert.IsNull(b.GetOther(b.Builder.NewAtom()));
        }

        [TestMethod()]
        public virtual void TestGetConnectedAtoms_IAtom()
        {
            var b = (IBond)NewChemObject();
            var atoms = new IAtom[3];
            atoms[0] = b.Builder.NewAtom("B");
            atoms[1] = b.Builder.NewAtom("H");
            atoms[2] = b.Builder.NewAtom("B");
            b.SetAtoms(atoms);
            b.Order = BondOrder.Single; // C=O bond

            var connectedAtoms = b.GetConnectedAtoms(atoms[1]);
            Assert.IsNotNull(connectedAtoms);
            Assert.AreEqual(2, connectedAtoms.Count());
            Assert.IsNotNull(connectedAtoms.ElementAt(0));
            Assert.IsNotNull(connectedAtoms.ElementAt(1));

            // test default return value
            connectedAtoms = b.GetConnectedAtoms(b.Builder.NewAtom());
            Assert.IsNull(connectedAtoms);
        }

        [TestMethod()]
        public virtual void TestIsConnectedTo_IBond()
        {
            var b = (IBond)NewChemObject();
            var c1 = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");
            var c2 = b.Builder.NewAtom("C");
            var c3 = b.Builder.NewAtom("C");

            var b1 = b.Builder.NewBond(c1, o);
            var b2 = b.Builder.NewBond(o, c2);
            var b3 = b.Builder.NewBond(c2, c3);

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
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Double;

            Assert.AreEqual(BondOrder.Double, b.Order);
        }

        [TestMethod()]
        public virtual void TestSetOrder_BondOrder()
        {
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");
            b.SetAtoms(new[] { c, o });
            b.Order = BondOrder.Double;

            Assert.AreEqual(BondOrder.Double, b.Order);

            b.Order = BondOrder.Single;
            Assert.AreEqual(BondOrder.Single, b.Order);
        }

        [TestMethod()]
        public virtual void TestSetOrder_electronCounts()
        {
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("C");

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
            var b = (IBond)NewChemObject();
            var c = b.Builder.NewAtom("C");
            var o = b.Builder.NewAtom("O");
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
            var obj = NewChemObject();
            var c = obj.Builder.NewAtom("C");
            var o = obj.Builder.NewAtom("O");

            var b = obj.Builder.NewBond(c, o, BondOrder.Double, BondStereo.Up);
            Assert.AreEqual(BondStereo.Up, b.Stereo);
        }

        [TestMethod()]
        public virtual void TestGet2DCenter()
        {
            var obj = NewChemObject();
            var o = obj.Builder.NewAtom("O", Vector2.Zero);
            var c = obj.Builder.NewAtom("C", new Vector2(1, 1));
            var b = obj.Builder.NewBond(c, o);

            var b2d = b.GetGeometric2DCenter();
            Assert.AreEqual(0.5, b2d.X, 0.001);
            Assert.AreEqual(0.5, b2d.Y, 0.001);
        }

        [TestMethod()]
        public virtual void TestGet3DCenter()
        {
            var obj = NewChemObject();
            var o = obj.Builder.NewAtom("O", Vector3.Zero);
            var c = obj.Builder.NewAtom("C", new Vector3(1, 1, 1));
            var b = obj.Builder.NewBond(c, o);

            var b3d = b.GetGeometric3DCenter();
            Assert.AreEqual(0.5, b3d.X, 0.001);
            Assert.AreEqual(0.5, b3d.Y, 0.001);
            Assert.AreEqual(0.5, b3d.Z, 0.001);
        }

        [TestMethod()]

        public override void TestClone()
        {
            var bond = (IBond)NewChemObject();
            object clone = bond.Clone();
            Assert.IsNotNull(clone);
            Assert.IsTrue(clone is IBond);
        }

        [TestMethod()]
        public virtual void TestClone_IAtom()
        {
            var obj = NewChemObject();
            var atom1 = obj.Builder.NewAtom("C");
            var atom2 = obj.Builder.NewAtom("O");
            var bond = obj.Builder.NewBond(atom1, atom2);
            var clone = (IBond)bond.Clone();

            // test cloning of atoms
            Assert.AreNotSame(atom1, clone.Begin);
            Assert.AreNotSame(atom2, clone.End);
        }

        [TestMethod()]
        public virtual void TestClone_Order()
        {
            var obj = NewChemObject();
            var atom1 = obj.Builder.NewAtom("C");
            var atom2 = obj.Builder.NewAtom("O");
            var bond = obj.Builder.NewBond(atom1, atom2, BondOrder.Single);
            var clone = (IBond)bond.Clone();

            // test cloning of bond order
            bond.Order = BondOrder.Double;
            Assert.AreEqual(BondOrder.Single, clone.Order);
        }

        [TestMethod()]
        public virtual void TestClone_Stereo()
        {
            var obj = NewChemObject();
            var atom1 = obj.Builder.NewAtom("C");
            var atom2 = obj.Builder.NewAtom("O");
            var bond = obj.Builder.NewBond(atom1, atom2, BondOrder.Single, BondStereo.Up);
            var clone = (IBond)bond.Clone();

            // test cloning of bond order
            bond.Stereo = BondStereo.UpInverted;
            Assert.AreEqual(BondStereo.Up, clone.Stereo);
        }

        /// <summary>
        /// Test for RFC #9
        /// </summary>
        [TestMethod()]
        public override void TestToString()
        {
            var bond = (IBond)NewChemObject();
            var description = bond.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]
        public virtual void TestMultiCenter1()
        {
            var obj = NewChemObject();
            var atom1 = obj.Builder.NewAtom("C");
            var atom2 = obj.Builder.NewAtom("O");
            var atom3 = obj.Builder.NewAtom("C");

            var bond = obj.Builder.NewBond(new IAtom[] { atom1, atom2, atom3 });
            Assert.AreEqual(3, bond.Atoms.Count);
            Assert.AreEqual(atom1, bond.Atoms[0]);
            Assert.AreEqual(atom2, bond.Atoms[1]);
            Assert.AreEqual(atom3, bond.Atoms[2]);

            Assert.AreEqual(BondOrder.Unset, bond.Order);
        }

        [TestMethod()]
        public virtual void TestMultiCenterCompare()
        {
            var obj = NewChemObject();
            var atom1 = obj.Builder.NewAtom("C");
            var atom2 = obj.Builder.NewAtom("O");
            var atom3 = obj.Builder.NewAtom("C");

            var bond1 = obj.Builder.NewBond(new IAtom[] { atom1, atom2, atom3 });
            var bond2 = obj.Builder.NewBond(new IAtom[] { atom1, atom2, atom3 });

            Assert.IsTrue(bond1.Compare(bond2));

            var atom4 = obj.Builder.NewAtom("C");
            var bond3 = obj.Builder.NewBond(new IAtom[] { atom1, atom2, atom4 });
            Assert.IsFalse(bond1.Compare(bond3));
        }

        [TestMethod()]
        public virtual void TestMultiCenterContains()
        {
            var obj = NewChemObject();
            var atom1 = obj.Builder.NewAtom("C");
            var atom2 = obj.Builder.NewAtom("O");
            var atom3 = obj.Builder.NewAtom("C");
            var atom4 = obj.Builder.NewAtom("C");

            var bond1 = obj.Builder.NewBond(new IAtom[] { atom1, atom2, atom3 });
            Assert.IsTrue(bond1.Contains(atom1));
            Assert.IsTrue(bond1.Contains(atom2));
            Assert.IsTrue(bond1.Contains(atom3));
            Assert.IsFalse(bond1.Contains(atom4));
        }

        [TestMethod()]
        public virtual void TestMultiCenterIterator()
        {
            var obj = NewChemObject();
            var atom1 = obj.Builder.NewAtom("C");
            var atom2 = obj.Builder.NewAtom("O");
            var atom3 = obj.Builder.NewAtom("C");
            var atom4 = obj.Builder.NewAtom("C");

            var bond1 = obj.Builder.NewBond(new IAtom[] { atom1, atom2, atom3, atom4 });
            var atoms = bond1.Atoms;
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
            var obj = NewChemObject();
            var atom1 = obj.Builder.NewAtom("C");
            var atom2 = obj.Builder.NewAtom("O");
            var atom3 = obj.Builder.NewAtom("C");
            var atom4 = obj.Builder.NewAtom("C");

            var bond1 = obj.Builder.NewBond(new IAtom[] { atom1, atom2, atom3, atom4 });
            Assert.AreEqual(atom2, bond1.GetOther(atom1));
            Assert.IsNull(bond1.GetOther(obj.Builder.NewAtom()));

            var conAtoms = bond1.GetConnectedAtoms(atom1);
            bool correct = true;
            foreach (var atom in conAtoms)
            {
                if (atom.Equals(atom1))
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
                if (atom.Equals(atom3))
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
            var obj = NewChemObject();
            var atom1 = obj.Builder.NewAtom("C");
            var atom2 = obj.Builder.NewAtom("O");
            var atom3 = obj.Builder.NewAtom("C");
            var atom4 = obj.Builder.NewAtom("C");
            var atom5 = obj.Builder.NewAtom("C");

            var bond1 = obj.Builder.NewBond(new IAtom[] { atom1, atom2, atom3 });
            var bond2 = obj.Builder.NewBond(new IAtom[] { atom2, atom3, atom4 });
            var bond3 = obj.Builder.NewBond(new IAtom[] { atom2, atom4 });
            var bond4 = obj.Builder.NewBond(new IAtom[] { atom5, atom4 });

            Assert.IsTrue(bond1.IsConnectedTo(bond2));
            Assert.IsTrue(bond2.IsConnectedTo(bond1));
            Assert.IsTrue(bond1.IsConnectedTo(bond3));
            Assert.IsFalse(bond4.IsConnectedTo(bond1));
        }
    }
}
