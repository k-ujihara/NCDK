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
using NCDK.Renderers.Visitors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace NCDK.Depict
{
    /// <summary>
    /// Internal - depicts a set of molecules aligned in a grid. This class
    /// also handles the degenerate case of a single molecule as a 1x1 grid.
    /// </summary>
    sealed class MolGridDepiction 
        : Depiction
    {
        private readonly RendererModel model;
        private readonly Dimensions dimensions;
        private readonly int nCol, nRow;
        private readonly List<Bounds> elements;

        public MolGridDepiction(RendererModel model,
                                List<Bounds> molecules,
                                List<Bounds> titles,
                                Dimensions dimensions,
                                int nRow, int nCol)
            : base(model)
        {
            this.model = model;
            this.dimensions = dimensions;

            this.elements = new List<Bounds>();

            // degenerate case is when no title are provided
            if (!titles.Any())
            {
                elements.AddRange(molecules);
            }
            else
            {
                Trace.Assert(molecules.Count == titles.Count);
                // interweave molecules and titles
                for (int r = 0; r < nRow; r++)
                {
                    int fromIndex = r * nCol;
                    int toIndex = Math.Min(molecules.Count, (r + 1) * nCol);
                    if (fromIndex >= toIndex)
                        break;

                    List<Bounds> molsublist = molecules.GetRange(fromIndex, toIndex - fromIndex);
                    // need to pad list
                    while (molsublist.Count < nCol)
                        molsublist.Add(new Bounds());

                    elements.AddRange(molsublist);
                    elements.AddRange(titles.GetRange(fromIndex, toIndex - fromIndex));
                }
                nRow *= 2;
            }

            this.nCol = nCol;
            this.nRow = nRow;
        }

        public override Size Draw(DrawingContext drawingContext)
        {
            // format margins and padding for raster images
            double margin = GetMarginValue(DepictionGenerator.DefaultPixelMargin);
            double padding = GetPaddingValue(DefaultPaddingFactor * margin);
            double scale = model.GetScale();
            double zoom = model.GetZoomFactor();

            // row and col offsets for alignment
            double[] yOffset = new double[nRow + 1];
            double[] xOffset = new double[nCol + 1];

            Dimensions required = Dimensions.OfGrid(elements, yOffset, xOffset).Scale(scale * zoom);

            Dimensions total = CalcTotalDimensions(margin, padding, required, null);
            double fitting = CalcFitting(margin, padding, required, null);

            IDrawVisitor visitor = WPFDrawVisitor.ForVectorGraphics(drawingContext);

            // compound the zoom, fitting and scaling into a single value
            double rescale = zoom * fitting * scale;

            // x,y base coordinates include the margin and centering (only if fitting to a size)
            double xBase = margin + (total.width - 2 * margin - (nCol - 1) * padding - (rescale * xOffset[nCol])) / 2;
            double yBase = margin + (total.height - 2 * margin - (nRow - 1) * padding - (rescale * yOffset[nRow])) / 2;

            for (int i = 0; i < elements.Count; i++)
            {
                int row = i / nCol;
                int col = i % nCol;

                // skip empty elements
                var bounds = this.elements[i];
                if (bounds.IsEmpty())
                    continue;

                // calculate the 'view' bounds:
                //  amount of padding depends on which row or column we are in.
                //  the width/height of this col/row can be determined by the next offset
                double x = xBase + col * padding + rescale * xOffset[col];
                double y = yBase + row * padding + rescale * yOffset[row];
                double w = rescale * (xOffset[col + 1] - xOffset[col]);
                double h = rescale * (yOffset[row + 1] - yOffset[row]);

                Draw(visitor, zoom, bounds, new Rect(x, y, w, h));
            }

            return new Size(total.width, total.height);
        }

        private double CalcFitting(double margin, double padding, Dimensions required, string fmt)
        {
            if (dimensions == Dimensions.Automatic)
                return 1; // no fitting
            Dimensions targetDim = dimensions;

            targetDim = targetDim.Add(-2 * margin, -2 * margin)
                                             .Add(-((nCol - 1) * padding), -((nRow - 1) * padding));
            double resize = Math.Min(targetDim.width / required.width,
                                     targetDim.height / required.height);
            if (resize > 1 && !model.GetFitToScreen())
                resize = 1;
            return resize;
        }

        private Dimensions CalcTotalDimensions(double margin, double padding, Dimensions required, string fmt)
        {
            if (dimensions == Dimensions.Automatic)
            {
                return required.Add(2 * margin, 2 * margin)
                               .Add((nCol - 1) * padding, (nRow - 1) * padding);
            }
            else
            {
                // we want all vector graphics dims in MM
                return dimensions;
            }
        }

        internal override string ToVectorString(string fmt)
        {
            // format margins and padding for raster images
            double margin = GetMarginValue(DepictionGenerator.DefaultMillimeterMargin);
            double padding = GetPaddingValue(DefaultPaddingFactor * margin);
            double scale = model.GetScale();

            // All vector graphics will be written in mm not px to we need to
            // adjust the size of the molecules accordingly. For now the rescaling
            // is fixed to the bond length proposed by ACS 1996 guidelines (~5mm)
            double zoom = model.GetZoomFactor() * RescaleForBondLength(Depiction.ACS1996BondLength);

            // row and col offsets for alignment
            double[] yOffset = new double[nRow + 1];
            double[] xOffset = new double[nCol + 1];

            Dimensions required = Dimensions.OfGrid(elements, yOffset, xOffset).Scale(zoom * scale);

            Dimensions total = CalcTotalDimensions(margin, padding, required, fmt);
            double fitting = CalcFitting(margin, padding, required, fmt);

            // create the image for rendering
            IDrawVisitor visitor = new SvgDrawVisitor(total.width, total.height);

            if (fmt.Equals(SvgFormatKey))
            {
                SvgPrevisit(fmt, scale * zoom * fitting, (SvgDrawVisitor)visitor, elements);
            }
            else
            {
            }

            visitor.Visit(new RectangleElement(new Point(0, -total.height), total.width, total.height, true, model.GetBackgroundColor()), new ScaleTransform(1, -1));

            // compound the fitting and scaling into a single value
            double rescale = zoom * fitting * scale;

            // x,y base coordinates include the margin and centering (only if fitting to a size)
            double xBase = margin + (total.width - 2 * margin - (nCol - 1) * padding - (rescale * xOffset[nCol])) / 2;
            double yBase = margin + (total.height - 2 * margin - (nRow - 1) * padding - (rescale * yOffset[nRow])) / 2;

            for (int i = 0; i < elements.Count; i++)
            {
                int row = i / nCol;
                int col = i % nCol;

                // calculate the 'view' bounds:
                //  amount of padding depends on which row or column we are in.
                //  the width/height of this col/row can be determined by the next offset
                double x = xBase + col * padding + rescale * xOffset[col];
                double y = yBase + row * padding + rescale * yOffset[row];
                double w = rescale * (xOffset[col + 1] - xOffset[col]);
                double h = rescale * (yOffset[row + 1] - yOffset[row]);

                Draw(visitor, zoom, elements[i], new Rect(x, y, w, h));
            }

            return visitor.ToString();
        }
    }
}
