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
using System.Linq;
using System.Text;

namespace NCDK.QSAR.Results
{
    public interface IArrayResult
        : IDescriptorResult
    {
    }

    public class ArrayResult<T>
        : IArrayResult, IReadOnlyList<T>
    {
        int size;
        List<T> array;

        public ArrayResult()
            : this(0)
        {
        }

        public ArrayResult(int size)
        {
            this.size = size;
            this.array = new List<T>(size);
        }

        public ArrayResult(IEnumerable<T> array)
            : this(array.Count())
        {
            this.array.AddRange(array);
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

        public int Length => Math.Max(this.size, this.array.Count);

        public int Count => Length;

        public override string ToString()
        {
            return string.Join(",", this.Select(n => n.ToString()));
        }

        public IEnumerator<T> GetEnumerator() => array.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
