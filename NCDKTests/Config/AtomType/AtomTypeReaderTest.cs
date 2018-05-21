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
using System.IO;
using System.Linq;

namespace NCDK.Config.AtomType
{
    // @cdk.module test-core
    [TestClass()]
    public class AtomTypeReaderTest : CDKTestCase
    {
        [TestMethod()]
        public void TestAtomTypeReader_Reader()
        {
            AtomTypeReader reader = new AtomTypeReader(new StringReader(""));
            Assert.IsNotNull(reader);
        }

        [TestMethod()]
        public void TestReadAtomTypes_IChemObjectBuilder()
        {
            AtomTypeReader reader = new AtomTypeReader(
                new StringReader(
                    "<atomTypeList xmlns=\"http://www.xml-cml.org/schema/cml2/core\"                              "
                            + "  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"                                    "
                            + "  xsi:schemaLocation=\"http://www.xml-cml.org/schema/cml2/core ../../io/cml/data/cmlAll.xsd\""
                            + "  id=\"mol2\" title=\"MOL2 AtomTypes\">                                                      "
                            + "                                                                                             "
                            + "  <atomType id=\"C.3\" title=\"1\">                                                          "
                            + "    <atom elementType=\"C\"/>                                                                "
                            + "    <scalar dataType=\"xsd:string\" dictRef=\"cdk:hybridization\">sp3</scalar>               "
                            + "  </atomType>                                                                                "
                            + "  <atomType id=\"C.2\" title=\"2\">                                                          "
                            + "    <atom elementType=\"C\"/>                                                                "
                            + "    <scalar dataType=\"xsd:string\" dictRef=\"cdk:hybridization\">sp2</scalar>               "
                            + "  </atomType>                                                                                "
                            + "</atomTypeList>"));
            Assert.IsNotNull(reader);
            var types = reader.ReadAtomTypes(new ChemObject().Builder);
            Assert.IsNotNull(types);
            Assert.AreEqual(2, types.Count());
        }

        [TestMethod()]
        public void TestReadAtomTypes2()
        {
            string data = "<atomTypeList xmlns=\"http://www.xml-cml.org/schema/cml2/core\"                              "
                + "  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"                                    "
                + "  xsi:schemaLocation=\"http://www.xml-cml.org/schema/cml2/core ../../io/cml/data/cmlAll.xsd\""
                + "  id=\"mol2\" title=\"MOL2 AtomTypes\">                                                      "
                + "                                                                                             "
                + "  <atomType id=\"C.3\" title=\"1\">                                                          "
                + "    <atom elementType=\"C\"/>                                                                "
                + "    <scalar dataType=\"xsd:string\" dictRef=\"cdk:hybridization\">sp3</scalar>               "
                + "  </atomType>                                                                                "
                + "  <atomType id=\"C.2\" title=\"2\">                                                          "
                + "    <atom elementType=\"C\"/>                                                                "
                + "    <scalar dataType=\"xsd:string\" dictRef=\"cdk:hybridization\">sp2</scalar>               "
                + "  </atomType>                                                                                "
                + "</atomTypeList>";

            AtomTypeReader reader = new AtomTypeReader(new StringReader(data));
            Assert.IsNotNull(reader);
            var types = reader.ReadAtomTypes(new ChemObject().Builder);
            Assert.IsNotNull(types);
            Assert.AreEqual(2, types.Count()); 
        }

        [TestMethod()]
        public void TestReadAtomTypes_CDK()
        {
            string data = "<atomTypeList xmlns=\"http://www.xml-cml.org/schema/cml2/core\"                              \n"
                + "  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"                                    \n"
                + "  xsi:schemaLocation=\"http://www.xml-cml.org/schema/cml2/core ../../io/cml/data/cmlAll.xsd\"\n"
                + "  id=\"mol2\" title=\"MOL2 AtomTypes\">                                                      \n"
                + "                                                                                             \n"
                + "  <atomType id=\"C.sp\">\n" + "    <atom elementType=\"C\" formalCharge=\"0\">\n"
                + "      <scalar dataType=\"xsd:integer\" dictRef=\"cdk:formalNeighbourCount\">2</scalar>\n"
                + "      <scalar dataType=\"xsd:integer\" dictRef=\"cdk:lonePairCount\">0</scalar>\n"
                + "      <scalar dataType=\"xsd:integer\" dictRef=\"cdk:piBondCount\">2</scalar>\n" + "    </atom>\n"
                + "    <scalar dataType=\"xsd:string\" dictRef=\"cdk:hybridization\">sp1</scalar>\n"
                + "  </atomType>                                                                                "
                + "</atomTypeList>";

            AtomTypeReader reader = new AtomTypeReader(new StringReader(data));
            Assert.IsNotNull(reader);
            var types = reader.ReadAtomTypes(new ChemObject().Builder).ToList();
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Count);

            object obj = types[0];
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj is IAtomType);
            IAtomType atomType = (IAtomType)obj;

            Assert.AreEqual(0, atomType.FormalCharge.Value);
            Assert.AreEqual(Hybridization.SP1, atomType.Hybridization);
            Assert.AreEqual(0, atomType.GetProperty<int?>(CDKPropertyName.LonePairCount));
            Assert.AreEqual(2, atomType.GetProperty<int?>(CDKPropertyName.PiBondCount));
        }

        [TestMethod()]
        public void TestReadAtomTypes_FF()
        {
            string data = "<atomTypeList xmlns=\"http://www.xml-cml.org/schema/cml2/core\"                              \n"
                + "  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"                                    \n"
                + "  xsi:schemaLocation=\"http://www.xml-cml.org/schema/cml2/core ../../io/cml/data/cmlAll.xsd\"\n"
                + "  id=\"mol2\" title=\"MOL2 AtomTypes\">                                                      \n"
                + "                                                                                             \n"
                + " <atomType id=\"C\">\n" + "    <!-- for example in CC-->\n"
                + "   <atom elementType=\"C\" formalCharge=\"0\">\n"
                + "     <scalar dataType=\"xsd:double\" dictRef=\"cdk:maxBondOrder\">1.0</scalar>\n"
                + "     <scalar dataType=\"xsd:double\" dictRef=\"cdk:bondOrderSum\">4.0</scalar>\n"
                + "     <scalar dataType=\"xsd:integer\" dictRef=\"cdk:formalNeighbourCount\">4</scalar>\n"
                + "     <scalar dataType=\"xsd:integer\" dictRef=\"cdk:valency\">4</scalar>\n"
                + "     <scalar dataType=\"xsd:string\" dictRef=\"cdk:hybridization\">sp3</scalar>\n"
                + "     <scalar dataType=\"xsd:string\" dictRef=\"cdk:DA\">-</scalar>\n"
                + "     <scalar dataType=\"xsd:string\" dictRef=\"cdk:sphericalMatcher\">[CSP]-[0-4][-]?+;</scalar>\n"
                + "     <scalar dataType=\"xsd:integer\" dictRef=\"cdk:ringSize\">3</scalar>\n"
                + "     <scalar dataType=\"xsd:integer\" dictRef=\"cdk:ringConstant\">3</scalar>\n" + "   </atom>\n"
                + " </atomType>\n" + "</atomTypeList>\n";

            AtomTypeReader reader = new AtomTypeReader(new StringReader(data));
            Assert.IsNotNull(reader);
            var types = reader.ReadAtomTypes(new ChemObject().Builder).ToList();
            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Count);

            object obj = types[0];
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj is IAtomType);
            IAtomType atomType = (IAtomType)obj;

            Assert.AreEqual("[CSP]-[0-4][-]?+;", atomType.GetProperty<string>(CDKPropertyName.SphericalMatcher));
            Assert.IsFalse(atomType.IsHydrogenBondAcceptor);
            Assert.IsFalse(atomType.IsHydrogenBondDonor);

            Assert.AreEqual(3, atomType.GetProperty<int?>(CDKPropertyName.PartOfRingOfSize));
            Assert.AreEqual(3, atomType.GetProperty<int?>(CDKPropertyName.ChemicalGroupConstant));
        }
    }
}
