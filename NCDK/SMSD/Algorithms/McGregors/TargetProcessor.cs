/* Copyright (C) 2005-2006  Markus Leber
 *               2006-2009  Syed Asad Rahman <asad@ebi.ac.uk>
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
using System;
using System.Collections.Generic;

namespace NCDK.SMSD.Algorithms.McGregors
{
    /// <summary>
    /// Class to handle mappings of target molecule based on the query.
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    /// </summary>
    public class TargetProcessor
    {

        private List<string> cTab1Copy;
        private List<string> cTab2Copy;
        private string[] signArray;
        //number of remaining molecule A bonds after the clique search, which are
        //neighbors of the MCS
        private int neighborBondNumB = 0;
        //number of remaining molecule A bonds after the clique search, which aren't
        //neighbors
        private int setBondNumB = 0;
        private List<int> iBondNeighborsB;
        private List<string> cBondNeighborsB;
        private int newNeighborNumA;
        private List<int> newINeighborsA;
        private List<string> newCNeighborsA;

        /// <summary>
        ///
        /// <param name="cTab1Copy">/// @param cTab2Copy</param>
        /// <param name="signArray">/// @param neighborBondnumB</param>
        /// <param name="setBondnumB">/// @param iBondNeighborsB</param>
        /// <param name="cBondNeighborsB">/// @param newNeighborNumA</param>
        /// <param name="newINeighborsA">/// @param newCNeighborsA</param>
        /// </summary>
        protected internal TargetProcessor(List<string> cTab1Copy, List<string> cTab2Copy, string[] signArray,
                int neighborBondnumB, int setBondnumB, List<int> iBondNeighborsB, List<string> cBondNeighborsB,
                int newNeighborNumA, List<int> newINeighborsA, List<string> newCNeighborsA)
        {

            this.cTab1Copy = cTab1Copy;
            this.cTab2Copy = cTab2Copy;
            this.signArray = signArray;
            this.neighborBondNumB = neighborBondnumB;
            this.setBondNumB = setBondnumB;
            this.iBondNeighborsB = iBondNeighborsB;
            this.cBondNeighborsB = cBondNeighborsB;
            this.newNeighborNumA = newNeighborNumA;
            this.newCNeighborsA = newCNeighborsA;
            this.newINeighborsA = newINeighborsA;
        }

        protected internal void Process(IAtomContainer target, IList<int> unmappedAtomsMolB, int mappingSize,
                IList<int> iBondSetB, IList<string> cBondSetB, IList<int> mappedAtoms, int counter)
        {

            int unmappedNumB = unmappedAtomsMolB.Count;
            bool bondConsidered = false;
            bool normalBond = true;

            for (int atomIndex = 0; atomIndex < target.Bonds.Count; atomIndex++)
            {

                int indexI = target.Atoms.IndexOf(target.Bonds[atomIndex].Atoms[0]);
                int indexJ = target.Atoms.IndexOf(target.Bonds[atomIndex].Atoms[1]);
                int order = target.Bonds[atomIndex].Order.Numeric;

                for (int b = 0; b < unmappedNumB; b++)
                {
                    if (unmappedAtomsMolB[b].Equals(indexI))
                    {
                        normalBond = UnMappedAtomsEqualsIndexI(target, mappingSize, atomIndex, counter, mappedAtoms,
                                indexI, indexJ, order);
                        bondConsidered = true;
                    }
                    else if (unmappedAtomsMolB[b] == indexJ)
                    {
                        normalBond = UnMappedAtomsEqualsIndexJ(target, mappingSize, atomIndex, counter, mappedAtoms,
                                indexI, indexJ, order);
                        bondConsidered = true;
                    }

                    if (normalBond && bondConsidered)
                    {
                        MarkNormalBonds(atomIndex, iBondSetB, cBondSetB, indexI, indexJ, order);
                        normalBond = true;
                        break;
                    }

                }
                bondConsidered = false;
            }

        }

        /// <summary>
        ///
        /// <param name="setNumB">/// @param unmappedAtomsMolB</param>
        /// <param name="newMappingSize">/// @param iBondSetB</param>
        /// <param name="cBondSetB">/// @param newMapping</param>
        /// <param name="counter">/// @param newIBondSetB</param>
        /// <param name="newCBondSetB">/// </summary></param>
        protected internal void Process(int setNumB, IList<int> unmappedAtomsMolB, int newMappingSize,
                IList<int> iBondSetB, IList<string> cBondSetB, IList<int> newMapping, int counter,
                IList<int> newIBondSetB, IList<string> newCBondSetB)
        {

            //The special signs must be transfered to the corresponding atoms of molecule A

            bool bondConsidered = false;
            bool normalBond = true;
            for (int atomIndex = 0; atomIndex < setNumB; atomIndex++)
            {

                int indexI = iBondSetB[atomIndex * 3 + 0];
                int indexJ = iBondSetB[atomIndex * 3 + 1];
                int order = iBondSetB[atomIndex * 3 + 2];

                foreach (var unMappedAtomIndex in unmappedAtomsMolB)
                {
                    if (unMappedAtomIndex.Equals(indexI))
                    {
                        normalBond = UnMappedAtomsEqualsIndexI(setNumB, iBondSetB, newMappingSize, atomIndex, counter,
                                newMapping, indexI, indexJ, order);
                        bondConsidered = true;
                    }
                    else if (unMappedAtomIndex.Equals(indexJ))
                    {
                        normalBond = UnMappedAtomsEqualsIndexJ(setNumB, iBondSetB, newMappingSize, atomIndex, counter,
                                newMapping, indexI, indexJ, order);
                        bondConsidered = true;
                    }
                    if (normalBond && bondConsidered)
                    {
                        MarkNormalBonds(atomIndex, newIBondSetB, newCBondSetB, indexI, indexJ, order);
                        normalBond = true;
                        break;
                    }

                }
                bondConsidered = false;
            }
        }

        private bool UnMappedAtomsEqualsIndexI(IAtomContainer target, int mappingSize, int atomIndex, int counter,
                IList<int> mappedAtoms, int indexI, int indexJ, int order)
        {
            bool normalBond = true;
            for (int c = 0; c < mappingSize; c++)
            {
                if (mappedAtoms[c * 2 + 1].Equals(indexJ))
                {
                    SetBondNeighbors(indexI, indexJ, order);
                    if (string.Equals(cTab2Copy[atomIndex * 4 + 3], "X", StringComparison.OrdinalIgnoreCase))
                    {
                        Step1(atomIndex, counter);
                        McGregorChecks
                                .ChangeCharBonds(indexJ, signArray[counter], target.Bonds.Count, target, cTab2Copy);
                        int corAtom = McGregorChecks.SearchCorrespondingAtom(mappingSize, indexJ, 2, mappedAtoms);
                        //Commented by Asad
                        McGregorChecks.ChangeCharBonds(corAtom, signArray[counter], newNeighborNumA, newINeighborsA,
                                newCNeighborsA);
                        //                                ChangeCharBonds(corAtom, signArray[counter], query.Bonds.Count, query, cTab1Copy);
                        counter++;
                    }
                    else
                    {
                        Step2(atomIndex);
                    }
                    normalBond = false;
                    neighborBondNumB++;
                }
            }
            return normalBond;
        }

        private bool UnMappedAtomsEqualsIndexJ(IAtomContainer target, int mappingSize, int atomIndex, int counter,
                IList<int> mappedAtoms, int indexI, int indexJ, int order)
        {
            bool normalBond = true;
            for (int c = 0; c < mappingSize; c++)
            {
                if (mappedAtoms[c * 2 + 1].Equals(indexI))
                {
                    SetBondNeighbors(indexI, indexJ, order);
                    if (string.Equals(cTab2Copy[atomIndex * 4 + 2], "X", StringComparison.OrdinalIgnoreCase))
                    {
                        Step3(atomIndex, counter);
                        McGregorChecks
                                .ChangeCharBonds(indexI, signArray[counter], target.Bonds.Count, target, cTab2Copy);
                        int corAtom = McGregorChecks.SearchCorrespondingAtom(mappingSize, indexI, 2, mappedAtoms);
                        McGregorChecks.ChangeCharBonds(corAtom, signArray[counter], newNeighborNumA, newINeighborsA,
                                newCNeighborsA);
                        //                                ChangeCharBonds(corAtom, signArray[counter], query.Bonds.Count, query, cTab1Copy);
                        counter++;
                    }
                    else
                    {
                        Step4(atomIndex);
                    }
                    normalBond = false;
                    neighborBondNumB++;
                }
            }

            return normalBond;
        }

        private bool UnMappedAtomsEqualsIndexI(int setNumB, IList<int> iBondSetB, int newMappingSize,
                int atomIndex, int counter, IList<int> newMapping, int indexI, int indexJ, int order)
        {
            bool normalBond = true;
            for (int c = 0; c < newMappingSize; c++)
            {
                if (newMapping[c * 2 + 1].Equals(indexJ))
                {
                    SetBondNeighbors(indexI, indexJ, order);
                    if (string.Equals(cTab2Copy[atomIndex * 4 + 3], "X", StringComparison.OrdinalIgnoreCase))
                    {
                        Step1(atomIndex, counter);
                        McGregorChecks.ChangeCharBonds(indexJ, signArray[counter], setNumB, iBondSetB, cTab2Copy);
                        int corAtom = McGregorChecks.SearchCorrespondingAtom(newMappingSize, indexJ, 2, newMapping);
                        McGregorChecks.ChangeCharBonds(corAtom, signArray[counter], newNeighborNumA, newINeighborsA,
                                newCNeighborsA);
                        counter++;

                    }
                    else
                    {
                        Step2(atomIndex);
                    }

                    normalBond = false;
                    neighborBondNumB++;

                }
            }
            return normalBond;
        }

        private bool UnMappedAtomsEqualsIndexJ(int setNumB, IList<int> iBondSetB, int newMappingSize,
                int atomIndex, int counter, IList<int> newMapping, int indexI, int indexJ, int order)
        {
            bool normalBond = true;
            for (int c = 0; c < newMappingSize; c++)
            {
                if (newMapping[c * 2 + 1].Equals(indexI))
                {
                    SetBondNeighbors(indexI, indexJ, order);

                    if (string.Equals(cTab2Copy[atomIndex * 4 + 2], "X", StringComparison.OrdinalIgnoreCase))
                    {

                        Step3(atomIndex, counter);
                        McGregorChecks.ChangeCharBonds(indexI, signArray[counter], setNumB, iBondSetB, cTab2Copy);
                        int corAtom = McGregorChecks.SearchCorrespondingAtom(newMappingSize, indexI, 2, newMapping);
                        McGregorChecks.ChangeCharBonds(corAtom, signArray[counter], newNeighborNumA, newINeighborsA,
                                newCNeighborsA);
                        counter++;
                    }
                    else
                    {
                        Step4(atomIndex);
                    }

                    normalBond = false;
                    neighborBondNumB++;

                }
            }

            return normalBond;
        }

        private void MarkNormalBonds(int atomIndex, IList<int> iBondSetB, IList<string> cBondSetB, int indexI,
                int indexJ, int order)
        {
            iBondSetB.Add(indexI);
            iBondSetB.Add(indexJ);
            iBondSetB.Add(order);
            cBondSetB.Add(cTab2Copy[atomIndex * 4 + 0]);
            cBondSetB.Add(cTab2Copy[atomIndex * 4 + 1]);
            cBondSetB.Add("X");
            cBondSetB.Add("X");
            setBondNumB++;
        }

        private void SetBondNeighbors(int indexI, int indexJ, int order)
        {
            iBondNeighborsB.Add(indexI);
            iBondNeighborsB.Add(indexJ);
            iBondNeighborsB.Add(order);
        }

        private void Step1(int atomIndex, int counter)
        {
            cBondNeighborsB.Add(cTab2Copy[atomIndex * 4 + 0]);
            cBondNeighborsB.Add(signArray[counter]);
            cBondNeighborsB.Add("X");
            cBondNeighborsB.Add(cTab2Copy[atomIndex * 4 + 1]);
        }

        private void Step2(int atomIndex)
        {
            cBondNeighborsB.Add(cTab2Copy[atomIndex * 4 + 0]);
            cBondNeighborsB.Add(cTab2Copy[atomIndex * 4 + 1]);
            cBondNeighborsB.Add("X");
            cBondNeighborsB.Add(cTab2Copy[atomIndex * 4 + 3]);
        }

        private void Step3(int atomIndex, int counter)
        {
            cBondNeighborsB.Add(signArray[counter]);
            cBondNeighborsB.Add(cTab2Copy[atomIndex * 4 + 1]);
            cBondNeighborsB.Add(cTab2Copy[atomIndex * 4 + 0]);
            cBondNeighborsB.Add("X");
        }

        private void Step4(int atomIndex)
        {
            cBondNeighborsB.Add(cTab2Copy[atomIndex * 4 + 0]);
            cBondNeighborsB.Add(cTab2Copy[atomIndex * 4 + 1]);
            cBondNeighborsB.Add(cTab2Copy[atomIndex * 4 + 2]);
            cBondNeighborsB.Add("X");
        }

        /// <summary>
        ///
        /// <returns>/// </summary></returns>
        protected internal List<string> CTab1 => this.cTab1Copy;

        /// <summary>
        ///
        /// <returns>/// </summary></returns>
        protected internal List<string> CTab2 => this.cTab2Copy;

        /// <summary>
        ///
        /// <returns>number of remaining molecule A bonds after the clique search,</returns>
        /// which are neighbors of the MCS
        ///
        /// </summary>
        protected internal int NeighborBondNumB => this.neighborBondNumB;

        /// <summary>
        ///
        /// <returns>number of remaining molecule A bonds after the clique search,</returns>
        /// which aren't neighbors
        /// </summary>
        protected internal int BondNumB => this.setBondNumB;

        internal List<int> IBondNeighboursB => this.iBondNeighborsB;

        internal List<string> CBondNeighborsB => this.cBondNeighborsB;
    }
}
