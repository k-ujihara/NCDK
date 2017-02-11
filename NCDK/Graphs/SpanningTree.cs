/* Copyright (C) 2001-2007  Nina Jeliazkova
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
using System.Linq;
using System.Collections.Generic;
using NCDK.Common.Collections;

namespace NCDK.Graphs
{
    /**
	 * Spanning tree of a molecule.
	 * Used to discover the number of cyclic bonds in order to prevent the
	 * inefficient AllRingsFinder to run for too long.
	 *
	 * @author      Nina Jeliazkova
	 * @cdk.module  core
	 * @cdk.githash
	 * @cdk.dictref blue-obelisk:graphSpanningTree
	 * @cdk.keyword spanning tree
	 * @cdk.keyword ring finding
	 */
    public class SpanningTree
    {
        private const string ATOM_NUMBER = "ST_ATOMNO";

        private int[] parent = null;
        private int[][] cb = null;       // what is cb??? cyclic bonds?

        protected bool[] bondsInTree;

        private int sptSize = 0;
        private int edrSize = 0;

        private int bondsAcyclicCount = 0, bondsCyclicCount = 0;

        private IAtomContainer molecule = null;
        private int totalEdgeCount = 0, totalVertexCount = 0;
        private bool disconnected;
        private bool identifiedBonds;

        /**
		 * Is the molecule disconnected and has more then one component.
		 *
		 * @return the molecule is disconnected
		 */
        public bool IsDisconnected => disconnected;

        /**
		 * Create a new spanning tree for the provided molecule.
		 *
		 * @param atomContainer molecule to make a spanning tree for.
		 */
        public SpanningTree(IAtomContainer atomContainer)
        {
            identifiedBonds = false;
            BuildSpanningTree(atomContainer);
        }

        private bool FastFind(int vertex1, int vertex2, bool union)
        {
            int i = vertex1;
            while (parent[i] > 0)
                i = parent[i];
            int j = vertex2;
            while (parent[j] > 0)
                j = parent[j];
            int t;
            while (parent[vertex1] > 0)
            {
                t = vertex1;
                vertex1 = parent[vertex1];
                parent[t] = i;
            }
            while (parent[vertex2] > 0)
            {
                t = vertex2;
                vertex2 = parent[vertex2];
                parent[t] = j;
            }
            if (union && (i != j))
            {
                if (parent[j] < parent[i])
                {
                    parent[j] = parent[j] + parent[i] - 1;
                    parent[i] = j;
                }
                else
                {
                    parent[i] = parent[i] + parent[j] - 1;
                    parent[j] = i;
                }
            }
            return (i != j);
        }

        private void FastFindInit(int vertexCount)
        {
            parent = new int[vertexCount + 1];
            for (int i = 1; i <= vertexCount; i++)
            {
                parent[i] = 0;
            }
        }

        /*
		 * Kruskal algorithm
		 */
        private void BuildSpanningTree(IAtomContainer atomContainer)
        {
            disconnected = false;
            molecule = atomContainer;

            totalVertexCount = atomContainer.Atoms.Count;
            totalEdgeCount = atomContainer.Bonds.Count;

            sptSize = 0;
            edrSize = 0;
            FastFindInit(totalVertexCount);
            for (int i = 0; i < totalVertexCount; i++)
            {
                (atomContainer.Atoms[i]).SetProperty(ATOM_NUMBER, (i + 1).ToString());
            }
            IBond bond;
            int vertex1, vertex2;
            bondsInTree = new bool[totalEdgeCount];

            for (int b = 0; b < totalEdgeCount; b++)
            {
                bondsInTree[b] = false;
                bond = atomContainer.Bonds[b];
                vertex1 = int.Parse((bond.Atoms[0]).GetProperty<string>(ATOM_NUMBER));
                vertex2 = int.Parse((bond.Atoms[1]).GetProperty<string>(ATOM_NUMBER));
                //this below is a little bit  slower
                //v1 = atomContainer.Atoms.IndexOf(bond.GetAtomAt(0))+1;
                //v2 = atomContainer.Atoms.IndexOf(bond.GetAtomAt(1))+1;
                if (FastFind(vertex1, vertex2, true))
                {
                    bondsInTree[b] = true;
                    sptSize++;
                    //Debug.WriteLine("ST : includes bond between atoms "+v1+","+v2);
                }
                if (sptSize >= (totalVertexCount - 1)) break;

            }
            // if atomcontainer is connected then the number of bonds in the spanning tree = (No atoms-1)
            //i.e.  edgesRings = new Bond[E-V+1];
            //but to hold all bonds if atomContainer was disconnected then  edgesRings = new Bond[E-sptSize];
            if (sptSize != (totalVertexCount - 1)) disconnected = true;
            for (int b = 0; b < totalEdgeCount; b++)
                if (!bondsInTree[b])
                {
                    //			edgesRings[edrSize] = atomContainer.GetBondAt(b);
                    edrSize++;
                }
            cb = Arrays.CreateJagged<int>(edrSize, totalEdgeCount);
            for (int i = 0; i < edrSize; i++)
                for (int a = 0; a < totalEdgeCount; a++)
                    cb[i][a] = 0;

            // remove ATOM_NUMBER props again
            foreach (var atom in atomContainer.Atoms)
                atom.RemoveProperty(ATOM_NUMBER);
        }

        /**
		 * Access the computed spanning tree of the input molecule.
		 *
		 * @return acyclic tree of the input molecule
		 */
        public IAtomContainer GetSpanningTree()
        {
            IAtomContainer container = molecule.Builder.CreateAtomContainer();
            for (int a = 0; a < totalVertexCount; a++)
                container.Add(molecule.Atoms[a]);
            for (int b = 0; b < totalEdgeCount; b++)
                if (bondsInTree[b]) container.Add(molecule.Bonds[b]);
            return container;
        }

        /**
		 * Find a path connected <i>a1</i> and <i>a2</i> in the tree. If there was
		 * an edge between <i>a1</i> and <i>a2</i> this path is a cycle.
		 *
		 * @param spt spanning tree
		 * @param atom1  start of path (source)
		 * @param atom2  end of path (target)
		 * @return a path through the spanning tree from the source to the target
		 * @ thrown if the atom is not in the spanning
		 *                             tree
		 */
        public IAtomContainer GetPath(IAtomContainer spt, IAtom atom1, IAtom atom2)
        {
            IAtomContainer path = spt.Builder.CreateAtomContainer();
            PathTools.ResetFlags(spt);
            path.Add(atom1);
            PathTools.DepthFirstTargetSearch(spt, atom1, atom2, path);
            if (path.Atoms.Count == 1) path.Remove(atom1); // no path found: remove initial atom
            return path;
        }

        private IRing GetRing(IAtomContainer spt, IBond bond)
        {
            IRing ring = spt.Builder.CreateRing();
            PathTools.ResetFlags(spt);
            ring.Add(bond.Atoms[0]);
            PathTools.DepthFirstTargetSearch(spt, bond.Atoms[0], bond.Atoms[1], ring);
            ring.Add(bond);
            return ring;
        }

        private void GetBondsInRing(IAtomContainer mol, IRing ring, int[] bonds)
        {
            for (int i = 0; i < ring.Bonds.Count; i++)
            {
                int m = mol.Bonds.IndexOf(ring.Bonds[i]);
                bonds[m] = 1;
            }
        }

        /**
		 * The basic rings of the spanning tree. Using the pruned edges, return any path
		 * which connects the end points of the pruned edge in the tree. These paths form
		 * cycles.
		 *
		 * @return basic rings
		 * @ atoms not found in the molecule
		 */
        public IRingSet GetBasicRings()
        {
            IRingSet ringset = molecule.Builder.CreateRingSet();
            IAtomContainer spt = GetSpanningTree();
            for (int i = 0; i < totalEdgeCount; i++)
                if (!bondsInTree[i]) ringset.Add(GetRing(spt, molecule.Bonds[i]));
            return ringset;
        }

        /**
		 * Returns an IAtomContainer which contains all the atoms and bonds which
		 * are involved in ring systems.
		 *
		 * @see #GetAllRings()
		 * @see #GetBasicRings()
		 * @return the IAtomContainer as described above
		 */
        public IAtomContainer GetCyclicFragmentsContainer()
        {
            IAtomContainer fragContainer = this.molecule.Builder.CreateAtomContainer();
            IAtomContainer spt = GetSpanningTree();

            for (int i = 0; i < totalEdgeCount; i++)
                if (!bondsInTree[i])
                {
                    IRing ring = GetRing(spt, molecule.Bonds[i]);
                    for (int b = 0; b < ring.Bonds.Count; b++)
                    {
                        IBond ringBond = ring.Bonds[b];
                        if (!fragContainer.Contains(ringBond))
                        {
                            fragContainer.Add(ringBond);
                            for (int atomCount = 0; atomCount < ringBond.Atoms.Count; atomCount++)
                            {
                                IAtom atom = ringBond.Atoms[atomCount];
                                if (!fragContainer.Contains(atom))
                                {
                                    atom.IsInRing = true;
                                    fragContainer.Add(atom);
                                }
                            }
                        }
                    }
                }
            return fragContainer;
        }

        /**
		 * Identifies whether bonds are cyclic or not. It is used by several other methods.
		 */
        private void IdentifyBonds()
        {
            IAtomContainer spt = GetSpanningTree();
            IRing ring;
            int nBasicRings = 0;
            for (int i = 0; i < totalEdgeCount; i++)
            {
                if (!bondsInTree[i])
                {
                    ring = GetRing(spt, molecule.Bonds[i]);
                    for (int b = 0; b < ring.Bonds.Count; b++)
                    {
                        int m = molecule.Bonds.IndexOf(ring.Bonds[b]);
                        cb[nBasicRings][m] = 1;
                    }
                    nBasicRings++;
                }
            }
            bondsAcyclicCount = 0;
            bondsCyclicCount = 0;
            for (int i = 0; i < totalEdgeCount; i++)
            {
                int s = 0;
                for (int j = 0; j < nBasicRings; j++)
                {
                    s += cb[j][i];
                }
                switch (s)
                {
                    case (0):
                        {
                            bondsAcyclicCount++;
                            break;
                        }
                    case (1):
                        {
                            bondsCyclicCount++;
                            break;
                        }
                    default:
                        {
                            bondsCyclicCount++;
                            break;
                        }
                }
            }
            identifiedBonds = true;
        }

        /**
		 * All basic rings and the all pairs of basic rings share at least one edge
		 * combined.
		 *
		 * @return subset of all rings
		 * @ atom was not found in the molecule
		 * @see #GetBasicRings()
		 */
        public IRingSet GetAllRings()
        {
            IRingSet ringset = GetBasicRings();
            IRing newring;

            int nBasicRings = ringset.Count;
            for (int i = 0; i < nBasicRings; i++)
                GetBondsInRing(molecule, (IRing)ringset[i], cb[i]);

            for (int i = 0; i < nBasicRings; i++)
            {
                for (int j = i + 1; j < nBasicRings; j++)
                {
                    //Debug.WriteLine("combining rings "+(i+1)+","+(j+1));
                    newring = CombineRings(ringset, i, j);
                    //newring = CombineRings((Ring)ringset[i],(Ring)ringset[j]);
                    if (newring != null) ringset.Add(newring);
                }
            }

            return ringset;
        }

        /**
		 * Size of the spanning tree specified as the number of edges in the tree.
		 *
		 * @return number of edges in the spanning tree
		 */
        public int GetSpanningTreeSize()
        {
            return sptSize;
        }

        private IRing CombineRings(IRingSet ringset, int i, int j)
        {
            int c = 0;
            for (int b = 0; b < cb[i].Length; b++)
            {
                c = cb[i][b] + cb[j][b];
                if (c > 1) break; //at least one common bond
            }
            if (c < 2) return null;
            IRing ring = molecule.Builder.CreateRing();
            IRing ring1 = (IRing)ringset[i];
            IRing ring2 = (IRing)ringset[j];
            for (int b = 0; b < cb[i].Length; b++)
            {
                c = cb[i][b] + cb[j][b];
                if ((c == 1) && (cb[i][b] == 1))
                    ring.Add(molecule.Bonds[b]);
                else if ((c == 1) && (cb[j][b] == 1)) ring.Add(molecule.Bonds[b]);
            }
            for (int a = 0; a < ring1.Atoms.Count; a++)
                ring.Add(ring1.Atoms[a]);
            for (int a = 0; a < ring2.Atoms.Count; a++)
                ring.Add(ring2.Atoms[a]);

            return ring;
        }

        /**
		 * Number of acyclic bonds.
		 *
		 * @return Returns the bondsAcyclicCount.
		 */
        public int GetBondsAcyclicCount()
        {
            if (!identifiedBonds) IdentifyBonds();
            return bondsAcyclicCount;
        }

        /**
		 * Number of cyclic bonds.
		 *
		 * @return Returns the bondsCyclicCount.
		 */
        public int GetBondsCyclicCount()
        {
            if (!identifiedBonds) IdentifyBonds();
            return bondsCyclicCount;
        }
    }
}
