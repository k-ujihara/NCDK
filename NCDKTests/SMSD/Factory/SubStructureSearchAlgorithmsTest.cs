/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received rAtomCount copy of the GNU Lesser General Public
 * License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.SMSD.Factory
{
    /// <summary>
    /// Unit testing for the <see cref="SubStructureSearchAlgorithms"/> class.
    /// </summary>
    // @author     Syed Asad Rahman
    // @author     egonw
    // @cdk.module test-smsd
    [TestClass()]
    public class SubStructureSearchAlgorithmsTest
    {
        /// <summary>
        /// Tests if the CDKMCS can be instantiated without throwing exceptions.
        /// </summary>
        [TestMethod()]
        public void TestSubStructureSearchAlgorithms()
        {
            Assert.IsNotNull(new Isomorphism(Algorithm.CDKMCS, true));
            Assert.IsNotNull(new Isomorphism(Algorithm.CDKMCS, false));
        }

        /// <summary>
        /// Test of init method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestInit_3args_1()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/OCC=C");
            var queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            Assert.IsNotNull(smsd1.ReactantMolecule);
            Assert.IsNotNull(smsd1.ProductMolecule);
        }

        /// <summary>
        /// Test of init method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestInit_3args_2()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/OCC=C");
            var queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            Assert.IsNotNull(smsd1.ReactantMolecule);
            Assert.IsNotNull(smsd1.ProductMolecule);
        }

        /// <summary>
        /// Test of init method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestInit_3args_3()
        {
            //        string sourceMolFileName = "";
            //        string targetMolFileName = "";
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/OCC=C");
            var queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            Assert.IsNotNull(smsd1.ReactantMolecule);
            Assert.IsNotNull(smsd1.ProductMolecule);
        }

        /// <summary>
        /// Test of setChemFilters method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestSetChemFilters()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/OCC=C");
            var queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.AreEqual(1, smsd1.GetAllAtomMapping().Count);
        }

        /// <summary>
        /// Test of getFragmentSize method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestGetFragmentSize()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.SubStructure, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(false, true, false);
            int score = 2;
            Assert.AreEqual(score, smsd1.GetFragmentSize(0));
        }

        /// <summary>
        /// Test of getStereoScore method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestGetStereoScore()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/OCC=C");
            var queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            int score = 1048;
            Assert.AreEqual(score, smsd1.GetStereoScore(0));
        }

        /// <summary>
        /// Test of getEnergyScore method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestGetEnergyScore()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.SubStructure, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(false, false, true);
            var score = 610.0;
            Assert.AreEqual(score, smsd1.GetEnergyScore(0));
        }

        /// <summary>
        /// Test of getFirstMapping method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestGetFirstMapping()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(7, smsd1.GetFirstMapping().Count);
        }

        /// <summary>
        /// Test of getAllMapping method, of class SubStructureSearchAlgorithms.
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

            Assert.AreEqual(2, smsd1.GetAllMapping().Count);
        }

        /// <summary>
        /// Test of getFirstAtomMapping method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestGetFirstAtomMapping()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(7, smsd1.GetFirstAtomMapping().Count);
        }

        /// <summary>
        /// Test of getAllAtomMapping method, of class SubStructureSearchAlgorithms.
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

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(2, smsd1.GetAllAtomMapping().Count);
        }

        /// <summary>
        /// Test of getReactantMolecule method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestGetReactantMolecule()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(7, smsd1.ReactantMolecule.Atoms.Count);
        }

        /// <summary>
        /// Test of getProductMolecule method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestGetProductMolecule()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(20, smsd1.ProductMolecule.Atoms.Count);
        }

        /// <summary>
        /// Test of getTanimotoSimilarity method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestGetTanimotoSimilarity()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            double score = 0.35;
            Assert.AreEqual(score, smsd1.GetTanimotoSimilarity(), 0);
        }

        /// <summary>
        /// Test of isStereoMisMatch method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestIsStereoMisMatch()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.AreEqual(false, smsd1.IsStereoMisMatch());
        }

        /// <summary>
        /// Test of isSubgraph method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestIsSubgraph()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.SubStructure, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.AreEqual(true, smsd1.IsSubgraph());
        }

        /// <summary>
        /// Test of getEuclideanDistance method, of class SubStructureSearchAlgorithms.
        /// </summary>
        [TestMethod()]
        public void TestGetEuclideanDistance()
        {
            var sp = CDK.SilentSmilesParser;
            var target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            var queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.SubStructure, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            double score = 3.605;
            Assert.AreEqual(score, smsd1.GetEuclideanDistance(), 0.005);

            Isomorphism smsd2 = new Isomorphism(Algorithm.VFLibMCS, true);
            smsd2.Init(queryac, target, true, true);
            smsd2.SetChemFilters(true, true, true);

            Assert.AreEqual(score, smsd2.GetEuclideanDistance(), 0.005);
        }
    }
}
