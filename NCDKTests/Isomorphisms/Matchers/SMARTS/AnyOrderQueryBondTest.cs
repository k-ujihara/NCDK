/* Copyright (C) 2013  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Default;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /**
     * Checks the functionality of the IsomorphismTester
     *
     * @cdk.module test-smarts
     */
    [TestClass()]
    public class AnyOrderQueryBondTest : CDKTestCase
    {
        /**
         * @cdk.bug 1305
         */
        [TestMethod()]
        public void TestMatches()
        {
            IBond testBond = null;
            AnyOrderQueryBond matcher = new AnyOrderQueryBond(new Mock<IChemObjectBuilder>().Object);
            Assert.IsFalse(matcher.Matches(testBond));
        }

        [TestMethod()]
        public void TestAnyOrder()
        {
            AnyOrderQueryBond matcher = new AnyOrderQueryBond(new Mock<IChemObjectBuilder>().Object);
            IBond testBond = new Bond();
            foreach (var order in BondOrder.Values)
            {
                testBond.Order = order;
                Assert.IsTrue(matcher.Matches(testBond));
            }
        }
    }
}
