/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Geometries;
using NCDK.Numerics;
using System.Diagnostics;

namespace NCDK.IO.CML
{
    /// <summary>
    /// TestCase for reading CML files using a few test files
    /// in data/cmltest as found in the Jmol distribution
    /// (<see href="http://www.jmol.org/">http://www.jmol.org/</see>).
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public class JmolTest : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = CDK.Builder;

        /// <summary>
        /// Now come the actual tests...
        /// </summary>

        /// <summary>
        /// Special CML characteristics:
        /// <ul><item> &lt;crystal&gt;</item></ul>
        /// </summary>
        [TestMethod()]
        public void TestEstron()
        {
            var filename = "NCDK.Data.CML.estron.cml";
            Trace.TraceInformation("Testing: " + filename);
            IChemFile chemFile;
            using (var ins = ResourceLoader.GetAsStream(filename))
            using (var reader = new CMLReader(ins))
            {
                chemFile = reader.Read(builder.NewChemFile());
            }

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            var model = seq[0];
            Assert.IsNotNull(model);

            // test the molecule
            ICrystal crystal = model.Crystal;
            Assert.IsNotNull(crystal);
            Assert.AreEqual(4 * 42, crystal.Atoms.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(crystal));
            // test the cell axes
            Vector3 a = crystal.A;
            Assert.IsTrue(a.X != 0.0);
            Vector3 b = crystal.B;
            Assert.IsTrue(b.Y != 0.0);
            Vector3 c = crystal.C;
            Assert.IsTrue(c.Z != 0.0);
        }

        /// <summary>
        /// Special CML characteristics:
        /// - Jmol Animation
        /// </summary>
        [TestMethod(), Ignore()] // It is broken, but not used, AFAIK
        public void TestAnimation()
        {
            var filename = "NCDK.Data.CML.SN1_reaction.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new CMLReader(ins);
            var chemFile = reader.Read(builder.NewChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(34, seq.Count);
            var model = seq[0];
            Assert.IsNotNull(model);
            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);

            // test the molecule
            IAtomContainer mol = som[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(mol.Atoms.Count, 25);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
        }

        /// <summary>
        /// No special CML code, just regression test for Jmol releases
        /// </summary>
        [TestMethod()]
        public void TestMethanolTwo()
        {
            var filename = "NCDK.Data.CML.methanol2.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new CMLReader(ins);
            var chemFile = reader.Read(builder.NewChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            //Debug.WriteLine($"NO sequences: {chemFile.Count}");
            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            //Debug.WriteLine($"NO models: {seq.Count}");
            var model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.MoleculeSet.Count);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(mol.Atoms.Count, 6);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
        }

        /// <summary>
        /// No special CML code, just regression test for Jmol releases
        /// </summary>
        [TestMethod()]
        public void TestMethanolOne()
        {
            var filename = "NCDK.Data.CML.methanol1.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new CMLReader(ins);
            var chemFile = reader.Read(builder.NewChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            //Debug.WriteLine($"NO sequences: {chemFile.Count}");
            var seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            //Debug.WriteLine($"NO models: {seq.Count}");
            var model = seq[0];
            Assert.IsNotNull(model);
            var som = model.MoleculeSet;
            Assert.AreEqual(1, som.Count);

            // test the molecule
            IAtomContainer mol = som[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(mol.Atoms.Count, 6);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
        }
    }
}
