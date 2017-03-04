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

namespace NCDK.QSAR
{
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class DescriptorSpecificationTest : CDKTestCase
    {

        public DescriptorSpecificationTest()
            : base()
        { }

        private const string DESC_REF = "bla";
        private const string DESC_IMPL_TITLE = "bla2";
        private const string DESC_IMPL_VENDOR = "bla3";
        private const string DESC_IMPL_ID = "bla4";

        [TestMethod()]
        public void TestDescriptorSpecification_String_String_String_String()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Assert.IsNotNull(spec);
        }

        [TestMethod()]
        public void TestDescriptorSpecification_String_String_String()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_VENDOR);
            Assert.IsNotNull(spec);
            Assert.AreEqual(CDK.Version, spec.ImplementationIdentifier);
        }

        [TestMethod()]
        public void TestImplementationVendor()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Assert.AreEqual(DESC_IMPL_VENDOR, spec.ImplementationVendor);
        }

        [TestMethod()]
        public void TestSpecificationReference()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Assert.AreEqual(DESC_REF, spec.SpecificationReference);
        }

        [TestMethod()]
        public void TestImplementationIdentifier()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Assert.AreEqual(DESC_IMPL_ID, spec.ImplementationIdentifier);
        }

        [TestMethod()]
        public void TestImplementationTitle()
        {
            DescriptorSpecification spec = new DescriptorSpecification(DESC_REF, DESC_IMPL_TITLE, DESC_IMPL_ID,
                    DESC_IMPL_VENDOR);
            Assert.AreEqual(DESC_IMPL_TITLE, spec.ImplementationTitle);
        }
    }
}
