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
using System;
using System.Collections.Generic;

namespace NCDK.SMSD.Algorithms.McGregors
{
    /**
     * Class to handle mappings of query molecule.
     * @cdk.module smsd
     * @cdk.githash
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     */
    public class QueryProcessor
    {

        private List<string> cTab1Copy;
        private List<string> cTab2Copy;
        private string[] signs;
        private int neighborBondNumA = 0; //number of remaining molecule A bonds after the clique search, which are neighbors of the MCS_1
        private int setBondNumA = 0; //number of remaining molecule A bonds after the clique search, which aren't neighbors
        private List<int> iBondNeighborsA;
        private List<string> cBondNeighborsA;
        private int newNeighborNumA;
        private List<int> newINeighborsA;
        private List<string> newCNeighborsA;

        /**
         * Query molecule
         * @param cTab1Copy
         * @param cTab2Copy
         * @param signArray
         * @param neighborBondnumA
         * @param setBondnumA
         * @param iBondNeighborsA
         * @param cBondNeighborsA
         * @param mappingSize
         * @param iBondSetA
         * @param cBondSetA
         */
        protected internal QueryProcessor(List<string> cTab1Copy, List<string> cTab2Copy, string[] signArray,
                int neighborBondnumA, int setBondnumA, List<int> iBondNeighborsA, List<string> cBondNeighborsA,
                int mappingSize, List<int> iBondSetA, List<string> cBondSetA)
        {

            this.cTab1Copy = cTab1Copy;
            this.cTab2Copy = cTab2Copy;
            this.signs = signArray;
            this.neighborBondNumA = neighborBondnumA;
            this.setBondNumA = setBondnumA;
            this.iBondNeighborsA = iBondNeighborsA;
            this.cBondNeighborsA = cBondNeighborsA;
            this.newNeighborNumA = mappingSize;
            this.newINeighborsA = iBondSetA;
            this.newCNeighborsA = cBondSetA;
        }

        /**
         *
         * @param query
         * @param target
         * @param unmappedAtomsMolA
         * @param mappedAtoms
         * @param counter
         */
        protected internal void Process(IAtomContainer query, IAtomContainer target, IList<int> unmappedAtomsMolA,
                IList<int> mappedAtoms, int counter)
        {

            int unmappedNumA = unmappedAtomsMolA.Count;
            bool bondConsidered = false;
            bool normalBond = true;

            //        Console.Out.WriteLine("\n" + cTab1Copy + "\n");
            for (int atomIndex = 0; atomIndex < query.Bonds.Count; atomIndex++)
            {

                int indexI = query.Atoms.IndexOf(query.Bonds[atomIndex].Atoms[0]);
                int indexJ = query.Atoms.IndexOf(query.Bonds[atomIndex].Atoms[1]);
                int order = query.Bonds[atomIndex].Order.Numeric;

                //            Console.Out.WriteLine(AtomI + "= , =" + AtomJ );
                for (int unMappedAtomIndex = 0; unMappedAtomIndex < unmappedNumA; unMappedAtomIndex++)
                {

                    if (unmappedAtomsMolA[unMappedAtomIndex].Equals(indexI))
                    {
                        normalBond = UnMappedAtomsEqualsIndexJ(query, target, atomIndex, counter, mappedAtoms, indexI,
                                indexJ, order);
                        bondConsidered = true;
                    }
                    else //Does a ungemaptes atom at second position in the connection occur?
                    if (unmappedAtomsMolA[unMappedAtomIndex].Equals(indexJ))
                    {
                        normalBond = UnMappedAtomsEqualsIndexI(query, target, atomIndex, counter, mappedAtoms, indexI,
                                indexJ, order);
                        bondConsidered = true;
                    }
                    if (normalBond && bondConsidered)
                    {
                        MarkNormalBonds(atomIndex, indexI, indexJ, order);
                        normalBond = true;
                        break;
                    }
                }
                bondConsidered = false;
            }
        }

        /**
         *
         * @param query
         * @param target
         * @param unmappedAtomsMolA
         * @param mappedAtoms
         * @param counter
         */
        protected void Process(IQueryAtomContainer query, IAtomContainer target, IList<int> unmappedAtomsMolA,
                IList<int> mappedAtoms, int counter)
        {

            int unmappedNumA = unmappedAtomsMolA.Count;
            bool bondConsidered = false;
            bool normalBond = true;

            //        Console.Out.WriteLine("\n" + cTab1Copy + "\n");

            for (int atomIndex = 0; atomIndex < query.Bonds.Count; atomIndex++)
            {
                int indexI = query.Atoms.IndexOf(query.Bonds[atomIndex].Atoms[0]);
                int indexJ = query.Atoms.IndexOf(query.Bonds[atomIndex].Atoms[1]);
                int order = 0;
                if (!query.Bonds[atomIndex].Order.IsUnset)
                {
                    order = query.Bonds[atomIndex].Order.Numeric;
                }

                //            Console.Out.WriteLine(AtomI + "= , =" + AtomJ );
                for (int unMappedAtomIndex = 0; unMappedAtomIndex < unmappedNumA; unMappedAtomIndex++)
                {

                    if (unmappedAtomsMolA[unMappedAtomIndex].Equals(indexI))
                    {
                        normalBond = UnMappedAtomsEqualsIndexJ(query, target, atomIndex, counter, mappedAtoms, indexI,
                                indexJ, order);
                        bondConsidered = true;
                    }
                    else //Does a ungemaptes atom at second position in the connection occur?
                    if (unmappedAtomsMolA[unMappedAtomIndex].Equals(indexJ))
                    {
                        normalBond = UnMappedAtomsEqualsIndexI(query, target, atomIndex, counter, mappedAtoms, indexI,
                                indexJ, order);
                        bondConsidered = true;
                    }
                    if (normalBond && bondConsidered)
                    {
                        MarkNormalBonds(atomIndex, indexI, indexJ, order);
                        normalBond = true;
                        break;
                    }
                }
                bondConsidered = false;
            }
        }

        /**
         *
         * @param setNumA
         * @param setNumB
         * @param iBondSetA
         * @param iBondSetB
         * @param unmappedAtomsMolA
         * @param newMapping
         * @param counter
         */
        protected internal void Process(int setNumA, int setNumB, IList<int> iBondSetA, IList<int> iBondSetB,
                IList<int> unmappedAtomsMolA, IList<int> newMapping, int counter)
        {

            //
            //            int newMapingSize,
            //            List<int> new_iBondSetA,
            //            List<string> new_cBondSetA,
            bool bondConsidered = false;
            bool normalBond = true;

            for (int atomIndex = 0; atomIndex < setNumA; atomIndex++)
            {
                int indexI = iBondSetA[atomIndex * 3 + 0];
                int indexJ = iBondSetA[atomIndex * 3 + 1];
                int order = iBondSetA[atomIndex * 3 + 2];

                foreach (var unMappedAtomIndex in unmappedAtomsMolA)
                {
                    if (unMappedAtomIndex.Equals(indexI))
                    {
                        normalBond = UnMappedAtomsEqualsIndexJ(setNumA, setNumB, iBondSetA, iBondSetB, atomIndex,
                                counter, newMapping, indexI, indexJ, order);
                        bondConsidered = true;
                    }
                    else if (unMappedAtomIndex.Equals(indexJ))
                    {
                        normalBond = UnMappedAtomsEqualsIndexI(setNumA, setNumB, iBondSetA, iBondSetB, atomIndex,
                                counter, newMapping, indexI, indexJ, order);
                        bondConsidered = true;
                    }

                    if (normalBond && bondConsidered)
                    {
                        MarkNormalBonds(atomIndex, indexI, indexJ, order);
                        normalBond = true;
                        break;
                    }
                }
                bondConsidered = false;
            }
        }

        private int SearchCorrespondingAtom(int mappedAtomsSize, int atomFromOtherMolecule, int molecule,
                IList<int> mappedAtomsOrg)
        {

            List<int> mappedAtoms = new List<int>(mappedAtomsOrg);

            int correspondingAtom = 0;
            for (int a = 0; a < mappedAtomsSize; a++)
            {
                if ((molecule == 1) && (mappedAtoms[a * 2 + 0] == atomFromOtherMolecule))
                {
                    correspondingAtom = mappedAtoms[a * 2 + 1];
                }
                if ((molecule == 2) && (mappedAtoms[a * 2 + 1] == atomFromOtherMolecule))
                {
                    correspondingAtom = mappedAtoms[a * 2 + 0];
                }
            }
            return correspondingAtom;
        }

        private void MarkNormalBonds(int atomIndex, int indexI, int indexJ, int order)
        {
            newINeighborsA.Add(indexI);
            newINeighborsA.Add(indexJ);
            newINeighborsA.Add(order);
            newCNeighborsA.Add(cTab1Copy[atomIndex * 4 + 0]);
            newCNeighborsA.Add(cTab1Copy[atomIndex * 4 + 1]);
            newCNeighborsA.Add("X");
            newCNeighborsA.Add("X");
            setBondNumA++;
        }

        private void Step1(int atomIndex, int counter)
        {
            cBondNeighborsA.Add(cTab1Copy[atomIndex * 4 + 0]);
            cBondNeighborsA.Add(signs[counter]);
            cBondNeighborsA.Add("X");
            cBondNeighborsA.Add(cTab1Copy[atomIndex * 4 + 1]);
        }

        private void Step2(int atomIndex)
        {
            cBondNeighborsA.Add(cTab1Copy[atomIndex * 4 + 0]);
            cBondNeighborsA.Add(cTab1Copy[atomIndex * 4 + 1]);
            cBondNeighborsA.Add("X");
            cBondNeighborsA.Add(cTab1Copy[atomIndex * 4 + 3]);
        }

        private void Step3(int atomIndex, int counter)
        {
            cBondNeighborsA.Add(signs[counter]);
            cBondNeighborsA.Add(cTab1Copy[atomIndex * 4 + 1]);
            cBondNeighborsA.Add(cTab1Copy[atomIndex * 4 + 0]);
            cBondNeighborsA.Add("X");
        }

        private void Step4(int atomIndex)
        {
            cBondNeighborsA.Add(cTab1Copy[atomIndex * 4 + 0]);
            cBondNeighborsA.Add(cTab1Copy[atomIndex * 4 + 1]);
            cBondNeighborsA.Add(cTab1Copy[atomIndex * 4 + 2]);
            cBondNeighborsA.Add("X");
        }

        private bool UnMappedAtomsEqualsIndexJ(IAtomContainer query, IAtomContainer target, int atomIndex, int counter,
                IList<int> mappedAtoms, int indexI, int indexJ, int order)
        {
            bool normalBond = true;
            for (int c = 0; c < newNeighborNumA; c++)
            {

                if (mappedAtoms[c * 2].Equals(indexJ))
                {
                    SetBondNeighbors(indexI, indexJ, order);
                    if (string.Equals(cTab1Copy[atomIndex * 4 + 3], "X", StringComparison.OrdinalIgnoreCase))
                    {

                        Step1(atomIndex, counter);
                        McGregorChecks.ChangeCharBonds(indexI, signs[counter], query.Bonds.Count, query, cTab1Copy);

                        int corAtom = SearchCorrespondingAtom(newNeighborNumA, indexI, 1, mappedAtoms);
                        McGregorChecks.ChangeCharBonds(corAtom, signs[counter], target.Bonds.Count, target, cTab2Copy);
                        counter++;
                    }
                    else
                    {
                        Step2(atomIndex);
                    }
                    normalBond = false;
                    neighborBondNumA++;
                }
            }
            return normalBond;
        }

        private bool UnMappedAtomsEqualsIndexI(IAtomContainer query, IAtomContainer target, int atomIndex, int counter,
                IList<int> mappedAtoms, int indexI, int indexJ, int order)
        {

            bool normalBond = true;
            for (int c = 0; c < newNeighborNumA; c++)
            {

                if (mappedAtoms[c * 2 + 0].Equals(indexI))
                {
                    SetBondNeighbors(indexI, indexJ, order);
                    if (string.Equals(cTab1Copy[atomIndex * 4 + 2], "X", StringComparison.OrdinalIgnoreCase))
                    {
                        Step3(atomIndex, counter);
                        McGregorChecks.ChangeCharBonds(indexJ, signs[counter], query.Bonds.Count, query, cTab1Copy);

                        int corAtom = SearchCorrespondingAtom(newNeighborNumA, indexJ, 1, mappedAtoms);
                        McGregorChecks.ChangeCharBonds(corAtom, signs[counter], target.Bonds.Count, target, cTab2Copy);
                        counter++;
                    }
                    else
                    {
                        Step4(atomIndex);
                    }
                    normalBond = false;
                    neighborBondNumA++;
                    //Console.Out.WriteLine("Neighbor");
                    //Console.Out.WriteLine(neighborBondNumA);
                }
            }
            return normalBond;
        }

        private bool UnMappedAtomsEqualsIndexJ(int setNumA, int setNumB, IList<int> iBondSetA,
                IList<int> iBondSetB, int atomIndex, int counter, IList<int> newMapping, int indexI,
                int indexJ, int order)
        {
            bool normalBond = true;
            for (int c = 0; c < newNeighborNumA; c++)
            {

                if (newMapping[c * 2 + 0].Equals(indexJ))
                {

                    SetBondNeighbors(indexI, indexJ, order);
                    if (string.Equals(cTab1Copy[atomIndex * 4 + 3], "X", StringComparison.OrdinalIgnoreCase))
                    {
                        Step1(atomIndex, counter);
                        McGregorChecks.ChangeCharBonds(indexI, signs[counter], setNumA, iBondSetA, cTab1Copy);
                        int corAtom = McGregorChecks.SearchCorrespondingAtom(newNeighborNumA, indexI, 1, newMapping);
                        McGregorChecks.ChangeCharBonds(corAtom, signs[counter], setNumB, iBondSetB, cTab2Copy);
                        counter++;

                    }
                    else
                    {
                        Step2(atomIndex);
                    }
                    normalBond = false;
                    neighborBondNumA++;
                }
            }
            return normalBond;
        }

        private bool UnMappedAtomsEqualsIndexI(int setNumA, int setNumB, IList<int> iBondSetA,
                IList<int> iBondSetB, int atomIndex, int counter, IList<int> newMapping, int indexI,
                int indexJ, int order)
        {
            bool normalBond = true;
            for (int c = 0; c < newNeighborNumA; c++)
            {

                if (newMapping[c * 2 + 0].Equals(indexI))
                {

                    SetBondNeighbors(indexI, indexJ, order);
                    if (string.Equals(cTab1Copy[atomIndex * 4 + 2], "X", StringComparison.OrdinalIgnoreCase))
                    {
                        Step3(atomIndex, counter);
                        McGregorChecks.ChangeCharBonds(indexJ, signs[counter], setNumA, iBondSetA, cTab1Copy);
                        int corAtom = McGregorChecks.SearchCorrespondingAtom(newNeighborNumA, indexJ, 1, newMapping);
                        McGregorChecks.ChangeCharBonds(corAtom, signs[counter], setNumB, iBondSetB, cTab2Copy);
                        counter++;
                    }
                    else
                    {
                        Step4(atomIndex);
                    }

                    normalBond = false;
                    neighborBondNumA++;

                }
            }
            return normalBond;
        }

        private void SetBondNeighbors(int indexI, int indexJ, int order)
        {
            iBondNeighborsA.Add(indexI);
            iBondNeighborsA.Add(indexJ);
            iBondNeighborsA.Add(order);
        }

        /**
         *
         * @return cTabQuery copy
         */
        protected internal List<string> CTab1 => this.cTab1Copy;

        /**
         *
         * @return cTabTarget Copy
         */
        protected internal List<string> CTab2 => this.cTab2Copy;

        /**
         *
         * @return number of remaining molecule A bonds after the clique search,
         * which are neighbors of the MCS
         *
         */
        protected internal int NeighborBondNumA => this.neighborBondNumA;

        /**
         *
         * @return number of remaining molecule A bonds after the clique search,
         * which aren't neighbors
         */
        protected internal int BondNumA => this.setBondNumA;

        internal List<int> IBondNeighboursA => this.iBondNeighborsA;

        internal List<string> CBondNeighborsA => this.cBondNeighborsA;
    }
}
