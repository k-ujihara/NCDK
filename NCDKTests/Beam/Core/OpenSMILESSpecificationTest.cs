/*
 * Copyright (c) 2013, European Bioinformatics Institute (EMBL-EBI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies,
 * either expressed or implied, of the FreeBSD Project.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Beam
{
    /// <summary>
    /// Unit tests ensure round tripping for examples in the (<a
    /// href="http://www.opensmiles.org/opensmiles.htmll">OpenSMILES
    /// specification</a>)
    // @author John May
    /// </summary>
    [TestClass()]
    public class OpenSMILESSpecificationTest
    {
        // Atoms

        // Atomic Symbol

        [TestMethod()]
        public void Uranium()
        {
            RoundTrip("[U]");
        }

        [TestMethod()]
        public void Lead()
        {
            RoundTrip("[Pb]");
        }

        [TestMethod()]
        public void Helium()
        {
            RoundTrip("[He]");
        }

        [TestMethod()]
        public void UnknownTest()
        {
            RoundTrip("[*]");
        }

        // Hydrogens

        [TestMethod()]
        public void Methane()
        {
            RoundTrip("[CH4]");
        }

        [TestMethod()]
        public void HydrochloricAcid1()
        {
            RoundTrip("[ClH]");
        }

        [TestMethod()]
        public void HydrochloricAcid2()
        {
            RoundTrip("[ClH1]", "[ClH]");
        }

        // Charge

        [TestMethod()]
        public void Chloride_anion()
        {
            RoundTrip("[Cl-]");
        }

        [TestMethod()]
        public void Hydroxyl_anion1()
        {
            RoundTrip("[OH1-]", "[OH-]");
        }

        [TestMethod()]
        public void Hydroxyl_anion2()
        {
            RoundTrip("[OH-1]", "[OH-]");
        }

        [TestMethod()]
        public void Chloride_cation1()
        {
            RoundTrip("[Cu+2]");
        }

        [TestMethod()]
        public void Chloride_cation2()
        {
            RoundTrip("[Cu++]", "[Cu+2]");
        }

        // Isotopes

        [TestMethod()]
        public void Methane_c13()
        {
            RoundTrip("[13CH4]");
        }

        [TestMethod()]
        public void Deuterium()
        {
            RoundTrip("[13CH4]");
        }

        [TestMethod()]
        public void Uranium238()
        {
            RoundTrip("[238U]");
        }

        // Organic subset
        // see. ElementTest

        // Wildcard

        [TestMethod()]
        public void OrthoSubstitutedPhenol()
        {
            RoundTrip("Oc1c(*)cccc1");
        }

        // Atom Class

        [TestMethod()]
        public void Methane_atomClass2()
        {
            RoundTrip("[CH4:2]");
        }

        // Bonds

        [TestMethod()]
        public void Ethane()
        {
            RoundTrip("CC");
        }

        [TestMethod()]
        public void Ethanol()
        {
            RoundTrip("CCO");
        }

        [TestMethod()]
        public void N_butylamine1()
        {
            RoundTrip("NCCCC");
        }

        [TestMethod()]
        public void N_butylamine2()
        {
            RoundTrip("CCCCN");
        }

        [TestMethod()]
        public void Ethene()
        {
            RoundTrip("C=C");
        }

        [TestMethod()]
        public void HydrogenCyanide()
        {
            RoundTrip("C#N");
        }

        [TestMethod()]
        public void Test_2_butyne()
        {
            RoundTrip("CC#CC");
        }

        [TestMethod()]
        public void Propanol()
        {
            RoundTrip("CCC=O");
        }

        [TestMethod()]
        public void Octachlorodirhenate_III()
        {
            RoundTrip("[Rh-](Cl)(Cl)(Cl)(Cl)$[Rh-](Cl)(Cl)(Cl)Cl");
        }

        [TestMethod()]
        public void Ethane_explict_single_bond()
        {
            RoundTrip("C-C");
        }

        [TestMethod()]
        public void Ethanol_explict_single_bonds()
        {
            RoundTrip("C-C-O");
        }

        [TestMethod()]
        public void Butene_explict_single_bonds()
        {
            RoundTrip("C-C=C-C");
        }

        // Branches

        [TestMethod()]
        public void Test_2_ethyl_1_butanol()
        {
            RoundTrip("CCC(CC)CO");
        }

        [TestMethod()]
        public void Test_2_4_dimethyl_3_penthanone()
        {
            RoundTrip("CC(C)C(=O)C(C)C");
        }

        [TestMethod()]
        public void Test_2_propyl_3_isopropyl_1_propanol()
        {
            RoundTrip("OCC(CCC)C(C(C)C)CCC");
        }

        [TestMethod()]
        public void Thiosulfate()
        {
            RoundTrip("OS(=O)(=S)O");
        }

        [TestMethod()]
        public void C22H46()
        {
            RoundTrip("C(C(C(C(C(C(C(C(C(C(C(C(C(C(C(C(C(C(C(C(C))))))))))))))))))))C",
                      "C(CCCCCCCCCCCCCCCCCCCC)C");
        }

        // Rings

        [TestMethod()]
        public void Cyclohexane()
        {
            RoundTrip("C1CCCCC1");
        }

        [TestMethod()]
        public void Perhydroisoquinoline()
        {
            RoundTrip("N1CC2CCCC2CC1");
        }

        [TestMethod()]
        public void Cyclohexene1()
        {
            RoundTrip("C=1CCCCC=1",
                      "C=1CCCCC1");
        }

        [TestMethod()]
        public void Cyclohexene2()
        {
            RoundTrip("C1CCCCC=1",
                      "C=1CCCCC1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Cyclohexene_invalid()
        {
            Graph.FromSmiles("C-1CCCCC=1");
        }

        [TestMethod()]
        public void Cyclohexene_preferred()
        {
            RoundTrip("C=1CCCCC1");
        }

        [TestMethod()]
        public void Dicyclohexyl_reusing_rnums()
        {
            RoundTrip("C1CCCCC1C1CCCCC1",
                      "C1CCCCC1C2CCCCC2");
        }

        [TestMethod()]
        public void Dicyclohexyl_unique_rnums()
        {
            RoundTrip("C1CCCCC1C2CCCCC2");
        }

        [TestMethod()]
        public void Cyclohexane_rnum0()
        {
            RoundTrip("C0CCCCC0",
                      "C1CCCCC1");
        }

        [TestMethod()]
        public void Cyclohexane_2digit_rnum()
        {
            RoundTrip("C%25CCCCC%25",
                      "C1CCCCC1");
        }

        [TestMethod()]
        public void Max_rnum_99()
        {
            RoundTrip("C%123CCCCC%12CCC3",
                      "C12CCCCC1CCC2");
        }

        [TestMethod()]
        public void Mix_2digit_rnums_0()
        {
            RoundTrip("C0CCCCC%0",
                      "C1CCCCC1");
        }

        [TestMethod()]
        public void Mix_2digit_rnums_1()
        {
            RoundTrip("C1CCCCC%01",
                      "C1CCCCC1");
        }

        [TestMethod()]
        public void Spiro_5_5_undecane()
        {
            RoundTrip("C12(CCCCC1)CCCCC2");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Multi_edge_1()
        {
            Graph.FromSmiles("C12CCCCC12");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Multi_edge_2()
        {
            Graph.FromSmiles("C12C2CCC1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Loop()
        {
            Graph.FromSmiles("C11");
        }

        // Aromaticity

        [TestMethod()]
        public void Benzene()
        {
            RoundTrip("c1ccccc1");
        }

        [TestMethod()]
        public void Benzene_kekule()
        {
            RoundTrip("C1=CC=CC=C1");
        }

        [TestMethod()]
        public void Indane()
        {
            RoundTrip("c1ccc2CCCc2c1");
        }

        [TestMethod()]
        public void Indane_kekule()
        {
            RoundTrip("C1=CC=CC(CCC2)=C12",
                      "C1=CC=CC=2CCCC21"); // input wasn't a DFS
        }

        [TestMethod()]
        public void Furan()
        {
            RoundTrip("c1occc1");
        }

        [TestMethod()]
        public void Furan_kekule()
        {
            RoundTrip("C1OC=CC=1",
                      "C=1OC=CC1"); // ring bond on open
        }

        [TestMethod()]
        public void Cyclobutadiene()
        {
            RoundTrip("c1ccc1");
        }

        [TestMethod()]
        public void Cyclobutadiene_kekule()
        {
            RoundTrip("C1=CC=C1");
        }

        [TestMethod()]
        public void Biphenyl()
        {
            RoundTrip("c1ccccc1-c2ccccc2");
        }

        // More about Hydrogen

        [TestMethod()]
        public void Methane_implicit()
        {
            RoundTrip("C");
        }

        [TestMethod()]
        public void Methane_atomProperty()
        {
            RoundTrip("[CH4]");
        }

        [TestMethod()]
        public void Methane_explicit()
        {
            RoundTrip("[H]C([H])([H])[H]");
        }

        [TestMethod()]
        public void Methane_some_explicit()
        {
            RoundTrip("[H][CH2][H]");
        }

        [TestMethod()]
        public void Deuteroethane()
        {
            RoundTrip("[2H][CH2]C");
        }

        // Disconnected

        [TestMethod()]
        public void SodiumChloride()
        {
            RoundTrip("[Na+].[Cl-]");
        }

        [TestMethod()]
        public void Phenol_and_2_amino_ethanol()
        {
            RoundTrip("Oc1ccccc1.NCCO");
        }

        [TestMethod()]
        public void DiammoniumThiosulfate()
        {
            RoundTrip("[NH4+].[NH4+].[O-]S(=O)(=O)[S-]");
        }

        [TestMethod()]
        public void Phenol_2_amino_ethanol_1()
        {
            RoundTrip("c1cc(O.NCCO)ccc1",
                  "c1cc(O)ccc1.NCCO"); // non-DFS input
        }

        [TestMethod()]
        public void Phenol_2_amino_ethanol_2()
        {
            RoundTrip("Oc1cc(.NCCO)ccc1",
                      "Oc1ccccc1.NCCO"); // non-DFS input
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Dot_ring_bond()
        {
            Graph.FromSmiles("C.1CCCCC.1");
        }

        [TestMethod()]
        public void Ethane_using_dot()
        {
            RoundTrip("C1.C1", "CC");
        }

        [TestMethod()]
        public void Test_1_bromo_2_3_dichlorobenzene()
        {
            RoundTrip("c1c2c3c4cc1.Br2.Cl3.Cl4",
                  "c1c(c(c(cc1)Cl)Cl)Br"); // non-DFS
        }

        // Stereo chemistry

        [TestMethod()]
        public void Tetrahedral_anticlockwise()
        {
            RoundTrip("N[C@](Br)(O)C");
        }

        [TestMethod()]
        public void Tetrahedral_clockwise()
        {
            RoundTrip("N[C@@](Br)(O)C");
        }

        [TestMethod()]
        public void Tetrahedral_equivalent()
        {
            // we can show all these SMILES are equivalent if we change the Order
            // of the vertices
            RoundTrip("N[C@](Br)(O)C", new int[] { 3, 1, 0, 2, 4 }, "Br[C@](O)(N)C");
            RoundTrip("Br[C@](O)(N)C", new int[] { 2, 1, 0, 4, 3 }, "O[C@](Br)(C)N");
            RoundTrip("O[C@](Br)(C)N", new int[] { 3, 1, 0, 2, 4 }, "Br[C@](C)(O)N");
            RoundTrip("Br[C@](C)(O)N", new int[] { 2, 1, 0, 4, 3 }, "C[C@](Br)(N)O");
            RoundTrip("C[C@](Br)(N)O", new int[] { 3, 1, 0, 2, 4 }, "Br[C@](N)(C)O");
            RoundTrip("Br[C@](N)(C)O", new int[] { 2, 1, 4, 0, 3 }, "C[C@@](Br)(O)N");
            RoundTrip("C[C@@](Br)(O)N", new int[] { 4, 1, 0, 3, 2 }, "Br[C@@](N)(O)C");
            RoundTrip("Br[C@@](N)(O)C", new int[] { 2, 0, 4, 3, 1 }, "[C@@](C)(Br)(O)N");
            RoundTrip("[C@@](C)(Br)(O)N", new int[] { 0, 4, 1, 3, 2 }, "[C@@](Br)(N)(O)C");
        }

        [TestMethod()]
        public void Tetrahedral_equivalent_2()
        {
            RoundTrip("FC1C[C@](Br)(Cl)CCC1",
                      new int[] { 7, 6, 8, 0, 1, 2, 5, 4, 3 },
                      "[C@]1(Br)(Cl)CCCC(F)C1");
        }

        [TestMethod()]
        public void Tetrahedral_3_neighbors()
        {
            RoundTrip("N[C@H](O)C");
        }

        [TestMethod()]
        public void Trans_difluoroethane_1()
        {
            RoundTrip("F/C=C/F");
        }

        [TestMethod()]
        public void Trans_difluoroethane_2()
        {
            RoundTrip("F\\C=C\\F");
        }

        [TestMethod()]
        public void Trans_difluoroethane_3()
        {
            RoundTrip("C(\\F)=C/F");
            RoundTrip("C(\\\\F)=C/F", new int[] { 1, 0, 2, 3 }, "F/C=C/F");

        }

        [TestMethod()]
        public void Cis_difluoroethane_1()
        {
            RoundTrip("F/C=C\\F");
        }

        [TestMethod()]
        public void Cis_difluoroethane_2()
        {
            RoundTrip("F\\C=C/F");
        }

        [TestMethod()]
        public void Cis_difluoroethane_3()
        {
            RoundTrip("C(/F)=C/F");
            RoundTrip("C(/F)=C/F", new int[] { 1, 0, 2, 3 }, "F\\C=C/F");
        }

        // C/C(\F)=C/F - see AddUpDownBonds

        [TestMethod()]
        public void Trans_difluoro_implied()
        {
            RoundTrip("F/C(CC)=C/F");
        }

        [TestMethod()]
        public void Extended_cistrans_1()
        {
            RoundTrip("F/C=C=C=C/F");
        }

        [TestMethod()]
        public void Extended_cistrans_2()
        {
            RoundTrip("F\\C=C=C=C\\F");
        }

        // other stereo not yet supported

        [TestMethod()]
        public void Conjugated()
        {
            RoundTrip("F/C=C/C/C=C\\C");
        }

        [TestMethod()]
        public void Conjugated_partial()
        {
            RoundTrip("F/C=C/CC=CC");
        }

        [TestMethod()]
        public void Partial_tetrahedral()
        {
            RoundTrip("N1[C@H](Cl)[C@@H](Cl)C(Cl)CC1");
        }

        // Parsing termination
        [TestMethod()]
        public void Terminate_on_space()
        {
            RoundTrip("CCO ethanol", "CCO");
        }

        [TestMethod()]
        public void Terminate_on_tab()
        {
            RoundTrip("CCO\tethanol", "CCO");
        }

        [TestMethod()]
        public void Terminate_on_newline()
        {
            RoundTrip("CCO\nethanol", "CCO");
        }

        [TestMethod()]
        public void Terminate_on_carriage_return()
        {
            RoundTrip("CCO\r\nethanol", "CCO");
        }

        // Normalisation - part of the functions but we can test we can read/write them

        [TestMethod()]
        public void Ethanol_norm_1()
        {
            RoundTrip("CCO");
        }

        [TestMethod()]
        public void Ethanol_norm_2()
        {
            RoundTrip("OCC");
        }

        [TestMethod()]
        public void Ethanol_norm_3()
        {
            RoundTrip("C(O)C");
        }

        [TestMethod()]
        public void Ethanol_norm_4()
        {
            RoundTrip("[CH3][CH2][OH]");
        }

        [TestMethod()]
        public void Ethanol_norm_5()
        {
            RoundTrip("[H][C]([H])([H])C([H])([H])[O][H]");
        }

        // Standard form - again only that we can read/write is tested here

        [TestMethod()]
        public void Ethane_right()
        {
            RoundTrip("CC");
        }

        [TestMethod()]
        public void Ethane_wrong()
        {
            RoundTrip("[CH3][CH3]");
        }

        [TestMethod()]
        public void Leave_off_digit_on_single_charge()
        {
            RoundTrip("[CH3-1]", "[CH3-]");
        }

        [TestMethod()]
        public void Leave_off_digit_on_single_hydrogen()
        {
            RoundTrip("C[13CH1](C)C", "C[13CH](C)C");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Write_atom_properties_in_Order_1()
        {
            Parser.GetStrict("[C-H3]");    // this is accepted by daylight but doesn't match OpenSMILES grammar
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Write_atom_properties_in_Order_2()
        {
            Parser.GetStrict("C[CH@](Br)Cl");  // this is accepted by daylight but doesn't match OpenSMILES grammar
        }

        [TestMethod()]
        public void Methanide_wrong()
        {
            RoundTrip("[H][C-]([H])[H]");
        }

        [TestMethod()]
        public void Methanide_right()
        {
            RoundTrip("[CH3-]");
        }

        // Bonds

        [TestMethod()]
        public void Ethane_bonds_wrong()
        {
            RoundTrip("C-C");
        }

        [TestMethod()]
        public void Ethane_bonds_right()
        {
            RoundTrip("CC");
        }

        [TestMethod()]
        public void Benzene_bonds_wrong()
        {
            RoundTrip("c:1:c:c:c:c:c:1",
                      "c:1:c:c:c:c:c1");
        }

        [TestMethod()]
        public void Benzene_bonds_right()
        {
            RoundTrip("c1ccccc1");
        }

        [TestMethod()]
        public void Biphenyl_wrong()
        {
            RoundTrip("c1ccccc1c2ccccc2");
        }

        [TestMethod()]
        public void Biphenyl_right()
        {
            RoundTrip("c1ccccc1-c2ccccc2");
        }

        // Cycles

        [TestMethod()]
        public void Rnum_reuse_1()
        {
            RoundTrip("c1ccccc1C1CCCC1",
                      "c1ccccc1C2CCCC2");
        }

        [TestMethod()]
        public void Rnum_reuse_2()
        {
            RoundTrip("c0ccccc0C1CCCC1",
                      "c1ccccc1C2CCCC2");
        }

        // avoid ring closures on double bond - nice idea but not valid to implement
        [TestMethod()]
        public void Avoid_ring_closures_on_double_bond()
        {
            RoundTrip("CC=1CCCCC=1",
                      "CC=1CCCCC1");
        }

        // avoid closing/openning 2 rings on a single atom - yeah good luck :-)
        [TestMethod()]
        public void Avoid_starting_ringsystem_on_two_digits()
        {
            RoundTrip("C12(CCCCC1)CCCCC2",
                  "C12(CCCCC1)CCCCC2");
        }

        [TestMethod()]
        public void Use_simple_digits()
        {
            RoundTrip("C%01CCCCC%01",
                      "C1CCCCC1");
        }

        // starting branches

        [TestMethod()]
        public void Start_on_terminal_wrong()
        {
            RoundTrip("c1cc(CO)ccc1");
        }

        [TestMethod()]
        public void Start_on_terminal_right()
        {
            RoundTrip("OCc1ccccc1");
        }

        [TestMethod()]
        public void Short_branches_wrong()
        {
            RoundTrip("CC(CCCCCC)C");
        }

        [TestMethod()]
        public void Short_branched_right()
        {
            RoundTrip("CC(C)CCCCCC");
        }

        [TestMethod()]
        public void Start_on_hetroatom_wrong()
        {
            RoundTrip("CCCO");
        }

        [TestMethod()]
        public void Start_on_hetroatom_right()
        {
            RoundTrip("OCCC");
        }

        [TestMethod()]
        public void Only_use_dot_for_disconnected()
        {
            RoundTrip("C1.C1", "CC");
        }

        [TestMethod()]
        public void Write_IsAromatic_form_wrong()
        {
            RoundTrip("C1=CC=CC=C1");
        }

        [TestMethod()]
        public void Write_IsAromatic_form_right()
        {
            RoundTrip("c1ccccc1");
        }

        [TestMethod()]
        public void Remove_chiral_markings_wrong()
        {
            RoundTrip("Br[C@H](Br)C");
        }

        [TestMethod()]
        public void Remove_chiral_markings_right()
        {
            RoundTrip("BrC(Br)C");
        }

        [TestMethod()]
        public void Remove_directional_markings_wrong()
        {
            RoundTrip("F/C(/F)=C/F");
        }

        [TestMethod()]
        public void Remove_directional_markings_right()
        {
            RoundTrip("FC(F)=CF");
        }

        // Non-standard forms

        [TestMethod()]
        public void Extra_paratheses_1()
        {
            RoundTrip("C((C))O", "C(C)O");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Extra_paratheses_2()
        {
            Graph.FromSmiles("(N1CCCC1)");
        }

        [TestMethod()]
        public void Misplaced_dots_1()
        {
            RoundTrip(".CCO", "CCO");
        }

        [TestMethod()]
        public void Misplaced_dots_2()
        {
            RoundTrip("CCO.", "CCO");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Mismatch_ring()
        {
            Parser.Parse("C1CCC");
        }

        // invalid cis/trans - semantics
        // conflict cis/trans - semantics (see. functions)

        [TestMethod()]
        public void D_for_h2()
        {
            RoundTrip("D[CH3]",
                      "[2H][CH3]");
        }

        [TestMethod()]
        public void T_for_h3()
        {
            RoundTrip("T[CH3]",
                      "[3H][CH3]");
        }

        // lowercase for Sp2 - stupid :)

        // Extensions

        [TestMethod()]
        public void Nope_not_illegal()
        {
            RoundTrip("C/1=C/C=C\\C=C/C=C\\1",
                      "C/1=C/C=C\\C=C/C=C1");
        }

        [TestMethod()]
        public void Atom_based_db_stereo_trans_1()
        {
            RoundTrip("F[C@@H]=[C@H]F");
        }

        [TestMethod()]
        public void Atom_based_db_stereo_trans_2()
        {
            RoundTrip("F[C@H]=[C@@H]F");
        }

        [TestMethod()]
        public void Atom_based_db_stereo_cis_1()
        {
            RoundTrip("F[C@H]=[C@H]F");
        }

        [TestMethod()]
        public void Atom_based_db_stereo_cis_2()
        {
            RoundTrip("F[C@@H]=[C@@H]F");
        }

        [TestMethod()]
        public void Cyclooctatetraene()
        {
            RoundTrip("[C@H]1=[C@@H][C@@H]=[C@@H][C@@H]=[C@@H][C@@H]=[C@@H]1");
        }

        static void RoundTrip(string smi)
        {
            RoundTrip(smi, smi);
        }

        static void RoundTrip(string smi, int[] p, string exp)
        {
            try
            {
                Assert.AreEqual(exp, Generator.Generate(Parser.Parse(smi).Permute(p)));
            }
            catch (InvalidSmilesException e)
            {
                Assert.Fail(e.Message);
            }
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
