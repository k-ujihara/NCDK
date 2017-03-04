
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

using System;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class ElectronContainer 
        : ChemObject, IElectronContainer, ICloneable
    {
        private int? electronCount = 0;

        public ElectronContainer()
            : base()
        {
        }

        public virtual int? ElectronCount
        {
            get { return electronCount; }
            set
            {
                electronCount = value;
                NotifyChanged();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ElectronContainer(").Append(GetHashCode());
            if (ElectronCount > -1)
                sb.Append("EC:").Append(ElectronCount);
            sb.Append(')');
            return sb.ToString();
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class ElectronContainer 
        : ChemObject, IElectronContainer, ICloneable
    {
        private int? electronCount = 0;

        public ElectronContainer()
            : base()
        {
        }

        public virtual int? ElectronCount
        {
            get { return electronCount; }
            set
            {
                electronCount = value;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ElectronContainer(").Append(GetHashCode());
            if (ElectronCount > -1)
                sb.Append("EC:").Append(ElectronCount);
            sb.Append(')');
            return sb.ToString();
        }
    }
}
