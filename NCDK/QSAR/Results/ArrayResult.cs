/* Copyright (C) 2004-2017  The Chemistry Development Kit (CDK) project
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NCDK.QSAR.Results
{
    public class ArrayResult<T>
        : IDescriptorResult, IReadOnlyList<T>
    {
        int size;
        List<T> array;

        public ArrayResult()
        {
            this.size = 0;
            this.array = new List<T>();
        }

        public ArrayResult(int size)
        {
            this.size = size;
            this.array = new List<T>(size);
        }

        public void Add(T value)
        {
            array.Add(value);
        }

        public T this[int index]
        {
            get
            {
                if (index >= this.array.Count)
                {
                    return default(T);
                }
                return this.array[index];
            }
        }

        public virtual int Length => Math.Max(this.size, this.array.Count);

        public int Count => Length;

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

        public IEnumerator<T> GetEnumerator() => array.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
