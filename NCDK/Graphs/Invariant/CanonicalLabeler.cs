/* Copyright (C) 2001-2007  Oliver Horlacher <oliver.horlacher@therastrat.com>
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
 *  */
using NCDK.Smiles;
using NCDK.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace NCDK.Graphs.Invariant
{
    /**
     * Canonically labels an atom container implementing
     * the algorithm published in David Weininger et al. {@cdk.cite WEI89}.
     * The Collections.Sort() method uses a merge sort which is
     * stable and runs in n Log(n).
     *
     * @cdk.module standard
     * @cdk.githash
     *
     * @author   Oliver Horlacher <oliver.horlacher@therastrat.com>
     * @cdk.created  2002-02-26
     *
     * @cdk.keyword canonicalization
     * @deprecated this labeller uses slow data structures and has been replaced - {@link Canon}
     */
    [Obsolete]
    public class CanonicalLabeler
    {

        public CanonicalLabeler() { }

        /**
		 * Canonically label the fragment.  The labels are set as atom property InvPair.CANONICAL_LABEL of type int, indicating the canonical order.
		 * This is an implementation of the algorithm published in
		 * David Weininger et.al. {@cdk.cite WEI89}.
		 *
		 * <p>The Collections.Sort() method uses a merge sort which is
		 * stable and runs in n Log(n).
		 *
		 * <p>It is assumed that a chemically valid AtomContainer is provided:
		 * this method does not check
		 * the correctness of the AtomContainer. Negative H counts will
		 * cause a FormatException to be thrown.
		 * @param atomContainer The molecule to label
		 */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CanonLabel(IAtomContainer atomContainer)
        {
            if (atomContainer.Atoms.Count == 0) return;
            if (atomContainer.Atoms.Count == 1)
            {
                atomContainer.Atoms[0].SetProperty(InvPair.CANONICAL_LABEL, 1);
            }

            List<InvPair> vect = CreateInvarLabel(atomContainer);
            Step3(vect, atomContainer);
        }

        /**
		 * @param v the invariance pair vector
		 */
        private void Step2(List<InvPair> v, IAtomContainer atoms)
        {
            PrimeProduct(v, atoms);
            Step3(v, atoms);
        }

        /**
		 * @param v the invariance pair vector
		 */
        private void Step3(List<InvPair> v, IAtomContainer atoms)
        {
            SortArrayList(v);
            RankArrayList(v);
            if (!IsInvPart(v))
            {
                Step2(v, atoms);
            }
            else
            {
                //On first pass save, partitioning as symmetry classes.
                if (((InvPair)v[v.Count - 1]).Curr < v.Count)
                {
                    BreakTies(v);
                    Step2(v, atoms);
                }
                // now apply the ranking
                foreach (var aV in v)
                {
                    ((InvPair)aV).Commit();
                }
            }
        }

        /**
		 * Create initial invariant labeling corresponds to step 1
		 *
		 * @return List containing the
		 */
        private List<InvPair> CreateInvarLabel(IAtomContainer atomContainer)
        {
            var atoms = atomContainer.Atoms;
            StringBuilder inv;
            List<InvPair> vect = new List<InvPair>();
            foreach (var a in atoms)
            {
                inv = new StringBuilder();
                var connectedAtoms = atomContainer.GetConnectedAtoms(a).ToList();
                inv.Append(connectedAtoms.Count
                        + (a.ImplicitHydrogenCount ?? 0)); //Num connections
                inv.Append(connectedAtoms.Count); //Num of non H bonds
                inv.Append(PeriodicTable.GetAtomicNumber(a.Symbol));

                double charge = a.Charge ?? 0;
                if (charge < 0) //Sign of charge
                    inv.Append(1);
                else
                    inv.Append(0); //Absolute charge
                inv.Append((int)Math.Abs((a.FormalCharge ?? 0))); //Hydrogen count
                inv.Append((a.ImplicitHydrogenCount ?? 0));
                vect.Add(new InvPair(long.Parse(inv.ToString()), a));
            }
            return vect;
        }

        /**
		 * Calculates the product of the neighbouring primes.
		 *
		 * @param v the invariance pair vector
		 */
        private void PrimeProduct(List<InvPair> v, IAtomContainer atomContainer)
        {
            long summ;
            foreach (var inv in v)
            {
                var neighbour = atomContainer.GetConnectedAtoms(inv.Atom);
                summ = 1;
                foreach (var a in neighbour)
                {
                    int next = a.GetProperty<InvPair>(InvPair.INVARIANCE_PAIR).Prime;
                    summ = summ * next;
                }
                inv.Last = inv.Curr;
                inv.Curr = summ;
            }
        }


        /**
		 * Sorts the vector according to the current invariance, corresponds to step 3
		 *
		 * @param v the invariance pair vector
		 * @cdk.todo    can this be done in one loop?
		 */
        private void SortArrayList(List<InvPair> v)
        {
            v.Sort(ASortArrayListCompareComparer);
            //v.Sort(SortArrayListCompareComparerCurr);
            //v.Sort(SortArrayListCompareComparerLast);
        }

        static SortArrayListCompareComparer ASortArrayListCompareComparer = new SortArrayListCompareComparer();
        class SortArrayListCompareComparer : IComparer<InvPair>
        {
            public int Compare(InvPair o1, InvPair o2)
            {
                if (o1.Last > o2.Last) return +1;
                if (o1.Last < o2.Last) return -1;
                if (o1.Curr > o2.Curr) return +1;
                if (o1.Curr < o2.Curr) return -1;
                return 0;
            }
        }

        //static SortArrayListCompareCurr SortArrayListCompareComparerCurr = new SortArrayListCompareCurr();
        //class SortArrayListCompareCurr : IComparer<InvPair>
        //{
        //    public int Compare(InvPair o1, InvPair o2)
        //    {
        //        if (o1.Curr > o2.Curr) return +1;
        //        if (o1.Curr < o2.Curr) return -1;
        //        return 0;
        //    }
        //}

        //static SortArrayListCompareLast SortArrayListCompareComparerLast = new SortArrayListCompareLast();
        //class SortArrayListCompareLast : IComparer<InvPair>
        //{
        //    public int Compare(InvPair o1, InvPair o2)
        //    {
        //        if (o1.Last > o2.Last) return +1;
        //        if (o1.Last < o2.Last) return -1;
        //        return 0;
        //    }
        //}

        /**
		 * Rank atomic vector, corresponds to step 4.
		 *
		 *  @param v the invariance pair vector
		 */
        private void RankArrayList(List<InvPair> v)
        {
            int num = 1;
            var temp = new int[v.Count];
            InvPair last = (InvPair)v[0];
            int x;
            x = 0;
            foreach (var curr in v)
            {
                if (!last.Equals(curr))
                {
                    num++;
                }
                temp[x++] = num;
                last = curr;
            }
            x = 0;
            foreach (var curr in v)
            {
                curr.Curr = temp[x++];
                curr.SetPrime();
            }
        }

        /**
		 * Checks to see if the vector is invariantly partitioned
		 *
		 * @param v the invariance pair vector
		 * @return true if the vector is invariantly partitioned, false otherwise
		 */
        private bool IsInvPart(List<InvPair> v)
        {
            if (v[v.Count - 1].Curr == v.Count) return true;
            foreach (var curr in v)
            {
                if (curr.Curr != curr.Last) return false;
            }
            return true;
        }

        /**
		 * Break ties. Corresponds to step 7
		 *
		 * @param v the invariance pair vector
		 */
        private void BreakTies(List<InvPair> v)
        {
            InvPair last = null;
            int tie = 0;
            bool found = false;
            int x;
            x = 0;
            foreach (var curr in v)
            {
                curr.Curr = curr.Curr * 2;
                curr.SetPrime();
                if (x != 0 && !found && curr.Curr == last.Curr)
                {
                    tie = x - 1;
                    found = true;
                }
                last = curr;
                x++;
            }
            var v_tie = v[tie];
            v_tie.Curr = v_tie.Curr - 1;
            v_tie.SetPrime();
        }
    }
}
