/* Copyright (C) 2004-2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Default;
using NCDK.Graphs;
using NCDK.IO;
using NCDK.Isomorphisms;
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Smiles.SMARTS.Parser
{
    /// <summary>
    /// JUnit test routines for the SMARTS substructure search.
    ///
    // @author Dazhi Jiao
    // @cdk.module test-smarts
    // @cdk.require ant1.6
    /// </summary>
    [TestClass()]
    public class SMARTSSearchTest : CDKTestCase
    {
        private UniversalIsomorphismTester uiTester = new UniversalIsomorphismTester();

        internal static IAtomContainer CreateFromSmiles(string smiles)
        {
            return CreateFromSmiles(smiles, false);
        }

        internal static IAtomContainer SmilesAtomTyped(string smiles)
        {
            IAtomContainer molecule = CreateFromSmiles(smiles, false);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            return molecule;
        }

        internal static IAtomContainer CreateFromSmiles(string smiles, bool perserveAromaticity)
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            sp.Kekulise(!perserveAromaticity);
            return sp.ParseSmiles(smiles);
        }

        internal static SMARTSQueryTool CreateFromSmarts(string smarts)
        {
            SMARTSQueryTool sqt = new SMARTSQueryTool(smarts, Default.ChemObjectBuilder.Instance);
            return sqt;
        }

        internal static int[] Match(SMARTSQueryTool sqt, IAtomContainer m)
        {
            bool status = sqt.Matches(m);
            if (status)
            {
                return new int[] { sqt.MatchesCount, sqt.GetUniqueMatchingAtoms().Count };
            }
            else
            {
                return new int[] { 0, 0 };
            }
        }

        internal static int[] Match(string smarts, string smiles)
        {
            return Match(CreateFromSmarts(smarts), CreateFromSmiles(smiles));
        }

        [TestMethod()]
        public void TestMoleculeFromSDF()
        {
            string filename = "cnssmarts.sdf";
            var ins = ResourceLoader.GetAsStream(GetType(), filename);
            DefaultChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content;
            content = (ChemFile)reader.Read((ChemObject)new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer atomContainer = cList[0];

            SMARTSQueryTool sqt = new SMARTSQueryTool("[NX3;h1,h2,H1,H2;!$(NC=O)]", Default.ChemObjectBuilder.Instance);
            bool status = sqt.Matches(atomContainer);
            Assert.AreEqual(true, status);

            int nmatch = sqt.MatchesCount;
            int nqmatch = sqt.GetUniqueMatchingAtoms().Count;

            Assert.AreEqual(3, nmatch);
            Assert.AreEqual(3, nqmatch);

            sqt.Smarts = "[ND3]";
            status = sqt.Matches(atomContainer);
            Assert.AreEqual(false, status);
        }

        [TestMethod()]
        public void TestRGraphBond()
        {
            var query = SMARTSParser.Parse("CC=O", Default.ChemObjectBuilder.Instance);
            Debug.WriteLine("Query c:c: " + query.ToString());
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer atomContainer = sp.ParseSmiles("CCC=O"); // benzene, aromatic
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestAromaticBond()
        {
            var query = SMARTSParser.Parse("c:c", Default.ChemObjectBuilder.Instance);
            Debug.WriteLine("Query c:c: " + query.ToString());
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            sp.Kekulise(false);
            IAtomContainer atomContainer = sp.ParseSmiles("c1ccccc1"); // benzene, aromatic
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("C1CCCCC1"); // hexane, not aromatic
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestSingleBond()
        {
            var query = SMARTSParser.Parse("C-C", Default.ChemObjectBuilder.Instance);
            Debug.WriteLine("Query C-C: " + query.ToString());
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer atomContainer = sp.ParseSmiles("CCC");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("C=C");
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("C#C");
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestDoubleBond()
        {
            var query = SMARTSParser.Parse("C=C", Default.ChemObjectBuilder.Instance);
            Debug.WriteLine("Query C=C: " + query.ToString());
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer atomContainer = sp.ParseSmiles("CCC");
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("C=C");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("C#C");
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestTripleBond()
        {
            var query = SMARTSParser.Parse("C#C", Default.ChemObjectBuilder.Instance);
            Debug.WriteLine("Query C#C: " + query.ToString());
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer atomContainer = sp.ParseSmiles("CCC");
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("C=C");
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("C#C");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestAnyOrderBond()
        {
            var query = SMARTSParser.Parse("C~C", Default.ChemObjectBuilder.Instance);
            Debug.WriteLine("Query C~C: " + query.ToString());
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer atomContainer = sp.ParseSmiles("CCC");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("C=C");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("C#C");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestAnyAtom()
        {
            var query = SMARTSParser.Parse("C*C", Default.ChemObjectBuilder.Instance);
            Debug.WriteLine("Query C*C: " + query.ToString());
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer atomContainer = sp.ParseSmiles("CCC");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("CNC");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("CCN");
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestAliphaticAtom()
        {
            var query = SMARTSParser.Parse("CAC", Default.ChemObjectBuilder.Instance);
            Debug.WriteLine("Query CAC: " + query.ToString());
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer atomContainer = sp.ParseSmiles("CCC");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("CNC");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("c1ccccc1"); // benzene, aromatic
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestAromaticAtom()
        {
            var query = SMARTSParser.Parse("aaa", Default.ChemObjectBuilder.Instance);
            Debug.WriteLine("Query CaC: " + query.ToString());
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            sp.Kekulise(false);

            IAtomContainer atomContainer = sp.ParseSmiles("CCC");
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("c1ccccc1"); // benzene, aromatic
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));
        }

        [TestMethod()]
        public void TestSymbolQueryAtom()
        {
            var query = SMARTSParser.Parse("CCC", Default.ChemObjectBuilder.Instance);
            Debug.WriteLine("Query CAC: " + query.ToString());
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);

            IAtomContainer atomContainer = sp.ParseSmiles("CCC");
            Assert.IsTrue(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("CNC");
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));

            atomContainer = sp.ParseSmiles("c1ccccc1"); // benzene, aromatic
            Assert.IsFalse(uiTester.IsSubgraph(atomContainer, query));
        }

        /// <summary>
        /// From http://www.daylight.com/dayhtml_tutorials/languages/smarts/index.html
        /// </summary>
        [TestMethod()]
        public void TestPropertyCharge1()
        {
            int[] results = Match("[+1]", "[OH-].[Mg+2]");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestPropertyCharge2()
        {
            int[] results = Match("[+1]", "COCC(O)Cn1ccnc1[N+](=O)[O-]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyCharge3()
        {
            int[] results = Match("[+1]", "[NH4+]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyCharge4()
        {
            int[] results = Match("[+1]", "CN1C(=O)N(C)C(=O)C(N(C)C=N2)=C12");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestPropertyCharge5()
        {
            int[] results = Match("[+1]", "[Cl-].[Cl-].NC(=O)c2cc[n+](COC[n+]1ccccc1C=NO)cc2");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAromatic1()
        {
            int[] results = Match("[a]", "c1cc(C)c(N)cc1");
            Assert.AreEqual(6, results[0]);
            Assert.AreEqual(6, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAromatic2()
        {
            int[] results = Match("[a]", "c1c(C)c(N)cnc1");
            Assert.AreEqual(6, results[0]);
            Assert.AreEqual(6, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAromatic3()
        {
            int[] results = Match("[a]", "c1(C)c(N)cco1");
            Assert.AreEqual(5, results[0]);
            Assert.AreEqual(5, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAromatic4()
        {
            int[] results = Match("[a]", "c1c(C)c(N)c[nH]1");
            Assert.AreEqual(5, results[0]);
            Assert.AreEqual(5, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAromatic5()
        {
            int[] results = Match("[a]", "O=n1ccccc1");
            Assert.AreEqual(6, results[0]);
            Assert.AreEqual(6, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAromatic6()
        {
            int[] results = Match("[a]", "[O-][n+]1ccccc1");
            Assert.AreEqual(6, results[0]);
            Assert.AreEqual(6, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAromatic7()
        {
            int[] results = Match("[a]", "c1ncccc1C1CCCN1C");
            Assert.AreEqual(6, results[0]);
            Assert.AreEqual(6, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAromatic8()
        {
            int[] results = Match("[a]", "c1ccccc1C(=O)OC2CC(N3C)CCC3C2C(=O)OC");
            Assert.AreEqual(6, results[0]);
            Assert.AreEqual(6, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAliphatic1()
        {
            int[] results = Match("[A]", "c1cc(C)c(N)cc1");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAliphatic2()
        {
            int[] results = Match("[A]", "CCO");
            Assert.AreEqual(3, results[0]);
            Assert.AreEqual(3, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAliphatic3()
        {
            int[] results = Match("[A]", "C=CC=CC=C");
            Assert.AreEqual(6, results[0]);
            Assert.AreEqual(6, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAliphatic4()
        {
            int[] results = Match("[A]", "CC(C)(C)C");
            Assert.AreEqual(5, results[0]);
            Assert.AreEqual(5, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAliphatic5()
        {
            int[] results = Match("[A]", "CCN(CC)C(=O)C1CN(C)C2CC3=CNc(ccc4)c3c4C2=C1");
            Assert.AreEqual(15, results[0]);
            Assert.AreEqual(15, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAliphatic6()
        {
            int[] results = Match("[A]", "N12CCC36C1CC(C(C2)=CCOC4CC5=O)C4C3N5c7ccccc76");
            Assert.AreEqual(19, results[0]);
            Assert.AreEqual(19, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicNumber1()
        {
            int[] results = Match("[#6]", "c1cc(C)c(N)cc1");
            Assert.AreEqual(7, results[0]);
            Assert.AreEqual(7, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicNumber2()
        {
            int[] results = Match("[#6]", "CCO");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicNumber3()
        {
            int[] results = Match("[#6]", "C=CC=CC=C-O");
            Assert.AreEqual(6, results[0]);
            Assert.AreEqual(6, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicNumber4()
        {
            int[] results = Match("[#6]", "CC(C)(C)C");
            Assert.AreEqual(5, results[0]);
            Assert.AreEqual(5, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicNumber5()
        {
            int[] results = Match("[#6]", "COc1cc2c(ccnc2cc1)C(O)C4CC(CC3)C(C=C)CN34");
            Assert.AreEqual(20, results[0]);
            Assert.AreEqual(20, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicNumber6()
        {
            int[] results = Match("[#6]", "C123C5C(O)C=CC2C(N(C)CC1)Cc(ccc4O)c3c4O5");
            Assert.AreEqual(17, results[0]);
            Assert.AreEqual(17, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicNumber7()
        {
            int[] results = Match("[#6]", "C123C5C(OC(=O)C)C=CC2C(N(C)CC1)Cc(ccc4OC(=O)C)c3c4O5");
            Assert.AreEqual(21, results[0]);
            Assert.AreEqual(21, results[1]);
        }

        /// <summary>
        // @cdk.bug 2686473
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestPropertyAtomicNumber8()
        {
            int[] results = Match("[#16]", "COC1C(C(C(C(O1)CO)OC2C(C(C(C(O2)CO)S)O)O)O)O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        /// <summary>
        // @cdk.bug 2686473
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestPropertyAtomicNumber9()
        {
            int[] results = Match("[#6]", "[*]");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestPropertyR1()
        {
            int[] results = Match("[R2]", "N12CCC36C1CC(C(C2)=CCOC4CC5=O)C4C3N5c7ccccc76");
            Assert.AreEqual(7, results[0]);
            Assert.AreEqual(7, results[1]);
        }

        [TestMethod()]
        public void TestPropertyR2()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[R2]");
            sqt.UseSmallestSetOfSmallestRings(); // default for daylight
            int[] results = Match(sqt, CreateFromSmiles("COc1cc2c(ccnc2cc1)C(O)C4CC(CC3)C(C=C)CN34"));
            Assert.AreEqual(6, results[0]);
            Assert.AreEqual(6, results[1]);

        }

        //@Ignore("This feature was removed - essential rings aren't useful really")
        //[TestMethod()]
        public void TestPropertyR2_essentialRings()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[R2]");
            sqt.UseEssentialRings();
            int[] results = Match(sqt, CreateFromSmiles("COc1cc2c(ccnc2cc1)C(O)C4CC(CC3)C(C=C)CN34"));
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        //@Ignore("This feature is pending but will be the combinded in an 'OpenSMARTS'"
        //        + " configuration which uses the relevant rings.")
        //[TestMethod()]
        public void TestPropertyR2_relevantRings()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[R2]");
            sqt.UseRelevantRings();
            int[] results = Match(sqt, CreateFromSmiles("COc1cc2c(ccnc2cc1)C(O)C4CC(CC3)C(C=C)CN34"));
            Assert.AreEqual(8, results[0]);
            Assert.AreEqual(8, results[1]);
        }

        [TestMethod()]
        public void TestPropertyR3()
        {
            int[] results = Match("[R2]", "C123C5C(O)C=CC2C(N(C)CC1)Cc(ccc4O)c3c4O5");
            Assert.AreEqual(4, results[0]);
            Assert.AreEqual(4, results[1]);
        }

        [TestMethod()]
        public void TestPropertyR4()
        {
            int[] results = Match("[R2]", "C123C5C(OC(=O)C)C=CC2C(N(C)CC1)Cc(ccc4OC(=O)C)c3c4O5");
            Assert.AreEqual(4, results[0]);
            Assert.AreEqual(4, results[1]);
        }

        [TestMethod()]
        public void TestPropertyR5()
        {
            int[] results = Match("[R2]", "C1C(C)=C(C=CC(C)=CC=CC(C)=CCO)C(C)(C)C1");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestPropertyr1()
        {
            int[] results = Match("[r5]", "N12CCC36C1CC(C(C2)=CCOC4CC5=O)C4C3N5c7ccccc76");
            Assert.AreEqual(9, results[0]);
            Assert.AreEqual(9, results[1]);
        }

        [TestMethod()]
        public void TestPropertyr2()
        {
            int[] results = Match("[r5]", "COc1cc2c(ccnc2cc1)C(O)C4CC(CC3)C(C=C)CN34");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestPropertyr3()
        {
            int[] results = Match("[r5]", "C123C5C(O)C=CC2C(N(C)CC1)Cc(ccc4O)c3c4O5");
            Assert.AreEqual(5, results[0]);
            Assert.AreEqual(5, results[1]);
        }

        [TestMethod()]
        public void TestPropertyr4()
        {
            int[] results = Match("[r5]", "C123C5C(OC(=O)C)C=CC2C(N(C)CC1)Cc(ccc4OC(=O)C)c3c4O5");
            Assert.AreEqual(5, results[0]);
            Assert.AreEqual(5, results[1]);
        }

        [TestMethod()]
        public void TestPropertyr5()
        {
            int[] results = Match("[r5]", "C1C(C)=C(C=CC(C)=CC=CC(C)=CCO)C(C)(C)C1");
            Assert.AreEqual(5, results[0]);
            Assert.AreEqual(5, results[1]);
        }

        [TestMethod()]
        public void QuadBond()
        {
            int[] results = Match("*$*", "[Re]$[Re]");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyValence1()
        {
            int[] results = Match("[v4]", "C");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyValence2()
        {
            int[] results = Match("[v4]", "CCO");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestPropertyValence3()
        {
            int[] results = Match("[v4]", "[NH4+]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyValence4()
        {
            int[] results = Match("[v4]", "CC1(C)SC2C(NC(=O)Cc3ccccc3)C(=O)N2C1C(=O)O");
            Assert.AreEqual(16, results[0]);
            Assert.AreEqual(16, results[1]);
        }

        [TestMethod()]
        public void TestPropertyValence5()
        {
            int[] results = Match("[v4]", "[Cl-].[Cl-].NC(=O)c2cc[n+](COC[n+]1ccccc1C=NO)cc2");
            Assert.AreEqual(16, results[0]);
            Assert.AreEqual(16, results[1]);
        }

        [TestMethod()]
        public void TestPropertyX1()
        {
            int[] results = Match("[X2]", "CCO");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyX2()
        {
            int[] results = Match("[X2]", "O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyX3()
        {
            int[] results = Match("[X2]", "CCC(=O)CC");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestPropertyX4()
        {
            int[] results = Match("[X2]", "FC(Cl)=C=C(Cl)F");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyX5()
        {
            int[] results = Match("[X2]", "COc1cc2c(ccnc2cc1)C(O)C4CC(CC3)C(C=C)CN34");
            Assert.AreEqual(3, results[0]);
            Assert.AreEqual(3, results[1]);
        }

        [TestMethod()]
        public void TestPropertyX6()
        {
            int[] results = Match("[X2]", "C123C5C(O)C=CC2C(N(C)CC1)Cc(ccc4O)c3c4O5");
            Assert.AreEqual(3, results[0]);
            Assert.AreEqual(3, results[1]);
        }

        [TestMethod()]
        public void TestPropertyD1()
        {
            int[] results = Match("[D2]", "CCO");
            Assert.AreEqual(1, results[0]);
        }

        [TestMethod()]
        public void TestPropertyD2()
        {
            int[] results = Match("[D2]", "O");
            Assert.AreEqual(0, results[0]);
        }

        [TestMethod()]
        public void TestPropertyD3()
        {
            int[] results = Match("[D2]", "CCC(=O)CC");
            Assert.AreEqual(2, results[0]);
        }

        [TestMethod()]
        public void TestPropertyD4()
        {
            int[] results = Match("[D2]", "FC(Cl)=C=C(Cl)F");
            Assert.AreEqual(1, results[0]);
        }

        [TestMethod()]
        public void TestPropertyD5()
        {
            int[] results = Match("[D2]", "COc1cc2c(ccnc2cc1)C(O)C4CC(CC3)C(C=C)CN34");
            Assert.AreEqual(12, results[0]);
        }

        [TestMethod()]
        public void TestPropertyD6()
        {
            int[] results = Match("[D2]", "C123C5C(O)C=CC2C(N(C)CC1)Cc(ccc4O)c3c4O5");
            Assert.AreEqual(8, results[0]);
        }

        /// <summary>
        // @throws Exception
        // @cdk.bug 2489417
        /// </summary>
        [TestMethod()]
        public void TestPropertyD7()
        {
            int[] results = Match("[ND3]", "CCN([H])([H])");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        /// <summary>
        // @throws Exception
        // @cdk.bug 2489417
        /// </summary>
        [TestMethod()]
        public void TestPropertyD8()
        {
            int[] results = Match("[OD1]", "CO[H]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        /// <summary>
        // @throws Exception
        // @cdk.bug 2489417
        /// </summary>
        [TestMethod()]
        public void TestPropertyD9()
        {
            int[] results;

            results = Match("[OD1H]", "CO");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        /// <summary>
        // @throws Exception
        // @cdk.bug 2489417
        /// </summary>
        [TestMethod()]
        public void TestPropertyD10()
        {
            int[] results;

            results = Match("[OD1H]", "CO[H]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        /// <summary>
        // @throws Exception
        // @cdk.bug 2489417
        /// </summary>
        [TestMethod()]
        public void TestPropertyD11()
        {
            int[] results;

            results = Match("[OD1H]-*", "CCO");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        /// <summary>
        /// With '*' matching 'H', this smarts matches twice 'OC' and 'O[H]'.
        // @cdk.bug 2489417
        /// </summary>
        [TestMethod()]
        public void TestPropertyD12()
        {
            int[] results;

            results = Match("[OD1H]-*", "CCO[H]");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);

        }

        [TestMethod()]
        public void TestPropertyHAtom1()
        {
            int[] results = Match("[H]", "[H+].[Cl-]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyHAtom2()
        {
            int[] results = Match("[H]", "[2H]");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestPropertyHAtom3()
        {
            int[] results = Match("[H]", "[H][H]");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestPropertyHAtom4()
        {
            int[] results = Match("[H]", "[CH4]");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestPropertyHAtom5()
        {
            int[] results = Match("[H]", "[H]C([H])([H])[H]");
            Assert.AreEqual(4, results[0]);
            Assert.AreEqual(4, results[1]);
        }

        [TestMethod()]
        public void TestPropertyHTotal1()
        {
            int[] results = Match("[H1]", "CCO");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyHTotal2()
        {
            int[] results = Match("[H1]", "[2H]C#C");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestPropertyHTotal3()
        {
            int[] results = Match("[H1]", "[H]C(C)(C)C");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyHTotal4()
        {
            int[] results = Match("[H1]", "COc1cc2c(ccnc2cc1)C(O)C4CC(CC3)C(C=C)CN34");
            Assert.AreEqual(11, results[0]);
            Assert.AreEqual(11, results[1]);
        }

        [TestMethod()]
        public void TestPropertyHTotal5()
        {
            int[] results = Match("[H1]", "C123C5C(O)C=CC2C(N(C)CC1)Cc(ccc4O)c3c4O5");
            Assert.AreEqual(10, results[0]);
            Assert.AreEqual(10, results[1]);
        }

        [TestMethod()]
        public void TestPropertyHTotal6()
        {
            int[] results = Match("[H1]", "[H][H]");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAnyAtom1()
        {
            int[] results = Match("[*]", "C");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAnyAtom2()
        {
            int[] results = Match("[*]", "[2H]C");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAnyAtom3()
        {
            int[] results = Match("[*]", "[1H][1H]");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAnyAtom4()
        {
            int[] results = Match("[*]", "[1H]C([1H])([1H])[1H]");
            Assert.AreEqual(5, results[0]);
            Assert.AreEqual(5, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAnyAtom5()
        {
            int[] results = Match("[*]", "[H][H]");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        /// <summary>
        // @throws Exception
        // @cdk.bug 2489533
        /// </summary>
        [TestMethod()]
        public void TestPropertyAnyAtom6()
        {
            int[] result = Match("*", "CO");
            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(2, result[1]);
        }

        /// <summary>
        /// Bug was mistaken - '*' does match explicit H but in DEPICTMATCH H's are
        /// suppressed by default.
        ///
        // @throws Exception
        // @cdk.bug 2489533
        /// </summary>
        [TestMethod()]
        public void TestPropertyAnyAtom7()
        {
            int[] result = Match("*", "CO[H]");
            Assert.AreEqual(3, result[0]);
            Assert.AreEqual(3, result[1]);
        }

        /// <summary>
        /// Bug was mistaken - '*' does match explicit H but in DEPICTMATCH H's are
        /// suppressed by default.
        ///
        // @throws Exception
        // @cdk.bug 2489533
        /// </summary>
        [TestMethod()]
        public void TestPropertyAnyAtom8()
        {
            int[] result = Match("*", "[H]C([H])([H])[H]");
            Assert.AreEqual(5, result[0]);
            Assert.AreEqual(5, result[1]);
        }

        /// <summary>
        /// Bug was mistaken - '*' does match explicit H but in DEPICTMATCH H's are
        /// suppressed by default.
        ///
        // @throws Exception
        // @cdk.bug 2489533
        /// </summary>
        [TestMethod()]
        public void TestPropertyAnyAtom9()
        {
            int[] result = Match("*", "CCCC([2H])[H]");
            Assert.AreEqual(6, result[0]);
            Assert.AreEqual(6, result[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicMass1()
        {
            int[] results = Match("[13C]", "[13C]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicMass2()
        {
            int[] results = Match("[13C]", "[C]");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicMass3()
        {
            int[] results = Match("[13*]", "[13C]Cl");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicMass4()
        {
            int[] results = Match("[12C]", "CCl");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        /// <summary>
        // @cdk.bug 2490336
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestPropertyAtomicMass5()
        {
            int[] results = Match("[2H]", "CCCC([2H])[H]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicMass6()
        {
            int[] results = Match("[H]", "CCCC([2H])[H]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPropertyAtomicMass7()
        {
            int[] results = Match("[3H]", "CCCC([2H])([3H])[3H]");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestBondSingle1()
        {
            int[] results = Match("CC", "C=C");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestBondSingle2()
        {
            int[] results = Match("CC", "C#C");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestBondSingle3()
        {
            int[] results = Match("CC", "CCO");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestBondSingle4()
        {
            int[] results = Match("CC", "C1C(C)=C(C=CC(C)=CC=CC(C)=CCO)C(C)(C)C1");
            Assert.AreEqual(28, results[0]);
            Assert.AreEqual(14, results[1]);
        }

        [TestMethod()]
        public void TestBondSingle5()
        {
            int[] results = Match("CC", "CC1(C)SC2C(NC(=O)Cc3ccccc3)C(=O)N2C1C(=O)O");
            Assert.AreEqual(14, results[0]);
            Assert.AreEqual(7, results[1]);
        }

        [TestMethod()]
        public void TestBondAny1()
        {
            int[] results = Match("C~C", "C=C");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestBondAny2()
        {
            int[] results = Match("C~C", "C#C");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestBondAny3()
        {
            int[] results = Match("C~C", "CCO");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestBondAny4()
        {
            int[] results = Match("C~C", "C1C(C)=C(C=CC(C)=CC=CC(C)=CCO)C(C)(C)C1");
            Assert.AreEqual(38, results[0]);
            Assert.AreEqual(19, results[1]);
        }

        [TestMethod()]
        public void TestBondAny5()
        {
            int[] results = Match("[C,c]~[C,c]", "CC1(C)SC2C(NC(=O)Cc3ccccc3)C(=O)N2C1C(=O)O");
            Assert.AreEqual(28, results[0]);
            Assert.AreEqual(14, results[1]);
        }

        [TestMethod()]
        public void TestBondRing1()
        {
            int[] results = Match("C@C", "C=C");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestBondRing2()
        {
            int[] results = Match("C@C", "C#C");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestBondRing3()
        {
            int[] results = Match("C@C", "C1CCCCC1");
            Assert.AreEqual(12, results[0]);
            Assert.AreEqual(6, results[1]);
        }

        [TestMethod()]
        public void TestBondRing4()
        {
            int[] results = Match("[C,c]@[C,c]", "c1ccccc1Cc1ccccc1");
            Assert.AreEqual(24, results[0]);
            Assert.AreEqual(12, results[1]);
        }

        [TestMethod()]
        public void TestBondRing5()
        {
            int[] results = Match("[C,c]@[C,c]", "CCN(CC)C(=O)C1CN(C)C2CC3=CNc(ccc4)c3c4C2=C1");
            Assert.AreEqual(30, results[0]);
            Assert.AreEqual(15, results[1]);
        }

        [TestMethod()]
        public void TestBondRing6()
        {
            int[] results = Match("[C,c]@[C,c]", "N12CCC36C1CC(C(C2)=CCOC4CC5=O)C4C3N5c7ccccc76");
            Assert.AreEqual(44, results[0]);
            Assert.AreEqual(22, results[1]);
        }

        [TestMethod()]
        public void TestBondStereo1()
        {
            int[] results = Match("F/?C=C/Cl", "F/C=C/Cl");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestBondStereo2()
        {
            int[] results = Match("F/?C=C/Cl", "FC=C/Cl");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestBondStereo3()
        {
            int[] results = Match("F/?C=C/Cl", "FC=CCl");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestBondStereo4()
        {
            int[] results = Match("F/?C=C/Cl", "F\\C=C/Cl");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestLogicalNot1()
        {
            int[] results = Match("[!c]", "c1cc(C)c(N)cc1");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestLogicalNot2()
        {
            int[] results = Match("[!c]", "c1c(C)c(N)cnc1");
            Assert.AreEqual(3, results[0]);
            Assert.AreEqual(3, results[1]);
        }

        [TestMethod()]
        public void TestLogicalNot3()
        {
            int[] results = Match("[!c]", "c1(C)c(N)cco1");
            Assert.AreEqual(3, results[0]);
            Assert.AreEqual(3, results[1]);
        }

        [TestMethod()]
        public void TestLogicalNot4()
        {
            int[] results = Match("[!c]", "c1c(C)c(N)c[nH]1");
            Assert.AreEqual(3, results[0]);
            Assert.AreEqual(3, results[1]);
        }

        [TestMethod()]
        public void TestLogicalNot5()
        {
            int[] results = Match("[!c]", "O=n1ccccc1");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestLogicalNot6()
        {
            int[] results = Match("[!c]", "[O-][n+]1ccccc1");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestLogicalNot7()
        {
            int[] results = Match("[!c]", "c1ncccc1C1CCCN1C");
            Assert.AreEqual(7, results[0]);
            Assert.AreEqual(7, results[1]);
        }

        [TestMethod()]
        public void TestLogicalNot8()
        {
            int[] results = Match("[!c]", "c1ccccc1C(=O)OC2CC(N3C)CCC3C2C(=O)OC");
            Assert.AreEqual(16, results[0]);
            Assert.AreEqual(16, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr1()
        {
            int[] results = Match("[N,O,o]", "c1cc(C)c(N)cc1");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr2()
        {
            int[] results = Match("[N,O,o]", "c1c(C)c(N)cnc1");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr3()
        {
            int[] results = Match("[N,O,o]", "c1(C)c(N)cco1");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr4()
        {
            int[] results = Match("[N,O,o]", "c1c(C)c(N)c[nH]1");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr5()
        {
            int[] results = Match("[N,O,o]", "O=n1ccccc1");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr6()
        {
            int[] results = Match("[N,O,o]", "[O-][n+]1ccccc1");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr7()
        {
            int[] results = Match("[N,O,o]", "c1ncccc1C1CCCN1C");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr8()
        {
            int[] results = Match("[N,O,o]", "c1ccccc1C(=O)OC2CC(N3C)CCC3C2C(=O)OC");
            Assert.AreEqual(5, results[0]);
            Assert.AreEqual(5, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr9()
        {
            int[] results = Match("[N]=[N]-,=[N]", "CCCC(=O)C=C");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr10()
        {
            int[] results = Match("[N;$([N!X4])]!@;-[N;$([N!X4])]", "CCCC(=O)C=C");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr11()
        {
            int[] results = Match("[#6]!:;=[#6][#6](=O)[!O]", "CCCC(=O)C=C");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOr12()
        {
            int[] results = Match("C=,#C", "C=CCC#C");
            Assert.AreEqual(4, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrHighAnd1()
        {
            int[] results = Match("[N,#6&+1,+0]", "CCN(CC)C(=O)C1CN(C)C2CC3=CNc(ccc4)c3c4C2=C1");
            Assert.AreEqual(24, results[0]);
            Assert.AreEqual(24, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrHighAnd2()
        {
            int[] results = Match("[N,#6&+1,+0]", "N12CCC36C1CC(C(C2)=CCOC4CC5=O)C4C3N5c7ccccc76");
            Assert.AreEqual(25, results[0]);
            Assert.AreEqual(25, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrHighAnd3()
        {
            int[] results = Match("[N,#6&+1,+0]", "COc1cc2c(ccnc2cc1)C(O)C4CC(CC3)C(C=C)CN34");
            Assert.AreEqual(24, results[0]);
            Assert.AreEqual(24, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrHighAnd4()
        {
            int[] results = Match("[N,#6&+1,+0]", "C123C5C(O)C=CC2C(N(C)CC1)Cc(ccc4O)c3c4O5");
            Assert.AreEqual(21, results[0]);
            Assert.AreEqual(21, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrHighAnd5()
        {
            int[] results = Match("[N,#6&+1,+0]", "N1N([Hg-][O+]=C1N=Nc2ccccc2)c3ccccc3");
            Assert.AreEqual(17, results[0]);
            Assert.AreEqual(17, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrHighAnd6()
        {
            int[] results = Match("[N,#6&+1,+0]", "[Na+].[Na+].[O-]C(=O)c1ccccc1c2c3ccc([O-])cc3oc4cc(=O)ccc24");
            Assert.AreEqual(23, results[0]);
        }

        [TestMethod()]
        public void TestLogicalOrHighAnd7()
        {
            int[] results = Match("[N,#6&+1,+0]", "[Cl-].Clc1ccc([I+]c2cccs2)cc1");
            Assert.AreEqual(12, results[0]);
            Assert.AreEqual(12, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrLowAnd1()
        {
            int[] results = Match("[#7,C;+0,+1]", "CCN(CC)C(=O)C1CN(C)C2CC3=CNc(ccc4)c3c4C2=C1");
            Assert.AreEqual(15, results[0]);
            Assert.AreEqual(15, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrLowAnd2()
        {
            int[] results = Match("[#7,C;+0,+1]", "N12CCC36C1CC(C(C2)=CCOC4CC5=O)C4C3N5c7ccccc76");
            Assert.AreEqual(17, results[0]);
            Assert.AreEqual(17, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrLowAnd3()
        {
            int[] results = Match("[#7,C;+0,+1]", "COc1cc2c(ccnc2cc1)C(O)C4CC(CC3)C(C=C)CN34");
            Assert.AreEqual(13, results[0]);
            Assert.AreEqual(13, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrLowAnd4()
        {
            int[] results = Match("[#7,C;+0,+1]", "C123C5C(O)C=CC2C(N(C)CC1)Cc(ccc4O)c3c4O5");
            Assert.AreEqual(12, results[0]);
            Assert.AreEqual(12, results[1]);
        }

        [TestMethod()]
        public void TestLogicalOrLowAnd5()
        {
            int[] results = Match("[#7,C;+0,+1]", "N1N([Hg-][O+]=C1N=Nc2ccccc2)c3ccccc3");
            Assert.AreEqual(5, results[0]);
            Assert.AreEqual(5, results[1]);
        }

        /// <summary> The CDK aromaticity detection differs from Daylight - by persevering
        ///  aromaticity from the SMILES we can match correctly.  */
        [TestMethod()]
        public void TestLogicalOrLowAnd6()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[#7,C;+0,+1]");
            IAtomContainer smi = CreateFromSmiles("[Na+].[Na+].[O-]C(=O)c1ccccc1c2c3ccc([O-])cc3oc4cc(=O)ccc24");
            int[] results = Match(sqt, smi);
            Assert.AreEqual(1, results[0]);
        }

        [TestMethod()]
        public void TestLogicalOrLowAnd6_cdkAromaticity()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[#7,C;+0,+1]");
            IAtomContainer smi = CreateFromSmiles("[Na+].[Na+].[O-]C(=O)c1ccccc1c2c3ccc([O-])cc3oc4cc(=O)ccc24");
            sqt.SetAromaticity(new Aromaticity(ElectronDonation.Cdk(), Cycles.CDKAromaticSet));
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(smi);
            int[] results = Match(sqt, smi);
            Assert.AreEqual(8, results[0]);
        }

        [TestMethod()]
        public void TestLogicalOrLowAnd7()
        {
            int[] results = Match("[#7,C;+0,+1]", "[Cl-].Clc1ccc([I+]c2cccs2)cc1");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestRing1()
        {
            int[] results = Match("C1CCCCC1", "C1CCCCC1CCCC");
            Assert.AreEqual(12, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestRing2()
        {
            int[] results = Match("C1CCCCC1", "C1CCCCC1C1CCCCC1");
            Assert.AreEqual(24, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestRing3()
        {
            int[] results = Match("C1CCCCC1", "C1CCCC12CCCCC2");
            Assert.AreEqual(12, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestRing4()
        {
            int[] results = Match("C1CCCCC1", "c1ccccc1O");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestRing5()
        {
            int[] results = Match("C1CCCCC1", "c1ccccc1CCCCCC");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestRing_large()
        {
            int[] results = Match("C%10CCCCC%10", "C1CCCCC1O");
            Assert.AreEqual(12, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestRing_large2()
        {
            int[] results = Match("C%99CCCCC%99", "C1CCCCC1O");
            Assert.AreEqual(12, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestRing_large3()
        {
            int[] results = Match("C%991CCCCC%99CCCC1", "C12CCCCC2CCCC1");
            Assert.AreEqual(4, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestRing6()
        {
            int[] results = Match("C1CCCCC1", "CCCCCC");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestAromaticRing1()
        {
            int[] results = Match("c1ccccc1", "c1ccccc1");
            Assert.AreEqual(12, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAromaticRing2()
        {
            int[] results = Match("c1ccccc1", "c1cccc2c1cccc2");
            Assert.AreEqual(24, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestAromaticRing3()
        {
            int[] results = Match("c1ccccn1", "c1cccc2c1cccc2");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestAromaticRing4()
        {
            int[] results = Match("c1ccccn1", "c1cccc2c1cccn2");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid1()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(C)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid2()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CCCNC(N)=N)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid3()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CC(N)=O)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid4()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CC(O)=O)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid5()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CS)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid6()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CCC(N)=O)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid7()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CCC(O)=O)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid8()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC([H])C(O)=O");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid9()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CC1=CNC=N1)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid10()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(C(CC)C)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid11()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CC(C)C)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid12()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CCCCN)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid13()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CCSC)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid14()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CC1=CC=CC=C1)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid15()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "OC(C1CCCN1)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid16()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CO)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid17()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(C(C)O)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid18()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CC1=CNC2=C1C=CC=C2)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid19()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(CC1=CC=C(O)C=C1)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestAminoAcid20()
        {
            int[] results = Match("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]", "NC(C(C)C)C(O)=O");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestCyclicUreas()
        {
            int[] results = Match("[$(C1CNC(=O)N1)]", "N1C(=O)NCC1");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        /// <summary>
        // @throws Exception
        // @cdk.bug 1967468
        /// </summary>
        [TestMethod()]
        public void TestAcyclicUreas()
        {
            int[] results = Match("[$(CC);$(C1CNC(=O)N1)]", "C1CC1NC(=O)Nc2ccccc2");
            //        int[] results = Match("[$([CR][NR][CR](=O)[NR])]", "C1CC1NC(=O)Nc2ccccc2");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        /// <summary>
        // @cdk.bug 1985811
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestIndoleAgainstIndole()
        {
            int[] results = Match("c1ccc2cc[nH]c2(c1)", "C1(NC=C2)=C2C=CC=C1");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);

            results = Match("c1ccc2cc[nH]c2(c1)", "c1ccc2cc[nH]c2(c1)");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);

        }

        /// <summary>
        // @cdk.bug 1985811
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestPyridineAgainstPyridine()
        {
            int[] results = Match("c1ccncc1", "c1ccncc1");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);

            results = Match("c1ccncc1", "C1=NC=CC=C1");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestGroup5Elements()
        {
            int[] results = Match("[V,Cr,Mn,Nb,Mo,Tc,Ta,W,Re]", "[W]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestPeriodicGroupNumber()
        {
            int[] results = Match("[G14]", "CCN");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);

            results = Match("[G14,G15]", "CCN");
            Assert.AreEqual(3, results[0]);
            Assert.AreEqual(3, results[1]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidPeriodicGroupNumber()
        {
            Match("[G19]", "CCN");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidPeriodicGroupNumber_2()
        {
            Match("[G0]", "CCN");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidPeriodicGroupNumber_3()
        {
            Match("[G345]", "CCN");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNonPeriodicGroupNumber()
        {
            Match("[G]", "CCN"); // Should throw an exception if G is not followed by a number
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNonPeriodicGroupNumber_2()
        {
            Match("[GA]", "CCN"); // Should throw an exception if G is not followed by a number
        }

        [TestMethod()]
        public void TestNonCHHeavyAtom()
        {
            int[] results = Match("[#X]", "CCN");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);

            results = Match("[#X]", "CCNC(=O)CCSF");
            Assert.AreEqual(4, results[0]);
            Assert.AreEqual(4, results[1]);

            results = Match("C#[#X]", "CCNC(=O)C#N");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);

            results = Match("C#[#X]", "CCNC(=O)C#C");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);

        }

        [TestMethod()]
        public void TestHybridizationNumber()
        {
            int[] results = Match(CreateFromSmarts("[^1]"), SmilesAtomTyped("CCN"));
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);

            results = Match(CreateFromSmarts("[^1]"), SmilesAtomTyped("N#N"));
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);

            results = Match(CreateFromSmarts("[^1&N]"), SmilesAtomTyped("CC#C"));
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);

            results = Match(CreateFromSmarts("[^1&N]"), SmilesAtomTyped("CC#N"));
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);

            results = Match(CreateFromSmarts("[^1&N,^2&C]"), SmilesAtomTyped("CC(=O)CC(=O)CC#N"));
            Assert.AreEqual(3, results[0]);
            Assert.AreEqual(3, results[1]);

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadHybridizationNumber()
        {
            Match("[^]", "CCN"); // Should throw an exception if ^ is not followed by a number
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadHybridizationNumber_2()
        {
            Match("[^X]", "CCN"); // Should throw an exception if ^ is not followed by a number
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadHybridizationNumber_3()
        {
            Match("[^0]", "CCN"); // Should throw an exception if ^ is not followed by a number
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadHybridizationNumber_4()
        {
            Match("[^9]", "CCN"); // Should throw an exception if ^ is not followed by a number
        }

        /// <summary>
        // @cdk.bug  2589807
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestAromAliArom()
        {
            int[] results = Match("c-c", "COC1CN(CCC1NC(=O)C2=CC(=C(C=C2OC)N)Cl)CCCOC3=CC=C(C=C3)F");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);

            IAtomContainer m = CreateFromSmiles("c1ccccc1c2ccccc2");

            // note - missing explicit single bond, SMILES preserves the
            // aromatic specification but in this case we want the single
            // bond. as the molecule as assigned bond orders we can easily
            // remove the flags and reassign them correctly
            foreach (var bond in m.Bonds)
                bond.IsAromatic = false;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);
            Aromaticity.CDKLegacy.Apply(m);

            results = Match(CreateFromSmarts("c-c"), m);
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);

            results = Match("c-c", "c1ccccc1-c1ccccc1");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);

            results = Match("cc", "c1ccccc1-c1ccccc1");
            Assert.AreEqual(26, results[0]);
            Assert.AreEqual(13, results[1]);

            results = Match("cc", "c1ccccc1c2ccccc2");
            Assert.AreEqual(26, results[0]);
            Assert.AreEqual(13, results[1]);
        }

        [TestMethod()]
        public void TestUnspecifiedBond()
        {
            int[] results = Match("CC", "CCc1ccccc1");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(1, results[1]);

            results = Match("[#6][#6]", "CCc1ccccc1");
            Assert.AreEqual(16, results[0]);
            Assert.AreEqual(8, results[1]);

            results = Match("[#6]-[#6]", "CCc1ccccc1");
            Assert.AreEqual(4, results[0]);
            Assert.AreEqual(2, results[1]);

            results = Match("[#6]:[#6]", "CCc1ccccc1");
            Assert.AreEqual(12, results[0]);
            Assert.AreEqual(6, results[1]);

            results = Match("cc", "CCc1ccccc1");
            Assert.AreEqual(12, results[0]);
            Assert.AreEqual(6, results[1]);

            results = Match("c-c", "CCc1ccccc1");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);

            results = Match("c-C", "CCc1ccccc1");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        /// <summary>
        // @throws Exception
        // @cdk.bug 2587204
        /// </summary>
        [TestMethod()]
        public void TestLactamSimple()
        {
            int[] results = Match("[R0][ND3R][CR]=O", "N1(CC)C(=O)CCCC1");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        /// <summary>
        // @throws Exception
        // @cdk.bug 2587204
        /// </summary>
        [TestMethod()]
        public void TestLactamRecursive()
        {
            int[] results = Match("[R0]-[$([NRD3][CR]=O)]", "N1(CC)C(=O)CCCC1");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        [TestMethod()]
        public void TestLactamRecursiveAlternate()
        {
            int[] results = Match("[!R]-[$([NRD3][CR]=O)]", "N1(CC)C(=O)CCCC1");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        /// <summary>
        // @cdk.bug 2898399
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestHydrogen()
        {
            int[] results = Match("[H]", "[H]");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        /// <summary>
        // @cdk.bug 2898399
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestLeadingHydrogen()
        {
            int[] results = Match("[H][C@@]1(CCC(C)=CC1=O)C(C)=C", "[H][C@@]1(CCC(C)=CC1=O)C(C)=C");
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
        }

        /// <summary>
        // @cdk.bug 2871303
        /// <p/>
        /// Note that this test passes, and really indicates that
        /// the SMARTS below is not a correct one for vinylogous
        /// esters
        /// </summary>
        [TestMethod()]
        public void TestVinylogousEster()
        {
            int[] results = Match("[#6X3](=[OX1])[#6X3]=,:[#6X3][#6;!$(C=[O,N,S])]", "c1ccccc1C=O");
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        /// <summary>
        /// Check that bond order query respects aromaticity.
        ///
        // @throws Exception
        /// </summary>
        [TestMethod()]
        public void TestBondOrderQueryKekuleVsSmiles()
        {
            int[] results = Match("[#6]=[#6]", "c1ccccc1c2ccccc2");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);

            results = Match("[#6]=[#6]", "C1=C(C=CC=C1)C2=CC=CC=C2");
            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(0, results[1]);
        }

        [TestMethod()]
        public void TestSubstructureBug20141125()
        {
            int[] results = Match("[#6]S[#6]", "CSCCSCO");
            Assert.AreEqual(4, results[0]);
            Assert.AreEqual(2, results[1]);
        }

        [TestMethod()]
        public void TestSubstructureBug20141125_2()
        {
            int[] results = Match("[#6]S[#6]", "CSCCSC(O)CCCSCC");
            Assert.AreEqual(6, results[0]);
            Assert.AreEqual(3, results[1]);
        }

        /// <summary>
        /// Checks that when no number is specified for ring member ship any ring
        /// atom is matched.
        ///
        // @cdk.bug 1168
        /// </summary>
        [TestMethod()]
        public void UnspecifiedRingMembership()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 0 }, Match("[#6+0&R]=[#6+0&!R]", "C1=C2CCCC2CCC1")));
        }

        [TestMethod()]
        public void Cyclopropane()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 0 }, Match("**(*)*", "C1CC1")));
        }

        [TestMethod()]
        public void ComponentGrouping1()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 0 }, Match("[#8].[#8]", "O")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 1 }, Match("[#8].[#8]", "O=O")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 1 }, Match("[#8].[#8]", "OCCO")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 1 }, Match("[#8].[#8]", "O.CCO")));
        }

        [TestMethod()]
        public void ComponentGrouping2()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 0 }, Match("([#8].[#8])", "O")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 1 }, Match("([#8].[#8])", "O=O")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 1 }, Match("([#8].[#8])", "OCCO")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 0 }, Match("([#8].[#8])", "O.CCO")));
        }

        [TestMethod()]
        public void ComponentGrouping3()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 0 }, Match("([#8]).([#8])", "O")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 0 }, Match("([#8]).([#8])", "O=O")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 0 }, Match("([#8]).([#8])", "OCCO")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 1 }, Match("([#8]).([#8])", "O.CCO")));
        }

        /// <summary>
        /// Ensure a class cast exception is not thrown when matching stereochemistry. 
        // @cdk.bug 1358
        /// </summary>
        [TestMethod()]
        public void Bug1358()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 0 }, Match("[$([*@](~*)(~*)(*)*),$([*@H](*)(*)*),$([*@](~*)(*)*)]", "N#CN/C(=N/CCSCC=1N=CNC1C)NC")));
        }

        /// <summary>
        /// Ensure 'r' without a size is equivalent to !R0 and R.
        // @cdk.bug 1364
        /// </summary>
        [TestMethod()]
        public void Bug1364()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 7, 7 }, Match("[!R0!R1]", "C[C@]12CC3CC([NH2+]CC(=O)NCC4CC4)(C1)C[C@@](C)(C3)C2")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 7, 7 }, Match("[R!R1]", "C[C@]12CC3CC([NH2+]CC(=O)NCC4CC4)(C1)C[C@@](C)(C3)C2")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 7, 7 }, Match("[r!R1]", "C[C@]12CC3CC([NH2+]CC(=O)NCC4CC4)(C1)C[C@@](C)(C3)C2")));
        }
    }
}
