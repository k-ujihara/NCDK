/* Copyright (C) 2006-2007,2010  Egon Willighagen <egonw@users.sf.net>
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

using NCDK.Dict;
using System;
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// Base class for all chemical objects that maintain a list of Atoms and
    /// ElectronContainers.
    /// </summary>
    /// <example>
    /// Looping over all <see cref="IBond"/>s in the <see cref="IAtomContainer"/> is typically done like:
    /// <code>
    /// foreach (var bond in atomContainer.Bonds) {
    ///     // do something
    /// }
    /// </code>
    /// </example>
    // @cdk.module interfaces
    // @cdk.githash
    // @author     steinbeck
    // @cdk.created    2000-10-02
    public interface IAtomContainer
        : IChemObject, IChemObjectListener
    {
        /// <summary>
        /// Sets the array of atoms of this AtomContainer.
        /// </summary>
        /// <param name="atoms">The array of atoms to be assigned to this AtomContainer</param>
        void SetAtoms(IEnumerable<IAtom> atoms);
        void SetBonds(IEnumerable<IBond> bonds);

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="SetAtoms(IEnumerable{IAtom})"/>
        IList<IAtom> Atoms { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="SetBonds(IEnumerable{IBond})"/>
        IList<IBond> Bonds { get; }
        IList<ILonePair> LonePairs { get; }
        IList<ISingleElectron> SingleElectrons { get; }
        IList<IStereoElement> StereoElements { get; }
        void SetStereoElements(IEnumerable<IStereoElement> elements);
        void AddStereoElement(IStereoElement element);

        bool IsSingleOrDouble { get; set; }
        bool IsAromatic { get; set; }

        IEnumerable<IElectronContainer> GetElectronContainers();

        IBond GetBond(IAtom atom1, IAtom atom2);
        IEnumerable<IAtom> GetConnectedAtoms(IAtom atom);
        IEnumerable<IBond> GetConnectedBonds(IAtom atom);
        IEnumerable<ILonePair> GetConnectedLonePairs(IAtom atom);
        IEnumerable<ISingleElectron> GetConnectedSingleElectrons(IAtom atom);
        IEnumerable<IElectronContainer> GetConnectedElectronContainers(IAtom atom);

        double GetBondOrderSum(IAtom atom);
        BondOrder GetMaximumBondOrder(IAtom atom);
        BondOrder GetMinimumBondOrder(IAtom atom);

        void Add(IStereoElement element);
        void Add(IAtomContainer atomContainer);
        void Add(IAtom atom);
        void Add(IBond bond);
        void Add(ILonePair lonePair);
        void Add(ISingleElectron singleElectron);
        void Add(IElectronContainer electronContainer);

        void Remove(IAtomContainer atomContainer);
        void Remove(IAtom atom);
        void Remove(IBond bond);
        void Remove(ILonePair lonePair);
        void Remove(ISingleElectron singleElectron);
        void Remove(IElectronContainer electronContainer);

        void RemoveAtomAndConnectedElectronContainers(IAtom atom);
        void RemoveAllElements();
        void RemoveAllElectronContainers();
        void RemoveAllBonds();

        void AddBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo);
        void AddBond(IAtom atom1, IAtom atom2, BondOrder order);

        /// <summary>
        /// Removes the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connectes the two atoms</returns>
        IBond RemoveBond(IAtom atom1, IAtom atom2);

        void AddLonePair(IAtom atom);
        void AddSingleElectron(IAtom atom);

        bool Contains(IAtom atom);
        bool Contains(IBond bond);
        bool Contains(ILonePair lonePair);
        bool Contains(ISingleElectron singleElectron);
        bool Contains(IElectronContainer electronContainer);

        bool IsEmpty { get; }
    }
}
