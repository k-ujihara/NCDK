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
using NCDK.Graphs;

namespace NCDK.Isomorphisms
{
    /// <summary>
    /// A predicate for verifying component level grouping in query/target structure
    /// matching. The grouping is used by SMARTS and is critical to querying
    /// reactions. The grouping specifies that substructures in the query should
    /// match to separate components in the target. The grouping specification is
    /// indicated by an {@code int[]} array of length (|V(query)| + 1). The final
    /// index indicates the maximum component group (in the query). A specification
    /// of '0' indicates there are no grouping restrictions.
    /// </summary>
    /// <example>
    /// <code>
    /// // grouping is actually set by SMARTS parser but this shows how it's stored
    /// query.SetProperty(ComponentGrouping.Key, grouping);
    ///
    /// IAtomContainer query, target;
    /// Pattern        pattern = ...; // create pattern for query
    ///
    /// // filter for mappings which respect component grouping in the query
    /// Iterables.Filter(pattern.MatchAll(target),
    ///                  new ComponentGrouping(query, target));
    /// </code>
    /// </example>
    /// <seealso cref="Pattern"/>
    // @author John May
    // @cdk.module isomorphism
    public sealed class ComponentGrouping : Common.Base.Predicate<int[]>
    {
        /// <summary>
        /// Key indicates where the grouping should be store in the query
        /// properties.
        /// </summary>
        public const string Key = "COMPONENT.GROUPING";

        /// <summary>The required  (query) and the targetComponents of the target.</summary>
        private readonly int[] queryComponents, targetComponents;

        /// <summary>Connected components of the target.</summary>
        private readonly ConnectedComponents cc;

        /// <summary>
        /// Create a predicate to match components for the provided query and target.
        /// The target is converted to an adjacency list (<see cref="GraphUtil.ToAdjList(IAtomContainer)"/> 
        /// ) and the query components extracted
        /// from the property <see cref="Key"/> in the query.
        /// </summary>
        /// <param name="query">query structure</param>
        /// <param name="target">target structure</param>
        public ComponentGrouping(IAtomContainer query, IAtomContainer target)
            : this(query, GraphUtil.ToAdjList(target))
        { }

        /// <summary>
        /// Create a predicate to match components for the provided query and target.
        /// The target is pre-converted to an adjacency list (
        /// <see cref="GraphUtil.ToAdjList(IAtomContainer)"/>) and the query components extracted
        /// from the property <see cref="Key"/> in the query.
        /// </summary>
        /// <param name="query">query structure</param>
        /// <param name="target">target structure</param>
        public ComponentGrouping(IAtomContainer query, int[][] target)
            : this(query.GetProperty<int[]>(Key), query.GetProperty<int[]>(Key) != null ? new ConnectedComponents(target)
                    : null)
        { }

        /// <summary>
        /// Create a predicate to match components for the provided query (grouping)
        /// and target (connected components).
        /// </summary>
        /// <param name="grouping">query grouping</param>
        /// <param name="cc">connected component of the target</param>
        public ComponentGrouping(int[] grouping, ConnectedComponents cc)
        {
            this.queryComponents = grouping;
            this.cc = cc;
            this.targetComponents = cc != null ? cc.Components() : null;
        }

        /// <summary>
        /// Does the mapping respected the component grouping specified by the
        /// query.
        /// </summary>
        /// <param name="mapping">a permutation of the query vertices</param>
        /// <returns>the mapping preserves the specified grouping</returns>
        public bool Apply(int[] mapping)
        {
            // no grouping required
            if (queryComponents == null) return true;

            // bidirectional map of query/target components, last index
            // of query components holds the count
            int[] usedBy = new int[cc.NComponents + 1];
            int[] usedIn = new int[queryComponents[mapping.Length] + 1];

            // verify we don't have any collisions
            for (int v = 0; v < mapping.Length; v++)
            {
                if (queryComponents[v] == 0) continue;

                int w = mapping[v];

                int queryComponent = queryComponents[v];
                int targetComponent = targetComponents[w];

                // is the target component already used by a query component?
                if (usedBy[targetComponent] == 0)
                    usedBy[targetComponent] = queryComponent;
                else if (usedBy[targetComponent] != queryComponent) return false;

                // is the query component already used in a target component?
                if (usedIn[queryComponent] == 0)
                    usedIn[queryComponent] = targetComponent;
                else if (usedIn[queryComponent] != targetComponent) return false;

            }

            return true;
        }
    }
}
