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
    /**
     * Class for building/storing nodes (atoms) in the graph with atom
     * query capabilities.
     * @cdk.module smsd
     * @cdk.githash
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     */
    [Serializable]
    public class TargetProperties 
    {
        private IDictionary<IAtom, int> connectedTargetAtomCountMap = null;
        private IDictionary<IAtom, IList<IAtom>> connectedTargetAtomListMap = null;
        private IBond[][] map = null;
        private IDictionary<IAtom, int> atoms = null;
        private IDictionary<int, IAtom> atomsIndex = null;

        /**
         * @param atom
         * @return the connectedTargetAtomCountMap
         */
        public int CountNeighbors(IAtom atom)
        {
            if (connectedTargetAtomCountMap == null || !connectedTargetAtomCountMap.ContainsKey(atom))
            {
                Console.Out.WriteLine("Object not found in " + atoms.Count + " atoms");
                return 0;
            }
            return connectedTargetAtomCountMap[atom];
        }

        /**
         * @param atom
         * @return the connected Target Atom List
         */
        public IList<IAtom> GetNeighbors(IAtom atom)
        {
            return connectedTargetAtomListMap[atom];
        }

        /**
         * @param atom1
         * @param atom2
         * @return the map
         */
        public IBond GetBond(IAtom atom1, IAtom atom2)
        {
            return map[atoms[atom2]][atoms[atom1]];
        }

        /**
         * @return atom count
         */
        public int AtomCount => atoms.Count;

        /**
         *
         * @param container
         */
        public TargetProperties(IAtomContainer container)
        {
            int i = 0;
            atoms = new Dictionary<IAtom, int>();
            atomsIndex = new Dictionary<int, IAtom>();
            connectedTargetAtomCountMap = new Dictionary<IAtom, int>();
            connectedTargetAtomListMap = new Dictionary<IAtom, IList<IAtom>>();
            map = Arrays.CreateJagged<IBond>(container.Atoms.Count, container.Atoms.Count);
            foreach (var atom in container.Atoms)
            {
                int count = container.GetConnectedAtoms(atom).Count();
                connectedTargetAtomCountMap[atom] = count;
                var list = container.GetConnectedAtoms(atom);
                if (list != null)
                {
                    connectedTargetAtomListMap[atom] = list.ToList();
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
                map[atoms[bond.Atoms[0]]][atoms[bond.Atoms[1]]] = bond;
                map[atoms[bond.Atoms[1]]][atoms[bond.Atoms[0]]] = bond;
            }
        }

        public IAtom GetAtom(int j)
        {
            return atomsIndex[j];
        }
    }
}
