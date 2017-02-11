using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Beam
{
    /// <author>John May </author>
    [TestClass()]
    public class ToSubsetAtomsTest
    {
        [TestMethod()]
        public void UnknownTest()
        {
            Transform("[*]", "*");
        }

        [TestMethod()]
        public void inorganic()
        {
            Transform("[Ne]", "[Ne]");
        }

        [TestMethod()]
        public void Methane()
        {
            Transform("[CH4]", "C");
        }

        [TestMethod()]
        public void Monovalent_carbon()
        {
            Transform("[CH3]", "[CH3]");
        }

        [TestMethod()]
        public void Divalent_carbon()
        {
            Transform("[CH2]", "[CH2]");
        }

        [TestMethod()]
        public void Trivalent_carbon()
        {
            Transform("[CH]", "[CH]");
            Transform("[CH1]", "[CH]");
        }

        [TestMethod()]
        public void Carbon_12()
        {
            // note the isotope is specified and so must be a bracket atom
            Transform("[12C]", "[12C]");
        }

        [TestMethod()]
        public void Carbon_13()
        {
            Transform("[13C]", "[13C]");
        }

        [TestMethod()]
        public void Carbon_14()
        {
            Transform("[14C]", "[14C]");
        }

        [TestMethod()]
        public void oxidanide()
        {
            Transform("[OH-]", "[OH-]");
        }

        [TestMethod()]
        public void azanium()
        {
            Transform("[NH4+]", "[NH4+]");
        }

        [TestMethod()]
        public void Ethane_withAtomClass()
        {
            Transform("[CH3:1][CH3:0]", "[CH3:1]C");
        }

        [TestMethod()]
        public void Ethanol()
        {
            Transform("[CH3][CH2][OH]", "CCO");
        }

        [TestMethod()]
        public void StereoSpecification()
        {
            Transform("[C@H]([NH2])([OH])[CH3]", "[C@H](N)(O)C");
            Transform("[C@@H]([NH2])([OH])[CH3]", "[C@@H](N)(O)C");
        }

        [TestMethod()]
        public void noStereoSpecification()
        {
            Transform("[CH]([NH2])([OH])[CH3]", "C(N)(O)C");
        }

        [TestMethod()]
        public void Tricyclazole()
        {
            Transform("[CH3][c]1[cH][cH][cH][c]2[s][c]3[n][n][cH][n]3[c]12",
                      "Cc1cccc2sc3nncn3c12");
        }

        [TestMethod()]
        public void Pyrole_kekule()
        {
            Transform("[NH]1[CH]=[CH]N=[CH]1",
                      "N1C=CN=C1");
        }

        [TestMethod()]
        public void Pyrole()
        {
            Transform("[nH]1[cH][cH][n][cH]1",
                      "[nH]1ccnc1");
        }

        [TestMethod()]
        public void zinc_1()
        {
            Transform("c1cc(ccc1/C=c\\2/c(=O)o/c(=C\\Cl)/[nH]2)F",
                  "c1cc(ccc1/C=c\\2/c(=O)o/c(=C\\Cl)/[nH]2)F");
        }


        private void Transform(string input, string expected)
        {
            Graph g = Parser.Parse(input);
            ImplicitToExplicit ite = new ImplicitToExplicit();
            ToSubsetAtoms tsa = new ToSubsetAtoms();
            ExplicitToImplicit eti = new ExplicitToImplicit();
            string actual = Generator.Generate(eti.Apply(
                    tsa.Apply(
                            ite.Apply(g))));
            Assert.AreEqual(expected, actual);
        }
    }
}
