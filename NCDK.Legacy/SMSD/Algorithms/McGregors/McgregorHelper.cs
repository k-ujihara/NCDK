/* Copyright (C) 2006-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
using System.Collections.ObjectModel;

namespace NCDK.SMSD.Algorithms.McGregors
{
    /// <summary>
    /// Helper Class for McGregor algorithm.
    /// </summary>
    /// <remarks>
    /// The second part of the program extents the mapping by the McGregor algorithm in case,
    /// that not all atoms of molecule A and molecule B are mapped by the clique approach.
    /// </remarks>
    // @cdk.module smsd
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
    public class McgregorHelper
    {
        private readonly IList<string> cBondSetA;
        private readonly IList<string> cBondSetB;
        private readonly bool mappingCheckFlag;
        private readonly int mappedAtomCount;
        private readonly IList<int> mappedAtomsOrg;
        private readonly int neighborBondNumA;
        private readonly int neighborBondNumB;
        private readonly IList<int> iBondNeighborAtomsA;
        private readonly IList<int> iBondNeighborAtomsB;
        private readonly IList<string> cBondNeighborsA;
        private readonly IList<string> cBondNeighborsB;
        private readonly int setNumA;
        private readonly int setNumB;
        private readonly IList<int> iBondSetA;
        private readonly IList<int> iBondSetB;

        /// <summary>
        /// Stores the variables
        /// </summary>
        protected internal McgregorHelper(bool mappingCheckFlag, int mappedAtomCount, IList<int> mappedAtomsOrg,
                                 int neighborBondNumA, int neighborBondNumB, IList<int> iBondNeighborAtomsA,
                                 IList<int> iBondNeighborAtomsB, IList<string> cBondNeighborsA, IList<string> cBondNeighborsB, int setNumA,
                                 int setNumB, List<int> iBondSetA, IList<int> iBondSetB, IList<string> cBondSetA,
                                 IList<string> cBondSetB)
        {
            this.cBondSetA = cBondSetA;
            this.cBondSetB = cBondSetB;
            this.mappingCheckFlag = mappingCheckFlag;
            this.mappedAtomCount = mappedAtomCount;
            this.mappedAtomsOrg = mappedAtomsOrg;
            this.neighborBondNumA = neighborBondNumA;
            this.neighborBondNumB = neighborBondNumB;
            this.iBondNeighborAtomsA = iBondNeighborAtomsA;
            this.iBondNeighborAtomsB = iBondNeighborAtomsB;
            this.cBondNeighborsA = cBondNeighborsA;
            this.cBondNeighborsB = cBondNeighborsB;
            this.setNumA = setNumA;
            this.setNumB = setNumB;
            this.iBondSetA = iBondSetA;
            this.iBondSetB = iBondSetB;
        }

        /// <summary>
        /// <returns>the cBondSetA</returns>
        /// </summary>
        protected internal IList<string> GetCBondSetA()
        {
            return new ReadOnlyCollection<string>(cBondSetA);
        }

        /// <summary>
        /// <returns>the cBondSetB</returns>
        /// </summary>
        protected internal IList<string> GetCBondSetB()
        {
            return new ReadOnlyCollection<string>(cBondSetB);
        }

        /// <summary>
        /// <returns>the mappingCheckFlag</returns>
        /// </summary>
        protected internal bool IsMappingCheckFlag => mappingCheckFlag;

        /// <summary>
        /// <returns>the mappedAtomCount</returns>
        /// </summary>
        protected internal int MappedAtomCount => mappedAtomCount;

        /// <summary>
        /// <returns>the mappedAtomsOrg</returns>
        /// </summary>
        protected internal IList<int> GetMappedAtomsOrg()
        {
            return new ReadOnlyCollection<int>(mappedAtomsOrg);
        }

        /// <summary>
        /// <returns>the neighborBondNumA</returns>
        /// </summary>
        protected internal int NeighborBondNumA => neighborBondNumA;

        /// <summary>
        /// <returns>the neighborBondNumB</returns>
        /// </summary>
        protected internal int NeighborBondNumB => neighborBondNumB;

        /// <summary>
        /// <returns>the iBondNeighborAtomsA</returns>
        /// </summary>
        protected internal IList<int> GetIBondNeighborAtomsA()
        {
            return new ReadOnlyCollection<int>(iBondNeighborAtomsA);
        }

        /// <summary>
        /// <returns>the iBondNeighborAtomsB</returns>
        /// </summary>
        protected internal IList<int> GetIBondNeighborAtomsB()
        {
            return new ReadOnlyCollection<int>(iBondNeighborAtomsB);
        }

        /// <summary>
        /// <returns>the cBondNeighborsA</returns>
        /// </summary>
        protected internal IList<string> GetCBondNeighborsA()
        {
            return new ReadOnlyCollection<string>(cBondNeighborsA);
        }

        /// <summary>
        /// <returns>the cBondNeighborsB</returns>
        /// </summary>
        protected internal IList<string> GetCBondNeighborsB()
        {
            return new ReadOnlyCollection<string>(cBondNeighborsB);
        }

        /// <summary>
        /// <returns>the setNumA</returns>
        /// </summary>
        protected internal int SetNumA => setNumA;

        /// <summary>
        /// <returns>the iBondSetA</returns>
        /// </summary>
        protected internal IList<int> GetIBondSetA()
        {
            return new ReadOnlyCollection<int>(iBondSetA);
        }

        /// <summary>
        /// <returns>the iBondSetB</returns>
        /// </summary>
        protected internal IList<int> GetIBondSetB()
        {
            return new ReadOnlyCollection<int>(iBondSetB);
        }

        internal int SetNumB => setNumB;
    }
}
