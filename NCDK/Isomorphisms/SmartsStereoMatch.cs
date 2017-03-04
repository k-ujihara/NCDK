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
    /**
     * Filters SMARTS matches for those that have valid stereochemistry
     * configuration.
     *
     * Note: This class is internal and will be private in future.
     *
     * @author John May
     * @cdk.module smarts
     * @cdk.githash
     */
    public sealed class SmartsStereoMatch : NCDK.Common.Base.Predicate<int[]>
    {

        /// <summary>Query and target contains.</summary>
        private readonly IAtomContainer query, target;

        /// <summary>Atom to atom index lookup.</summary>
        private readonly IDictionary<IAtom, int> queryMap, targetMap;

        /// <summary>Indexed array of stereo elements.</summary>
        private readonly IStereoElement[] queryElements, targetElements;

        /// <summary>Indexed array of stereo element types.</summary>
        private readonly StereoType[] queryTypes, targetTypes;

        /// <summary>Indices of focus atoms of stereo elements.</summary>
        private readonly int[] queryStereoIndices, targetStereoIndices;

        /**
         * Create a predicate for checking mappings between a provided
         * {@code query} and {@code target}.
         *
         * @param query query container
         * @param target target container
         */
        public SmartsStereoMatch(IAtomContainer query, IAtomContainer target)
        {

            if (!(query is IQueryAtomContainer))
                throw new ArgumentException("match predicate is for SMARTS only");
            if (!(query.Atoms[0] is SMARTSAtom))
                throw new ArgumentException("match predicate is for SMARTS only");

            this.query = query;
            this.target = target;

            this.queryMap = IndexAtoms(query);
            this.targetMap = IndexAtoms(target);
            this.queryElements = new IStereoElement[query.Atoms.Count];
            this.targetElements = new IStereoElement[target.Atoms.Count];
            this.queryTypes = new StereoType[query.Atoms.Count];
            this.targetTypes = new StereoType[target.Atoms.Count];

            queryStereoIndices = IndexElements(queryMap, queryElements, queryTypes, query);
            targetStereoIndices = IndexElements(targetMap, targetElements, targetTypes, target);
        }

        /**
         * Is the {@code mapping} of the stereochemistry in the query preserved in
         * the target.
         *
         * @param mapping permutation of the query vertices
         * @return the stereo chemistry is value
         */
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

        /**
         * Verify the tetrahedral stereochemistry (clockwise/anticlockwise) of atom
         * {@code u} is preserved in the target when the {@code mapping} is used.
         *
         * @param u       tetrahedral index in the target
         * @param mapping mapping of vertices
         * @return the tetrahedral configuration is preserved
         */
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
            if (targetTypes[v] == StereoType.Unset) return queryAtom.ChiralityMatches(targetAtom, 0, p);

            // target was non-tetrahedral
            if (targetTypes[v] != StereoType.Tetrahedral) return false;

            int[] vs = Neighbors(targetElement, targetMap);
            int q = PermutationParity(vs) * Parity(targetElement.Stereo);

            return queryAtom.ChiralityMatches(targetAtom, q, p);
        }

        /**
         * Transforms the neighbors {@code us} adjacent to {@code u} into the target
         * indices using the mapping {@code mapping}. The transformation accounts
         * for an implicit hydrogen in the query being an explicit hydrogen in the
         * target.
         *
         * @param u       central atom of tetrahedral element
         * @param v       mapped central atom of the tetrahedral element
         * @param us      neighboring vertices of u (u plural)
         * @param mapping mapping from the query to the target
         * @return the neighbors us, transformed into the neighbors around v
         */
        private int[] Map(int u, int v, int[] us, int[] mapping)
        {
            for (int i = 0; i < us.Length; i++)
                us[i] = mapping[us[i]];
            return us;
        }

        /**
         * Verify the geometric stereochemistry (cis/trans) of the double bond
         * {@code u1=u2} is preserved in the target when the {@code mapping} is
         * used.
         *
         * @param u1      one index of the double bond
         * @param u2      other index of the double bond
         * @param mapping mapping of vertices
         * @return the geometric configuration is preserved
         */
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
            if (targetElement.StereoBond.Atoms[0] != target.Atoms[v1])
            {
                int tmp = v1;
                v1 = v2;
                v2 = tmp;
                swap = true;
            }

            var targetBonds = targetElement.Bonds;

            int p = Parity(queryElement.Stereo);
            int q = Parity(targetElement.Stereo);

            int uLeft = queryMap[queryBonds[0].GetConnectedAtom(query.Atoms[u1])];
            int uRight = queryMap[queryBonds[1].GetConnectedAtom(query.Atoms[u2])];

            int vLeft = targetMap[targetBonds[0].GetConnectedAtom(target.Atoms[v1])];
            int vRight = targetMap[targetBonds[1].GetConnectedAtom(target.Atoms[v2])];

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

        /**
         * Access the neighbors of {@code element} as their indices.
         *
         * @param element tetrahedral element
         * @param map     atom index lookup
         * @return the neighbors
         */
        private int[] Neighbors(ITetrahedralChirality element, IDictionary<IAtom, int> map)
        {
            var atoms = element.Ligands;
            int[] vs = new int[atoms.Count];
            for (int i = 0; i < atoms.Count; i++)
                vs[i] = map[atoms[i]];
            return vs;
        }

        /**
         * Compute the permutation parity of the values {@code vs}. The parity is
         * whether we need to do an odd or even number of swaps to put the values in
         * sorted order.
         *
         * @param vs values
         * @return parity of the permutation (odd = -1, even = +1)
         */
        private int PermutationParity(int[] vs)
        {
            int n = 0;
            for (int i = 0; i < vs.Length; i++)
                for (int j = i + 1; j < vs.Length; j++)
                    if (vs[i] > vs[j]) n++;
            return (n & 0x1) == 1 ? -1 : 1;
        }

        /**
         * Given an index of an atom in the query get the index of the other atom in
         * the double bond.
         *
         * @param i query atom index
         * @return the other atom index involved in a double bond
         */
        private int OtherIndex(int i)
        {
            IDoubleBondStereochemistry element = (IDoubleBondStereochemistry)queryElements[i];
            return queryMap[element.StereoBond.GetConnectedAtom(query.Atoms[i])];
        }

        /**
         * Create an index of atoms for the provided <paramref name="container"/>.
         *
         * @param container the container to index the atoms of
         * @return the index/lookup of atoms to the index they appear
         */
        private static IDictionary<IAtom, int> IndexAtoms(IAtomContainer container)
        {
            IDictionary<IAtom, int> map = new Dictionary<IAtom, int>(container.Atoms.Count);
            for (int i = 0; i < container.Atoms.Count; i++)
                map[container.Atoms[i]] = i;
            return map;
        }

        /**
         * Index the stereo elements of the <paramref name="container"/> into the the {@code
         * elements} and {@code types} arrays. The {@code map} is used for looking
         * up the index of atoms.
         *
         * @param map       index of atoms
         * @param elements  array to fill with stereo elements
         * @param types     type of stereo element indexed
         * @param container the container to index the elements of
         * @return indices of atoms involved in stereo configurations
         */
        private static int[] IndexElements(IDictionary<IAtom, int> map, IStereoElement[] elements, StereoType[] types,
                IAtomContainer container)
        {
            int[] indices = new int[container.Atoms.Count];
            int nElements = 0;
            foreach (var element in container.StereoElements)
            {
                if (element is ITetrahedralChirality)
                {
                    ITetrahedralChirality tc = (ITetrahedralChirality)element;
                    int idx = map[tc.ChiralAtom];
                    elements[idx] = element;
                    types[idx] = StereoType.Tetrahedral;
                    indices[nElements++] = idx;
                }
                else if (element is IDoubleBondStereochemistry)
                {
                    IDoubleBondStereochemistry dbs = (IDoubleBondStereochemistry)element;
                    int idx1 = map[dbs.StereoBond.Atoms[0]];
                    int idx2 = map[dbs.StereoBond.Atoms[1]];
                    elements[idx2] = elements[idx1] = element;
                    types[idx1] = types[idx2] = StereoType.Geometric;
                    indices[nElements++] = idx1; // only visit the first atom
                }
            }
            return Arrays.CopyOf(indices, nElements);
        }

        /**
         * Get the parity (-1,+1) of the tetrahedral configuration.
         *
         * @param stereo configuration
         * @return the parity
         */
        private int Parity(TetrahedralStereo stereo)
        {

            return stereo == TetrahedralStereo.Clockwise ? 1 : -1;
        }

        /**
         * Get the parity (-1,+1) of the geometric (double bond) configuration.
         *
         * @param conformation configuration
         * @return the parity
         */
        private int Parity(DoubleBondConformation conformation)
        {
            return conformation == DoubleBondConformation.Together ? 1 : -1;
        }

        // could be moved into the IStereoElement to allow faster introspection
        private enum StereoType
        {
            Unset = 0,
            Tetrahedral = 1,
            Geometric = 2,
        }
    }
}
