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
 */

using NCDK.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.SMSD.Algorithms.VFLib.Builder
{
    /// <summary>
    /// Class for building/storing nodes (atoms) in the graph with atom
    /// query capabilities.
    /// </summary>
    // @cdk.module smsd
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd .")]
    public class TargetProperties 
    {
        private Dictionary<IAtom, int> connectedTargetAtomCountMap = null;
        private readonly Dictionary<IAtom, IReadOnlyList<IAtom>> connectedTargetAtomListMap = null;
        private readonly IBond[][] map = null;
        private Dictionary<IAtom, int> atoms = null;
        private readonly Dictionary<int, IAtom> atomsIndex = null;

        public int CountNeighbors(IAtom atom)
        {
            if (connectedTargetAtomCountMap == null || !connectedTargetAtomCountMap.ContainsKey(atom))
            {
                Console.Out.WriteLine($"Object not found in {atoms.Count} atoms");
                return 0;
            }
            return connectedTargetAtomCountMap[atom];
        }

        public IReadOnlyList<IAtom> GetNeighbors(IAtom atom)
        {
            return connectedTargetAtomListMap[atom];
        }

        public IBond GetBond(IAtom atom1, IAtom atom2)
        {
            return map[atoms[atom2]][atoms[atom1]];
        }

        public int AtomCount => atoms.Count;

        public TargetProperties(IAtomContainer container)
        {
            int i = 0;
            atoms = new Dictionary<IAtom, int>();
            atomsIndex = new Dictionary<int, IAtom>();
            connectedTargetAtomCountMap = new Dictionary<IAtom, int>();
            connectedTargetAtomListMap = new Dictionary<IAtom, IReadOnlyList<IAtom>>();
            map = Arrays.CreateJagged<IBond>(container.Atoms.Count, container.Atoms.Count);
            foreach (var atom in container.Atoms)
            {
                int count = container.GetConnectedBonds(atom).Count();
                connectedTargetAtomCountMap[atom] = count;
                var list = container.GetConnectedAtoms(atom);
                if (list != null)
                {
                    connectedTargetAtomListMap[atom] = list.ToReadOnlyList();
                }
                else
                {
                    connectedTargetAtomListMap[atom] = new List<IAtom>();
                }
                atoms[atom] = i;
                atomsIndex[i] = atom;
                i++;
            }

            foreach (var bond in container.Bonds)
            {
                map[atoms[bond.Begin]][atoms[bond.End]] = bond;
                map[atoms[bond.End]][atoms[bond.Begin]] = bond;
            }
        }

        public IAtom GetAtom(int j)
        {
            return atomsIndex[j];
        }
    }
}
