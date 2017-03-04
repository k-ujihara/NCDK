/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.ne>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;

namespace NCDK.QSAR.Result
{
    /// <summary>
    /// IDescriptorResult type for bools.
    ///
    // @cdk.module standard
    // @cdk.githash
    /// </summary>
    [Serializable]
    public class BooleanResultType : IDescriptorResult
    {
        public override string ToString()
        {
            return "BooleanResultType";
        }

        public int Length => 1;
    }
}
