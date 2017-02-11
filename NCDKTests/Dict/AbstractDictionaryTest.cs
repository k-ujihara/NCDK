/* Copyright (C) 2012-2013  Egon Willighagen <egonw@users.sf.net>
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

namespace NCDK.Dict
{
    /**
	 * @cdk.module test-dict
	 */
	[TestClass()]
    public abstract class AbstractDictionaryTest
    {
        private DictionaryMap testClass;

        protected void SetTestClass(DictionaryMap testClass)
        {
            this.testClass = testClass;
        }

        protected DictionaryMap GetTestClass()
        {
            return this.testClass;
        }

        [TestMethod()]
        public void TestSetTestClass()
        {
            Assert.IsNotNull(this.testClass);
        }

        [TestMethod()]
        public void TestNS()
        {
            DictionaryMap dict = GetTestClass();
            Assert.IsNotNull(dict);
            Assert.IsNull(dict.NS);
            dict.NS = "http://www.namespace.example.org/";
            Assert.AreEqual("http://www.namespace.example.org/", dict.NS);
        }

        [TestMethod()]
        public void TestAddEntry()
        {
            DictionaryMap dict = GetTestClass();
            Assert.IsNotNull(dict);
            Assert.AreEqual(0, dict.Count);
            Assert.IsFalse(dict.HasEntry("someidentifier"));
            Entry entry = new Entry();
            entry.Id = "someidentifier";
            dict.AddEntry(entry);
            Assert.AreEqual(1, dict.Count);
            Assert.IsTrue(dict.HasEntry("someidentifier"));
            Assert.AreEqual(entry, dict.GetEntry("someidentifier"));
        }
    }
}
