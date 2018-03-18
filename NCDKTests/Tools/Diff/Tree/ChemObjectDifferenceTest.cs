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
    public class ChemObjectDifferenceTest : CDKTestCase
    {
        [TestMethod()]
        public void TestToString()
        {
            ChemObjectDifference diff = new ChemObjectDifference("AtomTypeDiff");
            string diffString = diff.ToString();
            Assert.IsNotNull(diffString);
            Assert.AreEqual(0, diffString.Length);

            diff.AddChild(StringDifference.Construct("Foo", "bar", "bar1"));
            diffString = diff.ToString();
            Assert.IsNotNull(diffString);
            AssertOneLiner(diffString);
            AssertContains(diffString, "AtomTypeDiff");
            AssertContains(diffString, "{");
            AssertContains(diffString, "}");
        }
    }
}
