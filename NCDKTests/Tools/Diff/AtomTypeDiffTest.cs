/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Tools.Diff.Tree;

namespace NCDK.Tools.Diff
{
    // @cdk.module test-diff 
    public class AtomTypeDiffTest : CDKTestCase
    {
        [TestMethod()]
        public void TestMatchAgainstItself()
        {
            var m_element1 = new Mock<IAtomType>(); IAtomType element1 = m_element1.Object;
            string result = AtomTypeDiff.Diff(element1, element1);
            AssertZeroLength(result);
        }

        [TestMethod()]
        public void TestDiff()
        {
            var m_element1 = new Mock<IAtomType>(); IAtomType element1 = m_element1.Object;
            var m_element2 = new Mock<IAtomType>(); IAtomType element2 = m_element2.Object;
            m_element1.SetupGet(n => n.Hybridization).Returns(Hybridization.Planar3);
            m_element2.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3);

            string result = AtomTypeDiff.Diff(element1, element2);
            Assert.IsNotNull(result);
            Assert.AreNotSame(0, result.Length);
            AssertContains(result, "AtomTypeDiff");
            AssertContains(result, "PLANAR3/SP3");
        }

        [TestMethod()]
        public void TestDifference()
        {
            var m_element1 = new Mock<IAtomType>(); IAtomType element1 = m_element1.Object;
            var m_element2 = new Mock<IAtomType>(); IAtomType element2 = m_element2.Object;
            m_element1.SetupGet(n => n.Hybridization).Returns(Hybridization.Planar3);
            m_element2.SetupGet(n => n.Hybridization).Returns(Hybridization.SP3);

            IDifference difference = AtomTypeDiff.Difference(element1, element2);
            Assert.IsNotNull(difference);
        }
    }
}
