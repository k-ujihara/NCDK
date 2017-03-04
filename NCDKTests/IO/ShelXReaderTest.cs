/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Geometries;
using System.Diagnostics;

namespace NCDK.IO
{
    ///  @cdk.module test-io
    [TestClass()]
    public class ShelXReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.ShelX.frame_1.res";
        static readonly ShelXReader simpleReader = new ShelXReader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        [TestMethod()]
        public void TestAccepts()
        {
            ShelXReader reader = new ShelXReader();
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
            Assert.IsTrue(reader.Accepts(typeof(Crystal)));
        }

        [TestMethod()]
        public void TestReading()
        {
            string filename = "NCDK.Data.ShelX.frame_1.res";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            ShelXReader reader = new ShelXReader(ins);
            Crystal crystal = (Crystal)reader.Read(new Crystal());
            reader.Close();
            Assert.IsNotNull(crystal);
            Assert.AreEqual(42, crystal.Atoms.Count);
            var notional = CrystalGeometryTools.CartesianToNotional(crystal.A, crystal.B, crystal.C);
            Assert.AreEqual(7.97103, notional[0], 0.001);
            Assert.AreEqual(18.77220, notional[1], 0.001);
            Assert.AreEqual(10.26222, notional[2], 0.001);
            Assert.AreEqual(90.0000, notional[3], 0.001);
            Assert.AreEqual(90.0000, notional[4], 0.001);
            Assert.AreEqual(90.0000, notional[5], 0.001);
        }
    }
}
