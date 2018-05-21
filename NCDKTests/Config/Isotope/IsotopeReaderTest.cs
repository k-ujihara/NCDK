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
using NCDK.Default;
using System.IO;
using System.Text;

namespace NCDK.Config.Isotope
{
   // @cdk.module test-extra
    [TestClass()]
    public class IsotopeReaderTest : CDKTestCase
    {
        [TestMethod()]
        public void TestIsotopeReader_InputStream_IChemObjectBuilder()
        {
            IsotopeReader reader = new IsotopeReader(new MemoryStream(new byte[0]), new ChemObject().Builder);
            Assert.IsNotNull(reader);
        }

        [TestMethod()]
        public void TestReadIsotopes()
        {
            IsotopeReader reader = new IsotopeReader(new MemoryStream(Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><list></list>")), new ChemObject().Builder);
            Assert.IsNotNull(reader);
            var isotopes = reader.ReadIsotopes();
            Assert.IsNotNull(isotopes);
            Assert.AreEqual(0, isotopes.Count);
        }

        [TestMethod()]
        public void TestReadIsotopes2()
        {
            var isotopeData = "<?xml version=\"1.0\"?>" + "<list xmlns=\"http://www.xml-cml.org/schema/cml2/core\""
                + "    xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\""
                + "    xsi:schemaLocation=\"http://www.xml-cml.org/schema/cml2/core ../../io/cml/data/cmlCore.xsd\">"
                + "" + "    <isotopeList id=\"H\">"
                + "        <isotope id=\"H1\" isotopeNumber=\"1\" elementType=\"H\">"
                + "            <abundance dictRef=\"cdk:relativeAbundance\">100.0</abundance>"
                + "            <scalar dictRef=\"cdk:exactMass\">1.00782504</scalar>"
                + "            <scalar dictRef=\"cdk:atomicNumber\">1</scalar>" + "        </isotope>"
                + "        <isotope id=\"H2\" isotopeNumber=\"2\" elementType=\"H\">"
                + "            <abundance dictRef=\"cdk:relativeAbundance\">0.015</abundance>"
                + "            <scalar dictRef=\"cdk:exactMass\">2.01410179</scalar>"
                + "            <scalar dictRef=\"cdk:atomicNumber\">1</scalar>" + "        </isotope>"
                + "        <isotope id=\"D2\" isotopeNumber=\"2\" elementType=\"D\">"
                + "            <abundance dictRef=\"cdk:relativeAbundance\">0.015</abundance>"
                + "            <scalar dictRef=\"cdk:exactMass\">2.01410179</scalar>"
                + "            <scalar dictRef=\"cdk:atomicNumber\">1</scalar>" + "        </isotope>"
                + "    </isotopeList>" + "</list>";

            IsotopeReader reader = new IsotopeReader(new MemoryStream(Encoding.UTF8.GetBytes(isotopeData)), new ChemObject().Builder);
            Assert.IsNotNull(reader);
            var isotopes = reader.ReadIsotopes();
            Assert.IsNotNull(isotopes);
            Assert.AreEqual(3, isotopes.Count);
        }
    }
}
