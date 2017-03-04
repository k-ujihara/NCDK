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
    /// A cubic curve in the path.
    /// </summary>
    // @author Arvid
    // @cdk.module renderbasic
    // @cdk.githash
    public class CubicTo : PathElement
    {
        /// <summary>Coordinates of control point 1, control point 2 and end point.</summary>
        public Point coord1;
        public Point coord2;
        public Point coord3;

        /// <summary>
        /// Make a cubic curve path element.
        /// </summary>
        /// <param name="cp1">first control point in the cubic</param>
        /// <param name="cp2">second control point in the cubic</param>
        /// <param name="ep">end point of the cubic</param>
        public CubicTo(Vector2 cp1, Vector2 cp2, Vector2 ep)
			: this(ToPoint(cp1), ToPoint(cp2), ToPoint(ep))
        { }

        /// <summary>
        /// Make a cubic curve path element.
        /// </summary>
        /// <param name="cp1">first control point in the cubic</param>
        /// <param name="cp2">second control point in the cubic</param>
        /// <param name="ep">end point of the cubic</param>
        public CubicTo(Point cp1, Point cp2, Point ep)
            : base(PathType.CubicTo)
        {
            coord1 = cp1;
            coord2 = cp2;
            coord3 = ep;
        }

        /// <summary>
        /// Make a cubic curve path element.
        /// </summary>
        /// <param name="coords">[0,1] : control point 1, [2,3] : control point 2, [4,5] end point</param>
        public CubicTo(Point[] coords)
            : this(coords[0], coords[1], coords[2])
        { }

        public override Point[] Points
        {
            get { return new[] { coord1, coord2, coord3 }; }
            set
            {
                coord1 = value[0];
                coord2 = value[1];
                coord3 = value[2];
            }
        }
    }
}
