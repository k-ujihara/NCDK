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
using NCDK.QSAR.Result;

namespace NCDK.QSAR
{
    // @cdk.module test-standard
    [TestClass()]
    public class DescriptorValueTest : CDKTestCase
    {

        public DescriptorValueTest()
            : base()
        { }

        private const string DESC_REF = "bla";
        private const string DESC_IMPL_TITLE = "bla2";
        private const string DESC_IMPL_VENDOR = "bla3";
        private const string DESC_IMPL_ID = "bla4";

        [TestMethod()]
        public void TestDescriptorValue_DescriptorSpecification_arrayString_arrayObject_IDescriptorResult_arrayString()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            DescriptorValue value = new DescriptorValue(spec, new string[0], new object[0], new DoubleResult(0.7),
                    new string[] { "bla" });
            Assert.IsNotNull(value);
        }

        [TestMethod()]
        public void TestGetValue()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            DoubleResult doubleVal = new DoubleResult(0.7);
            DescriptorValue value = new DescriptorValue(spec, new string[0], new object[0], doubleVal, new string[] { "bla" });
            Assert.AreEqual(doubleVal, value.GetValue());
        }

        [TestMethod()]
        public void TestGetSpecification()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            DoubleResult doubleVal = new DoubleResult(0.7);
            DescriptorValue value = new DescriptorValue(spec, new string[0], new object[0], doubleVal, new string[] { "bla" });
            Assert.AreEqual(spec, value.Specification);
        }

        [TestMethod()]
        public void TestGetParameters()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            DoubleResult doubleVal = new DoubleResult(0.7);
            DescriptorValue value = new DescriptorValue(spec, new string[0], new object[0], doubleVal, new string[] { "bla" });
            Assert.AreEqual(0, value.Parameters.Length);
        }

        [TestMethod()]
        public void TestGetParameterNames()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            DoubleResult doubleVal = new DoubleResult(0.7);
            DescriptorValue value = new DescriptorValue(spec, new string[0], new object[0], doubleVal, new string[] { "bla" });
            Assert.AreEqual(0, value.ParameterNames.Length);
        }

        [TestMethod()]
        public void TestGetNames()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            DoubleResult doubleVal = new DoubleResult(0.7);
            DescriptorValue value = new DescriptorValue(spec, new string[0], new object[0], doubleVal, new string[] { "bla" });
            Assert.AreEqual(1, value.Names.Length);
        }

        [TestMethod()]
        public void TestGetException()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            DoubleResult doubleVal = new DoubleResult(0.7);
            DescriptorValue value = new DescriptorValue(spec, new string[0], new object[0], doubleVal, new string[] { "bla" },
                    new CDKException("A test exception"));
            Assert.AreEqual(typeof(CDKException).ToString() + ": A test exception", value.GetException()
                    .ToString());
        }
    }
}
