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
using NCDK.Numerics;

namespace NCDK.Tools.Diff.Tree
{
    // @cdk.module test-diff 
    public class Point2dDifferenceTest : CDKTestCase
    {
        [TestMethod()]
        public void TestDiff()
        {
            Vector2 foo = new Vector2(1.0, 2.0);
            Vector2 bar = new Vector2(1.0, 5.0);
            IDifference result = Point2dDifference.Construct("Foo", foo, bar);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestSame()
        {
            Vector2 foo = new Vector2(1.0, 2.0);
            Vector2 bar = new Vector2(1.0, 2.0);
            IDifference result = Point2dDifference.Construct("Foo", foo, bar);
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void TestTwoNull()
        {
            IDifference result = Point2dDifference.Construct("Foo", null, null);
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void TestOneNull()
        {
            Vector2 bar = new Vector2(1.0, 2.0);
            IDifference result = Point2dDifference.Construct("Foo", null, bar);
            Assert.IsNotNull(result);

            result = Point2dDifference.Construct("Foo", bar, null);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestToString()
        {
            Vector2 bar = new Vector2(1.0, 5.0);
            IDifference result = Point2dDifference.Construct("Foo", null, bar);
            string diffString = result.ToString();
            Assert.IsNotNull(diffString);
            AssertOneLiner(diffString);
        }
    }
}
