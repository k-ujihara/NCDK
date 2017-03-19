/* Copyright (C) 2006-2007  Todd Martin (Environmental Protection Agency)
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.Aromaticities;
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;
using System;
using System.Linq;

namespace NCDK.Tools
{
    /// <summary>
    /// Utility class written by Todd Martin, for help in his QSAR descriptors and SMILES
    /// parser. Seems to have overlap with, at least, cdk.normalize.Normalizer.
    /// </summary>
    /// <para>TODO: merge with Normalizer.</para>
    /// <seealso cref="Normalize.Normalizer"/>
    // @author     Todd Martin
    // @cdk.module extra
    // @cdk.githash
    public class CDKUtilities
    {
        public static string FixSmiles(string Smiles)
        {
            Smiles = Smiles.Replace("CL", "Cl");
            Smiles = Smiles.Replace("(H)", "([H])");
            //        Smiles=Smiles.Replace("N=N#N","N=[N+]=[N-]");
            //        Smiles=Smiles.Replace("#N=O","#[N+][O-]");
            Smiles = Smiles.Trim();

            return Smiles;
        }

        private static bool FixNitroGroups(IAtomContainer m)
        {
            // changes nitros given by N(=O)(=O) to [N+](=O)[O-]
            bool changed = false;
            try
            {
                for (int i = 0; i <= m.Atoms.Count - 1; i++)
                {
                    IAtom a = m.Atoms[i];
                    if (a.Symbol.Equals("N"))
                    {
                        var ca = m.GetConnectedAtoms(a).ToList();

                        if (ca.Count == 3)
                        {
                            IAtom[] cao = new IAtom[2];

                            int count = 0;

                            for (int j = 0; j <= 2; j++)
                            {
                                if (((IAtom)ca[j]).Symbol.Equals("O"))
                                {
                                    count++;
                                }
                            }

                            if (count > 1)
                            {

                                count = 0;
                                for (int j = 0; j <= 2; j++)
                                {
                                    IAtom caj = (IAtom)ca[j];
                                    if (caj.Symbol.Equals("O"))
                                    {
                                        if (m.GetConnectedAtoms(caj).Count() == 1)
                                        {// account for possibility of ONO2
                                            cao[count] = caj;
                                            count++;
                                        }
                                    }
                                }

                                BondOrder order1 = m.GetBond(a, cao[0]).Order;
                                BondOrder order2 = m.GetBond(a, cao[1]).Order;

                                //if (totalobonds==4) { // need to fix (FIXME)
                                if (order1 == BondOrder.Single && order2 == BondOrder.Double)
                                {
                                    a.FormalCharge = 1;
                                    cao[0].FormalCharge = -1; // pick first O arbitrarily
                                    m.GetBond(a, cao[0]).Order = BondOrder.Single;
                                    changed = true;
                                }
                            } //else if (count==1) {// end if count>1

                        }// end ca==3 if

                    } // end symbol == N

                }

                return changed;
            }
            catch (Exception)
            {
                return changed;
            }
        }

        public static bool FixNitroGroups2(IAtomContainer m)
        {
            // changes nitros given by [N+](=O)[O-] to N(=O)(=O)
            bool changed = false;
            try
            {
                for (int i = 0; i <= m.Atoms.Count - 1; i++)
                {
                    IAtom a = m.Atoms[i];
                    if (a.Symbol.Equals("N"))
                    {
                        var ca = m.GetConnectedAtoms(a).ToList();

                        if (ca.Count == 3)
                        {
                            IAtom[] cao = new IAtom[2];

                            int count = 0;

                            for (int j = 0; j <= 2; j++)
                            {
                                IAtom caj = ca[j];
                                if (caj.Symbol.Equals("O"))
                                {
                                    count++;
                                }
                            }

                            if (count > 1)
                            {
                                count = 0;
                                for (int j = 0; j <= 2; j++)
                                {
                                    IAtom caj = (IAtom)ca[j];
                                    if (caj.Symbol.Equals("O"))
                                    {
                                        if (m.GetConnectedAtoms(caj).Count() == 1)
                                        {// account for possibility of ONO2
                                            cao[count] = caj;
                                            count++;
                                        }
                                    }
                                }

                                BondOrder order1 = m.GetBond(a, cao[0]).Order;
                                BondOrder order2 = m.GetBond(a, cao[1]).Order;

                                //int totalobonds=0;
                                //totalobonds+=m.Bonds[a,cao[0]].Order;
                                //                        totalobonds+=m.Bonds[a,cao[1]].Order;

                                //if (totalobonds==4) { // need to fix
                                if ((order1 == BondOrder.Single && order2 == BondOrder.Double)
                                        || (order1 == BondOrder.Double && order2 == BondOrder.Single))
                                {
                                    a.FormalCharge = 0;
                                    cao[0].FormalCharge = 0; // pick first O arbitrarily
                                    cao[1].FormalCharge = 0; // pick first O arbitrarily
                                    m.GetBond(a, cao[0]).Order = BondOrder.Double;
                                    m.GetBond(a, cao[1]).Order = BondOrder.Double;
                                    changed = true;
                                }
                            } // end if count>1

                        }// end ca==3 if

                    } // end symbol == N

                }

                return changed;
            }
            catch (Exception)
            {
                return changed;
            }
        }

        public static void FixAromaticityForXLogP(IAtomContainer m)
        {
            // need to find rings and aromaticity again since added H's

            IRingSet rs = null;
            try
            {
                AllRingsFinder arf = new AllRingsFinder();
                rs = arf.FindAllRings(m);

                // SSSRFinder s = new SSSRFinder(m);
                // srs = s.FindEssentialRings();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            try
            {
                // figure out which atoms are in aromatic rings:
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);
                Aromaticity.CDKLegacy.Apply(m);
                // figure out which rings are aromatic:
                RingSetManipulator.MarkAromaticRings(rs);
                // figure out which simple (non cycles) rings are aromatic:
                // HueckelAromaticityDetector.DetectAromaticity(m, srs);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            // only atoms in 6 membered rings are aromatic
            // determine largest ring that each atom is a part of

            for (int i = 0; i <= m.Atoms.Count - 1; i++)
            {
                m.Atoms[i].IsAromatic = false;

                jloop: for (int j = 0; j <= rs.Count - 1; j++)
                {
                    //Debug.WriteLine(i+"\t"+j);
                    IRing r = (IRing)rs[j];
                    if (!r.IsAromatic)
                    {
                        goto continue_jloop;
                    }

                    bool haveatom = r.Contains(m.Atoms[i]);

                    //Debug.WriteLine("haveatom="+haveatom);

                    if (haveatom && r.Atoms.Count == 6)
                    {
                        m.Atoms[i].IsAromatic = true;
                    }
            continue_jloop:
                    ;
                }
            }
        }

        public static void FixSulphurH(IAtomContainer m)
        {
            // removes extra H's attached to sulphurs
            //Debug.WriteLine("EnterFixSulphur");

            for (int i = 0; i <= m.Atoms.Count - 1; i++)
            {
                IAtom a = m.Atoms[i];

                if (a.Symbol.Equals("S"))
                {
                    var connectedAtoms = m.GetConnectedAtoms(a);

                    int bondOrderSum = 0;

                    foreach (var conAtom in connectedAtoms)
                    {
                        if (!conAtom.Symbol.Equals("H"))
                        {
                            IBond bond = m.GetBond(a, conAtom);
                            if (bond.Order == BondOrder.Single)
                            {
                                bondOrderSum += 1;
                            }
                            else if (bond.Order == BondOrder.Double)
                            {
                                bondOrderSum += 2;
                            }
                            else if (bond.Order == BondOrder.Triple)
                            {
                                bondOrderSum += 3;
                            }
                            else if (bond.Order == BondOrder.Quadruple)
                            {
                                bondOrderSum += 4;
                            }
                        }
                    }

                    if (bondOrderSum > 1)
                    {
                        foreach (var conAtom in connectedAtoms)
                        {
                            if (conAtom.Symbol.Equals("H"))
                            {
                                m.RemoveAtomAndConnectedElectronContainers(conAtom);
                            }
                        }
                    }
                }
            }
        }
    }
}
