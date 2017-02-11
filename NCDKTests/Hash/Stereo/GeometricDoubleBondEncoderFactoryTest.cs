/*
 * Copyright (c) 2013. John May <jwmay@users.sf.net>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Base;
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Default;
using NCDK.Numerics;
using System.Reflection;
using static NCDK.Hash.Stereo.GeometricDoubleBondEncoderFactory;

namespace NCDK.Hash.Stereo
{
    /**
     * @author John May
     * @cdk.module test-hash
     */
    [TestClass()]
    public class GeometricDoubleBondEncoderFactoryTest
    {

        [TestMethod()]
        public void TestCreate()
        {
            var m_mol = new Mock<IAtomContainer>(); var mol = m_mol.Object;

            // a       d    0       3
            //  \     /      \     /
            //   b = c        1 = 2
            //  /     \      /     \
            // e       f    4       5
            var m_a = new Mock<IAtom>(); var a = m_a.Object; // 0
            var m_b = new Mock<IAtom>(); var b = m_b.Object; // 1
            var m_c = new Mock<IAtom>(); var c = m_c.Object; // 2
            var m_d = new Mock<IAtom>(); var d = m_d.Object; // 3
            var m_e = new Mock<IAtom>(); var e = m_e.Object; // 4
            var m_f = new Mock<IAtom>(); var f = m_f.Object; // 5

            m_mol.Setup(n => n.Atoms.IndexOf(a)).Returns(0);
            m_mol.Setup(n => n.Atoms.IndexOf(b)).Returns(1);
            m_mol.Setup(n => n.Atoms.IndexOf(c)).Returns(2);
            m_mol.Setup(n => n.Atoms.IndexOf(d)).Returns(3);
            m_mol.Setup(n => n.Atoms.IndexOf(e)).Returns(4);
            m_mol.Setup(n => n.Atoms.IndexOf(f)).Returns(5);

            m_mol.Setup(n => n.Atoms[0]).Returns(a);
            m_mol.Setup(n => n.Atoms[1]).Returns(b);
            m_mol.Setup(n => n.Atoms[2]).Returns(c);
            m_mol.Setup(n => n.Atoms[3]).Returns(d);
            m_mol.Setup(n => n.Atoms[4]).Returns(e);
            m_mol.Setup(n => n.Atoms[5]).Returns(f);

            m_b.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);
            m_c.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);

            m_a.SetupGet(n => n.Point2D).Returns(new Vector2());
            m_b.SetupGet(n => n.Point2D).Returns(new Vector2());
            m_c.SetupGet(n => n.Point2D).Returns(new Vector2());
            m_d.SetupGet(n => n.Point2D).Returns(new Vector2());
            m_e.SetupGet(n => n.Point2D).Returns(new Vector2());
            m_f.SetupGet(n => n.Point2D).Returns(new Vector2());

            var m_ba = new Mock<IBond>(); var ba = m_ba.Object;
            var m_be = new Mock<IBond>(); var be = m_be.Object;
            var m_bc = new Mock<IBond>(); var bc = m_bc.Object;
            var m_cd = new Mock<IBond>(); var cd = m_cd.Object;
            var m_cf = new Mock<IBond>(); var cf = m_cf.Object;

            m_ba.SetupGet(n => n.Atoms[0]).Returns(b);
            m_ba.SetupGet(n => n.Atoms[1]).Returns(a);
            m_be.SetupGet(n => n.Atoms[0]).Returns(b);
            m_be.SetupGet(n => n.Atoms[1]).Returns(e);
            m_bc.SetupGet(n => n.Atoms[0]).Returns(b);
            m_bc.SetupGet(n => n.Atoms[1]).Returns(c);
            m_cd.SetupGet(n => n.Atoms[0]).Returns(c);
            m_cd.SetupGet(n => n.Atoms[1]).Returns(d);
            m_cf.SetupGet(n => n.Atoms[0]).Returns(c);
            m_cf.SetupGet(n => n.Atoms[1]).Returns(f);

            m_bc.SetupGet(n => n.Order).Returns(BondOrder.Double);

            var bonds = new[] { ba, be, bc, cd, cf };
            m_mol.SetupGet(n => n.Bonds).Returns(bonds);

            m_mol.Setup(n => n.GetConnectedBonds(a)).Returns(new[] { ba });
            m_mol.Setup(n => n.GetConnectedBonds(b)).Returns(new[] { ba, bc, be });
            m_mol.Setup(n => n.GetConnectedBonds(c)).Returns(new[] { bc, cd, cf });
            m_mol.Setup(n => n.GetConnectedBonds(d)).Returns(new[] { cd });
            m_mol.Setup(n => n.GetConnectedBonds(e)).Returns(new[] { be });
            m_mol.Setup(n => n.GetConnectedBonds(f)).Returns(new[] { cf });

            var factory = new GeometricDoubleBondEncoderFactory();

            int[][] g = new int[][] { new[] { 1 }, new[] { 0, 2, 4 }, new[] { 1, 3, 5 }, new[] { 2 }, new[] { 1 }, new[] { 2 } };

            Assert.IsTrue(factory.Create(mol, g) is MultiStereoEncoder);
        }

        [TestMethod()]
        public void TestCreate_NoCoordinates()
        {

            var m_mol = new Mock<IAtomContainer>(); var mol = m_mol.Object;

            // a       d    0       3
            //  \     /      \     /
            //   b = c        1 = 2
            //  /     \      /     \
            // e       f    4       5
            var m_a = new Mock<IAtom>(); var a = m_a.Object; // 0
            var m_b = new Mock<IAtom>(); var b = m_b.Object; // 1
            var m_c = new Mock<IAtom>(); var c = m_c.Object; // 2
            var m_d = new Mock<IAtom>(); var d = m_d.Object; // 3
            var m_e = new Mock<IAtom>(); var e = m_e.Object; // 4
            var m_f = new Mock<IAtom>(); var f = m_f.Object; // 5

            m_mol.Setup(n => n.Atoms.IndexOf(a)).Returns(0);
            m_mol.Setup(n => n.Atoms.IndexOf(b)).Returns(1);
            m_mol.Setup(n => n.Atoms.IndexOf(c)).Returns(2);
            m_mol.Setup(n => n.Atoms.IndexOf(d)).Returns(3);
            m_mol.Setup(n => n.Atoms.IndexOf(e)).Returns(4);
            m_mol.Setup(n => n.Atoms.IndexOf(f)).Returns(5);

            m_mol.SetupGet(n => n.Atoms[0]).Returns(a);
            m_mol.SetupGet(n => n.Atoms[1]).Returns(b);
            m_mol.SetupGet(n => n.Atoms[2]).Returns(c);
            m_mol.SetupGet(n => n.Atoms[3]).Returns(d);
            m_mol.SetupGet(n => n.Atoms[4]).Returns(e);
            m_mol.SetupGet(n => n.Atoms[5]).Returns(f);

            m_b.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);
            m_c.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);

            var m_ba = new Mock<IBond>(); var ba = m_ba.Object;
            var m_be = new Mock<IBond>(); var be = m_be.Object;
            var m_bc = new Mock<IBond>(); var bc = m_bc.Object;
            var m_cd = new Mock<IBond>(); var cd = m_cd.Object;
            var m_cf = new Mock<IBond>(); var cf = m_cf.Object;

            m_ba.SetupGet(n => n.Atoms[0]).Returns(b);
            m_ba.SetupGet(n => n.Atoms[1]).Returns(a);
            m_be.SetupGet(n => n.Atoms[0]).Returns(b);
            m_be.SetupGet(n => n.Atoms[1]).Returns(e);
            m_bc.SetupGet(n => n.Atoms[0]).Returns(b);
            m_bc.SetupGet(n => n.Atoms[1]).Returns(c);
            m_cd.SetupGet(n => n.Atoms[0]).Returns(c);
            m_cd.SetupGet(n => n.Atoms[1]).Returns(d);
            m_cf.SetupGet(n => n.Atoms[0]).Returns(c);
            m_cf.SetupGet(n => n.Atoms[1]).Returns(f);

            m_bc.SetupGet(n => n.Order).Returns(BondOrder.Double);
            m_mol.SetupGet(n => n.Bonds).Returns(new[] { ba, be, bc, cd, cf });

            m_mol.Setup(n => n.GetConnectedBonds(a)).Returns(new[] { ba });
            m_mol.Setup(n => n.GetConnectedBonds(b)).Returns(new[] { ba, bc, be });
            m_mol.Setup(n => n.GetConnectedBonds(c)).Returns(new[] { bc, cd, cf });
            m_mol.Setup(n => n.GetConnectedBonds(d)).Returns(new[] { cd });
            m_mol.Setup(n => n.GetConnectedBonds(e)).Returns(new[] { be });
            m_mol.Setup(n => n.GetConnectedBonds(f)).Returns(new[] { cf });

            var factory = new GeometricDoubleBondEncoderFactory();

            int[][] g = new int[][] { new[] { 1 }, new[] { 0, 2, 4 }, new[] { 1, 3, 5 }, new[] { 2 }, new[] { 1 }, new[] { 2 } };

            Assert.IsTrue(factory.Create(mol, g) == StereoEncoder.EMPTY);
        }

        [TestMethod()]
        public void TestGeometric_2D()
        {
            var m_l = new Mock<IAtom>(); var l = m_l.Object; // 0
            var m_r = new Mock<IAtom>(); var r = m_r.Object; // 1
            var m_l1 = new Mock<IAtom>(); var l1 = m_l1.Object; // 2
            var m_l2 = new Mock<IAtom>(); var l2 = m_l2.Object; // 3
            var m_r1 = new Mock<IAtom>(); var r1 = m_r1.Object; // 4
            var m_r2 = new Mock<IAtom>(); var r2 = m_r2.Object; // 5

            var m_m = new Mock<IAtomContainer>(); var m = m_m.Object;

            m_m.SetupGet(n => n.Atoms[0]).Returns(l);
            m_m.SetupGet(n => n.Atoms[1]).Returns(r);
            m_m.SetupGet(n => n.Atoms[2]).Returns(l1);
            m_m.SetupGet(n => n.Atoms[3]).Returns(l2);
            m_m.SetupGet(n => n.Atoms[4]).Returns(r1);
            m_m.SetupGet(n => n.Atoms[5]).Returns(r2);

            m_l.SetupGet(n => n.Point2D).Returns(new Vector2());
            m_r.SetupGet(n => n.Point2D).Returns(new Vector2());
            m_l1.SetupGet(n => n.Point2D).Returns(new Vector2());
            m_l2.SetupGet(n => n.Point2D).Returns(new Vector2());
            m_r1.SetupGet(n => n.Point2D).Returns(new Vector2());
            m_r2.SetupGet(n => n.Point2D).Returns(new Vector2());

            GeometricParity p = Geometric(m, 0, 1, 2, 3, 4, 5);
            Assert.IsTrue(p is DoubleBond2DParity);
        }

        [TestMethod()]
        public void TestGeometric_3D()
        {
            var m_l = new Mock<IAtom>(); var l = m_l.Object; // 0
            var m_r = new Mock<IAtom>(); var r = m_r.Object; // 1
            var m_l1 = new Mock<IAtom>(); var l1 = m_l1.Object; // 2
            var m_l2 = new Mock<IAtom>(); var l2 = m_l2.Object; // 3
            var m_r1 = new Mock<IAtom>(); var r1 = m_r1.Object; // 4
            var m_r2 = new Mock<IAtom>(); var r2 = m_r2.Object; // 5

            var m_m = new Mock<IAtomContainer>(); var m = m_m.Object;

            m_m.SetupGet(n => n.Atoms[0]).Returns(l);
            m_m.SetupGet(n => n.Atoms[1]).Returns(r);
            m_m.SetupGet(n => n.Atoms[2]).Returns(l1);
            m_m.SetupGet(n => n.Atoms[3]).Returns(l2);
            m_m.SetupGet(n => n.Atoms[4]).Returns(r1);
            m_m.SetupGet(n => n.Atoms[5]).Returns(r2);

            m_l.SetupGet(n => n.Point3D).Returns(new Vector3());
            m_r.SetupGet(n => n.Point3D).Returns(new Vector3());
            m_l1.SetupGet(n => n.Point3D).Returns(new Vector3());
            m_l2.SetupGet(n => n.Point3D).Returns(new Vector3());
            m_r1.SetupGet(n => n.Point3D).Returns(new Vector3());
            m_r2.SetupGet(n => n.Point3D).Returns(new Vector3());

            GeometricParity p = Geometric(m, 0, 1, 2, 3, 4, 5);
            Assert.IsTrue(p is DoubleBond3DParity);
        }

        [TestMethod()]
        public void TestPermutation_SingleSubstituents()
        {
            // for a double atom with only one substituent the permutation parity
            // should be the identity (i.e. 1)
            Assert.AreEqual(PermutationParity.IDENTITY, Permutation(new int[] { 1, 2 }));
        }

        [TestMethod()]
        public void TestPermutation_TwoSubstituents()
        {
            PermutationParity p = Permutation(new int[] { 1, 2, 0 });
            Assert.IsTrue(p is BasicPermutationParity);
            FieldInfo field = p.GetType().GetField("indices", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 2 }, (int[])field.GetValue(p)));
        }

        [TestMethod()]
        public void TestMoveToBack()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 2, 0 }, MoveToBack(new int[] { 0, 1, 2 }, 0)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 2, 1 }, MoveToBack(new int[] { 0, 1, 2 }, 1)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 2 }, MoveToBack(new int[] { 0, 1, 2 }, 2)));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 1, 4, 5, 6, 2 }, MoveToBack(new int[] { 0, 1, 2, 4, 5, 6 }, 2)));
        }

        [TestMethod()]
        public void TestAccept_Hybridization()
        {

            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_a = new Mock<IBond>(); var a = m_a.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            var m_c = new Mock<IBond>(); var c = m_c.Object;

            m_a.SetupGet(n => n.Order).Returns(BondOrder.Double);

            var bonds = new[] { a, b, c };
            Assert.IsFalse(GeometricDoubleBondEncoderFactory.Accept(atom, bonds));

            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);

            Assert.IsTrue(GeometricDoubleBondEncoderFactory.Accept(atom, bonds));
        }

        [TestMethod()]
        public void TestAccept_QueryBond()
        {

            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_a = new Mock<IBond>(); var a = m_a.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            var m_c = new Mock<IBond>(); var c = m_c.Object;

            var bonds = new[] { a, b, c };

            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);
            m_a.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsTrue(GeometricDoubleBondEncoderFactory.Accept(atom, bonds));
            m_b.SetupGet(n => n.Stereo).Returns(BondStereo.UpOrDown);
            Assert.IsFalse(GeometricDoubleBondEncoderFactory.Accept(atom, bonds));
            m_b.SetupGet(n => n.Stereo).Returns(BondStereo.UpOrDownInverted);
            Assert.IsFalse(GeometricDoubleBondEncoderFactory.Accept(atom, bonds));
        }

        [TestMethod()]
        public void TestAccept_CumulatedDoubleBond()
        {

            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_a = new Mock<IBond>(); var a = m_a.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            var m_c = new Mock<IBond>(); var c = m_c.Object;

            var bonds = new[] { a, b, c };

            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);
            m_a.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsTrue(GeometricDoubleBondEncoderFactory.Accept(atom, bonds));
            m_b.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsFalse(GeometricDoubleBondEncoderFactory.Accept(atom, bonds));
        }

        [TestMethod()]
        public void TestAccept_NoSubstituents()
        {

            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_a = new Mock<IBond>(); var a = m_a.Object;

            var bonds = new[] { a };

            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);
            m_a.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsFalse(GeometricDoubleBondEncoderFactory.Accept(atom, bonds));
        }
    }
}
