/* Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 * You should have received sourceAtom copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.SMSD.Helper;
using NCDK.Tools.Manipulator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.SMSD.Algorithms.RGraphs
{
    /// <summary>
    /// This algorithm derives from the algorithm described in
    /// [Tonnelier, C. and Jauffret, Ph. and Hanser, Th. and Jauffret, Ph. and Kaufmann, G.,
    /// Machine Learning of generic reactions:
    /// 3. An efficient algorithm for maximal common substructure determination,
    /// Tetrahedron Comput. Methodol., 1990, 3:351-358] and modified in the thesis of
    /// T. Hanser [Unknown BibTeXML type: HAN93].
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public class CDKRMapHandler
    {
        public CDKRMapHandler()
        {
        }

        /// <summary>
        /// Returns source molecule
        /// </summary>
        /// <returns>the source</returns>
        public IAtomContainer Source { get; set; }

        /// <summary>
        /// Returns target molecule
        /// </summary>
        /// <returns>the target</returns>
        public IAtomContainer Target { get; set; }

        /// <summary>
        /// This function calculates all the possible combinations of MCS
        /// </summary>
        /// <param name="molecule1"></param>
        /// <param name="molecule2"></param>
        /// <param name="shouldMatchBonds"></param>
        /// <exception cref="CDKException"></exception>
        public void CalculateOverlapsAndReduce(IAtomContainer molecule1, IAtomContainer molecule2, bool shouldMatchBonds)
        {
            Source = molecule1;
            Target = molecule2;

            Mappings = new List<IReadOnlyDictionary<int, int>>();

            if ((Source.Atoms.Count == 1) || (Target.Atoms.Count == 1))
            {
                List<CDKRMap> overlaps = CDKMCS.CheckSingleAtomCases(Source, Target);
                int nAtomsMatched = overlaps.Count;
                nAtomsMatched = (nAtomsMatched > 0) ? 1 : 0;
                if (nAtomsMatched > 0)
                {
                    /* UnComment this to get one Unique Mapping */
                    //List reducedList = RemoveRedundantMappingsForSingleAtomCase(overlaps);
                    //int counter = 0;
                    IdentifySingleAtomsMatchedParts(overlaps, Source, Target);
                }
            }
            else
            {
                var overlaps = CDKMCS.Search(Source, Target, new BitArray(Source.Bonds.Count), new BitArray(Target.Bonds.Count), true,true, shouldMatchBonds);
                var reducedList = RemoveSubGraph(overlaps);
                var allMaxOverlaps = GetAllMaximum(reducedList);
                while (allMaxOverlaps.Count != 0)
                {
                    var maxOverlapsAtoms = MakeAtomsMapOfBondsMap(allMaxOverlaps.Peek(), Source, Target);
                    IdentifyMatchedParts(maxOverlapsAtoms, Source, Target);
                    allMaxOverlaps.Pop();
                }
            }

            FinalMappings.Instance.Set(Mappings);
        }

        /// <summary>
        /// This function calculates only one solution (exact) because we are looking at the
        /// molecules which are exactly same in terms of the bonds and atoms determined by the
        /// Fingerprint
        /// </summary>
        /// <param name="molecule1"></param>
        /// <param name="molecule2"></param>
        /// <param name="shouldMatchBonds"></param>
        /// <exception cref="CDKException"></exception>
        public void CalculateOverlapsAndReduceExactMatch(IAtomContainer molecule1, IAtomContainer molecule2, bool shouldMatchBonds)
        {
            Source = molecule1;
            Target = molecule2;

            Mappings = new List<IReadOnlyDictionary<int, int>>();

            //Console.Out.WriteLine("Searching: ");
            //List overlaps = UniversalIsomorphismTesterBondTypeInSensitive.GetSubgraphAtomsMap(source, target);

            if ((Source.Atoms.Count == 1) || (Target.Atoms.Count == 1))
            {
                List<CDKRMap> overlaps = CDKMCS.CheckSingleAtomCases(Source, Target);
                int nAtomsMatched = overlaps.Count;
                nAtomsMatched = (nAtomsMatched > 0) ? 1 : 0;
                if (nAtomsMatched > 0)
                {
                    IdentifySingleAtomsMatchedParts(overlaps, Source, Target);
                }
            }
            else
            {
                var overlaps = CDKMCS.Search(Source, Target, new BitArray(Source.Bonds.Count), new BitArray(Target.Bonds.Count), true,
                                    true, shouldMatchBonds);

                var reducedList = RemoveSubGraph(overlaps);
                var allMaxOverlaps = GetAllMaximum(reducedList);

                while (allMaxOverlaps.Count != 0)
                {
                    var maxOverlapsAtoms = MakeAtomsMapOfBondsMap(allMaxOverlaps.Peek(), Source,
                        Target);
                    IdentifyMatchedParts(maxOverlapsAtoms, Source, Target);
                    allMaxOverlaps.Pop();
                }
            }
            FinalMappings.Instance.Set(Mappings);
        }

        /// <summary>
        /// This function calculates only one solution (exact) because we are looking at the
        /// molecules which are exactly same in terms of the bonds and atoms determined by the
        /// Fingerprint
        /// </summary>
        /// <param name="molecule1"></param>
        /// <param name="molecule2"></param>
        /// <param name="shouldMatchBonds"></param>
        /// <exception cref="CDKException"></exception>
        public void CalculateSubGraphs(IAtomContainer molecule1, IAtomContainer molecule2, bool shouldMatchBonds)
        {
            Source = molecule1;
            Target = molecule2;

            Mappings = new List<IReadOnlyDictionary<int, int>>();

            //Console.Out.WriteLine("Searching: ");
            //List overlaps = UniversalIsomorphismTesterBondTypeInSensitive.GetSubgraphAtomsMap(source, target);

            if ((Source.Atoms.Count == 1) || (Target.Atoms.Count == 1))
            {

                List<CDKRMap> overlaps = CDKMCS.CheckSingleAtomCases(Source, Target);
                int nAtomsMatched = overlaps.Count;
                nAtomsMatched = (nAtomsMatched > 0) ? 1 : 0;
                if (nAtomsMatched > 0)
                {
                    IdentifySingleAtomsMatchedParts(overlaps, Source, Target);
                }
            }
            else
            {
                var overlaps = CDKMCS.GetSubgraphMaps(Source, Target, shouldMatchBonds);

                var reducedList = RemoveSubGraph(overlaps);
                var allMaxOverlaps = GetAllMaximum(reducedList);

                while (allMaxOverlaps.Count != 0)
                {
                    var maxOverlapsAtoms = MakeAtomsMapOfBondsMap(allMaxOverlaps.Peek(), Source,
                        Target);
                    IdentifyMatchedParts(maxOverlapsAtoms, Source, Target);
                    allMaxOverlaps.Pop();
                }
            }
            FinalMappings.Instance.Set(Mappings);
        }

        /// <summary>
        /// This function calculates only one solution (exact) because we are looking at the
        /// molecules which are exactly same in terms of the bonds and atoms determined by the
        /// Fingerprint
        /// </summary>
        /// <param name="molecule1"></param>
        /// <param name="molecule2"></param>
        /// <param name="shouldMatchBonds"></param>
        /// <exception cref="CDKException"></exception>
        public void CalculateIsomorphs(IAtomContainer molecule1, IAtomContainer molecule2, bool shouldMatchBonds)
        {
            Source = molecule1;
            Target = molecule2;

            Mappings = new List<IReadOnlyDictionary<int, int>>();

            //Console.Out.WriteLine("Searching: ");
            //List overlaps = UniversalIsomorphismTesterBondTypeInSensitive.GetSubgraphAtomsMap(source, target);

            if ((Source.Atoms.Count == 1) || (Target.Atoms.Count == 1))
            {
                List<CDKRMap> overlaps = CDKMCS.CheckSingleAtomCases(Source, Target);
                int nAtomsMatched = overlaps.Count;
                nAtomsMatched = (nAtomsMatched > 0) ? 1 : 0;
                if (nAtomsMatched > 0)
                {
                    IdentifySingleAtomsMatchedParts(overlaps, Source, Target);
                }
            }
            else
            {
                var overlaps = CDKMCS.GetIsomorphMaps(Source, Target, shouldMatchBonds);

                var reducedList = RemoveSubGraph(overlaps);
                var allMaxOverlaps = GetAllMaximum(reducedList);

                while (allMaxOverlaps.Count != 0)
                {
                    var maxOverlapsAtoms = MakeAtomsMapOfBondsMap(allMaxOverlaps.Peek(), Source,
                        Target);
                    IdentifyMatchedParts(maxOverlapsAtoms, Source, Target);
                    allMaxOverlaps.Pop();
                }
            }
            FinalMappings.Instance.Set(Mappings);
        }

        protected static IReadOnlyList<IReadOnlyList<CDKRMap>> RemoveSubGraph(IReadOnlyList<IReadOnlyList<CDKRMap>> overlaps)
        {
            var reducedList = new List<IReadOnlyList<CDKRMap>>(overlaps);

            for (int i = 0; i < overlaps.Count; i++)
            {
                var graphI = overlaps[i];

                for (int j = i + 1; j < overlaps.Count; j++)
                {
                    var graphJ = overlaps[j];

                    // Gi included in Gj or Gj included in Gi then
                    // reduce the irrelevant solution
                    if (graphI.Count != graphJ.Count)
                    {
                        if (IsSubgraph(graphJ, graphI))
                        {
                            reducedList.Remove(graphI);
                        }
                        else if (IsSubgraph(graphI, graphJ))
                        {
                            reducedList.Remove(graphJ);
                        }
                    }
                }
            }
            return reducedList;
        }

        protected static IReadOnlyList<CDKRMap> RemoveRedundantMappingsForSingleAtomCase(IReadOnlyList<CDKRMap> overlaps)
        {
            List<CDKRMap> reducedList = new List<CDKRMap>
            {
                overlaps[0]
            };
            //reducedList.Add(overlaps[1]);
            return reducedList;
        }

        /// <summary>
        ///  This makes sourceAtom map of matching atoms out of sourceAtom map of matching bonds as produced by the Get(Subgraph|Ismorphism)Map methods.
        /// </summary>
        /// <param name="rMapList">The list produced by the getMap method.</param>
        /// <param name="graph1">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="graph2">second molecule. May be an IQueryAtomContainer.</param>
        /// <returns>The mapping found projected on graph1. This is sourceAtom List of CDKRMap objects containing Ids of matching atoms.</returns>
        private static IReadOnlyList<IReadOnlyList<CDKRMap>> MakeAtomsMapOfBondsMap(IReadOnlyList<CDKRMap> rMapList, IAtomContainer graph1, IAtomContainer graph2)
        {
            if (rMapList == null)
            {
                return (null);
            }
            IReadOnlyList<IReadOnlyList<CDKRMap>> result = null;
            if (rMapList.Count == 1)
            {
                result = MakeAtomsMapOfBondsMapSingleBond(rMapList, graph1, graph2);
            }
            else
            {
                List<CDKRMap> resultLocal = new List<CDKRMap>();
                for (int i = 0; i < rMapList.Count; i++)
                {
                    IBond qBond = graph1.Bonds[rMapList[i].Id1];
                    IBond tBond = graph2.Bonds[rMapList[i].Id2];
                    IAtom[] qAtoms = BondManipulator.GetAtomArray(qBond);
                    IAtom[] tAtoms = BondManipulator.GetAtomArray(tBond);
                    for (int j = 0; j < 2; j++)
                    {
                        var bondsConnectedToAtom1j = graph1.GetConnectedBonds(qAtoms[j]);
                        foreach (var bondConnectedToAtom1j in bondsConnectedToAtom1j)
                        {
                            if (bondConnectedToAtom1j != qBond)
                            {
                                IBond testBond = bondConnectedToAtom1j;
                                foreach (var rMap in rMapList)
                                {
                                    IBond testBond2;
                                    if ((rMap).Id1 == graph1.Bonds.IndexOf(testBond))
                                    {
                                        testBond2 = graph2.Bonds[(rMap).Id2];
                                        for (int n = 0; n < 2; n++)
                                        {
                                            var bondsToTest = graph2.GetConnectedBonds(tAtoms[n]);
                                            if (bondsToTest.Contains(testBond2))
                                            {
                                                CDKRMap map;
                                                if (j == n)
                                                {
                                                    map = new CDKRMap(graph1.Atoms.IndexOf(qAtoms[0]),
                                                            graph2.Atoms.IndexOf(tAtoms[0]));
                                                }
                                                else
                                                {
                                                    map = new CDKRMap(graph1.Atoms.IndexOf(qAtoms[1]),
                                                            graph2.Atoms.IndexOf(tAtoms[0]));
                                                }
                                                if (!resultLocal.Contains(map))
                                                {
                                                    resultLocal.Add(map);
                                                }
                                                CDKRMap map2;
                                                if (j == n)
                                                {
                                                    map2 = new CDKRMap(graph1.Atoms.IndexOf(qAtoms[1]),
                                                            graph2.Atoms.IndexOf(tAtoms[1]));
                                                }
                                                else
                                                {
                                                    map2 = new CDKRMap(graph1.Atoms.IndexOf(qAtoms[0]),
                                                            graph2.Atoms.IndexOf(tAtoms[1]));
                                                }
                                                if (!resultLocal.Contains(map2))
                                                {
                                                    resultLocal.Add(map2);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                result = new List<IReadOnlyList<CDKRMap>>
                {
                    resultLocal
                };
            }
            return result;
        }

        /// <summary>
        ///  This makes atom map of matching atoms out of atom map of matching bonds as produced by the Get(Subgraph|Ismorphism)Map methods.
        ///  Added by Asad since CDK one doesn't pick up the correct changes
        /// </summary>
        /// <param name="list">The list produced by the getMap method.</param>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <returns>The mapping found projected on sourceGraph. This is atom List of CDKRMap objects containing Ids of matching atoms.</returns>
        private static IReadOnlyList<IReadOnlyList<CDKRMap>> MakeAtomsMapOfBondsMapSingleBond(IReadOnlyList<CDKRMap> list, IAtomContainer sourceGraph, IAtomContainer targetGraph)
        {
            if (list == null)
            {
                return null;
            }
            var bondMap = new Dictionary<IBond, IBond>(list.Count);
            foreach (var solBondMap in list)
            {
                var id1 = solBondMap.Id1;
                var id2 = solBondMap.Id2;
                var qBond = sourceGraph.Bonds[id1];
                var tBond = targetGraph.Bonds[id2];
                bondMap[qBond] = tBond;
            }
            var result1 = new List<CDKRMap>();
            var result2 = new List<CDKRMap>();
            foreach (var qbond in sourceGraph.Bonds)
            {
                if (bondMap.ContainsKey(qbond))
                {
                    var tbond = bondMap[qbond];
                    CDKRMap map00 = null;
                    CDKRMap map01 = null;
                    CDKRMap map10 = null;
                    CDKRMap map11 = null;

                    if (string.Equals(qbond.Atoms[0].Symbol, tbond.Atoms[0].Symbol, StringComparison.Ordinal)
                     && string.Equals(qbond.Atoms[1].Symbol, tbond.Atoms[1].Symbol, StringComparison.Ordinal))
                    {
                        map00 = new CDKRMap(sourceGraph.Atoms.IndexOf(qbond.Atoms[0]), targetGraph.Atoms.IndexOf(tbond.Atoms[0]));
                        map11 = new CDKRMap(sourceGraph.Atoms.IndexOf(qbond.Atoms[1]), targetGraph.Atoms.IndexOf(tbond.Atoms[1]));
                        if (!result1.Contains(map00))
                        {
                            result1.Add(map00);
                        }
                        if (!result1.Contains(map11))
                        {
                            result1.Add(map11);
                        }
                    }
                    if (string.Equals(qbond.Atoms[0].Symbol, tbond.Atoms[1].Symbol, StringComparison.Ordinal)
                     && string.Equals(qbond.Atoms[1].Symbol, tbond.Atoms[0].Symbol, StringComparison.Ordinal))
                    {
                        map01 = new CDKRMap(sourceGraph.Atoms.IndexOf(qbond.Atoms[0]), targetGraph.Atoms.IndexOf(tbond.Atoms[1]));
                        map10 = new CDKRMap(sourceGraph.Atoms.IndexOf(qbond.Atoms[1]), targetGraph.Atoms.IndexOf(tbond.Atoms[0]));
                        if (!result2.Contains(map01))
                        {
                            result2.Add(map01);
                        }
                        if (!result2.Contains(map10))
                        {
                            result2.Add(map10);
                        }
                    }
                }
            }
            var result = new List<IReadOnlyList<CDKRMap>>();
            if (result1.Count == result2.Count)
            {
                result.Add(result1);
                result.Add(result2);
            }
            else if (result1.Count > result2.Count)
            {
                result.Add(result1);
            }
            else
            {
                result.Add(result2);
            }
            return result;
        }

        protected static IList GetMaximum(IList<IList> overlaps)
        {
            IList list = null;
            int count = 0;
            foreach (var o in overlaps)
            {
                var arrayList = o;
                if (arrayList.Count > count)
                {
                    list = arrayList;
                    count = arrayList.Count;
                }
            }
            return list;
        }

        protected static Stack<IReadOnlyList<CDKRMap>> GetAllMaximum(IReadOnlyList<IReadOnlyList<CDKRMap>> overlaps)
        {
            Stack<IReadOnlyList<CDKRMap>> allMaximumMappings = null;

            int count = -1;

            foreach (var arrayList in overlaps)
            {
                if (arrayList.Count > count)
                {
                    var list = new List<CDKRMap>(arrayList);
                    count = arrayList.Count;
                    allMaximumMappings = new Stack<IReadOnlyList<CDKRMap>>();
                    allMaximumMappings.Push(list);
                }
                else if (arrayList.Count == count)
                {
                    var list = new List<CDKRMap>(arrayList);
                    count = arrayList.Count;
                    allMaximumMappings.Push(list);
                }
            }
            return allMaximumMappings;
        }

        protected void IdentifyMatchedParts(IReadOnlyList<IReadOnlyList<CDKRMap>> list, IAtomContainer source, IAtomContainer target)
        {
            var array1 = new List<IAtom>();
            var array2 = new List<IAtom>();

            // We have serial numbers of the bonds/Atoms to delete Now we will
            // collect the actual bond/Atoms rather than serial number for deletion.
            // RonP flag check whether reactant is mapped on product or Vise Versa
            foreach (var rMap in list)
            {
                var atomNumbersFromContainer = new SortedDictionary<int, int>();
                foreach (var rmap in rMap)
                {
                    IAtom sourceAtom = source.Atoms[rmap.Id1];
                    IAtom targetAtom = target.Atoms[rmap.Id2];

                    array1.Add(sourceAtom);
                    array2.Add(targetAtom);

                    int indexI = source.Atoms.IndexOf(sourceAtom);
                    int indexJ = target.Atoms.IndexOf(targetAtom);

                    atomNumbersFromContainer[indexI] = indexJ;
                }

                // Added the Mapping Numbers to the FinalMapping*
                Mappings.Add(atomNumbersFromContainer);
            }
        }

        protected void IdentifySingleAtomsMatchedParts(List<CDKRMap> list, IAtomContainer source, IAtomContainer target)
        {
            List<IAtom> array1 = new List<IAtom>();
            List<IAtom> array2 = new List<IAtom>();

            // We have serial numbers of the bonds/Atoms to delete Now we will
            // collect the actual bond/Atoms rather than serial number for deletion.
            // RonP flag check whether reactant is mapped on product or Vise Versa
            
            SortedDictionary<int, int> atomNumbersFromContainer = new SortedDictionary<int, int>();

            foreach (var rmap in list)
            {
                IAtom sAtom = source.Atoms[rmap.Id1];
                IAtom tAtom = target.Atoms[rmap.Id2];

                array1.Add(sAtom);
                array2.Add(tAtom);

                int indexI = source.Atoms.IndexOf(sAtom);
                int indexJ = target.Atoms.IndexOf(tAtom);

                atomNumbersFromContainer[indexI] = indexJ;

                // Added the Mapping Numbers to the FinalMapping*
                Mappings.Add(atomNumbersFromContainer);
            }
        }

        protected static bool IsSubgraph(IReadOnlyList<CDKRMap> rmaps1, IReadOnlyList<CDKRMap> rmaps2)
        {
            var rmaps2clone = new List<CDKRMap>(rmaps2);
            foreach (var rmap1 in rmaps1)
            {
                bool found = false;
                for (int i = 0; i < rmaps2clone.Count; ++i)
                {
                    CDKRMap rmap2 = rmaps2clone[i];
                    if (IsSameRMap(rmap1, rmap2))
                    {
                        rmaps2clone.RemoveAt(i);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }
            return true;
        }

        protected static bool IsSameRMap(CDKRMap sourceRMap, CDKRMap targetRMap)
        {
            return sourceRMap.Id1 == targetRMap.Id1 && sourceRMap.Id2 == targetRMap.Id2 ? true : false;
        }

        /// <summary>
        /// mapping solutions
        /// </summary>
        public IList<IReadOnlyDictionary<int, int>> Mappings { get; internal set; }

        /// <summary>
        /// true if a time out occurred else false
        /// </summary>
        public bool IsTimedOut { get; set; } = false;
    }
}

