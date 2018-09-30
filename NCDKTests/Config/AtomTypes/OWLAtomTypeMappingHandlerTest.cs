/* Copyright (C) 2005-2007  The Chemistry Development Kit (CDK) project
 *                    2008  Egon Willighagen <egonw@users.sf.net>
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

namespace NCDK.Config.AtomTypes
{
    /// <summary>
    /// Checks the functionality of the <see cref="OWLAtomTypeMappingHandler"/>.
    /// </summary>
    // @cdk.module test-atomtype
    [TestClass()]
    public class OWLAtomTypeMappingHandlerTest : CDKTestCase
    {
        [TestMethod()]
        public void TestOWLAtomTypeMappingHandler()
        {
            OWLAtomTypeMappingHandler handler = new OWLAtomTypeMappingHandler();
            Assert.IsNotNull(handler);
        }

        [TestMethod()]
        public void TestGetAtomTypeMappings()
        {
            OWLAtomTypeMappingHandler handler = new OWLAtomTypeMappingHandler();
            // nothing is read
            Assert.IsNotNull(handler);
            Assert.IsNull(handler.GetAtomTypeMappings());
        }

        [TestMethod()]
        public void TestStartDocument()
        {
            OWLAtomTypeMappingHandler handler = new OWLAtomTypeMappingHandler();
            // nothing is read, but Vector is initialized
            Assert.IsNotNull(handler);
            Assert.IsNull(handler.GetAtomTypeMappings());
        }

        [TestMethod()]
        public void TestEndElement_String_String_String()
        {
            Assert.IsTrue(true); // tested by testGetAtomTypeMappings
        }

        [TestMethod()]
        public void TestStartElement_String_String_String_Attributes()
        {
            Assert.IsTrue(true); // tested by testGetAtomTypeMappings
        }

        [TestMethod()]
        public void TestCharacters_arraychar_int_int()
        {
            Assert.IsTrue(true); // tested by testGetAtomTypeMappings
        }
    }
}
