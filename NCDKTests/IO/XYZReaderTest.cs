/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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
using System.Diagnostics;

namespace NCDK.IO
{
    /**
     * TestCase for the reading XYZ files using a test file.
     *
     * @cdk.module test-io
     *
     * @see org.openscience.cdk.io.XYZReader
     */
    [TestClass()]
    public class XYZReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.XYZ.viagra.xyz";
        static readonly XYZReader simpleReader = new XYZReader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        [TestMethod()]
        public void TestAccepts()
        {
            XYZReader reader = new XYZReader();
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
        }

        [TestMethod()]
        public void TestViagra()
        {
            string filename = "NCDK.Data.XYZ.viagra.xyz";
            Trace.TraceInformation("Testing: ", filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            XYZReader reader = new XYZReader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);
            IAtomContainer m = som[0];
            Assert.IsNotNull(m);
            Assert.AreEqual(63, m.Atoms.Count);
            Assert.AreEqual(0, m.Bonds.Count);

            Assert.AreEqual("N", m.Atoms[0].Symbol);
            Assert.IsNotNull(m.Atoms[0].Point3D);
            Assert.AreEqual(-3.4932, m.Atoms[0].Point3D.Value.X, 0.0001);
            Assert.AreEqual(-1.8950, m.Atoms[0].Point3D.Value.Y, 0.0001);
            Assert.AreEqual(0.1795, m.Atoms[0].Point3D.Value.Z, 0.0001);
        }

        [TestMethod()]
        public void TestComment()
        {
            string filename = "NCDK.Data.XYZ.viagra_withComment.xyz";
            Trace.TraceInformation("Testing: ", filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            XYZReader reader = new XYZReader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);
            IAtomContainer m = som[0];
            Assert.IsNotNull(m);
            Assert.AreEqual(63, m.Atoms.Count);
            Assert.AreEqual(0, m.Bonds.Count);

            // atom 63: H    3.1625    3.1270   -0.9362
            Assert.AreEqual("H", m.Atoms[62].Symbol);
            Assert.IsNotNull(m.Atoms[62].Point3D);
            Assert.AreEqual(3.1625, m.Atoms[62].Point3D.Value.X, 0.0001);
            Assert.AreEqual(3.1270, m.Atoms[62].Point3D.Value.Y, 0.0001);
            Assert.AreEqual(-0.9362, m.Atoms[62].Point3D.Value.Z, 0.0001);
        }
    }
}
