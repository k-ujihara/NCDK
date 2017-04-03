/* Copyright (C) 2005-2006 Markus Leber
 *               2006-2009 Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Helper;
using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK.SMSD.Algorithms.McGregors
{
    /// <summary>
    /// Class which reports MCS solutions based on the McGregor algorithm
    /// published in 1982.
    ///
    ///  <para>The SMSD algorithm is described in this paper.
    /// please refer Rahman <i>et.al. 2009</i>
    ///  <token>cdk-cite-SMSD2009</token>.
    ///  </para>
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public sealed class McGregor
    {
        private IAtomContainer source = null;
        private IAtomContainer target = null;
        private BinaryTree last = null;
        private BinaryTree first = null;
        private Stack<IList<int>> bestArcs = null;
        private IList<int> modifiedARCS = null;
        private int bestarcsleft = 0;
        private int globalMCSSize = 0;
        private IList<IList<int>> mappings = null;
        /* This should be more or equal to all the atom types */
        private static readonly string[] SIGNS = {"$1", "$2", "$3", "$4", "$5", "$6", "$7", "$8", "$9", "$10", "$11",
                                                               "$12", "$13", "$15", "$16", "$17", "$18", "$19", "$20", "$21", "$22", "$23", "$24", "$25", "$26", "$27",
                                                               "$28", "$29", "$30", "$31", "$32", "$33", "$34", "$35", "$36", "$37", "$38", "$39", "$40", "$41", "$42",
                                                               "$43", "$44", "$45", "$46", "$47", "$48", "$49", "$50", "$51", "$52", "$53", "$54", "$55"};
        private bool newMatrix = false;
        private bool bondMatch = false;

        /// <summary>
        /// Constructor for the McGregor algorithm.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="mappings"></param>
        /// <param name="shouldMatchBonds"></param>
        public McGregor(IAtomContainer source, IAtomContainer target, IList<IList<int>> mappings, bool shouldMatchBonds)
        {
            IsBondMatch = shouldMatchBonds;
            this.source = source;
            this.target = target;
            this.mappings = mappings;
            this.bestarcsleft = 0;

            if (mappings.Count != 0)
            {
                this.globalMCSSize = mappings[0].Count;
            }
            else
            {
                this.globalMCSSize = 0;
            }
            this.modifiedARCS = new List<int>();
            this.bestArcs = new Stack<IList<int>>();
            this.newMatrix = false;
        }

        /// <summary>
        /// Constructor for the McGregor algorithm.
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="mappings"></param>
        /// </summary>
        public McGregor(IQueryAtomContainer source, IAtomContainer target, IList<IList<int>> mappings)
        {
            IsBondMatch = true;
            this.source = source;
            this.target = target;
            this.mappings = mappings;
            this.bestarcsleft = 0;

            if (mappings.Count != 0)
            {
                this.globalMCSSize = mappings[0].Count;
            }
            else
            {
                this.globalMCSSize = 0;
            }
            this.modifiedARCS = new List<int>();
            this.bestArcs = new Stack<IList<int>>();
            this.newMatrix = false;
        }

        /// <summary>
        /// Start McGregor search and extend the mappings if possible.
        /// </summary>
        /// <param name="largestMappingSize"></param>
        /// <param name="presentMapping"></param>
        /// <exception cref="IOException"></exception>
        public void StartMcGregorIteration(int largestMappingSize, IDictionary<int, int> presentMapping)
        {
            this.globalMCSSize = (largestMappingSize / 2);
            var cTab1Copy = McGregorChecks.GenerateCTabCopy(source);
            var cTab2Copy = McGregorChecks.GenerateCTabCopy(target);

            //find mapped atoms of both molecules and store these in mappedAtoms
            List<int> mappedAtoms = new List<int>();
            //        Console.Out.WriteLine("\nMapped Atoms");
            foreach (var map in presentMapping)
            {
                //            Console.Out.WriteLine("i:" + map.Key + " j:" + map.Value);
                mappedAtoms.Add(map.Key);
                mappedAtoms.Add(map.Value);
            }
            int mappingSize = presentMapping.Count;

            List<int> iBondNeighborsA = new List<int>();
            List<string> cBondNeighborsA = new List<string>();

            List<int> iBondSetA = new List<int>();
            List<string> cBondSetA = new List<string>();

            List<int> iBondNeighborsB = new List<int>();
            List<int> iBondSetB = new List<int>();
            List<string> cBondNeighborsB = new List<string>();
            List<string> cBondSetB = new List<string>();

            //find unmapped atoms of molecule A

            List<int> unmappedAtomsMolA = McGregorChecks.MarkUnMappedAtoms(true, source, presentMapping);
            int counter = 0;
            int gSetBondNumA = 0;
            int gSetBondNumB = 0;
            int gNeighborBondnumA = 0; //number of remaining molecule A bonds after the clique search, which are neighbors of the MCS_1
            int gNeighborBondNumB = 0; //number of remaining molecule B bonds after the clique search, which are neighbors of the MCS_1

            QueryProcessor queryProcess = new QueryProcessor(cTab1Copy, cTab2Copy, SIGNS, gNeighborBondnumA,
                    gSetBondNumA, iBondNeighborsA, cBondNeighborsA, mappingSize, iBondSetA, cBondSetA);

            if (!(source is IQueryAtomContainer))
            {
                queryProcess.Process(source, target, unmappedAtomsMolA, mappedAtoms, counter);
            }
            else
            {
                queryProcess.Process((IQueryAtomContainer)source, target, unmappedAtomsMolA, mappedAtoms, counter);
            }

            cTab1Copy = queryProcess.CTab1;
            cTab2Copy = queryProcess.CTab2;
            gSetBondNumA = queryProcess.BondNumA;
            gNeighborBondnumA = queryProcess.NeighborBondNumA;
            iBondNeighborsA = queryProcess.IBondNeighboursA;
            cBondNeighborsA = queryProcess.CBondNeighborsA;

            //find unmapped atoms of molecule B
            List<int> unmappedAtomsMolB = McGregorChecks.MarkUnMappedAtoms(false, target, presentMapping);

            //        Console.Out.WriteLine("unmappedAtomsMolB: " + unmappedAtomsMolB.Count);

            //Extract bonds which are related with unmapped atoms of molecule B.
            //In case that unmapped atoms are connected with already mapped atoms, the mapped atoms are labelled with
            //new special signs -> the result are two vectors: cBondNeighborsA and int_bonds_molB, which contain those
            //bonds of molecule B, which are relevant for the McGregorBondTypeInSensitive algorithm.
            //The special signs must be transfered to the corresponding atoms of molecule A

            TargetProcessor targetProcess = new TargetProcessor(cTab1Copy, cTab2Copy, SIGNS, gNeighborBondNumB,
                    gSetBondNumB, iBondNeighborsB, cBondNeighborsB, gNeighborBondnumA, iBondNeighborsA,
                    cBondNeighborsA);

            targetProcess.Process(target, unmappedAtomsMolB, mappingSize, iBondSetB, cBondSetB, mappedAtoms,
                    counter);

            cTab1Copy = targetProcess.CTab1;
            cTab2Copy = targetProcess.CTab2;
            gSetBondNumB = targetProcess.BondNumB;
            gNeighborBondNumB = targetProcess.NeighborBondNumB;
            iBondNeighborsB = targetProcess.IBondNeighboursB;
            cBondNeighborsB = targetProcess.CBondNeighborsB;

            bool dummy = false;

            McgregorHelper mcGregorHelper = new McgregorHelper(dummy, presentMapping.Count, mappedAtoms,
                    gNeighborBondnumA, gNeighborBondNumB, iBondNeighborsA, iBondNeighborsB, cBondNeighborsA,
                    cBondNeighborsB, gSetBondNumA, gSetBondNumB, iBondSetA, iBondSetB, cBondSetA, cBondSetB);
            Iterator(mcGregorHelper);
        }

        /// <summary>
        /// Start McGregor search and extend the mappings if possible.
        /// </summary>
        /// <param name="largestMappingSize"></param>
        /// <param name="cliqueVector"></param>
        /// <param name="compGraphNodes"></param>
        /// <exception cref="IOException"></exception>
        public void StartMcGregorIteration(int largestMappingSize, IList<int> cliqueVector,
                IList<int> compGraphNodes)
        {
            this.globalMCSSize = (largestMappingSize / 2);
            List<string> cTab1Copy = McGregorChecks.GenerateCTabCopy(source);

            List<string> cTab2Copy = McGregorChecks.GenerateCTabCopy(target);

            //find mapped atoms of both molecules and store these in mappedAtoms
            List<int> mappedAtoms = new List<int>();

            int mappedAtomCount = 0;

            List<int> iBondNeighborAtomsA = new List<int>();
            List<string> cBondNeighborsA = new List<string>();

            List<int> iBondSetA = new List<int>();
            List<string> cBondSetA = new List<string>();

            List<int> iBondNeighborAtomsB = new List<int>();
            List<int> iBondSetB = new List<int>();
            List<string> cBondNeighborsB = new List<string>();
            List<string> cBondSetB = new List<string>();

            int cliqueSize = cliqueVector.Count;
            int vecSize = compGraphNodes.Count;

            int cliqueNumber = 0;

            for (int a = 0; a < cliqueSize; a++)
            {
                //go through all clique nodes
                cliqueNumber = cliqueVector[a];
                for (int b = 0; b < vecSize; b += 3)
                {
                    //go through all nodes in the compatibility graph
                    if (cliqueNumber == compGraphNodes[b + 2])
                    {
                        mappedAtoms.Add(compGraphNodes[b]);
                        mappedAtoms.Add(compGraphNodes[b + 1]);
                        mappedAtomCount++;
                    }
                }
            }

            //find unmapped atoms of molecule A
            List<int> unmappedAtomsMolA = McGregorChecks.MarkUnMappedAtoms(true, source, mappedAtoms, cliqueSize);

            int counter = 0;
            int setNumA = 0;
            int setNumB = 0;
            int localNeighborBondnumA = 0; //number of remaining molecule A bonds after the clique search, which are neighbors of the MCS_1
            int localNeighborBondNumB = 0; //number of remaining molecule B bonds after the clique search, which are neighbors of the MCS_1

            //Extract bonds which are related with unmapped atoms of molecule A.
            //In case that unmapped atoms are connected with already mapped atoms, the mapped atoms are labelled with
            //new special signs -> the result are two vectors: cBondNeighborsA and int_bonds_molA, which contain those
            //bonds of molecule A, which are relevant for the McGregorBondTypeInSensitive algorithm.
            //The special signs must be transfered to the corresponding atoms of molecule B

            QueryProcessor queryProcess = new QueryProcessor(cTab1Copy, cTab2Copy, SIGNS, localNeighborBondnumA,
                    setNumA, iBondNeighborAtomsA, cBondNeighborsA, cliqueSize, iBondSetA, cBondSetA);

            queryProcess.Process(source, target, unmappedAtomsMolA, mappedAtoms, counter);

            cTab1Copy = queryProcess.CTab1;
            cTab2Copy = queryProcess.CTab2;
            setNumA = queryProcess.BondNumA;
            localNeighborBondnumA = queryProcess.NeighborBondNumA;
            iBondNeighborAtomsA = queryProcess.IBondNeighboursA;
            cBondNeighborsA = queryProcess.CBondNeighborsA;

            //find unmapped atoms of molecule B
            List<int> unmappedAtomsMolB = McGregorChecks.MarkUnMappedAtoms(false, target, mappedAtoms, cliqueSize);

            //Extract bonds which are related with unmapped atoms of molecule B.
            //In case that unmapped atoms are connected with already mapped atoms, the mapped atoms are labelled with
            //new special signs -> the result are two vectors: cBondNeighborsA and int_bonds_molB, which contain those
            //bonds of molecule B, which are relevant for the McGregorBondTypeInSensitive algorithm.
            //The special signs must be transfered to the corresponding atoms of molecule A

            TargetProcessor targetProcess = new TargetProcessor(cTab1Copy, cTab2Copy, SIGNS, localNeighborBondNumB,
                    setNumB, iBondNeighborAtomsB, cBondNeighborsB, localNeighborBondnumA, iBondNeighborAtomsA,
                    cBondNeighborsA);

            targetProcess.Process(target, unmappedAtomsMolB, cliqueSize, iBondSetB, cBondSetB, mappedAtoms, counter);

            cTab1Copy = targetProcess.CTab1;
            cTab2Copy = targetProcess.CTab2;
            setNumB = targetProcess.BondNumB;
            localNeighborBondNumB = targetProcess.NeighborBondNumB;
            iBondNeighborAtomsB = targetProcess.IBondNeighboursB;
            cBondNeighborsB = targetProcess.CBondNeighborsB;

            bool dummy = false;

            McgregorHelper mcGregorHelper = new McgregorHelper(dummy, mappedAtomCount, mappedAtoms, localNeighborBondnumA,
                    localNeighborBondNumB, iBondNeighborAtomsA, iBondNeighborAtomsB, cBondNeighborsA, cBondNeighborsB,
                    setNumA, setNumB, iBondSetA, iBondSetB, cBondSetA, cBondSetB);
            Iterator(mcGregorHelper);

        }

        private int Iterator(McgregorHelper mcGregorHelper)
        {

            bool mappingCheckFlag = mcGregorHelper.IsMappingCheckFlag;
            int mappedAtomCount = mcGregorHelper.MappedAtomCount;
            List<int> mappedAtoms = new List<int>(mcGregorHelper.GetMappedAtomsOrg());
            int neighborBondNumA = mcGregorHelper.NeighborBondNumA;
            int neighborBondNumB = mcGregorHelper.NeighborBondNumB;

            //        //check possible mappings:
            bool furtherMappingFlag = McGregorChecks.IsFurtherMappingPossible(source, target, mcGregorHelper,
                    IsBondMatch);

            if (neighborBondNumA == 0 || neighborBondNumB == 0 || mappingCheckFlag || !furtherMappingFlag)
            {
                SetFinalMappings(mappedAtoms, mappedAtomCount);
                return 0;
            }

            modifiedARCS.Clear();
            int size = neighborBondNumA * neighborBondNumB;
            for (int i = 0; i < size; i++)
            {
                modifiedARCS.Insert(i, 0);
            }
            SetModifedArcs(mcGregorHelper);
            first = new BinaryTree(-1);
            last = first;
            last.Equal = null;
            last.NotEqual = null;
            bestarcsleft = 0;

            Startsearch(mcGregorHelper);
            Stack<IList<int>> bestArcsCopy = new Stack<IList<int>>();

            foreach (var bestArc in bestArcs)
                bestArcsCopy.Push(bestArc);
            while (bestArcs.Count != 0)
            {
                bestArcs.Pop();
            }
            SearchAndExtendMappings(bestArcsCopy, mcGregorHelper);

            //Console.Out.WriteLine("In the iterator Termination");
            //Console.Out.WriteLine("============+++++++++==============");
            //Console.Out.WriteLine("Mapped Atoms before iterator Over: " + mappedAtoms);
            return 0;
        }

        private void SearchAndExtendMappings(Stack<IList<int>> bestarcsCopy, McgregorHelper mcGregorHelper)
        {
            int mappedAtomCount = mcGregorHelper.MappedAtomCount;

            int setNumA = mcGregorHelper.SetNumA;
            int setNumB = mcGregorHelper.SetNumB;
            var iBondSetA = mcGregorHelper.GetIBondSetA();
            var iBondSetB = mcGregorHelper.GetIBondSetB();
            var cBondSetA = mcGregorHelper.GetCBondSetA();
            var cBondSetB = mcGregorHelper.GetCBondSetB();

            while (bestarcsCopy.Count != 0)
            {
                var mArcsVector = new List<int>(bestarcsCopy.Peek());
                var newMapping = FindMcGregorMapping(mArcsVector, mcGregorHelper);

                int newMapingSize = newMapping.Count / 2;
                bool noFurtherMappings = false;
                if (mappedAtomCount == newMapingSize)
                {
                    noFurtherMappings = true;
                }

                List<int> newINeighborsA = new List<int>(); //instead of iBondNeighborAtomsA
                List<int> newINeighborsB = new List<int>(); //instead of iBondNeighborAtomsB
                List<string> newCNeighborsA = new List<string>(); //instead of cBondNeighborsA
                List<string> newCNeighborsB = new List<string>(); //instead of cBondNeighborsB
                List<int> newIBondSetA = new List<int>(); //instead of iBondSetA
                List<int> newIBondSetB = new List<int>(); //instead of iBondSetB
                List<string> newCBondSetA = new List<string>(); //instead of cBondSetA
                List<string> newCBondSetB = new List<string>(); //instead of cBondSetB
                                                                //new values for setNumA + setNumB
                                                                //new arrays for iBondSetA + iBondSetB + cBondSetB + cBondSetB

                List<string> cSetACopy = McGregorChecks.GenerateCSetCopy(setNumA, cBondSetA);
                List<string> cSetBCopy = McGregorChecks.GenerateCSetCopy(setNumB, cBondSetB);

                //find unmapped atoms of molecule A
                List<int> unmappedAtomsMolA = new List<int>();
                int unmappedNumA = 0;
                bool atomAIsUnmapped = true;

                for (int a = 0; a < source.Atoms.Count; a++)
                {
                    for (int b = 0; b < newMapingSize; b++)
                    {
                        if (a == newMapping[b * 2 + 0])
                        {
                            atomAIsUnmapped = false;
                        }

                    }
                    if (atomAIsUnmapped)
                    {
                        unmappedAtomsMolA.Insert(unmappedNumA++, a);
                    }
                    atomAIsUnmapped = true;
                }

                //The special signs must be transfered to the corresponding atoms of molecule B

                int counter = 0;
                //number of remaining molecule A bonds after the clique search, which aren't neighbors
                int newSetBondNumA = 0; //instead of setNumA
                int newNeighborNumA = 0; //instead of localNeighborBondnumA

                QueryProcessor queryProcess = new QueryProcessor(cSetACopy, cSetBCopy, SIGNS, newNeighborNumA,
                        newSetBondNumA, newINeighborsA, newCNeighborsA, newMapingSize, newIBondSetA, newCBondSetA);

                queryProcess.Process(setNumA, setNumB, iBondSetA, iBondSetB, unmappedAtomsMolA, newMapping, counter);

                cSetACopy = queryProcess.CTab1;
                cSetBCopy = queryProcess.CTab2;
                newSetBondNumA = queryProcess.BondNumA;
                newNeighborNumA = queryProcess.NeighborBondNumA;
                newINeighborsA = queryProcess.IBondNeighboursA;
                newCNeighborsA = queryProcess.CBondNeighborsA;

                //find unmapped atoms of molecule B

                List<int> unmappedAtomsMolB = new List<int>();
                int unmappedNumB = 0;
                bool atomBIsUnmapped = true;

                for (int a = 0; a < target.Atoms.Count; a++)
                {
                    for (int b = 0; b < newMapingSize; b++)
                    {
                        if (a == newMapping[b * 2 + 1])
                        {
                            atomBIsUnmapped = false;
                        }
                    }
                    if (atomBIsUnmapped)
                    {
                        unmappedAtomsMolB.Insert(unmappedNumB++, a);
                    }
                    atomBIsUnmapped = true;
                }

                //number of remaining molecule B bonds after the clique search, which aren't neighbors
                int newSetBondNumB = 0; //instead of setNumB
                int newNeighborNumB = 0; //instead of localNeighborBondNumB

                TargetProcessor targetProcess = new TargetProcessor(cSetACopy, cSetBCopy, SIGNS, newNeighborNumB,
                        newSetBondNumB, newINeighborsB, newCNeighborsB, newNeighborNumA, newINeighborsA,
                        newCNeighborsA);

                targetProcess.Process(setNumB, unmappedAtomsMolB, newMapingSize, iBondSetB, cBondSetB, newMapping,
                        counter, newIBondSetB, newCBondSetB);

                cSetACopy = targetProcess.CTab1;
                cSetBCopy = targetProcess.CTab2;
                newSetBondNumB = targetProcess.BondNumB;
                newNeighborNumB = targetProcess.NeighborBondNumB;
                newINeighborsB = targetProcess.IBondNeighboursB;
                newCNeighborsB = targetProcess.CBondNeighborsB;

                //             Console.Out.WriteLine("Mapped Atoms before Iterator2: " + mappedAtoms);
                McgregorHelper newMH = new McgregorHelper(noFurtherMappings, newMapingSize, newMapping, newNeighborNumA,
                        newNeighborNumB, newINeighborsA, newINeighborsB, newCNeighborsA, newCNeighborsB,
                        newSetBondNumA, newSetBondNumB, newIBondSetA, newIBondSetB, newCBondSetA, newCBondSetB);

                Iterator(newMH);
                bestarcsCopy.Pop();
                //            Console.Out.WriteLine("End of the iterator!!!!");
            }
        }

        private IList<int> FindMcGregorMapping(List<int> mArcs, McgregorHelper mcGregorHelper)
        {
            int neighborBondNumA = mcGregorHelper.NeighborBondNumA;
            int neighborBondNumB = mcGregorHelper.NeighborBondNumB;
            List<int> currentMapping = new List<int>(mcGregorHelper.GetMappedAtomsOrg());
            List<int> additionalMapping = new List<int>();

            for (int x = 0; x < neighborBondNumA; x++)
            {
                for (int y = 0; y < neighborBondNumB; y++)
                {
                    if (mArcs[x * neighborBondNumB + y] == 1)
                    {
                        ExtendMapping(x, y, mcGregorHelper, additionalMapping, currentMapping);
                    }
                }
            }

            int additionalMappingSize = additionalMapping.Count;
            //add McGregorBondTypeInSensitive mapping to the Clique mapping
            for (int a = 0; a < additionalMappingSize; a += 2)
            {
                currentMapping.Add(additionalMapping[a + 0]);
                currentMapping.Add(additionalMapping[a + 1]);
            }

            //        remove recurring mappings from currentMapping

            var uniqueMapping = McGregorChecks.RemoveRecurringMappings(currentMapping);
            return uniqueMapping;
        }

        private void SetModifedArcs(McgregorHelper mcGregorHelper)
        {
            int neighborBondNumA = mcGregorHelper.NeighborBondNumA;
            int neighborBondNumB = mcGregorHelper.NeighborBondNumB;
            var iBondNeighborAtomsA = mcGregorHelper.GetIBondNeighborAtomsA();
            var iBondNeighborAtomsB = mcGregorHelper.GetIBondNeighborAtomsB();
            var cBondNeighborsA = mcGregorHelper.GetCBondNeighborsA();
            var cBondNeighborsB = mcGregorHelper.GetCBondNeighborsB();
            for (int row = 0; row < neighborBondNumA; row++)
            {
                for (int column = 0; column < neighborBondNumB; column++)
                {

                    string g1A = cBondNeighborsA[row * 4 + 0];
                    string g2A = cBondNeighborsA[row * 4 + 1];
                    string g1B = cBondNeighborsB[column * 4 + 0];
                    string g2B = cBondNeighborsB[column * 4 + 1];

                    if (MatchGAtoms(g1A, g2A, g1B, g2B))
                    {
                        int indexI = iBondNeighborAtomsA[row * 3 + 0];
                        int indexIPlus1 = iBondNeighborAtomsA[row * 3 + 1];

                        IAtom r1A = source.Atoms[indexI];
                        IAtom r2A = source.Atoms[indexIPlus1];
                        IBond reactantBond = source.GetBond(r1A, r2A);

                        int indexJ = iBondNeighborAtomsB[column * 3 + 0];
                        int indexJPlus1 = iBondNeighborAtomsB[column * 3 + 1];

                        IAtom p1B = target.Atoms[indexJ];
                        IAtom p2B = target.Atoms[indexJPlus1];
                        IBond productBond = target.GetBond(p1B, p2B);
                        if (McGregorChecks.IsMatchFeasible(source, reactantBond, target, productBond, IsBondMatch))
                        {
                            modifiedARCS[row * neighborBondNumB + column] = 1;
                        }
                    }
                }
            }
        }

        private void Partsearch(int xstart, int ystart, IList<int> tempMArcsOrg, McgregorHelper mcGregorHelper)
        {
            int neighborBondNumA = mcGregorHelper.NeighborBondNumA;
            int neighborBondNumB = mcGregorHelper.NeighborBondNumB;

            int xIndex = xstart;
            int yIndex = ystart;

            List<int> tempMArcs = new List<int>(tempMArcsOrg);

            if (tempMArcs[xstart * neighborBondNumB + ystart] == 1)
            {

                McGregorChecks.RemoveRedundantArcs(xstart, ystart, tempMArcs, mcGregorHelper);
                int arcsleft = McGregorChecks.CountArcsLeft(tempMArcs, neighborBondNumA, neighborBondNumB);

                //test Best arcs left and skip rest if needed
                if (arcsleft >= bestarcsleft)
                {
                    SetArcs(xIndex, yIndex, arcsleft, tempMArcs, mcGregorHelper);
                }
            }
            else
            {
                do
                {
                    yIndex++;
                    if (yIndex == neighborBondNumB)
                    {
                        yIndex = 0;
                        xIndex++;
                    }

                } while ((xIndex < neighborBondNumA) && (tempMArcs[xIndex * neighborBondNumB + yIndex] != 1)); //Correction by ASAD set value minus 1

                if (xIndex < neighborBondNumA)
                {

                    Partsearch(xIndex, yIndex, tempMArcs, mcGregorHelper);
                    tempMArcs[xIndex * neighborBondNumB + yIndex] = 0;
                    Partsearch(xIndex, yIndex, tempMArcs, mcGregorHelper);
                }
                else
                {
                    int arcsleft = McGregorChecks.CountArcsLeft(tempMArcs, neighborBondNumA, neighborBondNumB);
                    if (arcsleft >= bestarcsleft)
                    {
                        PopBestArcs(arcsleft);

                        if (CheckmArcs(tempMArcs, neighborBondNumA, neighborBondNumB))
                        {
                            bestArcs.Push(tempMArcs);
                        }

                    }
                }
            }
        }

        //The function is called in function partsearch. The function is given indexZ temporary matrix.
        //The function checks whether the temporary matrix is already found by calling the function
        //"verifyNodes". If the matrix already exists the function returns false which means that
        //the matrix will not be stored. Otherwise the function returns true which means that the
        //matrix will be stored in function partsearch.
        private bool CheckmArcs(List<int> mArcsT, int neighborBondNumA, int neighborBondNumB)
        {

            int size = neighborBondNumA * neighborBondNumA;
            List<int> posNumList = new List<int>(size);

            for (int i = 0; i < posNumList.Count; i++)
            {
                posNumList.Insert(i, 0);
            }

            int yCounter = 0;
            int countEntries = 0;
            for (int x = 0; x < (neighborBondNumA * neighborBondNumB); x++)
            {
                if (mArcsT[x] == 1)
                {
                    posNumList.Insert(yCounter++, x);
                    countEntries++;
                }
            }
            bool flag = false;

            VerifyNodes(posNumList, first, 0, countEntries);
            if (IsNewMatrix)
            {
                flag = true;
            }

            return flag;

        }

        private bool VerifyNodes(List<int> matrix, BinaryTree currentStructure, int index, int fieldLength)
        {
            if (index < fieldLength)
            {
                if (matrix[index] == currentStructure.Value && currentStructure.Equal != null)
                {
                    IsNewMatrix = false;
                    VerifyNodes(matrix, currentStructure.Equal, index + 1, fieldLength);
                }
                if (matrix[index] != currentStructure.Value)
                {
                    if (currentStructure.NotEqual != null)
                    {
                        VerifyNodes(matrix, currentStructure.NotEqual, index, fieldLength);
                    }

                    if (currentStructure.NotEqual == null)
                    {
                        currentStructure.NotEqual = new BinaryTree(matrix[index]);
                        currentStructure.NotEqual.NotEqual = null;
                        int yIndex = 0;

                        BinaryTree lastOne = currentStructure.NotEqual;

                        while ((yIndex + index + 1) < fieldLength)
                        {
                            lastOne.Equal = new BinaryTree(matrix[yIndex + index + 1]);
                            lastOne = lastOne.Equal;
                            lastOne.NotEqual = null;
                            yIndex++;

                        }
                        lastOne.Equal = null;
                         IsNewMatrix = true;
                    }

                }
            }
            return true;
        }

        private void Startsearch(McgregorHelper mcGregorHelper)
        {
            int neighborBondNumA = mcGregorHelper.NeighborBondNumA;
            int neighborBondNumB = mcGregorHelper.NeighborBondNumB;

            int size = neighborBondNumA * neighborBondNumB;
            List<int> fixArcs = new List<int>(size);//  Initialize fixArcs with 0
            for (int i = 0; i < size; i++)
            {
                fixArcs.Insert(i, 0);
            }

            int xIndex = 0;
            int yIndex = 0;

            while ((xIndex < neighborBondNumA) && (modifiedARCS[xIndex * neighborBondNumB + yIndex] != 1))
            {
                yIndex++;
                if (yIndex == neighborBondNumB)
                {
                    yIndex = 0;
                    xIndex++;
                }
            }

            if (xIndex == neighborBondNumA)
            {
                yIndex = neighborBondNumB - 1;
                xIndex -= 1;
            }

            if (modifiedARCS[xIndex * neighborBondNumB + yIndex] == 0)
            {
                Partsearch(xIndex, yIndex, modifiedARCS, mcGregorHelper);
            }

            if (modifiedARCS[xIndex * neighborBondNumB + yIndex] != 0)
            {
                Partsearch(xIndex, yIndex, modifiedARCS, mcGregorHelper);
                modifiedARCS[xIndex * neighborBondNumB + yIndex] = 0;
                Partsearch(xIndex, yIndex, modifiedARCS, mcGregorHelper);
            }

        }

        /// <summary>
        /// Returns computed mappings.
        /// <returns>mappings</returns>
        /// </summary>
        public IList<IList<int>> Mappings => mappings;

        /// <summary>
        /// Returns MCS size.
        /// <returns>MCS size</returns>
        /// </summary>
        public int MCSSize => this.globalMCSSize;

        private void SetFinalMappings(List<int> mappedAtoms, int mappedAtomCount)
        {
            try
            {
                if (mappedAtomCount >= globalMCSSize)
                {
                    //                    Console.Out.WriteLine("Hello-1");
                    if (mappedAtomCount > globalMCSSize)
                    {
                        //                        Console.Out.WriteLine("Hello-2");
                        this.globalMCSSize = mappedAtomCount;
                        //                        Console.Out.WriteLine("best_MAPPING_size: " + globalMCSSize);
                        mappings.Clear();
                    }
                    mappings.Add(mappedAtoms);
                    //                    Console.Out.WriteLine("mappings " + mappings);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.StackTrace);
            }
        }

        private void SetArcs(int xIndex, int yIndex, int arcsleft, List<int> tempMArcs, McgregorHelper mcGregorHelper)
        {
            int neighborBondNumA = mcGregorHelper.NeighborBondNumA;
            int neighborBondNumB = mcGregorHelper.NeighborBondNumB;
            do
            {
                yIndex++;
                if (yIndex == neighborBondNumB)
                {
                    yIndex = 0;
                    xIndex++;

                }
            } //Correction by ASAD set value minus 1
            while ((xIndex < neighborBondNumA) && (tempMArcs[xIndex * neighborBondNumB + yIndex] != 1));
            if (xIndex < neighborBondNumA)
            {
                Partsearch(xIndex, yIndex, tempMArcs, mcGregorHelper);
                tempMArcs[xIndex * neighborBondNumB + yIndex] = 0;
                Partsearch(xIndex, yIndex, tempMArcs, mcGregorHelper);
            }
            else
            {
                PopBestArcs(arcsleft);
                if (CheckmArcs(tempMArcs, neighborBondNumA, neighborBondNumB))
                {
                    bestArcs.Push(tempMArcs);
                }
            }
        }

        private void PopBestArcs(int arcsleft)
        {
            if (arcsleft > bestarcsleft)
            {
                McGregorChecks.RemoveTreeStructure(first);
                first = last = new BinaryTree(-1);
                last.Equal = null;
                last.NotEqual = null;
                while (bestArcs.Count != 0)
                {
                    bestArcs.Pop();
                }
            }
            bestarcsleft = arcsleft;
        }

        private void ExtendMapping(int xIndex, int yIndex, McgregorHelper mcGregorHelper, List<int> additionalMapping,
                List<int> currentMapping)
        {

            int atom1MoleculeA = mcGregorHelper.GetIBondNeighborAtomsA()[xIndex * 3 + 0];
            int atom2MoleculeA = mcGregorHelper.GetIBondNeighborAtomsA()[xIndex * 3 + 1];
            int atom1MoleculeB = mcGregorHelper.GetIBondNeighborAtomsB()[yIndex * 3 + 0];
            int atom2MoleculeB = mcGregorHelper.GetIBondNeighborAtomsB()[yIndex * 3 + 1];

            IAtom r1A = source.Atoms[atom1MoleculeA];
            IAtom r2A = source.Atoms[atom2MoleculeA];
            IBond reactantBond = source.GetBond(r1A, r2A);

            IAtom p1B = target.Atoms[atom1MoleculeB];
            IAtom p2B = target.Atoms[atom2MoleculeB];
            IBond productBond = target.GetBond(p1B, p2B);

            //      Bond Order Check Introduced by Asad

            if (McGregorChecks.IsMatchFeasible(source, reactantBond, target, productBond, IsBondMatch))
            {

                for (int indexZ = 0; indexZ < mcGregorHelper.MappedAtomCount; indexZ++)
                {

                    int mappedAtom1 = currentMapping[indexZ * 2 + 0];
                    int mappedAtom2 = currentMapping[indexZ * 2 + 1];

                    if ((mappedAtom1 == atom1MoleculeA) && (mappedAtom2 == atom1MoleculeB))
                    {
                        additionalMapping.Add(atom2MoleculeA);
                        additionalMapping.Add(atom2MoleculeB);
                    }
                    else if ((mappedAtom1 == atom1MoleculeA) && (mappedAtom2 == atom2MoleculeB))
                    {
                        additionalMapping.Add(atom2MoleculeA);
                        additionalMapping.Add(atom1MoleculeB);
                    }
                    else if ((mappedAtom1 == atom2MoleculeA) && (mappedAtom2 == atom1MoleculeB))
                    {
                        additionalMapping.Add(atom1MoleculeA);
                        additionalMapping.Add(atom2MoleculeB);
                    }
                    else if ((mappedAtom1 == atom2MoleculeA) && (mappedAtom2 == atom2MoleculeB))
                    {
                        additionalMapping.Add(atom1MoleculeA);
                        additionalMapping.Add(atom1MoleculeB);
                    }
                }//for loop
            }
        }

        private bool MatchGAtoms(string g1A, string g2A, string g1B, string g2B)
        {
            return (string.Equals(g1A, g1B, StringComparison.OrdinalIgnoreCase) && string.Equals(g2A, g2B, StringComparison.OrdinalIgnoreCase))
                    || (string.Equals(g1A, g2B, StringComparison.OrdinalIgnoreCase) && string.Equals(g2A, g1B, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if its a new Matrix.
        /// </summary>
        public bool IsNewMatrix
        {
            get
            {
                return newMatrix;
            }
            set
            {
                this.newMatrix = value;
            }
        }

        /// <summary>
        /// Should bonds match
        /// </summary>
        private bool IsBondMatch
        {
            get
            {
                return bondMatch;
            }
            set
            {
                this.bondMatch = value;
            }
        }
    }
}
