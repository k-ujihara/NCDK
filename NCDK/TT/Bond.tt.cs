


// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>

/* Copyright (C) 1997-2007  Christoph Steinbeck <steinbeck@users.sf.net>
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
 *
 */
 using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NCDK.Numerics;

namespace NCDK.Default
{
    /// <summary>
    /// Implements the concept of a covalent bond between two or more atoms. A bond is
    /// considered to be a number of electrons connecting two or more of atoms.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It should be noted that the majority of applications will consider 2-center bonds,
    /// especially since the bond orders currently supported are really only valid for
    /// 2-center bonds. However the code does support multi-center bonds, though the
    /// orders may not make sense at this point.
    /// </para>
    /// <para>
    /// In general code that assumes bonds are 2-centered can use this class seamlessly, as
    /// the semantics are identical to the older versions. Care shoud be exercised when
    /// using multi-center bonds using this class as the orders may not make sense.
    /// </para>
    /// </remarks>
    // @author steinbeck
    // @cdk.githash
    // @cdk.created 2003-10-02
    // @cdk.keyword bond
    // @cdk.keyword atom
    // @cdk.keyword electron
    [Serializable]
    public class Bond
        : ElectronContainer, IBond, IChemObjectListener
    {
        internal bool isAromatic;
        internal bool isAliphatic;
        internal bool isInRing;
        internal bool isSingleOrDouble;
        internal bool isReactiveCenter;
        internal BondOrder order;
        internal BondStereo stereo;
        private IList<IAtom> atoms;
        
        /// <summary>
        /// Constructs an empty bond.
        /// </summary>
        public Bond()
            : this(Array.Empty<IAtom>())
        {
        }

        /// <summary>
        /// Constructs a bond with a single bond order.
        /// </summary>
        /// <param name="atom1">the first Atom in the bond</param>
        /// <param name="atom2">the second Atom in the bond</param>
        public Bond(IAtom atom1, IAtom atom2)
            : this(atom1, atom2, BondOrder.Single, BondStereo.None)
        {
        }

        /// <summary>
        /// Constructs a bond with a single bond order.
        /// </summary>
        /// <param name="atom1">the first Atom in the bond</param>
        /// <param name="atom2">the second Atom in the bond</param>
        /// <param name="order">the bond order</param>
        public Bond(IAtom atom1, IAtom atom2, BondOrder order)
            : this(atom1, atom2, order, BondStereo.None)
        {
        }

        /// <summary>
        /// Constructs a multi-center bond, with undefined order and no stereo information.
        /// </summary>
        /// <param name="atoms"><see cref="IEnumerable{T}"/> of <see cref="IAtom"/> containing the atoms constituting the bond</param>
        public Bond(IEnumerable<IAtom> atoms)
            : this(atoms, BondOrder.Unset, BondStereo.None)
        {
        }

        /// <summary>
        /// Constructs a multi-center bond, with a specified order and no stereo information.
        /// </summary>
        /// <param name="atoms">An array of <see cref="IAtom"/> containing the atoms constituting the bond</param>
        /// <param name="order">The order of the bond</param>
        public Bond(IEnumerable<IAtom> atoms, BondOrder order)
            : this(atoms, order, BondStereo.None)
        {
        }

        /// <summary>
        /// Constructs a bond with a single bond order.
        /// </summary>
        /// <param name="atom1">the first Atom in the bond</param>
        /// <param name="atom2">the second Atom in the bond</param>
        /// <param name="order">the bond order</param>
        /// <param name="stereo">a descriptor the stereochemical orientation of this bond</param>
        public Bond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo)
            : this(new IAtom[] { atom1, atom2, }, order, stereo)
        {
        }

        public Bond(IEnumerable<IAtom> atoms, BondOrder order, BondStereo stereo)
            : base()
        {
            InitAtoms(atoms);
            this.order = order;
            UpdateElectronCount(order);
            this.stereo = stereo;
        }

        private void InitAtoms(IEnumerable<IAtom> atoms)
        {
            if (atoms == null)
            {
                this.atoms = 
                    new List<IAtom>(2);
            }
            else
            {
                    this.atoms = 
                    new List<IAtom>(atoms);
            }
        }

        /// <inheritdoc/>
        public virtual IList<IAtom> Atoms => atoms;

		/// <inheritdoc/>
		public virtual int Index => -1;

		/// <inheritdoc/>
		public virtual IAtomContainer Container => null;

		/// <inheritdoc/>
        public IAtom Begin => atoms.Count < 1 ? null : atoms[0];

        /// <inheritdoc/>
        public IAtom End => atoms.Count < 2 ? null : atoms[1];

        /// <inheritdoc/>
        public IAtom GetOther(IAtom atom)
        {
            if (atoms[0].Equals(atom))
                return atoms[1];
            else if (atoms[1].Equals(atom))
                return atoms[0];
            return null;
        }

        /// <inheritdoc/>
        public virtual IAtom GetConnectedAtom(IAtom atom)
        {
            return GetOther(atom);
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
        {
            if (!atoms.Contains(atom))
                return null;
			return atoms.Where(n => n != atom);
        }

        /// <summary>
        /// Returns true if the given atom participates in this bond.
        /// </summary>
        /// <param name="atom">The atom to be tested if it participates in this bond</param>
        /// <returns>true if the atom participates in this bond</returns>
        public virtual bool Contains(IAtom atom)
        {
            if (atom == null)
                return false;
            return atoms.Contains(atom);
        }

        /// <summary>
        /// Sets the array of atoms making up this bond.
        /// </summary>
		/// <param name="atoms">An array of atoms that forms this bond</param>
		/// <seealso cref="Atoms"/>
       public virtual void SetAtoms(IEnumerable<IAtom> atoms)
        {
            this.atoms.Clear();
            foreach (var atom in atoms)
                this.atoms.Add(atom);
        }

        /// <summary>
        /// The bond order of this bond.
        /// </summary>
        public virtual BondOrder Order
        {
            get { return order; }
            set
            {
                order = value;
                UpdateElectronCount(value);
                NotifyChanged();
            }
        }

        private void UpdateElectronCount(BondOrder order)
        {
            if (order == BondOrder.Unset)
                return;
            this.ElectronCount = order.Numeric * 2;
        }

        /// <summary>
        /// The stereo descriptor for this bond.
        /// </summary>
        public virtual BondStereo Stereo
        {
            get { return stereo; }
            set
            {
                this.stereo = value;
                NotifyChanged();
            }
        }

        /// <summary>
        /// The geometric 2D center of the bond.
        /// </summary>
        public virtual Vector2 Geometric2DCenter
        {
            get
            {
                var x = atoms.Where(n => n != null).Select(n => n.Point2D.Value.X).Average();
                var y = atoms.Where(n => n != null).Select(n => n.Point2D.Value.Y).Average();
                return new Vector2(x, y);
            }
        }

        /// <summary>
        /// The geometric 3D center of the bond.
        /// </summary>
        public virtual Vector3 Geometric3DCenter
        {
            get
            {
                var x = atoms.Where(n => n != null).Select(n => n.Point3D.Value.X).Average();
                var y = atoms.Where(n => n != null).Select(n => n.Point3D.Value.Y).Average();
                var z = atoms.Where(n => n != null).Select(n => n.Point3D.Value.Z).Average();
                return new Vector3(x, y, z);
            }
        }

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
        public virtual bool IsAliphatic
        {
            get { return isAliphatic; }
            set
            {
                isAliphatic = value; 
                NotifyChanged();
            }
        }

        /// <inheritdoc/>
        public virtual bool IsInRing
        {
            get { return isInRing; }
            set
            {
                isInRing = value;
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
        public virtual bool IsReactiveCenter
        {
            get { return isReactiveCenter; }
            set
            {
                isReactiveCenter = value;
                NotifyChanged();
            }
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            IBond iclone;
            if (map.TryGetValue(this, out iclone))
                return iclone;
            var clone = (Bond)base.Clone(map);
            // clone all the Atoms
            if (atoms != null)
            {
                clone.InitAtoms(atoms.Select(n => (IAtom)n?.Clone(map)));
            }
            map.Add(this, clone);
            return clone;
        }

		/// <inheritdoc/>
		public override int GetHashCode() 
		{
			return base.GetHashCode();
		}

		/// <inheritdoc/>
		public override bool Equals(object obj) 
		{
			if (obj is BondRef)
				return base.Equals(((BondRef) obj).Deref());
			return base.Equals(obj);
		}

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Bond(").Append(this.GetHashCode());
            if (Order != BondOrder.Unset)
                sb.Append(", #O:").Append(Order);
            sb.Append(", #S:").Append(Stereo);
            if (atoms.Count > 0)
            {
                sb.Append(", #A:").Append(atoms.Count);
                foreach (var atom in atoms)
                    sb.Append(", ").Append(atom == null ? "null" : atom.ToString());
            }
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        /// <summary>
        /// Compares a bond with this bond.
        /// </summary>
        /// <param name="obj">Object of type Bond</param>
        /// <returns> true if the bond is equal to this bond</returns>
        public override bool Compare(object obj)
        {
            var bond = obj as Bond;
            if (bond == null)
                return false;
            return !atoms.Any(atom => !bond.Contains(atom));
            // bond order is ignored
        }

        /// <summary>
        /// Checks whether a bond is connected to another one.
        /// This can only be true if the bonds have an Atom in common.
        /// </summary>
        /// <param name="bond">The bond which is checked to be connect with this one</param>
        /// <returns>true if the bonds share an atom, otherwise false</returns>
        public virtual bool IsConnectedTo(IBond bond)
        {
            return atoms.Any(atom => bond.Contains(atom));
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                NotifyChanged();
        }
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Implements the concept of a covalent bond between two or more atoms. A bond is
    /// considered to be a number of electrons connecting two or more of atoms.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It should be noted that the majority of applications will consider 2-center bonds,
    /// especially since the bond orders currently supported are really only valid for
    /// 2-center bonds. However the code does support multi-center bonds, though the
    /// orders may not make sense at this point.
    /// </para>
    /// <para>
    /// In general code that assumes bonds are 2-centered can use this class seamlessly, as
    /// the semantics are identical to the older versions. Care shoud be exercised when
    /// using multi-center bonds using this class as the orders may not make sense.
    /// </para>
    /// </remarks>
    // @author steinbeck
    // @cdk.githash
    // @cdk.created 2003-10-02
    // @cdk.keyword bond
    // @cdk.keyword atom
    // @cdk.keyword electron
    [Serializable]
    public class Bond
        : ElectronContainer, IBond, IChemObjectListener
    {
        internal bool isAromatic;
        internal bool isAliphatic;
        internal bool isInRing;
        internal bool isSingleOrDouble;
        internal bool isReactiveCenter;
        internal BondOrder order;
        internal BondStereo stereo;
        private IList<IAtom> atoms;
        
        /// <summary>
        /// Constructs an empty bond.
        /// </summary>
        public Bond()
            : this(Array.Empty<IAtom>())
        {
        }

        /// <summary>
        /// Constructs a bond with a single bond order.
        /// </summary>
        /// <param name="atom1">the first Atom in the bond</param>
        /// <param name="atom2">the second Atom in the bond</param>
        public Bond(IAtom atom1, IAtom atom2)
            : this(atom1, atom2, BondOrder.Single, BondStereo.None)
        {
        }

        /// <summary>
        /// Constructs a bond with a single bond order.
        /// </summary>
        /// <param name="atom1">the first Atom in the bond</param>
        /// <param name="atom2">the second Atom in the bond</param>
        /// <param name="order">the bond order</param>
        public Bond(IAtom atom1, IAtom atom2, BondOrder order)
            : this(atom1, atom2, order, BondStereo.None)
        {
        }

        /// <summary>
        /// Constructs a multi-center bond, with undefined order and no stereo information.
        /// </summary>
        /// <param name="atoms"><see cref="IEnumerable{T}"/> of <see cref="IAtom"/> containing the atoms constituting the bond</param>
        public Bond(IEnumerable<IAtom> atoms)
            : this(atoms, BondOrder.Unset, BondStereo.None)
        {
        }

        /// <summary>
        /// Constructs a multi-center bond, with a specified order and no stereo information.
        /// </summary>
        /// <param name="atoms">An array of <see cref="IAtom"/> containing the atoms constituting the bond</param>
        /// <param name="order">The order of the bond</param>
        public Bond(IEnumerable<IAtom> atoms, BondOrder order)
            : this(atoms, order, BondStereo.None)
        {
        }

        /// <summary>
        /// Constructs a bond with a single bond order.
        /// </summary>
        /// <param name="atom1">the first Atom in the bond</param>
        /// <param name="atom2">the second Atom in the bond</param>
        /// <param name="order">the bond order</param>
        /// <param name="stereo">a descriptor the stereochemical orientation of this bond</param>
        public Bond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo)
            : this(new IAtom[] { atom1, atom2, }, order, stereo)
        {
        }

        public Bond(IEnumerable<IAtom> atoms, BondOrder order, BondStereo stereo)
            : base()
        {
            InitAtoms(atoms);
            this.order = order;
            UpdateElectronCount(order);
            this.stereo = stereo;
        }

        private void InitAtoms(IEnumerable<IAtom> atoms)
        {
            if (atoms == null)
            {
                this.atoms = 
                    new ObservableChemObjectCollection<IAtom>(2, this);
            }
            else
            {
                    this.atoms = 
                new ObservableChemObjectCollection<IAtom>(this, atoms);
            }
        }

        /// <inheritdoc/>
        public virtual IList<IAtom> Atoms => atoms;

		/// <inheritdoc/>
		public virtual int Index => -1;

		/// <inheritdoc/>
		public virtual IAtomContainer Container => null;

		/// <inheritdoc/>
        public IAtom Begin => atoms.Count < 1 ? null : atoms[0];

        /// <inheritdoc/>
        public IAtom End => atoms.Count < 2 ? null : atoms[1];

        /// <inheritdoc/>
        public IAtom GetOther(IAtom atom)
        {
            if (atoms[0].Equals(atom))
                return atoms[1];
            else if (atoms[1].Equals(atom))
                return atoms[0];
            return null;
        }

        /// <inheritdoc/>
        public virtual IAtom GetConnectedAtom(IAtom atom)
        {
            return GetOther(atom);
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
        {
            if (!atoms.Contains(atom))
                return null;
			return atoms.Where(n => n != atom);
        }

        /// <summary>
        /// Returns true if the given atom participates in this bond.
        /// </summary>
        /// <param name="atom">The atom to be tested if it participates in this bond</param>
        /// <returns>true if the atom participates in this bond</returns>
        public virtual bool Contains(IAtom atom)
        {
            if (atom == null)
                return false;
            return atoms.Contains(atom);
        }

        /// <summary>
        /// Sets the array of atoms making up this bond.
        /// </summary>
		/// <param name="atoms">An array of atoms that forms this bond</param>
		/// <seealso cref="Atoms"/>
       public virtual void SetAtoms(IEnumerable<IAtom> atoms)
        {
            this.atoms.Clear();
            foreach (var atom in atoms)
                this.atoms.Add(atom);
        }

        /// <summary>
        /// The bond order of this bond.
        /// </summary>
        public virtual BondOrder Order
        {
            get { return order; }
            set
            {
                order = value;
                UpdateElectronCount(value);
            }
        }

        private void UpdateElectronCount(BondOrder order)
        {
            if (order == BondOrder.Unset)
                return;
            this.ElectronCount = order.Numeric * 2;
        }

        /// <summary>
        /// The stereo descriptor for this bond.
        /// </summary>
        public virtual BondStereo Stereo
        {
            get { return stereo; }
            set
            {
                this.stereo = value;
            }
        }

        /// <summary>
        /// The geometric 2D center of the bond.
        /// </summary>
        public virtual Vector2 Geometric2DCenter
        {
            get
            {
                var x = atoms.Where(n => n != null).Select(n => n.Point2D.Value.X).Average();
                var y = atoms.Where(n => n != null).Select(n => n.Point2D.Value.Y).Average();
                return new Vector2(x, y);
            }
        }

        /// <summary>
        /// The geometric 3D center of the bond.
        /// </summary>
        public virtual Vector3 Geometric3DCenter
        {
            get
            {
                var x = atoms.Where(n => n != null).Select(n => n.Point3D.Value.X).Average();
                var y = atoms.Where(n => n != null).Select(n => n.Point3D.Value.Y).Average();
                var z = atoms.Where(n => n != null).Select(n => n.Point3D.Value.Z).Average();
                return new Vector3(x, y, z);
            }
        }

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
        public virtual bool IsAliphatic
        {
            get { return isAliphatic; }
            set
            {
                isAliphatic = value; 
            }
        }

        /// <inheritdoc/>
        public virtual bool IsInRing
        {
            get { return isInRing; }
            set
            {
                isInRing = value;
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
        public virtual bool IsReactiveCenter
        {
            get { return isReactiveCenter; }
            set
            {
                isReactiveCenter = value;
            }
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            IBond iclone;
            if (map.TryGetValue(this, out iclone))
                return iclone;
            var clone = (Bond)base.Clone(map);
            // clone all the Atoms
            if (atoms != null)
            {
                clone.InitAtoms(atoms.Select(n => (IAtom)n?.Clone(map)));
            }
            map.Add(this, clone);
            return clone;
        }

		/// <inheritdoc/>
		public override int GetHashCode() 
		{
			return base.GetHashCode();
		}

		/// <inheritdoc/>
		public override bool Equals(object obj) 
		{
			if (obj is BondRef)
				return base.Equals(((BondRef) obj).Deref());
			return base.Equals(obj);
		}

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Bond(").Append(this.GetHashCode());
            if (Order != BondOrder.Unset)
                sb.Append(", #O:").Append(Order);
            sb.Append(", #S:").Append(Stereo);
            if (atoms.Count > 0)
            {
                sb.Append(", #A:").Append(atoms.Count);
                foreach (var atom in atoms)
                    sb.Append(", ").Append(atom == null ? "null" : atom.ToString());
            }
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        /// <summary>
        /// Compares a bond with this bond.
        /// </summary>
        /// <param name="obj">Object of type Bond</param>
        /// <returns> true if the bond is equal to this bond</returns>
        public override bool Compare(object obj)
        {
            var bond = obj as Bond;
            if (bond == null)
                return false;
            return !atoms.Any(atom => !bond.Contains(atom));
            // bond order is ignored
        }

        /// <summary>
        /// Checks whether a bond is connected to another one.
        /// This can only be true if the bonds have an Atom in common.
        /// </summary>
        /// <param name="bond">The bond which is checked to be connect with this one</param>
        /// <returns>true if the bonds share an atom, otherwise false</returns>
        public virtual bool IsConnectedTo(IBond bond)
        {
            return atoms.Any(atom => bond.Contains(atom));
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
        }
    }
}
