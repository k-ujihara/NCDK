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
        private static Dictionary<string, int> valencesTable = new Dictionary<string, int>
        {
            { "H", 1 },
            { "He", 8 },
            { "Ne", 8 },
            { "Ar", 8 },
            { "Kr", 8 },
            { "Xe", 8 },
            { "Hg", 2 },
            { "Rn", 8 },
            { "Li", 1 },
            { "Be", 2 },
            { "B", 3 },
            { "C", 4 },
            { "N", 5 },
            { "O", 6 },
            { "F", 7 },
            { "Na", 1 },
            { "Mg", 2 },
            { "Al", 3 },
            { "Si", 4 },
            { "P", 5 },
            { "S", 6 },
            { "Cl", 7 },
            { "K", 1 },
            { "Ca", 2 },
            { "Ga", 3 },
            { "Ge", 4 },
            { "As", 5 },
            { "Se", 6 },
            { "Br", 7 },
            { "Rb", 1 },
            { "Sr", 2 },
            { "In", 3 },
            { "Sn", 4 },
            { "Sb", 5 },
            { "Te", 6 },
            { "I", 7 },
            { "Cs", 1 },
            { "Ba", 2 },
            { "Tl", 3 },
            { "Pb", 4 },
            { "Bi", 5 },
            { "Po", 6 },
            { "At", 7 },
            { "Fr", 1 },
            { "Ra", 2 },
            { "Cu", 2 },
            { "Mn", 2 },
            { "Co", 2 }
        };

        public static int GetValence(IAtom atom)
        {
            if (!valencesTable.TryGetValue(atom.Symbol, out int ret))
                throw new NoSuchAtomException();
            return ret;
        }
    }
}
