
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
        public static class Unknown
        {
            public const string Name = "Unknown";
            public const string Symbol = "";
            public const int AtomicNumber = 0;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Hydrogen
        {
            public const string Name = "Hydrogen";
            public const string Symbol = "H";
            public const int AtomicNumber = 1;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class H
        {
            public const string Name = Hydrogen.Name;
            public const string Symbol = Hydrogen.Symbol;
            public const int AtomicNumber = Hydrogen.AtomicNumber;
            public static IElement Element { get; } = Hydrogen.Element;
        }
        public static class Helium
        {
            public const string Name = "Helium";
            public const string Symbol = "He";
            public const int AtomicNumber = 2;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class He
        {
            public const string Name = Helium.Name;
            public const string Symbol = Helium.Symbol;
            public const int AtomicNumber = Helium.AtomicNumber;
            public static IElement Element { get; } = Helium.Element;
        }
        public static class Lithium
        {
            public const string Name = "Lithium";
            public const string Symbol = "Li";
            public const int AtomicNumber = 3;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Li
        {
            public const string Name = Lithium.Name;
            public const string Symbol = Lithium.Symbol;
            public const int AtomicNumber = Lithium.AtomicNumber;
            public static IElement Element { get; } = Lithium.Element;
        }
        public static class Beryllium
        {
            public const string Name = "Beryllium";
            public const string Symbol = "Be";
            public const int AtomicNumber = 4;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Be
        {
            public const string Name = Beryllium.Name;
            public const string Symbol = Beryllium.Symbol;
            public const int AtomicNumber = Beryllium.AtomicNumber;
            public static IElement Element { get; } = Beryllium.Element;
        }
        public static class Boron
        {
            public const string Name = "Boron";
            public const string Symbol = "B";
            public const int AtomicNumber = 5;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class B
        {
            public const string Name = Boron.Name;
            public const string Symbol = Boron.Symbol;
            public const int AtomicNumber = Boron.AtomicNumber;
            public static IElement Element { get; } = Boron.Element;
        }
        public static class Carbon
        {
            public const string Name = "Carbon";
            public const string Symbol = "C";
            public const int AtomicNumber = 6;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class C
        {
            public const string Name = Carbon.Name;
            public const string Symbol = Carbon.Symbol;
            public const int AtomicNumber = Carbon.AtomicNumber;
            public static IElement Element { get; } = Carbon.Element;
        }
        public static class Nitrogen
        {
            public const string Name = "Nitrogen";
            public const string Symbol = "N";
            public const int AtomicNumber = 7;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class N
        {
            public const string Name = Nitrogen.Name;
            public const string Symbol = Nitrogen.Symbol;
            public const int AtomicNumber = Nitrogen.AtomicNumber;
            public static IElement Element { get; } = Nitrogen.Element;
        }
        public static class Oxygen
        {
            public const string Name = "Oxygen";
            public const string Symbol = "O";
            public const int AtomicNumber = 8;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class O
        {
            public const string Name = Oxygen.Name;
            public const string Symbol = Oxygen.Symbol;
            public const int AtomicNumber = Oxygen.AtomicNumber;
            public static IElement Element { get; } = Oxygen.Element;
        }
        public static class Fluorine
        {
            public const string Name = "Fluorine";
            public const string Symbol = "F";
            public const int AtomicNumber = 9;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class F
        {
            public const string Name = Fluorine.Name;
            public const string Symbol = Fluorine.Symbol;
            public const int AtomicNumber = Fluorine.AtomicNumber;
            public static IElement Element { get; } = Fluorine.Element;
        }
        public static class Neon
        {
            public const string Name = "Neon";
            public const string Symbol = "Ne";
            public const int AtomicNumber = 10;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ne
        {
            public const string Name = Neon.Name;
            public const string Symbol = Neon.Symbol;
            public const int AtomicNumber = Neon.AtomicNumber;
            public static IElement Element { get; } = Neon.Element;
        }
        public static class Sodium
        {
            public const string Name = "Sodium";
            public const string Symbol = "Na";
            public const int AtomicNumber = 11;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Na
        {
            public const string Name = Sodium.Name;
            public const string Symbol = Sodium.Symbol;
            public const int AtomicNumber = Sodium.AtomicNumber;
            public static IElement Element { get; } = Sodium.Element;
        }
        public static class Magnesium
        {
            public const string Name = "Magnesium";
            public const string Symbol = "Mg";
            public const int AtomicNumber = 12;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Mg
        {
            public const string Name = Magnesium.Name;
            public const string Symbol = Magnesium.Symbol;
            public const int AtomicNumber = Magnesium.AtomicNumber;
            public static IElement Element { get; } = Magnesium.Element;
        }
        public static class Aluminium
        {
            public const string Name = "Aluminium";
            public const string Symbol = "Al";
            public const int AtomicNumber = 13;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Al
        {
            public const string Name = Aluminium.Name;
            public const string Symbol = Aluminium.Symbol;
            public const int AtomicNumber = Aluminium.AtomicNumber;
            public static IElement Element { get; } = Aluminium.Element;
        }
        public static class Silicon
        {
            public const string Name = "Silicon";
            public const string Symbol = "Si";
            public const int AtomicNumber = 14;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Si
        {
            public const string Name = Silicon.Name;
            public const string Symbol = Silicon.Symbol;
            public const int AtomicNumber = Silicon.AtomicNumber;
            public static IElement Element { get; } = Silicon.Element;
        }
        public static class Phosphorus
        {
            public const string Name = "Phosphorus";
            public const string Symbol = "P";
            public const int AtomicNumber = 15;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class P
        {
            public const string Name = Phosphorus.Name;
            public const string Symbol = Phosphorus.Symbol;
            public const int AtomicNumber = Phosphorus.AtomicNumber;
            public static IElement Element { get; } = Phosphorus.Element;
        }
        public static class Sulfur
        {
            public const string Name = "Sulfur";
            public const string Symbol = "S";
            public const int AtomicNumber = 16;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class S
        {
            public const string Name = Sulfur.Name;
            public const string Symbol = Sulfur.Symbol;
            public const int AtomicNumber = Sulfur.AtomicNumber;
            public static IElement Element { get; } = Sulfur.Element;
        }
        public static class Chlorine
        {
            public const string Name = "Chlorine";
            public const string Symbol = "Cl";
            public const int AtomicNumber = 17;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Cl
        {
            public const string Name = Chlorine.Name;
            public const string Symbol = Chlorine.Symbol;
            public const int AtomicNumber = Chlorine.AtomicNumber;
            public static IElement Element { get; } = Chlorine.Element;
        }
        public static class Argon
        {
            public const string Name = "Argon";
            public const string Symbol = "Ar";
            public const int AtomicNumber = 18;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ar
        {
            public const string Name = Argon.Name;
            public const string Symbol = Argon.Symbol;
            public const int AtomicNumber = Argon.AtomicNumber;
            public static IElement Element { get; } = Argon.Element;
        }
        public static class Potassium
        {
            public const string Name = "Potassium";
            public const string Symbol = "K";
            public const int AtomicNumber = 19;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class K
        {
            public const string Name = Potassium.Name;
            public const string Symbol = Potassium.Symbol;
            public const int AtomicNumber = Potassium.AtomicNumber;
            public static IElement Element { get; } = Potassium.Element;
        }
        public static class Calcium
        {
            public const string Name = "Calcium";
            public const string Symbol = "Ca";
            public const int AtomicNumber = 20;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ca
        {
            public const string Name = Calcium.Name;
            public const string Symbol = Calcium.Symbol;
            public const int AtomicNumber = Calcium.AtomicNumber;
            public static IElement Element { get; } = Calcium.Element;
        }
        public static class Scandium
        {
            public const string Name = "Scandium";
            public const string Symbol = "Sc";
            public const int AtomicNumber = 21;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Sc
        {
            public const string Name = Scandium.Name;
            public const string Symbol = Scandium.Symbol;
            public const int AtomicNumber = Scandium.AtomicNumber;
            public static IElement Element { get; } = Scandium.Element;
        }
        public static class Titanium
        {
            public const string Name = "Titanium";
            public const string Symbol = "Ti";
            public const int AtomicNumber = 22;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ti
        {
            public const string Name = Titanium.Name;
            public const string Symbol = Titanium.Symbol;
            public const int AtomicNumber = Titanium.AtomicNumber;
            public static IElement Element { get; } = Titanium.Element;
        }
        public static class Vanadium
        {
            public const string Name = "Vanadium";
            public const string Symbol = "V";
            public const int AtomicNumber = 23;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class V
        {
            public const string Name = Vanadium.Name;
            public const string Symbol = Vanadium.Symbol;
            public const int AtomicNumber = Vanadium.AtomicNumber;
            public static IElement Element { get; } = Vanadium.Element;
        }
        public static class Chromium
        {
            public const string Name = "Chromium";
            public const string Symbol = "Cr";
            public const int AtomicNumber = 24;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Cr
        {
            public const string Name = Chromium.Name;
            public const string Symbol = Chromium.Symbol;
            public const int AtomicNumber = Chromium.AtomicNumber;
            public static IElement Element { get; } = Chromium.Element;
        }
        public static class Manganese
        {
            public const string Name = "Manganese";
            public const string Symbol = "Mn";
            public const int AtomicNumber = 25;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Mn
        {
            public const string Name = Manganese.Name;
            public const string Symbol = Manganese.Symbol;
            public const int AtomicNumber = Manganese.AtomicNumber;
            public static IElement Element { get; } = Manganese.Element;
        }
        public static class Iron
        {
            public const string Name = "Iron";
            public const string Symbol = "Fe";
            public const int AtomicNumber = 26;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Fe
        {
            public const string Name = Iron.Name;
            public const string Symbol = Iron.Symbol;
            public const int AtomicNumber = Iron.AtomicNumber;
            public static IElement Element { get; } = Iron.Element;
        }
        public static class Cobalt
        {
            public const string Name = "Cobalt";
            public const string Symbol = "Co";
            public const int AtomicNumber = 27;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Co
        {
            public const string Name = Cobalt.Name;
            public const string Symbol = Cobalt.Symbol;
            public const int AtomicNumber = Cobalt.AtomicNumber;
            public static IElement Element { get; } = Cobalt.Element;
        }
        public static class Nickel
        {
            public const string Name = "Nickel";
            public const string Symbol = "Ni";
            public const int AtomicNumber = 28;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ni
        {
            public const string Name = Nickel.Name;
            public const string Symbol = Nickel.Symbol;
            public const int AtomicNumber = Nickel.AtomicNumber;
            public static IElement Element { get; } = Nickel.Element;
        }
        public static class Copper
        {
            public const string Name = "Copper";
            public const string Symbol = "Cu";
            public const int AtomicNumber = 29;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Cu
        {
            public const string Name = Copper.Name;
            public const string Symbol = Copper.Symbol;
            public const int AtomicNumber = Copper.AtomicNumber;
            public static IElement Element { get; } = Copper.Element;
        }
        public static class Zinc
        {
            public const string Name = "Zinc";
            public const string Symbol = "Zn";
            public const int AtomicNumber = 30;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Zn
        {
            public const string Name = Zinc.Name;
            public const string Symbol = Zinc.Symbol;
            public const int AtomicNumber = Zinc.AtomicNumber;
            public static IElement Element { get; } = Zinc.Element;
        }
        public static class Gallium
        {
            public const string Name = "Gallium";
            public const string Symbol = "Ga";
            public const int AtomicNumber = 31;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ga
        {
            public const string Name = Gallium.Name;
            public const string Symbol = Gallium.Symbol;
            public const int AtomicNumber = Gallium.AtomicNumber;
            public static IElement Element { get; } = Gallium.Element;
        }
        public static class Germanium
        {
            public const string Name = "Germanium";
            public const string Symbol = "Ge";
            public const int AtomicNumber = 32;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ge
        {
            public const string Name = Germanium.Name;
            public const string Symbol = Germanium.Symbol;
            public const int AtomicNumber = Germanium.AtomicNumber;
            public static IElement Element { get; } = Germanium.Element;
        }
        public static class Arsenic
        {
            public const string Name = "Arsenic";
            public const string Symbol = "As";
            public const int AtomicNumber = 33;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class As
        {
            public const string Name = Arsenic.Name;
            public const string Symbol = Arsenic.Symbol;
            public const int AtomicNumber = Arsenic.AtomicNumber;
            public static IElement Element { get; } = Arsenic.Element;
        }
        public static class Selenium
        {
            public const string Name = "Selenium";
            public const string Symbol = "Se";
            public const int AtomicNumber = 34;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Se
        {
            public const string Name = Selenium.Name;
            public const string Symbol = Selenium.Symbol;
            public const int AtomicNumber = Selenium.AtomicNumber;
            public static IElement Element { get; } = Selenium.Element;
        }
        public static class Bromine
        {
            public const string Name = "Bromine";
            public const string Symbol = "Br";
            public const int AtomicNumber = 35;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Br
        {
            public const string Name = Bromine.Name;
            public const string Symbol = Bromine.Symbol;
            public const int AtomicNumber = Bromine.AtomicNumber;
            public static IElement Element { get; } = Bromine.Element;
        }
        public static class Krypton
        {
            public const string Name = "Krypton";
            public const string Symbol = "Kr";
            public const int AtomicNumber = 36;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Kr
        {
            public const string Name = Krypton.Name;
            public const string Symbol = Krypton.Symbol;
            public const int AtomicNumber = Krypton.AtomicNumber;
            public static IElement Element { get; } = Krypton.Element;
        }
        public static class Rubidium
        {
            public const string Name = "Rubidium";
            public const string Symbol = "Rb";
            public const int AtomicNumber = 37;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Rb
        {
            public const string Name = Rubidium.Name;
            public const string Symbol = Rubidium.Symbol;
            public const int AtomicNumber = Rubidium.AtomicNumber;
            public static IElement Element { get; } = Rubidium.Element;
        }
        public static class Strontium
        {
            public const string Name = "Strontium";
            public const string Symbol = "Sr";
            public const int AtomicNumber = 38;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Sr
        {
            public const string Name = Strontium.Name;
            public const string Symbol = Strontium.Symbol;
            public const int AtomicNumber = Strontium.AtomicNumber;
            public static IElement Element { get; } = Strontium.Element;
        }
        public static class Yttrium
        {
            public const string Name = "Yttrium";
            public const string Symbol = "Y";
            public const int AtomicNumber = 39;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Y
        {
            public const string Name = Yttrium.Name;
            public const string Symbol = Yttrium.Symbol;
            public const int AtomicNumber = Yttrium.AtomicNumber;
            public static IElement Element { get; } = Yttrium.Element;
        }
        public static class Zirconium
        {
            public const string Name = "Zirconium";
            public const string Symbol = "Zr";
            public const int AtomicNumber = 40;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Zr
        {
            public const string Name = Zirconium.Name;
            public const string Symbol = Zirconium.Symbol;
            public const int AtomicNumber = Zirconium.AtomicNumber;
            public static IElement Element { get; } = Zirconium.Element;
        }
        public static class Niobium
        {
            public const string Name = "Niobium";
            public const string Symbol = "Nb";
            public const int AtomicNumber = 41;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Nb
        {
            public const string Name = Niobium.Name;
            public const string Symbol = Niobium.Symbol;
            public const int AtomicNumber = Niobium.AtomicNumber;
            public static IElement Element { get; } = Niobium.Element;
        }
        public static class Molybdenum
        {
            public const string Name = "Molybdenum";
            public const string Symbol = "Mo";
            public const int AtomicNumber = 42;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Mo
        {
            public const string Name = Molybdenum.Name;
            public const string Symbol = Molybdenum.Symbol;
            public const int AtomicNumber = Molybdenum.AtomicNumber;
            public static IElement Element { get; } = Molybdenum.Element;
        }
        public static class Technetium
        {
            public const string Name = "Technetium";
            public const string Symbol = "Tc";
            public const int AtomicNumber = 43;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Tc
        {
            public const string Name = Technetium.Name;
            public const string Symbol = Technetium.Symbol;
            public const int AtomicNumber = Technetium.AtomicNumber;
            public static IElement Element { get; } = Technetium.Element;
        }
        public static class Ruthenium
        {
            public const string Name = "Ruthenium";
            public const string Symbol = "Ru";
            public const int AtomicNumber = 44;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ru
        {
            public const string Name = Ruthenium.Name;
            public const string Symbol = Ruthenium.Symbol;
            public const int AtomicNumber = Ruthenium.AtomicNumber;
            public static IElement Element { get; } = Ruthenium.Element;
        }
        public static class Rhodium
        {
            public const string Name = "Rhodium";
            public const string Symbol = "Rh";
            public const int AtomicNumber = 45;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Rh
        {
            public const string Name = Rhodium.Name;
            public const string Symbol = Rhodium.Symbol;
            public const int AtomicNumber = Rhodium.AtomicNumber;
            public static IElement Element { get; } = Rhodium.Element;
        }
        public static class Palladium
        {
            public const string Name = "Palladium";
            public const string Symbol = "Pd";
            public const int AtomicNumber = 46;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Pd
        {
            public const string Name = Palladium.Name;
            public const string Symbol = Palladium.Symbol;
            public const int AtomicNumber = Palladium.AtomicNumber;
            public static IElement Element { get; } = Palladium.Element;
        }
        public static class Silver
        {
            public const string Name = "Silver";
            public const string Symbol = "Ag";
            public const int AtomicNumber = 47;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ag
        {
            public const string Name = Silver.Name;
            public const string Symbol = Silver.Symbol;
            public const int AtomicNumber = Silver.AtomicNumber;
            public static IElement Element { get; } = Silver.Element;
        }
        public static class Cadmium
        {
            public const string Name = "Cadmium";
            public const string Symbol = "Cd";
            public const int AtomicNumber = 48;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Cd
        {
            public const string Name = Cadmium.Name;
            public const string Symbol = Cadmium.Symbol;
            public const int AtomicNumber = Cadmium.AtomicNumber;
            public static IElement Element { get; } = Cadmium.Element;
        }
        public static class Indium
        {
            public const string Name = "Indium";
            public const string Symbol = "In";
            public const int AtomicNumber = 49;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class In
        {
            public const string Name = Indium.Name;
            public const string Symbol = Indium.Symbol;
            public const int AtomicNumber = Indium.AtomicNumber;
            public static IElement Element { get; } = Indium.Element;
        }
        public static class Tin
        {
            public const string Name = "Tin";
            public const string Symbol = "Sn";
            public const int AtomicNumber = 50;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Sn
        {
            public const string Name = Tin.Name;
            public const string Symbol = Tin.Symbol;
            public const int AtomicNumber = Tin.AtomicNumber;
            public static IElement Element { get; } = Tin.Element;
        }
        public static class Antimony
        {
            public const string Name = "Antimony";
            public const string Symbol = "Sb";
            public const int AtomicNumber = 51;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Sb
        {
            public const string Name = Antimony.Name;
            public const string Symbol = Antimony.Symbol;
            public const int AtomicNumber = Antimony.AtomicNumber;
            public static IElement Element { get; } = Antimony.Element;
        }
        public static class Tellurium
        {
            public const string Name = "Tellurium";
            public const string Symbol = "Te";
            public const int AtomicNumber = 52;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Te
        {
            public const string Name = Tellurium.Name;
            public const string Symbol = Tellurium.Symbol;
            public const int AtomicNumber = Tellurium.AtomicNumber;
            public static IElement Element { get; } = Tellurium.Element;
        }
        public static class Iodine
        {
            public const string Name = "Iodine";
            public const string Symbol = "I";
            public const int AtomicNumber = 53;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class I
        {
            public const string Name = Iodine.Name;
            public const string Symbol = Iodine.Symbol;
            public const int AtomicNumber = Iodine.AtomicNumber;
            public static IElement Element { get; } = Iodine.Element;
        }
        public static class Xenon
        {
            public const string Name = "Xenon";
            public const string Symbol = "Xe";
            public const int AtomicNumber = 54;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Xe
        {
            public const string Name = Xenon.Name;
            public const string Symbol = Xenon.Symbol;
            public const int AtomicNumber = Xenon.AtomicNumber;
            public static IElement Element { get; } = Xenon.Element;
        }
        public static class Caesium
        {
            public const string Name = "Caesium";
            public const string Symbol = "Cs";
            public const int AtomicNumber = 55;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Cs
        {
            public const string Name = Caesium.Name;
            public const string Symbol = Caesium.Symbol;
            public const int AtomicNumber = Caesium.AtomicNumber;
            public static IElement Element { get; } = Caesium.Element;
        }
        public static class Barium
        {
            public const string Name = "Barium";
            public const string Symbol = "Ba";
            public const int AtomicNumber = 56;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ba
        {
            public const string Name = Barium.Name;
            public const string Symbol = Barium.Symbol;
            public const int AtomicNumber = Barium.AtomicNumber;
            public static IElement Element { get; } = Barium.Element;
        }
        public static class Lanthanum
        {
            public const string Name = "Lanthanum";
            public const string Symbol = "La";
            public const int AtomicNumber = 57;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class La
        {
            public const string Name = Lanthanum.Name;
            public const string Symbol = Lanthanum.Symbol;
            public const int AtomicNumber = Lanthanum.AtomicNumber;
            public static IElement Element { get; } = Lanthanum.Element;
        }
        public static class Cerium
        {
            public const string Name = "Cerium";
            public const string Symbol = "Ce";
            public const int AtomicNumber = 58;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ce
        {
            public const string Name = Cerium.Name;
            public const string Symbol = Cerium.Symbol;
            public const int AtomicNumber = Cerium.AtomicNumber;
            public static IElement Element { get; } = Cerium.Element;
        }
        public static class Praseodymium
        {
            public const string Name = "Praseodymium";
            public const string Symbol = "Pr";
            public const int AtomicNumber = 59;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Pr
        {
            public const string Name = Praseodymium.Name;
            public const string Symbol = Praseodymium.Symbol;
            public const int AtomicNumber = Praseodymium.AtomicNumber;
            public static IElement Element { get; } = Praseodymium.Element;
        }
        public static class Neodymium
        {
            public const string Name = "Neodymium";
            public const string Symbol = "Nd";
            public const int AtomicNumber = 60;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Nd
        {
            public const string Name = Neodymium.Name;
            public const string Symbol = Neodymium.Symbol;
            public const int AtomicNumber = Neodymium.AtomicNumber;
            public static IElement Element { get; } = Neodymium.Element;
        }
        public static class Promethium
        {
            public const string Name = "Promethium";
            public const string Symbol = "Pm";
            public const int AtomicNumber = 61;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Pm
        {
            public const string Name = Promethium.Name;
            public const string Symbol = Promethium.Symbol;
            public const int AtomicNumber = Promethium.AtomicNumber;
            public static IElement Element { get; } = Promethium.Element;
        }
        public static class Samarium
        {
            public const string Name = "Samarium";
            public const string Symbol = "Sm";
            public const int AtomicNumber = 62;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Sm
        {
            public const string Name = Samarium.Name;
            public const string Symbol = Samarium.Symbol;
            public const int AtomicNumber = Samarium.AtomicNumber;
            public static IElement Element { get; } = Samarium.Element;
        }
        public static class Europium
        {
            public const string Name = "Europium";
            public const string Symbol = "Eu";
            public const int AtomicNumber = 63;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Eu
        {
            public const string Name = Europium.Name;
            public const string Symbol = Europium.Symbol;
            public const int AtomicNumber = Europium.AtomicNumber;
            public static IElement Element { get; } = Europium.Element;
        }
        public static class Gadolinium
        {
            public const string Name = "Gadolinium";
            public const string Symbol = "Gd";
            public const int AtomicNumber = 64;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Gd
        {
            public const string Name = Gadolinium.Name;
            public const string Symbol = Gadolinium.Symbol;
            public const int AtomicNumber = Gadolinium.AtomicNumber;
            public static IElement Element { get; } = Gadolinium.Element;
        }
        public static class Terbium
        {
            public const string Name = "Terbium";
            public const string Symbol = "Tb";
            public const int AtomicNumber = 65;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Tb
        {
            public const string Name = Terbium.Name;
            public const string Symbol = Terbium.Symbol;
            public const int AtomicNumber = Terbium.AtomicNumber;
            public static IElement Element { get; } = Terbium.Element;
        }
        public static class Dysprosium
        {
            public const string Name = "Dysprosium";
            public const string Symbol = "Dy";
            public const int AtomicNumber = 66;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Dy
        {
            public const string Name = Dysprosium.Name;
            public const string Symbol = Dysprosium.Symbol;
            public const int AtomicNumber = Dysprosium.AtomicNumber;
            public static IElement Element { get; } = Dysprosium.Element;
        }
        public static class Holmium
        {
            public const string Name = "Holmium";
            public const string Symbol = "Ho";
            public const int AtomicNumber = 67;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ho
        {
            public const string Name = Holmium.Name;
            public const string Symbol = Holmium.Symbol;
            public const int AtomicNumber = Holmium.AtomicNumber;
            public static IElement Element { get; } = Holmium.Element;
        }
        public static class Erbium
        {
            public const string Name = "Erbium";
            public const string Symbol = "Er";
            public const int AtomicNumber = 68;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Er
        {
            public const string Name = Erbium.Name;
            public const string Symbol = Erbium.Symbol;
            public const int AtomicNumber = Erbium.AtomicNumber;
            public static IElement Element { get; } = Erbium.Element;
        }
        public static class Thulium
        {
            public const string Name = "Thulium";
            public const string Symbol = "Tm";
            public const int AtomicNumber = 69;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Tm
        {
            public const string Name = Thulium.Name;
            public const string Symbol = Thulium.Symbol;
            public const int AtomicNumber = Thulium.AtomicNumber;
            public static IElement Element { get; } = Thulium.Element;
        }
        public static class Ytterbium
        {
            public const string Name = "Ytterbium";
            public const string Symbol = "Yb";
            public const int AtomicNumber = 70;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Yb
        {
            public const string Name = Ytterbium.Name;
            public const string Symbol = Ytterbium.Symbol;
            public const int AtomicNumber = Ytterbium.AtomicNumber;
            public static IElement Element { get; } = Ytterbium.Element;
        }
        public static class Lutetium
        {
            public const string Name = "Lutetium";
            public const string Symbol = "Lu";
            public const int AtomicNumber = 71;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Lu
        {
            public const string Name = Lutetium.Name;
            public const string Symbol = Lutetium.Symbol;
            public const int AtomicNumber = Lutetium.AtomicNumber;
            public static IElement Element { get; } = Lutetium.Element;
        }
        public static class Hafnium
        {
            public const string Name = "Hafnium";
            public const string Symbol = "Hf";
            public const int AtomicNumber = 72;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Hf
        {
            public const string Name = Hafnium.Name;
            public const string Symbol = Hafnium.Symbol;
            public const int AtomicNumber = Hafnium.AtomicNumber;
            public static IElement Element { get; } = Hafnium.Element;
        }
        public static class Tantalum
        {
            public const string Name = "Tantalum";
            public const string Symbol = "Ta";
            public const int AtomicNumber = 73;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ta
        {
            public const string Name = Tantalum.Name;
            public const string Symbol = Tantalum.Symbol;
            public const int AtomicNumber = Tantalum.AtomicNumber;
            public static IElement Element { get; } = Tantalum.Element;
        }
        public static class Tungsten
        {
            public const string Name = "Tungsten";
            public const string Symbol = "W";
            public const int AtomicNumber = 74;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class W
        {
            public const string Name = Tungsten.Name;
            public const string Symbol = Tungsten.Symbol;
            public const int AtomicNumber = Tungsten.AtomicNumber;
            public static IElement Element { get; } = Tungsten.Element;
        }
        public static class Rhenium
        {
            public const string Name = "Rhenium";
            public const string Symbol = "Re";
            public const int AtomicNumber = 75;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Re
        {
            public const string Name = Rhenium.Name;
            public const string Symbol = Rhenium.Symbol;
            public const int AtomicNumber = Rhenium.AtomicNumber;
            public static IElement Element { get; } = Rhenium.Element;
        }
        public static class Osmium
        {
            public const string Name = "Osmium";
            public const string Symbol = "Os";
            public const int AtomicNumber = 76;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Os
        {
            public const string Name = Osmium.Name;
            public const string Symbol = Osmium.Symbol;
            public const int AtomicNumber = Osmium.AtomicNumber;
            public static IElement Element { get; } = Osmium.Element;
        }
        public static class Iridium
        {
            public const string Name = "Iridium";
            public const string Symbol = "Ir";
            public const int AtomicNumber = 77;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ir
        {
            public const string Name = Iridium.Name;
            public const string Symbol = Iridium.Symbol;
            public const int AtomicNumber = Iridium.AtomicNumber;
            public static IElement Element { get; } = Iridium.Element;
        }
        public static class Platinum
        {
            public const string Name = "Platinum";
            public const string Symbol = "Pt";
            public const int AtomicNumber = 78;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Pt
        {
            public const string Name = Platinum.Name;
            public const string Symbol = Platinum.Symbol;
            public const int AtomicNumber = Platinum.AtomicNumber;
            public static IElement Element { get; } = Platinum.Element;
        }
        public static class Gold
        {
            public const string Name = "Gold";
            public const string Symbol = "Au";
            public const int AtomicNumber = 79;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Au
        {
            public const string Name = Gold.Name;
            public const string Symbol = Gold.Symbol;
            public const int AtomicNumber = Gold.AtomicNumber;
            public static IElement Element { get; } = Gold.Element;
        }
        public static class Mercury
        {
            public const string Name = "Mercury";
            public const string Symbol = "Hg";
            public const int AtomicNumber = 80;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Hg
        {
            public const string Name = Mercury.Name;
            public const string Symbol = Mercury.Symbol;
            public const int AtomicNumber = Mercury.AtomicNumber;
            public static IElement Element { get; } = Mercury.Element;
        }
        public static class Thallium
        {
            public const string Name = "Thallium";
            public const string Symbol = "Tl";
            public const int AtomicNumber = 81;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Tl
        {
            public const string Name = Thallium.Name;
            public const string Symbol = Thallium.Symbol;
            public const int AtomicNumber = Thallium.AtomicNumber;
            public static IElement Element { get; } = Thallium.Element;
        }
        public static class Lead
        {
            public const string Name = "Lead";
            public const string Symbol = "Pb";
            public const int AtomicNumber = 82;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Pb
        {
            public const string Name = Lead.Name;
            public const string Symbol = Lead.Symbol;
            public const int AtomicNumber = Lead.AtomicNumber;
            public static IElement Element { get; } = Lead.Element;
        }
        public static class Bismuth
        {
            public const string Name = "Bismuth";
            public const string Symbol = "Bi";
            public const int AtomicNumber = 83;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Bi
        {
            public const string Name = Bismuth.Name;
            public const string Symbol = Bismuth.Symbol;
            public const int AtomicNumber = Bismuth.AtomicNumber;
            public static IElement Element { get; } = Bismuth.Element;
        }
        public static class Polonium
        {
            public const string Name = "Polonium";
            public const string Symbol = "Po";
            public const int AtomicNumber = 84;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Po
        {
            public const string Name = Polonium.Name;
            public const string Symbol = Polonium.Symbol;
            public const int AtomicNumber = Polonium.AtomicNumber;
            public static IElement Element { get; } = Polonium.Element;
        }
        public static class Astatine
        {
            public const string Name = "Astatine";
            public const string Symbol = "At";
            public const int AtomicNumber = 85;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class At
        {
            public const string Name = Astatine.Name;
            public const string Symbol = Astatine.Symbol;
            public const int AtomicNumber = Astatine.AtomicNumber;
            public static IElement Element { get; } = Astatine.Element;
        }
        public static class Radon
        {
            public const string Name = "Radon";
            public const string Symbol = "Rn";
            public const int AtomicNumber = 86;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Rn
        {
            public const string Name = Radon.Name;
            public const string Symbol = Radon.Symbol;
            public const int AtomicNumber = Radon.AtomicNumber;
            public static IElement Element { get; } = Radon.Element;
        }
        public static class Francium
        {
            public const string Name = "Francium";
            public const string Symbol = "Fr";
            public const int AtomicNumber = 87;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Fr
        {
            public const string Name = Francium.Name;
            public const string Symbol = Francium.Symbol;
            public const int AtomicNumber = Francium.AtomicNumber;
            public static IElement Element { get; } = Francium.Element;
        }
        public static class Radium
        {
            public const string Name = "Radium";
            public const string Symbol = "Ra";
            public const int AtomicNumber = 88;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ra
        {
            public const string Name = Radium.Name;
            public const string Symbol = Radium.Symbol;
            public const int AtomicNumber = Radium.AtomicNumber;
            public static IElement Element { get; } = Radium.Element;
        }
        public static class Actinium
        {
            public const string Name = "Actinium";
            public const string Symbol = "Ac";
            public const int AtomicNumber = 89;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ac
        {
            public const string Name = Actinium.Name;
            public const string Symbol = Actinium.Symbol;
            public const int AtomicNumber = Actinium.AtomicNumber;
            public static IElement Element { get; } = Actinium.Element;
        }
        public static class Thorium
        {
            public const string Name = "Thorium";
            public const string Symbol = "Th";
            public const int AtomicNumber = 90;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Th
        {
            public const string Name = Thorium.Name;
            public const string Symbol = Thorium.Symbol;
            public const int AtomicNumber = Thorium.AtomicNumber;
            public static IElement Element { get; } = Thorium.Element;
        }
        public static class Protactinium
        {
            public const string Name = "Protactinium";
            public const string Symbol = "Pa";
            public const int AtomicNumber = 91;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Pa
        {
            public const string Name = Protactinium.Name;
            public const string Symbol = Protactinium.Symbol;
            public const int AtomicNumber = Protactinium.AtomicNumber;
            public static IElement Element { get; } = Protactinium.Element;
        }
        public static class Uranium
        {
            public const string Name = "Uranium";
            public const string Symbol = "U";
            public const int AtomicNumber = 92;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class U
        {
            public const string Name = Uranium.Name;
            public const string Symbol = Uranium.Symbol;
            public const int AtomicNumber = Uranium.AtomicNumber;
            public static IElement Element { get; } = Uranium.Element;
        }
        public static class Neptunium
        {
            public const string Name = "Neptunium";
            public const string Symbol = "Np";
            public const int AtomicNumber = 93;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Np
        {
            public const string Name = Neptunium.Name;
            public const string Symbol = Neptunium.Symbol;
            public const int AtomicNumber = Neptunium.AtomicNumber;
            public static IElement Element { get; } = Neptunium.Element;
        }
        public static class Plutonium
        {
            public const string Name = "Plutonium";
            public const string Symbol = "Pu";
            public const int AtomicNumber = 94;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Pu
        {
            public const string Name = Plutonium.Name;
            public const string Symbol = Plutonium.Symbol;
            public const int AtomicNumber = Plutonium.AtomicNumber;
            public static IElement Element { get; } = Plutonium.Element;
        }
        public static class Americium
        {
            public const string Name = "Americium";
            public const string Symbol = "Am";
            public const int AtomicNumber = 95;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Am
        {
            public const string Name = Americium.Name;
            public const string Symbol = Americium.Symbol;
            public const int AtomicNumber = Americium.AtomicNumber;
            public static IElement Element { get; } = Americium.Element;
        }
        public static class Curium
        {
            public const string Name = "Curium";
            public const string Symbol = "Cm";
            public const int AtomicNumber = 96;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Cm
        {
            public const string Name = Curium.Name;
            public const string Symbol = Curium.Symbol;
            public const int AtomicNumber = Curium.AtomicNumber;
            public static IElement Element { get; } = Curium.Element;
        }
        public static class Berkelium
        {
            public const string Name = "Berkelium";
            public const string Symbol = "Bk";
            public const int AtomicNumber = 97;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Bk
        {
            public const string Name = Berkelium.Name;
            public const string Symbol = Berkelium.Symbol;
            public const int AtomicNumber = Berkelium.AtomicNumber;
            public static IElement Element { get; } = Berkelium.Element;
        }
        public static class Californium
        {
            public const string Name = "Californium";
            public const string Symbol = "Cf";
            public const int AtomicNumber = 98;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Cf
        {
            public const string Name = Californium.Name;
            public const string Symbol = Californium.Symbol;
            public const int AtomicNumber = Californium.AtomicNumber;
            public static IElement Element { get; } = Californium.Element;
        }
        public static class Einsteinium
        {
            public const string Name = "Einsteinium";
            public const string Symbol = "Es";
            public const int AtomicNumber = 99;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Es
        {
            public const string Name = Einsteinium.Name;
            public const string Symbol = Einsteinium.Symbol;
            public const int AtomicNumber = Einsteinium.AtomicNumber;
            public static IElement Element { get; } = Einsteinium.Element;
        }
        public static class Fermium
        {
            public const string Name = "Fermium";
            public const string Symbol = "Fm";
            public const int AtomicNumber = 100;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Fm
        {
            public const string Name = Fermium.Name;
            public const string Symbol = Fermium.Symbol;
            public const int AtomicNumber = Fermium.AtomicNumber;
            public static IElement Element { get; } = Fermium.Element;
        }
        public static class Mendelevium
        {
            public const string Name = "Mendelevium";
            public const string Symbol = "Md";
            public const int AtomicNumber = 101;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Md
        {
            public const string Name = Mendelevium.Name;
            public const string Symbol = Mendelevium.Symbol;
            public const int AtomicNumber = Mendelevium.AtomicNumber;
            public static IElement Element { get; } = Mendelevium.Element;
        }
        public static class Nobelium
        {
            public const string Name = "Nobelium";
            public const string Symbol = "No";
            public const int AtomicNumber = 102;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class No
        {
            public const string Name = Nobelium.Name;
            public const string Symbol = Nobelium.Symbol;
            public const int AtomicNumber = Nobelium.AtomicNumber;
            public static IElement Element { get; } = Nobelium.Element;
        }
        public static class Lawrencium
        {
            public const string Name = "Lawrencium";
            public const string Symbol = "Lr";
            public const int AtomicNumber = 103;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Lr
        {
            public const string Name = Lawrencium.Name;
            public const string Symbol = Lawrencium.Symbol;
            public const int AtomicNumber = Lawrencium.AtomicNumber;
            public static IElement Element { get; } = Lawrencium.Element;
        }
        public static class Rutherfordium
        {
            public const string Name = "Rutherfordium";
            public const string Symbol = "Rf";
            public const int AtomicNumber = 104;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Rf
        {
            public const string Name = Rutherfordium.Name;
            public const string Symbol = Rutherfordium.Symbol;
            public const int AtomicNumber = Rutherfordium.AtomicNumber;
            public static IElement Element { get; } = Rutherfordium.Element;
        }
        public static class Dubnium
        {
            public const string Name = "Dubnium";
            public const string Symbol = "Db";
            public const int AtomicNumber = 105;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Db
        {
            public const string Name = Dubnium.Name;
            public const string Symbol = Dubnium.Symbol;
            public const int AtomicNumber = Dubnium.AtomicNumber;
            public static IElement Element { get; } = Dubnium.Element;
        }
        public static class Seaborgium
        {
            public const string Name = "Seaborgium";
            public const string Symbol = "Sg";
            public const int AtomicNumber = 106;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Sg
        {
            public const string Name = Seaborgium.Name;
            public const string Symbol = Seaborgium.Symbol;
            public const int AtomicNumber = Seaborgium.AtomicNumber;
            public static IElement Element { get; } = Seaborgium.Element;
        }
        public static class Bohrium
        {
            public const string Name = "Bohrium";
            public const string Symbol = "Bh";
            public const int AtomicNumber = 107;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Bh
        {
            public const string Name = Bohrium.Name;
            public const string Symbol = Bohrium.Symbol;
            public const int AtomicNumber = Bohrium.AtomicNumber;
            public static IElement Element { get; } = Bohrium.Element;
        }
        public static class Hassium
        {
            public const string Name = "Hassium";
            public const string Symbol = "Hs";
            public const int AtomicNumber = 108;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Hs
        {
            public const string Name = Hassium.Name;
            public const string Symbol = Hassium.Symbol;
            public const int AtomicNumber = Hassium.AtomicNumber;
            public static IElement Element { get; } = Hassium.Element;
        }
        public static class Meitnerium
        {
            public const string Name = "Meitnerium";
            public const string Symbol = "Mt";
            public const int AtomicNumber = 109;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Mt
        {
            public const string Name = Meitnerium.Name;
            public const string Symbol = Meitnerium.Symbol;
            public const int AtomicNumber = Meitnerium.AtomicNumber;
            public static IElement Element { get; } = Meitnerium.Element;
        }
        public static class Darmstadtium
        {
            public const string Name = "Darmstadtium";
            public const string Symbol = "Ds";
            public const int AtomicNumber = 110;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ds
        {
            public const string Name = Darmstadtium.Name;
            public const string Symbol = Darmstadtium.Symbol;
            public const int AtomicNumber = Darmstadtium.AtomicNumber;
            public static IElement Element { get; } = Darmstadtium.Element;
        }
        public static class Roentgenium
        {
            public const string Name = "Roentgenium";
            public const string Symbol = "Rg";
            public const int AtomicNumber = 111;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Rg
        {
            public const string Name = Roentgenium.Name;
            public const string Symbol = Roentgenium.Symbol;
            public const int AtomicNumber = Roentgenium.AtomicNumber;
            public static IElement Element { get; } = Roentgenium.Element;
        }
        public static class Copernicium
        {
            public const string Name = "Copernicium";
            public const string Symbol = "Cn";
            public const int AtomicNumber = 112;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Cn
        {
            public const string Name = Copernicium.Name;
            public const string Symbol = Copernicium.Symbol;
            public const int AtomicNumber = Copernicium.AtomicNumber;
            public static IElement Element { get; } = Copernicium.Element;
        }
        [Obsolete("Use " + nameof(Nihonium))]
        public static class Ununtrium
        {
            public const string Name = "Ununtrium";
            public const string Symbol = "Uut";
            public const int AtomicNumber = 113;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        [Obsolete("Use " + nameof(Nihonium))]
        public static class Uut
        {
            public const string Name = Ununtrium.Name;
            public const string Symbol = Ununtrium.Symbol;
            public const int AtomicNumber = Ununtrium.AtomicNumber;
            public static IElement Element { get; } = Ununtrium.Element;
        }
        public static class Nihonium
        {
            public const string Name = "Nihonium";
            public const string Symbol = "Nh";
            public const int AtomicNumber = 113;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Nh
        {
            public const string Name = Nihonium.Name;
            public const string Symbol = Nihonium.Symbol;
            public const int AtomicNumber = Nihonium.AtomicNumber;
            public static IElement Element { get; } = Nihonium.Element;
        }
        public static class Flerovium
        {
            public const string Name = "Flerovium";
            public const string Symbol = "Fl";
            public const int AtomicNumber = 114;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Fl
        {
            public const string Name = Flerovium.Name;
            public const string Symbol = Flerovium.Symbol;
            public const int AtomicNumber = Flerovium.AtomicNumber;
            public static IElement Element { get; } = Flerovium.Element;
        }
        [Obsolete("Use " + nameof(Moscovium))]
        public static class Ununpentium
        {
            public const string Name = "Ununpentium";
            public const string Symbol = "Uup";
            public const int AtomicNumber = 115;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        [Obsolete("Use " + nameof(Moscovium))]
        public static class Uup
        {
            public const string Name = Ununpentium.Name;
            public const string Symbol = Ununpentium.Symbol;
            public const int AtomicNumber = Ununpentium.AtomicNumber;
            public static IElement Element { get; } = Ununpentium.Element;
        }
        public static class Moscovium
        {
            public const string Name = "Moscovium";
            public const string Symbol = "Mc";
            public const int AtomicNumber = 115;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Mc
        {
            public const string Name = Moscovium.Name;
            public const string Symbol = Moscovium.Symbol;
            public const int AtomicNumber = Moscovium.AtomicNumber;
            public static IElement Element { get; } = Moscovium.Element;
        }
        public static class Livermorium
        {
            public const string Name = "Livermorium";
            public const string Symbol = "Lv";
            public const int AtomicNumber = 116;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Lv
        {
            public const string Name = Livermorium.Name;
            public const string Symbol = Livermorium.Symbol;
            public const int AtomicNumber = Livermorium.AtomicNumber;
            public static IElement Element { get; } = Livermorium.Element;
        }
        [Obsolete("Use " + nameof(Tennessine))]
        public static class Ununseptium
        {
            public const string Name = "Ununseptium";
            public const string Symbol = "Uus";
            public const int AtomicNumber = 117;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        [Obsolete("Use " + nameof(Tennessine))]
        public static class Uus
        {
            public const string Name = Ununseptium.Name;
            public const string Symbol = Ununseptium.Symbol;
            public const int AtomicNumber = Ununseptium.AtomicNumber;
            public static IElement Element { get; } = Ununseptium.Element;
        }
        public static class Tennessine
        {
            public const string Name = "Tennessine";
            public const string Symbol = "Ts";
            public const int AtomicNumber = 117;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Ts
        {
            public const string Name = Tennessine.Name;
            public const string Symbol = Tennessine.Symbol;
            public const int AtomicNumber = Tennessine.AtomicNumber;
            public static IElement Element { get; } = Tennessine.Element;
        }
        [Obsolete("Use " + nameof(Oganesson))]
        public static class Ununoctium
        {
            public const string Name = "Ununoctium";
            public const string Symbol = "Uuo";
            public const int AtomicNumber = 118;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        [Obsolete("Use " + nameof(Oganesson))]
        public static class Uuo
        {
            public const string Name = Ununoctium.Name;
            public const string Symbol = Ununoctium.Symbol;
            public const int AtomicNumber = Ununoctium.AtomicNumber;
            public static IElement Element { get; } = Ununoctium.Element;
        }
        public static class Oganesson
        {
            public const string Name = "Oganesson";
            public const string Symbol = "Og";
            public const int AtomicNumber = 118;
            public static IElement Element { get; } = new ImmutableElement(Symbol, AtomicNumber);
        }
        public static class Og
        {
            public const string Name = Oganesson.Name;
            public const string Symbol = Oganesson.Symbol;
            public const int AtomicNumber = Oganesson.AtomicNumber;
            public static IElement Element { get; } = Oganesson.Element;
        }
    }
}

namespace NCDK.Config
{
    public static partial class NaturalElement
    {
        /// <summary>
        /// Lookup elements by symbol / name.
        /// </summary>
        internal static IReadOnlyDictionary<string, int> SymbolMap { get; } = new Dictionary<string, int>()
            {
                ["unknown"] = NaturalElements.Unknown.AtomicNumber,
                [""] = NaturalElements.Unknown.AtomicNumber,
                ["hydrogen"] = NaturalElements.Hydrogen.AtomicNumber,
                ["h"] = NaturalElements.Hydrogen.AtomicNumber,
                ["helium"] = NaturalElements.Helium.AtomicNumber,
                ["he"] = NaturalElements.Helium.AtomicNumber,
                ["lithium"] = NaturalElements.Lithium.AtomicNumber,
                ["li"] = NaturalElements.Lithium.AtomicNumber,
                ["beryllium"] = NaturalElements.Beryllium.AtomicNumber,
                ["be"] = NaturalElements.Beryllium.AtomicNumber,
                ["boron"] = NaturalElements.Boron.AtomicNumber,
                ["b"] = NaturalElements.Boron.AtomicNumber,
                ["carbon"] = NaturalElements.Carbon.AtomicNumber,
                ["c"] = NaturalElements.Carbon.AtomicNumber,
                ["nitrogen"] = NaturalElements.Nitrogen.AtomicNumber,
                ["n"] = NaturalElements.Nitrogen.AtomicNumber,
                ["oxygen"] = NaturalElements.Oxygen.AtomicNumber,
                ["o"] = NaturalElements.Oxygen.AtomicNumber,
                ["fluorine"] = NaturalElements.Fluorine.AtomicNumber,
                ["f"] = NaturalElements.Fluorine.AtomicNumber,
                ["neon"] = NaturalElements.Neon.AtomicNumber,
                ["ne"] = NaturalElements.Neon.AtomicNumber,
                ["sodium"] = NaturalElements.Sodium.AtomicNumber,
                ["na"] = NaturalElements.Sodium.AtomicNumber,
                ["magnesium"] = NaturalElements.Magnesium.AtomicNumber,
                ["mg"] = NaturalElements.Magnesium.AtomicNumber,
                ["aluminium"] = NaturalElements.Aluminium.AtomicNumber,
                ["al"] = NaturalElements.Aluminium.AtomicNumber,
                ["silicon"] = NaturalElements.Silicon.AtomicNumber,
                ["si"] = NaturalElements.Silicon.AtomicNumber,
                ["phosphorus"] = NaturalElements.Phosphorus.AtomicNumber,
                ["p"] = NaturalElements.Phosphorus.AtomicNumber,
                ["sulfur"] = NaturalElements.Sulfur.AtomicNumber,
                ["s"] = NaturalElements.Sulfur.AtomicNumber,
                ["chlorine"] = NaturalElements.Chlorine.AtomicNumber,
                ["cl"] = NaturalElements.Chlorine.AtomicNumber,
                ["argon"] = NaturalElements.Argon.AtomicNumber,
                ["ar"] = NaturalElements.Argon.AtomicNumber,
                ["potassium"] = NaturalElements.Potassium.AtomicNumber,
                ["k"] = NaturalElements.Potassium.AtomicNumber,
                ["calcium"] = NaturalElements.Calcium.AtomicNumber,
                ["ca"] = NaturalElements.Calcium.AtomicNumber,
                ["scandium"] = NaturalElements.Scandium.AtomicNumber,
                ["sc"] = NaturalElements.Scandium.AtomicNumber,
                ["titanium"] = NaturalElements.Titanium.AtomicNumber,
                ["ti"] = NaturalElements.Titanium.AtomicNumber,
                ["vanadium"] = NaturalElements.Vanadium.AtomicNumber,
                ["v"] = NaturalElements.Vanadium.AtomicNumber,
                ["chromium"] = NaturalElements.Chromium.AtomicNumber,
                ["cr"] = NaturalElements.Chromium.AtomicNumber,
                ["manganese"] = NaturalElements.Manganese.AtomicNumber,
                ["mn"] = NaturalElements.Manganese.AtomicNumber,
                ["iron"] = NaturalElements.Iron.AtomicNumber,
                ["fe"] = NaturalElements.Iron.AtomicNumber,
                ["cobalt"] = NaturalElements.Cobalt.AtomicNumber,
                ["co"] = NaturalElements.Cobalt.AtomicNumber,
                ["nickel"] = NaturalElements.Nickel.AtomicNumber,
                ["ni"] = NaturalElements.Nickel.AtomicNumber,
                ["copper"] = NaturalElements.Copper.AtomicNumber,
                ["cu"] = NaturalElements.Copper.AtomicNumber,
                ["zinc"] = NaturalElements.Zinc.AtomicNumber,
                ["zn"] = NaturalElements.Zinc.AtomicNumber,
                ["gallium"] = NaturalElements.Gallium.AtomicNumber,
                ["ga"] = NaturalElements.Gallium.AtomicNumber,
                ["germanium"] = NaturalElements.Germanium.AtomicNumber,
                ["ge"] = NaturalElements.Germanium.AtomicNumber,
                ["arsenic"] = NaturalElements.Arsenic.AtomicNumber,
                ["as"] = NaturalElements.Arsenic.AtomicNumber,
                ["selenium"] = NaturalElements.Selenium.AtomicNumber,
                ["se"] = NaturalElements.Selenium.AtomicNumber,
                ["bromine"] = NaturalElements.Bromine.AtomicNumber,
                ["br"] = NaturalElements.Bromine.AtomicNumber,
                ["krypton"] = NaturalElements.Krypton.AtomicNumber,
                ["kr"] = NaturalElements.Krypton.AtomicNumber,
                ["rubidium"] = NaturalElements.Rubidium.AtomicNumber,
                ["rb"] = NaturalElements.Rubidium.AtomicNumber,
                ["strontium"] = NaturalElements.Strontium.AtomicNumber,
                ["sr"] = NaturalElements.Strontium.AtomicNumber,
                ["yttrium"] = NaturalElements.Yttrium.AtomicNumber,
                ["y"] = NaturalElements.Yttrium.AtomicNumber,
                ["zirconium"] = NaturalElements.Zirconium.AtomicNumber,
                ["zr"] = NaturalElements.Zirconium.AtomicNumber,
                ["niobium"] = NaturalElements.Niobium.AtomicNumber,
                ["nb"] = NaturalElements.Niobium.AtomicNumber,
                ["molybdenum"] = NaturalElements.Molybdenum.AtomicNumber,
                ["mo"] = NaturalElements.Molybdenum.AtomicNumber,
                ["technetium"] = NaturalElements.Technetium.AtomicNumber,
                ["tc"] = NaturalElements.Technetium.AtomicNumber,
                ["ruthenium"] = NaturalElements.Ruthenium.AtomicNumber,
                ["ru"] = NaturalElements.Ruthenium.AtomicNumber,
                ["rhodium"] = NaturalElements.Rhodium.AtomicNumber,
                ["rh"] = NaturalElements.Rhodium.AtomicNumber,
                ["palladium"] = NaturalElements.Palladium.AtomicNumber,
                ["pd"] = NaturalElements.Palladium.AtomicNumber,
                ["silver"] = NaturalElements.Silver.AtomicNumber,
                ["ag"] = NaturalElements.Silver.AtomicNumber,
                ["cadmium"] = NaturalElements.Cadmium.AtomicNumber,
                ["cd"] = NaturalElements.Cadmium.AtomicNumber,
                ["indium"] = NaturalElements.Indium.AtomicNumber,
                ["in"] = NaturalElements.Indium.AtomicNumber,
                ["tin"] = NaturalElements.Tin.AtomicNumber,
                ["sn"] = NaturalElements.Tin.AtomicNumber,
                ["antimony"] = NaturalElements.Antimony.AtomicNumber,
                ["sb"] = NaturalElements.Antimony.AtomicNumber,
                ["tellurium"] = NaturalElements.Tellurium.AtomicNumber,
                ["te"] = NaturalElements.Tellurium.AtomicNumber,
                ["iodine"] = NaturalElements.Iodine.AtomicNumber,
                ["i"] = NaturalElements.Iodine.AtomicNumber,
                ["xenon"] = NaturalElements.Xenon.AtomicNumber,
                ["xe"] = NaturalElements.Xenon.AtomicNumber,
                ["caesium"] = NaturalElements.Caesium.AtomicNumber,
                ["cs"] = NaturalElements.Caesium.AtomicNumber,
                ["barium"] = NaturalElements.Barium.AtomicNumber,
                ["ba"] = NaturalElements.Barium.AtomicNumber,
                ["lanthanum"] = NaturalElements.Lanthanum.AtomicNumber,
                ["la"] = NaturalElements.Lanthanum.AtomicNumber,
                ["cerium"] = NaturalElements.Cerium.AtomicNumber,
                ["ce"] = NaturalElements.Cerium.AtomicNumber,
                ["praseodymium"] = NaturalElements.Praseodymium.AtomicNumber,
                ["pr"] = NaturalElements.Praseodymium.AtomicNumber,
                ["neodymium"] = NaturalElements.Neodymium.AtomicNumber,
                ["nd"] = NaturalElements.Neodymium.AtomicNumber,
                ["promethium"] = NaturalElements.Promethium.AtomicNumber,
                ["pm"] = NaturalElements.Promethium.AtomicNumber,
                ["samarium"] = NaturalElements.Samarium.AtomicNumber,
                ["sm"] = NaturalElements.Samarium.AtomicNumber,
                ["europium"] = NaturalElements.Europium.AtomicNumber,
                ["eu"] = NaturalElements.Europium.AtomicNumber,
                ["gadolinium"] = NaturalElements.Gadolinium.AtomicNumber,
                ["gd"] = NaturalElements.Gadolinium.AtomicNumber,
                ["terbium"] = NaturalElements.Terbium.AtomicNumber,
                ["tb"] = NaturalElements.Terbium.AtomicNumber,
                ["dysprosium"] = NaturalElements.Dysprosium.AtomicNumber,
                ["dy"] = NaturalElements.Dysprosium.AtomicNumber,
                ["holmium"] = NaturalElements.Holmium.AtomicNumber,
                ["ho"] = NaturalElements.Holmium.AtomicNumber,
                ["erbium"] = NaturalElements.Erbium.AtomicNumber,
                ["er"] = NaturalElements.Erbium.AtomicNumber,
                ["thulium"] = NaturalElements.Thulium.AtomicNumber,
                ["tm"] = NaturalElements.Thulium.AtomicNumber,
                ["ytterbium"] = NaturalElements.Ytterbium.AtomicNumber,
                ["yb"] = NaturalElements.Ytterbium.AtomicNumber,
                ["lutetium"] = NaturalElements.Lutetium.AtomicNumber,
                ["lu"] = NaturalElements.Lutetium.AtomicNumber,
                ["hafnium"] = NaturalElements.Hafnium.AtomicNumber,
                ["hf"] = NaturalElements.Hafnium.AtomicNumber,
                ["tantalum"] = NaturalElements.Tantalum.AtomicNumber,
                ["ta"] = NaturalElements.Tantalum.AtomicNumber,
                ["tungsten"] = NaturalElements.Tungsten.AtomicNumber,
                ["w"] = NaturalElements.Tungsten.AtomicNumber,
                ["rhenium"] = NaturalElements.Rhenium.AtomicNumber,
                ["re"] = NaturalElements.Rhenium.AtomicNumber,
                ["osmium"] = NaturalElements.Osmium.AtomicNumber,
                ["os"] = NaturalElements.Osmium.AtomicNumber,
                ["iridium"] = NaturalElements.Iridium.AtomicNumber,
                ["ir"] = NaturalElements.Iridium.AtomicNumber,
                ["platinum"] = NaturalElements.Platinum.AtomicNumber,
                ["pt"] = NaturalElements.Platinum.AtomicNumber,
                ["gold"] = NaturalElements.Gold.AtomicNumber,
                ["au"] = NaturalElements.Gold.AtomicNumber,
                ["mercury"] = NaturalElements.Mercury.AtomicNumber,
                ["hg"] = NaturalElements.Mercury.AtomicNumber,
                ["thallium"] = NaturalElements.Thallium.AtomicNumber,
                ["tl"] = NaturalElements.Thallium.AtomicNumber,
                ["lead"] = NaturalElements.Lead.AtomicNumber,
                ["pb"] = NaturalElements.Lead.AtomicNumber,
                ["bismuth"] = NaturalElements.Bismuth.AtomicNumber,
                ["bi"] = NaturalElements.Bismuth.AtomicNumber,
                ["polonium"] = NaturalElements.Polonium.AtomicNumber,
                ["po"] = NaturalElements.Polonium.AtomicNumber,
                ["astatine"] = NaturalElements.Astatine.AtomicNumber,
                ["at"] = NaturalElements.Astatine.AtomicNumber,
                ["radon"] = NaturalElements.Radon.AtomicNumber,
                ["rn"] = NaturalElements.Radon.AtomicNumber,
                ["francium"] = NaturalElements.Francium.AtomicNumber,
                ["fr"] = NaturalElements.Francium.AtomicNumber,
                ["radium"] = NaturalElements.Radium.AtomicNumber,
                ["ra"] = NaturalElements.Radium.AtomicNumber,
                ["actinium"] = NaturalElements.Actinium.AtomicNumber,
                ["ac"] = NaturalElements.Actinium.AtomicNumber,
                ["thorium"] = NaturalElements.Thorium.AtomicNumber,
                ["th"] = NaturalElements.Thorium.AtomicNumber,
                ["protactinium"] = NaturalElements.Protactinium.AtomicNumber,
                ["pa"] = NaturalElements.Protactinium.AtomicNumber,
                ["uranium"] = NaturalElements.Uranium.AtomicNumber,
                ["u"] = NaturalElements.Uranium.AtomicNumber,
                ["neptunium"] = NaturalElements.Neptunium.AtomicNumber,
                ["np"] = NaturalElements.Neptunium.AtomicNumber,
                ["plutonium"] = NaturalElements.Plutonium.AtomicNumber,
                ["pu"] = NaturalElements.Plutonium.AtomicNumber,
                ["americium"] = NaturalElements.Americium.AtomicNumber,
                ["am"] = NaturalElements.Americium.AtomicNumber,
                ["curium"] = NaturalElements.Curium.AtomicNumber,
                ["cm"] = NaturalElements.Curium.AtomicNumber,
                ["berkelium"] = NaturalElements.Berkelium.AtomicNumber,
                ["bk"] = NaturalElements.Berkelium.AtomicNumber,
                ["californium"] = NaturalElements.Californium.AtomicNumber,
                ["cf"] = NaturalElements.Californium.AtomicNumber,
                ["einsteinium"] = NaturalElements.Einsteinium.AtomicNumber,
                ["es"] = NaturalElements.Einsteinium.AtomicNumber,
                ["fermium"] = NaturalElements.Fermium.AtomicNumber,
                ["fm"] = NaturalElements.Fermium.AtomicNumber,
                ["mendelevium"] = NaturalElements.Mendelevium.AtomicNumber,
                ["md"] = NaturalElements.Mendelevium.AtomicNumber,
                ["nobelium"] = NaturalElements.Nobelium.AtomicNumber,
                ["no"] = NaturalElements.Nobelium.AtomicNumber,
                ["lawrencium"] = NaturalElements.Lawrencium.AtomicNumber,
                ["lr"] = NaturalElements.Lawrencium.AtomicNumber,
                ["rutherfordium"] = NaturalElements.Rutherfordium.AtomicNumber,
                ["rf"] = NaturalElements.Rutherfordium.AtomicNumber,
                ["dubnium"] = NaturalElements.Dubnium.AtomicNumber,
                ["db"] = NaturalElements.Dubnium.AtomicNumber,
                ["seaborgium"] = NaturalElements.Seaborgium.AtomicNumber,
                ["sg"] = NaturalElements.Seaborgium.AtomicNumber,
                ["bohrium"] = NaturalElements.Bohrium.AtomicNumber,
                ["bh"] = NaturalElements.Bohrium.AtomicNumber,
                ["hassium"] = NaturalElements.Hassium.AtomicNumber,
                ["hs"] = NaturalElements.Hassium.AtomicNumber,
                ["meitnerium"] = NaturalElements.Meitnerium.AtomicNumber,
                ["mt"] = NaturalElements.Meitnerium.AtomicNumber,
                ["darmstadtium"] = NaturalElements.Darmstadtium.AtomicNumber,
                ["ds"] = NaturalElements.Darmstadtium.AtomicNumber,
                ["roentgenium"] = NaturalElements.Roentgenium.AtomicNumber,
                ["rg"] = NaturalElements.Roentgenium.AtomicNumber,
                ["copernicium"] = NaturalElements.Copernicium.AtomicNumber,
                ["cn"] = NaturalElements.Copernicium.AtomicNumber,
                ["ununtrium"] = NaturalElements.Ununtrium.AtomicNumber,
                ["uut"] = NaturalElements.Ununtrium.AtomicNumber,
                ["nihonium"] = NaturalElements.Nihonium.AtomicNumber,
                ["nh"] = NaturalElements.Nihonium.AtomicNumber,
                ["flerovium"] = NaturalElements.Flerovium.AtomicNumber,
                ["fl"] = NaturalElements.Flerovium.AtomicNumber,
                ["ununpentium"] = NaturalElements.Ununpentium.AtomicNumber,
                ["uup"] = NaturalElements.Ununpentium.AtomicNumber,
                ["moscovium"] = NaturalElements.Moscovium.AtomicNumber,
                ["mc"] = NaturalElements.Moscovium.AtomicNumber,
                ["livermorium"] = NaturalElements.Livermorium.AtomicNumber,
                ["lv"] = NaturalElements.Livermorium.AtomicNumber,
                ["ununseptium"] = NaturalElements.Ununseptium.AtomicNumber,
                ["uus"] = NaturalElements.Ununseptium.AtomicNumber,
                ["tennessine"] = NaturalElements.Tennessine.AtomicNumber,
                ["ts"] = NaturalElements.Tennessine.AtomicNumber,
                ["ununoctium"] = NaturalElements.Ununoctium.AtomicNumber,
                ["uuo"] = NaturalElements.Ununoctium.AtomicNumber,
                ["oganesson"] = NaturalElements.Oganesson.AtomicNumber,
                ["og"] = NaturalElements.Oganesson.AtomicNumber,
                // recently named elements
                ["uub"] = NaturalElements.Copernicium.AtomicNumber, // 2009
                ["ununbium"] = NaturalElements.Copernicium.AtomicNumber,
                ["uuq"] = NaturalElements.Flerovium.AtomicNumber, // 2012
                ["ununquadium"] = NaturalElements.Flerovium.AtomicNumber,
                ["uuh"] = NaturalElements.Livermorium.AtomicNumber, // 2012
                ["ununhexium"] = NaturalElements.Livermorium.AtomicNumber,
                // alternative spellings
                ["sulphur"] = NaturalElements.Sulfur.AtomicNumber,
                ["cesium"] = NaturalElements.Caesium.AtomicNumber,
                ["aluminum"] = NaturalElements.Aluminium.AtomicNumber,
            };

        /// <summary>
        /// Lookup elements by atomic number.
        /// </summary>
        public static IReadOnlyList<IElement> Elements { get; } = new IElement[] 
            { 
                NaturalElements.Unknown.Element, 
                NaturalElements.Hydrogen.Element, 
                NaturalElements.Helium.Element, 
                NaturalElements.Lithium.Element, 
                NaturalElements.Beryllium.Element, 
                NaturalElements.Boron.Element, 
                NaturalElements.Carbon.Element, 
                NaturalElements.Nitrogen.Element, 
                NaturalElements.Oxygen.Element, 
                NaturalElements.Fluorine.Element, 
                NaturalElements.Neon.Element, 
                NaturalElements.Sodium.Element, 
                NaturalElements.Magnesium.Element, 
                NaturalElements.Aluminium.Element, 
                NaturalElements.Silicon.Element, 
                NaturalElements.Phosphorus.Element, 
                NaturalElements.Sulfur.Element, 
                NaturalElements.Chlorine.Element, 
                NaturalElements.Argon.Element, 
                NaturalElements.Potassium.Element, 
                NaturalElements.Calcium.Element, 
                NaturalElements.Scandium.Element, 
                NaturalElements.Titanium.Element, 
                NaturalElements.Vanadium.Element, 
                NaturalElements.Chromium.Element, 
                NaturalElements.Manganese.Element, 
                NaturalElements.Iron.Element, 
                NaturalElements.Cobalt.Element, 
                NaturalElements.Nickel.Element, 
                NaturalElements.Copper.Element, 
                NaturalElements.Zinc.Element, 
                NaturalElements.Gallium.Element, 
                NaturalElements.Germanium.Element, 
                NaturalElements.Arsenic.Element, 
                NaturalElements.Selenium.Element, 
                NaturalElements.Bromine.Element, 
                NaturalElements.Krypton.Element, 
                NaturalElements.Rubidium.Element, 
                NaturalElements.Strontium.Element, 
                NaturalElements.Yttrium.Element, 
                NaturalElements.Zirconium.Element, 
                NaturalElements.Niobium.Element, 
                NaturalElements.Molybdenum.Element, 
                NaturalElements.Technetium.Element, 
                NaturalElements.Ruthenium.Element, 
                NaturalElements.Rhodium.Element, 
                NaturalElements.Palladium.Element, 
                NaturalElements.Silver.Element, 
                NaturalElements.Cadmium.Element, 
                NaturalElements.Indium.Element, 
                NaturalElements.Tin.Element, 
                NaturalElements.Antimony.Element, 
                NaturalElements.Tellurium.Element, 
                NaturalElements.Iodine.Element, 
                NaturalElements.Xenon.Element, 
                NaturalElements.Caesium.Element, 
                NaturalElements.Barium.Element, 
                NaturalElements.Lanthanum.Element, 
                NaturalElements.Cerium.Element, 
                NaturalElements.Praseodymium.Element, 
                NaturalElements.Neodymium.Element, 
                NaturalElements.Promethium.Element, 
                NaturalElements.Samarium.Element, 
                NaturalElements.Europium.Element, 
                NaturalElements.Gadolinium.Element, 
                NaturalElements.Terbium.Element, 
                NaturalElements.Dysprosium.Element, 
                NaturalElements.Holmium.Element, 
                NaturalElements.Erbium.Element, 
                NaturalElements.Thulium.Element, 
                NaturalElements.Ytterbium.Element, 
                NaturalElements.Lutetium.Element, 
                NaturalElements.Hafnium.Element, 
                NaturalElements.Tantalum.Element, 
                NaturalElements.Tungsten.Element, 
                NaturalElements.Rhenium.Element, 
                NaturalElements.Osmium.Element, 
                NaturalElements.Iridium.Element, 
                NaturalElements.Platinum.Element, 
                NaturalElements.Gold.Element, 
                NaturalElements.Mercury.Element, 
                NaturalElements.Thallium.Element, 
                NaturalElements.Lead.Element, 
                NaturalElements.Bismuth.Element, 
                NaturalElements.Polonium.Element, 
                NaturalElements.Astatine.Element, 
                NaturalElements.Radon.Element, 
                NaturalElements.Francium.Element, 
                NaturalElements.Radium.Element, 
                NaturalElements.Actinium.Element, 
                NaturalElements.Thorium.Element, 
                NaturalElements.Protactinium.Element, 
                NaturalElements.Uranium.Element, 
                NaturalElements.Neptunium.Element, 
                NaturalElements.Plutonium.Element, 
                NaturalElements.Americium.Element, 
                NaturalElements.Curium.Element, 
                NaturalElements.Berkelium.Element, 
                NaturalElements.Californium.Element, 
                NaturalElements.Einsteinium.Element, 
                NaturalElements.Fermium.Element, 
                NaturalElements.Mendelevium.Element, 
                NaturalElements.Nobelium.Element, 
                NaturalElements.Lawrencium.Element, 
                NaturalElements.Rutherfordium.Element, 
                NaturalElements.Dubnium.Element, 
                NaturalElements.Seaborgium.Element, 
                NaturalElements.Bohrium.Element, 
                NaturalElements.Hassium.Element, 
                NaturalElements.Meitnerium.Element, 
                NaturalElements.Darmstadtium.Element, 
                NaturalElements.Roentgenium.Element, 
                NaturalElements.Copernicium.Element, 
                NaturalElements.Nihonium.Element, 
                NaturalElements.Flerovium.Element, 
                NaturalElements.Moscovium.Element, 
                NaturalElements.Livermorium.Element, 
                NaturalElements.Tennessine.Element, 
                NaturalElements.Oganesson.Element, 
                NaturalElements.Ununtrium.Element, 
                NaturalElements.Ununpentium.Element, 
                NaturalElements.Ununseptium.Element, 
                NaturalElements.Ununoctium.Element, 
            };

        internal static IReadOnlyList<string> Names { get; } = new string[]
            { 
                NaturalElements.Unknown.Name,
                NaturalElements.Hydrogen.Name,
                NaturalElements.Helium.Name,
                NaturalElements.Lithium.Name,
                NaturalElements.Beryllium.Name,
                NaturalElements.Boron.Name,
                NaturalElements.Carbon.Name,
                NaturalElements.Nitrogen.Name,
                NaturalElements.Oxygen.Name,
                NaturalElements.Fluorine.Name,
                NaturalElements.Neon.Name,
                NaturalElements.Sodium.Name,
                NaturalElements.Magnesium.Name,
                NaturalElements.Aluminium.Name,
                NaturalElements.Silicon.Name,
                NaturalElements.Phosphorus.Name,
                NaturalElements.Sulfur.Name,
                NaturalElements.Chlorine.Name,
                NaturalElements.Argon.Name,
                NaturalElements.Potassium.Name,
                NaturalElements.Calcium.Name,
                NaturalElements.Scandium.Name,
                NaturalElements.Titanium.Name,
                NaturalElements.Vanadium.Name,
                NaturalElements.Chromium.Name,
                NaturalElements.Manganese.Name,
                NaturalElements.Iron.Name,
                NaturalElements.Cobalt.Name,
                NaturalElements.Nickel.Name,
                NaturalElements.Copper.Name,
                NaturalElements.Zinc.Name,
                NaturalElements.Gallium.Name,
                NaturalElements.Germanium.Name,
                NaturalElements.Arsenic.Name,
                NaturalElements.Selenium.Name,
                NaturalElements.Bromine.Name,
                NaturalElements.Krypton.Name,
                NaturalElements.Rubidium.Name,
                NaturalElements.Strontium.Name,
                NaturalElements.Yttrium.Name,
                NaturalElements.Zirconium.Name,
                NaturalElements.Niobium.Name,
                NaturalElements.Molybdenum.Name,
                NaturalElements.Technetium.Name,
                NaturalElements.Ruthenium.Name,
                NaturalElements.Rhodium.Name,
                NaturalElements.Palladium.Name,
                NaturalElements.Silver.Name,
                NaturalElements.Cadmium.Name,
                NaturalElements.Indium.Name,
                NaturalElements.Tin.Name,
                NaturalElements.Antimony.Name,
                NaturalElements.Tellurium.Name,
                NaturalElements.Iodine.Name,
                NaturalElements.Xenon.Name,
                NaturalElements.Caesium.Name,
                NaturalElements.Barium.Name,
                NaturalElements.Lanthanum.Name,
                NaturalElements.Cerium.Name,
                NaturalElements.Praseodymium.Name,
                NaturalElements.Neodymium.Name,
                NaturalElements.Promethium.Name,
                NaturalElements.Samarium.Name,
                NaturalElements.Europium.Name,
                NaturalElements.Gadolinium.Name,
                NaturalElements.Terbium.Name,
                NaturalElements.Dysprosium.Name,
                NaturalElements.Holmium.Name,
                NaturalElements.Erbium.Name,
                NaturalElements.Thulium.Name,
                NaturalElements.Ytterbium.Name,
                NaturalElements.Lutetium.Name,
                NaturalElements.Hafnium.Name,
                NaturalElements.Tantalum.Name,
                NaturalElements.Tungsten.Name,
                NaturalElements.Rhenium.Name,
                NaturalElements.Osmium.Name,
                NaturalElements.Iridium.Name,
                NaturalElements.Platinum.Name,
                NaturalElements.Gold.Name,
                NaturalElements.Mercury.Name,
                NaturalElements.Thallium.Name,
                NaturalElements.Lead.Name,
                NaturalElements.Bismuth.Name,
                NaturalElements.Polonium.Name,
                NaturalElements.Astatine.Name,
                NaturalElements.Radon.Name,
                NaturalElements.Francium.Name,
                NaturalElements.Radium.Name,
                NaturalElements.Actinium.Name,
                NaturalElements.Thorium.Name,
                NaturalElements.Protactinium.Name,
                NaturalElements.Uranium.Name,
                NaturalElements.Neptunium.Name,
                NaturalElements.Plutonium.Name,
                NaturalElements.Americium.Name,
                NaturalElements.Curium.Name,
                NaturalElements.Berkelium.Name,
                NaturalElements.Californium.Name,
                NaturalElements.Einsteinium.Name,
                NaturalElements.Fermium.Name,
                NaturalElements.Mendelevium.Name,
                NaturalElements.Nobelium.Name,
                NaturalElements.Lawrencium.Name,
                NaturalElements.Rutherfordium.Name,
                NaturalElements.Dubnium.Name,
                NaturalElements.Seaborgium.Name,
                NaturalElements.Bohrium.Name,
                NaturalElements.Hassium.Name,
                NaturalElements.Meitnerium.Name,
                NaturalElements.Darmstadtium.Name,
                NaturalElements.Roentgenium.Name,
                NaturalElements.Copernicium.Name,
                NaturalElements.Nihonium.Name,
                NaturalElements.Flerovium.Name,
                NaturalElements.Moscovium.Name,
                NaturalElements.Livermorium.Name,
                NaturalElements.Tennessine.Name,
                NaturalElements.Oganesson.Name,
 
            };

        /// <summary>
        /// Return the period in the periodic table this element belongs to. If
        /// the element is <see cref="NaturalElements.Unknown"/> it's period is 0.
        /// </summary>
        internal static IReadOnlyList<int> Periods { get; } = new int[] 
            { 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,  };

        /// <summary>
        /// Return the group in the periodic table this element belongs to. If
        /// the element does not belong to a group then it's group is '0'.
        /// </summary>
        internal static IReadOnlyList<int> Groups { get; } = new int[] 
            { 0, 1, 18, 1, 2, 13, 14, 15, 16, 17, 18, 1, 2, 13, 14, 15, 16, 17, 18, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,  };

        /// <summary>
        /// Covalent radius (<i>r<sub>cov</sub></i>).
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Covalent_radius">Covalent radius</seealso>
        internal static IReadOnlyList<double?> CovalentRadiuses { get; } = new double?[] 
            { null, 0.37, 0.32, 1.34, 0.90, 0.82, 0.77, 0.75, 0.73, 0.71, 0.69, 1.54, 1.30, 1.18, 1.11, 1.06, 1.02, 0.99, 0.97, 1.96, 1.74, 1.44, 1.36, 1.25, 1.27, 1.39, 1.25, 1.26, 1.21, 1.38, 1.31, 1.26, 1.22, 1.19, 1.16, 1.14, 1.10, 2.11, 1.92, 1.62, 1.48, 1.37, 1.45, 1.56, 1.26, 1.35, 1.31, 1.53, 1.48, 1.44, 1.41, 1.38, 1.35, 1.33, 1.30, 2.25, 1.98, 1.69, null, null, null, null, null, 2.40, null, null, null, null, null, null, null, 1.60, 1.50, 1.38, 1.46, 1.59, 1.28, 1.37, 1.28, 1.44, 1.49, 1.48, 1.47, 1.46, 1.46, null, 1.45, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };

        /// <summary>
        /// van der Waals radius (<i>r<sub>w</sub></i>).
        /// </summary>
        internal static IReadOnlyList<double?> VdwRadiuses { get; } = new double?[] 
            { null, 1.20, 1.40, 2.20, 1.90, 1.80, 1.70, 1.60, 1.55, 1.50, 1.54, 2.40, 2.20, 2.10, 2.10, 1.95, 1.80, 1.80, 1.88, 2.80, 2.40, 2.30, 2.15, 2.05, 2.05, 2.05, 2.05, null, null, null, 2.10, 2.10, 2.10, 2.05, 1.90, 1.90, 2.02, 2.90, 2.55, 2.40, 2.30, 2.15, 2.10, 2.05, 2.05, null, 2.05, 2.10, 2.20, 2.20, 2.25, 2.20, 2.10, 2.10, 2.16, 3.00, 2.70, 2.50, 2.48, 2.47, 2.45, 2.43, 2.42, 2.40, 2.38, 2.37, 2.35, 2.33, 2.32, 2.30, 2.28, 2.27, 2.25, 2.20, 2.10, 2.05, null, null, 2.05, 2.10, 2.05, 2.20, 2.30, 2.30, null, null, null, null, null, null, 2.40, null, 2.30, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };

        /// <summary>
        /// Pauling electronegativity, symbol χ, is a chemical property that describes
        /// the tendency of an atom or a functional group to attract electrons
        /// (or electron density) towards itself. This method provides access to the
        /// Pauling electronegativity value for a chemical element. If no value is
        /// available <see langword="null"/> is returned.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Electronegativity#Pauling_electronegativity">Pauling Electronegativity</seealso>
        internal static IReadOnlyList<double?> Electronegativities { get; } = new double?[] 
            { null, 2.20, null, 0.98, 1.57, 2.04, 2.55, 3.04, 3.44, 3.98, null, 0.93, 1.31, 1.61, 1.90, 2.19, 2.58, 3.16, null, 0.82, 1.00, 1.36, 1.54, 1.63, 1.66, 1.55, 1.83, 1.88, 1.91, 1.90, 1.65, 1.81, 2.01, 2.18, 2.55, 2.96, 3.00, 0.82, 0.95, 1.22, 1.33, 1.60, 2.16, 1.90, 2.20, 2.28, 2.20, 1.93, 1.69, 1.78, 1.96, 2.05, 2.10, 2.66, 2.60, 0.79, 0.89, 1.10, 1.12, 1.13, 1.14, null, 1.17, null, 1.20, null, 1.22, 1.23, 1.24, 1.25, null, 1.27, 1.30, 1.50, 2.36, 1.90, 2.20, 2.20, 2.28, 2.54, 2.00, 1.62, 2.33, 2.02, 2.00, 2.20, null, 0.70, 0.90, 1.10, 1.30, 1.50, 1.38, 1.36, 1.28, 1.30, 1.30, 1.30, 1.30, 1.30, 1.30, 1.30, 1.30, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };
    }
}

