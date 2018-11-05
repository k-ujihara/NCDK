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
    public static partial class NaturalElement
    {
        /// <summary>
        /// Obtain the element with the specified atomic number. If no element had
        /// the specified atomic number then <see cref="NaturalElements.Unknown"/> is returned.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Config.ChemicalElement_Example.cs+OfNumber"]/*' />
        /// </example>
        /// <param name="number">atomic number</param>
        /// <returns>an element, or <see cref="NaturalElements.Unknown"/></returns>
        public static IElement OfNumber(int number)
        {
            if (number < 0 || number >= Elements.Count)
                return NaturalElements.Unknown.Element;
            return Elements[number];
        }

        /// <summary>
        /// Obtain the element with the specified symbol or name. If no element had
        /// the specified symbol or name then <see cref="NaturalElements.Unknown.AtomicNumber"/> is returned. The
        /// input is case-insensitive.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Config.ChemicalElement_Example.cs+ToAtomicNumber"]/*' />
        /// </example>
        /// <param name="text">input string</param>
        /// <returns>An atomic number, or <see cref="NaturalElements.Unknown.AtomicNumber"/></returns>
        public static int ToAtomicNumber(string text)
        {
            if (text == null)
                return NaturalElements.Unknown.AtomicNumber;
            if (!SymbolMap.TryGetValue(text.ToLowerInvariant(), out int e))
                e = NaturalElements.Unknown.AtomicNumber;
            return e;
        }
    }
}
