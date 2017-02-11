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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Collections;
using NCDK.Graphs;
using NCDK.RingSearches;
using System.Collections.Generic;
using static NCDK.Graphs.GraphUtil;
using static NCDK.Common.Base.Preconditions;
using System;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /**
     * Computes and stores atom invariants in a single object. The atom invariants
     * are utilised as additional information for the {@link SMARTSAtom}s to match.
     * The values provide additional invariants which are not defined in the {@link
     * IAtom} API and avoids storing multiple properties in a type unsafe map
     * ({@link IAtom#SetProperty(Object, Object)}). <p/> Depending on the SMARTS
     * implementation different values for the ring information may be set. The
     * choice of ring set affects {@link #RingNumber} and {@link #RingSize}.
     * Some implementations store all ring sizes whilst others (Daylight) store only
     * the smallest. The {@link #Degree} also depends on whether hydrogens are
     * suppressed or represented as explicit atoms. <p/> The {@link
     * #ConfigureDaylightWithRingInfo(IAtomContainer)} and {@link
     * #ConfigureDaylightWithoutRingInfo(IAtomContainer)} static utilities create
     * and set the invariants following the Daylight implementation. The invariants
     * are set on the {@link #Key} property of each atom.
     *
     * @author John May
     * @cdk.module smarts
     */
#if TEST
    public
#endif
    sealed class SMARTSAtomInvariants
    {
        /// <summary>Property key to index the class by.</summary>
        public const string Key = "SMARTS.INVARIANTS";

        /// <summary>the molecule which this atom belongs.</summary>
        private readonly IAtomContainer target;

        /// <summary>Total number of bonds formed - also refereed to as bond order sum.</summary>
        private readonly int valence;

        /// <summary>The number of rings this atom can be found in.</summary>
        private readonly int ringNumber;

        /// <summary>The size of rings an atom is found in.</summary>
        private readonly ICollection<int> ringSize;

        /// <summary>Total number of connected atoms including implicit hydrogens.</summary>
        private readonly int connectivity;

        /// <summary>Total number of connected ring bonds.</summary>
        private readonly int ringConnectivity;

        /// <summary>Total number of explicitly connected atoms.</summary>
        private readonly int degree;

        /// <summary>The total number of hydrogens on an atom.</summary>
        private readonly int totalHydrogenCount;

        /**
         * Internal constructor - simple takes all the values.
         *
         * @param valence            the valence value
         * @param ringNumber         number of rings an atom belongs to (variable)
         * @param ringSize           the size of the rings (variable)
         * @param ringConnectivity   the number of connected ring bonds (or atoms)
         * @param degree             the degree of an atom
         * @param connectivity       the number of connections (degree + implicit H
         *                           count)
         * @param totalHydrogenCount the total number of hydrogens
         */
        public SMARTSAtomInvariants(IAtomContainer target, int valence, int ringNumber, ICollection<int> ringSize,
                int ringConnectivity, int degree, int connectivity, int totalHydrogenCount)
        {
            this.target = target;
            this.valence = valence;
            this.ringNumber = ringNumber;
            this.ringSize = ringSize;
            this.connectivity = connectivity;
            this.totalHydrogenCount = totalHydrogenCount;
            this.ringConnectivity = ringConnectivity;
            this.degree = degree;
        }

        public IAtomContainer Target => target;

        /**
         * Access the valence of this atom. The valence is matched by the {@code
         * v<NUMBER>} SMARTS token. The valence is the total number of bonds formed
         * by this atom and <b>NOT</b> the number of valence electrons. As such
         * {@code [v3]} will match a 3 valent nitrogen and {@code [v5]} will match a
         * 5 valent nitrogen. The value is separate from {@link IAtom#Valency}
         * so it can be cleaned up after matching and avoid confusion with what the
         * value should be.
         *
         * @return the valence of the atom.
         */
        public int Valence => valence;

        /**
         * The number of rings this atom belong to. The value is matched by the
         * {@code R<NUMBER>} token and depends on the ring set used. The Daylight
         * implementation uses the non-unique Smallest Set of Smallest Rings (SSSR)
         * which can lead to inconsistent matches.
         *
         * @return number or rings
         */
        public int RingNumber => ringNumber;

        /**
         * The sizes of rings this atoms belongs to. The value is matched by the
         * {@code r<NUMBER>} token and depends on the ring set used. The Daylight
         * implementation uses this value to match the smallest ring to which this
         * atom is a member. It may be beneficial to match multiple ring sizes (not
         * yet defined by OpenSMARTS).
         *
         * @return ring sizes
         */
        public ICollection<int> RingSize => ringSize;

        /**
         * The number of connected ring bonds (or atoms). This value is matched by
         * the {@code x<NUMBER>} token. The Daylight implementation counts the
         * number of connected ring bonds but it may be beneficial to match the atom
         * ring connectivity (not yet defined by OpenSMARTS).
         *
         * @return ring connectivity
         */
        public int RingConnectivity => ringConnectivity;

        /**
         * The number of connected bonds including those to hydrogens. This value is
         * matched by the {@code X<NUMBER>} token. This value depends on whether the
         * hydrogens have been suppressed or are represented as explicit atoms.
         *
         * @return connectivity
         */
        public int Connectivity => connectivity;

        /**
         * The degree of a vertex defined as the number of explicit connected bonds.
         * This value is matched by the {@code D<NUMBER>} token. This value depends
         * on whether the hydrogens have been suppressed or are represented as
         * explicit atoms.
         *
         * @return connectivity
         */
        public int Degree => degree;

        /**
         * The total number of hydrogens attached to an atom.
         *
         * @return
         */
        public int TotalHydrogenCount => totalHydrogenCount;

        /**
         * Computes {@link SMARTSAtomInvariants} and stores on the {@link #Key} or
         * each {@link IAtom} in the {@code container}. The {@link
         * CDKConstants#ISINRING} is also set for each bond. This configuration does
         * not include ring information and values are left as unset.
         * Ring membership is still configured but not ring size.
         *
         * <blockquote><pre>
         *     IAtomContainer container = ...;
         *     SMARTSAtomInvariants.ConfigureDaylightWithoutRingInfo(container);
         *     foreach (var atom in container.Atoms) {
         *         SMARTSAtomInvariants inv = atom.GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key);
         *     }
         * </pre></blockquote>
         *
         * @param container the container to configure
         */
        public static void ConfigureDaylightWithoutRingInfo(IAtomContainer container)
        {
            EdgeToBondMap map = EdgeToBondMap.WithSpaceFor(container);
            int[][] graph = GraphUtil.ToAdjList(container, map);
            ConfigureDaylight(container, graph, map, false);
        }

        /**
         * Computes {@link SMARTSAtomInvariants} and stores on the {@link #Key} or
         * each {@link IAtom} in the {@code container}. The {@link
         * CDKConstants#ISINRING} is also set for each bond. This configuration
         * includes the ring information as used by the Daylight implementation.
         * That is the Smallest Set of Smallest Rings (SSSR) is used and only the
         * smallest ring is stored for the {@link #RingSize}.
         *
         * <blockquote><pre>
         *     IAtomContainer container = ...;
         *     SMARTSAtomInvariants.ConfigureDaylightWithRingInfo(container);
         *     foreach (var atom in container.Atoms) {
         *         SMARTSAtomInvariants inv = atom.GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key);
         *
         *     }
         * </pre></blockquote>
         *
         * @param container the container to configure
         */
        public static void ConfigureDaylightWithRingInfo(IAtomContainer container)
        {
            EdgeToBondMap map = EdgeToBondMap.WithSpaceFor(container);
            int[][] graph = GraphUtil.ToAdjList(container, map);
            ConfigureDaylight(container, graph, map, true);
        }

        /**
         * Computes invariants - see {@link #ConfigureDaylightWithRingInfo(IAtomContainer)}
         * and {@link #ConfigureDaylightWithoutRingInfo(IAtomContainer)}.
         *
         * @param container the container to configure
         * @param graph     the graph for quick traversal
         * @param bondMap   the bond map for quick bond lookup
         * @param ringInfo  logical condition as whether ring info should be
         *                  included
         */
        private static void ConfigureDaylight(IAtomContainer container, int[][] graph, EdgeToBondMap bondMap,
                bool ringInfo)
        {

            int nAtoms = container.Atoms.Count;

            int[] ringNumber = new int[nAtoms];
            int[] ringSize = new int[nAtoms];

            Arrays.Fill(ringSize, nAtoms + 1);

            if (ringInfo)
            {
                // non-unique but used by daylight
                foreach (var cycle in Cycles.SSSR(container).GetPaths())
                {
                    int size = cycle.Length - 1;
                    for (int i = 1; i < cycle.Length; i++)
                    {
                        int v = cycle[i];
                        if (size < ringSize[v]) ringSize[v] = size;
                        ringNumber[v]++;
                        bondMap[cycle[i], cycle[i - 1]].IsInRing = true;
                    }
                }
            }
            else
            {
                // ring membership is super cheap
                foreach (var bond in new RingSearch(container, graph).RingFragments().Bonds)
                {
                    bond.IsInRing = true;
                }
            }

            for (int v = 0; v < nAtoms; v++)
            {

                IAtom atom = container.Atoms[v];

                int implHCount = CheckNotNull(atom.ImplicitHydrogenCount, "Implicit hydrogen count was not set.");

                int totalHCount = implHCount;
                int valence = implHCount;
                int degree = 0;
                int ringConnections = 0;

                // traverse bonds
                foreach (var w in graph[v])
                {
                    IBond bond = bondMap[v, w];
                    BondOrder order = bond.Order;

                    if (order.IsUnset)
                        throw new NullReferenceException("Bond order was not set.");

                    valence += order.Numeric;

                    degree++;

                    if (bond.IsInRing)
                    {
                        ringConnections++;
                    }

                    if (container.Atoms[w].AtomicNumber == 1)
                    {
                        totalHCount++;
                    }

                }

                SMARTSAtomInvariants inv = new SMARTSAtomInvariants(container, valence, ringNumber[v],
                        ringSize[v] <= nAtoms ? new int[] { ringSize[v] } : new int[0],
                        ringConnections, degree, degree + implHCount, totalHCount);

                // if there was no properties a default size LinkedHashMap is created
                // automatically
                atom.SetProperty(SMARTSAtomInvariants.Key, inv);
            }
        }
    }
}
