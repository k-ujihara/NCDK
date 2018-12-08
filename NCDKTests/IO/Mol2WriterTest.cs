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
using Moq;
using NCDK.Aromaticities;
using NCDK.Silent;
using NCDK.Tools.Manipulator;
using System;
using System.IO;
using System.Linq;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the writer MOL2 writer.
    /// </summary>
    /// <seealso cref="Mol2Writer"/>
    /// <seealso cref="SMILES2Mol2WriterTest"/>
    /// <seealso cref="Mol2Writer"/>
    [TestClass()]
    public class Mol2WriterTest : ChemObjectIOTest
    {
        protected override Type ChemObjectIOToTestType => typeof(Mol2Writer);
        private static IChemObjectBuilder builder = ChemObjectBuilder.Instance;

        [TestMethod()]
        public void TestAccepts()
        {
            Mol2Writer writer = new Mol2Writer(new StringWriter());
            Assert.IsTrue(writer.Accepts(typeof(AtomContainer)));
        }

        // @cdk.bug 2675188
        [TestMethod(), Ignore()] // moved to SMILES2Mol2WriterTest
        public void TestWriter1()
        {
            IAtomContainer molecule = new Mock<IAtomContainer>().Object;

            StringWriter swriter = new StringWriter();
            Mol2Writer writer = new Mol2Writer(swriter);
            writer.Write(molecule);
            writer.Close();
            Assert.IsTrue(swriter.ToString().IndexOf("1 C1 0.000 0.000 0.000 C.3") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("1 2 1 1") > 0);
        }

        [TestMethod(), Ignore()] // moved to SMILES2Mol2WriterTest
        public void TestWriter2()
        {
            IAtomContainer molecule = new Mock<IAtomContainer>().Object;
            Aromaticity.CDKLegacy.Apply(molecule);

            StringWriter swriter = new StringWriter();
            Mol2Writer writer = new Mol2Writer(swriter);
            writer.Write(molecule);
            writer.Close();

            Assert.IsTrue(swriter.ToString().IndexOf("1 C1 0.000 0.000 0.000 C.ar") > 0, "Aromatic atom not properly reported");
            Assert.IsTrue(swriter.ToString().IndexOf("8 O8 0.000 0.000 0.000 O.2") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("7 C7 0.000 0.000 0.000 C.2") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("1 2 1 ar") > 0, "Aromatic bond not properly reported");
            Assert.IsTrue(swriter.ToString().IndexOf("8 8 7 2") > 0);
        }

        [TestMethod(), Ignore()] // moved to SMILES2Mol2WriterTest
        public void TestWriterForAmide()
        {
            IAtomContainer molecule = new Mock<IAtomContainer>().Object;
            Aromaticity.CDKLegacy.Apply(molecule);

            StringWriter swriter = new StringWriter();
            Mol2Writer writer = new Mol2Writer(swriter);
            writer.Write(molecule);
            writer.Close();

            Assert.IsTrue(swriter.ToString().IndexOf("1 C1 0.000 0.000 0.000 C.3") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("3 O3 0.000 0.000 0.000 O.") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("4 N4 0.000 0.000 0.000 N.a") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("1 2 1 1") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("3 4 2 am") > 0, "Amide bond not properly reported");
            Assert.IsTrue(swriter.ToString().IndexOf("4 5 4 1") > 0);
        }

        /// <summary>
        /// This test just ensures that Mol2Writer does not throw an NPE.
        ///
        /// It does not test whether the output is correct or not.
        /// </summary>
        // @cdk.bug 3315503
        [TestMethod()]
        public void TestMissingAtomType()
        {
            var filename = "NCDK.Data.MDL.ligand-1a0i.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLV2000Reader(ins);
            IChemFile fileContents = (IChemFile)reader.Read(builder.NewChemFile());
            reader.Close();
            var molecules = ChemFileManipulator.GetAllAtomContainers(fileContents).ToReadOnlyList();
            IAtomContainer mol = molecules[0];
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            Aromaticity.CDKLegacy.Apply(mol);

            StringWriter writer = new StringWriter();
            Mol2Writer molwriter = new Mol2Writer(writer);
            molwriter.Write(mol);
            molwriter.Close();

            string mol2file = writer.ToString();
            Assert.IsTrue(mol2file.Contains("-1.209 -18.043 49.44 X") || mol2file.Contains("-1.209 -18.043 49.440 X"));
        }
    }
}
