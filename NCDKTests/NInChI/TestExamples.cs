using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.NInChI
{
    [TestClass()]
    public class TestExamples
    {
        [TestMethod()]
        public void TestExampleStructure2InchiDiChloroethene()
        {
            // START SNIPPET: structure2inchi-dichloroethene
            // Example input - 2D E-1,2-dichloroethene
            NInchiInput input = new NInchiInput();
            //
            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(2.866, -0.250, 0.000, "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(3.732, 0.250, 0.000, "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(2.000, 2.500, 0.000, "Cl"));
            NInchiAtom a4 = input.Add(new NInchiAtom(4.598, -0.250, 0.000, "Cl"));
            a1.ImplicitH = 1;
            a2.ImplicitH = 1;
            //
            // Add bond
            input.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Double));
            input.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Add(new NInchiBond(a2, a4, INCHI_BOND_TYPE.Single));
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            //END SNIPPET: structure2inchi - dichloroethene
        }


        [TestMethod()]
        public void TestExampleStructure2InchiAlanine()
        {
            // START SNIPPET: structure2inchi-alanine
            // Example input - 0D D-Alanine
            NInchiInput input = new NInchiInput();
            //
            // Generate atoms
            NInchiAtom a1 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a2 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a3 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "N"));
            NInchiAtom a4 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "C"));
            NInchiAtom a5 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "O"));
            NInchiAtom a6 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "O"));
            NInchiAtom a7 = input.Add(new NInchiAtom(0.0, 0.0, 0.0, "H"));
            a3.ImplicitH = 2;
            a4.ImplicitH = 3;
            a5.ImplicitH = 1;
            //
            // Add bonds
            input.Add(new NInchiBond(a1, a2, INCHI_BOND_TYPE.Single));
            input.Add(new NInchiBond(a1, a3, INCHI_BOND_TYPE.Single));
            input.Add(new NInchiBond(a1, a4, INCHI_BOND_TYPE.Single));
            input.Add(new NInchiBond(a2, a5, INCHI_BOND_TYPE.Single));
            input.Add(new NInchiBond(a2, a6, INCHI_BOND_TYPE.Double));
            input.Add(new NInchiBond(a1, a7, INCHI_BOND_TYPE.Single));
            //
            // Add stereo parities
            input.Stereos.Add(NInchiStereo0D
                    .CreateNewTetrahedralStereo0D(a1, a3, a4, a7, a2, INCHI_PARITY.Even));
            //
            NInchiOutput output = NInchiWrapper.GetInchi(input);
            // END SNIPPET: structure2inchi-alanine
        }

        [TestMethod()]
        public void TestInchi2InchiHydrogen()
        {
            // START SNIPPET: inchi2inchi-hydrogen
            // Input InChI with fixed-hydrogen layer
            string inchiIn = "InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/"
                    + "t2-/m0/s1/f/h5H";
            //
            NInchiOutput output = NInchiWrapper.GetInchiFromInchi(
                        new NInchiInputInchi(inchiIn));
            string inchiOut = output.InChI;
            // Output InChI:
            //   InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1
            // END SNIPPET: inchi2inchi-hydrogen
        }

        [TestMethod()]
        public void TestInchi2InchiCompress()
        {
            // START SNIPPET: inchi2inchi-compress
            string inchi = "InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)";
            //
            // Compress InChI
            NInchiOutput cout = NInchiWrapper.GetInchiFromInchi(
                        new NInchiInputInchi(inchi, "-compress"));
            string compressedInchi = cout.InChI;
            // compressedInchi = InChI=1/C3H7NO2/cABBCC/hB1D2A3,1EF
            //
            // Uncompress InChI
            NInchiOutput ucout = NInchiWrapper.GetInchiFromInchi(
                    new NInchiInputInchi(compressedInchi));
            string uncompressedInchi = ucout.InChI;
            // uncompressedInchi = InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)
            // END SNIPPET: inchi2inchi-compress
        }

        [TestMethod()]
        public void TestInchi2Structure()
        {
            // START SNIPPET: inchi2structure
            NInchiInputInchi input = new NInchiInputInchi("InChI=1/C2H6/c1-2/h1-2H3");
            NInchiOutputStructure output = NInchiWrapper.GetStructureFromInchi(input);
            //
            INCHI_RET retStatus = output.ReturnStatus;
            int nat = output.Atoms.Count;
            int nbo = output.Bonds.Count;
            int nst = output.Stereos.Count;
            //
            NInchiAtom at0 = output.Atoms[0];
            // END SNIPPET: inchi2structure
        }

        [TestMethod()]
        public void TestInchi2InchiKey()
        {

            // START SNIPPET: inchi2inchikey
            NInchiOutputKey output = NInchiWrapper.GetInchiKey("InChI=1S/C2H6/c1-2/h1-2H3");
            string key = output.Key;
            // END SNIPPET: inchi2inchikey
        }

        [TestMethod()]
        public void TestCheckInchi()
        {
            // START SNIPPET: checkinchi
            bool strict = true;
            INCHI_STATUS status = NInchiWrapper.CheckInchi("InChI=1S/C2H6/c1-2/h1-2H3", strict);
            // END SNIPPET: checkinchi
        }

        [TestMethod()]
        public void TestCheckInchiKey()
        {
            // START SNIPPET: checkinchikey
            INCHI_KEY_STATUS status = NInchiWrapper.CheckInchiKey("OTMSDBZUPAUEDD-UHFFFAOYSA-N");
            // END SNIPPET: checkinchikey
        }

        [TestMethod()]
        public void TestAuxinfo2Input()
        {
            // START SNIPPET: auxinfo2input
            NInchiInputData data = NInchiWrapper.GetInputFromAuxInfo("AuxInfo=1/1/N:4,1,2,3,5,6/"
                        + "E:(5,6)/it:im/rA:6CCNCOO/rB:s1;N1;s1;s2;d2;/rC:264,968,0;295,985,0;233,986,0;"
                        + "264,932,0;326,967,0;295,1021,0;");
            INCHI_RET ret = data.ReturnStatus;
            NInchiInput input = data.Input;
            // END SNIPPET: auxinfo2input
        }
    }
}
