















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara

/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Default;
using System.Text;

namespace NCDK.Default
{
    /**
	 * Holder for secundary protein structure elements. Lously modeled after
	 * the Jmol Structure.java.
	 *
	 * @author     egonw
	 *
	 * @cdk.module data
	 * @cdk.githash
	 */
    public class PDBStructure : ChemObject, IPDBStructure
    {
        public const string HELIX = "helix";
        public const string SHEET = "sheet";
        public const string TURN = "turn";

        /// <summary>
        /// Structure Type of this structure.
        /// </summary>
        public string StructureType { get; set; }

        /// <summary>
        /// start Chain identifier of this structure.
        /// </summary>
        public char? StartChainID { get; set; }

        /// <summary>
        /// Start sequence number of this structure.
        /// </summary>
        public int? StartSequenceNumber { get; set; }

        /// <summary>
        /// Start Code for insertion of residues of this structure.
        /// </summary>
        public char? StartInsertionCode { get; set; }

        /// <summary>
        /// The ending Chain identifier of this structure.
        /// </summary>
        public char? EndChainID { get; set; }

        /// <summary>
        /// The ending sequence number of this structure.
        /// </summary>
        public int? EndSequenceNumber { get; set; }

        /// <summary>
        /// The ending Code for insertion of residues of this structure.
        /// </summary>
        public char? EndInsertionCode { get; set; }
    }
}
namespace NCDK.Silent
{
    /**
	 * Holder for secundary protein structure elements. Lously modeled after
	 * the Jmol Structure.java.
	 *
	 * @author     egonw
	 *
	 * @cdk.module data
	 * @cdk.githash
	 */
    public class PDBStructure : ChemObject, IPDBStructure
    {
        public const string HELIX = "helix";
        public const string SHEET = "sheet";
        public const string TURN = "turn";

        /// <summary>
        /// Structure Type of this structure.
        /// </summary>
        public string StructureType { get; set; }

        /// <summary>
        /// start Chain identifier of this structure.
        /// </summary>
        public char? StartChainID { get; set; }

        /// <summary>
        /// Start sequence number of this structure.
        /// </summary>
        public int? StartSequenceNumber { get; set; }

        /// <summary>
        /// Start Code for insertion of residues of this structure.
        /// </summary>
        public char? StartInsertionCode { get; set; }

        /// <summary>
        /// The ending Chain identifier of this structure.
        /// </summary>
        public char? EndChainID { get; set; }

        /// <summary>
        /// The ending sequence number of this structure.
        /// </summary>
        public int? EndSequenceNumber { get; set; }

        /// <summary>
        /// The ending Code for insertion of residues of this structure.
        /// </summary>
        public char? EndInsertionCode { get; set; }
    }
}
