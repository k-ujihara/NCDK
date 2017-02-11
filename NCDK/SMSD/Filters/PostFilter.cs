
using NCDK.Common.Collections;
using NCDK.SMSD.Helper;
using System.Collections.Generic;
/**
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
namespace NCDK.SMSD.Filters
{
    /**
     * Class that cleans redundant mappings from the solution set.
     * <OL>
     *
     * <lI>1: Stereo match, bond type, ring etc,
     * <lI>2: Fragment size,
     * <lI>3: Bond breaking energy
     *
     * </OL>
     * @cdk.module smsd
     * @cdk.githash
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     */
    public class PostFilter
    {

        /**
         *
         * Creates a new instance of Post Filter and removes
         * redundant Mapping(s).
         *
         * @param mappings
         * @return Filtered non-redundant mappings
         */
        public static IList<IDictionary<int, int>> Filter(IList<IList<int>> mappings)
        {
            FinalMappings finalMappings = FinalMappings.Instance;
            if (mappings != null && mappings.Count != 0)
            {
                finalMappings.Set(RemoveRedundantMapping(mappings));
                mappings.Clear();
            }
            else
            {
                finalMappings.Set(new List<IDictionary<int, int>>());
            }
            return finalMappings.GetFinalMapping();
        }

        private static DictionaryEqualityComparer<int, int> DictionaryEqualityComparer_int_int { get; } = new DictionaryEqualityComparer<int, int>();

        private static bool HasMap(IDictionary<int, int> newMap, List<IDictionary<int, int>> nonRedundantMapping)
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

        /**
         *
         * @param mappingOrg
         * @return
         */
        private static List<IDictionary<int, int>> RemoveRedundantMapping(IList<IList<int>> mappingOrg)
        {
            List<IDictionary<int, int>> nonRedundantMapping = new List<IDictionary<int, int>>();
            foreach (var mapping in mappingOrg)
            {
                IDictionary<int, int> newMap = GetMappingMapFromList(mapping);
                if (!HasMap(newMap, nonRedundantMapping))
                {
                    nonRedundantMapping.Add(newMap);
                }
            }
            return nonRedundantMapping;
        }

        private static IDictionary<int, int> GetMappingMapFromList(IList<int> list)
        {
            IDictionary<int, int> newMap = new SortedDictionary<int, int>();
            for (int index = 0; index < list.Count; index += 2)
            {
                newMap[list[index]] = list[index + 1];
            }
            return newMap;
        }
    }
}
