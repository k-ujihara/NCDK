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
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Geometries;
using System.Diagnostics;

namespace NCDK.IO.CML
{
    /// <summary>
    /// TestCase for reading CML files using a few test files
    /// in data/cmltest as found in the original Jumbo3 release
    /// (http://www.xml-cml.org/).
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public class JumboTest : CDKTestCase
    {
        /// <summary>
        /// Special CML characteristics:
        /// <![CDATA[- <atomArray><atom/><atom/></atomArray>]]>
        /// - X2D only
        /// </summary>
        [TestMethod()]
        public void TestCuran()
        {
            string filename = "NCDK.Data.CML.curan.xml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count, 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(mol.Atoms.Count, 24);
            Assert.AreEqual(mol.Bonds.Count, 28);
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(mol));
        }

        /// <summary>
        /// Special CML characteristics:
        /// - use of cml: namespace
        /// - X2D only
        /// </summary>
        [TestMethod()]
        public void TestCephNS()
        {
            string filename = "NCDK.Data.CML.ceph-ns.xml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count, 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(mol.Atoms.Count, 15);
            Assert.AreEqual(mol.Bonds.Count, 16);
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(mol));
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(mol));
        }

        /// <summary>
        /// Special CML characteristics:
        /// - <![CDATA[<atomArray><stringArray builtin="atomId"/></atomArray>]]>
        /// - <![CDATA[<bondArray><stringArray builtin="atomRef"/></atomArray>]]> 
        /// - no coords
        /// </summary>
        [TestMethod()]
        public void TestNucleustest()
        {
            string filename = "NCDK.Data.CML.nucleustest.xml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CMLReader reader = new CMLReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();

            // test the resulting ChemFile content
            Assert.IsNotNull(chemFile);
            Assert.AreEqual(chemFile.Count, 1);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(seq.Count, 1);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);
            Assert.AreEqual(model.MoleculeSet.Count, 1);

            // test the molecule
            IAtomContainer mol = model.MoleculeSet[0];
            Assert.IsNotNull(mol);
            Assert.AreEqual(11, mol.Atoms.Count, "Incorrect number of atoms");
            Assert.AreEqual(12, mol.Bonds.Count, "Incorrect number of bonds");
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(mol), "File does not have 3D coordinates");
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(mol), "File does not have 2D coordinates");
        }
    }
}
