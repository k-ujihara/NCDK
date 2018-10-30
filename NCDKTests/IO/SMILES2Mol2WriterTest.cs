/*
 * Copyright (c) 2013. John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the writer MOL2 writer from smiles.
    /// </summary>
    /// <seealso cref="Mol2Writer"/>
    // @cdk.module test-smiles
    [TestClass()]
    public class SMILES2Mol2WriterTest
    {
        // @cdk.bug 2675188
        [TestMethod()]
        public void TestWriter1()
        {
            var sp = CDK.SilentSmilesParser;
            var molecule = sp.ParseSmiles("C([H])([H])([H])([H])");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            StringWriter swriter = new StringWriter();
            Mol2Writer writer = new Mol2Writer(swriter);
            writer.Write(molecule);
            writer.Close();
            Assert.IsTrue(swriter.ToString().IndexOf("1 C1 0.000 0.000 0.000 C.3") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("1 1 2 1") > 0);
        }

        [TestMethod()]
        public void TestWriter2()
        {
            var sp = CDK.SilentSmilesParser;
            var molecule = sp.ParseSmiles("c1ccccc1C=O");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);

            StringWriter swriter = new StringWriter();
            Mol2Writer writer = new Mol2Writer(swriter);
            writer.Write(molecule);
            writer.Close();

            Assert.IsTrue(swriter.ToString().IndexOf("1 C1 0.000 0.000 0.000 C.ar") > 0, "Aromatic atom not properly reported");
            Assert.IsTrue(swriter.ToString().IndexOf("8 O8 0.000 0.000 0.000 O.2") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("7 C7 0.000 0.000 0.000 C.2") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("1 1 2 ar") > 0, "Aromatic bond not properly reported");
            Assert.IsTrue(swriter.ToString().IndexOf("8 7 8 2") > 0);
        }

        [TestMethod()]
        public void TestWriterForAmide()
        {
            var sp = CDK.SilentSmilesParser;
            var molecule = sp.ParseSmiles("CC(=O)NC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            StringWriter swriter = new StringWriter();
            Mol2Writer writer = new Mol2Writer(swriter);
            writer.Write(molecule);
            writer.Close();

            Assert.IsTrue(swriter.ToString().IndexOf("1 C1 0.000 0.000 0.000 C.3") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("3 O3 0.000 0.000 0.000 O.") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("4 N4 0.000 0.000 0.000 N.a") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("1 1 2 1") > 0);
            Assert.IsTrue(swriter.ToString().IndexOf("3 2 4 am") > 0, "Amide bond not properly reported");
            Assert.IsTrue(swriter.ToString().IndexOf("4 4 5 1") > 0);
        }
    }
}

