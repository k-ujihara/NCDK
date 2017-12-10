/*
 *
 * Copyright (C) 2007-2010  Syed Asad Rahman {asad@ebi.atomContainer.uk}
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
 * You should have received atom copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 *  Copyright (C) 2002-2007  Stephane Werner <mail@ixelis.net>
 *
 *  This code has been kindly provided by Stephane Werner
 *  and Thierry Hanser from IXELIS mail@ixelis.net
 *
 *  IXELIS sarl - Semantic Information Systems
 *  17 rue des C???res 67200 Strasbourg, France
 *  Tel/Fax : +33(0)3 88 27 81 39 Email: mail@ixelis.net
 *
 *  CDK Contact: cdk-devel@lists.sf.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received atom copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Algorithms.Matchers;
using NCDK.SMSD.Global;
using NCDK.SMSD.Tools;
using NCDK.Tools.Manipulator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NCDK.SMSD.Algorithms.RGraph
{
    /// <summary>
    ///  This class implements atom multipurpose structure comparison tool.
    ///  It allows to find maximal common substructure, find the
    ///  mapping of atom substructure in another structure, and the mapping of
    ///  two isomorphic structures.
    /// </summary>
    /// <remarks>
    ///  <para>Structure comparison may be associated to bondA1 constraints
    ///  (mandatory bonds, e.graphContainer. scaffolds, reaction cores,...) on each source graph.
    ///  The constraint flexibility allows atom number of interesting queries.
    ///  The substructure analysis relies on the CDKRGraph generic class (see: CDKRGraph)
    ///  This class implements the link between the CDKRGraph model and the
    ///  the CDK model in this way the CDKRGraph remains independant and may be used
    ///  in other contexts.</para>
    ///
    ///  <para>This algorithm derives from the algorithm described in
    ///  <token>cdk-cite-HAN90</token> and modified in the thesis of T. Hanser <token>cdk-cite-HAN93</token>.</para>
    ///
    ///  <para>With the <see cref="IsSubgraph(IAtomContainer, IAtomContainer, bool)"/> method, the second, and only the second
    ///  argument <b>may</b> be atom <see cref="IQueryAtomContainer"/>, which allows one to do MQL like queries.
    ///  The first IAtomContainer must never be an <see cref="IQueryAtomContainer"/>.
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.SMSD.Algorithms.RGraph.CDKMCS_Example.cs"]/*' />
    /// </para>
    /// <note type="warning">
    ///    As atom result of the adjacency perception used in this algorithm
    ///    there is atom single limitation : cyclopropane and isobutane are seen as isomorph
    ///    This is due to the fact that these two compounds are the only ones where
    ///    each bondA1 is connected two each other bondA1 (bonds are fully conected)
    ///    with the same number of bonds and still they have different structures
    ///    The algotihm could be easily enhanced with atom simple atom mapping manager
    ///    to provide an atom level overlap definition that would reveal this case.
    ///    We decided not to penalize the whole procedure because of one single
    ///    exception query. Furthermore isomorphism may be discarded since the number of atoms are
    ///    not the same (3 != 4) and in most case this will be already
    ///    screened out by atom fingerprint based filtering.
    ///    It is possible to add atom special treatment for this special query.
    ///    Be reminded that this algorithm matches bonds only.
    /// </note>
    /// </remarks>
    // @author      Stephane Werner from IXELIS mail@ixelis.net, Syed Asad Rahman <asad@ebi.ebi.uk> (modified the orignal code)
    // @cdk.created 2002-07-17
    // @cdk.require java1.5+
    // @cdk.module  smsd
    // @cdk.githash
    public class CDKMCS
    {
        const int ID1 = 0;
        const int ID2 = 1;
        private static TimeManager timeManager = null;

        ///////////////////////////////////////////////////////////////////////////
        //                            Query Methods
        //
        // This methods are simple applications of the CDKRGraph model on atom containers
        // using different constrains and search options. They give an example of the
        // most common queries but of course it is possible to define other type of
        // queries exploiting the constrain and option combinations
        //
        //
        // Isomorphism search
        /// <summary>
        /// Tests if sourceGraph and targetGraph are isomorph.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>true if the 2 molecule are isomorph</returns>
        /// <exception cref="CDKException">if the first molecule is an instance of IQueryAtomContainer</exception>
        public static bool IsIsomorph(IAtomContainer sourceGraph, IAtomContainer targetGraph, bool shouldMatchBonds)
        {
            if (sourceGraph is IQueryAtomContainer)
            {
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");
            }

            if (targetGraph.Atoms.Count != sourceGraph.Atoms.Count)
            {
                return false;
            }
            // check single atom case
            if (targetGraph.Atoms.Count == 1)
            {
                IAtom atom = sourceGraph.Atoms[0];
                IAtom atom2 = targetGraph.Atoms[0];
                if (atom is IQueryAtom)
                {
                    IQueryAtom qAtom = (IQueryAtom)atom;
                    return qAtom.Matches(targetGraph.Atoms[0]);
                }
                else if (atom2 is IQueryAtom)
                {
                    IQueryAtom qAtom = (IQueryAtom)atom2;
                    return qAtom.Matches(sourceGraph.Atoms[0]);
                }
                else
                {
                    string atomSymbol = atom2.Symbol;
                    return sourceGraph.Atoms[0].Symbol.Equals(atomSymbol);
                }
            }
            return (GetIsomorphMap(sourceGraph, targetGraph, shouldMatchBonds) != null);
        }

        /// <summary>
        /// Returns the first isomorph mapping found or null.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>the first isomorph mapping found projected of sourceGraph. This is atom List of CDKRMap objects containing Ids of matching bonds.</returns>
        /// <exception cref="CDKException"></exception>
        public static IList<CDKRMap> GetIsomorphMap(IAtomContainer sourceGraph, IAtomContainer targetGraph, bool shouldMatchBonds)
        {
            if (sourceGraph is IQueryAtomContainer)
            {
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");
            }

            IList<CDKRMap> result = null;

            var rMapsList = Search(sourceGraph, targetGraph, GetBitSet(sourceGraph),
                        GetBitSet(targetGraph), false, false, shouldMatchBonds);

            if (rMapsList.Count != 0)
            {
                result = rMapsList[0];
            }

            return result;
        }

        /// <summary>
        /// Returns the first isomorph 'atom mapping' found for targetGraph in sourceGraph.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>the first isomorph atom mapping found projected on sourceGraph.</returns>
        /// This is atom List of CDKRMap objects containing Ids of matching atoms.
        /// <exception cref="CDKException">if the first molecules is not an instance of <see cref="IQueryAtomContainer"/></exception>
        public static IList<CDKRMap> GetIsomorphAtomsMap(IAtomContainer sourceGraph, IAtomContainer targetGraph, bool shouldMatchBonds)
        {
            if (sourceGraph is IQueryAtomContainer)
            {
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");
            }

            IList<CDKRMap> list = CheckSingleAtomCases(sourceGraph, targetGraph);
            if (list == null)
            {
                return MakeAtomsMapOfBondsMap(CDKMCS.GetIsomorphMap(sourceGraph, targetGraph, shouldMatchBonds),
                        sourceGraph, targetGraph);
            }
            else if (list.Count == 0)
            {
                return null;
            }
            else
            {
                return list;
            }
        }

        /// <summary>
        /// Returns all the isomorph 'mappings' found between two
        /// atom containers.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>the list of all the 'mappings'</returns>
        /// <exception cref="CDKException"></exception>
        public static IList<IList<CDKRMap>> GetIsomorphMaps(IAtomContainer sourceGraph, IAtomContainer targetGraph, bool shouldMatchBonds)
        {
            return Search(sourceGraph, targetGraph, GetBitSet(sourceGraph), GetBitSet(targetGraph), true, true, shouldMatchBonds);
        }

        //
        // Subgraph search
        /// <summary>
        /// Returns all the subgraph 'bondA1 mappings' found for targetGraph in sourceGraph.
        /// This is an ArrayList of ArrayLists of CDKRMap objects.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>the list of all the 'mappings' found projected of sourceGraph</returns>
        /// <exception cref="CDKException"></exception>
        public static IList<IList<CDKRMap>> GetSubgraphMaps(IAtomContainer sourceGraph, IAtomContainer targetGraph,
                bool shouldMatchBonds)
        {
            return Search(sourceGraph, targetGraph, new BitArray(sourceGraph.Bonds.Count), GetBitSet(targetGraph), true, true, shouldMatchBonds);
        }

        /// <summary>
        /// Returns the first subgraph 'bondA1 mapping' found for targetGraph in sourceGraph.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>the first subgraph bondA1 mapping found projected on sourceGraph. This is atom List of CDKRMap objects containing Ids of matching bonds.</returns>
        /// <exception cref="CDKException"></exception>
        public static IList<CDKRMap> GetSubgraphMap(IAtomContainer sourceGraph, IAtomContainer targetGraph, bool shouldMatchBonds)
        {
            IList<CDKRMap> result = null;
            var rMapsList = Search(sourceGraph, targetGraph, new BitArray(sourceGraph.Bonds.Count), GetBitSet(targetGraph), false,
                        false, shouldMatchBonds);

            if (rMapsList.Count != 0)
            {
                result = rMapsList[0];
            }

            return result;
        }

        /// <summary>
        /// Returns all subgraph 'atom mappings' found for targetGraph in sourceGraph.
        /// This is an ArrayList of ArrayLists of CDKRMap objects.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>all subgraph atom mappings found projected on sourceGraph. This is atom
        ///             List of CDKRMap objects containing Ids of matching atoms.</returns>
        /// <exception cref="CDKException"></exception>
        public static IList<IList<CDKRMap>> GetSubgraphAtomsMaps(IAtomContainer sourceGraph, IAtomContainer targetGraph,
                bool shouldMatchBonds)
        {
            IList<CDKRMap> list = CheckSingleAtomCases(sourceGraph, targetGraph);
            if (list == null)
            {
                return MakeAtomsMapsOfBondsMaps(CDKMCS.GetSubgraphMaps(sourceGraph, targetGraph, shouldMatchBonds),
                        sourceGraph, targetGraph);
            }
            else
            {
                IList<IList<CDKRMap>> atomsMap = new List<IList<CDKRMap>>();
                atomsMap.Add(list);
                return atomsMap;
            }
        }

        /// <summary>
        /// Returns the first subgraph 'atom mapping' found for targetGraph in sourceGraph.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>the first subgraph atom mapping found projected on sourceGraph.
        ///            This is atom List of CDKRMap objects containing Ids of matching atoms.</returns>
        /// <exception cref="CDKException"></exception>
        public static IList<CDKRMap> GetSubgraphAtomsMap(IAtomContainer sourceGraph, IAtomContainer targetGraph, bool shouldMatchBonds)
        {
            IList<CDKRMap> list = CheckSingleAtomCases(sourceGraph, targetGraph);
            if (list == null)
            {
                return MakeAtomsMapOfBondsMap(CDKMCS.GetSubgraphMap(sourceGraph, targetGraph, shouldMatchBonds),
                        sourceGraph, targetGraph);
            }
            else if (list.Count == 0)
            {
                return null;
            }
            else
            {
                return list;
            }
        }

        /// <summary>
        /// Tests if targetGraph atom subgraph of sourceGraph.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>true if targetGraph atom subgraph on sourceGraph</returns>
        /// <exception cref="CDKException"></exception>
        public static bool IsSubgraph(IAtomContainer sourceGraph, IAtomContainer targetGraph, bool shouldMatchBonds)
        {
            if (sourceGraph is IQueryAtomContainer)
            {
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");
            }

            if (targetGraph.Atoms.Count > sourceGraph.Atoms.Count)
            {
                return false;
            }
            // test for single atom case
            if (targetGraph.Atoms.Count == 1)
            {
                IAtom atom = targetGraph.Atoms[0];
                for (int i = 0; i < sourceGraph.Atoms.Count; i++)
                {
                    IAtom atom2 = sourceGraph.Atoms[i];
                    if (atom is IQueryAtom)
                    {
                        IQueryAtom qAtom = (IQueryAtom)atom;
                        if (qAtom.Matches(atom2))
                        {
                            return true;
                        }
                    }
                    else if (atom2 is IQueryAtom)
                    {
                        IQueryAtom qAtom = (IQueryAtom)atom2;
                        if (qAtom.Matches(atom))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (atom2.Symbol.Equals(atom.Symbol))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            if (!TestSubgraphHeuristics(sourceGraph, targetGraph))
            {
                return false;
            }
            return (GetSubgraphMap(sourceGraph, targetGraph, shouldMatchBonds) != null);
        }

        // Maximum common substructure search
        /// <summary>
        /// Returns all the maximal common substructure between 2 atom containers.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>the list of all the maximal common substructure
        ///             found projected of sourceGraph (list of <see cref="IAtomContainer"/>)</returns>
        /// <exception cref="CDKException"></exception>
        public static IList<IAtomContainer> GetOverlaps(IAtomContainer sourceGraph, IAtomContainer targetGraph, bool shouldMatchBonds)
        {
            IList<IList<CDKRMap>> rMapsList = Search(sourceGraph, targetGraph, new BitArray(sourceGraph.Bonds.Count), new BitArray(targetGraph.Bonds.Count), true, false,
                shouldMatchBonds);

            // projection on G1
            IList<IAtomContainer> graphList = ProjectList(rMapsList, sourceGraph, ID1);

            // reduction of set of solution (isomorphism and substructure
            // with different 'mappings'

            return GetMaximum(graphList, shouldMatchBonds);
        }

        /// <summary>
        /// Transforms an AtomContainer into atom BitArray (which's size = number of bondA1
        /// in the atomContainer, all the bit are set to true).
        ///
        /// <param name="atomContainer">AtomContainer to transform</param>
        /// <returns>The bitSet</returns>
        /// </summary>
        public static BitArray GetBitSet(IAtomContainer atomContainer)
        {
            BitArray bitSet;
            int size = atomContainer.Bonds.Count;

            if (size != 0)
            {
                bitSet = new BitArray(size);
                for (int i = 0; i < size; i++)
                {
                    bitSet.Set(i, true);
                }
            }
            else
            {
                bitSet = new BitArray(0);
            }

            return bitSet;
        }

        //
        //          Internal methods
        /// <summary>
        /// Builds the CDKRGraph ( resolution graph ), from two atomContainer
        /// (description of the two molecules to compare)
        /// This is the interface point between the CDK model and
        /// the generic MCSS algorithm based on the RGRaph.
        /// </summary>
        /// <param name="sourceGraph">Description of the first molecule</param>
        /// <param name="targetGraph">Description of the second molecule</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>the rGraph</returns>
        /// <exception cref="CDKException"></exception>
        public static CDKRGraph BuildRGraph(IAtomContainer sourceGraph, IAtomContainer targetGraph, bool shouldMatchBonds)
        {
            CDKRGraph rGraph = new CDKRGraph();
            NodeConstructor(rGraph, sourceGraph, targetGraph, shouldMatchBonds);
            ArcConstructor(rGraph, sourceGraph, targetGraph);
            return rGraph;
        }

        /// <summary>
        /// General Rgraph parsing method (usually not used directly)
        /// This method is the entry point for the recursive search
        /// adapted to the atom container input.
        /// </summary>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <param name="sourceBitSet">initial condition ( bonds from sourceGraph that must be contains in the solution )</param>
        /// <param name="targetBitSet">initial condition ( bonds from targetGraph that must be contains in the solution )</param>
        /// <param name="findAllStructure">if false stop at the first structure found</param>
        /// <param name="findAllMap">if true search all the 'mappings' for one same structure</param>
        /// <param name="shouldMatchBonds"></param>
        /// <returns>atom List of Lists of CDKRMap objects that represent the search solutions</returns>
        /// <exception cref="CDKException"></exception>
        public static IList<IList<CDKRMap>> Search(IAtomContainer sourceGraph, IAtomContainer targetGraph,
                BitArray sourceBitSet, BitArray targetBitSet, bool findAllStructure, bool findAllMap,
                bool shouldMatchBonds)
        {
            // handle single query atom case separately
            if (targetGraph.Atoms.Count == 1)
            {
                var matches = new List<IList<CDKRMap>>();
                IAtom queryAtom = targetGraph.Atoms[0];

                // we can have a IQueryAtomContainer *or* an IAtomContainer
                if (queryAtom is IQueryAtom)
                {
                    IQueryAtom qAtom = (IQueryAtom)queryAtom;
                    foreach (var atom in sourceGraph.Atoms)
                    {
                        if (qAtom.Matches(atom))
                        {
                            List<CDKRMap> lmap = new List<CDKRMap>();
                            lmap.Add(new CDKRMap(sourceGraph.Atoms.IndexOf(atom), 0));
                            matches.Add(lmap);
                        }
                    }
                }
                else
                {
                    foreach (var atom in sourceGraph.Atoms)
                    {
                        if (queryAtom.Symbol.Equals(atom.Symbol))
                        {
                            List<CDKRMap> lmap = new List<CDKRMap>();
                            lmap.Add(new CDKRMap(sourceGraph.Atoms.IndexOf(atom), 0));
                            matches.Add(lmap);
                        }
                    }
                }
                return matches;
            }

            // reset result
            var rMapsList = new List<IList<CDKRMap>>();
            // build the CDKRGraph corresponding to this problem
            CDKRGraph rGraph = BuildRGraph(sourceGraph, targetGraph, shouldMatchBonds);
            SetTimeManager(new TimeManager());
            // parse the CDKRGraph with the given constrains and options
            rGraph.Parse(sourceBitSet, targetBitSet, findAllStructure, findAllMap, GetTimeManager());
            var solutionList = rGraph.Solutions;

            // conversions of CDKRGraph's internal solutions to G1/G2 mappings
            foreach (var set in solutionList)
            {
                rMapsList.Add(rGraph.BitSetToRMap(set));
            }

            return rMapsList;
        }

        //////////////////////////////////////
        //    Manipulation tools
        /// <summary>
        /// Projects atom list of CDKRMap on atom molecule.
        /// </summary>
        /// <param name="rMapList">the list to project</param>
        /// <param name="graph">the molecule on which project</param>
        /// <param name="key">the key in the CDKRMap of the molecule graph</param>
        /// <returns>an AtomContainer</returns>
        public static IAtomContainer Project(IList<CDKRMap> rMapList, IAtomContainer graph, int key)
        {
            IAtomContainer atomContainer = graph.Builder.NewAtomContainer();

            IDictionary<IAtom, IAtom> table = new Dictionary<IAtom, IAtom>();
            IAtom atom1;
            IAtom atom2;
            IAtom atom;
            IBond bond;

            foreach (var rMap in rMapList)
            {
                if (key == CDKMCS.ID1)
                {
                    bond = graph.Bonds[rMap.Id1];
                }
                else
                {
                    bond = graph.Bonds[rMap.Id2];
                }

                atom = bond.Atoms[0];
                if (!table.TryGetValue(atom, out atom1))
                {
                    atom1 = (IAtom)atom.Clone();
                    atomContainer.Atoms.Add(atom1);
                    table[atom] = atom1;
                }

                atom = bond.Atoms[1];
                if (!table.TryGetValue(atom, out atom2))
                {
                    atom2 = (IAtom)atom.Clone();
                    atomContainer.Atoms.Add(atom2);
                    table[atom] = atom2;
                }
                IBond newBond = graph.Builder.NewBond(atom1, atom2, bond.Order);
                newBond.IsAromatic = bond.IsAromatic;
                atomContainer.Bonds.Add(newBond);
            }
            return atomContainer;
        }

        /// <summary>
        /// Projects atom list of RMapsList on atom molecule.
        /// </summary>
        /// <param name="rMapsList">list of RMapsList to project</param>
        /// <param name="graph">the molecule on which project</param>
        /// <param name="key">the key in the CDKRMap of the molecule graph</param>
        /// <returns>atom list of AtomContainer</returns>
        public static IList<IAtomContainer> ProjectList(IList<IList<CDKRMap>> rMapsList, IAtomContainer graph, int key)
        {
            IList<IAtomContainer> graphList = new List<IAtomContainer>();

            foreach (var rMapList in rMapsList)
            {
                IAtomContainer atomContainer = Project(rMapList, graph, key);
                graphList.Add(atomContainer);
            }
            return graphList;
        }

        /// <summary>
        /// Removes all redundant solution.
        /// </summary>
        /// <param name="graphList">the list of structure to clean</param>
        /// <returns>the list cleaned</returns>
        /// <exception cref="CDKException">if there is atom problem in obtaining subgraphs</exception>
        private static IList<IAtomContainer> GetMaximum(IList<IAtomContainer> graphList, bool shouldMatchBonds)
        {
            List<IAtomContainer> reducedGraphList = new List<IAtomContainer>(graphList);

            for (int i = 0; i < graphList.Count; i++)
            {
                IAtomContainer graphI = graphList[i];

                for (int j = i + 1; j < graphList.Count; j++)
                {
                    IAtomContainer graphJ = graphList[j];

                    // Gi included in Gj or Gj included in Gi then
                    // reduce the irrelevant solution
                    if (IsSubgraph(graphJ, graphI, shouldMatchBonds))
                    {
                        reducedGraphList.Remove(graphI);
                    }
                    else if (IsSubgraph(graphI, graphJ, shouldMatchBonds))
                    {
                        reducedGraphList.Remove(graphJ);
                    }
                }
            }
            return reducedGraphList;
        }

        /// <summary>
        ///  Checks for single atom cases before doing subgraph/isomorphism search
        /// </summary>
        /// <param name="sourceGraph">AtomContainer to match on. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">AtomContainer as query. May be an IQueryAtomContainer.</param>
        /// <returns>List of List of CDKRMap objects for the Atoms (not Bonds!), null if no single atom case</returns>
        /// <exception cref="CDKException">if the first molecule is an instance of IQueryAtomContainer</exception>
        public static List<CDKRMap> CheckSingleAtomCases(IAtomContainer sourceGraph, IAtomContainer targetGraph)
        {
            if (sourceGraph is IQueryAtomContainer)
            {
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");
            }

            if (targetGraph.Atoms.Count == 1)
            {
                List<CDKRMap> arrayList = new List<CDKRMap>();
                IAtom atom = targetGraph.Atoms[0];
                if (atom is IQueryAtom)
                {
                    IQueryAtom qAtom = (IQueryAtom)atom;
                    for (int i = 0; i < sourceGraph.Atoms.Count; i++)
                    {
                        if (qAtom.Matches(sourceGraph.Atoms[i]))
                        {
                            arrayList.Add(new CDKRMap(i, 0));
                        }
                    }
                }
                else
                {
                    string atomSymbol = atom.Symbol;
                    for (int i = 0; i < sourceGraph.Atoms.Count; i++)
                    {
                        if (sourceGraph.Atoms[i].Symbol.Equals(atomSymbol))
                        {
                            arrayList.Add(new CDKRMap(i, 0));
                        }
                    }
                }
                return arrayList;
            }
            else if (sourceGraph.Atoms.Count == 1)
            {
                List<CDKRMap> arrayList = new List<CDKRMap>();
                IAtom atom = sourceGraph.Atoms[0];
                for (int i = 0; i < targetGraph.Atoms.Count; i++)
                {
                    IAtom atom2 = targetGraph.Atoms[i];
                    if (atom2 is IQueryAtom)
                    {
                        IQueryAtom qAtom = (IQueryAtom)atom2;
                        if (qAtom.Matches(atom))
                        {
                            arrayList.Add(new CDKRMap(0, i));
                        }
                    }
                    else
                    {
                        if (atom2.Symbol.Equals(atom.Symbol))
                        {
                            arrayList.Add(new CDKRMap(0, i));
                        }
                    }
                }
                return arrayList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  This makes maps of matching atoms out of atom maps of matching bonds as produced by the Get(Subgraph|Ismorphism)Maps methods.
        /// </summary>
        /// <param name="list">The list produced by the getMap method.</param>
        /// <param name="sourceGraph">The first atom container. Must not be atom IQueryAtomContainer.</param>
        /// <param name="targetGraph">The second one (first and second as in getMap). May be an QueryAtomContaienr.</param>
        /// <returns>A Vector of Vectors of CDKRMap objects of matching Atoms.</returns>
        public static IList<IList<CDKRMap>> MakeAtomsMapsOfBondsMaps(IList<IList<CDKRMap>> list, IAtomContainer sourceGraph, IAtomContainer targetGraph)
        {
            if (list == null)
            {
                return list;
            }
            if (targetGraph.Atoms.Count == 1)
            {
                return list; // since the RMap is already an atom-atom mapping
            }
            List<IList<CDKRMap>> result = new List<IList<CDKRMap>>();
            foreach (var l2 in list)
            {
                result.Add(MakeAtomsMapOfBondsMap(l2, sourceGraph, targetGraph));
            }
            return result;
        }

        /// <summary>
        ///  This makes atom map of matching atoms out of atom map of matching bonds as produced by the Get(Subgraph|Ismorphism)Map methods.
        /// </summary>
        /// <param name="list">The list produced by the getMap method.</param>
        /// <param name="sourceGraph">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="targetGraph">second molecule. May be an IQueryAtomContainer.</param>
        /// <returns>The mapping found projected on sourceGraph. This is atom List of CDKRMap objects containing Ids of matching atoms.</returns>
        public static IList<CDKRMap> MakeAtomsMapOfBondsMap(IList<CDKRMap> list, IAtomContainer sourceGraph, IAtomContainer targetGraph)
        {
            if (list == null)
            {
                return (list);
            }
            List<CDKRMap> result = new List<CDKRMap>();
            for (int i = 0; i < list.Count; i++)
            {
                IBond bond1 = sourceGraph.Bonds[list[i].Id1];
                IBond bond2 = targetGraph.Bonds[list[i].Id2];
                IAtom[] atom1 = BondManipulator.GetAtomArray(bond1);
                IAtom[] atom2 = BondManipulator.GetAtomArray(bond2);
                for (int j = 0; j < 2; j++)
                {
                    var bondsConnectedToAtom1j = sourceGraph.GetConnectedBonds(atom1[j]);
                    foreach (var bondConnectedToAtom1j in bondsConnectedToAtom1j)
                    {
                        if (bondConnectedToAtom1j != bond1)
                        {
                            IBond testBond = bondConnectedToAtom1j;
                            for (int m = 0; m < list.Count; m++)
                            {
                                IBond testBond2;
                                if ((list[m]).Id1 == sourceGraph.Bonds.IndexOf(testBond))
                                {
                                    testBond2 = targetGraph.Bonds[(list[m]).Id2];
                                    for (int n = 0; n < 2; n++)
                                    {
                                        var bondsToTest = targetGraph.GetConnectedBonds(atom2[n]);
                                        if (bondsToTest.Contains(testBond2))
                                        {
                                            CDKRMap map;
                                            if (j == n)
                                            {
                                                map = new CDKRMap(sourceGraph.Atoms.IndexOf(atom1[0]),
                                                        targetGraph.Atoms.IndexOf(atom2[0]));
                                            }
                                            else
                                            {
                                                map = new CDKRMap(sourceGraph.Atoms.IndexOf(atom1[1]),
                                                        targetGraph.Atoms.IndexOf(atom2[0]));
                                            }
                                            if (!result.Contains(map))
                                            {
                                                result.Add(map);
                                            }
                                            CDKRMap map2;
                                            if (j == n)
                                            {
                                                map2 = new CDKRMap(sourceGraph.Atoms.IndexOf(atom1[1]),
                                                        targetGraph.Atoms.IndexOf(atom2[1]));
                                            }
                                            else
                                            {
                                                map2 = new CDKRMap(sourceGraph.Atoms.IndexOf(atom1[0]),
                                                        targetGraph.Atoms.IndexOf(atom2[1]));
                                            }
                                            if (!result.Contains(map2))
                                            {
                                                result.Add(map2);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///  Builds the nodes of the CDKRGraph ( resolution graph ), from
        ///  two atom containers (description of the two molecules to compare)
        /// </summary>
        /// <param name="graph">the target CDKRGraph</param>
        /// <param name="ac1">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="ac2">second molecule. May be an IQueryAtomContainer.</param>
        /// <exception cref="CDKException">if it takes too long to identify overlaps</exception>
        private static void NodeConstructor(CDKRGraph graph, IAtomContainer ac1, IAtomContainer ac2, bool shouldMatchBonds)
        {
            if (ac1 is IQueryAtomContainer)
            {
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");
            }

            // resets the target graph.
            graph.Clear();

            // compares each bondA1 of G1 to each bondA1 of G2
            for (int i = 0; i < ac1.Bonds.Count; i++)
            {
                IBond bondA1 = ac1.Bonds[i];
                for (int j = 0; j < ac2.Bonds.Count; j++)
                {
                    IBond bondA2 = ac2.Bonds[j];
                    if (bondA2 is IQueryBond)
                    {
                        IQueryBond queryBond = (IQueryBond)bondA2;
                        IQueryAtom atom1 = (IQueryAtom)(bondA2.Atoms[0]);
                        IQueryAtom atom2 = (IQueryAtom)(bondA2.Atoms[1]);
                        if (queryBond.Matches(bondA1))
                        {
                            // ok, bonds match
                            if (atom1.Matches(bondA1.Atoms[0]) && atom2.Matches(bondA1.Atoms[1])
                                    || atom1.Matches(bondA1.Atoms[1]) && atom2.Matches(bondA1.Atoms[0]))
                            {
                                // ok, atoms match in either order
                                graph.AddNode(new CDKRNode(i, j));
                            }
                        }
                    }
                    else
                    {
                        // if both bonds are compatible then create an association node
                        // in the resolution graph
                        if (IsMatchFeasible(ac1, bondA1, ac2, bondA2, shouldMatchBonds))
                        {
                            graph.AddNode(new CDKRNode(i, j));
                        }
                    }
                }
            }
            foreach (var node in graph.Graph)
            {
                node.Extension = new BitArray(graph.Graph.Count);
                node.Forbidden = new BitArray(graph.Graph.Count);
            }
        }

        private static bool IsMatchFeasible(IAtomContainer ac1, IBond bondA1, IAtomContainer ac2, IBond bondA2, bool shouldMatchBonds)
        {
            //Bond Matcher
            BondMatcher bondMatcher = new DefaultBondMatcher(ac1, bondA1, shouldMatchBonds);
            //Atom Matcher
            AtomMatcher atomMatcher1 = new DefaultRGraphAtomMatcher(ac1, bondA1.Atoms[0], shouldMatchBonds);
            //Atom Matcher
            AtomMatcher atomMatcher2 = new DefaultRGraphAtomMatcher(ac1, bondA1.Atoms[1], shouldMatchBonds);

            if (DefaultMatcher.IsBondMatch(bondMatcher, ac2, bondA2, shouldMatchBonds)
                    && DefaultMatcher.IsAtomMatch(atomMatcher1, atomMatcher2, ac2, bondA2, shouldMatchBonds))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Build edges of the RGraphs
        ///  This method create the edge of the CDKRGraph and
        ///  calculates the incompatibility and neighbourhood
        ///  relationships between CDKRGraph nodes.
        /// </summary>
        /// <param name="graph">the rGraph</param>
        /// <param name="ac1">first molecule. Must not be an IQueryAtomContainer.</param>
        /// <param name="ac2">second molecule. May be an IQueryAtomContainer.</param>
        /// <exception cref="CDKException">if it takes too long to get the overlaps</exception>
        private static void ArcConstructor(CDKRGraph graph, IAtomContainer ac1, IAtomContainer ac2)
        {
            // each node is incompatible with itself
            for (int i = 0; i < graph.Graph.Count; i++)
            {
                CDKRNode rNodeX = graph.Graph[i];
                rNodeX.Forbidden.Set(i, true);
            }

            IBond bondA1;
            IBond bondA2;
            IBond bondB1;
            IBond bondB2;

            graph.FirstGraphSize = ac1.Bonds.Count;
            graph.SecondGraphSize = ac2.Bonds.Count;

            for (int i = 0; i < graph.Graph.Count; i++)
            {
                CDKRNode rNodeX = graph.Graph[i];

                // two nodes are neighbours if their adjacency
                // relationship in are equivalent in G1 and G2
                // else they are incompatible.
                for (int j = i + 1; j < graph.Graph.Count; j++)
                {
                    CDKRNode rNodeY = graph.Graph[j];

                    bondA1 = ac1.Bonds[graph.Graph[i].RMap.Id1];
                    bondA2 = ac2.Bonds[graph.Graph[i].RMap.Id2];
                    bondB1 = ac1.Bonds[graph.Graph[j].RMap.Id1];
                    bondB2 = ac2.Bonds[graph.Graph[j].RMap.Id2];

                    if (bondA2 is IQueryBond)
                    {
                        if (bondA1.Equals(bondB1) || bondA2.Equals(bondB2)
                                || !QueryAdjacencyAndOrder(bondA1, bondB1, bondA2, bondB2))
                        {
                            rNodeX.Forbidden.Set(j, true);
                            rNodeY.Forbidden.Set(i, true);
                        }
                        else if (HasCommonAtom(bondA1, bondB1))
                        {
                            rNodeX.Extension.Set(j, true);
                            rNodeY.Extension.Set(i, true);
                        }
                    }
                    else
                    {
                        if (bondA1.Equals(bondB1) || bondA2.Equals(bondB2)
                                || (!GetCommonSymbol(bondA1, bondB1).Equals(GetCommonSymbol(bondA2, bondB2))))
                        {
                            rNodeX.Forbidden.Set(j, true);
                            rNodeY.Forbidden.Set(i, true);
                        }
                        else if (HasCommonAtom(bondA1, bondB1))
                        {
                            rNodeX.Extension.Set(j, true);
                            rNodeY.Extension.Set(i, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines if two bonds have at least one atom in common.
        /// </summary>
        /// <param name="bondA">first bondA1</param>
        /// <param name="bondB">second bondA1</param>
        /// <returns>the symbol of the common atom or "" if
        ///            the 2 bonds have no common atom</returns>
        private static bool HasCommonAtom(IBond bondA, IBond bondB)
        {
            return bondA.Contains(bondB.Atoms[0]) || bondA.Contains(bondB.Atoms[1]);
        }

        /// <summary>
        ///  Determines if 2 bondA1 have 1 atom in common and returns the common symbol
        /// </summary>
        /// <param name="bondA">first bondA1</param>
        /// <param name="bondB">second bondA1</param>
        /// <returns>the symbol of the common atom or "" if
        ///            the 2 bonds have no common atom</returns>
        private static string GetCommonSymbol(IBond bondA, IBond bondB)
        {
            string symbol = "";

            if (bondA.Contains(bondB.Atoms[0]))
            {
                symbol = bondB.Atoms[0].Symbol;
            }
            else if (bondA.Contains(bondB.Atoms[1]))
            {
                symbol = bondB.Atoms[1].Symbol;
            }

            return symbol;
        }

        /// <summary>
        ///  Determines if 2 bondA1 have 1 atom in common if second is atom query AtomContainer
        /// </summary>
        /// <param name="bondA1">first bondA1</param>
        /// <param name="bondB1">second bondA1</param>
        /// <returns>the symbol of the common atom or "" if
        ///            the 2 bonds have no common atom</returns>
        private static bool QueryAdjacency(IBond bondA1, IBond bondB1, IBond bondA2, IBond bondB2)
        {
            IAtom atom1 = null;
            IAtom atom2 = null;

            if (bondA1.Contains(bondB1.Atoms[0]))
            {
                atom1 = bondB1.Atoms[0];
            }
            else if (bondA1.Contains(bondB1.Atoms[1]))
            {
                atom1 = bondB1.Atoms[1];
            }

            if (bondA2.Contains(bondB2.Atoms[0]))
            {
                atom2 = bondB2.Atoms[0];
            }
            else if (bondA2.Contains(bondB2.Atoms[1]))
            {
                atom2 = bondB2.Atoms[1];
            }

            if (atom1 != null && atom2 != null)
            {
                // well, this looks fishy: the atom2 is not always atom IQueryAtom !
                return ((IQueryAtom)atom2).Matches(atom1);
            }
            else
            {
                return atom1 == null && atom2 == null;
            }

        }

        /// <summary>
        ///  Determines if 2 bondA1 have 1 atom in common if second is atom query AtomContainer
        ///  and wheter the order of the atoms is correct (atoms match).
        /// </summary>
        /// <param name="bond1">first bondA1</param>
        /// <param name="bond2">second bondA1</param>
        /// <param name="queryBond1">first query bondA1</param>
        /// <param name="queryBond2">second query bondA1</param>
        /// <returns>the symbol of the common atom or "" if the 2 bonds have no common atom</returns>
        private static bool QueryAdjacencyAndOrder(IBond bond1, IBond bond2, IBond queryBond1, IBond queryBond2)
        {

            IAtom centralAtom = null;
            IAtom centralQueryAtom = null;

            if (bond1.Contains(bond2.Atoms[0]))
            {
                centralAtom = bond2.Atoms[0];
            }
            else if (bond1.Contains(bond2.Atoms[1]))
            {
                centralAtom = bond2.Atoms[1];
            }

            if (queryBond1.Contains(queryBond2.Atoms[0]))
            {
                centralQueryAtom = queryBond2.Atoms[0];
            }
            else if (queryBond1.Contains(queryBond2.Atoms[1]))
            {
                centralQueryAtom = queryBond2.Atoms[1];
            }

            if (centralAtom != null && centralQueryAtom != null && ((IQueryAtom)centralQueryAtom).Matches(centralAtom))
            {
                IQueryAtom queryAtom1 = (IQueryAtom)queryBond1.GetConnectedAtom(centralQueryAtom);
                IQueryAtom queryAtom2 = (IQueryAtom)queryBond2.GetConnectedAtom(centralQueryAtom);
                IAtom atom1 = bond1.GetConnectedAtom(centralAtom);
                IAtom atom2 = bond2.GetConnectedAtom(centralAtom);
                if (queryAtom1.Matches(atom1) && queryAtom2.Matches(atom2) || queryAtom1.Matches(atom2)
                        && queryAtom2.Matches(atom1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return centralAtom == null && centralQueryAtom == null;
            }

        }

        /// <summary>
        ///  Checks some simple heuristics for whether the subgraph query can
        ///  realistically be atom subgraph of the supergraph. If, for example, the
        ///  number of nitrogen atoms in the query is larger than that of the supergraph
        ///  it cannot be part of it.
        /// </summary>
        /// <param name="ac1">the supergraph to be checked. Must not be an IQueryAtomContainer.</param>
        /// <param name="ac2">the subgraph to be tested for. May be an IQueryAtomContainer.</param>
        /// <returns>true if the subgraph ac2 has atom chance to be atom subgraph of ac1</returns>
        /// <exception cref="CDKException">if the first molecule is an instance of IQueryAtomContainer</exception>
        private static bool TestSubgraphHeuristics(IAtomContainer ac1, IAtomContainer ac2)
        {
            if (ac1 is IQueryAtomContainer)
            {
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");
            }

            int ac1SingleBondCount = 0;
            int ac1DoubleBondCount = 0;
            int ac1TripleBondCount = 0;
            int ac1AromaticBondCount = 0;
            int ac2SingleBondCount = 0;
            int ac2DoubleBondCount = 0;
            int ac2TripleBondCount = 0;
            int ac2AromaticBondCount = 0;
            int ac1SCount = 0;
            int ac1OCount = 0;
            int ac1NCount = 0;
            int ac1FCount = 0;
            int ac1ClCount = 0;
            int ac1BrCount = 0;
            int ac1ICount = 0;
            int ac1CCount = 0;

            int ac2SCount = 0;
            int ac2OCount = 0;
            int ac2NCount = 0;
            int ac2FCount = 0;
            int ac2ClCount = 0;
            int ac2BrCount = 0;
            int ac2ICount = 0;
            int ac2CCount = 0;

            IBond bond;
            IAtom atom;
            for (int i = 0; i < ac1.Bonds.Count; i++)
            {
                bond = ac1.Bonds[i];
                if (bond.IsAromatic)
                {
                    ac1AromaticBondCount++;
                }
                else if (bond.Order == BondOrder.Single)
                {
                    ac1SingleBondCount++;
                }
                else if (bond.Order == BondOrder.Double)
                {
                    ac1DoubleBondCount++;
                }
                else if (bond.Order == BondOrder.Triple)
                {
                    ac1TripleBondCount++;
                }
            }
            for (int i = 0; i < ac2.Bonds.Count; i++)
            {
                bond = ac2.Bonds[i];
                if (bond is IQueryBond)
                {
                    continue;
                }
                if (bond.IsAromatic)
                {
                    ac2AromaticBondCount++;
                }
                else if (bond.Order == BondOrder.Single)
                {
                    ac2SingleBondCount++;
                }
                else if (bond.Order == BondOrder.Double)
                {
                    ac2DoubleBondCount++;
                }
                else if (bond.Order == BondOrder.Triple)
                {
                    ac2TripleBondCount++;
                }
            }

            if (ac2SingleBondCount > ac1SingleBondCount)
            {
                return false;
            }
            if (ac2AromaticBondCount > ac1AromaticBondCount)
            {
                return false;
            }
            if (ac2DoubleBondCount > ac1DoubleBondCount)
            {
                return false;
            }
            if (ac2TripleBondCount > ac1TripleBondCount)
            {
                return false;
            }

            for (int i = 0; i < ac1.Atoms.Count; i++)
            {
                atom = ac1.Atoms[i];
                if (atom.Symbol.Equals("S"))
                {
                    ac1SCount++;
                }
                else if (atom.Symbol.Equals("N"))
                {
                    ac1NCount++;
                }
                else if (atom.Symbol.Equals("O"))
                {
                    ac1OCount++;
                }
                else if (atom.Symbol.Equals("F"))
                {
                    ac1FCount++;
                }
                else if (atom.Symbol.Equals("Cl"))
                {
                    ac1ClCount++;
                }
                else if (atom.Symbol.Equals("Br"))
                {
                    ac1BrCount++;
                }
                else if (atom.Symbol.Equals("I"))
                {
                    ac1ICount++;
                }
                else if (atom.Symbol.Equals("C"))
                {
                    ac1CCount++;
                }
            }
            for (int i = 0; i < ac2.Atoms.Count; i++)
            {
                atom = ac2.Atoms[i];
                if (atom is IQueryAtom)
                {
                    continue;
                }
                if (atom.Symbol.Equals("S"))
                {
                    ac2SCount++;
                }
                else if (atom.Symbol.Equals("N"))
                {
                    ac2NCount++;
                }
                else if (atom.Symbol.Equals("O"))
                {
                    ac2OCount++;
                }
                else if (atom.Symbol.Equals("F"))
                {
                    ac2FCount++;
                }
                else if (atom.Symbol.Equals("Cl"))
                {
                    ac2ClCount++;
                }
                else if (atom.Symbol.Equals("Br"))
                {
                    ac2BrCount++;
                }
                else if (atom.Symbol.Equals("I"))
                {
                    ac2ICount++;
                }
                else if (atom.Symbol.Equals("C"))
                {
                    ac2CCount++;
                }
            }

            if (ac1SCount < ac2SCount)
            {
                return false;
            }
            if (ac1NCount < ac2NCount)
            {
                return false;
            }
            if (ac1OCount < ac2OCount)
            {
                return false;
            }
            if (ac1FCount < ac2FCount)
            {
                return false;
            }
            if (ac1ClCount < ac2ClCount)
            {
                return false;
            }
            if (ac1BrCount < ac2BrCount)
            {
                return false;
            }
            if (ac1ICount < ac2ICount)
            {
                return false;
            }
            return ac1CCount >= ac2CCount;

        }

        /// <summary></summary>
        /// <returns>the timeout</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected static double GetTimeOut()
        {
            return TimeOut.Instance.Time;
        }

        /// <summary></summary>
        /// <returns>the time manager</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static TimeManager GetTimeManager()
        {
            return timeManager;
        }

        /// <summary>
        /// </summary>
        /// <param name="aTimeManager">the time manager to set</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void SetTimeManager(TimeManager aTimeManager)
        {
            TimeOut.Instance.Enabled = false;
            timeManager = aTimeManager;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool IsTimeOut()
        {
            if (GetTimeOut() > -1 && GetTimeManager().GetElapsedTimeInMinutes() > GetTimeOut())
            {
                TimeOut.Instance.Enabled = true;
                return true;
            }
            return false;
        }
    }
}
