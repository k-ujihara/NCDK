/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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

namespace NCDK.AtomTypes.Mappers
{
    /// <summary>
    /// This class tests the mapper that maps CDK atom types to other atom type
    /// schemes.
    /// </summary>
    // @cdk.module test-atomtype
    [TestClass()]
    public class AtomTypeMapperTest : CDKTestCase
    {
        [TestMethod()]
        public void TestGetInstance_String()
        {
            AtomTypeMapper mapper = AtomTypeMapper.GetInstance("NCDK.Dict.Data.cdk-sybyl-mappings.owl");
            Assert.IsNotNull(mapper);
        }

        [TestMethod()]
        public void TestGetInstance_String_InputStream()
        {
            AtomTypeMapper mapper = AtomTypeMapper.GetInstance(
                "NCDK.Dict.Data.cdk-sybyl-mappings.owl",  
                ResourceLoader.GetAsStream("NCDK.Dict.Data.cdk-sybyl-mappings.owl"));
            Assert.IsNotNull(mapper);
        }

        [TestMethod()]
        public void TestGetMapping()
        {
            string mapping = "NCDK.Dict.Data.cdk-sybyl-mappings.owl";
            AtomTypeMapper mapper = AtomTypeMapper.GetInstance(mapping);
            Assert.IsNotNull(mapper);
            Assert.AreEqual(mapping, mapper.Mapping);
        }

        [TestMethod()]
        public void TestMapAtomType_String()
        {
            string mapping = "NCDK.Dict.Data.cdk-sybyl-mappings.owl";
            AtomTypeMapper mapper = AtomTypeMapper.GetInstance(mapping);
            Assert.IsNotNull(mapper);
            Assert.AreEqual("C.3", mapper.MapAtomType("C.sp3"));
        }
    }
}
