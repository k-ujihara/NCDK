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
using System;
using System.Collections.Generic;
using static NCDK.Common.Base.Preconditions;
using static NCDK.Graphs.GraphUtil;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    /// Computes and stores atom invariants in a single object. The atom invariants
    /// are utilised as additional information for the <see cref="SMARTSAtom"/>s to match.
    /// The values provide additional invariants which are not defined in the <see cref="IAtom"/> 
    /// API and avoids storing multiple properties in a type unsafe map
    /// (<see cref="IChemObject.SetProperty(object, object)"/>). 
    /// </summary>
    /// <remarks>
    /// <para>Depending on the SMARTS
    /// implementation different values for the ring information may be set. The
    /// choice of ring set affects <see cref="RingNumber"/> and <see cref="RingSize"/> .
    /// Some implementations store all ring sizes whilst others (Daylight) store only
    /// the smallest. The <see cref="Degree"/>  also depends on whether hydrogens are
    /// suppressed or represented as explicit atoms. </para>
    /// <para>The <see cref="ConfigureDaylightWithRingInfo(IAtomContainer)"/> and
    /// <see cref="ConfigureDaylightWithoutRingInfo(IAtomContainer)"/>
    /// static utilities create
    /// and set the invariants following the Daylight implementation. The invariants
    /// are set on the <see cref="Key"/>  property of each atom.
    /// </para>
    /// </remarks>
    // @author John May
    // @cdk.module smarts
    internal sealed class SMARTSAtomInvariants
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

        /// <summary>
        /// Internal constructor - simple takes all the values.
        /// </summary>
        /// <param name="valence">the valence value</param>
        /// <param name="ringNumber">number of rings an atom belongs to (variable)</param>
        /// <param name="ringSize">the size of the rings (variable)</param>
        /// <param name="ringConnectivity">the number of connected ring bonds (or atoms)</param>
        /// <param name="degree">the degree of an atom</param>
        /// <param name="connectivity">the number of connections (degree + implicit H count)</param>
        /// <param name="totalHydrogenCount">the total number of hydrogens</param>
        public SMARTSAtomInvariants(IAtomContainer target, int valence, int ringNumber, ICollection<int> ringSize, int ringConnectivity, int degree, int connectivity, int totalHydrogenCount)
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

        /// <summary>
        /// Access the valence of this atom. The valence is matched by the "v&lt;NUMBER&gt;"
        /// SMARTS token. The valence is the total number of bonds formed
        /// by this atom and <b>NOT</b> the number of valence electrons. As such
        /// "[v3]" will match a 3 valent nitrogen and "[v5]" will match a
        /// 5 valent nitrogen. The value is separate from <see cref="IAtomType.Valency"/> 
        /// so it can be cleaned up after matching and avoid confusion with what the
        /// value should be.
        /// </summary>
        /// <returns>the valence of the atom.</returns>
        public int Valence => valence;

        /// <summary>
        /// The number of rings this atom belong to. The value is matched by the
        /// "R&lt;NUMBER&gt;" token and depends on the ring set used. The Daylight
        /// implementation uses the non-unique Smallest Set of Smallest Rings (SSSR)
        /// which can lead to inconsistent matches.
        /// </summary>
        /// <returns>number or rings</returns>
        public int RingNumber => ringNumber;

        /// <summary>
        /// The sizes of rings this atoms belongs to. The value is matched by the
        /// "r&lt;NUMBER&gt;" token and depends on the ring set used. The Daylight
        /// implementation uses this value to match the smallest ring to which this
        /// atom is a member. It may be beneficial to match multiple ring sizes (not
        /// yet defined by OpenSMARTS).
        /// </summary>
        /// <returns>ring sizes</returns>
        public ICollection<int> RingSize => ringSize;

        /// <summary>
        /// The number of connected ring bonds (or atoms). This value is matched by
        /// the "x&lt;NUMBER&gt;" token. The Daylight implementation counts the
        /// number of connected ring bonds but it may be beneficial to match the atom
        /// ring connectivity (not yet defined by OpenSMARTS).
        /// </summary>
        /// <returns>ring connectivity</returns>
        public int RingConnectivity => ringConnectivity;

        /// <summary>
        /// The number of connected bonds including those to hydrogens. This value is
        /// matched by the "X&lt;NUMBER&gt;" token. This value depends on whether the
        /// hydrogens have been suppressed or are represented as explicit atoms.
        /// </summary>
        /// <returns>connectivity</returns>
        public int Connectivity => connectivity;

        /// <summary>
        /// The degree of a vertex defined as the number of explicit connected bonds.
        /// This value is matched by the "D&lt;NUMBER&gt;" token. This value depends
        /// on whether the hydrogens have been suppressed or are represented as
        /// explicit atoms.
        /// </summary>
        /// <returns>connectivity</returns>
        public int Degree => degree;

        /// <summary>
        /// The total number of hydrogens attached to an atom.
        /// </summary>
        /// <returns></returns>
        public int TotalHydrogenCount => totalHydrogenCount;

        /// <summary>
        /// Computes <see cref="SMARTSAtomInvariants"/> and stores on the <see cref="Key"/> or
        /// each <see cref="IAtom"/> in the <paramref name="container"/>. The <see cref="IMolecularEntity.IsInRing"/>  
        /// is also set for each bond. This configuration does
        /// not include ring information and values are left as unset.
        /// Ring membership is still configured but not ring size.
        /// </summary>
        /// <example>
        /// <code>
        ///     IAtomContainer container = ...;
        ///     SMARTSAtomInvariants.ConfigureDaylightWithoutRingInfo(container);
        ///     foreach (var atom in container.Atoms) {
        ///         SMARTSAtomInvariants inv = atom.GetProperty&lt;SMARTSAtomInvariants&gt;(SMARTSAtomInvariants.Key);
        ///     }
        /// </code>
        /// </example>
        /// <param name="container">the container to configure</param>
        public static void ConfigureDaylightWithoutRingInfo(IAtomContainer container)
        {
            var map = EdgeToBondMap.WithSpaceFor(container);
            var graph = GraphUtil.ToAdjList(container, map);
            ConfigureDaylight(container, graph, map, false);
        }

        /// <summary>
        /// Computes <see cref="SMARTSAtomInvariants"/> and stores on the <see cref="Key"/> or
        /// each <see cref="IAtom"/> in the <paramref name="container"/>. The <see cref="IMolecularEntity.IsInRing"/>  
        /// is also set for each bond. This configuration
        /// includes the ring information as used by the Daylight implementation.
        /// That is the Smallest Set of Smallest Rings (SSSR) is used and only the
        /// smallest ring is stored for the <see cref="RingSize"/> .
        /// </summary>
        /// <example>
        /// <code>
        ///     IAtomContainer container = ...;
        ///     SMARTSAtomInvariants.ConfigureDaylightWithRingInfo(container);
        ///     foreach (var atom in container.Atoms) {
        ///         SMARTSAtomInvariants inv = atom.GetProperty&lt;SMARTSAtomInvariants&gt;(SMARTSAtomInvariants.Key);
        ///     }
        /// </code>
        /// </example>
        /// <param name="container">the container to configure</param>
        public static void ConfigureDaylightWithRingInfo(IAtomContainer container)
        {
            var map = EdgeToBondMap.WithSpaceFor(container);
            var graph = GraphUtil.ToAdjList(container, map);
            ConfigureDaylight(container, graph, map, true);
        }

        /// <summary>
        /// Computes invariants - see <see cref="ConfigureDaylightWithRingInfo(IAtomContainer)"/> 
        /// and <see cref="ConfigureDaylightWithoutRingInfo(IAtomContainer)"/>.
        /// </summary>
        /// <param name="container">the container to configure</param>
        /// <param name="graph">the graph for quick traversal</param>
        /// <param name="bondMap">the bond map for quick bond lookup</param>
        /// <param name="ringInfo">logical condition as whether ring info should be included</param>
        private static void ConfigureDaylight(IAtomContainer container, int[][] graph, EdgeToBondMap bondMap, bool ringInfo)
        {
            var nAtoms = container.Atoms.Count;

            var ringNumber = new int[nAtoms];
            var ringSize = new int[nAtoms];

            Arrays.Fill(ringSize, nAtoms + 1);

            if (ringInfo)
            {
                // non-unique but used by daylight
                foreach (var cycle in Cycles.FindSSSR(container).GetPaths())
                {
                    var size = cycle.Length - 1;
                    for (int i = 1; i < cycle.Length; i++)
                    {
                        var v = cycle[i];
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
                var atom = container.Atoms[v];

                int implHCount = CheckNotNull(atom.ImplicitHydrogenCount, "Implicit hydrogen count was not set.");

                int totalHCount = implHCount;
                int valence = implHCount;
                int degree = 0;
                int ringConnections = 0;

                // traverse bonds
                foreach (var w in graph[v])
                {
                    var bond = bondMap[v, w];
                    var order = bond.Order;

                    if (order.IsUnset())
                        throw new NullReferenceException("Bond order was not set.");

                    valence += order.Numeric();

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

                var inv = new SMARTSAtomInvariants(container, valence, ringNumber[v],
                        ringSize[v] <= nAtoms ? new int[] { ringSize[v] } : Array.Empty<int>(),
                        ringConnections, degree, degree + implHCount, totalHCount);

                // if there was no properties a default size LinkedHashMap is created
                // automatically
                atom.SetProperty(SMARTSAtomInvariants.Key, inv);
            }
        }
    }
}
