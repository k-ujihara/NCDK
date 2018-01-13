/*  Copyright (C) 2008  Arvid Berg <goglepox@users.sf.net>
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

using System.Windows;
using System.Windows.Media;

namespace NCDK.Renderers.Elements
{
    /// <summary>
    /// A 'wedge' is a triangle aligned along a bond that indicates stereochemistry.
    /// It can be dashed or not to indicate up and down.
    /// </summary>
    // @cdk.module renderbasic
    // @cdk.githash
    public class WedgeLineElement : LineElement
    {
        /// <summary>
        /// If the bond is dashed ,wedged, or "up_or_down", i.e., not defined.
        /// </summary>
        public enum Types
        {
            Dashed, Wedged, Indiff
        }

        /// <summary>
        /// The type of the bond (dashed, wedged, not defined).
        /// </summary>
        public readonly Types type;

        /// <summary>
        /// The direction indicates which way the wedge gets thicker.
        /// </summary>
        public readonly Direction direction;

        /// <summary>
        /// 'toFirst' means that the wedge gets thicker in the direction of the first
        /// point in the line.
        /// </summary>
        public enum Direction
        {
            toFirst, toSecond,
        }

        /// <summary>
        /// Make a wedge between the points (x1, y1) and (x2, y2) with a certain
        /// width, direction, dash, and color.
        /// </summary>
        /// <param name="firstPoint">the coordinate of the first point</param>
        /// <param name="secondPoint">the coordinate of the second point</param>
        /// <param name="width">the width of the wedge</param>
        /// <param name="type">the bond is dashed ,wedged, or "up_or_down", i.e., not defined.</param>
        /// <param name="direction">the direction of the thickness</param>
        /// <param name="color">the color of the wedge</param>
        public WedgeLineElement(Point firstPoint, Point secondPoint, double width, Types type, Direction direction, Color color)
            : base(firstPoint, secondPoint, width, color)
        {
            this.type = type;
            this.direction = direction;
        }

        /// <summary>
        /// Make a wedge along the given line element.
        /// </summary>
        /// <param name="element">the line element to use as the basic geometry</param>
        /// <param name="type">if the bond is dashed ,wedged, or "up_or_down", i.e., not defined</param>
        /// <param name="direction">the direction of the thickness</param>
        /// <param name="color">the color of the wedge</param>
        public WedgeLineElement(LineElement element, Types type, Direction direction, Color color)
            : this(direction == Direction.toFirst ? element.secondPoint : element.firstPoint,
                direction == Direction.toFirst ? element.firstPoint : element.secondPoint,
                element.width, type, direction, color)
        { }

        /// <inheritdoc/>
        public override void Accept(IRenderingVisitor v)
        {
            v.Visit(this);
        }
    }
}
