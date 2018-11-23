/*  Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Graphs;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Class that returns the number of atoms in the largest chain.
    /// </summary>
    // @author chhoppe from EUROSCREEN
    // @cdk.created 2006-1-03
    // @cdk.module qsarmolecular
    // @cdk.dictref qsar-descriptors:largestChain
    [DescriptorSpecification("http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#largestChain")]
    public class LargestChainDescriptor : AbstractDescriptor, IMolecularDescriptor
    {
        private readonly IAtomContainer container;

        /// <param name="container"></param>
        /// <param name="checkRingSystem"><see langword="true"/> is the <see cref="IMolecularEntity.IsInRing"/> has to be set</param>
        public LargestChainDescriptor(IAtomContainer container, bool checkAromaticity = false, bool checkRingSystem = false)
        {
            if (checkRingSystem)
                Cycles.MarkRingAtomsAndBonds(container);

            this.container = container;
        }

        [DescriptorResult]
        public class Result : AbstractDescriptorResult
        {
            public Result(int value)
            {
                this.NumberOfAtoms = value;
            }

            /// <summary>
            /// The number of atoms in the largest chain.
            /// </summary>
            [DescriptorResultProperty("nAtomLC")]
            public int NumberOfAtoms { get; private set; }

            public int Value => NumberOfAtoms;
        }

        /// <summary>
        /// Calculate the count of atoms of the largest chain in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <returns>the number of atoms in the largest chain.</returns>
        public Result Calculate()
        {
            // make a subset molecule only including acyclic non-hydrogen atoms
            var included = new HashSet<IAtom>(container.Atoms.Where(atom => !atom.IsInRing && atom.AtomicNumber != 1));
            var subset = SubsetMol(container, included);

            var apsp = new AllPairsShortestPaths(subset);

            int max = 0;
            var numAtoms = subset.Atoms.Count;
            for (int i = 0; i < numAtoms; i++)
            {
                for (int j = i + 1; j < numAtoms; j++)
                {
                    int len = apsp.From(i).GetPathTo(j).Length;
                    if (len > max)
                        max = len;
                }
            }

            return new Result(max);
        }

        private static IAtomContainer SubsetMol(IAtomContainer mol, ISet<IAtom> include)
        {
            var cpy = mol.Builder.NewAtomContainer();
            foreach (var atom in mol.Atoms)
                if (include.Contains(atom))
                    cpy.Atoms.Add(atom);
            foreach (var bond in mol.Bonds)
                if (include.Contains(bond.Atoms[0]) && include.Contains(bond.Atoms[1]))
                    cpy.Bonds.Add(bond);
            return cpy;
        }

        IDescriptorResult IMolecularDescriptor.Calculate() => Calculate();
    }
}
