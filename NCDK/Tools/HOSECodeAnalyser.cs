/* Copyright (C) 1997-2007  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System.Collections.Generic;

namespace NCDK.Tools
{
    /**
     * Analyses a molecular formula given in string format and builds
     * an AtomContainer with the Atoms in the molecular formula.
     *
     * About implicit H handling: By default the methods to calculate formula, natural and canonical mass
     * use the explicit Hs and only the explicit Hs if there is at least one in the molecule, implicit Hs are
     * ignored. If there is no explicit H and only then the implicit Hs are used. If you use the constructor
     * MFAnalyser(IAtomContainer ac, bool useboth) and set useboth to true, all explicit Hs and all implicit Hs are used,
     * the implicit ones also on atoms with explicit Hs.
     *
     * @author         egonw
     * @cdk.created    2007-03-08
     * @cdk.module     extra
     * @cdk.githash
     */
    public class HOSECodeAnalyser
    {
        public static IList<string> GetElements(string code)
        {
            List<string> elementList = new List<string>();

            if (code.Length == 0)
            {
                return elementList;
            }

            string currentSymbol = null;
            for (int f = 0; f < code.Length; f++)
            {
                char currentChar = code[f];
                if (currentChar >= 'A' && currentChar <= 'Z')
                {
                    currentSymbol = "" + currentChar;
                    if (f < code.Length)
                    {
                        currentChar = code[f + 1];
                        if (currentChar >= 'a' && currentChar <= 'z')
                        {
                            currentSymbol += currentChar;
                        }
                    }
                }
                else
                {
                    currentSymbol = null;
                }
                if (currentSymbol != null)
                {
                    if (!elementList.Contains(currentSymbol))
                    {
                        // reverse HOSECodeGenerator.getElementSymbol translations
                        if (currentSymbol.Equals("Y"))
                        {
                            currentSymbol = "Br";
                        }
                        else if (currentSymbol.Equals("X"))
                        {
                            currentSymbol = "Cl";
                        }
                        else if (currentSymbol.Equals("Q"))
                        {
                            currentSymbol = "Si";
                        }
                        elementList.Add(currentSymbol);
                    }
                }
            }
            return elementList;
        }
    }
}
