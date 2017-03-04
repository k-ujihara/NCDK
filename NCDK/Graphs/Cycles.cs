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
using NCDK.RingSearches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static NCDK.Graphs.GraphUtil;

namespace NCDK.Graphs
{
    /// <summary>
    /// A utility class for storing and computing the cycles of a chemical graph.
    /// Utilities are also provided for converting the cycles to <see cref="IRing"/>s. A
    /// brief description of each cycle set is given below - for a more comprehensive
    /// review please see - {@cdk.cite Berger04}.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><see cref="All()"/> - all simple cycles in the graph, the number of
    /// cycles generated may be very large and may not be feasible for some
    /// molecules, such as, fullerene.</item> <item><see cref="MCB"/> (aka. SSSR) - minimum
    /// cycle basis (MCB) of a graph, these cycles are linearly independent and can
    /// be used to generate all of cycles in the graph. It is important to note the
    /// MCB is not unique and a that there may be multiple equally valid MCBs. The
    /// smallest set of smallest rings (SSSR) is often used to refer to the MCB but
    /// originally SSSR was defined as a strictly fundamental cycle basis {@cdk.cite
    /// Berger04}. Not every graph has a strictly fundamental cycle basis the
    /// definition has come to mean the MCB. Due to the non-uniqueness of the
    /// MCB/SSSR its use is discouraged.</item> <item><see cref="Relevant"/> - relevant
    /// cycles of a graph, the smallest set of uniquely defined short cycles. If a
    /// graph has a single MCB then the relevant cycles and MCB are the same. If the
    /// graph has multiple MCB then the relevant cycles is the union of all MCBs. The
    /// number of relevant cycles may be exponential but it is possible to determine
    /// how many relevant cycles there are in polynomial time without generating
    /// them. For chemical graphs the number of relevant cycles is usually within
    /// manageable bounds. </item> <item><see cref="Essential"/> - essential cycles of a
    /// graph. Similar to the relevant cycles the set is unique for a graph. If a
    /// graph has a single MCB then the essential cycles and MCB are the same. If the
    /// graph has multiple MCB then the essential cycles is the intersect of all
    /// MCBs. That is the cycles which appear in every MCB. This means that is is
    /// possible to have no essential cycles in a molecule which clearly has cycles
    /// (e.g. bridged system like bicyclo[2.2.2]octane). </item> <item> {@link
    /// #TripletShort} - the triple short cycles are the shortest cycle through
    /// each triple of vertices. This allows one to generate the envelope rings of
    /// some molecules (e.g. naphthalene) without generating all cycles. The cycles
    /// are primarily useful for the CACTVS Substructure Keys (PubChem fingerprint).
    /// </item> <item> <see cref="VertexShort"/> - the shortest cycles through each vertex.
    /// Unlike the MCB, linear independence is not checked and it may not be possible
    /// to generate all other cycles from this set. In practice the vertex/edge short
    /// cycles are similar to MCB. </item> <item> <see cref="EdgeShort"/> - the shortest
    /// cycles through each edge. Unlike the MCB, linear independence is not checked
    /// and it may not be possible to generate all other cycles from this set. In
    /// practice the vertex/edge short cycles are similar to MCB. </item>
    /// </list>
    /// </remarks>
    // @author John May
    // @cdk.module core
    // @cdk.githash
    public sealed class Cycles
    {
        /// <summary>Vertex paths for each cycle.</summary>
        private readonly int[][] paths;

        /// <summary>The input container - allows us to create 'Ring' objects.</summary>
        private readonly IAtomContainer container;

        /// <summary>Mapping for quick lookup of bond mapping.</summary>
        private readonly EdgeToBondMap bondMap;

        /// <summary>
        /// Internal constructor - may change in future but currently just takes the
        /// cycle paths and the container from which they came.
        /// </summary>
        /// <param name="paths">the cycle paths (closed vertex walks)</param>
        /// <param name="container">the input container</param>
        private Cycles(int[][] paths, IAtomContainer container, EdgeToBondMap bondMap)
        {
            this.paths = paths;
            this.container = container;
            this.bondMap = bondMap;
        }

        /// <summary>
        /// How many cycles are stored.
        /// </summary>
        /// <returns>number of cycles</returns>
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

        /// <summary>
        /// Convert the cycles to a <see cref="IRingSet"/> containing the <see cref="IAtom"/>s
        /// and <see cref="IBond"/>s of the input molecule.
        /// </summary>
        /// <returns>ringset for the cycles</returns>
        public IRingSet ToRingSet()
        {
            return ToRingSet(container, paths, bondMap);
        }

        /// <summary>
        /// A cycle finder which will compute all simple cycles in a molecule.
        /// The threshold values can not be tuned and is set at a value which will
        /// complete in reasonable time for most molecules. To change the threshold
        /// values please use the stand-alone <see cref="AllCycles"/> or <see cref="AllRingsFinder"/>.
        /// All cycles is every
        /// possible simple cycle (i.e. non-repeating vertices) in the chemical
        /// graph. As an example - all simple cycles of anthracene includes, 3 cycles
        /// of length 6, 2 of length 10 and 1 of length 14.
        /// </summary>
        /// <example>
        /// <code>
        /// CycleFinder cf = Cycles.All();
        /// foreach (var m in ms) {
        ///     try {
        ///         Cycles   cycles = cf.Find(m);
        ///         IRingSet rings  = cycles.ToRingSet();
        ///     } catch (Intractable e) {
        ///         // handle error - note it is common that finding all simple
        ///         // cycles in chemical graphs is intractable
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>finder for all simple cycles</returns>
        /// <seealso cref="All(IAtomContainer)"/>
        /// <seealso cref="AllCycles"/>
        /// <seealso cref="AllRingsFinder"/>
        public static CycleFinder All()
        {
            return CycleComputation.ALL;
        }

        /// <summary>
        /// All cycles of smaller than or equal to the specified length. If a length
        /// is also provided to <see cref="CycleFinder.Find(IAtomContainer, int)"/> the
        /// minimum of the two limits is used.
        /// </summary>
        /// <param name="length">maximum size or cycle to find</param>
        /// <returns>cycle finder</returns>
        public static CycleFinder All(int length)
        {
            return new AllUpToLength(length);
        }

        /// <summary>
        /// A cycle finder which will compute the minimum cycle basis (MCB) of
        /// a molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// CycleFinder cf = Cycles.MCB;
        /// foreach (var m in ms) {
        ///     try {
        ///         Cycles   cycles = cf.Find(m);
        ///         IRingSet rings  = cycles.ToRingSet();
        ///     } catch (Intractable e) {
        ///         // ignore error - MCB should never be intractable
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>finder for all simple cycles</returns>
        /// <seealso cref="FindMCB(IAtomContainer)"/>
        /// <seealso cref="MinimumCycleBasis"/>
        public static CycleFinder MCB => CycleComputation.MCB;

        /// <summary>
        /// A cycle finder which will compute the relevant cycle basis (RC) of a molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// CycleFinder cf = Cycles.Relevant;
        /// foreach (var m in ms) {
        ///     try {
        ///         Cycles   cycles = cf.Find(m);
        ///         IRingSet rings  = cycles.ToRingSet();
        ///     } catch (Intractable e) {
        ///         // ignore error - there may be an exponential number of cycles
        ///         // but this is not currently checked
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="FindRelevant(IAtomContainer)"/>
        /// <seealso cref="RelevantCycles"/>
        public static CycleFinder Relevant => CycleComputation.RELEVANT;

        /// <summary>
        /// A cycle finder which will compute the essential cycles of a molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// CycleFinder cf = Cycles.Essential;
        /// foreach (var m in ms) {
        ///     try {
        ///         Cycles   cycles = cf.Find(m);
        ///         IRingSet rings  = cycles.ToRingSet();
        ///     } catch (Intractable e) {
        ///         // ignore error - essential cycles do not check tractability
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>finder for essential cycles</returns>
        /// <seealso cref="FindRelevant(IAtomContainer)"/>
        /// <seealso cref="RelevantCycles"/>
        public static CycleFinder Essential => CycleComputation.ESSENTIAL;

        /// <summary>
        /// A cycle finder which will compute the triplet short cycles of a
        /// molecule. These cycles are the shortest through each triplet of vertices
        /// are utilised in the generation of CACTVS Substructure Keys (PubChem
        /// Fingerprint). Currently the triplet cycles are non-canonical (which in
        /// this algorithms case means unique). For finer tuning of options please
        /// use the <see cref="TripletShortCycles"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// CycleFinder cf = Cycles.TripletShort;
        /// foreach (var m in ms) {
        ///     try {
        ///         Cycles   cycles = cf.Find(m);
        ///         IRingSet rings  = cycles.ToRingSet();
        ///     } catch (Intractable e) {
        ///         // ignore error - triple short cycles do not check tractability
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="FindTripletShort(IAtomContainer)"/>
        /// <seealso cref="TripletShortCycles"/>
        public static CycleFinder TripletShort => CycleComputation.TRIPLET_SHORT;

        /// <summary>
        /// Create a cycle finder which will compute the shortest cycles of each
        /// vertex in a molecule. Unlike the SSSR/MCB computation linear independence
        /// is not required and provides some performance gain. In practise typical
        /// chemical graphs are small and the linear independence check is relatively
        /// fast.
        /// </summary>
        /// <example>
        /// <code>
        /// CycleFinder cf = Cycles.VertexShort;
        /// foreach (var m in ms) {
        ///     try {
        ///         Cycles   cycles = cf.Find(m);
        ///         IRingSet rings  = cycles.ToRingSet();
        ///     } catch (Intractable e) {
        ///         // ignore error - vertex short cycles do not check tractability
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>finder for vertex short cycles</returns>
        /// <seealso cref="FindVertexShort(IAtomContainer)"/>
        public static CycleFinder VertexShort => CycleComputation.VERTEX_SHORT;

        /// <summary>
        /// Create a cycle finder which will compute the shortest cycles of each
        /// vertex in a molecule. Unlike the SSSR/MCB computation linear independence
        /// is not required and provides some performance gain. In practise typical
        /// chemical graphs are small and the linear independence check is relatively
        /// fast.
        /// </summary>
        /// <example>
        /// <code>
        /// CycleFinder cf = Cycles.EdgeShort;
        /// foreach (var m in ms) {
        ///     try {
        ///         Cycles   cycles = cf.Find(m);
        ///         IRingSet rings  = cycles.ToRingSet();
        ///     } catch (Intractable e) {
        ///         // ignore error - edge short cycles do not check tractability
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>finder for edge short cycles</returns>
        /// <seealso cref="FindEdgeShort(IAtomContainer)"/>
        public static CycleFinder EdgeShort => CycleComputation.EDGE_SHORT;

        /// <summary>
        /// Create a cycle finder which will compute a set of cycles traditionally
        /// used by the CDK to test for aromaticity. This set of cycles is the
        /// MCB/SSSR and <see cref="all"/> cycles for fused systems with 3 or less rings.
        /// This allows on to test aromaticity of envelope rings in compounds such as
        /// azulene without generating an huge number of cycles for large fused
        /// systems (e.g. fullerenes). The use case was that computation of all
        /// cycles previously took a long time and ring systems with more than 2
        /// rings were too difficult. However it is now more efficient to simply
        /// check all cycles/rings without using the MCB/SSSR. This computation will
        /// fail for complex fused systems but the failure is fast and one can easily
        /// 'fall back' to a smaller set of cycles after catching the exception.
        /// </summary>
        /// <example>
        /// <code>
        /// CycleFinder cf = Cycles.CDKAromaticSet;
        /// foreach (var m in ms) {
        ///     try {
        ///         Cycles   cycles = cf.Find(m);
        ///         IRingSet rings  = cycles.ToRingSet();
        ///     } catch (Intractable e) {
        ///         // ignore error - edge short cycles do not check tractability
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>finder for cdk aromatic cycles</returns>
        /// <seealso cref="FindEdgeShort(IAtomContainer)"/>
        public static CycleFinder CDKAromaticSet => CycleComputation.CDK_AROMATIC;

        /// <summary>
        /// Find all cycles in a fused system or if there were too many cycles
        /// fallback and use the shortest cycles through each vertex. Typically the
        /// types of molecules which the vertex short cycles are provided for are
        /// fullerenes. This cycle finder is well suited to aromaticity.
        /// </summary>
        /// <example>
        /// <code>
        /// CycleFinder cf = Cycles.FindAllOrVertexShort();
        /// foreach (var m in ms) {
        ///     try {
        ///         Cycles   cycles = cf.Find(m);
        ///         IRingSet rings  = cycles.ToRingSet();
        ///     } catch (Intractable e) {
        ///         // ignore error - edge short cycles do not check tractability
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>a cycle finder which computes all cycles if possible or provides the vertex short cycles</returns>
        [Obsolete("use " + nameof(Or) +" to define a custom fall - back)")]
        public static CycleFinder FindAllOrVertexShort()
        {
            return Or(All(), VertexShort);
        }

        /// <summary>
        /// Find and mark all cyclic atoms and bonds in the provided molecule.
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <seealso cref="IMolecularEntity.IsInRing"/>
        public static void MarkRingAtomsAndBonds(IAtomContainer mol)
        {
            EdgeToBondMap bonds = EdgeToBondMap.WithSpaceFor(mol);
            MarkRingAtomsAndBonds(mol, GraphUtil.ToAdjList(mol, bonds), bonds);
        }

        /// <summary>
        /// Find and mark all cyclic atoms and bonds in the provided molecule. This optimised version
        /// allows the caller to optionally provided indexed fast access structure which would otherwise
        /// be created.
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <seealso cref="IMolecularEntity.IsInRing"/>
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

        /// <summary>
        /// Use an auxiliary cycle finder if the primary method was intractable.
        /// </summary>
        /// <example>
        /// <code>
        /// // all cycles or all cycles size &lt;= 6
        /// CycleFinder cf = Cycles.Or(Cycles.All(), Cycles.All(6));
        /// </code>
        /// It is possible to nest multiple levels.
        /// <code>
        /// // all cycles or relevant or essential
        /// CycleFinder cf = Cycles.Or(Cycles.All(), Cycles.Or(Cycles.Relevant, Cycles.Essential));
        /// </code>
        /// </example>
        /// <param name="primary">primary cycle finding method</param>
        /// <param name="auxiliary">auxiliary cycle finding method if the primary failed</param>
        /// <returns>a new cycle finder</returns>
        public static CycleFinder Or(CycleFinder primary, CycleFinder auxiliary)
        {
            return new Fallback(primary, auxiliary);
        }

        /// <summary>
        /// Find all simple cycles in a molecule. The threshold values can not be
        /// tuned and is set at a value which will complete in reasonable time for
        /// most molecules. To change the threshold values please use the stand-alone
        /// <see cref="AllCycles"/> or {@link org.openscience.cdk.ringsearch.AllRingsFinder}.
        /// All cycles is every possible simple cycle (i.e. non-repeating vertices)
        /// in the chemical graph. As an example - all simple cycles of anthracene
        /// includes, 3 cycles of length 6, 2 of length 10 and 1 of length 14.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var m in ms) {
        ///     try {
        ///         Cycles   cycles = Cycles.All(m);
        ///         IRingSet rings  = cycles.ToRingSet();
        ///     } catch (Intractable e) {
        ///         // handle error - note it is common that finding all simple
        ///         // cycles in chemical graphs is intractable
        ///     }
        /// }
        /// </code>
        /// </example>
        ///
        /// <returns>all simple cycles</returns>
        /// <exception cref="">the algorithm reached a limit which caused it to abort in reasonable time</exception>
        /// <seealso cref="All()"/>
        /// <seealso cref="AllCycles"/>
        /// <seealso cref="AllRingsFinder"/>
        public static Cycles All(IAtomContainer container)
        {
            return All().Find(container, container.Atoms.Count);
        }

        /// <summary>
        /// All cycles of smaller than or equal to the specified length.
        /// </summary>
        /// <param name="container">input container</param>
        /// <param name="length">maximum size or cycle to find</param>
        /// <returns>all cycles</returns>
        /// <exception cref="">computation was not feasible</exception>
        public static Cycles All(IAtomContainer container, int length)
        {
            return All().Find(container, length);
        }

        /// <summary>
        /// Find the minimum cycle basis (MCB) of a molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var m in ms) {
        ///     Cycles   cycles = Cycles.FindMCB(m);
        ///     IRingSet rings  = cycles.ToRingSet();
        /// }
        /// </code>
        /// </example>
        /// <returns>cycles belonging to the minimum cycle basis</returns>
        /// <seealso cref="MCB"/>
        /// <seealso cref="MinimumCycleBasis"/>
        public static Cycles FindMCB(IAtomContainer container)
        {
            return _invoke(MCB, container);
        }

        /// <summary>
        /// Find the smallest set of smallest rings (SSSR) - aka minimum cycle basis
        /// (MCB) of a molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var m in ms) {
        ///     Cycles   cycles = Cycles.SSSR(m);
        ///     IRingSet rings  = cycles.ToRingSet();
        /// }
        /// </code>
        /// </example>
        /// <returns>cycles belonging to the minimum cycle basis</returns>
        /// <seealso cref="MCB"/>
        /// <seealso cref="FindMCB(IAtomContainer)"/>
        /// <seealso cref="MinimumCycleBasis"/>
        public static Cycles SSSR(IAtomContainer container)
        {
            return FindMCB(container);
        }

        /// <summary>
        /// Find the relevant cycles of a molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var m in ms) {
        ///     Cycles   cycles = Cycles.FindRelevant(m);
        ///     IRingSet rings  = cycles.ToRingSet();
        /// }
        /// </code>
        /// </example>
        /// <returns>relevant cycles</returns>
        /// <seealso cref="Relevant"/>
        /// <seealso cref="RelevantCycles"/>
        public static Cycles FindRelevant(IAtomContainer container)
        {
            return _invoke(Relevant, container);
        }

        /// <summary>
        /// Find the essential cycles of a molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var m in ms) {
        ///     Cycles   cycles = Cycles.FindEssential(m);
        ///     IRingSet rings  = cycles.ToRingSet();
        /// }
        /// </code>
        /// </example>
        /// <returns>essential cycles</returns>
        /// <seealso cref="Relevant"/>
        /// <seealso cref="RelevantCycles"/>
        public static Cycles FindEssential(IAtomContainer container)
        {
            return _invoke(Essential, container);
        }

        /// <summary>
        /// Find the triplet short cycles of a molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var m in ms) {
        ///     Cycles   cycles = Cycles.FindTripletShort(m);
        ///     IRingSet rings  = cycles.ToRingSet();
        /// }
        /// </code>
        /// </example>
        /// <returns>triplet short cycles</returns>
        /// <seealso cref="TripletShort"/>
        /// <seealso cref="TripletShortCycles"/>
        public static Cycles FindTripletShort(IAtomContainer container)
        {
            return _invoke(TripletShort, container);
        }

        /// <summary>
        /// Find the vertex short cycles of a molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var m in ms) {
        ///     Cycles   cycles = Cycles.FindVertexShort(m);
        ///     IRingSet rings  = cycles.ToRingSet();
        /// }
        /// </code>
        /// </example>
        /// <returns>triplet short cycles</returns>
        /// <seealso cref="VertexShort"/>
        /// <seealso cref="VertexShortCycles"/>
        public static Cycles FindVertexShort(IAtomContainer container)
        {
            return _invoke(VertexShort, container);
        }

        /// <summary>
        /// Find the edge short cycles of a molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var m in ms) {
        ///     Cycles   cycles = Cycles.FindEdgeShort(m);
        ///     IRingSet rings  = cycles.ToRingSet();
        /// }
        /// </code>
        /// </example>
        /// <returns>edge short cycles</returns>
        /// <seealso cref="EdgeShort"/>
        /// <seealso cref="EdgeShortCycles"/>
        public static Cycles FindEdgeShort(IAtomContainer container)
        {
            return _invoke(EdgeShort, container);
        }

        /// <summary>
        /// Derive a new cycle finder that only provides cycles without a chord.
        /// </summary>
        /// <param name="original">find the initial cycles before filtering</param>
        /// <returns>cycles or the original without chords</returns>
        public static CycleFinder GetUnchorded(CycleFinder original)
        {
            return new Unchorded(original);
        }

        /// <summary>
        /// Internal method to wrap cycle computations which <i>should</i> be
        /// tractable. That is they currently won't throw the exception - if the
        /// method does throw an exception an internal error is triggered as a sanity
        /// check.
        /// </summary>
        /// <param name="finder">the cycle finding method</param>
        /// <param name="container">the molecule to find the cycles of</param>
        /// <returns>the cycles of the molecule</returns>
        private static Cycles _invoke(CycleFinder finder, IAtomContainer container)
        {
            return _invoke(finder, container, container.Atoms.Count);
        }

        /// <summary>
        /// Internal method to wrap cycle computations which <i>should</i> be
        /// tractable. That is they currently won't throw the exception - if the
        /// method does throw an exception an internal error is triggered as a sanity
        /// check.
        /// </summary>
        /// <param name="finder">the cycle finding method</param>
        /// <param name="container">the molecule to find the cycles of</param>
        /// <param name="length">maximum size or cycle to find</param>
        /// <returns>the cycles of the molecule</returns>
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

            /// <summary>
            /// Apply cycle perception to the graph (g) - graph is expeced to be
            /// biconnected.
            /// </summary>
            /// <param name="graph">the graph (adjacency list)</param>
            /// <returns>the cycles of the graph</returns>
            /// <exception cref="">the computation reached a set limit</exception>
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
                foreach (var fused in ringSearch.Fused())
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
                foreach (var fused in ringSearch.Fused())
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

        /// <summary>
        /// Internal - lifts a path in a subgraph back to the original graph.
        /// </summary>
        /// <param name="path">a path</param>
        /// <param name="mapping">vertex mapping</param>
        /// <returns>the lifted path</returns>
        private static int[] Lift(int[] path, int[] mapping)
        {
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = mapping[path[i]];
            }
            return path;
        }

        /// <summary>
        /// Internal - convert a set of cycles to an ring set.
        /// </summary>
        /// <param name="container">molecule</param>
        /// <param name="cycles">a cycle of the chemical graph</param>
        /// <param name="bondMap">mapping of the edges (int,int) to the bonds of the container</param>
        /// <returns>the ring set</returns>
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

        /// <summary>
        /// Internal - convert a set of cycles to a ring.
        /// </summary>
        /// <param name="container">molecule</param>
        /// <param name="cycle">a cycle of the chemical graph</param>
        /// <param name="bondMap">mapping of the edges (int,int) to the bonds of the container</param>
        /// <returns>the ring for the specified cycle</returns>
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

        /// <summary>
        /// Obtain the bond between the atoms at index 'u' and 'v'. If the 'bondMap'
        /// is non-null it is used for direct lookup otherwise the slower linear
        /// lookup in 'container' is used.
        /// </summary>
        /// <param name="container">a structure</param>
        /// <param name="bondMap">optimised map of atom indices to bond instances</param>
        /// <param name="u">an atom index</param>
        /// <param name="v">an atom index (connected to u)</param>
        /// <returns>the bond between u and v</returns>
        private static IBond GetBond(IAtomContainer container, EdgeToBondMap bondMap, int u, int v)
        {
            if (bondMap != null) return bondMap[u, v];
            return container.GetBond(container.Atoms[u], container.Atoms[v]);
        }

        /// <summary>
        /// All cycles smaller than or equal to a specified length.
        /// </summary>
        private sealed class AllUpToLength
            : CycleFinder
        {
            private readonly int predefinedLength;

            /// <summary>
            /// See <see cref="AllRingsFinder.Threshold.PubChem_99"/>.
            /// </summary>
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
                foreach (var fused in ringSearch.Fused())
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

            /// <summary>
            /// Find rings in a biconnected component.
            /// </summary>
            /// <param name="g">adjacency list</param>
            /// <returns></returns>
            /// <exception cref="">computation was not feasible</exception>
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

        /// <summary>
        /// Find cycles using a primary cycle finder, if the computation was
        /// intractable fallback to an auxiliary cycle finder.
        /// </summary>
        private sealed class Fallback : CycleFinder
        {
            private CycleFinder primary, auxiliary;

            /// <summary>
            /// Create a fallback for two cycle finders.
            /// </summary>
            /// <param name="primary">the primary cycle finder</param>
            /// <param name="auxiliary">the auxiliary cycle finder</param>
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

        /// <summary>
        /// Remove cycles with a chord from an existing set of cycles.
        /// </summary>
        private sealed class Unchorded : CycleFinder
        {
            private CycleFinder primary;

            /// <summary>
            /// Filter any cycles produced by the {@code primary} cycle finder and
            /// only allow those without a chord.
            /// </summary>
            /// <param name="primary">the primary cycle finder</param>
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

            /// <summary>
            /// The cycle path is accepted if it does not have chord.
            /// </summary>
            /// <param name="path">a path</param>
            /// <param name="graph">the adjacency of atoms</param>
            /// <returns>accept the path as unchorded</returns>
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
