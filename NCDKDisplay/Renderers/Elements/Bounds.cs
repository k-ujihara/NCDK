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
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WPF = System.Windows;

namespace NCDK.Renderers.Elements
{
    /// <summary>
    /// Defines a bounding box element which the renderer can use to determine the true
    /// drawing limits. Using only atom coordinates adjuncts (e.g. hydrogen labels)
    /// may be truncated. If a generator provide a bounding box element, then the
    /// min/max bounds of all bounding boxes are utilised.
    /// </summary>
    // @author John May
    // @cdk.module renderbasic
    // @cdk.githash
    public sealed class Bounds : IRenderingElement
    {
        /// <summary>
        /// Minimum x/y coordinates.
        /// </summary>
        public double minX, minY;

        /// <summary>
        /// Maximum x/y coordinates.
        /// </summary>
        public double maxX, maxY;

        /// <summary>
        /// Know which elements are within this bound box.
        /// </summary>
        private readonly ElementGroup elements = new ElementGroup();

        /// <summary>
        /// Specify the min/max coordinates of the bounding box.
        /// </summary>
        /// <param name="x1">min x coordinate</param>
        /// <param name="y1">min y coordinate</param>
        /// <param name="x2">max x coordinate</param>
        /// <param name="y2">max y coordinate</param>
        public Bounds(double x1, double y1, double x2, double y2)
        {
            this.minX = x1;
            this.minY = y1;
            this.maxX = x2;
            this.maxY = y2;
        }

        /// <summary>
        /// An empty bounding box.
        /// </summary>
        public Bounds()
            : this(double.MaxValue, double.MaxValue,
                   double.MinValue, double.MinValue)
        { }

        /// <summary>
        /// An bounding box around the specified element.
        /// </summary>
        public Bounds(IRenderingElement element)
            : this()
        {
            Add(element);
        }

        /// <summary>
        /// Add the specified element bounds.
        /// </summary>
        public void Add(IRenderingElement element)
        {
            elements.Add(element);
            Traverse(element);
        }

        /// <summary>
        /// Ensure the point x,y is included in the bounding box.
        /// </summary>
        /// <param name="p">coordinate</param>
        public void Add(Point p)
        {
            if (p.X < minX) minX = p.X;
            if (p.Y < minY) minY = p.Y;
            if (p.X > maxX) maxX = p.X;
            if (p.Y > maxY) maxY = p.Y;
        }

        /// <summary>
        /// Add one bounds to another.
        /// </summary>
        /// <param name="bounds">other bounds</param>
        public void Add(Bounds bounds)
        {
            if (bounds.minX < minX) minX = bounds.minX;
            if (bounds.minY < minY) minY = bounds.minY;
            if (bounds.maxX > maxX) maxX = bounds.maxX;
            if (bounds.maxY > maxY) maxY = bounds.maxY;
        }

        /// <summary>
        /// Add the provided general path to the bounding box.
        /// </summary>
        /// <param name="path">general path</param>
        private void Add(GeneralPath path)
        {
            var b = path.elements.Bounds;
            if (b.IsEmpty)
                return;
            Add(b.BottomLeft);
            Add(b.BottomRight);
            Add(b.TopLeft);
            Add(b.TopRight);
        }

        private void Traverse(IRenderingElement newElement)
        {
            Deque<IRenderingElement> stack = new Deque<IRenderingElement>();
            stack.Push(newElement);
            while (stack.Any())
            {
                var element = stack.Poll();
                switch (element)
                {
                    case Bounds e:
                        Add(e);
                        break;
                    case GeneralPath e:
                        Add(e);
                        break;
                    case LineElement lineElem:
                        var vec = lineElem.secondPoint - lineElem.firstPoint;
                        var ortho = new WPF::Vector(-vec.Y, vec.X);
                        ortho.Normalize();
                        vec.Normalize();
                        ortho *= lineElem.width / 2;  // stroke width
                        vec *= lineElem.width / 2;    // stroke rounded also makes line longer
                        Add(lineElem.firstPoint - vec + ortho);
                        Add(lineElem.secondPoint + vec + ortho);
                        Add(lineElem.firstPoint - vec - ortho);
                        Add(lineElem.secondPoint + vec - ortho);
                        break;
                    case ElementGroup elementGroup:
                        stack.AddRange(elementGroup);
                        break;
                    case MarkedElement e:
                        stack.Add(e.Element());
                        break;
                    default:
                        // ignored from bounds calculation, we don't really
                        // care but log we skipped it
                        Trace.TraceWarning($"{element.GetType()} not included in bounds calculation");
                        break;
                }
            }
        }

        /// <summary>
        /// Access the root rendering element, it contains all
        /// elements added to the bounds so far.
        /// </summary>
        /// <returns>root rendering element</returns>
        public IRenderingElement Root => elements;

        /// <summary>
        /// Specifies the width of the bounding box.
        /// </summary>
        /// <returns>the width of the bounding box</returns>
        public double Width => maxX - minX;

        /// <summary>
        /// Specifies the height of the bounding box.
        /// </summary>
        /// <returns>the height of the bounding box</returns>
        public double Height => maxY - minY;

        /// <summary>
        /// The bounds are empty and contain no elements.
        ///
        /// <returns>bounds are empty (true) or not (false)</returns>
        /// </summary>
        public bool IsEmpty() => minX > maxX || minY > maxY;

        public void Accept(IRenderingVisitor visitor, Transform transform)
        {
            visitor.Visit(this, transform);
        }

        public void Accept(IRenderingVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return "{{" + minX + ", " + minY + "} - {" + maxX + ", " + maxY + "}}";
        }
    }
}
