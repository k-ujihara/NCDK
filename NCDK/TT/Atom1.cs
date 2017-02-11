















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2016  Kazuya Ujihara

using NCDK.Config;
using System;
using NCDK.Numerics;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class Atom
        : AtomType, IAtom
    {
        internal double? charge;
        internal int? implicitHydrogenCount;
        internal Vector2? point2D;
        internal Vector3? point3D;
        internal Vector3? fractionalPoint3D;
        internal int? stereoParity;
        internal bool isSingleOrDouble;

        public Atom()
            : this((string)null)
        { }

        public Atom(string elementSymbol)
            : this(new Element(elementSymbol, Elements.OfString(elementSymbol).AtomicNumber))
        {
            FormalCharge = 0;
        }

        public Atom(string elementSymbol, Vector2 point2d)
            : base(elementSymbol)
        {
            this.point2D = point2d;
        }

        public Atom(string elementSymbol, Vector3 point3d)
            : base(elementSymbol)
        {
            this.point3D = point3d;
        }

        public Atom(IElement element)
            : base(element)
        {
            IAtom a = element as IAtom;
            if (a != null)
            {
                this.point2D = a.Point2D;
                this.point3D = a.Point3D;
                this.fractionalPoint3D = a.FractionalPoint3D;
                this.implicitHydrogenCount = a.ImplicitHydrogenCount;
                this.charge = a.Charge;
                this.stereoParity = a.StereoParity;
            }
        }

        public virtual double? Charge
        {
            get { return charge; }
            set 
            {
                 charge = value;  
                NotifyChanged();
            }
        }

        public virtual int? ImplicitHydrogenCount
        {
            get { return implicitHydrogenCount; }
            set 
            {
                implicitHydrogenCount = value;  
                NotifyChanged();
            }
        }

        public virtual Vector2? Point2D
        {
            get { return point2D; }
            set 
            {
                point2D = value;  
                NotifyChanged();
            }
        }

        public virtual Vector3? Point3D
        {
            get { return point3D; }
            set 
            {
                point3D = value;  
                NotifyChanged();
            }
        }

        public virtual Vector3? FractionalPoint3D
        {
            get { return fractionalPoint3D; }
            set 
            {
                fractionalPoint3D = value;  
                NotifyChanged();
            }
        }

        public virtual int? StereoParity
        {
            get { return stereoParity; }
            set 
            {
                stereoParity = value;  
                NotifyChanged();
            }
        }

        public bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set 
            {
                isSingleOrDouble = value;  
                NotifyChanged();
            }
        }

        public override bool Compare(object obj)
        {
            var aa = obj as IAtom;
            return aa != null && base.Compare(obj)
                && Point2D == aa.Point2D
                && Point3D == aa.Point3D
                && ImplicitHydrogenCount == aa.ImplicitHydrogenCount
                && StereoParity == aa.StereoParity
                && Charge == aa.Charge;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Atom(").Append(GetHashCode());
            if (Symbol != null)
                sb.Append(", S:").Append(Symbol);
            if (ImplicitHydrogenCount != null)
                sb.Append(", H:").Append(ImplicitHydrogenCount);
            if (StereoParity != null)
                sb.Append(", SP:").Append(StereoParity);
            if (Point2D != null)
                sb.Append(", 2D:[").Append(Point2D).Append(']');
            if (Point3D != null)
                sb.Append(", 3D:[").Append(Point3D).Append(']');
            if (FractionalPoint3D != null)
                sb.Append(", F3D:[").Append(FractionalPoint3D);
            if (Charge != null)
                sb.Append(", C:").Append(Charge);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
			if (map == null)
				throw new ArgumentNullException(nameof(map));
            IAtom clone;
            if (map.AtomMap.TryGetValue(this, out clone))
                return clone;
            clone = (Atom)base.Clone(map);
            map.AtomMap.Add(this, clone);
            return clone;
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class Atom
        : AtomType, IAtom
    {
        internal double? charge;
        internal int? implicitHydrogenCount;
        internal Vector2? point2D;
        internal Vector3? point3D;
        internal Vector3? fractionalPoint3D;
        internal int? stereoParity;
        internal bool isSingleOrDouble;

        public Atom()
            : this((string)null)
        { }

        public Atom(string elementSymbol)
            : this(new Element(elementSymbol, Elements.OfString(elementSymbol).AtomicNumber))
        {
            FormalCharge = 0;
        }

        public Atom(string elementSymbol, Vector2 point2d)
            : base(elementSymbol)
        {
            this.point2D = point2d;
        }

        public Atom(string elementSymbol, Vector3 point3d)
            : base(elementSymbol)
        {
            this.point3D = point3d;
        }

        public Atom(IElement element)
            : base(element)
        {
            IAtom a = element as IAtom;
            if (a != null)
            {
                this.point2D = a.Point2D;
                this.point3D = a.Point3D;
                this.fractionalPoint3D = a.FractionalPoint3D;
                this.implicitHydrogenCount = a.ImplicitHydrogenCount;
                this.charge = a.Charge;
                this.stereoParity = a.StereoParity;
            }
        }

        public virtual double? Charge
        {
            get { return charge; }
            set 
            {
                 charge = value;  
            }
        }

        public virtual int? ImplicitHydrogenCount
        {
            get { return implicitHydrogenCount; }
            set 
            {
                implicitHydrogenCount = value;  
            }
        }

        public virtual Vector2? Point2D
        {
            get { return point2D; }
            set 
            {
                point2D = value;  
            }
        }

        public virtual Vector3? Point3D
        {
            get { return point3D; }
            set 
            {
                point3D = value;  
            }
        }

        public virtual Vector3? FractionalPoint3D
        {
            get { return fractionalPoint3D; }
            set 
            {
                fractionalPoint3D = value;  
            }
        }

        public virtual int? StereoParity
        {
            get { return stereoParity; }
            set 
            {
                stereoParity = value;  
            }
        }

        public bool IsSingleOrDouble
        {
            get { return isSingleOrDouble; }
            set 
            {
                isSingleOrDouble = value;  
            }
        }

        public override bool Compare(object obj)
        {
            var aa = obj as IAtom;
            return aa != null && base.Compare(obj)
                && Point2D == aa.Point2D
                && Point3D == aa.Point3D
                && ImplicitHydrogenCount == aa.ImplicitHydrogenCount
                && StereoParity == aa.StereoParity
                && Charge == aa.Charge;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Atom(").Append(GetHashCode());
            if (Symbol != null)
                sb.Append(", S:").Append(Symbol);
            if (ImplicitHydrogenCount != null)
                sb.Append(", H:").Append(ImplicitHydrogenCount);
            if (StereoParity != null)
                sb.Append(", SP:").Append(StereoParity);
            if (Point2D != null)
                sb.Append(", 2D:[").Append(Point2D).Append(']');
            if (Point3D != null)
                sb.Append(", 3D:[").Append(Point3D).Append(']');
            if (FractionalPoint3D != null)
                sb.Append(", F3D:[").Append(FractionalPoint3D);
            if (Charge != null)
                sb.Append(", C:").Append(Charge);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
			if (map == null)
				throw new ArgumentNullException(nameof(map));
            IAtom clone;
            if (map.AtomMap.TryGetValue(this, out clone))
                return clone;
            clone = (Atom)base.Clone(map);
            map.AtomMap.Add(this, clone);
            return clone;
        }
    }
}
