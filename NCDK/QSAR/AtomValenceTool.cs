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
    public class AtomValenceTool
    {
        private static IDictionary<string, int> valencesTable = null;

        public static int GetValence(IAtom atom)
        {
            if (valencesTable == null)
            {
                valencesTable = new Dictionary<string, int>();
                valencesTable.Add("H", 1);
                valencesTable.Add("He", 8);
                valencesTable.Add("Ne", 8);
                valencesTable.Add("Ar", 8);
                valencesTable.Add("Kr", 8);
                valencesTable.Add("Xe", 8);
                valencesTable.Add("Hg", 2);
                valencesTable.Add("Rn", 8);
                valencesTable.Add("Li", 1);
                valencesTable.Add("Be", 2);
                valencesTable.Add("B", 3);
                valencesTable.Add("C", 4);
                valencesTable.Add("N", 5);
                valencesTable.Add("O", 6);
                valencesTable.Add("F", 7);
                valencesTable.Add("Na", 1);
                valencesTable.Add("Mg", 2);
                valencesTable.Add("Al", 3);
                valencesTable.Add("Si", 4);
                valencesTable.Add("P", 5);
                valencesTable.Add("S", 6);
                valencesTable.Add("Cl", 7);
                valencesTable.Add("K", 1);
                valencesTable.Add("Ca", 2);
                valencesTable.Add("Ga", 3);
                valencesTable.Add("Ge", 4);
                valencesTable.Add("As", 5);
                valencesTable.Add("Se", 6);
                valencesTable.Add("Br", 7);
                valencesTable.Add("Rb", 1);
                valencesTable.Add("Sr", 2);
                valencesTable.Add("In", 3);
                valencesTable.Add("Sn", 4);
                valencesTable.Add("Sb", 5);
                valencesTable.Add("Te", 6);
                valencesTable.Add("I", 7);
                valencesTable.Add("Cs", 1);
                valencesTable.Add("Ba", 2);
                valencesTable.Add("Tl", 3);
                valencesTable.Add("Pb", 4);
                valencesTable.Add("Bi", 5);
                valencesTable.Add("Po", 6);
                valencesTable.Add("At", 7);
                valencesTable.Add("Fr", 1);
                valencesTable.Add("Ra", 2);
                valencesTable.Add("Cu", 2);
                valencesTable.Add("Mn", 2);
                valencesTable.Add("Co", 2);
            }

            int ret;
            if (!valencesTable.TryGetValue(atom.Symbol, out ret))
                throw new System.NullReferenceException();
            return ret;
        }
    }
}
