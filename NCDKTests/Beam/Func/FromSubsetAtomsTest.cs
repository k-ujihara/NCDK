using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Beam
{
    // @author John May 
    [TestClass()]
    public class FromSubsetAtomsTest
    {
        [TestMethod()]
        public void UnknownTest()
        {
            Transform("*", "[*]");
        }

        [TestMethod()]
        public void Inorganic()
        {
            Transform("[Ne]", "[Ne]");
        }

        [TestMethod()]
        public void Methane()
        {
            Transform("C", "[CH4]");
        }

        [TestMethod()]
        public void Ethane_withAtomClass()
        {
            Transform("[CH3:1]C", "[CH3:1][CH3]");
        }

        [TestMethod()]
        public void Ethanol()
        {
            Transform("CCO", "[CH3][CH2][OH]");
        }

        [TestMethod()]
        public void StereoSpecification()
        {
            Transform("[C@H](N)(O)C", "[C@H]([NH2])([OH])[CH3]");
            Transform("[C@@H](N)(O)C", "[C@@H]([NH2])([OH])[CH3]");
        }

        [TestMethod()]
        public void NoStereoSpecification()
        {
            Transform("C(N)(O)C", "[CH]([NH2])([OH])[CH3]");
        }

        [TestMethod()]
        public void BracketAtom()
        {
            // should provide identity of bracket atom
            IAtom input = new AtomImpl.BracketAtom(Element.Carbon, 1, 0);
            IAtom output = FromSubsetAtoms.FromSubset(input, 0, 0);

            Assert.AreSame(output, input);
        }

        [TestMethod()]
        public void Aliphatic_carbon()
        {
            IAtom actual = FromSubsetAtoms
                    .FromSubset(AtomImpl.AliphaticSubset.Carbon, 3, 0);
            IAtom expect = new AtomImpl.BracketAtom(Element.Carbon, 1, 0);
            Assert.AreEqual(actual, expect);
        }

        [TestMethod()]
        public void Aromatic_carbon()
        {
            IAtom actual = FromSubsetAtoms
                    .FromSubset(AtomImpl.AromaticSubset.Carbon, 2, 0);
            IAtom expect = new AtomImpl.BracketAtom(-1, Element.Carbon, 1, 0, 0, true);
            Assert.AreEqual(expect, actual);
        }

        [TestMethod()]
        public void Indolizine()
        {
            Transform("c2cc1cccn1cc2",
                      "[cH]1[cH][c]2[cH][cH][cH][n]2[cH][cH]1");
        }

        [TestMethod()]
        public void Indolizine_kekule()
        {
            Transform("C1=CN2C=CC=CC2=C1",
                      "[CH]1=[CH][N]2[CH]=[CH][CH]=[CH][C]2=[CH]1");
        }

        [TestMethod()]
        public void Test_1H_imidazole()
        {
            Transform("[H]n1ccnc1",
                      "[H][n]1[cH][cH][n][cH]1");
        }

        [TestMethod()]
        public void Test_1H_imidazole_kekule()
        {
            Transform("[H]N1C=CN=C1",
                      "[H][N]1[CH]=[CH][N]=[CH]1");
        }

        [TestMethod()]
        public void CDK_bug_1363882()
        {
            Transform("[H]c2c([H])c(c1c(nc(n1([H]))C(F)(F)F)c2Cl)Cl",
                      "[H][c]1[c]([H])[c]([c]2[c]([n][c]([n]2[H])[C]([F])([F])[F])[c]1[Cl])[Cl]");
        }

        [TestMethod()]
        public void CDK_bug_1579235()
        {
            Transform("c2cc1cccn1cc2",
                      "[cH]1[cH][c]2[cH][cH][cH][n]2[cH][cH]1");
        }

        [TestMethod()]
        public void Sulphur()
        {
            Transform("S([H])[H]",
                      "[S]([H])[H]");
            Transform("[H]S([H])[H]",
                      "[H][SH]([H])[H]");
            Transform("[H]S([H])([H])[H]",
                      "[H][S]([H])([H])[H]");
            Transform("[H]S([H])([H])([H])[H]",
                      "[H][SH]([H])([H])([H])[H]");
            Transform("[H]S([H])([H])([H])([H])[H]",
                      "[H][S]([H])([H])([H])([H])[H]");
        }

        [TestMethod()]
        public void Tricyclazole()
        {
            Transform("Cc1cccc2sc3nncn3c12",
                      "[CH3][c]1[cH][cH][cH][c]2[s][c]3[n][n][cH][n]3[c]12");
        }

        [TestMethod()]
        public void Tricyclazole_kekule()
        {
            Transform("CC1=C2N3C=NN=C3SC2=CC=C1",
                      "[CH3][C]1=[C]2[N]3[CH]=[N][N]=[C]3[S][C]2=[CH][CH]=[CH]1");
        }

        /// ("bad molecule - should have utility to find/fix this types of errors")
        public void MixingAromaticAndKekule()
        {
            Transform("c1=cc=cc=c1",
                      "[cH]1=[cH][cH]=[cH][cH]=[cH]1");
        }

        [TestMethod()]
        public void Quinone()
        {
            Transform("oc1ccc(o)cc1",
                      "[o][c]1[cH][cH][c]([o])[cH][cH]1");
        }

        /// <summary>1-(1H-pyrrol-2-yl)pyrrole</summary>
        [TestMethod()]
        public void Pyroles()
        {
            Transform("c1ccn(c1)-c1ccc[nH]1",
                  "[cH]1[cH][cH][n]([cH]1)-[c]2[cH][cH][cH][nH]2");
        }

        [TestMethod()]
        public void CDK_bug_956926()
        {
            Transform("[c+]1ccccc1",
                      "[c+]1[cH][cH][cH][cH][cH]1");
        }

        private void Transform(string input, string expected)
        {
            Graph g = Parser.Parse(input);
            ImplicitToExplicit ite = new ImplicitToExplicit();
            FromSubsetAtoms fsa = new FromSubsetAtoms();
            ExplicitToImplicit eti = new ExplicitToImplicit();
            string actual = Generator.Generate(eti.Apply(
                fsa.Apply(
                        ite.Apply(g))));
            Assert.AreEqual(expected, actual);
        }
    }
}
