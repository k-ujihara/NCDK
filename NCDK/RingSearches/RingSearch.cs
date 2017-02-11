/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by the
 * Free Software Foundation; either version 2.1 of the License, or (at your
 * option) any later version. All we ask is that proper credit is given for our
 * work, which includes - but is not limited to - adding the above copyright
 * notice to the beginning of your source code files, and to any copyright
 * notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using NCDK.Graphs;

namespace NCDK.RingSearches
{
    /**
     * Efficiently search for atoms that are members of a ring. A depth first search
     * (DFS) determines which vertices belong to cycles (rings). As cycles are
     * discovered they are separated into two sets of cycle systems, fused and
     * isolated. A ring is in a fused cycle systems if it shares at least one edge
     * (bond) with another cycle. The isolated cycles consist of cycles which at
     * most share only one vertex (atom) with another cyclic system. A molecule may
     * contain more then one isolated and/or fused cycle system (see. Examples).
     * Additional computations such as C<sub>R</sub> (relevant cycles), Minimum
     * Cycle Basis (MCB) (aka. Smallest Set of Smallest Rings (SSSR)) or the Set of
     * All Rings can be completely bypassed for members of the isolated rings. Since
     * every isolated cycle (ring) does not share any edges (bonds) with any other
     * elementary cycle it cannot be made by composing any other cycles (rings).
     * Therefore, all isolated cycles (rings) are relevant and are members of all
     * minimum cycle bases (SSSRs). <b>Important</b> the cycle sets returned are not
     * ordered in the path of the cycle.
     *
     * <br/> <h4>Further Explanation</h4> The diagram below illustrates the isolated
     * and fused sets of cyclic atoms. The colored circles indicate the atoms and
     * bonds that are returned for each molecules. <br/><br/> <img alt="isolated and
     * fused cycle systems" src="http://i56.photobucket.com/albums/g187/johnymay/isolated-and-fused-cycles-01_zpse0311377.png"/>
     * <br/> <p/> <ol type="a"> <li>Two separate isolated cycles</li> <li>Two
     * separate fused cycle systems. The bridged systems are fused but separate from
     * each other</li> <li>Fused rings - a single fused cycle system</li> <li>Spiro
     * rings - three separate isolated systems, no bonds are shared</li>
     * <li>Cyclophane - a single fused system, the perimeter rings share bonds with
     * the smaller rings </li> <li>One isolated system and one fused system</li>
     * </ol>
     *
     * <h4>Example Usage</h4>
     * <blockquote><pre>
     * // construct the search for a given molecule, if an adjacency list
     * // representation (int[][]) is available this can be passed to the
     * // constructor for improved performance
     * IAtomContainer container  = ...;
     * RingSearch     ringSearch = new RingSearch(container);
     *
     * // indices of cyclic vertices
     * int[] cyclic = ringSearch.Cyclic();
     *
     * // iterate over fused systems (atom indices)
     * For(int[] fused : ringSearch.FUsed()){
     *     ...
     * }
     *
     * // iterate over isolated rings (atom indices)
     * For(int[] isolated : ringSearch.Isolated()){
     *     ...
     * }
     *
     * // convenience methods for getting the fragments
     * IAtomContainer cyclic = ringSearch.RingFragments();
     *
     * For(IAtomContainer fragment : ringSearch.FUsedRingFragments()){
     *     ....
     * }
     * For(IAtomContainer fragment : ringSearch.IsolatedRingFragments()){
     *     ....
     * }                *
     * </pre></blockquote>
     *
     * @author John May
     * @cdk.module core
     * @cdk.githash
     * @see <a href="http://en.wikipedia.org/wiki/Cycle_(graph_theory)">Cycle (Graph
     *      Theory) - Wikipedia</a>
     * @see <a href="http://efficientbits.blogspot.co.uk/2012/12/scaling-up-faster-ring-detection-in-cdk.html">Scaling
     *      Up: Faster Ring Detecting in CDK - Efficient Bits, Blog</a>
     * @see org.openscience.cdk.graph.SpanningTree
     * @see SSSRFinder
     * @see AllRingsFinder
     * @see CyclicVertexSearch
     */
    public sealed class RingSearch
    {
		/* depending on molecule size, delegate the search to one of two sub-classes */
		private readonly CyclicVertexSearch searcher;

		/* input atom container */
		private readonly IAtomContainer     container;

		/**
		 * Create a new RingSearch for the specified container.
		 *
		 * @param container non-null input structure
		 * @     if the container was null
		 * @ if the container contains a bond which
		 *                                  references an atom which could not be
		 *                                  found
		 */
		public RingSearch(IAtomContainer container) 
            :this(container, GraphUtil.ToAdjList(container))
		{}

		/**
		 * Create a new RingSearch for the specified container and graph. The
		 * adjacency list allows much faster graph traversal but is not free to
		 * create. If the adjacency list representation of the input container has
		 * already been created you can bypass the creation with this constructor.
		 *
		 * @param container non-null input structure
		 * @param graph     non-null adjacency list representation of the container
		 * @ if the container or graph was null
		 */
		public RingSearch(IAtomContainer container, int[][] graph) 
            :this(container, MakeSearcher(graph))
		{}

		/**
		 * Create a new RingSearch for the specified container using the provided
		 * search.
		 *
		 * @param container non-null input structure
		 * @param searcher  non-null adjacency list representation of the container
		 * @ if the container or searcher was null
		 */
		public RingSearch(IAtomContainer container, CyclicVertexSearch searcher)
        {
			if (container == null)
                throw new ArgumentNullException(nameof(container), "container must not be null");
            if (searcher == null)
                throw new ArgumentNullException(nameof(searcher), "searcher was null");
			this.searcher = searcher;
			this.container = container;
		}

		/**
		 * Utility method making a new {@link CyclicVertexSearch} during
		 * construction.
		 *
		 * @param graph non-null graph
		 * @return a new cyclic vertex search for the given graph
		 * @ if the graph was null
		 */
		private static CyclicVertexSearch MakeSearcher(int[][] graph) {

			if (graph == null)
                throw new ArgumentNullException(nameof(graph), "graph[][] must not be null");

			// if the molecule has 64 or less atoms we can use single 64 bit long
			// values to represent our sets of vertices
			if (graph.Length <= 64) {
				return new RegularCyclicVertexSearch(graph);
			} else {
				return new JumboCyclicVertexSearch(graph);
			}
		}

		/**
		 * Determine whether the edge between the vertices <i>u</i> and <i>v</i> is
		 * cyclic.
		 *
		 * @param u an end point of the edge
		 * @param v another end point of the edge
		 * @return whether the edge formed by the given end points is in a cycle
		 */
		public bool Cyclic(int u, int v) {
			return searcher.Cyclic(u, v);
		}

		/**
		 * Determine whether the provided atom belongs to a ring (is cyclic).
		 *
		 * <blockquote><pre>
		 * IAtomContainer mol        = ...;
		 * RingSearch     ringSearch = new RingSearch(mol);
		 *
		 * For(IAtom atom : mol.Atoms){
		 *     if(ringSearch.Cyclic(atom)){
		 *         ...
		 *     }
		 * }
		 * </pre></blockquote>
		 *
		 * @param atom an atom
		 * @return whether the atom is in a ring
		 * @ the atom was not found
		 */
		public bool Cyclic(IAtom atom)
        {
            int i = container.Atoms.IndexOf(atom);
			if (i < 0)
                throw new NoSuchAtomException("no such atom"); // fixed CDK's bug
			return Cyclic(i);
		}

		/**
		 * Determine whether the bond is cyclic. Note this currently requires a
		 * linear search to look-up the indices of each atoms.
		 *
		 * @param bond a bond of the container
		 * @return whether the vertex at the given index is in a cycle
		 */
		public bool Cyclic(IBond bond) {
			// XXX: linear search - but okay for now
			int u = container.Atoms.IndexOf(bond.Atoms[0]);
			int v = container.Atoms.IndexOf(bond.Atoms[1]);
			if (u < 0 || v < 0) throw new NoSuchAtomException("atoms of the bond are not found in the container");
			return searcher.Cyclic(u, v);
		}

		/**
		 * Determine whether the vertex at index <i>i</i> is a cyclic vertex.
		 *
		 * <blockquote><pre>
		 * IAtomContainer  mol    = ...;
		 * RingSearch      tester = new RingSearch(mol);
		 *
		 * int n = mol.Atoms.Count;
		 * For(int i = 0; i < n; i++){
		 *     if(tester.Cyclic(i)){
		 *         ...
		 *     }
		 * }
		 * </pre></blockquote>
		 *
		 * @param i atom index
		 * @return whether the vertex at the given index is in a cycle
		 */
		public bool Cyclic(int i) {
			return searcher.Cyclic(i);
		}

		/**
		 * Construct a set of vertices which belong to any cycle (ring).
		 *
		 * @return cyclic vertices
		 */
		public int[] Cyclic() {
			return searcher.Cyclic();
		}

		/**
		 * Construct the sets of vertices which belong to isolated rings.
		 *
		 * <blockquote><pre>
		 * IAtomContainer  biphenyl   = ...;
		 * RingSearch      ringSearch = new RingSearch(biphenyl);
		 *
		 * int[][] isolated = ringSearch.Isolated();
		 * isolated.Length; // 2 isolated rings in biphenyl
		 *
		 * isolated[0].Length; // 6 vertices in one benzene
		 * isolated[1].Length; // 6 vertices in the other benzene
		 *
		 * </pre></blockquote>
		 *
		 * @return array of isolated fragments, defined by the vertices in the
		 *         fragment
		 */
		public int[][] Isolated() {
			return searcher.Isolated();
		}

		/**
		 * Construct the sets of vertices which belong to fused ring systems.
		 *
		 * <blockquote><pre>
		 * IAtomContainer  mol        = ...;
		 * RingSearch      ringSearch = new RingSearch(mol);
		 *
		 * int[][] fused = ringSearch.FUsed();
		 * fused.Length; // e.g. 3 separate fused ring systems
		 *
		 * fused[0].Length; // e.g. 6 vertices in the first system
		 * fused[1].Length; // e.g. 10 vertices in the second system
		 * fused[2].Length; // e.g. 4 vertices in the third system
		 *
		 * </pre></blockquote>
		 *
		 * @return array of fused fragments, defined by the vertices in the
		 *         fragment
		 */
		public int[][] FUsed() {
			return searcher.FUsed();
		}

		/**
		 * Extract the cyclic atom and bond fragments of the container. Bonds which
		 * join two different isolated/fused cycles (e.g. biphenyl) are not be
		 * included.
		 *
		 * @return a new container with only the cyclic atoms and bonds
		 * @see org.openscience.cdk.graph.SpanningTree#GetCyclicFragmentsContainer()
		 */
		public IAtomContainer RingFragments() {

			int[] vertices = Cyclic();

			int n = vertices.Length;

			IAtom[] atoms = new IAtom[n];
			List<IBond> bonds = new List<IBond>();

			for (int i = 0; i < vertices.Length; i++) {
				atoms[i] = container.Atoms[vertices[i]];
			}

            var abonds = container.Bonds;

            foreach (var bond in abonds) {

				IAtom either = bond.Atoms[0];
				IAtom other = bond.Atoms[1];

				int u = container.Atoms.IndexOf(either);
				int v = container.Atoms.IndexOf(other);

				// add the bond if the vertex colors match
				if (searcher.Cyclic(u, v)) bonds.Add(bond);
			}

			IChemObjectBuilder builder = container.Builder;
            IAtomContainer fragment = builder.CreateAtomContainer(atoms, bonds);

			return fragment;
		}

        /**
		 * Determines whether the two vertex colors match. This method provides the
		 * conditional as to whether to include a bond in the construction of the
		 * {@link #RingFragments()}.
		 *
		 * @param eitherColor either vertex color
		 * @param otherColor  other vertex color
		 * @return whether the two vertex colours match
		 */
#if TEST
        public
#endif
        static bool Match(int eitherColor, int otherColor) {
			return (eitherColor != -1 && otherColor != -1)
					&& (eitherColor == otherColor || (eitherColor == 0 || otherColor == 0));
		}

		/**
		 * Construct a list of <see cref="IAtomContainer"/>s each of which only contains a
		 * single isolated ring. A ring is consider isolated if it does not share
		 * any bonds with another ring. By this definition each ring of a spiro ring
		 * system is considered isolated. The atoms are <b>not</b> arranged
		 * sequential.
		 *
		 * @return list of isolated ring fragments
		 * @see #Isolated()
		 */
		public IList<IAtomContainer> IsolatedRingFragments() {
			return ToFragments(Isolated());
		}

		/**
		 * Construct a list of <see cref="IAtomContainer"/>s which only contain fused
		 * rings. A ring is consider fused if it shares any bonds with another ring.
		 * By this definition bridged ring systems are also included. The atoms are
		 * <b>not</b> arranged sequential.
		 *
		 * @return list of fused ring fragments
		 * @see #FUsed()
		 */
		public IList<IAtomContainer> FUsedRingFragments() {
			return ToFragments(FUsed());
		}

		/**
		 * Utility method for creating the fragments for the fused/isolated sets
		 *
		 * @param verticesList 2D array of vertices (rows=n fragments)
		 * @return the vertices converted to an atom container
		 * @see #ToFragment(int[])
		 * @see #FUsedRingFragments()
		 * @see #IsolatedRingFragments()
		 */
		private IList<IAtomContainer> ToFragments(int[][] verticesList) {
			List<IAtomContainer> fragments = new List<IAtomContainer>();
			foreach (var vertices in verticesList) {
				fragments.Add(ToFragment(vertices));
			}
			return fragments;
		}

		/**
		 * Utility method for creating a fragment from an array of vertices
		 *
		 * @param vertices array of vertices .Length=cycle weight, values 0 ...
		 *                 nAtoms
		 * @return atom container only containing the specified atoms (and bonds)
		 */
		private IAtomContainer ToFragment(int[] vertices) {

			int n = vertices.Length;

            ICollection<IAtom> atoms = new HashSet<IAtom>();
            IList<IBond> bonds = new List<IBond>();

			// fill the atom set
			foreach (var v in vertices) {
				atoms.Add(container.Atoms[v]);
			}

			// include bonds that have both atoms in the atoms set
			foreach (var bond in container.Bonds) {
				IAtom either = bond.Atoms[0];
				IAtom other = bond.Atoms[1];
				if (atoms.Contains(either) && atoms.Contains(other)) {
					bonds.Add(bond);
				}
			}

			IAtomContainer fragment = container.Builder.CreateAtomContainer(atoms, bonds);

			return fragment;
		}
	}
}
