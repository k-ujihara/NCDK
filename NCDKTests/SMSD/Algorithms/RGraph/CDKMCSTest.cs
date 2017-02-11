/*
 *
 * Copyright (C) 1997-2007  The Chemistry Development Kit (CKD) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Default;
using NCDK.Graphs;
using NCDK.IO;
using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles;
using NCDK.SMSD.Tools;
using NCDK.Templates;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.IO;

namespace NCDK.SMSD.Algorithms.RGraph
{
    /**
     * @cdk.module test-smsd
     * @author     Syed Asad Rahman
     * @cdk.require java1.5+
     */
    [TestClass()]
    public class CDKMCSTest : CDKTestCase
    {
        bool standAlone = false;

        [TestMethod()]
        public void TestIsSubgraph_IAtomContainer_IAtomContainer()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
            IAtomContainer frag1 = TestMoleculeFactory.MakeCyclohexene(); //one double bond in ring
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(frag1);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol.Builder);
            adder.AddImplicitHydrogens(mol);
            adder = CDKHydrogenAdder.GetInstance(frag1.Builder);
            adder.AddImplicitHydrogens(frag1);
            Aromaticity.CDKLegacy.Apply(mol);
            Aromaticity.CDKLegacy.Apply(frag1);

            if (standAlone)
            {
                Console.Out.WriteLine("Cyclohexene is a subgraph of alpha-Pinen: " + CDKMCS.IsSubgraph(mol, frag1, true));
            }
            else
            {
                Assert.IsTrue(CDKMCS.IsSubgraph(mol, frag1, true));
            }

        }

        /**
         * @cdk.bug 1708336
         * @throws Exception
         */
        [TestMethod()]
        public void TestSFBug1708336()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer atomContainer = builder.CreateAtomContainer();
            atomContainer.Atoms.Add(builder.CreateAtom("C"));
            atomContainer.Atoms.Add(builder.CreateAtom("C"));
            atomContainer.Atoms.Add(builder.CreateAtom("N"));
            atomContainer.AddBond(atomContainer.Atoms[0], atomContainer.Atoms[1], BondOrder.Single);
            atomContainer.AddBond(atomContainer.Atoms[1], atomContainer.Atoms[2], BondOrder.Single);
            IQueryAtomContainer query = new QueryAtomContainer(Default.ChemObjectBuilder.Instance);
            IQueryAtom a1 = new SymbolQueryAtom(Default.ChemObjectBuilder.Instance);
            a1.Symbol = "C";

            var a2 = new Isomorphisms.Matchers.SMARTS.AnyAtom(Default.ChemObjectBuilder.Instance);

            IBond b1 = new OrderQueryBond(a1, a2, BondOrder.Single, Default.ChemObjectBuilder.Instance);

            IQueryAtom a3 = new SymbolQueryAtom(Default.ChemObjectBuilder.Instance);
            a3.Symbol = "C";

            IBond b2 = new OrderQueryBond(a2, a3, BondOrder.Single, Default.ChemObjectBuilder.Instance);
            query.Atoms.Add(a1);
            query.Atoms.Add(a2);
            query.Atoms.Add(a3);

            query.Bonds.Add(b1);
            query.Bonds.Add(b2);

            var list = CDKMCS.GetSubgraphMaps(atomContainer, query, true);

            Assert.IsTrue(list.Count == 0);
        }

        [TestMethod()]
        public void Test2()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
            IAtomContainer frag1 = TestMoleculeFactory.MakeCyclohexane(); // no double bond in ring
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(frag1);
            Aromaticity.CDKLegacy.Apply(mol);
            Aromaticity.CDKLegacy.Apply(frag1);

            if (standAlone)
            {
                Console.Out.WriteLine("Cyclohexane is a subgraph of alpha-Pinen: " + CDKMCS.IsSubgraph(mol, frag1, true));
            }
            else
            {
                Assert.IsTrue(!CDKMCS.IsSubgraph(mol, frag1, true));
            }
        }

        [TestMethod()]
        public void Test3()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(frag1);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol.Builder);
            adder.AddImplicitHydrogens(mol);
            adder = CDKHydrogenAdder.GetInstance(frag1.Builder);
            adder.AddImplicitHydrogens(frag1);
            Aromaticity.CDKLegacy.Apply(mol);
            Aromaticity.CDKLegacy.Apply(frag1);

            if (standAlone)
            {
                Console.Out.WriteLine("Pyrrole is a subgraph of Indole: " + CDKMCS.IsSubgraph(mol, frag1, true));
            }
            else
            {
                Assert.IsTrue(CDKMCS.IsSubgraph(mol, frag1, true));
            }
        }

        [TestMethod()]
        public void TestBasicQueryAtomContainer()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C"); // acetic acid anhydride
            IAtomContainer SMILESquery = sp.ParseSmiles("CC"); // acetic acid anhydride
            var query = QueryAtomContainerCreator.CreateBasicQueryContainer(SMILESquery);

            Assert.IsTrue(CDKMCS.IsSubgraph(atomContainer, query, true));
        }

        [TestMethod()]
        public void TestGetSubgraphAtomsMaps_IAtomContainer()
        {
            int[] result1 = { 6, 5, 7, 8, 0 };
            int[] result2 = { 3, 4, 2, 1, 0 };

            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(frag1);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol.Builder);
            adder.AddImplicitHydrogens(mol);
            adder = CDKHydrogenAdder.GetInstance(frag1.Builder);
            adder.AddImplicitHydrogens(frag1);
            Aromaticity.CDKLegacy.Apply(mol);
            Aromaticity.CDKLegacy.Apply(frag1);

            var list = CDKMCS.GetSubgraphAtomsMaps(mol, frag1, true);
            var first = list[0];
            for (int i = 0; i < first.Count; i++)
            {
                CDKRMap rmap = first[i];
                Assert.AreEqual(rmap.Id1, result1[i]);
                Assert.AreEqual(rmap.Id2, result2[i]);
            }
        }

        [TestMethod()]
        public void TestGetSubgraphMap_IAtomContainer_IAtomContainer()
        {
            string molfile = "NCDK.Data.MDL.decalin.mol";
            string queryfile = "NCDK.Data.MDL.decalin.mol";
            IAtomContainer mol = new AtomContainer();
            IAtomContainer temp = new AtomContainer();
            QueryAtomContainer query1 = null;
            QueryAtomContainer query2 = null;

            Stream ins = this.GetType().Assembly.GetManifestResourceStream(molfile);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            reader.Read(mol);
            ins = this.GetType().Assembly.GetManifestResourceStream(queryfile);
            reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            reader.Read(temp);
            query1 = QueryAtomContainerCreator.CreateBasicQueryContainer(temp);

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("C1CCCCC1");
            query2 = QueryAtomContainerCreator.CreateBasicQueryContainer(atomContainer);

            var list = CDKMCS.GetSubgraphMap(mol, query1, true);
            Assert.AreEqual(11, list.Count);

            list = CDKMCS.GetSubgraphMap(mol, query2, true);
            Assert.AreEqual(6, list.Count);

        }

        /**
         * @cdk.bug 1110537
         * @throws Exception
         */
        [TestMethod()]
        public void TestGetOverlaps_IAtomContainer_IAtomContainer()
        {
            string file1 = "NCDK.Data.MDL.5SD.mol";
            string file2 = "NCDK.Data.MDL.ADN.mol";
            IAtomContainer mol1 = new AtomContainer();
            IAtomContainer mol2 = new AtomContainer();

            Stream ins1 = this.GetType().Assembly.GetManifestResourceStream(file1);
            new MDLV2000Reader(ins1, ChemObjectReaderModes.Strict).Read(mol1);
            Stream ins2 = this.GetType().Assembly.GetManifestResourceStream(file2);
            new MDLV2000Reader(ins2, ChemObjectReaderModes.Strict).Read(mol2);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol1.Builder);
            adder.AddImplicitHydrogens(mol1);
            Aromaticity.CDKLegacy.Apply(mol1);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            adder = CDKHydrogenAdder.GetInstance(mol2.Builder);
            adder.AddImplicitHydrogens(mol2);
            Aromaticity.CDKLegacy.Apply(mol2);

            var list = CDKMCS.GetOverlaps(mol1, mol2, true);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(11, ((AtomContainer)list[0]).Atoms.Count);

            list = CDKMCS.GetOverlaps(mol2, mol1, true);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(11, ((AtomContainer)list[0]).Atoms.Count);
        }

        /**
         * @cdk.bug 999330
         * @throws Exception
         */
        [TestMethod()]
        public void TestSFBug999330()
        {
            string file1 = "NCDK.Data.MDL.5SD.mol";
            string file2 = "NCDK.Data.MDL.ADN.mol";
            IAtomContainer mol1 = new AtomContainer();
            IAtomContainer mol2 = new AtomContainer();

            Stream ins1 = this.GetType().Assembly.GetManifestResourceStream(file1);
            new MDLV2000Reader(ins1, ChemObjectReaderModes.Strict).Read(mol1);
            Stream ins2 = this.GetType().Assembly.GetManifestResourceStream(file2);
            new MDLV2000Reader(ins2, ChemObjectReaderModes.Strict).Read(mol2);
            AtomContainerAtomPermutor permutor = new AtomContainerAtomPermutor(mol2);
            permutor.MoveNext();
            mol2 = new AtomContainer(permutor.Current);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(mol1.Builder);
            adder.AddImplicitHydrogens(mol1);
            Aromaticity.CDKLegacy.Apply(mol1);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            adder = CDKHydrogenAdder.GetInstance(mol2.Builder);
            adder.AddImplicitHydrogens(mol2);
            Aromaticity.CDKLegacy.Apply(mol2);

            var list1 = CDKMCS.GetOverlaps(mol1, mol2, true);
            var list2 = CDKMCS.GetOverlaps(mol2, mol1, true);
            Assert.AreEqual(1, list1.Count);
            Assert.AreEqual(1, list2.Count);
            Assert.AreEqual(((AtomContainer)list1[0]).Atoms.Count,
                    ((AtomContainer)list2[0]).Atoms.Count);
        }

        [TestMethod()]
        public void TestItself()
        {
            string smiles = "C1CCCCCCC1CC";
            var query = QueryAtomContainerCreator.CreateAnyAtomContainer(new SmilesParser(
                    Default.ChemObjectBuilder.Instance).ParseSmiles(smiles), true);
            IAtomContainer ac = new SmilesParser(Default.ChemObjectBuilder.Instance).ParseSmiles(smiles);
            if (standAlone)
            {
                Console.Out.WriteLine("AtomCount of query: " + query.Atoms.Count);
                Console.Out.WriteLine("AtomCount of target: " + ac.Atoms.Count);
            }

            bool matched = CDKMCS.IsSubgraph(ac, query, true);
            if (standAlone)
            {
                Console.Out.WriteLine("QueryAtomContainer matched: " + matched);
            }
            if (!standAlone)
            {
                Assert.IsTrue(matched);
            }
        }

        [TestMethod()]
        public void TestIsIsomorph_IAtomContainer_IAtomContainer()
        {
            AtomContainer ac1 = new AtomContainer();
            ac1.Atoms.Add(new Atom("C"));
            AtomContainer ac2 = new AtomContainer();
            ac2.Atoms.Add(new Atom("C"));
            Assert.IsTrue(CDKMCS.IsIsomorph(ac1, ac2, true));
            Assert.IsTrue(CDKMCS.IsSubgraph(ac1, ac2, true));
        }

        [TestMethod()]
        public void TestAnyAtomAnyBondCase()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("O1C=CC=C1");
            IAtomContainer queryac = sp.ParseSmiles("C1CCCC1");
            var query = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(queryac, false);

            Assert.IsTrue(CDKMCS.IsSubgraph(target, query, true), "C1CCCC1 should be a subgraph of O1C=CC=C1");
            Assert.IsTrue(CDKMCS.IsIsomorph(target, query, true), "C1CCCC1 should be a isomorph of O1C=CC=C1");
        }

        /**
         * @cdk.bug 1633201
         * @throws Exception
         */
        [TestMethod()]
        public void TestFirstArgumentMustNotBeAnQueryAtomContainer()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("O1C=CC=C1");
            IAtomContainer queryac = sp.ParseSmiles("C1CCCC1");
            var query = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(queryac, false);

            try
            {
                CDKMCS.IsSubgraph(query, target, true);
                Assert.Fail("The UniversalIsomorphism should check when the first arguments is a QueryAtomContainer");
            }
            catch (Exception)
            {
                // OK, it must Assert.fail!
            }
        }

        /**
         * @cdk.bug 2888845
         * @throws Exception
         */
        [TestMethod()]
        public void TestSingleAtomMatching1()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("[H]");
            IAtomContainer queryac = sp.ParseSmiles("[H]");
            var query = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(queryac);

            var matches = CDKMCS.GetIsomorphMaps(target, query, true);
            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(1, matches[0].Count);
            CDKRMap mapping = matches[0][0];
            Assert.AreEqual(0, mapping.Id1);
            Assert.AreEqual(0, mapping.Id2);
            var atomMappings = CDKMCS.MakeAtomsMapsOfBondsMaps(matches, target, query);
            Assert.AreEqual(matches, atomMappings);
        }

        /**
         * @cdk.bug 2888845
         * @throws Exception
         */
        [TestMethod()]
        public void TestSingleAtomMatching2()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("CNC");
            IAtomContainer queryac = sp.ParseSmiles("C");
            var query = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(queryac);

            var matches = CDKMCS.GetIsomorphMaps(target, query, true);
            Assert.AreEqual(2, matches.Count);
            Assert.AreEqual(1, matches[0].Count);
            Assert.AreEqual(1, matches[1].Count);
            CDKRMap map1 = matches[0][0];
            CDKRMap map2 = matches[1][0];

            Assert.AreEqual(0, map1.Id1);
            Assert.AreEqual(0, map1.Id2);

            Assert.AreEqual(2, map2.Id1);
            Assert.AreEqual(0, map2.Id2);

            var atomMappings = CDKMCS.MakeAtomsMapsOfBondsMaps(matches, target, query);
            Assert.AreEqual(matches, atomMappings);
        }

        /**
         * Test of getTimeManager method, of class CDKMCS.
         */
        [TestMethod()]
        public void TestGetTimeManager()
        {
            TimeManager expResult = new TimeManager();
            Assert.IsNotNull(expResult);
        }

        /**
         * Test of setTimeManager method, of class CDKMCS.
         */
        [TestMethod()]
        public void TestSetTimeManager()
        {
            TimeManager aTimeManager = new TimeManager();
            CDKMCS.SetTimeManager(aTimeManager);
            Assert.IsNotNull(CDKMCS.GetTimeManager().GetElapsedTimeInSeconds());
        }
    }
}
