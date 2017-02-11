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
using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles;
using NCDK.SMSD.Algorithms.VFLib;
using NCDK.SMSD.Tools;
using System;

namespace NCDK.SMSD
{
    /**
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     *
     * @cdk.module test-smsd
     * @cdk.require java1.6+
     */
    //[TestCategory("SlowTest")]
    [TestClass()]
    public class SMSDBondSensitiveTest
    {

        private static IAtomContainer Napthalene { get; } = Molecules.CreateNaphthalene();
        private static IAtomContainer Cyclohexane { get; } = Molecules.CreateCyclohexane();
        private static IAtomContainer Benzene { get; } = Molecules.CreateBenzene();

        [TestMethod()]
        public void TestSubgraph()
        {

            Isomorphism sbf = new Isomorphism(Algorithm.SubStructure, true);
            sbf.Init(Benzene, Napthalene, true, true);
            sbf.SetChemFilters(false, false, false);
            Console.Out.WriteLine("Match " + sbf.GetTanimotoSimilarity());
            Console.Out.WriteLine("Match count: " + sbf.GetAllAtomMapping().Count);
            Assert.IsTrue(sbf.IsSubgraph());
            Assert.AreEqual(24, sbf.GetAllAtomMapping().Count);
        }

        [TestMethod()]
        public void TestMatchCount()
        {
            Isomorphism smsd = new Isomorphism(Algorithm.VFLibMCS, true);
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer query = sp.ParseSmiles("CC");
            IAtomContainer target = sp.ParseSmiles("C1CCC12CCCC2");

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
        public void TestVFLib()
        {

            Isomorphism sbf = new Isomorphism(Algorithm.VFLibMCS, true);
            sbf.Init(Benzene, Benzene, true, true);
            sbf.SetChemFilters(true, true, true);
            Assert.IsTrue(sbf.IsSubgraph());

        }

        [TestMethod()]
        public void TestSubStructure()
        {
            Isomorphism sbf = new Isomorphism(Algorithm.SubStructure, true);
            sbf.Init(Benzene, Benzene, true, true);
            sbf.SetChemFilters(false, false, false);
            Assert.IsTrue(sbf.IsSubgraph());
        }

        [TestMethod()]
        public void TestCDKMCS()
        {
            Isomorphism ebimcs = new Isomorphism(Algorithm.CDKMCS, true);
            ebimcs.Init(Benzene, Benzene, true, true);
            ebimcs.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs.GetFirstMapping().Count);
            Assert.IsTrue(ebimcs.IsSubgraph());
        }

        [TestMethod()]
        public void TestMCSPlus()
        {

            Isomorphism ebimcs = new Isomorphism(Algorithm.MCSPlus, false);
            ebimcs.Init(Cyclohexane, Benzene, true, true);
            ebimcs.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs.GetFirstMapping().Count);
            Assert.IsTrue(ebimcs.IsSubgraph());

            ebimcs = new Isomorphism(Algorithm.CDKMCS, true);
            ebimcs.Init(Cyclohexane, Benzene, true, true);
            ebimcs.SetChemFilters(true, true, true);
            Assert.IsFalse(ebimcs.IsSubgraph());
        }

        [TestMethod()]
        public void TestSMSD()
        {

            //        Isomorphism ebimcs = new Isomorphism(Algorithm.VFLibMCS, true);
            //        ebimcs.Init(Cyclohexane, Benzene, true, true);
            //        ebimcs.SetChemFilters(true, true, true);
            //        Assert.AreEqual(1, ebimcs.GetFirstMapping().Count);

            Isomorphism ebimcs1 = new Isomorphism(Algorithm.Default, true);
            ebimcs1.Init(Benzene, Napthalene, true, true);
            ebimcs1.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs1.GetFirstAtomMapping().Count);

            ebimcs1 = new Isomorphism(Algorithm.Default, false);
            ebimcs1.Init(Benzene, Napthalene, true, true);
            ebimcs1.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs1.GetFirstAtomMapping().Count);

            ebimcs1 = new Isomorphism(Algorithm.VFLibMCS, true);
            ebimcs1.Init(Benzene, Napthalene, true, true);
            ebimcs1.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs1.GetFirstAtomMapping().Count);

            ebimcs1 = new Isomorphism(Algorithm.CDKMCS, true);
            ebimcs1.Init(Benzene, Napthalene, true, true);
            ebimcs1.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs1.GetFirstAtomMapping().Count);

            ebimcs1 = new Isomorphism(Algorithm.MCSPlus, true);
            ebimcs1.Init(Benzene, Napthalene, true, true);
            ebimcs1.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs1.GetFirstAtomMapping().Count);
        }

        [TestMethod()]
        public void TestSMSDCyclohexaneBenzeneSubgraph()
        {

            //        IQueryAtomContainer queryContainer = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(Cyclohexane);

            Isomorphism ebimcs = new Isomorphism(Algorithm.VFLibMCS, true);
            ebimcs.Init(Cyclohexane, Benzene, true, true);
            ebimcs.SetChemFilters(true, true, true);
            Assert.IsFalse(ebimcs.IsSubgraph());
        }

        [TestMethod()]
        public void TestSMSDBondSensitive()
        {

            Isomorphism ebimcs3 = new Isomorphism(Algorithm.CDKMCS, true);
            ebimcs3.Init(Cyclohexane, Benzene, true, true);
            ebimcs3.SetChemFilters(false, false, false);
            Assert.IsFalse(ebimcs3.IsSubgraph());

            Isomorphism ebimcs4 = new Isomorphism(Algorithm.CDKMCS, true);
            ebimcs4.Init(Benzene, Napthalene, true, true);
            ebimcs4.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs4.GetFirstAtomMapping().Count);

            Isomorphism ebimcs5 = new Isomorphism(Algorithm.VFLibMCS, true);
            ebimcs5.Init(Cyclohexane, Benzene, true, true);
            ebimcs5.SetChemFilters(true, true, true);
            Assert.IsFalse(ebimcs5.IsSubgraph());

            Isomorphism ebimcs6 = new Isomorphism(Algorithm.VFLibMCS, true);
            ebimcs6.Init(Benzene, Napthalene, true, true);
            ebimcs6.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs6.GetFirstAtomMapping().Count);

            Isomorphism ebimcs7 = new Isomorphism(Algorithm.MCSPlus, true);
            ebimcs7.Init(Cyclohexane, Benzene, true, true);
            ebimcs7.SetChemFilters(true, true, true);
            Assert.IsFalse(ebimcs7.IsSubgraph());

            Isomorphism ebimcs8 = new Isomorphism(Algorithm.MCSPlus, true);
            ebimcs8.Init(Benzene, Napthalene, true, true);
            ebimcs8.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs8.GetFirstAtomMapping().Count);
        }

        [TestMethod()]
        public void TestSMSDChemicalFilters()
        {

            Isomorphism ebimcs1 = new Isomorphism(Algorithm.Default, true);
            ebimcs1.Init(Napthalene, Benzene, true, true);
            ebimcs1.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs1.GetAllMapping().Count);
            Assert.IsFalse(ebimcs1.IsSubgraph());
        }

        //    [TestMethod()]
        //    public void TestSingleMappingTesting() {
        //
        //        SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
        //        IAtomContainer atomContainer = sp.ParseSmiles("C");
        //        IQueryAtomContainer query = QueryAtomContainerCreator.CreateBasicQueryContainer(atomContainer);
        //
        //        IAtomContainer mol2 = Molecules.Create4Toluene();
        //
        //        ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
        //        Aromaticity.CDKLegacy.Apply(mol2);
        //
        //        bool bondSensitive = true;
        //        bool removeHydrogen = true;
        //        bool stereoMatch = true;
        //        bool fragmentMinimization = true;
        //        bool energyMinimization = true;
        //
        //        Isomorphism comparison1 = new Isomorphism(Algorithm.Default, bondSensitive);
        //        comparison1.Init(query, mol2, removeHydrogen, true);
        //        comparison1.SetChemFilters(stereoMatch, fragmentMinimization, energyMinimization);
        //
        //        Assert.AreEqual(true, comparison1.IsSubgraph());
        //        Assert.AreEqual(1, comparison1.GetAllMapping().Count);
        //
        //
        //    }
        /**
         * frag is a subgraph of the het mol
         * @throws Exception
         */
        [TestMethod()]
        public void TestSMSDAdpAtpSubgraph()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string adp = "NC1=NC=NC2=C1N=CN2[C@@H]1O[C@H](COP(O)(=O)OP(O)(O)=O)[C@@H](O)[C@H]1O";
            string atp = "NC1=NC=NC2=C1N=CN2[C@@H]1O[C@H](COP(O)(=O)OP(O)(=O)OP(O)(O)=O)[C@@H](O)[C@H]1O";
            IAtomContainer mol1 = sp.ParseSmiles(adp);
            IAtomContainer mol2 = sp.ParseSmiles(atp);

            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);

            //	Calling the main algorithm to perform MCS cearch

            Aromaticity.CDKLegacy.Apply(mol1);
            Aromaticity.CDKLegacy.Apply(mol2);

            bool bondSensitive = true;
            bool removeHydrogen = true;
            bool stereoMatch = true;
            bool fragmentMinimization = true;
            bool energyMinimization = true;

            Isomorphism comparison = new Isomorphism(Algorithm.Default, bondSensitive);
            comparison.Init(mol1, mol2, removeHydrogen, true);
            comparison.SetChemFilters(stereoMatch, fragmentMinimization, energyMinimization);

            //      Get modified Query and Target Molecules as Mappings will correspond to these molecules
            Assert.IsTrue(comparison.IsSubgraph());
            Assert.AreEqual(2, comparison.GetAllMapping().Count);
            Assert.AreEqual(27, comparison.GetFirstMapping().Count);

        }

        [TestMethod()]
        public void TestSMSDLargeSubgraph()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            string c03374 = "CC1=C(C=C)\\C(NC1=O)=C" + "\\C1=C(C)C(CCC(=O)O[C@@H]2O[C@@H]"
                    + "([C@@H](O)[C@H](O)[C@H]2O)C(O)=O)" + "=C(CC2=C(CCC(O)=O)C(C)=C(N2)" + "\\C=C2NC(=O)C(C=C)=C/2C)N1";

            string c05787 = "CC1=C(C=C)\\C(NC1=O)=C" + "\\C1=C(C)C(CCC(=O)O[C@@H]2O[C@@H]"
                    + "([C@@H](O)[C@H](O)[C@H]2O)C(O)=O)" + "=C(CC2=C(CCC(=O)O[C@@H]3O[C@@H]"
                    + "([C@@H](O)[C@H](O)[C@H]3O)C(O)=O)" + "C(C)=C(N2)" + "\\C=C2NC(=O)C(C=C)=C/2C)N1";

            IAtomContainer mol1 = sp.ParseSmiles(c03374);
            IAtomContainer mol2 = sp.ParseSmiles(c05787);

            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);

            IAtomContainer source = ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(mol1);
            IAtomContainer target = ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(mol2);

            //	Calling the main algorithm to perform MCS cearch

            Aromaticity.CDKLegacy.Apply(source);
            Aromaticity.CDKLegacy.Apply(target);

            bool bondSensitive = true;
            bool removeHydrogen = true;
            bool stereoMatch = true;
            bool fragmentMinimization = true;
            bool energyMinimization = true;

            Isomorphism comparison = new Isomorphism(Algorithm.SubStructure, bondSensitive);
            comparison.Init(source, target, removeHydrogen, true);
            comparison.SetChemFilters(stereoMatch, fragmentMinimization, energyMinimization);

            Assert.IsTrue(comparison.IsSubgraph());
            Assert.AreEqual(55, comparison.GetFirstMapping().Count);
        }
    }
}
