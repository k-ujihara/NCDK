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
using System.Collections.Generic;
using System.Text;

namespace NCDK.QSAR.Result
{
    /// <summary>
    // @cdk.module standard
    // @cdk.githash
    /// </summary>
    public class DoubleArrayResult : DoubleArrayResultType
    {
        private List<double> array;

        public DoubleArrayResult()
            : base(0)
        {
            this.array = new List<double>();
        }

        public DoubleArrayResult(int size)
            : base(size)
        {
            this.array = new List<double>(size);
        }

        public void Add(double value)
        {
            array.Add(value);
        }

        public double this[int index]
        {
            get
            {
                if (index >= this.array.Count)
                {
                    return 0.0;
                }
                return this.array[index];
            }
        }

        public override int Length => Math.Max(base.Length, this.array.Count);

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < Length; i++)
            {
                buf.Append(this[i]);
                if (i + 1 < Length) buf.Append(',');
            }
            return buf.ToString();
        }
    }
}
