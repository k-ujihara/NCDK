using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Beam
{
   /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class LocaliseTest
    {
        [TestMethod()]
        public void Furan()
        {
            Test("o1cccc1", "O1C=CC=C1");
        }

        [TestMethod()]
        public void Benzen()
        {
            Test("c1ccccc1", "C1=CC=CC=C1");
        }

        [TestMethod()]
        public void Quinone()
        {
            Test("oc1ccc(o)cc1", "O=C1C=CC(=O)C=C1");
            Test("O=c1ccc(=O)cc1", "O=C1C=CC(=O)C=C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Methane()
        {
            Test("c", "C"); // note daylight makes it 'CH3' but we say - valence error
        }

        [TestMethod()]
        public void Ethene()
        {
            Test("cc", "C=C");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Invalid_acyclic_chain()
        {
            Test("ccc", "n/a");
        }

        [TestMethod()]
        public void Buta_1_3_diene()
        {
            Test("cccc", "C=CC=C");
        }

        // some allow lower-case to be radical, this should throw an exception
        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Carbon_radical()
        {
            Test("C1CCcCC1", "n/a");
        }

        [TestMethod()]
        public void Hexa_1_3_5_triene()
        {
            Test("cccccc", "C=CC=CC=C");
        }

        [TestMethod()]
        public void Test_4H_pyran_4_one()
        {
            Test("oc1ccocc1", "O=C1C=COC=C1");
        }

        [TestMethod()]
        public void Pyrole()
        {
            Test("[nH]1cccc1", "[NH]1C=CC=C1");
        }

        [TestMethod()]
        public void CHEMBL385384()
        {
            Test("CCc1c(C#N)c(c2ccc(cc2)c3ccc(OC)cc3)c(C(=O)O)n1C",
             "CCC1=C(C#N)C(C2=CC=C(C=C2)C3=CC=C(OC)C=C3)=C(C(=O)O)N1C");
        }

        [TestMethod()]
        public void Imidazole()
        {
            Test("c1c[nH]cn1", "C1=C[NH]C=N1");
        }

        [TestMethod()]
        public void Benzimidazole()
        {
            Test("c1nc2ccccc2[nH]1", "C1=NC2=CC=CC=C2[NH]1");
        }

        [TestMethod()]
        public void Napthalene()
        {
            Test("c1ccc2ccccc2c1", "C1=CC=C2C=CC=CC2=C1");
        }

        [TestMethod()]
        public void Anthracene()
        {
            Test("c1ccc2cc3ccccc3cc2c1", "C1=CC=C2C=C3C=CC=CC3=CC2=C1");
        }

        [TestMethod()]
        public void Thiophene()
        {
            Test("s1cccc1", "S1C=CC=C1");
        }

        [TestMethod()]
        public void Imidazol_3_ium()
        {
            Test("c1c[nH+]c[nH]1", "C1=C[NH+]=C[NH]1");
        }

        [TestMethod()]
        public void Exocyclic_NO_bond()
        {
            Test("Nc1c2nc[nH]c2ncn1=O", "NC1=C2N=C[NH]C2=NC=N1=O");
        }

        [TestMethod()]
        public void Biphenyl()
        {
            Test("c1ccccc1c1ccccc1", "C1=CC=CC=C1C2=CC=CC=C2");
            Test("c1ccccc1-c1ccccc1", "C1=CC=CC=C1-C2=CC=CC=C2");
        }

        [TestMethod()]
        public void Phospho_nitro_ring()
        {
            Test("n1pnpnp1", "N1=PN=PN=P1");
        }

        [TestMethod()]
        public void Phospho_nitro_ring_exocyclic_oxygen()
        {
            Test("n1p(O)(O)np(O)(O)np1(O)(O)", "N1=P(O)(O)N=P(O)(O)N=P1(O)O");
        }

        [TestMethod()]
        public void Hexamethylidenecyclohexane()
        {
            Test("cc1c(c)c(c)c(c)c(c)c1c", "C=C1C(=C)C(=C)C(=C)C(=C)C1=C");
            Test("C=c1c(=C)c(=C)c(=C)c(=C)c1=C", "C=C1C(=C)C(=C)C(=C)C(=C)C1=C");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void EMolecules492140() 
        {
            Test("c1ccc2c(c1)c1[n-]c2/N=c/2\\[n-]c(c3c2cccc3)/N=c/2\\[n-]/c(=N\\c3[n-]/c(=N\\1)/c1ccccc31)/c1c2cccc1.[Cu+4] 492140", "n/a");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Tryptophanyl_radical()
        {
            Test("NC(Cc1c[n]c2ccccc12)C(O)=O",
                 "n/a");
        }

        [TestMethod()]
        public void TrivalentBoronNoPiBonds() 
        {
            Test("b1(C)ob(C)ob1(C)", "B1(C)OB(C)OB1C");
        }

        [TestMethod()]
        public void Thiophene_oxide()
        {
            Test("O=s1cccc1",
                 "O=S1C=CC=C1");
        }

        [TestMethod()]
        public void Tellurophene()
        {
            Test("[Te]1cccc1", "[Te]1C=CC=C1");
            Test("[te]1cccc1", "[Te]1C=CC=C1");
        }

        [TestMethod()]
        public void Porphyrin1()
        {
            Test("c1cc2cc3ccc(cc4ccc(cc5ccc(cc1n2)[nH]5)n4)[nH]3",
                 "C1=CC2=CC3=CC=C(C=C4C=CC(C=C5C=CC(=CC1=N2)[NH]5)=N4)[NH]3");
        }

        [TestMethod()]
        public void CHEMBL438024() 
        {
            Test("COC(=O)C1=C(C)NC(=C(C1c2c(nc3sccn23)c4cc(OC)ccc4OC)C(=O)OC)C",
                 "COC(=O)C1=C(C)NC(=C(C1C2=C(N=C3SC=CN23)C4=CC(OC)=CC=C4OC)C(=O)OC)C");
        }

        [TestMethod()]
        public void Porphyrin2()
        {
            Test("c1cc2cc3ccc(cc4ccc(cc5ccc(cc1n2)n5)n4)n3",
                 "C1=CC=2C=C3C=CC(C=C4C=CC(C=C5C=CC(C=C1N2)=N5)=N4)=N3");
        }

        // Sulphur with two double bonds
        [TestMethod()]
        public void Chembl_1188068()
        {
            Test("COc1cc2nc(ns(=O)(C)c2cc1OC)N3CCN(CC3)C(=O)c4oc(SC)nn4",
                 "COC1=CC2=NC(=NS(=O)(C)=C2C=C1OC)N3CCN(CC3)C(=O)C=4OC(SC)=NN4");
        }

        // Sulphur cation with exo cyclic double bond
        [TestMethod()]
        public void Chembl_1434989()
        {
            Test("[O-][s+]1(=O)[nH]c2c(cc(Cl)c3ccc(Cl)nc23)c4ccccc14",
                 "[O-][S+]1(=O)[NH]C2=C(C=C(Cl)C3=CC=C(Cl)N=C23)C4=CC=CC=C14");
        }

        [TestMethod()]
        public void Chembl_423544()
        {
            Test("CCc1n[c]#[c]n1CC2CC(C(=O)O2)(c3ccccc3)c4ccccc4",
                 "CCC1=N[C]#[C]N1CC2CC(C(=O)O2)(C3=CC=CC=C3)C4=CC=CC=C4");
        }

        [TestMethod()]
        public void Chembl_422679()
        {
            Test("CCO/C(O)=C1\\C(COCCNc2n[s+]([O-])nc2OC)=NC(C)=C(C(=O)OC)C1c1cccc(Cl)c1Cl CHEMBL422679",
                 "CCO/C(O)=C1\\C(COCCNC2=N[S+]([O-])N=C2OC)=NC(C)=C(C(=O)OC)C1C3=CC=CC(Cl)=C3Cl");
        }

        [TestMethod()]
        public void Tropylium()
        {
            Test("[cH+]1cccccc1", "[CH+]1C=CC=CC=C1");
        }

        /// <summary>
        /// Test case from Noel that should fail Kekulization
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void ExocyclicCarbonFiveMemberRing()
        {
            Test("c1n(=C)ccc1", "n/a");
        }

        [TestMethod()]
        public void ExocyclicCarbonSixMemberRing()
        {
            Test("c1cn(=C)ccc1", "C1=CN(=C)=CC=C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Pyrole_invalid()
        {
            Test("n1cncc1", "n/a");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Imidazole_invalid()
        {
            Test("c1nc2ccccc2n1", "n/a");
        }

        [TestMethod()]
        public void Mixing_aromatic_and_aliphatic()
        {
            Test("c1=cc=cc=c1", "C1=CC=CC=C1");
            Test("c-1c-cc-cc1", "C-1=C-C=C-C=C1");
            Test("C:1:C:C:C:C:C1", "C1CCCCC1"); // XXX: not handled inplace
        }

        // http://sourceforge.net/mailarchive/forum.php?thread_name=60825b0f0709302037g2d68f2eamdb5ebecf3baea6d1%40mail.gmail.com&forum_name=blueobelisk-smiles
        [TestMethod()]
        public void bezene_inconsistent()
        {
            Test("c1=ccccc1", "C1=CC=CC=C1");
            Test("c1=cc=ccc1", "C1=CC=CC=C1");
            Test("c1=cc=cc=c1", "C1=CC=CC=C1");
            Test("c1=c:c:c:c:c1", "C1=CC=CC=C1");
            Test("c1=c:c=c:c:c1", "C1=CC=CC=C1");
            Test("c1=c-c=c:c:c1", "C1=C-C=CC=C1");
        }

        [TestMethod()]
        public void Fluorene()
        {
            Test("C1c2ccccc2-c3ccccc13", "C1C2=CC=CC=C2-C3=CC=CC=C13");
            Test("C1c2ccccc2c3ccccc13", "C1C2=CC=CC=C2C3=CC=CC=C13");
        }

        [TestMethod()]
        public void Hexaoxane()
        {
            Test("o1ooooo1", "O1OOOOO1");
        }

        [TestMethod()]
        public void Pyrole_aliphatic_n()
        {
            Test("c1cNcc1", "C1=CNC=C1");
        }

        [TestMethod()]
        public void Furan_aliphatic_o()
        {
            Test("c1cOcc1", "C1=COC=C1");
        }

        [TestMethod()]
        public void Bo_25756()
        {
            Test("Nc1c2c3ccccc3c4cccc(cc1)c24",
                        "NC1=C2C3=CC=CC=C3C=4C=CC=C(C=C1)C24");
        }

        /* Examples from http://www.daylight.com/dayhtml_tutorials/languages/smiles/smiles_examples.html */

        [TestMethod()]
        public void Viagra()
        {
            Test("CCc1nn(C)c2c(=O)[nH]c(nc12)c3cc(ccc3OCC)S(=O)(=O)N4CCN(C)CC4",
                 "CCC1=NN(C)C=2C(=O)[NH]C(=NC12)C3=CC(=CC=C3OCC)S(=O)(=O)N4CCN(C)CC4");
        }

        [TestMethod()]
        public void Xanax()
        {
            Test("Cc1nnc2CN=C(c3ccccc3)c4cc(Cl)ccc4-n12",
                "CC1=NN=C2CN=C(C3=CC=CC=C3)C4=CC(Cl)=CC=C4-N12");
        }

        [TestMethod()]
        public void Phentermine()
        {
            Test("CC(C)(N)Cc1ccccc1",
                 "CC(C)(N)CC1=CC=CC=C1");
        }

        [TestMethod()]
        public void Valium()
        {
            Test("CN1C(=O)CN=C(c2ccccc2)c3cc(Cl)ccc13",
                 "CN1C(=O)CN=C(C2=CC=CC=C2)C3=CC(Cl)=CC=C13");
        }

        [TestMethod()]
        public void Ambien()
        {
            Test("CN(C)C(=O)Cc1c(nc2ccc(C)cn12)c3ccc(C)cc3",
                 "CN(C)C(=O)CC1=C(N=C2C=CC(C)=CN12)C3=CC=C(C)C=C3");
        }

        [TestMethod()]
        public void Nexium()
        {
            Test("COc1ccc2[nH]c(nc2c1)S(=O)Cc3ncc(C)c(OC)c3C",
                 "COC1=CC=C2[NH]C(=NC2=C1)S(=O)CC3=NC=C(C)C(OC)=C3C");
        }

        [TestMethod()]
        public void Vioxx()
        {
            Test("CS(=O)(=O)c1ccc(cc1)C2=C(C(=O)OC2)c3ccccc3",
                 "CS(=O)(=O)C1=CC=C(C=C1)C2=C(C(=O)OC2)C3=CC=CC=C3");
        }

        [TestMethod()]
        public void Paxil()
        {
            Test("Fc1ccc(cc1)C2CCNCC2COc3ccc4OCOc4c3",
                 "FC1=CC=C(C=C1)C2CCNCC2COC3=CC=C4OCOC4=C3");
        }

        [TestMethod()]
        public void Lipitor()
        {
            Test("CC(C)c1c(C(=O)Nc2ccccc2)c(c(c3ccc(F)cc3)n1CC[C@@H]4C[C@@H](O)CC(=O)O4)c5ccccc5",
                 "CC(C)C1=C(C(=O)NC2=CC=CC=C2)C(=C(C3=CC=C(F)C=C3)N1CC[C@@H]4C[C@@H](O)CC(=O)O4)C5=CC=CC=C5");
        }

        [TestMethod()]
        public void Cialis()
        {
            Test("CN1CC(=O)N2[C@@H](c3[nH]c4ccccc4c3C[C@@H]2C1=O)c5ccc6OCOc6c5",
                 "CN1CC(=O)N2[C@@H](C=3[NH]C4=CC=CC=C4C3C[C@@H]2C1=O)C5=CC=C6OCOC6=C5");
        }

        [TestMethod()]
        public void Strychnine()
        {
            Test("O=C1C[C@H]2OCC=C3CN4CC[C@@]56[C@H]4C[C@H]3[C@H]2[C@H]6N1c7ccccc75",
                 "O=C1C[C@H]2OCC=C3CN4CC[C@]56[C@H]4C[C@H]3[C@H]2[C@H]5N1C7=CC=CC=C76");
        }

        [TestMethod()]
        public void Cocaine()
        {
            Test("COC(=O)[C@H]1[C@@H]2CC[C@H](C[C@@H]1OC(=O)c3ccccc3)N2C",
                 "COC(=O)[C@H]1[C@@H]2CC[C@H](C[C@@H]1OC(=O)C3=CC=CC=C3)N2C");
        }

        [TestMethod()]
        public void Quinine()
        {
            Test("COc1ccc2nccc([C@@H](O)[C@H]3C[C@@H]4CCN3C[C@@H]4C=C)c2c1",
                 "COC1=CC=C2N=CC=C([C@@H](O)[C@H]3C[C@@H]4CCN3C[C@@H]4C=C)C2=C1");
        }

        [TestMethod()]
        public void LysergicAcid()
        {
            Test("CN1C[C@@H](C=C2[C@H]1Cc3c[nH]c4cccc2c34)C(=O)O",
                 "CN1C[C@@H](C=C2[C@H]1CC3=C[NH]C4=CC=CC2=C34)C(=O)O");
        }

        [TestMethod()]
        public void LSD()
        {
            Test("CCN(CC)C(=O)[C@H]1CN(C)[C@@H]2Cc3c[nH]c4cccc(C2=C1)c34",
                 "CCN(CC)C(=O)[C@H]1CN(C)[C@@H]2CC3=C[NH]C4=CC=CC(C2=C1)=C34");
        }

        [TestMethod()]
        public void Morphine()
        {
            Test("CN1CC[C@]23[C@H]4Oc5c3c(C[C@@H]1[C@@H]2C=C[C@@H]4O)ccc5O",
                 "CN1CC[C@@]23[C@H]4OC5=C2C(C[C@@H]1[C@@H]3C=C[C@@H]4O)=CC=C5O");
        }

        [TestMethod()]
        public void Heroin()
        {
            Test("CN1CC[C@]23[C@H]4Oc5c3c(C[C@@H]1[C@@H]2C=C[C@@H]4OC(=O)C)ccc5OC(=O)C",
                 "CN1CC[C@@]23[C@H]4OC5=C2C(C[C@@H]1[C@@H]3C=C[C@@H]4OC(=O)C)=CC=C5OC(=O)C");
        }

        [TestMethod()]
        public void Nicotine()
        {
            Test("CN1CCC[C@H]1c2cccnc2",
                 "CN1CCC[C@H]1C2=CC=CN=C2");
        }

        [TestMethod()]
        public void Caffeine()
        {
            Test("Cn1cnc2n(C)c(=O)n(C)c(=O)c12",
                 "CN1C=NC=2N(C)C(=O)N(C)C(=O)C12");
        }

        // N,N-Diallylmelamine 
        [TestMethod()]
        public void Ncs4420()
        {
            Test("[nH2]c1nc(nc(n1)n(Ccc)Ccc)[nH2]",
                 "[NH2]C1=NC(=NC(=N1)N(CC=C)CC=C)[NH2]");
        }

        [TestMethod()]
        public void Carbon_anion()
        {
            Test("O=c1cc[cH-]cc1",
                 "O=C1C=C[CH-]C=C1");
            Test("oc1cc[cH-]cc1",
                 "O=C1C=C[CH-]C=C1");
        }

        [TestMethod()]
        public void Sulphur_cation()
        {
            Test("CC(C)(C)c1cc2c3[s-](oc2=O)oc(=O)c3c1",
                 "CC(C)(C)C1=CC2=C3[S-](OC2=O)OC(=O)C3=C1");
        }

        [TestMethod()]
        public void NitrogenRadical()
        {
            Test("c1cc(c([n+]c1)N)[N+](=O)[O-]",
                 "C1=CC(=C([N+]=C1)N)[N+](=O)[O-]");
        }

        [TestMethod()] public void SmallRingTest_5() 
        {
            Graph g = Graph.FromSmiles("C1CCCC1");
            Assert.IsTrue(Localise.InSmallRing(g, g.CreateEdge(0, 1)));
        }

        [TestMethod()] public void SmallRingTest_7() 
        {
            Graph g = Graph.FromSmiles("C1CCCCCC1");
            Assert.IsTrue(Localise.InSmallRing(g, g.CreateEdge(0, 1)));
        }

        [TestMethod()] public void SmallRingTest_8() 
        {
            Graph g = Graph.FromSmiles("C1CCCCCCC1");
            Assert.IsFalse(Localise.InSmallRing(g, g.CreateEdge(0, 1)));
        }

        [TestMethod()] public void SmallRingTest_linked() 
        {
            Graph g = Graph.FromSmiles("C1CCC(CC1)=C1CCCCC1");
            Assert.IsFalse(Localise.InSmallRing(g, g.CreateEdge(3, 6)));
        }

        static void Test(string delocalised, string localised)
        {
            Graph g = Graph.FromSmiles(delocalised);
            Graph h = Localise.GenerateLocalise(g);
            Assert.AreEqual(localised, h.ToSmiles());   //fixed Beam
        }

        /// <summary>
        /// Example from Noel's analysis generate with RDKit, the hcount changes before/after
        /// kekulization. This was problematic because explicit single bonds were incorrectly
        /// present in the SMILES string. Nether the less it should cause a problem.
        /// </summary>
        /// <exception cref="InvalidSmilesException"></exception>
        [TestMethod()]
        public void UnchangedHydrogenCount()
        {
            string smi = "c12c3c4c5c6c7c-4c4c8c9c%10c%11c%12c%13c%14c%15c%16c%17c%18c%19c%20c(c1c1c%21c%20c%20c%22c%19c%16c%16c%22c%19c%22c%20c=%21c(c8c%22c%10c%19c%12c%14%16)C48C31CC(=N)C=C8)c-%18c1c2c5c2c3c6c(c%11c79)c%13c-3c%15C%17C12";
            Graph g1 = Graph.FromSmiles(smi);
            Graph g2 = Graph.FromSmiles(smi).Kekule();
            for (int i = 0; i < g1.Order; i++)
                Assert.AreEqual(g1.ImplHCount(i), g2.ImplHCount(i), "Atom idx=" + i + " had a different hydrogen count before/after kekulization");
        }

        [TestMethod()]
        public void Biphenylene()
        {
            string smi = "c1cccc2-c3ccccc3-c12";
            Assert.AreEqual("C1=CC=CC=2-C3=CC=CC=C3-C12", Graph.FromSmiles(smi).Kekule().ToSmiles());
        }
    }
}
