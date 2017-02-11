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

using NCDK.Common.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NCDK.Graphs.Invariant
{
    /**
	 * An implementation based on the canon algorithm {@cdk.cite WEI89}. The
	 * algorithm uses an initial set of of invariants which are assigned a rank.
	 * Equivalent ranks are then shattered using an unambiguous function (in this
	 * case, the product of primes of adjacent ranks). Once no more equivalent ranks
	 * can be shattered ties are artificially broken and rank shattering continues.
	 * Unlike the original description rank stability is not maintained reducing
	 * the number of values to rank at each stage to only those which are equivalent.
	 * <p/>
	 *
	 * The initial set of invariants is basic and are - <i>
	 * "sufficient for the purpose of obtaining unique notation for simple SMILES,
	 *  but it is not necessarily a “complete” set. No “perfect” set of invariants
	 *  is known that will distinguish all possible graph asymmetries. However,
	 *  for any given set of structures, a set of invariants can be devised to
	 *  provide the necessary discrimination"</i> {@cdk.cite WEI89}. As such this
	 *  producer should not be considered a complete canonical labelled but in
	 *  practice performs well. For a more accurate and computationally expensive
	 *  labelling, please using the {@link InChINumbersTools}.
	 *
	 * <blockquote><pre>
	 * IAtomContainer m = ...;
	 * int[][]        g = GraphUtil.ToAdjList(m);
	 *
	 * // obtain canon labelling
	 * long[] labels = Canon.Label(m, g);
	 *
	 * // obtain symmetry classes
	 * long[] labels = Canon.Symmetry(m, g);
	 * </pre></blockquote>
	 *
	 * @author John May
	 * @cdk.module standard
	 * @cdk.githash
	 */
    public sealed class Canon
    {

        private const int N_PRIMES = 10000;
        /**
		 * Graph, adjacency list representation.
		 */
        private readonly int[][] g;

        /**
		 * Storage of canon labelling and symmetry classes.
		 */
        private readonly long[] labelling, symmetry;

        /// <summary>Only compute the symmetry classes.</summary>
        private bool symOnly = false;

        /**
		 * Create a canon labelling for the graph (g) with the specified
		 * invariants.
		 *
		 * @param g         a graph (adjacency list representation)
		 * @param hydrogens binary vector of terminal hydrogens
		 * @param partition an initial partition of the vertices
		 */
        private Canon(int[][] g, long[] partition, bool[] hydrogens, bool symOnly)
        {
            this.g = g;
            this.symOnly = symOnly;
            labelling = (long[])partition.Clone();
            symmetry = Refine(labelling, hydrogens);
        }

        /**
		 * Compute the canonical labels for the provided structure. The labelling
		 * does not consider isomer information or stereochemistry. The current
		 * implementation does not fully distinguish all structure topologies
		 * but in practise performs well in the majority of cases. A complete
		 * canonical labelling can be obtained using the {@link InChINumbersTools}
		 * but is computationally much more expensive.
		 *
		 * @param container structure
		 * @param g         adjacency list graph representation
		 * @return the canonical labelling
		 * @see EquivalentClassPartitioner
		 * @see InChINumbersTools
		 */
        public static long[] Label(IAtomContainer container, int[][] g)
        {
            return Label(container, g, basicInvariants(container, g));
        }

        /**
		 * Compute the canonical labels for the provided structure. The labelling
		 * does not consider isomer information or stereochemistry. This method
		 * allows provision of a custom array of initial invariants.
		 *
		 * <p/>
		 * The current
		 * implementation does not fully distinguish all structure topologies
		 * but in practise performs well in the majority of cases. A complete
		 * canonical labelling can be obtained using the {@link InChINumbersTools}
		 * but is computationally much more expensive.
		 *
		 * @param container  structure
		 * @param g          adjacency list graph representation
		 * @param invariants initial invariants
		 * @return the canonical labelling
		 * @see EquivalentClassPartitioner
		 * @see InChINumbersTools
		 */
        public static long[] Label(IAtomContainer container, int[][] g, long[] invariants)
        {
            if (invariants.Length != g.Length)
                throw new ArgumentException("number of invariants != number of atoms");
            return new Canon(g, invariants, terminalHydrogens(container, g), false).labelling;
        }

        /**
		 * Compute the symmetry classes for the provided structure. There are known
		 * examples where symmetry is incorrectly found. The {@link
		 * EquivalentClassPartitioner} gives more accurate symmetry perception but
		 * this method is very quick and in practise successfully portions the
		 * majority of chemical structures.
		 *
		 * @param container structure
		 * @param g         adjacency list graph representation
		 * @return symmetry classes
		 * @see EquivalentClassPartitioner
		 */
        public static long[] Symmetry(IAtomContainer container, int[][] g)
        {
            return new Canon(g, basicInvariants(container, g), terminalHydrogens(container, g), true).symmetry;
        }

        /**
		 * Internal - refine invariants to a canonical labelling and
		 * symmetry classes.
		 *
		 * @param invariants the invariants to refine (canonical labelling gets
		 *                   written here)
		 * @param hydrogens  binary vector of terminal hydrogens
		 * @return the symmetry classes
		 */
        private long[] Refine(long[] invariants, bool[] hydrogens)
        {

            int ord = g.Length;

            InvariantRanker ranker = new InvariantRanker(ord);

            // current/next vertices, these only hold the vertices which are
            // equivalent
            int[] currVs = new int[ord];
            int[] nextVs = new int[ord];

            // fill with identity (also set number of non-unique)
            int nnu = ord;
            for (int i = 0; i < ord; i++)
                currVs[i] = i;

            long[] prev = invariants;
            long[] curr = Arrays.CopyOf(invariants, ord);

            // initially all labels are 1, the input invariants are then used to
            // refine this coarse partition
            Arrays.Fill(prev, 1L);

            // number of ranks
            int n = 0, m = 0;

            // storage of symmetry classes
            long[] symmetry = null;

            while (n < ord)
            {

                // refine the initial invariants using product of primes from
                // adjacent ranks
                while ((n = ranker.rank(currVs, nextVs, nnu, curr, prev)) > m && n < ord)
                {
                    nnu = 0;
                    for (int i = 0; i < ord && nextVs[i] >= 0; i++)
                    {
                        int v = nextVs[i];
                        currVs[nnu++] = v;
                        curr[v] = hydrogens[v] ? prev[v] : PrimeProduct(g[v], prev, hydrogens);
                    }
                    m = n;
                }

                if (symmetry == null)
                {

                    // After symmetry classes have been found without hydrogens we add
                    // back in the hydrogens and assign ranks. We don't refine the
                    // partition until the next time round the while loop to avoid
                    // artificially splitting due to hydrogen representation, for example
                    // the two hydrogens are equivalent in this SMILES for ethane '[H]CC'
                    for (int i = 0; i < g.Length; i++)
                    {
                        if (hydrogens[i])
                        {
                            curr[i] = prev[g[i][0]];
                            hydrogens[i] = false;
                        }
                    }
                    n = ranker.rank(currVs, nextVs, nnu, curr, prev);
                    symmetry = Arrays.CopyOf(prev, ord);

                    // Update the buffer of non-unique vertices as hydrogens next
                    // to discrete heavy atoms are also discrete (and removed from
                    // 'nextVs' during ranking.
                    nnu = 0;
                    for (int i = 0; i < ord && nextVs[i] >= 0; i++)
                    {
                        currVs[nnu++] = nextVs[i];
                    }
                }

                // partition is discrete or only symmetry classes are needed
                if (symOnly || n == ord) return symmetry;

                // artificially split the lowest cell, we perturb the value
                // of all vertices with equivalent rank to the lowest non-unique
                // vertex
                int lo = nextVs[0];
                for (int i = 1; i < ord && nextVs[i] >= 0 && prev[nextVs[i]] == prev[lo]; i++)
                    prev[nextVs[i]]++;

                // could also swap but this is cleaner
                Array.Copy(nextVs, 0, currVs, 0, nnu);
            }

            return symmetry;
        }

        /**
		 * Compute the prime product of the values (ranks) for the given
		 * adjacent neighbors (ws).
		 *
		 * @param ws    indices (adjacent neighbors)
		 * @param ranks invariant ranks
		 * @return the prime product
		 */
        private long PrimeProduct(int[] ws, long[] ranks, bool[] hydrogens)
        {
            long prod = 1;
            foreach (var w in ws)
            {
                if (!hydrogens[w])
                {
                    prod *= PRIMES[(int)ranks[w]];
                }
            }
            return prod;
        }

        /**
		 * Generate the initial invariants for each atom in the {@code container}.
		 * The labels use the invariants described in {@cdk.cite WEI89}. <p/>
		 *
		 * The bits in the low 32-bits are: {@code 0000000000xxxxXXXXeeeeeeescchhhh}
		 * where:
		 * <ul>
		 *     <li>0: padding</li>
		 *     <li>x: number of connections</li>
		 *     <li>X: number of non-hydrogens bonds</li>
		 *     <li>e: atomic number</li>
		 *     <li>s: sign of charge</li>
		 *     <li>c: absolute charge</li>
		 *     <li>h: number of attached hydrogens</li>
		 * </ul>
		 *
		 * <b>Important: These invariants are <i>basic</i> and there are known
		 * examples they don't distinguish. One trivial example to consider is
		 * {@code [O]C=O} where both oxygens have no hydrogens and a single
		 * connection but the atoms are not equivalent. Including a better
		 * initial partition is more expensive</b>
		 *
		 * @param container an atom container to generate labels for
		 * @param graph     graph representation (adjacency list)
		 * @return initial invariants
		 * @ an atom had unset atomic number, hydrogen
		 *                              count or formal charge
		 */
        public static long[] basicInvariants(IAtomContainer container, int[][] graph)
        {

            long[] labels = new long[graph.Length];

            for (int v = 0; v < graph.Length; v++)
            {
                IAtom atom = container.Atoms[v];

                int deg = graph[v].Length;
                int impH = GetImplH(atom);
                int expH = 0;
                int elem = GetAtomicNumber(atom);
                int chg = Charge(atom);

                // count non-suppressed (explicit) hydrogens
                foreach (var w in graph[v])
                    if (GetAtomicNumber(container.Atoms[w]) == 1) expH++;

                long label = 0; // connectivity (first in)
                label |= (long)(deg + impH & 0xf);
                label <<= 4; // connectivity (heavy) <= 15 (4 bits)
                label |= (long)(deg - expH & 0xf);
                label <<= 7; // atomic number <= 127 (7 bits)
                label |= (long)(elem & 0x7f);
                label <<= 1; // charge sign == 1 (1 bit)
                label |= (long)(chg >> 31 & 0x1);
                label <<= 2; // charge <= 3 (2 bits)
                label |= (long)(Math.Abs(chg) & 0x3);
                label <<= 4; // hydrogen count <= 15 (4 bits)
                label |= (long)(impH + expH & 0xf);

                labels[v] = label;
            }
            return labels;
        }

        /**
		 * Access atomic number of atom defaulting to 0 for pseudo atoms.
		 *
		 * @param atom an atom
		 * @return the atomic number
		 * @ the atom was non-pseudo at did not have an
		 *                              atomic number
		 */
        private static int GetAtomicNumber(IAtom atom)
        {
            int? elem = atom.AtomicNumber;
            if (elem.HasValue) return elem.Value;
            if (atom is IPseudoAtom) return 0;
            throw new NullReferenceException("a non-pseudoatom had unset atomic number");
        }

        /**
		 * Access implicit hydrogen count of the atom defaulting to 0 for pseudo
		 * atoms.
		 *
		 * @param atom an atom
		 * @return the implicit hydrogen count
		 * @ the atom was non-pseudo at did not have an
		 *                              implicit hydrogen count
		 */
        private static int GetImplH(IAtom atom)
        {
            int? h = atom.ImplicitHydrogenCount;
            if (h.HasValue) return h.Value;
            if (atom is IPseudoAtom) return 0;
            throw new NullReferenceException("a non-pseudoatom had unset hydrogen count");
        }

        /**
		 * Access formal charge of an atom defaulting to 0 if undefined.
		 *
		 * @param atom an atom
		 * @return the formal charge
		 */
        private static int Charge(IAtom atom)
        {
            int? charge = atom.FormalCharge;
            if (charge.HasValue) return charge.Value;
            return 0;
        }

        /**
		 * Locate explicit hydrogens that are attached to exactly one other atom.
		 *
		 * @param ac a structure
		 * @return binary set of terminal hydrogens
		 */
        public static bool[] terminalHydrogens(IAtomContainer ac, int[][] g)
        {

            bool[] hydrogens = new bool[ac.Atoms.Count];

            // we specifically don't check for null atomic number, this must be set.
            // if not, something major is wrong
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                hydrogens[i] = ac.Atoms[i].AtomicNumber == 1 && g[i].Length == 1;
            }

            return hydrogens;
        }

        /**
		 * The first 10,000 primes.
		 */
        private static readonly int[] PRIMES = LoadPrimes();

        private static int[] LoadPrimes()
        {
            try
            {
                using (var srm = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Canon), "primes.dat"))
                using (var br = new StreamReader(srm))
                {
                    int[] primes = new int[N_PRIMES];
                    int i = 0;
                    string line = null;
                    while ((line = br.ReadLine()) != null)
                    {
                        primes[i++] = int.Parse(line);
                    }
                    Trace.Assert(i == N_PRIMES);
                    return primes;
                }
            }
            catch (FormatException)
            {
                Console.Error.WriteLine("Critical - could not load primes table for canonical labelling!");
                return new int[0];
            }
        }
    }
}
