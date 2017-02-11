/* Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
using System.Collections.Generic;

namespace NCDK.SMSD
{
    /**
     * Interface for mappings.
     * @cdk.module smsd
     * @cdk.githash
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     */

    public interface IFinalMapping : IEnumerable<IDictionary<int, int>>
    {

        /**
         * Adds mapping to the mapping list
         * @param mapping List of all MCS mapping between a given
         * reactant and product
         */
        void Add(IDictionary<int, int> mapping);

        /**
         * Sets mapping list
         * @param list List of all MCS mapping between a given
         * reactant and product
         */
        void Set(IList<IDictionary<int, int>> list);

        /**
         * clear the mapping
         */
        void Clear();

        /**
         * Returns the stored mappings
         * @return get of MCS mapping List
         */
        IList<IDictionary<int, int>> GetFinalMapping();

        /**
         * Returns number of stored mappings
         * @return size of the mapping
         */
        int Count { get; }
    }
}
