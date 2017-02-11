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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NCDK.QSAR.Result
{
    // @cdk.module standard
    // @cdk.githash
    public class IntegerArrayResult : IntegerArrayResultType, IList<int>
    {
        private List<int> array;

        public IntegerArrayResult()
            : base(0)
        {
            this.array = new List<int>();
        }

        public IntegerArrayResult(int size)
            : base(size)
        {
            this.array = new List<int>(size);
        }

        public void Add(int value)
        {
            array.Add(value);
        }

        public int this[int index]
        {
            get
            {
                if (index >= this.array.Count)
                {
                    return 0;
                }
                return this.array[index];
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public override int Length => Math.Max(base.Length, this.array.Count);
        public int Count => Length;
        public bool IsReadOnly => true;

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

        public int IndexOf(int item) => array.IndexOf(item);
        public void Insert(int index, int item) { throw new InvalidOperationException(); }
        public void RemoveAt(int index) { throw new InvalidOperationException(); }
        public void Clear() { throw new InvalidOperationException(); }
        public bool Contains(int item) => array.Contains(item);
        public void CopyTo(int[] array, int arrayIndex) { throw new InvalidOperationException(); }
        public bool Remove(int item) { throw new InvalidOperationException(); }

        public IEnumerator<int> GetEnumerator() => array.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

