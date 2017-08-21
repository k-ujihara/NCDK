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

namespace NCDK.QSAR.Results
{
    // @cdk.module test-standard
    [TestClass()]
    public class BooleanResultTest : CDKTestCase
    {
        public BooleanResultTest()
            : base()
        { }

        // well, these tests are not shocking...

        [TestMethod()]
        public void TestBooleanResult_bool()
        {
            Result<bool> result = new Result<bool>(true);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void TestBooleanValue()
        {
            Assert.IsTrue(new Result<bool>(true).Value);
            Assert.IsFalse(new Result<bool>(false).Value);
        }

        [TestMethod()]
        public void TestToString()
        {
            Assert.AreEqual(true.ToString(), new Result<bool>(true).ToString());
            Assert.AreEqual(false.ToString(), new Result<bool>(false).ToString());
        }
    }
}
