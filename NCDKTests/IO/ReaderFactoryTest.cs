/* Copyright (C) 2003-2018  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
 *  
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO.Formats;
using NCDK.Tools.Manipulator;
using System.IO;
using System.IO.Compression;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the instantiation and functionality of the {@link ReaderFactory}.
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public class ReaderFactoryTest : AbstractReaderFactoryTest
    {
        private ReaderFactory factory = new ReaderFactory();

        [TestMethod()]
        public void TestCreateReader_IChemFormat()
        {
            IChemFormat format = (IChemFormat)XYZFormat.Instance;
            ISimpleChemObjectReader reader = factory.CreateReader(format, new StringReader(""));
            Assert.IsNotNull(reader);
            Assert.AreEqual(format.FormatName, reader.Format.FormatName);
        }

        [TestMethod()]
        public void TestGaussian98()
        {
            ExpectReader("NCDK.Data.Gaussian.g98.out", Gaussian98Format.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestGhemical()
        {
            ExpectReader("NCDK.Data.Ghemical.ethene.mm1gp", GhemicalSPMFormat.Instance, 6, 5);
        }

        [TestMethod()]
        public void TestCML()
        {
            ExpectReader("NCDK.Data.CML.estron.cml", CMLFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestXYZ()
        {
            ExpectReader("NCDK.Data.XYZ.bf3.xyz", XYZFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestShelX()
        {
            ExpectReader("NCDK.Data.ShelX.frame_1.res", ShelXFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestMDLMol()
        {
            ExpectReader("NCDK.Data.MDL.bug1014344-1.mol", MDLFormat.Instance, 21, 21);
        }

        [TestMethod()]
        public void TestMDLMolV2000()
        {
            ExpectReader("NCDK.Data.MDL.methylbenzol.mol", MDLV2000Format.Instance, 15, 15);
        }

        [TestMethod()]
        public void TestDetection()
        {
            ExpectReader("NCDK.Data.MDL.withcharges.mol", MDLV2000Format.Instance, 9, 9);
        }

        [TestMethod()]
        public void TestMDLMolV3000()
        {
            ExpectReader("NCDK.Data.MDL.molV3000.mol", MDLV3000Format.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestMDLRxnV2000()
        {
            ExpectReader("NCDK.Data.MDL.reaction-1.rxn", MDLRXNV2000Format.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestMDLRxnV3000()
        {
            ExpectReader("NCDK.Data.MDL.reaction_v3.rxn", MDLRXNV3000Format.Instance, -1, -1);
        }

        [Ignore()] // test moved to cdk-test-pdb/PDBReaderFactoryTest
        [TestMethod()]
        public void TestPDB()
        {
            ExpectReader("NCDK.Data.PDB.coffeine.pdb", PDBFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestMol2()
        {
            ExpectReader("NCDK.Data.Mol2.fromWebsite.mol2", Mol2Format.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestCTX()
        {
            ExpectReader("NCDK.Data.CTX.methanol_with_descriptors.ctx", CTXFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestPubChemCompoundASN()
        {
            ExpectReader("NCDK.Data.ASN.PubChem.cid1.asn", PubChemASNFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestPubChemSubstanceXML()
        {
            ExpectReader("NCDK.Data.ASN.PubChem.sid577309.xml", PubChemSubstanceXMLFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestPubChemCompoundXML()
        {
            ExpectReader("NCDK.Data.ASN.PubChem.cid1145.xml", PubChemCompoundXMLFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestSmiles()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.Smiles.drugs.smi");
            var reader = factory.CreateReader(ins);
            Assert.IsNull(reader);
        }

        // @cdk.bug 2153298
        [TestMethod()]
        public void TestBug2153298()
        {
            string filename = "NCDK.Data.ASN.PubChem.cid1145.xml";
            var ins = ResourceLoader.GetAsStream(filename);
            Assert.IsNotNull(ins, "Cannot find file: " + filename);
            IChemFormatMatcher realFormat = (IChemFormatMatcher)PubChemCompoundXMLFormat.Instance;
            factory.RegisterFormat(realFormat);
            // ok, if format ok, try instantiating a reader
            ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = factory.CreateReader(ins);
            Assert.IsNotNull(reader);
            Assert.AreEqual(((IChemFormat)PubChemCompoundXMLFormat.Instance).ReaderClassName, reader.GetType().FullName);
            // now try reading something from it
            IAtomContainer molecule = (IAtomContainer)reader.Read(new AtomContainer());
            Assert.IsNotNull(molecule);
            Assert.AreNotSame(0, molecule.Atoms.Count);
            Assert.AreNotSame(0, molecule.Bonds.Count);
        }

        [TestMethod()]
        public void TestReadGz()
        {
            string filename = "NCDK.Data.XYZ.bf3.xyz.gz";
            var input =new GZipStream(ResourceLoader.GetAsStream(filename), CompressionMode.Decompress);
            // ok, if format ok, try instantiating a reader
            ISimpleChemObjectReader reader = factory.CreateReader(input);
            Assert.IsNotNull(reader);
            Assert.AreEqual(((IChemFormat)XYZFormat.Instance).ReaderClassName, reader.GetType().FullName);
            // now try reading something from it
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IAtomContainer molecule = new AtomContainer();
            foreach (var container in ChemFileManipulator.GetAllAtomContainers(chemFile))
            {
                molecule.Add(container);
            }
            Assert.IsNotNull(molecule);
            Assert.AreEqual(4, molecule.Atoms.Count);
        }

        [TestMethod()]
        public void TestReadGzWithGzipDetection()
        {
            string filename = "NCDK.Data.XYZ.bf3.xyz.gz";
            var input = ResourceLoader.GetAsStream(filename);
            // ok, if format ok, try instantiating a reader
            ISimpleChemObjectReader reader = factory.CreateReader(input);
            Assert.IsNotNull(reader);
            Assert.AreEqual(((IChemFormat)XYZFormat.Instance).ReaderClassName, reader.GetType().FullName);
            // now try reading something from it
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            IAtomContainer molecule = new AtomContainer();
            foreach (var container in ChemFileManipulator.GetAllAtomContainers(chemFile))
            {
                molecule.Add(container);
            }
            Assert.IsNotNull(molecule);
            Assert.AreEqual(4, molecule.Atoms.Count);
        }
    }
}
