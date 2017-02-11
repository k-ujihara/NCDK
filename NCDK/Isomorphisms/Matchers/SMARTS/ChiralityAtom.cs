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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /**
     * This matches an atom with chirality property. It is not implemented yet.
     * It'll match any atom right now.
     *
     * @cdk.module  smarts
     * @cdk.githash
     * @cdk.keyword SMARTS
     */
    public class ChiralityAtom : SMARTSAtom
    {
        /// <summary>
        /// The degree of the chirality
        /// </summary>
        public int Degree { get; set; }

        /// <summary>
        /// Whether unspecified chirality should be taken into consideration
        /// </summary>
        public bool IsUnspecified { get; set; }

        /// <summary>
        /// Whether the chirality is clockwise
        /// </summary>
        public bool IsClockwise { get; set; }

        /**
         * Creates a new instance
         *
         */
        public ChiralityAtom(IChemObjectBuilder builder)
            : base(builder)
        {
        }

        public override bool Matches(IAtom atom)
        {
            // match testing is done after the match is complete
            return true;
        }

        /// <inheritdoc/>
        public override bool ChiralityMatches(IAtom target, int tParity, int permParity)
        {
            int qParity = permParity * (IsClockwise ? 1 : -1);
            return IsUnspecified && tParity == 0 || qParity == tParity;
        }
    }
}
