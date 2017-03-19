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
namespace NCDK.Isomorphisms
{
    /// <summary>
    /// A structural pattern for finding an exact matching in a target compound.
    /// </summary>
    // @author John May
    // @cdk.module isomorphism
    public abstract class Pattern
    {
        /// <summary>
        /// Find a matching of this pattern in the <paramref name="target"/>. If no such order
        /// exist an empty mapping is returned. Depending on the implementation
        /// stereochemistry may be checked (recommended).
        /// </summary>
        /// <example><code>
        /// Pattern        pattern = ...; // create pattern
        /// foreach (var m in ms) {
        ///     int[] mapping = pattern.Match(m);
        ///     if (mapping.Length > 0) {
        ///         // found mapping!
        ///     }
        /// }
        /// </code></example>
        /// <param name="target">the container to search for the pattern in</param>
        /// <returns>the mapping from the pattern to the target or an empty array</returns>
        public abstract int[] Match(IAtomContainer target);

        /// <summary>
        /// Determine if there is a mapping of this pattern in the <paramref name="target"/>.
        /// Depending on the implementation stereochemistry may be checked
        /// (recommended).
        /// </summary>
        /// <example><code>
        /// Pattern        pattern = ...; // create pattern
        /// foreach (var m in ms) {
        ///     if (pattern.Matches(m)) {
        ///         // found mapping!
        ///     }
        /// }
        /// </code></example>
        /// <param name="target">the container to search for the pattern in</param>
        /// <returns>the mapping from the pattern to the target</returns>
        public bool Matches(IAtomContainer target)
        {
            return Match(target).Length > 0;
        }

        /// <summary>
        /// Find all mappings of this pattern in the <paramref name="target"/>. Stereochemistry
        /// should not be checked to allow filtering with <see cref="Mappings.GetStereochemistry"/>. 
        /// </summary>
        /// <example>
        /// <code>
        /// Pattern pattern = Pattern.FindSubstructure(query);
        /// foreach (var m in ms) {
        ///     for (int[] mapping : pattern.MatchAll(m)) {
        ///         // found mapping
        ///     }
        /// }
        /// </code>
        /// Using the fluent interface (see <see cref="Mappings"/>) we can search and
        /// manipulate the mappings. Here's an example of finding the first 5
        /// mappings and creating an array. If the mapper is lazy other states are
        /// simply not explored.
        ///
        /// <code>
        /// // find only the first 5 mappings and store them in an array
        /// Pattern pattern  = Pattern.FindSubstructure(query);
        /// int[][] mappings = pattern.MatchAll(target)
        ///                           .Limit(5)
        ///                           .ToArray();
        /// </code>
        /// </example>
        /// <param name="target">the container to search for the pattern in</param>
        /// <returns>the mapping from the pattern to the target</returns>
        /// <seealso cref="Mappings"/>
        public abstract Mappings MatchAll(IAtomContainer target);

        /// <summary>
        /// Create a pattern which can be used to find molecules which contain the
        /// <paramref name="query"/> structure. The default structure search implementation is
        /// <see cref="VentoFoggia"/>.
        /// </summary>
        /// <param name="query">the substructure to find</param>
        /// <returns>a pattern for finding the <paramref name="query"/></returns>
        /// <seealso cref="VentoFoggia"/>
        public static Pattern FindSubstructure(IAtomContainer query)
        {
            return VentoFoggia.FindSubstructure(query);
        }

        /// <summary>
        /// Create a pattern which can be used to find molecules which are the same
        /// as the <paramref name="query"/> structure. The default structure search
        /// implementation is <see cref="VentoFoggia"/>.
        /// </summary>
        /// <param name="query">the substructure to find</param>
        /// <returns>a pattern for finding the <paramref name="query"/></returns>
        /// <seealso cref="VentoFoggia"/>
        public static Pattern FindIdentical(IAtomContainer query)
        {
            return VentoFoggia.FindSubstructure(query);
        }
    }
}