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
    /// <see cref="NaturalElement"/> enumeration.
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
            return NaturalElement.OfString(symbol).VdwRadius;
        }
        
        /// <summary>
        /// Get the covalent radius for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the covalent radius</returns>
        public static double? GetCovalentRadius(string symbol)
        {
            return NaturalElement.OfString(symbol).CovalentRadius;
        }

        /// <summary>
        /// Get the CAS ID for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the CAS ID</returns>
        public static string GetCASId(string symbol)
        {
            return MapToCasId[NaturalElement.OfString(symbol).AtomicNumber]; 
        }

        /// <summary>
        /// Get the chemical series for an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the chemical series of the element</returns>
        public static string GetChemicalSeries(string symbol)
        {
            if (!MapToSeries.TryGetValue(NaturalElement.OfString(symbol).AtomicNumber, out string series))
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
            return NaturalElement.OfString(symbol).Group;
        }

        /// <summary>
        /// Get the name of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the name of the element</returns>
        public static string GetName(string symbol)
        {
            return NaturalElement.OfString(symbol).Name;
        }

        /// <summary>
        /// Get the period of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the period</returns>
        public static int GetPeriod(string symbol)
        {
            return NaturalElement.OfString(symbol).Period;
        }

        /// <summary>
        /// Get the phase of the element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the phase of the element</returns>
        public static string GetPhase(string symbol)
        {
            if (!MapToPhase.TryGetValue(NaturalElement.OfString(symbol).AtomicNumber, out string phase))
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
            return NaturalElement.OfString(symbol).AtomicNumber;
        }

        /// <summary>
        /// Get the Pauling electronegativity of an element.
        /// </summary>
        /// <param name="symbol">the symbol of the element</param>
        /// <returns>the Pauling electronegativity</returns>
        public static double? GetPaulingElectronegativity(string symbol)
        {
            return NaturalElement.OfString(symbol).Electronegativity;
        }

        /// <summary>
        /// Get the symbol for the specified atomic number.
        /// </summary>
        /// <param name="atomicNumber">the atomic number of the element</param>
        /// <returns>the corresponding symbol</returns>
        public static string GetSymbol(int atomicNumber)
        {
            return NaturalElement.OfNumber(atomicNumber).Symbol;
        }

        /// <summary>
        /// The number of elements in the periodic table
        /// </summary>
        public static int ElementCount => NaturalElement.Values.Count;

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
        
        private static void AddToMap(Dictionary<int, string> ids, string name, params NaturalElement[] elements)
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
                { NaturalElements.Unknown.AtomicNumber, "" },
                { NaturalElements.Hydrogen.AtomicNumber, "1333-74-0" },
                { NaturalElements.Helium.AtomicNumber, "7440-59-7" },
                { NaturalElements.Lithium.AtomicNumber, "7439-93-2" },
                { NaturalElements.Beryllium.AtomicNumber, "7440-41-7" },
                { NaturalElements.Boron.AtomicNumber, "7440-42-8" },
                { NaturalElements.Carbon.AtomicNumber, "7440-44-0" },
                { NaturalElements.Nitrogen.AtomicNumber, "7727-37-9" },
                { NaturalElements.Oxygen.AtomicNumber, "7782-44-7" },
                { NaturalElements.Fluorine.AtomicNumber, "7782-41-4" },
                { NaturalElements.Neon.AtomicNumber, "7440-01-9" },
                { NaturalElements.Sodium.AtomicNumber, "7440-23-5" },
                { NaturalElements.Magnesium.AtomicNumber, "7439-95-4" },
                { NaturalElements.Aluminium.AtomicNumber, "7429-90-5" },
                { NaturalElements.Silicon.AtomicNumber, "7440-21-3" },
                { NaturalElements.Phosphorus.AtomicNumber, "7723-14-0" },
                { NaturalElements.Sulfur.AtomicNumber, "7704-34-9" },
                { NaturalElements.Chlorine.AtomicNumber, "7782-50-5" },
                { NaturalElements.Argon.AtomicNumber, "7440-37-1" },
                { NaturalElements.Potassium.AtomicNumber, "7440-09-7" },
                { NaturalElements.Calcium.AtomicNumber, "7440-70-2" },
                { NaturalElements.Scandium.AtomicNumber, "7440-20-2" },
                { NaturalElements.Titanium.AtomicNumber, "7440-32-6" },
                { NaturalElements.Vanadium.AtomicNumber, "7440-62-2" },
                { NaturalElements.Chromium.AtomicNumber, "7440-47-3" },
                { NaturalElements.Manganese.AtomicNumber, "7439-96-5" },
                { NaturalElements.Iron.AtomicNumber, "7439-89-6" },
                { NaturalElements.Cobalt.AtomicNumber, "7440-48-4" },
                { NaturalElements.Nickel.AtomicNumber, "7440-02-0" },
                { NaturalElements.Copper.AtomicNumber, "7440-50-8" },
                { NaturalElements.Zinc.AtomicNumber, "7440-66-6" },
                { NaturalElements.Gallium.AtomicNumber, "7440-55-3" },
                { NaturalElements.Germanium.AtomicNumber, "7440-56-4" },
                { NaturalElements.Arsenic.AtomicNumber, "7440-38-2" },
                { NaturalElements.Selenium.AtomicNumber, "7782-49-2" },
                { NaturalElements.Bromine.AtomicNumber, "7726-95-6" },
                { NaturalElements.Krypton.AtomicNumber, "7439-90-9" },
                { NaturalElements.Rubidium.AtomicNumber, "7440-17-7" },
                { NaturalElements.Strontium.AtomicNumber, "7440-24-6" },
                { NaturalElements.Yttrium.AtomicNumber, "7440-65-5" },
                { NaturalElements.Zirconium.AtomicNumber, "7440-67-7" },
                { NaturalElements.Niobium.AtomicNumber, "7440-03-1" },
                { NaturalElements.Molybdenum.AtomicNumber, "7439-98-7" },
                { NaturalElements.Technetium.AtomicNumber, "7440-26-8" },
                { NaturalElements.Ruthenium.AtomicNumber, "7440-18-8" },
                { NaturalElements.Rhodium.AtomicNumber, "7440-16-6" },
                { NaturalElements.Palladium.AtomicNumber, "7440-05-3" },
                { NaturalElements.Silver.AtomicNumber, "7440-22-4" },
                { NaturalElements.Cadmium.AtomicNumber, "7440-43-9" },
                { NaturalElements.Indium.AtomicNumber, "7440-74-6" },
                { NaturalElements.Tin.AtomicNumber, "7440-31-5" },
                { NaturalElements.Antimony.AtomicNumber, "7440-36-0" },
                { NaturalElements.Tellurium.AtomicNumber, "13494-80-9" },
                { NaturalElements.Iodine.AtomicNumber, "7553-56-2" },
                { NaturalElements.Xenon.AtomicNumber, "7440-63-3" },
                { NaturalElements.Caesium.AtomicNumber, "7440-46-2" },
                { NaturalElements.Barium.AtomicNumber, "7440-39-3" },
                { NaturalElements.Lanthanum.AtomicNumber, "7439-91-0" },
                { NaturalElements.Cerium.AtomicNumber, "7440-45-1" },
                { NaturalElements.Praseodymium.AtomicNumber, "7440-10-0" },
                { NaturalElements.Neodymium.AtomicNumber, "7440-00-8" },
                { NaturalElements.Promethium.AtomicNumber, "7440-12-2" },
                { NaturalElements.Samarium.AtomicNumber, "7440-19-9" },
                { NaturalElements.Europium.AtomicNumber, "7440-53-1" },
                { NaturalElements.Gadolinium.AtomicNumber, "7440-54-2" },
                { NaturalElements.Terbium.AtomicNumber, "7440-27-9" },
                { NaturalElements.Dysprosium.AtomicNumber, "7429-91-6" },
                { NaturalElements.Holmium.AtomicNumber, "7440-60-0" },
                { NaturalElements.Erbium.AtomicNumber, "7440-52-0" },
                { NaturalElements.Thulium.AtomicNumber, "7440-30-4" },
                { NaturalElements.Ytterbium.AtomicNumber, "7440-64-4" },
                { NaturalElements.Lutetium.AtomicNumber, "7439-94-3" },
                { NaturalElements.Hafnium.AtomicNumber, "7440-58-6" },
                { NaturalElements.Tantalum.AtomicNumber, "7440-25-7" },
                { NaturalElements.Tungsten.AtomicNumber, "7440-33-7" },
                { NaturalElements.Rhenium.AtomicNumber, "7440-15-5" },
                { NaturalElements.Osmium.AtomicNumber, "7440-04-2" },
                { NaturalElements.Iridium.AtomicNumber, "7439-88-5" },
                { NaturalElements.Platinum.AtomicNumber, "7440-06-4" },
                { NaturalElements.Gold.AtomicNumber, "7440-57-5" },
                { NaturalElements.Mercury.AtomicNumber, "7439-97-6" },
                { NaturalElements.Thallium.AtomicNumber, "7440-28-0" },
                { NaturalElements.Lead.AtomicNumber, "7439-92-1" },
                { NaturalElements.Bismuth.AtomicNumber, "7440-69-9" },
                { NaturalElements.Polonium.AtomicNumber, "7440-08-6" },
                { NaturalElements.Astatine.AtomicNumber, "7440-08-6" },
                { NaturalElements.Radon.AtomicNumber, "10043-92-2" },
                { NaturalElements.Francium.AtomicNumber, "7440-73-5" },
                { NaturalElements.Radium.AtomicNumber, "7440-14-4" },
                { NaturalElements.Actinium.AtomicNumber, "7440-34-8" },
                { NaturalElements.Thorium.AtomicNumber, "7440-29-1" },
                { NaturalElements.Protactinium.AtomicNumber, "7440-13-3" },
                { NaturalElements.Uranium.AtomicNumber, "7440-61-1" },
                { NaturalElements.Neptunium.AtomicNumber, "7439-99-8" },
                { NaturalElements.Plutonium.AtomicNumber, "7440-07-5" },
                { NaturalElements.Americium.AtomicNumber, "7440-35-9" },
                { NaturalElements.Curium.AtomicNumber, "7440-51-9" },
                { NaturalElements.Berkelium.AtomicNumber, "7440-40-6" },
                { NaturalElements.Californium.AtomicNumber, "7440-71-3" },
                { NaturalElements.Einsteinium.AtomicNumber, "7429-92-7" },
                { NaturalElements.Fermium.AtomicNumber, "7440-72-4" },
                { NaturalElements.Mendelevium.AtomicNumber, "7440-11-1" },
                { NaturalElements.Nobelium.AtomicNumber, "10028-14-5" },
                { NaturalElements.Lawrencium.AtomicNumber, "22537-19-5" },
                { NaturalElements.Rutherfordium.AtomicNumber, "53850-36-5" },
                { NaturalElements.Dubnium.AtomicNumber, "53850-35-4" },
                { NaturalElements.Seaborgium.AtomicNumber, "54038-81-2" },
                { NaturalElements.Bohrium.AtomicNumber, "54037-14-8" },
                { NaturalElements.Hassium.AtomicNumber, "54037-57-9" },
                { NaturalElements.Meitnerium.AtomicNumber, "54038-01-6" },
                { NaturalElements.Darmstadtium.AtomicNumber, "54083-77-1" },
                { NaturalElements.Roentgenium.AtomicNumber, "54386-24-2" },
                { NaturalElements.Copernicium.AtomicNumber, "54084-26-3" },
                { NaturalElements.Ununtrium.AtomicNumber, "" },
                { NaturalElements.Flerovium.AtomicNumber, "54085-16-4" },
                { NaturalElements.Ununpentium.AtomicNumber, "" },
                { NaturalElements.Livermorium.AtomicNumber, "54100-71-9" },
                { NaturalElements.Ununseptium.AtomicNumber, "" },
                { NaturalElements.Ununoctium.AtomicNumber, "" }
            };

            return ids;
        }
        
        private static Dictionary<int, string> MakeMapToSeries()
        {
            var ids = new Dictionary<int, string>();
            AddToMap(ids, "Non Metal", NaturalElements.Sulfur, NaturalElements.Selenium, NaturalElements.Oxygen, NaturalElements.Carbon, NaturalElements.Phosphorus, NaturalElements.Hydrogen, NaturalElements.Nitrogen);
            AddToMap(ids, "Noble Gas", NaturalElements.Helium, NaturalElements.Krypton, NaturalElements.Xenon, NaturalElements.Argon, NaturalElements.Radon, NaturalElements.Neon);
            AddToMap(ids, "Alkali Metal", NaturalElements.Sodium, NaturalElements.Rubidium, NaturalElements.Potassium, NaturalElements.Caesium, NaturalElements.Francium, NaturalElements.Lithium);
            AddToMap(ids, "Alkali Earth Metal", NaturalElements.Strontium, NaturalElements.Radium, NaturalElements.Calcium, NaturalElements.Magnesium, NaturalElements.Barium, NaturalElements.Beryllium);
            AddToMap(ids, "Metalloid", NaturalElements.Silicon, NaturalElements.Arsenic, NaturalElements.Tellurium, NaturalElements.Germanium, NaturalElements.Antimony, NaturalElements.Polonium, NaturalElements.Boron);
            AddToMap(ids, "Halogen", NaturalElements.Fluorine, NaturalElements.Iodine, NaturalElements.Chlorine, NaturalElements.Astatine, NaturalElements.Bromine);
            AddToMap(ids, "Metal", NaturalElements.Gallium, NaturalElements.Indium, NaturalElements.Aluminium, NaturalElements.Thallium, NaturalElements.Tin, NaturalElements.Lead, NaturalElements.Bismuth);
            AddToMap(ids, "Transition Metal", NaturalElements.Seaborgium, NaturalElements.Hafnium,
                NaturalElements.Roentgenium, NaturalElements.Iridium, NaturalElements.Nickel, NaturalElements.Meitnerium, NaturalElements.Yttrium, NaturalElements.Copper, NaturalElements.Rutherfordium, NaturalElements.Tungsten, NaturalElements.Copernicium,
                NaturalElements.Rhodium, NaturalElements.Cobalt, NaturalElements.Zinc, NaturalElements.Platinum, NaturalElements.Gold, NaturalElements.Cadmium, NaturalElements.Manganese, NaturalElements.Darmstadtium, NaturalElements.Dubnium, NaturalElements.Palladium, NaturalElements.Vanadium,
                NaturalElements.Titanium, NaturalElements.Tantalum, NaturalElements.Chromium, NaturalElements.Molybdenum, NaturalElements.Ruthenium, NaturalElements.Zirconium, NaturalElements.Osmium, NaturalElements.Bohrium, NaturalElements.Rhenium, NaturalElements.Niobium,
                NaturalElements.Scandium, NaturalElements.Technetium, NaturalElements.Hassium, NaturalElements.Mercury, NaturalElements.Iron, NaturalElements.Silver);
            AddToMap(ids, "Lanthanide", NaturalElements.Terbium, NaturalElements.Samarium, NaturalElements.Lutetium,
                NaturalElements.Neodymium, NaturalElements.Cerium, NaturalElements.Europium, NaturalElements.Gadolinium, NaturalElements.Thulium, NaturalElements.Lanthanum, NaturalElements.Erbium, NaturalElements.Promethium, NaturalElements.Holmium, NaturalElements.Praseodymium,
                NaturalElements.Dysprosium, NaturalElements.Ytterbium);
            AddToMap(ids, "Actinide", NaturalElements.Fermium, NaturalElements.Protactinium, NaturalElements.Plutonium, NaturalElements.Thorium, NaturalElements.Lawrencium, NaturalElements.Einsteinium,
                NaturalElements.Nobelium, NaturalElements.Actinium, NaturalElements.Americium, NaturalElements.Curium, NaturalElements.Berkelium, NaturalElements.Mendelevium, NaturalElements.Uranium, NaturalElements.Californium, NaturalElements.Neptunium);
            return ids;
        }

        private static Dictionary<int, string> MakeMapToPhase()
        {
            var ids = new Dictionary<int, string>();
            AddToMap(ids, "Solid", NaturalElements.Sulfur, NaturalElements.Hafnium, NaturalElements.Terbium, NaturalElements.Calcium, NaturalElements.Gadolinium, NaturalElements.Nickel, NaturalElements.Cerium, NaturalElements.Germanium, NaturalElements.Phosphorus, NaturalElements.Copper, NaturalElements.Polonium,
                NaturalElements.Lead, NaturalElements.Gold, NaturalElements.Iodine, NaturalElements.Cadmium, NaturalElements.Ytterbium, NaturalElements.Manganese, NaturalElements.Lithium, NaturalElements.Palladium, NaturalElements.Vanadium, NaturalElements.Chromium, NaturalElements.Molybdenum,
                NaturalElements.Potassium, NaturalElements.Ruthenium, NaturalElements.Osmium, NaturalElements.Boron, NaturalElements.Bismuth, NaturalElements.Rhenium, NaturalElements.Holmium, NaturalElements.Niobium, NaturalElements.Praseodymium, NaturalElements.Barium,
                NaturalElements.Antimony, NaturalElements.Thallium, NaturalElements.Iron, NaturalElements.Silver, NaturalElements.Silicon, NaturalElements.Caesium, NaturalElements.Astatine, NaturalElements.Iridium, NaturalElements.Francium, NaturalElements.Lutetium, NaturalElements.Yttrium,
                NaturalElements.Rubidium, NaturalElements.Lanthanum, NaturalElements.Tungsten, NaturalElements.Erbium, NaturalElements.Selenium, NaturalElements.Gallium, NaturalElements.Carbon, NaturalElements.Rhodium, NaturalElements.Uranium, NaturalElements.Dysprosium, NaturalElements.Cobalt,
                NaturalElements.Zinc, NaturalElements.Platinum, NaturalElements.Protactinium, NaturalElements.Titanium, NaturalElements.Arsenic, NaturalElements.Tantalum, NaturalElements.Thorium, NaturalElements.Samarium, NaturalElements.Europium, NaturalElements.Neodymium,
                NaturalElements.Zirconium, NaturalElements.Radium, NaturalElements.Thulium, NaturalElements.Sodium, NaturalElements.Scandium, NaturalElements.Tellurium, NaturalElements.Indium, NaturalElements.Beryllium, NaturalElements.Aluminium, NaturalElements.Strontium, NaturalElements.Tin,
                NaturalElements.Magnesium);
            AddToMap(ids, "Liquid", NaturalElements.Bromine, NaturalElements.Mercury);
            AddToMap(ids, "Gas", NaturalElements.Fluorine, NaturalElements.Oxygen, NaturalElements.Xenon, NaturalElements.Argon, NaturalElements.Chlorine, NaturalElements.Helium, NaturalElements.Krypton, NaturalElements.Hydrogen, NaturalElements.Radon, NaturalElements.Nitrogen, NaturalElements.Neon);
            AddToMap(ids, "Synthetic", NaturalElements.Fermium, NaturalElements.Seaborgium, NaturalElements.Plutonium, NaturalElements.Roentgenium, NaturalElements.Lawrencium, NaturalElements.Meitnerium, NaturalElements.Einsteinium, NaturalElements.Nobelium, NaturalElements.Actinium,
                NaturalElements.Rutherfordium, NaturalElements.Americium, NaturalElements.Curium, NaturalElements.Bohrium, NaturalElements.Berkelium, NaturalElements.Promethium, NaturalElements.Copernicium, NaturalElements.Technetium, NaturalElements.Hassium,
                NaturalElements.Californium, NaturalElements.Mendelevium, NaturalElements.Neptunium, NaturalElements.Darmstadtium, NaturalElements.Dubnium);
            return ids;
        }
    }
}