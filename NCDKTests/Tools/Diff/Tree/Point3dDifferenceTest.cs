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
    public class Point3dDifferenceTest : CDKTestCase
    {
        [TestMethod()]
        public void TestDiff()
        {
            Vector3 foo = new Vector3(1.0, 2.0, 4.5);
            Vector3 bar = new Vector3(1.0, 5.0, 8.3);
            IDifference result = Point3dDifference.Construct("Foo", foo, bar);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestSame()
        {
            Vector3 foo = new Vector3(1.0, 2.0, 4.5);
            Vector3 bar = new Vector3(1.0, 2.0, 4.5);
            IDifference result = Point3dDifference.Construct("Foo", foo, bar);
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void TestTwoNull()
        {
            IDifference result = Point3dDifference.Construct("Foo", null, null);
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void TestOneNull()
        {
            Vector3 bar = new Vector3(1.0, 5.0, 8.3);
            IDifference result = Point3dDifference.Construct("Foo", null, bar);
            Assert.IsNotNull(result);

            result = Point3dDifference.Construct("Foo", bar, null);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestToString()
        {
            Vector3 bar = new Vector3(1.0, 5.0, 8.3);
            IDifference result = Point3dDifference.Construct("Foo", null, bar);
            string diffString = result.ToString();
            Assert.IsNotNull(diffString);
            AssertOneLiner(diffString);
        }
    }
}
