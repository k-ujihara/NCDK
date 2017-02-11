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
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.SMSD.Factory
{
    /**
     * Unit testing for the {@link SubStructureSearchAlgorithms} class.
     * @author     Syed Asad Rahman
     * @author     egonw
     * @cdk.module test-smsd
     */
    [TestClass()]
    public class SubStructureSearchAlgorithmsTest
    {

        /**
         * Tests if the CDKMCS can be instantiated without throwing exceptions.
         */
        [TestMethod()]
        public void TestSubStructureSearchAlgorithms()
        {
            Assert.IsNotNull(new Isomorphism(Algorithm.CDKMCS, true));
            Assert.IsNotNull(new Isomorphism(Algorithm.CDKMCS, false));
        }

        /**
         * Test of init method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestInit_3args_1()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/OCC=C");
            IAtomContainer queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            Assert.IsNotNull(smsd1.ReactantMolecule);
            Assert.IsNotNull(smsd1.ProductMolecule);
        }

        /**
         * Test of init method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestInit_3args_2()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/OCC=C");
            IAtomContainer queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            Assert.IsNotNull(smsd1.ReactantMolecule);
            Assert.IsNotNull(smsd1.ProductMolecule);
        }

        /**
         * Test of init method, of class SubStructureSearchAlgorithms.
         * @throws Exception
         */
        [TestMethod()]
        public void TestInit_3args_3()
        {
            //        string sourceMolFileName = "";
            //        string targetMolFileName = "";
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/OCC=C");
            IAtomContainer queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            Assert.IsNotNull(smsd1.ReactantMolecule);
            Assert.IsNotNull(smsd1.ProductMolecule);

        }

        /**
         * Test of setChemFilters method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestSetChemFilters()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/OCC=C");
            IAtomContainer queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.AreEqual(1, smsd1.GetAllAtomMapping().Count);
        }

        /**
         * Test of getFragmentSize method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestGetFragmentSize()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.SubStructure, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(false, true, false);
            int score = 2;
            Assert.AreEqual(score, smsd1.GetFragmentSize(0));
        }

        /**
         * Test of getStereoScore method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestGetStereoScore()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/OCC=C");
            IAtomContainer queryac = sp.ParseSmiles("CCCOCC(C)=C");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, false, false);
            int score = 1048;
            Assert.AreEqual(score, smsd1.GetStereoScore(0));
        }

        /**
         * Test of getEnergyScore method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestGetEnergyScore()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.SubStructure, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(false, false, true);
            var score = 610.0;
            Assert.AreEqual(score, smsd1.GetEnergyScore(0));
        }

        /**
         * Test of getFirstMapping method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestGetFirstMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(7, smsd1.GetFirstMapping().Count);
        }

        /**
         * Test of getAllMapping method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestGetAllMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            sp.Kekulise(false);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(target);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(queryac);
            Aromaticity.CDKLegacy.Apply(target);
            Aromaticity.CDKLegacy.Apply(queryac);

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(2, smsd1.GetAllMapping().Count);
        }

        /**
         * Test of getFirstAtomMapping method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestGetFirstAtomMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(7, smsd1.GetFirstAtomMapping().Count);
        }

        /**
         * Test of getAllAtomMapping method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestGetAllAtomMapping()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            sp.Kekulise(false);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(target);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(queryac);
            Aromaticity.CDKLegacy.Apply(target);
            Aromaticity.CDKLegacy.Apply(queryac);

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(2, smsd1.GetAllAtomMapping().Count);
        }

        /**
         * Test of getReactantMolecule method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestGetReactantMolecule()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(7, smsd1.ReactantMolecule.Atoms.Count);
        }

        /**
         * Test of getProductMolecule method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestGetProductMolecule()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            Assert.AreEqual(20, smsd1.ProductMolecule.Atoms.Count);
        }

        /**
         * Test of getTanimotoSimilarity method, of class SubStructureSearchAlgorithms.
         * @throws Exception
         */
        [TestMethod()]
        public void TestGetTanimotoSimilarity()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, true);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);

            double score = 0.35;
            Assert.AreEqual(score, smsd1.GetTanimotoSimilarity(), 0);
        }

        /**
         * Test of isStereoMisMatch method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         * @throws CDKException
         */
        [TestMethod()]
        public void TestIsStereoMisMatch()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.Default, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.AreEqual(false, smsd1.IsStereoMisMatch());
        }

        /**
         * Test of isSubgraph method, of class SubStructureSearchAlgorithms.
         * @throws InvalidSmilesException
         */
        [TestMethod()]
        public void TestIsSubgraph()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            Isomorphism smsd1 = new Isomorphism(Algorithm.SubStructure, false);
            smsd1.Init(queryac, target, true, true);
            smsd1.SetChemFilters(true, true, true);
            Assert.AreEqual(true, smsd1.IsSubgraph());
        }

        /**
         * Test of getEuclideanDistance method, of class SubStructureSearchAlgorithms.
         * @throws Exception
         */
        [TestMethod()]
        public void TestGetEuclideanDistance()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

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
