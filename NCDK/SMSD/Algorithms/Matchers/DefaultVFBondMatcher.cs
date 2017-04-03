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
using System.Linq;

namespace NCDK.SMSD.Algorithms.Matchers
{
    /// <summary>
    /// Checks if a bond is matching between query and target molecules.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public class DefaultVFBondMatcher : VFBondMatcher
    {
        private IBond queryBond = null;
        private int unsaturation = 0;
        private bool shouldMatchBonds;
        private IQueryBond smartQueryBond = null;

        /// <summary>
        /// Bond type flag
        /// Constructor
        /// </summary>
        public DefaultVFBondMatcher()
        {
            this.queryBond = null;
            this.unsaturation = -1;
            shouldMatchBonds = false;
        }

        /// <summary>
        /// Constructor
        /// <param name="queryMol">query Molecule</param>
        /// <param name="queryBond">query Molecule</param>
        /// <param name="shouldMatchBonds">bond match flag</param>
        /// </summary>
        public DefaultVFBondMatcher(IAtomContainer queryMol, IBond queryBond, bool shouldMatchBonds)
            : base()
        {
            this.queryBond = queryBond;
            this.unsaturation = GetUnsaturation(queryMol, this.queryBond);
            IsBondMatchFlag = shouldMatchBonds;
        }

        /// <summary>
        /// Constructor
        /// <param name="queryBond">query Molecule</param>
        /// </summary>
        public DefaultVFBondMatcher(IQueryBond queryBond)
                : base()

        {
            this.smartQueryBond = queryBond;
        }

        /// <inheritdoc/>
        /// <param name="targetConatiner">target container</param>
        /// <param name="targetBond">target bond</param>
        /// <returns>true if bonds match</returns>
        public bool Matches(TargetProperties targetConatiner, IBond targetBond)
        {
            if (this.smartQueryBond != null)
            {
                return smartQueryBond.Matches(targetBond);
            }
            else
            {
                if (!IsBondMatchFlag)
                {
                    return true;
                }
                if (IsBondMatchFlag && IsBondTypeMatch(targetBond))
                {
                    return true;
                }
                if (IsBondMatchFlag && this.unsaturation == GetUnsaturation(targetConatiner, targetBond))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if a bond is matched between query and target
        /// </summary>
        private bool IsBondTypeMatch(IBond targetBond)
        {
            int reactantBondType = queryBond.Order.Numeric;
            int productBondType = targetBond.Order.Numeric;
            if ((queryBond.IsAromatic == targetBond.IsAromatic)
                    && (reactantBondType == productBondType))
            {
                return true;
            }
            else if (queryBond.IsAromatic && targetBond.IsAromatic)
            {
                return true;
            }
            return false;
        }

        private int GetUnsaturation(TargetProperties container, IBond bond)
        {
            return GetUnsaturation(container, bond.Atoms[0]) + GetUnsaturation(container, bond.Atoms[1]);
        }

        private int GetUnsaturation(TargetProperties container, IAtom atom)
        {
            return GetValency(atom) - container.CountNeighbors(atom);
        }

        private int GetValency(IAtom atom)
        {
            return (atom.Valency == null) ? 0 : atom.Valency.Value;
        }

        private int GetUnsaturation(IAtomContainer container, IBond bond)
        {
            return GetUnsaturation(container, bond.Atoms[0]) + GetUnsaturation(container, bond.Atoms[1]);
        }

        private int GetUnsaturation(IAtomContainer container, IAtom atom)
        {
            return GetValency(atom) - (CountNeighbors(container, atom) + CountImplicitHydrogens(atom));
        }

        private int CountNeighbors(IAtomContainer container, IAtom atom)
        {
            return container.GetConnectedAtoms(atom).Count();
        }

        private int CountImplicitHydrogens(IAtom atom)
        {
            return atom.ImplicitHydrogenCount ?? 0;
        }

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
    }
}

