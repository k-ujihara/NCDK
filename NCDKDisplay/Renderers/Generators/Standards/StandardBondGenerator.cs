/*
 * Copyright (c) 2014  European Bioinformatics Institute (EMBL-EBI)
 *                     John May <jwmay@users.sf.net>
 *               2014  Mark B Vine (orcid:0000-0002-7794-0426)
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Common.Mathematics;
using NCDK.Common.Primitives;
using NCDK.Config;
using NCDK.Graphs;
using NCDK.Numerics;
using NCDK.Renderers.Elements;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using static NCDK.Renderers.Generators.Standards.VecmathUtil;

namespace NCDK.Renderers.Generators.Standards
{
    /// <summary>
    /// Generates <see cref="IRenderingElement"/>s for bonds. The generator is internal and called by the
    /// <see cref="StandardGenerator"/>. A new bond generator is required for each container instance.
    /// </summary>
    /// <remarks>
    /// The bonds generated are: 
    /// <list type="bullet">
    /// <item><see cref="GenerateSingleBond(IBond, IAtom, IAtom)"/> - delegates to one of the following types: 
    ///     <list type="bullet"> 
    ///         <item><see cref="GeneratePlainSingleBond(IAtom, IAtom)"/> - single line between two atoms</item>
    ///         <item><see cref="GenerateBoldWedgeBond(IAtom, IAtom, IList{IBond})"/> - wedged up stereo </item> 
    ///         <item><see cref="GenerateHashedWedgeBond(IAtom, IAtom, List{IBond})"/> - wedged down stereo bond </item> 
    ///         <item><see cref="GenerateWavyBond(IAtom, IAtom)"/> - up or down bond </item>
    ///     </list> 
    /// </item>
    /// <item><see cref="GenerateDoubleBond(IBond, bool)"/> - delegates to one of the following types: 
    ///     <list type="bullet"> 
    ///         <item><see cref="GenerateOffsetDoubleBond(IBond, IAtom, IAtom, IBond, List{IBond}, bool)"/> - one line rests on the center between the atoms</item>
    ///         <item><see cref="GenerateCenteredDoubleBond(IBond, IAtom, IAtom, List{IBond}, List{IBond})"/>- both lines rest equidistant from the center between the atoms</item>
    ///         <item><see cref="GenerateCrossedDoubleBond(IAtom, IAtom)"/> - unknown double stereochemistry </item> 
    ///     </list> 
    /// </item>
    ///     <item><see cref="GenerateTripleBond(IBond, IAtom, IAtom)"/> - composes a single and double bond</item>
    ///     <item><see cref="GenerateDashedBond(IAtom, IAtom)"/> - the unknown bond type</item> 
    /// </list>
    /// </remarks>
    // @author John May
    internal sealed class StandardBondGenerator
    {
        private readonly IAtomContainer container;
        private readonly AtomSymbol[] symbols;
        private readonly RendererModel parameters;

        // indexes of atoms and rings
        private readonly IDictionary<IAtom, int> atomIndexMap = new Dictionary<IAtom, int>();
        private readonly IDictionary<IBond, IAtomContainer> ringMap;

        // parameters
        private readonly double scale;
        private readonly double stroke;
        private readonly double separation;
        private readonly double backOff;
        private readonly double wedgeWidth;
        private readonly double hashSpacing;
        private readonly double waveSpacing;
        private readonly Color foreground, annotationColor;
        private readonly bool fancyBoldWedges, fancyHashedWedges;
        private readonly double annotationDistance, annotationScale;
        private readonly Typeface font;
        private readonly double emSize;
        private readonly ElementGroup annotations;
        private readonly bool forceDelocalised;

        /// <summary>
        /// Create a new standard bond generator for the provided structure (container) with the laid out
        /// atom symbols. The parameters of the bond generation are also provided and the scaled 'stroke'
        /// width which is used to scale all other parameters.
        /// </summary>
        /// <param name="container">structure representation</param>
        /// <param name="symbols">generated atom symbols</param>
        /// <param name="parameters">rendering options</param>
        /// <param name="stroke">scaled stroke width</param>
        private StandardBondGenerator(IAtomContainer container, AtomSymbol[] symbols, RendererModel parameters, ElementGroup annotations, Typeface font, double emSize, double stroke)
        {
            this.container = container;
            this.symbols = symbols;
            this.parameters = parameters;
            this.annotations = annotations;

            // index atoms and rings
            for (int i = 0; i < container.Atoms.Count; i++)
                atomIndexMap[container.Atoms[i]] = i;
            ringMap = RingPreferenceMap(container);

            // set parameters
            this.scale = parameters.GetScale();
            this.stroke = stroke;
            double length = parameters.GetBondLength() / scale;
            this.separation = (parameters.GetBondSeparation() * parameters.GetBondLength()) / scale;
            this.backOff = parameters.GetSymbolMarginRatio() * stroke;
            this.wedgeWidth = parameters.GetWedgeRatio() * stroke;
            this.hashSpacing = parameters.GetHashSpacing() / scale;
            this.waveSpacing = parameters.GetWaveSpacing() / scale;
            this.fancyBoldWedges = parameters.GetFancyBoldWedges();
            this.fancyHashedWedges = parameters.GetFancyHashedWedges();
            this.annotationDistance = parameters.GetAnnotationDistance() * (parameters.GetBondLength() / scale);
            this.annotationScale = (1 / scale) * parameters.GetAnnotationFontScale();
            this.annotationColor = parameters.GetAnnotationColor();
            this.forceDelocalised = parameters.GetForceDelocalisedBondDisplay();
            this.font = font;
            this.emSize = emSize;

            // foreground is based on the carbon color
            this.foreground = parameters.GetAtomColorer().GetAtomColor(container.Builder.NewAtom("C"));
        }

        /// <summary>
        /// Generates bond elements for the provided structure (container) with the laid out atom
        /// symbols. The parameters of the bond generation are also provided and the scaled 'stroke'
        /// width which is used to scale all other parameters.
        /// </summary>
        /// <param name="container">structure representation</param>
        /// <param name="symbols">generated atom symbols</param>
        /// <param name="parameters">rendering options</param>
        /// <param name="stroke">scaled stroke width</param>
        public static IRenderingElement[] GenerateBonds(IAtomContainer container, AtomSymbol[] symbols, RendererModel parameters, double stroke, Typeface font, double emSize, ElementGroup annotations)
        {
            StandardBondGenerator bondGenerator = new StandardBondGenerator(container, symbols, parameters, annotations, font, emSize, stroke);
            IRenderingElement[] elements = new IRenderingElement[container.Bonds.Count];
            for (int i = 0; i < container.Bonds.Count; i++)
            {
                var bond = container.Bonds[i];
                if (!StandardGenerator.IsHidden(bond))
                {
                    elements[i] = bondGenerator.Generate(bond);
                }
            }
            return elements;
        }

        /// <summary>
        /// Generate a rendering element for a given bond.
        /// </summary>
        /// <param name="bond">a bond</param>
        /// <returns>rendering element</returns>
        internal IRenderingElement Generate(IBond bond)
        {
            IAtom atom1 = bond.Begin;
            IAtom atom2 = bond.End;

            BondOrder order = bond.Order;

            IRenderingElement elem;

            switch (order)
            {
                case BondOrder.Single:
                    if (bond.IsAromatic && forceDelocalised)
                        elem = GenerateDoubleBond(bond, true);
                    else
                        elem = GenerateSingleBond(bond, atom1, atom2);
                    break;
                case BondOrder.Double:
                    elem = GenerateDoubleBond(bond, bond.IsAromatic && forceDelocalised);
                    break;
                case BondOrder.Triple:
                    elem = GenerateTripleBond(bond, atom1, atom2);
                    break;
                default:
                    if (bond.IsAromatic && order == BondOrder.Unset)
                    {
                        elem = GenerateDoubleBond(bond, true);
                    }
                    else
                    {
                        // bond orders > 3 not supported
                        elem = GenerateDashedBond(atom1, atom2);
                    }
                    break;
            }

            // attachment point drawing, in future we could also draw the attach point
            // number, typically within a circle
            if (IsAttachPoint(atom1))
            {
                ElementGroup elemGrp = new ElementGroup
                {
                    elem,
                    GenerateAttachPoint(atom1, bond)
                };
                elem = elemGrp;
            }
            if (IsAttachPoint(atom2))
            {
                ElementGroup elemGrp = new ElementGroup
                {
                    elem,
                    GenerateAttachPoint(atom2, bond)
                };
                elem = elemGrp;
            }

            return elem;
        }

        /// <summary>
        /// Generate a rendering element for single bond with the provided stereo type.
        /// </summary>
        /// <param name="bond">the bond to render</param>
        /// <param name="from">an atom</param>
        /// <param name="to">another atom</param>
        /// <returns>bond rendering element</returns>
        private IRenderingElement GenerateSingleBond(IBond bond, IAtom from, IAtom to)
        {
            BondStereo stereo = bond.Stereo;
            if (stereo == BondStereo.None)
                return GeneratePlainSingleBond(from, to);

            var fromBonds = container.GetConnectedBonds(from).ToList();
            var toBonds = container.GetConnectedBonds(to).ToList();

            fromBonds.Remove(bond);
            toBonds.Remove(bond);

            // add annotation label
            string label = StandardGenerator.GetAnnotationLabel(bond);
            if (label != null) AddAnnotation(from, to, label);

            switch (stereo)
            {
                case BondStereo.None:
                    return GeneratePlainSingleBond(from, to);
                case BondStereo.Down:
                    return GenerateHashedWedgeBond(from, to, toBonds);
                case BondStereo.DownInverted:
                    return GenerateHashedWedgeBond(to, from, fromBonds);
                case BondStereo.Up:
                    return GenerateBoldWedgeBond(from, to, toBonds);
                case BondStereo.UpInverted:
                    return GenerateBoldWedgeBond(to, from, fromBonds);
                case BondStereo.UpOrDown:
                case BondStereo.UpOrDownInverted: // up/down is undirected
                    return GenerateWavyBond(to, from);
                default:
                    Trace.TraceWarning("Unknown single bond stereochemistry ", stereo, " is not displayed");
                    return GeneratePlainSingleBond(from, to);
            }
        }

        /// <summary>
        /// Generate a plain single bond between two atoms accounting for displayed symbols.
        /// </summary>
        /// <param name="from">one atom</param>
        /// <param name="to">the other atom</param>
        /// <returns>rendering element</returns>
        internal IRenderingElement GeneratePlainSingleBond(IAtom from, IAtom to)
        {
            return NewLineElement(BackOffPoint(from, to), BackOffPoint(to, from));
        }

        /// <summary>
        /// Generates a rendering element for a bold wedge bond (i.e. up) from one atom to another.
        /// </summary>
        /// <param name="from">narrow end of the wedge</param>
        /// <param name="to">bold end of the wedge</param>
        /// <param name="toBonds">bonds connected to the 'to atom'</param>
        /// <returns>the rendering element</returns>
        internal IRenderingElement GenerateBoldWedgeBond(IAtom from, IAtom to, IList<IBond> toBonds)
        {
            var fromPoint = from.Point2D.Value;
            var toPoint = to.Point2D.Value;

            var fromBackOffPoint = BackOffPoint(from, to);
            var toBackOffPoint = BackOffPoint(to, from);

            var unit = NewUnitVector(fromPoint, toPoint);
            var perpendicular = NewPerpendicularVector(unit);

            double halfNarrowEnd = stroke / 2;
            double halfWideEnd = wedgeWidth / 2;

            double opposite = halfWideEnd - halfNarrowEnd;
            double adjacent = Vector2.Distance(fromPoint, toPoint);

            double fromOffset = halfNarrowEnd + opposite / adjacent * Vector2.Distance(fromBackOffPoint, fromPoint);
            double toOffset = halfNarrowEnd + opposite / adjacent * Vector2.Distance(toBackOffPoint, fromPoint);

            // four points of the trapezoid
            Vector2 a = Sum(fromBackOffPoint, Scale(perpendicular, fromOffset));
            Vector2 b = Sum(fromBackOffPoint, Scale(perpendicular, -fromOffset));
            Vector2 c = Sum(toBackOffPoint, Scale(perpendicular, -toOffset));
            Vector2 e = toBackOffPoint;
            Vector2 d = Sum(toBackOffPoint, Scale(perpendicular, toOffset));

            // don't adjust wedge if the angle is shallow than this amount
            double threshold = Vectors.DegreeToRadian(15);

            // if the symbol at the wide end of the wedge is not displayed, we can improve
            // the aesthetics by adjusting the endpoints based on connected bond angles.
            if (fancyBoldWedges && !HasDisplayedSymbol(to))
            {
                // slanted wedge
                if (toBonds.Count == 1)
                {
                    IBond toBondNeighbor = toBonds[0];
                    IAtom toNeighbor = toBondNeighbor.GetOther(to);

                    Vector2 refVector = NewUnitVector(toPoint, toNeighbor.Point2D.Value);
                    bool wideToWide = false;

                    // special case when wedge bonds are in a bridged ring, wide-to-wide end we
                    // don't want to slant as normal but rather butt up against each wind end
                    if (AtWideEndOfWedge(to, toBondNeighbor))
                    {
                        refVector = Sum(refVector, Negate(unit));
                        wideToWide = true;
                    }

                    double theta = Vectors.Angle(refVector, unit);

                    if (theta > threshold && theta + threshold + threshold < Math.PI)
                    {
                        c = Intersection(b, NewUnitVector(b, c), toPoint, refVector);
                        d = Intersection(a, NewUnitVector(a, d), toPoint, refVector);

                        // the points c, d, and e lie on the center point of the line between
                        // the 'to' and 'toNeighbor'. Since the bond is drawn with a stroke and
                        // has a thickness we need to move these points slightly to be flush
                        // with the bond depiction, we only do this if the bond is not
                        // wide-on-wide with another bold wedge
                        if (!wideToWide)
                        {
                            double nudge = (stroke / 2) / Math.Sin(theta);
                            c = Sum(c, Scale(unit, nudge));
                            d = Sum(d, Scale(unit, nudge));
                            e = Sum(e, Scale(unit, nudge));
                        }
                    }
                }

                // bifurcated (forked) wedge
                else if (toBonds.Count > 1)
                {
                    Vector2 refVectorA = GetNearestVector(perpendicular, to, toBonds);
                    Vector2 refVectorB = GetNearestVector(Negate(perpendicular), to, toBonds);

                    if (Vectors.Angle(refVectorB, unit) > threshold) c = Intersection(b, NewUnitVector(b, c), toPoint, refVectorB);
                    if (Vectors.Angle(refVectorA, unit) > threshold) d = Intersection(a, NewUnitVector(a, d), toPoint, refVectorA);
                }
            }

            var path = new PathGeometry();
            var pf = new PathFigure()
            {
                StartPoint = ToPoint(a),
                Segments = new PathSegmentCollection(new[]
                    {
                        new LineSegment(ToPoint(b), false),
                        new LineSegment(ToPoint(c), false),
                        new LineSegment(ToPoint(e), false),
                        new LineSegment(ToPoint(d), false),
                    }),
            };
            path.Figures.Add(pf);

            return new GeneralPath(path, foreground);
        }

        /// <summary>
        /// Generates a rendering element for a hashed wedge bond (i.e. down) from one atom to another.
        /// </summary>
        /// <param name="from">narrow end of the wedge</param>
        /// <param name="to">bold end of the wedge</param>
        /// <param name="toBonds">bonds connected to</param>
        /// <returns>the rendering element</returns>
        internal IRenderingElement GenerateHashedWedgeBond(IAtom from, IAtom to, List<IBond> toBonds)
        {
            Vector2 fromPoint = from.Point2D.Value;
            Vector2 toPoint = to.Point2D.Value;

            Vector2 fromBackOffPoint = BackOffPoint(from, to);
            Vector2 toBackOffPoint = BackOffPoint(to, from);

            Vector2 unit = NewUnitVector(fromPoint, toPoint);
            Vector2 perpendicular = NewPerpendicularVector(unit);

            double halfNarrowEnd = stroke / 2;
            double halfWideEnd = wedgeWidth / 2;

            double opposite = halfWideEnd - halfNarrowEnd;
            double adjacent = Vector2.Distance(fromPoint, toPoint);

            int nSections = (int)(adjacent / hashSpacing);
            double step = adjacent / (nSections - 1);

            ElementGroup group = new ElementGroup();

            double start = HasDisplayedSymbol(from) ? Vector2.Distance(fromPoint, fromBackOffPoint) : double.NegativeInfinity;
            double end = HasDisplayedSymbol(to) ? Vector2.Distance(fromPoint, toBackOffPoint) : double.PositiveInfinity;

            // don't adjust wedge if the angle is shallow than this amount
            double threshold = Vectors.DegreeToRadian(35);

            Vector2 hatchAngle = perpendicular;

            // fancy hashed wedges with slanted hatch sections aligned with neighboring bonds
            if (CanDrawFancyHashedWedge(to, toBonds, adjacent))
            {
                IBond toBondNeighbor = toBonds[0];
                IAtom toNeighbor = toBondNeighbor.GetOther(to);

                Vector2 refVector = NewUnitVector(toPoint, toNeighbor.Point2D.Value);

                // special case when wedge bonds are in a bridged ring, wide-to-wide end we
                // don't want to slant as normal but rather butt up against each wind end
                if (AtWideEndOfWedge(to, toBondNeighbor))
                {
                    refVector = Sum(refVector, Negate(unit));
                    refVector = Vector2.Normalize(refVector);
                }

                // only slant if the angle isn't shallow
                double theta = Vectors.Angle(refVector, unit);
                if (theta > threshold && theta + threshold + threshold < Math.PI)
                {
                    hatchAngle = refVector;
                }
            }

            for (int i = 0; i < nSections; i++)
            {
                double distance = i * step;

                // don't draw if we're within an atom symbol
                if (distance < start || distance > end) continue;

                double offset = halfNarrowEnd + opposite / adjacent * distance;
                Vector2 interval = fromPoint + Scale(unit, distance);
                group.Add(NewLineElement(Sum(interval, Scale(hatchAngle, offset)),
                        Sum(interval, Scale(hatchAngle, -offset))));
            }

            return group;
        }

        /// <summary>
        /// A fancy hashed wedge can be drawn if the following conditions are met:
        /// <list type="number">
        /// <item><see cref="RendererModelTools.GetFancyHashedWedges"/> is enabled</item>
        /// <item>Bond is of 'normal' length</item>
        /// <item>The atom at the wide has one other neighbor and no symbol displayed</item>
        /// </list>
        /// </summary>
        /// <param name="to">the target atom</param>
        /// <param name="toBonds">bonds to the target atom (excluding the hashed wedge)</param>
        /// <param name="length">the length of the bond (unscaled)</param>
        /// <returns>a fancy hashed wedge can be rendered</returns>
        private bool CanDrawFancyHashedWedge(IAtom to, List<IBond> toBonds, double length)
        {
            // a bond is long if is more than 4 units larger that the desired 'BondLength'
            bool longBond = (length * scale) - parameters.GetBondLength() > 4;
            return fancyHashedWedges && !longBond && !HasDisplayedSymbol(to) && toBonds.Count == 1;
        }

        /// <summary>
        /// Generates a wavy bond (up or down stereo) between two atoms.
        /// </summary>
        /// <param name="from">drawn from this atom</param>
        /// <param name="to">drawn to this atom</param>
        /// <returns>generated rendering element</returns>
        internal IRenderingElement GenerateWavyBond(IAtom from, IAtom to)
        {
            Vector2 fromPoint = from.Point2D.Value;
            Vector2 toPoint = to.Point2D.Value;

            Vector2 fromBackOffPoint = BackOffPoint(from, to);
            Vector2 toBackOffPoint = BackOffPoint(to, from);

            Vector2 unit = NewUnitVector(fromPoint, toPoint);
            Vector2 perpendicular = NewPerpendicularVector(unit);

            double length = Vector2.Distance(fromPoint, toPoint);

            // 2 times the number of wave sections because each semi circle is drawn with two parts
            int nCurves = 2 * (int)(length / waveSpacing);
            double step = length / nCurves;

            Vector2 peak = Scale(perpendicular, step);

            double start = fromPoint.Equals(fromBackOffPoint) ? double.MinValue : Vector2.Distance(fromPoint, fromBackOffPoint);
            double end = toPoint.Equals(toBackOffPoint) ? double.MaxValue : Vector2.Distance(fromPoint, toBackOffPoint);

            var path = new PathGeometry();
            PathFigure pf = null;
            if (start == double.MinValue)
            {
                pf = new PathFigure()
                {
                    StartPoint = ToPoint(fromPoint)
                };
            }

            // the wavy bond is drawn using Bézier curves, removing the control points each
            // first 'endPoint' of the iteration forms a zig-zag pattern. The second 'endPoint'
            // lies on the central line between the atoms.

            // the following may help to visualise what we're doing,
            // s  = start (could be any end point)
            // e  = end point
            // cp = control points 1 and 2
            //
            //     cp2 e cp1                   cp2 e cp1
            //  cp1          cp2           cp1           cp2
            //  s ---------- e ----------- e ----------- e ------------ center line
            //               cp1           cp2           cp1
            //                   cp2 e cp1                   cp2 e
            //  |            |
            //  --------------
            //   one iteration
            //
            //  |     |
            //  -------
            //   one curveTo / 'step' distance

            // for the back off on atom symbols, the start position is the first end point after
            // the backed off point. Similarly, the curve is only drawn if the end point is
            // before the 'toBackOffPoint'

            for (int i = 1; i < nCurves; i += 2)
            {
                peak = Negate(peak); // alternate wave side

                // curving away from the center line
                {
                    double dist = i * step;

                    if (dist >= start && dist <= end)
                    {
                        // first end point
                        Vector2 endPoint = Sum(Sum(fromPoint, Scale(unit, dist)), peak);
                        if (pf != null)
                        {
                            var controlPoint1 = Sum(Sum(fromPoint, Scale(unit, (i - 1) * step)), Scale(peak, 0.5));
                            var controlPoint2 = Sum(Sum(fromPoint, Scale(unit, (i - 0.5) * step)), peak);
                            pf.Segments.Add(new BezierSegment(ToPoint(controlPoint1), ToPoint(controlPoint2), ToPoint(endPoint), true));
                        }
                        else
                        {
                            pf = new PathFigure()
                            {
                                StartPoint = ToPoint(endPoint)
                            };
                        }
                    }
                }

                // curving towards the center line
                {
                    double dist = (i + 1) * step;

                    if (dist >= start && dist <= end)
                    {
                        // second end point
                        var endPoint = ToPoint(Sum(fromPoint, Scale(unit, dist)));

                        if (pf != null)
                        {
                            var controlPoint1 = ToPoint(Sum(Sum(fromPoint, Scale(unit, (i + 0.5) * step)), peak));
                            var controlPoint2 = ToPoint(Sum(Sum(fromPoint, Scale(unit, dist)), Scale(peak, 0.5)));
                            pf.Segments.Add(new BezierSegment(controlPoint1, controlPoint2, endPoint, true));
                        }
                        else
                        {
                            pf = new PathFigure()
                            {
                                StartPoint = endPoint
                            };
                        }
                    }
                }
            }
            if (pf != null)
                path.Figures.Add(pf);

            return new GeneralPath(path, foreground).Outline(stroke);
        }

        /// <summary>
        /// Generates a double bond rendering element by deciding how best to display it.
        /// </summary>
        /// <param name="bond">the bond to render</param>
        /// <returns>rendering element</returns>
        private IRenderingElement GenerateDoubleBond(IBond bond, bool dashed)
        {
            bool cyclic = ringMap.ContainsKey(bond);

            // select offset bonds from either the preferred ring or the whole structure
            IAtomContainer refContainer = cyclic ? ringMap[bond] : container;

            int length = refContainer.Atoms.Count;
            int index1 = refContainer.Atoms.IndexOf(bond.Begin);
            int index2 = refContainer.Atoms.IndexOf(bond.End);

            // if the bond is in a cycle we are using ring bonds to determine offset, since rings
            // have been normalised and ordered to wind anti-clockwise we want to get the atoms
            // in the order they are in the ring.
            bool outOfOrder = cyclic && index1 == (index2 + 1) % length;

            IAtom atom1 = outOfOrder ? bond.End : bond.Begin;
            IAtom atom2 = outOfOrder ? bond.Begin : bond.End;

            if (BondStereo.EOrZ.Equals(bond.Stereo)) return GenerateCrossedDoubleBond(atom1, atom2);

            var atom1Bonds = refContainer.GetConnectedBonds(atom1).ToList();
            var atom2Bonds = refContainer.GetConnectedBonds(atom2).ToList();

            atom1Bonds.Remove(bond);
            atom2Bonds.Remove(bond);

            if (cyclic)
            {
                // get the winding relative to the ring
                int wind1 = Winding(atom1Bonds[0], bond);
                int wind2 = Winding(bond, atom2Bonds[0]);
                if (wind1 > 0)
                {
                    return GenerateOffsetDoubleBond(bond, atom1, atom2, atom1Bonds[0], atom2Bonds, dashed);
                }
                else if (wind2 > 0)
                {
                    return GenerateOffsetDoubleBond(bond, atom2, atom1, atom2Bonds[0], atom1Bonds, dashed);
                }
                else
                {
                    // special case, offset line is drawn on the opposite side for
                    // when concave in macro cycle
                    //
                    //           ---
                    //         a --- b
                    //        /       \
                    //    -- x         x --
                    return GenerateOffsetDoubleBond(bond, atom1, atom2, atom1Bonds[0], atom2Bonds, true, dashed);
                }
            }
            else if (atom1Bonds.Count == 1 && !HasDisplayedSymbol(atom1) && (!HasDisplayedSymbol(atom2) || !atom2Bonds.Any()))
            {
                return GenerateOffsetDoubleBond(bond, atom1, atom2, atom1Bonds[0], atom2Bonds, dashed);
            }
            else if (atom2Bonds.Count == 1 && !HasDisplayedSymbol(atom2) && (!HasDisplayedSymbol(atom1) || !atom1Bonds.Any()))
            {
                return GenerateOffsetDoubleBond(bond, atom2, atom1, atom2Bonds[0], atom1Bonds, dashed);
            }
            else if (SpecialOffsetBondNextToWedge(atom1, atom1Bonds) && !HasDisplayedSymbol(atom1))
            {
                return GenerateOffsetDoubleBond(bond, atom1, atom2, SelectPlainSingleBond(atom1Bonds), atom2Bonds, dashed);
            }
            else if (SpecialOffsetBondNextToWedge(atom2, atom2Bonds) && !HasDisplayedSymbol(atom2))
            {
                return GenerateOffsetDoubleBond(bond, atom2, atom1, SelectPlainSingleBond(atom2Bonds), atom1Bonds, dashed);
            }
            else
            {
                if (dashed)
                {
                    return GenerateDashedBond(atom1, atom2);
                }
                else
                {
                    return GenerateCenteredDoubleBond(bond, atom1, atom2, atom1Bonds, atom2Bonds);
                }
            }
        }

        /// <summary>
        /// Special condition for drawing offset bonds. If the double bond is adjacent to two bonds
        /// and one of those bonds is wedge (with this atom at the wide end) and the other is plain
        /// single bond, we can improve aesthetics by offsetting the double bond.
        /// </summary>
        /// <param name="atom">an atom</param>
        /// <param name="bonds">bonds connected to 'atom'</param>
        /// <returns>special case</returns>
        private bool SpecialOffsetBondNextToWedge(IAtom atom, List<IBond> bonds)
        {
            if (bonds.Count != 2) return false;
            if (AtWideEndOfWedge(atom, bonds[0]) && IsPlainBond(bonds[1])) return true;
            if (AtWideEndOfWedge(atom, bonds[1]) && IsPlainBond(bonds[0])) return true;
            return false;
        }

        /// <summary>
        /// Select a plain bond from a list of bonds. If no bond was found, the first
        /// is returned.
        /// </summary>
        /// <param name="bonds">list of bonds</param>
        /// <returns>a plain bond</returns>
        /// <seealso cref="IsPlainBond(IBond)"/>
        private IBond SelectPlainSingleBond(List<IBond> bonds)
        {
            foreach (var bond in bonds)
            {
                if (IsPlainBond(bond))
                    return bond;
            }
            return bonds[0];
        }

        /// <summary>
        /// A plain bond is a single bond with no stereochemistry type.
        /// </summary>
        /// <param name="bond">the bond to check</param>
        /// <returns>the bond is plain</returns>
        private static bool IsPlainBond(IBond bond)
        {
            return BondOrder.Single.Equals(bond.Order) && (bond.Stereo == BondStereo.None);
        }

        /// <summary>
        /// Check if the provided bond is a wedge (bold or hashed) and whether the atom is at the wide
        /// end.
        /// </summary>
        /// <param name="atom">atom to check</param>
        /// <param name="bond">bond to check</param>
        /// <returns>the atom is at the wide end of the wedge in the provided bond</returns>
        private bool AtWideEndOfWedge(IAtom atom, IBond bond)
        {
            if (bond.Stereo == BondStereo.None) return false;
            switch (bond.Stereo)
            {
                case BondStereo.Up:
                    return bond.End == atom;
                case BondStereo.UpInverted:
                    return bond.Begin == atom;
                case BondStereo.Down:
                    return bond.End == atom;
                case BondStereo.DownInverted:
                    return bond.Begin == atom;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Displays an offset double bond as per the IUPAC recommendation (GR-1.10) <token>cdk-cite-Brecher08</token>. 
        /// An offset bond has one line drawn between the two atoms and other draw to one
        /// side. The side is determined by the 'atom1Bond' parameter. The first atom should not have a
        /// displayed symbol.
        /// </summary>
        /// <param name="atom1">first atom</param>
        /// <param name="atom2">second atom</param>
        /// <param name="atom1Bond">the reference bond used to decide which side the bond is offset</param>
        /// <param name="atom2Bonds">the bonds connected to atom 2</param>
        /// <returns>the rendered bond element</returns>
        private IRenderingElement GenerateOffsetDoubleBond(IBond bond, IAtom atom1, IAtom atom2, IBond atom1Bond, List<IBond> atom2Bonds, bool dashed)
        {
            return GenerateOffsetDoubleBond(bond, atom1, atom2, atom1Bond, atom2Bonds, false, dashed);
        }

        /// <summary>
        /// Displays an offset double bond as per the IUPAC recomendation (GR-1.10) <token>cdk-cite-Brecher08</token>.
        /// An offset bond has one line drawn between the two atoms and other draw to one
        /// side. The side is determined by the 'atom1Bond' parameter. The first atom should not have a
        /// displayed symbol.
        /// </summary>
        /// <param name="atom1">first atom</param>
        /// <param name="atom2">second atom</param>
        /// <param name="atom1Bond">the reference bond used to decide which side the bond is offset</param>
        /// <param name="atom2Bonds">the bonds connected to atom 2</param>
        /// <param name="invert">invert the offset (i.e. opposite to reference bond)</param>
        /// <returns>the rendered bond element</returns>
        private IRenderingElement GenerateOffsetDoubleBond(IBond bond, IAtom atom1, IAtom atom2, IBond atom1Bond, List<IBond> atom2Bonds, bool invert, bool dashed)
        {
            if (HasDisplayedSymbol(atom1))
            {

            }
            Debug.Assert(atom1Bond != null);

            Vector2 atom1Point = atom1.Point2D.Value;
            Vector2 atom2Point = atom2.Point2D.Value;

            Vector2 atom1BackOffPoint = BackOffPoint(atom1, atom2);
            Vector2 atom2BackOffPoint = BackOffPoint(atom2, atom1);

            Vector2 unit = NewUnitVector(atom1Point, atom2Point);
            Vector2 perpendicular = NewPerpendicularVector(unit);

            Vector2 reference = NewUnitVector(atom1.Point2D.Value, atom1Bond.GetOther(atom1).Point2D.Value);

            // there are two perpendicular vectors, this check ensures we have one on the same side as
            // the reference
            if (Vector2.Dot(reference, perpendicular) < 0) perpendicular = Negate(perpendicular);
            // caller requested inverted drawing
            if (invert) perpendicular = Negate(perpendicular);

            // when the symbol is terminal, we move it such that it is between the two lines
            if (!atom2Bonds.Any() && HasDisplayedSymbol(atom2))
            {
                int atom2index = atomIndexMap[atom2];
                Vector2 nudge = Scale(perpendicular, separation / 2);
                symbols[atom2index] = symbols[atom2index].Translate(nudge.X, nudge.Y);
            }

            // the offset line isn't drawn the full length and is backed off more depending on the
            // angle of adjacent bonds, see GR-1.10 in the IUPAC recommendations
            double atom1Offset = 0;
            double atom2Offset = 0;

            if (dashed || !HasDisplayedSymbol(atom1))
            {
                atom1Offset = AdjacentLength(Sum(reference, unit), perpendicular, separation);
            }

            // reference bond may be on the other side (invert specified) - the offset needs negating
            if (Vector2.Dot(reference, perpendicular) < 0) atom1Offset = -atom1Offset;

            // the second atom may have zero or more bonds which we can use to get the offset
            // we find the one which is closest to the perpendicular vector
            if (atom2Bonds.Any() && (dashed || !HasDisplayedSymbol(atom2)))
            {
                Vector2 closest = GetNearestVector(perpendicular, atom2, atom2Bonds);
                atom2Offset = AdjacentLength(Sum(closest, Negate(unit)), perpendicular, separation);

                // closest bond may still be on the other side, if so the offset needs
                // negating
                if (Vector2.Dot(closest, perpendicular) < 0) atom2Offset = -atom2Offset;
            }

            double halfBondLength = Vector2.Distance(atom1Point, atom2BackOffPoint) / 2;
            if (atom1Offset > halfBondLength || atom1Offset < 0) atom1Offset = 0;
            if (atom2Offset > halfBondLength || atom2Offset < 0) atom2Offset = 0;

            ElementGroup group = new ElementGroup
            {
                NewLineElement(atom1BackOffPoint, atom2BackOffPoint)
            };
            if (dashed)
            {
                Vector2 beg = atom1Point + perpendicular * separation;
                Vector2 end = atom2Point + perpendicular * separation;
                group.Add(GenerateDashedBond(beg, end,
                                             atom1Offset,
                                             Vector2.Distance(beg, end) - atom2Offset));
            }
            else
            {
                Vector2 beg = atom1BackOffPoint + perpendicular * separation;
                Vector2 end = atom2BackOffPoint + perpendicular * separation;
                group.Add(NewLineElement(beg + unit * atom1Offset,
                                         end + unit * -atom2Offset));
            }

            // add annotation label on the opposite side
            string label = StandardGenerator.GetAnnotationLabel(bond);
            if (label != null) AddAnnotation(atom1, atom2, label, VecmathUtil.Negate(perpendicular));

            return group;
        }

        /// <summary>
        /// Generates a centered double bond. Here the lines are depicted each side and equidistant from
        /// the line that travel through the two atoms.
        /// </summary>
        /// <param name="atom1">an atom</param>
        /// <param name="atom2">the other atom</param>
        /// <param name="atom1Bonds">bonds to the first atom (excluding that being rendered)</param>
        /// <param name="atom2Bonds">bonds to the second atom (excluding that being rendered)</param>
        /// <returns>the rendering element</returns>
        private IRenderingElement GenerateCenteredDoubleBond(IBond bond, IAtom atom1, IAtom atom2, List<IBond> atom1Bonds, List<IBond> atom2Bonds)
        {
            Vector2 atom1BackOffPoint = BackOffPoint(atom1, atom2);
            Vector2 atom2BackOffPoint = BackOffPoint(atom2, atom1);

            Vector2 unit = NewUnitVector(atom1BackOffPoint, atom2BackOffPoint);
            Vector2 perpendicular1 = NewPerpendicularVector(unit);
            Vector2 perpendicular2 = Negate(perpendicular1);

            double halfBondLength = Vector2.Distance(atom1BackOffPoint, atom2BackOffPoint) / 2;
            double halfSeparation = separation / 2;

            ElementGroup group = new ElementGroup();

            Vector2 line1Atom1Point = Sum(atom1BackOffPoint, Scale(perpendicular1, halfSeparation));
            Vector2 line1Atom2Point = Sum(atom2BackOffPoint, Scale(perpendicular1, halfSeparation));
            Vector2 line2Atom1Point = Sum(atom1BackOffPoint, Scale(perpendicular2, halfSeparation));
            Vector2 line2Atom2Point = Sum(atom2BackOffPoint, Scale(perpendicular2, halfSeparation));

            // adjust atom 1 lines to be flush with adjacent bonds
            if (!HasDisplayedSymbol(atom1) && atom1Bonds.Count > 1)
            {
                Vector2 nearest1 = GetNearestVector(perpendicular1, atom1, atom1Bonds);
                Vector2 nearest2 = GetNearestVector(perpendicular2, atom1, atom1Bonds);

                double line1Adjust = AdjacentLength(nearest1, perpendicular1, halfSeparation);
                double line2Adjust = AdjacentLength(nearest2, perpendicular2, halfSeparation);

                // don't adjust beyond half the bond length
                if (line1Adjust > halfBondLength || line1Adjust < 0) line1Adjust = 0;
                if (line2Adjust > halfBondLength || line2Adjust < 0) line2Adjust = 0;

                // corner case when the adjacent bonds are acute to the double bond,
                if (Vector2.Dot(nearest1, unit) > 0) line1Adjust = -line1Adjust;
                if (Vector2.Dot(nearest2, unit) > 0) line2Adjust = -line2Adjust;

                line1Atom1Point = Sum(line1Atom1Point, Scale(unit, -line1Adjust));
                line2Atom1Point = Sum(line2Atom1Point, Scale(unit, -line2Adjust));
            }

            // adjust atom 2 lines to be flush with adjacent bonds
            if (!HasDisplayedSymbol(atom2) && atom2Bonds.Count > 1)
            {
                Vector2 nearest1 = GetNearestVector(perpendicular1, atom2, atom2Bonds);
                Vector2 nearest2 = GetNearestVector(perpendicular2, atom2, atom2Bonds);

                double line1Adjust = AdjacentLength(nearest1, perpendicular1, halfSeparation);
                double line2Adjust = AdjacentLength(nearest2, perpendicular2, halfSeparation);

                // don't adjust beyond half the bond length
                if (line1Adjust > halfBondLength || line1Adjust < 0) line1Adjust = 0;
                if (line2Adjust > halfBondLength || line2Adjust < 0) line2Adjust = 0;

                // corner case when the adjacent bonds are acute to the double bond
                if (Vector2.Dot(nearest1, unit) < 0) line1Adjust = -line1Adjust;
                if (Vector2.Dot(nearest2, unit) < 0) line2Adjust = -line2Adjust;

                line1Atom2Point = Sum(line1Atom2Point, Scale(unit, line1Adjust));
                line2Atom2Point = Sum(line2Atom2Point, Scale(unit, line2Adjust));
            }

            group.Add(NewLineElement(line1Atom1Point, line1Atom2Point));
            group.Add(NewLineElement(line2Atom1Point, line2Atom2Point));

            // add annotation label
            string label = StandardGenerator.GetAnnotationLabel(bond);
            if (label != null) AddAnnotation(atom1, atom2, label);

            return group;
        }

        /// <summary>
        /// The crossed bond defines unknown geometric isomerism on a double bond. The cross is
        /// displayed for <see cref="BondStereo.EOrZ"/>.
        /// </summary>
        /// <param name="from">drawn from this atom</param>
        /// <param name="to">drawn to this atom</param>
        /// <returns>generated rendering element</returns>
        private IRenderingElement GenerateCrossedDoubleBond(IAtom from, IAtom to)
        {
            Vector2 atom1BackOffPoint = BackOffPoint(from, to);
            Vector2 atom2BackOffPoint = BackOffPoint(to, from);

            Vector2 unit = NewUnitVector(atom1BackOffPoint, atom2BackOffPoint);
            Vector2 perpendicular1 = NewPerpendicularVector(unit);
            Vector2 perpendicular2 = Negate(perpendicular1);

            double halfSeparation = separation / 2;

            // same as centered double bond, this could be improved by interpolating the points
            // during back off
            Vector2 line1Atom1Point = Sum(atom1BackOffPoint, Scale(perpendicular1, halfSeparation));
            Vector2 line1Atom2Point = Sum(atom2BackOffPoint, Scale(perpendicular1, halfSeparation));
            Vector2 line2Atom1Point = Sum(atom1BackOffPoint, Scale(perpendicular2, halfSeparation));
            Vector2 line2Atom2Point = Sum(atom2BackOffPoint, Scale(perpendicular2, halfSeparation));

            // swap end points to generate a cross
            ElementGroup group = new ElementGroup
            {
                NewLineElement(line1Atom1Point, line2Atom2Point),
                NewLineElement(line2Atom1Point, line1Atom2Point)
            };
            return group;
        }

        /// <summary>
        /// Generate a triple bond rendering, the triple is composed of a plain single bond and a
        /// centered double bond.
        /// </summary>
        /// <param name="atom1">an atom</param>
        /// <param name="atom2">the other atom</param>
        /// <returns>triple bond rendering element</returns>
        private IRenderingElement GenerateTripleBond(IBond bond, IAtom atom1, IAtom atom2)
        {
            ElementGroup group = new ElementGroup();

            var p1 = ToPoint(BackOffPoint(atom1, atom2));
            var p2 = ToPoint(BackOffPoint(atom2, atom1));

            var perp = NewPerpendicularVector(NewUnitVector(p1, p2));
            perp *= separation;

            group.Add(new LineElement(p1, p2, stroke, foreground));
            group.Add(new LineElement(p1 + perp, p2 + perp, stroke, foreground));
            group.Add(new LineElement(p1 - perp, p2 - perp, stroke, foreground));

            // add annotation label
            string label = StandardGenerator.GetAnnotationLabel(bond);
            if (label != null) AddAnnotation(atom1, atom2, label);

            return group;
        }

        /// <summary>
        /// Draws a crossing wavy line at the end of a bond to indicate a point of attachment.
        /// </summary>
        /// <param name="atom">atom that is an attachment point</param>
        /// <param name="bond">bond that is an attachment point</param>
        /// <returns>the rendering element</returns>
        private IRenderingElement GenerateAttachPoint(IAtom atom, IBond bond)
        {
            Vector2 mid = atom.Point2D.Value;
            Vector2 bndVec = VecmathUtil.NewUnitVector(atom, bond);
            Vector2 bndXVec = VecmathUtil.NewPerpendicularVector(bndVec);

            double length = Vector2.Distance(atom.Point2D.Value, bond.GetOther(atom).Point2D.Value);
            bndXVec *= length / 2;
            Vector2 beg = VecmathUtil.Sum(atom.Point2D.Value, bndXVec);
            bndXVec *= -1;
            Vector2 end = VecmathUtil.Sum(atom.Point2D.Value, bndXVec);

            // wavy line between beg and end, see generateWavyBond for explanation

            int nCurves = (int)(2 * Math.Ceiling(length / waveSpacing));
            double step = length / nCurves;

            bndXVec = Vector2.Normalize(bndXVec);
            Vector2 peak = Scale(bndVec, step);
            Vector2 unit = VecmathUtil.NewUnitVector(beg, end);

            var path = new PathGeometry();
            int halfNCurves = nCurves / 2;
            {
                // one half
                var pf = new PathFigure()
                {
                    StartPoint = ToPoint(mid),
                };
                for (int i = 1; i < halfNCurves; i += 2)
                {
                    peak = Vector2.Negate(peak); // alternate wave side

                    // curving away from the center line
                    {
                        double dist = i * step;
                        // first end point
                        var endPoint = ToPoint(mid + Scale(unit, dist) + peak);
                        var controlPoint1 = ToPoint(mid + Scale(unit, (i - 1) * step) + Scale(peak, 0.5));
                        var controlPoint2 = ToPoint(mid + Scale(unit, (i - 0.5) * step) + peak);
                        pf.Segments.Add(new BezierSegment(controlPoint1, controlPoint2, endPoint, true));
                    }

                    // curving towards the center line
                    {
                        double dist = (i + 1) * step;
                        // second end point
                        var endPoint = ToPoint(mid + Scale(unit, dist));
                        var controlPoint1 = ToPoint(mid + Scale(unit, (i + 0.5) * step) + peak);
                        var controlPoint2 = ToPoint(mid + Scale(unit, dist) + Scale(peak, 0.5));
                        pf.Segments.Add(new BezierSegment(controlPoint1, controlPoint2, endPoint, true));
                    }
                }
                path.Figures.Add(pf);
            }

            {
                // other half
                var pf = new PathFigure()
                {
                    StartPoint = ToPoint(mid)
                };
                unit = Vector2.Negate(unit);
                peak = Vector2.Negate(peak);
                for (int i = 1; i < halfNCurves; i += 2)
                {
                    peak = Vector2.Negate(peak); // alternate wave side

                    // curving away from the center line
                    {
                        double dist = i * step;
                        // first end point
                        var endPoint = ToPoint(mid + Scale(unit, dist) + peak);
                        var controlPoint1 = ToPoint(mid + Scale(unit, (i - 1) * step) + Scale(peak, 0.5));
                        var controlPoint2 = ToPoint(mid + Scale(unit, (i - 0.5) * step) + peak);
                        pf.Segments.Add(new BezierSegment(controlPoint1, controlPoint2, endPoint, true));
                    }

                    // curving towards the center line
                    {
                        double dist = (i + 1) * step;
                        // second end point
                        var endPoint = ToPoint(mid + Scale(unit, dist));
                        var controlPoint1 = ToPoint(mid + Scale(unit, (i + 0.5) * step) + peak);
                        var controlPoint2 = ToPoint(mid + Scale(unit, dist) + Scale(peak, 0.5));
                        pf.Segments.Add(new BezierSegment(controlPoint1, controlPoint2, endPoint, true));
                    }
                    path.Figures.Add(pf);
                }
            }

            return new GeneralPath(path, foreground).Outline(stroke);
        }

        /// <summary>
        /// Determine if an atom is an attach point.
        /// </summary>
        /// <param name="atom">potential attach point atom</param>
        /// <returns>the atom is an attachment point</returns>
        private bool IsAttachPoint(IAtom atom)
        {
            return atom is IPseudoAtom && ((IPseudoAtom)atom).AttachPointNum > 0;
        }

        /// <summary>
        /// Add an annotation label for the bond between the two atoms. The side of the bond that
        /// is chosen is arbitrary.
        /// </summary>
        /// <param name="atom1">first atom</param>
        /// <param name="atom2">second atom</param>
        /// <param name="label">annotation label</param>
        /// <seealso cref="AddAnnotation(IAtom, IAtom, string, Vector2)"/>
        private void AddAnnotation(IAtom atom1, IAtom atom2, string label)
        {
            Vector2 perpendicular = VecmathUtil.NewPerpendicularVector(VecmathUtil.NewUnitVector(atom1.Point2D.Value, atom2.Point2D.Value));
            AddAnnotation(atom1, atom2, label, perpendicular);
        }

        /// <summary>
        /// Add an annotation label for the bond between the two atoms on the specified 'side' (provided
        /// as a the perpendicular directional vector).
        /// </summary>
        /// <param name="atom1">first atom</param>
        /// <param name="atom2">second atom</param>
        /// <param name="label">annotation label</param>
        /// <param name="perpendicular">the vector along which to place the annotation (starting from the midpoint)</param>
        private void AddAnnotation(IAtom atom1, IAtom atom2, string label, Vector2 perpendicular)
        {
            Vector2 midPoint = VecmathUtil.Midpoint(atom1.Point2D.Value, atom2.Point2D.Value);

            TextOutline outline = StandardGenerator.GenerateAnnotation(midPoint, label, perpendicular, annotationDistance, annotationScale, font, emSize, null);
            annotations.Add(MarkedElement.Markup(GeneralPath.ShapeOf(outline.GetOutline(), annotationColor), "annotation"));
        }

        /// <summary>
        /// Generates a rendering element for displaying an 'unknown' bond type.
        /// </summary>
        /// <param name="fromPoint">drawn from this point</param>
        /// <param name="toPoint">drawn to this point</param>
        /// <param name="start">only start drawing dashes after this point</param>
        /// <param name="end">stop drawing dashes after this point</param>
        /// <returns>rendering of unknown bond</returns>
        internal IRenderingElement GenerateDashedBond(Vector2 fromPoint, Vector2 toPoint, double start, double end)
        {
            Vector2 unit = NewUnitVector(fromPoint, toPoint);

            int nDashes = parameters.GetDashSection();

            double step = Vector2.Distance(fromPoint, toPoint) / ((3 * nDashes) - 2);

            ElementGroup group = new ElementGroup();

            double distance = 0;

            for (int i = 0; i < nDashes; i++)
            {

                // draw a full dash section
                if (distance > start && distance + step < end)
                {
                    group.Add(NewLineElement(fromPoint * Scale(unit, distance),
                            Sum(fromPoint, Scale(unit, distance + step))));
                }
                // draw a dash section that starts late
                else if (distance + step > start && distance + step < end)
                {
                    group.Add(NewLineElement(Sum(fromPoint, Scale(unit, start)),
                            Sum(fromPoint, Scale(unit, distance + step))));
                }
                // draw a dash section that stops early
                else if (distance > start && distance < end)
                {
                    group.Add(NewLineElement(Sum(fromPoint, Scale(unit, distance)), Sum(fromPoint, Scale(unit, end))));
                }

                distance += step;
                distance += step; // the gap
                distance += step; // the gap
            }

            return group;
        }

        IRenderingElement GenerateDashedBond(IAtom from, IAtom to)
        {
            Vector2 fromPoint = from.Point2D.Value;
            Vector2 toPoint = to.Point2D.Value;
            double start = HasDisplayedSymbol(from) ? Vector2.Distance(fromPoint, BackOffPoint(from, to)) : double.NegativeInfinity;
            double end = HasDisplayedSymbol(to) ? Vector2.Distance(fromPoint, BackOffPoint(to, from)) : double.PositiveInfinity;
            return GenerateDashedBond(fromPoint, toPoint, start, end);
        }

        /// <summary>
        /// Create a new line element between two points. The line has the specified stroke and
        /// foreground color.
        /// </summary>
        /// <param name="a">start of the line</param>
        /// <param name="b">end of the line</param>
        /// <returns>line rendering element</returns>
        internal IRenderingElement NewLineElement(Vector2 a, Vector2 b)
        {
            return new LineElement(ToPoint(a), ToPoint(b), stroke, foreground);
        }

        /// <summary>
        /// Determine the backed off (start) point of the 'from' atom for the line between 'from' and
        /// 'to'.
        /// </summary>
        /// <param name="from">start atom</param>
        /// <param name="to">end atom</param>
        /// <returns>the backed off point of 'from' atom</returns>
        internal Vector2 BackOffPoint(IAtom from, IAtom to)
        {
            return BackOffPointOf(symbols[atomIndexMap[from]], from.Point2D.Value, to.Point2D.Value, backOff);
        }

        /// <summary>
        /// Check if an atom has a displayed symbol.
        /// </summary>
        /// <param name="atom">the atom to check</param>
        /// <returns>the atom has a displayed symbol</returns>
        internal bool HasDisplayedSymbol(IAtom atom)
        {
            return symbols[atomIndexMap[atom]] != null;
        }

        /// <summary>
        /// Determine the backed off (start) point of the 'from' atom for the line between 'from' and
        /// 'to' given the symbol present at the 'from' point and the back off amount.
        /// </summary>
        /// <param name="symbol">the symbol present at the 'fromPoint' atom, may be null</param>
        /// <param name="fromPoint">the location of the from atom</param>
        /// <param name="toPoint">the location of the to atom</param>
        /// <param name="backOff">the amount to back off from the symbol</param>
        /// <returns>the backed off (start) from point</returns>
        internal static Vector2 BackOffPointOf(AtomSymbol symbol, Vector2 fromPoint, Vector2 toPoint, double backOff)
        {
            // no symbol
            if (symbol == null)
                return fromPoint;

            Vector2 intersect = ToVector(symbol.GetConvexHull().Intersect(ToPoint(fromPoint), ToPoint(toPoint)));

            // does not intersect
            if (intersect == null)
                return fromPoint;

            // move the point away from the intersect by the desired back off amount
            Vector2 unit = NewUnitVector(fromPoint, toPoint);
            return intersect + Scale(unit, backOff);
        }

        /// <summary>
        /// Determine the winding of two bonds. The winding is > 0 for anti clockwise and &lt; 0
        /// for clockwise and is relative to bond 1.
        /// </summary>
        /// <param name="bond1">first bond</param>
        /// <param name="bond2">second bond</param>
        /// <returns>winding relative to bond</returns>
        /// <exception cref="ArgumentException">bonds share no atoms</exception>
        internal static int Winding(IBond bond1, IBond bond2)
        {
            IAtom atom1 = bond1.Atoms[0];
            IAtom atom2 = bond1.Atoms[1];
            if (bond2.Contains(atom1))
            {
                return Winding(atom2.Point2D.Value, atom1.Point2D.Value, bond2.GetOther(atom1).Point2D.Value);
            }
            else if (bond2.Contains(atom2))
            {
                return Winding(atom1.Point2D.Value, atom2.Point2D.Value, bond2.GetOther(atom2).Point2D.Value);
            }
            else
            {
                throw new ArgumentException("Bonds do not share any atoms");
            }
        }

        /// <summary>
        /// Creates a mapping of bonds to preferred rings (stored as IAtomContainers).
        /// </summary>
        /// <param name="container">structure representation</param>
        /// <returns>bond to ring map</returns>
        internal static IDictionary<IBond, IAtomContainer> RingPreferenceMap(IAtomContainer container)
        {
            IRingSet relevantRings = Cycles.FindSSSR(container).ToRingSet();
            var rings = AtomContainerSetManipulator.GetAllAtomContainers(relevantRings).ToList();

            rings.Sort(new RingBondOffsetComparator());

            var ringMap = new Dictionary<IBond, IAtomContainer>();

            // index bond -> ring based on the first encountered bond
            foreach (var ring in rings)
            {
                NormalizeRingWinding(ring);
                foreach (var bond in ring.Bonds)
                {
                    if (ringMap.ContainsKey(bond)) continue;
                    ringMap[bond] = ring;
                }
            }

            return new ReadOnlyDictionary<IBond, IAtomContainer>(ringMap);
        }

        /// <summary>
        /// Normalise the ring ordering in a ring such that the overall winding is anti clockwise.
        /// The normalisation exploits the fact that (most) rings will be drawn with more convex
        /// turns (i.e. close to 30 degrees). This not bullet proof, consider a hexagon drawn as
        /// a three point star.
        /// </summary>
        /// <param name="container">the ring to normalize</param>
        internal static void NormalizeRingWinding(IAtomContainer container)
        {
            int prev = container.Atoms.Count - 1;
            int curr = 0;
            int next = 1;

            int n = container.Atoms.Count;

            int winding = 0;

            while (curr < n)
            {
                winding += Winding(container.Atoms[prev].Point2D.Value, container.Atoms[curr].Point2D.Value, container.Atoms[next % n].Point2D.Value);
                prev = curr;
                curr = next;
                next = next + 1;
            }

            if (winding < 0)
            {
                IAtom[] atoms = new IAtom[n];
                for (int i = 0; i < n; i++)
                    atoms[n - i - 1] = container.Atoms[i];
                container.Atoms.Clear();
                foreach (var atom in atoms)
                    container.Atoms.Add(atom);
            }
        }

        /// <summary>
        /// Determine the winding of three points using the determinant.
        /// </summary>
        /// <param name="a">first point</param>
        /// <param name="b">second point</param>
        /// <param name="c">third point</param>
        /// <returns>&lt; 0 = clockwise, 0 = linear, &gt; 0 anti-clockwise</returns>
        internal static int Winding(Vector2 a, Vector2 b, Vector2 c)
        {
            return (int)Math.Sign((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X));
        }

        /// <summary>
        /// Order rings by preference of double bond offset. Rings that appear first have preference of
        /// the double bond.
        /// <list type="number">
        /// <item>rings of size 6, 5, 7, 4, 3 are preferred (in that order)</item>
        /// <item>rings with more double bonds are preferred</item>
        /// <item>rings with a higher carbon count are preferred</item>
        /// </list>
        /// </summary>
        internal sealed class RingBondOffsetComparator : IComparer<IAtomContainer>
        {
            private static readonly int[] PREFERENCE_INDEX = new int[8];

            static RingBondOffsetComparator()
            {
                int preference = 0;
                foreach (var size in new int[] { 6, 5, 7, 4, 3 })
                {
                    PREFERENCE_INDEX[size] = preference++;
                }
            }

            /// <summary>
            /// Create a new comparator.
            /// </summary>
            public RingBondOffsetComparator()
            { }

            private static readonly Config.ChemicalElement[] preferedElements = new []
            {
                ChemicalElements.Carbon,
                ChemicalElements.Nitrogen,
                ChemicalElements.Oxygen,
                ChemicalElements.Sulfur,
                ChemicalElements.Phosphorus
            };

            /// <inheritdoc/>
            public int Compare(IAtomContainer containerA, IAtomContainer containerB)
            {
                // first order by size
                int sizeCmp = Ints.Compare(SizePreference(containerA.Atoms.Count),
                        SizePreference(containerB.Atoms.Count));
                if (sizeCmp != 0) return sizeCmp;

                // now order by number of double bonds
                int piBondCmp = Ints.Compare(CountNumberOfDoubleBonds(containerA), CountNumberOfDoubleBonds(containerB));
                if (piBondCmp != 0) return -piBondCmp;

                // order by element frequencies, all carbon rings are preferred
                int[] freqA = CountLightElements(containerA);
                int[] freqB = CountLightElements(containerB);

                foreach (var element in preferedElements)
                {
                    int elemCmp = Ints.Compare(freqA[element.AtomicNumber], freqB[element.AtomicNumber]);
                    if (elemCmp != 0) return -elemCmp;
                }

                return 0;
            }

            /// <summary>
            /// Convert an absolute size value into the size preference.
            /// </summary>
            /// <param name="size">number of atoms or bonds in a ring</param>
            /// <returns>size preference</returns>
            internal static int SizePreference(int size)
            {
                if (size < 3) throw new ArgumentException("a ring must have at least 3 atoms");
                if (size > 7) return size;
                return PREFERENCE_INDEX[size];
            }

            /// <summary>
            /// Count the number of double bonds in a container.
            /// </summary>
            /// <param name="container">structure representation</param>
            /// <returns>number of double bonds</returns>
            internal static int CountNumberOfDoubleBonds(IAtomContainer container)
            {
                int count = 0;
                foreach (var bond in container.Bonds)
                    if (BondOrder.Double.Equals(bond.Order)) count++;
                return count;
            }

            /// <summary>
            /// Count the light elements (atomic number &lt; 19) in an atom container. The count is provided
            /// as a frequency vector indexed by atomic number.
            /// </summary>
            /// <param name="container">structure representation</param>
            /// <returns>frequency vector of atomic numbers 0-18</returns>
            internal static int[] CountLightElements(IAtomContainer container)
            {
                // count elements up to Argon (number=18)
                int[] freq = new int[19];
                foreach (var atom in container.Atoms)
                {
                    if (atom.AtomicNumber >= 0 && atom.AtomicNumber < 19)
                        freq[atom.AtomicNumber.Value]++;
                }
                return freq;
            }
        }
    }
}
