/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.Stereo
{
    /// <summary>
    /// Verifies the types of atoms accepted as exhibiting stereo chemistry.
    /// </summary>
    // @author John May
    // @cdk.module test-standard
    [TestClass()]
    public class StereocentersTest
    {
        [TestMethod()]
        public void Boron_v4_anion()
        {
            CreateTetrahedral("[BH-](C)(N)O");
            CreateTetrahedral("[B-](C)(N)(O)CC");

            None("[BH2-](C)(C)");
            None("[BH3-](C)");
            None("[BH4-]");

            None("[B-](=C)(=C)(=C)(=C)"); // abnormal valence
            None("[B-](=C)(=C)");
            None("[B-](=C)(C)(C)(C)");

            None("B(C)");
            None("B(C)(N)");
            None("B(C)(N)O");
            None("B(C)(N)(O)CC"); // abnormal valence
        }

        [TestMethod()]
        public void Carbon_v4_neutral()
        {
            // accept Sp3 Carbons with < 2 hydrogens
            CreateTetrahedral("C(C)(N)(O)");
            CreateTetrahedral("C(C)(N)(O)CC");

            // reject when > 1 hydrogen or < 4 neighbors
            None("C");
            None("C(C)");
            None("C(C)(N)");
            None("C(=C)(C)N");
            Bicoordinate("C(=CC)=CC");
            None("C(=C)(=C)(=C)=C"); // nb abnormal valence
            None("C#N");
        }

        [TestMethod()]
        public void Carbon_cation()
        {
            None("[C+](C)(N)(O)");
            None("[C+](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Carbon_anion()
        {
            None("[C-](C)(N)(O)");
            None("[C-](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Silicon_v4_neutral()
        {
            CreateTetrahedral("[SiH](C)(N)(O)");
            CreateTetrahedral("[Si](C)(N)(O)CC");

            None("[Si](=C)(C)C");
            None("[Si](=C)=C");
            None("[Si](#C)C");
        }

        [TestMethod()]
        public void Silicon_cation()
        {
            None("[Si+](C)(N)(O)");
            None("[Si+](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Silicon_anion()
        {
            None("[Si-](C)(N)(O)");
            None("[Si-](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Germanium_v4_neutral()
        {
            CreateTetrahedral("[GeH](C)(N)(O)");
            CreateTetrahedral("[Ge](C)(N)(O)CC");

            None("[Ge](=C)(C)C");
            None("[Ge](=C)=C");
            None("[Ge](#C)C");
        }

        [TestMethod()]
        public void Germanium_cation()
        {
            None("[Ge+](C)(N)(O)");
            None("[Ge+](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Germanium_anion()
        {
            None("[Ge-](C)(N)(O)");
            None("[Ge-](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Tin_v4_neutral()
        {
            CreateTetrahedral("[SnH](C)(N)(O)");
            CreateTetrahedral("[Sn](C)(N)(O)CC");

            None("[Sn](=C)(C)C");
            None("[Sn](=C)=C");
            None("[Sn](#C)C");
        }

        [TestMethod()]
        public void Tin_cation()
        {
            None("[Sn+](C)(N)(O)");
            None("[Sn+](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Tin_anion()
        {
            None("[Sn-](C)(N)(O)");
            None("[Sn-](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Nitrogen_v3_neutral()
        {
            // nitrogen inversion -> reject
            None("N");
            None("N(C)(N)(O)");
            None("N(=C)(C)");
        }

        [TestMethod()]
        public void Nitrogen_v3_neutral_in_small_ring()
        {
            CreateTetrahedral("N(C)(C1)O1");
            CreateTetrahedral("N(C)(C1)C1C");
        }

        [TestMethod()]
        public void Nitrogen_v3_neutral_in_larger_ring()
        {
            None("N(C)(C1)CCCC1"); // n.b. equivalence checked later
            None("N(C)(C1)CCCC1C");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Nitrogen_v3_neutral_reject_H()
        {
            None("N(C1)C1"); // n.b. equivalence checked later
            None("N(C1)C1C");
        }

        [TestMethod()]
        public void Nitrogen_v4_cation()
        {
            CreateTetrahedral("[N+](C)(N)(O)CC");
            None("[N+](=C)(C)C");
            None("[N+](=C)=C");
            None("[N+](#C)C");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Nitrogen_v4_cation_reject_h()
        {
            None("[NH+](=C)(C)C");
            None("[NH2+](C)C");
            None("[NH3+]C");
            None("[NH4+]");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Nitrogen_v4_cation_reject_h_on_terminal()
        {
            None("[N+](N)([NH])(C)CC");
            None("[N+](O)([O])(C)CC");
            None("[N+](S)([S])(C)CC");
            None("[N+]([SeH])([Se])(C)C");
            None("[N+]([TeH])([Te])(C)C");
        }

        [TestMethod()]
        public void Nitrogen_v5_neutral()
        {
            CreateTetrahedral("N(=C)(C)(N)O");
            None("N(=C)(=C)C");
            None("N(#C)=C");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Nitrogen_v5_neutral_reject_h()
        {
            None("N(=C)(C)(C)");
            None("N(=C)(C)");
            None("N(=C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Nitrogen_v5_neutral_reject_h_on_terminal()
        {
            None("N(N)(=N)(C)CC");
            None("N(O)(=O)(C)CC");
            None("N(S)(=S)(C)CC");
            None("N([SeH])(=[Se])(C)C");
            None("N([TeH])(=[Te])(C)C");
        }

        // n.b. undocumented by the InChI tech manual
        [TestMethod()]
        public void Phosphorus_v3_neutral()
        {
            CreateTetrahedral("P(C)(N)(O)");
            None("P(=C)(C)");
            None("P(#C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Phosphorus_v3_neutral_reject_H()
        {
            None("P(C)(C)");
            None("P(C)");
            None("P");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Phosphorus_v3_neutral_reject_h_on_terminal()
        {
            None("P(N)([NH4])C");
            None("P(S)([SH4])C");

        }

        [TestMethod()]
        public void Phosphorus_v4_cation()
        {
            CreateTetrahedral("[P+](C)(N)(O)CC");
            None("[P+](=C)(C)C");
            None("[P+](=C)=C");
            None("[P+](#C)C");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor. Since InChI software v.
        /// 1.02-standard (2009), phosphines and arsines are always treated as
        /// stereogenic even with H atom neighbors
        /// </summary>
        [TestMethod()]
        public void Phosphorus_v4_cation_Accept_h()
        {
            CreateTetrahedral("[PH+](C)(N)O");
            None("[PH2+](C)C");
            None("[PH3+]C");
            None("[PH4+]");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Phosphorus_v4_cation_reject_h_on_terminal()
        {
            None("[P+](N)([N])(C)CC");
            None("[P+](O)([O])(C)CC");
            None("[P+](S)([S])(C)CC");
            None("[P+]([SeH])([Se])(C)CC");
            None("[P+]([TeH])([Te])(C)CC");
        }

        [TestMethod()]
        public void Phosphorus_v5_neutral()
        {
            CreateTetrahedral("P(=C)(C)(N)O");
            None("P(=C)(=C)C");
            None("P(#C)=C");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Phosphorus_v5_neutral_reject_h()
        {
            None("P(=C)(C)(C)");
            None("P(=C)(C)");
            None("P(=C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Phosphorus_v5_neutral_reject_h_on_terminal()
        {
            None("P(=N)(N)(C)CC");
            None("P(=O)(O)(C)CC");
            None("P(=S)(S)(C)CC");
            None("P(=[Se])([SeH])(C)C");
            None("P(=[Te])([TeH])(C)C");
        }

        [TestMethod()]
        public void Arsenic_v4_cation()
        {
            CreateTetrahedral("[As+](C)(N)(O)CC");
            None("[As+](=C)(C)(C)");
            None("[As+](=C)(=C)");
            None("[As+](#C)(C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor. Since InChI software v.
        /// 1.02-standard (2009), phosphines and arsines are always treated as
        /// stereogenic even with H atom neighbors
        /// </summary>
        [TestMethod()]
        public void Arsenic_v4_cation_Accept_h()
        {
            CreateTetrahedral("[AsH+](C)(N)O");
            None("[AsH2+](C)C");
            None("[AsH3+]C");
            None("[AsH4+]");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Arsenic_v4_cation_reject_h_on_terminal()
        {
            None("[As+](N)([N])(C)CC");
            None("[As+](O)([O])(C)CC");
            None("[As+](S)([S])(C)CC");
            None("[As+]([SeH])([Se])(C)CC");
            None("[As+]([TeH])([Te])(C)CC");
        }

        [TestMethod()]
        public void Sulphur_4v_neutral()
        {
            CreateTetrahedral("S(=O)(C)CC");
            None("S(C)(N)(O)CC");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Sulphur_4v_neutral_reject_h()
        {
            None("S(=O)(C)");
            None("S(=O)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Sulphur_4v_neutral_reject_h_on_terminal()
        {
            None("S(=N)(N)C");
            None("S(=O)(O)C");
            None("S(=S)(S)C");
            None("S(=[Se])([SeH])C");
            None("S(=[Te])([TeH])C");

            CreateTetrahedral("S(=O)(S)N");
        }

        [TestMethod()]
        public void Sulphur_3v_cation()
        {
            CreateTetrahedral("[S+](C)(N)(O)");
            None("[S+](=C)(C)");
        }

        [TestMethod()]
        public void Sulphur_1v_anion()
        {
            None("[S-](C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Sulphur_3v_cation_reject_h()
        {
            None("[SH+](C)(C)");
            None("[SH2+](C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Sulphur_3v_cation_reject_h_on_terminal()
        {
            None("[S+](N)([N])(C)");
            None("[S+](O)([O])(C)");
            None("[S+]([SeH])([Se])(C)");
            None("[S+]([TeH])([Te])(C)");

            CreateTetrahedral("[S+](O)(OC)(C)");
            CreateTetrahedral("[S+](OC)(OC)(C)");
        }

        [TestMethod()]
        public void Sulphur_6v_neutral()
        {
            CreateTetrahedral("S(=C)(=CC)(C)(CC)");
            None("S(=C)(C)(CC)(CCC)(CCCC)");
            None("S(C)(C)(CC)(CCCC)(CCCC)(CCCCC)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Sulphur_6v_neutral_reject_h()
        {
            None("S(=C)(=C)(C)");
            None("S(=C)(=C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Sulphur_6v_neutral_reject_h_on_terminal()
        {
            None("S(=N)(=C)(N)(C)");
            None("S(=O)(=C)(O)(C)");
            None("S(=S)(=C)(S)(C)");
            None("S(=[Se])(=C)([SeH])(C)");
            None("S(=[Te])(=C)([TeH])(C)");

            CreateTetrahedral("S(=O)(=N)(S)(C)");
        }

        [TestMethod()]
        public void Sulphur_5v_cation()
        {
            CreateTetrahedral("[S+](=C)(N)(O)(C)");
            None("[S+](C)(C)(C)(C)(C)");
        }

        [TestMethod()]
        public void Sulphur_3v_anion()
        {
            None("[S-](C)(C)(C)");
            None("[S-](=C)(C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Sulphur_5v_cation_reject_h()
        {
            None("[SH+](=C)(CC)(CCC)");
            None("[SH2+](=C)(C)");
            None("[SH3+](=C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Sulphur_5v_cation_reject_h_on_terminal()
        {
            None("[S+](=N)(N)(C)(CC)");
            None("[S+](=O)(O)(C)(CC)");
            None("[S+](=[Se])([SeH])(C)(CC)");
            None("[S+](=[Te])([TeH])(C)(CC)");

            CreateTetrahedral("[S+](=O)(N)(C)(CC)");
            CreateTetrahedral("[S+](=O)(N)(S)(CC)");
        }

        [TestMethod()]
        public void Selenium_4v_neutral()
        {
            CreateTetrahedral("[Se](=O)(C)(CC)");
            None("[Se](C)(CC)(CCC)(CCCC)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Selenium_4v_neutral_reject_h()
        {
            None("[SeH](=O)(C)");
            None("[SeH2](=O)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Selenium_4v_neutral_reject_h_on_terminal()
        {
            None("[Se](=N)(N)C");
            None("[Se](=O)(O)C");
            None("[Se](=S)(S)C");
            None("[Se](=[Se])([SeH])C");
            None("[Se](=[Te])([TeH])C");

            CreateTetrahedral("[Se](=O)(S)N");
        }

        [TestMethod()]
        public void Selenium_3v_cation()
        {
            CreateTetrahedral("[Se+](C)(CC)(CCC)");
            None("[Se+](=C)(C)");
        }

        [TestMethod()]
        public void Selenium_1v_anion()
        {
            None("[Se-](C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Selenium_3v_cation_reject_h()
        {
            None("[SeH+](C)(C)");
            None("[SeH2+](C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Selenium_3v_cation_reject_h_on_terminal()
        {
            None("[Se+](N)(=N)(C)C");
            None("[Se+](O)(=O)(C)C");
            None("[Se+](O)(=O)(C)C");
            None("[Se+]([SeH])(=[Se])(C)C");
            None("[Se+]([TeH])(=[Te])(C)C");

            CreateTetrahedral("[Se+](O)(=N)([SeH])C");
            CreateTetrahedral("[Se+](O)(=N)(C)CC");
        }

        [TestMethod()]
        public void Selenium_6v_neutral()
        {
            CreateTetrahedral("[Se](=C)(=CC)(C)(CC)");
            None("[Se](=C)(C)(CC)(CCC)(CCCC)");
            None("[Se](C)(C)(CC)(CCC)(CCCC)(CCCC)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Selenium_6v_neutral_reject_h()
        {
            None("[SeH](=C)(=C)(C)");
            None("[SeH2](=C)(=C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Selenium_6v_neutral_reject_h_on_terminal()
        {
            None("[Se](=N)(=N)(N)(C)");
            None("[Se](=O)(=O)(O)(C)");
            None("[Se](=S)(=S)(S)(C)");
            None("[Se](=[Se])(=[Se])([SeH])(C)");
            None("[Se](=[Te])(=[Te])([TeH])(C)");

            CreateTetrahedral("[Se](=O)(=N)(S)(C)");
        }

        [TestMethod()]
        public void Selenium_5v_cation()
        {
            CreateTetrahedral("[Se+](=C)(CC)(CCC)(CCCC)");
            None("[Se+](C)(CC)(CCC)(CCCC)(CCCCC)");
        }

        [TestMethod()]
        public void Selenium_3v_anion()
        {
            None("[Se-](C)(C)(C)");
            None("[Se-](=C)(C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (a) A terminal H atom neighbor
        /// </summary>
        [TestMethod()]
        public void Selenium_5v_cation_reject_h()
        {
            None("[SeH+](=C)(C)(CC)");
            None("[SeH2+](=C)(C)");
            None("[SeH3+](=C)");
        }

        /// <summary>
        /// An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
        /// if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
        /// connected by any kind of bond, where X is O, S, Se, Te, or N.
        /// </summary>
        [TestMethod()]
        public void Selenium_5v_cation_reject_h_on_terminal()
        {
            None("[Se+](=N)(N)(C)(CC)");
            None("[Se+](=O)(O)(C)(CC)");
            None("[Se+](=[Se])([SeH])(C)(CC)");
            None("[Se+](=[Te])([TeH])(C)(CC)");

            CreateTetrahedral("[Se+](=O)(N)(C)(CC)");
            CreateTetrahedral("[Se+](=O)(N)(S)(CC)");
        }

        /// <summary>Geometric.</summary>
        [TestMethod()]
        public void Carbon_neutral_geometric()
        {
            Geometric("C(=CC)C");
            Geometric("[CH](=CC)C");
            Geometric("C([H])(=CC)C");
            None("[CH2](=CC)");
            Bicoordinate("C(=C)(=CC)");
            None("C(#CC)C");
        }

        [TestMethod()]
        public void Silicon_neutral_geometric()
        {
            Geometric("[SiH](=[SiH]C)C");
            Geometric("[Si]([H])(=[SiH]C)C");
            None("[Si](=C)(=CC)");
            None("[Si](#CC)C");
        }

        [TestMethod()]
        public void Germanium_neutral_geometric()
        {
            Geometric("[GeH](=[GeH]C)C");
            Geometric("[Ge]([H])(=[GeH]C)C");
            None("[Ge](=C)(=CC)");
            None("[Ge](#CC)C");
        }

        /// <summary>
        /// This one is a bit of an odd bull and changes depending on hydrogen
        /// representation. In most cast it's probably tautomeric. Note that
        /// InChI does allow it: InChI=1S/H2N2/c1-2/h1-2H/b2-1+
        /// </summary>
        [TestMethod()]
        public void Nitrogen_neutral_geometric()
        {
            Test("N(=NC)C", CoordinateType.Tricoordinate, true);
            Test("N(=NC)", CoordinateType.None, false);
            Test("N(=N)C", CoordinateType.None, false);
            Test("N(=N)", CoordinateType.None, false);
            Test("N(=NC)[H]", CoordinateType.Tricoordinate, false);
            Test("N(=N[H])[H]", CoordinateType.Tricoordinate, false);
            Test("N(=N[H])[H]", CoordinateType.Tricoordinate, false);
        }

        [TestMethod()]
        public void Nitrogen_cation_geometric()
        {
            Geometric("[NH+](=[NH+]C)C");
            Geometric("[N+]([H])(=[NH+]C)C");
            None("[NH2+](=[NH+]C)C");
        }

        [TestMethod()]
        public void Bridgehead_nitrogens()
        {
            CreateTetrahedral("N1(CC2)CC2CC1");
            // fused
            None("N1(CCCC2)CCCC12");
            // adjacent to fused (but not fused)
            CreateTetrahedral("N1(c(cccc3)c32)CC2CC1");
        }

        // assert the first atom of the SMILES is accepted as a tetrahedral center
        static void CreateTetrahedral(string smi)
        {
            SmilesParser sp = CDK.SmilesParser;
            Test(sp.ParseSmiles(smi), CoordinateType.Tetracoordinate, smi + " was not accepted", true);
        }

        // assert the first atom of the SMILES is accepted as a geometric center
        static void Geometric(string smi)
        {
            SmilesParser sp = CDK.SmilesParser;
            Test(sp.ParseSmiles(smi), CoordinateType.Tricoordinate, smi + " was not accepted", true);
        }

        // assert the first atom of the SMILES is accepted as a bicoordinate center
        static void Bicoordinate(string smi)
        {
            SmilesParser sp = CDK.SmilesParser;
            Test(sp.ParseSmiles(smi), CoordinateType.Bicoordinate, smi + " was not accepted", true);
        }

        // assert the first atom of the SMILES is non stereogenic
        static void None(string smi)
        {
            SmilesParser sp = CDK.SmilesParser;
            Test(sp.ParseSmiles(smi), CoordinateType.None, smi + " was not rejected", true);
        }

        // check if the first atom of the container is accepted
        static void Test(IAtomContainer container, CoordinateType type, string mesg, bool hnorm)
        {
            Assert.AreEqual(type, Stereocenters.Of(container).ElementType(0), mesg);
            if (hnorm)
            {
                AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
                Assert.AreEqual(type, Stereocenters.Of(container).ElementType(0), mesg + " (unsupressed hydrogens)");
            }
        }

        static void Test(string smi, CoordinateType type, bool hnorm)
        {
            SmilesParser sp = CDK.SmilesParser;
            Test(sp.ParseSmiles(smi), type, smi + " was not accepted", hnorm);
        }
    }
}
