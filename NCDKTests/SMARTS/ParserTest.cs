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
using NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.Silent;
using NCDK.Smiles.SMARTS.Parser;
using System;

namespace NCDK.SMARTS
{
    // @author Egon Willighagen
    // @cdk.module test-smarts
    // @cdk.require ant1.6
    public class ParserTest : CDKTestCase
    {
        private void Parse(string smarts, SmartsFlaver flav)
        {
            var builder = CDK.Builder;
            if (!Smarts.Parse(builder.NewAtomContainer(), smarts, flav))
                throw new Exception(Smarts.GetLastErrorMessage());
        }

        private void Parse(string smarts)
        {
            Parse(smarts, SmartsFlaver.Loose);
        }

        [TestMethod()]
        public void ErrorHandling()
        {
            var builder = CDK.Builder;
            if (!Smarts.Parse(builder.NewAtomContainer(), "CCCJCCC"))
            {
                Assert.AreEqual("Unexpected character", Smarts.GetLastErrorMessage());
                Assert.AreEqual("CCCJCCC\n   ^\n", Smarts.GetLastErrorLocation());
            }
        }

        [TestMethod()]
        public void TestQueryAtomCreation()
        {
            var container = SMARTSParser.Parse("*", ChemObjectBuilder.Instance);
            Assert.AreEqual(1, container.Atoms.Count);
            IAtom atom = container.Atoms[0];
            Assert.IsTrue(atom is SMARTSAtom);
        }

        [TestMethod()]
        public void TestAliphaticAtom()
        {
            var container = SMARTSParser.Parse("A", ChemObjectBuilder.Instance);
            Assert.AreEqual(1, container.Atoms.Count);
            IAtom atom = container.Atoms[0];
            Assert.IsTrue(atom is SMARTSAtom);
        }

        [TestMethod()]
        public void TestAromaticAtom()
        {
            var container = SMARTSParser.Parse("a", ChemObjectBuilder.Instance);
            Assert.AreEqual(1, container.Atoms.Count);
            IAtom atom = container.Atoms[0];
            Assert.IsTrue(atom is SMARTSAtom);
        }

        [TestMethod()]
        public void TestDegree()
        {
            var container = SMARTSParser.Parse("[D2]", ChemObjectBuilder.Instance);
            Assert.AreEqual(1, container.Atoms.Count);
            IAtom atom = container.Atoms[0];
            Assert.IsTrue(atom is SMARTSAtom);
        }

        [TestMethod()]
        public void TestImplicitHCount()
        {
            var container = SMARTSParser.Parse("[h3]", ChemObjectBuilder.Instance);
            Assert.AreEqual(1, container.Atoms.Count);
            IAtom atom = container.Atoms[0];
            Assert.IsTrue(atom is SMARTSAtom);
        }

        [TestMethod()]
        public void TestTotalHCount()
        {
            var container = SMARTSParser.Parse("[H2]", ChemObjectBuilder.Instance);
            Assert.AreEqual(1, container.Atoms.Count);
            IAtom atom = container.Atoms[0];
            Assert.IsTrue(atom is SMARTSAtom);
        }

        /// <summary>
        // @cdk.bug 1760967
        /// </summary>
        [TestMethod()]
        public void TestSingleBond()
        {
            var container = SMARTSParser.Parse("C-C", ChemObjectBuilder.Instance);
            Assert.AreEqual(2, container.Atoms.Count);
            Assert.AreEqual(1, container.Bonds.Count);
            var bond = container.Bonds[0];
            Assert.IsTrue(bond is OrderQueryBond);
            var qBond = (OrderQueryBond)bond;
            Assert.AreEqual(BondOrder.Single, qBond.Order);
        }

        [TestMethod()]
        public void TestDoubleBond()
        {
            var container = SMARTSParser.Parse("C=C", ChemObjectBuilder.Instance);
            Assert.AreEqual(2, container.Atoms.Count);
            Assert.AreEqual(1, container.Bonds.Count);
            var bond = container.Bonds[0];
            Assert.IsTrue(bond is OrderQueryBond);
            var qBond = (OrderQueryBond)bond;
            Assert.AreEqual(BondOrder.Double, qBond.Order);
        }

        [TestMethod()]
        public void TestTripleBond()
        {
            var container = SMARTSParser.Parse("C#C", ChemObjectBuilder.Instance);
            Assert.AreEqual(2, container.Atoms.Count);
            Assert.AreEqual(1, container.Bonds.Count);
            var bond = container.Bonds[0];
            Assert.IsTrue(bond is OrderQueryBond);
            var qBond = (OrderQueryBond)bond;
            Assert.AreEqual(BondOrder.Triple, qBond.Order);
        }

        [TestMethod()]
        public void TestAromaticBond()
        {
            var container = SMARTSParser.Parse("C:C", ChemObjectBuilder.Instance);
            Assert.AreEqual(2, container.Atoms.Count);
            Assert.AreEqual(1, container.Bonds.Count);
            var bond = container.Bonds[0];
            Assert.IsTrue(bond is AromaticQueryBond);
        }

        [TestMethod()]
        public void TestAnyOrderBond()
        {
            var container = SMARTSParser.Parse("C~C", ChemObjectBuilder.Instance);
            Assert.AreEqual(2, container.Atoms.Count);
            Assert.AreEqual(1, container.Bonds.Count);
            var bond = container.Bonds[0];
            Assert.IsTrue(bond is AnyOrderQueryBond);
        }

        // @cdk.bug 2786624
        [TestMethod()]
        public void Test2LetterSMARTS()
        {
            var query = SMARTSParser.Parse("Sc1ccccc1", ChemObjectBuilder.Instance);
            Assert.AreEqual(7, query.Atoms.Count);
            Assert.AreEqual("S", query.Atoms[0].Symbol);
        }

        [TestMethod()]
        public void TestPattern1()
        {
            Parse("[CX4]");
        }

        [TestMethod()]
        public void TestPattern2()
        {
            Parse("[$([CX2](=C)=C)]");
        }

        [TestMethod()]
        public void TestPattern3()
        {
            Parse("[$([CX3]=[CX3])]");
        }

        [TestMethod()]
        public void TestPattern4()
        {
            Parse("[$([CX2]#C)]");
        }

        [TestMethod()]
        public void TestPattern5()
        {
            Parse("[CX3]=[OX1]");
        }

        [TestMethod()]
        public void TestPattern6()
        {
            Parse("[$([CX3]=[OX1]),$([CX3+]-[OX1-])]");
        }

        [TestMethod()]
        public void TestPattern7()
        {
            Parse("[CX3](=[OX1])C");
        }

        [TestMethod()]
        public void TestPattern8()
        {
            Parse("[OX1]=CN");
        }

        [TestMethod()]
        public void TestPattern9()
        {
            Parse("[CX3](=[OX1])O");
        }

        [TestMethod()]
        public void TestPattern10()
        {
            Parse("[CX3](=[OX1])[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern11()
        {
            Parse("[CX3H1](=O)[#6]");
        }

        [TestMethod()]
        public void TestPattern12()
        {
            Parse("[CX3](=[OX1])[OX2][CX3](=[OX1])");
        }

        [TestMethod()]
        public void TestPattern13()
        {
            Parse("[NX3][CX3](=[OX1])[#6]");
        }

        [TestMethod()]
        public void TestPattern14()
        {
            Parse("[NX3][CX3]=[NX3+]");
        }

        [TestMethod()]
        public void TestPattern15()
        {
            Parse("[NX3,NX4+][CX3](=[OX1])[OX2,OX1-]");
        }

        [TestMethod()]
        public void TestPattern16()
        {
            Parse("[NX3][CX3](=[OX1])[OX2H0]");
        }

        [TestMethod()]
        public void TestPattern17()
        {
            Parse("[NX3,NX4+][CX3](=[OX1])[OX2H,OX1-]");
        }

        [TestMethod()]
        public void TestPattern18()
        {
            Parse("[CX3](=O)[O-]");
        }

        [TestMethod()]
        public void TestPattern19()
        {
            Parse("[CX3](=[OX1])(O)O");
        }

        [TestMethod()]
        public void TestPattern20()
        {
            Parse("[CX3](=[OX1])([OX2])[OX2H,OX1H0-1]");
        }

        [TestMethod()]
        public void TestPattern21()
        {
            Parse("[CX3](=O)[OX2H1]");
        }

        [TestMethod()]
        public void TestPattern22()
        {
            Parse("[CX3](=O)[OX1H0-,OX2H1]");
        }

        [TestMethod()]
        public void TestPattern23()
        {
            Parse("[NX3][CX2]#[NX1]");
        }

        [TestMethod()]
        public void TestPattern24()
        {
            Parse("[#6][CX3](=O)[OX2H0][#6]");
        }

        [TestMethod()]
        public void TestPattern25()
        {
            Parse("[#6][CX3](=O)[#6]");
        }

        [TestMethod()]
        public void TestPattern26()
        {
            Parse("[OD2]([#6])[#6]");
        }

        [TestMethod()]
        public void TestPattern27()
        {
            Parse("[H]");
        }

        [TestMethod()]
        public void TestPattern28()
        {
            Parse("[!#1]");
        }

        [TestMethod()]
        public void TestPattern29()
        {
            Parse("[H+]");
        }

        [TestMethod()]
        public void TestPattern30()
        {
            Parse("[+H]");
        }

        [TestMethod()]
        public void TestPattern31()
        {
            Parse("[NX3;H2,H1;!$(NC=O)]");
        }

        [TestMethod()]
        public void TestPattern32()
        {
            Parse("[NX3][CX3]=[CX3]");
        }

        [TestMethod()]
        public void TestPattern33()
        {
            Parse("[NX3;H2,H1;!$(NC=O)].[NX3;H2,H1;!$(NC=O)]");
        }

        [TestMethod()]
        public void TestPattern34()
        {
            Parse("[NX3][$(C=C),$(cc)]");
        }

        [TestMethod()]
        public void TestPattern35()
        {
            Parse("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]");
        }

        [TestMethod()]
        public void TestPattern36()
        {
            Parse("[NX3H2,NH3X4+][CX4H]([*])[CX3](=[OX1])[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[OX2H,OX1-]");
        }

        [TestMethod()]
        public void TestPattern37()
        {
            Parse("[$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H]([*])[CX3](=[OX1])[OX2H,OX1-,N]");
        }

        [TestMethod()]
        public void TestPattern38()
        {
            Parse("[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern39()
        {
            Parse("[CH2X4][CH2X4][CH2X4][NHX3][CH0X3](=[NH2X3+,NHX2+0])[NH2X3]");
        }

        [TestMethod()]
        public void TestPattern40()
        {
            Parse("[CH2X4][CX3](=[OX1])[NX3H2]");
        }

        [TestMethod()]
        public void TestPattern41()
        {
            Parse("[CH2X4][CX3](=[OX1])[OH0-,OH]");
        }

        [TestMethod()]
        public void TestPattern42()
        {
            Parse("[CH2X4][SX2H,SX1H0-]");
        }

        [TestMethod()]
        public void TestPattern43()
        {
            Parse("[CH2X4][CH2X4][CX3](=[OX1])[OH0-,OH]");
        }

        [TestMethod()]
        public void TestPattern44()
        {
            Parse("[$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H2][CX3](=[OX1])[OX2H,OX1-,N])]");
        }

        [TestMethod()]
        public void TestPattern45()
        {
            Parse("[CH2X4][#6X3]1:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:[#6X3H]:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:[#6X3H]1");
        }

        [TestMethod()]
        public void TestPattern47()
        {
            Parse("[CHX4]([CH3X4])[CH2X4][CH3X4]");
        }

        [TestMethod()]
        public void TestPattern48()
        {
            Parse("[CH2X4][CHX4]([CH3X4])[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern49()
        {
            Parse("[CH2X4][CH2X4][CH2X4][CH2X4][NX4+,NX3+0]");
        }

        [TestMethod()]
        public void TestPattern50()
        {
            Parse("[CH2X4][CH2X4][SX2][CH3X4]");
        }

        [TestMethod()]
        public void TestPattern51()
        {
            Parse("[CH2X4][cX3]1[cX3H][cX3H][cX3H][cX3H][cX3H]1");
        }

        [TestMethod()]
        public void TestPattern52()
        {
            Parse("[$([NX3H,NX4H2+]),$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3](=[OX1])[OX2H,OX1-,N]");
        }

        [TestMethod()]
        public void TestPattern53()
        {
            Parse("[CH2X4][OX2H]");
        }

        [TestMethod()]
        public void TestPattern54()
        {
            Parse("[NX3][CX3]=[SX1]");
        }

        [TestMethod()]
        public void TestPattern55()
        {
            Parse("[CHX4]([CH3X4])[OX2H]");
        }

        [TestMethod()]
        public void TestPattern56()
        {
            Parse("[CH2X4][cX3]1[cX3H][nX3H][cX3]2[cX3H][cX3H][cX3H][cX3H][cX3]12");
        }

        [TestMethod()]
        public void TestPattern57()
        {
            Parse("[CH2X4][cX3]1[cX3H][cX3H][cX3]([OHX2,OH0X1-])[cX3H][cX3H]1");
        }

        [TestMethod()]
        public void TestPattern58()
        {
            Parse("[CHX4]([CH3X4])[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern59()
        {
            Parse("[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern60()
        {
            Parse("[CH2X4][CH2X4][CH2X4][NHX3][CH0X3](=[NH2X3+,NHX2+0])[NH2X3]");
        }

        [TestMethod()]
        public void TestPattern61()
        {
            Parse("[CH2X4][CX3](=[OX1])[NX3H2]");
        }

        [TestMethod()]
        public void TestPattern62()
        {
            Parse("[CH2X4][CX3](=[OX1])[OH0-,OH]");
        }

        [TestMethod()]
        public void TestPattern63()
        {
            Parse("[CH2X4][SX2H,SX1H0-]");
        }

        [TestMethod()]
        public void TestPattern64()
        {
            Parse("[CH2X4][CH2X4][CX3](=[OX1])[OH0-,OH]");
        }

        [TestMethod()]
        public void TestPattern65()
        {
            Parse("[CH2X4][#6X3]1:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:[#6X3H]:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:[#6X3H]1");
        }

        [TestMethod()]
        public void TestPattern67()
        {
            Parse("[CHX4]([CH3X4])[CH2X4][CH3X4]");
        }

        [TestMethod()]
        public void TestPattern68()
        {
            Parse("[CH2X4][CHX4]([CH3X4])[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern69()
        {
            Parse("[CH2X4][CH2X4][CH2X4][CH2X4][NX4+,NX3+0]");
        }

        [TestMethod()]
        public void TestPattern70()
        {
            Parse("[CH2X4][CH2X4][SX2][CH3X4]");
        }

        [TestMethod()]
        public void TestPattern71()
        {
            Parse("[CH2X4][cX3]1[cX3H][cX3H][cX3H][cX3H][cX3H]1");
        }

        [TestMethod()]
        public void TestPattern72()
        {
            Parse("[CH2X4][OX2H]");
        }

        [TestMethod()]
        public void TestPattern73()
        {
            Parse("[CHX4]([CH3X4])[OX2H]");
        }

        [TestMethod()]
        public void TestPattern74()
        {
            Parse("[CH2X4][cX3]1[cX3H][nX3H][cX3]2[cX3H][cX3H][cX3H][cX3H][cX3]12");
        }

        [TestMethod()]
        public void TestPattern75()
        {
            Parse("[CH2X4][cX3]1[cX3H][cX3H][cX3]([OHX2,OH0X1-])[cX3H][cX3H]1");
        }

        [TestMethod()]
        public void TestPattern76()
        {
            Parse("[CHX4]([CH3X4])[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern77()
        {
            Parse("[$(*-[NX2-]-[NX2+]#[NX1]),$(*-[NX2]=[NX2+]=[NX1-])]");
        }

        [TestMethod()]
        public void TestPattern78()
        {
            Parse("[$([NX1-]=[NX2+]=[NX1-]),$([NX1]#[NX2+]-[NX1-2])]");
        }

        [TestMethod()]
        public void TestPattern79()
        {
            Parse("[#7]");
        }

        [TestMethod()]
        public void TestPattern80()
        {
            Parse("[NX2]=N");
        }

        [TestMethod()]
        public void TestPattern81()
        {
            Parse("[NX2]=[NX2]");
        }

        [TestMethod()]
        public void TestPattern82()
        {
            Parse("[$([NX2]=[NX3+]([O-])[#6]),$([NX2]=[NX3+0](=[O])[#6])]");
        }

        [TestMethod()]
        public void TestPattern83()
        {
            Parse("[$([#6]=[N+]=[N-]),$([#6-]-[N+]#[N])]");
        }

        [TestMethod()]
        public void TestPattern84()
        {
            Parse("[$([nr5]:[nr5,or5,sr5]),$([nr5]:[cr5]:[nr5,or5,sr5])]");
        }

        [TestMethod()]
        public void TestPattern85()
        {
            Parse("[NX3][NX3]");
        }

        [TestMethod()]
        public void TestPattern86()
        {
            Parse("[NX3][NX2]=[*]");
        }

        [TestMethod()]
        public void TestPattern87()
        {
            Parse("[CX3;$([C]([#6])[#6]),$([CH][#6])]=[NX2][#6]");
        }

        [TestMethod()]
        public void TestPattern88()
        {
            Parse("[$([CX3]([#6])[#6]),$([CX3H][#6])]=[$([NX2][#6]),$([NX2H])]");
        }

        [TestMethod()]
        public void TestPattern89()
        {
            Parse("[NX3+]=[CX3]");
        }

        [TestMethod()]
        public void TestPattern90()
        {
            Parse("[CX3](=[OX1])[NX3H][CX3](=[OX1])");
        }

        [TestMethod()]
        public void TestPattern91()
        {
            Parse("[CX3](=[OX1])[NX3H0]([#6])[CX3](=[OX1])");
        }

        [TestMethod()]
        public void TestPattern92()
        {
            Parse("[CX3](=[OX1])[NX3H0]([NX3H0]([CX3](=[OX1]))[CX3](=[OX1]))[CX3](=[OX1])");
        }

        [TestMethod()]
        public void TestPattern93()
        {
            Parse("[$([NX3](=[OX1])(=[OX1])O),$([NX3+]([OX1-])(=[OX1])O)]");
        }

        [TestMethod()]
        public void TestPattern94()
        {
            Parse("[$([OX1]=[NX3](=[OX1])[OX1-]),$([OX1]=[NX3+]([OX1-])[OX1-])]");
        }

        [TestMethod()]
        public void TestPattern95()
        {
            Parse("[NX1]#[CX2]");
        }

        [TestMethod()]
        public void TestPattern96()
        {
            Parse("[CX1-]#[NX2+]");
        }

        [TestMethod()]
        public void TestPattern97()
        {
            Parse("[$([NX3](=O)=O),$([NX3+](=O)[O-])][!#8]");
        }

        [TestMethod()]
        public void TestPattern98()
        {
            Parse("[$([NX3](=O)=O),$([NX3+](=O)[O-])][!#8].[$([NX3](=O)=O),$([NX3+](=O)[O-])][!#8]");
        }

        [TestMethod()]
        public void TestPattern99()
        {
            Parse("[NX2]=[OX1]");
        }

        [TestMethod()]
        public void TestPattern101()
        {
            Parse("[$([#7+][OX1-]),$([#7v5]=[OX1]);!$([#7](~[O])~[O]);!$([#7]=[#7])]");
        }

        [TestMethod()]
        public void TestPattern102()
        {
            Parse("[OX2H]");
        }

        [TestMethod()]
        public void TestPattern103()
        {
            Parse("[#6][OX2H]");
        }

        [TestMethod()]
        public void TestPattern104()
        {
            Parse("[OX2H][CX3]=[OX1]");
        }

        [TestMethod()]
        public void TestPattern105()
        {
            Parse("[OX2H]P");
        }

        [TestMethod()]
        public void TestPattern106()
        {
            Parse("[OX2H][#6X3]=[#6]");
        }

        [TestMethod()]
        public void TestPattern107()
        {
            Parse("[OX2H][cX3]:[c]");
        }

        [TestMethod()]
        public void TestPattern108()
        {
            Parse("[OX2H][$(C=C),$(cc)]");
        }

        [TestMethod()]
        public void TestPattern109()
        {
            Parse("[$([OH]-*=[!#6])]");
        }

        [TestMethod()]
        public void TestPattern110()
        {
            Parse("[OX2,OX1-][OX2,OX1-]");
        }

        [TestMethod()]
        public void TestPattern111()
        { // Phosphoric_acid groups.
            Parse("[$(P(=[OX1])([$([OX2H]),$([OX1-]),$([OX2]P)])([$([OX2H]),$([OX1-]),$([OX2]P)])[$([OX2H]),$([OX1-]),$([OX2]P)]),$([P+]([OX1-])([$([OX"
                    + "2H]),$([OX1-]),$([OX2]P)])([$([OX2H]),$([OX1-]),$([OX2]P)])[$([OX2H]),$([OX1-]),$([OX2]P)])]");
        }

        [TestMethod()]
        public void TestPattern112()
        { // Phosphoric_ester groups.
            Parse("[$(P(=[OX1])([OX2][#6])([$([OX2H]),$([OX1-]),$([OX2][#6])])[$([OX2H]),$([OX1-]),$([OX2][#6]),$([OX2]P)]),$([P+]([OX1-])([OX2][#6])(["
                    + "$([OX2H]),$([OX1-]),$([OX2][#6])])[$([OX2H]),$([OX1-]),$([OX2][#6]),$([OX2]P)])]");
        }

        [TestMethod()]
        public void TestPattern113()
        {
            Parse("[S-][CX3](=S)[#6]");
        }

        [TestMethod()]
        public void TestPattern114()
        {
            Parse("[#6X3](=[SX1])([!N])[!N]");
        }

        [TestMethod()]
        public void TestPattern115()
        {
            Parse("[SX2]");
        }

        [TestMethod()]
        public void TestPattern116()
        {
            Parse("[#16X2H]");
        }

        [TestMethod()]
        public void TestPattern117()
        {
            Parse("[#16!H0]");
        }

        [TestMethod()]
        public void TestPattern118()
        {
            Parse("[NX3][CX3]=[SX1]");
        }

        [TestMethod()]
        public void TestPattern119()
        {
            Parse("[#16X2H0]");
        }

        [TestMethod()]
        public void TestPattern120()
        {
            Parse("[#16X2H0][!#16]");
        }

        [TestMethod()]
        public void TestPattern121()
        {
            Parse("[#16X2H0][#16X2H0]");
        }

        [TestMethod()]
        public void TestPattern122()
        {
            Parse("[#16X2H0][!#16].[#16X2H0][!#16]");
        }

        [TestMethod()]
        public void TestPattern123()
        {
            Parse("[$([#16X3](=[OX1])[OX2H0]),$([#16X3+]([OX1-])[OX2H0])]");
        }

        [TestMethod()]
        public void TestPattern124()
        {
            Parse("[$([#16X3](=[OX1])[OX2H,OX1H0-]),$([#16X3+]([OX1-])[OX2H,OX1H0-])]");
        }

        [TestMethod()]
        public void TestPattern125()
        {
            Parse("[$([#16X4](=[OX1])=[OX1]),$([#16X4+2]([OX1-])[OX1-])]");
        }

        [TestMethod()]
        public void TestPattern126()
        {
            Parse("[$([#16X4](=[OX1])(=[OX1])([#6])[#6]),$([#16X4+2]([OX1-])([OX1-])([#6])[#6])]");
        }

        [TestMethod()]
        public void TestPattern127()
        {
            Parse("[$([#16X4](=[OX1])(=[OX1])([#6])[OX2H,OX1H0-]),$([#16X4+2]([OX1-])([OX1-])([#6])[OX2H,OX1H0-])]");
        }

        [TestMethod()]
        public void TestPattern128()
        {
            Parse("[$([#16X4](=[OX1])(=[OX1])([#6])[OX2H0]),$([#16X4+2]([OX1-])([OX1-])([#6])[OX2H0])]");
        }

        [TestMethod()]
        public void TestPattern129()
        {
            Parse("[$([#16X4]([NX3])(=[OX1])(=[OX1])[#6]),$([#16X4+2]([NX3])([OX1-])([OX1-])[#6])]");
        }

        [TestMethod()]
        public void TestPattern130()
        {
            Parse("[SX4](C)(C)(=O)=N");
        }

        [TestMethod()]
        public void TestPattern131()
        {
            Parse("[$([SX4](=[OX1])(=[OX1])([!O])[NX3]),$([SX4+2]([OX1-])([OX1-])([!O])[NX3])]");
        }

        [TestMethod()]
        public void TestPattern132()
        {
            Parse("[$([#16X3]=[OX1]),$([#16X3+][OX1-])]");
        }

        [TestMethod()]
        public void TestPattern133()
        {
            Parse("[$([#16X3](=[OX1])([#6])[#6]),$([#16X3+]([OX1-])([#6])[#6])]");
        }

        [TestMethod()]
        public void TestPattern134()
        {
            Parse("[$([#16X4](=[OX1])(=[OX1])([OX2H,OX1H0-])[OX2][#6]),$([#16X4+2]([OX1-])([OX1-])([OX2H,OX1H0-])[OX2][#6])]");
        }

        [TestMethod()]
        public void TestPattern135()
        {
            Parse("[$([SX4](=O)(=O)(O)O),$([SX4+2]([O-])([O-])(O)O)]");
        }

        [TestMethod()]
        public void TestPattern136()
        {
            Parse("[$([#16X4](=[OX1])(=[OX1])([OX2][#6])[OX2][#6]),$([#16X4](=[OX1])(=[OX1])([OX2][#6])[OX2][#6])]");
        }

        [TestMethod()]
        public void TestPattern137()
        {
            Parse("[$([#16X4]([NX3])(=[OX1])(=[OX1])[OX2][#6]),$([#16X4+2]([NX3])([OX1-])([OX1-])[OX2][#6])]");
        }

        [TestMethod()]
        public void TestPattern138()
        {
            Parse("[$([#16X4]([NX3])(=[OX1])(=[OX1])[OX2H,OX1H0-]),$([#16X4+2]([NX3])([OX1-])([OX1-])[OX2H,OX1H0-])]");
        }

        [TestMethod()]
        public void TestPattern139()
        {
            Parse("[#16X2][OX2H,OX1H0-]");
        }

        [TestMethod()]
        public void TestPattern140()
        {
            Parse("[#16X2][OX2H0]");
        }

        [TestMethod()]
        public void TestPattern141()
        {
            Parse("[#6][F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern142()
        {
            Parse("[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern143()
        {
            Parse("[F,Cl,Br,I].[F,Cl,Br,I].[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern144()
        {
            Parse("[CX3](=[OX1])[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern145()
        {
            Parse("[$([#6X4@](*)(*)(*)*),$([#6X4@H](*)(*)*)]");
        }

        [TestMethod()]
        public void TestPattern146()
        {
            Parse("[$([cX2+](:*):*)]");
        }

        [TestMethod()]
        public void TestPattern147()
        {
            Parse("[$([cX3](:*):*),$([cX2+](:*):*)]");
        }

        [TestMethod()]
        public void TestPattern148()
        {
            Parse("[$([cX3](:*):*),$([cX2+](:*):*),$([CX3]=*),$([CX2+]=*)]");
        }

        [TestMethod()]
        public void TestPattern149()
        {
            Parse("[$([nX3](:*):*),$([nX2](:*):*),$([#7X2]=*),$([NX3](=*)=*),$([#7X3+](-*)=*),$([#7X3+H]=*)]");
        }

        [TestMethod()]
        public void TestPattern150()
        {
            Parse("[$([#1X1][$([nX3](:*):*),$([nX2](:*):*),$([#7X2]=*),$([NX3](=*)=*),$([#7X3+](-*)=*),$([#7X3+H]=*)])]");
        }

        [TestMethod()]
        public void TestPattern151()
        {
            Parse("[$([NX4+]),$([NX3]);!$(*=*)&!$(*:*)]");
        }

        [TestMethod()]
        public void TestPattern152()
        {
            Parse("[$([#1X1][$([NX4+]),$([NX3]);!$(*=*)&!$(*:*)])]");
        }

        [TestMethod()]
        public void TestPattern153()
        {
            Parse("[$([$([NX3]=O),$([NX3+][O-])])]");
        }

        [TestMethod()]
        public void TestPattern154()
        {
            Parse("[$([$([NX4]=O),$([NX4+][O-])])]");
        }

        [TestMethod()]
        public void TestPattern155()
        {
            Parse("[$([$([NX4]=O),$([NX4+][O-,#0])])]");
        }

        [TestMethod()]
        public void TestPattern156()
        {
            Parse("[$([NX4+]),$([NX4]=*)]");
        }

        [TestMethod()]
        public void TestPattern157()
        {
            Parse("[$([SX3]=N)]");
        }

        [TestMethod()]
        public void TestPattern158()
        {
            Parse("[$([SX1]=[#6])]");
        }

        [TestMethod()]
        public void TestPattern159()
        {
            Parse("[$([NX1]#*)]");
        }

        [TestMethod()]
        public void TestPattern160()
        {
            Parse("[$([OX2])]");
        }

        [TestMethod()]
        public void TestPattern161()
        {
            Parse("[R0;D2][R0;D2][R0;D2][R0;D2]");
        }

        [TestMethod()]
        public void TestPattern162()
        {
            Parse("[R0;D2]~[R0;D2]~[R0;D2]~[R0;D2]");
        }

        [TestMethod()]
        public void TestPattern163()
        {
            Parse("[AR0]~[AR0]~[AR0]~[AR0]~[AR0]~[AR0]~[AR0]~[AR0]");
        }

        [TestMethod()]
        public void TestPattern164()
        {
            Parse("[!$([#6+0]);!$(C(F)(F)F);!$(c(:[!c]):[!c])!$([#6]=,#[!#6])]");
        }

        [TestMethod()]
        public void TestPattern165()
        {
            Parse("[$([#6+0]);!$(C(F)(F)F);!$(c(:[!c]):[!c])!$([#6]=,#[!#6])]");
        }

        [TestMethod()]
        public void TestPattern166()
        {
            Parse("[$([SX1]~P)]");
        }

        [TestMethod()]
        public void TestPattern167()
        {
            Parse("[$([NX3]C=N)]");
        }

        [TestMethod()]
        public void TestPattern168()
        {
            Parse("[$([NX3]N=C)]");
        }

        [TestMethod()]
        public void TestPattern169()
        {
            Parse("[$([NX3]N=N)]");
        }

        [TestMethod()]
        public void TestPattern170()
        {
            Parse("[$([OX2]C=N)]");
        }

        [TestMethod()]
        public void TestPattern171()
        {
            Parse("[!$(*#*)&!D1]-!@[!$(*#*)&!D1]");
        }

        [TestMethod()]
        public void TestPattern172()
        {
            Parse("[$([*R2]([*R])([*R])([*R]))].[$([*R2]([*R])([*R])([*R]))]");
        }

        [TestMethod()]
        public void TestPattern173()
        {
            Parse("*-!:aa-!:*");
        }

        [TestMethod()]
        public void TestPattern174()
        {
            Parse("*-!:aaa-!:*");
        }

        [TestMethod()]
        public void TestPattern175()
        {
            Parse("*-!:aaaa-!:*");
        }

        [TestMethod()]
        public void TestPattern176()
        {
            Parse("*-!@*");
        }

        [TestMethod()]
        public void TestPattern177()
        { // CIS or TRANS double or aromatic bond in a ring
            Parse("*/,\\[R]=,:;@[R]/,\\*");
        }

        [TestMethod()]
        public void TestPattern178()
        { // Fused benzene rings
            Parse("c12ccccc1cccc2");
        }

        [TestMethod()]
        public void TestPattern179()
        {
            Parse("[r;!r3;!r4;!r5;!r6;!r7]");
        }

        [TestMethod()]
        public void TestPattern180()
        {
            Parse("[sX2r5]");
        }

        [TestMethod()]
        public void TestPattern181()
        {
            Parse("[oX2r5]");
        }

        [TestMethod()]
        public void TestPattern182()
        { // Unfused benzene ring
            Parse("[cR1]1[cR1][cR1][cR1][cR1][cR1]1");
        }

        [TestMethod()]
        public void TestPattern183()
        { // Multiple non-fused benzene rings
            Parse("[cR1]1[cR1][cR1][cR1][cR1][cR1]1.[cR1]1[cR1][cR1][cR1][cR1][cR1]1");
        }

        [TestMethod()]
        public void TestPattern184()
        { // Generic amino acid: low specificity.
            Parse("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]");
        }

        [TestMethod()]
        public void TestPattern185()
        { //Template for 20 standard a.a.s
            Parse("[$([$([NX3H,NX4H2+]),$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3](=[OX1])[OX2H,OX1-,N]),"
                    + "$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX"
                    + "4H2][CX3](=[OX1])[OX2H,OX1-,N]),$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H]([*])[CX3](=[OX1])[OX2H,OX1-,N])]");
        }

        [TestMethod()]
        public void TestPattern186()
        { // Proline
            Parse("[$([NX3H,NX4H2+]),$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3](=[OX1])[OX2H,OX1-,N]");
        }

        [TestMethod()]
        public void TestPattern187()
        { // Glycine
            Parse("[$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H2][CX3](=[OX1])[OX2H,OX1-,N])]");
        }

        [TestMethod()]
        public void TestPattern188()
        { // Alanine
            Parse("[$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H]([CH3X4])[CX3](=[OX1])[OX2H,OX1-,N]");
        }

        [TestMethod()]
        public void TestPattern189()
        { //18_standard_aa_side_chains.
            Parse("([$([CH3X4]),$([CH2X4][CH2X4][CH2X4][NHX3][CH0X3](=[NH2X3+,NHX2+0])[NH2X3]),"
                    + "$([CH2X4][CX3](=[OX1])[NX3H2]),$([CH2X4][CX3](=[OX1])[OH0-,OH]),"
                    + "$([CH2X4][SX2H,SX1H0-]),$([CH2X4][CH2X4][CX3](=[OX1])[OH0-,OH]),"
                    + "$([CH2X4][#6X3]1:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:"
                    + "[#6X3H]:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:[#6X3H]1),"
                    + "$([CHX4]([CH3X4])[CH2X4][CH3X4]),$([CH2X4][CHX4]([CH3X4])[CH3X4]),"
                    + "$([CH2X4][CH2X4][CH2X4][CH2X4][NX4+,NX3+0]),$([CH2X4][CH2X4][SX2][CH3X4]),"
                    + "$([CH2X4][cX3]1[cX3H][cX3H][cX3H][cX3H][cX3H]1),$([CH2X4][OX2H]),"
                    + "$([CHX4]([CH3X4])[OX2H]),$([CH2X4][cX3]1[cX3H][nX3H][cX3]2[cX3H][cX3H][cX3H][cX3H][cX3]12),"
                    + "$([CH2X4][cX3]1[cX3H][cX3H][cX3]([OHX2,OH0X1-])[cX3H][cX3H]1),$([CHX4]([CH3X4])[CH3X4])])");
        }

        [TestMethod()]
        public void TestPattern190()
        { // N in Any_standard_amino_acid.
            Parse("[$([$([NX3H,NX4H2+]),$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3]"
                    + "(=[OX1])[OX2H,OX1-,N]),$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H2][CX3]"
                    + "(=[OX1])[OX2H,OX1-,N]),$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H]([$([CH3X4]),"
                    + "$([CH2X4][CH2X4][CH2X4][NHX3][CH0X3](=[NH2X3+,NHX2+0])[NH2X3]),$"
                    + "([CH2X4][CX3](=[OX1])[NX3H2]),$([CH2X4][CX3](=[OX1])[OH0-,OH]),"
                    + "$([CH2X4][SX2H,SX1H0-]),$([CH2X4][CH2X4][CX3](=[OX1])[OH0-,OH]),"
                    + "$([CH2X4][#6X3]1:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:"
                    + "[#6X3H]:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:[#6X3H]1),"
                    + "$([CHX4]([CH3X4])[CH2X4][CH3X4]),$([CH2X4][CHX4]([CH3X4])[CH3X4]),"
                    + "$([CH2X4][CH2X4][CH2X4][CH2X4][NX4+,NX3+0]),$([CH2X4][CH2X4][SX2][CH3X4]),"
                    + "$([CH2X4][cX3]1[cX3H][cX3H][cX3H][cX3H][cX3H]1),$([CH2X4][OX2H]),"
                    + "$([CHX4]([CH3X4])[OX2H]),$([CH2X4][cX3]1[cX3H][nX3H][cX3]2[cX3H][cX3H][cX3H][cX3H][cX3]12),"
                    + "$([CH2X4][cX3]1[cX3H][cX3H][cX3]([OHX2,OH0X1-])[cX3H][cX3H]1),"
                    + "$([CHX4]([CH3X4])[CH3X4])])[CX3](=[OX1])[OX2H,OX1-,N])]");
        }

        [TestMethod()]
        public void TestPattern191()
        { // Non-standard amino acid.
            Parse("[$([NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]);!$([$([$([NX3H,NX4H2+]),"
                    + "$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3](=[OX1])[OX2H,OX1-,N]),"
                    + "$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H2][CX3](=[OX1])[OX2H,OX1-,N]),"
                    + "$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H]([$([CH3X4]),$([CH2X4][CH2X4][CH2X4][NHX3][CH0X3]"
                    + "(=[NH2X3+,NHX2+0])[NH2X3]),$([CH2X4][CX3](=[OX1])[NX3H2]),$([CH2X4][CX3](=[OX1])[OH0-,OH]),"
                    + "$([CH2X4][SX2H,SX1H0-]),$([CH2X4][CH2X4][CX3](=[OX1])[OH0-,OH]),$([CH2X4][#6X3]1:"
                    + "[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:"
                    + "[#6X3H]:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),"
                    + "$([#7X3H])]:[#6X3H]1),$([CHX4]([CH3X4])[CH2X4][CH3X4]),$([CH2X4][CHX4]([CH3X4])[CH3X4]),"
                    + "$([CH2X4][CH2X4][CH2X4][CH2X4][NX4+,NX3+0]),$([CH2X4][CH2X4][SX2][CH3X4]),"
                    + "$([CH2X4][cX3]1[cX3H][cX3H][cX3H][cX3H][cX3H]1),$([CH2X4][OX2H]),$([CHX4]([CH3X4])[OX2H]),"
                    + "$([CH2X4][cX3]1[cX3H][nX3H][cX3]2[cX3H][cX3H][cX3H][cX3H][cX3]12),"
                    + "$([CH2X4][cX3]1[cX3H][cX3H][cX3]([OHX2,OH0X1-])[cX3H][cX3H]1),"
                    + "$([CHX4]([CH3X4])[CH3X4])])[CX3](=[OX1])[OX2H,OX1-,N])])]");
        }

        [TestMethod()]
        public void TestPattern192()
        { //Azide group
            Parse("[$(*-[NX2-]-[NX2+]#[NX1]),$(*-[NX2]=[NX2+]=[NX1-])]");
        }

        [TestMethod()]
        public void TestPattern193()
        { // Azide ion
            Parse("[$([NX1-]=[NX2+]=[NX1-]),$([NX1]#[NX2+]-[NX1-2])]");
        }

        [TestMethod()]
        public void TestPattern194()
        { //Azide or azide ion
            Parse("[$([$(*-[NX2-]-[NX2+]#[NX1]),$(*-[NX2]=[NX2+]=[NX1-])]),$([$([NX1-]=[NX2+]=[NX1-]),$([NX1]#[NX2+]-[NX1-2])])]");
        }

        [TestMethod()]
        public void TestPattern195()
        { // Sulfide
            Parse("[#16X2H0]");
        }

        [TestMethod()]
        public void TestPattern196()
        { // Mono-sulfide
            Parse("[#16X2H0][!#16]");
        }

        [TestMethod()]
        public void TestPattern197()
        { // Di-sulfide
            Parse("[#16X2H0][#16X2H0]");
        }

        [TestMethod()]
        public void TestPattern198()
        { // Two sulfides
            Parse("[#16X2H0][!#16].[#16X2H0][!#16]");
        }

        [TestMethod()]
        public void TestPattern199()
        { // Acid/conj-base
            Parse("[OX2H,OX1H0-]");
        }

        [TestMethod()]
        public void TestPattern200()
        { // Non-acid Oxygen
            Parse("[OX2H0]");
        }

        [TestMethod()]
        public void TestPattern201()
        { // Acid/base
            Parse("[H1,H0-]");
        }

        [TestMethod()]
        public void TestPattern202()
        {
            Parse("([Cl!$(Cl~c)].[c!$(c~Cl)])");
        }

        [TestMethod()]
        public void TestPattern203()
        {
            Parse("([Cl]).([c])");
        }

        [TestMethod()]
        public void TestPattern204()
        {
            Parse("([Cl].[c])");
        }

        [TestMethod()]
        public void TestPattern205()
        {
            Parse("[NX3;H2,H1;!$(NC=O)].[NX3;H2,H1;!$(NC=O)]");
        }

        [TestMethod()]
        public void TestPattern206()
        {
            Parse("[#0]");
        }

        [TestMethod()]
        public void TestPattern207()
        {
            Parse("[*!H0,#1]");
        }

        [TestMethod()]
        public void TestPattern208()
        {
            Parse("[#6!H0,#1]");
        }

        [TestMethod()]
        public void TestPattern209()
        {
            Parse("[H,#1]");
        }

        [TestMethod()]
        public void TestPattern210()
        {
            Parse("[!H0;F,Cl,Br,I,N+,$([OH]-*=[!#6]),+]");
        }

        [TestMethod()]
        public void TestPattern211()
        {
            Parse("[CX3](=O)[OX2H1]");
        }

        [TestMethod()]
        public void TestPattern212()
        {
            Parse("[CX3](=O)[OX1H0-,OX2H1]");
        }

        [TestMethod()]
        public void TestPattern213()
        {
            Parse("[$([OH]-*=[!#6])]");
        }

        [TestMethod()]
        public void TestPattern214()
        { // Phosphoric_Acid
            Parse("[$(P(=[OX1])([$([OX2H]),$([OX1-]),$([OX2]P)])([$([OX2H]),$([OX1-]),$([OX2]P)])[$([OX2H]),$([OX1-]),$([OX2]P)]),$([P+]([OX1-])([$([OX"
                    + "2H]),$([OX1-]),$([OX2]P)])([$([OX2H]),$([OX1-]),$([OX2]P)])[$([OX2H]),$([OX1-]),$([OX2]P)])]");
        }

        [TestMethod()]
        public void TestPattern215()
        { // Sulfonic Acid. High specificity.
            Parse("[$([#16X4](=[OX1])(=[OX1])([#6])[OX2H,OX1H0-]),$([#16X4+2]([OX1-])([OX1-])([#6])[OX2H,OX1H0-])]");
        }

        [TestMethod()]
        public void TestPattern216()
        { // Acyl Halide
            Parse("[CX3](=[OX1])[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern217()
        {
            Parse("[NX2-]");
        }

        [TestMethod()]
        public void TestPattern218()
        {
            Parse("[OX2H+]=*");
        }

        [TestMethod()]
        public void TestPattern219()
        {
            Parse("[OX3H2+]");
        }

        [TestMethod()]
        public void TestPattern220()
        {
            Parse("[#6+]");
        }

        [TestMethod()]
        public void TestPattern221()
        {
            Parse("[$([cX2+](:*):*)]");
        }

        [TestMethod()]
        public void TestPattern222()
        {
            Parse("[$([NX1-]=[NX2+]=[NX1-]),$([NX1]#[NX2+]-[NX1-2])]");
        }

        [TestMethod()]
        public void TestPattern223()
        {
            Parse("[+1]~*~*~[-1]");
        }

        [TestMethod()]
        public void TestPattern224()
        {
            Parse("[$([!-0!-1!-2!-3!-4]~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~*~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~*~*~*~*~*~[!+0!+1!+2!+3!+4])]");
        }

        [TestMethod()]
        public void TestPattern225()
        {
            Parse("([!-0!-1!-2!-3!-4].[!+0!+1!+2!+3!+4])");
        }

        [TestMethod()]
        public void TestPattern226()
        { // Hydrogen-bond acceptor, Only hits carbonyl and nitroso
            Parse("[#6,#7;R0]=[#8]");
        }

        [TestMethod()]
        public void TestPattern227()
        { // Hydrogen-bond acceptor
            Parse("[!$([#6,F,Cl,Br,I,o,s,nX3,#7v5,#15v5,#16v4,#16v6,*+1,*+2,*+3])]");
        }

        [TestMethod()]
        public void TestPattern228()
        {
            Parse("[!$([#6,H0,-,-2,-3])]");
        }

        [TestMethod()]
        public void TestPattern229()
        {
            Parse("[!H0;#7,#8,#9]");
        }

        [TestMethod()]
        public void TestPattern230()
        {
            Parse("[O,N;!H0]-*~*-*=[$([C,N;R0]=O)]");
        }

        [TestMethod()]
        public void TestPattern231()
        {
            Parse("[#6;X3v3+0]");
        }

        [TestMethod()]
        public void TestPattern232()
        {
            Parse("[#7;X2v4+0]");
        }

        [TestMethod()]
        public void TestPattern233()
        { // Amino Acid
            Parse("[$([$([NX3H,NX4H2+]),$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3](=[OX1])[OX2H,OX1-,N]),$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H2]["
                    + "CX3](=[OX1])[OX2H,OX1-,N]),$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H]([*])[CX3](=[OX1])[OX2H,OX1-,N])]");
        }

        [TestMethod()]
        public void TestPattern234()
        {
            Parse("[#6][CX3](=O)[$([OX2H0]([#6])[#6]),$([#7])]");
        }

        [TestMethod()]
        public void TestPattern235()
        {
            Parse("[#8]=[C,N]-aaa[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern236()
        {
            Parse("[O,N;!H0;R0]");
        }

        [TestMethod()]
        public void TestPattern237()
        {
            Parse("[#8]=[C,N]");
        }

        [TestMethod()]
        public void TestPattern238()
        { // PCB
            Parse("[$(c:cCl),$(c:c:cCl),$(c:c:c:cCl)]-[$(c:cCl),$(c:c:cCl),$(c:c:c:cCl)]");
        }

        [TestMethod()]
        public void TestPattern239()
        { // Imidazolium Nitrogen
            Parse("[nX3r5+]:c:n");
        }

        [TestMethod()]
        public void TestPattern240()
        { // 1-methyl-2-hydroxy benzene with either a Cl or H at the 5 position.
            Parse("Cc1:c(O):c:c:[$(cCl),$([cH])]:c1");
        }

        [TestMethod()]
        public void TestPattern241()
        { // Nonstandard atom groups.
            Parse("[!#1;!#2;!#3;!#5;!#6;!#7;!#8;!#9;!#11;!#12;!#15;!#16;!#17;!#19;!#20;!#35;!#53]");
        }

        [TestMethod()]
        public void TestRing()
        {
            Parse("[$([C;#12]=1CCCCC1)]");
        }

        [TestMethod()]
        public void TestHydrogen()
        {
            Parse("[H]");
        }

        [TestMethod()]
        public void TestHybridizationNumber1()
        {
            Parse("[^1]");
        }

        [TestMethod()]
        public void TestHybridizationNumber2()
        {
            Parse("[^1&N]");
        }

        [TestMethod()]
        public void TestHybridizationNumber3()
        {
            Parse("[^1&N,^2&C]");
        }

        [TestMethod()]
        [ExpectedException(typeof(ParseException))]
        public void TestHybridizationNumber4()
        {
            Parse("[^]");
        }

        [TestMethod()]
        [ExpectedException(typeof(ParseException))]
        public void TestHybridizationNumber5()
        {
            Parse("[^X]");
        }

        [TestMethod()]
        [ExpectedException(typeof(ParseException))]
        public void TestHybridizationNumber6()
        {
            Parse("[^0]");
        }

        [TestMethod()]
        [ExpectedException(typeof(ParseException))]
        public void TestHybridizationNumber7()
        {
            Parse("[^9]");
        }

        [TestMethod()]
        public void TestNonCHHeavyAtom1()
        {
            Parse("[#X]");
        }

        [TestMethod()]
        public void TestNonCHHeavyAtom2()
        {
            Parse("C#[#X]");
        }

        [TestMethod()]
        public void TestPeriodicGroupNumber1()
        {
            Parse("[G14]");
        }

        [TestMethod()]
        public void TestPeriodicGroupNumber2()
        {
            Parse("[G14,G15]");
        }

        [TestMethod()]
        [ExpectedException(typeof(ParseException))]
        public void TestPeriodicGroupNumber3()
        {
            Parse("[G19]");
        }

        [TestMethod()]
        [ExpectedException(typeof(ParseException))]
        public void TestPeriodicGroupNumber4()
        {
            Parse("[G0]");
        }

        [TestMethod()]
        [ExpectedException(typeof(ParseException))]
        public void TestPeriodicGroupNumber5()
        {
            Parse("[G345]");
        }

        [TestMethod()]
        [ExpectedException(typeof(ParseException))]
        public void TestPeriodicGroupNumber6()
        {
            Parse("[G]");
        }

        [TestMethod()]
        [ExpectedException(typeof(ParseException))]
        public void TestPeriodicGroupNumber7()
        {
            Parse("[GA]");
        }

        [TestMethod()]
        public void TestGroup5Elements()
        {
            Parse("[V,Cr,Mn,Nb,Mo,Tc,Ta,W,Re]");
        }

        [TestMethod()]
        public void EndOnSpace()
        {
            Parse("C ");
        }

        [TestMethod()]
        public void EndOnTab()
        {
            Parse("C\t");
        }

        [TestMethod()]
        public void EndOnNewline()
        {
            Parse("C\n");
        }

        [TestMethod()]
        public void EndOnCarriageReturn()
        {
            Parse("C\r");
        }

        // @cdk.bug 909
        [TestMethod()]
        public void Bug909()
        {
            Parse("O=C1NCCSc2ccccc12");
        }
    }
}
