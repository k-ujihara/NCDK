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
using System.Collections.Generic;

namespace NCDK.Tools.Diff.Tree
{
    // @cdk.module test-diff 
    public class AbstractDifferenceListTest : CDKTestCase
    {
        [TestMethod()]
        public void TestConstructor()
        {
            DifferenceClass diffClass = new DifferenceClass();
            Assert.IsNotNull(diffClass);
        }

        [TestMethod()]
        public void TestAddChild()
        {
            DifferenceClass diffClass = new DifferenceClass();
            diffClass.AddChild(StringDifference.Construct("Foo", "Bar1", "Bar2"));
            Assert.AreEqual(1, diffClass.ChildCount());

            diffClass.AddChild(null);
            Assert.AreEqual(1, diffClass.ChildCount());
        }

        [TestMethod()]
        public void TestChildDiffs()
        {
            DifferenceClass diffClass = new DifferenceClass();
            List<IDifference> diffs = new List<IDifference>();
            diffs.Add(StringDifference.Construct("Foo", "Bar1", "Bar2"));
            diffs.Add(IntegerDifference.Construct("Foo", 1, 2));
            diffClass.AddChildren(diffs);
            Assert.AreEqual(2, diffClass.ChildCount());
            var diffs2 = diffClass.GetChildren().GetEnumerator();
            int count = 0;
            while (diffs2.MoveNext())
            {
                var dummy = diffs2.Current;
                count++;
            }
            Assert.AreEqual(2, count);
        }

        private class DifferenceClass : AbstractDifferenceList
        {
        }
    }
}
