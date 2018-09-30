/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Silent;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading MDL RXN files using one test file.
    /// </summary>
    /// <seealso cref="MDLRXNReader"/>
    // @cdk.module test-io
    [TestClass()]
    public class MDLRXNV2000ReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.MDL.0024.stg02.rxn";
        protected override Type ChemObjectIOToTestType => typeof(MDLRXNV2000Reader);

        [TestMethod()]
        public void TestAccepts()
        {
            MDLRXNV2000Reader reader = new MDLRXNV2000Reader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
            Assert.IsTrue(reader.Accepts(typeof(ChemModel)));
            Assert.IsTrue(reader.Accepts(typeof(Reaction)));
        }

        // @cdk.bug 1849923
        [TestMethod()]
        public void TestReadReactions1()
        {
            string filename1 = "NCDK.Data.MDL.0024.stg02.rxn";
            Trace.TraceInformation("Testing: " + filename1);
            var ins1 = ResourceLoader.GetAsStream(filename1);
            MDLRXNV2000Reader reader1 = new MDLRXNV2000Reader(ins1, ChemObjectReaderMode.Strict);
            IReaction reaction1 = new Reaction();
            reaction1 = (IReaction)reader1.Read(reaction1);
            reader1.Close();

            Assert.IsNotNull(reaction1);
            Assert.AreEqual(1, reaction1.Reactants.Count);
            Assert.AreEqual(1, reaction1.Products.Count);
            IAtomContainer reactant = reaction1.Reactants[0];
            Assert.IsNotNull(reactant);
            Assert.AreEqual(46, reactant.Atoms.Count);
            Assert.AreEqual(44, reactant.Bonds.Count);
            IAtomContainer product = reaction1.Products[0];
            Assert.IsNotNull(product);
            Assert.AreEqual(46, product.Atoms.Count);
            Assert.AreEqual(43, product.Bonds.Count);

        }

        // @cdk.bug 1851202
        [TestMethod()]
        public void TestBug1851202()
        {
            string filename1 = "NCDK.Data.MDL.0002.stg01.rxn";
            Trace.TraceInformation("Testing: " + filename1);
            var ins1 = ResourceLoader.GetAsStream(filename1);
            MDLRXNV2000Reader reader1 = new MDLRXNV2000Reader(ins1, ChemObjectReaderMode.Strict);
            IReaction reaction1 = new Reaction();
            reaction1 = (IReaction)reader1.Read(reaction1);
            reader1.Close();

            Assert.IsNotNull(reaction1);
            Assert.AreEqual(1, reaction1.Reactants.Count);
            Assert.AreEqual(1, reaction1.Products.Count);
            IAtomContainer reactant = reaction1.Reactants[0];
            Assert.IsNotNull(reactant);
            Assert.AreEqual(30, reactant.Atoms.Count);
            Assert.AreEqual(25, reactant.Bonds.Count);
            IAtomContainer product = reaction1.Products[0];
            Assert.IsNotNull(product);
            Assert.AreEqual(30, product.Atoms.Count);
            Assert.AreEqual(26, product.Bonds.Count);

        }

        [TestMethod()]
        public void TestReadMapping()
        {
            string filename2 = "NCDK.Data.MDL.mappingTest.rxn";
            Trace.TraceInformation("Testing: " + filename2);
            var ins2 = ResourceLoader.GetAsStream(filename2);
            MDLRXNV2000Reader reader2 = new MDLRXNV2000Reader(ins2);
            IReaction reaction2 = new Reaction();
            reaction2 = (IReaction)reader2.Read(reaction2);
            reader2.Close();

            Assert.IsNotNull(reaction2);
            var maps = reaction2.Mappings.GetEnumerator();
            Assert.IsTrue(maps.MoveNext());
        }

        [TestMethod()]
        public void TestAgentParts()
        {
            using (var ins = ResourceLoader.GetAsStream(this.GetType(), ("ethylesterification.mol")))
            {
                MDLRXNV2000Reader rdr = new MDLRXNV2000Reader(ins);
                IReaction reaction = rdr.Read(new Reaction());
                Assert.AreEqual(1, reaction.Agents.Count);
            }
        }
    }
}
