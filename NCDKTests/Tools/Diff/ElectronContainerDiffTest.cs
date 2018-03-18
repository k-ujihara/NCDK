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
    public class ElectronContainerDiffTest : CDKTestCase
    {
        [TestMethod()]
        public void TestMatchAgainstItself()
        {
            var m_atom1 = new Mock<IElectronContainer>(); IElectronContainer atom1 = m_atom1.Object;
            string result = ElectronContainerDiff.Diff(atom1, atom1);
            AssertZeroLength(result);
        }

        [TestMethod()]
        public void TestDiff()
        {
            var m_ec1 = new Mock<IElectronContainer>(); IElectronContainer ec1 = m_ec1.Object;
            var m_ec2 = new Mock<IElectronContainer>(); IElectronContainer ec2 = m_ec2.Object;
            m_ec1.SetupGet(n => n.ElectronCount).Returns(2);
            m_ec2.SetupGet(n => n.ElectronCount).Returns(3);

            string result = ElectronContainerDiff.Diff(ec1, ec2);
            Assert.IsNotNull(result);
            Assert.AreNotSame(0, result.Length);
            AssertContains(result, "ElectronContainerDiff");
            AssertContains(result, "eCount");
            AssertContains(result, "2/3");
        }

        [TestMethod()]
        public void TestDifference()
        {
            var m_ec1 = new Mock<IElectronContainer>(); IElectronContainer ec1 = m_ec1.Object;
            var m_ec2 = new Mock<IElectronContainer>(); IElectronContainer ec2 = m_ec2.Object;
            m_ec1.SetupGet(n => n.ElectronCount).Returns(2);
            m_ec2.SetupGet(n => n.ElectronCount).Returns(3);

            IDifference difference = ElectronContainerDiff.Difference(ec1, ec2);
            Assert.IsNotNull(difference);
        }
    }
}
