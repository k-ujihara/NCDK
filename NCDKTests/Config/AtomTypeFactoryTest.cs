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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NCDK.Config
{
    /// <summary>
    /// Checks the functionality of the <see cref="AtomTypeFactory"/>.
    /// </summary>
    //  @cdk.module test-core
    [TestClass()]
    public class AtomTypeFactoryTest 
        : CDKTestCase
    {
        private readonly static IChemObjectBuilder builder = CDK.Builder;
        static AtomTypeFactory atf = CDK.AtomTypeFactory;
        private const string JAXP_SCHEMA_LANGUAGE = "http://java.sun.com/xml/jaxp/properties/schemaLanguage";
        private const string W3C_XML_SCHEMA = "http://www.w3.org/2001/XMLSchema";

        private static IList<FileInfo> tmpFiles = new List<FileInfo>();
        private static string tmpCMLSchema;

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
        public virtual void TestAtomTypeFactory()
        {
            Assert.IsNotNull(atf);
            Assert.AreNotSame(atf.Count, 0);
        }

        [TestMethod()]
        public virtual void TestGetInstance_InputStream_String_IChemObjectBuilder()
        {
            var configFile = "NCDK.Config.Data.structgen_atomtypes.xml";
            var ins = ResourceLoader.GetAsStream(typeof(AtomTypeFactory), configFile);
            AtomTypeFactory atf = AtomTypeFactory.GetInstance(ins, "xml");
            Assert.IsNotNull(atf);
            Assert.AreNotSame(0, atf.Count);
        }

        [TestMethod()]
        public void TestGetInstance_String_IChemObjectBuilder()
        {
            string configFile = "NCDK.Config.Data.structgen_atomtypes.xml";
            AtomTypeFactory atf = AtomTypeFactory.GetInstance(configFile);
            Assert.IsNotNull(atf);
            Assert.AreNotSame(0, atf.Count);
        }

        [TestMethod()]
        public void TestGetInstance_IChemObjectBuilder()
        {
            AtomTypeFactory atf = AtomTypeFactory.GetInstance();
            Assert.IsNotNull(atf);
        }

        [TestMethod()]
        public virtual void TestGetSize()
        {
            AtomTypeFactory atf = AtomTypeFactory.GetInstance();
            Assert.AreNotSame(0, atf.Count);
        }

        [TestMethod()]
        public virtual void TestGetAllAtomTypes()
        {
            AtomTypeFactory atf = AtomTypeFactory.GetInstance();
            IAtomType[] types = atf.GetAllAtomTypes()?.ToArray();
            Assert.IsNotNull(types);
            Assert.AreNotSame(0, types.Length);
        }

        [TestMethod()]
        public virtual void TestGetAtomType_String()
        {
            var atomType = atf.GetAtomType("C4");
            Assert.IsNotNull(atomType);
            Assert.AreEqual("C", atomType.Symbol);
            Assert.AreEqual("C4", atomType.AtomTypeName);
            Assert.AreEqual(4.0, atomType.BondOrderSum??0, 0.001);
            Assert.AreEqual(BondOrder.Triple, atomType.MaxBondOrder);
        }

        [TestMethod()]
        public virtual void TestGetAtomTypes_String() 
        {
            IAtomType[] atomTypes = atf.GetAtomTypes("C")?.ToArray();

            Assert.IsNotNull(atomTypes);
            Assert.IsTrue(0 < atomTypes.Length);
            Assert.AreEqual("C", atomTypes[0].Symbol);
        }

        [TestMethod()]
        public virtual void TestGetAtomTypeFromPDB()
        {
            AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Config.Data.pdb_atomtypes.xml");
            IAtomType atomType = factory.GetAtomType("ALA.CA");
            Assert.IsNotNull(atomType);
            Assert.AreEqual("C", atomType.Symbol);
            Assert.AreEqual("ALA.CA", atomType.AtomTypeName);
        }

        [TestMethod()]
        public virtual void TestGetAtomTypeFromOWL()
        {
            AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Dict.Data.cdk-atom-types.owl");
            IAtomType atomType;

            atomType = factory.GetAtomType("C.sp3");
            Assert.IsNotNull(atomType);
            Assert.AreEqual("C", atomType.Symbol);
            Assert.AreEqual("C.sp3", atomType.AtomTypeName);
            Assert.AreEqual(Hybridization.SP3, atomType.Hybridization);
            Assert.AreEqual(0, atomType.FormalCharge);
            Assert.AreEqual(4, atomType.FormalNeighbourCount);
            Assert.IsNotNull(atomType.GetProperty<object>(CDKPropertyName.LonePairCount));
            Assert.AreEqual(0, atomType.GetProperty<int>(CDKPropertyName.LonePairCount));
            Assert.AreEqual(0, atomType.GetProperty<int>(CDKPropertyName.PiBondCount));
            Assert.AreEqual(BondOrder.Single, atomType.MaxBondOrder);
            Assert.AreEqual(4.0, atomType.BondOrderSum.Value, 0.1);

            atomType = factory.GetAtomType("N.sp2.radical");
            Assert.IsNotNull(atomType);
            Assert.AreEqual("N", atomType.Symbol);
            Assert.AreEqual("N.sp2.radical", atomType.AtomTypeName);
            Assert.AreEqual(Hybridization.SP2, atomType.Hybridization);
            Assert.AreEqual(0, atomType.FormalCharge);
            Assert.AreEqual(1, atomType.FormalNeighbourCount);
            Assert.IsNotNull(atomType.GetProperty<object>(CDKPropertyName.LonePairCount));
            Assert.AreEqual(1, atomType.GetProperty<int>(CDKPropertyName.LonePairCount));
            Assert.AreEqual(1, atomType.GetProperty<int>(CDKPropertyName.PiBondCount));
            Assert.AreEqual(BondOrder.Double, atomType.MaxBondOrder);
            Assert.AreEqual(2.0, atomType.BondOrderSum.Value, 0.1);

            atomType = factory.GetAtomType("N.planar3");
            Assert.IsNotNull(atomType);
            Assert.AreEqual("N", atomType.Symbol);
            Assert.AreEqual("N.planar3", atomType.AtomTypeName);
            Assert.AreEqual(Hybridization.Planar3, atomType.Hybridization);
            Assert.AreEqual(0, atomType.FormalCharge);
            Assert.AreEqual(3, atomType.FormalNeighbourCount);
            Assert.IsNotNull(atomType.GetProperty<object>(CDKPropertyName.LonePairCount));
            Assert.AreEqual(1, atomType.GetProperty<int>(CDKPropertyName.LonePairCount));
            Assert.AreEqual(0, atomType.GetProperty<int>(CDKPropertyName.PiBondCount));
            Assert.AreEqual(BondOrder.Single, atomType.MaxBondOrder);
            Assert.AreEqual(3.0, atomType.BondOrderSum.Value, 0.1);
        }

        [TestMethod()]
        public virtual void TestGetAtomTypeFromOWL_Sybyl()
        {
            AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Dict.Data.sybyl-atom-types.owl");

            IAtomType atomType;
            atomType = factory.GetAtomType("C.3");
            Assert.IsNotNull(atomType);
            Assert.AreEqual("C", atomType.Symbol);
            Assert.AreEqual("C.3", atomType.AtomTypeName);
            Assert.AreEqual(4, atomType.FormalNeighbourCount);
            Assert.AreEqual(Hybridization.SP3, atomType.Hybridization);
            Assert.AreEqual(0, atomType.FormalCharge);
            Assert.AreEqual(0, atomType.GetProperty<int?>(CDKPropertyName.LonePairCount));
            Assert.AreEqual(0, atomType.GetProperty<int?>(CDKPropertyName.PiBondCount));
        }

        [TestMethod()]
        public virtual void TestGetAtomTypeFromJmol()
        {
            AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Config.Data.jmol_atomtypes.txt");
            IAtomType atomType = factory.GetAtomType("H");
            Assert.IsNotNull(atomType);
            Assert.AreEqual("H", atomType.Symbol);
            Assert.AreEqual("H", atomType.AtomTypeName);
        }

        [TestMethod()]
        public virtual void TestConfigure_IAtom()
        {
            IAtom atom = builder.NewAtom();
            atom.AtomTypeName = "C.ar";
            AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Config.Data.mol2_atomtypes.xml");
            IAtomType atomType = factory.Configure(atom);
            Assert.IsNotNull(atomType);
            Assert.AreEqual("C", atom.Symbol);
        }

        /// <summary>
        /// Test reading from a XML config file with content like:
        /// <![CDATA[
        ///   <atomType id="C">
        ///    <!-- for example in CC-->
        ///    <atom elementType="C" formalCharge="0">
        ///      <scalar dataType="xsd:double" dictRef="cdk:maxBondOrder">1.0</scalar>
        ///      <scalar dataType="xsd:double" dictRef="cdk:bondOrderSum">4.0</scalar>
        ///      <scalar dataType="xsd:integer" dictRef="cdk:formalNeighbourCount">4</scalar>
        ///      <scalar dataType="xsd:integer" dictRef="cdk:valency">4</scalar>
        ///    </atom>
        ///    <scalar dataType="xsd:string" dictRef="cdk:hybridization">sp3</scalar>
        ///    <scalar dataType="xsd:string" dictRef="cdk:DA">-</scalar>
        ///    <scalar dataType="xsd:string" dictRef="cdk:sphericalMatcher">[CSP]-[0-4][-]?+;[A-Za-z\+\-&amp;&amp;[^=%]]{0,6}[(].*+</scalar>
        ///  </atomType>
        /// ]]>
        /// </summary>
        /// <exception cref="Exception">if the atom type info cannot be loaded</exception>
        [TestMethod()]
        public virtual void TestGetAtomTypeFromMM2()
        {
            AtomTypeFactory factory;
            factory = AtomTypeFactory.GetInstance("NCDK.Config.Data.mm2_atomtypes.xml");

            IAtomType atomType = factory.GetAtomType("C");
            Assert.IsNotNull(atomType);
            Assert.AreEqual("C", atomType.Symbol);
            Assert.AreEqual("C", atomType.AtomTypeName);
            Assert.AreEqual("[CSP]-[0-4][-]?+;[A-Za-z\\+\\-&&[^=%]]{0,6}[(].*+",
                    atomType.GetProperty<string>(CDKPropertyName.SphericalMatcher));
            Assert.AreEqual(Hybridization.SP3, atomType.Hybridization);

            atomType = factory.GetAtomType("Sthi");
            Assert.IsNotNull(atomType);
            Assert.AreEqual("S", atomType.Symbol);
            Assert.AreEqual("Sthi", atomType.AtomTypeName);
            Assert.AreEqual("S-[2];[H]{0,3}+=C.*+", atomType.GetProperty<string>(CDKPropertyName.SphericalMatcher));
            Assert.AreEqual(Hybridization.SP2, atomType.Hybridization);
            Assert.IsTrue(atomType.IsHydrogenBondAcceptor);
            Assert.AreEqual(5, atomType.GetProperty<int?>(CDKPropertyName.PartOfRingOfSize));
        }

        [TestMethod()]
        public virtual void TestCanReadCMLSchema()
        {
            using (var cmlSchema = new FileStream(tmpCMLSchema, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var schemaDoc = XDocument.Load(cmlSchema);
                Assert.IsNotNull(schemaDoc.Root);
                XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
                Assert.AreEqual(xsd + "schema", schemaDoc.Root.Name);
            }
        }

        [TestCategory("XMLValidity"), Ignore()]
        [TestMethod()]
        public virtual void TestXMLValidityMM2()
        {
            AssertValidCML("NCDK.Config.Data.mm2_atomtypes.xml", "MM2");
        }

        [TestCategory("XMLValidity"), Ignore()]
        [TestMethod()]
        public virtual void TestXMLValidityMMFF94()
        {
            AssertValidCML("NCDK.Config.Data.mmff94_atomtypes.xml", "MMFF94");
        }

        [TestCategory("XMLValidity"), Ignore()]
        [TestMethod()]
        public virtual void TestXMLValidityMol2()
        {
            AssertValidCML("NCDK.Config.Data.mol2_atomtypes.xml", "Mol2");
        }

        [TestCategory("XMLValidity"), Ignore()]
        [TestMethod()]
        public virtual void TestXMLValidityPDB()
        {
            AssertValidCML("NCDK.Config.Data.pdb_atomtypes.xml", "PDB");
        }

        [TestCategory("XMLValidity"), Ignore()]
        [TestMethod()]
        public virtual void TestXMLValidityStructGen()
        {
            AssertValidCML("NCDK.Config.Data.structgen_atomtypes.xml", "StructGen");
        }

        private static void AssertValidCML(string atomTypeList, string shortcut)
        {
            using (var ins = ResourceLoader.GetAsStream(typeof(AtomTypeFactory).Assembly, atomTypeList))
            {
                var tmpInput = CopyFileToTmp(shortcut, ".cmlinput", ins, "../../io/cml/data/cml25b1.xsd", new Uri(tmpCMLSchema).AbsolutePath);

                var doc = new XmlDocument();
                doc.Load(tmpInput);
                doc.Validate((sender, e) => Assert.Fail($"{shortcut} is not valid on line {e.Exception.LinePosition}: {e.Message}"));
            }
        }
    }
}