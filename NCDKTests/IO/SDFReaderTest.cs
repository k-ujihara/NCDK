/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading MDL mol files using one test file.
    /// </summary>
    /// <seealso cref="MDLReader"/>
    // @cdk.module test-io
	[TestClass()]
    public class SDFReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.MDL.test.sdf";
        static readonly MDLV2000Reader simpleReader = new MDLV2000Reader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        [TestMethod()]
        public void TestAccepts()
        {
            MDLV2000Reader reader = new MDLV2000Reader();
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
            Assert.IsTrue(reader.Accepts(typeof(ChemModel)));
            Assert.IsTrue(reader.Accepts(typeof(Silent.AtomContainer)));
        }

        [TestMethod()]
        public void TestSDFFile()
        {
            string filename = "NCDK.Data.MDL.test.sdf"; // a multi molecule SDF file
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile fileContents = (ChemFile)reader.Read(new ChemFile());
            reader.Close();
            Assert.AreEqual(1, fileContents.Count);
            var sequence = fileContents[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(9, sequence.Count);
            for (int i = 0; i < sequence.Count; i++)
            {
                Assert.IsNotNull(sequence[i]);
            }
        }

        [TestMethod()]
        public void TestDataFromSDFReading()
        {
            string filename = "NCDK.Data.MDL.test.sdf"; // a multi molecule SDF file
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile fileContents = (ChemFile)reader.Read(new ChemFile());
            reader.Close();
            Assert.AreEqual(1, fileContents.Count);
            var sequence = fileContents[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(9, sequence.Count);
            var model = sequence[0];
            Assert.IsNotNull(model);

            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);
            IAtomContainer m = som[0];
            Assert.IsNotNull(m);
            Assert.AreEqual("1", m.GetProperty<string>("E_NSC"));
            Assert.AreEqual("553-97-9", m.GetProperty<string>("E_CAS"));
        }

        [TestMethod()]
        public void TestMultipleDataFields()
        {
            string filename = "NCDK.Data.MDL.bug1587283.mol";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile fileContents = (IChemFile)reader.Read(new ChemFile());
            reader.Close();
            Assert.AreEqual(1, fileContents.Count);
            IChemSequence sequence = fileContents[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(1, sequence.Count);
            IChemModel model = sequence[0];
            Assert.IsNotNull(model);
            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);
            IAtomContainer m = som[0];
            Assert.IsNotNull(m);
            Assert.AreEqual("B02", m.GetProperty<string>("id_no"));
            Assert.AreEqual("2-2", m.GetProperty<string>("eductkey"));
            Assert.AreEqual("1", m.GetProperty<string>("Step"));
            Assert.AreEqual("2", m.GetProperty<string>("Pos"));
            Assert.AreEqual("B02", m.GetProperty<string>("Tag"));
        }

        [TestMethod()]
        public void TestSDFFile4()
        {
            string filename = "NCDK.Data.MDL.test4.sdf"; // a multi molecule SDF file
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile fileContents = (ChemFile)reader.Read(new ChemFile());
            reader.Close();
            Assert.AreEqual(1, fileContents.Count);
            var sequence = fileContents[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(2, sequence.Count);
            for (int i = 0; i < sequence.Count; i++)
            {
                Assert.IsNotNull(sequence[i]);
            }
        }

        [TestMethod()]
        public void TestSDFFile3()
        {
            string filename = "NCDK.Data.MDL.test3.sdf"; // a multi molecule SDF file
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile fileContents = (ChemFile)reader.Read(new ChemFile());
            reader.Close();
            Assert.AreEqual(1, fileContents.Count);
            var sequence = fileContents[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(2, sequence.Count);
            for (int i = 0; i < sequence.Count; i++)
            {
                Assert.IsNotNull(sequence[i]);
            }
        }

        [TestMethod()]
        public void TestSDFFile5()
        {
            string filename = "NCDK.Data.MDL.test5.sdf"; // a multi molecule SDF file
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile fileContents = (ChemFile)reader.Read(new ChemFile());
            reader.Close();
            Assert.AreEqual(1, fileContents.Count);
            var sequence = fileContents[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(2, sequence.Count);
            for (int i = 0; i < sequence.Count; i++)
            {
                Assert.IsNotNull(sequence[i]);
            }
        }

        /// <summary>
        /// Test for bug 1974826.
        /// </summary>
        // @cdk.bug 1974826
        [TestMethod()]
        public void TestSDFFile6()
        {
            string filename = "NCDK.Data.MDL.test6.sdf"; // a multi molecule SDF file
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            ChemFile fileContents = (ChemFile)reader.Read(new ChemFile());
            reader.Close();
            Assert.AreEqual(1, fileContents.Count);
            var sequence = fileContents[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(3, sequence.Count);
            for (int i = 0; i < sequence.Count; i++)
            {
                Assert.IsNotNull(sequence[i]);
            }

            IChemModel model = sequence[0];
            Assert.IsNotNull(model);
            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);
            IAtomContainer m = som[0];
            Assert.IsNotNull(m);
            Assert.AreEqual("ola11", m.GetProperty<string>("STRUCTURE ID"));
        }

        /// <summary>
        /// Tests that data fields starting with a '>' are allowed.
        /// </summary>
        // @cdk.bug 2911300
        [TestMethod()]
        public void TestBug2911300()
        {
            string filename = "NCDK.Data.MDL.bug2911300.sdf";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IChemFile fileContents = (IChemFile)reader.Read(new ChemFile());
            reader.Close();
            Assert.AreEqual(1, fileContents.Count);
            IChemSequence sequence = fileContents[0];
            IChemModel model = sequence[0];
            Assert.IsNotNull(model);
            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);
            IAtomContainer m = som[0];
            Assert.IsNotNull(m);
            Assert.AreEqual(">1", m.GetProperty<string>("IC50_uM"));
        }
    }
}
