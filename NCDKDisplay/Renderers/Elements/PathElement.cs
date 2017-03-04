/*  Copyright (C) 2008  Gilleain Torrance <gilleain.torrance@gmail.com>
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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace NCDK.Renderers.Elements
{
    /// <summary>
    /// A path composed of points.
    /// </summary>
    // @cdk.module renderbasic
    // @cdk.githash
    public class PathElement : IRenderingElement
    {
        /// <summary>The points that make up the path. </summary>
        public readonly IList<Point> points;

        /// <summary>The color of the path. </summary>
        public readonly Color color;

        /// <summary>
        /// Make a path from the list of points.
        /// </summary>
        /// <param name="points">points defining the path</param>
        /// <param name="color">color of the path</param>
        public PathElement(IList<Point> points, Color color)
        {
            this.points = points;
            this.color = color;
        }

        public virtual void Accept(IRenderingVisitor v)
        {
            v.Visit(this);
        }
    }
}
