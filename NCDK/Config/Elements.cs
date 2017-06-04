/* 
 * Copyright (C) 2006-2012  Egon Willighagen <egonw@users.sf.net>
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
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
using System.Collections.Generic;

namespace NCDK.Config
{
    /// <summary>
    /// Enumeration of chemical elements. Data is taken from the Blue Obelisk Data
    /// Repository, version 3. This enumeration is auto-generated with utilities
    /// found in the 'cdk-build-utils' project.
    /// </summary>
    // @author      egonw
    // @author      john may
    // @cdk.module  core
    // @cdk.githash
    public sealed partial class Elements
    {
        public string Name { get; private set; }

        /// <summary>
        /// The atomic number of the element. An <see cref="Unknown"/> element
        /// has an atomic number of '0'.
        /// </summary>
        public int AtomicNumber { get; private set; }

        /// <summary>
        /// Return the period in the periodic table this element belongs to. If
        /// the element is <see cref="Unknown"/> it's period is 0.
        /// </summary>
        public int Period { get; private set; }

        /// <summary>
        /// Return the group in the periodic table this element belongs to. If
        /// the element does not belong to a group then it's group is '0'.
        /// </summary>
        public int Group { get; private set; }

        /// <summary>
        /// The element symbol, C for carbon, N for nitrogen, Na for sodium, etc. An
        /// <see cref="Unknown"/> element has no symbol.
        /// </summary>
        public string Symbol { get; private set; }

        /// <summary>
        /// Covalent radius (<i>r<sub>cov</sub></i>), van der Waals radius
        /// (<i>r<sub>w</sub></i>) and Pauling electronegativity.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Covalent_radius">Covalent radius</seealso>
        public double? CovalentRadius { get; private set; }
        public double? VdwRadius { get; private set; }

        /// <summary>
        /// Electronegativity, symbol χ, is a chemical property that describes
        /// the tendency of an atom or a functional group to attract electrons
        /// (or electron density) towards itself. This method provides access to the
        /// Pauling electronegativity value for a chemical element. If no value is
        /// available <see langword="null"/> is returned.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Electronegativity#Pauling_electronegativity">Pauling Electronegativity</seealso>
        public double? Electronegativity { get; private set; }

        /// <summary>
        /// An <see cref="IElement"/> instance of this element.
        /// </summary>
        private readonly IElement instance;

        /// <summary>
        /// Lookup elements by symbol / name.
        /// </summary>
        internal static readonly IDictionary<string, Elements> symbolMap = new Dictionary<string, Elements>();

        static Elements()
        {
            for (var i = 0; i < Values.Length; i++)
            {
                var elm = Values[i];
                symbolMap.Add(elm.Symbol.ToLowerInvariant(), elm);
                symbolMap.Add(elm.Name.ToLowerInvariant(), elm);
            }

            // recently named elements
            symbolMap.Add("uub", Copernicium); // 2009
            symbolMap.Add("ununbium", Copernicium);

            symbolMap.Add("uuq", Flerovium); // 2012
            symbolMap.Add("ununquadium", Flerovium);

            symbolMap.Add("uuh", Livermorium); // 2012
            symbolMap.Add("ununhexium", Livermorium);

            // 2016
            symbolMap["uut"] = Nihonium;
            symbolMap["uup"] = Moscovium;
            symbolMap["uus"] = Tennessine;
            symbolMap["uuo"] = Oganesson;

            // alternative spellings
            symbolMap.Add("sulphur", Sulfur);
            symbolMap.Add("cesium", Caesium);
            symbolMap.Add("aluminum", Aluminium);
        }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="number">atomic number</param>
        /// <param name="symbol">symbol</param>
        /// <param name="period">periodic table period</param>
        /// <param name="group">periodic table group</param>
        /// <param name="rCov">covalent radius</param>
        /// <param name="rW">van der Waals radius</param>
        /// <param name="electronegativity">Pauling electronegativity</param>
        private Elements(string name, int number, string symbol, int period, int group, double? rCov, double? rW, double? electronegativity)
        {
            this.Name = name;
            this.AtomicNumber = number;
            this.Period = period;
            this.Group = group;
            this.Symbol = symbol;
            this.CovalentRadius = rCov;
            this.VdwRadius = rW;
            this.Electronegativity = electronegativity;
            this.instance = new NaturalElement(symbol, number);
        }

        /// <summary>
        /// Access an  <see cref="IElement"/> instance of the chemical element.
        /// </summary>
        /// <returns>an instance</returns>
        public IElement ToIElement()
        {
            return instance;
        }

        /// <summary>
        /// Obtain the element with the specified atomic number. If no element had
        /// the specified atomic number then <see cref="Unknown"/> is returned.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Config.Elements.cs+OfNumber"]/*' />
        /// </example>
        /// <param name="number">number atomic number</param>
        /// <returns>an element, or <see cref="Unknown"/></returns>
        public static Elements OfNumber(int number)
        {
            if (number < 0 || number >= Values.Length) return Unknown;
            return Values[number];
        }

        /// <summary>
        /// Obtain the element with the specified symbol or name. If no element had
        /// the specified symbol or name then <see cref="Unknown"/> is returned. The
        /// input is case-insensitive.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Config.Elements.cs+OfString"]/*' />
        /// </example>
        /// <param name="str">input string</param>
        /// <returns>an element, or <see cref="Unknown"/></returns>
        public static Elements OfString(string str)
        {
            if (str == null) return Unknown;
            Elements e;
            if (!symbolMap.TryGetValue(str.ToLowerInvariant(), out e))
                e = Unknown;
            return e;
        }
    }
}
