/* Copyright (C) 2004-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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

using NCDK.Config;
using System;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    /// This matches Hydrogen atoms.
    /// </summary>
    // @cdk.module smarts
    // @cdk.githash
    // @cdk.keyword SMARTS
    [Obsolete]
    public class HydrogenAtom : SMARTSAtom
    {
        /// <summary>Creates a new instance.</summary>
        public HydrogenAtom(IChemObjectBuilder builder)
            : base(builder)
        { }

        public override bool Matches(IAtom atom)
        {
            if (!atom.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
            {
                return false;
            }

            if (atom.FormalCharge == 1)
            { // proton matches
                return true;
            }

            // hydrogens connected to other hydrogens, e.g., molecular hydrogen
            var list = Invariants(atom).Target.GetConnectedAtoms(atom);
            foreach (var connAtom in list)
            {
                if (connAtom.AtomicNumber.Equals(ChemicalElement.AtomicNumbers.H))
                {
                    return true;
                }
            }

            // hydrogens connected to other than one other atom, e.g., bridging hydrogens
            if (Invariants(atom).Degree > 1)
            {
                return true;
            }

            //isotopic hydrogen specifications, e.g. deuterium [2H] or tritium etc
            if (atom.MassNumber != null)
            {
                if (MassNumber.Value == atom.MassNumber.Value) return true;
            }
            else
            {
                // target atom is [H], so make sure query atom has mass number = 1
                if (MassNumber == 1) return true;
            }

            return false;
        }
    }
}
