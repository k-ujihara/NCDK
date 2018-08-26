/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Silent;
using System;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading Ghemical molecular dynamics files using one test file.
    /// </summary>
    /// <seealso cref="GhemicalMMReader"/>
    // @cdk.module test-io
    [TestClass()]
    public class GhemicalReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.Ghemical.ethene.mm1gp";
        protected override Type ChemObjectIOToTestType => typeof(GhemicalMMReader);

        [TestMethod()]
        public void TestAccepts()
        {
            Assert.IsTrue(ChemObjectIOToTest.Accepts(typeof(ChemModel)));
        }

        [TestMethod()]
        public void TestExample()
        {
            string testfile = "!Header mm1gp 100\n" + "!Info 1\n" + "!Atoms 6\n" + "0 6 \n" + "1 6 \n" + "2 1 \n"
                    + "3 1 \n" + "4 1 \n" + "5 1 \n" + "!Bonds 5\n" + "1 0 D \n" + "2 0 S \n" + "3 0 S \n" + "4 1 S \n"
                    + "5 1 S \n" + "!Coord\n" + "0 0.06677 -0.00197151 4.968e-07 \n"
                    + "1 -0.0667699 0.00197154 -5.19252e-07 \n" + "2 0.118917 -0.097636 2.03406e-06 \n"
                    + "3 0.124471 0.0904495 -4.84021e-07 \n" + "4 -0.118917 0.0976359 -2.04017e-06 \n"
                    + "5 -0.124471 -0.0904493 5.12591e-07 \n" + "!Charges\n" + "0 -0.2\n" + "1 -0.2\n" + "2 0.1\n"
                    + "3 0.1\n" + "4 0.1\n" + "5 0.1\n" + "!End";
            StringReader stringReader = new StringReader(testfile);
            GhemicalMMReader reader = new GhemicalMMReader(stringReader);
            ChemModel model = (ChemModel)reader.Read((ChemObject)new ChemModel());
            reader.Close();

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.MoleculeSet);
            var som = model.MoleculeSet;
            Assert.IsNotNull(som);
            Assert.AreEqual(1, som.Count);
            IAtomContainer m = som[0];
            Assert.IsNotNull(m);
            Assert.AreEqual(6, m.Atoms.Count);
            Assert.AreEqual(5, m.Bonds.Count);

            // test reading of formal charges
            IAtom a = m.Atoms[0];
            Assert.IsNotNull(a);
            Assert.AreEqual(6, a.AtomicNumber.Value);
            Assert.AreEqual(-0.2, a.Charge.Value, 0.01);
            Assert.AreEqual(0.06677, a.Point3D.Value.X, 0.01);
        }

        [TestMethod()]
        public void TestEthene()
        {
            string filename = "NCDK.Data.Ghemical.ethene.mm1gp";
            var ins = ResourceLoader.GetAsStream(filename);
            GhemicalMMReader reader = new GhemicalMMReader(ins);
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
            Assert.AreEqual(6, m.Atoms.Count);
            Assert.AreEqual(5, m.Bonds.Count);
        }
    }
}
