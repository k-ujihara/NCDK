/* Copyright (C) 2004-2007  Egon Willighagen <egonw@users.sf.net>
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
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using System;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading Cerius<sup>2</sup> Polymorph Predictor files using a test file.
    /// </summary>
    /// <see cref="PMPReader"/>
    // @cdk.module test-io
    [TestClass()]
    public class PMPReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.PMP.aceticacid.pmp";
        protected override Type ChemObjectIOToTestType => typeof(PMPReader);

        [TestMethod()]
        public void TestAccepts()
        {
            PMPReader reader = new PMPReader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
        }

        [TestMethod()]
        public void TestAceticAcid()
        {
            string filename = "NCDK.Data.PMP.aceticacid.pmp";
            var ins = ResourceLoader.GetAsStream(filename);
            PMPReader reader = new PMPReader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            ICrystal crystal = model.Crystal;
            Assert.IsNotNull(crystal);
            Assert.AreEqual(32, crystal.Atoms.Count);
            Assert.AreEqual(28, crystal.Bonds.Count);

            Assert.AreEqual("O", crystal.Atoms[6].Symbol);
            Assert.AreEqual(1.4921997, crystal.Atoms[6].Point3D.Value.X, 0.00001);
            Assert.AreEqual("O", crystal.Atoms[7].Symbol);
            Assert.AreEqual(1.4922556, crystal.Atoms[7].Point3D.Value.X, 0.00001);
        }

        [TestMethod()]
        public void TestTwoAceticAcid()
        {
            string filename = "NCDK.Data.PMP.two_aceticacid.pmp";
            var ins = ResourceLoader.GetAsStream(filename);
            PMPReader reader = new PMPReader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(2, seq.Count);

            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            ICrystal crystal = model.Crystal;
            Assert.IsNotNull(crystal);
            Assert.AreEqual(32, crystal.Atoms.Count);
            Assert.AreEqual(28, crystal.Bonds.Count);

            model = seq[1];
            Assert.IsNotNull(model);
            crystal = model.Crystal;
            Assert.IsNotNull(crystal);
            Assert.AreEqual(32, crystal.Atoms.Count);
            Assert.AreEqual(28, crystal.Bonds.Count);
        }
    }
}
