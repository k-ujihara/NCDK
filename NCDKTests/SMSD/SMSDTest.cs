/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Silent;
using NCDK.IO;
using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles;
using NCDK.SMSD.Tools;
using NCDK.Tools.Manipulator;
using System.Diagnostics;
using System.IO;

namespace NCDK.SMSD
{
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public class SMSDTest
    {
        public SMSDTest() { }

        /// <summary>
        /// Test of init method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestInit_3args_1()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/OCC=C");
            var queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            Assert.IsNotNull(smsd1.ReactantMolecule);
            Assert.IsNotNull(smsd1.ProductMolecule);
        }

        /// <summary>
        /// Test of init method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestInit_3args_2()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/OCC=C");
            var queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            Assert.IsNotNull(smsd1.ReactantMolecule);
            Assert.IsNotNull(smsd1.ProductMolecule);
        }

        /// <summary>
        /// Test of searchMCS method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestSearchMCS()
        {
            try
            {
                var sp = new SmilesParser(ChemObjectBuilder.Instance, false);
                IAtomContainer target = null;
                target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(target);
                var queryac = sp.ParseSmiles("Nc1ccccc1");
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(queryac);
                Aromaticity.CDKLegacy.Apply(target);
                Aromaticity.CDKLegacy.Apply(queryac);
                Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
                smsd1.Init(queryac, target, true, true);
                smsd1.SetChemFilters(true, true, true);
                Assert.AreEqual(7, smsd1.GetFirstAtomMapping().Count);
                Assert.AreEqual(2, smsd1.GetAllAtomMapping().Count);
                Assert.IsNotNull(smsd1.GetFirstMapping());
            }
            catch (InvalidSmilesException ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        /// <summary>
        /// Test of set method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestSet_IAtomContainer_IAtomContainer()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

        }

        /// <summary>
        /// Test of set method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestSet_String_String()
        {
            string molfile = "NCDK.Data.MDL.decalin.mol";
            string queryfile = "NCDK.Data.MDL.decalin.mol";
            IAtomContainer query = new AtomContainer();
            IAtomContainer target = new AtomContainer();

            var ins = ResourceLoader.GetAsStream(molfile);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            reader.Read(query);
            ins = ResourceLoader.GetAsStream(queryfile);
            reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            reader.Read(target);

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(query, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            double score = 1.0;
            Assert.AreEqual(score, smsd1.GetTanimotoSimilarity(), 0.0001);
        }

        /// <summary>
        /// Test of set method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestSet_MolHandler_MolHandler()
        {
            var sp = CDK.SmilesParser;

            var target1 = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");
            MolHandler source = new MolHandler(queryac, true, true);
            MolHandler target = new MolHandler(target1, true, true);
            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(source.Molecule, target.Molecule, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.IsNotNull(smsd1.GetFirstMapping());
        }

        /// <summary>
        /// Test of getAllAtomMapping method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetAllAtomMapping()
        {
            var sp = new SmilesParser(ChemObjectBuilder.Instance, false);
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(target);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(queryac);
            Aromaticity.CDKLegacy.Apply(target);
            Aromaticity.CDKLegacy.Apply(queryac);

            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(queryac);
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(target);

            //    Calling the main algorithm to perform MCS cearch

            Aromaticity.CDKLegacy.Apply(queryac);
            Aromaticity.CDKLegacy.Apply(target);

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.IsNotNull(smsd1.GetFirstMapping());
            Assert.AreEqual(2, smsd1.GetAllAtomMapping().Count);
        }

        /// <summary>
        /// Test of getAllMapping method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetAllMapping()
        {
            var sp = new SmilesParser(ChemObjectBuilder.Instance, false);
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(target);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(queryac);
            Aromaticity.CDKLegacy.Apply(target);
            Aromaticity.CDKLegacy.Apply(queryac);

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(2, smsd1.GetAllMapping().Count);
        }

        /// <summary>
        /// Test of getFirstAtomMapping method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetFirstAtomMapping()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(7, smsd1.GetFirstAtomMapping().Count);
        }

        /// <summary>
        /// Test of getFirstMapping method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetFirstMapping()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.IsNotNull(smsd1.GetFirstMapping());

            Assert.AreEqual(7, smsd1.GetFirstMapping().Count);
        }

        /// <summary>
        /// Test of setChemFilters method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestSetChemFilters()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/OCC=C");
            var queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.AreEqual(1, smsd1.GetAllAtomMapping().Count);
        }

        /// <summary>
        /// Test of getFragmentSize method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetFragmentSize()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(false, true, false);
            int score = 2;
            Assert.AreEqual(score, smsd1.GetFragmentSize(0));
        }

        /// <summary>
        /// Test of getStereoScore method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetStereoScore()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/OCC=C");
            var queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            int score = 1048;
            Assert.AreEqual(score, smsd1.GetStereoScore(0));
        }

        /// <summary>
        /// Test of getEnergyScore method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetEnergyScore()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(false, false, true);
            double score = 610.0;
            Assert.AreEqual(score, smsd1.GetEnergyScore(0));
        }

        /// <summary>
        /// Test of getReactantMolecule method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetReactantMolecule()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(7, smsd1.ReactantMolecule.Atoms.Count);
        }

        /// <summary>
        /// Test of getProductMolecule method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetProductMolecule()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(20, smsd1.ProductMolecule.Atoms.Count);
        }

        /// <summary>
        /// Test of getTanimotoSimilarity method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetTanimotoSimilarity()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            double score = 0.35;
            Assert.AreEqual(score, smsd1.GetTanimotoSimilarity(), 0);
        }

        /// <summary>
        /// Test of isStereoMisMatch method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestIsStereoMisMatch()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.AreEqual(false, smsd1.IsStereoMisMatch());
        }

        /// <summary>
        /// Test of isSubgraph method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestIsSubgraph()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.SubStructure, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.AreEqual(true, smsd1.IsSubgraph());
        }

        /// <summary>
        /// Test of getEuclideanDistance method, of class Isomorphism.
        /// </summary>
        [TestMethod()]
        public void TestGetEuclideanDistance()
        {
            var sp = CDK.SmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            var smsd1 = new Isomorphism(Algorithm.SubStructure, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            double score = 3.605;
            Assert.AreEqual(score, smsd1.GetEuclideanDistance(), 0.005);

            Isomorphism smsd2 = new Isomorphism(Algorithm.VFLibMCS, true);
            smsd2.Init(queryac, target, true, true);
            smsd2.SetChemFilters(true, true, true);

            Assert.AreEqual(score, smsd2.GetEuclideanDistance(), 0.005);
        }

        [TestMethod()]
        public void TestQueryAtomContainerDefault()
        {
            Isomorphism smsd = new Isomorphism(Algorithm.Default, true);
            var sp = CDK.SmilesParser;
            var query = sp.ParseSmiles("CC");
            var target = sp.ParseSmiles("C1CCC12CCCC2");

            smsd.Init(query, target, false, true);
            bool foundMatches = smsd.IsSubgraph();
            Assert.IsTrue(foundMatches);

            IQueryAtomContainer queryContainer = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(query);
            smsd.Init(queryContainer, target);
            foundMatches = smsd.IsSubgraph();
            Assert.IsTrue(foundMatches);
        }

        [TestMethod()]
        public void TestQueryAtomContainerMCSPLUS()
        {
            Isomorphism smsd = new Isomorphism(Algorithm.MCSPlus, true);
            var sp = CDK.SmilesParser;
            var query = sp.ParseSmiles("CC");
            var target = sp.ParseSmiles("C1CCC12CCCC2");

            smsd.Init(query, target, false, true);
            bool foundMatches = smsd.IsSubgraph();
            Assert.IsTrue(foundMatches);

            IQueryAtomContainer queryContainer = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(query);
            smsd.Init(queryContainer, target);
            foundMatches = smsd.IsSubgraph();
            Assert.IsTrue(foundMatches);
        }

        [TestMethod()]
        public void TestQueryAtomContainerSubstructure()
        {
            var sp = CDK.SmilesParser;
            var query = sp.ParseSmiles("CC");
            var target = sp.ParseSmiles("C1CCC12CCCC2");

            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(query);
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(target);

            //    Calling the main algorithm to perform MCS cearch

            Aromaticity.CDKLegacy.Apply(query);
            Aromaticity.CDKLegacy.Apply(target);

            Isomorphism smsd = new Isomorphism(Algorithm.SubStructure, true);
            smsd.Init(query, target, false, true);
            bool foundMatches = smsd.IsSubgraph();
            Assert.IsTrue(foundMatches);

            //        IQueryAtomContainer queryContainer = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(query);
            //
            //        Isomorphism smsd1 = new Isomorphism(Algorithm.SubStructure, true);
            //        smsd1.Init(queryContainer, target, true, true);
            //        smsd1.SetChemFilters(true, true, true);
            //        foundMatches = smsd1.IsSubgraph();
            //        Assert.IsFalse(foundMatches);
        }

        public void TestQueryAtomCount()
        {
            Isomorphism smsd = new Isomorphism(Algorithm.Default, true);
            var sp = CDK.SmilesParser;
            var query = sp.ParseSmiles("CC");
            var target = sp.ParseSmiles("C1CCC12CCCC2");

            smsd.Init(query, target, false, true);
            bool foundMatches = smsd.IsSubgraph();
            Assert.AreEqual(18, smsd.GetAllAtomMapping().Count);
            Assert.IsTrue(foundMatches);

            IQueryAtomContainer queryContainer = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(query);
            smsd.Init(queryContainer, target);
            foundMatches = smsd.IsSubgraph();
            Assert.IsTrue(foundMatches);
        }

        [TestMethod()]
        public void TestMatchCount()
        {
            Isomorphism smsd = new Isomorphism(Algorithm.Default, true);
            var sp = CDK.SmilesParser;
            var query = sp.ParseSmiles("CC");
            var target = sp.ParseSmiles("C1CCC12CCCC2");

            smsd.Init(query, target, false, true);
            bool foundMatches = smsd.IsSubgraph();
            Assert.AreEqual(18, smsd.GetAllAtomMapping().Count);
            Assert.IsTrue(foundMatches);

            IQueryAtomContainer queryContainer = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(query);
            smsd.Init(queryContainer, target);
            foundMatches = smsd.IsSubgraph();
            Assert.IsTrue(foundMatches);
        }

        [TestMethod()]
        public void TestMatchCountCDKMCS()
        {
            Isomorphism smsd = new Isomorphism(Algorithm.CDKMCS, true);
            var sp = CDK.SmilesParser;
            var query = sp.ParseSmiles("CC");
            var target = sp.ParseSmiles("C1CCC12CCCC2");

            smsd.Init(query, target, false, true);
            bool foundMatches = smsd.IsSubgraph();
            Assert.AreEqual(18, smsd.GetAllAtomMapping().Count);
            Assert.IsTrue(foundMatches);

            IQueryAtomContainer queryContainer = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(query);
            smsd.Init(queryContainer, target);
            foundMatches = smsd.IsSubgraph();
            Assert.IsTrue(foundMatches);
        }

        [TestMethod()]
        public void TestImpossibleQuery()
        {
            Isomorphism smsd = new Isomorphism(Algorithm.Default, true);
            var sp = CDK.SmilesParser;
            var query = sp.ParseSmiles("CC");
            var target = sp.ParseSmiles("C");

            smsd.Init(query, target, false, true);
            bool foundMatches = smsd.IsSubgraph();
            Assert.IsFalse(foundMatches);
        }
    }
}
