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

namespace NCDK.Hash
{
    // @author John May
    // @cdk.module test-hash
    [TestClass()]
    public class AtomSuppressionTest
    {
        [TestMethod()]
        public void TestGetUnsuppressed()
        {
            AtomSuppression suppression = AtomSuppression.Unsuppressed;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            Suppressed suppressed = suppression.Suppress(container);
            Assert.IsFalse(suppressed.Contains(0));
            Assert.IsFalse(suppressed.Contains(1));
            Assert.IsFalse(suppressed.Contains(2));
            Assert.IsFalse(suppressed.Contains(3));
            Assert.IsFalse(suppressed.Contains(4));
        }

        [TestMethod()]
        public void TestAnyHydrogens()
        {
            AtomSuppression suppression = AtomSuppression.AnyHydrogens;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(5);

            var m_carbon = new Mock<IAtom>(); var carbon = m_carbon.Object;
            var m_hydrogen = new Mock<IAtom>(); var hydrogen = m_hydrogen.Object;

            m_carbon.SetupGet(n => n.Symbol).Returns("C");
            m_hydrogen.SetupGet(n => n.Symbol).Returns("H");

            m_container.SetupGet(n => n.Atoms[0]).Returns(carbon);
            m_container.SetupGet(n => n.Atoms[1]).Returns(hydrogen);
            m_container.SetupGet(n => n.Atoms[2]).Returns(carbon);
            m_container.SetupGet(n => n.Atoms[3]).Returns(carbon);
            m_container.SetupGet(n => n.Atoms[4]).Returns(hydrogen);

            Suppressed suppressed = suppression.Suppress(container);
            Assert.IsFalse(suppressed.Contains(0));
            Assert.IsTrue(suppressed.Contains(1));
            Assert.IsFalse(suppressed.Contains(2));
            Assert.IsFalse(suppressed.Contains(3));
            Assert.IsTrue(suppressed.Contains(4));
        }

        [TestMethod()]
        public void TestAnyPseudos()
        {
            AtomSuppression suppression = AtomSuppression.AnyPseudos;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(5);

            var m_carbon = new Mock<IAtom>(); var carbon = m_carbon.Object;
            var m_pseudo = new Mock<IPseudoAtom>(); var pseudo = m_pseudo.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(carbon);
            m_container.SetupGet(n => n.Atoms[1]).Returns(pseudo);
            m_container.SetupGet(n => n.Atoms[2]).Returns(carbon);
            m_container.SetupGet(n => n.Atoms[3]).Returns(carbon);
            m_container.SetupGet(n => n.Atoms[4]).Returns(pseudo);

            Suppressed suppressed = suppression.Suppress(container);
            Assert.IsFalse(suppressed.Contains(0));
            Assert.IsTrue(suppressed.Contains(1));
            Assert.IsFalse(suppressed.Contains(2));
            Assert.IsFalse(suppressed.Contains(3));
            Assert.IsTrue(suppressed.Contains(4));
        }
    }
}
