/* Copyright (C) 2005-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Config.Isotopes;

namespace NCDK.Config.Isotope
{
    // @cdk.module test-extra
    [TestClass()]
    public class IsotopeHandlerTest : CDKTestCase
    {
        // serious testing is done in IsotopeFactoryTest; the factory
        // requires this class to work properly. But nevertheless:

        [TestMethod()]
        public void TestIsotopeHandler_IChemObjectBuilder()
        {
            IsotopeHandler handler = new IsotopeHandler();
            Assert.IsNotNull(handler);
        }

        [TestMethod()]
        public void TestGetIsotopes()
        {
            IsotopeHandler handler = new IsotopeHandler();
            // nothing is read
            Assert.IsNotNull(handler);
            Assert.IsNull(handler.Isotopes);
        }

        [TestMethod()]
        public void TestStartDocument()
        {
            IsotopeHandler handler = new IsotopeHandler();
            // nothing is read, but Vector is initialized
            Assert.IsNotNull(handler);
            Assert.IsNull(handler.Isotopes);
        }

        [TestMethod()]
        public void TestCharacters_arraychar_int_int()
        {
            // nothing I can test here that IsotopeFactoryTest doesn't do
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void TestEndElement_String_String_String()
        {
            // nothing I can test here that IsotopeFactoryTest doesn't do
            Assert.IsTrue(true);
        }
    }
}
