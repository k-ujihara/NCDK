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

namespace NCDK.Tools.Diff.Tree
{
    // @cdk.module test-diff 
    public class BooleanDifferenceTest : CDKTestCase
    {
        [TestMethod()]
        public void TestDiff()
        {
            IDifference result = BooleanDifference.Construct("Foo", true, false);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestSame()
        {
            IDifference result = BooleanDifference.Construct("Foo", false, false);
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void TestTwoNull()
        {
            IDifference result = BooleanDifference.Construct("Foo", null, null);
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void TestOneNull()
        {
            IDifference result = BooleanDifference.Construct("Foo", null, false);
            Assert.IsNotNull(result);

            result = BooleanDifference.Construct("Foo", false, null);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestToString()
        {
            IDifference result = BooleanDifference.Construct("Foo", true, false);
            string diffString = result.ToString();
            Assert.IsNotNull(diffString);
            AssertOneLiner(diffString);
        }
    }
}
