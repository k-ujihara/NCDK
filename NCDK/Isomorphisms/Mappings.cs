/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
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
using NCDK.Isomorphisms.Matchers;
using System.Collections.Generic;
using System.Linq;
using NCDK.Graphs;
using System.Collections.ObjectModel;
using System.Collections;

namespace NCDK.Isomorphisms
{
    /// <summary>
    /// A fluent interface for handling (sub)-graph mappings from a query to a target
    /// structure. The utility allows one to modify the mappings and provides
    /// convenience utilities. <see cref="Mappings"/> are obtained from a (sub)-graph
    /// matching using <see cref="Pattern"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// IAtomContainer query  = ...;
    /// IAtomContainer target = ...;
    ///
    /// Mappings mappings = Pattern.FindSubstructure(query)
    ///                            .MatchAll(target);
    /// </code>
    ///
    /// The primary function is to provide an iterable of matches - each match is
    /// a permutation (mapping) of the query graph indices (atom indices).
    ///
    /// <code>
    /// for (int[] p : mappings) {
    ///     for (int i = 0; i &lt; p.Length; i++)
    ///         // query.Atoms[i] is mapped to target.Atoms[p[i]];
    /// }
    /// </code>
    ///
    /// The matches can be filtered to provide only those that have valid
    /// stereochemistry.
    ///
    /// <code>
    /// for (int[] p : mappings.GetStereochemistry()) {
    ///     // ...
    /// }
    /// </code>
    ///
    /// Unique matches can be obtained for both atoms and bonds.
    ///
    /// <code>
    /// for (int[] p : mappings.GetUniqueAtoms()) {
    ///     // ...
    /// }
    ///
    /// for (int[] p : mappings.GetUniqueBonds()) {
    ///     // ...
    /// }
    /// </code>
    ///
    /// As matches may be lazily generated - iterating over the match twice (as
    /// above) will actually perform two graph matchings. If the mappings are needed
    /// for subsequent use the <see cref="ToArray"/> provides the permutations as a
    /// fixed size array.
    ///
    /// <code>
    /// int[][] ps = mappings.ToArray();
    /// for (int[] p : ps) {
    ///    // ...
    /// }
    /// </code>
    ///
    /// Graphs with a high number of automorphisms can produce many valid matchings.
    /// Operations can be combined such as to limit the number of matches we
    /// retrieve.
    ///
    /// <code>
    /// // first ten matches
    /// for (int[] p : mappings.Limit(10)) {
    ///     // ...
    /// }
    ///
    /// // first 10 unique matches
    /// for (int[] p : mappings.GetUniqueAtoms()
    ///                        .Limit(10)) {
    ///     // ...
    /// }
    ///
    /// // ensure we don't waste memory and only 'fix' up to 100 unique matches
    /// int[][] ps = mappings.GetUniqueAtoms()
    ///                      .Limit(100)
    ///                      .ToArray();
    /// </code>
    ///
    /// There is no restrictions on which operation can be applied and how many times
    /// but the order of operations may change the result.
    ///
    /// <code>
    /// // first 100 unique matches
    /// Mappings m = mappings.GetUniqueAtoms()
    ///                      .Limit(100);
    ///
    /// // unique matches in the first 100 matches
    /// Mappings m = mappings.Limit(100)
    ///                      .GetUniqueAtoms();
    ///
    /// // first 10 unique matches in the first 100 matches
    /// Mappings m = mappings.Limit(100)
    ///                      .GetUniqueAtoms()
    ///                      .Limit(10);
    ///
    /// // number of unique atom matches
    /// int n = mappings.CountUnique();
    ///
    /// // number of unique atom matches with correct stereochemistry
    /// int n = mappings.GetStereochemistry()
    ///                 .CountUnique();
    ///
    /// </code>
    /// </example>
    /// <seealso cref="Pattern"/>
    // @author John May
    // @cdk.module isomorphism
    // @cdk.keyword substructure search
    // @cdk.keyword structure search
    // @cdk.keyword mappings
    // @cdk.keyword matching
    // @cdk.githash
    public sealed class Mappings : IEnumerable<int[]>
    {
        /// <summary>Iterable permutations of the query vertices.</summary>
        private readonly IEnumerable<int[]> iterable;

        /// <summary>Query and target structures.</summary>
        private IAtomContainer query, target;

        /// <summary>
        /// Create a fluent mappings instance for the provided query / target and an
        /// iterable of permutations on the query vertices (specified as indices).
        /// </summary>
        /// <param name="query">the structure to be found</param>
        /// <param name="target">the structure being searched</param>
        /// <param name="iterable">iterable of permutation</param>
        /// <seealso cref="Pattern"/>
        internal Mappings(IAtomContainer query, IAtomContainer target, IEnumerable<int[]> iterable)
        {
            this.query = query;
            this.target = target;
            this.iterable = iterable;
        }

        /// <summary>
        /// Filter the mappings and keep only those which match the provided
        /// predicate (Guava).
        /// </summary>
        /// <example>
        /// <code>
        ///     IAtomContainer query;
        ///     IAtomContainer target;
        ///
        ///     // obtain only the mappings where the first atom in the query is
        ///     // mapped to the first atom in the target
        ///     Mappings mappings = Pattern.FindSubstructure(query)
        ///                                .MatchAll(target)
        ///                                .Filter(new Predicate&lt;int[]&gt;() {
        ///                                    public bool Apply(int[] input) {
        ///                                        return input[0] == 0;
        ///                                    }});
        /// </code>
        /// </example>
        /// <param name="predicate">a predicate</param>
        /// <returns>fluent-api reference</returns>
        public Mappings Filter(NCDK.Common.Base.Predicate<int[]> predicate)
        {
            return new Mappings(query, target, iterable.Where(n => predicate.Apply(n)));
        }

        /// <summary>
        /// Enumerate the mappings to another type. Each mapping is transformed using the
        /// provided function.
        /// </summary>
        /// <example>
        /// <code>
        /// readonly IAtomContainer query;
        /// readonly IAtomContainer target;
        /// 
        /// Mappings mappings = Pattern.FindSubstructure(query)
        ///                             .MatchAll(target);
        /// // a string that indicates the mapping of atom elements and numbers
        /// IEnumerable&lt;string&gt; strs = mappings.GetMapping(new Function&lt;int[], string&gt;() 
        /// {
        ///   StringBuilder sb = new StringBuilder();
        ///   for (int i = 0; i &lt; input.Length; i++) {
        ///     if (i > 0) sb.Append(", ");
        ///     sb.Append(query.Atoms[i])
        ///       .Append(i + 1)
        ///       .Append(" -> ")
        ///       .Append(target.Atoms[input[i]])
        ///       .Append(input[i] + 1);
        ///    }
        ///    return sb.ToString();
        /// }
        /// </code></example>
        /// <typeparam name="T"></typeparam>
        /// <param name="f">function to transform a mapping</param>
        /// <returns>The transformed types</returns>
        public IEnumerable<T> GetMapping<T>(NCDK.Common.Base.Function<int[], T> f)
        {
            return iterable.Select(n => f.Apply(n));
        }

        /// <summary>
        /// Limit the number of mappings - only this number of mappings will be
        /// generate.
        /// </summary>
        /// <param name="limit">the number of mappings</param>
        /// <returns>fluent-api instance</returns>
        public Mappings Limit(int limit)
        {
            return new Mappings(query, target, iterable.Take(limit));
        }

        /// <summary>
        /// Filter the mappings for those which preserve stereochemistry specified in
        /// the query.
        /// </summary>
        /// <returns>fluent-api instance</returns>
        public Mappings GetStereochemistry()
        {
            // query structures currently have special requirements (i.e. SMARTS)
            if (query is IQueryAtomContainer) return this;
            return Filter(new StereoMatch(query, target));
        }

        /// <summary>
        /// Filter the mappings for those which cover a unique set of atoms in the
        /// target. The unique atom mappings are a subset of the unique bond
        /// matches.
        /// </summary>
        /// <returns>fluent-api instance</returns>
        /// <seealso cref="GetUniqueBonds"/>
        public Mappings GetUniqueAtoms()
        {
            // we need the unique predicate to be reset for each new iterator -
            // otherwise multiple iterations are always filtered (seen before)
            var m = new UniqueAtomMatches();
            return new Mappings(query, target, iterable.Where(n => m.Apply(n)));
        }

        /// <summary>
        /// Filter the mappings for those which cover a unique set of bonds in the
        /// target.
        /// </summary>
        /// <returns>fluent-api instance</returns>
        /// <seealso cref="GetUniqueAtoms"/>
        public Mappings GetUniqueBonds()
        {
            // we need the unique predicate to be reset for each new iterator -
            // otherwise multiple iterations are always filtered (seen before)
            int[][] g = GraphUtil.ToAdjList(query);
            var m = new UniqueBondMatches(g);
            return new Mappings(query, target, iterable.Where(n => m.Apply(n)));
        }

        /// <summary>
        /// Mappings are lazily generated and best used in a loop. However if all
        /// mappings are required this method can provide a fixed size array of
        /// mappings.
        /// </summary>
        /// <example>
        /// <code>
        /// IAtomContainer query  = ...;
        /// IAtomContainer target = ...;
        ///
        /// Pattern pat = Pattern.FindSubstructure(query);
        ///
        /// // lazily iterator
        /// for (int[] mapping : pat.MatchAll(target)) {
        ///     // logic...
        /// }
        ///
        /// int[][] mappings = pat.MatchAll(target)
        ///                       .ToArray();
        ///
        /// // same as lazy iterator but we now can refer to and parse 'mappings'
        /// // to other methods without regenerating the graph match
        /// for (int[] mapping : mappings) {
        ///     // logic...
        /// }
        /// </code>
        ///
        /// The method can be used in combination with other modifiers.
        ///
        /// <code>
        ///
        /// IAtomContainer query  = ...;
        /// IAtomContainer target = ...;
        ///
        /// Pattern pat = Pattern.FindSubstructure(query);
        ///
        /// // array of the first 5 unique atom mappings
        /// int[][] mappings = pat.MatchAll(target)
        ///                       .GetUniqueAtoms()
        ///                       .Limit(5)
        ///                       .ToArray();
        /// </code>
        /// </example>
        /// <returns>array of mappings</returns>
        public int[][] ToArray()
        {
            return iterable.ToArray();
        }

        /// <summary>
        /// Convert the permutations to a atom-atom map.
        /// </summary>
        /// <example>
        /// <code>
        /// for (IDictionary&lt;IAtom,IAtom&gt; map : mappings.ToAtomMap()) {
        ///     for (KeyValuePair&lt;IAtom,IAtom&gt; e : map.EntrySet()) {
        ///         IAtom queryAtom  = e.Key;
        ///         IAtom targetAtom = e.Value;
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>iterable of atom-atom mappings</returns>
        public IEnumerable<IDictionary<IAtom, IAtom>> ToAtomMap()
        {
            return GetMapping(new AtomMaper(query, target));
        }

        /// <summary>
        /// Convert the permutations to a bond-bond map.
        /// </summary>
        /// <example>
        /// <code>
        /// for (IDictionary&lt;IBond,IBond&gt; map : mappings.ToBondMap()) {
        ///     for (KeyValuePair&lt;IBond,IBond&gt; e : map.EntrySet()) {
        ///         IBond queryBond  = e.Key;
        ///         IBond targetBond = e.Value;
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>iterable of bond-bond mappings</returns>
        public IEnumerable<IDictionary<IBond, IBond>> ToBondMap()
        {
            return GetMapping(new BondMaper(query, target));
        }

        /// <summary>
        /// Convert the permutations to an atom-atom bond-bond map.
        /// </summary>
        /// <example><code>
        /// foreach (IDictionary&lt;IChemObject,IChemObject&gt; map in mappings.ToBondMap()) {
        ///    foreach (KeyValuePair&lt;IChemObject,IChemObject&gt; e in map) {
        ///       IChemObject queryObj = e.Key;
        ///       IChemObject targetObj = e.Value;
        ///    }
        ///    IAtom matchedAtom = map[query.Atoms[i]];
        ///    IBond matchedBond = map[query.Bonds[i]];
        /// }
        /// </code></example>
        /// <returns>iterable of atom-atom and bond-bond mappings</returns>
        public IEnumerable<IDictionary<IChemObject, IChemObject>> ToAtomBondMap()
        {
            return GetMapping(new AtomBondMaper(query, target));
        }

        /// <summary>
        /// Obtain the chem objects (atoms and bonds) that have 'hit' in the target molecule.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var obj in mappings.ToChemObjects()) {
        ///   if (obj is IAtom) {
        ///      // this atom was 'hit' by the pattern
        ///   }
        /// }
        /// </code>
        /// </example>
        /// <returns>lazy iterable of chem objects</returns>
        public IEnumerable<IChemObject> ToChemObjects()
        {
            foreach (var a in GetMapping(new AtomBondMaper(query, target)).Select(map => map.Values))
                foreach (var b in a)
                    yield return b;
            yield break;
        }

        /// <summary>
        /// Obtain the mapped substructures (atoms/bonds) of the target compound. The atoms
        /// and bonds are the same as in the target molecule but there may be less of them.
        /// </summary>
        /// <example>
        /// <code>
        /// IAtomContainer query, target
        /// Mappings mappings = ...;
        /// foreach (var mol in mol.ToSubstructures()) {
        ///    foreach (var atom in mol.Atoms)
        ///      target.Contains(atom); // always true
        ///    foreach (var atom in target.Atoms)
        ///      mol.Contains(atom): // not always true
        /// }
        /// </code>
        /// </example>
        /// <returns>lazy iterable of molecules</returns>
        public IEnumerable<IAtomContainer> ToSubstructures()
        {
            return GetMapping(new AtomBondMaper(query, target)).Select(map =>
            {
                IAtomContainer submol = target.Builder.CreateAtomContainer();
                foreach (var atom in query.Atoms)
                    submol.Atoms.Add((IAtom)map[atom]);
                foreach (var bond in query.Bonds)
                    submol.Bonds.Add((IBond)map[bond]);
                return submol;
            });
        }

        /// <summary>
        /// Efficiently determine if there are at least 'n' matches
        /// </summary>
        /// <example>
        /// <code>
        /// Mappings mappings = ...;
        ///
        /// if (mappings.AtLeast(5))
        ///    // set bit flag etc.
        ///
        /// // are the at least 5 unique matches?
        /// if (mappings.GetUniqueAtoms().AtLeast(5))
        ///    // set bit etc.
        /// </code>
        /// </example>
        /// <param name="n">number of matches</param>
        /// <returns>there are at least 'n' matches</returns>
        public bool AtLeast(int n)
        {
            return Limit(n).Count() == n;
        }

        /// <summary>
        /// Obtain the first match - if there is no first match an empty array is
        /// returned.
        /// </summary>
        /// <returns>first match</returns>
        public int[] First()
        {
            var f = iterable.FirstOrDefault();
            return f ?? new int[0];
        }

        /// <summary>
        /// Convenience method to count the number mappings. Note mappings are lazily
        /// generated and checking the count and then iterating over the mappings
        /// currently performs two searches. If the mappings are also needed, it is
        /// more efficient to check the mappings and count manually.
        /// </summary>
        /// <returns>number of matches</returns>
        public int Count()
        {
            return iterable.Count();
        }

        /// <summary>
        /// Convenience method to count the number of unique atom mappings. Note
        /// mappings are lazily generated and checking the count and then iterating
        /// over the mappings currently performs two searches. If the mappings are
        /// also needed, it is more efficient to check the mappings and count
        /// manually.
        ///
        /// The method is simply invokes <c>mappings.GetUniqueAtoms().Count()</c>.
        /// </summary>
        /// <returns>number of matches</returns>
        public int CountUnique()
        {
            return GetUniqueAtoms().Count();
        }

        /// <inheritdoc/>
        public IEnumerator<int[]> GetEnumerator()
        {
            return iterable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Utility to transform a permutation into the atom-atom map.</summary>
        private sealed class AtomMaper : NCDK.Common.Base.Function<int[], IDictionary<IAtom, IAtom>>
        {
            /// <summary>Query/target containers from the graph matching.</summary>
            private readonly IAtomContainer query, target;

            /// <summary>
            /// Use the provided query and target to obtain the atom instances.
            /// </summary>
            /// <param name="query">the structure to be found</param>
            /// <param name="target">the structure being searched</param>
            public AtomMaper(IAtomContainer query, IAtomContainer target)
            {
                this.query = query;
                this.target = target;
            }

            /// <inheritdoc/>
            public IDictionary<IAtom, IAtom> Apply(int[] mapping)
            {
                var map = new Dictionary<IAtom, IAtom>();
                for (int i = 0; i < mapping.Length; i++)
                    map.Add(query.Atoms[i], target.Atoms[mapping[i]]);
                return new ReadOnlyDictionary<IAtom, IAtom>(map);
            }
        }

        /// <summary>Utility to transform a permutation into the bond-bond map.</summary>
        public sealed class BondMaper : NCDK.Common.Base.Function<int[], IDictionary<IBond, IBond>>
        {
            /// <summary>The query graph - indicates a presence of edges.</summary>
            private readonly int[][] g1;

            /// <summary>Bond look ups for the query and target.</summary>
            private readonly GraphUtil.EdgeToBondMap bonds1, bonds2;

            /// <summary>
            /// Use the provided query and target to obtain the bond instances.
            /// </summary>
            /// <param name="query">the structure to be found</param>
            /// <param name="target">the structure being searched</param>
            public BondMaper(IAtomContainer query, IAtomContainer target)
            {
                this.bonds1 = GraphUtil.EdgeToBondMap.WithSpaceFor(query);
                this.bonds2 = GraphUtil.EdgeToBondMap.WithSpaceFor(target);
                this.g1 = GraphUtil.ToAdjList(query, bonds1);
                GraphUtil.ToAdjList(target, bonds2);
            }

            /// <inheritdoc/>
            public IDictionary<IBond, IBond> Apply(int[] mapping)
            {
                var map = new Dictionary<IBond, IBond>();
                for (int u = 0; u < g1.Length; u++)
                {
                    foreach (var v in g1[u])
                    {
                        if (v > u)
                        {
                            map.Add(bonds1[u, v], bonds2[mapping[u], mapping[v]]);
                        }
                    }
                }
                return new ReadOnlyDictionary<IBond, IBond>(map);
            }
        }

        /// <summary>Utility to transform a permutation into an atom-atom and bond-bond map.</summary>
        private sealed class AtomBondMaper : NCDK.Common.Base.Function<int[], IDictionary<IChemObject, IChemObject>>
        {
            /// <summary>The query graph - indicates a presence of edges.</summary>
            private readonly int[][] g1;

            /// <summary>Bond look ups for the query and target.</summary>
            private readonly GraphUtil.EdgeToBondMap bonds1, bonds2;

            private IAtomContainer query;
            private IAtomContainer target;

            /// <summary>
            /// Use the provided query and target to obtain the bond instances.
            /// </summary>
            /// <param name="query">the structure to be found</param>
            /// <param name="target">the structure being searched</param>
            public AtomBondMaper(IAtomContainer query, IAtomContainer target)
            {
                this.query = query;
                this.target = target;
                this.bonds1 = GraphUtil.EdgeToBondMap.WithSpaceFor(query);
                this.bonds2 = GraphUtil.EdgeToBondMap.WithSpaceFor(target);
                this.g1 = GraphUtil.ToAdjList(query, bonds1);
                GraphUtil.ToAdjList(target, bonds2);
            }

            /// <inheritdoc/>
            public IDictionary<IChemObject, IChemObject> Apply(int[] mapping)
            {
                var map = new Dictionary<IChemObject, IChemObject>();
                for (int u = 0; u < g1.Length; u++)
                {
                    map.Add(query.Atoms[u], target.Atoms[mapping[u]]);
                    foreach (var v in g1[u])
                    {
                        if (v > u)
                        {
                            map.Add(bonds1[u, v], bonds2[mapping[u], mapping[v]]);
                        }
                    }
                }
                return new ReadOnlyDictionary<IChemObject, IChemObject>(map);
            }
        }
    }
}
