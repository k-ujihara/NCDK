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
using static NCDK.Graphs.Rebond.RebondTool;

namespace NCDK.Graphs.Rebond
{
    // @cdk.module test-standard
    [TestClass()]
    public class BsptTest : CDKTestCase
    {
        public BsptTest()
                : base()
        { }

        [TestMethod()]
        public void TestToString()
        {
            var bspt = new Bspt<ITupleAtom>(3);
            Assert.IsNotNull(bspt.ToString());
        }

        [TestMethod()]
        public void TestBspt()
        {
            var bspt = new Bspt<ITupleAtom>(3);
            Assert.IsNotNull(bspt);
        }
    }
}

