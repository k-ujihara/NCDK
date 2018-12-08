/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 *  */
using NCDK.Common.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO.Formats;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NCDK.Tools;
using System;
using System.Linq;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the instantiation and functionality of the <see cref="FormatFactory"/>.
    /// </summary>
    // @cdk.module test-ioformats
    [TestClass()]
    public class FormatFactoryTest : CDKTestCase
    {
        private static FormatFactory factory = new FormatFactory();

        [TestMethod()]
        public void TestGaussian94()
        {
            ExpectFormat("NCDK.Data.Gaussian.4-cyanophenylnitrene-Benzazirine-TS.g94.out", Gaussian94Format.Instance);
        }

        [TestMethod()]
        public void TestGaussian98()
        {
            ExpectFormat("NCDK.Data.Gaussian.g98.out", Gaussian98Format.Instance);
        }

        [TestMethod()]
        public void TestGaussian92()
        {
            ExpectFormat("NCDK.Data.Gaussian.phenylnitrene.g92.out", Gaussian92Format.Instance);
        }

        [TestMethod()]
        public void TestGhemical()
        {
            ExpectFormat("NCDK.Data.Ghemical.ethene.mm1gp", GhemicalSPMFormat.Instance);
        }

        [TestMethod()]
        public void TestJaguar()
        {
            ExpectFormat("NCDK.Data.Jaguar.ch4-opt.out", JaguarFormat.Instance);
        }

        [TestMethod()]
        public void TestInChI()
        {
            ExpectFormat("NCDK.Data.InChI.guanine.inchi.xml", InChIFormat.Instance);
        }

        [TestMethod()]
        public void TestInChIPlainText()
        {
            ExpectFormat("NCDK.Data.InChI.guanine.inchi", InChIPlainTextFormat.Instance);
        }

        [TestMethod()]
        public void TestVASP()
        {
            ExpectFormat("NCDK.Data.VASP.LiMoS2_optimisation_ISIF3.vasp", VASPFormat.Instance);
        }

        [TestMethod()]
        public void TestAces2()
        {
            ExpectFormat("NCDK.Data.Aces2.ch3oh_ace.out", Aces2Format.Instance);
        }

        [TestMethod()]
        public void TestADF()
        {
            ExpectFormat("NCDK.Data.ADF.ammonia.adf.out", ADFFormat.Instance);
        }

        [TestMethod()]
        public void TestGamess()
        {
            ExpectFormat("NCDK.Data.Gamess.ch3oh_gam.out", GamessFormat.Instance);
        }

        [TestMethod()]
        public void TestABINIT()
        {
            ExpectFormat("NCDK.Data.ABINIT.t54.in", ABINITFormat.Instance);
        }

        [TestMethod()]
        public void TestCML()
        {
            ExpectFormat("NCDK.Data.CML.estron.cml", CMLFormat.Instance);
        }

        [TestMethod()]
        public void TestXYZ()
        {
            ExpectFormat("NCDK.Data.XYZ.bf3.xyz", XYZFormat.Instance);
        }

        [TestMethod()]
        public void TestShelX()
        {
            ExpectFormat("NCDK.Data.ShelX.frame_1.res", ShelXFormat.Instance);
        }

        [TestMethod()]
        public void TestMDLMol()
        {
            ExpectFormat("NCDK.Data.MDL.bug1014344-1.mol", MDLFormat.Instance);
        }

        [TestMethod()]
        public void TestMDLMolV2000()
        {
            ExpectFormat("NCDK.Data.MDL.methylbenzol.mol", MDLV2000Format.Instance);
        }

        [TestMethod()]
        public void TestDetection()
        {
            ExpectFormat("NCDK.Data.MDL.withcharges.mol", MDLV2000Format.Instance);
        }

        [TestMethod()]
        public void TestMDLMolV3000()
        {
            ExpectFormat("NCDK.Data.MDL.molV3000.mol", MDLV3000Format.Instance);
        }

        [TestMethod()]
        public void TestPDB()
        {
            ExpectFormat("NCDK.Data.PDB.coffeine.pdb", PDBFormat.Instance);
        }

        [TestMethod()]
        public void TestMol2()
        {
            ExpectFormat("NCDK.Data.Mol2.fromWebsite.mol2", Mol2Format.Instance);
        }

        [TestMethod()]
        public void TestCTX()
        {
            ExpectFormat("NCDK.Data.CTX.methanol_with_descriptors.ctx", CTXFormat.Instance);
        }

        [TestMethod()]
        public void TestPubChemCompoundASN()
        {
            ExpectFormat("NCDK.Data.ASN.PubChem.cid1.asn", PubChemASNFormat.Instance);
        }

        [TestMethod()]
        public void TestPubChemSubstancesASN()
        {
            ExpectFormat("NCDK.Data.ASN.PubChem.list.asn", PubChemSubstancesASNFormat.Instance);
        }

        [TestMethod()]
        public void TestPubChemCompoundsXML()
        {
            ExpectFormat("NCDK.Data.ASN.PubChem.aceticAcids38.xml", PubChemCompoundsXMLFormat.Instance);
        }

        [TestMethod()]
        public void TestPubChemSubstancesXML()
        {
            ExpectFormat("NCDK.Data.ASN.PubChem.taxols.xml", PubChemSubstancesXMLFormat.Instance);
        }

        [TestMethod()]
        public void TestPubChemSubstanceXML()
        {
            ExpectFormat("NCDK.Data.ASN.PubChem.sid577309.xml", PubChemSubstanceXMLFormat.Instance);
        }

        [TestMethod()]
        public void TestPubChemCompoundXML()
        {
            ExpectFormat("NCDK.Data.ASN.PubChem.cid1145.xml", PubChemCompoundXMLFormat.Instance);
        }

        private static void ExpectFormat(string filename, IResourceFormat expectedFormat)
        {
            var ins = ResourceLoader.GetAsStream(filename);
            Assert.IsNotNull(ins, $"Cannot find file: {filename}");
            if (expectedFormat is IChemFormatMatcher)
            {
                factory.RegisterFormat((IChemFormatMatcher)expectedFormat);
            }
            ins = new BufferedStream(ins);
            IChemFormat format = factory.GuessFormat(ins);
            Assert.IsNotNull(format);
            Assert.AreEqual(expectedFormat.FormatName, format.FormatName);
        }

        // @cdk.bug 2153298
        [TestMethod()]
        public void TestGuessFormat()
        {
            var filename = "NCDK.Data.XYZ.bf3.xyz";
            var input = ResourceLoader.GetAsStream(filename);
            input = new BufferedStream(input);
            IChemFormat format = factory.GuessFormat(input);
            Assert.IsNotNull(format);
            // make sure the Stream is properly reset
            var reader = new StreamReader(input);
            string line = reader.ReadLine();
            Assert.IsNotNull(line);
            Assert.AreEqual("4", line);
            line = reader.ReadLine();
            Assert.IsNotNull(line);
            Assert.AreEqual("Bortrifluorid", line);
        }

        [TestMethod()]
        public void TestGuessFormat_Gz()
        {
            var filename = "NCDK.Data.XYZ.bf3.xyz.gz";
            Stream input = new ReadSeekableStream(new GZipStream(ResourceLoader.GetAsStream(filename), CompressionMode.Decompress), 60000);
            IChemFormat format = factory.GuessFormat(input);
            Assert.IsNotNull(format);
            // make sure the Stream is properly reset
            var reader = new StreamReader(input);
            string line = reader.ReadLine();
            Assert.IsNotNull(line);
            Assert.AreEqual("4", line);
            line = reader.ReadLine();
            Assert.IsNotNull(line);
            Assert.AreEqual("Bortrifluorid", line);
        }

        [TestMethod()]
        public void TestGuessFormat_Reader()
        {
            var filename = "NCDK.Data.XYZ.bf3.xyz";
            var input = ResourceLoader.GetAsStream(filename);
            var reader = new StreamReader(input);
            IChemFormat format = factory.GuessFormat(reader);
            Assert.IsNotNull(format);
            // make sure the Reader is properly reset
            input.Seek(0, SeekOrigin.Begin);
            ((StreamReader)reader).DiscardBufferedData();
            string line = reader.ReadLine();
            Assert.IsNotNull(line);
            Assert.AreEqual("4", line);
            line = reader.ReadLine();
            Assert.IsNotNull(line);
            Assert.AreEqual("Bortrifluorid", line);
        }

        [TestMethod()]
        public void TestGetFormats()
        {
            var formats = factory.Formats;
            Assert.IsNotNull(formats);
            Assert.AreNotSame(0, formats.Count);
            foreach (var matcher in formats)
            {
                Assert.IsNotNull(matcher);
            }
        }

        class DummyFormat : IChemFormatMatcher
        {
            public string ReaderClassName => null;
            public string WriterClassName => null;
            public DataFeatures SupportedDataFeatures => 0;
            public DataFeatures RequiredDataFeatures => 0;
            public string FormatName => "Dummy Format";
            public string MIMEType => null;
            public bool IsXmlBased => false;
            public string PreferredNameExtension => "dummy";
            public IReadOnlyList<string> NameExtensions { get; } = new string[] { "dummy", "dum" };

            public MatchResult Matches(IEnumerable<string> lines)
            {
                var first = lines.FirstOrDefault();
                if (first != null && first.StartsWith("DummyFormat:", StringComparison.Ordinal))
                {
                    return new MatchResult(true, this, 0);
                }
                return MatchResult.NoMatch;
            }
        }

        [TestMethod()]
        public void TestRegisterFormat()
        {
            factory.RegisterFormat(new DummyFormat());
            StringReader reader = new StringReader("DummyFormat:");
            IChemFormat format = factory.GuessFormat(reader);
            Assert.IsNotNull(format);
            Assert.IsTrue(format is DummyFormat);
        }
    }
}
