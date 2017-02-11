/*
 * Copyright (C) 2012   Syed Asad Rahman <asad@ebi.ac.uk>
 *               2013   John May         <jwmay@users.sf.net>
 *
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Collections;
using NCDK.Graphs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NCDK.Fingerprint
{
    /**
     *
    // @author Syed Asad Rahman (2012)
    // @author John May (2013)
    // @cdk.keyword fingerprint
    // @cdk.keyword similarity
    // @cdk.module fingerprint
    // @cdk.githash
     *
     */
#if TEST
    public
#endif
    sealed class ShortestPathWalker
    {
        /* container which is being traversed */
        private readonly IAtomContainer container;

        /* set of encoded atom paths */
        private readonly IList<string> paths;

        /* list of encoded pseudo atoms */
        private readonly List<string> pseudoAtoms;

        /* maximum number of shortest paths, when there is more then one path */
        private const int MAX_SHORTEST_PATHS = 5;

        /**
        // Create a new shortest path walker for a given container.
        // @param container the molecule to encode the shortest paths
         */
        public ShortestPathWalker(IAtomContainer container)
        {
            this.container = container;
            this.pseudoAtoms = new List<string>(5);
            this.paths = new ReadOnlyCollection<string>(Traverse());
        }

        /**
        // Access a set of all shortest paths.
        // @return the paths
         */
        public IList<string> GetPaths()
        {
            return new ReadOnlyCollection<string>(paths);
        }

        /**
        // Traverse all-pairs of shortest-paths within a chemical graph.
         */
        private IList<string> Traverse()
        {

            var paths = new SortedSet<string>();

            // All-Pairs Shortest-Paths (APSP)
            AllPairsShortestPaths apsp = new AllPairsShortestPaths(container);

            for (int i = 0, n = container.Atoms.Count; i < n; i++)
            {

                paths.Add(ToAtomPattern(container.Atoms[i]));

                // only do the comparison for i,j then reverse the path for j,i
                for (int j = i + 1; j < n; j++)
                {

                    int nPaths = apsp.From(i).GetNPathsTo(j);

                    // only encode when there is a manageable number of paths
                    if (nPaths > 0 && nPaths < MAX_SHORTEST_PATHS)
                    {

                        foreach (var path in apsp.From(i).GetPathsTo(j))
                        {
                            paths.Add(Encode(path));
                            paths.Add(Encode(Reverse(path)));
                        }

                    }

                }
            }

            return paths.ToList();

        }

        /**
        // Reverse an array of integers.
         *
        // @param src array to reverse
        // @return reversed copy of <i>src</i>
         */
        private int[] Reverse(int[] src)
        {
            int[] dest = Arrays.CopyOf(src, src.Length);
            int left = 0;
            int right = src.Length - 1;

            while (left < right)
            {
                // swap the values at the left and right indices
                dest[left] = src[right];
                dest[right] = src[left];

                // move the left and right index pointers in toward the center
                left++;
                right--;
            }
            return dest;
        }

        /**
        // Encode the provided path of atoms to a string.
         *
        // @param path inclusive array of vertex indices
        // @return encoded path
         */
        private string Encode(int[] path)
        {
            StringBuilder sb = new StringBuilder(path.Length * 3);

            for (int i = 0, n = path.Length - 1; i <= n; i++)
            {

                IAtom atom = container.Atoms[path[i]];

                sb.Append(ToAtomPattern(atom));

                if (atom is IPseudoAtom)
                {
                    pseudoAtoms.Add(atom.Symbol);
                    // potential bug, although the atoms are canonical we cannot guarantee the order we will visit them.
                    // sb.Append(PeriodicTable.GetElementCount() + pseudoAtoms.Count());
                }

                // if we are not at the last index, add the connecting bond
                if (i < n)
                {
                    IBond bond = container.GetBond(container.Atoms[path[i]], container.Atoms[path[i + 1]]);
                    sb.Append(GetBondSymbol(bond));
                }

            }

            return sb.ToString();
        }

        /**
        // Convert an atom to a string representation. Currently this method just
        // returns the symbol but in future may include other properties, such as, stereo
        // descriptor and charge.
        // @param atom The atom to encode
        // @return encoded atom
         */
        private string ToAtomPattern(IAtom atom)
        {
            return atom.Symbol;
        }

        /**
        // Gets the bondSymbol attribute of the HashedFingerprinter class
         *
        // @param bond Description of the Parameter
        // @return The bondSymbol value
         *]\     */
        private char GetBondSymbol(IBond bond)
        {
            if (IsSP2Bond(bond))
            {
                return '@';
            }
            else
            {
                switch (bond.Order.Numeric)
                {
                    case 1:
                        return '1';
                    case 2:
                        return '2';
                    case 3:
                        return '3';
                    case 4:
                        return '4';
                    default:
                        return '5';
                }
            }
        }

        /**
        // Returns true if the bond binds two atoms, and both atoms are SP2 in a ring system.
         */
        private bool IsSP2Bond(IBond bond)
        {
            return bond.IsAromatic;
        }

        /**
        // @inheritDoc
         */

        public override string ToString()
        {
            int n = this.paths.Count();
            string[] paths = this.paths.ToArray();
            StringBuilder sb = new StringBuilder(n * 5);

            for (int i = 0, last = n - 1; i < n; i++)
            {
                sb.Append(paths[i]);
                if (i != last) sb.Append("->");
            }

            return sb.ToString();
        }
    }
}
