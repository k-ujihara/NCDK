/* Copyright (C) 2004-2008  The Chemistry Development Kit (CDK) project
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 * (or see http://www.gnu.org/copyleft/lesser.html)
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace NCDK.Smiles.SMARTS.Parser
{
    /// <summary>
    /// JUnit testing routine for SmartsQueryVisitor
    /// </summary>
    // @author Dazhi Jiao
    // @cdk.created 2007-05-10
    // @cdk.module test-smarts
    // @cdk.keyword SMARTS
    [TestClass()]
    public class SmartsQueryVisitorTest : CDKTestCase
    {
        public void Visit(string smarts)
        {
            SMARTSParser parser = new SMARTSParser(new StringReader(smarts));
            ASTStart start = parser.Start();
            SmartsQueryVisitor visitor = new SmartsQueryVisitor(Default.ChemObjectBuilder.Instance);
            visitor.Visit(start, null);
        }

        [TestMethod()]
        public void TestPattern1()
        {
            Visit("[CX4]");
        }

        [TestMethod()]
        public void TestPattern2()
        {
            Visit("[$([CX2](=C)=C)]");
        }

        [TestMethod()]
        public void TestPattern3()
        {
            Visit("[$([CX3]=[CX3])]");
        }

        [TestMethod()]
        public void TestPattern4()
        {
            Visit("[$([CX2]#C)]");
        }

        [TestMethod()]
        public void TestPattern5()
        {
            Visit("[CX3]=[OX1]");
        }

        [TestMethod()]
        public void TestPattern6()
        {
            Visit("[$([CX3]=[OX1]),$([CX3+]-[OX1-])]");
        }

        [TestMethod()]
        public void TestPattern7()
        {
            Visit("[CX3](=[OX1])C");
        }

        [TestMethod()]
        public void TestPattern8()
        {
            Visit("[OX1]=CN");
        }

        [TestMethod()]
        public void TestPattern9()
        {
            Visit("[CX3](=[OX1])O");
        }

        [TestMethod()]
        public void TestPattern10()
        {
            Visit("[CX3](=[OX1])[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern11()
        {
            Visit("[CX3H1](=O)[#6]");
        }

        [TestMethod()]
        public void TestPattern12()
        {
            Visit("[CX3](=[OX1])[OX2][CX3](=[OX1])");
        }

        [TestMethod()]
        public void TestPattern13()
        {
            Visit("[NX3][CX3](=[OX1])[#6]");
        }

        [TestMethod()]
        public void TestPattern14()
        {
            Visit("[NX3][CX3]=[NX3+]");
        }

        [TestMethod()]
        public void TestPattern15()
        {
            Visit("[NX3,NX4+][CX3](=[OX1])[OX2,OX1-]");
        }

        [TestMethod()]
        public void TestPattern16()
        {
            Visit("[NX3][CX3](=[OX1])[OX2H0]");
        }

        [TestMethod()]
        public void TestPattern17()
        {
            Visit("[NX3,NX4+][CX3](=[OX1])[OX2H,OX1-]");
        }

        [TestMethod()]
        public void TestPattern18()
        {
            Visit("[CX3](=O)[O-]");
        }

        [TestMethod()]
        public void TestPattern19()
        {
            Visit("[CX3](=[OX1])(O)O");
        }

        [TestMethod()]
        public void TestPattern20()
        {
            Visit("[CX3](=[OX1])([OX2])[OX2H,OX1H0-1]");
        }

        [TestMethod()]
        public void TestPattern21()
        {
            Visit("[CX3](=O)[OX2H1]");
        }

        [TestMethod()]
        public void TestPattern22()
        {
            Visit("[CX3](=O)[OX1H0-,OX2H1]");
        }

        [TestMethod()]
        public void TestPattern23()
        {
            Visit("[NX3][CX2]#[NX1]");
        }

        [TestMethod()]
        public void TestPattern24()
        {
            Visit("[#6][CX3](=O)[OX2H0][#6]");
        }

        [TestMethod()]
        public void TestPattern25()
        {
            Visit("[#6][CX3](=O)[#6]");
        }

        [TestMethod()]
        public void TestPattern26()
        {
            Visit("[OD2]([#6])[#6]");
        }

        [TestMethod()]
        public void TestPattern27()
        {
            Visit("[H]");
        }

        [TestMethod()]
        public void TestPattern28()
        {
            Visit("[!#1]");
        }

        [TestMethod()]
        public void TestPattern29()
        {
            Visit("[H+]");
        }

        [TestMethod()]
        public void TestPattern30()
        {
            Visit("[+H]");
        }

        [TestMethod()]
        public void TestPattern31()
        {
            Visit("[NX3;H2,H1;!$(NC=O)]");
        }

        [TestMethod()]
        public void TestPattern32()
        {
            Visit("[NX3][CX3]=[CX3]");
        }

        [TestMethod()]
        public void TestPattern33()
        {
            Visit("[NX3;H2,H1;!$(NC=O)].[NX3;H2,H1;!$(NC=O)]");
        }

        [TestMethod()]
        public void TestPattern34()
        {
            Visit("[NX3][$(C=C),$(cc)]");
        }

        [TestMethod()]
        public void TestPattern35()
        {
            Visit("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]");
        }

        [TestMethod()]
        public void TestPattern36()
        {
            Visit("[NX3H2,NH3X4+][CX4H]([*])[CX3](=[OX1])[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[OX2H,OX1-]");
        }

        [TestMethod()]
        public void TestPattern37()
        {
            Visit("[$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H]([*])[CX3](=[OX1])[OX2H,OX1-,N]");
        }

        [TestMethod()]
        public void TestPattern38()
        {
            Visit("[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern39()
        {
            Visit("[CH2X4][CH2X4][CH2X4][NHX3][CH0X3](=[NH2X3+,NHX2+0])[NH2X3]");
        }

        [TestMethod()]
        public void TestPattern40()
        {
            Visit("[CH2X4][CX3](=[OX1])[NX3H2]");
        }

        [TestMethod()]
        public void TestPattern41()
        {
            Visit("[CH2X4][CX3](=[OX1])[OH0-,OH]");
        }

        [TestMethod()]
        public void TestPattern42()
        {
            Visit("[CH2X4][SX2H,SX1H0-]");
        }

        [TestMethod()]
        public void TestPattern43()
        {
            Visit("[CH2X4][CH2X4][CX3](=[OX1])[OH0-,OH]");
        }

        [TestMethod()]
        public void TestPattern44()
        {
            Visit("[$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H2][CX3](=[OX1])[OX2H,OX1-,N])]");
        }

        [TestMethod()]
        public void TestPattern45()
        {
            Visit("[CH2X4][#6X3]1:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:[#6X3H]:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:[#6X3H]1");
        }

        [TestMethod()]
        public void TestPattern47()
        {
            Visit("[CHX4]([CH3X4])[CH2X4][CH3X4]");
        }

        [TestMethod()]
        public void TestPattern48()
        {
            Visit("[CH2X4][CHX4]([CH3X4])[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern49()
        {
            Visit("[CH2X4][CH2X4][CH2X4][CH2X4][NX4+,NX3+0]");
        }

        [TestMethod()]
        public void TestPattern50()
        {
            Visit("[CH2X4][CH2X4][SX2][CH3X4]");
        }

        [TestMethod()]
        public void TestPattern51()
        {
            Visit("[CH2X4][cX3]1[cX3H][cX3H][cX3H][cX3H][cX3H]1");
        }

        [TestMethod()]
        public void TestPattern52()
        {
            Visit("[$([NX3H,NX4H2+]),$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3](=[OX1])[OX2H,OX1-,N]");
        }

        [TestMethod()]
        public void TestPattern53()
        {
            Visit("[CH2X4][OX2H]");
        }

        [TestMethod()]
        public void TestPattern54()
        {
            Visit("[NX3][CX3]=[SX1]");
        }

        [TestMethod()]
        public void TestPattern55()
        {
            Visit("[CHX4]([CH3X4])[OX2H]");
        }

        [TestMethod()]
        public void TestPattern56()
        {
            Visit("[CH2X4][cX3]1[cX3H][nX3H][cX3]2[cX3H][cX3H][cX3H][cX3H][cX3]12");
        }

        [TestMethod()]
        public void TestPattern57()
        {
            Visit("[CH2X4][cX3]1[cX3H][cX3H][cX3]([OHX2,OH0X1-])[cX3H][cX3H]1");
        }

        [TestMethod()]
        public void TestPattern58()
        {
            Visit("[CHX4]([CH3X4])[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern59()
        {
            Visit("[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern60()
        {
            Visit("[CH2X4][CH2X4][CH2X4][NHX3][CH0X3](=[NH2X3+,NHX2+0])[NH2X3]");
        }

        [TestMethod()]
        public void TestPattern61()
        {
            Visit("[CH2X4][CX3](=[OX1])[NX3H2]");
        }

        [TestMethod()]
        public void TestPattern62()
        {
            Visit("[CH2X4][CX3](=[OX1])[OH0-,OH]");
        }

        [TestMethod()]
        public void TestPattern63()
        {
            Visit("[CH2X4][SX2H,SX1H0-]");
        }

        [TestMethod()]
        public void TestPattern64()
        {
            Visit("[CH2X4][CH2X4][CX3](=[OX1])[OH0-,OH]");
        }

        [TestMethod()]
        public void TestPattern65()
        {
            Visit("[CH2X4][#6X3]1:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:[#6X3H]:[$([#7X3H+,#7X2H0+0]:[#6X3H]:[#7X3H]),$([#7X3H])]:[#6X3H]1");
        }

        [TestMethod()]
        public void TestPattern67()
        {
            Visit("[CHX4]([CH3X4])[CH2X4][CH3X4]");
        }

        [TestMethod()]
        public void TestPattern68()
        {
            Visit("[CH2X4][CHX4]([CH3X4])[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern69()
        {
            Visit("[CH2X4][CH2X4][CH2X4][CH2X4][NX4+,NX3+0]");
        }

        [TestMethod()]
        public void TestPattern70()
        {
            Visit("[CH2X4][CH2X4][SX2][CH3X4]");
        }

        [TestMethod()]
        public void TestPattern71()
        {
            Visit("[CH2X4][cX3]1[cX3H][cX3H][cX3H][cX3H][cX3H]1");
        }

        [TestMethod()]
        public void TestPattern72()
        {
            Visit("[CH2X4][OX2H]");
        }

        [TestMethod()]
        public void TestPattern73()
        {
            Visit("[CHX4]([CH3X4])[OX2H]");
        }

        [TestMethod()]
        public void TestPattern74()
        {
            Visit("[CH2X4][cX3]1[cX3H][nX3H][cX3]2[cX3H][cX3H][cX3H][cX3H][cX3]12");
        }

        [TestMethod()]
        public void TestPattern75()
        {
            Visit("[CH2X4][cX3]1[cX3H][cX3H][cX3]([OHX2,OH0X1-])[cX3H][cX3H]1");
        }

        [TestMethod()]
        public void TestPattern76()
        {
            Visit("[CHX4]([CH3X4])[CH3X4]");
        }

        [TestMethod()]
        public void TestPattern77()
        {
            Visit("[$(*-[NX2-]-[NX2+]#[NX1]),$(*-[NX2]=[NX2+]=[NX1-])]");
        }

        [TestMethod()]
        public void TestPattern78()
        {
            Visit("[$([NX1-]=[NX2+]=[NX1-]),$([NX1]#[NX2+]-[NX1-2])]");
        }

        [TestMethod()]
        public void TestPattern79()
        {
            Visit("[#7]");
        }

        [TestMethod()]
        public void TestPattern80()
        {
            Visit("[NX2]=N");
        }

        [TestMethod()]
        public void TestPattern81()
        {
            Visit("[NX2]=[NX2]");
        }

        [TestMethod()]
        public void TestPattern82()
        {
            Visit("[$([NX2]=[NX3+]([O-])[#6]),$([NX2]=[NX3+0](=[O])[#6])]");
        }

        [TestMethod()]
        public void TestPattern83()
        {
            Visit("[$([#6]=[N+]=[N-]),$([#6-]-[N+]#[N])]");
        }

        [TestMethod()]
        public void TestPattern84()
        {
            Visit("[$([nr5]:[nr5,or5,sr5]),$([nr5]:[cr5]:[nr5,or5,sr5])]");
        }

        [TestMethod()]
        public void TestPattern85()
        {
            Visit("[NX3][NX3]");
        }

        [TestMethod()]
        public void TestPattern86()
        {
            Visit("[NX3][NX2]=[*]");
        }

        [TestMethod()]
        public void TestPattern87()
        {
            Visit("[CX3;$([C]([#6])[#6]),$([CH][#6])]=[NX2][#6]");
        }

        [TestMethod()]
        public void TestPattern88()
        {
            Visit("[$([CX3]([#6])[#6]),$([CX3H][#6])]=[$([NX2][#6]),$([NX2H])]");
        }

        [TestMethod()]
        public void TestPattern89()
        {
            Visit("[NX3+]=[CX3]");
        }

        [TestMethod()]
        public void TestPattern90()
        {
            Visit("[CX3](=[OX1])[NX3H][CX3](=[OX1])");
        }

        [TestMethod()]
        public void TestPattern91()
        {
            Visit("[CX3](=[OX1])[NX3H0]([#6])[CX3](=[OX1])");
        }

        [TestMethod()]
        public void TestPattern92()
        {
            Visit("[CX3](=[OX1])[NX3H0]([NX3H0]([CX3](=[OX1]))[CX3](=[OX1]))[CX3](=[OX1])");
        }

        [TestMethod()]
        public void TestPattern93()
        {
            Visit("[$([NX3](=[OX1])(=[OX1])O),$([NX3+]([OX1-])(=[OX1])O)]");
        }

        [TestMethod()]
        public void TestPattern94()
        {
            Visit("[$([OX1]=[NX3](=[OX1])[OX1-]),$([OX1]=[NX3+]([OX1-])[OX1-])]");
        }

        [TestMethod()]
        public void TestPattern95()
        {
            Visit("[NX1]#[CX2]");
        }

        [TestMethod()]
        public void TestPattern96()
        {
            Visit("[CX1-]#[NX2+]");
        }

        [TestMethod()]
        public void TestPattern97()
        {
            Visit("[$([NX3](=O)=O),$([NX3+](=O)[O-])][!#8]");
        }

        [TestMethod()]
        public void TestPattern98()
        {
            Visit("[$([NX3](=O)=O),$([NX3+](=O)[O-])][!#8].[$([NX3](=O)=O),$([NX3+](=O)[O-])][!#8]");
        }

        [TestMethod()]
        public void TestPattern99()
        {
            Visit("[NX2]=[OX1]");
        }

        [TestMethod()]
        public void TestPattern101()
        {
            Visit("[$([#7+][OX1-]),$([#7v5]=[OX1]);!$([#7](~[O])~[O]);!$([#7]=[#7])]");
        }

        [TestMethod()]
        public void TestPattern102()
        {
            Visit("[OX2H]");
        }

        [TestMethod()]
        public void TestPattern103()
        {
            Visit("[#6][OX2H]");
        }

        [TestMethod()]
        public void TestPattern104()
        {
            Visit("[OX2H][CX3]=[OX1]");
        }

        [TestMethod()]
        public void TestPattern105()
        {
            Visit("[OX2H]P");
        }

        [TestMethod()]
        public void TestPattern106()
        {
            Visit("[OX2H][#6X3]=[#6]");
        }

        [TestMethod()]
        public void TestPattern107()
        {
            Visit("[OX2H][cX3]:[c]");
        }

        [TestMethod()]
        public void TestPattern108()
        {
            Visit("[OX2H][$(C=C),$(cc)]");
        }

        [TestMethod()]
        public void TestPattern109()
        {
            Visit("[$([OH]-*=[!#6])]");
        }

        [TestMethod()]
        public void TestPattern110()
        {
            Visit("[OX2,OX1-][OX2,OX1-]");
        }

        [TestMethod()]
        public void TestPattern111()
        { // Phosphoric_acid groups.
            Visit("[$(P(=[OX1])([$([OX2H]),$([OX1-]),$([OX2]P)])([$([OX2H]),$([OX1-]),$([OX2]P)])[$([OX2H]),$([OX1-]),$([OX2]P)]),$([P+]([OX1-])([$([OX"
                    + "2H]),$([OX1-]),$([OX2]P)])([$([OX2H]),$([OX1-]),$([OX2]P)])[$([OX2H]),$([OX1-]),$([OX2]P)])]");
        }

        [TestMethod()]
        public void TestPattern112()
        { // Phosphoric_ester groups.
            Visit("[$(P(=[OX1])([OX2][#6])([$([OX2H]),$([OX1-]),$([OX2][#6])])[$([OX2H]),$([OX1-]),$([OX2][#6]),$([OX2]P)]),$([P+]([OX1-])([OX2][#6])(["
                    + "$([OX2H]),$([OX1-]),$([OX2][#6])])[$([OX2H]),$([OX1-]),$([OX2][#6]),$([OX2]P)])]");
        }

        [TestMethod()]
        public void TestPattern113()
        {
            Visit("[S-][CX3](=S)[#6]");
        }

        [TestMethod()]
        public void TestPattern114()
        {
            Visit("[#6X3](=[SX1])([!N])[!N]");
        }

        [TestMethod()]
        public void TestPattern115()
        {
            Visit("[SX2]");
        }

        [TestMethod()]
        public void TestPattern116()
        {
            Visit("[#16X2H]");
        }

        [TestMethod()]
        public void TestPattern117()
        {
            Visit("[#16!H0]");
        }

        [TestMethod()]
        public void TestPattern118()
        {
            Visit("[NX3][CX3]=[SX1]");
        }

        [TestMethod()]
        public void TestPattern119()
        {
            Visit("[#16X2H0]");
        }

        [TestMethod()]
        public void TestPattern120()
        {
            Visit("[#16X2H0][!#16]");
        }

        [TestMethod()]
        public void TestPattern121()
        {
            Visit("[#16X2H0][#16X2H0]");
        }

        [TestMethod()]
        public void TestPattern122()
        {
            Visit("[#16X2H0][!#16].[#16X2H0][!#16]");
        }

        [TestMethod()]
        public void TestPattern123()
        {
            Visit("[$([#16X3](=[OX1])[OX2H0]),$([#16X3+]([OX1-])[OX2H0])]");
        }

        [TestMethod()]
        public void TestPattern124()
        {
            Visit("[$([#16X3](=[OX1])[OX2H,OX1H0-]),$([#16X3+]([OX1-])[OX2H,OX1H0-])]");
        }

        [TestMethod()]
        public void TestPattern125()
        {
            Visit("[$([#16X4](=[OX1])=[OX1]),$([#16X4+2]([OX1-])[OX1-])]");
        }

        [TestMethod()]
        public void TestPattern126()
        {
            Visit("[$([#16X4](=[OX1])(=[OX1])([#6])[#6]),$([#16X4+2]([OX1-])([OX1-])([#6])[#6])]");
        }

        [TestMethod()]
        public void TestPattern127()
        {
            Visit("[$([#16X4](=[OX1])(=[OX1])([#6])[OX2H,OX1H0-]),$([#16X4+2]([OX1-])([OX1-])([#6])[OX2H,OX1H0-])]");
        }

        [TestMethod()]
        public void TestPattern128()
        {
            Visit("[$([#16X4](=[OX1])(=[OX1])([#6])[OX2H0]),$([#16X4+2]([OX1-])([OX1-])([#6])[OX2H0])]");
        }

        [TestMethod()]
        public void TestPattern129()
        {
            Visit("[$([#16X4]([NX3])(=[OX1])(=[OX1])[#6]),$([#16X4+2]([NX3])([OX1-])([OX1-])[#6])]");
        }

        [TestMethod()]
        public void TestPattern130()
        {
            Visit("[SX4](C)(C)(=O)=N");
        }

        [TestMethod()]
        public void TestPattern131()
        {
            Visit("[$([SX4](=[OX1])(=[OX1])([!O])[NX3]),$([SX4+2]([OX1-])([OX1-])([!O])[NX3])]");
        }

        [TestMethod()]
        public void TestPattern132()
        {
            Visit("[$([#16X3]=[OX1]),$([#16X3+][OX1-])]");
        }

        [TestMethod()]
        public void TestPattern133()
        {
            Visit("[$([#16X3](=[OX1])([#6])[#6]),$([#16X3+]([OX1-])([#6])[#6])]");
        }

        [TestMethod()]
        public void TestPattern134()
        {
            Visit("[$([#16X4](=[OX1])(=[OX1])([OX2H,OX1H0-])[OX2][#6]),$([#16X4+2]([OX1-])([OX1-])([OX2H,OX1H0-])[OX2][#6])]");
        }

        [TestMethod()]
        public void TestPattern135()
        {
            Visit("[$([SX4](=O)(=O)(O)O),$([SX4+2]([O-])([O-])(O)O)]");
        }

        [TestMethod()]
        public void TestPattern136()
        {
            Visit("[$([#16X4](=[OX1])(=[OX1])([OX2][#6])[OX2][#6]),$([#16X4](=[OX1])(=[OX1])([OX2][#6])[OX2][#6])]");
        }

        [TestMethod()]
        public void TestPattern137()
        {
            Visit("[$([#16X4]([NX3])(=[OX1])(=[OX1])[OX2][#6]),$([#16X4+2]([NX3])([OX1-])([OX1-])[OX2][#6])]");
        }

        [TestMethod()]
        public void TestPattern138()
        {
            Visit("[$([#16X4]([NX3])(=[OX1])(=[OX1])[OX2H,OX1H0-]),$([#16X4+2]([NX3])([OX1-])([OX1-])[OX2H,OX1H0-])]");
        }

        [TestMethod()]
        public void TestPattern139()
        {
            Visit("[#16X2][OX2H,OX1H0-]");
        }

        [TestMethod()]
        public void TestPattern140()
        {
            Visit("[#16X2][OX2H0]");
        }

        [TestMethod()]
        public void TestPattern141()
        {
            Visit("[#6][F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern142()
        {
            Visit("[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern143()
        {
            Visit("[F,Cl,Br,I].[F,Cl,Br,I].[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern144()
        {
            Visit("[CX3](=[OX1])[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern145()
        {
            Visit("[$([#6X4@](*)(*)(*)*),$([#6X4@H](*)(*)*)]");
        }

        [TestMethod()]
        public void TestPattern146()
        {
            Visit("[$([cX2+](:*):*)]");
        }

        [TestMethod()]
        public void TestPattern147()
        {
            Visit("[$([cX3](:*):*),$([cX2+](:*):*)]");
        }

        [TestMethod()]
        public void TestPattern148()
        {
            Visit("[$([cX3](:*):*),$([cX2+](:*):*),$([CX3]=*),$([CX2+]=*)]");
        }

        [TestMethod()]
        public void TestPattern149()
        {
            Visit("[$([nX3](:*):*),$([nX2](:*):*),$([#7X2]=*),$([NX3](=*)=*),$([#7X3+](-*)=*),$([#7X3+H]=*)]");
        }

        [TestMethod()]
        public void TestPattern150()
        {
            Visit("[$([#1X1][$([nX3](:*):*),$([nX2](:*):*),$([#7X2]=*),$([NX3](=*)=*),$([#7X3+](-*)=*),$([#7X3+H]=*)])]");
        }

        [TestMethod()]
        public void TestPattern151()
        {
            Visit("[$([NX4+]),$([NX3]);!$(*=*)&!$(*:*)]");
        }

        [TestMethod()]
        public void TestPattern152()
        {
            Visit("[$([#1X1][$([NX4+]),$([NX3]);!$(*=*)&!$(*:*)])]");
        }

        [TestMethod()]
        public void TestPattern153()
        {
            Visit("[$([$([NX3]=O),$([NX3+][O-])])]");
        }

        [TestMethod()]
        public void TestPattern154()
        {
            Visit("[$([$([NX4]=O),$([NX4+][O-])])]");
        }

        [TestMethod()]
        public void TestPattern155()
        {
            Visit("[$([$([NX4]=O),$([NX4+][O-,#0])])]");
        }

        [TestMethod()]
        public void TestPattern156()
        {
            Visit("[$([NX4+]),$([NX4]=*)]");
        }

        [TestMethod()]
        public void TestPattern157()
        {
            Visit("[$([SX3]=N)]");
        }

        [TestMethod()]
        public void TestPattern158()
        {
            Visit("[$([SX1]=[#6])]");
        }

        [TestMethod()]
        public void TestPattern159()
        {
            Visit("[$([NX1]#*)]");
        }

        [TestMethod()]
        public void TestPattern160()
        {
            Visit("[$([OX2])]");
        }

        [TestMethod()]
        public void TestPattern161()
        {
            Visit("[R0;D2][R0;D2][R0;D2][R0;D2]");
        }

        [TestMethod()]
        public void TestPattern162()
        {
            Visit("[R0;D2]~[R0;D2]~[R0;D2]~[R0;D2]");
        }

        [TestMethod()]
        public void TestPattern163()
        {
            Visit("[AR0]~[AR0]~[AR0]~[AR0]~[AR0]~[AR0]~[AR0]~[AR0]");
        }

        [TestMethod()]
        public void TestPattern164()
        {
            Visit("[!$([#6+0]);!$(C(F)(F)F);!$(c(:[!c]):[!c])!$([#6]=,#[!#6])]");
        }

        [TestMethod()]
        public void TestPattern165()
        {
            Visit("[$([#6+0]);!$(C(F)(F)F);!$(c(:[!c]):[!c])!$([#6]=,#[!#6])]");
        }

        [TestMethod()]
        public void TestPattern166()
        {
            Visit("[$([SX1]~P)]");
        }

        [TestMethod()]
        public void TestPattern167()
        {
            Visit("[$([NX3]C=N)]");
        }

        [TestMethod()]
        public void TestPattern168()
        {
            Visit("[$([NX3]N=C)]");
        }

        [TestMethod()]
        public void TestPattern169()
        {
            Visit("[$([NX3]N=N)]");
        }

        [TestMethod()]
        public void TestPattern170()
        {
            Visit("[$([OX2]C=N)]");
        }

        [TestMethod()]
        public void TestPattern171()
        {
            Visit("[!$(*#*)&!D1]-!@[!$(*#*)&!D1]");
        }

        [TestMethod()]
        public void TestPattern172()
        {
            Visit("[$([*R2]([*R])([*R])([*R]))].[$([*R2]([*R])([*R])([*R]))]");
        }

        [TestMethod()]
        public void TestPattern173()
        {
            Visit("*-!:aa-!:*");
        }

        [TestMethod()]
        public void TestPattern174()
        {
            Visit("*-!:aaa-!:*");
        }

        [TestMethod()]
        public void TestPattern175()
        {
            Visit("*-!:aaaa-!:*");
        }

        [TestMethod()]
        public void TestPattern176()
        {
            Visit("*-!@*");
        }

        [TestMethod()]
        public void TestPattern177()
        { // CIS or TRANS double or aromatic bond in a ring
            Visit("*/,\\[R]=,:;@[R]/,\\*");
        }

        [TestMethod()]
        public void TestPattern178()
        { // Fused benzene rings
            Visit("c12ccccc1cccc2");
        }

        [TestMethod()]
        public void TestPattern179()
        {
            Visit("[r;!r3;!r4;!r5;!r6;!r7]");
        }

        [TestMethod()]
        public void TestPattern180()
        {
            Visit("[sX2r5]");
        }

        [TestMethod()]
        public void TestPattern181()
        {
            Visit("[oX2r5]");
        }

        [TestMethod()]
        public void TestPattern182()
        { // Unfused benzene ring
            Visit("[cR1]1[cR1][cR1][cR1][cR1][cR1]1");
        }

        [TestMethod()]
        public void TestPattern183()
        { // Multiple non-fused benzene rings
            Visit("[cR1]1[cR1][cR1][cR1][cR1][cR1]1.[cR1]1[cR1][cR1][cR1][cR1][cR1]1");
        }

        [TestMethod()]
        public void TestPattern184()
        { // Generic amino acid: low specificity.
            Visit("[NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]");
        }

        [TestMethod()]
        public void TestPattern185()
        { //Template for 20 standard a.a.s
            Visit("[$([$([NX3H,NX4H2+]),$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3](=[OX1])[OX2H,OX1-,N]),"
                    + "$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX"
                    + "4H2][CX3](=[OX1])[OX2H,OX1-,N]),$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H]([*])[CX3](=[OX1])[OX2H,OX1-,N])]");
        }

        [TestMethod()]
        public void TestPattern186()
        { // Proline
            Visit("[$([NX3H,NX4H2+]),$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3](=[OX1])[OX2H,OX1-,N]");
        }

        [TestMethod()]
        public void TestPattern187()
        { // Glycine
            Visit("[$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H2][CX3](=[OX1])[OX2H,OX1-,N])]");
        }

        [TestMethod()]
        public void TestPattern188()
        { // Alanine
            Visit("[$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H]([CH3X4])[CX3](=[OX1])[OX2H,OX1-,N]");
        }

        [TestMethod()]
        public void TestPattern189()
        { //18_standard_aa_side_chains.
            Visit("([$([CH3X4]),$([CH2X4][CH2X4][CH2X4][NHX3][CH0X3](=[NH2X3+,NHX2+0])[NH2X3]),"
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
            Visit("[$([$([NX3H,NX4H2+]),$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3]"
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
            Visit("[$([NX3,NX4+][CX4H]([*])[CX3](=[OX1])[O,N]);!$([$([$([NX3H,NX4H2+]),"
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
            Visit("[$(*-[NX2-]-[NX2+]#[NX1]),$(*-[NX2]=[NX2+]=[NX1-])]");
        }

        [TestMethod()]
        public void TestPattern193()
        { // Azide ion
            Visit("[$([NX1-]=[NX2+]=[NX1-]),$([NX1]#[NX2+]-[NX1-2])]");
        }

        [TestMethod()]
        public void TestPattern194()
        { //Azide or azide ion
            Visit("[$([$(*-[NX2-]-[NX2+]#[NX1]),$(*-[NX2]=[NX2+]=[NX1-])]),$([$([NX1-]=[NX2+]=[NX1-]),$([NX1]#[NX2+]-[NX1-2])])]");
        }

        [TestMethod()]
        public void TestPattern195()
        { // Sulfide
            Visit("[#16X2H0]");
        }

        [TestMethod()]
        public void TestPattern196()
        { // Mono-sulfide
            Visit("[#16X2H0][!#16]");
        }

        [TestMethod()]
        public void TestPattern197()
        { // Di-sulfide
            Visit("[#16X2H0][#16X2H0]");
        }

        [TestMethod()]
        public void TestPattern198()
        { // Two sulfides
            Visit("[#16X2H0][!#16].[#16X2H0][!#16]");
        }

        [TestMethod()]
        public void TestPattern199()
        { // Acid/conj-base
            Visit("[OX2H,OX1H0-]");
        }

        [TestMethod()]
        public void TestPattern200()
        { // Non-acid Oxygen
            Visit("[OX2H0]");
        }

        [TestMethod()]
        public void TestPattern201()
        { // Acid/base
            Visit("[H1,H0-]");
        }

        [TestMethod()]
        public void TestPattern202()
        {
            Visit("([Cl!$(Cl~c)].[c!$(c~Cl)])");
        }

        [TestMethod()]
        public void TestPattern203()
        {
            Visit("([Cl]).([c])");
        }

        [TestMethod()]
        public void TestPattern204()
        {
            Visit("([Cl].[c])");
        }

        [TestMethod()]
        public void TestPattern205()
        {
            Visit("[NX3;H2,H1;!$(NC=O)].[NX3;H2,H1;!$(NC=O)]");
        }

        [TestMethod()]
        public void TestPattern206()
        {
            Visit("[#0]");
        }

        [TestMethod()]
        public void TestPattern207()
        {
            Visit("[*!H0,#1]");
        }

        [TestMethod()]
        public void TestPattern208()
        {
            Visit("[#6!H0,#1]");
        }

        [TestMethod()]
        public void TestPattern209()
        {
            Visit("[H,#1]");
        }

        [TestMethod()]
        public void TestPattern210()
        {
            Visit("[!H0;F,Cl,Br,I,N+,$([OH]-*=[!#6]),+]");
        }

        [TestMethod()]
        public void TestPattern211()
        {
            Visit("[CX3](=O)[OX2H1]");
        }

        [TestMethod()]
        public void TestPattern212()
        {
            Visit("[CX3](=O)[OX1H0-,OX2H1]");
        }

        [TestMethod()]
        public void TestPattern213()
        {
            Visit("[$([OH]-*=[!#6])]");
        }

        [TestMethod()]
        public void TestPattern214()
        { // Phosphoric_Acid
            Visit("[$(P(=[OX1])([$([OX2H]),$([OX1-]),$([OX2]P)])([$([OX2H]),$([OX1-]),$([OX2]P)])[$([OX2H]),$([OX1-]),$([OX2]P)]),$([P+]([OX1-])([$([OX"
                    + "2H]),$([OX1-]),$([OX2]P)])([$([OX2H]),$([OX1-]),$([OX2]P)])[$([OX2H]),$([OX1-]),$([OX2]P)])]");
        }

        [TestMethod()]
        public void TestPattern215()
        { // Sulfonic Acid. High specificity.
            Visit("[$([#16X4](=[OX1])(=[OX1])([#6])[OX2H,OX1H0-]),$([#16X4+2]([OX1-])([OX1-])([#6])[OX2H,OX1H0-])]");
        }

        [TestMethod()]
        public void TestPattern216()
        { // Acyl Halide
            Visit("[CX3](=[OX1])[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern217()
        {
            Visit("[NX2-]");
        }

        [TestMethod()]
        public void TestPattern218()
        {
            Visit("[OX2H+]=*");
        }

        [TestMethod()]
        public void TestPattern219()
        {
            Visit("[OX3H2+]");
        }

        [TestMethod()]
        public void TestPattern220()
        {
            Visit("[#6+]");
        }

        [TestMethod()]
        public void TestPattern221()
        {
            Visit("[$([cX2+](:*):*)]");
        }

        [TestMethod()]
        public void TestPattern222()
        {
            Visit("[$([NX1-]=[NX2+]=[NX1-]),$([NX1]#[NX2+]-[NX1-2])]");
        }

        [TestMethod()]
        public void TestPattern223()
        {
            Visit("[+1]~*~*~[-1]");
        }

        [TestMethod()]
        public void TestPattern224()
        {
            Visit("[$([!-0!-1!-2!-3!-4]~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~*~*~*~*~[!+0!+1!+2!+3!+4]),$([!-0!-1!-2!-3!-4]~*~*~*~*~*~*~*~*~*~[!+0!+1!+2!+3!+4])]");
        }

        [TestMethod()]
        public void TestPattern225()
        {
            Visit("([!-0!-1!-2!-3!-4].[!+0!+1!+2!+3!+4])");
        }

        [TestMethod()]
        public void TestPattern226()
        { // Hydrogen-bond acceptor, Only hits carbonyl and nitroso
            Visit("[#6,#7;R0]=[#8]");
        }

        [TestMethod()]
        public void TestPattern227()
        { // Hydrogen-bond acceptor
            Visit("[!$([#6,F,Cl,Br,I,o,s,nX3,#7v5,#15v5,#16v4,#16v6,*+1,*+2,*+3])]");
        }

        [TestMethod()]
        public void TestPattern228()
        {
            Visit("[!$([#6,H0,-,-2,-3])]");
        }

        [TestMethod()]
        public void TestPattern229()
        {
            Visit("[!H0;#7,#8,#9]");
        }

        [TestMethod()]
        public void TestPattern230()
        {
            Visit("[O,N;!H0]-*~*-*=[$([C,N;R0]=O)]");
        }

        [TestMethod()]
        public void TestPattern231()
        {
            Visit("[#6;X3v3+0]");
        }

        [TestMethod()]
        public void TestPattern232()
        {
            Visit("[#7;X2v4+0]");
        }

        [TestMethod()]
        public void TestPattern233()
        { // Amino Acid
            Visit("[$([$([NX3H,NX4H2+]),$([NX3](C)(C)(C))]1[CX4H]([CH2][CH2][CH2]1)[CX3](=[OX1])[OX2H,OX1-,N]),$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H2]["
                    + "CX3](=[OX1])[OX2H,OX1-,N]),$([$([NX3H2,NX4H3+]),$([NX3H](C)(C))][CX4H]([*])[CX3](=[OX1])[OX2H,OX1-,N])]");
        }

        [TestMethod()]
        public void TestPattern234()
        {
            Visit("[#6][CX3](=O)[$([OX2H0]([#6])[#6]),$([#7])]");
        }

        [TestMethod()]
        public void TestPattern235()
        {
            Visit("[#8]=[C,N]-aaa[F,Cl,Br,I]");
        }

        [TestMethod()]
        public void TestPattern236()
        {
            Visit("[O,N;!H0;R0]");
        }

        [TestMethod()]
        public void TestPattern237()
        {
            Visit("[#8]=[C,N]");
        }

        [TestMethod()]
        public void TestPattern238()
        { // PCB
            Visit("[$(c:cCl),$(c:c:cCl),$(c:c:c:cCl)]-[$(c:cCl),$(c:c:cCl),$(c:c:c:cCl)]");
        }

        [TestMethod()]
        public void TestPattern239()
        { // Imidazolium Nitrogen
            Visit("[nX3r5+]:c:n");
        }

        [TestMethod()]
        public void TestPattern240()
        { // 1-methyl-2-hydroxy benzene with either a Cl or H at the 5 position.
            Visit("[c;$([*Cl]),$([*H1])]1ccc(O)c(C)c1");
            Visit("Cc1:c(O):c:c:[$(cCl),$([cH])]:c1");
        }

        [TestMethod()]
        public void TestPattern241()
        { // Nonstandard atom groups.
            Visit("[!#1;!#2;!#3;!#5;!#6;!#7;!#8;!#9;!#11;!#12;!#15;!#16;!#17;!#19;!#20;!#35;!#53]");
        }

        [TestMethod()]
        public void TestRing()
        {
            Visit("[$([C;#12]=1CCCCC1)]");
        }
    }
}
