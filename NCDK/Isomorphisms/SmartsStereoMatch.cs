/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Collections;
using NCDK.Isomorphisms.Matchers;
using NCDK.Isomorphisms.Matchers.SMARTS;
using System;
using System.Collections.Generic;

namespace NCDK.Isomorphisms
{
    /// <summary>
    /// Filters SMARTS matches for those that have valid stereochemistry
    /// configuration.
    /// </summary>
    /// <remarks>
    /// <note type="note">
    /// This class is internal and will be private in future.
    /// </note>
    /// </remarks>
    // @author John May
    // @cdk.module smarts
    [Obsolete]
    public sealed class SmartsStereoMatch
    {
        /// <summary>Query and target contains.</summary>
        private readonly IAtomContainer query, target;

        /// <summary>Atom to atom index lookup.</summary>
        private readonly Dictionary<IAtom, int> queryMap, targetMap;

        /// <summary>Indexed array of stereo elements.</summary>
        private readonly IStereoElement<IChemObject, IChemObject>[] queryElements, targetElements;

        /// <summary>Indexed array of stereo element types.</summary>
        private readonly StereoType[] queryTypes, targetTypes;

        /// <summary>Indices of focus atoms of stereo elements.</summary>
        private readonly int[] queryStereoIndices, targetStereoIndices;

        /// <summary>
        /// Create a predicate for checking mappings between a provided
        /// <paramref name="query"/> and <paramref name="target"/>.
        /// </summary>
        /// <param name="query">query container</param>
        /// <param name="target">target container</param>
        public SmartsStereoMatch(IAtomContainer query, IAtomContainer target)
        {
            if (!(query is IQueryAtomContainer))
                throw new ArgumentException("match predicate is for SMARTS only");

            this.query = query;
            this.target = target;

            this.queryMap = IndexAtoms(query);
            this.targetMap = IndexAtoms(target);
            this.queryElements = new IStereoElement<IChemObject, IChemObject>[query.Atoms.Count];
            this.targetElements = new IStereoElement<IChemObject, IChemObject>[target.Atoms.Count];
            this.queryTypes = new StereoType[query.Atoms.Count];
            this.targetTypes = new StereoType[target.Atoms.Count];

            queryStereoIndices = IndexElements(queryMap, queryElements, queryTypes, query);
            targetStereoIndices = IndexElements(targetMap, targetElements, targetTypes, target);
        }

        /// <summary>
        /// Is the <paramref name="mapping"/> of the stereochemistry in the query preserved in
        /// the target.
        /// </summary>
        /// <param name="mapping">permutation of the query vertices</param>
        /// <returns>the stereo chemistry is value</returns>
        public bool Apply(int[] mapping)
        {
            foreach (var u in queryStereoIndices)
            {
                switch (queryTypes[u])
                {
                    case StereoType.Tetrahedral:
                        if (!CheckTetrahedral(u, mapping)) return false;
                        break;
                    case StereoType.Geometric:
                        if (!CheckGeometric(u, OtherIndex(u), mapping)) return false;
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Verify the tetrahedral stereochemistry (clockwise/anticlockwise) of atom
        /// <paramref name="u"/> is preserved in the target when the <paramref name="mapping"/> is used.
        /// </summary>
        /// <param name="u">tetrahedral index in the target</param>
        /// <param name="mapping">mapping of vertices</param>
        /// <returns>the tetrahedral configuration is preserved</returns>
        private bool CheckTetrahedral(int u, int[] mapping)
        {
            int v = mapping[u];

            if (targetTypes[v] != StereoType.Unset && targetTypes[v] != StereoType.Tetrahedral)
                return false;

            ITetrahedralChirality queryElement = (ITetrahedralChirality)queryElements[u];
            ITetrahedralChirality targetElement = (ITetrahedralChirality)targetElements[v];

            SMARTSAtom queryAtom = (SMARTSAtom)query.Atoms[u];
            IAtom targetAtom = target.Atoms[v];

            int[] us = Neighbors(queryElement, queryMap);
            us = Map(u, v, us, mapping);
            int p = PermutationParity(us);

            // check if unspecified was allowed
            if (targetTypes[v] == StereoType.Unset)
            {
                if (queryAtom is SMARTSAtom)
                    return ((SMARTSAtom)queryAtom).ChiralityMatches(targetAtom, 0, p);
                else
                    return ((QueryAtom)queryAtom).Expression.Matches(targetAtom, 0);
            }

            // target was non-tetrahedral
            if (targetTypes[v] != StereoType.Tetrahedral) return false;

            int[] vs = Neighbors(targetElement, targetMap);
            int q = PermutationParity(vs) * Parity(targetElement.Stereo);

            if (queryAtom is SMARTSAtom)
                return ((SMARTSAtom)queryAtom).ChiralityMatches(targetAtom, q, p);
            else
            {
                q *= p;
                if (q < 0)
                    return ((QueryAtom)queryAtom).Expression.Matches(targetAtom, (int)StereoConfigurations.Left);
                else if (q > 0)
                    return ((QueryAtom)queryAtom).Expression.Matches(targetAtom, (int)StereoConfigurations.Right);
                else
                    return ((QueryAtom)queryAtom).Expression.Matches(targetAtom, 0);
            }
        }

        /// <summary>
        /// Transforms the neighbors <paramref name="us"/> adjacent to <paramref name="u"/> into the target
        /// indices using the mapping <paramref name="mapping"/>. The transformation accounts
        /// for an implicit hydrogen in the query being an explicit hydrogen in the
        /// target.
        /// </summary>
        /// <param name="u">central atom of tetrahedral element</param>
        /// <param name="v">mapped central atom of the tetrahedral element</param>
        /// <param name="us">neighboring vertices of <paramref name="u"/> (<paramref name="u"/> plural)</param>
        /// <param name="mapping">mapping from the query to the target</param>
        /// <returns>the neighbors us, transformed into the neighbors around v</returns>
        private static int[] Map(int u, int v, int[] us, int[] mapping)
        {
            for (int i = 0; i < us.Length; i++)
                us[i] = mapping[us[i]];
            return us;
        }

        /// <summary>
        /// Verify the geometric stereochemistry (cis/trans) of the double bond
        /// <c><paramref name="u1"/>=<paramref name="u2"/></c> is preserved in the target when the <paramref name="mapping"/> is
        /// used.
        /// </summary>
        /// <param name="u1">one index of the double bond</param>
        /// <param name="u2">other index of the double bond</param>
        /// <param name="mapping">mapping of vertices</param>
        /// <returns>the geometric configuration is preserved</returns>
        private bool CheckGeometric(int u1, int u2, int[] mapping)
        {
            int v1 = mapping[u1];
            int v2 = mapping[u2];

            if (targetTypes[v1] != StereoType.Unset && targetTypes[v1] != StereoType.Geometric)
                return false;
            if (targetTypes[v2] != StereoType.Unset && targetTypes[v2] != StereoType.Geometric)
                return false;

            IDoubleBondStereochemistry queryElement = (IDoubleBondStereochemistry)queryElements[u1];
            var queryBonds = queryElement.Bonds;

            bool unspecified = ((StereoBond)queryBonds[0]).IsUnspecified || ((StereoBond)queryBonds[1]).IsUnspecified;

            if (unspecified && (targetTypes[v1] == StereoType.Unset || targetTypes[v2] == StereoType.Unset)) return true;

            // no configuration in target
            if (targetTypes[v1] != StereoType.Geometric || targetTypes[v2] != StereoType.Geometric) return false;

            IDoubleBondStereochemistry targetElement = (IDoubleBondStereochemistry)targetElements[v1];

            // although the atoms were mapped and 'v1' and 'v2' are bond in double-bond
            // elements they are not in the same element
            if (!targetElement.StereoBond.Contains(target.Atoms[v1])
                    || !targetElement.StereoBond.Contains(target.Atoms[v2])) return false;

            // bond is undirected so we need to ensure v1 is the first atom in the bond
            // we also need to to swap the substituents later
            bool swap = false;
            if (targetElement.StereoBond.Begin != target.Atoms[v1])
            {
                int tmp = v1;
                v1 = v2;
                v2 = tmp;
                swap = true;
            }

            var targetBonds = targetElement.Bonds;

            int p = Parity(queryElement.Stereo);
            int q = Parity(targetElement.Stereo);

            int uLeft = queryMap[queryBonds[0].GetOther(query.Atoms[u1])];
            int uRight = queryMap[queryBonds[1].GetOther(query.Atoms[u2])];

            int vLeft = targetMap[targetBonds[0].GetOther(target.Atoms[v1])];
            int vRight = targetMap[targetBonds[1].GetOther(target.Atoms[v2])];

            if (swap)
            {
                int tmp = vLeft;
                vLeft = vRight;
                vRight = tmp;
            }
            if (mapping[uLeft] != vLeft) p *= -1;
            if (mapping[uRight] != vRight) p *= -1;

            return p == q;
        }

        /// <summary>
        /// Access the neighbors of <paramref name="element"/> as their indices.
        /// </summary>
        /// <param name="element">tetrahedral element</param>
        /// <param name="map">atom index lookup</param>
        /// <returns>the neighbors</returns>
        private static int[] Neighbors(ITetrahedralChirality element, Dictionary<IAtom, int> map)
        {
            var atoms = element.Ligands;
            int[] vs = new int[atoms.Count];
            for (int i = 0; i < atoms.Count; i++)
                vs[i] = map[atoms[i]];
            return vs;
        }

        /// <summary>
        /// Compute the permutation parity of the values <paramref name="vs"/>. The parity is
        /// whether we need to do an odd or even number of swaps to put the values in
        /// sorted order.
        /// </summary>
        /// <param name="vs">values</param>
        /// <returns>parity of the permutation (odd = -1, even = +1)</returns>
        private static int PermutationParity(int[] vs)
        {
            int n = 0;
            for (int i = 0; i < vs.Length; i++)
                for (int j = i + 1; j < vs.Length; j++)
                    if (vs[i] > vs[j]) n++;
            return (n & 0x1) == 1 ? -1 : 1;
        }

        /// <summary>
        /// Given an index of an atom in the query get the index of the other atom in
        /// the double bond.
        /// </summary>
        /// <param name="i">query atom index</param>
        /// <returns>the other atom index involved in a double bond</returns>
        private int OtherIndex(int i)
        {
            IDoubleBondStereochemistry element = (IDoubleBondStereochemistry)queryElements[i];
            return queryMap[element.StereoBond.GetOther(query.Atoms[i])];
        }

        /// <summary>
        /// Create an index of atoms for the provided <paramref name="container"/>.
        /// </summary>
        /// <param name="container">the container to index the atoms of</param>
        /// <returns>the index/lookup of atoms to the index they appear</returns>
        private static Dictionary<IAtom, int> IndexAtoms(IAtomContainer container)
        {
            var map = new Dictionary<IAtom, int>(container.Atoms.Count);
            for (int i = 0; i < container.Atoms.Count; i++)
                map[container.Atoms[i]] = i;
            return map;
        }

        /// <summary>
        /// Index the stereo elements of the <paramref name="container"/> into the the 
        /// <paramref name="elements"/> and <paramref name="types"/> arrays. The <paramref name="map"/> is used for looking
        /// up the index of atoms.
        /// </summary>
        /// <param name="map">index of atoms</param>
        /// <param name="elements">array to fill with stereo elements</param>
        /// <param name="types">type of stereo element indexed</param>
        /// <param name="container">the container to index the elements of</param>
        /// <returns>indices of atoms involved in stereo configurations</returns>
        private static int[] IndexElements(Dictionary<IAtom, int> map, IStereoElement<IChemObject, IChemObject>[] elements, StereoType[] types, IAtomContainer container)
        {
            var indices = new int[container.Atoms.Count];
            int nElements = 0;
            foreach (var element in container.StereoElements)
            {
                if (element is ITetrahedralChirality tc)
                {
                    int idx = map[tc.ChiralAtom];
                    elements[idx] = element;
                    types[idx] = StereoType.Tetrahedral;
                    indices[nElements++] = idx;
                }
                else if (element is IDoubleBondStereochemistry dbs)
                {
                    int idx1 = map[dbs.StereoBond.Begin];
                    int idx2 = map[dbs.StereoBond.End];
                    elements[idx2] = elements[idx1] = element;
                    types[idx1] = types[idx2] = StereoType.Geometric;
                    indices[nElements++] = idx1; // only visit the first atom
                }
            }
            return Arrays.CopyOf(indices, nElements);
        }

        /// <summary>
        /// Get the parity (-1,+1) of the tetrahedral configuration.
        /// </summary>
        /// <param name="stereo">configuration</param>
        /// <returns>the parity</returns>
        private static int Parity(TetrahedralStereo stereo)
        {
            return stereo == TetrahedralStereo.Clockwise ? 1 : -1;
        }

        /// <summary>
        /// Get the parity (-1,+1) of the geometric (double bond) configuration.
        /// </summary>
        /// <param name="conformation">configuration</param>
        /// <returns>the parity</returns>
        private static int Parity(DoubleBondConformation conformation)
        {
            return conformation == DoubleBondConformation.Together ? 1 : -1;
        }

        // could be moved into the StereoElement to allow faster introspection
        private enum StereoType
        {
            Unset = 0,
            Tetrahedral = 1,
            Geometric = 2,
        }
    }
}
