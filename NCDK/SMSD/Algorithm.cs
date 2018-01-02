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

using System.Reflection;

namespace NCDK.SMSD
{
    /// <summary>
    /// This class represents various algorithm type supported by SMSD.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public enum Algorithm
    {
        /// <summary>
        /// Default SMSD algorithm.
        /// </summary>
        Default = 0,

        /// <summary>
        /// MCS Plus algorithm.
        /// </summary>
        MCSPlus = 1,

        /// <summary>
        /// VF Lib based MCS algorithm.
        /// </summary>
        VFLibMCS = 2,

        /// <summary>
        /// CDK UIT MCS.
        /// </summary>
        CDKMCS = 3,

        /// <summary>
        /// Substructure search will return all maps.
        /// </summary>
        SubStructure = 4,

        /// <summary>
        /// Substructure search will return first map.
        /// </summary>
        TurboSubStructure = 5,
    }

    public static class AlgorithmTools
    {
        private static readonly string[] descriptions = new[]
        {
            "Default SMSD algorithm",
            "MCS Plus algorithm",
            "VF Lib based MCS algorithm",
            "CDK UIT MCS",
            "Substructure search",
            "Turbo Mode: Substructure search",
        };

        ///// <summary>
        ///// type of algorithm.
        ///// </summary>
        //public int Type => Ordinal;

        /// <summary>
        /// short description of the algorithm.
        /// </summary>
        public static string Description(this Algorithm value)
            => descriptions[(int)value];
    }
}
