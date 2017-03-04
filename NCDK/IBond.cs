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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */ 
using System;
using System.Collections.Generic;
using NCDK.Numerics;

namespace NCDK
{
    /// <summary>
    /// Implements the concept of a covalent bond between two or more atoms. A bond is
    /// considered to be a number of electrons connecting two ore more atoms.
    ///type filter text
    // @cdk.module interfaces
    // @cdk.githash
    ///
    // @author      egonw
    // @cdk.created 2005-08-24
    // @cdk.keyword bond
    // @cdk.keyword atom
    // @cdk.keyword electron
    /// </summary>
    public interface IBond
        : IElectronContainer, IMolecularEntity
    {
        IList<IAtom> Atoms { get; }

        /// <summary>
        /// Sets the array of atoms making up this bond.
        /// </summary>
        /// <param name="atoms">An array of atoms that forms this bond</param>
        void SetAtoms(IEnumerable<IAtom> atoms);

        /// <summary>
        /// Returns the atom connected to the given atom.
        /// </summary>
        /// <param name="atom">The atom the bond partner is searched of</param>
        /// <returns>the connected atom or <c>null</c> if the given atom is not part of the bond</returns>
        IAtom GetConnectedAtom(IAtom atom);

        /// <summary>
        /// Returns all the atoms in the bond connected to the given atom.
        /// </summary>
        /// <param name="atom">The atoms the bond partner is searched of</param>
        /// <returns>the connected atoms or <c>null</c> if the given atom is not part of the bond</returns>
        IEnumerable<IAtom> GetConnectedAtoms(IAtom atom);

        /// <summary>
        /// Returns true if the given atom participates in this bond.
        ///
        /// <param name="atom">The atom to be tested if it participates in this bond</param>
        /// <returns>true if the atom participates in this bond</returns>
        /// </summary>
        bool Contains(IAtom atom);

        /// <summary>
        /// The bond order of this bond.
        /// </summary>
        BondOrder Order { get; set; }

        /// <summary>
        /// The stereo descriptor for this bond.
        /// </summary>
        BondStereo Stereo { get; set; }

        /// <summary>
        /// The geometric 2D center of the bond.
        /// </summary>
        Vector2 Geometric2DCenter { get; }

        /// <summary>
        /// The geometric 3D center of the bond.
        /// </summary>
        Vector3 Geometric3DCenter { get; }

        /// <summary>
        /// Checks whether a bond is connected to another one. This can only be true if the bonds have an Atom in common.
        /// </summary>
        /// <param name="bond">The bond which is checked to be connect with this one</param>
        /// <returns><c>true</c>, if the bonds share an atom, otherwise <c>false</c></returns>
        bool IsConnectedTo(IBond bond);

        /// <summary>
        /// Flag used for marking uncertainty of the bond order.
        /// If used on an
        /// <ul>
        ///  <li><see cref="IAtomContainer"/> it means that one or several of the bonds have
        ///         this flag raised (which may indicate aromaticity).</li>
        ///  <li><see cref="IBond"/> it means that it's unclear whether the bond is a single or
        ///         double bond.</li>
        ///  <li><see cref="IAtom"/> it is a way for the Smiles parser to indicate that this atom was
        ///         written with a lower case letter, e.g. 'c' rather than 'C'</li>
        /// </ul>
        /// </summary>
        bool IsSingleOrDouble { get; set; }

        bool IsReactiveCenter { get; set; }
    }
}
