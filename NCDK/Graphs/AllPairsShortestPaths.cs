/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NCDK.Graphs
{
    /// <summary>
    /// Utility to determine the shortest paths between all pairs of atoms in a molecule.
    /// </summary>
    /// <example>
    /// <cdoe>
    /// IAtomContainer        benzene = MoleculeFactory.MakeBenzene();
    /// AllPairsShortestPaths apsp    = new AllPairsShortestPaths(benzene);
    ///
    /// for (int i = 0; i &lt; benzene.Atoms.Count; i++) {
    ///
    ///     // only to half the comparisons, we can reverse the
    ///     // path[] to get all j to i
    ///     for (int j = i + 1; j &lt; benzene.Atoms.Count; j++) {
    ///
    ///         // reconstruct shortest path from i to j
    ///         int[] path = apsp.From(i).PathTo(j);
    ///
    ///         // reconstruct all shortest paths from i to j
    ///         int[][] paths = apsp.From(i).PathsTo(j);
    ///
    ///         // reconstruct the atoms in the path from i to j
    ///         IAtom[] atoms = apsp.From(i).GetAtomsTo(j);
    ///
    ///         // access the number of paths from i to j
    ///         int nPaths = apsp.From(i).NPathsTo(j);
    ///
    ///         // access the distance from i to j
    ///         int distance = apsp.From(i).NPathsTo(j);
    ///
    ///     }
    /// }
    /// </cdoe>
    /// </example>
    /// <seealso cref="ShortestPaths"/>
    // @author John May
    // @cdk.module core
    // @cdk.githash
    public sealed class AllPairsShortestPaths {
        private readonly IAtomContainer container;
        private readonly ShortestPaths[] shortestPaths;

        /// <summary>
        /// Create a new all shortest paths utility for an <see cref="IAtomContainer"/>.
        /// </summary>
        /// <param name="container">the molecule of which to find the shortest paths</param>
        public AllPairsShortestPaths(IAtomContainer container) {
            // ToAdjList performs null check
            int[][] adjacent = GraphUtil.ToAdjList(container);
            int n = container.Atoms.Count;
            this.container = container;
            this.shortestPaths = new ShortestPaths[n];

            // for each atom construct the ShortestPaths object
            for (int i = 0; i < n; i++)
            {
                shortestPaths[i] = new ShortestPaths(adjacent, container, i);
            }
        }

        /// <summary>
        /// Access the shortest paths object for provided start vertex.
        /// </summary>
        /// <example><code>
        /// AllPairsShortestPaths apsp = ...;
        ///
        /// // access explicitly
        /// ShortestPaths sp = asp.From(0);
        ///
        /// // or chain method calls
        /// int[] path = asp.From(0).PathTo(5);
        /// </code>
        /// </example>
        /// <param name="start">the start vertex of the path</param>
        /// <returns>The shortest paths from the given state vertex</returns>
        /// <seealso cref="ShortestPaths"/>
        public ShortestPaths From(int start) {
            return (start < 0 || start >= shortestPaths.Length) ? EMPTY_SHORTEST_PATHS : shortestPaths[start];
        }

        /// <summary>
        /// Access the shortest paths object for provided start atom.
        /// </summary>
        /// <example><code>
        /// AllPairsShortestPaths apsp = ...;
        /// IAtom start, end = ...;
        ///
        /// // access explicitly
        /// ShortestPaths sp = asp.From(start);
        ///
        /// // or chain the method calls together
        ///
        /// // first path from start to end atom
        /// int[] path = asp.From(start).PathTo(end);
        ///
        /// // first atom path from start to end atom
        /// IAtom[] atoms = asp.From(start).AtomTo(end);
        /// </code></example>
        /// <param name="start">the start atom of the path</param>
        /// <returns>The shortest paths from the given state vertex</returns>
        /// <seealso cref="ShortestPaths"/>
        public ShortestPaths From(IAtom start) {
            // currently container.Atoms.IndexOf() return -1 when null
            return From(container.Atoms.IndexOf(start));
        }

        /// <summary>
        /// an empty atom container so we can handle invalid vertices/atoms better.
        /// Not very pretty but we can't access the domain model from cdk-core.
        /// </summary>
        private static readonly IAtomContainer EMPTY_CONTAINER = new EmptyAtomContainer();

        private class EmptyAtomContainer
            : IAtomContainer
        {
            public IList<IAtom> Atoms => Array.Empty<IAtom>();
            public IList<IBond> Bonds => Array.Empty<IBond>();
            public IChemObjectBuilder Builder
            { get { throw new InvalidOperationException("not supported"); } }

            public ICollection<IChemObjectListener> Listeners { get; } = new ReadOnlyCollection<IChemObjectListener>(new List<IChemObjectListener>());
            public bool Notification { get { return false; } set { } }
            public void NotifyChanged() { }

            public string Id
            {
                get { throw new InvalidOperationException("not supported"); }
                set { }
            }

            public bool Compare(object obj) => this == obj;

            public bool IsEmpty() => true;

            public bool IsPlaced
            {
                get { throw new InvalidOperationException("not supported"); }
                set { }
            }

            public bool IsVisited
            {
                get { throw new InvalidOperationException("not supported"); }
                set { }
            }

            public bool IsSingleOrDouble
            {
                get { throw new InvalidOperationException("not supported"); }
                set { }
            }

            public bool IsAromatic
            {
                get { throw new InvalidOperationException("not supported"); }
                set { }
            }

            public IList<ILonePair> LonePairs => Array.Empty<ILonePair>();
            public IList<ISingleElectron> SingleElectrons => Array.Empty<ISingleElectron>();
            
            public void Add(IAtomContainer atomContainer)
            { throw new InvalidOperationException("not supported"); }

            public void AddElectronContainer(IElectronContainer electronContainer)
            { throw new InvalidOperationException("not supported"); }

            public void AddBond(IAtom atom1, IAtom atom2, BondOrder order)
            { }

            public void AddBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo)
            { }

            public void AddLonePairTo(IAtom atom)
            { }

            public void AddSingleElectronTo(IAtom atom)
            { }

            public object Clone()
            { throw new InvalidOperationException("not supported"); }

            public ICDKObject Clone(CDKObjectMap map)
            { throw new InvalidOperationException("not supported"); }

            public bool Contains(ILonePair lonePair)
                => false;

            public bool Contains(IElectronContainer electronContainer)
                => false;

            public bool Contains(ISingleElectron singleElectron)
                => false;

            public bool Contains(IBond bond)
                => false;

            public bool Contains(IAtom atom)
                => false;

            public IBond GetBond(IAtom atom1, IAtom atom2)
            { throw new InvalidOperationException("not supported"); }

            public double GetBondOrderSum(IAtom atom)
            { throw new InvalidOperationException("not supported"); }

            public IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
            { throw new InvalidOperationException("not supported"); }

            public IEnumerable<IBond> GetConnectedBonds(IAtom atom)
            { throw new InvalidOperationException("not supported"); }

            public IEnumerable<IElectronContainer> GetConnectedElectronContainers(IAtom atom)
            { throw new InvalidOperationException("not supported"); }

            public IEnumerable<ILonePair> GetConnectedLonePairs(IAtom atom)
            { throw new InvalidOperationException("not supported"); }

            public IEnumerable<ISingleElectron> GetConnectedSingleElectrons(IAtom atom)
            { throw new InvalidOperationException("not supported"); }

            public IEnumerable<IElectronContainer> GetElectronContainers()
            { throw new InvalidOperationException("not supported"); }

            public BondOrder GetMaximumBondOrder(IAtom atom)
            { throw new InvalidOperationException("not supported"); }

            public BondOrder GetMinimumBondOrder(IAtom atom)
            { throw new InvalidOperationException("not supported"); }

            public void SetProperties(IEnumerable<KeyValuePair<object, object>> properties) { throw new InvalidOperationException("not supported"); }
            public void AddProperties(IEnumerable<KeyValuePair<object, object>> properties ) { throw new InvalidOperationException("not supported"); }
            public IDictionary<object, object> GetProperties() { throw new InvalidOperationException("not supported"); }
            public T GetProperty<T>(object description, T defautValue)  { throw new InvalidOperationException("not supported"); }
            public T GetProperty<T>(object description) => GetProperty(description, default(T));
            public void RemoveProperty(object description) { }
            public void SetProperty(object key, object value) { throw new InvalidOperationException("not supported"); }

            public IList<IStereoElement> StereoElements => Array.Empty<IStereoElement>();

            public void RemoveElectronContainer(IElectronContainer electronContainer)
            { }

            public IBond RemoveBond(IAtom atom0, IAtom atom1)
            {
                return null;
            }

            public void Remove(IAtomContainer atomContainer)
            { }

            public void RemoveAllBonds()
            { }

            public void RemoveAllElectronContainers()
            { }

            public void RemoveAllElements()
            { }

            public void RemoveAtomAndConnectedElectronContainers(IAtom atom)
            { }

            public void SetStereoElements(IEnumerable<IStereoElement> elements)
            { throw new InvalidOperationException("not supported"); }

            public void OnStateChanged(ChemObjectChangeEventArgs evt)
            {
                NotifyChanged();
            }

            public void SetAtoms(IEnumerable<IAtom> atoms)
            { throw new InvalidOperationException("not supported"); }

            public void SetBonds(IEnumerable<IBond> bonds)
            { throw new InvalidOperationException("not supported"); }
        }

        /// <summary>
        /// pseudo shortest-paths - when an invalid atom is given. this will always
        /// return 0 .Length paths and distances.
        /// </summary>
        private static readonly ShortestPaths  EMPTY_SHORTEST_PATHS = new ShortestPaths(new int[0][], EMPTY_CONTAINER, 0);
    }
}
