
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
    public static class NaturalElements
    {
        public static readonly NaturalElement Unknown = new NaturalElement("Unknown", 0, "", 0, 0, null, 0.00, null);
        public static readonly NaturalElement Hydrogen = new NaturalElement("Hydrogen", 1, "H", 1, 1, 0.37, 1.20, 2.20);
        public static readonly NaturalElement Helium = new NaturalElement("Helium", 2, "He", 1, 18, 0.32, 1.40, null);
        public static readonly NaturalElement Lithium = new NaturalElement("Lithium", 3, "Li", 2, 1, 1.34, 2.20, 0.98);
        public static readonly NaturalElement Beryllium = new NaturalElement("Beryllium", 4, "Be", 2, 2, 0.90, 1.90, 1.57);
        public static readonly NaturalElement Boron = new NaturalElement("Boron", 5, "B", 2, 13, 0.82, 1.80, 2.04);
        public static readonly NaturalElement Carbon = new NaturalElement("Carbon", 6, "C", 2, 14, 0.77, 1.70, 2.55);
        public static readonly NaturalElement Nitrogen = new NaturalElement("Nitrogen", 7, "N", 2, 15, 0.75, 1.60, 3.04);
        public static readonly NaturalElement Oxygen = new NaturalElement("Oxygen", 8, "O", 2, 16, 0.73, 1.55, 3.44);
        public static readonly NaturalElement Fluorine = new NaturalElement("Fluorine", 9, "F", 2, 17, 0.71, 1.50, 3.98);
        public static readonly NaturalElement Neon = new NaturalElement("Neon", 10, "Ne", 2, 18, 0.69, 1.54, null);
        public static readonly NaturalElement Sodium = new NaturalElement("Sodium", 11, "Na", 3, 1, 1.54, 2.40, 0.93);
        public static readonly NaturalElement Magnesium = new NaturalElement("Magnesium", 12, "Mg", 3, 2, 1.30, 2.20, 1.31);
        public static readonly NaturalElement Aluminium = new NaturalElement("Aluminium", 13, "Al", 3, 13, 1.18, 2.10, 1.61);
        public static readonly NaturalElement Silicon = new NaturalElement("Silicon", 14, "Si", 3, 14, 1.11, 2.10, 1.90);
        public static readonly NaturalElement Phosphorus = new NaturalElement("Phosphorus", 15, "P", 3, 15, 1.06, 1.95, 2.19);
        public static readonly NaturalElement Sulfur = new NaturalElement("Sulfur", 16, "S", 3, 16, 1.02, 1.80, 2.58);
        public static readonly NaturalElement Chlorine = new NaturalElement("Chlorine", 17, "Cl", 3, 17, 0.99, 1.80, 3.16);
        public static readonly NaturalElement Argon = new NaturalElement("Argon", 18, "Ar", 3, 18, 0.97, 1.88, null);
        public static readonly NaturalElement Potassium = new NaturalElement("Potassium", 19, "K", 4, 1, 1.96, 2.80, 0.82);
        public static readonly NaturalElement Calcium = new NaturalElement("Calcium", 20, "Ca", 4, 2, 1.74, 2.40, 1.00);
        public static readonly NaturalElement Scandium = new NaturalElement("Scandium", 21, "Sc", 4, 3, 1.44, 2.30, 1.36);
        public static readonly NaturalElement Titanium = new NaturalElement("Titanium", 22, "Ti", 4, 4, 1.36, 2.15, 1.54);
        public static readonly NaturalElement Vanadium = new NaturalElement("Vanadium", 23, "V", 4, 5, 1.25, 2.05, 1.63);
        public static readonly NaturalElement Chromium = new NaturalElement("Chromium", 24, "Cr", 4, 6, 1.27, 2.05, 1.66);
        public static readonly NaturalElement Manganese = new NaturalElement("Manganese", 25, "Mn", 4, 7, 1.39, 2.05, 1.55);
        public static readonly NaturalElement Iron = new NaturalElement("Iron", 26, "Fe", 4, 8, 1.25, 2.05, 1.83);
        public static readonly NaturalElement Cobalt = new NaturalElement("Cobalt", 27, "Co", 4, 9, 1.26, null, 1.88);
        public static readonly NaturalElement Nickel = new NaturalElement("Nickel", 28, "Ni", 4, 10, 1.21, null, 1.91);
        public static readonly NaturalElement Copper = new NaturalElement("Copper", 29, "Cu", 4, 11, 1.38, null, 1.90);
        public static readonly NaturalElement Zinc = new NaturalElement("Zinc", 30, "Zn", 4, 12, 1.31, 2.10, 1.65);
        public static readonly NaturalElement Gallium = new NaturalElement("Gallium", 31, "Ga", 4, 13, 1.26, 2.10, 1.81);
        public static readonly NaturalElement Germanium = new NaturalElement("Germanium", 32, "Ge", 4, 14, 1.22, 2.10, 2.01);
        public static readonly NaturalElement Arsenic = new NaturalElement("Arsenic", 33, "As", 4, 15, 1.19, 2.05, 2.18);
        public static readonly NaturalElement Selenium = new NaturalElement("Selenium", 34, "Se", 4, 16, 1.16, 1.90, 2.55);
        public static readonly NaturalElement Bromine = new NaturalElement("Bromine", 35, "Br", 4, 17, 1.14, 1.90, 2.96);
        public static readonly NaturalElement Krypton = new NaturalElement("Krypton", 36, "Kr", 4, 18, 1.10, 2.02, 3.00);
        public static readonly NaturalElement Rubidium = new NaturalElement("Rubidium", 37, "Rb", 5, 1, 2.11, 2.90, 0.82);
        public static readonly NaturalElement Strontium = new NaturalElement("Strontium", 38, "Sr", 5, 2, 1.92, 2.55, 0.95);
        public static readonly NaturalElement Yttrium = new NaturalElement("Yttrium", 39, "Y", 5, 3, 1.62, 2.40, 1.22);
        public static readonly NaturalElement Zirconium = new NaturalElement("Zirconium", 40, "Zr", 5, 4, 1.48, 2.30, 1.33);
        public static readonly NaturalElement Niobium = new NaturalElement("Niobium", 41, "Nb", 5, 5, 1.37, 2.15, 1.60);
        public static readonly NaturalElement Molybdenum = new NaturalElement("Molybdenum", 42, "Mo", 5, 6, 1.45, 2.10, 2.16);
        public static readonly NaturalElement Technetium = new NaturalElement("Technetium", 43, "Tc", 5, 7, 1.56, 2.05, 1.90);
        public static readonly NaturalElement Ruthenium = new NaturalElement("Ruthenium", 44, "Ru", 5, 8, 1.26, 2.05, 2.20);
        public static readonly NaturalElement Rhodium = new NaturalElement("Rhodium", 45, "Rh", 5, 9, 1.35, null, 2.28);
        public static readonly NaturalElement Palladium = new NaturalElement("Palladium", 46, "Pd", 5, 10, 1.31, 2.05, 2.20);
        public static readonly NaturalElement Silver = new NaturalElement("Silver", 47, "Ag", 5, 11, 1.53, 2.10, 1.93);
        public static readonly NaturalElement Cadmium = new NaturalElement("Cadmium", 48, "Cd", 5, 12, 1.48, 2.20, 1.69);
        public static readonly NaturalElement Indium = new NaturalElement("Indium", 49, "In", 5, 13, 1.44, 2.20, 1.78);
        public static readonly NaturalElement Tin = new NaturalElement("Tin", 50, "Sn", 5, 14, 1.41, 2.25, 1.96);
        public static readonly NaturalElement Antimony = new NaturalElement("Antimony", 51, "Sb", 5, 15, 1.38, 2.20, 2.05);
        public static readonly NaturalElement Tellurium = new NaturalElement("Tellurium", 52, "Te", 5, 16, 1.35, 2.10, 2.10);
        public static readonly NaturalElement Iodine = new NaturalElement("Iodine", 53, "I", 5, 17, 1.33, 2.10, 2.66);
        public static readonly NaturalElement Xenon = new NaturalElement("Xenon", 54, "Xe", 5, 18, 1.30, 2.16, 2.60);
        public static readonly NaturalElement Caesium = new NaturalElement("Caesium", 55, "Cs", 6, 1, 2.25, 3.00, 0.79);
        public static readonly NaturalElement Barium = new NaturalElement("Barium", 56, "Ba", 6, 2, 1.98, 2.70, 0.89);
        public static readonly NaturalElement Lanthanum = new NaturalElement("Lanthanum", 57, "La", 6, 3, 1.69, 2.50, 1.10);
        public static readonly NaturalElement Cerium = new NaturalElement("Cerium", 58, "Ce", 6, 0, null, 2.48, 1.12);
        public static readonly NaturalElement Praseodymium = new NaturalElement("Praseodymium", 59, "Pr", 6, 0, null, 2.47, 1.13);
        public static readonly NaturalElement Neodymium = new NaturalElement("Neodymium", 60, "Nd", 6, 0, null, 2.45, 1.14);
        public static readonly NaturalElement Promethium = new NaturalElement("Promethium", 61, "Pm", 6, 0, null, 2.43, null);
        public static readonly NaturalElement Samarium = new NaturalElement("Samarium", 62, "Sm", 6, 0, null, 2.42, 1.17);
        public static readonly NaturalElement Europium = new NaturalElement("Europium", 63, "Eu", 6, 0, 2.40, 2.40, null);
        public static readonly NaturalElement Gadolinium = new NaturalElement("Gadolinium", 64, "Gd", 6, 0, null, 2.38, 1.20);
        public static readonly NaturalElement Terbium = new NaturalElement("Terbium", 65, "Tb", 6, 0, null, 2.37, null);
        public static readonly NaturalElement Dysprosium = new NaturalElement("Dysprosium", 66, "Dy", 6, 0, null, 2.35, 1.22);
        public static readonly NaturalElement Holmium = new NaturalElement("Holmium", 67, "Ho", 6, 0, null, 2.33, 1.23);
        public static readonly NaturalElement Erbium = new NaturalElement("Erbium", 68, "Er", 6, 0, null, 2.32, 1.24);
        public static readonly NaturalElement Thulium = new NaturalElement("Thulium", 69, "Tm", 6, 0, null, 2.30, 1.25);
        public static readonly NaturalElement Ytterbium = new NaturalElement("Ytterbium", 70, "Yb", 6, 0, null, 2.28, null);
        public static readonly NaturalElement Lutetium = new NaturalElement("Lutetium", 71, "Lu", 6, 0, 1.60, 2.27, 1.27);
        public static readonly NaturalElement Hafnium = new NaturalElement("Hafnium", 72, "Hf", 6, 4, 1.50, 2.25, 1.30);
        public static readonly NaturalElement Tantalum = new NaturalElement("Tantalum", 73, "Ta", 6, 5, 1.38, 2.20, 1.50);
        public static readonly NaturalElement Tungsten = new NaturalElement("Tungsten", 74, "W", 6, 6, 1.46, 2.10, 2.36);
        public static readonly NaturalElement Rhenium = new NaturalElement("Rhenium", 75, "Re", 6, 7, 1.59, 2.05, 1.90);
        public static readonly NaturalElement Osmium = new NaturalElement("Osmium", 76, "Os", 6, 8, 1.28, null, 2.20);
        public static readonly NaturalElement Iridium = new NaturalElement("Iridium", 77, "Ir", 6, 9, 1.37, null, 2.20);
        public static readonly NaturalElement Platinum = new NaturalElement("Platinum", 78, "Pt", 6, 10, 1.28, 2.05, 2.28);
        public static readonly NaturalElement Gold = new NaturalElement("Gold", 79, "Au", 6, 11, 1.44, 2.10, 2.54);
        public static readonly NaturalElement Mercury = new NaturalElement("Mercury", 80, "Hg", 6, 12, 1.49, 2.05, 2.00);
        public static readonly NaturalElement Thallium = new NaturalElement("Thallium", 81, "Tl", 6, 13, 1.48, 2.20, 1.62);
        public static readonly NaturalElement Lead = new NaturalElement("Lead", 82, "Pb", 6, 14, 1.47, 2.30, 2.33);
        public static readonly NaturalElement Bismuth = new NaturalElement("Bismuth", 83, "Bi", 6, 15, 1.46, 2.30, 2.02);
        public static readonly NaturalElement Polonium = new NaturalElement("Polonium", 84, "Po", 6, 16, 1.46, null, 2.00);
        public static readonly NaturalElement Astatine = new NaturalElement("Astatine", 85, "At", 6, 17, null, null, 2.20);
        public static readonly NaturalElement Radon = new NaturalElement("Radon", 86, "Rn", 6, 18, 1.45, null, null);
        public static readonly NaturalElement Francium = new NaturalElement("Francium", 87, "Fr", 7, 1, null, null, 0.70);
        public static readonly NaturalElement Radium = new NaturalElement("Radium", 88, "Ra", 7, 2, null, null, 0.90);
        public static readonly NaturalElement Actinium = new NaturalElement("Actinium", 89, "Ac", 7, 3, null, null, 1.10);
        public static readonly NaturalElement Thorium = new NaturalElement("Thorium", 90, "Th", 7, 0, null, 2.40, 1.30);
        public static readonly NaturalElement Protactinium = new NaturalElement("Protactinium", 91, "Pa", 7, 0, null, null, 1.50);
        public static readonly NaturalElement Uranium = new NaturalElement("Uranium", 92, "U", 7, 0, null, 2.30, 1.38);
        public static readonly NaturalElement Neptunium = new NaturalElement("Neptunium", 93, "Np", 7, 0, null, null, 1.36);
        public static readonly NaturalElement Plutonium = new NaturalElement("Plutonium", 94, "Pu", 7, 0, null, null, 1.28);
        public static readonly NaturalElement Americium = new NaturalElement("Americium", 95, "Am", 7, 0, null, null, 1.30);
        public static readonly NaturalElement Curium = new NaturalElement("Curium", 96, "Cm", 7, 0, null, null, 1.30);
        public static readonly NaturalElement Berkelium = new NaturalElement("Berkelium", 97, "Bk", 7, 0, null, null, 1.30);
        public static readonly NaturalElement Californium = new NaturalElement("Californium", 98, "Cf", 7, 0, null, null, 1.30);
        public static readonly NaturalElement Einsteinium = new NaturalElement("Einsteinium", 99, "Es", 7, 0, null, null, 1.30);
        public static readonly NaturalElement Fermium = new NaturalElement("Fermium", 100, "Fm", 7, 0, null, null, 1.30);
        public static readonly NaturalElement Mendelevium = new NaturalElement("Mendelevium", 101, "Md", 7, 0, null, null, 1.30);
        public static readonly NaturalElement Nobelium = new NaturalElement("Nobelium", 102, "No", 7, 0, null, null, 1.30);
        public static readonly NaturalElement Lawrencium = new NaturalElement("Lawrencium", 103, "Lr", 7, 0, null, null, null);
        public static readonly NaturalElement Rutherfordium = new NaturalElement("Rutherfordium", 104, "Rf", 7, 4, null, null, null);
        public static readonly NaturalElement Dubnium = new NaturalElement("Dubnium", 105, "Db", 7, 5, null, null, null);
        public static readonly NaturalElement Seaborgium = new NaturalElement("Seaborgium", 106, "Sg", 7, 6, null, null, null);
        public static readonly NaturalElement Bohrium = new NaturalElement("Bohrium", 107, "Bh", 7, 7, null, null, null);
        public static readonly NaturalElement Hassium = new NaturalElement("Hassium", 108, "Hs", 7, 8, null, null, null);
        public static readonly NaturalElement Meitnerium = new NaturalElement("Meitnerium", 109, "Mt", 7, 9, null, null, null);
        public static readonly NaturalElement Darmstadtium = new NaturalElement("Darmstadtium", 110, "Ds", 7, 10, null, null, null);
        public static readonly NaturalElement Roentgenium = new NaturalElement("Roentgenium", 111, "Rg", 7, 11, null, null, null);
        public static readonly NaturalElement Copernicium = new NaturalElement("Copernicium", 112, "Cn", 7, 12, null, null, null);
        [Obsolete("Use " + nameof(Nihonium))]
        public static readonly NaturalElement Ununtrium = new NaturalElement("Ununtrium", 113, "Uut", 7, 13, null, null, null);
        public static readonly NaturalElement Nihonium = new NaturalElement("Nihonium", 113, "Nh", 7, 13, null, null, null);
        public static readonly NaturalElement Flerovium = new NaturalElement("Flerovium", 114, "Fl", 7, 14, null, null, null);
        [Obsolete("Use " + nameof(Moscovium))]
        public static readonly NaturalElement Ununpentium = new NaturalElement("Ununpentium", 115, "Uup", 7, 15, null, null, null);
        public static readonly NaturalElement Moscovium = new NaturalElement("Moscovium", 115, "Mc", 7, 15, null, null, null);
        public static readonly NaturalElement Livermorium = new NaturalElement("Livermorium", 116, "Lv", 7, 16, null, null, null);
        [Obsolete("Use " + nameof(Tennessine))]
        public static readonly NaturalElement Ununseptium = new NaturalElement("Ununseptium", 117, "Uus", 7, 17, null, null, null);
        public static readonly NaturalElement Tennessine = new NaturalElement("Tennessine", 117, "Ts", 7, 17, null, null, null);
        [Obsolete("Use " + nameof(Oganesson))]
        public static readonly NaturalElement Ununoctium = new NaturalElement("Ununoctium", 118, "Uuo", 7, 18, null, null, null);
        public static readonly NaturalElement Oganesson = new NaturalElement("Oganesson", 118, "Og", 7, 18, null, null, null);
    }
    public sealed partial class NaturalElement
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
            internal const int H = 1;
            internal const int He = 2;
            internal const int Li = 3;
            internal const int Be = 4;
            internal const int B = 5;
            internal const int C = 6;
            internal const int N = 7;
            internal const int O = 8;
            internal const int F = 9;
            internal const int Ne = 10;
            internal const int Na = 11;
            internal const int Mg = 12;
            internal const int Al = 13;
            internal const int Si = 14;
            internal const int P = 15;
            internal const int S = 16;
            internal const int Cl = 17;
            internal const int Ar = 18;
            internal const int K = 19;
            internal const int Ca = 20;
            internal const int Sc = 21;
            internal const int Ti = 22;
            internal const int V = 23;
            internal const int Cr = 24;
            internal const int Mn = 25;
            internal const int Fe = 26;
            internal const int Co = 27;
            internal const int Ni = 28;
            internal const int Cu = 29;
            internal const int Zn = 30;
            internal const int Ga = 31;
            internal const int Ge = 32;
            internal const int As = 33;
            internal const int Se = 34;
            internal const int Br = 35;
            internal const int Kr = 36;
            internal const int Rb = 37;
            internal const int Sr = 38;
            internal const int Y = 39;
            internal const int Zr = 40;
            internal const int Nb = 41;
            internal const int Mo = 42;
            internal const int Tc = 43;
            internal const int Ru = 44;
            internal const int Rh = 45;
            internal const int Pd = 46;
            internal const int Ag = 47;
            internal const int Cd = 48;
            internal const int In = 49;
            internal const int Sn = 50;
            internal const int Sb = 51;
            internal const int Te = 52;
            internal const int I = 53;
            internal const int Xe = 54;
            internal const int Cs = 55;
            internal const int Ba = 56;
            internal const int La = 57;
            internal const int Ce = 58;
            internal const int Pr = 59;
            internal const int Nd = 60;
            internal const int Pm = 61;
            internal const int Sm = 62;
            internal const int Eu = 63;
            internal const int Gd = 64;
            internal const int Tb = 65;
            internal const int Dy = 66;
            internal const int Ho = 67;
            internal const int Er = 68;
            internal const int Tm = 69;
            internal const int Yb = 70;
            internal const int Lu = 71;
            internal const int Hf = 72;
            internal const int Ta = 73;
            internal const int W = 74;
            internal const int Re = 75;
            internal const int Os = 76;
            internal const int Ir = 77;
            internal const int Pt = 78;
            internal const int Au = 79;
            internal const int Hg = 80;
            internal const int Tl = 81;
            internal const int Pb = 82;
            internal const int Bi = 83;
            internal const int Po = 84;
            internal const int At = 85;
            internal const int Rn = 86;
            internal const int Fr = 87;
            internal const int Ra = 88;
            internal const int Ac = 89;
            internal const int Th = 90;
            internal const int Pa = 91;
            internal const int U = 92;
            internal const int Np = 93;
            internal const int Pu = 94;
            internal const int Am = 95;
            internal const int Cm = 96;
            internal const int Bk = 97;
            internal const int Cf = 98;
            internal const int Es = 99;
            internal const int Fm = 100;
            internal const int Md = 101;
            internal const int No = 102;
            internal const int Lr = 103;
            internal const int Rf = 104;
            internal const int Db = 105;
            internal const int Sg = 106;
            internal const int Bh = 107;
            internal const int Hs = 108;
            internal const int Mt = 109;
            internal const int Ds = 110;
            internal const int Rg = 111;
            internal const int Cn = 112;
            internal const int Uut = 113;
            internal const int Nh = 113;
            internal const int Fl = 114;
            internal const int Uup = 115;
            internal const int Mc = 115;
            internal const int Lv = 116;
            internal const int Uus = 117;
            internal const int Ts = 117;
            internal const int Uuo = 118;
            internal const int Og = 118;
        }

        /// <summary>
        /// Lookup elements by symbol / name.
        /// </summary>
        internal static IReadOnlyDictionary<string, NaturalElement> SymbolMap { get; } = new Dictionary<string, NaturalElement>()
            {
                ["unknown"] = NaturalElements.Unknown,
                [""] = NaturalElements.Unknown,
                ["hydrogen"] = NaturalElements.Hydrogen,
                ["h"] = NaturalElements.Hydrogen,
                ["helium"] = NaturalElements.Helium,
                ["he"] = NaturalElements.Helium,
                ["lithium"] = NaturalElements.Lithium,
                ["li"] = NaturalElements.Lithium,
                ["beryllium"] = NaturalElements.Beryllium,
                ["be"] = NaturalElements.Beryllium,
                ["boron"] = NaturalElements.Boron,
                ["b"] = NaturalElements.Boron,
                ["carbon"] = NaturalElements.Carbon,
                ["c"] = NaturalElements.Carbon,
                ["nitrogen"] = NaturalElements.Nitrogen,
                ["n"] = NaturalElements.Nitrogen,
                ["oxygen"] = NaturalElements.Oxygen,
                ["o"] = NaturalElements.Oxygen,
                ["fluorine"] = NaturalElements.Fluorine,
                ["f"] = NaturalElements.Fluorine,
                ["neon"] = NaturalElements.Neon,
                ["ne"] = NaturalElements.Neon,
                ["sodium"] = NaturalElements.Sodium,
                ["na"] = NaturalElements.Sodium,
                ["magnesium"] = NaturalElements.Magnesium,
                ["mg"] = NaturalElements.Magnesium,
                ["aluminium"] = NaturalElements.Aluminium,
                ["al"] = NaturalElements.Aluminium,
                ["silicon"] = NaturalElements.Silicon,
                ["si"] = NaturalElements.Silicon,
                ["phosphorus"] = NaturalElements.Phosphorus,
                ["p"] = NaturalElements.Phosphorus,
                ["sulfur"] = NaturalElements.Sulfur,
                ["s"] = NaturalElements.Sulfur,
                ["chlorine"] = NaturalElements.Chlorine,
                ["cl"] = NaturalElements.Chlorine,
                ["argon"] = NaturalElements.Argon,
                ["ar"] = NaturalElements.Argon,
                ["potassium"] = NaturalElements.Potassium,
                ["k"] = NaturalElements.Potassium,
                ["calcium"] = NaturalElements.Calcium,
                ["ca"] = NaturalElements.Calcium,
                ["scandium"] = NaturalElements.Scandium,
                ["sc"] = NaturalElements.Scandium,
                ["titanium"] = NaturalElements.Titanium,
                ["ti"] = NaturalElements.Titanium,
                ["vanadium"] = NaturalElements.Vanadium,
                ["v"] = NaturalElements.Vanadium,
                ["chromium"] = NaturalElements.Chromium,
                ["cr"] = NaturalElements.Chromium,
                ["manganese"] = NaturalElements.Manganese,
                ["mn"] = NaturalElements.Manganese,
                ["iron"] = NaturalElements.Iron,
                ["fe"] = NaturalElements.Iron,
                ["cobalt"] = NaturalElements.Cobalt,
                ["co"] = NaturalElements.Cobalt,
                ["nickel"] = NaturalElements.Nickel,
                ["ni"] = NaturalElements.Nickel,
                ["copper"] = NaturalElements.Copper,
                ["cu"] = NaturalElements.Copper,
                ["zinc"] = NaturalElements.Zinc,
                ["zn"] = NaturalElements.Zinc,
                ["gallium"] = NaturalElements.Gallium,
                ["ga"] = NaturalElements.Gallium,
                ["germanium"] = NaturalElements.Germanium,
                ["ge"] = NaturalElements.Germanium,
                ["arsenic"] = NaturalElements.Arsenic,
                ["as"] = NaturalElements.Arsenic,
                ["selenium"] = NaturalElements.Selenium,
                ["se"] = NaturalElements.Selenium,
                ["bromine"] = NaturalElements.Bromine,
                ["br"] = NaturalElements.Bromine,
                ["krypton"] = NaturalElements.Krypton,
                ["kr"] = NaturalElements.Krypton,
                ["rubidium"] = NaturalElements.Rubidium,
                ["rb"] = NaturalElements.Rubidium,
                ["strontium"] = NaturalElements.Strontium,
                ["sr"] = NaturalElements.Strontium,
                ["yttrium"] = NaturalElements.Yttrium,
                ["y"] = NaturalElements.Yttrium,
                ["zirconium"] = NaturalElements.Zirconium,
                ["zr"] = NaturalElements.Zirconium,
                ["niobium"] = NaturalElements.Niobium,
                ["nb"] = NaturalElements.Niobium,
                ["molybdenum"] = NaturalElements.Molybdenum,
                ["mo"] = NaturalElements.Molybdenum,
                ["technetium"] = NaturalElements.Technetium,
                ["tc"] = NaturalElements.Technetium,
                ["ruthenium"] = NaturalElements.Ruthenium,
                ["ru"] = NaturalElements.Ruthenium,
                ["rhodium"] = NaturalElements.Rhodium,
                ["rh"] = NaturalElements.Rhodium,
                ["palladium"] = NaturalElements.Palladium,
                ["pd"] = NaturalElements.Palladium,
                ["silver"] = NaturalElements.Silver,
                ["ag"] = NaturalElements.Silver,
                ["cadmium"] = NaturalElements.Cadmium,
                ["cd"] = NaturalElements.Cadmium,
                ["indium"] = NaturalElements.Indium,
                ["in"] = NaturalElements.Indium,
                ["tin"] = NaturalElements.Tin,
                ["sn"] = NaturalElements.Tin,
                ["antimony"] = NaturalElements.Antimony,
                ["sb"] = NaturalElements.Antimony,
                ["tellurium"] = NaturalElements.Tellurium,
                ["te"] = NaturalElements.Tellurium,
                ["iodine"] = NaturalElements.Iodine,
                ["i"] = NaturalElements.Iodine,
                ["xenon"] = NaturalElements.Xenon,
                ["xe"] = NaturalElements.Xenon,
                ["caesium"] = NaturalElements.Caesium,
                ["cs"] = NaturalElements.Caesium,
                ["barium"] = NaturalElements.Barium,
                ["ba"] = NaturalElements.Barium,
                ["lanthanum"] = NaturalElements.Lanthanum,
                ["la"] = NaturalElements.Lanthanum,
                ["cerium"] = NaturalElements.Cerium,
                ["ce"] = NaturalElements.Cerium,
                ["praseodymium"] = NaturalElements.Praseodymium,
                ["pr"] = NaturalElements.Praseodymium,
                ["neodymium"] = NaturalElements.Neodymium,
                ["nd"] = NaturalElements.Neodymium,
                ["promethium"] = NaturalElements.Promethium,
                ["pm"] = NaturalElements.Promethium,
                ["samarium"] = NaturalElements.Samarium,
                ["sm"] = NaturalElements.Samarium,
                ["europium"] = NaturalElements.Europium,
                ["eu"] = NaturalElements.Europium,
                ["gadolinium"] = NaturalElements.Gadolinium,
                ["gd"] = NaturalElements.Gadolinium,
                ["terbium"] = NaturalElements.Terbium,
                ["tb"] = NaturalElements.Terbium,
                ["dysprosium"] = NaturalElements.Dysprosium,
                ["dy"] = NaturalElements.Dysprosium,
                ["holmium"] = NaturalElements.Holmium,
                ["ho"] = NaturalElements.Holmium,
                ["erbium"] = NaturalElements.Erbium,
                ["er"] = NaturalElements.Erbium,
                ["thulium"] = NaturalElements.Thulium,
                ["tm"] = NaturalElements.Thulium,
                ["ytterbium"] = NaturalElements.Ytterbium,
                ["yb"] = NaturalElements.Ytterbium,
                ["lutetium"] = NaturalElements.Lutetium,
                ["lu"] = NaturalElements.Lutetium,
                ["hafnium"] = NaturalElements.Hafnium,
                ["hf"] = NaturalElements.Hafnium,
                ["tantalum"] = NaturalElements.Tantalum,
                ["ta"] = NaturalElements.Tantalum,
                ["tungsten"] = NaturalElements.Tungsten,
                ["w"] = NaturalElements.Tungsten,
                ["rhenium"] = NaturalElements.Rhenium,
                ["re"] = NaturalElements.Rhenium,
                ["osmium"] = NaturalElements.Osmium,
                ["os"] = NaturalElements.Osmium,
                ["iridium"] = NaturalElements.Iridium,
                ["ir"] = NaturalElements.Iridium,
                ["platinum"] = NaturalElements.Platinum,
                ["pt"] = NaturalElements.Platinum,
                ["gold"] = NaturalElements.Gold,
                ["au"] = NaturalElements.Gold,
                ["mercury"] = NaturalElements.Mercury,
                ["hg"] = NaturalElements.Mercury,
                ["thallium"] = NaturalElements.Thallium,
                ["tl"] = NaturalElements.Thallium,
                ["lead"] = NaturalElements.Lead,
                ["pb"] = NaturalElements.Lead,
                ["bismuth"] = NaturalElements.Bismuth,
                ["bi"] = NaturalElements.Bismuth,
                ["polonium"] = NaturalElements.Polonium,
                ["po"] = NaturalElements.Polonium,
                ["astatine"] = NaturalElements.Astatine,
                ["at"] = NaturalElements.Astatine,
                ["radon"] = NaturalElements.Radon,
                ["rn"] = NaturalElements.Radon,
                ["francium"] = NaturalElements.Francium,
                ["fr"] = NaturalElements.Francium,
                ["radium"] = NaturalElements.Radium,
                ["ra"] = NaturalElements.Radium,
                ["actinium"] = NaturalElements.Actinium,
                ["ac"] = NaturalElements.Actinium,
                ["thorium"] = NaturalElements.Thorium,
                ["th"] = NaturalElements.Thorium,
                ["protactinium"] = NaturalElements.Protactinium,
                ["pa"] = NaturalElements.Protactinium,
                ["uranium"] = NaturalElements.Uranium,
                ["u"] = NaturalElements.Uranium,
                ["neptunium"] = NaturalElements.Neptunium,
                ["np"] = NaturalElements.Neptunium,
                ["plutonium"] = NaturalElements.Plutonium,
                ["pu"] = NaturalElements.Plutonium,
                ["americium"] = NaturalElements.Americium,
                ["am"] = NaturalElements.Americium,
                ["curium"] = NaturalElements.Curium,
                ["cm"] = NaturalElements.Curium,
                ["berkelium"] = NaturalElements.Berkelium,
                ["bk"] = NaturalElements.Berkelium,
                ["californium"] = NaturalElements.Californium,
                ["cf"] = NaturalElements.Californium,
                ["einsteinium"] = NaturalElements.Einsteinium,
                ["es"] = NaturalElements.Einsteinium,
                ["fermium"] = NaturalElements.Fermium,
                ["fm"] = NaturalElements.Fermium,
                ["mendelevium"] = NaturalElements.Mendelevium,
                ["md"] = NaturalElements.Mendelevium,
                ["nobelium"] = NaturalElements.Nobelium,
                ["no"] = NaturalElements.Nobelium,
                ["lawrencium"] = NaturalElements.Lawrencium,
                ["lr"] = NaturalElements.Lawrencium,
                ["rutherfordium"] = NaturalElements.Rutherfordium,
                ["rf"] = NaturalElements.Rutherfordium,
                ["dubnium"] = NaturalElements.Dubnium,
                ["db"] = NaturalElements.Dubnium,
                ["seaborgium"] = NaturalElements.Seaborgium,
                ["sg"] = NaturalElements.Seaborgium,
                ["bohrium"] = NaturalElements.Bohrium,
                ["bh"] = NaturalElements.Bohrium,
                ["hassium"] = NaturalElements.Hassium,
                ["hs"] = NaturalElements.Hassium,
                ["meitnerium"] = NaturalElements.Meitnerium,
                ["mt"] = NaturalElements.Meitnerium,
                ["darmstadtium"] = NaturalElements.Darmstadtium,
                ["ds"] = NaturalElements.Darmstadtium,
                ["roentgenium"] = NaturalElements.Roentgenium,
                ["rg"] = NaturalElements.Roentgenium,
                ["copernicium"] = NaturalElements.Copernicium,
                ["cn"] = NaturalElements.Copernicium,
                ["ununtrium"] = NaturalElements.Ununtrium,
                ["uut"] = NaturalElements.Ununtrium,
                ["nihonium"] = NaturalElements.Nihonium,
                ["nh"] = NaturalElements.Nihonium,
                ["flerovium"] = NaturalElements.Flerovium,
                ["fl"] = NaturalElements.Flerovium,
                ["ununpentium"] = NaturalElements.Ununpentium,
                ["uup"] = NaturalElements.Ununpentium,
                ["moscovium"] = NaturalElements.Moscovium,
                ["mc"] = NaturalElements.Moscovium,
                ["livermorium"] = NaturalElements.Livermorium,
                ["lv"] = NaturalElements.Livermorium,
                ["ununseptium"] = NaturalElements.Ununseptium,
                ["uus"] = NaturalElements.Ununseptium,
                ["tennessine"] = NaturalElements.Tennessine,
                ["ts"] = NaturalElements.Tennessine,
                ["ununoctium"] = NaturalElements.Ununoctium,
                ["uuo"] = NaturalElements.Ununoctium,
                ["oganesson"] = NaturalElements.Oganesson,
                ["og"] = NaturalElements.Oganesson,
                // recently named elements
                ["uub"] = NaturalElements.Copernicium, // 2009
                ["ununbium"] = NaturalElements.Copernicium,
                ["uuq"] = NaturalElements.Flerovium, // 2012
                ["ununquadium"] = NaturalElements.Flerovium,
                ["uuh"] = NaturalElements.Livermorium, // 2012
                ["ununhexium"] = NaturalElements.Livermorium,
                // alternative spellings
                ["sulphur"] = NaturalElements.Sulfur,
                ["cesium"] = NaturalElements.Caesium,
                ["aluminum"] = NaturalElements.Aluminium,
            };


        /// <summary>
        /// Lookup elements by atomic number.
        /// </summary>
        public static IReadOnlyList<NaturalElement> Values { get; } = new NaturalElement[] 
            {
                NaturalElements.Unknown,
                NaturalElements.Hydrogen,
                NaturalElements.Helium,
                NaturalElements.Lithium,
                NaturalElements.Beryllium,
                NaturalElements.Boron,
                NaturalElements.Carbon,
                NaturalElements.Nitrogen,
                NaturalElements.Oxygen,
                NaturalElements.Fluorine,
                NaturalElements.Neon,
                NaturalElements.Sodium,
                NaturalElements.Magnesium,
                NaturalElements.Aluminium,
                NaturalElements.Silicon,
                NaturalElements.Phosphorus,
                NaturalElements.Sulfur,
                NaturalElements.Chlorine,
                NaturalElements.Argon,
                NaturalElements.Potassium,
                NaturalElements.Calcium,
                NaturalElements.Scandium,
                NaturalElements.Titanium,
                NaturalElements.Vanadium,
                NaturalElements.Chromium,
                NaturalElements.Manganese,
                NaturalElements.Iron,
                NaturalElements.Cobalt,
                NaturalElements.Nickel,
                NaturalElements.Copper,
                NaturalElements.Zinc,
                NaturalElements.Gallium,
                NaturalElements.Germanium,
                NaturalElements.Arsenic,
                NaturalElements.Selenium,
                NaturalElements.Bromine,
                NaturalElements.Krypton,
                NaturalElements.Rubidium,
                NaturalElements.Strontium,
                NaturalElements.Yttrium,
                NaturalElements.Zirconium,
                NaturalElements.Niobium,
                NaturalElements.Molybdenum,
                NaturalElements.Technetium,
                NaturalElements.Ruthenium,
                NaturalElements.Rhodium,
                NaturalElements.Palladium,
                NaturalElements.Silver,
                NaturalElements.Cadmium,
                NaturalElements.Indium,
                NaturalElements.Tin,
                NaturalElements.Antimony,
                NaturalElements.Tellurium,
                NaturalElements.Iodine,
                NaturalElements.Xenon,
                NaturalElements.Caesium,
                NaturalElements.Barium,
                NaturalElements.Lanthanum,
                NaturalElements.Cerium,
                NaturalElements.Praseodymium,
                NaturalElements.Neodymium,
                NaturalElements.Promethium,
                NaturalElements.Samarium,
                NaturalElements.Europium,
                NaturalElements.Gadolinium,
                NaturalElements.Terbium,
                NaturalElements.Dysprosium,
                NaturalElements.Holmium,
                NaturalElements.Erbium,
                NaturalElements.Thulium,
                NaturalElements.Ytterbium,
                NaturalElements.Lutetium,
                NaturalElements.Hafnium,
                NaturalElements.Tantalum,
                NaturalElements.Tungsten,
                NaturalElements.Rhenium,
                NaturalElements.Osmium,
                NaturalElements.Iridium,
                NaturalElements.Platinum,
                NaturalElements.Gold,
                NaturalElements.Mercury,
                NaturalElements.Thallium,
                NaturalElements.Lead,
                NaturalElements.Bismuth,
                NaturalElements.Polonium,
                NaturalElements.Astatine,
                NaturalElements.Radon,
                NaturalElements.Francium,
                NaturalElements.Radium,
                NaturalElements.Actinium,
                NaturalElements.Thorium,
                NaturalElements.Protactinium,
                NaturalElements.Uranium,
                NaturalElements.Neptunium,
                NaturalElements.Plutonium,
                NaturalElements.Americium,
                NaturalElements.Curium,
                NaturalElements.Berkelium,
                NaturalElements.Californium,
                NaturalElements.Einsteinium,
                NaturalElements.Fermium,
                NaturalElements.Mendelevium,
                NaturalElements.Nobelium,
                NaturalElements.Lawrencium,
                NaturalElements.Rutherfordium,
                NaturalElements.Dubnium,
                NaturalElements.Seaborgium,
                NaturalElements.Bohrium,
                NaturalElements.Hassium,
                NaturalElements.Meitnerium,
                NaturalElements.Darmstadtium,
                NaturalElements.Roentgenium,
                NaturalElements.Copernicium,
                NaturalElements.Nihonium,
                NaturalElements.Flerovium,
                NaturalElements.Moscovium,
                NaturalElements.Livermorium,
                NaturalElements.Tennessine,
                NaturalElements.Oganesson,
            };
    }
}

