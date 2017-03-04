/*
 * Copyright (C) 2017 Kazuya Ujihara 
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
using System.Windows.Media;
using static NCDK.Renderers.Generators.Standards.VecmathUtil;

namespace NCDK.Renderers.Elements.Path
{
    /// <summary>
    /// A arc element in the path.
    /// </summary>
    public class ArcTo : PathElement
    {
        /// <summary>The point to make an arc to.</summary>
        public Point coord;

        public Size Size { get; set; }
        public double RotationAngle { get; set; }
        public bool IsLargeArc { get; set; }
        public SweepDirection SweepDirection { get; set; }

        /// <summary>
        /// Make an arc to this point.
        /// </summary>
        /// <param name="point">the endpoint of the arc</param>
        public ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection)
            : base(PathType.ArcTo)
        {
            this.coord = point;
            Size = size;
            IsLargeArc = isLargeArc;
            SweepDirection = sweepDirection;
        }

        /// <inheritdoc/>
        public override Point[] Points
        {
            get { return new[] { coord }; }
            set { coord = value[0]; }
        }
    }
}
