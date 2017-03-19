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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using NCDK.Graphs;
using System;
using System.Collections.Generic;
using NCDK.Numerics;
using static NCDK.Graphs.GraphUtil;

namespace NCDK.Stereo
{
    /// <summary>
    /// Create stereo elements for a structure with 2D and 3D coordinates. The
    /// factory does not verify whether atoms can or cannot support stereochemistry -
    /// for this functionality use <see cref="Stereocenters"/>. The factory will not create
    /// stereo elements if there is missing information (wedge/hatch bonds, undefined
    /// coordinates) or the layout indicates unspecified configuration.
    ///
    /// Stereocenters specified with inverse down (hatch) bond style are created if
    /// the configuration is unambiguous and the bond does not connect to another
    /// stereocenter.
    /// </summary>
    /// <example><code>
    /// IAtomContainer       container = ...;
    /// StereoElementFactory stereo    = StereoElementFactory.Using2DCoordinates()
    ///                                                      .InterpretProjections(Projection.Haworth);
    ///
    /// // set the elements replacing any existing elements (recommended)
    /// container.SetStereoElements(stereo.CreateAll());
    ///
    /// // adding elements individually is no recommended as the AtomContainer
    /// // does not check for duplicate or contradicting elements
    /// foreach (var element in stereo.CreateAll())
    ///     container.AddStereoElement(element); // bad, there may already be elements
    /// </code>
    /// </example>
    /// <seealso cref="Stereocenters"/>
    // @author John May
    // @cdk.module standard
    // @cdk.githash
    public abstract class StereoElementFactory
    {

        /// <summary>Native CDK structure representation.</summary>
        protected readonly IAtomContainer container;

        /// <summary>Adjacency list graph representation.</summary>
        protected readonly int[][] graph;

        /// <summary>A bond map for fast access to bond labels between two atom indices.</summary>
        protected readonly EdgeToBondMap bondMap;

        protected readonly List<Projection> projections = new List<Projection>();

        /// <summary>
        /// Internal constructor.
        ///
        /// <param name="container">an atom container</param>
        /// <param name="graph">adjacency list representation</param>
        /// <param name="bondMap">lookup bonds by atom index</param>
        /// </summary>
        protected StereoElementFactory(IAtomContainer container, int[][] graph, EdgeToBondMap bondMap)
        {
            this.container = container;
            this.graph = graph;
            this.bondMap = bondMap;
        }

        /// <summary>
        /// Creates all stereo elements found by <see cref="Stereocenters"/> using the or
        /// 2D/3D coordinates to specify the configuration (clockwise/anticlockwise).
        /// Currently only <see cref="ITetrahedralChirality"/> and {@link
        /// IDoubleBondStereochemistry} elements are created..
        ///
        /// <returns>a list of stereo elements</returns>
        /// </summary>
        public IList<IStereoElement> CreateAll()
        {

            Stereocenters centers = new Stereocenters(container, graph, bondMap);
            List<IStereoElement> elements = new List<IStereoElement>();

            // projection recognition (note no action in constructors)
            FischerRecognition fischerRecon = new FischerRecognition(container, graph, bondMap, centers);
            CyclicCarbohydrateRecognition cycleRecon = new CyclicCarbohydrateRecognition(container, graph, bondMap, centers);

            elements.AddRange(fischerRecon.Recognise(projections));
            elements.AddRange(cycleRecon.Recognise(projections));

            for (int v = 0; v < graph.Length; v++)
            {
                switch (centers.ElementType(v))
                {
                    case Stereocenters.CoordinateTypes.Bicoordinate:
                        int t0 = graph[v][0];
                        int t1 = graph[v][1];
                        if (centers.ElementType(t0) == Stereocenters.CoordinateTypes.Tricoordinate
                                && centers.ElementType(t1) == Stereocenters.CoordinateTypes.Tricoordinate)
                        {
                            if (centers.IsStereocenter(t0) && centers.IsStereocenter(t1))
                            {
                                IStereoElement element_ = CreateExtendedTetrahedral(v, centers);
                                if (element_ != null) elements.Add(element_);
                            }
                        }
                        break;
                    case Stereocenters.CoordinateTypes.Tricoordinate:
                        if (!centers.IsStereocenter(v)) continue;
                        foreach (var w in graph[v])
                        {
                            if (w > v && bondMap[v, w].Order == BondOrder.Double)
                            {
                                if (centers.IsStereocenter(w))
                                {
                                    IStereoElement element_ = CreateGeometric(v, w, centers);
                                    if (element_ != null) elements.Add(element_);
                                }
                                break;
                            }
                        }
                        break;
                    case Stereocenters.CoordinateTypes.Tetracoordinate:
                        IStereoElement element = CreateTetrahedral(v, centers);
                        if (element != null) elements.Add(element);
                        break;
                }
            }

            return elements;
        }

        /// <summary>
        /// Create a tetrahedral element for the atom at index <paramref name="v"/>. If a
        /// tetrahedral element could not be created then null is returned. An
        /// element can not be created if, one or more atoms was missing coordinates,
        /// the atom has an unspecified (wavy) bond, the atom is no non-planar bonds
        /// (i.e. up/down, wedge/hatch). The method does not check if tetrahedral
        /// chirality is supported - for this functionality use <see cref="Stereocenters"/>.
        /// </summary>
        /// <example><code>
        /// StereoElementFactory  factory   = ...; // 2D/3D
        /// IAtomContainer        container = ...; // container
        ///
        /// for (int v = 0; v &lt; container.Atoms.Count; v++) {
        ///     // ... verify v is a stereo atom ...
        ///     ITetrahedralChirality element = factory.CreateTetrahedral(v);
        ///     if (element != null)
        ///         container.AddStereoElement(element);
        /// }
        /// </code>
        /// </example>
        /// <param name="v">atom index (vertex)</param>
        /// <returns>a new stereo element</returns>
        public abstract ITetrahedralChirality CreateTetrahedral(int v, Stereocenters stereocenters);

        /// <summary>
        /// Create a tetrahedral element for the atom. If a tetrahedral element could
        /// not be created then null is returned. An element can not be created if,
        /// one or more atoms was missing coordinates, the atom has an unspecified
        /// (wavy) bond, the atom is no non-planar bonds (i.e. up/down, wedge/hatch).
        /// The method does not check if tetrahedral chirality is supported - for
        /// this functionality use <see cref="Stereocenters"/>.
        /// </summary>
        /// <example><code>
        /// StereoElementFactory  factory   = ...; // 2D/3D
        /// IAtomContainer        container = ...; // container
        ///
        /// foreach (var atom in container.Atoms) {
        ///     // ... verify atom is a stereo atom ...
        ///     ITetrahedralChirality element = factory.CreateTetrahedral(atom);
        ///     if (element != null)
        ///         container.AddStereoElement(element);
        /// }
        /// </code></example>
        /// <param name="atom">atom</param>
        /// <returns>a new stereo element</returns>
        public abstract ITetrahedralChirality CreateTetrahedral(IAtom atom, Stereocenters stereocenters);

        /// <summary>
        /// Create a geometric element (double-bond stereochemistry) for the provided
        /// atom indices. If the configuration could not be created a null element is
        /// returned. There is no configuration is the coordinates do not indicate a
        /// configuration, there were undefined coordinates or an unspecified bond
        /// label. The method does not check if double bond stereo is supported - for
        /// this functionality use <see cref="Stereocenters"/>.
        /// </summary>
        /// <param name="u">an atom index</param>
        /// <param name="v">an atom pi bonded 'v'</param>
        /// <returns>a new stereo element</returns>
        public abstract IDoubleBondStereochemistry CreateGeometric(int u, int v, Stereocenters stereocenters);

        /// <summary>
        /// Create a geometric element (double-bond stereochemistry) for the provided
        /// double bond. If the configuration could not be created a null element is
        /// returned. There is no configuration is the coordinates do not indicate a
        /// configuration, there were undefined coordinates or an unspecified bond
        /// label. The method does not check if double bond stereo is supported - for
        /// this functionality use <see cref="Stereocenters"/>.
        /// </summary>
        /// <example><code>
        /// StereoElementFactory  factory   = ...; // 2D/3D
        /// IAtomContainer        container = ...; // container
        ///
        /// foreach (var bond in container.Bonds) {
        ///     if (bond.Order != Double)
        ///         continue;
        ///     // ... verify bond is a stereo bond...
        ///     IDoubleBondStereochemistry element = factory.CreateGeometric(bond);
        ///     if (element != null)
        ///         container.AddStereoElement(element);
        /// }
        /// </code></example>
        /// <param name="bond">the bond to create a configuration for</param>
        /// <returns>a new stereo element</returns>
        public abstract IDoubleBondStereochemistry CreateGeometric(IBond bond, Stereocenters stereocenters);

        /// <summary>
        /// Create an extended tetrahedral element for the atom at index <paramref name="v"/>.
        /// If an extended  tetrahedral element could not be created then null is
        /// returned. An element can not be created if, one or more atoms was
        /// missing coordinates, the atom has an unspecified (wavy) bond, the atom
        /// is no non-planar bonds (i.e. up/down, wedge/hatch). The method does not
        /// check if tetrahedral chirality is supported - for this functionality
        /// use <see cref="Stereocenters"/>.
        /// </summary>
        /// <example><code>
        /// StereoElementFactory  factory   = ...; // 2D/3D
        /// IAtomContainer        container = ...; // container
        ///
        /// for (int v = 0; v &lt; container.Atoms.Count; v++) {
        ///     // ... verify v is a stereo atom ...
        ///     ExtendedTetrahedral element = factory.CreateExtendedTetrahedral(v);
        ///     if (element != null)
        ///         container.AddStereoElement(element);
        /// }
        /// </code></example>
        /// <param name="v">atom index (vertex)</param>
        /// <returns>a new stereo element</returns>
        public abstract ExtendedTetrahedral CreateExtendedTetrahedral(int v, Stereocenters stereocenters);

        /// <summary>
        /// Indicate that stereochemistry drawn as a certain projection should be
        /// interpreted. 
        /// </summary>
        /// <code>
        /// StereoElementFactory factory = 
        ///   StereoElementFactory.Using2DCoordinates(container)
        ///                       .InterpretProjections(Projection.Fischer, Projection.Haworth);
        /// </code>
        /// <param name="projections">types of projection</param>
        /// <returns>self</returns>
        /// <seealso cref="Projection"/>
        public StereoElementFactory InterpretProjections(params Projection[] projections)
        {
            this.projections.AddRange(projections);
            return this;
        }

        /// <summary>
        /// Create a stereo element factory for creating stereo elements using 2D
        /// coordinates and depiction labels (up/down, wedge/hatch).
        /// </summary>
        /// <param name="container">the structure to create the factory for</param>
        /// <returns>the factory instance</returns>
        public static StereoElementFactory Using2DCoordinates(IAtomContainer container)
        {
            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(container);
            int[][] graph = GraphUtil.ToAdjList(container, bondMap);
            return new StereoElementFactory2D(container, graph, bondMap);
        }

        /// <summary>
        /// Create a stereo element factory for creating stereo elements using 3D
        /// coordinates and depiction labels (up/down, wedge/hatch).
        /// </summary>
        /// <param name="container">the structure to create the factory for</param>
        /// <returns>the factory instance</returns>
        public static StereoElementFactory Using3DCoordinates(IAtomContainer container)
        {
            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(container);
            int[][] graph = GraphUtil.ToAdjList(container, bondMap);
            return new StereoElementFactory3D(container, graph, bondMap);
        }

        private static bool HasUnspecifiedParity(IAtom atom)
        {
            return atom.StereoParity != null && atom.StereoParity == 3;
        }

        /// <summary>Create stereo-elements from 2D coordinates.</summary>
        sealed class StereoElementFactory2D : StereoElementFactory
        {
            /// <summary>
            /// Threshold at which the determinant is considered too small (unspecified
            /// by coordinates).
            /// </summary>
            private const double Threshold = 0.1;

            /// <summary>
            /// Create a new stereo-element factory for the specified structure.
            /// </summary>
            /// <param name="container">native CDK structure representation</param>
            /// <param name="graph">adjacency list representation</param>
            /// <param name="bondMap">fast bond lookup from atom indices</param>
            public StereoElementFactory2D(IAtomContainer container, int[][] graph, EdgeToBondMap bondMap)
                : base(container, graph, bondMap)
            {
            }

            /// <inheritdoc/>
            public override ITetrahedralChirality CreateTetrahedral(IAtom atom, Stereocenters stereocenters)
            {
                return CreateTetrahedral(container.Atoms.IndexOf(atom), stereocenters);
            }

            /// <inheritdoc/>
            public override IDoubleBondStereochemistry CreateGeometric(IBond bond, Stereocenters stereocenters)
            {
                return CreateGeometric(container.Atoms.IndexOf(bond.Atoms[0]), container.Atoms.IndexOf(bond.Atoms[1]), stereocenters);
            }

            /// <inheritdoc/>
            public override ITetrahedralChirality CreateTetrahedral(int v, Stereocenters stereocenters)
            {
                IAtom focus = container.Atoms[v];

                if (HasUnspecifiedParity(focus)) return null;

                IAtom[] neighbors = new IAtom[4];
                int[] elevation = new int[4];

                neighbors[3] = focus;

                bool nonplanar = false;
                int n = 0;

                foreach (var w in graph[v])
                {
                    IBond bond = bondMap[v, w];

                    // wavy bond
                    if (IsUnspecified(bond)) return null;

                    neighbors[n] = container.Atoms[w];
                    elevation[n] = ElevationOf(focus, bond);

                    if (elevation[n] != 0) nonplanar = true;

                    n++;
                }

                // too few/many neighbors
                if (n < 3 || n > 4) return null;

                // TODO: verify valid wedge/hatch configurations using similar procedure
                // to NonPlanarBonds in the cdk-sdg package.

                // no up/down bonds present - check for inverted down/hatch
                if (!nonplanar)
                {
                    int[] ws = graph[v];
                    for (int i = 0; i < ws.Length; i++)
                    {
                        int w = ws[i];
                        IBond bond = bondMap[v, w];

                        // we have already previously checked whether 'v' is at the
                        // 'point' and so these must be inverse (fat-end @
                        // stereocenter) ala Daylight
                        if (bond.Stereo == BondStereo.Down || bond.Stereo == BondStereo.DownInverted)
                        {

                            // we stick to the 'point' end convention but can
                            // interpret if the bond isn't connected to another
                            // stereocenter - otherwise it's ambiguous!
                            if (stereocenters.IsStereocenter(w)) continue;

                            elevation[i] = -1;
                            nonplanar = true;
                        }
                    }

                    // still no bonds to use
                    if (!nonplanar) return null;
                }

                int parity = Parity(focus, neighbors, elevation);

                if (parity == 0) return null;

                TetrahedralStereo winding = parity > 0 ? TetrahedralStereo.AntiClockwise : TetrahedralStereo.Clockwise;

                return new TetrahedralChirality(focus, neighbors, winding);
            }

            /// <inheritdoc/>
            public override IDoubleBondStereochemistry CreateGeometric(int u, int v, Stereocenters stereocenters)
            {
                if (HasUnspecifiedParity(container.Atoms[u]) || HasUnspecifiedParity(container.Atoms[v])) return null;

                int[] us = graph[u];
                int[] vs = graph[v];

                if (us.Length < 2 || us.Length > 3 || vs.Length < 2 || vs.Length > 3) return null;

                // move pi bonded neighbors to back
                MoveToBack(us, v);
                MoveToBack(vs, u);

                IAtom[] vAtoms = new IAtom[]{container.Atoms[us[0]], container.Atoms[us.Length > 2 ? us[1] : u], container.Atoms[v]};
                IAtom[] wAtoms = new IAtom[]{container.Atoms[vs[0]], container.Atoms[vs.Length > 2 ? vs[1] : v], container.Atoms[u]};

                // are any substituents a wavy unspecified bond
                if (IsUnspecified(bondMap[u, us[0]]) || IsUnspecified(bondMap[u, us[1]])
                        || IsUnspecified(bondMap[v, vs[0]]) || IsUnspecified(bondMap[v, vs[1]])) return null;

                int parity = Parity(vAtoms) * Parity(wAtoms);
                DoubleBondConformation conformation = parity > 0 ? DoubleBondConformation.Opposite : DoubleBondConformation.Together;

                if (parity == 0) return null;

                IBond bond = bondMap[u, v];

                // crossed bond
                if (IsUnspecified(bond)) return null;

                // put the bond in to v is the first neighbor
                bond.SetAtoms(new[] { container.Atoms[u], container.Atoms[v] });

                return new DoubleBondStereochemistry(bond, new IBond[] { bondMap[u, us[0]], bondMap[v, vs[0]] }, conformation);
            }

            /// <inheritdoc/>
            public override ExtendedTetrahedral CreateExtendedTetrahedral(int v, Stereocenters stereocenters)
            {
                IAtom focus = container.Atoms[v];

                if (HasUnspecifiedParity(focus)) return null;

                IAtom[] terminals = ExtendedTetrahedral.FindTerminalAtoms(container, focus);

                int t0 = container.Atoms.IndexOf(terminals[0]);
                int t1 = container.Atoms.IndexOf(terminals[1]);

                // check the focus is cumulated
                if (bondMap[v, t0].Order != BondOrder.Double
                        || bondMap[v, t1].Order != BondOrder.Double) return null;

                IAtom[] neighbors = new IAtom[4];
                int[] elevation = new int[4];

                neighbors[1] = terminals[0];
                neighbors[3] = terminals[1];

                int n = 0;
                foreach (var w in graph[t0])
                {
                    IBond bond = bondMap[t0, w];
                    if (w == v) continue;
                    if (bond.Order != BondOrder.Single) return null;
                    if (IsUnspecified(bond)) return null;
                    neighbors[n] = container.Atoms[w];
                    elevation[n] = ElevationOf(terminals[0], bond);
                    n++;
                }
                n = 2;
                foreach (var w in graph[t1])
                {
                    IBond bond = bondMap[t1, w];
                    if (w == v) continue;
                    if (bond.Order != BondOrder.Single) return null;
                    if (IsUnspecified(bond)) return null;
                    neighbors[n] = container.Atoms[w];
                    elevation[n] = ElevationOf(terminals[1], bond);
                    n++;
                }

                if (elevation[0] != 0 || elevation[1] != 0)
                {
                    if (elevation[2] != 0 || elevation[3] != 0) return null;
                }
                else
                {
                    if (elevation[2] == 0 && elevation[3] == 0) return null; // undefined configuration
                }

                int parity = Parity(focus, neighbors, elevation);

                TetrahedralStereo winding = parity > 0 ? TetrahedralStereo.AntiClockwise : TetrahedralStereo.Clockwise;

                return new ExtendedTetrahedral(focus, neighbors, winding);
            }

            /// <summary>
            /// Is the provided bond have an unspecified stereo label.
            /// </summary>
            /// <param name="bond">a bond</param>
            /// <returns>the bond has unspecified stereochemistry</returns>
            private bool IsUnspecified(IBond bond)
            {
                switch (bond.Stereo.Ordinal)
                {
                    case BondStereo.O.UpOrDown:
                    case BondStereo.O.UpOrDownInverted:
                    case BondStereo.O.EOrZ:
                        return true;
                    default:
                        return false;
                }
            }

            /// <summary>
            /// Parity computation for one side of a double bond in a geometric center.
            /// </summary>
            /// <param name="atoms">atoms around the double bonded atom, 0: substituent, 1:
            ///              other substituent (or focus), 2: double bonded atom</param>
            /// <returns>the parity of the atoms</returns>
            private int Parity(IAtom[] atoms)
            {
                if (atoms.Length != 3) throw new ArgumentException("incorrect number of atoms");

                Vector2? a = atoms[0].Point2D;
                Vector2? b = atoms[1].Point2D;
                Vector2? c = atoms[2].Point2D;

                if (a == null || b == null || c == null) return 0;

                double det = Det(a.Value.X, a.Value.Y, b.Value.X, b.Value.Y, c.Value.X, c.Value.Y);

                // unspecified by coordinates
                if (Math.Abs(det) < Threshold) return 0;

                return (int)Math.Sign(det);
            }

            /// <summary>
            /// Parity computation for 2D tetrahedral stereocenters.
            /// </summary>
            /// <param name="atoms">the atoms surrounding the central focus atom</param>
            /// <param name="elevations">the elevations of each atom</param>
            /// <returns>the parity (winding)</returns>
            private int Parity(IAtom focus, IAtom[] atoms, int[] elevations)
            {
                if (atoms.Length != 4) throw new ArgumentException("incorrect number of atoms");

                Vector2[] coordinates = new Vector2[atoms.Length];
                for (int i = 0; i < atoms.Length; i++)
                {
                    var atoms_i_Point2D = atoms[i].Point2D;
                    if (atoms_i_Point2D == null) return 0;
                    coordinates[i] = ToUnitVector(focus.Point2D.Value, atoms_i_Point2D.Value);
                }

                double det = Parity(coordinates, elevations);

                return (int)Math.Sign(det);
            }

            /// <summary>
            /// Obtain the unit vector between two points.
            /// </summary>
            /// <param name="from">the base of the vector</param>
            /// <param name="to">the point of the vector</param>
            /// <returns>the unit vector</returns>
            private Vector2 ToUnitVector(Vector2 from, Vector2 to)
            {
                if (from == to) return Vector2.Zero;
                Vector2 v2d = new Vector2(to.X - from.X, to.Y - from.Y);
                return Vector2.Normalize(v2d);
            }

            /// <summary>
            /// Compute the signed volume of the tetrahedron from the planar points
            /// and elevations.
            /// </summary>
            /// <param name="coordinates">locations in the plane</param>
            /// <param name="elevations">elevations above/below the plane</param>
            /// <returns>the determinant (signed volume of tetrahedron)</returns>
            private double Parity(Vector2[] coordinates, int[] elevations)
            {
                double x1 = coordinates[0].X;
                double x2 = coordinates[1].X;
                double x3 = coordinates[2].X;
                double x4 = coordinates[3].X;

                double y1 = coordinates[0].Y;
                double y2 = coordinates[1].Y;
                double y3 = coordinates[2].Y;
                double y4 = coordinates[3].Y;

                return (elevations[0] * Det(x2, y2, x3, y3, x4, y4)) - (elevations[1] * Det(x1, y1, x3, y3, x4, y4))
                        + (elevations[2] * Det(x1, y1, x2, y2, x4, y4)) - (elevations[3] * Det(x1, y1, x2, y2, x3, y3));
            }

            /// <summary>3x3 determinant helper for a constant third column</summary>
            private static double Det(double xa, double ya, double xb, double yb, double xc, double yc)
            {
                return (xa - xc) * (yb - yc) - (ya - yc) * (xb - xc);
            }

            /// <summary>
            /// Utility find the specified value, <paramref name="v"/>, in the array of values,
            /// <paramref name="vs"/> and moves it to the back.
            /// </summary>
            /// <param name="vs">an array of values (containing v)</param>
            /// <param name="v">a value</param>
            private static void MoveToBack(int[] vs, int v)
            {
                for (int i = 0; i < vs.Length; i++)
                {
                    if (vs[i] == v)
                    {
                        Array.Copy(vs, i + 1, vs, i + 1 - 1, vs.Length - (i + 1));
                        vs[vs.Length - 1] = v;
                        return;
                    }
                }
            }

            /// <summary>
            /// Obtain the elevation of an atom connected to the <paramref name="focus"/> by the
            /// specified <paramref name="bond"/>.
            /// </summary>
            /// <param name="focus">a focus of stereochemistry</param>
            /// <param name="bond">a bond connecting the focus to a substituent</param>
            /// <returns>the elevation of the connected atom, +1 above, -1 below, 0 planar</returns>
            private int ElevationOf(IAtom focus, IBond bond)
            {
                switch (bond.Stereo.Ordinal)
                {
                    case BondStereo.O.Up:
                        return bond.Atoms[0] == focus ? +1 : 0;
                    case BondStereo.O.UpInverted:
                        return bond.Atoms[1] == focus ? +1 : 0;
                    case BondStereo.O.Down:
                        return bond.Atoms[0] == focus ? -1 : 0;
                    case BondStereo.O.DownInverted:
                        return bond.Atoms[1] == focus ? -1 : 0;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>Create stereo-elements from 3D coordinates.</summary>
        private sealed class StereoElementFactory3D : StereoElementFactory
        {
            /// <summary>
            /// Create a new stereo-element factory for the specified structure.
            /// </summary>
            /// <param name="container">native CDK structure representation</param>
            /// <param name="graph">adjacency list representation</param>
            /// <param name="bondMap">fast bond lookup from atom indices</param>
            public StereoElementFactory3D(IAtomContainer container, int[][] graph, EdgeToBondMap bondMap)
                : base(container, graph, bondMap)
            {
            }

            /// <inheritdoc/>
            public override ITetrahedralChirality CreateTetrahedral(IAtom atom, Stereocenters stereocenters)
            {
                return CreateTetrahedral(container.Atoms.IndexOf(atom), stereocenters);
            }

            /// <inheritdoc/>
            public override IDoubleBondStereochemistry CreateGeometric(IBond bond, Stereocenters stereocenters)
            {
                return CreateGeometric(container.Atoms.IndexOf(bond.Atoms[0]), container.Atoms.IndexOf(bond.Atoms[1]), stereocenters);
            }

            /// <inheritdoc/>
            public override ITetrahedralChirality CreateTetrahedral(int v, Stereocenters stereocenters)
            {
                if (!stereocenters.IsStereocenter(v)) return null;

                IAtom focus = container.Atoms[v];

                if (HasUnspecifiedParity(focus)) return null;

                IAtom[] neighbors = new IAtom[4];

                neighbors[3] = focus;

                int n = 0;

                foreach (var w in graph[v])
                    neighbors[n++] = container.Atoms[w];

                // too few/many neighbors
                if (n < 3 || n > 4) return null;

                // TODO: verify valid wedge/hatch configurations using similar procedure
                // to NonPlanarBonds in the cdk-sdg package

                int parity = Parity(neighbors);

                TetrahedralStereo winding = parity > 0 ? TetrahedralStereo.AntiClockwise : TetrahedralStereo.Clockwise;

                return new TetrahedralChirality(focus, neighbors, winding);
            }

            /// <inheritdoc/>
            public override IDoubleBondStereochemistry CreateGeometric(int u, int v, Stereocenters stereocenters)
            {
                if (HasUnspecifiedParity(container.Atoms[u]) || HasUnspecifiedParity(container.Atoms[v])) return null;

                int[] us = graph[u];
                int[] vs = graph[v];

                int x = us[0] == v ? us[1] : us[0];
                int w = vs[0] == u ? vs[1] : vs[0];

                IAtom uAtom = container.Atoms[u];
                IAtom vAtom = container.Atoms[v];
                IAtom uSubstituentAtom = container.Atoms[x];
                IAtom vSubstituentAtom = container.Atoms[w];

                if (uAtom.Point3D == null || vAtom.Point3D == null || uSubstituentAtom.Point3D == null
                        || vSubstituentAtom.Point3D == null) return null;

                int parity = Parity(uAtom.Point3D.Value, vAtom.Point3D.Value, uSubstituentAtom.Point3D.Value,
                        vSubstituentAtom.Point3D.Value);

                DoubleBondConformation conformation = parity > 0 ? DoubleBondConformation.Opposite : DoubleBondConformation.Together;

                IBond bond = bondMap[u, v];
                bond.Atoms[0] = uAtom;
                bond.Atoms[1] = vAtom;

                return new DoubleBondStereochemistry(bond, new IBond[] { bondMap[u, x], bondMap[v, w], }, conformation);
            }

            /// <inheritdoc/>
            public override ExtendedTetrahedral CreateExtendedTetrahedral(int v, Stereocenters stereocenters)
            {
                IAtom focus = container.Atoms[v];

                if (HasUnspecifiedParity(focus)) return null;

                IAtom[] terminals = ExtendedTetrahedral.FindTerminalAtoms(container, focus);
                IAtom[] neighbors = new IAtom[4];

                int t0 = container.Atoms.IndexOf(terminals[0]);
                int t1 = container.Atoms.IndexOf(terminals[1]);

                // check the focus is cumulated
                if (bondMap[v, t0].Order != BondOrder.Double
                        || bondMap[v, t1].Order != BondOrder.Double) return null;

                neighbors[1] = terminals[0];
                neighbors[3] = terminals[1];

                int n = 0;
                foreach (var w in graph[t0])
                {
                    if (bondMap[t0, w].Order != BondOrder.Single) continue;
                    neighbors[n++] = container.Atoms[w];
                }
                n = 2;
                foreach (var w in graph[t1])
                {
                    if (bondMap[t1, w].Order != BondOrder.Single) continue;
                    neighbors[n++] = container.Atoms[w];
                }

                int parity = Parity(neighbors);

                TetrahedralStereo winding = parity > 0 ? TetrahedralStereo.AntiClockwise : TetrahedralStereo.Clockwise;

                return new ExtendedTetrahedral(focus, neighbors, winding);
            }

            /// <summary>3x3 determinant helper for a constant third column</summary>
            private static double Det(double xa, double ya, double xb, double yb, double xc, double yc)
            {
                return (xa - xc) * (yb - yc) - (ya - yc) * (xb - xc);
            }

            /// <summary>
            /// Parity computation for one side of a double bond in a geometric center.
            /// The method needs the 3D coordinates of the double bond atoms (first 2
            /// arguments) and the coordinates of two substituents (one at each end).
            /// </summary>
            /// <param name="u">an atom double bonded to v</param>
            /// <param name="v">an atom double bonded to u</param>
            /// <param name="x">an atom sigma bonded to u</param>
            /// <param name="w">an atom sigma bonded to v</param>
            /// <returns>the parity of the atoms</returns>
            private int Parity(Vector3 u, Vector3 v, Vector3 x, Vector3 w)
            {
                // create three vectors, v->u, v->w and u->x
                double[] vu = ToVector(v, u);
                double[] vw = ToVector(v, w);
                double[] ux = ToVector(u, x);

                // normal vector (to compare against), the normal vector (n) looks like:
                // x     n w
                //  \    |/
                //   u = v
                double[] normal = CrossProduct(vu, CrossProduct(vu, vw));

                // compare the dot products of v->w and u->x, if the signs are the same
                // they are both pointing the same direction. if a value is close to 0
                // then it is at pi/2 radians (i.e. unspecified) however 3D coordinates
                // are generally discrete and do not normally represent on unspecified
                // stereo configurations so we don't check this
                int parity = (int)Math.Sign(Dot(normal, vw)) * (int)Math.Sign(Dot(normal, ux));

                // invert sign, this then matches with Sp2 double bond parity
                return parity * -1;
            }

            /// <summary>
            /// Parity computation for 3D tetrahedral stereocenters.
            /// </summary>
            /// <param name="atoms">the atoms surrounding the central focus atom</param>
            /// <returns>the parity (winding)</returns>
            private int Parity(IAtom[] atoms)
            {
                if (atoms.Length != 4) throw new ArgumentException("incorrect number of atoms");

                Vector3[] coordinates = new Vector3[atoms.Length];
                for (int i = 0; i < atoms.Length; i++)
                {
                    var c = atoms[i].Point3D;
                    if (c == null) return 0;
                    coordinates[i] = c.Value;
                }

                double x1 = coordinates[0].X;
                double x2 = coordinates[1].X;
                double x3 = coordinates[2].X;
                double x4 = coordinates[3].X;

                double y1 = coordinates[0].Y;
                double y2 = coordinates[1].Y;
                double y3 = coordinates[2].Y;
                double y4 = coordinates[3].Y;

                double z1 = coordinates[0].Z;
                double z2 = coordinates[1].Z;
                double z3 = coordinates[2].Z;
                double z4 = coordinates[3].Z;

                double det = (z1 * Det(x2, y2, x3, y3, x4, y4)) - (z2 * Det(x1, y1, x3, y3, x4, y4))
                        + (z3 * Det(x1, y1, x2, y2, x4, y4)) - (z4 * Det(x1, y1, x2, y2, x3, y3));

                return (int)Math.Sign(det);
            }

            /// <summary>
            /// Create a vector by specifying the source and destination coordinates.
            /// </summary>
            /// <param name="src">start point of the vector</param>
            /// <param name="dest">end point of the vector</param>
            /// <returns>a new vector</returns>
            private static double[] ToVector(Vector3 src, Vector3 dest)
            {
                return new double[] { dest.X - src.X, dest.Y - src.Y, dest.Z - src.Z };
            }

            /// <summary>
            /// Dot product of two 3D coordinates
            /// </summary>
            /// <param name="u">either 3D coordinates</param>
            /// <param name="v">other 3D coordinates</param>
            /// <returns>the dot-product</returns>
            private static double Dot(double[] u, double[] v)
            {
                return (u[0] * v[0]) + (u[1] * v[1]) + (u[2] * v[2]);
            }

            /// <summary>
            /// Cross product of two 3D coordinates
            /// </summary>
            /// <param name="u">either 3D coordinates</param>
            /// <param name="v">other 3D coordinates</param>
            /// <returns>the cross-product</returns>
            private static double[] CrossProduct(double[] u, double[] v)
            {
                return new double[]{(u[1] * v[2]) - (v[1] * u[2]), (u[2] * v[0]) - (v[2] * u[0]), (u[0] * v[1]) - (v[0] * u[1])};
            }
        }
    }
}
