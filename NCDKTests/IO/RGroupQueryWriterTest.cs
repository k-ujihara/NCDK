/*
 * Copyright (C) 2010  Mark Rijnbeek <mark_rynbeek@users.sf.net>
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may
 * distribute with programs based on this work.
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
using NCDK.Isomorphisms.Matchers;
using System.IO;
using System.Text.RegularExpressions;

namespace NCDK.IO
{
    /// <summary>
    /// JUnit tests for {@link org.openscience.cdk.io.RGroupQueryWriter}.
    /// Idea: read the test RGfiles into an object model, then writes the
    /// same model out as an RGfile again without changing anything. Then
    /// check that the original inputfile and the outputfile have the same content.
    ///
    // @cdk.module test-io
    // @author Mark Rijnbeek
    /// </summary>
    [TestClass()]
    public class RGroupQueryWriterTest : ChemObjectIOTest
    {
        protected override IChemObjectIO ChemObjectIOToTest { get; } = new RGroupQueryWriter();
        private static IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

        [TestMethod()]
        public override void TestAcceptsAtLeastOneChemObjectClass()
        {
            new RGroupQueryWriter().Accepts(typeof(RGroupQuery));
        }

        [TestMethod()]
        public void TestRgroupQueryFile_1()
        {
            string rgFile = Recreate("NCDK.Data.MDL.rgfile.1.mol");

            Assert.AreEqual(0, CountSubstring("AAL", rgFile), "AAL lines");
            Assert.AreEqual(1, CountSubstring("LOG", rgFile), "LOG lines");
            Assert.AreEqual(3, CountSubstring("APO", rgFile), "APO lines");
            Assert.IsTrue(rgFile.Contains("M  LOG  1   1   0   1   0,1-3"));
            Assert.AreEqual(59, CountSubstring("\n", rgFile), "Total #lines");
        }

        [TestMethod()]
        public void TestRgroupQueryFile_2()
        {
            string rgFile = Recreate("NCDK.Data.MDL.rgfile.2.mol");

            Assert.AreEqual(1, CountSubstring("AAL", rgFile), "AAL lines");
            Assert.AreEqual(3, CountSubstring("LOG", rgFile), "LOG lines");
            Assert.AreEqual(5, CountSubstring("APO", rgFile), "APO lines");
            Assert.IsTrue(rgFile.Contains("M  RGP  4   1  11   2   2   3   2   4   1"));
            Assert.AreEqual(107, CountSubstring("\n", rgFile), "Total #lines");
        }

        [TestMethod()]
        public void TestRgroupQueryFile_3()
        {
            string rgFile = Recreate("NCDK.Data.MDL.rgfile.3.mol");
            Assert.AreEqual(2, CountSubstring("AAL", rgFile), "AAL lines");
            Assert.AreEqual(1, CountSubstring("LOG", rgFile), "LOG lines");
            Assert.AreEqual(2, CountSubstring("APO", rgFile), "APO lines");
            Assert.AreEqual(66, CountSubstring("\n", rgFile), "Total #lines");
            Assert.IsTrue(rgFile.Contains("M  RGP  2   5   1   7   1"));
        }

        [TestMethod()]
        public void TestRgroupQueryFile_4()
        {
            string rgFile = Recreate("NCDK.Data.MDL.rgfile.4.mol");
            Assert.AreEqual(0, CountSubstring("AAL", rgFile), "AAL lines");
            Assert.AreEqual(3, CountSubstring("\\$CTAB", rgFile), "\\$CTAB lines");
            // the R-group is detached, we don't write APO lines (unlike the 0 value APO in the input file)
            Assert.AreEqual(0, CountSubstring("APO", rgFile), "APO lines");
            Assert.AreEqual(46, CountSubstring("\n", rgFile), "Total #lines");
            Assert.IsTrue(rgFile.Contains("M  RGP  1   6   1"));
        }

        [TestMethod()]
        public void TestRgroupQueryFile_5()
        {
            string rgFile = Recreate("NCDK.Data.MDL.rgfile.5.mol");
            Assert.AreEqual(4, CountSubstring("LOG", rgFile), "LOG lines");
            Assert.AreEqual(0, CountSubstring("APO", rgFile), "APO lines");
            Assert.AreEqual(2, CountSubstring("M  RGP", rgFile), "M  RGP lines"); //overflow
            Assert.AreEqual(132, CountSubstring("\n", rgFile), "Total #lines");
        }

        [TestMethod()]
        public void TestRgroupQueryFile_6()
        {
            string rgFile = Recreate("NCDK.Data.MDL.rgfile.6.mol");
            Assert.AreEqual(1, CountSubstring("AAL", rgFile), "AAL lines");
            Assert.AreEqual(3, CountSubstring("LOG", rgFile), "LOG lines");
            Assert.AreEqual(1, CountSubstring("APO", rgFile), "APO lines");
            Assert.AreEqual(57, CountSubstring("\n", rgFile), "Total #lines");
        }

        [TestMethod()]
        public void TestRgroupQueryFile_7()
        {
            string rgFile = Recreate("NCDK.Data.MDL.rgfile.7.mol");
            Assert.AreEqual(1, CountSubstring("LOG", rgFile), "LOG lines");
            Assert.AreEqual(2, CountSubstring("APO", rgFile), "APO lines");
            Assert.IsTrue(rgFile.Contains("M  RGP  3   4  32   6  32   7  32"));
            Assert.AreEqual(53, CountSubstring("\n", rgFile), "Total #lines");
        }

        private int CountSubstring(string regExp, string text)
        {
            var p = new Regex(regExp, RegexOptions.Compiled);
            var m = p.Matches(text); // get a matcher object
            return m.Count;
        }

        public void TestAcceptsAtLeastOneDebugObject() { }

        public override void TestAcceptsAtLeastOneNonotifyObject() { }

        private string Recreate(string file)
        {
            StringWriter sw = new StringWriter();
            RGroupQueryWriter rgw = new RGroupQueryWriter(sw);
            var ins = ResourceLoader.GetAsStream(file);
            RGroupQueryReader reader = new RGroupQueryReader(ins);
            RGroupQuery rGroupQuery = (RGroupQuery)reader.Read(new RGroupQuery(Default.ChemObjectBuilder.Instance));
            rgw.Write(rGroupQuery);
            string output = sw.ToString();
            return output;
        }
    }
}
