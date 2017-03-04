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
using NCDK.Numerics;
using System.Collections.Generic;
using System.Windows.Media;
using WPF = System.Windows;

namespace NCDK.Renderers.Elements.Path
{
    /// <summary>
    /// Builder class for paths. All methods for constructing path elements return
    /// a reference to the builder, so that it can be used like:
    /// <code>
    ///  PathBuilder builder = new PathBuilder();
    ///  builder.MoveTo(p1).LineTo(p1).Close();
    ///  GeneralPath path = builder.CreatePath();
    /// </code>
    /// </summary>
    // @author Arvid
    // @cdk.module renderbasic
    // @cdk.githash
    public class PathBuilder
    {
        /// <summary>The path that is being built</summary>
        private IList<PathElement> elements;

        /// <summary>The color of the path</summary>
        private Color color;

        /// <summary>
        /// Make a new path builder with a default color of black.
        /// </summary>
        public PathBuilder()
            : this(WPF.Media.Colors.Black)
        { }

        /// <summary>
        /// Make a path builder that will make a path with a particular color.
        /// </summary>
        /// <param name="color">the color of the path</param>
        public PathBuilder(Color color)
        {
            elements = new List<PathElement>();
            this.color = color;
        }

        /// <summary>
        /// Internal method that adds the element to the path.
        /// </summary>
        /// <param name="element">the element to add to the path</param>
        private void Add<T>(T element) where T : PathElement
        {
            elements.Add(element);
        }

        /// <summary>
        /// Make a move in the path, without drawing anything. This is usually used
        /// to start a path.
        /// </summary>
        /// <param name="point">the point to move to</param>
        /// <returns>a reference to this builder</returns>
        public PathBuilder MoveTo(Vector2 point)
        {
            Add(new MoveTo(point));
            return this;
        }

        /// <summary>
        /// Make a line in the path, from the last point to the given point.
        /// </summary>
        /// <param name="point">the point to make a line to</param>
        /// <returns>a reference to this builder</returns>
        public PathBuilder LineTo(Vector2 point)
        {
            Add(new LineTo(point));
            return this;
        }

        /// <summary>
        /// Make a quadratic curve in the path, with one control point.
        /// </summary>
        /// <param name="cp">the control point of the curve</param>
        /// <param name="ep">the end point of the curve</param>
        /// <returns>a reference to this builder</returns>
        public PathBuilder quadTo(Vector2 cp, Vector2 ep)
        {
            Add(new QuadTo(cp, ep));
            return this;
        }

        /// <summary>
        /// Make a cubic curve in the path, with two control points.
        /// </summary>
        /// <param name="cp1">the first control point</param>
        /// <param name="cp2">the second control point</param>
        /// <param name="ep">the end point of the curve</param>
        /// <returns>a reference to this builder</returns>
        public PathBuilder CubicTo(Vector2 cp1, Vector2 cp2, Vector2 ep)
        {
            Add(new CubicTo(cp1, cp2, ep));
            return this;
        }

        /// <summary>
        /// Close the path.
        /// </summary>
        public void Close()
        {
            Add(new Close());
        }

        /// <summary>
        /// The color if this path.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// Create and return the final path.
        /// </summary>
        /// <returns>the newly created path</returns>
        public GeneralPath CreatePath()
        {
            return new GeneralPath(elements, color);
        }
    }
}
