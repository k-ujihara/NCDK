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
    public sealed partial class ChemicalElement
    {
        public string Name { get; private set; }

        /// <summary>
        /// The atomic number of the element. An <see cref="ChemicalElements.Unknown"/> element
        /// has an atomic number of '0'.
        /// </summary>
        public int AtomicNumber { get; private set; }

        /// <summary>
        /// Return the period in the periodic table this element belongs to. If
        /// the element is <see cref="ChemicalElements.Unknown"/> it's period is 0.
        /// </summary>
        public int Period { get; private set; }

        /// <summary>
        /// Return the group in the periodic table this element belongs to. If
        /// the element does not belong to a group then it's group is '0'.
        /// </summary>
        public int Group { get; private set; }

        /// <summary>
        /// The element symbol, C for carbon, N for nitrogen, Na for sodium, etc. An
        /// <see cref="ChemicalElements.Unknown"/> element has no symbol.
        /// </summary>
        public string Symbol { get; private set; }

        /// <summary>
        /// Covalent radius (<i>r<sub>cov</sub></i>).
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Covalent_radius">Covalent radius</seealso>
        public double? CovalentRadius { get; private set; }

        /// <summary>
        /// van der Waals radius (<i>r<sub>w</sub></i>).
        /// </summary>
        public double? VdwRadius { get; private set; }

        /// <summary>
        /// Pauling electronegativity, symbol χ, is a chemical property that describes
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
        internal ChemicalElement(string name, int number, string symbol, int period, int group, double? rCov, double? rW, double? electronegativity)
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
        /// the specified atomic number then <see cref="ChemicalElements.Unknown"/> is returned.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Config.ChemicalElement_Example.cs+OfNumber"]/*' />
        /// </example>
        /// <param name="number">atomic number</param>
        /// <returns>an element, or <see cref="ChemicalElements.Unknown"/></returns>
        public static ChemicalElement OfNumber(int number)
        {
            if (number < 0 || number >= Values.Count)
                return ChemicalElements.Unknown;
            return Values[number];
        }

        /// <summary>
        /// Obtain the element with the specified symbol or name. If no element had
        /// the specified symbol or name then <see cref="ChemicalElements.Unknown"/> is returned. The
        /// input is case-insensitive.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Config.ChemicalElement_Example.cs+OfString"]/*' />
        /// </example>
        /// <param name="str">input string</param>
        /// <returns>an element, or <see cref="ChemicalElements.Unknown"/></returns>
        public static ChemicalElement OfString(string str)
        {
            if (str == null)
                return ChemicalElements.Unknown;
            if (!SymbolMap.TryGetValue(str.ToLowerInvariant(), out ChemicalElement e))
                e = ChemicalElements.Unknown;
            return e;
        }
    }
}
