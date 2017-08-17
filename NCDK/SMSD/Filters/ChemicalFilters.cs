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
 * You should have received eAtom copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Aromaticities;
using NCDK.Graphs;
using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Ring;
using NCDK.SMSD.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
namespace NCDK.SMSD.Filters
{
    /// <summary>
    /// Class that ranks MCS final solution according to the chemical rules.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public class ChemicalFilters
    {
        private List<IDictionary<int, int>> allMCS = null;
        private IDictionary<int, int> firstSolution = null;
        private List<IDictionary<IAtom, IAtom>> allAtomMCS = null;
        private IDictionary<IAtom, IAtom> firstAtomMCS = null;
        private List<double> stereoScore = null;
        private List<int> fragmentSize = null;
        private List<double> bEnergies = null;
        private IAtomContainer rMol = null;
        private IAtomContainer pMol = null;

        /// <summary>
        /// This class has all the three chemical filters supported by the SMSD.
        /// i.e ring matches, bond energy etc
        ///
        /// <list type="bullet">
        /// <item>a: Bond energy,</item>
        /// <item>b: Fragment count,</item>
        /// <item>c: Stereo matches</item>
        /// </list> 
        /// </summary>
        public ChemicalFilters(List<IDictionary<int, int>> allMCS, List<IDictionary<IAtom, IAtom>> allAtomMCS,
                IDictionary<int, int> firstSolution, IDictionary<IAtom, IAtom> firstAtomMCS, IAtomContainer sourceMol,
                IAtomContainer targetMol)
        {
            this.allAtomMCS = allAtomMCS;
            this.allMCS = allMCS;
            this.firstAtomMCS = firstAtomMCS;
            this.firstSolution = firstSolution;
            this.pMol = targetMol;
            this.rMol = sourceMol;

            stereoScore = new List<double>();
            fragmentSize = new List<int>();
            bEnergies = new List<double>();

        }

        private void Clear()
        {

            firstSolution.Clear();
            allMCS.Clear();
            allAtomMCS.Clear();
            firstAtomMCS.Clear();
            stereoScore.Clear();
            fragmentSize.Clear();
            bEnergies.Clear();

        }

        private void Clear(IDictionary<int, IDictionary<int, int>> sortedAllMCS,
                IDictionary<int, IDictionary<IAtom, IAtom>> sortedAllAtomMCS, IDictionary<int, double> stereoScoreMap,
                IDictionary<int, int> fragmentScoreMap, IDictionary<int, double> energySelectionMap)
        {
            sortedAllMCS.Clear();
            sortedAllAtomMCS.Clear();
            stereoScoreMap.Clear();
            fragmentScoreMap.Clear();
            energySelectionMap.Clear();
        }

        private void AddSolution(int counter, int key, IDictionary<int, IDictionary<IAtom, IAtom>> allFragmentAtomMCS,
                IDictionary<int, IDictionary<int, int>> allFragmentMCS, IDictionary<int, double> stereoScoreMap,
                IDictionary<int, double> energyScoreMap, IDictionary<int, int> fragmentScoreMap)
        {

            allAtomMCS.Insert(counter, allFragmentAtomMCS[key]);
            allMCS.Insert(counter, allFragmentMCS[key]);
            stereoScore.Insert(counter, stereoScoreMap[key]);
            fragmentSize.Insert(counter, fragmentScoreMap[key]);
            bEnergies.Insert(counter, energyScoreMap[key]);

        }

        private void InitializeMaps(IDictionary<int, IDictionary<int, int>> sortedAllMCS,
                IDictionary<int, IDictionary<IAtom, IAtom>> sortedAllAtomMCS, IDictionary<int, double> stereoScoreMap,
                IDictionary<int, int> fragmentScoreMap, IDictionary<int, double> energySelectionMap)
        {

            int index = 0;
            foreach (var atomsMCS in allAtomMCS)
            {
                sortedAllAtomMCS[index] = atomsMCS;
                fragmentScoreMap[index] = 0;
                energySelectionMap[index] = 0.0;
                stereoScoreMap[index] = 0.0;
                index++;
            }

            index = 0;
            foreach (var mcs in allMCS)
            {
                sortedAllMCS[index] = mcs;
                index++;
            }

            index = 0;
            foreach (var score in bEnergies)
            {
                energySelectionMap[index] = score;
                index++;
            }

            index = 0;
            foreach (var score in fragmentSize)
            {
                fragmentScoreMap[index] = score;
                index++;
            }

            index = 0;
            foreach (var score in stereoScore)
            {
                stereoScoreMap[index] = score;
                index++;
            }

        }

        /// <summary>
        /// Sort MCS solution by stereo and bond type matches.
        /// </summary>
        /// <exception cref="CDKException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SortResultsByStereoAndBondMatch()
        {

            //        Console.Out.WriteLine("\n\n\n\nSort By ResultsByStereoAndBondMatch");

            IDictionary<int, IDictionary<int, int>> allStereoMCS = new Dictionary<int, IDictionary<int, int>>();
            IDictionary<int, IDictionary<IAtom, IAtom>> allStereoAtomMCS = new Dictionary<int, IDictionary<IAtom, IAtom>>();

            IDictionary<int, int> fragmentScoreMap = new SortedDictionary<int, int>();
            IDictionary<int, double> energyScoreMap = new SortedDictionary<int, double>();
            IDictionary<int, double> aStereoScoreMap = new Dictionary<int, double>();

            InitializeMaps(allStereoMCS, allStereoAtomMCS, aStereoScoreMap, fragmentScoreMap, energyScoreMap);

            bool stereoMatchFlag = GetStereoBondChargeMatch(aStereoScoreMap, allStereoMCS, allStereoAtomMCS);

            bool flag = false;
            if (stereoMatchFlag)
            {
                //Higher Score is mapped preferred over lower
                var stereoScoreMap = SortMapByValueInDecendingOrder(aStereoScoreMap);
                double higestStereoScore = !stereoScoreMap.Any() ? 0 : stereoScoreMap.First().Value;
                double secondhigestStereoScore = higestStereoScore;
                foreach (var entry in stereoScoreMap)
                {
                    if (secondhigestStereoScore < higestStereoScore && entry.Value > secondhigestStereoScore)
                    {
                        secondhigestStereoScore = entry.Value;
                    }
                    else if (secondhigestStereoScore == higestStereoScore
                          && entry.Value < secondhigestStereoScore)
                    {
                        secondhigestStereoScore = entry.Value;
                    }
                }

                if (stereoScoreMap.Any())
                {
                    flag = true;
                    Clear();
                }

                /* Put back the sorted solutions */

                int counter = 0;
                foreach (var entry in stereoScoreMap)
                {
                    var i = entry.Key;
                    //                Console.Out.WriteLine("Sorted Map key " + I + " Sorted Value: " + stereoScoreMap[I]);
                    //                Console.Out.WriteLine("Stereo MCS " + allStereoMCS[I] + " Stereo Value: "
                    //                        + stereoScoreMap[I]);
                    if (higestStereoScore == entry.Value)
                    {
                        //|| secondhigestStereoScore == stereoScoreMap[I].Value) {
                        AddSolution(counter, i, allStereoAtomMCS, allStereoMCS, aStereoScoreMap, energyScoreMap,
                                fragmentScoreMap);
                        counter++;

                        //                    Console.Out.WriteLine("Sorted Map key " + I + " Sorted Value: " + stereoScoreMap[I]);
                        //                    Console.Out.WriteLine("Stereo MCS " + allStereoMCS[I] + " Stereo Value: "
                        //                            + stereoScoreMap[I]);
                    }
                }
                if (flag)
                {
                    foreach (var e in allMCS[0])
                        firstSolution[e.Key] = e.Value;
                    foreach (var e in allAtomMCS[0])
                        firstAtomMCS[e.Key] = e.Value;
                    Clear(allStereoMCS, allStereoAtomMCS, aStereoScoreMap, fragmentScoreMap, energyScoreMap);
                }
            }

        }

        /// <summary>
        /// Sort solution by ascending order of the fragment count.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SortResultsByFragments()
        {

            //        Console.Out.WriteLine("\nSort By Fragment");
            IDictionary<int, IDictionary<int, int>> allFragmentMCS = new SortedDictionary<int, IDictionary<int, int>>();
            IDictionary<int, IDictionary<IAtom, IAtom>> allFragmentAtomMCS = new SortedDictionary<int, IDictionary<IAtom, IAtom>>();

            IDictionary<int, double> stereoScoreMap = new SortedDictionary<int, double>();
            IDictionary<int, double> energyScoreMap = new SortedDictionary<int, double>();
            IDictionary<int, int> fragmentScoreMap = new SortedDictionary<int, int>();

            InitializeMaps(allFragmentMCS, allFragmentAtomMCS, stereoScoreMap, fragmentScoreMap, energyScoreMap);

            int minFragmentScore = 9999;
            foreach (var key in allFragmentAtomMCS.Keys)
            {
                IDictionary<IAtom, IAtom> mcsAtom = allFragmentAtomMCS[key];
                int fragmentCount = GetMappedMoleculeFragmentSize(mcsAtom);
                fragmentScoreMap[key] = fragmentCount;
                if (minFragmentScore > fragmentCount)
                {
                    minFragmentScore = fragmentCount;
                }
            }
            bool flag = false;
            if (minFragmentScore < 9999)
            {
                flag = true;
                Clear();
            }
            int counter = 0;
            foreach (var map in fragmentScoreMap)
            {
                if (minFragmentScore == map.Value)
                {
                    AddSolution(counter, map.Key, allFragmentAtomMCS, allFragmentMCS, stereoScoreMap, energyScoreMap,
                            fragmentScoreMap);
                    counter++;
                    //                Console.Out.WriteLine("Fragment key " + map.Key + " Size: " + fragmentScoreMap[map.Key]);
                    //                Console.Out.WriteLine("Fragment MCS " + allFragmentMCS[map.Key] + " Stereo Value: "
                    //                        + stereoScoreMap[map.Key]);
                }
            }

            if (flag)
            {
                foreach (var e in allMCS[0])
                    firstSolution[e.Key] = e.Value;
                foreach (var e in allAtomMCS[0])
                    firstAtomMCS[e.Key] = e.Value;
                Clear(allFragmentMCS, allFragmentAtomMCS, stereoScoreMap, fragmentScoreMap, energyScoreMap);
            }
        }

        /// <summary>
        /// Sort MCS solution by bond breaking energy.
        /// </summary>
        /// <exception cref="CDKException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SortResultsByEnergies()
        {

            //        Console.Out.WriteLine("\nSort By Energies");
            IDictionary<int, IDictionary<int, int>> allEnergyMCS = new SortedDictionary<int, IDictionary<int, int>>();
            IDictionary<int, IDictionary<IAtom, IAtom>> allEnergyAtomMCS = new SortedDictionary<int, IDictionary<IAtom, IAtom>>();

            IDictionary<int, double> stereoScoreMap = new SortedDictionary<int, double>();
            IDictionary<int, int> fragmentScoreMap = new SortedDictionary<int, int>();
            IDictionary<int, double> aEnergySelectionMap = new SortedDictionary<int, double>();

            InitializeMaps(allEnergyMCS, allEnergyAtomMCS, stereoScoreMap, fragmentScoreMap, aEnergySelectionMap);

            foreach (var key in allEnergyMCS.Keys)
            {
                IDictionary<int, int> mcsAtom = allEnergyMCS[key];
                double energies = GetMappedMoleculeEnergies(mcsAtom);
                aEnergySelectionMap[key] = energies;
            }

            var energySelectionMap = SortMapByValueInAccendingOrder(aEnergySelectionMap);
            bool flag = false;

            double lowestEnergyScore = 99999999.99;
            foreach (var entry in energySelectionMap)
            {
                lowestEnergyScore = entry.Value;
                flag = true;
                Clear();
                break;
            }

            int counter = 0;
            foreach (var map in energySelectionMap)
            {
                if (lowestEnergyScore == map.Value)
                {
                    AddSolution(counter, map.Key, allEnergyAtomMCS, allEnergyMCS, stereoScoreMap, aEnergySelectionMap,
                            fragmentScoreMap);
                    counter++;
                    //
                    //                Console.Out.WriteLine("Energy key " + map.Key + "Energy MCS " + allEnergyMCS[map.Key]);
                    //                Console.Out.WriteLine("Frag Size: " + fragmentScoreMap[map.Key] + " Stereo Value: "
                    //                        + stereoScoreMap[map.Key]);

                }
            }

            if (flag)
            {
                foreach (var e in allMCS[0])
                    firstSolution[e.Key] = e.Value;
                foreach (var e in allAtomMCS[0])
                    firstAtomMCS[e.Key] = e.Value;
                Clear(allEnergyMCS, allEnergyAtomMCS, stereoScoreMap, fragmentScoreMap, aEnergySelectionMap);
            }
        }

        private IDictionary<IBond, IBond> MakeBondMapsOfAtomMaps(IAtomContainer ac1, IAtomContainer ac2,
                IDictionary<int, int> mappings)
        {

            IDictionary<IBond, IBond> maps = new Dictionary<IBond, IBond>();

            foreach (var atoms in ac1.Atoms)
            {

                int ac1AtomNumber = ac1.Atoms.IndexOf(atoms);

                if (mappings.ContainsKey(ac1AtomNumber))
                {

                    int ac2AtomNumber = mappings[ac1AtomNumber];

                    var connectedAtoms = ac1.GetConnectedAtoms(atoms);

                    foreach (var cAtoms in connectedAtoms)
                    {
                        int ac1ConnectedAtomNumber = ac1.Atoms.IndexOf(cAtoms);

                        if (mappings.ContainsKey(ac1ConnectedAtomNumber))
                        {
                            {
                                int ac2ConnectedAtomNumber = mappings[ac1ConnectedAtomNumber];

                                IBond ac1Bond = ac1.GetBond(atoms, cAtoms);
                                IBond ac2Bond = ac2
                                        .GetBond(ac2.Atoms[ac2AtomNumber], ac2.Atoms[ac2ConnectedAtomNumber]);

                                if (ac2Bond == null)
                                {
                                    ac2Bond = ac2.GetBond(ac2.Atoms[ac2ConnectedAtomNumber], ac2.Atoms[ac2AtomNumber]);
                                }

                                if (ac1Bond != null && ac2Bond != null)
                                {
                                    maps[ac1Bond] = ac2Bond;
                                }
                            }
                        }
                    }
                }
            }
            //        Console.Out.WriteLine("Mol Map size:" + maps.Count);
            return maps;

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private int GetMappedMoleculeFragmentSize(IDictionary<IAtom, IAtom> mcsAtomSolution)
        {

            //      Console.Out.WriteLine("Mol Size Eorg: " + sourceMol.Molecule.Atoms.Count + " , Mol Size Porg: " +
            //        targetMol.Molecule.Atoms.Count);

            IAtomContainer educt = Default.ChemObjectBuilder.Instance.NewAtomContainer(rMol);
            IAtomContainer product = Default.ChemObjectBuilder.Instance.NewAtomContainer(pMol);

            if (mcsAtomSolution != null)
            {
                foreach (var map in mcsAtomSolution)
                {
                    IAtom atomE = map.Key;
                    IAtom atomP = map.Value;
                    educt.RemoveAtomAndConnectedElectronContainers(atomE);
                    product.RemoveAtomAndConnectedElectronContainers(atomP);
                }
            }
            return GetFragmentCount(educt) + GetFragmentCount(product);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private double GetMappedMoleculeEnergies(IDictionary<int, int> mcsAtomSolution)
        {

            //        Console.Out.WriteLine("\nSort By Energies");
            double totalBondEnergy = -9999.0;

            IAtomContainer educt = Default.ChemObjectBuilder.Instance.NewAtomContainer(rMol);
            IAtomContainer product = Default.ChemObjectBuilder.Instance.NewAtomContainer(pMol);

            foreach (var eAtom in educt.Atoms)
            {
                eAtom.IsPlaced = false;
            }

            foreach (var pAtom in product.Atoms)
            {
                pAtom.IsPlaced = false;
            }

            if (mcsAtomSolution != null)
            {
                foreach (var map in mcsAtomSolution)
                {
                    int eNum = map.Key;
                    int pNum = map.Value;

                    IAtom eAtom = educt.Atoms[eNum];
                    IAtom pAtom = product.Atoms[pNum];

                    eAtom.IsPlaced = true;
                    pAtom.IsPlaced = true;
                }
            }

            if (mcsAtomSolution != null)
            {
                totalBondEnergy = GetEnergy(educt, product);
            }
            return totalBondEnergy;
        }

        internal static IEnumerable<KeyValuePair<int, double>> SortMapByValueInAccendingOrder(IDictionary<int, double> map)
        {
            return map.OrderBy(entry => entry.Value);
        }


        internal static IEnumerable<KeyValuePair<int, double>> SortMapByValueInDecendingOrder(IDictionary<int, double> map)
        {
            return map.OrderByDescending(entry => entry.Value);
        }

        /// <summary>
        /// Return sorted energy in ascending order.
        /// <returns>sorted bond breaking energy</returns>
        /// </summary>
        public IList<double> GetSortedEnergy()
        {
            return new ReadOnlyCollection<double>(bEnergies);
        }

        /// <summary>
        /// Return sorted fragment in ascending order of the size.
        /// <returns>sorted fragment count</returns>
        /// </summary>
        public IList<int> GetSortedFragment()
        {
            return new ReadOnlyCollection<int>(fragmentSize);
        }

        /// <summary>
        /// Return Stereo matches in descending order.
        /// <returns>sorted stereo matches</returns>
        /// </summary>
        public IList<double> GetStereoMatches()
        {
            return new ReadOnlyCollection<double>(stereoScore);
        }

        private List<object> GetMappedFragment(IAtomContainer molecule, ICollection<IAtom> atomsMCS)
        {
            IAtomContainer subgraphContainer = molecule.Builder.NewAtomContainer(molecule);
            List<IAtom> list = new List<IAtom>(atomsMCS.Count);
            foreach (var atom in atomsMCS)
            {
                int post = molecule.Atoms.IndexOf(atom);
                //            Console.Out.WriteLine("Atom to be removed " + post);
                list.Add(subgraphContainer.Atoms[post]);
            }

            List<IAtom> rlist = new List<IAtom>();
            foreach (var atoms in subgraphContainer.Atoms)
            {
                if (!list.Contains(atoms))
                {
                    rlist.Add(atoms);
                }
            }

            foreach (var atoms in rlist)
            {
                subgraphContainer.RemoveAtomAndConnectedElectronContainers(atoms);
            }
            List<object> l = new List<object>();
            l.Add(list);
            l.Add(subgraphContainer);
            return l;
        }

        private double GetAtomScore(double score, IDictionary<IAtom, IAtom> atomMapMCS, IAtomContainer reactant,
                IAtomContainer product)
        {
            foreach (var mappings in atomMapMCS)
            {
                IAtom rAtom = mappings.Key;
                IAtom pAtom = mappings.Value;

                int rHCount = 0;
                int pHCount = 0;
                double rBO = reactant.GetBondOrderSum(rAtom);
                double pBO = product.GetBondOrderSum(pAtom);

                if (rAtom.ImplicitHydrogenCount != null)
                {
                    rHCount = rAtom.ImplicitHydrogenCount.Value;
                }
                if (pAtom.ImplicitHydrogenCount != null)
                {
                    pHCount = pAtom.ImplicitHydrogenCount.Value;
                }

                int hScore = Math.Abs(rHCount - pHCount);
                double boScore = Math.Abs(rBO - pBO);

                if (rHCount != pHCount)
                {
                    score -= hScore;
                }
                else
                {
                    score += hScore;
                }

                if (rBO != pBO)
                {
                    score -= boScore;
                }
                else
                {
                    score += boScore;
                }
            }
            return score;
        }

        private double GetBondScore(double score, IDictionary<IBond, IBond> bondMaps)
        {
            foreach (var matchedBonds in bondMaps)
            {

                IBond rBond = matchedBonds.Key;
                IBond pBond = matchedBonds.Value;

                score += GetBondFormalChargeMatches(rBond, pBond);
                score += GetBondTypeMatches(rBond, pBond);
            }
            return score;
        }

        private double GetBondFormalChargeMatches(IBond rBond, IBond pBond)
        {
            double score = 0.0;
            if (rBond != null && pBond != null)
            {
                IAtom ratom1 = rBond.Atoms[0];
                IAtom ratom2 = rBond.Atoms[1];
                IAtom patom1 = pBond.Atoms[0];
                IAtom patom2 = pBond.Atoms[1];

                if (ratom1.Symbol.Equals(patom1.Symbol) && ratom1.Symbol.Equals(patom1.Symbol))
                {
                    if ((ratom1.FormalCharge != patom1.FormalCharge)
                            || ratom2.FormalCharge != patom2.FormalCharge)
                    {
                        if (ConvertBondOrder(rBond) != ConvertBondOrder(pBond))
                        {
                            score += 5 * Math.Abs(ConvertBondOrder(rBond) + ConvertBondOrder(pBond));
                        }
                    }
                    if (ratom1.FormalCharge == patom1.FormalCharge
                            && (ConvertBondOrder(rBond) - ConvertBondOrder(pBond)) == 0)
                    {
                        score += 100;
                    }
                    if (ratom2.FormalCharge == patom2.FormalCharge
                            && (ConvertBondOrder(rBond) - ConvertBondOrder(pBond)) == 0)
                    {
                        score += 100;
                    }
                }
                else if (ratom1.Symbol.Equals(patom2.Symbol) && ratom2.Symbol.Equals(patom1.Symbol))
                {
                    if ((ratom1.FormalCharge != patom2.FormalCharge)
                            || ratom2.FormalCharge != patom1.FormalCharge)
                    {
                        if (ConvertBondOrder(rBond) != ConvertBondOrder(pBond))
                        {
                            score += 5 * Math.Abs(ConvertBondOrder(rBond) + ConvertBondOrder(pBond));
                        }
                    }
                    if (ratom1.FormalCharge == patom2.FormalCharge
                            && (ConvertBondOrder(rBond) - ConvertBondOrder(pBond)) == 0)
                    {
                        score += 100;
                    }
                    if (ratom2.FormalCharge == patom1.FormalCharge
                            && (ConvertBondOrder(rBond) - ConvertBondOrder(pBond)) == 0)
                    {
                        score += 100;
                    }
                }
            }

            return score;
        }

        private double GetBondTypeMatches(IBond queryBond, IBond targetBond)
        {
            double score = 0;

            if (targetBond is IQueryBond && queryBond is IBond)
            {
                IQueryBond bond = (IQueryBond)targetBond;
                IQueryAtom atom1 = (IQueryAtom)(targetBond.Atoms[0]);
                IQueryAtom atom2 = (IQueryAtom)(targetBond.Atoms[1]);
                if (bond.Matches(queryBond))
                {
                    // ok, bonds match
                    if (atom1.Matches(queryBond.Atoms[0]) && atom2.Matches(queryBond.Atoms[1])
                            || atom1.Matches(queryBond.Atoms[1]) && atom2.Matches(queryBond.Atoms[0]))
                    {
                        // ok, atoms match in either order
                        score += 4;
                    }
                }
                else
                {
                    score -= 4;
                }
            }
            else if (queryBond is IQueryBond && targetBond is IBond)
            {
                IQueryBond bond = (IQueryBond)queryBond;
                IQueryAtom atom1 = (IQueryAtom)(queryBond.Atoms[0]);
                IQueryAtom atom2 = (IQueryAtom)(queryBond.Atoms[1]);
                if (bond.Matches(targetBond))
                {
                    // ok, bonds match
                    if (atom1.Matches(targetBond.Atoms[0]) && atom2.Matches(targetBond.Atoms[1])
                            || atom1.Matches(targetBond.Atoms[1]) && atom2.Matches(targetBond.Atoms[0]))
                    {
                        // ok, atoms match in either order
                        score += 4;
                    }
                }
                else
                {
                    score -= 4;
                }
            }
            else
            {

                int reactantBondType = ConvertBondOrder(queryBond);
                int productBondType = ConvertBondOrder(targetBond);
                int rStereo = ConvertBondStereo(queryBond);
                int pStereo = ConvertBondStereo(targetBond);
                if ((queryBond.IsAromatic == targetBond.IsAromatic)
                        && (reactantBondType == productBondType))
                {
                    score += 8;
                }
                else if (queryBond.IsAromatic && targetBond.IsAromatic)
                {
                    score += 4;
                }

                if (reactantBondType == productBondType)
                {
                    score += productBondType;
                }
                else
                {
                    score -= 4 * Math.Abs(reactantBondType - productBondType);
                }

                if (rStereo != 4 || pStereo != 4 || rStereo != 3 || pStereo != 3)
                {
                    if (rStereo == pStereo)
                    {
                        score += 1;
                    }
                    else
                    {
                        score -= 1;
                    }
                }

            }
            return score;
        }

        private double GetRingMatchScore(IList<Object> list)
        {
            double lScore = 0;
            List<IAtom> listMap = (List<IAtom>)list[0];
            IAtomContainer ac = (IAtomContainer)list[1];
            HanserRingFinder ringFinder = new HanserRingFinder();
            IRingSet rRings = null;
            try
            {

                rRings = ringFinder.GetRingSet(ac);
            }
            catch (CDKException ex)
            {
                Trace.TraceError(ex.Message);
            }
            RingSetManipulator.Sort(rRings);
            //        Console.Out.WriteLine("Ring length " + );
            lScore = GetRingMatch(rRings, listMap);
            return lScore;
        }

        private double GetEnergy(IAtomContainer educt, IAtomContainer product)
        {
            double eEnergy = 0.0;
            BondEnergies bondEnergy = BondEnergies.Instance;
            for (int i = 0; i < educt.Bonds.Count; i++)
            {
                IBond bond = educt.Bonds[i];
                eEnergy += GetBondEnergy(bond, bondEnergy);
            }
            double pEnergy = 0.0;
            for (int j = 0; j < product.Bonds.Count; j++)
            {
                IBond bond = product.Bonds[j];
                pEnergy += GetBondEnergy(bond, bondEnergy);
            }
            return (eEnergy + pEnergy);
        }

        private double GetBondEnergy(IBond bond, BondEnergies bondEnergy)
        {
            double energy = 0.0;
            if ((bond.Atoms[0].IsPlaced == true && bond.Atoms[1].IsPlaced == false)
                    || (bond.Atoms[0].IsPlaced == false && bond.Atoms[1].IsPlaced == true))
            {
                int val = bondEnergy.GetEnergies(bond.Atoms[0], bond.Atoms[1], bond.Order);
                energy = val;
            }
            return energy;
        }

        private double GetRingMatch(IRingSet rings, List<IAtom> atoms)
        {
            double score = 0.0;
            foreach (var a in atoms)
            {
                foreach (var ring in rings)
                {
                    if (ring.Contains(a))
                    {
                        score += 10;
                    }
                }
            }
            return score;
        }

        private bool GetStereoBondChargeMatch(IDictionary<int, double> stereoScoreMap,
                IDictionary<int, IDictionary<int, int>> allStereoMCS, IDictionary<int, IDictionary<IAtom, IAtom>> allStereoAtomMCS)
        {

            bool stereoMatchFlag = false;
            IAtomContainer reactant = rMol;
            IAtomContainer product = pMol;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactant);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(product);
            Aromaticity.CDKLegacy.Apply(reactant);
            Aromaticity.CDKLegacy.Apply(product);

            foreach (var key in allStereoMCS.Keys)
            {
                double score = 0.0;
                //            Console.Out.WriteLine("\nStart score " + score);
                IDictionary<int, int> atomsMCS = allStereoMCS[key];
                IDictionary<IAtom, IAtom> atomMapMCS = allStereoAtomMCS[key];
                score = GetAtomScore(score, atomMapMCS, reactant, product);
                IDictionary<IBond, IBond> bondMaps = MakeBondMapsOfAtomMaps(rMol, pMol, atomsMCS);

                if (rMol.Bonds.Count > 1 && pMol.Bonds.Count > 1)
                {
                    List<Object> subgraphRList = GetMappedFragment(rMol, atomMapMCS.Keys);

                    double rscore = GetRingMatchScore(subgraphRList);
                    List<Object> subgraphPList = GetMappedFragment(pMol, atomMapMCS.Values);
                    double pscore = GetRingMatchScore(subgraphPList);
                    score = rscore + pscore;
                }
                score = GetBondScore(score, bondMaps);

                if (!stereoMatchFlag)
                {
                    stereoMatchFlag = true;
                }
                stereoScoreMap[key] = score;
            }
            return stereoMatchFlag;
        }

        private int GetFragmentCount(IAtomContainer molecule)
        {
            bool fragmentFlag = true;
            var fragmentMolSet = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
            int countFrag = 0;
            if (molecule.Atoms.Count > 0)
            {
                fragmentFlag = ConnectivityChecker.IsConnected(molecule);
                if (!fragmentFlag)
                {
                    fragmentMolSet.AddRange(ConnectivityChecker.PartitionIntoMolecules(molecule));
                }
                else
                {
                    fragmentMolSet.Add(molecule);
                }
                countFrag = fragmentMolSet.Count;
            }
            return countFrag;
        }

        /// <summary>
        /// Get bond order value as <see cref="BondOrder"/>
        /// </summary>
        /// <param name="srcOrder">numerical bond order</param>
        /// <returns>the bond order type for the given numerical bond order</returns>
        public static BondOrder ConvertOrder(double srcOrder)
        {
            if (srcOrder > 3.5)
            {
                return BondOrder.Quadruple;
            }
            if (srcOrder > 2.5)
            {
                return BondOrder.Triple;
            }
            if (srcOrder > 1.5)
            {
                return BondOrder.Double;
            }
            if (srcOrder > 0.5)
            {
                return BondOrder.Single;
            }
            return BondOrder.Unset;
        }

        /// <summary>
        /// Get bond order value as <see cref="int"/> value.
        /// </summary>
        /// <param name="bond">The <see cref="IBond"/> for which the order is returned.</param>
        /// <returns>1 for a single bond, 2 for a double bond, 3 for a triple bond, 4 for a quadruple bond,
        ///              and 0 for any other bond type.</returns>
        public static int ConvertBondOrder(IBond bond)
        {
            return bond.Order.Numeric;
        }

        /// <summary>
        /// Get stereo value as integer
        /// </summary>
        public static int ConvertBondStereo(IBond bond)
        {
            int value = 0;
            switch (bond.Stereo.Ordinal)
            {
                case BondStereo.O.Up:
                case BondStereo.O.UpInverted:
                    value = 1;
                    break;
                case BondStereo.O.Down:
                case BondStereo.O.DownInverted:
                    value = 6;
                    break;
                case BondStereo.O.UpOrDown:
                case BondStereo.O.UpOrDownInverted:
                    value = 4;
                    break;
                case BondStereo.O.EOrZ:
                    value = 3;
                    break;
                default:
                    value = 0;
                    break;
            }
            return value;
        }

        /// <summary>
        /// Get stereo value as Stereo enum
        /// </summary>
        public static BondStereo ConvertStereo(int stereoValue)
        {
            BondStereo stereo = BondStereo.None;
            if (stereoValue == 1)
            {
                // up bond
                stereo = BondStereo.Up;
            }
            else if (stereoValue == 6)
            {
                // down bond
                stereo = BondStereo.Down;
            }
            else if (stereoValue == 0)
            {
                // bond has no stereochemistry
                stereo = BondStereo.None;
            }
            else if (stereoValue == 4)
            {
                //up or down bond
                stereo = BondStereo.UpOrDown;
            }
            else if (stereoValue == 3)
            {
                //e or z undefined
                stereo = BondStereo.EOrZ;
            }
            return stereo;
        }
    }
}
