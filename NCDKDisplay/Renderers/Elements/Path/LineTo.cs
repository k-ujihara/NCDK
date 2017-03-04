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
using System.Windows;
using static NCDK.Renderers.Generators.Standards.VecmathUtil;

namespace NCDK.Renderers.Elements.Path
{
    /// <summary>
    /// A line element in the path.
    /// </summary>
    // @author Arvid
    // @cdk.module renderbasic
    // @cdk.githash
    public class LineTo : PathElement
    {
        /// <summary>The point to make a line to.</summary>
        public Point coord;

        /// <summary>
        /// Make a line to this point.
        /// </summary>
        /// <param name="point">the endpoint of the line</param>
        public LineTo(Vector2 point)
            : this(ToPoint(point))
        { }

        public LineTo(Point point)
            : base(PathType.LineTo)
        {
            this.coord = point;
        }

        /// <summary>
        /// Make a line path element.
        /// </summary>
        /// <param name="coords">the coordinate in index 0</param>
        public LineTo(Point[] coords)
            : this(coords[0])
        {
        }

        /// <inheritdoc/>
        public override Point[] Points
        {
            get { return new[] { coord }; }
            set { coord = value[0]; }
        }
    }
}
