
















// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2015-2017  Kazuya Ujihara

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class Isotope
            : Element, IIsotope, ICloneable
    {
        private double? naturalAbundance;
        private double? exactMass;
        private int? massNumber;

        public Isotope(string elementSymbol)
            : base(elementSymbol)
        {
        }

        public Isotope(int atomicNumber, string elementSymbol, int massNumber, double exactMass, double abundance)
            : base(elementSymbol, atomicNumber)
        {
            this.massNumber = massNumber;
            this.exactMass = exactMass;
            this.naturalAbundance = abundance;
        }

        public Isotope(int atomicNumber, string elementSymbol, double exactMass, double abundance)
            : this(elementSymbol, atomicNumber)
        {
            this.exactMass = exactMass;
            this.naturalAbundance = abundance;
        }

        public Isotope(string elementSymbol, int massNumber)
            : base(elementSymbol)
        {
            this.massNumber = massNumber;
        }

        public Isotope(IElement element)
            : base(element)
        {
            var isotope = element as IIsotope;
            if (isotope != null)
            {
                this.exactMass = isotope.ExactMass;
                this.naturalAbundance = isotope.NaturalAbundance;
                this.massNumber = isotope.MassNumber;
            }
        }

        public virtual double? NaturalAbundance
        {
            get { return naturalAbundance; }
            set
            {
                naturalAbundance = value;
                NotifyChanged();
            }
        }

        public virtual double? ExactMass
        {
            get { return exactMass; }
            set
            {
                exactMass = value;
                NotifyChanged();
            }
        }

        public virtual int? MassNumber
        {
            get { return massNumber; }
            set
            {
                massNumber = value;
                NotifyChanged();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Isotope(").Append(GetHashCode());
            if (MassNumber != null)
                sb.Append(", MN:").Append(MassNumber);
            if (ExactMass != null)
                sb.Append(", EM:").Append(ExactMass);
            if (NaturalAbundance != null)
                sb.Append(", AB:").Append(NaturalAbundance);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override bool Compare(object obj)
        {
            var isotope = obj as Isotope;
            return isotope != null && base.Compare(obj)
                && isotope.MassNumber == MassNumber
                && NearlyEquals(isotope.ExactMass, ExactMass)
                && NearlyEquals(isotope.NaturalAbundance, NaturalAbundance);
        }

        private static bool NearlyEquals(double? a, double? b)
        {
            if (a.HasValue != b.HasValue)
                return false;
            if (a.HasValue && b.HasValue)
                if (Math.Abs(a.Value - b.Value) > 0.0000001)
                    return false;
            return true;
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class Isotope
            : Element, IIsotope, ICloneable
    {
        private double? naturalAbundance;
        private double? exactMass;
        private int? massNumber;

        public Isotope(string elementSymbol)
            : base(elementSymbol)
        {
        }

        public Isotope(int atomicNumber, string elementSymbol, int massNumber, double exactMass, double abundance)
            : base(elementSymbol, atomicNumber)
        {
            this.massNumber = massNumber;
            this.exactMass = exactMass;
            this.naturalAbundance = abundance;
        }

        public Isotope(int atomicNumber, string elementSymbol, double exactMass, double abundance)
            : this(elementSymbol, atomicNumber)
        {
            this.exactMass = exactMass;
            this.naturalAbundance = abundance;
        }

        public Isotope(string elementSymbol, int massNumber)
            : base(elementSymbol)
        {
            this.massNumber = massNumber;
        }

        public Isotope(IElement element)
            : base(element)
        {
            var isotope = element as IIsotope;
            if (isotope != null)
            {
                this.exactMass = isotope.ExactMass;
                this.naturalAbundance = isotope.NaturalAbundance;
                this.massNumber = isotope.MassNumber;
            }
        }

        public virtual double? NaturalAbundance
        {
            get { return naturalAbundance; }
            set
            {
                naturalAbundance = value;
            }
        }

        public virtual double? ExactMass
        {
            get { return exactMass; }
            set
            {
                exactMass = value;
            }
        }

        public virtual int? MassNumber
        {
            get { return massNumber; }
            set
            {
                massNumber = value;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Isotope(").Append(GetHashCode());
            if (MassNumber != null)
                sb.Append(", MN:").Append(MassNumber);
            if (ExactMass != null)
                sb.Append(", EM:").Append(ExactMass);
            if (NaturalAbundance != null)
                sb.Append(", AB:").Append(NaturalAbundance);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override bool Compare(object obj)
        {
            var isotope = obj as Isotope;
            return isotope != null && base.Compare(obj)
                && isotope.MassNumber == MassNumber
                && NearlyEquals(isotope.ExactMass, ExactMass)
                && NearlyEquals(isotope.NaturalAbundance, NaturalAbundance);
        }

        private static bool NearlyEquals(double? a, double? b)
        {
            if (a.HasValue != b.HasValue)
                return false;
            if (a.HasValue && b.HasValue)
                if (Math.Abs(a.Value - b.Value) > 0.0000001)
                    return false;
            return true;
        }
    }
}
