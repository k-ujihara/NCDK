/*
 * Copyright (C) 2009-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 *
 * MX Cheminformatics Tools for Java
 *
 * Copyright (c) 2007-2009 Metamolecular, LLC
 *
 * http://metamolecular.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 */
using NCDK.SMSD.Algorithms.Matchers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NCDK.SMSD.Algorithms.VFLib.Builder
{
    /// <summary>
    /// Class for parsing and generating query graph.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
    public class VFQueryBuilder : IQuery
    {
        private IList<INode> nodesList;
        private IList<IEdge> edgesList;
        private IDictionary<INode, IAtom> nodeBondMap;

        /// <summary>
        /// Constructor for VF Query Builder
        /// </summary>
        public VFQueryBuilder()
        {
            nodesList = new List<INode>();
            edgesList = new List<IEdge>();
            nodeBondMap = new Dictionary<INode, IAtom>();
        }

        public IEnumerable<IEdge> Edges()
        {
            return new ReadOnlyCollection<IEdge>(edgesList);
        }

        public IEnumerable<INode> Nodes()
        {
            return new ReadOnlyCollection<INode>(nodesList);
        }

        public INode GetNode(int index)
        {
            return nodesList[index];
        }

        /// <summary>
        /// Return a node for a given atom else return null
        /// </summary>
        /// <param name="atom"></param>
        /// <returns>Node in the graph for a given atom</returns>
        public INode GetNode(IAtom atom)
        {
            foreach (var v in nodeBondMap)
            {
                if (v.Value.Equals(atom))
                {
                    return v.Key;
                }
            }

            return null;
        }

        public IEdge GetEdge(int index)
        {
            return edgesList[index];
        }

        public IEdge GetEdge(INode source, INode target)
        {
            if (source == target)
            {
                return null;
            }

            NodeBuilder sourceImpl = (NodeBuilder)source;

            foreach (var edge in sourceImpl.GetEdges())
            {
                if (edge.Source == target || edge.Target == target)
                {
                    return edge;
                }
            }

            return null;
        }

        /// <summary>
        /// Add and return a node for a query atom
        /// </summary>
        /// <returns>added Node</returns>
        public INode AddNode(VFAtomMatcher matcher, IAtom atom)
        {
            NodeBuilder node = new NodeBuilder(matcher);
            nodesList.Add(node);
            nodeBondMap[node] = atom;
            return node;
        }

        public IAtom GetAtom(INode node)
        {
            return nodeBondMap[node];
        }

        public int CountNodes()
        {
            return nodesList.Count;
        }

        public int CountEdges()
        {
            return edgesList.Count;
        }

        /// <summary>
        /// Construct and return an edge for a given query and target node
        /// </summary>
        /// <returns>connected edges</returns>
        public IEdge Connect(INode source, INode target, VFBondMatcher matcher)
        {
            NodeBuilder sourceImpl = (NodeBuilder)source;
            NodeBuilder targetImpl = (NodeBuilder)target;
            EdgeBuilder edge = new EdgeBuilder(sourceImpl, targetImpl, matcher);

            sourceImpl.AddNeighbor(targetImpl);
            targetImpl.AddNeighbor(sourceImpl);

            sourceImpl.AddEdge(edge);
            targetImpl.AddEdge(edge);

            edgesList.Add(edge);
            return edge;
        }
    }
}
