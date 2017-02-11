/* 
 *  Copyright (C) 2002-2007  The Chemistry Development Kit (CDK) project
 *                     2014  Mark B Vine (orcid:0000-0002-7794-0426)
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All I ask is that proper credit is given for my work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Base;
using NCDK.Aromaticities;
using NCDK.AtomTypes;
using NCDK.Graphs;
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCDK.SGroups
{
    /**
     * Tool that tries to deduce bond orders based on connectivity and hybridization
     * for a number of common ring systems of up to seven-membered rings. It assumes
     * that atom types have been perceived before that class is used.
     *
     * <p>The calculation can be interrupted with {@link #SetInterrupted(bool)},
     * but assumes that this class is not used in a threaded fashion. When a calculation
     * is interrupted, the bool is reset to false.
     *
     * @author Todd Martin
     * @cdk.module smiles
     * @cdk.githash
     * @cdk.keyword bond order
     *
     * @cdk.bug 1895805
     * @cdk.bug 1931262
     *
     * @cdk.threadnonsafe
     * @deprecated Use the newer {@link org.openscience.cdk.aromaticity.Kekulization}
     */
    [Obsolete]
    public class DeduceBondSystemTool
    {

        private AllRingsFinder allRingsFinder;

        private IList<int[]> listOfRings = null;

        /// <summary>
        /// <see langword="true"/> if the next or running calculation should be interrupted.
        /// </summary>
        public bool Interrupted;

        /**
         * Constructor for the DeduceBondSystemTool object.
         */
        public DeduceBondSystemTool()
        {
            allRingsFinder = new AllRingsFinder();
        }

        /**
         * Constructor for the DeduceBondSystemTool object accepting a custom {@link AllRingsFinder}.
         *
         * @param ringFinder a custom {@link AllRingsFinder}.
         */
        public DeduceBondSystemTool(AllRingsFinder ringFinder)
        {
            allRingsFinder = ringFinder;
        }

        /**
         * Determines if, according to the algorithms implemented in this class, the given
         * AtomContainer has properly distributed double bonds.
         *
         * @param  m <see cref="IAtomContainer"/> to check the bond orders for.
         * @return true, if bond orders are properly distributed
         * @ thrown when something went wrong
         */
        public bool IsOK(IAtomContainer m)
        {
            // OK, we take advantage here from the fact that this class does not take
            // into account rings larger than 7 atoms. See FixAromaticBondOrders().
            IRingSet rs = allRingsFinder.FindAllRings(m, 7);
            StoreRingSystem(m, rs);
            bool StructureOK = this.IsStructureOK(m);
            IRingSet irs = this.RemoveExtraRings(m);

            if (irs == null) throw new CDKException("error in AllRingsFinder.findAllRings");

            int count = this.GetBadCount(m, irs);

            return StructureOK && count == 0;
        }

        /**
         * Added missing bond orders based on atom type information.
         *
         * @param atomContainer <see cref="IAtomContainer"/> for which to distribute double bond orders
         * @return a <see cref="IAtomContainer"/> with assigned double bonds.
         * @ if something went wrong.
         */
        public IAtomContainer FixAromaticBondOrders(IAtomContainer atomContainer)
        {
            // OK, we take advantage here from the fact that this class does not take
            // into account rings larger than 7 atoms. See FixAromaticBondOrders().
            IRingSet rs = allRingsFinder.FindAllRings(atomContainer, 7);
            StoreRingSystem(atomContainer, rs);

            IRingSet ringSet;

            // TODO remove rings with nonsp2 Carbons(?) and rings larger than 7 atoms
            ringSet = RemoveExtraRings(atomContainer);

            if (ringSet == null) throw new CDKException("failure in AllRingsFinder.findAllRings");

            IList<IList<IList<string>>> MasterList = new List<IList<IList<string>>>();

            //this.counter=0;// counter which keeps track of all current possibilities for placing double bonds

            this.FixPyridineNOxides(atomContainer, ringSet);

            for (int i = 0; i <= ringSet.Count - 1; i++)
            {

                IRing ring = (IRing)ringSet[i];

                // only takes into account rings up to 7 atoms... if that changes,
                // make sure to update the FindAllRings() calls too!
                if (ring.Atoms.Count == 5)
                {
                    FiveMemberedRingPossibilities(atomContainer, ring, MasterList);
                }
                else if (ring.Atoms.Count == 6)
                {
                    SixMemberedRingPossibilities(atomContainer, ring, MasterList);
                }
                else if (ring.Atoms.Count == 7)
                {
                    SevenMemberedRingPossibilities(atomContainer, ring, MasterList);
                    //TODO- add code for all 7 membered aromatic ring possibilities not just 3 bonds
                }
                else
                {
                    //TODO: what about other rings systems?
                    Debug.WriteLine("Found ring of size: " + ring.Atoms.Count);
                }
            }

            IAtomContainerSet<IAtomContainer> som = atomContainer.Builder.CreateAtomContainerSet();

            //		int number=1; // total number of possibilities
            //
            //		for (int ii=0;ii<=MasterList.Count-1;ii++) {
            //		List ringlist=(List)MasterList[ii];
            //		number*=ringlist.Count;
            //		}
            //		Debug.WriteLine("number= "+number);

            int[] choices;

            //if (number> 1000000) return null;

            choices = new int[MasterList.Count];

            if (MasterList.Count > 0)
            {
                IAtomContainer iAtomContainer = Loop(DateTime.Now.Ticks, atomContainer, 0, MasterList, choices, som);
                if (iAtomContainer != null) return iAtomContainer;
            }

            int mincount = 99999999;

            int best = -1; // one with minimum number of bad atoms

            // minimize number of potentially bad nitrogens among AtomContainers in the set

            for (int i = 0; i <= som.Count - 1; i++)
            {

                IAtomContainer mol = som[i];

                ringSet = RemoveExtraRings(mol);

                if (ringSet == null) continue;

                int count = GetBadCount(mol, ringSet);

                //Debug.WriteLine(i + "\t" + count);

                if (count < mincount)
                {
                    mincount = count;
                    best = i;
                }

            }

            if (som.Count > 0) return som[best];
            return atomContainer;
        }

        private void FixPyridineNOxides(IAtomContainer atomContainer, IRingSet ringSet)
        {

            //convert n(=O) to [n+][O-]

            for (int i = 0; i < atomContainer.Atoms.Count; i++)
            {
                IAtom ai = atomContainer.Atoms[i];

                if (ai.Symbol.Equals("N") && (ai.FormalCharge == null || ai.FormalCharge == 0))
                {
                    if (InRingSet(ai, ringSet))
                    {
                        IEnumerable<IAtom> ca = atomContainer.GetConnectedAtoms(ai);
                        foreach (var caj in ca)
                        {
                            if (caj.Symbol.Equals("O")
                                    && atomContainer.GetBond(ai, caj).Order == BondOrder.Double)
                            {
                                ai.FormalCharge = 1;
                                caj.FormalCharge = -1;
                                atomContainer.GetBond(ai, caj).Order = BondOrder.Single;
                            }
                        }// end for (int j=0;j<ca.Count;j++)

                    } // end if (InRingSet(ai,ringSet)) {
                } // end if (ai.Symbol.Equals("N") && ai.FormalCharge==0)

            } // end for (int i=0;i<atomContainer.Atoms.Count;i++)

        }

        private void ApplyBonds(IAtomContainer m, IList<string> al)
        {

            //Debug.WriteLine("");

            for (int i = 0; i <= al.Count - 1; i++)
            {

                string s = al[i];
                string s1 = s.Substring(0, s.IndexOf('-'));
                string s2 = s.Substring(s.IndexOf('-') + 1);

                int i1 = int.Parse(s1);
                int i2 = int.Parse(s2);

                //Debug.WriteLine(s1+"\t"+s2);

                IBond b = m.GetBond(m.Atoms[i1], m.Atoms[i2]);
                b.Order = BondOrder.Double;

            }

        }

        private void FiveMemberedRingPossibilities(IAtomContainer m, IRing r, IList<IList<IList<string>>> MasterList)
        {
            // 5 possibilities for placing 2 double bonds
            // 5 possibilities for placing 1 double bond

            int[] num = new int[5]; // stores atom numbers based on atom numbers in AtomContainer instead of ring

            for (int j = 0; j <= 4; j++)
            {
                num[j] = m.Atoms.IndexOf(r.Atoms[j]);
                //Debug.WriteLine(num[j]);
            }

            List<string> al1 = new List<string>();
            List<string> al2 = new List<string>();
            List<string> al3 = new List<string>();
            List<string> al4 = new List<string>();
            List<string> al5 = new List<string>();

            List<string> al6 = new List<string>();
            List<string> al7 = new List<string>();
            List<string> al8 = new List<string>();
            List<string> al9 = new List<string>();
            List<string> al10 = new List<string>();

            al1.Add(num[1] + "-" + num[2]);
            al1.Add(num[3] + "-" + num[4]);

            al2.Add(num[2] + "-" + num[3]);
            al2.Add(num[0] + "-" + num[4]);

            al3.Add(num[0] + "-" + num[1]);
            al3.Add(num[3] + "-" + num[4]);

            al4.Add(num[0] + "-" + num[4]);
            al4.Add(num[1] + "-" + num[2]);

            al5.Add(num[0] + "-" + num[1]);
            al5.Add(num[2] + "-" + num[3]);

            al6.Add(num[0] + "-" + num[1]);
            al7.Add(num[1] + "-" + num[2]);
            al8.Add(num[2] + "-" + num[3]);
            al9.Add(num[3] + "-" + num[4]);
            al10.Add(num[4] + "-" + num[0]);

            List<IList<string>> mal = new List<IList<string>>();

            mal.Add(al1);
            mal.Add(al2);
            mal.Add(al3);
            mal.Add(al4);
            mal.Add(al5);

            mal.Add(al6);
            mal.Add(al7);
            mal.Add(al8);
            mal.Add(al9);
            mal.Add(al10);

            //		mal.Add(al11);

            MasterList.Add(mal);

        }

        private void SixMemberedRingPossibilities(IAtomContainer m, IRing r, IList<IList<IList<string>>> MasterList)
        {
            // 2 possibilities for placing 3 double bonds
            // 6 possibilities for placing 2 double bonds
            // 6 possibilities for placing 1 double bonds

            IAtom[] ringatoms = new IAtom[6];

            ringatoms[0] = r.Atoms[0];

            int[] num = new int[6];

            for (int j = 0; j <= 5; j++)
            {
                num[j] = m.Atoms.IndexOf(r.Atoms[j]);
            }

            List<string> al1 = new List<string>();
            List<string> al2 = new List<string>();

            al1.Add(num[0] + "-" + num[1]);
            al1.Add(num[2] + "-" + num[3]);
            al1.Add(num[4] + "-" + num[5]);

            al2.Add(num[1] + "-" + num[2]);
            al2.Add(num[3] + "-" + num[4]);
            al2.Add(num[5] + "-" + num[0]);

            List<string> al3 = new List<string>();
            List<string> al4 = new List<string>();
            List<string> al5 = new List<string>();
            List<string> al6 = new List<string>();
            List<string> al7 = new List<string>();
            List<string> al8 = new List<string>();
            List<string> al9 = new List<string>();
            List<string> al10 = new List<string>();
            List<string> al11 = new List<string>();

            List<string> al12 = new List<string>();
            List<string> al13 = new List<string>();
            List<string> al14 = new List<string>();
            List<string> al15 = new List<string>();
            List<string> al16 = new List<string>();
            List<string> al17 = new List<string>();

            List<string> al18 = new List<string>();

            al3.Add(num[0] + "-" + num[1]);
            al3.Add(num[2] + "-" + num[3]);

            al4.Add(num[0] + "-" + num[1]);
            al4.Add(num[4] + "-" + num[5]);

            al5.Add(num[1] + "-" + num[2]);
            al5.Add(num[3] + "-" + num[4]);

            al6.Add(num[1] + "-" + num[2]);
            al6.Add(num[0] + "-" + num[5]);

            al7.Add(num[2] + "-" + num[3]);
            al7.Add(num[4] + "-" + num[5]);

            al8.Add(num[0] + "-" + num[5]);
            al8.Add(num[3] + "-" + num[4]);

            al9.Add(num[0] + "-" + num[1]);
            al9.Add(num[3] + "-" + num[4]);

            al10.Add(num[1] + "-" + num[2]);
            al10.Add(num[4] + "-" + num[5]);

            al11.Add(num[2] + "-" + num[3]);
            al11.Add(num[0] + "-" + num[5]);

            al12.Add(num[0] + "-" + num[1]);
            al13.Add(num[1] + "-" + num[2]);
            al14.Add(num[2] + "-" + num[3]);
            al15.Add(num[3] + "-" + num[4]);
            al16.Add(num[4] + "-" + num[5]);
            al17.Add(num[5] + "-" + num[0]);

            List<IList<string>> mal = new List<IList<string>>();

            mal.Add(al1);
            mal.Add(al2);

            mal.Add(al3);
            mal.Add(al4);
            mal.Add(al5);
            mal.Add(al6);
            mal.Add(al7);
            mal.Add(al8);
            mal.Add(al9);
            mal.Add(al10);
            mal.Add(al11);

            mal.Add(al12);
            mal.Add(al13);
            mal.Add(al14);
            mal.Add(al15);
            mal.Add(al16);
            mal.Add(al17);
            mal.Add(al18);

            MasterList.Add(mal);

        }

        private void SevenMemberedRingPossibilities(IAtomContainer m, IRing r, IList<IList<IList<string>>> MasterList)
        {
            // for now only consider case where have 3 double bonds

            IAtom[] ringatoms = new IAtom[7];

            ringatoms[0] = r.Atoms[0];

            int[] num = new int[7];

            for (int j = 0; j <= 6; j++)
            {
                num[j] = m.Atoms.IndexOf(r.Atoms[j]);
            }

            List<string> al1 = new List<string>();
            List<string> al2 = new List<string>();
            List<string> al3 = new List<string>();
            List<string> al4 = new List<string>();
            List<string> al5 = new List<string>();

            al1.Add(num[0] + "-" + num[1]);
            al1.Add(num[2] + "-" + num[3]);
            al1.Add(num[4] + "-" + num[5]);

            al2.Add(num[0] + "-" + num[1]);
            al2.Add(num[2] + "-" + num[3]);
            al2.Add(num[5] + "-" + num[6]);

            al3.Add(num[1] + "-" + num[2]);
            al3.Add(num[3] + "-" + num[4]);
            al3.Add(num[5] + "-" + num[6]);

            al4.Add(num[1] + "-" + num[2]);
            al4.Add(num[3] + "-" + num[4]);
            al4.Add(num[6] + "-" + num[0]);

            al5.Add(num[2] + "-" + num[3]);
            al5.Add(num[4] + "-" + num[5]);
            al5.Add(num[6] + "-" + num[0]);

            List<IList<string>> mal = new List<IList<string>>();

            mal.Add(al1);
            mal.Add(al2);
            mal.Add(al3);
            mal.Add(al4);
            mal.Add(al5);

            MasterList.Add(mal);

        }

        private int GetBadCount(IAtomContainer atomContainer, IRingSet ringSet)
        {
            // finds count of nitrogens in the rings that have 4 bonds
            // to non hydrogen atoms and one to hydrogen
            // or nitrogens with 2 double bonds to atoms in the ringset
            // or have S atom with more than 2 bonds
            // these arent necessarily bad- just unlikely

            int count = 0;

            for (int j = 0; j <= atomContainer.Atoms.Count - 1; j++)
            {
                IAtom atom = atomContainer.Atoms[j];

                //Debug.WriteLine(mol.GetBondOrderSum(a));

                if (InRingSet(atom, ringSet))
                {
                    //Debug.WriteLine("in ring set");

                    if (atom.Symbol.Equals("N"))
                    {
                        if (atom.FormalCharge == 0)
                        {
                            //						Debug.WriteLine(mol.GetBondOrderSum(a));
                            if (atomContainer.GetBondOrderSum(atom) == 4)
                            {
                                count++; //
                            }
                            else if (atomContainer.GetBondOrderSum(atom) == 5)
                            {
                                // check if have 2 double bonds to atom in ring
                                int doublebondcount = 0;
                                IEnumerable<IAtom> ca = atomContainer.GetConnectedAtoms(atom);

                                foreach (var a in ca)
                                {
                                    if (atomContainer.GetBond(atom, a).Order == BondOrder.Double)
                                    {
                                        if (InRingSet(a, ringSet))
                                        {
                                            doublebondcount++;
                                        }
                                    }
                                }

                                if (doublebondcount == 2)
                                {
                                    count++;
                                }

                            }
                        }
                        else if (atom.FormalCharge == 1)
                        {
                            if (atomContainer.GetBondOrderSum(atom) == 5)
                            {
                                count++;
                            }
                        }
                    }
                    else if (atom.Symbol.Equals("S"))
                    {
                        if (atomContainer.GetBondOrderSum(atom) > 2)
                        {
                            count++;
                        }
                    }
                }
            }
            //Debug.WriteLine("here bad count = " + count);

            return count;
        }

        private bool InRingSet(IAtom atom, IRingSet ringSet)
        {
            for (int i = 0; i < ringSet.Count; i++)
            {
                IRing ring = (IRing)ringSet[i];
                if (ring.Contains(atom)) return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="starttime">Start time in ticks.</param>
        /// <param name="atomContainer"></param>
        /// <param name="index"></param>
        /// <param name="MasterList"></param>
        /// <param name="choices"></param>
        /// <param name="som"></param>
        /// <returns></returns>
        private IAtomContainer Loop(long starttime, IAtomContainer atomContainer, int index,
                IList<IList<IList<string>>> MasterList, int[] choices, IAtomContainerSet<IAtomContainer> som)
        {

            //Debug.WriteLine(System.CurrentTimeMillis());

            long time = DateTime.Now.Ticks;

            long diff = time - starttime;

            if (diff > 100000 * 10000)
            {
                //time out after 100 seconds
                throw new CDKException("Timed out after 100 seconds.");
            }
            else if (this.Interrupted)
            {
                // reset the interruption
                this.Interrupted = false;
                throw new CDKException("Process was interrupted.");
            }

            IList<IList<string>> ringlist = MasterList[index];

            IAtomContainer mnew2 = null;

            for (int i = 0; i <= ringlist.Count - 1; i++)
            {

                choices[index] = i;

                if (index == MasterList.Count - 1)
                {
                    //Debug.WriteLine(choices[0]+"\t"+choices[1]);

                    IAtomContainer mnew = null;
                    try
                    {
                        mnew = (IAtomContainer)atomContainer.Clone();
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Failed to clone atomContainer: ", e.Message);
                        Debug.WriteLine(e);
                    }

                    for (int j = 0; j <= MasterList.Count - 1; j++)
                    {
                        IList<IList<string>> ringlist2 = MasterList[j];
                        IList<string> bondlist = ringlist2[choices[j]];
                        //					Debug.WriteLine(j+"\t"+choices[j]);
                        ApplyBonds(mnew, bondlist);
                    }
                    //				Debug.WriteLine("");

                    if (IsStructureOK(mnew))
                    {

                        IRingSet rs = this.RemoveExtraRings(mnew); // need to redo this since created new atomContainer (mnew)

                        if (rs != null)
                        {

                            int count = this.GetBadCount(mnew, rs);
                            // Debug.WriteLine("bad count="+count);

                            if (count == 0)
                            {
                                // Debug.WriteLine("found match after "+counter+"
                                // iterations");
                                return mnew; // dont worry about adding to set
                                             // just finish
                            }
                            else
                            {
                                som.Add(mnew);
                            }
                        }
                    }

                }

                if (index + 1 <= MasterList.Count - 1)
                {
                    // Debug.WriteLine("here3="+counter);
                    mnew2 = Loop(starttime, atomContainer, index + 1, MasterList, choices, som); //recursive def
                }

                if (mnew2 is IAtomContainer)
                {
                    return mnew2;
                }
            }
            return null;

        }

        private bool IsStructureOK(IAtomContainer atomContainer)
        {
            try
            {
                CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(atomContainer.Builder);
                foreach (var atom in atomContainer.Atoms)
                {
                    IAtomType matched = matcher.FindMatchingAtomType(atomContainer, atom);
                    if (matched == null || matched.AtomTypeName.Equals("X")) return false;
                }

                IRingSet ringSet = RecoverRingSystem(atomContainer);
                //Debug.WriteLine("Rs size= "+rs.Count);

                // clear aromaticity flags
                for (int i = 0; i <= atomContainer.Atoms.Count - 1; i++)
                {
                    atomContainer.Atoms[i].IsAromatic = false;
                }
                for (int i = 0; i <= ringSet.Count - 1; i++)
                {
                    IRing r = (IRing)ringSet[i];
                    r.IsAromatic = false;
                }
                // now, detect aromaticity from cratch, and mark rings as aromatic too (if ...)
                Aromaticity.CDKLegacy.Apply(atomContainer);
                for (int i = 0; i <= ringSet.Count - 1; i++)
                {
                    IRing ring = (IRing)ringSet[i];
                    RingManipulator.MarkAromaticRings(ring);
                }

                //			Figure out which rings we want to make sure are aromatic:
                bool[] Check = this.FindRingsToCheck(ringSet);

                //			for (int i=0;i<=Check.Length-1;i++) {
                //			Debug.WriteLine(i+"\t"+rs[i].Atoms.Count+"\t"+Check[i]);
                //			}

                for (int i = 0; i <= ringSet.Count - 1; i++)
                {
                    IRing ring = (IRing)ringSet[i];

                    //Debug.WriteLine(k+"\t"+r.Atoms.Count+"\t"+r.IsAromatic);
                    if (Check[i])
                    {

                        for (int j = 0; j <= ring.Atoms.Count - 1; j++)
                        {
                            if (ring.Atoms[j].ImplicitHydrogenCount.HasValue
                                    && ring.Atoms[j].ImplicitHydrogenCount.Value < 0)
                            {
                                return false;
                            }
                        }

                        if (!ring.IsAromatic)
                        {
                            //						Debug.WriteLine(counter+"\t"+"ring not aromatic"+"\t"+r.Atoms.Count);
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }

        }

        /**
         * Remove rings.
         * <p/>
         * Removes rings which do not have all sp2/planar3 aromatic atoms and also gets rid of rings that have more than
         * 7 or less than 5 atoms in them.
         *
         * @param m The AtomContainer from which we want to remove rings
         * @return The set of reduced rings
         */
        private IRingSet RemoveExtraRings(IAtomContainer m)
        {

            try
            {
                IRingSet rs = Cycles.SSSR(m).ToRingSet();

                //remove rings which dont have all aromatic atoms (according to hybridization set by lower case symbols in smiles):

                //Debug.WriteLine("numrings="+rs.Count);

                for (int i = 0; i <= rs.Count - 1; i++)
                {

                    IRing r = (IRing)rs[i];

                    if (r.Atoms.Count > 7 || r.Atoms.Count < 5)
                    {
                        rs.RemoveAt(i);
                        i--; // go back to first one
                        goto iloop;
                    }

                    //int NonSP2Count = 0;

                    for (int j = 0; j <= r.Atoms.Count - 1; j++)
                    {
                        //Debug.WriteLine(j+"\t"+r.GetAtomAt(j).Symbol+"\t"+r.GetAtomAt(j).Hybridization);
                        if (r.Atoms[j].Hybridization.IsUnset
                                || !(r.Atoms[j].Hybridization == Hybridization.SP2 || 
                                      r.Atoms[j].Hybridization == Hybridization.Planar3))
                        {
                            rs.RemoveAt(i);
                            i--; // go back
                            goto iloop;
                            //                        NonSP2Count++;
                            //                        if (r.Atoms[j].Symbol.Equals("C")) {
                            //                            rs.RemoveAtomContainer(i);
                            //                            i--; // go back
                            //                            continue iloop;
                            //                        }
                        }
                    }

                    //                if (NonSP2Count > 1) {
                    //                    rs.RemoveAtomContainer(i);
                    //                    i--; // go back
                    //                    continue iloop;
                    //                }

                    iloop:
                    ;
                }
                return rs;

            }
            catch (Exception)
            {
                return m.Builder.CreateRingSet();
            }
        }

        private bool[] FindRingsToCheck(IRingSet rs)
        {

            bool[] Check = new bool[rs.Count];

            for (int i = 0; i <= Check.Length - 1; i++)
            {
                Check[i] = true;
            }



            for (int i = 0; i <= rs.Count - 1; i++)
            {

                IRing r = (IRing)rs[i];

                if (r.Atoms.Count > 7)
                {
                    Check[i] = false;
                    continue;
                }

                int NonSP2Count = 0;

                for (int j = 0; j <= r.Atoms.Count - 1; j++)
                {

                    // Debug.WriteLine(j+"\t"+r.GetAtomAt(j).Symbol+"\t"+r.GetAtomAt(j).Hybridization);

                    if (r.Atoms[j].Hybridization.IsUnset || 
                        r.Atoms[j].Hybridization != Hybridization.SP2)
                    {
                        NonSP2Count++;
                        if (r.Atoms[j].Symbol.Equals("C"))
                        {
                            Check[i] = false;
                            goto iloop;
                        }
                    }
                }

                if (NonSP2Count > 1)
                {
                    Check[i] = false;
                    continue;
                }
                iloop:
                ;
            }

            return Check;
        }

        /**
         * Stores an IRingSet corresponding to a AtomContainer using the bond numbers.
         *
         * @param mol      The IAtomContainer for which to store the IRingSet.
         * @param ringSet  The IRingSet to store
         */
        private void StoreRingSystem(IAtomContainer mol, IRingSet ringSet)
        {
            listOfRings = new List<int[]>(); // this is a list of int arrays
            for (int r = 0; r < ringSet.Count; ++r)
            {
                IRing ring = (IRing)ringSet[r];
                int[] bondNumbers = new int[ring.Bonds.Count];
                for (int i = 0; i < ring.Bonds.Count; ++i)
                    bondNumbers[i] = mol.Bonds.IndexOf(ring.Bonds[i]);
                listOfRings.Add(bondNumbers);
            }
        }

        /**
         * Recovers a RingSet corresponding to a AtomContainer that has been
         * stored by StoreRingSystem().
         *
         * @param mol      The IAtomContainer for which to recover the IRingSet.
         */
        private IRingSet RecoverRingSystem(IAtomContainer mol)
        {
            IRingSet ringSet = mol.Builder.CreateRingSet();
            foreach (var bondNumbers in listOfRings)
            {
                IRing ring = mol.Builder.CreateRing();
                foreach (var bondNumber in bondNumbers)
                {
                    IBond bond = mol.Bonds[bondNumber];
                    ring.Add(bond);
                    if (!ring.Contains(bond.Atoms[0])) ring.Atoms.Add(bond.Atoms[0]);
                    if (!ring.Contains(bond.Atoms[1])) ring.Atoms.Add(bond.Atoms[1]);
                }
                ringSet.Add(ring);
            }
            return ringSet;
        }
    }
}
