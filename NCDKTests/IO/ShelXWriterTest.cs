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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Geometries;
using System.IO;

namespace NCDK.IO
{
    // @cdk.module test-extra
    [TestClass()]
    public class ShelXWriterTest : CDKTestCase
    {
        [TestMethod()]
        public void TestRoundTrip()
        {
            Crystal crystal = new Crystal();
            double a = 3.0;
            double b = 5.0;
            double c = 7.0;
            double alpha = 90.0;
            double beta = 110.0;
            double gamma = 100.0;
            var axes = CrystalGeometryTools.NotionalToCartesian(a, b, c, alpha, beta, gamma);
            crystal.A = axes[0];
            crystal.B = axes[1];
            crystal.C = axes[2];

            // serialazing
            StringWriter sWriter = new StringWriter();
            ShelXWriter resWriter = new ShelXWriter(sWriter);
            resWriter.Write(crystal);
            resWriter.Close();
            string resContent = sWriter.ToString();

            // deserialazing
            ShelXReader resReader = new ShelXReader(new StringReader(resContent));
            ICrystal rCrystal = (ICrystal)resReader.Read(new Crystal());

            // OK, do checking
            Assert.IsNotNull(rCrystal);
            Assert.AreEqual(crystal.A.X, rCrystal.A.X, 0.001);
            Assert.AreEqual(crystal.A.Y, rCrystal.A.Y, 0.001);
            Assert.AreEqual(crystal.A.Z, rCrystal.A.Z, 0.001);
            Assert.AreEqual(crystal.B.X, rCrystal.B.X, 0.001);
            Assert.AreEqual(crystal.B.Y, rCrystal.B.Y, 0.001);
            Assert.AreEqual(crystal.B.Z, rCrystal.B.Z, 0.001);
            Assert.AreEqual(crystal.C.X, rCrystal.C.X, 0.001);
            Assert.AreEqual(crystal.C.Y, rCrystal.C.Y, 0.001);
            Assert.AreEqual(crystal.C.Z, rCrystal.C.Z, 0.001);
        }
    }
}
