/* Copyright (C) 1997-2007  Chris Pudney
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace NCDK.Renderers.Colors
{
    /// <summary>
    /// Colors atoms using CPK color scheme {@cdk.cite BER2001}. 
    /// </summary>
    // @cdk.module  render
    // @cdk.githash
    // @cdk.keyword atom coloring, CPK
    [Obsolete(nameof(JmolColors) + " provides more comprehensive CPK color pallet")]
    [Serializable]
    public class CPKAtomColors : IAtomColorer
    {
        ////////////
        // CONSTANTS
        ////////////

        // CPK colours.
        private static readonly Color LIGHT_GREY = Color.FromRgb(0xC8, 0xC8, 0xC8);
        private static readonly Color SKY_BLUE = Color.FromRgb(0x8F, 0x8F, 0xFF);
        private static readonly Color RED = Color.FromRgb(0xF0, 0x00, 0x00);
        private static readonly Color Yellow = Color.FromRgb(0xFF, 0xC8, 0x32);
        private static readonly Color WHITE = Color.FromRgb(0xFF, 0xFF, 0xFF);
        private static readonly Color PINK = Color.FromRgb(0xFF, 0xC0, 0xCB);
        private static readonly Color GOLDEN_ROD = Color.FromRgb(0xDA, 0xA5, 0x20);
        private static readonly Color Blue = Color.FromRgb(0x00, 0x00, 0xFF);
        private static readonly Color ORANGE = Color.FromRgb(0xFF, 0xA5, 0x00);
        private static readonly Color DARK_GREY = Color.FromRgb(0x80, 0x80, 0x90);
        private static readonly Color BROWN = Color.FromRgb(0xA5, 0x2A, 0x2A);
        private static readonly Color PURPLE = Color.FromRgb(0xA0, 0x20, 0xF0);
        private static readonly Color DEEP_PINK = Color.FromRgb(0xFF, 0x14, 0x93);
        private static readonly Color GREEN = Color.FromRgb(0x00, 0xFF, 0x00);
        private static readonly Color FIRE_BRICK = Color.FromRgb(0xB2, 0x22, 0x22);
        private static readonly Color FOREST_GREEN = Color.FromRgb(0x22, 0x8B, 0x22);

        // The atom color look-up table.
        private static readonly IDictionary<int, Color> ATOM_COLORS_MASSNUM = new Dictionary<int, Color>();
        private static readonly IDictionary<string, Color> ATOM_COLORS_SYMBOL = new Dictionary<string, Color>();

        // Build table.
        static CPKAtomColors()
        {
            // Colors keyed on (uppercase) atomic symbol.
            ATOM_COLORS_SYMBOL["H"] = WHITE;
            ATOM_COLORS_SYMBOL["HE"] = PINK;
            ATOM_COLORS_SYMBOL["LI"] = FIRE_BRICK;
            ATOM_COLORS_SYMBOL["B"] = GREEN;
            ATOM_COLORS_SYMBOL["C"] = LIGHT_GREY;
            ATOM_COLORS_SYMBOL["N"] = SKY_BLUE;
            ATOM_COLORS_SYMBOL["O"] = RED;
            ATOM_COLORS_SYMBOL["F"] = GOLDEN_ROD;
            ATOM_COLORS_SYMBOL["NA"] = Blue;
            ATOM_COLORS_SYMBOL["MG"] = FOREST_GREEN;
            ATOM_COLORS_SYMBOL["AL"] = DARK_GREY;
            ATOM_COLORS_SYMBOL["SI"] = GOLDEN_ROD;
            ATOM_COLORS_SYMBOL["P"] = ORANGE;
            ATOM_COLORS_SYMBOL["S"] = Yellow;
            ATOM_COLORS_SYMBOL["CL"] = GREEN;
            ATOM_COLORS_SYMBOL["CA"] = DARK_GREY;
            ATOM_COLORS_SYMBOL["TI"] = DARK_GREY;
            ATOM_COLORS_SYMBOL["CR"] = DARK_GREY;
            ATOM_COLORS_SYMBOL["MN"] = DARK_GREY;
            ATOM_COLORS_SYMBOL["FE"] = ORANGE;
            ATOM_COLORS_SYMBOL["NI"] = BROWN;
            ATOM_COLORS_SYMBOL["CU"] = BROWN;
            ATOM_COLORS_SYMBOL["ZN"] = BROWN;
            ATOM_COLORS_SYMBOL["BR"] = BROWN;
            ATOM_COLORS_SYMBOL["AG"] = DARK_GREY;
            ATOM_COLORS_SYMBOL["I"] = PURPLE;
            ATOM_COLORS_SYMBOL["BA"] = ORANGE;
            ATOM_COLORS_SYMBOL["AU"] = GOLDEN_ROD;

            // Colors keyed on atomic number.
            ATOM_COLORS_MASSNUM[1] = ATOM_COLORS_SYMBOL["H"];
            ATOM_COLORS_MASSNUM[2] = ATOM_COLORS_SYMBOL["HE"];
            ATOM_COLORS_MASSNUM[3] = ATOM_COLORS_SYMBOL["LI"];
            ATOM_COLORS_MASSNUM[5] = ATOM_COLORS_SYMBOL["B"];
            ATOM_COLORS_MASSNUM[6] = ATOM_COLORS_SYMBOL["C"];
            ATOM_COLORS_MASSNUM[7] = ATOM_COLORS_SYMBOL["N"];
            ATOM_COLORS_MASSNUM[8] = ATOM_COLORS_SYMBOL["O"];
            ATOM_COLORS_MASSNUM[9] = ATOM_COLORS_SYMBOL["F"];
            ATOM_COLORS_MASSNUM[11] = ATOM_COLORS_SYMBOL["NA"];
            ATOM_COLORS_MASSNUM[12] = ATOM_COLORS_SYMBOL["MG"];
            ATOM_COLORS_MASSNUM[13] = ATOM_COLORS_SYMBOL["AL"];
            ATOM_COLORS_MASSNUM[14] = ATOM_COLORS_SYMBOL["SI"];
            ATOM_COLORS_MASSNUM[15] = ATOM_COLORS_SYMBOL["P"];
            ATOM_COLORS_MASSNUM[16] = ATOM_COLORS_SYMBOL["S"];
            ATOM_COLORS_MASSNUM[17] = ATOM_COLORS_SYMBOL["CL"];
            ATOM_COLORS_MASSNUM[20] = ATOM_COLORS_SYMBOL["CA"];
            ATOM_COLORS_MASSNUM[22] = ATOM_COLORS_SYMBOL["TI"];
            ATOM_COLORS_MASSNUM[24] = ATOM_COLORS_SYMBOL["CR"];
            ATOM_COLORS_MASSNUM[25] = ATOM_COLORS_SYMBOL["MN"];
            ATOM_COLORS_MASSNUM[26] = ATOM_COLORS_SYMBOL["FE"];
            ATOM_COLORS_MASSNUM[28] = ATOM_COLORS_SYMBOL["NI"];
            ATOM_COLORS_MASSNUM[29] = ATOM_COLORS_SYMBOL["CU"];
            ATOM_COLORS_MASSNUM[30] = ATOM_COLORS_SYMBOL["ZN"];
            ATOM_COLORS_MASSNUM[35] = ATOM_COLORS_SYMBOL["BR"];
            ATOM_COLORS_MASSNUM[47] = ATOM_COLORS_SYMBOL["AG"];
            ATOM_COLORS_MASSNUM[53] = ATOM_COLORS_SYMBOL["I"];
            ATOM_COLORS_MASSNUM[56] = ATOM_COLORS_SYMBOL["BA"];
            ATOM_COLORS_MASSNUM[79] = ATOM_COLORS_SYMBOL["AU"];
        }

        //////////
        // METHODS
        //////////

        /// <summary>
        /// Returns the font color for atom given atom.
        /// </summary>
        /// <param name="atom">the atom.</param>
        /// <returns>A color for the atom.</returns>
        public Color GetAtomColor(IAtom atom)
        {
            return GetAtomColor(atom, DEEP_PINK);
        }

        /// <summary>
        /// Returns the font color for atom given atom.
        /// </summary>
        /// <param name="atom">the atom.</param>
        /// <param name="defaultColor">atom default color.</param>
        /// <returns>A color for the atom.  The default colour is used if none is found for the atom.</returns>
        public Color GetAtomColor(IAtom atom, Color defaultColor)
        {
            Color color = defaultColor;
            string symbol = atom.Symbol.ToUpperInvariant();
            if (atom.AtomicNumber != null && ATOM_COLORS_MASSNUM.ContainsKey(atom.AtomicNumber.Value))
            {
                color = ATOM_COLORS_MASSNUM[atom.AtomicNumber.Value]; // lookup by atomic number.
            }
            else if (ATOM_COLORS_SYMBOL.ContainsKey(symbol))
            {
                color = ATOM_COLORS_SYMBOL[symbol]; // lookup by atomic symbol.
            }

            return Color.FromRgb(color.R, color.G, color.B); // return atom copy.
        }
    }
}
