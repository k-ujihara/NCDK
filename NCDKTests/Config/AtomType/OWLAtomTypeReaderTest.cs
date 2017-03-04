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
using NCDK.Default;
using System.Collections.Generic;
using System.IO;

namespace NCDK.Config.AtomType
{
    /// <summary>
    /// Checks the functionality of the AtomTypeReader.
    ///
    // @cdk.module test-core
    /// </summary>
    [TestClass()]
    public class OWLAtomTypeReaderTest : CDKTestCase
    {

        private const string OWL_CONTENT = "<?xml version=\"1.0\"?>" + "<!DOCTYPE rdf:RDF ["
                                                 + "  <!ENTITY rdf  \"http://www.w3.org/1999/02/22-rdf-syntax-ns#\" >"
                                                 + "  <!ENTITY elem \"http://cdk.sf.net/ontologies/elements#\" >"
                                                 + "  <!ENTITY at   \"http://cdk.sf.net/ontologies/atomtypes#\" >"
                                                 + "  <!ENTITY cdkat \"http://cdk.sf.net/ontologies/atomtypes/cdk#\" >"
                                                 + "]>" + "<rdf:RDF xmlns=\"&cdkat;\" xml:base=\"&cdkat;\""
                                                 + "         xmlns:at=\"&at;\"" + "         xmlns:elem=\"&elem;\""
                                                 + "         xmlns:rdf=\"&rdf;\"" + ">"
                                                 + "  <at:AtomType rdf:ID=\"C.sp3.0\">"
                                                 + "    <at:categorizedAs rdf:resource=\"&cdkat;C.sp3\"/>"
                                                 + "    <at:hasElement rdf:resource=\"&elem;C\"/>"
                                                 + "    <at:hybridization rdf:resource=\"&at;sp3\"/>"
                                                 + "    <at:formalCharge>0</at:formalCharge>"
                                                 + "    <at:lonePairCount>0</at:lonePairCount>"
                                                 + "    <at:formalNeighbourCount>4</at:formalNeighbourCount>"
                                                 + "    <at:piBondCount>0</at:piBondCount>"
                                                 + "    <at:singleElectronCount>0</at:singleElectronCount>"
                                                 + "  </at:AtomType>" + "</rdf:RDF>";

        [TestMethod()]
        public void TestAtomTypeReader_Reader()
        {
            OWLAtomTypeReader reader = new OWLAtomTypeReader(new StringReader(""));
            Assert.IsNotNull(reader);
        }

        [TestMethod()]
        public void TestReadAtomTypes_IChemObjectBuilder()
        {
            OWLAtomTypeReader reader = new OWLAtomTypeReader(new StringReader(OWL_CONTENT));
            Assert.IsNotNull(reader);
            var types = reader.ReadAtomTypes(new ChemObject().Builder);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Count);
        }

        [TestMethod()]
        public void TestReadAtomTypes_CDK()
        {
            OWLAtomTypeReader reader = new OWLAtomTypeReader(new StringReader(OWL_CONTENT));
            Assert.IsNotNull(reader);
            var types = reader.ReadAtomTypes(new ChemObject().Builder);
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Count);

            object obj = types[0];
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj is IAtomType);
            IAtomType atomType = (IAtomType)obj;

            Assert.AreEqual("C", atomType.Symbol);
            Assert.AreEqual("C.sp3.0", atomType.AtomTypeName);
            Assert.AreEqual(0, atomType.FormalCharge.Value);
            Assert.AreEqual(Hybridization.SP3, atomType.Hybridization);
            Assert.AreEqual(4, atomType.FormalNeighbourCount.Value);
            Assert.AreEqual(0, atomType.GetProperty<int>(CDKPropertyName.LONE_PAIR_COUNT));
            Assert.AreEqual(0, atomType.GetProperty<int>(CDKPropertyName.PI_BOND_COUNT));
            Assert.AreEqual(0, atomType.GetProperty<int>(CDKPropertyName.SINGLE_ELECTRON_COUNT));
        }
    }
}
