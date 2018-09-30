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
using NCDK.QSAR.Results;
using System;

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
        public void TestDescriptorValueDescriptorSpecificationArrayStringArrayObjectIDescriptorResultArrayString()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            var value = new DescriptorValue<Result<double>>(spec, Array.Empty<string>(), Array.Empty<object>(), new Result<double>(0.7),
                    new string[] { "bla" });
            Assert.IsNotNull(value);
        }

        [TestMethod()]
        public void TestGetValue()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Result<double> doubleVal = new Result<double>(0.7);
            var value = new DescriptorValue<Result<double>>(spec, Array.Empty<string>(), Array.Empty<object>(), doubleVal, new string[] { "bla" });
            Assert.AreEqual(doubleVal, value.Value);
        }

        [TestMethod()]
        public void TestGetSpecification()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Result<double> doubleVal = new Result<double>(0.7);
            var value = new DescriptorValue<Result<double>>(spec, Array.Empty<string>(), Array.Empty<object>(), doubleVal, new string[] { "bla" });
            Assert.AreEqual(spec, value.Specification);
        }

        [TestMethod()]
        public void TestGetParameters()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Result<double> doubleVal = new Result<double>(0.7);
            var value = new DescriptorValue<Result<double>>(spec, Array.Empty<string>(), Array.Empty<object>(), doubleVal, new string[] { "bla" });
            Assert.AreEqual(0, value.Parameters.Count);
        }

        [TestMethod()]
        public void TestGetParameterNames()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Result<double> doubleVal = new Result<double>(0.7);
            var value = new DescriptorValue<Result<double>>(spec, Array.Empty<string>(), Array.Empty<object>(), doubleVal, new string[] { "bla" });
            Assert.AreEqual(0, value.ParameterNames.Count);
        }

        [TestMethod()]
        public void TestGetNames()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Result<double> doubleVal = new Result<double>(0.7);
            var doubleVals = new ArrayResult<double> { 0.1, 0.2 };
            IDescriptorValue value;
            value = new DescriptorValue<Result<double>>(spec, Array.Empty<string>(), Array.Empty<object>(), doubleVal, new string[] { "bla" });
            Assert.AreEqual(1, value.Names.Count);
            value = new DescriptorValue<Result<double>>(spec, Array.Empty<string>(), Array.Empty<object>(), doubleVal, Array.Empty<string>());
            Assert.AreEqual(1, value.Names.Count);
            value = new DescriptorValue<Result<double>>(spec, Array.Empty<string>(), Array.Empty<object>(), doubleVal, null);
            Assert.AreEqual(1, value.Names.Count);
            value = new DescriptorValue<ArrayResult<double>>(spec, Array.Empty<string>(), Array.Empty<object>(), doubleVals, null);
            Assert.AreEqual(2, value.Names.Count);
        }

        [TestMethod()]
        public void TestGetException()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Result<double> doubleVal = new Result<double>(0.7);
            var value = new DescriptorValue<Result<double>>(spec, Array.Empty<string>(), Array.Empty<object>(), doubleVal, new string[] { "bla" },
                    new CDKException("A test exception"));
            Assert.IsInstanceOfType(value.Exception, typeof(CDKException));
        }
    }
}
