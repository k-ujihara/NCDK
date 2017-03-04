/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System.Diagnostics;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading MDL RXN files using one test file.
    /// </summary>
    /// <seealso cref="MDLRXNReader"/>
    // @cdk.module test-io
    [TestClass()]
    public class MDLRXNV3000ReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.MDL.reaction_v3.rxn";
        static readonly MDLRXNV3000Reader simpleReader = new MDLRXNV3000Reader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;
        
        [TestMethod()]
        public void TestAccepts()
        {
            MDLRXNV3000Reader reader = new MDLRXNV3000Reader();
            Assert.IsTrue(reader.Accepts(typeof(ChemModel)));
            Assert.IsTrue(reader.Accepts(typeof(Reaction)));
        }

        /// <summary>
        // @cdk.bug 1849925
        /// </summary>
        [TestMethod()]
        public void TestReadReactions1()
        {
            string filename1 = "NCDK.Data.MDL.reaction_v3.rxn";
            Trace.TraceInformation("Testing: " + filename1);
            var ins1 = ResourceLoader.GetAsStream(filename1);
            MDLRXNV3000Reader reader1 = new MDLRXNV3000Reader(ins1, ChemObjectReaderModes.Strict);
            IReaction reaction1 = new Reaction();
            reaction1 = (IReaction)reader1.Read(reaction1);
            reader1.Close();

            Assert.IsNotNull(reaction1);
            Assert.AreEqual(1, reaction1.Reactants.Count);
            Assert.AreEqual(1, reaction1.Products.Count);
            IAtomContainer reactant = reaction1.Reactants[0];
            Assert.IsNotNull(reactant);
            Assert.AreEqual(32, reactant.Atoms.Count);
            Assert.AreEqual(29, reactant.Bonds.Count);
            IAtomContainer product = reaction1.Products[0];
            Assert.IsNotNull(product);
            Assert.AreEqual(32, product.Atoms.Count);
            Assert.AreEqual(29, product.Bonds.Count);
        }
    }
}
