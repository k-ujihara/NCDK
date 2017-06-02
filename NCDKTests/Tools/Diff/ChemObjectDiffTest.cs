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
    public class ChemObjectDiffTest : CDKTestCase
    {
        [TestMethod()]
        public void TestMatchAgainstItself()
        {
            var m_atom1 = new Mock<IChemObject>(); IChemObject atom1 = m_atom1.Object;
            string result = ChemObjectDiff.Diff(atom1, atom1);
            AssertZeroLength(result);
        }

        [TestMethod()]
        public void TestDiff()
        {
            var m_atom1 = new Mock<IChemObject>(); IChemObject atom1 = m_atom1.Object;
            var m_atom2 = new Mock<IChemObject>(); IChemObject atom2 = m_atom2.Object;
            m_atom1.SetupGet(n => n.IsPlaced).Returns(false);
            m_atom1.SetupGet(n => n.IsVisited).Returns(false);
            m_atom2.SetupGet(n => n.IsPlaced).Returns(false);
            m_atom2.SetupGet(n => n.IsVisited).Returns(true);

            string result = ChemObjectDiff.Diff(atom1, atom2);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Length, "Expected non-zero-length result");
            AssertContains(result, "ChemObjectDiff");
            AssertContains(result, "F/T");
        }

        [TestMethod()]
        public void TestDifference()
        {
            var m_atom1 = new Mock<IChemObject>(); IChemObject atom1 = m_atom1.Object;
            var m_atom2 = new Mock<IChemObject>(); IChemObject atom2 = m_atom2.Object;
            m_atom1.SetupGet(n => n.IsPlaced).Returns(false);
            m_atom1.SetupGet(n => n.IsVisited).Returns(false);
            m_atom2.SetupGet(n => n.IsPlaced).Returns(false);
            m_atom2.SetupGet(n => n.IsVisited).Returns(true);

            IDifference difference = ChemObjectDiff.Difference(atom1, atom2);
            Assert.IsNotNull(difference);
        }
    }
}
