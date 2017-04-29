/* Copyright (C) 2001-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *                    2013,2016  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using NCDK.Tools;

namespace NCDK.Config
{
    /// <summary>
    /// List of isotopes. Data is taken from the <see href="https://github.com/egonw/bodr">Blue Obelisk Data Repository</see>,
    /// <see href="https://github.com/egonw/bodr/releases/tag/BODR-10">version 10</see> <token>cdk-cite-BODR10</token>.
    /// The data set is described in the first Blue Obelisk paper <token>cdk-cite-Guha2006</token>.
    /// </summary>
    /// <remarks>
    /// <para>The <pre>isotopes.dat</pre> file that is used by this class is a binary class
    /// of this data, improving loading times over the BODR XML representation. It is created
    /// from the original BODR files using tools from the <pre>cdk-build-util</pre>
    /// repository.</para>
    /// </remarks>
    // @author      egonw
    // @cdk.module  core
    // @cdk.githash
    public class Isotopes
        : IsotopeFactory
    {
        private static Isotopes myself = null;

        /// <summary>
        /// A singleton instance of this class.
        /// </summary>
        /// <exception cref="IOException">when reading of the data file did not work</exception>
        public static Isotopes Instance
        {
            get
            {
                if (myself == null) myself = new Isotopes();
                return myself;
            }
        }

        private Isotopes()
        {
            string configFile = "NCDK.Config.Data.isotopes.dat";
            isotopes = new Dictionary<string, IList<IIsotope>>();
            var ins = ResourceLoader.GetAsStream(configFile);

            byte[] buffer = new byte[8];
            ins.Read(buffer, 0, 4); Array.Reverse(buffer, 0, 4);
            int isotopeCount = BitConverter.ToInt32(buffer, 0);

            for (int i = 0; i < isotopeCount; i++)
            {
                int atomicNum = ins.ReadByte();
                ins.Read(buffer, 0, 2); Array.Reverse(buffer, 0, 2);
                int massNum = BitConverter.ToInt16(buffer, 0);
                ins.Read(buffer, 0, 8); Array.Reverse(buffer, 0, 8);
                double exactMass = BitConverter.ToDouble(buffer, 0);
                double natAbund;
                if (ins.ReadByte() == 1)
                {
                    ins.Read(buffer, 0, 8); Array.Reverse(buffer, 0, 8);
                    natAbund = BitConverter.ToDouble(buffer, 0);
                }
                else
                {
                    natAbund = 0;
                }
                IIsotope isotope = new BODRIsotope(PeriodicTable.GetSymbol(atomicNum), atomicNum, massNum, exactMass,
                        natAbund);
                Add(isotope);
            }
            majorIsotopes = new Dictionary<string, IIsotope>();
        }

        /// <summary>
        /// Gets an array of all isotopes known to the IsotopeFactory for the given element symbol.
        /// </summary>
        /// <param name="symbol">An element symbol to search for</param>
        /// <returns>Isotopes that matches the given element symbol</returns>
        public override IEnumerable<IIsotope> GetIsotopes(string symbol)
        {
            IList<IIsotope> isotopedForValue;
            if (!isotopes.TryGetValue(symbol, out isotopedForValue))
                yield break;
            var list = new List<IIsotope>();
            foreach (var isotope in isotopedForValue)
            {
                if (isotope.Symbol == symbol)
                    yield return isotope;
            }
            yield break;
        }

        /// <summary>
        /// Gets a array of all isotopes known to the IsotopeFactory.
        /// </summary>
        /// <returns>An array of all isotopes</returns>
        public override IEnumerable<IIsotope> GetIsotopes()
        {
            return base.GetIsotopes();
        }
        
        /// <summary>
        /// Gets an array of all isotopes matching the searched exact mass within a certain difference.
        /// </summary>
        /// <param name="exactMass">search mass</param>
        /// <param name="difference">mass the isotope is allowed to differ from the search mass</param>
        /// <returns>All isotopes</returns>
        public override IEnumerable<IIsotope> GetIsotopes(double exactMass, double difference)
        {
            return base.GetIsotopes(exactMass, difference);
        }

        /// <summary>
        /// Get isotope based on element symbol and mass number.
        /// </summary>
        /// <param name="symbol">the element symbol</param>
        /// <param name="massNumber">the mass number</param>
        /// <returns>the corresponding isotope</returns>
        public override IIsotope GetIsotope(string symbol, int massNumber)
        {
            IList<IIsotope> isotopesForValue;
            if (isotopes.TryGetValue(symbol, out isotopesForValue))
                foreach (var isotope in isotopesForValue)
                    if (isotope.Symbol == symbol && isotope.MassNumber == massNumber)
                        return isotope;
            return null;
        }

        /// <summary>
        /// Get an isotope based on the element symbol and exact mass.
        /// </summary>
        /// <param name="symbol">the element symbol</param>
        /// <param name="exactMass">the mass number</param>
        /// <param name="tolerance">allowed difference from provided exact mass</param>
        /// <returns>the corresponding isotope</returns>
        public override IIsotope GetIsotope(string symbol, double exactMass, double tolerance)
        {
            IIsotope ret = null;
            IList<IIsotope> isotopesForValue;
            if (isotopes.TryGetValue(symbol, out isotopesForValue))
            {
                double minDiff = double.MaxValue;
                foreach (var isotope in isotopesForValue)
                {
                    if (!isotope.ExactMass.HasValue)
                        continue;
                    var diff = Math.Abs(isotope.ExactMass.Value - exactMass);
                    if (isotope.Symbol == symbol && diff <= tolerance && diff < minDiff)
                    {
                        ret = isotope;
                        minDiff = diff;
                    }
                }
            }
            return ret;
        }
        
        /// <summary>
        /// Returns the most abundant (major) isotope with a given atomic number.
        /// </summary>
        /// <remarks>
        /// The isotope's abundance is for atoms with atomic number 60 and smaller
        /// defined as a number that is proportional to the 100 of the most abundant
        /// isotope. For atoms with higher atomic numbers, the abundance is defined
        /// as a percentage.
        /// </remarks>
        /// <param name="atomicNumber">The atomic number for which an isotope is to be returned</param>
        /// <returns>The isotope corresponding to the given atomic number</returns>
        public override IIsotope GetMajorIsotope(int atomicNumber)
        {
            IList<IIsotope> isotopesForValue; ;
            if (!isotopes.TryGetValue(PeriodicTable.GetSymbol(atomicNumber), out isotopesForValue))
                return null;
            IIsotope major = null;
            foreach (var isotope in isotopesForValue)
            {
                if (isotope.AtomicNumber == atomicNumber)
                {
                    if (major == null || isotope.NaturalAbundance > major.NaturalAbundance)
                    {
                        major = isotope;
                    }
                }
            }
            if (major == null)
                Trace.TraceError($"Could not find major isotope for: {atomicNumber}");
            return major;
        }

        /// <summary>
        /// Returns the most abundant (major) isotope whose symbol equals element.
        /// </summary>
        /// <param name="symbol">the symbol of the element in question</param>
        /// <returns>The major isotope value</returns>
        public override IIsotope GetMajorIsotope(string symbol)
        {
            IIsotope major;
            if (!majorIsotopes.TryGetValue(symbol, out major))
            {
                IList<IIsotope> isotopesForValue;
                if (!isotopes.TryGetValue(symbol, out isotopesForValue))
                {
                    Trace.TraceError($"Could not find major isotope for: {symbol}");
                    return null;
                }
                foreach (var isotope in isotopesForValue)
                { 
                    if (isotope.Symbol == symbol)
                    {
                        if (major == null || isotope.NaturalAbundance > major.NaturalAbundance)
                        {
                            major = isotope;
                        }
                    }
                }
                if (major == null)
                {
                    Trace.TraceError($"Could not find major isotope for: {symbol}");
                }
                else
                {
                    majorIsotopes.Add(symbol, major);
                }
            }
            return major;
        }
    }
}
