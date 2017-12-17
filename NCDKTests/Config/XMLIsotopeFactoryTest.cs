/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
 *
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NCDK.Config
{
    /// <summary>
    /// Checks the functionality of the <see cref="IsotopeFactory"/>
    /// </summary>
    // @cdk.module test-extra
    [TestClass()]
    public class XMLIsotopeFactoryTest : CDKTestCase
    {
        bool standAlone = false;
        readonly static AtomTypeFactory atf = AtomTypeFactory.GetInstance(new ChemObject().Builder);
        private const string JAXP_SCHEMA_LANGUAGE = "http://java.sun.com/xml/jaxp/properties/schemaLanguage";
        private const string W3C_XML_SCHEMA = "http://www.w3.org/2001/XMLSchema";

        private static IList<FileInfo> tmpFiles = new List<FileInfo>();
        private static FileInfo tmpCMLSchema;

        [ClassInitialize()]
        public static void Initialize(TestContext context)
        {
            using (var ins = ResourceLoader.GetAsStream(typeof(AtomTypeFactory).Assembly, "NCDK.IO.CML.Data.cml25b1.xsd"))
            {
                tmpCMLSchema = CopyFileToTmp("cml2.5.b1", ".xsd", ins, null, null);
            }
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            foreach (var fi in tmpFiles)
            {
                try
                {
                    fi.Delete();
                }
                catch (Exception)
                { }
            }
            tmpFiles.Clear();
        }

        [TestMethod()]
        public void TestGetInstance_IChemObjectBuilder()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Assert.IsNotNull(isofac);
        }

        [TestMethod()]
        public void TestGetSize()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Assert.IsTrue(isofac.Count > 0);
        }

        [TestMethod()]
        public void TestConfigure_IAtom()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Atom atom = new Atom("H");
            isofac.Configure(atom);
            Assert.AreEqual(1, atom.AtomicNumber.Value);
        }

        [TestMethod()]
        public void TestConfigure_IAtom_IIsotope()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Atom atom = new Atom("H");
            IIsotope isotope = new Default.Isotope("H", 2);
            isofac.Configure(atom, isotope);
            Assert.AreEqual(2, atom.MassNumber.Value);
        }

        [TestMethod()]
        public void TestGetMajorIsotope_String()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope isotope = isofac.GetMajorIsotope("Te");
            if (standAlone) Console.Out.WriteLine($"Isotope: {isotope}");
            Assert.AreEqual(129.9062244, isotope.ExactMass.Value, 0.0001);
        }

        [TestMethod()]
        public void TestGetMajorIsotope_Nonelement()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope isotope = isofac.GetMajorIsotope("E");
            Assert.IsNull(isotope);
        }

        [TestMethod()]
        public void TestGetMajorIsotope_int()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope isotope = isofac.GetMajorIsotope(17);
            Assert.AreEqual("Cl", isotope.Symbol);
        }

        [TestMethod()]
        public void TestGetElement_String()
        {
            XMLIsotopeFactory elfac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IElement element = elfac.GetElement("Br");
            Assert.AreEqual(35, element.AtomicNumber.Value);
        }

        [TestMethod()]
        public void TestGetElement_Nonelement()
        {
            XMLIsotopeFactory elfac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IElement element = elfac.GetElement("E");
            Assert.IsNull(element);
        }

        [TestMethod()]
        public void TestGetElement_int()
        {
            XMLIsotopeFactory elfac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IElement element = elfac.GetElement(6);
            Assert.AreEqual("C", element.Symbol);
        }

        [TestMethod()]
        public void TestGetElementSymbol_int()
        {
            XMLIsotopeFactory elfac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            String symbol = elfac.GetElementSymbol(8);
            Assert.AreEqual("O", symbol);
        }

        [TestMethod()]
        public void TestGetIsotopes_String()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope[] list = isofac.GetIsotopes("He").ToArray();
            Assert.AreEqual(8, list.Length);
        }

        [TestMethod()]
        public void TestGetIsotopes_Nonelement()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope[] list = isofac.GetIsotopes("E").ToArray();
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Length);
        }

        [TestMethod()]
        public void TestGetIsotopes()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope[] list = isofac.GetIsotopes().ToArray();
            Assert.IsTrue(list.Length > 200);
        }

        [TestMethod()]
        public void TestGetIsotopes_double_double()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope[] list = isofac.GetIsotopes(87.90, 0.01).ToArray();
            //        should return:
            //        Isotope match: 88Sr has mass 87.9056121
            //        Isotope match: 88Y has mass 87.9095011
            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(88, list[0].MassNumber.Value);
            Assert.AreEqual(88, list[1].MassNumber.Value);
        }

        [TestMethod()]
        public void TestIsElement_String()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Assert.IsTrue(isofac.IsElement("C"));
        }

        [TestMethod()]
        public void TestConfigureAtoms_IAtomContainer()
        {
            AtomContainer container = new AtomContainer();
            container.Atoms.Add(new Atom("C"));
            container.Atoms.Add(new Atom("H"));
            container.Atoms.Add(new Atom("N"));
            container.Atoms.Add(new Atom("O"));
            container.Atoms.Add(new Atom("F"));
            container.Atoms.Add(new Atom("Cl"));
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            isofac.ConfigureAtoms(container);
            foreach (var atom in container.Atoms)
            {
                Assert.IsTrue(0 < atom.AtomicNumber.Value);
            }
        }

        [TestMethod()]
        public void TestXMLValidityHybrid()
        {
            AssertValidCML("NCDK.Config.Data.isotopes.xml", "Isotopes");
        }

        private void AssertValidCML(string atomTypeList, string shortcut)
        {
            var ins = ResourceLoader.GetAsStream(atomTypeList);
            FileInfo tmpInput = CopyFileToTmp(shortcut, ".cmlinput", ins, "../../io/cml/data/cml25b1.xsd", new Uri(tmpCMLSchema.FullName).AbsolutePath);
            Assert.IsNotNull(ins, "Could not find the atom type list CML source");

            var doc = new XmlDocument();
            doc.Load(tmpInput.FullName);
            doc.Validate((sender, e) => Assert.Fail($"{shortcut} is not valid on line {e.Exception.LinePosition}: {e.Message}"));
        }

        [TestMethod()]
        public void TestCanReadCMLSchema()
        {
            var cmlSchema = new FileStream(tmpCMLSchema.FullName, FileMode.Open);
            Assert.IsNotNull(cmlSchema, "Could not find the CML schema");

            // make sure the schema is read
            var schemaDoc = XDocument.Load(cmlSchema);
            Assert.IsNotNull(schemaDoc.Root);
            Assert.AreEqual(((XNamespace)"http://www.w3.org/2001/XMLSchema") + "schema", schemaDoc.Root.Name);
        }

        private static FileInfo CopyFileToTmp(string prefix, string suffix, Stream ins, string toReplace, string replaceWith)
        {
            var tmpFile = new FileInfo(Path.Combine(Path.GetTempPath(), prefix ?? "" + Guid.NewGuid().ToString() + suffix ?? ""));
            string all;
            using (var rs = new StreamReader(ins))
            {
                all = rs.ReadToEnd();
                if (toReplace != null && replaceWith != null)
                    all = all.Replace(toReplace, replaceWith);
            }
            using (var ws = new StreamWriter(tmpFile.FullName))
            {
                ws.Write(all);
            }
            return tmpFile;
        }

        [TestMethod()]
        public void TestGetNaturalMass_IElement()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Assert.AreEqual(1.0079760, isofac.GetNaturalMass(new Element("H")), 0.1);
        }

        [TestMethod()]
        public void TestGetIsotope()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            Assert.AreEqual(13.00335484, isofac.GetIsotope("C", 13).ExactMass.Value, 0.0000001);
        }

        [TestMethod()]
        public void TestGetIsotopeFromExactMass()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope carbon13 = isofac.GetIsotope("C", 13);
            IIsotope match = isofac.GetIsotope(carbon13.Symbol, carbon13.ExactMass.Value, 0.0001);
            Assert.IsNotNull(match);
            Assert.AreEqual(13, match.MassNumber.Value);
        }

        [TestMethod()]
        public void TestYeahSure()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope match = isofac.GetIsotope("H", (int)13.00001, 0.0001);
            Assert.IsNull(match);
        }

        [TestMethod()]
        public void TestGetIsotopeFromExactMass_LargeTolerance()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IIsotope carbon13 = isofac.GetIsotope("C", 13);
            IIsotope match = isofac.GetIsotope(carbon13.Symbol, carbon13.ExactMass.Value, 2.0);
            Assert.IsNotNull(match);
            Assert.AreEqual(13, match.MassNumber.Value);
        }

        // @cdk.bug 3534288
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNonexistingElement()
        {
            XMLIsotopeFactory isofac = XMLIsotopeFactory.GetInstance(new ChemObject().Builder);
            IAtom xxAtom = new Atom("Xx");
            isofac.Configure(xxAtom);
        }
    }
}
