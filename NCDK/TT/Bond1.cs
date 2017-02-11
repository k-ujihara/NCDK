















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NCDK.Numerics;

namespace NCDK.Default
{
    [Serializable]
    public class Bond
        : ElectronContainer, IBond, IChemObjectListener, ICloneable
    {
	    internal bool isAromatic;
        internal bool isAliphatic;
        internal bool isInRing;
        internal bool isSingleOrDouble;
        internal bool isReactiveCenter;
        internal BondOrder order;
        internal BondStereo stereo;
        private IList<IAtom> atoms;

        public Bond()
            : this(Array.Empty<IAtom>())
        {
        }

        public Bond(IAtom atom1, IAtom atom2)
            : this(atom1, atom2, BondOrder.Single, BondStereo.None)
        {
        }

        public Bond(IAtom atom1, IAtom atom2, BondOrder order)
            : this(atom1, atom2, order, BondStereo.None)
        {
        }

        public Bond(IEnumerable<IAtom> atoms)
            : this(atoms, BondOrder.Unset, BondStereo.None)
        {
        }

        public Bond(IEnumerable<IAtom> atoms, BondOrder order)
            : this(atoms, order, BondStereo.None)
        {
        }

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

        public virtual IList<IAtom> Atoms => atoms;

        public virtual IAtom GetConnectedAtom(IAtom atom)
        {
            if (atoms[0] == atom)
                return atoms[1];
            else if (atoms[1] == atom)
                return atoms[0];
            return null;
        }

        public virtual void SetAtoms(IEnumerable<IAtom> atoms)
        {
            this.atoms.Clear();
            foreach (var atom in atoms)
                this.atoms.Add(atom);
        }

        public virtual IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
        {
            if (!atoms.Contains(atom))
                return null;
            return atoms.Where(n => n != atom);
        }

        public virtual bool Contains(IAtom atom)
        {
            if (atom == null)
                return false;
            return atoms.Contains(atom);
        }

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

        public virtual BondStereo Stereo
        {
            get { return stereo; }
            set
            {
                this.stereo = value;
                NotifyChanged();
            }
        }

        public virtual Vector2 Geometric2DCenter
        {
            get
            {
                var x = atoms.Where(n => n != null).Select(n => n.Point2D.Value.X).Average();
                var y = atoms.Where(n => n != null).Select(n => n.Point2D.Value.Y).Average();
                return new Vector2(x, y);
            }
        }

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

        public virtual bool IsConnectedTo(IBond bond)
        {
            return atoms.Any(atom => bond.Contains(atom));
        }

        public virtual bool IsAromatic
        {
            get { return isAromatic; }
            set
            {
                isAromatic = value; 
                NotifyChanged();
            }
        }

        public virtual bool IsAliphatic
        {
            get { return isAliphatic; }
            set
            {
                isAliphatic = value; 
                NotifyChanged();
            }
        }

        public virtual bool IsInRing
        {
            get { return isInRing; }
            set
            {
                isInRing = value;
                NotifyChanged();
            }
        }

        public virtual bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set
            {
                isSingleOrDouble = value;
                NotifyChanged();
            }
        }

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
            if (map.BondMap.TryGetValue(this, out iclone))
                return iclone;
            var clone = (Bond)base.Clone(map);
            // clone all the Atoms
            if (atoms != null)
            {
                clone.InitAtoms(atoms.Select(n => (IAtom)n?.Clone(map)));
            }
            map.BondMap.Add(this, clone);
            return clone;
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

        public override bool Compare(object obj)
        {
            var bond = obj as Bond;
			if (bond == null)
				return false;
            return !atoms.Any(atom => !bond.Contains(atom));
            // bond order is ignored
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
                NotifyChanged();
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class Bond
        : ElectronContainer, IBond, IChemObjectListener, ICloneable
    {
	    internal bool isAromatic;
        internal bool isAliphatic;
        internal bool isInRing;
        internal bool isSingleOrDouble;
        internal bool isReactiveCenter;
        internal BondOrder order;
        internal BondStereo stereo;
        private IList<IAtom> atoms;

        public Bond()
            : this(Array.Empty<IAtom>())
        {
        }

        public Bond(IAtom atom1, IAtom atom2)
            : this(atom1, atom2, BondOrder.Single, BondStereo.None)
        {
        }

        public Bond(IAtom atom1, IAtom atom2, BondOrder order)
            : this(atom1, atom2, order, BondStereo.None)
        {
        }

        public Bond(IEnumerable<IAtom> atoms)
            : this(atoms, BondOrder.Unset, BondStereo.None)
        {
        }

        public Bond(IEnumerable<IAtom> atoms, BondOrder order)
            : this(atoms, order, BondStereo.None)
        {
        }

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

        public virtual IList<IAtom> Atoms => atoms;

        public virtual IAtom GetConnectedAtom(IAtom atom)
        {
            if (atoms[0] == atom)
                return atoms[1];
            else if (atoms[1] == atom)
                return atoms[0];
            return null;
        }

        public virtual void SetAtoms(IEnumerable<IAtom> atoms)
        {
            this.atoms.Clear();
            foreach (var atom in atoms)
                this.atoms.Add(atom);
        }

        public virtual IEnumerable<IAtom> GetConnectedAtoms(IAtom atom)
        {
            if (!atoms.Contains(atom))
                return null;
            return atoms.Where(n => n != atom);
        }

        public virtual bool Contains(IAtom atom)
        {
            if (atom == null)
                return false;
            return atoms.Contains(atom);
        }

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

        public virtual BondStereo Stereo
        {
            get { return stereo; }
            set
            {
                this.stereo = value;
            }
        }

        public virtual Vector2 Geometric2DCenter
        {
            get
            {
                var x = atoms.Where(n => n != null).Select(n => n.Point2D.Value.X).Average();
                var y = atoms.Where(n => n != null).Select(n => n.Point2D.Value.Y).Average();
                return new Vector2(x, y);
            }
        }

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

        public virtual bool IsConnectedTo(IBond bond)
        {
            return atoms.Any(atom => bond.Contains(atom));
        }

        public virtual bool IsAromatic
        {
            get { return isAromatic; }
            set
            {
                isAromatic = value; 
            }
        }

        public virtual bool IsAliphatic
        {
            get { return isAliphatic; }
            set
            {
                isAliphatic = value; 
            }
        }

        public virtual bool IsInRing
        {
            get { return isInRing; }
            set
            {
                isInRing = value;
            }
        }

        public virtual bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set
            {
                isSingleOrDouble = value;
            }
        }

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
            if (map.BondMap.TryGetValue(this, out iclone))
                return iclone;
            var clone = (Bond)base.Clone(map);
            // clone all the Atoms
            if (atoms != null)
            {
                clone.InitAtoms(atoms.Select(n => (IAtom)n?.Clone(map)));
            }
            map.BondMap.Add(this, clone);
            return clone;
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

        public override bool Compare(object obj)
        {
            var bond = obj as Bond;
			if (bond == null)
				return false;
            return !atoms.Any(atom => !bond.Contains(atom));
            // bond order is ignored
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
        }
    }
}
