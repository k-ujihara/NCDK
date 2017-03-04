/* Copyright (C) 2010  Mark Rijnbeek <markr@ebi.ac.uk>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    /// Captures query bond types defined in the CTFile.
    /// </summary>
    // @cdk.module  isomorphism
    // @cdk.githash
    public class CTFileQueryBond : Default.Bond, IQueryBond // use Default.Bond instead of QueryBond
    {
        /// <summary>
        /// Bond types, as stated in the CTFile manual
        /// </summary>
        public enum BondType
        {
            Unset = 0,
            Single = 1,
            Double = 2,
            Triple = 3,
            Aromatic = 4,
            SingleOrDouble = 5,
            SingleOrAromatic = 6,
            DoubleOrAromatic = 7,
            Any = 8,
        }

        public CTFileQueryBond(IChemObjectBuilder builder)
        {
            this.Builder = builder;
        }

        /// <summary>
        /// The type of this bond.
        /// </summary>
        public CTFileQueryBond.BondType Type { get; set; } = BondType.Unset;

        public bool Matches(IBond bond)
        {
            return false;
        }

        /// <summary>
        /// Create a CTFileQueryBond of the specified type (from the MDL spec). The
        /// bond copies the atoms and sets the type using the value 'type', 5 = single
        /// or double, 8 = any, etc.
        ///
        /// <param name="bond">an existing bond</param>
        /// <param name="type">the specified type</param>
        /// <returns>a new CTFileQueryBond</returns>
        /// </summary>
        public static CTFileQueryBond OfType(IBond bond, int type)
        {
            CTFileQueryBond queryBond = new CTFileQueryBond(bond.Builder);
            queryBond.Order = BondOrder.Unset;
            queryBond.SetAtoms(bond.Atoms);
            switch (type)
            {
                case 1:
                    queryBond.Type = BondType.Single;
                    break;
                case 2:
                    queryBond.Type = BondType.Double;
                    break;
                case 3:
                    queryBond.Type = BondType.Triple;
                    break;
                case 4:
                    queryBond.Type = BondType.Aromatic;
                    break;
                case 5:
                    queryBond.Type = BondType.SingleOrDouble;
                    break;
                case 6:
                    queryBond.Type = BondType.SingleOrAromatic;
                    break;
                case 7:
                    queryBond.Type = BondType.DoubleOrAromatic;
                    break;
                case 8:
                    queryBond.Type = BondType.Any;
                    break;
                default:
                    throw new ArgumentException("Unknown bond type: " + type);
            }
            return queryBond;
        }
    }
}
