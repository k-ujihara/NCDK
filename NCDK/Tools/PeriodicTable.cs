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
    /// <see cref="Elements"/> enumeration.
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
            return Elements.OfString(symbol).VdwRadius;
        }
        
        /// <summary>
        /// Get the covalent radius for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the covalent radius</returns>
        public static double? GetCovalentRadius(string symbol)
        {
            return Elements.OfString(symbol).CovalentRadius;
        }

        /// <summary>
        /// Get the CAS ID for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the CAS ID</returns>
        public static string GetCASId(string symbol)
        {
            return MapToCasId[Elements.OfString(symbol).AtomicNumber]; 
        }

        /// <summary>
        /// Get the chemical series for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the chemical series of the element</returns>
        public static string GetChemicalSeries(string symbol)
        {
            if (!MapToSeries.TryGetValue(Elements.OfString(symbol).AtomicNumber, out string series))
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
            return Elements.OfString(symbol).Group;
        }

        /// <summary>
        /// Get the name of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the name of the element</returns>
        public static string GetName(string symbol)
        {
            return Elements.OfString(symbol).Name;
        }

        /// <summary>
        /// Get the period of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the period</returns>
        public static int GetPeriod(string symbol)
        {
            return Elements.OfString(symbol).Period;
        }

        /// <summary>
        /// Get the phase of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the phase of the element</returns>
        public static string GetPhase(string symbol)
        {
            if (!MapToPhase.TryGetValue(Elements.OfString(symbol).AtomicNumber, out string phase))
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
            return Elements.OfString(symbol).AtomicNumber;
        }

        /// <summary>
        /// Get the Pauling electronegativity of an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the Pauling electronegativity</returns>
        public static double? GetPaulingElectronegativity(string symbol)
        {
            return Elements.OfString(symbol).Electronegativity;
        }

        /// <summary>
        /// Get the symbol for the specified atomic number.
        /// </summary>
        /// <param name="atomicNumber">the atomic number of the element</param>
        /// <returns>the corresponding symbol</returns>
        public static string GetSymbol(int atomicNumber)
        {
            return Elements.OfNumber(atomicNumber).Symbol;
        }

        /// <summary>
        /// The number of elements in the periodic table
        /// </summary>
        public static int ElementCount => Elements.Values.Length;

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
        
        private static void AddToMap(Dictionary<int, string> ids, string name, params Elements[] elements)
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
                { Elements.Unknown.AtomicNumber, "" },
                { Elements.Hydrogen.AtomicNumber, "1333-74-0" },
                { Elements.Helium.AtomicNumber, "7440-59-7" },
                { Elements.Lithium.AtomicNumber, "7439-93-2" },
                { Elements.Beryllium.AtomicNumber, "7440-41-7" },
                { Elements.Boron.AtomicNumber, "7440-42-8" },
                { Elements.Carbon.AtomicNumber, "7440-44-0" },
                { Elements.Nitrogen.AtomicNumber, "7727-37-9" },
                { Elements.Oxygen.AtomicNumber, "7782-44-7" },
                { Elements.Fluorine.AtomicNumber, "7782-41-4" },
                { Elements.Neon.AtomicNumber, "7440-01-9" },
                { Elements.Sodium.AtomicNumber, "7440-23-5" },
                { Elements.Magnesium.AtomicNumber, "7439-95-4" },
                { Elements.Aluminium.AtomicNumber, "7429-90-5" },
                { Elements.Silicon.AtomicNumber, "7440-21-3" },
                { Elements.Phosphorus.AtomicNumber, "7723-14-0" },
                { Elements.Sulfur.AtomicNumber, "7704-34-9" },
                { Elements.Chlorine.AtomicNumber, "7782-50-5" },
                { Elements.Argon.AtomicNumber, "7440-37-1" },
                { Elements.Potassium.AtomicNumber, "7440-09-7" },
                { Elements.Calcium.AtomicNumber, "7440-70-2" },
                { Elements.Scandium.AtomicNumber, "7440-20-2" },
                { Elements.Titanium.AtomicNumber, "7440-32-6" },
                { Elements.Vanadium.AtomicNumber, "7440-62-2" },
                { Elements.Chromium.AtomicNumber, "7440-47-3" },
                { Elements.Manganese.AtomicNumber, "7439-96-5" },
                { Elements.Iron.AtomicNumber, "7439-89-6" },
                { Elements.Cobalt.AtomicNumber, "7440-48-4" },
                { Elements.Nickel.AtomicNumber, "7440-02-0" },
                { Elements.Copper.AtomicNumber, "7440-50-8" },
                { Elements.Zinc.AtomicNumber, "7440-66-6" },
                { Elements.Gallium.AtomicNumber, "7440-55-3" },
                { Elements.Germanium.AtomicNumber, "7440-56-4" },
                { Elements.Arsenic.AtomicNumber, "7440-38-2" },
                { Elements.Selenium.AtomicNumber, "7782-49-2" },
                { Elements.Bromine.AtomicNumber, "7726-95-6" },
                { Elements.Krypton.AtomicNumber, "7439-90-9" },
                { Elements.Rubidium.AtomicNumber, "7440-17-7" },
                { Elements.Strontium.AtomicNumber, "7440-24-6" },
                { Elements.Yttrium.AtomicNumber, "7440-65-5" },
                { Elements.Zirconium.AtomicNumber, "7440-67-7" },
                { Elements.Niobium.AtomicNumber, "7440-03-1" },
                { Elements.Molybdenum.AtomicNumber, "7439-98-7" },
                { Elements.Technetium.AtomicNumber, "7440-26-8" },
                { Elements.Ruthenium.AtomicNumber, "7440-18-8" },
                { Elements.Rhodium.AtomicNumber, "7440-16-6" },
                { Elements.Palladium.AtomicNumber, "7440-05-3" },
                { Elements.Silver.AtomicNumber, "7440-22-4" },
                { Elements.Cadmium.AtomicNumber, "7440-43-9" },
                { Elements.Indium.AtomicNumber, "7440-74-6" },
                { Elements.Tin.AtomicNumber, "7440-31-5" },
                { Elements.Antimony.AtomicNumber, "7440-36-0" },
                { Elements.Tellurium.AtomicNumber, "13494-80-9" },
                { Elements.Iodine.AtomicNumber, "7553-56-2" },
                { Elements.Xenon.AtomicNumber, "7440-63-3" },
                { Elements.Caesium.AtomicNumber, "7440-46-2" },
                { Elements.Barium.AtomicNumber, "7440-39-3" },
                { Elements.Lanthanum.AtomicNumber, "7439-91-0" },
                { Elements.Cerium.AtomicNumber, "7440-45-1" },
                { Elements.Praseodymium.AtomicNumber, "7440-10-0" },
                { Elements.Neodymium.AtomicNumber, "7440-00-8" },
                { Elements.Promethium.AtomicNumber, "7440-12-2" },
                { Elements.Samarium.AtomicNumber, "7440-19-9" },
                { Elements.Europium.AtomicNumber, "7440-53-1" },
                { Elements.Gadolinium.AtomicNumber, "7440-54-2" },
                { Elements.Terbium.AtomicNumber, "7440-27-9" },
                { Elements.Dysprosium.AtomicNumber, "7429-91-6" },
                { Elements.Holmium.AtomicNumber, "7440-60-0" },
                { Elements.Erbium.AtomicNumber, "7440-52-0" },
                { Elements.Thulium.AtomicNumber, "7440-30-4" },
                { Elements.Ytterbium.AtomicNumber, "7440-64-4" },
                { Elements.Lutetium.AtomicNumber, "7439-94-3" },
                { Elements.Hafnium.AtomicNumber, "7440-58-6" },
                { Elements.Tantalum.AtomicNumber, "7440-25-7" },
                { Elements.Tungsten.AtomicNumber, "7440-33-7" },
                { Elements.Rhenium.AtomicNumber, "7440-15-5" },
                { Elements.Osmium.AtomicNumber, "7440-04-2" },
                { Elements.Iridium.AtomicNumber, "7439-88-5" },
                { Elements.Platinum.AtomicNumber, "7440-06-4" },
                { Elements.Gold.AtomicNumber, "7440-57-5" },
                { Elements.Mercury.AtomicNumber, "7439-97-6" },
                { Elements.Thallium.AtomicNumber, "7440-28-0" },
                { Elements.Lead.AtomicNumber, "7439-92-1" },
                { Elements.Bismuth.AtomicNumber, "7440-69-9" },
                { Elements.Polonium.AtomicNumber, "7440-08-6" },
                { Elements.Astatine.AtomicNumber, "7440-08-6" },
                { Elements.Radon.AtomicNumber, "10043-92-2" },
                { Elements.Francium.AtomicNumber, "7440-73-5" },
                { Elements.Radium.AtomicNumber, "7440-14-4" },
                { Elements.Actinium.AtomicNumber, "7440-34-8" },
                { Elements.Thorium.AtomicNumber, "7440-29-1" },
                { Elements.Protactinium.AtomicNumber, "7440-13-3" },
                { Elements.Uranium.AtomicNumber, "7440-61-1" },
                { Elements.Neptunium.AtomicNumber, "7439-99-8" },
                { Elements.Plutonium.AtomicNumber, "7440-07-5" },
                { Elements.Americium.AtomicNumber, "7440-35-9" },
                { Elements.Curium.AtomicNumber, "7440-51-9" },
                { Elements.Berkelium.AtomicNumber, "7440-40-6" },
                { Elements.Californium.AtomicNumber, "7440-71-3" },
                { Elements.Einsteinium.AtomicNumber, "7429-92-7" },
                { Elements.Fermium.AtomicNumber, "7440-72-4" },
                { Elements.Mendelevium.AtomicNumber, "7440-11-1" },
                { Elements.Nobelium.AtomicNumber, "10028-14-5" },
                { Elements.Lawrencium.AtomicNumber, "22537-19-5" },
                { Elements.Rutherfordium.AtomicNumber, "53850-36-5" },
                { Elements.Dubnium.AtomicNumber, "53850-35-4" },
                { Elements.Seaborgium.AtomicNumber, "54038-81-2" },
                { Elements.Bohrium.AtomicNumber, "54037-14-8" },
                { Elements.Hassium.AtomicNumber, "54037-57-9" },
                { Elements.Meitnerium.AtomicNumber, "54038-01-6" },
                { Elements.Darmstadtium.AtomicNumber, "54083-77-1" },
                { Elements.Roentgenium.AtomicNumber, "54386-24-2" },
                { Elements.Copernicium.AtomicNumber, "54084-26-3" },
                { Elements.Ununtrium.AtomicNumber, "" },
                { Elements.Flerovium.AtomicNumber, "54085-16-4" },
                { Elements.Ununpentium.AtomicNumber, "" },
                { Elements.Livermorium.AtomicNumber, "54100-71-9" },
                { Elements.Ununseptium.AtomicNumber, "" },
                { Elements.Ununoctium.AtomicNumber, "" }
            };

            return ids;
        }
        
        private static Dictionary<int, string> MakeMapToSeries()
        {
            var ids = new Dictionary<int, string>();
            AddToMap(ids, "Non Metal", Elements.Sulfur, Elements.Selenium, Elements.Oxygen, Elements.Carbon, Elements.Phosphorus, Elements.Hydrogen, Elements.Nitrogen);
            AddToMap(ids, "Noble Gas", Elements.Helium, Elements.Krypton, Elements.Xenon, Elements.Argon, Elements.Radon, Elements.Neon);
            AddToMap(ids, "Alkali Metal", Elements.Sodium, Elements.Rubidium, Elements.Potassium, Elements.Caesium, Elements.Francium, Elements.Lithium);
            AddToMap(ids, "Alkali Earth Metal", Elements.Strontium, Elements.Radium, Elements.Calcium, Elements.Magnesium, Elements.Barium, Elements.Beryllium);
            AddToMap(ids, "Metalloid", Elements.Silicon, Elements.Arsenic, Elements.Tellurium, Elements.Germanium, Elements.Antimony, Elements.Polonium, Elements.Boron);
            AddToMap(ids, "Halogen", Elements.Fluorine, Elements.Iodine, Elements.Chlorine, Elements.Astatine, Elements.Bromine);
            AddToMap(ids, "Metal", Elements.Gallium, Elements.Indium, Elements.Aluminium, Elements.Thallium, Elements.Tin, Elements.Lead, Elements.Bismuth);
            AddToMap(ids, "Transition Metal", Elements.Seaborgium, Elements.Hafnium,
                Elements.Roentgenium, Elements.Iridium, Elements.Nickel, Elements.Meitnerium, Elements.Yttrium, Elements.Copper, Elements.Rutherfordium, Elements.Tungsten, Elements.Copernicium,
                Elements.Rhodium, Elements.Cobalt, Elements.Zinc, Elements.Platinum, Elements.Gold, Elements.Cadmium, Elements.Manganese, Elements.Darmstadtium, Elements.Dubnium, Elements.Palladium, Elements.Vanadium,
                Elements.Titanium, Elements.Tantalum, Elements.Chromium, Elements.Molybdenum, Elements.Ruthenium, Elements.Zirconium, Elements.Osmium, Elements.Bohrium, Elements.Rhenium, Elements.Niobium,
                Elements.Scandium, Elements.Technetium, Elements.Hassium, Elements.Mercury, Elements.Iron, Elements.Silver);
            AddToMap(ids, "Lanthanide", Elements.Terbium, Elements.Samarium, Elements.Lutetium,
                Elements.Neodymium, Elements.Cerium, Elements.Europium, Elements.Gadolinium, Elements.Thulium, Elements.Lanthanum, Elements.Erbium, Elements.Promethium, Elements.Holmium, Elements.Praseodymium,
                Elements.Dysprosium, Elements.Ytterbium);
            AddToMap(ids, "Actinide", Elements.Fermium, Elements.Protactinium, Elements.Plutonium, Elements.Thorium, Elements.Lawrencium, Elements.Einsteinium,
                Elements.Nobelium, Elements.Actinium, Elements.Americium, Elements.Curium, Elements.Berkelium, Elements.Mendelevium, Elements.Uranium, Elements.Californium, Elements.Neptunium);
            return ids;
        }

        private static Dictionary<int, string> MakeMapToPhase()
        {
            var ids = new Dictionary<int, string>();
            AddToMap(ids, "Solid", Elements.Sulfur, Elements.Hafnium, Elements.Terbium, Elements.Calcium, Elements.Gadolinium, Elements.Nickel, Elements.Cerium, Elements.Germanium, Elements.Phosphorus, Elements.Copper, Elements.Polonium,
                Elements.Lead, Elements.Gold, Elements.Iodine, Elements.Cadmium, Elements.Ytterbium, Elements.Manganese, Elements.Lithium, Elements.Palladium, Elements.Vanadium, Elements.Chromium, Elements.Molybdenum,
                Elements.Potassium, Elements.Ruthenium, Elements.Osmium, Elements.Boron, Elements.Bismuth, Elements.Rhenium, Elements.Holmium, Elements.Niobium, Elements.Praseodymium, Elements.Barium,
                Elements.Antimony, Elements.Thallium, Elements.Iron, Elements.Silver, Elements.Silicon, Elements.Caesium, Elements.Astatine, Elements.Iridium, Elements.Francium, Elements.Lutetium, Elements.Yttrium,
                Elements.Rubidium, Elements.Lanthanum, Elements.Tungsten, Elements.Erbium, Elements.Selenium, Elements.Gallium, Elements.Carbon, Elements.Rhodium, Elements.Uranium, Elements.Dysprosium, Elements.Cobalt,
                Elements.Zinc, Elements.Platinum, Elements.Protactinium, Elements.Titanium, Elements.Arsenic, Elements.Tantalum, Elements.Thorium, Elements.Samarium, Elements.Europium, Elements.Neodymium,
                Elements.Zirconium, Elements.Radium, Elements.Thulium, Elements.Sodium, Elements.Scandium, Elements.Tellurium, Elements.Indium, Elements.Beryllium, Elements.Aluminium, Elements.Strontium, Elements.Tin,
                Elements.Magnesium);
            AddToMap(ids, "Liquid", Elements.Bromine, Elements.Mercury);
            AddToMap(ids, "Gas", Elements.Fluorine, Elements.Oxygen, Elements.Xenon, Elements.Argon, Elements.Chlorine, Elements.Helium, Elements.Krypton, Elements.Hydrogen, Elements.Radon, Elements.Nitrogen, Elements.Neon);
            AddToMap(ids, "Synthetic", Elements.Fermium, Elements.Seaborgium, Elements.Plutonium, Elements.Roentgenium, Elements.Lawrencium, Elements.Meitnerium, Elements.Einsteinium, Elements.Nobelium, Elements.Actinium,
                Elements.Rutherfordium, Elements.Americium, Elements.Curium, Elements.Bohrium, Elements.Berkelium, Elements.Promethium, Elements.Copernicium, Elements.Technetium, Elements.Hassium,
                Elements.Californium, Elements.Mendelevium, Elements.Neptunium, Elements.Darmstadtium, Elements.Dubnium);
            return ids;
        }
    }
}