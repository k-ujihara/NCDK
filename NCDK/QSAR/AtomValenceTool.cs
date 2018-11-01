/* Copyright (C) 2004-2007  Matteo Floris <mfe4@users.sf.net>
 *                    2008  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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

using NCDK.Config;
using System.Collections.Generic;

namespace NCDK.QSAR
{
    /// <summary>
    /// This class returns the valence of an atom.
    /// </summary>
    // @author      mfe4
    // @cdk.created 2004-11-13
    // @cdk.module  standard
    // @cdk.githash
    // @cdk.dictref valence, atom
    public static class AtomValenceTool
    {
        private static readonly Dictionary<int, int> valencesTable = new Dictionary<int, int>
        {
            { ChemicalElement.AtomicNumbers.H, 1 },
            { ChemicalElement.AtomicNumbers.He, 8 },
            { ChemicalElement.AtomicNumbers.Ne, 8 },
            { ChemicalElement.AtomicNumbers.Ar, 8 },
            { ChemicalElement.AtomicNumbers.Kr, 8 },
            { ChemicalElement.AtomicNumbers.Xe, 8 },
            { ChemicalElement.AtomicNumbers.Hg, 2 },
            { ChemicalElement.AtomicNumbers.Rn, 8 },
            { ChemicalElement.AtomicNumbers.Li, 1 },
            { ChemicalElement.AtomicNumbers.Be, 2 },
            { ChemicalElement.AtomicNumbers.B, 3 },
            { ChemicalElement.AtomicNumbers.C, 4 },
            { ChemicalElement.AtomicNumbers.N, 5 },
            { ChemicalElement.AtomicNumbers.O, 6 },
            { ChemicalElement.AtomicNumbers.F, 7 },
            { ChemicalElement.AtomicNumbers.Na, 1 },
            { ChemicalElement.AtomicNumbers.Mg, 2 },
            { ChemicalElement.AtomicNumbers.Al, 3 },
            { ChemicalElement.AtomicNumbers.Si, 4 },
            { ChemicalElement.AtomicNumbers.P, 5 },
            { ChemicalElement.AtomicNumbers.S, 6 },
            { ChemicalElement.AtomicNumbers.Cl, 7 },
            { ChemicalElement.AtomicNumbers.K, 1 },
            { ChemicalElement.AtomicNumbers.Ca, 2 },
            { ChemicalElement.AtomicNumbers.Ga, 3 },
            { ChemicalElement.AtomicNumbers.Ge, 4 },
            { ChemicalElement.AtomicNumbers.As, 5 },
            { ChemicalElement.AtomicNumbers.Se, 6 },
            { ChemicalElement.AtomicNumbers.Br, 7 },
            { ChemicalElement.AtomicNumbers.Rb, 1 },
            { ChemicalElement.AtomicNumbers.Sr, 2 },
            { ChemicalElement.AtomicNumbers.In, 3 },
            { ChemicalElement.AtomicNumbers.Sn, 4 },
            { ChemicalElement.AtomicNumbers.Sb, 5 },
            { ChemicalElement.AtomicNumbers.Te, 6 },
            { ChemicalElement.AtomicNumbers.I, 7 },
            { ChemicalElement.AtomicNumbers.Cs, 1 },
            { ChemicalElement.AtomicNumbers.Ba, 2 },
            { ChemicalElement.AtomicNumbers.Tl, 3 },
            { ChemicalElement.AtomicNumbers.Pb, 4 },
            { ChemicalElement.AtomicNumbers.Bi, 5 },
            { ChemicalElement.AtomicNumbers.Po, 6 },
            { ChemicalElement.AtomicNumbers.At, 7 },
            { ChemicalElement.AtomicNumbers.Fr, 1 },
            { ChemicalElement.AtomicNumbers.Ra, 2 },
            { ChemicalElement.AtomicNumbers.Cu, 2 },
            { ChemicalElement.AtomicNumbers.Mn, 2 },
            { ChemicalElement.AtomicNumbers.Co, 2 }
        };

        public static int GetValence(IAtom atom)
        {
            if (!valencesTable.TryGetValue(atom.AtomicNumber.Value, out int ret))
                throw new NoSuchAtomException();
            return ret;
        }
    }
}
