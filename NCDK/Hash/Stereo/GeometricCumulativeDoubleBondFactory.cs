/*
 * Copyright (c) 2013 John May <jwmay@users.sf.net>
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

using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Numerics;

namespace NCDK.Hash.Stereo
{
    /// <summary>
    /// Stereo encoder factory for 2D and 3D cumulative double bonds.
    ///
    // @author John May
    // @cdk.module hash
    /// </summary>
    public class GeometricCumulativeDoubleBondFactory : IStereoEncoderFactory
    {

        /// <summary>
        /// Create a stereo encoder for cumulative double bonds.
        ///
        /// <param name="container">the container</param>
        /// <param name="graph">adjacency list representation of the container</param>
        /// <returns>a stereo encoder</returns>
        /// </summary>
        public IStereoEncoder Create(IAtomContainer container, int[][] graph)
        {
            int n = container.Atoms.Count;
            BondMap map = new BondMap(n);

            var encoders = new List<IStereoEncoder>(1);

            // index double bonds by their atoms
            foreach (var bond in container.Bonds)
            {
                if (IsDoubleBond(bond)) map.Add(bond);
            }

            var visited = new HashSet<IAtom>();

            // find atoms which are connected between two double bonds
            foreach (var a in map.Atoms)
            {
                var bonds = map.bonds[a];
                if (bonds.Count == 2)
                {

                    // (s)tart/(e)nd of cumulated system: -s=a=e-
                    IAtom s = bonds[0].GetConnectedAtom(a);
                    IAtom e = bonds[1].GetConnectedAtom(a);
                    // need the parents to re-use the double bond encoder
                    IAtom sParent = a;
                    IAtom eParent = a;

                    visited.Add(a);
                    visited.Add(s);
                    visited.Add(e);

                    int size = 2;

                    // expand out from 'l'
                    while (s != null && map.Cumulated(s))
                    {
                        IAtom p = map.bonds[s][0].GetConnectedAtom(s);
                        IAtom q = map.bonds[s][1].GetConnectedAtom(s);
                        sParent = s;
                        s = visited.Add(p) ? p : visited.Add(q) ? q : null;
                        size++;
                    }

                    // expand from 'r'
                    while (e != null && map.Cumulated(e))
                    {
                        IAtom p = map.bonds[e][0].GetConnectedAtom(e);
                        IAtom q = map.bonds[e][1].GetConnectedAtom(e);
                        eParent = e;
                        e = visited.Add(p) ? p : visited.Add(q) ? q : null;
                        size++;
                    }

                    // s and e are null if we had a cumulative cycle...
                    if (s != null && e != null)
                    {

                        // system has now be expanded, size is the number of double
                        // bonds. For odd numbers we use E/Z whilst for even are
                        // axial M/P.
                        //  \           /
                        //   s = = = = e
                        //  /           \
                        if (IsOdd(size))
                        {
                            IStereoEncoder encoder = GeometricDoubleBondEncoderFactory.NewEncoder(container, s, sParent, e,
                                    eParent, graph);
                            if (encoder != null)
                            {
                                encoders.Add(encoder);
                            }
                        }
                        else
                        {
                            IStereoEncoder encoder = AxialEncoder(container, s, e);
                            if (encoder != null)
                            {
                                encoders.Add(encoder);
                            }
                        }
                    }
                }
            }

            return encoders.Count == 0 ? StereoEncoder.EMPTY : new MultiStereoEncoder(encoders);
        }

        /// <summary>
        /// Create an encoder for axial 2D stereochemistry for the given start and
        /// end atoms.
        ///
        /// <param name="container">the molecule</param>
        /// <param name="start">start of the cumulated system</param>
        /// <param name="end">end of the cumulated system</param>
        /// <returns>an encoder or null if there are no coordinated</returns>
        /// </summary>
        internal static IStereoEncoder AxialEncoder(IAtomContainer container, IAtom start, IAtom end)
        {
            var startBonds = container.GetConnectedBonds(start);
            var endBonds = container.GetConnectedBonds(end);

            if (startBonds.Count() < 2 || endBonds.Count() < 2) return null;

            if (Has2DCoordinates(startBonds) && Has2DCoordinates(endBonds))
            {
                return Axial2DEncoder(container, start, startBonds, end, endBonds);
            }
            else if (Has3DCoordinates(startBonds) && Has3DCoordinates(endBonds))
            {
                return Axial3DEncoder(container, start, startBonds, end, endBonds);
            }

            return null;
        }

        /// <summary>
        /// Create an encoder for axial 2D stereochemistry for the given start and
        /// end atoms.
        ///
        /// <param name="container">the molecule</param>
        /// <param name="start">start of the cumulated system</param>
        /// <param name="startBonds">bonds connected to the start</param>
        /// <param name="end">end of the cumulated system</param>
        /// <param name="endBonds">bonds connected to the end</param>
        /// <returns>an encoder</returns>
        /// </summary>
        private static IStereoEncoder Axial2DEncoder(IAtomContainer container, IAtom start, IEnumerable<IBond> startBonds,
                IAtom end, IEnumerable<IBond> endBonds)
        {
            Vector2[] ps = new Vector2[4];
            int[] es = new int[4];

            PermutationParity perm = new CombinedPermutationParity(Fill2DCoordinates(container, start, startBonds, ps, es,
                    0), Fill2DCoordinates(container, end, endBonds, ps, es, 2));

            GeometricParity geom = new Tetrahedral2DParity(ps, es);

            int u = container.Atoms.IndexOf(start);
            int v = container.Atoms.IndexOf(end);

            return new GeometryEncoder(new int[] { u, v }, perm, geom);
        }

        /// <summary>
        /// Create an encoder for axial 3D stereochemistry for the given start and
        /// end atoms.
        ///
        /// <param name="container">the molecule</param>
        /// <param name="start">start of the cumulated system</param>
        /// <param name="startBonds">bonds connected to the start</param>
        /// <param name="end">end of the cumulated system</param>
        /// <param name="endBonds">bonds connected to the end</param>
        /// <returns>an encoder</returns>
        /// </summary>
        private static IStereoEncoder Axial3DEncoder(IAtomContainer container, IAtom start, IEnumerable<IBond> startBonds,
                IAtom end, IEnumerable<IBond> endBonds)
        {

            Vector3[] coordinates = new Vector3[4];

            PermutationParity perm = new CombinedPermutationParity(Fill3DCoordinates(container, start, startBonds,
                    coordinates, 0), Fill3DCoordinates(container, end, endBonds, coordinates, 2));

            GeometricParity geom = new Tetrahedral3DParity(coordinates);

            int u = container.Atoms.IndexOf(start);
            int v = container.Atoms.IndexOf(end);

            return new GeometryEncoder(new int[] { u, v }, perm, geom);
        }

        /// <summary>
        /// Fill the {@literal coordinates} and {@literal elevation} from the given
        /// offset index. If there is only one connection then the second entry (from
        /// the offset) will use the coordinates of <i>a</i>. The permutation parity
        /// is also built and returned.
        ///
        /// <param name="container">atom container</param>
        /// <param name="a">the central atom</param>
        /// <param name="connected">bonds connected to the central atom</param>
        /// <param name="coordinates">the coordinates array to fill</param>
        /// <param name="elevations">the elevations of the connected atoms</param>
        /// <param name="offset">current location in the offset array</param>
        /// <returns>the permutation parity</returns>
        /// </summary>
        private static PermutationParity Fill2DCoordinates(IAtomContainer container, IAtom a, IEnumerable<IBond> connected,
                Vector2[] coordinates, int[] elevations, int offset)
        {
            int i = 0;
            coordinates[offset + 1] = a.Point2D.Value;
            elevations[offset + 1] = 0;
            int[] indices = new int[2];

            foreach (var bond in connected)
            {
                if (!IsDoubleBond(bond))
                {
                    IAtom other = bond.GetConnectedAtom(a);
                    coordinates[i + offset] = other.Point2D.Value;
                    elevations[i + offset] = Elevation(bond, a);
                    indices[i] = container.Atoms.IndexOf(other);
                    i++;
                }
            }

            if (i == 1)
            {
                return PermutationParity.IDENTITY;
            }
            else
            {
                return new BasicPermutationParity(indices);
            }

        }

        /// <summary>
        /// Fill the {@literal coordinates} from the given offset index. If there is
        /// only one connection then the second entry (from the offset) will use the
        /// coordinates of <i>a</i>. The permutation parity is also built and
        /// returned.
        ///
        /// <param name="container">atom container</param>
        /// <param name="a">the central atom</param>
        /// <param name="connected">bonds connected to the central atom</param>
        /// <param name="coordinates">the coordinates array to fill</param>
        /// <param name="offset">current location in the offset array</param>
        /// <returns>the permutation parity</returns>
        /// </summary>
        private static PermutationParity Fill3DCoordinates(IAtomContainer container, IAtom a, IEnumerable<IBond> connected,
                Vector3[] coordinates, int offset)
        {

            int i = 0;
            int[] indices = new int[2];

            foreach (var bond in connected)
            {
                if (!IsDoubleBond(bond))
                {
                    IAtom other = bond.GetConnectedAtom(a);
                    coordinates[i + offset] = other.Point3D.Value;
                    indices[i] = container.Atoms.IndexOf(other);
                    i++;
                }
            }

            // only one connection, use the coordinate of 'a'
            if (i == 1)
            {
                coordinates[offset + 1] = a.Point3D.Value;
                return PermutationParity.IDENTITY;
            }
            else
            {
                return new BasicPermutationParity(indices);
            }
        }

        /// <summary>
        /// Check if all atoms in the bond list have 2D coordinates. There is some
        /// redundant checking but the list will typically be short.
        ///
        /// <param name="bonds">the bonds to check</param>
        /// <returns>whether all atoms have 2D coordinates</returns>
        /// </summary>
        private static bool Has2DCoordinates(IEnumerable<IBond> bonds)
        {
            foreach (var bond in bonds)
            {
                if (bond.Atoms[0].Point2D == null || bond.Atoms[1].Point2D == null) return false;
            }
            return true;
        }

        /// <summary>
        /// Check if all atoms in the bond list have 3D coordinates. There is some
        /// redundant checking but the list will typically be short.
        ///
        /// <param name="bonds">the bonds to check</param>
        /// <returns>whether all atoms have 2D coordinates</returns>
        /// </summary>
        private static bool Has3DCoordinates(IEnumerable<IBond> bonds)
        {
            foreach (var bond in bonds)
            {
                if (bond.Atoms[0].Point3D == null || bond.Atoms[1].Point3D == null) return false;
            }
            return true;
        }

        /// <summary>
        /// Access the elevation of a bond relative to the given source atom. With a
        /// wedge bond if the atom <i>a</i> is the <i>point</i> end then the bond
        /// comes off the paper <i>above</i> the plane. If <i>a</i> is the <i>fat</i>
        /// end then the bond from <i>a</i> goes <i>below</i> the plane.
        ///
        /// <param name="bond">a bond</param>
        /// <param name="a">an atom</param>
        /// <returns>elevation of bond</returns>
        /// </summary>
        internal static int Elevation(IBond bond, IAtom a)
        {
            return bond.Atoms[0].Equals(a) ? Elevation(bond) : Elevation(bond) * -1;
        }

        /// <summary>
        /// Access the elevation of a bond.
        ///
        /// <param name="bond">the bond</param>
        /// <returns>+1 above the plane, 0 in the plane (default) or -1 below the</returns>
        ///         plane
        /// </summary>
        internal static int Elevation(IBond bond)
        {
            BondStereo stereo = bond.Stereo;
            switch (stereo.Ordinal)
            {
                case BondStereo.O.None:
                    return 0;
                case BondStereo.O.Up:
                case BondStereo.O.DownInverted:
                    return +1;
                case BondStereo.O.Down:
                case BondStereo.O.UpInverted:
                    return -1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Is the value <i>x</i> odd?
        ///
        /// <param name="x">an int value</param>
        /// <returns>whether x is odd</returns>
        /// </summary>
        private static bool IsOdd(int x)
        {
            return (x & 0x1) != 0;
        }

        /// <summary>
        /// Determine whether the bond order is 'double'.
        ///
        /// <param name="bond">a bond</param>
        /// <returns>the bond is a double bond.</returns>
        /// </summary>
        private static bool IsDoubleBond(IBond bond)
        {
            return BondOrder.Double.Equals(bond.Order);
        }

        /// <summary>
        /// Helper class for storing a lookup of atoms and their connected double
        /// bonds.
        /// </summary>
        private class BondMap
        {
            public IDictionary<IAtom, List<IBond>> bonds;

            /// <summary>
            /// Create new bond map for the specified number of atoms.
            ///
            /// <param name="n">atom count</param>
            /// </summary>
            public BondMap(int n)
            {
                bonds = new Dictionary<IAtom, List<IBond>>(n > 3 ? n + (n / 3) : n);
            }

            /// <summary>
            /// List of bonds involving the atom.
            ///
            /// <param name="a">atom</param>
            /// <returns>list of bonds, empty if none stored</returns>
            /// </summary>
            public IEnumerable<IBond> GetBonds(IAtom a)
            {
                List<IBond> bs;
                if (!bonds.TryGetValue(a, out bs))
                    return new IBond[0];
                return bs;
            }

            /// <summary>
            /// Check whether the the atom is cumulated - two consecutive double
            /// bonds.
            ///
            /// <param name="a">an atom</param>
            /// <returns>whether the atom is cumulated</returns>
            /// </summary>
            public bool Cumulated(IAtom a)
            {
                return bonds[a].Count == 2;
            }

            /// <summary>
            /// Add a bond to the map.
            ///
            /// <param name="bond">the bond to add</param>
            /// </summary>
            public void Add(IBond bond)
            {
                Add(bond.Atoms[0], bond);
                Add(bond.Atoms[1], bond);
            }

            /// <summary>
            /// Add the bond for the provided atom.
            ///
            /// <param name="a">an atom of the bond</param>
            /// <param name="b">the bond</param>
            /// </summary>
            private void Add(IAtom a, IBond b)
            {
                if (!GetBonds(a).Any())
                    bonds[a] = new List<IBond>(2);
                bonds[a].Add(b);
            }

            /// <summary>
            /// Set of atoms which have double bonds.
            ///
            /// <returns>iterable set of atoms</returns>
            /// </summary>
            public IEnumerable<IAtom> Atoms => bonds.Keys;
        }
    }
}
