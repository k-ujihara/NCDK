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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Geometries;
using NCDK.IO.Formats;
using NCDK.IO.Listener;
using NCDK.IO.Setting;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO.Iterator
{
    /// <summary>
    /// TestCase for the reading MDL mol files using one test file.
    /// </summary>
    /// <seealso cref="MDLReader"/>
    // @cdk.module test-io
    [TestClass()]
    public class EnumerableSDFReaderTest : CDKTestCase
    {
        [TestMethod()]
        public void TestSDF()
        {
            string filename = "NCDK.Data.MDL.test2.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            EnumerableSDFReader reader = new EnumerableSDFReader(ins, Silent.ChemObjectBuilder.Instance);

            int molCount = 0;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;
                Assert.AreEqual(MDLV2000Format.Instance, reader.Format, "Molecule # was not in MDL V2000 format: " + molCount);
            }

            Assert.AreEqual(6, molCount);
            reader.Close();
        }

        [TestMethod()]
        public void TestSDF_broken_stream()
        {
            string filename = "NCDK.Data.MDL.test2.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var streamReader = new StreamReader(ins); // { public bool Ready()  {    return false;     }   };

            EnumerableSDFReader reader = new EnumerableSDFReader(streamReader, Silent.ChemObjectBuilder.Instance);

            int molCount = 0;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;
                Assert.AreEqual(MDLV2000Format.Instance, reader.Format, "Molecule # was not in MDL V2000 format: " + molCount);
            }

            Assert.AreEqual(6, molCount);
            reader.Close();
        }

        [TestMethod()]
        public void TestReadTitle()
        {
            string filename = "NCDK.Data.MDL.test.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            EnumerableSDFReader reader = new EnumerableSDFReader(ins, Silent.ChemObjectBuilder.Instance);
            var etor = reader.GetEnumerator();
            Assert.IsTrue(etor.MoveNext());
            object obj = etor.Current;
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj is IAtomContainer);
            Assert.AreEqual("2-methylbenzo-1,4-quinone", ((IAtomContainer)obj).Title);
            Assert.AreEqual(MDLV2000Format.Instance, reader.Format);
            reader.Close();
        }

        [TestMethod()]
        public void TestReadDataItems()
        {
            string filename = "NCDK.Data.MDL.test.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            EnumerableSDFReader reader = new EnumerableSDFReader(ins, Silent.ChemObjectBuilder.Instance);

            var etor = reader.GetEnumerator();
            Assert.IsTrue(etor.MoveNext());
            object obj = etor.Current;
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj is IAtomContainer);
            IAtomContainer m = (IAtomContainer)obj;
            Assert.AreEqual("1", m.GetProperty<string>("E_NSC"));
            Assert.AreEqual("553-97-9", m.GetProperty<string>("E_CAS"));
            reader.Close();
        }

        [TestMethod()]
        public void TestMultipleEntryFields()
        {
            string filename = "NCDK.Data.MDL.test.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            using (var reader = new EnumerableSDFReader(ins, Silent.ChemObjectBuilder.Instance))
            {
                var tor = reader.GetEnumerator();
                tor.MoveNext();
                IAtomContainer m = (IAtomContainer)tor.Current;
                Assert.AreEqual("553-97-9", m.GetProperty<string>("E_CAS"));
                tor.MoveNext();
                m = tor.Current;
                Assert.AreEqual("120-78-5", m.GetProperty<string>("E_CAS"));
            }
        }

        [TestMethod()]
        public void TestOnMDLMolfile()
        {
            string filename = "NCDK.Data.MDL.bug682233.mol";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            EnumerableSDFReader reader = new EnumerableSDFReader(ins, Silent.ChemObjectBuilder.Instance);

            int molCount = 0;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;
            }

            Assert.AreEqual(1, molCount);
            reader.Close();
        }

        [TestMethod()]
        public void TestOnSingleEntrySDFile()
        {
            string filename = "NCDK.Data.MDL.singleMol.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            EnumerableSDFReader reader = new EnumerableSDFReader(ins, Silent.ChemObjectBuilder.Instance);

            int molCount = 0;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;
            }

            Assert.AreEqual(1, molCount);
            reader.Close();
        }

        [TestMethod()]
        public void TestEmptyEntryIteratingReader()
        {
            string filename = "NCDK.Data.MDL.emptyStructures.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            EnumerableSDFReader reader = new EnumerableSDFReader(ins, Silent.ChemObjectBuilder.Instance);
            int molCount = 0;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;

                if (molCount == 2)
                {
                    IAtomContainer mol = (IAtomContainer)obj;
                    string s = mol.GetProperty<string>("Species");
                    Assert.AreEqual("rat", s);
                }
            }

            Assert.AreEqual(2, molCount);
            reader.Close();
        }

        // @cdk.bug 2692107
        [TestMethod()]
        public void TestZeroZCoordinates()
        {
            string filename = "NCDK.Data.MDL.nozcoord.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var prop = new NameValueCollection
            {
                ["ForceReadAs3DCoordinates"] = "true"
            };
            PropertiesListener listener = new PropertiesListener(prop);
            EnumerableSDFReader reader = new EnumerableSDFReader(ins, Silent.ChemObjectBuilder.Instance);
            reader.Listeners.Add(listener);
            reader.CustomizeJob();
            int molCount = 0;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;
                bool has3d = GeometryUtil.Has3DCoordinates((IAtomContainer)obj);
                Assert.IsTrue(has3d);
            }
            Assert.AreNotSame(0, molCount);
            reader.Close();
        }

        [TestMethod()]
        public void TestNo3DCoordsButForcedAs()
        {
            // First test unforced 3D coordinates
            string filename = "NCDK.Data.MDL.no3dStructures.sdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            EnumerableSDFReader reader = new EnumerableSDFReader(ins, Silent.ChemObjectBuilder.Instance);
            int molCount = 0;
            IAtomContainer mol = null;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;
                mol = (IAtomContainer)obj;
            }

            Assert.AreEqual(2, molCount);
            Assert.IsNotNull(mol.Atoms[0].Point2D);
            Assert.IsNull(mol.Atoms[0].Point3D);
            reader.Close();

            // Now test forced 3D coordinates
            Trace.TraceInformation("Testing: " + filename);
            ins = ResourceLoader.GetAsStream(filename);
            reader = new EnumerableSDFReader(ins, Silent.ChemObjectBuilder.Instance);
            reader.Listeners.Add(new MyListener());
            reader.CustomizeJob();
            molCount = 0;
            mol = null;
            foreach (var obj in reader)
            {
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                molCount++;
                mol = (IAtomContainer)obj;
            }

            Assert.AreEqual(2, molCount);
            Assert.IsNull(mol.Atoms[0].Point2D);
            Assert.IsNotNull(mol.Atoms[0].Point3D);
            reader.Close();
        }

        class MyListener : IChemObjectIOListener
        {
            public void ProcessIOSettingQuestion(IOSetting setting)
            {
                if (string.Equals("ForceReadAs3DCoordinates", setting.Name, StringComparison.Ordinal))
                {
                    try
                    {
                        setting.Setting = "true";
                    }
                    catch (CDKException e)
                    {
                        Trace.TraceError($"Could not set forceReadAs3DCoords setting: {e.Message}");
                        Debug.WriteLine(e);
                    }
                }
            }
        }

        // @cdk.bug 3488307
        [TestMethod()]
        public void TestBrokenSDF()
        {
            string path = "NCDK.Data.MDL.bug3488307.sdf";
            var ins = ResourceLoader.GetAsStream(path);
            var builder = Silent.ChemObjectBuilder.Instance;
            EnumerableSDFReader reader = new EnumerableSDFReader(ins, builder)
            {
                Skip = true // skip over null entries and keep reading until EOF
            };

            int count = 0;

            foreach (var m in reader)
            {
                count++;
            }

            reader.Close();

            Assert.AreEqual(3, count);
        }

        [TestMethod()]
        public void TestV3000MolfileFormat()
        {
            string path = "NCDK.Data.MDL.molV3000.mol";
            var ins = ResourceLoader.GetAsStream(path);
            var builder = Silent.ChemObjectBuilder.Instance;
            EnumerableSDFReader reader = new EnumerableSDFReader(ins, builder)
            {
                Skip = true // skip over null entries and keep reading until EOF
            };

            int count = 0;

            foreach (var m in reader)
            {
                count++;
            }

            reader.Close();

            Assert.AreEqual(1, count);
        }
    }
}
