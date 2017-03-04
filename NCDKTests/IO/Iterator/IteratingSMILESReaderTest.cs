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
using NCDK.IO.Formats;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO.Iterator
{
    /// <summary>
    /// TestCase for the reading SMILES mol files using one test file.
    ///
    // @cdk.module test-smiles
    // @see org.openscience.cdk.io.SMILESReader
    /// </summary>
    [TestClass()]
    public class IteratingSMILESReaderTest : CDKTestCase
    {
        [TestMethod()]
        public void TestSMILESFileWithNames()
        {
            string filename = "NCDK.Data.Smiles.test.smi";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            IteratingSMILESReader reader = new IteratingSMILESReader(ins, Default.ChemObjectBuilder.Instance);

            int molCount = 0;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;
            }

            Assert.AreEqual(5, molCount);

            reader.Close();
        }

        [TestMethod()]
        public void TestSMILESFileWithSpacesAndTabs()
        {
            string filename = "NCDK.Data.Smiles.tabs.smi";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            IteratingSMILESReader reader = new IteratingSMILESReader(ins, Default.ChemObjectBuilder.Instance);

            int molCount = 0;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;
            }

            Assert.AreEqual(5, molCount);

            reader.Close();
        }

        [TestMethod()]
        public void TestSMILESTitles()
        {
            string filename = "NCDK.Data.Smiles.tabs.smi";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            IteratingSMILESReader reader = new IteratingSMILESReader(ins, Default.ChemObjectBuilder.Instance);
            foreach (var mol in reader)
            {
                string title = (string)mol.GetProperty<string>(CDKPropertyName.TITLE);
                Assert.IsNotNull(title);
            }

        }

        [TestMethod()]
        public void TestSMILESFile()
        {
            string filename = "NCDK.Data.Smiles.test2.smi";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            IteratingSMILESReader reader = new IteratingSMILESReader(ins, Default.ChemObjectBuilder.Instance);

            int molCount = 0;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;
            }

            Assert.AreEqual(5, molCount);
        }

        [TestMethod()]
        public void TestGetFormat()
        {
            string filename = "NCDK.Data.Smiles.test2.smi";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            IteratingSMILESReader reader = new IteratingSMILESReader(ins, Default.ChemObjectBuilder.Instance);
            IResourceFormat format = reader.Format;
            Assert.IsTrue(format is SMILESFormat);
        }

        [TestMethod()]
        public void TestSetReader1()
        {
            string filename = "NCDK.Data.Smiles.test2.smi";
            var ins1 = ResourceLoader.GetAsStream(filename);
            IteratingSMILESReader reader = new IteratingSMILESReader(ins1, Default.ChemObjectBuilder.Instance);
            int molCount = 0;
            foreach (var mol in reader)
            {
                molCount++;
            }
            filename = "NCDK.Data.Smiles.tabs.smi";
            var ins2 = ResourceLoader.GetAsStream(filename);
            reader.SetReader(ins2);
            molCount = 0;
            foreach (var mol in reader)
            {
                molCount++;
            }
            Assert.AreEqual(5, molCount);
        }

        [TestMethod()]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRemove()
        {
            string filename = "NCDK.Data.Smiles.test2.smi";
            var ins1 = ResourceLoader.GetAsStream(filename);
            IteratingSMILESReader reader = new IteratingSMILESReader(ins1, Default.ChemObjectBuilder.Instance);
            int molCount = 0;
            foreach (var mol in reader)
            {
                molCount++;
                if (molCount > 2) break;
            }
            reader.Remove();
        }

        [TestMethod()]
        public void IsEmptyTest()
        {
            TextReader reader = new StringReader(" empty1\n empty2");
            var smis = new IteratingSMILESReader(reader, Silent.ChemObjectBuilder.Instance).GetEnumerator();
            Assert.IsTrue(smis.MoveNext());
            IAtomContainer m1 = smis.Current;
            Assert.AreEqual(0, m1.Atoms.Count);
            Assert.AreEqual("empty1", m1.GetProperty<string>(CDKPropertyName.TITLE));
            Assert.IsTrue(smis.MoveNext());
            IAtomContainer m2 = smis.Current;
            Assert.AreEqual(0, m2.Atoms.Count);
            Assert.AreEqual("empty2", m2.GetProperty<string>(CDKPropertyName.TITLE));
            Assert.IsFalse(smis.MoveNext());
        }

        [TestMethod()]
        public void ProblemSmiles()
        {

            TextReader reader = new StringReader(" okay\nn1cccc1 bad\n okay");
            var smis = new IteratingSMILESReader(reader, Silent.ChemObjectBuilder.Instance).GetEnumerator();
            Assert.IsTrue(smis.MoveNext());
            IAtomContainer m1 = smis.Current;
            Assert.AreEqual(0, m1.Atoms.Count);
            Assert.AreEqual("okay", m1.GetProperty<string>(CDKPropertyName.TITLE));
            Assert.IsTrue(smis.MoveNext());
            IAtomContainer m2 = smis.Current;
            Assert.AreEqual(0, m2.Atoms.Count);
            Assert.AreEqual("bad", m2.GetProperty<string>(CDKPropertyName.TITLE));
            Assert.AreEqual("n1cccc1 bad", m2.GetProperty<string>(IteratingSMILESReader.BAD_SMILES_INPUT));
            smis.MoveNext();
            IAtomContainer m3 = smis.Current;
            Assert.AreEqual(0, m3.Atoms.Count);
            Assert.AreEqual("okay", m3.GetProperty<string>(CDKPropertyName.TITLE));
            Assert.IsFalse(smis.MoveNext());
        }
    }
}
