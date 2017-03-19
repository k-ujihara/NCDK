/* Copyright (C) 2009-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.SMSD.Algorithms.Matchers;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NCDK.SMSD.Algorithms.VFLib.Builder
{
    /// <summary>
    /// Class for building/storing nodes (atoms) in the graph with atom
    /// query capabilities.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public class NodeBuilder : INode
    {

        private List<INode> neighborsList;
        private List<IEdge> edgesList;
        private VFAtomMatcher matcher;

        /// <summary>
        /// Construct a node for a query atom
        /// <param name="matcher"></param>
        /// </summary>
        protected internal NodeBuilder(VFAtomMatcher matcher)
        {
            edgesList = new List<IEdge>();
            neighborsList = new List<INode>();
            this.matcher = matcher;
        }

        public int CountNeighbors() => neighborsList.Count;

        public IEnumerable<INode> Neighbors()
        {
            return new ReadOnlyCollection<INode>(neighborsList);
        }

        public VFAtomMatcher AtomMatcher => matcher;

        public IList<IEdge> GetEdges() => new ReadOnlyCollection<IEdge>(edgesList);

        public void AddEdge(EdgeBuilder edge)
        {
            edgesList.Add(edge);
        }

        public void AddNeighbor(NodeBuilder node)
        {
            neighborsList.Add(node);
        }
    }
}
