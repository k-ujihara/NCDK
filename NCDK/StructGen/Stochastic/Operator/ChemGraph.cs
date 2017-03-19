/* Copyright (C) 1997-2007  The CDK project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Collections;
using NCDK.Graphs.Matrix;
using NCDK.Maths;
using System.Collections.Generic;

namespace NCDK.StructGen.Stochastic.Operator
{
    // @cdk.module     structgen
    // @cdk.githash
    public class ChemGraph
    {
        /* Number of atoms in this structure */
        protected int dim;
        /* Number of atoms needed to form subgraph */
        public int NumAtoms { get; set; }
        protected double[][] contab;
        /* Number of atoms that have been traversed */
        protected int travIndex;
        /* Flag: true if atom visited during a traversal */
        protected bool[] visited;
        /* Depth first traversal of the graph */
        public IList<int> Subgraph { get; set; }

        public ChemGraph(IAtomContainer chrom)
        {
            dim = chrom.Atoms.Count;
            NumAtoms = (int)(dim / 2);
            //contab = Arrays.CreateJagged<double>(dim, dim); // Maybe CDK's bug
            contab = ConnectionMatrix.GetMatrix(chrom);
        }

        public IList<int> PickDFGraph()
        {
            //depth first search from a randomly selected atom

            travIndex = 0;
            Subgraph = new List<int>();
            visited = new bool[dim];
            for (int atom = 0; atom < dim; atom++)
                visited[atom] = false;
            int seedAtom = RandomNumbersTool.RandomInt(0, dim - 1);
            RecursiveDFT(seedAtom);

            return Subgraph;
        }

        private void RecursiveDFT(int atom)
        {
            if ((travIndex < NumAtoms) && (!visited[atom]))
            {
                Subgraph.Add(atom);
                travIndex++;
                visited[atom] = true;

                //            for (int nextAtom = 0; nextAtom < dim; nextAtom++) //not generalized
                //                if (contab[atom][nextAtom] != 0) RecursiveDFT(nextAtom);
                List<int> adjSet = new List<int>();
                for (int nextAtom = 0; nextAtom < dim; nextAtom++)
                {
                    if ((int)contab[atom][nextAtom] != 0)
                    {
                        adjSet.Add(nextAtom);
                    }
                }
                while (adjSet.Count > 0)
                {
                    int adjIndex = RandomNumbersTool.RandomInt(0, adjSet.Count - 1);
                    RecursiveDFT(adjSet[adjIndex]);
                    adjSet.RemoveAt(adjIndex);
                }
            }
        }

        public IList<int> PickBFGraph()
        {
            //breadth first search from a randomly selected atom

            travIndex = 0;
            Subgraph = new List<int>();
            visited = new bool[dim];
            for (int atom = 0; atom < dim; atom++)
                visited[atom] = false;
            int seedAtom = RandomNumbersTool.RandomInt(0, dim - 1);

            List<int> atomQueue = new List<int>();
            atomQueue.Add(seedAtom);
            visited[seedAtom] = true;

            while (atomQueue.Count != 0 && Subgraph.Count < NumAtoms)
            {
                int foreAtom = atomQueue[0];
                Subgraph.Add(foreAtom);
                atomQueue.RemoveAt(0);
                travIndex++;

                List<int> adjSet = new List<int>();
                for (int nextAtom = 0; nextAtom < dim; nextAtom++)
                {
                    if (((int)contab[foreAtom][nextAtom] != 0) && (!visited[nextAtom]))
                    {
                        adjSet.Add(nextAtom);
                    }
                }
                while (adjSet.Count > 0)
                {
                    int adjIndex = RandomNumbersTool.RandomInt(0, adjSet.Count - 1);
                    atomQueue.Add((int)adjSet[adjIndex]);
                    visited[adjSet[adjIndex]] = true;
                    adjSet.RemoveAt(adjIndex);
                }
            }
            return Subgraph;
        }
    }
}
