

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <uzzy@users.sourceforge.net>

/* Copyright (C) 1997-2007  Christoph Steinbeck
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NCDK.Default
{
    /// <summary>
    /// Base class for all chemical objects that maintain a list of Atoms and
    /// ElectronContainers. 
    /// </summary>
    /// <example>
    /// Looping over all Bonds in the AtomContainer is typically done like: 
    /// <code>
    /// foreach (IBond aBond in atomContainer.Bonds)
    /// {
    ///         // do something
    /// }
    /// </code>
    /// </example>
    // @cdk.githash 
    // @author steinbeck
    // @cdk.created 2000-10-02 
    [Serializable]
    public class AtomContainer
        : ChemObject, IAtomContainer, IChemObjectListener
    {
        /// <summary>
        /// Atoms contained by this object.
        /// </summary>
        internal IList<IAtom> atoms;

        /// <summary>
        /// Bonds contained by this object.
        /// </summary>
        internal IList<IBond> bonds;

        /// <summary>
        /// Lone pairs contained by this object.
        /// </summary>
        internal IList<ILonePair> lonePairs;

        /// <summary>
        /// Single electrons contained by this object.
        /// </summary>
        internal IList<ISingleElectron> singleElectrons;

        /// <summary>
        /// Stereo elements contained by this object.
        /// </summary>
        internal IList<IStereoElement> stereoElements;

        internal bool isAromatic;
        internal bool isSingleOrDouble;

        private void Init(
            IList<IAtom> atoms,
            IList<IBond> bonds,
            IList<ILonePair> lonePairs,
            IList<ISingleElectron> singleElectrons,
            IList<IStereoElement> stereoElements)
        {
            this.atoms = atoms;
            this.bonds = bonds;
            this.lonePairs = lonePairs;
            this.singleElectrons = singleElectrons;
            this.stereoElements = stereoElements;
        }

        public AtomContainer(
            IEnumerable<IAtom> atoms,
            IEnumerable<IBond> bonds,
            IEnumerable<ILonePair> lonePairs,
            IEnumerable<ISingleElectron> singleElectrons,
            IEnumerable<IStereoElement> stereoElements)
        {
            Init(
                CreateObservableChemObjectCollection(atoms, false),
                CreateObservableChemObjectCollection(bonds, true),
                CreateObservableChemObjectCollection(lonePairs, true),
                CreateObservableChemObjectCollection(singleElectrons, true),
                new List<IStereoElement>(stereoElements)
            );
        }

        private ObservableChemObjectCollection<T> CreateObservableChemObjectCollection<T>(IEnumerable<T> objs, bool allowDup) where T : IChemObject
        {
 
            var list = new ObservableChemObjectCollection<T>(this, objs);
            list.AllowDuplicate = allowDup;
            return list;
        }

        public AtomContainer(
           IEnumerable<IAtom> atoms,
           IEnumerable<IBond> bonds)
             : this(
                  atoms,
                  bonds,
                  Array.Empty<ILonePair>(),
                  Array.Empty<ISingleElectron>(),
                  Array.Empty<IStereoElement>())
        { }

        /// <summary>
        ///  Constructs an empty AtomContainer.
        /// </summary>
        public AtomContainer()
            : this(
                      Array.Empty<IAtom>(), 
                      Array.Empty<IBond>(), 
                      Array.Empty<ILonePair>(),
                      Array.Empty<ISingleElectron>(),
                      Array.Empty<IStereoElement>())
        { }

        /// <summary>
        /// Constructs an AtomContainer with a copy of the atoms and electronContainers
        /// of another AtomContainer (A shallow copy, i.e., with the same objects as in
        /// the original AtomContainer).
        /// </summary>
        /// <param name="container">An AtomContainer to copy the atoms and electronContainers from</param>
        public AtomContainer(IAtomContainer container)
            : this(
                  container.Atoms,
                  container.Bonds,
                  container.LonePairs,
                  container.SingleElectrons,
                  container.StereoElements)
        { }

        /// <inheritdoc/>
        public virtual bool IsAromatic
        {
            get { return isAromatic; }
            set
            {
                isAromatic = value; 
                NotifyChanged();
            }
        }
        
        /// <inheritdoc/>
        public virtual bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set
            {
                isSingleOrDouble = value;
                NotifyChanged();
            }
        }

        /// <inheritdoc/>
        public virtual IList<IAtom> Atoms => atoms;

        /// <inheritdoc/>
        public virtual IList<IBond> Bonds => bonds;

        /// <inheritdoc/>
        public virtual IList<ILonePair> LonePairs => lonePairs;

        /// <inheritdoc/>
        public virtual IList<ISingleElectron> SingleElectrons => singleElectrons;

        /// <inheritdoc/>
        public virtual IList<IStereoElement> StereoElements => stereoElements;

        /// <inheritdoc/>
        public virtual void SetStereoElements(IEnumerable<IStereoElement> elements) => stereoElements = new List<IStereoElement>(elements);

        /// <summary>
        /// Returns the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connects the two atoms</returns>
        public virtual IBond GetBond(IAtom atom1, IAtom atom2)
        {
            return bonds.Where(bond => bond.Contains(atom1) && bond.GetConnectedAtom(atom1) == atom2).FirstOrDefault();
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
        {
            foreach (var bond in Bonds)
                if (bond.Contains(atom))
                    yield return bond.GetConnectedAtom(atom);
            yield break;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IBond> GetConnectedBonds(IAtom atom)
        {
            return bonds.Where(bond => bond.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ILonePair> GetConnectedLonePairs(IAtom atom)
        {
            return LonePairs.Where(lonePair => lonePair.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ISingleElectron> GetConnectedSingleElectrons(IAtom atom)
        {
            return SingleElectrons.Where(singleElectron => singleElectron.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IElectronContainer> GetConnectedElectronContainers(IAtom atom)
        {
            foreach (var e in GetConnectedBonds(atom))
                yield return e;
            foreach (var e in GetConnectedLonePairs(atom))
                yield return e;
            foreach (var e in GetConnectedSingleElectrons(atom))
                yield return e;
            yield break;
        }

        private IEnumerable<BondOrder> GetBondOrders(IAtom atom)
        {
            return bonds.Where(bond => bond.Contains(atom))
                .Select(bond => bond.Order)
                .Where(order => !order.IsUnset);
        }

        /// <inheritdoc/>
        public virtual double GetBondOrderSum(IAtom atom)
        {
            return GetBondOrders(atom).Select(order => order.Numeric).Sum();
        }

        /// <inheritdoc/>
        public virtual BondOrder GetMaximumBondOrder(IAtom atom)
        {
            var max = BondOrder.Single;
            foreach (var order in GetBondOrders(atom))
            {
                if (max.Numeric < order.Numeric)
                    max = order;
            }
            return max;
        }

        /// <inheritdoc/>
        public virtual BondOrder GetMinimumBondOrder(IAtom atom)
        {
            var min = BondOrder.Quadruple;
            foreach (var order in GetBondOrders(atom))
            {
                if (min.Numeric > order.Numeric)
                    min = order;
            }
            return min;
        }

        /// <inheritdoc/>
        public virtual void Add(IAtomContainer atomContainer)
        {
            foreach (var atom in atomContainer.Atoms.Where(atom => !Contains(atom)))
                Atoms.Add(atom);
            foreach (var bond in atomContainer.Bonds.Where(bond => !Contains(bond)))
                Bonds.Add(bond);
            foreach (var lonePair in atomContainer.LonePairs.Where(lonePair => !Contains(lonePair)))
                LonePairs.Add(lonePair);
            foreach (var singleElectron in atomContainer.SingleElectrons.Where(singleElectron => !Contains(singleElectron)))
                SingleElectrons.Add(singleElectron);
            foreach (var se in atomContainer.StereoElements)
                stereoElements.Add(se);

             NotifyChanged();         }

        /// <inheritdoc/>
        public virtual void AddElectronContainer(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
            {
                Bonds.Add(bond);
                return;
            }
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
            { 
                LonePairs.Add(lonePair);
                return;
            }
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
            {
                SingleElectrons.Add(singleElectron);
                return;
            }
        }

        /// <inheritdoc/>
        public virtual void Remove(IAtomContainer atomContainer)
        {
            foreach (var atom in atomContainer.Atoms)
                Atoms.Remove(atom);
            foreach (var bond in atomContainer.Bonds)
                Bonds.Remove(bond);
            foreach (var lonePair in atomContainer.LonePairs)
                LonePairs.Remove(lonePair);
            foreach (var singleElectron in atomContainer.SingleElectrons)
                SingleElectrons.Remove(singleElectron);
        }

        /// <inheritdoc/>
        public virtual void RemoveElectronContainer(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
            {
                Bonds.Remove(bond);
                return;
            }
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
            {
                LonePairs.Remove(lonePair);
                return;
            }
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
            {
                SingleElectrons.Remove(singleElectron);
                return;
            }
        }

        /// <inheritdoc/>
        public virtual void RemoveAtomAndConnectedElectronContainers(IAtom atom)
        {
            {
                var toRemove = bonds.Where(bond => bond.Contains(atom)).ToList();
                foreach (var bond in toRemove)
                    bonds.Remove(bond);
            }
            {
                var toRemove = lonePairs.Where(lonePair => lonePair.Contains(atom)).ToList();
                foreach (var lonePair in toRemove)
                    lonePairs.Remove(lonePair);
            }
            {
                var toRemove = singleElectrons.Where(singleElectron => singleElectron.Contains(atom)).ToList();
                foreach (var singleElectron in toRemove)
                    singleElectrons.Remove(singleElectron);
            }
            {
                var toRemove = stereoElements.Where(stereoElement => stereoElement.Contains(atom)).ToList();
                foreach (var stereoElement in toRemove)
                    stereoElements.Remove(stereoElement);
            }

            Atoms.Remove(atom);

             NotifyChanged();         }

        /// <inheritdoc/>
        public virtual void RemoveAllElements()
        {
            RemoveAllElectronContainers();
            foreach (var atom in atoms)
                atom.Listeners?.Remove(this);
            atoms.Clear();
            stereoElements.Clear();

             NotifyChanged();         }

        /// <inheritdoc/>
        public virtual void RemoveAllElectronContainers()
        {
            RemoveAllBonds();
            foreach (var e in lonePairs)
                e.Listeners?.Remove(this);
            foreach (var e in singleElectrons)
                e.Listeners?.Remove(this);
            lonePairs.Clear();
            singleElectrons.Clear();

             NotifyChanged();         }

        /// <inheritdoc/>
        public virtual void RemoveAllBonds()
        {
            foreach (var e in bonds)
                e.Listeners?.Remove(this);
            bonds.Clear();
             NotifyChanged();         }

        /// <inheritdoc/>
        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            var bond = Builder.CreateBond(atom1, atom2, order, stereo);
            Bonds.Add(bond);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            IBond bond = Builder.CreateBond(atom1, atom2, order);
            Bonds.Add(bond);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddLonePairTo(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.CreateLonePair(atom);
            e.Listeners?.Add(this);
            LonePairs.Add(e);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddSingleElectronTo(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.CreateSingleElectron(atom);
            e.Listeners?.Add(this);
            SingleElectrons.Add(e);
            // no OnStateChanged
        }

        /// <summary>
        /// Removes the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connects the two atoms</returns>
        public virtual IBond RemoveBond(IAtom atom1, IAtom atom2)
        {
            var bond = GetBond(atom1, atom2);
            if (bond != null)
                Bonds.Remove(bond);
            return bond;
        }

        /// <inheritdoc/>
        public virtual bool Contains(IAtom atom) => atoms.Any(n => n == atom);

        /// <inheritdoc/>
        public virtual bool Contains(IBond bond) => bonds.Any(n => n == bond);

        /// <inheritdoc/>
        public virtual bool Contains(ILonePair lonePair) => lonePairs.Any(n => n == lonePair);

        /// <inheritdoc/>
        public virtual bool Contains(ISingleElectron singleElectron) => singleElectrons.Any(n => n == singleElectron);

        /// <inheritdoc/>
        public virtual bool Contains(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
                return Contains(bond);
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
                return Contains(lonePair);
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
                return Contains(singleElectron);
            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AtomContainer(");
            sb.Append(ToStringInner());
            sb.Append(')');
            return sb.ToString();
        }

        internal virtual string ToStringInner()
        {
            var sb = new StringBuilder();
            sb.Append(GetHashCode());
            Append(sb, atoms, "A");
            Append(sb, bonds, "B");
            Append(sb, lonePairs, "LP");
            Append(sb, singleElectrons, "SE");
            if (stereoElements.Count > 0)
            {
                sb.Append(", ST:[#").Append(stereoElements.Count);
                foreach (var elements in stereoElements)
                    sb.Append(", ").Append(elements.ToString());
                sb.Append(']');
            }
            return sb.ToString();
        }

        private void Append<T>(StringBuilder sb, ICollection<T> os, string tag)
        {
            if (os.Count > 0)
            {
                sb.Append(", #").Append(tag).Append(":").Append(os.Count);
                foreach (var e in os)
                    sb.Append(", ").Append(e.ToString());
            }
        }

        /// <inheritdoc/>
        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (AtomContainer)base.Clone(map);            
            clone.atoms = CreateObservableChemObjectCollection(atoms.Where(n => n != null).Select(n => (IAtom)n.Clone(map)), false);
            clone.bonds = CreateObservableChemObjectCollection(bonds.Where(n => n != null).Select(n => (IBond)n.Clone(map)), true);
            clone.lonePairs = CreateObservableChemObjectCollection(lonePairs.Where(n => n != null).Select(n => (ILonePair)n.Clone(map)), true);
            clone.singleElectrons = CreateObservableChemObjectCollection(singleElectrons.Where(n => n != null).Select(n => (ISingleElectron)n.Clone(map)), true);
            clone.stereoElements = new List<IStereoElement>(stereoElements.Select(n => (IStereoElement)n.Clone(map)));

            return clone;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IElectronContainer> GetElectronContainers()
        {
            return bonds.Cast<IElectronContainer>().Concat(LonePairs).Concat(SingleElectrons);
        }

        /// <inheritdoc/>
        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
             NotifyChanged(evt);         }

        /// <inheritdoc/>
        public void SetAtoms(IEnumerable<IAtom> atoms)
        {
            this.atoms.Clear();
            foreach (var atom in atoms)
                this.atoms.Add(atom);
        }

        /// <inheritdoc/>
        public void SetBonds(IEnumerable<IBond> bonds)
        {
            this.bonds.Clear();
            foreach (var bond in bonds)
                this.bonds.Add(bond);
        }

        /// <inheritdoc/>
        public virtual bool IsEmpty() => atoms.Count == 0;
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Base class for all chemical objects that maintain a list of Atoms and
    /// ElectronContainers. 
    /// </summary>
    /// <example>
    /// Looping over all Bonds in the AtomContainer is typically done like: 
    /// <code>
    /// foreach (IBond aBond in atomContainer.Bonds)
    /// {
    ///         // do something
    /// }
    /// </code>
    /// </example>
    // @cdk.githash 
    // @author steinbeck
    // @cdk.created 2000-10-02 
    [Serializable]
    public class AtomContainer
        : ChemObject, IAtomContainer, IChemObjectListener
    {
        /// <summary>
        /// Atoms contained by this object.
        /// </summary>
        internal IList<IAtom> atoms;

        /// <summary>
        /// Bonds contained by this object.
        /// </summary>
        internal IList<IBond> bonds;

        /// <summary>
        /// Lone pairs contained by this object.
        /// </summary>
        internal IList<ILonePair> lonePairs;

        /// <summary>
        /// Single electrons contained by this object.
        /// </summary>
        internal IList<ISingleElectron> singleElectrons;

        /// <summary>
        /// Stereo elements contained by this object.
        /// </summary>
        internal IList<IStereoElement> stereoElements;

        internal bool isAromatic;
        internal bool isSingleOrDouble;

        private void Init(
            IList<IAtom> atoms,
            IList<IBond> bonds,
            IList<ILonePair> lonePairs,
            IList<ISingleElectron> singleElectrons,
            IList<IStereoElement> stereoElements)
        {
            this.atoms = atoms;
            this.bonds = bonds;
            this.lonePairs = lonePairs;
            this.singleElectrons = singleElectrons;
            this.stereoElements = stereoElements;
        }

        public AtomContainer(
            IEnumerable<IAtom> atoms,
            IEnumerable<IBond> bonds,
            IEnumerable<ILonePair> lonePairs,
            IEnumerable<ISingleElectron> singleElectrons,
            IEnumerable<IStereoElement> stereoElements)
        {
            Init(
                CreateObservableChemObjectCollection(atoms, false),
                CreateObservableChemObjectCollection(bonds, true),
                CreateObservableChemObjectCollection(lonePairs, true),
                CreateObservableChemObjectCollection(singleElectrons, true),
                new List<IStereoElement>(stereoElements)
            );
        }

        private ObservableChemObjectCollection<T> CreateObservableChemObjectCollection<T>(IEnumerable<T> objs, bool allowDup) where T : IChemObject
        {
 
            var list = new ObservableChemObjectCollection<T>(null, objs);
            list.AllowDuplicate = allowDup;
            return list;
        }

        public AtomContainer(
           IEnumerable<IAtom> atoms,
           IEnumerable<IBond> bonds)
             : this(
                  atoms,
                  bonds,
                  Array.Empty<ILonePair>(),
                  Array.Empty<ISingleElectron>(),
                  Array.Empty<IStereoElement>())
        { }

        /// <summary>
        ///  Constructs an empty AtomContainer.
        /// </summary>
        public AtomContainer()
            : this(
                      Array.Empty<IAtom>(), 
                      Array.Empty<IBond>(), 
                      Array.Empty<ILonePair>(),
                      Array.Empty<ISingleElectron>(),
                      Array.Empty<IStereoElement>())
        { }

        /// <summary>
        /// Constructs an AtomContainer with a copy of the atoms and electronContainers
        /// of another AtomContainer (A shallow copy, i.e., with the same objects as in
        /// the original AtomContainer).
        /// </summary>
        /// <param name="container">An AtomContainer to copy the atoms and electronContainers from</param>
        public AtomContainer(IAtomContainer container)
            : this(
                  container.Atoms,
                  container.Bonds,
                  container.LonePairs,
                  container.SingleElectrons,
                  container.StereoElements)
        { }

        /// <inheritdoc/>
        public virtual bool IsAromatic
        {
            get { return isAromatic; }
            set
            {
                isAromatic = value; 
            }
        }
        
        /// <inheritdoc/>
        public virtual bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set
            {
                isSingleOrDouble = value;
            }
        }

        /// <inheritdoc/>
        public virtual IList<IAtom> Atoms => atoms;

        /// <inheritdoc/>
        public virtual IList<IBond> Bonds => bonds;

        /// <inheritdoc/>
        public virtual IList<ILonePair> LonePairs => lonePairs;

        /// <inheritdoc/>
        public virtual IList<ISingleElectron> SingleElectrons => singleElectrons;

        /// <inheritdoc/>
        public virtual IList<IStereoElement> StereoElements => stereoElements;

        /// <inheritdoc/>
        public virtual void SetStereoElements(IEnumerable<IStereoElement> elements) => stereoElements = new List<IStereoElement>(elements);

        /// <summary>
        /// Returns the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connects the two atoms</returns>
        public virtual IBond GetBond(IAtom atom1, IAtom atom2)
        {
            return bonds.Where(bond => bond.Contains(atom1) && bond.GetConnectedAtom(atom1) == atom2).FirstOrDefault();
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
        {
            foreach (var bond in Bonds)
                if (bond.Contains(atom))
                    yield return bond.GetConnectedAtom(atom);
            yield break;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IBond> GetConnectedBonds(IAtom atom)
        {
            return bonds.Where(bond => bond.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ILonePair> GetConnectedLonePairs(IAtom atom)
        {
            return LonePairs.Where(lonePair => lonePair.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ISingleElectron> GetConnectedSingleElectrons(IAtom atom)
        {
            return SingleElectrons.Where(singleElectron => singleElectron.Contains(atom));
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IElectronContainer> GetConnectedElectronContainers(IAtom atom)
        {
            foreach (var e in GetConnectedBonds(atom))
                yield return e;
            foreach (var e in GetConnectedLonePairs(atom))
                yield return e;
            foreach (var e in GetConnectedSingleElectrons(atom))
                yield return e;
            yield break;
        }

        private IEnumerable<BondOrder> GetBondOrders(IAtom atom)
        {
            return bonds.Where(bond => bond.Contains(atom))
                .Select(bond => bond.Order)
                .Where(order => !order.IsUnset);
        }

        /// <inheritdoc/>
        public virtual double GetBondOrderSum(IAtom atom)
        {
            return GetBondOrders(atom).Select(order => order.Numeric).Sum();
        }

        /// <inheritdoc/>
        public virtual BondOrder GetMaximumBondOrder(IAtom atom)
        {
            var max = BondOrder.Single;
            foreach (var order in GetBondOrders(atom))
            {
                if (max.Numeric < order.Numeric)
                    max = order;
            }
            return max;
        }

        /// <inheritdoc/>
        public virtual BondOrder GetMinimumBondOrder(IAtom atom)
        {
            var min = BondOrder.Quadruple;
            foreach (var order in GetBondOrders(atom))
            {
                if (min.Numeric > order.Numeric)
                    min = order;
            }
            return min;
        }

        /// <inheritdoc/>
        public virtual void Add(IAtomContainer atomContainer)
        {
            foreach (var atom in atomContainer.Atoms.Where(atom => !Contains(atom)))
                Atoms.Add(atom);
            foreach (var bond in atomContainer.Bonds.Where(bond => !Contains(bond)))
                Bonds.Add(bond);
            foreach (var lonePair in atomContainer.LonePairs.Where(lonePair => !Contains(lonePair)))
                LonePairs.Add(lonePair);
            foreach (var singleElectron in atomContainer.SingleElectrons.Where(singleElectron => !Contains(singleElectron)))
                SingleElectrons.Add(singleElectron);
            foreach (var se in atomContainer.StereoElements)
                stereoElements.Add(se);

                    }

        /// <inheritdoc/>
        public virtual void AddElectronContainer(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
            {
                Bonds.Add(bond);
                return;
            }
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
            { 
                LonePairs.Add(lonePair);
                return;
            }
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
            {
                SingleElectrons.Add(singleElectron);
                return;
            }
        }

        /// <inheritdoc/>
        public virtual void Remove(IAtomContainer atomContainer)
        {
            foreach (var atom in atomContainer.Atoms)
                Atoms.Remove(atom);
            foreach (var bond in atomContainer.Bonds)
                Bonds.Remove(bond);
            foreach (var lonePair in atomContainer.LonePairs)
                LonePairs.Remove(lonePair);
            foreach (var singleElectron in atomContainer.SingleElectrons)
                SingleElectrons.Remove(singleElectron);
        }

        /// <inheritdoc/>
        public virtual void RemoveElectronContainer(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
            {
                Bonds.Remove(bond);
                return;
            }
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
            {
                LonePairs.Remove(lonePair);
                return;
            }
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
            {
                SingleElectrons.Remove(singleElectron);
                return;
            }
        }

        /// <inheritdoc/>
        public virtual void RemoveAtomAndConnectedElectronContainers(IAtom atom)
        {
            {
                var toRemove = bonds.Where(bond => bond.Contains(atom)).ToList();
                foreach (var bond in toRemove)
                    bonds.Remove(bond);
            }
            {
                var toRemove = lonePairs.Where(lonePair => lonePair.Contains(atom)).ToList();
                foreach (var lonePair in toRemove)
                    lonePairs.Remove(lonePair);
            }
            {
                var toRemove = singleElectrons.Where(singleElectron => singleElectron.Contains(atom)).ToList();
                foreach (var singleElectron in toRemove)
                    singleElectrons.Remove(singleElectron);
            }
            {
                var toRemove = stereoElements.Where(stereoElement => stereoElement.Contains(atom)).ToList();
                foreach (var stereoElement in toRemove)
                    stereoElements.Remove(stereoElement);
            }

            Atoms.Remove(atom);

                    }

        /// <inheritdoc/>
        public virtual void RemoveAllElements()
        {
            RemoveAllElectronContainers();
            foreach (var atom in atoms)
                atom.Listeners?.Remove(this);
            atoms.Clear();
            stereoElements.Clear();

                    }

        /// <inheritdoc/>
        public virtual void RemoveAllElectronContainers()
        {
            RemoveAllBonds();
            foreach (var e in lonePairs)
                e.Listeners?.Remove(this);
            foreach (var e in singleElectrons)
                e.Listeners?.Remove(this);
            lonePairs.Clear();
            singleElectrons.Clear();

                    }

        /// <inheritdoc/>
        public virtual void RemoveAllBonds()
        {
            foreach (var e in bonds)
                e.Listeners?.Remove(this);
            bonds.Clear();
                    }

        /// <inheritdoc/>
        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            var bond = Builder.CreateBond(atom1, atom2, order, stereo);
            Bonds.Add(bond);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddBond(IAtom atom1, IAtom atom2, BondOrder order)
        {
            if (!(Contains(atom1) && Contains(atom2)))
                throw new InvalidOperationException();
            IBond bond = Builder.CreateBond(atom1, atom2, order);
            Bonds.Add(bond);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddLonePairTo(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.CreateLonePair(atom);
            e.Listeners?.Add(this);
            LonePairs.Add(e);
            // no OnStateChanged
        }

        /// <inheritdoc/>
        public virtual void AddSingleElectronTo(IAtom atom)
        {
            if (!Contains(atom))
                throw new InvalidOperationException();
            var e = Builder.CreateSingleElectron(atom);
            e.Listeners?.Add(this);
            SingleElectrons.Add(e);
            // no OnStateChanged
        }

        /// <summary>
        /// Removes the bond that connects the two given atoms.
        /// </summary>
        /// <param name="atom1">The first atom</param>
        /// <param name="atom2">The second atom</param>
        /// <returns>The bond that connects the two atoms</returns>
        public virtual IBond RemoveBond(IAtom atom1, IAtom atom2)
        {
            var bond = GetBond(atom1, atom2);
            if (bond != null)
                Bonds.Remove(bond);
            return bond;
        }

        /// <inheritdoc/>
        public virtual bool Contains(IAtom atom) => atoms.Any(n => n == atom);

        /// <inheritdoc/>
        public virtual bool Contains(IBond bond) => bonds.Any(n => n == bond);

        /// <inheritdoc/>
        public virtual bool Contains(ILonePair lonePair) => lonePairs.Any(n => n == lonePair);

        /// <inheritdoc/>
        public virtual bool Contains(ISingleElectron singleElectron) => singleElectrons.Any(n => n == singleElectron);

        /// <inheritdoc/>
        public virtual bool Contains(IElectronContainer electronContainer)
        {
            var bond = electronContainer as IBond;
            if (bond != null)
                return Contains(bond);
            var lonePair = electronContainer as ILonePair;
            if (lonePair != null)
                return Contains(lonePair);
            var singleElectron = electronContainer as ISingleElectron;
            if (singleElectron != null)
                return Contains(singleElectron);
            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AtomContainer(");
            sb.Append(ToStringInner());
            sb.Append(')');
            return sb.ToString();
        }

        internal virtual string ToStringInner()
        {
            var sb = new StringBuilder();
            sb.Append(GetHashCode());
            Append(sb, atoms, "A");
            Append(sb, bonds, "B");
            Append(sb, lonePairs, "LP");
            Append(sb, singleElectrons, "SE");
            if (stereoElements.Count > 0)
            {
                sb.Append(", ST:[#").Append(stereoElements.Count);
                foreach (var elements in stereoElements)
                    sb.Append(", ").Append(elements.ToString());
                sb.Append(']');
            }
            return sb.ToString();
        }

        private void Append<T>(StringBuilder sb, ICollection<T> os, string tag)
        {
            if (os.Count > 0)
            {
                sb.Append(", #").Append(tag).Append(":").Append(os.Count);
                foreach (var e in os)
                    sb.Append(", ").Append(e.ToString());
            }
        }

        /// <inheritdoc/>
        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (AtomContainer)base.Clone(map);            
            clone.atoms = CreateObservableChemObjectCollection(atoms.Where(n => n != null).Select(n => (IAtom)n.Clone(map)), false);
            clone.bonds = CreateObservableChemObjectCollection(bonds.Where(n => n != null).Select(n => (IBond)n.Clone(map)), true);
            clone.lonePairs = CreateObservableChemObjectCollection(lonePairs.Where(n => n != null).Select(n => (ILonePair)n.Clone(map)), true);
            clone.singleElectrons = CreateObservableChemObjectCollection(singleElectrons.Where(n => n != null).Select(n => (ISingleElectron)n.Clone(map)), true);
            clone.stereoElements = new List<IStereoElement>(stereoElements.Select(n => (IStereoElement)n.Clone(map)));

            return clone;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IElectronContainer> GetElectronContainers()
        {
            return bonds.Cast<IElectronContainer>().Concat(LonePairs).Concat(SingleElectrons);
        }

        /// <inheritdoc/>
        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                    }

        /// <inheritdoc/>
        public void SetAtoms(IEnumerable<IAtom> atoms)
        {
            this.atoms.Clear();
            foreach (var atom in atoms)
                this.atoms.Add(atom);
        }

        /// <inheritdoc/>
        public void SetBonds(IEnumerable<IBond> bonds)
        {
            this.bonds.Clear();
            foreach (var bond in bonds)
                this.bonds.Add(bond);
        }

        /// <inheritdoc/>
        public virtual bool IsEmpty() => atoms.Count == 0;
    }
}
