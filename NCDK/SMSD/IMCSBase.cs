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
using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NCDK.SMSD
{
    /// <summary>
    /// Interface that holds basic core interface for all MCS algorithm.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Category("Legacy")]
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
    public interface IMCSBase
    {
        /// <summary>
        /// Initialise the query and target molecule.
        /// </summary>
        /// <param name="source">source molecule</param>
        /// <param name="target">target molecule</param>
        /// <exception cref="CDKException"></exception>
        void Set(MolHandler source, MolHandler target);

        /// <summary>
        /// Initialise the query and target molecule.
        /// </summary>
        /// <param name="source">source molecule</param>
        /// <param name="target">target molecule</param>
        /// <exception cref="CDKException"></exception>
        void Set(IQueryAtomContainer source, IAtomContainer target);

        /// <summary>
        /// Returns all plausible mappings between query and target molecules.
        /// Each map in the list has atom-atom equivalence of the mappings
        /// between query and target molecule i.e. map.Key for the query
        /// and map.Value for the target molecule
        /// </summary>
        /// <returns>All possible MCS atom Mappings</returns>
        IList<IDictionary<IAtom, IAtom>> GetAllAtomMapping();

        /// <summary>
        /// Returns all plausible mappings between query and target molecules.
        /// Each map in the list has atom-atom equivalence index of the mappings
        /// between query and target molecule i.e. map.Key for the query
        /// and map.Value for the target molecule
        /// </summary>
        /// <returns>All possible MCS Mapping Index</returns>
        IList<IDictionary<int, int>> GetAllMapping();

        /// <summary>
        /// Returns one of the best matches with atoms mapped.
        /// </summary>
        /// <returns>Best Atom Mapping</returns>
        IDictionary<IAtom, IAtom> GetFirstAtomMapping();

        /// <summary>
        /// Returns one of the best matches with atom indexes mapped.
        /// </summary>
        /// <returns>Best Mapping Index</returns>
        IDictionary<int, int> GetFirstMapping();
    }
}

