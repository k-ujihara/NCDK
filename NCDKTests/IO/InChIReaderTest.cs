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
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading INChI files using one test file.
    /// </summary>
    /// <seealso cref="InChIReader"/>
    // @cdk.module test-extra
    [TestClass()]
    public class InChIReaderTest 
        : SimpleChemObjectReaderTest
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;
        protected override string TestFile => "NCDK.Data.InChI.guanine.inchi.xml";
        protected override Type ChemObjectIOToTestType => typeof(InChIReader);
        
        [TestMethod()]
        public void TestAccepts()
        {
            Assert.IsTrue(ChemObjectIOToTest.Accepts(typeof(IChemFile)));
        }

        /// <summary>
        /// Test a INChI 1.1Beta file containing the two tautomers
        /// of guanine.
        /// </summary>
        [TestMethod()]
        public void TestGuanine()
        {
            var filename = "NCDK.Data.InChI.guanine.inchi.xml";
            Trace.TraceInformation("Testing: ", filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new InChIReader(ins);
            var chemFile = reader.Read(builder.NewChemFile());

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            var model = seq[0];
            Assert.IsNotNull(model);
            var moleculeSet = model.MoleculeSet;
            Assert.IsNotNull(moleculeSet);
            var molecule = moleculeSet[0];
            Assert.IsNotNull(molecule);

            Assert.AreEqual(11, molecule.Atoms.Count);
            Assert.AreEqual(12, molecule.Bonds.Count);
        }

        [TestMethod()]
        public override void TestSetReader_Reader()
        {
            // CDKException expected as these INChI files are XML, which must
            // be read via Stream
            var ctor = ChemObjectIOToTestType.GetConstructor(new Type[] { typeof(TextReader) });
            Assert.IsNull(ctor);
        }
    }
}
