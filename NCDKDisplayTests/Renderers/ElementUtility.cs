/* Copyright (C) 2010  Gilleain Torrance <gilleain.torrance@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Renderers.Elements;
using NCDK.Renderers.Fonts;
using NCDK.Renderers.Visitors;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace NCDK.Renderers
{
    /// <summary>
    /// Utility class for testing.
    /// </summary>
    // @author     maclean
    // @cdk.module test-renderbasic
    public class ElementUtility 
        : IDrawVisitor
    {
        private List<IRenderingElement> elements = new List<IRenderingElement>();
        private Transform transform;
        private RendererModel model;
        private bool getElementGroups = false;

        public int NumberOfElements()
        {
            return this.elements.Count;
        }


        public void Visit(IRenderingElement element, Transform transform)
        {
            var save = transform;
            this.transform = transform;
            Visit(element);
            transform = save;
        }

        public void Visit(IRenderingElement element)
        {
            if (element is ElementGroup)
            {
                if (getElementGroups)
                {
                    this.elements.Add(element);
                }
                ((ElementGroup)element).Visit(this);
            }
            else if (element is MarkedElement)
            {
                Visit(((MarkedElement)element).Element());
            }
            else if (element is Bounds)
            {
                Visit(((Bounds)element).Root);
            }
            else
            {
                this.elements.Add(element);
            }
        }

        public IList<IRenderingElement> GetElements()
        {
            return this.elements;
        }

        public IList<IRenderingElement> GetAllSimpleElements(IRenderingElement root)
        {
            elements.Clear();
            getElementGroups = false;
            root.Accept(this);
            return new List<IRenderingElement>(elements);
        }

        public Point TransformPoint(Point p)
        {
            return transform.Transform(p);
        }

        public IFontManager FontManager { get => null; set { } }

        public RendererModel RendererModel
        {
            get => this.model;
            set => this.model = value;
        }

        public RendererModel GetModel()
        {
            return this.model;
        }

        public string ToString(int[] p)
        {
            return $"({p[0]}, {p[1]})";
        }

        public string ToString(Point p)
        {
            return ToString(p.X, p.Y);
        }

        public string ToString(double x, double y)
        {
            return $"({x.ToString("F1")}, {y.ToString("F1")})";
        }

        public string ToString(double x, double y, double r)
        {
            return $"({x.ToString("F1")}, {y.ToString("F1")}, {r.ToString("F1")})";
        }

        public string ToString(IRenderingElement element)
        {
            switch (element)
            {
                case LineElement e:
                    {
                        string p1 = ToString(e.firstPoint);
                        string p2 = ToString(e.secondPoint);
                        string p1T = ToString(TransformPoint(e.firstPoint));
                        string p2T = ToString(TransformPoint(e.secondPoint));
                        string lineFormat = "Line [%s, %s] -> [%s, %s]\n";
                        return string.Format(lineFormat, p1, p2, p1T, p2T);
                    }
                case OvalElement e:
                    {
                        double r = e.radius;
                        string c = ToString(e.coord.X, e.coord.Y, r);
                        string p1 = ToString(TransformPoint(new Point(e.coord.X - r, e.coord.Y - r)));
                        string p2 = ToString(TransformPoint(new Point(e.coord.X + r, e.coord.Y + r)));
                        return string.Format("Oval [%s] -> [%s, %s]\n", c, p1, p2);
                    }
                case AtomSymbolElement e:
                    return string.Format("AtomSymbol [%s]\n", e.text);
                case ElementGroup e:
                    return "Element Group\n";
                default:
                    return "Unknown element\n";
            }
        }

        public void PrintToStream(IRenderingElement root, TextWriter stream)
        {
            root.Accept(this);
            foreach (var element in this.elements)
            {
                stream.Write(ToString(element));
            }
        }
    }
}
