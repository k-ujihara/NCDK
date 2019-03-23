/*
 *
 * Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
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

using NCDK.Common.Collections;
using NCDK.SMSD.Helper;
using System;
using System.Collections.Generic;

namespace NCDK.SMSD.Filters
{
    /// <summary>
    /// Class that cleans redundant mappings from the solution set.
    /// <list type="bullet">
    /// <item>1: Stereo match, bond type, ring etc,</item>
    /// <item>2: Fragment size,</item>
    /// <item>3: Bond breaking energy</item>
    /// </list> 
    /// </summary>
    // @cdk.module smsd
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
    public static class PostFilter
    {
        /// <summary>
        /// Creates a new instance of Post Filter and removes
        /// redundant Mapping(s).
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns>Filtered non-redundant mappings</returns>
        public static IReadOnlyList<IReadOnlyDictionary<int, int>> Filter(IList<IReadOnlyList<int>> mappings)
        {
            FinalMappings finalMappings = FinalMappings.Instance;
            if (mappings != null && mappings.Count != 0)
            {
                finalMappings.Set(RemoveRedundantMapping(mappings));
                mappings.Clear();
            }
            else
            {
                finalMappings.Set(new List<IReadOnlyDictionary<int, int>>());
            }
            return finalMappings.GetFinalMapping();
        }

        private static DictionaryEqualityComparer<int, int> DictionaryEqualityComparer_int_int { get; } = new DictionaryEqualityComparer<int, int>();

        private static bool HasMap(IReadOnlyDictionary<int, int> newMap, IReadOnlyList<IReadOnlyDictionary<int, int>> nonRedundantMapping)
        {
            foreach (var storedMap in nonRedundantMapping)
            {
                if (DictionaryEqualityComparer_int_int.Equals(storedMap, newMap))
                {
                    return true;
                }
            }
            return false;
        }

        private static List<IReadOnlyDictionary<int, int>> RemoveRedundantMapping(IList<IReadOnlyList<int>> mappingOrg)
        {
            var nonRedundantMapping = new List<IReadOnlyDictionary<int, int>>();
            foreach (var mapping in mappingOrg)
            {
                var newMap = GetMappingMapFromList(mapping);
                if (!HasMap(newMap, nonRedundantMapping))
                {
                    nonRedundantMapping.Add(newMap);
                }
            }
            return nonRedundantMapping;
        }

        private static SortedDictionary<int, int> GetMappingMapFromList(IReadOnlyList<int> list)
        {
            var newMap = new SortedDictionary<int, int>();
            for (int index = 0; index < list.Count; index += 2)
            {
                newMap[list[index]] = list[index + 1];
            }
            return newMap;
        }
    }
}
