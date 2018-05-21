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
using NCDK.Default;
using NCDK.Geometries;
using System.Diagnostics;

namespace NCDK.IO.CML
{
    /// <summary>
    /// TestCase for reading CML files using a few test files
    /// in data/cmltest as found in the JChemPaint distribution
    /// (http://jchempaint.sf.org/).
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public class JChemPaintTest : CDKTestCase
    {
        /// <summary>
        /// This one tests a CML2 file.
        /// </summary>
        [TestMethod()]
        public void TestSalt()
        {
            string filename = "NCDK.Data.CML.COONa.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            //Debug.WriteLine("NO sequences: " + chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            //Debug.WriteLine("NO models: " + seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.MoleculeSet.Count);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(4, mol.Atoms.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
        }

        /// <summary>
        /// This one tests reading of output from the WWMM matrix (KEGG collection).
        /// </summary>
        [TestMethod()]
        public void TestWWMMOutput()
        {
            string filename = "NCDK.Data.CML.keggtest.cml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            var moleculeSet = model.MoleculeSet;
            Assert.IsNotNull(moleculeSet);
            Assert.AreEqual(1, moleculeSet.Count);

            // test the molecule
            IAtomContainer mol = moleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(mol));
        }
    }
}
