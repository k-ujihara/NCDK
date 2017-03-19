/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
    /// Object that provides access to the calculated descriptor value.
    /// </summary>
    // @cdk.module standard
    // @cdk.githash
    [Serializable]
    public class IntegerResult : IDescriptorResult
    {
        public int Value { get; private set; }

        public IntegerResult(int value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();

        public int Length => 1;
    }
}
