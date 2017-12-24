using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;

namespace NCDK.Beam
{
   /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class DaylightDelocalisationTest
    {
        void Assert_AreEqual(object expected, object actual)
        {
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        [TestMethod()]
        public void Benzene()
        {
            Graph g = Graph.FromSmiles("[CH]1=[CH][CH]=[CH][CH]=[CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true, true });
        }

        [TestMethod()]
        public void Azulene()
        {
            Graph g = Graph.FromSmiles("[CH]1=[CH][C]2=[CH][CH]=[CH][CH]=[CH][C]2=[CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true, true, true, true, true, true });
        }

        [TestMethod()]
        public void Cyclopenta_b_azepine()
        {
            Graph g = Graph.FromSmiles("[CH]1=[CH][C]2=[N][CH]=[CH][CH]=[CH][C]2=[CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true, true, true, true, true, true });
        }

        [TestMethod()]
        public void Sp2_oxygen_cation()
        {
            Graph g = Graph.FromSmiles("[CH]1[NH+]=[CH][C]2=[CH][CH]=[CH][O+]12");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { false, false, false, false, false, false, false, false });
        }

        [TestMethod()]
        public void Pyridine()
        {
            Graph g = Graph.FromSmiles("[N]1=[CH][CH]=[CH][CH]=[CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true, true });
        }

        [TestMethod()]
        public void Test_1_H_pyrole()
        {
            Test("N1C=CC=C1", "[nH]1cccc1");
        }

        [TestMethod()]
        public void Pyridine_n_oxide()
        {
            Graph g = Graph.FromSmiles("[O]=[N]1=[CH][CH]=[CH][CH]=[CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { false, true, true, true, true, true, true });
        }

        [TestMethod()]
        public void Pyridine_n_oxide_charge_sep()
        {
            Graph g = Graph.FromSmiles("[O-][N+]1=[CH][CH]=[CH][CH]=[CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { false, true, true, true, true, true, true });
        }

        [TestMethod()]
        public void Furan()
        {
            Graph g = Graph.FromSmiles("O1C=CC=C1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true });
        }

        [TestMethod()]
        public void Thiophene()
        {
            Graph g = Graph.FromSmiles("[S]1[CH]=[CH][CH]=[CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true });
        }

        [TestMethod()]
        public void Cyclopentadienyl_anion()
        {
            Graph g = Graph.FromSmiles("[CH-]1[CH]=[CH][CH]=[CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true });
        }

        [TestMethod()]
        public void Cyclodecapentaene()
        {
            Graph g = Graph.FromSmiles("[CH]=1[CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true, true, true, true, true, true });
        }

        [TestMethod()]
        public void Cyclotetradecaheptaene()
        {
            Graph g = Graph.FromSmiles("[CH]=1[CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true });
        }

        [TestMethod()]
        public void Cyclooctadecanonaene()
        {
            Graph g = Graph.FromSmiles("[CH]=1[CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, });
        }

        [TestMethod()]
        public void Cyclodocosaundecaene()
        {
            Graph g = Graph.FromSmiles("[CH]=1[CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]=[CH][CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, });
        }

        [TestMethod()]
        public void Cyclohexa_g_chromen_6_one()
        {
            Graph g = Graph.FromSmiles("[O]=[C]1[CH]=[CH][CH]=[C]2[CH]=[C]3[O][CH]=[CH][CH]=[C]3[CH]=[C]12");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, });
        }

        // n.b. looks like a crown :-)
        [TestMethod()]
        public void Trioxanetrione()
        {
            Graph g = Graph.FromSmiles("[O]1[O][O][C](=[O])[C](=[O])[C](=[O])1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, false, true, false, true, false });
        }

        [TestMethod()]
        public void Dimethylidenecyclohexadiene()
        {
            Graph g = Graph.FromSmiles("[CH2]=[C]1[CH]=[CH][C](=[CH2])[CH]=[CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { false, true, true, true, true, false, true, true });
        }

        [TestMethod(), Ignore()]
        public void Test()
        {
            Graph g = Graph.FromSmiles("[CH2]=[C]1[CH]=[CH][N](=[CH2])=[CH]1");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { false, false, true, true, true, true, true, true });
        }

        [TestMethod()]
        public void Noroborane()
        {
            Graph g = Graph.FromSmiles("[CH2]=[C]1[C]2=[CH][CH]=[C]1[N]=[CH]2");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { false, false, true, true, true, true, true, true });
        }

        // http://www.eyesopen.com/docs/toolkits/current/html/OEChem_TK-python/_images/OEAssignAromaticFlags_Table.png
        [TestMethod(), Ignore()]
        public void Openeye_comparison_5()
        {
            Graph g = Graph.FromSmiles("[NH]1[C]2=[CH][CH]=[C]1[CH]=[C]3[CH]=[CH][C]([CH]=[C]4[NH][C]([CH]=[CH]4)=[CH][C]5=[N][C]([CH]=[CH]5)=[CH]2)=[N]3");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, });
        }

        // same as above but without the hydrogens http://www.eyesopen.com/docs/toolkits/current/html/OEChem_TK-python/_images/OEAssignAromaticFlags_Table.png
        [TestMethod()]
        public void Porphyrin()
        {
            Graph g = Graph.FromSmiles("C1=CC=2C=C3C=CC(C=C4C=CC(C=C5C=CC(C=C1N2)=N5)=N4)=N3");
            AllCycles d = AllCycles.DaylightModel(g);
            Assert_AreEqual(d.aromatic, new bool[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, });
        }

        [TestMethod()]
        public void Limit_all_cycles()
        {
            Test("C1=CC=CC=C1", "c1ccccc1", 6);
            Test("[CH+]1C=CC=CC=C1", "[CH+]1C=CC=CC=C1", 6);
        }

        [TestMethod()]
        public void Fullerene_c70()
        {
            Test("C1=2C3=C4C5=C1C1=C6C7=C5C5=C8C4=C4C9=C3C3=C%10C=2C2=C1C1=C%11C%12=C%13C%14=C%15C%16=C%17C%18=C%19C%20=C%16C%16=C%14C%12=C%12C%14=C%21C%22=C(C%20=C%16%14)C%14=C%19C%16=C(C4=C8C(=C%18%16)C4=C%17C%15=C(C7=C54)C%13=C61)C1=C%14C%22=C(C3=C91)C1=C%21C%12=C%11C2=C%101",
                 "c12c3c4c5c1c6c7c8c5c9c%10c4c%11c%12c3c%13c%14c2c%15c6c%16c%17c%18c%19c%20c%21c%22c%23c%24c%25c%26c%22c%27c%20c%18c%28c%29c%30c%31c(c%26c%27%29)c%32c%25c%33c(c%11c%10c(c%24%33)c%34c%23c%21c(c8c9%34)c%19c7%16)c%35c%32c%31c(c%13c%12%35)c%36c%30c%28c%17c%15c%14%36",
                 6);
        }

        /* Carbon Examples */

        // carbon gives 1 electron (double bond) (6 * 1) % 4 = 2 
        [TestMethod()]
        public void Carbon_6_memberRing()
        {
            Test("C1=CC=CC=C1", "c1ccccc1");
        }

        // carbon gives 1 electron (double bond) (10 * 1) % 4 = 2
        [TestMethod()]
        public void Carbon_10_memberRing()
        {
            Test("C1=CC=CC=CC=CC=C1", "c1ccccccccc1");
        }

        // carbon anion gives 2 electrons (lone pair) (6 * 1) % 4 = 2
        [TestMethod()]
        public void Carbon_anion_5_memberRing()
        {
            Test("[CH-]1C=CC=C1", "[cH-]1cccc1");
        }

        // carbon anion gives 2 electrons (lone pair) (6 * 1) % 4 = 2
        [TestMethod()]
        public void Carbon_dianion_5_memberRing()
        {
            Test("[CH-2]1C=CC=C1", "[CH-2]1C=CC=C1");
        }

        // carbon cation gives 1 electron (double bond) (5 * 1) % 4 != 2
        [TestMethod()]
        public void Carbon_cation_5_memberRing()
        {
            Test("[CH+]1C=CC=C1", "[CH+]1C=CC=C1");
        }

        // carbon cation (5 valent) 
        [TestMethod()]
        public void Carbon_cation_5v_5_memberRing()
        {
            Test("C=[C+]1=CC(=C)C=C1", "C=[C+]1=CC(=C)C=C1");
        }

        // carbon cation (5 valent) 
        [TestMethod()]
        public void Carbon_cation_5v_6_memberRing()
        {
            Test("C=[C+]1=CC=CC=C1", "C=[C+]1=CC=CC=C1");
            Test("[CH2+]1=CC=CC=C1", "[CH2+]1=CC=CC=C1");
        }

        // carbon cation gives 1 electron (double bond) (6 * 1) % 4 = 2, but not Sp2 hybridised?
        [TestMethod()]
        public void Carbon_cation_6_memberRing()
        {
            Test("C1=CC=[C+]C=C1", "c1cc[c+]cc1");
        }

        [TestMethod()]
        public void Carbon_cation_7_memberRing()
        {
            Test("C=1[CH+]C=CC=CC1", "c1[cH+]ccccc1");
        }

        // carbon dication gives 0 electron (4 * 1) % 4 != 2
        [TestMethod()]
        public void Carbon_dication_5_memberRing()
        {
            Test("[C+2]1C=CC=C1", "[C+2]1C=CC=C1");
        }

        // carbon dication gives 1 electron (double bond) (4 * 1) % 4 != 2 but unusual charge
        [TestMethod()]
        public void Carbon_dication_6_memberRing()
        {
            Test("C1=CC=[C+2]C=C1", "C1=CC=[C+2]C=C1");
        }

        // carbon gives 1 electron (double bond) (4 * 1) % 4 != 2 
        [TestMethod()]
        public void Carbon_5_memberRing_exoCyclic()
        {
            Test("O=C1C=CC=C1", "O=C1C=CC=C1");
        }

        // carbon gives 1 electron (double bond) (6 * 1) % 4 = 2 - but the exocyclic electronegatic double bond takes it away 
        [TestMethod()]
        public void Carbon_7_memberRing_exoCyclic_O()
        {
            Test("O=C1C=CC=CC=C1", "O=c1cccccc1");
        }

        // carbon gives 1 electron (double bond) (6 * 1) % 4 = 2 - but the exocyclic electronegatic double bond takes it away 
        [TestMethod()]
        public void Carbon_7_memberRing_exoCyclic_N()
        {
            Test("N=C1C=CC=CC=C1", "N=c1cccccc1");
        }

        // carbon gives 1 electron (double bond) (6 * 1) % 4 = 2 - but the exocyclic electronegatic double bond takes it away 
        [TestMethod()]
        public void Carbon_7_memberRing_exoCyclic_S()
        {
            Test("S=C1C=CC=CC=C1", "S=c1cccccc1");
        }

        // carbon gives 1 electron (double bond) (6 * 1) % 4 = 2 - but the exocyclic electronegatic double bond takes it away 
        [TestMethod()]
        public void Carbon_7_memberRing_exoCyclic_Se()
        {
            Test("[Se]=C1C=CC=CC=C1", "[Se]=c1cccccc1");
        }

        // carbon gives 1 electron (double bond) (6 * 1) % 4 = 2 - but the exocyclic electronegatic double bond takes it away 
        [TestMethod()]
        public void Carbon_7_memberRing_exoCyclic_P()
        {
            Test("P=C1C=CC=CC=C1", "P=c1cccccc1");
        }

        // carbon gives 1 electron (double bond) (6 * 1) % 4 = 2 - but the exocyclic electronegatic double bond takes it away 
        [TestMethod()]
        public void Carbon_7_memberRing_exoCyclic_As()
        {
            Test("[AsH]=C1C=CC=CC=C1", "[AsH]=c1cccccc1");
        }

        // carbon gives 1 electron (double bond) (6 * 1) % 4 = 2 - but the exocyclic electronegatic double bond takes it away 
        [TestMethod()]
        public void Carbon_7_memberRing_exoCyclic_B()
        {
            Test("B=C1C=CC=CC=C1", "B=c1cccccc1");
        }

        // carbon gives 1 electron (double bond) (7 * 1) % 4 != 2  
        [TestMethod()]
        public void Carbon_7_memberRing_exoCyclic_C()
        {
            Test("C=C1C=CC=CC=C1", "C=C1C=CC=CC=C1");
        }

        /* Nitrogen Examples */

        // 2 electrons from the lone-pair
        [TestMethod()]
        public void Nitrogen_5_memberRing()
        {
            Test("N1C=CC=C1", "[nH]1cccc1");
        }

        // 1 electron from the double-bond
        [TestMethod()]
        public void Nitrogen_6_memberRing()
        {
            Test("N=1C=CC=CC1", "n1ccccc1");
        }

        // 0 electrons (Sp3)
        [TestMethod()]
        public void Nitrogen_cation_5_memberRing()
        {
            Test("[NH2+]1C=CC=C1", "[NH2+]1C=CC=C1");
        }

        // 0 electrons (Sp3)
        [TestMethod()]
        public void Nitrogen_cation_6_memberRing()
        {
            Test("[NH2+]1C=CC=C1", "[NH2+]1C=CC=C1");
        }

        // 0 electrons (Sp3) note - 6 electrons in ring thus 4n+2 valid
        [TestMethod()]
        public void Nitrogen_cation_6_memberRing2()
        {
            Test("N1C=C[NH2+]C=C1", "N1C=C[NH2+]C=C1");
        }

        // 0 electrons (Sp2) - 4n+2 not valid
        [TestMethod()]
        public void Nitrogen_dication_5_memberRing()
        {
            Test("[NH+2]1C=CC=C1", "[NH+2]1C=CC=C1");
        }

        // 0 electrons (Sp2) - 4n+2 valid but abnormal charge
        [TestMethod()]
        public void Nitrogen_dication_6_memberRing()
        {
            Test("N1C=C[NH+2]C=C1", "N1C=C[NH+2]C=C1");
        }

        // 2 electrons (lone pair)
        [TestMethod()]
        public void Nitrogen_anion_5_memberRing()
        {
            Test("[N-]1C=CC=C1", "[n-]1cccc1");
        }

        // 2 electrons (lone pair) - 4n+2 not valid, unusual valence
        [TestMethod()]
        public void Nitrogen_anion_6_memberRing()
        {
            Test("[N-]=1C=CC=CC1", "[N-]=1C=CC=CC1");
        }

        // 2 electrons (lone pair) - 4n+2 valid - not aromatic
        [TestMethod()]
        public void Nitrogen_anion_6_memberRing2()
        {
            Test("[N-]1C=CNC=C1", "[N-]1C=CNC=C1");
        }

        // 1 electron (double bond) - 4n+2 valid - but Sp3 - not aromatic
        [TestMethod()]
        public void Nitrogen_anion_6_memberRing3()
        {
            Test("[NH2-]=1C=CC=CC1", "[NH2-]=1C=CC=CC1");
        }

        // 1 electron (double bond) - 4n+2 valid - but not aromatic (2 double bonds?)
        [TestMethod()]
        public void Nitrogen_anion_6_memberRing_exoCyclic_N()
        {
            Test("[N]=1(=N)C=CC=CC1", "[N]=1(=N)C=CC=CC1");
        }

        [TestMethod()]
        public void Nitrogen_anion_6_memberRing_exoCyclic_O()
        {
            Test("N=1(=O)C=CC=CC1", "n1(=O)ccccc1");
        }

        [TestMethod()]
        public void Nitrogen_anion_6_memberRing_exoCyclic_S()
        {
            Test("[N]=1(=S)C=CC=CC1", "[N]=1(=S)C=CC=CC1");
        }

        // okay it doesn't given 2 electron (note 4n+2 valid if case)
        [TestMethod()]
        public void Nitrogen_2_doubleBond_exocyclic_N()
        {
            Test("C=C1C=C[N](=N)=C1", "C=C1C=C[N](=N)=C1");
        }

        // okay it doesn't given 0 electron (note 4n+2 valid if case)
        [TestMethod()]
        public void Nitrogen_2_doubleBond_exocyclic_O()
        {
            Test("N=[N]1=COC=C1", "N=[N]1=COC=C1");
        }

        // 2 electrons from lone pair
        [TestMethod()]
        public void Nitrogen_anion()
        {
            Test("O=C1C=C[N-]C=C1", "O=c1cc[n-]cc1");
        }

        /* Oxygen Examples */

        [TestMethod()]
        public void Oxygen_5_member_ring()
        {
            Test("O1C=CC=C1", "o1cccc1");
        }

        // 4n+2 invalid
        [TestMethod()]
        public void Oxygen_6_member_ring()
        {
            Test("C=C1C=COC=C1", "C=C1C=COC=C1");
        }

        // 4n+2 not invalid
        [TestMethod()]
        public void Oxygen_7_member_ring()
        {
            Test("O1C=CC=CC=C1", "O1C=CC=CC=C1");
        }

        [TestMethod()]
        public void Oxygen_cation_5_member_ring()
        {
            Test("[OH+]1C=CC=C1", "[OH+]1C=CC=C1");
        }

        [TestMethod()]
        public void Oxygen_cation_6_member_ring()
        {
            Test("C=C1C=C[OH+]C=C1", "C=C1C=C[OH+]C=C1");
        }

        [TestMethod()]
        public void Oxygen_cation_7_member_ring()
        {
            Test("[OH+]1C=CC=CC=C1", "[OH+]1C=CC=CC=C1");
        }

        [TestMethod()]
        public void Oxygen_cation_5_member_ring_piBond()
        {
            Test("C=C1C=C[O+]=C1", "C=C1C=C[O+]=C1");
        }

        [TestMethod()]
        public void Oxygen_cation_6_member_ring_piBond()
        {
            Test("C1=CC=[O+]C=C1", "c1cc[o+]cc1");
        }

        [TestMethod()]
        public void Oxygen_cation_7_member_ring_piBond()
        {
            Test("C=C1C=CC=C[O+]=C1", "C=C1C=CC=C[O+]=C1");
        }

        /* Sulphur Examples */

        [TestMethod()]
        public void Sulfur_5_member_ring()
        {
            Test("S1C=CC=C1", "s1cccc1");
        }

        // 4n+2 invalid
        [TestMethod()]
        public void Sulfur_6_member_ring()
        {
            Test("C=C1C=CSC=C1", "C=C1C=CSC=C1");
        }

        // 4n+2 not invalid
        [TestMethod()]
        public void Sulfur_7_member_ring()
        {
            Test("S1C=CC=CC=C1", "S1C=CC=CC=C1");
        }

        [TestMethod()]
        public void Sulfur_cation_5_member_ring()
        {
            Test("[SH+]1C=CC=C1", "[SH+]1C=CC=C1");
        }

        [TestMethod()]
        public void Sulfur_cation_6_member_ring()
        {
            Test("C=C1C=C[SH+]C=C1", "C=C1C=C[SH+]C=C1");
        }

        [TestMethod()]
        public void Sulfur_cation_7_member_ring()
        {
            Test("[SH+]1C=CC=CC=C1", "[SH+]1C=CC=CC=C1");
        }

        [TestMethod()]
        public void Sulfur_cation_5_member_ring_piBond()
        {
            Test("C=C1C=C[S+]=C1", "C=C1C=C[S+]=C1");
        }

        [TestMethod()]
        public void Sulfur_cation_6_member_ring_piBond()
        {
            Test("C1=CC=[S+]C=C1", "c1cc[s+]cc1");
        }

        [TestMethod()]
        public void Sulfur_cation_7_member_ring_piBond()
        {
            Test("C=C1C=CC=C[S+]=C1", "C=C1C=CC=C[S+]=C1");
        }

        [TestMethod()]
        public void Nitrogen_3_valent_acyclic()
        {
            Test("C=N1C=CC(=C)C=C1", "C=N1C=CC(=C)C=C1");
            Test("N=N1C=CC(=C)C=C1", "N=N1C=CC(=C)C=C1");
            Test("O=N1C=CC(=C)C=C1", "O=N1C=CC(=C)C=C1");
            Test("P=N1C=CC(=C)C=C1", "P=N1C=CC(=C)C=C1");
            Test("S=N1C=CC(=C)C=C1", "S=N1C=CC(=C)C=C1");
            // cation 
            Test("C=[N+]1C=CC(=C)C=C1", "C=[n+]1ccc(=C)cc1");
            Test("N=[N+]1C=CC(=C)C=C1", "N=[n+]1ccc(=C)cc1");
            Test("O=[N+]1C=CC(=C)C=C1", "O=[n+]1ccc(=C)cc1");
            Test("P=[N+]1C=CC(=C)C=C1", "P=[n+]1ccc(=C)cc1");
            Test("S=[N+]1C=CC(=C)C=C1", "S=[n+]1ccc(=C)cc1");
            // anion (abnormal valence) 
            Test("C=[N-]1C=CC=C1", "C=[N-]1C=CC=C1");
            Test("N=[N-]1C=CC=C1", "N=[N-]1C=CC=C1");
            Test("O=[N-]1C=CC=C1", "O=[N-]1C=CC=C1");
            Test("P=[N-]1C=CC=C1", "P=[N-]1C=CC=C1");
            Test("S=[N-]1C=CC=C1", "S=[N-]1C=CC=C1");
        }

        [TestMethod()]
        public void Nitrogen_5_valent_acyclic()
        {
            Test("C=[N]1=CC=CC=C1", "C=[N]1=CC=CC=C1");
            Test("N=[N]1=CC=CC=C1", "N=[N]1=CC=CC=C1");
            Test("O=[N]1=CC=CC=C1", "O=[n]1ccccc1");
            Test("P=[N]1=CC=CC=C1", "P=[N]1=CC=CC=C1");
            Test("S=[N]1=CC=CC=C1", "S=[N]1=CC=CC=C1");
        }

        [TestMethod()]
        public void Phosphorus_acyclic()
        {
            Test("C=P1C=CC(=C)C=C1", "C=P1C=CC(=C)C=C1");
            Test("N=P1C=CC(=C)C=C1", "N=P1C=CC(=C)C=C1");
            Test("O=P1C=CC(=C)C=C1", "O=P1C=CC(=C)C=C1");
            Test("P=P1C=CC(=C)C=C1", "P=P1C=CC(=C)C=C1");
            Test("S=P1C=CC(=C)C=C1", "S=P1C=CC(=C)C=C1");
            // cation 
            Test("C=[P+]1C=CC(=C)C=C1", "C=[p+]1ccc(=C)cc1");
            Test("N=[P+]1C=CC(=C)C=C1", "N=[p+]1ccc(=C)cc1");
            Test("O=[P+]1C=CC(=C)C=C1", "O=[p+]1ccc(=C)cc1");
            Test("P=[P+]1C=CC(=C)C=C1", "P=[p+]1ccc(=C)cc1");
            Test("S=[P+]1C=CC(=C)C=C1", "S=[p+]1ccc(=C)cc1");
            // anion (valid but lone pair not given) 
            Test("C=[P-]1C=CC=C1", "C=[P-]1C=CC=C1");
            Test("N=[P-]1C=CC=C1", "N=[P-]1C=CC=C1");
            Test("O=[P-]1C=CC=C1", "O=[P-]1C=CC=C1");
            Test("P=[P-]1C=CC=C1", "P=[P-]1C=CC=C1");
            Test("S=[P-]1C=CC=C1", "S=[P-]1C=CC=C1");
        }

        [TestMethod()]
        public void Sulfur_acyclic()
        {
            Test("C=S1C=CC=C1", "C=S1C=CC=C1");
            Test("N=S1C=CC=C1", "N=S1C=CC=C1");
            Test("O=S1C=CC=C1", "O=s1cccc1");
            Test("P=S1C=CC=C1", "P=S1C=CC=C1");
            Test("S=S1C=CC=C1", "S=S1C=CC=C1");
            // cation
            Test("C=[SH+]1C=CC=C1", "C=[SH+]1C=CC=C1");
            Test("N=[SH+]1C=CC=C1", "N=[SH+]1C=CC=C1");
            Test("O=[SH+]1C=CC=C1", "O=[SH+]1C=CC=C1");
            Test("P=[SH+]1C=CC=C1", "P=[SH+]1C=CC=C1");
            Test("S=[SH+]1C=CC=C1", "S=[SH+]1C=CC=C1");
            // anion (note abnormal valence)
            Test("C=[S-]1C=CC=C1", "C=[S-]1C=CC=C1");
            Test("N=[S-]1C=CC=C1", "N=[S-]1C=CC=C1");
            Test("O=[S-]1C=CC=C1", "O=[S-]1C=CC=C1");
            Test("P=[S-]1C=CC=C1", "P=[S-]1C=CC=C1");
            Test("S=[S-]1C=CC=C1", "S=[S-]1C=CC=C1");
        }

        /* Phosphorus Examples */
        /* Arsenic Examples */

        [TestMethod()]
        public void Arsenic_acyclic()
        {
            Test("C=[As+]1C=COC=C1", "C=[As+]1C=COC=C1");
            Test("N=[As+]1C=COC=C1", "N=[As+]1C=COC=C1");
            Test("O=[As+]1C=COC=C1", "O=[As+]1C=COC=C1");
            Test("P=[As+]1C=COC=C1", "P=[As+]1C=COC=C1");
            Test("S=[As+]1C=COC=C1", "S=[As+]1C=COC=C1");
            // cation 
            Test("C=[As+]1C=CC(=C)C=C1", "C=[As+]1C=CC(=C)C=C1");
            Test("N=[As+]1C=CC(=C)C=C1", "N=[As+]1C=CC(=C)C=C1");
            Test("O=[As+]1C=CC(=C)C=C1", "O=[As+]1C=CC(=C)C=C1");
            Test("P=[As+]1C=CC(=C)C=C1", "P=[As+]1C=CC(=C)C=C1");
            Test("S=[As+]1C=CC(=C)C=C1", "S=[As+]1C=CC(=C)C=C1");
        }

        /* Selenium Examples */

        // misc

        [TestMethod()]
        public void Multi_cyclic_components()
        {
            Test("CCCCCC[N+]1=C(\\C=C/2\\C(=C(C2=O)C3=CC=C(S3)C4=CC=C(S4)C5=CC=C(S5)C6=C([O-])\\C(=C/C=7SC8=CC=CC=C8[N+]7CCCCCC)\\C6=O)[O-])SC9=CC=CC=C19",
                 "CCCCCC[n+]1c(\\C=C/2\\C(=C(C2=O)c3ccc(s3)c4ccc(s4)c5ccc(s5)C6=C([O-])\\C(=C/c7sc8ccccc8[n+]7CCCCCC)\\C6=O)[O-])sc9ccccc19");
        }

        /// Tests referring to compounds from - 
        /// http://blueobelisk.shapado.com/questions/IsAromaticity-perception-differences
        /// </summary>

        [TestMethod()]
        public void Bo_6678()
        {
            // note different from daylight due to their use of SSSR
            Test("O=C1OC(=O)C2=C3C1=CC=C1C(=O)OC(=O)C(C=C2)=C31",
                 "O=C1OC(=O)c2c3c1ccc4C(=O)OC(=O)c(cc2)c34");
        }

        [TestMethod()] public void Bo_8317()
        {
            // note different from daylight due to their use of SSS
            Test("O=C1C2=CC=CC=C2C2=C3C1=CC=C1C4=CC=C5C(=O)C6=CC=CC=C6C6=C5C4=C(C=C6)C(C=C2)=C31",
             "O=C1c2ccccc2c3c4c1ccc5c6ccc7C(=O)c8ccccc8c9c7c6c(cc9)c(cc3)c45");
        }

        [TestMethod()] public void Bo_8978()
        {
            Test("C1=CC=C2C(=C1)C1=N\\C\\2=N/C2=N/C(=N\\C3=N\\C(=N/C4=N/C(=N\\1)/C1=CC=CC=C41)\\C1=CC=CC=C31)/C1=CC=CC=C21",
             "c1ccc2c(c1)c3nc2nc4nc(nc5nc(nc6nc(n3)c7ccccc67)c8ccccc58)c9ccccc49");
        }

        [TestMethod()] public void Bo_18301()
        {
            // note different from daylight due to their use of SSSR
            Test("O=C1C=CC2=C3C1=CC=C1C(=O)C4=CC=CC=C4C(C=C2)=C31",
             "O=C1C=Cc2c3c1ccc4C(=O)c5ccccc5c(cc2)c34");
        }

        [TestMethod()] public void Bo_21963()
        {
            Test("O=C1C2=CC=CC3=C2C2=C(C=CC=C12)C=C3",
             "O=c1c2cccc3c2c4c(cccc14)cc3");
        }

        [TestMethod()] public void Bo_25756()
        {
            Test("NC1=C2C3=CC=CC=C3C3=CC=CC(C=C1)=C23",
             "Nc1c2c3ccccc3c4cccc(cc1)c24");
        }

        [TestMethod()] public void Bo_39171()
        {
            Test("O=C1C(=O)C2=CC3=CC=CC=C3C3=C2C2=C(C=CC=C12)C=C3",
             "O=c1c(=O)c2cc3ccccc3c4c2c5c(cccc15)cc4");
        }

        [TestMethod()] public void Bo_75696()
        {
            Test("O=C1NC(=O)C2=CC3=C(C=C12)C(=O)NC3=O",
             "O=c1[nH]c(=O)c2cc3c(cc12)c(=O)[nH]c3=O");
        }

        [TestMethod()]
        public void Bo_78222()
        {
            Test("[O-]S(=O)(=O)OC1=C2C=CC3=C(NC4=CC=C5C6=CC=CC=C6C(=O)C6=C5C4=C3C=C6)C2=C(OS([O-])(=O)=O)C2=CC=CC=C12",
             "[O-]S(=O)(=O)Oc1c2ccc3c([nH]c4ccc5c6ccccc6c(=O)c7c5c4c3cc7)c2c(OS([O-])(=O)=O)c8ccccc18");
        }

        [TestMethod()] public void Bo_83217()
        {
            Test("CN(C)C1=CC=[C-]C=C1",
             "CN(C)c1cc[c-]cc1");
        }

        /// <summary>
        /// Daylight Examples <seealso cref="http://www.daylight.com/dayhtml_tutorials/languages/smiles/smiles_examples.html"/>
        /// </summary>
        [TestMethod(), Ignore()] // need to kekulize 
        public void DaylightExamples()
        {
            Test("CCc1nn(C)c2c(=O)[nH]c(nc12)c3cc(ccc3OCC)S(=O)(=O)N4CCN(C)CC4", "CCc1nn(C)c2c(=O)[nH]c(nc12)c3cc(ccc3OCC)S(=O)(=O)N4CCN(C)CC4");
            Test("Cc1nnc2CN=C(c3ccccc3)c4cc(Cl)ccc4-n12", "Cc1nnc2CN=C(c3ccccc3)c4cc(Cl)ccc4-n12");
            Test("CC(C)(N)Cc1ccccc1", "CC(C)(N)Cc1ccccc1");
            Test("CN1C(=O)CN=C(c2ccccc2)c3cc(Cl)ccc13", "CN1C(=O)CN=C(c2ccccc2)c3cc(Cl)ccc13");
            Test("CN(C)C(=O)Cc1c(nc2ccc(C)cn12)c3ccc(C)cc3", "CN(C)C(=O)Cc1c(Nc2ccc(C)cn12)c3ccc(C)cc3");
            Test("COc1ccc2[nH]c(nc2c1)S(=O)Cc3ncc(C)c(OC)c3C", "COc1ccc2[nH]c(nc2c1)S(=O)Cc3ncc(C)c(OC)c3C");
            Test("CS(=O)(=O)c1ccc(cc1)C2=C(C(=O)OC2)c3ccccc3", "CS(=O)(=O)c1ccc(cc1)C2=C(C(=O)OC2)c3ccccc3");
            Test("Fc1ccc(cc1)C2CCNCC2COc3ccc4OCOc4c3", "Fc1ccc(cc1)C2CCNCC2COc3ccc4OCOc4c3");
            Test("CC(C)c1c(C(=O)Nc2ccccc2)c(c(c3ccc(F)cc3)n1CC[C@@H]4C[C@@H](O)CC(=O)O4)c5ccccc5", "CC(C)c1c(C(=O)Nc2ccccc2)c(c(c3ccc(F)cc3)n1CC[C@@H]4C[C@@H](O)CC(=O)O4)c5ccccc5");
            Test("CN1CC(=O)N2[C@@H](c3[nH]c4ccccc4c3C[C@@H]2C1=O)c5ccc6OCOc6c5", "CN1CC(=O)N2[C@@H](c3[nH]c4ccccc4c3C[C@@H]2C1=O)c5ccc6OCOc6c5");
            Test("O=C1C[C@H]2OCC=C3CN4CC[C@@]56[C@H]4C[C@H]3[C@H]2[C@H]6N1c7ccccc75", "O=C1C[C@H]2OCC=C3CN4CC[C@@]56[C@H]4C[C@H]3[C@H]2[C@H]6N1c7ccccc75");
            Test("COC(=O)[C@H]1[C@@H]2CC[C@H](C[C@@H]1OC(=O)c3ccccc3)N2C", "COC(=O)[C@H]1[C@@H]2CC[C@H](C[C@@H]1OC(=O)c3ccccc3)N2C");
            Test("COc1ccc2nccc([C@@H](O)[C@H]3C[C@@H]4CCN3C[C@@H]4C=C)c2c1", "COc1ccc2nccc([C@@H](O)[C@H]3C[C@@H]4CCN3C[C@@H]4C=C)c2c1");
            Test("CN1C[C@@H](C=C2[C@H]1Cc3c[nH]c4cccc2c34)C(=O)O", "CN1C[C@@H](C=C2[C@H]1Cc3c[nH]c4cccc2c34)C(=O)O");
            Test("CCN(CC)C(=O)[C@H]1CN(C)[C@@H]2Cc3c[nH]c4cccc(C2=C1)c34", "CCN(CC)C(=O)[C@H]1CN(C)[C@@H]2Cc3c[nH]c4cccc(C2=C1)c34");
            Test("CN1CC[C@]23[C@H]4Oc5c3c(C[C@@H]1[C@@H]2C=C[C@@H]4O)ccc5O", "CN1CC[C@]23[C@H]4Oc5c3c(C[C@@H]1[C@@H]2C=C[C@@H]4O)ccc5O");
            Test("CN1CC[C@]23[C@H]4Oc5c3c(C[C@@H]1[C@@H]2C=C[C@@H]4OC(=O)C)ccc5OC(=O)C", "CN1CC[C@]23[C@H]4Oc5c3c(C[C@@H]1[C@@H]2C=C[C@@H]4OC(=O)C)ccc5OC(=O)C");
            Test("CN1CCC[C@H]1c2cccnc2", "CN1CCC[C@H]1c2cccnc2");
            Test("Cn1cnc2n(C)c(=O)n(C)c(=O)c12", "Cn1cnc2n(C)c(=O)n(C)c(=O)c12");
            Test("C/C(=C\\CO)/C=C/C=C(/C)\\C=C\\C1=C(C)CCCC1(C)C", "C/C(=C\\CO)/C=C/C=C(/C)\\C=C\\C1=C(C)CCCC1(C)C");
        }

        [TestMethod()] public void Non_daylight_aromatic_element()
        {
            Test("CC1=CC=C2[Bi](Cl)C3=CC=CC=C3S(=O)(=O)C2=C1",
             "Cc1ccc2[Bi](Cl)c3ccccc3S(=O)(=O)c2c1");
        }

        [TestMethod()] public void Acyclic_charge()
        {
            Test("[Na+].[Na+].[S-2]",
             "[Na+].[Na+].[S-2]");
        }


        private static void Test(string org, string exp)
        {
            Graph g = Graph.FromSmiles(org);
            Graph h = AllCycles.DaylightModel(g).AromaticForm();
            Assert.AreEqual(h.ToSmiles(), exp);
        }

        private static void Test(string org, string exp, int lim)
        {
            Graph g = Graph.FromSmiles(org);
            Graph h = AllCycles.DaylightModel(g, lim).AromaticForm();
            Assert.AreEqual(h.ToSmiles(), exp);
        }
    }
}
