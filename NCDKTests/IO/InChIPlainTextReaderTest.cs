/* Copyright (C) 2002-2007  The Chemistry Development Kit (CDK) project
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
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading INChI plain text files.
    /// </summary>
    /// <see cref="InChIReader"/>
    // @cdk.module test-extra
    [TestClass()]
    public class InChIPlainTextReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.InChI.guanine.inchi";
        protected override Type ChemObjectIOToTestType => typeof(InChIPlainTextReader);

        [TestMethod()]
        public void TestAccepts()
        {
            InChIPlainTextReader reader = new InChIPlainTextReader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
            reader.Close();
        }

        /// <summary>
        /// Test a INChI 1.12Beta file containing the two tautomers of guanine.
        /// </summary>
        [TestMethod()]
        public void TestGuanine()
        {
            string filename = "NCDK.Data.InChI.guanine.inchi";
            Trace.TraceInformation("Testing: ", filename);
            var ins = ResourceLoader.GetAsStream(filename);
            InChIPlainTextReader reader = new InChIPlainTextReader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            var moleculeSet = model.MoleculeSet;
            Assert.IsNotNull(moleculeSet);
            IAtomContainer molecule = moleculeSet[0];
            Assert.IsNotNull(molecule);

            Assert.AreEqual(11, molecule.Atoms.Count);
            Assert.AreEqual(12, molecule.Bonds.Count);
        }

        [TestMethod()]
        public void TestChebi26120()
        {
            var ins = new StringReader("InChI=1/C40H62/c1-33(2)19-13-23-37(7)27-17-31-39(9)29-15-25-35(5)21-11-12-22-36(6)26-16-30-40(10)32-18-28-38(8)24-14-20-34(3)4/h11-12,15,19-22,25,27-30H,13-14,16-18,23-24,26,31-32H2,1-10H3");
            InChIPlainTextReader reader = new InChIPlainTextReader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            var moleculeSet = model.MoleculeSet;
            Assert.IsNotNull(moleculeSet);
            IAtomContainer molecule = moleculeSet[0];
            Assert.IsNotNull(molecule);

            Assert.AreEqual(40, molecule.Atoms.Count);
            Assert.AreEqual(39, molecule.Bonds.Count);
        }

        [TestMethod()]
        public void TestPlatinum()
        {
            StringReader ins = new StringReader("InChI=1S/Pt");
            InChIPlainTextReader reader = new InChIPlainTextReader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            var moleculeSet = model.MoleculeSet;
            Assert.IsNotNull(moleculeSet);
            IAtomContainer molecule = moleculeSet[0];
            Assert.IsNotNull(molecule);

            Assert.AreEqual(1, molecule.Atoms.Count);
            Assert.AreEqual(0, molecule.Bonds.Count);
        }
    }
}
