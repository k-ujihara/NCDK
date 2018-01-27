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

using System;
using System.Collections.Generic;
using NCDK.Config;

namespace NCDK.Tools
{
    /// <summary>
    /// Represents elements of the Periodic Table.  This utility class was
    /// previously useful when one wants generic properties of elements such as
    /// atomic number, VdW radius etc. The new approach to this is to use the
    /// <see cref="ChemicalElement"/> enumeration.
    /// </summary>
    // @author Rajarshi Guha
    // @cdk.created 2008-06-12
    // @cdk.keyword element
    // @cdk.keyword periodic table
    // @cdk.keyword radius, vanderwaals
    // @cdk.keyword electronegativity
    // @cdk.module core
    // @cdk.githash 
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
            return ChemicalElement.OfString(symbol).VdwRadius;
        }
        
        /// <summary>
        /// Get the covalent radius for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the covalent radius</returns>
        public static double? GetCovalentRadius(string symbol)
        {
            return ChemicalElement.OfString(symbol).CovalentRadius;
        }

        /// <summary>
        /// Get the CAS ID for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the CAS ID</returns>
        public static string GetCASId(string symbol)
        {
            return MapToCasId[ChemicalElement.OfString(symbol).AtomicNumber]; 
        }

        /// <summary>
        /// Get the chemical series for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the chemical series of the element</returns>
        public static string GetChemicalSeries(string symbol)
        {
            if (!MapToSeries.TryGetValue(ChemicalElement.OfString(symbol).AtomicNumber, out string series))
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
            return ChemicalElement.OfString(symbol).Group;
        }

        /// <summary>
        /// Get the name of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the name of the element</returns>
        public static string GetName(string symbol)
        {
            return ChemicalElement.OfString(symbol).Name;
        }

        /// <summary>
        /// Get the period of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the period</returns>
        public static int GetPeriod(string symbol)
        {
            return ChemicalElement.OfString(symbol).Period;
        }

        /// <summary>
        /// Get the phase of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the phase of the element</returns>
        public static string GetPhase(string symbol)
        {
            if (!MapToPhase.TryGetValue(ChemicalElement.OfString(symbol).AtomicNumber, out string phase))
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
            return ChemicalElement.OfString(symbol).AtomicNumber;
        }

        /// <summary>
        /// Get the Pauling electronegativity of an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the Pauling electronegativity</returns>
        public static double? GetPaulingElectronegativity(string symbol)
        {
            return ChemicalElement.OfString(symbol).Electronegativity;
        }

        /// <summary>
        /// Get the symbol for the specified atomic number.
        /// </summary>
        /// <param name="atomicNumber">the atomic number of the element</param>
        /// <returns>the corresponding symbol</returns>
        public static string GetSymbol(int atomicNumber)
        {
            return ChemicalElement.OfNumber(atomicNumber).Symbol;
        }

        /// <summary>
        /// The number of elements in the periodic table
        /// </summary>
        public static int ElementCount => ChemicalElement.Values.Count;

        private static Dictionary<int, string> MapTo(ref Dictionary<int, string> mapTo, Func<Dictionary<int, string>> mapMaker)
        {
            Dictionary<int, string> result = mapTo;
            if (result == null)
            {
                lock (syncLock)
                {
                    result = mapTo;
                    if (result == null)
                        mapTo = result = mapMaker.Invoke();
                }
            }
            return result;
        }
        
        private static void AddToMap(Dictionary<int, string> ids, string name, params ChemicalElement[] elements)
        {
            foreach (var elm in elements)
                ids.Add(elm.AtomicNumber, name);
        }

        private static Dictionary<int, string> mapToCasId;
        private static Dictionary<int, string> MapToCasId
        { get { return MapTo(ref mapToCasId, MakeMapToCasId); } }

        private static Dictionary<int, string> mapToSeries;
        private static Dictionary<int, string> MapToSeries
        { get { return MapTo(ref mapToSeries, MakeMapToSeries); } }

        private static Dictionary<int, string> mapToPhase;
        private static Dictionary<int, string> MapToPhase
        { get { return MapTo(ref mapToPhase, MakeMapToPhase); } }

        private static Dictionary<int, string> MakeMapToCasId()
        {
            var ids = new Dictionary<int, string>
            {
                { ChemicalElements.Unknown.AtomicNumber, "" },
                { ChemicalElements.Hydrogen.AtomicNumber, "1333-74-0" },
                { ChemicalElements.Helium.AtomicNumber, "7440-59-7" },
                { ChemicalElements.Lithium.AtomicNumber, "7439-93-2" },
                { ChemicalElements.Beryllium.AtomicNumber, "7440-41-7" },
                { ChemicalElements.Boron.AtomicNumber, "7440-42-8" },
                { ChemicalElements.Carbon.AtomicNumber, "7440-44-0" },
                { ChemicalElements.Nitrogen.AtomicNumber, "7727-37-9" },
                { ChemicalElements.Oxygen.AtomicNumber, "7782-44-7" },
                { ChemicalElements.Fluorine.AtomicNumber, "7782-41-4" },
                { ChemicalElements.Neon.AtomicNumber, "7440-01-9" },
                { ChemicalElements.Sodium.AtomicNumber, "7440-23-5" },
                { ChemicalElements.Magnesium.AtomicNumber, "7439-95-4" },
                { ChemicalElements.Aluminium.AtomicNumber, "7429-90-5" },
                { ChemicalElements.Silicon.AtomicNumber, "7440-21-3" },
                { ChemicalElements.Phosphorus.AtomicNumber, "7723-14-0" },
                { ChemicalElements.Sulfur.AtomicNumber, "7704-34-9" },
                { ChemicalElements.Chlorine.AtomicNumber, "7782-50-5" },
                { ChemicalElements.Argon.AtomicNumber, "7440-37-1" },
                { ChemicalElements.Potassium.AtomicNumber, "7440-09-7" },
                { ChemicalElements.Calcium.AtomicNumber, "7440-70-2" },
                { ChemicalElements.Scandium.AtomicNumber, "7440-20-2" },
                { ChemicalElements.Titanium.AtomicNumber, "7440-32-6" },
                { ChemicalElements.Vanadium.AtomicNumber, "7440-62-2" },
                { ChemicalElements.Chromium.AtomicNumber, "7440-47-3" },
                { ChemicalElements.Manganese.AtomicNumber, "7439-96-5" },
                { ChemicalElements.Iron.AtomicNumber, "7439-89-6" },
                { ChemicalElements.Cobalt.AtomicNumber, "7440-48-4" },
                { ChemicalElements.Nickel.AtomicNumber, "7440-02-0" },
                { ChemicalElements.Copper.AtomicNumber, "7440-50-8" },
                { ChemicalElements.Zinc.AtomicNumber, "7440-66-6" },
                { ChemicalElements.Gallium.AtomicNumber, "7440-55-3" },
                { ChemicalElements.Germanium.AtomicNumber, "7440-56-4" },
                { ChemicalElements.Arsenic.AtomicNumber, "7440-38-2" },
                { ChemicalElements.Selenium.AtomicNumber, "7782-49-2" },
                { ChemicalElements.Bromine.AtomicNumber, "7726-95-6" },
                { ChemicalElements.Krypton.AtomicNumber, "7439-90-9" },
                { ChemicalElements.Rubidium.AtomicNumber, "7440-17-7" },
                { ChemicalElements.Strontium.AtomicNumber, "7440-24-6" },
                { ChemicalElements.Yttrium.AtomicNumber, "7440-65-5" },
                { ChemicalElements.Zirconium.AtomicNumber, "7440-67-7" },
                { ChemicalElements.Niobium.AtomicNumber, "7440-03-1" },
                { ChemicalElements.Molybdenum.AtomicNumber, "7439-98-7" },
                { ChemicalElements.Technetium.AtomicNumber, "7440-26-8" },
                { ChemicalElements.Ruthenium.AtomicNumber, "7440-18-8" },
                { ChemicalElements.Rhodium.AtomicNumber, "7440-16-6" },
                { ChemicalElements.Palladium.AtomicNumber, "7440-05-3" },
                { ChemicalElements.Silver.AtomicNumber, "7440-22-4" },
                { ChemicalElements.Cadmium.AtomicNumber, "7440-43-9" },
                { ChemicalElements.Indium.AtomicNumber, "7440-74-6" },
                { ChemicalElements.Tin.AtomicNumber, "7440-31-5" },
                { ChemicalElements.Antimony.AtomicNumber, "7440-36-0" },
                { ChemicalElements.Tellurium.AtomicNumber, "13494-80-9" },
                { ChemicalElements.Iodine.AtomicNumber, "7553-56-2" },
                { ChemicalElements.Xenon.AtomicNumber, "7440-63-3" },
                { ChemicalElements.Caesium.AtomicNumber, "7440-46-2" },
                { ChemicalElements.Barium.AtomicNumber, "7440-39-3" },
                { ChemicalElements.Lanthanum.AtomicNumber, "7439-91-0" },
                { ChemicalElements.Cerium.AtomicNumber, "7440-45-1" },
                { ChemicalElements.Praseodymium.AtomicNumber, "7440-10-0" },
                { ChemicalElements.Neodymium.AtomicNumber, "7440-00-8" },
                { ChemicalElements.Promethium.AtomicNumber, "7440-12-2" },
                { ChemicalElements.Samarium.AtomicNumber, "7440-19-9" },
                { ChemicalElements.Europium.AtomicNumber, "7440-53-1" },
                { ChemicalElements.Gadolinium.AtomicNumber, "7440-54-2" },
                { ChemicalElements.Terbium.AtomicNumber, "7440-27-9" },
                { ChemicalElements.Dysprosium.AtomicNumber, "7429-91-6" },
                { ChemicalElements.Holmium.AtomicNumber, "7440-60-0" },
                { ChemicalElements.Erbium.AtomicNumber, "7440-52-0" },
                { ChemicalElements.Thulium.AtomicNumber, "7440-30-4" },
                { ChemicalElements.Ytterbium.AtomicNumber, "7440-64-4" },
                { ChemicalElements.Lutetium.AtomicNumber, "7439-94-3" },
                { ChemicalElements.Hafnium.AtomicNumber, "7440-58-6" },
                { ChemicalElements.Tantalum.AtomicNumber, "7440-25-7" },
                { ChemicalElements.Tungsten.AtomicNumber, "7440-33-7" },
                { ChemicalElements.Rhenium.AtomicNumber, "7440-15-5" },
                { ChemicalElements.Osmium.AtomicNumber, "7440-04-2" },
                { ChemicalElements.Iridium.AtomicNumber, "7439-88-5" },
                { ChemicalElements.Platinum.AtomicNumber, "7440-06-4" },
                { ChemicalElements.Gold.AtomicNumber, "7440-57-5" },
                { ChemicalElements.Mercury.AtomicNumber, "7439-97-6" },
                { ChemicalElements.Thallium.AtomicNumber, "7440-28-0" },
                { ChemicalElements.Lead.AtomicNumber, "7439-92-1" },
                { ChemicalElements.Bismuth.AtomicNumber, "7440-69-9" },
                { ChemicalElements.Polonium.AtomicNumber, "7440-08-6" },
                { ChemicalElements.Astatine.AtomicNumber, "7440-08-6" },
                { ChemicalElements.Radon.AtomicNumber, "10043-92-2" },
                { ChemicalElements.Francium.AtomicNumber, "7440-73-5" },
                { ChemicalElements.Radium.AtomicNumber, "7440-14-4" },
                { ChemicalElements.Actinium.AtomicNumber, "7440-34-8" },
                { ChemicalElements.Thorium.AtomicNumber, "7440-29-1" },
                { ChemicalElements.Protactinium.AtomicNumber, "7440-13-3" },
                { ChemicalElements.Uranium.AtomicNumber, "7440-61-1" },
                { ChemicalElements.Neptunium.AtomicNumber, "7439-99-8" },
                { ChemicalElements.Plutonium.AtomicNumber, "7440-07-5" },
                { ChemicalElements.Americium.AtomicNumber, "7440-35-9" },
                { ChemicalElements.Curium.AtomicNumber, "7440-51-9" },
                { ChemicalElements.Berkelium.AtomicNumber, "7440-40-6" },
                { ChemicalElements.Californium.AtomicNumber, "7440-71-3" },
                { ChemicalElements.Einsteinium.AtomicNumber, "7429-92-7" },
                { ChemicalElements.Fermium.AtomicNumber, "7440-72-4" },
                { ChemicalElements.Mendelevium.AtomicNumber, "7440-11-1" },
                { ChemicalElements.Nobelium.AtomicNumber, "10028-14-5" },
                { ChemicalElements.Lawrencium.AtomicNumber, "22537-19-5" },
                { ChemicalElements.Rutherfordium.AtomicNumber, "53850-36-5" },
                { ChemicalElements.Dubnium.AtomicNumber, "53850-35-4" },
                { ChemicalElements.Seaborgium.AtomicNumber, "54038-81-2" },
                { ChemicalElements.Bohrium.AtomicNumber, "54037-14-8" },
                { ChemicalElements.Hassium.AtomicNumber, "54037-57-9" },
                { ChemicalElements.Meitnerium.AtomicNumber, "54038-01-6" },
                { ChemicalElements.Darmstadtium.AtomicNumber, "54083-77-1" },
                { ChemicalElements.Roentgenium.AtomicNumber, "54386-24-2" },
                { ChemicalElements.Copernicium.AtomicNumber, "54084-26-3" },
                { ChemicalElements.Ununtrium.AtomicNumber, "" },
                { ChemicalElements.Flerovium.AtomicNumber, "54085-16-4" },
                { ChemicalElements.Ununpentium.AtomicNumber, "" },
                { ChemicalElements.Livermorium.AtomicNumber, "54100-71-9" },
                { ChemicalElements.Ununseptium.AtomicNumber, "" },
                { ChemicalElements.Ununoctium.AtomicNumber, "" }
            };

            return ids;
        }
        
        private static Dictionary<int, string> MakeMapToSeries()
        {
            var ids = new Dictionary<int, string>();
            AddToMap(ids, "Non Metal", ChemicalElements.Sulfur, ChemicalElements.Selenium, ChemicalElements.Oxygen, ChemicalElements.Carbon, ChemicalElements.Phosphorus, ChemicalElements.Hydrogen, ChemicalElements.Nitrogen);
            AddToMap(ids, "Noble Gas", ChemicalElements.Helium, ChemicalElements.Krypton, ChemicalElements.Xenon, ChemicalElements.Argon, ChemicalElements.Radon, ChemicalElements.Neon);
            AddToMap(ids, "Alkali Metal", ChemicalElements.Sodium, ChemicalElements.Rubidium, ChemicalElements.Potassium, ChemicalElements.Caesium, ChemicalElements.Francium, ChemicalElements.Lithium);
            AddToMap(ids, "Alkali Earth Metal", ChemicalElements.Strontium, ChemicalElements.Radium, ChemicalElements.Calcium, ChemicalElements.Magnesium, ChemicalElements.Barium, ChemicalElements.Beryllium);
            AddToMap(ids, "Metalloid", ChemicalElements.Silicon, ChemicalElements.Arsenic, ChemicalElements.Tellurium, ChemicalElements.Germanium, ChemicalElements.Antimony, ChemicalElements.Polonium, ChemicalElements.Boron);
            AddToMap(ids, "Halogen", ChemicalElements.Fluorine, ChemicalElements.Iodine, ChemicalElements.Chlorine, ChemicalElements.Astatine, ChemicalElements.Bromine);
            AddToMap(ids, "Metal", ChemicalElements.Gallium, ChemicalElements.Indium, ChemicalElements.Aluminium, ChemicalElements.Thallium, ChemicalElements.Tin, ChemicalElements.Lead, ChemicalElements.Bismuth);
            AddToMap(ids, "Transition Metal", ChemicalElements.Seaborgium, ChemicalElements.Hafnium,
                ChemicalElements.Roentgenium, ChemicalElements.Iridium, ChemicalElements.Nickel, ChemicalElements.Meitnerium, ChemicalElements.Yttrium, ChemicalElements.Copper, ChemicalElements.Rutherfordium, ChemicalElements.Tungsten, ChemicalElements.Copernicium,
                ChemicalElements.Rhodium, ChemicalElements.Cobalt, ChemicalElements.Zinc, ChemicalElements.Platinum, ChemicalElements.Gold, ChemicalElements.Cadmium, ChemicalElements.Manganese, ChemicalElements.Darmstadtium, ChemicalElements.Dubnium, ChemicalElements.Palladium, ChemicalElements.Vanadium,
                ChemicalElements.Titanium, ChemicalElements.Tantalum, ChemicalElements.Chromium, ChemicalElements.Molybdenum, ChemicalElements.Ruthenium, ChemicalElements.Zirconium, ChemicalElements.Osmium, ChemicalElements.Bohrium, ChemicalElements.Rhenium, ChemicalElements.Niobium,
                ChemicalElements.Scandium, ChemicalElements.Technetium, ChemicalElements.Hassium, ChemicalElements.Mercury, ChemicalElements.Iron, ChemicalElements.Silver);
            AddToMap(ids, "Lanthanide", ChemicalElements.Terbium, ChemicalElements.Samarium, ChemicalElements.Lutetium,
                ChemicalElements.Neodymium, ChemicalElements.Cerium, ChemicalElements.Europium, ChemicalElements.Gadolinium, ChemicalElements.Thulium, ChemicalElements.Lanthanum, ChemicalElements.Erbium, ChemicalElements.Promethium, ChemicalElements.Holmium, ChemicalElements.Praseodymium,
                ChemicalElements.Dysprosium, ChemicalElements.Ytterbium);
            AddToMap(ids, "Actinide", ChemicalElements.Fermium, ChemicalElements.Protactinium, ChemicalElements.Plutonium, ChemicalElements.Thorium, ChemicalElements.Lawrencium, ChemicalElements.Einsteinium,
                ChemicalElements.Nobelium, ChemicalElements.Actinium, ChemicalElements.Americium, ChemicalElements.Curium, ChemicalElements.Berkelium, ChemicalElements.Mendelevium, ChemicalElements.Uranium, ChemicalElements.Californium, ChemicalElements.Neptunium);
            return ids;
        }

        private static Dictionary<int, string> MakeMapToPhase()
        {
            var ids = new Dictionary<int, string>();
            AddToMap(ids, "Solid", ChemicalElements.Sulfur, ChemicalElements.Hafnium, ChemicalElements.Terbium, ChemicalElements.Calcium, ChemicalElements.Gadolinium, ChemicalElements.Nickel, ChemicalElements.Cerium, ChemicalElements.Germanium, ChemicalElements.Phosphorus, ChemicalElements.Copper, ChemicalElements.Polonium,
                ChemicalElements.Lead, ChemicalElements.Gold, ChemicalElements.Iodine, ChemicalElements.Cadmium, ChemicalElements.Ytterbium, ChemicalElements.Manganese, ChemicalElements.Lithium, ChemicalElements.Palladium, ChemicalElements.Vanadium, ChemicalElements.Chromium, ChemicalElements.Molybdenum,
                ChemicalElements.Potassium, ChemicalElements.Ruthenium, ChemicalElements.Osmium, ChemicalElements.Boron, ChemicalElements.Bismuth, ChemicalElements.Rhenium, ChemicalElements.Holmium, ChemicalElements.Niobium, ChemicalElements.Praseodymium, ChemicalElements.Barium,
                ChemicalElements.Antimony, ChemicalElements.Thallium, ChemicalElements.Iron, ChemicalElements.Silver, ChemicalElements.Silicon, ChemicalElements.Caesium, ChemicalElements.Astatine, ChemicalElements.Iridium, ChemicalElements.Francium, ChemicalElements.Lutetium, ChemicalElements.Yttrium,
                ChemicalElements.Rubidium, ChemicalElements.Lanthanum, ChemicalElements.Tungsten, ChemicalElements.Erbium, ChemicalElements.Selenium, ChemicalElements.Gallium, ChemicalElements.Carbon, ChemicalElements.Rhodium, ChemicalElements.Uranium, ChemicalElements.Dysprosium, ChemicalElements.Cobalt,
                ChemicalElements.Zinc, ChemicalElements.Platinum, ChemicalElements.Protactinium, ChemicalElements.Titanium, ChemicalElements.Arsenic, ChemicalElements.Tantalum, ChemicalElements.Thorium, ChemicalElements.Samarium, ChemicalElements.Europium, ChemicalElements.Neodymium,
                ChemicalElements.Zirconium, ChemicalElements.Radium, ChemicalElements.Thulium, ChemicalElements.Sodium, ChemicalElements.Scandium, ChemicalElements.Tellurium, ChemicalElements.Indium, ChemicalElements.Beryllium, ChemicalElements.Aluminium, ChemicalElements.Strontium, ChemicalElements.Tin,
                ChemicalElements.Magnesium);
            AddToMap(ids, "Liquid", ChemicalElements.Bromine, ChemicalElements.Mercury);
            AddToMap(ids, "Gas", ChemicalElements.Fluorine, ChemicalElements.Oxygen, ChemicalElements.Xenon, ChemicalElements.Argon, ChemicalElements.Chlorine, ChemicalElements.Helium, ChemicalElements.Krypton, ChemicalElements.Hydrogen, ChemicalElements.Radon, ChemicalElements.Nitrogen, ChemicalElements.Neon);
            AddToMap(ids, "Synthetic", ChemicalElements.Fermium, ChemicalElements.Seaborgium, ChemicalElements.Plutonium, ChemicalElements.Roentgenium, ChemicalElements.Lawrencium, ChemicalElements.Meitnerium, ChemicalElements.Einsteinium, ChemicalElements.Nobelium, ChemicalElements.Actinium,
                ChemicalElements.Rutherfordium, ChemicalElements.Americium, ChemicalElements.Curium, ChemicalElements.Bohrium, ChemicalElements.Berkelium, ChemicalElements.Promethium, ChemicalElements.Copernicium, ChemicalElements.Technetium, ChemicalElements.Hassium,
                ChemicalElements.Californium, ChemicalElements.Mendelevium, ChemicalElements.Neptunium, ChemicalElements.Darmstadtium, ChemicalElements.Dubnium);
            return ids;
        }
    }
}