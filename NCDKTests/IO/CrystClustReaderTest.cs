/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using System.Diagnostics;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading MDL mol files using one test file.
    /// </summary>
    /// <seealso cref="CrystClustReader"/>
    // @cdk.module test-extra
    [TestClass()]
    public class CrystClustReaderTest
        : SimpleChemObjectReaderTest
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;

        protected override string TestFile => "NCDK.Data.CrystClust.estron.crystclust";
        protected override Type ChemObjectIOToTestType => typeof(CrystClustReader);

        [TestMethod()]
        public void TestAccepts()
        {
            Assert.IsTrue(ChemObjectIOToTest.Accepts(typeof(IChemFile)));
            Assert.IsFalse(ChemObjectIOToTest.Accepts(typeof(IAtomContainer)));
        }

        [TestMethod()]
        public void TestEstrone()
        {
            var filename = "NCDK.Data.CrystClust.estron.crystclust";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new CrystClustReader(ins);
            var chemFile = reader.Read(builder.NewChemFile());

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(2, seq.Count);
            var model = seq[0];
            Assert.IsNotNull(model);

            var crystal = model.Crystal;
            Assert.IsNotNull(crystal);
            Assert.AreEqual(42, crystal.Atoms.Count);
            Assert.AreEqual(1, crystal.Z.Value);

            // test reading of partial charges
            var atom = crystal.Atoms[0];
            Assert.IsNotNull(atom);
            Assert.AreEqual("O", atom.Symbol);
            Assert.AreEqual(-0.68264902, atom.Charge.Value, 0.00000001);

            // test unit cell axes
            var a = crystal.A;
            Assert.AreEqual(7.971030, a.X, 0.000001);
            Assert.AreEqual(0.0, a.Y, 0.000001);
            Assert.AreEqual(0.0, a.Z, 0.000001);
            var b = crystal.B;
            Assert.AreEqual(0.0, b.X, 0.000001);
            Assert.AreEqual(18.772200, b.Y, 0.000001);
            Assert.AreEqual(0.0, b.Z, 0.000001);
            var c = crystal.C;
            Assert.AreEqual(0.0, c.X, 0.000001);
            Assert.AreEqual(0.0, c.Y, 0.000001);
            Assert.AreEqual(10.262220, c.Z, 0.000001);
        }
    }
}
