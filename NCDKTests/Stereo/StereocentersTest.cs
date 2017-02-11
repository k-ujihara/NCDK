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
    /**
     * Verifies the types of atoms accepted as exhibiting stereo chemistry.
     *
     * @author John May
     * @cdk.module test-standard
     */
    [TestClass()]
    public class StereocentersTest
    {

        [TestMethod()]
        public void boron_v4_anion()
        {

            CreateTetrahedral("[BH-](C)(N)O");
            CreateTetrahedral("[B-](C)(N)(O)CC");

            none("[BH2-](C)(C)");
            none("[BH3-](C)");
            none("[BH4-]");

            none("[B-](=C)(=C)(=C)(=C)"); // abnormal valence
            none("[B-](=C)(=C)");
            none("[B-](=C)(C)(C)(C)");

            none("B(C)");
            none("B(C)(N)");
            none("B(C)(N)O");
            none("B(C)(N)(O)CC"); // abnormal valence
        }

        [TestMethod()]
        public void Carbon_v4_neutral()
        {

            // accept Sp3 Carbons with < 2 hydrogens
            CreateTetrahedral("C(C)(N)(O)");
            CreateTetrahedral("C(C)(N)(O)CC");

            // reject when > 1 hydrogen or < 4 neighbors
            none("C");
            none("C(C)");
            none("C(C)(N)");
            none("C(=C)(C)N");
            Bicoordinate("C(=CC)=CC");
            none("C(=C)(=C)(=C)=C"); // nb abnormal valence
            none("C#N");
        }

        [TestMethod()]
        public void Carbon_cation()
        {
            none("[C+](C)(N)(O)");
            none("[C+](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Carbon_anion()
        {
            none("[C-](C)(N)(O)");
            none("[C-](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Silicon_v4_neutral()
        {
            CreateTetrahedral("[SiH](C)(N)(O)");
            CreateTetrahedral("[Si](C)(N)(O)CC");

            none("[Si](=C)(C)C");
            none("[Si](=C)=C");
            none("[Si](#C)C");
        }

        [TestMethod()]
        public void Silicon_cation()
        {
            none("[Si+](C)(N)(O)");
            none("[Si+](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void Silicon_anion()
        {
            none("[Si-](C)(N)(O)");
            none("[Si-](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void germanium_v4_neutral()
        {
            CreateTetrahedral("[GeH](C)(N)(O)");
            CreateTetrahedral("[Ge](C)(N)(O)CC");

            none("[Ge](=C)(C)C");
            none("[Ge](=C)=C");
            none("[Ge](#C)C");
        }

        [TestMethod()]
        public void germanium_cation()
        {
            none("[Ge+](C)(N)(O)");
            none("[Ge+](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void germanium_anion()
        {
            none("[Ge-](C)(N)(O)");
            none("[Ge-](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void tin_v4_neutral()
        {
            CreateTetrahedral("[SnH](C)(N)(O)");
            CreateTetrahedral("[Sn](C)(N)(O)CC");

            none("[Sn](=C)(C)C");
            none("[Sn](=C)=C");
            none("[Sn](#C)C");
        }

        [TestMethod()]
        public void tin_cation()
        {
            none("[Sn+](C)(N)(O)");
            none("[Sn+](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void tin_anion()
        {
            none("[Sn-](C)(N)(O)");
            none("[Sn-](C)(N)(O)CC"); // nb abnormal valence
        }

        [TestMethod()]
        public void nitrogen_v3_neutral()
        {

            // nitrogen inversion -> reject
            none("N");
            none("N(C)(N)(O)");
            none("N(=C)(C)");
        }

        [TestMethod()]
        public void nitrogen_v3_neutral_in_small_ring()
        {
            CreateTetrahedral("N(C)(C1)O1");
            CreateTetrahedral("N(C)(C1)C1C");
        }

        [TestMethod()]
        public void nitrogen_v3_neutral_in_larger_ring()
        {
            none("N(C)(C1)CCCC1"); // n.b. equivalence checked later
            none("N(C)(C1)CCCC1C");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void nitrogen_v3_neutral_reject_H()
        {
            none("N(C1)C1"); // n.b. equivalence checked later
            none("N(C1)C1C");
        }

        [TestMethod()]
        public void nitrogen_v4_cation()
        {
            CreateTetrahedral("[N+](C)(N)(O)CC");
            none("[N+](=C)(C)C");
            none("[N+](=C)=C");
            none("[N+](#C)C");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void nitrogen_v4_cation_reject_h()
        {
            none("[NH+](=C)(C)C");
            none("[NH2+](C)C");
            none("[NH3+]C");
            none("[NH4+]");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void nitrogen_v4_cation_reject_h_on_terminal()
        {
            none("[N+](N)([NH])(C)CC");
            none("[N+](O)([O])(C)CC");
            none("[N+](S)([S])(C)CC");
            none("[N+]([SeH])([Se])(C)C");
            none("[N+]([TeH])([Te])(C)C");
        }

        [TestMethod()]
        public void nitrogen_v5_neutral()
        {
            CreateTetrahedral("N(=C)(C)(N)O");
            none("N(=C)(=C)C");
            none("N(#C)=C");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void nitrogen_v5_neutral_reject_h()
        {
            none("N(=C)(C)(C)");
            none("N(=C)(C)");
            none("N(=C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void nitrogen_v5_neutral_reject_h_on_terminal()
        {
            none("N(N)(=N)(C)CC");
            none("N(O)(=O)(C)CC");
            none("N(S)(=S)(C)CC");
            none("N([SeH])(=[Se])(C)C");
            none("N([TeH])(=[Te])(C)C");
        }

        // n.b. undocumented by the InChI tech manual
        [TestMethod()]
        public void Phosphorus_v3_neutral()
        {
            CreateTetrahedral("P(C)(N)(O)");
            none("P(=C)(C)");
            none("P(#C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void Phosphorus_v3_neutral_reject_H()
        {
            none("P(C)(C)");
            none("P(C)");
            none("P");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void Phosphorus_v3_neutral_reject_h_on_terminal()
        {
            none("P(N)([NH4])C");
            none("P(S)([SH4])C");

        }

        [TestMethod()]
        public void Phosphorus_v4_cation()
        {
            CreateTetrahedral("[P+](C)(N)(O)CC");
            none("[P+](=C)(C)C");
            none("[P+](=C)=C");
            none("[P+](#C)C");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor. Since InChI software v.
         * 1.02-standard (2009), phosphines and arsines are always treated as
         * stereogenic even with H atom neighbors
         *
         * @
         */
        [TestMethod()]
        public void Phosphorus_v4_cation_Accept_h()
        {
            CreateTetrahedral("[PH+](C)(N)O");
            none("[PH2+](C)C");
            none("[PH3+]C");
            none("[PH4+]");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void Phosphorus_v4_cation_reject_h_on_terminal()
        {
            none("[P+](N)([N])(C)CC");
            none("[P+](O)([O])(C)CC");
            none("[P+](S)([S])(C)CC");
            none("[P+]([SeH])([Se])(C)CC");
            none("[P+]([TeH])([Te])(C)CC");
        }

        [TestMethod()]
        public void Phosphorus_v5_neutral()
        {
            CreateTetrahedral("P(=C)(C)(N)O");
            none("P(=C)(=C)C");
            none("P(#C)=C");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void Phosphorus_v5_neutral_reject_h()
        {
            none("P(=C)(C)(C)");
            none("P(=C)(C)");
            none("P(=C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void Phosphorus_v5_neutral_reject_h_on_terminal()
        {
            none("P(=N)(N)(C)CC");
            none("P(=O)(O)(C)CC");
            none("P(=S)(S)(C)CC");
            none("P(=[Se])([SeH])(C)C");
            none("P(=[Te])([TeH])(C)C");
        }

        [TestMethod()]
        public void Arsenic_v4_cation()
        {
            CreateTetrahedral("[As+](C)(N)(O)CC");
            none("[As+](=C)(C)(C)");
            none("[As+](=C)(=C)");
            none("[As+](#C)(C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor. Since InChI software v.
         * 1.02-standard (2009), phosphines and arsines are always treated as
         * stereogenic even with H atom neighbors
         *
         * @
         */
        [TestMethod()]
        public void Arsenic_v4_cation_Accept_h()
        {
            CreateTetrahedral("[AsH+](C)(N)O");
            none("[AsH2+](C)C");
            none("[AsH3+]C");
            none("[AsH4+]");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void Arsenic_v4_cation_reject_h_on_terminal()
        {
            none("[As+](N)([N])(C)CC");
            none("[As+](O)([O])(C)CC");
            none("[As+](S)([S])(C)CC");
            none("[As+]([SeH])([Se])(C)CC");
            none("[As+]([TeH])([Te])(C)CC");
        }

        [TestMethod()]
        public void sulphur_4v_neutral()
        {
            CreateTetrahedral("S(=O)(C)CC");
            none("S(C)(N)(O)CC");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void sulphur_4v_neutral_reject_h()
        {
            none("S(=O)(C)");
            none("S(=O)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void sulphur_4v_neutral_reject_h_on_terminal()
        {
            none("S(=N)(N)C");
            none("S(=O)(O)C");
            none("S(=S)(S)C");
            none("S(=[Se])([SeH])C");
            none("S(=[Te])([TeH])C");

            CreateTetrahedral("S(=O)(S)N");
        }

        [TestMethod()]
        public void sulphur_3v_cation()
        {
            CreateTetrahedral("[S+](C)(N)(O)");
            none("[S+](=C)(C)");
        }

        [TestMethod()]
        public void sulphur_1v_anion()
        {
            none("[S-](C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void sulphur_3v_cation_reject_h()
        {
            none("[SH+](C)(C)");
            none("[SH2+](C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void sulphur_3v_cation_reject_h_on_terminal()
        {
            none("[S+](N)([N])(C)");
            none("[S+](O)([O])(C)");
            none("[S+]([SeH])([Se])(C)");
            none("[S+]([TeH])([Te])(C)");

            CreateTetrahedral("[S+](O)(OC)(C)");
            CreateTetrahedral("[S+](OC)(OC)(C)");
        }

        [TestMethod()]
        public void sulphur_6v_neutral()
        {
            CreateTetrahedral("S(=C)(=CC)(C)(CC)");
            none("S(=C)(C)(CC)(CCC)(CCCC)");
            none("S(C)(C)(CC)(CCCC)(CCCC)(CCCCC)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void sulphur_6v_neutral_reject_h()
        {
            none("S(=C)(=C)(C)");
            none("S(=C)(=C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void sulphur_6v_neutral_reject_h_on_terminal()
        {
            none("S(=N)(=C)(N)(C)");
            none("S(=O)(=C)(O)(C)");
            none("S(=S)(=C)(S)(C)");
            none("S(=[Se])(=C)([SeH])(C)");
            none("S(=[Te])(=C)([TeH])(C)");

            CreateTetrahedral("S(=O)(=N)(S)(C)");
        }

        [TestMethod()]
        public void sulphur_5v_cation()
        {
            CreateTetrahedral("[S+](=C)(N)(O)(C)");
            none("[S+](C)(C)(C)(C)(C)");
        }

        [TestMethod()]
        public void sulphur_3v_anion()
        {
            none("[S-](C)(C)(C)");
            none("[S-](=C)(C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void sulphur_5v_cation_reject_h()
        {
            none("[SH+](=C)(CC)(CCC)");
            none("[SH2+](=C)(C)");
            none("[SH3+](=C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void sulphur_5v_cation_reject_h_on_terminal()
        {
            none("[S+](=N)(N)(C)(CC)");
            none("[S+](=O)(O)(C)(CC)");
            none("[S+](=[Se])([SeH])(C)(CC)");
            none("[S+](=[Te])([TeH])(C)(CC)");

            CreateTetrahedral("[S+](=O)(N)(C)(CC)");
            CreateTetrahedral("[S+](=O)(N)(S)(CC)");
        }

        [TestMethod()]
        public void selenium_4v_neutral()
        {
            CreateTetrahedral("[Se](=O)(C)(CC)");
            none("[Se](C)(CC)(CCC)(CCCC)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void selenium_4v_neutral_reject_h()
        {
            none("[SeH](=O)(C)");
            none("[SeH2](=O)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void selenium_4v_neutral_reject_h_on_terminal()
        {
            none("[Se](=N)(N)C");
            none("[Se](=O)(O)C");
            none("[Se](=S)(S)C");
            none("[Se](=[Se])([SeH])C");
            none("[Se](=[Te])([TeH])C");

            CreateTetrahedral("[Se](=O)(S)N");
        }

        [TestMethod()]
        public void selenium_3v_cation()
        {
            CreateTetrahedral("[Se+](C)(CC)(CCC)");
            none("[Se+](=C)(C)");
        }

        [TestMethod()]
        public void selenium_1v_anion()
        {
            none("[Se-](C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void selenium_3v_cation_reject_h()
        {
            none("[SeH+](C)(C)");
            none("[SeH2+](C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void selenium_3v_cation_reject_h_on_terminal()
        {
            none("[Se+](N)(=N)(C)C");
            none("[Se+](O)(=O)(C)C");
            none("[Se+](O)(=O)(C)C");
            none("[Se+]([SeH])(=[Se])(C)C");
            none("[Se+]([TeH])(=[Te])(C)C");

            CreateTetrahedral("[Se+](O)(=N)([SeH])C");
            CreateTetrahedral("[Se+](O)(=N)(C)CC");
        }

        [TestMethod()]
        public void selenium_6v_neutral()
        {
            CreateTetrahedral("[Se](=C)(=CC)(C)(CC)");
            none("[Se](=C)(C)(CC)(CCC)(CCCC)");
            none("[Se](C)(C)(CC)(CCC)(CCCC)(CCCC)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void selenium_6v_neutral_reject_h()
        {
            none("[SeH](=C)(=C)(C)");
            none("[SeH2](=C)(=C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void selenium_6v_neutral_reject_h_on_terminal()
        {
            none("[Se](=N)(=N)(N)(C)");
            none("[Se](=O)(=O)(O)(C)");
            none("[Se](=S)(=S)(S)(C)");
            none("[Se](=[Se])(=[Se])([SeH])(C)");
            none("[Se](=[Te])(=[Te])([TeH])(C)");

            CreateTetrahedral("[Se](=O)(=N)(S)(C)");
        }

        [TestMethod()]
        public void selenium_5v_cation()
        {
            CreateTetrahedral("[Se+](=C)(CC)(CCC)(CCCC)");
            none("[Se+](C)(CC)(CCC)(CCCC)(CCCCC)");
        }

        [TestMethod()]
        public void selenium_3v_anion()
        {
            none("[Se-](C)(C)(C)");
            none("[Se-](=C)(C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (a) A terminal H atom neighbor
         *
         * @
         */
        [TestMethod()]
        public void selenium_5v_cation_reject_h()
        {
            none("[SeH+](=C)(C)(CC)");
            none("[SeH2+](=C)(C)");
            none("[SeH3+](=C)");
        }

        /**
         * An atom or positive ion N, P, As, S, or Se is not treated as stereogenic
         * if it has - (b) At least two terminal neighbors, XHm and XHn, (n+m>0)
         * connected by any kind of bond, where X is O, S, Se, Te, or N.
         *
         * @
         */
        [TestMethod()]
        public void selenium_5v_cation_reject_h_on_terminal()
        {
            none("[Se+](=N)(N)(C)(CC)");
            none("[Se+](=O)(O)(C)(CC)");
            none("[Se+](=[Se])([SeH])(C)(CC)");
            none("[Se+](=[Te])([TeH])(C)(CC)");

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
            none("[CH2](=CC)");
            Bicoordinate("C(=C)(=CC)");
            none("C(#CC)C");
        }

        [TestMethod()]
        public void Silicon_neutral_geometric()
        {
            Geometric("[SiH](=[SiH]C)C");
            Geometric("[Si]([H])(=[SiH]C)C");
            none("[Si](=C)(=CC)");
            none("[Si](#CC)C");
        }

        [TestMethod()]
        public void germanium_neutral_geometric()
        {
            Geometric("[GeH](=[GeH]C)C");
            Geometric("[Ge]([H])(=[GeH]C)C");
            none("[Ge](=C)(=CC)");
            none("[Ge](#CC)C");
        }

        [TestMethod()]
        public void nitrogen_neutral_geometric()
        {
            Geometric("N(=NC)C");
            none("N(=NC)");
            none("N(=N)C");
            none("N(=N)");
        }

        [TestMethod()]
        public void nitrogen_cation_geometric()
        {
            Geometric("[NH+](=[NH+]C)C");
            Geometric("[N+]([H])(=[NH+]C)C");
            none("[NH2+](=[NH+]C)C");
        }

        // assert the first atom of the SMILES is accepted as a tetrahedral center
        void CreateTetrahedral(string smi)
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            Test(sp.ParseSmiles(smi), Stereocenters.Type.Tetracoordinate, smi + " was not accepted");
        }

        // assert the first atom of the SMILES is accepted as a geometric center
        void Geometric(string smi)
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            Test(sp.ParseSmiles(smi), Stereocenters.Type.Tricoordinate, smi + " was not accepted");
        }

        // assert the first atom of the SMILES is accepted as a bicoordinate center
        void Bicoordinate(string smi)
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            Test(sp.ParseSmiles(smi), Stereocenters.Type.Bicoordinate, smi + " was not accepted");
        }

        // assert the first atom of the SMILES is non stereogenic
        void none(string smi)
        {
            SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            Test(sp.ParseSmiles(smi), Stereocenters.Type.None, smi + " was not rejected");
        }

        // check if the first atom of the container is accepted
        void Test(IAtomContainer container, Stereocenters.Type type, string mesg)
        {
            Assert.AreEqual(Stereocenters.Of(container).ElementType(0), type, mesg);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(container);
            Assert.AreEqual(Stereocenters.Of(container).ElementType(0), type, mesg + " (unsupressed hydrogens)");
        }
    }
}
