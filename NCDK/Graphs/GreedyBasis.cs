/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using NCDK.Common.Collections;
using static NCDK.Graphs.InitialCycles;

namespace NCDK.Graphs
{
    /**
	 * Greedily compute a cycle basis from a provided set of initial cycles using
	 * Gaussian elimination.
	 *
	 * @author John May
	 * @cdk.module core
	 * @cdk.githash
	 * @see RelevantCycles
	 */
#if TEST

    public
#endif

        class GreedyBasis {
        /// <summary>Cycles which are members of the basis</summary>
        private readonly List<Cycle> basis;

        /// <summary>Edges of the current basis.</summary>
        private readonly BitArray      edgesOfBasis;

		/// <summary>Number of edges</summary>
		private readonly int m;

        /**
		 * Create a new basis for the <i>potential</i> number of cycles and the
		 * <i>exact</i> number of edges. These values can be obtained from an {@link
		 * InitialCycles} instance.
		 *
		 * @param n potential number of cycles in the basis
		 * @param m number of edges in the graph
		 * @see org.openscience.cdk.graph.InitialCycles#GetNumberOfCycles()
		 * @see org.openscience.cdk.graph.InitialCycles#GetNumberOfEdges()
		 */
        public GreedyBasis(int n, int m) {
            this.basis = new List<Cycle>(n);
            this.edgesOfBasis = new BitArray(m);
            this.m = m;
        }

        /**
		 * Access the members of the basis.
		 *
		 * @return cycles ordered by length
		 */
        public IList<Cycle> Members => new ReadOnlyCollection<Cycle>(basis);

        /**
		 * The size of the basis.
		 *
		 * @return number of cycles in the basis
		 */
        public int Count => Members.Count;

        /**
		 * Add a cycle to the basis.
		 *
		 * @param cycle new basis member
		 */
        public void Add(Cycle cycle)
        {
            basis.Add(cycle);
            edgesOfBasis.Or(cycle.EdgeVector);
        }

        /**
		 * Add all cycles to the basis.
		 *
		 * @param cycles new members of the basis
		 */
        public void AddAll(IEnumerable<Cycle> cycles) {
            foreach (var cycle in cycles)
                Add(cycle);
        }

        /**
		 * Check if all the edges of the <i>cycle</i> are present in the current
		 * <i>basis</i>.
		 *
		 * @param cycle an initial cycle
		 * @return any edges of the basis are present
		 */
        public bool IsSubsetOfBasis(Cycle cycle) {
            BitArray edgeVector = cycle.EdgeVector;
            int intersect = BitArrays.Cardinality(And(edgesOfBasis, edgeVector));
            return intersect == cycle.Length;
        }

        /**
		 * Determine whether the <i>candidate</i> cycle is linearly
		 * <i>independent</i> from the current basis.
		 *
		 * @param candidate a cycle not in currently in the basis
		 * @return the candidate is independent
		 */
        public bool IsIndependent(Cycle candidate) {

            // simple checks for independence
            if (basis.Count == 0 || !IsSubsetOfBasis(candidate)) return true;

             BitMatrix matrix = BitMatrix.From(basis, candidate);

            // perform gaussian elimination
            matrix.Eliminate();

            // if the last row (candidate) was eliminated it is not independent
            return !matrix.Eliminated(basis.Count);
        }

        /// <summary>and <i>s</i> and <i>t</i> without modifying <i>s</i></summary>
        private static BitArray And(BitArray s, BitArray t) {
            BitArray u = (BitArray)s.Clone();
            u.And(t);
            return u;
        }
    }
}

