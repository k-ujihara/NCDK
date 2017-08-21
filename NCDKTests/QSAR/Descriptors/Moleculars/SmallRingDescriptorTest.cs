/* Copyright (c) 2014 Collaborative Drug Discovery, Inc. <alex@collaborativedrug.com>
 *
 * Implemented by Alex M. Clark, produced by Collaborative Drug Discovery, Inc.
 * Made available to the CDK community under the terms of the GNU LGPL.
 *
 *    http://collaborativedrug.com
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Common.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;
using NCDK.QSAR.Results;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Test for small rings Descriptor.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class SmallRingDescriptorTest : MolecularDescriptorTest
    {
        public SmallRingDescriptorTest()
        {
            SetDescriptor(typeof(SmallRingDescriptor));
        }

        [TestMethod()]
        public void TestDescriptors()
        {
            Trace.TraceInformation("CircularFingerprinter test: loading source materials");

            string fnzip = "NCDK.Data.CDD.aromring_validation.zip";
            Trace.TraceInformation("Loading source content: " + fnzip);
            using (var ins = ResourceLoader.GetAsStream(fnzip))
            {
                Validate(ins);
            }
            Trace.TraceInformation("CircularFingerprinter test: completed without any problems");
        }

        // included to shutdown the warning messages for not having tests for trivial methods
        [TestMethod()]
        public void Nop() { }

        // run through the cases
        private void Validate(Stream ins)
        {
            var content = new Dictionary<string, byte[]>();
            using (var zip = new ZipArchive(ins))
            {
                // stream the contents form the zipfile: these are all short
                foreach (var ze in zip.Entries)
                {
                    string fn = ze.Name;
                    using (var buff = new MemoryStream())
                    using (var zs = ze.Open())
                    {
                        int b;
                        while ((b = zs.ReadByte()) != -1)
                            buff.WriteByte((byte)b);
                        content[fn] = buff.ToArray();
                    }
                }
            }

            for (int idx = 1; ; idx++)
            {
                string basefn = idx.ToString();
                while (basefn.Length < 6)
                    basefn = "0" + basefn;
                byte[] molBytes;
                if (!content.TryGetValue(basefn + ".mol", out molBytes))
                    break;

                AtomContainer mol = new AtomContainer();
                using (var mdl = new MDLV2000Reader(new MemoryStream(molBytes)))
                {
                    mdl.Read(mol);
                }

                IList<string> bits;
                using (var rin = new MemoryStream(content[basefn + ".rings"]))
                using (var rdr = new StreamReader(rin))
                {
                    bits = Strings.Tokenize(rdr.ReadLine(), ' ');
                }
                int wantSmallRings = int.Parse(bits[0]);
                int wantRingBlocks = int.Parse(bits[1]);
                int wantAromRings = int.Parse(bits[2]);
                int wantAromBlocks = int.Parse(bits[3]);

                Trace.TraceInformation("FN=" + basefn + " MOL=" + mol.Atoms.Count + "," + mol.Bonds.Count + " nSmallRings="
                        + wantSmallRings + " nRingBlocks=" + wantRingBlocks + " nAromRings=" + wantAromRings
                        + " nAromBlocks=" + wantAromBlocks);

                SmallRingDescriptor descr = new SmallRingDescriptor();
                DescriptorValue results = descr.Calculate(mol);
                string[] names = results.Names;
                ArrayResult<int> values = (ArrayResult<int>)results.Value;

                int gotSmallRings = 0, gotRingBlocks = 0, gotAromRings = 0, gotAromBlocks = 0;
                for (int n = 0; n < names.Length; n++)
                {
                    if (names[n].Equals("nSmallRings"))
                        gotSmallRings = values[n];
                    else if (names[n].Equals("nRingBlocks"))
                        gotRingBlocks = values[n];
                    else if (names[n].Equals("nAromRings"))
                        gotAromRings = values[n];
                    else if (names[n].Equals("nAromBlocks")) gotAromBlocks = values[n];
                }

                string error = null;
                if (gotSmallRings != wantSmallRings)
                    error = "Got " + gotSmallRings + " small rings, expected " + wantSmallRings;
                else if (gotRingBlocks != wantRingBlocks)
                    error = "Got " + gotRingBlocks + " ring blocks, expected " + wantRingBlocks;
                else if (gotAromRings != wantAromRings)
                    error = "Got " + gotAromRings + " aromatic rings, expected " + wantAromRings;
                else if (gotAromBlocks != wantAromBlocks)
                    error = "Got " + gotAromBlocks + " aromatic blocks, expected " + wantAromBlocks;

                if (error != null)
                {
                    using (var str = new StringWriter())
                    using (var wtr = new MDLV2000Writer(str))
                    {
                        wtr.Write(mol);
                        error += "\nMolecule:\n" + str.ToString();
                    }
                    throw new CDKException(error);
                }
            }
        }
    }
}
