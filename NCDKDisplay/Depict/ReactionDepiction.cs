/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
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

using NCDK.Renderers;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Generators;
using NCDK.Renderers.Visitors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NCDK.Depict
{
    /// <summary>
    /// Internal - depiction of a single reaction. We divide the reaction into two draw steps.
    /// The first step draws the main components (reactants and products) whilst the second
    /// draws the side components (agents: catalysts, solvents, spectators, etc). Reaction
    /// direction is drawn a single headed arrow (forward and backward) or an equilibrium
    /// (bidirectional).
    /// </summary>
    sealed class ReactionDepiction : Depiction
    {

        private readonly RendererModel model;
        private readonly Dimensions dimensions;

        // molecule sets and titles
        private readonly List<Bounds> mainComp = new List<Bounds>();
        private readonly List<Bounds> sideComps = new List<Bounds>();
        private readonly Bounds conditions;
        private readonly Bounds title;

        // arrow info
        private readonly int arrowIdx;
        private readonly ReactionDirections direction;
        private readonly double arrowHeight;
        private readonly double minArrowWidth;

        // dimensions and spacing of side components
        private readonly Dimensions sideDim;
        private readonly Dimensions mainDim;
        private readonly Dimensions condDim;

        private readonly double[] xOffsets, yOffsets;
        private readonly double[] xOffsetSide, yOffsetSide;

        private readonly int nRow, nCol;

        private readonly Color fgcol;

        public ReactionDepiction(RendererModel model,
                                 List<Bounds> reactants,
                                 List<Bounds> products,
                                 List<Bounds> agents,
                                 Bounds plus,
          ReactionDirections direction,
                                 Dimensions dimensions,
                                 IList<Bounds> reactantTitles,
                                 IList<Bounds> productTitles,
                                 Bounds title,
                                 Bounds conditions,
                                 Color fgcol)
            : base(model)
        {
            this.model = model;
            this.dimensions = dimensions;
            this.title = title;
            this.fgcol = fgcol;


            // side components (catalysts, solvents, etc) note we deliberately
            // swap sideGrid width and height as we to stack agents on top of
            // each other. By default determineGrid tries to make the grid
            // wide but we want it tall
            this.sideComps.AddRange(agents);
            Dimension sideGrid = Dimensions.DetermineGrid(sideComps.Count);
            Dimensions prelimSideDim = Dimensions.OfGrid(sideComps,
                                                         yOffsetSide = new double[sideGrid.Width + 1],
                                                         xOffsetSide = new double[sideGrid.Height + 1]);

            // build the main components, we add a 'plus' between each molecule
            foreach (var reactant in reactants)
            {
                this.mainComp.Add(reactant);
                this.mainComp.Add(plus);
            }

            // replacing trailing plus with placeholder for arrow
            if (!reactants.Any())
                this.mainComp.Add(new Bounds());
            else
                this.mainComp[this.mainComp.Count - 1] = new Bounds();

            foreach (var product in products)
            {
                this.mainComp.Add(product);
                this.mainComp.Add(plus);
            }

            // trailing plus not needed
            if (products.Any())
                this.mainComp.RemoveAt(this.mainComp.Count - 1);

            // add title if supplied, we simply line them up with
            // the main components and the add them as an extra
            // row
            if (reactantTitles.Any() || productTitles.Any())
            {
                if (reactantTitles.Any() && reactantTitles.Count != reactants.Count)
                    throw new ArgumentException("Number of reactant titles differed from number of reactants");
                if (productTitles.Any() && productTitles.Count != products.Count)
                    throw new ArgumentException("Number of product titles differed from number of products");
                List<Bounds> mainTitles = new List<Bounds>();
                foreach (var reactantTitle in reactantTitles)
                {
                    mainTitles.Add(reactantTitle);
                    mainTitles.Add(new Bounds());
                }
                if (!reactants.Any())
                    mainTitles.Add(new Bounds()); // gap for arrow
                foreach (var productTitle in productTitles)
                {
                    mainTitles.Add(productTitle);
                    mainTitles.Add(new Bounds());
                }
                // remove trailing space for plus
                if (products.Any())
                    mainTitles.RemoveAt(mainTitles.Count - 1);

                Trace.Assert(mainTitles.Count == mainComp.Count);
                this.mainComp.AddRange(mainTitles);
                this.nRow = 2;
                this.nCol = mainComp.Count / 2;
            }
            else
            {
                this.nRow = 1;
                this.nCol = mainComp.Count;
            }

            this.conditions = conditions;

            // arrow params
            this.arrowIdx = Math.Max(reactants.Count + reactants.Count - 1, 0);
            this.direction = direction;
            this.arrowHeight = plus.Height;
            this.minArrowWidth = 4 * arrowHeight;

            mainDim = Dimensions.OfGrid(mainComp,
                                        yOffsets = new double[nRow + 1],
                                        xOffsets = new double[nCol + 1]);

            double middleRequired = Math.Max(prelimSideDim.w, conditions.Width);

            // avoid v. small arrows, we take in to account the padding provided by the arrow head height/length
            if (middleRequired < minArrowWidth - arrowHeight - arrowHeight)
            {
                // adjust x-offset so side components are centered
                double xAdjust = (minArrowWidth - middleRequired) / 2;
                for (int i = 0; i < xOffsetSide.Length; i++)
                    xOffsetSide[i] += xAdjust;
                // need to recenter agents
                if (conditions.Width > prelimSideDim.w)
                {
                    for (int i = 0; i < xOffsetSide.Length; i++)
                        xOffsetSide[i] += (conditions.Width - prelimSideDim.w) / 2;
                }
                // update side dims
                this.sideDim = new Dimensions(minArrowWidth, prelimSideDim.h);
                this.condDim = new Dimensions(minArrowWidth, conditions.Height);
            }
            else
            {
                // arrow padding
                for (int i = 0; i < xOffsetSide.Length; i++)
                    xOffsetSide[i] += arrowHeight;

                // need to recenter agents
                if (conditions.Width > prelimSideDim.w)
                {
                    for (int i = 0; i < xOffsetSide.Length; i++)
                        xOffsetSide[i] += (conditions.Width - prelimSideDim.w) / 2;
                }

                this.sideDim = new Dimensions(2 * arrowHeight + middleRequired,
                                              prelimSideDim.h);
                this.condDim = new Dimensions(2 * arrowHeight + middleRequired,
                                              conditions.Height);
            }
        }

        public override RenderTargetBitmap ToImg()
        {

            // format margins and padding for raster images
            double scale = model.GetV<double>(typeof(BasicSceneGenerator.Scale));
            double zoom = model.GetV<double>(typeof(BasicSceneGenerator.ZoomFactor));
            double margin = GetMarginValue(DepictionGenerator.DEFAULT_PX_MARGIN);
            double padding = GetPaddingValue(DEFAULT_PADDING_FACTOR * margin);

            // work out the required space of the main and side components separately
            // will draw these in two passes (main then side) hence want different offsets for each
            int nSideCol = xOffsetSide.Length - 1;
            int nSideRow = yOffsetSide.Length - 1;

            Dimensions sideRequired = sideDim.Scale(scale * zoom);
            Dimensions mainRequired = mainDim.Scale(scale * zoom);
            Dimensions condRequired = condDim.Scale(scale * zoom);

            Dimensions titleRequired = new Dimensions(title.Width, title.Height).Scale(scale * zoom);

            double firstRowHeight = scale * zoom * yOffsets[1];
            Dimensions total = CalcTotalDimensions(margin, padding, mainRequired, sideRequired, titleRequired, firstRowHeight, null);
            double fitting = CalcFitting(margin, padding, mainRequired, sideRequired, titleRequired, firstRowHeight, null);

            // create the image for rendering
            var img = new RenderTargetBitmap((int)Math.Ceiling(total.w), (int)Math.Ceiling(total.h), 96, 96, PixelFormats.Pbgra32);

            // we use the AWT for vector graphics if though we're raster because
            // fractional strokes can be figured out by interpolation, without
            // when we shrink diagrams bonds can look too bold/chubby
            var dv = new DrawingVisual();
            using (var g2 = dv.RenderOpen())
            {
                IDrawVisitor visitor = WPFDrawVisitor.ForVectorGraphics(g2);
                visitor.SetTransform(new ScaleTransform(1, -1));
                visitor.Visit(new RectangleElement(new Point(0, -total.h), total.w, total.h, true, model.GetV<Color>(typeof(BasicSceneGenerator.BackgroundColor))));

                // compound the zoom, fitting and scaling into a single value
                double rescale = zoom * fitting * scale;
                double mainCompOffset = 0;

                // shift product x-offset to make room for the arrow / side components
                mainCompOffset = fitting * sideRequired.h + nSideRow * padding - fitting * firstRowHeight / 2;
                for (int i = arrowIdx + 1; i < xOffsets.Length; i++)
                {
                    xOffsets[i] += sideRequired.w * 1 / (scale * zoom);
                }

                // MAIN COMPONENTS DRAW
                // x,y base coordinates include the margin and centering (only if fitting to a size)
                double totalRequiredWidth = 2 * margin + Math.Max(0, nCol - 1) * padding + Math.Max(0, nSideCol - 1) * padding + (rescale * xOffsets[nCol]);
                double totalRequiredHeight = 2 * margin + Math.Max(0, nRow - 1) * padding + (!title.IsEmpty() ? padding : 0) + Math.Max(mainCompOffset, 0) + fitting * mainRequired.h + fitting * Math.Max(0, titleRequired.h);
                double xBase = margin + (total.w - totalRequiredWidth) / 2;
                double yBase = margin + Math.Max(mainCompOffset, 0) + (total.h - totalRequiredHeight) / 2;
                for (int i = 0; i < mainComp.Count; i++)
                {

                    int row = i / nCol;
                    int col = i % nCol;

                    // calc the 'view' bounds:
                    //  amount of padding depends on which row or column we are in.
                    //  the width/height of this col/row can be determined by the next offset
                    double x = xBase + col * padding + rescale * xOffsets[col];
                    double y = yBase + row * padding + rescale * yOffsets[row];
                    double w = rescale * (xOffsets[col + 1] - xOffsets[col]);
                    double h = rescale * (yOffsets[row + 1] - yOffsets[row]);

                    // intercept arrow draw and make it as big as need
                    if (i == arrowIdx)
                    {
                        w = rescale * (xOffsets[i + 1] - xOffsets[i]) + Math.Max(0, nSideCol - 1) * padding;
                        Draw(visitor,
                             1, // no zoom since arrows is drawn as big as needed
                             CreateArrow(w, arrowHeight * rescale),
                             rect(x, y, w, h));
                        continue;
                    }

                    // extra padding from the side components
                    if (i > arrowIdx)
                        x += Math.Max(0, nSideCol - 1) * padding;

                    // skip empty elements
                    Bounds bounds = this.mainComp[i];
                    if (bounds.IsEmpty())
                        continue;

                    Draw(visitor, zoom, bounds, rect(x, y, w, h));
                }

                // RXN TITLE DRAW
                if (!title.IsEmpty())
                {
                    double y = yBase + nRow * padding + rescale * yOffsets[nRow];
                    double h = rescale * title.Height;
                    Draw(visitor, zoom, title, rect(0, y, total.w, h));
                }

                // SIDE COMPONENTS DRAW
                xBase += arrowIdx * padding + rescale * xOffsets[arrowIdx];
                yBase -= mainCompOffset;
                for (int i = 0; i < sideComps.Count; i++)
                {
                    int row = i / nSideCol;
                    int col = i % nSideCol;

                    // calc the 'view' bounds:
                    //  amount of padding depends on which row or column we are in.
                    //  the width/height of this col/row can be determined by the next offset
                    double x = xBase + col * padding + rescale * xOffsetSide[col];
                    double y = yBase + row * padding + rescale * yOffsetSide[row];
                    double w = rescale * (xOffsetSide[col + 1] - xOffsetSide[col]);
                    double h = rescale * (yOffsetSide[row + 1] - yOffsetSide[row]);

                    Draw(visitor, zoom, sideComps[i], rect(x, y, w, h));
                }

                // CONDITIONS DRAW
                if (!conditions.IsEmpty())
                {
                    yBase += mainCompOffset;        // back to top
                    yBase += (fitting * mainRequired.h) / 2;    // now on center line (arrow)
                    yBase += arrowHeight;           // now just bellow
                    Draw(visitor, zoom, conditions, rect(xBase,
                                                         yBase,
                                                         fitting * condRequired.w, fitting * condRequired.h));
                }

                // reset shared xOffsets
                if (sideComps.Any())
                {
                    for (int i = arrowIdx + 1; i < xOffsets.Length; i++)
                        xOffsets[i] -= sideRequired.w * 1 / (scale * zoom);
                }
                img.Render(dv);
                return img;
            }
        }

        internal override string ToVecStr(string fmt)
        {
            // format margins and padding for raster images
            double scale = model.GetV<double>(typeof(BasicSceneGenerator.Scale));

            double margin = GetMarginValue(DepictionGenerator.DEFAULT_MM_MARGIN);
            double padding = GetPaddingValue(DEFAULT_PADDING_FACTOR * margin);

            // All vector graphics will be written in mm not px to we need to
            // adjust the size of the molecules accordingly. For now the rescaling
            // is fixed to the bond length proposed by ACS 1996 guidelines (~5mm)
            double zoom = model.GetV<double>(typeof(BasicSceneGenerator.ZoomFactor)) * RescaleForBondLength(Depiction.ACS_1996_BOND_LENGTH_MM);

            // PDF and PS units are in Points (1/72 inch) in FreeHEP so need to adjust for that
            if (fmt.Equals(PDF_FMT) || fmt.Equals(PS_FMT))
            {
                zoom *= MM_TO_POINT;
                margin *= MM_TO_POINT;
                padding *= MM_TO_POINT;
            }

            // work out the required space of the main and side components separately
            // will draw these in two passes (main then side) hence want different offsets for each
            int nSideCol = xOffsetSide.Length - 1;
            int nSideRow = yOffsetSide.Length - 1;

            Dimensions sideRequired = sideDim.Scale(scale * zoom);
            Dimensions mainRequired = mainDim.Scale(scale * zoom);
            Dimensions condRequired = condDim.Scale(scale * zoom);

            Dimensions titleRequired = new Dimensions(title.Width, title.Height).Scale(scale * zoom);

            double firstRowHeight = scale * zoom * yOffsets[1];
            Dimensions total = CalcTotalDimensions(margin, padding, mainRequired, sideRequired, titleRequired, firstRowHeight, fmt);
            double fitting = CalcFitting(margin, padding, mainRequired, sideRequired, titleRequired, firstRowHeight, fmt);

            // create the image for rendering
            FreeHepWrapper wrapper = null;
            if (!fmt.Equals(SVG_FMT))
                wrapper = new FreeHepWrapper(fmt, total.w, total.h);
            IDrawVisitor visitor = fmt.Equals(SVG_FMT) ? (IDrawVisitor)new SvgDrawVisitor(total.w, total.h)
                                                            : (IDrawVisitor)WPFDrawVisitor.ForVectorGraphics(wrapper.g2);
            if (fmt.Equals(SVG_FMT))
            {
                SvgPrevisit(fmt, scale * zoom * fitting, (SvgDrawVisitor)visitor, mainComp);
            }
            else
            {
                // pdf can handle fraction coords just fine
                //((WPFDrawVisitor) visitor).SetRounding(false);
            }

            // background color
            visitor.SetTransform(new ScaleTransform(1, -1));
            visitor.Visit(new RectangleElement(new Point(0, -total.h), total.w, total.h, true, model.GetV<Color>(typeof(BasicSceneGenerator.BackgroundColor))));

            // compound the zoom, fitting and scaling into a single value
            double rescale = zoom * fitting * scale;
            double mainCompOffset = 0;

            // shift product x-offset to make room for the arrow / side components
            mainCompOffset = fitting * sideRequired.h + nSideRow * padding - fitting * firstRowHeight / 2;
            for (int i = arrowIdx + 1; i < xOffsets.Length; i++)
            {
                xOffsets[i] += sideRequired.w * 1 / (scale * zoom);
            }

            // MAIN COMPONENTS DRAW
            // x,y base coordinates include the margin and centering (only if fitting to a size)
            double totalRequiredWidth = 2 * margin + Math.Max(0, nCol - 1) * padding + Math.Max(0, nSideCol - 1) * padding + (rescale * xOffsets[nCol]);
            double totalRequiredHeight = 2 * margin + Math.Max(0, nRow - 1) * padding + (!title.IsEmpty() ? padding : 0) + Math.Max(mainCompOffset, 0) + fitting * mainRequired.h + fitting * Math.Max(0, titleRequired.h);
            double xBase = margin + (total.w - totalRequiredWidth) / 2;
            double yBase = margin + Math.Max(mainCompOffset, 0) + (total.h - totalRequiredHeight) / 2;
            for (int i = 0; i < mainComp.Count; i++)
            {

                int row = i / nCol;
                int col = i % nCol;

                // calc the 'view' bounds:
                //  amount of padding depends on which row or column we are in.
                //  the width/height of this col/row can be determined by the next offset
                double x = xBase + col * padding + rescale * xOffsets[col];
                double y = yBase + row * padding + rescale * yOffsets[row];
                double w = rescale * (xOffsets[col + 1] - xOffsets[col]);
                double h = rescale * (yOffsets[row + 1] - yOffsets[row]);

                // intercept arrow draw and make it as big as need
                if (i == arrowIdx)
                {
                    w = rescale * (xOffsets[i + 1] - xOffsets[i]) + Math.Max(0, nSideCol - 1) * padding;
                    Draw(visitor,
                         1, // no zoom since arrows is drawn as big as needed
                         CreateArrow(w, arrowHeight * rescale),
                         rect(x, y, w, h));
                    continue;
                }

                // extra padding from the side components
                if (i > arrowIdx)
                    x += Math.Max(0, nSideCol - 1) * padding;

                // skip empty elements
                Bounds bounds = this.mainComp[i];
                if (bounds.IsEmpty())
                    continue;

                Draw(visitor, zoom, bounds, rect(x, y, w, h));
            }

            // RXN TITLE DRAW
            if (!title.IsEmpty())
            {
                double y = yBase + nRow * padding + rescale * yOffsets[nRow];
                double h = rescale * title.Height;
                Draw(visitor, zoom, title, rect(0, y, total.w, h));
            }

            // SIDE COMPONENTS DRAW
            xBase += arrowIdx * padding + rescale * xOffsets[arrowIdx];
            yBase -= mainCompOffset;
            for (int i = 0; i < sideComps.Count; i++)
            {
                int row = i / nSideCol;
                int col = i % nSideCol;

                // calc the 'view' bounds:
                //  amount of padding depends on which row or column we are in.
                //  the width/height of this col/row can be determined by the next offset
                double x = xBase + col * padding + rescale * xOffsetSide[col];
                double y = yBase + row * padding + rescale * yOffsetSide[row];
                double w = rescale * (xOffsetSide[col + 1] - xOffsetSide[col]);
                double h = rescale * (yOffsetSide[row + 1] - yOffsetSide[row]);

                Draw(visitor, zoom, sideComps[i], rect(x, y, w, h));
            }

            // CONDITIONS DRAW
            if (!conditions.IsEmpty())
            {
                yBase += mainCompOffset;         // back to top
                yBase += (fitting * mainRequired.h) / 2;     // now on center line (arrow)
                yBase += arrowHeight;            // now just bellow
                Draw(visitor, zoom, conditions, rect(xBase,
                                                     yBase,
                                                     fitting * condRequired.w, fitting * condRequired.h));
            }

            // reset shared xOffsets
            if (sideComps.Any())
            {
                for (int i = arrowIdx + 1; i < xOffsets.Length; i++)
                    xOffsets[i] -= sideRequired.w * 1 / (scale * zoom);
            }

            if (wrapper != null)
            {
                wrapper.Dispose();
                return wrapper.ToString();
            }
            else
            {
                return visitor.ToString();
            }
        }

        private double CalcFitting(double margin, double padding, Dimensions mainRequired, Dimensions sideRequired,
                                   Dimensions titleRequired,
                                   double firstRowHeight, string fmt)
        {
            if (dimensions == Dimensions.AUTOMATIC)
                return 1; // no fitting

            int nSideCol = xOffsetSide.Length - 1;
            int nSideRow = yOffsetSide.Length - 1;

            // need padding in calculation
            double mainCompOffset = sideRequired.h > 0 ? sideRequired.h + (nSideRow * padding) - (firstRowHeight / 2) : 0;
            if (mainCompOffset < 0)
                mainCompOffset = 0;

            Dimensions required = mainRequired.Add(sideRequired.w, mainCompOffset)
                                              .Add(0, Math.Max(0, titleRequired.h));

            // We take out the padding height of the side components but in reality
            // some of it overlaps, since reactions are normally wider then they are
            // tall we won't normally bit fitting by this param. If do fit by this
            // param we might make the depiction smaller then it needs to be but thats
            // better than cutting bits off
            Dimensions targetDim = dimensions;

            targetDim = targetDim.Add(-2 * margin, -2 * margin)
                                 .Add(-((nCol - 1) * padding), -((nRow - 1) * padding))
                                 .Add(-(nSideCol - 1) * padding, -(nSideRow - 1) * padding)
                                 .Add(0, titleRequired.h > 0 ? -padding : 0);

            // PDF and PS are in point to we need to account for that
            if (PDF_FMT.Equals(fmt) || PS_FMT.Equals(fmt))
                targetDim = targetDim.Scale(MM_TO_POINT);

            double resize = Math.Min(targetDim.w / required.w,
                                     targetDim.h / required.h);

            if (resize > 1 && !model.GetV<bool>(typeof(BasicSceneGenerator.FitToScreen)))
                resize = 1;
            return resize;
        }

        private Dimensions CalcTotalDimensions(double margin, double padding, Dimensions mainRequired,
                                               Dimensions sideRequired, Dimensions titleRequired,
                                               double firstRowHeight,
                                               string fmt)
        {
            if (dimensions == Dimensions.AUTOMATIC)
            {

                int nSideCol = xOffsetSide.Length - 1;
                int nSideRow = yOffsetSide.Length - 1;

                double mainCompOffset = sideRequired.h + (nSideRow * padding) - (firstRowHeight / 2);
                if (mainCompOffset < 0)
                    mainCompOffset = 0;

                double titleExtra = Math.Max(0, titleRequired.h);
                if (titleExtra > 0)
                    titleExtra += padding;

                return mainRequired.Add(2 * margin, 2 * margin)
                                   .Add(Math.Max(0, nCol - 1) * padding, (nRow - 1) * padding)
                                   .Add(Math.Max(0, sideRequired.w), 0)           // side component extra width
                                   .Add(Math.Max(0, nSideCol - 1) * padding, 0) // side component padding
                                   .Add(0, mainCompOffset)
                                   .Add(0, titleExtra);

            }
            else
            {
                // we want all vector graphics dims in MM
                if (PDF_FMT.Equals(fmt) || PS_FMT.Equals(fmt))
                    return dimensions.Scale(MM_TO_POINT);
                else
                    return dimensions;
            }
        }

        private Rect rect(double x, double y, double w, double h)
        {
            return new Rect(x, y, w, h);
        }

        private Bounds CreateArrow(double minWidth, double minHeight)
        {
            Bounds arrow = new Bounds();
            double headThickness = minHeight / 3;
            double inset = 0.8;
            double headLength = minHeight;
            switch (direction)
            {
                case ReactionDirections.Forward:
                    {
                        var fp = new PathFigure();
                        arrow.Add(new LineElement(new Point(0, 0), new Point(minWidth - 0.5 * headLength, 0), minHeight / 14, fgcol));
                        fp.StartPoint = new Point(minWidth, 0);
                        fp.Segments.Add(new LineSegment(new Point(minWidth - headLength, +headThickness), true));
                        fp.Segments.Add(new LineSegment(new Point(minWidth - inset * headLength, 0), true));
                        fp.Segments.Add(new LineSegment(new Point(minWidth - headLength, -headThickness), true));
                        fp.IsClosed = true;
                        var path = new PathGeometry(new[] { fp });
                        arrow.Add(GeneralPath.ShapeOf(path, fgcol));
                    }
                    break;
                case ReactionDirections.Backward:
                    {
                        var fp = new PathFigure();
                        arrow.Add(new LineElement(new Point(0.5 * headLength, 0), new Point(minWidth, 0), minHeight / 14, fgcol));
                        fp.StartPoint = new Point(0, 0);
                        fp.Segments.Add(new LineSegment(new Point(minHeight, +headThickness), true));
                        fp.Segments.Add(new LineSegment(new Point(minHeight - (1 - inset) * minHeight, 0), true));
                        fp.Segments.Add(new LineSegment(new Point(minHeight, -headThickness), true));
                        fp.IsClosed = true;
                        var path = new PathGeometry(new[] { fp });
                        arrow.Add(GeneralPath.ShapeOf(path, fgcol));
                    }
                    break;
                case ReactionDirections.Bidirectional: // equilibrium?
                    {
                        var fp1 = new PathFigure();
                        fp1.StartPoint = new Point(0, 0.5 * +headThickness);
                        fp1.Segments.Add(new LineSegment(new Point(minWidth + minHeight + minHeight, 0.5 * +headThickness), true));
                        fp1.Segments.Add(new LineSegment(new Point(minWidth + minHeight, 1.5 * +headThickness), true));
                        var fp2 = new PathFigure();
                        fp2.StartPoint = new Point(minWidth + minHeight + minHeight, 0.5 * -headThickness);
                        fp2.Segments.Add(new LineSegment(new Point(0, 0.5 * -headThickness), true));
                        fp2.Segments.Add(new LineSegment(new Point(minHeight, 1.5 * -headThickness), true));
                        var path = new PathGeometry(new[] { fp1, fp2 });
                        arrow.Add(GeneralPath.OutlineOf(path, minHeight / 14, fgcol));
                    }
                    break;
            }

            return arrow;
        }
    }
}
