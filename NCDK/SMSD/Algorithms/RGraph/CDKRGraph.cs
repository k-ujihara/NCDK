/* Copyright (C) 2002-2007  Stephane Werner <mail@ixelis.net>
 *               2007-2010  Syed Asad Rahman <asad@ebi.ac.uk>
 *
 * This code has been kindly provided by Stephane Werner
 * and Thierry Hanser from IXELIS mail@ixelis.net.
 *
 * IXELIS sarl - Semantic Information Systems
 *               17 rue des C?dres 67200 Strasbourg, France
 *               Tel/Fax : +33(0)3 88 27 81 39 Email: mail@ixelis.net
 *
 * CDK Contact: cdk-devel@lists.sf.net
 *
 * This program is free software; you can redistribute maxIterator and/or
 * modify maxIterator under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that maxIterator will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR sourceBitSet PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Collections;
using NCDK.SMSD.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.SMSD.Algorithms.RGraph
{
    /**
     * This class implements the Resolution Graph (CDKRGraph).
     * The CDKRGraph is a graph based representation of the search problem.
     * An CDKRGraph is constructed from the two compared graphs (G1 and G2).
     * Each vertex (node) in the CDKRGraph represents a possible association
     * from an edge in G1 with an edge in G2. Thus two compatible bonds
     * in two molecular graphs are represented by a vertex in the CDKRGraph.
     * Each edge in the CDKRGraph corresponds to a common adjacency relationship
     * between the 2 couple of compatible edges associated to the 2 CDKRGraph nodes
     * forming this edge.
     *
     * <p>Example:
     * <pre>
     *    G1 : C-C=O  and G2 : C-C-C=0
     *         1 2 3           1 2 3 4
     * </pre>
     *
     *  <p>The resulting CDKRGraph(G1,G2) will contain 3 nodes:
     *  <ul>
     *    <li>Node sourceBitSet : association between bond C-C :  1-2 in G1 and 1-2 in G2
     *    <li>Node targetBitSet : association between bond C-C :  1-2 in G1 and 2-3 in G2
     *    <li>Node C : association between bond C=0 :  2-3 in G1 and 3-4 in G2
     *  </ul>
     *  The CDKRGraph will also contain one edge representing the
     *  adjacency between node targetBitSet and C  that is : bonds 1-2 and 2-3 in G1
     *  and bonds 2-3 and 3-4 in G2.
     *
     *  <p>Once the CDKRGraph has been built from the two compared graphs
     *  maxIterator becomes a very interesting tool to perform all kinds of
     *  structural search (isomorphism, substructure search, maximal common
     *  substructure,....).
     *
     *  <p>The  search may be constrained by mandatory elements (e.g. bonds that
     *  have to be present in the mapped common substructures).
     *
     *  <p>Performing a query on an CDKRGraph requires simply to set the constrains
     *  (if any) and to invoke the parsing method (Parse())
     *
     *  <p>The CDKRGraph has been designed to be a generic tool. It may be constructed
     *  from any kind of source graphs, thus maxIterator is not restricted to a chemical
     *  context.
     *
     *  <p>The CDKRGraph model is indendant from the CDK model and the link between
     *  both model is performed by the RTools class. In this way the CDKRGraph
     *  class may be reused in other graph context (conceptual graphs,....)
     *
     *  <p><bitSet>Important note</bitSet>: This implementation of the algorithm has not been
     *                      optimized for speed at this stage. It has been
     *                      written with the goal to clearly retrace the
     *                      principle of the underlined search method. There is
     *                      room for optimization in many ways including the
     *                      the algorithm itself.
     *
     *  <p>This algorithm derives from the algorithm described in
     *  {@cdk.cite HAN90} and modified in the thesis of T. Hanser {@cdk.cite HAN93}.
     *
     * @author      Stephane Werner from IXELIS mail@ixelis.net,
     *              Syed Asad Rahman <asad@ebi.ac.uk> (modified the orignal code)
     * @cdk.created 2002-07-17
     * @cdk.require java1.4+
     * @cdk.module  smsd
     * @cdk.githash
     */
    public class CDKRGraph
    {
        /// <summary>
        /// Constructor for the CDKRGraph object and creates an empty CDKRGraph.
        /// </summary>
        public CDKRGraph()
        {
            this.Graph = new List<CDKRNode>();
            this.Solutions = new List<BitArray>();
            this.GraphBitSet = new BitArray(0);
        }

        private bool CheckTimeOut()
        {
            if (CDKMCS.IsTimeOut())
            {
                IsStop = true;
                return true;
            }
            return false;
        }

        // dimensions of the compared graphs

        /// <summary>
        /// the size of the first of the two compared graphs.
        /// </summary>
        public int FirstGraphSize { get; set; }

        /// <summary>
        /// the size of the second of the two compared graphs.
        /// </summary>
        public int SecondGraphSize { get; set; }

        /// <summary>
        /// Reinitialisation of the TGraph.
        /// </summary>
        public void Clear()
        {
            Graph.Clear();
            GraphBitSet.SetAll(false);
        }

        /// <summary>
        /// an CDKRGraph is a list of CDKRGraph nodes each node keeping track of its neighbors.
        /// </summary>
        public IList<CDKRNode> Graph { get; private set; }

        /**
         *  Adds a new node to the CDKRGraph.
         * @param  newNode  The node to add to the graph
         */
        public void AddNode(CDKRNode newNode)
        {
            Graph.Add(newNode);
            BitArrays.SetValue(GraphBitSet, Graph.Count - 1, true);
        }

        /**
         *  Parsing of the CDKRGraph. This is the main method
         *  to perform a query. Given the constrains sourceBitSet and targetBitSet
         *  defining mandatory elements in G1 and G2 and given
         *  the search options, this method builds an initial set
         *  of starting nodes (targetBitSet) and parses recursively the
         *  CDKRGraph to find a list of solution according to
         *  these parameters.
         *
         * @param  sourceBitSet  constrain on the graph G1
         * @param  targetBitSet  constrain on the graph G2
         * @param  findAllStructure true if we want all results to be generated
         * @param  findAllMap true is we want all possible 'mappings'
         * @param timeManager
         * @throws CDKException
         */
        public void Parse(BitArray sourceBitSet, BitArray targetBitSet, bool findAllStructure, bool findAllMap,
                TimeManager timeManager)
        {
            // initialize the list of solution
            CheckTimeOut();
            // initialize the list of solution
            Solutions.Clear();

            // builds the set of starting nodes
            // according to the constrains
            BitArray bitSet = BuildB(sourceBitSet, targetBitSet);

            // setup options
            SetAllStructure(findAllStructure);
            SetAllMap(findAllMap);

            // parse recursively the CDKRGraph
            ParseRec(new BitArray(bitSet.Count), bitSet, new BitArray(bitSet.Count));
        }

        /**
         *  Parsing of the CDKRGraph. This is the recursive method
         *  to perform a query. The method will recursively
         *  parse the CDKRGraph thru connected nodes and visiting the
         *  CDKRGraph using allowed adjacency relationship.
         *
         * @param  traversed  node already parsed
         * @param  extension  possible extension node (allowed neighbors)
         * @param  forbiden   node forbidden (set of node incompatible with the current solution)
         */
        private void ParseRec(BitArray traversed, BitArray extension, BitArray forbidden)
        {
            BitArray newTraversed = null;
            BitArray newExtension = null;
            BitArray newForbidden = null;
            BitArray potentialNode = null;

            CheckTimeOut();

            // if there is no more extension possible we
            // have reached a potential new solution
            if (BitArrays.IsEmpty(extension))
            {
                Solution(traversed);
            } // carry on with each possible extension
            else
            {
                // calculates the set of nodes that may still
                // be reached at this stage (not forbidden)
                potentialNode = ((BitArray)GraphBitSet.Clone());
                BitArrays.AndNot(potentialNode, forbidden);
                potentialNode.Or(traversed);

                // checks if we must continue the search
                // according to the potential node set
                if (MustContinue(potentialNode))
                {
                    // carry on research and update iteration count
                    NbIteration = NbIteration + 1;

                    // for each node in the set of possible extension (neighbors of
                    // the current partial solution, include the node to the solution
                    // and parse recursively the CDKRGraph with the new context.
                    for (int x = BitArrays.NextSetBit(extension, 0); x >= 0; x = BitArrays.NextSetBit(extension, x + 1))
                    {
#if !DEBUG && !TEST
                        if (IsStop)
                            break;
#endif

                        // evaluates the new set of forbidden nodes
                        // by including the nodes not compatible with the
                        // newly accepted node.
                        newForbidden = (BitArray)forbidden.Clone();
                        newForbidden.Or((Graph[x]).Forbidden);

                        // if maxIterator is the first time we are here then
                        // traversed is empty and we initialize the set of
                        // possible extensions to the extension of the first
                        // accepted node in the solution.
                        if (BitArrays.IsEmpty(traversed))
                        {
                            newExtension = (BitArray)((Graph[x]).Extension.Clone());
                        } // else we simply update the set of solution by
                          // including the neighbors of the newly accepted node
                        else
                        {
                            newExtension = (BitArray)extension.Clone();
                            newExtension.Or((Graph[x]).Extension);
                        }

                        // extension my not contain forbidden nodes
                        BitArrays.AndNot(newExtension, newForbidden);

                        // create the new set of traversed node
                        // (update current partial solution)
                        // and add x to the set of forbidden node
                        // (a node may only appear once in a solution)
                        newTraversed = (BitArray)traversed.Clone();
                        newTraversed.Set(x, true);
                        forbidden.Set(x, true);

                        // parse recursively the CDKRGraph
                        ParseRec(newTraversed, newExtension, newForbidden);
                    }
                }
            }
        }

        /**
         * Checks if a potential solution is a real one
         * (not included in a previous solution)
         *  and add this solution to the solution list
         * in case of success.
         *
         * @param  traversed  new potential solution
         */
        private void Solution(BitArray traversed)
        {
            bool included = false;
            BitArray projG1 = ProjectG1(traversed);
            BitArray projG2 = ProjectG2(traversed);

            // the solution must follows the search constrains
            // (must contain the mandatory elements in G1 an G2)
            if (IsContainedIn(SourceBitSet, projG1) && IsContainedIn(TargetBitSet, projG2))
            {
                // the solution should not be included in a previous solution
                // at the CDKRGraph level. So we check against all previous solution
                // On the other hand if a previous solution is included in the
                // new one, the previous solution is removed.
                var solutionIndexesToRemove = new List<int>();
                for (int ii = 0; ii < Solutions.Count; ii++)
                {
                    if (included)
                        break;
                    var sol = Solutions[ii];

                    CheckTimeOut();
                    if (!BitArrays.AreEqual(sol, traversed))
                    {
                        // if we asked to save all 'mappings' then keep this mapping
                        if (IsFindAllMap && (BitArrays.AreEqual(projG1, ProjectG1(sol)) || BitArrays.AreEqual(projG2, ProjectG2(sol))))
                        {
                            // do nothing
                        } // if the new solution is included mark maxIterator as included
                        else if (IsContainedIn(projG1, ProjectG1(sol)) || IsContainedIn(projG2, ProjectG2(sol)))
                        {
                            included = true;
                        } // if the previous solution is contained in the new one, remove the previous solution
                        else if (IsContainedIn(ProjectG1(sol), projG1) || IsContainedIn(ProjectG2(sol), projG2))
                        {
                            solutionIndexesToRemove.Add(ii);
                        }
                    }
                    else
                    {
                        // solution already exists
                        included = true;
                    }
                }
                solutionIndexesToRemove.Reverse();
                foreach (var i in solutionIndexesToRemove)
                    Solutions.RemoveAt(i);

                if (included == false)
                {
                    // if maxIterator is really a new solution add maxIterator to the
                    // list of current solution
                    Solutions.Add(traversed);
                }

                if (!IsFindAllStructure)
                {
                    // if we need only one solution
                    // stop the search process
                    // (e.g. substructure search)
                    IsStop = true;
                }
            }
        }

        /**
         *  Determine if there are potential solution remaining.
         * @param       potentialNode  set of remaining potential nodes
         * @return      true if maxIterator is worse to continue the search
         */
        private bool MustContinue(BitArray potentialNode)
        {
            bool result = true;
            bool cancel = false;
            BitArray projG1 = ProjectG1(potentialNode);
            BitArray projG2 = ProjectG2(potentialNode);

            // if we reached the maximum number of
            // search iterations than do not continue
            if (MaxIteration != -1 && NbIteration >= MaxIteration)
            {
                return false;
            }

            // if constrains may no more be fulfilled then stop.
            if (!IsContainedIn(SourceBitSet, projG1) || !IsContainedIn(TargetBitSet, projG2))
            {
                return false;
            }

            // check if the solution potential is not included in an already
            // existing solution
            foreach (var sol in Solutions)
            {
                if (cancel)
                    break;

                // if we want every 'mappings' do not stop
                if (IsFindAllMap && (BitArrays.AreEqual(projG1, ProjectG1(sol)) || BitArrays.AreEqual(projG2, ProjectG2(sol))))
                {
                    // do nothing
                } // if maxIterator is not possible to do better than an already existing solution than stop.
                else if (IsContainedIn(projG1, ProjectG1(sol)) || IsContainedIn(projG2, ProjectG2(sol)))
                {
                    result = false;
                    cancel = true;
                }
            }

            return result;
        }

        /**
         *  Builds the initial extension set. This is the
         *  set of node that may be used as seed for the
         *  CDKRGraph parsing. This set depends on the constrains
         *  defined by the user.
         * @param  sourceBitSet  constraint in the graph G1
         * @param  targetBitSet  constraint in the graph G2
         * @return
         */
        private BitArray BuildB(BitArray sourceBitSet, BitArray targetBitSet)
        {
            this.SourceBitSet = sourceBitSet;
            this.TargetBitSet = targetBitSet;

            BitArray bistSet = new BitArray(Graph.Count);
            
            // only nodes that fulfill the initial constrains
            // are allowed in the initial extension set : targetBitSet
            foreach (var rNode in Graph)
            {
                CheckTimeOut();

                if ((sourceBitSet[rNode.RMap.Id1] || BitArrays.IsEmpty(sourceBitSet))
                        && (targetBitSet[rNode.RMap.Id2] || BitArrays.IsEmpty(targetBitSet)))
                {
                    bistSet.Set(Graph.IndexOf(rNode), true);
                }
            }
            return bistSet;
        }

        /**
         *  Converts a CDKRGraph bitset (set of CDKRNode)
         * to a list of CDKRMap that represents the
         * mapping between to substructures in G1 and G2
         * (the projection of the CDKRGraph bitset on G1
         * and G2).
         *
         * @param  set  the BitArray
         * @return      the CDKRMap list
         */
        public IList<CDKRMap> BitSetToRMap(BitArray set)
        {
            List<CDKRMap> rMapList = new List<CDKRMap>();

            for (int x = BitArrays.NextSetBit(set, 0); x >= 0; x = BitArrays.NextSetBit(set, x + 1))
            {
                CDKRNode xNode = Graph[x];
                rMapList.Add(xNode.RMap);
            }
            return rMapList;
        }

        /**
         *  Sets the 'AllStructres' option. If true
         * all possible solutions will be generated. If false
         * the search will stop as soon as a solution is found.
         * (e.g. when we just want to know if a G2 is
         *  a substructure of G1 or not).
         *
         * @param  findAllStructure
         */
        public void SetAllStructure(bool findAllStructure)
        {
            this.IsFindAllStructure = findAllStructure;
        }

        /**
         *  Sets the 'finAllMap' option. If true
         * all possible 'mappings' will be generated. If false
         * the search will keep only one 'mapping' per structure
         * association.
         *
         * @param  findAllMap
         */
        public void SetAllMap(bool findAllMap)
        {
            this.IsFindAllMap = findAllMap;
        }

        /**
         * Sets the maxIteration for the CDKRGraph parsing. If set to -1,
         * then no iteration maximum is taken into account.
         *
         * @param  maxIterator  The new maxIteration value
         */
        public void SetMaxIteration(int maxIterator)
        {
            this.MaxIteration = maxIterator;
        }

        /// <summary>
        /// Returns a string representation of the CDKRGraph.
        /// </summary>
        /// <returns>a string representation of the CDKRGraph.</returns>
        public override string ToString()
        {
            string message = "";
            int jIndex = 0;

            foreach (var rNode in Graph)
            {
                message += "-------------\n" + "CDKRNode " + jIndex + "\n" + rNode.ToString() + "\n";
                jIndex++;
            }
            return message;
        }

        /////////////////////////////////
        // BitArray tools
        /**
         *  Projects a CDKRGraph bitset on the source graph G1.
         * @param  set  CDKRGraph BitArray to project
         * @return      The associate BitArray in G1
         */
        public BitArray ProjectG1(BitArray set)
        {
            BitArray projection = new BitArray(FirstGraphSize);
            CDKRNode xNode = null;

            for (int x = BitArrays.NextSetBit(set, 0); x >= 0; x = BitArrays.NextSetBit(set, x + 1))
            {
                xNode = Graph[x];
                projection.Set(xNode.RMap.Id1, true);
            }
            return projection;
        }

        /**
         *  Projects a CDKRGraph bitset on the source graph G2.
         * @param  set  CDKRGraph BitArray to project
         * @return      The associate BitArray in G2
         */
        public BitArray ProjectG2(BitArray set)
        {
            BitArray projection = new BitArray(SecondGraphSize);
            CDKRNode xNode = null;

            for (int x = BitArrays.NextSetBit(set, 0); x >= 0; x = BitArrays.NextSetBit(set, x + 1))
            {
                xNode = Graph[x];
                projection.Set(xNode.RMap.Id2, true);
            }
            return projection;
        }

        /**
         *  Test if set sourceBitSet is contained in  set targetBitSet.
         * @param  sourceBitSet  a bitSet
         * @param  targetBitSet  a bitSet
         * @return    true if  sourceBitSet is contained in  targetBitSet
         */
        private bool IsContainedIn(BitArray sourceBitSet, BitArray targetBitSet)
        {
            bool result = false;

            if (BitArrays.IsEmpty(sourceBitSet))
            {
                return true;
            }

            BitArray setA = (BitArray)sourceBitSet.Clone();
            setA.And(targetBitSet);

            if (BitArrays.AreEqual(setA, sourceBitSet))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// flag to define if we want to get all possible 'structures'
        /// </summary>
        private bool IsFindAllStructure { get; set; }

        /// <summary>
        /// The solution list
        /// </summary>
        public IList<BitArray> Solutions { get; private set; }

        private BitArray TargetBitSet { get; set; }
        private BitArray SourceBitSet { get; set; }

        /// <summary>
        /// maximal number of iterations before search break
        /// </summary>
        private int MaxIteration { get; set; } = -1;
        /// <summary>
        /// flag to define if we want to get all possible 'mappings'
        /// </summary>
        private bool IsFindAllMap { get; set; }

        /// <summary>
        /// if true is a search has to be stopped
        /// </summary>
        private bool IsStop { get; set; }

        private int NbIteration { get; set; }

        private BitArray GraphBitSet { get; set; }
    }
}
