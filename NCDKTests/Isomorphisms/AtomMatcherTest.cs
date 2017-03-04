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
using System;

namespace NCDK.Isomorphisms
{
    /// <summary>
    // @author John May
    // @cdk.module test-isomorphism
    /// </summary>
    [TestClass()]
    public class AtomMatcherTest
    {
        [TestMethod()]
        public void AnyMatch()
        {
            AtomMatcher matcher = AtomMatcher.CreateAnyMatcher();
            var m_atom1 = new Mock<IAtom>();
            var m_atom2 = new Mock<IAtom>();
            var m_atom3 = new Mock<IAtom>();
            m_atom1.SetupGet(n => n.AtomicNumber).Returns(6);
            m_atom2.SetupGet(n => n.AtomicNumber).Returns(7);
            m_atom3.SetupGet(n => n.AtomicNumber).Returns(8);
            Assert.IsTrue(matcher.Matches(m_atom1.Object, m_atom2.Object));
            Assert.IsTrue(matcher.Matches(m_atom2.Object, m_atom1.Object));
            Assert.IsTrue(matcher.Matches(m_atom1.Object, m_atom3.Object));
            Assert.IsTrue(matcher.Matches(m_atom3.Object, m_atom1.Object));
            Assert.IsTrue(matcher.Matches(m_atom2.Object, m_atom3.Object));
            Assert.IsTrue(matcher.Matches(m_atom1.Object, null));
            Assert.IsTrue(matcher.Matches(null, null));
        }

        [TestMethod()]
        public void ElementMatch()
        {
            AtomMatcher matcher = AtomMatcher.CreateElementMatcher();
            var m_atom1 = new Mock<IAtom>();
            var m_atom2 = new Mock<IAtom>();
            m_atom1.SetupGet(n => n.AtomicNumber).Returns(6);
            m_atom2.SetupGet(n => n.AtomicNumber).Returns(6);
            Assert.IsTrue(matcher.Matches(m_atom1.Object, m_atom2.Object));
            Assert.IsTrue(matcher.Matches(m_atom2.Object, m_atom1.Object));
        }

        [TestMethod()]
        public void ElementMismatch()
        {
            AtomMatcher matcher = AtomMatcher.CreateElementMatcher();
            var m_atom1 = new Mock<IAtom>();
            var m_atom2 = new Mock<IAtom>();
            m_atom1.SetupGet(n => n.AtomicNumber).Returns(6);
            m_atom2.SetupGet(n => n.AtomicNumber).Returns(8);
            Assert.IsFalse(matcher.Matches(m_atom1.Object, m_atom2.Object));
            Assert.IsFalse(matcher.Matches(m_atom2.Object, m_atom1.Object));
        }

        [TestMethod()]
        public void ElementPseudo()
        {
            AtomMatcher matcher = AtomMatcher.CreateElementMatcher();
            var m_atom1 = new Mock<IPseudoAtom>();
            var m_atom2 = new Mock<IPseudoAtom>();
            Assert.IsTrue(matcher.Matches(m_atom1.Object, m_atom2.Object));
            Assert.IsTrue(matcher.Matches(m_atom2.Object, m_atom1.Object));
        }

        [TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void ElementError()
        {
            AtomMatcher matcher = AtomMatcher.CreateElementMatcher();
            var m_atom1 = new Mock<IAtom>();
            var m_atom2 = new Mock<IAtom>();
            m_atom1.SetupGet(n => n.AtomicNumber).Returns((int?)null);
            m_atom2.SetupGet(n => n.AtomicNumber).Returns((int?)null);
            matcher.Matches(m_atom1.Object, m_atom2.Object);
        }

        [TestMethod()]
        public void QueryMatch()
        {
            AtomMatcher matcher = AtomMatcher.CreateQueryMatcher();
            var m_atom1 = new Mock<IQueryAtom>();
            var m_atom2 = new Mock<IAtom>();
            var m_atom3 = new Mock<IAtom>();
            m_atom1.Setup(n => n.Matches(m_atom2.Object)).Returns(true);
            m_atom1.Setup(n => n.Matches(m_atom3.Object)).Returns(false);
            Assert.IsTrue(matcher.Matches(m_atom1.Object, m_atom2.Object));
            Assert.IsFalse(matcher.Matches(m_atom1.Object, m_atom3.Object));
        }
    }
}
