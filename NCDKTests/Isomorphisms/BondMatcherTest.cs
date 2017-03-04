/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Isomorphisms.Matchers;

namespace NCDK.Isomorphisms
{
    /// <summary>
    // @author John May
    // @cdk.module test-isomorphism
    /// </summary>
    [TestClass()]
    public class BondMatcherTest
    {

        [TestMethod()]
        public void AnyMatch()
        {
            BondMatcher matcher = BondMatcher.CreateAnyMatcher();
            IBond bond1 = new Mock<IBond>().Object;
            IBond bond2 = new Mock<IBond>().Object;
            IBond bond3 = new Mock<IBond>().Object;
            Assert.IsTrue(matcher.Matches(bond1, bond2));
            Assert.IsTrue(matcher.Matches(bond2, bond1));
            Assert.IsTrue(matcher.Matches(bond1, bond3));
            Assert.IsTrue(matcher.Matches(bond1, null));
            Assert.IsTrue(matcher.Matches(null, null));
        }

        [TestMethod()]
        public void AromaticMatch()
        {
            BondMatcher matcher = BondMatcher.CreateOrderMatcher();
            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;
            m_bond1.SetupGet(n => n.IsAromatic).Returns(true);
            m_bond2.SetupGet(n => n.IsAromatic).Returns(true);
            m_bond1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_bond2.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsTrue(matcher.Matches(bond1, bond2));
            Assert.IsTrue(matcher.Matches(bond2, bond1));
        }

        [TestMethod()]
        public void AliphaticMatch()
        {
            BondMatcher matcher = BondMatcher.CreateOrderMatcher();
            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;
            m_bond1.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond2.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_bond2.SetupGet(n => n.Order).Returns(BondOrder.Single);
            Assert.IsTrue(matcher.Matches(bond1, bond2));
            Assert.IsTrue(matcher.Matches(bond2, bond1));
        }

        [TestMethod()]
        public void AromaticStrictMatch()
        {
            BondMatcher matcher = BondMatcher.CreateStrictOrderMatcher();
            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;
            m_bond1.SetupGet(n => n.IsAromatic).Returns(true);
            m_bond2.SetupGet(n => n.IsAromatic).Returns(true);
            m_bond1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_bond2.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsTrue(matcher.Matches(bond1, bond2));
            Assert.IsTrue(matcher.Matches(bond2, bond1));
        }

        [TestMethod()]
        public void AliphaticStrictMatch()
        {
            BondMatcher matcher = BondMatcher.CreateStrictOrderMatcher();
            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;
            m_bond1.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond2.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_bond2.SetupGet(n => n.Order).Returns(BondOrder.Single);
            Assert.IsTrue(matcher.Matches(bond1, bond2));
            Assert.IsTrue(matcher.Matches(bond2, bond1));
        }

        [TestMethod()]
        public void AliphaticMismatch_aromatic()
        {
            BondMatcher matcher = BondMatcher.CreateOrderMatcher();
            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;
            m_bond1.SetupGet(n => n.IsAromatic).Returns(true);
            m_bond2.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_bond2.SetupGet(n => n.Order).Returns(BondOrder.Single);
            Assert.IsTrue(matcher.Matches(bond1, bond2));
            Assert.IsTrue(matcher.Matches(bond2, bond1));
        }

        [TestMethod()]
        public void AliphaticStrictMismatch_aromatic()
        {
            BondMatcher matcher = BondMatcher.CreateStrictOrderMatcher();
            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;
            m_bond1.SetupGet(n => n.IsAromatic).Returns(true);
            m_bond2.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_bond2.SetupGet(n => n.Order).Returns(BondOrder.Single);
            Assert.IsFalse(matcher.Matches(bond1, bond2));
            Assert.IsFalse(matcher.Matches(bond2, bond1));
        }

        [TestMethod()]
        public void AliphaticMismatch_order()
        {
            BondMatcher matcher = BondMatcher.CreateOrderMatcher();
            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;
            m_bond1.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond2.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_bond2.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsFalse(matcher.Matches(bond1, bond2));
            Assert.IsFalse(matcher.Matches(bond2, bond1));
        }

        [TestMethod()]
        public void AliphaticStrictMismatch_order()
        {
            BondMatcher matcher = BondMatcher.CreateStrictOrderMatcher();
            var m_bond1 = new Mock<IBond>(); IBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;
            m_bond1.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond2.SetupGet(n => n.IsAromatic).Returns(false);
            m_bond1.SetupGet(n => n.Order).Returns(BondOrder.Single);
            m_bond2.SetupGet(n => n.Order).Returns(BondOrder.Double);
            Assert.IsFalse(matcher.Matches(bond1, bond2));
            Assert.IsFalse(matcher.Matches(bond2, bond1));
        }

        [TestMethod()]
        public void QueryMatch()
        {
            BondMatcher matcher = BondMatcher.CreateQueryMatcher();
            var m_bond1 = new Mock<IQueryBond>(); IQueryBond bond1 = m_bond1.Object;
            var m_bond2 = new Mock<IBond>(); IBond bond2 = m_bond2.Object;
            var m_bond3 = new Mock<IBond>(); IBond bond3 = m_bond3.Object;
            m_bond1.Setup(n => n.Matches(bond2)).Returns(true);
            m_bond1.Setup(n => n.Matches(bond3)).Returns(false);
            Assert.IsTrue(matcher.Matches(bond1, bond2));
            Assert.IsFalse(matcher.Matches(bond1, bond3));
        }
    }
}
