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

using NCDK.Common.Collections;
using NCDK.Common.Mathematics;
using NCDK.Renderers;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Elements.Path;
using NCDK.Renderers.Fonts;
using NCDK.Renderers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace NCDK.Depict
{
    /// <summary>
    /// Internal - An SvgDrawVisitor, currently only certain elements are supported
    /// but covers depictions generated by the <see cref="Renderers.Generators.Standards.StandardGenerator"/>
    /// (only <see cref="Renderers.Elements.LineElement"/> and <see cref="Renderers.Elements.GeneralPath"/>).
    /// </summary>
    /// <example>
    /// Usage:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Depict.SvgDrawVisitor_Example.cs+1"]/*' />
    /// </example>
    public sealed class SvgDrawVisitor : IDrawVisitor
    {
        private readonly StringBuilder sb = new StringBuilder(5000);

        private int indentLvl = 0;
        private Transform transform = null;
        private RendererModel model = null;
        private static string FormatDecimal(double v)
            => ((float)v).ToString();//NCDK.Common.Primitives.Strings.JavaFormat(v, 2, false);

        private bool defaultsWritten = false;
        private Color? defaultStroke = null;
        private Color? defaultFill = null;
        private string defaultStrokeWidth = null;

        /// <summary>
        /// Create an SvgDrawVisitor with the specified width/height
        /// </summary>
        /// <param name="w">width of canvas in mm</param>
        /// <param name="h">height of canvas in mm</param>
        public SvgDrawVisitor(double w, double h)
        {
            WriteHeader(w, h);
        }

        private void WriteHeader(double w, double h)
        {
            sb.Append("<?xml version='1.0' encoding='UTF-8'?>\n")
              .Append("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">\n");
            sb.Append("<svg")
              .Append(" version='1.2'")
              .Append(" xmlns='http://www.w3.org/2000/svg'")
              .Append(" xmlns:xlink='http://www.w3.org/1999/xlink'")
              .Append(" width='").Append(ToString(w)).Append("mm'")
              .Append(" height='").Append(ToString(h)).Append("mm'")
              .Append(" viewBox='0 0 ").Append(ToString(w)).Append(" ").Append(ToString(h)).Append("'")
              .Append(">\n");
            indentLvl += 2;
            AppendIdent();
            sb.Append("<desc>Generated by the Chemistry Development Kit (http://github.com/cdk)</desc>\n");
        }

        private void AppendIdent()
        {
            for (int i = 0; i < indentLvl; i++)
                sb.Append(' ');
        }

        private double Scaled(double num)
        {
            if (transform == null)
                return num;
            // presumed uniform x/y scaling
            return transform.Value.M11 * num;
        }

        private void TransformPoints(Point[] points)
        {
            if (transform != null)
            {
                for (var i = 0; i < points.Length; i++)
                {
                    var p = points[i];
                    points[i] = transform.Transform(p);
                }
            }
        }

        private string ToString(double num)
        {
            return FormatDecimal(num);
        }

        private void AppendPoints(StringBuilder sb, Point[] points, int numPoints)
        {
            switch (numPoints)
            {
                case 1:
                    sb.Append(FormatDecimal(points[0].X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[0].Y));
                    break;
                case 2:
                    sb.Append(FormatDecimal(points[0].X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[0].Y));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[1].X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[1].Y));
                    break;
                case 3:
                    sb.Append(FormatDecimal(points[0].X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[0].Y));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[1].X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[1].Y));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[2].X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[2].Y));
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private void AppendRelativePoints(StringBuilder sb, Point[] points, Point vBase, int numPoints)
        {
            switch (numPoints)
            {
                case 1:
                    sb.Append(FormatDecimal(points[0].X - vBase.X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[0].Y - vBase.Y));
                    break;
                case 2:
                    sb.Append(FormatDecimal(points[0].X - vBase.X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[0].Y - vBase.Y));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[1].X - vBase.X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[1].Y - vBase.Y));
                    break;
                case 3:
                    sb.Append(FormatDecimal(points[0].X - vBase.X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[0].Y - vBase.Y));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[1].X - vBase.X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[1].Y - vBase.Y));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[2].X - vBase.X));
                    sb.Append(' ');
                    sb.Append(FormatDecimal(points[2].Y - vBase.Y));
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        string ToString(Color col)
        {
            if (col.A == 255)
            {
                return $"#{col.R.ToString("X2")}{col.G.ToString("X2")}{col.B.ToString("X2")}";
            }
            else
            {
                return $"rgba({col.R},{col.G},{col.B},{(col.A / 255.0).ToString("F2")})";
            }
        }

        public IFontManager FontManager
        {
            get => null;
            set { } // ignored
        }

        public RendererModel RendererModel
        {
            get => this.model;
            set => this.model = value;
        }

        /// <summary>
        /// Pre-visit allows us to prepare the visitor for more optimal output.
        /// Currently we
        /// - find the most common stoke/fill/stroke-width values and set these as defaults
        /// </summary>
        /// <param name="elements">elements to be visited</param>
        public void Previsit(IEnumerable<IRenderingElement> elements)
        {
            Deque<IRenderingElement> queue = new ArrayDeque<IRenderingElement>();
            queue.AddRange(elements);

            var strokeFreq = new FreqMap<Color>();
            var fillFreq = new FreqMap<Color>();
            var strokeWidthFreq = new FreqMap<double>();

            while (queue.Any())
            {
                IRenderingElement element = queue.Poll();
                // wrappers first
                if (element is Bounds)
                {
                    queue.Add(((Bounds)element).Root);
                }
                else if (element is MarkedElement)
                {
                    queue.Add(((MarkedElement)element).Element());
                }
                else if (element is ElementGroup)
                {
                    foreach (var child in (ElementGroup)element)
                        queue.Add(child);
                }
                else if (element is LineElement)
                {
                    strokeFreq.Add(((LineElement)element).color);
                    strokeWidthFreq.Add(Scaled(((LineElement)element).width));
                }
                else if (element is GeneralPath)
                {
                    if (((GeneralPath)element).fill)
                        fillFreq.Add(((GeneralPath)element).color);
                }
                else
                {
                    // ignored
                }
            }

            if (!defaultsWritten)
            {
                defaultFill = fillFreq.GetMostFrequent();
                defaultStroke = strokeFreq.GetMostFrequent();
                var strokeWidth = strokeWidthFreq.GetMostFrequent();
                if (strokeWidth != null)
                    defaultStrokeWidth = ToString(strokeWidth.Value);
            }
        }

        private void Visit(GeneralPath elem)
        {
            Visit(null, null, elem);
        }

        private void Visit(string id, string cls, GeneralPath elem)
        {
            AppendIdent();
            sb.Append("<path");
            if (id != null) sb.Append(" id='").Append(id).Append("'");
            if (cls != null) sb.Append(" class='").Append(cls).Append("'");
            sb.Append(" d='");
            Point currPoint = new Point(0, 0);
            foreach (var pelem in elem.elements)
            {
                switch (pelem.type)
                {
                    case Renderers.Elements.Path.PathType.Close:
                        sb.Append("z");
                        currPoint = new Point(0, 0);
                        break;
                    case Renderers.Elements.Path.PathType.LineTo:
                        {
                            var points = pelem.Points;
                            TransformPoints(points);
                            var d = points[0] - currPoint;
                            // horizontal and vertical lines can be even more compact
                            if (Math.Abs(d.X) < 0.01)
                            {
                                sb.Append("v").Append(ToString(d.Y));
                            }
                            else if ((Math.Abs(d.Y) < 0.01))
                            {
                                sb.Append("h").Append(ToString(d.X));
                            }
                            else
                            {
                                sb.Append("l");
                                AppendRelativePoints(sb, points, currPoint, 1);
                            }
                            currPoint = points[0];
                        }
                        break;
                    case Renderers.Elements.Path.PathType.MoveTo:
                        {
                            var points = pelem.Points;
                            // We have Move as always absolute
                            sb.Append("M");
                            TransformPoints(points);
                            AppendPoints(sb, points, 1);
                            currPoint = points[0];
                        }
                        break;
                    case Renderers.Elements.Path.PathType.QuadTo:
                        {
                            var points = pelem.Points;
                            sb.Append("q");
                            TransformPoints(points);
                            AppendRelativePoints(sb, points, currPoint, 2);
                            currPoint = points[1];
                        }
                        break;
                    case Renderers.Elements.Path.PathType.CubicTo:
                        {
                            var points = pelem.Points;
                            sb.Append("c");
                            TransformPoints(points);
                            AppendRelativePoints(sb, points, currPoint, 3);
                            currPoint = points[2];
                        }
                        break;
                    case Renderers.Elements.Path.PathType.ArcTo:
                        {
                            var points = pelem.Points;
                            sb.Append("a");
                            TransformPoints(points);
                            ArcTo e = (ArcTo)pelem;
                            var size = transform == null ? e.Size : new Size(e.Size.Width * transform.Value.M11, e.Size.Height * transform.Value.M22);
                            sb.Append(FormatDecimal(size.Width));
                            sb.Append(' ');
                            sb.Append(FormatDecimal(size.Height));
                            sb.Append(' ');
                            sb.Append(FormatDecimal(Vectors.RadianToDegree(e.RotationAngle)));
                            sb.Append(' ');
                            sb.Append(e.IsLargeArc ? '1' : '0');
                            sb.Append(' ');
                            sb.Append((int)e.SweepDirection);
                            sb.Append(' ');
                            AppendRelativePoints(sb, points, currPoint, 1);
                            currPoint = points[0];
                        }
                        break;
                }
            }
            sb.Append("'");
            if (elem.fill)
            {
                sb.Append(" stroke='none'");
                if (defaultFill == null || !defaultFill.Equals(elem.color))
                    sb.Append(" fill='").Append(ToString(elem.color)).Append("'");
            }
            else
            {
                sb.Append(" fill='none'");
                sb.Append(" stroke='").Append(ToString(elem.color)).Append("'");
                sb.Append(" stroke-width='").Append(ToString(Scaled(elem.stroke))).Append("'");
            }
            sb.Append("/>\n");
        }

        private void Visit(LineElement elem)
        {
            Visit(null, null, elem);
        }

        private void Visit(string id, string cls, LineElement elem)
        {
            var points = new Point[] { elem.firstPoint, elem.secondPoint };
            TransformPoints(points);
            AppendIdent();
            sb.Append("<line");
            if (id != null) sb.Append(" id='").Append(id).Append("'");
            if (cls != null) sb.Append(" class='").Append(cls).Append("'");
            sb.Append(" x1='").Append(ToString(points[0].X)).Append("'")
              .Append(" y1='").Append(ToString(points[0].Y)).Append("'")
              .Append(" x2='").Append(ToString(points[1].X)).Append("'")
              .Append(" y2='").Append(ToString(points[1].Y)).Append("'");
            if (defaultStroke == null || !defaultStroke.Equals(elem.color))
                sb.Append(" stroke='").Append(ToString(elem.color)).Append("'");
            if (defaultStroke == null || !defaultStrokeWidth.Equals(ToString(Scaled(elem.width))))
                sb.Append(" stroke-width='").Append(ToString(Scaled(elem.width))).Append("'");
            sb.Append("/>\n");
        }

        private void Visit(MarkedElement elem)
        {
            string id = elem.Id;
            var classes = elem.GetClasses();
            string cls = !classes.Any() ? null : string.Join(" ", classes);

            IRenderingElement marked = elem.Element();

            // unpack singletons
            while (marked is ElementGroup)
            {
                var iter = ((ElementGroup)marked).GetEnumerator();
                if (!iter.MoveNext())
                    marked = null;
                else
                {
                    marked = iter.Current;
                    if (iter.MoveNext())
                        marked = null; // non-singleton
                }
            }

            if (marked == null)
                marked = elem.Element();

            // we try to
            if (marked is LineElement)
            {
                Visit(id, cls, (LineElement)marked);
            }
            else if (marked is GeneralPath)
            {
                Visit(id, cls, (GeneralPath)marked);
            }
            else
            {
                AppendIdent();
                sb.Append("<g");
                if (id != null)
                    sb.Append(" id='").Append(elem.Id).Append("'");
                if (cls != null)
                    sb.Append(" class='").Append(cls).Append("'");
                sb.Append(">\n");
                indentLvl += 2;
                Visit(marked);
                indentLvl -= 2;
                AppendIdent();
                sb.Append("</g>\n");
            }
        }

        private void Visit(RectangleElement elem)
        {
            AppendIdent();
            var points = new Point[] { elem.coord };
            TransformPoints(points);
            sb.Append("<rect");
            sb.Append(" x='").Append(ToString(points[0].X)).Append("'");
            sb.Append(" y='").Append(ToString(points[0].Y - elem.height)).Append("'");
            sb.Append(" width='").Append(ToString(Scaled(elem.width))).Append("'");
            sb.Append(" height='").Append(ToString(Scaled(elem.height))).Append("'");
            if (elem.filled)
            {
                sb.Append(" fill='").Append(ToString(elem.color)).Append("'");
                sb.Append(" stroke='none'");
            }
            else
            {
                sb.Append(" fill='none'");
                sb.Append(" stroke='").Append(ToString(elem.color)).Append("'");
            }
            sb.Append("/>\n");
        }

        private void Visit(OvalElement elem)
        {
            AppendIdent();
            var points = new Point[] { elem.coord };
            TransformPoints(points);
            sb.Append("<ellipse");
            sb.Append(" cx='").Append(ToString(points[0].X)).Append("'");
            sb.Append(" cy='").Append(ToString(points[0].Y)).Append("'");
            sb.Append(" rx='").Append(ToString(Scaled(elem.radius))).Append("'");
            sb.Append(" ry='").Append(ToString(Scaled(elem.radius))).Append("'");
            if (elem.fill)
            {
                sb.Append(" fill='").Append(ToString(elem.color)).Append("'");
                sb.Append(" stroke='none'");
            }
            else
            {
                sb.Append(" fill='none'");
                sb.Append(" stroke='").Append(ToString(elem.color)).Append("'");
            }
            sb.Append("/>\n");
        }

        private void Visit(TextElement elem)
        {
            AppendIdent();
            Point[] points = new Point[] { elem.coord };
            TransformPoints(points);
            sb.Append("<text ");
            sb.Append(" x='").Append(ToString(points[0].X)).Append("'");
            sb.Append(" y='").Append(ToString(points[0].Y)).Append("'");
            sb.Append(" fill='").Append(ToString(elem.color)).Append("'");
            sb.Append(" text-anchor='middle'");
            // TODO need font manager for scaling...
            sb.Append(">");
            sb.Append(System.Security.SecurityElement.Escape(elem.text));
            sb.Append("</text>\n");
        }

        public void Visit(IRenderingElement root)
        {
            if (!defaultsWritten)
            {
                AppendIdent();
                sb.Append("<g")
                  .Append(" stroke-linecap='round'")
                  .Append(" stroke-linejoin='round'");
                if (defaultStroke != null)
                    sb.Append(" stroke='").Append(ToString(defaultStroke.Value)).Append("'");
                if (defaultStrokeWidth != null)
                    sb.Append(" stroke-width='").Append(defaultStrokeWidth).Append("'");
                if (defaultFill != null)
                    sb.Append(" fill='").Append(ToString(defaultFill.Value)).Append("'");
                sb.Append(">\n");
                indentLvl += 2;
                defaultsWritten = true;
            }

            Deque<IRenderingElement> queue = new ArrayDeque<IRenderingElement>
            {
                root
            };
            while (queue.Any())
            {
                IRenderingElement elem = queue.Poll();
                if (elem is ElementGroup)
                {
                    foreach (var child in (ElementGroup)elem)
                        queue.Add(child);
                }
                else if (elem is Bounds)
                {
                    queue.Add(((Bounds)elem).Root);
                }
                else if (elem is MarkedElement)
                {
                    if (model != null && model.GetV<bool>(typeof(RendererModel.MarkedOutput)))
                    {
                        Visit(((MarkedElement)elem));
                    }
                    else
                    {
                        Visit(((MarkedElement)elem).Element());
                    }
                }
                else if (elem is LineElement)
                {
                    Visit((LineElement)elem);
                }
                else if (elem is GeneralPath)
                {
                    Visit((GeneralPath)elem);
                }
                else if (elem is RectangleElement)
                {
                    Visit((RectangleElement)elem);
                }
                else if (elem is OvalElement)
                {
                    Visit((OvalElement)elem);
                }
                else if (elem is TextElement)
                {
                    Visit((TextElement)elem);
                }
                else
                {
                    Console.Error.WriteLine($"{elem.GetType().FullName} rendering element is not supported by this visitor, parts of the depiction may missing!");
                }
            }
        }

        public Transform Transform
        {
            get => this.transform;
            set => this.transform = value;
        }

        public override string ToString()
        {
            if (defaultsWritten)
                return sb.ToString() + "  </g>\n</svg>\n";
            return sb.ToString() + "</svg>\n";
        }

        private sealed class Counter
        {
            public int Count { get; set; } = 1;
        }

        private sealed class FreqMap<T> where T : struct
        {
            Dictionary<T, Counter> map = new Dictionary<T, Counter>();

            public FreqMap()
            {
            }

            public void Add(T obj)
            {
                if (!map.TryGetValue(obj, out Counter counter))
                {
                    map[obj] = new Counter();
                }
                else
                {
                    counter.Count++;
                }
            }

            public T? GetMostFrequent()
            {
                if (!map.Any())
                {
                    return null;
                }
                else
                {
                    T? maxKey = null;
                    foreach (var e in map)
                    {
                        if (maxKey == null || e.Value.Count > map[maxKey.Value].Count)
                            maxKey = e.Key;
                    }
                    return maxKey;
                }
            }
        }
    }
}
