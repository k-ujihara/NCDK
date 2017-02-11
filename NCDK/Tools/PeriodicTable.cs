using System;
using System.Collections.Generic;
using NCDK.Config;

namespace NCDK.Tools
{
    public sealed class PeriodicTable
    {
        private readonly static object syncLock = new object();

        public static double? GetVdwRadius(string symbol)
        {
            return Elements.OfString(symbol).VdwRadius;
        }

        public static double? GetCovalentRadius(string symbol)
        {
            return Elements.OfString(symbol).CovalentRadius;
        }

        public static string GetCASId(string symbol)
        {
            return MapToCasId[Elements.OfString(symbol).AtomicNumber]; 
        }

        public static string GetChemicalSeries(string symbol)
        {
            string series;
            if (!MapToSeries.TryGetValue(Elements.OfString(symbol).AtomicNumber, out series))
                return "";
            return series;
        }

        public static int GetGroup(string symbol)
        {
            return Elements.OfString(symbol).Group;
        }

        public static string GetName(string symbol)
        {
            return Elements.OfString(symbol).Name;
        }

        public static int GetPeriod(string symbol)
        {
            return Elements.OfString(symbol).Period;
        }

        public static string GetPhase(string symbol)
        {
            string phase;
            if (!MapToPhase.TryGetValue(Elements.OfString(symbol).AtomicNumber, out phase))
                return "";
            return phase;
        }

        public static int GetAtomicNumber(string symbol)
        {
            return Elements.OfString(symbol).AtomicNumber;
        }

        public static double? GetPaulingElectronegativity(string symbol)
        {
            return Elements.OfString(symbol).Electronegativity;
        }

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
            var ids = new Dictionary<int, string>();

            ids.Add(Elements.Unknown.AtomicNumber, "");
            ids.Add(Elements.Hydrogen.AtomicNumber, "1333-74-0");
            ids.Add(Elements.Helium.AtomicNumber, "7440-59-7");
            ids.Add(Elements.Lithium.AtomicNumber, "7439-93-2");
            ids.Add(Elements.Beryllium.AtomicNumber, "7440-41-7");
            ids.Add(Elements.Boron.AtomicNumber, "7440-42-8");
            ids.Add(Elements.Carbon.AtomicNumber, "7440-44-0");
            ids.Add(Elements.Nitrogen.AtomicNumber, "7727-37-9");
            ids.Add(Elements.Oxygen.AtomicNumber, "7782-44-7");
            ids.Add(Elements.Fluorine.AtomicNumber, "7782-41-4");
            ids.Add(Elements.Neon.AtomicNumber, "7440-01-9");
            ids.Add(Elements.Sodium.AtomicNumber, "7440-23-5");
            ids.Add(Elements.Magnesium.AtomicNumber, "7439-95-4");
            ids.Add(Elements.Aluminium.AtomicNumber, "7429-90-5");
            ids.Add(Elements.Silicon.AtomicNumber, "7440-21-3");
            ids.Add(Elements.Phosphorus.AtomicNumber, "7723-14-0");
            ids.Add(Elements.Sulfur.AtomicNumber, "7704-34-9");
            ids.Add(Elements.Chlorine.AtomicNumber, "7782-50-5");
            ids.Add(Elements.Argon.AtomicNumber, "7440-37-1");
            ids.Add(Elements.Potassium.AtomicNumber, "7440-09-7");
            ids.Add(Elements.Calcium.AtomicNumber, "7440-70-2");
            ids.Add(Elements.Scandium.AtomicNumber, "7440-20-2");
            ids.Add(Elements.Titanium.AtomicNumber, "7440-32-6");
            ids.Add(Elements.Vanadium.AtomicNumber, "7440-62-2");
            ids.Add(Elements.Chromium.AtomicNumber, "7440-47-3");
            ids.Add(Elements.Manganese.AtomicNumber, "7439-96-5");
            ids.Add(Elements.Iron.AtomicNumber, "7439-89-6");
            ids.Add(Elements.Cobalt.AtomicNumber, "7440-48-4");
            ids.Add(Elements.Nickel.AtomicNumber, "7440-02-0");
            ids.Add(Elements.Copper.AtomicNumber, "7440-50-8");
            ids.Add(Elements.Zinc.AtomicNumber, "7440-66-6");
            ids.Add(Elements.Gallium.AtomicNumber, "7440-55-3");
            ids.Add(Elements.Germanium.AtomicNumber, "7440-56-4");
            ids.Add(Elements.Arsenic.AtomicNumber, "7440-38-2");
            ids.Add(Elements.Selenium.AtomicNumber, "7782-49-2");
            ids.Add(Elements.Bromine.AtomicNumber, "7726-95-6");
            ids.Add(Elements.Krypton.AtomicNumber, "7439-90-9");
            ids.Add(Elements.Rubidium.AtomicNumber, "7440-17-7");
            ids.Add(Elements.Strontium.AtomicNumber, "7440-24-6");
            ids.Add(Elements.Yttrium.AtomicNumber, "7440-65-5");
            ids.Add(Elements.Zirconium.AtomicNumber, "7440-67-7");
            ids.Add(Elements.Niobium.AtomicNumber, "7440-03-1");
            ids.Add(Elements.Molybdenum.AtomicNumber, "7439-98-7");
            ids.Add(Elements.Technetium.AtomicNumber, "7440-26-8");
            ids.Add(Elements.Ruthenium.AtomicNumber, "7440-18-8");
            ids.Add(Elements.Rhodium.AtomicNumber, "7440-16-6");
            ids.Add(Elements.Palladium.AtomicNumber, "7440-05-3");
            ids.Add(Elements.Silver.AtomicNumber, "7440-22-4");
            ids.Add(Elements.Cadmium.AtomicNumber, "7440-43-9");
            ids.Add(Elements.Indium.AtomicNumber, "7440-74-6");
            ids.Add(Elements.Tin.AtomicNumber, "7440-31-5");
            ids.Add(Elements.Antimony.AtomicNumber, "7440-36-0");
            ids.Add(Elements.Tellurium.AtomicNumber, "13494-80-9");
            ids.Add(Elements.Iodine.AtomicNumber, "7553-56-2");
            ids.Add(Elements.Xenon.AtomicNumber, "7440-63-3");
            ids.Add(Elements.Caesium.AtomicNumber, "7440-46-2");
            ids.Add(Elements.Barium.AtomicNumber, "7440-39-3");
            ids.Add(Elements.Lanthanum.AtomicNumber, "7439-91-0");
            ids.Add(Elements.Cerium.AtomicNumber, "7440-45-1");
            ids.Add(Elements.Praseodymium.AtomicNumber, "7440-10-0");
            ids.Add(Elements.Neodymium.AtomicNumber, "7440-00-8");
            ids.Add(Elements.Promethium.AtomicNumber, "7440-12-2");
            ids.Add(Elements.Samarium.AtomicNumber, "7440-19-9");
            ids.Add(Elements.Europium.AtomicNumber, "7440-53-1");
            ids.Add(Elements.Gadolinium.AtomicNumber, "7440-54-2");
            ids.Add(Elements.Terbium.AtomicNumber, "7440-27-9");
            ids.Add(Elements.Dysprosium.AtomicNumber, "7429-91-6");
            ids.Add(Elements.Holmium.AtomicNumber, "7440-60-0");
            ids.Add(Elements.Erbium.AtomicNumber, "7440-52-0");
            ids.Add(Elements.Thulium.AtomicNumber, "7440-30-4");
            ids.Add(Elements.Ytterbium.AtomicNumber, "7440-64-4");
            ids.Add(Elements.Lutetium.AtomicNumber, "7439-94-3");
            ids.Add(Elements.Hafnium.AtomicNumber, "7440-58-6");
            ids.Add(Elements.Tantalum.AtomicNumber, "7440-25-7");
            ids.Add(Elements.Tungsten.AtomicNumber, "7440-33-7");
            ids.Add(Elements.Rhenium.AtomicNumber, "7440-15-5");
            ids.Add(Elements.Osmium.AtomicNumber, "7440-04-2");
            ids.Add(Elements.Iridium.AtomicNumber, "7439-88-5");
            ids.Add(Elements.Platinum.AtomicNumber, "7440-06-4");
            ids.Add(Elements.Gold.AtomicNumber, "7440-57-5");
            ids.Add(Elements.Mercury.AtomicNumber, "7439-97-6");
            ids.Add(Elements.Thallium.AtomicNumber, "7440-28-0");
            ids.Add(Elements.Lead.AtomicNumber, "7439-92-1");
            ids.Add(Elements.Bismuth.AtomicNumber, "7440-69-9");
            ids.Add(Elements.Polonium.AtomicNumber, "7440-08-6");
            ids.Add(Elements.Astatine.AtomicNumber, "7440-08-6");
            ids.Add(Elements.Radon.AtomicNumber, "10043-92-2");
            ids.Add(Elements.Francium.AtomicNumber, "7440-73-5");
            ids.Add(Elements.Radium.AtomicNumber, "7440-14-4");
            ids.Add(Elements.Actinium.AtomicNumber, "7440-34-8");
            ids.Add(Elements.Thorium.AtomicNumber, "7440-29-1");
            ids.Add(Elements.Protactinium.AtomicNumber, "7440-13-3");
            ids.Add(Elements.Uranium.AtomicNumber, "7440-61-1");
            ids.Add(Elements.Neptunium.AtomicNumber, "7439-99-8");
            ids.Add(Elements.Plutonium.AtomicNumber, "7440-07-5");
            ids.Add(Elements.Americium.AtomicNumber, "7440-35-9");
            ids.Add(Elements.Curium.AtomicNumber, "7440-51-9");
            ids.Add(Elements.Berkelium.AtomicNumber, "7440-40-6");
            ids.Add(Elements.Californium.AtomicNumber, "7440-71-3");
            ids.Add(Elements.Einsteinium.AtomicNumber, "7429-92-7");
            ids.Add(Elements.Fermium.AtomicNumber, "7440-72-4");
            ids.Add(Elements.Mendelevium.AtomicNumber, "7440-11-1");
            ids.Add(Elements.Nobelium.AtomicNumber, "10028-14-5");
            ids.Add(Elements.Lawrencium.AtomicNumber, "22537-19-5");
            ids.Add(Elements.Rutherfordium.AtomicNumber, "53850-36-5");
            ids.Add(Elements.Dubnium.AtomicNumber, "53850-35-4");
            ids.Add(Elements.Seaborgium.AtomicNumber, "54038-81-2");
            ids.Add(Elements.Bohrium.AtomicNumber, "54037-14-8");
            ids.Add(Elements.Hassium.AtomicNumber, "54037-57-9");
            ids.Add(Elements.Meitnerium.AtomicNumber, "54038-01-6");
            ids.Add(Elements.Darmstadtium.AtomicNumber, "54083-77-1");
            ids.Add(Elements.Roentgenium.AtomicNumber, "54386-24-2");
            ids.Add(Elements.Copernicium.AtomicNumber, "54084-26-3");
            ids.Add(Elements.Ununtrium.AtomicNumber, "");
            ids.Add(Elements.Flerovium.AtomicNumber, "54085-16-4");
            ids.Add(Elements.Ununpentium.AtomicNumber, "");
            ids.Add(Elements.Livermorium.AtomicNumber, "54100-71-9");
            ids.Add(Elements.Ununseptium.AtomicNumber, "");
            ids.Add(Elements.Ununoctium.AtomicNumber, "");

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