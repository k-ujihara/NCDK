/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Numerics;
using System;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading CIF files using a test file with the <see cref="CIFReader"/>.
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public class CIFReaderTest : ChemObjectIOTest
    {
        protected override Type ChemObjectIOToTestType => typeof(CIFReader);

        /// <summary>
        /// Ensure a CIF file from the crystallography open database can be read.
        /// Example input <see href="http://www.crystallography.net/1100784.cif">1100784</see>.
        /// </summary>
        [TestMethod()]
        public void Cod1100784()
        {
            var ins = ResourceLoader.GetAsStream(GetType(), "1100784.cif");
            CIFReader cifReader = new CIFReader(ins);
            //        try {
            IChemFile chemFile = cifReader.Read(new ChemFile());
            Assert.AreEqual(1, chemFile.Count);
            Assert.AreEqual(1, chemFile[0].Count);
            Assert.IsNotNull(chemFile[0][0].Crystal);
            //        } finally {
            cifReader.Close();
            //        }
        }

        [TestMethod()]
        public void Cod1100784AtomCount()
        {
            var ins = ResourceLoader.GetAsStream(GetType(), "1100784.cif");
            CIFReader cifReader = new CIFReader(ins);
            IChemFile chemFile = cifReader.Read(new ChemFile());
            ICrystal crystal = chemFile[0][0].Crystal;
            Assert.AreEqual(72, crystal.Atoms.Count);
            cifReader.Close();
        }

        [TestMethod()]
        public void Cod1100784CellLengths()
        {
            var ins = ResourceLoader.GetAsStream(GetType(), "1100784.cif");
            CIFReader cifReader = new CIFReader(ins);
            IChemFile chemFile = cifReader.Read(new ChemFile());
            ICrystal crystal = chemFile[0][0].Crystal;
            Assert.IsTrue(Math.Abs(crystal.A.Length() - 10.9754) < 1E-5);
            Assert.IsTrue(Math.Abs(crystal.B.Length() - 11.4045) < 1E-5);
            Assert.IsTrue(Math.Abs(crystal.C.Length() - 12.9314) < 1E-5);
            cifReader.Close();
        }

        [TestMethod()]
        public void Cod1100784CellAngles()
        {
            var ins = ResourceLoader.GetAsStream(GetType(), "1100784.cif");
            CIFReader cifReader = new CIFReader(ins);
            IChemFile chemFile = cifReader.Read(new ChemFile());
            ICrystal crystal = chemFile[0][0].Crystal;
            Vector3 a = crystal.A;
            Vector3 b = crystal.B;
            Vector3 c = crystal.C;
            double alpha = Math.Acos(Vector3.Dot(b, c) / (b.Length() * c.Length())) * 180 / Math.PI;
            double beta = Math.Acos(Vector3.Dot(c, a) / (c.Length() * a.Length())) * 180 / Math.PI;
            double gamma = Math.Acos(Vector3.Dot(a, b) / (a.Length() * b.Length())) * 180 / Math.PI;
            Assert.IsTrue(Math.Abs(alpha - 109.1080) < 1E-5);
            Assert.IsTrue(Math.Abs(beta - 98.4090) < 1E-5);
            Assert.IsTrue(Math.Abs(gamma - 102.7470) < 1E-5);
            cifReader.Close();
        }
    }
}
