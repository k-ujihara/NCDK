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
    public class BooleanArrayDifferenceTest : CDKTestCase
    {
        [TestMethod()]
        public void TestDiff()
        {
            IDifference result = BooleanArrayDifference.Construct("Foo", new bool[] { true, true }, new bool[]{false, false});
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestSame()
        {
            IDifference result = BooleanArrayDifference.Construct("Foo", new bool[] { false, false }, new bool[]{false, false});
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void TestTwoNull()
        {
            IDifference result = BooleanArrayDifference.Construct("Foo", null, null);
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void TestOneNull()
        {
            IDifference result = BooleanArrayDifference.Construct("Foo", null, new bool[] { false, false });
            Assert.IsNotNull(result);

            result = BooleanArrayDifference.Construct("Foo", new bool[] { false, false }, null);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestToString()
        {
            IDifference result = BooleanArrayDifference.Construct("Foo", new bool[] { true }, new bool[] { false });
            string diffString = result.ToString();
            Assert.IsNotNull(diffString);
            AssertOneLiner(diffString);
        }
    }
}
