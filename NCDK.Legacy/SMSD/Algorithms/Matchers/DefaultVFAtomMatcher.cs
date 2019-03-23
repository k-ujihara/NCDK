/* Copyright (C) 2009-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 *
 * MX Cheminformatics Tools for Java
 *
 * Copyright (c) 2007-2009 Metamolecular, LLC
 *
 * http://metamolecular.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Algorithms.VFLib.Builder;
using System;
using System.Linq;

namespace NCDK.SMSD.Algorithms.Matchers
{
    /// <summary>
    /// Checks if atom is matching between query and target molecules.
    /// </summary>
    // @cdk.module smsd
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete]
    public class DefaultVFAtomMatcher : IVFAtomMatcher
    {
        private int maximumNeighbors;
        private string symbol = null;
        private readonly IAtom qAtom = null;
        private IQueryAtom smartQueryAtom = null;
        private bool shouldMatchBonds = false;

        public bool IsBondMatchFlag
        {
            get
            {
                return shouldMatchBonds;
            }
            set
            {
                this.shouldMatchBonds = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultVFAtomMatcher()
        {
            this.qAtom = null;
            symbol = null;
            maximumNeighbors = -1;
        }

        /// <summary>
        /// Constructor
        /// <param name="queryContainer">query atom container</param>
        /// <param name="atom">query atom</param>
        /// <param name="shouldMatchBonds">bond matching flag</param>
        /// </summary>
        public DefaultVFAtomMatcher(IAtomContainer queryContainer, IAtom atom, bool shouldMatchBonds)
                : this()
        {
            this.qAtom = atom;
            this.symbol = atom.Symbol;
            IsBondMatchFlag = shouldMatchBonds;

            //        Console.Out.WriteLine("Atom " + atom.Symbol);
            //        Console.Out.WriteLine("MAX allowed " + maximumNeighbors);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smartQueryAtom">query atom</param>
        /// <param name="container"></param>
        public DefaultVFAtomMatcher(IQueryAtom smartQueryAtom, IQueryAtomContainer container)
                : base()
        {
            this.smartQueryAtom = smartQueryAtom;
            this.symbol = smartQueryAtom.Symbol;
        }

        /// <summary>
        /// Constructor
        /// <param name="queryContainer">query atom container</param>
        /// <param name="template">query atom</param>
        /// <param name="blockedPositions">/// <param name="shouldMatchBonds">bond matching flag</param></param>
        /// </summary>
        public DefaultVFAtomMatcher(IAtomContainer queryContainer, IAtom template, int blockedPositions,
                bool shouldMatchBonds)
                : this(queryContainer, template, shouldMatchBonds)
        {
            this.maximumNeighbors = CountImplicitHydrogens(template) + queryContainer.GetConnectedAtoms(template).Count()
                    - blockedPositions;
        }

        /// <summary>
        ///
        /// <param name="maximum">numbers of connected atoms allowed</param>
        /// </summary>
        public void SetMaximumNeighbors(int maximum)
        {
            this.maximumNeighbors = maximum;
        }

        public void SetSymbol(string symbol)
        {
            this.symbol = symbol;
        }

        private bool MatchSymbol(IAtom atom)
        {
            if (symbol == null)
            {
                return false;
            }
            return symbol.Equals(atom.Symbol, StringComparison.Ordinal);
        }

        private bool MatchMaximumNeighbors(TargetProperties targetContainer, IAtom targetAtom)
        {
            if (maximumNeighbors == -1 || !IsBondMatchFlag)
            {
                return true;
            }

            int maximumTargetNeighbors = targetContainer.CountNeighbors(targetAtom);
            return maximumTargetNeighbors <= maximumNeighbors;
        }

        private static int CountImplicitHydrogens(IAtom atom)
        {
            return atom.ImplicitHydrogenCount ?? 0;
        }

        public bool Matches(TargetProperties targetContainer, IAtom targetAtom)
        {
            if (smartQueryAtom != null && qAtom == null)
            {
                if (!smartQueryAtom.Matches(targetAtom))
                {
                    return false;
                }
            }
            else
            {
                if (!MatchSymbol(targetAtom))
                {
                    return false;
                }
                if (!MatchMaximumNeighbors(targetContainer, targetAtom))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

