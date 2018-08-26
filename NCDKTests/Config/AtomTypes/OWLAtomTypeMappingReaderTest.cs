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
using System.IO;
using System.Linq;

namespace NCDK.Config.AtomTypes
{
    /// <summary>
    /// Checks the functionality of the <see cref="OWLAtomTypeMappingReader"/>.
    /// </summary>
    // @cdk.module test-atomtype
    [TestClass()]
    public class OWLAtomTypeMappingReaderTest : CDKTestCase
    {
        private const string OWL_CONTENT = "<?xml version=\"1.0\"?>" + "<!DOCTYPE rdf:RDF ["
            + "  <!ENTITY rdf  \"http://www.w3.org/1999/02/22-rdf-syntax-ns#\" >"
            + "  <!ENTITY elem \"http://cdk.sf.net/ontologies/elements#\" >"
            + "  <!ENTITY at   \"http://cdk.sf.net/ontologies/atomtypes#\" >"
            + "  <!ENTITY cdkat \"http://cdk.sf.net/ontologies/atomtypes/cdk#\" >"
            + "  <!ENTITY sybylat \"http://cdk.sf.net/ontologies/atomtypes/sybyl#\" >"
            + "  <!ENTITY atm  \"http://cdk.sf.net/ontologies/atomtypemappings#\" >"
            + "  <!ENTITY owl  \"http://www.w3.org/2002/07/owl#\" >" + "]>"
            + "<rdf:RDF xmlns=\"&cdkat;\" xml:base=\"&cdkat;\""
            + "         xmlns:at=\"&at;\"" + "         xmlns:elem=\"&elem;\""
            + "         xmlns:rdf=\"&rdf;\"" + "         xmlns:atm=\"&atm;\""
            + "         xmlns:owl=\"&owl;\"" + ">"
            + "  <owl:ObjectProperty rdf:about=\"&atm;mapsToType\"/>"
            + "  <owl:ObjectProperty rdf:about=\"&atm;equivalentAsType\"/>"
            + "  <owl:Thing rdf:about=\"&sybylat;X\"/>"
            + "  <owl:Thing rdf:about=\"&sybylat;C.3\"/>"
            + "  <owl:Thing rdf:about=\"&cdkat;X\">"
            + "    <atm:mapsToType rdf:resource=\"&sybylat;X\"/>" + "  </owl:Thing>"
            + "  <owl:Thing rdf:about=\"&cdkat;C.sp3\">"
            + "    <atm:equivalentAsType rdf:resource=\"&sybylat;C.3\"/>"
            + "  </owl:Thing>" + "</rdf:RDF>";

        [TestMethod()]
        public void TestOWLAtomTypeMappingReaderReader()
        {
            OWLAtomTypeMappingReader reader = new OWLAtomTypeMappingReader(new StringReader(""));
            Assert.IsNotNull(reader);
        }

        [TestMethod()]
        public void TestReadAtomTypeMappings()
        {
            OWLAtomTypeMappingReader reader = new OWLAtomTypeMappingReader(new StringReader(OWL_CONTENT));
            Assert.IsNotNull(reader);
            var mappings = reader.ReadAtomTypeMappings();
            Assert.IsNotNull(mappings);
            Assert.AreEqual(2, mappings.Count);
        }

        [TestMethod()]
        public void TestReadAtomTypesCDK2Sybyl()
        {
            OWLAtomTypeMappingReader reader = new OWLAtomTypeMappingReader(new StringReader(OWL_CONTENT));
            Assert.IsNotNull(reader);
            var mappings = reader.ReadAtomTypeMappings();
            Assert.IsNotNull(mappings);
            Assert.AreEqual(2, mappings.Count);

            var cdkTypes = mappings.Keys;
            Assert.IsNotNull(cdkTypes);
            Assert.IsTrue(cdkTypes.Contains("C.sp3"));
            Assert.IsTrue(cdkTypes.Contains("X"));

            Assert.AreEqual("X", mappings["X"]);
            Assert.AreEqual("C.3", mappings["C.sp3"]);
        }
    }
}
