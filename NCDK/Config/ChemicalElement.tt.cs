
/* 
 * Copyright (C) 2017-2018  Kazuya Ujihara <ujihara.kazuya@gmail.com>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Collections.Generic;

namespace NCDK.Config
{
    public static class ChemicalElements
    {
        public static readonly ChemicalElement Unknown = new ChemicalElement("Unknown", 0, "", 0, 0, null, 0.00, null);
        public static readonly ChemicalElement Hydrogen = new ChemicalElement("Hydrogen", 1, "H", 1, 1, 0.37, 1.20, 2.20);
        public static readonly ChemicalElement Helium = new ChemicalElement("Helium", 2, "He", 1, 18, 0.32, 1.40, null);
        public static readonly ChemicalElement Lithium = new ChemicalElement("Lithium", 3, "Li", 2, 1, 1.34, 2.20, 0.98);
        public static readonly ChemicalElement Beryllium = new ChemicalElement("Beryllium", 4, "Be", 2, 2, 0.90, 1.90, 1.57);
        public static readonly ChemicalElement Boron = new ChemicalElement("Boron", 5, "B", 2, 13, 0.82, 1.80, 2.04);
        public static readonly ChemicalElement Carbon = new ChemicalElement("Carbon", 6, "C", 2, 14, 0.77, 1.70, 2.55);
        public static readonly ChemicalElement Nitrogen = new ChemicalElement("Nitrogen", 7, "N", 2, 15, 0.75, 1.60, 3.04);
        public static readonly ChemicalElement Oxygen = new ChemicalElement("Oxygen", 8, "O", 2, 16, 0.73, 1.55, 3.44);
        public static readonly ChemicalElement Fluorine = new ChemicalElement("Fluorine", 9, "F", 2, 17, 0.71, 1.50, 3.98);
        public static readonly ChemicalElement Neon = new ChemicalElement("Neon", 10, "Ne", 2, 18, 0.69, 1.54, null);
        public static readonly ChemicalElement Sodium = new ChemicalElement("Sodium", 11, "Na", 3, 1, 1.54, 2.40, 0.93);
        public static readonly ChemicalElement Magnesium = new ChemicalElement("Magnesium", 12, "Mg", 3, 2, 1.30, 2.20, 1.31);
        public static readonly ChemicalElement Aluminium = new ChemicalElement("Aluminium", 13, "Al", 3, 13, 1.18, 2.10, 1.61);
        public static readonly ChemicalElement Silicon = new ChemicalElement("Silicon", 14, "Si", 3, 14, 1.11, 2.10, 1.90);
        public static readonly ChemicalElement Phosphorus = new ChemicalElement("Phosphorus", 15, "P", 3, 15, 1.06, 1.95, 2.19);
        public static readonly ChemicalElement Sulfur = new ChemicalElement("Sulfur", 16, "S", 3, 16, 1.02, 1.80, 2.58);
        public static readonly ChemicalElement Chlorine = new ChemicalElement("Chlorine", 17, "Cl", 3, 17, 0.99, 1.80, 3.16);
        public static readonly ChemicalElement Argon = new ChemicalElement("Argon", 18, "Ar", 3, 18, 0.97, 1.88, null);
        public static readonly ChemicalElement Potassium = new ChemicalElement("Potassium", 19, "K", 4, 1, 1.96, 2.80, 0.82);
        public static readonly ChemicalElement Calcium = new ChemicalElement("Calcium", 20, "Ca", 4, 2, 1.74, 2.40, 1.00);
        public static readonly ChemicalElement Scandium = new ChemicalElement("Scandium", 21, "Sc", 4, 3, 1.44, 2.30, 1.36);
        public static readonly ChemicalElement Titanium = new ChemicalElement("Titanium", 22, "Ti", 4, 4, 1.36, 2.15, 1.54);
        public static readonly ChemicalElement Vanadium = new ChemicalElement("Vanadium", 23, "V", 4, 5, 1.25, 2.05, 1.63);
        public static readonly ChemicalElement Chromium = new ChemicalElement("Chromium", 24, "Cr", 4, 6, 1.27, 2.05, 1.66);
        public static readonly ChemicalElement Manganese = new ChemicalElement("Manganese", 25, "Mn", 4, 7, 1.39, 2.05, 1.55);
        public static readonly ChemicalElement Iron = new ChemicalElement("Iron", 26, "Fe", 4, 8, 1.25, 2.05, 1.83);
        public static readonly ChemicalElement Cobalt = new ChemicalElement("Cobalt", 27, "Co", 4, 9, 1.26, null, 1.88);
        public static readonly ChemicalElement Nickel = new ChemicalElement("Nickel", 28, "Ni", 4, 10, 1.21, null, 1.91);
        public static readonly ChemicalElement Copper = new ChemicalElement("Copper", 29, "Cu", 4, 11, 1.38, null, 1.90);
        public static readonly ChemicalElement Zinc = new ChemicalElement("Zinc", 30, "Zn", 4, 12, 1.31, 2.10, 1.65);
        public static readonly ChemicalElement Gallium = new ChemicalElement("Gallium", 31, "Ga", 4, 13, 1.26, 2.10, 1.81);
        public static readonly ChemicalElement Germanium = new ChemicalElement("Germanium", 32, "Ge", 4, 14, 1.22, 2.10, 2.01);
        public static readonly ChemicalElement Arsenic = new ChemicalElement("Arsenic", 33, "As", 4, 15, 1.19, 2.05, 2.18);
        public static readonly ChemicalElement Selenium = new ChemicalElement("Selenium", 34, "Se", 4, 16, 1.16, 1.90, 2.55);
        public static readonly ChemicalElement Bromine = new ChemicalElement("Bromine", 35, "Br", 4, 17, 1.14, 1.90, 2.96);
        public static readonly ChemicalElement Krypton = new ChemicalElement("Krypton", 36, "Kr", 4, 18, 1.10, 2.02, 3.00);
        public static readonly ChemicalElement Rubidium = new ChemicalElement("Rubidium", 37, "Rb", 5, 1, 2.11, 2.90, 0.82);
        public static readonly ChemicalElement Strontium = new ChemicalElement("Strontium", 38, "Sr", 5, 2, 1.92, 2.55, 0.95);
        public static readonly ChemicalElement Yttrium = new ChemicalElement("Yttrium", 39, "Y", 5, 3, 1.62, 2.40, 1.22);
        public static readonly ChemicalElement Zirconium = new ChemicalElement("Zirconium", 40, "Zr", 5, 4, 1.48, 2.30, 1.33);
        public static readonly ChemicalElement Niobium = new ChemicalElement("Niobium", 41, "Nb", 5, 5, 1.37, 2.15, 1.60);
        public static readonly ChemicalElement Molybdenum = new ChemicalElement("Molybdenum", 42, "Mo", 5, 6, 1.45, 2.10, 2.16);
        public static readonly ChemicalElement Technetium = new ChemicalElement("Technetium", 43, "Tc", 5, 7, 1.56, 2.05, 1.90);
        public static readonly ChemicalElement Ruthenium = new ChemicalElement("Ruthenium", 44, "Ru", 5, 8, 1.26, 2.05, 2.20);
        public static readonly ChemicalElement Rhodium = new ChemicalElement("Rhodium", 45, "Rh", 5, 9, 1.35, null, 2.28);
        public static readonly ChemicalElement Palladium = new ChemicalElement("Palladium", 46, "Pd", 5, 10, 1.31, 2.05, 2.20);
        public static readonly ChemicalElement Silver = new ChemicalElement("Silver", 47, "Ag", 5, 11, 1.53, 2.10, 1.93);
        public static readonly ChemicalElement Cadmium = new ChemicalElement("Cadmium", 48, "Cd", 5, 12, 1.48, 2.20, 1.69);
        public static readonly ChemicalElement Indium = new ChemicalElement("Indium", 49, "In", 5, 13, 1.44, 2.20, 1.78);
        public static readonly ChemicalElement Tin = new ChemicalElement("Tin", 50, "Sn", 5, 14, 1.41, 2.25, 1.96);
        public static readonly ChemicalElement Antimony = new ChemicalElement("Antimony", 51, "Sb", 5, 15, 1.38, 2.20, 2.05);
        public static readonly ChemicalElement Tellurium = new ChemicalElement("Tellurium", 52, "Te", 5, 16, 1.35, 2.10, 2.10);
        public static readonly ChemicalElement Iodine = new ChemicalElement("Iodine", 53, "I", 5, 17, 1.33, 2.10, 2.66);
        public static readonly ChemicalElement Xenon = new ChemicalElement("Xenon", 54, "Xe", 5, 18, 1.30, 2.16, 2.60);
        public static readonly ChemicalElement Caesium = new ChemicalElement("Caesium", 55, "Cs", 6, 1, 2.25, 3.00, 0.79);
        public static readonly ChemicalElement Barium = new ChemicalElement("Barium", 56, "Ba", 6, 2, 1.98, 2.70, 0.89);
        public static readonly ChemicalElement Lanthanum = new ChemicalElement("Lanthanum", 57, "La", 6, 3, 1.69, 2.50, 1.10);
        public static readonly ChemicalElement Cerium = new ChemicalElement("Cerium", 58, "Ce", 6, 0, null, 2.48, 1.12);
        public static readonly ChemicalElement Praseodymium = new ChemicalElement("Praseodymium", 59, "Pr", 6, 0, null, 2.47, 1.13);
        public static readonly ChemicalElement Neodymium = new ChemicalElement("Neodymium", 60, "Nd", 6, 0, null, 2.45, 1.14);
        public static readonly ChemicalElement Promethium = new ChemicalElement("Promethium", 61, "Pm", 6, 0, null, 2.43, null);
        public static readonly ChemicalElement Samarium = new ChemicalElement("Samarium", 62, "Sm", 6, 0, null, 2.42, 1.17);
        public static readonly ChemicalElement Europium = new ChemicalElement("Europium", 63, "Eu", 6, 0, 2.40, 2.40, null);
        public static readonly ChemicalElement Gadolinium = new ChemicalElement("Gadolinium", 64, "Gd", 6, 0, null, 2.38, 1.20);
        public static readonly ChemicalElement Terbium = new ChemicalElement("Terbium", 65, "Tb", 6, 0, null, 2.37, null);
        public static readonly ChemicalElement Dysprosium = new ChemicalElement("Dysprosium", 66, "Dy", 6, 0, null, 2.35, 1.22);
        public static readonly ChemicalElement Holmium = new ChemicalElement("Holmium", 67, "Ho", 6, 0, null, 2.33, 1.23);
        public static readonly ChemicalElement Erbium = new ChemicalElement("Erbium", 68, "Er", 6, 0, null, 2.32, 1.24);
        public static readonly ChemicalElement Thulium = new ChemicalElement("Thulium", 69, "Tm", 6, 0, null, 2.30, 1.25);
        public static readonly ChemicalElement Ytterbium = new ChemicalElement("Ytterbium", 70, "Yb", 6, 0, null, 2.28, null);
        public static readonly ChemicalElement Lutetium = new ChemicalElement("Lutetium", 71, "Lu", 6, 0, 1.60, 2.27, 1.27);
        public static readonly ChemicalElement Hafnium = new ChemicalElement("Hafnium", 72, "Hf", 6, 4, 1.50, 2.25, 1.30);
        public static readonly ChemicalElement Tantalum = new ChemicalElement("Tantalum", 73, "Ta", 6, 5, 1.38, 2.20, 1.50);
        public static readonly ChemicalElement Tungsten = new ChemicalElement("Tungsten", 74, "W", 6, 6, 1.46, 2.10, 2.36);
        public static readonly ChemicalElement Rhenium = new ChemicalElement("Rhenium", 75, "Re", 6, 7, 1.59, 2.05, 1.90);
        public static readonly ChemicalElement Osmium = new ChemicalElement("Osmium", 76, "Os", 6, 8, 1.28, null, 2.20);
        public static readonly ChemicalElement Iridium = new ChemicalElement("Iridium", 77, "Ir", 6, 9, 1.37, null, 2.20);
        public static readonly ChemicalElement Platinum = new ChemicalElement("Platinum", 78, "Pt", 6, 10, 1.28, 2.05, 2.28);
        public static readonly ChemicalElement Gold = new ChemicalElement("Gold", 79, "Au", 6, 11, 1.44, 2.10, 2.54);
        public static readonly ChemicalElement Mercury = new ChemicalElement("Mercury", 80, "Hg", 6, 12, 1.49, 2.05, 2.00);
        public static readonly ChemicalElement Thallium = new ChemicalElement("Thallium", 81, "Tl", 6, 13, 1.48, 2.20, 1.62);
        public static readonly ChemicalElement Lead = new ChemicalElement("Lead", 82, "Pb", 6, 14, 1.47, 2.30, 2.33);
        public static readonly ChemicalElement Bismuth = new ChemicalElement("Bismuth", 83, "Bi", 6, 15, 1.46, 2.30, 2.02);
        public static readonly ChemicalElement Polonium = new ChemicalElement("Polonium", 84, "Po", 6, 16, 1.46, null, 2.00);
        public static readonly ChemicalElement Astatine = new ChemicalElement("Astatine", 85, "At", 6, 17, null, null, 2.20);
        public static readonly ChemicalElement Radon = new ChemicalElement("Radon", 86, "Rn", 6, 18, 1.45, null, null);
        public static readonly ChemicalElement Francium = new ChemicalElement("Francium", 87, "Fr", 7, 1, null, null, 0.70);
        public static readonly ChemicalElement Radium = new ChemicalElement("Radium", 88, "Ra", 7, 2, null, null, 0.90);
        public static readonly ChemicalElement Actinium = new ChemicalElement("Actinium", 89, "Ac", 7, 3, null, null, 1.10);
        public static readonly ChemicalElement Thorium = new ChemicalElement("Thorium", 90, "Th", 7, 0, null, 2.40, 1.30);
        public static readonly ChemicalElement Protactinium = new ChemicalElement("Protactinium", 91, "Pa", 7, 0, null, null, 1.50);
        public static readonly ChemicalElement Uranium = new ChemicalElement("Uranium", 92, "U", 7, 0, null, 2.30, 1.38);
        public static readonly ChemicalElement Neptunium = new ChemicalElement("Neptunium", 93, "Np", 7, 0, null, null, 1.36);
        public static readonly ChemicalElement Plutonium = new ChemicalElement("Plutonium", 94, "Pu", 7, 0, null, null, 1.28);
        public static readonly ChemicalElement Americium = new ChemicalElement("Americium", 95, "Am", 7, 0, null, null, 1.30);
        public static readonly ChemicalElement Curium = new ChemicalElement("Curium", 96, "Cm", 7, 0, null, null, 1.30);
        public static readonly ChemicalElement Berkelium = new ChemicalElement("Berkelium", 97, "Bk", 7, 0, null, null, 1.30);
        public static readonly ChemicalElement Californium = new ChemicalElement("Californium", 98, "Cf", 7, 0, null, null, 1.30);
        public static readonly ChemicalElement Einsteinium = new ChemicalElement("Einsteinium", 99, "Es", 7, 0, null, null, 1.30);
        public static readonly ChemicalElement Fermium = new ChemicalElement("Fermium", 100, "Fm", 7, 0, null, null, 1.30);
        public static readonly ChemicalElement Mendelevium = new ChemicalElement("Mendelevium", 101, "Md", 7, 0, null, null, 1.30);
        public static readonly ChemicalElement Nobelium = new ChemicalElement("Nobelium", 102, "No", 7, 0, null, null, 1.30);
        public static readonly ChemicalElement Lawrencium = new ChemicalElement("Lawrencium", 103, "Lr", 7, 0, null, null, null);
        public static readonly ChemicalElement Rutherfordium = new ChemicalElement("Rutherfordium", 104, "Rf", 7, 4, null, null, null);
        public static readonly ChemicalElement Dubnium = new ChemicalElement("Dubnium", 105, "Db", 7, 5, null, null, null);
        public static readonly ChemicalElement Seaborgium = new ChemicalElement("Seaborgium", 106, "Sg", 7, 6, null, null, null);
        public static readonly ChemicalElement Bohrium = new ChemicalElement("Bohrium", 107, "Bh", 7, 7, null, null, null);
        public static readonly ChemicalElement Hassium = new ChemicalElement("Hassium", 108, "Hs", 7, 8, null, null, null);
        public static readonly ChemicalElement Meitnerium = new ChemicalElement("Meitnerium", 109, "Mt", 7, 9, null, null, null);
        public static readonly ChemicalElement Darmstadtium = new ChemicalElement("Darmstadtium", 110, "Ds", 7, 10, null, null, null);
        public static readonly ChemicalElement Roentgenium = new ChemicalElement("Roentgenium", 111, "Rg", 7, 11, null, null, null);
        public static readonly ChemicalElement Copernicium = new ChemicalElement("Copernicium", 112, "Cn", 7, 12, null, null, null);
        [Obsolete("Use " + nameof(Nihonium))]
        public static readonly ChemicalElement Ununtrium = new ChemicalElement("Ununtrium", 113, "Uut", 0, 0, null, null, null);
        public static readonly ChemicalElement Nihonium = new ChemicalElement("Nihonium", 113, "Nh", 0, 0, null, null, null);
        public static readonly ChemicalElement Flerovium = new ChemicalElement("Flerovium", 114, "Fl", 0, 0, null, null, null);
        [Obsolete("Use " + nameof(Moscovium))]
        public static readonly ChemicalElement Ununpentium = new ChemicalElement("Ununpentium", 115, "Uup", 0, 0, null, null, null);
        public static readonly ChemicalElement Moscovium = new ChemicalElement("Moscovium", 115, "Mc", 0, 0, null, null, null);
        public static readonly ChemicalElement Livermorium = new ChemicalElement("Livermorium", 116, "Lv", 0, 0, null, null, null);
        [Obsolete("Use " + nameof(Tennessine))]
        public static readonly ChemicalElement Ununseptium = new ChemicalElement("Ununseptium", 117, "Uus", 0, 0, null, null, null);
        public static readonly ChemicalElement Tennessine = new ChemicalElement("Tennessine", 117, "Ts", 0, 0, null, null, null);
        [Obsolete("Use " + nameof(Oganesson))]
        public static readonly ChemicalElement Ununoctium = new ChemicalElement("Ununoctium", 118, "Uuo", 0, 0, null, null, null);
        public static readonly ChemicalElement Oganesson = new ChemicalElement("Oganesson", 118, "Og", 0, 0, null, null, null);
    }
    public sealed partial class ChemicalElement
    {
#pragma warning disable CA1034 // Nested types should not be visible
        public static class AtomicNumbers
#pragma warning restore CA1034 // Nested types should not be visible
        {
            public const int Unknown = 0;
            public const int Hydrogen = 1;
            public const int Helium = 2;
            public const int Lithium = 3;
            public const int Beryllium = 4;
            public const int Boron = 5;
            public const int Carbon = 6;
            public const int Nitrogen = 7;
            public const int Oxygen = 8;
            public const int Fluorine = 9;
            public const int Neon = 10;
            public const int Sodium = 11;
            public const int Magnesium = 12;
            public const int Aluminium = 13;
            public const int Silicon = 14;
            public const int Phosphorus = 15;
            public const int Sulfur = 16;
            public const int Chlorine = 17;
            public const int Argon = 18;
            public const int Potassium = 19;
            public const int Calcium = 20;
            public const int Scandium = 21;
            public const int Titanium = 22;
            public const int Vanadium = 23;
            public const int Chromium = 24;
            public const int Manganese = 25;
            public const int Iron = 26;
            public const int Cobalt = 27;
            public const int Nickel = 28;
            public const int Copper = 29;
            public const int Zinc = 30;
            public const int Gallium = 31;
            public const int Germanium = 32;
            public const int Arsenic = 33;
            public const int Selenium = 34;
            public const int Bromine = 35;
            public const int Krypton = 36;
            public const int Rubidium = 37;
            public const int Strontium = 38;
            public const int Yttrium = 39;
            public const int Zirconium = 40;
            public const int Niobium = 41;
            public const int Molybdenum = 42;
            public const int Technetium = 43;
            public const int Ruthenium = 44;
            public const int Rhodium = 45;
            public const int Palladium = 46;
            public const int Silver = 47;
            public const int Cadmium = 48;
            public const int Indium = 49;
            public const int Tin = 50;
            public const int Antimony = 51;
            public const int Tellurium = 52;
            public const int Iodine = 53;
            public const int Xenon = 54;
            public const int Caesium = 55;
            public const int Barium = 56;
            public const int Lanthanum = 57;
            public const int Cerium = 58;
            public const int Praseodymium = 59;
            public const int Neodymium = 60;
            public const int Promethium = 61;
            public const int Samarium = 62;
            public const int Europium = 63;
            public const int Gadolinium = 64;
            public const int Terbium = 65;
            public const int Dysprosium = 66;
            public const int Holmium = 67;
            public const int Erbium = 68;
            public const int Thulium = 69;
            public const int Ytterbium = 70;
            public const int Lutetium = 71;
            public const int Hafnium = 72;
            public const int Tantalum = 73;
            public const int Tungsten = 74;
            public const int Rhenium = 75;
            public const int Osmium = 76;
            public const int Iridium = 77;
            public const int Platinum = 78;
            public const int Gold = 79;
            public const int Mercury = 80;
            public const int Thallium = 81;
            public const int Lead = 82;
            public const int Bismuth = 83;
            public const int Polonium = 84;
            public const int Astatine = 85;
            public const int Radon = 86;
            public const int Francium = 87;
            public const int Radium = 88;
            public const int Actinium = 89;
            public const int Thorium = 90;
            public const int Protactinium = 91;
            public const int Uranium = 92;
            public const int Neptunium = 93;
            public const int Plutonium = 94;
            public const int Americium = 95;
            public const int Curium = 96;
            public const int Berkelium = 97;
            public const int Californium = 98;
            public const int Einsteinium = 99;
            public const int Fermium = 100;
            public const int Mendelevium = 101;
            public const int Nobelium = 102;
            public const int Lawrencium = 103;
            public const int Rutherfordium = 104;
            public const int Dubnium = 105;
            public const int Seaborgium = 106;
            public const int Bohrium = 107;
            public const int Hassium = 108;
            public const int Meitnerium = 109;
            public const int Darmstadtium = 110;
            public const int Roentgenium = 111;
            public const int Copernicium = 112;
            public const int Ununtrium = 113;
            public const int Nihonium = 113;
            public const int Flerovium = 114;
            public const int Ununpentium = 115;
            public const int Moscovium = 115;
            public const int Livermorium = 116;
            public const int Ununseptium = 117;
            public const int Tennessine = 117;
            public const int Ununoctium = 118;
            public const int Oganesson = 118;
        }

        private static IReadOnlyDictionary<string, ChemicalElement> symbolMap;
        
        /// <summary>
        /// Lookup elements by symbol / name.
        /// </summary>
        internal static IReadOnlyDictionary<string, ChemicalElement> SymbolMap
        {
            get
            {
                if (symbolMap == null)
                {
                    symbolMap = new Dictionary<string, ChemicalElement>()
                    {
                        ["unknown"] = ChemicalElements.Unknown,
                        [""] = ChemicalElements.Unknown,
                        ["hydrogen"] = ChemicalElements.Hydrogen,
                        ["h"] = ChemicalElements.Hydrogen,
                        ["helium"] = ChemicalElements.Helium,
                        ["he"] = ChemicalElements.Helium,
                        ["lithium"] = ChemicalElements.Lithium,
                        ["li"] = ChemicalElements.Lithium,
                        ["beryllium"] = ChemicalElements.Beryllium,
                        ["be"] = ChemicalElements.Beryllium,
                        ["boron"] = ChemicalElements.Boron,
                        ["b"] = ChemicalElements.Boron,
                        ["carbon"] = ChemicalElements.Carbon,
                        ["c"] = ChemicalElements.Carbon,
                        ["nitrogen"] = ChemicalElements.Nitrogen,
                        ["n"] = ChemicalElements.Nitrogen,
                        ["oxygen"] = ChemicalElements.Oxygen,
                        ["o"] = ChemicalElements.Oxygen,
                        ["fluorine"] = ChemicalElements.Fluorine,
                        ["f"] = ChemicalElements.Fluorine,
                        ["neon"] = ChemicalElements.Neon,
                        ["ne"] = ChemicalElements.Neon,
                        ["sodium"] = ChemicalElements.Sodium,
                        ["na"] = ChemicalElements.Sodium,
                        ["magnesium"] = ChemicalElements.Magnesium,
                        ["mg"] = ChemicalElements.Magnesium,
                        ["aluminium"] = ChemicalElements.Aluminium,
                        ["al"] = ChemicalElements.Aluminium,
                        ["silicon"] = ChemicalElements.Silicon,
                        ["si"] = ChemicalElements.Silicon,
                        ["phosphorus"] = ChemicalElements.Phosphorus,
                        ["p"] = ChemicalElements.Phosphorus,
                        ["sulfur"] = ChemicalElements.Sulfur,
                        ["s"] = ChemicalElements.Sulfur,
                        ["chlorine"] = ChemicalElements.Chlorine,
                        ["cl"] = ChemicalElements.Chlorine,
                        ["argon"] = ChemicalElements.Argon,
                        ["ar"] = ChemicalElements.Argon,
                        ["potassium"] = ChemicalElements.Potassium,
                        ["k"] = ChemicalElements.Potassium,
                        ["calcium"] = ChemicalElements.Calcium,
                        ["ca"] = ChemicalElements.Calcium,
                        ["scandium"] = ChemicalElements.Scandium,
                        ["sc"] = ChemicalElements.Scandium,
                        ["titanium"] = ChemicalElements.Titanium,
                        ["ti"] = ChemicalElements.Titanium,
                        ["vanadium"] = ChemicalElements.Vanadium,
                        ["v"] = ChemicalElements.Vanadium,
                        ["chromium"] = ChemicalElements.Chromium,
                        ["cr"] = ChemicalElements.Chromium,
                        ["manganese"] = ChemicalElements.Manganese,
                        ["mn"] = ChemicalElements.Manganese,
                        ["iron"] = ChemicalElements.Iron,
                        ["fe"] = ChemicalElements.Iron,
                        ["cobalt"] = ChemicalElements.Cobalt,
                        ["co"] = ChemicalElements.Cobalt,
                        ["nickel"] = ChemicalElements.Nickel,
                        ["ni"] = ChemicalElements.Nickel,
                        ["copper"] = ChemicalElements.Copper,
                        ["cu"] = ChemicalElements.Copper,
                        ["zinc"] = ChemicalElements.Zinc,
                        ["zn"] = ChemicalElements.Zinc,
                        ["gallium"] = ChemicalElements.Gallium,
                        ["ga"] = ChemicalElements.Gallium,
                        ["germanium"] = ChemicalElements.Germanium,
                        ["ge"] = ChemicalElements.Germanium,
                        ["arsenic"] = ChemicalElements.Arsenic,
                        ["as"] = ChemicalElements.Arsenic,
                        ["selenium"] = ChemicalElements.Selenium,
                        ["se"] = ChemicalElements.Selenium,
                        ["bromine"] = ChemicalElements.Bromine,
                        ["br"] = ChemicalElements.Bromine,
                        ["krypton"] = ChemicalElements.Krypton,
                        ["kr"] = ChemicalElements.Krypton,
                        ["rubidium"] = ChemicalElements.Rubidium,
                        ["rb"] = ChemicalElements.Rubidium,
                        ["strontium"] = ChemicalElements.Strontium,
                        ["sr"] = ChemicalElements.Strontium,
                        ["yttrium"] = ChemicalElements.Yttrium,
                        ["y"] = ChemicalElements.Yttrium,
                        ["zirconium"] = ChemicalElements.Zirconium,
                        ["zr"] = ChemicalElements.Zirconium,
                        ["niobium"] = ChemicalElements.Niobium,
                        ["nb"] = ChemicalElements.Niobium,
                        ["molybdenum"] = ChemicalElements.Molybdenum,
                        ["mo"] = ChemicalElements.Molybdenum,
                        ["technetium"] = ChemicalElements.Technetium,
                        ["tc"] = ChemicalElements.Technetium,
                        ["ruthenium"] = ChemicalElements.Ruthenium,
                        ["ru"] = ChemicalElements.Ruthenium,
                        ["rhodium"] = ChemicalElements.Rhodium,
                        ["rh"] = ChemicalElements.Rhodium,
                        ["palladium"] = ChemicalElements.Palladium,
                        ["pd"] = ChemicalElements.Palladium,
                        ["silver"] = ChemicalElements.Silver,
                        ["ag"] = ChemicalElements.Silver,
                        ["cadmium"] = ChemicalElements.Cadmium,
                        ["cd"] = ChemicalElements.Cadmium,
                        ["indium"] = ChemicalElements.Indium,
                        ["in"] = ChemicalElements.Indium,
                        ["tin"] = ChemicalElements.Tin,
                        ["sn"] = ChemicalElements.Tin,
                        ["antimony"] = ChemicalElements.Antimony,
                        ["sb"] = ChemicalElements.Antimony,
                        ["tellurium"] = ChemicalElements.Tellurium,
                        ["te"] = ChemicalElements.Tellurium,
                        ["iodine"] = ChemicalElements.Iodine,
                        ["i"] = ChemicalElements.Iodine,
                        ["xenon"] = ChemicalElements.Xenon,
                        ["xe"] = ChemicalElements.Xenon,
                        ["caesium"] = ChemicalElements.Caesium,
                        ["cs"] = ChemicalElements.Caesium,
                        ["barium"] = ChemicalElements.Barium,
                        ["ba"] = ChemicalElements.Barium,
                        ["lanthanum"] = ChemicalElements.Lanthanum,
                        ["la"] = ChemicalElements.Lanthanum,
                        ["cerium"] = ChemicalElements.Cerium,
                        ["ce"] = ChemicalElements.Cerium,
                        ["praseodymium"] = ChemicalElements.Praseodymium,
                        ["pr"] = ChemicalElements.Praseodymium,
                        ["neodymium"] = ChemicalElements.Neodymium,
                        ["nd"] = ChemicalElements.Neodymium,
                        ["promethium"] = ChemicalElements.Promethium,
                        ["pm"] = ChemicalElements.Promethium,
                        ["samarium"] = ChemicalElements.Samarium,
                        ["sm"] = ChemicalElements.Samarium,
                        ["europium"] = ChemicalElements.Europium,
                        ["eu"] = ChemicalElements.Europium,
                        ["gadolinium"] = ChemicalElements.Gadolinium,
                        ["gd"] = ChemicalElements.Gadolinium,
                        ["terbium"] = ChemicalElements.Terbium,
                        ["tb"] = ChemicalElements.Terbium,
                        ["dysprosium"] = ChemicalElements.Dysprosium,
                        ["dy"] = ChemicalElements.Dysprosium,
                        ["holmium"] = ChemicalElements.Holmium,
                        ["ho"] = ChemicalElements.Holmium,
                        ["erbium"] = ChemicalElements.Erbium,
                        ["er"] = ChemicalElements.Erbium,
                        ["thulium"] = ChemicalElements.Thulium,
                        ["tm"] = ChemicalElements.Thulium,
                        ["ytterbium"] = ChemicalElements.Ytterbium,
                        ["yb"] = ChemicalElements.Ytterbium,
                        ["lutetium"] = ChemicalElements.Lutetium,
                        ["lu"] = ChemicalElements.Lutetium,
                        ["hafnium"] = ChemicalElements.Hafnium,
                        ["hf"] = ChemicalElements.Hafnium,
                        ["tantalum"] = ChemicalElements.Tantalum,
                        ["ta"] = ChemicalElements.Tantalum,
                        ["tungsten"] = ChemicalElements.Tungsten,
                        ["w"] = ChemicalElements.Tungsten,
                        ["rhenium"] = ChemicalElements.Rhenium,
                        ["re"] = ChemicalElements.Rhenium,
                        ["osmium"] = ChemicalElements.Osmium,
                        ["os"] = ChemicalElements.Osmium,
                        ["iridium"] = ChemicalElements.Iridium,
                        ["ir"] = ChemicalElements.Iridium,
                        ["platinum"] = ChemicalElements.Platinum,
                        ["pt"] = ChemicalElements.Platinum,
                        ["gold"] = ChemicalElements.Gold,
                        ["au"] = ChemicalElements.Gold,
                        ["mercury"] = ChemicalElements.Mercury,
                        ["hg"] = ChemicalElements.Mercury,
                        ["thallium"] = ChemicalElements.Thallium,
                        ["tl"] = ChemicalElements.Thallium,
                        ["lead"] = ChemicalElements.Lead,
                        ["pb"] = ChemicalElements.Lead,
                        ["bismuth"] = ChemicalElements.Bismuth,
                        ["bi"] = ChemicalElements.Bismuth,
                        ["polonium"] = ChemicalElements.Polonium,
                        ["po"] = ChemicalElements.Polonium,
                        ["astatine"] = ChemicalElements.Astatine,
                        ["at"] = ChemicalElements.Astatine,
                        ["radon"] = ChemicalElements.Radon,
                        ["rn"] = ChemicalElements.Radon,
                        ["francium"] = ChemicalElements.Francium,
                        ["fr"] = ChemicalElements.Francium,
                        ["radium"] = ChemicalElements.Radium,
                        ["ra"] = ChemicalElements.Radium,
                        ["actinium"] = ChemicalElements.Actinium,
                        ["ac"] = ChemicalElements.Actinium,
                        ["thorium"] = ChemicalElements.Thorium,
                        ["th"] = ChemicalElements.Thorium,
                        ["protactinium"] = ChemicalElements.Protactinium,
                        ["pa"] = ChemicalElements.Protactinium,
                        ["uranium"] = ChemicalElements.Uranium,
                        ["u"] = ChemicalElements.Uranium,
                        ["neptunium"] = ChemicalElements.Neptunium,
                        ["np"] = ChemicalElements.Neptunium,
                        ["plutonium"] = ChemicalElements.Plutonium,
                        ["pu"] = ChemicalElements.Plutonium,
                        ["americium"] = ChemicalElements.Americium,
                        ["am"] = ChemicalElements.Americium,
                        ["curium"] = ChemicalElements.Curium,
                        ["cm"] = ChemicalElements.Curium,
                        ["berkelium"] = ChemicalElements.Berkelium,
                        ["bk"] = ChemicalElements.Berkelium,
                        ["californium"] = ChemicalElements.Californium,
                        ["cf"] = ChemicalElements.Californium,
                        ["einsteinium"] = ChemicalElements.Einsteinium,
                        ["es"] = ChemicalElements.Einsteinium,
                        ["fermium"] = ChemicalElements.Fermium,
                        ["fm"] = ChemicalElements.Fermium,
                        ["mendelevium"] = ChemicalElements.Mendelevium,
                        ["md"] = ChemicalElements.Mendelevium,
                        ["nobelium"] = ChemicalElements.Nobelium,
                        ["no"] = ChemicalElements.Nobelium,
                        ["lawrencium"] = ChemicalElements.Lawrencium,
                        ["lr"] = ChemicalElements.Lawrencium,
                        ["rutherfordium"] = ChemicalElements.Rutherfordium,
                        ["rf"] = ChemicalElements.Rutherfordium,
                        ["dubnium"] = ChemicalElements.Dubnium,
                        ["db"] = ChemicalElements.Dubnium,
                        ["seaborgium"] = ChemicalElements.Seaborgium,
                        ["sg"] = ChemicalElements.Seaborgium,
                        ["bohrium"] = ChemicalElements.Bohrium,
                        ["bh"] = ChemicalElements.Bohrium,
                        ["hassium"] = ChemicalElements.Hassium,
                        ["hs"] = ChemicalElements.Hassium,
                        ["meitnerium"] = ChemicalElements.Meitnerium,
                        ["mt"] = ChemicalElements.Meitnerium,
                        ["darmstadtium"] = ChemicalElements.Darmstadtium,
                        ["ds"] = ChemicalElements.Darmstadtium,
                        ["roentgenium"] = ChemicalElements.Roentgenium,
                        ["rg"] = ChemicalElements.Roentgenium,
                        ["copernicium"] = ChemicalElements.Copernicium,
                        ["cn"] = ChemicalElements.Copernicium,
                        ["ununtrium"] = ChemicalElements.Ununtrium,
                        ["uut"] = ChemicalElements.Ununtrium,
                        ["nihonium"] = ChemicalElements.Nihonium,
                        ["nh"] = ChemicalElements.Nihonium,
                        ["flerovium"] = ChemicalElements.Flerovium,
                        ["fl"] = ChemicalElements.Flerovium,
                        ["ununpentium"] = ChemicalElements.Ununpentium,
                        ["uup"] = ChemicalElements.Ununpentium,
                        ["moscovium"] = ChemicalElements.Moscovium,
                        ["mc"] = ChemicalElements.Moscovium,
                        ["livermorium"] = ChemicalElements.Livermorium,
                        ["lv"] = ChemicalElements.Livermorium,
                        ["ununseptium"] = ChemicalElements.Ununseptium,
                        ["uus"] = ChemicalElements.Ununseptium,
                        ["tennessine"] = ChemicalElements.Tennessine,
                        ["ts"] = ChemicalElements.Tennessine,
                        ["ununoctium"] = ChemicalElements.Ununoctium,
                        ["uuo"] = ChemicalElements.Ununoctium,
                        ["oganesson"] = ChemicalElements.Oganesson,
                        ["og"] = ChemicalElements.Oganesson,
                        // recently named elements
                        ["uub"] = ChemicalElements.Copernicium, // 2009
                        ["ununbium"] = ChemicalElements.Copernicium,
                        ["uuq"] = ChemicalElements.Flerovium, // 2012
                        ["ununquadium"] = ChemicalElements.Flerovium,
                        ["uuh"] = ChemicalElements.Livermorium, // 2012
                        ["ununhexium"] = ChemicalElements.Livermorium,
                        // alternative spellings
                        ["sulphur"] = ChemicalElements.Sulfur,
                        ["cesium"] = ChemicalElements.Caesium,
                        ["aluminum"] = ChemicalElements.Aluminium,
                    };
                }
                return symbolMap;
            }
        }

        private static IReadOnlyList<ChemicalElement> values;

        /// <summary>
        /// Lookup elements by atomic number.
        /// </summary>
        public static IReadOnlyList<ChemicalElement> Values
        {
            get
            {
                if (values == null)
                {
                    values = new ChemicalElement[] 
                    {
                    ChemicalElements.Unknown,
                    ChemicalElements.Hydrogen,
                    ChemicalElements.Helium,
                    ChemicalElements.Lithium,
                    ChemicalElements.Beryllium,
                    ChemicalElements.Boron,
                    ChemicalElements.Carbon,
                    ChemicalElements.Nitrogen,
                    ChemicalElements.Oxygen,
                    ChemicalElements.Fluorine,
                    ChemicalElements.Neon,
                    ChemicalElements.Sodium,
                    ChemicalElements.Magnesium,
                    ChemicalElements.Aluminium,
                    ChemicalElements.Silicon,
                    ChemicalElements.Phosphorus,
                    ChemicalElements.Sulfur,
                    ChemicalElements.Chlorine,
                    ChemicalElements.Argon,
                    ChemicalElements.Potassium,
                    ChemicalElements.Calcium,
                    ChemicalElements.Scandium,
                    ChemicalElements.Titanium,
                    ChemicalElements.Vanadium,
                    ChemicalElements.Chromium,
                    ChemicalElements.Manganese,
                    ChemicalElements.Iron,
                    ChemicalElements.Cobalt,
                    ChemicalElements.Nickel,
                    ChemicalElements.Copper,
                    ChemicalElements.Zinc,
                    ChemicalElements.Gallium,
                    ChemicalElements.Germanium,
                    ChemicalElements.Arsenic,
                    ChemicalElements.Selenium,
                    ChemicalElements.Bromine,
                    ChemicalElements.Krypton,
                    ChemicalElements.Rubidium,
                    ChemicalElements.Strontium,
                    ChemicalElements.Yttrium,
                    ChemicalElements.Zirconium,
                    ChemicalElements.Niobium,
                    ChemicalElements.Molybdenum,
                    ChemicalElements.Technetium,
                    ChemicalElements.Ruthenium,
                    ChemicalElements.Rhodium,
                    ChemicalElements.Palladium,
                    ChemicalElements.Silver,
                    ChemicalElements.Cadmium,
                    ChemicalElements.Indium,
                    ChemicalElements.Tin,
                    ChemicalElements.Antimony,
                    ChemicalElements.Tellurium,
                    ChemicalElements.Iodine,
                    ChemicalElements.Xenon,
                    ChemicalElements.Caesium,
                    ChemicalElements.Barium,
                    ChemicalElements.Lanthanum,
                    ChemicalElements.Cerium,
                    ChemicalElements.Praseodymium,
                    ChemicalElements.Neodymium,
                    ChemicalElements.Promethium,
                    ChemicalElements.Samarium,
                    ChemicalElements.Europium,
                    ChemicalElements.Gadolinium,
                    ChemicalElements.Terbium,
                    ChemicalElements.Dysprosium,
                    ChemicalElements.Holmium,
                    ChemicalElements.Erbium,
                    ChemicalElements.Thulium,
                    ChemicalElements.Ytterbium,
                    ChemicalElements.Lutetium,
                    ChemicalElements.Hafnium,
                    ChemicalElements.Tantalum,
                    ChemicalElements.Tungsten,
                    ChemicalElements.Rhenium,
                    ChemicalElements.Osmium,
                    ChemicalElements.Iridium,
                    ChemicalElements.Platinum,
                    ChemicalElements.Gold,
                    ChemicalElements.Mercury,
                    ChemicalElements.Thallium,
                    ChemicalElements.Lead,
                    ChemicalElements.Bismuth,
                    ChemicalElements.Polonium,
                    ChemicalElements.Astatine,
                    ChemicalElements.Radon,
                    ChemicalElements.Francium,
                    ChemicalElements.Radium,
                    ChemicalElements.Actinium,
                    ChemicalElements.Thorium,
                    ChemicalElements.Protactinium,
                    ChemicalElements.Uranium,
                    ChemicalElements.Neptunium,
                    ChemicalElements.Plutonium,
                    ChemicalElements.Americium,
                    ChemicalElements.Curium,
                    ChemicalElements.Berkelium,
                    ChemicalElements.Californium,
                    ChemicalElements.Einsteinium,
                    ChemicalElements.Fermium,
                    ChemicalElements.Mendelevium,
                    ChemicalElements.Nobelium,
                    ChemicalElements.Lawrencium,
                    ChemicalElements.Rutherfordium,
                    ChemicalElements.Dubnium,
                    ChemicalElements.Seaborgium,
                    ChemicalElements.Bohrium,
                    ChemicalElements.Hassium,
                    ChemicalElements.Meitnerium,
                    ChemicalElements.Darmstadtium,
                    ChemicalElements.Roentgenium,
                    ChemicalElements.Copernicium,
                    ChemicalElements.Nihonium,
                    ChemicalElements.Flerovium,
                    ChemicalElements.Moscovium,
                    ChemicalElements.Livermorium,
                    ChemicalElements.Tennessine,
                    ChemicalElements.Oganesson,
                    };
                }
                return values;
            }
        }
    }
}

