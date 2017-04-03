

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara

using NCDK.Common.Mathematics;
using System;
using System.Collections.Generic;
using NCDK.Numerics;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class Crystal
        : AtomContainer, ICrystal, ICloneable
    {
        private Vector3 a = Vector3.Zero;
        public Vector3 A
        {
            get { return a; }
            set { a = value;  NotifyChanged();  }
        }

        private Vector3 b = Vector3.Zero;
        public Vector3 B
        {
            get { return b; }
            set { b = value;  NotifyChanged();  }
        }

        private Vector3 c = Vector3.Zero;
        public Vector3 C
        {
            get { return c; }
            set { c = value;  NotifyChanged();  }
        }

        private string spaceGroup = "P1";
        public string SpaceGroup
        {
            get { return spaceGroup; }
            set { spaceGroup = value;  NotifyChanged();  }
        }

        private int? z = 1;
        public int? Z
        {
            get { return z; }
            set { z = value;  NotifyChanged();  }
        }

        public Crystal()
            : base()
        {
        }

        public Crystal(IAtomContainer container)
            : base(container)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Crystal(").Append(GetHashCode());
            if (SpaceGroup != null)
                sb.Append(", SG=").Append(SpaceGroup);
            if (Z > 0)
                sb.Append(", Z=").Append(Z);
            sb.Append(", a=(").Append(a.X).Append(", ").Append(a.Y).Append(", ").Append(a.Z);
            sb.Append(", b=(").Append(b.X).Append(", ").Append(b.Y).Append(", ").Append(b.Z);
            sb.Append(", c=(").Append(c.X).Append(", ").Append(c.Y).Append(", ").Append(c.Z);
            sb.Append(", ").Append(base.ToString());
            return sb.ToString();
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class Crystal
        : AtomContainer, ICrystal, ICloneable
    {
        private Vector3 a = Vector3.Zero;
        public Vector3 A
        {
            get { return a; }
            set { a = value;  }
        }

        private Vector3 b = Vector3.Zero;
        public Vector3 B
        {
            get { return b; }
            set { b = value;  }
        }

        private Vector3 c = Vector3.Zero;
        public Vector3 C
        {
            get { return c; }
            set { c = value;  }
        }

        private string spaceGroup = "P1";
        public string SpaceGroup
        {
            get { return spaceGroup; }
            set { spaceGroup = value;  }
        }

        private int? z = 1;
        public int? Z
        {
            get { return z; }
            set { z = value;  }
        }

        public Crystal()
            : base()
        {
        }

        public Crystal(IAtomContainer container)
            : base(container)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Crystal(").Append(GetHashCode());
            if (SpaceGroup != null)
                sb.Append(", SG=").Append(SpaceGroup);
            if (Z > 0)
                sb.Append(", Z=").Append(Z);
            sb.Append(", a=(").Append(a.X).Append(", ").Append(a.Y).Append(", ").Append(a.Z);
            sb.Append(", b=(").Append(b.X).Append(", ").Append(b.Y).Append(", ").Append(b.Z);
            sb.Append(", c=(").Append(c.X).Append(", ").Append(c.Y).Append(", ").Append(c.Z);
            sb.Append(", ").Append(base.ToString());
            return sb.ToString();
        }
    }
}
