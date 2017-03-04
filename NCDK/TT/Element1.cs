
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

using NCDK.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class Element
        : ChemObject, IElement, ICloneable
    {
        internal int? atomicNumber;
        internal string symbol;

        public Element()
            : this(null, null)
        {
        }

        public Element(IElement element)
            : this(element.Symbol, element.AtomicNumber)
        {
        }

        public Element(string symbol)
            : this(symbol, Elements.OfString(symbol).AtomicNumber)
        {
        }

        public Element(string symbol, int? atomicNumber)
            : base()
        {
            this.symbol = symbol;
            this.atomicNumber = atomicNumber;
        }

        public virtual int? AtomicNumber
        {
            get { return atomicNumber; }

            set
            {
                atomicNumber = value;
                NotifyChanged();
            }
        }

        public virtual string Symbol
        {
            get { return symbol; }

            set
            {
                symbol = value;
                NotifyChanged();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Element(").Append(GetHashCode());
            if (Symbol != null)
                sb.Append(", S:").Append(Symbol);
            if (Id != null)
                sb.Append(", ID:").Append(Id);
            if (AtomicNumber != null)
                sb.Append(", AN:").Append(AtomicNumber);
            sb.Append(')');
            return sb.ToString();
        }

        public override bool Compare(object obj)
        {
            var elem = obj as Element;
            if (elem == null)
                return false;
            if (!base.Compare(obj))
                return false;
            return AtomicNumber == elem.AtomicNumber && Symbol == elem.Symbol;
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class Element
        : ChemObject, IElement, ICloneable
    {
        internal int? atomicNumber;
        internal string symbol;

        public Element()
            : this(null, null)
        {
        }

        public Element(IElement element)
            : this(element.Symbol, element.AtomicNumber)
        {
        }

        public Element(string symbol)
            : this(symbol, Elements.OfString(symbol).AtomicNumber)
        {
        }

        public Element(string symbol, int? atomicNumber)
            : base()
        {
            this.symbol = symbol;
            this.atomicNumber = atomicNumber;
        }

        public virtual int? AtomicNumber
        {
            get { return atomicNumber; }

            set
            {
                atomicNumber = value;
            }
        }

        public virtual string Symbol
        {
            get { return symbol; }

            set
            {
                symbol = value;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Element(").Append(GetHashCode());
            if (Symbol != null)
                sb.Append(", S:").Append(Symbol);
            if (Id != null)
                sb.Append(", ID:").Append(Id);
            if (AtomicNumber != null)
                sb.Append(", AN:").Append(AtomicNumber);
            sb.Append(')');
            return sb.ToString();
        }

        public override bool Compare(object obj)
        {
            var elem = obj as Element;
            if (elem == null)
                return false;
            if (!base.Compare(obj))
                return false;
            return AtomicNumber == elem.AtomicNumber && Symbol == elem.Symbol;
        }
    }
}
