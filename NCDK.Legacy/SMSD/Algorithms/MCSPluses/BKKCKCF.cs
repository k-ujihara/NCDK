/*
 *
 * Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 * You should have received index copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Common.Collections;
using NCDK.SMSD.Tools;
using System;
using System.Collections.Generic;

namespace NCDK.SMSD.Algorithms.MCSPluses
{
    /// <summary>
    /// This class implements Bron-Kerbosch clique detection algorithm as it is
    /// described in [F. Cazals, vertexOfCurrentClique. Karande: An Algorithm for reporting maximal c-cliques;
    /// processedVertex.Comp. Sc. (2005); vol 349; pp. 484-490]
    /// 
    /// BronKerboschCazalsKarandeKochCliqueFinder.java
    /// </summary>
    // @cdk.module smsd
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete]
    public class BKKCKCF
    {
        private List<IReadOnlyList<int>> maxCliquesSet = null;

        private IReadOnlyList<int> cEdges = null;
        private IReadOnlyList<int> dEdges = null;
        private int bestCliqueSize = 0;
        private IReadOnlyList<int> compGraphNodes = null;
        private double dEdgeIterationSize = 0;
        private readonly double cEdgeIterationSize = 0;

        /// <summary>
        /// Creates index new instance of Bron Kerbosch Cazals Karande Koch Clique Finder
        /// This class implements Bron-Kerbosch clique detection algorithm as it is
        /// described in [F. Cazals, vertexOfCurrentClique. Karande: An Algorithm for reporting maximal c-cliques;
        /// processedVertex.Comp. Sc. (2005); vol 349; pp.
        /// 484-490]
        /// </summary>
        /// <param name="compGraphNodesOrg"></param>
        /// <param name="cEdgesOrg">C-Edges set of allowed edges</param>
        /// <param name="dEdgesOrg">D-Edges set of prohibited edges</param>
        internal BKKCKCF(IReadOnlyList<int> compGraphNodesOrg, IReadOnlyList<int> cEdgesOrg, IReadOnlyList<int> dEdgesOrg)
        {
            MCSPlus.SetTimeManager(new TimeManager());
            this.compGraphNodes = compGraphNodesOrg;
            this.cEdges = cEdgesOrg;
            this.dEdges = dEdgesOrg;
            bestCliqueSize = 0;

            //Orignal assignment as per paper
            dEdgeIterationSize = dEdges.Count / 2;

            //Orignal assignment as per paper
            cEdgeIterationSize = cEdges.Count / 2;

            //Initialization maxCliquesSet

            maxCliquesSet = new List<IReadOnlyList<int>>();

            Init();
        }

        /// <summary>
        /// Call the wrapper for ENUMERATE_CLIQUES
        /// </summary>
        private void Init()
        {
            // vertex: stored all the vertices for the Graph G vertex[G] nodes of
            // vector compGraphNodes are stored in vertex
            List<int> vertex = new List<int>(); //Initialization of ArrayList vertex

            int vertexCount = compGraphNodes.Count / 3;

            //Console.Out.WriteLine("ArrayList vertex is initialized");
            for (int a = 0; a < vertexCount; a++)
            {
                vertex.Add(compGraphNodes[a * 3 + 2]);
                //Console.Out.Write("vertex[" + index + "]: " + compGraphNodes[index * 3 + 2] + " ");
            }
            //Console.Out.WriteLine();

            vertex.Add(0);
            // Console.Out.WriteLine("ArrayList vertex :" + vertex);

            // processedVertex: is index set of vertices which have already been used
            List<int> processedVertex = new List<int>();

            // Let processedVertex be the set of Nodes already been used in the
            // initialization
            InitIterator(vertex, processedVertex);
            processedVertex.Clear();
            //Console.Out.WriteLine("maxCliquesSet: " + maxCliquesSet);
        }

        private int EnumerateCliques(List<int> vertexOfCurrentClique, Deque<int> potentialCVertex,
                List<int> potentialDVertex, List<int> excludedVertex, List<int> excludedCVertex)
        {
            List<int> potentialVertex = new List<int>();//Defined as potentialCVertex' in the paper
            foreach (var i in potentialCVertex)
            {
                potentialVertex.Add(i);
            }

            if ((potentialCVertex.Count == 1) && (excludedVertex.Count == 0))
            {

                //store best solutions in stack maxCliquesSet
                int cliqueSize = vertexOfCurrentClique.Count;

                if (cliqueSize >= bestCliqueSize)
                {
                    if (cliqueSize > bestCliqueSize)
                    {

                        maxCliquesSet.Clear();
                        bestCliqueSize = cliqueSize;

                    }
                    if (cliqueSize == bestCliqueSize)
                    {
                        //Console.Out.WriteLine("vertexOfCurrentClique-Clique " + vertexOfCurrentClique);
                        maxCliquesSet.Add(vertexOfCurrentClique);
                    }
                }
                return 0;
            }
            FindCliques(potentialVertex, vertexOfCurrentClique, potentialCVertex, potentialDVertex, excludedVertex,
                    excludedCVertex);
            return 0;
        }

        private List<int> FindNeighbors(int centralNode)
        {

            List<int> neighborVertex = new List<int>();

            for (int a = 0; a < cEdgeIterationSize; a++)
            {
                if (cEdges[a * 2 + 0] == centralNode)
                {
                    //          Console.Out.WriteLine( cEdges[index*2+0] + " " + cEdges[index*2+1]);
                    neighborVertex.Add(cEdges[a * 2 + 1]);
                    neighborVertex.Add(1); // 1 means: is connected via C-edge
                }
                else if (cEdges[a * 2 + 1] == centralNode)
                {
                    //           Console.Out.WriteLine(cEdges[index*2+0] + " " + cEdges[index*2+1]);
                    neighborVertex.Add(cEdges[a * 2 + 0]);
                    neighborVertex.Add(1); // 1 means: is connected via C-edge
                }

            }
            for (int a = 0; a < dEdgeIterationSize; a++)
            {
                if (dEdges[a * 2 + 0] == centralNode)
                {
                    //       Console.Out.WriteLine( dEdges[index*2+0] + " " + dEdges[index*2+1]);
                    neighborVertex.Add(dEdges[a * 2 + 1]);
                    neighborVertex.Add(2); // 2 means: is connected via D-edge
                }
                else if (dEdges[a * 2 + 1] == centralNode)
                {
                    //        Console.Out.WriteLine(dEdges[index*2+0] + " " + dEdges[index*2+1]);
                    neighborVertex.Add(dEdges[a * 2 + 0]);
                    neighborVertex.Add(2); // 2 means: is connected via D-edge
                }
            }
            return neighborVertex;
        }

        protected int GetBestCliqueSize()
        {
            return bestCliqueSize;
        }

         internal Deque<IReadOnlyList<int>> GetMaxCliqueSet()
        {
            var solution = new Deque<IReadOnlyList<int>>();
            foreach (var s in maxCliquesSet)
                solution.Push(s);
            return solution;
        }

        private void FindCliques(List<int> potentialVertex, List<int> vertexOfCurrentClique,
                Deque<int> potentialCVertex, List<int> potentialDVertex, List<int> excludedVertex,
                List<int> excludedCVertex)
        {
            int index = 0;
            List<int> neighbourVertex = new List<int>(); ////Initialization ArrayList neighbourVertex

            while (potentialVertex[index] != 0)
            {
                int potentialVertexIndex = potentialVertex[index];

                potentialCVertex.Remove(potentialVertexIndex); 

                List<int> rCopy = new List<int>(vertexOfCurrentClique);
                Deque<int> pCopy = new Deque<int>();
                List<int> qCopy = new List<int>(potentialDVertex);
                List<int> xCopy = new List<int>(excludedVertex);
                List<int> yCopy = new List<int>(excludedCVertex);

                neighbourVertex.Clear();

                foreach (var obj in potentialCVertex)
                {
                    pCopy.Push(obj);
                }

                pCopy.Pop();
                //find the neighbors of the central node from potentialCVertex
                //Console.Out.WriteLine("potentialVertex.ElementAt(index): " + potentialVertex.ElementAt(index));

                neighbourVertex = FindNeighbors(potentialVertexIndex);
                GroupNeighbors(index, pCopy, qCopy, xCopy, yCopy, neighbourVertex, potentialDVertex, potentialVertex,
                        excludedVertex, excludedCVertex);
                Deque<int> pCopyNIntersec = new Deque<int>();
                List<int> qCopyNIntersec = new List<int>();
                List<int> xCopyNIntersec = new List<int>();
                List<int> yCopyNIntersec = new List<int>();

                CopyVertex(neighbourVertex, pCopyNIntersec, pCopy, qCopyNIntersec, qCopy, xCopyNIntersec,
                        xCopy, yCopyNIntersec, yCopy);

                pCopyNIntersec.Push(0);
                rCopy.Add(potentialVertexIndex);
                EnumerateCliques(rCopy, pCopyNIntersec, qCopyNIntersec, xCopyNIntersec, yCopyNIntersec);
                excludedVertex.Add(potentialVertexIndex);
                index++;
            }
        }

        private static void CopyVertex(List<int> neighbourVertex, Deque<int> pCopyNIntersec, Deque<int> pCopy,
                List<int> qCopyNIntersec, List<int> qCopy, List<int> xCopyNIntersec,
                List<int> xCopy, List<int> yCopyNIntersec, List<int> yCopy)
        {
            int nElement = -1;
            int nSize = neighbourVertex.Count;

            for (int sec = 0; sec < nSize; sec += 2)
            {

                nElement = neighbourVertex[sec];

                if (pCopy.Contains(nElement))
                {
                    pCopyNIntersec.Push(nElement);
                }
                if (qCopy.Contains(nElement))
                {
                    qCopyNIntersec.Add(nElement);
                }
                if (xCopy.Contains(nElement))
                {
                    xCopyNIntersec.Add(nElement);
                }
                if (yCopy.Contains(nElement))
                {
                    yCopyNIntersec.Add(nElement);
                }
            }
        }

        private static void GroupNeighbors(int index, Deque<int> pCopy, List<int> qCopy, List<int> xCopy,
                List<int> yCopy, List<int> neighbourVertex, List<int> potentialDVertex,
                List<int> potentialVertex, List<int> excludedVertex, List<int> excludedCVertex)
        {

            int nSize = neighbourVertex.Count;

            //Console.Out.WriteLine("Neighbors: ");

            for (int b = 0; b < nSize; b += 2)
            {
                // neighbourVertex[index] is node v
                //Grouping of the neighbors:

                int nElementAtB = neighbourVertex[b];

                if (neighbourVertex[b + 1] == 1)
                {
                    //u and v are adjacent via index C-edge

                    if (potentialDVertex.Contains(nElementAtB))
                    {

                        pCopy.Push(nElementAtB);
                        //delete neighbourVertex[index] bzw. potentialDVertex[c] from set qCopy, remove C-edges
                        qCopy.Remove(nElementAtB);

                    }
                    if (excludedCVertex.Contains(nElementAtB))
                    {
                        if (excludedVertex.Contains(nElementAtB))
                        {
                            xCopy.Add(nElementAtB);
                        }
                        yCopy.Remove(nElementAtB);
                    }
                }

                //find respective neighbor position in potentialVertex, which is needed for the deletion from potentialVertex

                if (potentialVertex.IndexOf(nElementAtB) <= index && potentialVertex.IndexOf(nElementAtB) > -1)
                {
                    --index;
                }
                potentialVertex.Remove(nElementAtB);
            }
        }

        private void SetEdges()
        {
            bool dEdgeFlag = false;

            if (dEdges.Count > cEdges.Count)
            {
                if (dEdges.Count > 10000000 && cEdges.Count > 100000)
                {
                    dEdgeIterationSize = dEdges.Count * 0.000001;
                    dEdgeFlag = true;
                }
                else if (dEdges.Count > 10000000 && cEdges.Count > 5000)
                {
                    dEdgeIterationSize = dEdges.Count * 0.001;
                    dEdgeFlag = true;
                }

                //        else if (dEdges.Count > 5000000 && dEdges.Count > cEdges.Count) {
                //            dEdgeIterationSize = (float) dEdges.Count * 0.0001;
                //            dEdgeFlag = true;
                //
                //        } else if (dEdges.Count > 100000 && dEdges.Count > cEdges.Count) {
                //            dEdgeIterationSize = (float) dEdges.Count * 0.1;
                //            dEdgeFlag = true;
                //        }

                //        }

                //        else if (dEdges.Count >= 10000 && 500 >= cEdges.Count) {
                //            dEdgeIterationSize = (float) dEdges.Count * 0.1;
                //            dEdgeFlag = true;
                //        }
                //
                //
                //

            }

            if (dEdgeFlag)
            {
                CheckLowestEdgeCount();
            }
        }

        private void InitIterator(List<int> vertex, List<int> processedVertex)
        {
            // vertexOfCurrentClique: set of vertices belonging to the current clique
            List<int> vertexOfCurrentClique = new List<int>();

            // potentialCVertex: is index set of vertices which <index>can</index>
            // be addedto vertexOfCurrentClique, because they are neighbours of
            // vertex u via <i>c-edges</i>
            Deque<int> potentialCVertex = new Deque<int>();
            
            // potentialDVertex: is index set of vertices which
            // <index>cannot</index> be added tovertexOfCurrentClique, because they
            // are neighbours of vertex u via <i>d-edges</i>

            List<int> potentialDVertex = new List<int>();

            // excludedVertex: set of vertices which are not allowed to be added to
            // vertexOfCurrentClique
            List<int> excludedVertex = new List<int>();

            // excludedCVertex: set of vertices which are not allowed to be added to C

            List<int> excludedCVertex = new List<int>();

            // neighbourVertex[u]: set of neighbours of vertex u in Graph G

            List<int> neighbourVertex = new List<int>();

            int index = 0;
            while (vertex[index] != 0)
            {
                int centralNode = vertex[index];
                potentialCVertex.Clear();
                potentialDVertex.Clear();
                excludedVertex.Clear();
                vertexOfCurrentClique.Clear();

                //find the neighbors of the central node from vertex
                neighbourVertex = FindNeighbors(centralNode);

                for (int c = 0; c < neighbourVertex.Count; c = c + 2)
                {
                    // u and v are adjacent via index vertexOfCurrentClique-edge
                    int neighbourVertexOfC = neighbourVertex[c];

                    //find respective neighbor position in potentialCVertex, which is needed for the deletion from vertex
                    //delete neighbor from set vertex

                    if (neighbourVertex[c + 1] == 1)
                    {
                        if (processedVertex.Contains(neighbourVertexOfC))
                        {
                            excludedVertex.Add(neighbourVertexOfC);
                        }
                        else
                        {
                            potentialCVertex.Push(neighbourVertexOfC);
                        }
                    }
                    else if (neighbourVertex[c + 1] == 2)
                    {
                        // u and v are adjacent via index potentialDVertex-edge
                        //Console.Out.WriteLine("u and v are adjacent via index potentialDVertex-edge: " + neighbourVertex.ElementAt(c));

                        if (processedVertex.Contains(neighbourVertexOfC))
                        {
                            excludedCVertex.Add(neighbourVertexOfC);
                        }
                        else
                        {
                            potentialDVertex.Add(neighbourVertexOfC);
                        }
                    }

                    if (vertex.IndexOf(neighbourVertexOfC) <= index && vertex.IndexOf(neighbourVertexOfC) > -1)
                    {
                        --index;
                    }
                    vertex.Remove(neighbourVertexOfC);
                    //Console.Out.WriteLine("Elements Removed from vertex:" + neighbourVertexOfC);
                }

                potentialCVertex.Push(0);
                vertexOfCurrentClique.Add(centralNode);

                EnumerateCliques(vertexOfCurrentClique, potentialCVertex, potentialDVertex, excludedVertex, excludedCVertex);
                //EnumerateCliques(vertexOfCurrentClique, potentialCVertex, potentialDVertex, excludedVertex);
                processedVertex.Add(centralNode);
                index++;
            }
        }

        private void CheckLowestEdgeCount()
        {
            if (dEdgeIterationSize < 1 && cEdges.Count <= 5000)
            {
                dEdgeIterationSize = 2;
            }
            else if (dEdgeIterationSize < 1)
            {
                dEdgeIterationSize = 1;
            }
        }
    }
}
