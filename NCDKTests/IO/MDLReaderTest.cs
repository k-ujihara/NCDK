/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading MDL mol files using one test file.
    /// A test case for SDF files is available as separate Class.
    /// </summary>
    /// <seealso cref="MDLReader"/>
    /// <seealso cref="SDFReaderTest"/>
    // @cdk.module test-io
    [TestClass()]
    public class MDLReaderTest 
        : SimpleChemObjectReaderTest
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;
        protected override string TestFile => "NCDK.Data.MDL.Strychnine_nichtOK.mol";
        protected override Type ChemObjectIOToTestType => typeof(MDLReader);

        [TestMethod()]
        public void TestAccepts()
        {
            var reader = new MDLReader(new StringReader(""))
            {
                ReaderMode = ChemObjectReaderMode.Strict
            };
            Assert.IsTrue(reader.Accepts(typeof(IChemFile)));
            Assert.IsTrue(reader.Accepts(typeof(IChemModel)));
            Assert.IsTrue(reader.Accepts(typeof(IAtomContainer)));
        }

        [TestMethod()]
        public void TestReadFromStringReader()
        {
            var mdl = "cyclopropane.mol\n" + "\n" + "\n" + "  9  9  0  0\n"
                    + "   -0.0073   -0.5272    0.9655 C   0  0  0  0  0\n"
                    + "   -0.6776   -0.7930   -0.3498 C   0  0  0  0  0\n"
                    + "    0.2103    0.4053   -0.1891 C   0  0  0  0  0\n"
                    + "    0.8019   -1.1711    1.2970 H   0  0  0  0  0\n"
                    + "   -0.6000   -0.2021    1.8155 H   0  0  0  0  0\n"
                    + "   -1.7511   -0.6586   -0.4435 H   0  0  0  0  0\n"
                    + "   -0.3492   -1.6277   -0.9620 H   0  0  0  0  0\n"
                    + "    1.1755    0.4303   -0.6860 H   0  0  0  0  0\n"
                    + "   -0.2264    1.3994   -0.1675 H   0  0  0  0  0\n" + "  1  2  1  6  0  0\n"
                    + "  1  3  1  6  0  0\n" + "  1  4  1  0  0  0\n" + "  1  5  1  1  0  0\n" + "  2  3  1  0  0  0\n"
                    + "  2  6  1  0  0  0\n" + "  2  7  1  6  0  0\n" + "  3  8  1  6  0  0\n" + "  3  9  1  0  0  0\n";
            var reader = new MDLReader(new StringReader(mdl), ChemObjectReaderMode.Strict);
            var chemFile = reader.Read(builder.NewChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            var model = seq[0];
            Assert.IsNotNull(model);

            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);
            var m = som[0];
            Assert.IsNotNull(m);
            Assert.AreEqual(9, m.Atoms.Count);
            Assert.AreEqual(9, m.Bonds.Count);
        }

        // @cdk.bug 1542467
        [TestMethod()]
        public void TestBug1542467()
        {
            var filename = "NCDK.Data.MDL.Strychnine_nichtOK.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLReader(ins, ChemObjectReaderMode.Strict);
            var chemFile = reader.Read(builder.NewChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToReadOnlyList();
            Assert.AreEqual(1, containersList.Count);
            Assert.IsTrue((containersList[0]).Atoms.Count > 0);
            Assert.IsTrue((containersList[0]).Bonds.Count > 0);
        }

        [TestMethod()]
        public void TestReadProton()
        {
            string mdl = "proton.mol\n" + "\n" + "\n" + "  1  0  0  0  0                 1\n"
                    + "   -0.0073   -0.5272    0.9655 H   0  3  0  0  0\n";
            var reader = new MDLReader(new StringReader(mdl), ChemObjectReaderMode.Strict);
            var mol = reader.Read(builder.NewAtomContainer());
            reader.Close();
            Assert.IsNotNull(mol);
            Assert.AreEqual(1, mol.Atoms.Count);
            Assert.AreEqual(0, mol.Bonds.Count);
            var atom = mol.Atoms[0];
            Assert.AreEqual(1, atom.FormalCharge.Value);
        }

        /// <summary>
        /// The corrupt file is really ok; it is just not V2000 material.
        /// </summary>
        [TestMethod()]
        public void TestSDF()
        {
            var filename = "NCDK.Data.MDL.prev2000.sd";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLReader(ins, ChemObjectReaderMode.Strict);
            var chemFile = reader.Read(builder.NewChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToReadOnlyList();
            Assert.AreEqual(2, containersList.Count);
            Assert.AreEqual(39, (containersList[0]).Atoms.Count);
            Assert.AreEqual(41, (containersList[0]).Bonds.Count);
            Assert.AreEqual(29, (containersList[1]).Atoms.Count);
            Assert.AreEqual(28, (containersList[1]).Bonds.Count);
        }

        /// <summary>
        /// Tests that the '0' read from the bond block for bond stereo
        /// is read is 'no stereochemistry involved'.
        /// </summary>
        [TestMethod()]
        public void TestStereoReadZeroDefault()
        {
            var filename = "NCDK.Data.MDL.prev2000.sd";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLReader(ins, ChemObjectReaderMode.Strict);
            var chemFile = reader.Read(builder.NewChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToReadOnlyList();
            Assert.AreEqual(2, containersList.Count);
            var container = containersList[0];
            Assert.AreEqual(BondStereo.None, container.Bonds[0].Stereo);
        }

        [TestMethod()]
        public void TestEmptyString()
        {
            var emptyString = "";
            var reader = new MDLReader(new StringReader(emptyString), ChemObjectReaderMode.Strict);
            var mol = reader.Read(builder.NewAtomContainer());
            reader.Close();
            Assert.IsNull(mol);
        }

        [TestMethod()]
        public void TestUndefinedStereo()
        {
            var filename = "NCDK.Data.MDL.ChEBI_26120.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLReader(ins, ChemObjectReaderMode.Relaxed);
            var mol = reader.Read(builder.NewAtomContainer());
            reader.Close();
            Assert.AreEqual(BondStereo.EOrZ, mol.Bonds[1].Stereo);
            Assert.AreEqual(BondStereo.EOrZ, mol.Bonds[6].Stereo);
            Assert.AreEqual(BondStereo.EOrZ, mol.Bonds[7].Stereo);
            Assert.AreEqual(BondStereo.EOrZ, mol.Bonds[11].Stereo);
        }

        [TestMethod()]
        public void TestReadAtomAtomMapping()
        {
            var filename = "NCDK.Data.MDL.a-pinene-with-atom-atom-mapping.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);

            var mol = reader.Read(builder.NewAtomContainer());
            reader.Close();
            Assert.IsNotNull(mol);
            Assert.AreEqual(1, mol.Atoms[0].GetProperty<int>(CDKPropertyName.AtomAtomMapping));
            Assert.AreEqual(15, mol.Atoms[1].GetProperty<int>(CDKPropertyName.AtomAtomMapping));
            Assert.IsNull(mol.Atoms[2].GetProperty<int?>(CDKPropertyName.AtomAtomMapping));
        }

        [TestMethod()]
        public void TestHas2DCoordinates_With000()
        {
            var filenameMol = "NCDK.Data.MDL.with000coordinate.mol";
            var ins = ResourceLoader.GetAsStream(filenameMol);
            var reader = new MDLReader(ins, ChemObjectReaderMode.Relaxed);
            var molOne = reader.Read(builder.NewAtomContainer());
            reader.Close();
            Assert.IsNull(molOne.Atoms[0].Point2D);
        }

        // @cdk.bug 3485634
        [TestMethod()]
        public void TestMissingAtomProperties()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.bug3485634.mol");
            var reader = new MDLReader(ins);
            var molecule = builder.NewAtomContainer();
            molecule = reader.Read(molecule);
            reader.Close();
            Assert.AreEqual(9, molecule.Atoms.Count);
        }

        // @cdk.bug 1356
        [TestMethod()]
        public void Properties()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.bug1356.sdf");
            var reader = new MDLReader(ins);
            var chemfile = builder.NewChemFile();
            chemfile = reader.Read(chemfile);
            var container = ChemFileManipulator.GetAllAtomContainers(chemfile).First();
            Assert.IsNotNull(container.GetProperty<object>("first"));
            Assert.IsNotNull(container.GetProperty<object>("second"));
            reader.Close();
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void WrongFormat()
        {
            var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.bug1356.sdf");
            var reader = new MDLReader(ins, ChemObjectReaderMode.Strict);
            var chemfile = builder.NewChemFile();
            chemfile = reader.Read(chemfile);
        }
    }
}
