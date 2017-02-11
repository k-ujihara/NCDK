/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your container code files, and to any copyright notice that you may distribute
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

namespace NCDK.SMSD.Algorithms.McGregors
{
    /**
     * Class to perform check/methods for McGregor class.
     * @cdk.module smsd
     * @cdk.githash
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     */
    public class McGregorChecks
    {

        /**
         *
         * @param source
         * @param target
         * @param neighborBondNumA
         * @param neighborBondNumB
         * @param iBondNeighborAtomsA
         * @param iBondNeighborAtomsB
         * @param cBondNeighborsA
         * @param cBondNeighborsB
         * @param shouldMatchBonds
         * @return
         */
        protected internal static bool IsFurtherMappingPossible(IAtomContainer source, IAtomContainer target,
                int neighborBondNumA, int neighborBondNumB, IList<int> iBondNeighborAtomsA,
                IList<int> iBondNeighborAtomsB, IList<string> cBondNeighborsA, IList<string> cBondNeighborsB,
                bool shouldMatchBonds)
        {

            for (int row = 0; row < neighborBondNumA; row++)
            {
                //            Console.Out.WriteLine("i " + row);
                string g1A = cBondNeighborsA[row * 4 + 0];
                string g2a = cBondNeighborsA[row * 4 + 1];

                for (int column = 0; column < neighborBondNumB; column++)
                {

                    string g1B = cBondNeighborsB[column * 4 + 0];
                    string g2B = cBondNeighborsB[column * 4 + 1];

                    if (IsAtomMatch(g1A, g2a, g1B, g2B))
                    {
                        try
                        {

                            int indexI = iBondNeighborAtomsA[row * 3 + 0];
                            int indexIPlus1 = iBondNeighborAtomsA[row * 3 + 1];

                            int indexJ = iBondNeighborAtomsB[column * 3 + 0];
                            int indexJPlus1 = iBondNeighborAtomsB[column * 3 + 1];

                            IAtom r1A = source.Atoms[indexI];
                            IAtom r2A = source.Atoms[indexIPlus1];
                            IBond reactantBond = source.GetBond(r1A, r2A);

                            IAtom p1B = target.Atoms[indexJ];
                            IAtom p2B = target.Atoms[indexJPlus1];
                            IBond productBond = target.GetBond(p1B, p2B);

                            if (IsMatchFeasible(source, reactantBond, target, productBond, shouldMatchBonds))
                            {
                                return true;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.Out.WriteLine(e.StackTrace);
                        }
                    }
                }
            }

            return false;
        }

        protected internal static bool IsMatchFeasible(IAtomContainer ac1, IBond bondA1, IAtomContainer ac2, IBond bondA2,
                bool shouldMatchBonds)
        {

            if (ac1 is IQueryAtomContainer)
            {
                if (((IQueryBond)bondA1).Matches(bondA2))
                {
                    IQueryAtom atom1 = (IQueryAtom)(bondA1.Atoms[0]);
                    IQueryAtom atom2 = (IQueryAtom)(bondA1.Atoms[1]);
                    // ok, bonds match
                    if (atom1.Matches(bondA2.Atoms[0]) && atom2.Matches(bondA2.Atoms[1])
                            || atom1.Matches(bondA2.Atoms[1]) && atom2.Matches(bondA2.Atoms[0]))
                    {
                        // ok, atoms match in either order
                        return true;
                    }
                    return false;
                }
                return false;
            }
            else
            {

                //Bond Matcher
                var bondMatcher = new Matchers.DefaultBondMatcher(ac1, bondA1, shouldMatchBonds);
                //Atom Matcher
                var atomMatcher1 = new Matchers.DefaultMCSPlusAtomMatcher(ac1, bondA1.Atoms[0], shouldMatchBonds);
                //Atom Matcher
                var atomMatcher2 = new Matchers.DefaultMCSPlusAtomMatcher(ac1, bondA1.Atoms[1], shouldMatchBonds);

                if (Matchers.DefaultMatcher.IsBondMatch(bondMatcher, ac2, bondA2, shouldMatchBonds)
                        && Matchers.DefaultMatcher.IsAtomMatch(atomMatcher1, atomMatcher2, ac2, bondA2, shouldMatchBonds))
                {
                    return true;
                }
                return false;
            }
        }

        /**
         *
         * @param mappedAtomsSize
         * @param atomFromOtherMolecule
         * @param molecule
         * @param mappedAtomsOrg
         * @return
         */
        protected internal static int SearchCorrespondingAtom(int mappedAtomsSize, int atomFromOtherMolecule, int molecule,
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

        /**
         *
         * @param g1A
         * @param g2A
         * @param g1B
         * @param g2B
         * @return
         */
        protected internal static bool IsAtomMatch(string g1A, string g2A, string g1B, string g2B)
        {
            if ((string.Equals(g1A, g1B, StringComparison.OrdinalIgnoreCase) && string.Equals(g2A, g2B, StringComparison.OrdinalIgnoreCase))
                    || (string.Equals(g1A, g2B, StringComparison.OrdinalIgnoreCase) && string.Equals(g2A, g1B, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
            return false;
        }

        /*
         * Modified function call by ASAD in Java have to check
         */
        protected internal static int RemoveTreeStructure(BinaryTree curStruc)
        {

            BinaryTree equalStruc = curStruc.Equal;
            BinaryTree notEqualStruc = curStruc.NotEqual;
            curStruc = null;

            if (equalStruc != null)
            {
                RemoveTreeStructure(equalStruc);
            }

            if (notEqualStruc != null)
            {
                RemoveTreeStructure(notEqualStruc);
            }

            return 0;
        }

        //Function compaires a structure array with itself. Sometimes a mapping occurs several times within the array.
        //The function eliminates these recurring mappings. Function is called in function best_solution.
        //The function is called by itself as long as the last list element is processed.
        /**
         *
         * @param atomMapping
         * @return
         */
        protected internal static IList<int> RemoveRecurringMappings(IList<int> atomMapping)
        {
            bool exist = true;
            List<int> tempMap = new List<int>();
            int tempCounter = 0;
            int atomMappingSize = atomMapping.Count;
            for (int x = 0; x < atomMappingSize; x += 2)
            {
                int atom = atomMapping[x];
                for (int y = x + 2; y < atomMappingSize; y += 2)
                {
                    if (atom == atomMapping[y])
                    {
                        exist = false;
                    }
                }
                if (exist == true)
                {
                    tempMap.Add(atomMapping[x + 0]);
                    tempMap.Add(atomMapping[x + 1]);
                    tempCounter += 2;
                }

                exist = true;
            }

            return tempMap;
        }

        /**
         * The function is called in function partsearch. The function is given a temporary matrix and a position (row/column)
         * within this matrix. First the function sets all entries to zero, which can be exlcuded in respect to the current
         * atom by atom matching. After this the function replaces all entries in the same row and column of the current
         * position by zeros. Only the entry of the current position is set to one.
         * Return value "count_arcsleft" counts the number of arcs, which are still in the matrix.
         * @param row
         * @param column
         * @param marcs
         * @param mcGregorHelper
         */
        protected internal static void RemoveRedundantArcs(int row, int column, IList<int> marcs, McgregorHelper mcGregorHelper)
        {
            int neighborBondNumA = mcGregorHelper.NeighborBondNumA;
            int neighborBondNumB = mcGregorHelper.NeighborBondNumB;
            var iBondNeighborAtomsA = mcGregorHelper.GetIBondNeighborAtomsA();
            var iBondNeighborAtomsB = mcGregorHelper.GetIBondNeighborAtomsB();
            int g1Atom = iBondNeighborAtomsA[row * 3 + 0];
            int g2Atom = iBondNeighborAtomsA[row * 3 + 1];
            int g3Atom = iBondNeighborAtomsB[column * 3 + 0];
            int g4Atom = iBondNeighborAtomsB[column * 3 + 1];

            for (int x = 0; x < neighborBondNumA; x++)
            {
                int rowAtom1 = iBondNeighborAtomsA[x * 3 + 0];
                int rowAtom2 = iBondNeighborAtomsA[x * 3 + 1];

                for (int y = 0; y < neighborBondNumB; y++)
                {
                    int columnAtom3 = iBondNeighborAtomsB[y * 3 + 0];
                    int columnAtom4 = iBondNeighborAtomsB[y * 3 + 1];

                    if (McGregorChecks.Cases(g1Atom, g2Atom, g3Atom, g4Atom, rowAtom1, rowAtom2, columnAtom3,
                            columnAtom4))
                    {
                        marcs[x * neighborBondNumB + y] = 0;
                    }

                }
            }

            for (int v = 0; v < neighborBondNumA; v++)
            {
                marcs[v * neighborBondNumB + column] = 0;
            }

            for (int w = 0; w < neighborBondNumB; w++)
            {
                marcs[row * neighborBondNumB + w] = 0;
            }

            marcs[row * neighborBondNumB + column] = 1;
        }

        /**
         *
         * @param bondNumber
         * @param cSet
         * @return
         */
        protected internal static List<string> GenerateCSetCopy(int bondNumber, IList<string> cSet)
        {
            List<string> cTabCopy = new List<string>();
            for (int a = 0; a < bondNumber; a++)
            {
                cTabCopy.Add(cSet[a * 4 + 0]);
                cTabCopy.Add(cSet[a * 4 + 1]);
                cTabCopy.Add("X");
                cTabCopy.Add("X");
            }
            return cTabCopy;
        }

        /**
         *
         * @param atomContainer
         * @return
         * @throws IOException
         */
        protected internal static List<string> GenerateCTabCopy(IAtomContainer atomContainer)
        {
            List<string> cTabCopy = new List<string>();
            foreach (var bond in atomContainer.Bonds)
            {
                string atomI = bond.Atoms[0].Symbol;
                string atomJ = bond.Atoms[1].Symbol;
                cTabCopy.Add(atomI);
                cTabCopy.Add(atomJ);
                cTabCopy.Add("X");
                cTabCopy.Add("X");
            }
            return cTabCopy;
        }

        /**
         *
         * @param g1Atom
         * @param g3Atom
         * @param g4Atom
         * @param rowAtom1
         * @param rowAtom2
         * @param columnAtom3
         * @param columnAtom4
         * @return
         */
        protected static bool Case1(int g1Atom, int g3Atom, int g4Atom, int rowAtom1, int rowAtom2,
                int columnAtom3, int columnAtom4)
        {
            if (((g1Atom == rowAtom1) || (g1Atom == rowAtom2))
                    && (!(((columnAtom3 == g3Atom) || (columnAtom4 == g3Atom)) || ((columnAtom3 == g4Atom) || (columnAtom4 == g4Atom)))))
            {
                return true;
            }
            return false;
        }

        /**
         *
         * @param g2Atom
         * @param g3Atom
         * @param g4Atom
         * @param rowAtom1
         * @param rowAtom2
         * @param columnAtom3
         * @param columnAtom4
         * @return
         */
        protected static bool Case2(int g2Atom, int g3Atom, int g4Atom, int rowAtom1, int rowAtom2,
                int columnAtom3, int columnAtom4)
        {
            if (((g2Atom == rowAtom1) || (g2Atom == rowAtom2))
                    && (!(((columnAtom3 == g3Atom) || (columnAtom4 == g3Atom)) || ((columnAtom3 == g4Atom) || (columnAtom4 == g4Atom)))))
            {
                return true;
            }
            return false;
        }

        /**
         *
         * @param g1Atom
         * @param g3Atom
         * @param g2Atom
         * @param rowAtom1
         * @param rowAtom2
         * @param columnAtom3
         * @param columnAtom4
         * @return
         */
        protected static bool Case3(int g1Atom, int g3Atom, int g2Atom, int rowAtom1, int rowAtom2,
                int columnAtom3, int columnAtom4)
        {
            if (((g3Atom == columnAtom3) || (g3Atom == columnAtom4))
                    && (!(((rowAtom1 == g1Atom) || (rowAtom2 == g1Atom)) || ((rowAtom1 == g2Atom) || (rowAtom2 == g2Atom)))))
            {
                return true;
            }
            return false;
        }

        /**
         *
         * @param g1Atom
         * @param g2Atom
         * @param g4Atom
         * @param rowAtom1
         * @param rowAtom2
         * @param columnAtom3
         * @param columnAtom4
         * @return
         */
        protected static bool Case4(int g1Atom, int g2Atom, int g4Atom, int rowAtom1, int rowAtom2,
                int columnAtom3, int columnAtom4)
        {
            if (((g4Atom == columnAtom3) || (g4Atom == columnAtom4))
                    && (!(((rowAtom1 == g1Atom) || (rowAtom2 == g1Atom)) || ((rowAtom1 == g2Atom) || (rowAtom2 == g2Atom)))))
            {
                return true;
            }
            return false;
        }

        /**
         *
         * @param g1Atom
         * @param g2Atom
         * @param g3Atom
         * @param g4Atom
         * @param rowAtom1
         * @param rowAtom2
         * @param columnAtom3
         * @param columnAtom4
         * @return
         */
        protected static bool Cases(int g1Atom, int g2Atom, int g3Atom, int g4Atom, int rowAtom1, int rowAtom2,
                int columnAtom3, int columnAtom4)
        {
            if (Case1(g1Atom, g3Atom, g4Atom, rowAtom1, rowAtom2, columnAtom3, columnAtom4)
                    || Case2(g2Atom, g3Atom, g4Atom, rowAtom1, rowAtom2, columnAtom3, columnAtom4)
                    || Case3(g1Atom, g3Atom, g2Atom, rowAtom1, rowAtom2, columnAtom3, columnAtom4)
                    || Case4(g1Atom, g2Atom, g4Atom, rowAtom1, rowAtom2, columnAtom3, columnAtom4))
            {
                return true;
            }
            return false;
        }

        /**
         *
         * @param source
         * @param target
         * @param neighborBondNumA
         * @param neighborBondNumB
         * @param iBondNeighborAtomsA
         * @param iBondNeighborAtomsB
         * @param cBondNeighborsA
         * @param cBondNeighborsB
         * @param modifiedARCS
         * @param shouldMatchBonds
         * @return
         */
        protected internal static List<int> SetArcs(IAtomContainer source, IAtomContainer target, int neighborBondNumA,
                int neighborBondNumB, List<int> iBondNeighborAtomsA, List<int> iBondNeighborAtomsB,
                List<string> cBondNeighborsA, List<string> cBondNeighborsB, List<int> modifiedARCS,
                bool shouldMatchBonds)
        {

            for (int row = 0; row < neighborBondNumA; row++)
            {
                for (int column = 0; column < neighborBondNumB; column++)
                {

                    string g1A = cBondNeighborsA[row * 4 + 0];
                    string g2A = cBondNeighborsA[row * 4 + 1];
                    string g1B = cBondNeighborsB[column * 4 + 0];
                    string g2B = cBondNeighborsB[column * 4 + 1];

                    if (McGregorChecks.IsAtomMatch(g1A, g2A, g1B, g2B))
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
                        if (IsMatchFeasible(source, reactantBond, target, productBond, shouldMatchBonds))
                        {
                            modifiedARCS[row * neighborBondNumB + column] = 1;
                        }
                    }
                }
            }
            return modifiedARCS;
        }

        /**
         *
         * @param tempmarcs
         * @param neighborBondNumA
         * @param neighborBondNumB
         * @return
         */
        protected internal static int CountArcsLeft(List<int> tempmarcs, int neighborBondNumA, int neighborBondNumB)
        {
            int arcsleft = 0;

            for (int a = 0; a < neighborBondNumA; a++)
            {
                for (int b = 0; b < neighborBondNumB; b++)
                {

                    if (tempmarcs[a * neighborBondNumB + b] == (1))
                    {
                        arcsleft++;
                    }
                }
            }
            return arcsleft;
        }

        /**
         *
         * @param correspondingAtom
         * @param newSymbol
         * @param neighborBondNum
         * @param atomContainer
         * @param cBondNeighbors
         * @return
         */
        protected internal static int ChangeCharBonds(int correspondingAtom, string newSymbol, int neighborBondNum,
                IAtomContainer atomContainer, IList<string> cBondNeighbors)
        {
            for (int atomIndex = 0; atomIndex < neighborBondNum; atomIndex++)
            {
                IBond bond = atomContainer.Bonds[atomIndex];
                if ((atomContainer.Atoms.IndexOf(bond.Atoms[0]) == correspondingAtom)
                        && string.Equals(cBondNeighbors[atomIndex * 4 + 2], "X", StringComparison.OrdinalIgnoreCase))
                {
                    cBondNeighbors[atomIndex * 4 + 2] = cBondNeighbors[atomIndex * 4 + 0];
                    cBondNeighbors[atomIndex * 4 + 0] = newSymbol;
                }

                if ((atomContainer.Atoms.IndexOf(bond.Atoms[1]) == correspondingAtom)
                        && string.Equals(cBondNeighbors[atomIndex * 4 + 3], "X", StringComparison.OrdinalIgnoreCase))
                {
                    cBondNeighbors[atomIndex * 4 + 3] = cBondNeighbors[atomIndex * 4 + 1];
                    cBondNeighbors[atomIndex * 4 + 1] = newSymbol;
                }
            }
            return 0;
        }

        /**
         *
         * @param correspondingAtom
         * @param newSymbol
         * @param neighborBondNum
         * @param iBondNeighbors
         * @param cBondNeighbors
         * @return
         */
        protected internal static int ChangeCharBonds(int correspondingAtom, string newSymbol, int neighborBondNum,
                IList<int> iBondNeighbors, IList<string> cBondNeighbors)
        {

            for (int atomIndex = 0; atomIndex < neighborBondNum; atomIndex++)
            {
                if ((iBondNeighbors[atomIndex * 3 + 0] == (correspondingAtom))
                        && string.Equals(cBondNeighbors[atomIndex * 4 + 2], "X", StringComparison.OrdinalIgnoreCase))
                {
                    cBondNeighbors[atomIndex * 4 + 2] = cBondNeighbors[atomIndex * 4 + 0];
                    cBondNeighbors[atomIndex * 4 + 0] = newSymbol;
                }

                if ((iBondNeighbors[atomIndex * 3 + 1] == (correspondingAtom))
                        && string.Equals(cBondNeighbors[atomIndex * 4 + 3], "X", StringComparison.OrdinalIgnoreCase))
                {
                    cBondNeighbors[atomIndex * 4 + 3] = cBondNeighbors[atomIndex * 4 + 1];
                    cBondNeighbors[atomIndex * 4 + 1] = newSymbol;
                }
            }
            return 0;
        }

        internal static bool IsFurtherMappingPossible(IAtomContainer source, IAtomContainer target,
                McgregorHelper mcGregorHelper, bool shouldMatchBonds)
        {

            int neighborBondNumA = mcGregorHelper.NeighborBondNumA;
            int neighborBondNumB = mcGregorHelper.NeighborBondNumB;
            var iBondNeighborAtomsA = mcGregorHelper.GetIBondNeighborAtomsA();
            var iBondNeighborAtomsB = mcGregorHelper.GetIBondNeighborAtomsB();
            var cBondNeighborsA = mcGregorHelper.GetCBondNeighborsA();
            var cBondNeighborsB = mcGregorHelper.GetCBondNeighborsB();

            for (int row = 0; row < neighborBondNumA; row++)
            {
                //            Console.Out.WriteLine("i " + row);
                string g1A = cBondNeighborsA[row * 4 + 0];
                string g2A = cBondNeighborsA[row * 4 + 1];

                for (int column = 0; column < neighborBondNumB; column++)
                {

                    string g1B = cBondNeighborsB[column * 4 + 0];
                    string g2B = cBondNeighborsB[column * 4 + 1];

                    if (IsAtomMatch(g1A, g2A, g1B, g2B))
                    {
                        try
                        {

                            int indexI = iBondNeighborAtomsA[row * 3 + 0];
                            int indexIPlus1 = iBondNeighborAtomsA[row * 3 + 1];

                            int indexJ = iBondNeighborAtomsB[column * 3 + 0];
                            int indexJPlus1 = iBondNeighborAtomsB[column * 3 + 1];

                            IAtom r1A = source.Atoms[indexI];
                            IAtom r2A = source.Atoms[indexIPlus1];
                            IBond reactantBond = source.GetBond(r1A, r2A);

                            IAtom p1B = target.Atoms[indexJ];
                            IAtom p2B = target.Atoms[indexJPlus1];
                            IBond productBond = target.GetBond(p1B, p2B);

                            if (IsMatchFeasible(source, reactantBond, target, productBond, shouldMatchBonds))
                            {
                                return true;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.Out.WriteLine(e.StackTrace);
                        }
                    }
                }
            }

            return false;
        }

        internal static List<int> MarkUnMappedAtoms(bool flag, IAtomContainer container, IDictionary<int, int> presentMapping)
        {
            List<int> unmappedMolAtoms = new List<int>();

            int unmappedNum = 0;
            bool atomIsUnmapped = true;

            for (int a = 0; a < container.Atoms.Count; a++)
            {
                //Atomic list are only numbers from 1 to atom_number1
                if (flag && presentMapping.ContainsKey(a))
                {
                    atomIsUnmapped = false;
                }
                else if (!flag && presentMapping.Values.Contains(a))
                {
                    atomIsUnmapped = false;
                }
                if (atomIsUnmapped)
                {
                    unmappedMolAtoms.Insert(unmappedNum++, a);
                }
                atomIsUnmapped = true;
            }
            return unmappedMolAtoms;
        }

        internal static List<int> MarkUnMappedAtoms(bool flag, IAtomContainer container, List<int> mappedAtoms,
                int cliqueSize)
        {
            List<int> unmappedMolAtoms = new List<int>();
            int unmappedNum = 0;
            bool atomIsUnmapped = true;
            for (int a = 0; a < container.Atoms.Count; a++)
            {
                //Atomic list are only numbers from 1 to atom_number1
                for (int b = 0; b < cliqueSize; b += 2)
                {
                    if (flag && mappedAtoms[b] == a)
                    {
                        atomIsUnmapped = false;
                    }
                    else if (!flag && mappedAtoms[b + 1] == a)
                    {
                        atomIsUnmapped = false;
                    }
                }
                if (atomIsUnmapped)
                {
                    unmappedMolAtoms.Insert(unmappedNum++, a);
                }
                atomIsUnmapped = true;
            }
            return unmappedMolAtoms;
        }
    }
}
