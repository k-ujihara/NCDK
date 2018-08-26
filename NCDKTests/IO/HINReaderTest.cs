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
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;
using System.Linq;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading HIN mol files using one test file.
    /// </summary>
    /// <see cref="HINReader"/>
    // @cdk.module test-io
    [TestClass()]
    public class HINReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.HIN.benzene.hin";
        protected override Type ChemObjectIOToTestType => typeof(HINReader);

        [TestMethod()]
        public void TestAccepts()
        {
            Assert.IsTrue(ChemObjectIOToTest.Accepts(typeof(ChemFile)));
        }

        [TestMethod()]
        public void TestBenzene()
        {
            string filename = "NCDK.Data.HIN.benzene.hin";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            HINReader reader = new HINReader(ins);
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
            Assert.AreEqual(12, m.Atoms.Count);
            // AreEqual(?, m.Bonds.Count);
        }

        [TestMethod()]
        public void TestMoleculeTwo()
        {
            string filename = "NCDK.Data.HIN.molecule2.hin";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            HINReader reader = new HINReader(ins);
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
            Assert.AreEqual(37, m.Atoms.Count);
            // AreEqual(?, m.Bonds.Count);
        }

        [TestMethod()]
        public void TestMultiple()
        {
            string filename = "NCDK.Data.HIN.multiple.hin";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            HINReader reader = new HINReader(ins);
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
            Assert.AreEqual(3, som.Count);
        }

        [TestMethod()]
        public void TestIsConnectedFromHINFile()
        {
            string filename = "NCDK.Data.HIN.connectivity1.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            IChemFile content = reader.Read(new ChemFile());
            reader.Close();
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = cList[0];
            Assert.AreEqual(57, ac.Atoms.Count);
            Assert.AreEqual(59, ac.Bonds.Count);
        }

        // @cdk.bug 2984581
        [TestMethod()]
        public void TestAromaticRingsLine()
        {
            string filename = "NCDK.Data.HIN.bug2984581.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            IChemFile content = reader.Read(new ChemFile());
            reader.Close();
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            Assert.AreEqual(1, cList.Count);
        }

        // @cdk.bug 2984581
        [TestMethod()]
        public void TestReadAromaticRingsKeyword()
        {
            string filename = "NCDK.Data.HIN.arorings.hin";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new HINReader(ins);
            IChemFile content = reader.Read(new ChemFile());
            reader.Close();
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            Assert.AreEqual(1, cList.Count);

            IAtomContainer mol = cList[0];
            Assert.IsTrue(mol.Atoms[0].IsAromatic);
            Assert.IsTrue(mol.Atoms[2].IsAromatic);
            Assert.IsTrue(mol.Atoms[3].IsAromatic);
            Assert.IsTrue(mol.Atoms[5].IsAromatic);
            Assert.IsTrue(mol.Atoms[4].IsAromatic);
            Assert.IsTrue(mol.Atoms[1].IsAromatic);

            Assert.IsTrue(mol.Atoms[7].IsAromatic);
            Assert.IsTrue(mol.Atoms[12].IsAromatic);
            Assert.IsTrue(mol.Atoms[11].IsAromatic);
            Assert.IsTrue(mol.Atoms[10].IsAromatic);
            Assert.IsTrue(mol.Atoms[9].IsAromatic);
            Assert.IsTrue(mol.Atoms[8].IsAromatic);

            // make sure that only the phenyl C's were marked as aromatic
            foreach (var atom in mol.Atoms)
            {
                if (atom.Symbol.Equals("C"))
                    Assert.IsTrue(atom.IsAromatic,
                        $"{atom.Symbol} (index {mol.Atoms.IndexOf(atom)}) was wrongly marked as aromatic");
            }
        }
    }
}
