/* Copyright (C) 2002-2007  Stephane Werner <mail@ixelis.net>
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
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */

using NCDK.Isomorphisms.Matchers;
using NCDK.Isomorphisms.MCSS;
using NCDK.Tools.Manipulator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Isomorphisms
{
    /// <summary>
    ///  This class implements a multipurpose structure comparison tool.
    ///  It allows to find maximal common substructure, find the
    ///  mapping of a substructure in another structure, and the mapping of
    ///  two isomorphic structures.
    /// </summary>
    /// <remarks>
    ///  Structure comparison may be associated to bond constraints
    ///  (mandatory bonds, e.g. scaffolds, reaction cores,...) on each source graph.
    ///  The constraint flexibility allows a number of interesting queries.
    ///  The substructure analysis relies on the RGraph generic class (see: RGraph)
    ///  This class implements the link between the RGraph model and the
    ///  the CDK model in this way the <see cref="RGraph"/> remains independent and may be used
    ///  in other contexts.
    ///  <para>
    ///  This algorithm derives from the algorithm described in
    ///  <token>cdk-cite-HAN90</token> and modified in the thesis of T. Hanser <token>cdk-cite-HAN93</token>.
    ///  </para>
    /// <note type="warning">
    ///    As a result of the adjacency perception used in this algorithm
    ///    there is a single limitation: cyclopropane and isobutane are seen as isomorph.
    ///    This is due to the fact that these two compounds are the only ones where
    ///    each bond is connected two each other bond (bonds are fully connected)
    ///    with the same number of bonds and still they have different structures
    ///    The algorithm could be easily enhanced with a simple atom mapping manager
    ///    to provide an atom level overlap definition that would reveal this case.
    ///    We decided not to penalize the whole procedure because of one single
    ///    exception query. Furthermore isomorphism may be discarded since  the number of atoms are
    ///    not the same (3 != 4) and in most case this will be already
    ///    screened out by a fingerprint based filtering.
    ///    It is possible to add a special treatment for this special query.
    ///    Be reminded that this algorithm matches bonds only.
    /// </note>
    /// <para>
    /// <note type="note">
    /// While most isomorphism queries involve a multi-atom query structure
    /// there may be cases in which the query atom is a single atom. In such a case
    /// a mapping of target bonds to query bonds is not feasible. In such a case, the RMap objects
    /// correspond to atom indices rather than bond indices. In general, this will not affect user
    /// code and the same sequence of method calls for matching multi-atom query structures will
    /// work for single atom query structures as well.
    /// </note>
    /// </para>
    /// </remarks>
    /// <example>
    ///  With the <see cref="IsSubgraph(IAtomContainer, IAtomContainer)"/> method,
    ///  the second, and only the second argument <b>may</b> be a <see cref="IQueryAtomContainer"/>,
    ///  which allows one to do SMARTS or MQL like queries.
    ///  The first <see cref="IAtomContainer"/> must never be an <see cref="IQueryAtomContainer"/>.
    ///  An example:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Isomorphisms.UniversalIsomorphismTester_Example.cs"]/*' />
    /// </example>
    // @author      Stephane Werner from IXELIS mail@ixelis.net
    // @cdk.created 2002-07-17
    // @cdk.require java1.4+
    // @cdk.module  standard
    // @cdk.githash
    public class UniversalIsomorphismTester
    {
        const int ID1 = 0;
        const int ID2 = 1;
        private long start;

        /// <summary>
        /// Sets the time in milliseconds until the substructure search will be breaked.
        /// </summary>
        public long Timeout { get; set; } = -1;

        public UniversalIsomorphismTester()
        {
        }

        ///////////////////////////////////////////////////////////////////////////
        //                            Query Methods
        //
        // This methods are simple applications of the RGraph model on atom containers
        // using different constrains and search options. They give an example of the
        // most common queries but of course it is possible to define other type of
        // queries exploiting the constrain and option combinations
        //

        ////
        // Isomorphism search

        /// <summary>
        /// Tests if g1 and g2 are isomorph.
        /// </summary>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>true if the 2 molecule are isomorph</returns>
        /// <exception cref="CDKException">if the first molecule is an instance of IQueryAtomContainer</exception>
        public bool IsIsomorph(IAtomContainer g1, IAtomContainer g2)
        {
            if (g1 is IQueryAtomContainer)
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");

            if (g2.Atoms.Count != g1.Atoms.Count) return false;
            // check single atom case
            if (g2.Atoms.Count == 1)
            {
                IAtom atom = g1.Atoms[0];
                IAtom atom2 = g2.Atoms[0];
                if (atom is IQueryAtom)
                {
                    IQueryAtom qAtom = (IQueryAtom)atom;
                    return qAtom.Matches(g2.Atoms[0]);
                }
                else if (atom2 is IQueryAtom)
                {
                    IQueryAtom qAtom = (IQueryAtom)atom2;
                    return qAtom.Matches(g1.Atoms[0]);
                }
                else
                {
                    string atomSymbol = atom2.Symbol;
                    return g1.Atoms[0].Symbol.Equals(atomSymbol);
                }
            }
            return (GetIsomorphMap(g1, g2) != null);
        }

        /// <summary>
        /// Returns the first isomorph mapping found or null.
        /// </summary>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>the first isomorph mapping found projected of g1. This is a List of RMap objects containing Ids of matching bonds.</returns>
        public IList<RMap> GetIsomorphMap(IAtomContainer g1, IAtomContainer g2)
        {
            if (g1 is IQueryAtomContainer)
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");

            IList<RMap> result = null;

            IList<IList<RMap>> rMapsList = Search(g1, g2, GetBitSet(g1), GetBitSet(g2), false, false);

            if (!(rMapsList.Count == 0))
                result = rMapsList[0];

            return result;
        }

        /// <summary>
        /// Returns the first isomorph 'atom mapping' found for g2 in g1.
        /// </summary>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>the first isomorph atom mapping found projected on g1. This is a List of RMap objects containing Ids of matching atoms.</returns>
        /// <exception cref="CDKException">if the first molecules is not an instance of <see cref="IQueryAtomContainer"/></exception>
        public IList<RMap> GetIsomorphAtomsMap(IAtomContainer g1, IAtomContainer g2)
        {
            if (g1 is IQueryAtomContainer)
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");

            IList<RMap> list = CheckSingleAtomCases(g1, g2);
            if (list == null)
            {
                return MakeAtomsMapOfBondsMap(GetIsomorphMap(g1, g2), g1, g2);
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
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>the list of all the 'mappings'</returns>
        public IList<IList<RMap>> GetIsomorphMaps(IAtomContainer g1, IAtomContainer g2)
        {
            return Search(g1, g2, GetBitSet(g1), GetBitSet(g2), true, true);
        }

        /////
        // Subgraph search

        /// <summary>
        /// Returns all the subgraph 'bond mappings' found for g2 in g1.
        /// This is an <see cref="IList{T}"/> of <see cref="IList{T}"/>s of <see cref="RMap"/> objects.
        /// </summary>
        /// <remarks>
        /// Note that if the query molecule is a single atom, then bond mappings
        /// cannot be defined. In such a case, the <see cref="RMap"/> object refers directly to
        /// atom - atom mappings. Thus RMap.id1 is the index of the target atom
        /// and RMap.id2 is the index of the matching query atom (in this case,
        /// it will always be 0). Note that in such a case, there is no need
        /// to call <see cref="MakeAtomsMapOfBondsMap(IList{RMap}, IAtomContainer, IAtomContainer)"/> ,
        /// though if it is called, then the
        /// return value is simply the same as the return value of this method.
        /// </remarks>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>the list of all the 'mappings' found projected of g1</returns>
        /// <seealso cref="MakeAtomsMapsOfBondsMaps(IList{IList{RMap}}, IAtomContainer, IAtomContainer)"/>
        public IList<IList<RMap>> GetSubgraphMaps(IAtomContainer g1, IAtomContainer g2)
        {
            return Search(g1, g2, new BitArray(g1.Bonds.Count), GetBitSet(g2), true, true);
        }

        /// <summary>
        /// Returns the first subgraph 'bond mapping' found for g2 in g1.
        /// </summary>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>the first subgraph bond mapping found projected on g1. This is a <see cref="IList{T}"/> of <see cref="RMap"/> objects containing Ids of matching bonds.</returns>
        public IList<RMap> GetSubgraphMap(IAtomContainer g1, IAtomContainer g2)
        {
            IList<RMap> result = null;
            IList<IList<RMap>> rMapsList = Search(g1, g2, new BitArray(g1.Bonds.Count), GetBitSet(g2), false, false);

            if (!(rMapsList.Count == 0))
            {
                result = rMapsList[0];
            }

            return result;
        }

        /// <summary>
        /// Returns all subgraph 'atom mappings' found for g2 in g1, where g2 must be a substructure
        /// of g1. If it is not a substructure, null will be returned.
        /// This is an <see cref="IList{T}"/> of <see cref="IList{T}"/>s of <see cref="RMap"/> objects.
        /// </summary>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">substructure to be mapped. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>all subgraph atom mappings found projected on g1. This is a <see cref="IList{T}"/> of <see cref="RMap"/> objects containing Ids of matching atoms.</returns>
        public IList<IList<RMap>> GetSubgraphAtomsMaps(IAtomContainer g1, IAtomContainer g2)
        {
            IList<RMap> list = CheckSingleAtomCases(g1, g2);
            if (list == null)
            {
                return MakeAtomsMapsOfBondsMaps(GetSubgraphMaps(g1, g2), g1, g2);
            }
            else
            {
                IList<IList<RMap>> atomsMap = new List<IList<RMap>>();
                atomsMap.Add(list);
                return atomsMap;
            }
        }

        /// <summary>
        /// Returns the first subgraph 'atom mapping' found for g2 in g1, where g2 must be a substructure
        /// of g1. If it is not a substructure, null will be returned.
        /// </summary>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">substructure to be mapped. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>the first subgraph atom mapping found projected on g1. This is a <see cref="IList{T}"/> of <see cref="RMap"/> objects containing Ids of matching atoms.</returns>
        public IList<RMap> GetSubgraphAtomsMap(IAtomContainer g1, IAtomContainer g2)
        {
            IList<RMap> list = CheckSingleAtomCases(g1, g2);
            if (list == null)
            {
                return MakeAtomsMapOfBondsMap(GetSubgraphMap(g1, g2), g1, g2);
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
        /// Tests if g2 a subgraph of g1.
        /// </summary>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>true if g2 a subgraph on g1</returns>
        public bool IsSubgraph(IAtomContainer g1, IAtomContainer g2)
        {
            if (g1 is IQueryAtomContainer)
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");

            if (g2.Atoms.Count > g1.Atoms.Count) return false;
            // test for single atom case
            if (g2.Atoms.Count == 1)
            {
                IAtom atom = g2.Atoms[0];
                for (int i = 0; i < g1.Atoms.Count; i++)
                {
                    IAtom atom2 = g1.Atoms[i];
                    if (atom is IQueryAtom)
                    {
                        IQueryAtom qAtom = (IQueryAtom)atom;
                        if (qAtom.Matches(atom2)) return true;
                    }
                    else if (atom2 is IQueryAtom)
                    {
                        IQueryAtom qAtom = (IQueryAtom)atom2;
                        if (qAtom.Matches(atom)) return true;
                    }
                    else
                    {
                        if (atom2.Symbol.Equals(atom.Symbol)) return true;
                    }
                }
                return false;
            }
            if (!TestSubgraphHeuristics(g1, g2)) return false;
            return (GetSubgraphMap(g1, g2) != null);
        }

        ////
        // Maximum common substructure search

        /// <summary>
        /// Returns all the maximal common substructure between two atom containers.
        /// </summary>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>the list of all the maximal common substructure found projected of g1 (list of <see cref="IAtomContainer"/>)</returns>
        public IList<IAtomContainer> GetOverlaps(IAtomContainer g1, IAtomContainer g2)
        {
            start = DateTime.Now.Ticks / 10000;
            IList<IList<RMap>> rMapsList = Search(g1, g2, new BitArray(g1.Bonds.Count), new BitArray(g2.Bonds.Count), true, false);

            // projection on G1
            IList<IAtomContainer> graphList = ProjectList(rMapsList, g1, ID1);

            // reduction of set of solution (isomorphism and substructure
            // with different 'mappings'

            return GetMaximum(graphList);
        }

        /// <summary>
        /// Transforms an AtomContainer into a <see cref="BitArray"/> (which's size = number of bond
        /// in the atomContainer, all the bit are set to true).
        /// </summary>
        /// <param name="ac"><see cref="IAtomContainer"/> to transform</param>
        /// <returns>The bitSet</returns>
        public static BitArray GetBitSet(IAtomContainer ac)
        {
            BitArray bs;
            int n = ac.Bonds.Count;

            if (n != 0)
            {
                bs = new BitArray(n).Not(); // set all bit true
            }
            else
            {
                bs = new BitArray(0);
            }

            return bs;
        }

        //////////////////////////////////////////////////
        //          Internal methods

        /// <summary>
        /// Builds the <see cref="RGraph"/> ( resolution graph ), from two atomContainer
        /// (description of the two molecules to compare)
        /// This is the interface point between the CDK model and
        /// the generic MCSS algorithm based on the RGRaph.
        /// </summary>
        /// <param name="g1">Description of the first molecule</param>
        /// <param name="g2">Description of the second molecule</param>
        /// <returns>the rGraph</returns>
        public static RGraph BuildRGraph(IAtomContainer g1, IAtomContainer g2)
        {
            RGraph rGraph = new RGraph();
            NodeConstructor(rGraph, g1, g2);
            ArcConstructor(rGraph, g1, g2);
            return rGraph;
        }

        /// <summary>
        /// General <see cref="RGraph"/> parsing method (usually not used directly)
        /// This method is the entry point for the recursive search
        /// adapted to the atom container input.
        /// </summary>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="c1">initial condition ( bonds from g1 that must be contains in the solution )</param>
        /// <param name="c2">initial condition ( bonds from g2 that must be contains in the solution )</param>
        /// <param name="findAllStructure">if false stop at the first structure found</param>
        /// <param name="findAllMap">if true search all the 'mappings' for one same structure</param>
        /// <returns>a List of Lists of <see cref="RMap"/> objects that represent the search solutions</returns>
        public IList<IList<RMap>> Search(IAtomContainer g1, IAtomContainer g2, BitArray c1, BitArray c2, bool findAllStructure, bool findAllMap)
        {
            // remember start time
            start = DateTime.Now.Ticks /10000;

            // handle single query atom case separately
            if (g2.Atoms.Count == 1)
            {
                List<IList<RMap>> matches = new List<IList<RMap>>();
                IAtom queryAtom = g2.Atoms[0];

                // we can have a IQueryAtomContainer *or* an IAtomContainer
                if (queryAtom is IQueryAtom)
                {
                    IQueryAtom qAtom = (IQueryAtom)queryAtom;
                    foreach (var atom in g1.Atoms)
                    {
                        if (qAtom.Matches(atom))
                        {
                            List<RMap> lmap = new List<RMap>();
                            lmap.Add(new RMap(g1.Atoms.IndexOf(atom), 0));
                            matches.Add(lmap);
                        }
                    }
                }
                else
                {
                    foreach (var atom in g1.Atoms)
                    {
                        if (queryAtom.Symbol.Equals(atom.Symbol))
                        {
                            List<RMap> lmap = new List<RMap>();
                            lmap.Add(new RMap(g1.Atoms.IndexOf(atom), 0));
                            matches.Add(lmap);
                        }
                    }
                }
                return matches;
            }

            // reset result
            List<IList<RMap>> rMapsList = new List<IList<RMap>>();

            // build the RGraph corresponding to this problem
            RGraph rGraph = BuildRGraph(g1, g2);
            // Set time data
            rGraph.Timeout = Timeout;
            rGraph.Start = start;
            // parse the RGraph with the given constrains and options
            rGraph.Parse(c1, c2, findAllStructure, findAllMap);
            IList<BitArray> solutionList = rGraph.Solutions;

            // conversions of RGraph's internal solutions to G1/G2 mappings
            foreach (var set in solutionList)
            {
                IList<RMap> rmap = rGraph.BitSetToRMap(set);
                if (CheckQueryAtoms(rmap, g1, g2)) rMapsList.Add(rmap);
            }

            return rMapsList;
        }

        /// <summary>
        /// Checks that <see cref="IQueryAtom"/>'s correctly match consistently.
        /// </summary>
        /// <param name="bondmap">bond mapping</param>
        /// <param name="g1">target graph</param>
        /// <param name="g2">query graph</param>
        /// <returns>the atom matches are consistent</returns>
        private bool CheckQueryAtoms(IList<RMap> bondmap, IAtomContainer g1, IAtomContainer g2)
        {
            if (!(g2 is IQueryAtomContainer)) return true;
            IList<RMap> atommap = MakeAtomsMapOfBondsMap(bondmap, g1, g2);
            foreach (var rmap in atommap)
            {
                IAtom a1 = g1.Atoms[rmap.Id1];
                IAtom a2 = g2.Atoms[rmap.Id2];
                if (a2 is IQueryAtom)
                {
                    if (!((IQueryAtom)a2).Matches(a1)) return false;
                }
            }
            return true;
        }

        //////////////////////////////////////
        //    Manipulation tools

        /// <summary>
        /// Projects a list of <see cref="RMap"/> on a molecule.
        /// </summary>
        /// <param name="rMapList">the list to project</param>
        /// <param name="g">the molecule on which project</param>
        /// <param name="id">the id in the <see cref="RMap"/> of the molecule g</param>
        /// <returns>an AtomContainer</returns>
        public static IAtomContainer Project(IList<RMap> rMapList, IAtomContainer g, int id)
        {
            IAtomContainer ac = g.Builder.CreateAtomContainer();

            IDictionary<IAtom, IAtom> table = new Dictionary<IAtom, IAtom>();
            IAtom a1;
            IAtom a2;
            IAtom a;
            IBond bond;

            foreach (var rMap in rMapList)
            {
                if (id == UniversalIsomorphismTester.ID1)
                {
                    bond = g.Bonds[rMap.Id1];
                }
                else
                {
                    bond = g.Bonds[rMap.Id2];
                }

                a = bond.Atoms[0];
                if (!table.TryGetValue(a, out a1))
                {
                    a1 = (IAtom)a.Clone();
                    ac.Atoms.Add(a1);
                    table.Add(a, a1);
                }

                a = bond.Atoms[1];
                if (!table.TryGetValue(a, out a2))
                {
                    a2 = (IAtom)a.Clone();
                    ac.Atoms.Add(a2);
                    table.Add(a, a2);
                }
                IBond newBond = g.Builder.CreateBond(a1, a2, bond.Order);
                newBond.IsAromatic = bond.IsAromatic;
                ac.Bonds.Add(newBond);
            }
            return ac;
        }

        /// <summary>
        /// Projects a list of RMapsList on a molecule.
        /// </summary>
        /// <param name="rMapsList">list of RMapsList to project</param>
        /// <param name="g">the molecule on which project</param>
        /// <param name="id">the id in the RMap of the molecule g</param>
        /// <returns>a list of AtomContainer</returns>
        public static IList<IAtomContainer> ProjectList(IList<IList<RMap>> rMapsList, IAtomContainer g, int id)
        {
            List<IAtomContainer> graphList = new List<IAtomContainer>();

            foreach (var rMapList in rMapsList)
            {
                IAtomContainer ac = Project(rMapList, g, id);
                graphList.Add(ac);
            }
            return graphList;
        }

        /// <summary>
        /// Removes all redundant solution.
        /// </summary>
        /// <param name="graphList">the list of structure to clean</param>
        /// <returns>the list cleaned</returns>
        /// <exception cref="CDKException">if there is a problem in obtaining subgraphs</exception>
        private IList<IAtomContainer> GetMaximum(IList<IAtomContainer> graphList)
        {
            List<IAtomContainer> reducedGraphList = new List<IAtomContainer>();
            reducedGraphList.AddRange(graphList);

            for (int i = 0; i < graphList.Count; i++)
            {
                IAtomContainer gi = graphList[i];

                for (int j = i + 1; j < graphList.Count; j++)
                {
                    IAtomContainer gj = graphList[j];

                    // Gi included in Gj or Gj included in Gi then
                    // reduce the irrelevant solution
                    if (IsSubgraph(gj, gi))
                    {
                        reducedGraphList.Remove(gi);
                    }
                    else if (IsSubgraph(gi, gj))
                    {
                        reducedGraphList.Remove(gj);
                    }
                }
            }
            return reducedGraphList;
        }

        /// <summary>
        ///  Checks for single atom cases before doing subgraph/isomorphism search.
        /// </summary>
        /// <param name="g1">AtomContainer to match on. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">AtomContainer as query. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns><see cref="IList{T}"/> of <see cref="IList{T}"/> of <see cref="RMap"/> objects for the Atoms (not Bonds!), null if no single atom case</returns>
        /// <exception cref="CDKException">if the first molecule is an instance of IQueryAtomContainer</exception>
        public static IList<RMap> CheckSingleAtomCases(IAtomContainer g1, IAtomContainer g2)
        {
            if (g1 is IQueryAtomContainer)
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");

            if (g2.Atoms.Count == 1)
            {
                List<RMap> arrayList = new List<RMap>();
                IAtom atom = g2.Atoms[0];
                if (atom is IQueryAtom)
                {
                    IQueryAtom qAtom = (IQueryAtom)atom;
                    for (int i = 0; i < g1.Atoms.Count; i++)
                    {
                        if (qAtom.Matches(g1.Atoms[i])) arrayList.Add(new RMap(i, 0));
                    }
                }
                else
                {
                    string atomSymbol = atom.Symbol;
                    for (int i = 0; i < g1.Atoms.Count; i++)
                    {
                        if (g1.Atoms[i].Symbol.Equals(atomSymbol)) arrayList.Add(new RMap(i, 0));
                    }
                }
                return arrayList;
            }
            else if (g1.Atoms.Count == 1)
            {
                List<RMap> arrayList = new List<RMap>();
                IAtom atom = g1.Atoms[0];
                for (int i = 0; i < g2.Atoms.Count; i++)
                {
                    IAtom atom2 = g2.Atoms[i];
                    if (atom2 is IQueryAtom)
                    {
                        IQueryAtom qAtom = (IQueryAtom)atom2;
                        if (qAtom.Matches(atom)) arrayList.Add(new RMap(0, i));
                    }
                    else
                    {
                        if (atom2.Symbol.Equals(atom.Symbol)) arrayList.Add(new RMap(0, i));
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
        ///  This makes maps of matching atoms out of a maps of matching bonds as produced by the
        ///  Get(Subgraph|Ismorphism)Maps methods.
        /// </summary>
        /// <param name="l">The list produced by the getMap method.</param>
        /// <param name="g1">The first atom container. Must not be a <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">The second one (first and second as in getMap). May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>A List of <see cref="IList{T}"/>s of <see cref="RMap"/> objects of matching Atoms.</returns>
        public static IList<IList<RMap>> MakeAtomsMapsOfBondsMaps(IList<IList<RMap>> l, IAtomContainer g1, IAtomContainer g2)
        {
            if (l == null)
            {
                return l;
            }
            if (g2.Atoms.Count == 1) return l; // since the RMap is already an atom-atom mapping
            IList<IList<RMap>> result = new List<IList<RMap>>();
            foreach (var l2 in l)
            {
                result.Add(MakeAtomsMapOfBondsMap(l2, g1, g2));
            }
            return result;
        }

        /// <summary>
        /// This makes a map of matching atoms out of a map of matching bonds as produced by the
        /// <see cref="GetSubgraphMap(IAtomContainer, IAtomContainer)"/>/<see cref="GetIsomorphMap(IAtomContainer, IAtomContainer)"/> methods.
        /// </summary>
        /// <param name="l">The list produced by the getMap method.</param>
        /// <param name="g1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="g2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>The mapping found projected on g1. This is a <see cref="List{T}"/> of <see cref="RMap"/> objects containing Ids of matching atoms.</returns>
        public static IList<RMap> MakeAtomsMapOfBondsMap(IList<RMap> l, IAtomContainer g1, IAtomContainer g2)
        {
            if (l == null) return (l);
            List<RMap> result = new List<RMap>();
            for (int i = 0; i < l.Count; i++)
            {
                IBond bond1 = g1.Bonds[l[i].Id1];
                IBond bond2 = g2.Bonds[l[i].Id2];
                IAtom[] atom1 = BondManipulator.GetAtomArray(bond1);
                IAtom[] atom2 = BondManipulator.GetAtomArray(bond2);
                for (int j = 0; j < 2; j++)
                {
                    var bondsConnectedToAtom1j = g1.GetConnectedBonds(atom1[j]);
                    foreach (var kbond in bondsConnectedToAtom1j)
                    {
                        if (kbond != bond1)
                        {
                            IBond testBond = kbond;
                            for (int m = 0; m < l.Count; m++)
                            {
                                IBond testBond2;
                                if ((l[m]).Id1 == g1.Bonds.IndexOf(testBond))
                                {
                                    testBond2 = g2.Bonds[(l[m]).Id2];
                                    for (int n = 0; n < 2; n++)
                                    {
                                        var bondsToTest = g2.GetConnectedBonds(atom2[n]);
                                        if (bondsToTest.Contains(testBond2))
                                        {
                                            RMap map;
                                            if (j == n)
                                            {
                                                map = new RMap(g1.Atoms.IndexOf(atom1[0]), g2.Atoms.IndexOf(atom2[0]));
                                            }
                                            else
                                            {
                                                map = new RMap(g1.Atoms.IndexOf(atom1[1]), g2.Atoms.IndexOf(atom2[0]));
                                            }
                                            if (!result.Contains(map))
                                            {
                                                result.Add(map);
                                            }
                                            RMap map2;
                                            if (j == n)
                                            {
                                                map2 = new RMap(g1.Atoms.IndexOf(atom1[1]), g2.Atoms.IndexOf(atom2[1]));
                                            }
                                            else
                                            {
                                                map2 = new RMap(g1.Atoms.IndexOf(atom1[0]), g2.Atoms.IndexOf(atom2[1]));
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
        ///  Builds  the nodes of the <see cref="RGraph"/> ( resolution graph ), from
        ///  two atom containers (description of the two molecules to compare)
        /// </summary>
        /// <param name="gr">the target RGraph</param>
        /// <param name="ac1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="ac2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <exception cref="CDKException">if it takes too long to identify overlaps</exception>
        private static void NodeConstructor(RGraph gr, IAtomContainer ac1, IAtomContainer ac2)
        {
            if (ac1 is IQueryAtomContainer)
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");

            // resets the target graph.
            gr.Clear();

            // compares each bond of G1 to each bond of G2
            for (int i = 0; i < ac1.Bonds.Count; i++)
            {
                for (int j = 0; j < ac2.Bonds.Count; j++)
                {
                    IBond bondA2 = ac2.Bonds[j];
                    if (bondA2 is IQueryBond)
                    {
                        IQueryBond queryBond = (IQueryBond)bondA2;
                        IQueryAtom atom1 = (IQueryAtom)(bondA2.Atoms[0]);
                        IQueryAtom atom2 = (IQueryAtom)(bondA2.Atoms[1]);
                        IBond bond = ac1.Bonds[i];
                        if (queryBond.Matches(bond))
                        {
                            var bondAtom0 = bond.Atoms[0];
                            var bondAtom1 = bond.Atoms[1];
                            // ok, bonds match
                            if (atom1.Matches(bondAtom0) && atom2.Matches(bondAtom1)
                                    || atom1.Matches(bondAtom1) && atom2.Matches(bondAtom0))
                            {
                                // ok, atoms match in either order
                                gr.AddNode(new RNode(i, j));
                            }
                        }
                    }
                    else
                    {
                        // if both bonds are compatible then create an association node
                        // in the resolution graph
                        var ac1Bondi = ac1.Bonds[i];
                        var ac2Bondj = ac2.Bonds[j];
                        // bond type conditions
                        if (( // same bond order and same aromaticity flag (either both on or off)
                                ac1Bondi.Order == ac2Bondj.Order && ac1Bondi.IsAromatic == ac2Bondj.IsAromatic) || ( // both bond are aromatic
                                ac1Bondi.IsAromatic && ac2Bondj.IsAromatic))
                        {
                            var ac1Bondi0 = ac1Bondi.Atoms[0];
                            var ac1Bondi1 = ac1Bondi.Atoms[1];
                            var ac2Bondj0 = ac2Bondj.Atoms[0];
                            var ac2Bondj1 = ac2Bondj.Atoms[1];
                            // atom type conditions
                            if (
                                // a1 = a2 && b1 = b2
                                (ac1Bondi0.Symbol.Equals(ac2Bondj0.Symbol) && ac1Bondi1.Symbol.Equals(ac2Bondj1.Symbol)) ||
                                // a1 = b2 && b1 = a2
                                (ac1Bondi0.Symbol.Equals(ac2Bondj1.Symbol) && ac1Bondi1.Symbol.Equals(ac2Bondj0.Symbol)))
                            {
                                gr.AddNode(new RNode(i, j));
                            }
                        }
                    }
                }
            }
            foreach (var node in gr.Graph)
                node.EnsureNodeCount(gr.Graph.Count);
        }

        /// <summary>
        ///  Build edges of the <see cref="RGraph"/>s.
        ///  This method create the edge of the RGraph and
        ///  calculates the incompatibility and neighborhood
        ///  relationships between RGraph nodes.
        /// </summary>
        /// <param name="gr">the rGraph</param>
        /// <param name="ac1">first molecule. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="ac2">second molecule. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <exception cref="CDKException">if it takes too long to identify overlaps</exception>
        private static void ArcConstructor(RGraph gr, IAtomContainer ac1, IAtomContainer ac2)
        {
            // each node is incompatible with himself
            for (int i = 0; i < gr.Graph.Count; i++)
            {
                RNode x = (RNode)gr.Graph[i];
                x.Forbidden.Set(i, true);
            }

            IBond a1;
            IBond a2;
            IBond b1;
            IBond b2;

            gr.FirstGraphSize = ac1.Bonds.Count;
            gr.SecondGraphSize = ac2.Bonds.Count;

            for (int i = 0; i < gr.Graph.Count; i++)
            {
                RNode x = gr.Graph[i];

                // two nodes are neighbors if their adjacency
                // relationship in are equivalent in G1 and G2
                // else they are incompatible.
                for (int j = i + 1; j < gr.Graph.Count; j++)
                {
                    RNode y = gr.Graph[j];

                    a1 = ac1.Bonds[x.RMap.Id1];
                    a2 = ac2.Bonds[x.RMap.Id2];

                    b1 = ac1.Bonds[y.RMap.Id1];
                    b2 = ac2.Bonds[y.RMap.Id2];

                    if (a2 is IQueryBond)
                    {
                        if (a1.Equals(b1) || a2.Equals(b2) || !QueryAdjacencyAndOrder(a1, b1, a2, b2))
                        {
                            x.Forbidden.Set(j, true);
                            y.Forbidden.Set(i, true);
                        }
                        else if (HasCommonAtom(a1, b1))
                        {
                            x.Extension.Set(j, true);
                            y.Extension.Set(i, true);
                        }
                    }
                    else
                    {
                        if (a1.Equals(b1) || a2.Equals(b2) || (!GetCommonSymbol(a1, b1).Equals(GetCommonSymbol(a2, b2))))
                        {
                            x.Forbidden.Set(j, true);
                            y.Forbidden.Set(i, true);
                        }
                        else if (HasCommonAtom(a1, b1))
                        {
                            x.Extension.Set(j, true);
                            y.Extension.Set(i, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines if two bonds have at least one atom in common.
        /// </summary>
        /// <param name="a">first bond</param>
        /// <param name="b">second bond</param>
        /// <returns> the symbol of the common atom or "" if the 2 bonds have no common atom</returns>
        private static bool HasCommonAtom(IBond a, IBond b)
        {
            return a.Contains(b.Atoms[0]) || a.Contains(b.Atoms[1]);
        }

        /// <summary>
        /// Determines if 2 bond have 1 atom in common and returns the common symbol.
        /// </summary>
        /// <param name="a">first bond</param>
        /// <param name="b">second bond</param>
        /// <returns>the symbol of the common atom or "" if the 2 bonds have no common atom</returns>
        private static string GetCommonSymbol(IBond a, IBond b)
        {
            string symbol = "";

            if (a.Contains(b.Atoms[0]))
            {
                symbol = b.Atoms[0].Symbol;
            }
            else if (a.Contains(b.Atoms[1]))
            {
                symbol = b.Atoms[1].Symbol;
            }

            return symbol;
        }

        /// <summary>
        /// Determines if 2 bond have 1 atom in common if second is a query AtomContainer.
        /// </summary>
        /// <param name="a1">first bond</param>
        /// <param name="b1">second bond</param>
        /// <param name="a2">first bond</param>
        /// <param name="b2">second bond</param>
        /// <returns>the symbol of the common atom or "" if the 2 bonds have no common atom</returns>
        private static bool QueryAdjacency(IBond a1, IBond b1, IBond a2, IBond b2)
        {
            IAtom atom1 = null;
            IAtom atom2 = null;

            if (a1.Contains(b1.Atoms[0]))
            {
                atom1 = b1.Atoms[0];
            }
            else if (a1.Contains(b1.Atoms[1]))
            {
                atom1 = b1.Atoms[1];
            }

            if (a2.Contains(b2.Atoms[0]))
            {
                atom2 = b2.Atoms[0];
            }
            else if (a2.Contains(b2.Atoms[1]))
            {
                atom2 = b2.Atoms[1];
            }

            if (atom1 != null && atom2 != null)
            {
                // well, this looks fishy: the atom2 is not always a IQueryAtom !
                return ((IQueryAtom)atom2).Matches(atom1);
            }
            else
                return atom1 == null && atom2 == null;
        }

        /// <summary>
        ///  Determines if 2 bond have 1 atom in common if second is a query AtomContainer
        ///  and whether the order of the atoms is correct (atoms match).
        /// </summary>
        /// <param name="bond1">first bond</param>
        /// <param name="bond2">second bond</param>
        /// <param name="queryBond1">first query bond</param>
        /// <param name="queryBond2">second query bond</param>
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
                    return false;
            }
            else
                return centralAtom == null && centralQueryAtom == null;

        }

        /// <summary>
        ///  Checks some simple heuristics for whether the subgraph query can
        ///  realistically be a subgraph of the supergraph. If, for example, the
        ///  number of nitrogen atoms in the query is larger than that of the supergraph
        ///  it cannot be part of it.
        /// </summary>
        /// <param name="ac1">the supergraph to be checked. Must not be an <see cref="IQueryAtomContainer"/>.</param>
        /// <param name="ac2">the subgraph to be tested for. May be an <see cref="IQueryAtomContainer"/>.</param>
        /// <returns>true if the subgraph ac2 has a chance to be a subgraph of ac1</returns>
        /// <exception cref="CDKException">if the first molecule is an instance of <see cref="IQueryAtomContainer"/></exception>
        private static bool TestSubgraphHeuristics(IAtomContainer ac1, IAtomContainer ac2)
        {
            if (ac1 is IQueryAtomContainer)
                throw new CDKException("The first IAtomContainer must not be an IQueryAtomContainer");

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
                    ac1AromaticBondCount++;
                else if (bond.Order == BondOrder.Single)
                    ac1SingleBondCount++;
                else if (bond.Order == BondOrder.Double)
                    ac1DoubleBondCount++;
                else if (bond.Order == BondOrder.Triple) ac1TripleBondCount++;
            }
            for (int i = 0; i < ac2.Bonds.Count; i++)
            {
                bond = ac2.Bonds[i];
                if (bond is IQueryBond) continue;
                if (bond.IsAromatic)
                    ac2AromaticBondCount++;
                else if (bond.Order == BondOrder.Single)
                    ac2SingleBondCount++;
                else if (bond.Order == BondOrder.Double)
                    ac2DoubleBondCount++;
                else if (bond.Order == BondOrder.Triple) ac2TripleBondCount++;
            }

            if (ac2SingleBondCount > ac1SingleBondCount) return false;
            if (ac2AromaticBondCount > ac1AromaticBondCount) return false;
            if (ac2DoubleBondCount > ac1DoubleBondCount) return false;
            if (ac2TripleBondCount > ac1TripleBondCount) return false;

            for (int i = 0; i < ac1.Atoms.Count; i++)
            {
                atom = ac1.Atoms[i];
                if (atom.Symbol.Equals("S"))
                    ac1SCount++;
                else if (atom.Symbol.Equals("N"))
                    ac1NCount++;
                else if (atom.Symbol.Equals("O"))
                    ac1OCount++;
                else if (atom.Symbol.Equals("F"))
                    ac1FCount++;
                else if (atom.Symbol.Equals("Cl"))
                    ac1ClCount++;
                else if (atom.Symbol.Equals("Br"))
                    ac1BrCount++;
                else if (atom.Symbol.Equals("I"))
                    ac1ICount++;
                else if (atom.Symbol.Equals("C")) ac1CCount++;
            }
            for (int i = 0; i < ac2.Atoms.Count; i++)
            {
                atom = ac2.Atoms[i];
                if (atom is IQueryAtom) continue;
                if (atom.Symbol.Equals("S"))
                    ac2SCount++;
                else if (atom.Symbol.Equals("N"))
                    ac2NCount++;
                else if (atom.Symbol.Equals("O"))
                    ac2OCount++;
                else if (atom.Symbol.Equals("F"))
                    ac2FCount++;
                else if (atom.Symbol.Equals("Cl"))
                    ac2ClCount++;
                else if (atom.Symbol.Equals("Br"))
                    ac2BrCount++;
                else if (atom.Symbol.Equals("I"))
                    ac2ICount++;
                else if (atom.Symbol.Equals("C")) ac2CCount++;
            }

            if (ac1SCount < ac2SCount) return false;
            if (ac1NCount < ac2NCount) return false;
            if (ac1OCount < ac2OCount) return false;
            if (ac1FCount < ac2FCount) return false;
            if (ac1ClCount < ac2ClCount) return false;
            if (ac1BrCount < ac2BrCount) return false;
            if (ac1ICount < ac2ICount) return false;
            return ac1CCount >= ac2CCount;
        }
    }
}
