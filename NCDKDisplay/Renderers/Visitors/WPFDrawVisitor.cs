/* Copyright (C) 2008 Gilleain Torrance <gilleain.torrance@gmail.com>
 *
 *  Contact: cdk-devel@list.sourceforge.net
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
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Numerics;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Elements.Path;
using NCDK.Renderers.Fonts;
using NCDK.Renderers.Generators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using static NCDK.Renderers.Generators.BasicBondGenerator;
using static NCDK.Renderers.Generators.BasicSceneGenerator;
using static NCDK.Renderers.Generators.Standards.VecmathUtil;
using WPF = System.Windows;

namespace NCDK.Renderers.Visitors
{
    /// <summary>
    /// Implementation of the <see cref="IDrawVisitor"/> interface for the WPF,
    /// allowing molecules to be rendered with toolkits based on WPF.
    /// </summary>
    // @cdk.module renderawt
    // @cdk.githash
    public class WPFDrawVisitor : AbstractWPFDrawVisitor
    {
        /// <summary>
        /// The font manager cannot be set by the constructor as it needs to
        /// be managed by the Renderer.
        /// </summary>
        private WPFFontManager fontManager;

        /// <summary>
        /// The renderer model cannot be set by the constructor as it needs to
        /// be managed by the Renderer.
        /// </summary>
        private RendererModel rendererModel;

        /// <summary>
        /// The current stroke map.
        /// </summary>
        public IDictionary<int, Pen> StrokeMap { get; } = new Dictionary<int, Pen>();

        /// <summary>
        /// Returns the current <see cref="RendererModel"/>.
        /// </summary>
        /// <returns>the current model</returns>
        public RendererModel GetRendererModel()
        {
            return rendererModel;
        }

        private readonly double minStroke;
        private readonly bool strokeCache;
        private readonly DrawingContext graphics;

        /// <summary>
        /// Returns the <see cref="DrawingContext"/> for for this visitor.
        /// </summary>
        /// <returns>the <see cref="DrawingContext"/> object</returns>
        public DrawingContext GetGraphics()
        {
            return graphics;
        }

        /// <summary>
        /// Constructs a new <see cref="IDrawVisitor"/> using the AWT widget toolkit,
        /// taking a <see cref="DrawingContext"/> object to which the chemical content
        /// is drawn.
        /// </summary>
        /// <param name="graphics"><see cref="DrawingContext"/> to which will be drawn</param>
        public WPFDrawVisitor(DrawingContext graphics)
            : this(graphics, true, 1.5)
        { }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="graphics">the graphics instance</param>
        /// <param name="strokeCache">cache strokes internally, only sizes at 0.25 increments are stored</param>
        /// <param name="minStroke">the minimum stroke, strokes smaller than this are automatically resized</param>
        private WPFDrawVisitor(DrawingContext graphics, bool strokeCache, double minStroke)
        {
            this.graphics = graphics;
            this.fontManager = null;
            this.rendererModel = null;
            this.strokeCache = strokeCache;
            this.minStroke = minStroke;
        }

        /// <summary>
        /// Create a draw visitor that will be rendering to a vector graphics output. This disables
        /// the minimum stroke size and stroke caching when drawing lines.
        /// </summary>
        /// <param name="g2">graphics environment</param>
        /// <returns>draw visitor</returns>
        public static WPFDrawVisitor ForVectorGraphics(DrawingContext g2)
        {
            return new WPFDrawVisitor(g2, false, double.NegativeInfinity);
        }

        private void Visit(ElementGroup elementGroup)
        {
            elementGroup.Visit(this);
        }

        private void Visit(LineElement line)
        {
            // scale the stroke by zoom + scale (both included in the AffineTransform)
            var width = line.width * transform.Value.M11;  // GetScaleX()
            if (width < minStroke) width = minStroke;

            int key = (int)(width * 4); // store 2.25, 2.5, 2.75 etc to separate keys

            Pen pen;
            if (strokeCache && StrokeMap.TryGetValue(key, out pen))
            {
                // do nothing
            }
            else
            {
                Pen stroke = new Pen(new SolidColorBrush(line.color), width);
                pen = stroke;
                StrokeMap[key] = stroke;
            }

            var linePoints = new WPF::Point[] { line.firstPoint, line.secondPoint };

            linePoints[0] = transform.Transform(linePoints[0]);
            linePoints[1] = transform.Transform(linePoints[1]);
            graphics.DrawLine(pen, linePoints[0], linePoints[1]);
        }

        private void Visit(OvalElement oval)
        {
            var radius = ScaleX(oval.radius);
            var diameter = ScaleX(oval.radius * 2);
            var center = Transform(oval.coord);
            var brush = new SolidColorBrush(oval.color);

            if (oval.fill)
            {
                this.graphics.DrawEllipse(
                    brush,
                    null,
                    new WPF.Point(center.X - radius, center.Y - radius), diameter, diameter);
            }
            else
            {
                this.graphics.DrawEllipse(
                    null,
                    new Pen(brush, 1),
                    new WPF.Point(center.X - radius, center.Y - radius), diameter, diameter);
            }
        }

        private double ScaleX(double xCoord)
        {
            return (xCoord * transform.Value.M11);  // Scale X
        }

        private double ScaleY(double yCoord)
        {
            return (yCoord * transform.Value.M22); // Scale Y
        }

        private WPF::Point Transform(WPF::Point coord)
        {
            var result = transform.Transform(coord);
            return result;
        }

        private Color GetBackgroundColor()
        {
            return rendererModel == null ?
                new BasicSceneGenerator.BackgroundColor().Default.Value :
                rendererModel.GetV<Color>(typeof(BasicSceneGenerator.BackgroundColor));
        }

        private void Visit(TextElement textElement)
        {
            var point = this.GetTextBasePoint(textElement.text, textElement.coord, this.fontManager.Typeface, this.fontManager.Size);
            var textBounds = this.GetTextBounds(textElement.text, textElement.coord, this.fontManager.Typeface, this.fontManager.Size);
            var backColor = GetBackgroundColor();
            this.graphics.DrawRectangle(new SolidColorBrush(backColor), null, textBounds);
            this.graphics.DrawText(new FormattedText(
                          textElement.text,
                          CultureInfo.CurrentCulture,
                          WPF.FlowDirection.LeftToRight,
                          this.fontManager.CureentTypeface,
                          this.fontManager.Size,
                          new SolidColorBrush(textElement.color)), point);
        }

        private void Visit(WedgeLineElement wedge)
        {
            // make the vector normal to the wedge axis
            Vector2 normal = new Vector2(wedge.firstPoint.Y - wedge.secondPoint.Y, wedge.secondPoint.X - wedge.firstPoint.X);
            normal = Vector2.Normalize(normal);
            normal *= (rendererModel.GetV<double>(typeof(WedgeWidth)) / rendererModel.GetV<double>(typeof(Scale)));

            // make the triangle corners
            Vector2 vertexA = new Vector2(wedge.firstPoint.X, wedge.firstPoint.Y);
            Vector2 vertexB = new Vector2(wedge.secondPoint.X, wedge.secondPoint.Y) + normal;
            Vector2 vertexC = vertexB - normal;

            var brush = new SolidColorBrush(wedge.color);

            if (wedge.type == WedgeLineElement.TYPE.Dashed)
            {
                #region this.DrawDashedWedge(vertexA, vertexB, vertexC);
                var pen = new Pen(brush, 1);

                // calculate the distances between lines
                double distance = Vector2.Distance(vertexB, vertexA);
                double gapFactor = 0.1;
                double gap = distance * gapFactor;
                double numberOfDashes = distance / gap;
                double displacement = 0;

                // draw by interpolating along the edges of the triangle
                for (int i = 0; i < numberOfDashes; i++)
                {
                    Vector2 point1 = Vector2.Lerp(vertexA, vertexB, displacement);
                    Vector2 point2 = Vector2.Lerp(vertexA, vertexC, displacement);
                    var p1T = this.TransformPoint(ToPoint(point1));
                    var p2T = this.TransformPoint(ToPoint(point2));
                    this.graphics.DrawLine(pen, p1T, p2T);
                    if (distance * (displacement + gapFactor) >= distance)
                    {
                        break;
                    }
                    else
                    {
                        displacement += gapFactor;
                    }
                }
                #endregion
            }
            else if (wedge.type == WedgeLineElement.TYPE.Wedged)
            {
                #region this.DrawFilledWedge(brush, vertexA, vertexB, vertexC);
                var pointB = this.TransformPoint(ToPoint(vertexB));
                var pointC = this.TransformPoint(ToPoint(vertexC));
                var pointA = this.TransformPoint(ToPoint(vertexA));

                var figure = new PathFigure();
                figure.StartPoint = pointB;
                figure.Segments.Add(new LineSegment(pointC, false));
                figure.Segments.Add(new LineSegment(pointA, false));
                var g = new PathGeometry(new[] { figure });
                this.graphics.DrawGeometry(brush, null, g);
                #endregion
            }
            else if (wedge.type == WedgeLineElement.TYPE.Indiff)
            {
                #region  this.DrawIndiffWedge(vertexA, vertexB, vertexC);
                var pen = new Pen(brush, 1);
                pen.StartLineCap = PenLineCap.Round;
                pen.LineJoin = PenLineJoin.Round;
                pen.EndLineCap = PenLineCap.Round;

                // calculate the distances between lines
                double distance = Vector2.Distance(vertexB, vertexA);
                double gapFactor = 0.05;
                double gap = distance * gapFactor;
                double numberOfDashes = distance / gap;
                double displacement = 0;

                // draw by interpolating along the edges of the triangle
                Vector2 point1 = Vector2.Lerp(vertexA, vertexB, displacement);
                bool flip = false;
                var p1T = this.TransformPoint(ToPoint(point1));
                displacement += gapFactor;
                for (int i = 0; i < numberOfDashes; i++)
                {
                    Vector2 point2;
                    if (flip)
                    {
                        point2 = Vector2.Lerp(vertexA, vertexC, displacement);
                    }
                    else
                    {
                        point2 = Vector2.Lerp(vertexA, vertexB, displacement);
                    }
                    flip = !flip;
                    var p2T = this.TransformPoint(ToPoint(point2));
                this.graphics.DrawLine(pen, p1T, p2T);
                    if (distance * (displacement + gapFactor) >= distance)
                    {
                        break;
                    }
                    else
                    {
                        p1T = p2T;
                        displacement += gapFactor;
                    }
                }
                #endregion
            }
        }

        private void Visit(AtomSymbolElement atomSymbol)
        {
            var xy = this.TransformPoint(atomSymbol.coord);

            var bounds = GetTextBounds(atomSymbol.text, this.fontManager.CureentTypeface, this.fontManager.Size);

            double w = bounds.Width;
            double h = bounds.Height;

            double xOffset = bounds.X;
            double yOffset = bounds.Y + bounds.Height;

            bounds = new WPF.Rect(xy.X - (w / 2), xy.Y - (h / 2), w, h);

            var backgroundBrush = new SolidColorBrush(GetBackgroundColor());
            var atomSymbolBrush = new SolidColorBrush(atomSymbol.color);

            double padding = h / 4;
            this.graphics.DrawRoundedRectangle(
                backgroundBrush, null,
                new WPF::Rect(
                    bounds.X - (padding / 2), bounds.Y - (padding / 2),
                    bounds.Width + padding, bounds.Height + padding), padding, padding);
            this.graphics.DrawText(new FormattedText(
                    atomSymbol.text,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    atomSymbolBrush),
                    new WPF::Point(bounds.X - xOffset, bounds.Y + h - yOffset));

            int offset = 10; // XXX
            string chargeString;
            if (atomSymbol.formalCharge == 0)
            {
                return;
            }
            else if (atomSymbol.formalCharge == 1)
            {
                chargeString = "+";
            }
            else if (atomSymbol.formalCharge > 1)
            {
                chargeString = atomSymbol.formalCharge + "+";
            }
            else if (atomSymbol.formalCharge == -1)
            {
                chargeString = "-";
            }
            else if (atomSymbol.formalCharge < -1)
            {
                int absCharge = Math.Abs(atomSymbol.formalCharge);
                chargeString = absCharge + "-";
            }
            else
            {
                return;
            }

            var xCoord = bounds.GetCenterX();
            var yCoord = bounds.GetCenterY();
            if (atomSymbol.alignment == 1)
            { // RIGHT
                this.graphics.DrawText(new FormattedText(
                    chargeString,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    atomSymbolBrush),
                    new WPF::Point(xCoord + offset, bounds.Top));
            }
            else if (atomSymbol.alignment == -1)
            { // LEFT
                this.graphics.DrawText(new FormattedText(
                    chargeString,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    atomSymbolBrush),
                    new WPF::Point(xCoord - offset, bounds.Top));
            }
            else if (atomSymbol.alignment == 2)
            { // TOP
                this.graphics.DrawText(new FormattedText(
                    chargeString,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    atomSymbolBrush),
                    new WPF::Point(xCoord, yCoord - offset));
            }
            else if (atomSymbol.alignment == -2)
            { // BOT
                this.graphics.DrawText(new FormattedText(
                    chargeString,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    atomSymbolBrush),
                    new WPF::Point(xCoord, yCoord + offset));
            }
        }

        private void Visit(RectangleElement rectangle)
        {
            //this.graphics.SetColor(rectangle.color);
            var width = ScaleX(rectangle.width);
            var height = ScaleY(rectangle.height);
            var p = Transform(rectangle.coord);
            var rect = new WPF.Rect(p.X, p.Y, width, height);
            if (rectangle.filled)
            {
                this.graphics.DrawRectangle(new SolidColorBrush(rectangle.color), null, rect);
            }
            else
            {
                this.graphics.DrawRectangle(null, new Pen(new SolidColorBrush(rectangle.color), 1), rect);
            }
        }

        private void Visit(Elements.PathElement path)
        {
            var pen = new Pen(new SolidColorBrush(path.color), 1);
            for (int i = 1; i < path.points.Count; i++)
            {
                var point1 = path.points[i - 1];
                var point2 = path.points[i];
                var lineStart = this.TransformPoint(point1);
                var lineEnd = this.TransformPoint(point2);
                this.graphics.DrawLine(pen, lineStart, lineEnd);
            }
        }

        private void Visit(GeneralPath path)
        {
            PathGeometry g = new PathGeometry();
            g.FillRule = path.winding;

            PathFigure pf = null;
            foreach (var element in path.elements)
            {
                var pp = element.Points;
                switch (element.Type)
                {
                    case PathType.MoveTo:
                        if (pf != null)
                            g.Figures.Add(pf);
                        pf = new PathFigure();
                        pf.StartPoint = Transform(pp[0]);
                        break;
                    case PathType.LineTo:
                        var ls = new LineSegment(Transform(pp[0]), true);
                        pf.Segments.Add(ls);
                        break;
                    case PathType.QuadTo:
                        var qs = new QuadraticBezierSegment(Transform(pp[0]), Transform(pp[1]), true);
                        pf.Segments.Add(qs);
                        break;
                    case PathType.CubicTo:
                        var cs = new BezierSegment(Transform(pp[0]), Transform(pp[1]), Transform(pp[2]), true);
                        pf.Segments.Add(cs);
                        break;
                    case PathType.Close:
                    default:
                        g.Figures.Add(pf);
                        pf = null;
                        break;
                }
            }

            this.graphics.DrawGeometry(
                path.fill ? new SolidColorBrush(path.color) : null,
                new Pen(new SolidColorBrush(path.color), path.stroke),
                g);
        }


        private void Visit(ArrowElement line)
        {
            double scale = rendererModel.GetV<double>(typeof(Scale));

            Pen pen = null;
            {
                int w = (int)(line.width * scale);
                if (StrokeMap.ContainsKey(w))
                {
                    pen = StrokeMap[w];
                }
                else
                {
                    pen = new Pen(new SolidColorBrush(line.color), w);
                    StrokeMap[w] = pen;
                }
            }

            var a = this.TransformPoint(line.start);
            var b = this.TransformPoint(line.end);
            graphics.DrawLine(pen, a, b);
            double aW = rendererModel.GetV<double>(typeof(ArrowHeadWidth)) / scale;
            if (line.direction)
            {
                var c = this.TransformPoint(new WPF.Point(line.start.X - aW, line.start.Y - aW));
                var d = this.TransformPoint(new WPF.Point(line.start.X - aW, line.start.Y + aW));
                graphics.DrawLine(pen, a, c);
                graphics.DrawLine(pen, a, d);
            }
            else
            {
                var c = this.TransformPoint(new WPF.Point(line.end.X + aW, line.end.Y - aW));
                var d = this.TransformPoint(new WPF.Point(line.end.X + aW, line.end.Y + aW));
                graphics.DrawLine(pen, b, c);
                graphics.DrawLine(pen, b, d);
            }
        }

        private void Visit(TextGroupElement textGroup)
        {
            var point = GetTextBasePoint(textGroup.text, textGroup.coord, fontManager.CureentTypeface, fontManager.Size);
            var textBounds = GetTextBounds(textGroup.text, textGroup.coord, fontManager.CureentTypeface, fontManager.Size);
            this.graphics.DrawRectangle(new SolidColorBrush(GetBackgroundColor()), null, textBounds);
            this.graphics.DrawText(new FormattedText(
                textGroup.text,
                CultureInfo.CurrentCulture,
                WPF.FlowDirection.LeftToRight,
                this.fontManager.CureentTypeface,
                this.fontManager.Size,
                new SolidColorBrush(textGroup.color)),
                new WPF::Point(point.X, point.Y));

            var coord = new WPF::Point(textBounds.GetCenterX(), textBounds.GetCenterY());
            var coord1 = textBounds.TopLeft;
            var coord2 = new WPF::Point(point.X + textBounds.Width, textBounds.Bottom);

            var vo = coord2 - coord1;
            var o = new WPF::Size(vo.X, vo.Y);
            foreach (var child in textGroup.children)
            {
                WPF::Point p;

                switch (child.position.Ordinal)
                {
                    case TextGroupElement.Position.O.NE:
                        p = new WPF::Point(coord2.X, coord1.Y);
                        break;
                    case TextGroupElement.Position.O.N:
                        p = new WPF::Point(coord1.X, coord1.Y);
                        break;
                    case TextGroupElement.Position.O.NW:
                        p = new WPF::Point(coord1.X - o.Width, coord1.Y);
                        break;
                    case TextGroupElement.Position.O.W:
                        p = new WPF::Point(coord1.X - o.Width, coord.Y);
                        break;
                    case TextGroupElement.Position.O.SW:
                        p = new WPF::Point(coord1.X - o.Width, coord1.Y + o.Height);
                        break;
                    case TextGroupElement.Position.O.S:
                        p = new WPF::Point(coord1.X, coord2.Y + o.Height);
                        break;
                    case TextGroupElement.Position.O.SE:
                        p = new WPF::Point(coord2.X, coord2.Y + o.Height);
                        break;
                    case TextGroupElement.Position.O.E:
                        p = new WPF::Point(coord2.X, coord.Y);
                        break;
                    default:
                        p = new WPF::Point(coord.X, coord.Y);
                        break;
                }

                this.graphics.DrawText(new FormattedText(
                    child.text,
                    CultureInfo.CurrentCulture,
                    WPF.FlowDirection.LeftToRight,
                    this.fontManager.CureentTypeface,
                    this.fontManager.Size,
                    new SolidColorBrush(textGroup.color)),
                    p);

                if (child.subscript != null)
                {
                    var childBounds = GetTextBounds(child.text, p, fontManager.CureentTypeface, fontManager.Size);
                    var scx = p.X + (childBounds.Width * 0.75);
                    var scy = p.Y + (childBounds.Height / 3);
                    this.graphics.DrawText(new FormattedText(
                        child.subscript,
                        CultureInfo.CurrentCulture,
                        WPF.FlowDirection.LeftToRight,
                        this.fontManager.CureentTypeface,
                        this.fontManager.Size - 2,
                        new SolidColorBrush(textGroup.color)),
                        p);
                }
            }
        }

        /// <inheritdoc/>
        public override void Visit(IRenderingElement element)
        {
            if (element is ElementGroup)
                Visit((ElementGroup)element);
            else if (element is WedgeLineElement)
                Visit((WedgeLineElement)element);
            else if (element is LineElement)
                Visit((LineElement)element);
            else if (element is OvalElement)
                Visit((OvalElement)element);
            else if (element is TextGroupElement)
                Visit((TextGroupElement)element);
            else if (element is AtomSymbolElement)
                Visit((AtomSymbolElement)element);
            else if (element is TextElement)
                Visit((TextElement)element);
            else if (element is RectangleElement)
                Visit((RectangleElement)element);
            else if (element is Renderers.Elements.PathElement)
                Visit((Renderers.Elements.PathElement)element);
            else if (element is GeneralPath)
                Visit((GeneralPath)element);
            else if (element is ArrowElement)
                Visit((ArrowElement)element);
            else if (element is Bounds)
            {
                Visit(((Bounds)element).Root);
            }
            else if (element is MarkedElement)
            {
                Visit(((MarkedElement)element).Element());
            }
            else
                Console.Error.WriteLine("Visitor method for " + element.GetType().FullName + " is not implemented");
        }

        /// <summary>
        /// The font manager must be set by any renderer that uses this class!
        /// This manager is needed to keep track of fonts of the right size.
        /// </summary>
        /// <param name="fontManager">the <see cref="IFontManager"/> to be used</param>
        public override void SetFontManager(IFontManager fontManager)
        {
            this.fontManager = (WPFFontManager)fontManager;
        }

        /// <inheritdoc/>
        public override void SetRendererModel(RendererModel rendererModel)
        {
            this.rendererModel = rendererModel;
            if (rendererModel.HasParameter(typeof(UseAntiAliasing)))
            {
                if (rendererModel.GetV<bool>(typeof(UseAntiAliasing)))
                {
                    // just ignore it.
                    //graphics..SetRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
                    // g.SetStroke(new WPF::Media.Pen((int)rendererModel.GetBondWidth()));
                }
            }
        }
    }
}
