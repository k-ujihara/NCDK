/* Copyright (C) 1997-2007  The Chemistry Development Kit (CKD) project
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
using NCDK.Aromaticities;
using NCDK.AtomTypes;
using NCDK.Default;
using NCDK.Graphs;
using NCDK.IO;
using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles;
using NCDK.Templates;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections;
using System.Linq;

namespace NCDK.Isomorphisms
{
    // @cdk.module test-standard
    [TestClass()]
    public class UniversalIsomorphismTesterTest : CDKTestCase
    {
        bool standAlone = false;
        private UniversalIsomorphismTester uiTester = new UniversalIsomorphismTester();

        [TestMethod()]
        public void TestIsSubgraph_IAtomContainer_IAtomContainer()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
            IAtomContainer frag1 = TestMoleculeFactory.MakeCyclohexene(); //one double bond in ring
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(frag1);
            Aromaticity.CDKLegacy.Apply(mol);
            Aromaticity.CDKLegacy.Apply(frag1);

            if (standAlone)
            {
                Console.Out.WriteLine("Cyclohexene is a subgraph of alpha-Pinen: " + uiTester.IsSubgraph(mol, frag1));
            }
            else
            {
                Assert.IsTrue(uiTester.IsSubgraph(mol, frag1));
            }

        }

        // @cdk.bug 1708336
        [TestMethod()]
        public void TestSFBug1708336()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IAtomContainer atomContainer = builder.NewAtomContainer();
            atomContainer.Atoms.Add(builder.NewAtom("C"));
            atomContainer.Atoms.Add(builder.NewAtom("C"));
            atomContainer.Atoms.Add(builder.NewAtom("N"));
            atomContainer.AddBond(atomContainer.Atoms[0], atomContainer.Atoms[1], BondOrder.Single);
            atomContainer.AddBond(atomContainer.Atoms[1], atomContainer.Atoms[2], BondOrder.Single);
            IQueryAtomContainer query = new QueryAtomContainer(Default.ChemObjectBuilder.Instance);
            IQueryAtom a1 = new SymbolQueryAtom(Default.ChemObjectBuilder.Instance);
            a1.Symbol = "C";

            var a2 = new Matchers.SMARTS.AnyAtom(Default.ChemObjectBuilder.Instance);

            IBond b1 = new OrderQueryBond(a1, a2, BondOrder.Single, Default.ChemObjectBuilder.Instance);

            IQueryAtom a3 = new SymbolQueryAtom(Default.ChemObjectBuilder.Instance);
            a3.Symbol = "C";

            IBond b2 = new OrderQueryBond(a2, a3, BondOrder.Single, Default.ChemObjectBuilder.Instance);
            query.Atoms.Add(a1);
            query.Atoms.Add(a2);
            query.Atoms.Add(a3);

            query.Bonds.Add(b1);
            query.Bonds.Add(b2);

            var list = uiTester.GetSubgraphMaps(atomContainer, query);

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
                Console.Out.WriteLine("Cyclohexane is a subgraph of alpha-Pinen: " + uiTester.IsSubgraph(mol, frag1));
            }
            else
            {
                Assert.IsTrue(!uiTester.IsSubgraph(mol, frag1));
            }
        }

        [TestMethod()]
        public void Test3()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeIndole();
            IAtomContainer frag1 = TestMoleculeFactory.MakePyrrole();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(frag1);
            Aromaticity.CDKLegacy.Apply(mol);
            Aromaticity.CDKLegacy.Apply(frag1);

            if (standAlone)
            {
                Console.Out.WriteLine("Pyrrole is a subgraph of Indole: " + uiTester.IsSubgraph(mol, frag1));
            }
            else
            {
                Assert.IsTrue(uiTester.IsSubgraph(mol, frag1));
            }
        }

        [TestMethod()]
        public void TestBasicQueryAtomContainer()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("CC(=O)OC(=O)C"); // acetic acid anhydride
            IAtomContainer SMILESquery = sp.ParseSmiles("CC"); // acetic acid anhydride
            var query = QueryAtomContainerCreator.CreateBasicQueryContainer(SMILESquery);

            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));
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
            Aromaticity.CDKLegacy.Apply(mol);
            Aromaticity.CDKLegacy.Apply(frag1);

            var list = uiTester.GetSubgraphAtomsMaps(mol, frag1);
            var first = list[0];
            for (int i = 0; i < first.Count; i++)
            {
                var rmap = first[i];
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

            var ins = ResourceLoader.GetAsStream(molfile);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            reader.Read(mol);
            ins = ResourceLoader.GetAsStream(queryfile);
            reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            reader.Read(temp);
            query1 = QueryAtomContainerCreator.CreateBasicQueryContainer(temp);

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer atomContainer = sp.ParseSmiles("C1CCCCC1");
            query2 = QueryAtomContainerCreator.CreateBasicQueryContainer(atomContainer);

            var list = uiTester.GetSubgraphMap(mol, query1);
            Assert.AreEqual(11, list.Count);

            list = uiTester.GetSubgraphMap(mol, query2);
            Assert.AreEqual(6, list.Count);

        }

        // @cdk.bug 1110537
        [TestMethod()]
        public void TestGetOverlaps_IAtomContainer_IAtomContainer()
        {
            string file1 = "NCDK.Data.MDL.5SD.mol";
            string file2 = "NCDK.Data.MDL.ADN.mol";
            IAtomContainer mol1 = new AtomContainer();
            IAtomContainer mol2 = new AtomContainer();

            var ins1 = ResourceLoader.GetAsStream(file1);
            new MDLV2000Reader(ins1, ChemObjectReaderModes.Strict).Read(mol1);
            var ins2 = ResourceLoader.GetAsStream(file2);
            new MDLV2000Reader(ins2, ChemObjectReaderModes.Strict).Read(mol2);

            var list = uiTester.GetOverlaps(mol1, mol2);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(11, ((AtomContainer)list[0]).Atoms.Count);

            list = uiTester.GetOverlaps(mol2, mol1);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(11, ((AtomContainer)list[0]).Atoms.Count);
        }

        // @cdk.bug 2944080
        [TestMethod()]
        public void TestBug2944080()
        {
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = smilesParser.ParseSmiles("CCC(CC)(C(=O)NC(=O)NC(C)=O)Br");
            IAtomContainer mol2 = smilesParser.ParseSmiles("CCC(=CC)C(=O)NC(N)=O");

            var list = uiTester.GetOverlaps(mol1, mol2);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(9, list[0].Atoms.Count);

            list = uiTester.GetOverlaps(mol2, mol1);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(9, list[0].Atoms.Count);
        }

        // @cdk.bug 2944080
        [TestMethod()]
        public void TestGetSubgraphAtomsMap_2944080()
        {
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = smilesParser.ParseSmiles("CCC(CC)(C(=O)NC(=O)NC(C)=O)Br");
            IAtomContainer mol2 = smilesParser.ParseSmiles("CCCC(=O)NC(N)=O");

            //Test for atom mapping between the mols
            var maplist = uiTester.GetSubgraphAtomsMap(mol1, mol2);
            Assert.IsNotNull(maplist);
            Assert.AreEqual(9, maplist.Count);
        }

        // @cdk.bug 2944080
        [TestMethod()]
        public void TestGetSubgraphMap_2944080()
        {
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = smilesParser.ParseSmiles("CCC(CC)(C(=O)NC(=O)NC(C)=O)Br");
            IAtomContainer mol2 = smilesParser.ParseSmiles("CCCC(=O)NC(N)=O");

            //Test for atom mapping between the mols
            var maplist = uiTester.GetSubgraphMap(mol1, mol2);
            Assert.IsNotNull(maplist);
            Assert.AreEqual(8, maplist.Count);
        }

        // @cdk.bug 2944080
        [TestMethod()]
        public void TestSearchNoConditions_2944080()
        {
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = smilesParser.ParseSmiles("CCC(CC)(C(=O)NC(=O)NC(C)=O)Br");
            IAtomContainer mol2 = smilesParser.ParseSmiles("CCCC(=O)NC(N)=O");

            //Test for atom mapping between the mols
            var maplist = uiTester.Search(mol1, mol2, new BitArray(mol1.Atoms.Count),
                            UniversalIsomorphismTester.GetBitSet(mol2), false, false);
            Assert.IsNotNull(maplist);
            Assert.AreEqual(1, maplist.Count);
        }

        // @cdk.bug 2944080
        [TestMethod()]
        public void TestSearch_2944080()
        {
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = smilesParser.ParseSmiles("CCC(CC)(C(=O)NC(=O)NC(C)=O)Br");
            IAtomContainer mol2 = smilesParser.ParseSmiles("CCC(=CC)C(=O)NC(N)=O");

            //Test for atom mapping between the mols
            var list = uiTester.Search(mol1, mol2, new BitArray(mol1.Atoms.Count), new BitArray(mol2.Atoms.Count), true, true);
            Assert.AreEqual(3, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                var first = list[i];
                Assert.AreNotSame(0, first.Count);
            }

            list = uiTester.Search(mol1, mol2, new BitArray(mol1.Atoms.Count), new BitArray(mol2.Atoms.Count), false, false);
            Assert.AreEqual(1, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                var first = list[i];
                Assert.AreNotSame(0, first.Count);
            }
        }

        // @cdk.bug 2944080
        [TestMethod()]
        public void TestGetSubgraphAtomsMaps_2944080()
        {
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol1 = smilesParser.ParseSmiles("CCC(CC)(C(=O)NC(=O)NC(C)=O)Br");
            IAtomContainer mol2 = smilesParser.ParseSmiles("CCCC(=O)NC(N)=O");

            var list = uiTester.GetSubgraphAtomsMaps(mol1, mol2);
            Assert.IsNotNull(list);
            Assert.AreNotSame(0, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                var first = list[i];
                Assert.IsNotNull(first);
                Assert.AreNotSame(0, first.Count);
            }
        }

        [TestMethod()]
        public void TestGetSubgraphAtomsMap_Butane()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeAlkane(4);
            IAtomContainer mol2 = TestMoleculeFactory.MakeAlkane(4);

            // Test for atom mapping between the mols
            var maplist = uiTester.GetSubgraphAtomsMap(mol2, mol1);
            Assert.IsNotNull(maplist);
            Assert.AreEqual(4, maplist.Count);

            maplist = uiTester.GetSubgraphAtomsMap(mol1, mol2);
            Assert.IsNotNull(maplist);
            Assert.AreEqual(4, maplist.Count);
        }

        [TestMethod()]
        public void TestGetSubgraphAtomsMaps_Butane()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeAlkane(4);
            IAtomContainer mol2 = TestMoleculeFactory.MakeAlkane(4);

            var list = uiTester.GetSubgraphAtomsMaps(mol1, mol2);
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                var first = list[i];
                Assert.IsNotNull(first);
                Assert.AreEqual(4, first.Count);
            }
        }

        // @cdk.bug 999330
        [TestMethod()]
        public void TestSFBug999330()
        {
            string file1 = "NCDK.Data.MDL.5SD.mol";
            string file2 = "NCDK.Data.MDL.ADN.mol";
            IAtomContainer mol1 = new AtomContainer();
            IAtomContainer mol2 = new AtomContainer();

            var ins1 = ResourceLoader.GetAsStream(file1);
            new MDLV2000Reader(ins1, ChemObjectReaderModes.Strict).Read(mol1);
            var ins2 = ResourceLoader.GetAsStream(file2);
            new MDLV2000Reader(ins2, ChemObjectReaderModes.Strict).Read(mol2);
            AtomContainerAtomPermutor permutor = new AtomContainerAtomPermutor(mol2);
            permutor.MoveNext();
            mol2 = new AtomContainer((AtomContainer)permutor.Current);

            var list1 = uiTester.GetOverlaps(mol1, mol2);
            var list2 = uiTester.GetOverlaps(mol2, mol1);
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

            bool matched = uiTester.IsSubgraph(ac, query);
            if (standAlone) Console.Out.WriteLine("QueryAtomContainer matched: " + matched);
            if (!standAlone) Assert.IsTrue(matched);
        }

        [TestMethod()]
        public void TestIsIsomorph_IAtomContainer_IAtomContainer()
        {
            AtomContainer ac1 = new AtomContainer();
            ac1.Atoms.Add(new Atom("C"));
            AtomContainer ac2 = new AtomContainer();
            ac2.Atoms.Add(new Atom("C"));
            Assert.IsTrue(uiTester.IsIsomorph(ac1, ac2));
            Assert.IsTrue(uiTester.IsSubgraph(ac1, ac2));
        }

        [TestMethod()]
        public void TestAnyAtomAnyBondCase()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("O1C=CC=C1");
            IAtomContainer queryac = sp.ParseSmiles("C1CCCC1");
            var query = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(queryac, false);

            Assert.IsTrue(uiTester.IsSubgraph(target, query), "C1CCCC1 should be a subgraph of O1C=CC=C1");
            Assert.IsTrue(uiTester.IsIsomorph(target, query), "C1CCCC1 should be a isomorph of O1C=CC=C1");
        }

        // @cdk.bug 1633201
        [TestMethod()]
        public void TestFirstArgumentMustNotBeAnQueryAtomContainer()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("O1C=CC=C1");
            IAtomContainer queryac = sp.ParseSmiles("C1CCCC1");
            var query = QueryAtomContainerCreator.CreateAnyAtomAnyBondContainer(queryac, false);

            try
            {
                uiTester.IsSubgraph(query, target);
                Assert.Fail("The UniversalIsomorphism should check when the first arguments is a QueryAtomContainer");
            }
            catch (Exception)
            {
                // OK, it must Assert.fail!
            }
        }

        [TestMethod()]
        public void TestSingleAtomMatching()
        {

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer target = sp.ParseSmiles("C");
            IAtomContainer query = sp.ParseSmiles("C");

            UniversalIsomorphismTester tester = new UniversalIsomorphismTester();
            Assert.IsTrue(tester.IsIsomorph(target, query));
            Assert.IsTrue(tester.IsIsomorph(query, target));
        }

        [TestMethod()]
        public void TestSingleAtomMismatching()
        {

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer target = sp.ParseSmiles("C");
            IAtomContainer query = sp.ParseSmiles("N");

            UniversalIsomorphismTester tester = new UniversalIsomorphismTester();
            Assert.IsFalse(tester.IsIsomorph(target, query), "Single carbon and nitrogen should not match");
            Assert.IsFalse(tester.IsIsomorph(query, target), "Single nitrogen and carbon should not match");
        }

        // @cdk.bug 2888845
        [TestMethod()]
        public void TestSingleAtomMatching1()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("[H]");
            IAtomContainer queryac = sp.ParseSmiles("[H]");
            var query = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(queryac);

            var matches = uiTester.GetIsomorphMaps(target, query);
            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(1, matches[0].Count);
            var mapping = matches[0][0];
            Assert.AreEqual(0, mapping.Id1);
            Assert.AreEqual(0, mapping.Id2);
            var atomMappings = UniversalIsomorphismTester.MakeAtomsMapsOfBondsMaps(matches, target, query);
            Assert.AreEqual(matches, atomMappings);
        }

        // @cdk.bug 2888845
        [TestMethod()]
        public void TestSingleAtomMatching2()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("CNC");
            IAtomContainer queryac = sp.ParseSmiles("C");
            var query = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(queryac);

            var matches = uiTester.GetIsomorphMaps(target, query);
            Assert.AreEqual(2, matches.Count);
            Assert.AreEqual(1, matches[0].Count);
            Assert.AreEqual(1, matches[1].Count);
            var map1 = matches[0][0];
            var map2 = matches[1][0];

            Assert.AreEqual(0, map1.Id1);
            Assert.AreEqual(0, map1.Id2);

            Assert.AreEqual(2, map2.Id1);
            Assert.AreEqual(0, map2.Id2);

            var atomMappings = UniversalIsomorphismTester.MakeAtomsMapsOfBondsMaps(matches, target, query);
            Assert.AreEqual(matches, atomMappings);
        }

        // @cdk.bug 2912627
        [TestMethod()]
        public void TestSingleAtomMatching3()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("CNC");
            IAtomContainer queryac = sp.ParseSmiles("C");

            var matches = uiTester.GetIsomorphMaps(target, queryac);
            Assert.AreEqual(2, matches.Count);
            Assert.AreEqual(1, matches[0].Count);
            Assert.AreEqual(1, matches[1].Count);
            var map1 = matches[0][0];
            var map2 = matches[1][0];

            Assert.AreEqual(0, map1.Id1);
            Assert.AreEqual(0, map1.Id2);

            Assert.AreEqual(2, map2.Id1);
            Assert.AreEqual(0, map2.Id2);

            var atomMappings = UniversalIsomorphismTester.MakeAtomsMapsOfBondsMaps(matches, target, queryac);
            Assert.AreEqual(matches, atomMappings);
        }

        [TestMethod()]
        public void TestUITTimeoutFix()
        {
            // Load molecules
            string filename = "NCDK.Data.MDL.UITTimeout.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read(new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer[] molecules = new IAtomContainer[2];
            for (int j = 0; j < 2; j++)
            {
                IAtomContainer aAtomContainer = (IAtomContainer)cList[j];
                CDKAtomTypeMatcher tmpMatcher = CDKAtomTypeMatcher.GetInstance(aAtomContainer.Builder);
                CDKHydrogenAdder tmpAdder = CDKHydrogenAdder.GetInstance(aAtomContainer.Builder);
                for (int i = 0; i < aAtomContainer.Atoms.Count; i++)
                {
                    IAtom tmpAtom = aAtomContainer.Atoms[i];
                    IAtomType tmpType = tmpMatcher.FindMatchingAtomType(aAtomContainer, tmpAtom);
                    AtomTypeManipulator.Configure(tmpAtom, tmpType);
                    tmpAdder.AddImplicitHydrogens(aAtomContainer, tmpAtom);
                }
                AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(aAtomContainer);
                molecules[j] = aAtomContainer;
            }
            var query = QueryAtomContainerCreator.CreateAnyAtomForPseudoAtomQueryContainer(molecules[1]);
            // test
            long starttime = System.DateTime.Now.Ticks;
            uiTester.Timeout = 200;
            uiTester.GetSubgraphAtomsMaps(molecules[0], query);
            long duration = System.DateTime.Now.Ticks - starttime;
            // The search must last much longer then two seconds if the timeout not works
            Assert.IsTrue(duration < 2000 * 10000);  // 1 msec = 10000 ticks
        }

        // @cdk.bug 3513335
        [TestMethod()]
        public void TestUITSymmetricMatch()
        {
            QueryAtomContainer q = new QueryAtomContainer(Default.ChemObjectBuilder.Instance);
            //setting atoms
            IQueryAtom a0 = new Matchers.SMARTS.AliphaticSymbolAtom("C", Default.ChemObjectBuilder.Instance);
            q.Atoms.Add(a0);
            IQueryAtom a1 = new Matchers.SMARTS.AnyAtom(Default.ChemObjectBuilder.Instance);
            q.Atoms.Add(a1);
            IQueryAtom a2 = new Matchers.SMARTS.AnyAtom(Default.ChemObjectBuilder.Instance);
            q.Atoms.Add(a2);
            IQueryAtom a3 = new Matchers.SMARTS.AliphaticSymbolAtom("C", Default.ChemObjectBuilder.Instance);
            q.Atoms.Add(a3);
            //setting bonds
            var b0 = new Matchers.SMARTS.OrderQueryBond(
                            BondOrder.Single, Default.ChemObjectBuilder.Instance);
            b0.SetAtoms(new IAtom[] { a0, a1 });
            q.Bonds.Add(b0);
            var b1 = new Matchers.SMARTS.OrderQueryBond(
                            BondOrder.Single, Default.ChemObjectBuilder.Instance);
            b1.SetAtoms(new IAtom[] { a1, a2 });
            q.Bonds.Add(b1);
            var b2 = new Matchers.SMARTS.OrderQueryBond(
                            BondOrder.Single, Default.ChemObjectBuilder.Instance);
            b2.SetAtoms(new IAtom[] { a2, a3 });
            q.Bonds.Add(b2);

            //Creating 'SCCS' target molecule
            AtomContainer target = new AtomContainer();
            //atoms
            IAtom ta0 = new Atom("S");
            target.Atoms.Add(ta0);
            IAtom ta1 = new Atom("C");
            target.Atoms.Add(ta1);
            IAtom ta2 = new Atom("C");
            target.Atoms.Add(ta2);
            IAtom ta3 = new Atom("S");
            target.Atoms.Add(ta3);
            //bonds
            IBond tb0 = new Bond();
            tb0.SetAtoms(new IAtom[] { ta0, ta1 });
            tb0.Order = BondOrder.Single;
            target.Bonds.Add(tb0);

            IBond tb1 = new Bond();
            tb1.SetAtoms(new IAtom[] { ta1, ta2 });
            tb1.Order = BondOrder.Single;
            target.Bonds.Add(tb1);

            IBond tb2 = new Bond();
            tb2.SetAtoms(new IAtom[] { ta2, ta3 });
            tb2.Order = BondOrder.Single;
            target.Bonds.Add(tb2);

            //Isomorphism check
            bool res = uiTester.IsSubgraph(target, q);
            Assert.IsFalse(res, "C**C should not match SCCS");
        }
    }
}
