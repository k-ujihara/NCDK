/*  Copyright (C) 2003-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *                     2009  Mark Rijnbeek <markr@ebi.ac.uk>
 *                     2009  Mark Rijnbeek <mark_rynbeek@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Geometries;
using NCDK.IO;
using NCDK.SGroups;
using NCDK.Smiles;
using NCDK.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NCDK.Numerics;
using NCDK.Stereo;

namespace NCDK.Layout
{
    /// <summary>
    ///  A set of test cases for the StructureDiagramGenerator
    /// </summary>
    // @cdk.module test-sdg
    // @author     steinbeck
    // @cdk.created    August 29, 2003
    [TestClass()]
    public class StructureDiagramGeneratorTest : CDKTestCase
    {
        private static readonly StructureDiagramGenerator SDG = new StructureDiagramGenerator();

        static StructureDiagramGeneratorTest()
        {
            SDG.UseIdentityTemplates = true;
        }

        public static IAtomContainer Layout(IAtomContainer mol)
        {
            SDG.SetMolecule(mol, false);
            SDG.GenerateCoordinates();
            return mol;
        }

        public void VisualBugPMR()
        {
            string filename = "NCDK.Data.CML.SL0016a.cml";
            Stream ins = ResourceLoader.GetAsStream(filename);
            CMLReader reader = new CMLReader(ins);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer mol = model.MoleculeSet[0];
            //MoleculeViewer2D.Display(mol, true, false, JFrame.DO_NOTHING_ON_CLOSE,"");
        }

        // @cdk.bug 1670871
        [TestMethod()]
        [Timeout(5000)]
        public void TestBugLecture2007()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            //IAtomContainer mol = sp.ParseSmiles("Oc1nc(Nc2c(nn(c12)C)CCC)c3cc(ccc3(OCC))S(=O)(=O)N4CCN(C)CC4");
            IAtomContainer mol = sp.ParseSmiles("O=C(N1CCN(CC1)CCCN(C)C)C3(C=2C=CC(=CC=2)C)(CCCCC3)");

            //IAtomContainer mol = sp.ParseSmiles("C1CCC1CCCCCCCC1CC1");

            IAtomContainer ac = Layout(mol);
            //        MoleculeViewer2D.Display(new AtomContainer(ac), false);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void TestAlphaPinene()
        {
            IAtomContainer m = TestMoleculeFactory.MakeAlphaPinene();
            IAtomContainer ac = Layout(m);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void TestBridgedHydrogen()
        {
            IAtomContainer mol = new AtomContainer();
            IAtom carbon1 = new Atom("C");
            IAtom carbon2 = new Atom("C");
            IAtom bridgingHydrogen = new Atom("H");
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(bridgingHydrogen);
            mol.Atoms.Add(carbon2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            Layout(mol);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(mol));
        }
        
        [TestMethod()]
        [Timeout(5000)]
        public void TestBiphenyl()
        {
            IAtomContainer m = TestMoleculeFactory.MakeBiphenyl();
            IAtomContainer ac = Layout(m);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void Test4x3CondensedRings()
        {
            IAtomContainer m = TestMoleculeFactory.Make4x3CondensedRings();
            IAtomContainer ac = Layout(m);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void TestPhenylEthylBenzene()
        {
            IAtomContainer m = TestMoleculeFactory.MakePhenylEthylBenzene();
            IAtomContainer ac = Layout(m);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void TestSpiroRings()
        {
            IAtomContainer m = TestMoleculeFactory.MakeSpiroRings();
            IAtomContainer ac = Layout(m);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void TestMethylDecaline()
        {
            IAtomContainer m = TestMoleculeFactory.MakeMethylDecaline();
            IAtomContainer ac = Layout(m);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void TestBranchedAliphatic()
        {
            IAtomContainer m = TestMoleculeFactory.MakeBranchedAliphatic();
            IAtomContainer ac = Layout(m);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void TestDiamantane()
        {
            IAtomContainer m = TestMoleculeFactory.MakeDiamantane();
            IAtomContainer ac = Layout(m);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        // @cdk.bug 1670871
        [TestMethod()]
        [Timeout(5000)]
        public void TestBug1670871()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CC(=O)OC1C=CC(SC23CC4CC(CC(C4)C2)C3)N(C1SC56CC7CC(CC(C7)C5)C6)C(C)=O");
            IAtomContainer ac = Layout(mol);
            //MoleculeViewer2D.Display(new AtomContainer(ac), false);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void TestEthylCyclohexane()
        {
            IAtomContainer m = TestMoleculeFactory.MakeEthylCyclohexane();
            IAtomContainer ac = Layout(m);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        [TestMethod()]
        [Timeout(5000)]
        public void TestBicycloRings()
        {
            IAtomContainer m = TestMoleculeFactory.MakeBicycloRings();
            IAtomContainer ac = Layout(m);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        public IAtomContainer MakeJhao3()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=C1C2=CC13(CC23)");
            return mol;
        }

        public IAtomContainer MakeJhao4()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCC3C1CC23(CC12)");
            return mol;
        }

        [TestMethod()]
        [Timeout(5000)]
        public void TestBenzene()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("c1ccccc1");
            IAtomContainer ac = Layout(mol);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        // @cdk.bug 780545
        [TestMethod()]
        [Timeout(5000)]
        public void TestBug780545()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C"));
            IAtomContainer ac = Layout(mol);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        // @cdk.bug 1598409
        [TestMethod()]
        [Timeout(5000)]
        public void TestBug1598409()
        {
            string smiles = "c1(:c(:c2-C(-c3:c(-C(=O)-c:2:c(:c:1-[H])-[H]):c(:c(:c(:c:3-[H])-[H])-N(-[H])-[H])-[H])=O)-[H])-[H]";
            SmilesParser parser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer cdkMol = parser.ParseSmiles(smiles);
            Layout(cdkMol);
        }

        // @cdk.bug 1572062
        [TestMethod()]
        [Timeout(5000)]
        public void TestBug1572062()
        {
            string filename = "NCDK.Data.MDL.sdg_test.mol";

            //        set up molecule reader
            Stream ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader molReader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);

            //        read molecule
            IAtomContainer molecule = molReader.Read(Default.ChemObjectBuilder.Instance.NewAtomContainer());

            //        rebuild 2D coordinates
            for (int i = 0; i < 10; i++)
            {
                Layout(molecule);
            }
        }

        // @cdk.bug 884993
        public void TestBug884993()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[N+](=O)([O-])C1=C(O)C(=CC(=C1)[N+](=O)[O-])[N+](=O)[O-].C23N(CCCC2)CCCC3");
            IAtomContainer ac = Layout(mol);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(ac));
        }

        /// <summary>
        /// Test for bug #1677912 "SDG JUnit test hangs"
        /// The SMILES parsing takes hence a longer timeout.
        /// </summary>
        // @cdk.bug 1677912
        [TestMethod()]
        [Timeout(10000)]
        public void TestBug1677912SDGHangs()
        {
            // Parse the SMILES
            string smiles = "[NH](-[CH]1-[CH]2-[CH2]-[CH]3-[CH2]-[CH]-1-[CH2]-[CH](-[CH2]-2)-[CH2]-3)-C(=O)-C(=O)-[CH2]-c1:n:c(:c(:[cH]:c:1-C(=O)-O-[CH3])-C(=O)-O-[CH3])-[CH2]-C(=O)-C(=O)-[NH]-[CH]1-[CH]2-[CH2]-[CH]3-[CH2]-[CH]-1-[CH2]-[CH](-[CH2]-2)-[CH2]-3";
            SmilesParser smilesParser = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer molecule = smilesParser.ParseSmiles(smiles);

            // Generate 2D coordinates
            Layout(molecule);

            // Test completed, no timeout occurred
        }

        // @cdk.bug 1714794
        [TestMethod()]
        [Timeout(5000)]
        public void TestBug1714794()
        {
            string problematicMol2AsSmiles = "N1c2c(c3c(c4c(c(c3O)C)OC(OC=CC(C(C(C(C(C(C(C(C=CC=C(C1=O)C)C)O)C)O)C)OC(=O)C)C)OC)(C4=O)C)c(c2C=NN(C12CC3CC(C1)CC(C2)C3)C)O)O";
            SmilesParser parser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer cdkMol = parser.ParseSmiles(problematicMol2AsSmiles);
            long t0 = DateTime.Now.Ticks;
            Layout(cdkMol);
            long t1 = DateTime.Now.Ticks;
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(cdkMol));

            string problematicMol2 = "@<TRIPOS>MOLECULE\n" + "mol_197219.smi\n" + " 129 135 0 0 0\n" + "SMALL\n"
                    + "GASTEIGER\n" + "Energy = 0\n" + "\n" + "@<TRIPOS>ATOM\n"
                    + "      1 N1          0.0000    0.0000    0.0000 N.am    1  <1>        -0.2782\n"
                    + "      2 H1          0.0000    0.0000    0.0000 H       1  <1>         0.1552\n"
                    + "      3 C1          0.0000    0.0000    0.0000 C.ar    1  <1>         0.0886\n"
                    + "      4 C2          0.0000    0.0000    0.0000 C.ar    1  <1>         0.1500\n"
                    + "      5 C3          0.0000    0.0000    0.0000 C.ar    1  <1>         0.0714\n"
                    + "      6 C4          0.0000    0.0000    0.0000 C.ar    1  <1>         0.0456\n"
                    + "      7 C5          0.0000    0.0000    0.0000 C.ar    1  <1>         0.0788\n"
                    + "      8 C6          0.0000    0.0000    0.0000 C.ar    1  <1>         0.1435\n"
                    + "      9 C7          0.0000    0.0000    0.0000 C.ar    1  <1>         0.0342\n"
                    + "     10 C8          0.0000    0.0000    0.0000 C.ar    1  <1>         0.1346\n"
                    + "     11 O1          0.0000    0.0000    0.0000 O.3     1  <1>        -0.5057\n"
                    + "     12 H2          0.0000    0.0000    0.0000 H       1  <1>         0.2922\n"
                    + "     13 C9          0.0000    0.0000    0.0000 C.3     1  <1>        -0.0327\n"
                    + "     14 H3          0.0000    0.0000    0.0000 H       1  <1>         0.0280\n"
                    + "     15 H4          0.0000    0.0000    0.0000 H       1  <1>         0.0280\n"
                    + "     16 H5          0.0000    0.0000    0.0000 H       1  <1>         0.0280\n"
                    + "     17 O2          0.0000    0.0000    0.0000 O.3     1  <1>        -0.4436\n"
                    + "     18 C10         0.0000    0.0000    0.0000 C.3     1  <1>         0.3143\n"
                    + "     19 O3          0.0000    0.0000    0.0000 O.2     1  <1>        -0.4528\n"
                    + "     20 C11         0.0000    0.0000    0.0000 C.2     1  <1>         0.0882\n"
                    + "     21 H6          0.0000    0.0000    0.0000 H       1  <1>         0.1022\n"
                    + "     22 C12         0.0000    0.0000    0.0000 C.2     1  <1>        -0.0208\n"
                    + "     23 H7          0.0000    0.0000    0.0000 H       1  <1>         0.0628\n"
                    + "     24 C13         0.0000    0.0000    0.0000 C.3     1  <1>         0.0854\n"
                    + "     25 H8          0.0000    0.0000    0.0000 H       1  <1>         0.0645\n"
                    + "     26 C14         0.0000    0.0000    0.0000 C.3     1  <1>         0.0236\n"
                    + "     27 H9          0.0000    0.0000    0.0000 H       1  <1>         0.0362\n"
                    + "     28 C15         0.0000    0.0000    0.0000 C.3     1  <1>         0.1131\n"
                    + "     29 H10         0.0000    0.0000    0.0000 H       1  <1>         0.0741\n"
                    + "     30 C16         0.0000    0.0000    0.0000 C.3     1  <1>         0.0200\n"
                    + "     31 H11         0.0000    0.0000    0.0000 H       1  <1>         0.0359\n"
                    + "     32 C17         0.0000    0.0000    0.0000 C.3     1  <1>         0.0661\n"
                    + "     33 H12         0.0000    0.0000    0.0000 H       1  <1>         0.0600\n"
                    + "     34 C18         0.0000    0.0000    0.0000 C.3     1  <1>         0.0091\n"
                    + "     35 H13         0.0000    0.0000    0.0000 H       1  <1>         0.0348\n"
                    + "     36 C19         0.0000    0.0000    0.0000 C.3     1  <1>         0.0661\n"
                    + "     37 H14         0.0000    0.0000    0.0000 H       1  <1>         0.0602\n"
                    + "     38 C20         0.0000    0.0000    0.0000 C.3     1  <1>         0.0009\n"
                    + "     39 H15         0.0000    0.0000    0.0000 H       1  <1>         0.0365\n"
                    + "     40 C21         0.0000    0.0000    0.0000 C.2     1  <1>        -0.0787\n"
                    + "     41 H16         0.0000    0.0000    0.0000 H       1  <1>         0.0576\n"
                    + "     42 C22         0.0000    0.0000    0.0000 C.2     1  <1>        -0.0649\n"
                    + "     43 H17         0.0000    0.0000    0.0000 H       1  <1>         0.0615\n"
                    + "     44 C23         0.0000    0.0000    0.0000 C.2     1  <1>        -0.0542\n"
                    + "     45 H18         0.0000    0.0000    0.0000 H       1  <1>         0.0622\n"
                    + "     46 C24         0.0000    0.0000    0.0000 C.2     1  <1>         0.0115\n"
                    + "     47 C25         0.0000    0.0000    0.0000 C.2     1  <1>         0.2441\n"
                    + "     48 O4          0.0000    0.0000    0.0000 O.2     1  <1>        -0.2702\n"
                    + "     49 C26         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0348\n"
                    + "     50 H19         0.0000    0.0000    0.0000 H       1  <1>         0.0279\n"
                    + "     51 H20         0.0000    0.0000    0.0000 H       1  <1>         0.0279\n"
                    + "     52 H21         0.0000    0.0000    0.0000 H       1  <1>         0.0279\n"
                    + "     53 C27         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0566\n"
                    + "     54 H22         0.0000    0.0000    0.0000 H       1  <1>         0.0236\n"
                    + "     55 H23         0.0000    0.0000    0.0000 H       1  <1>         0.0236\n"
                    + "     56 H24         0.0000    0.0000    0.0000 H       1  <1>         0.0236\n"
                    + "     57 O5          0.0000    0.0000    0.0000 O.3     1  <1>        -0.3909\n"
                    + "     58 H25         0.0000    0.0000    0.0000 H       1  <1>         0.2098\n"
                    + "     59 C28         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0577\n"
                    + "     60 H26         0.0000    0.0000    0.0000 H       1  <1>         0.0234\n"
                    + "     61 H27         0.0000    0.0000    0.0000 H       1  <1>         0.0234\n"
                    + "     62 H28         0.0000    0.0000    0.0000 H       1  <1>         0.0234\n"
                    + "     63 O6          0.0000    0.0000    0.0000 O.3     1  <1>        -0.3910\n"
                    + "     64 H29         0.0000    0.0000    0.0000 H       1  <1>         0.2098\n"
                    + "     65 C29         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0567\n"
                    + "     66 H30         0.0000    0.0000    0.0000 H       1  <1>         0.0234\n"
                    + "     67 H31         0.0000    0.0000    0.0000 H       1  <1>         0.0234\n"
                    + "     68 H32         0.0000    0.0000    0.0000 H       1  <1>         0.0234\n"
                    + "     69 O7          0.0000    0.0000    0.0000 O.3     1  <1>        -0.4608\n"
                    + "     70 C30         0.0000    0.0000    0.0000 C.2     1  <1>         0.3042\n"
                    + "     71 O8          0.0000    0.0000    0.0000 O.2     1  <1>        -0.2512\n"
                    + "     72 C31         0.0000    0.0000    0.0000 C.3     1  <1>         0.0332\n"
                    + "     73 H33         0.0000    0.0000    0.0000 H       1  <1>         0.0342\n"
                    + "     74 H34         0.0000    0.0000    0.0000 H       1  <1>         0.0342\n"
                    + "     75 H35         0.0000    0.0000    0.0000 H       1  <1>         0.0342\n"
                    + "     76 C32         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0564\n"
                    + "     77 H36         0.0000    0.0000    0.0000 H       1  <1>         0.0234\n"
                    + "     78 H37         0.0000    0.0000    0.0000 H       1  <1>         0.0234\n"
                    + "     79 H38         0.0000    0.0000    0.0000 H       1  <1>         0.0234\n"
                    + "     80 O9          0.0000    0.0000    0.0000 O.3     1  <1>        -0.3753\n"
                    + "     81 C33         0.0000    0.0000    0.0000 C.3     1  <1>         0.0372\n"
                    + "     82 H39         0.0000    0.0000    0.0000 H       1  <1>         0.0524\n"
                    + "     83 H40         0.0000    0.0000    0.0000 H       1  <1>         0.0524\n"
                    + "     84 H41         0.0000    0.0000    0.0000 H       1  <1>         0.0524\n"
                    + "     85 C34         0.0000    0.0000    0.0000 C.2     1  <1>         0.2505\n"
                    + "     86 O10         0.0000    0.0000    0.0000 O.2     1  <1>        -0.2836\n"
                    + "     87 C35         0.0000    0.0000    0.0000 C.3     1  <1>         0.0210\n"
                    + "     88 H42         0.0000    0.0000    0.0000 H       1  <1>         0.0309\n"
                    + "     89 H43         0.0000    0.0000    0.0000 H       1  <1>         0.0309\n"
                    + "     90 H44         0.0000    0.0000    0.0000 H       1  <1>         0.0309\n"
                    + "     91 C36         0.0000    0.0000    0.0000 C.ar    1  <1>         0.1361\n"
                    + "     92 C37         0.0000    0.0000    0.0000 C.ar    1  <1>         0.0613\n"
                    + "     93 C38         0.0000    0.0000    0.0000 C.2     1  <1>         0.0580\n"
                    + "     94 H45         0.0000    0.0000    0.0000 H       1  <1>         0.0853\n"
                    + "     95 N2          0.0000    0.0000    0.0000 N.2     1  <1>        -0.1915\n"
                    + "     96 N3          0.0000    0.0000    0.0000 N.pl3   1  <1>        -0.2525\n"
                    + "     97 C39         0.0000    0.0000    0.0000 C.3     1  <1>         0.0525\n"
                    + "     98 C40         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0271\n"
                    + "     99 H46         0.0000    0.0000    0.0000 H       1  <1>         0.0289\n"
                    + "    100 H47         0.0000    0.0000    0.0000 H       1  <1>         0.0289\n"
                    + "    101 C41         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0385\n"
                    + "    102 H48         0.0000    0.0000    0.0000 H       1  <1>         0.0302\n"
                    + "    103 C42         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0472\n"
                    + "    104 H49         0.0000    0.0000    0.0000 H       1  <1>         0.0271\n"
                    + "    105 H50         0.0000    0.0000    0.0000 H       1  <1>         0.0271\n"
                    + "    106 C43         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0385\n"
                    + "    107 H51         0.0000    0.0000    0.0000 H       1  <1>         0.0302\n"
                    + "    108 C44         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0271\n"
                    + "    109 H52         0.0000    0.0000    0.0000 H       1  <1>         0.0289\n"
                    + "    110 H53         0.0000    0.0000    0.0000 H       1  <1>         0.0289\n"
                    + "    111 C45         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0472\n"
                    + "    112 H54         0.0000    0.0000    0.0000 H       1  <1>         0.0271\n"
                    + "    113 H55         0.0000    0.0000    0.0000 H       1  <1>         0.0271\n"
                    + "    114 C46         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0385\n"
                    + "    115 H56         0.0000    0.0000    0.0000 H       1  <1>         0.0302\n"
                    + "    116 C47         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0271\n"
                    + "    117 H57         0.0000    0.0000    0.0000 H       1  <1>         0.0289\n"
                    + "    118 H58         0.0000    0.0000    0.0000 H       1  <1>         0.0289\n"
                    + "    119 C48         0.0000    0.0000    0.0000 C.3     1  <1>        -0.0472\n"
                    + "    120 H59         0.0000    0.0000    0.0000 H       1  <1>         0.0271\n"
                    + "    121 H60         0.0000    0.0000    0.0000 H       1  <1>         0.0271\n"
                    + "    122 C49         0.0000    0.0000    0.0000 C.3     1  <1>         0.0189\n"
                    + "    123 H61         0.0000    0.0000    0.0000 H       1  <1>         0.0444\n"
                    + "    124 H62         0.0000    0.0000    0.0000 H       1  <1>         0.0444\n"
                    + "    125 H63         0.0000    0.0000    0.0000 H       1  <1>         0.0444\n"
                    + "    126 O11         0.0000    0.0000    0.0000 O.3     1  <1>        -0.5054\n"
                    + "    127 H64         0.0000    0.0000    0.0000 H       1  <1>         0.2922\n"
                    + "    128 O12         0.0000    0.0000    0.0000 O.3     1  <1>        -0.5042\n"
                    + "    129 H65         0.0000    0.0000    0.0000 H       1  <1>         0.2923\n" + "@<TRIPOS>BOND\n"
                    + "     1     1     2    1\n" + "     2     1     3    1\n" + "     3     3     4   ar\n"
                    + "     4     4     5   ar\n" + "     5     5     6   ar\n" + "     6     6     7   ar\n"
                    + "     7     7     8   ar\n" + "     8     8     9   ar\n" + "     9     9    10   ar\n"
                    + "    10     5    10   ar\n" + "    11    10    11    1\n" + "    12    11    12    1\n"
                    + "    13     9    13    1\n" + "    14    13    14    1\n" + "    15    13    15    1\n"
                    + "    16    13    16    1\n" + "    17     8    17    1\n" + "    18    17    18    1\n"
                    + "    19    18    19    1\n" + "    20    19    20    1\n" + "    21    20    21    1\n"
                    + "    22    20    22    2\n" + "    23    22    23    1\n" + "    24    22    24    1\n"
                    + "    25    24    25    1\n" + "    26    24    26    1\n" + "    27    26    27    1\n"
                    + "    28    26    28    1\n" + "    29    28    29    1\n" + "    30    28    30    1\n"
                    + "    31    30    31    1\n" + "    32    30    32    1\n" + "    33    32    33    1\n"
                    + "    34    32    34    1\n" + "    35    34    35    1\n" + "    36    34    36    1\n"
                    + "    37    36    37    1\n" + "    38    36    38    1\n" + "    39    38    39    1\n"
                    + "    40    38    40    1\n" + "    41    40    41    1\n" + "    42    40    42    2\n"
                    + "    43    42    43    1\n" + "    44    42    44    1\n" + "    45    44    45    1\n"
                    + "    46    44    46    2\n" + "    47    46    47    1\n" + "    48     1    47   am\n"
                    + "    49    47    48    2\n" + "    50    46    49    1\n" + "    51    49    50    1\n"
                    + "    52    49    51    1\n" + "    53    49    52    1\n" + "    54    38    53    1\n"
                    + "    55    53    54    1\n" + "    56    53    55    1\n" + "    57    53    56    1\n"
                    + "    58    36    57    1\n" + "    59    57    58    1\n" + "    60    34    59    1\n"
                    + "    61    59    60    1\n" + "    62    59    61    1\n" + "    63    59    62    1\n"
                    + "    64    32    63    1\n" + "    65    63    64    1\n" + "    66    30    65    1\n"
                    + "    67    65    66    1\n" + "    68    65    67    1\n" + "    69    65    68    1\n"
                    + "    70    28    69    1\n" + "    71    69    70    1\n" + "    72    70    71    2\n"
                    + "    73    70    72    1\n" + "    74    72    73    1\n" + "    75    72    74    1\n"
                    + "    76    72    75    1\n" + "    77    26    76    1\n" + "    78    76    77    1\n"
                    + "    79    76    78    1\n" + "    80    76    79    1\n" + "    81    24    80    1\n"
                    + "    82    80    81    1\n" + "    83    81    82    1\n" + "    84    81    83    1\n"
                    + "    85    81    84    1\n" + "    86    18    85    1\n" + "    87     7    85    1\n"
                    + "    88    85    86    2\n" + "    89    18    87    1\n" + "    90    87    88    1\n"
                    + "    91    87    89    1\n" + "    92    87    90    1\n" + "    93     6    91   ar\n"
                    + "    94    91    92   ar\n" + "    95     3    92   ar\n" + "    96    92    93    1\n"
                    + "    97    93    94    1\n" + "    98    93    95    2\n" + "    99    95    96    1\n"
                    + "   100    96    97    1\n" + "   101    97    98    1\n" + "   102    98    99    1\n"
                    + "   103    98   100    1\n" + "   104    98   101    1\n" + "   105   101   102    1\n"
                    + "   106   101   103    1\n" + "   107   103   104    1\n" + "   108   103   105    1\n"
                    + "   109   103   106    1\n" + "   110   106   107    1\n" + "   111   106   108    1\n"
                    + "   112   108   109    1\n" + "   113   108   110    1\n" + "   114    97   108    1\n"
                    + "   115   106   111    1\n" + "   116   111   112    1\n" + "   117   111   113    1\n"
                    + "   118   111   114    1\n" + "   119   114   115    1\n" + "   120   114   116    1\n"
                    + "   121   116   117    1\n" + "   122   116   118    1\n" + "   123    97   116    1\n"
                    + "   124   114   119    1\n" + "   125   119   120    1\n" + "   126   119   121    1\n"
                    + "   127   101   119    1\n" + "   128    96   122    1\n" + "   129   122   123    1\n"
                    + "   130   122   124    1\n" + "   131   122   125    1\n" + "   132    91   126    1\n"
                    + "   133   126   127    1\n" + "   134     4   128    1\n" + "   135   128   129    1\n";
            Mol2Reader r = new Mol2Reader(new StringReader(problematicMol2));
            IChemModel model = (IChemModel)r.Read(Silent.ChemObjectBuilder.Instance.NewChemModel());
            IAtomContainer mol = model.MoleculeSet[0];
            Layout(mol);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(mol));
        }

        IAtomContainer MakeTetraMethylCycloButane()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            mol.Atoms.Add(new Atom("C")); // 1
            mol.Atoms.Add(new Atom("C")); // 2
            mol.Atoms.Add(new Atom("C")); // 3
            mol.Atoms.Add(new Atom("C")); // 4
            mol.Atoms.Add(new Atom("C")); // 5
            mol.Atoms.Add(new Atom("C")); // 6
            mol.Atoms.Add(new Atom("C")); // 7
            mol.Atoms.Add(new Atom("C")); // 8

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[3], mol.Atoms[0], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single); // 6
            mol.AddBond(mol.Atoms[2], mol.Atoms[6], BondOrder.Single); // 7
            mol.AddBond(mol.Atoms[3], mol.Atoms[7], BondOrder.Single); // 8
            return mol;
        }

        IAtomContainer MakeJhao1()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            mol.Atoms.Add(new Atom("C")); // 1
            mol.Atoms.Add(new Atom("C")); // 2
            mol.Atoms.Add(new Atom("C")); // 3
            mol.Atoms.Add(new Atom("C")); // 4
            mol.Atoms.Add(new Atom("C")); // 5
            mol.Atoms.Add(new Atom("C")); // 6
            mol.Atoms.Add(new Atom("C")); // 7
            mol.Atoms.Add(new Atom("O")); // 8
            mol.Atoms.Add(new Atom("C")); // 9

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[0], mol.Atoms[7], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single); // 6
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Single); // 7
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 8
            mol.AddBond(mol.Atoms[2], mol.Atoms[5], BondOrder.Single); // 9
            mol.AddBond(mol.Atoms[2], mol.Atoms[6], BondOrder.Single); // 10
            mol.AddBond(mol.Atoms[2], mol.Atoms[7], BondOrder.Single); // 11
            mol.AddBond(mol.Atoms[3], mol.Atoms[8], BondOrder.Single); // 12
            return mol;
        }

        IAtomContainer MakeJhao2()
        {
            IAtomContainer mol = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            mol.Atoms.Add(new Atom("C")); // 1
            mol.Atoms.Add(new Atom("C")); // 2
            mol.Atoms.Add(new Atom("C")); // 3
            mol.Atoms.Add(new Atom("C")); // 4
            mol.Atoms.Add(new Atom("C")); // 5
            mol.Atoms.Add(new Atom("C")); // 6
            mol.Atoms.Add(new Atom("C")); // 7
            mol.Atoms.Add(new Atom("O")); // 8
            mol.Atoms.Add(new Atom("C")); // 9

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[0], mol.Atoms[7], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Single); // 6
            mol.AddBond(mol.Atoms[1], mol.Atoms[7], BondOrder.Single); // 7
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 8
            mol.AddBond(mol.Atoms[2], mol.Atoms[4], BondOrder.Single); // 9
            mol.AddBond(mol.Atoms[2], mol.Atoms[5], BondOrder.Single); // 10
            mol.AddBond(mol.Atoms[2], mol.Atoms[6], BondOrder.Single); // 11
            mol.AddBond(mol.Atoms[3], mol.Atoms[8], BondOrder.Single); // 12
            return mol;
        }

        // @cdk.bug 1750968
        public IAtomContainer MakeBug1750968()
        {
            string filename = "NCDK.Data.MDL.bug_1750968.mol";

            //        set up molecule reader
            Stream ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader molReader = new MDLReader(ins, ChemObjectReaderModes.Strict);

            //        read molecule
            return molReader.Read(Default.ChemObjectBuilder.Instance.NewAtomContainer());
        }

        /// <summary>
        /// Test for StructureDiagramGenerator bug #1772609 "NPE with bridged rings in SDG/RingPlacer".
        /// In method RingPlacer.PlaceBridgedRing(...) it could happen, that not all atoms of an unplaced
        /// ring were selected for placing. Thus, those atoms later lacked 2D coordinates (they were null)
        /// and the RingPlacer crashed with a NullPointerException such as:
        ///
        /// java.lang.NullPointerException
        ///   at javax.vecmath.Tuple2d.<init>(Tuple2d.java:66)
        ///   at javax.vecmath.Vector2.<init>(Vector2.java:74)
        ///   at org.openscience.cdk.layout.RingPlacer.PlaceFusedRing(RingPlacer.java:379)
        ///   at org.openscience.cdk.layout.RingPlacer.PlaceRing(RingPlacer.java:99)
        ///   at org.openscience.cdk.layout.RingPlacer.PlaceConnectedRings(RingPlacer.java:663)
        ///   at org.openscience.cdk.layout.StructureDiagramGenerator.LayoutRingSet(StructureDiagramGenerator.java:516)
        ///   at org.openscience.cdk.layout.StructureDiagramGenerator.GenerateCoordinates(StructureDiagramGenerator.java:379)
        ///   at org.openscience.cdk.layout.StructureDiagramGenerator.GenerateCoordinates(StructureDiagramGenerator.java:445)
        ///
        /// Author: Andreas Schueller <a.schueller@chemie.uni-frankfurt.de>
        /// </summary>
        // @cdk.bug 1772609
        [TestMethod()]
        [Timeout(5000)]
        public void TestNPEWithBridgedRingsBug1772609()
        {
            // set up molecule reader
            string filename = "NCDK.Data.MDL.bug1772609.mol";
            Stream ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader molReader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);

            // read molecule
            IAtomContainer molecule = (IAtomContainer)molReader.Read(Silent.ChemObjectBuilder.Instance.NewAtomContainer());

            // rebuild 2D coordinates
            // repeat this 10 times since the bug does only occur by chance
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    Layout(molecule);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.StackTrace);
                Assert.Fail("Test failed trying to layout bridged ring systems.");
            }
        }

        /// <summary>
        /// Test for bug #1784850 "SDG hangs in infinite loop".
        /// Fixed by correcting the safteyCounter check.
        ///
        /// Author: Andreas Schueller <a.schueller@chemie.uni-frankfurt.de>
        /// </summary>
        // @cdk.bug 1784850
        [TestMethod()]
        [Timeout(5000)]
        public void TestBug1784850InfiniteLoop()
        {
            // set up molecule reader
            string filename = "NCDK.Data.MDL.bug1784850.mol";
            Stream ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader molReader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);

            // read molecule
            IAtomContainer molecule = molReader.Read(Default.ChemObjectBuilder.Instance.NewAtomContainer());

            // rebuild 2D coordinates
            Layout(molecule);

            // test completed, no timeout occurred
        }

        /// <summary>
        /// For the SMILES compound below (the largest molecule in Chembl) a
        /// handful of atoms had invalid (NaN) Double coordinates.
        /// </summary>
        // @cdk.bug 2842445
        [TestMethod()]
        [Timeout(10000)]
        public void TestBug2843445NaNCoords()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            string smiles = "CCCC[C@H](NC(=O)[C@H](CCC(O)=O)NC(=O)[C@@H](NC(=O)[C@@H](CCCC)NC"
                    + "(=O)[C@H](CC(N)=O)NC(=O)[C@H](CCC\\N=C(\\N)N)NC(=O)[C@H](CC(C)C)NC"
                    + "(=O)[C@H](CC(C)C)NC(=O)[C@H](CC1=CNC=N1)NC(=O)[C@H](CC1=CC=CC=C1"
                    + ")NC(=O)[C@@H](NC(=O)[C@H](CC(C)C)NC(=O)[C@H](CC(O)=O)NC(=O)[C@@H"
                    + "](NC(=O)[C@H](CO)NC(=O)[C@@H](NC(=O)[C@@H]1CCCN1C(=O)[C@@H]1CCCN"
                    + "1C(=O)[C@H](CC(O)=O)NC(=O)[C@H](CC(O)=O)NC(=O)[C@@H](N)CC(N)=O)["
                    + "C@@H](C)CC)[C@@H](C)CC)[C@@H](C)O)[C@@H](C)CC)C(=O)N[C@@H](C)C(="
                    + "O)N[C@@H](CCC\\N=C(\\N)N)C(=O)N[C@@H]([C@@H](C)CC)C(=O)N[C@@H](CCC"
                    + "(O)=O)C(=O)N[C@@H](CC(N)=O)C(=O)N[C@@H](CCC(=O)OC)C(=O)N[C@@H](C"
                    + "CC\\N=C(\\N)N)C(=O)N[C@@H](CCC(O)=O)C(=O)N[C@@H](CCC(O)=O)C(=O)N[C"
                    + "@@H](C)C(=O)NCC(=O)N[C@@H](CCCCN)C(=O)N[C@@H](CC(N)=O)C(=O)N[C@@"
                    + "H](CCC\\N=C(\\N)N)C(=O)N[C@@H](CCCCN)C(=O)N[C@@H](CC1=CC=C(O)C=C1)"
                    + "C(=O)N[C@@H](CC(C)C)C(=O)N[C@@H](CC(O)=O)C(=O)N[C@@H](CCC(O)=O)C" + "(=O)N[C@@H](C(C)C)C(N)=O";
            IAtomContainer mol = sp.ParseSmiles(smiles);

            Layout(mol);

            int invalidCoordCount = 0;
            foreach (var atom in mol.Atoms)
            {
                if (double.IsNaN(atom.Point2D.Value.X) || double.IsNaN(atom.Point2D.Value.Y))
                {
                    invalidCoordCount++;
                }
            }
            Assert.AreEqual(0, invalidCoordCount, "No 2d coordinates should be NaN");
        }

        /// <summary>
        /// The following SMILES compound gets null cordinates.
        /// </summary>
        /// <exception cref="Exception">if the test failed</exception>
        // @cdk.bug 1234
        [TestMethod()]
        [Timeout(5000)]
        [ExpectedException(typeof(CDKException), AllowDerivedTypes = true)]
        public void TestBug1234()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            string smiles = "C1C1";

            IAtomContainer mol = sp.ParseSmiles(smiles);
            Layout(mol);

            int invalidCoordCount = 0;
            foreach (var atom in mol.Atoms)
            {
                if (atom.Point2D == null)
                {
                    invalidCoordCount++;
                }
            }
            Assert.AreEqual(0, invalidCoordCount, "No 2d coordinates should be null");
        }

        /// <summary>
        /// Tests case where calling generateExperimentalCoordinates threw an NPE.
        /// </summary>
        // @cdk.bug 1269
        [TestMethod()]
        [Timeout(5000)]
        public void TestBug1269()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            string smiles = "O=C(O)[C@H](N)C"; // L-alanine, but any [C@H] will do
            IAtomContainer mol = sp.ParseSmiles(smiles);

            SDG.Molecule = mol;
            SDG.GenerateExperimentalCoordinates(new Vector2(0, 1));
        }

        /// <summary>
        /// Does the SDG handle non-connected molecules?
        /// </summary>
        // @cdk.bug 1279
        [TestMethod()]
        [Timeout(5000)]
        public void TestBug1279()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            string smiles = "[NH4+].CP(=O)(O)CCC(N)C(=O)[O-]";

            IAtomContainer mol = sp.ParseSmiles(smiles);

            Layout(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsNotNull(atom.Point2D);
        }

        [TestMethod()]
        public void AlleneWithImplHDoesNotCauseNPE()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            string smiles = "CC=[C@]=CC";

            IAtomContainer mol = sp.ParseSmiles(smiles);

            Layout(mol);
        }

        [TestMethod()]
        public void PyrroleWithIdentityTemplate()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            string smiles = "C1=CNC=C1";

            StructureDiagramGenerator generator = new StructureDiagramGenerator();
            generator.UseIdentityTemplates = true;

            IAtomContainer mol = sp.ParseSmiles(smiles);

            generator.SetMolecule(mol, false);
            generator.GenerateCoordinates();

            IAtom nitrogen = mol.Atoms[2];

            // nitrogen is lowest point
            Assert.IsTrue(nitrogen.Point2D.Value.Y < mol.Atoms[0].Point2D.Value.Y);
            Assert.IsTrue(nitrogen.Point2D.Value.Y < mol.Atoms[1].Point2D.Value.Y);
            Assert.IsTrue(nitrogen.Point2D.Value.Y < mol.Atoms[3].Point2D.Value.Y);
            Assert.IsTrue(nitrogen.Point2D.Value.Y < mol.Atoms[4].Point2D.Value.Y);
        }

        [TestMethod()]
        public void PyrroleWithIdentityTemplate40()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            string smiles = "C1=CNC=C1";

            StructureDiagramGenerator generator = new StructureDiagramGenerator();
            generator.UseIdentityTemplates = true;

            IAtomContainer mol = sp.ParseSmiles(smiles);

            generator.SetMolecule(mol, false);
            generator.GenerateCoordinates();

            IAtom nitrogen = mol.Atoms[2];

            // nitrogen is lowest point
            Assert.IsTrue(nitrogen.Point2D.Value.Y < mol.Atoms[0].Point2D.Value.Y);
            Assert.IsTrue(nitrogen.Point2D.Value.Y < mol.Atoms[1].Point2D.Value.Y);
            Assert.IsTrue(nitrogen.Point2D.Value.Y < mol.Atoms[3].Point2D.Value.Y);
            Assert.IsTrue(nitrogen.Point2D.Value.Y < mol.Atoms[4].Point2D.Value.Y);
        }

        [TestMethod()]
        public void PyrroleWithoutIdentityTemplate()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            string smiles = "C1=CNC=C1";

            StructureDiagramGenerator generator = new StructureDiagramGenerator();
            generator.UseIdentityTemplates = false;

            IAtomContainer mol = sp.ParseSmiles(smiles);

            generator.SetMolecule(mol, false);
            generator.GenerateCoordinates();

            double minY = double.MaxValue;
            int i = -1;

            // note if the SDG changes the nitrogen might be at
            // the bottom by chance when generated ab initio
            for (int j = 0; j < mol.Atoms.Count; j++)
            {
                IAtom atom = mol.Atoms[j];
                if (atom.Point2D.Value.Y < minY)
                {
                    minY = atom.Point2D.Value.Y;
                    i = j;
                }
            }

            // N is at index 2
            Assert.AreNotEqual(2, i);
        }

        [TestMethod()]
        public void HandleFragments()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCOCC.o1cccc1");
            Layout(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsNotNull(atom.Point2D);
        }

        [TestMethod()]
        public void IonicBondsInAlCl3()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[Al+3].[Cl-].[Cl-].[Cl-]");
            Layout(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsNotNull(atom.Point2D);
            Assert.AreEqual(
                1.5 * SDG.BondLength,
                Vector2.Distance(mol.Atoms[0].Point2D.Value, mol.Atoms[1].Point2D.Value),
                0.001);
            Assert.AreEqual(
                1.5 * SDG.BondLength,
                Vector2.Distance(mol.Atoms[0].Point2D.Value, mol.Atoms[2].Point2D.Value),
                0.001);
            Assert.AreEqual(
                1.5 * SDG.BondLength,
                Vector2.Distance(mol.Atoms[0].Point2D.Value, mol.Atoms[3].Point2D.Value),
                0.001);
        }

        [TestMethod()]
        public void IonicBondsInK2CO3()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[K+].[O-]C(=O)[O-].[K+]");
            Layout(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsNotNull(atom.Point2D);
            Assert.AreEqual(
                1.5 * SDG.BondLength,
                Vector2.Distance(mol.Atoms[0].Point2D.Value, mol.Atoms[1].Point2D.Value),
                0.001);
            Assert.AreEqual(
                1.5 * SDG.BondLength,
                Vector2.Distance(mol.Atoms[4].Point2D.Value, mol.Atoms[5].Point2D.Value),
                0.001);
        }

        // subjective... since the real structure is lattice but looks better than a grid
        [TestMethod()]
        public void IonicBondsInLiAlH4()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[Li+].[Al+3].[Cl-].[Cl-].[Cl-].[Cl-]");
            Layout(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsNotNull(atom.Point2D);
            for (int i = 2; i < 5; i++)
            {
                var distLi = Vector2.Distance(mol.Atoms[0].Point2D.Value, mol.Atoms[i].Point2D.Value);
                var distAl = Vector2.Distance(mol.Atoms[1].Point2D.Value, mol.Atoms[i].Point2D.Value);
                var diffLi = distLi - 1.5 * SDG.BondLength;
                var diffAl = distAl - 1.5 * SDG.BondLength;
                if (Math.Abs(diffLi) > 0.001 && Math.Abs(diffAl) > 0.001)
                    Assert.Fail("Chlorine must be bond length from Al or Li atoms");
            }
        }

        [TestMethod()]
        public void IonicBondsInSodiumBenzoate()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[Na+].[O-]C(=O)c1ccccc1");
            Layout(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsNotNull(atom.Point2D);
            Assert.AreEqual(
                1.5 * SDG.BondLength,
                Vector2.Distance(mol.Atoms[0].Point2D.Value, mol.Atoms[1].Point2D.Value),
                0.001);
        }

        // SMILES have been shuffled the smiles to make it harder... otherwise we
        // get it right by chance
        [TestMethod()]
        public void Chembl12276()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[Cl-].C(C1=CC=CC2=C(C=CC=C12)[N+](=O)[O-])[N+](C)(CCCl)CCCl");
            Layout(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsNotNull(atom.Point2D);
            Assert.AreEqual(17, mol.Atoms[0].AtomicNumber);
            Assert.AreEqual(7, mol.Atoms[15].AtomicNumber);
            Assert.AreEqual(
                1.5 * SDG.BondLength,
                Vector2.Distance(mol.Atoms[0].Point2D.Value, mol.Atoms[15].Point2D.Value),
                0.001);
        }

        [TestMethod()]
        public void CalciumOxide()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[Ca+2].[O-2]");
            Layout(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsNotNull(atom.Point2D);
            Assert.AreEqual(
                1.5 * SDG.BondLength,
                Vector2.Distance(mol.Atoms[0].Point2D.Value, mol.Atoms[1].Point2D.Value),
                0.001);
        }

        [TestMethod()]
        public void EthaneHCL()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("Cl.CC");
            Layout(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsNotNull(atom.Point2D);
        }

        // An extreme test case suggest by Roger Sayle showing Humpty Dumpty reassembly
        [TestMethod()]
        public void MultipleSalts()
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("[K+].[Al+3].[Cl-].[Cl-].[K+].[Cl-].[Cl-].[Al+3].[Cl-].[Pt+2]([NH3])[NH3].[Cl-].[Cl-].[Cl-].[O-][C+]([O-])[O-]");
            Layout(mol);
            foreach (var atom in mol.Atoms)
                Assert.IsNotNull(atom.Point2D);

            IAtom platinum = null;
            var aluminiums = new HashSet<IAtom>();
            var potassiums = new HashSet<IAtom>();
            var chlorines = new HashSet<IAtom>();
            var oxygens = new HashSet<IAtom>();
            foreach (var atom in mol.Atoms)
            {
                if (atom.Symbol.Equals("Cl"))
                    chlorines.Add(atom);
                else if (atom.Symbol.Equals("O"))
                    oxygens.Add(atom);
                else if (atom.Symbol.Equals("Al"))
                    aluminiums.Add(atom);
                else if (atom.Symbol.Equals("K"))
                    potassiums.Add(atom);
                else if (atom.Symbol.Equals("Pt"))
                    platinum = atom;
            }

            Assert.IsNotNull(platinum);
            Assert.AreEqual(2, potassiums.Count);
            Assert.AreEqual(3, oxygens.Count);
            Assert.AreEqual(8, chlorines.Count);

            // platin has two chlorines...
            int ptFound = 0;
            foreach (var chlorine in chlorines)
            {
                var delta = Vector2.Distance(chlorine.Point2D.Value, platinum.Point2D.Value) - 1.5 * SDG.BondLength;
                if (Math.Abs(delta) < 0.01)
                    ptFound++;
            }
            Assert.AreEqual(2, ptFound);

            // K+ each have an oxygen
            foreach (var potassium in potassiums)
            {
                int kFound = 0;
                foreach (var oxygen in oxygens)
                {
                    var delta = Vector2.Distance(oxygen.Point2D.Value, potassium.Point2D.Value) - 1.5 * SDG.BondLength;
                    if (Math.Abs(delta) < 0.01)
                        kFound++;
                }
                Assert.AreEqual(1, kFound);
            }

            // Al+3 each have 3 chlorines
            foreach (var aluminium in aluminiums)
            {
                int clFound = 0;
                foreach (var chlorine in chlorines)
                {
                    var delta = Vector2.Distance(chlorine.Point2D.Value, aluminium.Point2D.Value) - 1.5 * SDG.BondLength;
                    if (Math.Abs(delta) < 0.01)
                        clFound++;
                }
                Assert.AreEqual(3, clFound);
            }
        }

        [TestMethod()]
        public void PlaceCrossingSgroupBrackets()
        {
            IAtomContainer mol = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("C"));
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("C"));
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("C"));
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("O"));
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms[1].ImplicitHydrogenCount = 2;
            mol.Atoms[2].ImplicitHydrogenCount = 2;
            mol.Atoms[3].ImplicitHydrogenCount = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            Sgroup sgroup = new Sgroup();
            sgroup.Type = SgroupType.CtabStructureRepeatUnit;
            sgroup.Subscript = "n";
            sgroup.PutValue(SgroupKeys.CtabConnectivity, "HT");
            sgroup.Atoms.Add(mol.Atoms[1]);
            sgroup.Atoms.Add(mol.Atoms[2]);
            sgroup.Bonds.Add(mol.Bonds[0]);
            sgroup.Bonds.Add(mol.Bonds[2]);
            mol.SetProperty(CDKPropertyName.CtabSgroups, new[] { sgroup });

            Layout(mol);
            var brackets = (IList<SgroupBracket>)sgroup.GetValue(SgroupKeys.CtabBracket);
            Assert.IsNotNull(brackets);
            Assert.AreEqual(2, brackets.Count);
        }

        [TestMethod()]
        public void PlaceNonCrossingSgroupBrackets()
        {
            IAtomContainer mol = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("C"));
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("C"));
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("C"));
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("O"));
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms[1].ImplicitHydrogenCount = 2;
            mol.Atoms[2].ImplicitHydrogenCount = 2;
            mol.Atoms[3].ImplicitHydrogenCount = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            Sgroup sgroup = new Sgroup();
            sgroup.Type = SgroupType.CtabStructureRepeatUnit;
            sgroup.Subscript = "n";
            sgroup.PutValue(SgroupKeys.CtabConnectivity, "HT");
            foreach (var atom in mol.Atoms)
                sgroup.Atoms.Add(atom);
            mol.SetProperty(CDKPropertyName.CtabSgroups, new[] { sgroup });

            Layout(mol);
            var brackets = (IList<SgroupBracket>)sgroup.GetValue(SgroupKeys.CtabBracket);
            Assert.IsNotNull(brackets);
            Assert.AreEqual(2, brackets.Count);
        }

        [TestMethod()]
        public void PlaceOverlappingCrossingSgroupBrackets()
        {
            IAtomContainer mol = Silent.ChemObjectBuilder.Instance.NewAtomContainer();
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("C"));
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("C"));
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("C"));
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("C"));
            mol.Atoms.Add(Silent.ChemObjectBuilder.Instance.NewAtom("O"));
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms[1].ImplicitHydrogenCount = 2;
            mol.Atoms[2].ImplicitHydrogenCount = 2;
            mol.Atoms[3].ImplicitHydrogenCount = 2;
            mol.Atoms[3].ImplicitHydrogenCount = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);

            Sgroup sgroup1 = new Sgroup();
            sgroup1.Type = SgroupType.CtabStructureRepeatUnit;
            sgroup1.Subscript = "n";
            sgroup1.PutValue(SgroupKeys.CtabConnectivity, "HT");
            sgroup1.Atoms.Add(mol.Atoms[1]);
            sgroup1.Atoms.Add(mol.Atoms[2]);
            sgroup1.Bonds.Add(mol.Bonds[1]);
            sgroup1.Bonds.Add(mol.Bonds[2]);

            Sgroup sgroup2 = new Sgroup();
            sgroup2.Type = SgroupType.CtabStructureRepeatUnit;
            sgroup2.Subscript = "m";
            sgroup2.PutValue(SgroupKeys.CtabConnectivity, "HT");
            sgroup2.Atoms.Add(mol.Atoms[1]);
            sgroup2.Atoms.Add(mol.Atoms[2]);
            sgroup2.Atoms.Add(mol.Atoms[3]);
            sgroup2.Bonds.Add(mol.Bonds[1]);
            sgroup2.Bonds.Add(mol.Bonds[3]);
            mol.SetProperty(CDKPropertyName.CtabSgroups, new[] { sgroup1, sgroup2 });

            Layout(mol);
            var brackets1 = (IList<SgroupBracket>)sgroup1.GetValue(SgroupKeys.CtabBracket);
            Assert.IsNotNull(brackets1);
            Assert.AreEqual(2, brackets1.Count);
            var brackets2 = (IList<SgroupBracket>)sgroup2.GetValue(SgroupKeys.CtabBracket);
            Assert.IsNotNull(brackets2);
            Assert.AreEqual(2, brackets2.Count);
        }

        bool IsCrossing(IBond a, IBond b)
        {
            Vector2 p1 = a.Begin.Point2D.Value;
            Vector2 p2 = a.End.Point2D.Value;
            Vector2 p3 = b.Begin.Point2D.Value;
            Vector2 p4 = b.End.Point2D.Value;
            return Vectors.LinesIntersect(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y);
        }

        [TestMethod()]
        public void PositionalVariation()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1CCCC.*[R1].*C(=O)O");

            Sgroup sgroup1 = new Sgroup();
            sgroup1.Type = SgroupType.ExtMulticenter;
            Trace.Assert(mol.Bonds[10].Contains(mol.Atoms[10]));
            sgroup1.Atoms.Add(mol.Atoms[10]);
            sgroup1.Bonds.Add(mol.Bonds[10]);
            sgroup1.Atoms.Add(mol.Atoms[0]);
            sgroup1.Atoms.Add(mol.Atoms[1]);
            sgroup1.Atoms.Add(mol.Atoms[2]);
            sgroup1.Atoms.Add(mol.Atoms[3]);
            sgroup1.Atoms.Add(mol.Atoms[4]);
            sgroup1.Atoms.Add(mol.Atoms[5]);

            Sgroup sgroup2 = new Sgroup();
            sgroup2.Type = SgroupType.ExtMulticenter;
            Trace.Assert(mol.Bonds[11].Contains(mol.Atoms[12]));
            sgroup2.Atoms.Add(mol.Atoms[12]);
            sgroup2.Bonds.Add(mol.Bonds[11]);
            sgroup2.Atoms.Add(mol.Atoms[0]);
            sgroup2.Atoms.Add(mol.Atoms[1]);
            sgroup2.Atoms.Add(mol.Atoms[2]);
            sgroup2.Atoms.Add(mol.Atoms[3]);
            sgroup2.Atoms.Add(mol.Atoms[4]);
            sgroup2.Atoms.Add(mol.Atoms[5]);

            mol.SetProperty(CDKPropertyName.CtabSgroups, new[] { sgroup1, sgroup2 });
            Layout(mol);

            int numCrossing = 0;
            for (int i = 0; i < 6; i++)
            {
                if (IsCrossing(mol.Bonds[i], mol.Bonds[10]))
                    numCrossing++;
                if (IsCrossing(mol.Bonds[i], mol.Bonds[11]))
                    numCrossing++;
            }
        }

        [TestMethod()]
        public void DisconnectedMultigroupPlacement()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1.c1ccccc1.c1ccccc1");

            // build multiple group Sgroup
            Sgroup sgroup = new Sgroup();
            sgroup.Type = SgroupType.CtabMultipleGroup;
            foreach (var atom in mol.Atoms)
                sgroup.Atoms.Add(atom);
            var patoms = new List<IAtom>(6);
            foreach (var atom in mol.Atoms)
            {
                patoms.Add(atom);
                if (patoms.Count == 6)
                    break;
            }
            sgroup.PutValue(SgroupKeys.CtabParentAtomList, patoms);
            mol.SetProperty(CDKPropertyName.CtabSgroups, new[] { sgroup });
            Layout(mol);
            for (int i = 0; i < 6; i++)
            {
                Assert.IsTrue(Vector2.Distance(
                    mol.Atoms[i].Point2D.Value,
                    mol.Atoms[i + 6].Point2D.Value)
                    < 0.01);
                Assert.IsTrue(Vector2.Distance(
                    mol.Atoms[i].Point2D.Value,
                    mol.Atoms[i + 12].Point2D.Value)
                    < 0.01);
            }
        }

        /// <summary>
        /// These molecules are laid out 'H2N=NH2.H2N=NH2', ensure we give them more space than
        /// usual (bond length)
        /// </summary>
        [TestMethod()]
        public void Dihydroazine()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = smipar.ParseSmiles("N=N.N=N");
            Layout(mol);
            Assert.IsTrue(mol.Atoms[2].Point2D.Value.X - mol.Atoms[1].Point2D.Value.X > SDG.BondLength);
        }

        [TestMethod()]
        public void NH4OH()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = smipar.ParseSmiles("[NH4+].[OH-]");
            Layout(mol);
            Assert.IsTrue(SDG.BondLength < mol.Atoms[1].Point2D.Value.X - mol.Atoms[0].Point2D.Value.X);
        }

        [TestMethod()]
        public void FragmentDoubleBondConfiguration()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            IAtomContainer mol = smipar.ParseSmiles("C(\\C)=C/C.C(\\C)=C\\C.C(\\C)=C/C.C(\\C)=C\\C");
            Layout(mol);
            var elements = StereoElementFactory.Using2DCoordinates(mol).CreateAll();
            int numCis = 0;
            int numTrans = 0;
            foreach (IStereoElement se in elements)
            {
                if (se is IDoubleBondStereochemistry)
                {
                    DoubleBondConformation config = ((IDoubleBondStereochemistry)se).Stereo;
                    if (config == DoubleBondConformation.Together)
                        numCis++;
                    else if (config == DoubleBondConformation.Opposite)
                        numTrans++;
                }
            }
            Assert.AreEqual(2, numCis);
            Assert.AreEqual(2, numTrans);
        }
    }
}
