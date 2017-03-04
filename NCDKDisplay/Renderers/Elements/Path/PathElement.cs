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

using System.Windows;

namespace NCDK.Renderers.Elements.Path
{
    /// <summary>
    /// A path element.
    /// </summary>
    // @author Arvid
    // @cdk.module renderbasic
    // @cdk.githash
    public abstract class PathElement
    {
        /// <summary>the type of the path element.</summary>
        public readonly PathType type;

        /// <summary>
        /// Create a path element.
        /// </summary>
        /// <param name="type"><see cref="PathType"/> of this path element</param>
        public PathElement(PathType type)
        {
            this.type = type;
        }

        /// <summary>
        /// Get the type of the path element.
        /// </summary>
        /// <returns>the type of the path element</returns>
        public PathType Type => type;

        /// <summary>
        /// The provided array with the specified coordinates (length = 3) of this path element.
        /// </summary>
        public abstract Point[] Points { get; set; }
    }
}
