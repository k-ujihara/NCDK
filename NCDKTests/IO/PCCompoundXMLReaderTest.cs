/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
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
    // @cdk.module test-io
    [TestClass()]
    public class PCCompoundXMLReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.ASN.PubChem.cid1145.xml";
        protected override Type ChemObjectIOToTestType => typeof(PCCompoundXMLReader);

        [TestMethod()]
        public void TestAccepts()
        {
            PCCompoundXMLReader reader = new PCCompoundXMLReader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeof(AtomContainer)));
        }

        [TestMethod()]
        public void TestReading()
        {
            string filename = "NCDK.Data.ASN.PubChem.cid1145.xml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            PCCompoundXMLReader reader = new PCCompoundXMLReader(ins);
            IAtomContainer molecule = (IAtomContainer)reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsNotNull(molecule);

            // check atom stuff
            Assert.AreEqual(14, molecule.Atoms.Count);
            Assert.AreEqual("O", molecule.Atoms[0].Symbol);
            Assert.AreEqual(-1, molecule.Atoms[0].FormalCharge);
            Assert.AreEqual("N", molecule.Atoms[1].Symbol);
            Assert.AreEqual(1, molecule.Atoms[1].FormalCharge);

            // check bond stuff
            Assert.AreEqual(13, molecule.Bonds.Count);
            Assert.IsNotNull(molecule.Bonds[3]);

            // coordinates
            Assert.IsNull(molecule.Atoms[0].Point3D);
            var npoint = molecule.Atoms[0].Point2D;
            Assert.IsNotNull(npoint);
            var point = npoint.Value;
            Assert.AreEqual(3.7320508956909, point.X, 0.00000001);
            Assert.AreEqual(0.5, point.Y, 0.00000001);
        }

        [TestMethod()]
        public void TestReading3DCoords()
        {
            string filename = "NCDK.Data.ASN.PubChem.cid176.xml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            PCCompoundXMLReader reader = new PCCompoundXMLReader(ins);
            IAtomContainer molecule = (IAtomContainer)reader.Read(new AtomContainer());
            reader.Close();
            Assert.IsNotNull(molecule);

            // check atom stuff
            Assert.AreEqual(8, molecule.Atoms.Count);
            Assert.IsNull(molecule.Atoms[0].Point2D);
            var npoint = molecule.Atoms[0].Point3D;
            Assert.IsNotNull(npoint);
            var point = npoint.Value;
            Assert.AreEqual(-0.9598, point.X, 0.0001);
            Assert.AreEqual(1.5616, point.Y, 0.0001);
            Assert.AreEqual(1.8714, point.Z, 0.0001);
        }
    }
}
