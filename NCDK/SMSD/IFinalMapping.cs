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
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NCDK.SMSD
{
    /// <summary>
    /// Interface for mappings.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Category("Legacy")]
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
    public interface IFinalMapping : IEnumerable<IDictionary<int, int>>
    {

        /// <summary>
        /// Adds mapping to the mapping list
        /// </summary>
        /// <param name="mapping">List of all MCS mapping between a given
        /// reactant and product</param>
        void Add(IDictionary<int, int> mapping);

        /// <summary>
        /// Sets mapping list
        /// <param name="list">List of all MCS mapping between a given
        /// reactant and product</param>
        /// </summary>
        void Set(IList<IDictionary<int, int>> list);

        /// <summary>
        /// clear the mapping
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns the stored mappings
        /// </summary>
        /// <returns>get of MCS mapping List</returns>
        IList<IDictionary<int, int>> GetFinalMapping();

        /// <summary>
        /// Returns number of stored mappings
        /// </summary>
        /// <returns>size of the mapping</returns>
        int Count { get; }
    }
}
