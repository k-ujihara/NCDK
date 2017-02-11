/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Graphs.Rebond
{
    // @cdk.module test-standard
    [TestClass()]
    public class PointTest : CDKTestCase
    {
        public PointTest()
                : base()
        { }

        [TestMethod()]
        public void TestPoint_Double_double_double()
        {
            Point point = new Point(0.1, 0.2, 0.3);
            Assert.IsNotNull(point);
        }

        [TestMethod()]
        public void TestGetDimValue_int()
        {
            Point point = new Point(0.1, 0.2, 0.3);
            Assert.AreEqual(0.1, point.GetDimValue(0), 0.0001);
            Assert.AreEqual(0.2, point.GetDimValue(1), 0.0001);
            Assert.AreEqual(0.3, point.GetDimValue(2), 0.0001);
        }

        [TestMethod()]
        public void TestToString()
        {
            Point point = new Point(0.1, 0.2, 0.3);
            Assert.AreEqual("<0.1,0.2,0.3>", point.ToString());
        }
    }
}
