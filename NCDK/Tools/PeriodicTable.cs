/* Copyright (C) 2008  Rajarshi Guha <rajarshi@users.sf.net>
 *               2011  Jonathan Alvarsson <jonalv@users.sf.net>
 *               2014  Mark B Vine (orcid:0000-0002-7794-0426)
 *
 * Contact: cdk-devel@lists.sf.net
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Config;
using System.Collections.Generic;
using static NCDK.Config.NaturalElement;
using static NCDK.Config.NaturalElements;

namespace NCDK.Tools
{
    /// <summary>
    /// Represents elements of the Periodic Table. This utility class was
    /// previously useful when one wants generic properties of elements such as
    /// atomic number, VdW radius etc. The new approach to this is to use the
    /// <see cref="NaturalElement"/> enumeration.
    /// </summary>
    // @author Rajarshi Guha
    // @cdk.created 2008-06-12
    // @cdk.keyword element
    // @cdk.keyword periodic table
    // @cdk.keyword radius, vanderwaals
    // @cdk.keyword electronegativity
    // @cdk.module core
    public sealed class PeriodicTable
    {
        /// <summary>A lock used for locking CAD ID initialisation.</summary>
        private readonly static object syncLock = new object();

        /// <summary>
        /// Get the Van der Waals radius for the element in question.
        /// </summary>
        /// <param name="symbol">The symbol of the element</param>
        /// <returns>the Van der waals radius</returns>
        public static double? GetVdwRadius(string symbol)
        {
            return VdwRadiuses[ToAtomicNumber(symbol)];
        }
        
        /// <summary>
        /// Get the covalent radius for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the covalent radius</returns>
        public static double? GetCovalentRadius(string symbol)
        {
            return CovalentRadiuses[ToAtomicNumber(symbol)];
        }

        /// <summary>
        /// Get the CAS ID for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the CAS ID</returns>
        public static string GetCASId(string symbol)
        {
            return MapToCasId[ToAtomicNumber(symbol)]; 
        }

        /// <summary>
        /// Get the chemical series for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the chemical series of the element</returns>
        public static string GetChemicalSeries(string symbol)
        {
            if (!MapToSeries.TryGetValue(ToAtomicNumber(symbol), out string series))
                return "";
            return series;
        }

        /// <summary>
        /// Get the group of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the group</returns>
        public static int GetGroup(string symbol)
        {
            return GetGroup(ToAtomicNumber(symbol));
        }

        public static int GetGroup(int number)
        {
            return NaturalElement.Groups[number];
        }

        /// <summary>
        /// Get the name of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the name of the element</returns>
        public static string GetName(string symbol)
        {
            return NaturalElement.Names[NaturalElement.ToAtomicNumber(symbol)];
        }

        /// <summary>
        /// Get the period of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the period</returns>
        public static int GetPeriod(string symbol)
        {
            return Periods[ToAtomicNumber(symbol)];
        }

        public static int GetPeriod(int number)
        {
            return NaturalElement.Periods[number];
        }

        /// <summary>
        /// Get the phase of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the phase of the element</returns>
        public static string GetPhase(string symbol)
        {
            if (!MapToPhase.TryGetValue(ToAtomicNumber(symbol), out string phase))
                return "";
            return phase;
        }

        /// <summary>
        /// Get the atomic number of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the atomic number</returns>
        public static int GetAtomicNumber(string symbol)
        {
            return ToAtomicNumber(symbol);
        }

        /// <summary>
        /// Get the Pauling electronegativity of an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the Pauling electronegativity</returns>
        public static double? GetPaulingElectronegativity(string symbol)
        {
            return Electronegativities[ToAtomicNumber(symbol)];
        }

        /// <summary>
        /// Get the symbol for the specified atomic number.
        /// </summary>
        /// <param name="atomicNumber">the atomic number of the element</param>
        /// <returns>the corresponding symbol</returns>
        public static string GetSymbol(int atomicNumber)
        {
            return OfNumber(atomicNumber).Symbol;
        }

        /// <summary>
        /// The number of elements in the periodic table
        /// </summary>
        public static int ElementCount => NaturalElement.Elements.Count;

        private static Dictionary<int, string> MapToCasId { get; } = new Dictionary<int, string>
            {
                { Unknown.AtomicNumber, "" },
                { Hydrogen.AtomicNumber, "1333-74-0" },
                { Helium.AtomicNumber, "7440-59-7" },
                { Lithium.AtomicNumber, "7439-93-2" },
                { Beryllium.AtomicNumber, "7440-41-7" },
                { Boron.AtomicNumber, "7440-42-8" },
                { Carbon.AtomicNumber, "7440-44-0" },
                { Nitrogen.AtomicNumber, "7727-37-9" },
                { Oxygen.AtomicNumber, "7782-44-7" },
                { Fluorine.AtomicNumber, "7782-41-4" },
                { Neon.AtomicNumber, "7440-01-9" },
                { Sodium.AtomicNumber, "7440-23-5" },
                { Magnesium.AtomicNumber, "7439-95-4" },
                { Aluminium.AtomicNumber, "7429-90-5" },
                { Silicon.AtomicNumber, "7440-21-3" },
                { Phosphorus.AtomicNumber, "7723-14-0" },
                { Sulfur.AtomicNumber, "7704-34-9" },
                { Chlorine.AtomicNumber, "7782-50-5" },
                { Argon.AtomicNumber, "7440-37-1" },
                { Potassium.AtomicNumber, "7440-09-7" },
                { Calcium.AtomicNumber, "7440-70-2" },
                { Scandium.AtomicNumber, "7440-20-2" },
                { Titanium.AtomicNumber, "7440-32-6" },
                { Vanadium.AtomicNumber, "7440-62-2" },
                { Chromium.AtomicNumber, "7440-47-3" },
                { Manganese.AtomicNumber, "7439-96-5" },
                { Iron.AtomicNumber, "7439-89-6" },
                { Cobalt.AtomicNumber, "7440-48-4" },
                { Nickel.AtomicNumber, "7440-02-0" },
                { Copper.AtomicNumber, "7440-50-8" },
                { Zinc.AtomicNumber, "7440-66-6" },
                { Gallium.AtomicNumber, "7440-55-3" },
                { Germanium.AtomicNumber, "7440-56-4" },
                { Arsenic.AtomicNumber, "7440-38-2" },
                { Selenium.AtomicNumber, "7782-49-2" },
                { Bromine.AtomicNumber, "7726-95-6" },
                { Krypton.AtomicNumber, "7439-90-9" },
                { Rubidium.AtomicNumber, "7440-17-7" },
                { Strontium.AtomicNumber, "7440-24-6" },
                { Yttrium.AtomicNumber, "7440-65-5" },
                { Zirconium.AtomicNumber, "7440-67-7" },
                { Niobium.AtomicNumber, "7440-03-1" },
                { Molybdenum.AtomicNumber, "7439-98-7" },
                { Technetium.AtomicNumber, "7440-26-8" },
                { Ruthenium.AtomicNumber, "7440-18-8" },
                { Rhodium.AtomicNumber, "7440-16-6" },
                { Palladium.AtomicNumber, "7440-05-3" },
                { Silver.AtomicNumber, "7440-22-4" },
                { Cadmium.AtomicNumber, "7440-43-9" },
                { Indium.AtomicNumber, "7440-74-6" },
                { Tin.AtomicNumber, "7440-31-5" },
                { Antimony.AtomicNumber, "7440-36-0" },
                { Tellurium.AtomicNumber, "13494-80-9" },
                { Iodine.AtomicNumber, "7553-56-2" },
                { Xenon.AtomicNumber, "7440-63-3" },
                { Caesium.AtomicNumber, "7440-46-2" },
                { Barium.AtomicNumber, "7440-39-3" },
                { Lanthanum.AtomicNumber, "7439-91-0" },
                { Cerium.AtomicNumber, "7440-45-1" },
                { Praseodymium.AtomicNumber, "7440-10-0" },
                { Neodymium.AtomicNumber, "7440-00-8" },
                { Promethium.AtomicNumber, "7440-12-2" },
                { Samarium.AtomicNumber, "7440-19-9" },
                { Europium.AtomicNumber, "7440-53-1" },
                { Gadolinium.AtomicNumber, "7440-54-2" },
                { Terbium.AtomicNumber, "7440-27-9" },
                { Dysprosium.AtomicNumber, "7429-91-6" },
                { Holmium.AtomicNumber, "7440-60-0" },
                { Erbium.AtomicNumber, "7440-52-0" },
                { Thulium.AtomicNumber, "7440-30-4" },
                { Ytterbium.AtomicNumber, "7440-64-4" },
                { Lutetium.AtomicNumber, "7439-94-3" },
                { Hafnium.AtomicNumber, "7440-58-6" },
                { Tantalum.AtomicNumber, "7440-25-7" },
                { Tungsten.AtomicNumber, "7440-33-7" },
                { Rhenium.AtomicNumber, "7440-15-5" },
                { Osmium.AtomicNumber, "7440-04-2" },
                { Iridium.AtomicNumber, "7439-88-5" },
                { Platinum.AtomicNumber, "7440-06-4" },
                { Gold.AtomicNumber, "7440-57-5" },
                { Mercury.AtomicNumber, "7439-97-6" },
                { Thallium.AtomicNumber, "7440-28-0" },
                { Lead.AtomicNumber, "7439-92-1" },
                { Bismuth.AtomicNumber, "7440-69-9" },
                { Polonium.AtomicNumber, "7440-08-6" },
                { Astatine.AtomicNumber, "7440-08-6" },
                { Radon.AtomicNumber, "10043-92-2" },
                { Francium.AtomicNumber, "7440-73-5" },
                { Radium.AtomicNumber, "7440-14-4" },
                { Actinium.AtomicNumber, "7440-34-8" },
                { Thorium.AtomicNumber, "7440-29-1" },
                { Protactinium.AtomicNumber, "7440-13-3" },
                { Uranium.AtomicNumber, "7440-61-1" },
                { Neptunium.AtomicNumber, "7439-99-8" },
                { Plutonium.AtomicNumber, "7440-07-5" },
                { Americium.AtomicNumber, "7440-35-9" },
                { Curium.AtomicNumber, "7440-51-9" },
                { Berkelium.AtomicNumber, "7440-40-6" },
                { Californium.AtomicNumber, "7440-71-3" },
                { Einsteinium.AtomicNumber, "7429-92-7" },
                { Fermium.AtomicNumber, "7440-72-4" },
                { Mendelevium.AtomicNumber, "7440-11-1" },
                { Nobelium.AtomicNumber, "10028-14-5" },
                { Lawrencium.AtomicNumber, "22537-19-5" },
                { Rutherfordium.AtomicNumber, "53850-36-5" },
                { Dubnium.AtomicNumber, "53850-35-4" },
                { Seaborgium.AtomicNumber, "54038-81-2" },
                { Bohrium.AtomicNumber, "54037-14-8" },
                { Hassium.AtomicNumber, "54037-57-9" },
                { Meitnerium.AtomicNumber, "54038-01-6" },
                { Darmstadtium.AtomicNumber, "54083-77-1" },
                { Roentgenium.AtomicNumber, "54386-24-2" },
                { Copernicium.AtomicNumber, "54084-26-3" },
                { Ununtrium.AtomicNumber, "" },
                { Flerovium.AtomicNumber, "54085-16-4" },
                { Ununpentium.AtomicNumber, "" },
                { Livermorium.AtomicNumber, "54100-71-9" },
                { Ununseptium.AtomicNumber, "" },
                { Ununoctium.AtomicNumber, "" }
            };

        private static void AddToMap(Dictionary<int, string> map, string text, params int[] numbers)
        {
            foreach (var n in numbers)
                map[n] = text;
        }

        private static Dictionary<int, string> MapToSeries { get; } = MakeMapToSeries();
        
        private static Dictionary<int, string> MakeMapToSeries()
        {
            var ids = new Dictionary<int, string>();
            AddToMap(ids, "Non Metal", Sulfur.AtomicNumber, Selenium.AtomicNumber, Oxygen.AtomicNumber, Carbon.AtomicNumber, Phosphorus.AtomicNumber, Hydrogen.AtomicNumber, Nitrogen.AtomicNumber);
            AddToMap(ids, "Noble Gas", Helium.AtomicNumber, Krypton.AtomicNumber, Xenon.AtomicNumber, Argon.AtomicNumber, Radon.AtomicNumber, Neon.AtomicNumber);
            AddToMap(ids, "Alkali Metal", Sodium.AtomicNumber, Rubidium.AtomicNumber, Potassium.AtomicNumber, Caesium.AtomicNumber, Francium.AtomicNumber, Lithium.AtomicNumber);
            AddToMap(ids, "Alkali Earth Metal", Strontium.AtomicNumber, Radium.AtomicNumber, Calcium.AtomicNumber, Magnesium.AtomicNumber, Barium.AtomicNumber, Beryllium.AtomicNumber);
            AddToMap(ids, "Metalloid", Silicon.AtomicNumber, Arsenic.AtomicNumber, Tellurium.AtomicNumber, Germanium.AtomicNumber, Antimony.AtomicNumber, Polonium.AtomicNumber, Boron.AtomicNumber);
            AddToMap(ids, "Halogen", Fluorine.AtomicNumber, Iodine.AtomicNumber, Chlorine.AtomicNumber, Astatine.AtomicNumber, Bromine.AtomicNumber);
            AddToMap(ids, "Metal", Gallium.AtomicNumber, Indium.AtomicNumber, Aluminium.AtomicNumber, Thallium.AtomicNumber, Tin.AtomicNumber, Lead.AtomicNumber, Bismuth.AtomicNumber);
            AddToMap(ids, "Transition Metal", Seaborgium.AtomicNumber, Hafnium.AtomicNumber,
                Roentgenium.AtomicNumber, Iridium.AtomicNumber, Nickel.AtomicNumber, Meitnerium.AtomicNumber, Yttrium.AtomicNumber, Copper.AtomicNumber, Rutherfordium.AtomicNumber, Tungsten.AtomicNumber, Copernicium.AtomicNumber,
                Rhodium.AtomicNumber, Cobalt.AtomicNumber, Zinc.AtomicNumber, Platinum.AtomicNumber, Gold.AtomicNumber, Cadmium.AtomicNumber, Manganese.AtomicNumber, Darmstadtium.AtomicNumber, Dubnium.AtomicNumber, Palladium.AtomicNumber, Vanadium.AtomicNumber,
                Titanium.AtomicNumber, Tantalum.AtomicNumber, Chromium.AtomicNumber, Molybdenum.AtomicNumber, Ruthenium.AtomicNumber, Zirconium.AtomicNumber, Osmium.AtomicNumber, Bohrium.AtomicNumber, Rhenium.AtomicNumber, Niobium.AtomicNumber,
                Scandium.AtomicNumber, Technetium.AtomicNumber, Hassium.AtomicNumber, Mercury.AtomicNumber, Iron.AtomicNumber, Silver.AtomicNumber);
            AddToMap(ids, "Lanthanide", Terbium.AtomicNumber, Samarium.AtomicNumber, Lutetium.AtomicNumber,
                Neodymium.AtomicNumber, Cerium.AtomicNumber, Europium.AtomicNumber, Gadolinium.AtomicNumber, Thulium.AtomicNumber, Lanthanum.AtomicNumber, Erbium.AtomicNumber, Promethium.AtomicNumber, Holmium.AtomicNumber, Praseodymium.AtomicNumber,
                Dysprosium.AtomicNumber, Ytterbium.AtomicNumber);
            AddToMap(ids, "Actinide", Fermium.AtomicNumber, Protactinium.AtomicNumber, Plutonium.AtomicNumber, Thorium.AtomicNumber, Lawrencium.AtomicNumber, Einsteinium.AtomicNumber,
                Nobelium.AtomicNumber, Actinium.AtomicNumber, Americium.AtomicNumber, Curium.AtomicNumber, Berkelium.AtomicNumber, Mendelevium.AtomicNumber, Uranium.AtomicNumber, Californium.AtomicNumber, Neptunium.AtomicNumber);
            return ids;
        }

        private static Dictionary<int, string> MapToPhase { get; } = MakeMapToPhase();

        private static Dictionary<int, string> MakeMapToPhase()
        {
            var ids = new Dictionary<int, string>();
            AddToMap(ids, "Solid", Sulfur.AtomicNumber, Hafnium.AtomicNumber, Terbium.AtomicNumber, Calcium.AtomicNumber, Gadolinium.AtomicNumber, Nickel.AtomicNumber, Cerium.AtomicNumber, Germanium.AtomicNumber, Phosphorus.AtomicNumber, Copper.AtomicNumber, Polonium.AtomicNumber,
                Lead.AtomicNumber, Gold.AtomicNumber, Iodine.AtomicNumber, Cadmium.AtomicNumber, Ytterbium.AtomicNumber, Manganese.AtomicNumber, Lithium.AtomicNumber, Palladium.AtomicNumber, Vanadium.AtomicNumber, Chromium.AtomicNumber, Molybdenum.AtomicNumber,
                Potassium.AtomicNumber, Ruthenium.AtomicNumber, Osmium.AtomicNumber, Boron.AtomicNumber, Bismuth.AtomicNumber, Rhenium.AtomicNumber, Holmium.AtomicNumber, Niobium.AtomicNumber, Praseodymium.AtomicNumber, Barium.AtomicNumber,
                Antimony.AtomicNumber, Thallium.AtomicNumber, Iron.AtomicNumber, Silver.AtomicNumber, Silicon.AtomicNumber, Caesium.AtomicNumber, Astatine.AtomicNumber, Iridium.AtomicNumber, Francium.AtomicNumber, Lutetium.AtomicNumber, Yttrium.AtomicNumber,
                Rubidium.AtomicNumber, Lanthanum.AtomicNumber, Tungsten.AtomicNumber, Erbium.AtomicNumber, Selenium.AtomicNumber, Gallium.AtomicNumber, Carbon.AtomicNumber, Rhodium.AtomicNumber, Uranium.AtomicNumber, Dysprosium.AtomicNumber, Cobalt.AtomicNumber,
                Zinc.AtomicNumber, Platinum.AtomicNumber, Protactinium.AtomicNumber, Titanium.AtomicNumber, Arsenic.AtomicNumber, Tantalum.AtomicNumber, Thorium.AtomicNumber, Samarium.AtomicNumber, Europium.AtomicNumber, Neodymium.AtomicNumber,
                Zirconium.AtomicNumber, Radium.AtomicNumber, Thulium.AtomicNumber, Sodium.AtomicNumber, Scandium.AtomicNumber, Tellurium.AtomicNumber, Indium.AtomicNumber, Beryllium.AtomicNumber, Aluminium.AtomicNumber, Strontium.AtomicNumber, Tin.AtomicNumber,
                Magnesium.AtomicNumber);
            AddToMap(ids, "Liquid", Bromine.AtomicNumber, Mercury.AtomicNumber);
            AddToMap(ids, "Gas", Fluorine.AtomicNumber, Oxygen.AtomicNumber, Xenon.AtomicNumber, Argon.AtomicNumber, Chlorine.AtomicNumber, Helium.AtomicNumber, Krypton.AtomicNumber, Hydrogen.AtomicNumber, Radon.AtomicNumber, Nitrogen.AtomicNumber, Neon.AtomicNumber);
            AddToMap(ids, "Synthetic", Fermium.AtomicNumber, Seaborgium.AtomicNumber, Plutonium.AtomicNumber, Roentgenium.AtomicNumber, Lawrencium.AtomicNumber, Meitnerium.AtomicNumber, Einsteinium.AtomicNumber, Nobelium.AtomicNumber, Actinium.AtomicNumber,
                Rutherfordium.AtomicNumber, Americium.AtomicNumber, Curium.AtomicNumber, Bohrium.AtomicNumber, Berkelium.AtomicNumber, Promethium.AtomicNumber, Copernicium.AtomicNumber, Technetium.AtomicNumber, Hassium.AtomicNumber,
                Californium.AtomicNumber, Mendelevium.AtomicNumber, Neptunium.AtomicNumber, Darmstadtium.AtomicNumber, Dubnium.AtomicNumber);
            return ids;
        }
    }
}
