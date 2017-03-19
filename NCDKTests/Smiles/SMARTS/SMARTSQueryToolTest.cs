/* Copyright (C) 2007  Rajarshi Guha <>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Config;
using NCDK.Graphs;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Smiles.SMARTS
{
    /// <summary>
    /// JUnit test routines for the SMARTS substructure search.
    ///
    // @author Rajarshi Guha
    // @cdk.module test-smarts
    // @cdk.require ant1.6
    /// </summary>
    public class SMARTSQueryToolTest : CDKTestCase
    {

        /// <summary>
        /// <exception cref="CDKException"></exception>
        // @cdk.bug 2788357
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLexicalError()
        {
            SMARTSQueryTool sqt = new SMARTSQueryTool("Epoxide", Default.ChemObjectBuilder.Instance);
        }

        [TestMethod()]
        public void TestQueryTool()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C");
            SMARTSQueryTool querytool = new SMARTSQueryTool("O=CO", Default.ChemObjectBuilder.Instance);

            bool status = querytool.Matches(atomContainer);
            Assert.IsTrue(status);

            int nmatch = querytool.MatchesCount;
            Assert.AreEqual(2, nmatch);

            List<int> map1 = new List<int>();
            map1.Add(1);
            map1.Add(2);
            map1.Add(3);

            List<int> map2 = new List<int>();
            map2.Add(3);
            map2.Add(4);
            map2.Add(5);

            var mappings = querytool.GetMatchingAtoms();
            var ret1 = mappings[0].OrderBy(n => n).ToList();
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(Compares.AreDeepEqual(map1[i], ret1[i]));
            }

            var ret2 = mappings[1].OrderBy(n => n).ToList();
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(map2[i], ret2[i]);
            }
        }

        [TestMethod()]
        public void TestQueryToolSingleAtomCase()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("C1CCC12CCCC2");
            SMARTSQueryTool querytool = new SMARTSQueryTool("C", Default.ChemObjectBuilder.Instance);

            bool status = querytool.Matches(atomContainer);
            Assert.IsTrue(status);

            int nmatch = querytool.MatchesCount;
            Assert.AreEqual(8, nmatch);
        }

        [TestMethod()]
        public void TestQueryToolReSetSmarts()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("C1CCC12CCCC2");
            SMARTSQueryTool querytool = new SMARTSQueryTool("C", Default.ChemObjectBuilder.Instance);

            bool status = querytool.Matches(atomContainer);
            Assert.IsTrue(status);

            int nmatch = querytool.MatchesCount;
            Assert.AreEqual(8, nmatch);

            querytool.Smarts = "CC";
            status = querytool.Matches(atomContainer);
            Assert.IsTrue(status);

            nmatch = querytool.MatchesCount;
            Assert.AreEqual(18, nmatch);

            var umatch = querytool.GetUniqueMatchingAtoms();
            Assert.AreEqual(9, umatch.Count);
        }

        [TestMethod()]
        public void TestUniqueQueries()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("c1ccccc1CCCNCCCc1ccccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(atomContainer);
            Aromaticity.CDKLegacy.Apply(atomContainer);
            SMARTSQueryTool querytool = new SMARTSQueryTool("c1ccccc1", Default.ChemObjectBuilder.Instance);

            bool status = querytool.Matches(atomContainer);
            Assert.IsTrue(status);

            int nmatch = querytool.MatchesCount;
            Assert.AreEqual(24, nmatch);

            var umatch = querytool.GetUniqueMatchingAtoms();
            Assert.AreEqual(2, umatch.Count);
        }

        [TestMethod()]
        public void TestQuery()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("c12cc(CCN)ccc1c(COC)ccc2");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(atomContainer);
            Aromaticity.CDKLegacy.Apply(atomContainer);
            SMARTSQueryTool querytool = new SMARTSQueryTool("c12ccccc1cccc2", Default.ChemObjectBuilder.Instance);

            bool status = querytool.Matches(atomContainer);
            Assert.IsTrue(status);

            int nmatch = querytool.MatchesCount;
            Assert.AreEqual(4, nmatch);

            var umatch = querytool.GetUniqueMatchingAtoms();
            Assert.AreEqual(1, umatch.Count);
        }

        /// <summary>
        /// Note that we don't test the generated SMILES against the
        /// molecule obtained from the factory since the factory derived
        /// molecule does not have an explicit hydrogen, which it really should
        /// have.
        ///
        // @cdk.bug 1985811
        /// </summary>
        [TestMethod()]
        public void TestIndoleAgainstItself()
        {

            IAtomContainer indole = TestMoleculeFactory.MakeIndole();
            AddImplicitHydrogens(indole);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(indole);
            Aromaticity.CDKLegacy.Apply(indole);
            SmilesGenerator generator = new SmilesGenerator().Aromatic();
            string indoleSmiles = generator.Create(indole);
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            indole = smilesParser.ParseSmiles(indoleSmiles);

            SMARTSQueryTool querytool = new SMARTSQueryTool(indoleSmiles, Default.ChemObjectBuilder.Instance);
            Assert.IsTrue(querytool.Matches(indole));
        }

        /// <summary>
        // @cdk.bug 2149621
        /// </summary>
        [TestMethod()]
        public void TestMethane()
        {
            IAtomContainer methane = Silent.ChemObjectBuilder.Instance.CreateAtomContainer();
            IAtom carbon = methane.Builder.CreateAtom(Elements.Carbon.ToIElement());
            carbon.ImplicitHydrogenCount = 4;
            methane.Atoms.Add(carbon);

            SMARTSQueryTool sqt = new SMARTSQueryTool("CC", Default.ChemObjectBuilder.Instance);
            bool matches = sqt.Matches(methane);
            Assert.IsFalse(matches);

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAromaticity()
        {
            SMARTSQueryTool sqt = new SMARTSQueryTool("CC", Default.ChemObjectBuilder.Instance);
            sqt.SetAromaticity(null);
        }

        [TestMethod()]
        public void SetAromaticity()
        {
            SMARTSQueryTool sqt = new SMARTSQueryTool("[a]", Default.ChemObjectBuilder.Instance);

            IAtomContainer furan = CreateFromSmiles("O1C=CC=C1");

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(furan);

            sqt.SetAromaticity(new Aromaticity(ElectronDonation.CDKModel, Cycles.MCBFinder));
            Assert.IsTrue(sqt.Matches(furan, true));

            sqt.SetAromaticity(new Aromaticity(ElectronDonation.PiBondsModel, Cycles.MCBFinder));
            Assert.IsFalse(sqt.Matches(furan, true));
        }

        static IAtomContainer CreateFromSmiles(string smi)
        {
            return new SmilesParser(Silent.ChemObjectBuilder.Instance).ParseSmiles(smi);
        }
    }
}
