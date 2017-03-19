using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Beam
{
    /// <summary>
    /// Unit tests ensure round tripping for all examples in the (<a
    /// href="http://www.daylight.com/dayhtml/doc/theory/theory.smiles.html">Daylight
    /// theory manual</a>)
    /// </summary>
    // @author John May
    [TestClass()]
    public class DaylightRoundTrippingTest
    {

        // 3. SMILES - A Simplified Chemical Language

        [TestMethod()]
        public void Ethane()
        {
            RoundTrip("CC");
        }

        [TestMethod()]
        public void CarbonDioxide()
        {
            RoundTrip("O=C=O");
        }

        [TestMethod()]
        public void HydrogenCyanide()
        {
            RoundTrip("C#N");
        }

        [TestMethod()]
        public void Triethylamine()
        {
            RoundTrip("CCN(CC)CC");
        }

        [TestMethod()]
        public void AceticAcid()
        {
            RoundTrip("CC(=O)O");
        }

        [TestMethod()]
        public void Cyclohexane()
        {
            RoundTrip("C1CCCCC1");
        }

        [TestMethod()]
        public void Benzene()
        {
            RoundTrip("c1ccccc1");
        }

        [TestMethod()]
        public void HydroniumIon()
        {
            RoundTrip("[OH3+]");
        }

        [TestMethod()]
        public void DeuteriumOxide()
        {
            RoundTrip("[2H]O[2H]");
        }

        [TestMethod()]
        public void Uranium235()
        {
            RoundTrip("[235U]");
        }

        [TestMethod()]
        public void EDifluoroethene()
        {
            RoundTrip("Cl/C=C/F");
        }

        [TestMethod()]
        public void ZDifluoroethene()
        {
            RoundTrip("F/C=C\\F");
        }

        [TestMethod()]
        public void LAlanine()
        {
            RoundTrip("N[C@@H](C)C(=O)O");
        }

        [TestMethod()]
        public void DAlanine()
        {
            RoundTrip("N[C@H](C)C(=O)O");
        }

        // 3.2.1 Atoms

        [TestMethod()]
        public void Methane()
        {
            RoundTrip("C");
        }

        [TestMethod()]
        public void Phosphine()
        {
            RoundTrip("P");
        }

        [TestMethod()]
        public void Ammonia()
        {
            RoundTrip("N");
        }

        [TestMethod()]
        public void HydrogenSulfide()
        {
            RoundTrip("S");
        }

        [TestMethod()]
        public void Water()
        {
            RoundTrip("O");
        }

        [TestMethod()]
        public void HydrochloricAcid()
        {
            RoundTrip("Cl");
        }

        [TestMethod()]
        public void ElementalSulfur()
        {
            RoundTrip("[S]");
        }

        [TestMethod()]
        public void ElementalGold()
        {
            RoundTrip("[Au]");
        }

        [TestMethod()]
        public void Proton()
        {
            RoundTrip("[H+]");
        }

        [TestMethod()]
        public void IronIIcation()
        {
            RoundTrip("[Fe+2]");
        }

        [TestMethod()]
        public void HydroxylAnion()
        {
            RoundTrip("[OH-]");
        }

        [TestMethod()]
        public void IronIIcation2()
        {
            RoundTrip("[Fe++]", "[Fe+2]");
        }

        [TestMethod()]
        public void HydroniumCation()
        {
            RoundTrip("[OH3+]");
        }

        [TestMethod()]
        public void AmmoniumCation()
        {
            RoundTrip("[NH4+]");
        }

        // 3.2.2 Bonds

        [TestMethod()]
        public void Formaldehyde()
        {
            RoundTrip("C=O");
        }

        [TestMethod()]
        public void Ethene()
        {
            RoundTrip("C=C");
        }

        [TestMethod()]
        public void DimethylEther()
        {
            RoundTrip("COC");
        }

        [TestMethod()]
        public void Ethanol()
        {
            RoundTrip("CO");
        }

        [TestMethod()]
        public void MolecularHydrogen()
        {
            RoundTrip("[H][H]");
        }

        [TestMethod()]
        public void Test_6_hydroxy_1_4_hexadiene()
        {
            RoundTrip("[CH2]=[CH]-[CH2]-[CH]=[CH]-[CH2]-[OH]");
            RoundTrip("C=CCC=CCO");
            RoundTrip("C=C-C-C=C-C-O");
            RoundTrip("OCC=CCC=C");
        }

        // 3.2.4 Branches

        [TestMethod()]
        public void Triethylamine_2()
        {
            RoundTrip("CCN(CC)CC");
        }

        [TestMethod()]
        public void IsobutyricAcid()
        {
            RoundTrip("CC(C)C(=O)O");
        }

        [TestMethod()]
        public void Test_3_propyl_4_isopropyl_1_heptene()
        {
            RoundTrip("C=CC(CCC)C(C(C)C)CCC");
        }

        // 3.2.4 Cyclic Structures

        [TestMethod()]
        public void Cyclohexane_2()
        {
            RoundTrip("C1CCCCC1");
        }

        [TestMethod()]
        public void Test_1_methyl_3_bromo_cyclohexene_1_1()
        {
            RoundTrip("CC1=CC(Br)CCC1");
        }

        [TestMethod()]
        public void Test_1_methyl_3_bromo_cyclohexene_1_2()
        {
            RoundTrip("CC1=CC(CCC1)Br");
        }

        [TestMethod()]
        public void Cubane()
        {
            RoundTrip("C12C3C4C1C5C4C3C25");
        }

        [TestMethod()]
        public void Test_1_oxan_2_yl_piperidine()
        {
            RoundTrip("O1CCCCC1N1CCCCC1", "O1CCCCC1N2CCCCC2");
        }

        // 3.2.5 Disconnected Structures

        [TestMethod()]
        public void SodiumPhenoxide_1()
        {
            RoundTrip("[Na+].[O-]c1ccccc1");
        }

        [TestMethod()]
        public void SodiumPhenoxide_2()
        {
            RoundTrip("C1cc([O-].[Na+])ccc1", "C1cc([O-])ccc1.[Na+]");
        }

        [TestMethod()]
        public void DisconnectedEthane()
        {
            RoundTrip("C1.C1", "CC");
        }

        // 3.3 Isomeric SMILES
        // 3.3.1 Isotopic Specification

        [TestMethod()]
        public void Carbon_12()
        {
            RoundTrip("[12C]");
        }

        [TestMethod()]
        public void Carbon_13()
        {
            RoundTrip("[13C]");
        }

        [TestMethod()]
        public void Carbon_unspecifiedMass()
        {
            RoundTrip("[C]");
        }

        [TestMethod()]
        public void C13Methane()
        {
            RoundTrip("[13CH4]");
        }

        // 3.3.2 Configuration Around Double Bonds

        [TestMethod()]
        public void Difluoroethene_1()
        {
            RoundTrip("F/C=C/F");
        }

        [TestMethod()]
        public void Difluoroethene_2()
        {
            RoundTrip("F\\C=C\\F");
        }

        [TestMethod()]
        public void Difluoroethene_3()
        {
            RoundTrip("F/C=C\\F");
        }

        [TestMethod()]
        public void Difluoroethene_4()
        {
            RoundTrip("F\\C=C/F");
        }

        [TestMethod()]
        public void CompletelySpecified()
        {
            RoundTrip("F/C=C/C=C/C");
        }

        [TestMethod()]
        public void PartiallySpecified()
        {
            RoundTrip("F/C=C/C=CC");
        }

        // 3.3.3. Configuration Around Tetrahedral Centers

        [TestMethod()]
        public void UnspecifiedChirality()
        {
            RoundTrip("NC(C)(F)C(=O)O");
        }

        [TestMethod()]
        public void SpecifiedChirality_1()
        {
            RoundTrip("N[C@](C)(F)C(=O)O");
        }

        [TestMethod()]
        public void SpecifiedChirality_2()
        {
            RoundTrip("N[C@@](C)(F)C(=O)O");
        }

        [TestMethod()]
        public void LAlanine_1()
        {
            RoundTrip("N[C@@]([H])(C)C(=O)O");
        }

        [TestMethod()]
        public void LAlanine_2()
        {
            RoundTrip("N[C@@H](C)C(=O)O");
        }

        [TestMethod()]
        public void LAlanine_3()
        {
            RoundTrip("N[C@H](C(=O)O)C");
        }

        [TestMethod()]
        public void LAlanine_4()
        {
            RoundTrip("[H][C@](N)(C)C(=O)O");
        }

        [TestMethod()]
        public void LAlanine_5()
        {
            RoundTrip("[C@H](N)(C)C(=O)O");
        }

        [TestMethod()]
        public void DAlanine_1()
        {
            RoundTrip("N[C@]([H])(C)C(=O)O");
        }

        [TestMethod()]
        public void DAlanine_2()
        {
            RoundTrip("N[C@H](C)C(=O)O");
        }

        [TestMethod()]
        public void DAlanine_3()
        {
            RoundTrip("N[C@@H](C(=O)O)C");
        }

        [TestMethod()]
        public void DAlanine_4()
        {
            RoundTrip("[H][C@@](N)(C)C(=O)O");
        }

        [TestMethod()]
        public void DAlanine_5()
        {
            RoundTrip("[C@@H](N)(C)C(=O)O");
        }

        [TestMethod()]
        public void Methyloxane_1()
        {
            RoundTrip("C[C@H]1CCCCO1");
        }

        [TestMethod()]
        public void Methyloxane_2()
        {
            RoundTrip("O1CCCC[C@@H]1C");
        }

        // 3.3.4 General Chiral Specification

        // not yet supported

        static void RoundTrip(string smi)
        {
            RoundTrip(smi, smi);
        }

        static void RoundTrip(string smi, string exp)
        {
            try
            {
                Assert.AreEqual(Generator.Generate(Parser.Parse(smi)), exp);
            }
            catch (InvalidSmilesException e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
