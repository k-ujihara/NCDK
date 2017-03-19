/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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
 using System;
using NCDK.Numerics;

namespace NCDK
{
    /// <summary>
    /// Represents the idea of an chemical atom.
    /// </summary>
    // @author      egonw
    // @cdk.created 2005-08-24
    // @cdk.keyword atom
    // @cdk.githash
    public interface IAtom
        : IAtomType
    {
        /// <summary>
        /// The partial charge of this atom.
        /// </summary>
        double? Charge { get; set; }

        /// <summary>
        /// The implicit hydrogen count of this atom.
        /// </summary>
        int? ImplicitHydrogenCount { get; set; }

        /// <summary>
        /// A point specifying the location of this atom in a 2D space.
        /// </summary>
        Vector2? Point2D { get; set; }

        /// <summary>
        /// A point specifying the location of this atom in 3D space.
        /// </summary>
        Vector3? Point3D { get; set; }

        /// <summary>
        /// A point specifying the location of this atom in a Crystal unit cell.
        /// </summary>
        Vector3? FractionalPoint3D { get; set; }

        /// <summary>
        /// The stereo parity of this atom. It uses the predefined values found in CDKConstants.
        /// </summary>
        [Obsolete("Use " + nameof(IStereoElement) + " for storing stereochemistry")]
        int? StereoParity { get; set; }

        /// <summary>
        /// A way for the Smiles parser to indicate that this atom was written with a lower case letter, e.g. 'c' rather than 'C'.
        /// </summary>
        bool IsSingleOrDouble { get; set; }
    }
}
