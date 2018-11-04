/*
 * Copyright (c) 2017 John Mayfield <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Config;
using NCDK.Graphs;
using NCDK.Isomorphisms.Matchers;
using NCDK.Silent;
using NCDK.Templates;
using System;

namespace NCDK.Isomorphisms
{
    [TestClass()]
    public class ExprTest
    {
        [TestMethod()]
        public void TestT()
        {
            var expr = new Expr(ExprType.True);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestF()
        {
            var expr = new Expr(ExprType.False);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestAndTT()
        {
            var expr = new Expr(ExprType.And, new Expr(ExprType.True), new Expr(ExprType.True));
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestAndTF()
        {
            var expr = new Expr(ExprType.And, new Expr(ExprType.True), new Expr(ExprType.False));
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestAndFT()
        {
            var expr = new Expr(ExprType.And, new Expr(ExprType.False), new Expr(ExprType.True));
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestOrTT()
        {
            var expr = new Expr(ExprType.Or, new Expr(ExprType.True), new Expr(ExprType.True));
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestOrTF()
        {
            var expr = new Expr(ExprType.Or, new Expr(ExprType.True), new Expr(ExprType.False));
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestOrFT()
        {
            var expr = new Expr(ExprType.Or, new Expr(ExprType.False), new Expr(ExprType.True));
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestOrFF()
        {
            var expr = new Expr(ExprType.Or, new Expr(ExprType.False), new Expr(ExprType.False));
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestNotF()
        {
            var expr = new Expr(ExprType.Not, new Expr(ExprType.False), null);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestNotT()
        {
            var expr = new Expr(ExprType.Not, new Expr(ExprType.True), null);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestNotStereo()
        {
            var expr = new Expr(ExprType.Not, new Expr(ExprType.Stereochemistry, 1), null);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsTrue(expr.Matches(atom));
            Assert.IsTrue(expr.Matches(atom, 2));
            Assert.IsFalse(expr.Matches(atom, 1));
        }

        [TestMethod()]
        public void TestNotStereo3()
        {
            var expr = new Expr(ExprType.Not, new Expr(ExprType.Stereochemistry, 1).Or(new Expr(ExprType.Stereochemistry, 0)), null);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsTrue(expr.Matches(atom));
            Assert.IsTrue(expr.Matches(atom, 2));
            Assert.IsFalse(expr.Matches(atom, 1));
        }

        [TestMethod()]
        public void TestNotStereo4()
        {
            var expr = new Expr(ExprType.Not, new Expr(ExprType.Or, new Expr(ExprType.True), new Expr(ExprType.True)), null);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestStereoT()
        {
            var expr = new Expr(ExprType.Stereochemistry, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsTrue(expr.Matches(atom, 1));
        }

        [TestMethod()]
        public void TestStereoF()
        {
            var expr = new Expr(ExprType.Stereochemistry, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            Assert.IsFalse(expr.Matches(atom, 2));
        }

        [TestMethod()]
        public void TestIsAromatic()
        {
            var expr = new Expr(ExprType.IsAromatic);
            var atom = new Atom { IsAromatic = false };
            Assert.IsFalse(expr.Matches(atom));
            atom.IsAromatic = true;
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestIsAliphaticT()
        {
            var expr = new Expr(ExprType.IsAliphatic);
            var atom = new Atom { IsAromatic = false };
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestIsAliphaticF()
        {
            var expr = new Expr(ExprType.IsAliphatic);
            var atom = new Atom { IsAromatic = true };
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestIsHetero()
        {
            var expr = new Expr(ExprType.IsHetero);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.AtomicNumber).Returns(1);
            Assert.IsFalse(expr.Matches(m_atom.Object));
            m_atom.SetupGet(n => n.AtomicNumber).Returns(6);
            Assert.IsFalse(expr.Matches(m_atom.Object));
            m_atom.SetupGet(n => n.AtomicNumber).Returns(8);
            Assert.IsTrue(expr.Matches(m_atom.Object));
        }

        [TestMethod()]
        public void TestHasImplicitHydrogenT()
        {
            var expr = new Expr(ExprType.HasImplicitHydrogen);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(1);
            Assert.IsTrue(expr.Matches(atom));
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(2);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHasImplicitHydrogenF()
        {
            var expr = new Expr(ExprType.HasImplicitHydrogen);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(0);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHasImplicitHydrogenNull()
        {
            var expr = new Expr(ExprType.HasImplicitHydrogen);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns((int?)null);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHasIsotope()
        {
            var expr = new Expr(ExprType.HasIsotope);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.MassNumber).Returns((int?)null);
            Assert.IsFalse(expr.Matches(atom));
            m_atom.SetupGet(n => n.MassNumber).Returns(12);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHasUnspecIsotope()
        {
            var expr = new Expr(ExprType.HasUnspecifiedIsotope);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.MassNumber).Returns(12);
            Assert.IsFalse(expr.Matches(atom));
            m_atom.SetupGet(n => n.MassNumber).Returns((int?)null);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestIsInRing()
        {
            var expr = new Expr(ExprType.IsInRing);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.IsInRing).Returns(false);
            Assert.IsFalse(expr.Matches(atom));
            m_atom.SetupGet(n => n.IsInRing).Returns(true);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestIsInChain()
        {
            var expr = new Expr(ExprType.IsInChain);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.IsInRing).Returns(false);
            Assert.IsTrue(expr.Matches(atom));
            m_atom.SetupGet(n => n.IsInRing).Returns(true);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestUnsaturatedT()
        {
            var expr = new Expr(ExprType.Unsaturated);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Double);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { bond });
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestUnsaturatedF()
        {
            var expr = new Expr(ExprType.Unsaturated);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { bond });
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestElementT()
        {
            for (int num = 1; num < 54; ++num)
            {
                var expr = new Expr(ExprType.Element, num);
                var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
                m_atom.SetupGet(n => n.AtomicNumber).Returns(num);
                Assert.IsTrue(expr.Matches(atom));
            }
        }

        [TestMethod()]
        public void TestElementF()
        {
            for (int num = 1; num < 54; ++num)
            {
                var expr = new Expr(ExprType.Element, num);
                var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
                m_atom.SetupGet(n => n.AtomicNumber).Returns(num + 1);
                Assert.IsFalse(expr.Matches(atom));
            }
        }

        [TestMethod()]
        public void TestAliphaticElementT()
        {
            for (int num = 1; num < 54; ++num)
            {
                var expr = new Expr(ExprType.AliphaticElement, num);
                var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
                m_atom.SetupGet(n => n.AtomicNumber).Returns(num);
                m_atom.SetupGet(n => n.IsAromatic).Returns(false);
                Assert.IsTrue(expr.Matches(atom));
            }
        }

        [TestMethod()]
        public void TestAliphaticElementF()
        {
            for (int num = 1; num < 54; ++num)
            {
                var expr = new Expr(ExprType.AliphaticElement, num);
                var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
                m_atom.SetupGet(n => n.AtomicNumber).Returns(num);
                m_atom.SetupGet(n => n.IsAromatic).Returns(true);
                Assert.IsFalse(expr.Matches(atom));
            }
        }

        [TestMethod()]
        public void TestAliphaticElementFalse2()
        {
            for (int num = 1; num < 54; ++num)
            {
                var expr = new Expr(ExprType.AliphaticElement, num);
                var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
                m_atom.SetupGet(n => n.AtomicNumber).Returns(num + 1);
                m_atom.SetupGet(n => n.IsAromatic).Returns(false);
                Assert.IsFalse(expr.Matches(atom));
            }
        }

        [TestMethod()]
        public void TestAromaticElementT()
        {
            for (int num = 1; num < 54; ++num)
            {
                var expr = new Expr(ExprType.AromaticElement, num);
                var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
                m_atom.SetupGet(n => n.AtomicNumber).Returns(num);
                m_atom.SetupGet(n => n.IsAromatic).Returns(true);
                Assert.IsTrue(expr.Matches(atom));
            }
        }

        [TestMethod()]
        public void TestAromaticElementF()
        {
            for (int num = 1; num < 54; ++num)
            {
                var expr = new Expr(ExprType.AromaticElement, num);
                var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
                m_atom.SetupGet(n => n.AtomicNumber).Returns(num);
                m_atom.SetupGet(n => n.IsAromatic).Returns(false);
                Assert.IsFalse(expr.Matches(atom));
            }
        }

        [TestMethod()]
        public void TestAromaticElementFalse2()
        {
            for (int num = 1; num < 54; ++num)
            {
                var expr = new Expr(ExprType.AromaticElement, num);
                var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
                m_atom.SetupGet(n => n.AtomicNumber).Returns(num + 1);
                m_atom.SetupGet(n => n.IsAromatic).Returns(true);
                Assert.IsFalse(expr.Matches(atom));
            }
        }

        [TestMethod()]
        public void TestHCountT()
        {
            var expr = new Expr(ExprType.ImplicitHCount, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(1);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHCountF()
        {
            var expr = new Expr(ExprType.ImplicitHCount, 2);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestTotalHCountT()
        {
            var expr = new Expr(ExprType.TotalHCount, 3);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_h = new Mock<IAtom>(); var h = m_h.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            m_b.Setup(n => n.GetOther(atom)).Returns(h);
            m_b.Setup(n => n.GetOther(h)).Returns(atom);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(2);
            m_h.SetupGet(n => n.AtomicNumber).Returns(1);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b });
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestTotalHCountF()
        {
            var expr = new Expr(ExprType.TotalHCount, 2);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_h = new Mock<IAtom>(); var h = m_h.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            m_b.Setup(n => n.GetOther(atom)).Returns(h);
            m_b.Setup(n => n.GetOther(h)).Returns(atom);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(2);
            m_h.SetupGet(n => n.AtomicNumber).Returns(1);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b });
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestTotalHCountNullImplT()
        {
            var expr = new Expr(ExprType.TotalHCount, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_h = new Mock<IAtom>(); var h = m_h.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            m_b.Setup(n => n.GetOther(atom)).Returns(h);
            m_b.Setup(n => n.GetOther(h)).Returns(atom);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns((int?)null);
            m_h.SetupGet(n => n.AtomicNumber).Returns(1);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b });
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestTotalHCountImplF()
        {
            var expr = new Expr(ExprType.TotalHCount, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_h = new Mock<IAtom>(); var h = m_h.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            m_b.Setup(n => n.GetOther(atom)).Returns(h);
            m_b.Setup(n => n.GetOther(h)).Returns(atom);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(2);
            m_h.SetupGet(n => n.AtomicNumber).Returns(1);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b });
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestDegreeT()
        {
            var expr = new Expr(ExprType.Degree, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.Setup(n => n.Bonds.Count).Returns(1);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestDegreeF()
        {
            var expr = new Expr(ExprType.Degree, 2);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.Setup(n => n.Bonds.Count).Returns(1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestTotalDegreeT()
        {
            var expr = new Expr(ExprType.TotalDegree, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.Setup(n => n.Bonds.Count).Returns(1);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(0);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestTotalDegreeF()
        {
            var expr = new Expr(ExprType.TotalDegree, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.Setup(n => n.Bonds.Count).Returns(1);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHeavyDegreeT()
        {
            var expr = new Expr(ExprType.HeavyDegree, 0);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_h = new Mock<IAtom>(); var h = m_h.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            m_atom.Setup(n => n.Bonds.Count).Returns(1);
            m_b.Setup(n => n.GetOther(atom)).Returns(h);
            m_b.Setup(n => n.GetOther(h)).Returns(atom);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(2);
            m_h.SetupGet(n => n.AtomicNumber).Returns(1);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b });
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHeavyDegreeF()
        {
            var expr = new Expr(ExprType.HeavyDegree, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_h = new Mock<IAtom>(); var h = m_h.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            m_atom.Setup(n => n.Bonds.Count).Returns(1);
            m_b.Setup(n => n.GetOther(atom)).Returns(h);
            m_b.Setup(n => n.GetOther(h)).Returns(atom);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(2);
            m_h.SetupGet(n => n.AtomicNumber).Returns(1);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b });
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHeteroSubT()
        {
            var expr = new Expr(ExprType.HeteroSubstituentCount, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_o = new Mock<IAtom>(); var o = m_o.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            m_atom.Setup(n => n.Bonds.Count).Returns(1);
            m_b.Setup(n => n.GetOther(atom)).Returns(o);
            m_b.Setup(n => n.GetOther(o)).Returns(atom);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(2);
            m_o.SetupGet(n => n.AtomicNumber).Returns(8);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b });
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHeteroSubFailFastF()
        {
            var expr = new Expr(ExprType.HeteroSubstituentCount, 2);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_o = new Mock<IAtom>(); var o = m_o.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            m_atom.Setup(n => n.Bonds.Count).Returns(1);
            m_b.Setup(n => n.GetOther(atom)).Returns(o);
            m_b.Setup(n => n.GetOther(o)).Returns(atom);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(2);
            m_o.SetupGet(n => n.AtomicNumber).Returns(8);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b });
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHeteroSubF()
        {
            var expr = new Expr(ExprType.HeteroSubstituentCount, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_c = new Mock<IAtom>(); var c = m_c.Object;
            var m_b = new Mock<IBond>(); var b = m_b.Object;
            m_atom.Setup(n => n.Bonds.Count).Returns(1);
            m_b.Setup(n => n.GetOther(atom)).Returns(c);
            m_b.Setup(n => n.GetOther(c)).Returns(atom);
            m_atom.SetupGet(n => n.ImplicitHydrogenCount).Returns(2);
            m_c.SetupGet(n => n.AtomicNumber).Returns(6);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b });
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestValenceT()
        {
            var expr = new Expr(ExprType.Valence, 4);
            var m_a1 = new Mock<IAtom>(); var a1 = m_a1.Object;
            var m_b1 = new Mock<IBond>(); var b1 = m_b1.Object;
            var m_b2 = new Mock<IBond>(); var b2 = m_b2.Object;
            m_a1.SetupGet(n => n.ImplicitHydrogenCount).Returns(1);
            m_b1.SetupGet(n => n.Order).Returns(BondOrder.Double);
            m_b2.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_a1.SetupGet(n => n.Bonds).Returns(new[] { b1, b2 });
            Assert.IsTrue(expr.Matches(a1));
        }

        [TestMethod()]
        public void TestValenceF()
        {
            var expr = new Expr(ExprType.Valence, 4);
            var m_a1 = new Mock<IAtom>(); var a1 = m_a1.Object;
            var m_b1 = new Mock<IBond>(); var b1 = m_b1.Object;
            var m_b2 = new Mock<IBond>(); var b2 = m_b2.Object;
            m_a1.SetupGet(n => n.ImplicitHydrogenCount).Returns(1);
            m_b1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_b2.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_a1.SetupGet(n => n.Bonds).Returns(new[] { b1, b2 });
            Assert.IsFalse(expr.Matches(a1));
        }

        [TestMethod()]
        public void TestValenceNullOrderT()
        {
            var expr = new Expr(ExprType.Valence, 4);
            var m_a1 = new Mock<IAtom>(); var a1 = m_a1.Object;
            var m_b1 = new Mock<IBond>(); var b1 = m_b1.Object;
            var m_b2 = new Mock<IBond>(); var b2 = m_b2.Object;
            m_a1.SetupGet(n => n.ImplicitHydrogenCount).Returns(1);
            m_b1.SetupGet(n => n.Order).Returns(BondOrder.Double);
            m_b2.SetupGet(n => n.Order).Returns(null);
            m_a1.SetupGet(n => n.Bonds).Returns(new[] { b1, b2 });
            Assert.IsFalse(expr.Matches(a1));
        }

        [TestMethod()]
        public void TestValenceFailFastF()
        {
            var expr = new Expr(ExprType.Valence, 2);
            var m_a1 = new Mock<IAtom>(); var a1 = m_a1.Object;
            m_a1.SetupGet(n => n.ImplicitHydrogenCount).Returns(4);
            Assert.IsFalse(expr.Matches(a1));
        }

        [TestMethod()]
        public void TestIsotopeT()
        {
            var expr = new Expr(ExprType.Isotope, 13);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.MassNumber).Returns(13);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestIsotopeF()
        {
            var expr = new Expr(ExprType.Isotope, 12);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.MassNumber).Returns(13);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestFormalChargeT()
        {
            var expr = new Expr(ExprType.FormalCharge, -1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.FormalCharge).Returns(-1);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestFormalChargeF()
        {
            var expr = new Expr(ExprType.FormalCharge, -1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.FormalCharge).Returns(0);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestRingBondCountT()
        {
            var expr = new Expr(ExprType.RingBondCount, 3);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_b1 = new Mock<IBond>(); var b1 = m_b1.Object;
            var m_b2 = new Mock<IBond>(); var b2 = m_b2.Object;
            var m_b3 = new Mock<IBond>(); var b3 = m_b3.Object;
            m_atom.SetupGet(n => n.IsInRing).Returns(true);
            m_atom.Setup(n => n.Bonds.Count).Returns(3);
            m_b1.SetupGet(n => n.IsInRing).Returns(true);
            m_b2.SetupGet(n => n.IsInRing).Returns(true);
            m_b3.SetupGet(n => n.IsInRing).Returns(true);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b1, b2, b3 });
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestRingBondCountF()
        {
            var expr = new Expr(ExprType.RingBondCount, 3);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_b1 = new Mock<IBond>(); var b1 = m_b1.Object;
            var m_b2 = new Mock<IBond>(); var b2 = m_b2.Object;
            var m_b3 = new Mock<IBond>(); var b3 = m_b3.Object;
            m_atom.SetupGet(n => n.IsInRing).Returns(true);
            m_atom.Setup(n => n.Bonds.Count).Returns(3);
            m_b1.SetupGet(n => n.IsInRing).Returns(true);
            m_b2.SetupGet(n => n.IsInRing).Returns(true);
            m_b3.SetupGet(n => n.IsInRing).Returns(false);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b1, b2, b3 });
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestRingBondCountNonRingF()
        {
            var expr = new Expr(ExprType.RingBondCount, 3);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.IsInRing).Returns(false);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestRingBondCountLessBondsF()
        {
            var expr = new Expr(ExprType.RingBondCount, 3);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.IsInRing).Returns(true);
            m_atom.Setup(n => n.Bonds.Count).Returns(2);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestInsaturationT()
        {
            var expr = new Expr(ExprType.Insaturation, 2);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_b1 = new Mock<IBond>(); var b1 = m_b1.Object;
            var m_b2 = new Mock<IBond>(); var b2 = m_b2.Object;
            m_b1.SetupGet(n => n.Order).Returns(BondOrder.Double);
            m_b2.SetupGet(n => n.Order).Returns(BondOrder.Double);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b1, b2 });
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestInsaturationF()
        {
            var expr = new Expr(ExprType.Insaturation, 2);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_b1 = new Mock<IBond>(); var b1 = m_b1.Object;
            var m_b2 = new Mock<IBond>(); var b2 = m_b2.Object;
            m_b1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_b2.SetupGet(n => n.Order).Returns(BondOrder.Double);
            m_atom.SetupGet(n => n.Bonds).Returns(new[] { b1, b2 });
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestGroupT()
        {
            var expr = new Expr(ExprType.PeriodicGroup, NaturalElements.Chlorine.Group);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.AtomicNumber).Returns(9);
            Assert.IsTrue(expr.Matches(atom));
            m_atom.SetupGet(n => n.AtomicNumber).Returns(17);
            Assert.IsTrue(expr.Matches(atom));
            m_atom.SetupGet(n => n.AtomicNumber).Returns(35);
            Assert.IsTrue(expr.Matches(atom));
            m_atom.SetupGet(n => n.AtomicNumber).Returns(53);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestGroupF()
        {
            var expr = new Expr(ExprType.PeriodicGroup, NaturalElements.Chlorine.Group);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.AtomicNumber).Returns(8);
            Assert.IsFalse(expr.Matches(atom));
            m_atom.SetupGet(n => n.AtomicNumber).Returns(16);
            Assert.IsFalse(expr.Matches(atom));
            m_atom.SetupGet(n => n.AtomicNumber).Returns(34);
            Assert.IsFalse(expr.Matches(atom));
            m_atom.SetupGet(n => n.AtomicNumber).Returns(52);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestGroupNull()
        {
            var expr = new Expr(ExprType.PeriodicGroup, NaturalElements.Chlorine.Group);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.AtomicNumber).Returns((int?)null);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisation0F()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 0);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp1T()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP1);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp1F()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp2T()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 2);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP2);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp2F()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 2);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3T()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 3);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3F()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 3);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3d1T()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 4);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3D1);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3d1F()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 4);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3d2T()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 5);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3D2);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3d2F()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 5);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3d3T()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 6);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3D3);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3d3F()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 6);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3d4T()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 7);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3D4);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3d4F()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 7);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3d5T()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 8);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3D5);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp1Null()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 1);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(null);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestHybridisationSp3d5F()
        {
            var expr = new Expr(ExprType.HybridisationNumber, 8);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.Hybridization).Returns(Hybridization.SP1);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestReactionRoleT()
        {
            var expr = new Expr(ExprType.ReactionRole, ReactionRole.Reactant.Ordinal());
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.Setup(n => n.GetProperty<ReactionRole>(CDKPropertyName.ReactionRole)).Returns(ReactionRole.Reactant);
            Assert.IsTrue(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestReactionRoleF()
        {
            var expr = new Expr(ExprType.ReactionRole, ReactionRole.Reactant.Ordinal());
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.Setup(n => n.GetProperty<ReactionRole>(CDKPropertyName.ReactionRole)).Returns(ReactionRole.Product);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestReactionRoleNull()
        {
            var expr = new Expr(ExprType.ReactionRole, ReactionRole.Reactant.Ordinal());
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.Setup(n => n.GetProperty<ReactionRole>(CDKPropertyName.ReactionRole)).Returns(ReactionRole.None);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestRingSize6()
        {
            var expr = new Expr(ExprType.RingSize, 6);
            var mol = TestMoleculeFactory.MakeNaphthalene();
            Cycles.MarkRingAtomsAndBonds(mol);
            Assert.IsTrue(expr.Matches(mol.Atoms[0]));
        }

        [TestMethod()]
        public void TestRingSize10()
        {
            var expr = new Expr(ExprType.RingSize, 10);
            var mol = TestMoleculeFactory.MakeNaphthalene();
            Cycles.MarkRingAtomsAndBonds(mol);
            Assert.IsTrue(expr.Matches(mol.Atoms[0]));
        }

        [TestMethod()]
        public void TestRingSmallestNonRing()
        {
            var expr = new Expr(ExprType.RingSmallest, 6);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.IsInRing).Returns(false);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestRingNonRing()
        {
            var expr = new Expr(ExprType.RingSize, 6);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.IsInRing).Returns(false);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestRingCountNonRing()
        {
            var expr = new Expr(ExprType.RingCount, 6);
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            m_atom.SetupGet(n => n.IsInRing).Returns(false);
            Assert.IsFalse(expr.Matches(atom));
        }

        [TestMethod()]
        public void TestRingSmallestSize6()
        {
            var expr = new Expr(ExprType.RingSmallest, 6);
            var mol = TestMoleculeFactory.MakeNaphthalene();
            Cycles.MarkRingAtomsAndBonds(mol);
            Assert.IsTrue(expr.Matches(mol.Atoms[0]));
        }

        [TestMethod()]
        public void TestRingSmallestSize10()
        {
            var expr = new Expr(ExprType.RingSmallest, 10);
            var mol = TestMoleculeFactory.MakeNaphthalene();
            Cycles.MarkRingAtomsAndBonds(mol);
            Assert.IsFalse(expr.Matches(mol.Atoms[0]));
        }

        [TestMethod()]
        public void TestRingSmallestSize5And6()
        {
            Expr expr5 = new Expr(ExprType.RingSmallest, 5);
            Expr expr6 = new Expr(ExprType.RingSmallest, 6);
            var mol = TestMoleculeFactory.MakeIndole();
            Cycles.MarkRingAtomsAndBonds(mol);
            int numSmall5 = 0;
            int numSmall6 = 0;
            foreach (var atom in mol.Atoms)
            {
                if (expr5.Matches(atom))
                    numSmall5++;
                if (expr6.Matches(atom))
                    numSmall6++;
            }
            Assert.AreEqual(5, numSmall5);
            Assert.AreEqual(4, numSmall6);
        }

        [TestMethod()]
        public void TestRingSize5And6()
        {
            Expr expr5 = new Expr(ExprType.RingSize, 5);
            Expr expr6 = new Expr(ExprType.RingSize, 6);
            var mol = TestMoleculeFactory.MakeIndole();
            Cycles.MarkRingAtomsAndBonds(mol);
            int numSmall5 = 0;
            int numSmall6 = 0;
            foreach (var atom in mol.Atoms)
            {
                if (expr5.Matches(atom))
                    numSmall5++;
                if (expr6.Matches(atom))
                    numSmall6++;
            }
            Assert.AreEqual(5, numSmall5);
            Assert.AreEqual(6, numSmall6);
        }

        [TestMethod()]
        public void TestRingCount2()
        {
            var expr = new Expr(ExprType.RingCount, 2);
            var mol = TestMoleculeFactory.MakeNaphthalene();
            Cycles.MarkRingAtomsAndBonds(mol);
            int count = 0;
            foreach (var atom in mol.Atoms)
            {
                if (expr.Matches(atom))
                    count++;
            }
            Assert.AreEqual(2, count);
        }

        [TestMethod()]
        public void NonAtomExpr()
        {
            try
            {
                var expr = new Expr(ExprType.AliphaticOrder, 2);
                var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
                Assert.IsFalse(expr.Matches(atom));
                Assert.Fail();
            }
            catch (ArgumentException)
            { }
        }

        /* Bond Exprs */

        [TestMethod()]
        public void TestBondT()
        {
            var expr = new Expr(ExprType.True);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondF()
        {
            var expr = new Expr(ExprType.False);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondAndTT()
        {
            var expr = new Expr(ExprType.And, new Expr(ExprType.True), new Expr(ExprType.True));
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondAndTF()
        {
            var expr = new Expr(ExprType.And, new Expr(ExprType.True), new Expr(ExprType.False));
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondAndFT()
        {
            var expr = new Expr(ExprType.And, new Expr(ExprType.False), new Expr(ExprType.True));
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondOrTT()
        {
            var expr = new Expr(ExprType.Or, new Expr(ExprType.True), new Expr(ExprType.True));
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondOrTF()
        {
            var expr = new Expr(ExprType.Or, new Expr(ExprType.True), new Expr(ExprType.False));
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondOrFT()
        {
            var expr = new Expr(ExprType.Or, new Expr(ExprType.False), new Expr(ExprType.True));
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondOrFF()
        {
            var expr = new Expr(ExprType.Or, new Expr(ExprType.False), new Expr(ExprType.False));
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondNotF()
        {
            var expr = new Expr(ExprType.Not, new Expr(ExprType.False), null);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondNotT()
        {
            var expr = new Expr(ExprType.Not, new Expr(ExprType.True), null);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondNotStereo()
        {
            var expr = new Expr(ExprType.Not, new Expr(ExprType.Stereochemistry, 1), null);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsTrue(expr.Matches(bond));
            Assert.IsTrue(expr.Matches(bond, 2));
            Assert.IsFalse(expr.Matches(bond, 1));
        }

        [TestMethod()]
        public void TestBondNotStereo3()
        {
            var expr = new Expr(ExprType.Not, new Expr(ExprType.Stereochemistry, 1).Or(new Expr(ExprType.Stereochemistry, 0)), null);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsTrue(expr.Matches(bond));
            Assert.IsTrue(expr.Matches(bond, 2));
            Assert.IsFalse(expr.Matches(bond, 1));
        }

        [TestMethod()]
        public void TestBondNotStereo4()
        {
            var expr = new Expr(ExprType.Not, new Expr(ExprType.Or, new Expr(ExprType.True), new Expr(ExprType.True)), null);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondStereoT()
        {
            var expr = new Expr(ExprType.Stereochemistry, 1);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsTrue(expr.Matches(bond, 1));
        }

        [TestMethod()]
        public void TestBondStereoF()
        {
            var expr = new Expr(ExprType.Stereochemistry, 1);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            Assert.IsFalse(expr.Matches(bond, 2));
        }

        [TestMethod()]
        public void TestBondIsAromaticT()
        {
            var expr = new Expr(ExprType.IsAromatic);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsAromatic).Returns(true);
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondIsAromaticF()
        {
            var expr = new Expr(ExprType.IsAromatic);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsAromatic).Returns(false);
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondIsAliphaticT()
        {
            var expr = new Expr(ExprType.IsAliphatic);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsAromatic).Returns(false);
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondIsAliphaticF()
        {
            var expr = new Expr(ExprType.IsAliphatic);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsAromatic).Returns(true);
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondIsChainT()
        {
            var expr = new Expr(ExprType.IsInChain);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsInRing).Returns(false);
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondIsChainF()
        {
            var expr = new Expr(ExprType.IsInChain);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsInRing).Returns(true);
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondIsRingT()
        {
            var expr = new Expr(ExprType.IsInRing);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsInRing).Returns(true);
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondIsRingF()
        {
            var expr = new Expr(ExprType.IsInRing);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsInRing).Returns(false);
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondOrderT()
        {
            var expr = new Expr(ExprType.AliphaticOrder, 2);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondOrderF()
        {
            var expr = new Expr(ExprType.AliphaticOrder, 2);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Single);
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondOrderAlipF()
        {
            var expr = new Expr(ExprType.AliphaticOrder, 2);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsAromatic).Returns(true);
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestBondOrderNullF()
        {
            var expr = new Expr(ExprType.AliphaticOrder, 2);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.Order).Returns(null);
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestSingleOrAromaticT()
        {
            var expr = new Expr(ExprType.SingleOrAromatic);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Single);
            Assert.IsTrue(expr.Matches(bond));
            m_bond.SetupGet(n => n.IsAromatic).Returns(true);
            m_bond.SetupGet(n => n.Order).Returns(null);
            Assert.IsTrue(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestSingleOrAromaticF()
        {
            var expr = new Expr(ExprType.SingleOrAromatic);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Triple);
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestDoubleOrAromaticT()
        {
            var expr = new Expr(ExprType.DoubleOrAromatic);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.IsAromatic).Returns(true);
            m_bond.SetupGet(n => n.Order).Returns(null);
            Assert.IsTrue(expr.Matches(bond));
            m_bond.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsTrue(expr.Matches(bond));
            m_bond.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Triple);
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestSingleOrDoubleT()
        {
            var expr = new Expr(ExprType.SingleOrDouble);
            var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Single);
            Assert.IsTrue(expr.Matches(bond));
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsTrue(expr.Matches(bond));
            m_bond.SetupGet(n => n.Order).Returns(BondOrder.Triple);
            Assert.IsFalse(expr.Matches(bond));
        }

        [TestMethod()]
        public void TestNonBondExpr()
        {
            try
            {
                var expr = new Expr(ExprType.RingCount, 1);
                var m_bond = new Mock<IBond>(); var bond = m_bond.Object;
                expr.Matches(bond);
                Assert.Fail();
            }
            catch (ArgumentException)
            { }
        }

        [TestMethod()]
        public void TestToString()
        {
            Assert.AreEqual(nameof(ExprType.True), new Expr(ExprType.True).ToString());
            Assert.AreEqual($"{nameof(ExprType.Element)}=8", new Expr(ExprType.Element, 8).ToString());
            Assert.AreEqual($"{nameof(ExprType.Or)}({nameof(ExprType.Element)}=8,{nameof(ExprType.Degree)}=3)", new Expr(ExprType.Element, 8).Or(new Expr(ExprType.Degree, 3)).ToString());
            Assert.AreEqual($"{nameof(ExprType.And)}({nameof(ExprType.Element)}=8,{nameof(ExprType.Degree)}=3)", new Expr(ExprType.Element, 8).And(new Expr(ExprType.Degree, 3)).ToString());
            Assert.AreEqual($"{nameof(ExprType.Not)}({nameof(ExprType.Element)}=8)", new Expr(ExprType.Element, 8).Negate().ToString());
            Assert.AreEqual($"{nameof(ExprType.Recursive)}(...)", new Expr(ExprType.Recursive, null).ToString());
        }

        [TestMethod()]
        public void TestNegationOptimizations()
        {
            Assert.AreEqual(new Expr(ExprType.False), new Expr(ExprType.True).Negate());
            Assert.AreEqual(new Expr(ExprType.True), new Expr(ExprType.False).Negate());
            Assert.AreEqual(new Expr(ExprType.IsInChain), new Expr(ExprType.IsInRing).Negate());
            Assert.AreEqual(new Expr(ExprType.IsInRing), new Expr(ExprType.IsInChain).Negate());
            Assert.AreEqual(new Expr(ExprType.IsAromatic), new Expr(ExprType.IsAliphatic).Negate());
            Assert.AreEqual(new Expr(ExprType.IsAliphatic), new Expr(ExprType.IsAromatic).Negate());
            Assert.AreEqual(new Expr(ExprType.Not, new Expr(ExprType.Element, 8), null), new Expr(ExprType.Element, 8).Negate());
            Assert.AreEqual(new Expr(ExprType.Element, 8), new Expr(ExprType.Not, new Expr(ExprType.Element, 8), null).Negate());
            Assert.AreEqual(new Expr(ExprType.HasIsotope), new Expr(ExprType.HasUnspecifiedIsotope).Negate());
            Assert.AreEqual(new Expr(ExprType.HasUnspecifiedIsotope), new Expr(ExprType.HasIsotope).Negate());
        }

        [TestMethod()]
        public void TestLeftBalancedOr1()
        {
            Expr expr1 = new Expr(ExprType.Element, 9);
            Expr expr2 = new Expr(ExprType.Element, 17).Or(new Expr(ExprType.Element, 35));
            expr1.Or(expr2);
            Assert.AreEqual(ExprType.Element, expr1.Left.GetExprType());
        }

        [TestMethod()]
        public void TestLeftBalancedOr2()
        {
            Expr expr1 = new Expr(ExprType.Element, 9);
            Expr expr2 = new Expr(ExprType.Element, 17).Or(new Expr(ExprType.Element, 35));
            expr2.Or(expr1);
            Assert.AreEqual(ExprType.Element, expr2.Left.GetExprType());
        }

        [TestMethod()]
        public void TestLeftBalancedAnd1()
        {
            Expr expr1 = new Expr(ExprType.Element, 9);
            Expr expr2 = new Expr(ExprType.Degree, 2).And(new Expr(ExprType.HasImplicitHydrogen));
            expr1.And(expr2);
            Assert.AreEqual(ExprType.Element, expr1.Left.GetExprType());
        }

        [TestMethod()]
        public void TestLeftBalancedAnd2()
        {
            Expr expr1 = new Expr(ExprType.Element, 9);
            Expr expr2 = new Expr(ExprType.Degree, 2).And(new Expr(ExprType.HasImplicitHydrogen));
            expr2.And(expr1);
            Assert.AreEqual(ExprType.Degree, expr2.Left.GetExprType());
            Assert.AreEqual(ExprType.And, expr2.Right.GetExprType());
            Assert.AreEqual(ExprType.HasImplicitHydrogen, expr2.Right.Left.GetExprType());
            Assert.AreEqual(ExprType.Element, expr2.Right.Right.GetExprType());
        }

        [TestMethod()]
        public void AlwaysTrueAnd()
        {
            Assert.AreEqual(new Expr(ExprType.True), new Expr(ExprType.True).And(new Expr(ExprType.True)));
        }

        [TestMethod()]
        public void AlwaysFalseAnd()
        {
            Assert.AreEqual(new Expr(ExprType.False), new Expr(ExprType.False).And(new Expr(ExprType.True)));
        }

        [TestMethod()]
        public void RemoveFalseOr()
        {
            Assert.AreEqual(new Expr(ExprType.Degree, 2), new Expr(ExprType.Degree, 2).Or(new Expr(ExprType.False)));
            Assert.AreEqual(new Expr(ExprType.Degree, 2), new Expr(ExprType.Degree, 2).Or(new Expr(ExprType.True)));
            Assert.AreEqual(new Expr(ExprType.Degree, 2), new Expr(ExprType.False).Or(new Expr(ExprType.Degree, 2)));
            Assert.AreEqual(new Expr(ExprType.Degree, 2), new Expr(ExprType.True).Or(new Expr(ExprType.Degree, 2)));
        }
    }
}
