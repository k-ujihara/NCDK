
using System;

namespace NCDK.Config
{
    public sealed partial class Elements
    {
        public static readonly Elements Unknown = new Elements("Unknown", 0, "", 0, 0, null, 0.00, null);
        public static readonly Elements Hydrogen = new Elements("Hydrogen", 1, "H", 1, 1, 0.37, 1.20, 2.20);
        public static readonly Elements Helium = new Elements("Helium", 2, "He", 1, 18, 0.32, 1.40, null);
        public static readonly Elements Lithium = new Elements("Lithium", 3, "Li", 2, 1, 1.34, 2.20, 0.98);
        public static readonly Elements Beryllium = new Elements("Beryllium", 4, "Be", 2, 2, 0.90, 1.90, 1.57);
        public static readonly Elements Boron = new Elements("Boron", 5, "B", 2, 13, 0.82, 1.80, 2.04);
        public static readonly Elements Carbon = new Elements("Carbon", 6, "C", 2, 14, 0.77, 1.70, 2.55);
        public static readonly Elements Nitrogen = new Elements("Nitrogen", 7, "N", 2, 15, 0.75, 1.60, 3.04);
        public static readonly Elements Oxygen = new Elements("Oxygen", 8, "O", 2, 16, 0.73, 1.55, 3.44);
        public static readonly Elements Fluorine = new Elements("Fluorine", 9, "F", 2, 17, 0.71, 1.50, 3.98);
        public static readonly Elements Neon = new Elements("Neon", 10, "Ne", 2, 18, 0.69, 1.54, null);
        public static readonly Elements Sodium = new Elements("Sodium", 11, "Na", 3, 1, 1.54, 2.40, 0.93);
        public static readonly Elements Magnesium = new Elements("Magnesium", 12, "Mg", 3, 2, 1.30, 2.20, 1.31);
        public static readonly Elements Aluminium = new Elements("Aluminium", 13, "Al", 3, 13, 1.18, 2.10, 1.61);
        public static readonly Elements Silicon = new Elements("Silicon", 14, "Si", 3, 14, 1.11, 2.10, 1.90);
        public static readonly Elements Phosphorus = new Elements("Phosphorus", 15, "P", 3, 15, 1.06, 1.95, 2.19);
        public static readonly Elements Sulfur = new Elements("Sulfur", 16, "S", 3, 16, 1.02, 1.80, 2.58);
        public static readonly Elements Chlorine = new Elements("Chlorine", 17, "Cl", 3, 17, 0.99, 1.80, 3.16);
        public static readonly Elements Argon = new Elements("Argon", 18, "Ar", 3, 18, 0.97, 1.88, null);
        public static readonly Elements Potassium = new Elements("Potassium", 19, "K", 4, 1, 1.96, 2.80, 0.82);
        public static readonly Elements Calcium = new Elements("Calcium", 20, "Ca", 4, 2, 1.74, 2.40, 1.00);
        public static readonly Elements Scandium = new Elements("Scandium", 21, "Sc", 4, 3, 1.44, 2.30, 1.36);
        public static readonly Elements Titanium = new Elements("Titanium", 22, "Ti", 4, 4, 1.36, 2.15, 1.54);
        public static readonly Elements Vanadium = new Elements("Vanadium", 23, "V", 4, 5, 1.25, 2.05, 1.63);
        public static readonly Elements Chromium = new Elements("Chromium", 24, "Cr", 4, 6, 1.27, 2.05, 1.66);
        public static readonly Elements Manganese = new Elements("Manganese", 25, "Mn", 4, 7, 1.39, 2.05, 1.55);
        public static readonly Elements Iron = new Elements("Iron", 26, "Fe", 4, 8, 1.25, 2.05, 1.83);
        public static readonly Elements Cobalt = new Elements("Cobalt", 27, "Co", 4, 9, 1.26, null, 1.88);
        public static readonly Elements Nickel = new Elements("Nickel", 28, "Ni", 4, 10, 1.21, null, 1.91);
        public static readonly Elements Copper = new Elements("Copper", 29, "Cu", 4, 11, 1.38, null, 1.90);
        public static readonly Elements Zinc = new Elements("Zinc", 30, "Zn", 4, 12, 1.31, 2.10, 1.65);
        public static readonly Elements Gallium = new Elements("Gallium", 31, "Ga", 4, 13, 1.26, 2.10, 1.81);
        public static readonly Elements Germanium = new Elements("Germanium", 32, "Ge", 4, 14, 1.22, 2.10, 2.01);
        public static readonly Elements Arsenic = new Elements("Arsenic", 33, "As", 4, 15, 1.19, 2.05, 2.18);
        public static readonly Elements Selenium = new Elements("Selenium", 34, "Se", 4, 16, 1.16, 1.90, 2.55);
        public static readonly Elements Bromine = new Elements("Bromine", 35, "Br", 4, 17, 1.14, 1.90, 2.96);
        public static readonly Elements Krypton = new Elements("Krypton", 36, "Kr", 4, 18, 1.10, 2.02, 3.00);
        public static readonly Elements Rubidium = new Elements("Rubidium", 37, "Rb", 5, 1, 2.11, 2.90, 0.82);
        public static readonly Elements Strontium = new Elements("Strontium", 38, "Sr", 5, 2, 1.92, 2.55, 0.95);
        public static readonly Elements Yttrium = new Elements("Yttrium", 39, "Y", 5, 3, 1.62, 2.40, 1.22);
        public static readonly Elements Zirconium = new Elements("Zirconium", 40, "Zr", 5, 4, 1.48, 2.30, 1.33);
        public static readonly Elements Niobium = new Elements("Niobium", 41, "Nb", 5, 5, 1.37, 2.15, 1.60);
        public static readonly Elements Molybdenum = new Elements("Molybdenum", 42, "Mo", 5, 6, 1.45, 2.10, 2.16);
        public static readonly Elements Technetium = new Elements("Technetium", 43, "Tc", 5, 7, 1.56, 2.05, 1.90);
        public static readonly Elements Ruthenium = new Elements("Ruthenium", 44, "Ru", 5, 8, 1.26, 2.05, 2.20);
        public static readonly Elements Rhodium = new Elements("Rhodium", 45, "Rh", 5, 9, 1.35, null, 2.28);
        public static readonly Elements Palladium = new Elements("Palladium", 46, "Pd", 5, 10, 1.31, 2.05, 2.20);
        public static readonly Elements Silver = new Elements("Silver", 47, "Ag", 5, 11, 1.53, 2.10, 1.93);
        public static readonly Elements Cadmium = new Elements("Cadmium", 48, "Cd", 5, 12, 1.48, 2.20, 1.69);
        public static readonly Elements Indium = new Elements("Indium", 49, "In", 5, 13, 1.44, 2.20, 1.78);
        public static readonly Elements Tin = new Elements("Tin", 50, "Sn", 5, 14, 1.41, 2.25, 1.96);
        public static readonly Elements Antimony = new Elements("Antimony", 51, "Sb", 5, 15, 1.38, 2.20, 2.05);
        public static readonly Elements Tellurium = new Elements("Tellurium", 52, "Te", 5, 16, 1.35, 2.10, 2.10);
        public static readonly Elements Iodine = new Elements("Iodine", 53, "I", 5, 17, 1.33, 2.10, 2.66);
        public static readonly Elements Xenon = new Elements("Xenon", 54, "Xe", 5, 18, 1.30, 2.16, 2.60);
        public static readonly Elements Caesium = new Elements("Caesium", 55, "Cs", 6, 1, 2.25, 3.00, 0.79);
        public static readonly Elements Barium = new Elements("Barium", 56, "Ba", 6, 2, 1.98, 2.70, 0.89);
        public static readonly Elements Lanthanum = new Elements("Lanthanum", 57, "La", 6, 3, 1.69, 2.50, 1.10);
        public static readonly Elements Cerium = new Elements("Cerium", 58, "Ce", 6, 0, null, 2.48, 1.12);
        public static readonly Elements Praseodymium = new Elements("Praseodymium", 59, "Pr", 6, 0, null, 2.47, 1.13);
        public static readonly Elements Neodymium = new Elements("Neodymium", 60, "Nd", 6, 0, null, 2.45, 1.14);
        public static readonly Elements Promethium = new Elements("Promethium", 61, "Pm", 6, 0, null, 2.43, null);
        public static readonly Elements Samarium = new Elements("Samarium", 62, "Sm", 6, 0, null, 2.42, 1.17);
        public static readonly Elements Europium = new Elements("Europium", 63, "Eu", 6, 0, 2.40, 2.40, null);
        public static readonly Elements Gadolinium = new Elements("Gadolinium", 64, "Gd", 6, 0, null, 2.38, 1.20);
        public static readonly Elements Terbium = new Elements("Terbium", 65, "Tb", 6, 0, null, 2.37, null);
        public static readonly Elements Dysprosium = new Elements("Dysprosium", 66, "Dy", 6, 0, null, 2.35, 1.22);
        public static readonly Elements Holmium = new Elements("Holmium", 67, "Ho", 6, 0, null, 2.33, 1.23);
        public static readonly Elements Erbium = new Elements("Erbium", 68, "Er", 6, 0, null, 2.32, 1.24);
        public static readonly Elements Thulium = new Elements("Thulium", 69, "Tm", 6, 0, null, 2.30, 1.25);
        public static readonly Elements Ytterbium = new Elements("Ytterbium", 70, "Yb", 6, 0, null, 2.28, null);
        public static readonly Elements Lutetium = new Elements("Lutetium", 71, "Lu", 6, 0, 1.60, 2.27, 1.27);
        public static readonly Elements Hafnium = new Elements("Hafnium", 72, "Hf", 6, 4, 1.50, 2.25, 1.30);
        public static readonly Elements Tantalum = new Elements("Tantalum", 73, "Ta", 6, 5, 1.38, 2.20, 1.50);
        public static readonly Elements Tungsten = new Elements("Tungsten", 74, "W", 6, 6, 1.46, 2.10, 2.36);
        public static readonly Elements Rhenium = new Elements("Rhenium", 75, "Re", 6, 7, 1.59, 2.05, 1.90);
        public static readonly Elements Osmium = new Elements("Osmium", 76, "Os", 6, 8, 1.28, null, 2.20);
        public static readonly Elements Iridium = new Elements("Iridium", 77, "Ir", 6, 9, 1.37, null, 2.20);
        public static readonly Elements Platinum = new Elements("Platinum", 78, "Pt", 6, 10, 1.28, 2.05, 2.28);
        public static readonly Elements Gold = new Elements("Gold", 79, "Au", 6, 11, 1.44, 2.10, 2.54);
        public static readonly Elements Mercury = new Elements("Mercury", 80, "Hg", 6, 12, 1.49, 2.05, 2.00);
        public static readonly Elements Thallium = new Elements("Thallium", 81, "Tl", 6, 13, 1.48, 2.20, 1.62);
        public static readonly Elements Lead = new Elements("Lead", 82, "Pb", 6, 14, 1.47, 2.30, 2.33);
        public static readonly Elements Bismuth = new Elements("Bismuth", 83, "Bi", 6, 15, 1.46, 2.30, 2.02);
        public static readonly Elements Polonium = new Elements("Polonium", 84, "Po", 6, 16, 1.46, null, 2.00);
        public static readonly Elements Astatine = new Elements("Astatine", 85, "At", 6, 17, null, null, 2.20);
        public static readonly Elements Radon = new Elements("Radon", 86, "Rn", 6, 18, 1.45, null, null);
        public static readonly Elements Francium = new Elements("Francium", 87, "Fr", 7, 1, null, null, 0.70);
        public static readonly Elements Radium = new Elements("Radium", 88, "Ra", 7, 2, null, null, 0.90);
        public static readonly Elements Actinium = new Elements("Actinium", 89, "Ac", 7, 3, null, null, 1.10);
        public static readonly Elements Thorium = new Elements("Thorium", 90, "Th", 7, 0, null, 2.40, 1.30);
        public static readonly Elements Protactinium = new Elements("Protactinium", 91, "Pa", 7, 0, null, null, 1.50);
        public static readonly Elements Uranium = new Elements("Uranium", 92, "U", 7, 0, null, 2.30, 1.38);
        public static readonly Elements Neptunium = new Elements("Neptunium", 93, "Np", 7, 0, null, null, 1.36);
        public static readonly Elements Plutonium = new Elements("Plutonium", 94, "Pu", 7, 0, null, null, 1.28);
        public static readonly Elements Americium = new Elements("Americium", 95, "Am", 7, 0, null, null, 1.30);
        public static readonly Elements Curium = new Elements("Curium", 96, "Cm", 7, 0, null, null, 1.30);
        public static readonly Elements Berkelium = new Elements("Berkelium", 97, "Bk", 7, 0, null, null, 1.30);
        public static readonly Elements Californium = new Elements("Californium", 98, "Cf", 7, 0, null, null, 1.30);
        public static readonly Elements Einsteinium = new Elements("Einsteinium", 99, "Es", 7, 0, null, null, 1.30);
        public static readonly Elements Fermium = new Elements("Fermium", 100, "Fm", 7, 0, null, null, 1.30);
        public static readonly Elements Mendelevium = new Elements("Mendelevium", 101, "Md", 7, 0, null, null, 1.30);
        public static readonly Elements Nobelium = new Elements("Nobelium", 102, "No", 7, 0, null, null, 1.30);
        public static readonly Elements Lawrencium = new Elements("Lawrencium", 103, "Lr", 7, 0, null, null, null);
        public static readonly Elements Rutherfordium = new Elements("Rutherfordium", 104, "Rf", 7, 4, null, null, null);
        public static readonly Elements Dubnium = new Elements("Dubnium", 105, "Db", 7, 5, null, null, null);
        public static readonly Elements Seaborgium = new Elements("Seaborgium", 106, "Sg", 7, 6, null, null, null);
        public static readonly Elements Bohrium = new Elements("Bohrium", 107, "Bh", 7, 7, null, null, null);
        public static readonly Elements Hassium = new Elements("Hassium", 108, "Hs", 7, 8, null, null, null);
        public static readonly Elements Meitnerium = new Elements("Meitnerium", 109, "Mt", 7, 9, null, null, null);
        public static readonly Elements Darmstadtium = new Elements("Darmstadtium", 110, "Ds", 7, 10, null, null, null);
        public static readonly Elements Roentgenium = new Elements("Roentgenium", 111, "Rg", 7, 11, null, null, null);
        public static readonly Elements Copernicium = new Elements("Copernicium", 112, "Cn", 7, 12, null, null, null);
		[Obsolete]
        public static readonly Elements Ununtrium = new Elements("Ununtrium", 113, "Uut", 0, 0, null, null, null);
        public static readonly Elements Nihonium = new Elements("Nihonium", 113, "Nh", 0, 0, null, null, null);
        public static readonly Elements Flerovium = new Elements("Flerovium", 114, "Fl", 0, 0, null, null, null);
		[Obsolete]
        public static readonly Elements Ununpentium = new Elements("Ununpentium", 115, "Uup", 0, 0, null, null, null);
        public static readonly Elements Moscovium = new Elements("Moscovium", 115, "Mc", 0, 0, null, null, null);
        public static readonly Elements Livermorium = new Elements("Livermorium", 116, "Lv", 0, 0, null, null, null);
		[Obsolete]
        public static readonly Elements Ununseptium = new Elements("Ununseptium", 117, "Uus", 0, 0, null, null, null);
        public static readonly Elements Tennessine = new Elements("Tennessine", 117, "Ts", 0, 0, null, null, null);
		[Obsolete]
        public static readonly Elements Ununoctium = new Elements("Ununoctium", 118, "Uuo", 0, 0, null, null, null);
        public static readonly Elements Oganesson = new Elements("Oganesson", 118, "Og", 0, 0, null, null, null);
        public static class O
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
            public const int Nihonium = 114;
            public const int Flerovium = 115;
            public const int Ununpentium = 116;
            public const int Moscovium = 117;
            public const int Livermorium = 118;
            public const int Ununseptium = 119;
            public const int Tennessine = 120;
            public const int Ununoctium = 121;
            public const int Oganesson = 122;
        }

        /// <summary>
        /// Lookup elements by atomic number.
        /// </summary>
        public static Elements[] Values { get; } = new Elements[] 
        {
            Unknown,
            Hydrogen,
            Helium,
            Lithium,
            Beryllium,
            Boron,
            Carbon,
            Nitrogen,
            Oxygen,
            Fluorine,
            Neon,
            Sodium,
            Magnesium,
            Aluminium,
            Silicon,
            Phosphorus,
            Sulfur,
            Chlorine,
            Argon,
            Potassium,
            Calcium,
            Scandium,
            Titanium,
            Vanadium,
            Chromium,
            Manganese,
            Iron,
            Cobalt,
            Nickel,
            Copper,
            Zinc,
            Gallium,
            Germanium,
            Arsenic,
            Selenium,
            Bromine,
            Krypton,
            Rubidium,
            Strontium,
            Yttrium,
            Zirconium,
            Niobium,
            Molybdenum,
            Technetium,
            Ruthenium,
            Rhodium,
            Palladium,
            Silver,
            Cadmium,
            Indium,
            Tin,
            Antimony,
            Tellurium,
            Iodine,
            Xenon,
            Caesium,
            Barium,
            Lanthanum,
            Cerium,
            Praseodymium,
            Neodymium,
            Promethium,
            Samarium,
            Europium,
            Gadolinium,
            Terbium,
            Dysprosium,
            Holmium,
            Erbium,
            Thulium,
            Ytterbium,
            Lutetium,
            Hafnium,
            Tantalum,
            Tungsten,
            Rhenium,
            Osmium,
            Iridium,
            Platinum,
            Gold,
            Mercury,
            Thallium,
            Lead,
            Bismuth,
            Polonium,
            Astatine,
            Radon,
            Francium,
            Radium,
            Actinium,
            Thorium,
            Protactinium,
            Uranium,
            Neptunium,
            Plutonium,
            Americium,
            Curium,
            Berkelium,
            Californium,
            Einsteinium,
            Fermium,
            Mendelevium,
            Nobelium,
            Lawrencium,
            Rutherfordium,
            Dubnium,
            Seaborgium,
            Bohrium,
            Hassium,
            Meitnerium,
            Darmstadtium,
            Roentgenium,
            Copernicium,
            Ununtrium,
            Nihonium,
            Flerovium,
            Ununpentium,
            Moscovium,
            Livermorium,
            Ununseptium,
            Tennessine,
            Ununoctium,
            Oganesson,
        };
    }
}

