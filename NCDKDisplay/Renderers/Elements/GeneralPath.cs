/*
 * Copyright (C) 2009  Arvid Berg <goglepox@users.sourceforge.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Renderers.Elements.Path;
using System.Collections.Generic;
using System.Windows.Media;

namespace NCDK.Renderers.Elements
{
    /// <summary>
    /// A path of rendering elements from the elements.path package.
    /// </summary>
    // @author Arvid
    // @cdk.module renderbasic
    // @cdk.githash
    public class GeneralPath : IRenderingElement
    {
        /// <summary>The color of the path.</summary>
        public readonly Color color;

        /// <summary>The width of the stroke.</summary>
        public readonly double stroke;

        /// <summary>Fill the shape instead of drawing outline.</summary>
        public readonly bool fill;

        /// <summary>The elements in the path.</summary>
        public readonly IList<Path.PathElement> elements;

        /// <summary>Winding rule for determining path interior.</summary>
        public readonly FillRule winding;

        /// <summary>
        /// Make a path from a list of path elements.
        /// </summary>
        /// <param name="elements">the elements that make up the path</param>
        /// <param name="color">the color of the path</param>
        public GeneralPath(IList<Path.PathElement> elements, Color color)
           : this(elements, color, FillRule.EvenOdd, 1, true)
        { }

        /// <summary>
        /// Make a path from a list of path elements.
        /// </summary>
        /// <param name="elements">the elements that make up the path</param>
        /// <param name="color">the color of the path</param>
        private GeneralPath(IList<Path.PathElement> elements, Color color, FillRule winding, double stroke, bool fill)
        {
            this.elements = elements;
            this.color = color;
            this.winding = winding;
            this.fill = fill;
            this.stroke = stroke;
        }

        /// <summary>
        /// Recolor the path with the specified color.
        /// </summary>
        /// <param name="newColor">new path color</param>
        /// <returns>the recolored path</returns>
        public GeneralPath Recolor(Color newColor)
        {
            return new GeneralPath(elements, newColor, winding, stroke, fill);
        }

        /// <summary>
        /// Outline the general path with the specified stroke size.
        ///
        /// <param name="newStroke">new stroke size</param>
        /// <returns>the outlined path</returns>
        public GeneralPath Outline(double newStroke)
        {
            return new GeneralPath(elements, color, winding, newStroke, false);
        }

        public virtual void Accept(IRenderingVisitor v)
        {
            v.Visit(this);
        }

        /// <summary>
        /// Create a filled path of the specified Java 2D Shape and color.
        /// </summary>
        /// <param name="shape">shape</param>
        /// <param name="color">the color to fill the shape with</param>
        /// <returns>a new general path</returns>
        public static GeneralPath ShapeOf(Geometry shape, Color color)
        {
            PathGeometry pathIt = shape is PathGeometry ?
                (PathGeometry)shape :
                shape.GetFlattenedPathGeometry(0.01, ToleranceType.Absolute);
            var elements = ToPathElements(pathIt);
            return new GeneralPath(elements, color, pathIt.FillRule, 0, true);
        }

        /// <summary>
        /// Create an outline path of the specified Java 2D Shape and color.
        /// </summary>
        /// <param name="shape">Java 2D shape</param>
        /// <param name="color">the color to draw the outline with</param>
        /// <returns>a new general path</returns>
        public static GeneralPath OutlineOf(Geometry shape, double stroke, Color color)
        {
            var pathIt = shape.GetFlattenedPathGeometry();
            var elements = ToPathElements(pathIt);
            return new GeneralPath(elements, color, pathIt.FillRule, stroke, false);
        }

        private static List<Path.PathElement> ToPathElements(PathGeometry pathIt)
        {
            var elements = new List<Path.PathElement>();
            foreach (var figure in pathIt.Figures)
            {
                elements.Add(new MoveTo(figure.StartPoint));
                foreach (var seg in figure.Segments)
                {
                    if (seg is LineSegment)
                    {
                        var s = (LineSegment)seg;
                        elements.Add(new LineTo(s.Point));
                    }
                    else if (seg is QuadraticBezierSegment)
                    {
                        var s = (QuadraticBezierSegment)seg;
                        elements.Add(new QuadTo(s.Point1, s.Point2));
                    }
                    else if (seg is BezierSegment)
                    {
                        var s = (BezierSegment)seg;
                        elements.Add(new CubicTo(s.Point1, s.Point2, s.Point3));
                    }
                    else if (seg is PolyLineSegment)
                    {
                        var s = (PolyLineSegment)seg;
                        foreach (var p in s.Points)
                            elements.Add(new LineTo(p));
                    }
                    else if (seg is ArcSegment)
                    {
                        var s = (ArcSegment)seg;
                        elements.Add(new ArcTo(s.Point, s.Size, s.RotationAngle, s.IsLargeArc, s.SweepDirection));  // TODO handle ArcSegment
                    }
                    else if (seg is PolyBezierSegment)
                    {
                        var s = (PolyBezierSegment)seg;
                        foreach (var p in s.Points)
                            elements.Add(new LineTo(p));  // TODO handle PolyBezierSegment
                    }
                    else if (seg is PolyQuadraticBezierSegment)
                    {
                        var s = (PolyQuadraticBezierSegment)seg;
                        foreach (var p in s.Points)
                            elements.Add(new LineTo(p));  // TODO handle PolyQuadraticBezierSegment
                    }
                }
                elements.Add(new Close());
            }
            return elements;
        }
    }
}
