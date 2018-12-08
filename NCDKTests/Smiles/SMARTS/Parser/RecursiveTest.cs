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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Common.Base;
using NCDK.Graphs;
using NCDK.IO.Iterator;
using NCDK.Silent;
using NCDK.Tools.Manipulator;
using System.IO;
using System.Linq;
using static NCDK.Smiles.SMARTS.Parser.SMARTSSearchTest;

namespace NCDK.Smiles.SMARTS.Parser
{
    /// <summary>
    /// Test recursive smarts
    /// </summary>
    // @author Dazhi Jiao
    // @cdk.module test-smarts
    // @cdk.require ant1.6
    [TestClass()]
    public class RecursiveTest : CDKTestCase
    {
        private int nmatch;
        private int nqmatch;

        public void Match(string smarts, string smiles)
        {
            SMARTSQueryTool sqt = new SMARTSQueryTool(smarts, ChemObjectBuilder.Instance);
            var sp = CDK.SmilesParser;
            var atomContainer = sp.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(atomContainer);
            Aromaticity.CDKLegacy.Apply(atomContainer);
            bool status = sqt.Matches(atomContainer);
            if (status)
            {
                nmatch = sqt.MatchesCount;
                nqmatch = sqt.GetUniqueMatchingAtoms().Count();
            }
            else
            {
                nmatch = 0;
                nqmatch = 0;
            }
        }

        [TestMethod()]
        public void TestRecursiveSmarts1()
        {
            Match("[$(*O);$(*CC)]", "O[Po]CC");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts2()
        {
            Match("[$(*O);$(*CC)]", "OCCC");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts3()
        {
            Match("[$(*O);$(*CC)]", "CN1C(=O)N(C)C(=O)C(N(C)C=N2)=C12");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts4()
        {
            Match("[$(*O);$(*CC)]", "c1ncccc1C1CCCN1C");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts5()
        {
            Match("[$(*O);$(*CC)]", "N12CCC36C1CC(C(C2)=CCOC4CC5=O)C4C3N5c7ccccc76");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts6()
        {
            Match("[$([CX3]=[CX1]),$([CX3+]-[CX1-])]", "CN1C(=O)N(C)C(=O)C(N(C)C=N2)=C12");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts7()
        {
            Match("[$([CX3]=[OX1]),$([CX3+]-[OX1-])]", "c1ncccc1C1CCCN1C");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts8()
        {
            Match("[$([CX3]=[OX1]),$([CX3+]-[OX1-])]", "c1ccccc1C(=O)OC2CC(N3C)CCC3C2C(=O)OC");
            Assert.AreEqual(2, nmatch);
            Assert.AreEqual(2, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts9()
        {
            Match("[$([CX3]=[OX1]),$([CX3+]-[OX1-])]", "CCN(CC)C(=O)C1CN(C)C2CC3=CNc(ccc4)c3c4C2=C1");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts10()
        {
            Match("[$([CX3]=[OX1]),$([CX3+]-[OX1-])]", "CC[C+]([O-])C");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts11()
        {
            Match("[$([CX3]=[OX1]),$([CX3+]-[OX1-])]", "CCCCC[C+]([O-])CCCC");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts12()
        {
            Match("[$([CX3]=[OX1]),$([CX3+]-[OX1-])]", "CCCCCC(=O)CCCC");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts13()
        {
            Match("[$([C]aaO);$([C]aaaN)]", "c1c(C)c(O)c(N)cc1");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts14()
        {
            Match("[$([C]aaO);$([C]aaaN)]", "Oc1c(C)cc(N)cc1");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts15()
        {
            Match("[$([C]aaO);$([C]aaaN)]", "Oc1c(C)ccc(N)c1");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts16()
        {
            Match("[$([C]aaO);$([C]aaaN)]", "c1c(C)c(N)c(O)cc1");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts17()
        {
            Match("[$(C(=O)O),$(P(=O)),$(S(=O)O)]", "CC(=O)O");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);

            Match("[C&$(C(=O)O),P&$(P(=O)),S&$(S(=O)O)]", "CC(=O)O");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts18()
        {
            Match("[!$([#6,H0,-,-2,-3])]", "CCNC");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);

            Match("[!$([#6,H0,-,-2,-3])]", "CCN(C)C");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts19()
        {
            Match("[!H0;#7,#8,#9]", "CCN(C)C");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);

            Match("[!H0;#7,#8,#9]", "CC(=O)O");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts20()
        {
            Match("[C;D2;$(C(=C)(=C))]", "CCC=C=CC");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts21()
        {
            Match("[C;D2;H2;$(C(C)(C))]", "CC(C)CC");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);

            Match("[C;D2;H2;$(C(C)(C))]", "CC(C)CCC");
            Assert.AreEqual(2, nmatch);
            Assert.AreEqual(2, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts22()
        {
            Match("[C;D3;H1;$(C(C)(C)(C))]", "C(C)(C)CC(C)(C)C");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);

            Match("[C;D3;H1;$(C(C)(C)(C))]", "C(C)(C)C(C)(C)CC(C)C");
            Assert.AreEqual(2, nmatch);
            Assert.AreEqual(2, nqmatch);

            Match("[C;D3;H1;$(C(C)(C)(C))]", "C(C)CC(C)(C)C");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts23()
        {
            Match("[C;D2;H2;$(C(C)(C))]", "C(C)CC(C)(C)C");
            Assert.AreEqual(2, nmatch);
            Assert.AreEqual(2, nqmatch);

            Match("[C;D2;H2;$(C(C)(C))]", "C(C)(C)C(C)C(C)(C)C");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);

            Match("[C;D2;H2;$(C(C)(C))]", "C(C)(C)C(C)C(C)CCCC");
            Assert.AreEqual(3, nmatch);
            Assert.AreEqual(3, nqmatch);

        }

        [TestMethod()]
        public void TestRecursiveSmarts24()
        {
            Match("[S;D2;$(S(C)(C))]", "CCSCC");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);

            Match("[S;D2;$(S(C)(C))]", "CCS(=O)(=O)CC");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);

            Match("[S;D2;$(S(C)(C))]", "CCCCC");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);

        }

        [TestMethod()]
        public void TestRecursiveSmarts25()
        {
            Match("[NX3;H2,H1;!$(NC=O)]", "Cc1nc2=NC3=C(C(n2[nH]1)c1cc(cc(c1)F)F)C(=O)CC(C3)c1ccco1");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts34()
        {
            Match("[NX3;h2,h1,H1,H2;!$(NC=O)]", "NC1CCCC1C(CCNC)Cc1ccccc1N");
            Assert.AreEqual(3, nmatch);
            Assert.AreEqual(3, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts30()
        {
            Match("[NX3;H2,H1;!$(NC=O)]", "CC1CCCC(C1)N1CCN(CC1)C1CCN(CC1)Cc1ccccc1");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts31()
        {
            Match("[NX3;H2,H1;!$(NC=O)]", "CCOc1cc2c(cc1/C=C/C(=O)c1ccc(cc1)S(=O)(=O)N1CCCC1)OC(C2)C");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts32()
        {
            Match("[NX3;H2,H1;!$(NC=O)]", "CN1CCc2cc3c(c(c2C1CC(=O)/C=C/c1ccco1)OC)OCO3");
            Assert.AreEqual(0, nmatch);
            Assert.AreEqual(0, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts33()
        {
            Match("[NX3;H2,H1;!$(NC=O)]", "Cc1nc2=NC3=C(C(n2[nH]1)c1cc(cc(c1)F)F)C(=O)CC(C3)c1ccco1");
            Assert.AreEqual(1, nmatch);
            Assert.AreEqual(1, nqmatch);
        }

        [TestMethod()]
        public void TestRecursiveSmarts26()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[NX3;H2,H1;!$(NC=O)]");
            IAtomContainer smi = CreateFromSmiles("CCCc1cc(=O)nc([nH]1)S");
            int[] result = SMARTSSearchTest.Match(sqt, smi);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(0, result[1]);
        }

        [TestMethod()]
        public void TestRecursiveSmarts26_cdkAromaticModel()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[NX3;H2,H1;!$(NC=O)]");
            IAtomContainer smi = CreateFromSmiles("CCCc1cc(=O)nc([nH]1)S");
            sqt.SetAromaticity(new Aromaticity(ElectronDonation.CDKModel, Cycles.CDKAromaticSetFinder));
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(smi);
            int[] result = SMARTSSearchTest.Match(sqt, smi);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(1, result[1]);
        }

        [TestMethod()]
        public void TestRecursiveSmarts27_cdkAromaticModel()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[NX3;H2,H1;!$(NC=O)]");
            IAtomContainer smi = CreateFromSmiles("CCCc1nc(c2n1[nH]c(nc2=O)c1cc(ccc1OCC)S(=O)(=O)N1CCN(CC1)CC)C");
            sqt.SetAromaticity(new Aromaticity(ElectronDonation.CDKModel, Cycles.CDKAromaticSetFinder));
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(smi);
            int[] result = SMARTSSearchTest.Match(sqt, smi);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(1, result[1]);
        }

        [TestMethod()]
        public void TestRecursiveSmarts27()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[NX3;H2,H1;!$(NC=O)]");
            IAtomContainer smi = CreateFromSmiles("CCCc1nc(c2n1[nH]c(nc2=O)c1cc(ccc1OCC)S(=O)(=O)N1CCN(CC1)CC)C");
            int[] result = SMARTSSearchTest.Match(sqt, smi);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(0, result[1]);
        }

        [TestMethod()]
        public void TestRecursive28()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[NX3;H2,H1;!$(NC=O)]");
            IAtomContainer smi = CreateFromSmiles("Cc1ccc[n+]2c1[nH]cc(c2=O)c1n[nH]nn1");
            int[] result = SMARTSSearchTest.Match(sqt, smi);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(0, result[1]);
        }

        [TestMethod()]
        public void TestRecursive28_cdkAromaticModel()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[NX3;H2,H1;!$(NC=O)]");
            IAtomContainer smi = CreateFromSmiles("Cc1ccc[n+]2c1[nH]cc(c2=O)c1n[nH]nn1");
            sqt.SetAromaticity(new Aromaticity(ElectronDonation.CDKModel, Cycles.CDKAromaticSetFinder));
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(smi);
            int[] result = SMARTSSearchTest.Match(sqt, smi);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(1, result[1]);
        }

        [TestMethod()]
        public void TestRecursive29()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[NX3;H2,H1;!$(NC=O)]");
            IAtomContainer smi = CreateFromSmiles("Cc1cc(=O)c(c[nH]1)C(=O)NC(c1ccc(cc1)O)C(=O)NC1C(=O)N2C1SCC(=C2C(=O)O)CSc1nnnn1C");
            int[] result = SMARTSSearchTest.Match(sqt, smi);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(0, result[1]);
        }

        [TestMethod()]
        public void NestedRecursion()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 2, 2 }, SMARTSSearchTest.Match("[$(*C[$(*C)$(**N)])]", "CCCCN")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 1 }, SMARTSSearchTest.Match("[$(*C[$(*C)$(**N)])]", "CCN")));
        }

        [TestMethod()]
        public void TestRecursive29_cdkAromaticModel()
        {
            SMARTSQueryTool sqt = CreateFromSmarts("[NX3;H2,H1;!$(NC=O)]");
            IAtomContainer smi = CreateFromSmiles("Cc1cc(=O)c(c[nH]1)C(=O)NC(c1ccc(cc1)O)C(=O)NC1C(=O)N2C1SCC(=C2C(=O)O)CSc1nnnn1C");
            sqt.SetAromaticity(new Aromaticity(ElectronDonation.CDKModel, Cycles.CDKAromaticSetFinder));
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(smi);
            int[] result = SMARTSSearchTest.Match(sqt, smi);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(1, result[1]);
        }

        [TestCategory("SlowTest")]
        [TestMethod()]
        public void TestBasicAmineOnDrugs_cdkAromaticModel()
        {
            var filename = "drugs.smi";
            var ins = ResourceLoader.GetAsStream(GetType(), filename);
            using (EnumerableSMILESReader reader = new EnumerableSMILESReader(ins, ChemObjectBuilder.Instance))
            {
                SMARTSQueryTool sqt = new SMARTSQueryTool("[NX3;H2,H1;!$(NC=O)]", ChemObjectBuilder.Instance);
                sqt.SetAromaticity(new Aromaticity(ElectronDonation.CDKModel, Cycles.CDKAromaticSetFinder));
                int nmatch = 0;
                int nmol = 0;
                foreach (var container in reader)
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);

                    // skip un-typed atoms, they can't be run through the CDK aromatic
                    // model
                    foreach (var atom in container.Atoms)
                    {
                        if (atom.AtomTypeName == null)
                        {
                            goto continue_READ;
                        }
                    }

                    if (sqt.Matches(container))
                    {
                        nmatch++;
                    }
                    nmol++;
                    continue_READ:
                    ;
                }
                Assert.AreEqual(141, nmol);
                Assert.AreEqual(4, nmatch);
            }
        }

        [TestCategory("SlowTest")]
        [TestMethod()]
        public void TestBasicAmineOnDrugs()
        {
            var filename = "drugs.smi";
            var ins = ResourceLoader.GetAsStream(GetType(), filename);

            SMARTSQueryTool sqt = new SMARTSQueryTool("[NX3;H2,H1;!$(NC=O)]", ChemObjectBuilder.Instance);

            // iterating SMILES reader doesn't allow us to turn off automatic aromaticity
            // perception
            var sp = CDK.SmilesParser;

            int nmatch = 0;
            int nmol = 0;
            using (var reader = new StreamReader(ins))
            {
                string smi;
                while ((smi = reader.ReadLine()) != null)
                {
                    var container = sp.ParseSmiles(smi.Split('\t')[0]);
                    if (sqt.Matches(container))
                    {
                        nmatch++;
                    }
                    nmol++;
                }
                Assert.AreEqual(141, nmol);
                Assert.AreEqual(0, nmatch);
            }
        }

        // @cdk.bug 1312
        [TestMethod()]
        public void RecursiveComponentGrouping()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 1 }, SMARTSSearchTest.Match("[O;D1;$(([a,A]).([A,a]))][CH]=O", "OC=O.c1ccccc1")));
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 0, 0 }, SMARTSSearchTest.Match("[O;D1;$(([a,A]).([A,a]))][CH]=O", "OC=O")));
        }

        // @cdk.bug 844
        [TestMethod()]
        public void Bug844()
        {
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 1, 1 }, SMARTSSearchTest.Match("[*R0]-[$([NRD3][CR]=O)]", "N1(CC)C(=O)CCCC1")));
        }
    }
}
