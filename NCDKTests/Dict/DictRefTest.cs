/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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

namespace NCDK.Dict
{
    /// <summary>
    /// Checks the functionality of the DictRef class.
    /// </summary>
    /// <seealso cref="DictRef"/>
    // @cdk.module test-standard
    [TestClass()]
    public class DictRefTest : CDKTestCase
    {
        public DictRefTest()
            : base()
        { }

        // test constructors

        [TestMethod()]
        public void TestDictRef_String_String()
        {
            DictRef dictRef = new DictRef("bar:foo", "bla");
            Assert.IsNotNull(dictRef);
        }

        [TestMethod()]
        public void TestGetType()
        {
            DictRef dictRef = new DictRef("bar:foo", "bla");
            Assert.AreEqual("bar:foo", dictRef.Type);
        }

        [TestMethod()]
        public void TestGetDictRef()
        {
            DictRef dictRef = new DictRef("bar:foo", "bla");
            Assert.AreEqual("bla", dictRef.Reference);
        }

        /// <summary>Test for RFC #9</summary>
        [TestMethod()]
        public void TestToString()
        {
            DictRef dictRef = new DictRef("bar:foo", "bla");
            string description = dictRef.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }
    }
}
