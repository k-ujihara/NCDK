/* Copyright (C) 2011  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@slists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for reading CML files.
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public class CMLReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.CML.3.cml";
        protected override Type ChemObjectIOToTestType => typeof(CMLReader);

        [TestMethod()]
        public void TestAccepts()
        {
            Assert.IsTrue(ChemObjectIOToTest.Accepts(typeof(ChemFile)));
        }

        [TestMethod()]
        public override void TestSetReader_Reader()
        {
            var ctor = ChemObjectIOToTestType.GetConstructor(new Type[] { typeof(TextReader) });
            Assert.IsNull(ctor);
        }

        /// <summary>
        /// Ensure stereoBond content is read if the usual "dictRef" attribute is not
        /// supplied
        /// </summary>
        // @cdk.bug 1248
        [TestMethod()]
        public void TestBug1248()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.CML.(1R)-1-aminoethan-1-ol.cml");
            CMLReader reader = new CMLReader(ins);
            try
            {
                IChemFile cfile = reader.Read(Default.ChemObjectBuilder.Instance.CreateChemFile());

                Assert.IsNotNull(cfile, "ChemFile was Null");

                var containers = ChemFileManipulator.GetAllAtomContainers(cfile);

                Assert.AreEqual(1, containers.Count(), "Expected a single atom container");

                IAtomContainer container = containers.First();

                Assert.IsNotNull(container, "Null atom container read");

                IBond bond = container.Bonds[2];

                Assert.IsNotNull(bond, "Null bond");

                Assert.AreEqual(BondStereo.Up, bond.Stereo, "Expected Wedge (Up) Bond");
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Ensure correct atomic numbers are read and does not default to 1
        /// </summary>
        // @cdk.bug 1245
        [TestMethod()]
        public void TestBug1245()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.CML.(1R)-1-aminoethan-1-ol.cml");
            CMLReader reader = new CMLReader(ins);
            try
            {
                IChemFile cfile = reader.Read(Default.ChemObjectBuilder.Instance.CreateChemFile());

                Assert.IsNotNull(cfile, "ChemFile was Null");

                var containers = ChemFileManipulator.GetAllAtomContainers(cfile);

                Assert.AreEqual(1, containers.Count(), "Expected a single atom container");

                IAtomContainer container = containers.First();

                Assert.IsNotNull(container, "Null atom container read");

                foreach (var atom in container.Atoms)
                {
                    Assert.AreEqual(
                        PeriodicTable.GetAtomicNumber(atom.Symbol),
                        atom.AtomicNumber,
                        "Incorrect atomic number");
                }
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Ensures that when multiple stereo is set the dictRef is favoured
        /// and the charContent is not used. Here is an example of what we expect
        /// to read.
        /// <pre>
        /// <bond atomRefs2="a1 a4" order="1">
        ///     <bondStereo dictRef="cml:W"/> <!-- should be W -->
        /// </bond>
        ///
        /// <bond atomRefs2="a1 a4" order="1">
        ///     <bondStereo>W</bondStereo> <!-- should be W -->
        /// </bond>
        ///
        /// <bond atomRefs2="a1 a4" order="1">
        ///    <bondStereo dictRef="cml:W">W</bondStereo> <!-- should be W -->
        /// </bond>
        ///
        /// <bond atomRefs2="a1 a4" order="1">
        ///    <bondStereo dictRef="cml:W">H</bondStereo> <!-- should be W -->
        /// </bond>
        /// </pre>
        /// </summary>
        /// <seealso cref="TestBug1248"/>
        // @cdk.bug 1274
        [TestMethod()]
        public void TestBug1274()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.CML.(1R)-1-aminoethan-1-ol-multipleBondStereo.cml");
            CMLReader reader = new CMLReader(ins);
            try
            {
                IChemFile cfile = reader.Read(Default.ChemObjectBuilder.Instance.CreateChemFile());

                Assert.IsNotNull(cfile, "ChemFile was Null");

                var containers = ChemFileManipulator.GetAllAtomContainers(cfile);

                Assert.AreEqual(1, containers.Count(), "Expected a single atom container");

                IAtomContainer container = containers.First();

                Assert.IsNotNull(container, "Null atom container read");

                // we check here that the charContent is not used and also that more then
                // one stereo isn't set
                Assert.AreEqual(BondStereo.None, container.Bonds[0].Stereo, "expected non-stereo bond");
                Assert.AreEqual(BondStereo.Down, container.Bonds[1].Stereo, "expected Hatch (Down) Bond");
                Assert.AreEqual(BondStereo.None, container.Bonds[2].Stereo, "expected non-stereo bond");
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Ensures that <pre><bondStereo dictRef="cml:"/></pre> doesn't cause an exception
        /// </summary>
        // @cdk.bug 1275
        [TestMethod()]
        public void TestBug1275()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.CML.(1R)-1-aminoethan-1-ol-malformedDictRef.cml");
            CMLReader reader = new CMLReader(ins);
            try
            {
                IChemFile cfile = reader.Read(Default.ChemObjectBuilder.Instance.CreateChemFile());

                Assert.IsNotNull(cfile, "ChemFile was Null");

                var containers = ChemFileManipulator.GetAllAtomContainers(cfile);

                Assert.AreEqual(1, containers.Count(), "Expected a single atom container");

                IAtomContainer container = containers.First();

                Assert.IsNotNull(container, "Null atom container read");

                // we check here that the malformed dictRef doesn't throw an exception
                Assert.AreEqual(BondStereo.None, container.Bonds[0].Stereo, "expected non-stereo bond");
                Assert.AreEqual(BondStereo.Up, container.Bonds[1].Stereo, "expected Wedge (Up) Bond");
                Assert.AreEqual(BondStereo.None, container.Bonds[2].Stereo, "expected non-stereo bond");
            }
            finally
            {
                reader.Close();
            }
        }

        [TestMethod()]
        public void TestWedgeBondParsing()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.CML.AZD5423.xml");
            CMLReader reader = new CMLReader(ins);
            try
            {
                IChemFile cfile = reader.Read(Default.ChemObjectBuilder.Instance.CreateChemFile());
                Assert.IsNotNull(cfile, "ChemFile was Null");
                var containers = ChemFileManipulator.GetAllAtomContainers(cfile);
                Assert.AreEqual(1, containers.Count(), "Expected a single atom container");
                IAtomContainer container = containers.First();
                Assert.IsNotNull(container, "Null atom container read");

                // we check here that the malformed dictRef doesn't throw an exception
                for (int i = 0; i < 19; i++)
                {
                    Assert.AreEqual(
                        BondStereo.None, container.Bonds[i].Stereo,
                        "found an unexpected wedge bond for " + i + ": " + container.Bonds[i].Stereo);
                }
                Assert.AreEqual(BondStereo.Down, container.Bonds[19].Stereo, "expected a wedge bond");
                for (int i = 20; i < 30; i++)
                {
                    Assert.AreEqual(
                        BondStereo.None, container.Bonds[i].Stereo,
                         "found an unexpected wedge bond for " + i + ": " + container.Bonds[i].Stereo);
                }
                Assert.AreEqual(BondStereo.Up, container.Bonds[30].Stereo, "expected a wedge bond");
                for (int i = 31; i <= 37; i++)
                {
                    Assert.AreEqual(
                    BondStereo.None, container.Bonds[i].Stereo,
                    "found an unexpected wedge bond for " + i + ": " + container.Bonds[i].Stereo);
                }
            }
            finally
            {
                reader.Close();
            }
        }

        [TestMethod()]
        public void TestSFBug1085912_1()
        {
            string cmlContent = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>"
                    + "<molecule convention=\"PDB\" dictRef=\"pdb:model\" xmlns=\"http://www.xml-cml.org/schema\">"
                    + "  <molecule dictRef=\"pdb:sequence\" id=\"ALAA116\">"
                    + "    <atomArray>"
                    + "      <atom id=\"a9794931\" elementType=\"N\" x3=\"-10.311\" y3=\"2.77\" z3=\"-9.837\" formalCharge=\"0\">"
                    + "        <scalar dictRef=\"cdk:partialCharge\" dataType=\"xsd:double\">0.0</scalar>"
                    + "      </atom>"
                    + "      <atom id=\"a5369354\" elementType=\"C\" x3=\"-9.75\" y3=\"4.026\" z3=\"-9.35\" formalCharge=\"0\">"
                    + "        <scalar dictRef=\"cdk:partialCharge\" dataType=\"xsd:double\">0.0</scalar>"
                    + "      </atom>"
                    + "      <atom id=\"a14877152\" elementType=\"C\" x3=\"-10.818\" y3=\"5.095\" z3=\"-9.151\" formalCharge=\"0\">"
                    + "        <scalar dictRef=\"cdk:partialCharge\" dataType=\"xsd:double\">0.0</scalar>"
                    + "      </atom>"
                    + "      <atom id=\"a26221736\" elementType=\"O\" x3=\"-11.558\" y3=\"5.433\" z3=\"-10.074\" formalCharge=\"0\">"
                    + "        <scalar dictRef=\"cdk:partialCharge\" dataType=\"xsd:double\">0.0</scalar>"
                    + "      </atom>"
                    + "      <atom id=\"a4811470\" elementType=\"C\" x3=\"-8.678\" y3=\"4.536\" z3=\"-10.304\" formalCharge=\"0\">"
                    + "        <scalar dictRef=\"cdk:partialCharge\" dataType=\"xsd:double\">0.0</scalar>"
                    + "      </atom>"
                    + "      <atom id=\"a211489\" elementType=\"H\" x3=\"-10.574\" y3=\"2.695\" z3=\"-10.778\" formalCharge=\"0\">"
                    + "        <scalar dictRef=\"cdk:partialCharge\" dataType=\"xsd:double\">0.0</scalar>"
                    + "      </atom>"
                    + "      <atom id=\"a31287617\" elementType=\"H\" x3=\"-9.279\" y3=\"3.829\" z3=\"-8.398\" formalCharge=\"0\">"
                    + "        <scalar dictRef=\"cdk:partialCharge\" dataType=\"xsd:double\">0.0</scalar>"
                    + "      </atom>"
                    + "      <atom id=\"a19487109\" elementType=\"H\" x3=\"-8.523\" y3=\"3.813\" z3=\"-11.09\" formalCharge=\"0\">"
                    + "        <scalar dictRef=\"cdk:partialCharge\" dataType=\"xsd:double\">0.0</scalar>"
                    + "      </atom>"
                    + "      <atom id=\"a28589522\" elementType=\"H\" x3=\"-8.994\" y3=\"5.477\" z3=\"-10.737\" formalCharge=\"0\">"
                    + "        <scalar dictRef=\"cdk:partialCharge\" dataType=\"xsd:double\">0.0</scalar>"
                    + "      </atom>"
                    + "      <atom id=\"a4638116\" elementType=\"H\" x3=\"-7.754\" y3=\"4.682\" z3=\"-9.763\" formalCharge=\"0\">"
                    + "        <scalar dictRef=\"cdk:partialCharge\" dataType=\"xsd:double\">0.0</scalar>"
                    + "      </atom>" + "    </atomArray>" + "  </molecule>" + "</molecule>";
            CMLReader reader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(cmlContent)));
            try
            {
                IChemFile cfile = reader.Read(Default.ChemObjectBuilder.Instance.CreateChemFile());
                Assert.IsNotNull(cfile, "ChemFile was Null");
                var containers = ChemFileManipulator.GetAllAtomContainers(cfile);
                Assert.AreEqual(1, containers.Count(), "Expected a single atom container");
                IAtomContainer container = containers.First();
                Assert.IsNotNull(container, "Null atom container read");

                // OK, now test that the residue identifier is properly read
                Assert.AreEqual("ALAA116", container.Id);
                Console.Out.WriteLine("" + container);
            }
            finally
            {
                reader.Close();
            }
        }

        [TestMethod()]
        public void TestMixedNamespaces()
        {
            var ins = ResourceLoader.GetAsStream(this.GetType(), "US06358966-20020319-C00001-enr.cml");
            CMLReader reader = new CMLReader(ins);
            try
            {
                IChemFile cfile = reader.Read(Default.ChemObjectBuilder.Instance.CreateChemFile());
                Assert.AreEqual(34, ChemFileManipulator.GetAtomCount(cfile));
                Assert.AreEqual(39, ChemFileManipulator.GetBondCount(cfile));
            }
            finally
            {
                reader.Close();
            }
        }
    }
}
