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
using static NCDK.Graphs.GraphUtil;
using NCDK.Common.Collections;
using NCDK.RingSearches;
using System.Collections;

namespace NCDK.Graphs
{
    /**
     * A utility class for storing and computing the cycles of a chemical graph.
     * Utilities are also provided for converting the cycles to {@link IRing}s. A
     * brief description of each cycle set is given below - for a more comprehensive
     * review please see - {@cdk.cite Berger04}.
     *
     * <ul> <li>{@link #All()} - all simple cycles in the graph, the number of
     * cycles generated may be very large and may not be feasible for some
     * molecules, such as, fullerene.</li> <li>{@link #MCB} (aka. SSSR) - minimum
     * cycle basis (MCB) of a graph, these cycles are linearly independent and can
     * be used to generate all of cycles in the graph. It is important to note the
     * MCB is not unique and a that there may be multiple equally valid MCBs. The
     * smallest set of smallest rings (SSSR) is often used to refer to the MCB but
     * originally SSSR was defined as a strictly fundamental cycle basis {@cdk.cite
     * Berger04}. Not every graph has a strictly fundamental cycle basis the
     * definition has come to mean the MCB. Due to the non-uniqueness of the
     * MCB/SSSR its use is discouraged.</li> <li>{@link #Relevant} - relevant
     * cycles of a graph, the smallest set of uniquely defined short cycles. If a
     * graph has a single MCB then the relevant cycles and MCB are the same. If the
     * graph has multiple MCB then the relevant cycles is the union of all MCBs. The
     * number of relevant cycles may be exponential but it is possible to determine
     * how many relevant cycles there are in polynomial time without generating
     * them. For chemical graphs the number of relevant cycles is usually within
     * manageable bounds. </li> <li>{@link #Essential} - essential cycles of a
     * graph. Similar to the relevant cycles the set is unique for a graph. If a
     * graph has a single MCB then the essential cycles and MCB are the same. If the
     * graph has multiple MCB then the essential cycles is the intersect of all
     * MCBs. That is the cycles which appear in every MCB. This means that is is
     * possible to have no essential cycles in a molecule which clearly has cycles
     * (e.g. bridged system like bicyclo[2.2.2]octane). </li> <li> {@link
     * #TripletShort} - the triple short cycles are the shortest cycle through
     * each triple of vertices. This allows one to generate the envelope rings of
     * some molecules (e.g. naphthalene) without generating all cycles. The cycles
     * are primarily useful for the CACTVS Substructure Keys (PubChem fingerprint).
     * </li> <li> {@link #VertexShort} - the shortest cycles through each vertex.
     * Unlike the MCB, linear independence is not checked and it may not be possible
     * to generate all other cycles from this set. In practice the vertex/edge short
     * cycles are similar to MCB. </li> <li> {@link #EdgeShort} - the shortest
     * cycles through each edge. Unlike the MCB, linear independence is not checked
     * and it may not be possible to generate all other cycles from this set. In
     * practice the vertex/edge short cycles are similar to MCB. </li> </ul>
     *
     * @author John May
     * @cdk.module core
     * @cdk.githash
     */
    public sealed class Cycles
    {
        /// <summary>Vertex paths for each cycle.</summary>
        private readonly int[][] paths;

        /// <summary>The input container - allows us to create 'Ring' objects.</summary>
        private readonly IAtomContainer container;

        /// <summary>Mapping for quick lookup of bond mapping.</summary>
        private readonly EdgeToBondMap bondMap;

        /**
         * Internal constructor - may change in future but currently just takes the
         * cycle paths and the container from which they came.
         *
         * @param paths     the cycle paths (closed vertex walks)
         * @param container the input container
         */
        private Cycles(int[][] paths, IAtomContainer container, EdgeToBondMap bondMap)
        {
            this.paths = paths;
            this.container = container;
            this.bondMap = bondMap;
        }

        /**
         * How many cycles are stored.
         *
         * @return number of cycles
         */
        public int GetNumberOfCycles()
        {
            return paths.Length;
        }

        public int[][] GetPaths()
        {
            int[][] cpy = new int[paths.Length][];
            for (int i = 0; i < paths.Length; i++)
                cpy[i] = Arrays.Clone(paths[i]);
            return cpy;
        }

        /**
         * Convert the cycles to a {@link IRingSet} containing the {@link IAtom}s
         * and {@link IBond}s of the input molecule.
         *
         * @return ringset for the cycles
         */
        public IRingSet ToRingSet()
        {
            return ToRingSet(container, paths, bondMap);
        }

        /**
         * Create a cycle finder which will compute all simple cycles in a molecule.
         * The threshold values can not be tuned and is set at a value which will
         * complete in reasonable time for most molecules. To change the threshold
         * values please use the stand-alone {@link AllCycles} or {@link
         * org.openscience.cdk.ringsearch.AllRingsFinder}. All cycles is every
         * possible simple cycle (i.e. non-repeating vertices) in the chemical
         * graph. As an example - all simple cycles of anthracene includes, 3 cycles
         * of length 6, 2 of length 10 and 1 of length 14.
         *
         * <blockquote>
         * <pre>
         * CycleFinder cf = Cycles.All();
         * foreach (var m in ms) {
         *     try {
         *         Cycles   cycles = cf.Find(m);
         *         IRingSet rings  = cycles.ToRingSet();
         *     } catch (Intractable e) {
         *         // handle error - note it is common that finding all simple
         *         // cycles in chemical graphs is intractable
         *     }
         * }
         * </pre>
         * </blockquote>
         *
         * @return finder for all simple cycles
         * @see #All(IAtomContainer)
         * @see AllCycles
         * @see org.openscience.cdk.ringsearch.AllRingsFinder
         */
        public static CycleFinder All()
        {
            return CycleComputation.ALL;
        }

        /**
         * All cycles of smaller than or equal to the specified length. If a length
         * is also provided to {@link CycleFinder#Find(IAtomContainer, int)} the
         * minimum of the two limits is used.
         *
         * @param length maximum size or cycle to find
         * @return cycle finder
         */
        public static CycleFinder All(int length)
        {
            return new AllUpToLength(length);
        }

        /**
         * Create a cycle finder which will compute the minimum cycle basis (MCB) of
         * a molecule.
         *
         * <blockquote>
         * <pre>
         * CycleFinder cf = Cycles.MCB;
         * foreach (var m in ms) {
         *     try {
         *         Cycles   cycles = cf.Find(m);
         *         IRingSet rings  = cycles.ToRingSet();
         *     } catch (Intractable e) {
         *         // ignore error - MCB should never be intractable
         *     }
         * }
         * </pre>
         * </blockquote>
         *
         * @return finder for all simple cycles
         * @see #FindMCB(IAtomContainer)
         * @see MinimumCycleBasis
         */
        public static CycleFinder MCB => CycleComputation.MCB;

        /**
         * Create a cycle finder which will compute the relevant cycle basis (RC) of
         * a molecule.
         *
         * <blockquote>
         * <pre>
         * CycleFinder cf = Cycles.Relevant;
         * foreach (var m in ms) {
         *     try {
         *         Cycles   cycles = cf.Find(m);
         *         IRingSet rings  = cycles.ToRingSet();
         *     } catch (Intractable e) {
         *         // ignore error - there may be an exponential number of cycles
         *         // but this is not currently checked
         *     }
         * }
         * </pre>
         * </blockquote>
         *
         * @return finder for relevant cycles
         * @see #FindRelevant(IAtomContainer)
         * @see RelevantCycles
         */
        public static CycleFinder Relevant => CycleComputation.RELEVANT;

        /**
         * Create a cycle finder which will compute the essential cycles of a
         * molecule.
         *
         * <blockquote>
         * <pre>
         * CycleFinder cf = Cycles.Essential;
         * foreach (var m in ms) {
         *     try {
         *         Cycles   cycles = cf.Find(m);
         *         IRingSet rings  = cycles.ToRingSet();
         *     } catch (Intractable e) {
         *         // ignore error - essential cycles do not check tractability
         *     }
         * }
         * </pre>
         * </blockquote>
         *
         * @return finder for essential cycles
         * @see #FindRelevant(IAtomContainer)
         * @see RelevantCycles
         */
        public static CycleFinder Essential => CycleComputation.ESSENTIAL;

        /**
         * Create a cycle finder which will compute the triplet short cycles of a
         * molecule. These cycles are the shortest through each triplet of vertices
         * are utilised in the generation of CACTVS Substructure Keys (PubChem
         * Fingerprint). Currently the triplet cycles are non-canonical (which in
         * this algorithms case means unique). For finer tuning of options please
         * use the {@link TripletShortCycles}.
         *
         * <blockquote>
         * <pre>
         * CycleFinder cf = Cycles.TripletShort;
         * foreach (var m in ms) {
         *     try {
         *         Cycles   cycles = cf.Find(m);
         *         IRingSet rings  = cycles.ToRingSet();
         *     } catch (Intractable e) {
         *         // ignore error - triple short cycles do not check tractability
         *     }
         * }
         * </pre>
         * </blockquote>
         *
         * @return finder for triplet short cycles
         * @see #FindTripletShort(IAtomContainer)
         * @see TripletShortCycles
         */
        public static CycleFinder TripletShort => CycleComputation.TRIPLET_SHORT;

        /**
         * Create a cycle finder which will compute the shortest cycles of each
         * vertex in a molecule. Unlike the SSSR/MCB computation linear independence
         * is not required and provides some performance gain. In practise typical
         * chemical graphs are small and the linear independence check is relatively
         * fast.
         *
         * <blockquote>
         * <pre>
         * CycleFinder cf = Cycles.VertexShort;
         * foreach (var m in ms) {
         *     try {
         *         Cycles   cycles = cf.Find(m);
         *         IRingSet rings  = cycles.ToRingSet();
         *     } catch (Intractable e) {
         *         // ignore error - vertex short cycles do not check tractability
         *     }
         * }
         * </pre>
         * </blockquote>
         *
         * @return finder for vertex short cycles
         * @see #FindVertexShort(IAtomContainer)
         */
        public static CycleFinder VertexShort => CycleComputation.VERTEX_SHORT;

        /**
         * Create a cycle finder which will compute the shortest cycles of each
         * vertex in a molecule. Unlike the SSSR/MCB computation linear independence
         * is not required and provides some performance gain. In practise typical
         * chemical graphs are small and the linear independence check is relatively
         * fast.
         *
         * <blockquote>
         * <pre>
         * CycleFinder cf = Cycles.EdgeShort;
         * foreach (var m in ms) {
         *     try {
         *         Cycles   cycles = cf.Find(m);
         *         IRingSet rings  = cycles.ToRingSet();
         *     } catch (Intractable e) {
         *         // ignore error - edge short cycles do not check tractability
         *     }
         * }
         * </pre>
         * </blockquote>
         *
         * @return finder for edge short cycles
         * @see #FindEdgeShort(IAtomContainer)
         */
        public static CycleFinder EdgeShort => CycleComputation.EDGE_SHORT;

        /**
         * Create a cycle finder which will compute a set of cycles traditionally
         * used by the CDK to test for aromaticity. This set of cycles is the
         * MCB/SSSR and {@link #all} cycles for fused systems with 3 or less rings.
         * This allows on to test aromaticity of envelope rings in compounds such as
         * azulene without generating an huge number of cycles for large fused
         * systems (e.g. fullerenes). The use case was that computation of all
         * cycles previously took a long time and ring systems with more than 2
         * rings were too difficult. However it is now more efficient to simply
         * check all cycles/rings without using the MCB/SSSR. This computation will
         * fail for complex fused systems but the failure is fast and one can easily
         * 'fall back' to a smaller set of cycles after catching the exception.
         *
         * <blockquote>
         * <pre>
         * CycleFinder cf = Cycles.CDKAromaticSet;
         * foreach (var m in ms) {
         *     try {
         *         Cycles   cycles = cf.Find(m);
         *         IRingSet rings  = cycles.ToRingSet();
         *     } catch (Intractable e) {
         *         // ignore error - edge short cycles do not check tractability
         *     }
         * }
         * </pre>
         * </blockquote>
         *
         * @return finder for cdk aromatic cycles
         * @see #FindEdgeShort(IAtomContainer)
         */
        public static CycleFinder CDKAromaticSet => CycleComputation.CDK_AROMATIC;

        /**
         * Find all cycles in a fused system or if there were too many cycles
         * fallback and use the shortest cycles through each vertex. Typically the
         * types of molecules which the vertex short cycles are provided for are
         * fullerenes. This cycle finder is well suited to aromaticity.
         *
         * <blockquote>
         * <pre>
         * CycleFinder cf = Cycles.FindAllOrVertexShort();
         * foreach (var m in ms) {
         *     try {
         *         Cycles   cycles = cf.Find(m);
         *         IRingSet rings  = cycles.ToRingSet();
         *     } catch (Intractable e) {
         *         // ignore error - edge short cycles do not check tractability
         *     }
         * }
         * </pre>
         * </blockquote>
         *
         * @return a cycle finder which computes all cycles if possible or provides
         * the vertex short cycles
         * @deprecated use {@link #or} to define a custom fall-back
         */
        [Obsolete]
        public static CycleFinder FindAllOrVertexShort()
        {
            return Or(All(), VertexShort);
        }

        /**
         * Find and mark all cyclic atoms and bonds in the provided molecule.
         *
         * @param mol molecule
         * @see IBond#IsInRing
         * @see IAtom#IsInRing
         */
        public static void MarkRingAtomsAndBonds(IAtomContainer mol)
        {
            EdgeToBondMap bonds = EdgeToBondMap.WithSpaceFor(mol);
            MarkRingAtomsAndBonds(mol, GraphUtil.ToAdjList(mol, bonds), bonds);
        }

        /**
         * Find and mark all cyclic atoms and bonds in the provided molecule. This optimised version
         * allows the caller to optionally provided indexed fast access structure which would otherwise
         * be created.
         *
         * @param mol molecule
         * @see IBond#IsInRing
         * @see IAtom#IsInRing
         */
        public static void MarkRingAtomsAndBonds(IAtomContainer mol, int[][] adjList, EdgeToBondMap bondMap)
        {
            RingSearch ringSearch = new RingSearch(mol, adjList);
            for (int v = 0; v < mol.Atoms.Count; v++)
            {
                mol.Atoms[v].IsInRing = false;
                foreach (var w in adjList[v])
                {
                    // note we only mark the bond on second visit (first v < w) and
                    // clear flag on first visit (or if non-cyclic)
                    if (v > w && ringSearch.Cyclic(v, w))
                    {
                        bondMap[v, w].IsInRing = true;
                        mol.Atoms[v].IsInRing = true;
                        mol.Atoms[w].IsInRing = true;
                    }
                    else
                    {
                        bondMap[v, w].IsInRing = false;
                    }
                }
            }
        }

        /**
         * Use an auxiliary cycle finder if the primary method was intractable.
         *
         * <blockquote><pre>
         * // all cycles or all cycles size <= 6
         * CycleFinder cf = Cycles.Or(Cycles.All(), Cycles.All(6));
         * </pre></blockquote>
         *
         * It is possible to nest multiple levels.
         *
         * <blockquote><pre>
         * // all cycles or relevant or essential
         * CycleFinder cf = Cycles.Or(Cycles.All(),
         *                            Cycles.Or(Cycles.Relevant,
         *                                      Cycles.Essential));
         * </pre></blockquote>
         *
         * @param primary   primary cycle finding method
         * @param auxiliary auxiliary cycle finding method if the primary failed
         * @return a new cycle finder
         */
        public static CycleFinder Or(CycleFinder primary, CycleFinder auxiliary)
        {
            return new Fallback(primary, auxiliary);
        }

        /**
         * Find all simple cycles in a molecule. The threshold values can not be
         * tuned and is set at a value which will complete in reasonable time for
         * most molecules. To change the threshold values please use the stand-alone
         * {@link AllCycles} or {@link org.openscience.cdk.ringsearch.AllRingsFinder}.
         * All cycles is every possible simple cycle (i.e. non-repeating vertices)
         * in the chemical graph. As an example - all simple cycles of anthracene
         * includes, 3 cycles of length 6, 2 of length 10 and 1 of length 14.
         *
         * <blockquote>
         * <pre>
         * foreach (var m in ms) {
         *     try {
         *         Cycles   cycles = Cycles.All(m);
         *         IRingSet rings  = cycles.ToRingSet();
         *     } catch (Intractable e) {
         *         // handle error - note it is common that finding all simple
         *         // cycles in chemical graphs is intractable
         *     }
         * }
         * </pre>
         * </blockquote>
         *
         * @return all simple cycles
         * @ the algorithm reached a limit which caused it to
         *                     abort in reasonable time
         * @see #All()
         * @see AllCycles
         * @see org.openscience.cdk.ringsearch.AllRingsFinder
         */
        public static Cycles All(IAtomContainer container)
        {
            return All().Find(container, container.Atoms.Count);
        }

        /**
         * All cycles of smaller than or equal to the specified length.
         *
         * @param container input container
         * @param length    maximum size or cycle to find
         * @return all cycles
         * @ computation was not feasible
         */
        public static Cycles All(IAtomContainer container, int length)
        {
            return All().Find(container, length);
        }

        /**
         * Find the minimum cycle basis (MCB) of a molecule.
         *
         * <blockquote>
         * <pre>
         * foreach (var m in ms) {
         *     Cycles   cycles = Cycles.FindMCB(m);
         *     IRingSet rings  = cycles.ToRingSet();
         * }
         * </pre>
         * </blockquote>
         *
         * @return cycles belonging to the minimum cycle basis
         * @see #MCB
         * @see MinimumCycleBasis
         */
        public static Cycles FindMCB(IAtomContainer container)
        {
            return _invoke(MCB, container);
        }

        /**
         * Find the smallest set of smallest rings (SSSR) - aka minimum cycle basis
         * (MCB) of a molecule.
         *
         * <blockquote>
         * <pre>
         * foreach (var m in ms) {
         *     Cycles   cycles = Cycles.SSSR(m);
         *     IRingSet rings  = cycles.ToRingSet();
         * }
         * </pre>
         * </blockquote>
         *
         * @return cycles belonging to the minimum cycle basis
         * @see #MCB
         * @see #FindMCB(IAtomContainer)
         * @see MinimumCycleBasis
         */
        public static Cycles SSSR(IAtomContainer container)
        {
            return FindMCB(container);
        }

        /**
         * Find the relevant cycles of a molecule.
         *
         * <blockquote>
         * <pre>
         * foreach (var m in ms) {
         *     Cycles   cycles = Cycles.FindRelevant(m);
         *     IRingSet rings  = cycles.ToRingSet();
         * }
         * </pre>
         * </blockquote>
         *
         * @return relevant cycles
         * @see #Relevant
         * @see RelevantCycles
         */
        public static Cycles FindRelevant(IAtomContainer container)
        {
            return _invoke(Relevant, container);
        }

        /**
         * Find the essential cycles of a molecule.
         *
         * <blockquote>
         * <pre>
         * foreach (var m in ms) {
         *     Cycles   cycles = Cycles.FindEssential(m);
         *     IRingSet rings  = cycles.ToRingSet();
         * }
         * </pre>
         * </blockquote>
         *
         * @return essential cycles
         * @see #Relevant
         * @see RelevantCycles
         */
        public static Cycles FindEssential(IAtomContainer container)
        {
            return _invoke(Essential, container);
        }

        /**
         * Find the triplet short cycles of a molecule.
         *
         * <blockquote>
         * <pre>
         * foreach (var m in ms) {
         *     Cycles   cycles = Cycles.FindTripletShort(m);
         *     IRingSet rings  = cycles.ToRingSet();
         * }
         * </pre>
         * </blockquote>
         *
         * @return triplet short cycles
         * @see #TripletShort
         * @see TripletShortCycles
         */
        public static Cycles FindTripletShort(IAtomContainer container)
        {
            return _invoke(TripletShort, container);
        }

        /**
         * Find the vertex short cycles of a molecule.
         *
         * <blockquote>
         * <pre>
         * foreach (var m in ms) {
         *     Cycles   cycles = Cycles.FindVertexShort(m);
         *     IRingSet rings  = cycles.ToRingSet();
         * }
         * </pre>
         * </blockquote>
         *
         * @return triplet short cycles
         * @see #VertexShort
         * @see VertexShortCycles
         */
        public static Cycles FindVertexShort(IAtomContainer container)
        {
            return _invoke(VertexShort, container);
        }

        /**
         * Find the edge short cycles of a molecule.
         *
         * <blockquote>
         * <pre>
         * foreach (var m in ms) {
         *     Cycles   cycles = Cycles.FindEdgeShort(m);
         *     IRingSet rings  = cycles.ToRingSet();
         * }
         * </pre>
         * </blockquote>
         *
         * @return edge short cycles
         * @see #EdgeShort
         * @see EdgeShortCycles
         */
        public static Cycles FindEdgeShort(IAtomContainer container)
        {
            return _invoke(EdgeShort, container);
        }

        /**
         * Derive a new cycle finder that only provides cycles without a chord.
         *
         * @param original find the initial cycles before filtering
         * @return cycles or the original without chords
         */
        public static CycleFinder GetUnchorded(CycleFinder original)
        {
            return new Unchorded(original);
        }

        /**
         * Internal method to wrap cycle computations which <i>should</i> be
         * tractable. That is they currently won't throw the exception - if the
         * method does throw an exception an internal error is triggered as a sanity
         * check.
         *
         * @param finder    the cycle finding method
         * @param container the molecule to find the cycles of
         * @return the cycles of the molecule
         */
        private static Cycles _invoke(CycleFinder finder, IAtomContainer container)
        {
            return _invoke(finder, container, container.Atoms.Count);
        }

        /**
         * Internal method to wrap cycle computations which <i>should</i> be
         * tractable. That is they currently won't throw the exception - if the
         * method does throw an exception an internal error is triggered as a sanity
         * check.
         *
         * @param finder    the cycle finding method
         * @param container the molecule to find the cycles of
         * @param length    maximum size or cycle to find
         * @return the cycles of the molecule
         */
        private static Cycles _invoke(CycleFinder finder, IAtomContainer container, int length)
        {
            try
            {
                return finder.Find(container, length);
            }
            catch (Intractable e)
            {
                throw new ApplicationException("Cycle computation should not be intractable: ", e);
            }
        }

        /// <summary>Interbank enumeration of cycle finders.</summary>
        private abstract class CycleComputation
                  : CycleFinder
        {
            public static CycleComputation MCB = new MCB_CycleComputation();
            class MCB_CycleComputation
                : CycleComputation
            {
                /// <inheritdoc/>
                public override int[][] Apply(int[][] graph, int length)
                {
                    InitialCycles ic = InitialCycles.OfBiconnectedComponent(graph, length);
                    return new MinimumCycleBasis(ic, true).GetPaths();
                }
            }

            public static CycleComputation ESSENTIAL = new ESSENTIAL_CycleComputation();
            class ESSENTIAL_CycleComputation
                : CycleComputation
            {

                /// <inheritdoc/>
                public override int[][] Apply(int[][] graph, int length)
                {
                    InitialCycles ic = InitialCycles.OfBiconnectedComponent(graph, length);
                    RelevantCycles rc = new RelevantCycles(ic);
                    return new EssentialCycles(rc, ic).GetPaths();
                }
            }

            public static CycleComputation RELEVANT = new RELEVANT_CycleComputation();
            class RELEVANT_CycleComputation
                : CycleComputation
            {
                /// <inheritdoc/>
                public override int[][] Apply(int[][] graph, int length)
                {
                    InitialCycles ic = InitialCycles.OfBiconnectedComponent(graph, length);
                    return new RelevantCycles(ic).GetPaths();
                }
            }

            public static CycleComputation ALL = new ALL_CycleComputation();
            class ALL_CycleComputation
                : CycleComputation
            {
                /// <inheritdoc/>
                public override int[][] Apply(int[][] graph, int length)
                {
                    int threshold = 684; // see. AllRingsFinder.Threshold.Pubchem_99
                    AllCycles ac = new AllCycles(graph, Math.Min(length, graph.Length), threshold);
                    if (!ac.Completed)
                        throw new Intractable("A large number of cycles were being generated and the"
                                + " computation was aborted. Please use AllCycles/AllRingsFinder with"
                                + " and specify a larger threshold or use a CycleFinger with a fall-back"
                                + " to a set unique cycles: e.g. Cycles.FindAllOrVertexShort().");
                    return ac.GetPaths();
                }
            }

            public static CycleComputation TRIPLET_SHORT = new TRIPLET_SHORT_CycleComputation();
            class TRIPLET_SHORT_CycleComputation
                  : CycleComputation
            {

                /// <inheritdoc/>
                public override int[][] Apply(int[][] graph, int length)
                {
                    InitialCycles ic = InitialCycles.OfBiconnectedComponent(graph, length);
                    return new TripletShortCycles(new MinimumCycleBasis(ic, true), false).GetPaths();
                }
            }

            public static CycleComputation VERTEX_SHORT = new VERTEX_SHORT_CycleComputation();
            class VERTEX_SHORT_CycleComputation
                : CycleComputation
            {
                /// <inheritdoc/>
                public override int[][] Apply(int[][] graph, int length)
                {
                    InitialCycles ic = InitialCycles.OfBiconnectedComponent(graph, length);
                    return new VertexShortCycles(ic).GetPaths();
                }
            }

            public static CycleComputation EDGE_SHORT = new EDGE_SHORT_CycleComputation();
            class EDGE_SHORT_CycleComputation
                : CycleComputation
            {
                /// <inheritdoc/>
                public override int[][] Apply(int[][] graph, int length)
                {
                    InitialCycles ic = InitialCycles.OfBiconnectedComponent(graph, length);
                    return new EdgeShortCycles(ic).GetPaths();
                }
            }

            public static CycleComputation CDK_AROMATIC = new CDK_AROMATIC_CycleComputation();
            class CDK_AROMATIC_CycleComputation
                : CycleComputation
            {
                /// <inheritdoc/>
                public override int[][] Apply(int[][] graph, int length)
                {
                    InitialCycles ic = InitialCycles.OfBiconnectedComponent(graph, length);
                    MinimumCycleBasis mcb = new MinimumCycleBasis(ic, true);

                    // As per the old aromaticity detector if the MCB/SSSR is made
                    // of 2 or 3 rings we check all rings for aromaticity - otherwise
                    // we just check the MCB/SSSR
                    if (mcb.Count > 3)
                    {
                        return mcb.GetPaths();
                    }
                    else
                    {
                        return ALL.Apply(graph, length);
                    }
                }
            }

            public static CycleComputation ALL_OR_VERTEX_SHORT = new ALL_OR_VERTEX_SHORT_CycleComputation();
            class ALL_OR_VERTEX_SHORT_CycleComputation
                : CycleComputation
            {
                /// <inheritdoc/>
                public override int[][] Apply(int[][] graph, int length)
                {
                    int threshold = 684; // see. AllRingsFinder.Threshold.Pubchem_99
                    AllCycles ac = new AllCycles(graph, Math.Min(length, graph.Length), threshold);

                    return ac.Completed ? ac.GetPaths() : VERTEX_SHORT.Apply(graph, length);
                }
            }

            /**
             * Apply cycle perception to the graph (g) - graph is expeced to be
             * biconnected.
             *
             * @param graph the graph (adjacency list)
             * @return the cycles of the graph
             * @ the computation reached a set limit
             */
            public abstract int[][] Apply(int[][] graph, int length);

            /// <inheritdoc/>
            public Cycles Find(IAtomContainer molecule)
            {
                return Find(molecule, molecule.Atoms.Count);
            }

            /// <inheritdoc/>
            public Cycles Find(IAtomContainer molecule, int length)
            {
                EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(molecule);
                int[][] graph = GraphUtil.ToAdjList(molecule, bondMap);
                RingSearch ringSearch = new RingSearch(molecule, graph);

                IList<int[]> walks = new List<int[]>(6);

                // all isolated cycles are relevant - all we need to do is walk around
                // the vertices in the subset 'isolated'
                foreach (var isolated in ringSearch.Isolated())
                {
                    if (isolated.Length <= length) walks.Add(GraphUtil.Cycle(graph, isolated));
                }

                // each biconnected component which isn't an isolated cycle is processed
                // separately as a subgraph.
                foreach (var fused in ringSearch.FUsed())
                {

                    // make a subgraph and 'apply' the cycle computation - the walk
                    // (path) is then lifted to the original graph
                    foreach (var cycle in Apply(GraphUtil.Subgraph(graph, fused), length))
                    {
                        walks.Add(Lift(cycle, fused));
                    }
                }

                return new Cycles(walks.ToArray(), molecule, bondMap);
            }

            /// <inheritdoc/>
            public Cycles Find(IAtomContainer molecule, int[][] graph, int length)
            {

                RingSearch ringSearch = new RingSearch(molecule, graph);

                List<int[]> walks = new List<int[]>();

                // all isolated cycles are relevant - all we need to do is walk around
                // the vertices in the subset 'isolated'
                foreach (var isolated in ringSearch.Isolated())
                {
                    walks.Add(GraphUtil.Cycle(graph, isolated));
                }

                // each biconnected component which isn't an isolated cycle is processed
                // separately as a subgraph.
                foreach (var fused in ringSearch.FUsed())
                {

                    // make a subgraph and 'apply' the cycle computation - the walk
                    // (path) is then lifted to the original graph
                    foreach (var cycle in Apply(GraphUtil.Subgraph(graph, fused), length))
                    {
                        walks.Add(Lift(cycle, fused));
                    }
                }

                return new Cycles(walks.ToArray(), molecule, null);
            }
        }

        /**
         * Internal - lifts a path in a subgraph back to the original graph.
         *
         * @param path    a path
         * @param mapping vertex mapping
         * @return the lifted path
         */
        private static int[] Lift(int[] path, int[] mapping)
        {
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = mapping[path[i]];
            }
            return path;
        }

        /**
         * Internal - convert a set of cycles to an ring set.
         *
         * @param container molecule
         * @param cycles    a cycle of the chemical graph
         * @param bondMap   mapping of the edges (int,int) to the bonds of the
         *                  container
         * @return the ring set
         */
        private static IRingSet ToRingSet(IAtomContainer container, int[][] cycles, EdgeToBondMap bondMap)
        {

            // note currently no way to say the size of the RingSet
            // even through we know it
            IChemObjectBuilder builder = container.Builder;
            IRingSet rings = builder.CreateRingSet();

            foreach (var cycle in cycles)
            {
                rings.Add(ToRing(container, cycle, bondMap));
            }

            return rings;
        }

        /**
         * Internal - convert a set of cycles to a ring.
         *
         * @param container molecule
         * @param cycle     a cycle of the chemical graph
         * @param bondMap   mapping of the edges (int,int) to the bonds of the
         *                  container
         * @return the ring for the specified cycle
         */
        private static IRing ToRing(IAtomContainer container, int[] cycle, EdgeToBondMap bondMap)
        {
            IAtom[] atoms = new IAtom[cycle.Length - 1];
            IBond[] bonds = new IBond[cycle.Length - 1];

            for (int i = 1; i < cycle.Length; i++)
            {
                int v = cycle[i];
                int u = cycle[i - 1];
                atoms[i - 1] = container.Atoms[u];
                bonds[i - 1] = GetBond(container, bondMap, u, v);
            }

            IChemObjectBuilder builder = container.Builder;
            IAtomContainer ring = builder.CreateAtomContainer(atoms, bonds);

            return builder.CreateRing(ring);
        }

        /**
         * Obtain the bond between the atoms at index 'u' and 'v'. If the 'bondMap'
         * is non-null it is used for direct lookup otherwise the slower linear
         * lookup in 'container' is used.
         *
         * @param container a structure
         * @param bondMap   optimised map of atom indices to bond instances
         * @param u         an atom index
         * @param v         an atom index (connected to u)
         * @return the bond between u and v
         */
        private static IBond GetBond(IAtomContainer container, EdgeToBondMap bondMap, int u, int v)
        {
            if (bondMap != null) return bondMap[u, v];
            return container.GetBond(container.Atoms[u], container.Atoms[v]);
        }

        /**
         * All cycles smaller than or equal to a specified length.
         */
        private sealed class AllUpToLength
            : CycleFinder
        {
            private readonly int predefinedLength;

            // see. AllRingsFinder.Threshold.Pubchem_99
            private readonly int threshold = 684;

            internal AllUpToLength(int length)
            {
                this.predefinedLength = length;
            }

            /// <inheritdoc/>
            public Cycles Find(IAtomContainer molecule)
            {
                return Find(molecule, molecule.Atoms.Count);
            }

            /// <inheritdoc/>
            public Cycles Find(IAtomContainer molecule, int length)
            {
                return Find(molecule, GraphUtil.ToAdjList(molecule), length);
            }

            /// <inheritdoc/>
            public Cycles Find(IAtomContainer molecule, int[][] graph, int length)
            {
                RingSearch ringSearch = new RingSearch(molecule, graph);

                if (this.predefinedLength < length) length = this.predefinedLength;

                IList<int[]> walks = new List<int[]>(6);

                // all isolated cycles are relevant - all we need to do is walk around
                // the vertices in the subset 'isolated'
                foreach (var isolated in ringSearch.Isolated())
                {
                    if (isolated.Length <= length) walks.Add(GraphUtil.Cycle(graph, isolated));
                }

                // each biconnected component which isn't an isolated cycle is processed
                // separately as a subgraph.
                foreach (var fused in ringSearch.FUsed())
                {

                    // make a subgraph and 'apply' the cycle computation - the walk
                    // (path) is then lifted to the original graph
                    foreach (var cycle in FindInFUsed(GraphUtil.Subgraph(graph, fused), length))
                    {
                        walks.Add(Lift(cycle, fused));
                    }
                }

                return new Cycles(walks.ToArray(), molecule, null);
            }

            /**
             * Find rings in a biconnected component.
             *
             * @param g adjacency list
             * @return
             * @ computation was not feasible
             */
            private int[][] FindInFUsed(int[][] g, int length)
            {
                AllCycles allCycles = new AllCycles(g, Math.Min(g.Length, length), threshold);
                if (!allCycles.Completed)
                    throw new Intractable("A large number of cycles were being generated and the"
                            + " computation was aborted. Please us AllCycles/AllRingsFinder with"
                            + " and specify a larger threshold or an alternative cycle set.");
                return allCycles.GetPaths();
            }
        }

        /**
         * Find cycles using a primary cycle finder, if the computation was
         * intractable fallback to an auxiliary cycle finder.
         */
        private sealed class Fallback : CycleFinder
        {
            private CycleFinder primary, auxiliary;

            /**
        	 * Create a fallback for two cycle finders.
        	 *
        	 * @param primary   the primary cycle finder
        	 * @param auxiliary the auxiliary cycle finder
        	 */
            internal Fallback(CycleFinder primary, CycleFinder auxiliary)
            {
                this.primary = primary;
                this.auxiliary = auxiliary;
            }

            /// <inheritdoc/>

            public Cycles Find(IAtomContainer molecule)
            {
                return Find(molecule, molecule.Atoms.Count);
            }

            /// <inheritdoc/>

            public Cycles Find(IAtomContainer molecule, int length)
            {
                return Find(molecule, GraphUtil.ToAdjList(molecule), length);
            }

            /// <inheritdoc/>

            public Cycles Find(IAtomContainer molecule, int[][] graph, int length)
            {
                try
                {
                    return primary.Find(molecule, graph, length);
                }
                catch (Intractable)
                {
                    // auxiliary may still thrown an exception
                    return auxiliary.Find(molecule, graph, length);
                }
            }
        }

        /**
         * Remove cycles with a chord from an existing set of cycles.
         */
        private sealed class Unchorded : CycleFinder
        {

            private CycleFinder primary;

            /**
        	 * Filter any cycles produced by the {@code primary} cycle finder and
        	 * only allow those without a chord.
        	 *
        	 * @param primary the primary cycle finder
        	 */
            internal Unchorded(CycleFinder primary)
            {
                this.primary = primary;
            }

            /// <inheritdoc/>

            public Cycles Find(IAtomContainer molecule)
            {
                return Find(molecule, molecule.Atoms.Count);
            }

            /// <inheritdoc/>

            public Cycles Find(IAtomContainer molecule, int length)
            {
                return Find(molecule, GraphUtil.ToAdjList(molecule), length);
            }

            /// <inheritdoc/>

            public Cycles Find(IAtomContainer molecule, int[][] graph, int length)
            {

                Cycles inital = primary.Find(molecule, graph, length);

                int[][] filtered = new int[inital.GetNumberOfCycles()][];
                int n = 0;

                foreach (var path in inital.paths) {
                    if (Accept(path, graph)) filtered[n++] = path;
                }

                return new Cycles(Arrays.CopyOf(filtered, n), inital.container, inital.bondMap);
            }

            /**
        	 * The cycle path is accepted if it does not have chord.
        	 *
        	 * @param path  a path
        	 * @param graph the adjacency of atoms
        	 * @return accept the path as unchorded
        	 */
            private bool Accept(int[] path, int[][] graph)
            {
                BitArray vertices = new BitArray(0);

                foreach (var v in path)
                    BitArrays.SetValue(vertices, v, true);
                
                for (int j = 1; j < path.Length; j++)
                {
                    int v = path[j];
                    int prev = path[j - 1];
                    int next = path[(j + 1) % (path.Length - 1)];

                    foreach (var w in graph[v])
                    {
                        // chord found
                        if (w != prev && w != next && BitArrays.GetValue(vertices, w)) return false;
                    }

                }

                return true;
            }
        }
    }
}
