
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

using System;
using NCDK.Numerics;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class PseudoAtom
        : Atom, ICloneable, IPseudoAtom
    {
        private string label;
        private int attachPointNum;

        public PseudoAtom()
            : this("*")
        {
        }

        public PseudoAtom(string label)
            : base("R")
        {
            this.label = label;
            base.fractionalPoint3D = null;
            base.point2D = null;
            base.point3D = null;

            base.stereoParity = 0;
        }

        public PseudoAtom(IElement element)
            : base(element)
        {
            var aa = element as IPseudoAtom;
            if (aa != null)
                this.label = aa.Label;
            else
            {
                base.symbol = "R";
                this.label = element.Symbol;
            }
        }

        public PseudoAtom(string label, Vector3 point3d)
            : this(label)
        {
            base.point3D = point3d;
        }

        public PseudoAtom(string label, Vector2 point2d)
            : this(label)
        {
            base.point2D = point2d;
        }

        public virtual string Label
        {
            get { return label; }
            set { label = value;  NotifyChanged();  }
        }

        public virtual int AttachPointNum
        {
            get { return attachPointNum; }
            set { attachPointNum = value;  NotifyChanged();  }
        }

        public override int? StereoParity
        {
            get { return base.StereoParity; }
            set { /* this is undefined, always */}
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("PseudoAtom(");
            sb.Append(GetHashCode());
            if (Label != null)
                sb.Append(", ").Append(Label);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            return base.Clone(map);
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class PseudoAtom
        : Atom, ICloneable, IPseudoAtom
    {
        private string label;
        private int attachPointNum;

        public PseudoAtom()
            : this("*")
        {
        }

        public PseudoAtom(string label)
            : base("R")
        {
            this.label = label;
            base.fractionalPoint3D = null;
            base.point2D = null;
            base.point3D = null;

            base.stereoParity = 0;
        }

        public PseudoAtom(IElement element)
            : base(element)
        {
            var aa = element as IPseudoAtom;
            if (aa != null)
                this.label = aa.Label;
            else
            {
                base.symbol = "R";
                this.label = element.Symbol;
            }
        }

        public PseudoAtom(string label, Vector3 point3d)
            : this(label)
        {
            base.point3D = point3d;
        }

        public PseudoAtom(string label, Vector2 point2d)
            : this(label)
        {
            base.point2D = point2d;
        }

        public virtual string Label
        {
            get { return label; }
            set { label = value;  }
        }

        public virtual int AttachPointNum
        {
            get { return attachPointNum; }
            set { attachPointNum = value;  }
        }

        public override int? StereoParity
        {
            get { return base.StereoParity; }
            set { /* this is undefined, always */}
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("PseudoAtom(");
            sb.Append(GetHashCode());
            if (Label != null)
                sb.Append(", ").Append(Label);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            return base.Clone(map);
        }
    }
}
