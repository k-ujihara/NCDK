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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Mathematics;
using System;
using System.Collections.Generic;
using NCDK.Numerics;

namespace NCDK.SGroups
{
    /// <summary>
    /// Representation of an Sgroup bracket.
    /// </summary>
    public class SgroupBracket
    {
        private Vector2 p1, p2;

        /// <summary>
        /// Create an Sgroup bracket.
        ///
        /// <param name="x1">first x coord</param>
        /// <param name="y1">first y coord</param>
        /// <param name="x2">second x coord</param>
        /// <param name="y2">second y coord</param>
        /// </summary>
        public SgroupBracket(double x1, double y1, double x2, double y2)
        {
            this.p1 = new Vector2(x1, y1);
            this.p2 = new Vector2(x2, y2);
        }

        /// <summary>
        /// First point of the bracket (x1,y1).
        ///
        /// <returns>first point</returns>
        /// </summary>
        public Vector2 FirstPoint
        {
            get { return p1; }
            set { p1 = value; }
        }

        /// <summary>
        /// Second point of the bracket (x2,y2).
        ///
        /// <returns>second point</returns>
        /// </summary>
        public Vector2 SecondPoint
        {
            get { return p2; }
            set { p2 = value; }
        }

        public override string ToString()
        {
            return "SgroupBracket{" +
                   "x1=" + p1.X +
                   ", y1=" + p1.Y +
                   ", x2=" + p2.X +
                   ", y2=" + p2.Y +
                   '}';
        }
    }
}
