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
            { NaturalElement.AtomicNumbers.H, 1 },
            { NaturalElement.AtomicNumbers.He, 8 },
            { NaturalElement.AtomicNumbers.Ne, 8 },
            { NaturalElement.AtomicNumbers.Ar, 8 },
            { NaturalElement.AtomicNumbers.Kr, 8 },
            { NaturalElement.AtomicNumbers.Xe, 8 },
            { NaturalElement.AtomicNumbers.Hg, 2 },
            { NaturalElement.AtomicNumbers.Rn, 8 },
            { NaturalElement.AtomicNumbers.Li, 1 },
            { NaturalElement.AtomicNumbers.Be, 2 },
            { NaturalElement.AtomicNumbers.B, 3 },
            { NaturalElement.AtomicNumbers.C, 4 },
            { NaturalElement.AtomicNumbers.N, 5 },
            { NaturalElement.AtomicNumbers.O, 6 },
            { NaturalElement.AtomicNumbers.F, 7 },
            { NaturalElement.AtomicNumbers.Na, 1 },
            { NaturalElement.AtomicNumbers.Mg, 2 },
            { NaturalElement.AtomicNumbers.Al, 3 },
            { NaturalElement.AtomicNumbers.Si, 4 },
            { NaturalElement.AtomicNumbers.P, 5 },
            { NaturalElement.AtomicNumbers.S, 6 },
            { NaturalElement.AtomicNumbers.Cl, 7 },
            { NaturalElement.AtomicNumbers.K, 1 },
            { NaturalElement.AtomicNumbers.Ca, 2 },
            { NaturalElement.AtomicNumbers.Ga, 3 },
            { NaturalElement.AtomicNumbers.Ge, 4 },
            { NaturalElement.AtomicNumbers.As, 5 },
            { NaturalElement.AtomicNumbers.Se, 6 },
            { NaturalElement.AtomicNumbers.Br, 7 },
            { NaturalElement.AtomicNumbers.Rb, 1 },
            { NaturalElement.AtomicNumbers.Sr, 2 },
            { NaturalElement.AtomicNumbers.In, 3 },
            { NaturalElement.AtomicNumbers.Sn, 4 },
            { NaturalElement.AtomicNumbers.Sb, 5 },
            { NaturalElement.AtomicNumbers.Te, 6 },
            { NaturalElement.AtomicNumbers.I, 7 },
            { NaturalElement.AtomicNumbers.Cs, 1 },
            { NaturalElement.AtomicNumbers.Ba, 2 },
            { NaturalElement.AtomicNumbers.Tl, 3 },
            { NaturalElement.AtomicNumbers.Pb, 4 },
            { NaturalElement.AtomicNumbers.Bi, 5 },
            { NaturalElement.AtomicNumbers.Po, 6 },
            { NaturalElement.AtomicNumbers.At, 7 },
            { NaturalElement.AtomicNumbers.Fr, 1 },
            { NaturalElement.AtomicNumbers.Ra, 2 },
            { NaturalElement.AtomicNumbers.Cu, 2 },
            { NaturalElement.AtomicNumbers.Mn, 2 },
            { NaturalElement.AtomicNumbers.Co, 2 }
        };

        public static int GetValence(IAtom atom)
        {
            if (!valencesTable.TryGetValue(atom.AtomicNumber.Value, out int ret))
                throw new NoSuchAtomException();
            return ret;
        }
    }
}
