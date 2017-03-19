

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

using System;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class Monomer 
        : AtomContainer, IMonomer, ICloneable
    {
        private string monomerName;
        private string monomerType;

        /// <summary>The name of this monomer (e.g. Trp42).</summary>
        public string MonomerName
        {
            get { return monomerName; }
            set
            {
                monomerName = value;
                 NotifyChanged();             }
        }

        /// <summary>The type of this monomer (e.g. TRP).</summary>
        public string MonomerType
        {
            get { return monomerType; }
            set
            {
                monomerType = value;
                 NotifyChanged();             }
        }

        public Monomer()
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Monomer{").Append(GetHashCode());
            if (MonomerName != null)
                sb.Append(", N=").Append(MonomerName);
            if (MonomerType != null)
                sb.Append(", T=").Append(MonomerType);
            sb.Append('}');
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
    public class Monomer 
        : AtomContainer, IMonomer, ICloneable
    {
        private string monomerName;
        private string monomerType;

        /// <summary>The name of this monomer (e.g. Trp42).</summary>
        public string MonomerName
        {
            get { return monomerName; }
            set
            {
                monomerName = value;
                            }
        }

        /// <summary>The type of this monomer (e.g. TRP).</summary>
        public string MonomerType
        {
            get { return monomerType; }
            set
            {
                monomerType = value;
                            }
        }

        public Monomer()
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Monomer{").Append(GetHashCode());
            if (MonomerName != null)
                sb.Append(", N=").Append(MonomerName);
            if (MonomerType != null)
                sb.Append(", T=").Append(MonomerType);
            sb.Append('}');
            return sb.ToString();
        }

        public override ICDKObject Clone(CDKObjectMap map)
        {
            return base.Clone(map);
        }
    }
}
