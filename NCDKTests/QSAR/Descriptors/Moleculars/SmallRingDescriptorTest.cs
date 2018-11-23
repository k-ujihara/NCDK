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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Primitives;
using NCDK.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class SmallRingDescriptorTest : MolecularDescriptorTest<SmallRingDescriptor>
    {
        [TestMethod()]
        public void TestDescriptors()
        {
            string fnzip = "NCDK.Data.CDD.aromring_validation.zip";
            using (var ins = ResourceLoader.GetAsStream(fnzip))
            {
                Validate(ins);
            }
        }

        // run through the cases
        private static void Validate(Stream ins)
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
                if (!content.TryGetValue(basefn + ".mol", out byte[] molBytes))
                    break;

                var mol = CDK.Builder.NewAtomContainer();
                using (var mdl = new MDLV2000Reader(new MemoryStream(molBytes)))
                {
                    mdl.Read(mol);
                }

                IList<string> bits;
                using (var rdr = new StreamReader(new MemoryStream(content[basefn + ".rings"])))
                {
                    bits = Strings.Tokenize(rdr.ReadLine(), ' ');
                }
                int wantSmallRings = int.Parse(bits[0], NumberFormatInfo.InvariantInfo);
                int wantRingBlocks = int.Parse(bits[1], NumberFormatInfo.InvariantInfo);
                int wantAromRings = int.Parse(bits[2], NumberFormatInfo.InvariantInfo);
                int wantAromBlocks = int.Parse(bits[3], NumberFormatInfo.InvariantInfo);

                Trace.TraceInformation("FN=" + basefn + " MOL=" + mol.Atoms.Count + "," + mol.Bonds.Count + " nSmallRings="
                        + wantSmallRings + " nRingBlocks=" + wantRingBlocks + " nAromRings=" + wantAromRings
                        + " nAromBlocks=" + wantAromBlocks);

                var descr = new SmallRingDescriptor(mol);
                var results = descr.Calculate();
                var names = results.Keys;
                var values = results.Values.Cast<int>().ToReadOnlyList();

                int gotSmallRings = 0, gotRingBlocks = 0, gotAromRings = 0, gotAromBlocks = 0;
                int n = 0;
                foreach (var name in names)
                {
                    if (name.Equals("nSmallRings"))
                        gotSmallRings = values[n];
                    else if (name.Equals("nRingBlocks"))
                        gotRingBlocks = values[n];
                    else if (name.Equals("nAromRings"))
                        gotAromRings = values[n];
                    else if (name.Equals("nAromBlocks"))
                        gotAromBlocks = values[n];
                    n++;
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
                    Assert.Fail(error);
                }
            }
        }
    }
}
