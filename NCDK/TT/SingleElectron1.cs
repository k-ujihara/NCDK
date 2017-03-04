
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

using System;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class SingleElectron
        : ElectronContainer, ISingleElectron, ICloneable
    {
        protected IAtom atom;

        public SingleElectron(IAtom atom)
        {
            this.atom = atom;
        }

        public SingleElectron()
            : this(null)
        {
        }

        public override int? ElectronCount
        {
            get { return 1; }
            set {  }
        }

        public IAtom Atom
        {
            get { return atom; }
            set
            {
                atom = value;
                 NotifyChanged();             }
        }

        public bool Contains(IAtom atom) => this.atom == atom;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("SingleElectron(");
            sb.Append(GetHashCode());
            if (atom != null)
            {
                sb.Append(", ");
                sb.Append(atom.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (SingleElectron)base.Clone(map);
            clone.atom = (IAtom)atom?.Clone(map);
            return clone;
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class SingleElectron
        : ElectronContainer, ISingleElectron, ICloneable
    {
        protected IAtom atom;

        public SingleElectron(IAtom atom)
        {
            this.atom = atom;
        }

        public SingleElectron()
            : this(null)
        {
        }

        public override int? ElectronCount
        {
            get { return 1; }
            set {  }
        }

        public IAtom Atom
        {
            get { return atom; }
            set
            {
                atom = value;
                            }
        }

        public bool Contains(IAtom atom) => this.atom == atom;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("SingleElectron(");
            sb.Append(GetHashCode());
            if (atom != null)
            {
                sb.Append(", ");
                sb.Append(atom.ToString());
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (SingleElectron)base.Clone(map);
            clone.atom = (IAtom)atom?.Clone(map);
            return clone;
        }
    }
}
