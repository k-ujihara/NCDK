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
using System;
using System.Collections.Generic;
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
    public class MDLRXNReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.MDL.reaction-1.rxn";
        protected override Type ChemObjectIOToTestType => typeof(MDLRXNReader);

        [TestMethod()]
        public void TestAccepts()
        {
            MDLRXNReader reader = new MDLRXNReader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
            Assert.IsTrue(reader.Accepts(typeof(ChemModel)));
            Assert.IsTrue(reader.Accepts(typeof(Reaction)));
            Assert.IsTrue(reader.Accepts(typeof(ReactionSet)));
            Assert.IsFalse(reader.Accepts(typeof(AtomContainerSet<IAtomContainer>)));
            Assert.IsFalse(reader.Accepts(typeof(AtomContainer)));
        }

        [TestMethod()]
        public void TestReadReactions1()
        {
            string filename1 = "NCDK.Data.MDL.reaction-1.rxn";
            Trace.TraceInformation("Testing: " + filename1);
            var ins1 = ResourceLoader.GetAsStream(filename1);
            MDLRXNReader reader1 = new MDLRXNReader(ins1);
            IReaction reaction1 = new Reaction();
            reaction1 = (IReaction)reader1.Read(reaction1);
            reader1.Close();

            Assert.IsNotNull(reaction1);
            Assert.AreEqual(2, reaction1.Reactants.Count);
            Assert.AreEqual(1, reaction1.Products.Count);

            var educts = reaction1.Reactants;
            // Check Atom symbols of first educt
            string[] atomSymbolsOfEduct1 = { "C", "C", "O", "Cl" };
            for (int i = 0; i < educts[0].Atoms.Count; i++)
            {
                Assert.AreEqual(atomSymbolsOfEduct1[i], educts[0].Atoms[i].Symbol);
            }

            // Check Atom symbols of second educt
            for (int i = 0; i < educts[1].Atoms.Count; i++)
            {
                Assert.AreEqual("C", educts[1].Atoms[i].Symbol);
            }

            // Check Atom symbols of first product
            var products = reaction1.Products;
            string[] atomSymbolsOfProduct1 = { "C", "C", "C", "C", "C", "C", "C", "O", "C" };
            for (int i = 0; i < products[0].Atoms.Count; i++)
            {
                Assert.AreEqual(atomSymbolsOfProduct1[i], products[0].Atoms[i].Symbol);
            }
        }

        [TestMethod()]
        public void TestReadReactions2()
        {
            string filename2 = "NCDK.Data.MDL.reaction-2.rxn";
            Trace.TraceInformation("Testing: " + filename2);
            var ins2 = ResourceLoader.GetAsStream(filename2);
            MDLRXNReader reader2 = new MDLRXNReader(ins2);
            IReaction reaction2 = new Reaction();
            reaction2 = (IReaction)reader2.Read(reaction2);
            reader2.Close();

            Assert.IsNotNull(reaction2);
            Assert.AreEqual(2, reaction2.Reactants.Count);
            Assert.AreEqual(2, reaction2.Products.Count);
        }

        [TestMethod()]
        public void TestReadMapping()
        {
            string filename2 = "NCDK.Data.MDL.mappingTest.rxn";
            Trace.TraceInformation("Testing: " + filename2);
            var ins2 = ResourceLoader.GetAsStream(filename2);
            MDLRXNReader reader2 = new MDLRXNReader(ins2);
            IReaction reaction2 = new Reaction();
            reaction2 = (IReaction)reader2.Read(reaction2);
            reader2.Close();

            Assert.IsNotNull(reaction2);
            IEnumerator<IMapping> maps = reaction2.Mappings.GetEnumerator();
            Assert.IsTrue(maps.MoveNext());
        }

        [TestMethod()]
        public void TestRDFChemFile()
        {
            string filename = "NCDK.Data.MDL.qsar-reaction-test.rdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLRXNReader reader = new MDLRXNReader(ins);
            IChemFile chemFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(chemFile);

            Assert.AreEqual(2, chemFile[0][0].ReactionSet.Count);
            Assert.AreEqual(2, chemFile[0][0].ReactionSet[0]
                    .Reactants.Count);
            Assert.AreEqual(3, chemFile[0][0].ReactionSet[0]
                    .Reactants[0].Atoms.Count);
            Assert.AreEqual(2, chemFile[0][0].ReactionSet[0]
                    .Reactants[1].Atoms.Count);
            Assert.AreEqual(2, chemFile[0][0].ReactionSet[0]
                    .Products.Count);
            Assert.AreEqual(2, chemFile[0][0].ReactionSet[0]
                    .Products[0].Atoms.Count);
            Assert.AreEqual(2, chemFile[0][0].ReactionSet[0]
                    .Products[1].Atoms.Count);

            Assert.AreEqual(1, chemFile[0][0].ReactionSet[1]
                    .Reactants.Count);
            Assert.AreEqual(3, chemFile[0][0].ReactionSet[1]
                    .Reactants[0].Atoms.Count);
            Assert.AreEqual(1, chemFile[0][0].ReactionSet[1]
                    .Products.Count);
            Assert.AreEqual(2, chemFile[0][0].ReactionSet[1]
                    .Products[0].Atoms.Count);

        }

        [TestMethod()]
        public void TestRDFModel()
        {
            string filename = "NCDK.Data.MDL.qsar-reaction-test.rdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLRXNReader reader = new MDLRXNReader(ins);
            IChemModel chemModel = (IChemModel)reader.Read(new ChemModel());
            reader.Close();
            Assert.IsNotNull(chemModel);

            Assert.AreEqual(2, chemModel.ReactionSet.Count);
            Assert.AreEqual(2, chemModel.ReactionSet[0].Reactants.Count);
            Assert.AreEqual(3, chemModel.ReactionSet[0].Reactants[0]
                    .Atoms.Count);
            Assert.AreEqual(2, chemModel.ReactionSet[0].Reactants[1]
                    .Atoms.Count);
            Assert.AreEqual(2, chemModel.ReactionSet[0].Products.Count);
            Assert.AreEqual(2, chemModel.ReactionSet[0].Products[0]
                    .Atoms.Count);
            Assert.AreEqual(2, chemModel.ReactionSet[0].Products[1]
                    .Atoms.Count);

            Assert.AreEqual(1, chemModel.ReactionSet[1].Reactants.Count);
            Assert.AreEqual(3, chemModel.ReactionSet[1].Reactants[0]
                    .Atoms.Count);
            Assert.AreEqual(1, chemModel.ReactionSet[1].Products.Count);
            Assert.AreEqual(2, chemModel.ReactionSet[1].Products[0]
                    .Atoms.Count);

        }

        [TestMethod()]
        public void TestRDFReactioniSet()
        {
            string filename = "NCDK.Data.MDL.qsar-reaction-test.rdf";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLRXNReader reader = new MDLRXNReader(ins);
            IReactionSet reactionSet = (IReactionSet)reader.Read(new ReactionSet());
            reader.Close();
            Assert.IsNotNull(reactionSet);

            Assert.AreEqual(2, reactionSet.Count);
            Assert.AreEqual(2, reactionSet[0].Reactants.Count);
            Assert.AreEqual(3, reactionSet[0].Reactants[0].Atoms.Count);
            Assert.AreEqual(2, reactionSet[0].Reactants[1].Atoms.Count);
            Assert.AreEqual(2, reactionSet[0].Products.Count);
            Assert.AreEqual(2, reactionSet[0].Products[0].Atoms.Count);
            Assert.AreEqual(2, reactionSet[0].Products[1].Atoms.Count);

            Assert.AreEqual(1, reactionSet[1].Reactants.Count);
            Assert.AreEqual(3, reactionSet[1].Reactants[0].Atoms.Count);
            Assert.AreEqual(1, reactionSet[1].Products.Count);
            Assert.AreEqual(2, reactionSet[1].Products[0].Atoms.Count);
        }

        /// <summary>
        /// This test checks of different numbering for the same mapping gives the same result.
        /// </summary>
        [TestMethod()]
        public void TestAsadExamples()
        {
            string filename = "NCDK.Data.MDL.output.rxn";
            Trace.TraceInformation("Testing: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MDLRXNReader reader = new MDLRXNReader(ins);
            IReactionSet reactionSet = (IReactionSet)reader.Read(new ReactionSet());
            reader.Close();
            filename = "NCDK.Data.MDL.output_Cleaned.rxn";
            Trace.TraceInformation("Testing: " + filename);
            ins = ResourceLoader.GetAsStream(filename);
            reader = new MDLRXNReader(ins);
            IReactionSet reactionSet2 = (IReactionSet)reader.Read(new ReactionSet());
            reader.Close();
            Assert.AreEqual(reactionSet[0].Mappings.Count, reactionSet2[0].Mappings.Count);
            for (int i = 0; i < reactionSet[0].Mappings.Count; i++)
            {
                Assert.AreEqual(
                    _GetAtomNumber(reactionSet, reactionSet[0].Mappings[i][0]),
                    _GetAtomNumber(reactionSet2, reactionSet2[0].Mappings[i][0]));
                Assert.AreEqual(
                    _GetAtomNumber(reactionSet, reactionSet[0].Mappings[i][1]),
                    _GetAtomNumber(reactionSet2, reactionSet2[0].Mappings[i][1]));
            }
        }

        /// <summary>
        /// Tells the position of an atom in a reaction. Format is "reaction/product:numberofreaction/product_atomnumber".
        /// </summary>
        /// <param name="reactionSet">The reactionSet in which to search.</param>
        /// <param name="chemObject">The atom to search for.</param>
        /// <returns>The position in the said format.</returns>
        /// <exception cref="CDKException">Atom not found in reactionSet.</exception>
        private string _GetAtomNumber(IReactionSet reactionSet, IChemObject chemObject)
        {
            for (int i = 0; i < reactionSet[0].Reactants.Count; i++)
            {
                for (int k = 0; k < reactionSet[0].Reactants[i].Atoms.Count; k++)
                {
                    if (reactionSet[0].Reactants[i].Atoms[k] == chemObject)
                        return "reactant:" + i + "_" + k;
                }
            }
            for (int i = 0; i < reactionSet[0].Products.Count; i++)
            {
                for (int k = 0; k < reactionSet[0].Products[i].Atoms.Count; k++)
                {
                    if (reactionSet[0].Products[i].Atoms[k] == chemObject)
                        return "product:" + i + "_" + k;
                }
            }
            throw new CDKException("not found");
        }
    }
}
