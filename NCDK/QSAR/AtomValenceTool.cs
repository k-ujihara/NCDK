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
    // @cdk.dictref valence, atom
    public static class AtomValenceTool
    {
        private static readonly Dictionary<int, int> valencesTable = new Dictionary<int, int>
        {
            { NaturalElements.H.AtomicNumber, 1 },
            { NaturalElements.He.AtomicNumber, 8 },
            { NaturalElements.Ne.AtomicNumber, 8 },
            { NaturalElements.Ar.AtomicNumber, 8 },
            { NaturalElements.Kr.AtomicNumber, 8 },
            { NaturalElements.Xe.AtomicNumber, 8 },
            { NaturalElements.Hg.AtomicNumber, 2 },
            { NaturalElements.Rn.AtomicNumber, 8 },
            { NaturalElements.Li.AtomicNumber, 1 },
            { NaturalElements.Be.AtomicNumber, 2 },
            { NaturalElements.B.AtomicNumber, 3 },
            { NaturalElements.C.AtomicNumber, 4 },
            { NaturalElements.N.AtomicNumber, 5 },
            { NaturalElements.O.AtomicNumber, 6 },
            { NaturalElements.F.AtomicNumber, 7 },
            { NaturalElements.Na.AtomicNumber, 1 },
            { NaturalElements.Mg.AtomicNumber, 2 },
            { NaturalElements.Al.AtomicNumber, 3 },
            { NaturalElements.Si.AtomicNumber, 4 },
            { NaturalElements.P.AtomicNumber, 5 },
            { NaturalElements.S.AtomicNumber, 6 },
            { NaturalElements.Cl.AtomicNumber, 7 },
            { NaturalElements.K.AtomicNumber, 1 },
            { NaturalElements.Ca.AtomicNumber, 2 },
            { NaturalElements.Ga.AtomicNumber, 3 },
            { NaturalElements.Ge.AtomicNumber, 4 },
            { NaturalElements.As.AtomicNumber, 5 },
            { NaturalElements.Se.AtomicNumber, 6 },
            { NaturalElements.Br.AtomicNumber, 7 },
            { NaturalElements.Rb.AtomicNumber, 1 },
            { NaturalElements.Sr.AtomicNumber, 2 },
            { NaturalElements.In.AtomicNumber, 3 },
            { NaturalElements.Sn.AtomicNumber, 4 },
            { NaturalElements.Sb.AtomicNumber, 5 },
            { NaturalElements.Te.AtomicNumber, 6 },
            { NaturalElements.I.AtomicNumber, 7 },
            { NaturalElements.Cs.AtomicNumber, 1 },
            { NaturalElements.Ba.AtomicNumber, 2 },
            { NaturalElements.Tl.AtomicNumber, 3 },
            { NaturalElements.Pb.AtomicNumber, 4 },
            { NaturalElements.Bi.AtomicNumber, 5 },
            { NaturalElements.Po.AtomicNumber, 6 },
            { NaturalElements.At.AtomicNumber, 7 },
            { NaturalElements.Fr.AtomicNumber, 1 },
            { NaturalElements.Ra.AtomicNumber, 2 },
            { NaturalElements.Cu.AtomicNumber, 2 },
            { NaturalElements.Mn.AtomicNumber, 2 },
            { NaturalElements.Co.AtomicNumber, 2 }
        };

        public static int GetValence(IAtom atom)
        {
            if (!valencesTable.TryGetValue(atom.AtomicNumber.Value, out int ret))
                throw new NoSuchAtomException();
            return ret;
        }
    }
}
