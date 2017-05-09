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
using NCDK.IO.Iterator;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading MDL mol files using one test file.
    /// </summary>
    /// <seealso cref="MDLReader"/>
    // @cdk.module test-smiles
    [TestClass()]
    public class SMILESReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.Smiles.smiles.smi";
        static readonly ISimpleChemObjectReader simpleReader = new SMILESReader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        [TestMethod()]
        public void TestAccepts()
        {
            SMILESReader reader = new SMILESReader();
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
            Assert.IsTrue(reader.Accepts(typeof(AtomContainerSet<IAtomContainer>)));
        }

        [TestMethod()]
        public void TestReading()
        {
            string filename = "NCDK.Data.Smiles.smiles.smi";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            SMILESReader reader = new SMILESReader(ins);
            IAtomContainerSet<IAtomContainer> som = reader.Read(new AtomContainerSet<IAtomContainer>());
            Assert.AreEqual(8, som.Count);
        }

        [TestMethod()]
        public void TestReadingSmiFile_1()
        {
            string filename = "NCDK.Data.Smiles.smiles.smi";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            SMILESReader reader = new SMILESReader(ins);
            IAtomContainerSet<IAtomContainer> som = reader.Read(new AtomContainerSet<IAtomContainer>());
            string name = null;
            IAtomContainer thisMol = som[0];
            name = (thisMol.GetProperty<string>("SMIdbNAME")).ToString();
            Assert.AreEqual("benzene", name);
        }

        [TestMethod()]
        public void TestReadingSmiFile_2()
        {
            string filename = "NCDK.Data.Smiles.smiles.smi";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            SMILESReader reader = new SMILESReader(ins);
            IAtomContainerSet<IAtomContainer> som = reader.Read(new AtomContainerSet<IAtomContainer>());
            IAtomContainer thisMol = som[1];
            Assert.IsNull(thisMol.GetProperty<object>("SMIdbNAME"));
        }

        [TestMethod()]
        public void TestReadingSmiFile_3()
        {
            string filename = "NCDK.Data.Smiles.test3.smi";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            SMILESReader reader = new SMILESReader(ins);
            IAtomContainerSet<IAtomContainer> som = reader.Read(new AtomContainerSet<IAtomContainer>());
            Assert.AreEqual(5, som.Count);
        }

        [TestMethod()]
        public void badSmilesLine()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            string input = "C\nn1cccc1\nc1ccccc1\n";
            DefaultChemObjectReader cor = new SMILESReader(new StringReader(input));
            IAtomContainerSet<IAtomContainer> mols = cor.Read(bldr.CreateAtomContainerSet());
            Assert.AreEqual(3, mols.Count);
            Assert.AreEqual(1, mols[0].Atoms.Count);
            Assert.IsNull(mols[0].GetProperty<string>(IteratingSMILESReader.BadSmilesInput));
            Assert.AreEqual(0, mols[1].Atoms.Count);
            Assert.IsNotNull(mols[1].GetProperty<string>(IteratingSMILESReader.BadSmilesInput));
            Assert.AreEqual(6, mols[2].Atoms.Count);
            Assert.IsNull(mols[2].GetProperty<string>(IteratingSMILESReader.BadSmilesInput));
        }
    }
}
