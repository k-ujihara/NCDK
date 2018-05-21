/* Copyright (C) 2012  Egon Willighagen <egonw@users.sf.net>
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

namespace NCDK.Dict
{
    // @cdk.module test-dict
    [TestClass()]
    public abstract class AbstractEntryTest : CDKTestCase
    {
        private Entry testClass;

        protected void SetTestClass(Entry testClass)
        {
            this.testClass = testClass;
        }

        protected Entry GetTestClass()
        {
            return this.testClass;
        }

        [TestMethod()]
        public virtual void TestSetTestClass()
        {
            Assert.IsNotNull(this.testClass);
        }

        [TestMethod()]
        public virtual void TestToString()
        {
            Entry entry = GetTestClass();
            entry.Id = "testid";
            entry.Label = "testTerm";
            Assert.IsNotNull(entry);
            Assert.AreEqual("Entry[testid](testTerm)", entry.ToString());
        }

        [TestMethod()]
        public virtual void TestLabel()
        {
            Entry entry = GetTestClass();
            Assert.AreEqual("", entry.Label);
            entry.Label = "label";
            Assert.AreEqual("label", entry.Label);
        }

        [TestMethod()]
        public virtual void TestID()
        {
            Entry entry = GetTestClass();
            Assert.AreEqual("", entry.Id);
            entry.Id = "identifier";
            Assert.AreEqual("identifier", entry.Id);
        }

        [TestMethod()]
        public virtual void TestDefinition()
        {
            Entry entry = GetTestClass();
            Assert.IsNull(entry.Definition);
            entry.Definition = "This is a definition.";
            Assert.AreEqual("This is a definition.", entry.Definition);
        }

        [TestMethod()]
        public virtual void TestDescriptorMetadata()
        {
            Entry entry = GetTestClass();
            Assert.IsNotNull(entry.DescriptorMetadata);
            IList<string> metadata = entry.DescriptorMetadata;
            Assert.AreEqual(0, metadata.Count);
            entry.AddDescriptorMetadata("This entry was written by me.");
            metadata = entry.DescriptorMetadata;
            Assert.AreEqual(1, metadata.Count);
        }

        [TestMethod()]
        public virtual void TestDescription()
        {
            Entry entry = GetTestClass();
            Assert.IsNull(entry.Description);
            entry.Description = "This is a description.";
            Assert.AreEqual("This is a description.", entry.Description);
        }

        [TestMethod()]
        public virtual void TestClassName()
        {
            Entry entry = GetTestClass();
            Assert.IsNull(entry.ClassName);
            entry.ClassName = "NCDK.DoesNotExist";
            Assert.AreEqual("NCDK.DoesNotExist", entry.ClassName);
        }

        [TestMethod()]
        public virtual void TestRawContent()
        {
            Entry entry = GetTestClass();
            Assert.IsNull(entry.RawContent);
            object someObject = (double)5;
            entry.RawContent = someObject;
            Assert.AreEqual(someObject, entry.RawContent);
        }
    }
}