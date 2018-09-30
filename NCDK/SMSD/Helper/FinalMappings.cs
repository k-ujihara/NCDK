/*
 *
 * Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute iterator and/or
 * modify iterator under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that iterator will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
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
using System.Runtime.CompilerServices;
namespace NCDK.SMSD.Helper
{
    /// <summary>
    /// Class that stores raw Mapping(s) after each algorithm is executed.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public class FinalMappings : IFinalMapping
    {
        private static List<IReadOnlyDictionary<int, int>> mappings = null;
        private static FinalMappings instance = null;

        protected internal FinalMappings()
        {
            mappings = new List<IReadOnlyDictionary<int, int>>();
        }

        /// <summary>
        /// Stores mapping solutions
        /// </summary>
        public static FinalMappings Instance
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (instance == null)
                {
                    instance = new FinalMappings();
                }
                return instance;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(IReadOnlyDictionary<int, int> mapping)
        {
            mappings.Add(mapping);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="list">list of mappings</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Set(IList<IReadOnlyDictionary<int, int>> list)
        {
            this.Clear();
            mappings.AddRange(list);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Clear()
        {
            mappings.Clear();
        }

        public IReadOnlyList<IReadOnlyDictionary<int, int>> GetFinalMapping()
        {
            return mappings;
        }

        public IEnumerator<IReadOnlyDictionary<int, int>> GetEnumerator()
        {
            return mappings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public int Count => mappings.Count;
    }
}
